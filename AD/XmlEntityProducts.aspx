<%@ Page Language="C#" AutoEventWireup="false" %>
<%
    Response.Clear();
    Response.ClearHeaders();
    Response.ContentType = "text/xml";

    AspDotNetStorefrontCommon.Customer cust = new AspDotNetStorefrontCommon.Customer(true);
    
    string sql = "SELECT P.ProductID,P.Name FROM Product P, ";
    string entity = AspDotNetStorefrontCommon.CommonLogic.QueryStringCanBeDangerousContent("entityName").ToUpperInvariant();
    int eID = AspDotNetStorefrontCommon.CommonLogic.QueryStringNativeInt("entityid");
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
    sql += "Product" + entity + " X WHERE X." + entity + "ID=" + eID + " AND X.ProductID=P.ProductID and P.Deleted = 0 ORDER BY X.DisplayOrder, P.Name";

    ComponentArt.Web.UI.TreeView TreeView1 = new ComponentArt.Web.UI.TreeView();
    System.Data.IDataReader dr = AspDotNetStorefrontCommon.DB.GetRS(sql);
    while (dr.Read())
    {
        ComponentArt.Web.UI.TreeViewNode node = new ComponentArt.Web.UI.TreeViewNode();
        //string msg = "&#8226;&#160;&lt;a target=entityBody href=entityEditProductVariant.aspx?ProductID=" + AspDotNetStorefrontCommon.DB.RSFieldInt(dr, "ProductID") + "&entityname=" + entity + "&EntityID=" + eID + "&Variantid=0&gt;Add&lt;/a&gt;&lt;br /&gt;&#8226;&#160;&lt;a target=entityBody href=entityProductVariantsOverview.aspx?ProductID=" + AspDotNetStorefrontCommon.DB.RSFieldInt(dr, "ProductID") + "&entityname=" + entity + "&EntityID=" + eID + "&gt;Variants&lt;/a&gt;";
        //node.Text = "<span onMouseout=\"delayhidetip()\" onMouseover=\"fixedtooltip('" + msg + "', this, event, '100px')\"><font color=\"green\">" + AspDotNetStorefrontCommon.DB.RSField(dr, "Name") + "</font></span>";
        node.Text = "<font color=\"green\">" + AspDotNetStorefrontCommon.DB.RSFieldByLocale(dr, "Name", cust.LocaleSetting) + "</font>";
        node.ContentCallbackUrl = "XmlEntityProductVariants.aspx?pID=" + AspDotNetStorefrontCommon.DB.RSFieldInt(dr, "ProductID") + "&entityName=" + entity + "&entityID=" + eID; ;
        node.NavigateUrl = "entityEditProducts.aspx?iden=" + AspDotNetStorefrontCommon.DB.RSFieldInt(dr, "ProductID") + "&entityName=" + entity + "&entityFilterID=" + eID;
        node.Target = "entityBody";
        TreeView1.Nodes.Add(node);
    }
    dr.Close();

    ComponentArt.Web.UI.TreeViewNode nodeAdd = new ComponentArt.Web.UI.TreeViewNode();
    nodeAdd.Text = "<font color=\"red\">Add Product</font>";
    nodeAdd.NavigateUrl = "entityEditProducts.aspx?iden=0&entityName=" + entity + "&entityFilterID=" + eID;
    nodeAdd.Target = "entityBody";
    TreeView1.Nodes.Add(nodeAdd);

    Response.Write(TreeView1.GetXml());
    Response.End();
 %>