<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.gallery" CodeFile="gallery.aspx.cs" Theme="Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Galleries</title>
    <script src="jscripts/toolTip.js" type="text/javascript">
        /***********************************************
        * Cool DHTML tooltip script II- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
        * This notice MUST stay intact for legal use
        * Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
        ***********************************************/
    </script>
</head>

<body>
    <form id="frmGallery" runat="server">   
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
                                        Manage Galleries
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
                                    <font class="subTitle">Galleries:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="100%">
                                    <div class="wrapper">
                                        <asp:Panel runat="server" id="pnlGrid">
                                        <asp:Button runat="server" ID="btnInsert" CssClass="normalButton" Text="ADD NEW" OnClick="btnInsert_Click" />
                                        &nbsp;
                                        <asp:Button runat="server" ID="btnOrder" CssClass="normalButton" Text="Update Order" OnClick="btnOrder_Click" />
                                        <asp:GridView Width="100%" ID="gMain" runat="server" PagerStyle-HorizontalAlign="left" PagerSettings-Position="TopAndBottom" AutoGenerateColumns="False" AllowPaging="true" PageSize="15" AllowSorting="True" CssClass="overallGrid" HorizontalAlign="Left" OnRowCancelingEdit="gMain_RowCancelingEdit" OnRowCommand="gMain_RowCommand" OnRowDataBound="gMain_RowDataBound" OnSorting="gMain_Sorting" OnPageIndexChanging="gMain_PageIndexChanging" OnRowUpdating="gMain_RowUpdating" OnRowEditing="gMain_RowEditing">
                                            <Columns>
                                                <asp:CommandField ItemStyle-Width="60" ButtonType="Image" CancelImageUrl="icons/cancel.gif" CancelText="Cancel" EditImageUrl="icons/edit.gif" EditText="Edit" ShowEditButton="True" UpdateImageUrl="icons/update.gif" UpdateText="Update" />
                                                <asp:BoundField DataField="GalleryID" HeaderText="ID" ReadOnly="True" SortExpression="GalleryID" ItemStyle-CssClass="lighterData" />
                                                
                                                <asp:TemplateField HeaderText="Image" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
                                                        <asp:Literal runat="server" ID="ltImage"></asp:Literal>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:FileUpload CssClass="fileUpload" ID="fuMain" runat="server" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField> 
                                                
                                                <asp:TemplateField HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
                                                        <asp:Literal runat="server" ID="ltName" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'></asp:Literal>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <%# DataBinder.Eval(Container.DataItem, "EditName") %>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>   
                                                 
                                                <asp:TemplateField HeaderText="Description" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
                                                        <asp:Literal runat="server" ID="ltDescription" Text='<%# DataBinder.Eval(Container.DataItem, "Description") %>'></asp:Literal>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <%# DataBinder.Eval(Container.DataItem, "EditDescription") %>
                                                    </EditItemTemplate>
                                                </asp:TemplateField> 
                                                
                                                <asp:TemplateField HeaderText="Manage Images" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
                                                        <a href='galleryImages.aspx?GalleryID=<%# Eval("GalleryID") %>'>Manage Images</a>
                                                    </ItemTemplate>
                                                </asp:TemplateField>  
                                                
                                                <asp:TemplateField HeaderText="URL" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltURL" runat="server"></asp:Literal>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Literal ID="ltURL" runat="server"></asp:Literal>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>  
                                                
                                                <asp:TemplateField HeaderText="Display Order" ItemStyle-CssClass="lightData">
                                                    <ItemTemplate>
                                                        <input size="2" type="text" name='DisplayOrder_<%# Eval("GalleryID") %>' value='<%# Eval("DisplayOrder") %>' >                                                
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <input size="2" type="text" name='DisplayOrder_<%# Eval("GalleryID") %>' value='<%# Eval("DisplayOrder") %>' >                                                
                                                    </EditItemTemplate>
                                                </asp:TemplateField>  
                                                                                                 
                                                <asp:TemplateField HeaderText="Directory" ItemStyle-CssClass="lightData">
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltDirectory" runat="server" Text='<%# Eval("DirName") %>'></asp:Literal>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox CssClass="singleNormal" ID="txtDirectory" runat="server"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfvDirectory" runat="server" ErrorMessage="!!" ControlToValidate="txtDirectory"></asp:RequiredFieldValidator>
                                                        <asp:Literal ID="ltDirectory" runat="server"></asp:Literal>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>  
                                                                                        
                                                <asp:TemplateField ItemStyle-CssClass="selectData" ItemStyle-Width="25">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imgDelete" CommandName="DeleteItem" CommandArgument='<%# Eval("GalleryID") %>' runat="Server" AlternateText="Delete" ImageUrl="icons/delete.gif" />                                                        
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField Visible="false" DataField="EditName" ReadOnly="true" />
                                                <asp:BoundField Visible="false" DataField="EditDescription" ReadOnly="true" />
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
                                                    <font class="subTitleSmall">*Name:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:Literal ID="ltName" runat="Server"></asp:Literal>
                                                </td>
                                            </tr>  
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Description:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:Literal ID="ltDescription" runat="Server"></asp:Literal>
                                                </td>
                                            </tr>  
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Image:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:FileUpload CssClass="fileUpload" ID="fuMain" runat="server" />
                                                </td>
                                            </tr>  
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">*Directory:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtDirectory" runat="server" CssClass="singleNormal" ValidationGroup="gAdd"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ErrorMessage="Fill in Directory" ControlToValidate="txtDirectory" ID="RequiredFieldValidator2" ValidationGroup="gAdd" SetFocusOnError="true" runat="server">!!</asp:RequiredFieldValidator> 
                                                    <img src="images/info.gif" border="0" onmouseout="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>ex: gallery1</font>', 300)" alt="" />
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
                                                <td colspan="2">
                                                    <asp:ValidationSummary ValidationGroup="gAdd" ID="validationSummary" runat="server" EnableClientScript="true" ShowMessageBox="true" ShowSummary="false" Enabled="true" />
                                                </td>
                                            </tr>
                                        </table>
                                        <div style="width: 100%; text-align: center;">
                                            &nbsp;&nbsp;<asp:Button ValidationGroup="gAdd" ID="btnSubmit" runat="server" CssClass="normalButton" OnClick="btnSubmit_Click" Text="Submit" />
                                            &nbsp;&nbsp;<asp:Button ID="btnCancel" runat="server" CssClass="normalButton" Text="Cancel" OnClick="btnCancel_Click" />
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
    <asp:Literal ID="ltScript2" runat="server"></asp:Literal>
    </form>
</body>
</html>