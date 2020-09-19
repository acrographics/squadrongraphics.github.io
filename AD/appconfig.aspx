<%@ Page Language="C#" AutoEventWireup="true" CodeFile="appconfig.aspx.cs" Inherits="AspDotNetStorefrontAdmin.appconfig" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>AppConfig Paramenters</title>
</head>

<script type="text/javascript">
    function getDelete()
    {
        return 'Confirm Delete';
    }
</script>
<body>
    <form id="frmAppConfig" runat="server">
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
                                        Manage AppConfig Parameters
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
                                <td class="titleTable" width="130">
                                    <font class="subTitle">Add New:</font>
                                </td>
                                <td style="width: 5px;" />
                                <td style="width: 1px; background-color: #a2a2a2;" />
                                <td style="width: 5px;" />
                                <td class="titleTable">
                                    <font class="subTitle">Config Variables:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTableNP" valign="top" width="130">
                                    <asp:Panel ID="pnlAdd" runat="server" DefaultButton="btnInsert">
                                        <asp:TextBox ID="txtAddName" Width="140" runat="server" CssClass="default">Config Name</asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="!!" ControlToValidate="txtAddName" Display="dynamic"></asp:RequiredFieldValidator>
                                        <asp:TextBox ID="txtAddValue" Width="140" runat="server" CssClass="default">Config Value</asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator" runat="server" ErrorMessage="!!" ControlToValidate="txtAddValue" Display="dynamic"></asp:RequiredFieldValidator>
                                        <asp:DropDownList ID="ddAddGroup" runat="server" CssClass="default"></asp:DropDownList>
                                        <asp:CheckBox ID="cbADDSA" runat="server" Text="SA Only" CssClass="default" />
                                        <asp:Button runat="server" ID="btnInsert" CssClass="normalButton" Text="ADD" OnClick="btnInsert_Click" />
                                    </asp:Panel>
                                    <br /><br />
                                    <asp:Panel ID="SearchPanel" runat="server" DefaultButton="btnSearch">
                                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td class="titleTable">
                                                <font class="subTitle">Config Search</font>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="contentTableAPL">
                                                <div>
                                                    <asp:TextBox ID="txtSearch" Width="140" runat="server" CssClass="default"></asp:TextBox>
                                                    <asp:Button runat="server" ID="btnSearch" CssClass="normalButton" Text="Search" OnClick="btnSearch_Click" />
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    </asp:Panel>
                                    <br />
                                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td class="titleTable">
                                                <font class="subTitle">Config Groups:</font>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="contentTableAPL">
                                                <div>
                                                    <asp:DropDownList runat="server" ID="ddConfigGroups" AutoPostBack="true" OnSelectedIndexChanged="ddConfigGroups_SelectedIndexChanged">
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
                                        <asp:GridView Width="100%" ID="gMain" runat="server" PagerStyle-HorizontalAlign="left" PagerSettings-Position="TopAndBottom" AutoGenerateColumns="False" AllowPaging="true" PageSize="15" AllowSorting="True" CssClass="overallGrid" HorizontalAlign="Left" OnRowCancelingEdit="gMain_RowCancelingEdit" OnRowCommand="gMain_RowCommand" OnRowDataBound="gMain_RowDataBound" OnSorting="gMain_Sorting" OnPageIndexChanging="gMain_PageIndexChanging" OnRowUpdating="gMain_RowUpdating" OnRowEditing="gMain_RowEditing">
                                            <Columns>
                                                <asp:CommandField ButtonType="Image" CancelImageUrl="icons/cancel.gif" CancelText="Cancel" EditImageUrl="icons/edit.gif" EditText="Edit" ShowEditButton="True" UpdateImageUrl="icons/update.gif" UpdateText="Update" />
                                                <asp:BoundField DataField="AppConfigID" HeaderText="ID" ReadOnly="True" SortExpression="AppConfigID" ItemStyle-CssClass="lighterData" />
                                                <asp:TemplateField HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
                                                        <%# DataBinder.Eval(Container.DataItem, "Name") %>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtName" runat="Server" CssClass="singleAuto" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'></asp:TextBox>
                                                        <asp:Literal id="ltName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'></asp:Literal>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator" runat="server" ErrorMessage="!!" ControlToValidate="txtName"></asp:RequiredFieldValidator>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Description" ItemStyle-CssClass="lightestData">
                                                    <ItemTemplate>
                                                        <div style="white-space: normal; overflow: visible; width: 100%;">
							                            <%# DataBinder.Eval(Container.DataItem, "Description") %>
							                            </div>
						                            </ItemTemplate>
						                            <EditItemTemplate>
							                            <asp:TextBox Runat="server" ID="txtDescription" CssClass="multiShorter" TextMode="MultiLine" Text='<%# DataBinder.Eval(Container.DataItem, "Description") %>'></asp:TextBox>
						                            </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Value" ItemStyle-CssClass="normalData">
                                                    <ItemTemplate>
                                                        <div style="white-space: normal; overflow: visible; width: 225px;">
							                            <%# DataBinder.Eval(Container.DataItem, "ConfigValue") %>
							                            </div>
						                            </ItemTemplate>
						                            <EditItemTemplate>
							                            <asp:TextBox Runat="server" ID="txtValue" CssClass="singleAuto" TextMode="SingleLine" Text='<%# DataBinder.Eval(Container.DataItem, "ConfigValue") %>'></asp:TextBox>
							                        </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Group" SortExpression="GroupName" ItemStyle-CssClass="lightData">
                                                    <ItemTemplate>
                                                        <%# DataBinder.Eval(Container.DataItem, "GroupName")%>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:DropDownList ID="ddEditGroup" runat="server" CssClass="default"></asp:DropDownList>
                                                        OR 
                                                        <asp:TextBox ID="txtGroup" runat="Server" CssClass="default"></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="SA" ItemStyle-CssClass="lightData">
                                                    <ItemTemplate>
							                            <%# DataBinder.Eval(Container.DataItem, "SuperOnly") %>
						                            </ItemTemplate>
						                            <EditItemTemplate>
							                            <asp:CheckBox Runat="server" ID="cbAdmin" />
						                            </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="selectData">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imgDelete" CommandName="DeleteItem" CommandArgument='<%# Eval("AppConfigID") %>' runat="Server" AlternateText="Delete" ImageUrl="icons/delete.gif" />                                                        
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
