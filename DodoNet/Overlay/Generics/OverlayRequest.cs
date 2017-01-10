using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DodoNet.Overlay.Generics
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Type Reply</typeparam>
    [Serializable]
    public abstract class OverlayRequest<T> : OverlayRequest 
        where T : OverlayReply, new()
    {
        public OverlayRequest() 
            : base()
        {
        }

        public OverlayRequest(Node localNode)
            : base(localNode)
        {
        }

        public override void ArrivedAtDestination(Node overlayNode, Message m)
        {
            T reply = new T() {
                RequestId = Id };

            overlayNode.SendReply(reply, m.Source);
        }
    }
}
