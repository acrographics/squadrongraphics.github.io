<%@ Page Language="C#" AutoEventWireup="true" CodeFile="feeds.aspx.cs" Inherits="feeds" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
</head>
<body>
    <form id="form1" runat="server">
        <div id="help">
            <div>
                <table border="0" cellpadding="1" cellspacing="0" class="outerTable">
                    <tr>
                        <td>
                            <div class="wrapper">
                                <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                                    <tr>
                                        <td class="titleTable">
                                            <font class="subTitle">Now In:</font>
                                        </td>
                                        <td class="contentTable">
                                            <font class="title">
                                                Feeds
                                            </font>
                                        </td>
                                        <td style="width: 10px;" />
                                        <td class="titleTable">
                                            <font class="subTitle">View:</font>
                                        </td>
                                        <td class="contentTable">
                                            <a href="sitemap2.aspx">Site Map</a>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <br/>
        <div id="Div1"><b>Please refer to your feed provider manual (e.g. froogle.com) for the correct values for all fields entered.</b><br /><br /></div>
        <div id="content">
        <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
            <tr>
                <td>
                    <div class="wrapper">                       
                        <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                            <tr>
                                <td class="titleTable">
                                    <font class="subTitle">Import Prices:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="100%">
                                    <div class="wrapperLeft">
                                        <asp:Panel ID="pnlFeeds" runat="server">
                                            <asp:Button ID="btnAddFeed1" runat="server" CssClass="normalButton" Text="Add New Feed" OnClientClick="document.location.href='editfeed.aspx'; return false;" />
                                            <br /><br />
                                            <asp:Label ID="lblError" runat="server" ForeColor="red" Visible="false"></asp:Label>
                                            <table width="100%">
                                                <asp:Repeater ID="rptrFeeds" runat="server" OnItemCommand="rptrFeeds_ItemCommand" OnItemDataBound="rptrFeeds_ItemDataBound">
                                                    <HeaderTemplate>
                                                        <tr bgcolor="#DDDDDD">
                                                            <th width="5%" align="left">ID</th>
                                                            <th align="left">Feed Name</th>
                                                            <th align="left">XmlPackage</th>
                                                            <th width="10%">Edit</th>
                                                            <th width="10%">Execute</th>
                                                            <th width="10%">Delete</th>
                                                        </tr>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <tr bgcolor="#DDDDDD">
                                                            <td><%# DataBinder.Eval(Container.DataItem, "FeedID") %></td>
                                                            <td><asp:HyperLink ID="lnkFeedEdit" runat="server" NavigateUrl='<%# "editfeed.aspx?feedid=" + DataBinder.Eval(Container.DataItem, "FeedID").ToString()%>' Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'></asp:HyperLink></td>
                                                            <td><%# DataBinder.Eval(Container.DataItem, "XmlPackage") %></td>
                                                            <td align="center"><asp:Button ID="btnEditFeed" runat="server" CssClass="normalButton" Text="Edit Feed" OnClientClick='<%# "EditFeed(" + DataBinder.Eval(Container.DataItem, "FeedID").ToString() + "); return false;"%>' /></td>
                                                            <td align="center"><asp:Button ID="btnExecuteFeed" runat="server" CssClass="normalButton" Text="Execute Feed" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "FeedID") %>' CommandName='execute' /></td>
                                                            <td align="center"><asp:Button ID="btnDeleteFeed" runat="server" CssClass="normalButton" Text="Delete Feed" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "FeedID") %>' CommandName='delete' /></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                    <FooterTemplate></FooterTemplate>
                                                </asp:Repeater>
                                            </table>
                                        </asp:Panel>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
<script type="text/javascript" >
    function EditFeed(feedid){
        document.location.href = "editfeed.aspx?feedid=" + feedid;
    }
</script>
