using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

using DodoNet.Http.Util;
using DodoNet.Overlay;
using DodoNet.Tools;
using DodoNet.Collections;
using DodoNet.Compression.GZip;
using DodoNet.Extensions;
using DodoNet.Web;

namespace DodoNet.Http
{
    public class HttpRequest : OverlayRequest, IHttpMessage, IDisposable
    {
        #region Head

        public HttpHead Head { get; set; }

        public Stream HeadStream { get; set; }
        
        public HttpBody Body { get ;  set; }

        public bool pendingHead = true;

        public bool IsCompleted { get; set; }

        public string OriginalRequestUrl { get; set; }        
        public string SessionAuth;

        string url = string.Empty;
        public string Url { get { return url; } set { ProcessUrl(value); } }

        public Uri Uri { get; set; }

        public NameValueCollection UriArgs = new NameValueCollection();

        private NameValueCollection form = new NameValueCollection();
        public NameValueCollection Form { get { return form;  } set { form = value; } }

        NameValueCollection<FileAttached> _files = new NameValueCollection<FileAttached>();
        public NameValueCollection<FileAttached> Files { get { return _files; } set { _files = value; } }

        public string PhysicalPathFile;

        string host;
        public string Host 
        {
            get
            {
                return host;
            }

            set
            {
                if (value.StartsWith("acp"))
                {
                    Uri = new Uri(value);

                    host = Uri.Host;
                }
                else
                {
                    int port = DodoConfig.Port;
                    int pos = value.IndexOf(":");
                    if (pos > -1)
                    {
                        host = value.Substring(0, pos);
                        port = Convert.ToInt32(value.Substring(pos + 1));
                    }
                    else
                        host = value;

                    var pathPart = string.Empty;
                    var queryPart = string.Empty;
                    
                    pos = OriginalRequestUrl.IndexOf("?");
                    if (pos > 0)
                    {
                        pathPart = OriginalRequestUrl.Substring(0, pos);
                        queryPart = OriginalRequestUrl.Substring(pos + 1);
                    }
                    else
                    {
                        pathPart = OriginalRequestUrl;
                    }

                    var ub1 = new UriBuilder(Uri.UriSchemeHttp, host, port);
                    ub1.Path = pathPart;
                    ub1.Query = queryPart;
                    Uri = ub1.Uri;
                }
            }
        }

        public StringCollection Accept;
        public StringCollection AcceptLanguage;
        public StringCollection AcceptEncoding;
        public StringCollection AcceptCharset;
        public string KeepAlive;
        public string Connection;
        public string Referer;
        public string Cookie;
        public DateTime IfUnmodifiedSince;
        public DateTime IfModifiedSince;
        public string IfNoneMatch;

        public MessageTypes Method { get; set; }
        public string Version { get; set; }

        public string ContentType { get; set; }

        public int ContentLength { get; set; }

        public string Extension { get; set; }

        public TransferEncodings TransferEncoding { get; set; }
        
        Encoding contentTransferEncoding = Encoding.GetEncoding(WebConfig.HttpEncodingDefault);
        public Encoding ContentTransferEncoding { get { return contentTransferEncoding; } set { contentTransferEncoding = value; } }

        public string User;
        public string Password;

        bool isFirefoxBrowser;
        public bool IsFirefoxBrowser { get { return isFirefoxBrowser; } }

        bool isMSIEBrowser;
        public bool IsMSIEBrowser { get { return isMSIEBrowser; } }

        List<HttpBody> bodyParts = new List<HttpBody>();
        public List<HttpBody> BodyParts { get { return bodyParts; } set { bodyParts = value; } }

        string userAgent;
        public string UserAgent
        {
            get { return userAgent; }

            set
            {
                userAgent = value;

                isFirefoxBrowser = userAgent.IndexOf("Firefox") > -1;
                isMSIEBrowser = userAgent.IndexOf("MSIE") > -1;
            }
        }

        #endregion

        #region Constructors

        public HttpRequest(Node localNode)
            : base(localNode)
        {
            Init();
        }

        public HttpRequest()
        {
            Init();   
        }

        void Init()
        {
            Head = new HttpHead();

            Method = MessageTypes.GET;

            Accept = new StringCollection();
            Accept.Add("text/html");
            Accept.Add("application/xml");
            Accept.Add("application/xhtml+xml");

            AcceptCharset = new StringCollection();
            AcceptCharset.Add(ContentTransferEncoding.WebName);
            
            AcceptEncoding = new StringCollection();
            AcceptEncoding.Add("gzip");

            AcceptLanguage = new StringCollection();
            AcceptLanguage.Add(global::System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag);
            AcceptLanguage.Add(global::System.Globalization.CultureInfo.CurrentCulture.Parent.IetfLanguageTag);
        }

