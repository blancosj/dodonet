using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace DodoNet.Extensions
{
    public static class StringCollectionExtensions
    {
        /// <summary>
        /// Preguntar si contiene el siguiente string ignorando
        /// mayúsculas y minúsculas
        /// </summary>
        /// <param name="coll"></param>
        /// <param name="expr">String a buscar</param>
        /// <returns></returns>
        public static bool ContainsIC(this StringCollection coll, string expr)
        {
            bool ret = false;

            foreach (var s in coll)
            {
                ret = string.Equals(s, expr, StringComparison.InvariantCultureIgnoreCase);
                if (ret)
                    break;
            }

            return ret;
        }

        public static string Join(this StringCollection coll, string separator)
        {
            string ret = string.Empty;
            
            string[] tmp = new string[coll.Count];
            coll.CopyTo(tmp, 0);
            ret = string.Join(separator, tmp);

            return ret;
        }

        /// <summary>
        /// Agrega todos los elementos de otra colección de tipo StringCollection
        /// </summary>
        /// <param name="coll"></param>
        /// <param name="coll"></param>
        public static void AddRange(this StringCollection coll, StringCollection src)
        {
            foreach (var s in src)
            {
                if (!coll.ContainsIC(s))
                    coll.Add(s);
            }
        }
    }
}
