using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

using DodoNet.Overlay;
using DodoNet.Tools;

namespace DodoNet.Web
{
    public class WebSessionCollection
    {
        object sync = new object();
        public object Sync { get { return sync; } }

        Dictionary<WebAuth, IWebSession> sessions = new Dictionary<WebAuth, IWebSession>();

        public IWebSession this[WebAuth auth]
        {
            get
            {
                IWebSession ret = null;
                try
                {
                    ret = sessions[auth];
                }
                catch { }
                return ret;
            }
        }

        public WebSessionCollection(Node localNode)
        {
            localNode.SetTimer(new WebSessionChecker(this));
        }

        public IWebSession[] GetArray()
        {
            lock (sync)
            {
                IWebSession[] ret = new IWebSession[sessions.Count];
                sessions.Values.CopyTo(ret, 0);
                return ret;
            }
        }

        public void AddSession(IWebSession session)
        {
            lock (sync)
            {
                if (!sessions.ContainsKey(session.Auth))
                    sessions.Add(session.Auth, session);
                // ((HtmlSession)his[auth]).d

            }
        }

        public bool RemoveSession(WebAuth auth)
        {
            lock (sync)
            {
                bool ret = false;
                if (sessions.ContainsKey(auth))
                {
                    sessions[auth].App.OnRemoveSession((WebSession)sessions[auth]);
                    return sessions.Remove(auth);
                }
                return ret;
            }
        }

        public bool IsValidSession(WebAuth auth)
        {
            lock (sync)
            {
                bool ret = false;
                try
                {
                    ret = sessions[auth].IsValid();
                }
                catch { }
                return ret;
            }
        }

        public string GeneratorAuth()
        {
            return string.Format("{0:00000000}", Conversion.Num2Don(DateTime.Now.Ticks));
        }
    }

    public class WebSessionChecker : Timer
    {
        WebSessionCollection sessions;

        public WebSessionChecker(WebSessionCollection sessions)
            : base(100, 300)
        {
            this.sessions = sessions;
        }

        public override void Execute(Node localNode)
        {
            lock (sessions.Sync)
            {
                foreach (IWebSession s in sessions.GetArray())
                {
                    if (!s.IsValid())
                        s.Dispose();
                }
            }
        }
    }
}
