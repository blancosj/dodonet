using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using DodoNet.Http;
using DodoNet.JQuery;
using DodoNet.Web;

namespace DodoService.HttpMethods
{
    public class SBD : IHttpMethod
    {
        #region IHttpMethod Members

        public bool MustProcess()
        {
            return WebApplication.CurrentContext.Request.Extension == ".sbd";
        }

        public void Process(WebApplication app)
        {
            var dodoApp = (DodoApplication)app;
            var request = WebApplication.CurrentContext.Request;
            var reply = WebApplication.CurrentContext.Reply;

            string destiny = request.Url;

            destiny = destiny.Replace("/", @"\");
            if (destiny.StartsWith(@"\")) destiny = destiny.Substring(1);
            request.PhysicalPathFile = Path.Combine(dodoApp.DefaultFolder, destiny);

            reply.LoadFile("viewersbd.aspx");
        }

        #endregion
    }
}
