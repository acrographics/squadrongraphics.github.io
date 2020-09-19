// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/producttypes.aspx.cs 5     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web.UI.WebControls;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for producttypes.
	/// </summary>
	public partial class producttypes : System.Web.UI.Page
	{
        protected string selectSQL = "select * from ProductType ";
        protected Customer cust;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            if (!IsPostBack)
            {
                this.loadScript(false);
                ViewState["Sort"] = "Name";
                ViewState["SortOrder"] = "ASC";
                ViewState["SQLString"] = this.selectSQL;

                this.buildGridData(this.buildGridData());
            }
        }

        protected void loadScript(bool load)
        {
            if (load)
            {
                if (AppLogic.NumLocaleSettingsInstalled() > 1)
                {
                    this.ltScript.Text += CommonLogic.ReadFile("jscripts/tabs.js", true);
                }
            }
            else
            {
                this.ltScript.Text = "";
            }
        }

        protected DataSet buildGridData()
        {
            string sql = ViewState["SQLString"].ToString();
            sql += " order by " + ViewState["Sort"].ToString() + " " + ViewState["SortOrder"].ToString() + ", DisplayOrder";

            DataSet ds = DB.GetDS(sql, false);
            if (ds.Tables[0].Rows.Count > 0)
            {
                ds.Tables[0].Columns.Add("EditName");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dr["EditName"] = AppLogic.GetLocaleEntryFields(dr[2].ToString(), "Name", false, true, true, "Please enter the Product Type name", 100, 25, 0, 0, false);
                }
            }

            return ds;
        }

        protected void buildGridData(DataSet ds)
        {
            gMain.DataSource = ds;
            gMain.DataBind();
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

        protected void gMain_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            //if new item and cancel, must delete
            if (Localization.ParseBoolean(ViewState["IsInsert"].ToString()))
            {
                GridViewRow row = gMain.Rows[e.RowIndex];
                if (row != null)
                {
                    int iden = Localization.ParseNativeInt(row.Cells[1].Text.ToString());
                    this.deleteRow(iden);
                }
            }

            ViewState["IsInsert"] = false;
            ViewState["SQLString"] = this.selectSQL;

            gMain.EditIndex = -1;
            this.buildGridData(this.buildGridData());
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

                //Localization
                if ((e.Row.RowState == DataControlRowState.Normal) || (e.Row.RowState == DataControlRowState.Alternate))
                {
                    e.Row.Cells[2].Text = XmlCommon.GetLocaleEntry(((Literal)e.Row.FindControl("ltName")).Text, cust.LocaleSetting, false);
                }
            }
        }
        protected void gMain_Sorting(object sender, GridViewSortEventArgs e)
        {
            ViewState["IsInsert"] = false;
            gMain.EditIndex = -1;
            ViewState["Sort"] = e.SortExpression.ToString();
            ViewState["SortOrder"] = (ViewState["SortOrder"].ToString() == "ASC" ? "DESC" : "ASC");
            this.buildGridData(this.buildGridData());
        }
        protected void gMain_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            this.resetError("", false);

            if (e.CommandName == "DeleteItem")
            {
                ViewState["IsInsert"] = false;
                gMain.EditIndex = -1;
                int iden = Localization.ParseNativeInt(e.CommandArgument.ToString());
                this.deleteRow(iden);
            }
        }
        
        protected void deleteRow(int iden)
        {
            StringBuilder sql = new StringBuilder(2500);
            sql.Append("delete from ProductType where ProductTypeID=" + iden);
            try
            {
                DB.ExecuteSQL(sql.ToString());
                AppLogic.ClearCache();
                this.buildGridData(this.buildGridData());
                this.resetError("Item Deleted", false);
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't delete from database: " + ex.ToString());
            }
        }

        protected void gMain_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["IsInsert"] = false;
            this.resetError("", false);
            gMain.PageIndex = e.NewPageIndex;
            gMain.EditIndex = -1;
            this.buildGridData(this.buildGridData());
        }
        protected void gMain_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            //ViewState["IsInsert"] = false;
            bool IsInserted = (bool)ViewState["IsInsert"];
            GridViewRow row = gMain.Rows[e.RowIndex];

            if (row != null)
            {
                string iden = row.Cells[1].Text.ToString();
                string name = AppLogic.FormLocaleXml("Name");

                // see if this appconfig already exists:
                int N = DB.GetSqlN("select count(Name) as N from producttype  " + DB.GetNoLock() + " where producttypeid<>" + iden + " and lower(Name)=" + DB.SQuote(name.ToLowerInvariant()));
                if (N != 0)
                {
                    this.resetError("There is already another product type with that name.", true);
                    return;
                }

                StringBuilder sql = new StringBuilder();

                if (!IsInserted)
                {
                    sql.Append("update producttype set ");
                    sql.Append("Name=" + DB.SQuote(name));
                    sql.Append(" where producttypeID=" + iden);
                }
                else
                {
                    String NewGUID = DB.GetNewGUID();
                    sql.Append("insert into producttype(producttypeGUID,Name) values(");
                    sql.Append(DB.SQuote(NewGUID) + ",");
                    sql.Append(DB.SQuote(name));
                    sql.Append(")");
                }

                try
                {
                    DB.ExecuteSQL(sql.ToString());
                    this.resetError("Item updated", false);
                    AppLogic.ClearCache();
                    gMain.EditIndex = -1;
                    ViewState["SQLString"] = this.selectSQL;
                    this.buildGridData(this.buildGridData());
                }
                catch (Exception ex)
                {
                    throw new Exception("Couldn't update database: " + sql.ToString() + ex.ToString());
                }
            }
        }
        protected void gMain_RowEditing(object sender, GridViewEditEventArgs e)
        {
            ViewState["IsInsert"] = false;
            gMain.EditIndex = e.NewEditIndex;

            this.loadScript(true);

            this.buildGridData(this.buildGridData());
        }
        protected void btnInsert_Click(object sender, EventArgs e)
        {
            ViewState["IsInsert"] = false;
            gMain.EditIndex = -1;
            StringBuilder sql = new StringBuilder();

            // see if this name is already there:
            string name = "NEW Product Type";
            int N = DB.GetSqlN("select count(producttypeid) as N from producttype  " + DB.GetNoLock() + " where lower(Name)=" + DB.SQuote(name.ToLowerInvariant()));
            if (N != 0)
            {
                name = name + DateTime.Now.Millisecond;
            }

            // ok to add them:
            String NewGUID = DB.GetNewGUID();
            sql.Append("insert into producttype(producttypeGUID,Name) values(");
            sql.Append(DB.SQuote(NewGUID) + ",");
            sql.Append(DB.SQuote(name));
            sql.Append(")");

            try
            {
                DB.ExecuteSQL(sql.ToString());
                AppLogic.ClearCache();
                ViewState["SQLString"] = selectSQL + " WHERE [Name]='" + name + "' ";
                ViewState["Sort"] = "producttypeID";
                ViewState["SortOrder"] = "DESC";
                gMain.EditIndex = 0;
                this.loadScript(true);
                DataSet ds = buildGridData();
                int productTypeID = (int)ds.Tables[0].Rows[0]["ProductTypeID"];
                deleteRow(productTypeID);
                buildGridData(ds);
                this.resetError("New Product Type added.", false);
                ViewState["IsInsert"] = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't update database: " + ex.ToString());
            }
        }
	}
}
