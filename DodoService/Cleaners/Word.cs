using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using DodoNet.Http;
using DodoNet.Web;

namespace DodoService.Cleaners
{
    public class Word : ICleaner
    {
        #region ICleaner Members

        delegate void MyDelegate();

        public System.IO.Stream Clean(System.IO.Stream st)
        {
            MemoryStream ret = new MemoryStream();

            Encoding e = Encoding.GetEncoding(WebConfig.HttpEncodingDefault);
            StreamReader r = new StreamReader(st, e);

            st.Position = 0;
            string tmp = r.ReadToEnd();
            /*
            Regex rex = new Regex(@"\<(?<inner>(?<tag>\w+)[^>]*)[>]", 
                RegexOptions.Compiled | RegexOptions.Multiline);

            string tmpNew = string.Empty;

            MatchEvaluator meval = delegate(Match m)
                {
                    string val = m.Value;                                       

                    bool found = false;
                    foreach (string item in "meta;link".Split(char.Parse(";")))
                    {
                        found = m.Groups["tag"].Value.StartsWith(item);

                        if (found)
                        {
                            val = string.Empty;
                            break;
                        }
                    }

                    return val;
                };

            tmpNew = rex.Replace(tmp, meval);

            rex = new Regex(@"\<(?<tag>w:\w+).*(\k<tag>)[>]", 
                RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

            tmpNew = rex.Replace(tmpNew, string.Empty);

            rex = new Regex(@"\<(?<tag>style\w*)(.*)(\k<tag>)[>]",
                RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

            tmpNew = rex.Replace(tmpNew, string.Empty);

            rex = new Regex(@"\<(?<tag>xml\w*)(.*)(\k<tag>)[>]",
                RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

            tmpNew = rex.Replace(tmpNew, string.Empty);
            */

            /*
            tmp = tmp.Replace("\r\n\r\n", "\r\n");

            Regex rex = new Regex(@"<(?<tag>style\w*)>(?<text>.*)</\k<tag>>",
                RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

            tmp = rex.Replace(tmp, p => string.Empty);

            rex = new Regex(@"<(?<tag>w:\w*)>(?<text>.*)</\k<tag>>",
                RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

            myspace.com/jenny_florida

            // public delegate string MatchEvaluator(Match match);            
            */

            tmp = tmp.Replace("\r\n\r\n", "\r\n");

            
            int pos = 0;

            /*
            while ((pos = tmp.IndexOf("<w:", pos)) > 0)
            {
                int tmpPos = 0;
                
                tmpPos = tmp.IndexOf(">", pos);
                pos += 3;
                string wordLookup = tmp.Substring(pos, tmpPos - pos);
                wordLookup = "</w:" + wordLookup + ">";

                int tmpPosSpace = 0;
                if ((tmpPosSpace = wordLookup.IndexOf(" ")) > 0)
                {
                    wordLookup = wordLookup.Substring(4, tmpPosSpace - 4);
                }                    

                int posY = 0;
                tmpPos++;
                posY = tmp.IndexOf(wordLookup, tmpPos + 1);

                if (posY > 0)
                {                    
                    posY += wordLookup.Length;
                }

                pos -= 3;
                tmp = tmp.Substring(0, pos) + tmp.Substring(posY + 1);
            }
            */

            if (tmp.IndexOf("<w:") > 0)
            {
                string labelBegin = "<style>";
                string labelEnd = "</style>";
                pos = 0;
                while ((pos = tmp.IndexOf(labelBegin, pos)) > 0)
                {
                    int tmpPos = tmp.IndexOf(labelEnd);

                    if (tmpPos > 0)
                    {
                        tmpPos += labelEnd.Length;
                        tmp = tmp.Substring(0, pos) + tmp.Substring(tmpPos);
                    }
                }
            }

            StreamWriter w = new StreamWriter(ret, e);
            w.Write(tmp);
            w.Flush();

            return ret;
        }

        #endregion
    }
}
