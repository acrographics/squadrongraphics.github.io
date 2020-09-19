// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/entityEditProducts.aspx.cs 18    10/04/06 12:07p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using AspDotNetStorefrontCommon;
using System.Xml;

public partial class entityEditProducts : System.Web.UI.Page
{
    private Customer ThisCustomer;
    private EntityHelper entity;
    private string eName;
    private int pID;
    private int eID;
    private EntitySpecs eSpecs;
    private ProductDescriptionFile pdesc;
    private int skinID = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");

        if (!IsPostBack)
        {
            ViewState.Add("ProductEditID", 0);
        }

        PopularityRow.Visible = AppLogic.AppConfigBool("ShowPopularity");

        ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

        pdesc = new ProductDescriptionFile(pID, ThisCustomer.LocaleSetting, skinID);
        pID = CommonLogic.QueryStringNativeInt("iden");
        eID = CommonLogic.QueryStringNativeInt("EntityFilterID");
        eName = CommonLogic.QueryStringCanBeDangerousContent("EntityName");
        eSpecs = EntityDefinitions.LookupSpecs(eName);

        if (Localization.ParseNativeInt(ViewState["ProductEditID"].ToString()) > 0)
        {
            pID = Localization.ParseNativeInt(ViewState["ProductEditID"].ToString());
        }

        switch (eName.ToUpperInvariant())
        {
            case "SECTION":
                ltPreEntity.Text = "Section";
                entity = new EntityHelper(EntityDefinitions.readonly_SectionEntitySpecs);
                break;
            case "MANUFACTURER":
                ltPreEntity.Text = "Manufacturer";
                entity = new EntityHelper(EntityDefinitions.readonly_ManufacturerEntitySpecs);
                break;
            case "DISTRIBUTOR":
                ltPreEntity.Text = "Distributor";
                entity = new EntityHelper(EntityDefinitions.readonly_DistributorEntitySpecs);
                break;
            case "LIBRARY":
                ltPreEntity.Text = "Library";
                entity = new EntityHelper(EntityDefinitions.readonly_LibraryEntitySpecs);
                break;
            default:
                ltPreEntity.Text = "Category";
                entity = new EntityHelper(EntityDefinitions.readonly_CategoryEntitySpecs);
                break;
        }

        if (!IsPostBack)
        {
            btnDeleteAll.Attributes.Add("onclick", "return confirm('Confirm Delete');");
            if (ThisCustomer.ThisCustomerSession.Session("entityUserLocale").Length == 0)
            {
                ThisCustomer.ThisCustomerSession.SetVal("entityUserLocale", Localization.GetWebConfigLocale());
            }

            ddLocale.Items.Clear();
#if PRO
                //not supported in PRO version
#else
            System.Data.DataSet ds = Localization.GetLocales();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ddLocale.Items.Add(new ListItem(DB.RowField(dr, "Name"), DB.RowField(dr, "Name")));
            }
            ds.Dispose();
#endif
            if (ddLocale.Items.Count < 2)
            {
                tblLocale.Visible = false;
            }
            LoadScript();
            LoadContent();
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

