using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Overlay;

namespace DodoNet.Session
{
    [Serializable]
    public class SessionFinalizeReply : OverlayReply, ISessionRequestFinalizeReply
    {
        public string RequestId { get { return RequestId; } set { RequestId = value; } }

        private NodeId joinedNode;
        public NodeId JoinedNode { get { return joinedNode; } set { joinedNode = value; } }

        public SessionFinalizeReply(SessionFinalize request)
            : base((OverlayRequest)request)
        {
        }
    }
}
