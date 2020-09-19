// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/gallery.aspx.cs 5     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using AspDotNetStorefrontCommon;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Web;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for gallery
    /// </summary>
    public partial class gallery : System.Web.UI.Page
    {
        protected string selectSQL = "select * from Gallery  " + DB.GetNoLock();
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
                DB.ExecuteSQL("DELETE FROM Gallery WHERE [Name] LIKE '%<locale name=\"en-US\">NEW Gallery</locale>%' OR [Name]='NEW Gallery'");

                loadScript(false);
                ViewState["Sort"] = "DisplayOrder,Name";
                ViewState["SortOrder"] = "ASC";
                ViewState["SQLString"] = selectSQL;

                ShowAddPanel(false);
            }
        }

        protected void loadScript(bool load)
        {
            if (load)
            {
                if (AppLogic.NumLocaleSettingsInstalled() > 1)
                {
                    ltScript.Text += CommonLogic.ReadFile("jscripts/tabs.js", true);
                }
            }
            else
            {
                ltScript.Text = "";
            }
            ltScript2.Text = ("<script type=\"text/javascript\">\n");
            ltScript2.Text += ("function DeleteImage(imgurl,name)\n");
            ltScript2.Text += ("{\n");
            ltScript2.Text += ("window.open('deleteimage.aspx?imgurl=' + imgurl + '&FormImageName=' + name,\"AspDotNetStorefrontAdmin_ML\",\"height=250,width=440,top=10,left=20,status=no,toolbar=no,menubar=no,scrollbars=yes,location=no\")\n");
            ltScript2.Text += ("}\n");
            ltScript2.Text += ("</SCRIPT>\n");
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
                    dr["EditName"] = AppLogic.GetLocaleEntryFields(dr[2].ToString(), "Name", false, true, true, "Please enter the " + AppLogic.GetString("AppConfig.CategoryPromptSingular", 0, cust.LocaleSetting).ToLower() + " name", 100, 25, 0, 0, false);
                }

                ds.Tables[0].Columns.Add("EditDescription");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dr["EditDescription"] = AppLogic.GetLocaleEntryFields(dr[4].ToString(), "Description", true, true, true, "Please enter the " + AppLogic.GetString("AppConfig.CategoryPromptSingular", 0, cust.LocaleSetting).ToLower() + " description", 100, 25, 0, 0, false);
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

            ((Literal)Form.FindControl("ltError")).Text = str;
        }

        protected void gMain_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {            
            ViewState["SQLString"] = selectSQL;

            ViewState["FirstEntry"] = "0";

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

                //Click to edit
                if ((e.Row.RowState == DataControlRowState.Normal) || (e.Row.RowState == DataControlRowState.Alternate))
                {
                    e.Row.Attributes.Add("ondblclick", "javascript:__doPostBack('gMain','Edit$" + e.Row.RowIndex + "')");
                }

                //Localization
                if ((e.Row.RowState == DataControlRowState.Normal) || (e.Row.RowState == DataControlRowState.Alternate))
                {
                    e.Row.Cells[3].Text = XmlCommon.GetLocaleEntry(((Literal)e.Row.FindControl("ltName")).Text, cust.LocaleSetting, false);
                    e.Row.Cells[4].Text = XmlCommon.GetLocaleEntry(((Literal)e.Row.FindControl("ltDescription")).Text, cust.LocaleSetting, false);
                }

                if ((e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit)
                {
                    DataRowView myrow = (DataRowView)e.Row.DataItem;

                    Literal ltDirectory = (Literal)e.Row.FindControl("ltDirectory");
                    TextBox txtDirectory = (TextBox)e.Row.FindControl("txtDirectory");
                    RequiredFieldValidator rfvDirectory = (RequiredFieldValidator)e.Row.FindControl("rfvDirectory");

                    try
                    {
                        if (Localization.ParseNativeInt(ViewState["FirstEntry"].ToString()) == 1)
                        {
                            txtDirectory.Visible = true;
                            rfvDirectory.Visible = true;
                            ltDirectory.Visible = false;
                        }
                        else
                        {
                            txtDirectory.Visible = false;
                            rfvDirectory.Visible = false;
                            ltDirectory.Visible = true;
                        }
                    }
                    catch
                    {
                        txtDirectory.Visible = false;
                        rfvDirectory.Visible = false;
                        ltDirectory.Visible = true;
                    }
                }
                else
                {
                    //Image
                    String Image1URL = AppLogic.LookupImage("Gallery", iden, "icon", 1, cust.LocaleSetting);
                    Literal ltImage = (Literal)e.Row.FindControl("ltImage");
                    ltImage.Text = "";
                    if (Image1URL.Length != 0)
                    {
                        if (Image1URL.IndexOf("nopicture") == -1)
                        {
                            ltImage.Text = ("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image1URL + "','GalleryPic');\">Delete Image</a><br/>\n");
                        }
                        ltImage.Text += ("<img id=\"GalleryPic\" name=\"GalleryPic\" height=\"35\" border=\"0\" src=\"" + Image1URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\"/>\n");
                    }
                }

                //URL
                ((Literal)e.Row.FindControl("ltURL")).Text = "<a target=\"_blank\" href=\"../showgallery.aspx?galleryid=" + iden + "\">showgallery.aspx?galleryid=" + iden + "</a>";
            }            
        }
        protected void gMain_Sorting(object sender, GridViewSortEventArgs e)
        {
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
            // delete any images:
            try
            {
                System.IO.File.Delete(AppLogic.GetImagePath("Gallery", "icon", true) + iden + ".jpg");
                System.IO.File.Delete(AppLogic.GetImagePath("Gallery", "icon", true) + iden + ".png");
                System.IO.File.Delete(AppLogic.GetImagePath("Gallery", "icon", true) + iden + ".gif");
            }
            catch { }

            // delete the gallery directory also!
            String GalleryDirName = AppLogic.GetGalleryDir(iden);
            String SFP = CommonLogic.SafeMapPath("../images/spacer.gif").Replace("images\\spacer.gif", "images\\gallery") + "\\" + GalleryDirName;
            try
            {
                if (Directory.Exists(SFP))
                {
                    String[] files = Directory.GetFiles(SFP, "*.*");
                    foreach (String file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                    Directory.Delete(SFP);
                }
            }
            catch { }

            // delete the gallery:
            DB.ExecuteSQL("delete from gallery where GalleryID=" + iden);

            buildGridData(buildGridData());
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
                int iden = Localization.ParseNativeInt(row.Cells[1].Text.ToString());
                string name = AppLogic.FormLocaleXml("Name");
                FileUpload fu = (FileUpload) row.FindControl("fuMain");
                
                StringBuilder sql = new StringBuilder();

                sql.Append("update Gallery set ");
                sql.Append("Name=" + DB.SQuote(name) + ",");

                if (AppLogic.FormLocaleXml("Description").Length != 0)
                {
                    sql.Append("Description=" + DB.SQuote(AppLogic.FormLocaleXml("Description")));
                }
                else
                {
                    sql.Append("Description=NULL");
                }
                sql.Append(" where GalleryID=" + iden.ToString());

                try
                {
                    DB.ExecuteSQL(sql.ToString());
                    UploadImage(iden, fu);

                    resetError("Item updated", false);
                    gMain.EditIndex = -1;
                    ViewState["SQLString"] = selectSQL;
                    buildGridData(buildGridData());
                }
                catch (Exception ex)
                {
                    throw new Exception("Couldn't update database: " + sql.ToString() + ex.ToString());
                }
            }
        }

        protected void UploadImage(int iden, FileUpload fu)
        {
            // handle image uploaded:
            String Image1 = String.Empty;
            HttpPostedFile Image1File = fu.PostedFile;
            if (Image1File.ContentLength != 0)
            {
                // delete any current image file first
                try
                {
                    System.IO.File.Delete(AppLogic.GetImagePath("Gallery", "icon", true) + iden + ".jpg");
                    System.IO.File.Delete(AppLogic.GetImagePath("Gallery", "icon", true) + iden + ".gif");
                    System.IO.File.Delete(AppLogic.GetImagePath("Gallery", "icon", true) + iden + ".png");
                }
                catch
                { }

                String s = Image1File.ContentType;
                switch (Image1File.ContentType)
                {
                    case "image/gif":
                        Image1 = AppLogic.GetImagePath("Gallery", "icon", true) + iden + ".gif";
                        Image1File.SaveAs(Image1);
                        break;
                    case "image/x-png":
                        Image1 = AppLogic.GetImagePath("Gallery", "icon", true) + iden + ".png";
                        Image1File.SaveAs(Image1);
                        break;
                    case "image/jpg":
                    case "image/jpeg":
                    case "image/pjpeg":
                        Image1 = AppLogic.GetImagePath("Gallery", "icon", true) + iden + ".jpg";
                        Image1File.SaveAs(Image1);
                        break;
                }
            }
        }

        protected void gMain_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gMain.EditIndex = e.NewEditIndex;

            loadScript(true);

            buildGridData(buildGridData());
        }
        //protected void btnInsert_Click(object sender, EventArgs e)
        //{
        //    ViewState["IsInsert"] = false;
        //    gMain.EditIndex = -1;
        //    StringBuilder sql = new StringBuilder(2500);

        //    // see if this name is already there:
        //    string name = "NEW Gallery";

        //    // ok to add them:
        //    String NewGUID = DB.GetNewGUID();
        //    sql.Append("insert into gallery(GalleryGUID,Name,DirName) values(");
        //    sql.Append(DB.SQuote(NewGUID) + ",");
        //    sql.Append(DB.SQuote(name) + ",");
        //    sql.Append("''");
        //    sql.Append(")");

        //    try
        //    {
        //        DB.ExecuteSQL(sql.ToString());
        //        AppLogic.ClearCache();
        //        ViewState["SQLString"] = selectSQL + " WHERE [Name]='" + name + "' ";
        //        ViewState["Sort"] = "GalleryID";
        //        ViewState["SortOrder"] = "DESC";

        //        ViewState["FirstEntry"] = 1;

        //        gMain.EditIndex = 0;
        //        loadScript(true);
        //        buildGridData(buildGridData());
        //        resetError("New Gallery added.", false);
        //        ViewState["IsInsert"] = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Couldn't update database: " + ex.ToString());
        //    }
        //}
        protected void btnOrder_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= Request.Form.Count - 1; i++)
            {
                if (Request.Form.Keys[i].IndexOf("DisplayOrder_") != -1)
                {
                    String[] keys = Request.Form.Keys[i].Split('_');
                    int GalleryID = Localization.ParseUSInt(keys[1]);
                    int DispOrd = 1;
                    try
                    {
                        DispOrd = Localization.ParseUSInt(Request.Form[Request.Form.Keys[i]]);
                    }
                    catch { }
                    DB.ExecuteSQL("update gallery set DisplayOrder=" + DispOrd.ToString() + " where GalleryID=" + GalleryID.ToString());
                }
            }
            buildGridData(buildGridData());
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            resetError("", false);

            gMain.EditIndex = -1;
            ShowAddPanel(true);

            txtOrder.Text = "1";
            txtDirectory.Text = "";
            ltName.Text = AppLogic.GetLocaleEntryFields("", "Name", false, true, true, "Please enter the Name", 100, 25, 0, 0, false);
            ltDescription.Text = AppLogic.GetLocaleEntryFields("", "Description", true, true, false, "Please enter the Description", 0, 0, 5, 80, false);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            resetError("", false);
            StringBuilder sql = new StringBuilder();

            if (ValidInput())
            {
                string name = AppLogic.FormLocaleXml("Name");
                string description = AppLogic.FormLocaleXml("Description");
                string order = txtOrder.Text.Trim();
                string dir = txtDirectory.Text.Trim();

                // ok to add them:
                String NewGUID = DB.GetNewGUID();
                sql.Append("insert into gallery(GalleryGUID,Name,Description,DirName,DisplayOrder) values(");
                sql.Append(DB.SQuote(NewGUID) + ",");
                sql.Append(DB.SQuote(name) + ",");
                sql.Append(DB.SQuote(description) + ",");
                
                //directory name
                sql.Append(DB.SQuote(dir) + ",");
                sql.Append(order); 
                sql.Append(")");
                        
                try
                {
                    DB.ExecuteSQL(sql.ToString());
                     
                    int iden = DB.GetSqlN("SELECT GalleryID AS N FROM Gallery WHERE GalleryGUID=" + DB.SQuote(NewGUID));
                    //create dir
                    String SFP = CommonLogic.SafeMapPath("../images/spacer.gif").Replace("images\\spacer.gif", "images\\gallery") + "\\" + AppLogic.GetGalleryDir(iden);
                    try
                    {
                        if (!Directory.Exists(SFP))
                        {
                            DirectoryInfo di = Directory.CreateDirectory(SFP);
                        }

                    }
                    catch { }

                    //upload Image
                    UploadImage(iden, fuMain);

                    this.resetError("Gallery added.", false);
                    ShowAddPanel(false);
                }
                catch
                {
                    this.resetError("Gallery already exists.", true);
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