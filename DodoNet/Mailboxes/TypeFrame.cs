using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet
{
    /// <summary>
    /// Tipos de mensaje
    /// </summary>
    public class TypeFrame
    {
        public const short Data = 0x0000;
        public const short FileStream = 0x0002;
        public const short HTTP = 0x0004;
    }
}
