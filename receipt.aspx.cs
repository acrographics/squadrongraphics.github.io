// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/receipt.aspx.cs 6     9/13/06 11:13p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Net.Mail;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for receipt.
    /// </summary>
    public partial class receipt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            SkinBase.RequireSecurePage();

            int OrderNumber = CommonLogic.QueryStringUSInt("OrderNumber");
            int OrderCustomerID = Order.GetOrderCustomerID(OrderNumber);

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;// who is logged in now viewing this page:

            // currently viewing user must be logged in to view receipts:
            if (!ThisCustomer.IsRegistered)
            {
                Response.Redirect("signin.aspx?returnurl=receipt.aspx?" + Server.UrlEncode(CommonLogic.ServerVariables("QUERY_STRING")));
            }

            // are we allowed to view?
            // if currently logged in user is not the one who owns the order, and this is not an admin user who is logged in, reject the view:
            if (ThisCustomer.CustomerID != OrderCustomerID && !ThisCustomer.IsAdminUser)
            {
                Response.Redirect(SE.MakeDriverLink("ordernotfound"));
            }
            Order o = new Order(OrderNumber, ThisCustomer.LocaleSetting);
            Response.Write(o.Receipt(ThisCustomer, "", false));
        }

    }
}
