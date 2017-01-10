using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Overlay;

namespace DodoNet.Session
{
    /// <summary>
    /// Respuesta a una peticion de union
    /// </summary>
    [Serializable]
    public class SessionReply : OverlayReply, ISessionReply
    {
        public bool AllowJoin { get; set; }

        public string Auth { get; set; }

        int reasonReject;
        public int ReasonReject { get; set; }

        public NodeInfo NodeInfoReceiver { get; set; }
        public NodeBind NodeBindReceiver { get; set; }

        public SessionReply(SessionRequest request)
            : base(request)
        {
        }

        public override void ArrivedAtDestination(Node overlayNode, Message msg)
        {
            ((SessionRequest)overlayNode.RequestReplyTable[RequestId].request).Auth = Auth;

            base.ArrivedAtDestination(overlayNode, msg);
        }
    }
}
