using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CommonLibrary.WebObject
{
    public class WebHelper
    {
        public enum Browser
        {
            FireFox,
            IE5,
            IE6,
            IE7,
            IE8
        }

        public static string GetRequestInfo(System.Web.HttpRequest request)
        {
            if (request == null) return string.Empty;
            StringBuilder sbRet = new StringBuilder();
            sbRet.AppendLine("[Specific Information]");
            sbRet.Append("FORM DATA:").AppendLine(request.Form.ToString());
            sbRet.AppendLine("[Server Variables]");
            if (request.ServerVariables != null && request.ServerVariables.Count > 0)
            {
                for (int i = 0; i < request.ServerVariables.Count; i++)
                {
                    string sv = request.ServerVariables[i];
                    if (string.IsNullOrEmpty(sv)) continue;
                    sbRet.Append(request.ServerVariables.Keys[i]).Append(": ").AppendLine(sv);
                }
            }
            return sbRet.ToString();
        }

        public static string GetRequestUrl()
        {
            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null && System.Web.HttpContext.Current.Request.Url != null)
            {
                return System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            }
            return string.Empty;
        }

        public static string GetCurrentUrl()
        {
            string abPath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            abPath = abPath.Substring(1, abPath.Length - 1);

            string siteName = string.Empty;
            if (abPath.Split('/').Length > 1)
            {
                siteName = abPath.Substring(0, abPath.IndexOf("/"));
                if (!string.IsNullOrEmpty(siteName))
                    siteName = "/" + siteName;
            }
            return string.Format("http://{0}:{1}{2}/", System.Web.HttpContext.Current.Request.Url.Host, System.Web.HttpContext.Current.Request.Url.Port.ToString(), siteName);
        }

        public static bool IsFireFox(System.Web.UI.Page page)
        {
            return page.Request.ServerVariables["HTTP_USER_AGENT"].ToLower().IndexOf("firefox") > 0;
        }

        public static bool IsIE(System.Web.UI.Page page)
        {
            //return page.Request.ServerVariables["HTTP_USER_AGENT"].ToUpper().IndexOf("MSIE") > 0;
            return page.Request.Browser.Browser.ToUpper().IndexOf("IE") >= 0;
        }

        public static bool IsIE6(System.Web.UI.Page page)
        {
            return IsIE(page) && page.Request.Browser.Version.IndexOf("6.0") >= 0;
        }

        public static bool IsIE7(System.Web.UI.Page page)
        {
            return IsIE(page) && page.Request.Browser.Version.IndexOf("7.0") >= 0;
        }

        public static bool IsIE8(System.Web.UI.Page page)
        {
            return IsIE(page) && page.Request.Browser.Version.IndexOf("8.0") >= 0;
        }

        public static void WriteXml(System.Web.UI.Page page, string xml)
        {
            page.Response.Clear();
            page.Response.ContentType = "text/xml";
            page.Response.Write(xml);
            page.Response.End();
        }

        public static string GetBrowserInfo()
        {
            return GetBrowserInfo(System.Web.HttpContext.Current.Request);
        }

        public static string GetBrowserInfo(System.Web.HttpRequest request)
        {
            System.Text.StringBuilder sb_ret = new System.Text.StringBuilder();
            if (request != null)
            {
                sb_ret.Append("Browser:").Append(request.Browser.Browser);
                sb_ret.Append(",ID:").Append(request.Browser.Id);
                sb_ret.Append(",Version:").Append(request.Browser.Version);
                if (request.Browser.Beta)
                {
                    sb_ret.Append("(Beta)");
                }
            }
            return sb_ret.ToString();
        }

        public static string GetUserHostAddress()
        {
            return GetUserHostAddress(System.Web.HttpContext.Current.Request);
        }

        public static string GetUserHostAddress(System.Web.HttpRequest request)
        {
            if (request != null)
            {
                return request.UserHostAddress;
            }
            return "";
        }

        public static void DownLoadFile(string url, string fileName)
        {
            System.Net.WebClient req = new System.Net.WebClient();
            //Creating an instance of a credential cache, and passing the username and password to it
            System.Net.CredentialCache mycache = new System.Net.CredentialCache();
            mycache.Add(new Uri(url), "Basic", new System.Net.NetworkCredential());
            req.Credentials = mycache;

            //Creating an instance of a Response object
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.Clear();
            response.ClearContent();
            response.ClearHeaders();
            response.Buffer = true;

            //Keep the current page as it is, and writes the content to an new instance, which prompts the user to download the file 
            response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
            byte[] data = req.DownloadData(url);
            response.BinaryWrite(data);
            //response.Flush();
        }

        private static List<string> GetRequestKeys(RequestPostMethod method)
        {
            List<string> reqKeys = new List<string>();
            if (method == RequestPostMethod.Either)
                reqKeys.AddRange(System.Web.HttpContext.Current.Request.Form.AllKeys.Length > 0 ? System.Web.HttpContext.Current.Request.Form.AllKeys : System.Web.HttpContext.Current.Request.QueryString.AllKeys);
            else if (method == RequestPostMethod.Form)
                reqKeys.AddRange(System.Web.HttpContext.Current.Request.Form.AllKeys);
            else if (method == RequestPostMethod.Query)
                reqKeys.AddRange(System.Web.HttpContext.Current.Request.QueryString.AllKeys);
            else if (method == RequestPostMethod.Both)
            {
                reqKeys.AddRange(System.Web.HttpContext.Current.Request.Form.AllKeys);
                reqKeys.AddRange(System.Web.HttpContext.Current.Request.QueryString.AllKeys);
            }
            return reqKeys;
        }
        public static void SetValues<T>(T o, string prefix, string dateFormat)
            where T : class, new()
        {
            SetValues<T>(o, prefix, dateFormat, RequestPostMethod.Either);
        }

        public static void SetValues<T>(T o, string prefix, string dateFormat, RequestPostMethod method)
            where T : class, new()
        {
            if (o == null) return;

            foreach (string k in GetRequestKeys(method))
            {
                if (k.StartsWith(prefix))
                {
                    string column = k.Remove(0, prefix.Length);
                    if (prefix.EndsWith("_"))
                    {
                        column = column.Substring(0);
                    }
                    else
                    {
                        if (column.Length > 1)
                            column = column.Substring(1);
                    }
                    if (string.IsNullOrEmpty(column))
                        continue;
                    //DataMapping.ObjectHelper.SetValue(o, column, System.Web.HttpUtility.UrlDecode(System.Web.HttpContext.Current.Request[k]), CommonLibrary.Utility.DateHelper.GetDateString(CommonLibrary.Utility.DateHelper.DateFormat.ddMMyyyys));
                    DataMapping.ObjectHelper.SetValue(o, column, System.Web.HttpContext.Current.Request[k], dateFormat);
                }
            }
        }

        public static void SetValues<T>(T o, string prefix, Utility.DateHelper.DateFormat dateFormat, RequestPostMethod method)
                    where T : class, new()
        {
            SetValues(o, prefix, CommonLibrary.Utility.DateHelper.GetDateString(dateFormat), method);
        }

        public static void SetValues<T>(T o, string prefix)
            where T : class, new()
        {
            SetValues(o, prefix, CommonLibrary.Utility.DateHelper.GetDateString(CommonLibrary.Utility.DateHelper.DateFormat.ddMMyyyys));
        }

        public static void SetValues<T>(T o, string prefix, Utility.DateHelper.DateFormat dateFormat)
           where T : class, new()
        {
            SetValues(o, prefix, CommonLibrary.Utility.DateHelper.GetDateString(dateFormat));
        }

        public static List<T> GetListFromRequest<T>(string prefix)
            where T : class, new()
        {
            return GetListFromRequest<T>(prefix, CommonLibrary.Utility.DateHelper.DateFormat.ddMMyyyys, RequestPostMethod.Either);
        }

        public static List<T> GetListFromRequest<T>(string prefix, Utility.DateHelper.DateFormat dateFormat, RequestPostMethod method)
            where T : class, new()
        {
            return GetListFromRequest<T>(prefix, CommonLibrary.Utility.DateHelper.DateFormat.ddMMyyyys, method, null);
        }

        public static List<T> GetListFromRequest<T>(string prefix, Utility.DateHelper.DateFormat dateFormat, RequestPostMethod method, AfterSetValue<T> funcAfterSetValue)
            where T : class, new()
        {
            List<T> list = new List<T>();
            Type t = typeof(T);
            int maxId = Utility.NumberHelper.ToInt(HttpContext.Current.Request[string.Concat(prefix, "Num")], 0);
            if (maxId > 0)
            {
                for (int i = 1; i <= maxId; i++)
                {
                    bool isValid = false;
                    string k = "";
                    foreach (string key in GetRequestKeys(method))
                    {
                        if (key.StartsWith(string.Concat(prefix, i)))
                        {
                            isValid = true;
                            k = key;
                            break;
                        }
                    }
                    if (isValid)
                    {
                        if (t.IsClass)
                        {
                            T obj = new T();
                            WebHelper.SetValues<T>(obj, string.Concat(prefix, i), dateFormat, method);
                            if (funcAfterSetValue != null)
                            {
                                funcAfterSetValue(obj, prefix, i);
                            }
                            list.Add(obj);
                        }
                        else if (t.IsValueType)
                        {
                            T e = (T)Convert.ChangeType(HttpContext.Current.Request[k], t);
                            if (funcAfterSetValue != null)
                                funcAfterSetValue(e, prefix, i);
                            list.Add(e);
                        }
                    }
                }
            }
            return list;
        }

        public static List<T> GetListFromRequestForVal<T>(string prefix)
        {
            List<T> list = new List<T>();
            Type t = typeof(T);
            int maxId = Utility.NumberHelper.ToInt(HttpContext.Current.Request[string.Concat(prefix, "Num")], 0);
            if (maxId > 0)
            {
                for (int i = 1; i <= maxId; i++)
                {
                    bool isValid = false;
                    string k = "";
                    foreach (string key in HttpContext.Current.Request.Form.AllKeys.Length > 0 ? HttpContext.Current.Request.Form.AllKeys : HttpContext.Current.Request.QueryString.AllKeys)
                    {
                        if (key.StartsWith(string.Concat(prefix, i)))
                        {
                            isValid = true;
                            k = key;
                            break;
                        }
                    }
                    if (isValid)
                    {
                        T e;
                        if (t.IsValueType)
                        {
                            if (t.IsEnum)
                            {
                                e = (T)Enum.Parse(typeof(T), HttpContext.Current.Request[k]);
                            }
                            else
                                e = (T)Convert.ChangeType(HttpContext.Current.Request[k], t);
                            list.Add(e);
                        }
                        else if (t == typeof(string))
                        {
                            e = (T)Convert.ChangeType(HttpContext.Current.Request[k], t);
                            list.Add(e);
                        }
                    }
                }
            }
            return list;
        }

        public enum RequestPostMethod
        {
            Either,
            Form,
            Query,
            Both,
        }

        public delegate void AfterSetValue<T>(T obj, string prefix, int index)
            where T : class, new();
    }
}
