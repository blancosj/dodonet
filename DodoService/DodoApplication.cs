using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Text.RegularExpressions;

using DodoNet;
using DodoNet.Tools;
using DodoNet.Http;
using DodoNet.Http.Compilation;
using DodoNet.Web;

using Newtonsoft.Json;

namespace DodoService
{
    public class DodoApplication : WebApplication, IDisposable
    {
        public const string collateMySql = "COLLATE utf8_unicode_ci";
        public const long MultiNumForId = 90000000000000;

        string defaultFolder;
        public string DefaultFolder { get { return defaultFolder; } set { defaultFolder = value; } }

        public DodoDb CurrentDb { get; private set; }
        public Node LocalNode { get; set; }

        public string ShorterServerUrl { get; private set; }

        public string ProxyFileGet { get; set; }

        public string pathDatabase;

        public DodoRules Rules { get; private set; }

        /// <summary>
        /// inicio de la aplicacion
        /// </summary>
        public void InitApp(Node localNode)
        {
            LocalNode = localNode;
            Sessions = new WebSessionCollection(localNode);
            WebConfig.SessionIdLength = 10;

            Home = "index.aspx";            

            ShorterServerUrl = "pwq.me";

            CurrentDb = new DodoDb(this);
            Rules = new DodoRules(this);

            Methods.Add(new DodoService.HttpMethods.ASPX());
            Methods.Add(new DodoService.HttpMethods.JMX());
            Methods.Add(new DodoService.HttpMethods.JXP());
            Methods.Add(new DodoService.HttpMethods.FileDownload());
            Methods.Add(new DodoService.HttpMethods.FileGet());
            Methods.Add(new DodoService.HttpMethods.GetItem());
            Methods.Add(new DodoService.HttpMethods.GetMethod());
            Methods.Add(new DodoService.HttpMethods.SaveItem());
            Methods.Add(new DodoService.HttpMethods.LastItemsOf());
            Methods.Add(new DodoService.HttpMethods.ProxyBox());
            Methods.Add(new DodoService.HttpMethods.GetBox());
            Methods.Add(new DodoService.HttpMethods.ProxyRest());
            Methods.Add(new DodoService.HttpMethods.SBD());
        }

        public void OpenDatabase(string pathDatabase)
        {
            this.pathDatabase = pathDatabase;
        }

        #region Application

        public override void NewNeighbour(NodeBind nodeBind)
        {
            
        }

        public override void NeighbourRemoved(NodeBind nodeBind, bool failure)
        {
            
        }

        #endregion

        #region Manage Session

        public override void OnAddSession(HttpContext context)
        {
            DodoSession session = null;

            if (context.Request.Form["email"] != null)
            {
                session = CurrentDb.CheckEmail(
                    context.Request.Form["email"]);

                if (session == null)
                    throw new Exception("Email is not correct");
            }
            else
            {
                session = CurrentDb.CheckUserAndPwd(
                    context.Request.Form["user"], context.Request.Form["pwd"]);

                if (session == null)
                    throw new Exception("User or Password is not correct");
            }

            Sessions.AddSession(session);
            SetSessionState(context, session);
        }

        #endregion

        public void WriteGlobalVars()
        {
            HttpContext context = DodoApplication.CurrentContext;
            DodoSession session = (DodoSession)context.Session;
            context.Reply.Write("<script type=\"text/javascript\">");
            context.Reply.Write("var currentUserId = '{0}';", session.userId);
            context.Reply.Write("var currentUserName = '{0}';", session.userName);
            context.Reply.Write("var currentSession = '{0}';", session.Auth);
            context.Reply.Write("</script>");
        }

        public override void GetReply(HttpContext context)
        {
            try
            {
                if (context.Request.Url == "/")
                {
                    var tmp = context.Request.OriginalRequestUrl;

                    int pos = tmp.IndexOf("/");
                    if (pos > -1)
                        tmp = tmp.Insert(pos + 1, Home);

                    context.Request.Url = tmp;
                }

                string destiny = context.Request.Url;

                destiny = destiny.Replace("/", @"\");
                if (destiny.StartsWith(@"\")) destiny = destiny.Substring(1);
                context.Request.PhysicalPathFile = Path.Combine(DefaultFolder, destiny);

                bool defaultMethod = true;

                foreach (var item in Methods)
                {
                    if (item.MustProcess())
                    {
                        item.Process(this);
                        defaultMethod = false;
                        break;
                    }
                }

                if (defaultMethod)
                    context.Reply.LoadFile();
            }
            catch (Exception err)
            {
                context.Reply = new HttpError(err, context.Request);
            }
        }

        public long CalcUniqueId(string uniqueId)
        {
            long ret = 0;

            if (uniqueId.StartsWith("~"))
                ret = Conversion.Don2Num(uniqueId.Substring(1)) / DodoApplication.MultiNumForId;
            else
                long.TryParse(uniqueId, out ret);

            return ret;
        }

        public void Dispose()
        {
        }

        #region IWebApplicationBase Members

        public Dictionary<string, IWebApplicationBase> ExtList
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        #endregion
    }
}
