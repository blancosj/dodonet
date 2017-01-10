using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Session
{
    public interface ISessionRequestFinalizeReply
    {
        NodeId JoinedNode { get; set; }
    }
}
