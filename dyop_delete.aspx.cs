// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/dyop_delete.aspx.cs 5     8/19/06 8:46p Buddy $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using AspDotNetStorefrontCommon;


namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for dyop_delete.
    /// </summary>
    public partial class dyop_delete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            int PackID = CommonLogic.QueryStringUSInt("PackID");
            int ProductID = CommonLogic.QueryStringUSInt("ProductID");
            int CategoryID = CommonLogic.QueryStringUSInt("CategoryID");
            int SectionID = CommonLogic.QueryStringUSInt("SectionID");
            int DeleteID = CommonLogic.QueryStringUSInt("DeleteID");
            string SCartRecID = CommonLogic.QueryStringCanBeDangerousContent("cartrecid");

            if (DeleteID != 0)
            {
                CustomCart.DeleteItem(DeleteID, CartTypeEnum.ShoppingCart);
            }

            String url = SE.MakeProductAndEntityLink(CommonLogic.QueryStringCanBeDangerousContent("entityname"), PackID, CommonLogic.QueryStringUSInt("entityid"), "");
            Response.Redirect(url + CommonLogic.IIF(SCartRecID == "", "", "?cartrecid=" + SCartRecID));
        }

    }
}
