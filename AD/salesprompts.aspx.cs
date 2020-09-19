// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/salesprompts.aspx.cs 8     9/15/06 10:27p Redwoodtree $
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

    public partial class salesprompts : System.Web.UI.Page
    {
        protected string selectSQL = "select * from SalesPrompt where deleted=0";
        protected Customer cust;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            if (!IsPostBack)
            {
                //Delete Default
                DB.ExecuteSQL("DELETE FROM SalesPrompt WHERE [Name]='NEW Sales Prompt'");

                this.loadScript(false);
                ViewState["Sort"] = "Name";
                ViewState["SortOrder"] = "ASC";
                ViewState["SQLString"] = this.selectSQL;

                ShowAddPanel(false);
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
            sql += " order by " + ViewState["Sort"].ToString() + " " + ViewState["SortOrder"].ToString();

            DataSet ds = DB.GetDS(sql, false);
            if (ds.Tables[0].Rows.Count > 0)
            {
                ds.Tables[0].Columns.Add("EditName");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dr["EditName"] = AppLogic.GetLocaleEntryFields(dr[2].ToString(), "Name", false, true, true, "Please enter the " + AppLogic.GetString("AppConfig.CategoryPromptSingular", 0, cust.LocaleSetting).ToLowerInvariant() + " name", 100, 25, 0, 0, false);
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
                gMain.EditIndex = -1;
                int iden = Localization.ParseNativeInt(e.CommandArgument.ToString());
                this.deleteRow(iden);
            }
        }
        protected void deleteRow(int iden)
        {
            StringBuilder sql = new StringBuilder(2500);
            sql.Append("update salesprompt set deleted=1 where SalesPromptID=" + iden.ToString());
            try
            {
                DB.ExecuteSQL(sql.ToString());
                AppLogic.ClearCache();
                this.buildGridData(this.buildGridData());
                this.resetError("Item Deleted", false);
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't update database: " + ex.ToString());
            }
        }
        protected void deleteRowPerm(int iden)
        {
            StringBuilder sql = new StringBuilder(2500);
            sql.Append("DELETE FROM SalesPrompt where SalesPromptID=" + iden.ToString());
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
            this.resetError("", false);
            gMain.PageIndex = e.NewPageIndex;
            gMain.EditIndex = -1;
            this.buildGridData(this.buildGridData());
        }
        protected void gMain_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gMain.Rows[e.RowIndex];

            if (row != null)
            {
                string iden = row.Cells[1].Text.ToString();
                string name = AppLogic.FormLocaleXml("Name");

                // see if this appconfig already exists:
                int N = DB.GetSqlN("select count(Name) as N from SalesPrompt  " + DB.GetNoLock() + " where SalesPromptID<>" + iden + " and lower(Name)=" + DB.SQuote(name.ToLowerInvariant()));
                if (N != 0)
                {
                    this.resetError("There is already another sales prompt with that name.", true);
                    return;
                }

                StringBuilder sql = new StringBuilder(2500);

                sql.Append("update salesprompt set ");
                sql.Append("Name=" + DB.SQuote(name));
                sql.Append(" where SalesPromptID=" + iden);

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
            gMain.EditIndex = e.NewEditIndex;
            
            this.loadScript(true);
            
            this.buildGridData(this.buildGridData());
        }
        protected void btnInsert_Click(object sender, EventArgs e)
        {
            resetError("", false);

            gMain.EditIndex = -1;
            ShowAddPanel(true);

            ltPrompt.Text = AppLogic.GetLocaleEntryFields("", "Name", false, true, true, "Please enter the Sales Prompt", 400, 25, 0, 0, false);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            resetError("", false);
            StringBuilder sql = new StringBuilder();

            if (ValidInput())
            {
                string name = AppLogic.FormLocaleXml("Name");

                // ok to add them:
                String NewGUID = DB.GetNewGUID();
                sql.Append("insert into SalesPrompt(SalesPromptGUID,Name) values(");
                sql.Append(DB.SQuote(NewGUID) + ",");
                sql.Append(DB.SQuote(name));
                sql.Append(")");

                try
                {
                    DB.ExecuteSQL(sql.ToString());
                    this.resetError("Sales Prompt added.", false);
                    ShowAddPanel(false);
                }
                catch
                {
                    this.resetError("Sales Prompt already exists.", true);
                    ShowAddPanel(true);
                }
            }
            else
            {
                this.resetError("Please input all required fields.", true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            resetError("", false);

            ShowAddPanel(false);
        }

        protected bool ValidInput()
        {
            if (AppLogic.FormLocaleXml("Name").Equals("<ml></ml>"))
            {
                return false;
            }

            return true;
        }

        protected void ShowAddPanel(bool showAdd)
        {
            if (showAdd)
            {
                loadScript(true);
                pnlAdd.Visible = true;
                pnlGrid.Visible = false;
            }
            else
            {
                loadScript(false);
                pnlAdd.Visible = false;
                pnlGrid.Visible = true;

                buildGridData(buildGridData());
            }
        }
    }
}