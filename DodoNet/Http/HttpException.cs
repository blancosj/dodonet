using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Http
{
    [global::System.Serializable]
    public class HttpException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public Codes Code { get { return Reply.Code; } }
        public string CodeReason { get { return Reply.CodeReason; } }
        public HttpReply Reply { get; private set; }

        public HttpException() { }

        public HttpException(HttpReply reply) 
            : base(reply.Body == null ? reply.CodeReason : reply.Body.GetPlainText())
        {
            this.Reply = reply;
        }

        public HttpException(HttpReply reply, string extraInfo)
            : base(string.Format("{0} {1}", 
                reply.Body == null ? reply.CodeReason : reply.Body.GetPlainText(), 
                extraInfo))          
        {
            this.Reply = reply;
        }

        public HttpException(string message) : base(message) { }
        public HttpException(string message, Exception inner) : base(message, inner) { }
        protected HttpException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
