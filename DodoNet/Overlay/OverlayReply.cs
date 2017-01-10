using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Overlay
{
    [Serializable]
    public abstract class OverlayReply : OverlayMessage
    {
        public string RequestId { get; set; }

        public OverlayReply() { }

        public OverlayReply(OverlayRequest request)
        {
            RequestId = request.Id;  
        }

        public override void ArrivedAtDestination(Node overlayNode, Message m)
        {
            throw new NotImplementedException();
        }
    }
}
