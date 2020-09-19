// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/cardinal_process.aspx.cs 8     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using AspDotNetStorefrontCommon;
using AspDotNetStorefrontGateways;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for cardinal_process.
    /// </summary>
    public partial class cardinal_process : System.Web.UI.Page
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = -1;
            Response.AddHeader("pragma", "no-cache");

            Response.Cache.SetAllowResponseInBrowserHistory(false);

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
            ThisCustomer.RequireCustomerRecord();

            int CustomerID = ThisCustomer.CustomerID;
            String Payload = ThisCustomer.ThisCustomerSession["Cardinal.Payload"];
            String PaRes = CommonLogic.FormCanBeDangerousContent("PaRes");
            String TransactionID = ThisCustomer.ThisCustomerSession["Cardinal.TransactionID"];
            int OrderNumber = ThisCustomer.ThisCustomerSession.SessionUSInt("Cardinal.OrderNumber");

            String ReturnURL = String.Empty;

            if (ShoppingCart.CartIsEmpty(CustomerID, CartTypeEnum.ShoppingCart))
            {
                ReturnURL = "ShoppingCart.aspx";
            }

            if (ReturnURL.Length == 0)
            {
                if (OrderNumber == 0)
                {
                    ReturnURL = "checkoutpayment.aspx?error=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("cardinal_process.aspcs.cs.1", 1, Localization.GetWebConfigLocale()));
                }
            }

            if (ReturnURL.Length == 0)
            {
                if (Payload.Length == 0 || TransactionID.Length == 0)
                {
                    ReturnURL = "checkoutpayment.aspx?error=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("cardinal_process.aspcs.cs.1", 1, Localization.GetWebConfigLocale()));
                }
            }

            String PAResStatus = String.Empty;
            String SignatureVerification = String.Empty;
            String ErrorNo = String.Empty;
            String ErrorDesc = String.Empty;

            if (ReturnURL.Length == 0)
            {
                String CardinalAuthenticateResult = String.Empty;
                String AuthResult = Cardinal.PreChargeAuthenticate(OrderNumber, PaRes, TransactionID, out PAResStatus, out SignatureVerification, out ErrorNo, out ErrorDesc, out CardinalAuthenticateResult);
                ThisCustomer.ThisCustomerSession["Cardinal.AuthenticateResult"] = CardinalAuthenticateResult;

                //=====================================================================================
                // Determine if the Authentication was Successful or Error
                //
                // Please consult the documentation regarding the handling of each response scenario.
                //
                // If the Authentication results (PAResStatus) is a Y or A, and the SignatureVerification is Y, then
                // the Payer Authentication was successful. The Authorization Message should be processed,
                // and the User taken to a Order Confirmation location.
                //
                // If the Authentication results were not successful (PAResStatus = N), or
                // the ErrorNo was NOT //0// then the Consumer should be redirected, and prompted for another
                // form of payment.
                //
                // If the Authentication results were not successful (PAResStatus = U) and the ErrorNo = //0//
                // then authorization message should be processed. In this case the merchant will retain
                // liability for this transaction if it is sent to authorization.
                //
                // Note that it is also important that you account for cases when your flow logic can account
                // for error cases, and the flow can be broken after //N// number of attempts
                //=====================================================================================

                // handle success cases:
                if (((PAResStatus == "Y" || PAResStatus == "A") && SignatureVerification == "Y") || (PAResStatus == "U" && ErrorNo == "0"))
                {
                    ShoppingCart cart = new ShoppingCart(1, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);

                    // GET CAVV from authenticate call result:
                    String CAVV = CommonLogic.ExtractToken(ThisCustomer.ThisCustomerSession["Cardinal.AuthenticateResult"], "<Cavv>", "</Cavv>");
                    String ECI = CommonLogic.ExtractToken(ThisCustomer.ThisCustomerSession["Cardinal.AuthenticateResult"], "<EciFlag>", "</EciFlag>");
                    String XID = CommonLogic.ExtractToken(ThisCustomer.ThisCustomerSession["Cardinal.AuthenticateResult"], "<Xid>", "</Xid>");

                    Address UseBillingAddress = new Address();
                    UseBillingAddress.LoadByCustomer(ThisCustomer.CustomerID, ThisCustomer.PrimaryBillingAddressID, AddressTypes.Billing);

                    String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, CAVV, ECI, XID, String.Empty);

                    if (status != AppLogic.ro_OK)
                    {
                        ReturnURL = "checkoutpayment.aspx?error=1&errormsg=" + status;
                    }
                    else
                    {
                        // store cardinal call results for posterity:
                        DB.ExecuteSQL("update orders set CardinalLookupResult=" + DB.SQuote(ThisCustomer.ThisCustomerSession["Cardinal.LookupResult"]) + ", CardinalAuthenticateResult=" + DB.SQuote(ThisCustomer.ThisCustomerSession["Cardinal.AuthenticateResult"]) + " where OrderNumber=" + OrderNumber.ToString());
                        ReturnURL = "orderconfirmation.aspx?ordernumber=" + OrderNumber.ToString() + "&paymentmethod=Credit+Card";
                    }
                }

                // handle failure:
                if (PAResStatus == "N" || ErrorNo != "0")
                {
                    ReturnURL = "checkoutpayment.aspx?error=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("cardinal_process.aspx.3", 1, Localization.GetWebConfigLocale()));
                }


                // handle failure:
                if (PAResStatus == "N" || ErrorNo != "0")
                {
                    ReturnURL = "checkoutpayment.aspx?error=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("cardinal_process.aspx.4", 1, Localization.GetWebConfigLocale()));
                }
            }

            if (ReturnURL.Length == 0)
            {
                ReturnURL = "checkoutpayment.aspx?error=1&errormsg=" + Server.UrlEncode(String.Format(AppLogic.GetString("cardinal_process.aspx.5", 1, Localization.GetWebConfigLocale()), ErrorDesc));
            }
            ThisCustomer.ThisCustomerSession["Cardinal.LookupResult"] = String.Empty;
            ThisCustomer.ThisCustomerSession["Cardinal.AuthenticateResult"] = String.Empty;
            ThisCustomer.ThisCustomerSession["Cardinal.ACSUrl"] = String.Empty;
            ThisCustomer.ThisCustomerSession["Cardinal.Payload"] = String.Empty;
            ThisCustomer.ThisCustomerSession["Cardinal.TransactionID"] = String.Empty;
            ThisCustomer.ThisCustomerSession["Cardinal.OrderNumber"] = String.Empty;
            ThisCustomer.ThisCustomerSession["Cardinal.LookupResult"] = String.Empty;

            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
            Response.Write("<html><head><title>Cardinal Process</title></head><body>");
            Response.Write("<script type=\"text/javascript\">\n");
            Response.Write("top.location='" + ReturnURL + "';\n");
            Response.Write("</SCRIPT>\n");
            Response.Write("<div align=\"center\">" + String.Format(AppLogic.GetString("cardinal_process.aspx.6", 1, Localization.GetWebConfigLocale()), ReturnURL) + "</div>");
            Response.Write("</body></html>");

        }
    }
}
