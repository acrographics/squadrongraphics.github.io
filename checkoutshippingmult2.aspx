<%@ Page language="c#" Inherits="AspDotNetStorefront.checkoutshippingmult2" CodeFile="checkoutshippingmult2.aspx.cs" %>
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
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/checkoutshippingmult.aspx" Top="0" Left="173" Bottom="54" Right="259" />
        </asp:ImageMap>
    </asp:Panel>
    
    <asp:Panel ID="pnlErrorMsg" runat="server" Visible="false">
        <asp:Label ID="ErrorMsgLabel" CssClass="errorLg" runat="server"></asp:Label>
    </asp:Panel>
    
    <aspdnsf:Topic runat="server" ID="CheckoutShippingMult2PageHeader" TopicName="CheckoutShippingMult2PageHeader" />
    <asp:Literal ID="XmlPackage_CheckoutShippingMult2PageHeader" runat="server" Mode="PassThrough"></asp:Literal>

    <asp:Panel ID="pnlGetFreeShipping" runat="server" CssClass="FreeShippingThresholdPrompt" Visible="false">
        <asp:Literal ID="GetFreeShipping" runat="server" Mode="PassThrough"></asp:Literal>
    </asp:Panel>
    
    <asp:Panel ID="pnlIsFreeShipping" runat="server" CssClass="FreeShippingThresholdPrompt" Visible="false">
        <asp:Literal ID="IsFreeShipping" runat="server" Mode="PassThrough"></asp:Literal>
    </asp:Panel>
    
    <asp:Literal ID="checkoutshippingmult2aspx16" Mode="PassThrough" runat="server"></asp:Literal>

    <asp:Literal ID="CartItems" Mode="PassThrough" runat="server"></asp:Literal>
    
    <aspdnsf:Topic runat="server" ID="CheckoutShippingMult2PageFooter" TopicName="CheckoutShippingMult2PageFooter" />
    <asp:Literal ID="XmlPackage_CheckoutShippingMult2PageFooter" runat="server" Mode="PassThrough"></asp:Literal>

</body>
</html>