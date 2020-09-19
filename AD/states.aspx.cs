// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/states.aspx.cs 25    9/30/06 3:38p Redwoodtree $
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
	/// <summary>
	/// Summary description for states.
	/// </summary>
    public partial class states : System.Web.UI.Page
    {
        protected string selectSQL = "select State.*,Country.Name as Country from State  " + DB.GetNoLock() + " left outer join Country " + DB.GetNoLock() + " on State.CountryID=Country.CountryID";
        protected string defaultSort = "State.DisplayOrder,";
        protected Customer ThisCustomer;
        int RowBoundIndex = 1;

        private ArrayList TaxClassIDs = new ArrayList();
         

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            RowBoundIndex = 1;
            ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            //add tax class inputs
            IDataReader dr = DB.GetRS("SELECT * FROM TaxClass " + DB.GetNoLock() + " ORDER BY DisplayOrder, Name");
            while (dr.Read())
            {
                TaxClassIDs.Add(DB.RSFieldInt(dr, "TaxClassID"));
            }
            dr.Close();

            if (!IsPostBack)
            {
                DB.ExecuteSQL("delete from state where Name='NEW State'");

                this.loadScript(false);
                ViewState["Sort"] = "State.Name";
                ViewState["SortOrder"] = "ASC";
                ViewState["SQLString"] = this.selectSQL;

                ShowAddPanel(false);
            }
            RowBoundIndex = 1;

            ClientScriptManager cs = Page.ClientScript;

            StringBuilder tmpS = new StringBuilder(1024);
            tmpS.Append("function CopyTaxDown(theForm,TaxClassID,InitialFormField)\n");
            tmpS.Append("{\n");
            //tmpS.Append("	alert('Form=' + theForm.name + ' TaxClassID=' + TaxClassID + ' InitialFormField=' + InitialFormField);\n");
            tmpS.Append("   var pat = 'TR_' + TaxClassID;\n");
            //tmpS.Append("   alert('Pat=' + pat);\n");
            tmpS.Append("	for (i = 0; i < theForm.length; i++)\n");
            tmpS.Append("	{\n");
            tmpS.Append("		var str = theForm.elements[i].name;\n");
            tmpS.Append("       if(str.substring(0,pat.length) == pat)\n");
            tmpS.Append("       {\n");
            //tmpS.Append("		    alert(str);\n");
            tmpS.Append("           theForm.elements[i].value = document.getElementById(InitialFormField).value;\n");
            tmpS.Append("       }\n");
            tmpS.Append("	}\n");
            tmpS.Append("}\n");

            cs.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), tmpS.ToString(), true);

        }

        protected void loadScript(bool load)
        {
            if (!load)
            {
                this.ltScript.Text = "";
            }
        }

        protected DataSet buildGridData()
        {
            string sql = ViewState["SQLString"].ToString();
            sql += " order by " + defaultSort + " " + (ViewState["Sort"].ToString().ToLowerInvariant() == "displayorder" ? "Name" : ViewState["Sort"].ToString()) + " " + ViewState["SortOrder"].ToString();
            DataSet ds = DB.GetDS(sql, false);
            return ds;
        }

        protected void buildGridData(DataSet ds)
        {
            string temp = "<div style=\"border-bottom: solid 1px #cccccc; text-align: center; width: 100%; float: left; font-weight: bold;\">Specify Tax Rates</div><div style=\"clear: both;\"></div>";

            IDataReader dr = DB.GetRS("SELECT * FROM TaxClass " + DB.GetNoLock() + " ORDER BY DisplayOrder, Name");
            int count = 0;
            while (dr.Read())
            {
                count++;
                temp += "<div style=\"width: %W%; border-right: solid 1px white; float: left; white-space: wrap;\">" + XmlCommon.GetLocaleEntry(DB.RSField(dr, "Name"), ThisCustomer.LocaleSetting, false) + "</div>";
            }
            dr.Close();

            if (count > 0)
            {
                gMain.Columns[5].HeaderText = temp.Replace("%W%", ((100 / count) - 1) + "%");
            }

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
                DataRowView myrow = (DataRowView)e.Row.DataItem;
                int iden = Localization.ParseNativeInt(myrow["StateID"].ToString());

                ImageButton ib = (ImageButton)e.Row.FindControl("imgDelete");
                ib.Attributes.Add("onClick", "javascript: return confirm('Confirm Delete?')");

                Literal ltTaxRate = (Literal)e.Row.FindControl("ltTaxRate");

                StringBuilder tmpS = new StringBuilder(1024);
                for(int i = 0; i < TaxClassIDs.Count; i++)
                {
                    int classID = (int)TaxClassIDs[i];
                    string rate = Localization.CurrencyStringForDBWithoutExchangeRate(AppLogic.StateTaxRatesTable.GetTaxRate(iden, classID));
                    tmpS.Append("<div style=\"width: (!W!); border-right: solid 1px white; float: left; white-space: wrap;\">");
                    tmpS.Append("<input type=\"text\" name='TR_" + classID.ToString() + "_" + iden + "' class=\"single4chars\" value='" + rate + "'></input>%");
                    if (RowBoundIndex == 1)
                    {
                        tmpS.Append("&nbsp;<img href=\"javascript:void(0)\" onClick=\"CopyTaxDown(document.forms[0]," + classID.ToString() + ",'TR_" + classID.ToString() + "_" + iden.ToString() + "')\" style=\"cursor:hand; cursor:pointer;\" src=\"images/downarrow.gif\" border=\"0\" alt=\"Copy Rate Down\">");
                    }
                    tmpS.Append("</div>");
                }

                if (tmpS.Length > 0)
                {
                    ltTaxRate.Text = tmpS.ToString().Replace("(!W!)", ((100 / TaxClassIDs.Count) - 1) + "%");
                }
                else
                {
                    ltTaxRate.Text = "No Tax Classes";
                }

                RowBoundIndex++;

                //Click to edit
                if ((e.Row.RowState == DataControlRowState.Normal) || (e.Row.RowState == DataControlRowState.Alternate))
                {
                    e.Row.Attributes.Add("ondblclick", "javascript:__doPostBack('gMain','Edit$" + e.Row.RowIndex + "')");
                }

                if ((e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit)
                {
                    string country = myrow["Country"].ToString();

                    DropDownList dd = (DropDownList)e.Row.FindControl("ddCountry");
                    RadioButtonList cb = (RadioButtonList)e.Row.FindControl("rblTax");

                    ListItem li = new ListItem(" - Select One -", "0");
                    dd.Items.Add(li);

                    IDataReader rsst = DB.GetRS("select * from Country  " + DB.GetNoLock() + " order by DisplayOrder,Name");
                    while (rsst.Read())
                    {
                        li = new ListItem(DB.RSField(rsst, "Name"), DB.RSFieldInt(rsst, "CountryID").ToString());
                        dd.Items.Add(li);
                    }
                    rsst.Close();

                    //match selection
                    foreach (ListItem liC in dd.Items)
                    {
                        if (liC.Text.Equals(country))
                        {
                            dd.ClearSelection();
                            liC.Selected = true;
                            break;
                        }
                    }

                    ClientScript.RegisterStartupScript(this.GetType(), "goToScript", "<script type=\"text/javascript\">location.href = '#a" + myrow["stateID"].ToString() + "';</script>");
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
                deleteRowPerm(iden);
            }
        }

        protected void deleteRowPerm(int iden)
        {
            try
            {
                AppLogic.StateTaxRatesTable.Remove(iden);
                DB.ExecuteSQL("delete from State where StateID=" + iden.ToString());
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
                string name = ((TextBox)row.FindControl("txtName")).Text.Trim();
                string abbr = ((TextBox)row.FindControl("txtAbbreviation")).Text.Trim();
                int country = Localization.ParseNativeInt(((DropDownList)row.FindControl("ddCountry")).SelectedValue);
                int order = Localization.ParseNativeInt(((TextBox)row.FindControl("txtOrder")).Text.Trim());

                // see if already exists:
                int N = DB.GetSqlN("select count(Name) as N from State  " + DB.GetNoLock() + " where StateID<>" + iden + " and lower(Name)=" + DB.SQuote(name.ToLowerInvariant()));
                if (N != 0)
                {
                    this.resetError("There is already another state with that name.", true);
                    return;
                }

                StringBuilder sql = new StringBuilder(4096);

                sql.Append("update State set ");
                sql.Append("Name=" + DB.SQuote(name) + ",");
                sql.Append("CountryID=" + country + ",");
                sql.Append("DisplayOrder=" + order + ",");
                sql.Append("Abbreviation=" + DB.SQuote(CommonLogic.Left(abbr,5)));
                sql.Append(" where StateID=" + iden);

                try
                {
                    DB.ExecuteSQL(sql.ToString());
                    this.resetError("Item updated", false);
                    gMain.EditIndex = -1;
                    ViewState["SQLString"] = this.selectSQL;

                    //UpdateTaxOrder();
                    for (int i = 0; i <= Request.Form.Count - 1; i++)
                    {
                        //TR_CLASSID_STATEID
                        if (Request.Form.Keys[i].IndexOf("TR_") != -1)
                        {
                            String[] keys = Request.Form.Keys[i].Split('_');
                            int StateID = Localization.ParseUSInt(keys[2]);
                            int ClassID = Localization.ParseUSInt(keys[1]);
                            if (StateID == Localization.ParseUSInt(iden))
                            {
                                decimal tax = Decimal.Zero;
                                try
                                {
                                    tax = Localization.ParseNativeDecimal(Request.Form[Request.Form.Keys[i]]);
                                }
                                catch { }
                                StateTaxRate str = AppLogic.StateTaxRatesTable[StateID, ClassID];
                                try
                                {
                                    if (str == null)
                                    {
                                        AppLogic.StateTaxRatesTable.Add(StateID, ClassID, tax);
                                    }
                                    else
                                    {
                                        str.Update(tax);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string err = ex.Message;
                                }
                            }
                        }
                    }

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
                       
            txtName.Text = "";
            txtAbbr.Text = "";
            txtOrder.Text = "1";

            ddCountry.Items.Clear();
            ddCountry.ClearSelection();
            ListItem li = new ListItem(" - Select One -", "0");
            ddCountry.Items.Add(li);
            IDataReader rsst = DB.GetRS("select * from Country  " + DB.GetNoLock() + " order by DisplayOrder,Name");
            while (rsst.Read())
            {
                li = new ListItem(DB.RSField(rsst, "Name"), DB.RSFieldInt(rsst, "CountryID").ToString());
                ddCountry.Items.Add(li);
            }
            rsst.Close();

            rblShipping.SelectedIndex = 0;
        }

        protected void btnUpdateOrder_Click(object sender, EventArgs e)
        {
            UpdateTaxOrder();

            resetError("Display Order and Taxes updated.", false);
            this.buildGridData(this.buildGridData());
        }

        protected void UpdateTaxOrder()
        {
            for (int i = 0; i <= Request.Form.Count - 1; i++)
            {
                if (Request.Form.Keys[i].IndexOf("DisplayOrder_") != -1)
                {
                    String[] keys = Request.Form.Keys[i].Split('_');
                    int StateID = Localization.ParseUSInt(keys[1]);
                    int DispOrd = 1;
                    try
                    {
                        DispOrd = Localization.ParseUSInt(Request.Form[Request.Form.Keys[i]]);
                    }
                    catch { }
                    DB.ExecuteSQL("update State set DisplayOrder=" + DispOrd.ToString() + " where StateID=" + StateID.ToString());
                }
            }

            //handle taxes
            //DB.ExecuteSQL("DELETE FROM StateTaxRate");
            for (int i = 0; i <= Request.Form.Count - 1; i++)
            {
                //TR_CLASSID_STATEID
                if (Request.Form.Keys[i].IndexOf("TR_") != -1)
                {
                    String[] keys = Request.Form.Keys[i].Split('_');
                    int StateID = Localization.ParseUSInt(keys[2]);
                    int ClassID = Localization.ParseUSInt(keys[1]);
                    decimal tax = Decimal.Zero;
                    try
                    {
                        tax = Localization.ParseNativeDecimal(Request.Form[Request.Form.Keys[i]]);
                    }
                    catch { }
                    StateTaxRate ctr = AppLogic.StateTaxRatesTable[StateID, ClassID];
                    try
                    {
                        if (ctr == null)
                        {
                            AppLogic.StateTaxRatesTable.Add(StateID, ClassID, tax);
                        }
                        else
                        {
                            ctr.Update(tax);
                        }
                    }
                    catch (Exception ex)
                    {
                        string err = ex.Message;
                    }

                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            resetError("", false);
            StringBuilder sql = new StringBuilder();

            if (ValidInput())
            {
                string name = txtName.Text.Trim();
                string abbr = txtAbbr.Text.Trim();
                int country = Localization.ParseNativeInt(ddCountry.SelectedValue);
                int order = Localization.ParseNativeInt(txtOrder.Text.Trim());

                // ok to add them:
                String NewGUID = DB.GetNewGUID();
                sql.Append("insert into State(StateGUID,[Name],Abbreviation,CountryID,DisplayOrder) values(");
                sql.Append(DB.SQuote(NewGUID) + ",");
                sql.Append(DB.SQuote(name) + ",");
                sql.Append(DB.SQuote(abbr) + ",");
                sql.Append(country + ",");
                sql.Append(order + ")");

                try
                {
                    DB.ExecuteSQL(sql.ToString());
                    this.resetError("State added.", false);
                    ShowAddPanel(false);
                }
                catch
                {
                    this.resetError("State abbreviation already exists.", true);
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
