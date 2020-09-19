<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.storewide" CodeFile="storewide.aspx.cs" Theme="Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Store-Wide Maintenance</title>
    <script type="text/javascript"  src="jscripts/toolTip.js">
        /***********************************************
        * Cool DHTML tooltip script II- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
        * This notice MUST stay intact for legal use
        * Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
        ***********************************************/
    </script>
</head>

<body>
    <form id="frmStoreWide" runat="server">   
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
                                        Store-Wide Maintenance
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
                                    <font class="subTitle">Maintenance:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="100%">
                                    <div class="wrapper">
                                        This page helps you perform recommended store-wide maintenance.
                                        <br /><br />
                                        <div id="divMain" runat="server">
                                            <table cellpadding="5" cellspacing="0" border="0" width="100%">
                                                <tr>
                                                    <td align="right" valign="top" style="border-bottom: dashed 1px #666666;">
                                                        <font class="subTitleSmall">
                                                            Set 'On Sale' Prompt:
                                                        </font>
                                                    </td>
                                                    <td align="left" style="border-bottom: dashed 1px #666666;">
                                                        <asp:DropDownList ID="ddOnSale" runat="server" CssClass="default"></asp:DropDownList>
                                                        <asp:RequiredFieldValidator InitialValue="0" ValidationGroup="group1" ErrorMessage="!!" ControlToValidate="ddOnSale" Display="dynamic" SetFocusOnError="true" runat="server" ID="RequiredFieldValidator"></asp:RequiredFieldValidator>
                                                        <br />
                                                        For Category: <asp:DropDownList ID="ddOnSaleCat" runat="server" CssClass="default"></asp:DropDownList>
                                                        <br />
                                                        For Department: <asp:DropDownList ID="ddOnSaleDep" runat="server" CssClass="default"></asp:DropDownList>
                                                        <br />
                                                        For Manufacturer: <asp:DropDownList ID="ddOnSaleManu" runat="server" CssClass="default"></asp:DropDownList>
                                                        <%--<img onmouseover="ddrivetip('<font class=\'exampleText\'>Since the storefront can log all SQL statements done by admin users, this can help keep the sql log table size to a minimum. The SQL Log records are not needed by the storefront site to operate. They are simply an audit capability if required.</font>', 300)" onmouseout="hideddrivetip()" src="images/info.gif" alt="" border="0" />--%>
                                                        <br />
                                                        <asp:Button ID="btnSubmit1" ValidationGroup="group1" CssClass="normalButton" runat="server" Text="Submit" OnClick="btnSubmit1_Click" />
                                                    </td>
                                                </tr>                                                   
                                                <%--<tr>
                                                    <td colspan="2" style="height: 10px;"></td>
                                                </tr>--%>
                                                <tr>
                                                    <td align="right" valign="top" style="border-bottom: dashed 1px #666666;">
                                                        <font class="subTitleSmall">
                                                            Set Spec Title For All Products:
                                                        </font>
                                                    </td>
                                                    <td align="left" style="border-bottom: dashed 1px #666666;">
                                                        <asp:TextBox ID="txtSpec" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ValidationGroup="group2" ErrorMessage="!!" ControlToValidate="txtSpec" Display="dynamic" SetFocusOnError="true" runat="server" ID="RequiredFieldValidator1"></asp:RequiredFieldValidator>
                                                        <%--<img onmouseover="ddrivetip('<font class=\'exampleText\'>Since the storefront can log all SQL statements done by admin users, this can help keep the sql log table size to a minimum. The SQL Log records are not needed by the storefront site to operate. They are simply an audit capability if required.</font>', 300)" onmouseout="hideddrivetip()" src="images/info.gif" alt="" border="0" />--%>
                                                        <br />
                                                        <asp:Button ID="btnSubmit2" ValidationGroup="group2" CssClass="normalButton" runat="server" Text="Submit" OnClick="btnSubmit2_Click" />
                                                    </td>
                                                </tr>                                                    
                                                <%--<tr>
                                                    <td colspan="2" style="height: 10px;"></td>
                                                </tr>--%> 
                                                <tr>
                                                    <td align="right" valign="top" style="border-bottom: dashed 1px #666666;">
                                                        <font class="subTitleSmall">
                                                            Set Specs Inline Flag For All Products:
                                                        </font>
                                                    </td>
                                                    <td align="left" style="border-bottom: dashed 1px #666666;">
                                                        <asp:RadioButtonList ID="rblSpecsInline" runat="server">
                                                            <asp:ListItem Value="0">No</asp:ListItem>
                                                            <asp:ListItem Value="1">Yes</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                        <%--<img onmouseover="ddrivetip('<font class=\'exampleText\'>Since the storefront can log all SQL statements done by admin users, this can help keep the sql log table size to a minimum. The SQL Log records are not needed by the storefront site to operate. They are simply an audit capability if required.</font>', 300)" onmouseout="hideddrivetip()" src="images/info.gif" alt="" border="0" />--%>
                                                        <br />
                                                        <asp:Button ID="btnSubmit3" ValidationGroup="group3" CssClass="normalButton" runat="server" Text="Submit" OnClick="btnSubmit3_Click" />
                                                    </td>
                                                </tr>                                                    
                                                <%--<tr>
                                                    <td colspan="2" style="height: 10px;"></td>
                                                </tr>--%> 
                                                <tr>
                                                    <td align="right" valign="top" style="border-bottom: dashed 1px #666666;">
                                                        <font class="subTitleSmall">
                                                            Set Quantity Discount Table to be<br />used for ALL Products & Variants:
                                                        </font>
                                                    </td>
                                                    <td align="left" style="border-bottom: dashed 1px #666666;">
                                                        <asp:DropDownList ID="ddDiscountTable" runat="server"></asp:DropDownList>
                                                        <%--<img onmouseover="ddrivetip('<font class=\'exampleText\'>Since the storefront can log all SQL statements done by admin users, this can help keep the sql log table size to a minimum. The SQL Log records are not needed by the storefront site to operate. They are simply an audit capability if required.</font>', 300)" onmouseout="hideddrivetip()" src="images/info.gif" alt="" border="0" />--%>
                                                        <br />
                                                        <asp:Button ID="btnSubmit4" ValidationGroup="group4" CssClass="normalButton" runat="server" Text="Submit" OnClick="btnSubmit4_Click" />
                                                    </td>
                                                </tr>                                                     
                                               <%-- <tr>
                                                    <td colspan="2" style="height: 10px;"></td>
                                                </tr>--%>
                                                <tr>
                                                    <td align="right" valign="top" style="border-bottom: dashed 1px #666666;">
                                                        <font class="subTitleSmall">
                                                            Set Sales Discount Percentage:
                                                        </font>
                                                    </td>
                                                    <td align="left" style="border-bottom: dashed 1px #666666;">
                                                        <asp:TextBox CssClass="singleShort" runat="server" ID="txtDiscountPercent"></asp:TextBox>
                                                        <asp:RequiredFieldValidator InitialValue="0" ValidationGroup="group5" ErrorMessage="!!" ControlToValidate="txtDiscountPercent" Display="dynamic" SetFocusOnError="true" runat="server" ID="RequiredFieldValidator2"></asp:RequiredFieldValidator>
                                                        <br />
                                                        For Category: <asp:DropDownList ID="ddDiscountCate" runat="server" CssClass="default"></asp:DropDownList>
                                                        <br />
                                                        For Department: <asp:DropDownList ID="ddDiscountDep" runat="server" CssClass="default"></asp:DropDownList>
                                                        <br />
                                                        For Manufacturer: <asp:DropDownList ID="ddDiscountManu" runat="server" CssClass="default"></asp:DropDownList>
                                                        <%--<img onmouseover="ddrivetip('<font class=\'exampleText\'>Since the storefront can log all SQL statements done by admin users, this can help keep the sql log table size to a minimum. The SQL Log records are not needed by the storefront site to operate. They are simply an audit capability if required.</font>', 300)" onmouseout="hideddrivetip()" src="images/info.gif" alt="" border="0" />--%>
                                                        <br />
                                                        <asp:Button ID="btnSubmit5" ValidationGroup="group5" CssClass="normalButton" runat="server" Text="Submit" OnClick="btnSubmit5_Click" />
                                                    </td>
                                                </tr>                                                    
                                                <%--<tr>
                                                    <td colspan="2" style="height: 10px;"></td>
                                                </tr>--%>
                                                <tr>
                                                    <td align="right" valign="top" style="border-bottom: dashed 1px #666666;">
                                                        <font class="subTitleSmall">
                                                            Reset All Default Variants:
                                                        </font>
                                                    </td>
                                                    <td align="left" style="border-bottom: dashed 1px #666666;">
                                                        This cannot be undone!
                                                        <br />
                                                        Resets the default variant for each product to the first one by DisplayOrder,Name
                                                        <br />
                                                        <asp:Button ID="btnSubmit6" ValidationGroup="group6" CssClass="normalButton" runat="server" Text="Submit" OnClick="btnSubmit6_Click" />
                                                    </td>
                                                </tr>                                                
                                                <%--<tr>
                                                    <td colspan="2" style="height: 10px;"></td>
                                                </tr>--%>
                                                <tr>
                                                    <td align="right" valign="top">
                                                        <font class="subTitleSmall">
                                                            Reset All product SENames:
                                                        </font>
                                                    </td>
                                                    <td align="left" >
                                                        This cannot be undone!
                                                        <br />
                                                        Sets the SEName field in the product table for ALL products, this may take a long time if you have a lot of products.
                                                        <br />
                                                        <asp:Button ID="btnSubmit7" ValidationGroup="group7" CssClass="normalButton" runat="server" Text="Submit" OnClick="btnSubmit7_Click" />
                                                    </td>
                                                </tr>                                                
                                            </table>
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
