using System;
using System.Collections.Generic;

namespace DataMapping
{
    internal static class MappingInfoCache
    {
        private static Dictionary<string, List<PropertyMappingInfo>> cache = new Dictionary<string, List<PropertyMappingInfo>>();
        internal static List<PropertyMappingInfo> GetCache(string typeName)
        {
            List<PropertyMappingInfo> info = null;
            try
            {
                info = (List<PropertyMappingInfo>)cache[typeName];

            }
            catch (KeyNotFoundException) { }

            return info;
        }

        internal static void SetCache(string typeName, List<PropertyMappingInfo> mappingInfoList)
        {
            try
            {
                cache[typeName] = mappingInfoList;
            }
            catch
            {
                cache = new Dictionary<string, List<PropertyMappingInfo>>();
            }
        }

        public static void ClearCache()
        {
            cache.Clear();
        }
    }
}
