using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace CommonLibrary.WebObject
{
    public class ControlHelper
    {
        public static string GetChildControlPrefix(Control control)
        {
            if (control.NamingContainer.Parent == null)
            {
                return control.ID;
            }
            else
            {
                return String.Format("{0}_{1}", control.NamingContainer.ClientID, control.ID);
            }
        }

        public static string ControlToHtml(System.Web.UI.WebControls.WebControl c)
        {
            StringBuilder sb = new StringBuilder();
            c.RenderControl(new System.Web.UI.HtmlTextWriter(new System.IO.StringWriter(sb)));
            return sb.ToString();
        }

        public static string ControlToHtml(System.Web.UI.Control c)
        {
            StringBuilder sb = new StringBuilder();
            c.RenderControl(new System.Web.UI.HtmlTextWriter(new System.IO.StringWriter(sb)));
            return sb.ToString();
        }

        //public static System.Web.UI.Control GetControl(Page p, ControlCollection cc, string id)
        //{
        //    Control uc = null;
        //    if (cc == null) cc = p.Page.Controls;
        //    foreach (Control c in cc)
        //    {
        //        if (c.ID == id)
        //        {
        //            uc = c;
        //            break;
        //        }
        //        else if (c.Controls.Count > 0)
        //        {
        //            uc = GetControl(p, c.Controls, id);
        //        }
        //        if (uc != null) break;
        //    }
        //    return uc;
        //}

        public static System.Web.UI.Control GetControl(ControlCollection baseCC, ControlCollection cc, string id)
        {
            Control uc = null;
            if (cc == null) cc = baseCC;
            foreach (Control c in cc)
            {
                if (c.ID == id)
                {
                    uc = c;
                    break;
                }
                else if (c.Controls.Count > 0)
                {
                    uc = GetControl(baseCC, c.Controls, id);
                }
                if (uc != null) break;
            }
            return uc;
        }

        public static T GetControl<T>(ControlCollection cc, string id) where T : class, new()
        {
            Type t = typeof(T);
            Control c = GetControl(cc, null, id);
            if (c != null) return c as T;
            return default(T);
        }
    }
}
