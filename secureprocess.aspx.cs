// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Text;
using System.Data;
using AspDotNetStorefrontCommon;
using AspDotNetStorefrontGateways;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for secureprocess.
    /// 3-D Secure processing.
    /// </summary>
    public partial class secureprocess : System.Web.UI.Page
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            Response.Cache.SetAllowResponseInBrowserHistory(false);

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
            ThisCustomer.RequireCustomerRecord();

            int CustomerID = ThisCustomer.CustomerID;
            String paReq = ThisCustomer.ThisCustomerSession["3Dsecure.paReq"];
            String PaRes = CommonLogic.FormCanBeDangerousContent("PaRes");
            String MerchantData = CommonLogic.FormCanBeDangerousContent("MD");
            String TransactionID = ThisCustomer.ThisCustomerSession["3Dsecure.XID"];
            int OrderNumber = ThisCustomer.ThisCustomerSession.SessionUSInt("3Dsecure.OrderNumber");

            // The PaRes should have no whitespace in it, we need to strip it out.
            PaRes = PaRes.Replace(" ", "");
            PaRes = PaRes.Replace("\r", "");
            PaRes = PaRes.Replace("\n", "");

            if (PaRes.Length != 0)
            {
                ThisCustomer.ThisCustomerSession["3Dsecure.PaRes"] = PaRes;
            }

            String ErrorDesc = String.Empty;

            String ReturnURL = String.Empty;

            if (MerchantData != ThisCustomer.ThisCustomerSession["3Dsecure.MD"])
            {
                ReturnURL = "checkoutpayment.aspx?error=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("secureprocess.aspx.1", 1, Localization.GetWebConfigLocale()));
            }

            if (ShoppingCart.CartIsEmpty(CustomerID, CartTypeEnum.ShoppingCart))
            {
                ReturnURL = "ShoppingCart.aspx";
            }

            if (ReturnURL.Length == 0)
            {
                if (OrderNumber == 0)
                {
                    ReturnURL = "checkoutpayment.aspx?error=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("secureprocess.aspx.1", 1, Localization.GetWebConfigLocale()));
                }
            }

            if (ReturnURL.Length == 0)
            {
                if (paReq.Length == 0 || TransactionID.Length == 0)
                {
                    ReturnURL = "checkoutpayment.aspx?error=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("secureprocess.aspx.1", 1, Localization.GetWebConfigLocale()));
                }
            }

            if (ReturnURL.Length == 0)
            {
                ShoppingCart cart = new ShoppingCart(1, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);

                // The session may have changed in MakeOrder, so get the latest values from the DB
                CustomerSession cSession = new CustomerSession(ThisCustomer.CustomerID);

                if (status == AppLogic.ro_OK)
                {
                    if (cSession["3DSecure.LookupResult"].Length > 0)
                    {
                        // the data in this session variable will be encoded, so decode it before saving to the database
                        byte[] decodedBytes = Convert.FromBase64String(cSession["3DSecure.LookupResult"]);
                        String LookupResult = Encoding.UTF8.GetString(decodedBytes);
                        DB.ExecuteSQL("update orders set CardinalLookupResult=" + DB.SQuote(LookupResult) + " where OrderNumber=" + OrderNumber.ToString());
                        cSession["3DSecure.LookupResult"] = String.Empty;
                        // at this point we are done with the session altogether
                        CustomerSession.StaticClear(ThisCustomer.CustomerID);
                    }
                    ReturnURL = "orderconfirmation.aspx?ordernumber=" + OrderNumber.ToString() + "&paymentmethod=Credit+Card";
                }
                else
                {
                    ErrorDesc = status;
                }


            }


            if (ReturnURL.Length == 0)
            {
                ReturnURL = "checkoutpayment.aspx?error=1&errormsg=" + Server.UrlEncode(String.Format(AppLogic.GetString("secureprocess.aspx.5", 1, Localization.GetWebConfigLocale()), ErrorDesc));
            }

            ThisCustomer.ThisCustomerSession["3DSecure.CustomerID"] = String.Empty;
            ThisCustomer.ThisCustomerSession["3DSecure.OrderNumber"] = String.Empty;
            ThisCustomer.ThisCustomerSession["3DSecure.ACSUrl"] = String.Empty;
            ThisCustomer.ThisCustomerSession["3DSecure.paReq"] = String.Empty;
            ThisCustomer.ThisCustomerSession["3DSecure.XID"] = String.Empty;
            ThisCustomer.ThisCustomerSession["3DSecure.MD"] = String.Empty;
            ThisCustomer.ThisCustomerSession["3Dsecure.PaRes"] = String.Empty;


            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
            Response.Write("<html><head><title>3-D Secure Process</title></head><body>");
            Response.Write("<script type=\"text/javascript\">\n");
            Response.Write("top.location='" + ReturnURL + "';\n");
            Response.Write("</SCRIPT>\n");
            Response.Write("<div align=\"center\">" + String.Format(AppLogic.GetString("secureprocess.aspx.6", 1, Localization.GetWebConfigLocale()), ReturnURL) + "</div>");
            Response.Write("</body></html>");

        }
    }
}
