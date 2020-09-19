// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/feeds.aspx.cs 4     8/30/06 8:33p Redwoodtree $
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
using AspDotNetStorefrontAdmin;
using AspDotNetStorefrontCommon;


public partial class feeds : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");
        
        if (!IsPostBack)
        {
            InitializePageData();
        }
    }

    protected void rptrFeeds_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item)
        {
            Button delButton = (Button)e.Item.FindControl("btnDeleteFeed");
            delButton.Attributes.Add("onClick", "javascript: return confirm('Are you sure you want to delete this feed?')");
        }
    }


    protected void rptrFeeds_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        int FeedID = Convert.ToInt32(e.CommandArgument);
        switch (e.CommandName)
        {
            case "execute":
                ExecuteFeed(FeedID);
                break;
            case "delete":
                DeleteFeed(FeedID);
                break;
        }
    }

    private void InitializePageData()
    {
        IDataReader dr = DB.GetRS("aspdnsf_GetFeed");
        rptrFeeds.DataSource = dr;
        rptrFeeds.DataBind();
        dr.Close();
        lblError.Visible = (lblError.Text.Length > 0);
    }

    private void ExecuteFeed(int FeedID)
    {
        Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
        Feed f = new Feed(FeedID);
        String RuntimeParams = String.Empty;
        lblError.Text = f.ExecuteFeed(ThisCustomer,RuntimeParams);
        InitializePageData();
    }

    private void DeleteFeed(int FeedID)
    {
        lblError.Text = Feed.DeleteFeed(FeedID);
        InitializePageData();
    }
}
