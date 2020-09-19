<%@ Page language="c#" Inherits="AspDotNetStorefront.checkoutpayment" CodeFile="checkoutpayment.aspx.cs" %>
<%@ Register TagPrefix="aspdnsfc" Namespace="ASPDNSFControls" Assembly="ASPDNSFControls" %>
<%@ Register TagPrefix="aspdnsf" TagName="Topic" src="TopicControl.ascx" %>
<%@ Register TagPrefix="aspdnsf" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <asp:Literal ID="JSPopupRoutines" runat="server"></asp:Literal>

    <asp:Panel ID="pnlHeaderGraphic" runat="server" HorizontalAlign="center">
        <asp:ImageMap ID="checkoutheadergraphic" HotSpotMode="PostBack" runat="server">
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx" Top="0" Left="0" Bottom="54" Right="87" />
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/account.aspx?checkout=true" Top="0" Left="87" Bottom="54" Right="173" />
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Inactive" NavigateUrl="~/checkoutshipping.aspx" Top="0" Left="173" Bottom="54" Right="259" />
        </asp:ImageMap>
    </asp:Panel>
    
    <asp:Panel ID="pnlErrorMsg" runat="server" Visible="false">
        <asp:Label ID="ErrorMsgLabel" CssClass="error" runat="server"></asp:Label>
        <br/>
    </asp:Panel>
    
    <aspdnsf:Topic runat="server" ID="CheckoutPaymentPageHeader" TopicName="CheckoutPaymentPageHeader" />
    <asp:Literal ID="XmlPackage_CheckoutPaymentPageHeader" runat="server" Mode="PassThrough"></asp:Literal>

    <form runat="server">
        <asp:Panel ID="pnlNoPaymentRequired" runat="server" HorizontalAlign="Center" Visible="false">
            <asp:Label ID="NoPaymentRequired" runat="server" Font-Bold="true" ForeColor="blue"></asp:Label><br /><br />
            <asp:Literal ID="Finalization" runat="server" Mode="PassThrough"></asp:Literal>
            <div class="buttonwrapper">
        <center><div style="width:300px"><aspdnsfc:NiceButton ID="btnContinueCheckOut1" runat="server" Text="(!checkoutpayment.aspx.18!)" CssClass="PaymentPageContinueCheckoutButton" />
        </div>
        </center>
        </div>
        </asp:Panel>
        <asp:Panel ID="pnlPaymentOptions" runat="server" HorizontalAlign="left" Visible="true">
            <table cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtCreditCard" runat="server" Visible="false" AutoPostBack="true" /></td>
                    <td><asp:Image ID="CCIMage" runat="server" Visible="false" /></td>
                </tr>
                <tr>
                    <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtPURCHASEORDER" Text="(!checkoutpayment.aspx.8!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                    <td></td>
                </tr>
                <tr>
                    <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtCODMONEYORDER" Text="(!checkoutpayment.aspx.24!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                    <td></td>
                </tr>
                <tr>
                    <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtCODCOMPANYCHECK" Text="(!checkoutpayment.aspx.22!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                    <td></td>
                </tr>
                <tr>
                    <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtCODNET30" Text="(!checkoutpayment.aspx.23!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                    <td></td>
                </tr>
                <tr>
                    <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtPAYPAL" Text="" runat="server" Visible="false" AutoPostBack="true" /></td>
                    <td><asp:Image ID="PayPalImage" runat="server" Visible="false" /></td>
                </tr>
                <tr>
                    <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtREQUESTQUOTE" Text="(!checkoutpayment.aspx.10!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                    <td></td>
                </tr>
                <tr>
                    <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtCHECKBYMAIL" Text="(!checkoutpayment.aspx.11!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                    <td></td>
                </tr>
                <tr>
                    <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtCOD" Text="(!checkoutpayment.aspx.12!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                    <td></td>
                </tr>
                <tr>
                    <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtECHECK" Text="(!checkoutpayment.aspx.13!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                    <td></td>
                </tr>
                <tr>
                    <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtMICROPAY" Text="(!checkoutpayment.aspx.14!)" runat="server" Visible="false" AutoPostBack="true" /></td>
                    <td></td>
                </tr>
                <tr>
                    <td><asp:RadioButton GroupName="PaymentSelection" ID="pmtPAYPALEXPRESS" Text="" runat="server" Visible="false" AutoPostBack="true" /></td>
                    <td><asp:Image ID="PayPalExpressImage" runat="server" Visible="false" /></td>
                </tr>
            </table>
            <br />
        </asp:Panel>
    </form>

        <asp:Panel ID="paymentPanes" runat="server" HorizontalAlign="left" style="width: 90%; border: 1px; border-style: solid; padding-left: 10px; padding-top: 10px; padding-right: 10px; padding-bottom: 20px;">
            <asp:Panel ID="pnlCreditCardPane" runat="server" Visible="false">
                <aspdnsf:Topic runat="server" ID="CheckoutCreditCardPageHeader" TopicName="CheckoutCreditCardPageHeader" />
                <asp:Literal ID="CCForm" runat="server" Mode="passThrough"></asp:Literal>
            </asp:Panel>
            <asp:Panel ID="pnlECheckPane" runat="server" Visible="false">
                <aspdnsf:Topic runat="server" ID="CheckoutECheckPageHeader" TopicName="CheckoutECheckPageHeader" />
                <asp:Literal ID="ECheckForm" runat="server" Mode="passThrough"></asp:Literal>
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
        
        <asp:Panel ID="pnlOrderSummary" runat="server">
            <asp:Literal ID="OrderSummary" Mode="PassThrough" runat="server"></asp:Literal>
        </asp:Panel>
        
        <aspdnsf:Topic runat="server" ID="CheckoutPaymentPageFooter" TopicName="CheckoutPaymentPageFooter" />
        <asp:Literal ID="XmlPackage_CheckoutPaymentPageFooter" runat="server" Mode="PassThrough"></asp:Literal>
</body>
</html>