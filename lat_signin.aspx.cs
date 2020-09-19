// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/lat_signin.aspx.cs 14    9/30/06 10:27p Buddy $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Web;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for lat_signin.
    /// </summary>
    public partial class lat_signin : SkinBase
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            RequireSecurePage();

            int AffiliateID = CommonLogic.CookieUSInt("LATAffiliateID");


            // this may be overwridden by the XmlPackage below!
            SectionTitle = "<a href=\"lat_account.aspx\">" + AppLogic.GetString("AppConfig.AffiliateProgramName", SkinID, ThisCustomer.LocaleSetting) + "</a> - Sign In";

            String ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("returnurl");
            AppLogic.CheckForScriptTag(ReturnURL);
            if (!IsPostBack)
            {
                UpdatePageContent();
            }

        }


        private void UpdatePageContent()
        {
            AppConfig_AffiliateProgramName.Text = AppLogic.GetString("AppConfig.AffiliateProgramName", SkinID, ThisCustomer.LocaleSetting);
            AppConfig_AffiliateProgramName2.Text = AppLogic.GetString("AppConfig.AffiliateProgramName", SkinID, ThisCustomer.LocaleSetting);
            AppConfig_AffiliateProgramName3.Text = AppLogic.GetString("AppConfig.AffiliateProgramName", SkinID, ThisCustomer.LocaleSetting);
            AskAQuestion.NavigateUrl = "mailto:" + AppLogic.AppConfig("AffiliateEMailAddress");
            affiliateheader_small_gif.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/affiliateheader_small.jpg");

            if (CommonLogic.QueryStringCanBeDangerousContent("errormsg").Length != 0)
            {
                AppLogic.CheckForScriptTag(CommonLogic.QueryStringCanBeDangerousContent("errormsg"));
                lblErrMsg.Text = Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("errormsg")) + " Please Try Again:";
            }
            else
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("notemsg").Length != 0)
                {
                    AppLogic.CheckForScriptTag(CommonLogic.QueryStringCanBeDangerousContent("notemsg"));
                    lblNote.Text = Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("notemsg"));
                }
            }

            lnkSupportEmail.NavigateUrl = SE.MakeDriverLink("Contact");
            lblReqPwdErr.Text = "";

        }

        private void ProcessSignIn()
        {
            String EMailField = EMail.Text.ToLowerInvariant().Trim();
            String PasswordField = Password.Text;

            if (EMailField.Length == 0 || PasswordField.Length == 0)
            {
                lblErrMsg.Text = "Please enter both your e-mail and password<br/><br/>";
                return;
            }

            Affiliate a = new Affiliate(EMailField);
            Password p = new Password(PasswordField, a.SaltKey);

            if (a == null || a.Password != p.SaltedPassword)
            {
                Response.Redirect("lat_signin.aspx?errormsg=" + AppLogic.GetString("lat_signin_process.aspx.1", SkinID, ThisCustomer.LocaleSetting));
            }

            int AffiliateID = a.AffiliateID;

            // we've got a good login:
            AppLogic.SetSessionCookie("LATAffiliateID", AffiliateID.ToString());

            String AffiliateGUID = a.AffiliateGUID.ToString();

            lblSigninSuccess.Text = AppLogic.GetString("AppConfig.AffiliateProgramName", SkinID, ThisCustomer.LocaleSetting) + " sign-in complete, please wait...";
            pnlSigninSuccess.Visible = true;
            pnlMain.Visible = false;

            String ReturnURLx = Server.HtmlDecode(ReturnURL.Text);
            if (ReturnURLx.Length == 0)
            {
                ReturnURLx = "lat_account.aspx";
            }
            Response.AddHeader("REFRESH", "1; URL=" + Server.UrlDecode(ReturnURLx));
        }

        public void btnSignIn_Click(object sender, EventArgs e)
        {
            ProcessSignIn();
        }

        public void btnLostPassword_Click(object sender, EventArgs e)
        {
            if (ResetPwdEMail.Text.Trim().Length == 0)
            {
                lblReqPwdErr.Text = "Please enter a valid email address";
                return;
            }

            String AffName = AppLogic.GetString("AppConfig.AffiliateProgramName", 1, Localization.GetWebConfigLocale());
            Affiliate a = new Affiliate(ResetPwdEMail.Text);
            if (a.AffiliateID != -1)
            {
                Password p = new Password();
                a.Update(null, p.SaltedPassword, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, p.Salt);
                try
                {
                    AppLogic.SendMail(AffName + " Password", AppLogic.RunXmlPackage("notification.lostpassword.xml.config", null, ThisCustomer, ThisCustomer.SkinID, "", "thisaffiliateid=" + a.AffiliateID.ToString() + "&newpwd=" + p.ClearPassword + "&AffName=" + AffName, false, false), true, AppLogic.AppConfig("AffiliateEMailAddress"), AppLogic.AppConfig("AffiliateEMailAddress"), a.EMail, a.EMail, "", AppLogic.MailServer());
                    lblReqPwdErr.Text = "Your new password has been sent.";
                }
                catch
                {
                    lblReqPwdErr.Text = "There were problems emailing your password please try again later.";
                }
            }
            else
            {
                lblReqPwdErr.Text = "There is no registered " + AffName + " member with that e-mail address!";
            }
        }
    }
}
