// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/partners.aspx.cs 7     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.IO;
using AspDotNetStorefrontCommon;
using System.Web.UI.WebControls;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for partners.
    /// </summary>
    public partial class partners : System.Web.UI.Page
    {
        protected string selectSQL = "select * from Partner " + DB.GetNoLock() + " WHERE deleted=0";
        protected Customer cust;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            if (!IsPostBack)
            {
                loadScript(false);
                ViewState["Sort"] = "DisplayOrder,Name";
                ViewState["SortOrder"] = "ASC";
                ViewState["SQLString"] = selectSQL;

                buildGridData(buildGridData());
            }
        }

        protected void loadScript(bool load)
        {
            if (load)
            {
                if (AppLogic.NumLocaleSettingsInstalled() > 1)
                {
                    ltScript.Text += "";
                }
            }
            else
            {
                ltScript.Text = "";
            }
            ltScript2.Text = "";
        }

        protected DataSet buildGridData()
        {
            string sql = ViewState["SQLString"].ToString();
            sql += " order by " + ViewState["Sort"].ToString() + " " + ViewState["SortOrder"].ToString();

            DataSet ds = DB.GetDS(sql, false);

            return ds;
        }

        protected void buildGridData(DataSet ds)
        {
            gMain.DataSource = ds;
            gMain.DataBind();
            ds.Dispose();
        }

        protected void gMain_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gMain.EditIndex = e.NewEditIndex;
            this.loadScript(true);
            this.buildGridData(this.buildGridData());
        }

        protected void gMain_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gMain.Rows[e.RowIndex];

            if (row != null)
            {
                string iden = row.Cells[1].Text.ToString();
                string name = ((TextBox)row.FindControl("txtName")).Text;

                StringBuilder sql = new StringBuilder(2500);

                sql.Append("update Partner set ");
                sql.Append("Name=" + DB.SQuote(name));
                sql.Append(" where PartnerID=" + iden);

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
            //if new item and cancel, must delete
            //if (Localization.ParseBoolean(ViewState["IsInsert"].ToString()))
            //{
            //    GridViewRow row = gMain.Rows[e.RowIndex];
            //    if (row != null)
            //    {
            //        int iden = Localization.ParseNativeInt(row.Cells[1].Text.ToString());
            //        deleteRow(iden);
            //    }
            //}

            ViewState["IsInsert"] = false;
            ViewState["SQLString"] = selectSQL;

            gMain.EditIndex = -1;
            buildGridData(buildGridData());
        }

        protected void gMain_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton ib = (ImageButton)e.Row.FindControl("imgDelete");
                ib.Attributes.Add("onClick", "javascript: return confirm('Confirm Delete?')");
                int iden = Localization.ParseNativeInt(e.Row.Cells[1].Text);

                //Localization
                //e.Row.Cells[3].Text = XmlCommon.GetLocaleEntry(((Literal)e.Row.FindControl("ltName")).Text, cust.LocaleSetting, false);

                //Image
                String Image1URL = AppLogic.LookupImage("Partner", iden, "icon", 1, cust.LocaleSetting);
                Literal ltImage = (Literal)e.Row.FindControl("ltImage");
                ltImage.Text = "";
                if (Image1URL.Length != 0)
                {
                    ltImage.Text += ("<img id=\"GalleryPic\" name=\"PartnerPic\" height=\"35\" border=\"0\" src=\"" + Image1URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\"/>\n");
                }
            }
        }
        protected void gMain_Sorting(object sender, GridViewSortEventArgs e)
        {
            ViewState["IsInsert"] = false;
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
                ViewState["IsInsert"] = false;
                gMain.EditIndex = -1;
                int iden = Localization.ParseNativeInt(e.CommandArgument.ToString());
                deleteRow(iden);
            }
        }

        protected void deleteRow(int iden)
        {
            // delete the mfg:
            DB.ExecuteSQL("update Partner set deleted=1 where PartnerID=" + iden);
            // delete any images:
            try
            {
                System.IO.File.Delete(AppLogic.GetImagePath("Partner", "icon", true) + iden + ".jpg");
                System.IO.File.Delete(AppLogic.GetImagePath("Partner", "icon", true) + iden + ".gif");
                System.IO.File.Delete(AppLogic.GetImagePath("Partner", "icon", true) + iden + ".png");
            }
            catch { }

            buildGridData(buildGridData());
        }

        protected void gMain_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["IsInsert"] = false;
            resetError("", false);
            gMain.PageIndex = e.NewPageIndex;
            gMain.EditIndex = -1;
            buildGridData(buildGridData());
        }
       
        protected void btnOrder_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= Request.Form.Count - 1; i++)
            {
                if (Request.Form.Keys[i].IndexOf("DisplayOrder_") != -1)
                {
                    String[] keys = Request.Form.Keys[i].Split('_');
                    int PartnerID = Localization.ParseUSInt(keys[1]);
                    int DispOrd = 1;
                    try
                    {
                        DispOrd = Localization.ParseUSInt(Request.Form[Request.Form.Keys[i]]);
                    }
                    catch { }
                    DB.ExecuteSQL("update Partner set DisplayOrder=" + DispOrd.ToString() + " where PartnerID=" + PartnerID.ToString());
                }
            }
            buildGridData(buildGridData());
        }
        protected void btnInsert_Click(object sender, EventArgs e)
        {
            Response.Redirect("editPartner.aspx?partnerID=0");
        }

    }
}
