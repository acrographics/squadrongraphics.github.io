// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/editaddress.aspx.cs 13    9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for editaddress.
    /// </summary>
    public partial class editaddress : SkinBase
    {

        bool Checkout = false;
        int AddressID = 0;
        AddressTypes AddressType = AddressTypes.Unknown;
        bool CanDelete = false;
        Address theAddress = new Address();
        readonly bool ValidateAddress = (AppLogic.AppConfig("VerifyAddressesProvider").Length > 0);
        string Prompt = string.Empty;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            RequireSecurePage();
            //RequiresLogin(CommonLogic.GetThisPageName(false) + "?" + CommonLogic.ServerVariables("QUERY_STRING"));

            Checkout = CommonLogic.QueryStringBool("checkout");
            String AddressTypeString = CommonLogic.QueryStringCanBeDangerousContent("AddressType");
            AddressID = CommonLogic.QueryStringUSInt("AddressID");
            theAddress.LoadFromDB(AddressID);
            Prompt = CommonLogic.QueryStringCanBeDangerousContent("Prompt");
            if (CommonLogic.QueryStringCanBeDangerousContent("RETURNURL") != "")
            {
                ViewState["RETURNURL"] = CommonLogic.QueryStringCanBeDangerousContent("RETURNURL");
            }

            if (!ThisCustomer.OwnsThisAddress(AddressID))
            {
                Response.Redirect("default.aspx");
            }

            AppLogic.CheckForScriptTag(AddressTypeString);

            SectionTitle = "<a href=\"selectaddress.aspx?checkout=" + Checkout.ToString() + "&AddressType=" + AddressTypeString + "\">" + String.Format(AppLogic.GetString("selectaddress.aspx.1", SkinID, ThisCustomer.LocaleSetting), AddressTypeString) + "</a> &rarr; ";
            SectionTitle += AppLogic.GetString("editaddress.aspx.1", SkinID, ThisCustomer.LocaleSetting);
                        
            //AddressType = (AddressTypes)Enum.Parse(typeof(AddressTypes), AddressTypeString, true);
            AddressType = DetermineAddressType(AddressTypeString);
            CanDelete = (0 == DB.GetSqlN(String.Format("select count(0) as N from ShoppingCart  " + DB.GetNoLock() + " where (ShippingAddressID={0} or BillingAddressID={0}) and CartType={1}", AddressID, (int)CartTypeEnum.RecurringCart)));

            if (!IsPostBack)
            {
                InitializePageContent();
            }
        }

        private AddressTypes DetermineAddressType(string addressTypeString)
        {
            AddressTypes type = AddressTypes.Billing;
            string compareString = addressTypeString.ToString().ToLowerInvariant();

            if(compareString == AddressTypes.Billing.ToString().ToLowerInvariant())
            {
                type = AddressTypes.Billing;
            }
            else if(compareString == AddressTypes.Shipping.ToString().ToLowerInvariant())
            {
                type = AddressTypes.Shipping;
            }
            else if(compareString == AddressTypes.Account.ToString().ToLowerInvariant())
            {
                type = AddressTypes.Account;
            }
            else if (compareString == AddressTypes.Unknown.ToString().ToLowerInvariant())
            {
                type = AddressTypes.Unknown;
            }
            else
            {
                // should default it to something...
                type = AddressTypes.Billing;
            }

            return type;
        }

        public void btnValidateAddress_Click(object sender, EventArgs e)
        {
            ProcessForm(true, Convert.ToInt32(((Button)sender).CommandArgument));
        }

        public void btnSaveAddress_Click(object sender, EventArgs e)
        {
            ProcessForm(false, Convert.ToInt32(((Button)sender).CommandArgument));
        }

        private void ProcessForm(bool UseValidationService, int AddressID)
        {
            ThisCustomer.RequireCustomerRecord();
            bool AllowShipToDifferentThanBillTo = AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo") && !AppLogic.AppConfigBool("SkipShippingOnCheckout");
            string ResidenceType = ddlResidenceType.SelectedValue;
            bool valid = true;
            string errormsg = string.Empty;

            //Address thisAddress = new Address(ThisCustomer.CustomerID);
            //thisAddress.AddressID = AddressID;
            //thisAddress.LoadFromDB(AddressID);

            /*
            //Validate required fields
            //if (CommonLogic.FormCanBeDangerousContent("AddressNickName").Trim() == "")
            //{
            //    valid = false;
            //    errormsg += "&bull;Address NickName is required<br/>";
            //}
            if (CommonLogic.FormCanBeDangerousContent("AddressFirstName").Trim() == "")
            {
                valid = false;
                errormsg += "&bull;First Name is required<br/>";
            }
            if (CommonLogic.FormCanBeDangerousContent("AddressLastName").Trim() == "")
            {
                valid = false;
                errormsg += "&bull;Last Name is required<br/>";
            }
            if (CommonLogic.FormCanBeDangerousContent("AddressPhone").Trim() == "")
            {
                valid = false;
                errormsg += "&bull;Phone is required<br/>";
            }
            if (CommonLogic.FormCanBeDangerousContent("AddressAddress1").Trim() == "")
            {
                valid = false;
                errormsg += "&bull;Address1 is required<br/>";
            }
            if (CommonLogic.FormCanBeDangerousContent("AddressCity").Trim() == "")
            {
                valid = false;
                errormsg += "&bull;City is required<br/>";
            }
            if (CommonLogic.FormCanBeDangerousContent("AddressState").Trim() == "")
            {
                valid = false;
                errormsg += "&bull;State is required<br/>";
            }
            if (CommonLogic.FormCanBeDangerousContent("AddressZip").Trim() == "")
            {
                valid = false;
                errormsg += "&bull;Zip is required<br/>";
            }
            if (CommonLogic.FormCanBeDangerousContent("AddressCountry").Trim() == "")
            {
                valid = false;
                errormsg += "&bull;Country is required<br/>";
            }
            */

            // Payment method validations
            if (AddressType == AddressTypes.Billing)
            {
                string paymentMethodLastUsed = AppLogic.CleanPaymentMethod(CommonLogic.FormCanBeDangerousContent("PaymentMethod"));
                if (paymentMethodLastUsed == AppLogic.ro_PMECheck)
                {
                    if (string.IsNullOrEmpty(CommonLogic.FormCanBeDangerousContent("ECheckBankABACode")))
                    {
                        valid = false;
                        errormsg += "&bull;Bank ABA Code is required<br/>";
                    }
                    if (string.IsNullOrEmpty(CommonLogic.FormCanBeDangerousContent("ECheckBankAccountNumber")))
                    {
                        valid = false;
                        errormsg += "&bull;Bank Account Number is required<br/>";
                    }
                    if (string.IsNullOrEmpty(CommonLogic.FormCanBeDangerousContent("ECheckBankName")))
                    {
                        valid = false;
                        errormsg += "&bull;Bank Account Name is required<br/>";
                    }
                    if (string.IsNullOrEmpty(CommonLogic.FormCanBeDangerousContent("ECheckBankAccountName")))
                    {
                        valid = false;
                        errormsg += "&bull;Bank Account Name is required<br/>";
                    }
                }
                if (paymentMethodLastUsed == AppLogic.ro_PMCreditCard)
                {
                    if (string.IsNullOrEmpty(CommonLogic.FormCanBeDangerousContent("CardName")))
                    {
                        valid = false;
                        errormsg += "&bull;Card Name is required<br/>";
                    }
                    if (string.IsNullOrEmpty(CommonLogic.FormCanBeDangerousContent("CardType")))
                    {
                        valid = false;
                        errormsg += "&bull;Card Type is required<br/>";
                    }
                    if(string.IsNullOrEmpty(CommonLogic.FormCanBeDangerousContent("CardNumber")))
                    {
                        valid = false;
                        errormsg += "&bull;Card Number is required<br/>";
                    }
                    
                    int iexpMonth = 0;
                    int iexpYear = 0;
                    string expMonth = CommonLogic.FormCanBeDangerousContent("CardExpirationMonth");
                    string expYear = CommonLogic.FormCanBeDangerousContent("CardExpirationYear");

                    if (string.IsNullOrEmpty(expMonth) ||
                        !int.TryParse(expMonth, out iexpMonth) ||
                        !(iexpMonth > 0))
                    {
                        valid = false;
                        errormsg += "&bull;Please select the Card Expiration Month<br/>";
                    }
                    if (string.IsNullOrEmpty(expYear) ||
                        !int.TryParse(expYear, out iexpYear) ||
                        !(iexpYear > 0))
                    {
                        valid = false;
                        errormsg += "&bull;Please select the Card Expiration Year<br/>";
                    }
                }
            }

            if (!Page.IsValid || !valid)
            {
                ErrorMsgLabel.Text = "<br /><br /> Some errors occured trying to update your address.  Please correct them and try again.<br /><br />";
                foreach (IValidator aValidator in this.Validators)
                {
                    if (!aValidator.IsValid)
                    {
                        ErrorMsgLabel.Text += "&bull; " + aValidator.ErrorMessage + "<br />";
                    }
                }
                ErrorMsgLabel.Text += "<br />";
                ErrorMsgLabel.Text += errormsg;
                InitializePageContent();
                return;
            }

            theAddress.AddressType = AddressType;
            theAddress.NickName = txtAddressNickName.Text;
            theAddress.FirstName = txtFirstName.Text;
            theAddress.LastName = txtLastName.Text;
            theAddress.Company = txtCompany.Text;
            theAddress.Address1 = txtAddress1.Text;
            theAddress.Address2 = txtAddress2.Text;
            theAddress.Suite = txtSuite.Text;
            theAddress.City = txtCity.Text;
            theAddress.State = ddlState.SelectedValue;
            theAddress.Zip = txtZip.Text;
            theAddress.Country = ddlCountry.SelectedValue;
            theAddress.Phone = txtPhone.Text;
            if (ResidenceType == "2")
            {
                theAddress.ResidenceType = ResidenceTypes.Commercial;
            }
            else if (ResidenceType == "1")
            {
                theAddress.ResidenceType = ResidenceTypes.Residential;
            }
            else
            {
                theAddress.ResidenceType = ResidenceTypes.Unknown;
            }
            if (theAddress.AddressType == AddressTypes.Billing)
            {
                theAddress.PaymentMethodLastUsed = AppLogic.CleanPaymentMethod(CommonLogic.FormCanBeDangerousContent("PaymentMethod"));
                if (theAddress.PaymentMethodLastUsed == AppLogic.ro_PMECheck)
                {
                    theAddress.ECheckBankABACode = CommonLogic.FormCanBeDangerousContent("ECheckBankABACode");
                    theAddress.ECheckBankAccountNumber = CommonLogic.FormCanBeDangerousContent("ECheckBankAccountNumber");
                    theAddress.ECheckBankName = CommonLogic.FormCanBeDangerousContent("ECheckBankName");
                    theAddress.ECheckBankAccountName = CommonLogic.FormCanBeDangerousContent("ECheckBankAccountName");
                    theAddress.ECheckBankAccountType = CommonLogic.FormCanBeDangerousContent("ECheckBankAccountType");
                }
                if (theAddress.PaymentMethodLastUsed == AppLogic.ro_PMCreditCard)
                {
                    theAddress.CardName = CommonLogic.FormCanBeDangerousContent("CardName");
                    theAddress.CardType = CommonLogic.FormCanBeDangerousContent("CardType");

                    string tmpS = CommonLogic.FormCanBeDangerousContent("CardNumber");
                    if (!tmpS.StartsWith("*"))
                    {
                        theAddress.CardNumber = tmpS;
                    }
                    theAddress.CardExpirationMonth = CommonLogic.FormCanBeDangerousContent("CardExpirationMonth");
                    theAddress.CardExpirationYear = CommonLogic.FormCanBeDangerousContent("CardExpirationYear");
                }
            }
            theAddress.UpdateDB();

            string RETURNURL = "";
            if (ViewState["RETURNURL"] != null)
            {
                RETURNURL = "&ReturnUrl=" + ViewState["RETURNURL"].ToString();
            }
            if (UseValidationService)
            {

                Address StandardizedAddress = new Address();
                String ValidateResult = AddressValidation.RunValidate(theAddress, out StandardizedAddress);
                theAddress = StandardizedAddress;
                theAddress.UpdateDB();

                if (ValidateResult != AppLogic.ro_OK)
                {
                    Response.Redirect("editaddress.aspx?Checkout=" + Checkout.ToString() + "&AddressType=" + AddressType.ToString() + "&AddressID=" + AddressID.ToString() + "&prompt=" + ValidateResult + RETURNURL);
                }
            }

            Response.Redirect(String.Format("selectaddress.aspx?Checkout={0}&AddressType={1}" + RETURNURL, Checkout.ToString(), AddressType));
        }
        
        public void btnDeleteAddress_Click(object sender, EventArgs e)
        {
            int DeleteAddressID = Convert.ToInt32(((Button)sender).CommandArgument);
            if (DeleteAddressID != 0)
            {
                Address adr = new Address();
                adr.LoadFromDB(DeleteAddressID);
                // make sure ok to delete:
                if (ThisCustomer.CustomerID == adr.CustomerID || ThisCustomer.IsAdminUser)
                {
                    Address.DeleteFromDB(DeleteAddressID, ThisCustomer.CustomerID);
                }
                Response.Redirect(String.Format("selectaddress.aspx?Checkout={0}&AddressType={1}", Checkout.ToString(), AddressType));
            }
        }

        public void ddlCountry_OnChange(object sender, EventArgs e)
        {
            string sql = String.Empty;
            if (ddlCountry.SelectedIndex > 0)
            {
                sql = "select s.* from dbo.State s " + DB.GetNoLock() + " join dbo.country c on s.countryid = c.countryid where c.name = " + DB.SQuote(ddlCountry.SelectedValue) + " order by s.DisplayOrder,s.Name";
            }
            else
            {
                sql = "select * from dbo.State  " + DB.GetNoLock() + " where countryid=(select countryid from country where name='United States') order by DisplayOrder,Name";
            }

            SetStateList(ddlCountry.SelectedValue);
        }

        private void InitializePageContent()
        {
            lblError.Visible = (lblError.Text.Trim() != "");
            pnlCheckoutImage.Visible = Checkout;
            CheckoutImage.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_2.gif");
            tblAddressList.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
            tblAddressListBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
            editaddress_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/editaddress.gif");
            litAddressPrompt.Text = CommonLogic.IIF(AddressType == AddressTypes.Shipping, AppLogic.GetString("editaddress.aspx.2", SkinID, ThisCustomer.LocaleSetting), AppLogic.GetString("editaddress.aspx.12", SkinID, ThisCustomer.LocaleSetting));
            if (Prompt.Length > 0)
            {
                litAddressPrompt.Text += "<br /><strong><font color=\"red\">" + Prompt + "</font></strong>";
            }
            //litAddressForm.Text = theAddress.InputHTML();
            bool CustCCRequired = ThisCustomer.MasterShouldWeStoreCreditCardInfo;
            pnlBillingData.Visible = (AddressType == AddressTypes.Billing && CustCCRequired);
            editaddress_aspx_4.Text = AppLogic.GetString("editaddress.aspx.4", SkinID, ThisCustomer.LocaleSetting);
            editaddress_aspx_5.Text = AppLogic.GetString("editaddress.aspx.5", SkinID, ThisCustomer.LocaleSetting);
            editaddress_aspx_6.Text = AppLogic.GetString("editaddress.aspx.6", SkinID, ThisCustomer.LocaleSetting);
            //pnlCCData.Visible = (theAddress.CardNumber.Length != 0);
            //pnlEcheckData.Visible = (theAddress.ECheckBankAccountNumber.Length != 0);
            editaddress_aspx_7.Text = AppLogic.GetString("editaddress.aspx.7", SkinID, ThisCustomer.LocaleSetting);
            litCCForm.Text = theAddress.InputCardHTML(ThisCustomer, false, false);
            editaddress_aspx_8.Text = AppLogic.GetString("editaddress.aspx.8", SkinID, ThisCustomer.LocaleSetting);
            litECheckForm.Text = theAddress.InputECheckHTML(false);
            btnSaveAddress.Text = AppLogic.GetString("editaddress.aspx.9", SkinID, ThisCustomer.LocaleSetting);
            btnSaveAddress.CommandArgument = AddressID.ToString();
            btnDeleteAddress.Visible = CanDelete;
            btnDeleteAddress.CommandArgument = AddressID.ToString();
            btnDeleteAddress.Text = AppLogic.GetString("editaddress.aspx.10", SkinID, ThisCustomer.LocaleSetting);
            pnlEcheckData.Attributes.Add("style", "display:none;");
            pnlCCData.Attributes.Add("style", "display:none;");
            btnValidateAddress.Text = AppLogic.GetString("editaddress.aspx.14", SkinID, ThisCustomer.LocaleSetting);
            btnValidateAddress.CommandArgument = AddressID.ToString();
            btnValidateAddress.Visible = ValidateAddress;
            lblValidateAddressSpacer.Visible = ValidateAddress;

            if(!IsPostBack && CustCCRequired)
            {
                CreditCard.Checked = true;
            }

            if (CreditCard.Checked || ECheck.Checked)
            {
                if (CreditCard.Checked)
                {
                    CreditCard.Checked = true;
                    pnlCCData.Attributes.Add("style", "display:block;");
                    pnlEcheckData.Attributes.Add("style", "display:none;");
                }
                else if (ECheck.Checked)
                {
                    ECheck.Checked = true;
                    pnlCCData.Attributes.Add("style", "display:none;");
                    pnlEcheckData.Attributes.Add("style", "display:block;");
                }
            }
            else
            {
                if (theAddress.PaymentMethodLastUsed == AppLogic.ro_PMCreditCard)
                {
                    CreditCard.Checked = true;
                    pnlCCData.Attributes.Add("style", "display:block;");
                }
                else if (theAddress.PaymentMethodLastUsed == AppLogic.ro_PMECheck)
                {
                    ECheck.Checked = true;
                    pnlEcheckData.Attributes.Add("style", "display:block;");
                }
            }

            txtAddressNickName.Text = theAddress.NickName;
            txtFirstName.Text = theAddress.FirstName;
            txtLastName.Text = theAddress.LastName;
            txtPhone.Text = theAddress.Phone;
            txtCompany.Text = theAddress.Company;
            ddlResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.55", SkinID, ThisCustomer.LocaleSetting), ((int)ResidenceTypes.Unknown).ToString()));
            ddlResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.56", SkinID, ThisCustomer.LocaleSetting), ((int)ResidenceTypes.Residential).ToString()));
            ddlResidenceType.Items.Add(new ListItem(AppLogic.GetString("address.cs.57", SkinID, ThisCustomer.LocaleSetting), ((int)ResidenceTypes.Commercial).ToString()));
            ddlResidenceType.SelectedValue = ((int)theAddress.ResidenceType).ToString();
            txtAddress1.Text = theAddress.Address1;
            txtAddress2.Text = theAddress.Address2;
            txtSuite.Text = theAddress.Suite;
            txtCity.Text = theAddress.City;
            txtZip.Text = theAddress.Zip;


            IDataReader dr = DB.GetRS("select * from Country  " + DB.GetNoLock() + " where Published = 1 order by DisplayOrder,Name");
            ddlCountry.DataSource = dr;
            ddlCountry.DataTextField = "Name";
            ddlCountry.DataValueField = "Name";
            ddlCountry.DataBind();
            dr.Close();
            ddlCountry.SelectedValue = theAddress.Country;
            SetStateList(theAddress.Country);
            ddlState.SelectedValue = theAddress.State;

            GetJS();
        }

        private void GetJS()
        {
            StringBuilder js = new StringBuilder("<script type=\"text/javascript\">");
            js.Append("function ShowPaymentInput(theRadio)");
            js.Append("{");
            js.Append("if (theRadio.value == 'none')");
            js.Append("{");
            js.Append("document.getElementById('" + pnlEcheckData.ClientID + "').style.display = 'none';");
            js.Append("document.getElementById('" + pnlCCData.ClientID + "').style.display = 'none';");
            js.Append("}");
            js.Append("else if (theRadio.value == 'ECheck')");
            js.Append("{");
            js.Append("document.getElementById('" + pnlEcheckData.ClientID + "').style.display = '';");
            js.Append("document.getElementById('" + pnlCCData.ClientID + "').style.display = 'none';");
            js.Append("}");
            js.Append("else");
            js.Append("{");
            js.Append("document.getElementById('" + pnlEcheckData.ClientID + "').style.display = 'none';");
            js.Append("document.getElementById('" + pnlCCData.ClientID + "').style.display = '';");
            js.Append("}");
            js.Append("return true;");
            js.Append("}");
            js.Append("</script> ");
            ClientScriptManager cs = this.ClientScript;
            cs.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), js.ToString());
        }

        private void SetStateList(string Country)
        {
            string sql = String.Empty;
            if (Country.Length > 0)
            {
                sql = "select s.* from dbo.State s " + DB.GetNoLock() + " join dbo.country c on s.countryid = c.countryid where c.name = " + DB.SQuote(Country) + " order by s.DisplayOrder,s.Name";
            }
            else
            {
                sql = "select * from dbo.State  " + DB.GetNoLock() + " where countryid = 222 order by DisplayOrder,Name";
            }

            ddlState.ClearSelection();
            IDataReader dr = DB.GetRS(sql);
            ddlState.DataSource = dr;
            ddlState.DataTextField = "Name";
            ddlState.DataValueField = "Abbreviation";
            ddlState.DataBind();
            dr.Close();
            if (ddlState.Items.Count == 0)
            {
                ddlState.Items.Insert(0, new ListItem("Other (Non U.S.)", "--"));
                ddlState.SelectedIndex = 0;
            }
        }

        override protected void OnInit(EventArgs e)
        {
            if(CommonLogic.QueryStringBool("recurringedit"))
            {
                SetTemplate("empty.ascx");
            }
            base.OnInit(e);
        }
    }
}
