using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
using System.Collections;
using CommonLibrary.WebObject.Entities;
using System.Web.Caching;

namespace CommonLibrary.WebObject
{
    public class CacheManager
    {
        #region Property
        public const string SPLIT_KEY_FLAG = "^,^";
        #endregion

        #region Function

        #region For cache manage page
        public static Caches GetCaches()
        {
            Cache _cache = HttpRuntime.Cache;
            IDictionaryEnumerator cacheEnum = _cache.GetEnumerator();
            CacheList cl = new CacheList();
            cl = new CacheList();
            while (cacheEnum.MoveNext())
            {
                cl.Add(new CacheInfo(cacheEnum.Key.ToString(), cacheEnum.Value.GetType().ToString()));
            }
            Caches rs = new Caches();
            rs.CacheList = new CacheList();
            cl.SortBy(CacheInfo.Columns.Key, true);
            rs.CacheList.AddRange(cl);
            rs.Total = cl.Count;
            return rs;
        }

        public static bool ClearCache(string url, string key, out string errMsg)
        {
            string rs = Utility.RequestHelper.GetRequest(string.Concat(url, "?type=1&key=", key), 0);
            bool isSuccess = rs == Definition.OK_FLAG;
            errMsg = isSuccess ? string.Empty : rs;
            return isSuccess;
        }

        public static bool ClearCacheStartWith(string url, string prefix, out string errMsg)
        {
            string rs = Utility.RequestHelper.GetRequest(string.Concat(url, "?type=3&key=", prefix), 0);
            bool isSuccess = rs == Definition.OK_FLAG;
            errMsg = isSuccess ? string.Empty : rs;
            return isSuccess;
        }

        public static bool ClearCache(string url, string[] keys, out string errMsg)
        {
            string rs = Utility.RequestHelper.GetRequest(string.Concat(url, "?type=1&keys=", HttpUtility.UrlEncode(Utility.StringHelper.ArrayToString(keys, SPLIT_KEY_FLAG))), 0);
            bool isSuccess = rs == Definition.OK_FLAG;
            errMsg = isSuccess ? string.Empty : rs;
            return isSuccess;
        }

        public static bool ClearAllCaches(string url, out string errMsg)
        {
            string rs = Utility.RequestHelper.GetRequest(string.Concat(url, "?type=2&key=all"), 0);
            bool isSuccess = rs == Definition.OK_FLAG;
            errMsg = isSuccess ? string.Empty : rs;
            return isSuccess;
        }

        public static Caches GetCaches(string url, int pageIndex, int pageSize, string sort, bool isAsc, out string errMsg)
        {
            return GetCaches(url, pageIndex, pageSize, sort, isAsc, "", "", out errMsg);
        }

        public static Caches GetCaches(string url, int pageIndex, int pageSize, string sort, bool isAsc, string cacheKey, string cacheType, out string errMsg)
        {

            string result = Utility.RequestHelper.GetRequest(string.Concat(url, "?type=0", "&page=", pageIndex, "&size=", pageSize, "&sort=", sort, "&asc=", isAsc ? "Y" : "N","&cache_key=", cacheKey, "&cache_type=", cacheType), 0);
            errMsg = string.Empty;
            try
            {
                return Utility.SerializationHelper.FromXml<Caches>(result);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(result))
                    errMsg = result;
                else
                    errMsg = ex.Message;
                return new Caches();
            }
        }

        public static string GetCacheDetail(string url, string key)
        {
            return Utility.RequestHelper.GetRequest(string.Concat(url, "?type=4&key=", key), 0);
        }

        #endregion

        #endregion


    }
}
