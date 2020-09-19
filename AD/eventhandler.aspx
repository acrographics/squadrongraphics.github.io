<%@ Page Language="C#" AutoEventWireup="true" CodeFile="eventhandler.aspx.cs" Inherits="AspDotNetStorefrontAdmin.eventhandler" Theme="Default"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Event Handler Parameters</title>
</head>
<script type="text/javascript">
    function getDelete()
    {
        return 'Confirm Delete';
    }
</script>
<body>
    <form id="frmEventHandlers" runat="server">
    <div>
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
                                        <font class="title">Manage EventHandlers Parameters </font>
                                    </td>
                                    <!--</tr>
                            <tr>-->
                                    <td style="width: 10px">
                                    </td>
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
            <div style="margin-top: 5px; margin-bottom: 5px">
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
                                    <td class="titleTable" width="130" style="height: 18px">
                                        <font class="subTitle">Add New:</font>
                                    </td>
                                    <td style="width: 5px; height: 18px;">
                                    </td>
                                    <td style="width: 1px; background-color: #a2a2a2; height: 18px;">
                                    </td>
                                    <td style="width: 5px; height: 18px;">
                                    </td>
                                    <td class="titleTable" style="height: 18px">
                                        <font class="subTitle">Event Handler Variables:</font>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="contentTableNP" valign="top" width="130">
                                        <asp:Panel ID="pnlAdd" runat="server" DefaultButton="btnInsert">
                                            <asp:TextBox ID="txtAddName" runat="server" CssClass="default" Width="140">EventHandler Name</asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtAddName" Display="dynamic" ErrorMessage="!!"></asp:RequiredFieldValidator><br />
                                            <asp:DropDownList ID="ddAddXmlPackage" runat="server" CssClass="default" Width="200px" OnSelectedIndexChanged="ddAddXmlPackage_SelectedIndexChanged"></asp:DropDownList>
                                            <asp:TextBox ID="txtAddURL" runat="server" CssClass="default" Width="140">Callout URL</asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator" runat="server" ControlToValidate="txtAddURL" Display="dynamic" ErrorMessage="!!"></asp:RequiredFieldValidator>
                                            <asp:Button ID="btnInsert" runat="server" CssClass="normalButton" OnClick="btnInsert_Click" Text="ADD" />
                                        </asp:Panel>
                                        <br />
                                        <br />
                                        <br />
                                    </td>
                                    <td style="width: 5px">
                                    </td>
                                    <td style="width: 1px; background-color: #a2a2a2">
                                    </td>
                                    <td style="width: 5px">
                                    </td>
                                    <td class="contentTable" valign="top" width="*">
                                        <div class="wrapperLeft">
                                            <asp:GridView ID="gMain" runat="server" AllowPaging="true" AllowSorting="True" AutoGenerateColumns="False" CssClass="overallGrid" HorizontalAlign="Left" OnPageIndexChanging="gMain_PageIndexChanging" OnRowCancelingEdit="gMain_RowCancelingEdit" OnRowCommand="gMain_RowCommand" OnRowDataBound="gMain_RowDataBound" OnRowEditing="gMain_RowEditing" OnRowUpdating="gMain_RowUpdating" OnSorting="gMain_Sorting" PagerSettings-Position="TopAndBottom" PagerStyle-HorizontalAlign="left" PageSize="15" Width="100%">
                                                <Columns>
                                                    <asp:CommandField ButtonType="Image" CancelImageUrl="icons/cancel.gif" CancelText="Cancel"
                                                        EditImageUrl="icons/edit.gif" EditText="Edit" ShowEditButton="True" UpdateImageUrl="icons/update.gif"
                                                        UpdateText="Update" />
                                                    <asp:BoundField DataField="EventID" HeaderText="ID" ItemStyle-CssClass="lighterData"
                                                        ReadOnly="True" SortExpression="EventID" />
                                                    <asp:TemplateField HeaderText="EventName" ItemStyle-CssClass="normalData" SortExpression="EventName">
                                                        <ItemTemplate>
                                                            <%# DataBinder.Eval(Container.DataItem, "EventName") %>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="txtEventName" runat="Server" CssClass="singleAuto" Text='<%# DataBinder.Eval(Container.DataItem, "EventName") %>'></asp:TextBox>
                                                            <asp:Literal ID="ltEventName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "EventName") %>'></asp:Literal>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator" runat="server" ControlToValidate="txtEventName"
                                                                ErrorMessage="!!"></asp:RequiredFieldValidator>
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="XML Package" ItemStyle-CssClass="lightestData">
                                                        <ItemTemplate>
                                                            <div style="white-space: normal; overflow: visible; width: 100%;">
                                                                <%# DataBinder.Eval(Container.DataItem, "XmlPackage") %>
                                                            </div>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:DropDownList ID="ddEditXMLPackage" runat="server" CssClass="default"></asp:DropDownList>
                                                            OR 
                                                            <asp:TextBox ID="txtXMLPackage" runat="Server" CssClass="default"></asp:TextBox>
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Callout URL" ItemStyle-CssClass="normalData">
                                                        <ItemTemplate>
                                                            <div style="white-space: normal; overflow: visible; width: 225px;">
                                                                <%# DataBinder.Eval(Container.DataItem, "CalloutURL") %>
                                                            </div>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="txtCalloutURL" runat="server" CssClass="singleAuto" Text='<%# DataBinder.Eval(Container.DataItem, "CalloutURL") %>'
                                                                TextMode="SingleLine"></asp:TextBox>
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Active" ItemStyle-CssClass="normalData">
                                                        <ItemTemplate>
                                                            <div style="white-space: normal; overflow: visible; width: 225px;">
                                                                <%# DataBinder.Eval(Container.DataItem, "Active") %>
                                                            </div>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:CheckBox ID="cbkActive" runat="server" CssClass="singleAuto" Checked='<%# DataBinder.Eval(Container.DataItem, "Active") %>'></asp:CheckBox>
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Debug" ItemStyle-CssClass="normalData">
                                                        <ItemTemplate>
                                                            <div style="white-space: normal; overflow: visible; width: 225px;">
                                                                <%# DataBinder.Eval(Container.DataItem, "Debug") %>
                                                            </div>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:CheckBox ID="cbkDebug" runat="server" CssClass="singleAuto" Checked='<%# DataBinder.Eval(Container.DataItem, "Debug") %>'></asp:CheckBox>
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="selectData">
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="imgDelete" runat="Server" AlternateText="Delete" CommandArgument='<%# Eval("EventID") %>'
                                                                CommandName="DeleteItem" ImageUrl="icons/delete.gif" />
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
    
    </div>
    </form>
</body>
</html>
