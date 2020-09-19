// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/lat_driver.aspx.cs 5     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
	/// <summary>
	/// Summary description for lat_driver.
	/// </summary>
	public partial class lat_driver : SkinBase
	{
        int AffiliateID = 0;
		protected void Page_Load(object sender, System.EventArgs e)
		{
            RequireSecurePage();
            AffiliateID = CommonLogic.CookieUSInt("LATAffiliateID");

            if (AffiliateID == 0 || !AppLogic.IsValidAffiliate(AffiliateID))
            {
                Response.Redirect("lat_signin.aspx?returnurl=" + Server.UrlEncode(CommonLogic.GetThisPageName(true) + "?" + CommonLogic.ServerVariables("QUERY_STRING")));
            }

            if (!IsPostBack)
            {
                UpdatePageContent();
            }
        }

        private void UpdatePageContent()
        {
            AppConfigAffiliateProgramName.Text = AppLogic.GetString("AppConfig.AffiliateProgramName", SkinID, ThisCustomer.LocaleSetting) + " Member Sign-Out";
            imgLogOut.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/logout.gif");
            AskAQuestion.NavigateUrl = "mailto:" + AppLogic.AppConfig("AffiliateEMailAddress");
            affiliateheader_small_gif.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/affiliateheader_small.jpg");

            String TN = CommonLogic.QueryStringCanBeDangerousContent("topic");
            AppLogic.CheckForScriptTag(TN);
            Topic t = new Topic(TN,ThisCustomer.LocaleSetting,SkinID,base.GetParser);
			if(t.Contents.Length == 0)
			{
				PageTopic.Text = "<img src=\"images/spacer.gif\" border=\"0\" height=\"100\" width=\"1\"><br/>\n";
				PageTopic.Text += "<p align=\"center\"><font class=\"big\"><b>This page is currently empty. Please check back again for an update.</b></font></p>";
			}
			else
			{
                PageTopic.Text = "<!-- READ FROM " + CommonLogic.IIF(t.FromDB, "DB", "FILE: " + t.FN) + ": " + " -->";
				PageTopic.Text += t.Contents.Replace("%AFFILIATEID%",AffiliateID.ToString());
                PageTopic.Text += "<!-- END OF " + CommonLogic.IIF(t.FromDB, "DB", "FILE: " + t.FN) + ": " + " -->";
			}
            
        }
	}
}
