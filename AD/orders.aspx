<%@ Page EnableViewState="true" language="c#" Inherits="AspDotNetStorefrontAdmin.orders" CodeFile="orders.aspx.cs" EnableEventValidation="false" %>
<%@ Register TagPrefix="ew" Namespace="eWorld.UI" Assembly="eWorld.UI" %>
<html>
<head>
<title>Order Review</title>
</head>
<body>
<Form id="frmOrder" name="frmOrder" method="post" runat="server">
	<TABLE cellSpacing="0" cellPadding="1" width="100%" border="1">
		<TR>
			<TH width="50%" bgColor="silver">
				<asp:literal id="Literal1" runat="server" Text="DateRange:"></asp:literal></TH>
			<TH bgColor="silver">
				<asp:literal id="Literal2" runat="server" Text="OrderQualifiers:"></asp:literal></TH></TR>
		<TR>
			<TD align="center" bgColor="#ffffcc">
				<TABLE height="10%" width="100%">
					<TR>
						<TD align="right" width="50%">StartDate :
						</TD>
						<TD>
							<ew:calendarpopup id="dateStart" runat="server" Height="20px" DisableTextboxEntry="False" AllowArbitraryText="False"
								padsingledigits="True" nullable="True" calendarwidth="200" Width="80px" showgototoday="True" imageurl="skins\Skin_1\images\calendar.gif"
								Font-Size="9px">
								<weekdaystyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Black"
									backcolor="Gainsboro"></weekdaystyle>
								<monthheaderstyle font-size="Small" font-names="Verdana,Helvetica,Tahoma,Arial" font-bold="True" forecolor="White"
									backcolor="Gray"></monthheaderstyle>
								<offmonthstyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Gainsboro"
									backcolor="Gainsboro"></offmonthstyle>
								<gototodaystyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Black"
									backcolor="White"></gototodaystyle>
								<todaydaystyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="MediumBlue"
									backcolor="Gainsboro"></todaydaystyle>
								<dayheaderstyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Black"
									backcolor="Azure"></dayheaderstyle>
								<weekendstyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Red"
									backcolor="Gainsboro"></weekendstyle>
								<selecteddatestyle borderstyle="Inset" font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial"
									borderwidth="2px" forecolor="MediumBlue" backcolor="Silver"></selecteddatestyle>
								<cleardatestyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Black"
									backcolor="White"></cleardatestyle>
							</ew:calendarpopup></TD>
					</TR>
					<TR>
						<TD align="right">EndDate :
						</TD>
						<TD>
							<ew:calendarpopup id="dateEnd" runat="server" Height="20px" DisableTextboxEntry="False" AllowArbitraryText="False"
								padsingledigits="True" nullable="True" calendarwidth="200" Width="80px" showgototoday="True" imageurl="skins\Skin_1\images\calendar.gif"
								Font-Size="9px">
								<weekdaystyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Black"
									backcolor="Gainsboro"></weekdaystyle>
								<monthheaderstyle font-size="Small" font-names="Verdana,Helvetica,Tahoma,Arial" font-bold="True" forecolor="White"
									backcolor="Gray"></monthheaderstyle>
								<offmonthstyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Gainsboro"
									backcolor="Gainsboro"></offmonthstyle>
								<gototodaystyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Black"
									backcolor="White"></gototodaystyle>
								<todaydaystyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="MediumBlue"
									backcolor="Gainsboro"></todaydaystyle>
								<dayheaderstyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Black"
									backcolor="Azure"></dayheaderstyle>
								<weekendstyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Red"
									backcolor="Gainsboro"></weekendstyle>
								<selecteddatestyle borderstyle="Inset" font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial"
									borderwidth="2px" forecolor="MediumBlue" backcolor="Silver"></selecteddatestyle>
								<cleardatestyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Black"
									backcolor="White"></cleardatestyle>
							</ew:calendarpopup></TD>
					</TR>
					<tr>
						<td align="right" valign="top">Report Type:</td>
						<td>
								<asp:RadioButton id="RegularReport" runat="server" GroupName="ReportTypeGroup" Text="Regular Report"
									Checked="True"></asp:RadioButton><br/>
								<asp:RadioButton id="BulkPrintingReport" runat="server" GroupName="ReportTypeGroup" Text="Bulk Printing Report"></asp:RadioButton><br />
								<asp:RadioButton id="SummaryReport" runat="server" GroupName="ReportTypeGroup" Text="Summary Report"></asp:RadioButton>
						</td>
					</tr>
