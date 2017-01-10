using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Http;
using DodoNet.Web;

namespace DodoService.HttpMethods
{
    public class FileDownload : IHttpMethod
    {
        #region IHttpMethod Members

        public bool MustProcess()
        {
            return WebApplication.CurrentContext.Request.Url.EndsWith("filedownload.dodo");
        }

        public void Process(WebApplication app)
        {
            HttpContext context = WebApplication.CurrentContext;
            DodoApplication dodoApp = (DodoApplication)app;

            var fileDownloadItem =
                dodoApp.CurrentDb.GetItem(dodoApp.CalcUniqueId(context.Request.UriArgs["code"]));

            if (fileDownloadItem != null && fileDownloadItem.HasFile)
            {
                if (dodoApp.Rules.CheckDeniedRules(DodoEnumRules.CAN_FORCE_DOWNLOAD, fileDownloadItem))
                {
                    context.Reply.RedirectTo(FileGet.PageFileGet + "?code=" + dodoApp.CalcUniqueId(context.Request.UriArgs["code"]));
                }
                else
                {
                    context.Reply.ForceDownload(fileDownloadItem.GetFileStream(), fileDownloadItem.fileName);
                    context.Reply.LastModified = fileDownloadItem.modified;
                    context.Reply.Code = Codes.OK;
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