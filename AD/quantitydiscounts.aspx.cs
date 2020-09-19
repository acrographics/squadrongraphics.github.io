// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/quantitydiscounts.aspx.cs 5     9/30/06 3:38p Redwoodtree $
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

public partial class quantitydiscounts : System.Web.UI.Page
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
            ViewState["EditingQD"] = false;
            ViewState["EditingQDID"] = "0";
            ViewState["UseBoxes"] = false;

            this.loadTree();
            this.loadScript(false);
            this.resetForm();
            this.phMain.Visible = false;

            this.btnDelete.Attributes.Add("onClick", "return confirm('Confirm Delete');");
        }
    }

    private void loadTree()
    {
        try
        {
            treeMain.Nodes.Clear();

            DataSet ds = DB.GetDS("select * from QuantityDiscount  " + DB.GetNoLock() + " order by DisplayOrder,Name", false);
			
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                TreeNode myNode = new TreeNode();
                myNode.Text = Server.HtmlEncode(DB.RowFieldByLocale(row, "Name", cust.LocaleSetting));
                myNode.Value = DB.RowFieldInt(row, "QuantityDiscountID").ToString();
                myNode.ImageUrl = "icons/dot.gif";
                treeMain.Nodes.Add(myNode);
            }

            ds.Dispose();
        }
        catch (Exception ex)
        {
            this.resetError(ex.ToString(), true);
        }
    }

    protected void treeMain_SelectedNodeChanged(object sender, EventArgs e)
    {
        this.ltValid.Text = "";
        this.resetForm();
        ViewState["EditingQD"] = true;
        this.loadScript(true);
            
        this.resetError("", false);
        this.phMain.Visible = true;

        string index = treeMain.SelectedNode.Value;
        ViewState["EditingQDID"] = index;

        this.btnInsert.Visible = true;
        this.btnInsert.Text = "Insert NEW Values";
        this.divInitial.Visible = false;
        this.divInitialDD.Visible = false;
        ViewState["UseBoxes"] = false;

        this.getQDDetails();
    }

    protected void getQDDetails()
    {
        IDataReader rs = DB.GetRS("select * from QuantityDiscount  " + DB.GetNoLock() + " where QuantityDiscountID=" + ViewState["EditingQDID"].ToString());
        if (!rs.Read())
        {
            rs.Close();
            this.resetError("Unable to retrieve data.", true);
            return;
        }

        //editing QD
        this.ltMode.Text = "Editing";
        this.btnSubmit.Text = "Update Discount Table Name";
        this.btnDelete.Visible = true;

        this.ltName.Text = AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Name"), "Name", false, true, true, "Please enter the quantity discount table name", 100, 30, 0, 0, false);
        ddlDscntType.SelectedIndex = DB.RSFieldTinyInt( rs , "DiscountType" );
        this.showDiscountGrid();
        rs.Close();
    }

    private void showDiscountGrid()
    {
        this.divGrid.Visible = true;
        this.buildGridData("LowQuantity");
    }
    private void hideDiscountGrid()
    {
        this.divGrid.Visible = false;
    }

    protected void resetError(string error, bool isError)
    {
        string str = "<font class=\"noticeMsg\">NOTICE:</font>&nbsp;&nbsp;&nbsp;";
        if (isError)
        {
            str = "<font class=\"errorMsg\">ERROR:</font>&nbsp;&nbsp;&nbsp;";
        }

        if (error.Length > 0)
        {
            str += error + "";
        }
        else
        {
            str = "";
        }

        ((Literal)this.Form.FindControl("ltError")).Text = str;
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        this.ltValid.Text = "";
        ViewState["EditingQD"] = false;
        ViewState["EditingQDID"] = "0";
        this.loadScript(true);
        this.phMain.Visible = true;
        this.btnDelete.Visible = false;
        this.resetForm();
        this.loadTree();
        //new QD
        this.ltMode.Text = "Adding a";
        this.btnSubmit.Text = "Add Discount Table";

        this.divInitialDD.Visible = true;
        ViewState["UseBoxes"] = true;
        this.btnInsert.Visible = false;

        this.hideDiscountGrid();
    }

    protected void loadScript(bool load)
    {
        if (load)
        {
            if (AppLogic.NumLocaleSettingsInstalled() > 1)
            {
                this.ltScript1.Text = CommonLogic.ReadFile("jscripts/tabs.js", true);
            }
        }
        else
        {
            this.ltScript.Text = "";
            this.ltStyles.Text = "";
        }
    }

    protected bool validateInput()
    {
        string frmName = AppLogic.FormLocaleXml( "Name" );
        if (frmName.Equals("<ml></ml>") || string.IsNullOrEmpty( frmName ))
        {
            string temp = "Please enter the quantity discount table name! <script type=\"text/javascript\">alert('Please enter the quantity discount table name!');</script>";
            this.resetError(temp, true);
            return false;
        }
        return true;
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        this.ltValid.Text = "";
        bool Editing = Localization.ParseBoolean(ViewState["EditingQD"].ToString());
        string QDID = ViewState["EditingQDID"].ToString();
        IDataReader rs;

        if (this.validateInput())
        {
            this.divInitialDD.Visible = false;

            try
            {
                StringBuilder sql = new StringBuilder(2500);
                if (!Editing)
                {
                    // ok to add:

                    ResetTxtControls();

                    String NewGUID = DB.GetNewGUID();
                    sql.Append("insert into quantitydiscount(QuantityDiscountGUID,Name,DiscountType) values(");
                    sql.Append(DB.SQuote(NewGUID) + ",");
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                    sql.Append( ddlDscntType.SelectedValue );
                    sql.Append(")");

                    DB.ExecuteSQL(sql.ToString());
                    this.resetError("Discount Table added.", false);

                    rs = DB.GetRS("select QuantityDiscountID from QuantityDiscount  " + DB.GetNoLock() + " where QuantityDiscountGUID=" + DB.SQuote(NewGUID));
                    rs.Read();
                    QDID = DB.RSFieldInt(rs, "QuantityDiscountID").ToString();
                    rs.Close();
                    ViewState["EditingQD"] = true;
                    ViewState["EditingQDID"] = QDID;
                    this.resetForm();
                    this.loadTree();
                    this.getQDDetails();

                    this.btnInsert.Visible = true;
                    this.btnInsert.Text = "Insert Initial Values";

                    //based on the number of initial values selected, display boxes
                    this.divInitial.Visible = true;
                    int boxes = Localization.ParseUSInt(this.ddValues.SelectedValue);
                    if (boxes == 3)
                    {
                        this.tr4.Visible = false;
                        this.tr5.Visible = false;
                    }
                    if (boxes == 1)
                    {
                        this.tr2.Visible = false;
                        this.tr3.Visible = false;
                        this.tr4.Visible = false;
                        this.tr5.Visible = false;
                    }
                }
                else
                {
                    // ok to update:
                    sql.Append("update quantitydiscount set ");
                    sql.Append("Name=" + DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                    sql.Append( "DiscountType=" + ddlDscntType.SelectedValue );
                    sql.Append(" where QuantityDiscountID=" + QDID.ToString());
                    DB.ExecuteSQL(sql.ToString());

                    this.resetError("Discount Table name updated.", false);
                    this.loadTree();
                    this.getQDDetails();

                    this.btnInsert.Visible = true;
                    this.btnInsert.Text = "Insert NEW Values";
                    this.divInitial.Visible = false;
                }
            }
            catch (Exception ex)
            {
                this.resetError("Error occurred: " + ex.ToString(), true);
            }

            this.divInitialDD.Visible = false;
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        this.ltValid.Text = "";
        string QDID = ViewState["EditingQDID"].ToString();

        DB.ExecuteSQL("update category set QuantityDiscountID=0 where QuantityDiscountID=" + QDID);
        DB.ExecuteSQL("update product set QuantityDiscountID=0 where QuantityDiscountID=" + QDID);
        //DB.ExecuteSQL("update productvariant set QuantityDiscountID=0 where QuantityDiscountID=" + QDID);
        // delete the record:
        DB.ExecuteSQL("delete from quantitydiscounttable where quantitydiscountid=" + QDID);
        DB.ExecuteSQL("delete from QuantityDiscount where QuantityDiscountID=" + QDID);

        this.phMain.Visible = false;
        this.hideDiscountGrid();
        this.loadTree();
        this.loadScript(false);
        ViewState["EditingQD"] = false;
        ViewState["EditingQDID"] = "0";
        resetError("Discount Table deleted.", false);
    }

    protected void resetForm()
    {
        this.hideDiscountGrid();
        this.ltName.Text = AppLogic.GetLocaleEntryFields("", "Name", false, true, true, "Please enter the quantity discount table name", 100, 30, 0, 0, false);
    }

    protected bool validateForm()
    {
        bool valid = true;
        string temp = "";

        if ((AppLogic.FormLocaleXml("Name").Length == 0) || (AppLogic.FormLocaleXml("Name").Equals("<ml></ml>")))
        {
            valid = false;
            temp += "Please fill out the Name";
        }

        if (!valid)
        {
            this.ltName.Text = AppLogic.GetLocaleEntryFields(AppLogic.FormLocaleXml("Name"), "Name", false, true, true, "Please enter the quantity discount table name", 100, 30, 0, 0, false);
            
            this.ltValid.Text = "<script type=\"text/javascript\">alert('" + temp + "');</script>";
        }

        return valid;
    }

    protected void buildGridData(string order)
    {
        string sql = "select * from QuantityDiscountTable  " + DB.GetNoLock() + " where QuantityDiscountID=" + ViewState["EditingQDID"].ToString() + " order by " + order;
        
        DataSet ds = DB.GetDS(sql, false);
        gMain.DataSource = ds;
        gMain.DataBind();
        ds.Dispose();
    }
    protected void gMain_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        //if new item and cancel, must delete
        if (Localization.ParseBoolean(ViewState["IsInsert"].ToString()))
        {
            GridViewRow row = gMain.Rows[e.RowIndex];
            if (row != null)
            {
                int iden = Localization.ParseNativeInt(row.Cells[1].Text.ToString());
                this.deleteRowPerm(iden);
            }
        }

        ViewState["IsInsert"] = false;

        gMain.EditIndex = -1;
        this.buildGridData("LowQuantity");
    }
    protected void gMain_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ImageButton ib = (ImageButton)e.Row.FindControl("imgDelete");
            ib.Attributes.Add("onClick", "javascript: return confirm('Confirm Delete?')");

            //Click to edit
            if ((e.Row.RowState == DataControlRowState.Normal) || (e.Row.RowState == DataControlRowState.Alternate))
            {
                e.Row.Attributes.Add("ondblclick", "javascript:__doPostBack('gMain','Edit$" + e.Row.RowIndex + "')");
            }
        }
        if ((e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit)
        {
            DataRowView myrow = (DataRowView)e.Row.DataItem;
           
            TextBox txt = (TextBox)e.Row.FindControl("txtHigh");
            txt.Attributes.Add("onKeyPress", "javascript:if (event.keyCode == 13) return false;");
            txt = (TextBox)e.Row.FindControl("txtLow");
            txt.Attributes.Add("onKeyPress", "javascript:if (event.keyCode == 13) return false;");
            txt = (TextBox)e.Row.FindControl("txtPercent");
            txt.Attributes.Add("onKeyPress", "javascript:if (event.keyCode == 13) return false;");
        }
    }
    protected void gMain_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        this.resetError("", false);

        if (e.CommandName == "DeleteItem")
        {
            ViewState["IsInsert"] = false;
            gMain.EditIndex = -1;
            int iden = Localization.ParseNativeInt(e.CommandArgument.ToString());
            this.deleteRowPerm(iden);
        }
    }
    protected void deleteRowPerm(int iden)
    {
        StringBuilder sql = new StringBuilder(2500);
        sql.Append("DELETE FROM QuantityDiscountTable where QuantityDiscountTableID=" + iden.ToString());
        try
        {
            DB.ExecuteSQL(sql.ToString());
            AppLogic.ClearCache();
            this.buildGridData("LowQuantity");
            this.resetError("Discount Table Item Deleted", false);
        }
        catch (Exception ex)
        {
            throw new Exception("Couldn't delete from database: " + ex.ToString());
        }
    }
    protected void gMain_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        ViewState["IsInsert"] = false;
        GridViewRow row = gMain.Rows[e.RowIndex];

        if (row != null)
        {
            string iden = row.Cells[1].Text.ToString();
            TextBox low = (TextBox)row.FindControl("txtLow");
            TextBox high = (TextBox)row.FindControl("txtHigh");
            TextBox percent = (TextBox)row.FindControl("txtPercent");

            if (Localization.ParseUSInt(low.Text) > Localization.ParseUSInt(high.Text))
            {
                high.Text = (Localization.ParseUSInt(low.Text) + 1).ToString();
            }

            StringBuilder sql = new StringBuilder(2500);

            sql.Append("update QuantityDiscountTable set ");
            sql.Append("LowQuantity=" + Localization.IntStringForDB(Localization.ParseUSInt(low.Text)) + ",");
            sql.Append("HighQuantity=" + Localization.IntStringForDB(Localization.ParseUSInt(high.Text)) + ",");
            sql.Append("DiscountPercent=" + Localization.DecimalStringForDB(Localization.ParseUSDecimal(percent.Text)));
            sql.Append(" where QuantityDiscountTableID=" + iden);
            DB.ExecuteSQL(sql.ToString());

            this.resetError("Quantity Table Item updated", false);

            try
            {
                DB.ExecuteSQL(sql.ToString());
                gMain.EditIndex = -1;
                DB.ExecuteSQL("Update QuantityDiscountTable set HighQuantity=999999 where HighQuantity=0 and LowQuantity<>0");
                this.buildGridData("LowQuantity");
            }
            catch
            {
                this.resetError("Couldn't update values.", true);
            }
        }
    }
    protected void gMain_RowEditing(object sender, GridViewEditEventArgs e)
    {
        ViewState["IsInsert"] = false;
        gMain.EditIndex = e.NewEditIndex;

        this.loadScript(true);

        this.buildGridData("LowQuantity");
    }
    protected void btnInsert_Click(object sender, EventArgs e)
    {
        ViewState["IsInsert"] = false;
        bool initial = Localization.ParseBoolean(ViewState["UseBoxes"].ToString());

        gMain.EditIndex = -1;
        StringBuilder sql = new StringBuilder(2500);

        string QuantityDiscountID = ViewState["EditingQDID"].ToString();
        String NewGUID = DB.GetNewGUID();

        if (!initial)
        {
            try
            {
                DB.ExecuteSQL("insert into QuantityDiscountTable(QuantityDiscountTableGUID,QuantityDiscountID,LowQuantity,HighQuantity,DiscountPercent,CreatedOn) values(" + DB.SQuote(NewGUID) + "," + QuantityDiscountID.ToString() + "," + Localization.IntStringForDB(0) + "," + Localization.IntStringForDB(999999) + "," + Localization.DecimalStringForDB(Localization.ParseUSDecimal("0.00")) + "," + DB.DateQuote(Localization.ToDBDateTimeString(System.DateTime.Now)) + ")");
                gMain.EditIndex = 0;
                this.buildGridData("QuantityDiscountTableID DESC");
                this.resetError("Quantity Discount Item added.", false);
                ViewState["IsInsert"] = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't update database: " + ex.ToString());
            }
        }
        else
        {
            //get initial box values
            int boxes = Localization.ParseUSInt(this.ddValues.SelectedValue);
            
            int low1 = Localization.ParseUSInt(this.txtLow1.Text);
            int high1 = Localization.ParseUSInt(this.txtHigh1.Text);
            decimal percent1 = Localization.ParseUSDecimal(this.txtPercent1.Text);
            int low2 = Localization.ParseUSInt(this.txtLow2.Text);
            int high2 = Localization.ParseUSInt(this.txtHigh2.Text);
            decimal percent2 = Localization.ParseUSDecimal(this.txtPercent2.Text);
            int low3 = Localization.ParseUSInt(this.txtLow3.Text);
            int high3 = Localization.ParseUSInt(this.txtHigh3.Text);
            decimal percent3 = Localization.ParseUSDecimal(this.txtPercent3.Text);
            int low4 = Localization.ParseUSInt(this.txtLow4.Text);
            int high4 = Localization.ParseUSInt(this.txtHigh4.Text);
            decimal percent4 = Localization.ParseUSDecimal(this.txtPercent4.Text);
            int low5 = Localization.ParseUSInt(this.txtLow5.Text);
            int high5 = Localization.ParseUSInt(this.txtHigh5.Text);
            decimal percent5 = Localization.ParseUSDecimal(this.txtPercent5.Text);

            //insert first
            if (low1 != 0 || high1 != 0)
            {
                DB.ExecuteSQL("insert into QuantityDiscountTable(QuantityDiscountTableGUID,QuantityDiscountID,LowQuantity,HighQuantity,DiscountPercent,CreatedOn) values(" + DB.SQuote(DB.GetNewGUID()) + "," + QuantityDiscountID.ToString() + "," + Localization.IntStringForDB(low1) + "," + Localization.IntStringForDB(high1) + "," + Localization.DecimalStringForDB(percent1) + "," + DB.DateQuote(Localization.ToDBDateTimeString(System.DateTime.Now)) + ")");
            }

            if (boxes > 1)
            {
                //insert second and third
                if (low2 != 0 || high2 != 0)
                {
                    DB.ExecuteSQL("insert into QuantityDiscountTable(QuantityDiscountTableGUID,QuantityDiscountID,LowQuantity,HighQuantity,DiscountPercent,CreatedOn) values(" + DB.SQuote(DB.GetNewGUID()) + "," + QuantityDiscountID.ToString() + "," + Localization.IntStringForDB(low2) + "," + Localization.IntStringForDB(high2) + "," + Localization.DecimalStringForDB(percent2) + "," + DB.DateQuote(Localization.ToDBDateTimeString(System.DateTime.Now)) + ")");
                }
                if (low3 != 0 || high3 != 0)
                {
                    DB.ExecuteSQL("insert into QuantityDiscountTable(QuantityDiscountTableGUID,QuantityDiscountID,LowQuantity,HighQuantity,DiscountPercent,CreatedOn) values(" + DB.SQuote(DB.GetNewGUID()) + "," + QuantityDiscountID.ToString() + "," + Localization.IntStringForDB(low3) + "," + Localization.IntStringForDB(high3) + "," + Localization.DecimalStringForDB(percent3) + "," + DB.DateQuote(Localization.ToDBDateTimeString(System.DateTime.Now)) + ")");
                }
            }

            if (boxes > 3)
            {
                //insert fourth and fifth
                if (low4 != 0 || high4 != 0)
                {
                    DB.ExecuteSQL("insert into QuantityDiscountTable(QuantityDiscountTableGUID,QuantityDiscountID,LowQuantity,HighQuantity,DiscountPercent,CreatedOn) values(" + DB.SQuote(DB.GetNewGUID()) + "," + QuantityDiscountID.ToString() + "," + Localization.IntStringForDB(low4) + "," + Localization.IntStringForDB(high4) + "," + Localization.DecimalStringForDB(percent4) + "," + DB.DateQuote(Localization.ToDBDateTimeString(System.DateTime.Now)) + ")");
                }
                if (low5 != 0 || high5 != 0)
                {
                    DB.ExecuteSQL("insert into QuantityDiscountTable(QuantityDiscountTableGUID,QuantityDiscountID,LowQuantity,HighQuantity,DiscountPercent,CreatedOn) values(" + DB.SQuote(DB.GetNewGUID()) + "," + QuantityDiscountID.ToString() + "," + Localization.IntStringForDB(low5) + "," + Localization.IntStringForDB(high5) + "," + Localization.DecimalStringForDB(percent5) + "," + DB.DateQuote(Localization.ToDBDateTimeString(System.DateTime.Now)) + ")");
                }
            }
            this.divInitial.Visible = false;

            gMain.EditIndex = -1; 
            DB.ExecuteSQL("Update QuantityDiscountTable set HighQuantity=999999 where HighQuantity=0 and LowQuantity<>0");
                
            this.buildGridData("LowQuantity");
            this.resetError("Quantity Discount Items added.", false);
            ViewState["UseBoxes"] = false;
            this.btnInsert.Text = "Insert NEW Values";        
        }
    }

    private void ResetTxtControls()
    {
        txtLow1.Text = string.Empty;
        txtHigh1.Text = string.Empty;
        txtPercent1.Text = string.Empty;
        txtLow2.Text = string.Empty;
        txtHigh2.Text = string.Empty;
        txtPercent2.Text = string.Empty;
        txtLow3.Text = string.Empty;
        txtHigh3.Text = string.Empty;
        txtPercent3.Text = string.Empty;
        txtLow4.Text = string.Empty;
        txtHigh4.Text = string.Empty;
        txtPercent4.Text = string.Empty;
        txtLow5.Text = string.Empty;
        txtHigh5.Text = string.Empty;
        txtPercent5.Text = string.Empty;
    }
}
