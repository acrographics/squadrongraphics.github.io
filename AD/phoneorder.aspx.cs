// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2006.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/phoneorder.aspx.cs 24    9/24/06 11:54p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{

    public partial class phoneorder : System.Web.UI.Page
    {
        private Customer ThisCustomer;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
            ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            GetJavaScriptFunctions();

            if (!this.IsPostBack)
            {
                if (AppLogic.AppConfigBool("PhoneOrder.EMailIsOptional"))
                {
                    valRegExValEmail.Enabled = false;
                }
                BillingState.ClearSelection();
                BillingState.Items.Clear();
                ShippingState.ClearSelection();
                ShippingState.Items.Clear();
                DataSet ds = DB.GetDS("select * from State " + DB.GetNoLock() + " order by DisplayOrder,Name", false);
                ShippingState.DataSource = ds;
                ShippingState.DataBind();
                BillingState.DataSource = ds;
                BillingState.DataBind();
                ds.Dispose();

                ShippingCountry.ClearSelection();
                ShippingCountry.Items.Clear();
                BillingCountry.ClearSelection();
                BillingCountry.Items.Clear();
                ds = DB.GetDS("select * from Country " + DB.GetNoLock() + " order by DisplayOrder,Name", false);
                ShippingCountry.DataSource = ds;
                ShippingCountry.DataBind();
                BillingCountry.DataSource = ds;
                BillingCountry.DataBind();
                ds.Dispose();

                AffiliateList.ClearSelection();
                AffiliateList.Items.Clear();
                ds = DB.GetDS("select AffiliateID,Name,DisplayOrder from Affiliate " + DB.GetNoLock() + " union select 0,'--N/A--', 0 from Affiliate " + DB.GetNoLock() + " order by DisplayOrder,Name", false);
                AffiliateList.DataSource = ds;
                AffiliateList.DataBind();
                if (ds.Tables[0].Rows.Count == 0)
                {
                    AffiliateList.Visible = false;
                    AffiliatePrompt.Visible = false;
                }
                ds.Dispose();


                CustomerLevelList.ClearSelection();
                CustomerLevelList.Items.Clear();
                IDataReader dr = DB.GetRS("select CustomerLevelID,Name,DisplayOrder from CustomerLevel " + DB.GetNoLock() + " union select 0,'--N/A--', 0 from CustomerLevel " + DB.GetNoLock() + " order by DisplayOrder,Name");
                while (dr.Read())
                {
                    CustomerLevelList.Items.Add(new ListItem(DB.RSFieldByLocale(dr, "Name", ThisCustomer.LocaleSetting), DB.RSFieldInt(dr, "CustomerLevelID").ToString()));
                }
                dr.Dispose();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            // create new customer
            CreateNewCustomerPanel.Visible = true;
            SearchCustomerPanel.Visible = false;

            SetContextToNewCustomer();

            //TopPanel.Visible = false;
            //Button1.Visible = false;
            //Button2.Visible = false;
        }

        protected void Button2_Click1(object sender, EventArgs e)
        {
            SearchCustomerPanel.Visible = true;
            CreateNewCustomerPanel.Visible = false;
            //TopPanel.Visible = false;
            //Button1.Visible = false;
            //Button2.Visible = false;

            String sql = "select top 50 CustomerID, FirstName, LastName, EMail from Customer " + DB.GetNoLock() + " where deleted=0 and IsAdmin=0 and (convert(nvarchar(10),CustomerID)=" + DB.SQuote(TextBox1.Text) + " or FirstName like " + DB.SQuote("%" + TextBox1.Text + "%") + " or LastName like " + DB.SQuote("%" + TextBox1.Text + "%") + " or CustomerID in (select CustomerID from Orders where convert(nvarchar(10),OrderNumber)=" + DB.SQuote(TextBox1.Text) + ") or EMail like " + DB.SQuote("%" + TextBox1.Text + "%") + ")";
            SQLText.Text = sql;
            //SQLText.Visible = true;
            DataSet ds = DB.GetDS(sql, false);
            GridView1.DataSource = ds;
            GridView1.DataBind();
            ds.Dispose();
        }

        protected void SetContextToCustomer(int RowID)
        {
            SetContextToNewCustomer();
            EMailAlreadyTaken.Visible = false;
            SearchCustomerPanel.Visible = false;
            DataKey data = GridView1.DataKeys[RowID];
            int iCustomerID = (int)data.Values["CustomerID"];
            CustomerIDPanel.Visible = true;
            CustomerID.Text = iCustomerID.ToString();
            IDataReader rs = DB.GetRS("select * from Customer " + DB.GetNoLock() + " where deleted=0 and IsAdmin=0 and CustomerID=" + iCustomerID.ToString());
            if (rs.Read())
            {
                CreateNewCustomerPanel.Visible = true;
                FirstName.Text = DB.RSField(rs, "FirstName");
                LastName.Text = DB.RSField(rs, "LastName");
                EMail.Text = DB.RSField(rs, "EMail").ToLowerInvariant().Trim();
                Phone.Text = DB.RSField(rs, "Phone");
                Over13.Checked = DB.RSFieldBool(rs, "Over13Checked");
                RadioButtonList1.SelectedIndex = CommonLogic.IIF(DB.RSFieldBool(rs,"OkToEMail"),0,1);

                Address BillingAddress = new Address();
                BillingAddress.LoadByCustomer(iCustomerID, AddressTypes.Billing);
                BillingFirstName.Text = BillingAddress.FirstName;
                BillingLastName.Text = BillingAddress.LastName;
                BillingPhone.Text = BillingAddress.Phone;
                BillingCompany.Text = BillingAddress.Company;
                try
                {
                    BillingResidenceType.ClearSelection();
                    if (BillingAddress.ResidenceType != ResidenceTypes.Unknown)
                    {
                        BillingResidenceType.Items.FindByValue(((int)BillingAddress.ResidenceType).ToString()).Selected = true;
                    }
                }
                catch { }
                BillingAddress1.Text = BillingAddress.Address1;
                BillingAddress2.Text = BillingAddress.Address2;
                BillingSuite.Text = BillingAddress.Suite;
                BillingCity.Text = BillingAddress.City;
                try
                {
                    BillingState.SelectedIndex = -1;
                    BillingState.ClearSelection();
                    BillingState.Items.FindByValue(BillingAddress.State).Selected = true;
                }
                catch { }
                BillingZip.Text = BillingAddress.Zip;
                try
                {
                    BillingCountry.SelectedIndex = -1;
                    BillingCountry.ClearSelection();
                    BillingCountry.Items.FindByValue(BillingAddress.Country).Selected = true;
                }
                catch { }

                Address ShippingAddress = new Address();
                ShippingAddress.LoadByCustomer(iCustomerID, AddressTypes.Shipping);
                ShippingFirstName.Text = ShippingAddress.FirstName;
                ShippingLastName.Text = ShippingAddress.LastName;
                ShippingPhone.Text = ShippingAddress.Phone;
                ShippingCompany.Text = ShippingAddress.Company;
                try
                {
                    ShippingResidenceType.ClearSelection();
                    if (ShippingAddress.ResidenceType != ResidenceTypes.Unknown)
                    {
                        ShippingResidenceType.Items.FindByValue(((int)ShippingAddress.ResidenceType).ToString()).Selected = true;
                    }
                }
                catch { }
                ShippingAddress1.Text = ShippingAddress.Address1;
                ShippingAddress2.Text = ShippingAddress.Address2;
                ShippingSuite.Text = ShippingAddress.Suite;
                ShippingCity.Text = ShippingAddress.City;
                try
                {
                    ShippingState.SelectedIndex = -1;
                    ShippingState.ClearSelection();
                    ShippingState.Items.FindByValue(ShippingAddress.State).Selected = true;
                }
                catch { }
                ShippingZip.Text = ShippingAddress.Zip;
                try
                {
                    ShippingCountry.SelectedIndex = -1;
                    ShippingCountry.ClearSelection();
                    ShippingCountry.Items.FindByValue(ShippingAddress.Country).Selected = true;
                }
                catch { }
                try
                {
                    AffiliateList.SelectedIndex = -1;
                    AffiliateList.ClearSelection();
                    AffiliateList.Items.FindByValue(DB.RSFieldInt(rs, "AffiliateID").ToString()).Selected = true;
                }
                catch { }
                try
                {
                    CustomerLevelList.SelectedIndex = -1;
                    CustomerLevelList.ClearSelection();
                    CustomerLevelList.Items.FindByValue(DB.RSFieldInt(rs, "CustomerLevelID").ToString()).Selected = true;
                }
                catch { }

            }
            rs.Close();
            CreateCustomer.Visible = false;
            UseCustomer.Visible = true;
            UpdateCustomer.Visible = true;
        }

        protected void SetContextToNewCustomer()
        {
            SearchCustomerPanel.Visible = false;
            CustomerIDPanel.Visible = false;

            CustomerID.Text = String.Empty;
            FirstName.Text = String.Empty;
            LastName.Text = String.Empty;
            EMail.Text = String.Empty;
            Phone.Text = String.Empty;
            RadioButtonList1.SelectedIndex = 0;
            Over13.Checked = true;

            BillingFirstName.Text = String.Empty;
            BillingLastName.Text = String.Empty;
            BillingPhone.Text = String.Empty;
            BillingCompany.Text = String.Empty;
            BillingResidenceType.ClearSelection();
            BillingResidenceType.SelectedIndex = 1;
            BillingAddress1.Text = String.Empty;
            BillingAddress2.Text = String.Empty;
            BillingSuite.Text = String.Empty;
            BillingCity.Text = String.Empty;
            BillingState.ClearSelection();
            BillingState.SelectedIndex = 0;
            BillingZip.Text = String.Empty;
            BillingCountry.ClearSelection();
            BillingCountry.SelectedIndex = 0;

            ShippingFirstName.Text = String.Empty;
            ShippingLastName.Text = String.Empty;
            ShippingPhone.Text = String.Empty;
            ShippingCompany.Text = String.Empty;
            ShippingResidenceType.ClearSelection();
            ShippingResidenceType.SelectedIndex = 1;
            ShippingAddress1.Text = String.Empty;
            ShippingAddress2.Text = String.Empty;
            ShippingSuite.Text = String.Empty;
            ShippingCity.Text = String.Empty;
            ShippingCountry.ClearSelection();
            ShippingCountry.SelectedIndex = -1;
            ShippingState.ClearSelection();
            ShippingState.SelectedIndex = -1;
            ShippingZip.Text = String.Empty;
            
            try
            {
                AffiliateList.SelectedIndex = -1;
                AffiliateList.ClearSelection();
            }
            catch { }
            try
            {
                CustomerLevelList.SelectedIndex = -1;
                CustomerLevelList.ClearSelection();
            }
            catch { }

            CreateCustomer.Visible = true;
            UseCustomer.Visible = false;
            UpdateCustomer.Visible = false;
            CreateNewCustomerPanel.Visible = true;
            CustomerStatsPanel.Visible = false;
            ImpersonationPanel.Visible = false;
            Panel3.Visible = false;
            Panel2.Visible = false;
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToLowerInvariant().Equals("select"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                SetContextToCustomer(index);
            }
        }

        private void SetFramePage(String FrameName, String Url)
        {
            HtmlControl frame1 = (HtmlControl)this.FindControl(FrameName);
            frame1.Attributes["src"] = Url;
            if (FrameName.IndexOf("Impersonation") != -1)
            {
                ImpersonationPanel.Visible = true;
                Panel3.Visible = false;
                Panel2.Visible = false;
            }
            else if (FrameName.IndexOf("Panel2") != -1)
            {
                ImpersonationPanel.Visible = false;
                Panel2.Visible = true;
                Panel3.Visible = false;
            }
            else if (FrameName.IndexOf("Panel3") != -1)
            {
                ImpersonationPanel.Visible = false;
                Panel2.Visible = false;
                Panel3.Visible = true;
            }
        }

        private void SetToImpersonationPageContext(int CustomerID, String PageName, bool UseEmptyTemplate)
        {
            String IGD = String.Empty;
            IDataReader rs = DB.GetRS("Select CustomerGUID from Customer " + DB.GetNoLock() + " where deleted=0 and IsAdmin=0 and CustomerID=" + CustomerID.ToString());
            if (rs.Read())
            {
                IGD = DB.RSFieldGUID(rs, "CustomerGUID").ToString();
            }
            rs.Close();
            String Url = PageName + CommonLogic.IIF(PageName.IndexOf("?") == -1, "?", "&") + "IGD=" + IGD.ToString();
            SetFramePage("ImpersonationFrame", Url);
        }

        private void SetToPanel2Page(int CustomerID, String PageName, bool UseEmptyTemplate)
        {
            String IGD = String.Empty;
            IDataReader rs = DB.GetRS("Select CustomerGUID from Customer " + DB.GetNoLock() + " where deleted=0 and IsAdmin=0 and CustomerID=" + CustomerID.ToString());
            if (rs.Read())
            {
                IGD = DB.RSFieldGUID(rs, "CustomerGUID").ToString();
            }
            rs.Close();
            String Url = PageName + CommonLogic.IIF(PageName.IndexOf("?") == -1, "?", "&") + "IGD=" + IGD.ToString();
            SetFramePage("LeftPanel2Frame", Url);
        }

        private void SetToPanel3Page(int CustomerID, String PageName, bool UseEmptyTemplate)
        {
            String IGD = String.Empty;
            IDataReader rs = DB.GetRS("Select CustomerGUID from Customer " + DB.GetNoLock() + " where deleted=0 and IsAdmin=0 and CustomerID=" + CustomerID.ToString());
            if (rs.Read())
            {
                IGD = DB.RSFieldGUID(rs, "CustomerGUID").ToString();
            }
            rs.Close();
            String Url = PageName + CommonLogic.IIF(PageName.IndexOf("?") == -1, "?", "&") + "IGD=" + IGD.ToString();
            SetFramePage("LeftPanel3Frame", Url);
        }


        protected void CreateCustomer_Click(object sender, EventArgs e)
        {
            bool ErrorsFound = false;
            EMailAlreadyTaken.Visible = false;

            String AccountName = (FirstName.Text.Trim() + " " + LastName.Text.Trim()).Trim();
            if (AccountName.Length == 0)
            {
                AccountName = (BillingFirstName.Text.Trim() + " " + BillingLastName.Text.Trim()).Trim();
            }

            if (EMail.Text.ToLowerInvariant().Trim().Length != 0 && !AppLogic.AppConfigBool("AllowCustomerDuplicateEMailAddresses"))
            {
                int NN = DB.GetSqlN("select count(*) as N from customer " + DB.GetNoLock() + " where deleted=0 and IsAdmin=0 and EMail=" + DB.SQuote(EMail.Text.ToLowerInvariant().Trim()));
                if (NN > 0)
                {
                    EMailAlreadyTaken.Visible = true;
                    ErrorsFound = true;
                }
            }

            if (!ErrorsFound)
            {
                int m_CustomerID = 0;
                String m_CustomerGUID = String.Empty;
                Customer.MakeAnonCustomerRecord(out m_CustomerID, out m_CustomerGUID);
                if (EMail.Text.ToLowerInvariant().Trim().Length == 0)
                {
                    IDataReader rs = DB.GetRS("select EMail from Customer " + DB.GetNoLock() + " where deleted=0 and IsAdmin=0 and CustomerID=" + m_CustomerID.ToString());
                    if (rs.Read())
                    {
                        EMail.Text = DB.RSField(rs, "EMail").ToLowerInvariant().Trim();
                    }
                    rs.Close();
                }

                // when first created, we create a random password for the user, and e-mail it to them!
                Password p = new RandomPassword();

                if (EMail.Text.ToLowerInvariant().Trim().Length != 0 && AppLogic.MailServer().Length != 0 && AppLogic.MailServer() != AppLogic.ro_TBD)
                {
                    try
                    {
                        AppLogic.SendMail(AppLogic.AppConfig("StoreName") + " - " + AppLogic.GetString("cst_account_process.aspx.1", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), AppLogic.RunXmlPackage("notification.lostpassword.xml.config", null, ThisCustomer, ThisCustomer.SkinID, "", "thiscustomerid=" + m_CustomerID.ToString() + "&newpwd=" + p.ClearPassword.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;").Replace("&", "&amp;"), false, false), true, AppLogic.AppConfig("MailMe_FromAddress"), AppLogic.AppConfig("MailMe_FromName"), EMail.Text.ToLowerInvariant().Trim(), EMail.Text.ToLowerInvariant().Trim(), "", "", AppLogic.MailServer());
                        Security.LogEvent("Admin Create Phone Order Customer Random Password", "", m_CustomerID, ThisCustomer.CustomerID, Convert.ToInt32(ThisCustomer.CurrentSessionID));
                    }
                    catch
                    {
                        // we have NO way of handling a failure to send the customer their pwd here!!!
                    }
                }

                StringBuilder sql = new StringBuilder(1024);
                sql.Append("update customer set ");
                sql.Append("RegisterDate=getdate(),");
                sql.Append("FirstName=" + DB.SQuote(FirstName.Text.Trim()) + ",");
                sql.Append("LastName=" + DB.SQuote(LastName.Text.Trim()) + ",");
                sql.Append("EMail=" + DB.SQuote(EMail.Text.ToLowerInvariant().Trim()) + ",");
                sql.Append("IsRegistered=" + CommonLogic.IIF(EMail.Text.ToLowerInvariant().Trim().Length != 0 && (FirstName.Text.Trim() + LastName.Text.Trim()).Trim().Length != 0, "1", "0") + ",");

                // set their pwd to the new random password, must be changed on first login:
                sql.Append("Password=" + DB.SQuote(p.SaltedPassword) + ",");
                sql.Append("SaltKey=" + p.Salt.ToString() + ",");
                sql.Append("PwdChangeRequired=1,");

                sql.Append("Phone=" + DB.SQuote(Phone.Text) + ",");
                if (AffiliateList.SelectedIndex > 0)
                {
                    sql.Append("AffiliateID=" + AffiliateList.SelectedValue + ",");
                }
                if (CustomerLevelList.SelectedIndex > 0)
                {
                    sql.Append("CustomerLevelID=" + CustomerLevelList.SelectedValue + ",");
                }
                sql.Append("Over13Checked=" + CommonLogic.IIF(Over13.Checked, "1", "0") + ",");
                sql.Append("OKToEMail=" + CommonLogic.IIF(RadioButtonList1.SelectedValue == "Yes", "1", "0"));
                sql.Append(" where CustomerID=" + m_CustomerID.ToString());
                DB.ExecuteSQL(sql.ToString());

                Address BillingAddress = new Address();
                Address ShippingAddress = new Address();

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
                BillingAddress.EMail = EMail.Text.ToLowerInvariant().Trim();

                BillingAddress.InsertDB(m_CustomerID);
                BillingAddress.MakeCustomersPrimaryAddress(AddressTypes.Billing);

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
                ShippingAddress.EMail = EMail.Text.ToLowerInvariant().Trim();

                ShippingAddress.InsertDB(m_CustomerID);
                ShippingAddress.MakeCustomersPrimaryAddress(AddressTypes.Shipping);

                CustomerID.Text = m_CustomerID.ToString();
            }

            if (!ErrorsFound)
            {
                CreateCustomer.Visible = false;
                UpdateCustomer.Visible = true;
                UseCustomer.Visible = true;
            }
        }

        protected void UpdateCustomer_Click(object sender, EventArgs e)
        {
            bool ErrorsFound = false;
            EMailAlreadyTaken.Visible = false;

            String AccountName = (FirstName.Text.Trim() + " " + LastName.Text.Trim()).Trim();
            if (AccountName.Length == 0)
            {
                AccountName = (BillingFirstName.Text.Trim() + " " + BillingLastName.Text.Trim()).Trim();
            }

            if (!AppLogic.AppConfigBool("PhoneOrder.EMailIsOptional") && EMail.Text.ToLowerInvariant().Trim().Length != 0 && !AppLogic.AppConfigBool("AllowCustomerDuplicateEMailAddresses"))
            {
                int NN = DB.GetSqlN("select count(*) as N from customer " + DB.GetNoLock() + " where deleted=0 and IsAdmin=0 and EMail=" + DB.SQuote(EMail.Text.ToLowerInvariant().Trim()) + " and CustomerID<> " + CustomerID.Text);
                if (NN > 0)
                {
                    EMailAlreadyTaken.Visible = true;
                    ErrorsFound = true;
                }
            }

            if (!ErrorsFound)
            {
                int m_CustomerID = System.Int32.Parse(CustomerID.Text);
                StringBuilder sql = new StringBuilder(1024);
                sql.Append("update customer set ");
                sql.Append("FirstName=" + DB.SQuote(FirstName.Text.Trim()) + ",");
                sql.Append("LastName=" + DB.SQuote(LastName.Text.Trim()) + ",");
                sql.Append("EMail=" + DB.SQuote(EMail.Text.ToLowerInvariant().Trim()) + ",");
                sql.Append("Phone=" + DB.SQuote(Phone.Text.Trim()) + ",");
                if (AffiliateList.SelectedIndex > 0)
                {
                    sql.Append("AffiliateID=" + AffiliateList.SelectedValue + ",");
                }
                if (CustomerLevelList.SelectedIndex > 0)
                {
                    sql.Append("CustomerLevelID=" + CustomerLevelList.SelectedValue + ",");
                }
                sql.Append("Over13Checked=" + CommonLogic.IIF(Over13.Checked, "1", "0") + ",");
                sql.Append("OKToEMail=" + CommonLogic.IIF(RadioButtonList1.SelectedValue == "Yes", "1", "0"));
                sql.Append(" where CustomerID=" + m_CustomerID.ToString());
                DB.ExecuteSQL(sql.ToString());

                Address BillingAddress = new Address();
                Address ShippingAddress = new Address();

                BillingAddress.LoadByCustomer(m_CustomerID, AddressTypes.Billing);
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
                BillingAddress.EMail = EMail.Text.ToLowerInvariant().Trim();
                if (BillingAddress.AddressID == 0)
                {
                    BillingAddress.InsertDB(m_CustomerID);
                }
                else
                {
                    BillingAddress.UpdateDB();
                }
                BillingAddress.MakeCustomersPrimaryAddress(AddressTypes.Billing);

                ShippingAddress.LoadByCustomer(m_CustomerID, AddressTypes.Shipping);
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
                ShippingAddress.EMail = EMail.Text.ToLowerInvariant().Trim();
                if (ShippingAddress.AddressID == 0)
                {
                    ShippingAddress.InsertDB(m_CustomerID);
                }
                else
                {
                    ShippingAddress.UpdateDB();
                }
                ShippingAddress.MakeCustomersPrimaryAddress(AddressTypes.Shipping);
            }

            if (!ErrorsFound)
            {
                CreateCustomer.Visible = false;
                UpdateCustomer.Visible = true;
                UseCustomer.Visible = true;
            }
        }

        protected void UseCustomer_Click(object sender, EventArgs e)
        {
            UpdateCustomer_Click(sender, e);
            if (EMailAlreadyTaken.Visible == false)
            {

                // use customer as is:
                TopPanel.Visible = false;
                SearchCustomerPanel.Visible = false;
                CreateNewCustomerPanel.Visible = false;
                CustomerStatsPanel.Visible = true;
                CreateCustomer.Visible = false;
                UseCustomer.Visible = false;
                UpdateCustomer.Visible = false;

                UsingCustomerID.Text = CustomerID.Text;
                UsingFirstName.Text = FirstName.Text;
                UsingLastName.Text = LastName.Text;
                UsingEMail.Text = EMail.Text.ToLowerInvariant().Trim();

                SetToImpersonationPageContext(System.Int32.Parse(CustomerID.Text), "../default.aspx", false);
            }
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            // Set Home Page
            SetToImpersonationPageContext(System.Int32.Parse(CustomerID.Text), "../default.aspx", false);
        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            // cancel order
            ThisCustomer.ThisCustomerSession["igd"] = "";
            Response.Redirect("phoneorder.aspx");
        }

        protected void Button88_Click(object sender, EventArgs e)
        {
            // done with order
            ThisCustomer.ThisCustomerSession["igd"] = "";
            Response.Redirect("phoneorder.aspx");
        }

        protected void Button7_Click(object sender, EventArgs e)
        {
            // set SiteMap1
            SetToPanel2Page(System.Int32.Parse(CustomerID.Text), "phonesitemap1.aspx", false);
        }

        protected void Button18_Click(object sender, EventArgs e)
        {
            // set SiteMap2
            SetToPanel2Page(System.Int32.Parse(CustomerID.Text), "phonesitemap.aspx", false);
        }


        protected void Button9_Click(object sender, EventArgs e)
        {
            // do a search
            SetToPanel2Page(System.Int32.Parse(CustomerID.Text), "phonesearch.aspx?searchterm=" + Server.UrlEncode(QuickSearch.Text), false);
        }

        protected void Button10_Click(object sender, EventArgs e)
        {
            // shopping cart
            SetToImpersonationPageContext(System.Int32.Parse(CustomerID.Text), "../shoppingcart.aspx", false);
        }

        protected void Button11_Click(object sender, EventArgs e)
        {
            // checkout
            SetToImpersonationPageContext(System.Int32.Parse(CustomerID.Text), "../checkoutshipping.aspx", false);
        }
        protected void Button12_Click(object sender, EventArgs e)
        {
            // re-edit customer
            CreateNewCustomerPanel.Visible = true;
            CustomerStatsPanel.Visible = false;
            ImpersonationPanel.Visible = false;
            CreateCustomer.Visible = false;
            UpdateCustomer.Visible = true;
            UseCustomer.Visible = true;
            Panel3.Visible = false;
            Panel2.Visible = false;
        }
        protected void Button13_Click(object sender, EventArgs e)
        {
            // restart impersonation
            UseCustomer_Click(sender, e);
        }

        private void GetJavaScriptFunctions()
        {
            btnCopyAccount.Attributes.Add("onclick", "copyaccount(this.form);");
            string strScript = "<script type=\"text/javascript\">";
            strScript += "function copyaccount(theForm){ ";
            strScript += "theForm." + BillingFirstName.ClientID + ".value = theForm." + FirstName.ClientID + ".value;";
            strScript += "theForm." + BillingLastName.ClientID + ".value = theForm." + LastName.ClientID + ".value;";
            strScript += "theForm." + BillingPhone.ClientID + ".value = theForm." + Phone.ClientID + ".value;";
            strScript += "return true; }  ";
            strScript += "</script> ";

            ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), strScript);

            btnCopyBilling.Attributes.Add("onclick", "copybilling(this.form);");
            strScript = string.Empty;
            strScript = "<script type=\"text/javascript\">";
            strScript += "function copybilling(theForm){";
            strScript += "theForm." + ShippingFirstName.ClientID + ".value = theForm." + BillingFirstName.ClientID + ".value;";
            strScript += "theForm." + ShippingLastName.ClientID + ".value = theForm." + BillingLastName.ClientID + ".value;";
            strScript += "theForm." + ShippingPhone.ClientID + ".value = theForm." + BillingPhone.ClientID + ".value;";
            strScript += "theForm." + ShippingCompany.ClientID + ".value = theForm." + BillingCompany.ClientID + ".value;";
            strScript += "theForm." + ShippingCompany.ClientID + ".value = theForm." + BillingCompany.ClientID + ".value;";
            strScript += "theForm." + ShippingResidenceType.ClientID + "selectedIndex =  theForm." + BillingResidenceType.ClientID + ".selectedIndex;";
            strScript += "theForm." + ShippingAddress1.ClientID  + ".value = theForm." + BillingAddress1.ClientID + ".value;";
            strScript += "theForm." + ShippingAddress2.ClientID + ".value = theForm." + BillingAddress2.ClientID + ".value;";
            strScript += "theForm." + ShippingSuite.ClientID + ".value = theForm." + BillingSuite.ClientID + ".value;";
            strScript += "theForm." + ShippingCity.ClientID + ".value = theForm." + BillingCity.ClientID + ".value;";
            strScript += "theForm." + ShippingState.ClientID + ".value = theForm." + BillingState.ClientID + ".value;";
            strScript += "theForm." + ShippingZip.ClientID + ".value = theForm." + BillingZip.ClientID + ".value;";
            strScript += "theForm." + ShippingCountry.ClientID + ".selectedIndex =  theForm." + BillingCountry.ClientID + ".selectedIndex;";
            strScript += "return true; }  ";
            strScript += "</script> ";

            ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), strScript);
        }

    }
}