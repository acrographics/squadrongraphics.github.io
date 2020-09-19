<%@ Page Language="C#" AutoEventWireup="true" CodeFile="checkoutgiftcard.aspx.cs" Inherits="AspDotNetStorefront.checkoutgiftcard" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title></title>
</head>
<body>
    <form runat="server">
        <asp:Literal ID="JSPopupRoutines" runat="server"></asp:Literal>
        <asp:Panel ID="pnlHeaderGraphic" runat="server" HorizontalAlign="Center" Visible="false">
            <asp:ImageMap ID="checkoutheadergraphic" HotSpotMode="PostBack" runat="server">
                <asp:RectangleHotSpot HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx" Top="0" Left="0" Bottom="54" Right="87" />
                <asp:RectangleHotSpot HotSpotMode="Navigate" NavigateUrl="~/account.aspx?checkout=true" Top="0" Left="87" Bottom="54" Right="173" />
            </asp:ImageMap>
            <asp:HyperLink ID="lnkGiftCard" NavigateUrl="~/checkoutgiftcard.aspx" runat="server"></asp:HyperLink>
        </asp:Panel>
        <br />
        <asp:Panel ID="pnlEmailGiftCards" runat="server" HorizontalAlign="Center">
            <asp:Label ID="lblErrMsg" runat="server" Font-Bold="true" ForeColor="red" Visible="false"></asp:Label>
            <br />
            <asp:Repeater ID="rptrEmailGiftCards" runat="server">
                <HeaderTemplate>
                    <table width="100%">
                        <tr>
                            <td align="center">
                </HeaderTemplate>
                <ItemTemplate>
                    <table>
                        <tr>
                          <th colspan="2" valign="top" bgcolor="#cccccc">
                            <%# AspDotNetStorefrontCommon.XmlCommon.GetLocaleEntry(DataBinder.Eval(Container.DataItem, "ProductName").ToString(), ThisCustomer.LocaleSetting, true) + AspDotNetStorefrontCommon.XmlCommon.GetLocaleEntry(DataBinder.Eval(Container.DataItem, "VariantName").ToString(), ThisCustomer.LocaleSetting, true)%>
                            <%# AspDotNetStorefrontCommon.CommonLogic.IIF(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "InitialAmount")) == 0, "", DataBinder.Eval(Container.DataItem, "InitialAmount", "{0:$#,##0.00}"))%>
                          </th>
                        </tr>
                        <tr>
                          <td>
                            <b>Recipient's Name:</b>
                            <br /><asp:TextBox ID="giftcardid" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "GiftCardID") %>' Visible="false"></asp:TextBox>
                            <asp:TextBox ID="EmailName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EmailName") %>' Columns="35"></asp:TextBox>
                          </td>
                          <td>
                            <b>Recipient's Email Address:</b>
                            <br />
                            <asp:TextBox ID="EmailTo" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EmailTo") %>' Columns="35"></asp:TextBox>
                          </td>
                        </tr>
                        <tr>
                          <th colspan="2">
                            Message for Recipient:<br />
                            <asp:TextBox ID="EmailMessage" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EmailMessage") %>' TextMode="MultiLine" Columns="75" Rows="4"></asp:TextBox>
                          </th>
                        </tr>
                    </table>
                </ItemTemplate>
                <FooterTemplate>
                            </td>
                        </tr>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            &nbsp;&nbsp;<asp:Button ID="btnContinueCheckout" runat="server" OnClick="btnContinue_Click" Text="Continue Checkout" />
        </asp:Panel>
        <asp:Literal ID="GiftCardXmlPackage" runat="Server" Mode="PassThrough"></asp:Literal>
    </form>
</body>
</html>
