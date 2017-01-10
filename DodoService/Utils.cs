using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace DodoService
{
    public class Utils
    {
        /// <summary>
        /// pass content with document.writeln
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static MemoryStream GetCodeToEmbedded(MemoryStream s, Encoding enc)
        {
            var c = new Cleaners.Word();
            var ms = (MemoryStream)c.Clean(s);
            ms.Position = 0;

            var ret = new MemoryStream();
            var w = new StreamWriter(ret, enc);
            var r = new StreamReader(ms, enc);

            ret.Position = 0;
            s.Position = 0;
            var line = string.Empty;

            // while (!String.IsNullOrEmpty(line = r.ReadLine()))

            while ((line = r.ReadLine()) != null)
            {
                var tmp = line.Replace("\"", "\\\"");
                w.WriteLine("document.write(\"{0}\");", tmp);
            }

            w.Flush();
            
            return ret;
        }
    }
}
