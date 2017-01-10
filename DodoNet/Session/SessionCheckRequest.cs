using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Overlay;

namespace DodoNet.Session
{
    [Serializable]
    public class SessionCheckRequest : OverlayRequest
    {
        public string auth;

        public SessionCheckRequest(Node localNode)
            : base(localNode)
        {
        }

        public override void ArrivedAtDestination(Node overlayNode, Message msg)
        {
            SessionCheckReply reply = new SessionCheckReply(this);
            reply.successVerify = overlayNode.Sessions.ExistsById(auth);

            if (!reply.successVerify)
            {
                Node.LogAppendLine("El nodo '" + auth + "' no está conectado");
            }

            overlayNode.SendReply(reply, msg.Source);
        }
    }
}
