using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DodoService
{
    [Serializable]
    public class DodoRecord
    {
        public string id;
        public string desc;
        public string item;

        public Stream GetItemStream()
        {
            return new MemoryStream(Encoding.Default.GetBytes(item)) as Stream;
        }
    }
}
