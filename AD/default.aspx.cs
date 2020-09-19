// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/default.aspx.cs 4     9/03/06 8:41p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for _default1.
	/// </summary>
    public partial class _default : AspDotNetStorefront.SkinBase
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            if (CommonLogic.QueryStringCanBeDangerousContent("flushcache").Length != 0 || CommonLogic.QueryStringCanBeDangerousContent("resetcache").Length != 0 || CommonLogic.QueryStringCanBeDangerousContent("clearcache").Length != 0)
            {
                // make sure all products have a variant. this is a bit of a hack doing it here, but was required by tech support:
                DB.ExecuteSQL("exec aspdnsf_CreateMissingVariants");
                AppLogic.ApplicationStart(false);
                Response.Redirect("default.aspx");
            }
		}

		override protected void OnInit(EventArgs e)
		{
			SetTemplate("main.ascx");
			base.OnInit(e);
		}

	}
}
