<%@ Page Language="C#" AutoEventWireup="true" CodeFile="giftcards.aspx.cs" Inherits="AspDotNetStorefrontAdmin.giftcards" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Gift Cards</title>
</head>

<body>
    <form id="frmGiftcards" runat="server">
    <div id="help">
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
                                        Manage Gift Cards
                                    </font>
                                </td>
                            <!--</tr>
                            <tr>-->
                                <td style="width: 10px;" />
                                <td class="titleTable">
                                    <font class="subTitle">View:</font>
                                </td>
                                <td class="contentTable">
                                    <a href="splash.aspx">Home</a>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
        <div style="margin-bottom: 5px; margin-top: 5px;">
            <asp:Literal ID="ltError" runat="server"></asp:Literal>
        </div>
    </div>
    <div id="content">
        <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
            <tr>
                <td>
                    <div class="wrapper">                       
                        <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                            <tr>
                                <td class="titleTable" width="160">
                                    <font class="subTitle">Gift Card Search:</font>
                                </td>
                                <td style="width: 5px;" />
                                <td style="width: 1px; background-color: #a2a2a2;" />
                                <td style="width: 5px;" />
                                <td class="titleTable">
                                    <font class="subTitle">Gift Cards:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTablePRB" valign="top" width="160">
                                    <asp:TextBox ID="txtSearch" CssClass="singleAutoFull" runat="server"></asp:TextBox>
                                    <asp:DropDownList ID="ddSearch" CssClass="singleAutoFull" runat="server">
                                        <asp:ListItem Value="1">in Customer E-Mail</asp:ListItem>
                                        <asp:ListItem Value="2">in Customer Name</asp:ListItem>
                                        <asp:ListItem Value="3" Selected="true">in Serial Number</asp:ListItem>                                        
                                    </asp:DropDownList>
                                    <br />
                                    <asp:Button runat="server" ID="btnSearch" CssClass="normalButton" Text="Search" OnClick="btnSearch_Click" />
                                    <br /><br />
                                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td class="titleTable">
                                                <font class="subTitle">Gift Card Types:</font>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="contentTableNPLR">
                                                <div>
                                                    <asp:DropDownList CssClass="singleAutoFull" runat="server" ID="ddTypes" AutoPostBack="true" OnSelectedIndexChanged="ddTypes_SelectedIndexChanged">
                                                    </asp:DropDownList>                                                    
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td class="titleTable">
                                                <font class="subTitle">Gift Card Status:</font>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="contentTableNPLR">
                                                <div>
                                                    <asp:DropDownList CssClass="singleAutoFull" runat="server" ID="ddStatus" AutoPostBack="true" OnSelectedIndexChanged="ddStatus_SelectedIndexChanged">
                                                    </asp:DropDownList>                                                    
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                    <div id="divForFilters" runat="server">
                                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td class="titleTable">
                                                <font class="subTitle">For Product:</font>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="contentTableNPLR">
                                                <div>
                                                    <asp:DropDownList CssClass="singleAutoFull" runat="server" ID="ddForProduct" AutoPostBack="true" OnSelectedIndexChanged="ddForProduct_SelectedIndexChanged">
                                                    </asp:DropDownList>                                                    
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td class="titleTable">
                                                <font class="subTitle">For Manufacturer:</font>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="contentTableNPLR">
                                                <div>
                                                    <asp:DropDownList CssClass="singleAutoFull" runat="server" ID="ddForManufacturer" AutoPostBack="true" OnSelectedIndexChanged="ddForManufacturer_SelectedIndexChanged">
                                                    </asp:DropDownList>                                                    
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td class="titleTable">
                                                <font class="subTitle">For Category:</font>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="contentTableNPLR">
                                                <div>
                                                    <asp:DropDownList CssClass="singleAutoFull" runat="server" ID="ddForCategory" AutoPostBack="true" OnSelectedIndexChanged="ddForCategory_SelectedIndexChanged">
                                                    </asp:DropDownList>                                                    
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td class="titleTable">
                                                <font class="subTitle">For Section:</font>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="contentTableNPLR">
                                                <div>
                                                    <asp:DropDownList CssClass="singleAutoFull" runat="server" ID="ddForSection" AutoPostBack="true" OnSelectedIndexChanged="ddForSection_SelectedIndexChanged">
                                                    </asp:DropDownList>                                                    
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                    </div>
                                    <asp:Button runat="server" ID="btnReset" CssClass="normalButton" Text="Reset" OnClick="btnReset_Click" />
                                </td>
                                <td style="width: 5px;" />
                                <td style="width: 1px; background-color: #a2a2a2;" />
                                <td style="width: 5px;" />
                                <td class="contentTable" valign="top" width="*">
                                    <div class="wrapperLeft">
                                        <asp:Button runat="server" ID="btnInsert" CssClass="normalButton" Text="ADD NEW" OnClick="btnInsert_Click" />
                                        <asp:GridView Width="100%" ID="gMain" runat="server" PagerStyle-HorizontalAlign="left" PagerSettings-Position="TopAndBottom" AutoGenerateColumns="False" AllowPaging="true" PageSize="25" AllowSorting="True" CssClass="overallGrid" HorizontalAlign="Left" OnRowCommand="gMain_RowCommand" OnRowDataBound="gMain_RowDataBound" OnSorting="gMain_Sorting" OnPageIndexChanging="gMain_PageIndexChanging">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <span style='white-space: nowrap;'>
                                                            <a href='editgiftcard.aspx?iden=<%# Eval("GiftCardID") %>'>
                                                                <img src="icons/edit.gif" border="0" alt="Edit" />
                                                            </a>
                                                        </span>
						                            </ItemTemplate>
						                        </asp:TemplateField>
                                                <asp:BoundField DataField="GiftCardID" HeaderText="ID" ReadOnly="True" SortExpression="GiftCardID" ItemStyle-CssClass="lighterData" />
						                        <asp:TemplateField HeaderText="<span style='white-space: nowrap;'>Serial Number</span>" SortExpression="SerialNumber" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
                                                        <a href='editgiftcard.aspx?iden=<%# Eval("GiftCardID") %>'>
                                                            <%# DataBinder.Eval(Container.DataItem, "SerialNumber")%>
                                                        </a>
                                                    </ItemTemplate>
                                                </asp:TemplateField>						                        
                                                <asp:TemplateField HeaderText="Type" ItemStyle-CssClass="lightData">
                                                    <ItemTemplate>
							                            <span style='white-space: nowrap;'>
							                                <asp:Literal ID="ltCardType" runat="server"></asp:Literal>
							                            </span>
						                            </ItemTemplate>
						                        </asp:TemplateField>
						                        <asp:TemplateField HeaderText="CreatedOn" SortExpression="CreatedOn" ItemStyle-CssClass="lighterData">
                                                    <ItemTemplate>
							                            <%# DataBinder.Eval(Container.DataItem, "CreatedOn")%>
						                            </ItemTemplate>
						                        </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Customer" SortExpression="LastName" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
                                                        <span style='white-space: nowrap;'>
                                                            <%# (DataBinder.Eval(Container.DataItem, "FirstName").ToString() + " " + DataBinder.Eval(Container.DataItem, "LastName").ToString()).Trim() %> 
                                                        </span>
						                            </ItemTemplate>
						                        </asp:TemplateField>
                                                <asp:TemplateField HeaderText="<span style='white-space: nowrap;'>Order#</span>" SortExpression="OrderNumber" ItemStyle-CssClass="normalData" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                                                    <ItemTemplate>
                                                        <%# DataBinder.Eval(Container.DataItem, "OrderNumber")%>
						                            </ItemTemplate>
						                        </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Initial Value" SortExpression="InitialAmount" ItemStyle-CssClass="lightData" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltInitialAmount" runat="server"></asp:Literal>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Remaining Amount" SortExpression="Balance" ItemStyle-CssClass="lightData" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltBalance" runat="server"></asp:Literal>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Expires On" SortExpression="ExpirationDate" ItemStyle-CssClass="lightData" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
							                            <%# AspDotNetStorefrontCommon.Localization.ToNativeShortDateString(Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "ExpirationDate")))%> 
							                            <asp:Literal ID="ltCardStatus" runat="server"></asp:Literal>
						                            </ItemTemplate>
						                        </asp:TemplateField>
						                        <asp:TemplateField HeaderText="Usage History" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <span style='white-space: nowrap;'>
                                                            <a href='giftcardusage.aspx?iden=<%# Eval("GiftCardID") %>'>USAGE</a>
                                                        </span>
						                            </ItemTemplate>
						                        </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="selectData" HeaderText="Action" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkAction" CommandName="ItemAction" runat="Server"></asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <PagerSettings FirstPageText="&amp;lt;&amp;lt;First Page" LastPageText="Last Page&amp;gt;&amp;gt;"
                                                Mode="NumericFirstLast" PageButtonCount="15" />
                                            <FooterStyle CssClass="footerGrid" />
                                            <RowStyle CssClass="DataCellGrid" />
                                            <EditRowStyle CssClass="DataCellGridEdit" />
                                            <PagerStyle CssClass="pagerGrid" />
                                            <HeaderStyle CssClass="headerGrid" />
                                            <AlternatingRowStyle CssClass="DataCellGridAlt" />
                                        </asp:GridView>
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
