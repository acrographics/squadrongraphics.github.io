// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/news.aspx.cs 4     9/30/06 3:38p Redwoodtree $
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

public partial class news : System.Web.UI.Page
{
    protected Customer cust;

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");

        cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

        if (!IsPostBack)
        {
            ViewState["EditingNews"] = false;
            ViewState["EditingNewsID"] = "0"; 

            this.loadTree();
            this.loadScript(false);
            this.resetForm();
            this.phMain.Visible = false;

            this.btnDelete.Attributes.Add("onClick", "return confirm('Confirm Delete');");
        }
    }

    private void loadTree()
    {
        try
        {
            treeMain.Nodes.Clear();

            DataSet ds = DB.GetDS("select * from News " + DB.GetNoLock() + " where deleted=0 order by CreatedOn desc", false);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                TreeNode myNode = new TreeNode();

                string temp = CommonLogic.Ellipses(DB.RowFieldByLocale(row, "Headline", cust.LocaleSetting), 20, false) + " (" + Localization.ToNativeShortDateString(DB.RowFieldDateTime(row,"CreatedOn")) + ")";

                myNode.Text = Server.HtmlEncode(temp);
                myNode.Value = DB.RowFieldInt(row, "NewsID").ToString();
                myNode.ImageUrl = "icons/dot.gif";
                treeMain.Nodes.Add(myNode);
            }

            ds.Dispose();
        }
        catch (Exception ex)
        {
            this.resetError(ex.ToString(), true);
        }
    }

    protected void treeMain_SelectedNodeChanged(object sender, EventArgs e)
    {
        this.resetForm();
        ViewState["EditingNews"] = true;
        this.loadScript(true);
            
        this.resetError("", false);
        this.phMain.Visible = true;

        string index = treeMain.SelectedNode.Value;
        ViewState["EditingNewsID"] = index;

        this.getNewsDetails();
    }

    protected void getNewsDetails()
    {
        IDataReader rs = DB.GetRS("select * from News " + DB.GetNoLock() + " where NewsID=" + ViewState["EditingNewsID"].ToString() + " ORDER BY createdon DESC");
        if (!rs.Read())
        {
            rs.Close();
            this.resetError("Unable to retrieve data.", true);
            return;
        }

        //editing News
        this.ltMode.Text = "Editing";
        this.btnSubmit.Text = "Update News";
        this.btnDelete.Visible = true;
                
        this.ltHeadline.Text = AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Headline"), "Headline", false, true, true, "Please enter the news headline", 100, 50, 0, 0, false);
        //this.ltCopy.Text = AppLogic.GetLocaleEntryFields(DB.RSField(rs, "NewsCopy"), "NewsCopy", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true);
        radCopy.Html = DB.RSField( rs , "NewsCopy" );
        this.txtDate.Text = CommonLogic.IIF(true, Localization.ToNativeShortDateString(DB.RSFieldDateTime(rs, "ExpiresOn")), Localization.ToNativeShortDateString(System.DateTime.Now.AddMonths(1)));

        this.rbPublished.Items.FindByValue(rs["Published"].ToString()).Selected = true;
        rs.Close();
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

        ((Literal)this.Form.FindControl("ltError")).Text = str;
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        ViewState["EditingNews"] = false;
        ViewState["EditingNewsID"] = "0";
        this.loadScript(true);
        this.phMain.Visible = true;
        this.btnDelete.Visible = false;
        this.resetForm();
        this.loadTree();
        //new News
        this.ltMode.Text = "Adding a";
        this.btnSubmit.Text = "Add News";

    }

    protected void loadScript(bool load)
    {
        if (load)
        {
            this.ltScript.Text = ("\n<script type=\"text/javascript\">\n");
            this.ltScript.Text += ("    Calendar.setup({\n");
            this.ltScript.Text += ("        inputField     :    \"txtDate\",      // id of the input field\n");
            this.ltScript.Text += ("        ifFormat       :    \"" + Localization.JSCalendarDateFormatSpec() + "\",       // format of the input field\n");
            this.ltScript.Text += ("        showsTime      :    false,            // will display a time selector\n");
            this.ltScript.Text += ("        button         :    \"f_trigger_s\",   // trigger for the calendar (button ID)\n");
            this.ltScript.Text += ("        singleClick    :    true            // double-click mode\n");
            this.ltScript.Text += ("    });\n");
            this.ltScript.Text += ("</script>\n");

            if (AppLogic.NumLocaleSettingsInstalled() > 1)
            {
                this.ltScript1.Text = CommonLogic.ReadFile("jscripts/tabs.js", true);
            }

            this.ltDate.Text = "<img src=\"" + AppLogic.LocateImageURL("skins/skin_1/images/calendar.gif") + "\" style=\"cursor:hand;cursor:pointer;\" align=\"absmiddle\" id=\"f_trigger_s\">&nbsp;<small>(" + Localization.ShortDateFormat() + ")</small>";

            this.ltStyles.Text = ("  <!-- calendar stylesheet -->\n");
            this.ltStyles.Text += ("  <link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"jscalendar/calendar-win2k-cold-1.css\" title=\"win2k-cold-1\" />\n");
            this.ltStyles.Text += ("\n");
            this.ltStyles.Text += ("  <!-- main calendar program -->\n");
            this.ltStyles.Text += ("  <script type=\"text/javascript\" src=\"jscalendar/calendar.js\"></script>\n");
            this.ltStyles.Text += ("\n");
            this.ltStyles.Text += ("  <!-- language for the calendar -->\n");
            this.ltStyles.Text += ("  <script type=\"text/javascript\" src=\"jscalendar/lang/" + Localization.JSCalendarLanguageFile() + "\"></script>\n");
            this.ltStyles.Text += ("\n");
            this.ltStyles.Text += ("  <!-- the following script defines the Calendar.setup helper function, which makes\n");
            this.ltStyles.Text += ("       adding a calendar a matter of 1 or 2 lines of code. -->\n");
            this.ltStyles.Text += ("  <script type=\"text/javascript\" src=\"jscalendar/calendar-setup.js\"></script>\n");
        }
        else
        {
            this.ltScript.Text = "";
            this.ltStyles.Text = "";
        }
    }

    protected bool validateInput()
    {
        string headline = AppLogic.FormLocaleXml( "Headline" );
        if (headline.Equals("<ml></ml>") || string.IsNullOrEmpty( headline))
        {
            // this.ltCopy.Text = AppLogic.GetLocaleEntryFields(AppLogic.FormLocaleXml("NewsCopy"), "NewsCopy", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true);

            string temp = "Please enter the Headline! <script type=\"text/javascript\">alert('Please enter the Headline!');</script>";
            this.resetError(temp, true);
            return false;
        }
        return true;
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        bool Editing = Localization.ParseBoolean(ViewState["EditingNews"].ToString());
        string NewsID = ViewState["EditingNewsID"].ToString();
        IDataReader rs;

        //throw new Exception(DB.SQuote(AppLogic.FormLocaleXml("Headline")) + "|" + Editing.ToString());
        if (this.validateInput())
        {
            try
            {
                DateTime dt = System.DateTime.Now.AddMonths(6);
                if (this.txtDate.Text.Length > 0)
                {
                    dt = Localization.ParseNativeDateTime(this.txtDate.Text);
                }

                StringBuilder sql = new StringBuilder(2500);
                if (!Editing)
                {
                    // ok to add them:
                    String NewGUID = DB.GetNewGUID();
                    sql.Append("insert into news(NewsGUID,ExpiresOn,Headline,NewsCopy,Published) values(");
                    sql.Append(DB.SQuote(NewGUID) + ",");
                    sql.Append(DB.DateQuote(Localization.ToDBDateTimeString(dt)) + ",");
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Headline")) + ",");
                    sql.Append(DB.SQuote(radCopy.Html) + ",");
                    sql.Append(this.rbPublished.SelectedValue.ToString() + "");
                    sql.Append(")");

                    DB.ExecuteSQL(sql.ToString());

                    this.resetError("News added.", false);
                    this.loadTree();

                    rs = DB.GetRS("select NewsID from news " + DB.GetNoLock() + " where deleted=0 and NewsGUID=" + DB.SQuote(NewGUID));
                    rs.Read();
                    NewsID = DB.RSFieldInt(rs, "NewsID").ToString();
                    ViewState["EditingNews"] = true;
                    ViewState["EditingNewsID"] = NewsID;
                    rs.Close();

                    this.getNewsDetails();
                }
                else
                {
                    // ok to update:
                    sql.Append("update news set ");
                    sql.Append("Headline=" + DB.SQuote(AppLogic.FormLocaleXml("Headline")) + ",");
                    sql.Append("NewsCopy=" + DB.SQuote(radCopy.Html) + ",");
                    sql.Append("ExpiresOn=" + DB.DateQuote(Localization.ToDBDateTimeString(dt)) + ",");
                    sql.Append("Published=" + this.rbPublished.SelectedValue.ToString() + " ");
                    sql.Append("where NewsID=" + NewsID.ToString());
                    DB.ExecuteSQL(sql.ToString());
                    this.resetError("News updated.", false);
                    this.loadTree();

                    this.getNewsDetails();
                }
            }
            catch (Exception ex)
            {
                this.resetError("Unexpected error occurred: "+ex.ToString(), true);
            }
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        string NewsID = ViewState["EditingNewsID"].ToString();
        DB.ExecuteSQL("update News set deleted=1 where NewsID=" + NewsID);
        this.phMain.Visible = false;
        this.loadTree();
        this.loadScript(false);
        ViewState["EditingNews"] = false;
        ViewState["EditingNewsID"] = "0";
        resetError("News deleted.", false);
    }

    protected void resetForm()
    {
        this.ltHeadline.Text = AppLogic.GetLocaleEntryFields("", "Headline", false, true, true, "Please enter the news headline", 100, 50, 0, 0, false);
        //this.ltCopy.Text = AppLogic.GetLocaleEntryFields("", "NewsCopy", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true);

        this.txtDate.Text = Localization.ToNativeShortDateString(System.DateTime.Now.AddMonths(1));

        this.rbPublished.SelectedIndex = 1;
    }
}
