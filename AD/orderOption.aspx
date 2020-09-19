<%@ Page Language="C#" AutoEventWireup="true" CodeFile="orderOption.aspx.cs" Inherits="orderOption" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Order Option</title>
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
    <form id="frmOrderOptions" runat="server" enctype="multipart/form-data" method="post">
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
                                        Manage Order Options
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
            <asp:Label ID="lblMsg" runat="server"></asp:Label>
        </div>
    </div>
    <div id="content">
        <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
            <tr>
                <td>
                    <div class="wrapper">                       
                        <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                            <tr>
                                <td class="titleTable" width="185">
                                    <font class="subTitle">Option List:</font>
                                </td>
                                <td style="width: 5px;" />
                                <td style="width: 1px; background-color: #EBEBEB;" />
                                <td style="width: 5px;" />
                                <td class="titleTable">
                                    <font class="subTitle">Option Details:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="185">
                                    <div class="wrapperTop">
                                        <asp:Button runat="server" ID="btnAdd" CssClass="normalButton" Text="ADD NEW" OnClick="btnAdd_Click" />                                   
                                    </div>                                    
                                    <div class="wrapperTop">
                                        <asp:TreeView ID="treeMain" runat="server" OnSelectedNodeChanged="treeMain_SelectedNodeChanged">
                                        </asp:TreeView>
                                    </div>   
                                    <div class="wrapperTop">
                                        <asp:Button id="btnUpdate" runat="server" CssClass="normalButton" Text="Update Display Order" OnClick="btnUpdate_Click" />
                                    </div>
                                </td>
                                <td style="width: 5px;" />
                                <td style="width: 1px; background-color: #EBEBEB;" />
                                <td style="width: 5px;" />
                                <td class="contentTable" valign="top" width="*">
                                    <div class="wrapperLeft">
                                        <asp:PlaceHolder ID="phMain" runat="server">
                                            <font class="titleMessage">
                                                <asp:Literal runat="server" ID="ltMode"></asp:Literal> Order Option
                                            </font>
                                            <div style="margin-top: 10px;"></div>
                                            <table width="100%" cellpadding="1" cellspacing="0" border="0">
                                                <tr>
                                                    <td width="260" align="right" valign="top">
                                                        <font class="subTitleSmall">*Option Name:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:Literal ID="ltName" runat="server"></asp:Literal>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="top">
                                                        <font class="subTitleSmall">Description:</font>
                                                    </td>
                                                    <td align="left" valign="top">
                                                        <!--<asp:Literal ID="ltDescription" runat="server"></asp:Literal>-->
                                                        <ed:RadEditorWrapper id="radDescription" runat="server"></ed:RadEditorWrapper>
                                                    </td>
                                                </tr>       
                                                <tr>
                                                    <td align="right" valign="middle">
                                                        <font class="subTitleSmall">*Default Is Checked:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:RadioButtonList ID="rbIsChecked" runat="server" RepeatDirection="horizontal">
                                                            <asp:ListItem Value="0">No</asp:ListItem>
                                                            <asp:ListItem Value="1" Selected="true">Yes</asp:ListItem>                                                            
                                                        </asp:RadioButtonList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="middle">
                                                        <font class="subTitleSmall">Cost:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox ID="txtCost" runat="server" CssClass="singleShortest"></asp:TextBox>
                                                        <img src="images/info.gif" alt="" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>In format 0.00.</font>', 300)" />
                                                    </td>
                                                </tr>  
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Tax Class:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:DropDownList ID="ddTaxClass" runat="Server"></asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                <tr>
                                                    <td align="right" valign="top">
                                                        <font class="subTitleSmall">Icon:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:FileUpload CssClass="fileUpload" ID="fuIcon" runat="Server" />
                                                        <div>
                                                            <asp:Literal ID="ltIcon" runat="server"></asp:Literal>
                                                        </div>
                                                    </td>
                                                </tr>                                              
                                            </table>
                                            <div style="width: 100%; text-align: center; padding-top: 10px;">
                                                &nbsp;&nbsp;<asp:Button ID="btnSubmit" runat="server" CssClass="normalButton" OnClick="btnSubmit_Click"  OnClientClick="return validate();" />
                                                &nbsp;&nbsp;<asp:Button ID="btnDelete" runat="server" CssClass="normalButton" OnClick="btnDelete_Click" Text="Delete Order Option" />
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
        <asp:Literal ID="ltScript2" runat="server"></asp:Literal>
    </div>
    </form>
</body>
</html>
