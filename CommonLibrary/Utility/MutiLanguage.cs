using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Utility
{

    public class MutiLanguage
    {
        public enum Languages
        {
            en_us,
            zh_cn,
            zh_tw
        }

        public static readonly string[] LanguageStrings= { "en-us", "zh-cn", "zh-tw" };

        public static string EnumToString(Languages lang)
        {
            string language = "en-us";
            switch (lang)
            {
                case Languages.en_us:
                    language = "en-us";
                    break;
                case Languages.zh_cn:
                    language = "zh-cn";
                    break;
                case Languages.zh_tw:
                    language = "zh-tw";
                    break;
                default:
                    break;
            }
            return language;
        }

        public static Languages GetCultureType()
        {
            System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentUICulture;
            string lang = null;
            if (ci != null) lang = ci.ToString().ToLower();

            if (lang == null || lang == "en-us")
                return Languages.en_us;
            else if (lang == "zh-cn")
                return Languages.zh_cn;
            else if (lang == "zh-tw")
                return Languages.zh_tw;
            else
                return Languages.en_us;
        }

        public static string GetLanguageString()
        {
            return EnumToString(GetCultureType());
        }

        public static string GetValueForLang(string en_us_value, string zh_cn_value, string zh_tw_value)
        {
            string ret = string.Empty;
            switch (CommonLibrary.Utility.MutiLanguage.GetCultureType())
            {
                case MutiLanguage.Languages.en_us:
                    ret = en_us_value;
                    break;
                case MutiLanguage.Languages.zh_cn:
                    ret = zh_cn_value;
                    break;
                case MutiLanguage.Languages.zh_tw:
                    ret = zh_tw_value;
                    break;
                default:
                    break;
            }
            if (string.IsNullOrEmpty(ret)) ret = en_us_value;
            return ret;
        }

        #region Resource

        public static string GetResource(Type t, string name, string lang)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager(t);
            System.Resources.ResourceSet rs = rm.GetResourceSet(new System.Globalization.CultureInfo(lang), true, true);
            return rs.GetString(name);
        }

        public static string GetResource(Type t, string name)
        {
            return GetResource(t, name, GetLanguageString());
        }

        #endregion
    }
}