        #endregion

        #region Members

        public HttpBody CreateBody()
        {
            Body = new HttpBody(this);
            return Body;
        }

        public string GetBoundary()
        {
            return Head.GetParameter(HttpConst.ContentType, HttpConst.Boundary);
        }

        public string GetBoundaryStart()
        {
            return String.Format("--{0}", GetBoundary());
        }

        public string GetBoundaryEnd()
        {
            return String.Format("--{0}--", GetBoundary());
        }

        public void ProcessUrl(string uriNew)
        {
            OriginalRequestUrl = uriNew;

            string tmp = string.Empty;

            // identificador de sesión cookieless
            SessionAuth = UrlUtils.GetSessionId("/", OriginalRequestUrl);
            if (!string.IsNullOrEmpty(SessionAuth))
                tmp = UrlUtils.RemoveSessionId(UrlUtils.GetDirectory(OriginalRequestUrl), OriginalRequestUrl);
            else
                tmp = OriginalRequestUrl;

            var uriTmp = new Uri(tmp, UriKind.RelativeOrAbsolute);
            if (uriTmp.IsAbsoluteUri)
                Host = uriTmp.Host;

            // verificar los argumentos                 
            int pos = tmp.IndexOf("?");
            if (pos > 0)
            {
                UriArgs.Clear();

                NameValueCollection nvc =
                    HttpUtility.ParseQueryString(
                    tmp.Substring(pos + 1), Encoding.Default);
                for (int x = 0; x < nvc.Count; x++)
                {
                    string key = nvc.GetKey(x);
                    if (key != null)
                    {
                        string value = "";
                        if (nvc.GetValues(x).Length > 0)
                        {
                            foreach (string line in nvc.GetValues(x))
                            {
                                if (value.Length > 0)
                                {
                                    value = "\\r\\n";
                                }
                                value += line;
                            }
                        }
                        UriArgs.Add(key, value);
                    }
                }

                tmp = tmp.Substring(0, pos);
            }

            Extension = Path.GetExtension(tmp);

            url = tmp;
        }

        #endregion

        #region Overlay Members

        public override void ArrivedAtDestination(Node overlayNode, Message m)
        {
            /*
            HttpReply reply = new HttpReply(this);
            reply.LoadFile("c:\\eula.1028.txt");
            overlayNode.SendReply(m.source, reply, Connection == "close");
            */

            Node.LogAppendLine("Request Host: {0}", Host);

            WebApplication app = overlayNode.Apps[Host] as WebApplication;
            IWebApplicationBase baseForExt = null;

            Node.LogAppendLine("Total apps: {0}", overlayNode.Apps.GetArray().Length);

            if (app != null)
                Node.LogAppendLine("Found App for: {0}", Host);

            // si no hay ninguna aplicación para procesar la petición
            // intentamos utilizar la aplicación por defecto
            if (app == null)
                app = overlayNode.WebApplicationDefault as WebApplication;

            HttpContext context = new HttpContext(overlayNode, this, app);

            if (app != null)
            {
                /*
                string ext = Path.GetExtension(RequestUrl);
                if (!String.IsNullOrEmpty(ext))
                    app.ExtList.TryGetValue(ext, out baseForExt);
                */
 
                // guardamos el contexto en el thread actual
                WebApplication.CurrentContext = context;

                // ejecutamos la aplicación
                // if (baseForExt == null)
                //  app.GetReply(context);

                app.SetHomePage(context);
                app.GetReply(context);

                // else
                    //baseForExt.GetReply(overlayNode, context);
            }

            overlayNode.SendReply(context.Reply, m.Source, Connection == "close");
        }

        #endregion

        #region IHttpMessage Members

