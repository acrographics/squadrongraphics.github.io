<%@ Page Language="C#" AutoEventWireup="false" %>
<%
    Response.Clear();
    Response.ClearHeaders();
    Response.ContentType = "text/xml";

    AspDotNetStorefrontCommon.Customer cust = ((AspDotNetStorefrontCommon.AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
    
    string entity = AspDotNetStorefrontCommon.CommonLogic.QueryStringCanBeDangerousContent("entityName").ToUpperInvariant();
    int eID = AspDotNetStorefrontCommon.CommonLogic.QueryStringNativeInt("EntityID");
    int pID = AspDotNetStorefrontCommon.CommonLogic.QueryStringNativeInt("pID");
    switch (entity)
    {
        case "SECTION":
            entity = "Section";
            break;
        case "MANUFACTURER":
            entity = "Manufacturer";
            break;
        case "DISTRIBUTOR":
            entity = "Distributor";
            break;
        case "GENRE":
            entity = "Genre";
            break;
        case "VECTOR":
            entity = "Vector";
            break;
        case "LIBRARY":
            entity = "Category";
            break;
        default:
            entity = "Category";
            break;
    }

    string sql = "SELECT VariantID,[Name] FROM ProductVariant ";
    sql += " WHERE ProductID=" + pID + " ORDER BY DisplayOrder, Name";

    ComponentArt.Web.UI.TreeView TreeView1 = new ComponentArt.Web.UI.TreeView();

    ComponentArt.Web.UI.TreeViewNode nodeAll = new ComponentArt.Web.UI.TreeViewNode();
    nodeAll.Text = "<font color=\"Maroon\">All Variants</font>";
    nodeAll.NavigateUrl = "entityProductVariantsOverview.aspx?ProductID=" + pID + "&entityname=" + entity + "&EntityID=" + eID;
    nodeAll.Target = "entityBody";
    TreeView1.Nodes.Add(nodeAll);
    
    System.Data.IDataReader dr = AspDotNetStorefrontCommon.DB.GetRS(sql);
    while (dr.Read())
    {
        ComponentArt.Web.UI.TreeViewNode node = new ComponentArt.Web.UI.TreeViewNode();
        string name = AspDotNetStorefrontCommon.DB.RSFieldByLocale(dr, "Name", cust.LocaleSetting);
        node.Text = "<font color=\"Maroon\">" + (name.Trim().Length == 0 ? "(Unnamed Variant)" : name.Trim()) + "</font>";
        node.NavigateUrl = "entityEditProductVariant.aspx?ProductID=" + pID + "&entityname=" + entity + "&EntityID=" + eID + "&Variantid=" + AspDotNetStorefrontCommon.DB.RSFieldInt(dr, "VariantID");
        node.Target = "entityBody";
        TreeView1.Nodes.Add(node);
    }
    dr.Close();

    ComponentArt.Web.UI.TreeViewNode nodeAdd = new ComponentArt.Web.UI.TreeViewNode();
    nodeAdd.Text = "<font color=\"red\">Add Variant</font>";
    nodeAdd.NavigateUrl = "entityEditProductVariant.aspx?ProductID=" + pID + "&entityname=" + entity + "&EntityID=" + eID + "&Variantid=0";
    nodeAdd.Target = "entityBody";
    TreeView1.Nodes.Add(nodeAdd);
    
    Response.Write(TreeView1.GetXml());
    Response.End();
 %>