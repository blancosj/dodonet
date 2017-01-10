using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using DodoNet.Http;
using DodoNet.Http.Util;
using DodoNet.Overlay;
using DodoNet.Tools;
using DodoNet.Utility;

namespace DodoNet.Web
{
    public class WebApplication : App, IWebApplicationBase, IDisposable
    {
        static LocalDataStoreSlot secSlot = Thread.GetNamedDataSlot("Context");

        /// <summary>
        /// HttpContext actual
        /// </summary>
        public static HttpContext CurrentContext
        {
            get
            {
                return Thread.GetData(secSlot) as HttpContext;
            }
            set
            {
                Thread.SetData(secSlot, value);
            }
        }

        /// <summary>
        /// HttpContext en ejecución
        /// </summary>
        public HttpContext ExecutionContext { get { return WebApplication.CurrentContext; } }

        /// <summary>
        /// página de inicio
        /// </summary>        
        string home = "index.htm";
        public string Home { get { return home; } set { home = value; } }

        string instanceId = Conversion.Bytes2Don(Guid.NewGuid().ToByteArray());
        public string InstanceId { get { return instanceId; } }

        public string DefaultFolder { get; set; }

        /// <summary>
        /// dominio virtual (si nulo es cualquier dominio)
        /// </summary>
        string host;
        public string Host { get { return host; } set { host = value; } }

        protected List<IHttpMethod> Methods = new List<IHttpMethod>();

        Dictionary<string, IWebApplicationBase> extList = new Dictionary<string, IWebApplicationBase>();

        /// <summary>
        /// Extensiones
        /// </summary>
        public Dictionary<string, IWebApplicationBase> ExtList { get { return extList; } set { extList = value; } }

        WebSessionCollection sessions;
        public WebSessionCollection Sessions { get { return sessions; } set { sessions = value; } }

        #region Manage sessions

        /// <summary>
        /// sesión nueva
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnAddSession(HttpContext context)
        {
            WebSession session = new WebSession(this);
            Sessions.AddSession(session);
            SetSessionState(context, session);
        }

        /// <summary>
        /// redirigimos a la página con el nodeId de sesión
        /// </summary>
        /// <param name="context"></param>
        /// <param name="session"></param>
        public void SetSessionState(HttpContext context, IWebSession session)
        {
            string url = UrlUtils.InsertSessionId("", (string)session.Auth,
                context.Request.OriginalRequestUrl);

            url = UrlUtils.RemoveArgs(url);

            context.Reply.RedirectTo(url);
        }

        /// <summary>
        /// redirigimos a la página con el nodeId de sesión
        /// </summary>
        /// <param name="nodeAddress"></param>
        /// <param name="context"></param>
        /// <param name="session"></param>
        public void RedirectSessionState(string address, HttpContext context, IWebSession session)
        {
            string url = UrlUtils.InsertSessionId("", (string)session.Auth, address);
            context.Reply.RedirectTo(url);
        }

        /// <summary>
        /// quita el codigo de sesión
        /// </summary>
        /// <param name="context"></param>
        public void RemoveUrlSession(HttpContext context)
        {
            if (!String.IsNullOrEmpty(context.Request.SessionAuth))
            {
                context.Reply.RedirectTo(context.Request.Url);
            }
        }

        /// <summary>
        /// verificar sesión
        /// </summary>
        /// <param name="context"></param>
        public virtual bool OnCheckSession(HttpContext context, bool canCreate)
        {
            bool ret = false;
            if (context.Request.SessionAuth != null)
            {
                if (!Sessions.IsValidSession((WebAuth)context.Request.SessionAuth))
                    OnInvalidatedSession(context);
                else
                {
                    // la sesión es válida por lo cual actualizamos la actividad
                    context.Session.UpdateActivity();
                    ret = true;
                }
            }
            else if (canCreate)
                OnAddSession(context);

            return ret;
        }

        /// <summary>
        /// sesión inválida
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnInvalidatedSession(HttpContext context)
        {
            // si la sesión existe le mandamos a la página de inicio
            context.Reply.RedirectTo("/");
        }

        /// <summary>
        /// finalizada session
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnRemoveSession(WebSession session)
        {
        }

        #endregion

        public void SetHomePage(HttpContext context)
        {
            if (context.Request.Url == "/")
            {
                var tmp = context.Request.OriginalRequestUrl;

                int pos = tmp.IndexOf("/");
                if (pos > -1)
                    tmp = tmp.Insert(pos + 1, Home);

                context.Request.Url = tmp;
            }
        }

        /// <summary>
        /// gestionar petición y mandar respuesta
        /// </summary>
        /// <param name="overlayNode"></param>
        /// <param name="context"></param>
        public virtual void GetReply(HttpContext context)
        {
            Node.LogAppendLine(context.Request.Uri.ToString());
            
            string destiny = context.Request.Url;

            destiny = destiny.Replace("/", @"\");
            if (destiny.StartsWith(@"\")) destiny = destiny.Substring(1);
            context.Request.PhysicalPathFile = Path.Combine(DefaultFolder, destiny);
            Node.LogAppendLine(context.Request.PhysicalPathFile);
            context.Reply.LoadFile();
        }

        public override void NewNeighbour(NodeBind nodeBind)
        {
        }

        public override void NeighbourRemoved(NodeBind nodeBind, bool failure)
        {
        }

        public override void MessageReceived(OverlayMessage msg)
        {
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
