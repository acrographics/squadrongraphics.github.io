<%@ Page Language="C#" AutoEventWireup="true" CodeFile="entityMenu.aspx.cs" Inherits="entityMenu" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ OutputCache  Duration="1"  Location="none" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Entity Menu</title>
</head>
<body>
    <form id="frmEntityMenu" runat="server">
    <div id="help">
        <table border="0" cellpadding="1" cellspacing="0" class="outerTable">
            <tr>
                <td>
                    <div class="wrapper">
                        <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                            <tr>                            
                                <td class="titleTable">
                                    <font class="subTitle">View:</font>
                                </td>
                                <td class="contentTable">
                                    <a href="splash.aspx" target="content">Home</a>
                                </td>
                            </tr>
                            <tr>
                                <td class="titleTable">
                                    <font class="subTitle">Entity:</font>
                                </td>
                                <td class="contentTable">
                                    <font class="title">
                                        <asp:Literal ID="ltEntity" runat="server"></asp:Literal>
                                    </font>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
        <div style="margin-bottom: 5px; margin-top: 5px;">
            <asp:Literal ID="ltError" runat="server"></asp:Literal>
        </div>
    </div>
    <div id="content">
        <table border="0" cellpadding="1" cellspacing="0" class="outerTable">
            <tr>
                <td>
                    <div class="wrapperNR">                       
                        <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                            <tr>
                                <td class="titleTable">
                                    <font class="subTitle">Select:</font>                                    
                                </td>
                                <td class="contentTable">
                                    <asp:DropDownList ID="ddEntity" runat="server" OnSelectedIndexChanged="ddEntity_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTableAPR" valign="top" colspan="2">
                                    <div style="white-space: nowrap; overflow: visible;">                                         
                                        <div style="margin-top: 5px; margin-bottom: 0px; white-space: nowrap; overflow: visible;">
                                            <span style="padding-left: 0px;">
                                                <asp:LinkButton ID="btnExpandAll" runat="server" Text="Expand Tree" OnClick="btnExpandAll_Click"></asp:LinkButton>
                                                <asp:LinkButton ID="btnCollapseAll" runat="server" Text="Collapse Tree" OnClick="btnCollapseAll_Click"></asp:LinkButton>
                                            </span>
                                            <div style="position: relative; left: -5px; white-space: nowrap; overflow: visible;">
                                            <ComponentArt:TreeView id="PageTree" 
	                                          ClientScriptLocation="skins/componentart_webui_client/"
                                              DragAndDropEnabled="false" 
                                              NodeEditingEnabled="false" 
                                              KeyboardEnabled="true"
                                              CssClass="TreeView" 
                                              NodeCssClass="TreeNode" 
                                              HoverNodeCssClass="HoverTreeNode" 
                                              SelectedNodeCssClass="SelectedTreeNode" 
                                              NodeEditCssClass="NodeEdit"                                               
                                              NodeLabelPadding="0"
                                              ItemSpacing="0" 
                                              NodeIndent="0" 
                                              ImagesBaseUrl="image/lines" 
                                              LineImagesFolderUrl="images/lines" 
                                              ShowLines="true" 
                                              ParentNodeImageUrl="" 
                                              LeafNodeImageUrl="" 
                                              EnableViewState="false" 
                                              DefaultTarget="entityBody" 
                                              MarginCssClass="TreeMargin" NodeRowCssClass="TreeRow" ExpandNodeOnSelect="false" 
                                              DefaultMarginImageHeight="0"
			                                  runat="server" OnNodeDataBound="PageTree_NodeDataBound">
			                                  <%--<ClientTemplates>
			                                    <ComponentArt:ClientTemplate ID="CRenderHtml">
                                                    ## DataItem.Text.replace('&lt;', '<'); ##
                                                </ComponentArt:ClientTemplate>
			                                  </ClientTemplates>--%>
                                            </ComponentArt:TreeView>
                                            </div>
                                        </div>                                                                            
                                    </div>                                    
                                </td>
                            </tr>
                        </table>
                    </div>                    
                </td>
            </tr>
        </table>                                    
    </div>
    </form>  
    <script type="text/javascript"  src="jscripts/fixedToolTip.js" type="text/javascript">
        /***********************************************
        * Fixed ToolTip script- © Dynamic Drive (www.dynamicdrive.com)
        * This notice MUST stay intact for legal use
        * Visit http://www.dynamicdrive.com/ for full source code
        ***********************************************/
    </script>  
    <asp:Literal ID="ltScript" runat="server"></asp:Literal>    
</body>
</html>