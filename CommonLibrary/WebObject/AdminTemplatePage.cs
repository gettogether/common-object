using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.WebObject
{
    public class AdminTemplatePage : System.Web.UI.Page
    {
        #region Property
        public int type
        {
            get
            {
                return Utility.NumberHelper.ToInt(Request["type"], 0);
            }
        }

        public string key
        {
            get
            {
                return Request["key"];
            }
        }

        public int PageIndex
        {
            get { return Utility.NumberHelper.ToInt(Request["page"], 1); }
        }

        public int PageSize
        {
            get { return Utility.NumberHelper.ToInt(Request["size"], 20); }
        }

        private string _Sort;
        public string Sort
        {
            get
            {
                if (!string.IsNullOrEmpty(Request["sort"]))
                {
                    _Sort = Request["sort"];
                    return _Sort;
                }
                return _Sort;
            }
            set
            {
                _Sort = value;
            }
        }

        private bool _IsAsc;
        public bool IsAsc
        {
            get
            {
                if (!string.IsNullOrEmpty(Request["asc"]))
                {
                    _IsAsc = Request["asc"] == "Y";
                }
                return _IsAsc;
            }
            set
            {
                _IsAsc = value;
            }
        }

        private const string ADMIN_ADDRESS_KEY = "admin_address_key";

        #endregion

        protected override void OnPreLoad(EventArgs e)
        {
            List<string> adminAddress = Utility.CacheHelper.GetCache(ADMIN_ADDRESS_KEY) as List<string>;
            if (adminAddress == null && !string.IsNullOrEmpty(Utility.ConfigHelper.GetAppSetting("AdminAddress")))
            {
                adminAddress = new List<string>();
                adminAddress.AddRange(Utility.ConfigHelper.GetAppSetting("AdminAddress").Split(','));
                Utility.CacheHelper.SetCache(ADMIN_ADDRESS_KEY, adminAddress);
            }
            if (adminAddress != null && adminAddress.Count > 0 && !adminAddress.Contains(Request.UserHostAddress))
            {
                Response.Write("You have no right to view this page.");
                Response.End();
            }
            base.OnPreLoad(e);
        }

        public void OK(string extraMsg)
        {
            string r = string.IsNullOrEmpty(extraMsg) ? Definition.OK_FLAG : string.Concat(Definition.OK_FLAG, Definition.SPLIT_EXTRA_FALAG, extraMsg);
            Response.Write(r);
        }
        public void OK()
        {
            OK("");
        }
    }
}
