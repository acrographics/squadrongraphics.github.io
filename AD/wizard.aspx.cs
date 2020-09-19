// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2006.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/wizard.aspx.cs 7     9/14/06 12:05a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{

    public partial class wizard : System.Web.UI.Page
    {
        protected Customer cust;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            if (!IsPostBack)
            {
                if (!cust.IsAdminSuperUser)
                {
                    this.resetError("Insufficient Permission!", true);
                    this.divMain.Visible = false;
                }
                else
                {
                    this.divMain.Visible = true;
                    this.loadData();
                }

                EncryptWebConfigRow.Visible = (AppLogic.TrustLevel == AspNetHostingPermissionLevel.Unrestricted);
            }
        }

        protected void loadData()
        {
            bool BadSSL = CommonLogic.QueryStringBool("BadSSL");
            if (BadSSL)
            {
                resetError("No SSL certificate was found on your site. Please check with your hosting company! You must be able to invoke your store site using https:// before turning SSL on in this admin site!", false);
            }

            this.txtStoreName.Text = AppLogic.AppConfig("StoreName");
            this.txtStoreDomain.Text = AppLogic.LiveServer();
            this.txtStoreEmail.Text = AppLogic.AppConfig("MailMe_FromAddress");
            this.txtEmailName.Text = AppLogic.AppConfig("MailMe_FromName");
            this.txtZip.Text = AppLogic.AppConfig("RTShipping.OriginZip");
            this.ddMode.Items.FindByValue(AppLogic.TransactionMode()).Selected = true;
            this.txtCurrency.Text = Localization.StoreCurrency();
            this.txtCurrencyNumeric.Text = Localization.StoreCurrencyNumericCode();
            String PM = AppLogic.AppConfig("PaymentMethods").ToUpperInvariant();
            foreach (ListItem li in this.cblPaymentMethods.Items)
            {
                if (("," + PM + ",").IndexOf("," + li.Value.ToUpperInvariant() + ",") != -1)
                {
                    li.Selected = true;
                }
                if (li.Text.ToUpperInvariant() == AppLogic.ro_PMMicropay && AppLogic.AppConfigBool("Micropay.Enabled"))
                {
                    li.Selected = true;
                }
            }

            String GW = AppLogic.ActivePaymentGatewayCleaned();
            foreach (ListItem li in this.rblGateway.Items)
            {
                if (GW.Equals(li.Value))
                {
                    li.Selected = true;
                }
            }

            bool live = AppLogic.AppConfigBool("UseLiveTransactions");
            this.rblLiveTransactions.Items.FindByValue(live.ToString().ToLowerInvariant()).Selected = true;

            bool ssl = AppLogic.UseSSL();
            this.rblSSL.Items.FindByValue(ssl.ToString().ToLowerInvariant()).Selected = true;

            if (AppLogic.TrustLevel == AspNetHostingPermissionLevel.Unrestricted)
            {
                Configuration webconfig = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
                AppSettingsSection appsettings = (AppSettingsSection)webconfig.GetSection("appSettings");
                rblEncrypt.Items.FindByValue(appsettings.SectionInformation.IsProtected.ToString().ToLowerInvariant()).Selected = true;
            }
        }

        protected void resetError(string error, bool isError)
        {
            string str = "<font class=\"noticeMsg\">NOTICE:</font>&nbsp;&nbsp;&nbsp;";
            if (isError)
                str = "<font class=\"errorMsg\">ERROR:</font>&nbsp;&nbsp;&nbsp;";

            if (error.Length > 0)
                str += error + "";
            else
                str = "";

            this.ltError.Text = str;
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            bool BadSSL = CommonLogic.QueryStringBool("BadSSL");

            // save the config settings:
            AppLogic.SetAppConfig("RTShipping.OriginZip", this.txtZip.Text, false);
            AppLogic.SetAppConfig("LiveServer", this.txtStoreDomain.Text, false);
            AppLogic.SetAppConfig("MailMe_FromAddress", this.txtStoreEmail.Text, false);
            AppLogic.SetAppConfig("MailMe_FromName", this.txtEmailName.Text, false);
            AppLogic.SetAppConfig("MailMe_ToAddress", this.txtStoreEmail.Text, false);
            AppLogic.SetAppConfig("MailMe_ToName", this.txtEmailName.Text, false);
            if (AppLogic.AppConfig("MailMe_Server").ToLowerInvariant() == "mail.yourdomain.com")
            {
                AppLogic.SetAppConfig("MailMe_Server", "mail." + this.txtStoreDomain.Text, false);
            }
            AppLogic.SetAppConfig("GotOrderEMailFrom", this.txtStoreEmail.Text, false);
            AppLogic.SetAppConfig("GotOrderEMailFromName", this.txtEmailName.Text, false);
            AppLogic.SetAppConfig("GotOrderEMailTo", this.txtStoreEmail.Text, false);
            AppLogic.SetAppConfig("ReceiptEMailFrom", this.txtStoreEmail.Text, false);
            AppLogic.SetAppConfig("ReceiptEMailFromName", this.txtEmailName.Text, false);

            if (AppLogic.TrustLevel == AspNetHostingPermissionLevel.Unrestricted)
            {
                string encyptionprovider = AppLogic.AppConfig("Web.Config.EncryptionProvider");
                if (encyptionprovider != "DataProtectionConfigurationProvider" && encyptionprovider != "RsaProtectedConfigurationProvider")
                {
                    encyptionprovider = "DataProtectionConfigurationProvider";
                }
                Configuration webconfig = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
                AppSettingsSection appsettings = (AppSettingsSection)webconfig.GetSection("appSettings");
                if (rblEncrypt.SelectedValue.ToLowerInvariant() == "true" && !appsettings.SectionInformation.IsProtected)
                {
                    appsettings.SectionInformation.ProtectSection(encyptionprovider);
                    appsettings.SectionInformation.ForceSave = true;
                    webconfig.Save(ConfigurationSaveMode.Full);
                }
                else if (rblEncrypt.SelectedValue.ToLowerInvariant() == "false" && appsettings.SectionInformation.IsProtected)
                {
                    appsettings.SectionInformation.UnprotectSection();
                    appsettings.SectionInformation.ForceSave = true;
                    webconfig.Save(ConfigurationSaveMode.Full);
                }
            }

            if (this.rblSSL.SelectedValue.ToLowerInvariant() == "true")
            {
                bool OkToUseSSL = false;
                String WorkerWindowInSSL = String.Empty;
                try
                {
                    WorkerWindowInSSL = CommonLogic.AspHTTP(AppLogic.GetStoreHTTPLocation(false).Replace("http://", "https://") + "empty.htm",10);
                }
                catch { }
                if (WorkerWindowInSSL.IndexOf("Worker") != -1)
                {
                    OkToUseSSL = true;
                }
                if (OkToUseSSL)
                {
                    AppLogic.AppConfigTable["UseSSL"].ConfigValue = "true";
                }
                else
                {
                    BadSSL = true;
                    AppLogic.AppConfigTable["UseSSL"].ConfigValue = "false";
                }
            }
            else
            {
                AppLogic.AppConfigTable["UseSSL"].ConfigValue =  "false";
            }
            AppLogic.AppConfigTable["UseLiveTransactions"].ConfigValue  = this.rblLiveTransactions.SelectedValue;

            AppLogic.AppConfigTable["TransactionMode"].ConfigValue = this.ddMode.SelectedValue;

            AppLogic.AppConfigTable["Localization.StoreCurrency"].ConfigValue = this.txtCurrency.Text;
            AppLogic.AppConfigTable["Localization.StoreCurrencyNumericCode"].ConfigValue = this.txtCurrencyNumeric.Text;

            AppLogic.AppConfigTable["StoreName"].ConfigValue = this.txtStoreName.Text;
            AppLogic.SetAppConfig("SE_MetaTitle", this.txtStoreName.Text, false);
            AppLogic.SetAppConfig("Dispatch_SiteName", this.txtStoreName.Text, false);

            string temp = "";
            foreach (ListItem li in this.cblPaymentMethods.Items)
            {
                if (li.Selected)
                {
                    temp += "," + li.Value;
                }
            }
            AppLogic.AppConfigTable["PaymentMethods"].ConfigValue = CommonLogic.IIF(temp.Length > 1, temp.Substring(1), "");
            if (temp.ToUpperInvariant().IndexOf(AppLogic.ro_PMMicropay) != -1)
            {
                AppLogic.AppConfigTable["Micropay.Enabled"].ConfigValue =  "true"; // preserve setting of obsolete appconfig
            }
            else
            {
                AppLogic.AppConfigTable["Micropay.Enabled"].ConfigValue = "false"; // preserve setting of obsolete appconfig
            }
            AppLogic.AppConfigTable["PaymentGateway"].ConfigValue = this.rblGateway.SelectedValue;

            if (AppLogic.AppConfig("OrderShowCCPwd") == "WIZARD")
            {
                AppLogic.AppConfigTable["OrderShowCCPwd"].ConfigValue = CommonLogic.GetRandomNumber(1000, 1000000).ToString() + CommonLogic.GetRandomNumber(1000, 1000000).ToString() + CommonLogic.GetRandomNumber(1000, 1000000).ToString();
            }

            AppLogic.SetAppConfig("WizardRun", "true", false);

            AppLogic.ClearCache();
            this.resetError("Configuration Wizard completed successfully.", false);

            this.loadData();
        }
    }
}