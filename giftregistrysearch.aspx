<%@ Page language="c#" Inherits="AspDotNetStorefront.giftregistrysearch" CodeFile="giftregistrysearch.aspx.cs" %>
<html>
<head></head>
<body>
    <form runat="server">
        <asp:Table ID="tblGiftRegistrySearch" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
            <asp:TableRow>
                <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                    <asp:Image ID="giftregistry3_gif" runat="server" /><br/>

                    <asp:Table ID="tblGiftRegistrySearchBox" CellSpacing="0" CellPadding="3" Width="100%" runat="server">
                        <asp:TableRow>
                            <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                <asp:Literal ID="giftregistrysearch_aspx_2" runat="server" Mode="PassThrough"></asp:Literal>
                                
                                <table width="100%">
                                    <tr>
                                        <td width="33%" align="center" valign="top">
                                            <asp:Literal ID="giftregistrysearch_aspx_3" runat="server" Mode="PassThrough"></asp:Literal><br />
                                        </td>
                                        <td width="33%" align="center" valign="top">
                                            <asp:Literal ID="giftregistrysearch_aspx_4" runat="server" Mode="PassThrough"></asp:Literal><br />
                                        </td>
                                        <td width="33%" align="center" valign="top">
                                            <asp:Literal ID="giftregistrysearch_aspx_5" runat="server" Mode="PassThrough"></asp:Literal><br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td width="33%" align="center" valign="top">
                                            <asp:TextBox ID="txtSearchForName"   MaxLength="100" runat="server" TabIndex="1"></asp:TextBox><br />
                                        </td>
                                        <td width="33%" align="center" valign="top">
                                            <asp:TextBox ID="txtSearchForNickName" MaxLength="100" runat="server" TabIndex="3"></asp:TextBox><br />
                                        </td>
                                        <td width="33%" align="center" valign="top">
                                            <asp:TextBox ID="txtSearchForEMail" MaxLength="100" runat="server" TabIndex="5"></asp:TextBox><br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td width="33%" align="center" valign="top">
                                            <asp:Button ID="btnSearchForName" TabIndex="2" CssClass="GiftRegistrySearchButton" runat="server" OnClick="btnSearchForName_Click" />
                                        </td>
                                        <td width="33%" align="center" valign="top">
                                            <asp:Button ID="btnSearchForNickName" TabIndex="4" CssClass="GiftRegistrySearchButton" runat="server" OnClick="btnSearchForNickName_Click" />
                                        </td>
                                        <td width="33%" align="center" valign="top">
                                            <asp:Button ID="btnSearchForEMail" TabIndex="6" CssClass="GiftRegistrySearchButton" runat="server" OnClick="btnSearchForEMail_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <asp:Panel ID="pnlSearchResults" runat="server" Visible="false">
            <br />
            <p align="left">
                <asp:Literal ID="giftregistry_aspx_16" runat="server" Mode="PassThrough" ></asp:Literal>
                <asp:Button ID="btnSaveButton" runat="server" CssClass="GiftRegistrySaveButton" Visible="true" OnClick="btnSaveButton_Click" />
            </p>
            <asp:Table ID="tblSearchResults" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                        <asp:Image ID="giftregistry5_gif" runat="server" /><br/>

                        <asp:Table ID="tblSearchResultsBox" CellSpacing="0" CellPadding="4" Width="100%" runat="server">
                            <asp:TableRow>
                                <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                    <asp:Literal ID="SearchResults" runat="server" Mode="PassThrough"></asp:Literal>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
    </form>
</body>
</html>