using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet;
using DodoNet.Overlay;

namespace DodoNet.Session
{
    [Serializable]
    public class SessionFinalize : OverlayRequest
    {
        public NodeId nodeId;

        public SessionFinalize(Node localNode)
            : base(localNode)
        {
            this.nodeId = localNode.localBind.NodeId;
        }

        public override void ArrivedAtDestination(Node overlayNode, Message msg)
        {
            if (overlayNode.Sessions.ExistsByNodeId(nodeId))
            {
                overlayNode.FinalizeSession(nodeId, false);

                SessionFinalizeReply reply = new SessionFinalizeReply(this);
                overlayNode.SendReply(reply, msg.Source);
            }
            else
            {
                DodoException ex = new DodoException("Error, el identificador {0} no existe como sesión", nodeId.Id);
                overlayNode.SendReply(new OverlayErrorMsg(this, ex), msg.Source);
            }
        }
    }
}
