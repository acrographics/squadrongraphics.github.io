// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/stringresource.aspx.cs 12    9/30/06 1:08p Redwoodtree $
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

    public partial class stringresource : System.Web.UI.Page
    {
        protected Customer cust;
        protected string selectSQL = "select * from StringResource ";

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            if (!IsPostBack)
            {
                string query = CommonLogic.QueryStringCanBeDangerousContent("searchfor");
                string locale = CommonLogic.QueryStringCanBeDangerousContent("ShowLocaleSetting");

                loadTree();
                loadLocales();
                ViewState["Sort"] = "Name";
                ViewState["SortOrder"] = "ASC";
                ViewState["SQLString"] = selectSQL;

                if ((query.Length > 0) || (locale.Length > 0))
                {
                    pnlAdd.Visible = false;
                    pnlGrid.Visible = true;
                    resultFilter(query, locale);
                }
                else
                {
                    ShowAddPanel(false);
                }

                txtSearch.Attributes.Add("onKeyPress", "javascript:if (event.keyCode == 13) __doPostBack('btnSearch','')");
            }

            if (!ddLocales.SelectedValue.Equals("Reset") && !ddLocales.SelectedValue.Equals("NEW String"))
            {
                divActions.Visible = true;
                if (ddLocales.SelectedValue.Equals("en-US"))
                {
                    btnShowMissing.Visible = false;
                }
                else
                {
                    btnShowMissing.Visible = true;
                }

                //Confirmations
                if (DB.GetSqlN("select count(*) as N from StringResource with (NOLOCK) where LocaleSetting=" + DB.SQuote(ddLocales.SelectedValue)) == 0)
                {
                    btnLoadExcelServer.Text = "Load from Excel File On Server";
                    btnUploadExcel.Text = "Load from Excel File On Your PC";
                    btnClearLocale.Visible = false;
                    btnShowMissing.Visible = false;
                    btnShowModified.Visible = false;
                }
                else
                {
                    btnLoadExcelServer.Text = "ReLoad from Excel File On Server";
                    btnUploadExcel.Text = "ReLoad from Excel File On Your PC";
                    btnClearLocale.Visible = true;
                    btnShowMissing.Visible = true;
                    btnShowModified.Visible = true;
                }

                btnLoadExcelServer.Attributes.Add("onclick", "return confirm('Are you sure you want to reload all strings for the " + ddLocales.SelectedValue + " locale from the /StringResources/strings." + ddLocales.SelectedValue + ".xls file?')");
                btnClearLocale.Attributes.Add("onclick", "return confirm('Are you sure you want to delete all strings in the " + ddLocales.SelectedValue + " locale from the database?')");
                btnUploadExcel.Attributes.Add("onclick", "return confirm('Are you sure you want to upload and load an excel file with all strings for the " + ddLocales.SelectedValue + " locale?')");
            }
            else
            {
                divActions.Visible = false;
            }
        }

        protected void loadScript(bool load)
        {
            if (load)
            {
                
            }
            else
            {
                
            }
        }

        private void loadTree()
        {
            try
            {
                string index = "#ABCDEFGHIJKLMNOPQRSTUVWXYZ";
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
                resetError(ex.ToString(), true);
            }
        }

        private void loadLocales()
        {
            try
            {
                ddLocales.Items.Clear();
                ddLocales.Items.Add(new ListItem("ALL LOCALES", "Reset"));
                //ddLocales.Items.Add(new ListItem("NEW String", "NEW String"));
                DataSet ds = DB.GetDS("select Name from LocaleSetting " + DB.GetNoLock() + " order by DisplayOrder,Description", AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    ListItem myNode = new ListItem();
                    myNode.Value = DB.RowField(row, "Name");
                    ddLocales.Items.Add(myNode);
                }
                ds.Dispose();
            }
            catch (Exception ex)
            {
                resetError(ex.ToString(), true);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gMain.EditIndex = -1;
            resetError("", false);

            resultFilter("", "");
        }

        protected void resultFilter(string SearchFor, string locale)
        {
            if (SearchFor.Length == 0)
            {
                SearchFor = txtSearch.Text;
            }
            if (locale.Length == 0)
            {
                locale = ddLocales.SelectedValue;
            }

            String sql = selectSQL + DB.GetNoLock() + " ";
            String WhereClause = String.Empty;

            //search filter
            if (SearchFor.Length != 0)
            {
                if (WhereClause.Length != 0)
                {
                    WhereClause += " and ";
                }
                WhereClause += " (Name like " + DB.SQuote("%" + SearchFor + "%") + " or ConfigValue like " + DB.SQuote("%" + SearchFor + "%") + ")";
            }

            //locale filter
            if ((locale.Length != 0) && (!locale.Equals("Reset")))
            {
                if (WhereClause.Length != 0)
                {
                    WhereClause += " and ";
                }
                WhereClause += " LocaleSetting like " + DB.SQuote(locale);
            }

            //starts with filter
            string Index = "";
            for (int i = 0; i < treeMain.Nodes.Count; i++)
            {
                if (treeMain.Nodes[i].Selected)
                {
                    Index = treeMain.Nodes[i].Value;
                    break;
                }
            }
            if (Index.Length > 0)
            {
                if (!Index.Equals("All"))
                {
                    if (WhereClause.Length != 0)
                    {
                        WhereClause += " and ";
                    }
                    WhereClause += " Name like " + DB.SQuote(Index + "%");
                }
                if (Index.Equals("#"))
                {
                    if (WhereClause.Length != 0)
                    {
                        WhereClause += " and ";
                    }
                    WhereClause += " Name like (" + DB.SQuote("0%") + ")";
                    WhereClause += " OR Name like (" + DB.SQuote("1%") + ")";
                    WhereClause += " OR Name like (" + DB.SQuote("2%") + ")";
                    WhereClause += " OR Name like (" + DB.SQuote("3%") + ")";
                    WhereClause += " OR Name like (" + DB.SQuote("4%") + ")";
                    WhereClause += " OR Name like (" + DB.SQuote("5%") + ")";
                    WhereClause += " OR Name like (" + DB.SQuote("6%") + ")";
                    WhereClause += " OR Name like (" + DB.SQuote("7%") + ")";
                    WhereClause += " OR Name like (" + DB.SQuote("8%") + ")";
                    WhereClause += " OR Name like (" + DB.SQuote("9%") + ")";
                    WhereClause += " OR Name like (" + DB.SQuote("10%") + ")";
                }
            }

            //Super admin filter
            if (!cust.IsAdminSuperUser)
            {
                if (WhereClause.Length != 0)
                {
                    WhereClause += " and ";
                }
                WhereClause += " UPPER(Name) <> 'ADMIN_SUPERUSER' ";
            }
            if (WhereClause.Length != 0)
            {
                sql += " where " + WhereClause;
            }

            ViewState["SQLString"] = sql.ToString();
            sql += " order by " + ViewState["Sort"].ToString() + " " + ViewState["SortOrder"].ToString();

            buildGridData(DB.GetDS(sql, false));

            txtSearch.Text = SearchFor;
            ddLocales.SelectedIndex = -1;
            (ddLocales.Items.FindByValue(locale)).Selected = true;
        }

        protected void ddLocales_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetError("", false);
            resultFilter("", "");
        }

        protected void treeMain_SelectedNodeChanged(object sender, EventArgs e)
        {
            gMain.EditIndex = -1;
            resetError("", false);

            resultFilter("", "");
        }

        protected DataSet buildGridData()
        {
            string sql = ViewState["SQLString"].ToString();
            sql += " order by " + ViewState["Sort"].ToString() + " " + ViewState["SortOrder"].ToString();

            return DB.GetDS(sql, false);
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

            ((Literal)Form.FindControl("ltError")).Text = str;
        }

        protected void gMain_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            resetError("", false);

            gMain.EditIndex = -1;
            buildGridData(buildGridData());
        }
        protected void gMain_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if ((e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit)
            {
                DataRowView myrow = (DataRowView)e.Row.DataItem;
                string locale = myrow["LocaleSetting"].ToString().ToLowerInvariant();
                DropDownList dd = (DropDownList)e.Row.FindControl("ddLocale");
                DataSet ds = DB.GetDS("select * from LocaleSetting  " + DB.GetNoLock() + " order by displayorder,description", AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dd.Items.Add(new ListItem(dr["Name"].ToString(), dr["Name"].ToString()));
                    if (dr["Name"].ToString().ToLowerInvariant().Equals(locale))
                    {
                        dd.Items[dd.Items.Count - 1].Selected = true;
                    }
                }
                ds.Dispose();
            }

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
        }
        protected void gMain_Sorting(object sender, GridViewSortEventArgs e)
        {
            resetError("", false);

            gMain.EditIndex = -1;
            ViewState["Sort"] = e.SortExpression.ToString();
            ViewState["SortOrder"] = (ViewState["SortOrder"].ToString() == "ASC" ? "DESC" : "ASC");
            buildGridData(buildGridData());
        }
        protected void gMain_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            resetError("", false);

            if (e.CommandName == "DeleteItem")
            {
                gMain.EditIndex = -1;
                int iden = Localization.ParseNativeInt(e.CommandArgument.ToString());
                deleteRow(iden);
            }
        }
        protected void deleteRow(int iden)
        {
            StringBuilder sql = new StringBuilder(2500);
            sql.Append("delete from StringResource where StringResourceID=" + iden.ToString());
            try
            {
                DB.ExecuteSQL(sql.ToString());
                AppLogic.ClearCache();
                buildGridData(buildGridData());
                resetError("Item Deleted", false);
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't update database: " + ex.ToString());
            }
        }

        protected void gMain_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            resetError("", false);
            gMain.PageIndex = e.NewPageIndex;
            gMain.EditIndex = -1;
            buildGridData(buildGridData());
        }
        protected void gMain_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gMain.Rows[e.RowIndex];

            if (row != null)
            {
                string iden = row.Cells[1].Text.ToString();
                TextBox name = (TextBox)row.FindControl("txtName");
                TextBox value = (TextBox)row.FindControl("txtValue");
                DropDownList locale = (DropDownList)row.FindControl("ddLocale");
                StringBuilder sql = new StringBuilder(2500);

                int N = DB.GetSqlN("select count(Name) as N from StringResource " + DB.GetNoLock() + " where Name=" + DB.SQuote(name.Text) + " and LocaleSetting=" + DB.SQuote(locale.SelectedValue) + " and StringResourceID <> " + iden);
                if (N != 0)
                {
                    resetError("Another string exists with that Name and Locale combination.", true);
                    return;
                }

                //// ok to update:
                //sql.Append("update StringResource set ");
                //sql.Append("Name=" + DB.SQuote(name.Text) + ",");
                //sql.Append("LocaleSetting=" + DB.SQuote(Localization.CheckLocaleSettingForProperCase(locale.SelectedValue)) + ",");
                //sql.Append("ConfigValue=" + DB.SQuote(value.Text));
                //sql.Append(",Modified=1");
                //sql.Append(" where StringResourceID=" + iden);

                //string err = StringResource.Update(Convert.ToInt32(iden), name.Text, Localization.CheckLocaleSettingForProperCase(locale.SelectedValue), value.Text);
                StringResource sr = AppLogic.StringResourceTable[Convert.ToInt32(iden)];
                if (sr != null)
                {
                    string err = sr.Update(name.Text, Localization.CheckLocaleSettingForProperCase(locale.SelectedValue), value.Text);
                    if (err == string.Empty)
                    {
                        //DB.ExecuteSQL(sql.ToString());
                        resetError("Item updated", false);
                        //AppLogic.ClearCache();
                        gMain.EditIndex = -1;
                    }
                    else
                    {
                        resetError("Item could not be updated, The following error was received: " + err, true);
                    }
                }
                else
                {
                    resetError("Item could not be found in collection", true);
                }

                resultFilter("", Localization.CheckLocaleSettingForProperCase(locale.SelectedValue));
            }
        }
        protected void gMain_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gMain.EditIndex = e.NewEditIndex;

            buildGridData(buildGridData());

        }        

        protected void btnLoadExcelServer_Click(object sender, EventArgs e)
        {
            resetError("", false);
            string selectedLocale = ddLocales.SelectedValue;
            string stringResourceFilePath = string.Empty;
            bool stringResourceFileExists = AppLogic.CheckStringResourceExcelFileExists(selectedLocale, out stringResourceFilePath);
            if (stringResourceFileExists)
            {
                Response.Redirect(string.Format("importstringresourcefile1.aspx?showlocalesetting={0}&master=true", selectedLocale));
                //AppLogic.ReloadStringResources(ddLocales.SelectedValue);
                //resetError("Locale reloaded from Server.", false);
                //buildGridData(buildGridData());
            }
            else
            {
                resetError(string.Format("String Resource File {0} not found!!!",stringResourceFilePath), true);
            }
        }

        protected void btnClearLocale_Click(object sender, EventArgs e)
        {
            resetError("", false);
            DB.ExecuteSQL("delete from StringResource where LocaleSetting=" + DB.SQuote(ddLocales.SelectedValue));
            AppLogic.LoadStringResourcesFromDB(false);
            resetError("Locale Cleared.", false);
            buildGridData(buildGridData());
            btnClearLocale.Visible = false;
            btnShowMissing.Visible = false;
            btnShowModified.Visible = false;
        }
        protected void btnUploadExcel_Click(object sender, EventArgs e)
        {
            Response.Redirect("importstringresourcefile1.aspx?showlocalesetting=" + ddLocales.SelectedValue);
        }
        protected void btnShowMissing_Click(object sender, EventArgs e)
        {
            Response.Redirect("stringresourcerpt.aspx?reporttype=missing&ShowLocaleSetting=" + ddLocales.SelectedValue);
        }
        protected void btnShowModified_Click(object sender, EventArgs e)
        {
            Response.Redirect("stringresourcerpt.aspx?reporttype=modified&ShowLocaleSetting=" + ddLocales.SelectedValue);
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            resetError("", false);

            gMain.EditIndex = -1;
            ShowAddPanel(true);

            txtDescription.Text = "";
            txtName.Text = "";

            ddLocale.Items.Clear();
            ddLocale.Items.Add(new ListItem("- Select -", "0"));

            DataSet ds = DB.GetDS("select * from LocaleSetting  " + DB.GetNoLock() + " order by displayorder,description", AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ddLocale.Items.Add(new ListItem(dr["Name"].ToString(), dr["Name"].ToString()));
            }
            ds.Dispose();

            try
            {
                if (ddLocales.SelectedValue != "Reset")
                {
                    ddLocale.SelectedValue = ddLocales.SelectedValue;
                }
            }
            catch { }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            resetError("", false);
            StringBuilder sql = new StringBuilder();

            if (ValidInput())
            {
                string name = txtName.Text.Trim();
                string value = txtDescription.Text.Trim();
                string locale = this.ddLocale.SelectedValue;


                StringResource sr = AppLogic.StringResourceTable[locale, name];
                if (sr == null)
                {
                    string err = AppLogic.StringResourceTable.Add(name, locale, value);
                    if (err == string.Empty)
                    {
                        this.resetError("String Resource added.", false);
                        ShowAddPanel(false);
                    }
                    else
                    {
                        this.resetError("String Resource was not added.  The following error occured: " + err, true);
                        ShowAddPanel(true);
                    }
                }
                else
                {
                    this.resetError("String Resource already exists.", true);
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