using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Web.Caching;
using System.Reflection;
using System.Collections;
using CommonLibrary.ObjectBase;
using CommonLibrary.WebObject.Entities;

namespace CommonLibrary.WebObject
{
    public class CacheManagerTemplatePage : AdminTemplatePage
    {
        public string CacheKey
        {
            get { return Request["cache_key"]; }
        }

        public string CacheType
        {
            get { return Request["cache_type"]; }
        }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            try
            {
                if (type == 0)
                {
                    Response.Write(Utility.SerializationHelper.SerializeToXml(GetCaches(false, PageIndex, PageSize, Sort, IsAsc, CacheKey, CacheType)));
                }
                else if (type == 1)
                {
                    if (!string.IsNullOrEmpty(key))
                    {
                        Utility.CacheHelper.RemoveCache(key);
                        OK();
                    }
                    else if (!string.IsNullOrEmpty(Request["keys"]))
                    {
                        foreach (string k in Request["keys"].Split(new string[] { CacheManager.SPLIT_KEY_FLAG }, StringSplitOptions.RemoveEmptyEntries))
                            Utility.CacheHelper.RemoveCache(k);
                        OK();
                    }
                }
                else if (type == 2 && key == "all")
                {
                    Utility.CacheHelper.RemoveAllCache();
                    OK();
                }
                else if (type == 3 && !string.IsNullOrEmpty(key))
                {
                    Utility.CacheHelper.RemoveCaches(key, CommonLibrary.Utility.CacheHelper.RemoveCacheType.StartWith);
                    OK();
                }
                else if (type == 4 && !string.IsNullOrEmpty(key))
                {
                    Response.Write(GetCacheDetail(key));
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            Response.End();
        }

        #region Function

        public static Caches GetCaches()
        {
            return GetCaches(true, 0, 0, CacheInfo.Columns.Key.ToString(), true, "", "");
        }

        public static Caches GetCaches(int pageIndex, int pageSize, string sort, bool isAsc)
        {
            return GetCaches(false, pageIndex, pageSize, sort, isAsc, "", "");
        }

        private static Caches GetCaches(bool isGetAll, int pageIndex, int pageSize, string sort, bool isAsc, string cacheKey, string cacheType)
        {
            Cache _cache = HttpRuntime.Cache;
            IDictionaryEnumerator cacheEnum = _cache.GetEnumerator();
            CacheList cl = new CacheList();
            cl = new CacheList();
            while (cacheEnum.MoveNext())
            {
                if (!string.IsNullOrEmpty(cacheKey) && cacheEnum.Key.ToString().ToUpper().IndexOf(cacheKey.ToUpper()) < 0) continue;
                if (!string.IsNullOrEmpty(cacheType) && cacheEnum.Value.GetType().ToString().ToUpper().IndexOf(cacheType.ToUpper()) < 0) continue;
                cl.Add(new CacheInfo(cacheEnum.Key.ToString(), cacheEnum.Value.GetType().ToString()));
            }
            Caches rs = new Caches();
            rs.CacheList = new CacheList();
            if (isGetAll)
            {
                cl.SortBy(sort, isAsc);
                rs.CacheList.AddRange(cl);
            }
            else
                rs.CacheList.AddRange(cl.GetPaging(pageSize, pageIndex, sort, isAsc));
            rs.Total = cl.Count;
            return rs;
        }

        public static string GetCacheDetail(string key)
        {
            object cache = Utility.CacheHelper.GetCache(key);
            if (cache == null)
                return string.Empty;
            else
            {
                StringBuilder sb = new StringBuilder();
                Serialize(ref sb, cache, key);
                return sb.ToString();
            }
        }

        private static void Serialize(ref StringBuilder sb, object obj, string key)
        {
            Type t = obj.GetType();
            try
            {
                if (t.IsGenericType && t.Name == "Dictionary`2")
                {
                    sb.AppendLine("<Collections>");
                    IDictionaryEnumerator enumrator = t.GetMethod("GetEnumerator").Invoke(obj, null) as IDictionaryEnumerator;
                    while (enumrator.MoveNext())
                    {
                        sb.AppendLine("<Collection>");
                        sb.AppendLine(string.Concat("<key>", enumrator.Key, "</key>"));
                        Serialize(ref sb, enumrator.Value, enumrator.Key.ToString());
                        sb.AppendLine("</Collection>");
                    }
                    sb.AppendLine("</Collections>");
                }
                else if (t == typeof(DataTable))
                {
                    DataTable table = obj as DataTable;
                    if (string.IsNullOrEmpty(table.TableName))
                        table.TableName = string.Concat(key, "-Row");
                    sb.Append(Utility.SerializationHelper.SerializeDataTableXml(table));
                }
                else
                    sb.Append(Utility.SerializationHelper.SerializeToXml(obj));

            }
            catch (Exception ex)
            {
                sb.Append(ex.Message);
            }
        }

        #endregion

    }

}
