// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/taxzips.aspx.cs 11    9/30/06 3:38p Redwoodtree $
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
    /// Summary description for taxzips.
    /// </summary>
    public partial class taxzips : System.Web.UI.Page
    {
        protected string selectSQL = "select distinct ZipCode from ZipTaxRate  " + DB.GetNoLock();
        protected string defaultSort = "";
        protected Customer cust;
        int RowBoundIndex = 1;

        private ArrayList TaxClassIDs = new ArrayList();


        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            //add tax class inputs
            IDataReader dr = DB.GetRS("SELECT * FROM TaxClass " + DB.GetNoLock() + " ORDER BY DisplayOrder, Name");
            while (dr.Read())
            {
                TaxClassIDs.Add(DB.RSFieldInt(dr, "TaxClassID"));
            }
            dr.Close();


            if (!IsPostBack)
            {
                ShowAddPanel(false);

                this.loadScript(false);
                ViewState["Sort"] = "ZipCode";
                ViewState["SortOrder"] = "ASC";
                ViewState["SQLString"] = this.selectSQL;

                this.buildGridData(this.buildGridData());
            }

            RowBoundIndex = 1;

            ClientScriptManager cs = Page.ClientScript;

            StringBuilder tmpS = new StringBuilder(1024);
            tmpS.Append("function CopyTaxDown(theForm,TaxClassID,InitialFormField)\n");
            tmpS.Append("{\n");
            tmpS.Append("   var pat = 'TR_' + TaxClassID;\n");
            tmpS.Append("	for (i = 0; i < theForm.length; i++)\n");
            tmpS.Append("	{\n");
            tmpS.Append("		var str = theForm.elements[i].name;\n");
            tmpS.Append("       if(str.substring(0,pat.length) == pat)\n");
            tmpS.Append("       {\n");
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
            sql += " order by " + defaultSort + " " + ViewState["Sort"].ToString() + " " + ViewState["SortOrder"].ToString();

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
                temp += "<div style=\"width: %W%; border-right: solid 1px white; float: left; white-space: wrap;\">" + XmlCommon.GetLocaleEntry(DB.RSField(dr, "Name"), cust.LocaleSetting, false) + "</div>";
            }
            dr.Close();

            if (count > 0)
            {
                gMain.Columns[2].HeaderText = temp.Replace("%W%", ((100 / count) - 1) + "%");
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

        protected void gMain_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView myrow = (DataRowView)e.Row.DataItem;
                string ZipCode = myrow["ZipCode"].ToString();

                ImageButton ib = (ImageButton)e.Row.FindControl("imgDelete");
                ib.Attributes.Add("onClick", "javascript: return confirm('Confirm Delete?')");

                Literal ltTaxRate = (Literal)e.Row.FindControl("ltTaxRate");

                StringBuilder tmpS = new StringBuilder(1024);
                for (int i = 0; i < TaxClassIDs.Count; i++)
                {
                    int classID = (int)TaxClassIDs[i];
                    string rate = Localization.CurrencyStringForDBWithoutExchangeRate(AppLogic.ZipTaxRatesTable.GetTaxRate(ZipCode, classID));
                    tmpS.Append("<div style=\"width: (!W!); border-right: solid 1px white; float: left; white-space: wrap;\">");
                    tmpS.Append("<input type=\"text\" name='TR_" + classID.ToString() + "_" + ZipCode + "' class=\"single4chars\" value='" + rate + "'></input>%");
                    if (RowBoundIndex == 1)
                    {
                        tmpS.Append("&nbsp;<img href=\"javascript:void(0)\" onClick=\"CopyTaxDown(document.forms[0]," + classID.ToString() + ",'TR_" + classID.ToString() + "_" + ZipCode + "')\" style=\"cursor:hand; cursor:pointer;\" src=\"images/downarrow.gif\" border=\"0\" alt=\"Copy Rate Down\">");
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

                    ClientScript.RegisterStartupScript(this.GetType(), "goToScript", "<script type=\"text/javascript\">location.href = '#a" + myrow["ZipCode"].ToString() + "';</script>");
                    //set the original zip for editing
                    ViewState["OriginalZip"] = myrow["ZipCode"].ToString();
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
                string iden = e.CommandArgument.ToString();
                deleteRowPerm(iden);
            }
        }

        protected void deleteRowPerm(string iden)
        {
            StringBuilder sql = new StringBuilder(2500);
            sql.Append("delete from ZipTaxRate where ZipCode=" + DB.SQuote(iden));
            try
            {
                DB.ExecuteSQL(sql.ToString());
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

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            resetError("", false);
            gMain.EditIndex = -1;

            txtTax.Text = "";
            txtZip.Text = "";

            ShowAddPanel(true);
        }

        protected void gMain_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            //if new item and cancel, must delete
            DB.ExecuteSQL("DELETE FROM ZipTaxRate WHERE ZipCode='ZIP'");

            ViewState["SQLString"] = this.selectSQL;

            gMain.EditIndex = -1;
            this.buildGridData(this.buildGridData());
        }

        protected void gMain_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gMain.Rows[e.RowIndex];

            if (row != null)
            {
                string orig = ViewState["OriginalZip"].ToString();
                string zip = (((TextBox)row.FindControl("txtZip")).Text.Trim()).ToString();
                //decimal tax = Localization.ParseNativeDecimal(((TextBox)row.FindControl("txtTax")).Text.Trim());

                StringBuilder sql = new StringBuilder(1024);

                //make sure no duplicates
                if (!orig.Equals(zip))
                {
                    int count = DB.GetSqlN("SELECT count(*) AS N FROM ZipTaxRate WHERE ZipCode = " + DB.SQuote(zip));
                    if (count > 0)
                    {
                        resetError("Duplicate Zip Code exists", true);
                        return;
                    }
                }

                //sql.Append("update ZipTaxRate set ");
                //sql.Append("ZipCode=" + DB.SQuote(zip) + ",");
                //sql.Append("TaxRate=" + tax);
                //sql.Append(" where ZipCode=" + DB.SQuote(orig));

                try
                {
                    //DB.ExecuteSQL(sql.ToString());
                    this.resetError("Item updated", false);
                    gMain.EditIndex = -1;
                    ViewState["SQLString"] = this.selectSQL;

                    //DB.ExecuteSQL("DELETE FROM ZipTaxRate WHERE ZipCode='ZIP'");
                    for (int i = 0; i <= Request.Form.Count - 1; i++)
                    {
                        //TR_CLASSID_ZipCode
                        if (Request.Form.Keys[i].IndexOf("TR_") != -1)
                        {
                            String[] keys = Request.Form.Keys[i].Split('_');
                            string ZipCode = keys[2];
                            int ClassID = Localization.ParseUSInt(keys[1]);
                            decimal taxrate = Decimal.Zero;
                            if (ZipCode == zip)
                            {
                                try
                                {
                                    taxrate = Localization.ParseNativeDecimal(Request.Form[Request.Form.Keys[i]]);
                                }
                                catch { }
                                ZipTaxRate ztr = AppLogic.ZipTaxRatesTable[ZipCode, ClassID];
                                try
                                {
                                    if (ztr == null)
                                    {
                                        AppLogic.ZipTaxRatesTable.Add(ZipCode, ClassID, taxrate);
                                    }
                                    else
                                    {
                                        ztr.Update(taxrate);
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

        protected void btnUpdateOrder_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= Request.Form.Count - 1; i++)
            {
                if (Request.Form.Keys[i].IndexOf("TaxRate_") != -1)
                {
                    String[] keys = Request.Form.Keys[i].Split('_');
                    String ZipCode = keys[1];
                    Decimal TaxRate = System.Decimal.Zero;
                    try
                    {
                        if (CommonLogic.FormCanBeDangerousContent("TaxRate_" + ZipCode).Length != 0)
                        {
                            TaxRate = Localization.ParseUSDecimal(CommonLogic.FormCanBeDangerousContent("TaxRate_" + ZipCode));
                        }
                        DB.ExecuteSQL("update ZipTaxRate set TaxRate=" + CommonLogic.IIF(TaxRate != 0.0M, Localization.DecimalStringForDB(TaxRate), "NULL") + " where ZipCode=" + DB.SQuote(ZipCode));
                    }
                    catch { }
                }
            }

            DB.ExecuteSQL("delete from ZipTaxRate where TaxRate IS NULL or TaxRate=0.0");

            resetError("Tax Rates updated.", false);
            this.buildGridData(this.buildGridData());
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            resetError("", false);
            StringBuilder sql = new StringBuilder(2500);

            string zip = txtZip.Text.Trim();
            decimal tax = Localization.ParseNativeDecimal(txtTax.Text.Trim());

            // ok to add them:
            ZipTaxRate ztr = AppLogic.ZipTaxRatesTable[CommonLogic.Left(zip, 5), AppLogic.AppConfigNativeInt("Admin_DefaultTaxClassID ")];
            if (ztr == null)
            {
                AppLogic.ZipTaxRatesTable.Add(CommonLogic.Left(zip, 5), AppLogic.AppConfigNativeInt("Admin_DefaultTaxClassID "), tax);
                this.resetError("Zip Code added.", false);
            }
            else
            {
                ztr.Update(tax);
                this.resetError("Zip Code already exists and was updated.", false);
            }
            this.buildGridData(this.buildGridData());

            ShowAddPanel(false);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            resetError("", false);

            ShowAddPanel(false);
        }

        protected void ShowAddPanel(bool showAdd)
        {
            if (showAdd)
            {
                pnlAdd.Visible = true;
                pnlGrid.Visible = false;
            }
            else
            {
                pnlAdd.Visible = false;
                pnlGrid.Visible = true;
            }
        }
    }
}
