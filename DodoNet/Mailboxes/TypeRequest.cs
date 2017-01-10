using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Mailboxes
{
    /// <summary>
    /// Tipos de peticiones
    /// </summary>
    public class TypeRequest
    {
        // tipo de informacion
        public const short InfoClient = 0x0001;
        public const short ListClients = 0x0002;
        public const short Connection = 0x0003;

        // eventos del usuario
        public const short TypeReplyUser = 0x5000;
    }
}
