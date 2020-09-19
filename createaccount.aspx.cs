// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2004.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/createaccount.aspx.cs 43    10/03/06 7:56p Redwoodtree $
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

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for createaccount.
    /// </summary>
    public partial class createaccount : SkinBase
    {

        bool Checkout = false;
        bool SkipRegistration = false;
        bool RequireSecurityCode = false;
        bool AllowShipToDifferentThanBillTo = false;
        bool VerifyAddressPrompt = false;
        String VerifyResult = String.Empty;
        String ReturnURL = String.Empty;
        Address BillingAddress = new AspDotNetStorefrontCommon.Address(); // qualification needed for vb.net (not sure why)
        Address ShippingAddress = new AspDotNetStorefrontCommon.Address(); // qualification needed for vb.net (not sure why)
        Address StandardizedAddress = new AspDotNetStorefrontCommon.Address();

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            RequireSecurePage();
            ThisCustomer.RequireCustomerRecord();

            SectionTitle = AppLogic.GetString("createaccount.aspx.1", SkinID, ThisCustomer.LocaleSetting);
            Checkout = CommonLogic.QueryStringBool("checkout");
            SkipRegistration = CommonLogic.QueryStringBool("skipreg");

            if (!Checkout)
            {
                RequireSecurityCode = AppLogic.AppConfigBool("SecurityCodeRequiredOnCreateAccount");
            }
            else
            {
                RequireSecurityCode = AppLogic.AppConfigBool("SecurityCodeRequiredOnCreateAccountDuringCheckout");
            }

            if (Checkout)
            {
                SectionTitle = AppLogic.GetString("createaccount.aspx.2", SkinID, ThisCustomer.LocaleSetting) + SectionTitle;

                // -----------------------------------------------------------------------------------------------
                // NOTE ON PAGE LOAD LOGIC:
                // We are checking here for required elements to allowing the customer to stay on this page.
                // Many of these checks may be redundant, and they DO add a bit of overhead in terms of db calls, but ANYTHING really
                // could have changed since the customer was on the last page. Remember, the web is completely stateless. Assume this
                // page was executed by ANYONE at ANYTIME (even someone trying to break the cart). 
                // It could have been yesterday, or 1 second ago, and other customers could have purchased limitied inventory products, 
                // coupons may no longer be valid, etc, etc, etc...
                // -----------------------------------------------------------------------------------------------
                ShoppingCart cart = new ShoppingCart(SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);
                cart.ValidProceedCheckout(); // will not come back from this if any issue. they are sent back to the cart page!

            }

            AllowShipToDifferentThanBillTo = AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo") && !AppLogic.AppConfigBool("SkipShippingOnCheckout");
            ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");
            AppLogic.CheckForScriptTag(ReturnURL);
            ErrorMsgLabel.Text = "";

            if (!AppLogic.AppConfigBool("RequireOver13Checked"))
            {
                pnlOver13.Visible = false;
            }
            if (!AppLogic.AppConfigBool("VAT.Enabled"))
            {
                VATRegistrationIDRow.Visible = false;
            }

            if (!IsPostBack)
            {
                BillingAddress.LoadByCustomer(ThisCustomer.CustomerID, ThisCustomer.PrimaryBillingAddressID, AddressTypes.Billing);
                ShippingAddress.LoadByCustomer(ThisCustomer.CustomerID, ThisCustomer.PrimaryShippingAddressID, AddressTypes.Shipping);
                InitializeValidationErrorMessages();
                InitializePageContent();
            }
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

        void BillingState_DataBound(object sender, EventArgs e)
        {
            //BillingState.Items.Insert(0, new ListItem(AppLogic.GetString("createaccount.aspx.48", SkinID, ThisCustomer.LocaleSetting), ""));
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

        void BillingCountry_DataBound(object sender, EventArgs e)
        {
            //BillingCountry.Items.Insert(0, new ListItem(AppLogic.GetString("createaccount.aspx.48", SkinID, ThisCustomer.LocaleSetting), ""));
            int i = BillingCountry.Items.IndexOf(BillingCountry.Items.FindByValue(BillingAddress.Country));
            if (i == -1 || BillingAddress.Country == String.Empty)
            {
                BillingCountry.SelectedValue = "United States";
            }
            else
            {
                BillingCountry.SelectedIndex = i;
            }
        }

        void ShippingState_DataBound(object sender, EventArgs e)
        {
            //ShippingState.Items.Insert(0, new ListItem(AppLogic.GetString("createaccount.aspx.48", SkinID, ThisCustomer.LocaleSetting), ""));
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

        void ShippingCountry_DataBound(object sender, EventArgs e)
        {
            //ShippingCountry.Items.Insert(0, new ListItem(AppLogic.GetString("createaccount.aspx.48", SkinID, ThisCustomer.LocaleSetting), ""));
            int i = ShippingCountry.Items.IndexOf(ShippingCountry.Items.FindByValue(ShippingAddress.Country));
            if (i == -1 || ShippingAddress.Country == String.Empty)
            {
                ShippingCountry.SelectedValue = "United States";
            }
            else
            {
                ShippingCountry.SelectedIndex = i;
            }
        }

        public void btnContinueCheckout_Click(object sender, EventArgs e)
        {
            CreateAccount();
        }

        public void valCustSecurityCode_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = (SecurityCode.Text.Trim() == CommonLogic.SessionNotServerFarmSafe("SecurityCode"));
        }

        public void btnShppingEqBilling_Click(object sender, EventArgs e)
        {
            SetPasswordFields();
            SetShippingStateList(BillingCountry.SelectedValue);

            ShippingFirstName.Text = BillingFirstName.Text;
            ShippingLastName.Text = BillingLastName.Text;
            ShippingPhone.Text = BillingPhone.Text;
            ShippingCompany.Text = BillingCompany.Text;
            ShippingResidenceType.SelectedIndex = BillingResidenceType.SelectedIndex;
            ShippingAddress1.Text = BillingAddress1.Text;
            ShippingAddress2.Text = BillingAddress2.Text;
            ShippingSuite.Text = BillingSuite.Text;
            ShippingCity.Text = BillingCity.Text;
            ShippingCountry.SelectedIndex = BillingCountry.SelectedIndex;
            ShippingState.SelectedIndex = BillingState.SelectedIndex;
            ShippingZip.Text = BillingZip.Text;
        }

        #endregion


        public void ValidatePassword(object source, ServerValidateEventArgs args)
        {
            string pwd1 = ViewState["custpwd"].ToString();
            string pwd2 = ViewState["custpwd2"].ToString();

            if (pwd1.Length == 0)
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
                    if (AppLogic.AppConfigBool("UseStrongPwd") || ThisCustomer.IsAdminUser)
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

        #region Private Functions

        private void InitializePageContent()
        {

            if (Checkout)
            {
                pnlCheckoutImage.Visible = true;
                CheckoutImage.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_2.gif");
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("errormsg").Length > 0)
            {
                AppLogic.CheckForScriptTag(CommonLogic.QueryStringCanBeDangerousContent("errormsg"));
                ErrorMsgLabel.Text = Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg")).Replace("+", " ");
            }

            if (Checkout && !ThisCustomer.IsRegistered)
            {
                Signin.Text = "<p align=\"left\">" + AppLogic.GetString("createaccount.aspx.3", SkinID, ThisCustomer.LocaleSetting) + " <a href=\"signin.aspx?checkout=" + CommonLogic.QueryStringBool("checkout").ToString().ToLowerInvariant() + "&returnURL=" + Server.UrlEncode(CommonLogic.IIF(Checkout, "shoppingcart.aspx?checkout=true", "account.aspx")) + "\"><b>" + AppLogic.GetString("createaccount.aspx.4", SkinID, ThisCustomer.LocaleSetting) + "</b></a>.</p>";
            }

            if (ThisCustomer.Password.Length != 0)
            {
                PasswordPanel.Visible = false;
            }

            Over13.Checked = ThisCustomer.IsOver13;
            VATRegistrationID.Text = ThisCustomer.VATRegistrationID;

            //Account Info
            if (!SkipRegistration)
            {
                pnlAccountInfo.Visible = true;
                tblAccount.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
                tblAccountBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
                accountinfo_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/accountinfo.gif");
                if (ViewState["custpwd"] == null)
                {
                    txtpassword.TextMode = TextBoxMode.Password;
                    txtpassword2.TextMode = TextBoxMode.Password;
                }
                if (Checkout && (AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout") || AppLogic.AppConfigBool("HidePasswordFieldDuringCheckout")))
                {
                    valPassword.Visible = false;
                    valPassword.Enabled = false;
                }
                Over13.Visible = AppLogic.AppConfigBool("RequireOver13Checked");
                if (!AppLogic.AppConfigBool("VAT.Enabled"))
                {
                    VATRegistrationIDRow.Visible = false;
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
                    valCustSecurityCode.ErrorMessage = AppLogic.GetString("createaccount_process.aspx.2", 1, Localization.GetWebConfigLocale());
                    SecurityImage.Visible = true;
                    SecurityImage.ImageUrl = "jpegimage.aspx";

                }

                if (!IsPostBack)
                {
                    FirstName.Text = Server.HtmlEncode(CommonLogic.IIF(ThisCustomer.FirstName.Length != 0, ThisCustomer.FirstName, BillingAddress.FirstName));
                    LastName.Text = Server.HtmlEncode(CommonLogic.IIF(ThisCustomer.LastName.Length != 0, ThisCustomer.LastName, BillingAddress.LastName));

                    String emailx = ThisCustomer.EMail;
                    EMail.Text = Server.HtmlEncode(emailx).ToLowerInvariant().Trim();

                    Phone.Text = Server.HtmlEncode(CommonLogic.IIF(ThisCustomer.Phone.Length != 0, ThisCustomer.Phone, BillingAddress.Phone));
                    // Create a phone validation error message

                    createaccountaspx23.Text = "*" + AppLogic.GetString("createaccount.aspx.23", SkinID, ThisCustomer.LocaleSetting);
                    OKToEMailYes.Checked = (ThisCustomer.EMail.Length != 0);
                    OKToEMailNo.Checked = !OKToEMailYes.Checked;
                }
            }
            else
            {
                valReqSkipRegEmail.Enabled = AppLogic.AppConfigBool("AnonCheckoutReqEmail");
                String emailx = ThisCustomer.EMail;
                txtSkipRegEmail.Text = Server.HtmlEncode(emailx).ToLowerInvariant().Trim();
                Literal2.Visible = AppLogic.AppConfigBool("RequireOver13Checked");
                SkipRegOver13.Visible = AppLogic.AppConfigBool("RequireOver13Checked");
                SkipRegOver13.Checked = ThisCustomer.IsOver13;
                pnlSkipReg.Visible = true;
                createaccountaspx30.Visible = false;
                BillingEqualsAccount.Visible = false;
                createaccountaspx31.Visible = false;
            }

            if (!IsPostBack)
            {

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

                createaccountaspx31.Text = AppLogic.GetString("createaccount.aspx.31", SkinID, ThisCustomer.LocaleSetting);
                if (AllowShipToDifferentThanBillTo)
                {
                    createaccountaspx30.Text = AppLogic.GetString("createaccount.aspx.30", SkinID, ThisCustomer.LocaleSetting);
                }
                else
                {
                    createaccountaspx30.Text = AppLogic.GetString("createaccount.aspx.32", SkinID, ThisCustomer.LocaleSetting);
                }

                BillingFirstName.Text = Server.HtmlEncode(CommonLogic.IIF(ThisCustomer.FirstName.Length != 0, ThisCustomer.FirstName, BillingAddress.FirstName));
                BillingLastName.Text = Server.HtmlEncode(CommonLogic.IIF(ThisCustomer.LastName.Length != 0, ThisCustomer.LastName, BillingAddress.LastName));
                BillingPhone.Text = Server.HtmlEncode(BillingAddress.Phone);
                BillingCompany.Text = Server.HtmlEncode(BillingAddress.Company);
                BillingResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.55", SkinID, ThisCustomer.LocaleSetting), ((int)ResidenceTypes.Unknown).ToString()));
                BillingResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.56", SkinID, ThisCustomer.LocaleSetting), ((int)ResidenceTypes.Residential).ToString()));
                BillingResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.57", SkinID, ThisCustomer.LocaleSetting), ((int)ResidenceTypes.Commercial).ToString()));
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
                BillingCountry.SelectedIndex = 0;

                SetBillingStateList(BillingCountry.SelectedValue);
                try
                {
                    BillingState.SelectedValue = BillingAddress.State;
                }
                catch { }

                //Shipping Info
                if (AllowShipToDifferentThanBillTo)
                {
                    pnlShippingInfo.Visible = true;
                    tblShippingInfo.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
                    tblShippingInfoBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
                    shippinginfo_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/shippinginfo.gif");

                    ShippingFirstName.Text = Server.HtmlEncode(ShippingAddress.FirstName);
                    ShippingLastName.Text = Server.HtmlEncode(CommonLogic.IIF(ThisCustomer.LastName.Length != 0, ThisCustomer.LastName, BillingAddress.LastName));
                    ShippingPhone.Text = Server.HtmlEncode(ShippingAddress.Phone);
                    ShippingCompany.Text = Server.HtmlEncode(ShippingAddress.Company);
                    //ShippingResidenceType
                    ShippingResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.55", SkinID, ThisCustomer.LocaleSetting), ((int)ResidenceTypes.Unknown).ToString()));
                    ShippingResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.56", SkinID, ThisCustomer.LocaleSetting), ((int)ResidenceTypes.Residential).ToString()));
                    ShippingResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.57", SkinID, ThisCustomer.LocaleSetting), ((int)ResidenceTypes.Commercial).ToString()));
                    ShippingResidenceType.SelectedIndex = 1;
                    ShippingAddress1.Text = Server.HtmlEncode(ShippingAddress.Address1);
                    ShippingAddress2.Text = Server.HtmlEncode(ShippingAddress.Address2);
                    ShippingSuite.Text = Server.HtmlEncode(ShippingAddress.Suite);
                    ShippingCity.Text = Server.HtmlEncode(ShippingAddress.City);
                    ShippingZip.Text = ShippingAddress.Zip;

                    dr = DB.GetRS("select * from Country " + DB.GetNoLock() + " where Published = 1 order by DisplayOrder,Name");
                    ShippingCountry.DataSource = dr;
                    ShippingCountry.DataTextField = "Name";
                    ShippingCountry.DataValueField = "Name";
                    ShippingCountry.DataBind();
                    dr.Close();
                    ShippingCountry.SelectedIndex = 0;

                    SetShippingStateList(ShippingCountry.SelectedValue);

                    try
                    {
                        ShippingState.SelectedValue = ShippingAddress.State;
                    }
                    catch { }

                }
            }
            if (!ThisCustomer.IsRegistered)
            {
                if (SkipRegistration)
                {
                    btnContinueCheckout.Text = CommonLogic.IIF(Checkout, AppLogic.GetString("createaccount.aspx.76", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), AppLogic.GetString("createaccount.aspx.75", SkinID, ThisCustomer.LocaleSetting));
                }
                else
                {
                    btnContinueCheckout.Text = CommonLogic.IIF(Checkout, AppLogic.GetString("createaccount.aspx.74", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), AppLogic.GetString("createaccount.aspx.75", SkinID, ThisCustomer.LocaleSetting));
                }
            }
            else
            {
                btnContinueCheckout.Text = AppLogic.GetString("account.aspx.60", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            }

            GetJavaScriptFunctions();
            AppLogic.GetButtonDisable(this.btnContinueCheckout);

        }

        private void InitializeValidationErrorMessages()
        {
            //valReqFName.ErrorMessage = AppLogic.GetString("createaccount.aspx.5", SkinID, ThisCustomer.LocaleSetting);
            //valReqLName.ErrorMessage = AppLogic.GetString("createaccount.aspx.5", SkinID, ThisCustomer.LocaleSetting);
            valReqEmail.ErrorMessage = AppLogic.GetString("createaccount.aspx.16", SkinID, ThisCustomer.LocaleSetting);
            valRegExValEmail.ErrorMessage = AppLogic.GetString("createaccount.aspx.17", SkinID, ThisCustomer.LocaleSetting);
            reqValPassword.ErrorMessage = AppLogic.GetString("createaccount.aspx.20", SkinID, ThisCustomer.LocaleSetting);
            valPassword.ErrorMessage = AppLogic.GetString("createaccount.aspx.21", SkinID, ThisCustomer.LocaleSetting);
            valReqPhone.ErrorMessage = AppLogic.GetString("createaccount.aspx.24", SkinID, ThisCustomer.LocaleSetting);
            valReqSecurityCode.ErrorMessage = AppLogic.GetString("signin.aspx.20", SkinID, ThisCustomer.LocaleSetting);
            valReqBillFName.ErrorMessage = AppLogic.GetString("createaccount.aspx.34", SkinID, ThisCustomer.LocaleSetting);
            valReqBillLName.ErrorMessage = AppLogic.GetString("createaccount.aspx.36", SkinID, ThisCustomer.LocaleSetting);
            valReqBillPhone.ErrorMessage = AppLogic.GetString("createaccount.aspx.38", SkinID, ThisCustomer.LocaleSetting);
            valReqBillAddr1.ErrorMessage = AppLogic.GetString("createaccount.aspx.42", SkinID, ThisCustomer.LocaleSetting);
            valReqBillCity.ErrorMessage = AppLogic.GetString("createaccount.aspx.46", SkinID, ThisCustomer.LocaleSetting);
            valReqBillZip.ErrorMessage = AppLogic.GetString("createaccount.aspx.50", SkinID, ThisCustomer.LocaleSetting);
            valReqBillState.ErrorMessage = AppLogic.GetString("createaccount.aspx.8", SkinID, ThisCustomer.LocaleSetting);
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
            BillingEqualsAccount.Attributes.Add("onclick", "copyaccount(this.form);");
            string strScript = "<script type=\"text/javascript\">";
            strScript += "function copyaccount(theForm){ ";
            strScript += "if (theForm." + BillingEqualsAccount.ClientID + ".checked){";
            strScript += "theForm." + BillingFirstName.ClientID + ".value = theForm." + FirstName.ClientID + ".value;";
            strScript += "theForm." + BillingLastName.ClientID + ".value = theForm." + LastName.ClientID + ".value;";
            strScript += "theForm." + BillingPhone.ClientID + ".value = theForm." + Phone.ClientID + ".value;";
            strScript += "} ";
            strScript += "return true; }  ";
            strScript += "</script> ";

            ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), strScript);

        }

        private void CreateAccount()
        {
            SetPasswordFields();

            string AccountName = (FirstName.Text.Trim() + " " + LastName.Text.Trim()).Trim();
            if (SkipRegistration)
            {
                AccountName = (BillingFirstName.Text.Trim() + " " + BillingLastName.Text.Trim()).Trim();
            }

            if (SkipRegistration)
            {
                Page.Validate("skipreg");
            }
            else
            {
                Page.Validate("registration");
            }
            Page.Validate("createacccount");

            String Adr1 = CommonLogic.IIF(AllowShipToDifferentThanBillTo, ShippingAddress1.Text, BillingAddress1.Text);
            Adr1 = Adr1.Replace(" ", "").Trim().Replace(".", "");
            bool IsPOBoxAddress = (Adr1.StartsWith("pobox", StringComparison.InvariantCultureIgnoreCase) || Adr1.StartsWith("box", StringComparison.InvariantCultureIgnoreCase) || Adr1 == "postoffice");
            bool RejectDueToPOBoxAddress = (IsPOBoxAddress && AppLogic.AppConfigBool("POBox.Disallow")); // undocumented feature

            if (Page.IsValid && AccountName.Length > 0)
            {
                String EMailField = CommonLogic.IIF(SkipRegistration, txtSkipRegEmail.Text.ToLowerInvariant().Trim(), EMail.Text.ToLowerInvariant().Trim());
                bool EMailAlreadyTaken = Customer.EmailInUse(EMailField, ThisCustomer.CustomerID) && !AppLogic.AppConfigBool("AllowCustomerDuplicateEMailAddresses");

                String PWD = ViewState["custpwd"].ToString();
                Password p = new Password(PWD);
                String newpwd = p.SaltedPassword;
                System.Nullable<int> newsaltkey = p.Salt;
                if (ThisCustomer.Password.Length != 0)
                {
                    // do NOT allow passwords to be changed on this page. this is only for creating an account.
                    // if they want to change their password, they must use their account page
                    newpwd = null;
                    newsaltkey = null;
                }
                if (!EMailAlreadyTaken)
                {
                    AppLogic.eventHandler("CreateAccount").CallEvent("&CreateAccount=true");

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
                    ShippingAddress = new Address();

                    BillingAddress.LastName = BillingLastName.Text;
                    BillingAddress.FirstName = BillingFirstName.Text;
                    BillingAddress.Phone = BillingPhone.Text;
                    BillingAddress.Company = BillingCompany.Text;
                    BillingAddress.ResidenceType = (ResidenceTypes)Convert.ToInt32(BillingResidenceType.SelectedValue);
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

                    if (AllowShipToDifferentThanBillTo)
                    {
                        ShippingAddress.LastName = ShippingLastName.Text;
                        ShippingAddress.FirstName = ShippingFirstName.Text;
                        ShippingAddress.Phone = ShippingPhone.Text;
                        ShippingAddress.Company = ShippingCompany.Text;
                        ShippingAddress.ResidenceType = (ResidenceTypes)Convert.ToInt32(ShippingResidenceType.SelectedValue);
                        ShippingAddress.Address1 = ShippingAddress1.Text;
                        ShippingAddress.Address2 = ShippingAddress2.Text;
                        ShippingAddress.Suite = ShippingSuite.Text;
                        ShippingAddress.City = ShippingCity.Text;
                        ShippingAddress.State = ShippingState.SelectedValue;
                        ShippingAddress.Zip = ShippingZip.Text;
                        ShippingAddress.Country = ShippingCountry.SelectedValue;
                        ShippingAddress.EMail = EMailField;

                        ShippingAddress.InsertDB(ThisCustomer.CustomerID);
                        if (AppLogic.AppConfig("VerifyAddressesProvider") != "")
                        {

                            VerifyResult = AddressValidation.RunValidate(ShippingAddress, out StandardizedAddress);
                            VerifyAddressPrompt = (VerifyResult != AppLogic.ro_OK);
                            if (VerifyAddressPrompt)
                            {
                                ShippingAddress = StandardizedAddress;
                                ShippingAddress.UpdateDB();
                            }
                        }
                        ShippingAddress.MakeCustomersPrimaryAddress(AddressTypes.Shipping);
                    }
                    else
                    {
                        if (AppLogic.AppConfig("VerifyAddressesProvider") != "")
                        {

                            VerifyResult = AddressValidation.RunValidate(BillingAddress, out StandardizedAddress);
                            VerifyAddressPrompt = (VerifyResult != AppLogic.ro_OK);
                            if (VerifyAddressPrompt)
                            {
                                BillingAddress = StandardizedAddress;
                                BillingAddress.UpdateDB();
                            }
                        }
                        BillingAddress.MakeCustomersPrimaryAddress(AddressTypes.Shipping);
                    }

                    String vtr = VATRegistrationID.Text;
                    if (!AppLogic.AppConfigBool("VAT.Enabled") || !AppLogic.VATRegistrationIDIsValid(BillingCountry.SelectedValue, vtr))
                    {
                        vtr = String.Empty;
                    }
                    ThisCustomer.SetVATRegistrationID(vtr);
                }
                if (Checkout)
                {
                    if (EMailAlreadyTaken)
                    {
                        ErrorMsgLabel.Text = AppLogic.GetString("createaccount_process.aspx.1", 1, Localization.GetWebConfigLocale());
                        InitializePageContent();
                    }
                    else if (RejectDueToPOBoxAddress)
                    {
                        ErrorMsgLabel.Text = "P.O. Box ship-to addresses are not supported. Please enter a full street residential or commercial address. Thanks";
                        InitializePageContent();
                    }
                    else
                    {
                        if (AppLogic.AppConfigBool("SendWelcomeEmail"))
                        {
                            AppLogic.SendMail(AppLogic.GetString("createaccount.aspx.79", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), AppLogic.RunXmlPackage(AppLogic.AppConfig("XmlPackage.WelcomeEmail"), null, ThisCustomer, this.SkinID, "", "fullname=" + FirstName.Text.Trim() + " " + LastName.Text.Trim(), false, false, this.EntityHelpers), true, AppLogic.AppConfig("MailMe_FromAddress"), AppLogic.AppConfig("MailMe_FromName"), EMailField, FirstName.Text.Trim() + " " + LastName.Text.Trim(), "", AppLogic.MailServer());
                        }
                        if (VerifyAddressPrompt)
                        {
                            if (AllowShipToDifferentThanBillTo)
                            {
                                Response.Redirect("editaddress.aspx?Checkout=True&AddressType=Shipping&AddressID=" + Customer.GetCustomerPrimaryShippingAddressID(ThisCustomer.CustomerID).ToString() + "&NewAccount=true&prompt=" + VerifyResult);
                            }
                            else
                            {
                                Response.Redirect("editaddress.aspx?Checkout=True&AddressType=Billing&AddressID=" + Customer.GetCustomerPrimaryShippingAddressID(ThisCustomer.CustomerID).ToString() + "&NewAccount=true&prompt=" + VerifyResult);
                            }
                        }
                        else
                        {
                            Response.Redirect("checkoutshipping.aspx");
                        }
                    }
                }
                else
                {
                    if (EMailAlreadyTaken)
                    {
                        DB.ExecuteSQL("update customer set EMail='', IsRegistered = 0 where CustomerID=" + ThisCustomer.CustomerID);
                        ErrorMsgLabel.Text = AppLogic.GetString("createaccount_process.aspx.1", 1, Localization.GetWebConfigLocale());
                        InitializePageContent();
                    }
                    else if (RejectDueToPOBoxAddress)
                    {
                        ErrorMsgLabel.Text = "P.O. Box ship-to addresses are not supported. Please enter a full street residential or commercial address. Thanks";
                        InitializePageContent();
                    }
                    else
                    {
                        if (AppLogic.AppConfigBool("SendWelcomeEmail"))
                        {
                            AppLogic.SendMail(AppLogic.GetString("createaccount.aspx.79", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), AppLogic.RunXmlPackage(AppLogic.AppConfig("XmlPackage.WelcomeEmail"), null, ThisCustomer, this.SkinID, "", "", false, false, this.EntityHelpers), true, AppLogic.AppConfig("MailMe_FromAddress"), AppLogic.AppConfig("MailMe_FromName"), EMailField, FirstName.Text.Trim() + " " + LastName.Text.Trim(), "", AppLogic.MailServer());
                        }
                        if (VerifyAddressPrompt)
                        {
                            if (AllowShipToDifferentThanBillTo)
                            {
                                Response.Redirect("editaddress.aspx?Checkout=False&AddressType=Shipping&AddressID=" + Customer.GetCustomerPrimaryShippingAddressID(ThisCustomer.CustomerID).ToString() + "&NewAccount=true&prompt=" + VerifyResult);
                            }
                            else
                            {
                                Response.Redirect("editaddress.aspx?Checkout=False&AddressType=Billing&AddressID=" + Customer.GetCustomerPrimaryShippingAddressID(ThisCustomer.CustomerID).ToString() + "&NewAccount=true&prompt=" + VerifyResult);
                            }
                        }
                        else
                        {
                            Response.Redirect("account.aspx?newaccount=true");
                        }
                    }
                }
            }
            else
            {
                ErrorMsgLabel.Text += "<br /><br /> Some errors occured trying to create your account.  Please correct them and try again.<br /><br />";
                if (AccountName.Length == 0)
                {
                    ErrorMsgLabel.Text += "&bull; " + AppLogic.GetString("createaccount.aspx.5", 1, Localization.GetWebConfigLocale()) + "<br />";
                }
                foreach (IValidator aValidator in this.Validators)
                {
                    if (!aValidator.IsValid)
                    {
                        ErrorMsgLabel.Text += "&bull; " + aValidator.ErrorMessage + "<br />";
                    }
                }
                ErrorMsgLabel.Text += "<br />";
                GetJavaScriptFunctions();
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
        }

        private void SetPasswordFields()
        {
            if (ViewState["custpwd"] == null)
            {
                ViewState["custpwd"] = "";
            }
            if (txtpassword.Text.Trim() != "")
            {
                ViewState["custpwd"] = txtpassword.Text;
                reqValPassword.Enabled = false;
            }

            if (ViewState["custpwd2"] == null)
            {
                ViewState["custpwd2"] = "";
            }
            if (txtpassword2.Text != "")
            {
                ViewState["custpwd2"] = txtpassword2.Text;
            }
        }

        #endregion
    }
}
