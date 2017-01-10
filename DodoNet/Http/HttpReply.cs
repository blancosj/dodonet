using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using DodoNet.Overlay;
using DodoNet.Compression.GZip;
using DodoNet.Extensions;
using DodoNet.Web;

namespace DodoNet.Http
{
    public class HttpReply : OverlayReply, IHttpMessage, IDisposable
    {
        #region Properties

        Codes code = Codes.OK;
        public Codes Code
        {
            get { return code; }

            set
            {
                switch (value)
                {
                    case Codes.NOT_MODIFIED:
                        if (Body != null)
                            Body.Empty();
                        break;
                }
                code = value;
            }
        }

        public string Connection = "keep-alive";

        public string Location { get; set; }

        public HttpHead Head { get; set; }

        public Stream HeadStream { get; set; }

        public DateTime LastModified
        {
            get
            {
                return Body.LastModified;
            }

            set
            {
                if (value <= IfModifiedSince)
                    Code = Codes.NOT_MODIFIED;
                Body.LastModified = value;
            }
        }

        public DateTime IfModifiedSince;

        public string PhysicalPathFile;

        public Encoding ContentTransferEncoding { get; set; }

        NameValueCollection form = new NameValueCollection();
        public NameValueCollection Form { get { return form; } set { form = value; } }

        public IHttpRender Render { get; set; }

        public bool ForcedFileDownload { get; private set; }

        public bool LoadedStream { get; private set; }

        public HttpBody Body { get; set; }

        public HttpRequest Request { get; set; }

        public string Version { get; set; }

        public MessageTypes Method { get; set; }

        public string Url { get; set; }

        public string CodeReason { get; set; }

        public string ContentType { get; set; }

        public bool IsCompleted { get; set; }

        public int ContentLength { get; set; }

        public string Server { get; set; }

        public TransferEncodings TransferEncoding { get; set; }

        #endregion

        public HttpReply(HttpRequest request)
            : base(request)
        {
            this.Request = request;

            Head = new HttpHead();
            IfModifiedSince = request.IfModifiedSince;
            ContentTransferEncoding = request.ContentTransferEncoding;
            ContentTransferEncoding = Encoding.GetEncoding(WebConfig.HttpEncodingDefault);
        }

        public HttpReply()
        {
            Head = new HttpHead();
            ContentTransferEncoding = Encoding.GetEncoding(WebConfig.HttpEncodingDefault);
        }

        public void RedirectTo(string url)
        {
            Location = url;
            Code = Codes.TEMPORARY_REDIRECT;
        }

        public void RedirectTo(string url, bool endResponse)
        {
            RedirectTo(url);

            if (endResponse)
                throw new EndResponseException();
        }

        /// <summary>
        /// load file of physicalPathFile request
        /// </summary>
        public void LoadFile()
        {
            LoadFile(Request.PhysicalPathFile);
        }

        /// <summary>
        /// cargar un archivo dado
        /// </summary>
        /// <param name="physicalPathFile"></param>
        public void LoadFile(string physicalPathFile)
        {
            try
            {
                PrepareBody();
                Body.LoadFile(physicalPathFile);
                LastModified = Body.LastModified;
                CompressBody();
            }
            catch (FileNotFoundException)
            {
                Code = Codes.NOT_FOUND;
            }
        }

        public void LoadStream(Stream stream, string contentType)
        {
            LoadedStream = true;

            PrepareBody();
            Body.LoadStream(stream, contentType);
            CompressBody();
        }

        public void SetStream(Stream stream, string contentType)
        {
            PrepareBody();
            Body.ContentType = contentType;
            Body.ContentStream = stream;
            CompressBody();
        }

        public void ForceDownload(Stream stream, string fileName)
        {
            PrepareBody();
            Body.ContentStream = stream;
            Body.ContentType = "application/octet-stream";
            Body.ContentDisposition = string.Format("Content-Disposition: attachment; filename={0}", fileName);
        }

        public void LoadText(string text)
        {            
            PrepareBody();
            Body.ContentType = "text/html";
            Body.LoadText(text);
            CompressBody();
        }

        public void AddText(string text, params object[] args)
        {
            Body.AddText(text, args);
        }

        public void CompressBody()
        {
            if (Body.ContentType.StartsWith("text") && Request.Accept.ContainsIC("gzip"))
                Body.ContentEncoding = ContentEncodings.GZIP;
        }

