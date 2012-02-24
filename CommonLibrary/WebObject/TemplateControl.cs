using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web.UI;

namespace CommonLibrary.WebObject
{
    public class TemplateControl : System.Web.UI.UserControl
    {
        private static bool _css = true;
        public static string GetCss(bool change)
        {
            if (!change) _css = !_css;
            if (_css)
            {
                _css = false;
                return "td";
            }
            else
            {
                _css = true;
                return "td2";
            }
        }

        public string Html
        {
            get
            {
                StringWriter sw = new StringWriter();
                HtmlTextWriter htmltw = new HtmlTextWriter(sw);
                OnLoad(null);
                base.Render(htmltw);
                System.Text.StringBuilder html = sw.GetStringBuilder();
                return html.ToString();
            }
        }

        public Control GetControl(string id)
        {
            return ControlHelper.GetControl(this.Page.Controls, null, id);
        }

        public T GetControl<T>(string id) where T : class, new()
        {
            return ControlHelper.GetControl<T>(this.Controls, id);
        }

        public static void SetValues<T>(T o, string prefix)
            where T : class, new()
        {
            WebHelper.SetValues(o, prefix);
        }
    }
}
