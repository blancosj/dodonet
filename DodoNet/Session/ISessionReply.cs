using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Session
{
    public interface ISessionReply
    {
        // inicio de sesión permitido
        bool AllowJoin { get; set; }
        // identificador de sesion
        string Auth { get; set; }
        // razon del rechazo
        int ReasonReject { get; set; }
    }
}
