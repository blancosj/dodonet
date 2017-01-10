using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Overlay;

namespace DodoNet
{
    public class LocalMessage : Message
    {
        public LocalMessage(Continuation continuation)
            : base(continuation)
        {
        }

        public override NodeBind GetNextHop(Node localNode)
        {
            return localNode.localBind;
        }
    }
}
