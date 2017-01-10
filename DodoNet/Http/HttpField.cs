using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

// desiiangel

using DodoNet.Collections;

namespace DodoNet.Http
{
    public class HttpField
    {
        NameValueCollection<HttpParameter> parameters = new NameValueCollection<HttpParameter>();
        public NameValueCollection<HttpParameter> Parameters { get { return parameters; } }

        public string Name;
        public string Value;
        public StringCollection Values = new StringCollection();
        public string RawValue;

        public int TotalValues { get { return Values.Count; } }

        public HttpField()
        {
            
        }

        public HttpField(string name, object value)
        {
            this.Name = name;
            this.Value = value.ToString();
        }

        public void Parse(string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                int colonIndex = data.IndexOf(":");
                if (colonIndex != -1)
                    Name = data.Substring(0, colonIndex);
                else
                    Name = "";

                colonIndex++;

                if (RawValue == null)
                    RawValue = data.Substring(colonIndex).Trim();

                int endingIndex = data.IndexOf(";", colonIndex);

                if (endingIndex != -1)
                {
                    Value = data.Substring(colonIndex, endingIndex - colonIndex).Trim();

                    foreach (string param in data.Substring(endingIndex + 1).Split(new char[] { char.Parse(";") }))
                    {
                        int equalIndex = param.IndexOf('=');
                        string tmpName;

                        if (equalIndex != -1)
                        {
                            tmpName = param.Substring(0, equalIndex).Trim();
                            equalIndex++;
                            string tmpValue = param.Substring(equalIndex, param.Length - equalIndex).Trim();

                            HttpParameter tmpParameter = new HttpParameter(tmpName, tmpValue.Trim(new char[] { char.Parse("\"") }));
                            parameters.Add(tmpName, tmpParameter);
                        }
                        else
                            break;
                    }
                }
                else
                    Value = data.Substring(colonIndex, data.Length - colonIndex).Trim();

                foreach (string param in Value.Split(new char[] { char.Parse(",") }))
                {
                    Values.Add(param);
                }
            }
        }

        public string GetParameter(string parameter)
        {
            return parameters[parameter].Value;
        }

        public override string ToString()
        {
            return string.Format("{0}={1}", Name, Value);
        }
    }
}
