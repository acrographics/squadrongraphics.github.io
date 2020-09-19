<%@ Page Language="c#" Inherits="AspDotNetStorefront.checkoutanon" CodeFile="checkoutanon.aspx.cs" %>
<%@ Register TagPrefix="aspdnsf" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register TagPrefix="aspdnsfc" Namespace="ASPDNSFControls" Assembly="ASPDNSFControls" %>
<html>
<head>
</head>
<body>
    <form runat="Server">
        <asp:Literal ID="JSPopupRoutines" runat="server"></asp:Literal>
        <div align="center">
            <asp:Panel ID="HeaderPanel" runat="server" Width="90%">
                <div align="center" style="text-align: center">
                    <asp:Panel ID="CheckoutPanel" runat="server">
                        <div id="CheckoutSequence" align="center">
                            <asp:ImageMap ID="CheckoutMap" runat="server" >
                                <asp:RectangleHotSpot Bottom="54" HotSpotMode="Navigate" NavigateUrl="shoppingcart.aspx?resetlinkback=1&amp;checkout=true" Right="87" />
                            </asp:ImageMap><br />
                        </div>
                    </asp:Panel>
                </div>
            </asp:Panel>
            <asp:Panel ID="ErrorPanel" runat="server" Visible="False" HorizontalAlign="center">
                        <asp:Label ID="ErrorMsgLabel" CssClass="errorLg" runat="server"></asp:Label><br /><br />
            </asp:Panel>
            <asp:Panel ID="FormPanel" runat="server" Width="90%">
                    <table cellspacing="0" cellpadding="1" width="550" border="0" id="table1">
                        <tr>
                            <td valign="top" width="300">
                                <asp:Label ID="Label6" runat="server" Text="(!checkoutanon.aspx.12!)" Font-Bold="true"></asp:Label><br /><br />
                                <asp:Label ID="Label1" runat="server" Text="(!checkoutanon.aspx.3!)" /><br /><br />
                                <table cellspacing="0" cellpadding="0" width="100%" border="0" id="table2">
                                    <tr>
                                        <td align="left"><asp:Label ID="Label2" runat="server" Text="(!checkoutanon.aspx.4!)" Font-Bold="true" /></td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:TextBox ID="EMail" runat="server" ValidationGroup="Group1" MaxLength="100" CausesValidation="True"
                                                AutoCompleteType="Email" Width="157px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="EMail" runat="server" ValidationGroup="Group1" ErrorMessage="!!" Display="Dynamic" Font-Bold="True" SetFocusOnError="True"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <img src="images/spacer.gif" alt="" width="100%" height="10" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left"><asp:Label ID="Label3" runat="server" Text="(!checkoutanon.aspx.5!)" Font-Bold="true" /></td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:TextBox ID="Password" runat="server" ValidationGroup="Group1" MaxLength="50" CausesValidation="True" TextMode="Password" Width="155px"></asp:TextBox>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top" align="left">
                                            <br />
                                            <div class="buttonwrapper">
                                            <aspdnsfc:NiceButton ID="btnSignInAndCheckout" CssClass="UpdateCartButton" Text="(!checkoutanon.aspx.13!)" runat="server" ValidationGroup="Group1" CausesValidation="true" OnClick="btnSignInAndCheckout_Click" />
                                            </div>
                                            <br /><br />
                                            <asp:Label ID="Label5" runat="server" Text="(!checkoutanon.aspx.7!)" Font-Bold="true" /><br /><br />
                                            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="signin.aspx?checkout=true">(!checkoutanon.aspx.8!)</asp:HyperLink>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td valign="top" width="20" rowspan="2"></td>
                            <td valign="top" width="240"  align="left">
                                <asp:Label ID="Label7" runat="server" Text="(!checkoutanon.aspx.9!)" Font-Bold="true" /><br /><br />
                                <div>
                                <aspdnsfc:NiceButton ID="RegisterAndCheckoutButton" CssClass="UpdateCartButton" Text="(!checkoutanon.aspx.14!)" runat="server" CausesValidation="false" OnClick="RegisterAndCheckoutButton_Click"  />
                                </div>
                                <br /><br />
                                <aspdnsf:Topic runat="server" ID="Teaser" TopicName="CheckoutAnonTeaser" />
                                <asp:Panel runat="Server" ID="PasswordOptionalPanel" Visible="false">
                                    <br /><br />
                                    <asp:Label ID="Label8" runat="server" Text="(!checkoutanon.aspx.10!)" Font-Bold="true" /><br /><br />
                                    <asp:Label ID="Label9" runat="server" Text="(!checkoutanon.aspx.11!)" /><br /><br />
                                    <div>
                                    <aspdnsfc:NiceButton ID="Skipregistration" CssClass="UpdateCartButton" Text="(!checkoutanon.aspx.15!)" runat="server" CausesValidation="false" OnClick="Skipregistration_Click" />
                                    </div>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
            </asp:Panel>
            <asp:Panel ID="ExecutePanel" runat="server" Width="90%" HorizontalAlign="center" Visible="false">
                    <img src="images/spacer.gif" alt="" width="100%" height="40" />
                    <asp:Label ID="SignInExecuteLabel" runat="server" Font-Bold="true"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlChangePwd" runat="server" Visible="false">
                <table width="100%">
                    <tbody>
                        <tr valign="top" >
                            <td align="left" width="90%" align="left">
                                <asp:Label ID="Label10" runat="server" Text="(!signin.aspx.22!)" Font-Bold="true" ForeColor="red"></asp:Label><br/><br/>
                                <asp:Label ID="lblPwdChgErr" runat="server" Font-Bold="true" ForeColor="red" Visible="false"></asp:Label>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td class="MediumCell" align="left" width="90%">
                                &nbsp;<b><asp:Label ID="Label19" runat="server" Text="(!signin.aspx.8!)"></asp:Label></b>
                            </td>
                        </tr>
                        <tr class="MediumCell" valign="top">
                            <td class="LightCell" align="left" width="90%" align="left">
                                <table cellspacing="5" cellpadding="0" width="100%" border="0">
                                    <tbody>
                                        <tr valign="baseline">
                                            <td colspan="2" align="left">
                                                <b><font class="LightCellText">
                                                    <asp:Label ID="Label13" runat="server" Text="(!signin.aspx.23!)"></asp:Label>
                                                </font></b>
                                            </td>
                                        </tr>
                                        <tr valign="baseline">
                                            <td valign="middle" align="right">
                                                <asp:Label ID="Label14" runat="server" Text="(!signin.aspx.10!)"></asp:Label>
                                            </td>
                                            <td valign="middle" align="left">
                                                <asp:TextBox ID="CustomerEmail" runat="server" ValidationGroup="Group3" Columns="30" MaxLength="100" CausesValidation="True" AutoCompleteType="Email"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ValidationGroup="Group3" ErrorMessage="(!signin.aspx.3!)" ControlToValidate="CustomerEmail"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="middle" align="right">
                                                <asp:Label ID="Label15" runat="server" Text="Old Password"></asp:Label>
                                            </td>
                                            <td valign="middle" align="left">
                                                <asp:TextBox ID="OldPassword" runat="server" ValidationGroup="Group3" Columns="30" MaxLength="50" CausesValidation="True" TextMode="Password"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ValidationGroup="Group3" ErrorMessage="(!signin.aspx.4!)" ControlToValidate="OldPassword"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="middle" align="right">
                                                <asp:Label ID="Label16" runat="server" Text="New Password"></asp:Label>
                                            </td>
                                            <td valign="middle" align="left">
                                                <asp:TextBox ID="NewPassword" runat="server" ValidationGroup="Group3" Columns="30" MaxLength="50" CausesValidation="True" TextMode="Password"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ValidationGroup="Group3" ErrorMessage="(!signin.aspx.4!)" ControlToValidate="NewPassword"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="middle" align="right">
                                                <asp:Label ID="Label17" runat="server" Text="Confirm New Password"></asp:Label>
                                            </td>
                                            <td valign="middle" align="left">
                                                <asp:TextBox ID="NewPassword2" runat="server" ValidationGroup="Group3" Columns="30" MaxLength="50" CausesValidation="True" TextMode="Password"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ValidationGroup="Group3" ErrorMessage="(!signin.aspx.4!)" ControlToValidate="NewPassword2"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr valign="baseline">
                                            <td valign="middle" align="right">
                                                <font class="LightCellText">
                                                    <asp:Label ID="Label18" runat="server" Text="(!signin.aspx.21!)" Visible="False"></asp:Label></font></td>
                                            <td valign="middle" align="left">
                                                <asp:TextBox ID="SecurityCode2" runat="server" Visible="False" ValidationGroup="Group3" CausesValidation="True" Width="73px" EnableViewState="False"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="SecurityCode2" ErrorMessage="(!signin.aspx.20!)" ValidationGroup="Group3" Enabled="False"></asp:RequiredFieldValidator></td>
                                        </tr>
                                        <tr valign="baseline">
                                            <td valign="middle" align="center" colspan="2">
                                                <asp:Image ID="Image1" runat="server" Visible="False"></asp:Image>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td align="left" width="90%">
                                <p align="right">
                                   <div>
                                   <aspdnsfc:NiceButton ID="btnChgPwd" OnClick="btnChgPwd_Click" runat="server" Text="Change Password" ValidationGroup="Group1"/>
                                   </div>
                                </p>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </asp:Panel>
        
        </div>
    </form>
</body>
</html>
