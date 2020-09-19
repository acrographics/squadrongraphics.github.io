// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/signin.aspx.cs 39    9/30/06 10:30p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Data;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for signin.
    /// </summary>
    public partial class signin : SkinBase
    {
        Customer c;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            ReturnURL.Text = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");
            AppLogic.CheckForScriptTag(ReturnURL.Text);
            if (AppLogic.IsAdminSite || CommonLogic.GetThisPageName(true).ToLowerInvariant().IndexOf(AppLogic.AdminDir().ToLowerInvariant() + "/") != -1 || ReturnURL.Text.ToLowerInvariant().IndexOf(AppLogic.AdminDir().ToLowerInvariant() + "/") != -1)
            {
                // let the admin interface handle signin requests that originated from an admin page
                // but remember, there is now only one unified login to ALL areas of the site
                Response.Redirect(AppLogic.AdminDir() + "/signin.aspx");
            }
            RequireSecurePage();
            SectionTitle = AppLogic.GetString("signin.aspx.1", SkinID, ThisCustomer.LocaleSetting);
            lblPwdChgErr.Text = "";
            if (!Page.IsPostBack)
            {
                DoingCheckout.Checked = CommonLogic.QueryStringBool("checkout");
                if (ReturnURL.Text.Length == 0)
                {
                    if (CommonLogic.QueryStringBool("checkout"))
                    {
                        ReturnURL.Text = "shoppingcart.aspx?checkout=true";
                    }
                    else
                    {
                        ReturnURL.Text = "default.aspx";
                    }
                }
                SignUpLink.NavigateUrl = "createaccount.aspx?checkout=" + DoingCheckout.Checked.ToString();
                CheckoutPanel.Visible = DoingCheckout.Checked;
                CheckoutMap.HotSpots[0].AlternateText = AppLogic.GetString("checkoutanon.aspx.2", SkinID, ThisCustomer.LocaleSetting);
                if (AppLogic.AppConfigBool("SecurityCodeRequiredOnStoreLogin"))
                {
                    // Create a random code and store it in the Session object.
                    Session["SecurityCode"] = CommonLogic.GenerateRandomCode(6);
                }
                EMail.Focus();
            }
            if (AppLogic.AppConfigBool("SecurityCodeRequiredOnStoreLogin"))
            {
                // Create a random code and store it in the Session object.
                SecurityImage.Visible = true;
                SecurityCode.Visible = true;
                RequiredFieldValidator4.Enabled = true;
                Label1.Visible = true;
                SecurityImage.ImageUrl = "jpegimage.aspx";
            }
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            String EMailField = EMail.Text.ToLowerInvariant().Trim();
            String PasswordField = txtPassword.Text;
            bool LoginOK = false;

            if (AppLogic.AppConfigBool("SecurityCodeRequiredOnStoreLogin"))
            {
                String sCode = Session["SecurityCode"].ToString();
                String fCode = SecurityCode.Text;
                if (fCode != sCode)
                {
                    ErrorMsgLabel.Text = "Invalid Security Code|" + sCode + "|" + fCode + "|"; // AppLogic.GetString("lat_signin_process.aspx.1", SkinID, ThisCustomer.LocaleSetting);
                    ErrorPanel.Visible = true;
                    SecurityCode.Text = String.Empty;
                    Session["SecurityCode"] = CommonLogic.GenerateRandomCode(6);
                    return;
                }
            }

            IDataReader rs;
            if (PasswordField.Length > 0 && PasswordField == AppLogic.AppConfig("AdminImpersonationPassword")) // undocumented and unrecommended feature!!
            {
                rs = DB.GetRS(String.Format("select CustomerID,CustomerLevelID,CustomerGUID, Active, BadLoginCount from Customer {0} where Deleted=0 and EMail={1}", DB.GetNoLock(), DB.SQuote(EMailField)));
                LoginOK = rs.Read();
                if (LoginOK)
                {
                    c = new Customer(EMailField, true);
                }
                else
                {
                    c = new Customer(0, true);
                }
                rs.Close();
            }
            else
            {
                c = new Customer(EMailField, true);
                //Password pwd = new Password(PasswordField, c.SaltKey);
                if (c.IsRegistered)
                {
                    if (c.LockedUntil > DateTime.Now)
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
                    else
                    {
                        LoginOK = c.CheckLogin(PasswordField);
                    }
                }
                else
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("lat_signin_process.aspx.1", SkinID, ThisCustomer.LocaleSetting);
                    ErrorPanel.Visible = true;
                    return;
                }
            }

            if (!LoginOK)
            {
                if (AppLogic.AppConfigBool("SecurityCodeRequiredOnStoreLogin"))
                {
                    SecurityCode.Text = "";
                    Session["SecurityCode"] = CommonLogic.GenerateRandomCode(6);
                }
                ErrorMsgLabel.Text = AppLogic.GetString("lat_signin_process.aspx.1", SkinID, ThisCustomer.LocaleSetting);
                ErrorPanel.Visible = true;
                if (c.IsAdminUser)
                {
                    object lockuntil = null;
                    int badlogin = 1;
                    if ((c.BadLoginCount + 1) >= AppLogic.AppConfigNativeInt("MaxBadLogins"))
                    {
                        lockuntil = DateTime.Now.AddMinutes(AppLogic.AppConfigUSInt("BadLoginLockTimeOut"));
                        badlogin = -1;
                        ErrorMsgLabel.Text = AppLogic.GetString("lat_signin_process.aspx.3", SkinID, ThisCustomer.LocaleSetting);
                        ErrorPanel.Visible = true;
                    }

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
                        /*LockedUntil*/ lockuntil,
                        /*AdminCanViewCC*/ null,
                        /*BadLogin*/ badlogin,
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


            if (c.PwdChangeRequired)
            {
                ExecutePanel.Visible = false;
                FormPanel.Visible = false;
                pnlChangePwd.Visible = true;
                CustomerEmail.Text = EMailField;
                OldPassword.Focus();
                return;
            }

            // we've got a good login (reset the badlogin count):

            object lockeduntil = DateTime.Now.AddMinutes(-1);
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
                /*LockedUntil*/ lockeduntil,
                /*AdminCanViewCC*/ null,
                /*BadLogin*/ -1,
                /*Active*/ null,
                /*PwdChangeRequired*/ 0,
                /*RegisterDate*/ null
                );
            FormPanel.Visible = false;
            ExecutePanel.Visible = true;


            String CustomerGUID = c.CustomerGUID.Replace("{", "").Replace("}", "");

            SignInExecuteLabel.Text = AppLogic.GetString("signin.aspx.2", SkinID, ThisCustomer.LocaleSetting);

            string sReturnURL = FormsAuthentication.GetRedirectUrl(CustomerGUID, PersistLogin.Checked);
            FormsAuthentication.SetAuthCookie(CustomerGUID, PersistLogin.Checked);

            if (sReturnURL.Length == 0)
            {
                sReturnURL = ReturnURL.Text;
            }
            if (sReturnURL.Length == 0 || sReturnURL == "signin.aspx")
            {
                if (DoingCheckout.Checked)
                {
                    sReturnURL = "shoppingcart.aspx";
                }
                else
                {
                    sReturnURL = "default.aspx";
                }
            }
            Response.AddHeader("REFRESH", "1; URL=" + Server.UrlDecode(sReturnURL));
        }

        protected void btnRequestNewPassword_Click(object sender, EventArgs e)
        {
            ErrorPanel.Visible = true; // that is where the status msg goes, in all cases in this routine

            ErrorMsgLabel.Text = String.Empty;

            string EMail = ForgotEMail.Text;

            if (EMail.Length == 0)
            {
                ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.4", SkinID, ThisCustomer.LocaleSetting);
                return;
            }

            ErrorMsgLabel.Text = "Email: " + EMail;
            Customer c = new Customer(EMail);

            if (!c.IsRegistered)
            {
                ErrorMsgLabel.Text = "<font color=\"#FF0000\"><b>" + AppLogic.GetString("signin.aspx.25", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "</b></font>";
                return;
            }
            else
            {
                Password p;
                if (c.IsAdminUser || c.IsAdminSuperUser)
                {
                    p = new RandomStrongPassword();
                }
                else
                {
                    p = new RandomPassword();
                }
                bool SendWasOk = false;
                try
                {
                    String FromEMail = AppLogic.AppConfig("MailMe_FromAddress");
                    String PackageName = AppLogic.AppConfig("XmlPackage.LostPassword");
                    XmlPackage2 pkg = new XmlPackage2(PackageName, ThisCustomer, SkinID, String.Empty, "newpwd=" + p.ClearPassword.Replace("&", "*") + "&thiscustomerid=" + ThisCustomer.CustomerID.ToString());
                    String MsgBody = pkg.TransformString();
                    AppLogic.SendMail(AppLogic.AppConfig("StoreName") + " " + AppLogic.GetString("lostpassword.aspx.6", SkinID, ThisCustomer.LocaleSetting), MsgBody, true, FromEMail, FromEMail, EMail, EMail, "", AppLogic.MailServer());
                    SendWasOk = true;
                    object lockuntil = DateTime.Now.AddMinutes(-1);
                    c.UpdateCustomer(
                        /*CustomerLevelID*/ null,
                        /*EMail*/ null,
                        /*SaltedAndHashedPassword*/ p.SaltedPassword,
                        /*SaltKey*/ p.Salt,
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
                        /*LockedUntil*/ lockuntil,
                        /*AdminCanViewCC*/ null,
                        /*BadLogin*/ -1,
                        /*Active*/ null,
                        /*PwdChangeRequired*/ 1,
                        /*RegisterDate*/ null
                    );
                }
                catch { }
                if (!SendWasOk)
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.3", SkinID, ThisCustomer.LocaleSetting);
                }
                else
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.2", SkinID, ThisCustomer.LocaleSetting);
                }
            }
        }

        protected void btnChgPwd_Click(object sender, EventArgs e)
        {
            String EMailField = CustomerEmail.Text.ToLowerInvariant();
            String PasswordField = OldPassword.Text;
            String newpwd = NewPassword.Text;
            String newencryptedpwd = "";

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

            string sReturnURL = FormsAuthentication.GetRedirectUrl(CustomerGUID, PersistLogin.Checked);
            FormsAuthentication.SetAuthCookie(CustomerGUID, PersistLogin.Checked);

            if (sReturnURL.Length == 0)
            {
                sReturnURL = ReturnURL.Text;
            }
            if (sReturnURL.Length == 0)
            {
                if (DoingCheckout.Checked)
                {
                    sReturnURL = "shoppingcart.aspx";
                }
                else
                {
                    sReturnURL = "default.aspx";
                }
            }
            Response.AddHeader("REFRESH", "1; URL=" + Server.UrlDecode(sReturnURL));
        }

        private bool ValidateNewPassword()
        {
            bool valPwd = false;
            if (NewPassword.Text.Replace("*", "").Trim().Length == 0)
            {
                return false;
            }

            if (NewPassword.Text == NewPassword2.Text)
            {
                try
                {
                    if (AppLogic.AppConfigBool("UseStrongPwd"))
                    {
                        Regex re = new Regex(AppLogic.AppConfig("CustomerPwdValidator"));
                        valPwd = re.IsMatch(NewPassword.Text);
                    }
                    else
                    {
                        valPwd = (NewPassword.Text.Length > 4);
                    }
                    if (!valPwd)
                    {
                        lblPwdChgErr.Text = "<br/><br/>" + AppLogic.GetString("account.aspx.7", SkinID, ThisCustomer.LocaleSetting);
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
                lblPwdChgErr.Text = "<br/><br/>" + AppLogic.GetString("createaccount.aspx.80", SkinID, ThisCustomer.LocaleSetting);
            }
            return valPwd;
        }


    }
}
