using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Overlay
{
    [Serializable]
    public abstract class OverlayRequest : OverlayMessage
    {
        public string Id { get; set; }
        public int TimeoutMs { get; set; }

        [NonSerialized]
        RequestAsyncResult async;
        public RequestAsyncResult Async { get { return async; } set { async = value; } }

        public OverlayRequest()
        {
            TimeoutMs = 50000;
        }

        public OverlayRequest(Node localNode)
            : this()
        {
            Id = localNode.RequestReplyTable.GenerateRequestId();
        }

        public override void ArrivedAtDestination(Node overlayNode, Message m)
        {
            throw new NotImplementedException();
        }
    }
}
