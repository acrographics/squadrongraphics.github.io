<%@ Page Language="C#" AutoEventWireup="true" CodeFile="stringresourcerpt.aspx.cs" Inherits="AspDotNetStorefrontAdmin.stringresourcerpt" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>String Resource Report</title>
</head>

<body>
    <form id="frmStringResourceMissing" runat="server">   
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
                                        String Resource Report
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
                                <td class="titleTable">
                                    <font class="subTitle"><asp:Label ID="ReportLabel" runat="server">Missing Strings:</asp:Label></font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="100%" style="height: 81px">
                                    <div class="wrapper">
                                        <asp:Literal ID="ltLocale" runat="server"></asp:Literal>
                                        <br /><asp:Button ID="btnExportExcel" runat="server" CssClass="normalButton" Text="Export to Excel" OnClick="btnExportExcel_Click"/><br />
                                        &nbsp;<asp:Literal ID="ltData" runat="server"></asp:Literal>
                                        <br />
                                        <asp:Button ID="btnSubmit" runat="server" CssClass="normalButton" Text="Add/Update" OnClick="btnSubmit_Click" />
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
