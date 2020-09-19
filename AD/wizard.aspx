<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wizard.aspx.cs" Inherits="AspDotNetStorefrontAdmin.wizard" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Wizard</title>
    <script type="text/javascript"  src="jscripts/toolTip.js">
        /***********************************************
        * Cool DHTML tooltip script II- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
        * This notice MUST stay intact for legal use
        * Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
        ***********************************************/
    </script>
</head>

<body>
    <form id="frmWizard" runat="server">   
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
                                        Configuration Wizard
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
                                <td class="titleTable">
                                    <font class="subTitle">Wizard:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="100%">
                                    <div class="wrapper">
                                        This wizard can help you configure your store's primary configuration variables after first installation. 
                                        <br />
                                        <div id="divMain" runat="server">
                                            <p>Fields marked with an asterisk (*) are required. All other fields are optional.</p>
                                            <table cellpadding="1" cellspacing="0" border="0">
                                                <tr>
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">*Store Name:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox ID="txtStoreName" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                        <img src="images/info.gif" border="0" onmouseout="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter the name of your store, e.g. ACME Widgets.</font>', 300)" alt="" />
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator" runat="server" ControlToValidate="txtStoreName" SetFocusOnError="true" ErrorMessage="Fill in Store Name">!!</asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">*Store Domain:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox ID="txtStoreDomain" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                        <img src="images/info.gif" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter the domain name of the production store site, with no www, e.g. yourdomain.com</font>', 300)" border="0" onmouseout="hideddrivetip()" alt="" />
                                                        <asp:RequiredFieldValidator ControlToValidate="txtStoreDomain" ErrorMessage="Fill in Store Domain" ID="RequiredFieldValidator1" runat="server" SetFocusOnError="true">!!</asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">*Store E-Mail Address:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox ID="txtStoreEmail" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                        <img src="images/info.gif" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter the e-mail address that the store should use to send order receipts, e.g. sales@yourdomain.com</font>', 300)" border="0" onmouseout="hideddrivetip()" alt="" />
                                                        <asp:RegularExpressionValidator ControlToValidate="txtStoreEmail" ErrorMessage="Invalid Store E-Mail" ID="RequiredFieldValidator2" runat="server" SetFocusOnError="true" ValidationExpression="^[a-zA-Z0-9][-\w\.\+]*@([a-zA-Z0-9][\w\-]*\.)+[a-zA-Z]{2,4}$">!</asp:RegularExpressionValidator>
                                                        <asp:RequiredFieldValidator ControlToValidate="txtStoreEmail" ErrorMessage="Fill in Store Email" ID="RequiredFieldValidator3" runat="server" SetFocusOnError="true">!!</asp:RequiredFieldValidator>                                                        
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">*Store E-Mail Name:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox ID="txtEmailName" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                        <img src="images/info.gif" border="0" onmouseout="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter the friendly name for the store e-mail address, e.g. Sales</font>', 300)" alt="" />
                                                        <asp:RequiredFieldValidator ControlToValidate="txtEmailName" ErrorMessage="Fill in Store E-Mail Name" ID="RequiredFieldValidator4" runat="server" SetFocusOnError="true">!!</asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">Store Zip Code:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox ID="txtZip" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                        <img onmouseover="ddrivetip('<font class=\'exampleText\'>If using Real Time Shipping, the store needs to know your shipment source zip code.</font>', 300)" src="images/info.gif" border="0" onmouseout="hideddrivetip()" alt="" />                                                        
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">Transaction Mode:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:DropDownList runat="server" ID="ddMode">
                                                            <asp:ListItem>AUTH</asp:ListItem>
                                                            <asp:ListItem>AUTH CAPTURE</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <img onmouseover="ddrivetip('<font class=\'exampleText\'>AUTH = Authorize Orders Only. AUTH CAPTURE = Authorize AND Capture Orders in Real Time.</font>', 300)" src="images/info.gif" border="0" onmouseout="hideddrivetip()" alt="" />                                                        
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">Store Currency:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox ID="txtCurrency" runat="server" CssClass="single3chars" MaxLength="3"></asp:TextBox>
                                                        <img onmouseover="ddrivetip('<font class=\'exampleText\'>Your store master currency, this is the ISO 4217 Standard Code, e.g. USD.</font>', 300)" src="images/info.gif" border="0" onmouseout="hideddrivetip()" alt="" />                                                        
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">Store Currency Numberic Code:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox ID="txtCurrencyNumeric" runat="server" CssClass="single3chars" MaxLength="3"></asp:TextBox>
                                                        <img onmouseover="ddrivetip('<font class=\'exampleText\'>Your store master currency numeric code, this is the ISO 4217 Standard Numeric Code, e.g. 840.</font>', 300)" src="images/info.gif" border="0" onmouseout="hideddrivetip()" alt="" />                                                        
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="250" align="right" valign="top">
                                                        <font class="subTitleSmall">Payment Methods Accepted:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:CheckBoxList runat="server" ID="cblPaymentMethods">
                                                            <asp:ListItem Value="Credit Card">Credit Card</asp:ListItem>
                                                            <asp:ListItem Value="PayPal">PayPal <img onmouseover="ddrivetip('<font class=\'exampleText\'>It is highly recommended that you use PayPal Express Checkout instead of this option when possible.</font>', 300)" src="images/info.gif" border="0" onmouseout="hideddrivetip()" align="absmiddle" alt="" /></asp:ListItem>
                                                            <asp:ListItem Value="PayPalExpress">PayPal Express Checkout <img onmouseover="ddrivetip('<font class=\'exampleText\'>If you select Express Checkout, you probably don\'t want to select PayPal above.</font>', 300)" src="images/info.gif" border="0" onmouseout="hideddrivetip()" align="absmiddle" alt="" /></asp:ListItem>
                                                            <asp:ListItem Value="Request Quote">Request For Quotes</asp:ListItem>
                                                            <asp:ListItem Value="Purchase Order">Purchase Orders</asp:ListItem>
                                                            <asp:ListItem Value="Check By Mail">Checks</asp:ListItem>
                                                            <asp:ListItem Value="C.O.D.">C.O.D.</asp:ListItem>
                                                            <asp:ListItem Value="ECHECK">E-Checks</asp:ListItem>
                                                            <asp:ListItem Value="MICROPAY">MICROPAY</asp:ListItem>
                                                        </asp:CheckBoxList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="250" align="right" valign="top">
                                                        <font class="subTitleSmall">Payment Gateway:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:RadioButtonList ID="rblGateway" runat="server" RepeatColumns="1" RepeatDirection="Vertical">
                                                            <asp:ListItem Value="MANUAL" Selected="true">Manual <img onmouseover="ddrivetip('<font class=\'exampleText\'>No gateway, cards are not charged, order info just stored.</font>', 300)" src="images/info.gif" border="0" onmouseout="hideddrivetip()" align="absmiddle" alt="" /></asp:ListItem>
                                                            <asp:ListItem Value="2CHECKOUT">2Checkout</asp:ListItem>
                                                            <asp:ListItem Value="AUTHORIZENET">Authorize.net</asp:ListItem>
                                                            <asp:ListItem Value="CARDIASERVICES">Cardia Services (Norway)</asp:ListItem>
                                                            <asp:ListItem Value="CENTRALPAYMENTS">CentralPayments</asp:ListItem>
                                                            <asp:ListItem Value="CYBERSOURCE">Cybersource</asp:ListItem>
                                                            <asp:ListItem Value="EFSNET">EFSNET</asp:ListItem>
                                                            <asp:ListItem Value="EPROCESSINGNETWORK">eProcessingNetwork</asp:ListItem>
                                                            <asp:ListItem Value="EWAY">eWay (Australia)</asp:ListItem>
                                                            <asp:ListItem Value="ESELECTPLUS">eSelectPlus (Moneris), US Version</asp:ListItem>
                                                            <asp:ListItem Value="HSBC">HSBC</asp:ListItem>
                                                            <asp:ListItem Value="IDEPOSIT">iDeposit.net</asp:ListItem>
                                                            <asp:ListItem Value="ITRANSACT">ITransact</asp:ListItem>
                                                            <asp:ListItem Value="JETPAY">JetPay</asp:ListItem>
                                                            <asp:ListItem Value="IATS">IATS Ticketmaster</asp:ListItem>
                                                            <asp:ListItem Value="LINKPOINT">Linkpoint</asp:ListItem>
                                                            <asp:ListItem Value="NETBILLING">NetBilling</asp:ListItem>
                                                            <asp:ListItem Value="PAYFUSE">PayFuse</asp:ListItem>
                                                            <asp:ListItem Value="PAYJUNCTION">PayJunction</asp:ListItem>
                                                            <asp:ListItem Value="PAYMENTECH">Paymentech</asp:ListItem>
                                                            <asp:ListItem Value="PAYFLOWPRO">PayPal PayFlow Pro (PayPal UK Website Payments Pro)</asp:ListItem>
                                                            <asp:ListItem Value="PAYPALPRO">PayPal Website Payments Pro (enables PayPal Express Checkout also) US Only</asp:ListItem>
                                                            <asp:ListItem Value="PINNACLEPAYMENTS">Pinnacle Payments (SmartPayments)</asp:ListItem>
                                                            <asp:ListItem Value="PLUGNPAY">PlugNPay</asp:ListItem>
                                                            <asp:ListItem Value="PROTX">ProtX</asp:ListItem>
                                                            <asp:ListItem Value="QUICKBOOKS">Quick Books Merchant Services</asp:ListItem>
                                                            <asp:ListItem Value="QUICKCOMMERCE">QuickCommerce</asp:ListItem>
                                                            <asp:ListItem Value="SKIPJACK">SkipJack</asp:ListItem>
                                                            <asp:ListItem Value="TELLUS">TELLUS</asp:ListItem>
                                                            <asp:ListItem Value="TRANSACTIONCENTRAL">Transaction Central <img onmouseover="ddrivetip('<font class=\'exampleText\'>Same as MerchantAnywhere.</font>', 300)" src="images/info.gif" border="0" onmouseout="hideddrivetip()" alt="" align="absmiddle" /></asp:ListItem>
                                                            <asp:ListItem Value="USAEPAY">USAePay</asp:ListItem>
                                                            <asp:ListItem Value="VIAKLIX">viaKLIX <img onmouseover="ddrivetip('<font class=\'exampleText\'>To use the viaKLIX gateway you must set the Transaction Mode to AUTH CAPTURE above.</font>', 300)" src="images/info.gif" border="0" onmouseout="hideddrivetip()" alt="" align="absmiddle" /></asp:ListItem>
                                                            <asp:ListItem Value="WORLDPAY JUNIOR">WORLDPAY JUNIOR</asp:ListItem>
                                                            <asp:ListItem Value="YOURPAY">YourPay</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="250" align="right" valign="top">
                                                        <font class="subTitleSmall">Use Live Transactions:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:RadioButtonList ID="rblLiveTransactions" runat="server" RepeatColumns="2" RepeatDirection="Horizontal" CellPadding="5" CellSpacing="0">
                                                            <asp:ListItem Value="false" Selected="true">No <img onmouseover="ddrivetip('<font class=\'exampleText\'>Store Test Mode.</font>', 300)" align="absmiddle" src="images/info.gif" border="0" onmouseout="hideddrivetip()" alt="" /></asp:ListItem>
                                                            <asp:ListItem Value="true">Yes <img onmouseover="ddrivetip('<font class=\'exampleText\'>Live Mode.</font>', 300)" align="absmiddle" src="images/info.gif" border="0" onmouseout="hideddrivetip()" alt="" /></asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="250" align="right" valign="top">
                                                        <font class="subTitleSmall">Use SSL:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:RadioButtonList ID="rblSSL" runat="server" RepeatColumns="2" RepeatDirection="Horizontal" CellPadding="5" CellSpacing="0">
                                                            <asp:ListItem Value="false" Selected="true">No <img onmouseover="ddrivetip('<font class=\'exampleText\'>No SSL.</font>', 300)" align="absmiddle" src="images/info.gif" border="0" onmouseout="hideddrivetip()" alt="" /></asp:ListItem>
                                                            <asp:ListItem Value="true">Yes <img onmouseover="ddrivetip('<font class=\'exampleText\'>You MUST have your SSL certificate installed BEFORE checking this yes!!!</font>', 300)" align="absmiddle" src="images/info.gif" border="0" onmouseout="hideddrivetip()" alt="" /></asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                </tr>
                                                <tr id="EncryptWebConfigRow" runat="server">
                                                    <td width="250" align="right" valign="top">
                                                        <font class="subTitleSmall">Encrypt the Web.Config:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:RadioButtonList ID="rblEncrypt" runat="server" RepeatColumns="2" RepeatDirection="Horizontal" CellPadding="5" CellSpacing="0">
                                                            <asp:ListItem Value="false" Selected="true">No <img onmouseover="ddrivetip('<font class=\'exampleText\'>select No to make your web.config editable</font>', 300)" align="absmiddle" src="images/info.gif" border="0" onmouseout="hideddrivetip()" alt="" /></asp:ListItem>
                                                            <asp:ListItem Value="true">Yes <img onmouseover="ddrivetip('<font class=\'exampleText\'>select Yes to encrypt your web.config and protect sensitive application data</font>', 300)" align="absmiddle" src="images/info.gif" border="0" onmouseout="hideddrivetip()" alt="" /></asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>&nbsp;</td>
                                                    <td align="left" style="padding-top: 5px;">
                                                        <asp:Button ID="btnSubmit" runat="Server" CssClass="normalButton" Text="Submit" OnClick="btnSubmit_Click" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:ValidationSummary ID="validationSummary" runat="server" EnableClientScript="true" ShowMessageBox="true" ShowSummary="false" Enabled="true" />
                                        </div>
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
