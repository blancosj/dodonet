using System;
using System.Collections.Generic;
using System.Text;

using DodoNet.Overlay;

namespace DodoNet.Session
{
    public interface ISessionRequest
    {
        string Auth { get; }
        NodeBind NodeBindApplicant { get; set; }
        NodeBind NodeBindRemote { get; set; }
        ISessionReply SessionReply { get; set; }
        void Dispose(bool failed);
    }
}
