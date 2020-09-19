// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/resetsqllog.aspx.cs 3     9/03/06 8:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for resetsqllog.
	/// </summary>
    public partial class resetsqllog : AspDotNetStorefront.SkinBase
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache"); 
            

		}

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
		{
			writer.Write("<img src=\"../images/spacer.gif\" border=\"0\" height=\"100\" width=\"1\"><br/>\n");
			if(ThisCustomer.IsAdminSuperUser)
			{
				DB.ExecuteSQL("truncate table SQLLog");
				writer.Write("<p align=\"center\"><font class=\"big\"><b>Done.</b></font></p>");
			}
			else
			{
				writer.Write("<p align=\"center\"><font class=\"big\" color=red><b>INSUFFICIENT PRIVILEGE</b></font></p>");
			}
			
		}

	}
}
