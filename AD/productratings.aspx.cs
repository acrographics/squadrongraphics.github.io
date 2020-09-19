// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/productratings.aspx.cs 4     9/03/06 8:42p Redwoodtree $
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
    /// Summary description for productratings
    /// </summary>
    public partial class productratings : AspDotNetStorefront.SkinBase
    {

        private int ProductID;
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            ProductID = CommonLogic.QueryStringUSInt("ProductID");
            if (ProductID == 0)
            {
                Response.Redirect("products.aspx");
            }
            if (CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
            {
                // delete the rating:
                Ratings.DeleteRating(CommonLogic.QueryStringUSInt("DeleteID"));
            }
            SectionTitle = "<a href=\"products.aspx\">Products</a> - Product Ratings For: " + AppLogic.GetProductName(ProductID, ThisCustomer.LocaleSetting);
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write(Ratings.Display(ThisCustomer, ProductID, 0, 0, 0, SkinID));
        }

    }
}
