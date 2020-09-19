<%@ Page Language="C#" AutoEventWireup="true" CodeFile="editgiftcard.aspx.cs" Inherits="AspDotNetStorefrontAdmin.editgiftcard" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Edit Gift Card</title>
    <script type="text/javascript" src="jscripts/toolTip.js" >
        /***********************************************
        * Cool DHTML tooltip script II- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
        * This notice MUST stay intact for legal use
        * Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
        ***********************************************/
    </script>
    <asp:Literal runat="server" id="ltStyles"></asp:Literal>
</head>

<body>
    <form id="frmGiftcards" runat="server">
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
                                        <a href="giftCards.aspx">Manage Gift Cards</a> ->
                                        Edit Gift Card: <span style="color: Red;"><asp:Literal ID="ltCard" runat="server"></asp:Literal></span>
                                        &nbsp;
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
                                    |
                                    <a href="giftcards.aspx">Gift Cards</a>
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
                                <td class="titleTable" width="100%">
                                    <font class="subTitle">Gift Card Details:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="*">
                                    <div class="wrapper">
                                        <div id="divMain" runat="server">
                                            To update or view the transaction history for this gift card, or to add or remove funds from this gift card, click <asp:HyperLink ID="lnkUsage" runat="server" Text="here"></asp:HyperLink>.
                                            <br /><br />
                                            Fields marked with an asterisk (*) are required. All other fields are optional.
                                            <br /><br />
                                            <table cellpadding="1" cellspacing="0" border="0">
                                                <tr>
                                                    <td width="250" align="right">
                                                        <asp:Label ID="lblAction" runat="server" CssClass="subTitleSmall" Text="Action"></asp:Label>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:RadioButtonList ID="rblAction" runat="server">
                                                            <asp:ListItem Value="0">Enabled</asp:ListItem>
                                                            <asp:ListItem Value="1">Disabled</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">*Serial Number:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox ValidationGroup="main" ID="txtSerial" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                        <img src="images/info.gif" border="0" onmouseout="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter the Gift Card\'s Serial Number</font>', 300)" alt="" />
                                                        <asp:RequiredFieldValidator ValidationGroup="main" ID="RequiredFieldValidator" runat="server" ControlToValidate="txtSerial" SetFocusOnError="true" ErrorMessage="Fill in Serial Number">!!</asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">*Expiration Date:</font>
                                                    </td>
                                                    <td align="left" valign="top">
                                                        <asp:TextBox ValidationGroup="main" ID="txtDate" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                        <asp:Literal ID="ltDate" runat="server"></asp:Literal>
                                                        <asp:RequiredFieldValidator ValidationGroup="main" ErrorMessage="Fill in Expiration Date" ControlToValidate="txtDate" ID="RequiredFieldValidator3" SetFocusOnError="true" runat="server">!!</asp:RequiredFieldValidator> 
                                                    </td>
                                                </tr>
                                                 <tr runat="server" id="PurchasedByCustomerIDTextRow">
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">Purchased by Customer ID:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox ValidationGroup="main" ID="txtCustomer" runat="server" CssClass="singleShorter"></asp:TextBox>
                                                        <img src="images/info.gif" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter the Customer ID of the purchasing customer.</font>', 300)" border="0" onmouseout="hideddrivetip()" alt="" />
                                                    </td>
                                                </tr>                                                
                                                <tr runat="server" id="PurchasedByCustomerIDLiteralRow">
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">Purchased by Customer ID:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                       <asp:Literal ID="ltCustomerID" runat="server"></asp:Literal>
                                                       [<asp:Literal ID="ltCustomer2" runat="server">Customer Name</asp:Literal>]
                                                      </td>
                                                </tr>   
                                                <tr runat="server" id="OrderNumberRow">
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">Original Order Number:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:Literal ID="txtOrder" runat="server"></asp:Literal>
                                                    </td>
                                                </tr>
                                                <tr runat="server" id="InitialAmountTextRow">
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">*Initial Amount:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox ValidationGroup="main" ID="txtAmount" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                                                        <asp:RequiredFieldValidator ValidationGroup="main" ErrorMessage="Please enter the initial gift card value" ControlToValidate="txtAmount" ID="RequiredFieldValidator2" SetFocusOnError="true" runat="server">!!</asp:RequiredFieldValidator> 
                                                </td>
                                                </tr>
                                                <tr runat="server" id="InitialAmountLiteralRow">
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">Initial Amount:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:Literal ID="ltAmount" runat="server"></asp:Literal>
                                                </td>
                                                </tr>
                                                <tr runat="server" id="RemainingBalanceRow">
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">Remaining Balance:</font>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:Literal ID="ltCurrentBalance" runat="server"></asp:Literal>
                                                    </td>
                                                </tr>
