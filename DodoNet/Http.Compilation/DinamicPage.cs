using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DodoNet.Http.Util;
using DodoNet.Web;

namespace DodoNet.Http.Compilation
{
    public class DinamicPage : IHttpRender, IDisposable
    {
        object me;
        public object Me { get { return me; } set { me = value; } }

        public object Session
        { 
            get 
            {
                // if (context.Session == null)
                //     throw new Exception("No existe una sesión activa.");
                return Context.Session;
            } 
        }

        public HttpRequest Request { get { return Context.Request; } }

        public HttpReply Reply { get { return Context.Reply; } }

        public object App { get { return Context.App; } }

        public Node Server { get { return Context.OverlayNode; } }

        public HttpContext Context { get; set; }

        public StringBuilder OutStream { get; set; }        

        public DinamicPage() { }

        public virtual int Render() { return 0; }

        #region SessionRequest

        /// <summary>
        /// verificar sesion html
        /// </summary>
        public bool CheckSession(bool canCreate)
        {
            return Context.App.OnCheckSession(Context, canCreate);
        }

        #endregion

        #region print

        public void printl(string text)
        {
            OutStream.AppendLine(text);
        }

        public void print(string text)
        {
            OutStream.Append(text);
        }

        public void printl(string text, object arg1)
        {
            OutStream.AppendLine(String.Format(text, arg1));
        }

        public void print(string text, object arg1)
        {
            OutStream.Append(String.Format(text, arg1));
        }

        public void printl(string text, object arg1, object arg2)
        {
            OutStream.AppendLine(String.Format(text, arg1, arg2));
        }

        public void print(string text, object arg1, object arg2)
        {
            OutStream.Append(String.Format(text, arg1, arg2));
        }

        public void printl(string text, object arg1, object arg2, object arg3)
        {
            OutStream.AppendLine(String.Format(text, arg1, arg2, arg3));
        }

        public void print(string text, object arg1, object arg2, object arg3)
        {
            OutStream.Append(String.Format(text, arg1, arg2, arg3));
        }

        public void printl(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            OutStream.AppendLine(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10));
        }

        public void print(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            OutStream.Append(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10));
        }

        public void printl(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16)
        {
            OutStream.AppendLine(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16));
        }

        public void print(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16)
        {
            OutStream.Append(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16));
        }

        public void printl(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17)
        {
            OutStream.AppendLine(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17));
        }

        public void print(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17)
        {
            OutStream.Append(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17));
        }

        public void printl(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18)
        {
            OutStream.AppendLine(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18));
        }

        public void print(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18)
        {
            OutStream.Append(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18));
        }

        public void printl(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19)
        {
            OutStream.AppendLine(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19));
        }

        public void print(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19)
        {
            OutStream.Append(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19));
        }

        public void printl(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20)
        {
            OutStream.AppendLine(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20));
        }

        public void print(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20)
        {
            OutStream.Append(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20));
        }

        public void printl(string text, params object[] args)
        {
            OutStream.AppendLine(String.Format(text, args));
        }

        public void print(string text, params object[] args)
        {
            OutStream.Append(String.Format(text, args));
        }

        public void printobj(IWebObject obj)
        {
            OutStream.Append(obj.Render());
        }

        public void printfile(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, true))
                {
                    char[] buffer = new char[4096];
                    int n = 0;
                    while ((n = sr.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        OutStream.Append(buffer, 0, n);
                    }
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
