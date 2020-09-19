// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/orderconfirmation.aspx.cs 12    9/17/06 1:07p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;
using System.Web.Security;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for orderconfirmation.
    /// </summary>
    public partial class orderconfirmation : SkinBase
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            RequireSecurePage();

            Address BillingAddress = new Address();

            if (!ThisCustomer.IsRegistered)
            {
                bool boolAllowAnon = AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout");
                if (!boolAllowAnon && ThisCustomer.PrimaryBillingAddressID > 0)
                {
                    BillingAddress.LoadByCustomer(ThisCustomer.CustomerID, ThisCustomer.PrimaryBillingAddressID, AddressTypes.Billing);
                    if (BillingAddress.PaymentMethodLastUsed == AppLogic.ro_PMPayPalExpress || BillingAddress.PaymentMethodLastUsed == AppLogic.ro_PMPayPalExpressMark)
                    {
                        boolAllowAnon = AppLogic.AppConfigBool("PayPal.Express.AllowAnonCheckout");
                    }
                }

                if (!boolAllowAnon)
                {
                    RequiresLogin(CommonLogic.GetThisPageName(false) + "?" + CommonLogic.ServerVariables("QUERY_STRING"));
                }
            }

            // this may be overwridden by the XmlPackage below!
            SectionTitle = AppLogic.GetString("orderconfirmation.aspx.1", SkinID, ThisCustomer.LocaleSetting);

            // clear anything that should not be stored except for immediate usage:
            BillingAddress.LoadByCustomer(ThisCustomer.CustomerID, ThisCustomer.PrimaryBillingAddressID, AddressTypes.Billing);
            BillingAddress.PONumber = String.Empty;
            if (!ThisCustomer.MasterShouldWeStoreCreditCardInfo)
            {
                BillingAddress.ClearCCInfo();
            }
            BillingAddress.UpdateDB();
            AppLogic.ClearCardExtraCodeInSession(ThisCustomer);
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            int CustomerID = ThisCustomer.CustomerID;
            int OrderNumber = CommonLogic.QueryStringUSInt("OrderNumber");

            // ----------------------------------------------------------------------------------------
            // WRITE OUT ANY HEADER CHECKOUT SEQUENCE GRAPHIC:
            // ----------------------------------------------------------------------------------------
            writer.Write("<div align=\"center\">");
            writer.Write("<img src=\"" + AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_6.gif") + "\" width=\"550\" height=\"54\" border=\"0\" >\n");
            writer.Write("</div>");

            if (CustomerID != 0 && OrderNumber != 0)
            {
                Order ord = new Order(OrderNumber, ThisCustomer.LocaleSetting);

                if (ThisCustomer.CustomerID != ord.CustomerID)
                {
                    Response.Redirect(SE.MakeDriverLink("ordernotfound"));
                }
                
                if (ThisCustomer.ThisCustomerSession["3DSecure.LookupResult"].Length > 0)
                {
                    DB.ExecuteSQL("update orders set CardinalLookupResult=" + DB.SQuote(ThisCustomer.ThisCustomerSession["3DSecure.LookupResult"]) + " where OrderNumber=" + OrderNumber.ToString());
                }
                ThisCustomer.ThisCustomerSession.Clear();

                String ReceiptURL = "receipt.aspx?ordernumber=" + OrderNumber.ToString() + "&customerid=" + CustomerID.ToString();
                IDataReader rs = DB.GetRS("select * from dbo.orders where customerid=" + CustomerID.ToString() + " and ordernumber=" + OrderNumber.ToString());
                bool orderexists = rs.Read();
                rs.Close();
                if (orderexists)
                {

                    String PM = AppLogic.CleanPaymentMethod(ord.PaymentMethod);
                    String StoreName = AppLogic.AppConfig("StoreName");
                    bool UseLiveTransactions = AppLogic.AppConfigBool("UseLiveTransactions");

                    if (!ord.AlreadyConfirmed)
                    {
                        DB.ExecuteSQL("update Customer set OrderOptions=NULL, OrderNotes=NULL, FinalizationData=NULL where CustomerID=" + CustomerID.ToString());

                        if (ord.TransactionIsCaptured() && ord.HasGiftRegistryComponents())
                        {
                            ord.FinalizeGiftRegistryComponents();
                        }
                        AppLogic.SendOrderEMail(ThisCustomer, OrderNumber, false, PM, true, base.EntityHelpers, base.GetParser);
                    }

                    String XmlPackageName = AppLogic.AppConfig("XmlPackage.OrderConfirmationPage");
                    if (XmlPackageName.Length == 0)
                    {
                        XmlPackageName = "page.orderconfirmation.xml.config";
                    }

                    if (XmlPackageName.Length != 0)
                    {
                        writer.Write(AppLogic.RunXmlPackage(XmlPackageName, base.GetParser, ThisCustomer, SkinID, String.Empty, "OrderNumber=" + OrderNumber.ToString(), true, true));
                    }

                    if (!ord.AlreadyConfirmed)
                    {
                        if (AppLogic.AppConfigBool("IncludeGoogleTrackingCode"))
                        {
                            Topic GoogleTrackingCode = new Topic("GoogleTrackingCode");
                            if (GoogleTrackingCode.Contents.Length != 0)
                            {
                               // writer.Write(GoogleTrackingCode.Contents.Replace("(!ORDERTOTAL!)", Localization.CurrencyStringForGatewayWithoutExchangeRate(ord.Total(true))).Replace("(!ORDERNUMBER!)", OrderNumber.ToString()).Replace("(!CUSTOMERID!)", ThisCustomer.CustomerID.ToString()));
                            }
                        }
                        if (AppLogic.AppConfigBool("IncludeOvertureTrackingCode"))
                        {
                            Topic OvertureTrackingCode = new Topic("OvertureTrackingCode");
                            if (OvertureTrackingCode.Contents.Length != 0)
                            {
                                writer.Write(OvertureTrackingCode.Contents.Replace("(!ORDERTOTAL!)", Localization.CurrencyStringForGatewayWithoutExchangeRate(ord.Total(true))).Replace("(!ORDERNUMBER!)", OrderNumber.ToString()).Replace("(!CUSTOMERID!)", ThisCustomer.CustomerID.ToString()));
                            }
                        }

                        Topic GeneralTrackingCode = new Topic("ConfirmationTracking");
                        if (GeneralTrackingCode.Contents.Length != 0)
                        {
                            writer.Write(GeneralTrackingCode.Contents.Replace("(!ORDERTOTAL!)", Localization.CurrencyStringForGatewayWithoutExchangeRate(ord.Total(true))).Replace("(!ORDERNUMBER!)", OrderNumber.ToString()).Replace("(!CUSTOMERID!)", ThisCustomer.CustomerID.ToString()));
                        }
                    }
                    DB.ExecuteSQL("Update Orders set AlreadyConfirmed=1 where OrderNumber=" + OrderNumber.ToString());
                }
                else
                {
                    writer.Write("<div align=\"center\">");
                    writer.Write("<br/><br/><br/><br/><br/>");
                    writer.Write(AppLogic.GetString("orderconfirmation.aspx.19", SkinID, ThisCustomer.LocaleSetting));
                    writer.Write("<br/><br/><br/><br/><br/>");
                    writer.Write("</div>");
                }
            }
            else
            {
                writer.Write("<p><b>Error: Invalid Customer ID or Invalid Order Number</b></p>");
            }

            if (!ThisCustomer.IsRegistered || AppLogic.AppConfigBool("ForceSignoutOnOrderCompletion"))
            {
                if (AppLogic.AppConfigBool("SiteDisclaimerRequired"))
                {
                    AppLogic.SetSessionCookie("SiteDisclaimerAccepted", String.Empty);
                }

                //V3_9 Kill the Authentication ticket.
                Session.Clear();
                Session.Abandon();
                FormsAuthentication.SignOut();
                ThisCustomer.Logout();
            }
        }

    }
}
