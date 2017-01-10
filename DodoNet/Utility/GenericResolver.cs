using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace DodoNet.Utility
{
    /// <summary>
    /// as __type is missing, we need to add this
    /// </summary>
    public class GenericResolver : SimpleTypeResolver
    {
        public GenericResolver() { }

        public override Type ResolveType(string id)
        {
            Type ret = null;
           
            try
            {
                ret = base.ResolveType(id) ?? typeof(Dictionary<string, object>);
            }
            catch { }

            return ret;
        }
    }
}
