using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Http;
using DodoNet.Web;

namespace DodoService.HttpMethods
{
    public class FileGet : IHttpMethod
    {
        public static string PageFileGet = "fileget.dodo";

        #region IHttpMethod Members

        public bool MustProcess()
        {
            return WebApplication.CurrentContext.Request.Url.EndsWith(PageFileGet);
        }

        public void Process(WebApplication app)
        {
            var context = WebApplication.CurrentContext;
            var dodoApp = (DodoApplication)app;

            var fileGetItem =
                dodoApp.CurrentDb.GetItem(dodoApp.CalcUniqueId(context.Request.UriArgs["code"]));

            if (fileGetItem != null && fileGetItem.HasFile)
            {
                var newContentType = string.Empty;
                if (dodoApp.Rules.AnyContentTypeMandatory(fileGetItem, out newContentType))
                {
                    var uriProxy = new UriBuilder(context.Request.Uri);
                    uriProxy.Path = string.Format("/{0}", ProxyBox.ProxyPageBox);
                    context.Reply.RedirectTo(uriProxy.ToString());
                }
                else
                {
                    var ub = new UriBuilder(context.Request.Uri);
                    ub.Path = GetBox.PageGetBox;
                    context.Reply.RedirectTo(ub.ToString());
                }
            }
            else
            {
                context.Reply.Code = Codes.NOT_FOUND;
            }
        }

        #endregion
    }
}
