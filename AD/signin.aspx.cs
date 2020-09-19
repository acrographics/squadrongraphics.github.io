// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2006.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/signin.aspx.cs 40    9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using AspDotNetStorefrontCommon;
using System.Text.RegularExpressions;

public partial class signin : System.Web.UI.Page
{
    private String LastSecurityCode = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");

        if (AppLogic.IsAdminSite && AppLogic.OnLiveServer() && AppLogic.UseSSL() && CommonLogic.ServerVariables("SERVER_PORT_SECURE") != "1")
        {
            if (AppLogic.RedirectLiveToWWW())
            {
                HttpContext.Current.Response.Redirect("https://www." + AppLogic.LiveServer() + CommonLogic.ServerVariables("PATH_INFO") + "?" + CommonLogic.ServerVariables("QUERY_STRING"));
            }
            else
            {
                HttpContext.Current.Response.Redirect("https://" + AppLogic.LiveServer() + CommonLogic.ServerVariables("PATH_INFO") + "?" + CommonLogic.ServerVariables("QUERY_STRING"));
            }
        }

        if (!IsPostBack)
        {
            
            this.loadHolders();
            if (AppLogic.AppConfigBool("SecurityCodeRequiredOnAdminLogin"))
            {
                Session["SecurityCode"] = CommonLogic.GenerateRandomCode(6);
            }
        }

        Label6.Text = AppLogic.GetString("signin.aspx.15", 1, Localization.GetWebConfigLocale());
        Label8.Text = AppLogic.GetString("signin.aspx.16", 1, Localization.GetWebConfigLocale());
        Label1.Text = AppLogic.GetString("signin.aspx.10", 1, Localization.GetWebConfigLocale());
        Label2.Text = AppLogic.GetString("signin.aspx.17", 1, Localization.GetWebConfigLocale());
        Label3.Text = AppLogic.GetString("signin.aspx.9", 1, Localization.GetWebConfigLocale());
        Label4.Text = AppLogic.GetString("signin.aspx.8", 1, Localization.GetWebConfigLocale());
        Label5.Text = AppLogic.GetString("signin.aspx.10", 1, Localization.GetWebConfigLocale());
        Label7.Text = AppLogic.GetString("signin.aspx.12", 1, Localization.GetWebConfigLocale());
        btnRequestNewPassword.Text = AppLogic.GetString("signin.aspx.18", 1, Localization.GetWebConfigLocale());

