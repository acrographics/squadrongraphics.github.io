// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/topics.aspx.cs 9     10/04/06 12:00p Redwoodtree $
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

public partial class topics : System.Web.UI.Page
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
            ViewState["EditingTopic"] = false;
            ViewState["EditingTopicID"] = "0";

            if (cust.ThisCustomerSession.Session("topicPageLocale").Length == 0)
            {
                cust.ThisCustomerSession.SetVal("topicPageLocale", Localization.GetWebConfigLocale());
            }

            this.loadDD();
            this.loadTree();
            this.loadScript(false);
            this.resetForm();
            this.phMain.Visible = false;

            this.btnDelete.Attributes.Add("onClick", "return confirm('Confirm Delete');");

        }
        ltName.ID = "Name" + "_" + ddlPageLocales.SelectedValue.Replace("-", "_");
        ltTitle.ID = "Title" + "_" + ddlPageLocales.SelectedValue.Replace("-", "_");
    }

    private void loadDD()
    {
        this.ddLocales.Items.Clear();
        this.ddSkins.Items.Clear();

        this.ddSkins.Items.Add(new ListItem("ALL SKINS", "0"));
        this.ddLocales.Items.Add(new ListItem("ALL LOCALES", "0"));

        foreach (String i in AppLogic.FindAllSkins().Split(','))
        {
            this.ddSkins.Items.Add(new ListItem("Skin_" + i, i.ToString()));
        }

        DataSet ds = DB.GetDS("select Name from LocaleSetting " + DB.GetNoLock() + " order by DisplayOrder,Description", AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            this.ddLocales.Items.Add(new ListItem(DB.RowField(row, "Name"), DB.RowField(row, "Name")));
            this.ddlPageLocales.Items.Add(new ListItem(DB.RowField(row, "Name"), DB.RowField(row, "Name")));            
        }
        ds.Dispose();
        this.ddlPageLocales.Items.FindByValue(Localization.GetWebConfigLocale()).Selected = true;
    }

    private void loadTree()
    {
        try
        {
            treeMain.Nodes.Clear();

            //DATABASE TOPICS
            String ShowLocaleSetting = this.ddLocales.SelectedValue.Trim();
            if (ShowLocaleSetting.Equals("0"))
            {
                ShowLocaleSetting = "";
            }
            int ShowSkinID = Localization.ParseNativeInt(this.ddSkins.SelectedValue);

            string sql = string.Format( "select * from Topic  {0} where deleted=0 {1} order by DisplayOrder, Name ASC" , DB.GetNoLock() , CommonLogic.IIF(ShowSkinID != 0, " and (SkinID IS NULL or SkinID=0 or SkinID=" + ShowSkinID.ToString() + ")", "") );
            DataSet ds = DB.GetDS(sql, false);

            treeMain.Nodes.Add(new TreeNode("<b>Database</b>", "-1"));
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                TreeNode myNode = new TreeNode();
                myNode.Text = DB.RowFieldByLocale(row, "Name", cust.LocaleSetting);
                myNode.Value = DB.RowField(row, "TopicID");
                myNode.ImageUrl = "icons/dot.gif";
                //treeMain.FindNode("Database").ChildNodes.Add(myNode);
                treeMain.Nodes.Add(myNode);
            }
            ds.Dispose();

            // FILE BASED TOPICS:
            treeMain.Nodes.Add(new TreeNode("<b>File</b>", "-1"));
            foreach (String sid in AppLogic.FindAllSkins().Split(','))
            {
                int j = Localization.ParseUSInt(sid);
                //this.ltError.Text += "SkinID" + ShowSkinID + "|" + j;
                if (ShowSkinID == 0 || ShowSkinID == j)
                {
                    // File Topics:
                    // create an array to hold the list of files
                    ArrayList fArray = new ArrayList();

                    // get information about our initial directory
                    String SFP = CommonLogic.SafeMapPath(CommonLogic.IIF(AppLogic.IsAdminSite, "../", "") + "skins/skin_" + sid + "/template.htm").Replace("template.htm", "");

                    DirectoryInfo dirInfo = new DirectoryInfo(SFP);

                    // retrieve array of files & subdirectories
                    FileSystemInfo[] myDir = dirInfo.GetFileSystemInfos();

                    for (int i = 0; i < myDir.Length; i++)
                    {
                        // check the file attributes, skip subdirs:
                        if (!((Convert.ToUInt32(myDir[i].Attributes) & Convert.ToUInt32(FileAttributes.Directory)) > 0))
                        {
                            bool skipit = false;
                            if (!myDir[i].FullName.EndsWith("htm", StringComparison.InvariantCultureIgnoreCase) || (myDir[i].FullName.ToUpperInvariant().IndexOf("TEMPLATE") != -1))
                            {
                                skipit = true;
                            }
                            if (ShowLocaleSetting.Length != 0)
                            {
                                if (!myDir[i].FullName.EndsWith("." + ShowLocaleSetting.ToUpperInvariant() + ".htm", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    skipit = true;
                                }
                            }
                            if (!skipit)
                            {
                                fArray.Add(Path.GetFileName(myDir[i].FullName));
                            }
                        }
                    }

                    if (fArray.Count != 0)
                    {
                        // sort the files alphabetically
                        fArray.Sort(0, fArray.Count, null);
                        for (int i = 0; i < fArray.Count; i++)
                        {
                            TreeNode myNode = new TreeNode();
                            myNode.Value = "skins/skin_" + sid + "/" + fArray[i].ToString();
                            myNode.Text = "<span onclick=\"window.open('../" + myNode.Value +"','Topic','');\">skin_" + sid + "/" + fArray[i].ToString() + "</span>";
                            myNode.ImageUrl = "icons/dot.gif";
                            //treeMain.FindNode("File").ChildNodes.Add(myNode);
                            treeMain.Nodes.Add(myNode);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            this.resetError(ex.ToString(), true);
        }
    }


    protected void treeMain_SelectedNodeChanged(object sender, EventArgs e)
    {
        this.ltValid.Text = "";

        if (!this.treeMain.SelectedValue.Equals("-1"))
        {
            if (this.treeMain.SelectedValue.IndexOf("skin") == -1)
            {
                this.resetForm();
                ViewState["EditingTopic"] = true;
                this.loadScript(true);

                this.resetError("", false);
                this.phMain.Visible = true;

                string index = treeMain.SelectedNode.Value;
                ViewState["EditingTopicID"] = index;

                this.getTopicDetails();
                return;
            }
        }

        this.phMain.Visible = false;
        this.resetForm();
        this.loadTree();
        this.loadScript(false);
    }

    protected void getTopicDetails(  )
    {
        string iden = ViewState["EditingTopicID"].ToString();
        
        //pull locale from user session
        string entityLocale = cust.ThisCustomerSession.Session("topicPageLocale");
        if (entityLocale.Length > 0)
        {
            try
            {
                ddlPageLocales.SelectedValue = entityLocale;
                // user's locale may not exist any more, so don't let the assignment crash
            }
            catch { }
        }

        IDataReader rs = DB.GetRS(string.Format( "select * from Topic {0} where TopicID={1}" , DB.GetNoLock() , iden ));
        if (!rs.Read())
        {
            rs.Close();
            this.resetError("Unable to retrieve data.", true);
            return;
        }

        //editing Topic
        bool Editing = Localization.ParseBoolean(ViewState["EditingTopic"].ToString());
        this.ltMode.Text = "Editing";
        this.btnSubmit.Text = "Update Topic";
        this.btnDelete.Visible = true;

        this.ltName.Text = DB.RSFieldByLocale(rs, "Name", ddlPageLocales.SelectedValue);
        this.ltTitle.Text = DB.RSFieldByLocale(rs, "Title", ddlPageLocales.SelectedValue);
        //this.ltDescription.Text = AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Description"), "Description", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), !Editing || DB.RSFieldBool(rs, "HTMLOk"));
        radDescription.Html = DB.RSFieldByLocale(rs, "Description", ddlPageLocales.SelectedValue);
        this.ltSETitle.Text = DB.RSFieldByLocale(rs, "SETitle", ddlPageLocales.SelectedValue);
        this.ltSEKeywords.Text = DB.RSFieldByLocale(rs, "SEKeywords", ddlPageLocales.SelectedValue);
        this.ltSEDescription.Text = DB.RSFieldByLocale(rs, "SEDescription", ddlPageLocales.SelectedValue);

        this.txtContentsBG.Text = CommonLogic.IIF(Editing, DB.RSField(rs, "ContentsBGColor"), "");
        this.txtPageBG.Text = CommonLogic.IIF(Editing, DB.RSField(rs, "PageBGColor"), "");
        this.txtPassword.Text = CommonLogic.IIF(Editing, DB.RSField(rs, "Password"), "");
        this.txtSkin.Text = CommonLogic.IIF(DB.RSFieldInt(rs, "SkinID") == 0, "", DB.RSFieldInt(rs, "SkinID").ToString());
        this.txtDspOrdr.Text = DB.RSFieldInt( rs , "DisplayOrder" ).ToString( );
        this.txtSkinColor.Text = CommonLogic.IIF(Editing, DB.RSField(rs, "GraphicsColor"), "");

        this.rbDisclaimer.SelectedIndex = CommonLogic.IIF(DB.RSFieldBool(rs,"RequiresDisclaimer"),1,0);
        this.rbHTML.SelectedIndex = CommonLogic.IIF(DB.RSFieldBool(rs, "HTMLOk"), 1, 0);
        this.rbPublish.SelectedIndex = CommonLogic.IIF(DB.RSFieldBool(rs, "ShowInSiteMap"), 1, 0);
        this.rbSubscription.SelectedIndex = CommonLogic.IIF(DB.RSFieldBool(rs, "RequiresSubscription"), 1, 0);
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

        //((Literal)this.Form.FindControl("ltError")).Text = str;
        ltError.Text = str;
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        this.ltValid.Text = "";

        ViewState["EditingTopic"] = false;
        ViewState["EditingTopicID"] = "0";
        this.loadScript(true);
        this.resetForm();
        this.phMain.Visible = true;
        this.btnDelete.Visible = false;
        this.loadTree();
        //new Topic
        this.ltMode.Text = "Adding a";
        this.btnSubmit.Text = "Add Topic";
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
            this.ltStyles.Text = "";
        }
    }

    protected bool validateForm()
    {
        bool valid = true;
        string temp = "";
        //string focus = "";

        if ((string.IsNullOrEmpty(ltName.Text) || (AppLogic.FormLocaleXml("Name").Equals("<ml></ml>"))))
        {
            valid = false;
            temp += "Please fill out the Name";
            //focus = "Name";
        }
        else if ((string.IsNullOrEmpty( ltTitle.Text) || (AppLogic.FormLocaleXml("Title").Equals("<ml></ml>"))))
        {
            valid = false;
            temp += "Please fill out the Title";
            //focus = "Title";
        }
        if (!valid)
        {
            //this.ltName.Text = AppLogic.FormLocaleXml("Name");
            //this.ltTitle.Text = AppLogic.FormLocaleXml("Title");      
            this.ltValid.Text = "<script type=\"text/javascript\">alert('" + temp + "');</script>";// document.frmTopics." + focus + ".focus();</script>";
        }

        return valid;
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        updateTopic();
    }

    protected void updateTopic()
    {
        this.ltValid.Text = "";

        if (this.validateForm())
        {
            bool Editing = Localization.ParseBoolean(ViewState["EditingTopic"].ToString());
            string TopicID = ViewState["EditingTopicID"].ToString();
            IDataReader rs;

            StringBuilder sql = new StringBuilder(2500);
            if (!Editing)
            {
                // ok to add them:
                String NewGUID = DB.GetNewGUID();
                sql.Append("insert into Topic(TopicGUID,Name,SkinID,DisplayOrder,ContentsBGColor,PageBGColor,GraphicsColor,Title,Description,Password,RequiresSubscription,HTMLOk,RequiresDisclaimer,ShowInSiteMap,SEKeywords,SEDescription,SETitle) values(");
                sql.Append(DB.SQuote(NewGUID) + ",");
                sql.Append(DB.SQuote(AppLogic.FormLocaleXml(ltName.Text, ddlPageLocales.SelectedValue)) + ",");
                sql.Append(Localization.ParseUSInt(this.txtSkin.Text) + ",");
                sql.Append(Localization.ParseUSInt(this.txtDspOrdr.Text) + ",");
                sql.Append(DB.SQuote(this.txtContentsBG.Text) + ",");
                sql.Append(DB.SQuote(this.txtPageBG.Text) + ",");
                sql.Append(DB.SQuote(this.txtSkinColor.Text) + ",");
                sql.Append(DB.SQuote(AppLogic.FormLocaleXml(ltTitle.Text, ddlPageLocales.SelectedValue)) + ",");
                if (radDescription.Html.Length != 0)
                {
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml(radDescription.Html, ddlPageLocales.SelectedValue)) + ",");
                }
                else
                {
                    sql.Append("NULL,");
                }
                if (this.txtPassword.Text.Trim().Length != 0)
                {
                    sql.Append(DB.SQuote(this.txtPassword.Text) + ",");
                }
                else
                {
                    sql.Append("NULL,");
                }
                sql.Append(this.rbSubscription.SelectedValue.ToString() + ",");
                sql.Append(this.rbHTML.SelectedValue.ToString() + ",");
                sql.Append(this.rbDisclaimer.SelectedValue.ToString() + ",");
                sql.Append(this.rbPublish.SelectedValue.ToString() + ",");
                if (AppLogic.FormLocaleXml("SEKeywords").Length != 0)
                {
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml(ltSEKeywords.Text, ddlPageLocales.SelectedValue)) + ",");
                }
                else
                {
                    sql.Append("NULL,");
                }
                if (AppLogic.FormLocaleXml("SEDescription").Length != 0)
                {
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml(ltSEDescription.Text, ddlPageLocales.SelectedValue)) + ",");
                }
                else
                {
                    sql.Append("NULL,");
                }
                if (AppLogic.FormLocaleXml("SETitle").Length != 0)
                {
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml(ltSETitle.Text, ddlPageLocales.SelectedValue)));
                }
                else
                {
                    sql.Append("NULL");
                }
                sql.Append(")");
                DB.ExecuteSQL(sql.ToString());

                this.resetError("Topic added.", false);

                rs = DB.GetRS(string.Format( "select TopicID from Topic  {0} where deleted=0 and TopicGUID={1}" , DB.GetNoLock() , DB.SQuote(NewGUID) ));
                rs.Read();
                TopicID = DB.RSFieldInt(rs, "TopicID").ToString();
                ViewState["EditingTopic"] = true;
                ViewState["EditingTopicID"] = TopicID.ToString();
                rs.Close();

                string treeT = TopicID.ToString();
                this.loadTree();
                foreach (TreeNode node in treeMain.Nodes)
                {
                    if (node.Value.Equals(treeT))
                    {
                        node.Selected = true;
                    }
                }

                this.getTopicDetails();
            }
            else
            {
                // ok to update:
                sql.Append("update Topic set ");
                sql.Append("Name=" + DB.SQuote(AppLogic.FormLocaleXml("Name", ltName.Text, ddlPageLocales.SelectedValue, "topic", Convert.ToInt32(TopicID))) + ",");
                sql.Append("SkinID=" + Localization.ParseUSInt(this.txtSkin.Text) + ",");
                sql.Append("DisplayOrder=" + Localization.ParseUSInt(this.txtDspOrdr.Text) + ",");
                sql.Append("ContentsBGColor=" + DB.SQuote(this.txtContentsBG.Text) + ",");
                sql.Append("PageBGColor=" + DB.SQuote(this.txtPageBG.Text) + ",");
                sql.Append("GraphicsColor=" + DB.SQuote(this.txtSkinColor.Text) + ",");
                sql.Append("Title=" + DB.SQuote(AppLogic.FormLocaleXml("Title", ltTitle.Text, ddlPageLocales.SelectedValue, "topic", Convert.ToInt32(TopicID))) + ",");
                if (radDescription.Html.Length != 0)
                {
                    sql.Append("Description=" + DB.SQuote(AppLogic.FormLocaleXml("Description", radDescription.Html, ddlPageLocales.SelectedValue, "topic", Convert.ToInt32(TopicID))) + ",");
                }
                else
                {
                    sql.Append("Description=NULL,");
                }
                if (this.txtPassword.Text.Trim().Length != 0)
                {
                    sql.Append("Password=" + DB.SQuote(this.txtPassword.Text) + ",");
                }
                else
                {
                    sql.Append("Password=NULL,");
                }
                sql.Append("RequiresSubscription=" + this.rbSubscription.SelectedValue.ToString() + ",");
                sql.Append("HTMLOk=" + this.rbHTML.SelectedValue.ToString() + ",");
                sql.Append("RequiresDisclaimer=" + this.rbDisclaimer.SelectedValue.ToString() + ",");
                sql.Append("ShowInSiteMap=" + this.rbPublish.SelectedValue.ToString() + ",");
                if (AppLogic.FormLocaleXml("SEKeywords").Length != 0)
                {
                    sql.Append("SEKeywords=" + DB.SQuote(AppLogic.FormLocaleXml("SEKeywords", ltSEKeywords.Text, ddlPageLocales.SelectedValue, "topic", Convert.ToInt32(TopicID))) + ",");
                }
                else
                {
                    sql.Append("SEKeywords=NULL,");
                }
                if (AppLogic.FormLocaleXml("SEDescription").Length != 0)
                {
                    sql.Append("SEDescription=" + DB.SQuote(AppLogic.FormLocaleXml("SEDescription", ltSEDescription.Text, ddlPageLocales.SelectedValue, "topic", Convert.ToInt32(TopicID))) + ",");
                }
                else
                {
                    sql.Append("SEDescription=NULL,");
                }
                if (AppLogic.FormLocaleXml("SETitle").Length != 0)
                {
                    sql.Append("SETitle=" + DB.SQuote(AppLogic.FormLocaleXml("SETitle", ltSETitle.Text, ddlPageLocales.SelectedValue, "topic", Convert.ToInt32(TopicID))));
                }
                else
                {
                    sql.Append("SETitle=NULL");
                }
                sql.Append(" where TopicID=" + TopicID.ToString());
                //throw new Exception(sql.ToString());
                DB.ExecuteSQL(sql.ToString());
                this.resetError("Topic updated.", false);
                this.getTopicDetails();

                string treeT = treeMain.SelectedValue;
                this.loadTree();
                foreach (TreeNode node in treeMain.Nodes)
                {
                    if (node.Value.Equals(treeT))
                    {
                        node.Selected = true;
                    }
                }
            }
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        deleteTopic();
    }

    protected void deleteTopic()
    {
        this.ltValid.Text = "";

        string TopicID = ViewState["EditingTopicID"].ToString();
        // delete the mfg:
        DB.ExecuteSQL("update Topic set deleted=1 where TopicID=" + TopicID);
        this.phMain.Visible = false;
        this.loadTree();
        this.loadScript(false);
        ViewState["EditingTopic"] = false;
        ViewState["EditingTopicID"] = "0";
        resetError("Topic deleted.", false);
    }

    protected void resetForm()
    {
        this.txtContentsBG.Text = "";
        this.txtPageBG.Text = "";
        this.txtPassword.Text = "";
        this.txtSkin.Text = "";
        this.txtSkinColor.Text = "";

        this.rbDisclaimer.SelectedIndex = 0;
        this.rbHTML.SelectedIndex = 1;
        this.rbPublish.SelectedIndex = 0;
        this.rbSubscription.SelectedIndex = 0;

        // Not sure this is correct...
        this.ltName.Text = "";
        this.ltTitle.Text = "";
        //this.ltDescription.Text = AppLogic.GetLocaleEntryFields("", "Description", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true);
        radDescription.Html = "";
        this.ltSETitle.Text = "";
        this.ltSEKeywords.Text = "";
        this.ltSEDescription.Text = "";

        cust.ThisCustomerSession.SetVal("topicPageLocale", Localization.GetWebConfigLocale());
        ddlPageLocales.SelectedValue = Localization.GetWebConfigLocale();
        ViewState["EditingTopicID"] = "";
        this.resetError("", false);

    }
    protected void ddSkins_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.ltValid.Text = "";

        string treeT = treeMain.SelectedValue;
        this.loadTree();
        foreach (TreeNode node in treeMain.Nodes)
        {
            if (node.Value.Equals(treeT))
            {
                node.Selected = true;
            }
        }
    }
    
    protected void ddLocales_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.ltValid.Text = "";

        string treeT = treeMain.SelectedValue;
        this.loadTree();
        foreach (TreeNode node in treeMain.Nodes)
        {
            if (node.Value.Equals(treeT))
            {
                node.Selected = true;
            }
        }
    }

    protected void ddlPageLocales_SelectedIndexChanged(object sender, EventArgs e)
    {
        cust.ThisCustomerSession.SetVal("topicPageLocale", ddlPageLocales.SelectedValue);
        if (ViewState["EditingTopicID"].ToString() != "")
        {
            getTopicDetails();
        }
    }
}
