using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using DodoNet.Http;

namespace DodoService
{
    public abstract class DodoConverter
    {
        public abstract string AcceptedExt { get; }
        public abstract string ConvertedExt { get; }

        public string ConvertedContentType
        {
            get
            {
                return ContentTypes.GetExtensionType(ConvertedExt);
            }
        }

        public abstract void Convert(Stream source, Stream target, out string log);

        public abstract void Convert(string pathSource, ref string pathTarget, out string log);
    }
}