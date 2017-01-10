using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DodoNet.Http.Caching
{
    public class WebCache : IDisposable
    {
        Dictionary<string, CacheItem> cache;
        object sync;

        public WebCache()
        {
            cache = new Dictionary<string, CacheItem>();
            sync = new object();
        }

        public object Add(string key, object value)
        {
            lock (sync)
            {
                object ret = null;
                if (!cache.ContainsKey(key))
                    Insert(key, value);
                else
                    ret = cache[key];
                return ret;
            }
        }

        public object Get(string key)
        {
            object ret = null;
            if (cache.ContainsKey(key))
                ret = cache[key].Value;
            return ret;
        }

        void Insert(string key, object value)
        {
            CacheItem item = new CacheItem();
            item.Key = key;
            item.Value = value;
            item.LastChange = DateTime.Now;
            cache.Add(key, item);
        }

        #region IDisposable Members

        public void Dispose()
        {
            cache.Clear();
        }

        #endregion
    }

    class CacheItem
    {
        public string Key;
        public object Value;
        public DateTime LastChange;
    }
}
