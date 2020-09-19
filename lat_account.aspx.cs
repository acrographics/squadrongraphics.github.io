// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/lat_account.aspx.cs 13    9/14/06 12:05a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    public partial class lat_account : SkinBase
    {
        string AffState = string.Empty;
        string AffCountry = string.Empty;
        int AffiliateID = 0;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            RequireSecurePage();
            AffiliateID = CommonLogic.CookieUSInt("LATAffiliateID");

            if (AffiliateID == 0 || !AppLogic.IsValidAffiliate(AffiliateID))
            {
                Response.Redirect("lat_signin.aspx?returnurl=" + Server.UrlEncode(CommonLogic.GetThisPageName(true) + "?" + CommonLogic.ServerVariables("QUERY_STRING")));
            }

            SectionTitle = "<a href=\"lat_account.aspx\">" + AppLogic.GetString("AppConfig.AffiliateProgramName", SkinID, ThisCustomer.LocaleSetting) + "</a> - Account Summary Page";

            lblErrorMsg.Text = "";
            lblNote.Text = "";
            if (!IsPostBack)
            {
                InitializePageContent();
            }
        }

        public void btnUpdate3_Click(object sender, EventArgs e)
        {
            ProcessAccountUpdate();
        }

        public void btnUpdate2_Click(object sender, EventArgs e)
        {
            ProcessAccountUpdate();
        }

        public void btnUpdate1_Click(object sender, EventArgs e)
        {
            ProcessAccountUpdate();
        }

        public void Country_DataBound(object sender, EventArgs e)
        {
            Country.Items.Insert(0, new ListItem(AppLogic.GetString("requestcatalog.aspx.20", SkinID, ThisCustomer.LocaleSetting), ""));
            int i = Country.Items.IndexOf(Country.Items.FindByValue(AffCountry));
            if (i == -1)
                Country.SelectedIndex = 0;
            else
                Country.SelectedIndex = i;
        }

        public void State_DataBound(object sender, EventArgs e)
        {
            State.Items.Insert(0, new ListItem(AppLogic.GetString("requestcatalog.aspx.20", SkinID, ThisCustomer.LocaleSetting), ""));
            int i = State.Items.IndexOf(State.Items.FindByValue(AffState));
            if (i == -1)
                State.SelectedIndex = 0;
            else
                State.SelectedIndex = i;
        }


        private void InitializePageContent()
        {
            AppConfigAffiliateProgramName.Text = AppLogic.GetString("AppConfig.AffiliateProgramName", SkinID, ThisCustomer.LocaleSetting) + " Member Sign-Out";
            imgLogOut.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/logout.gif");
            AskAQuestion.NavigateUrl = "mailto:" + AppLogic.AppConfig("AffiliateEMailAddress");


            //
            affiliateheader_small_gif.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/affiliateheader_small.jpg");
            AppConfig_AffiliateProgramName2.Text = String.Format(AppLogic.GetString("lataccount.aspx.31", SkinID, ThisCustomer.LocaleSetting),AppLogic.GetString("AppConfig.AffiliateProgramName", SkinID, ThisCustomer.LocaleSetting));
            AppConfig_AffiliateProgramName3.Text = String.Format(AppLogic.GetString("lataccount.aspx.32", SkinID, ThisCustomer.LocaleSetting),AppLogic.GetString("AppConfig.AffiliateProgramName", SkinID, ThisCustomer.LocaleSetting));
            AppConfig_AffiliateProgramName4.Text = String.Format(AppLogic.GetString("lataccount.aspx.30", SkinID, ThisCustomer.LocaleSetting),AppLogic.GetString("AppConfig.AffiliateProgramName", SkinID, ThisCustomer.LocaleSetting));

            tblAccount.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
            tblAcctInfoBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
            accountinfo_gif.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/accountinfo.gif");

            tblOnlineInfo.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
            tblOnlineInfoBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
            onlineinfo_gif.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/onlineinfo.gif");

            Affiliate a = new Affiliate(AffiliateID);
            if (a.AffiliateID != -1)
            {
                //Fill Account data fields
                FirstName.Text = a.FirstName;
                LastName.Text = a.LastName;
                EMail.Text = a.EMail.ToLowerInvariant().Trim();
                AppConfig_AffiliateProgramName2.Text = String.Format(AppLogic.GetString("lataccount.aspx.31", SkinID, ThisCustomer.LocaleSetting),AppLogic.GetString("AppConfig.AffiliateProgramName", SkinID, ThisCustomer.LocaleSetting));
                AffPassword.Text = String.Empty;
                AffPassword2.Text = String.Empty;
                Company.Text = Server.HtmlEncode(a.Company);
                Address1.Text = Server.HtmlEncode(a.Address1);
                Address2.Text = Server.HtmlEncode(a.Address2);
                Suite.Text = Server.HtmlEncode(a.Suite);
                City.Text = Server.HtmlEncode(a.City);

                AffState = a.State;
                IDataReader dr = DB.GetRS("select * from State  " + DB.GetNoLock() + " order by DisplayOrder,Name");
                State.DataSource = dr;
                State.DataTextField = "Name";
                State.DataValueField = "Abbreviation";
                State.DataBind();
                dr.Close();

                Zip.Text = Server.HtmlEncode(a.Zip);

                AffCountry = a.Country;
                IDataReader dr2 = DB.GetRS("select * from Country  " + DB.GetNoLock() + " where Published = 1 order by DisplayOrder,Name");
                Country.DataSource = dr2;
                Country.DataTextField = "Name";
                Country.DataValueField = "Name";
                Country.DataBind();
                dr2.Close();

                Phone.Text = Server.HtmlEncode(a.Phone);
                DOBTxt.Text = Localization.ToNativeShortDateString(a.DateOfBirth);

                //Website Data
                WebSiteName.Text = a.WebSiteName;
                WebSiteDescription.Text = a.WebSiteDescription;
                URL.Text = a.URL;

            }

            AppLogic.GetButtonDisable(btnUpdate1);
            AppLogic.GetButtonDisable(btnUpdate2);

        }
        
        private void ProcessAccountUpdate()
        {
            Affiliate a = new Affiliate(AffiliateID);
            Page.Validate();
            if (Page.IsValid && a.AffiliateID != -1)
            {
                try
                {
                    string pwd = null;
                    object saltkey = null;
                    if (AffPassword.Text.Trim().Length > 0)
                    {
                        Password p = new Password(AffPassword.Text, a.SaltKey);
                        pwd = p.SaltedPassword;
                        saltkey = p.Salt;
                    }

                    object dob = Localization.ParseNativeDateTime(DOBTxt.Text);
                    if((DateTime)dob == DateTime.MinValue)
                    {
                        dob = null;
                    }
                    
                    
                    String theUrl2 = CommonLogic.Left(URL.Text, 80);
                    if (theUrl2.IndexOf("http://") == -1 && theUrl2.Length != 0)
                    {
                        theUrl2 = "http://" + theUrl2;
                    }

                    string Name = FirstName.Text + " " + LastName.Text;
                    
                    a.Update(EMail.Text.ToLowerInvariant().Trim(), pwd, dob, null, null, CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("URL").Length == 0, 0, 1), CommonLogic.Left(FirstName.Text, 50), CommonLogic.Left(LastName.Text, 50), CommonLogic.Left(Name, 100), Company.Text, Address1.Text.Replace("\x0D\x0A", ""), Address2.Text.Replace("\x0D\x0A", ""), Suite.Text, City.Text, State.SelectedValue, Zip.Text, Country.SelectedValue, AppLogic.MakeProperPhoneFormat(Phone.Text), WebSiteName.Text, WebSiteDescription.Text, theUrl2, null, null, null, null, null, null, null, null, null, null, null, null, null, null, saltkey);

                    lblErrMsg.Text = "Account Updated";
                }
                catch
                {
                    lblErrMsg.Text = "<p><b>ERROR: There was an unknown error in updating your new account record. Please <a href=\"t-contact.aspx\">contact a service representative</a> for assistance.<br/><br/></b></p>";
                }

                AppLogic.SetSessionCookie("LATAffiliateID", a.AffiliateID.ToString());

                InitializePageContent();
            }
            else
            {
                lblErrorMsg.Text += "<br /><br /> Some errors occured trying to create your affiliate account.  Please correct them and try again.<br /><br />";
                foreach (IValidator aValidator in this.Validators)
                {
                    if (!aValidator.IsValid)
                    {
                        lblErrorMsg.Text += "&bull; " + aValidator.ErrorMessage + "<br />";
                    }
                }
                lblErrorMsg.Text += "<br />";
            }

        }

        protected void ValidatePassword(object source, ServerValidateEventArgs args)
        {
            if (AffPassword.Text.Trim() == "")
            {
                args.IsValid = true;
                return;
            }
            if (AffPassword.Text == AffPassword2.Text)
            {
                try
                {
                    valPwd.ErrorMessage = AppLogic.GetString("account.aspx.7", SkinID, ThisCustomer.LocaleSetting);

                    Regex re = new Regex(AppLogic.AppConfig("CustomerPwdValidator"));
                    if (re.IsMatch(AffPassword.Text))
                    {
                        args.IsValid = true;
                    }
                    else
                    {
                        args.IsValid = false;
                        valPwd.ErrorMessage = AppLogic.GetString("account.aspx.69", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
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
