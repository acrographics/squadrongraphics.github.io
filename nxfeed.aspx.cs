 // ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/nxfeed.aspx.cs 3     10/04/06 6:20a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Globalization;
using AspDotNetStorefrontCommon;
//using AspDotNetStorefrontPatterns;

namespace AspDotNetStorefront
{
	/// <summary>
    /// Summary description for nxfeed.
	/// </summary>
    public partial class nxfeed : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
            String FeedDocument = AppLogic.RunXmlPackage("feed.nextopia.xml.config", null, null, 1, String.Empty, String.Empty, false, false);
            Response.Write(FeedDocument);
        }

	}
}
