using System;
using System.Collections.Generic;
using System.Text;

namespace DodoNet.Http
{
    public class FileAttached
    {
        public string Name;
        public string ContentType;
        public string FileTmp;

        public FileAttached(string name, string contentType, string fileTmp)
        {
            this.Name = name;
            this.ContentType = contentType;
            this.FileTmp = fileTmp;
        }
    }
}