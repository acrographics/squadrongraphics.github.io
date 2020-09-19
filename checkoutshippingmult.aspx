<%@ Page language="c#" Inherits="AspDotNetStorefront.checkoutshippingmult" CodeFile="checkoutshippingmult.aspx.cs" %>
<%@ Register TagPrefix="aspdnsfc" Namespace="ASPDNSFControls" Assembly="ASPDNSFControls" %>
<%@ Register TagPrefix="aspdnsf" TagName="Topic" src="TopicControl.ascx" %>
<%@ Register TagPrefix="aspdnsf" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <asp:Literal ID="JSPopupRoutines" runat="server"></asp:Literal>
    
    <asp:Panel ID="headergraphic" runat="server" HorizontalAlign="Center">
        <asp:ImageMap ID="checkoutheadergraphic" HotSpotMode="PostBack" runat="server">
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx" Top="0" Left="0" Bottom="54" Right="87" />
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/account.aspx?checkout=true" Top="0" Left="87" Bottom="54" Right="173" />
        </asp:ImageMap>
    </asp:Panel>
    
    <asp:Panel ID="pnlErrorMsg" runat="server" Visible="false">
        <asp:Label ID="ErrorMsgLabel" CssClass="errorLg" runat="server"></asp:Label>
    </asp:Panel>
    
    <aspdnsf:Topic runat="server" ID="CheckoutShippingMultPageHeader" TopicName="CheckoutShippingMultPageHeader" />
    <asp:Literal ID="XmlPackage_CheckoutShippingPageHeader" runat="server" Mode="PassThrough"></asp:Literal>

    <asp:Panel ID="pnlGetFreeShipping" runat="server" CssClass="FreeShippingThresholdPrompt" Visible="false">
        <asp:Literal ID="GetFreeShipping" runat="server" Mode="PassThrough"></asp:Literal>
    </asp:Panel>
    
    <asp:Panel ID="pnlIsFreeShipping" runat="server" CssClass="FreeShippingThresholdPrompt" Visible="false">
        <asp:Literal ID="IsFreeShipping" runat="server" Mode="PassThrough"></asp:Literal>
    </asp:Panel>
    
    <asp:Literal ID="checkoutshippingmultaspx16" Mode="PassThrough" runat="server"></asp:Literal>
    <asp:Literal ID="checkoutshippingmultaspx18" Mode="PassThrough" runat="server"></asp:Literal>
    
    <asp:Literal ID="CartItems" Mode="PassThrough" runat="server"></asp:Literal>
    
    <aspdnsf:Topic runat="server" ID="CheckoutShippingMultPageFooter" TopicName="CheckoutShippingMultPageFooter" />
    <asp:Literal ID="XmlPackage_CheckoutShippingMultPageFooter" runat="server" Mode="PassThrough"></asp:Literal>

</body>
</html>