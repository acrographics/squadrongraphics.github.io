// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/recentcomments.aspx.cs 2     7/19/06 11:13p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Text;
using System.Globalization;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
	/// <summary>
	/// Summary description for recentcomments.
	/// </summary>
	public partial class recentcomments : SkinBase
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }

            // this may be overwridden by the XmlPackage below!
            SectionTitle = AppLogic.GetString("recentcomments.aspx.1", SkinID, ThisCustomer.LocaleSetting);

            // set the Customer context, and set the SkinBase context, so meta tags to be set if they are not blank in the XmlPackage results
            XmlPackage1.SetContext = this;
        }
	}
}
