using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Session
{
    public enum SessionStates
    {
        disconnected, connected, reconnecting, disposed
    }

    public enum SessionType
    {
        fromOuter, fromInner
    }

    public enum SessionReasonReject
    {
        idrepeated, unknown,
        /// <summary>
        /// razones programadas por el usuario
        /// </summary>
        user = 1000
    }
}
