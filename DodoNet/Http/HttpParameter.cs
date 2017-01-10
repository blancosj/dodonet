using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace DodoNet.Http
{
    public class HttpParameter
    {
        public string Name;
        public NameValueCollection Values;

        public string Value { get { return Values[0]; } }

        public string RawValue;

        public int TotalValues { get { return Values.Count; } }

        public HttpParameter(string name, string value)
        {
            this.Name = name;

            RawValue = value;

            NameValueCollection tmpValues = new NameValueCollection();
            foreach (string tmpAE in value.Split(new char[] { char.Parse(",") }))
            {
                tmpValues.Add(tmpAE, tmpAE);
            }

            this.Values = new NameValueCollection();
            this.Values.Add(value, value);
        }

        public HttpParameter(string name, NameValueCollection values)
        {
            this.Name = name;
            this.Values = values;
        }
    }
}
