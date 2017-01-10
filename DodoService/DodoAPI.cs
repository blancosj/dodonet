using System;
using System.Collections.Generic;
using System.Text;

using DodoNet;
using DodoNet.Overlay;

namespace DodoService.API
{
    [Serializable]
    public class DodoInsertRecord : OverlayRequest
    {
        public DodoRecord record;

        public DodoInsertRecord(Node localNode)
            : base(localNode)
        {
        }

        public override void ArrivedAtDestination(Node overlayNode, Message msg)
        {
            /*
            DodoService.DodoApplication app = (DodoService.DodoApplication)overlayNode.WebApplicationDefault;
            app.CurrentDb.AddRecord(record);
            DodoReply reply = new DodoReply(this);
            overlayNode.SendReply(msg.source, reply);
            */
        }
    }

    [Serializable]
    public class DodoReply : OverlayReply
    {
        public string result = "error";

        public DodoReply(OverlayRequest request)
            : base(request)
        {
        }
    }
}
