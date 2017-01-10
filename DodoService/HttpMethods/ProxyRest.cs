using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using DodoNet.Http;
using DodoNet;
using DodoNet.Web;
using Newtonsoft.Json;

namespace DodoService.HttpMethods
{
    public class ProxyRest : IHttpMethod
    {
        public Dictionary<string, Type> Models { get; set; }

        public ProxyRest()
        {
            Models = new Dictionary<string, Type>();
            Models.Add("item", typeof(DodoItem));
            Models.Add("search", typeof(DodoItem));
        }

        #region IHttpMethod Members

        public bool MustProcess()
        {
            return GetModel() != null;
        }

        public void Process(WebApplication app)
        {
            var context = WebApplication.CurrentContext;
            var request = context.Request;
            var reply = context.Reply;

            try
            {
                switch (request.Method)
                {
                    case MessageTypes.GET:
                        if (request.UriArgs["skip"] != null ||
                            request.UriArgs["take"] != null)
                            CallGetRangeMethod();
                        else
                            CallGetMethod();
                        break;
                    case MessageTypes.POST:
                        break;
                    case MessageTypes.DELETE:
                        break;
                    case MessageTypes.PUT:
                        break;
                    default:
                        throw new ExecutionEngineException();
                        break;
                }
            }
            catch
            {
                reply.Code = Codes.BAD;
            }
        }

        private Type GetModel()
        {
            Type ret = null;

            var context = WebApplication.CurrentContext;
            var request = context.Request;

            ret = (from item in Models.Where(item => request.Uri.Segments.Length > 1)
                   let tmp = request.Uri.Segments[1].TrimEnd(Char.Parse("/")).Equals(item.Key, StringComparison.InvariantCultureIgnoreCase)
                   where tmp
                   select item.Value).FirstOrDefault();

            return ret;
        }

        private void CallGetRangeMethod()
        {
            var context = WebApplication.CurrentContext;
            var request = context.Request;

            int skip = Convert.ToInt32(request.UriArgs["skip"]);
            int take = Convert.ToInt32(request.UriArgs["take"]);

            int x = 0;
            var key = string.Empty;
            var args = new List<object>();
            for (x = 2; x < request.Uri.Segments.Length; x++)
            {
                var tmp = request.Uri.Segments[x];
                tmp = tmp.TrimEnd(Char.Parse("/"));
                args.Add(request.Uri.Segments[x]);
            }

            var modelName = request.Uri.Segments[1].TrimEnd(Char.Parse("/"));

            var findArgs = new Type[] { typeof(string), typeof(int), typeof(int), typeof(object[]) };
            var minfo = GetType().GetMethod("GetMethod", findArgs);

            if (minfo != null)
                minfo.Invoke(this, new object[] { modelName, skip, take, args.ToArray() });
            else
                throw new ExecutionEngineException();
        }

        private void CallGetMethod()
        {
            var context = WebApplication.CurrentContext;
            var request = context.Request;

            var modelName = request.Uri.Segments[1].TrimEnd(Char.Parse("/"));

            int x = 0;
            var key = string.Empty;
            var args = new List<object>();
            for (x = 2; x < request.Uri.Segments.Length; x++)
            {
                var tmp = request.Uri.Segments[x];
                tmp = tmp.TrimEnd(Char.Parse("/"));
                if (x == 2)
                    key = tmp;
                else
                    args.Add(tmp);
            }

            var findArgs = new Type[] { typeof(string), typeof(string), typeof(object[]) };
            var minfo = GetType().GetMethod("GetMethod", findArgs);

            if (minfo != null)
                minfo.Invoke(this, new object[] { modelName, key, args.ToArray() });
            else
                throw new ExecutionEngineException();
        }

        #endregion

        #region Rest Methods

        public void GetMethod(string model, string key, params object[] args)
        {
            var context = WebApplication.CurrentContext;
            var request = context.Request;
            var reply = context.Reply;
            var dodoApp = (DodoApplication)context.App;

            object item = null;

            string tmpUserId = request.UriArgs["userId"];
            
            switch(model)
            {
                case "item":

                    string folderId = request.UriArgs["folderId"] ?? "0";
                    string searchExpr = request.UriArgs["searchExpr"] ?? string.Empty;

                    item = dodoApp.CurrentDb.SearchMyRecords(
                        Convert.ToInt32(tmpUserId), searchExpr, Convert.ToInt64(folderId));
                    break;
            }

            if (item != null)
            {
                //var ser = new JavaScriptSerializer(new SimpleTypeResolver());
                //var msg = ser.Serialize(item);
                //var msg = JavaScriptConvert.SerializeObject(item);
                var msg = Serialization.Json().Serialize(item);

                reply.LoadText(msg);
                reply.Code = Codes.OK;
            }
            else
                reply.Code = Codes.NOT_FOUND;
        }

        public void GetMethod(string model, int skip, int take, params object[] args)
        {
            var context = WebApplication.CurrentContext;
            var request = context.Request;
            var reply = context.Reply;
            var dodoApp = (DodoApplication)context.App;

            var userId = request.UriArgs["userId"];

            var item = dodoApp.CurrentDb.GetItems(skip, take, userId);

            if (item != null)
            {
                var msg = Serialization.Json().Serialize(item);

                //var msg = JavaScriptConvert.SerializeObject(item);
                reply.LoadText(msg);
                reply.Code = Codes.OK;
            }
            else
                reply.Code = Codes.NOT_FOUND;
        }

        //public void PostMethod(params object[] args)
        //{
        //}

        //public void PutMethod(string key, params object[] args)
        //{
        //}

        //public void DeleteMethod(string key)
        //{
        //}

        #endregion
    }
}
