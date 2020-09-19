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

    public partial class eventhandler : System.Web.UI.Page
    {
        protected string selectSQL = "select EventID, EventName, CalloutURL, XmlPackage, Active,Debug from EventHandler ";
        private Customer cust;

        protected void Page_Load(object sender, EventArgs e)
        {         
                Response.CacheControl = "private";
                Response.Expires = 0;
                Response.AddHeader("pragma", "no-cache");
                cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

                if (!IsPostBack)
                {
                    string query = CommonLogic.QueryStringCanBeDangerousContent("searchfor");

                    loadXMLPackages();

                    ViewState["Sort"] = "EventName";
                    ViewState["SortOrder"] = "ASC";
                    ViewState["SQLString"] = selectSQL;

                    if (query.Length > 0)
                    {
                        resultFilter(query);
                    }
                    else
                    {
                        buildGridData(buildGridData());
                    }
                }
            
        }
       

        protected void resultFilter(string SearchFor)
        {
            String sql = selectSQL + DB.GetNoLock() + " ";
            String WhereClause= String.Empty;

            //search
            if (SearchFor.Length != 0)
            {
                WhereClause = " (EventName like " + DB.SQuote("%" + SearchFor + "%") + " or CalloutURL like " + DB.SQuote("%" + SearchFor + "%") + ")";
            }

            //set states
            ViewState["SQLString"] = sql.ToString();
            sql += " order by " + ViewState["Sort"].ToString() + " " + ViewState["SortOrder"].ToString();

            //build grid
            buildGridData(DB.GetDS(sql, false));
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

            ((Literal)Form.FindControl("ltError")).Text = str;
        }

        protected void gMain_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            btnInsert.Enabled = true;
            //if new item and cancel, must delete
            if (Localization.ParseBoolean(ViewState["IsInsert"].ToString()))
            {
                GridViewRow row = gMain.Rows[e.RowIndex];
                if (row != null)
                {
                    int iden = Localization.ParseNativeInt(row.Cells[1].Text.ToString());
                    deleteRow(iden);
                }
            }

            ViewState["IsInsert"] = false;

            gMain.EditIndex = -1;
            buildGridData(buildGridData());
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


                DropDownList ddXmlPackage = (DropDownList)e.Row.FindControl("ddEditXMLPackage");
                ddXmlPackage.Items.Clear();
                ListItem myNode = new ListItem();
                myNode.Value = "Select a Package...";
                ddXmlPackage.Items.Add(myNode);

                String Location = CommonLogic.SafeMapPath("~/XMLPackages");
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Location);
                foreach (System.IO.FileInfo f in dir.GetFiles("event.*")) 
                {
                    //LOAD FILES 
                    ListItem myNode1 = new ListItem();
                    myNode1.Value = f.Name.ToUpperInvariant();
                    ddXmlPackage.Items.Add(myNode1);
                } 
                
                if (ddXmlPackage.Items.Count > 1)
                {
                    ddXmlPackage.SelectedValue = myrow["XMLPackage"].ToString().ToUpperInvariant();
                }
               try
                {
                    if (ViewState["FirstTimeEdit"].ToString() == "0")
                    {
                        TextBox txt = (TextBox)e.Row.FindControl("txtEventName");
                        txt.Visible = false;
                        Literal lt = (Literal)e.Row.FindControl("ltEventName");
                        lt.Visible = true;
                    }
                    else
                    {
                        TextBox txt = (TextBox)e.Row.FindControl("txtEventName");
                        txt.Visible = true;
                        Literal lt = (Literal)e.Row.FindControl("ltEventName");
                        lt.Visible = false;
                    }
                }
                catch
                {
                    TextBox txt = (TextBox)e.Row.FindControl("txtEventName");
                    txt.Visible = false;
                    Literal lt = (Literal)e.Row.FindControl("ltEventName");
                    lt.Visible = true;
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
                int eventID = Localization.ParseNativeInt(e.CommandArgument.ToString());
                deleteRow(eventID);
            }
        }
        protected void deleteRow(int iden)
        {
            StringBuilder sql = new StringBuilder(2500);
            sql.Append("DELETE FROM EventHandler WHERE EventID=" + iden.ToString());
            try
            {
                DB.ExecuteSQL(sql.ToString());
                AppLogic.ClearCache();

                loadXMLPackages();

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
            ViewState["IsInsert"] = false;
            resetError("", false);
            gMain.PageIndex = e.NewPageIndex;
            gMain.EditIndex = -1;
            buildGridData(buildGridData());
        }
        protected void gMain_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            btnInsert.Enabled = true;
         
            ViewState["IsInsert"] = false;
            GridViewRow row = gMain.Rows[e.RowIndex];

            if (row != null)
            {
                string iden = row.Cells[1].Text.ToString();
                TextBox eventname = (TextBox)row.FindControl("txtEventName");
                TextBox xmlpackage = (TextBox)row.FindControl("txtXMLPackage");
                TextBox callouturl = (TextBox)row.FindControl("txtCalloutURL");
                DropDownList ddXMLPackage = (DropDownList)row.FindControl("ddEditXMLPackage");
                CheckBox Active = (CheckBox)row.FindControl("cbkActive");
                CheckBox Debug = (CheckBox)row.FindControl("cbkDebug");

                string XMLPackagename = ddXMLPackage.SelectedValue;
                if (ddXMLPackage.SelectedIndex == 0)
                {
                    XMLPackagename = xmlpackage.Text;
                }

                AspdnsfEventHandler a = AppLogic.eventHandler(eventname.Text);
                if (a == null)
                {
                    resetError("You've specified an EventHandler that does not exist.", true);
                }
                else
                {
                    //object SuperOnly = null;
                    //if (cust.IsAdminSuperUser)
                    //{
                    //    SuperOnly = sa.Checked;
                    //}
                    a.Update(eventname.Text, callouturl.Text, XMLPackagename.ToUpperInvariant(), Active.Checked,Debug.Checked);   //TO_DO_MJ : How to set Active parameter on this page. Defaulted it to TRUE            

                    resetError("Item updated", false);
                }
                try
                {
                    loadXMLPackages();
                    gMain.EditIndex = -1;
                    resultFilter("");
                }
                catch { }
                }
              
        }
        protected void gMain_RowEditing(object sender, GridViewEditEventArgs e)
        {
            btnInsert.Enabled = false;
            ViewState["IsInsert"] = false;
            gMain.EditIndex = e.NewEditIndex;
            buildGridData(buildGridData());
        }
        protected void btnInsert_Click(object sender, EventArgs e)
        {
            ViewState["IsInsert"] = false;
            gMain.EditIndex = -1;
            StringBuilder sql = new StringBuilder(2500);

            if (txtAddName.Text.Trim().Length > 0 && txtAddName.Text.Trim().ToLowerInvariant() != "eventhandler name")
            {
                string name = txtAddName.Text.Trim();
                // see if this name is already there:
                if (AppLogic.eventHandler(name) != null)
                {
                    if (AppLogic.eventHandler(name).EventName.Length > 0)
                    {
                        resetError("There is already another Eventhandler parameter with that name.", true);
                        return;
                    }
                }
                try
                {
                    AppLogic.EventHandlerTable.Add(name, txtAddURL.Text.Trim(), ddAddXmlPackage.SelectedValue, true,true);//TO_DO_MJ Active value defualted to true again here
                    AppLogic.ClearCache();
                    ViewState["SQLString"] = selectSQL + " WHERE [EventName]=" + DB.SQuote(name);
                    ViewState["Sort"] = "EventID";
                    ViewState["SortOrder"] = "DESC";

                    ViewState["FirstTimeEdit"] = "0";//"1";

                    //gMain.EditIndex = 0;
                    buildGridData(buildGridData());
                    resetError("Item added.", false);
                    ViewState["IsInsert"] = true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Couldn't update database: " + ex.ToString());
                }
            }
            else
            {
                resetError("Please enter the EventHandler Name to add.", true);
            }
        }


        protected void ddAddXmlPackage_SelectedIndexChanged(object sender, EventArgs e)
        {      
            ViewState["IsInsert"] = false;
            gMain.EditIndex = -1;
            resetError("", false);
            gMain.PageIndex = 0;
            resultFilter("");
      
        }


        private void loadXMLPackages()
        {
            String Location = CommonLogic.SafeMapPath("~/XMLPackages");
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Location);
            foreach (System.IO.FileInfo f in dir.GetFiles("event.*")) 
            {
                //LOAD FILES 
                ListItem myNode = new ListItem();
                myNode.Value = f.Name.ToUpperInvariant();
                ddAddXmlPackage.Items.Add(myNode);
            } 
        }
}
}
