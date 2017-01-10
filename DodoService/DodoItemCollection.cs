using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using DodoNet.Http;
using DodoNet.Tools;
using DodoNet.Web;

namespace DodoService
{
    [Serializable]
    public class DodoItemCollection : List<DodoItem>
    {
        public string GetContentDetailedView()
        {
            var ret = string.Empty;
            var enc = Encoding.GetEncoding(WebConfig.HttpEncodingDefault);

            using (var ms = new MemoryStream())
            using (var w = new XmlTextWriter(ms, enc))
            {
                w.Formatting = Formatting.Indented;
                w.QuoteChar = char.Parse("'");
                w.WriteStartElement("table");
                w.WriteAttributeString("class", "dodoTable");

                foreach (var item in this)
                {
                    w.WriteStartElement("tr");
                    w.WriteStartElement("td");
                    w.WriteStartElement("a");
                    w.WriteAttributeString("href", item.GetFileUrl());
                    w.WriteAttributeString("title", item.fileName);
                    w.WriteString(string.IsNullOrEmpty(item.boxText) ? item.Keywords : item.boxText);
                    w.WriteEndElement();
                    w.WriteEndElement();
                    w.WriteStartElement("td"); w.WriteString(ReadableDataLength.Calculate(item.fileSize)); w.WriteEndElement();
                    w.WriteStartElement("td"); w.WriteString(item.modified.ToShortDateString()); w.WriteEndElement();
                    w.WriteStartElement("td"); w.WriteString(item.fileContentType); w.WriteEndElement();
                    w.WriteEndElement();
                }

                w.WriteEndElement();
                w.Flush();

                ms.Position = 0;

                using (var tmp = Utils.GetCodeToEmbedded(ms, enc))
                {
                    tmp.Position = 0;
                    using (var r = new StreamReader(tmp))
                    {
                        ret = r.ReadToEnd();
                    }
                }
            }

            return ret;
        }

        public string GetContent()
        {
            var ret = string.Empty;

            using (var ms = new MemoryStream())
            using (var w = new StreamWriter(ms))
            using (var reader = new StreamReader(ms))
            {
                foreach (var item in this)
                {
                    w.Write(item.GetContentToEmbedd());
                }

                w.Flush();

                ms.Position = 0;
                ret = reader.ReadToEnd();
            }

            return ret;
        }
    }
}