        ((Literal)Form.FindControl("ltError")).Text = str;
    }

    protected void LoadScript()
    {
        ltScript.Text = ("\n<script type=\"text/javascript\">\n");
        ltScript.Text += ("    Calendar.setup({\n");
        ltScript.Text += ("        inputField     :    \"txtStartDate\",      // id of the input field\n");
        ltScript.Text += ("        ifFormat       :    \"" + Localization.JSCalendarDateFormatSpec() + "\",       // format of the input field\n");
        ltScript.Text += ("        showsTime      :    false,            // will display a time selector\n");
        ltScript.Text += ("        button         :    \"f_trigger_s\",   // trigger for the calendar (button ID)\n");
        ltScript.Text += ("        singleClick    :    true            // double-click mode\n");
        ltScript.Text += ("    });\n");
        ltScript.Text += ("    Calendar.setup({\n");
        ltScript.Text += ("        inputField     :    \"txtStopDate\",      // id of the input field\n");
        ltScript.Text += ("        ifFormat       :    \"" + Localization.JSCalendarDateFormatSpec() + "\",       // format of the input field\n");
        ltScript.Text += ("        showsTime      :    false,            // will display a time selector\n");
        ltScript.Text += ("        button         :    \"f_trigger_sE\",   // trigger for the calendar (button ID)\n");
        ltScript.Text += ("        singleClick    :    true            // double-click mode\n");
        ltScript.Text += ("    });\n");
        ltScript.Text += ("</script>\n");

        ltStartDate.Text = "<img src=\"" + AppLogic.LocateImageURL("skins/skin_1/images/calendar.gif") + "\" style=\"cursor:hand;cursor:pointer;\" align=\"absmiddle\" id=\"f_trigger_s\">&nbsp;<small>(" + Localization.ShortDateFormat() + ")</small>";
        ltStopDate.Text = "<img src=\"" + AppLogic.LocateImageURL("skins/skin_1/images/calendar.gif") + "\" style=\"cursor:hand;cursor:pointer;\" align=\"absmiddle\" id=\"f_trigger_sE\">&nbsp;<small>(" + Localization.ShortDateFormat() + ")</small>";

        ltStyles.Text = ("  <!-- calendar stylesheet -->\n");
        ltStyles.Text += ("  <link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"jscalendar/calendar-win2k-cold-1.css\" title=\"win2k-cold-1\" />\n");
        ltStyles.Text += ("\n");
        ltStyles.Text += ("  <!-- main calendar program -->\n");
        ltStyles.Text += ("  <script type=\"text/javascript\" src=\"jscalendar/calendar.js\"></script>\n");
        ltStyles.Text += ("\n");
        ltStyles.Text += ("  <!-- language for the calendar -->\n");
        ltStyles.Text += ("  <script type=\"text/javascript\" src=\"jscalendar/lang/" + Localization.JSCalendarLanguageFile() + "\"></script>\n");
        ltStyles.Text += ("\n");
        ltStyles.Text += ("  <!-- the following script defines the Calendar.setup helper function, which makes\n");
        ltStyles.Text += ("       adding a calendar a matter of 1 or 2 lines of code. -->\n");
        ltStyles.Text += ("  <script type=\"text/javascript\" src=\"jscalendar/calendar-setup.js\"></script>\n");
    }

    protected void LoadContent()
    {
        ddDistributor.Items.Clear();
        ddDiscountTable.Items.Clear();
        ddManufacturer.Items.Clear();
        ddOnSalePrompt.Items.Clear();
        ddXmlPackage.Items.Clear();
        ddType.Items.Clear();
        ddTaxClass.Items.Clear();

        //pull locale from user session
        string entityLocale = ThisCustomer.ThisCustomerSession.Session("entityUserLocale");
        if (entityLocale.Length > 0)
        {
            try
            {
                ddLocale.SelectedValue = entityLocale; // user's locale may not exist any more, so don't let the assignment crash
            }
            catch { }
        }

        string locale = Localization.CheckLocaleSettingForProperCase(ddLocale.SelectedValue);

        bool ProductTracksInventoryBySizeAndColor = AppLogic.ProductTracksInventoryBySizeAndColor(pID);
        bool IsAKit = AppLogic.IsAKit(pID);
        bool IsAPack = AppLogic.IsAPack(pID);
        if (IsAKit || IsAPack)
        {
            ProductTracksInventoryBySizeAndColor = false;
        }
        ProductSpecFile pspec = new ProductSpecFile(pID, skinID);

        bool Editing = false;

        IDataReader rs = DB.GetRS("select * from Product  " + DB.GetNoLock() + " where ProductID=" + pID.ToString());
        if (rs.Read())
        {
            Editing = true;
            if (!DB.RSFieldBool(rs, "Published"))
            {
                lblPublished.ForeColor = System.Drawing.Color.Red;
                lblPublished.Font.Bold = true;
            }
            else
            {
                lblPublished.ForeColor = System.Drawing.Color.Black;
                lblPublished.Font.Bold = false;
            }

            if (DB.RSFieldBool(rs, "Deleted"))
            {
                btnDeleteProduct.Text = "UnDelete this Product";
            }
            else
            {
                btnDeleteProduct.Text = "Delete this Product";
            }
            pnlDelete.Visible = true;
        }
        else
        {
            pnlDelete.Visible = false;
        }

        ViewState.Add("ProductEdit", Editing);

        //////////////////////////////////////////////////////////////////////////
        //ltPreText.Text = "Please enter the following information about this Product. Fields marked with an asterisk (*) are required. All other fields are optional.";

        //load Product Types
        ddType.Items.Add(new ListItem(" - Select -", "0"));
        IDataReader rsst = DB.GetRS("select * from ProductType  " + DB.GetNoLock() + " order by DisplayOrder,Name");
        while (rsst.Read())
        {
            ddType.Items.Add(new ListItem(DB.RSFieldByLocale(rsst, "Name", ThisCustomer.LocaleSetting), DB.RSFieldInt(rsst, "ProductTypeID").ToString()));
        }
        rsst.Close();

        //load Manufacturers
        ddManufacturer.Items.Add(new ListItem(" - Select -", "0"));
        rsst = DB.GetRS("select * from Manufacturer " + DB.GetNoLock() + " where deleted=0 order by DisplayOrder,Name");
        while (rsst.Read())
        {
            ddManufacturer.Items.Add(new ListItem(DB.RSFieldByLocale(rsst, "Name", ThisCustomer.LocaleSetting), DB.RSFieldInt(rsst, "ManufacturerID").ToString()));
        }
        rsst.Close();

        //load Distributors
        ddDistributor.Items.Add(new ListItem(" - Select -", "0"));
        rsst = DB.GetRS("select * from Distributor  " + DB.GetNoLock() + " where Deleted=0 order by DisplayOrder,Name");
        while (rsst.Read())
        {
            ddDistributor.Items.Add(new ListItem(DB.RSFieldByLocale(rsst, "Name", ThisCustomer.LocaleSetting), DB.RSFieldInt(rsst, "DistributorID").ToString()));
        }
        rsst.Close();

        //load XmlPackages
        ArrayList xmlPackages = AppLogic.ReadXmlPackages("product", skinID);
        foreach (String s in xmlPackages)
        {
            ddXmlPackage.Items.Add(new ListItem(s, s));
        }

        //load Discount Tables
        ddDiscountTable.Items.Add(new ListItem("None", "0"));
        rsst = DB.GetRS("select * from QuantityDiscount  " + DB.GetNoLock() + " order by DisplayOrder,Name");
        while (rsst.Read())
        {
            ddDiscountTable.Items.Add(new ListItem(DB.RSFieldByLocale(rsst, "Name", ThisCustomer.LocaleSetting), DB.RSFieldInt(rsst, "QuantityDiscountID").ToString()));
        }
        rsst.Close();

        //load Tax Class
        ddTaxClass.Items.Add(new ListItem("None", "0"));
        rsst = DB.GetRS("select * from TaxClass  " + DB.GetNoLock() + " order by DisplayOrder,Name");
        while (rsst.Read())
        {
            ddTaxClass.Items.Add(new ListItem(DB.RSFieldByLocale(rsst, "Name", ThisCustomer.LocaleSetting), DB.RSFieldInt(rsst, "TaxClassID").ToString()));
        }
        rsst.Close();

        //On Sale prompt
        ddOnSalePrompt.Items.Add(new ListItem(" - Select -", "0"));
        rsst = DB.GetRS("select * from salesprompt  " + DB.GetNoLock() + " where deleted=0");
        while (rsst.Read())
        {
            ddOnSalePrompt.Items.Add(new ListItem(DB.RSFieldByLocale(rsst, "Name", ThisCustomer.LocaleSetting), DB.RSFieldInt(rsst, "SalesPromptID").ToString()));
        }
        rsst.Close();
        rsst.Dispose();

        //MAPPINGS
        cblCategory.Items.Clear();
        cblAffiliates.Items.Clear();
        cblCustomerLevels.Items.Clear();
        cblGenres.Items.Clear();
        cblVectors.Items.Clear();
        cblDepartment.Items.Clear();

        EntityHelper eTemp = new EntityHelper(EntityDefinitions.readonly_CategoryEntitySpecs);
        ArrayList al = eTemp.GetEntityArrayList(0, "", 0, ThisCustomer.LocaleSetting, false);
        for (int i = 0; i < al.Count; i++)
        {
            ListItemClass lic = (ListItemClass)al[i];
            string value = lic.Value.ToString();
            string name = lic.Item;

            cblCategory.Items.Add(new ListItem(name, value));
        }

        eTemp = new EntityHelper(EntityDefinitions.readonly_SectionEntitySpecs);
        al = eTemp.GetEntityArrayList(0, "", 0, ThisCustomer.LocaleSetting, false);
        for (int i = 0; i < al.Count; i++)
        {
            ListItemClass lic = (ListItemClass)al[i];
            string value = lic.Value.ToString();
            string name = lic.Item;

            cblDepartment.Items.Add(new ListItem(name, value));
        }

        eTemp = new EntityHelper(EntityDefinitions.readonly_AffiliateEntitySpecs);
        al = eTemp.GetEntityArrayList(0, "", 0, ThisCustomer.LocaleSetting, false);
        for (int i = 0; i < al.Count; i++)
        {
            ListItemClass lic = (ListItemClass)al[i];
            string value = lic.Value.ToString();
            string name = lic.Item;

            cblAffiliates.Items.Add(new ListItem(name, value));
        }

        eTemp = new EntityHelper(EntityDefinitions.readonly_CustomerLevelEntitySpecs);
        al = eTemp.GetEntityArrayList(0, "", 0, ThisCustomer.LocaleSetting, false);
        for (int i = 0; i < al.Count; i++)
        {
            ListItemClass lic = (ListItemClass)al[i];
            string value = lic.Value.ToString();
            string name = lic.Item;

            cblCustomerLevels.Items.Add(new ListItem(name, value));
        }

        eTemp = new EntityHelper(EntityDefinitions.readonly_GenreEntitySpecs);
        al = eTemp.GetEntityArrayList(0, "", 0, ThisCustomer.LocaleSetting, false);
        for (int i = 0; i < al.Count; i++)
        {
            ListItemClass lic = (ListItemClass)al[i];
            string value = lic.Value.ToString();
            string name = lic.Item;

            cblGenres.Items.Add(new ListItem(name, value));
        }

        eTemp = new EntityHelper(EntityDefinitions.readonly_VectorEntitySpecs);
        al = eTemp.GetEntityArrayList(0, "", 0, ThisCustomer.LocaleSetting, false);
        for (int i = 0; i < al.Count; i++)
        {
            ListItemClass lic = (ListItemClass)al[i];
            string value = lic.Value.ToString();
            string name = lic.Item;

            cblVectors.Items.Add(new ListItem(name, value));
        }

        txtSwatchImageMap.Columns = Localization.ParseNativeInt(AppLogic.AppConfig("Admin_TextareaWidth"));
        txtSwatchImageMap.Rows = AppLogic.AppConfigNativeInt("Admin_TextareaHeightSmall");

        txtMiscText.Columns = Localization.ParseNativeInt(AppLogic.AppConfig("Admin_TextareaWidth"));
        txtMiscText.Rows = 10;

        txtExtensionData.Columns = Localization.ParseNativeInt(AppLogic.AppConfig("Admin_TextareaWidth"));
        txtExtensionData.Rows = 10;

        txtFroogle.Columns = Localization.ParseNativeInt(AppLogic.AppConfig("Admin_TextareaWidth"));
        txtFroogle.Rows = 10;

        //////////////////////////////////////////////////////////////////////////////////////
        if (Editing)
        {
            TabStrip1.Tabs[TabStrip1.Tabs.Count - 1].Visible = false;
            this.phAllVariants.Visible = true;
            this.phAddVariant.Visible = false;

            SetVariantButtons();

            ltStatus.Text = "Editing Product";
            ltStatus2.Text = "<a href=\"entityproductvariantsoverview.aspx?productid=" + pID.ToString() + "&entityname=" + eName + "&entityid=" + eID.ToString() + "\">Show Variants</a>";
            btnSubmit.Text = "Update";
            ltEntity.Text = entity.GetEntityBreadcrumb6(eID, ThisCustomer.LocaleSetting) + " : <a href=\"entityProducts.aspx?entityName=" + eName + "&EntityFilterID=" + eID + "\">Product Management</a> : " + XmlCommon.GetLocaleEntry(DB.RSField(rs, "Name"), ddLocale.SelectedValue, false) + " (" + pID + ")";

            txtName.Text = XmlCommon.GetLocaleEntry(DB.RSField(rs, "Name"), ddLocale.SelectedValue, false);

            if (!DB.RSFieldBool(rs, "Published"))
            {
                rblPublished.BackColor = System.Drawing.Color.LightYellow;
            }
            rblPublished.SelectedIndex = (DB.RSFieldBool(rs, "Published") ? 1 : 0);
            GoogleCheckoutAllowed.SelectedIndex = (DB.RSFieldBool(rs, "GoogleCheckoutAllowed") ? 1 : 0);

            //match Type
            foreach (ListItem li in ddType.Items)
            {
                if (li.Value.Equals(DB.RSFieldInt(rs, "ProductTypeID").ToString()))
                {
                    ddType.SelectedValue = DB.RSFieldInt(rs, "ProductTypeID").ToString();
                }
            }

            //match Manufacturer
            foreach (ListItem li in ddManufacturer.Items)
            {
                if (li.Value.Equals(AppLogic.GetProductManufacturerID(pID).ToString()))
                {
                    ddManufacturer.SelectedValue = AppLogic.GetProductManufacturerID(pID).ToString();
                }
            }

            //match Distributor
            foreach (ListItem li in ddDistributor.Items)
            {
                if (li.Value.Equals(AppLogic.GetProductDistributorID(pID).ToString()))
                {
                    ddDistributor.SelectedValue = AppLogic.GetProductDistributorID(pID).ToString();
                }
            }

            //match XmlPackage
            ddXmlPackage.ClearSelection();
            foreach (ListItem li in ddXmlPackage.Items)
            {
                if (li.Value.Equals(DB.RSField(rs, "XmlPackage").ToLowerInvariant()))
                {
                    ddXmlPackage.SelectedValue = DB.RSField(rs, "XmlPackage").ToLowerInvariant();
                }
            }

            //match Discount Table
            ddDiscountTable.ClearSelection();
            foreach (ListItem li in ddDiscountTable.Items)
            {
                if (li.Value.Equals(DB.RSFieldInt(rs, "QuantityDiscountID").ToString()))
                {
                    ddDiscountTable.SelectedValue = DB.RSFieldInt(rs, "QuantityDiscountID").ToString();
                }
            }

            //match Tax Class
            ddTaxClass.ClearSelection();
            foreach (ListItem li in ddTaxClass.Items)
            {
                if (li.Value.Equals(DB.RSFieldInt(rs, "TaxClassID").ToString()))
                {
                    ddTaxClass.SelectedValue = DB.RSFieldInt(rs, "TaxClassID").ToString();
                }
            }

            //match On Sale
            ddOnSalePrompt.ClearSelection();
            foreach (ListItem li in ddOnSalePrompt.Items)
            {
                if (li.Value.Equals(DB.RSFieldInt(rs, "SalesPromptID").ToString()))
                {
                    ddOnSalePrompt.SelectedValue = DB.RSFieldInt(rs, "SalesPromptID").ToString();
                }
            }

            txtSKU.Text = DB.RSField(rs, "SKU");
            txtManufacturePartNumber.Text = DB.RSField(rs, "ManufacturerPartNumber");

            rblShowBuyButton.SelectedIndex = (DB.RSFieldBool(rs, "ShowBuyButton") ? 1 : 0);
            rblIsCallToOrder.SelectedIndex = (DB.RSFieldBool(rs, "IsCallToOrder") ? 1 : 0);
            rblHidePriceUntilCart.SelectedIndex = (DB.RSFieldBool(rs, "HidePriceUntilCart") ? 1 : 0);
            rblAddToPacks.SelectedIndex = (DB.RSFieldBool(rs, "ShowInProductBrowser") ? 1 : 0);
            rblExcludeFroogle.SelectedIndex = (DB.RSFieldBool(rs, "ExcludeFromPriceFeeds") ? 1 : 0);

            rblIsKit.SelectedIndex = (DB.RSFieldBool(rs, "IsAKit") ? 1 : 0);
            ltKit.Text = CommonLogic.IIF(DB.RSFieldBool(rs, "IsAKit"), "<a href=\"entityeditkit.aspx?productid=" + DB.RSFieldInt(rs, "ProductID").ToString() + "&entityName=" + eName + "&entityFilterID=" + eID + "\">Edit Kit</a>", "");

            rblIsPack.SelectedIndex = (DB.RSFieldBool(rs, "IsAPack") ? 1 : 0);
            txtPackSize.Text = DB.RSFieldInt(rs, "PackSize").ToString();
            txtPopularity.Text = DB.RSFieldInt(rs, "Popularity").ToString();

            if (IsAKit || IsAPack)
            {
                trInventory1.Visible = false;
                trInventory2.Visible = false;
                trInventory3.Visible = false;
            }
            else
            {
                trInventory1.Visible = true;
                trInventory2.Visible = true;
                trInventory3.Visible = true;

                rblTrackSizeColor.SelectedIndex = CommonLogic.IIF(DB.RSFieldBool(rs, "TrackInventoryBySizeAndColor"), 1, 0);
                txtColorOption.Text = XmlCommon.GetLocaleEntry(DB.RSField(rs, "ColorOptionPrompt"), ddLocale.SelectedValue, false);
                txtSizeOption.Text = XmlCommon.GetLocaleEntry(DB.RSField(rs, "SizeOptionPrompt"), ddLocale.SelectedValue, false);
            }

            rblRequiresTextField.SelectedIndex = CommonLogic.IIF(DB.RSFieldBool(rs, "RequiresTextOption"), 1, 0);
            txtTextFieldPrompt.Text = XmlCommon.GetLocaleEntry(DB.RSField(rs, "TextOptionPrompt"), ddLocale.SelectedValue, false);
            txtTextOptionMax.Text = DB.RSFieldInt(rs, "TextOptionMaxLength").ToString();

            rblRequiresRegistrationToView.SelectedIndex = CommonLogic.IIF(DB.RSFieldBool(rs, "RequiresRegistration"), 1, 0);

            txtPageSize.Text = CommonLogic.IIF(Editing, DB.RSFieldInt(rs, "PageSize").ToString(), AppLogic.AppConfig("Default_" + eSpecs.m_EntityName + "PageSize"));
            txtColumn.Text = CommonLogic.IIF(Editing, DB.RSFieldInt(rs, "ColWidth").ToString(), AppLogic.AppConfig("Default_" + eSpecs.m_EntityName + "ColWidth"));

            //Date
            txtStartDate.Text = Localization.ToNativeShortDateString(DB.RSFieldDateTime(rs, "AvailableStartDate"));
            DateTime stopDate = DB.RSFieldDateTime(rs, "AvailableStopDate");
            txtStopDate.Text = CommonLogic.IIF(stopDate == DateTime.MinValue, String.Empty, Localization.ToNativeShortDateString(stopDate));

            /*DESCRIPTION
            ltDescription.Text = ("<div id=\"idDescription\" style=\"height: 1%;\">");
            ltDescription.Text += ("<textarea rows=\"" + AppLogic.AppConfigUSInt("Admin_TextareaHeight") + "\" cols=\"" + AppLogic.AppConfigUSInt("Admin_TextareaWidth") + "\" id=\"Description\" name=\"Description\">" + XmlCommon.GetLocaleEntry(HttpContext.Current.Server.HtmlEncode(DB.RSField(rs, "Description")), ddLocale.SelectedValue, false) + "</textarea>\n");
            ltDescription.Text += (AppLogic.GenerateInnovaEditor("Description"));
            ltDescription.Text += ("</div>");*/
            //radDescription.Html = XmlCommon.GetLocaleEntry(HttpContext.Current.Server.HtmlEncode(DB.RSField(rs, "Description")), ddLocale.SelectedValue, false);

            radDescription.Html = DB.RSFieldByLocale(rs, "Description", ddLocale.SelectedValue);
            
            btnDescription.Visible = false;
            ltDescriptionFile1.Text = ltDescriptionFile2.Text = "";
            if (pdesc.Contents.Length != 0)
            {
                btnDescription.Visible = true;
                ltDescriptionFile1.Text = ("<b>From File: " + pdesc.URL + "</b> &nbsp;&nbsp;\n");
                ltDescriptionFile2.Text = ("<div style=\"border-style: dashed; border-width: 1px;\">\n");
                ltDescriptionFile2.Text += (pdesc.Contents);
                ltDescriptionFile2.Text += ("</div>\n");
            }

            /*SUMMARY
            ltSummary.Text = ("<div id=\"idSummary\" style=\"height: 1%;\">");
            ltSummary.Text += ("<textarea rows=\"" + AppLogic.AppConfigUSInt("Admin_TextareaHeight") + "\" cols=\"" + AppLogic.AppConfigUSInt("Admin_TextareaWidth") + "\" id=\"Summary\" name=\"Summary\">" + XmlCommon.GetLocaleEntry(HttpContext.Current.Server.HtmlEncode(DB.RSField(rs, "Summary")), ddLocale.SelectedValue, false) + "</textarea>\n");
            ltSummary.Text += (AppLogic.GenerateInnovaEditor("Summary"));
            ltSummary.Text += ("</div>");*/

            radSummary.Html = XmlCommon.GetLocaleEntry(HttpContext.Current.Server.HtmlEncode(DB.RSField(rs, "Summary")), ddLocale.SelectedValue, false);
            //FROOGLE
            txtFroogle.Text = XmlCommon.GetLocaleEntry(DB.RSField(rs, "FroogleDescription"), ddLocale.SelectedValue, false);

            //EXTENSION DATA
            txtExtensionData.Text = DB.RSField(rs, "ExtensionData");

            //MISC TEXT
            txtMiscText.Text = DB.RSField(rs, "MiscText");

            if (AppLogic.AppConfigBool("ShowAutoFill"))
            {
                lnkAutoFill.Visible = true;
                lnkAutoFill.NavigateUrl = "autofill.aspx?productid=" + pID.ToString();
            }
            else
            {
                lnkAutoFill.Visible = false;
            }

            //MAPPINGS
            //Category
            ArrayList alE = EntityHelper.GetProductEntityList(pID, EntityDefinitions.readonly_CategoryEntitySpecs.m_EntityName);
            foreach (ListItem li in cblCategory.Items)
            {
                if (alE.IndexOf(Localization.ParseNativeInt(li.Value)) > -1)
                {
                    li.Selected = true;
                }
            }
            //Section
            alE = EntityHelper.GetProductEntityList(pID, EntityDefinitions.readonly_SectionEntitySpecs.m_EntityName);
            foreach (ListItem li in cblDepartment.Items)
            {
                if (alE.IndexOf(Localization.ParseNativeInt(li.Value)) > -1)
                {
                    li.Selected = true;
                }
            }
            //Affiliate
            alE = EntityHelper.GetProductEntityList(pID, EntityDefinitions.readonly_AffiliateEntitySpecs.m_EntityName);
            foreach (ListItem li in cblAffiliates.Items)
            {
                if (alE.IndexOf(Localization.ParseNativeInt(li.Value)) > -1)
                {
                    li.Selected = true;
                }
            }
            //Customer Level
            alE = EntityHelper.GetProductEntityList(pID, EntityDefinitions.readonly_CustomerLevelEntitySpecs.m_EntityName);
            foreach (ListItem li in cblCustomerLevels.Items)
            {
                if (alE.IndexOf(Localization.ParseNativeInt(li.Value)) > -1)
                {
                    li.Selected = true;
                }
            }
            //Genre
            alE = EntityHelper.GetProductEntityList(pID, EntityDefinitions.readonly_GenreEntitySpecs.m_EntityName);
            foreach (ListItem li in cblGenres.Items)
            {
                if (alE.IndexOf(Localization.ParseNativeInt(li.Value)) > -1)
                {
                    li.Selected = true;
                }
            }
            //Vector
            alE = EntityHelper.GetProductEntityList(pID, EntityDefinitions.readonly_VectorEntitySpecs.m_EntityName);
            foreach (ListItem li in cblVectors.Items)
            {
                if (alE.IndexOf(Localization.ParseNativeInt(li.Value)) > -1)
                {
                    li.Selected = true;
                }
            }


            //RELATED and REQUIRED
            txtRelatedProducts.Text = DB.RSField(rs, "RelatedProducts");
            txtUpsellProducts.Text = DB.RSField(rs, "UpsellProducts");
            txtUpsellProductsDiscount.Text = Localization.FormatDecimal2Places(DB.RSFieldDecimal(rs, "UpsellProductDiscountPercentage"));
            txtRequiresProducts.Text = DB.RSField(rs, "RequiresProducts");

            //SEARCH ENGINE
            txtSETitle.Text = XmlCommon.GetLocaleEntry(DB.RSField(rs, "SETitle"), ddLocale.SelectedValue, false);
            txtSEKeywords.Text = XmlCommon.GetLocaleEntry(DB.RSField(rs, "SEKeywords"), ddLocale.SelectedValue, false);
            txtSEDescription.Text = XmlCommon.GetLocaleEntry(DB.RSField(rs, "SEDescription"), ddLocale.SelectedValue, false);
            txtSENoScript.Text = XmlCommon.GetLocaleEntry(DB.RSField(rs, "SENoScript"), ddLocale.SelectedValue, false);
            //txtSEAlt.Text = XmlCommon.GetLocaleEntry(DB.RSField(rs, "SEAltText"), ddLocale.SelectedValue, false);

            //SPECS
            txtSpecTitle.Text = XmlCommon.GetLocaleEntry(DB.RSField(rs, "SpecTitle"), ddLocale.SelectedValue, false);
            txtSpecCall.Text = DB.RSField(rs, "SpecCall");
            rblSpecsInline.SelectedIndex = (DB.RSFieldBool(rs, "SpecsInline") ? 1 : 0);
            if (pspec.Contents.Length != 0)
            {
                ltSpecs1.Text = ("<b>From File: " + pspec.FN + "</b> &nbsp;&nbsp;\n");
                ltSpecs2.Text = ("<div style=\"border-style: dashed; border-width: 1px;\">\n");
                ltSpecs2.Text += (pspec.Contents);
                ltSpecs2.Text += ("</div>\n");
            }

            //BG COLOR
            txtPageBG.Text = CommonLogic.IIF(Editing, DB.RSField(rs, "PageBGColor"), "");
            txtContentsBG.Text = CommonLogic.IIF(Editing, DB.RSField(rs, "ContentsBGColor"), "");
            txtSkinColor.Text = CommonLogic.IIF(Editing, DB.RSField(rs, "GraphicsColor"), "");

            // BEGIN IMAGES 
            txtImageOverride.Text = CommonLogic.IIF(Editing, DB.RSField(rs, "ImageFilenameOverride"), "");
            bool disableupload = (Editing && DB.RSField(rs, "ImageFilenameOverride") != "");
            if (eSpecs.m_HasIconPic)
            {
                fuIcon.Enabled = !disableupload;
                String Image1URL = AppLogic.LookupImage("Product", pID, "icon", skinID, ThisCustomer.LocaleSetting);
                if (Image1URL.Length == 0)
                {
                    Image1URL = AppLogic.NoPictureImageURL(false, skinID, ThisCustomer.LocaleSetting);
                }
                if (Image1URL.Length != 0)
                {
                    ltIcon.Text = "";
                    if (Image1URL.IndexOf("nopicture") == -1)
                    {
                        ltIcon.Text = ("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image1URL + "','Pic1');\">Click here</a> to delete the current image <br/>\n");
                    }
                    ltIcon.Text += "<img align=\"absmiddle\" style=\"margin-top: 3px; margin-bottom: 5px;\" id=\"Pic1\" name=\"Pic1\" border=\"0\" src=\"" + Image1URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\" />\n";
                    if (AppLogic.GetProductsFirstVariantID(pID) != 0)
                    {
                        ltIcon.Text += ("&nbsp;&nbsp;<a href=\"javascript:;\" onclick=\"window.open('EntityProductImageMgr.aspx?size=icon&productid=" + pID.ToString() + "','Images','height=450, width=780,  scrollbars=yes, resizable=yes, toolbar=no, status=yes, location=no, directories=no, menubar=no, alwaysRaised=yes');\">Icon Multi-Image Manager</a>");
                    }
                }
            }
            if (eSpecs.m_HasMediumPic)
            {
                fuMedium.Enabled = !disableupload;
                String Image2URL = AppLogic.LookupImage("Product", pID, "medium", skinID, ThisCustomer.LocaleSetting);
                if (Image2URL.Length == 0)
                {
                    Image2URL = AppLogic.NoPictureImageURL(false, skinID, ThisCustomer.LocaleSetting);
                }
                if (Image2URL.Length != 0)
                {
                    ltMedium.Text = "";
                    if (Image2URL.IndexOf("nopicture") == -1)
                    {
                        ltMedium.Text = ("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image2URL + "','Pic2');\">Click here</a> to delete the current image <br/>\n");
                    }
                    ltMedium.Text += "<img align=\"absmiddle\" style=\"margin-top: 3px; margin-bottom: 5px;\" id=\"Pic2\" name=\"Pic2\" border=\"0\" src=\"" + Image2URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\" />\n";
                    if (AppLogic.GetProductsFirstVariantID(pID) != 0)
                    {
                        ltMedium.Text += ("&nbsp;&nbsp;<a href=\"javascript:;\" onclick=\"window.open('EntityProductImageMgr.aspx?size=medium&productid=" + pID.ToString() + "','Images','height=550, width=780,  scrollbars=yes, resizable=yes, toolbar=no, status=yes, location=no, directories=no, menubar=no, alwaysRaised=yes');\">Medium Multi-Image Manager</a>");
                    }
                }
            }
            if (eSpecs.m_HasLargePic)
            {
                fuLarge.Enabled = !disableupload;
                String Image3URL = AppLogic.LookupImage("Product", pID, "large", skinID, ThisCustomer.LocaleSetting);
                if (Image3URL.Length == 0)
                {
                    Image3URL = AppLogic.NoPictureImageURL(false, skinID, ThisCustomer.LocaleSetting);
                }
                if (Image3URL.Length != 0)
                {
                    ltLarge.Text = "";
                    if (Image3URL.IndexOf("nopicture") == -1)
                    {
                        ltLarge.Text = ("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image3URL + "','Pic3');\">Click here</a> to delete the current image <br/>\n");
                    }
                    ltLarge.Text += "<img align=\"absmiddle\" style=\"margin-top: 3px; margin-bottom: 5px;\" id=\"Pic3\" name=\"Pic3\" border=\"0\" src=\"" + Image3URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\" />\n";
                    if (AppLogic.GetProductsFirstVariantID(pID) != 0)
                    {
                        ltLarge.Text += ("&nbsp;&nbsp;<a href=\"javascript:;\" onclick=\"window.open('EntityProductImageMgr.aspx?size=large&productid=" + pID.ToString() + "','Images','height=650, width=780,  scrollbars=yes, resizable=yes, toolbar=no, status=yes, location=no, directories=no, menubar=no, alwaysRaised=yes');\">Large Multi-Image Manager</a>");
                    }
                }
            }
            String Image4URL = AppLogic.LookupImage("Product", pID, "swatch", skinID, ThisCustomer.LocaleSetting);
            if (Image4URL.Length == 0)
            {
                Image4URL = AppLogic.NoPictureImageURL(false, skinID, ThisCustomer.LocaleSetting);
            }
            if (Image4URL.Length != 0)
            {
                ltSwatch.Text = "";
                if (Image4URL.IndexOf("nopicture") == -1)
                {
                    ltSwatch.Text = ("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image4URL + "','Pic4');\">Click here</a> to delete the current image <br/>\n");
                }
                ltSwatch.Text += "<img style=\"margin-top: 3px; margin-bottom: 5px;\" id=\"Pic4\" name=\"Pic4\" border=\"0\" src=\"" + Image4URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\" />\n";
            }
            txtSwatchImageMap.Text = DB.RSField(rs, "SwatchImageMap");
            // END IMAGES   

            //VARIANTS
        }
        else
        {
            TabStrip1.Tabs[TabStrip1.Tabs.Count - 1].Visible = true;

            this.phAllVariants.Visible = false;
            this.phAddVariant.Visible = true;
            ltStatus.Text = "Adding Product";
            btnSubmit.Text = "Add New";

            ltEntity.Text = entity.GetEntityBreadcrumb6(eID, ThisCustomer.LocaleSetting) + " : <a href=\"entityProducts.aspx?entityName=" + eName + "&EntityFilterID=" + eID + "\">Product Management</a> : Adding New Product";

            txtPageSize.Text = AppLogic.AppConfig("Default_" + eSpecs.m_EntityName + "PageSize");
            txtColumn.Text = AppLogic.AppConfig("Default_" + eSpecs.m_EntityName + "ColWidth");

            //match Type
            foreach (ListItem li in ddType.Items)
            {
                if (li.Value.Equals(AppLogic.AppConfigUSInt("Admin_DefaultProductTypeID").ToString()))
                {
                    ddType.SelectedValue = AppLogic.AppConfigUSInt("Admin_DefaultProductTypeID").ToString();
                }
            }

            //match On Sale
            foreach (ListItem li in ddOnSalePrompt.Items)
            {
                if (li.Value.Equals(AppLogic.AppConfigUSInt("Admin_DefaultSalesPromptID").ToString()))
                {
                    ddOnSalePrompt.SelectedValue = AppLogic.AppConfigUSInt("Admin_DefaultSalesPromptID").ToString();
                }
            }

            /*DESCRIPTION
            ltDescription.Text = ("<div id=\"idDescription\" style=\"height: 1%;\">");
            ltDescription.Text += ("<textarea rows=\"" + AppLogic.AppConfigUSInt("Admin_TextareaHeight") + "\" cols=\"" + AppLogic.AppConfigUSInt("Admin_TextareaWidth") + "\" id=\"Description\" name=\"Description\"></textarea>\n");
            ltDescription.Text += (AppLogic.GenerateInnovaEditor("Description"));
            ltDescription.Text += ("</div>");*/

            /*SUMMARY
            ltSummary.Text = ("<div id=\"idSummary\" style=\"height: 1%;\">");
            ltSummary.Text += ("<textarea rows=\"" + AppLogic.AppConfigUSInt("Admin_TextareaHeight") + "\" cols=\"" + AppLogic.AppConfigUSInt("Admin_TextareaWidth") + "\" id=\"Summary\" name=\"Summary\"></textarea>\n");
            ltSummary.Text += (AppLogic.GenerateInnovaEditor("Summary"));
            ltSummary.Text += ("</div>");*/

            //EXTENSION DATA
            txtExtensionData.Text = CommonLogic.IIF(Editing, DB.RSField(rs, "ExtensionData"), "");

            //MISCELLANEOUS DATA
            txtExtensionData.Text = CommonLogic.IIF(Editing, DB.RSField(rs, "MiscellaneousData"), "");

            //VARIANT
            trInventory.Visible = false;
            if (!ProductTracksInventoryBySizeAndColor)
            {
                trInventory.Visible = true;
            }

            //set defaults
            rblAddToPacks.SelectedIndex = 1;
            rblExcludeFroogle.SelectedIndex = 1;
            rblHidePriceUntilCart.SelectedIndex = 0;
            rblIsCallToOrder.SelectedIndex = 0;
            rblIsKit.SelectedIndex = 0;
            rblIsPack.SelectedIndex = 0;
            rblPublished.SelectedIndex = 1;
            GoogleCheckoutAllowed.SelectedIndex = 1;
            rblRequiresRegistrationToView.SelectedIndex = 0;
            rblRequiresTextField.SelectedIndex = 0;
            rblShowBuyButton.SelectedIndex = 1;
            rblSpecsInline.SelectedIndex = 0;
            rblTrackSizeColor.SelectedIndex = 0;

            //MAPPINGS
            if (eID > 0)
            {
                //match Manufacturer
                if (eName.ToUpperInvariant() == "MANUFACTURER")
                {
                    ddManufacturer.ClearSelection();
                    foreach (ListItem li in ddManufacturer.Items)
                    {
                        if (li.Value.Equals(eID.ToString()))
                        {
                            ddManufacturer.SelectedValue = eID.ToString();
                        }
                    }
                }

                //match Distributor
                if (eName.ToUpperInvariant() == "DISTRIBUTOR")
                {
                    ddDistributor.ClearSelection();
                    foreach (ListItem li in ddDistributor.Items)
                    {
                        if (li.Value.Equals(eID.ToString()))
                        {
                            ddDistributor.SelectedValue = eID.ToString();
                        }
                    }
                }

                //Category
                if (eName.ToUpperInvariant() == "CATEGORY")
                {
                    foreach (ListItem li in cblCategory.Items)
                    {
                        if (li.Value.Equals(eID.ToString()))
                        {
                            li.Selected = true;
                        }
                    }
                }

                //Section
                if (eName.ToUpperInvariant() == "SECTION")
                {
                    foreach (ListItem li in cblDepartment.Items)
                    {
                        if (li.Value.Equals(eID.ToString()))
                        {
                            li.Selected = true;
                        }
                    }
                }

                //Affiliate
                if (eName.ToUpperInvariant() == "AFFILIATE")
                {
                    foreach (ListItem li in cblAffiliates.Items)
                    {
                        if (li.Value.Equals(eID.ToString()))
                        {
                            li.Selected = true;
                        }
                    }
                }
            }
        }

        ltScript2.Text = ("<script type=\"text/javascript\">\n");
        ltScript2.Text += ("function DeleteImage(imgurl,name)\n");
        ltScript2.Text += ("{\n");
        ltScript2.Text += ("if(confirm('Are you sure you want to delete this image?')){\n");
        ltScript2.Text += ("window.open('deleteimage.aspx?imgurl=' + imgurl + '&FormImageName=' + name,\"Admin_ML\",\"height=250,width=440,top=10,left=20,status=no,toolbar=no,menubar=no,scrollbars=yes,location=no\")\n");
        ltScript2.Text += ("}}\n");
        ltScript2.Text += ("</SCRIPT>\n");

        //create iframe
        ltIFrame.Text = "<iframe src=\"entityProductVariants.aspx?ProductID=" + pID + "&entityname=" + eSpecs.m_EntityName + "&EntityID=" + eID.ToString() + "\" name=\"variantFrame\" id=\"variantFrame\" frameborder=\"0\" height=\"350\" onfocus=\"calcHeight();\" scrolling=\"auto\" width=\"100%\" marginheight=\"0\" marginwidth=\"0\"></iframe>";
        Pageview10.Attributes.Add("onFocus", "calcHeight();");

        rs.Close();
    }


    protected void ddLocale_SelectedIndexChanged(object sender, EventArgs e)
    {
        ThisCustomer.ThisCustomerSession.SetVal("entityUserLocale", ddLocale.SelectedValue);
        LoadContent();
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        UpdateProduct();
    }

    protected void UpdateProduct()
    {
        bool Editing = Localization.ParseBoolean(ViewState["ProductEdit"].ToString());
        StringBuilder sql = new StringBuilder(2500);
        String LargeProductImage = AppLogic.LookupImage("Product", pID, "large", skinID, ThisCustomer.LocaleSetting);

        if (!Editing)
        {
            // ok to ADD them:
            String NewGUID = DB.GetNewGUID();
            sql.Append("insert into product(ProductGUID,Name,SEName,ContentsBGColor,PageBGColor,GraphicsColor,ImageFilenameOverride,ProductTypeID,Summary,Description,ExtensionData,ColorOptionPrompt,SizeOptionPrompt,RequiresTextOption,TextOptionPrompt,TextOptionMaxLength,FroogleDescription,RelatedProducts,UpsellProducts,UpsellProductDiscountPercentage,RequiresProducts,SEKeywords,SEDescription,SETitle,SENoScript,SKU,PageSize,ColWidth,XmlPackage,ManufacturerPartNumber,SalesPromptID,SpecTitle,SpecCall,Published,GoogleCheckoutAllowed,ShowBuyButton,IsCallToOrder,HidePriceUntilCart,ShowInProductBrowser,ExcludeFromPriceFeeds,IsAKit,IsAPack,PackSize" + CommonLogic.IIF(AppLogic.AppConfigBool("ShowPopularity"), ",Popularity", "") + ",TrackInventoryBySizeAndColor,TrackInventoryBySize,TrackInventoryByColor,RequiresRegistration,SpecsInline,MiscText,SwatchImageMap,QuantityDiscountID,TaxClassID,AvailableStartDate,AvailableStopDate) values(");
            sql.Append(DB.SQuote(NewGUID) + ",");
            sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Name", txtName.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            sql.Append(DB.SQuote(SE.MungeName(AppLogic.GetFormsDefaultLocale("Name", txtName.Text, ddLocale.SelectedValue, "Product", pID))) + ",");
            sql.Append(DB.SQuote(txtContentsBG.Text) + ",");
            sql.Append(DB.SQuote(txtPageBG.Text) + ",");
            sql.Append(DB.SQuote(txtSkinColor.Text) + ",");
            sql.Append(DB.SQuote(txtImageOverride.Text) + ",");
            sql.Append(ddType.SelectedValue + ",");
            /*            sql.Append(DB.SQuote(AppLogic.FormLocaleXmlEditor("Summary", "Summary", ddLocale.SelectedValue, "Product", pID)) + ",");
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXmlEditor("Description", "Description", ddLocale.SelectedValue, "Product", pID)) + ",");
             */
            sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Summary", radSummary.Html, ddLocale.SelectedValue, eSpecs, eID)) + ",");
            sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Description", radDescription.Html, ddLocale.SelectedValue, eSpecs, eID)) + ",");
            sql.Append(DB.SQuote(txtExtensionData.Text) + ",");
            sql.Append(DB.SQuote(AppLogic.FormLocaleXml("ColorOptionPrompt", txtColorOption.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SizeOptionPrompt", txtSizeOption.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            sql.Append(rblRequiresTextField.SelectedValue + ",");
            sql.Append(DB.SQuote(AppLogic.FormLocaleXml("TextOptionPrompt", txtTextFieldPrompt.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            if (txtTextOptionMax.Text.Length != 0)
            {
                sql.Append(Localization.ParseNativeInt(txtTextOptionMax.Text).ToString() + ",");
            }
            else
            {
                sql.Append("NULL,");
            }
            sql.Append(DB.SQuote(AppLogic.FormLocaleXml("FroogleDescription", txtFroogle.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            if (txtRelatedProducts.Text.Length != 0)
            {
                sql.Append(DB.SQuote(txtRelatedProducts.Text.Trim().Replace(" ", "")) + ",");
            }
            else
            {
                sql.Append("NULL,");
            }
            if (txtUpsellProducts.Text.Length != 0)
            {
                sql.Append(DB.SQuote(txtUpsellProducts.Text.Trim().Replace(" ", "")) + ",");
            }
            else
            {
                sql.Append("NULL,");
            }
            sql.Append(Localization.DecimalStringForDB(Localization.ParseNativeDecimal(txtUpsellProductsDiscount.Text)) + ",");
            if (txtRequiresProducts.Text.Length != 0)
            {
                sql.Append(DB.SQuote(txtRequiresProducts.Text.Trim().Replace(" ", "")) + ",");
            }
            else
            {
                sql.Append("NULL,");
            }

            if (txtSEKeywords.Text.Length != 0)
            {
                sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SEKeywords", txtSEKeywords.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            }
            else
            {
                sql.Append("NULL,");
            }
            if (txtSEDescription.Text.Length != 0)
            {
                sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SEDescription", txtSEDescription.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            }
            else
            {
                sql.Append("NULL,");
            }
            if (txtSETitle.Text.Length != 0)
            {
                sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SETitle", txtSETitle.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            }
            else
            {
                sql.Append("NULL,");
            }
            if (txtSENoScript.Text.Length != 0)
            {
                sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SENoScript", txtSENoScript.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            }
            else
            {
                sql.Append("NULL,");
            }
            //if (txtSEAlt.Text.Length != 0)
            //{
            //    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SEAltText", txtSEAlt.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            //}
            //else
            //{
            //    sql.Append("NULL,");
            //}
            if (txtSKU.Text.Length != 0)
            {
                sql.Append(DB.SQuote(txtSKU.Text) + ",");
            }
            else
            {
                sql.Append("NULL,");
            }
            sql.Append(CommonLogic.IIF(txtPageSize.Text.Length == 0, AppLogic.AppConfig("Default_ProductPageSize"), txtPageSize.Text) + ",");
            sql.Append(CommonLogic.IIF(txtColumn.Text.Length == 0, AppLogic.AppConfig("Default_ProductColWidth"), txtColumn.Text) + ",");
            if (ddXmlPackage.SelectedValue != "0")
            {
                sql.Append(DB.SQuote(ddXmlPackage.SelectedValue.ToLowerInvariant()) + ",");
            }
            else
            {
                if (rblIsPack.SelectedValue == "1")
                {
                    sql.Append(DB.SQuote(AppLogic.ro_DefaultProductPackXmlPackage) + ","); // force a default!
                }
                else if (rblIsKit.SelectedValue == "1")
                {
                    sql.Append(DB.SQuote(AppLogic.ro_DefaultProductKitXmlPackage) + ","); // force a default!
                }
                else
                {
                    sql.Append(DB.SQuote(AppLogic.ro_DefaultProductXmlPackage) + ","); // force a default!
                }
            }
            sql.Append(DB.SQuote(txtManufacturePartNumber.Text) + ",");
            sql.Append(ddOnSalePrompt.SelectedValue + ",");
            sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SpecTitle", txtSpecTitle.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            if (txtSpecCall.Text.Length != 0)
            {
                sql.Append(DB.SQuote(txtSpecCall.Text) + ",");
            }
            else
            {
                sql.Append("NULL,");
            }
            sql.Append(rblPublished.SelectedValue + ",");
            sql.Append(GoogleCheckoutAllowed.SelectedValue + ",");
            sql.Append(rblShowBuyButton.SelectedValue + ",");
            sql.Append(rblIsCallToOrder.SelectedValue + ",");
            sql.Append(rblHidePriceUntilCart.SelectedValue + ",");
            sql.Append(rblAddToPacks.SelectedValue + ",");
            sql.Append(rblExcludeFroogle.SelectedValue + ",");
            sql.Append(rblIsKit.SelectedValue + ",");
            sql.Append(rblIsPack.SelectedValue + ",");
            sql.Append(Localization.ParseNativeInt(txtPackSize.Text).ToString() + ",");
            if (AppLogic.AppConfigBool("ShowPopularity"))
            {
                sql.Append(Localization.ParseNativeInt(txtPopularity.Text).ToString() + ",");
            }
            if (rblIsKit.SelectedValue == "1" || rblIsPack.SelectedValue == "1")
            {
                sql.Append("0,"); // cannot track inventory by size and color
                sql.Append("0,"); // cannot track inventory by size and color
                sql.Append("0,"); // cannot track inventory by size and color
            }
            else
            {
                sql.Append(rblTrackSizeColor.SelectedValue + ",");
                sql.Append(rblTrackSizeColor.SelectedValue + ","); // this is correct. change made in v6.1.1.1
                sql.Append(rblTrackSizeColor.SelectedValue + ","); // this is correct. change made in v6.1.1.1
            }
            sql.Append(rblRequiresRegistrationToView.SelectedValue + ",");
            sql.Append(rblSpecsInline.SelectedValue + ",");
            if (txtMiscText.Text.Length != 0)
            {
                sql.Append(DB.SQuote(txtMiscText.Text) + ",");
            }
            else
            {
                sql.Append("NULL,");
            }
            if (txtSwatchImageMap.Text.Length != 0)
            {
                sql.Append(DB.SQuote(txtSwatchImageMap.Text) + ",");
            }
            else
            {
                sql.Append("NULL,");
            }
            sql.Append(ddDiscountTable.SelectedValue + ",");
            sql.Append(ddTaxClass.SelectedValue + ",");

            //Dates
            if (txtStartDate.Text.Trim().Length != 0)
            {
                sql.Append(DB.SQuote(Localization.ToDBShortDateString(Localization.ParseNativeDateTime(txtStartDate.Text))) + ",");
            }
            else
            {
                sql.Append("getdate(),");
            }
            if (txtStopDate.Text.Trim().Length != 0)
            {
                sql.Append(DB.SQuote(Localization.ToDBShortDateString(Localization.ParseNativeDateTime(txtStopDate.Text))) + "");
            }
            else
            {
                sql.Append("NULL");
            }

            sql.Append(")");
            DB.ExecuteSQL(sql.ToString());

            IDataReader rs = DB.GetRS("select ProductID from product  " + DB.GetNoLock() + " where deleted=0 and ProductGUID=" + DB.SQuote(NewGUID));
            rs.Read();
            pID = DB.RSFieldInt(rs, "ProductID");
            ViewState.Add("ProductEdit", true);
            ViewState.Add("ProductEditID", pID);
            rs.Close();

            // ARE WE ADDING A SIMPLE PRODUCT, IF SO, CREATE THE DEFAULT VARIANT
            if (txtPrice.Text.Length != 0)
            {
                // ok to add:
                NewGUID = DB.GetNewGUID();
                sql.Remove(0, sql.Length);
                sql.Append("insert into productvariant(VariantGUID,Name,IsDefault,ProductID,Price,SalePrice,Weight,Dimensions,Inventory,Published,Colors,ColorSKUModifiers,Sizes,SizeSKUModifiers) values(");
                sql.Append(DB.SQuote(NewGUID) + ",");
                sql.Append(DB.SQuote("Default " + txtName.Text + " Variant") + ","); // add default variant name
                sql.Append("1,"); // IsDefault=1
                sql.Append(pID.ToString() + ",");
                sql.Append(Localization.DecimalStringForDB(Localization.ParseNativeDecimal(txtPrice.Text)) + ",");
                sql.Append(CommonLogic.IIF(txtSalePrice.Text.Length != 0, Localization.DecimalStringForDB(Localization.ParseNativeDecimal(txtSalePrice.Text)), "NULL") + ",");
                sql.Append(CommonLogic.IIF(txtWeight.Text.Length != 0, Localization.DecimalStringForDB(Localization.ParseNativeDecimal(txtWeight.Text)), "NULL") + ",");
                sql.Append(DB.SQuote(txtDimensions.Text) + ",");
                sql.Append(CommonLogic.IIF(txtInventory.Text.Length != 0, Localization.ParseNativeInt(txtInventory.Text).ToString(), AppLogic.AppConfigNativeInt("Admin_DefaultInventory").ToString()) + ",");
                sql.Append("1,");
                sql.Append(DB.SQuote(txtColors.Text) + ",");
                sql.Append(DB.SQuote(txtColorSKUModifiers.Text) + ",");
                sql.Append(DB.SQuote(txtSizes.Text) + ",");
                sql.Append(DB.SQuote(txtSizeSKUModifiers.Text));
                sql.Append(")");
                DB.ExecuteSQL(sql.ToString());

                resetError("Product Added and Default variant were added.", false);
            }
            else
            {
                resetError("Product Added. Default variant was NOT added. Please create a Variant for " + txtName.Text, false);
            }
            ltScript3.Text = "<script type=\"text/javascript\">parent.frames['entityMenu'].location.href = 'entityMenu.aspx?entityName=" + eSpecs.m_EntityName + "&entityID=" + eID + "';</script>";
        }
        else
        {
            // ok to update:
            sql.Append("update product set ");
            sql.Append("Name=" + DB.SQuote(AppLogic.FormLocaleXml("Name", txtName.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            sql.Append("SEName=" + DB.SQuote(SE.MungeName(AppLogic.GetFormsDefaultLocale("Name", txtName.Text, ddLocale.SelectedValue, "Product", pID))) + ",");
            sql.Append("ContentsBGColor=" + DB.SQuote(txtContentsBG.Text) + ",");
            sql.Append("PageBGColor=" + DB.SQuote(txtPageBG.Text) + ",");
            sql.Append("GraphicsColor=" + DB.SQuote(txtSkinColor.Text) + ",");
            sql.Append("ImageFilenameOverride=" + DB.SQuote(txtImageOverride.Text) + ",");
            sql.Append("ProductTypeID=" + ddType.SelectedValue + ",");
            /*            sql.Append("Summary=" + DB.SQuote(AppLogic.FormLocaleXmlEditor("Summary", "Summary", ddLocale.SelectedValue, "Product", pID)) + ",");
                        sql.Append("Description=" + DB.SQuote(AppLogic.FormLocaleXmlEditor("Description", "Description", ddLocale.SelectedValue, "Product", pID)) + ",");
            */
            sql.Append("Summary=" + DB.SQuote(AppLogic.FormLocaleXml("Summary", radSummary.Html, ddLocale.SelectedValue, eSpecs, eID)) + ",");
            sql.Append("Description=" + DB.SQuote(AppLogic.FormLocaleXml("Description", radDescription.Html, ddLocale.SelectedValue, eSpecs, eID)) + ",");

            sql.Append("ExtensionData=" + DB.SQuote(txtExtensionData.Text) + ",");
            sql.Append("ColorOptionPrompt=" + DB.SQuote(AppLogic.FormLocaleXml("ColorOptionPrompt", txtColorOption.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            sql.Append("SizeOptionPrompt=" + DB.SQuote(AppLogic.FormLocaleXml("SizeOptionPrompt", txtSizeOption.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            sql.Append("RequiresTextOption=" + rblRequiresTextField.SelectedValue + ",");
            sql.Append("TextOptionPrompt=" + DB.SQuote(AppLogic.FormLocaleXml("TextOptionPrompt", txtTextFieldPrompt.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            sql.Append("TextOptionMaxLength=" + Localization.ParseNativeInt(txtTextOptionMax.Text) + ",");
            sql.Append("FroogleDescription=" + DB.SQuote(AppLogic.FormLocaleXml("FroogleDescription", txtFroogle.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            sql.Append("RelatedProducts=" + DB.SQuote(txtRelatedProducts.Text.Trim().Replace(" ", "")) + ",");
            sql.Append("UpsellProducts=" + DB.SQuote(txtUpsellProducts.Text.Trim().Replace(" ", "")) + ",");
            sql.Append("UpsellProductDiscountPercentage=" + Localization.DecimalStringForDB(Localization.ParseNativeDecimal(txtUpsellProductsDiscount.Text)) + ",");
            sql.Append("RequiresProducts=" + DB.SQuote(txtRequiresProducts.Text.Trim().Replace(" ", "")) + ",");

            sql.Append("SEKeywords=" + DB.SQuote(AppLogic.FormLocaleXml("SEKeywords", txtSEKeywords.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            sql.Append("SEDescription=" + DB.SQuote(AppLogic.FormLocaleXml("SEDescription", txtSEDescription.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            sql.Append("SETitle=" + DB.SQuote(AppLogic.FormLocaleXml("SETitle", txtSETitle.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            sql.Append("SENoScript=" + DB.SQuote(AppLogic.FormLocaleXml("SENoScript", txtSENoScript.Text, ddLocale.SelectedValue, "Product", pID)) + ",");
            //sql.Append("SEAltText=" + DB.SQuote(AppLogic.FormLocaleXml("SEAltText", txtSEAlt.Text, ddLocale.SelectedValue, "Product", pID)) + ",");

            if (txtSKU.Text.Length != 0)
            {
                sql.Append("SKU=" + DB.SQuote(txtSKU.Text) + ",");
            }
            else
            {
                sql.Append("SKU=NULL,");
            }

            sql.Append("PageSize=" + CommonLogic.IIF(txtPageSize.Text.Length == 0, AppLogic.AppConfigNativeInt("Default_ProductPageSize").ToString(), Localization.ParseNativeInt(txtPageSize.Text).ToString()) + ",");
            sql.Append("ColWidth=" + CommonLogic.IIF(txtColumn.Text.Length == 0, AppLogic.AppConfigNativeInt("Default_ProductColWidth").ToString(), Localization.ParseNativeInt(txtColumn.Text).ToString()) + ",");

            if (ddXmlPackage.SelectedValue != "0")
            {
                sql.Append("XmlPackage=" + DB.SQuote(ddXmlPackage.SelectedValue.ToLowerInvariant()) + ",");
            }
            else
            {
                if (rblIsPack.SelectedValue == "1")
                {
                    sql.Append("XmlPackage=" + DB.SQuote(AppLogic.ro_DefaultProductPackXmlPackage) + ","); // force a default!
                }
                else if (rblIsKit.SelectedValue == "1")
                {
                    sql.Append("XmlPackage=" + DB.SQuote(AppLogic.ro_DefaultProductKitXmlPackage) + ","); // force a default!
                }
                else
                {
                    sql.Append("XmlPackage=" + DB.SQuote(AppLogic.ro_DefaultProductXmlPackage) + ","); // force a default!
                }
            }

            sql.Append("ManufacturerPartNumber=" + DB.SQuote(txtManufacturePartNumber.Text) + ",");
            sql.Append("SalesPromptID=" + Localization.ParseNativeInt(ddOnSalePrompt.SelectedValue).ToString() + ",");
            sql.Append("SpecTitle=" + DB.SQuote(AppLogic.FormLocaleXml("SpecTitle", txtSpecTitle.Text, ddLocale.SelectedValue, "Product", pID)) + ",");

            if (txtSpecCall.Text.Length != 0)
            {
                sql.Append("SpecCall=" + DB.SQuote(txtSpecCall.Text) + ",");
            }
            else
            {
                sql.Append("SpecCall=NULL,");
            }
            sql.Append("Published=" + rblPublished.SelectedValue + ",");
            sql.Append("GoogleCheckoutAllowed=" + GoogleCheckoutAllowed.SelectedValue + ",");
            sql.Append("ShowBuyButton=" + rblShowBuyButton.SelectedValue + ",");
            sql.Append("IsCallToOrder=" + rblIsCallToOrder.SelectedValue + ",");
            sql.Append("HidePriceUntilCart=" + rblHidePriceUntilCart.SelectedValue + ",");
            sql.Append("ShowInProductBrowser=" + rblAddToPacks.SelectedValue + ",");
            sql.Append("ExcludeFromPriceFeeds=" + rblExcludeFroogle.SelectedValue + ",");
            sql.Append("IsAKit=" + rblIsKit.SelectedValue + ",");
            sql.Append("IsAPack=" + rblIsPack.SelectedValue + ",");
            sql.Append("PackSize=" + Localization.ParseNativeInt(txtPackSize.Text).ToString() + ",");
            if (AppLogic.AppConfigBool("ShowPopularity"))
            {
                sql.Append("Popularity=" + Localization.ParseNativeInt(txtPopularity.Text).ToString() + ",");
            }

            if (rblIsKit.SelectedValue == "1" || rblIsPack.SelectedValue == "1")
            {
                sql.Append("TrackInventoryBySizeAndColor=0,"); // cannot track inventory by size and color
                sql.Append("TrackInventoryBySize=0,");
                sql.Append("TrackInventoryByColor=0,");
            }
            else
            {
                sql.Append("TrackInventoryBySizeAndColor=" + rblTrackSizeColor.SelectedValue + ",");
                sql.Append("TrackInventoryBySize=" + rblTrackSizeColor.SelectedValue + ","); // this is correct. change made in v6.1.1.1
                sql.Append("TrackInventoryByColor=" + rblTrackSizeColor.SelectedValue + ","); // this is correct. change made in v6.1.1.1
            }

            sql.Append("RequiresRegistration=" + rblRequiresRegistrationToView.SelectedValue + ",");
            sql.Append("SpecsInline=" + rblSpecsInline.SelectedValue + ",");

            if (txtMiscText.Text.Length != 0)
            {
                sql.Append("MiscText=" + DB.SQuote(txtMiscText.Text) + ",");
            }
            else
            {
                sql.Append("MiscText=NULL,");
            }
            if (txtSwatchImageMap.Text.Length != 0)
            {
                sql.Append("SwatchImageMap=" + DB.SQuote(txtSwatchImageMap.Text) + ",");
            }
            else
            {
                sql.Append("SwatchImageMap=NULL,");
            }
            sql.Append("QuantityDiscountID=" + Localization.ParseNativeInt(ddDiscountTable.SelectedValue) + ",");

            sql.Append("TaxClassID=" + Localization.ParseNativeInt(ddTaxClass.SelectedValue) + ",");

            //Dates
            if (txtStartDate.Text.Length != 0)
            {
                sql.Append("AvailableStartDate=" + DB.SQuote(Localization.ToDBShortDateString(Localization.ParseNativeDateTime(txtStartDate.Text))) + ",");
            }
            else
            {
                sql.Append("AvailableStartDate=getdate(),");
            }
            if (txtStopDate.Text.Length != 0)
            {
                sql.Append("AvailableStopDate=" + DB.SQuote(Localization.ToDBShortDateString(Localization.ParseNativeDateTime(txtStopDate.Text))) + "");
            }
            else
            {
                sql.Append("AvailableStopDate=NULL");
            }

            sql.Append(" where ProductID=" + pID.ToString());
            DB.ExecuteSQL(sql.ToString());

            ViewState.Add("ProductEdit", true);
            ViewState.Add("ProductEditID", pID);

            resetError("Product Updated.", false);
        }

        // UPDATE 1:1 ENTITY MAPPINGS:
        {
            String[] Entities = { "Manufacturer", "Distributor" };
            foreach (String en in Entities)
            {
                int NewID = 0;
                if (en.Equals("Manufacturer"))
                {
                    NewID = Localization.ParseNativeInt(ddManufacturer.SelectedValue);
                }
                else
                {
                    NewID = Localization.ParseNativeInt(ddDistributor.SelectedValue);
                }
                if (NewID == 0)
                {
                    // no mapping (should not be allowed by form validator, but...)
                    DB.ExecuteSQL("delete from Product" + en + " where ProductID=" + pID.ToString());
                }
                else
                {
                    int OldID = CommonLogic.IIF(en == "Manufacturer", AppLogic.GetProductManufacturerID(pID), AppLogic.GetProductDistributorID(pID));
                    if (OldID == 0)
                    {
                        // create default mapping:
                        DB.ExecuteSQL(String.Format("insert into Product{0}(ProductID,{1}ID,DisplayOrder) values({2},{3},1)", en, en, pID.ToString(), NewID.ToString()));
                    }
                    else if (OldID != NewID)
                    {
                        // update existing mapping:
                        DB.ExecuteSQL(String.Format("update Product{0} set {1}ID={2} where {3}ID={4} and ProductID={5}", en, en, NewID.ToString(), en, OldID.ToString(), pID.ToString()));
                    }
                }
            }
        }

        // UPDATE 1:N ENTITY MAPPINGS:
        {
            String[] Entities2 = { "Category", "Section", "Affiliate", "CustomerLevel", "Genre", "Vector" };
            foreach (String en in Entities2)
            {
                //String EnMap = CommonLogic.FormCanBeDangerousContent(en + "Map");
                String EnMap = "";
                if (en.Equals("Category"))
                {
                    foreach (ListItem li in cblCategory.Items)
                    {
                        if (li.Selected)
                        {
                            EnMap += "," + li.Value;
                        }
                    }
                }
                else if (en.Equals("Section"))
                {
                    foreach (ListItem li in cblDepartment.Items)
                    {
                        if (li.Selected)
                        {
                            EnMap += "," + li.Value;
                        }
                    }
                }
                else if (en.Equals("Affiliate"))
                {
                    foreach (ListItem li in cblAffiliates.Items)
                    {
                        if (li.Selected)
                        {
                            EnMap += "," + li.Value;
                        }
                    }
                }
                else if (en.Equals("CustomerLevel"))
                {
                    foreach (ListItem li in cblCustomerLevels.Items)
                    {
                        if (li.Selected)
                        {
                            EnMap += "," + li.Value;
                        }
                    }
                }
                else if (en.Equals("Genre"))
                {
                    foreach (ListItem li in cblGenres.Items)
                    {
                        if (li.Selected)
                        {
                            EnMap += "," + li.Value;
                        }
                    }
                }
                else
                {
                    foreach (ListItem li in cblVectors.Items)
                    {
                        if (li.Selected)
                        {
                            EnMap += "," + li.Value;
                        }
                    }
                }

                if (EnMap.Length > 1)
                {
                    EnMap = EnMap.Substring(1);// Remove the leading ,
                }

                if (EnMap.Length == 0)
                {
                    // no mappings
                    DB.ExecuteSQL(String.Format("delete from Product{0} where ProductID={1}", en, pID.ToString()));
                }
                else
                {
                    // remove any mappings not current anymore:
                    DB.ExecuteSQL(String.Format("delete from Product{0} where ProductID={1} and {2}ID not in ({3})", en, pID.ToString(), en, EnMap));
                    // add new default mappings:
                    String[] EnMapArray = EnMap.Split(',');
                    foreach (String EntityID in EnMapArray)
                    {
                        try
                        {
                            DB.ExecuteSQL(String.Format("insert Product{0}(ProductID,{1}ID,DisplayOrder) values({2},{3},1)", en, en, pID.ToString(), EntityID));
                        }
                        catch { }
                    }
                }
            }
        }

        //Upload Images
        HandleImageSubmits();

        LoadContent();
    }

    public void HandleImageSubmits()
    {
        // handle image uploaded:
        String FN = txtImageOverride.Text.Trim();
        if (AppLogic.AppConfigBool("UseSKUForProductImageName"))
        {
            FN = CommonLogic.FormCanBeDangerousContent("txtSKU").Trim();
        }
        if (FN.Length == 0)
        {
            FN = pID.ToString();
        }
        String ErrorMsg = String.Empty;

        String Image1 = String.Empty;
        String TempImage1 = String.Empty;
        HttpPostedFile Image1File = fuIcon.PostedFile;
        if (Image1File != null && Image1File.ContentLength != 0)
        {
            // delete any current image file first
            try
            {
                foreach (String ss in CommonLogic.SupportedImageTypes)
                {
                    if (FN.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) || FN.EndsWith(".gif", StringComparison.InvariantCultureIgnoreCase) || FN.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
                    {
                        System.IO.File.Delete(AppLogic.GetImagePath("Product", "icon", true) + FN);
                    }
                    else
                    {
                        System.IO.File.Delete(AppLogic.GetImagePath("Product", "icon", true) + FN + ss);
                    }
                }
            }
            catch
            { }

            String s = Image1File.ContentType;
            switch (Image1File.ContentType)
            {
                case "image/gif":
                    TempImage1 = AppLogic.GetImagePath("Product", "icon", true) + "tmp_" + FN + ".gif";
                    Image1 = AppLogic.GetImagePath("Product", "icon", true) + FN + ".gif";
                    Image1File.SaveAs(TempImage1);
                    AppLogic.ResizeEntityOrObject("Product", TempImage1, Image1, "icon", "image/gif");
                    AppLogic.DisposeOfTempImage(TempImage1);

                    break;
                case "image/x-png":
                case "image/png":
                    TempImage1 = AppLogic.GetImagePath("Product", "icon", true) + "tmp_" + FN + ".png";
                    Image1 = AppLogic.GetImagePath("Product", "icon", true) + FN + ".png";
                    Image1File.SaveAs(TempImage1);
                    AppLogic.ResizeEntityOrObject("Product", TempImage1, Image1, "icon", "image/png");

                    AppLogic.DisposeOfTempImage(TempImage1);
                    break;
                case "image/jpg":
                case "image/jpeg":
                case "image/pjpeg":
                    TempImage1 = AppLogic.GetImagePath("Product", "icon", true) + "tmp_" + FN + ".jpg";
                    Image1 = AppLogic.GetImagePath("Product", "icon", true) + FN + ".jpg";
                    Image1File.SaveAs(TempImage1);
                    AppLogic.ResizeEntityOrObject("Product", TempImage1, Image1, "icon", "image/jpeg");
                    AppLogic.DisposeOfTempImage(TempImage1);

                    break;
            }
        }

        String Image2 = String.Empty;
        String TempImage2 = String.Empty;
        HttpPostedFile Image2File = fuMedium.PostedFile;
        if (Image2File != null && Image2File.ContentLength != 0)
        {
            // delete any current image file first
            try
            {
                foreach (String ss in CommonLogic.SupportedImageTypes)
                {
                    System.IO.File.Delete(AppLogic.GetImagePath("Product", "medium", true) + FN + ss);
                }
            }
            catch
            { }

            String s = Image2File.ContentType;
            switch (Image2File.ContentType)
            {
                case "image/gif":

                    TempImage2 = AppLogic.GetImagePath("Product", "medium", true) + "tmp_" + FN + ".gif";
                    Image2 = AppLogic.GetImagePath("Product", "medium", true) + FN + ".gif";
                    Image2File.SaveAs(TempImage2);
                    AppLogic.ResizeEntityOrObject("Product", TempImage2, Image2, "medium", "image/gif");
                    AppLogic.DisposeOfTempImage(TempImage2);

                    break;
                case "image/x-png":
                case "image/png":

                    TempImage2 = AppLogic.GetImagePath("Product", "medium", true) + "tmp_" + FN + ".png";
                    Image2 = AppLogic.GetImagePath("Product", "medium", true) + FN + ".png";
                    Image2File.SaveAs(TempImage2);
                    AppLogic.ResizeEntityOrObject("Product", TempImage2, Image2, "medium", "image/png");
                    AppLogic.DisposeOfTempImage(TempImage2);

                    break;
                case "image/jpg":
                case "image/jpeg":
                case "image/pjpeg":

                    TempImage2 = AppLogic.GetImagePath("Product", "medium", true) + "tmp_" + FN + ".jpg";
                    Image2 = AppLogic.GetImagePath("Product", "medium", true) + FN + ".jpg";
                    Image2File.SaveAs(TempImage2);
                    AppLogic.ResizeEntityOrObject("Product", TempImage2, Image2, "medium", "image/jpeg");
                    AppLogic.DisposeOfTempImage(TempImage2);

                    break;
            }
        }

        String Image3 = String.Empty;
        String TempImage3 = String.Empty;
        HttpPostedFile Image3File = fuLarge.PostedFile;
        if (Image3File != null && Image3File.ContentLength != 0)
        {
            // delete any current image file first
            try
            {
                foreach (String ss in CommonLogic.SupportedImageTypes)
                {
                    System.IO.File.Delete(AppLogic.GetImagePath("Product", "large", true) + FN + ss);
                }
            }
            catch
            { }

            String s = Image3File.ContentType;
            switch (Image3File.ContentType)
            {
                case "image/gif":

                    TempImage3 = AppLogic.GetImagePath("Product", "large", true) + "tmp_" + FN + ".gif";
                    Image3 = AppLogic.GetImagePath("Product", "large", true) + FN + ".gif";
                    Image3File.SaveAs(TempImage3);
                    AppLogic.ResizeEntityOrObject("Product", TempImage3, Image3, "large", "image/gif");
                    AppLogic.CreateOthersFromLarge("Product", TempImage3, FN, "image/gif");

                    break;
                case "image/x-png":
                case "image/png":

                    TempImage3 = AppLogic.GetImagePath("Product", "large", true) + "tmp_" + FN + ".png";
                    Image3 = AppLogic.GetImagePath("Product", "large", true) + FN + ".png";
                    Image3File.SaveAs(TempImage3);
                    AppLogic.ResizeEntityOrObject("Product", TempImage3, Image3, "large", "image/png");
                    AppLogic.CreateOthersFromLarge("Product", TempImage3, FN, "image/png");

                    break;
                case "image/jpg":
                case "image/jpeg":
                case "image/pjpeg":

                    TempImage3 = AppLogic.GetImagePath("Product", "large", true) + "tmp_" + FN + ".jpg";
                    Image3 = AppLogic.GetImagePath("Product", "large", true) + FN + ".jpg";
                    Image3File.SaveAs(TempImage3);
                    AppLogic.ResizeEntityOrObject("Product", TempImage3, Image3, "large", "image/jpeg");
                    AppLogic.CreateOthersFromLarge("Product", TempImage3, FN, "image/jpeg");

                    break;
            }
            AppLogic.DisposeOfTempImage(TempImage3);
        }

        // color swatch image
        String Image4 = String.Empty;
        String TempImage4 = String.Empty;
        HttpPostedFile Image4File = fuSwatch.PostedFile;
        if (Image4File.ContentLength != 0)
        {
            // delete any current image file first
            try
            {
                foreach (String ss in CommonLogic.SupportedImageTypes)
                {
                    System.IO.File.Delete(AppLogic.GetImagePath("Product", "swatch", true) + FN + ss);
                }
            }
            catch
            { }

            String s = Image4File.ContentType;
            switch (Image4File.ContentType)
            {
                case "image/gif":

                    TempImage4 = AppLogic.GetImagePath("Product", "swatch", true) + "tmp_" + FN + ".gif";
                    Image4 = AppLogic.GetImagePath("Product", "swatch", true) + FN + ".gif";
                    Image4File.SaveAs(TempImage4);
                    AppLogic.ResizeEntityOrObject("Product", TempImage4, Image4, "swatch", "image/gif");
                    AppLogic.DisposeOfTempImage(TempImage4);

                    break;
                case "image/x-png":
                case "image/png":

                    TempImage4 = AppLogic.GetImagePath("Product", "swatch", true) + "tmp_" + FN + ".png";
                    Image4 = AppLogic.GetImagePath("Product", "swatch", true) + FN + ".png";
                    Image4File.SaveAs(TempImage4);
                    AppLogic.ResizeEntityOrObject("Product", TempImage4, Image4, "swatch", "image/png");
                    AppLogic.DisposeOfTempImage(TempImage4);

                    break;
                case "image/jpg":
                case "image/jpeg":
                case "image/pjpeg":

                    TempImage4 = AppLogic.GetImagePath("Product", "swatch", true) + "tmp_" + FN + ".jpg";
                    Image4 = AppLogic.GetImagePath("Product", "swatch", true) + FN + ".jpg";
                    Image4File.SaveAs(TempImage4);
                    AppLogic.ResizeEntityOrObject("Product", TempImage4, Image4, "swatch", "image/jpeg");
                    AppLogic.DisposeOfTempImage(TempImage4);

                    break;
            }
        }
    }

    protected void SetVariantButtons()
    {
        string temp = ("<a target=\"entityBody\" href=\"entityProductVariantsOverview.aspx?ProductID=" + pID + "&entityname=" + eSpecs.m_EntityName + "&EntityID=" + eID.ToString() + "\">Show/Edit/Add Variants</a>");

        ltVariantsLinks.Text = temp;
    }

    protected void btnDescription_Click(object sender, EventArgs e)
    {
        System.IO.File.Delete(pdesc.FN);
        LoadContent();
    }

    protected void btnSpecs_Click(object sender, EventArgs e)
    {
        ProductSpecFile pspec = new ProductSpecFile(pID, skinID);
        System.IO.File.Delete(pspec.FN);
        LoadContent();
    }

    protected void btnDeleteAll_Click(object sender, EventArgs e)
    {
        DB.ExecuteSQL("delete from CustomCart where VariantID in (select VariantID from ProductVariant where ProductID=" + pID.ToString() + ")");
        DB.ExecuteSQL("delete from KitCart where VariantID in (select VariantID from ProductVariant where ProductID=" + pID.ToString() + ")");
        DB.ExecuteSQL("delete from ShoppingCart where VariantID in (select VariantID from ProductVariant where ProductID=" + pID.ToString() + ")");
        DB.ExecuteSQL("delete from ProductVariant where ProductID=" + pID.ToString());

        LoadContent();
        resetError("All variants have been deleted", false);
    }

    protected void btnDeleteProduct_Click(object sender, EventArgs e)
    {
        DB.ExecuteSQL("update dbo.Product set Deleted = case deleted when 1 then 0 else 1 end where ProductID = " + pID.ToString());
        LoadContent();
    }

}