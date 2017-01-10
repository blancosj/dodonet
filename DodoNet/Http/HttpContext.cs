using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Web;

namespace DodoNet.Http
{
    public class HttpContext : IDisposable
    {
        /// <summary>
        /// last exception
        /// </summary>
        public Exception LastException { get; set; }
        /// <summary>
        /// node que interpreta 
        /// </summary>
        public Node OverlayNode { get; set; }
        /// <summary>
        /// petición
        /// </summary>
        public HttpRequest Request { get; set; }
        /// <summary>
        /// respuesta
        /// </summary>
        public HttpReply Reply { get; set; }
        /// <summary>
        /// aplicacion
        /// </summary>
        public WebApplication App { get; set; }

        public bool IsSession { get { return Session != null; } }

        /// <summary>
        /// sesion
        /// </summary>
        public IWebSession Session
        {
            get
            {
                try
                {
                    return App.Sessions[(WebAuth)Request.SessionAuth];
                }
                catch (NullReferenceException) { throw new Exception("No existe una sesión activa."); }
                catch { return null; }
            }
        }

        /// <summary>
        /// lanza último error
        /// </summary>
        public void ThrowLastException()
        {
            if (LastException != null) throw LastException;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="request"></param>
        /// <param name="app"></param>
        public HttpContext(Node overlayNode, HttpRequest request, WebApplication app)
        {
            this.OverlayNode = overlayNode;
            this.Request = request;
            this.App = app;

            Reply = new HttpReply(request);
        }

        /// <summary>
        /// hay una sesión valida y activa
        /// </summary>
        /// <returns></returns>
        public bool IsValidSession() { return Session != null && Session.IsValid(); }

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
