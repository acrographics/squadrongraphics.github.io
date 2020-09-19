<%@ Page Language="C#" AutoEventWireup="true" CodeFile="topics.aspx.cs" Inherits="topics" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Coupons</title>
    <asp:Literal runat="server" ID="ltStyles"></asp:Literal>
    <script type="text/javascript"  src="Editor/scripts/innovaeditor.js"></script>
    <!--<script type="text/javascript" src="jscripts/formValidate.js"></script>-->
    <script type="text/javascript"  src="jscripts/toolTip.js">
        /***********************************************
        * Cool DHTML tooltip script II- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
        * This notice MUST stay intact for legal use
        * Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
        ***********************************************/
    </script>
</head>
<body>
    <form id="frmTopics" runat="server">
    <asp:Literal ID="ltScript" runat="server"></asp:Literal>
    <asp:Literal ID="ltValid" runat="server"></asp:Literal>
    <div id="help">
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
                                        Manage Topics
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
        <div style="margin-bottom: 5px; margin-top: 5px;">
            <asp:Literal ID="ltError" runat="server"></asp:Literal>
        </div>
    </div>
    <div id="content">
        <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
            <tr>
                <td>
                    <div class="wrapper">                       
                        <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                            <tr>
                                <td class="titleTable" width="150">
                                    <font class="subTitle">Topic List:</font>
                                </td>
                                <td style="width: 5px;" />
                                <td style="width: 1px; background-color: #EBEBEB;" />
                                <td style="width: 5px;" />
                                <td class="titleTable">
                                    <font class="subTitle">Topic Details:</font>                                  
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="150">
                                    <div class="wrapperTop">
                                        <asp:Button runat="server" ID="btnAdd" CssClass="normalButton" Text="ADD NEW" OnClick="btnAdd_Click" />                                   
                                    </div>
                                    <div class="wrapperTopBottom">
                                        <font class="subTitleSmall">
                                            Show for Skin:
                                        </font>
                                        <asp:DropDownList ID="ddSkins" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddSkins_SelectedIndexChanged"></asp:DropDownList>
                                        <br />
                                        <font class="subTitleSmall">
                                            Show Locale Setting:
                                        </font>
                                        <asp:DropDownList ID="ddLocales" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddLocales_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                    <div class="wrapperBottom">
                                        <asp:TreeView ID="treeMain" runat="server" OnSelectedNodeChanged="treeMain_SelectedNodeChanged">
                                        </asp:TreeView>
                                    </div>
                                </td>
                                <td style="width: 5px;" />
                                <td style="width: 1px; background-color: #EBEBEB;" />
                                <td style="width: 5px;" />
                                <td class="contentTable" valign="top" width="*">
                                    <div class="wrapperLeft">
                                        <asp:PlaceHolder ID="phMain" runat="server">
                                            <font class="titleMessage">
                                                <asp:Literal runat="server" ID="ltMode"></asp:Literal> Topic&nbsp;&nbsp;Select Locale  <asp:DropDownList ID="ddlPageLocales" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlPageLocales_SelectedIndexChanged"></asp:DropDownList>
                                            </font>
                                            <p>
                                                Please enter the following information about this topic. Fields marked with an asterisk (*) are required. All other fields are optional.
                                            </p>                                            
                                            <table width="100%" cellpadding="1" cellspacing="0" border="0">
                                                <tr>
                                                    <td width="260" align="right" valign="top">
                                                        <font class="subTitleSmall">*Topic Name:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox runat="server" ID="ltName"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="top">
                                                        <font class="subTitleSmall">*Topic Title:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox runat="server" ID="ltTitle"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="middle">
                                                        <font class="subTitleSmall">*Applies Only To Skin:</font>
                                                    </td>
                                                    <td align="left" valign="top">
                                                        <asp:TextBox ID="txtSkin" runat="server" CssClass="singleShortest"></asp:TextBox>
                                                        <img src="images/info.gif" border="0" onmouseout="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Leave blank to allow topic to apply to all skins!</font>', 300)" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="middle">
                                                        <font class="subTitleSmall">Display Order:</font>
                                                    </td>
                                                    <td align="left" valign="top">
                                                        <asp:TextBox ID="txtDspOrdr" runat="server" CssClass="singleShortest"></asp:TextBox>
                                                    </td>
                                                </tr>                                                
                                                <tr>
                                                    <td align="right" valign="top" height="380">
                                                        <font class="subTitleSmall">Description:</font>
                                                    </td>
                                                    <td align="left" valign="top">
                                                        <ed:RadEditorWrapper runat="server" id="radDescription"></ed:RadEditorWrapper>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="top">
                                                        <font class="subTitleSmall">Search Engine Page Title:</font>
                                                    </td>
                                                    <td align="left" valign="top">
                                                        <asp:TextBox runat="server" ID="ltSETitle"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="top">
                                                        <font class="subTitleSmall">Search Engine Keywords:</font>
                                                    </td>
                                                    <td align="left" valign="top">
                                                        <asp:TextBox runat="server" ID="ltSEKeywords"></asp:TextBox>                                                    
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="top">
                                                        <font class="subTitleSmall">Search Engine Description:</font>
                                                    </td>
                                                    <td align="left" valign="top">
                                                        <asp:TextBox runat="server" ID="ltSEDescription"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="middle">
                                                        <font class="subTitleSmall">Password:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox ID="txtPassword" runat="server"></asp:TextBox>
                                                        <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Only required if you want to protect this topic content by requiring a password to be entered.</font>', 300)" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="middle">
                                                        <font class="subTitleSmall">Requires Subscription:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:RadioButtonList ID="rbSubscription" runat="server" RepeatDirection="horizontal">
                                                            <asp:ListItem Value="0" Selected="true">No</asp:ListItem>
                                                            <asp:ListItem Value="1">Yes</asp:ListItem>                                                            
                                                        </asp:RadioButtonList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="middle">
                                                        <font class="subTitleSmall">HTML OK:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:RadioButtonList ID="rbHTML" runat="server" RepeatDirection="horizontal">
                                                            <asp:ListItem Value="0" Selected="true">No</asp:ListItem>
                                                            <asp:ListItem Value="1">Yes</asp:ListItem>                                                            
                                                        </asp:RadioButtonList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="middle">
                                                        <font class="subTitleSmall">Requires Disclaimer:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:RadioButtonList ID="rbDisclaimer" runat="server" RepeatDirection="horizontal">
                                                            <asp:ListItem Value="0" Selected="true">No</asp:ListItem>
                                                            <asp:ListItem Value="1">Yes</asp:ListItem>                                                            
                                                        </asp:RadioButtonList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="middle">
                                                        <font class="subTitleSmall">Publish In Site Map:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:RadioButtonList ID="rbPublish" runat="server" RepeatDirection="horizontal">
                                                            <asp:ListItem Value="0" Selected="true">No</asp:ListItem>
                                                            <asp:ListItem Value="1">Yes</asp:ListItem>                                                            
                                                        </asp:RadioButtonList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="middle">
                                                        <font class="subTitleSmall">Page BG Color:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox ID="txtPageBG" runat="server" CssClass="singleShorter"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="middle">
                                                        <font class="subTitleSmall">Contents BG Color:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox ID="txtContentsBG" runat="server" CssClass="singleShorter"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="middle">
                                                        <font class="subTitleSmall">Skin Graphics Color:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox ID="txtSkinColor" runat="server" CssClass="singleShorter"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                            <div style="width: 100%; text-align: center;">
                                                &nbsp;&nbsp;<asp:Button ID="btnSubmit" runat="server" CssClass="normalButton" OnClick="btnSubmit_Click" />
                                                &nbsp;&nbsp;<asp:Button ID="btnDelete" runat="server" CssClass="normalButton" OnClick="btnDelete_Click" Text="Delete Topic" />
                                            </div>
                                        </asp:PlaceHolder>
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
</body>
</html>
