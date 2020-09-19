// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
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
using AspDotNetStorefrontGateways;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for editaddressrecurring.
    /// </summary>
    public partial class editaddressrecurring : SkinBase
    {

        int AddressID = 0;
        int OriginalRecurringOrderNumber = 0;
        Address theAddress = new Address();

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            AddressID = CommonLogic.QueryStringUSInt("AddressID");
            OriginalRecurringOrderNumber = CommonLogic.QueryStringUSInt("OriginalRecurringOrderNumber");
            theAddress.LoadFromDB(AddressID);

            if (!ThisCustomer.OwnsThisAddress(AddressID))
            {
                throw new ArgumentException("Permission Denied");
            }

            if (!IsPostBack)
            {
                InitializePageContent();
            }
            else
            {
                OriginalRecurringOrderNumber = Localization.ParseNativeInt(ltOriginalRecurringOrderNumber.Text);
            }
        }

        public void btnSaveAddress_Click(object sender, EventArgs e)
        {
            ProcessForm(false, Convert.ToInt32(((Button)sender).CommandArgument));
        }

        private void ProcessForm(bool UseValidationService, int AddressID)
        {
            string ResidenceType = ddlResidenceType.SelectedValue;
            bool valid = true;
            string errormsg = string.Empty;

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
            if (string.IsNullOrEmpty(CommonLogic.FormCanBeDangerousContent("CardNumber")))
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

            theAddress.AddressType = AddressTypes.Billing;
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

            theAddress.PaymentMethodLastUsed = AppLogic.ro_PMCreditCard;
            theAddress.CardName = CommonLogic.FormCanBeDangerousContent("CardName");
            theAddress.CardType = CommonLogic.FormCanBeDangerousContent("CardType");

            string tmpS = CommonLogic.FormCanBeDangerousContent("CardNumber");
            if (!tmpS.StartsWith("*"))
            {
                theAddress.CardNumber = tmpS;
            }
            theAddress.CardExpirationMonth = CommonLogic.FormCanBeDangerousContent("CardExpirationMonth");
            theAddress.CardExpirationYear = CommonLogic.FormCanBeDangerousContent("CardExpirationYear");
            theAddress.UpdateDB();

            litCCForm.Text = theAddress.InputCardHTML(ThisCustomer, false, false);

            RecurringOrderMgr rmgr = new RecurringOrderMgr(base.EntityHelpers, base.GetParser);
            rmgr.ProcessAutoBillAddressUpdate(OriginalRecurringOrderNumber, theAddress);
            if (!ThisCustomer.MasterShouldWeStoreCreditCardInfo)
            {
                theAddress.ClearCCInfo();
                theAddress.UpdateDB();
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
            ltOriginalRecurringOrderNumber.Text = OriginalRecurringOrderNumber.ToString();
            litCCForm.Text = theAddress.InputCardHTML(ThisCustomer, false, false);
            btnSaveAddress.Text = AppLogic.GetString("editaddress.aspx.9", SkinID, ThisCustomer.LocaleSetting);
            btnSaveAddress.CommandArgument = AddressID.ToString();

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
            SetTemplate("empty2.ascx");
            base.OnInit(e);
        }

    }
}
