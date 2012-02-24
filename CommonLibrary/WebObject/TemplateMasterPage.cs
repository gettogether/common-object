using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.WebObject
{
    public class TemplateMasterPage : System.Web.UI.MasterPage
    {
        //protected override void OnPreRender(EventArgs e)
        //{
        //    //if (ADHolidaysLibrary.SessionHelper.GetProfiles() == null)
        //    //{
        //    //    Response.Redirect("~/Login.aspx?to=1&lang=" + CommonLibrary.Utility.MutiLanguage.GetLanguageString(), true);
        //    //}
        //    base.OnPreRender(e);
        //}

        override protected void OnInit(EventArgs e)
        {
            //if (Global.EnableSimultaneousLogin > 0)
            CommonLibrary.WebObject.SimultaneousLogin.Validate(Page, "~/Login.aspx?ko=1&lang=" + CommonLibrary.Utility.MutiLanguage.GetLanguageString());
            base.OnInit(e);
        }

        //protected override void OnLoad(EventArgs e)
        //{
        //    if (ADHolidaysLibrary.SessionHelper.GetProfiles() != null && ADHolidaysLibrary.SessionHelper.GetProfiles().User.IsChangePassword)
        //    {
        //        Response.Redirect("~/Admin/ChangePassword.aspx");
        //    }
        //    base.OnLoad(e);
        //}

        public void ResponseOfNoRights()
        {
            Response.Write("You have no right to view this page.");
            Response.End();
        }

        public void CheckRight(bool allowed)
        {
            if (!allowed) ResponseOfNoRights();
        }
    }
}
