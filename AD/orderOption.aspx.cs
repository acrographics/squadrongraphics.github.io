// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/orderOption.aspx.cs 5     9/30/06 3:38p Redwoodtree $
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
using System.Globalization;
using System.IO;
using AspDotNetStorefrontCommon;

public partial class orderOption : System.Web.UI.Page
{
    protected Customer ThisCustomer;

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");

        ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
        lblMsg.Text = string.Empty;

        if (!IsPostBack)
        {
            ViewState["EditingOrderOptions"] = false;
            ViewState["EditingOrderOptionsID"] = "0";

            loadTree();
            loadScript(false);
            resetForm();
            phMain.Visible = false;

            btnDelete.Attributes.Add("onClick", "return confirm('Are you sure you want to delete this Order Option?');");
        }
    }

    private void loadTree()
    {
        try
        {
            treeMain.Nodes.Clear();

            //Order Options            
            string sql = "select * from OrderOption " + DB.GetNoLock() + " order by DisplayOrder,Name";
            DataSet ds = DB.GetDS(sql, false);

            treeMain.Nodes.Add(new TreeNode("[Display Order] <b>Options</b> ", "-1"));
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                TreeNode myNode = new TreeNode();
                myNode.Text = "<input size=2 type=\"text\" class=\"default\" name=\"DisplayOrder_" + DB.RowFieldInt(row, "OrderOptionID").ToString() + "\" value=\"" + DB.RowFieldInt(row, "DisplayOrder").ToString() + "\">&nbsp;" + DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting);
                myNode.Value = DB.RowFieldInt(row, "OrderOptionID").ToString();
                myNode.ImageUrl = "icons/dot.gif";
                
                treeMain.Nodes.Add(myNode);
            }
            ds.Dispose();
        }
        catch (Exception ex)
        {
            resetError(ex.ToString(), true);
        }
    }

    protected void treeMain_SelectedNodeChanged(object sender, EventArgs e)
    {
        ltValid.Text = "";

        if (!treeMain.SelectedValue.Equals("-1"))
        {
            resetForm();
            ViewState["EditingOrderOptions"] = true;
            loadScript(true);

            resetError("", false);
            phMain.Visible = true;

            string index = treeMain.SelectedNode.Value;
            ViewState["EditingOrderOptionsID"] = index;

            getOrderOptionsDetails();
            return;
        }

        phMain.Visible = false;
        resetForm();
        loadTree();
        loadScript(false);
    }

    private void HookUpClientSideValidation()
    {
        StringBuilder script = new StringBuilder();
        script.Append("<script type=\"text/javascript\"  language=\"javascript\" >\n");
        script.Append("function validate(){\n");
        script.Append("var exp = new RegExp(/^\\d*(\\.\\d{1,2})?$/);\n");
        script.Append("var cost = document.getElementById('" + this.txtCost.ClientID + "');\n");
        script.Append("if(cost.value == null || cost.value == '') cost.value = 0.0;\n");
        script.Append("if(!cost.value.match(exp))\n");
        script.Append("{\n alert('invalid number format'); \n return false;\n}");
        //script.Append("alert(document.getElementById('" + this.txtCost.ClientID + "').value; return false;");
        //script.Append("alert(document.getElementById('" + this.txtCost.ClientID + @"').value.test('^\d*(\.\d{1,2})?$')); return false;");

        // validate cost format..
        //script.Append("if(!document.getElementById('" + this.txtCost.ClientID + @"').value.test(/^\d*(\.\d{1,2})?$/)) alert('invalid number format');");
        script.Append("return true;\n");
        script.Append("}\n");
        script.Append("</script>\n");

        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
    }


    protected void getOrderOptionsDetails()
    {
        HookUpClientSideValidation();

        string iden = ViewState["EditingOrderOptionsID"].ToString();

        IDataReader rs = DB.GetRS("select * from OrderOption  " + DB.GetNoLock() + " where OrderOptionID=" + iden);
        if (!rs.Read())
        {
            rs.Close();
            resetError("Unable to retrieve data.", true);
            return;
        }

        //editing OrderOptions
        bool Editing = Localization.ParseBoolean(ViewState["EditingOrderOptions"].ToString());
        ltMode.Text = "Editing";
        btnSubmit.Text = "Update Order Option";
        btnDelete.Visible = true;

        ltName.Text = AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Name"), "Name", false, true, true, "Please enter the topic name", 100, 30, 0, 0, false);
        //ltDescription.Text = AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Description"), "Description", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true);
        radDescription.Html = DB.RSFieldByLocale(rs, "Description", ThisCustomer.LocaleSetting);
        
        rbIsChecked.SelectedIndex = CommonLogic.IIF(DB.RSFieldBool(rs, "DefaultIsChecked"), 1, 0);

        ddTaxClass.Items.Clear();
        IDataReader rsst = DB.GetRS("select * from TaxClass  " + DB.GetNoLock() + " order by DisplayOrder,Name");
        while (rsst.Read())
        {
            ddTaxClass.Items.Add(new ListItem(DB.RSFieldByLocale(rsst, "Name", ThisCustomer.LocaleSetting), DB.RSFieldInt(rsst, "TaxClassID").ToString()));
        }
        rsst.Close();
        ddTaxClass.Items.FindByValue(DB.RSFieldInt(rs, "TaxClassID").ToString()).Selected = true;
        txtCost.Text = CommonLogic.IIF(DB.RSFieldDecimal(rs, "Cost") != System.Decimal.Zero, Localization.CurrencyStringForDBWithoutExchangeRate(DB.RSFieldDecimal(rs, "Cost")), "");
        
        //IMAGE
        ltIcon.Text = "";
        String Image1URL = AppLogic.LookupImage("OrderOption", Localization.ParseNativeInt(iden), "icon", 1, ThisCustomer.LocaleSetting);
        if (Image1URL.Length != 0)
        {
            if (Image1URL.IndexOf("nopicture") == -1)
            {
                ltIcon.Text = ("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image1URL + "','OrderPic');\">Click here</a> to delete the current image<br/>\n");
            }
            ltIcon.Text += ("<br/><img id=\"OrderPic\" name=\"OrderPic\" border=\"0\" src=\"" + Image1URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\">\n");
        }
        rs.Close();

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
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        ltValid.Text = "";

        ViewState["EditingOrderOptions"] = false;
        ViewState["EditingOrderOptionsID"] = "0";
        loadScript(true);
        resetForm();
        phMain.Visible = true;
        btnDelete.Visible = false;
        loadTree();
        //new OrderOptions
        ltMode.Text = "Adding an";
        btnSubmit.Text = "Add Order Option";
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
            ltStyles.Text = "";
        }

        ltScript2.Text = ("<script type=\"text/javascript\">\n");
        ltScript2.Text += ("function DeleteImage(imgurl,name)\n");
        ltScript2.Text += ("{\n");
        ltScript2.Text += ("window.open('deleteimage.aspx?imgurl=' + imgurl + '&FormImageName=' + name,\"AspDotNetStorefrontAdmin_ML\",\"height=250,width=440,top=10,left=20,status=no,toolbar=no,menubar=no,scrollbars=yes,location=no\")\n");
        ltScript2.Text += ("}\n");
        ltScript2.Text += ("</SCRIPT>\n");
    }

    protected bool validateForm()
    {
        bool valid = true;
        string temp = "";
        //string focus = "";
       
        if ((AppLogic.FormLocaleXml("Name").Length == 0) || (AppLogic.FormLocaleXml("Name").Equals("<ml></ml>")))
        {
            valid = false;
            temp += "Please fill out the Name";
            //focus = "Name";
        }

        return valid;
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        updateOrderOptions();
    }

    protected void updateOrderOptions()
    {
        ltValid.Text = "";

        if (validateForm())
        {
            StringBuilder sql = new StringBuilder();
            decimal Cost = System.Decimal.Zero;
            if (txtCost.Text.Length != 0)
            {
                Cost = Localization.ParseNativeDecimal(txtCost.Text);
            }

            bool Editing = Localization.ParseBoolean(ViewState["EditingOrderOptions"].ToString());
            int OrderOptionID = Localization.ParseNativeInt(ViewState["EditingOrderOptionsID"].ToString());
            IDataReader rs;

            if (!Editing)
            {
                // ok to add them:
                String NewGUID = DB.GetNewGUID();
                sql.Append("insert into OrderOption(OrderOptionGUID,Name,Description,DefaultIsChecked,TaxClassID,Cost) values(");
                sql.Append(DB.SQuote(NewGUID) + ",");
                sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                if (radDescription.Html.Length != 0)
                {
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml(radDescription.Html)) + ",");
                }
                else
                {
                    sql.Append("NULL,");
                }
                sql.Append(rbIsChecked.SelectedValue + ",");
                sql.Append(ddTaxClass.SelectedValue + ",");
                sql.Append(CommonLogic.IIF(Cost != System.Decimal.Zero, Localization.DecimalStringForDB(Cost), "0.0"));
                sql.Append(")");
                DB.ExecuteSQL(sql.ToString());

                resetError("Order Option added.", false);

                rs = DB.GetRS("select OrderOptionID from OrderOption  " + DB.GetNoLock() + " where OrderOptionGUID=" + DB.SQuote(NewGUID));
                rs.Read();
                OrderOptionID = DB.RSFieldInt(rs, "OrderOptionID");
                ViewState["EditingOrderOptions"] = true;
                ViewState["EditingOrderOptionsID"] = OrderOptionID.ToString();
                rs.Close();

                string treeT = OrderOptionID.ToString();
                loadTree();
                foreach (TreeNode node in treeMain.Nodes)
                {
                    if (node.Value.Equals(treeT))
                    {
                        node.Selected = true;
                    }
                }

                //getOrderOptionsDetails();
            }
            else
            {
                // ok to update:
                sql.Append("update OrderOption set ");
                sql.Append("Name=" + DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                if (radDescription.Html.Length != 0)
                {
                    sql.Append("Description=" + DB.SQuote(AppLogic.FormLocaleXml(radDescription.Html)) + ",");
                }
                else
                {
                    sql.Append("Description=NULL,");
                }
                sql.Append("DefaultIsChecked=" + rbIsChecked.SelectedValue + ",");
                sql.Append("TaxClassID=" + ddTaxClass.SelectedValue + ",");
                sql.Append("Cost=" + CommonLogic.IIF(Cost != System.Decimal.Zero, Localization.DecimalStringForDB(Cost), "0.0"));
                sql.Append(" where OrderOptionID=" + OrderOptionID.ToString());
                DB.ExecuteSQL(sql.ToString());

                resetError("Order Option updated.", false);
                //getOrderOptionsDetails();

                string treeT = treeMain.SelectedValue;
                loadTree();
                foreach (TreeNode node in treeMain.Nodes)
                {
                    if (node.Value.Equals(treeT))
                    {
                        node.Selected = true;
                    }
                }
            }

            // handle image uploaded:
            try
            {
                String Image1 = String.Empty;
                HttpPostedFile Image1File = fuIcon.PostedFile;
                if (Image1File.ContentLength != 0)
                {
                    // delete any current image file first
                    try
                    {
                        System.IO.File.Delete(AppLogic.GetImagePath("OrderOption", "icon", true) + OrderOptionID.ToString() + ".jpg");
                        System.IO.File.Delete(AppLogic.GetImagePath("OrderOption", "icon", true) + OrderOptionID.ToString() + ".gif");
                        System.IO.File.Delete(AppLogic.GetImagePath("OrderOption", "icon", true) + OrderOptionID.ToString() + ".png");
                    }
                    catch
                    { }

                    String s = Image1File.ContentType;
                    switch (Image1File.ContentType)
                    {
                        case "image/gif":
                            Image1 = AppLogic.GetImagePath("OrderOption", "icon", true) + OrderOptionID.ToString() + ".gif";
                            Image1File.SaveAs(Image1);
                            break;
                        case "image/x-png":
                            Image1 = AppLogic.GetImagePath("OrderOption", "icon", true) + OrderOptionID.ToString() + ".png";
                            Image1File.SaveAs(Image1);
                            break;
                        case "image/jpg":
                        case "image/jpeg":
                        case "image/pjpeg":
                            Image1 = AppLogic.GetImagePath("OrderOption", "icon", true) + OrderOptionID.ToString() + ".jpg";
                            Image1File.SaveAs(Image1);
                            break;
                    }
                }

                getOrderOptionsDetails();
            }
            catch (Exception ex)
            {
                ltError.Text = CommonLogic.GetExceptionDetail(ex, "<br/>");
            }
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        deleteOrderOptions();
    }

    protected void deleteOrderOptions()
    {
        ltValid.Text = "";

        string OrderOptionsID = ViewState["EditingOrderOptionsID"].ToString();
        // delete the mfg:
        DB.ExecuteSQL("delete from orderoption where OrderOptionID=" + OrderOptionsID);
        phMain.Visible = false;
        loadTree();
        loadScript(false);
        ViewState["EditingOrderOptions"] = false;
        ViewState["EditingOrderOptionsID"] = "0";
        resetError("Order Option deleted.", false);
    }

    protected void resetForm()
    {
        ltName.Text = AppLogic.GetLocaleEntryFields("", "Name", false, true, true, "Please enter the topic name", 100, 30, 0, 0, false);
        //ltDescription.Text = AppLogic.GetLocaleEntryFields("", "Description", true, true, false, "", 0, 0, 20, AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true);
        txtCost.Text = "";
        rbIsChecked.SelectedIndex = 1;
        ltIcon.Text = "";
        //load Tax Class
        ddTaxClass.Items.Clear();
        IDataReader rsst = DB.GetRS("select * from TaxClass  " + DB.GetNoLock() + " order by DisplayOrder,Name");
        while (rsst.Read())
        {
            ddTaxClass.Items.Add(new ListItem(DB.RSFieldByLocale(rsst, "Name", ThisCustomer.LocaleSetting), DB.RSFieldInt(rsst, "TaxClassID").ToString()));
        }
        rsst.Close();
    }
        
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        for (int i = 0; i <= Request.Form.Count - 1; i++)
        {
            if (Request.Form.Keys[i].IndexOf("DisplayOrder_") != -1)
            {
                String[] keys = Request.Form.Keys[i].Split('_');
                int OrderOptionID = Localization.ParseUSInt(keys[1]);
                int DispOrd = 1;
                try
                {
                    DispOrd = Localization.ParseUSInt(Request.Form[Request.Form.Keys[i]]);
                }
                catch { }
                DB.ExecuteSQL("update OrderOption set DisplayOrder=" + DispOrd.ToString() + " where OrderOptionID=" + OrderOptionID.ToString());
                lblMsg.Text = "<b>Notice: </b>" + "Display Order Updated";
            }
        }
        loadTree();
    }
}
