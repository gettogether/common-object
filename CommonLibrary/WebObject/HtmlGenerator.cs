using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.WebObject
{
    public class HtmlGenerator
    {
        public static string GenText(string id, string css, string value, bool isRequired, string appendAttr)
        {
            return GenControl("input", "text", id, css, value, isRequired, appendAttr);
        }

        public static string GenCheckbox(string id, string css, string value, bool isRequired, string appendAttr)
        {
            return GenControl("input", "checkbox", id, css, value, isRequired, appendAttr);
        }

        public static string GenControl(string html, string type, string id, string css, string value, bool isRequired, string appendAttr)
        {
            StringBuilder sbControl = new StringBuilder();
            sbControl.Append("<").Append(html);
            if (!string.IsNullOrEmpty(type)) sbControl.Append(" type=\"").Append(type).Append("\"");
            if (!string.IsNullOrEmpty(id)) sbControl.Append("id=\"").Append(id).Append("\"");
            if (!string.IsNullOrEmpty(css)) sbControl.Append("class=\"").Append(css).Append("\"");
            if (!string.IsNullOrEmpty(value)) sbControl.Append("value=\"").Append(value).Append("\"");
            if (isRequired) sbControl.Append(" required=\"1\"");
            if (!string.IsNullOrEmpty(appendAttr)) sbControl.Append(" ").Append(appendAttr);
            sbControl.Append(" />");
            return sbControl.ToString();
        }

        public static string GenTextarea(string id, string css, string value, bool isRequired, string appendAttr)
        {
            StringBuilder sbControl = new StringBuilder();
            sbControl.Append("<textarea");
            if (!string.IsNullOrEmpty(id)) sbControl.Append(" id=\"").Append(id).Append("\"");
            if (!string.IsNullOrEmpty(css)) sbControl.Append("class=\"").Append(css).Append("\"");
            if (isRequired) sbControl.Append("required=\"1\"");
            if (!string.IsNullOrEmpty(appendAttr)) sbControl.Append(" ").Append(appendAttr);
            sbControl.Append(">").Append(value).Append("</textarea>");
            return sbControl.ToString();
        }

        public static string GenRadioButton(Dictionary<string, string> idTexts, string name, string chkId, string css)
        {
            string fmt = "<input id=\"{0}\" type=\"radio\" class=\"{1}\" name=\"{2}\"{4}><label for=\"{0}\">{3}</label>&nbsp;&nbsp;&nbsp;&nbsp;";

            StringBuilder sb;
            sb = new StringBuilder();
            foreach (KeyValuePair<string, string> k in idTexts)
            {
                sb.AppendFormat(fmt, k.Key, css, name, k.Value, chkId == k.Key ? " checked=\"checked\"" : "");
            }
            return sb.ToString();
        }

        public static string GenRadioButtonWithVal(Dictionary<string, string> valTexts, string name, string selected_value, string css, string preWraper, string appWraper)
        {
            string radio = "<input id=\"{0}\" type=\"radio\" class=\"{1}\" name=\"{2}\" value=\"{3}\" {4}><label for=\"{0}\">{5}</label>";
            string hiddenField = string.Concat("<input type=\"hidden\" id=\"", name, "\" value=\"", selected_value, "\" />");
            string jsEvent = string.Concat("onclick=\"o('", name, "').value=this.value;\"");
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (KeyValuePair<string, string> k in valTexts)
            {
                if (!string.IsNullOrEmpty(preWraper))
                    sb.Append(preWraper);
                sb.AppendFormat(radio, string.Concat(i, "_", name), css, name, k.Key, string.Concat(selected_value == k.Key ? "checked=\"checked\"" : "", jsEvent), k.Value);
                if (!string.IsNullOrEmpty(appWraper))
                    sb.Append(appWraper);
                i++;
            }
            sb.Append(hiddenField);
            return sb.ToString();
        }

        public static string GenRadioButtonForEnum(Type t, System.Resources.ResourceManager rm, string name, string selected_value, string css, string preWraper, string appWraper)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            foreach (int i in Enum.GetValues(t))
            {
                d.Add(i.ToString(), rm.GetString(Enum.GetName(t, i)));
            }
            return GenRadioButtonWithVal(d, name, selected_value, css, preWraper, appWraper);
        }

        public static string GenImage(string src, string appendStyle)
        {
            if (string.IsNullOrEmpty(src)) return string.Empty;
            if (!string.IsNullOrEmpty(appendStyle))
                return string.Concat("<img src=\"", src, "\" ", appendStyle, " />");
            else
                return string.Concat("<img src=\"", src, "\" />");
        }

        public static string GenCheckboxButtonWithVal(Dictionary<string, string> valTexts, string name, string selected_value, string css, string preWraper, string appWraper)
        {
            string checkbox = "<input id=\"{0}\" type=\"checkbox\" class=\"{1}\" name=\"{2}\" value=\"{3}\" {4}><label for=\"{0}\">{5}</label>";
            string hiddenField = string.Concat("<input type=\"hidden\" id=\"", name, "\" value=\"", selected_value, "\" />");
            string jsEvent = string.Concat("onclick=\"o('", name, "').value=this.value;\"");
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (KeyValuePair<string, string> k in valTexts)
            {
                if (!string.IsNullOrEmpty(preWraper))
                    sb.Append(preWraper);
                sb.AppendFormat(checkbox, string.Concat(i, "_", name), css, name, k.Key, string.Concat(selected_value == k.Key ? "checked=\"checked\"" : "", jsEvent), k.Value);
                if (!string.IsNullOrEmpty(appWraper))
                    sb.Append(appWraper);
                i++;
            }
            sb.Append(hiddenField);
            return sb.ToString();
        }
    }
}
