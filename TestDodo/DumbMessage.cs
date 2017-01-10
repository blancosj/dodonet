using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet;
using DodoNet.Overlay;
using DodoNet.Overlay.Generics;

namespace TestDodo
{
    [Serializable]
    public class DumbRequest : OverlayRequest<DumbReply>
    {
        public string Text { get; set; }

        public DumbRequest(Node localNode)
            : base(localNode)
        {
        }
    }

    [Serializable]
    public class DumbReply : OverlayReply
    {
    }
}
