// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/entityProducts.aspx.cs 11    9/18/06 2:39p Jan Simacek $
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

public partial class entityProducts : System.Web.UI.Page
{
    private Customer ThisCustomer;
    private EntityHelper entity;
    private string eName;
    private int eID;
    private EntitySpecs eSpecs;
    private int SiteID = 1;
    protected string selectSQL = "select * from Product";

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");

        ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

        eID = CommonLogic.QueryStringNativeInt("EntityFilterID");
        eName = CommonLogic.QueryStringCanBeDangerousContent("EntityName");
        eSpecs = EntityDefinitions.LookupSpecs(eName);

        switch (eName.ToUpperInvariant())
        {
            case "SECTION":
                ltPreEntity.Text = "Section's";
                entity = new EntityHelper(EntityDefinitions.readonly_SectionEntitySpecs);
                break;
            case "MANUFACTURER":
                ltPreEntity.Text = "Manufacturer's";
                entity = new EntityHelper(EntityDefinitions.readonly_ManufacturerEntitySpecs);
                break;
            case "DISTRIBUTOR":
                ltPreEntity.Text = "Distributor's";
                entity = new EntityHelper(EntityDefinitions.readonly_DistributorEntitySpecs);
                break;
            case "GENRE":
                ltPreEntity.Text = "Genre's";
                entity = new EntityHelper(EntityDefinitions.readonly_GenreEntitySpecs);
                break;
            case "VECTOR":
                ltPreEntity.Text = "Vector's";
                entity = new EntityHelper(EntityDefinitions.readonly_VectorEntitySpecs);
                break;
            case "LIBRARY":
                ltPreEntity.Text = "Library's";
                entity = new EntityHelper(EntityDefinitions.readonly_LibraryEntitySpecs);
                break;
            default:
                ltPreEntity.Text = "Category's";
                entity = new EntityHelper(EntityDefinitions.readonly_CategoryEntitySpecs);
                break;
        }

