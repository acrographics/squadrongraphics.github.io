// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/account.aspx.cs 45    10/03/06 7:56p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;
using AspDotNetStorefrontCommon;
using AspDotNetStorefrontGateways;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for account.
    /// </summary>
    public partial class account : SkinBase
    {

        bool EMailDup = false;
        bool AccountUpdated = false;
        bool EMailAlreadyTaken = false;
        bool Checkout = false;
        public CultureInfo SqlServerCulture = new System.Globalization.CultureInfo(CommonLogic.Application("DBSQLServerLocaleSetting")); // qualification needed for vb.net (not sure why)
        public string m_StoreLoc = AppLogic.GetStoreHTTPLocation(true);
        string SkinImagePath = string.Empty;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            RequireSecurePage();
            RequiresLogin(CommonLogic.GetThisPageName(false) + "?" + CommonLogic.ServerVariables("QUERY_STRING"));
            SectionTitle = AppLogic.GetString("account.aspx.56", SkinID, ThisCustomer.LocaleSetting);
            Checkout = CommonLogic.QueryStringBool("checkout");
            if (Checkout)
            {
                ThisCustomer.RequireCustomerRecord();
            }

            bool newAccount = CommonLogic.QueryStringBool("newaccount");
            if (newAccount)
            {
                ErrorMsgLabel.Text = "<b><center>You have successfully created a new acccount</center></b>";
            }

            ThisCustomer.ValidatePrimaryAddresses();

            bool AllowShipToDifferentThanBillTo = AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo") && !AppLogic.AppConfigBool("SkipShippingOnCheckout");
            if (!AllowShipToDifferentThanBillTo)
            {
                pnlShipping.Visible = pnlShipping2.Visible = false;
            }

            //If there is a DeleteID remove it from the cart
            int DeleteID = CommonLogic.QueryStringUSInt("DeleteID");
            if (DeleteID != 0 && Customer.OwnsThisOrder(ThisCustomer.CustomerID,DeleteID))
            {
                RecurringOrderMgr rmgr = new RecurringOrderMgr(base.EntityHelpers, base.GetParser);
                rmgr.CancelRecurringOrder(DeleteID);
            }

            //If there is a FullRefundID refund it
            int FullRefundID = CommonLogic.QueryStringUSInt("FullRefundID");
            if (FullRefundID != 0 && Customer.OwnsThisOrder(ThisCustomer.CustomerID, FullRefundID))
            {
                RecurringOrderMgr rmgr = new RecurringOrderMgr(base.EntityHelpers, base.GetParser);
                rmgr.ProcessAutoBillFullRefund(FullRefundID);
            }

            //If there is a PartialRefundID refund it
            int PartialRefundID = CommonLogic.QueryStringUSInt("PartialRefundID");
            if (PartialRefundID != 0 && Customer.OwnsThisOrder(ThisCustomer.CustomerID, PartialRefundID))
            {
                RecurringOrderMgr rmgr = new RecurringOrderMgr(base.EntityHelpers, base.GetParser);
                rmgr.ProcessAutoBillPartialRefund(PartialRefundID);
            }

            SkinImagePath = "skins/skin_" + SkinID.ToString() + "/images/";

            if (!AppLogic.AppConfigBool("RequireOver13Checked"))
            {
                pnlOver13.Visible = false;
            }

            StoreCCRow.Visible = ThisCustomer.MasterShouldWeStoreCreditCardInfo;

            if (!AppLogic.AppConfigBool("VAT.Enabled"))
            {
                VATRegistrationIDRow.Visible = false;
            }
            else
            {
                VATRegistrationIDRow.Visible = true;
            }
            if (!this.IsPostBack)
            {
                FirstName.Text = ThisCustomer.FirstName;
                LastName.Text = ThisCustomer.LastName;
                EMail.Text = ThisCustomer.EMail.ToLowerInvariant().Trim();
                CustPassword.Text = String.Empty;
                CustPassword2.Text = String.Empty;
                Phone.Text = ThisCustomer.Phone;
                ckbSaveCC.Checked = ThisCustomer.MasterShouldWeStoreCreditCardInfo;
                Over13.Checked = ThisCustomer.IsOver13;
                VATRegistrationID.Text = ThisCustomer.VATRegistrationID;
                if (ThisCustomer.OKToEMail)
                {
                    OKToEMailYes.Checked = true;
                }
                else
                {
                    OKToEMailNo.Checked = true;
                }

                RefreshPage();
            }
        }

        public void btnContinueToCheckOut_Click(object sender, EventArgs e)
        {
            Response.Redirect("checkoutshipping.aspx");
        }

        public void btnUpdateAccount_Click(object sender, EventArgs e)
        {
            Page.Validate("account");
            if (Page.IsValid)
            {
                String EMailField = EMail.Text.ToLowerInvariant().Trim();
                if (!AppLogic.AppConfigBool("AllowCustomerDuplicateEMailAddresses"))
                {
                    EMailAlreadyTaken = Customer.EmailInUse(EMailField, ThisCustomer.CustomerID);
                }

                Regex re = new Regex(@"^[a-zA-Z0-9][-\+\w\.]*@([a-zA-Z0-9][\w\-]*\.)+[a-zA-Z]{2,4}$");
                bool emailisvalid = re.IsMatch(EMailField);
                if (!emailisvalid)
                {
                    lblAcctUpdateMsg.Text = AppLogic.GetString("createaccount.aspx.17", SkinID, ThisCustomer.LocaleSetting);
                }

                if (EMailAlreadyTaken || !emailisvalid)
                {
                    EMailField = ThisCustomer.EMail ; // reset their e-mail, but then update their account with other changes below
                }


                string pwd = null;
                object saltkey = null;
                if (CustPassword.Text.Trim().Length > 0)
                {
                    Password p = new Password(CustPassword.Text);
                    pwd = p.SaltedPassword;
                    saltkey = p.Salt;
                }
                bool HasActiveRecurring = ThisCustomer.HasActiveRecurringOrders(true);
                YouHaveActiveRecurringOrdersWarning.Visible = false;
                if (!ckbSaveCC.Checked && (HasActiveRecurring && !AppLogic.AppConfigBool("Recurring.UseGatewayInternalBilling")))
                {
                    ckbSaveCC.Checked = true;
                    YouHaveActiveRecurringOrdersWarning.Visible = true;
                }

                String vtr = VATRegistrationID.Text;
                if (!AppLogic.AppConfigBool("VAT.Enabled"))
                {
                    vtr = null;
                    VATRegistrationIDIsInvalid.Visible = false;
                    VATRegistrationID.Text = String.Empty;
                }
                else
                {
                    if (vtr.Trim().Length == 0 || AppLogic.VATRegistrationIDIsValid(ThisCustomer, vtr))
                    {
                        VATRegistrationIDIsInvalid.Visible = false;
                    }
                    else
                    {
                        vtr = null;
                        VATRegistrationIDIsInvalid.Visible = true;
                        VATRegistrationID.Text = String.Empty;
                    }
                }
                ThisCustomer.UpdateCustomer(
                    /*CustomerLevelID*/ null,
                    /*EMail*/ EMailField,
                    /*SaltedAndHashedPassword*/ pwd,
                    /*SaltKey*/ saltkey,
                    /*DateOfBirth*/ null,
                    /*Gender*/ null,
                    /*FirstName*/ FirstName.Text,
                    /*LastName*/ LastName.Text,
                    /*Notes*/ null,
                    /*SkinID*/ null,
                    /*Phone*/ Phone.Text,
                    /*AffiliateID*/ null,
                    /*Referrer*/ null,
                    /*CouponCode*/ null,
                    /*OkToEmail*/ CommonLogic.IIF(OKToEMailYes.Checked, 1, 0),
                    /*IsAdmin*/ null,
                    /*BillingEqualsShipping*/ null,
                    /*LastIPAddress*/ null,
                    /*OrderNotes*/ null,
                    /*SubscriptionExpiresOn*/ null,
                    /*RTShipRequest*/ null,
                    /*RTShipResponse*/ null,
                    /*OrderOptions*/ null,
                    /*LocaleSetting*/ null,
                    /*MicroPayBalance*/ null,
                    /*RecurringShippingMethodID*/ null,
                    /*RecurringShippingMethod*/ null,
                    /*BillingAddressID*/ null,
                    /*ShippingAddressID*/ null,
                    /*GiftRegistryGUID*/ null,
                    /*GiftRegistryIsAnonymous*/ null,
                    /*GiftRegistryAllowSearchByOthers*/ null,
                    /*GiftRegistryNickName*/ null,
                    /*GiftRegistryHideShippingAddresses*/ null,
                    /*CODCompanyCheckAllowed*/ null,
                    /*CODNet30Allowed*/ null,
                    /*ExtensionData*/ null,
                    /*FinalizationData*/ null,
                    /*Deleted*/ null,
                    /*Over13Checked*/ CommonLogic.IIF(Over13.Checked, 1, 0),
                    /*CurrencySetting*/ null,
                    /*VATSetting*/ null,
                    /*VATRegistrationID*/ vtr,
                    /*StoreCCInDB*/ CommonLogic.IIF(this.ckbSaveCC.Checked, 1, 0),
                    /*IsRegistered*/ null,
                    /*LockedUntil*/ null,
                    /*AdminCanViewCC*/ null,
                    /*BadLogin*/ null,
                    /*Active*/ null,
                    /*PwdChangeRequired*/ null,
                    /*RegisterDate*/ null
                    );

                AccountUpdated = true;
                EMailDup = EMailAlreadyTaken;
            }
            RefreshPage();
        }


        public string GetPaymentStatus(string PaymentMethod, string CardNumber, string TransactionState)
        {
            String PaymentStatus = String.Empty;
            if (PaymentMethod.Length != 0)
            {
                PaymentStatus = AppLogic.GetString("account.aspx.43", SkinID, ThisCustomer.LocaleSetting) + " " + PaymentMethod.Replace(AppLogic.ro_PMMicropay, AppLogic.GetString("account.aspx.11", SkinID, ThisCustomer.LocaleSetting)) + "<br/>";
            }
            else
            {
                PaymentStatus = AppLogic.GetString("account.aspx.43", SkinID, ThisCustomer.LocaleSetting) + " " + CommonLogic.IIF(CardNumber.StartsWith(AppLogic.ro_PMPayPal,StringComparison.InvariantCultureIgnoreCase), AppLogic.GetString("account.aspx.44", SkinID, ThisCustomer.LocaleSetting), AppLogic.GetString("account.aspx.45", SkinID, ThisCustomer.LocaleSetting)) + "<br/>";
            }
            PaymentStatus += TransactionState;
            return PaymentStatus;

        }

        public string GetShippingStatus(int OrderNumber, string ShippedOn, string ShippedVIA, string ShippingTrackingNumber, string TransactionState, string DownloadEMailSentOn)
        {
            String ShippingStatus = String.Empty;
            if (AppLogic.OrderHasShippableComponents(OrderNumber))
            {
                if (ShippedOn != "")
                {
                    ShippingStatus = AppLogic.GetString("account.aspx.48", SkinID, ThisCustomer.LocaleSetting);
                    if (ShippedVIA.Length != 0)
                    {
                        ShippingStatus += " " + AppLogic.GetString("account.aspx.49", SkinID, ThisCustomer.LocaleSetting) + " " + ShippedVIA;
                    }
                    //IFormatProvider dateFormat = new CultureInfo(ThisCustomer.LocaleSetting, true);
                    ShippingStatus += " " + AppLogic.GetString("account.aspx.50", SkinID, ThisCustomer.LocaleSetting) + " " + Localization.ParseNativeDateTime(ShippedOn).ToString(new CultureInfo(ThisCustomer.LocaleSetting));
                    if (ShippingTrackingNumber.Length != 0)
                    {
                        ShippingStatus += " " + AppLogic.GetString("account.aspx.51", SkinID, ThisCustomer.LocaleSetting) + " ";

                        String TrackURL = Shipping.GetTrackingURL(ShippingTrackingNumber);
                        if (TrackURL.Length != 0)
                        {
                            ShippingStatus += "<a href=\"" + TrackURL + "\" target=\"_blank\">" + ShippingTrackingNumber + "</a>";
                        }
                        else
                        {
                            ShippingStatus += ShippingTrackingNumber;
                        }
                    }
                }
                else
                {
                    ShippingStatus = AppLogic.GetString("account.aspx.52", SkinID, ThisCustomer.LocaleSetting);
                }
            }
            if (AppLogic.OrderHasDownloadComponents(OrderNumber))
            {
                if (ShippingStatus.Length != 0)
                {
                    ShippingStatus += "<br/>";
                }
                DateTime dwm = Localization.ParseDBDateTime(DownloadEMailSentOn);
                if (dwm != System.DateTime.MinValue)
                {
                    ShippingStatus += String.Format(AppLogic.GetString("account.aspx.53a", SkinID, ThisCustomer.LocaleSetting), Localization.ToNativeShortDateString(dwm));
                }
                else
                {
                    ShippingStatus += AppLogic.GetString("account.aspx.53", SkinID, ThisCustomer.LocaleSetting);
                }
            }

            return ShippingStatus;
        }

        public string GetOrderTotal(int QuoteCheckout, string PaymentMethod, object decOrderTotal, int CouponType, object decCouponAmount)
        {
            decimal OrderTotal = Convert.ToDecimal(decOrderTotal);
            decimal CouponAmount = Convert.ToDecimal(decCouponAmount);
            if (CouponType == 2)
                OrderTotal = CommonLogic.IIF(OrderTotal < CouponAmount, 0, OrderTotal - CouponAmount);

            return CommonLogic.IIF(QuoteCheckout == 1 || AppLogic.CleanPaymentMethod(PaymentMethod) == AppLogic.ro_PMRequestQuote, AppLogic.GetString("account.aspx.54", SkinID, ThisCustomer.LocaleSetting), ThisCustomer.CurrencyString(OrderTotal));
        }

        public string GetCustSvcNotes(string CustomerServiceNotes)
        {
            if (AppLogic.AppConfigBool("ShowCustomerServiceNotesInReceipts"))
            {
                return CommonLogic.IIF(CustomerServiceNotes.Length == 0, AppLogic.GetString("order.cs.29", SkinID, ThisCustomer.LocaleSetting), CustomerServiceNotes);
            }
            else
            {
                return "";
            }

        }

        public string GetReorder(string OrderNumber, string RecurringSubscriptionID)
        {
#if PRO
            return String.Empty;
#else
            if (RecurringSubscriptionID.Length == 0)
            {
                return "<br/><br/><a href=\"javascript:ReOrder(" + OrderNumber + ");\">" + AppLogic.GetString("account.aspx.57", SkinID, ThisCustomer.LocaleSetting) + "</a>";
            }
            else
            {
                return String.Empty;
            }
#endif
        }


        public void RefreshPage()
        {
            Address BillingAddress = new Address();
            Address ShippingAddress = new Address();

            BillingAddress.LoadByCustomer(ThisCustomer.CustomerID, ThisCustomer.PrimaryBillingAddressID, AddressTypes.Billing);
            ShippingAddress.LoadByCustomer(ThisCustomer.CustomerID, ThisCustomer.PrimaryShippingAddressID, AddressTypes.Shipping);

            if (Checkout)
            {
                pnlCheckoutImage.Visible = true;
                CheckoutImage.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "step_2.gif");
                if (ThisCustomer.PrimaryBillingAddressID == 0 || ThisCustomer.PrimaryShippingAddressID == 0 || !ThisCustomer.HasAtLeastOneAddress())
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("account.aspx.73", ThisCustomer.SkinID, ThisCustomer.LocaleSetting); ;
                }

            }

            String XRI = AppLogic.LocateImageURL(SkinImagePath + "redarrow.gif");
            redarrow1.ImageUrl = XRI;
            redarrow2.ImageUrl = XRI;
            redarrow3.ImageUrl = XRI;
            pnlCheckoutImage.Visible = Checkout;
            unknownerrormsg.Text = Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("unknownerror"));
            ErrorMsgLabel.Text += Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("errormsg"));
            pnlAccountUpdated.Visible = AccountUpdated;
            if (AccountUpdated)
            {
                if (EMailAlreadyTaken)
                {
                    lblAcctUpdateMsg.Text += CommonLogic.IIF(lblAcctUpdateMsg.Text.Trim() == "", "", "<br/>") + AppLogic.GetString("account.aspx.3", SkinID, ThisCustomer.LocaleSetting);
                }
                else
                {
                    lblAcctUpdateMsg.Text = CommonLogic.IIF(lblAcctUpdateMsg.Text.Trim() == "", "", "<br/>") + AppLogic.GetString("account.aspx.2", SkinID, ThisCustomer.LocaleSetting);
                }
            }

            pnlNotCheckOutButtons.Visible = !Checkout;
            pnlShowWishButton.Visible = AppLogic.AppConfigBool("ShowWishButtons");
            pnlShowGiftRegistryButtons.Visible = AppLogic.AppConfigBool("ShowGiftRegistryButtons");
            pnlSubscriptionExpiresOn.Visible = (ThisCustomer.SubscriptionExpiresOn > System.DateTime.Now);
            lblSubscriptionExpiresOn.Text = String.Format(AppLogic.GetString("account.aspx.5", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), Localization.ToNativeShortDateString(ThisCustomer.SubscriptionExpiresOn));
            OriginalEMail.Text = ThisCustomer.EMail;
            imgAccountinfo.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "accountinfo.gif");
            note1.Visible = (ThisCustomer.CustomerLevelID != 0);
            note1.Text = String.Format(AppLogic.GetString("account.aspx.9", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), ThisCustomer.CustomerLevelName);
            MicroPayEnabled.Visible = (AppLogic.MicropayIsEnabled() && ThisCustomer.IsRegistered && AppLogic.GetMicroPayProductID() != 0);
            MicroPayEnabled.Text = String.Format(AppLogic.GetString("account.aspx.10", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), AppLogic.GetString("account.aspx.11", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), ThisCustomer.CurrencyString(ThisCustomer.MicroPayBalance));
            btnContinueToCheckOut.Visible = Checkout;

            lnkChangeBilling.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "change.gif");
            lnkChangeShipping.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "change.gif");
            if (ThisCustomer.PrimaryBillingAddressID == 0)
            {
                pnlBilling.Visible = false;
            }
            if (ThisCustomer.PrimaryShippingAddressID == 0)
            {
                pnlShipping.Visible = false;
            }
            lnkChangeBilling.NavigateUrl = "javascript:self.location='selectaddress.aspx?Checkout=" + Checkout.ToString() + "&AddressType=billing&returnURL=" + Server.UrlEncode("account.aspx?checkout=" + Checkout.ToString()) + "'";
            lnkChangeShipping.NavigateUrl = "javascript:self.location='selectaddress.aspx?Checkout=" + Checkout.ToString() + "&AddressType=shipping&returnURL=" + Server.UrlEncode("account.aspx?checkout=" + Checkout.ToString()) + "'";
            imgAddressbook.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "addressbook.gif");

            lnkAddBillingAddress.NavigateUrl = "selectaddress.aspx?add=true&addressType=Billing&Checkout=" + Checkout.ToString() + "&returnURL=" + Server.UrlEncode("account.aspx?checkout=" + Checkout.ToString());
            lnkAddBillingAddress.Text = AppLogic.GetString("account.aspx.63", SkinID, ThisCustomer.LocaleSetting);
            lnkAddShippingAddress.NavigateUrl = "selectaddress.aspx?add=true&addressType=Shipping&Checkout=" + Checkout.ToString() + "&returnURL=" + Server.UrlEncode("account.aspx?checkout=" + Checkout.ToString());
            lnkAddShippingAddress.Text = AppLogic.GetString("account.aspx.62", SkinID, ThisCustomer.LocaleSetting);

            litBillingAddress.Text = BillingAddress.DisplayHTML(Checkout);
            if (BillingAddress.PaymentMethodLastUsed.Length != 0)
            {
                litBillingAddress.Text += "<b>" + AppLogic.GetString("account.aspx.31", SkinID, ThisCustomer.LocaleSetting) + "</b><br/>";
                litBillingAddress.Text += BillingAddress.DisplayPaymentMethodInfo(ThisCustomer, BillingAddress.PaymentMethodLastUsed);
            }

            litShippingAddress.Text = ShippingAddress.DisplayHTML(Checkout);
            pnlOrderHistory.Visible = !Checkout;
            imgOrderhistory.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "orderhistory.gif");


