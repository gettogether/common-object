using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.WebObject
{
    public class OnlineUserTemplatePage : AdminTemplatePage
    {
        public string Company
        {
            get { return Request["ol_company"]; }
        }

        public string UserName
        {
            get { return Request["ol_username"]; }
        }

        public string Email
        {
            get { return Request["ol_email"]; }
        }

        public string Browser
        {
            get { return Request["ol_browser"]; }
        }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            if (type == 0)
            {
                Response.Clear();
                Response.Write(CommonLibrary.Utility.SerializationHelper.SerializeToXml(GetOnlineUsers(PageIndex, PageSize, Sort, IsAsc)));
            }
            else if (type == 1)
            {
                string killer = Request.UserHostAddress;
                if (!string.IsNullOrEmpty(Request["killer"])) killer = string.Concat(killer, "-", Request["killer"]);
                if (!string.IsNullOrEmpty(key))
                {
                    SimultaneousLogin.KillLogin(this.Page, key, killer);
                    OK();
                }
                else if (!string.IsNullOrEmpty(Request["keys"]))
                {
                    foreach (string k in Request["keys"].Split(new string[] { OnlineUserManager.SPLIT_KEY_FLAG }, StringSplitOptions.RemoveEmptyEntries))
                        SimultaneousLogin.KillLogin(this.Page, k, killer);
                    OK();
                }
            }
            Response.End();
        }

        //public OnlineUserManager.OnlineUsers GetOnlineUsers(int pageIndex, int pageSize, string sort, bool isAsc)
        //{
        //    //OnlineUserManager.OnlineUsers users = new OnlineUserManager.OnlineUsers();
        //    //WebObject.SimultaneousLogin.LoginInformationList list = SimultaneousLogin.GetLoginInformationList(this.Page);
        //    //users.OnlineUserList = new SimultaneousLogin.LoginInformationList();
        //    //users.OnlineUserList.AddRange(list.GetPaging(pageSize, pageIndex, sort, isAsc));
        //    //users.Total = list.Count;
        //    //return users;
        //    return GetOnlineUsers(pageIndex, pageSize, sort, isAsc, null, null, null, null);
        //}

        public OnlineUserManager.OnlineUsers GetOnlineUsers(int pageIndex, int pageSize, string sort, bool isAsc)
        {
            OnlineUserManager.OnlineUsers users = new OnlineUserManager.OnlineUsers();
            WebObject.SimultaneousLogin.LoginInformationList list = SimultaneousLogin.GetLoginInformationList(this.Page);
            WebObject.SimultaneousLogin.LoginInformationList list_result = new SimultaneousLogin.LoginInformationList();
            if (string.IsNullOrEmpty(Company) && string.IsNullOrEmpty(UserName) && string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Browser))
            {
                list_result = list;
            }
            else
            {
                foreach (WebObject.SimultaneousLogin.LoginInformation li in list)
                {
                    if (!string.IsNullOrEmpty(Company) && li.CompanyCode.ToUpper().IndexOf(Company.ToUpper()) < 0) continue;
                    if (!string.IsNullOrEmpty(UserName) && li.UserName.ToUpper().IndexOf(UserName.ToUpper()) < 0) continue;
                    if (!string.IsNullOrEmpty(Email) && li.Email.ToUpper().IndexOf(Email.ToUpper()) < 0) continue;
                    if (!string.IsNullOrEmpty(Browser) && li.Browser.ToUpper().IndexOf(Browser.ToUpper()) < 0) continue;
                    list_result.Add(li);
                }
            }
            users.OnlineUserList = new SimultaneousLogin.LoginInformationList();
            users.OnlineUserList.AddRange(list_result.GetPaging(pageSize, pageIndex, sort, isAsc));
            users.Total = list_result.Count;
            return users;
        }
    }


}
