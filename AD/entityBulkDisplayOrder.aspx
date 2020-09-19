<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.entityBulkDisplayOrder" CodeFile="entityBulkDisplayOrder.aspx.cs" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head id="Head1" runat="server">
        <title></title>
    </head>

    <body style="background-color: #f9f9f9; padding: 0px 0px 0px 0px; margin: 1px 1px 1px 1px;">
        <form id="frmEntityBulk" runat="server">   
            <asp:Label ID="lblpagehdr" runat="server" Font-Bold="true" Visible="false"></asp:Label><br/>
            <asp:Panel ID="pnlSubEntityList" runat="server" Visible="false">
                <table width="100%">
                    <tr>
                        <th align="left"></th>
                        <th align="left"></th>
                        <th align="left"><asp:Button ID="btnTopUpdate" runat="server" Text="Update Order" class="normalButton" OnClick="UpdateDisplayOrder" /></th>
                    </tr>
                    <asp:Repeater ID="subcategories" runat="server">
                        <HeaderTemplate>
                            <tr bgcolor="#DDDDDD">
                                <th align="left">ID</th>
                                <th align="left">Name</th>
                                <th align="left">Display Order</th>
                            </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr bgcolor="#DDDDDD">
                                <td>
                                    <%# ((System.Xml.XmlNode)Container.DataItem)["EntityID"].InnerText%>
                                </td>
                                <td>
                                    <%# getLocaleValue(((System.Xml.XmlNode)Container.DataItem)["Name"], "en-US")%>
                                </td>
                                <td>
                                    <asp:TextBox ID="DisplayOrder" runat="server" Columns="4" Text=<%# ((System.Xml.XmlNode)Container.DataItem)["DisplayOrder"].InnerText%>></asp:TextBox>
                                    <asp:TextBox ID="entityid" runat="server" Visible="false" Text=<%# ((System.Xml.XmlNode)Container.DataItem)["EntityID"].InnerText%>></asp:TextBox>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <tr bgcolor="#DDDDDD">
                                <td>
                                    <%# ((System.Xml.XmlNode)Container.DataItem)["EntityID"].InnerText%>
                                </td>
                                <td>
                                    <%# getLocaleValue(((System.Xml.XmlNode)Container.DataItem)["Name"], "en-US") %>
                                </td>
                                <td>
                                    <asp:TextBox ID="DisplayOrder" runat="server" Columns="4" Text=<%# ((System.Xml.XmlNode)Container.DataItem)["DisplayOrder"].InnerText%>></asp:TextBox>
                                    <asp:TextBox ID="entityid" runat="server" Visible="false" Text=<%# ((System.Xml.XmlNode)Container.DataItem)["EntityID"].InnerText%>></asp:TextBox>
                                </td>
                            </tr>
                        </AlternatingItemTemplate>
                        <FooterTemplate></FooterTemplate>
                    </asp:Repeater>
                    <tr>
                        <th align="left"></th>
                        <th align="left"></th>
                        <th align="left"><asp:Button ID="btnBotUpdate" runat="server" Text="Update Order" class="normalButton" OnClick="UpdateDisplayOrder" /></th>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnlNoSubEntities" runat="server" Visible="true" HorizontalAlign="Center" style="padding-top:30px;">
                <asp:Label ID="lblError" runat="server" Font-Bold="true" Font-Names="verdana"></asp:Label>
            </asp:Panel>
        </form>
    </body>
</html>