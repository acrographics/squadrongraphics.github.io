<%@ Page Language="C#" AutoEventWireup="true" CodeFile="viewshipment.aspx.cs" Inherits="AspDotNetStorefrontAdmin.viewshipment" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>WorldShip Import</title>
</head>
<body>
    <form id="WorldShip" runat="server">
        <table style="width: 402px">
            <tr>
                <td style="width: 100px">
                    </td>
            </tr>
            <tr>
                <td style="width: 100px">
                    <asp:GridView ID="dview" runat="server" AllowPaging="True" AllowSorting="True" CssClass="overallGrid" HorizontalAlign="Left" Width="50%" PageSize="50">
                        <PagerSettings FirstPageText="&amp;lt;&amp;lt;First Page" LastPageText="Last Page&amp;gt;&amp;gt;"
                            Mode="NextPreviousFirstLast" Position="TopAndBottom" />
                        <FooterStyle CssClass="footerGrid" />
                        <EditRowStyle CssClass="DataCellGridEdit" />
                        <SelectedRowStyle CssClass="DataCellGrid" />
                        <PagerStyle CssClass="pagerGrid" />
                        <HeaderStyle Font-Bold="True" />
                        <AlternatingRowStyle CssClass="DataCellGridAlt" />
                    </asp:GridView>                     
                </td>
            </tr>
            <tr>
                <td style="width: 100px">
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
