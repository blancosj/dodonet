using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Http;

using Newtonsoft.Json;

using DodoNet.Web;

namespace DodoService.HttpMethods
{
    public class SaveItem : IHttpMethod
    {
        #region IHttpMethod Members

        public bool MustProcess()
        {
            return WebApplication.CurrentContext.Request.Url.EndsWith("saveitem.dodo");
        }

        public void Process(WebApplication app)
        {
            HttpContext context = WebApplication.CurrentContext;
            DodoApplication dodoApp = (DodoApplication)app;

            DodoItem record = dodoApp.CurrentDb.ModifyRecordWithForm();
            string tmp = JavaScriptConvert.SerializeObject(record);
            context.Reply.LoadText(tmp);

            context.Reply.LastModified = DateTime.Now;
            context.Reply.Code = Codes.OK;
        }

        #endregion
    }
}
