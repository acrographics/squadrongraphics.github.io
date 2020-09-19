<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.fraudorders" CodeFile="fraudorders.aspx.cs" EnableEventValidation="false"%>
<form id="Form1" method="post" runat="server">
	<P>&nbsp;
		<TABLE id="Table1" cellSpacing="1" cellPadding="4" width="300" border="0">
			<TR>
				<TD height="32">
					<asp:Label id="Label1" runat="server" Width="88px">Report Date:</asp:Label>
					<asp:DropDownList id="DateSelect" runat="server"></asp:DropDownList>&nbsp;<INPUT type="submit" value="Go"></TD>
			</TR>
			<TR>
				<TD>
					<asp:DataGrid id="DataGrid1" runat="server" CellPadding="4" CellSpacing="1"></asp:DataGrid></TD>
			</TR>
		</TABLE>
	</P>
</form>
