using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Http;
using DodoNet.Web;

namespace DodoService.HttpMethods
{
    public class JMX : IHttpMethod
    {
        #region IHttpMethod Members

        public bool MustProcess()
        {
            return WebApplication.CurrentContext.Request.Extension == ".jmx";
        }

        public void Process(WebApplication app)
        {
            HttpContext context = WebApplication.CurrentContext;
            DodoApplication dodoApp = (DodoApplication)app;

            // interpretar las paginas si son de una extension determinada
            using (var aspi = new JMXInterpret(dodoApp, context.Request, context.Reply))
            {
                aspi.Interpret();
            }

            context.Reply.LastModified = DateTime.Now;
            context.Reply.Code = Codes.OK;
        }

        #endregion
    }
}
