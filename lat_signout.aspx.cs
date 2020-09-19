// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/lat_signout.aspx.cs 2     7/19/06 11:13p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
	/// <summary>
	/// Summary description for lat_signout.
	/// </summary>
	public partial class lat_signout : SkinBase
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			SectionTitle = AppLogic.GetString("AppConfig.AffiliateProgramName",SkinID,ThisCustomer.LocaleSetting) + " Signout";
			AppLogic.SetSessionCookie("LATAffiliateID",String.Empty);
            lblSignoutSuccess.Text = AppLogic.GetString("AppConfig.AffiliateProgramName",SkinID,ThisCustomer.LocaleSetting) + " sign-out complete, please wait...";
			Response.AddHeader("REFRESH","1; URL=t-affiliate.aspx");
		}

	}
}
