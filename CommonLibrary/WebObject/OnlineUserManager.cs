using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
using System.Collections;

namespace CommonLibrary.WebObject
{
    public class OnlineUserManager
    {
        #region Property
        public const string SPLIT_KEY_FLAG = "^,^";
        #endregion

        #region Function

        public static bool KillLogin(string url, string key, string killer, out string errMsg)
        {
            url = string.Concat(url, "?type=1&key=", key, "&killer=", killer);
            string rs = Utility.RequestHelper.GetRequest(url, 0);
            bool isSuccess = rs == Definition.OK_FLAG;
            errMsg = isSuccess ? string.Empty : rs;
            return isSuccess;
        }
        public static bool KillLogins(string url, string[] keys, string killer, out string errMsg)
        {
            url = string.Concat(url, "?type=1&keys=", HttpUtility.UrlEncode(Utility.StringHelper.ArrayToString(keys, SPLIT_KEY_FLAG)), "&killer=", killer);
            string rs = Utility.RequestHelper.GetRequest(url, 0);
            bool isSuccess = rs == Definition.OK_FLAG;
            errMsg = isSuccess ? string.Empty : rs;
            return isSuccess;
        }
        public static OnlineUsers GetOnlineUserList(string url, int pageIndex, int pageSize, string sort, bool isAsc, out string errMsg)
        {
            //errMsg = string.Empty;
            //url = string.Concat(url, "?type=0", "&page=", pageIndex, "&size=", pageSize, "&sort=", sort, "&asc=", isAsc ? "Y" : "N");
            //string result = Utility.RequestHelper.GetRequest(url, 0);
            //try
            //{
            //    return CommonLibrary.Utility.SerializationHelper.FromXml<OnlineUsers>(result);
            //}
            //catch (Exception ex)
            //{
            //    errMsg = ex.Message;
            //    return null;
            //}
            return GetOnlineUserList(url, pageIndex, pageSize, sort, isAsc, "", "", "", "", out errMsg);
        }

        public static OnlineUsers GetOnlineUserList(string url, int pageIndex, int pageSize, string sort, bool isAsc, string company, string userName, string email, string browser, out string errMsg)
        {
            errMsg = string.Empty;
            url = string.Concat(url, "?type=0", "&page=", pageIndex, "&size=", pageSize, "&sort=", sort, "&asc=", isAsc ? "Y" : "N", "&ol_company=", company, "&ol_username=", userName, "&ol_email=", email, "&ol_browser=", browser);
            string result = Utility.RequestHelper.GetRequest(url, 0);
            try
            {
                return CommonLibrary.Utility.SerializationHelper.FromXml<OnlineUsers>(result);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return null;
            }
        }

        public class OnlineUsers
        {
            private SimultaneousLogin.LoginInformationList _OnlineUserList;
            public SimultaneousLogin.LoginInformationList OnlineUserList
            {
                get { return _OnlineUserList; }
                set { _OnlineUserList = value; }
            }

            private int _Total;
            public int Total
            {
                get { return _Total; }
                set { _Total = value; }
            }
        }
        #endregion
    }
}
