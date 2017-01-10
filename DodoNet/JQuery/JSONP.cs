using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.JQuery
{
    public class JSONP
    {
        public string CallbackId { get; private set; }
        public string PayLoad { get; set; }

        public JSONP(string callbackId)
        {
            this.CallbackId = callbackId;
        }

        public string GetMessage()
        {
            StringBuilder ret = new StringBuilder();

            ret.AppendFormat("{0}(", CallbackId);

            if (string.IsNullOrEmpty(PayLoad))
                ret.Append("[]");
            else
                ret.Append(PayLoad);

            ret.Append(")");

            return ret.ToString();
        }
    }
}
