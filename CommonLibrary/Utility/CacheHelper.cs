using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Collections;

namespace CommonLibrary.Utility
{
    public class CacheHelper
    {
        public static bool AddToCache(string key, object item)
        {
            RemoveFromCache(key);
            HttpRuntime.Cache.Add(key, item, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            return true;
        }

        public static object GetFromCache(string key)
        {
            return HttpRuntime.Cache[key];
        }

        public static bool RemoveFromCache(string key)
        {
            if (GetFromCache(key) != null)
            {
                HttpRuntime.Cache.Remove(key);
            }
            return true;
        }

        public static object GetCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[CacheKey];
        }

        public static void SetCache(string CacheKey, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject);
        }

        public static void SetCache(string key, object obj, DateTime absolute_expiration)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(key, obj, null, absolute_expiration, TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);
        }

        public static void RemoveAllCache()
        {
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            ArrayList al = new ArrayList();
            while (CacheEnum.MoveNext())
            {
                al.Add(CacheEnum.Key);
            }
            foreach (string key in al)
            {
                _cache.Remove(key);
            }
        }

        public enum RemoveCacheType
        {
            StartWith,
            EndWith,
            Like,
            Equal,
            All,
        }
        public static void RemoveCaches(string key, RemoveCacheType type)
        {
            switch (type)
            {
                case RemoveCacheType.Equal:
                    RemoveCache(key);
                    return;
                case RemoveCacheType.All:
                    RemoveAllCache();
                    return;
                default:
                    break;
            }
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            ArrayList al = new ArrayList();
            while (CacheEnum.MoveNext())
            {
                switch (type)
                {
                    case RemoveCacheType.StartWith:
                        if (CacheEnum.Key.ToString().StartsWith(key))
                            al.Add(CacheEnum.Key);
                        break;
                    case RemoveCacheType.EndWith:
                        if (CacheEnum.Key.ToString().EndsWith(key))
                            al.Add(CacheEnum.Key);
                        break;
                    case RemoveCacheType.Like:
                        if (CacheEnum.Key.ToString().IndexOf(key) > -1)
                            al.Add(CacheEnum.Key);
                        break;
                    default:
                        break;
                }
            }
            foreach (string k in al)
            {
                _cache.Remove(k);
            }
        }

        public static void RemoveCache(string key)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Remove(key);
        }
    }
}
