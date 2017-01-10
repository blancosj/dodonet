using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoOpenId.OpenId
{
    public class KeyAssociation
    {
        public string Handle { get; set; }
        public DateTime Created { get; set; }

        public KeyAssociation(string handle)
        {
            this.Handle = handle;
            Created = DateTime.Now;
        }

        public bool IsExpired()
        {
            bool ret = false;
            var tmp = DateTime.Now.Subtract(Created);
            ret = tmp.TotalSeconds < 10;
            return ret;
        }
    }
}
