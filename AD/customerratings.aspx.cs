// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/customerratings.aspx.cs 4     9/03/06 8:41p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.IO;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for customerratings
    /// </summary>
    public partial class customerratings : AspDotNetStorefront.SkinBase
    {

        private Customer TargetCustomer;
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            TargetCustomer = new Customer(CommonLogic.QueryStringUSInt("CustomerID"), true);
            if (TargetCustomer.CustomerID == 0)
            {
                Response.Redirect("customers.aspx");
            }
            if (CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
            {
                // delete the rating:
                Ratings.DeleteRating(CommonLogic.QueryStringUSInt("DeleteID"));
            }
            if (CommonLogic.QueryStringCanBeDangerousContent("ClearFilthyID").Length != 0)
            {
                DB.ExecuteSQL("update rating set IsFilthy=0 where RatingID=" + CommonLogic.QueryStringUSInt("ClearFilthyID").ToString());
            }
            SectionTitle = "<a href=\"customers.aspx\">Customers</a> - Product Ratings By: <a href=\"cst_account.aspx?customerid=" + TargetCustomer.CustomerID.ToString() + "\">" + TargetCustomer.FullName() + "</a>";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write(Ratings.DisplayForCustomer(TargetCustomer.CustomerID, SkinID));
        }

    }
}
