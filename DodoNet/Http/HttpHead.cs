using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace DodoNet.Http
{
    public class HttpHead
    {
        public Hashtable Fields { get; set; }
        public Hashtable OtherFields { get; set; }

        public HttpHead()
        {
            Fields = CollectionsUtil.CreateCaseInsensitiveHashtable();
            OtherFields = CollectionsUtil.CreateCaseInsensitiveHashtable();
        }

        public void AddField(HttpField field)
        {
            var tmp = Fields[field.Name];

            if (tmp != null)
            {
                Fields[field.Name] += field.Value;
            }
            else
            {
                Fields.Add(field.Name, field);                
            }
        }

        public void AddField(string text)
        {
            HttpField field = new HttpField();
            field.Parse(text);
            Fields.Add(field.Name, field);
        }

        public void AddField(string name, string param)
        {
            Fields.Add(name, new HttpField()
            {
                Name = name,
                Value = param
            });
        }

        public HttpField GetField(string name)
        {
            return Fields[name] as HttpField;
        }

        public string GetParameter(string name, string param)
        {
            HttpField field = GetField(name);
            return field != null ? field.GetParameter(param) : null;
        }
    }
}
