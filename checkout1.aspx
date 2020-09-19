<%@ Page ClientTarget="UpLevel" Language="c#" Inherits="AspDotNetStorefront.checkout1" CodeFile="checkout1.aspx.cs" %>

<%@ Register TagPrefix="aspdnsf" TagName="Topic" Src="TopicControl.ascx" %>
<html xmlns="http://www.w3.org/1999/xhtml"> 
<head>
<title></title>
</head>
<body>
    <asp:Literal ID="CheckoutValidationScript" runat="server" Mode="PassThrough"></asp:Literal>
    <asp:Panel ID="pnlMain" runat="server" HorizontalAlign="Center" Visible="true">
        <aspdnsf:Topic runat="server" ID="Checkout1PageHeader" TopicName="Checkout1PageHeader" />
        <asp:Panel ID="pnlErrorMsg" runat="Server" HorizontalAlign="Left" Style="margin-left: 20px;">
            <asp:Label ID="ErrorMsgLabel" runat="server" Font-Bold="true" ForeColor="red"></asp:Label>
        </asp:Panel>
        
        <form runat="server">
            <asp:Panel ID="pnlAccountInfo" runat="server" Visible="false">
                <asp:Table ID="tblAccount" CellSpacing="0" CellPadding="0" Width="100%" runat="server">
                    <asp:TableRow>
                        <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                            <asp:Image runat="server" ID="accountinfo_gif" />&nbsp;&nbsp;&nbsp;<asp:Label ID="Signin" runat="server" ForeColor="red"></asp:Label><br />
                            <asp:Table ID="tblAccountBox" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td width="100%" colspan="2">
                                                    <asp:Label ID="Checkout1aspx12" Text="(!createaccount.aspx.12!)" runat="server" Font-Bold="true"></asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td width="100%" colspan="2">
                                                    <hr />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Literal ID="Checkout1aspx13" runat="server" Text="(!createaccount.aspx.13!)"></asp:Literal></td>
                                                <td>
                                                    <asp:TextBox ID="FirstName" Columns="20" MaxLength="50" CausesValidation="true" ValidationGroup="registration" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Literal ID="Checkout1aspx14" runat="server" Text="(!createaccount.aspx.14!)"></asp:Literal></td>
                                                <td>
                                                    <asp:TextBox ID="LastName" Columns="20" MaxLength="50" CausesValidation="true" ValidationGroup="registration" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Literal ID="Checkout1aspx15" runat="server" Text="(!createaccount.aspx.15!)"></asp:Literal></td>
                                                <td>
                                                    <asp:TextBox ID="EMail" runat="server" Columns="30" MaxLength="100" ValidationGroup="createacccount"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="valReqEmail" ControlToValidate="EMail" Display="None" EnableClientScript="false" runat="server" ValidationGroup="registration"></asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ID="valRegExValEmail" runat="SERVER" ControlToValidate="EMail" Display="None" EnableClientScript="false" ValidationExpression="^[a-zA-Z0-9][-\w\.]*@([a-zA-Z0-9][\w\-]*\.)+[a-zA-Z]{2,3}$" ValidationGroup="registration"></asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Literal ID="Checkout1aspx18" runat="server" Text="(!createaccount.aspx.18!)"></asp:Literal></td>
                                                <td>
                                                    <asp:TextBox ID="password" runat="server" Columns="20" MaxLength="30" TextMode="SingleLine" ValidationGroup="registration" CausesValidation="true"></asp:TextBox>
                                                    <asp:Literal ID="Checkout1aspx19" runat="server" Text="(!createaccount.aspx.19!)"></asp:Literal>
                                                    <asp:RequiredFieldValidator ID="reqValPassword" ControlToValidate="password" runat="server" Display="None" EnableClientScript="false" ValidationGroup="registration"></asp:RequiredFieldValidator>
                                                    <asp:CustomValidator ID="valPassword" ControlToValidate="password" Display="None" EnableClientScript="false" runat="server" ValidationGroup="registration" OnServerValidate="ValidatePassword"></asp:CustomValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Literal ID="Checkout1aspx21" runat="server" Text="(!createaccount.aspx.21!)"></asp:Literal></td>
                                                <td>
                                                    <asp:TextBox ID="password2" TextMode="SingleLine" Columns="20" MaxLength="30" runat="server" ValidationGroup="registration" CausesValidation="true"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Literal ID="Checkout1aspx23" runat="server"></asp:Literal></td>
                                                <td>
                                                    <asp:TextBox ID="Phone" runat="server" CausesValidation="true" Columns="14" MaxLength="20" ValidationGroup="registration"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="valReqPhone" ControlToValidate="Phone" EnableClientScript="false" runat="server" ValidationGroup="registration" Display="None"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top">
                                                    <asp:Literal ID="Checkout1aspx26" runat="server" Text="(!createaccount.aspx.26!)"></asp:Literal></td>
                                                <td valign="top">
                                                    <asp:Literal ID="Checkout1aspx27" runat="server" Text="(!createaccount.aspx.27!)"></asp:Literal>&nbsp;
                                                    <asp:RadioButton ID="OKToEMailYes" GroupName="OKToEMail" runat="server" />
                                                    <asp:Literal ID="Checkout1aspx28" runat="server" Text="(!createaccount.aspx.28!)"></asp:Literal>&nbsp;
                                                    <asp:RadioButton ID="OKToEMailNo" GroupName="OKToEMail" runat="server" />
                                                    <asp:Label ID="Checkout1aspx29" runat="server" Text="(!createaccount.aspx.29!)"></asp:Label>
                                                </td>
                                            </tr>
                                            <asp:Panel ID="pnlOver13" runat="server">
                                                <tr>
                                                    <td>
                                                        <asp:Literal ID="Literal1" runat="server" Text="(!createaccount.aspx.78!)"></asp:Literal></td>
                                                    <td>
                                                        <asp:CheckBox ID="Over13" Visible="true" runat="server" /></td>
                                                </tr>
                                            </asp:Panel>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="signinaspx21" CssClass="LightCellText" Text="(!signin.aspx.21!)" runat="server" Visible="false"></asp:Label></td>
                                                <td>
                                                    <asp:TextBox ID="SecurityCode" Columns="15" runat="server" CausesValidation="true" ValidationGroup="registration" Visible="false"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="valReqSecurityCode" ControlToValidate="SecurityCode" Display="None" EnableClientScript="false" Enabled="false" runat="server" ValidationGroup="registration"></asp:RequiredFieldValidator>
                                                    <asp:CustomValidator ID="valCustSecurityCode" ControlToValidate="SecurityCode" Display="None" EnableClientScript="false" Enabled="false" runat="server" ValidationGroup="registration" OnServerValidate="valCustSecurityCode_ServerValidate"></asp:CustomValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                </td>
                                                <td>
                                                    <asp:Image ID="SecurityImage" runat="server" Visible="false" /></td>
                                            </tr>
                                        </table>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:Panel>
            <br />
            <asp:Panel ID="pnlSkipReg" runat="server" Visible="false" HorizontalAlign="Left">
                <asp:Table ID="tblSkipReg" CellSpacing="0" CellPadding="0" Width="100%" runat="server">
                    <asp:TableRow>
                        <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                            <asp:Image runat="server" ID="skipreg_gif" />&nbsp;&nbsp;&nbsp;<asp:Label ID="skipRegSignin" runat="server" ForeColor="red"></asp:Label><br />
                            <asp:Table ID="tblSkipRegBox" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                        <asp:Literal ID="Checkout1_aspx_15_2" runat="server" Text="(!createaccount.aspx.15!)"></asp:Literal>&nbsp;
                                        <asp:TextBox ID="txtSkipRegEmail" runat="server" Columns="30" MaxLength="100" ValidationGroup="skipreg"></asp:TextBox>
                                        <small>Please enter your email so we can email your receipt.</small>
                                        <asp:RegularExpressionValidator ID="valRegExSkipRegEmail" ControlToValidate="txtSkipRegEmail" Display="None" runat="SERVER" ValidationGroup="skipreg" ValidationExpression="^[a-zA-Z0-9][-\w\.]*@([a-zA-Z0-9][\w\-]*\.)+[a-zA-Z]{2,3}$"></asp:RegularExpressionValidator>
                                        <br />
                                        <br /><asp:Literal ID="Literal2" runat="server" Text="(!createaccount.aspx.78!)"></asp:Literal> <asp:CheckBox ID="SkipRegOver13" Visible="true"  runat="server" />
                                        <br />
                                        <br />
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <br />
            </asp:Panel>
            <table width="100%" cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td align="left" valign="top">
                        <asp:Panel ID="pnlBillingInfo" runat="server">
                            <asp:Table ID="tblBillingInfo" CellSpacing="0" CellPadding="0" Width="100%" runat="server">
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                        <asp:Image runat="server" ID="billinginfo_gif" />&#0160;<a href="selectaddress.aspx?add=true&addressType=Billing&Checkout=True&returnURL=checkout1.aspx%3fcheckout%3dTrue">Add/Edit Billing Address</a><br />
                                        <asp:Table ID="tblBillingInfoBox" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                                            <asp:TableRow>
                                                <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                                        <tr>
                                                            <td width="100%" height="25" colspan="2">
                                                                <b>
                                                                    <asp:Literal ID="Checkout1aspx30" Mode="PassThrough" runat="Server"></asp:Literal>
                                                                    <asp:Literal ID="Checkout1aspx31" Mode="PassThrough" runat="server"></asp:Literal></b></td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx33" runat="server" Text="(!createaccount.aspx.33!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="BillingFirstName" Enabled="false" Columns="20" MaxLength="50" runat="server" CausesValidation="true" ValidationGroup="BillingCheckout1" />
                                                                <asp:RequiredFieldValidator ID="valReqBillFName" ValidationGroup="BillingCheckout1" ControlToValidate="BillingFirstName" Display="none" EnableClientScript="false" runat="server"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx35" runat="server" Text="(!createaccount.aspx.35!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="BillingLastName" Enabled="false" Columns="20" MaxLength="50" runat="server" CausesValidation="true" ValidationGroup="BillingCheckout1" />
                                                                <asp:RequiredFieldValidator ID="valReqBillLName" ControlToValidate="BillingLastName" Display="none" ValidationGroup="BillingCheckout1" EnableClientScript="false" runat="server"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx37" runat="server" Text="(!createaccount.aspx.37!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="BillingPhone" Enabled="false" Columns="20" MaxLength="25" runat="server" CausesValidation="true" ValidationGroup="BillingCheckout1" />
                                                                <asp:RequiredFieldValidator ID="valReqBillPhone" ControlToValidate="BillingPhone" EnableClientScript="false" runat="server" ValidationGroup="BillingCheckout1" Display="None"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx40" runat="server" Text="(!createaccount.aspx.40!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="BillingCompany" Enabled="false" Columns="25" MaxLength="100" runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="addresscs58" runat="server" Text="(!address.cs.58!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:DropDownList ID="BillingResidenceType" Enabled="false" runat="server"></asp:DropDownList></td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx41" runat="server" Text="(!createaccount.aspx.41!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="BillingAddress1" Enabled="false" Columns="25" MaxLength="100" runat="server" CausesValidation="true" ValidationGroup="BillingCheckout1" />
                                                                <asp:RequiredFieldValidator ID="valReqBillAddr1" ControlToValidate="BillingAddress1" EnableClientScript="false" runat="server" ValidationGroup="BillingCheckout1" Display="None"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx43" runat="server" Text="(!createaccount.aspx.43!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="BillingAddress2" Enabled="false" Columns="25" MaxLength="100" runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx44" runat="server" Text="(!createaccount.aspx.44!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="BillingSuite" Enabled="false" Columns="25" MaxLength="50" runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx45" runat="server" Text="(!createaccount.aspx.45!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="BillingCity" Enabled="false" Columns="25" MaxLength="50" runat="server" CausesValidation="true" ValidationGroup="BillingCheckout1" />
                                                                <asp:RequiredFieldValidator ID="valReqBillCity" ControlToValidate="BillingCity" EnableClientScript="false" runat="server" ValidationGroup="BillingCheckout1" Display="None"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx52" runat="server" Text="(!createaccount.aspx.52!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:DropDownList ID="BillingCountry" Enabled="false" Style="width: 175px;" runat="server" OnSelectedIndexChanged="BillingCountry_OnChange" AutoPostBack="True"></asp:DropDownList>
                                                                <asp:RequiredFieldValidator ID="valReqBillCountry" ControlToValidate="BillingCountry" EnableClientScript="false" runat="server" ValidationGroup="BillingCheckout1" Display="None"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx47" runat="server" Text="(!createaccount.aspx.47!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:DropDownList ID="BillingState" Enabled="false" Style="width: 175px;" runat="server"></asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx49" runat="server" Text="(!createaccount.aspx.49!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="BillingZip" Enabled="false" Columns="14" MaxLength="10" runat="server" CausesValidation="true" ValidationGroup="BillingCheckout1" />
                                                                <asp:RequiredFieldValidator ID="valReqBillZip" ControlToValidate="BillingZip" EnableClientScript="false" runat="server" ValidationGroup="BillingCheckout1" Display="None"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                        </asp:Table>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </asp:Panel>
                    </td>
                    <td align="left" valign="top">
                        <asp:Panel ID="pnlShippingInfo" runat="server" Visible="false">
                            <asp:Table ID="tblShippingInfo" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                        <asp:Image runat="server" ID="shippinginfo_gif" />&#0160;<a href="selectaddress.aspx?add=true&addressType=Shipping&Checkout=True&returnURL=checkout1.aspx%3fcheckout%3dTrue">Add/Edit Shipping Address</a><br />
                                        <asp:Table ID="tblShippingInfoBox" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                                            <asp:TableRow>
                                                <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                                        <tr>
                                                            <td width="100%" height="25" colspan="2">
                                                                <b>
                                                                    <asp:Literal ID="Checkout1aspx53" Mode="PassThrough" Text="(!createaccount.aspx.53!)" runat="server"></asp:Literal>
                                                                    <asp:CheckBox ID="ShippingEqualsBilling" runat="server" /></b>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx55" runat="server" Text="(!createaccount.aspx.55!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="ShippingFirstName" Enabled="false" Columns="20" MaxLength="50" runat="server" CausesValidation="true" ValidationGroup="ShippingCheckout1" />
                                                                <asp:RequiredFieldValidator ID="valReqShipFName" ControlToValidate="ShippingFirstName" EnableClientScript="false" runat="server" ValidationGroup="ShippingCheckout1" Display="None"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx57" runat="server" Text="(!createaccount.aspx.57!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="ShippingLastName" Enabled="false" Columns="20" MaxLength="50" runat="server" CausesValidation="true" ValidationGroup="ShippingCheckout1" />
                                                                <asp:RequiredFieldValidator ID="valReqShipLName" ControlToValidate="ShippingLastName" EnableClientScript="false" runat="server" ValidationGroup="ShippingCheckout1" Display="None"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx59" runat="server" Text="(!createaccount.aspx.59!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="ShippingPhone" Enabled="false" Columns="20" MaxLength="25" runat="server" CausesValidation="true" ValidationGroup="ShippingCheckout1" />
                                                                <asp:RequiredFieldValidator ID="valReqShipPhone" ControlToValidate="ShippingPhone" EnableClientScript="false" runat="server" ValidationGroup="ShippingCheckout1" Display="None"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx62" runat="server" Text="(!createaccount.aspx.62!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="ShippingCompany" Enabled="false" Columns="25" MaxLength="100" runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="addresscs58_2" runat="server" Text="(!address.cs.58!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:DropDownList ID="ShippingResidenceType" Enabled="false" runat="server"></asp:DropDownList></td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx63" runat="server" Text="(!createaccount.aspx.63!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="ShippingAddress1" Enabled="false" Columns="25" MaxLength="100" runat="server" CausesValidation="true" ValidationGroup="ShippingCheckout1" />
                                                                <asp:RequiredFieldValidator ID="valReqShipAddr1" ControlToValidate="ShippingAddress1" EnableClientScript="false" runat="server" ValidationGroup="ShippingCheckout1" Display="None"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx65" runat="server" Text="(!createaccount.aspx.65!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="ShippingAddress2" Enabled="false" Columns="25" MaxLength="100" runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx66" runat="server" Text="(!createaccount.aspx.66!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="ShippingSuite" Enabled="false" Columns="25" MaxLength="50" runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx67" runat="server" Text="(!createaccount.aspx.67!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="ShippingCity" Enabled="false" Columns="25" MaxLength="50" runat="server" CausesValidation="true" ValidationGroup="ShippingCheckout1" />
                                                                <asp:RequiredFieldValidator ID="valReqShipCity" ControlToValidate="ShippingCity" EnableClientScript="false" runat="server" ValidationGroup="ShippingCheckout1" Display="None"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx73" runat="server" Text="(!createaccount.aspx.73!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:DropDownList ID="ShippingCountry" Enabled="false" Style="width: 175px;" runat="server" OnSelectedIndexChanged="ShippingCountry_Change" AutoPostBack="True"></asp:DropDownList>
                                                                <asp:RequiredFieldValidator ID="valReqShipCountry" ControlToValidate="ShippingCountry" EnableClientScript="false" runat="server" ValidationGroup="ShippingCheckout1" Display="None"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx69" runat="server" Text="(!createaccount.aspx.69!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:DropDownList ID="ShippingState" Enabled="false" Style="width: 175px;" runat="server"></asp:DropDownList>
                                                                <asp:RequiredFieldValidator ID="valReqShipState" ControlToValidate="ShippingState" EnableClientScript="false" runat="server" ValidationGroup="ShippingCheckout1" Display="None"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="35%">
                                                                <asp:Literal ID="Checkout1aspx70" runat="server" Text="(!createaccount.aspx.70!)"></asp:Literal></td>
                                                            <td width="65%">
                                                                <asp:TextBox ID="ShippingZip" Enabled="false" Columns="14" MaxLength="10" runat="server" CausesValidation="true" ValidationGroup="ShippingCheckout1" />
                                                                <asp:RequiredFieldValidator ID="valReqShipZip" ControlToValidate="ShippingZip" EnableClientScript="false" runat="server" ValidationGroup="ShippingCheckout1" Display="None"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                        </asp:Table>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
            <br />
            <asp:Table ID="tblShippingSelect" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                        <asp:Image runat="server" ID="shippingselect_gif" /><br />
                        <asp:Table ID="tblShippingSelectBox" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                            <asp:TableRow>
                                <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                    <asp:Panel ID="pnlShippingOptions" runat="server" Visible="true">
                                        <aspdnsf:Topic runat="server" ID="Topic2" TopicName="CheckoutShippingPageHeader" />
                                        <asp:Literal ID="XmlPackage_CheckoutShippingPageHeader" runat="server" Mode="PassThrough"></asp:Literal>
                                        <asp:Panel ID="pnlGetFreeShippingMsg" CssClass="FreeShippingThresholdPrompt" runat="server">
                                            <asp:Literal ID="GetFreeShippingMsg" runat="server" Mode="PassThrough"></asp:Literal>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlFreeShippingMsg" CssClass="FreeShippingThresholdPrompt" runat="server">
                                            <asp:Label ID="FreeShippingMsg" runat="server"></asp:Label>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlCartAllowsShippingMethodSelection" runat="server">
                                            <asp:Label ID="ShipSelectionMsg" runat="server"></asp:Label>
                                            <asp:Literal ID="ShippingOptions" runat="server"></asp:Literal>
                                        </asp:Panel>
                                        <aspdnsf:Topic runat="server" ID="Topic3" TopicName="CheckoutShippingPageFooter" />
                                        <asp:Literal ID="XmlPackage_CheckoutShippingPageFooter" runat="server" Mode="PassThrough"></asp:Literal>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlRecalcShipping" runat="server" Visible="false" HorizontalAlign="center">
                                        <asp:Label ID="lblRecalcShippiingMsg" runat="server" Font-Bold="true" ForeColor="red" Text="(!checkout1.aspx.6!)"></asp:Label>
                                        <br /><br />
                                        <asp:Button ID="btnRecalcShipping" runat="server" Text="(!checkout1.aspx.5!)" OnClick="btnRecalcShipping_OnClick"/>
                                        <br /><br />
                                    </asp:Panel>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <br />
            
            <asp:Table ID="tblPaymentSelect" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                        <asp:Image runat="server" ID="paymentselect_gif" /><br />
                        <asp:Table ID="tblPaymentOptions" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                            <asp:TableRow>
                                <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                    <aspdnsf:Topic runat="server" ID="Topic1" TopicName="CheckoutPaymentPageHeader" />
                                    <asp:Literal ID="XmlPackage_CheckoutPaymentPageHeader" runat="server" Mode="PassThrough"></asp:Literal>
                                        
                                    <asp:Panel ID="pnlNoPaymentRequired" runat="server" HorizontalAlign="Center" Visible="false">
                                        <asp:Label ID="NoPaymentRequired" runat="server" Font-Bold="true" ForeColor="blue"></asp:Label><br /><br />
                                        <asp:Literal ID="Finalization" runat="server" Mode="PassThrough"></asp:Literal>
                                    </asp:Panel>
                                    
                                    <asp:Panel ID="pnlPaymentOptions" runat="server" HorizontalAlign="left" Visible="true">
                                        <table cellpadding="0" cellspacing="0" border="0">
                                            <tr>
                                                <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtCreditCard" OnCheckedChanged="pmtCreditCard_CheckedChanged" runat="server" Visible="false" AutoPostBack="true" /></td>
                                                <td><asp:Image ID="CCIMage" runat="server" Visible="false" /></td>
                                            </tr>
                                            <tr>
                                                <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtPURCHASEORDER" OnCheckedChanged="pmtPURCHASEORDER_CheckedChanged" Text="(!checkoutpayment.aspx.8!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtCODMONEYORDER" OnCheckedChanged="pmtCODMONEYORDER_CheckedChanged" Text="(!checkoutpayment.aspx.24!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtCODCOMPANYCHECK" OnCheckedChanged="pmtCODCOMPANYCHECK_CheckedChanged" Text="(!checkoutpayment.aspx.22!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtCODNET30" OnCheckedChanged="pmtCODNET30_CheckedChanged" Text="(!checkoutpayment.aspx.23!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtPAYPAL" OnCheckedChanged="pmtPAYPAL_CheckedChanged" Text="" runat="server" Visible="false" AutoPostBack="true" /></td>
                                                <td><asp:Image ID="PayPalImage" runat="server" Visible="false" /></td>
                                            </tr>
                                            <tr>
                                                <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtREQUESTQUOTE" OnCheckedChanged="pmtREQUESTQUOTE_CheckedChanged" Text="(!checkoutpayment.aspx.10!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtCHECKBYMAIL" OnCheckedChanged="pmtCHECKBYMAIL_CheckedChanged" Text="(!checkoutpayment.aspx.11!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtCOD" OnCheckedChanged="pmtCOD_CheckedChanged" Text="(!checkoutpayment.aspx.12!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtECHECK" OnCheckedChanged="pmtECHECK_CheckedChanged" Text="(!checkoutpayment.aspx.13!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtMICROPAY" OnCheckedChanged="pmtMICROPAY_CheckedChanged" Text="(!checkoutpayment.aspx.14!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtPAYPALEXPRESS" OnCheckedChanged="pmtPAYPALEXPRESS_CheckedChanged" Text="" runat="server" Visible="false" AutoPostBack="true" /></td>
                                                <td><asp:Image ID="PayPalExpressImage" runat="server" Visible="false" /></td>
                                            </tr>
                                        </table>
                                        <br />
                                    </asp:Panel>
                                    <hr/>
                                    <asp:Panel ID="paymentPanes" runat="server" HorizontalAlign="left" style="width: 90%; padding-left: 10px; padding-top: 10px; padding-right: 10px; padding-bottom: 20px;">
                                        <asp:Panel ID="pnlCreditCardPane" runat="server" Visible="false">
                                            <aspdnsf:Topic runat="server" ID="CheckoutCreditCardPageHeader" TopicName="CheckoutCreditCardPageHeader" />
                                            <asp:Literal ID="CCForm" runat="server" Mode="passThrough"></asp:Literal>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlPOPane" runat="server" Visible="false">
                                            <aspdnsf:Topic runat="server" ID="CheckoutPOPageHeader" TopicName="CheckoutPOPageHeader" />
                                            <asp:Literal ID="POForm" runat="server" Mode="passThrough"></asp:Literal>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlCODMOPane" runat="server" Visible="false">
                                            <aspdnsf:Topic runat="server" ID="CheckoutCODMoneyOrderPageHeader" TopicName="CheckoutCODMoneyOrderPageHeader" />
                                            <asp:Literal ID="CODMOForm" runat="server" Mode="passThrough"></asp:Literal>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlCODCoCheckPane" runat="server" Visible="false">
                                            <aspdnsf:Topic runat="server" ID="CheckoutCODCompanyCheckPageHeader" TopicName="CheckoutCODCompanyCheckPageHeader" />
                                            <asp:Literal ID="CODCoCheckForm" runat="server" Mode="passThrough"></asp:Literal>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlCODNet30Pane" runat="server" Visible="false">
                                            <aspdnsf:Topic runat="server" ID="CheckoutCODNet30PageHeader" TopicName="CheckoutCODNet30PageHeader" />
                                            <asp:Literal ID="CODNet30Form" runat="server" Mode="passThrough"></asp:Literal>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlPayPalPane" runat="server" Visible="false">
                                            <aspdnsf:Topic runat="server" ID="CheckoutPayPalPageHeader" TopicName="CheckoutPayPalPageHeader" />
                                            <asp:Literal ID="PayPalForm" runat="server" Mode="passThrough"></asp:Literal>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlReqQuotePane" runat="server" Visible="false">
                                            <aspdnsf:Topic runat="server" ID="CheckoutRequestQuotePageHeader" TopicName="CheckoutRequestQuotePageHeader" />
                                            <asp:Literal ID="ReqQuoteForm" runat="server" Mode="passThrough"></asp:Literal>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlCheckByMailPane" runat="server" Visible="false">
                                            <aspdnsf:Topic runat="server" ID="CheckoutCheckPageHeader" TopicName="CheckoutCheckPageHeader" />
                                            <asp:Literal ID="CheckByMailForm" runat="server" Mode="passThrough"></asp:Literal>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlCODPane" runat="server" Visible="false">
                                            <aspdnsf:Topic runat="server" ID="CheckoutCODPageHeader" TopicName="CheckoutCODPageHeader" />
                                            <asp:Literal ID="CODForm" runat="server" Mode="passThrough"></asp:Literal>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlECheckPane" runat="server" Visible="false">
                                            <aspdnsf:Topic runat="server" ID="CheckoutECheckPageHeader" TopicName="CheckoutECheckPageHeader" />
                                            <asp:Literal ID="ECheckForm" runat="server" Mode="passThrough"></asp:Literal>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlMicroPayPane" runat="server" Visible="false">
                                            <aspdnsf:Topic runat="server" ID="CheckoutMicropayPageHeader" TopicName="CheckoutMicropayPageHeader" />
                                            <asp:Literal ID="MicroPayForm" runat="server" Mode="passThrough"></asp:Literal>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlPayPalExpressPane" runat="server" Visible="false">
                                            <aspdnsf:Topic runat="server" ID="CheckoutPayPalExpressPageHeader" TopicName="CheckoutPayPalExpressPageHeader" />
                                            <asp:Literal ID="PayPalExpressForm" runat="server" Mode="passThrough"></asp:Literal>
                                        </asp:Panel>
                                    </asp:Panel>
                                    
                                    <asp:Panel ID="pnlTerms" runat="server" Visible="false">
                                        <asp:Literal ID="terms" Mode="PassThrough" runat="server"></asp:Literal>
                                    </asp:Panel>
                                                                    
                                    <aspdnsf:Topic runat="server" ID="Topic4" TopicName="CheckoutPaymentPageFooter" />
                                    <asp:Literal ID="XmlPackage_CheckoutPaymentPageFooter" runat="server" Mode="PassThrough"></asp:Literal>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <asp:Button ID="btnContinueCheckout" CssClass="ContinueCheckoutButton" UseSubmitBehavior="true" runat="server" Text="(!checkout1.aspx.1!)" CausesValidation="True" ValidationGroup="Checkout1" OnClick="btnContinueCheckout_Click" />
            <asp:ValidationSummary ID="valSummary" DisplayMode="List" runat="server" ShowMessageBox="true" ShowSummary="false" ValidationGroup="Checkout1" ForeColor="red" Font-Bold="true" />
        </form>
    </asp:Panel>

    <div>
        <asp:Literal ID="CartSummary" runat="server"></asp:Literal>
    </div>

    <asp:Panel ID="RTSdebug" runat="server">
        <asp:Literal ID="DebugInfo" runat="server" Mode="PassThrough"/>
    </asp:Panel>

    <asp:Literal ID="JSPopupRoutines" runat="server"></asp:Literal>
</body>
</html>
