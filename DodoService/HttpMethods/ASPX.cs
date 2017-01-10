using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Http;
using DodoNet.Web;

namespace DodoService.HttpMethods
{
    public class ASPX : IHttpMethod
    {
        #region IHttpMethod Members

        public bool MustProcess()
        {
            return WebApplication.CurrentContext.Request.Extension == ".aspx";
        }

        public void Process(WebApplication app)
        {
            // interpretar las paginas si son de una extension determinada
            ASPXInterpret aspi = new ASPXInterpret(WebApplication.CurrentContext);
            aspi.Interpret();
            aspi.Dispose();
        }

        #endregion
    }
}
