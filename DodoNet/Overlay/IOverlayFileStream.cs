using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DodoNet.Overlay
{
    public interface IOverlayFileStream
    {
        object Additional { get; set; }
        FileStream FileStream { get; set; }
    }
}
