// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/giftregistrysearch.aspx.cs 5     9/14/06 12:05a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for giftregistrysearch.
    /// </summary>
    public partial class giftregistrysearch : SkinBase
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            SectionTitle = AppLogic.GetString("giftregistrysearch.aspx.1", SkinID, ThisCustomer.LocaleSetting);
            if (!IsPostBack)
            {
                InitializePageContent();
            }
        }

        public void btnSearchForName_Click(object sender, EventArgs e)
        {
            string sql = "select top 1 CustomerID,GiftRegistryGUID from Customer where ltrim(rtrim(lower(FirstName + ' ' + LastName)))=" + DB.SQuote(txtSearchForName.Text.ToLowerInvariant()) + " and GiftRegistryIsAnonymous=0 and ShippingAddressID IS NOT NULL and ShippingAddressID<>0 and GiftRegistryAllowSearchByOthers=1";
            DoSearch(sql);
            InitializePageContent();
        }
        public void btnSearchForNickName_Click(object sender, EventArgs e)
        {
            string sql = "select top 1 CustomerID,GiftRegistryGUID from Customer where lower(GiftRegistryNickName)=" + DB.SQuote(txtSearchForNickName.Text.ToLowerInvariant()) + " and ShippingAddressID IS NOT NULL and ShippingAddressID<>0 and GiftRegistryAllowSearchByOthers=1";
            DoSearch(sql);
            InitializePageContent();
        }
        public void btnSearchForEMail_Click(object sender, EventArgs e)
        {
            string sql = "select top 1 CustomerID,GiftRegistryGUID from Customer where email=" + DB.SQuote(txtSearchForEMail.Text.ToLowerInvariant()) + " and GiftRegistryIsAnonymous=0 and ShippingAddressID IS NOT NULL and ShippingAddressID<>0 and GiftRegistryAllowSearchByOthers=1";
            DoSearch(sql);
            InitializePageContent();
        }
        public void btnSaveButton_Click(object sender, EventArgs e)
        {
            try // ignore dups
            {
                string sql = "insert CustomerGiftRegistrySearches(CustomerID,GiftRegistryGUID) values(" + ThisCustomer.CustomerID.ToString() + "," + DB.SQuote(((Button)sender).CommandArgument.ToString()) + ")";
                DB.ExecuteSQL(sql);
            }
            catch { }
            Response.Redirect("giftregistry.aspx");
        }


        private void InitializePageContent()
        {
            tblGiftRegistrySearch.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
            tblGiftRegistrySearchBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));

            giftregistry3_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/giftregistry3.gif");
            giftregistrysearch_aspx_2.Text = AppLogic.GetString("giftregistrysearch.aspx.2", SkinID, ThisCustomer.LocaleSetting) + "<br/><br/>";
            
            giftregistrysearch_aspx_3.Text = AppLogic.GetString("giftregistrysearch.aspx.3", SkinID, ThisCustomer.LocaleSetting);
            btnSearchForName.Text = AppLogic.GetString("giftregistrysearch.aspx.6", SkinID, ThisCustomer.LocaleSetting);
            txtSearchForName.Attributes.Add("onkeydown", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + btnSearchForName.ClientID + "').click();return false;}} else {return true}; ");

            giftregistrysearch_aspx_4.Text = AppLogic.GetString("giftregistrysearch.aspx.4", SkinID, ThisCustomer.LocaleSetting);
            btnSearchForNickName.Text = AppLogic.GetString("giftregistrysearch.aspx.6", SkinID, ThisCustomer.LocaleSetting);
            txtSearchForNickName.Attributes.Add("onkeydown", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + btnSearchForNickName.ClientID + "').click();return false;}} else {return true}; ");

            giftregistrysearch_aspx_5.Text = AppLogic.GetString("giftregistrysearch.aspx.5", SkinID, ThisCustomer.LocaleSetting);
            btnSearchForEMail.Text = AppLogic.GetString("giftregistrysearch.aspx.6", SkinID, ThisCustomer.LocaleSetting);
            txtSearchForEMail.Attributes.Add("onkeydown", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + btnSearchForEMail.ClientID + "').click();return false;}} else {return true}; ");

        }
        private void DoSearch(string sql)
        {
            String GiftRegistryGUID = String.Empty;
            int RegistryOwnerCustomerID = 0;
            IDataReader rs = DB.GetRS(sql);
            if (rs.Read())
            {
                GiftRegistryGUID = DB.RSFieldGUID(rs, "GiftRegistryGUID");
                RegistryOwnerCustomerID = DB.RSFieldInt(rs, "CustomerID");
            }
            rs.Close();

            if (GiftRegistryGUID.Length != 0 && RegistryOwnerCustomerID != 0)
            {
                if (RegistryOwnerCustomerID == ThisCustomer.CustomerID)
                {
                    Response.Redirect("giftregistry.aspx"); // they are viewing their OWN registry!
                }
                pnlSearchResults.Visible = true;
                Customer RegistryOwnerCustomer = new Customer(RegistryOwnerCustomerID, true);

                ShoppingCart cart = new ShoppingCart(SkinID, RegistryOwnerCustomer, CartTypeEnum.GiftRegistryCart, 0, false);
                String DisplayName = AppLogic.GiftRegistryDisplayName(RegistryOwnerCustomer.CustomerID, false, SkinID, ThisCustomer.LocaleSetting);
                giftregistry_aspx_16.Text = "<p align=\"left\">" + String.Format(AppLogic.GetString("giftregistry.aspx.16", SkinID, ThisCustomer.LocaleSetting), DisplayName);
                if (ThisCustomer.IsRegistered && ThisCustomer.HasCustomerRecord)
                {
                    giftregistry_aspx_16.Text += "&nbsp;&nbsp;";
                    btnSaveButton.CommandArgument = GiftRegistryGUID;
                    btnSaveButton.Visible = true;
                    btnSaveButton.Text = AppLogic.GetString("giftregistrysearch.aspx.12", SkinID, ThisCustomer.LocaleSetting);
                }
                giftregistry5_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/giftregistry5.gif");
                SearchResults.Text = cart.DisplayItems(true, ThisCustomer, false);
                tblSearchResults.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
                tblSearchResultsBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));

            }
            else
            {
                SearchResults.Text = AppLogic.GetString("giftregistrysearch.aspx.10", SkinID, ThisCustomer.LocaleSetting) + "<br/><br/>";
            }

        }
    }
}
