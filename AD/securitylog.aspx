<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.securitylog" CodeFile="securitylog.aspx.cs" EnableEventValidation="false"%>
<html>
<head>
<title>Encryption Test</title>
</head>
<body>
<form runat="server" id="form1">
    <b></b><strong>Your SecurityLog is Shown Below for the Last 365 Days:&nbsp;
        <asp:Button ID="Refresh" runat="server" OnClick="Refresh_Click" CssClass="normalButton" Text="Refresh Page" /><br /></strong>
    <br />
    <asp:GridView ID="GridView1" runat="server" AllowPaging="True" OnRowDataBound="GridView1_RowDataBound1" PageSize="100" Width="100%" OnPageIndexChanging="GridView1_PageIndexChanging">
        <HeaderStyle Font-Bold="True" Font-Size="XX-Small" HorizontalAlign="Left" VerticalAlign="Top" BackColor="#E0E0E0" />
        <RowStyle HorizontalAlign="Left" VerticalAlign="Top" />
    </asp:GridView>
    </form>
</body>
</html>


