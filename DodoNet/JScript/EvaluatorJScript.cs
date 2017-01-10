using System;
using System.Collections.Generic;
using System.Text;

using DodoNet.Http;
using DodoNet.Web;

namespace DodoNet.JScript
{
    /// <summary>
    /// clase para macro-compilar código. Utilizada para parsear código del tipo ASPX
    /// </summary>
    public class EvaluatorJScript : EvaluatorEngine, IHttpRender, IEvaluator, IDisposable
    {
        StringBuilder m_outstream;

        object m_me;
        public object Me 
        { 
            get 
            { 
                return m_me;
            } 

            set 
            {
                m_me = value;
                AddNewField("Me", value);
            } 
        }

        public StringBuilder outstream { get { return m_outstream; } set { m_outstream = value; } }

        public EvaluatorJScript()
            : base()
        {
            Me = this;
        }

        public virtual void Render() { }

        #region print

        public void printl(string text)
        {
            m_outstream.AppendLine(text);
        }

        public void print(string text)
        {
            m_outstream.Append(text);
        }

        public void printl(string text, object arg1)
        {
            m_outstream.AppendLine(String.Format(text, arg1));
        }

        public void print(string text, object arg1)
        {
            m_outstream.Append(String.Format(text, arg1));
        }

        public void printl(string text, object arg1, object arg2)
        {
            m_outstream.AppendLine(String.Format(text, arg1, arg2));
        }

        public void print(string text, object arg1, object arg2)
        {
            m_outstream.Append(String.Format(text, arg1, arg2));
        }

        public void printl(string text, object arg1, object arg2, object arg3)
        {
            m_outstream.AppendLine(String.Format(text, arg1, arg2, arg3));
        }

        public void print(string text, object arg1, object arg2, object arg3)
        {
            m_outstream.Append(String.Format(text, arg1, arg2, arg3));
        }

        public void printl(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            m_outstream.AppendLine(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10));
        }

        public void print(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            m_outstream.Append(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10));
        }

        public void printl(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16)
        {
            m_outstream.AppendLine(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16));
        }

        public void print(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16)
        {
            m_outstream.Append(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16));
        }

        public void printl(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17)
        {
            m_outstream.AppendLine(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17));
        }

        public void print(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17)
        {
            m_outstream.Append(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17));
        }

        public void printl(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18)
        {
            m_outstream.AppendLine(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18));
        }

        public void print(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18)
        {
            m_outstream.Append(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18));
        }

        public void printl(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19)
        {
            m_outstream.AppendLine(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19));
        }

        public void print(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19)
        {
            m_outstream.Append(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19));
        }

        public void printl(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20)
        {
            m_outstream.AppendLine(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20));
        }

        public void print(string text, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20)
        {
            m_outstream.Append(String.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20));
        }

        public void printl(string text, params object[] args)
        {
            m_outstream.AppendLine(String.Format(text, args));
        }

        public void print(string text, params object[] args)
        {
            m_outstream.Append(String.Format(text, args));
        }

        public void printobj(IWebObject obj)
        {
            m_outstream.Append(obj.Render());
        }

        public void printfile(string file)
        {
        }

        #endregion

        public virtual object DoEval(string expr)
        {
            return base.Eval(expr);
        }

        public virtual void Dispose()
        {
            if (m_outstream != null) m_outstream.Capacity = 0;
            base.Dispose();
        }
    }
}
