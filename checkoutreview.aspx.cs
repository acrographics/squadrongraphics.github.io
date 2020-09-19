// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2004.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/checkoutreview.aspx.cs 21    9/30/06 10:42p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Data;
using System.Web.UI.WebControls;
using System.Globalization;

using AspDotNetStorefrontCommon;
using AspDotNetStorefrontGateways;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for checkoutreview.
    /// </summary>
    public partial class checkoutreview : SkinBase
    {

        ShoppingCart cart = null;
        String PaymentMethod = String.Empty;
        String PM = String.Empty;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = -1;
            Response.AddHeader("pragma", "no-cache");

            if (AppLogic.AppConfigBool("RequireOver13Checked") && !ThisCustomer.IsOver13)
            {
                Response.Redirect("shoppingcart.aspx?errormsg=" + Server.UrlEncode(AppLogic.GetString("checkout.over13required", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)));
            }

            RequireSecurePage();

            // -----------------------------------------------------------------------------------------------
            // NOTE ON PAGE LOAD LOGIC:
            // We are checking here for required elements to allowing the customer to stay on this page.
            // Many of these checks may be redundant, and they DO add a bit of overhead in terms of db calls, but ANYTHING really
            // could have changed since the customer was on the last page. Remember, the web is completely stateless. Assume this
            // page was executed by ANYONE at ANYTIME (even someone trying to break the cart). 
            // It could have been yesterday, or 1 second ago, and other customers could have purchased limitied inventory products, 
            // coupons may no longer be valid, etc, etc, etc...
            // -----------------------------------------------------------------------------------------------
            ThisCustomer.RequireCustomerRecord();

            PaymentMethod = CommonLogic.QueryStringCanBeDangerousContent("PaymentMethod").Trim();
            PM = AppLogic.CleanPaymentMethod(PaymentMethod);

            if (!ThisCustomer.IsRegistered)
            {
                bool boolAllowAnon = AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout");

                if (!boolAllowAnon && (PM == AppLogic.ro_PMPayPalExpress || PM == AppLogic.ro_PMPayPalExpressMark))
                {
                    boolAllowAnon = AppLogic.AppConfigBool("PayPal.Express.AllowAnonCheckout");
                }

                if (!boolAllowAnon)
                {
                    Response.Redirect("createaccount.aspx?checkout=true");
                }
            }
            if (ThisCustomer.PrimaryBillingAddressID == 0 || (ThisCustomer.PrimaryShippingAddressID == 0 && (!AppLogic.AppConfigBool("SkipShippingOnCheckout") || !cart.IsAllDownloadComponents() || !cart.IsAllSystemComponents())))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutpayment.aspx.2", SkinID, ThisCustomer.LocaleSetting)));
            }

            SectionTitle = AppLogic.GetString("checkoutreview.aspx.1", SkinID, ThisCustomer.LocaleSetting);
            cart = new ShoppingCart(SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);


            // re-validate all shipping info, as ANYTHING could have changed since last page:
            if (!cart.ShippingIsAllValid())
            {
                HttpContext.Current.Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + HttpContext.Current.Server.UrlEncode(AppLogic.GetString("shoppingcart.cs.95", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)));
            }

            AppLogic.ValidatePM(PM); // this WILL throw a hard security exception on any problem!

            String PayPalToken = ThisCustomer.ThisCustomerSession["PayPalExpressToken"];
            String PayerID = ThisCustomer.ThisCustomerSession["PayPalExpressPayerID"];

            if ((PM == AppLogic.ro_PMPayPalExpress || PM == AppLogic.ro_PMPayPalExpressMark)
                && (PayPalToken.Length == 0 || PayerID.Length == 0))
            {
                Response.Redirect("checkoutpayment.aspx?errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutpayment.aspx.20", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)));
            }

            if (!IsPostBack)
            {
                InitializePageContent();

                if ((PM == AppLogic.ro_PMPayPalExpress || PM == AppLogic.ro_PMPayPalExpressMark)
                    && CommonLogic.QueryStringCanBeDangerousContent("useraction").ToUpperInvariant() == "COMMIT")
                {
                    ContinueCheckout();
                }
            }
            if (cart.CartItems.Count == 0)
            {
                Response.Redirect("shoppingcart.aspx");
            }
            AppLogic.eventHandler("CheckoutReview").CallEvent("&CheckoutReview=true");
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnContinueCheckout1.Click += new EventHandler(btnContinueCheckout1_Click);
            btnContinueCheckout2.Click += new EventHandler(btnContinueCheckout2_Click);
        }

        #endregion

        protected void btnContinueCheckout1_Click(object sender, EventArgs e)
        {
            btnContinueCheckout2.Enabled = false;
            ContinueCheckout();            
        }

        protected void btnContinueCheckout2_Click(object sender, EventArgs e)
        {
            btnContinueCheckout1.Enabled = false;
            ContinueCheckout();
        }

        private void ContinueCheckout()
        {
            String PayPalToken = CommonLogic.QueryStringCanBeDangerousContent("token").Trim();
            String PayerID = CommonLogic.QueryStringCanBeDangerousContent("payerid").Trim();
            ProcessCheckout();
        }

        private void InitializePageContent()
        {
            JSPopupRoutines.Text = AppLogic.GetJSPopupRoutines();
            checkoutheadergraphic.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_5.gif");
            for (int i = 0; i < checkoutheadergraphic.HotSpots.Count; i++)
            {
                RectangleHotSpot rhs = (RectangleHotSpot)checkoutheadergraphic.HotSpots[i];
                if (rhs.NavigateUrl.IndexOf("shoppingcart") != -1)
                {
                    rhs.AlternateText = AppLogic.GetString("checkoutreview.aspx.2", SkinID, ThisCustomer.LocaleSetting);
                }
                if (rhs.NavigateUrl.IndexOf("account") != -1)
                {
                    rhs.AlternateText = AppLogic.GetString("checkoutreview.aspx.3", SkinID, ThisCustomer.LocaleSetting);
                }
                if (rhs.NavigateUrl.IndexOf("checkoutshipping") != -1)
                {
                    rhs.AlternateText = AppLogic.GetString("checkoutreview.aspx.4", SkinID, ThisCustomer.LocaleSetting);
                }
                if (rhs.NavigateUrl.IndexOf("checkoutpayment") != -1)
                {
                    rhs.AlternateText = AppLogic.GetString("checkoutreview.aspx.5", SkinID, ThisCustomer.LocaleSetting);
                }
            }
            if (!AppLogic.AppConfigBool("SkipShippingOnCheckout"))
            {
                checkoutheadergraphic.HotSpots[2].HotSpotMode = HotSpotMode.Navigate;
                checkoutheadergraphic.HotSpots[2].NavigateUrl = CommonLogic.IIF(cart.HasMultipleShippingAddresses(), "checkoutshippingmult.aspx", "checkoutshipping.aspx");
            }

            String XmlPackageName = AppLogic.AppConfig("XmlPackage.CheckoutReviewPageHeader");
            if (XmlPackageName.Length != 0)
            {
                XmlPackage_CheckoutReviewPageHeader.Text = "<br/>" + AppLogic.RunXmlPackage(XmlPackageName, base.GetParser, ThisCustomer, SkinID, String.Empty, String.Empty, true, true);
            }

            Address BillingAddress = new Address();
            BillingAddress.LoadFromDB(ThisCustomer.PrimaryBillingAddressID);
            Address ShippingAddress = new Address();
            ShippingAddress.LoadFromDB(ThisCustomer.PrimaryShippingAddressID);

            litBillingAddress.Text = BillingAddress.DisplayString(true, true, "<br/>");

            litPaymentMethod.Text = GetPaymentMethod(BillingAddress);

            if (cart.HasMultipleShippingAddresses())
            {
                litShippingAddress.Text = "<br/>Multiple Ship Addresses";
            }
            else if (AppLogic.AppConfigBool("SkipShippingOnCheckout") || cart.IsAllDownloadComponents() || cart.IsAllSystemComponents())
            {
                ordercs57.Visible = false;
            }
            else
            {
                litShippingAddress.Text = ShippingAddress.DisplayString(true, true, "<br/>");
            }

            CartSummary.Text = cart.DisplaySummary(true, true, true, true, false);

            String XmlPackageName2 = AppLogic.AppConfig("XmlPackage.CheckoutReviewPageFooter");
            if (XmlPackageName2.Length != 0)
            {
                XmlPackage_CheckoutReviewPageFooter.Text = "<br/>" + AppLogic.RunXmlPackage(XmlPackageName2, base.GetParser, ThisCustomer, SkinID, String.Empty, String.Empty, true, true);
            }

            AppLogic.GetButtonDisable(btnContinueCheckout1);
            AppLogic.GetButtonDisable(btnContinueCheckout2);
            btnContinueCheckout1.Attributes["onclick"] = string.Format("{0}{1}", btnContinueCheckout1.Attributes["onclick"], "document.getElementById(\"" + btnContinueCheckout2.ClientID + "\").disabled = true;");
            btnContinueCheckout2.Attributes["onclick"] = string.Format("{0}{1}", btnContinueCheckout2.Attributes["onclick"], "document.getElementById(\"" + btnContinueCheckout1.ClientID + "\").disabled = true;");


        }

        private string GetPaymentMethod(Address BillingAddress)
        {
            if (cart.CartItems.Count == 0)
            {
                Response.Redirect("shoppingcart.aspx");
            }
            StringBuilder sPmtMethod = new StringBuilder(1024);
            if (PM == AppLogic.ro_PMCreditCard)
                if (cart.Total(true) == System.Decimal.Zero && AppLogic.AppConfigBool("SkipPaymentEntryOnZeroDollarCheckout"))
                {
                    sPmtMethod.Append(AppLogic.GetString("checkoutpayment.aspx.28", SkinID, ThisCustomer.LocaleSetting));
                }
                else
                {
                    sPmtMethod.Append(AppLogic.GetString("account.aspx.45", SkinID, ThisCustomer.LocaleSetting));
                    sPmtMethod.Append("<br/>");
                    sPmtMethod.Append("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
                    sPmtMethod.Append("<tr><td>");
                    sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.10", SkinID, ThisCustomer.LocaleSetting));
                    sPmtMethod.Append("</td><td>");
                    sPmtMethod.Append(BillingAddress.CardName);
                    sPmtMethod.Append("</td></tr>");
                    sPmtMethod.Append("<tr><td>");
                    sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.11", SkinID, ThisCustomer.LocaleSetting));
                    sPmtMethod.Append("</td><td>");
                    sPmtMethod.Append(BillingAddress.CardType);
                    sPmtMethod.Append("</td></tr>");
                    sPmtMethod.Append("<tr><td>");
                    sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.12", SkinID, ThisCustomer.LocaleSetting));
                    sPmtMethod.Append("</td><td>");
                    sPmtMethod.Append(AppLogic.SafeDisplayCardNumber(BillingAddress.CardNumber, "Address", BillingAddress.AddressID));
                    sPmtMethod.Append("</td></tr>");
                    sPmtMethod.Append("<tr><td>");
                    sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.13", SkinID, ThisCustomer.LocaleSetting));
                    sPmtMethod.Append("</td><td>");
                    sPmtMethod.Append(BillingAddress.CardExpirationMonth.PadLeft(2, '0') + "/" + BillingAddress.CardExpirationYear);
                    sPmtMethod.Append("</td></tr>");
                    sPmtMethod.Append("</table>");
                }
            else if (PM == AppLogic.ro_PMPurchaseOrder)
            {
                sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.14", SkinID, ThisCustomer.LocaleSetting) + BillingAddress.PONumber);
            }
            else if (PM == AppLogic.ro_PMCODMoneyOrder)
            {
                sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.14", SkinID, ThisCustomer.LocaleSetting) + BillingAddress.PONumber);
            }
            else if (PM == AppLogic.ro_PMCODCompanyCheck)
            {
                sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.14", SkinID, ThisCustomer.LocaleSetting) + BillingAddress.PONumber);
            }
            else if (PM == AppLogic.ro_PMCODNet30)
            {
                sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.14", SkinID, ThisCustomer.LocaleSetting) + BillingAddress.PONumber);
            }
            else if (PM == AppLogic.ro_PMPayPal)
            {
            }
            else if (PM == AppLogic.ro_PMPayPalExpress || PM == AppLogic.ro_PMPayPalExpressMark)
            {
                sPmtMethod.Append("<font color=\"red\">PayPal</font>");
            }
            else if (PM == AppLogic.ro_PMRequestQuote)
            {
                sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.20", SkinID, ThisCustomer.LocaleSetting));
            }
            else if (PM == AppLogic.ro_PMCheckByMail)
            {
                sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.21", SkinID, ThisCustomer.LocaleSetting));
            }
            else if (PM == AppLogic.ro_PMCOD)
            {
                sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.22", SkinID, ThisCustomer.LocaleSetting));
            }
            else if (PM == AppLogic.ro_PMECheck)
            {
                sPmtMethod.Append("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
                sPmtMethod.Append("<tr><td>");
                sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.15", SkinID, ThisCustomer.LocaleSetting));
                sPmtMethod.Append("</td><td>");
                sPmtMethod.Append(BillingAddress.ECheckBankAccountName);
                sPmtMethod.Append("</td></tr>");
                sPmtMethod.Append("<tr><td>");
                sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.16", SkinID, ThisCustomer.LocaleSetting));
                sPmtMethod.Append("</td><td>");
                sPmtMethod.Append(BillingAddress.ECheckBankAccountType);
                sPmtMethod.Append("</td></tr>");
                sPmtMethod.Append("<tr><td>");
                sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.17", SkinID, ThisCustomer.LocaleSetting));
                sPmtMethod.Append("</td><td>");
                sPmtMethod.Append(BillingAddress.ECheckBankName);
                sPmtMethod.Append("</td></tr>");
                sPmtMethod.Append("<tr><td>");
                sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.18", SkinID, ThisCustomer.LocaleSetting));
                sPmtMethod.Append("</td><td>");
                sPmtMethod.Append(BillingAddress.ECheckBankAccountNumber);
                sPmtMethod.Append("</td></tr>");
                sPmtMethod.Append("<tr><td>");
                sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.19", SkinID, ThisCustomer.LocaleSetting));
                sPmtMethod.Append("</td><td>");
                sPmtMethod.Append(BillingAddress.ECheckBankABACode);
                sPmtMethod.Append("</td></tr>");
                sPmtMethod.Append("</table>");
            }
            else if (PM == AppLogic.ro_PMMicropay)
            {
                sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.23", SkinID, ThisCustomer.LocaleSetting));

            }
            return sPmtMethod.ToString();
        }

        private void ProcessCheckout()
        {
            Address BillingAddress = new Address();
            BillingAddress.LoadByCustomer(ThisCustomer.CustomerID, ThisCustomer.PrimaryBillingAddressID, AddressTypes.Billing);

            int OrderNumber = 0;
            // ----------------------------------------------------------------
            // Process The Order:
            // ----------------------------------------------------------------

            if (PaymentMethod.Length == 0)
            {
                Response.Redirect("checkoutpayment.aspx?errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutpayment.aspx.20", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)));
            }
            if (PM == AppLogic.ro_PMCreditCard)
            {
                bool CardinalAllowed = AppLogic.AppConfigBool("CardinalCommerce.Centinel.Enabled") && !(cart.Total(true) == System.Decimal.Zero && AppLogic.AppConfigBool("SkipPaymentEntryOnZeroDollarCheckout"));
                if (CardinalAllowed && (BillingAddress.CardType.Trim().ToUpperInvariant() == "VISA" || BillingAddress.CardType.Trim().ToUpperInvariant() == "MASTERCARD"))
                {
                    // use cardinal pre-auth fraud screening:
                    String ACSUrl = String.Empty;
                    String Payload = String.Empty;
                    String TransactionID = String.Empty;
                    String CardinalLookupResult = String.Empty;
                    OrderNumber = AppLogic.GetNextOrderNumber();
                    if (Cardinal.PreChargeLookup(BillingAddress.CardNumber, Localization.ParseUSInt(BillingAddress.CardExpirationYear), Localization.ParseUSInt(BillingAddress.CardExpirationMonth), OrderNumber, cart.Total(true), "", out ACSUrl, out Payload, out TransactionID, out CardinalLookupResult))
                    {
                        // redirect to intermediary page which gets card password from user:
                        ThisCustomer.ThisCustomerSession["Cardinal.LookupResult"] = CardinalLookupResult;
                        ThisCustomer.ThisCustomerSession["Cardinal.ACSUrl"] = ACSUrl;
                        ThisCustomer.ThisCustomerSession["Cardinal.Payload"] = Payload;
                        ThisCustomer.ThisCustomerSession["Cardinal.TransactionID"] = TransactionID;
                        ThisCustomer.ThisCustomerSession["Cardinal.OrderNumber"] = OrderNumber.ToString();

                        Response.Redirect("cardinalform.aspx"); // this will eventually come "back" to us in cardinal_process.aspx after going through banking system pages
                    }
                    else
                    {
                        ThisCustomer.ThisCustomerSession["Cardinal.LookupResult"] = CardinalLookupResult;
                        // user not enrolled or cardinal gateway returned error, so process card normally, using already created order #:
                        String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                        if (status != AppLogic.ro_OK)
                        {
                            Response.Redirect("checkoutpayment.aspx?TryToShowPM=" + PM + "&errormsg=" + status);
                        }
                        DB.ExecuteSQL("update orders set CardinalLookupResult=" + DB.SQuote(ThisCustomer.ThisCustomerSession["Cardinal.LookupResult"]) + " where OrderNumber=" + OrderNumber.ToString());
                    }
                }
                else
                {
                    // try create the order record, check for status of TX though:
                    OrderNumber = AppLogic.GetNextOrderNumber();
                    String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                    if (status == AppLogic.ro_3DSecure)
                    { // If credit card is enrolled in a 3D Secure service (Verified by Visa, etc.)
                        Response.Redirect("secureform.aspx");
                    }
                    if (status != AppLogic.ro_OK)
                    {
                        Response.Redirect("checkoutpayment.aspx?TryToShowPM=" + PM + "&errormsg=" + Server.UrlEncode(status));
                    }
                }
            }
            else if (PM == AppLogic.ro_PMPurchaseOrder)
            {
                // try create the order record, check for status of TX though:
                OrderNumber = AppLogic.GetNextOrderNumber();
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                if (status != AppLogic.ro_OK)
                {
                    Response.Redirect("checkoutpayment.aspx?TryToShowPM=" + PM + "&errormsg=" + Server.UrlEncode(status));
                }
            }
            else if (PM == AppLogic.ro_PMCODMoneyOrder)
            {
                // try create the order record, check for status of TX though:
                OrderNumber = AppLogic.GetNextOrderNumber();
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                if (status != AppLogic.ro_OK)
                {
                    Response.Redirect("checkoutpayment.aspx?TryToShowPM=" + PM + "&errormsg=" + Server.UrlEncode(status));
                }
            }
            else if (PM == AppLogic.ro_PMCODCompanyCheck)
            {
                // try create the order record, check for status of TX though:
                OrderNumber = AppLogic.GetNextOrderNumber();
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                if (status != AppLogic.ro_OK)
                {
                    Response.Redirect("checkoutpayment.aspx?TryToShowPM=" + PM + "&errormsg=" + Server.UrlEncode(status));
                }
            }
            else if (PM == AppLogic.ro_PMCODNet30)
            {
                // try create the order record, check for status of TX though:
                OrderNumber = AppLogic.GetNextOrderNumber();
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                if (status != AppLogic.ro_OK)
                {
                    Response.Redirect("checkoutpayment.aspx?TryToShowPM=" + PM + "&errormsg=" + Server.UrlEncode(status));
                }
            }
            else if (PM == AppLogic.ro_PMPayPal)
            {
            }
            else if (PM == AppLogic.ro_PMPayPalExpress || PM == AppLogic.ro_PMPayPalExpressMark)
            {
                String PayPalToken = Security.UnmungeString(ThisCustomer.ThisCustomerSession["PayPalExpressToken"]);
                String PayerID = Security.UnmungeString(ThisCustomer.ThisCustomerSession["PayPalExpressPayerID"]);
                if (PayPalToken.Length > 0)
                {
                    OrderNumber = AppLogic.GetNextOrderNumber();

                    Address UseBillingAddress = new Address();
                    UseBillingAddress.LoadByCustomer(ThisCustomer.CustomerID, ThisCustomer.PrimaryBillingAddressID, AddressTypes.Billing);
                    UseBillingAddress.PaymentMethodLastUsed = PM;
                    UseBillingAddress.CardNumber = String.Empty;
                    UseBillingAddress.CardType = String.Empty;
                    UseBillingAddress.CardExpirationMonth = String.Empty;
                    UseBillingAddress.CardExpirationYear = String.Empty;
                    UseBillingAddress.CardName = String.Empty;
                    UseBillingAddress.CardStartDate = String.Empty;
                    UseBillingAddress.CardIssueNumber = String.Empty;
                    UseBillingAddress.UpdateDB();

                    String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, PayPalToken, PayerID, String.Empty, String.Empty);
                    if (status != AppLogic.ro_OK)
                    {
                        Response.Redirect("checkoutpayment.aspx?errormsg=" + Server.UrlEncode(status));
                    }
                    else
                    {
                        ThisCustomer.ThisCustomerSession["PayPalExpressToken"] = "";
                        ThisCustomer.ThisCustomerSession["PayPalExpressPayerID"] = "";
                    }
                }
                else
                {
                    Response.Redirect("shoppingcart.aspx?errormsg=The PaypalExpress checkout token has expired, please re-login to your PayPal account or checkout using a different method of payment.");
                }
            }
            else if (PM == AppLogic.ro_PMRequestQuote)
            {
                // try create the order record, check for status of TX though:
                OrderNumber = AppLogic.GetNextOrderNumber();
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                if (status != AppLogic.ro_OK)
                {
                    Response.Redirect("checkoutpayment.aspx?TryToShowPM=" + PM + "&errormsg=" + Server.UrlEncode(status));
                }
            }
            else if (PM == AppLogic.ro_PMCheckByMail)
            {
                // try create the order record, check for status of TX though:
                OrderNumber = AppLogic.GetNextOrderNumber();
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                if (status != AppLogic.ro_OK)
                {
                    Response.Redirect("checkoutpayment.aspx?TryToShowPM=" + PM + "&errormsg=" + Server.UrlEncode(status));
                }
            }
            else if (PM == AppLogic.ro_PMCOD)
            {
                // try create the order record, check for status of TX though:
                OrderNumber = AppLogic.GetNextOrderNumber();
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                if (status != AppLogic.ro_OK)
                {
                    Response.Redirect("checkoutpayment.aspx?TryToShowPM=" + PM + "&errormsg=" + Server.UrlEncode(status));
                }
            }
            else if (PM == AppLogic.ro_PMECheck)
            {
                // try create the order record, check for status of TX though:
                OrderNumber = AppLogic.GetNextOrderNumber();
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                if (status != AppLogic.ro_OK)
                {
                    Response.Redirect("checkoutpayment.aspx?TryToShowPM=" + PM + "&errormsg=" + Server.UrlEncode(status));
                }
            }
            else if (PM == AppLogic.ro_PMMicropay)
            {
                // try create the order record, check for status of TX though:
                OrderNumber = AppLogic.GetNextOrderNumber();
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                if (status != AppLogic.ro_OK)
                {
                    Response.Redirect("checkoutpayment.aspx?TryToShowPM=" + PM + "&errormsg=" + Server.UrlEncode(status));
                }
            }

            Response.Redirect("orderconfirmation.aspx?ordernumber=" + OrderNumber.ToString() + "&paymentmethod=" + Server.UrlEncode(PaymentMethod));
        }

    }
}