        public void AddField(HttpField field)
        {
            switch (field.Name)
            {
                case HttpConst.Host:
                    Host = field.Value;
                    break;
                case HttpConst.Accept:
                    Accept.AddRange(field.Values);
                    break;
                case HttpConst.AcceptLanguage:
                    AcceptLanguage.AddRange(field.Values);
                    break;
                case HttpConst.AcceptEncoding:
                    AcceptEncoding.AddRange(field.Values);
                    break;
                case HttpConst.AcceptCharset:
                    AcceptCharset.AddRange(field.Values);

                    if (AcceptCharset.Count > 0)
                        ContentTransferEncoding = Encoding.GetEncoding(AcceptCharset[0]);
                    break;
                case HttpConst.KeepAlive:
                    KeepAlive = field.Value;
                    break;
                case HttpConst.Connection:
                    Connection = field.Value;
                    break;
                case HttpConst.UserAgent:
                    UserAgent = field.RawValue;
                    break;
                case HttpConst.Referer:
                    Referer = field.Value;
                    break;
                case HttpConst.Cookie:
                    Cookie = field.Value;
                    break;
                case HttpConst.IfUnmodifiedSince:
                    // lastModified = HtmlTime.TimeHtml2Date(value);
                    break;
                case HttpConst.IfModifiedSince:
                    // se pueden recibir varios parametros "If-Modified-Since: Tue, 10 Oct 2006 13:16:15 GMT; length=13757"
                    IfModifiedSince = HttpTime.TimeHtml2Date(field.RawValue);
                    break;
                case HttpConst.IfNoneMatch:
                    break;
                case HttpConst.ContentType:
                    ContentType = field.Value;
                    break;
                case HttpConst.ContentLength:
                    ContentLength = int.Parse(field.Value);
                    break;
                case HttpConst.ContentTransferEncoding:
                    ContentTransferEncoding = Encoding.GetEncoding(field.Value);
                    break;
                case HttpConst.Authorization:
                    ParserUserAndPassword(field.Value);
                    break;
                default:
                    // currentRequest.RestHeaders.Add(tag.ToLower(), field);
                    // Head.Fields.Add(field.Name, field);
                    break;
            }

            Head.Fields.Add(field.Name, field);
        }

        void ParserUserAndPassword(string data)
        {
            string tmpBase64 = data.Substring(6);
            string tmp = Encoding.ASCII.GetString(Convert.FromBase64String(tmpBase64));
            string[] tmp2 = tmp.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            if (tmp2.Length > 1)
            {
                User = tmp2[0];
                Password = tmp2[1];
            }
        }

        /// <summary>
        /// Antes de enviar la petición se tiene que llamar a este método 
        /// para serializar la cabecera.
        /// ** El body del mensaje lo serializa la clase HttpBody
        /// </summary>
        public void PrepareSend()
        {
            HeadStream = (Stream)new MemoryStream();

            var headWriter = new StreamWriter(HeadStream, Encoding.ASCII);

            // serializamos los valores del formulario
            if (Form.Count > 0 && Body == null)
            {
                Method = MessageTypes.POST;

                Body = new HttpBody(this);

                int x = 0;
                for (x = 0; x < Form.Count; x++)
                {
                    var strVal = HttpUtility.UrlEncode(Form.Get(x));
                    Body.AddText("{0}={1}&", Form.GetKey(x),
                        strVal);
                }

                Body.ContentType = HttpConst.FormUrlEncoded;
            }

            headWriter.WriteLine("{0} {1} {2}",
                Method, Url, HttpConst.HttpVersion);

            headWriter.WriteLine("{0}: {1}", HttpConst.UserAgent, WebConfig.ServerVersion);
            headWriter.WriteLine("{0}: {1}", HttpConst.Host, Host);

            if (Accept.Count > 0)
                headWriter.WriteLine("{0}: {1}", HttpConst.Accept, Accept.Join(","));

            if (AcceptCharset.Count > 0)
                headWriter.WriteLine("{0}: {1}", HttpConst.AcceptCharset, AcceptCharset.Join(","));

            if (AcceptLanguage.Count > 0)
                headWriter.WriteLine("{0}: {1}", HttpConst.AcceptLanguage, AcceptLanguage.Join(","));

            if (AcceptEncoding.Count > 0)
                headWriter.WriteLine("{0}: {1}", HttpConst.AcceptEncoding, AcceptEncoding.Join(","));

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

                    headWriter.WriteLine("Content-Encoding: {0}",
                        cenc);
                }

                if (Body.ContentEncoding == ContentEncodings.GZIP)
                {
                    MemoryStream body = new MemoryStream();
                    GZip.Compress(Body.ContentStream, body);
                    Body.ContentStream = body;
                }

                headWriter.Write("Content-Type: {0}", Body.ContentType);

                if (Body.ContentType.StartsWith("text") && Body.CharSet.Length > 0)
                    headWriter.Write("; charset={0}", Body.CharSet);

                headWriter.WriteLine();

                if (Body.ContentDisposition != null && Body.ContentDisposition.Length > 0)
                    headWriter.WriteLine("Content-Disposition: {0}", Body.ContentDisposition);

                headWriter.WriteLine("Content-Length: {0}", Body.Length);
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

            ConsoleWritePlainText();
        }

        public void ConsoleWritePlainText()
        {
            StreamReader r1 = new StreamReader(HeadStream, Encoding.ASCII);
            HeadStream.Position = 0;
            Console.WriteLine(r1.ReadToEnd());

            if (Body != null)
            {
                StreamReader r2 = new StreamReader(Body.ContentStream);
                Body.ContentStream.Position = 0;
                Console.WriteLine(r2.ReadToEnd());
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (Body != null)
                Body.Dispose();
        }

        #endregion
    }
}
