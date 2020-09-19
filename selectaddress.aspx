<%@ Page language="c#" Inherits="AspDotNetStorefront.selectaddress" CodeFile="selectaddress.aspx.cs" %>
<html>
<head>
</head>
<body>
    <form runat="server">
        <asp:Panel ID="pnlCheckoutImage" runat="server" HorizontalAlign="Center" Visible="false">
            <asp:ImageMap ID="CheckoutImage" HotSpotMode="Navigate" runat="server">
                <asp:RectangleHotSpot AlternateText="Back To Step 1: Shopping Cart" Top="0" Left="0" Right="87" Bottom="54" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx?resetlinkback=1" />
            </asp:ImageMap>
            <br />
        </asp:Panel>

        <asp:Panel ID="pnlAddressList" runat="server" Visible="false">
            <asp:Table ID="tblAddressList" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                        <asp:Image runat="server" ID="addressbook_gif" /><br />
                        <asp:Table ID="tblAddressListBox" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                            <asp:TableRow>
                                <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                    <asp:Panel ID="pnlNewAddress" runat="server" Visible="false" HorizontalAlign="Center">
                                        <asp:Literal ID="litNewAddressForm" Mode="PassThrough" runat="server"></asp:Literal>
                                        <asp:Label ID="lblErrMsg" Font-Bold="true" ForeColor="red" runat="server"></asp:Label>
                                        <br />
                                        <br />
                                        <asp:Button ID="btnNewAddress" runat="server" CssClass="SelectAddressButton" />
                                        <br />
                                    </asp:Panel>
                                    <asp:Panel ID="pnlAddressListBottom" runat="server" Visible="true">
                                        <hr />
                                        <br />
                                        <ol>
                                        <asp:Repeater ID="AddressList" runat="server">
                                            <ItemTemplate>
                                                <li>
                                                <%#  AspDotNetStorefrontCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "PrimaryAddress").ToString() == "1", "<b>", "")%>
                                                        <%# DataBinder.Eval(Container.DataItem, "FirstName") %> <%# DataBinder.Eval(Container.DataItem, "LastName") %>
                                                        &nbsp;&nbsp;
                                                        <asp:ImageButton ID="btnMakePrimary" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "AddressID") %>' style="vertical-align: middle;" CommandName="makeprimary" runat="server" />
                                                        &nbsp;&nbsp;
                                                        <asp:ImageButton ID="btnEdit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "AddressID") %>' style="vertical-align: middle;" CommandName="edit" runat="server" />
                                                        &nbsp;&nbsp;
                                                        <asp:ImageButton ID="btnDelete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "AddressID") %>' style="vertical-align: middle;" CommandName="delete" runat="server" />
                                                        <br />
                                                        <%# AspDotNetStorefrontCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Company").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "Company") + "<br />")%>
                                                        <%# AspDotNetStorefrontCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Address1").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "Address1") + "<br />")%>
                                                        <%# AspDotNetStorefrontCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Address2").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "Address2") + "<br />")%>
                                                        <%# AspDotNetStorefrontCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Suite").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "Suite") + "<br />")%>
                                                        <%# AspDotNetStorefrontCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "CityStateZip").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "CityStateZip") + "<br />")%>
                                                        <%# AspDotNetStorefrontCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Country").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "Country") + "<br />")%>
                                                        <%# AspDotNetStorefrontCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Phone").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "Phone") + "<br />")%>
                                                        
                                                        <%# AspDotNetStorefrontCommon.CommonLogic.IIF(AddressType == AspDotNetStorefrontCommon.AddressTypes.Billing && DataBinder.Eval(Container.DataItem, "PaymentMethodLastUsed").ToString().Length != 0, this.DisplayPaymentMethod(this.ThisCustomer, DataBinder.Eval(Container.DataItem, "PaymentMethodLastUsed").ToString(), Convert.ToInt32(DataBinder.Eval(Container.DataItem, "CustomerID").ToString()), Convert.ToInt32(DataBinder.Eval(Container.DataItem, "AddressID").ToString())), "")%>

                                                <%#  AspDotNetStorefrontCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "PrimaryAddress").ToString() == "1", "</b>", "")%>
                                                <br />
                                                </li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <asp:Panel ID="liAdd" runat="server" Visible="true">
                                            <li>
                                                <asp:HyperLink ID="lnkAddAddress" runat="server"></asp:HyperLink>
                                            </li>
                                        </asp:Panel>
                                        </ol>
                                   </asp:Panel>                                    
                              </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
        <asp:Panel ID="pnlNoAddresses" runat="server" Visible="false"><asp:Literal ID="litNoAddresses" runat="server" Mode="PassThrough"></asp:Literal></asp:Panel>
        <p align="center"><asp:Button ID="btnReturn" runat="server" /><asp:Button ID="btnCheckOut" runat="server" Visible="false" /></p>
    </form>
</body>
</html>