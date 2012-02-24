using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.WebObject
{
    public class OnlineUserTemplateMasterPage : System.Web.UI.MasterPage
    {
        override protected void OnInit(EventArgs e)
        {
            //if (Global.EnableSimultaneousLogin > 0)
            CommonLibrary.WebObject.SimultaneousLogin.Validate(Page, "~/Login.aspx?ko=1&lang=" + CommonLibrary.Utility.MutiLanguage.GetLanguageString());
            base.OnInit(e);
        }
    }
}
