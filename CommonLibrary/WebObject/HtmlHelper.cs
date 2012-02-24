using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace CommonLibrary.WebObject
{
    public class HtmlHelper
    {
        #region Select

        #region Properties
        public static string[] PAGE_SELECTOR = new string[] { "5,5", "10,10", "20,20", "30,30", "50,50", "100,100" };
        public static string[] PAGE_SELECTOR_WITH_SELECT_ALL = new string[] { "5,5", "10,10", "20,20", "30,30", "50,50", "100,100", "1000,All" };
        public const string STR_SELECT = "<select id=\"{0}\" name=\"{0}\" class=\"{1}\" title=\"{2}\">{3}</select>";
        public const string STR_SELECT_WITH_EVENT = "<select id=\"{0}\" name=\"{0}\" class=\"{1}\" title=\"{2}\" {4}>{3}</select>";
        public const string STR_SELECT_ITEM = "<option value=\"{0}\">{1}</option>";
        public const string STR_SELECT_ITEM_SELECTED = "<option value=\"{0}\" selected=\"selected\">{1}</option>";
        public const string STR_WHITESPACE = "&nbsp;";
        public static string SortHeaderFmt = "<a href=\"Javascript:{0}\" style=\"white-space:nowrap;{1}\">{2}{3}</a>";
        public static string SortHeaderFmt2 = "<a href=\"Javascript:{0}\" style=\"{1}\">{2}{3}</a>";
        #endregion
        /// <summary>
        /// format:value=="":text,else text(value)
        /// </summary>
        /// <param name="source">dictionary:key->value,text->value</param>
        /// <param name="selected_value"></param>
        /// <param name="id">select id</param>
        /// <param name="css">select css</param>
        /// <param name="title">select title</param>
        /// <param name="jsFunc">extend attribute,"onchange="SetVisible();","style="display:none" eg.</param>
        /// <returns>select control html</returns>
        public static string GenSelect(System.Collections.Generic.Dictionary<string, string> source, string selected_value, string id, string css, string title, string jsFunc)
        {
            StringBuilder sb_ret = new StringBuilder();
            if (string.IsNullOrEmpty(selected_value)) selected_value = "";
            selected_value = selected_value.Trim().ToUpper();
            foreach (string k in source.Keys)
            {
                if (k.Trim().ToUpper().Equals(selected_value))
                {
                    if (k == "")
                        sb_ret.Append(string.Format(STR_SELECT_ITEM_SELECTED, k, string.Format("{0}", source[k])));
                    else
                        sb_ret.Append(string.Format(STR_SELECT_ITEM_SELECTED, k, string.Format("{0}({1})", source[k], k)));
                }
                else
                {
                    if (k == "")
                        sb_ret.Append(string.Format(STR_SELECT_ITEM, k, string.Format("{0}", source[k])));
                    else
                        sb_ret.Append(string.Format(STR_SELECT_ITEM, k, string.Format("{0}({1})", source[k], k)));
                }
            }
            if (jsFunc == "")
                return string.Format(STR_SELECT, id, css, title, sb_ret.ToString());
            else
                return string.Format(STR_SELECT_WITH_EVENT, id, css, title, sb_ret.ToString(), jsFunc);
        }
        /// <summary>
        /// format:empty:text,else:text
        /// </summary>
        /// <param name="source">dictionary:key->value,text->value</param>
        /// <param name="selected_value"></param>
        /// <param name="id">select id</param>
        /// <param name="css">select css</param>
        /// <param name="title">select title</param>
        /// <param name="jsFunc">extend attribute,"onchange="SetVisible();","style="display:none" eg.</param>
        /// <returns>select control html</returns>
        public static string GenSelectWithoutVal(System.Collections.Generic.Dictionary<string, string> source, string selected_value, string id, string css, string title, string jsFunc)
        {
            StringBuilder sb_ret = new StringBuilder();
            if (!string.IsNullOrEmpty(selected_value))
                selected_value = selected_value.Trim().ToUpper();
            else
                selected_value = "";
            foreach (string k in source.Keys)
            {
                if (k.Trim().ToUpper().Equals(selected_value))
                {
                    if (k == "")
                        sb_ret.Append(string.Format(STR_SELECT_ITEM_SELECTED, k, string.Format("{0}", source[k])));
                    else
                        sb_ret.Append(string.Format(STR_SELECT_ITEM_SELECTED, k, string.Format("{0}", source[k])));
                }
                else
                {
                    if (k == "")
                        sb_ret.Append(string.Format(STR_SELECT_ITEM, k, string.Format("{0}", source[k])));
                    else
                        sb_ret.Append(string.Format(STR_SELECT_ITEM, k, string.Format("{0}", source[k])));
                }
            }
            if (jsFunc == "")
                return string.Format(STR_SELECT, id, css, title, sb_ret.ToString());
            else
                return string.Format(STR_SELECT_WITH_EVENT, id, css, title, sb_ret.ToString(), jsFunc);
        }

        public static string GenSelectForEnum(Type t, System.Resources.ResourceManager rm, bool isForValues, string selected_value, string id, string css, string title, string jsFunc, Dictionary<string, string> source, List<string> excludeValues)
        {
            string name = string.Empty;
            if (source == null) source = new Dictionary<string, string>();
            if (isForValues)
            {
                string eName = string.Empty;
                foreach (int v in Enum.GetValues(t))
                {
                    eName = Enum.GetName(t, v);
                    if (rm != null)
                        name = rm.GetString(eName, System.Threading.Thread.CurrentThread.CurrentUICulture);
                    if (string.IsNullOrEmpty(name))
                        name = eName;
                    if (!(excludeValues != null && excludeValues.Contains(v.ToString())))
                        source.Add(v.ToString(), name);
                }
            }
            else
                foreach (string n in Enum.GetNames(t))
                {
                    if (rm != null)
                        name = rm.GetString(n, System.Threading.Thread.CurrentThread.CurrentUICulture);
                    if (string.IsNullOrEmpty(name))
                        name = n;
                    if (!(excludeValues != null && excludeValues.Contains(name)))
                        source.Add(n, name);
                }
            return CommonLibrary.WebObject.HtmlHelper.GenSelectWithoutVal(source, selected_value, id, css, title, jsFunc);
        }

        public static string GenSelectForEnum(Type t, System.Resources.ResourceManager rm, bool isForValues, string selected_value, string id, string css, string title, string jsFunc, Dictionary<string, string> source)
        {
            return CommonLibrary.WebObject.HtmlHelper.GenSelectForEnum(t, rm, isForValues, selected_value, id, css, title, jsFunc, source, null);
        }

        public static string GenSelectForEnum(Type t, System.Resources.ResourceManager rm, bool isForValues, string selected_value, string id, string css, string title, string jsFunc)
        {
            return GenSelectForEnum(t, rm, isForValues, selected_value, id, css, title, jsFunc, null);
        }

        public static string GenSelectForEnum(Type t, System.Resources.ResourceManager rm, bool isForValues, string selected_value, string id, string css, string title, string jsFunc, string appendName, string appendValue)
        {
            Dictionary<string, string> source = new Dictionary<string, string>();
            source.Add(appendValue, appendName);
            return GenSelectForEnum(t, rm, isForValues, selected_value, id, css, title, jsFunc, source);
        }

        public static string GenSelectForEnum(Type t, System.Resources.ResourceManager rm, bool isForValues, string selected_value, string id, string css, string title, string jsFunc, string appendName, string appendValue, string excludeValue)
        {
            Dictionary<string, string> source = new Dictionary<string, string>();
            if (appendName != null || appendValue != null)
                source.Add(appendValue, appendName);
            List<string> excludeVals = new List<string>();
            if (excludeValue != null)
                excludeVals.Add(excludeValue);
            return GenSelectForEnum(t, rm, isForValues, selected_value, id, css, title, jsFunc, source, excludeVals);
        }

        public static string GenSelectForEnum(Type t, bool isForValues, string selected_value, string id, string css, string title, string jsFunc, Dictionary<string, string> source)
        {
            if (source == null) source = new Dictionary<string, string>();
            if (isForValues)
            {
                string name = string.Empty;
                foreach (int v in Enum.GetValues(t))
                    source.Add(v.ToString(), Enum.GetName(t, v));
            }
            else
                foreach (string n in Enum.GetNames(t))
                    source.Add(n, n);
            return CommonLibrary.WebObject.HtmlHelper.GenSelectWithoutVal(source, selected_value, id, css, title, jsFunc);
        }

        public static string GenSelectForEnum(Type t, bool isForValues, string selected_value, string id, string css, string title, string jsFunc, string appendName, string appendValue)
        {
            Dictionary<string, string> source = new Dictionary<string, string>();
            source.Add(appendValue, appendName);
            return GenSelectForEnum(t, isForValues, selected_value, id, css, title, jsFunc, source);
        }

        public static string GenSelectForNumber(string selected_value, string id, string css, string title, string jsFunc, int start, int end, int spaceNumber)
        {
            Dictionary<string, string> dicNumbers = new Dictionary<string, string>();
            for (int i = start; i <= end; i = i + spaceNumber)
            {
                string s = i.ToString();
                dicNumbers.Add(s, s);
            }
            return GenSelectWithoutVal(dicNumbers, selected_value, id, css, title, jsFunc);
        }

        #endregion

        #region Paging
        public static string GenRecordInfo(int recordCount)
        {
            return string.Format(Resources.Resource.RecordsFoundString, recordCount);
        }
        public static string GenPageSize(string id, string css, decimal pageSize)
        {
            return GenPageSize(id, css, pageSize, null);
        }
        public static string GenPageSize(string id, string css, decimal pageSize, string evalCode)
        {
            return GenPageSize(id, css, pageSize, evalCode, false);
        }
        public static string GenPageSize(string id, string css, decimal pageSize, string evalCode,bool isNoSelectAllOption)
        {
            StringBuilder sb_r = new StringBuilder();
            StringBuilder sb_ret = new StringBuilder();
            foreach (string s in isNoSelectAllOption ? HtmlHelper.PAGE_SELECTOR : PAGE_SELECTOR_WITH_SELECT_ALL)
            {
                string[] info = s.Split(',');
                if (decimal.Parse(info[0]) == pageSize)
                    sb_ret.Append(string.Format(STR_SELECT_ITEM_SELECTED, info[0], info[1]));
                else
                    sb_ret.Append(string.Format(STR_SELECT_ITEM, info[0], info[1]));
            }
            if (evalCode != null & evalCode != "")
                return sb_r.AppendFormat(Resources.Resource.ShowString, string.Format(STR_SELECT_WITH_EVENT, id, css, "", sb_ret.ToString(), "onchange=\"ChangePageSize(this,'" + evalCode + "')\" style='width:50px;'")).ToString();
            else
                return sb_r.AppendFormat(Resources.Resource.ShowString, string.Format(STR_SELECT_WITH_EVENT, id, css, "", sb_ret.ToString(), "onchange='ChangePageSize(this)' style='width:50px;'")).ToString();
        }
        public static string GetSort(string key, string sortColumn, bool isAsc)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(sortColumn)) return string.Empty;
            if (key.ToLower() == sortColumn.ToLower())
            {
                if (isAsc)
                    return "<span class=\"asc\">&nbsp;</span>";
                else
                    return "<span class=\"desc\">&nbsp;</span>";
            }
            return string.Empty;
        }
        public static string GetSortHeader(string js, string title, string sortBy, bool isAsc, string currentSortBy)
        {
            return GetSortHeader(js, title, sortBy, isAsc, currentSortBy, "");
        }
        public static string GetSortHeader(string js, string title, string sortBy, bool isAsc, string currentSortBy, string style, bool isAppendStyle)
        {
            if (isAppendStyle)
                return string.Format(SortHeaderFmt, js, style, title, GetSort(currentSortBy, sortBy, isAsc));
            else
                return string.Format(SortHeaderFmt2, js, style, title, GetSort(currentSortBy, sortBy, isAsc));
        }
        public static string GetSortHeader(string js, string title, string sortBy, bool isAsc, string currentSortBy, string style)
        {
            return string.Format(SortHeaderFmt, js, style, title, GetSort(currentSortBy, sortBy, isAsc));
        }
        public static string MsgBoxHtml(string text, Page page)
        {
            string MsgBoxFormat = "<table width=\"100%\"><tr><td align=\"center\"><table border=\"0px\" cellpadding=\"0px\" cellspacing=\"0px\" width=\"450px\"><tr><td class=\"ad-lt\"></td><td class=\"ad-mt\"></td><td class=\"ad-rt\"></td></tr><tr><td class=\"ad-l\"></td><td class=\"ad-bg\" align=\"left\" style=\"height: 100px;\"><table width=\"90%\" border=\"0\"><tr><td style=\"padding: 5px 10px 5px 10px; width: 10%\">{0}</td><td align=\"left\" style=\"width: 90%\">{1}</td></tr></table></td><td class=\"ad-r\"></td></tr><tr><td class=\"ad-lb\"></td><td class=\"ad-mb\"></td><td class=\"ad-rb\"></td></tr></table></td></tr></table>";
            string Img = "<img src=\"" + page.ResolveUrl("~") + "images/error1.gif\" alt=\"\" />";
            return string.Format(MsgBoxFormat, Img, text);
        }
        public static string MsgBoxHtml(string text, Page page, string style)
        {
            string MsgBoxFormat = "<table width=\"100%\"><tr><td align=\"center\"><table border=\"0px\" cellpadding=\"0px\" cellspacing=\"0px\" style=\"{2}\"><tr><td class=\"ad-lt\" /><td class=\"ad-mt\" /><td class=\"ad-rt\" /></tr><tr><td class=\"ad-l\" /><td class=\"ad-bg\" align=\"left\" style=\"height: 100px;\"><table width=\"90%\" border=\"0\"><tr><td style=\"padding: 5px 10px 5px 10px; width: 10%\">{0}</td><td align=\"left\" style=\"width: 90%\">{1}</td></tr></table></td><td class=\"ad-r\" /></tr><tr><td class=\"ad-lb\" /><td class=\"ad-mb\" /><td class=\"ad-rb\" /></tr></table></td></tr></table>";
            string Img = "<img src=\"" + page.ResolveUrl("~") + "images/error1.gif\" alt=\"\" />";
            return string.Format(MsgBoxFormat, Img, text, style);
        }
        #endregion

        #region ReportView

        /// <summary>
        /// 返回导出工具条如果有Button，则selectFunName参数无效，否则，buttonFunName无效。
        /// </summary>
        /// <param name="selectId"></param>
        /// <param name="selectCss"></param>
        /// <param name="selectFunName"></param>
        /// <param name="isWithButton"></param>
        /// <param name="buttonId"></param>
        /// <param name="buttonCss"></param>
        /// <param name="buttonText"></param>
        /// <param name="buttonFunName"></param>
        /// <returns></returns>
        public static string GenReportViewerExportControl(string selectId, string selectCss, string selectFunName, bool isWithButton, string buttonId, string buttonCss, string buttonText, string buttonFunName)
        {
            StringBuilder sb_ret = new StringBuilder();
            StringBuilder sb_item = new StringBuilder();
            string[] EXPORT_SELECTOR = new string[3];
            EXPORT_SELECTOR[0] = "" + "," + Resources.Resource.SelectFormat;
            EXPORT_SELECTOR[1] = "pdf" + "," + Resources.Resource.ToPDF;
            EXPORT_SELECTOR[2] = "excel" + "," + Resources.Resource.ToExcel;
            foreach (string s in EXPORT_SELECTOR)
            {
                string[] info = s.Split(',');
                if (info[0] == string.Empty)
                    sb_item.Append(string.Format(STR_SELECT_ITEM_SELECTED, info[0], info[1]));
                else
                    sb_item.Append(string.Format(STR_SELECT_ITEM, info[0], info[1]));
            }
            string buttonString = string.Empty;
            string selectString = string.Empty;
            if (!isWithButton)
            {
                sb_ret.Append(string.Format(STR_SELECT_WITH_EVENT, selectId, selectCss, "", sb_item.ToString(), string.Format(" onchange=\"{0}\" ", selectFunName)));
                return sb_ret.ToString();
            }
            else
            {
                string selchangeFun = "if(document.getElementById('{0}').value==''){ document.getElementById('{1}').disabled=true;document.getElementById('{1}').style.cursor = 'default';document.getElementById('{1}').style.color = '#BFBFBF';}else{document.getElementById('{1}').disabled=false;document.getElementById('{1}').style.cursor = 'pointer';document.getElementById('{1}').style.color = '';} ";
                string selStr = selchangeFun.Replace("{0}", selectId).Replace("{1}", buttonId);
                sb_ret.Append(string.Format(STR_SELECT_WITH_EVENT, selectId, selectCss, "", sb_item.ToString(), string.Format(" onchange=\"{0}\" ", selStr)));
                sb_ret.Append(HtmlHelper.STR_WHITESPACE);
                sb_ret.Append(HtmlHelper.STR_WHITESPACE);
                sb_ret.Append(string.Format(HtmlHelper.STR_DISABLEBUTTON, buttonId, buttonCss, buttonFunName, buttonText));
                return sb_ret.ToString();
            }


        }

        #endregion

        #region Menu

        public const string MENU_MAIN = "<div id=\"tabs11\" style='font-weight: bold;'><ul>{0}<li></ul></div>";
        public const string MENU = "<li><a href=\"{0}\"><span>{1}</span></a></li>";
        public const string MENU_ACTIVE = "<li id=\"current\"><a href=\"{0}\"><span>{1}</span></a></li>";

        public const string MAIN_MENU = "<li class=\"m-n\"></li><li class=\"m-m-n\"><a href=\"{0}{1}\" style=\"color: #231f20;\">{2}</a></li><li class=\"m-r-n\"></li>";
        public const string MAIN_MENU_ACTIVE = "<li class=\"m-l-a\"></li><li class=\"m-m-a\"><a href=\"{0}{1}\" style=\"color: #fffe03;\">{2}</a></li><li class=\"m-r-a\"></li>";

        /// <summary>
        /// Get Menu Html
        /// </summary>
        /// <param name="p">Current Page Object</param>
        /// <param name="lMenus">List Item Like:"1,Home.aspx,Home|Home.aspx,Setup.aspx"</param>
        /// <returns></returns>
        public static string GenMenu(Page p, List<string> lMenus)
        {
            StringBuilder sb_menus = new StringBuilder();
            if (lMenus != null && lMenus.Count > 0)
            {
                string absolute_url = p.Request.Url.AbsolutePath;
                string resolve_url = p.ResolveUrl("~");
                foreach (string s in lMenus)
                {
                    string[] menu_info = s.Split('|');
                    if (menu_info != null && menu_info.Length > 1)
                    {
                        string[] menu_detail = menu_info[0].Split(',');
                        string[] menu_page_ref = menu_info[1].Split(',');
                        bool is_find_active_page = false;
                        if (menu_detail != null && menu_detail.Length > 2)
                        {
                            foreach (string pr in menu_page_ref)
                            {
                                if (absolute_url.ToLower().EndsWith(pr.ToLower()))
                                {
                                    sb_menus.Append(string.Format(MAIN_MENU_ACTIVE, resolve_url, menu_detail[1], menu_detail[2]));
                                    is_find_active_page = true;
                                    break;
                                }
                            }
                            if (!is_find_active_page)
                            {
                                sb_menus.Append(string.Format(MAIN_MENU, resolve_url, menu_detail[1], menu_detail[2]));
                            }
                        }
                    }
                }
            }
            return sb_menus.ToString();
        }

        #endregion

        #region Button

        public const string STR_BUTTON = "<input id=\"{0}\" class=\"{1}\" type=\"button\" onclick=\"{2}\" value=\"{3}\"/>";
        public const string STR_DISABLEBUTTON = "<input type=\"button\" id=\"{0}\" class=\"{1}\" onclick=\"{2}\" value=\"{3}\" style=\"cursor: default; color: #BFBFBF;\" enable=\"false\" disabled=\"\"/>";

        public static string GetButtonHtml(string id, string css, string onClick, string value, bool isDisabled)
        {
            if (isDisabled)
                return string.Format(STR_DISABLEBUTTON, id, css, onClick, value);
            else
                return string.Format(STR_BUTTON, id, css, onClick, value);
        }
        #endregion

        #region Other

        public static string GenCurrency(List<string> currencies, string id, string css, string selected_value, string title, string jsFunc, bool withAny)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            if (withAny)
            {
                d.Add("", CommonLibrary.Resources.Resource.Any);
            }
            if (currencies != null && currencies.Count > 0)
            {
                foreach (string c in currencies)
                {
                    if (d.ContainsKey(c)) continue;
                    d.Add(c, CommonLibrary.Resources.Currency.GetResource(c));
                }
            }
            return GenSelectWithoutVal(d, selected_value, id, css, title, jsFunc);
        }

        #endregion

    }
}
