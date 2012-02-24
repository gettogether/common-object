using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.WebObject
{
    public class Paging
    {
        //GetPagingString(5, PageIndex, PageSize, Result.Count, ResolveUrl("~/Air/Search.aspx"), "");
        public static string GetPagingString(int ButtonCount, int PageIndex, decimal PageSize, int RecordCount, string PageUrl, string PageJS,bool isShowOmit)
        {
            if (PageSize <= 0) return string.Empty;
            if (PageIndex <= 0) PageIndex = 1;
            if (RecordCount == 0 || RecordCount <= PageSize) return string.Empty;
            decimal page_size = Utility.NumberHelper.Rounding((RecordCount / PageSize), Utility.NumberHelper.RoundingTypes.Ceiling, 0);
            if (page_size <= 1) return "";
            string main_footer = "<div class=\"digg\">{0}</div>";
            string disabled_previous = "<span class=\"disabled\">< </span>";
            string disabled_next = "<span class=\"disabled\">> </span>";
            string current = string.Format("<span class=\"current\">{0}</span>", PageIndex);
            string link = string.Empty;
            string page_link = string.Empty;
            if (PageJS != string.Empty)
            {
                link = "<a class=\"Hand\" onclick=\"" + PageJS + "\">{1}</a>";
                page_link = "<a class=\"Hand\" onclick=\"" + PageJS + "\">{0}</a>";
            }
            else
            {
                string flag = "?";
                if (PageUrl.IndexOf("?") > 0) flag = "&";
                link = "<a class=\"Hand\" href=\"" + PageUrl + flag + "page={0}\">{1}</a>";
                page_link = "<a class=\"Hand\" href='" + PageUrl + flag + "page={0}'>{0}</a>";
            }
            string ret = "";
            if (PageIndex > ButtonCount)
            {
                ret += string.Format(link, 1, "|<");
            }
            if (PageIndex == 1)
            {
                ret += disabled_previous;
            }
            else
            {
                ret += string.Format(link, (PageIndex - 1), "<");
            }


            int start = PageIndex - ButtonCount;
            if (start <= 0) start = 1;
            for (int i = start; i <= page_size; i++)
            {
                if (isShowOmit)
                {
                    if (i == PageIndex)
                    {
                        ret += string.Format(current, i);
                    }
                    else
                    {
                        if (i > PageIndex + 2 && PageIndex >= 3 || PageIndex < 3 && i > 5)
                        {
                            ret += string.Concat("<label>бнбн</label>");
                            break;
                        }
                        if (PageIndex >= page_size - ButtonCount && i >= page_size - 4 && i < page_size - 2 || PageIndex < 3 && i <= 5 || i < PageIndex && i >= PageIndex - 2 || i > PageIndex && i <= PageIndex + 2)
                        {
                            ret += string.Format(page_link, i.ToString());
                        }
                    }
                }
                else
                {
                    if (i <= ((PageIndex < ButtonCount ? ButtonCount : PageIndex) + ButtonCount) || (i == 1 && i < 6))
                    {
                        if (i == PageIndex)
                        {
                            ret += string.Format(current, i);
                        }
                        else
                        {
                            ret += string.Format(page_link, i.ToString());
                        }
                    }
                }
            }
            if (PageIndex == page_size)
            {
                ret += disabled_next;
            }
            else
            {
                ret += string.Format(link, (PageIndex + 1), ">");
            }
            if (PageIndex != page_size && RecordCount / PageSize > ButtonCount * 2)
            {
                ret += string.Format(link, (page_size), ">|");
            }

            return string.Format(main_footer, ret);
        }

        public static string GetPagingString(int ButtonCount, int PageIndex, decimal PageSize, int RecordCount, string PageUrl, string PageJS)
        {
            return GetPagingString(ButtonCount, PageIndex, PageSize, RecordCount, PageUrl, PageJS,false);
        }
    }
}

/* css
div.digg
{
	float: left;
	padding: 3px;
	margin: 0px;
}
DIV.digg A
{
	border-right: #184785 1px solid;
	padding-right: 5px;
	border-top: #184785 1px solid;
	padding-left: 5px;
	padding-bottom: 2px;
	margin: 2px;
	border-left: #184785 1px solid;
	color: #184785;
	padding-top: 2px;
	border-bottom: #184785 1px solid;
	text-decoration: none;
}
DIV.digg A:hover
{
	border-right: #184785 1px solid;
	border-top: #184785 1px solid;
	border-left: #184785 1px solid;
	color: #184785;
	border-bottom: #184785 1px solid;
	background-color: #BFCAE6;
}
DIV.digg A:active
{
	border-right: #FF9A40 1px solid;
	border-top: #FF9A40 1px solid;
	border-left: #FF9A40 1px solid;
	color: #000;
	border-bottom: #FF9A40 1px solid;
}
DIV.digg SPAN.current
{
	border-right: #184785 1px solid;
	padding-right: 5px;
	border-top: #184785 1px solid;
	padding-left: 5px;
	font-weight: bold;
	padding-bottom: 2px;
	margin: 2px;
	border-left: #184785 1px solid;
	color: #fff;
	padding-top: 2px;
	border-bottom: #184785 1px solid;
	background-color: #184785;
}
DIV.digg SPAN.disabled
{
	border-right: #BFCAE6 1px solid;
	padding-right: 5px;
	border-top: #BFCAE6 1px solid;
	padding-left: 5px;
	padding-bottom: 2px;
	margin: 2px;
	border-left: #BFCAE6 1px solid;
	color: #BFCAE6;
	padding-top: 2px;
	border-bottom: #BFCAE6 1px solid;
}
*/