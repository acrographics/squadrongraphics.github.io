<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.recurringimport" CodeFile="recurringimport.aspx.cs" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Recurring Import</title>
</head>

<body>
    <form id="frmRecurringImport" runat="server">   
    <asp:Literal ID="ltScript" runat="server"></asp:Literal> 
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
                                        Import Recurring Order Status From Gateway
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
    <asp:Panel ID="pnlMain" runat="server" Width="100%">
        <asp:Label ID="lblLastRun" runat="server"></asp:Label><asp:Button ID="btnGetGatewayStatus" runat="server" Text="Get Today's Status File..." OnClick="btnGetGatewayStatus_Click" />
        <asp:Button ID="btnProcessFile" runat="server" Text="Process Records" OnClick="btnProcessFile_Click" Visible="False" />
        <br />
        <br />
        <asp:Label ID="PastePromptLabel" runat="server" Text="(or) Paste Gateway AutoBill Status E-Mail Contents Below:"></asp:Label><br />
        <asp:TextBox ID="txtInputFile" runat="server" Height="600px" TextMode="MultiLine"
            Width="45%" Font-Size="Small"></asp:TextBox>&nbsp;
        <asp:TextBox ID="txtResults" runat="server" Enabled="False" Height="600px" TextMode="MultiLine"
            Visible="False" Width="50%" Font-Size="Small">{Processing Results Will Go Here}</asp:TextBox>
     </asp:Panel>
    <asp:Panel ID="pnlNotSupported" runat="server" Width="100%">
            <asp:Label ID="lblNotSupported" runat="server" Text="This Page is NOT supported for your combination gateway and AppConfig settings, or you have no active gateway AutoBill recurring orders....This feature is only in BETA at this time also. You must have all of the following to use this feature: a) Recurring Billing products, b) using Gateway AutoBill Internal Processing, and c) only PayFlow PRO, Authorize.net, and Manual gateways support this. Contact support for more information..."></asp:Label><br />
    </asp:Panel>
    </div>
    </form>
</body>
</html>
