using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Http
{
    [global::System.Serializable]
    public class BadHttpException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public string TextLine { get; private set; }

        public BadHttpException() { }
        public BadHttpException(string textLine, Exception inner) 
            : base(string.Format("{0}{1}", "Parse exception"), inner)
        {
            this.TextLine = textLine;
        }
        public BadHttpException(string message) : base(message) { }
        protected BadHttpException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }    
}