#if PRO
            // gift cards not supported in PRO
#else
            GiftCards gc = new GiftCards(ThisCustomer.CustomerID, GiftCardCollectionFilterType.UsingCustomerID);
            if (gc.Count > 0)
            {
                rptrGiftCards.DataSource = gc;
                rptrGiftCards.DataBind();
                tblGiftCards.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
                tblGiftCardsBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
                giftcards_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/giftcards.gif");
                pnlGiftCards.Visible = true;
            }
#endif
            if (ShoppingCart.NumItems(ThisCustomer.CustomerID, CartTypeEnum.RecurringCart) != 0)
            {
                RecurringOrders.Text = "<p align=\"left\"><b>" + AppLogic.GetString("account.aspx.35", SkinID, ThisCustomer.LocaleSetting) + "</b></p>";

                // build JS code to show/hide address update block:
                StringBuilder tmpS = new StringBuilder(4096);
                tmpS.Append("<script type=\"text/javascript\">\n");
                tmpS.Append("function toggleLayer(DivID)\n");
                tmpS.Append("{\n");
                tmpS.Append("	var elem;\n");
                tmpS.Append("	var vis;\n");
                tmpS.Append("	if(document.getElementById)\n");
                tmpS.Append("	{\n");
                tmpS.Append("		// standards\n");
                tmpS.Append("		elem = document.getElementById(DivID);\n");
                tmpS.Append("	}\n");
                tmpS.Append("	else if(document.all)\n");
                tmpS.Append("	{\n");
                tmpS.Append("		// old msie versions\n");
                tmpS.Append("		elem = document.all[DivID];\n");
                tmpS.Append("	}\n");
                tmpS.Append("	else if(document.layers)\n");
                tmpS.Append("	{\n");
                tmpS.Append("		// nn4\n");
                tmpS.Append("		elem = document.layers[DivID];\n");
                tmpS.Append("	}\n");
                tmpS.Append("	vis = elem.style;\n");
                tmpS.Append("	if(vis.display == '' && elem.offsetWidth != undefined && elem.offsetHeight != undefined)\n");
                tmpS.Append("	{\n");
                tmpS.Append("		vis.display = (elem.offsetWidth != 0 && elem.offsetHeight != 0) ? 'block' : 'none';\n");
                tmpS.Append("	}\n");
                tmpS.Append("	vis.display = (vis.display == '' || vis.display == 'block') ? 'none' : 'block' ;\n");
                tmpS.Append("}\n");
                tmpS.Append("</script>\n");
                tmpS.Append("\n");
                tmpS.Append("<style type=\"text/css\">\n");
                tmpS.Append("	.addressBlockDiv { margin: 0px 20px 0px 20px;  display: none;}\n");
                tmpS.Append("</style>\n");
                RecurringOrders.Text += tmpS.ToString();

                IDataReader rsr = DB.GetRS("Select distinct OriginalRecurringOrderNumber from ShoppingCart  " + DB.GetNoLock() + " where CartType=" + ((int)CartTypeEnum.RecurringCart).ToString() + " and CustomerID=" + ThisCustomer.CustomerID.ToString() + " order by OriginalRecurringOrderNumber desc");
                while (rsr.Read())
                {
                    RecurringOrders.Text += AppLogic.GetRecurringCart(base.EntityHelpers, base.GetParser, ThisCustomer, DB.RSFieldInt(rsr, "OriginalRecurringOrderNumber"), SkinID, false);
                }
                rsr.Close();
                RecurringOrders.Text += "<br/><br/>";
            }

            IDataReader rs = DB.GetRS("Select * from orders  " + DB.GetNoLock() + " where TransactionState in (" + DB.SQuote(AppLogic.ro_TXStateAuthorized) + "," + DB.SQuote(AppLogic.ro_TXStateCaptured) + ") and CustomerID=" + ThisCustomer.CustomerID.ToString() + " order by OrderDate desc");
            orderhistorylist.DataSource = rs;
            orderhistorylist.DataBind();
            rs.Close();

            accountaspx55.Visible = (orderhistorylist.Items.Count == 0);

            CustPassword.Text = String.Empty;
            CustPassword2.Text = String.Empty;

            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), "function ReOrder(OrderNumber) {if(confirm('" + AppLogic.GetString("account.aspx.64", SkinID, ThisCustomer.LocaleSetting) + "')) {top.location.href='reorder.aspx?ordernumber='+OrderNumber;} }", true);
        }

        protected void ValidatePassword(object source, ServerValidateEventArgs args)
        {
            if (CustPassword.Text.Trim() == "")
            {
                args.IsValid = true;
                return;
            }
            if (CustPassword.Text == CustPassword2.Text)
            {
                try
                {
                    valPwd.ErrorMessage = AppLogic.GetString("account.aspx.7", SkinID, ThisCustomer.LocaleSetting);
                    if (AppLogic.AppConfigBool("UseStrongPwd") || ThisCustomer.IsAdminUser)
                    {

                        Regex re = new Regex(AppLogic.AppConfig("CustomerPwdValidator"));
                        if (re.IsMatch(CustPassword.Text))
                        {
                            args.IsValid = true;
                        }
                        else
                        {
                            args.IsValid = false;
                            valPwd.ErrorMessage = AppLogic.GetString("account.aspx.69", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                        }
                    }
                    else
                    {
                        args.IsValid = (CustPassword.Text.Length > 4);
                    }
                }
                catch
                {
                    AppLogic.SendMail("Invalid Password Validation Pattern", "", false, AppLogic.AppConfig("MailMe_ToAddress"), AppLogic.AppConfig("MailMe_ToAddress"), AppLogic.AppConfig("MailMe_ToAddress"), AppLogic.AppConfig("MailMe_ToAddress"), "", "", AppLogic.MailServer());
                    throw new Exception("Password validation expression is invalid, please notify site administrator");
                }
            }
            else
            {
                args.IsValid = false;
                valPwd.ErrorMessage = AppLogic.GetString("account.aspx.68", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            }
        }

    }

}
