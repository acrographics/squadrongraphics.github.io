<%@ Page language="c#" Inherits="AspDotNetStorefront.checkoutreview" CodeFile="checkoutreview.aspx.cs" %>
<%@ Register TagPrefix="aspdnsfc" Namespace="ASPDNSFControls" Assembly="ASPDNSFControls" %>
<%@ Register TagPrefix="aspdnsf" TagName="Topic" src="TopicControl.ascx" %>
<%@ Register TagPrefix="aspdnsf" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <asp:Panel ID="TopPanel" runat="server">
    <asp:Literal ID="JSPopupRoutines" runat="server"></asp:Literal>
    <div style="text-align:center;">
        <div style="text-align:center;">
            <asp:ImageMap ID="checkoutheadergraphic" HotSpotMode="PostBack" runat="server">
                <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx" Top="0" Left="0" Bottom="54" Right="87" />
                <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/account.aspx?checkout=true" Top="0" Left="87" Bottom="54" Right="173" />
                <asp:RectangleHotSpot AlternateText="" HotSpotMode="Inactive" NavigateUrl="~/checkoutshipping.aspx" Top="0" Left="173" Bottom="54" Right="259" />
                <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/checkoutpayment.aspx" Top="0" Left="259" Bottom="54" Right="345" />
            </asp:ImageMap>
        </div>

        <aspdnsf:Topic runat="server" ID="CheckoutReviewPageHeader" TopicName="CheckoutReviewPageHeader" />
        <asp:Literal ID="XmlPackage_CheckoutReviewPageHeader" runat="server" Mode="PassThrough"></asp:Literal>
        
        <form runat="server" action="checkoutreview.aspx">
            <br />
            <asp:Literal ID="checkoutreviewaspx6" Mode="PassThrough" runat="server" Text="(!checkoutreview.aspx.6!)"></asp:Literal>
            <br/><br/>
             <div class="buttonwrapper">
        <center><div style="width:200px"><aspdnsfc:NiceButton ID="btnContinueCheckout1" Text="(!checkoutreview.aspx.7!)" CssClass="ReviewPageContinueCheckoutButton" runat="server" OnClick="btnContinueCheckout1_Click" /></div></center></div>
            <br /><br />

            <table width="100%">
                <tr>
                    <td width="50%" align="left" valign="top">
                        <asp:Label ID="checkoutreviewaspx8" Text="(!checkoutreview.aspx.8!)" Font-Bold="true" runat="server"></asp:Label>
                        <br />
                        <asp:Literal ID="litBillingAddress" runat="server" Mode="PassThrough"></asp:Literal>
                        <br /><br />
                        <asp:Label ID="checkoutreviewaspx9" Text="(!checkoutreview.aspx.9!)" Font-Bold="true" runat="server"></asp:Label>
                        <br />
                        <asp:Literal ID="litPaymentMethod" runat="server" Mode="PassThrough"></asp:Literal>
                    </td>
                    <td width="50%" align="left" valign="top">
                        <asp:Label ID="ordercs57" Text="(!order.cs.57!)" Font-Bold="true" runat="server"></asp:Label>
                        <br />
                        <asp:Literal ID="litShippingAddress" runat="server" Mode="PassThrough"></asp:Literal>
                    </td>
                </tr>
            </table>
            <br />
            <div id="pnlOrderSummary">
            <asp:Literal ID="CartSummary" Mode="PassThrough" runat="server"></asp:Literal>
            </div>
            <br />
            
             <div class="buttonwrapper">
        <center><div style="width:200px"><aspdnsfc:NiceButton ID="btnContinueCheckout2" Text="(!checkoutreview.aspx.7!)" CssClass="ReviewPageContinueCheckoutButton" runat="server" OnClick="btnContinueCheckout2_Click" /></div></center></div>
            <br /><br />

            <aspdnsf:Topic runat="server" ID="CheckoutReviewPageFooter" TopicName="CheckoutReviewPageFooter" />
            <asp:Literal ID="XmlPackage_CheckoutReviewPageFooter" runat="server" Mode="PassThrough"></asp:Literal>
        </form>
    </div>
    </asp:Panel>
</body>
</html>
