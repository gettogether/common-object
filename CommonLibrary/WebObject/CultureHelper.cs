using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.WebObject
{
    public class CultureHelper
    {
        public static void SetCurrentLanguage(string language)
        {
            if (!string.IsNullOrEmpty(language))
            {
                language = language.Trim().ToLower();
                if ("zh-tw".Equals(language) || "zh-cn".Equals(language) || "en-us".Equals(language))
                {
                    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo(language);
                    if (ci != null)
                    {
                        System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
                    }
                }
            }
        }
    }
}
