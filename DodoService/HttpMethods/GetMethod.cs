using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using DodoNet.Http;
using DodoNet.Web;

namespace DodoService.HttpMethods
{
    public class GetMethod : IHttpMethod
    {
        #region IHttpMethod Members

        public bool MustProcess()
        {
            var tmp = WebApplication.CurrentContext.Request.Url;
            if (!tmp.EndsWith("/"))
                tmp += "/";

            return tmp.EndsWith("getmethod/");
        }

        public void Process(WebApplication app)
        {
            var context = WebApplication.CurrentContext;
            var dodoApp = (DodoApplication)app;

            if (context.Request.UriArgs["code"] != null)
            {
                DodoItem getMethodItem =
                    dodoApp.CurrentDb.GetItem(
                        dodoApp.CalcUniqueId(context.Request.UriArgs["code"]));

                if (getMethodItem != null && getMethodItem.Visible)
                {
                    var tmp = getMethodItem.GetContentToEmbedd();
                    context.Reply.LoadText(tmp);
                }
                else
                    context.Reply.LoadText(string.Format("document.write('{0}');", "<b>Item not found.</b>"));
            }
            else if (context.Request.UriArgs["keywords"] != null)
            {
                var tmpKeys = context.Request.UriArgs["keywords"];
                var tmpKeys2 = (string)tmpKeys.Clone();

                if (!string.IsNullOrEmpty(context.Request.Referer))
                {
                    var argsRef = System.Web.HttpUtility.ParseQueryString((new Uri(context.Request.Referer)).Query);
                    var reg = new Regex(@"(?={(\w+)).*(?<=})");

                    foreach (Match m in reg.Matches(tmpKeys))
                    {
                        var substKey = argsRef[m.Groups[1].Value];

                        if (!string.IsNullOrEmpty(substKey))
                            tmpKeys2 = tmpKeys2.Replace(m.Groups[0].Value, substKey);
                    }
                }

                var items = dodoApp.CurrentDb.SearchMyItems(
                    context.Request.UriArgs["user"], tmpKeys2);

                if (items.Count > 0)
                {
                    var typeView = ItemTypeView.Normal;
                    var tmpTypeView = context.Request.UriArgs["view"];
                    if (!string.IsNullOrEmpty(tmpTypeView))
                        typeView = (ItemTypeView)Enum.Parse(typeof(ItemTypeView), tmpTypeView, true);

                    context.Reply.PrepareBody();

                    switch (typeView)
                    {
                        case ItemTypeView.Normal:
                            context.Reply.AddText(items.GetContent());
                            break;
                        case ItemTypeView.Detailed:
                            context.Reply.AddText(items.GetContentDetailedView());
                            break;
                    }
                    context.Reply.CompressBody();
                }
            }

            context.Reply.LastModified = DateTime.Now;
            context.Reply.Code = Codes.OK;
        }

        #endregion
    }
}
