// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/entityProductVariantsOverview.aspx.cs 7     9/18/06 2:39p Jan Simacek $
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

public partial class entityProductVariantsOverview : System.Web.UI.Page
{
    private Customer ThisCustomer;
    private EntityHelper entity;
    private string eName;
    private int eID;
    private EntitySpecs eSpecs;
    private int SiteID = 1;
    private int pID;
    private string ProductSKU;
    protected string selectSQL = "select * from ProductVariant";
        
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");

        ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

        eID = CommonLogic.QueryStringNativeInt("EntityID");
        eName = CommonLogic.QueryStringCanBeDangerousContent("EntityName");
        eSpecs = EntityDefinitions.LookupSpecs(eName);
        entity = new EntityHelper(eSpecs);     
        pID = CommonLogic.QueryStringNativeInt("ProductID");

        ProductSKU = AppLogic.GetProductSKU(pID);

        if (!IsPostBack)
        {
            ltEntity.Text = entity.GetEntityBreadcrumb6(eID, ThisCustomer.LocaleSetting);
            ltProduct.Text = "<a href=\"entityEditProducts.aspx?iden=" + pID + "&entityName=" + eName + "&entityFilterID=" + eID + "\">" + AppLogic.GetProductName(pID, ThisCustomer.LocaleSetting) + " ("+pID+")</a>";

            string query = CommonLogic.QueryStringCanBeDangerousContent("searchfor");

            //loadTree();

            ViewState["SQLString"] = this.selectSQL;

            //set page settings
            if (ThisCustomer.ThisCustomerSession.Session("productVariantsSort").Length == 0)
            {
                ViewState["Sort"] = "DisplayOrder, Name";
            }
            else
            {
                ViewState["Sort"] = ThisCustomer.ThisCustomerSession.Session("productVariantsSort");
            }
            if (ThisCustomer.ThisCustomerSession.Session("productVariantsOrder").Length == 0)
            {
                ViewState["SortOrder"] = "ASC";
            }
            else
            {
                ViewState["SortOrder"] = ThisCustomer.ThisCustomerSession.Session("productVariantsOrder"); 
            }
            if (ThisCustomer.ThisCustomerSession.Session("productVariantsSearch").Length != 0)
            {
                query = ThisCustomer.ThisCustomerSession.Session("productVariantsSearch");
            }
            //if (CommonLogic.Session("productVariantsTree").Length != 0)
            //{
            //    treeMain.FindNode(CommonLogic.Session("productVariantsTree")).Selected = true;
            //}

            resultFilter(query);
            
            //txtSearch.Attributes.Add("onKeyPress", "javascript:if (event.keyCode == 13) __doPostBack('btnSearch','')");
            btnDeleteVariants.Attributes.Add("onclick", "return confirm('Are you sure you want to delete ALL variants for this product? This will also remove all items from customer shopping carts, wish lists, and gift registries IF they reference any of these variants!');");

            AppLogic.MakeSureProductHasAtLeastOneVariant(pID);
        }

