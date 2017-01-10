using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Http;
using DodoNet.Web;

namespace DodoService.HttpMethods
{
    public class GetItem : IHttpMethod
    {
        #region IHttpMethod Members

        public bool MustProcess()
        {
            return WebApplication.CurrentContext.Request.Url.EndsWith("getitem.dodo");
        }

        public void Process(WebApplication app)
        {
            HttpContext context = WebApplication.CurrentContext;
            DodoApplication dodoApp = (DodoApplication)app;

            DodoItem getItem = dodoApp.CurrentDb.GetItem(dodoApp.CalcUniqueId(context.Request.UriArgs["code"]));

            if (getItem != null && getItem.HasFile)
            {
                context.Reply.LoadText(getItem.GetContentToEmbedd());
                context.Reply.LastModified = DateTime.Now;
                context.Reply.Code = Codes.OK;
            }
            else
            {
                context.Reply.LoadText("Not found item");
                context.Reply.LastModified = DateTime.Now;
                context.Reply.Code = Codes.OK;
            }
        }

        #endregion
    }
}
