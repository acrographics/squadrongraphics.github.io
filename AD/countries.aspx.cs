// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/countries.aspx.cs 22    9/30/06 3:39p Redwoodtree $
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
    /// Summary description for countries.
    /// </summary>
    public partial class countries : System.Web.UI.Page
    {
        protected string selectSQL = "select * from Country  " + DB.GetNoLock();
        protected string defaultSort = "DisplayOrder,";
        protected Customer ThisCustomer;
        protected int RowBoundIndex = 1;

        private ArrayList TaxClassIDs = new ArrayList();

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
            RowBoundIndex = 1;

            //add tax class inputs
            IDataReader dr = DB.GetRS("SELECT * FROM TaxClass " + DB.GetNoLock() + " ORDER BY DisplayOrder, Name");
            while (dr.Read())
            {
                TaxClassIDs.Add(DB.RSFieldInt(dr, "TaxClassID"));
            }
            dr.Close();

            if (!IsPostBack)
            {


                DB.ExecuteSQL("delete from country where [Name]='NEW Country'");

                this.loadScript(false);
                ViewState["Sort"] = "Name";
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
            if (load)
            {
                if (AppLogic.NumLocaleSettingsInstalled() > 1)
                {
                    //this.ltScript.Text += CommonLogic.ReadFile("jscripts/tabs.js", true);
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
            sql += " order by " + defaultSort + " " + (ViewState["Sort"].ToString().ToLowerInvariant() == "displayorder" ? "Name" : ViewState["Sort"].ToString()) + " " + ViewState["SortOrder"].ToString();

            DataSet ds = DB.GetDS(sql, false);

            return ds;
        }

        protected void buildGridData(DataSet ds)
        {
            IDataReader dr = DB.GetRS("SELECT * FROM TaxClass ORDER BY DisplayOrder, Name");
            StringBuilder tmpS = new StringBuilder(1024);
            tmpS.Append("<div style=\"border-bottom: solid 1px #cccccc; text-align: center; width: 100%; float: left; font-weight: bold;\">Specify Tax Rates</div><div style=\"clear: both;\"></div>");
            int count = 0;
            while (dr.Read())
            {
                count++;
                tmpS.Append("<div style=\"width: (!W!); border-right: solid 1px white; float: left; white-space: wrap;\">" + XmlCommon.GetLocaleEntry(DB.RSField(dr, "Name"), ThisCustomer.LocaleSetting, false));
                tmpS.Append("</div>");
            }
            dr.Close();

            if (count > 0)
            {
                gMain.Columns[7].HeaderText = tmpS.ToString().Replace("(!W!)", ((100 / count) - 1) + "%");
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
                int iden = Localization.ParseNativeInt(myrow["CountryID"].ToString());

                ImageButton ib = (ImageButton)e.Row.FindControl("imgDelete");
                ib.Attributes.Add("onClick", "javascript: return confirm('Confirm Delete?')");

                //add tax class inputs
                StringBuilder tmpS = new StringBuilder(1024);
                Literal ltTaxRate = (Literal)e.Row.FindControl("ltTaxRate");
                for (int i = 0; i < TaxClassIDs.Count; i++)
                {
                    int classID = (int)TaxClassIDs[i];
                    string rate = Localization.CurrencyStringForDBWithoutExchangeRate(AppLogic.CountryTaxRatesTable.GetTaxRate(iden, classID));

                    /*
                    TextBox tb = new TextBox();
                    tb.ID = "TR_" + classID.ToString() + "_" + iden.ToString();
                    tb.Text = rate;
                    e.Row.Cells[7].Controls.Add(tb);
                    tb.Attributes.Add("class", "single4chars");
                    e.Row.Cells[7].Controls.Add(new LiteralControl("%"));
                    */
                    tmpS.Append("<div style=\"width: %W%; border-right: solid 1px white; float: left; white-space: wrap;\">");
                    tmpS.Append("<input type=\"text\" name='TR_" + classID + "_" + iden + "' class=\"single4chars\" value='" + rate + "'></input>%");
                    if (RowBoundIndex == 1)
                    {
                        tmpS.Append("&nbsp;<img href=\"javascript:void(0)\" onClick=\"CopyTaxDown(document.forms[0]," + classID.ToString() + ",'TR_" + classID.ToString() + "_" + iden.ToString() + "')\" style=\"cursor:hand; cursor:pointer;\" src=\"images/downarrow.gif\" border=\"0\" alt=\"Copy Rate Down\">");
                    }
                    tmpS.Append("</div>");
                    
                }
                RowBoundIndex++;

                if (TaxClassIDs.Count > 0)
                {
                    ltTaxRate.Text = tmpS.ToString().Replace("%W%", ((100 / TaxClassIDs.Count) - 1) + "%");
                    //ltTaxRate.Text = "";
                }
                else
                {
                    ltTaxRate.Text = "No Tax Classes";
                }

                //Click to edit
                if ((e.Row.RowState == DataControlRowState.Normal) || (e.Row.RowState == DataControlRowState.Alternate))
                {
                    e.Row.Attributes.Add("ondblclick", "javascript:__doPostBack('gMain','Edit$" + e.Row.RowIndex + "')");
                }

                if ((e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit)
                {
                    ClientScript.RegisterStartupScript(this.GetType(),"goToScript", "<script type=\"text/javascript\">location.href = '#a" + myrow["CountryID"].ToString() + "';</script>");
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
                AppLogic.CountryTaxRatesTable.Remove(iden);
                DB.ExecuteSQL("delete from Country where CountryID=" + iden.ToString());
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
                string twoLetter = ((TextBox)row.FindControl("txt2LetterIso")).Text.Trim();
                string threeLetter = ((TextBox)row.FindControl("txt3LetterIso")).Text.Trim();
                string numericISO = ((TextBox)row.FindControl("txtNumericISOCode")).Text.Trim();
                //int shipping = (Localization.ParseBoolean(((RadioButtonList)row.FindControl("rblTax")).SelectedValue) ? 1 : 0);
                int order = Localization.ParseNativeInt(((TextBox)row.FindControl("txtOrder")).Text.Trim());

                // see if already exists:
                int N = DB.GetSqlN("select count(Name) as N from Country  " + DB.GetNoLock() + " where CountryID<>" + iden + " and lower(Name)=" + DB.SQuote(name.ToLowerInvariant()));
                if (N != 0)
                {
                    this.resetError("There is already another country with that name.", true);
                    return;
                }

                StringBuilder sql = new StringBuilder(1024);

                sql.Append("update Country set ");
                sql.Append("Name=" + DB.SQuote(name) + ",");
                sql.Append("TwoLetterISOCode=" + DB.SQuote(CommonLogic.Left(twoLetter, 2)) + ",");
                sql.Append("ThreeLetterISOCode=" + DB.SQuote(CommonLogic.Left(threeLetter, 3)) + ",");
                sql.Append("NumericISOCode=" + DB.SQuote(CommonLogic.Left(numericISO, 3)) + ",");
                sql.Append("DisplayOrder=" + order);
                sql.Append(" where CountryID=" + iden.ToString());

                //throw new Exception(sql.ToString());
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
                            int CountryID = Localization.ParseUSInt(keys[2]);
                            int ClassID = Localization.ParseUSInt(keys[1]);
                            decimal tax = Decimal.Zero;
                            if (CountryID == Localization.ParseUSInt(iden))
                            {
                                try
                                {
                                    tax = Localization.ParseNativeDecimal(Request.Form[Request.Form.Keys[i]]);
                                }
                                catch { }
                                CountryTaxRate ctr = AppLogic.CountryTaxRatesTable[CountryID, ClassID];
                                try
                                {
                                    if (ctr == null)
                                    {
                                        AppLogic.CountryTaxRatesTable.Add(CountryID, ClassID, tax);
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

            txt2ISO.Text = "";
            txt3ISO.Text = "";
            txtName.Text = "";
            txtNumericISO.Text = "";
            txtOrder.Text = "1";
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
                    int CountryID = Localization.ParseUSInt(keys[1]);
                    int DispOrd = 1;
                    try
                    {
                        DispOrd = Localization.ParseUSInt(Request.Form[Request.Form.Keys[i]]);
                    }
                    catch { }
                    DB.ExecuteSQL("update Country set DisplayOrder=" + DispOrd.ToString() + " where CountryID=" + CountryID.ToString());
                }
            }

            //handle taxes
            //DB.ExecuteSQL("DELETE FROM CountryTaxRate");
            for (int i = 0; i <= Request.Form.Count - 1; i++)
            {
                //TR_CLASSID_STATEID
                if (Request.Form.Keys[i].IndexOf("TR_") != -1)
                {
                    String[] keys = Request.Form.Keys[i].Split('_');
                    int CountryID = Localization.ParseUSInt(keys[2]);
                    int ClassID = Localization.ParseUSInt(keys[1]);
                    decimal tax = Decimal.Zero;
                    try
                    {
                        tax = Localization.ParseNativeDecimal(Request.Form[Request.Form.Keys[i]]);
                    }
                    catch { }
                    CountryTaxRate ctr = AppLogic.CountryTaxRatesTable[CountryID, ClassID];
                    try
                    {
                        if (ctr == null)
                        {
                            AppLogic.CountryTaxRatesTable.Add(CountryID, ClassID, tax);
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
                string twoLetter = txt2ISO.Text.Trim();
                string threeLetter = txt3ISO.Text.Trim();
                string numericISO = txtNumericISO.Text.Trim();
                int order = Localization.ParseNativeInt(txtOrder.Text.Trim());

                // ok to add them:
                String NewGUID = DB.GetNewGUID();
                sql.Append("insert into Country(CountryGUID,[Name],TwoLetterISOCode,ThreeLetterISOCode,NumericISOCode,DisplayOrder) values(");
                sql.Append(DB.SQuote(NewGUID) + ",");
                sql.Append(DB.SQuote(name) + ",");
                sql.Append(DB.SQuote(twoLetter) + ",");
                sql.Append(DB.SQuote(threeLetter) + ",");
                sql.Append(DB.SQuote(numericISO) + ",");
                sql.Append(order + ")");

                try
                {
                    DB.ExecuteSQL(sql.ToString());
                    this.resetError("Country added.", false);
                    ShowAddPanel(false);
                }
                catch
                {
                    this.resetError("Country already exists.", true);
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

