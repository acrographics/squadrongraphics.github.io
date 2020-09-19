<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.uploadImages" CodeFile="uploadImages.aspx.cs" Theme="Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Upload Images</title>
    <asp:Literal ID="ltStyles" runat="server"></asp:Literal>
</head>

<body>
    <form id="frmUploadImages" runat="server" enctype="multipart/form-data" method="post">   
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
                                        Manage Image Uploads
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
        <table border="0" cellpadding="1" cellspacing="0" class="outerTable">
            <tr>
                <td>
                    <div class="wrapper">                       
                        <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                            <tr>
                                <td class="titleTable">
                                    <font class="subTitle">Upload Image:</font>
                                </td>
                                <td class="contentTable" valign="top">
                                    <asp:FileUpload ID="fuMain" runat="server" CssClass="fileUpload"  />
                                    <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="normalButton" OnClick="btnUpload_Click" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div style="margin-bottom: 5px; margin-top: 5px;">
       
    </div>
    <div id="content">
        <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
            <tr>
                <td>
                    <div class="wrapper">                       
                        <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                            <tr>
                                <td class="titleTable">
                                    <font class="subTitle">Images:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="100%">
                                    <div class="wrapper">
                                        <asp:GridView Width="100%" ID="gMain" runat="server" PagerStyle-HorizontalAlign="left" PagerSettings-Position="TopAndBottom" AutoGenerateColumns="False" AllowPaging="true" PageSize="15" AllowSorting="True" CssClass="overallGrid" HorizontalAlign="Left" OnRowCommand="gMain_RowCommand" OnRowDataBound="gMain_RowDataBound" OnPageIndexChanging="gMain_PageIndexChanging">
                                            <Columns>
                                                <asp:BoundField DataField="Path" Visible="false" ReadOnly="True" />
                                                <asp:BoundField DataField="FileName" HeaderText="File Name" ReadOnly="True" ItemStyle-CssClass="normalData" />
                                                <asp:BoundField DataField="SRC" HeaderText="IMG Tag src" ReadOnly="True" ItemStyle-CssClass="normalData" />
                                                <asp:BoundField DataField="Dimensions" HeaderText="Dimensions" ReadOnly="True" ItemStyle-CssClass="lighterData" />
                                                <asp:BoundField DataField="Size" HeaderText="Size (KB)" ReadOnly="True" ItemStyle-CssClass="lighterData" />
                                                <asp:TemplateField HeaderText="Image" ItemStyle-CssClass="normalData" >
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltImage" runat="server" Text='<%# Eval("Image") %>'></asp:Literal>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="selectData" ItemStyle-Width="25">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imgDelete" CommandName="DeleteItem" CommandArgument='<%# Eval("Path") %>' runat="Server" AlternateText="Delete" ImageUrl="icons/delete.gif" />                                                        
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
    <asp:Literal id="ltScript1" runat="server"></asp:Literal>
    </form>
</body>
</html>
