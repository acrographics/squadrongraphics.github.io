<%@ Page Language="C#" AutoEventWireup="true" CodeFile="entityProductVariantsOverview.aspx.cs" Inherits="entityProductVariantsOverview" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Products</title>
    <asp:Literal runat="server" ID="ltStyles"></asp:Literal>
    <script type="text/javascript"  src="jscripts/toolTip.js">
        /***********************************************
        * Cool DHTML tooltip script II- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
        * This notice MUST stay intact for legal use
        * Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
        ***********************************************/
    </script>
</head>
<body> <%--style="padding: 0px 0px 0px 0px; margin: 0px 0px 0px 0px;"--%>
    <div style="padding: 0px 0px 0px 0px; width: 100%; margin: 0px 0px 0px 0px;">
        <form id="frmEntityProducts" runat="server">
        <asp:Literal ID="ltScript1" runat="server"></asp:Literal>
    <%--<table width="100%" cellpadding="0" cellspacing="0" border="0"><tr><td>--%>
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
                                            <asp:Literal ID="ltEntity" runat="server"></asp:Literal> : <asp:Literal ID="ltProduct" runat="server"></asp:Literal> : Managing Variants
                                        </font>
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
                                    <%--<td class="titleTable" width="130">
                                        <font class="subTitle">Variant Search:</font>
                                    </td>
                                    <td style="width: 5px;" />
                                    <td style="width: 1px; background-color: #a2a2a2;" />
                                    <td style="width: 5px;" />
                                    --%>
                                    <td class="titleTable">
                                        <font class="subTitle">Variants:</font>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="contentTable" valign="top" width="*">
                                        <div class="wrapperLeft">
                                            <div style="float: left;">
                                                <div class="wrapperTop">
    					                            <asp:Button runat="server" ID="btnAdd" CssClass="normalButton" Text="ADD NEW" OnClick="btnAdd_Click" />
                                                </div>
                                            </div>
                                            <div style="float: left; padding-left: 10px;">
                                                <div class="wrapperTop">
    					                            <asp:Button ID="btnUpdate" runat="server" CssClass="normalButton" Text="Update Order and Default Variant" OnClick="btnUpdate_Click" />
                                                </div>
                                            </div>
                                            <div style="float: left; padding-left: 10px;">
                                                <div class="wrapperTop">
					                                <asp:Button runat="server" ID="btnDeleteVariants" CssClass="normalButton" Text="DELETE ALL VARIANTS" OnClick="btnDeleteVariants_Click" />
                                                </div>
                                            </div>
                                            <asp:GridView Width="100%" ID="gMain" runat="server" PagerStyle-HorizontalAlign="left" PagerSettings-Position="TopAndBottom" AutoGenerateColumns="False" AllowPaging="true" PageSize="15" AllowSorting="True" CssClass="overallGrid" HorizontalAlign="Left" OnRowCommand="gMain_RowCommand" OnRowDataBound="gMain_RowDataBound" OnSorting="gMain_Sorting" OnPageIndexChanging="gMain_PageIndexChanging">
                                                <Columns>
                                                    <asp:BoundField DataField="VariantID" HeaderText="ID" ReadOnly="True" SortExpression="VariantID" ItemStyle-CssClass="lighterData" />
                                                    <asp:TemplateField HeaderText="Image" ItemStyle-CssClass="normalData">
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltImage" runat="server"></asp:Literal>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Product" SortExpression="Name" ItemStyle-CssClass="normalData">
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltName" runat="server"></asp:Literal>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SKU" SortExpression="SKUSuffix" ItemStyle-CssClass="lighterData">
                                                        <ItemTemplate>
							                                <asp:Literal ID="ltSKU" runat="server"></asp:Literal>
						                                </ItemTemplate>
                                                    </asp:TemplateField>
						                            <asp:TemplateField HeaderText="Price" SortExpression="Price" ItemStyle-CssClass="normalData">
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltPrice" runat="server"></asp:Literal>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
						                            <asp:TemplateField HeaderText="Inventory" SortExpression="Inventory" ItemStyle-CssClass="lightData">
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltInventory" runat="server"></asp:Literal>
							                            </ItemTemplate>
						                            </asp:TemplateField>
						                            <asp:TemplateField HeaderText="Display Order" SortExpression="DisplayOrder" ItemStyle-CssClass="lightData">
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltDisplayOrder" runat="server"></asp:Literal>
							                            </ItemTemplate>
						                            </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Is Default Variant" ItemStyle-CssClass="lightData">
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltDefault" runat="server"></asp:Literal>
							                            </ItemTemplate>
                                                    </asp:TemplateField>    
                                                    <asp:TemplateField ItemStyle-CssClass="selectData" HeaderText="Clone">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkClone" CommandName="CloneItem" CommandArgument='<%# Eval("VariantID") %>' runat="Server" Text="Clone" />                                                        
                                                        </ItemTemplate>
                                                    </asp:TemplateField>      
                                                    <asp:TemplateField ItemStyle-CssClass="selectData" HeaderText="Move">
                                                        <ItemTemplate>
                                                            <input type="button" value="Move" name='Move_<%# Eval("VariantID") %>' onclick="window.open('entityMoveVariant.aspx?productid=<%# Eval("ProductID") %>&Variantid=<%# Eval("VariantID") %>','Move','height=200, width=300, scrollbars=yes, resizable=yes, toolbar=no, status=yes, location=no, directories=no, menubar=no, alwaysRaised=yes');" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>                                               
                                                    <asp:TemplateField ItemStyle-CssClass="selectData" HeaderText="Soft Delete">
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="imgDelete" CommandName="DeleteItem" CommandArgument='<%# Eval("VariantID") %>' runat="Server" AlternateText="Delete" ImageUrl="icons/delete.gif" />                                                        
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
    <%--</td></tr></table>--%>        
        </form>
        <asp:Literal ID="ltScript" runat="server"></asp:Literal>
    </div>
</body>
</html>
