// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2004.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/checkoutanon.aspx.cs 17    9/30/06 10:44p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Globalization;
using System.Web.Security;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for checkoutanon.
    /// </summary>
    public partial class checkoutanon : SkinBase
    {

        String PaymentMethod = String.Empty;
        ShoppingCart cart;
        Customer c;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = -1;
            Response.AddHeader("pragma", "no-cache");

            RequireSecurePage();

            SectionTitle = AppLogic.GetString("checkoutanon.aspx.1", SkinID, ThisCustomer.LocaleSetting);

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

            cart = new ShoppingCart(SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);

            cart.ValidProceedCheckout(); // will not come back from this if any issue. they are sent back to the cart page!

            CheckoutMap.HotSpots[0].AlternateText = AppLogic.GetString("checkoutanon.aspx.2", SkinID, ThisCustomer.LocaleSetting);

            if (AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout"))
            {
                PasswordOptionalPanel.Visible = true;
            }

            ErrorMsgLabel.Text = "";
            if (!IsPostBack)
            {
                InitializePageContent();
            }
        }

        private void InitializePageContent()
        {
            JSPopupRoutines.Text = AppLogic.GetJSPopupRoutines();
            CheckoutMap.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_2.gif");
        }

        protected void btnSignInAndCheckout_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {

                String EMailField = EMail.Text.ToLowerInvariant().Trim();
                String PasswordField = Password.Text;

                bool LoginOK = false;

                if (AppLogic.AppConfigBool("SecurityCodeRequiredOnStoreLogin"))
                {
                    String sCode = CommonLogic.SessionNotServerFarmSafe("SecurityCode");
                    String fCode = CommonLogic.FormCanBeDangerousContent("SecurityCode");
                    if (fCode != sCode)
                    {
                        ErrorMsgLabel.Text = AppLogic.GetString("lat_signin_process.aspx.1", SkinID, ThisCustomer.LocaleSetting);
                        ErrorPanel.Visible = true;
                        return;
                    }
                }

                Customer c = new Customer(EMailField, true);

                if (!c.IsRegistered)
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("lat_signin_process.aspx.1", SkinID, ThisCustomer.LocaleSetting);
                    ErrorPanel.Visible = true;
                    return;
                }
                else
                {
                    if (c.BadLoginCount >= AppLogic.AppConfigNativeInt("MaxBadLogins"))
                    {
                        ErrorMsgLabel.Text = AppLogic.GetString("lat_signin_process.aspx.3", SkinID, ThisCustomer.LocaleSetting);
                        ErrorPanel.Visible = true;
                        return;
                    }
                    else if (!c.Active)
                    {
                        ErrorMsgLabel.Text = AppLogic.GetString("lat_signin_process.aspx.2", SkinID, ThisCustomer.LocaleSetting);
                        ErrorPanel.Visible = true;
                        return;
                    }
                    else if (c.PwdChangeRequired)
                    {
                        ExecutePanel.Visible = false;
                        FormPanel.Visible = false;
                        pnlChangePwd.Visible = true;
                        CustomerEmail.Text = EMailField;
                        OldPassword.Focus();
                        return;
                    }
                    else
                    {
                        LoginOK = c.CheckLogin(PasswordField);
                    }
                }

                if (!LoginOK)
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("lat_signin_process.aspx.1", SkinID, ThisCustomer.LocaleSetting);
                    ErrorPanel.Visible = true;
                    if (c.IsAdminUser)
                    {
                        c.UpdateCustomer(
                            /*CustomerLevelID*/ null,
                            /*EMail*/ null,
                            /*SaltedAndHashedPassword*/ null,
                            /*SaltKey*/ null,
                            /*DateOfBirth*/ null,
                            /*Gender*/ null,
                            /*FirstName*/ null,
                            /*LastName*/ null,
                            /*Notes*/ null,
                            /*SkinID*/ null,
                            /*Phone*/ null,
                            /*AffiliateID*/ null,
                            /*Referrer*/ null,
                            /*CouponCode*/ null,
                            /*OkToEmail*/ null,
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
                            /*Over13Checked*/ null,
                            /*CurrencySetting*/ null,
                            /*VATSetting*/ null,
                            /*VATRegistrationID*/ null,
                            /*StoreCCInDB*/ null,
                            /*IsRegistered*/ null,
                            /*LockedUntil*/ null,
                            /*AdminCanViewCC*/ null,
                            /*BadLogin*/ 1,
                            /*Active*/ null,
                            /*PwdChangeRequired*/ null,
                            /*RegisterDate*/ null
                             );

                    }
                    Security.LogEvent("Store Login Failed", "Attempted login failed for email address " + EMailField, 0, 0, 0);
                    return;
                }

                int CurrentCustomerID = ThisCustomer.CustomerID;
                int NewCustomerID = c.CustomerID;

                AppLogic.ExecuteSigninLogic(CurrentCustomerID, NewCustomerID);

                if (c.IsAdminUser)
                {
                    Security.LogEvent("Store Login", "", c.CustomerID, c.CustomerID, c.ThisCustomerSession.SessionID);
                }

                // we've got a good login:
                FormPanel.Visible = false;
                HeaderPanel.Visible = false;
                ExecutePanel.Visible = true;


                String CustomerGUID = c.CustomerGUID.Replace("{", "").Replace("}", "");

                SignInExecuteLabel.Text = AppLogic.GetString("signin.aspx.2", SkinID, ThisCustomer.LocaleSetting);

                string sReturnURL = "shoppingcart.aspx";
                FormsAuthentication.SetAuthCookie(CustomerGUID, false);

                if (sReturnURL.Length == 0)
                {
                    sReturnURL = FormsAuthentication.GetRedirectUrl(CustomerGUID, false);
                }
                Response.AddHeader("REFRESH", "1; URL=" + Server.UrlDecode(sReturnURL));
            }
        }
        protected void RegisterAndCheckoutButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("createaccount.aspx?checkout=true");
        }
        protected void Skipregistration_Click(object sender, EventArgs e)
        {
            Response.Redirect("createaccount.aspx?checkout=true&skipreg=true");
        }

        protected void btnChgPwd_Click(object sender, EventArgs e)
        {
            String EMailField = CustomerEmail.Text.ToLowerInvariant();
            String PasswordField = OldPassword.Text;
            String newpwd = NewPassword.Text;
            String newencryptedpwd = "";
            lblPwdChgErr.Text = "";
            lblPwdChgErr.Visible = false;

            bool LoginOK = false;

            c = new Customer(EMailField, true);
            Password pwdold = new Password(PasswordField, c.SaltKey);
            Password pwdnew = new Password(newpwd, c.SaltKey);
            if (c.IsRegistered)
            {
                newencryptedpwd = pwdnew.SaltedPassword;
                if (c.BadLoginCount >= AppLogic.AppConfigNativeInt("MaxBadLogins"))
                {
                    lblPwdChgErr.Text = "<br/><br/>" + AppLogic.GetString("lat_signin_process.aspx.3", SkinID, ThisCustomer.LocaleSetting);
                    lblPwdChgErr.Visible = true;
                    return;
                }

                else if (!c.Active)
                {
                    lblPwdChgErr.Text = "<br/><br/>" + AppLogic.GetString("lat_signin_process.aspx.2", SkinID, ThisCustomer.LocaleSetting);
                    lblPwdChgErr.Visible = true;
                    return;
                }
                else if (c.IsAdminUser || AppLogic.AppConfigBool("UseStrongPwd"))
                {
                    Regex re = new Regex(AppLogic.AppConfig("CustomerPwdValidator"));
                    if (!re.IsMatch(newpwd))
                    {
                        lblPwdChgErr.Text = AppLogic.GetString("signin.aspx.26", SkinID, ThisCustomer.LocaleSetting);
                        lblPwdChgErr.Visible = true;
                        return;
                    }
                }

                LoginOK = (c.Password == pwdold.SaltedPassword);
            }
            else
            {
                lblPwdChgErr.Text = "<br/><br/>" + AppLogic.GetString("lat_signin_process.aspx.1", SkinID, ThisCustomer.LocaleSetting);
                lblPwdChgErr.Visible = true;
                return;
            }

            if (!c.IsAdminUser && !ValidateNewPassword())
            {
                lblPwdChgErr.Visible = true;
                return;
            }

            if (!LoginOK)
            {
                lblPwdChgErr.Text += "<br/>" + AppLogic.GetString("lat_signin_process.aspx.1", SkinID, ThisCustomer.LocaleSetting);
                lblPwdChgErr.Visible = true;
                if (c.IsAdminUser)
                {
                    c.UpdateCustomer(
                        /*CustomerLevelID*/ null,
                        /*EMail*/ null,
                        /*SaltedAndHashedPassword*/ null,
                        /*SaltKey*/ null,
                        /*DateOfBirth*/ null,
                        /*Gender*/ null,
                        /*FirstName*/ null,
                        /*LastName*/ null,
                        /*Notes*/ null,
                        /*SkinID*/ null,
                        /*Phone*/ null,
                        /*AffiliateID*/ null,
                        /*Referrer*/ null,
                        /*CouponCode*/ null,
                        /*OkToEmail*/ null,
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
                        /*Over13Checked*/ null,
                        /*CurrencySetting*/ null,
                        /*VATSetting*/ null,
                        /*VATRegistrationID*/ null,
                        /*StoreCCInDB*/ null,
                        /*IsRegistered*/ null,
                        /*LockedUntil*/ null,
                        /*AdminCanViewCC*/ null,
                        /*BadLogin*/ 1,
                        /*Active*/ null,
                        /*PwdChangeRequired*/ null,
                        /*RegisterDate*/ null
                        );
                }
                return;
            }



            if (c.IsAdminUser)
            {
                Security.LogEvent("Admin Password Changed", "", c.CustomerID, c.CustomerID, 0);
            }

            c.UpdateCustomer(
                /*CustomerLevelID*/ null,
                /*EMail*/ null,
                /*SaltedAndHashedPassword*/ pwdnew.SaltedPassword,
                /*SaltKey*/ pwdnew.Salt,
                /*DateOfBirth*/ null,
                /*Gender*/ null,
                /*FirstName*/ null,
                /*LastName*/ null,
                /*Notes*/ null,
                /*SkinID*/ null,
                /*Phone*/ null,
                /*AffiliateID*/ null,
                /*Referrer*/ null,
                /*CouponCode*/ null,
                /*OkToEmail*/ null,
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
                /*Over13Checked*/ null,
                /*CurrencySetting*/ null,
                /*VATSetting*/ null,
                /*VATRegistrationID*/ null,
                /*StoreCCInDB*/ null,
                /*IsRegistered*/ null,
                /*LockedUntil*/ null,
                /*AdminCanViewCC*/ null,
                /*BadLogin*/ -1,
                /*Active*/ null,
                /*PwdChangeRequired*/ 0,
                /*RegisterDate*/ null
                 );
            FormPanel.Visible = false;
            ExecutePanel.Visible = true;
            pnlChangePwd.Visible = false;

            AppLogic.ExecuteSigninLogic(ThisCustomer.CustomerID, c.CustomerID);

            String CustomerGUID = c.CustomerGUID.Replace("{", "").Replace("}", "");

            SignInExecuteLabel.Text = AppLogic.GetString("signin.aspx.24", SkinID, ThisCustomer.LocaleSetting);

            string sReturnURL = FormsAuthentication.GetRedirectUrl(CustomerGUID, true);
            FormsAuthentication.SetAuthCookie(CustomerGUID, true);

            if (sReturnURL.Length == 0)
            {
                sReturnURL = FormsAuthentication.GetRedirectUrl(CustomerGUID, false);
            }
            Response.AddHeader("REFRESH", "1; URL=" + Server.UrlDecode(sReturnURL));
        }

        private bool ValidateNewPassword()
        {
            bool IsValid = false;
            string err = string.Empty;
            if (NewPassword.Text.Replace("*", "").Trim().Length == 0)
            {
                err = "<br/><br/>" + AppLogic.GetString("account.aspx.7", SkinID, ThisCustomer.LocaleSetting);
                IsValid = false;
            }
            else if (NewPassword.Text == NewPassword2.Text)
            {
                try
                {
                    if (AppLogic.AppConfigBool("UseStrongPwd") || ThisCustomer.IsAdminUser)
                    {
                        Regex re = new Regex(AppLogic.AppConfig("CustomerPwdValidator"));
                        IsValid = re.IsMatch(NewPassword.Text);
                    }
                    else
                    {
                        IsValid = (NewPassword.Text.Length > 4);
                    }
                    if (!IsValid)
                    {
                        err = "<br/><br/>" + AppLogic.GetString("account.aspx.7", SkinID, ThisCustomer.LocaleSetting);
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
                err = "<br/><br/>" + AppLogic.GetString("createaccount.aspx.80", SkinID, ThisCustomer.LocaleSetting);
            }
            lblPwdChgErr.Text = err;
            return IsValid;
        }

    }
}
