<%@ Page Language="C#" AutoEventWireup="true" CodeFile="sitemap2.aspx.cs" Inherits="AspDotNetStorefrontAdmin.sitemap2" Theme="default" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Site Map</title>
</head>
<body>
<form id="form1" method="post" runat="server">
	    <div id="help">
        <div>
            <table border="0" cellpadding="1" cellspacing="0" class="outerTable">
                <tr>
                    <td>
                        <div class="wrapper">
                            <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                                <tr>
                                    <td class="titleTable">
                                        <font class="subTitle">Now In:</font>
                                    </td>
                                    <td class="contentTable">
                                        <font class="title">
                                            Site Map
                                        </font>
                                    </td>
                                    <td style="width: 10px;" />
                                    <td class="titleTable">
                                        <font class="subTitle">View:</font>
                                    </td>
                                    <td class="contentTable">
                                        <a href="splash.aspx">Home</a>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="content" style="height: 390px">
		<ComponentArt:SiteMap id="SiteMap1" runat="server" LeafNodeCssClass="SiteMapLeafNode" ParentNodeCssClass="SiteMapParentNode"
			RootNodeCssClass="SiteMapRootNode" CssClass="SiteMap" TreeLineImageHeight="20" TreeLineImageWidth="19" TreeLineImagesFolderUrl="images/lines2"
			TreeShowLines="true" SiteMapLayout="Tree" Height="1000" Width="100%">
			<table>
				<ComponentArt:SiteMapTableRow>
					<ComponentArt:SiteMapTableCell RootNodes="1" valign="top" width="20%"></ComponentArt:SiteMapTableCell>
					<ComponentArt:SiteMapTableCell RootNodes="1" valign="top" width="20%"></ComponentArt:SiteMapTableCell>
					<ComponentArt:SiteMapTableCell RootNodes="1" valign="top" width="20%"></ComponentArt:SiteMapTableCell>
					<ComponentArt:SiteMapTableCell RootNodes="1" valign="top" width="20%"></ComponentArt:SiteMapTableCell>
					<ComponentArt:SiteMapTableCell valign="top" width="20%"></ComponentArt:SiteMapTableCell>
				</ComponentArt:SiteMapTableRow>
			</table>
		</ComponentArt:SiteMap>
	</div>
</form>
</body>
</html>
