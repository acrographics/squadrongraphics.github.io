// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/clickout.aspx.cs 3     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
	/// <summary>
	/// Summary description for clickout.
	/// </summary>
	public partial class clickout : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");

            AppLogic.CheckForScriptTag(CommonLogic.QueryStringCanBeDangerousContent("Name"));
            AppLogic.CheckForScriptTag(CommonLogic.QueryStringCanBeDangerousContent("URL"));

            DB.ExecuteSQL("insert into ClickTrack(Name) values(" + DB.SQuote(CommonLogic.Left(Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("Name")), 100)) + ")");

			Response.Redirect(CommonLogic.QueryStringCanBeDangerousContent("URL"));
		}

	}
}
