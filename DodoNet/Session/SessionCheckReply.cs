using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Overlay;

namespace DodoNet.Session
{
    [Serializable]
    public class SessionCheckReply : OverlayReply
    {
        public bool successVerify = false;

        public SessionCheckReply(SessionCheckRequest request)
            : base(request)
        {
        }

        public void ArrivedAtDestination(Node overlayNode, Message msg)
        {
            // If the send failed, remove the destination node from our routing tables.
            if (!successVerify)
            {
                Node.LogAppendLine("El servidor a cerrado la sesión " + overlayNode.localBind.NodeId);

                overlayNode.NodeFailed(msg.Source);

                // finalizamos la sesión
                if (msg.Destiny.NodeId != null)
                    overlayNode.FinalizeSession(msg.Source.NodeId, true);
            }

            base.ArrivedAtDestination(overlayNode, msg);
        }
    }
}
