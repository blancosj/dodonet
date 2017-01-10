using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DodoNet.Overlay;
using DodoNet.Http;
using DodoNet.JScript;

using Microsoft.Vsa;
using Microsoft.JScript;

namespace DodoNet.Http.Compilation
{
    public class InterpreterAsmx : HttpInterpreter
    {
        public override string Extension { get { return ".asmx"; } }

        public HttpRequest Request { get { return context.Request; } }
        public HttpReply Reply { get { return context.Reply; } }

        public InterpreterAsmx(HttpContext context)
            : base(context)
        {
        }

        public override void Interpret()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    [Serializable]
    public class InterpreterAsmxException
    {
        public int number;
        public string source;
        public string description;
        public string stackTrace;

        public InterpreterAsmxException(int number, Exception err)
        {
            this.number = number;
            this.source = err.Source;
            this.description = string.Format("{0}\n{1}", err.Message, err.StackTrace);
        }
    }
}
