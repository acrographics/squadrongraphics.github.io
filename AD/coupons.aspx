<%@ Page Language="C#" AutoEventWireup="true" CodeFile="coupons.aspx.cs" Inherits="coupons" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Coupons</title>
    <asp:Literal runat="server" ID="ltStyles"></asp:Literal>
    <script type="text/javascript" src="jscripts/toolTip.js">
        /***********************************************
        * Cool DHTML tooltip script II- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
        * This notice MUST stay intact for legal use
        * Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
        ***********************************************/
    </script>
</head>
<body>
    <form id="frmCoupon" runat="server">
    <asp:Literal ID="ltScript1" runat="server"></asp:Literal>
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
                                        Manage Coupons
                                    </font>
                                </td>
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
                                <td class="titleTable" width="130">
                                    <font class="subTitle">Coupon Search:</font>
                                </td>
                                <td style="width: 5px;" />
                                <td style="width: 1px; background-color: #a2a2a2;" />
                                <td style="width: 5px;" />
                                <td class="titleTable">
                                    <font class="subTitle">Coupons:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTableNP" valign="top" width="130">
                                    <asp:TextBox ID="txtSearch" Width="130" runat="server"></asp:TextBox>
                                    <asp:Button runat="server" ID="btnSearch" CssClass="normalButton" Text="Search" OnClick="btnSearch_Click" />
                                    <br /><br />
                                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td class="titleTable">
                                                <font class="subTitle">Index:</font>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="contentTableAPL">
                                                <div>
                                                    <asp:TreeView ID="treeMain" runat="server" OnSelectedNodeChanged="treeMain_SelectedNodeChanged">
                                                    </asp:TreeView>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 5px;" />
                                <td style="width: 1px; background-color: #a2a2a2;" />
                                <td style="width: 5px;" />
                                <td class="contentTable" valign="top" width="*">
                                    <div class="wrapperLeft">
                                        <div class="wrapperTop">
    					                    <asp:Button runat="server" ID="btnAdd" CssClass="normalButton" Text="ADD NEW" OnClick="btnAdd_Click" />
                                        </div>
                                        <asp:GridView Width="100%" ID="gMain" runat="server" PagerStyle-HorizontalAlign="left" PagerSettings-Position="TopAndBottom" AutoGenerateColumns="False" AllowPaging="true" PageSize="15" AllowSorting="True" CssClass="overallGrid" HorizontalAlign="Left" OnRowCommand="gMain_RowCommand" OnRowDataBound="gMain_RowDataBound" OnSorting="gMain_Sorting" OnPageIndexChanging="gMain_PageIndexChanging">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Edit">
                                                    <ItemTemplate>
                                                        <a href='editCoupons.aspx?iden=<%# DataBinder.Eval(Container.DataItem, "CouponID")%>'><img src="icons/edit.gif" border="0" /></a>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="CouponID" HeaderText="ID" ReadOnly="True" SortExpression="CouponID" ItemStyle-CssClass="lighterData" />
                                                <asp:TemplateField HeaderText="Coupon Code" SortExpression="CouponCode" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
                                                        <%# DataBinder.Eval(Container.DataItem, "CouponCode")%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Description" ItemStyle-CssClass="lightestData">
                                                    <ItemTemplate>
                                                        <div style="white-space: normal; overflow: visible; width: 100%;">
							                            <%# DataBinder.Eval(Container.DataItem, "Description") %>
							                            </div>
						                            </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Date Created" SortExpression="CreatedOn" ItemStyle-CssClass="lighterData">
                                                    <ItemTemplate>
                                                        <%# DataBinder.Eval(Container.DataItem, "CreatedOn")%>
							                        </ItemTemplate>
						                        </asp:TemplateField>
						                        <asp:TemplateField HeaderText="Expiration Date" SortExpression="ExpirationDate" ItemStyle-CssClass="lighterData">
                                                    <ItemTemplate>
                                                        <%# DataBinder.Eval(Container.DataItem, "ExpirationDate")%>
							                        </ItemTemplate>
						                        </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Amount" SortExpression="DiscountAmount" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltAmount" runat="server"></asp:Literal>
							                        </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Percent" SortExpression="DiscountPercent" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltPercent" runat="server"></asp:Literal>
							                        </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Uses" SortExpression="NumUses" ItemStyle-CssClass="lighterData">
                                                    <ItemTemplate>
                                                        <%# DataBinder.Eval(Container.DataItem, "NumUses")%>
							                        </ItemTemplate>
						                        </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="selectData">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imgDelete" CommandName="DeleteItem" CommandArgument='<%# Eval("CouponID") %>' runat="Server" AlternateText="Delete" ImageUrl="icons/delete.gif" />                                                        
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
    <asp:Literal ID="ltScript" runat="server"></asp:Literal>
</body>
</html>