        if (!IsPostBack)
        {
            ltEntity.Text = entity.GetEntityBreadcrumb6(eID, ThisCustomer.LocaleSetting);

            string query = CommonLogic.QueryStringCanBeDangerousContent("searchfor");

            loadTree();
            loadTypes();

            ViewState["SQLString"] = this.selectSQL;

            //set page settings
            if (ThisCustomer.ThisCustomerSession.Session("ProductsSort").Length == 0)
            {
                ViewState["Sort"] = "Name";
            }
            else
            {
                ViewState["Sort"] = ThisCustomer.ThisCustomerSession.Session("ProductsSort");
            }
            if (ThisCustomer.ThisCustomerSession.Session("ProductsOrder").Length == 0)
            {
                ViewState["SortOrder"] = "ASC";
            }
            else
            {
                ViewState["SortOrder"] = ThisCustomer.ThisCustomerSession.Session("ProductsOrder");
            }
            if (ThisCustomer.ThisCustomerSession.Session("ProductsSearch").Length != 0)
            {
                query = ThisCustomer.ThisCustomerSession.Session("ProductsSearch");
            }
            if (ThisCustomer.ThisCustomerSession.Session("ProductsTree").Length != 0)
            {
                treeMain.FindNode(ThisCustomer.ThisCustomerSession.Session("ProductsTree")).Selected = true;
            }
            if (ThisCustomer.ThisCustomerSession.Session("ProductsType").Length != 0)
            {
                ddTypes.Items.FindByValue(ThisCustomer.ThisCustomerSession.Session("ProductsType")).Selected = true;
            }

            this.resultFilter(query);

            this.txtSearch.Attributes.Add("onKeyPress", "javascript:if (event.keyCode == 13) __doPostBack('btnSearch','')");
        }
    }

    private void loadTree()
    {
        try
        {
            string index = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            treeMain.Nodes.Clear();
            treeMain.Nodes.Add(new TreeNode("All", "All", "icons/dot.gif"));

            foreach (char c in index)
            {
                TreeNode myNode = new TreeNode();
                myNode.Text = c.ToString();
                myNode.ImageUrl = "icons/dot.gif";
                treeMain.Nodes.Add(myNode);
            }
        }
        catch (Exception ex)
        {
            this.resetError(ex.ToString(), true);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        ViewState["IsInsert"] = false;
        gMain.EditIndex = -1;
        this.resetError("", false);
        this.gMain.PageIndex = 0;

        this.resultFilter("");
    }

    private void loadTypes()
    {
        try
        {
            ddTypes.Items.Clear();
            ddTypes.Items.Add(new ListItem("All Product Types", "0"));
            DataSet ds = DB.GetDS("select * from ProductType " + DB.GetNoLock() + " order by DisplayOrder,Name", AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                ListItem myNode = new ListItem();
                myNode.Value = DB.RowField(row, "ProductTypeID");
                myNode.Text = DB.RowField(row, "Name");
                ddTypes.Items.Add(myNode);
            }
            ds.Dispose();
        }
        catch (Exception ex)
        {
            this.resetError(ex.ToString(), true);
        }
    }

    private int _entityId = 0;
    private string _entityName = string.Empty;

    protected void resultFilter(string SearchFor)
    {
        int CategoryFilterID = CommonLogic.QueryStringUSInt("CategoryFilterID");
        int SectionFilterID = CommonLogic.QueryStringUSInt("SectionFilterID");
        int ProductTypeFilterID = CommonLogic.QueryStringUSInt("ProductTypeFilterID");
        int ManufacturerFilterID = CommonLogic.QueryStringUSInt("ManufacturerFilterID");
        int DistributorFilterID = CommonLogic.QueryStringUSInt("DistributorFilterID");
        int GenreFilterID = CommonLogic.QueryStringUSInt("GenreFilterID");
        int VectorFilterID = CommonLogic.QueryStringUSInt("VectorFilterID");
        int AffiliateFilterID = CommonLogic.QueryStringUSInt("AffiliateFilterID");
        int CustomerLevelFilterID = CommonLogic.QueryStringUSInt("CustomerLevelFilterID");

        String ENCleaned = CommonLogic.QueryStringCanBeDangerousContent("EntityName").Trim().ToUpperInvariant();

        // kludge for now, during conversion to properly entity/object setup:
        if (ENCleaned == "CATEGORY")
        {
            CategoryFilterID = CommonLogic.QueryStringUSInt("EntityFilterID");
            SectionFilterID = 0;
            ProductTypeFilterID = Localization.ParseNativeInt(ddTypes.SelectedValue);
            ManufacturerFilterID = 0;
            DistributorFilterID = 0;
            GenreFilterID = 0;
            VectorFilterID = 0;
            AffiliateFilterID = 0;
            CustomerLevelFilterID = 0;

            /* Added by : Mark
             * No : 108
             * Date : 11.16.2006 */             
            _entityId = CategoryFilterID;
            _entityName = ENCleaned;
        }
        if (ENCleaned == "SECTION")
        {
            CategoryFilterID = 0;
            SectionFilterID = CommonLogic.QueryStringUSInt("EntityFilterID");
            ProductTypeFilterID = Localization.ParseNativeInt(ddTypes.SelectedValue);
            ManufacturerFilterID = 0;
            DistributorFilterID = 0;
            GenreFilterID = 0;
            VectorFilterID = 0;
            AffiliateFilterID = 0;
            CustomerLevelFilterID = 0;

            /* Added by : Mark
             * No : 108
             * Date : 11.16.2006 */
            _entityId = SectionFilterID;
            _entityName = ENCleaned;
        }
        if (ENCleaned == "MANUFACTURER")
        {
            CategoryFilterID = 0;
            SectionFilterID = 0;
            ProductTypeFilterID = Localization.ParseNativeInt(ddTypes.SelectedValue);
            ManufacturerFilterID = CommonLogic.QueryStringUSInt("EntityFilterID");
            DistributorFilterID = 0;
            GenreFilterID = 0;
            VectorFilterID = 0;
            AffiliateFilterID = 0;
            CustomerLevelFilterID = 0;

            /* Added by : Mark
             * No : 108
             * Date : 11.16.2006 */
            _entityId = ManufacturerFilterID;
            _entityName = ENCleaned;
        }
        if (ENCleaned == "DISTRIBUTOR")
        {
            CategoryFilterID = 0;
            SectionFilterID = 0;
            ProductTypeFilterID = Localization.ParseNativeInt(ddTypes.SelectedValue);
            ManufacturerFilterID = 0;
            DistributorFilterID = CommonLogic.QueryStringUSInt("EntityFilterID");
            GenreFilterID = 0;
            VectorFilterID = 0;
            AffiliateFilterID = 0;
            CustomerLevelFilterID = 0;

            /* Added by : Mark
             * No : 108
             * Date : 11.16.2006 */
            _entityId = DistributorFilterID;
            _entityName = ENCleaned;
        }
        if (ENCleaned == "GENRE")
        {
            CategoryFilterID = 0;
            SectionFilterID = 0;
            ProductTypeFilterID = Localization.ParseNativeInt(ddTypes.SelectedValue);
            ManufacturerFilterID = 0;
            DistributorFilterID = 0;
            GenreFilterID = CommonLogic.QueryStringUSInt("EntityFilterID");
            VectorFilterID = 0;
            AffiliateFilterID = 0;
            CustomerLevelFilterID = 0;

            /* Added by : Mark
             * No : 108
             * Date : 11.16.2006 */
            _entityId = GenreFilterID;
            _entityName = ENCleaned;
        }
        if (ENCleaned == "DISTRIBUTOR")
        {
            CategoryFilterID = 0;
            SectionFilterID = 0;
            ProductTypeFilterID = Localization.ParseNativeInt(ddTypes.SelectedValue);
            ManufacturerFilterID = 0;
            DistributorFilterID = 0;
            GenreFilterID = 0;
            VectorFilterID = CommonLogic.QueryStringUSInt("EntityFilterID");
            AffiliateFilterID = 0;
            CustomerLevelFilterID = 0;

            /* Added by : Mark
             * No : 108
             * Date : 11.16.2006 */
            _entityId = VectorFilterID;
            _entityName = ENCleaned;
        }
        if (ENCleaned == "AFFILIATE")
        {
            CategoryFilterID = 0;
            SectionFilterID = 0;
            ProductTypeFilterID = Localization.ParseNativeInt(ddTypes.SelectedValue);
            ManufacturerFilterID = 0;
            DistributorFilterID = 0;
            GenreFilterID = 0;
            VectorFilterID = 0;
            AffiliateFilterID = CommonLogic.QueryStringUSInt("EntityFilterID");
            CustomerLevelFilterID = 0;

            /* Added by : Mark
             * No : 108
             * Date : 11.16.2006 */
            _entityId = AffiliateFilterID;
            _entityName = ENCleaned;
        }
        if (ENCleaned == "CUSTOMERLEVEL")
        {
            CategoryFilterID = 0;
            SectionFilterID = 0;
            ProductTypeFilterID = Localization.ParseNativeInt(ddTypes.SelectedValue);
            ManufacturerFilterID = 0;
            DistributorFilterID = 0;
            GenreFilterID = 0;
            VectorFilterID = 0;
            AffiliateFilterID = 0;
            CustomerLevelFilterID = CommonLogic.QueryStringUSInt("EntityFilterID");

            /* Added by : Mark
             * No : 108
             * Date : 11.16.2006 */
            _entityId = CustomerLevelFilterID;
            _entityName = ENCleaned;
        }
        // end kludge

        //search
        if (SearchFor.Length == 0)
        {
            SearchFor = this.txtSearch.Text;
            ThisCustomer.ThisCustomerSession.SetVal("ProductsSearch", this.txtSearch.Text);
        }

        //Node filter
        string Index = "";
        for (int i = 0; i < this.treeMain.Nodes.Count; i++)
        {
            if (this.treeMain.Nodes[i].Selected)
            {
                Index = treeMain.Nodes[i].Value;

                ThisCustomer.ThisCustomerSession.SetVal("ProductsTree", treeMain.Nodes[i].Value);

                break;
            }
        }


        //Type filter
        string typepName = ddTypes.SelectedValue;
        ThisCustomer.ThisCustomerSession.SetVal("ProductsType", typepName);


        ThisCustomer.ThisCustomerSession.SetVal("ProductsSort", ViewState["Sort"].ToString());
        ThisCustomer.ThisCustomerSession.SetVal("ProductsOrder", ViewState["SortOrder"].ToString());

        //remember page
        if (ThisCustomer.ThisCustomerSession.SessionNativeInt("ProductsPage") > 0)
        {
            gMain.PageIndex = ThisCustomer.ThisCustomerSession.SessionNativeInt("ProductsPage");
        }

        ProductCollection products = new ProductCollection();
        products.CategoryID = CategoryFilterID;
        products.SectionID = SectionFilterID;
        products.ManufacturerID = ManufacturerFilterID;
        products.DistributorID = DistributorFilterID;
        products.GenreID = GenreFilterID;
        products.VectorID = VectorFilterID;
        products.AffiliateID = AffiliateFilterID;
        products.ProductTypeID = ProductTypeFilterID;
        products.PublishedOnly = false;
        products.SearchDescriptionAndSummaryFields = false;
        products.SearchMatch = SearchFor;

        if (!Index.Equals("All"))
        {
            products.SearchIndex = Index;
        }

        products.SortBy = ViewState["Sort"].ToString();
        products.SortOrder = ViewState["SortOrder"].ToString();

        DataSet dsProducts = products.LoadFromDBEntity();

        //build grid
        this.buildGridData(dsProducts);

        ((TextBox)this.Form.FindControl("txtSearch")).Text = SearchFor;
    }

    protected void treeMain_SelectedNodeChanged(object sender, EventArgs e)
    {
        ViewState["IsInsert"] = false;
        gMain.EditIndex = -1;
        this.resetError("", false);
        this.gMain.PageIndex = 0;

        this.resultFilter("");
    }

    protected void ddTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
        ViewState["IsInsert"] = false;
        gMain.EditIndex = -1;
        this.resetError("", false);
        gMain.PageIndex = 0;

        this.resultFilter("");
    }

    protected void buildGridData(DataSet ds)
    {
        gMain.DataSource = ds;
        gMain.DataBind();

        try
        {
            for (int i = 0; i < gMain.HeaderRow.Cells.Count; i++)
            {
                if (gMain.Columns[i].SortExpression.Equals(ViewState["Sort"].ToString()))
                {
                    Image arrow = new Image();
                    if (ViewState["SortOrder"].ToString().ToLowerInvariant().Equals("asc"))
                    {
                        arrow.ImageUrl = "icons/asc.gif";
                    }
                    else
                    {
                        arrow.ImageUrl = "icons/desc.gif";
                    }
                    gMain.HeaderRow.Cells[i].Controls.Add(arrow);
                }
            }
        }
        catch { }

        ds.Dispose();
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

        ((Literal)this.Form.FindControl("ltError")).Text = str;
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect("entityEditProducts.aspx?iden=0&entityName=" + eName + "&entityFilterID=" + eID);
    }

    protected void gMain_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView myrow = (DataRowView)e.Row.DataItem;

            //set delete confirms
            ImageButton iD = (ImageButton)e.Row.FindControl("imgDelete");
            iD.Attributes.Add("onClick", "javascript: return confirm('Are you sure you want to soft-delete product: " + myrow["ProductID"].ToString() + "? This will also remove all items from customer shopping carts, wish lists, and gift registries IF they reference this product! The product will remain in the db, with a delete flag set, so it is not visible on the admin or store sites, but you could recover the product record later if you have to.')");
            ImageButton iN = (ImageButton)e.Row.FindControl("imgNuke");
            iN.Attributes.Add("onClick", "javascript: return confirm('Are you sure you want to nuke product: " + myrow["ProductID"].ToString() + "? This will also remove all items from customer shopping carts, wish lists, and gift registries IF they reference this product! This action cannot be undone! The product, and it\\'s variants, are completely removed from the database.')");
            LinkButton lCc = (LinkButton)e.Row.FindControl("lnkClone");
            lCc.Attributes.Add("onClick", "javascript: return confirm('Are you sure you want to create a clone of this product, and all variants?')");

            if (Localization.ParseBoolean(myrow["IsSystem"].ToString()))
            {
                e.Row.Cells[8].Text = "System Product"; // this type of product can only be deleted in the db!
                iD.Visible = false;
                e.Row.Cells[9].Text = "System Product"; // this type of product can only be deleted in the db!
                iN.Visible = false;
            }

            Literal ltName = (Literal)e.Row.FindControl("ltName");
            ltName.Text = ("<a href=\"entityEditProducts.aspx?iden=" + myrow["ProductID"].ToString() + "&entityName=" + eName + "&entityFilterID=" + eID + "\">");
            ltName.Text += (XmlCommon.GetLocaleEntry(myrow["Name"].ToString(), ThisCustomer.LocaleSetting, true));
            ltName.Text += ("</a>");

            Literal ltImage = (Literal)e.Row.FindControl("ltImage");
            String Image1URL = AppLogic.LookupImage("Product", Localization.ParseNativeInt(myrow["ProductID"].ToString()), "icon", SiteID, ThisCustomer.LocaleSetting);
            ltImage.Text = "<img src=\"" + Image1URL + "\" width=\"25\" border=\"0\" align=\"absmiddle\">";

            //Variants
            ltName.Text += ("&nbsp;(<a href=\"entityProductVariantsOverview.aspx?ProductID=" + myrow["ProductID"].ToString() + "&entityname=" + eName + "&EntityID=" + eID + "\">Variants</a>)</div>");

            Literal ltInventory = (Literal)e.Row.FindControl("ltInventory");
            if (Localization.ParseBoolean(myrow["IsSystem"].ToString()))
            {
                ltInventory.Text = "System Product"; // this type of product can only be deleted in the db!
            }
            else
            {
                if (AppLogic.ProductTracksInventoryBySizeAndColor(Localization.ParseNativeInt(myrow["ProductID"].ToString())))
                {
                    ltInventory.Text = ("<a href=\"entityEditInventory.aspx?productid=" + myrow["ProductID"].ToString() + "\">Inventory</a>\n");
                }
                else
                {
                    ltInventory.Text = (myrow["Inventory"].ToString());
                }
            }

            LinkButton lC = (LinkButton)e.Row.FindControl("lnkClone");
            if (Localization.ParseBoolean(myrow["IsSystem"].ToString()))
            {
                e.Row.Cells[7].Text = "System Product"; // this type of product can only be deleted in the db!
                lC.Visible = false;
            }

            Literal ltRating = (Literal)e.Row.FindControl("ltRating");
            if (Localization.ParseBoolean(myrow["IsSystem"].ToString()))
            {
                ltRating.Text = ("System Product"); // this type of product can only be deleted in the db!
            }
            else
            {
                int NumRatings = DB.GetSqlN("select count(*) as N from rating  " + DB.GetNoLock() + " where productid=" + myrow["ProductID"].ToString());
                ltRating.Text = ("<a href=\"javascript:;\" onclick=\"window.open('entityProductRatings.aspx?Productid=" + myrow["ProductID"].ToString() + "','Rating','height=325, width=500, resizable=no, scrollbars=yes, toolbar=no, status=yes, location=no, directories=no, menubar=yes, alwaysRaised=yes');\">Ratings (" + NumRatings.ToString() + ")</a>\n");
            }
        }
    }

    protected void gMain_Sorting(object sender, GridViewSortEventArgs e)
    {
        gMain.EditIndex = -1;
        ViewState["Sort"] = e.SortExpression.ToString();
        ViewState["SortOrder"] = (ViewState["SortOrder"].ToString() == "ASC" ? "DESC" : "ASC");
        this.resultFilter("");
    }
    protected void gMain_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        this.resetError("", false);

        if (e.CommandName == "DeleteItem")
        {
            gMain.EditIndex = -1;
            int iden = Localization.ParseNativeInt(e.CommandArgument.ToString());
            this.deleteRow(iden);
        }
        else if (e.CommandName == "NukeItem")
        {
            gMain.EditIndex = -1;
            int iden = Localization.ParseNativeInt(e.CommandArgument.ToString());
            this.nukeRow(iden);
        }
        else if (e.CommandName == "CloneItem")
        {
            gMain.EditIndex = -1;
            int iden = Localization.ParseNativeInt(e.CommandArgument.ToString());
            this.cloneRow(iden);
        }

        this.resultFilter("");
    }

    protected void cloneRow(int iden)
    {
        if (iden != 0)
        {
            int NewProductID = 0;
            IDataReader rs = DB.GetRS("aspdnsf_CloneProduct " + iden.ToString());
            if (rs.Read())
            {
                NewProductID = DB.RSFieldInt(rs, "ProductID");
            }
            rs.Close();
            rs.Dispose();
            if (NewProductID != 0)
            {
                //this.buildGridData(this.buildGridData());
                this.resultFilter("");
                resetError("Product Cloned.", false);
            }
        }
    }

    private const string ENTITY_FRAME_MENU = "entityMenu";

    private void ForceReloadMenuList()
    {
        if (!string.IsNullOrEmpty(_entityName) && 
            !(_entityId == 0))
        {
            StringBuilder attachReloadScript = new StringBuilder();
            attachReloadScript.Append("<script language=\"Javascript\">");
            attachReloadScript.AppendFormat(
                "window.onload = function(){{ if(parent.{0}){{parent.{0}.document.location.href='entityMenu.aspx?entityName={1}';}} }};", 
                ENTITY_FRAME_MENU,
                _entityName
            );
            attachReloadScript.Append("</script>");

            Page.ClientScript.RegisterClientScriptBlock(
                this.GetType(), 
                Guid.NewGuid().ToString(), 
                attachReloadScript.ToString()
            );
        }
    }

    protected void deleteRow(int iden)
    {
        DB.ExecuteSQL("delete from ShoppingCart where productid=" + iden.ToString());
        DB.ExecuteSQL("delete from kitcart where productid=" + iden.ToString());
        DB.ExecuteSQL("delete from customcart where productid=" + iden.ToString());
        DB.ExecuteSQL("update Product set deleted=1 where ProductID=" + iden.ToString());

        /* Modified by : mark
         * Date : 11.16.2006
         * No : 108
         * Remove all Entity mappings for this product
         */
        DB.ExecuteSQL("delete from ProductAffiliate where productid=" + iden.ToString());
        DB.ExecuteSQL("delete from ProductCategory where productid=" + iden.ToString());
        DB.ExecuteSQL("delete from ProductCustomerLevel where productid=" + iden.ToString());
        DB.ExecuteSQL("delete from ProductDistributor where productid=" + iden.ToString());
        DB.ExecuteSQL("delete from ProductGenre where productid=" + iden.ToString());
        DB.ExecuteSQL("delete from ProductVector where productid=" + iden.ToString());
        DB.ExecuteSQL("delete from ProductLocaleSetting where productid=" + iden.ToString());
        DB.ExecuteSQL("delete from ProductManufacturer where productid=" + iden.ToString());
        DB.ExecuteSQL("delete from ProductSection where productid=" + iden.ToString());
        
        /******* end modification ****************/

        string treeT = treeMain.SelectedValue;
        string typeT = ddTypes.SelectedValue;
        this.loadTree();
        this.loadTypes();

        //take user back
        foreach (ListItem li in ddTypes.Items)
        {
            if (li.Value.ToUpperInvariant().Equals(typeT.ToUpperInvariant()))
            {
                ddTypes.ClearSelection();
                li.Selected = true;
            }
        }
        foreach (TreeNode node in treeMain.Nodes)
        {
            if (node.Value.Equals(treeT))
            {
                node.Selected = true;
            }
        }

        //this.buildGridData(this.buildGridData());
        this.resultFilter("");

        /******* start ****************/
        ForceReloadMenuList();
        /******* end modification ****************/

        resetError("Product deleted.", false);
    }
    protected void nukeRow(int iden)
    {
        DB.ExecuteLongTimeSQL("aspdnsf_NukeProduct " + iden.ToString(), 120);

        string treeT = treeMain.SelectedValue;
        this.loadTree();
        foreach (TreeNode node in treeMain.Nodes)
        {
            if (node.Value.Equals(treeT))
            {
                node.Selected = true;
            }
        }

        //this.buildGridData(this.buildGridData());
        this.resultFilter("");

        resetError("Product Nuked.", false);
    }

    protected void gMain_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.resetError("", false);
        gMain.PageIndex = e.NewPageIndex;
        gMain.EditIndex = -1;
        //this.buildGridData(this.buildGridData());
        ThisCustomer.ThisCustomerSession.SetVal("ProductsPage", e.NewPageIndex.ToString());
        this.resultFilter("");
    }
}
