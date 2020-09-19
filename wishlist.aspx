<%@ Page language="c#" Inherits="AspDotNetStorefront.wishlist" CodeFile="wishlist.aspx.cs" %>
<%@ Register TagPrefix="aspdnsf" TagName="Topic" src="TopicControl.ascx" %>
<html>
<head>
</head>
<body>

    <form runat="server" onsubmit="return FormValidator(this)">
        <aspdnsf:Topic runat="server" ID="TopicWishListPageHeader" TopicName="WishListPageHeader" />
        <asp:Literal ID="XmlPackage_WishListPageHeader" runat="server" Mode="PassThrough"></asp:Literal>

        <asp:Literal ID="XmlPackage_WishListPageTopControlLines" runat="server" Mode="PassThrough" Visible="false"></asp:Literal>
        
        <asp:Panel ID="pnlTopControlLines" runat="server" Visible="false">
            <table cellspacing="3" cellpadding="0" width="100%" border="0" id="table1">
                <tr>
                    <td valign="bottom" align="right">
                        <asp:Button ID="btnContinueShopping1" runat="server" CssClass="ContinueShoppingButton" OnClick="btnContinueShopping1_Click" />
                        <asp:Button ID="btnUpdateWishList1" runat="server" CssClass="UpdateWishButton" OnClick="btnUpdateWishList1_Click" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        
        
        <br />
        <asp:Panel ID="Panel1" runat="server" DefaultButton="btnUpdateWishList1">
            <asp:Table ID="tblWishList" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                        <asp:Image ID="wishlist_gif" runat="server" AlternateText="" /><br />

                        <asp:Table ID="tblWishListBox" CellSpacing="0" CellPadding="4" Width="100%" runat="server">
                            <asp:TableRow>
                                <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                    <asp:Literal ID="CartItems" runat="server" Mode="PassThrough"></asp:Literal>                                
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
        
        <asp:Literal ID="Xml_WishListPageBottomControlLines" runat="server" Mode="PassThrough" Visible="false"></asp:Literal>
        
        <asp:Panel ID="pnlBottomControlLines" runat="server">
            <table cellspacing="3" cellpadding="0" width="100%" border="0" id="table2">
                <tr>
                    <td valign="bottom" align="right">
                        <br />
                        <asp:Button ID="btnContinueShopping2" runat="server" CssClass="ContinueShoppingButton" OnClick="btnContinueShopping2_Click" />
                        <asp:Button ID="btnUpdateWishList2" runat="server" CssClass="UpdateWishButton" OnClick="btnUpdateWishList2_Click" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br />
        
        <aspdnsf:Topic runat="server" ID="TopicWishListPageFooter" TopicName="WishListPageFooter" />
        <asp:Literal ID="Xml_WishListPageFooter" runat="server" Mode="PassThrough"></asp:Literal>
    </form>
</body>
</html>
