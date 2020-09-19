// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2004.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/checkout1.aspx.cs 9     9/13/06 8:14p Buddy $
// ------------------------------------------------------------------------------------------
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AspDotNetStorefrontCommon;
using AspDotNetStorefrontGateways;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for checkout1.
    /// </summary>
    public partial class checkout1 : SkinBase
    {
        private bool SkipRegistration = false;
        private ShoppingCart cart;
        private bool RequireSecurityCode = false;
        private bool AllowShipToDifferentThanBillTo = false;
        private String ReturnURL = String.Empty;
        private Address BillingAddress = new Address(); // qualification needed for vb.net (not sure why)
        private Address ShippingAddress = new Address(); // qualification needed for vb.net (not sure why)
        private Shipping.ShippingCalculationEnum ShipCalcType;
        private bool ShippingRequiresAddressInfo = true;
        private bool RequireTerms = false;
        private string AllowedPaymentMethods = String.Empty;
        private string GW = String.Empty;
        private string SelectedPaymentType = String.Empty;
        private bool useLiveTransactions = false;
        //CheckoutHelper m_CHK;
        private decimal CartTotal = Decimal.Zero;
        private decimal NetTotal = Decimal.Zero;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = -1;
            Response.AddHeader("pragma", "no-cache");

            RequireSecurePage();
            ThisCustomer.RequireCustomerRecord();


            SkipRegistration = AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout") || AppLogic.AppConfigBool("HidePasswordFieldDuringCheckout");
            RequireSecurityCode = AppLogic.AppConfigBool("SecurityCodeRequiredOnCheckout1DuringCheckout");
            SectionTitle = (AppLogic.GetString("createaccount.aspx.2", SkinID, ThisCustomer.LocaleSetting) + " " + SectionTitle).Trim();

            // -----------------------------------------------------------------------------------------------
            // NOTE ON PAGE LOAD LOGIC:
            // We are checking here for required elements to allowing the customer to stay on this page.
            // Many of these checks may be redundant, and they DO add a bit of overhead in terms of db calls, but ANYTHING really
            // could have changed since the customer was on the last page. Remember, the web is completely stateless. Assume this
            // page was executed by ANYONE at ANYTIME (even someone trying to break the cart). 
            // It could have been yesterday, or 1 second ago, and other customers could have purchased limitied inventory products, 
            // coupons may no longer be valid, etc, etc, etc...
            // -----------------------------------------------------------------------------------------------

            cart = new ShoppingCart(SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);

//            m_CHK = new CheckoutHelper(ThisCustomer, cart, this.Page);

            if (cart.IsEmpty())
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (cart.InventoryTrimmed)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("shoppingcart.aspx.3", SkinID, ThisCustomer.LocaleSetting)));
            }

            if (cart.RecurringScheduleConflict)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("shoppingcart.aspx.19", SkinID, ThisCustomer.LocaleSetting)));
            }

            if (cart.HasCoupon() &&
                !cart.CouponIsValid)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&discountvalid=false");
            }

            if (!cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (!cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            // these conditions are NOT allowed to use 1 page checkout!
            if (cart.HasMultipleShippingAddresses() || cart.HasGiftRegistryComponents() ||
                cart.ContainsGiftCard())
            {
                if (ThisCustomer.IsRegistered &&
                    (ThisCustomer.PrimaryBillingAddressID == 0 || ThisCustomer.PrimaryShippingAddressID == 0 || !ThisCustomer.HasAtLeastOneAddress()))
                {
                    Response.Redirect("createaccount.aspx?checkout=true");
                }

                if (!ThisCustomer.IsRegistered || ThisCustomer.PrimaryBillingAddressID == 0 || ThisCustomer.PrimaryShippingAddressID == 0 ||
                    !ThisCustomer.HasAtLeastOneAddress())
                {
                    Response.Redirect("checkoutanon.aspx?checkout=true");
                }
                else
                {
                    if (AppLogic.AppConfigBool("SkipShippingOnCheckout") || cart.IsAllSystemComponents() ||
                        cart.IsAllDownloadComponents())
                    {
                        if (cart.ContainsGiftCard())
                        {
                            Response.Redirect("checkoutgiftcard.aspx");
                        }
                        else
                        {
                            Response.Redirect("checkoutpayment.aspx");
                        }
                    }

                    //Response.Redirect("selectaddress.aspx?gotostep3=true&checkout=true&addresstype=billing");
                    if (!cart.IsAllDownloadComponents() && !cart.IsAllFreeShippingComponents() && !cart.IsAllSystemComponents() && (cart.HasMultipleShippingAddresses() || cart.HasGiftRegistryComponents()) && cart.TotalQuantity() <= AppLogic.MultiShipMaxNumItemsAllowed() && cart.CartAllowsShippingMethodSelection &&
                        cart.TotalQuantity() > 1)
                    {
                        Response.Redirect("checkoutshippingmult.aspx");
                    }
                    else
                    {
                        Response.Redirect("checkoutshipping.aspx");
                    }
                }
            }

            AllowShipToDifferentThanBillTo = AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo");
            if (cart.IsAllDownloadComponents() ||
                cart.IsAllSystemComponents())
            {
                AllowShipToDifferentThanBillTo = false;
            }

            ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");
            if (ReturnURL.ToLowerInvariant().IndexOf("<script>") !=
                -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }
            ErrorMsgLabel.Text = "";

            if (!AppLogic.AppConfigBool("RequireOver13Checked"))
            {
                pnlOver13.Visible = false;
            }

            ShipCalcType = Shipping.GetActiveShippingCalculationID();
            if (ShipCalcType == Shipping.ShippingCalculationEnum.AllOrdersHaveFreeShipping || ShipCalcType == Shipping.ShippingCalculationEnum.CalculateShippingByTotal || ShipCalcType == Shipping.ShippingCalculationEnum.CalculateShippingByWeight || ShipCalcType == Shipping.ShippingCalculationEnum.UseFixedPercentageOfTotal || ShipCalcType == Shipping.ShippingCalculationEnum.UseFixedPrice ||
                ShipCalcType == Shipping.ShippingCalculationEnum.UseIndividualItemShippingCosts)
            {
                // these types of shipping calcs do NOT require address info, so show them right now on the page:
                // tbd                SetupShippingPanel();
                ShippingRequiresAddressInfo = false;
            }

            if (!ShippingRequiresAddressInfo)
            {
                // tbd                SetupPaymentPanel();
            }

            GW = AppLogic.ActivePaymentGatewayCleaned();
            useLiveTransactions = AppLogic.AppConfigBool("UseLiveTransactions");
            RequireTerms = AppLogic.AppConfigBool("RequireTermsAndConditionsAtCheckout");
            CartTotal = cart.Total(true);
            NetTotal = CartTotal - CommonLogic.IIF(cart.Coupon.m_CouponType == CouponTypeEnum.GiftCard, CommonLogic.IIF(CartTotal < cart.Coupon.m_DiscountAmount, CartTotal, cart.Coupon.m_DiscountAmount), 0);

            BillingAddress.LoadByCustomer(ThisCustomer.CustomerID, ThisCustomer.PrimaryBillingAddressID, AddressTypes.Billing);
            ShippingAddress.LoadByCustomer(ThisCustomer.CustomerID, ThisCustomer.PrimaryShippingAddressID, AddressTypes.Shipping);
            if (!IsPostBack)
            {
                ViewState["SelectedPaymentType"] = string.Empty;
                InitializeValidationErrorMessages();
                InitializePageContent();                
            }
            GetJavaScriptFunctions();
        }

        #region EventHandlers

        public void BillingCountry_OnChange(object sender, EventArgs e)
        {
            BillingState.SelectedIndex = -1;
            string sql = String.Empty;
            if (BillingCountry.SelectedIndex > 0)
            {
                sql = "select s.* from State s " + DB.GetNoLock() + " join country c " + DB.GetNoLock() + " on s.countryid = c.countryid where c.name = " + DB.SQuote(BillingCountry.SelectedValue) + " order by s.DisplayOrder,s.Name";
            }
            else
            {
                sql = "select * from State " + DB.GetNoLock() + " where countryid=(select countryid from country " + DB.GetNoLock() + " where name='United States') order by DisplayOrder,Name";
            }

            IDataReader dr = DB.GetRS(sql);
            BillingState.DataSource = dr;
            BillingState.DataTextField = "Name";
            BillingState.DataValueField = "Abbreviation";
            BillingState.DataBind();
            dr.Close();

            if (BillingState.Items.Count == 0)
            {
                BillingState.Items.Insert(0, new ListItem("Other (Non U.S.)", "--"));
                BillingState.SelectedIndex = 0;
            }
            else
            {
                BillingState.Items.Insert(0, new ListItem(AppLogic.GetString("address.cs.11", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), ""));
                BillingState.SelectedIndex = 0;
            }

            SetBillingStateList(BillingCountry.SelectedValue);
            // RJY Set the ShippingState to Match the BillingState
            // DragNet: 762
            ShippingCountry.SelectedValue = BillingCountry.SelectedValue;
            SetShippingStateList(ShippingCountry.SelectedValue);
            SetPasswordFields();
            GetJavaScriptFunctions();
        }

        public void ShippingCountry_Change(object sender, EventArgs e)
        {
            ShippingState.SelectedIndex = -1;
            SetShippingStateList(ShippingCountry.SelectedValue);
            SetPasswordFields();
            GetJavaScriptFunctions();
        }

        protected void BillingState_DataBound(object sender, EventArgs e)
        {
            BillingState.Items.Insert(0, new ListItem(AppLogic.GetString("createaccount.aspx.48", SkinID, ThisCustomer.LocaleSetting), ""));
            try
            {
                int i = BillingState.Items.IndexOf(BillingState.Items.FindByValue(BillingAddress.State));
                if (i == -1)
                {
                    BillingState.SelectedIndex = 0;
                }
                else
                {
                    BillingState.SelectedIndex = i;
                }
            }
            catch
            {
                BillingState.SelectedIndex = 0;
            }
        }

        private void BillingCountry_DataBound(object sender, EventArgs e)
        {
            BillingCountry.Items.Insert(0, new ListItem(AppLogic.GetString("createaccount.aspx.48", SkinID, ThisCustomer.LocaleSetting), ""));
            try
            {
                int i = BillingCountry.Items.IndexOf(BillingCountry.Items.FindByValue(BillingAddress.Country));
                if (i == -1)
                {
                    BillingCountry.SelectedIndex = 0;
                }
                else
                {
                    BillingCountry.SelectedIndex = i;
                }
            }
            catch
            {
                BillingCountry.SelectedIndex = 0;
            }
        }

        private void ShippingState_DataBound(object sender, EventArgs e)
        {
            ShippingState.Items.Insert(0, new ListItem(AppLogic.GetString("createaccount.aspx.48", SkinID, ThisCustomer.LocaleSetting), ""));
            try
            {
                int i = ShippingState.Items.IndexOf(ShippingState.Items.FindByValue(ShippingAddress.State));
                if (i == -1)
                {
                    ShippingState.SelectedIndex = 0;
                }
                else
                {
                    ShippingState.SelectedIndex = i;
                }
            }
            catch
            {
                ShippingState.SelectedIndex = 0;
            }
        }

        private void ShippingCountry_DataBound(object sender, EventArgs e)
        {
            ShippingCountry.Items.Insert(0, new ListItem(AppLogic.GetString("createaccount.aspx.48", SkinID, ThisCustomer.LocaleSetting), ""));
            try
            {
                int i = ShippingCountry.Items.IndexOf(ShippingCountry.Items.FindByValue(ShippingAddress.Country));
                if (i == -1)
                {
                    ShippingCountry.SelectedIndex = 0;
                }
                else
                {
                    ShippingCountry.SelectedIndex = i;
                }
            }
            catch
            {
                ShippingCountry.SelectedIndex = 0;
            }
        }

        public void btnContinueCheckout_Click(object sender, EventArgs e)
        {
            ProcessCheckout();
        }

        public void valCustSecurityCode_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = (SecurityCode.Text.Trim() == Session["SecurityCode"].ToString());
        }

        public void ValidatePassword(object source, ServerValidateEventArgs args)
        {
            string pwd1 = ViewState["custpwd"].ToString();
            string pwd2 = ViewState["custpwd2"].ToString();

            if (ThisCustomer.IsRegistered)
            {
                args.IsValid = true;
            }
            else if (pwd1.Length == 0)
            {
                args.IsValid = false;
                valPassword.ErrorMessage = AppLogic.GetString("createaccount.aspx.20", SkinID, ThisCustomer.LocaleSetting);
            }
            else if (pwd1.Trim().Length == 0)
            {
                args.IsValid = false;
                valPassword.ErrorMessage = AppLogic.GetString("account.aspx.74", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            }
            else if (pwd1 == pwd2)
            {
                try
                {
                    valPassword.ErrorMessage = AppLogic.GetString("account.aspx.7", SkinID, ThisCustomer.LocaleSetting);
                    if (AppLogic.AppConfigBool("UseStrongPwd") ||
                        ThisCustomer.IsAdminUser)
                    {
                        Regex re = new Regex(AppLogic.AppConfig("CustomerPwdValidator"));
                        if (re.IsMatch(pwd1))
                        {
                            args.IsValid = true;
                        }
                        else
                        {
                            args.IsValid = false;
                            valPassword.ErrorMessage = AppLogic.GetString("account.aspx.69", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                        }
                    }
                    else
                    {
                        args.IsValid = (pwd1.Length > 4);
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
                valPassword.ErrorMessage = AppLogic.GetString("createaccount.aspx.80", SkinID, ThisCustomer.LocaleSetting);
            }

            if (!args.IsValid)
            {
                ViewState["custpwd"] = "";
                ViewState["custpwd2"] = "";
            }
        }

        public void btnRecalcShipping_OnClick(object source, EventArgs e)
        {
            String EMailField = CommonLogic.IIF(SkipRegistration, txtSkipRegEmail.Text.ToLower().Trim(), EMail.Text.ToLower().Trim());

            bool acctvalid = false;
            if (!SkipRegistration)
            {
                SetPasswordFields();
                acctvalid = AccountIsValid();
                bool EMailAlreadyTaken = Customer.EmailInUse(EMailField, ThisCustomer.CustomerID) && !AppLogic.AppConfigBool("AllowCustomerDuplicateEMailAddresses");

                String PWD = ViewState["custpwd"].ToString();
                Password p = new Password(PWD);
                String newpwd = p.SaltedPassword;
                Nullable<int> newsaltkey = p.Salt;
                if (ThisCustomer.Password.Length != 0)
                {
                    // do NOT allow passwords to be changed on this page. this is only for creating an account.
                    // if they want to change their password, they must use their account page
                    newpwd = null;
                    newsaltkey = null;
                }
                if (!EMailAlreadyTaken && acctvalid)
                {
                    ThisCustomer.UpdateCustomer(
                        /*CustomerLevelID*/ null,
                                            /*EMail*/ EMailField,
                                            /*SaltedAndHashedPassword*/ newpwd,
                                            /*SaltKey*/ newsaltkey,
                                            /*DateOfBirth*/ null,
                                            /*Gender*/ null,
                                            /*FirstName*/ FirstName.Text.Trim(),
                                            /*LastName*/ LastName.Text.Trim(),
                                            /*Notes*/ null,
                                            /*SkinID*/ null,
                                            /*Phone*/ Phone.Text.Trim(),
                                            /*AffiliateID*/ null,
                                            /*Referrer*/ null,
                                            /*CouponCode*/ null,
                                            /*OkToEmail*/ CommonLogic.IIF(OKToEMailYes.Checked, 1, 0),
                                            /*IsAdmin*/ null,
                                            /*BillingEqualsShipping*/ CommonLogic.IIF(ShippingEqualsBilling.Checked, 1, 0),
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
                                            /*Over13Checked*/ CommonLogic.IIF(Over13.Checked || SkipRegOver13.Checked, 1, 0),
                                            /*CurrencySetting*/ null,
                                            /*VATSetting*/ null,
                                            /*VATRegistrationID*/ null,
                                            /*StoreCCInDB*/ null,
                                            /*IsRegistered*/ CommonLogic.IIF(SkipRegistration, 0, 1),
                                            /*LockedUntil*/ null,
                                            /*AdminCanViewCC*/ null,
                                            /*BadLogin*/ null,
                                            /*Active*/ null,
                                            /*PwdChangeRequired*/ null,
                                            /*RegisterDate*/ null
                        );
                }

                if (!acctvalid)
                {
                    ErrorMsgLabel.Text += "<br /><br /> Some errors occured trying to create your account.  Please correct them and try again.<br /><br />";

                    foreach (IValidator aValidator in Validators)
                    {
                        if (!aValidator.IsValid)
                        {
                            ErrorMsgLabel.Text += "&bull; " + aValidator.ErrorMessage + "<br />";
                        }
                    }
                    ErrorMsgLabel.Text += "<br />";
                }
            }

            bool shippingvalid = ShippingIsValid();
            if (AllowShipToDifferentThanBillTo)
            {
                if (shippingvalid)
                {
                    ShippingAddress = new Address();
                    ShippingAddress.LastName = ShippingLastName.Text;
                    ShippingAddress.FirstName = ShippingFirstName.Text;
                    ShippingAddress.Phone = ShippingPhone.Text;
                    ShippingAddress.Company = ShippingCompany.Text;
                    ShippingAddress.ResidenceType = (ResidenceTypes) Convert.ToInt32(ShippingResidenceType.SelectedValue);
                    ShippingAddress.Address1 = ShippingAddress1.Text;
                    ShippingAddress.Address2 = ShippingAddress2.Text;
                    ShippingAddress.Suite = ShippingSuite.Text;
                    ShippingAddress.City = ShippingCity.Text;
                    ShippingAddress.State = ShippingState.SelectedValue;
                    ShippingAddress.Zip = ShippingZip.Text;
                    ShippingAddress.Country = ShippingCountry.SelectedValue;
                    ShippingAddress.EMail = EMailField;

                    ShippingAddress.InsertDB(ThisCustomer.CustomerID);
                    ShippingAddress.MakeCustomersPrimaryAddress(AddressTypes.Shipping);
                    ThisCustomer.PrimaryShippingAddressID = ShippingAddress.AddressID;
                }
                else
                {
                    ShippingEqualsBilling.Checked = false;

                    if (ErrorMsgLabel.Text.Length == 0)
                    {
                        ErrorMsgLabel.Text += "<br /><br /> " + String.Format(AppLogic.GetString("checkout1.aspx.7", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), AppLogic.GetString("order.cs.55", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)) + "<br /><br />";
                    }


                    foreach (IValidator aValidator in Validators)
                    {
                        if (!aValidator.IsValid &&
                            ErrorMsgLabel.Text.IndexOf(aValidator.ErrorMessage) == -1)
                        {
                            ErrorMsgLabel.Text += "&bull; " + aValidator.ErrorMessage + "<br />";
                        }
                    }
                    ErrorMsgLabel.Text += "<br />";
                }
            }


            bool billingvalid = BillingIsValid();
            if (billingvalid)
            {
                BillingAddress = new Address();

                BillingAddress.LastName = BillingLastName.Text;
                BillingAddress.FirstName = BillingFirstName.Text;
                BillingAddress.Phone = BillingPhone.Text;
                BillingAddress.Company = BillingCompany.Text;
                BillingAddress.ResidenceType = (ResidenceTypes) Convert.ToInt32(BillingResidenceType.SelectedValue);
                BillingAddress.Address1 = BillingAddress1.Text;
                BillingAddress.Address2 = BillingAddress2.Text;
                BillingAddress.Suite = BillingSuite.Text;
                BillingAddress.City = BillingCity.Text;
                BillingAddress.State = BillingState.SelectedValue;
                BillingAddress.Zip = BillingZip.Text;
                BillingAddress.Country = BillingCountry.SelectedValue;
                BillingAddress.EMail = EMailField;

                BillingAddress.InsertDB(ThisCustomer.CustomerID);
                BillingAddress.MakeCustomersPrimaryAddress(AddressTypes.Billing);
                ThisCustomer.PrimaryBillingAddressID = BillingAddress.AddressID;
                if (!AllowShipToDifferentThanBillTo)
                {
                    ThisCustomer.PrimaryShippingAddressID = BillingAddress.AddressID;
                    BillingAddress.MakeCustomersPrimaryAddress(AddressTypes.Shipping);
                }
            }
            else
            {
                if (ErrorMsgLabel.Text.Length == 0)
                {
                    ErrorMsgLabel.Text += "<br /><br /> " + String.Format(AppLogic.GetString("checkout1.aspx.7", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), AppLogic.GetString("order.cs.55", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)) + "<br /><br />";
                }

                foreach (IValidator aValidator in Validators)
                {
                    if (!aValidator.IsValid &&
                        ErrorMsgLabel.Text.IndexOf(aValidator.ErrorMessage) == -1)
                    {
                        ErrorMsgLabel.Text += "&bull; " + aValidator.ErrorMessage + "<br />";
                    }
                }
                ErrorMsgLabel.Text += "<br />";
            }

            cart = new ShoppingCart(SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);
            InitializePageContent();
        }

        #endregion

        #region Private Functions

        private void InitializePageContent()
        {
            tblShippingSelectBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
            tblPaymentOptions.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
            shippingselect_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/shippingselect.gif");
            paymentselect_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/paymentselect.gif");

            InitializeAccountInfo();

            InitializeShippingOptions(cart);

            InitializePaymentOptions(cart);

            CartSummary.Text = cart.DisplaySummary(true, true, true, true);
        }

        private void InitializeValidationErrorMessages()
        {
            //valReqFName.ErrorMessage = AppLogic.GetString("createaccount.aspx.5", SkinID, ThisCustomer.LocaleSetting);
            //valReqLName.ErrorMessage = AppLogic.GetString("createaccount.aspx.5", SkinID, ThisCustomer.LocaleSetting);
            valReqEmail.ErrorMessage = AppLogic.GetString("createaccount.aspx.16", SkinID, ThisCustomer.LocaleSetting);
            valRegExValEmail.ErrorMessage = AppLogic.GetString("createaccount.aspx.17", SkinID, ThisCustomer.LocaleSetting);
            valPassword.ErrorMessage = AppLogic.GetString("createaccount.aspx.20", SkinID, ThisCustomer.LocaleSetting);
            valReqPhone.ErrorMessage = AppLogic.GetString("createaccount.aspx.24", SkinID, ThisCustomer.LocaleSetting);
            valReqSecurityCode.ErrorMessage = AppLogic.GetString("signin.aspx.20", SkinID, ThisCustomer.LocaleSetting);
            valReqBillFName.ErrorMessage = AppLogic.GetString("createaccount.aspx.34", SkinID, ThisCustomer.LocaleSetting);
            valReqBillLName.ErrorMessage = AppLogic.GetString("createaccount.aspx.36", SkinID, ThisCustomer.LocaleSetting);
            valReqBillPhone.ErrorMessage = AppLogic.GetString("createaccount.aspx.38", SkinID, ThisCustomer.LocaleSetting);
            valReqBillAddr1.ErrorMessage = AppLogic.GetString("createaccount.aspx.42", SkinID, ThisCustomer.LocaleSetting);
            valReqBillCity.ErrorMessage = AppLogic.GetString("createaccount.aspx.46", SkinID, ThisCustomer.LocaleSetting);
            valReqBillZip.ErrorMessage = AppLogic.GetString("createaccount.aspx.50", SkinID, ThisCustomer.LocaleSetting);
            //valReqBillState.ErrorMessage = AppLogic.GetString("createaccount.aspx.8", SkinID, ThisCustomer.LocaleSetting);
            valReqBillCountry.ErrorMessage = AppLogic.GetString("createaccount.aspx.9", SkinID, ThisCustomer.LocaleSetting);
            valReqShipFName.ErrorMessage = AppLogic.GetString("createaccount.aspx.56", SkinID, ThisCustomer.LocaleSetting);
            valReqShipLName.ErrorMessage = AppLogic.GetString("createaccount.aspx.58", SkinID, ThisCustomer.LocaleSetting);
            valReqShipPhone.ErrorMessage = AppLogic.GetString("createaccount.aspx.60", SkinID, ThisCustomer.LocaleSetting);
            valReqShipAddr1.ErrorMessage = AppLogic.GetString("createaccount.aspx.64", SkinID, ThisCustomer.LocaleSetting);
            valReqShipCity.ErrorMessage = AppLogic.GetString("createaccount.aspx.68", SkinID, ThisCustomer.LocaleSetting);
            valReqShipZip.ErrorMessage = AppLogic.GetString("createaccount.aspx.71", SkinID, ThisCustomer.LocaleSetting);
            valReqShipState.ErrorMessage = AppLogic.GetString("createaccount.aspx.10", SkinID, ThisCustomer.LocaleSetting);
            valReqShipCountry.ErrorMessage = AppLogic.GetString("createaccount.aspx.11", SkinID, ThisCustomer.LocaleSetting);
            valRegExSkipRegEmail.ErrorMessage = AppLogic.GetString("createaccount.aspx.17", SkinID, ThisCustomer.LocaleSetting);
        }

        private void GetJavaScriptFunctions()
        {
            //BillingEqualsAccount.Attributes.Add("onclick", "copyaccount(this.form);");

            StringBuilder s = new StringBuilder("<script type=\"text/javascript\" Language=\"JavaScript\">\n");

            //s.Append("function copyaccount(theForm){ ");
            //s.Append("if (theForm." + BillingEqualsAccount.ClientID + ".checked){");
            //s.Append("theForm." + BillingFirstName.ClientID + ".value = theForm." + FirstName.ClientID + ".value;");
            //s.Append("theForm." + BillingLastName.ClientID + ".value = theForm." + LastName.ClientID + ".value;");
            //s.Append("theForm." + BillingPhone.ClientID + ".value = theForm." + Phone.ClientID + ".value;");
            //s.Append("} ");
            //s.Append("return true; } \n\n");

            //s.Append("var netPostBack = __doPostBack;\n");
            //s.Append("__doPostBack = EscapeHtml;\n\n");

            s.Append("function EscapeHtml(eventTarget, eventArgument){\n");
            s.Append("\t var retVal = validateCheckout();\n");
            s.Append("\t alert(eventTarget);\n");
            s.Append("\t if(!retVal) return false;\n");
            s.Append("\t return netPostBack (eventTarget, eventArgument);\n");
            s.Append("}\n\n");

            s.Append("function validateCheckout(){\n");
            s.Append("\t var btn = document.getElementById('" + btnContinueCheckout.ClientID + "');\n");
            s.Append("\t var retVal = validateForm(document.getElementById('" + Page.Form.ClientID + "'));\n");
            s.Append("\t if(!retVal) {btn.disabled = false; return false;}\n");
            s.Append("\t retVal = ValidateAccountInfo();\n");
            s.Append("\t if(!retVal) {btn.disabled = false; return false;}\n");
            s.Append("\t retVal = ValidateShippingInfo();\n");
            s.Append("\t if(!retVal) {btn.disabled = false; return false;}\n");
            s.Append("\t retVal = ValidatePaymentInfo();\n");
            s.Append("\t if(!retVal) {btn.disabled = false; return false;}\n");
            s.Append("\t retVal = ValidateTerms();\n");
            s.Append("\t if(!retVal) {btn.disabled = false; return false;}\n");
            s.Append("\t btn.disabled = false;\n");
            s.Append("\t return true;\n");
            s.Append("}\n\n");


            s.Append("function ValidateTerms(){\n");
            if (RequireTerms)
            {
                //s.Append("	alert(document.getElementById('TermsAndConditionsRead').checked)\n");
                s.Append("	if(!document.getElementById('TermsAndConditionsRead').checked)\n");
                s.Append("	{\n");
                s.Append("		alert(\"" + AppLogic.GetString("checkoutpayment.aspx.15", SkinID, ThisCustomer.LocaleSetting) + "\");\n");
                s.Append("		return (false);\n");
                s.Append("    }\n");
            }
            s.Append("return true;\n");
            s.Append("}\n\n");


            s.Append("function ValidateAccountInfo(){\n");
            s.Append("return true;");
            s.Append("}\n\n");

            s.Append("function ValidateShippingInfo(){\n");
            if (AllowShipToDifferentThanBillTo && !AppLogic.AppConfigBool("SkipShippingOnCheckout") && !cart.IsAllDownloadComponents() &&
               !cart.IsAllSystemComponents())
            {
                s.Append("\tvar retVal = CheckoutShippingForm_Validator();");
                s.Append("return retVal;\n");
            }
            else
            {
                s.Append("return true;\n");
            }
            s.Append("}\n\n");

            s.Append("function ValidatePaymentInfo(){\n");
            //s.Append("return true;");
            if ((pmtCreditCard.Checked && 
                GW != Gateway.ro_GW2CHECKOUT && 
                GW != Gateway.ro_GWWORLDPAYJUNIOR &&
                GW != Gateway.ro_GWWORLDPAY) &&
                NetTotal != decimal.Zero)
            {
                s.Append("\tvar retVal = CreditCardForm_Validator(theForm);");
                s.Append( "return retVal;\n" );
            }
            else
            {
                s.Append("return true;\n");
            }
            //s.Append("\tValidatePayment();\n");
            s.Append("}\n\n");


            ShippingEqualsBilling.Attributes.Add("onclick", "copybilling(this.form);");
            s.Append("function copybilling(theForm){ ");
            s.Append("if (theForm." + ShippingEqualsBilling.ClientID + ".checked){ ");
            s.Append("	theForm." + ShippingFirstName.ClientID + ".value = theForm." + BillingFirstName.ClientID + ".value;");
            s.Append("	theForm." + ShippingLastName.ClientID + ".value = theForm." + BillingLastName.ClientID + ".value;");
            s.Append("	theForm." + ShippingPhone.ClientID + ".value = theForm." + BillingPhone.ClientID + ".value;");
            s.Append("	theForm." + ShippingCompany.ClientID + ".value = theForm." + BillingCompany.ClientID + ".value;");
            s.Append("	theForm." + ShippingResidenceType.ClientID + ".selectedIndex = theForm." + BillingResidenceType.ClientID + ".selectedIndex;");
            s.Append("	theForm." + ShippingAddress1.ClientID + ".value = theForm." + BillingAddress1.ClientID + ".value;");
            s.Append("	theForm." + ShippingAddress2.ClientID + ".value = theForm." + BillingAddress2.ClientID + ".value;");
            s.Append("	theForm." + ShippingSuite.ClientID + ".value = theForm." + BillingSuite.ClientID + ".value;");
            s.Append("	theForm." + ShippingCity.ClientID + ".value = theForm." + BillingCity.ClientID + ".value;");
            s.Append("	theForm." + ShippingState.ClientID + ".selectedIndex = theForm." + BillingState.ClientID + ".selectedIndex;");
            s.Append("	theForm." + ShippingZip.ClientID + ".value = theForm." + BillingZip.ClientID + ".value;");
            s.Append("	theForm." + ShippingCountry.ClientID + ".selectedIndex = theForm." + BillingCountry.ClientID + ".selectedIndex;");
            s.Append(" }");
            s.Append("return (true); }\n\n");

            s.Append("function TermsChecked(){ ");
            if (RequireTerms)
            {
                s.Append("	if(!document.getElementById('TermsAndConditionsRead').checked){");
                s.Append("		alert(\"" + AppLogic.GetString("checkoutpayment.aspx.15", SkinID, ThisCustomer.LocaleSetting) + "\");\n");
                s.Append("		return (false); }");
            }
            s.Append("	return (true); }");
            s.Append("</script>\n");


            Page.ClientScript.RegisterClientScriptBlock(GetType(), Guid.NewGuid().ToString(), s.ToString(), false);

            btnContinueCheckout.Attributes.Add("onclick", "return validateCheckout();");
        }

        private void ProcessCheckout()
        {
            SetPasswordFields();

            if (SkipRegistration)
            {
                Page.Validate("skipreg");
            }
            else
            {
                Page.Validate("registration");
            }
            Page.Validate("BillingCheckout1");
            if ( AllowShipToDifferentThanBillTo )
                Page.Validate("ShippingCheckout1");

            bool retVal = ProcessAccount();

            if (!retVal)
            {
                InitializePageContent();
                return;
            }

            retVal = ProcessShipping(cart);

            if (!retVal)
            {
                InitializePageContent();
                return;
            }

            ProcessPayment();

            InitializePageContent();
            GetJavaScriptFunctions();
        }

        private void SetPasswordFields()
        {
            if (ViewState["custpwd"] == null)
            {
                ViewState["custpwd"] = "";
            }
            if (password.Text.Trim() != "")
            {
                ViewState["custpwd"] = password.Text;
                reqValPassword.Enabled = false;
            }

            if (ViewState["custpwd2"] == null)
            {
                ViewState["custpwd2"] = "";
            }
            if (password2.Text != "")
            {
                ViewState["custpwd2"] = password2.Text;
            }
        }

        private void SetShippingStateList(string shippingCountry)
        {
            ShippingState.SelectedIndex = -1;
            string sql = String.Empty;
            if (shippingCountry.Length > 0)
            {
                sql = "select s.* from State s " + DB.GetNoLock() + " join country c " + DB.GetNoLock() + " on s.countryid = c.countryid where c.name = " + DB.SQuote(shippingCountry) + " order by s.DisplayOrder,s.Name";
            }
            else
            {
                sql = "select * from State " + DB.GetNoLock() + " where countryid=(select countryid from country " + DB.GetNoLock() + " where name='United States') order by DisplayOrder,Name";
            }

            IDataReader dr = DB.GetRS(sql);
            ShippingState.DataSource = dr;
            ShippingState.DataTextField = "Name";
            ShippingState.DataValueField = "Abbreviation";
            ShippingState.DataBind();
            dr.Close();
            if (ShippingState.Items.Count == 0)
            {
                ShippingState.Items.Insert(0, new ListItem("Other (Non U.S.)", "--"));
                ShippingState.SelectedIndex = 0;
            }
            else
            {
                ShippingState.Items.Insert(0, new ListItem(AppLogic.GetString("address.cs.11", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), ""));
                ShippingState.SelectedIndex = 0;
            }

            if (ShippingState.Items.IndexOf(ShippingState.Items.FindByValue(ShippingAddress.State)) >
                -1)
            {
                ShippingState.SelectedValue = ShippingAddress.State;
            }
        }

        private void SetBillingStateList(string BillingCountry)
        {
            BillingState.SelectedIndex = -1;
            string sql = String.Empty;
            if (BillingCountry.Length > 0)
            {
                sql = "select s.* from State s " + DB.GetNoLock() + " join country c " + DB.GetNoLock() + " on s.countryid = c.countryid where c.name = " + DB.SQuote(BillingCountry) + " order by s.DisplayOrder,s.Name";
            }
            else
            {
                sql = "select * from State " + DB.GetNoLock() + " where countryid=(select countryid from country " + DB.GetNoLock() + " where name='United States') order by DisplayOrder,Name";
            }

            IDataReader dr = DB.GetRS(sql);
            BillingState.DataSource = dr;
            BillingState.DataTextField = "Name";
            BillingState.DataValueField = "Abbreviation";
            BillingState.DataBind();
            dr.Close();
            if (BillingState.Items.Count == 0)
            {
                BillingState.Items.Insert(0, new ListItem("Other (Non U.S.)", "--"));
                BillingState.SelectedIndex = 0;
            }
            else
            {
                BillingState.Items.Insert(0, new ListItem(AppLogic.GetString("address.cs.11", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), ""));
                BillingState.SelectedIndex = 0;
            }
            if (BillingState.Items.IndexOf(BillingState.Items.FindByValue(BillingAddress.State)) >
                -1)
            {
                BillingState.SelectedValue = BillingAddress.State;
            }
        }

        private bool BillingIsValid()
        {
            bool billingIsValid = true;
            Page.Validate("BillingCheckout1");
            billingIsValid = valReqBillFName.IsValid && valReqBillLName.IsValid && valReqBillPhone.IsValid && valReqBillAddr1.IsValid && valReqBillCity.IsValid && valReqBillZip.IsValid && valReqBillCountry.IsValid;
            return billingIsValid;
        }

        private bool ShippingIsValid()
        {
            bool shippingIsValid = true;
            Page.Validate("ShippingCheckout1");
            shippingIsValid = valReqShipFName.IsValid && valReqShipLName.IsValid && valReqShipPhone.IsValid && valReqShipAddr1.IsValid && valReqShipCity.IsValid && valReqShipState.IsValid && valReqShipZip.IsValid && valReqShipCountry.IsValid;
            return shippingIsValid;
        }

        private bool AccountIsValid()
        {
            bool acctIsValid = true;
            Page.Validate("registration");
            acctIsValid = valRegExValEmail.IsValid && reqValPassword.IsValid && valPassword.IsValid && valReqPhone.IsValid && (!valReqSecurityCode.Enabled || valReqSecurityCode.IsValid) && (!valCustSecurityCode.Enabled || valCustSecurityCode.IsValid);
            return acctIsValid;
        }

        #endregion

        #region Account Info Section

        private void InitializeAccountInfo()
        {
            BillingFirstName.Enabled = false;
            BillingLastName.Enabled = false;
            BillingPhone.Enabled = false;
            BillingPhone.Enabled = false;
            BillingCompany.Enabled = false;
            BillingResidenceType.Enabled = false;
            BillingAddress1.Enabled = false;
            BillingAddress2.Enabled = false;
            BillingSuite.Enabled = false;
            BillingCity.Enabled = false;
            BillingState.Enabled = false;
            BillingZip.Enabled = false;
            BillingCountry.Enabled = false;

            ShippingEqualsBilling.Enabled = false;
            ShippingFirstName.Enabled = false;
            ShippingLastName.Enabled = false;
            ShippingPhone.Enabled = false;
            ShippingPhone.Enabled = false;
            ShippingCompany.Enabled = false;
            ShippingResidenceType.Enabled = false;
            ShippingAddress1.Enabled = false;
            ShippingAddress2.Enabled = false;
            ShippingSuite.Enabled = false;
            ShippingCity.Enabled = false;
            ShippingState.Enabled = false;
            ShippingZip.Enabled = false;
            ShippingCountry.Enabled = false;


            if (CommonLogic.QueryStringCanBeDangerousContent("errormsg").Length > 0)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("errormsg").ToLowerInvariant().IndexOf("<script>") !=
                    -1)
                {
                    throw new ArgumentException("SECURITY EXCEPTION");
                }
                ErrorMsgLabel.Text = Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg")).Replace("+", " ");
            }

            //Account Info
            if (SkipRegistration && !ThisCustomer.IsRegistered)
            {
                pnlSkipReg.Visible = true;
                tblSkipReg.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
                tblSkipRegBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
                skipreg_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/accountinfo.gif");
                if (!AppLogic.AppConfigBool("HidePasswordFieldDuringCheckout"))
                {
                    skipRegSignin.Text = AppLogic.GetString("checkout1.aspx.8", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                }
                if (ThisCustomer.PrimaryShippingAddressID == 0)
                {
                    EnableAddressFields("shipping");
                }
                if (ThisCustomer.PrimaryBillingAddressID == 0)
                {
                    EnableAddressFields("billing");
                }
            }
            else if (!ThisCustomer.IsRegistered)
            {
                Signin.Text = AppLogic.GetString("checkout1.aspx.8", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

                if (ThisCustomer.PrimaryShippingAddressID == 0)
                {
                    EnableAddressFields("shipping");
                }
                if (ThisCustomer.PrimaryBillingAddressID == 0)
                {
                    EnableAddressFields("billing");
                }
                pnlAccountInfo.Visible = true;
                tblAccount.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
                tblAccountBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
                accountinfo_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/accountinfo.gif");
                FirstName.Text = Server.HtmlEncode(CommonLogic.IIF(ThisCustomer.FirstName.Length != 0, ThisCustomer.FirstName, BillingAddress.FirstName));
                LastName.Text = Server.HtmlEncode(CommonLogic.IIF(ThisCustomer.LastName.Length != 0, ThisCustomer.LastName, BillingAddress.LastName));
                password.TextMode = TextBoxMode.Password;
                password2.TextMode = TextBoxMode.Password;

                String emailx = ThisCustomer.EMail;
                EMail.Text = Server.HtmlEncode(emailx);

                if ((AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout") || AppLogic.AppConfigBool("HidePasswordFieldDuringCheckout")))
                {
                    valPassword.Visible = false;
                    valPassword.Enabled = false;
                }

                Phone.Text = Server.HtmlEncode(CommonLogic.IIF(ThisCustomer.Phone.Length != 0, ThisCustomer.Phone, BillingAddress.Phone));
                // Create a phone validation error message

                Checkout1aspx23.Text = "*" + AppLogic.GetString("createaccount.aspx.23", SkinID, ThisCustomer.LocaleSetting);
                if (ThisCustomer.IsRegistered &&
                    ThisCustomer.OKToEMail)
                    OKToEMailYes.Checked = true;
                if (ThisCustomer.IsRegistered &&
                    !ThisCustomer.OKToEMail)
                    OKToEMailNo.Checked = true;

                if (AppLogic.AppConfigBool("RequireOver13Checked"))
                {
                    Over13.Visible = true;
                    Over13.Checked = ThisCustomer.IsOver13;
                }

                if (RequireSecurityCode)
                {
                    // Create a random code and store it in the Session object.
                    Session["SecurityCode"] = CommonLogic.GenerateRandomCode(6);
                    signinaspx21.Visible = true;
                    SecurityCode.Visible = true;
                    valReqSecurityCode.Visible = true;
                    valReqSecurityCode.Enabled = true;
                    valCustSecurityCode.Enabled = true;
                    valCustSecurityCode.Visible = true;
                    valCustSecurityCode.ErrorMessage = AppLogic.GetString("Checkout1_process.aspx.2", 1, Localization.GetWebConfigLocale());
                    SecurityImage.Visible = true;
                    SecurityImage.ImageUrl = "jpegimage.aspx";
                }
            }


            //Billing Info
            tblBillingInfo.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
            tblBillingInfoBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));

            if (AllowShipToDifferentThanBillTo)
            {
                billinginfo_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/billinginfo.gif");
            }
            else
            {
                billinginfo_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/shippingandbillinginfo.gif");
            }

            //BillingEqualsAccount.Visible = true;
            if (AllowShipToDifferentThanBillTo)
            {
                Checkout1aspx30.Text = AppLogic.GetString("createaccount.aspx.30", SkinID, ThisCustomer.LocaleSetting);
            }
            else
            {
                Checkout1aspx30.Text = AppLogic.GetString("checkout1.aspx.2", SkinID, ThisCustomer.LocaleSetting);
            }

            //BillingFirstName.Text = Server.HtmlEncode(CommonLogic.IIF(ThisCustomer.FirstName.Length != 0, ThisCustomer.FirstName, BillingAddress.FirstName));
            //BillingLastName.Text = Server.HtmlEncode(CommonLogic.IIF(ThisCustomer.LastName.Length != 0, ThisCustomer.LastName, BillingAddress.LastName));
            BillingFirstName.Text = Server.HtmlEncode(BillingAddress.FirstName);
            BillingLastName.Text = Server.HtmlEncode(BillingAddress.LastName);
            BillingPhone.Text = Server.HtmlEncode(BillingAddress.Phone);
            BillingCompany.Text = Server.HtmlEncode(BillingAddress.Company);
            BillingResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.55", SkinID, ThisCustomer.LocaleSetting), ((int) ResidenceTypes.Unknown).ToString()));
            BillingResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.56", SkinID, ThisCustomer.LocaleSetting), ((int) ResidenceTypes.Residential).ToString()));
            BillingResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.57", SkinID, ThisCustomer.LocaleSetting), ((int) ResidenceTypes.Commercial).ToString()));
            BillingResidenceType.SelectedIndex = 1;
            BillingAddress1.Text = Server.HtmlEncode(BillingAddress.Address1);
            BillingAddress2.Text = Server.HtmlEncode(BillingAddress.Address2);
            BillingSuite.Text = Server.HtmlEncode(BillingAddress.Suite);
            BillingCity.Text = Server.HtmlEncode(BillingAddress.City);
            BillingZip.Text = BillingAddress.Zip;


            IDataReader dr = DB.GetRS("select * from Country " + DB.GetNoLock() + " where Published = 1 order by DisplayOrder,Name");
            BillingCountry.DataSource = dr;
            BillingCountry.DataTextField = "Name";
            BillingCountry.DataValueField = "Name";
            BillingCountry.DataBind();
            dr.Close();

            if (BillingAddress.Country.Length > 0)
            {
                BillingCountry.SelectedValue = BillingAddress.Country;
            }

            SetBillingStateList(BillingCountry.SelectedValue);

            //Shipping Info
            if (AllowShipToDifferentThanBillTo && !AppLogic.AppConfigBool("SkipShippingOnCheckout") && !cart.IsAllDownloadComponents() &&
                !cart.IsAllSystemComponents())
            {
                pnlShippingInfo.Visible = true;
                tblShippingInfo.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
                tblShippingInfoBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
                shippinginfo_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/shippinginfo.gif");

                ShippingFirstName.Text = Server.HtmlEncode(ShippingAddress.FirstName);
                //ShippingLastName.Text = Server.HtmlEncode(CommonLogic.IIF(ThisCustomer.LastName.Length != 0, ThisCustomer.LastName, ShippingAddress.LastName));
                ShippingLastName.Text = Server.HtmlEncode(ShippingAddress.LastName);
                ShippingPhone.Text = Server.HtmlEncode(ShippingAddress.Phone);
                ShippingCompany.Text = Server.HtmlEncode(ShippingAddress.Company);
                //ShippingResidenceType
                ShippingResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.55", SkinID, ThisCustomer.LocaleSetting), ((int) ResidenceTypes.Unknown).ToString()));
                ShippingResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.56", SkinID, ThisCustomer.LocaleSetting), ((int) ResidenceTypes.Residential).ToString()));
                ShippingResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.57", SkinID, ThisCustomer.LocaleSetting), ((int) ResidenceTypes.Commercial).ToString()));
                ShippingResidenceType.SelectedIndex = 1;
                ShippingAddress1.Text = Server.HtmlEncode(ShippingAddress.Address1);
                ShippingAddress2.Text = Server.HtmlEncode(ShippingAddress.Address2);
                ShippingSuite.Text = Server.HtmlEncode(ShippingAddress.Suite);
                ShippingCity.Text = Server.HtmlEncode(ShippingAddress.City);
                ShippingZip.Text = ShippingAddress.Zip;


                dr = DB.GetRS("select * from Country  " + DB.GetNoLock() + " where Published = 1 order by DisplayOrder,Name");
                ShippingCountry.DataSource = dr;
                ShippingCountry.DataTextField = "Name";
                ShippingCountry.DataValueField = "Name";
                ShippingCountry.DataBind();
                dr.Close();

                if (ShippingAddress.Country.Length > 0)
                {
                    ShippingCountry.SelectedValue = ShippingAddress.Country;
                }

                SetShippingStateList(ShippingCountry.SelectedValue);
            }
        }

        private void EnableAddressFields(string which)
        {
            if (which == "billing" ||
                which == "both")
            {
                BillingFirstName.Enabled = true;
                BillingLastName.Enabled = true;
                BillingPhone.Enabled = true;
                BillingPhone.Enabled = true;
                BillingCompany.Enabled = true;
                BillingResidenceType.Enabled = true;
                BillingAddress1.Enabled = true;
                BillingAddress2.Enabled = true;
                BillingSuite.Enabled = true;
                BillingCity.Enabled = true;
                BillingState.Enabled = true;
                BillingZip.Enabled = true;
                BillingCountry.Enabled = true;
            }

            if (which == "shipping" ||
                which == "both")
            {
                ShippingEqualsBilling.Enabled = true;
                ShippingFirstName.Enabled = true;
                ShippingLastName.Enabled = true;
                ShippingPhone.Enabled = true;
                ShippingPhone.Enabled = true;
                ShippingCompany.Enabled = true;
                ShippingResidenceType.Enabled = true;
                ShippingAddress1.Enabled = true;
                ShippingAddress2.Enabled = true;
                ShippingSuite.Enabled = true;
                ShippingCity.Enabled = true;
                ShippingState.Enabled = true;
                ShippingZip.Enabled = true;
                ShippingCountry.Enabled = true;
            }
        }

        private bool ProcessAccount()
        {
            string AccountName = (FirstName.Text.Trim() + " " + LastName.Text.Trim()).Trim();
            if (SkipRegistration)
            {
                AccountName = (BillingFirstName.Text.Trim() + " " + BillingLastName.Text.Trim()).Trim();
            }

            if (Page.IsValid &&
                (AccountName.Length > 0 || ThisCustomer.IsRegistered))
            {
                if (!ThisCustomer.IsRegistered)
                {
                    String EMailField = CommonLogic.IIF(SkipRegistration, txtSkipRegEmail.Text.ToLower().Trim(), EMail.Text.ToLower().Trim());
                    bool EMailAlreadyTaken = false;

                    EMailAlreadyTaken = Customer.EmailInUse(EMailField, ThisCustomer.CustomerID) && !AppLogic.AppConfigBool("AllowCustomerDuplicateEMailAddresses");

                    String PWD = ViewState["custpwd"].ToString();
                    Password p = new Password(PWD);
                    String newpwd = p.SaltedPassword;
                    Nullable<int> newsaltkey = p.Salt;
                    if (ThisCustomer.Password.Length != 0)
                    {
                        // do NOT allow passwords to be changed on this page. this is only for creating an account.
                        // if they want to change their password, they must use their account page
                        newpwd = null;
                        newsaltkey = null;
                    }
                    if (!EMailAlreadyTaken)
                    {
                        ThisCustomer.UpdateCustomer(
                            /*CustomerLevelID*/ null,
                                                /*EMail*/ EMailField,
                                                /*SaltedAndHashedPassword*/ newpwd,
                                                /*SaltKey*/ newsaltkey,
                                                /*DateOfBirth*/ null,
                                                /*Gender*/ null,
                                                /*FirstName*/ FirstName.Text.Trim(),
                                                /*LastName*/ LastName.Text.Trim(),
                                                /*Notes*/ null,
                                                /*SkinID*/ null,
                                                /*Phone*/ Phone.Text.Trim(),
                                                /*AffiliateID*/ null,
                                                /*Referrer*/ null,
                                                /*CouponCode*/ null,
                                                /*OkToEmail*/ CommonLogic.IIF(OKToEMailYes.Checked, 1, 0),
                                                /*IsAdmin*/ null,
                                                /*BillingEqualsShipping*/ CommonLogic.IIF(AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo"), 0, 1),
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
                                                /*Over13Checked*/ CommonLogic.IIF(Over13.Checked || SkipRegOver13.Checked, 1, 0),
                                                /*CurrencySetting*/ null,
                                                /*VATSetting*/ null,
                                                /*VATRegistrationID*/ null,
                                                /*StoreCCInDB*/ null,
                                                /*IsRegistered*/ CommonLogic.IIF(SkipRegistration, 0, 1),
                                                /*LockedUntil*/ null,
                                                /*AdminCanViewCC*/ null,
                                                /*BadLogin*/ null,
                                                /*Active*/ null,
                                                /*PwdChangeRequired*/ null,
                                                /*RegisterDate*/ null
                            );


                        BillingAddress = new Address();
                        if (ThisCustomer.PrimaryBillingAddressID == 0)
                        {
                            BillingAddress.LastName = BillingLastName.Text;
                            BillingAddress.FirstName = BillingFirstName.Text;
                            BillingAddress.Phone = BillingPhone.Text;
                            BillingAddress.Company = BillingCompany.Text;
                            BillingAddress.ResidenceType = (ResidenceTypes) Convert.ToInt32(BillingResidenceType.SelectedValue);
                            BillingAddress.Address1 = BillingAddress1.Text;
                            BillingAddress.Address2 = BillingAddress2.Text;
                            BillingAddress.Suite = BillingSuite.Text;
                            BillingAddress.City = BillingCity.Text;
                            BillingAddress.State = BillingState.SelectedValue;
                            BillingAddress.Zip = BillingZip.Text;
                            BillingAddress.Country = BillingCountry.SelectedValue;
                            BillingAddress.EMail = EMailField;

                            BillingAddress.InsertDB(ThisCustomer.CustomerID);
                            BillingAddress.MakeCustomersPrimaryAddress(AddressTypes.Billing);
                            ThisCustomer.PrimaryBillingAddressID = BillingAddress.AddressID;
                        }

                        if (AllowShipToDifferentThanBillTo && !AppLogic.AppConfigBool("SkipShippingOnCheckout"))
                        {
                            ShippingAddress = new Address();
                            if (ThisCustomer.PrimaryShippingAddressID == 0)
                            {
                                ShippingAddress.LastName = ShippingLastName.Text;
                                ShippingAddress.FirstName = ShippingFirstName.Text;
                                ShippingAddress.Phone = ShippingPhone.Text;
                                ShippingAddress.Company = ShippingCompany.Text;
                                ShippingAddress.ResidenceType = (ResidenceTypes) Convert.ToInt32(ShippingResidenceType.SelectedValue);
                                ShippingAddress.Address1 = ShippingAddress1.Text;
                                ShippingAddress.Address2 = ShippingAddress2.Text;
                                ShippingAddress.Suite = ShippingSuite.Text;
                                ShippingAddress.City = ShippingCity.Text;
                                ShippingAddress.State = ShippingState.SelectedValue;
                                ShippingAddress.Zip = ShippingZip.Text;
                                ShippingAddress.Country = ShippingCountry.SelectedValue;
                                ShippingAddress.EMail = EMailField;

                                ShippingAddress.InsertDB(ThisCustomer.CustomerID);
                                ShippingAddress.MakeCustomersPrimaryAddress(AddressTypes.Shipping);
                                ThisCustomer.PrimaryShippingAddressID = ShippingAddress.AddressID;
                            }
                        }
                        else
                        {
                            BillingAddress.MakeCustomersPrimaryAddress(AddressTypes.Shipping);
                        }

                        if (AppLogic.AppConfigBool("SendWelcomeEmail"))
                        {
                            AppLogic.SendMail(AppLogic.GetString("createaccount.aspx.79", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), AppLogic.RunXmlPackage(AppLogic.AppConfig("XmlPackage.WelcomeEmail"), null, ThisCustomer, SkinID, "", "fullname=" + FirstName.Text.Trim() + " " + LastName.Text.Trim(), false, false, EntityHelpers), true, AppLogic.AppConfig("MailMe_FromAddress"), AppLogic.AppConfig("MailMe_FromName"), EMailField, FirstName.Text.Trim() + " " + LastName.Text.Trim(), "", AppLogic.AppConfig("MailMe_Server"));
                        }
                    }
                    else
                    {
                        ErrorMsgLabel.Text = AppLogic.GetString("createaccount_process.aspx.1", 1, Localization.GetWebConfigLocale());
                        return false;
                    }
                }
            }
            else
            {
                ErrorMsgLabel.Text += "<br /><br /> Some errors occured trying to create your account.  Please correct them and try again.<br /><br />";
                if (AccountName.Length == 0)
                    ErrorMsgLabel.Text += "&bull; " + AppLogic.GetString("createaccount.aspx.5", 1, Localization.GetWebConfigLocale()) + "<br />";
                foreach (IValidator aValidator in Validators)
                {
                    if (!aValidator.IsValid)
                    {
                        ErrorMsgLabel.Text += "&bull; " + aValidator.ErrorMessage + "<br />";
                    }
                }
                ErrorMsgLabel.Text += "<br />";
                return false;
            }
            return true;
        }

        #endregion

        #region Shipping Options Section

        private void InitializeShippingOptions(ShoppingCart cart)
        {
            if (ThisCustomer.PrimaryShippingAddressID > 0)
            {
                pnlShippingOptions.Visible = true;
                pnlRecalcShipping.Visible = false;
                btnContinueCheckout.Enabled = true;
                //Page.ClientScript.RegisterOnSubmitStatement(this.GetType(), "formsubmit", "if(!validateCheckout()) return false;\n");
            }
            else
            {
                pnlShippingOptions.Visible = false;
                pnlRecalcShipping.Visible = true;
                btnContinueCheckout.Enabled = false;
            }

            if (AppLogic.AppConfigBool("SkipShippingOnCheckout") || cart.IsAllDownloadComponents() ||
                cart.IsAllSystemComponents())
            {
                tblShippingSelect.Visible = false;
                btnContinueCheckout.Enabled = true;
                return;
            }
            else
            {
                tblShippingSelect.Visible = true;
            }


            if (!cart.ShippingIsFree &&
                cart.MoreNeededToReachFreeShipping != 0.0M)
            {
                pnlGetFreeShippingMsg.Visible = true;
                GetFreeShippingMsg.Text = String.Format(AppLogic.GetString("checkoutshipping.aspx.2", SkinID, ThisCustomer.LocaleSetting), ThisCustomer.CurrencyString(cart.FreeShippingThreshold), CommonLogic.Capitalize(cart.FreeShippingMethod));
            }

            FreeShippingMsg.Text = "";
            if (cart.ShippingIsFree)
            {
                FreeShippingMsg.Text = cart.GetFreeShippingReason();
            }

            StringBuilder CValScript = new StringBuilder(1000);
            CValScript.Append("<script type=\"text/javascript\">\n");

            CValScript.Append("// return the value of the radio button that is checked\n");
            CValScript.Append("// return '' if none are checked, or there are no radio buttons\n");
            CValScript.Append("function getCheckedValue(radioObj) {\n");
            CValScript.Append("	if(!radioObj)\n");
            CValScript.Append("		return '';\n");
            CValScript.Append("	var radioLength = radioObj.length;\n");
            CValScript.Append("	if(radioLength == undefined)\n");
            CValScript.Append("		if(radioObj.checked)\n");
            CValScript.Append("			return radioObj.value;\n");
            CValScript.Append("		else\n");
            CValScript.Append("			return '';\n");
            CValScript.Append("	for(var i = 0; i < radioLength; i++) {\n");
            CValScript.Append("		if(radioObj[i].checked) {\n");
            CValScript.Append("			return radioObj[i].value;\n");
            CValScript.Append("		}\n");
            CValScript.Append("	}\n");
            CValScript.Append("	return '';\n");
            CValScript.Append("}\n");

            CValScript.Append("function CheckoutShippingForm_Validator()\n");
            CValScript.Append("{\n");
            if (cart.CartAllowsShippingMethodSelection)
            {
                CValScript.Append("var objs = document.getElementsByName(\"ShippingMethodID\");\n");
                CValScript.Append("myOption = getCheckedValue(objs);\n");
                //CValScript.Append("myOption = getCheckedValue(theForm.ShippingMethodID);\n"); // this didn't work 100% of the time. have no clue why!!
                CValScript.Append("if (myOption == '')\n");
                CValScript.Append("{\n");
                CValScript.Append("    alert(\"" + AppLogic.GetString("checkoutshipping.aspx.17", SkinID, ThisCustomer.LocaleSetting) + "\");\n");
                CValScript.Append("    return (false);\n");
                CValScript.Append("}\n");
                CValScript.Append("return (true);\n");
            }
            else
            {
                CValScript.Append("return (true);\n");
            }
            CValScript.Append("}\n");
            CValScript.Append("</script>\n");

            CheckoutValidationScript.Text = CValScript.ToString();


            bool AnyShippingMethodsFound = false;
            pnlCartAllowsShippingMethodSelection.Visible = cart.CartAllowsShippingMethodSelection;
            if (cart.CartAllowsShippingMethodSelection)
            {
                if (Shipping.MultiShipEnabled() &&
                    cart.TotalQuantity() > 1)
                {
                    ShipSelectionMsg.Text = "<p><b>" + String.Format(AppLogic.GetString("checkoutshipping.aspx.15", SkinID, ThisCustomer.LocaleSetting), "checkoutshippingmult.aspx") + "</b></p>";
                }
                else
                {
                    ShipSelectionMsg.Text = "<p><b>" + AppLogic.GetString("checkoutshipping.aspx.11", SkinID, ThisCustomer.LocaleSetting) + "</b></p>";
                }
            }
            String ShipMethods = cart.GetShippingMethodList(String.Empty, out AnyShippingMethodsFound);
            ShippingOptions.Text = ShipMethods;


            if ((AppLogic.AppConfigBool("RTShipping.DumpXMLOnCheckoutShippingPage") || AppLogic.AppConfigBool("RTShipping.DumpXMLOnCartPage")) &&
                cart.ShipCalcID == Shipping.ShippingCalculationEnum.UseRealTimeRates)
            {
                StringBuilder tmpS = new StringBuilder(4096);
                tmpS.Append("<hr break=\"all\"/>");
                IDataReader rs = DB.GetRS("Select RTShipRequest,RTShipResponse from customer " + DB.GetNoLock() + " where CustomerID=" + ThisCustomer.CustomerID.ToString());
                if (rs.Read())
                {
                    String s = DB.RSField(rs, "RTShipRequest");
                    s = s.Replace("<?xml version=\"1.0\"?>", "");
                    try
                    {
                        s = XmlCommon.PrettyPrintXml("<roottag_justaddedfordisplayonthispage>" + s + "</roottag_justaddedfordisplayonthispage>"); // the RTShipRequest may have "two" XML docs in it :)
                    }
                    catch
                    {
                        s = DB.RSField(rs, "RTShipRequest");
                    }
                    tmpS.Append("<b>" + AppLogic.GetString("shoppingcart.aspx.5", SkinID, ThisCustomer.LocaleSetting) + "</b><br/><textarea rows=60 style=\"width: 100%\">" + s + "</textarea><br/><br/>");
                    try
                    {
                        s = XmlCommon.PrettyPrintXml(DB.RSField(rs, "RTShipResponse"));
                    }
                    catch
                    {
                        s = DB.RSField(rs, "RTShipResponse");
                    }
                    tmpS.Append("<b>" + AppLogic.GetString("shoppingcart.aspx.6", SkinID, ThisCustomer.LocaleSetting) + "</b><br/><textarea rows=60 style=\"width: 100%\">" + s + "</textarea><br/><br/>");
                }
                rs.Close();
                DebugInfo.Text = tmpS.ToString();
            }
        }

        private bool ProcessShipping(ShoppingCart cart)
        {
            if (AppLogic.AppConfigBool("SkipShippingOnCheckout"))
            {
                return true;
            }


            String ShippingMethodIDFormField = CommonLogic.FormCanBeDangerousContent("ShippingMethodID").Replace(",", ""); // remember to remove the stupid hidden field which adds a comma to the form post (stupid stupid javascript again)
            if (ShippingMethodIDFormField.Length == 0 &&
                !cart.ShippingIsFree)
            {
                ErrorMsgLabel.Text = AppLogic.GetString("checkoutshipping.aspx.17", SkinID, ThisCustomer.LocaleSetting);
                return false;
            }
            else
            {
                if (cart.IsEmpty())
                {
                    Response.Redirect("shoppingcart.aspx");
                }

                int ShippingMethodID = 0;
                String ShippingMethod = String.Empty;
                if (cart.ShipCalcID !=
                    Shipping.ShippingCalculationEnum.UseRealTimeRates)
                {
                    ShippingMethodID = Localization.ParseUSInt(ShippingMethodIDFormField);
                    ShippingMethod = Shipping.GetShippingMethodName(ShippingMethodID, null);
                }
                else
                {
                    if (ShippingMethodIDFormField.Length != 0 &&
                        ShippingMethodIDFormField.IndexOf('|') != -1)
                    {
                        String[] frmsplit = ShippingMethodIDFormField.Split('|');
                        ShippingMethodID = Localization.ParseUSInt(frmsplit[0]);
                        ShippingMethod = String.Format("{0}|{1}", frmsplit[1], frmsplit[2]);
                    }
                }

                if (cart.ShippingIsFree )
                {
                    ShippingMethodID = 0;
                    ShippingMethod = string.Format("FREE ({0})", cart.GetFreeShippingReason());
                }

                String sql = String.Format("update ShoppingCart set ShippingMethodID={0}, ShippingMethod={1} where CustomerID={2} and CartType={3}", ShippingMethodID.ToString(), DB.SQuote(ShippingMethod), ThisCustomer.CustomerID.ToString(), ((int) CartTypeEnum.ShoppingCart).ToString());
                DB.ExecuteSQL(sql);
                this.cart = new ShoppingCart(SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);
                if (cart.ContainsGiftCard())
                {
                    Response.Redirect("checkoutgiftcard.aspx");
                }
            }
            return true;
        }

        #endregion

        #region Payment Options Section

        private void InitializePaymentOptions(ShoppingCart cart)
        {
            JSPopupRoutines.Text = AppLogic.GetJSPopupRoutines();

            //HERE WE WILL DO THE LOOKUP for the new supported Shipping2Payment mapping
            if (AppLogic.AppConfigBool("UseMappingShipToPayment"))
            {
                try
                {
                    int intCustomerSelectedShippingMethodID = cart.FirstItem().m_ShippingMethodID;
                    IDataReader rsReferencePMForSelectedShippingMethod = DB.GetRS("SELECT MappedPM FROM ShippingMethod WHERE ShippingMethodID=" + intCustomerSelectedShippingMethodID.ToString());

                    while (rsReferencePMForSelectedShippingMethod.Read())
                    {
                        AllowedPaymentMethods = DB.RSField(rsReferencePMForSelectedShippingMethod, "MappedPM").ToUpperInvariant();
                    }
                    rsReferencePMForSelectedShippingMethod.Close();

                    if (AllowedPaymentMethods.Length <= 0)
                    {
                        AllowedPaymentMethods = AppLogic.AppConfig("PaymentMethods").ToUpperInvariant();
                    }
                }
                catch
                {
                    AllowedPaymentMethods = AppLogic.AppConfig("PaymentMethods").ToUpperInvariant();
                }
            }
            else
            {
                AllowedPaymentMethods = AppLogic.AppConfig("PaymentMethods").ToUpperInvariant();

                if (AppLogic.MicropayIsEnabled() &&
                    !cart.HasSystemComponents())
                {
                    if (AllowedPaymentMethods.Length != 0)
                    {
                        AllowedPaymentMethods += ",";
                    }
                    AllowedPaymentMethods += AppLogic.ro_PMMicropay;
                }
            }

            // When PAYPALPRO is active Gateway or PAYPALEXPRESS is available Payment Method
            // then we want to make the PayPal Express Mark available
            if ((AppLogic.ActivePaymentGatewayCleaned() == Gateway.ro_GWPAYPALPRO || AllowedPaymentMethods.IndexOf(AppLogic.ro_PMPayPalExpress) > -1)
                &&
                AllowedPaymentMethods.IndexOf(AppLogic.ro_PMPayPalExpressMark) == -1)
            {
                if (AllowedPaymentMethods.Length != 0)
                {
                    AllowedPaymentMethods += ",";
                }
                AllowedPaymentMethods += AppLogic.ro_PMPayPalExpressMark;
            }

            // Need to dbl check this
            SelectedPaymentType = CommonLogic.IIF(SelectedPaymentType == "" && ThisCustomer.RequestedPaymentMethod != "" && AllowedPaymentMethods.IndexOf(ThisCustomer.RequestedPaymentMethod.ToUpperInvariant()) != -1, AppLogic.CleanPaymentMethod(ThisCustomer.RequestedPaymentMethod), ViewState["SelectedPaymentType"].ToString());

            //Set credit card pane to be visible if that payment method is allowed, and no other payment method
            // is trying to be shown: If UseMappingShipToPayment is not activated Credit Card will always be
            // the default payment option that shows expnande to the customer.
            if (AppLogic.AppConfigBool("UseMappingShipToPayment"))
            {
                string[] strSplittedCurrentMappingsInDB = AllowedPaymentMethods.Split(new char[] {','});

                String PM = AppLogic.CleanPaymentMethod(strSplittedCurrentMappingsInDB[0]);
                if (PM == AppLogic.ro_PMMicropay)
                {
                    if (SelectedPaymentType.Length == 0 &&
                        AllowedPaymentMethods.IndexOf(AppLogic.ro_PMMicropay) != -1)
                    {
                        ResetPaymentPanes();
                        SelectedPaymentType = AppLogic.ro_PMMicropay;
                        pmtMICROPAY.Checked = true;
                        pnlMicroPayPane.Visible = true;
                    }
                }
                else if (PM == AppLogic.ro_PMPurchaseOrder)
                {
                    if (SelectedPaymentType.Length == 0 &&
                        AllowedPaymentMethods.IndexOf(AppLogic.ro_PMPurchaseOrder) != -1)
                    {
                        ResetPaymentPanes();
                        SelectedPaymentType = AppLogic.ro_PMPurchaseOrder;
                        pmtPURCHASEORDER.Checked = true;
                        pnlPOPane.Visible = true;
                    }
                }
                else if (PM == AppLogic.ro_PMCreditCard)
                {
                    if (SelectedPaymentType.Length == 0 &&
                        AllowedPaymentMethods.IndexOf(AppLogic.ro_PMCreditCard) != -1)
                    {
                        ResetPaymentPanes();
                        SelectedPaymentType = AppLogic.ro_PMCreditCard;
                        pmtCreditCard.Checked = true;
                        pnlCreditCardPane.Visible = true;
                    }
                }
                else if (PM == AppLogic.ro_PMPayPal)
                {
                    if (SelectedPaymentType.Length == 0 &&
                        AllowedPaymentMethods.IndexOf(AppLogic.ro_PMPayPal) != -1)
                    {
                        ResetPaymentPanes();
                        SelectedPaymentType = AppLogic.ro_PMPayPal;
                        pmtPAYPAL.Checked = true;
                        pnlPayPalPane.Visible = true;
                    }
                }
                else if (PM == AppLogic.ro_PMPayPalExpress)
                {
                    if (SelectedPaymentType.Length == 0 &&
                        AllowedPaymentMethods.IndexOf(AppLogic.ro_PMPayPalExpress) != -1)
                    {
                        ResetPaymentPanes();
                        SelectedPaymentType = AppLogic.ro_PMPayPalExpress;
                        pmtPAYPALEXPRESS.Checked = true;
                        pnlPayPalExpressPane.Visible = true;
                    }
                }
                else if (PM == AppLogic.ro_PMCOD)
                {
                    if (SelectedPaymentType.Length == 0 &&
                        AllowedPaymentMethods.IndexOf(AppLogic.ro_PMCOD) != -1)
                    {
                        ResetPaymentPanes();
                        SelectedPaymentType = AppLogic.ro_PMCOD;
                        pmtCOD.Checked = true;
                        pnlCODPane.Visible = true;
                    }
                }

                else if (PM == AppLogic.ro_PMECheck)
                {
                    if (SelectedPaymentType.Length == 0 &&
                        AllowedPaymentMethods.IndexOf(AppLogic.ro_PMECheck) != -1)
                    {
                        ResetPaymentPanes();
                        SelectedPaymentType = AppLogic.ro_PMECheck;
                        pmtECHECK.Checked = true;
                        pnlECheckPane.Visible = true;
                    }
                }

                else if (PM == AppLogic.ro_PMCheckByMail)
                {
                    if (SelectedPaymentType.Length == 0 &&
                        AllowedPaymentMethods.IndexOf(AppLogic.ro_PMCheckByMail) != -1)
                    {
                        ResetPaymentPanes();
                        SelectedPaymentType = AppLogic.ro_PMCheckByMail;
                        pmtCHECKBYMAIL.Checked = true;
                        pnlCheckByMailPane.Visible = true;
                    }
                }

                else if (PM == AppLogic.ro_PMRequestQuote)
                {
                    if (SelectedPaymentType.Length == 0 &&
                        AllowedPaymentMethods.IndexOf(AppLogic.ro_PMRequestQuote) != -1)
                    {
                        ResetPaymentPanes();
                        SelectedPaymentType = AppLogic.ro_PMRequestQuote;
                        pmtREQUESTQUOTE.Checked = true;
                        pnlReqQuotePane.Visible = true;
                    }
                }


                else if (PM == AppLogic.ro_PMCODNet30)
                {
                    if (SelectedPaymentType.Length == 0 &&
                        AllowedPaymentMethods.IndexOf(AppLogic.ro_PMCODNet30) != -1)
                    {
                        ResetPaymentPanes();
                        SelectedPaymentType = AppLogic.ro_PMCODNet30;
                        pmtCODNET30.Checked = true;
                        pnlCODNet30Pane.Visible = true;
                    }
                }

                else if (PM == AppLogic.ro_PMCODCompanyCheck)
                {
                    if (SelectedPaymentType.Length == 0 &&
                        AllowedPaymentMethods.IndexOf(AppLogic.ro_PMCODCompanyCheck) != -1)
                    {
                        ResetPaymentPanes();
                        SelectedPaymentType = AppLogic.ro_PMCODCompanyCheck;
                        pmtCODCOMPANYCHECK.Checked = true;
                        pnlCODCoCheckPane.Visible = true;
                    }
                }

                else if (PM == AppLogic.ro_PMCODMoneyOrder)
                {
                    if (SelectedPaymentType.Length == 0 &&
                        AllowedPaymentMethods.IndexOf(AppLogic.ro_PMCODMoneyOrder) != -1)
                    {
                        ResetPaymentPanes();
                        SelectedPaymentType = AppLogic.ro_PMCODMoneyOrder;
                        pmtCODMONEYORDER.Checked = true;
                        pnlCODMOPane.Visible = true;
                    }
                }
            }


            String TransactionMode = AppLogic.AppConfig("TransactionMode").Trim().ToUpperInvariant();
            bool useLiveTransactions = AppLogic.AppConfigBool("UseLiveTransactions");

            StringBuilder OrderFinalizationInstructions = new StringBuilder(4096);
            String OrderFinalizationXmlPackageName = AppLogic.AppConfig("XmlPackage.OrderFinalization");
            String OrderFinalizationXmlPackageFN = Server.MapPath("xmlpackages/" + OrderFinalizationXmlPackageName);

            if (CommonLogic.FileExists(OrderFinalizationXmlPackageFN))
            {
                XmlPackage2 p = new XmlPackage2(OrderFinalizationXmlPackageName, ThisCustomer, SkinID);
                OrderFinalizationInstructions.Append("<p align=\"left\"><b>" + AppLogic.GetString("checkoutreview.aspx.24", SkinID, ThisCustomer.LocaleSetting) + "</b></p>");
                OrderFinalizationInstructions.Append(p.TransformString());
            }
            if (OrderFinalizationInstructions.Length != 0)
            {
                OrderFinalizationInstructions.Append("<br/>");
            }
            Finalization.Text = OrderFinalizationInstructions.ToString(); // set the no payment panel here, in case it is needed

            if (CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg").Length != 0)
            {
                AppLogic.CheckForScriptTag(CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg"));
                pnlErrorMsg.Visible = true;
                ErrorMsgLabel.Text = Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg")).Replace("+", " ");
            }

            String XmlPackageName = AppLogic.AppConfig("XmlPackage.CheckoutPaymentPageHeader");
            if (XmlPackageName.Length != 0)
            {
                XmlPackage_CheckoutPaymentPageHeader.Text = AppLogic.RunXmlPackage(XmlPackageName, base.GetParser, ThisCustomer, SkinID, String.Empty, String.Empty, true, true);
            }


            if (NetTotal == Decimal.Zero &&
                AppLogic.AppConfigBool("SkipPaymentEntryOnZeroDollarCheckout"))
            {
                NoPaymentRequired.Text = AppLogic.GetString("checkoutpayment.aspx.28", SkinID, ThisCustomer.LocaleSetting);
                pnlNoPaymentRequired.Visible = true;
                pnlPaymentOptions.Visible = false;
                paymentPanes.Visible = false;
                //if (RequireTerms)
                //{
                //    Form.Attributes.Add("onsubmit", "return(TermsChecked())");
                //}
            }

            WritePaymentPanels(OrderFinalizationInstructions.ToString(), TransactionMode);

            if (RequireTerms)
            {
                pnlTerms.Visible = true;
                terms.Text = AppLogic.GetCheckoutTermsAndConditions(SkinID, ThisCustomer.LocaleSetting, base.GetParser, false);
            }
        }

        private void ProcessPayment()
        {
            int OrderNumber = 0;
            string PaymentMethod;

            if (NetTotal == Decimal.Zero && AppLogic.AppConfigBool("SkipPaymentEntryOnZeroDollarCheckout"))
            {                
                PaymentMethod = "Credit Card";
            }
            else
            {
                PaymentMethod = CommonLogic.FormCanBeDangerousContent("PaymentMethod");
            }

            AppLogic.ValidatePM(PaymentMethod); // this WILL throw a hard security exception on any problem!

            if (!ThisCustomer.IsRegistered)
            {
                bool boolAllowAnon = AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout");

                if (!boolAllowAnon &&
                    (PaymentMethod == AppLogic.ro_PMPayPalExpress || PaymentMethod == AppLogic.ro_PMPayPalExpressMark))
                {
                    boolAllowAnon = AppLogic.AppConfigBool("PayPal.Express.AllowAnonCheckout");
                }

                if (!boolAllowAnon)
                {
                    Response.Redirect("createaccount.aspx?checkout=true");
                }
            }

            if (ThisCustomer.PrimaryBillingAddressID == 0 ||
                (ThisCustomer.PrimaryShippingAddressID == 0 && (!AppLogic.AppConfigBool("SkipShippingOnCheckout") || !cart.IsAllDownloadComponents() || !cart.IsAllSystemComponents())))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutpayment.aspx.2", SkinID, ThisCustomer.LocaleSetting)));
            }

            if (cart.IsEmpty())
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (cart.InventoryTrimmed)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("shoppingcart.aspx.3", SkinID, ThisCustomer.LocaleSetting)));
            }

            if (cart.RecurringScheduleConflict)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("shoppingcart.aspx.19", SkinID, ThisCustomer.LocaleSetting)));
            }

            if (cart.HasCoupon() &&
                !cart.CouponIsValid)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&discountvalid=false");
            }

            if (!cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (!cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            // re-validate all shipping info, as ANYTHING could have changed since last page:
            if (!cart.ShippingIsAllValid())
            {
                HttpContext.Current.Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + HttpContext.Current.Server.UrlEncode(AppLogic.GetString("shoppingcart.cs.95", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)));
            }

            Address BillingAddress = new Address();
            BillingAddress.LoadByCustomer(ThisCustomer.CustomerID, ThisCustomer.PrimaryBillingAddressID, AddressTypes.Billing);

            // ----------------------------------------------------------------
            // Get the finalization info (if any):
            // ----------------------------------------------------------------
            StringBuilder FinalizationXml = new StringBuilder(4096);
            FinalizationXml.Append("<root>");
            for (int i = 0; i < Request.Form.Count; i++)
            {
                String FieldName = Request.Form.Keys[i];
                String FieldVal = Request.Form[Request.Form.Keys[i]].Trim();
                if (FieldName.StartsWith("finalization", StringComparison.InvariantCultureIgnoreCase) &&
                    !FieldName.EndsWith("_vldt", StringComparison.InvariantCultureIgnoreCase))
                {
                    FinalizationXml.Append("<field>");
                    FinalizationXml.Append("<" + XmlCommon.XmlEncode(FieldName) + ">");
                    FinalizationXml.Append(XmlCommon.XmlEncode(FieldVal));
                    FinalizationXml.Append("</" + XmlCommon.XmlEncode(FieldName) + ">");
                    FinalizationXml.Append("</field>");
                }
            }
            FinalizationXml.Append("</root>");
            DB.ExecuteSQL(String.Format("update customer set FinalizationData={0} where CustomerID={1}", DB.SQuote(FinalizationXml.ToString()), ThisCustomer.CustomerID.ToString()));

            // ----------------------------------------------------------------
            // Store the payment info (if required):
            // ----------------------------------------------------------------
            if (PaymentMethod.Length == 0)
            {
                ErrorMsgLabel.Text = AppLogic.GetString("checkoutpayment.aspx.20", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                return;
            }
            String PM = AppLogic.CleanPaymentMethod(PaymentMethod);
            if (PM == AppLogic.ro_PMCreditCard)
            {
                String CardName = CommonLogic.FormCanBeDangerousContent("CardName");
                String CardNumber = CommonLogic.FormCanBeDangerousContent("CardNumber").Trim().Replace(" ", "");
                String CardExtraCode = CommonLogic.FormCanBeDangerousContent("CardExtraCode").Trim().Replace(" ", "");
                String CardType = CommonLogic.FormCanBeDangerousContent("CardType").Trim().Replace(" ", "");
                String CardExpirationMonth = CommonLogic.FormCanBeDangerousContent("CardExpirationMonth").Trim().Replace(" ", "");
                String CardExpirationYear = CommonLogic.FormCanBeDangerousContent("CardExpirationYear").Trim().Replace(" ", "");
                String CardStartDate = CommonLogic.FormCanBeDangerousContent("CardStartDateMonth").Trim().Replace(" ", "").PadLeft(2, '0') + CommonLogic.FormCanBeDangerousContent("CardStartDateYear").Trim().Replace(" ", "");
                String CardIssueNumber = CommonLogic.FormCanBeDangerousContent("CardIssueNumber").Trim().Replace(" ", "");

                if (CardNumber.StartsWith("*"))
                {
                    // Still obscured in the form so use the original
                    CardNumber = BillingAddress.CardNumber;
                }

                if (CardExtraCode.StartsWith("*"))
                {
                    // Still obscured in the form so use the original
                    CardExtraCode = AppLogic.GetCardExtraCodeFromSession(ThisCustomer);
                }

                // store in appropriate session, encrypted, so it can be used when the order is actually "entered"
                AppLogic.StoreCardExtraCodeInSession(ThisCustomer, CardExtraCode);


                if (NetTotal == Decimal.Zero &&
                    AppLogic.AppConfigBool("SkipPaymentEntryOnZeroDollarCheckout"))
                {
                    // remember their info:
                    BillingAddress.PaymentMethodLastUsed = AppLogic.ro_PMCreditCard;
                    BillingAddress.ClearCCInfo();
                    BillingAddress.UpdateDB();
                }
                else
                {
                    if (CardNumber.Length == 0 || (!AppLogic.AppConfigBool("CardExtraCodeIsOptional") && CardExtraCode.Length == 0) || CardName.Length == 0 || CardExpirationMonth.Length == 0 ||
                        CardExpirationYear.Length == 0)
                    {
                        ErrorMsgLabel.Text = AppLogic.GetString("checkoutcard_process.aspx.1", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                        return;
                    }
                    // remember their info:
                    BillingAddress.PaymentMethodLastUsed = AppLogic.ro_PMCreditCard;
                    BillingAddress.CardName = CardName;
                    BillingAddress.CardType = CardType;
                    BillingAddress.CardNumber = CardNumber;
                    BillingAddress.CardExpirationMonth = CardExpirationMonth;
                    BillingAddress.CardExpirationYear = CardExpirationYear;
                    BillingAddress.CardStartDate = CommonLogic.IIF(CardStartDate == "00", String.Empty, CardStartDate);
                    BillingAddress.CardIssueNumber = CardIssueNumber;
                    BillingAddress.UpdateDB();
		

                }
            }
            else if (PM == AppLogic.ro_PMPurchaseOrder)
            {
                String PONumber = CommonLogic.FormCanBeDangerousContent("PONumber");
                if (PONumber.Length == 0)
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("checkoutpayment.aspx.21", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                    return;
                }

                // remember their info:
                BillingAddress.PaymentMethodLastUsed = AppLogic.ro_PMPurchaseOrder;
                BillingAddress.PONumber = PONumber;
                if (!ThisCustomer.MasterShouldWeStoreCreditCardInfo)
                {
                    BillingAddress.ClearCCInfo();
                }
                BillingAddress.UpdateDB();
            }
            else if (PM == AppLogic.ro_PMCODMoneyOrder)
            {
                String PONumber = CommonLogic.FormCanBeDangerousContent("PONumber");
                if (PONumber.Length == 0)
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("checkoutpayment.aspx.21", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                    return;
                }
                // remember their info:
                BillingAddress.PaymentMethodLastUsed = AppLogic.ro_PMCODMoneyOrder;
                BillingAddress.PONumber = PONumber;
                if (!ThisCustomer.MasterShouldWeStoreCreditCardInfo)
                {
                    BillingAddress.ClearCCInfo();
                }
                BillingAddress.UpdateDB();
            }
            else if (PM == AppLogic.ro_PMCODCompanyCheck)
            {
                String PONumber = CommonLogic.FormCanBeDangerousContent("PONumber");
                if (PONumber.Length == 0)
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("checkoutpayment.aspx.21", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                    return;
                }
                // remember their info:
                BillingAddress.PaymentMethodLastUsed = AppLogic.ro_PMCODCompanyCheck;
                BillingAddress.PONumber = PONumber;
                if (!ThisCustomer.MasterShouldWeStoreCreditCardInfo)
                {
                    BillingAddress.ClearCCInfo();
                }
                BillingAddress.UpdateDB();
            }
            else if (PM == AppLogic.ro_PMCODNet30)
            {
                String PONumber = CommonLogic.FormCanBeDangerousContent("PONumber");
                if (PONumber.Length == 0)
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("checkoutpayment.aspx.21", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                    return;
                }
                // remember their info:
                BillingAddress.PaymentMethodLastUsed = AppLogic.ro_PMCODNet30;
                BillingAddress.PONumber = PONumber;
                if (!ThisCustomer.MasterShouldWeStoreCreditCardInfo)
                {
                    BillingAddress.ClearCCInfo();
                }
                BillingAddress.UpdateDB();
            }
            else if (PM == AppLogic.ro_PMPayPal)
            {
                Response.Redirect("checkoutpayment.aspx?PaymentMethod=" + AppLogic.ro_PMPayPal);
            }
            else if (PM == AppLogic.ro_PMRequestQuote)
            {
                // no action required here
                BillingAddress.PaymentMethodLastUsed = AppLogic.ro_PMRequestQuote;
                if (!ThisCustomer.MasterShouldWeStoreCreditCardInfo)
                {
                    BillingAddress.ClearCCInfo();
                }
                BillingAddress.UpdateDB();
            }
            else if (PM == AppLogic.ro_PMCheckByMail)
            {
                // no action required here
                BillingAddress.PaymentMethodLastUsed = AppLogic.ro_PMCheckByMail;
                if (!ThisCustomer.MasterShouldWeStoreCreditCardInfo)
                {
                    BillingAddress.ClearCCInfo();
                }
                BillingAddress.UpdateDB();
            }
            else if (PM == AppLogic.ro_PMCOD)
            {
                // no action required here
                BillingAddress.PaymentMethodLastUsed = AppLogic.ro_PMCOD;
                if (!ThisCustomer.MasterShouldWeStoreCreditCardInfo)
                {
                    BillingAddress.ClearCCInfo();
                }
                BillingAddress.UpdateDB();
            }
            else if (PM == AppLogic.ro_PMECheck)
            {
                String ECheckBankName = CommonLogic.FormCanBeDangerousContent("ECheckBankName");
                String ECheckBankAccountNumber = CommonLogic.FormCanBeDangerousContent("ECheckBankAccountNumber");
                String ECheckBankAccountType = CommonLogic.FormCanBeDangerousContent("ECheckBankAccountType");
                String ECheckBankAccountName = CommonLogic.FormCanBeDangerousContent("ECheckBankAccountName");
                String ECheckBankABACode = CommonLogic.FormCanBeDangerousContent("ECheckBankABACode");
                if (ECheckBankName.Length == 0 || ECheckBankAccountNumber.Length == 0 || ECheckBankAccountType.Length == 0 || ECheckBankAccountName.Length == 0 ||
                    ECheckBankABACode.Length == 0)
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("checkoutcard_process.aspx.1", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                    return;
                }
                BillingAddress.PaymentMethodLastUsed = AppLogic.ro_PMECheck;
                BillingAddress.ECheckBankName = ECheckBankName;
                BillingAddress.ECheckBankAccountNumber = ECheckBankAccountNumber;
                BillingAddress.ECheckBankAccountType = ECheckBankAccountType;
                BillingAddress.ECheckBankAccountName = ECheckBankAccountName;
                BillingAddress.ECheckBankABACode = ECheckBankABACode;
                if (!ThisCustomer.MasterShouldWeStoreCreditCardInfo)
                {
                    BillingAddress.ClearCCInfo();
                }
                BillingAddress.UpdateDB();
            }
            else if (PM == AppLogic.ro_PMMicropay)
            {
                BillingAddress.PaymentMethodLastUsed = AppLogic.ro_PMMicropay;
                if (!ThisCustomer.MasterShouldWeStoreCreditCardInfo)
                {
                    BillingAddress.ClearCCInfo();
                }
                BillingAddress.UpdateDB();
            }
            else if (PM == AppLogic.ro_PMPayPalExpress ||
                     PM == AppLogic.ro_PMPayPalExpressMark)
            {
                BillingAddress.PaymentMethodLastUsed = PM;
                if (!ThisCustomer.MasterShouldWeStoreCreditCardInfo)
                {
                    BillingAddress.ClearCCInfo();
                }
                BillingAddress.UpdateDB();

                Address shippingAddress = new Address();
                shippingAddress.LoadByCustomer(ThisCustomer.CustomerID, ThisCustomer.PrimaryShippingAddressID, AddressTypes.Shipping);
                String sURL = Gateway.StartExpressCheckout(cart, shippingAddress);
                Response.Redirect(sURL);
            }


            if (AppLogic.AppConfigBool("Checkout.UseOnePageCheckout.UseFinalReviewOrderPage"))
            {
                Response.Redirect("checkoutreview.aspx?paymentmethod=" + Server.UrlEncode(PaymentMethod));
            }


            //Execute payment processing

            // ----------------------------------------------------------------
            // Process The Order:
            // ----------------------------------------------------------------

            if (PaymentMethod.Length == 0)
            {
                ErrorMsgLabel.Text = AppLogic.GetString("checkoutpayment.aspx.20", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                return;
            }
            if (PM == AppLogic.ro_PMCreditCard)
            {
                bool CardinalAllowed = AppLogic.AppConfigBool("CardinalCommerce.Centinel.Enabled") && !(cart.Total(true) == Decimal.Zero && AppLogic.AppConfigBool("SkipPaymentEntryOnZeroDollarCheckout"));
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
                            ErrorMsgLabel.Text = status;
                            return;
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
                    {
                        // If credit card is enrolled in a 3D Secure service (Verified by Visa, etc.)
                        Response.Redirect("secureform.aspx");
                    }
                    if (status != AppLogic.ro_OK)
                    {
                        ErrorMsgLabel.Text = status;
                        return;
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
                    ErrorMsgLabel.Text = status;
                    return;
                }
            }
            else if (PM == AppLogic.ro_PMCODMoneyOrder)
            {
                // try create the order record, check for status of TX though:
                OrderNumber = AppLogic.GetNextOrderNumber();
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                if (status != AppLogic.ro_OK)
                {
                    ErrorMsgLabel.Text = status;
                    return;
                }
            }
            else if (PM == "CODCOMPANYCHEC")
            {
                // try create the order record, check for status of TX though:
                OrderNumber = AppLogic.GetNextOrderNumber();
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                if (status != AppLogic.ro_OK)
                {
                    ErrorMsgLabel.Text = status;
                    return;
                }
            }
            else if (PM == AppLogic.ro_PMCODNet30)
            {
                // try create the order record, check for status of TX though:
                OrderNumber = AppLogic.GetNextOrderNumber();
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                if (status != AppLogic.ro_OK)
                {
                    ErrorMsgLabel.Text = status;
                    return;
                }
            }
            else if (PM == AppLogic.ro_PMPayPal)
            {
            }
            else if (PM == AppLogic.ro_PMPayPalExpress ||
                     PM == AppLogic.ro_PMPayPalExpressMark)
            {
                // will never make it this far due to redirect to PayPal.
            }
            else if (PM == AppLogic.ro_PMRequestQuote)
            {
                // try create the order record, check for status of TX though:
                OrderNumber = AppLogic.GetNextOrderNumber();
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                if (status != AppLogic.ro_OK)
                {
                    ErrorMsgLabel.Text = status;
                    return;
                }
            }
            else if (PM == AppLogic.ro_PMCheckByMail)
            {
                // try create the order record, check for status of TX though:
                OrderNumber = AppLogic.GetNextOrderNumber();
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                if (status != AppLogic.ro_OK)
                {
                    ErrorMsgLabel.Text = status;
                    return;
                }
            }
            else if (PM == AppLogic.ro_PMCOD)
            {
                // try create the order record, check for status of TX though:
                OrderNumber = AppLogic.GetNextOrderNumber();
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                if (status != AppLogic.ro_OK)
                {
                    ErrorMsgLabel.Text = status;
                    return;
                }
            }
            else if (PM == AppLogic.ro_PMECheck)
            {
                // try create the order record, check for status of TX though:
                OrderNumber = AppLogic.GetNextOrderNumber();
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                if (status != AppLogic.ro_OK)
                {
                    ErrorMsgLabel.Text = status;
                    return;
                }
            }
            else if (PM == AppLogic.ro_PMMicropay)
            {
                // try create the order record, check for status of TX though:
                OrderNumber = AppLogic.GetNextOrderNumber();
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);
                if (status != AppLogic.ro_OK)
                {
                    ErrorMsgLabel.Text = status;
                    return;
                }
            }

            Response.Redirect("orderconfirmation.aspx?ordernumber=" + OrderNumber.ToString() + "&paymentmethod=" + Server.UrlEncode(PaymentMethod));
        }

        private void ValidatepaymentInfo(string PM)
        {
            //switch (PM)
            //{
            //    case AppLogic.ro_PMCreditCard:
            //        break;
            //    case AppLogic.ro_PMPurchaseOrder:
            //        break;
            //    case AppLogic.ro_PMCODMoneyOrder:
            //        break;
            //    case AppLogic.ro_PMCODCompanyCheck:
            //        break;
            //    case AppLogic.ro_PMCODNet30:
            //        break;
            //    case AppLogic.ro_PMPayPal:
            //        break;
            //    case AppLogic.ro_PMRequestQuote:
            //        break;
            //    case AppLogic.ro_PMCheckByMail:
            //        break;
            //    case AppLogic.ro_PMCOD:
            //        break;
            //    case AppLogic.ro_PMECheck:
            //        break;
            //    case AppLogic.ro_PMMicropay:
            //        break;
            //    case AppLogic.ro_PMPayPalExpress:
            //        break;

            //}
        }


        public void pmtPAYPALEXPRESS_CheckedChanged(object sender, EventArgs e)
        {
            ResetPaymentPanes();
            ViewState["SelectedPaymentType"] = AppLogic.ro_PMPayPalExpress;
            if (pmtPAYPALEXPRESS.Checked)
            {
                pnlPayPalExpressPane.Visible = true;
                btnContinueCheckout.Text = AppLogic.GetString("checkoutpayment.aspx.19", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            }
            else
            {
                pnlPayPalExpressPane.Visible = false;
            }
        }

        public void pmtMICROPAY_CheckedChanged(object sender, EventArgs e)
        {
            ResetPaymentPanes();
            ViewState["SelectedPaymentType"] = AppLogic.ro_PMMicropay;
            if (pmtMICROPAY.Checked)
            {
                pnlMicroPayPane.Visible = true;
            }
            else
            {
                pnlMicroPayPane.Visible = false;
            }
        }

        public void pmtECHECK_CheckedChanged(object sender, EventArgs e)
        {
            ResetPaymentPanes();
            ViewState["SelectedPaymentType"] = AppLogic.ro_PMECheck;
            if (pmtECHECK.Checked)
            {
                pnlECheckPane.Visible = true;
            }
            else
            {
                pnlECheckPane.Visible = false;
            }
        }

        public void pmtCOD_CheckedChanged(object sender, EventArgs e)
        {
            ResetPaymentPanes();
            ViewState["SelectedPaymentType"] = AppLogic.ro_PMCOD;
            if (pmtCOD.Checked)
            {
                pnlCODPane.Visible = true;
            }
            else
            {
                pnlCODPane.Visible = false;
            }
        }

        public void pmtCHECKBYMAIL_CheckedChanged(object sender, EventArgs e)
        {
            ResetPaymentPanes();
            ViewState["SelectedPaymentType"] = AppLogic.ro_PMCheckByMail;
            if (pmtCHECKBYMAIL.Checked)
            {
                pnlCheckByMailPane.Visible = true;
            }
            else
            {
                pnlCheckByMailPane.Visible = false;
            }
        }

        public void pmtREQUESTQUOTE_CheckedChanged(object sender, EventArgs e)
        {
            ResetPaymentPanes();
            ViewState["SelectedPaymentType"] = AppLogic.ro_PMRequestQuote;
            if (pmtREQUESTQUOTE.Checked)
            {
                pnlReqQuotePane.Visible = true;
            }
            else
            {
                pnlReqQuotePane.Visible = false;
            }
        }

        public void pmtPAYPAL_CheckedChanged(object sender, EventArgs e)
        {
            ResetPaymentPanes();
            ViewState["SelectedPaymentType"] = AppLogic.ro_PMPayPal;
            if (pmtPAYPAL.Checked)
            {
                pnlPayPalPane.Visible = true;
                btnContinueCheckout.Text = AppLogic.GetString("checkoutpayment.aspx.19", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            }
            else
            {
                pnlPayPalPane.Visible = false;
            }
        }

        public void pmtCODNET30_CheckedChanged(object sender, EventArgs e)
        {
            ResetPaymentPanes();
            ViewState["SelectedPaymentType"] = AppLogic.ro_PMCODNet30;
            if (pmtCODNET30.Checked)
            {
                pnlCODNet30Pane.Visible = true;
            }
            else
            {
                pnlCODNet30Pane.Visible = false;
            }
        }

        public void pmtCODCOMPANYCHECK_CheckedChanged(object sender, EventArgs e)
        {
            ResetPaymentPanes();
            ViewState["SelectedPaymentType"] = AppLogic.ro_PMCODCompanyCheck;
            if (pmtCODCOMPANYCHECK.Checked)
            {
                pnlCODCoCheckPane.Visible = true;
            }
            else
            {
                pnlCODCoCheckPane.Visible = false;
            }
        }

        public void pmtCODMONEYORDER_CheckedChanged(object sender, EventArgs e)
        {
            ResetPaymentPanes();
            ViewState["SelectedPaymentType"] = AppLogic.ro_PMCODMoneyOrder;
            if (pmtCODMONEYORDER.Checked)
            {
                pnlCODMOPane.Visible = true;
            }
            else
            {
                pnlCODMOPane.Visible = false;
            }
        }

        public void pmtPURCHASEORDER_CheckedChanged(object sender, EventArgs e)
        {
            ResetPaymentPanes();
            ViewState["SelectedPaymentType"] = AppLogic.ro_PMPurchaseOrder;
            if (pmtPURCHASEORDER.Checked)
            {
                pnlPOPane.Visible = true;
            }
            else
            {
                pnlPOPane.Visible = false;
            }
        }

        public void pmtCreditCard_CheckedChanged(object sender, EventArgs e)
        {
            ResetPaymentPanes();
            ViewState["SelectedPaymentType"] = AppLogic.ro_PMCreditCard;
            if (pmtCreditCard.Checked)
            {
                pnlCreditCardPane.Visible = true;
            }
            else
            {
                pnlCreditCardPane.Visible = false;
            }
        }


        private void WritePaymentPanels(string OrderFinalizationInstructions, string TransactionMode)
        {
            Address BillingAddress = new Address();
            BillingAddress.LoadByCustomer(ThisCustomer.CustomerID, ThisCustomer.PrimaryBillingAddressID, AddressTypes.Billing);
            bool EChecksAllowed = ((GW == Gateway.ro_GWAUTHORIZENET || GW == Gateway.ro_GWEPROCESSINGNETWORK || GW == Gateway.ro_GWITRANSACT || GW == Gateway.ro_GWMANUAL) && AppLogic.TransactionModeIsAuthCapture()); // let manual gw use echecks so site testing can occur
            bool POAllowed = AppLogic.CustomerLevelAllowsPO(ThisCustomer.CustomerLevelID);
            bool CODCompanyCheckAllowed = ThisCustomer.CODCompanyCheckAllowed;
            bool CODNet30Allowed = ThisCustomer.CODNet30Allowed;


            foreach (String PM in AllowedPaymentMethods.Split(','))
            {
                String PMCleaned = AppLogic.CleanPaymentMethod(PM);
                if (PMCleaned == AppLogic.ro_PMCreditCard && GW != Gateway.ro_GW2CHECKOUT && GW != Gateway.ro_GWWORLDPAYJUNIOR &&
                    GW != Gateway.ro_GWWORLDPAY)
                {
                    pmtCreditCard.Visible = true;
                    pmtCreditCard.Text = AppLogic.GetString("checkoutpayment.aspx.7", SkinID, ThisCustomer.LocaleSetting) + "&nbsp;";
                    if (SelectedPaymentType == AppLogic.ro_PMCreditCard ||
                        SelectedPaymentType == String.Empty)
                    {
                        pnlCreditCardPane.Visible = true;
                        pmtCreditCard.Checked = true;
                    }
                    CCIMage.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/creditcards.jpg");
                    CCIMage.Visible = true;
                    CCForm.Text = WriteCCPane(OrderFinalizationInstructions, BillingAddress, RequireTerms, PM);
                }
                else if (PMCleaned == AppLogic.ro_PMPurchaseOrder)
                {
                    if (POAllowed)
                    {
                        pmtPURCHASEORDER.Visible = true;
                        if (SelectedPaymentType == AppLogic.ro_PMPurchaseOrder)
                        {
                            pnlPOPane.Visible = true;
                            pmtPURCHASEORDER.Checked = true;
                        }
                        POForm.Text = WritePURCHASEORDERPane(OrderFinalizationInstructions, BillingAddress, RequireTerms, PM);
                    }
                }
                else if (PMCleaned == AppLogic.ro_PMCODMoneyOrder)
                {
                    if (POAllowed)
                    {
                        pmtCODMONEYORDER.Visible = true;
                        if (SelectedPaymentType == AppLogic.ro_PMCODMoneyOrder)
                        {
                            pnlCODMOPane.Visible = true;
                            pmtCODMONEYORDER.Checked = true;
                        }
                        CODMOForm.Text = WriteCODMONEYORDERPane(OrderFinalizationInstructions, BillingAddress, RequireTerms, PM);
                    }
                }
                else if (PMCleaned == AppLogic.ro_PMCODCompanyCheck)
                {
                    if (CODCompanyCheckAllowed)
                    {
                        pmtCODCOMPANYCHECK.Visible = true;
                        if (SelectedPaymentType == AppLogic.ro_PMCODCompanyCheck)
                        {
                            pnlCODCoCheckPane.Visible = true;
                            pmtCODCOMPANYCHECK.Checked = true;
                        }
                        CODCoCheckForm.Text = WriteCODCOMPANYCHECKPane(OrderFinalizationInstructions, BillingAddress, RequireTerms, PM);
                    }
                }
                else if (PMCleaned == AppLogic.ro_PMCODNet30)
                {
                    if (CODNet30Allowed)
                    {
                        pmtCODNET30.Visible = true;
                        if (SelectedPaymentType == AppLogic.ro_PMCODNet30)
                        {
                            pnlCODNet30Pane.Visible = true;
                            pmtCODNET30.Checked = true;
                        }
                        CODNet30Form.Text = WriteCODNET30Pane(OrderFinalizationInstructions, BillingAddress, RequireTerms, PM);
                    }
                }
                else if (PMCleaned == AppLogic.ro_PMPayPal)
                {
                    pmtPAYPAL.Visible = true;
                    pmtPAYPAL.Text = AppLogic.GetString("checkoutpayment.aspx.9", SkinID, ThisCustomer.LocaleSetting) + "&nbsp;";
                    if (SelectedPaymentType == AppLogic.ro_PMPayPal)
                    {
                        pmtPAYPAL.Checked = true;
                    }
                    PayPalImage.ImageUrl = AppLogic.AppConfig("PayPal.PaymentIcon");
                    PayPalImage.Visible = true;
                    PayPalForm.Text = WritePayPalPane(OrderFinalizationInstructions, BillingAddress, RequireTerms, PM);
                }
                else if (PMCleaned == AppLogic.ro_PMPayPalExpressMark)
                {
                    pmtPAYPALEXPRESS.Visible = true;
                    pmtPAYPALEXPRESS.Text = AppLogic.GetString("checkoutpayment.aspx.9", SkinID, ThisCustomer.LocaleSetting) + "&nbsp;";
                    if (SelectedPaymentType == AppLogic.ro_PMPayPalExpressMark)
                    {
                        pnlPayPalPane.Visible = true;
                        pmtPAYPAL.Checked = true;
                    }
                    PayPalExpressImage.ImageUrl = AppLogic.AppConfig("PayPal.PaymentIcon");
                    PayPalExpressImage.Visible = true;
                    PayPalExpressForm.Text = WritePAYPALEXPRESSPane(OrderFinalizationInstructions, BillingAddress, RequireTerms, PM);
                }
                else if (PMCleaned == AppLogic.ro_PMRequestQuote)
                {
                    pmtREQUESTQUOTE.Visible = true;
                    if (SelectedPaymentType == AppLogic.ro_PMRequestQuote)
                    {
                        pnlReqQuotePane.Visible = true;
                        pmtREQUESTQUOTE.Checked = true;
                    }
                    ReqQuoteForm.Text = WriteREQUESTQUOTEPane(OrderFinalizationInstructions, BillingAddress, RequireTerms, PM);
                }
                else if (PMCleaned == AppLogic.ro_PMCheckByMail)
                {
                    pmtCHECKBYMAIL.Visible = true;
                    if (SelectedPaymentType == AppLogic.ro_PMCheckByMail)
                    {
                        pnlCheckByMailPane.Visible = true;
                        pmtCHECKBYMAIL.Checked = true;
                    }
                    CheckByMailForm.Text = WriteCHECKBYMAILPane(OrderFinalizationInstructions, BillingAddress, RequireTerms, PM);
                }
                else if (PMCleaned == AppLogic.ro_PMCOD)
                {
                    pmtCOD.Visible = true;
                    if (SelectedPaymentType == AppLogic.ro_PMCOD)
                    {
                        pnlCODPane.Visible = true;
                        pmtCOD.Checked = true;
                    }
                    CODForm.Text = WriteCODPane(OrderFinalizationInstructions, BillingAddress, RequireTerms, PM);
                }
                else if (PMCleaned == AppLogic.ro_PMECheck)
                {
                    if (EChecksAllowed)
                    {
                        pmtECHECK.Visible = true;
                        if (SelectedPaymentType == AppLogic.ro_PMECheck)
                        {
                            pnlECheckPane.Visible = true;
                            pmtECHECK.Checked = true;
                        }
                        ECheckForm.Text = WriteECHECKPane(OrderFinalizationInstructions, BillingAddress, RequireTerms, PM);
                    }
                }
                else if (PMCleaned == AppLogic.ro_PMMicropay)
                {
                    if (AppLogic.MicropayIsEnabled())
                    {
                        pmtMICROPAY.Visible = true;
                        if (SelectedPaymentType == AppLogic.ro_PMMicropay)
                        {
                            pnlMicroPayPane.Visible = true;
                            pmtMICROPAY.Checked = true;
                        }
                        MicroPayForm.Text = WriteMICROPAYPane(OrderFinalizationInstructions, BillingAddress, RequireTerms, PM);
                    }
                }
                else if (PMCleaned == AppLogic.ro_PMPayPalExpress)
                {
                    // nothing required
                }
            }
        }

        // non-real time gateway payment gateways cannot support finalization instructions (e.g. two checkout, worldpay, etc!)
        private string WriteCCPane(String OrderFinalizationInstructions, Address BillingAddress, bool RequireTerms, string PM)
        {
            StringBuilder s = new StringBuilder("");

            if (GW == Gateway.ro_GW2CHECKOUT)
            {
                s.Append("<font color=blue size=2><b>" + AppLogic.GetString("checkouttwocheckout.aspx.2", SkinID, ThisCustomer.LocaleSetting) + "</b></font><br/><br/>");
                s.Append("<div align=\"center\">\n");
                //s.Append("<form id=\"TwoCheckoutForm\" name=\"TwoCheckoutForm\" target=\"_top\" action=\"" + AppLogic.AppConfig("2CHECKOUT_Live_Server") + "\" method=\"post\" onsubmit=\"return (validateForm(this) && TwoCheckoutForm_Validator(this))\">\n");

                s.Append("<input type=\"hidden\" name=\"x_login\" value=\"" + AppLogic.AppConfig("2CHECKOUT_VendorID") + "\">\n");
                s.Append("<input type=\"hidden\" name=\"x_amount\" value=\"" + Localization.CurrencyStringForGatewayWithoutExchangeRate((NetTotal)) + "\">\n");
                s.Append("<input type=\"hidden\" name=\"x_invoice_num\" value=\"" + CommonLogic.GetNewGUID() + "\">\n"); // yuk...we don't know what the order nubmer will be...
                s.Append("<input type=\"hidden\" name=\"x_receipt_link_url\" value=\"" + AppLogic.GetStoreHTTPLocation(true) + "twocheckout_return.aspx\">\n");
                s.Append("<input type=\"hidden\" name=\"x_return_url\" value=\"" + AppLogic.GetStoreHTTPLocation(true) + "twocheckout_return.aspx\">\n");
                s.Append("<input type=\"hidden\" name=\"x_return\" value=\"" + AppLogic.GetStoreHTTPLocation(true) + "twocheckout_return.aspx\">\n");
                if (!useLiveTransactions)
                {
                    s.Append("<input type=\"hidden\" name=\"demo\" value=\"Y\">\n");
                }
                s.Append("<input type=\"hidden\" name=\"x_First_Name\" value=\"" + BillingAddress.FirstName + "\">\n");
                s.Append("<input type=\"hidden\" name=\"x_Last_Name\" value=\"" + BillingAddress.LastName + "\">\n");
                s.Append("<input type=\"hidden\" name=\"x_Address\" value=\"" + BillingAddress.Address1 + "\">\n");
                s.Append("<input type=\"hidden\" name=\"x_City\" value=\"" + BillingAddress.City + "\">\n"); // 2checkout docs are unclear as to the name of this parm
                s.Append("<input type=\"hidden\" name=\"x_State\" value=\"" + BillingAddress.State + "\">\n");
                s.Append("<input type=\"hidden\" name=\"x_Zip\" value=\"" + BillingAddress.Zip + "\">\n");
                s.Append("<input type=\"hidden\" name=\"x_Country\" value=\"" + BillingAddress.Country + "\">\n");
                s.Append("<input type=\"hidden\" name=\"x_EMail\" value=\"" + BillingAddress.EMail + "\">\n");
                s.Append("<input type=\"hidden\" name=\"x_Phone\" value=\"" + BillingAddress.Phone + "\">\n");

                s.Append("<input type=\"hidden\" name=\"city\" value=\"" + BillingAddress.City + "\">\n");
                s.Append("<p align=\"center\">");
                //s.Append("<input type=\"submit\" class=\"PaymentPageContinueCheckoutButton\" value=\"" + AppLogic.GetString("checkoutpayment.aspx.16", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "\">");
                s.Append("</p>");
                //s.Append("</form>\n");
                s.Append("</div>\n");
            }
            else if (GW == Gateway.ro_GWWORLDPAYJUNIOR ||
                     GW == Gateway.ro_GWWORLDPAY)
            {
                s.Append("<br/>" + AppLogic.GetString("checkoutworldpay.aspx.3", SkinID, ThisCustomer.LocaleSetting) + "<br/><br/>\n");
                //s.Append("<form action=\"" + AppLogic.AppConfig("WorldPay_Live_Server") + "\" method=\"post\" name=\"WorldPayForm\" id=\"WorldPayForm\" onsubmit=\"return (validateForm(this) && WorldPayForm_Validator(this))\">\n");

                s.Append("<input type=\"hidden\" name=\"instId\" value=\"" + AppLogic.AppConfig("WorldPay_InstallationID") + "\">\n");
                s.Append("<input type=\"hidden\" name=\"cartId\" value=\"" + cart.ThisCustomer.CustomerID.ToString() + "\">\n");
                s.Append("<input type=\"hidden\" name=\"amount\" value=\"" + Localization.CurrencyStringForGatewayWithoutExchangeRate((NetTotal)) + "\">\n");
                s.Append("<input type=\"hidden\" name=\"currency\" value=\"" + Localization.StoreCurrency() + "\">\n");
                s.Append("<input type=\"hidden\" name=\"des\" value=\"" + AppLogic.AppConfig("StoreName") + " Purchase\">\n");
                s.Append("<input type=\"hidden\" name=\"description\" value=\"" + AppLogic.AppConfig("StoreName") + " Purchase\">\n");
                s.Append("<input type=\"hidden\" name=\"MC_callback\" value=\"" + AppLogic.GetStoreHTTPLocation(true) + AppLogic.AppConfig("WorldPay_ReturnURL") + "\">\n");
                s.Append("<input type=\"hidden\" name=\"authMode\" value=\"" + CommonLogic.IIF(AppLogic.TransactionModeIsAuthOnly(), "E", "A") + "\">\n");

                s.Append("<input type=\"hidden\" name=\"name\" value=\"" + (BillingAddress.FirstName + " " + BillingAddress.LastName) + "\">\n");
                s.Append("<input type=\"hidden\" name=\"address\" value=\"" + BillingAddress.Address1 + "\">\n");
                s.Append("<input type=\"hidden\" name=\"postcode\" value=\"" + BillingAddress.Zip + "\">\n");
                s.Append("<input type=\"hidden\" name=\"country\" value=\"" + AppLogic.GetCountryTwoLetterISOCode(BillingAddress.Country) + "\">\n");
                s.Append("<input type=\"hidden\" name=\"tel\" value=\"" + BillingAddress.Phone + "\">\n");
                s.Append("<input type=\"hidden\" name=\"email\" value=\"" + BillingAddress.EMail + "\">\n");
                s.Append("<input type=\"hidden\" name=\"lang\" value=\"" + AppLogic.AppConfig("WorldPay_LanguageLocale") + "\">\n");

                if (AppLogic.AppConfigBool("WorldPay_FixContact"))
                {
                    s.Append("<input type=\"hidden\" name=\"fixContact\" value=\"true\">\n");
                }

                if (AppLogic.AppConfigBool("WorldPay_HideContact"))
                {
                    s.Append("<input type=\"hidden\" name=\"hideContact\" value=\"true\">\n");
                }

                if (AppLogic.AppConfigBool("WorldPay_TestMode"))
                {
                    s.Append("<input type=\"hidden\" name=\"testMode\" value=\"" + AppLogic.AppConfig("WorldPay_TestModeCode") + "\">\n");
                }
                s.Append("<p align=\"center\">");
                //s.Append("<input type=\"submit\" class=\"PaymentPageContinueCheckoutButton\" value=\"" + AppLogic.GetString("checkoutpayment.aspx.17", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "\">");
                s.Append("</p>");
                s.Append("<input type=\"hidden\" name=\"paymentmethod\" value=\"" + AppLogic.ro_PMCreditCard + "\">\n");
                //s.Append("</form>\n");
            }
            else
            {
                //s.Append("<form id=\"CreditCardForm\" name=\"CreditCardForm\" method=\"POST\" action=\"checkoutpayment.aspx?paymentmethod=" + Server.UrlEncode(PM) + "\" onsubmit=\"return (validateForm(this) && CreditCardForm_Validator(this))\">");
                s.Append(OrderFinalizationInstructions);
                s.Append("<input type=\"hidden\" name=\"paymentmethod\" value=\"" + AppLogic.ro_PMCreditCard + "\">\n");
                s.Append("<p><b>" + AppLogic.GetString("checkoutcard.aspx.6", SkinID, ThisCustomer.LocaleSetting) + "</b></p>");
                s.Append(BillingAddress.InputCardHTML(ThisCustomer, true, RequireTerms));
                s.Append("<br/>");
                s.Append("<p align=\"center\">");
                //s.Append("<input type=\"submit\" class=\"PaymentPageContinueCheckoutButton\" value=\"" + AppLogic.GetString("checkoutpayment.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "\">");
                s.Append("</p>");
                //s.Append("</form>");
            }
            return s.ToString();
        }

        private string WriteECHECKPane(String OrderFinalizationInstructions, Address BillingAddress, bool RequireTerms, string PM)
        {
            StringBuilder s = new StringBuilder("");
            //s.Append("<form id=\"ECheckForm\" name=\"ECheckForm\" method=\"POST\" action=\"checkoutpayment.aspx?paymentmethod=" + Server.UrlEncode(PM) + "\" onsubmit=\"return (validateForm(this) && ECheckForm_Validator(this))\">");
            s.Append(OrderFinalizationInstructions);
            s.Append("<input type=\"hidden\" name=\"paymentmethod\" value=\"" + AppLogic.ro_PMECheck + "\">\n");
            s.Append(BillingAddress.InputECheckHTML(true));
            s.Append("<br/>");
            s.Append("<p align=\"center\">");
            //s.Append("<input type=\"submit\" class=\"PaymentPageContinueCheckoutButton\" value=\"" + AppLogic.GetString("checkoutpayment.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "\">");
            s.Append("</p>");
            //s.Append("</form>");
            return s.ToString();
        }

        private string WritePURCHASEORDERPane(String OrderFinalizationInstructions, Address BillingAddress, bool RequireTerms, string PM)
        {
            StringBuilder s = new StringBuilder("");

            //s.Append("<form id=\"POForm\" name=\"POForm\" method=\"POST\" action=\"checkoutpayment.aspx?paymentmethod=" + Server.UrlEncode(PM) + "\" onsubmit=\"return (validateForm(this) && POFormForm_Validator(this))\">");
            s.Append(OrderFinalizationInstructions);
            s.Append("<input type=\"hidden\" name=\"paymentmethod\" value=\"" + AppLogic.ro_PMPurchaseOrder + "\">\n");
            s.Append("<b>" + AppLogic.GetString("checkoutpo.aspx.3", SkinID, ThisCustomer.LocaleSetting) + "</b><br/><br/>");
            s.Append(AppLogic.GetString("checkoutpo.aspx.4", SkinID, ThisCustomer.LocaleSetting));
            s.Append("<input type=\"text\" name=\"PONumber\" size=\"20\" maxlength=\"50\">\n");
            s.Append("<input type=\"hidden\" name=\"PONumber_vldt\" value=\"[req][blankalert=" + AppLogic.GetString("checkoutpo.aspx.5", SkinID, ThisCustomer.LocaleSetting) + "]\">");
            s.Append("<br/>");
            s.Append("<br/>");
            s.Append("<p align=\"center\">");
            //s.Append("<input type=\"submit\" class=\"PaymentPageContinueCheckoutButton\" value=\"" + AppLogic.GetString("checkoutpayment.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "\">");
            s.Append("</p>");
            //s.Append("</form>");

            return s.ToString();
        }

        private string WriteCODMONEYORDERPane(String OrderFinalizationInstructions, Address BillingAddress, bool RequireTerms, string PM)
        {
            StringBuilder s = new StringBuilder("");

            //s.Append("<form id=\"CODMoneyOrderForm\" name=\"CODMoneyOrderForm\" method=\"POST\" action=\"checkoutpayment.aspx?paymentmethod=" + Server.UrlEncode(PM) + "\" onsubmit=\"return (validateForm(this) && CODMoneyOrderForm_Validator(this))\">");
            s.Append(OrderFinalizationInstructions);
            s.Append("<input type=\"hidden\" name=\"paymentmethod\" value=\"" + AppLogic.ro_PMCODMoneyOrder + "\">\n");
            s.Append("<b>" + AppLogic.GetString("checkoutpo.aspx.3", SkinID, ThisCustomer.LocaleSetting) + "</b><br/><br/>");
            s.Append(AppLogic.GetString("checkoutpo.aspx.4", SkinID, ThisCustomer.LocaleSetting));
            s.Append("<input type=\"text\" name=\"PONumber\" size=\"20\" maxlength=\"50\">\n");
            s.Append("<input type=\"hidden\" name=\"PONumber_vldt\" value=\"[req][blankalert=" + AppLogic.GetString("checkoutpo.aspx.5", SkinID, ThisCustomer.LocaleSetting) + "]\">");
            s.Append("<br/>");
            s.Append("<p align=\"center\">");
            //s.Append("<input type=\"submit\" class=\"PaymentPageContinueCheckoutButton\" value=\"" + AppLogic.GetString("checkoutpayment.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "\">");
            s.Append("</p>");
            //s.Append("</form>");

            return s.ToString();
        }

        private string WriteCODCOMPANYCHECKPane(String OrderFinalizationInstructions, Address BillingAddress, bool RequireTerms, string PM)
        {
            StringBuilder s = new StringBuilder("");

            //s.Append("<form id=\"CODCompanyCheckForm\" name=\"CODCompanyCheckForm\" method=\"POST\" action=\"checkoutpayment.aspx?paymentmethod=" + Server.UrlEncode(PM) + "\" onsubmit=\"return (validateForm(this) && CODCompanyCheckForm_Validator(this))\">");
            s.Append(OrderFinalizationInstructions);
            s.Append("<input type=\"hidden\" name=\"paymentmethod\" value=\"" + AppLogic.ro_PMCODCompanyCheck + "\">\n");
            s.Append("<b>" + AppLogic.GetString("checkoutpo.aspx.3", SkinID, ThisCustomer.LocaleSetting) + "</b><br/><br/>");
            s.Append(AppLogic.GetString("checkoutpo.aspx.4", SkinID, ThisCustomer.LocaleSetting));
            s.Append("<input type=\"text\" name=\"PONumber\" size=\"20\" maxlength=\"50\">\n");
            s.Append("<input type=\"hidden\" name=\"PONumber_vldt\" value=\"[req][blankalert=" + AppLogic.GetString("checkoutpo.aspx.5", SkinID, ThisCustomer.LocaleSetting) + "]\">");
            s.Append("<br/>");
            s.Append("<p align=\"center\">");
            //s.Append("<input type=\"submit\" class=\"PaymentPageContinueCheckoutButton\" value=\"" + AppLogic.GetString("checkoutpayment.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "\">");
            s.Append("</p>");
            //s.Append("</form>");

            return s.ToString();
        }

        private string WriteCODNET30Pane(String OrderFinalizationInstructions, Address BillingAddress, bool RequireTerms, string PM)
        {
            StringBuilder s = new StringBuilder("");

            //s.Append("<form id=\"CODNet30Form\" name=\"CODNet30Form\" method=\"POST\" action=\"checkoutpayment.aspx?paymentmethod=" + Server.UrlEncode(PM) + "\" onsubmit=\"return (validateForm(this) && CODNet30Form_Validator(this))\">");
            s.Append(OrderFinalizationInstructions);
            s.Append("<input type=\"hidden\" name=\"paymentmethod\" value=\"" + AppLogic.ro_PMCODNet30 + "\">\n");
            s.Append("<b>" + AppLogic.GetString("checkoutpo.aspx.3", SkinID, ThisCustomer.LocaleSetting) + "</b><br/><br/>");
            s.Append(AppLogic.GetString("checkoutpo.aspx.4", SkinID, ThisCustomer.LocaleSetting));
            s.Append("<input type=\"text\" name=\"PONumber\" size=\"20\" maxlength=\"50\">\n");
            s.Append("<input type=\"hidden\" name=\"PONumber_vldt\" value=\"[req][blankalert=" + AppLogic.GetString("checkoutpo.aspx.5", SkinID, ThisCustomer.LocaleSetting) + "]\">");
            s.Append("<br/>");
            s.Append("<p align=\"center\">");
            //s.Append("<input type=\"submit\" class=\"PaymentPageContinueCheckoutButton\" value=\"" + AppLogic.GetString("checkoutpayment.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "\">");
            s.Append("</p>");
            //s.Append("</form>");

            return s.ToString();
        }

        // note, this payment method cannot support finalization instructions!
        private string WritePayPalPane(String OrderFinalizationInstructions, Address BillingAddress, bool RequireTerms, string PM)
        {
            StringBuilder s = new StringBuilder("");
            s.Append("<input type=\"hidden\" name=\"paymentmethod\" value=\"" + AppLogic.ro_PMPayPal + "\">\n");
            return s.ToString();
        }

        private string WriteREQUESTQUOTEPane(String OrderFinalizationInstructions, Address BillingAddress, bool RequireTerms, string PM)
        {
            StringBuilder s = new StringBuilder("");

            //s.Append("<form id=\"RequestQuoteForm\" name=\"RequestQuoteForm\" method=\"POST\" action=\"checkoutpayment.aspx?paymentmethod=" + Server.UrlEncode(PM) + "\" onsubmit=\"return (validateForm(this) && RequestQuoteForm_Validator(this))\">");
            s.Append(OrderFinalizationInstructions);
            s.Append("<input type=\"hidden\" name=\"paymentmethod\" value=\"" + AppLogic.ro_PMRequestQuote + "\">\n");
            s.Append("<p align=\"center\">");
            //s.Append("<input type=\"submit\" class=\"PaymentPageContinueCheckoutButton\" value=\"" + AppLogic.GetString("checkoutpayment.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "\">");
            s.Append("</p>");
            //s.Append("</form>");

            return s.ToString();
        }

        private string WriteCHECKBYMAILPane(String OrderFinalizationInstructions, Address BillingAddress, bool RequireTerms, string PM)
        {
            StringBuilder s = new StringBuilder("");

            //s.Append("<form id=\"CheckByMailForm\" name=\"CheckByMailForm\" method=\"POST\" action=\"checkoutpayment.aspx?paymentmethod=" + Server.UrlEncode(PM) + "\" onsubmit=\"return (validateForm(this) && CheckByMailForm_Validator(this))\">");
            //Topic t2x = new Topic("checkinstructions",ThisCustomer.LocaleSetting,SkinID,base.GetParser);
            //s.Append(t2x.Contents);
            s.Append(OrderFinalizationInstructions);
            s.Append("<input type=\"hidden\" name=\"paymentmethod\" value=\"" + AppLogic.ro_PMCheckByMail + "\">\n");
            s.Append("<p align=\"center\">");
            //s.Append("<input type=\"submit\" class=\"PaymentPageContinueCheckoutButton\" value=\"" + AppLogic.GetString("checkoutpayment.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "\">");
            s.Append("</p>");
            //s.Append("</form>");

            return s.ToString();
        }

        private string WriteCODPane(String OrderFinalizationInstructions, Address BillingAddress, bool RequireTerms, string PM)
        {
            StringBuilder s = new StringBuilder("");

            //s.Append("<form id=\"CODForm\" name=\"CODForm\" method=\"POST\" action=\"checkoutpayment.aspx?paymentmethod=" + Server.UrlEncode(PM) + "\" onsubmit=\"return (validateForm(this) && CODForm_Validator(this))\">");
            s.Append(OrderFinalizationInstructions);
            s.Append("<input type=\"hidden\" name=\"paymentmethod\" value=\"" + AppLogic.ro_PMCOD + "\">\n");
            s.Append("<p align=\"center\">");
            //s.Append("<input type=\"submit\" class=\"PaymentPageContinueCheckoutButton\" value=\"" + AppLogic.GetString("checkoutpayment.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "\">");
            s.Append("</p>");
            //s.Append("</form>");

            return s.ToString();
        }

        private string WriteMICROPAYPane(String OrderFinalizationInstructions, Address BillingAddress, bool RequireTerms, string PM)
        {
            StringBuilder s = new StringBuilder("");

            if (ThisCustomer.MicroPayBalance >= NetTotal)
            {
                //s.Append("<form id=\"MicropayForm\" name=\"MicropayForm\" method=\"POST\" action=\"checkoutpayment.aspx?paymentmethod=" + Server.UrlEncode(PM) + "\" onsubmit=\"return (validateForm(this) && MicropayForm_Validator(this))\">");
                s.Append(OrderFinalizationInstructions);
                s.Append("<input type=\"hidden\" name=\"paymentmethod\" value=\"" + AppLogic.ro_PMMicropay + "\">\n");
                s.Append("<p align=\"center\">");
                //s.Append("<input type=\"submit\" class=\"PaymentPageContinueCheckoutButton\" value=\"" + AppLogic.GetString("checkoutpayment.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "\">");
                s.Append("</p>");
                //s.Append("</form>");
            }
            else
            {
                s.Append(String.Format(AppLogic.GetString("checkoutpayment.aspx.26", SkinID, ThisCustomer.LocaleSetting), ThisCustomer.CurrencyString(ThisCustomer.MicroPayBalance)));
            }

            return s.ToString();
        }

        private string WritePAYPALEXPRESSPane(String OrderFinalizationInstructions, Address BillingAddress, bool RequireTerms, string PM)
        {
            StringBuilder s = new StringBuilder("");

            s.Append("<p align=\"center\">" + AppLogic.GetString("checkoutpaypal.aspx.2", SkinID, ThisCustomer.LocaleSetting) + "</p><br/>");
            s.Append(OrderFinalizationInstructions);
            s.Append("<input type=\"hidden\" name=\"paymentmethod\" value=\"" + AppLogic.ro_PMPayPalExpressMark + "\">\n");
            s.Append("<p align=\"center\">");
            s.Append("</p>");

            return s.ToString();
        }

        private void ResetPaymentPanes()
        {
            SelectedPaymentType = ViewState["SelectedPaymentType"].ToString();
            SetPasswordFields();
            String ShippingMethodIDFormField = CommonLogic.FormCanBeDangerousContent("ShippingMethodID").Replace(",", ""); // remember to remove the stupid hidden field which adds a comma to the form post (stupid stupid javascript again)
            int ShippingMethodID = 0;
            String ShippingMethod = String.Empty;
            if (cart.ShipCalcID !=
                Shipping.ShippingCalculationEnum.UseRealTimeRates)
            {
                ShippingMethodID = Localization.ParseUSInt(ShippingMethodIDFormField);
                ShippingMethod = Shipping.GetShippingMethodName(ShippingMethodID, null);
            }
            else
            {
                if (ShippingMethodIDFormField.Length != 0 &&
                    ShippingMethodIDFormField.IndexOf('|') != -1)
                {
                    String[] frmsplit = ShippingMethodIDFormField.Split('|');
                    ShippingMethodID = Localization.ParseUSInt(frmsplit[0]);
                    ShippingMethod = String.Format("{0}|{1}", frmsplit[1], frmsplit[2]);
                }
            }
            String sql = String.Format("update ShoppingCart set ShippingMethodID={0}, ShippingMethod={1} where CustomerID={2} and CartType={3}", ShippingMethodID.ToString(), DB.SQuote(ShippingMethod), ThisCustomer.CustomerID.ToString(), ((int) CartTypeEnum.ShoppingCart).ToString());
            DB.ExecuteSQL(sql);
            cart = new ShoppingCart(SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);
            InitializeShippingOptions(cart);

            pnlCreditCardPane.Visible = false;
            pnlPOPane.Visible = false;
            pnlCODMOPane.Visible = false;
            pnlCODCoCheckPane.Visible = false;
            pnlCODNet30Pane.Visible = false;
            pnlPayPalPane.Visible = false;
            pnlReqQuotePane.Visible = false;
            pnlCheckByMailPane.Visible = false;
            pnlCODPane.Visible = false;
            pnlECheckPane.Visible = false;
            pnlMicroPayPane.Visible = false;
            pnlPayPalExpressPane.Visible = false;

            btnContinueCheckout.Text = AppLogic.GetString("checkout1.aspx.1", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
        }

        #endregion
    }
}