        public void Write(string text, params object[] args)
        {
            Render.print(text, args);
        }

        public void PrepareBody()
        {
            if (Body != null)
                Body.Dispose();

            Body = new HttpBody(this);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (Body != null)
                Body.Dispose();

            if (HeadStream != null)
                HeadStream.Dispose();
        }

        #endregion

        #region IHttpMessage Members

        void IHttpMessage.AddField(HttpField field)
        {
            switch (field.Name)
            {
                case HttpConst.ContentType:
                    ContentType = field.Value;
                    break;
                case HttpConst.ContentLength:
                    ContentLength = int.Parse(field.Value);
                    break;
                case HttpConst.Server:
                    Server = field.Value;
                    break;
                case HttpConst.Location:
                    Location = field.Value;
                    break;
                case HttpConst.TransferEncoding:
                    TransferEncoding = (TransferEncodings)Enum.Parse(
                        typeof(TransferEncodings), 
                        field.Value, true);
                    break;
                default:
                    break;
            }

            Head.AddField(field);
        }

        #endregion

        public DodoNet.Collections.NameValueCollection<FileAttached> Files
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

        public string GetBoundaryStart()
        {
            throw new NotImplementedException();
        }

        public string GetBoundaryEnd()
        {
            throw new NotImplementedException();
        }

        public HttpBody CreateBody()
        {
            Body = new HttpBody(this);
            return Body;
        }

        public List<HttpBody> BodyParts
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

        public void PrepareSend()
        {
            HeadStream = (Stream)new MemoryStream();
            StreamWriter headWriter = new StreamWriter(HeadStream, Encoding.ASCII);

            string codeReason = HttpConst.CodeReasons[Code];

            headWriter.WriteLine("{0} {1} {2}",
                HttpConst.HttpVersion, (int)Code, codeReason);

            if (Location != null)
                headWriter.WriteLine("Location: {0}", Location);

            headWriter.WriteLine("Server: {0}", WebConfig.ServerVersion);
            headWriter.WriteLine("Connection: {0}", Connection);
            headWriter.WriteLine("Date: {0}", HttpTime.HtmlNow());

            if (Body != null && Body.HasContent)
            {
                if (Body.ContentEncoding != ContentEncodings.NORMAL)
                {
                    string cenc = "";

                    switch (Body.ContentEncoding)
                    {
                        case ContentEncodings.GZIP:
                            cenc = "gzip";
                            break;
                        case ContentEncodings.DEFLATE:
                            cenc = "deflate";
                            break;
                    }

                    headWriter.WriteLine("Content-Encoding: {0}", cenc);
                }

                if (Body.ContentEncoding == ContentEncodings.GZIP)
                {
                    MemoryStream body = new MemoryStream();
                    GZip.Compress(Body.ContentStream, body);
                    Body.ContentStream = body;
                }

                if (LastModified != null)
                {
                    headWriter.WriteLine("Last-Modified: {0}",
                        HttpTime.Date2TimeHtml(LastModified));
                }

                headWriter.Write("Content-Type: {0}", Body.ContentType);

                if (Body.ContentType.StartsWith("text") && Body.CharSet.Length > 0)
                    headWriter.Write("; charset={0}", Body.CharSet);

                headWriter.WriteLine();

                if (Body.ContentDisposition != null && Body.ContentDisposition.Length > 0)
                    headWriter.WriteLine("Content-Disposition: {0}", Body.ContentDisposition);

                headWriter.WriteLine("Content-Length: {0}", Body.Length);

                // BUG para MSIE para imagenes en CSS
                if (Body.ContentType.StartsWith("image") && Request.IsMSIEBrowser)
                {
                    headWriter.WriteLine("Expires: {0}", HttpTime.Date2TimeHtml(DateTime.Now.AddHours(2)));
                }

                headWriter.WriteLine("Cache-Control: {0}", "public, must-revalidate, max-age = 1");
            }
            else
            {
                headWriter.WriteLine("Content-Length: {0}", 0);
            }

            foreach (object item in Head.Fields.Values)
            {
                var field = (HttpField)item;
                switch (field.Name)
                {
                    default:
                        headWriter.WriteLine("{0}: {1}", field.Name, field.Value);
                        break;
                }
            }

            headWriter.WriteLine();
            headWriter.Flush();
        }
    }
}
