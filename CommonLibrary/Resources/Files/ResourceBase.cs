using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.WebObject;
using System.Reflection;

namespace CommonLibrary.Resources.Files
{
    public class ResourceBase<T>
        where T : ResourceObj, new()
    {
        public ResourceBase()
        {

        }

        public virtual void Load()
        {

        }
        public void LoadResourecesFromFile(string filePath, string cacheKey, Type resourceEnumType)
        {
            Type objType = this.GetType();
            foreach (string k in Enum.GetNames(resourceEnumType))
            {
                FieldInfo fi = objType.GetField(k);
                if (fi != null)
                {
                    foreach (PropertyInfo pi in fi.FieldType.GetProperties())
                    {
                        if (pi.CanWrite)
                            pi.SetValue(fi.GetValue(this), ResourcesHelper.GetResource(ResourcesHelper.GetResoureces(PropertyNameToLang(pi.Name), filePath, cacheKey), k), null);
                    }
                }
            }
            Utility.CacheHelper.RemoveCaches(cacheKey, Utility.CacheHelper.RemoveCacheType.StartWith);
        }

        public void LoadResourecesFromFile<R>(string filePath, string cacheKey, Dictionary<string, R> objs) where R : class, new()
        {
            foreach (string k in objs.Keys)
            {
                Type objType = objs[k].GetType();
                foreach (PropertyInfo pi in objType.GetProperties())
                {
                    if (pi.CanWrite)
                        pi.SetValue(objs[k], ResourcesHelper.GetResource(ResourcesHelper.GetResoureces(PropertyNameToLang(pi.Name), filePath, cacheKey), k), null);
                }
            }
            Utility.CacheHelper.RemoveCaches(cacheKey, Utility.CacheHelper.RemoveCacheType.StartWith);
        }

        private static string PropertyNameToLang(string name)
        {
            return name.Replace("_", "-");
        }
    }
}