<%--                                                  <tr>
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">Valid For Customer(s):</font>
                                                    </td>
                                                    <td align="left" valign="top">
                                                        <asp:TextBox ID="txtCustomers" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                        <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter the customer id(s) for which this coupon is valid, or leave blank to allow any customer to use it. Enter customer id\'s separated by a comma, e.g. 12343, 12344, 12345, etc...</font>', 300)" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                  <td width="250" align="right">
                                                        <font class="subTitleSmall">Valid For Product(s):</font>
                                                    </td>
                                                    <td align="left" valign="top">
                                                        <asp:TextBox ID="txtProducts" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                        <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter the product id(s) for which this coupon is valid, or leave blank to allow it to work on any product. Enter product id\'s separated by a comma, e.g. 40, 41, 42, etc...</font>', 300)" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">Valid For Category(s):</font>
                                                    </td>
                                                    <td align="left" valign="top">
                                                        <asp:TextBox ID="txtCategory" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                        <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter the section id(s) for which this coupon is valid, or leave blank to allow it to work on any section. Enter section id\'s separated by a comma, e.g. 1,2,3, etc...</font>', 300)" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">Valid For Section(s):</font>
                                                    </td>
                                                    <td align="left" valign="top">
                                                        <asp:TextBox ID="txtSection" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                        <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter the category id(s) for which this coupon is valid, or leave blank to allow it to work on any category. Enter category id\'s separated by a comma, e.g. 1,2,3, etc...</font>', 300)" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="250" align="right">
                                                        <font class="subTitleSmall">Valid For Manufacturer(s):</font>
                                                    </td>
                                                    <td align="left" valign="top">
                                                        <asp:TextBox ID="txtManufacturer" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                        <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter the manufacturer id(s) for which this coupon is valid, or leave blank to allow it to work on any manufacturer. Enter manufacturer id\'s separated by a comma, e.g. 1,2,3, etc...</font>', 300)" />
                                                    </td>
                                                </tr>--%>
                                                <tr runat="server" id="GiftCardTypeSelectRow">
                                                    <td width="250" align="right" style="padding-top: 10px;">
                                                        <font class="subTitleSmall">*Gift Card Type:</font>
                                                    </td>
                                                    <td align="left" valign="middle" style="padding-top: 10px;">
                                                        <asp:DropDownList ValidationGroup="main" ID="ddType" runat="server" CssClass="default">
                                                            <asp:ListItem Value="0"> - Select Type -</asp:ListItem>
                                                            <asp:ListItem Value="102">Certificate</asp:ListItem>
                                                            <asp:ListItem Value="101">E-Mail</asp:ListItem>
                                                            <asp:ListItem Value="100">Physical</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <img src="images/info.gif" border="0" onmouseout="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>The type of Gift Card.</font>', 300)" alt="" />
                                                        <asp:RequiredFieldValidator ValidationGroup="main" ControlToValidate="ddType" InitialValue="0" ErrorMessage="Select Gift Card Type" ID="RequiredFieldValidator5" runat="server" SetFocusOnError="true">!!</asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr runat="server" id="GiftCardTypeDisplayRow">
                                                    <td width="250" align="right" style="padding-top: 10px;">
                                                        <font class="subTitleSmall">Gift Card Type:</font>
                                                    </td>
                                                    <td align="left" valign="middle" style="padding-top: 10px;">
                                                    <asp:Literal ID="ltGiftCardType" runat="server" />
                                                    </td>
                                                </tr>

                                                <tr id="trEmail" runat="server">
                                                    <td width="250" align="right">
                                                        &nbsp;
                                                    </td>                                                    
                                                    <td align="left" valign="top" style="background-color: #f2f2f2; padding: 5px 3px 2px 3px;">
                                                        If gift card is <b>E-Mail type</b>:
                                                        <table cellpadding="1" cellspacing="0" border="0" style="margin-top: 10px;">
                                                            <tr>
                                                                <td align="right">
                                                                    <font class="subTitleSmall">E-Mail Subject:</font>
                                                                </td>
                                                                <td align="left" valign="middle">
                                                                    <asp:TextBox ID="txtEmailName" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                                    <img src="images/info.gif" border="0" onmouseout="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>The subject of the E-Mail</font>', 300)" alt="" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right">
                                                                    <font class="subTitleSmall">E-Mail To:</font>
                                                                </td>
                                                                <td align="left" valign="middle">
                                                                    <asp:TextBox ID="txtEmailTo" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                                    <img src="images/info.gif" border="0" onmouseout="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>The E-Mail address of recipient</font>', 300)" alt="" />
                                                                       <asp:RegularExpressionValidator id="RegularExpressionValidator1" runat="server" Display="None" ControlToValidate="txtEmailTo" EnableClientScript="false" ErrorMessage="Invalid E-Mail" SetFocusOnError="true" ValidationExpression="^[a-zA-Z0-9][-\w\.\+]*@([a-zA-Z0-9][\w\-]*\.)+[a-zA-Z]{2,4}$"></asp:RegularExpressionValidator></td>
                                                                     </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right">
                                                                    <font class="subTitleSmall">E-Mail Body:</font>
                                                                </td>
                                                                <td align="left" valign="middle">
                                                                    <asp:TextBox ID="txtEmailBody" runat="server" TextMode="MultiLine" CssClass="multiLong"></asp:TextBox>
                                                                    <img src="images/info.gif" border="0" onmouseout="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>The message of the E-Mail</font>', 300)" alt="" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>&nbsp;</td>
                                                    <td align="left" style="padding-top: 10px;">
                                                        <asp:Button ValidationGroup="main" ID="btnSubmit" runat="Server" CssClass="normalButton" Text="Submit" OnClick="btnSubmit_Click" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:ValidationSummary ValidationGroup="main" ID="validationSummary" runat="server" EnableClientScript="true" ShowMessageBox="true" ShowSummary="false" Enabled="true" />
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
    <asp:Literal ID="ltScript" runat="server"></asp:Literal>
</body>
</html>
