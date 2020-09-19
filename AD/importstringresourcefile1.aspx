<%@ Page Language="C#" AutoEventWireup="true" CodeFile="importstringresourcefile1.aspx.cs" Inherits="AspDotNetStorefrontAdmin.importstringresourcefile1" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Upload</title>
</head>

<body>
    <form id="frmImportStringResourceFile1" runat="server" enctype="multipart/form-data" method="post">   
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
                                        <asp:Literal ID="litStage" runat="server"/>
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
        <asp:Panel ID="pnlUpload" runat="server" width="100%">
            <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                <tr>
                    <td>
                        <div class="wrapper">                       
                            <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                                <tr>
                                    <td class="titleTable" style="width: 252px">
                                        <font class="subTitle">Upload File:</font>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="contentTable" valign="top" style="width: 252px">
                                        <div class="wrapper">
                                            Select the string resource Excel file that you want to upload. This file should be on your own local computer. The file should typically be named: strings.<asp:Literal ID="Literal1" runat="server"/>.xls<br /><br />Use the 'Browse' button to help locate and select the file if you need:
                                            <br /><br />
                                            *File:
                                            <asp:FileUpload width="250" ID="fuMain" CssClass="fileUpload" runat="server" /><br />
                                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator" ControlToValidate="fuMain" ErrorMessage="!"></asp:RequiredFieldValidator><br />
                                            <asp:CheckBox ID="chkReplaceExisting" runat="server" />Replace existing<br />
                                            <asp:CheckBox ID="chkLeaveModified" runat="server" />Leave my modified strings<br /><br />
                                            <asp:Button ID="btnSubmit" CssClass="normalButton" runat="server" Text="Upload" OnClick="btnSubmit_Click" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <br /><br/>
                                            <asp:HyperLink ID="lnkBack1" runat="server" Text="Back to String Resource Manager"></asp:HyperLink>
                        </div>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlReload" runat="server" Width="100%">
            <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                    <tr>
                        <td>
                            <div class="wrapper">                       
                                <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                                    <tr>
                                        <td class="titleTable">
                                            <font class="subTitle">Reload from Server Excel file:</font>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="contentTable" valign="top" width="100%">
                                            <div class="wrapper">
                                                <asp:CheckBox ID="chkReloadReplaceExisting" runat="server" />Replace existing<br />
                                                <asp:CheckBox ID="chkReloadLeaveModified" runat="server" />Leave my modified strings<br /><br />
                                                <asp:Button ID="btnReload" CssClass="normalButton" runat="server" Text="Reload Preview" OnClick="btnReload_Click"/>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <br /><br/>
                                <asp:HyperLink ID="lnkBack2" runat="server" Text="Back to String Resource Manager"></asp:HyperLink>
                            </div>
                        </td>
                    </tr>
                </table>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
