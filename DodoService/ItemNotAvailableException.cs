using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoService
{
    [global::System.Serializable]
    public class ItemNotAvailableException : Exception
    {
        public ItemNotAvailableException() { }
        public ItemNotAvailableException(string message) : base(message) { }
        public ItemNotAvailableException(string message, Exception inner) : base(message, inner) { }
        protected ItemNotAvailableException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
