using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Overlay
{
    public class OverlayError : OverlayReply
    {
        OverlayRequest request;
        Exception exception;

        public OverlayError(OverlayRequest request, Exception exception)
            : base(request)
        {
            this.request = request;
            this.exception = exception;
        }

        public override void ArrivedAtDestination(Node overlayNode, Message m)
        {
            throw new NotImplementedException();
        }

        public Exception GetException()
        {
            return null;
        }
    }
}