        txtEMail.Focus();
        if (AppLogic.AppConfigBool("SecurityCodeRequiredOnAdminLogin"))
        {
            SecurityImage.Visible = true;
            SecurityCode.Visible = true;
            RequiredFieldValidator4.Enabled = true;
            Label9.Visible = true;
            Label9.Text = AppLogic.GetString("signin.aspx.21", 1, Localization.GetWebConfigLocale());
            SecurityImage.ImageUrl = "jpegimage.aspx";
        }

    }


    protected void loadHolders()
    {
        ltStoreName.Text = AppLogic.AppConfig("StoreName");
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        int SkinID = 1;
        String EMailField = CommonLogic.FormCanBeDangerousContent("txtEMail").ToLowerInvariant().Trim();
        String PasswordField = CommonLogic.FormCanBeDangerousContent("txtPassword").Trim();

        bool LoginOK = false;

        if (AppLogic.AppConfigBool("SecurityCodeRequiredOnAdminLogin"))
        {
            String sCode = Session["SecurityCode"].ToString();
            String fCode = SecurityCode.Text;
            if (fCode != sCode)
            {
                ltError.Text = "Invalid Security Code|" + sCode + "|" + fCode + "|"; // AppLogic.GetString("lat_signin_process.aspx.1", SkinID, ThisCustomer.LocaleSetting);
                SecurityCode.Text = String.Empty;
                Session["SecurityCode"] = CommonLogic.GenerateRandomCode(6);
                return;
            }
        }

        Customer c = new Customer(EMailField, true);

        if (c.IsRegistered && (c.IsAdminUser || c.IsAdminSuperUser))
        {
            if (c.LockedUntil > DateTime.Now)
            {
                ltError.Text = AppLogic.GetString("lat_signin_process.aspx.3", SkinID, c.LocaleSetting);
                return;
            }
            else if (!c.Active)
            {
                ltError.Text = AppLogic.GetString("lat_signin_process.aspx.2", SkinID, c.LocaleSetting);
                return;
            }
            else if (c.PwdChanged.AddDays(AppLogic.AppConfigUSDouble("AdminPwdChangeDays")) < DateTime.Now || c.PwdChangeRequired)
            {
                lblPwdChgErrMsg.Text = AppLogic.GetString("lat_signin_process.aspx.4", SkinID, c.LocaleSetting);
                lblPwdChgErrMsg.Visible = true;
                pnlChangePwd.Visible = true;
                pnlSignIn.Visible = false;
                txtEmailNewPwd.Text = c.EMail;
                txtOldPwd.Focus();
                return;
            }
            else
            {
                LoginOK = c.CheckLogin(PasswordField);
            }
            if (!LoginOK)
            {
                Session["SecurityCode"] = CommonLogic.GenerateRandomCode(6);
                if (AppLogic.AppConfigBool("SecurityCodeRequiredOnAdminLogin"))
                {
                    SecurityCode.Text = "";
                    Session["SecurityCode"] = CommonLogic.GenerateRandomCode(6);
                }
                Security.LogEvent("Admin Site Login Failed", "Attempted login failed for email address " + EMailField, 0, 0, 0);
                ltError.Text = "<font color=\"#FF0000\"><b>" + AppLogic.GetString("lat_signin_process.aspx.1", c.SkinID, Localization.GetWebConfigLocale()) + "</b></font>";
                object lockuntil = null;
                int badlogin = 1;
                if ((c.BadLoginCount + 1) >= AppLogic.AppConfigNativeInt("MaxBadLogins"))
                {
                    lockuntil = DateTime.Now.AddMinutes(AppLogic.AppConfigUSInt("BadLoginLockTimeOut"));
                    badlogin = -1;
                    ltError.Text = AppLogic.GetString("lat_signin_process.aspx.3", SkinID, Localization.GetWebConfigLocale());
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
                return;
            }
        }
        else
        {
            Security.LogEvent("Invalid Admin Site Login", "An invalid login name was used: " + EMailField, 0, 0, 0);
            ltError.Text = "<font color=\"#FF0000\"><b>" + AppLogic.GetString("lat_signin_process.aspx.1", c.SkinID, Localization.GetWebConfigLocale()) + "</b></font>";
            return;
        }

        if (c.PwdChanged.AddDays(AppLogic.AppConfigUSDouble("AdminPwdChangeDays")) < DateTime.Now || c.PwdChangeRequired)
        {
            lblPwdChgErrMsg.Text = AppLogic.GetString("lat_signin_process.aspx.4", SkinID, c.LocaleSetting);
            lblPwdChgErrMsg.Visible = true;
            pnlChangePwd.Visible = true;
            pnlSignIn.Visible = false;
            txtEmailNewPwd.Focus();
            return;
        }


        // we've got a good login:
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

        //clear impersonation
        c.ThisCustomerSession.ClearVal("igd");

        string LocaleSetting = c.LocaleSetting;
        if (LocaleSetting.Length == 0)
        {
            LocaleSetting = Localization.GetWebConfigLocale();
        }
        LocaleSetting = Localization.CheckLocaleSettingForProperCase(LocaleSetting);
        String CustomerGUID = c.CustomerGUID.Replace("{", "").Replace("}", "");
        Security.LogEvent("Admin Site Login", "", c.CustomerID, c.CustomerID, c.ThisCustomerSession.SessionID);
        FormsAuthentication.SetAuthCookie(CustomerGUID, false);
        Response.Write("<script type=\"text/javascript\">top.location='default.aspx'</script>;");
    }

    protected void btnRequestNewPassword_Click(object sender, EventArgs e)
    {
        string FromEMail = AppLogic.AppConfig("MailMe_FromAddress");
        string EMail = CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("txtEMailRemind").Length > 0, CommonLogic.FormCanBeDangerousContent("txtEMailRemind"), CommonLogic.FormCanBeDangerousContent("txtEMail"));

        ltError.Text = "Email: " + EMail;

        Customer c = new Customer(EMail);

        if (!c.IsRegistered)
        {
            ltError.Text = "<font color=\"#FF0000\"><b>" + AppLogic.GetString("signin.aspx.admin.1", c.SkinID, c.LocaleSetting) + "</b></font>";
        }
        else if (AppLogic.MailServer().Length == 0 || AppLogic.MailServer() == AppLogic.ro_TBD)
        {
            ltError.Text = "<font color=\"#FF0000\"><b>" + AppLogic.GetString("signin.aspx.admin.2", c.SkinID, c.LocaleSetting) + "</b></font>";
        }
        else
        {
            Password p = new Password();
            bool SendWasOk = false;
            try
            {
                String PackageName = AppLogic.AppConfig("XmlPackage.LostPassword");
                XmlPackage2 pkg = new XmlPackage2(PackageName, c, c.SkinID, String.Empty, "newpwd=" + p.ClearPassword.Replace("&", "*") + "&thiscustomerid=" + c.CustomerID.ToString());
                String MsgBody = pkg.TransformString();
                AppLogic.SendMail(AppLogic.AppConfig("StoreName") + " " + AppLogic.GetString("lostpassword.aspx.6", c.SkinID, c.LocaleSetting), MsgBody, true, FromEMail, FromEMail, EMail, EMail, "", AppLogic.MailServer());
                SendWasOk = true;
                object lockeduntil = DateTime.Now.AddMinutes(-1);

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
                    /*LockedUntil*/ lockeduntil,
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
                ltError.Text = "<font color=\"#FF0000\"><b>" + AppLogic.GetString("lostpassword.aspx.3", c.SkinID, c.LocaleSetting) + "</b></font>";
            }
            else
            {
                ltError.Text = "<font color=\"#0000FF\"><b>" + AppLogic.GetString("lostpassword.aspx.2", c.SkinID, c.LocaleSetting) + "</b></font>";
            }
        }

    }

    protected void btnChangePwd_OnClick(object sender, EventArgs e)
    {
        Customer ThisCustomer = new Customer(txtEmailNewPwd.Text, true);
        string newpwd = txtNewPwd.Text;
        string oldpwd = txtOldPwd.Text;
        string confirmpwd = txtConfirmPwd.Text;
        txtNewPwd.Text = "";
        txtOldPwd.Text = "";
        txtConfirmPwd.Text = "";
        if (ThisCustomer.IsRegistered)
        {
            Password pwdold = new Password(oldpwd, ThisCustomer.SaltKey);
            Password pwdnew = new Password(newpwd, ThisCustomer.SaltKey);
            if (ThisCustomer.Password != pwdold.SaltedPassword)
            {
                lblPwdChgErrMsg.Text = "Invalid old password.";
                return;
            }
            if (newpwd != confirmpwd)
            {
                lblPwdChgErrMsg.Text = "The new password and the confirmation password do not match.";
                return;
            }
            Regex re = new Regex(AppLogic.AppConfig("CustomerPwdValidator"));
            if (!re.IsMatch(newpwd))
            {
                lblPwdChgErrMsg.Text = AppLogic.GetString("signin.aspx.26", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                return;
            }

            newpwd = pwdnew.SaltedPassword;
            oldpwd = pwdold.SaltedPassword;

            if (ThisCustomer.PwdPreviouslyUsed(newpwd))
            {
                lblPwdChgErrMsg.Text = "The new password has been previously used.  Please select a different password.";
                return;
            }

            ThisCustomer.UpdateCustomer(
                /*CustomerLevelID*/ null,
                /*EMail*/ null,
                /*SaltedAndHashedPassword*/ newpwd,
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
                /*BadLogin*/ null,
                /*Active*/ null,
                /*PwdChangeRequired*/ 0,
                /*RegisterDate*/ null
                   );

            Security.LogEvent("Admin Password Changed", "", ThisCustomer.CustomerID, ThisCustomer.CustomerID, 0);

            ThisCustomer.ThisCustomerSession.ClearVal("igd");

            string LocaleSetting = ThisCustomer.LocaleSetting;
            if (LocaleSetting.Length == 0)
            {
                LocaleSetting = Localization.GetWebConfigLocale();
            }
            LocaleSetting = Localization.CheckLocaleSettingForProperCase(LocaleSetting);
            String CustomerGUID = ThisCustomer.CustomerGUID.Replace("{", "").Replace("}", "");
            Security.LogEvent("Admin Site Login", "", ThisCustomer.CustomerID, ThisCustomer.CustomerID, ThisCustomer.ThisCustomerSession.SessionID);
            FormsAuthentication.SetAuthCookie(CustomerGUID, false);

            Response.Write("<script type=\"text/javascript\">top.location='default.aspx'</script>;");
        }
        else
        {
            lblPwdChgErrMsg.Text = "Please enter a valid administrator's email address.";
            return;
        }

    }
}
