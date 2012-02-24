using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Collections.Generic;

namespace CommonLibrary.WebObject
{
    public class TemplatePage : System.Web.UI.Page
    {
        public static CultureInfo defaultCulture = new CultureInfo("en-us");

        protected override void InitializeCulture()
        {
            string lang = Request["lang"];
            if (lang != null)
            {
                lang = lang.ToLower();
            }

            if ("zh-tw".Equals(lang) || "zh-cn".Equals(lang) || "en-us".Equals(lang))
            {
                Session["CurrentUICulture"] = new System.Globalization.CultureInfo(lang);
                Session["culture_string"] = lang;
            }

            System.Globalization.CultureInfo ci = Session["CurrentUICulture"] as System.Globalization.CultureInfo;
            if (ci != null)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
            }
            // Don't know why, but sometimes the browser culture gets automatically assigned
            // into current culture, so datetime passed into the database gets Chinese month names
            System.Threading.Thread.CurrentThread.CurrentCulture = defaultCulture;
        }

        public string GetCurrentUrl()
        {
            return WebHelper.GetCurrentUrl();
        }

        public bool IsFireFox()
        {
            return WebHelper.IsFireFox(this.Page);
        }
        public void WriteXml(string xml)
        {
            WebHelper.WriteXml(this.Page, xml);
        }

        public void ResponseNoRight()
        {
            Response.Write("You have no right to view this page.");
            Response.End();
        }

        public void CheckRight(bool allowed)
        {
            if (!allowed) ResponseNoRight();
        }

        public Control GetControl(string id)
        {
            return ControlHelper.GetControl(this.Page.Controls, null, id);
        }

        public T GetControl<T>(string id) where T : class, new()
        {
            return ControlHelper.GetControl<T>(Page.Controls, id);
        }

        public static void SetValues<T>(T o, string prefix)
            where T : class, new()
        {
            WebHelper.SetValues(o, prefix);
        }

        public static List<T> GetListFromRequest<T>(string prefix)
            where T : class, new()
        {
            return WebHelper.GetListFromRequest<T>(prefix);
        }

        public static List<T> GetListFromRequestForVal<T>(string prefix)
        {
            return WebHelper.GetListFromRequestForVal<T>(prefix);
        }

    }
}