using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Http;
using DodoNet.Web;

namespace DodoService.HttpMethods
{
    public class ProxyBox : IHttpMethod
    {
        public static string ProxyPageBox = "proxybox.aspx";

        #region IHttpMethod Members

        public bool MustProcess()
        {
            var req = WebApplication.CurrentContext.Request;
            bool ret = req.Url.IndexOf(FileGet.PageFileGet) > 0;
            return ret;
        }

        public void Process(WebApplication app)
        {
            var reply = WebApplication.CurrentContext.Reply;
            reply.RedirectTo(ProxyPageBox);
        }

        #endregion
    }
}
