using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Http;
using DodoNet.Web;

namespace DodoService.HttpMethods
{
    public class GetBox : IHttpMethod
    {
        public static string PageGetBox = "getbox.dodo";

        #region IHttpMethod Members

        public bool MustProcess()
        {
            return WebApplication.CurrentContext.Request.Url.EndsWith(PageGetBox);
        }

        public void Process(WebApplication app)
        {
            try
            {
                var context = WebApplication.CurrentContext;
                var dodoApp = (DodoApplication)app;

                var code = context.Request.UriArgs["code"];

                var fileGetItem = dodoApp.CurrentDb.GetItem(dodoApp.CalcUniqueId(code));

                if (fileGetItem != null && fileGetItem.HasFile)
                {
                    string newContentType;

                    if (dodoApp.Rules.AnyContentTypeMandatory(fileGetItem, out newContentType))
                    {
                        dodoApp.CurrentDb.GetItem2(fileGetItem, newContentType).SetDodoItem(fileGetItem);
                        context.Reply.RedirectTo(
                            string.Format("viewersbd.aspx?{0}", fileGetItem.fileName));
                    }

                    if (context.Request.UriArgs["mini"] != null)
                    {
                        context.Reply.SetStream(fileGetItem.GetMiniImageStream(), fileGetItem.imageMiniContentType);
                    }
                    else
                    {
                        context.Reply.SetStream(fileGetItem.GetFileStream(), fileGetItem.fileContentType);
                    }

                    context.Reply.LastModified = fileGetItem.modified;
                }
                else
                {
                    context.Reply.Code = Codes.NOT_FOUND;
                }
            }
            catch { }
        }

        #endregion
    }
}
