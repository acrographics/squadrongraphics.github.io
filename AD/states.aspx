<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.states" CodeFile="states.aspx.cs" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>States</title>
    <script type="text/javascript"  src="jscripts/toolTip.js">
        /***********************************************
        * Cool DHTML tooltip script II- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
        * This notice MUST stay intact for legal use
        * Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
        ***********************************************/
    </script>
</head>

<body>
    <form id="frmSalesPrompts" runat="server">   
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
                                        Taxes by State
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
                                    <font class="subTitle">State Taxes:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="100%">
                                    <div class="wrapper">
                                        <asp:Panel runat="server" id="pnlGrid">
                                        <asp:Button runat="server" ID="btnInsert" CssClass="normalButton" Text="ADD NEW" OnClick="btnInsert_Click" />
                                        <asp:Button runat="server" ID="btnUpdateOrder" CssClass="normalButton" Text="Update Display Order and Taxes" OnClick="btnUpdateOrder_Click" />
                                        <asp:GridView Width="100%" ID="gMain" runat="server" PagerStyle-HorizontalAlign="left" PagerSettings-Position="TopAndBottom" AutoGenerateColumns="False" AllowPaging="true" PageSize="999999" AllowSorting="True" CssClass="overallGrid" HorizontalAlign="Left" OnRowCancelingEdit="gMain_RowCancelingEdit" OnRowCommand="gMain_RowCommand" OnRowDataBound="gMain_RowDataBound" OnSorting="gMain_Sorting" OnPageIndexChanging="gMain_PageIndexChanging" OnRowUpdating="gMain_RowUpdating" OnRowEditing="gMain_RowEditing">
                                            <Columns>
                                                <asp:CommandField ItemStyle-Width="25" ButtonType="Image" CancelImageUrl="icons/cancel.gif" CancelText="Cancel" EditImageUrl="icons/edit.gif" EditText="Edit" ShowEditButton="True" UpdateImageUrl="icons/update.gif" UpdateText="Update" />
                                                <asp:BoundField DataField="StateID"  ItemStyle-Width="25" HeaderText="ID" ReadOnly="True" SortExpression="StateID" ItemStyle-CssClass="lighterData" />
                                                
                                                <asp:TemplateField HeaderText="State/Province" ItemStyle-Width="125" SortExpression="Name" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
                                                        <%# DataBinder.Eval(Container.DataItem, "Name") %>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <a name='a<%# Eval("StateID") %>'></a>
                                                        <asp:TextBox ID="txtName" runat="Server" CssClass="singleAuto" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator" runat="server" ErrorMessage="!!" ControlToValidate="txtName"></asp:RequiredFieldValidator>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                
                                                <asp:TemplateField HeaderText="Display Order" ItemStyle-Width="75" SortExpression="DisplayOrder" ItemStyle-CssClass="lighterData">
                                                    <ItemTemplate>
                                                        <input type="text" name='DisplayOrder_<%# Eval("StateID") %>' class="single3chars" value='<%# DataBinder.Eval(Container.DataItem, "DisplayOrder") %>'></input>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtOrder" runat="Server" CssClass="single3chars" Text='<%# DataBinder.Eval(Container.DataItem, "DisplayOrder") %>'></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="!!" ControlToValidate="txtOrder"></asp:RequiredFieldValidator>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>  
                                                
                                                <asp:TemplateField HeaderText="Abbrev." ItemStyle-Width="50" SortExpression="Abbreviation" ItemStyle-CssClass="lighterData">
                                                    <ItemTemplate>
                                                        <%# DataBinder.Eval(Container.DataItem, "Abbreviation")%>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtAbbreviation" runat="Server" CssClass="single4chars" Text='<%# DataBinder.Eval(Container.DataItem, "Abbreviation") %>'></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="!!" ControlToValidate="txtAbbreviation"></asp:RequiredFieldValidator>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>  
                                                   
                                                <asp:TemplateField HeaderText="Tax Rate" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltTaxRate" runat="server"></asp:Literal>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Literal ID="ltTaxRate" runat="server"></asp:Literal>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>  
                                                
                                                <asp:TemplateField HeaderText="Country" SortExpression="Country" ItemStyle-CssClass="lighterData">
                                                    <ItemTemplate>
                                                        <%# DataBinder.Eval(Container.DataItem, "Country")%>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:DropDownList ID="ddCountry" runat="server" CssClass="default"></asp:DropDownList>
                                                    </EditItemTemplate>
                                                </asp:TemplateField> 
                                                
                                                <asp:TemplateField HeaderText="Del" ItemStyle-CssClass="selectData" ItemStyle-Width="25">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imgDelete" CommandName="DeleteItem" CommandArgument='<%# Eval("StateID") %>' runat="Server" AlternateText="Delete" ImageUrl="icons/delete.gif" />                                                        
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField Visible="false" DataField="EditName" ReadOnly="true" />
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
                                        </asp:Panel>
                                        <asp:Panel ID="pnlAdd" runat="Server">
                                        <div style="margin-top: 5px; margin-bottom: 15px;">
                                            Fields marked with an asterisk (*) are required. All other fields are optional.
                                        </div>
                                        <table width="100%" cellpadding="1" cellspacing="0" border="0">
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">*State/Province:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtName" runat="server" CssClass="singleNormal" ValidationGroup="gAdd"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ErrorMessage="Fill in Name" ControlToValidate="txtName" ID="RequiredFieldValidator2" ValidationGroup="gAdd" SetFocusOnError="true" runat="server">!!</asp:RequiredFieldValidator> 
                                                    <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>ex: Arizona</font>', 300)" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">*Abbreviation:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtAbbr" runat="server" CssClass="single3chars" ValidationGroup="gAdd"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ErrorMessage="Fill in Abbreviation" ControlToValidate="txtAbbr" ID="RequiredFieldValidator3" ValidationGroup="gAdd" SetFocusOnError="true" runat="server">!!</asp:RequiredFieldValidator> 
                                                    <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>ex: AZ</font>', 300)" />
                                                </td>
                                            </tr>  
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Country:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:DropDownList ID="ddCountry" runat="Server"></asp:DropDownList>
                                                    <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>ex: United States</font>', 300)" />
                                                </td>
                                            </tr>                                           
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">*Display Order:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                     <asp:TextBox ID="txtOrder" runat="Server" CssClass="single3chars" ValidationGroup="gAdd">1</asp:TextBox>
                                                     <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Enter Display Order" ValidationGroup="gAdd" ControlToValidate="txtOrder">!!</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>                                           
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">*Tax Shipping:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:RadioButtonList ID="rblShipping" runat="server">
                                                        <asp:ListItem Selected="True">False</asp:ListItem>
                                                        <asp:ListItem>True</asp:ListItem>
                                                    </asp:RadioButtonList>                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:ValidationSummary ValidationGroup="gAdd" ID="validationSummary" runat="server" EnableClientScript="true" ShowMessageBox="true" ShowSummary="false" Enabled="true" />
                                                </td>
                                            </tr>
                                        </table>
                                        <div style="width: 100%; text-align: center;">
                                            &nbsp;&nbsp;<asp:Button ValidationGroup="gAdd" CssClass="normalButton" ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Submit" />
                                            &nbsp;&nbsp;<asp:Button ID="btnCancel" CssClass="normalButton" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
                                        </div>
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
    <asp:Literal ID="ltMiscellaneous" runat="server"></asp:Literal>
    </form>
</body>
</html>
