<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.FedExMeterRequest" CodeFile="fedexmeterrequest.aspx.cs" EnableEventValidation="false"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 
<HTML>
	<HEAD>
		<STYLE>
	BODY { FONT-SIZE: 12px; FONT-FAMILY: Trebuchet MS,Verdana,Arial,Helvetica,Sans-serif }
	TD { FONT-SIZE: 12px; FONT-FAMILY: Trebuchet MS,Verdana,Arial,Helvetica,Sans-serif }
	.error { FONT-WEIGHT: bold; FONT-SIZE: 14px; COLOR: red }
	.success { FONT-SIZE: 12px; COLOR: blue }
	</STYLE>
	</HEAD>
	<BODY>
		<FORM RUNAT="server" ID="default">
			<H3>FedEx Meter Number Request Form</H3>
			<TABLE BORDER="0" CELLSPACING="1" ID="table1">
				<TR>
					<TD COLSPAN="2">
						<H4>Account info:</H4>
					</TD>
				</TR>
				<TR>
					<TD>FedEx Account Number:</TD>
					<TD>
						<ASP:TEXTBOX ID="AccountNumber" RUNAT="server"></ASP:TEXTBOX></TD>
				</TR>
				<TR>
					<TD>FedEx Server:</TD>
					<TD>
						<ASP:TEXTBOX ID="FedExServer" RUNAT="server" WIDTH="272px">https://gateway.fedex.com/GatewayDC</ASP:TEXTBOX></TD>
				</TR>
				<TR>
					<TD COLSPAN="2">&nbsp;
						<ASP:LABEL ID="lblMeter" RUNAT="server" FONT-BOLD="True" FORECOLOR="Blue">- Your meter number will be displayed here -</ASP:LABEL></TD>
				</TR>
				<TR>
					<TD COLSPAN="2">
						<H4><B>Contact Info:</B></H4>
					</TD>
				</TR>
				<TR>
					<TD>Full name:</TD>
					<TD>
						<ASP:TEXTBOX ID="FullName" RUNAT="server"></ASP:TEXTBOX></TD>
				</TR>
				<TR>
					<TD>Company Name:</TD>
					<TD>
						<ASP:TEXTBOX ID="CompanyName" RUNAT="server"></ASP:TEXTBOX></TD>
				</TR>
				<TR>
					<TD>Department:</TD>
					<TD>
						<ASP:TEXTBOX ID="Department" RUNAT="server"></ASP:TEXTBOX></TD>
				</TR>
				<TR>
					<TD>Phone number:
					</TD>
					<TD>
						<ASP:TEXTBOX ID="PhoneNumber" RUNAT="server"></ASP:TEXTBOX></TD>
				</TR>
				<TR>
					<TD>Pager number:
					</TD>
					<TD>
						<ASP:TEXTBOX ID="PagerNumber" RUNAT="server"></ASP:TEXTBOX></TD>
				</TR>
				<TR>
					<TD>Fax number:
					</TD>
					<TD>
						<ASP:TEXTBOX ID="FaxNumber" RUNAT="server"></ASP:TEXTBOX></TD>
				</TR>
				<TR>
					<TD>EMail address:
					</TD>
					<TD>
						<ASP:TEXTBOX ID="EMail" RUNAT="server"></ASP:TEXTBOX></TD>
				</TR>
				<TR>
					<TD>
						Address:
					</TD>
					<TD>
						<ASP:TEXTBOX ID="Address" RUNAT="server"></ASP:TEXTBOX></TD>
				</TR>
				<TR>
					<TD>City:
					</TD>
					<TD>
						<ASP:TEXTBOX ID="City" RUNAT="server"></ASP:TEXTBOX></TD>
				</TR>
				<TR>
					<TD>State:
					</TD>
					<TD>
						<ASP:TEXTBOX ID="State" RUNAT="server"></ASP:TEXTBOX>&nbsp;(2 Chars only)</TD>
				</TR>
				<TR>
					<TD>Zip:
					</TD>
					<TD>
						<ASP:TEXTBOX ID="Zip" RUNAT="server"></ASP:TEXTBOX></TD>
				</TR>
				<TR>
					<TD>Country:
					</TD>
					<TD>
						<ASP:TEXTBOX ID="Country" RUNAT="server"></ASP:TEXTBOX>&nbsp;(2 Chars only)</TD>
				</TR>
				<TR>
					<TD>&nbsp;</TD>
					<TD>&nbsp;</TD>
				</TR>
				<TR>
					<TD COLSPAN="2">
						<P ALIGN="center">
							<ASP:BUTTON ID="btnSubmitRequest" RUNAT="server" CssClass="normalButton" TEXT="Submit Request" onclick="btnSubmitRequest_Click"></ASP:BUTTON></P>
					</TD>
				</TR>
			</TABLE>
			<P>
				&nbsp;<ASP:LITERAL ID="responseX" RUNAT="server" TEXT="- The full response will be displayed here -"></ASP:LITERAL></P>
			<P>&nbsp;</P>
		</FORM>
	</BODY>
</HTML>
