using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using DodoNet.Http;
using DodoNet.Web;

namespace DodoNet
{
    public class AppCollection : IDisposable
    {
        Dictionary<object, App> apps = new Dictionary<object, App>();
        Dictionary<string, IWebApplicationBase> appsHtml = new Dictionary<string, IWebApplicationBase>();
        object sync = new object();

        public App this[object key]
        {
            get
            {
                App ret = null;
                try
                {
                    ret = apps[key];
                }
                catch (KeyNotFoundException) { }
                return ret;
            }

            set
            {
                lock (sync)
                {
                    apps[key] = value;
                }
            }
        }

        public AppCollection()
        {
        }

        public App[] GetArray()
        {
            lock (sync)
            {
                App[] ret = new App[apps.Count];
                apps.Values.CopyTo(ret, 0);
                return ret;
            }
        }

        public void Add(App app, object key)
        {
            try
            {
                lock (sync)
                {
                    if (!apps.ContainsKey(key))
                    {
                        IWebApplicationBase tmp = app as IWebApplicationBase;
                        if (tmp != null)
                            appsHtml.Add(tmp.Host, tmp);

                        apps.Add(key, app);
                    }
                }
            }
            catch (Exception err)
            {
                Node.LogAppendLine(err);
            }
        }

        public void Remove(App app, object key)
        {
            try
            {
                lock (sync)
                {
                    if (apps.ContainsKey(key))
                    {
                        IWebApplicationBase tmp = app as IWebApplicationBase;
                        if (tmp != null && tmp.Host != null)
                            appsHtml.Remove(tmp.Host);

                        apps.Remove(key);
                    }
                }
            }
            catch (KeyNotFoundException) { }
        }

        public App GetApplication(object key)
        {
            App ret = null;
            try
            {
                ret = apps[key];                
            }
            catch (KeyNotFoundException) { }
            return ret;
        }
   
        #region IDisposable Members

        public void  Dispose()
        {
            lock (sync)
            {
                apps.Clear();
            }
        }

        #endregion
    }
}
