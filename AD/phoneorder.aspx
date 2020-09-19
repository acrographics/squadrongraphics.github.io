<%@ Page Language="C#" AutoEventWireup="true" CodeFile="phoneorder.aspx.cs" Inherits="AspDotNetStorefrontAdmin.phoneorder" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Phone Order Page</title>
</head>

<script type="JavaScript">
    function getDelete()
    {
        return 'Confirm Delete';
    }
</script>
<body>
    <form id="frmAppConfig" runat="server">
    <div id="help">
        <table border="0" cellpadding="1" cellspacing="0" class="outerTable">
            <tr>
                <td>
                    <div class="wrapper">
                        <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                            <tr>
                                <td class="titleTable">
                                    <font class="subTitle">Now In: </font>
                                </td>
                                <td class="contentTable">
                                    <font class="title">Enter Phone Order</font>&nbsp;&nbsp;<a href="phoneorder.aspx">Reset</a></td>
                                <td style="width: 10px;" />
                                <td class="titleTable">
                                    <font class="subTitle">View: </font>
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
    <asp:Panel runat="server" ID="TopPanel" DefaultButton="Button2">
    <div id="content">
        <table border="0" cellpadding="0" cellspacing="0" class="outerTable" width="100%">
            <tr>
                <td>
                    <div class="wrapper">Find Existing Customer:
                        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox1"
                            ErrorMessage="!" SetFocusOnError="True" ValidationGroup="Search" Font-Bold="True"></asp:RequiredFieldValidator>
                        <asp:Button ID="Button2" runat="server" CssClass="normalButton" Text="Search" OnClick="Button2_Click1" ValidationGroup="Search" />
                        &nbsp; &nbsp;&nbsp; or &nbsp; &nbsp;&nbsp; &nbsp;<asp:Button ID="Button1" runat="server" CssClass="normalButton" Text="Create New Customer" OnClick="Button1_Click" /><br />
                        <small>(by Last Name, E-Mail, CustomerID, or Order Number)</small></div>
                </td>
            </tr>
        </table>
    </div>
    </asp:Panel>
        <asp:Panel ID="SearchCustomerPanel" runat="server" Width="100%" Visible="False">
        <asp:Literal runat="server" ID="SQLText" Visible="false"></asp:Literal><br />
            &nbsp;Matching Customers: <br />
            <asp:GridView ID="GridView1" runat="server" DataKeyNames="CustomerID" OnRowCommand="GridView1_RowCommand" AutoGenerateColumns="False" Width="50%">
                <Columns>
                    <asp:BoundField DataField="CustomerID" HeaderText="Customer ID" >
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="FirstName" HeaderText="First Name" >
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="LastName" HeaderText="Last Name" >
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="EMail" HeaderText="EMail" >
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField ButtonType="Button" CommandName="Select" Text="Select">
                        <ControlStyle Font-Size="XX-Small" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:ButtonField>
                </Columns>
            </asp:GridView>
        </asp:Panel>
        <asp:Panel ID="CreateNewCustomerPanel" runat="server" Width="100%" Visible="False">
            <table class="innerTable" cellpadding="0" width="100%">
                <tr>
                    <td>
                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                        <tr><td colspan="4" style="height: 19px"><b>Account Info:</b><div ID="CustomerIDPanel" style="display:inline;" runat="server" visible="false">&nbsp;&nbsp;<b>(Customer ID=<asp:Literal ID="CustomerID" runat="server" />)</b></div></td></tr>
                            <tr>
                                <td align="right" valign="middle">*First Name: </td>
                                <td align="left" valign="middle">
                                    <asp:TextBox ID="FirstName" runat="server" Width="200"></asp:TextBox>
                                </td>
                                <td align="right" valign="middle">*Last Name: </td>
                                <td align="left" valign="middle">
                                    <asp:TextBox ID="LastName" runat="server" Width="200"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" valign="middle" style="height: 43px">E-Mail: </td>
                                <td align="left" valign="middle" style="height: 43px">
                                    <asp:TextBox ID="EMail" runat="server" Width="200"></asp:TextBox>
                                    <br /><asp:RegularExpressionValidator ID="valRegExValEmail" ControlToValidate="EMail" runat="SERVER" ValidationExpression="^[a-zA-Z0-9][-\w\.\+]*@([a-zA-Z0-9][\w\-]*\.)+[a-zA-Z]{2,4}$" ErrorMessage="Please Enter a valid EmailAddress" />
                                    <asp:Label ID="EMailAlreadyTaken" Text="&lt;br&gt;!!!DUPLICATE E-MAILS NOT ALLOWED!!!" Visible="False" runat="server" Font-Bold="True" Font-Size="X-Small" ForeColor="Red"></asp:Label>
                                </td>
                                <td align="right" valign="middle" style="height: 43px">Phone: </td>
                                <td align="left" valign="middle" style="height: 43px"><asp:TextBox ID="Phone" runat="server" Width="200"></asp:TextBox></td>
                             </tr>
                            <tr>
                                <td align="right" valign="middle">OK To EMail: </td>
                                <td align="left" valign="middle"><asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Selected="True" Value="Yes">Yes</asp:ListItem>
                                        <asp:ListItem>No</asp:ListItem></asp:RadioButtonList>
                                        </td>
                                <td align="right" valign="middle">Over 13 Years: </td>
                                <td align="left" valign="middle"><asp:CheckBox ID="Over13" runat="server" Checked="True" /></td>
                            </tr>
                            <tr>
                                <td align="right" valign="middle"><asp:Literal ID="AffiliatePrompt" Text="Affiliate:" runat="server"></asp:Literal></td>
		                        <td width="25%"><asp:DropDownList ID="AffiliateList" runat="server" DataTextField="Name" DataValueField="AffiliateID"></asp:DropDownList></td>
                                <td align="right" valign="middle"><asp:Literal ID="CustomerLevelPrompt" Text="Customer Level:" runat="server"></asp:Literal></td>
		                        <td width="25%"><asp:DropDownList ID="CustomerLevelList" runat="server" DataTextField="Name" DataValueField="CustomerLevelID"></asp:DropDownList></td>
                            </tr>
                            <tr>
                            <td colspan="2">
                                 <br /><b>Billing Info: <asp:Button ID="btnCopyAccount" runat="server" CssClass="normalButton" Text="Copy From Account" /></b><br /><br /></td><td colspan="2"><br />
                                <b>Shipping Info: <asp:Button ID="btnCopyBilling" runat="server" CssClass="normalButton" Text="Copy From Billing" /></b><br /><br />
                                </td>
                            </tr>
	<tr>
		<td align="right">*First Name: </td>
		<td width="25%">
		    <asp:TextBox runat="server" ID="BillingFirstName" maxLength="50"></asp:TextBox>
		    </td>
		<td align="right">*First Name: </td>
		<td width="25%">
		    <asp:TextBox runat="server" ID="ShippingFirstName" maxLength="50"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td align="right">*Last Name: </td>
		<td width="25%">
		    <asp:TextBox runat="server" ID="BillingLastName" maxLength="50"></asp:TextBox>
		</td>
		<td align="right">*Last Name: </td>
		<td width="25%">
		    <asp:TextBox runat="server" ID="ShippingLastName" maxLength="50"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td align="right">*Phone: </td>
		<td width="25%">
		    <asp:TextBox runat="server" ID="BillingPhone" maxLength="25"></asp:TextBox>
		</td>
		<td align="right">*Phone: </td>
		<td width="25%">
		    <asp:TextBox runat="server" ID="ShippingPhone" maxLength="25"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td align="right">Company: </td>
		<td width="25%"><asp:TextBox runat="server" ID="BillingCompany" maxLength="100" ></asp:TextBox></td>
		<td align="right">Company: </td>
		<td width="25%"><asp:TextBox runat="server" ID="ShippingCompany" maxLength="100" ></asp:TextBox></td>
	</tr>
	<tr>
		<td align="right">Address Type: </td>
		<td width="25%"><asp:DropDownList ID="BillingResidenceType" runat="server"><asp:ListItem Value="0">Unknown</asp:ListItem><asp:ListItem Value="1" selected=True>Residential</asp:ListItem><asp:ListItem Value="2">Commercial</asp:ListItem></asp:DropDownList></td>
		<td align="right">Address Type: </td>
		<td width="25%"><asp:DropDownList ID="ShippingResidenceType" runat="server"><asp:ListItem Value="0">Unknown</asp:ListItem><asp:ListItem Value="1" selected=True>Residential</asp:ListItem><asp:ListItem Value="2">Commercial</asp:ListItem></asp:DropDownList></td>
	</tr>
	<tr>
		<td align="right">*Address1: </td>
		<td width="25%">
		    <asp:TextBox runat="server" ID="BillingAddress1" maxLength="100" ></asp:TextBox>
		</td>
		<td align="right">*Address1: </td>
		<td width="25%"><asp:TextBox runat="server" ID="ShippingAddress1" maxLength="100" ></asp:TextBox></td>
	</tr>
	<tr>
		<td align="right">Address2: </td>
		<td width="25%"><asp:TextBox runat="server" ID="BillingAddress2" maxLength="100" ></asp:TextBox></td>
		<td align="right">Address2: </td>
		<td width="25%"><asp:TextBox runat="server" ID="ShippingAddress2" maxLength="100" ></asp:TextBox></td>
	</tr>
	<tr>
		<td align="right">Suite: </td>
		<td width="25%"><asp:TextBox runat="server" ID="BillingSuite" maxLength="50" ></asp:TextBox></td>
		<td align="right">Suite: </td>
		<td width="25%"><asp:TextBox runat="server" ID="ShippingSuite" maxLength="50" ></asp:TextBox></td>
	</tr>
	<tr>
		<td align="right">*City or APO/AFO: </td>
		<td width="25%">
		    <asp:TextBox runat="server" ID="BillingCity" maxLength="50" ></asp:TextBox>
		</td>
		<td align="right">*City or APO/AFO: </td>
		<td width="25%">
		    <asp:TextBox runat="server" ID="ShippingCity" maxLength="50" ></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td align="right" style="height: 22px">State/Province: </td>
		<td style="height: 22px"><asp:DropDownList ID="BillingState" runat="server" DataTextField="Name" DataValueField="Abbreviation"></asp:DropDownList></td>
		<td align="right" style="height: 22px">State/Province: </td>
		<td style="height: 22px"><asp:DropDownList ID="ShippingState" runat="server" DataTextField="Name" DataValueField="Abbreviation"></asp:DropDownList></td>
	</tr>
	<tr>
		<td align="right">*Zip: </td>
		<td width="25%">
		    <asp:TextBox runat="server" ID="BillingZip" maxLength="10"></asp:TextBox>
		</td>
		<td align="right">*Zip: </td>
		<td width="25%">
		    <asp:TextBox runat="server" ID="ShippingZip" maxLength="10"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td align="right">Country: </td>
		<td width="25%"><asp:DropDownList ID="BillingCountry" runat="server" DataTextField="Name" DataValueField="Name"></asp:DropDownList></td>
		<td align="right">Country: </td>
		<td width="25%"><asp:DropDownList ID="ShippingCountry" runat="server" DataTextField="Name" DataValueField="Name"></asp:DropDownList></td>
	</tr>
	<tr><td colspan="4" align="center">
        <br />
        <asp:Button ID="CreateCustomer" runat="server" CssClass="normalButton" Text="Create Customer" visible="false" OnClick="CreateCustomer_Click"/>&nbsp;
        <asp:Button ID="UpdateCustomer" runat="server" CssClass="normalButton" Text="Update Customer" visible="false" OnClick="UpdateCustomer_Click"/>
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;
        <asp:Button ID="UseCustomer" runat="server" CssClass="normalButton" Text="Use Customer" visible="false" OnClick="UseCustomer_Click"/></td></tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="CustomerStatsPanel" runat="server" visible="false"><b>Creating Order For Customer: ID=<asp:Literal ID="UsingCustomerID" runat="server"></asp:Literal>, FirstName=<asp:Literal ID="UsingFirstName" runat="server"></asp:Literal>, LastName=<asp:Literal ID="UsingLastName" runat="server"></asp:Literal>, EMail=<asp:Literal ID="UsingEMail" runat="server"></asp:Literal></b>
            <br />
        <asp:Button ID="Button5" runat="server"  CssClass="normalButton" Text="Home Page" OnClick="Button5_Click" />
        &nbsp;<asp:Button ID="Button7" runat="server"  CssClass="normalButton" Text="SiteMap1" OnClick="Button7_Click" />
        &nbsp;<asp:Button ID="Button18" runat="server"  CssClass="normalButton" Text="SiteMap2" OnClick="Button18_Click" />
        &nbsp;
        <asp:Panel ID="SearchPanel" runat="server" style="display:inline;" DefaultButton="Button9">
            <asp:TextBox ID="QuickSearch" runat="server" Font-Size="XX-Small"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="QuickSearch"
                ErrorMessage="!!" Font-Bold="True" SetFocusOnError="True" ValidationGroup="ProductSearch"></asp:RequiredFieldValidator>
            <asp:Button ID="Button9" runat="server" CssClass="normalButton" Text="<< Product Search" OnClick="Button9_Click" ValidationGroup="ProductSearch" /></asp:Panel>
            &nbsp;<asp:Button ID="Button10" runat="server" CssClass="normalButton" Text="View Cart" OnClick="Button10_Click" />
            &nbsp;<asp:Button ID="Button11" runat="server" CssClass="normalButton" Text="Checkout" OnClick="Button11_Click" />
            &nbsp;<asp:Button ID="Button12" runat="server" CssClass="normalButton" Text="Re-Edit Customer" OnClick="Button12_Click" />
            &nbsp;<asp:Button ID="Button13" runat="server" CssClass="normalButton" Text="Re-Start Impersonation" OnClick="Button13_Click" />
            &nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="Button6" CssClass="normalButton" runat="server" Text="Cancel Order" OnClick="Button6_Click" /><asp:Button ID="Button88" ControlStyle Font-Size="XX-Small" runat="server" Text="Done Order" OnClick="Button88_Click" /></asp:Panel>
        <asp:Panel ID="ImpersonationPanel" runat="Server" Visible="false" >
            <iframe id="ImpersonationFrame" frameborder="1" style="height: 600px; overflow:visible; width: 100%;" runat="server" scrolling="auto"></iframe>
    	</asp:Panel>
    	<asp:Panel ID="Panel3" runat="server" Visible="false">
    	<table width="100%" height="600" cellpadding="0" cellspacing="0" border="1">
    	<tr>
    	<td align="left" valign="top" height="100%" width="25%">
            <iframe id="LeftPanel3Frame" frameborder="1" width="100%" height="100%" runat="server" scrolling="auto"></iframe>
        </td>
    	<td align="left" valign="top" height="100%" width="25%">
            <iframe id="MiddlePanel3Frame" frameborder="1" width="100%" height="100%" runat="server" scrolling="auto"></iframe>
        </td>
    	<td align="left" valign="top" height="100%" width="50%">
            <iframe id="RightPanel3Frame" src="empty.htm" frameborder="1" width="100%" height="100%" runat="server" scrolling="auto"></iframe>
        </td>
        </tr>
        </table>
    	</asp:Panel>
    	<asp:Panel ID="Panel2" runat="server" Visible="false" Width="100%" Height="600">
    	<table width="100%" height="100%" cellpadding="0" cellspacing="0" border="1">
    	<tr>
    	<td align="left" valign="top" height="100%" width="40%">
            <iframe src="empty.htm" id="LeftPanel2Frame" name="LeftPanel2Frame" frameborder="1" width="100%" height="100%" runat="server" scrolling="auto"></iframe>
        </td>
    	<td align="left" valign="top" height="100%" width="60%">
            <iframe src="empty.htm" id="RightPanel2Frame" name="RightPanel2Frame" frameborder="1" width="100%" height="100%" runat="server" scrolling="auto"></iframe>
        </td>
        </tr>
        </table>
    	</asp:Panel>

    </form>
</body>
</html>
