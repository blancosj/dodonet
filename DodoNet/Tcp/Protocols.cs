using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet
{
    public class Protocols
    {
        public const short Prot_Undefined = 0x0000;
        public const short Prot_FV = 0x0001;
        public const short Prot_Telnet = 0x0002;
        public const short Prot_HTTP = 0x0003;
        public const short Prot_FV_Compress = 0x0004;
    }
}
