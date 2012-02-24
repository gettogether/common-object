using System;
using System.Configuration;
using System.Collections.Generic;

namespace CommonLibrary.Utility
{
    public static class ConfigHelper
    {
        public static string GetConnectionString(string key)
        {
            if (ConfigurationManager.ConnectionStrings[key] != null)
                return ConfigurationManager.ConnectionStrings[key].ConnectionString;
            return string.Empty;
        }

        public static Dictionary<string, string> GetConnectionStrings(string[] keys)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            foreach (string k in keys)
                ret[k] = GetConnectionString(k);
            return ret;
        }

        public static string GetAppSetting(string key)
        {
            return GetAppSetting(key, "");
        }

        public static string GetAppSetting(string key, string defaultValue)
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
                return ConfigurationManager.AppSettings[key];
            else
                return defaultValue;
        }

        public static Dictionary<string, string> GetAppSettings(string[] keys)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            foreach (string k in keys)
                ret[k] = GetAppSetting(k);
            return ret;
        }

        public static int GetIntergerSetting(string key)
        {
            return NumberHelper.ToInt(GetAppSetting(key), 0);
        }

        public static bool GetBoolSetting(string key)
        {
            return GetAppSetting(key) == "1";
        }

        public static void SaveSetting(string key, string value)
        {
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);
            config.Save(System.Configuration.ConfigurationSaveMode.Modified);
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
        }

        public static void SetConfig<T>(T t) where T : class, new()
        {
            foreach (string s in DataMapping.ObjectHelper.GetObjectFields(t.GetType()))
            {
                DataMapping.ObjectHelper.SetValue<T>(t, s, GetAppSetting(s), DateHelper.DateFormat.ddMMyyyys.ToString());
            }
        }

        public static Dictionary<string, Type> GetValueTypes<T>(T t) where T : class, new()
        {
            Dictionary<string, Type> ret = new Dictionary<string, Type>();
            foreach (DataMapping.PropertyMappingInfo p in DataMapping.ObjectHelper.GetProperties(t.GetType()))
            {
                ret.Add(p.DataFieldName, p.PropertyInfo.PropertyType);
            }
            return ret;
        }

        /// <summary>
        /// string[]{name,type,value}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string[] GetConfig<T>(T t, string key) where T : class, new()
        {
            foreach (string[] a in GetConfigs<T>(t))
            {
                if (a[0] == key)
                    return a;
            }
            return null;
        }

        /// <summary>
        /// string[]{name,type,value}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static List<string[]> GetConfigs<T>(T t) where T : class, new()
        {
            List<string[]> ret = new List<string[]>();
            foreach (DataMapping.PropertyMappingInfo p in DataMapping.ObjectHelper.GetProperties(t.GetType()))
            {
                ret.Add(new string[] { p.DataFieldName, p.PropertyInfo.PropertyType.ToString(), DataMapping.ObjectHelper.GetValue<T>(t, p.DataFieldName).ToString() });
            }
            return ret;
        }

        public static TimeSpan GetTime(string key)
        {
            return GetTime(key, string.Empty);
        }

        public static TimeSpan GetTime(string key, string defaultValue)
        {
            string v = GetAppSetting(key);
            if (string.IsNullOrEmpty(v)) v = defaultValue;
            if (string.IsNullOrEmpty(v)) return TimeSpan.MinValue;
            return DateHelper.ConvertTime(v);
        }
    }
}
