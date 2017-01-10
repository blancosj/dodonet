using System;
using System.IO;
using System.Text;

using DodoNet.Http;
using DodoNet.Web;

using Newtonsoft.Json;

namespace DodoService
{
    public class JMXInterpret : IDisposable
    {
        DodoApplication app;
        HttpRequest request;
        HttpReply reply;

        public JMXInterpret(IWebApplicationBase webApp, HttpRequest request, HttpReply reply)
        {
            this.request = request;
            this.reply = reply;

            app = (DodoApplication)webApp;
        }

        public void Interpret()
        {

            string method = Path.GetFileNameWithoutExtension(request.Url);
            object answer = null;

            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);

            string idcall = "";

            try
            {
                idcall = request.UriArgs["id"];
                DodoSession s = null;
                if (request.UriArgs["auth"] != null)
                    s = app.Sessions[(WebAuth)request.UriArgs["auth"]] as DodoSession;

                switch (method)
                {
                    case "getFolders":
                        int folderId_0 = 0;
                        int.TryParse(request.UriArgs["f"], out folderId_0);
                        var tmpGetFolders = app.CurrentDb.GetFolders(s, folderId_0);

                        writer.WriteLine("var reply = new dbapi.async(" + idcall + ", " + JavaScriptConvert.SerializeObject(tmpGetFolders) + ");");

                        /*
                        if (tmpGetFolders.Count > 0)
                            
                        else
                            writer.WriteLine("var reply = new dbapi.async(" + idcall + ", new Array());");
                        */

                        // writer.WriteLine("dbapi.cb(reply);");
                        break;
                    case "setVisibleRecord":
                        string _code = request.UriArgs["code"];
                        long code = Convert.ToInt64(_code);
                        string _show = request.UriArgs["show"];
                        bool show = Convert.ToBoolean(_show);
                        answer = app.CurrentDb.SetVisibleRecord(code, show, s);
                        writer.WriteLine("var reply = new dbapi.async(" + idcall + ", " + JavaScriptConvert.SerializeObject(answer) + ");");
                        break;
                    case "getRecord":
                        string c = request.UriArgs["code"];
                        answer = app.CurrentDb.GetRecord(c, s);
                        writer.WriteLine("var reply = new dbapi.async(" + idcall + ", " + JavaScriptConvert.SerializeObject(answer) + ");");
                        // writer.WriteLine("dbapi.cb(reply);");
                        break;
                    case "getItem":
                        answer = app.CurrentDb.GetItem(app.CalcUniqueId(request.UriArgs["code"]));
                        writer.WriteLine("var reply = new dbapi.async(" + idcall + ", " + JavaScriptConvert.SerializeObject(answer) + ");");
                        // writer.WriteLine("dbapi.cb(reply);");
                        break;
                    case "getContenPreview":
                        DodoItem previewContent = app.CurrentDb.GetItem(app.CalcUniqueId(request.UriArgs["code"]));

                        var dodoContent = new DodoContent()
                        {
                            Content = previewContent.GetContent()
                        };

                        writer.WriteLine("var reply = new dbapi.async(" + idcall + ", " + JavaScriptConvert.SerializeObject(dodoContent) + ");");
                        // writer.WriteLine("dbapi.cb(reply);");
                        break;
                    case "searchRecords":
                        string search = request.UriArgs["s"];
                        int folderId_1 = 0;
                        int.TryParse(request.UriArgs["f"], out folderId_1);
                        answer = app.CurrentDb.SearchMyRecords(s.userId, search, folderId_1);
                        writer.WriteLine("var reply = new dbapi.async(" + idcall + ", " + JavaScriptConvert.SerializeObject(answer) + ");");
                        // writer.WriteLine("dbapi.cb(reply);");
                        break;
                    case "modifyRecord":
                        var rec2 = (DodoItem)JavaScriptConvert.DeserializeObject(
                            request.UriArgs["record"].ToString(), typeof(DodoItem));
                        app.CurrentDb.ModifyRecord(ref rec2);
                        writer.WriteLine("var reply = new dbapi.async(" + idcall + ", " + JavaScriptConvert.SerializeObject(rec2) + ");");
                        // writer.WriteLine("dbapi.cb(reply);");
                        break;
                    case "modifyFolder":
                        DodoItem folder = (DodoItem)JavaScriptConvert.DeserializeObject(
                            request.UriArgs["folder"].ToString(), typeof(DodoItem));
                        app.CurrentDb.ModifyFolder(ref folder, s);
                        writer.WriteLine("var reply = new dbapi.async(" + idcall + ", " + JavaScriptConvert.SerializeObject(folder) + ");");
                        // writer.WriteLine("dbapi.cb(reply);");
                        break;
                    case "removeRecord":
                        string id = request.UriArgs["code"] != null ? (string)request.UriArgs["code"] : "";
                        app.CurrentDb.RemoveRecord(id);
                        writer.WriteLine("var reply = new dbapi.async(" + idcall + ", 'ok');");
                        // writer.WriteLine("dbapi.cb(reply);");
                        break;
                    default:
                        throw new Exception("El método no existe " + method);
                }
            }
            catch (Exception err)
            {
                sb.Length = 0;

                writer.WriteLine(string.Format("var e = new Error('{0}');", err.Message.Replace("\n", " ")));
                writer.WriteLine("var reply = new dbapi.async(" + idcall + ", e);");
            }
            // mandamos la respuesta
            reply.LoadText(sb.ToString());
        }

        public void Dispose()
        {
        }
    }
}
