<%@ Page Language="C#" AutoEventWireup="true" CodeFile="splash.aspx.cs" Inherits="AspDotNetStorefrontAdmin.splash" Theme="default" %>
<%@ Register TagPrefix="aspdnsf" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Home Page</title>
</head>
<body>
    <form id="frmAppConfig" runat="server">
    <div id="help">
        <div>
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
                                            Home
                                        </font>
                                    </td>
                                    <td style="width: 10px;" />
                                    <td class="titleTable">
                                        <font class="subTitle">View:</font>
                                    </td>
                                    <td class="contentTable">
                                        <a href="sitemap2.aspx">Site Map</a>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div style="margin-bottom: 5px; margin-top: 5px;">
            <asp:Literal ID="ltError" runat="server"></asp:Literal>
        </div>
    </div>
    <div id="content">
        <table width="100%" cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td valign="top" width="400">
                    <!-- LEFT COLUMN -->
                    <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                        <tr>
                            <td>
                                <div class="wrapper">                       
                                    <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                                        <tr>
                                            <td class="titleTable" width="100%">
                                                <font class="subTitle">Common Links:</font>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="contentTable" width="100%">
                                                <div style="margin-bottom: -10px;">
                                                    <ul>
                                                        <li runat="server" id="ChangeEncryptKeyReminder" visible="false"><a href="changeencryptkey.aspx"><font color="red"><b>Time To Change Your Encrypt Key!<b></font></a></li>
                                                        <li runat="server" id="WizardPrompt"><a href="wizard.aspx">Run Configuration Wizard</a></li>
                                                        <li runat="server" id="MonthlyMaintenancePrompt" visible="true"><a href="monthlymaintenance.aspx">Run Monthly Maintenance</a></li>
                                                        <li><a href="orders.aspx"><br />View/Manage Orders</a></li>
                                                        <li><a href="entityframe.aspx">View/Manage Products</a></li>
                                                        <li><a href="customers.aspx">View/Manage Customers</a></li>
                                                        <li><a href="appconfig.aspx">View/Edit AppConfigs<br /><br /></a></li>
                                                        <li><a href="appconfig.aspx?searchfor=mail">E-Mail Settings</a></li>
                                                        <li><asp:HyperLink ID="lnkGateway" runat="server" Text="Gateway Settings"></asp:HyperLink></li>
                                                        <li><asp:LinkButton ID="lnkSSL" runat="Server" OnClick="lnkSSL_Click" Enabled="false" Visible="false"/></li>
                                                    </ul>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                                        <tr>
                                            <td class="titleTable" width="100%">
                                                <font class="subTitle">System Information:</font>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="contentTable" width="100%">
                                                <div style="margin-bottom: 0px; margin-top: 3px;">
                                                    <table width="100%" cellpadding="2" cellspacing="0" border="0">
                                                        <tr><td width="180" align="right" valign="top"><font class="subTitleSmall">Version (Code/DB):</font></td><td valign="top"><asp:Literal ID="ltStoreVersion" runat="server"></asp:Literal></td></tr>
                                                        <tr><td align="right"><font class="subTitleSmall">Current Server Date/Time:</font></td><td><asp:Literal ID="ltDateTime" runat="server"></asp:Literal></td></tr>
                                                        <tr><td align="right"><font class="subTitleSmall">UseSSL:</font></td><td><asp:Literal ID="ltUseSSL" runat="server"></asp:Literal></td></tr>
                                                        <tr><td align="right"><font class="subTitleSmall">On Live Server:</font></td><td><asp:Literal ID="ltOnLiveServer" runat="server"></asp:Literal></td></tr>
                                                        <tr><td align="right"><font class="subTitleSmall">SERVER_PORT_SECURE:</font></td><td><asp:Literal ID="ltServerPortSecure" runat="server"></asp:Literal></td></tr>
                                                        <tr><td align="right"><font class="subTitleSmall">Caching Is:</font></td><td><asp:Literal ID="ltCaching" runat="server"></asp:Literal></td></tr>
                                                        <tr><td align="right"><font class="subTitleSmall">Primary Store Locale Setting:</font></td><td><asp:Literal ID="ltWebConfigLocaleSetting" runat="server"></asp:Literal></td></tr>
                                                        <tr><td align="right"><font class="subTitleSmall">SQL Locale Setting:</font></td><td><asp:Literal ID="ltSQLLocaleSetting" runat="server"></asp:Literal></td></tr>
                                                        <tr><td align="right"><font class="subTitleSmall">Customer Locale Setting:</font></td><td><asp:Literal ID="ltCustomerLocaleSetting" runat="server"></asp:Literal></td></tr>
                                                        <tr><td align="right"><font class="subTitleSmall">Primary Store Currency:</font></td><td><asp:Literal ID="PrimaryCurrency" runat="server"></asp:Literal></td></tr>
                                                        <tr><td align="right"><font class="subTitleSmall">Payment Gateway:</font></td><td><asp:Literal ID="ltPaymentGateway" runat="server"></asp:Literal></td></tr>
                                                        <tr><td align="right"><font class="subTitleSmall">Gateway Mode:</font></td><td><asp:Literal ID="ltUseLiveTransactions" runat="server"></asp:Literal></td></tr>
                                                        <tr><td align="right"><font class="subTitleSmall">Transaction Mode:</font></td><td><asp:Literal ID="ltTransactionMode" runat="server"></asp:Literal></td></tr>
                                                        <tr><td align="right"><font class="subTitleSmall">Payment Methods:</font></td><td><asp:Literal ID="ltPaymentMethods" runat="server"></asp:Literal></td></tr>
                                                        <tr><td align="right"><font class="subTitleSmall">MicroPay Enabled:</font></td><td><asp:Literal ID="ltMicroPayEnabled" runat="server"></asp:Literal></td></tr>
                                                        <tr><td align="right"><font class="subTitleSmall">Cardinal Enabled:</font></td><td><asp:Literal ID="CardinalEnabled" runat="server"></asp:Literal></td></tr>
                                                        <tr><td align="right"><font class="subTitleSmall">Store Credit Cards:</font></td><td><asp:Literal ID="StoreCC" runat="server"></asp:Literal></td></tr>
                                                        <tr><td align="right"><font class="subTitleSmall">Using Gatway Recurring Billing:</font></td><td><asp:Literal ID="GatewayRecurringBilling" runat="server"></asp:Literal></td></tr>
                                                    </table>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>

                </td>
                <td style="width: 5px;" />
                <td valign="top" > 
                    <!-- RIGHT COLUMN -->
                    <table id="StatsTable" runat="server" border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                        <tr>
                            <td>
                                <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                                    <tr>
                                        <td class="contentTable" width="100%" style="height: 23px">
                                            <div style="margin-bottom: 0px; margin-top: 5px; text-align:left; margin-right: 2px;">
                                                <asp:Literal ID="ltCustomerStats" runat="server"></asp:Literal>
                                                <br />
                                                <asp:Literal ID="ltStatistics" runat="server"></asp:Literal>
                                                <br />
                                                <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                                                <br />
                                                <asp:Literal ID="Literal2" runat="server"></asp:Literal>
                                                <br />
                                                <asp:Literal ID="Literal3" runat="server"></asp:Literal></div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>



                    <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                        <tr>
                            <td width="100%">
                                <div class="wrapper">                       
                                    <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                                        <tr>
                                            <td class="titleTable" width="100%">
                                                <font class="subTitle" style="float:left;">Latest Orders:</font>
                                                <div style="float: left">
                                                    &nbsp;<a href="orders.aspx">View Orders</a>
                                                    &nbsp;<a href="orderReports.aspx">Order Reports</a>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="contentTable" width="100%">
                                                <div style="margin-bottom: 10px; margin-top:5px; text-align:left;">
                                                    <asp:GridView ID="gOrders" AutoGenerateColumns="false" ShowHeader="true" ShowFooter="false" CssClass="overallGrid" runat="server" AllowPaging="false" AllowSorting="false" OnRowDataBound="gOrders_RowDataBound" Width="100%">
                                                        <FooterStyle CssClass="footerGrid" />
                                                        <RowStyle CssClass="DataCellGrid" />
                                                        <EditRowStyle CssClass="DataCellGridEdit" />
                                                        <PagerStyle CssClass="pagerGrid" />
                                                        <HeaderStyle CssClass="headerGrid" />
                                                        <AlternatingRowStyle CssClass="DataCellGridAlt" />
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Order">
                                                                <ItemTemplate> 
                                                                    <%# "<a href=\"orders.aspx?ordernumber=" + DataBinder.Eval(Container.DataItem, "OrderNumber") + "\">" + DataBinder.Eval(Container.DataItem, "OrderNumber") + "</a>" %>
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="lighterData" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Date">
                                                                <ItemTemplate> 
                                                                    <%# DataBinder.Eval(Container.DataItem, "OrderDate") %>
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="lightData" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Customer">
                                                                <ItemTemplate> 
                                                                    <%# (DataBinder.Eval(Container.DataItem, "FirstName") + " " + DataBinder.Eval(Container.DataItem, "LastName")).Trim() %>
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="normalData" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Shipping">
                                                                <ItemTemplate> 
                                                                    <%# DataBinder.Eval(Container.DataItem, "ShippingMethod") %>
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="normalData" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Total">
                                                                <ItemTemplate> 
                                                                    <%# AspDotNetStorefrontCommon.Localization.CurrencyStringForDisplayWithoutExchangeRate((decimal)DataBinder.Eval(Container.DataItem, "OrderTotal"),false) %>
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="normalData" />
                                                            </asp:TemplateField> 
                                                            <asp:TemplateField HeaderText="MaxMind" ItemStyle-HorizontalAlign="right">
                                                                <ItemTemplate> 
                                                                    <%# AspDotNetStorefrontCommon.Localization.DecimalStringForDB((decimal)DataBinder.Eval(Container.DataItem, "MaxMindFraudScore")) %>
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="normalData" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>

                                            </td>
                                        </tr>
                                    </table>
                                    <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                                        <tr>
                                            <td class="titleTable" width="100%">
                                                <font class="subTitle" style="float: left;">Latest Registered Customers:</font>
                                                <div style="float: left">
                                                    &nbsp;<a href="customers.aspx">View Customers</a>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="contentTable" width="100%">
                                                <div style="margin-bottom: 0px; margin-top:5px; text-align:left;">
                                                    <asp:GridView ID="gCustomers" AutoGenerateColumns="false" ShowHeader="true" ShowFooter="false" CssClass="overallGrid" runat="server" AllowPaging="false" AllowSorting="false" Width="100%">
                                                        <FooterStyle CssClass="footerGrid" />
                                                        <RowStyle CssClass="DataCellGrid" />
                                                        <EditRowStyle CssClass="DataCellGridEdit" />
                                                        <PagerStyle CssClass="pagerGrid" />
                                                        <HeaderStyle CssClass="headerGrid" />
                                                        <AlternatingRowStyle CssClass="DataCellGridAlt" />
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="CustomerID">
                                                                <ItemTemplate> 
                                                                    <%# DataBinder.Eval(Container.DataItem, "CustomerID") %>
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="lighterData" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Date">
                                                                <ItemTemplate> 
                                                                    <%# DataBinder.Eval(Container.DataItem, "RegisterDate") %>
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="lightData" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Customer">
                                                                <ItemTemplate> 
                                                                    <%# (DataBinder.Eval(Container.DataItem, "FirstName") + " " + DataBinder.Eval(Container.DataItem, "LastName")).Trim() %>
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="normalData" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                                <asp:Panel ID="pnlSecurityFeed" runat="server" Visible="true">   
                <aspdnsf:XmlPackage id="SecurityFeed" runat="server" PackageName="rss.aspdnsfrssconsumer.xml.config" RunTimeParams="channel=security&height=500"/>
                </asp:Panel>
                                                 <asp:Panel ID="pnlNewsFeed" runat="server" Visible="true" Width="100%">   
                    <aspdnsf:XmlPackage id="NewsFeed" runat="server" PackageName="rss.aspdnsfrssconsumer.xml.config" RunTimeParams="channel=news"/>
                    </asp:Panel>
                    <!--                   <asp:Panel ID="pnlSponsorFeed" runat="server" Visible="true">   
                    <aspdnsf:XmlPackage id="SponsorFeed" runat="server" PackageName="rss.aspdnsfrssconsumer.xml.config" RunTimeParams="channel=sponsors&height=330"/>
                    </asp:Panel>-->

                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
