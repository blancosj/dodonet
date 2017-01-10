using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

using DodoNet.Tools;
using DodoNet.Collections;
using DodoNet.Extensions;
using DodoNet.Web;

namespace DodoNet.Http
{
    public class HttpBody : Stream, IDisposable
    {
        public int state = 0; // -1 init, 0 head, 1 first blank line, 1 content, 2 last blank line

        string contentType;
        public string ContentType 
        {
            get
            {
                return contentType;
            }

            set
            {
                contentType = value;
                message.ContentType = value;
            }
        }

        public string CharSet = WebConfig.HttpEncodingDefault;

        public Encoding EncodingText { get; set; }

        public string ContentDisposition { get; set; }

        public FileAttached File { get; set; }

        public NameValueCollection<HttpField> Fields = new NameValueCollection<HttpField>();

        public override long Length
        {
            get 
            {
                long ret = 0;
                if (ContentStream !=null)
                    ret = ContentStream.Length;
                return ret;
            }
        }

        public ContentEncodings ContentEncoding = ContentEncodings.NORMAL;

        public DateTime LastModified;

        public bool HasContent { get { return ContentStream != null; } }

        public bool IsFile;

        public Stream ContentStream { get; set; }

        IHttpMessage message;

        bool isCompleted = false;
        public bool IsCompleted 
        { 
            get 
            {
                return isCompleted; 
            }

            set
            {
                isCompleted = value;
            }
        }        

        public List<HttpBody> BodyParts = new List<HttpBody>();

        public override long Position
        {
            get
            {
                long ret = 0;
                if (ContentStream != null)
                {
                    ret = ContentStream.Position;
                }
                return ret;
            }

            set
            {
                if (ContentStream != null)
                {
                    ContentStream.Position = value;
                }
            }
        }

        internal HttpBody(IHttpMessage message)
        {
            this.message = message;

            ContentType = message.ContentType;
            EncodingText = message.ContentTransferEncoding;
            CharSet = EncodingText.WebName;
        }

        internal HttpBody()
        {
            CharSet = WebConfig.HttpEncodingDefault;
            EncodingText = Encoding.GetEncoding(CharSet);
        }

        public void Empty()
        {
            if (ContentStream != null)
            {
                ContentStream.Dispose();
                ContentStream = null;
            }
        }

        public void CreateStream()
        {
            if (ContentStream == null)
            {
                HttpField field = Fields[HttpConst.ContentDisposition];

                if (field != null)
                {
                    HttpParameter param = field.Parameters["filename"];

                    if (param == null)
                    {
                        ContentStream = (Stream)new MemoryStream();
                    }
                    else
                    {
                        FileStream tmp = new FileStream(Path.GetTempFileName(),
                            FileMode.OpenOrCreate);
                        ContentStream = (Stream)tmp;
                        IsFile = true;

                        File =
                            new FileAttached(param.Value, 
                                Fields[HttpConst.ContentType].Value,
                                tmp.Name);
                    }
                }
                else
                {
                    ContentStream = (Stream)new MemoryStream();
                }
            }
        }

        public void LoadFile(string physicalPathFile, DateTime modifiedSince)
        {
            FileInfo fi = new FileInfo(physicalPathFile);

            if (fi.Exists)
            {
                if (fi.LastWriteTime >= modifiedSince)
                {
                    ContentType = ContentTypes.GetExtensionType(fi.Name);
                    ContentStream = (Stream)(new FileStream(physicalPathFile, FileMode.Open, FileAccess.Read, FileShare.Read));

                    if (ContentType.StartsWith("text"))
                    {
                        // StreamReader reader = new StreamReader(streamContent);
                        // auto detect by browser
                        CharSet = "";
                    }
                }

                LastModified = HttpTime.RoundDate(fi.LastWriteTime);
            }
            else
            {
                throw new FileNotFoundException("File Not Found!", physicalPathFile);
            }
        }

        public void LoadFile(string physicalPathFile)
        {
            LoadFile(physicalPathFile, DateTime.MinValue);
        }

        public void LoadText(string text)
        {
            ContentType = "text/html";

            byte[] buf = EncodingText.GetBytes(text);
            ContentStream = (Stream)(new MemoryStream());
            ContentStream.Write(buf, 0, buf.Length); 
        }

        public void AddText(string text, params object[] args)
        {
            byte[] buf = EncodingText.GetBytes(String.Format(text, args));

            if (ContentStream == null)
            {
                ContentStream = (Stream)(new MemoryStream());
                ContentType = ContentType ?? "text/html";
            }

            ContentStream.Write(buf, 0, buf.Length);
        }

        public void LoadStream(Stream stream, string contentType)
        {
            this.ContentType = contentType;

            CreateStream();
            stream.CopyTo(ContentStream, WebConfig.BufferSize);
        }

        public void SetStream(Stream stream, string contentType)
        {
            this.ContentType = contentType;

            ContentStream = stream;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ContentStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            CreateStream();

            ContentStream.Write(buffer, offset, count);
        }

        public int LastByte()
        {
            var prevPos = ContentStream.Position;
            ContentStream.Position = ContentStream.Length - 1;

            var ret = ContentStream.ReadByte();
            return ret;
        }

        /// <summary>
        /// devuelve el contenido del mensaje en texto plano
        /// </summary>
        /// <returns></returns>
        public string GetPlainText()
        {
            string ret = string.Empty;

            if (ContentStream != null)
            {
                ret = ContentStream.ReadToEnd(
                    message.ContentTransferEncoding,
                    WebConfig.BufferSize);
            }

            return ret;
        }

        public override bool CanRead
        {
            get { return ContentStream == null ? false : ContentStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanWrite
        {
            get { return ContentStream == null ? false : ContentStream.CanWrite; }
        }

        public override void Flush()
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

        public override void WriteByte(byte value)
        {
            CreateStream();

            ContentStream.WriteByte(value);
        }

        #region IDisposable Members

        public new void Dispose()
        {
            if (ContentStream != null)
                ContentStream.Dispose();
        }

        #endregion
    }
}
