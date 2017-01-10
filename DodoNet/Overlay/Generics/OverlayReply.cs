using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Overlay.Generics
{
    [Serializable]
    public abstract class OverlayReply<T> : OverlayBaseMessage
    {
        public string requestId;

        public OverlayReply() { }

        public OverlayReply(T request)
        {
            var request = default(T) as OverlayRequest<T>;
            requestId = request.id;
        }

        public override void ArrivedAtDestination(Node overlayNode, Message m)
        {
        }
    }
}
