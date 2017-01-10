using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using DodoNet;
using DodoNet.Overlay;

namespace DodoNet.Session
{
    /// <summary>
    /// Para hacer una peticion de unirse a un Nodo
    /// </summary>
    [Serializable]
    public class SessionRequest : OverlayRequest, ISessionRequest
    {
        public NodeInfo NodeInfoApplicant { get; set; }
        
        public NodeBind NodeBindApplicant { get; set; }

        public NodeBind NodeBindRemote { get; set; }

        public bool Reconnect { get; set; }
        public DateTime LastAttemptReconnection { get; set; }

        DateTime lastActivity;

        public string Auth { get; set; }

        [NonSerialized]
        ISessionReply sessionReply;
        public ISessionReply SessionReply { get { return sessionReply; } set { sessionReply = value; } }

        //[NonSerialized]public Node localNode;
        [NonSerialized]
        SessionStates state;
        public SessionStates State { get { return state; } set { state = value; } }

        [NonSerialized]
        SessionType type;
        public SessionType Type { get { return type; } set { type = value; } }

        public SessionRequest(Node localNode)
            : base(localNode)
        {
            LastAttemptReconnection = DateTime.Now;
            lastActivity = DateTime.Now;
        }

        /// <summary>
        /// Obtener un id unico de sesión
        /// </summary>
        /// <returns></returns>
        protected string GetIdRandom()
        {
            Random rdm = new Random();

            byte[] bytes = new byte[10];
            for (int x = 0; x < bytes.Length; x++)
            {
                bytes[x] = (byte)rdm.Next(100, 122);
            }            

            string tmp = System.Text.Encoding.ASCII.GetString(bytes);

            tmp = tmp.Replace("\0", "");

            string ret = string.Format("{0}-{1}",
                Guid.NewGuid().ToString(),
                tmp).Replace("-", "");

            return ret;
        }

        /// <summary>
        /// ponemos las ips reales. Si está detrás de un NAT queremos la ip de router y no la interna
        /// </summary>
        public void SetRealIP(Message msg)
        {
            // ponemos el bind remote
            NodeBindRemote = msg.Source;
            // cambiamos la ip por de donde salio el mensaje
            NodeBindApplicant.NodeAddress = NodeBindApplicant.NodeAddress.SubstIP(NodeBindRemote.NodeAddress.IPEndPoint.Address);
        }

        public override void ArrivedAtDestination(Node overlayNode, Message msg)
        {
            SessionReply reply = new SessionReply(this);

            lock (overlayNode.Sessions.Sync)
            {
                // comprobamos que el id no existe ya 
                if (!overlayNode.Sessions.ExistsByNodeId(msg.Source.NodeId))
                {
                    // ponemos la ip real cuando es 0.0.0.0 la ip q nos manda el cliente
                    SetRealIP(msg);

                    this.Auth = GetIdRandom();
                    // ((SessionReply)sessionReply).Auth;
                    // Iniciamos la sesion
                    overlayNode.InitiatedSession(this);
                    // indicamos que si se puede conectar
                    reply.AllowJoin = true;
                    reply.Auth = Auth;
                }
                else
                {
                    Console.WriteLine("Rechazada Sesion con: {0} El Id está repetido." + msg.Source);
                    reply.ReasonReject = (int)SessionReasonReject.idrepeated;
                    reply.AllowJoin = false;
                }
            }
            // Respondemos el resultado del intento de inicio de sesion.
            reply.NodeBindReceiver = overlayNode.localBind.Clone();
            reply.NodeInfoReceiver = overlayNode.localInfo;
            overlayNode.SendReply(reply, msg.Source);
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

        #endregion

        #region IDisposable Members

        /// <summary>
        /// método lanzado por Node cuando la sesión establecida con otro nodo se finaliza
        /// si la sesion se ha finalizado por un fallo el parametro failed es igual a true
        /// </summary>
        public virtual void Dispose(bool failed)
        {
            State = SessionStates.disposed;
        }

        #endregion
    }
}
