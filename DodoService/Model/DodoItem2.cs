using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoService
{
    public sealed class DodoItem2
    {
        public byte[] BoxBinary { get; set; }
        public long FileSize { get; set; }
        public DateTime Registered { get; set; }
        public string FileContentType { get; set; }
        public string FileName { get; set; }

        public void SetDodoItem(DodoItem item)
        {
            item.ReadBlobValues();

            item.boxBinary = BoxBinary;
            item.fileSize = FileSize;
            item.modified = Registered;
            item.fileContentType = FileContentType;
            item.fileName = FileName;
        }
    }
}
