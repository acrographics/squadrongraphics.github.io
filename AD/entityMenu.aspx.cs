// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/entityMenu.aspx.cs 9     9/18/06 2:39p Jan Simacek $
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
using System.Xml;
using AspDotNetStorefrontCommon;

public partial class entityMenu : System.Web.UI.Page
{
    private Customer ThisCustomer;

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");

        ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

        int iSkinID = 1;
        if (SkinID.Trim().Length != 0)
        {
            iSkinID = System.Int32.Parse(SkinID);
        }
        if (!IsPostBack)
        {
            ddEntity.Items.Clear();
            ddEntity.Items.Add(new ListItem(AppLogic.GetString("admin.menu.Categories", iSkinID, ThisCustomer.LocaleSetting).ToUpperInvariant(), "CATEGORY"));
            ddEntity.Items.Add(new ListItem(AppLogic.GetString("admin.menu.Sections", iSkinID, ThisCustomer.LocaleSetting).ToUpperInvariant(), "SECTION"));
            ddEntity.Items.Add(new ListItem(AppLogic.GetString("admin.menu.Manufacturers", iSkinID, ThisCustomer.LocaleSetting).ToUpperInvariant(), "MANUFACTURER"));
            ddEntity.Items.Add(new ListItem(AppLogic.GetString("admin.menu.Distributors", iSkinID, ThisCustomer.LocaleSetting).ToUpperInvariant(), "DISTRIBUTOR"));
#if PRO
                //not supported in PRO version
#else
            ddEntity.Items.Add(new ListItem(AppLogic.GetString("admin.menu.Libraries", iSkinID, ThisCustomer.LocaleSetting).ToUpperInvariant(), "LIBRARY"));
            ddEntity.Items.Add(new ListItem(AppLogic.GetString("admin.menu.Genres", iSkinID, ThisCustomer.LocaleSetting).ToUpperInvariant(), "GENRE"));
            ddEntity.Items.Add(new ListItem(AppLogic.GetString("admin.menu.Vectors", iSkinID, ThisCustomer.LocaleSetting).ToUpperInvariant(), "VECTOR"));
#endif
            ThisCustomer.ThisCustomerSession.SetVal("EntityDDSelected", "0");
            LoadTree();
        }
    }

    protected void resetError(string error, bool isError)
    {
        string str = "<font class=\"noticeMsg\">NOTICE:</font>&nbsp;&nbsp;&nbsp;";
        if (isError)
            str = "<font class=\"errorMsg\">ERROR:</font>&nbsp;&nbsp;&nbsp;";

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
   
    protected void LoadScript(bool load)
    {
        if (load)
        {
            
        }
        else
        {
            ltScript.Text = "";
        }
    }

    protected void LoadTree()//(int parentID)
    {
        ComponentArt.Web.UI.NavigationCustomTemplate htmlServerTemplate = new ComponentArt.Web.UI.NavigationCustomTemplate();
        htmlServerTemplate.Template = new RenderHtmlServerTemplate();
        htmlServerTemplate.ID = "SRenderHtml";
        PageTree.Templates.Add(htmlServerTemplate);
        
        string entity = "";

        if (ThisCustomer.ThisCustomerSession.Session("EntityDDSelected") == "1")
        {
            entity = ddEntity.SelectedValue;
        }
        else
        {
            if (CommonLogic.QueryStringCanBeDangerousContent("entityName").Length > 0)
            {
                entity = CommonLogic.QueryStringCanBeDangerousContent("entityName");
            }
        }

        if (entity.Length == 0)
        {
            entity = "Category";
        }

        EntityHelper e;
        StringBuilder tmpS = new StringBuilder();
        tmpS.Append("<siteMap>");
        
        switch (entity.ToUpperInvariant())
        {
            case "SECTION":
            case "DEPARTMENT":
                ltEntity.Text = "Departments";
                e = new EntityHelper(EntityDefinitions.readonly_SectionEntitySpecs);
                tmpS.Append(e.ComponentArtTreeEntityMenu(0, 1, ThisCustomer.LocaleSetting, CommonLogic.QueryStringUSInt("EntityID")));
                break;
            case "MANUFACTURER":
                ltEntity.Text = "Manufacturers";
                e = new EntityHelper(EntityDefinitions.readonly_ManufacturerEntitySpecs);
                tmpS.Append(e.ComponentArtTreeEntityMenu(0, 1, ThisCustomer.LocaleSetting, CommonLogic.QueryStringUSInt("EntityID")));
                break;
            case "DISTRIBUTOR":
                ltEntity.Text = "Distributors";
                e = new EntityHelper(EntityDefinitions.readonly_DistributorEntitySpecs);
                tmpS.Append(e.ComponentArtTreeEntityMenu(0, 1, ThisCustomer.LocaleSetting, CommonLogic.QueryStringUSInt("EntityID")));
                break;
            case "GENRE":
                ltEntity.Text = "Genres";
                e = new EntityHelper(EntityDefinitions.readonly_GenreEntitySpecs);
                tmpS.Append(e.ComponentArtTreeEntityMenu(0, 1, ThisCustomer.LocaleSetting, CommonLogic.QueryStringUSInt("EntityID")));
                break;
            case "VECTOR":
                ltEntity.Text = "Vectors";
                e = new EntityHelper(EntityDefinitions.readonly_VectorEntitySpecs);
                tmpS.Append(e.ComponentArtTreeEntityMenu(0, 1, ThisCustomer.LocaleSetting, CommonLogic.QueryStringUSInt("EntityID")));
                break;
            case "LIBRARY":
                ltEntity.Text = "Libraries";
                e = new EntityHelper(EntityDefinitions.readonly_LibraryEntitySpecs);
                tmpS.Append(e.ComponentArtTreeEntityMenu(0, 1, ThisCustomer.LocaleSetting, CommonLogic.QueryStringUSInt("EntityID")));
                break;
            default:
                ltEntity.Text = "Categories";
                e = new EntityHelper(EntityDefinitions.readonly_CategoryEntitySpecs);
                tmpS.Append(e.ComponentArtTreeEntityMenu(0, 1, ThisCustomer.LocaleSetting, CommonLogic.QueryStringUSInt("EntityID")));
                break;
        }

        tmpS.Append("</siteMap>");

        try
        {
            ddEntity.SelectedValue = entity.ToUpperInvariant();
        }
        catch { }
        
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(tmpS.ToString());
        PageTree.LoadXml(doc);
    }

    protected void ddEntity_SelectedIndexChanged(object sender, EventArgs e)
    {
        ThisCustomer.ThisCustomerSession.SetVal("EntityDDSelected", "1");
        LoadTree();
    }

    protected void PageTree_NodeDataBound(object sender, ComponentArt.Web.UI.TreeViewNodeDataBoundEventArgs e)
    {
        e.Node.TemplateId = "SRenderHtml";
    }

    protected void btnExpandAll_Click(object sender, EventArgs e)
    {
        TreeStatus(true);
    }
    protected void btnCollapseAll_Click(object sender, EventArgs e)
    {
        TreeStatus(false);
    }

    private void TreeStatus(bool expand)
    {
        LoadTree();
        if (expand)
        {
            PageTree.ExpandAll();
            btnCollapseAll.Visible = true;
            btnExpandAll.Visible = false;
        }
        else
        {
            PageTree.CollapseAll();
            btnCollapseAll.Visible = false;
            btnExpandAll.Visible = true;
        }
    }
}

public class RenderHtmlServerTemplate : ITemplate
{
    public void InstantiateIn(Control container)
    {
        ComponentArt.Web.UI.NavigationTemplateContainer templateContainer = (ComponentArt.Web.UI.NavigationTemplateContainer)container;
        container.Controls.Add(new LiteralControl(templateContainer.Attributes["Text"]));
    }
}