        AppLogic.EnsureProductHasADefaultVariantSet(pID);
    }

    //private void loadTree()
    //{
    //    try
    //    {
    //        string index = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    //        treeMain.Nodes.Clear();
    //        treeMain.Nodes.Add(new TreeNode("All", "All", "icons/dot.gif"));

    //        foreach (char c in index)
    //        {
    //            TreeNode myNode = new TreeNode();
    //            myNode.Text = c.ToString();
    //            myNode.ImageUrl = "icons/dot.gif";
    //            treeMain.Nodes.Add(myNode);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        this.resetError(ex.ToString(), true);
    //    }
    //}

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        ViewState["IsInsert"] = false;
        gMain.EditIndex = -1;
        this.resetError("", false);
        this.gMain.PageIndex = 0;

        this.resultFilter("");
    }

    protected void resultFilter(string SearchFor)
    {
        String sql = selectSQL + DB.GetNoLock() + " ";
        String WhereClause = " Deleted=0 AND ProductID=" + pID + " ";

        //search
        if (SearchFor.Length == 0)
        {
            //SearchFor = this.txtSearch.Text;
            //Session.Add("productVariantsSearch", this.txtSearch.Text);
        }

        if (SearchFor.Length != 0)
        {
            if (WhereClause.Length != 0)
            {
                WhereClause += " and ";
            }
            WhereClause += " (Name like " + DB.SQuote("%" + SearchFor + "%") + " or Description like " + DB.SQuote("%" + SearchFor + "%") + " or SKUSuffix like " + DB.SQuote("%" + SearchFor + "%") + ")";
        }

        ////Node filter
        //string Index = "";
        //for (int i = 0; i < this.treeMain.Nodes.Count; i++)
        //{
        //    if (this.treeMain.Nodes[i].Selected)
        //    {
        //        Index = treeMain.Nodes[i].Value;

        //        Session.Add("productVariantsTree", treeMain.Nodes[i].Value);

        //        break;
        //    }
        //}

        if (WhereClause.Length != 0)
        {
            sql += " where " + WhereClause;
        }

        //set states
        ViewState["SQLString"] = sql.ToString();
        sql += " order by " + ViewState["Sort"].ToString() + " " + ViewState["SortOrder"].ToString();

        ThisCustomer.ThisCustomerSession.SetVal("productVariantsSort", ViewState["Sort"].ToString());
        ThisCustomer.ThisCustomerSession.SetVal("productVariantsOrder", ViewState["SortOrder"].ToString());

        //build grid
        this.buildGridData(DB.GetDS(sql, false));

        //remember page
        if (ThisCustomer.ThisCustomerSession.SessionNativeInt("productVariantsPage") > 0)
        {
            gMain.PageIndex = ThisCustomer.ThisCustomerSession.SessionNativeInt("productVariantsPage");
        }

        //txtSearch.Text = SearchFor;
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

    //protected DataSet buildGridData()
    //{
    //    string sql = ViewState["SQLString"].ToString();
    //    sql += " order by " + ViewState["Sort"].ToString() + " " + ViewState["SortOrder"].ToString();

    //    return DB.GetDS(sql, false);
    //}

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
        Response.Redirect("entityEditProductVariant.aspx?ProductID=" + pID + "&entityname=" + eName + "&EntityID=" + eID + "&Variantid=0");
    }

    protected void gMain_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView myrow = (DataRowView)e.Row.DataItem;

            //set delete and action confirms
            ImageButton iD = (ImageButton)e.Row.FindControl("imgDelete");
            iD.Attributes.Add("onClick", "javascript: return confirm('Are you sure you want to soft-delete product: " + myrow["ProductID"].ToString() + "? This will also remove all items from customer shopping carts, wish lists, and gift registries IF they reference this product! The product will remain in the db, with a delete flag set, so it is not visible on the admin or store sites, but you could recover the product record later if you have to.')");
            LinkButton lCc = (LinkButton)e.Row.FindControl("lnkClone");
            lCc.Attributes.Add("onClick", "javascript: return confirm('Are you sure you want to create a clone of this variant?')");

            //Name and Image
            Literal ltName = (Literal)e.Row.FindControl("ltName");
            ltName.Text += ("<a href=\"entityEditProductVariant.aspx?ProductID=" + pID + "&entityname=" + eName + "&EntityID=" + eID + "&Variantid=" + myrow["VariantID"].ToString() + "\">" + CommonLogic.IIF(XmlCommon.GetLocaleEntry(myrow["Name"].ToString(), ThisCustomer.LocaleSetting, true).Length == 0, "(Unnamed Variant)", XmlCommon.GetLocaleEntry(myrow["Name"].ToString(), ThisCustomer.LocaleSetting, true)) + "</a>");

            Literal ltImage = (Literal)e.Row.FindControl("ltImage");
            String Image1URL = AppLogic.LookupImage("Variant", Localization.ParseNativeInt(myrow["VariantID"].ToString()), "icon", SiteID, ThisCustomer.LocaleSetting);
            ltImage.Text = "<img src=\"" + Image1URL + "\" width=\"25\" border=\"0\" align=\"absmiddle\">";
            

            //SKU
            Literal ltSKU = (Literal)e.Row.FindControl("ltSKU");
            ltSKU.Text = ProductSKU + myrow["SKUSuffix"].ToString();

            //Price
            Literal ltPrice = (Literal)e.Row.FindControl("ltPrice");
            ltPrice.Text = Localization.CurrencyStringForDisplayWithoutExchangeRate(Localization.ParseNativeDecimal(myrow["Price"].ToString()));
            ltPrice.Text += CommonLogic.IIF(Localization.ParseNativeDecimal(myrow["SalePrice"].ToString()) != System.Decimal.Zero, "<br/><span style=\"font-weight: bold; color: red;\">" + Localization.CurrencyStringForDisplayWithoutExchangeRate(Localization.ParseNativeDecimal(myrow["SalePrice"].ToString())) + "</span>", "&nbsp;");

            //Inventory
            Literal ltInventory = (Literal)e.Row.FindControl("ltInventory");
            if (AppLogic.ProductTracksInventoryBySizeAndColor(Localization.ParseNativeInt(myrow["ProductID"].ToString())))
            {
                ltInventory.Text = ("<a href=\"entityEditInventory.aspx?productid=" + myrow["ProductID"].ToString() + "\">Inventory</a>\n");
            }
            else
            {
                ltInventory.Text = (myrow["Inventory"].ToString());
            }

            LinkButton lC = (LinkButton)e.Row.FindControl("lnkClone");
            //if (Localization.ParseBoolean(myrow["IsSystem"].ToString()))
            //{
            //    e.Row.Cells[7].Text = "System Variant"; // this type of variant can only be deleted in the db!
            //    lC.Visible = false;
            //}

            //Display Order
            Literal ltDisplayOrder = (Literal)e.Row.FindControl("ltDisplayOrder");
            ltDisplayOrder.Text = "<input size=2 type=\"text\" name=\"DisplayOrder_" + myrow["VariantID"].ToString() + "\" value=\"" + Localization.ParseNativeInt(myrow["DisplayOrder"].ToString()) + "\">";

            //Default Variant
            Literal ltDefault = (Literal)e.Row.FindControl("ltDefault");
            ltDefault.Text = "<input type=\"radio\" name=\"IsDefault\" value=\"" + myrow["VariantID"].ToString() + "\" " + CommonLogic.IIF(myrow["IsDefault"].ToString() == "1", " checked ", "") + ">";
        }
    }

    protected void gMain_Sorting(object sender, GridViewSortEventArgs e)
    {
        gMain.EditIndex = -1;
        ViewState["Sort"] = e.SortExpression.ToString();
        ViewState["SortOrder"] = (ViewState["SortOrder"].ToString() == "ASC" ? "DESC" : "ASC");
        //this.buildGridData(this.buildGridData());
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
        else if (e.CommandName == "CloneItem")
        {
            gMain.EditIndex = -1;
            int iden = Localization.ParseNativeInt(e.CommandArgument.ToString());
            this.cloneRow(iden);
        }
    }

    protected void cloneRow(int iden)
    {
        if (iden != 0)
        {
            DB.ExecuteSQL("aspdnsf_CloneVariant " + iden.ToString() + "," + ThisCustomer.CustomerID.ToString());
            this.resultFilter("");
            resetError("Variant Cloned.", false);
        }
    }

    protected void deleteRow(int iden)
    {
        DB.ExecuteSQL("delete from CustomCart where VariantID=" + iden.ToString());
        DB.ExecuteSQL("delete from KitCart where VariantID=" + iden.ToString());
        DB.ExecuteSQL("delete from ShoppingCart where VariantID=" + iden.ToString());
        DB.ExecuteSQL("delete from ProductVariant where VariantID=" + iden.ToString());

        //string treeT = treeMain.SelectedValue;
        //this.loadTree();

        ////take user back
        //foreach (TreeNode node in treeMain.Nodes)
        //{
        //    if (node.Value.Equals(treeT))
        //    {
        //        node.Selected = true;
        //    }
        //}

        this.resultFilter("");
        resetError("Product deleted.", false);
    }
    
    protected void gMain_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.resetError("", false);
        gMain.PageIndex = e.NewPageIndex;
        gMain.EditIndex = -1;
        ThisCustomer.ThisCustomerSession.SetVal("ProductsPage", e.NewPageIndex.ToString());
        this.resultFilter("");
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        DB.ExecuteSQL("update ProductVariant set IsDefault=0 where ProductID=" + pID.ToString());
        if (CommonLogic.FormCanBeDangerousContent("IsDefault").Length == 0 || CommonLogic.FormUSInt("IsDefault") == 0)
        {
            // try to force a default variant, none was specified!
            DB.ExecuteSQL("update ProductVariant set IsDefault=1 where ProductID=" + pID.ToString() + " and VariantID in (SELECT top 1 VariantID from ProductVariant where ProductID=" + pID.ToString() + " order by DisplayOrder,Name)");
        }
        else
        {
            string temp = CommonLogic.FormUSInt("IsDefault").ToString();
            DB.ExecuteSQL("update ProductVariant set IsDefault=1 where ProductID=" + pID.ToString() + " and VariantID=" + temp);
        }
        for (int i = 0; i <= Request.Form.Count - 1; i++)
        {
            if (Request.Form.Keys[i].IndexOf("DisplayOrder_") != -1)
            {
                String[] keys = Request.Form.Keys[i].Split('_');
                int VariantID = Localization.ParseUSInt(keys[1]);
                int DispOrd = 1;
                try
                {
                    DispOrd = Localization.ParseUSInt(Request.Form[Request.Form.Keys[i]]);
                }
                catch { }
                DB.ExecuteSQL("update productvariant set DisplayOrder=" + DispOrd.ToString() + " where VariantID=" + VariantID.ToString());
            }
        }

        this.resultFilter("");
    }

    protected void btnDeleteVariants_Click(object sender, EventArgs e)
    {
        DB.ExecuteSQL("delete from CustomCart where VariantID in (select VariantID from ProductVariant where ProductID=" + pID.ToString() + ")");
        DB.ExecuteSQL("delete from KitCart where VariantID in (select VariantID from ProductVariant where ProductID=" + pID.ToString() + ")");
        DB.ExecuteSQL("delete from ShoppingCart where VariantID in (select VariantID from ProductVariant where ProductID=" + pID.ToString() + ")");
        DB.ExecuteSQL("delete from ProductVariant where ProductID=" + pID.ToString());
    }
}
