<%@ Page ClientTarget="UpLevel" language="c#" Inherits="AspDotNetStorefront.createaccount" CodeFile="createaccount.aspx.cs" %>
<%@ Register TagPrefix="aspdnsf" TagName="Topic" src="TopicControl.ascx" %>
<html>
<head>
</head>
<body>
    <asp:Panel ID="pnlMain" runat="server" HorizontalAlign="Center" Visible="true">
        <aspdnsf:Topic runat="server" ID="CreateAccountPageHeader" TopicName="CreateAccountPageHeader" />
        <asp:Panel ID="pnlCheckoutImage" runat="server" HorizontalAlign="Center" Visible="false">
            <asp:ImageMap ID="CheckoutImage" HotSpotMode="Navigate" runat="server">
                <asp:RectangleHotSpot Top="0" Left="0" Right="87" Bottom="54" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx?resetlinkback=1" />
            </asp:ImageMap>
            <br />
        </asp:Panel>
        
        <asp:Panel ID="pnlErrorMsg" runat="Server" HorizontalAlign="Left" style="margin-left:20px;">
            <asp:Label ID="ErrorMsgLabel" runat="server" Font-Bold="true" ForeColor="red"></asp:Label>
        </asp:Panel>

        <asp:Literal ID="Signin" runat="server" Mode="PassThrough"></asp:Literal>
        
        <form id="frmCreateAccount" runat="server">
            <asp:Panel ID="pnlAccountInfo" runat="server" Visible="false">
                <asp:Table ID="tblAccount" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                    <asp:TableRow>
                        <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                            <asp:Image runat="server" ID="accountinfo_gif" /><br />
                            <asp:Table ID="tblAccountBox" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                            
                                        <table border="0" cellpadding="2" cellspacing="2" width="100%">
                                            <tr>
                                                <td width="100%" colspan="2"><asp:Label ID="createaccountaspx12" Text="(!createaccount.aspx.12!)" runat="server" Font-Bold="true"></asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td width="100%" colspan="2">
                                                    <hr />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx13" runat="server" Text="(!createaccount.aspx.13!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:TextBox ID="FirstName" Columns="20" MaxLength="50" CausesValidation="true" ValidationGroup="registration" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx14" runat="server" Text="(!createaccount.aspx.14!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:TextBox ID="LastName" Columns="20" MaxLength="50" CausesValidation="true" ValidationGroup="registration" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx15" runat="server" Text="(!createaccount.aspx.15!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:TextBox ID="EMail" runat="server" Columns="30" MaxLength="100" ValidationGroup="registration" ></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="valReqEmail" ControlToValidate="EMail" Display="None" EnableClientScript="false" runat="server" ValidationGroup="registration"></asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ID="valRegExValEmail" ControlToValidate="EMail" Display="None" runat="SERVER" EnableClientScript="false" ValidationGroup="registration" ValidationExpression="^[a-zA-Z0-9][-\w\.\+]*@([a-zA-Z0-9][\w\-]*\.)+[a-zA-Z]{2,4}$"></asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <asp:Panel runat="server" ID="PasswordPanel">
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx18" runat="server" Text="(!createaccount.aspx.18!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:TextBox ID="txtpassword" runat="server" Columns="20" MaxLength="30" TextMode="SingleLine" ValidationGroup="registration" CausesValidation="true"></asp:TextBox>&nbsp;
                                                    <asp:Literal ID="createaccountaspx19" runat="server" Text="(!createaccount.aspx.19!)"></asp:Literal> 
                                                    <asp:RequiredFieldValidator ID="reqValPassword" ControlToValidate="txtpassword" runat="server" Display="None" EnableClientScript="false" ValidationGroup="registration"></asp:RequiredFieldValidator>
                                                    <asp:CustomValidator ID="valPassword" ControlToValidate="txtpassword" Display="None" EnableClientScript="false" runat="server" ValidationGroup="registration" OnServerValidate="ValidatePassword"></asp:CustomValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx21" runat="server" Text="(!createaccount.aspx.21!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:TextBox ID="txtpassword2" TextMode="SingleLine" Columns="20" maxlength="30" runat="server" ValidationGroup="registration" CausesValidation="true"></asp:TextBox>
                                                </td>
                                            </tr>
                                            </asp:Panel>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx23" runat="server"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:TextBox ID="Phone" runat="server" CausesValidation="true" Columns="14" MaxLength="20" ValidationGroup="registration"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="valReqPhone" ControlToValidate="Phone" EnableClientScript="false" runat="server" ValidationGroup="registration" Display="None"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr id="VATRegistrationIDRow" runat="server">
                                                <td width="35%" align="right"><asp:Literal ID="Literal3" runat="server" Text="(!account.aspx.71!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:TextBox ID="VATRegistrationID" runat="server" Columns="14" MaxLength="20"></asp:TextBox>
                                                </td>
                                            </tr>
											<asp:panel ID="pnlOver13" runat="server">
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="Literal1" runat="server" Text="(!createaccount.aspx.78!)"></asp:Literal></td>
                                                <td width="65%"><asp:CheckBox ID="Over13" Visible="true"  runat="server" /></td>
                                            </tr>
											</asp:panel>
                                            <tr>
                                                <td width="35%" valign="top" align="right"><asp:Literal ID="createaccountaspx26" runat="server" Text="(!createaccount.aspx.26!)"></asp:Literal><br />
                                                    <asp:Label ID="createaccountaspx29" runat="server" Text="(!createaccount.aspx.29!)"></asp:Label></td>
                                                <td width="65%" valign="top">
                                                    <asp:Literal ID="createaccountaspx27" runat="server" Text="(!createaccount.aspx.27!)"></asp:Literal>
                                                    <asp:RadioButton ID="OKToEMailYes" GroupName="OKToEMail" runat="server" />
                                                    &nbsp;&nbsp;&nbsp;&nbsp;
                                                    <asp:Literal ID="createaccountaspx28" runat="server" Text="(!createaccount.aspx.28!)"></asp:Literal>
                                                    <asp:RadioButton ID="OKToEMailNo" GroupName="OKToEMail" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Label ID="signinaspx21" CssClass="LightCellText" Text="(!signin.aspx.21!)" runat="server" Visible="false"></asp:Label></td>
                                                <td width="65%">
                                                    <asp:TextBox ID="SecurityCode" Columns="15" runat="server" CausesValidation="true" ValidationGroup="registration" Visible="false"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="valReqSecurityCode" ControlToValidate="SecurityCode" Display="None" EnableClientScript="false" Enabled="false" runat="server" ValidationGroup="registration"></asp:RequiredFieldValidator>
                                                    <asp:CustomValidator ID="valCustSecurityCode" ControlToValidate="SecurityCode" Display="None" EnableClientScript="false" Enabled="false" runat="server" ValidationGroup="registration" OnServerValidate="valCustSecurityCode_ServerValidate"></asp:CustomValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"></td>
                                                <td width="65%"><asp:Image ID="SecurityImage" runat="server" Visible="false" /></td>
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
                <asp:Literal ID="createaccount_aspx_15_2" runat="server" Text="(!createaccount.aspx.15!)"></asp:Literal>
                <asp:TextBox ID="txtSkipRegEmail" runat="server" Columns="30" MaxLength="100" ValidationGroup="skipreg" ></asp:TextBox> <small>Please enter your email so we can email your receipt.</small>
                <asp:RequiredFieldValidator ID="valReqSkipRegEmail" runat="server" ControlToValidate="txtSkipRegEmail" Enabled="false" Display="none" ValidationGroup="skipreg" EnableClientScript="false" ErrorMessage="(!createaccount.aspx.81!)"></asp:RequiredFieldValidator> 
                <asp:RegularExpressionValidator ID="valRegExSkipRegEmail" ControlToValidate="txtSkipRegEmail" Display="None" runat="SERVER" ValidationGroup="skipreg" ValidationExpression="^[a-zA-Z0-9][-\w\.\+]*@([a-zA-Z0-9][\w\-]*\.)+[a-zA-Z]{2,4}$"></asp:RegularExpressionValidator>  
                <br /><asp:Literal ID="Literal2" runat="server" Text="(!createaccount.aspx.78!)"></asp:Literal> <asp:CheckBox ID="SkipRegOver13" Visible="true"  runat="server" />
                <br /><br /><br />
            </asp:Panel>
            <asp:Panel ID="pnlBillingInfo" runat="server">
                <asp:Table ID="tblBillingInfo" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                    <asp:TableRow>
                        <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                            <asp:Image runat="server" ID="billinginfo_gif" /><br />
                            <asp:Table ID="tblBillingInfoBox" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                        <table border="0" cellpadding="2" cellspacing="2" width="100%">
                                            <tr>
                                                <td width="100%" colspan="2">
                                                    <b><asp:Literal ID="createaccountaspx30" Mode="PassThrough" runat="Server" ></asp:Literal> 
                                                    <asp:CheckBox ID="BillingEqualsAccount" runat="server" />
                                                    <asp:Literal ID="createaccountaspx31" Mode="PassThrough" runat="server"></asp:Literal></b></td>
                                            </tr>
                                            <tr>
                                                <td width="100%" colspan="2">
                                                    <hr />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx33" runat="server" Text="(!createaccount.aspx.33!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:TextBox ID="BillingFirstName" Columns="20" MaxLength="50" runat="server" CausesValidation="true" ValidationGroup="createaccount" />
                                                    <asp:RequiredFieldValidator ID="valReqBillFName" ValidationGroup="createacccount" ControlToValidate="BillingFirstName" Display="none" EnableClientScript="false" runat="server"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx35" runat="server" Text="(!createaccount.aspx.35!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:TextBox ID="BillingLastName" Columns="20" MaxLength="50" runat="server" CausesValidation="true" ValidationGroup="createaccount" /> 
                                                    <asp:RequiredFieldValidator ID="valReqBillLName" ControlToValidate="BillingLastName" Display="none" ValidationGroup="createacccount" EnableClientScript="false" runat="server"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx37" runat="server" Text="(!createaccount.aspx.37!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:TextBox ID="BillingPhone" Columns="20" MaxLength="25" runat="server" CausesValidation="true" ValidationGroup="createaccount" /> 
                                                    <asp:RequiredFieldValidator ID="valReqBillPhone" ControlToValidate="BillingPhone" EnableClientScript="false" runat="server" ValidationGroup="createaccount" Display="None"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx40" runat="server" Text="(!createaccount.aspx.40!)"></asp:Literal></td>
                                                <td width="65%"><asp:TextBox ID="BillingCompany" Columns="34" MaxLength="100" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="addresscs58" runat="server" Text="(!address.cs.58!)"></asp:Literal></td>
                                                <td width="65%"><asp:DropDownList ID="BillingResidenceType" runat="server" ></asp:DropDownList></td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx41" runat="server" Text="(!createaccount.aspx.41!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:TextBox ID="BillingAddress1" Columns="34" MaxLength="100" runat="server" CausesValidation="true" ValidationGroup="createaccount" /> 
                                                    <asp:RequiredFieldValidator ID="valReqBillAddr1" ControlToValidate="BillingAddress1" EnableClientScript="false" runat="server" ValidationGroup="createaccount" Display="None"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx43" runat="server" Text="(!createaccount.aspx.43!)"></asp:Literal></td>
                                                <td width="65%"><asp:TextBox ID="BillingAddress2" Columns="34" MaxLength="100" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx44" runat="server" Text="(!createaccount.aspx.44!)"></asp:Literal></td>
                                                <td width="65%"><asp:TextBox ID="BillingSuite" Columns="34" MaxLength="50" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx45" runat="server" Text="(!createaccount.aspx.45!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:TextBox ID="BillingCity" Columns="34" MaxLength="50" runat="server" CausesValidation="true" ValidationGroup="createaccount" /> 
                                                    <asp:RequiredFieldValidator ID="valReqBillCity" ControlToValidate="BillingCity" EnableClientScript="false" runat="server" ValidationGroup="createaccount" Display="None"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx52" runat="server" Text="(!createaccount.aspx.52!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:DropDownList ID="BillingCountry" runat="server" OnSelectedIndexChanged="BillingCountry_OnChange" AutoPostBack="True"></asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="valReqBillCountry" ControlToValidate="BillingCountry" EnableClientScript="true" runat="server" ValidationGroup="createaccount" Display="None"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx47" runat="server" Text="(!createaccount.aspx.47!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:DropDownList ID="BillingState" runat="server"></asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="valReqBillState" ControlToValidate="BillingState" EnableClientScript="false" runat="server" ValidationGroup="createaccount" Display="None"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx49" runat="server" Text="(!createaccount.aspx.49!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:TextBox ID="BillingZip" Columns="14" MaxLength="10" runat="server" CausesValidation="true" ValidationGroup="createaccount" />
                                                    <asp:RequiredFieldValidator ID="valReqBillZip" ControlToValidate="BillingZip" EnableClientScript="false" runat="server" ValidationGroup="createaccount" Display="None"></asp:RequiredFieldValidator>
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
            <br />
            <asp:Panel ID="pnlShippingInfo" runat="server" Visible="false">
                <asp:Table ID="tblShippingInfo" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                    <asp:TableRow>
                        <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                            <asp:Image runat="server" ID="shippinginfo_gif" /><br />
                            <asp:Table ID="tblShippingInfoBox" CellSpacing="2" CellPadding="2" Width="100%" runat="server">
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td width="100%" colspan="2">
                                                    <asp:Button ID="btnShppingEqBilling" runat="server" Text="(!createaccount.aspx.53!)" OnClick="btnShppingEqBilling_Click" />
                                                </td>
                                            </tr>
                                        </table>
                                        <hr />
                                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx55" runat="server" Text="(!createaccount.aspx.55!)"></asp:Literal></td>
                                                <td width="65%">
                                                <asp:TextBox ID="ShippingFirstName" Columns="20" MaxLength="50" runat="server" CausesValidation="true" ValidationGroup="createaccount" /> 
                                                <asp:RequiredFieldValidator ID="valReqShipFName" ControlToValidate="ShippingFirstName" EnableClientScript="false" runat="server" ValidationGroup="createaccount" Display="None"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx57" runat="server" Text="(!createaccount.aspx.57!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:TextBox ID="ShippingLastName" Columns="20" MaxLength="50" runat="server" CausesValidation="true" ValidationGroup="createaccount" />
                                                    <asp:RequiredFieldValidator ID="valReqShipLName" ControlToValidate="ShippingLastName" EnableClientScript="false" runat="server" ValidationGroup="createaccount" Display="None"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx59" runat="server" Text="(!createaccount.aspx.59!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:TextBox ID="ShippingPhone" Columns="20" MaxLength="25" runat="server" CausesValidation="true" ValidationGroup="createaccount" /> 
                                                    <asp:RequiredFieldValidator ID="valReqShipPhone" ControlToValidate="ShippingPhone" EnableClientScript="false" runat="server" ValidationGroup="createaccount" Display="None"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx62" runat="server" Text="(!createaccount.aspx.62!)"></asp:Literal></td>
                                                <td width="65%"><asp:TextBox ID="ShippingCompany" Columns="34" MaxLength="100" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="addresscs58_2" runat="server" Text="(!address.cs.58!)"></asp:Literal></td>
                                                <td width="65%"><asp:DropDownList ID="ShippingResidenceType" runat="server" ></asp:DropDownList></td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx63" runat="server" Text="(!createaccount.aspx.63!)"></asp:Literal></td>
                                                <td width="65%">
                                                <asp:TextBox ID="ShippingAddress1" Columns="34" MaxLength="100" runat="server" CausesValidation="true" ValidationGroup="createaccount" /> 
                                                <asp:RequiredFieldValidator ID="valReqShipAddr1" ControlToValidate="ShippingAddress1" EnableClientScript="false" runat="server" ValidationGroup="createaccount" Display="None"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx65" runat="server" Text="(!createaccount.aspx.65!)"></asp:Literal></td>
                                                <td width="65%"><asp:TextBox ID="ShippingAddress2" Columns="34" MaxLength="100" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx66" runat="server" Text="(!createaccount.aspx.66!)"></asp:Literal></td>
                                                <td width="65%"><asp:TextBox ID="ShippingSuite" Columns="34" MaxLength="50" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx67" runat="server" Text="(!createaccount.aspx.67!)"></asp:Literal></td>
                                                <td width="65%"><asp:TextBox ID="ShippingCity" Columns="34" MaxLength="50" runat="server" CausesValidation="true" ValidationGroup="createaccount" /> 
                                                <asp:RequiredFieldValidator ID="valReqShipCity" ControlToValidate="ShippingCity" EnableClientScript="false" runat="server" ValidationGroup="createaccount" Display="None"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx73" runat="server" Text="(!createaccount.aspx.73!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:DropDownList ID="ShippingCountry" runat="server" OnSelectedIndexChanged="ShippingCountry_Change" AutoPostBack="True"></asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="valReqShipCountry" ControlToValidate="ShippingCountry" EnableClientScript="true" runat="server" ValidationGroup="createaccount" Display="None"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx69" runat="server" Text="(!createaccount.aspx.69!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:DropDownList ID="ShippingState" runat="server"></asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="valReqShipState" ControlToValidate="ShippingState" EnableClientScript="false" runat="server" ValidationGroup="createaccount" Display="None"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="35%" align="right"><asp:Literal ID="createaccountaspx70" runat="server" Text="(!createaccount.aspx.70!)"></asp:Literal></td>
                                                <td width="65%">
                                                    <asp:TextBox ID="ShippingZip" Columns="14" MaxLength="10" runat="server" CausesValidation="true" ValidationGroup="createaccount" /> 
                                                    <asp:RequiredFieldValidator ID="valReqShipZip" ControlToValidate="ShippingZip" EnableClientScript="false" runat="server" ValidationGroup="createaccount" Display="None"></asp:RequiredFieldValidator>
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
            <br />
            <asp:Button ID="btnContinueCheckout" CssClass="ContinueCheckoutButton" OnClientClick="Page_ValidationActive=true;" runat="server" Text="(!createaccount.aspx.76!)" CausesValidation="True" ValidationGroup="createaccount" OnClick="btnContinueCheckout_Click" />
            <asp:ValidationSummary ID="valSummary" DisplayMode="List" runat="server" ShowMessageBox="true" ShowSummary="false" ValidationGroup="createaccount" ForeColor="red" Font-Bold="true" />
        </form>
    </asp:Panel>
</body>
</html>
