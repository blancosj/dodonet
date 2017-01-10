using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Http;
using DodoNet.Web;

namespace DodoService.HttpMethods
{
    public class LastItemsOf : IHttpMethod
    {
        #region IHttpMethod Members

        public bool MustProcess()
        {
            return WebApplication.CurrentContext.Request.Url.EndsWith("lastitemsof");
        }

        public void Process(WebApplication app)
        {
            HttpContext context = WebApplication.CurrentContext;
            DodoApplication dodoApp = (DodoApplication)app;

            var founds = dodoApp.CurrentDb.GetLastItems(
                context.Request.UriArgs["user"],
                context.Request.UriArgs["type"]);

            context.Reply.PrepareBody();

            foreach (var item in founds)
            {
                string tmp = item.GetAnnounce();
                context.Reply.AddText(tmp);
            }

            context.Reply.CompressBody();
            context.Reply.LastModified = DateTime.Now;
            context.Reply.Code = Codes.OK;
        }

        #endregion
    }
}
