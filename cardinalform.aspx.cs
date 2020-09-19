// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/cardinalform.aspx.cs 2     9/30/06 10:30p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for cardinalform.
    /// </summary>
    public partial class cardinalform : SkinBase
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            SectionTitle = "Order - Credit Card Verification:";
            if (ShoppingCart.CartIsEmpty(ThisCustomer.CustomerID, CartTypeEnum.ShoppingCart))
            {
                Response.Redirect("shoppingcart.aspx");
            }
            Panel1.Visible = ThisCustomer.IsAdminUser;
        }

    }
}
