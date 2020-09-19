<%@ Page Language="C#" AutoEventWireup="true" CodeFile="stringresource.aspx.cs" Inherits="AspDotNetStorefrontAdmin.stringresource" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>String Resource</title>
    <script type="text/javascript"  src="jscripts/toolTip.js" type="text/javascript">
        /***********************************************
        * Cool DHTML tooltip script II- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
        * This notice MUST stay intact for legal use
        * Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
        ***********************************************/
    </script>
</head>

<body>
    <form id="frmStringResource" runat="server">
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
                                        Manage String Resource
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
                                    <font class="subTitle">String Search:</font>
                                </td>
                                <td style="width: 5px;" />
                                <td style="width: 1px; background-color: #a2a2a2;" />
                                <td style="width: 5px;" />
                                <td class="titleTable">
                                    <font class="subTitle">String Resources:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTableNP" valign="top" width="130">
                                    <asp:TextBox ID="txtSearch" Width="130" runat="server"></asp:TextBox>
                                    <asp:Button runat="server" ID="btnSearch" class="stringResourceButton" CssClass="normalButton" Text="Search" OnClick="btnSearch_Click" />
                                    <br /><br />
                                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td class="titleTable">
                                                <font class="subTitle">Locale:</font>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="contentTableAPL">
                                                <div>
                                                    <asp:DropDownList runat="server" ID="ddLocales" AutoPostBack="true" OnSelectedIndexChanged="ddLocales_SelectedIndexChanged">
                                                    </asp:DropDownList>                                                    
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
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
                                        <asp:Panel runat="server" id="pnlGrid">
                                        <div id="divActions" runat="server" style="margin-top: 5px; margin-bottom: 5px;">
                                            <asp:Button runat="server" ID="btnInsert" CssClass="stringResourceButton" Text="ADD NEW String" Width="95" OnClick="btnInsert_Click" />
                                            <asp:Literal ID="ltActions" runat="Server"></asp:Literal>
                                            <asp:Button runat="server" ID="btnLoadExcelServer" CssClass="stringResourceButton" Text="Reload from Excel File on Server" Width="180" OnClick="btnLoadExcelServer_Click" />
                                            <asp:Button runat="server" ID="btnUploadExcel" CssClass="stringResourceButton" Text="Reload from Excel File on Your PC" Width="190" OnClick="btnUploadExcel_Click" />
                                            <asp:Button runat="server" ID="btnShowMissing" CssClass="stringResourceButton" Text="Show Missing Strings" Width="120" OnClick="btnShowMissing_Click" />
                                            <asp:Button runat="server" ID="btnShowModified" CssClass="stringResourceButton" Text="Show Modified Strings" Width="120" OnClick="btnShowModified_Click" />
                                            <asp:Button runat="server" ID="btnClearLocale" CssClass="stringResourceButton" Text="Clear Locale" Width="75" OnClick="btnClearLocale_Click" />
                                        </div>
                                        <asp:GridView Width="100%" ID="gMain" runat="server" PagerStyle-HorizontalAlign="left" PagerSettings-Position="TopAndBottom" AutoGenerateColumns="False" AllowPaging="true" PageSize="50" AllowSorting="True" CssClass="overallGrid" HorizontalAlign="Left" OnRowCancelingEdit="gMain_RowCancelingEdit" OnRowCommand="gMain_RowCommand" OnRowDataBound="gMain_RowDataBound" OnSorting="gMain_Sorting" OnPageIndexChanging="gMain_PageIndexChanging" OnRowUpdating="gMain_RowUpdating" OnRowEditing="gMain_RowEditing">
                                            <Columns>
                                                <asp:CommandField ButtonType="Image" CancelImageUrl="icons/cancel.gif" CancelText="Cancel" EditImageUrl="icons/edit.gif" EditText="Edit" ShowEditButton="True" UpdateImageUrl="icons/update.gif" UpdateText="Update" />
                                                <asp:BoundField DataField="StringResourceID" HeaderText="ID" ReadOnly="True" SortExpression="StringResourceID" ItemStyle-CssClass="lighterData" />
                                                <asp:TemplateField HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
							                            <%# AspDotNetStorefrontCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Modified").ToString() == "0", "","<font color=\"blue\"><b>") + DataBinder.Eval(Container.DataItem, "Name") + AspDotNetStorefrontCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Modified").ToString() == "0", "","</b></font>") %>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtName" runat="Server" CssClass="singleAuto" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator" ControlToValidate="txtName" runat="server" ErrorMessage="!!"></asp:RequiredFieldValidator>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Value" ItemStyle-CssClass="normalData" SortExpression="ConfigValue">
                                                    <ItemTemplate>
                                                        <div style="white-space: normal; overflow: visible; width: 225px;">
							                            <%# AspDotNetStorefrontCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Modified").ToString() == "0", "","<font color=\"blue\"><b>") + DataBinder.Eval(Container.DataItem, "ConfigValue") + AspDotNetStorefrontCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Modified").ToString() == "0", "","</b></font>") %>
							                            </div>
						                            </ItemTemplate>
						                            <EditItemTemplate>
							                            <asp:TextBox Runat="server" ID="txtValue" CssClass="multiAuto" TextMode="MultiLine" Text='<%# DataBinder.Eval(Container.DataItem, "ConfigValue") %>'></asp:TextBox>
						                            </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Modified" SortExpression="Modified" ItemStyle-CssClass="lightData">
                                                    <ItemTemplate>
                                                        <%# AspDotNetStorefrontCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Modified").ToString() == "0", "No","<font color=\"blue\"><b>Yes</b></font>") %>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Locale" SortExpression="LocaleSetting" ItemStyle-CssClass="lightData">
                                                    <ItemTemplate>
                                                        <%# DataBinder.Eval(Container.DataItem, "LocaleSetting")%>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:DropDownList ID="ddLocale" runat="server"></asp:DropDownList>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="selectData">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imgDelete" CommandName="DeleteItem" CommandArgument='<%# Eval("StringResourceID") %>' runat="Server" AlternateText="Delete" ImageUrl="icons/delete.gif" />                                                        
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
                                                    <asp:TextBox ID="txtName" runat="server" CssClass="singleNormal" ValidationGroup="gAdd"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ErrorMessage="Fill in Name" ControlToValidate="txtName" ID="RequiredFieldValidator2" ValidationGroup="gAdd" Display="dynamic" SetFocusOnError="true" runat="server">!!</asp:RequiredFieldValidator>
                                                    <img src="images/info.gif" border="0" onmouseout="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>ex: page.aspx.1</font>', 300)" alt="" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">*Value:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtDescription" runat="server" CssClass="multiNormal" TextMode="multiline" ValidationGroup="gAdd"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ErrorMessage="Fill in Description" ControlToValidate="txtDescription" ID="RequiredFieldValidator3" ValidationGroup="gAdd" Display="dynamic" SetFocusOnError="true" runat="server">!!</asp:RequiredFieldValidator>
                                                    <img src="images/info.gif" border="0" onmouseout="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>ex: String to appear for place holder.</font>', 300)" alt="" />
                                                </td>
                                            </tr>     
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Locale:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:DropDownList ID="ddLocale" runat="server" ValidationGroup="gAdd"></asp:DropDownList>
                                                    <asp:RequiredFieldValidator ErrorMessage="Select Locale" InitialValue="0" ControlToValidate="ddLocale" ID="RequiredFieldValidator1" ValidationGroup="gAdd" SetFocusOnError="true" runat="server">!!</asp:RequiredFieldValidator> 
                                                </td>
                                            </tr>                                                                                         
                                            <tr>
                                                <td colspan="2">
                                                    <asp:ValidationSummary ValidationGroup="gAdd" ID="validationSummary" runat="server" EnableClientScript="true" ShowMessageBox="true" ShowSummary="false" Enabled="true" />
                                                </td>
                                            </tr>
                                        </table>
                                        <div style="width: 100%; text-align: center;">
                                            &nbsp;&nbsp;<asp:Button ValidationGroup="gAdd" ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" CssClass="normalButton" Text="Submit" />
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
    </form>
</body>
</html>
