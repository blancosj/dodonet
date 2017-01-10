using System;
using System.Collections.Generic;
using System.Text;

using DodoNet.Http;
using DodoNet.Http.Compilation;

namespace DodoService
{
    public class ASPXInterpret : InterpreterAspx, IDisposable
    {
        DodoApplication app;
        public DodoApplication App { get { return app; } }

        public ASPXInterpret(HttpContext context)
            : base(context)
        {
            this.app = context.App as DodoApplication;
        }

        #region IDisposable Members

        public void Dispose()
        {
            // throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
