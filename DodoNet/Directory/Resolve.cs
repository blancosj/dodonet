using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Overlay;

namespace DodoNet
{
    [Serializable]
    public class ResolveRequest : OverlayRequest
    {
        NodeId requestNodeId;

        public ResolveRequest(NodeId requestNodeId, Node localNode)
            : base(localNode)
        {
            this.requestNodeId = requestNodeId;
        }

        public override void ArrivedAtDestination(Node overlayNode, Message msg)
        {
            ResolveReply reply = new ResolveReply(this);

            NodeBind nodeBind = overlayNode.Directory.ResolveOuter(requestNodeId);

            if (nodeBind != null)
            {
                reply.resolvedNodeBind = nodeBind;
                reply.resolvedOk = true;
            }
            else
            {
                reply.resolvedOk = false;
            }

            overlayNode.SendReply(reply, msg.Source);
        }
    }

    [Serializable]
    public class ResolveReply : OverlayReply
    {
        public NodeBind resolvedNodeBind;
        public bool resolvedOk;

        public ResolveReply(OverlayRequest request)
            : base(request)
        {
        }
    }
}