</TABLE>
				<HR/>
				<asp:radiobutton id="rbRange" runat="server" groupname="rbEasyRange" text="UseDateRangeAbove"
					Checked="True"></asp:radiobutton>
				<TABLE width="80%">
					<TR>
						<TD colSpan="2">
							<asp:radiobuttonlist id="rbEasyRange" runat="server" Width="328px" repeatcolumns="2">
								<asp:listitem Value="Today">Today</asp:listitem>
								<asp:listitem Value="ThisWeek">ThisWeek</asp:listitem>
								<asp:listitem Value="ThisMonth">ThisMonth</asp:listitem>
								<asp:listitem Value="ThisYear">ThisYear</asp:listitem>
								<asp:listitem Value="Yesterday">Yesterday</asp:listitem>
								<asp:listitem Value="LastWeek">LastWeek</asp:listitem>
								<asp:listitem Value="LastMonth">LastMonth</asp:listitem>
								<asp:listitem Value="LastYear">LastYear</asp:listitem>
							</asp:radiobuttonlist></TD>
					</TR>
				</TABLE>
			</TD>
			<TD vAlign="top" bgColor="#ccffff">
				<TABLE id="Table1" cellSpacing="0" cellPadding="2" width="100%" align="center" border="0">
					<TR>
						<TD>
							<asp:literal id="Literal3" runat="server" Text="Order Number/Transaction ID/Subscription ID:"></asp:literal></TD>
						<TD>
							<asp:textbox id="txtOrderNumber" runat="server" Width="150px"></asp:textbox></TD>
					</TR>
					<TR>
						<TD>Customer ID:</TD>
						<TD>
							<ew:numericbox id="txtCustomerID" runat="server" Width="150px" PositiveNumber="True" RealNumber="False"></ew:numericbox></TD>
					</TR>
					<TR>
						<TD>Customer E-Mail:</TD>
						<TD>
							<asp:textbox id="txtEMail" runat="server" width="150px"></asp:textbox></TD>
					</TR>
					<TR>
						<TD>Credit Card # (or Last4):</TD>
						<TD>
							<asp:textbox id="txtCreditCardNumber" runat="server" width="150px"></asp:textbox></TD>
					</TR>
					<TR>
						<TD>Customer Name:</TD>
						<TD>
							<asp:textbox id="txtCustomerName" runat="server" width="150px"></asp:textbox></TD>
					</TR>
					<TR>
						<TD>Company:</TD>
						<TD>
							<asp:textbox id="txtCompany" runat="server" width="150px"></asp:textbox></TD>
					</TR>
					<TR>
						<TD>Payment Method:</TD>
						<TD>
							<asp:dropdownlist id="ddPaymentMethod" runat="server" Width="150px">
								<asp:listitem Value="-" Selected="True">All Types</asp:listitem>
								<asp:listitem Value="CREDIT CARD">Credit Card</asp:listitem>
								<asp:listitem Value="PAYPAL">PayPal</asp:listitem>
								<asp:listitem Value="PAYPALEXPRESS">PayPal Express</asp:listitem>
								<asp:listitem Value="PURCHASE ORDER">Purchase Order</asp:listitem>
								<asp:listitem Value="REQUEST QUOTE">Request Quote</asp:listitem>
								<asp:listitem Value="CHECK BY MAIL">Check</asp:listitem>
								<asp:listitem Value="COD">C.O.D.</asp:listitem>
								<asp:listitem Value="CODMONEYORDER">C.O.D. Money Order</asp:listitem>
								<asp:listitem Value="CODCOMPANYCHECK">C.O.D. Company Check</asp:listitem>
								<asp:listitem Value="CODNET30">C.O.D. Net 30</asp:listitem>
								<asp:listitem Value="ECHECK">E-Check</asp:listitem>
								<asp:listitem Value="MICROPAY">MicroPay</asp:listitem>
							</asp:dropdownlist></TD>
					</TR>
					<TR>
						<TD>Transaction State:</TD>
						<TD>
							<asp:dropdownlist id="TransactionState" runat="server">
								<asp:listitem Value="-" Selected="True">All States</asp:listitem>
								<asp:listitem Value="AUTHORIZED">AUTHORIZED</asp:listitem>
								<asp:listitem Value="CAPTURED">CAPTURED</asp:listitem>
								<asp:listitem Value="VOIDED">VOIDED</asp:listitem>
								<asp:listitem Value="REFUNDED">REFUNDED</asp:listitem>
								<asp:listitem Value="FRAUD">FRAUD</asp:listitem>
								<asp:listitem Value="PENDING">PENDING</asp:listitem>
							</asp:dropdownlist></TD>
					</TR>
					<TR>
						<TD>Transaction Type:</TD>
						<TD>
							<asp:dropdownlist id="TransactionType" runat="server">
								<asp:listitem Value="-" Selected="True">All Types</asp:listitem>
								<asp:listitem Value="UNKNOWN">UNKNOWN</asp:listitem>
								<asp:listitem Value="CHARGE">CHARGE</asp:listitem>
								<asp:listitem Value="CREDIT">CREDIT</asp:listitem>
								<asp:listitem Value="RECURRING_AUTO">RECURRING_AUTO</asp:listitem>
							</asp:dropdownlist></TD>
					</TR>
					<TR id="ProductMatchRow" runat="server">
						<TD>Product:</TD>
						<TD>
							<asp:dropdownlist id="ProductMatch" runat="server">
							</asp:dropdownlist></TD>
					</TR>
					<TR>
						<TD>New Orders Only:</TD>
						<TD>
							<asp:radiobuttonlist id="rbNewOrdersOnly" runat="server" Width="150px" RepeatLayout="Flow" RepeatDirection="Horizontal">
								<asp:listitem Value="0">No</asp:listitem>
								<asp:listitem Value="1" Selected="True">Yes</asp:listitem>
							</asp:radiobuttonlist></TD>
					</TR>
					<TR>
						<TD>Affiliate:</TD>
						<TD>
							<asp:dropdownlist id="ddAffiliate" runat="server" Width="150px" datatextfield="Name" datavaluefield="AffiliateID"></asp:dropdownlist></TD>
					</TR>
					<TR>
						<TD>Coupon Code:</TD>
						<TD>
							<asp:dropdownlist id="ddCouponCode" runat="server" Width="150px" datatextfield="CouponCode" datavaluefield="CouponCode"></asp:dropdownlist></TD>
					</TR>
					<TR>
						<TD>Ship to State:</TD>
						<TD>
							<asp:dropdownlist id="ddShippingState" runat="server" Width="150px" datatextfield="Name" datavaluefield="Abbreviation"></asp:dropdownlist></TD>
					</TR>
				</TABLE>
			</TD>
		</TR>
		<TR>
			<TH align="right" bgColor="#cccccc">
				<asp:button id="btnSubmit" runat="server" CssClass="normalButton" Text="Submit" OnClick="btnSubmit_Click"></asp:button></TH>
			<TH align="left" bgColor="#cccccc">
				</TH></TR>
	</TABLE>
	<asp:label id="lblError" runat="server" Width="100%" Font-Bold="True"></asp:label>
	<asp:panel id="pnlRegularReport" runat="server" Visible="False">
		<TABLE cellSpacing="0" cellPadding="1" width="100%" border="0">
			<TR>
				<TD vAlign="top" align="center" width="210" height="1024"><br/>
					<TABLE cellSpacing="0" cellPadding="0" border="0">
						<TR>
							<TD>
								<TABLE id="tblHeader" cellSpacing="0" cellPadding="0" border="0">
									<TR>
										<TD>
											<asp:image id=Image1 runat="server" ImageUrl='<%# DataBinder.Eval(Page,"HeaderImage") %>'>
											</asp:image></TD>
									</TR>
								</TABLE>
							</TD>
						</TR>
						<TR>
							<TD>
								<asp:datalist id="dlSelected" runat="server" Width="100%" ItemStyle-BorderWidth="1" ItemStyle-BorderStyle="Solid"
									BorderStyle="None">
									<itemstyle borderwidth="1px" borderstyle="Solid"></itemstyle>
									<itemtemplate>
										<asp:label id="lblOrderDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"OrderDate","{0:d}") %>'>
										</asp:label>
										<asp:hyperlink id="hlOrderNumber" runat="server" NavigateUrl='<%# DataBinder.Eval(Container.DataItem,"OrderNumber","orderframe.aspx?ordernumber={0}") %>' Target="orderframe">
											<%# DataBinder.Eval(Container.DataItem,"OrderNumber") %>
										</asp:hyperlink>
										<asp:image id="imgNew" runat="server" ImageUrl='<%# DataBinder.Eval(Page, "NewImage") %>' Visible='<%# (DataBinder.Eval(Container.DataItem,"IsNew").ToString()=="1") %>' ImageAlign="AbsMiddle">
										</asp:image>
									</itemtemplate>
									<headerstyle borderwidth="0px" borderstyle="None" backcolor="White"></headerstyle>
								</asp:datalist></TD>
						</TR>
					</TABLE>
					</TD>
				<TD vAlign="top"><IFRAME id="orderframe" name="orderframe" src='<%# DataBinder.Eval(Page,"FirstOrderNumber","orderframe.aspx?ordernumber={0}") %>' frameBorder=0 width="100%" height="100%">
					</IFRAME></TD>
			</TR>
		</TABLE>
	</asp:panel>
	<asp:panel id="pnlBulkPrintingReport" runat="server" Visible="False">Bulk Printing Report<BR />&nbsp;<asp:Literal id="Literal4" runat="server"></asp:Literal></asp:panel>
	<asp:panel id="pnlSummaryReport" runat="server" Visible="False">
		Summary Report<br/>
<asp:DataGrid id="SummaryGrid" runat="server" CellPadding="2">
			<ItemStyle Font-Size="X-Small"></ItemStyle>
			<HeaderStyle Font-Bold="True"></HeaderStyle>
		</asp:DataGrid>
	</asp:panel>
</Form>
</body>
</html>
