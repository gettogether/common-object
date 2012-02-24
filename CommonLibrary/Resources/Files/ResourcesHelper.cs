using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Utility;
using CommonLibrary.WebObject;
using System.IO;

namespace CommonLibrary.Resources.Files
{
    public class ResourcesHelper
    {
        public static Resources GetResoureces(CommonLibrary.Utility.MutiLanguage.Languages lang, string filePath, string cacheKey)
        {
            return GetResoureces(CommonLibrary.Utility.MutiLanguage.EnumToString(lang), filePath, cacheKey);
        }

        public static Resources GetResoureces(string lang, string filePath, string cacheKey)
        {
            cacheKey = string.Concat(cacheKey, lang);
            Resources resoureces = CacheHelper.GetCache(cacheKey) as Resources;
            if (resoureces == null)
            {

                FileInfo fi = new FileInfo(filePath);
                string postfix;
                if (lang == MutiLanguage.EnumToString(MutiLanguage.Languages.en_us)) postfix = fi.Extension;
                else postfix = string.Concat(".", lang, fi.Extension);
                filePath = fi.FullName.Replace(fi.Extension, postfix);
                if (File.Exists(filePath))
                {
                    resoureces = new Resources().FormXml(File.ReadAllText(filePath));
                    CacheHelper.SetCache(cacheKey, resoureces);
                }
            }
            return resoureces;
        }

        public static string GetResource(Resources rs, object rn)
        {
            return GetResource(rs, rn.ToString());
        }

        public static string GetResource(Resources rs, string key)
        {
            string resource = string.Empty;
            if (rs != null && rs.Items != null && rs.Items.Length > 0)
            {
                foreach (ResourcesResource r in rs.Items)
                {
                    if (r.Key == key)
                        resource = r.Value;
                }
            }
            return resource;
        }

        public static string GetResourceFromFile(string filePath, MutiLanguage.Languages language)
        {
            string lang = MutiLanguage.EnumToString(language);
            string text = "";
            FileInfo fi = new FileInfo(filePath);
            string postfix;
            if (lang == MutiLanguage.EnumToString(MutiLanguage.Languages.en_us)) postfix = fi.Extension;
            else postfix = string.Concat(".", lang, fi.Extension);
            filePath = fi.FullName.Replace(fi.Extension, postfix);
            if (File.Exists(filePath))
            {
                text = File.ReadAllText(filePath, Encoding.UTF8);
            }
            return text;
        }
        public static ResourceObj GetResourceFromFile(string filePath)
        {
            return new ResourceObj(GetResourceFromFile(filePath, MutiLanguage.Languages.en_us), GetResourceFromFile(filePath,
                 MutiLanguage.Languages.zh_cn), GetResourceFromFile(filePath, MutiLanguage.Languages.zh_tw));
        }
    }
}
