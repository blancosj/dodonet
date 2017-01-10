using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Http;

namespace DodoNet.Web
{
    public class WebSession : IWebSession
    {
        #region Properties

        public WebAuth Auth { get; private set; }

        int timeOut = WebConfig.DefaultTimeOutSessionInactiveInSecs;

        DateTime started = DateTime.Now;

        object sync = new object();

        Dictionary<string, object> vars = new Dictionary<string, object>();

        DateTime lastActivity = DateTime.Now;

        public WebApplication App { get; set; }

        #endregion

        public object this[string id]
        {
            get
            {
                lock (sync)
                {
                    if (!vars.ContainsKey(id))
                        vars.Add(id, null);
                    return vars[id];
                }
            }

            set
            {
                lock (sync)
                {
                    if (vars.ContainsKey(id))
                        vars[id] = value;
                    else
                        vars.Add(id, value);
                }
            }
        }

        public WebSession(WebApplication app)
        {
            this.App = app;
            this.Auth = new WebAuth();
        }

        public void AddHttpObject(HttpContext context, IWebObject obj)
        {
            lock (sync)
            {
                if (!vars.ContainsKey(obj.Key))
                    vars.Add(obj.Key, obj);
                else
                    vars[obj.Key] = obj;
            }
        }

        #region Control activity

        /// <summary>
        /// actualizar con el dia y hora del momento de la llamada la var d ctrl
        /// con el momento en el que se produjo el ultimo signo de actividad
        /// </summary>
        public void UpdateActivity()
        {
            lastActivity = DateTime.Now;
        }

        /// <summary>
        /// tiempo en milisegundos de inactividad
        /// </summary>
        /// <returns></returns>
        public double GetInactivityMs()
        {
            TimeSpan tmp = DateTime.Now.Subtract(lastActivity);
            return tmp.TotalMilliseconds;
        }

        public bool IsValid()
        {
            TimeSpan ts = DateTime.Now.Subtract(lastActivity);
            return ts.Seconds < timeOut;
        }

        public Dictionary<string, object> Vars { get { return vars; } set { vars = value; } }

        public int TimeOut { get { return timeOut; } set { timeOut = value; } }

        public DateTime Started { get { return started; } }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            // quitamos la sesión de la aplicación
            App.Sessions.RemoveSession(Auth);

            // destruimos los objetos en vars
            foreach (object obj in vars.Values)
            {
                IDisposable tmp = obj as IDisposable;
                try
                {
                    if (tmp != null) tmp.Dispose();
                }
                catch { }
            }
        }

        #endregion
    }
}
