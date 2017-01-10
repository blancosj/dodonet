using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.CodeDom.Compiler;

using DodoNet.JScript;

namespace DodoNet.Http
{
    public class HttpError : HttpReply
    {
        Exception exception;

        public HttpError(Exception exception, HttpRequest request)
            : base(request)
        {
            this.exception = exception;
            
            Code = Codes.SERVER_ERROR;

            CreateContent();
        }

        void CreateContent()
        {
            StringBuilder b = new StringBuilder();
            if (exception is CompilationException)
            {
                CompilationException compilationException = (CompilationException)exception;

                b.Append("<head>");
                b.AppendFormat("<title>{0} {1}</title>", "Error", exception.GetType());
                b.Append("</head>");
                b.Append("<body>");
                b.Append("<pre>");

                string s;
                int line = 0;
                TextReader reader = new StringReader(compilationException.FileText);
                CompilerError ce = (CompilerError)compilationException.Errors[0];
                while ((s = reader.ReadLine()) != null)
                {
                    line++;
                    if (ce.Line == line)
                    {
                        b.AppendFormat("<span style=\"color: blue\">{0}</span></br>", ce.ErrorText);
                        b.Append("<span style=\"color: red\">");
                        // b.AppendFormat("Line {0}: {1}\r\n", line, HtmlEncode(s));
                        // b.Append("</span>");
                    }

                    b.AppendFormat("Line {0}: {1}\r\n", line, HtmlEncode(s));
                    if (line == ce.Line)
                        b.Append("</span>");
                }

                b.Append("</pre>");
                b.Append("</body>\r\n");
            }
            else if (exception is DodoException)
            {
                DodoException ae = (DodoException)exception;
                b.Append("<head>");
                b.AppendFormat("<title>{0} {1}</title>", "Error", ae.typeException);
                b.Append("</head>");
                b.Append("<body>");                
                b.AppendFormat("<h3>Exception type: {0}</h3>", ae.typeException);
                b.AppendFormat("<h4>Description: <b>{0}</b></h4>", ae.error_description);                
                b.AppendFormat("Target site '{0}' happened in '{1}' </br>", ae.error_details, ae.errorLocation);
                b.Append("<pre>");
                b.AppendFormat("Stack: </br>{0}", ae.stackTrace);
                b.Append("</pre>");
                b.Append("</body>\r\n");
            }
            else
            {
                b.Append("<head>");
                b.AppendFormat("<title>{0} {1}</title>", "Error", exception.GetType());
                b.Append("</head>");
                b.Append("<body>");
                b.Append("<pre>");
                b.AppendFormat("<b>{0}</b> </br>", exception.Message);
                b.AppendFormat("{0} </br>", exception.Source);
                b.AppendFormat("{0} </br>", exception.StackTrace);
                b.Append("</pre>");
                b.Append("</body>\r\n");                    
            }
            LoadText(b.ToString());
        }

        static string HtmlEncode(string s)
        {
            if (s == null)
                return s;

            string res = HttpUtility.HtmlEncode(s);
            return res.Replace("\r\n", "<br />");
        }
    }
}
