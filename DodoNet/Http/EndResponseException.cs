using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet.Http
{
    [global::System.Serializable]
    public class EndResponseException : Exception
    {
        public EndResponseException() { }
        public EndResponseException(string message) : base(message) { }
        public EndResponseException(string message, Exception inner) : base(message, inner) { }
        protected EndResponseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
