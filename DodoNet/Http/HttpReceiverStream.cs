using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Diagnostics;

using DodoNet.Overlay;
using DodoNet.Http.Util;
using DodoNet.Tools;

namespace DodoNet.Http
{
    public delegate void HttpMessageHandler(Route route, IHttpMessage message);

    public class HttpReceiverStream : Stream
    {        
        MemoryStream streamInner = new MemoryStream();
        long pendingPos;
        Node localNode;
        Route route;
        DateTime lastActivityWrite;
        int stateMultipart;
        List<byte> lineBufferMultipart = new List<byte>();
        HttpBody currentBodyMultipart;

        bool pendingHead = true;
        bool firstField = true;
        bool pendingChunkedContentSize = false;
        int lastChunkedContentSize;
        long currentRead;

        byte[] pendingBuffer;

        IHttpMessage currentMessage;

        public event HttpMessageHandler HandledMessage;

        object sync = new object();

        public HttpReceiverStream(Node localNode, Route route)
        {
            this.localNode = localNode;
            this.route = route;
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            try
            {
                if (pendingBuffer != null && pendingBuffer.Length > 0)
                {
                    streamInner.Write(pendingBuffer, 0, pendingBuffer.Length);
                    pendingBuffer = null;
                }

                streamInner.Write(buffer, offset, count);
                long limit = streamInner.Length;
                streamInner.Position = 0;

                while (streamInner.Position < limit)
                {
                    string line = null;

                    while (pendingHead && (line = Streams.ReadLine(streamInner)) != null)
                    {
                        if (line == "")
                        {
                            pendingHead = false;
                            
                            if (currentMessage.TransferEncoding == TransferEncodings.CHUNKED)
                                pendingChunkedContentSize = true;
                        }
                        else
                            ParseField(line);
                    }

                    while (pendingChunkedContentSize && (line = Streams.ReadLine(streamInner)) != null)
                    {
                        if (line == "")
                        {
                            if (lastChunkedContentSize == 0)
                                ResetPendingComplete();
                        }
                        else
                        {
                            lastChunkedContentSize = Convert.ToInt32(line, 16);
                            if (lastChunkedContentSize > 0)
                            {
                                currentMessage.ContentLength += lastChunkedContentSize;
                                pendingChunkedContentSize = false;
                            }
                        }
                    }

                    if (!pendingHead && !pendingChunkedContentSize)
                    {
                        if (currentMessage.ContentType == HttpConst.MultiPart)
                            ParseContentMultipart();
                        else
                            ParseContentBody();

                    }
                    else if (line == null)
                    {
                        long rd = streamInner.Length - streamInner.Position;

                        if (rd > 0)
                        {
                            pendingBuffer = new byte[rd];
                            streamInner.Read(pendingBuffer, 0, (int)rd);
                        }
                    }
                }

                streamInner.Position = 0;
                streamInner.SetLength(0);
            }
            catch (Exception err)
            {
                throw new BadProtocolException("Bad Protocol Exception", err);
            }

            try
            {
                if (currentMessage.IsCompleted)
                {
                    if (HandledMessage != null)
                        HandledMessage(route, currentMessage);

                    currentMessage = null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Parser Methods

        void ParseContentBody()
        {
            if (!pendingHead)
            {
                if (currentMessage.ContentLength > 0)
                {
                    long lenghtToRead = streamInner.Length - streamInner.Position;

                    if (lenghtToRead > 0)
                    {
                        if (currentMessage.Body == null)
                            currentMessage.CreateBody();

                        long pendingToRead = currentMessage.ContentLength - currentMessage.Body.Length;

                        if (lenghtToRead > pendingToRead)
                            lenghtToRead = pendingToRead;

                        long oldLength = currentMessage.Body.Length;
                        
                        Streams.CopyToEnd(streamInner, currentMessage.Body, lenghtToRead);

                        currentRead += lenghtToRead;

                        if (currentMessage.Body.Length >= currentMessage.ContentLength)
                        {
                            if (currentMessage.TransferEncoding == TransferEncodings.CHUNKED)
                                pendingChunkedContentSize = true;
                            else
                                ResetPendingComplete();
                        }

                        if (currentMessage.IsCompleted &&
                            currentMessage.ContentType == HttpConst.FormUrlEncoded)
                        {
                            currentMessage.Form = HttpUtility.ParseQueryString(currentMessage.Body.GetPlainText(),
                                currentMessage.ContentTransferEncoding);
                        }
                    }
                }
                else if (currentMessage.TransferEncoding != TransferEncodings.CHUNKED)
                {
                    currentMessage.IsCompleted = true;
                    firstField = true;
                    pendingHead = true;
                }
            }
        }

        void ResetPendingComplete()
        {
            currentMessage.IsCompleted = true;
            firstField = true;
            pendingChunkedContentSize = false;
            pendingHead = true;
            currentRead = 0;
        }

        void ParseField(string lineData)
        {
            try
            {
                if (firstField)
                {
                    ParseFirstField(lineData);
                    firstField = false;
                }
                else
                {
                    HttpField field = new HttpField();
                    field.Parse(lineData);
                    currentMessage.AddField(field);
                }
            }
            catch (Exception err)
            {
                throw new BadHttpException(lineData, err);
            }
        }

        void ParseFirstField(string lineData)
        {
            string[] tmp;

            tmp = lineData.Split(new char[] { char.Parse(" ") });

            if (tmp.Length > 0)
            {
                // if first line of message start with 
                // http version is a reply
                switch (tmp[0])
                {
                    case HttpConst.HttpOldVersion:
                    case HttpConst.HttpVersion:
                        currentMessage = new HttpReply()
                        {
                            Version = tmp[0],
                            Code = (Codes)Enum.Parse(typeof(Codes), tmp[1]),
                            CodeReason = string.Join(" ", tmp, 2, tmp.Length - 2),
                        };      
                        break;
                    default:                        
                        currentMessage = new HttpRequest(localNode)
                        {
                            Method = (MessageTypes)Enum.Parse(typeof(MessageTypes), tmp[0]),
                            Url = tmp[1],
                            Version = tmp[2]
                        };

                        var request = currentMessage as HttpRequest;

                        int pos = currentMessage.Url.IndexOf("?");
                        if (pos > -1)
                            request.UriArgs = HttpUtility.ParseQueryString(currentMessage.Url.Substring(pos + 1));                        
                        break;
                }
            }
        }

        void ParseContentMultipart()
        {
            int x = 0;

            int i;
            while (currentRead < currentMessage.ContentLength && (i = streamInner.ReadByte()) > -1)
            {
                x++;
                currentRead++;

                switch (i)
                {
                    case 13:
                        stateMultipart = 1;
                        break;
                    case 10:
                        stateMultipart = stateMultipart == 1 ? 2 : 0;
                        break;
                }

                if (currentBodyMultipart != null && currentBodyMultipart.state == 1)
                {
                    currentBodyMultipart.WriteByte((byte)i);
                }

                lineBufferMultipart.Add((byte)i);

                if (stateMultipart == 2)
                {
                    x = 0;
                    ParseLineMultipart(lineBufferMultipart.ToArray());
                    lineBufferMultipart.Clear();
                    stateMultipart = 0;
                    currentRead = 0;
                }
            }
        }

        void ParseLineMultipart(byte[] lineBytes)
        {
            string line = currentMessage.ContentTransferEncoding.GetString(lineBytes, 0, lineBytes.Length - 2);
            if (line == currentMessage.GetBoundaryStart())
            {
                if (currentBodyMultipart != null)
                {
                    AddCurrentBodyToRequest(lineBytes.Length + 2);
                }

                currentBodyMultipart = currentMessage.CreateBody();
                currentBodyMultipart.state = 0;
            }
            else if (line == currentMessage.GetBoundaryEnd())
            {
                if (currentBodyMultipart != null)
                {
                    AddCurrentBodyToRequest(lineBytes.Length + 2);
                }

                // currentBodyMultipart.streamContent.Se

                // currentMessage.Body.IsCompleted = true;
                currentMessage.IsCompleted = true;
                firstField = true;
                pendingHead = true;
            }
            else if (line == "" && currentBodyMultipart.state == 0)
            {
                currentBodyMultipart.state = 1;
            }
            else if (line != "" && currentBodyMultipart.state == 0)
            {
                HttpField field = new HttpField();
                field.Parse(line);

                currentBodyMultipart.Fields.Add(field.Name, field);
            }
        }

        void AddCurrentBodyToRequest(int totalBytesToCut)
        {
            currentBodyMultipart.state = 2;

            currentBodyMultipart.ContentStream.SetLength(currentBodyMultipart.ContentStream.Length - totalBytesToCut);

            currentMessage.BodyParts.Add(currentBodyMultipart);

            if (currentBodyMultipart.IsFile)
            {
                currentMessage.Files.Add(currentBodyMultipart.Fields[HttpConst.ContentDisposition].Parameters["Name"].Value,
                    currentBodyMultipart.File);
            }
            else
            {
                currentMessage.Form.Add(currentBodyMultipart.Fields[0].Parameters["Name"].Value,
                    currentBodyMultipart.GetPlainText());
            }

            currentBodyMultipart.Dispose();
            currentBodyMultipart = null;
        }

        #endregion
    }
}
