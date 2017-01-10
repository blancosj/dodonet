using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet
{
    /// <summary>
    /// Tipos de paquete
    /// </summary>
    public class TypePacket
    {
        public const short KeepAlive = 0x0007;
        public const short PreviousMsgData = 0x0008;
        public const short PartialData = 0x0009;

        public const short PreviousMsgFileStream = 0x0011;
        public const short PartialFileStream = 0x0018;

        public const short PreviousMsgDataFileStream = 0x0012;
        public const short PartialDataFileStream = 0x0019;

        public const short PreviousMsgStream = 0x0010;

        public const short PacketReply = 0x0014;
        public const short PacketRequest = 0x0016;
        public const short MsgRequest = 0x0017;


        // eventos del usuario
        public const short TypeUser = 0x5000;
    }
}
