using System;
using System.IO;
using System.Text;

using DodoNet.Http;
using DodoNet.Web;
using DodoNet.JQuery;

using Newtonsoft.Json;

namespace DodoService.HttpMethods
{
    class JXP : IHttpMethod
    {
        #region IHttpMethod Members

        public bool MustProcess()
        {
            return WebApplication.CurrentContext.Request.Extension == ".jxp";
        }

        public void Process(WebApplication app)
        {
            HttpContext context = WebApplication.CurrentContext;
            DodoApplication dodoApp = (DodoApplication)app;

            string method = Path.GetFileNameWithoutExtension(context.Request.Url);
            object answer = null;

            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);

            string idCallback = "";

            idCallback = context.Request.UriArgs["callback"];
            DodoSession s = null;
            if (context.Request.UriArgs["auth"] != null)
                s = app.Sessions[(WebAuth)context.Request.UriArgs["auth"]] as DodoSession;

            switch (method)
            {
                case "checkFile":

                    EnumStatusPoweq2 status = EnumStatusPoweq2.NOT_EXISTS;

                    try
                    {
                        var item = dodoApp.CurrentDb.GetItem(dodoApp.CalcUniqueId(context.Request.UriArgs["code"]));

                        string newContentType;

                        if (dodoApp.Rules.AnyContentTypeMandatory(item, out newContentType))
                        {
                            dodoApp.CurrentDb.GetItem2(item, newContentType).SetDodoItem(item);
                            status = dodoApp.CurrentDb.GetItem2Status(item, item.fileContentType);
                        }
                        else
                            status = EnumStatusPoweq2.AVAILABLE;
                        
                    }
                    catch (ItemNotFoundException) { }
                    catch (ItemNotAvailableException) { }

                    var resp = new JSONP(idCallback);
                    resp.PayLoad = JavaScriptConvert.SerializeObject(new string[] { status.ToString() });
                    writer.Write(resp.GetMessage());
                    break;
            }

            // mandamos la respuesta
            context.Reply.LoadText(sb.ToString());

            context.Reply.LastModified = DateTime.Now;
            context.Reply.Code = Codes.OK;
        }

        #endregion
    }
}
