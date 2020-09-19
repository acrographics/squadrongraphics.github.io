// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editfeed.aspx.cs 6     9/09/06 2:16p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using AspDotNetStorefrontAdmin;
using AspDotNetStorefrontCommon;

public partial class editfeed : System.Web.UI.Page
{
    int FeedID;
    Feed m_Feed;
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");

        if (this.ViewState["feedid"] != null)
        {
            FeedID = Convert.ToInt32(this.ViewState["feedid"]);
        }
        else
        {
            FeedID = CommonLogic.QueryStringUSInt("feedid");
        }

        if (FeedID != 0)
        {
            m_Feed = new Feed(FeedID);
        }

        if (!IsPostBack)
        {
            InitializePageData();
        }
    }

    private void InitializePageData()
    {
        string path = Server.MapPath("") + @"\XmlPackages";
        string searchpattern = "feed*";
        string[] feeds = Directory.GetFiles(path, searchpattern);

        foreach (string s in feeds)
        {
            XmlPackage.Items.Add(new ListItem(s.Substring(s.LastIndexOf(@"\") + 1), s.Substring(s.LastIndexOf(@"\") + 1)));
        }

        if (m_Feed != null)
        {
            txtFeedName.Text = m_Feed.Name;
            XmlPackage.SelectedValue = m_Feed.XmlPackage;
            txtFtpUserName.Text = m_Feed.FTPUsername;
            txtFtpPwd.Text = m_Feed.FTPPassword;
            txtFtpServer.Text = m_Feed.FTPServer;
            txtFtpPort.Text = m_Feed.FTPPort.ToString();
            txtFtpFileName.Text = m_Feed.FTPFilename;
            CanAutoFtp.Items[0].Selected = m_Feed.CanAutoFTP;
            CanAutoFtp.Items[1].Selected = !m_Feed.CanAutoFTP;
            btnSubmit.Text = "Update Feed";
            this.ViewState["feedid"] = FeedID.ToString();
            Label1.Text = "Editing Feed:";
            pageheading.Text = m_Feed.Name;
            btnExecFeed.Visible = true;
        }
        else
        {
            Label1.Text = "";
            pageheading.Text = "Create New Feed";
            btnExecFeed.Visible = false;
        }
        //lblError.Visible = (lblError.Text.Length > 0);
    }

    protected void resetError(string errorx, bool isError)
    {
        string str = "<font class=\"noticeMsg\">NOTICE:</font>&nbsp;&nbsp;&nbsp;";
        if (isError)
            str = "<font class=\"errorMsg\">ERROR:</font>&nbsp;&nbsp;&nbsp;";

        if (errorx.Length > 0)
            str += errorx + "";
        else
            str = "";

        this.ltError.Text = str;
    }

    protected void btnSubmit_OnClick(object sender, EventArgs e)
    {
        Page.Validate();
        if (Page.IsValid)
        {
            if (m_Feed != null)
            {
                resetError(UpdateFeed(), false);
            }
            else
            {
                m_Feed = Feed.CreateFeed(txtFeedName.Text,
                                         1,
                                         XmlPackage.SelectedValue,
                                         (CanAutoFtp.SelectedValue == "1"),
                                         txtFtpUserName.Text,
                                         txtFtpPwd.Text,
                                         txtFtpServer.Text,
                                         Convert.ToInt32(txtFtpPort.Text.Trim()),
                                         txtFtpFileName.Text,
                                         "");
            }
        }
        else
        {
            resetError("Errors occured " + CommonLogic.IIF(m_Feed == null, "creating", "updating") + " the feed", true);
        }
    
        InitializePageData();
    }

    protected void btnExecFeed_OnClick(object sender, EventArgs e)
    {
        ExecuteFeed();
    }

    private void ExecuteFeed()
    {
        Server.ScriptTimeout = 120;
        if (txtFtpServer.Text.Trim().Length == 0 && m_Feed.CanAutoFTP)
        {
            resetError("No ftp server specified", true);
        }
        else if (txtFtpFileName.Text.Trim().Length == 0)
        {
            resetError("No remote filename specified", true);
        }
        else
        {
            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
            String RuntimeParams = String.Empty;
            UpdateFeed();
            resetError(m_Feed.ExecuteFeed(ThisCustomer, RuntimeParams), false);
        }
        InitializePageData();
    }


    private string UpdateFeed()
    {
        return m_Feed.UpdateFeed(txtFeedName.Text,
                               -1,
                               XmlPackage.SelectedValue,
                               Convert.ToSByte(CanAutoFtp.SelectedValue),
                               txtFtpUserName.Text,
                               txtFtpPwd.Text,
                               txtFtpServer.Text,
                               Convert.ToInt32(txtFtpPort.Text.Trim()),
                               txtFtpFileName.Text.Trim(),
                               "");
    }


    protected void ValidateXmlPackage(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (XmlPackage.SelectedIndex > 0);
    }

    protected void ValidatePort(object source, ServerValidateEventArgs args)
    {
        string port = txtFtpPort.Text.Trim();
        if (port.Length > 0)
        {
            args.IsValid = (CommonLogic.IsInteger(port));
        }
        else
        {
            args.IsValid = true;
        }
    }

    protected void ValidateServer(object source, ServerValidateEventArgs args)
    {
        if (CanAutoFtp.Items[0].Selected && txtFtpServer.Text.Trim() == "")
        {
            args.IsValid = false;
        }
        else
        {
            args.IsValid = true;
        }
    }

}
