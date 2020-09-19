<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.partners" CodeFile="partners.aspx.cs" Theme="Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Partners</title>
</head>

<body>
    <form id="frmPartners" runat="server">   
    <asp:Literal ID="ltScript" runat="server"></asp:Literal> 
    <asp:Literal ID="ltValid" runat="server"></asp:Literal>
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
                                        Manage Partners
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
                                <td class="titleTable">
                                    <font class="subTitle">Partners:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="100%">
                                    <div class="wrapper">
                                        <asp:Button runat="server" ID="btnInsert" CssClass="normalButton" Text="ADD NEW" OnClick="btnInsert_Click" />
                                        &nbsp;
                                        <asp:Button runat="server" ID="btnOrder" CssClass="normalButton" Text="Update Order" OnClick="btnOrder_Click" />
                                        <asp:GridView Width="100%" ID="gMain" runat="server" PagerStyle-HorizontalAlign="left" PagerSettings-Position="TopAndBottom" AutoGenerateColumns="False" AllowPaging="true" PageSize="15" AllowSorting="True" CssClass="overallGrid" HorizontalAlign="Left" OnRowCancelingEdit="gMain_RowCancelingEdit" OnRowCommand="gMain_RowCommand" OnRowDataBound="gMain_RowDataBound" OnSorting="gMain_Sorting" OnPageIndexChanging="gMain_PageIndexChanging" OnRowUpdating="gMain_RowUpdating" OnRowEditing="gMain_RowEditing">
                                            <Columns>
                                                <asp:CommandField ItemStyle-Width="60" ButtonType="Image" CancelImageUrl="icons/cancel.gif" CancelText="Cancel" EditImageUrl="icons/edit.gif" EditText="Edit" ShowEditButton="True" UpdateImageUrl="icons/update.gif" UpdateText="Update" />
                                                <asp:BoundField DataField="PartnerID" HeaderText="ID" ReadOnly="True" SortExpression="PartnerID" ItemStyle-CssClass="lighterData" />
                                                
                                                <asp:TemplateField HeaderText="Image" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
                                                        <asp:Literal runat="server" ID="ltImage"></asp:Literal>
                                                    </ItemTemplate>
                                                </asp:TemplateField> 
                                                
                                                <asp:TemplateField HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
                                                        <asp:Literal runat="server" ID="ltName" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'></asp:Literal>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtName" runat="Server" Text='<%# DataBinder.Eval(Container.DataItem, "Name")%>' CssClass="singleNormal"></asp:TextBox>
                                                    </EditItemTemplate>                                                    
                                                </asp:TemplateField>   
                                                                                                 
                                                <asp:TemplateField HeaderText="Display Order" ItemStyle-CssClass="lightData">
                                                    <ItemTemplate>
                                                        <input class="single3chars" type="text" name='DisplayOrder_<%# Eval("PartnerID") %>' value='<%# Eval("DisplayOrder") %>' >                                                
                                                    </ItemTemplate>
                                                </asp:TemplateField>  
                                                                                                 
                                                <asp:TemplateField HeaderText="Edit Partner" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
                                                        <a href='editPartner.aspx?Partnerid=<%# Eval("PartnerID") %>'>Edit Partner</a>
                                                    </ItemTemplate>
                                                </asp:TemplateField>  
                                                                                        
                                                <asp:TemplateField ItemStyle-CssClass="selectData" ItemStyle-Width="25">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imgDelete" CommandName="DeleteItem" CommandArgument='<%# Eval("PartnerID") %>' runat="Server" AlternateText="Delete" ImageUrl="icons/delete.gif" />                                                        
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
    <asp:Literal ID="ltScript2" runat="server"></asp:Literal>
    </form>
</body>
</html>