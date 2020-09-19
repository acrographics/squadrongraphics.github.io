// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/manageratings.aspx.cs 4     9/03/06 10:00p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for manageratings.
	/// </summary>
	public partial class manageratings : AspDotNetStorefront.SkinBase
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache"); 
            

			SectionTitle = "Manage Ratings";
		}

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
		{
			if(CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
			{
				// delete the rating:
				Ratings.DeleteRating(CommonLogic.QueryStringUSInt("DeleteID"));
			}

			if(CommonLogic.QueryStringCanBeDangerousContent("ClearFilthyID").Length != 0)
			{
				DB.ExecuteSQL("update rating set IsFilthy=0 where RatingID=" + CommonLogic.QueryStringUSInt("ClearFilthyID").ToString());
			}

			writer.Write("<form method=\"GET\" action=\"manageratings.aspx\" name=\"SearchForm2\">\n");
			writer.Write("    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\n");
			writer.Write("      <tr align=\"left\">\n");
			writer.Write("        <td width=\"25%\">Search For Comment:</td>\n");
			writer.Write("        <td width=\"75%\">\n");
			writer.Write("          <input type=\"text\" name=\"SearchTerm\" size=\"25\" maxlength=\"70\" value=\"" + Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("SearchTerm")) + "\">\n");
			writer.Write("          <input type=\"hidden\" name=\"SearchTerm_Vldt\" value=\"[req][blankalert=Please enter something to search for!]\">\n");
			writer.Write("          &nbsp;<input type=\"submit\" value=\"Search\" name=\"B1\">&nbsp;&nbsp;&nbsp;&nbsp;<a href=\"manageratings.aspx?filthy=1\">SHOW FILTHY</a></td>\n");
			writer.Write("      </tr>\n");
			writer.Write("    </table>\n");
			writer.Write("</form>\n");

			String st = CommonLogic.QueryStringCanBeDangerousContent("SearchTerm").Trim();

			if(st.Length != 0)
			{
				writer.Write("<p align=\"left\"><b>Product Ratings Matching: " + st + "</b></p>");
			}
			if(CommonLogic.QueryStringCanBeDangerousContent("filthy").Length != 0)
			{
				writer.Write(Ratings.DisplayFilthy(ThisCustomer,SkinID));
			}
			else
			{
				writer.Write(Ratings.DisplayMatching(ThisCustomer,st,SkinID));
			}
		}

	}
}
