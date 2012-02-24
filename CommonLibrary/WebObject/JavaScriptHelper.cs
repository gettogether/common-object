using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace CommonLibrary.WebObject
{
    public class JavaScriptHelper
    {
        public static void RegisterAlertScript(string message, string navigateTo, string key, Page page)
        {
            string script = @"alert('" + message + @"');window.navigate('" + navigateTo + @"');";
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), page.UniqueID + key, script, true);
        }

        public static void RegisterConfirmScript(string message, string YesNavigateTo, string NoNavigateTo, string key, Page page)
        {
            string script = @"if(confirm('" + message + @"'))
                     window.navigate('" + YesNavigateTo + @"');
                   else
                     window.navigate('" + NoNavigateTo + @"')";
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), page.UniqueID + key, script, true);
        }

        public static void RegisterAlertAndBackScript(string message, string key, Page page)
        {
            string script = @"alert('" + message + @"');history.back();";
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), page.UniqueID + key, script, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="IsDoubleQuotation">Is " Or '</param>
        /// <returns></returns>
        public static string ReplaceSpecailChars(string text, bool IsDoubleQuotation)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            text = text.Replace("\\", "\\\\");
            if (IsDoubleQuotation)
            {
                text = text.Replace("\"", "\\\"");
            }
            else
            {
                text = text.Replace("\'", "\\\'");
            }
            return text;
        }

        public static string JsonValueEncode(string text, bool IsDoubleQuotation)
        {
            return System.Web.HttpUtility.HtmlEncode(JavaScriptHelper.ReplaceSpecailChars(text, IsDoubleQuotation));
        }

        public static string GenMutiLang2JSON(System.Resources.ResourceManager[] resources, string[] keys, Utility.MutiLanguage.Languages lang)
        {
            string itemFmt = "[\"{0}\",\"{1}\"],";
            StringBuilder sb = new StringBuilder();
            Dictionary<string, string> d = new Dictionary<string, string>();
            if (keys != null && keys.Length > 0)
            {
                string langStr = Utility.MutiLanguage.EnumToString(lang);
                foreach (string key in keys)
                {
                    if (!string.IsNullOrEmpty(key) && !d.ContainsKey(key))
                    {
                        string text = key;
                        string value = string.Empty;
                        if (resources != null && resources.Length > 0)
                        {
                            foreach (System.Resources.ResourceManager t in resources)
                            {
                                text = t.GetString(key, new System.Globalization.CultureInfo(Utility.MutiLanguage.EnumToString(lang)));
                                if (!string.IsNullOrEmpty(text))
                                    value = text;
                            }
                        }
                        d.Add(key, value);
                        sb.AppendFormat(itemFmt, key, ReplaceSpecailChars(value, true));
                    }
                }
            }
            return string.Format("var mtls = [{0}]", sb.ToString().TrimEnd(','));
        }
    }
}
