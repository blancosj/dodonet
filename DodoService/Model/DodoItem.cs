using System;
using System.IO;
using System.Text;
using System.Xml;

using DodoNet.Web;

namespace DodoService
{
    [Serializable]
    public class DodoItem : DodoItemBase
    {
        public bool UploadFile = false;

        [NonSerialized]
        Encoding Enc = Encoding.GetEncoding(WebConfig.HttpEncodingDefault);

        public string Title 
        {
            get
            {
                return string.IsNullOrEmpty(boxText) ? Keywords : boxText;
            }
        }

        public string GetContentToEmbedd()
        {
            string ret = string.Empty;

            using (var ms = GetContentStream())
            {
                using (var tmp = Utils.GetCodeToEmbedded((MemoryStream)ms, Enc))
                {
                    ret = ConvertStreamToString(tmp);
                }
            }

            return ret;
        }

        public string GetAnnounce()
        {
            string ret = string.Empty;

            using (var w = BeginXmlStream())
            {
                w.WriteStartElement("div");
                w.WriteAttributeString("class", "dodoAnnounce");
                w.WriteStartElement("a");
                w.WriteAttributeString("href", GetFileUrl());
                w.WriteAttributeString("class", "dodoFileDownload");
                w.WriteString(string.Format("{0:dd.MM.yy}.- {1}", registered, Title));
                w.WriteEndElement();
                w.WriteEndElement();
                w.Flush();

                EndXmlStream(w);

                using (var tmp = Utils.GetCodeToEmbedded((MemoryStream)w.BaseStream, Enc))
                {
                    ret = ConvertStreamToString(tmp);
                }
            }

            return ret;
        }

        public string GetContent()
        {
            string ret = string.Empty;

            using (var st = GetContentStream())
            {
                ret = ConvertStreamToString(st);
            }

            return ret;
        }

        Stream GetContentStream()
        {
            Stream ret = null;
            
            var w = BeginXmlStream();
            WriteContentStream(w);
            EndXmlStream(w);

            ret = w.BaseStream;

            return ret;
        }

        string ConvertStreamToString(Stream st)
        {
            string ret = string.Empty;
            var r = new StreamReader(st, Enc);
            st.Position = 0;
            ret = r.ReadToEnd();
            return ret;
        }

        XmlTextWriter BeginXmlStream()
        {
            var ret = new XmlTextWriter(new MemoryStream(), Enc);
            ret.QuoteChar = char.Parse("'");
            ret.Formatting = Formatting.Indented;
            return ret;
        }

        void EndXmlStream(XmlTextWriter w)
        {
        }

        public void WriteContentStream(XmlTextWriter w)
        {
            switch (Type)
            {
                case "file":
                    WriteContentOfFile(w);
                    break;
                case "text":
                    WriteContentOfText(w);
                    break;
                case "image":
                    WriteContentOfImage(w);
                    break;
                case "folder":
                    WriteContentOfFolder(w);
                    break;
            }
        }

        void WriteContentOfFile(XmlTextWriter w)
        {
            w.WriteStartElement("div");
            w.WriteAttributeString("class", "dodoText");
            w.WriteStartElement("a");
            w.WriteAttributeString("href", GetFileUrl());
            w.WriteAttributeString("class", "dodoFileDownload");
            w.WriteString(Title);
            w.WriteEndElement();
            w.WriteEndElement();
            w.Flush();
        }

        void WriteContentOfFolder(XmlTextWriter w)
        {
            w.WriteStartElement("div");
            w.WriteAttributeString("class", "dodoFolder");

            w.WriteStartElement("div");
            w.WriteAttributeString("class", "dodoFolderHead");
            w.WriteString(Title);
            w.WriteEndElement();

            w.WriteStartElement("div");
            w.WriteAttributeString("class", "dodoFolderContent");
            
            w.Flush();

            if (App.CurrentDb != null)
            {
                bool anyRecord = false;

                foreach (var item in App.CurrentDb.GetRecordsFromFolder(UniqueId))
                {
                    item.WriteContentStream(w);
                    anyRecord = true;
                }

                if (!anyRecord)
                    w.WriteString("0 Items");
            }

            w.WriteFullEndElement();

            w.WriteEndElement();

            w.Flush();
        }

        void WriteContentOfText(XmlTextWriter w)
        {
            w.WriteStartElement("div");
            w.WriteAttributeString("class", "dodoText");
            w.WriteRaw(boxText);
            w.WriteEndElement();
            w.Flush();
        }

        void WriteContentOfImage(XmlTextWriter w)
        {
            w.WriteStartElement("div");
            w.WriteAttributeString("class", "dodoImage");
            w.WriteStartElement("div");
            w.WriteAttributeString("class", "dodoInnerImage");
            w.WriteRaw(GetMiniImageElementHtml());
            w.WriteStartElement("div");
            w.WriteAttributeString("class", "dodoText");
            w.WriteRaw(boxText);
            w.WriteEndElement();
            w.WriteEndElement();
            w.WriteEndElement();
            w.Flush();
        }

        public Stream GetFileStream()
        {
            ReadBlobValues();

            var ret = new MemoryStream(boxBinary.Length);
            ret.Write(boxBinary, 0, boxBinary.Length);
            return ret;
        }

        public Stream GetMiniImageStream()
        {
            ReadBlobValues();

            var ret = new MemoryStream(this.imageMini.Length);
            ret.Write(imageMini, 0, imageMini.Length);
            return ret;
        }
    }
}
