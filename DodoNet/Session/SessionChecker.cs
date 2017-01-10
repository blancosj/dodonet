using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Overlay;

namespace DodoNet.Session
{
    /// <summary>
    /// comprueba de forma periodica que las sesiones a seeds 
    /// y que las sesiones entrantes estén vivas
    /// </summary>
    public class SessionChecker : Timer
    {
        public SessionChecker(long initialDelayMs, long periodMs)
            : base(initialDelayMs, periodMs)
        {
        }

        public override void Execute(Node localNode)
        {
            try
            {
                DateTime checkingNow = DateTime.Now;

                TimeSpan difftime = checkingNow.Subtract(localNode.lastKeepAliveCheck);
                if (difftime.TotalMilliseconds > DodoConfig.KeepAliveTimeout)
                {

                    foreach (ISessionRequest v in localNode.Sessions.GetArray())
                    {
                        SessionRequest session = (SessionRequest)v;

                        if (session.GetInactivityMs() > DodoConfig.KeepAliveTimeout)
                        {
                            Node.LogAppendLine("Mandando mensaje keep-alive a {0}", session.NodeBindApplicant);

                            localNode.SendKeepAlive(session.NodeBindApplicant);
                        }
                    }

                    foreach (ISessionRequest v in localNode.Seeds.GetArray())
                    {
                        SessionRequest session = (SessionRequest)v;

                        if (session.GetInactivityMs() > DodoConfig.KeepAliveTimeout)
                        {
                            Node.LogAppendLine("Mandando mensaje keep-alive a {0}", session.NodeBindRemote);

                            localNode.SendKeepAlive(session.NodeBindRemote, session.Auth);
                        }
                    }

                    // reseteamos
                    localNode.lastKeepAliveCheck = DateTime.Now;
                }

                ArrayList tmpPending = new ArrayList();

                // comprobar las sesiones que debemos de reconectar
                lock (localNode.pendingSessions)
                {
                    foreach (DictionaryEntry ide in localNode.pendingSessions)
                    {
                        SessionRequest session = (SessionRequest)ide.Value;
                        if (session.State == SessionStates.reconnecting)
                        {
                            try
                            {
                                tmpPending.Add(session);
                            }
                            catch (Exception err)
                            {
                                Console.WriteLine("Error en la reconexión {0}", err.Message);
                            }
                        }
                    }
                }

                foreach (object session in tmpPending)
                {
                    localNode.ReconnectSession((SessionRequest)session);
                }
            }
            catch (Exception err)
            {
                try
                {
                    Node.LogAppendLine("Error grave.");
                    Node.LogAppendLine(err);
                }
                catch { }
            }
        }
    }
}
