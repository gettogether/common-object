using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CommonLibrary.WebObject
{
    public class SimultaneousLogin
    {
        public class LoginInformation
        {
            public LoginInformation()
            {

            }
            #region Attributes
            private int _ID;

            public int ID
            {
                get { return _ID; }
                set { _ID = value; }
            }

            private bool _IsKilled;

            public bool IsKilled
            {
                get { return _IsKilled; }
                set { _IsKilled = value; }
            }

            private string _Killer;

            public string Killer
            {
                get { return _Killer; }
                set { _Killer = value; }
            }
            private bool _IsImpropriate;

            public bool IsImpropriate
            {
                get { return _IsImpropriate; }
                set { _IsImpropriate = value; }
            }

            private string _Key;

            public string Key
            {
                get { return _Key; }
                set { _Key = value; }
            }
            private string _KeyLogin;

            public string KeyLogin
            {
                get { return _KeyLogin; }
                set { _KeyLogin = value; }
            }

            private string _CompanyCode;

            public string CompanyCode
            {
                get { return _CompanyCode; }
                set { _CompanyCode = value; }
            }

            private string _UserCode;

            public string UserCode
            {
                get { return _UserCode; }
                set { _UserCode = value; }
            }
            private string _Email;

            public string Email
            {
                get { return _Email; }
                set { _Email = value; }
            }
            private string _IP;

            public string IP
            {
                get { return _IP; }
                set { _IP = value; }
            }

            private string _UserName;

            public string UserName
            {
                get { return _UserName; }
                set { _UserName = value; }
            }
            private DateTime _LoginTime = DateTime.Now;

            public DateTime LoginTime
            {
                get { return _LoginTime; }
                set { _LoginTime = value; }
            }
            private string _Browser = System.Web.HttpContext.Current.Request.Browser.Type;

            public string Browser
            {
                get { return _Browser; }
                set { _Browser = value; }
            }

            #endregion
            public LoginInformation(string companyCode, string userCode, string email, string userName)
            {
                this.Key = System.Web.HttpContext.Current.Session.SessionID;
                this.CompanyCode = companyCode;
                this.UserCode = userCode;
                this.Email = email;
                this.UserName = userName;
                this.KeyLogin = companyCode + userCode;
            }
        }
        public class LoginInformationList : ObjectBase.ListBase<LoginInformation>
        {
            public LoginInformationList()
            {

            }
        }
        public const string KEY = "OnlineUser";

        public static LoginInformationList GetLoginInformationList(System.Web.UI.Page p)
        {
            LoginInformationList ret = new LoginInformationList();
            Hashtable hOnline = (Hashtable)p.Application[KEY];
            if (hOnline != null)
            {
                IDictionaryEnumerator idE = hOnline.GetEnumerator();
                while (idE.MoveNext())
                {
                    LoginInformation li = (LoginInformation)idE.Value;
                    if (li != null && !li.IsImpropriate && !li.IsKilled)
                    {
                        ret.Add((LoginInformation)idE.Value);
                    }
                }
            }
            return ret;
        }

        public static void ValidateOnSessionEnd(System.Web.HttpApplicationState app, System.Web.SessionState.HttpSessionState sess)
        {
            Hashtable hOnline = (Hashtable)app[KEY];
            if (hOnline[sess.SessionID] != null)
            {
                hOnline.Remove(sess.SessionID);
                app.Lock();
                app[KEY] = hOnline;
                app.UnLock();
            }
        }

        public static LoginInformation GetLogin(System.Web.UI.Page p, string loginKey)
        {
            Hashtable hOnline = (Hashtable)p.Application[KEY];
            if (hOnline != null)
            {
                IDictionaryEnumerator idE = hOnline.GetEnumerator();
                while (idE.MoveNext())
                {
                    LoginInformation li = (LoginInformation)idE.Value;
                    if (li != null && li.Key.Equals(loginKey))
                    {
                        return li;
                    }
                }
            }
            return null;
        }

        public static void KillLogin(System.Web.UI.Page p, string loginKey, string killer)
        {
            Hashtable hOnline = (Hashtable)p.Application[KEY];
            if (hOnline != null)
            {
                IDictionaryEnumerator idE = hOnline.GetEnumerator();
                string strKey = "";
                while (idE.MoveNext())
                {
                    LoginInformation li = (LoginInformation)idE.Value;
                    if (li != null && li.Key.Equals(loginKey))
                    {
                        strKey = idE.Key.ToString();
                        li.Killer = killer;
                        li.IsKilled = true;
                        hOnline[strKey] = li;
                        break;
                    }
                }
            }
            else
            {
                hOnline = new Hashtable();
            }
            p.Application.Lock();
            p.Application[KEY] = hOnline;
            p.Application.UnLock();
        }

        public static bool IsLoginAgain(System.Web.UI.Page p, LoginInformation loginInfo)
        {
            loginInfo.IP = p.Request.UserHostAddress;
            Hashtable hOnline = (Hashtable)p.Application[KEY];
            if (hOnline != null)
            {
                IDictionaryEnumerator idE = hOnline.GetEnumerator();
                while (idE.MoveNext())
                {
                    LoginInformation li = (LoginInformation)idE.Value;
                    if (li != null && li.KeyLogin.Equals(loginInfo.KeyLogin) && !li.IsKilled)
                    {
                        if (li.IP.Equals(loginInfo.IP))
                        {
                            li.LoginTime = DateTime.Now;
                            return false;
                        }
                        return true;
                    }
                }
            }
            else
            {
                hOnline = new Hashtable();
            }
            hOnline[p.Session.SessionID] = loginInfo;
            p.Application.Lock();
            p.Application[KEY] = hOnline;
            p.Application.UnLock();
            return false;
        }

        public static bool IsLoginAgain(LoginInformation loginInfo)
        {
            loginInfo.IP = System.Web.HttpContext.Current.Request.UserHostAddress;
            Hashtable hOnline = (Hashtable)System.Web.HttpContext.Current.Application[KEY];
            if (hOnline != null)
            {
                IDictionaryEnumerator idE = hOnline.GetEnumerator();
                while (idE.MoveNext())
                {
                    LoginInformation li = (LoginInformation)idE.Value;
                    if (li != null && li.KeyLogin.Equals(loginInfo.KeyLogin) && !li.IsKilled)
                    {
                        if (li.IP.Equals(loginInfo.IP))
                        {
                            li.LoginTime = DateTime.Now;
                            return false;
                        }
                        return true;
                    }
                }
            }
            else
            {
                hOnline = new Hashtable();
            }
            hOnline[System.Web.HttpContext.Current.Session.SessionID] = loginInfo;
            System.Web.HttpContext.Current.Application.Lock();
            System.Web.HttpContext.Current.Application[KEY] = hOnline;
            System.Web.HttpContext.Current.Application.UnLock();
            return false;
        }

        public static bool Login(System.Web.UI.Page p, LoginInformation loginInfo)
        {
            loginInfo.IP = p.Request.UserHostAddress;
            Hashtable hOnline = (Hashtable)p.Application[KEY];
            if (hOnline == null)
            {
                hOnline = new Hashtable();
            }
            hOnline[p.Session.SessionID] = loginInfo;
            p.Application.Lock();
            p.Application[KEY] = hOnline;
            p.Application.UnLock();
            return false;
        }

        public static bool Login(LoginInformation loginInfo)
        {
            loginInfo.IP = System.Web.HttpContext.Current.Request.UserHostAddress;
            Hashtable hOnline = (Hashtable)System.Web.HttpContext.Current.Application[KEY];
            if (hOnline == null)
            {
                hOnline = new Hashtable();
            }
            hOnline[System.Web.HttpContext.Current.Session.SessionID] = loginInfo;
            System.Web.HttpContext.Current.Application.Lock();
            System.Web.HttpContext.Current.Application[KEY] = hOnline;
            System.Web.HttpContext.Current.Application.UnLock();
            return false;
        }

        public static void ValidateReplaceLogin(System.Web.UI.Page p, LoginInformation loginInfo)
        {
            loginInfo.IP = p.Request.UserHostAddress;
            Hashtable hOnline = (Hashtable)p.Application[KEY];
            if (hOnline != null)
            {
                IDictionaryEnumerator idE = hOnline.GetEnumerator();
                string strKey = "";
                while (idE.MoveNext())
                {
                    LoginInformation li = (LoginInformation)idE.Value;
                    if (li != null && li.KeyLogin.Equals(loginInfo.KeyLogin))
                    {
                        strKey = idE.Key.ToString();
                        li.IsImpropriate = true;
                        hOnline[strKey] = li;
                        break;
                    }
                }
            }
            else
            {
                hOnline = new Hashtable();
            }
            hOnline[p.Session.SessionID] = loginInfo;
            p.Application.Lock();
            p.Application[KEY] = hOnline;
            p.Application.UnLock();
        }

        public static void ValidateReplaceLogin(LoginInformation loginInfo)
        {
            loginInfo.IP = System.Web.HttpContext.Current.Request.UserHostAddress;
            Hashtable hOnline = (Hashtable)System.Web.HttpContext.Current.Application[KEY];
            if (hOnline != null)
            {
                IDictionaryEnumerator idE = hOnline.GetEnumerator();
                string strKey = "";
                while (idE.MoveNext())
                {
                    LoginInformation li = (LoginInformation)idE.Value;
                    if (li != null && li.KeyLogin.Equals(loginInfo.KeyLogin))
                    {
                        strKey = idE.Key.ToString();
                        li.IsImpropriate = true;
                        hOnline[strKey] = li;
                        break;
                    }
                }
            }
            else
            {
                hOnline = new Hashtable();
            }
            hOnline[System.Web.HttpContext.Current.Session.SessionID] = loginInfo;
            System.Web.HttpContext.Current.Application.Lock();
            System.Web.HttpContext.Current.Application[KEY] = hOnline;
            System.Web.HttpContext.Current.Application.UnLock();
        }

        public static void Validate(System.Web.UI.Page p, string redirectUrl)
        {
            Hashtable hOnline = (Hashtable)p.Application[KEY];
            if (hOnline != null)
            {
                //IDictionaryEnumerator idE = hOnline.GetEnumerator();
                LoginInformation loginInfo = (LoginInformation)hOnline[p.Session.SessionID];
                if (loginInfo == null) return;
                if (loginInfo.IsImpropriate)
                {
                    hOnline.Remove(p.Session.SessionID);
                    p.Application.Lock();
                    p.Application[KEY] = hOnline;
                    p.Application.UnLock();
                    p.Session.Abandon();
                    p.Response.Redirect(redirectUrl);
                }
                else if (loginInfo.IsKilled)
                {
                    hOnline.Remove(p.Session.SessionID);
                    p.Application.Lock();
                    p.Application[KEY] = hOnline;
                    p.Application.UnLock();
                    p.Session.Abandon();
                    p.Response.Redirect(redirectUrl + "&killer=" + loginInfo.Killer);
                }
                //if (loginInfo == null) return;
                //while (idE.MoveNext())
                //{
                //    LoginInformation li = (LoginInformation)idE.Value;
                //    if (li != null && li.Key.Equals(loginInfo.Key))
                //    {
                //        string v = idE.Value.ToString();
                //        if (idE.Value != null && v.StartsWith(FLAG))
                //        {
                //            hOnline.Remove(p.Session.SessionID);
                //            p.Application.Lock();
                //            p.Application[KEY] = hOnline;
                //            p.Application.UnLock();
                //            p.Session.Abandon();
                //            int i = v.IndexOf(':');
                //            string killer = v.Substring((i + 1), (v.Length - i - 1));
                //            if (string.IsNullOrEmpty(killer))
                //            {
                //                p.Response.Redirect(redirectUrl);
                //            }
                //            else
                //            {
                //                p.Response.Redirect(redirectUrl + "&killer=" + killer);
                //            }
                //        }
                //        break;
                //    }
                //}
            }
        }
    }
}
