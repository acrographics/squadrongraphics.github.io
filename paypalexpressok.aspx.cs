// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/paypalexpressok.aspx.cs 5     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using AspDotNetStorefrontCommon;
using AspDotNetStorefrontGateways;
using AspDotNetStorefrontEncrypt;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for paypalexpressok.
    /// </summary>
    public partial class paypalexpressok : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            String PayPalToken = CommonLogic.QueryStringCanBeDangerousContent("token");
            AppLogic.CheckForScriptTag(PayPalToken);
            
            String PayerID = Gateway.GetExpressCheckoutDetails(PayPalToken, ThisCustomer.CustomerID);

            if (String.IsNullOrEmpty(PayerID))
            { // If there is no PayerID then abort the transaction.
                Response.Redirect(AppLogic.AppConfig("PayPal.Express.CancelURL"));
                return;
            }
           
            if (!ThisCustomer.IsRegistered && !ThisCustomer.IsOver13)
            { // Set the Over13Checked flag since PayPal is permitting the order process, so they don't get rejected.
                DB.ExecuteSQL("update Customer set Over13Checked=1 where CustomerID=" + ThisCustomer.CustomerID.ToString());
            }
            ThisCustomer.ThisCustomerSession.SetVal("PayPalExpressToken", Security.MungeString(PayPalToken), DateTime.Now.AddHours(3));
            ThisCustomer.ThisCustomerSession.SetVal("PayPalExpressPayerID", Security.MungeString(PayerID), DateTime.Now.AddHours(3));

            Address BillingAddress = new Address();
            BillingAddress.LoadByCustomer(ThisCustomer.CustomerID, ThisCustomer.PrimaryBillingAddressID, AddressTypes.Billing);
            if (BillingAddress.PaymentMethodLastUsed == AppLogic.ro_PMPayPalExpressMark)
            {
                String next = "checkoutreview.aspx?paymentmethod=" + Server.UrlEncode(AppLogic.ro_PMPayPalExpressMark);

                if (CommonLogic.QueryStringBool("BypassOrderReview"))
                {
                    next += "&useraction=commit";
                }

                ShoppingCart cart = new ShoppingCart(ThisCustomer.SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);
                // re-validate all shipping info
                if (!cart.ShippingIsAllValid())
                {
                    next = "checkoutshipping.aspx";
                }

                Response.Redirect(next);
            }
            else
            {
                Response.Redirect("checkoutshipping.aspx");
            }
        }

    }
}
