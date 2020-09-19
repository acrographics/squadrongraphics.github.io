<%@ Page ClientTarget="UpLevel" language="c#" Inherits="AspDotNetStorefront.account" CodeFile="account.aspx.cs" %>
<%@ Register TagPrefix="aspdnsf" TagName="Topic" src="TopicControl.ascx" %>
<%@ Import namespace="AspDotNetStorefrontCommon" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title></title>
</head>
<body>
    <asp:Panel ID="pnlCheckoutImage" runat="server" HorizontalAlign="Center" Visible="false">
        <asp:ImageMap ID="CheckoutImage" HotSpotMode="Navigate" runat="server">
            <asp:RectangleHotSpot Top="0" Left="0" Right="87" Bottom="54" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx?resetlinkback=1" />
        </asp:ImageMap>
    </asp:Panel>
    
    <asp:Label ID="unknownerrormsg" runat="server" style="color:#FF0000;"></asp:Label>
    <asp:Label ID="ErrorMsgLabel" runat="server" style="color:#FF0000;"></asp:Label>
    
    <asp:Panel ID="pnlAccountUpdated" runat="server" HorizontalAlign="left">
        <asp:Label ID="lblAcctUpdateMsg" runat="server" style="font-weight:bold;color:#FF0000;"></asp:Label><br/><br/>
    </asp:Panel>
    
    <asp:Panel ID="pnlNotCheckOutButtons" runat="server" HorizontalAlign="left">
        <asp:Image ID="redarrow1" AlternateText="" runat="server" />&#0160;<b><asp:HyperLink runat="server" ID="accountaspx4" NavigateUrl="#OrderHistory" Text="(!account.aspx.4!)"></asp:HyperLink></b>
        <asp:Panel ID="pnlShowWishButton" runat="server"><asp:Image ID="redarrow2" AlternateText="" runat="server" />&#0160;<b><asp:HyperLink runat="server" ID="ShowWishButtons" NavigateUrl="~/wishlist.aspx" Text="(!account.aspx.58!)"></asp:HyperLink></b></asp:Panel>
        <asp:Panel ID="pnlShowGiftRegistryButtons" runat="server" ><asp:Image ID="redarrow3" AlternateText="" runat="server" />&#0160;<b><asp:HyperLink runat="server" ID="HyperLink1" NavigateUrl="~/giftregistry.aspx" Text="(!account.aspx.59!)"></asp:HyperLink></b></asp:Panel>
        <asp:Panel ID="pnlSubscriptionExpiresOn" runat="server"><br/><asp:Label runat="server" ID="lblSubscriptionExpiresOn"></asp:Label></asp:Panel>
    </asp:Panel>


    <aspdnsf:Topic runat="server" ID="HeaderMsg" TopicName="AccountPageHeader" />

    <form id="AccountForm" runat="server">
        <asp:Textbox ID="OriginalEMail" runat="server" Visible="false" />

        <p><asp:Label ID="note1" runat="server"></asp:Label></p>
        <p><asp:Label ID="MicroPayEnabled" runat="server"></asp:Label></p>
        
        <table width="100%" cellpadding="2" cellspacing="0" border="0" style="border-style: solid; border-width: 0px; border-color: #444444">
            <tr>
              <td align="left" valign="top">
                <asp:Image AlternateText="" ID="imgAccountinfo" runat="server"/>

                <table width="100%" cellpadding="0" cellspacing="0" border="0" style="border-style: solid; border-width: 1px; border-color: #444444;">
                  <tr>
                    <td align="left" valign="top">
                      <table border="0" cellpadding="4" cellspacing="0" width="100%">
                        <tr>
                          <td width="100%" colspan="2"><b><asp:Label ID="accountaspx12" runat="server" Text="(!account.aspx.12!)"></asp:Label></b></td>
                        </tr>

                        <tr>
                          <td width="100%" colspan="2">
                            <hr />
                          </td>
                        </tr>

                        <tr>
                          <td width="35%" align="right" valign="middle"><asp:Label ID="accountaspx13" runat="server" Text="(!account.aspx.13!)"></asp:Label></td>

                          <td width="65%" align="left" valign="middle">
                            <asp:TextBox ID="FirstName" Columns="20" MaxLength="50" runat="server" ValidationGroup="account" CausesValidation="true"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ReqValFirstName" ValidationGroup="account" ControlToValidate="FirstName" Display="none" EnableClientScript="false" ErrorMessage="FirstName field is required" SetFocusOnError="true" runat="server"></asp:RequiredFieldValidator>
                          </td>
                        </tr>

                        <tr>
                          <td width="35%" align="right" valign="middle"><asp:Label ID="accountaspx14" runat="server" Text="(!account.aspx.14!)"></asp:Label></td>

                          <td width="65%" align="left" valign="middle"><asp:TextBox ID="LastName" Columns="20" MaxLength="50" runat="server" ValidationGroup="account" CausesValidation="true"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ReqValLastName" ValidationGroup="account" ControlToValidate="LastName" Display="none" EnableClientScript="false" ErrorMessage="LastName field is required" runat="server"></asp:RequiredFieldValidator></td>
                        </tr>

                        <tr>
                          <td width="36%" align="right" valign="top"><asp:Label ID="accountaspx15" runat="server" Text="(!account.aspx.15!)"></asp:Label></td>

                          <td width="65%" align="left" valign="middle"><asp:TextBox ID="EMail" Columns="37" MaxLength="100" runat="server" ValidationGroup="account" CausesValidation="true"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ReqValEmail" runat="server" ValidationGroup="account" Display="none" EnableClientScript="false" ControlToValidate="EMail" ErrorMessage="E-Mail is required"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator id="RegExValEmail" runat="SERVER" Display="None" ControlToValidate="Email" EnableClientScript="false" ErrorMessage="E-Mail address is invalid" ValidationExpression="^[a-zA-Z0-9][-\w\.\+]*@([a-zA-Z0-9][\w\-]*\.)+[a-zA-Z]{2,4}$"></asp:RegularExpressionValidator></td>
                        </tr>

                        <tr>
                          <td width="35%" align="right" valign="top"><asp:Label ID="accountaspx18" runat="server" Text="(!account.aspx.66!)"></asp:Label></td>

                          <td width="65%" align="left" valign="top">
                            <asp:TextBox ID="CustPassword" TextMode="Password" Columns="20" maxlength="50" runat="server" ValidationGroup="account" CausesValidation="true"></asp:TextBox> <asp:Label ID="accountaspx19" runat="server" Text="(!account.aspx.19!)"></asp:Label>
                            <asp:CustomValidator ID="valPwd" ControlToValidate="CustPassword" EnableClientScript="false" ValidationGroup="account" Display="None" runat="server" OnServerValidate="ValidatePassword"></asp:CustomValidator>
                          </td>
                        </tr>

                        <tr>
                          <td width="35%" align="right" valign="middle"><asp:Label ID="accountaspx21" runat="server" Text="(!account.aspx.67!)"></asp:Label></td>

                          <td width="65%" align="left" valign="middle">
                            <asp:TextBox ID="CustPassword2" TextMode="Password" Columns="20" maxlength="50" runat="server" ValidationGroup="account" CausesValidation="true"></asp:TextBox>
                        </td>
                        </tr>

                        <tr>
                          <td width="35%" align="right" valign="middle"><asp:Label ID="accountaspx23" runat="server" Text="(!account.aspx.23!)"></asp:Label></td>
                          <td width="65%" align="left" valign="middle"><asp:TextBox id="Phone" Columns="14" MaxLength="20" runat="server" /></td>
                          <asp:RequiredFieldValidator ID="ReqValPhone" ValidationGroup="account" ControlToValidate="Phone" Display="none" EnableClientScript="false" ErrorMessage="Phone field is required" SetFocusOnError="true" runat="server"></asp:RequiredFieldValidator>
                        </tr>
                        
                        <tr ID="VATRegistrationIDRow" runat="server">
                          <td width="35%" align="right" valign="middle"><asp:Label ID="Label2" runat="server" Text="(!account.aspx.71!)"></asp:Label></td>
                          <td width="65%" align="left" valign="middle"><asp:TextBox id="VATRegistrationID" Columns="14" MaxLength="20" runat="server" />
                              <asp:Label ID="VATRegistrationIDIsInvalid" runat="server" Font-Bold="True" ForeColor="Red" Text="(!account.aspx.72!)" Visible="False"></asp:Label></td>
                        </tr>

						<asp:panel ID="pnlOver13" runat="server">
                        <tr>
                            <td width="35%" align="right" valign="middle"><asp:Literal ID="Literal1" runat="server" Text="(!createaccount.aspx.78!)"></asp:Literal></td>
                            <td width="65%" align="left" valign="middle"><asp:CheckBox ID="Over13" Visible="true"  runat="server" /></td>
                        </tr>
						</asp:panel>

                        <tr>
                          <td width="35%" align="right" valign="top"><asp:Label ID="accountaspx24" runat="server" Text="(!account.aspx.24!)"></asp:Label><br /><asp:Label ID="accountaspx27" runat="server" Text="(!account.aspx.27!)"></asp:Label></td>

                          <td width="65%" align="left" valign="top">
                            <asp:Label ID="accountaspx25" runat="server" Text="(!account.aspx.25!)"></asp:Label>&#160;
                            <asp:RadioButton ID="OKToEMailYes" GroupName="OKToEMail" runat="server"/> <asp:Label ID="accountaspx26" runat="server" Text="(!account.aspx.26!)"></asp:Label>&#160;
                            <asp:RadioButton ID="OKToEMailNo" GroupName="OKToEMail" runat="server" />
                            </td>
                        </tr>
                        <tr runat="server" id="StoreCCRow">
                          <td width="35%" align="right" valign="top"><asp:Label ID="Label1" runat="server" Text="(!account.aspx.65!)"></asp:Label></td>

                          <td width="65%" align="left" valign="top"><asp:CheckBox ID="ckbSaveCC" runat="server" />
                              <asp:Label ID="YouHaveActiveRecurringOrdersWarning" runat="server" Font-Bold="True" ForeColor="Red" Text="(!account.aspx.70!)" Visible="False"></asp:Label></td>
                        </tr>

                      </table>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
        </table>

        <center>
            <p>
                <asp:Button ID="btnUpdateAccount" OnClientClick="Page_ValidationActive=true;" CssClass="UpdateAccountButton" Text="(!account.aspx.28!)" runat="server" CausesValidation="true" ValidationGroup="account" OnClick="btnUpdateAccount_Click"  />
                <asp:Button ID="btnContinueToCheckOut" CssClass="AccountPageContinueCheckoutButton" Text="(!account.aspx.60!)" runat="server" CausesValidation="false" OnClick="btnContinueToCheckOut_Click" />
            </p>
        </center>
        <asp:ValidationSummary DisplayMode="List" ID="ValSummary" ShowMessageBox="false" runat="server" ShowSummary="true" ValidationGroup="account" ForeColor="red" Font-Bold="true"/>
    </form>

    <br/>
    <table width="100%" cellpadding="2" cellspacing="0" border="0" style="border-style: solid; border-width: 0px; border-color: #444444">
      <tr>
        <td align="left" valign="top">
          <asp:Image AlternateText="" ID="imgAddressbook"  runat="server" />

          <table width="100%" cellpadding="4" cellspacing="0" border="0" style="border-style: solid; border-width: 1px; border-color: #444444;">
            <tr>
              <td align="left" valign="top">
                <table width="100%" border="0">
                  <tr>
                    <td colspan="3">
                      <b><asp:Label ID="accountaspx29" runat="server" Text="(!account.aspx.29!)" ></asp:Label></b><br />
                    </td>
                  </tr>

                  <tr>
                    <td valign="top" width="50%">
                     <asp:Panel ID="pnlBilling" runat="server">
                       <hr />
                        <b><asp:Label ID="accountaspx30" runat="server" Text="(!account.aspx.30!)"></asp:Label>&#160;&#160;&#160;&#160;</b>
                        <asp:HyperLink ID="lnkChangeBilling" runat="server" ></asp:HyperLink><br/>
                        <asp:Literal ID="litBillingAddress" runat="server"></asp:Literal>
                    </asp:Panel>
                    </td>

                    <td valign="top">
                    <asp:Panel ID="pnlShipping" runat="server">
                        <hr />
                        <b><asp:Label ID="accountaspx32" runat="server" Text="(!account.aspx.32!)"></asp:Label>&#160;&#160;&#160;&#160;</b>
                        <asp:HyperLink ID="lnkChangeShipping" runat="Server"></asp:HyperLink><br/>
                        <asp:Literal ID="litShippingAddress" runat="server"></asp:Literal>
                       </asp:Panel>
                    </td>
                  </tr>

                  <tr>
                    <td valign="top" width="50%"><br /><b><asp:HyperLink ID="lnkAddBillingAddress" runat="server"></asp:HyperLink></b></td>
                    <td valign="top">
                       <asp:Panel ID="pnlShipping2" runat="server">
                        <br /><b><asp:HyperLink ID="lnkAddShippingAddress" runat="server"></asp:HyperLink></b>
                        </asp:Panel>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
            
        </td>
      </tr>
    </table>

    <asp:Panel ID="pnlGiftCards" runat="server" Visible="false">
        <br/>
        <br/>
        <asp:Table ID="tblGiftCards" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
            <asp:TableRow>
                <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                    <asp:Image ID="giftcards_gif" runat="server" AlternateText="" /><br />

                    <asp:Table ID="tblGiftCardsBox" CellSpacing="0" CellPadding="4" Width="100%" runat="server">
                        <asp:TableRow>
                            <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                <table width="100%">
                                    <tr><td width="60%"><b>Gift Card Serial #</b></td><td><b>Balance</b></td></tr>
                                    <tr><td colspan="2"><hr/></td></tr>
                                    <asp:Repeater ID="rptrGiftCards" runat="server" >
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# ((AspDotNetStorefrontCommon.GiftCard)Container.DataItem).SerialNumber %></td>
                                                <td><%# ThisCustomer.CurrencyString(((AspDotNetStorefrontCommon.GiftCard)Container.DataItem).Balance) %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </table>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
    
    <br />
    <br />
    <asp:Panel ID="pnlOrderHistory" runat="server">
       
        <asp:Literal ID="RecurringOrders" runat="Server"></asp:Literal>
        <a name="OrderHistory"></a>
        <table width="100%" cellpadding="2" cellspacing="0" border="0" style="border-style: solid; border-width: 0px; border-color: #444444">
          <tr>
            <td width="100%" align="left" valign="top">
              <a href="orderhistory.aspx"><asp:Image AlternateText="" runat="server" ImageUrl="" ID="imgOrderhistory" /> </a><br />
              <table width="100%" cellpadding="4" cellspacing="0" border="0" style="border-style: solid; border-width: 1px; border-color: #444444;">
                <tr>
                  <td width="100%" align="left" valign="top">
                    <table width="100%" cellpadding="2" cellspacing="0" border="1">
                      <tr>
                        <td align="center" valign="top"><b><asp:Label ID="accountaspx36" runat="server" Text="(!account.aspx.36!)" ></asp:Label></b><br />
                        <small><asp:Label ID="accountaspx37" runat="server" Text="(!account.aspx.37!)" ></asp:Label></small></td>

                        <td align="center" valign="top"><b><asp:Label ID="accountaspx38" runat="server" Text="(!account.aspx.38!)"></asp:Label></b></td>

                        <td align="center" valign="top"><b><asp:Label ID="accountaspx39" runat="server" Text="(!account.aspx.39!)"></asp:Label></b></td>

                        <td align="center" valign="top"><b><asp:Label ID="accountaspx40" runat="server" Text="(!account.aspx.40!)"></asp:Label></b></td>

                        <td align="center" valign="top"><b><asp:Label ID="accountaspx41" runat="server" Text="(!account.aspx.41!)"></asp:Label></b></td>

                        <td align="center" valign="top"><b><asp:Label ID="accountaspx42" runat="server" Text="(!account.aspx.42!)"></asp:Label></b></td>
                      </tr>
                        <asp:Repeater ID="orderhistorylist" runat="server">
                            <HeaderTemplate>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td align="center" valign="top">
                                        <a target="_blank" href='<%#m_StoreLoc + "receipt.aspx?ordernumber=" + DataBinder.Eval(Container.DataItem, "OrderNumber") %>'><%# DataBinder.Eval(Container.DataItem, "OrderNumber").ToString() %></a>
                                        <%#GetReorder(DataBinder.Eval(Container.DataItem, "OrderNumber").ToString(), DataBinder.Eval(Container.DataItem, "RecurringSubscriptionID").ToString())%>
                                    </td>
                                    <td align="center" valign="top"><%#AspDotNetStorefrontCommon.Localization.ConvertLocaleDateTime(DataBinder.Eval(Container.DataItem, "OrderDate").ToString(), Localization.GetWebConfigLocale(), ThisCustomer.LocaleSetting)%></td>
                                    <td align="center" valign="top"><%#GetPaymentStatus(DataBinder.Eval(Container.DataItem, "PaymentMethod").ToString(), DataBinder.Eval(Container.DataItem, "CardNumber").ToString(), DataBinder.Eval(Container.DataItem, "TransactionState").ToString())%></td>
                                    <td align="center" valign="top"><%#GetShippingStatus(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "OrderNumber").ToString()), DataBinder.Eval(Container.DataItem, "ShippedOn").ToString(), DataBinder.Eval(Container.DataItem, "ShippedVIA").ToString(), DataBinder.Eval(Container.DataItem, "ShippingTrackingNumber").ToString(), DataBinder.Eval(Container.DataItem, "TransactionState").ToString(), DataBinder.Eval(Container.DataItem, "DownloadEMailSentOn").ToString())%></td>
                                    <td align="center" valign="top"><%#GetOrderTotal(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "QuoteCheckout").ToString()), DataBinder.Eval(Container.DataItem, "PaymentMethod").ToString(), DataBinder.Eval(Container.DataItem, "OrderTotal").ToString(), Convert.ToInt32(DataBinder.Eval(Container.DataItem, "CouponType").ToString()), DataBinder.Eval(Container.DataItem, "CouponDiscountAmount"))%></td>
                                    <td align="center" valign="top"><%#GetCustSvcNotes(DataBinder.Eval(Container.DataItem, "CustomerServiceNotes").ToString())%></td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate></FooterTemplate>
                        </asp:Repeater>
                    </table>

                    <br/>
                    <asp:Label ID="accountaspx55" runat="server" Text="(!account.aspx.55!)"></asp:Label>
                    <br/>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
        </table>
    </asp:Panel>
</body>
</html>
