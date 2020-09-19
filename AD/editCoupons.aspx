<%@ Page Language="C#" AutoEventWireup="true" CodeFile="editCoupons.aspx.cs" Inherits="editCoupons" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Coupons</title>
    <asp:Literal runat="server" ID="ltStyles"></asp:Literal>
    <script type="text/javascript"  src="jscripts/toolTip.js">
        /***********************************************
        * Cool DHTML tooltip script II- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
        * This notice MUST stay intact for legal use
        * Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
        ***********************************************/
    </script>
</head>
<body>
    <form id="frmCoupon" runat="server">
    <asp:Literal ID="ltScript1" runat="server"></asp:Literal>
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
                                    <a href="coupons.aspx">
                                        <font class="title">
                                            Manage Coupons
                                        </font>
                                    </a>
                                    ->
                                    <font class="title">
                                        Editing Coupons:
                                        <span style="color: Red;">
                                            <asp:Literal id="ltCoupon" runat="server"></asp:Literal>
                                        </span>
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
                                    <font class="subTitle">Coupon Details:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="*">
                                    <div class="wrapperLeft">                                        
                                        <div style="margin-top: 5px; margin-bottom: 15px;">
                                            Fields marked with an asterisk (*) are required. All other fields are optional.
                                        </div>
                                        <table width="100%" cellpadding="1" cellspacing="0" border="0">
                                            <tr>
                                                <td width="260" align="right" valign="middle">
                                                    <font class="subTitleSmall">*Coupon Code:<br />(a-z, A-Z, space, and 0-9 only!)</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtCode" runat="server"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ErrorMessage="Please Enter The Coupon Code" ControlToValidate="txtCode" ID="RequiredFieldValidator" SetFocusOnError="true" runat="server">!!</asp:RequiredFieldValidator> 
                                                    (Has Been Used <asp:Literal ID="ltNumUses" runat="server"></asp:Literal> Times)
                                                    <br />
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtCode" ErrorMessage="The Coupon Code can only contain letters, digits, and space character" SetFocusOnError="True" ValidationExpression="^[A-Za-z0-9\s\-]*$"></asp:RegularExpressionValidator></td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">*Coupon Type:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:DropDownList ID="ddType" runat="server">
                                                        <asp:ListItem Value="0">Order - this coupon ONLY applies to the order subtotal</asp:ListItem>
                                                        <asp:ListItem Value="1">Product - this coupon ONLY applies to the specified product(s)</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="top">
                                                    <font class="subTitleSmall">*Expiration Date:</font>
                                                </td>
                                                <td align="left" valign="top">
                                                    <asp:TextBox ID="txtDate" runat="server"></asp:TextBox>
                                                    <asp:Literal ID="ltDate" runat="server"></asp:Literal>
                                                    <asp:RequiredFieldValidator ErrorMessage="Fill in Expiration Date" ControlToValidate="txtDate" ID="RequiredFieldValidator1" SetFocusOnError="true" runat="server">!!</asp:RequiredFieldValidator> 
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="top">
                                                    <font class="subTitleSmall">Description:</font>
                                                </td>
                                                <td align="left" valign="top">
                                                    <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" CssClass="multiLong"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">*Discount Percent:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtDiscountPercent" runat="server">0.0</asp:TextBox>
                                                    <asp:RequiredFieldValidator ErrorMessage="Fill in Discount Percent" ControlToValidate="txtDiscountPercent" ID="RequiredFieldValidator2" SetFocusOnError="true" runat="server">!!</asp:RequiredFieldValidator> 
                                                    <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter 0, or a percentage like 5 or 7.5</font>', 300)" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">*Discount Amount:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtDiscountAmount" runat="server">0.0</asp:TextBox>
                                                    <asp:RequiredFieldValidator ErrorMessage="Fill in Discount Amount" ControlToValidate="txtDiscountAmount" ID="RequiredFieldValidator3" SetFocusOnError="true" runat="server">!!</asp:RequiredFieldValidator> 
                                                    <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter 0, or a dollar amount like 2.50 or 10.00</font>', 300)" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">*Discount Includes Free Shipping:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:RadioButtonList ID="rbShipping" runat="server">
                                                        <asp:ListItem Value="0" Selected="true">No</asp:ListItem>
                                                        <asp:ListItem Value="1">Yes</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">*Expires On First Usage By Any Customer:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:RadioButtonList ID="rbFirstUsage" runat="server">
                                                        <asp:ListItem Value="0" Selected="true">No</asp:ListItem>
                                                        <asp:ListItem Value="1">Yes</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">*Expires After One Usage By Each Customer:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:RadioButtonList ID="rbOneUsage" runat="server">
                                                        <asp:ListItem Value="0" Selected="true">No</asp:ListItem>
                                                        <asp:ListItem Value="1">Yes</asp:ListItem>                                                            
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Expires After N Uses:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtNUses" runat="server">0</asp:TextBox>
                                                    <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter the # of times this coupon may be used by any/all customers, or 0 if unrestricted</font>', 300)" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Requires Minimum Order Amount:</font>
                                                </td>
                                                <td align="left" valign="top">
                                                    <asp:TextBox ID="txtMinOrderAmount" runat="server">0</asp:TextBox>
                                                    <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>If the coupon can only be used on orders that exceed a certain amount, enter that amount. Otherwise, leave this field blank or enter 0.0</font>', 300)" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Valid For Customer(s):</font>
                                                </td>
                                                <td align="left" valign="top">
                                                    <asp:TextBox ID="txtCustomers" runat="server"></asp:TextBox>
                                                    <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter the customer id(s) for which this coupon is valid, or leave blank to allow any customer to use it. Enter customer id\'s separated by a comma, e.g. 12343, 12344, 12345, etc...</font>', 300)" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Valid For Product(s):</font>
                                                </td>
                                                <td align="left" valign="top">
                                                    <asp:TextBox ID="txtProducts" runat="server"></asp:TextBox>
                                                    <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter the product id(s) for which this coupon is valid, or leave blank to allow it to work on any product. Enter product id\'s separated by a comma, e.g. 40, 41, 42, etc...</font>', 300)" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Valid For Category(s):</font>
                                                </td>
                                                <td align="left" valign="top">
                                                    <asp:TextBox ID="txtCategory" runat="server"></asp:TextBox>
                                                    <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter the category id(s) for which this coupon is valid, or leave blank to allow it to work on any category. Enter category id\'s separated by a comma, e.g. 1,2,3, etc...</font>', 300)" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Valid For Section(s):</font>
                                                </td>
                                                <td align="left" valign="top">
                                                    <asp:TextBox ID="txtSection" runat="server"></asp:TextBox>
                                                    <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter the section id(s) for which this coupon is valid, or leave blank to allow it to work on any section. Enter section id\'s separated by a comma, e.g. 1,2,3, etc...</font>', 300)" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Valid For Manufacturer(s):</font>
                                                </td>
                                                <td align="left" valign="top">
                                                    <asp:TextBox ID="txtManufacturer" runat="server"></asp:TextBox>
                                                    <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter the manufacturer id(s) for which this coupon is valid, or leave blank to allow it to work on any manufacturer. Enter manufacturer id\'s separated by a comma, e.g. 1,2,3, etc...</font>', 300)" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:ValidationSummary ID="validationSummary" runat="server" EnableClientScript="true" ShowMessageBox="true" ShowSummary="false" Enabled="true" />
                                                </td>
                                            </tr>
                                        </table>
                                        <div style="width: 100%; text-align: center;">
                                            &nbsp;&nbsp;<asp:Button ID="btnSubmit" runat="server" CssClass="normalButton" OnClick="btnSubmit_Click" />
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
