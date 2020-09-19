<%@ Page Language="C#" AutoEventWireup="true" CodeFile="orderReports.aspx.cs" Inherits="AspDotNetStorefrontAdmin.orderReports" Theme="default" %>
<%@ Register TagPrefix="ew" Namespace="eWorld.UI" Assembly="eWorld.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Order Reports</title>
    <asp:Literal runat="server" id="ltStyles"></asp:Literal>
    <script type="text/javascript"  src="jscripts/contractableHeaders.js" type="text/javascript">
        /***********************************************
        * Contractible Headers script- © Dynamic Drive (www.dynamicdrive.com)
        * This notice must stay intact for legal use. Last updated Mar 23rd, 2004.
        * Visit http://www.dynamicdrive.com/ for full source code
        ***********************************************/
    </script>
</head>
<body>
    <form id="frmOrderReports" runat="server">
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
                                        Order Reports
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
                                <td class="titleTable" width="300">
                                    <font class="subTitle">Specifications:</font>
                                </td>
                                <td style="width: 5px;" />
                                <td style="width: 1px; background-color: #EBEBEB;" />
                                <td style="width: 5px;" />
                                <td class="titleTable">
                                    <font class="subTitle">Order Report:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="300">
                                    <div class="wrapperTopBottom">
                                       <!-- Report Controls -->
                                       <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                            <tr>
                                                <td colspan="2" valign="top">
                                                    <div class="wrapperBottom">
                                                        <font class="titleMessage">Select Dates</font>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Start Date:</font>
                                                </td>
                                                <td align="left" valign="top">
                                                    <ew:calendarpopup id="dateStart" runat="server" Height="15px" DisableTextboxEntry="False" AllowArbitraryText="False"
								                        padsingledigits="True" nullable="True" calendarwidth="200" Width="80px" showgototoday="True" imageurl="skins\Skin_1\images\calendar.gif"
								                        Font-Size="9px" ButtonStyle-Height="20px">
								                        <weekdaystyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Black"
									                        backcolor="Gainsboro"></weekdaystyle>
								                        <monthheaderstyle font-size="Small" font-names="Verdana,Helvetica,Tahoma,Arial" font-bold="True" forecolor="White"
									                        backcolor="Gray"></monthheaderstyle>
								                        <offmonthstyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Gainsboro"
									                        backcolor="Gainsboro"></offmonthstyle>
								                        <gototodaystyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Black"
									                        backcolor="White"></gototodaystyle>
								                        <todaydaystyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="MediumBlue"
									                        backcolor="Gainsboro"></todaydaystyle>
								                        <dayheaderstyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Black"
									                        backcolor="Azure"></dayheaderstyle>
								                        <weekendstyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Red"
									                        backcolor="Gainsboro"></weekendstyle>
								                        <selecteddatestyle borderstyle="Inset" font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial"
									                        borderwidth="2px" forecolor="MediumBlue" backcolor="Silver"></selecteddatestyle>
								                        <cleardatestyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Black"
									                        backcolor="White"></cleardatestyle>
							                        </ew:calendarpopup>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">End Date:</font>
                                                </td>
                                                <td align="left" valign="top">
                                                    <ew:calendarpopup id="dateEnd" runat="server" Height="15px" DisableTextboxEntry="False" AllowArbitraryText="False"
								                        padsingledigits="True" nullable="True"  calendarwidth="200" Width="80px" showgototoday="True" imageurl="skins\Skin_1\images\calendar.gif"
								                        Font-Size="9px" ButtonStyle-Height="20px">
								                        <weekdaystyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Black"
									                        backcolor="Gainsboro"></weekdaystyle>
								                        <monthheaderstyle font-size="Small" font-names="Verdana,Helvetica,Tahoma,Arial" font-bold="True" forecolor="White"
									                        backcolor="Gray"></monthheaderstyle>
								                        <offmonthstyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Gainsboro"
									                        backcolor="Gainsboro"></offmonthstyle>
								                        <gototodaystyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Black"
									                        backcolor="White"></gototodaystyle>
								                        <todaydaystyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="MediumBlue"
									                        backcolor="Gainsboro"></todaydaystyle>
								                        <dayheaderstyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Black"
									                        backcolor="Azure"></dayheaderstyle>
								                        <weekendstyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Red"
									                        backcolor="Gainsboro"></weekendstyle>
								                        <selecteddatestyle borderstyle="Inset" font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial"
									                        borderwidth="2px" forecolor="MediumBlue" backcolor="Silver"></selecteddatestyle>
								                        <cleardatestyle font-size="XX-Small" font-names="Verdana,Helvetica,Tahoma,Arial" forecolor="Black"
									                        backcolor="White"></cleardatestyle>
							                        </ew:calendarpopup>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" valign="top">
                                                    <div class="wrapperTopBottom">
                                                        <font class="titleMessage">OR Select Range</font>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">&nbsp;</font>
                                                </td>
                                                <td align="left" valign="top">
                                                    <asp:RadioButtonList runat="server" ID="rblRange" CellPadding="0" CellSpacing="0" RepeatColumns="1">
                                                        <asp:ListItem Value="0" Selected="true">Use Dates above</asp:ListItem>
                                                        <asp:ListItem Value="1">Today</asp:ListItem>
                                                        <asp:ListItem Value="2">Yesterday</asp:ListItem>
                                                        <asp:ListItem Value="3">This Week</asp:ListItem>
                                                        <asp:ListItem Value="4">Last Week</asp:ListItem>
                                                        <asp:ListItem Value="5">This Month</asp:ListItem>
                                                        <asp:ListItem Value="6">Last Month</asp:ListItem>
                                                        <asp:ListItem Value="7">This Year</asp:ListItem>
                                                        <asp:ListItem Value="8">Last Year</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" valign="top">
                                                    <hr noshade="noshade" width="100%" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" valign="top">
                                                    <div class="wrapperBottom">
                                                        <font class="titleMessage">Select Category</font>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Category:</font>
                                                </td>
                                                <td align="left" valign="top">
                                                    <asp:DropDownList ID="ddCategory" runat="Server"></asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" valign="top">
                                                    <div class="wrapperTopBottom">
                                                        <font class="titleMessage">OR Select Department</font>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Section:</font>
                                                </td>
                                                <td align="left" valign="top">
                                                    <asp:DropDownList ID="ddSection" runat="Server"></asp:DropDownList>
                                                </td>
                                            <tr>
                                                <td colspan="2" valign="top">
                                                    <div class="wrapperTopBottom">
                                                        <font class="titleMessage">OR Select Manufacturer</font>
                                                    </div>
                                                </td>
                                            </tr>
                                            </tr>
                                              <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Manufacturer:</font>
                                                </td>
                                                <td align="left" valign="top">
                                                    <asp:DropDownList ID="ddManufacturer" runat="Server" AutoPostBack="True" OnSelectedIndexChanged="ddManufacturer_SelectedIndexChanged"></asp:DropDownList>
                                                </td>
                                            </tr>
<%--                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Product Type:</font>
                                                </td>
                                                <td align="left" valign="top">
                                                    <asp:DropDownList ID="ddType" runat="Server" AutoPostBack="True" OnSelectedIndexChanged="ddType_SelectedIndexChanged"></asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" valign="top">
                                                    <hr noshade="noshade" width="100%" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" valign="top">
                                                    <div class="wrapperBottom">
                                                        <font class="titleMessage">Select Product</font>
                                                    </div>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Product:</font>
                                                </td>
                                                <td align="left" valign="top">
                                                    <asp:DropDownList ID="ddProduct" runat="Server"></asp:DropDownList>
                                                </td>
                                            </tr>
--%>                                       </table>
                                    </div>
                                    <div class="wrapperTop">
                                        <asp:Button runat="server" ID="btnReport" CssClass="normalButton" Text="GET REPORT" OnClick="btnReport_Click" />                                   
                                    </div>
                                </td>
                                <td style="width: 5px;" />
                                <td style="width: 1px; background-color: #EBEBEB;" />
                                <td style="width: 5px;" />
                                <td class="contentTable" valign="top" width="*">
                                    <div class="wrapperLeft">
                                        <asp:PlaceHolder ID="phMain" runat="server">
                                            <font class="titleMessage">
                                                Report matching: <asp:Literal runat="server" ID="ltMode"></asp:Literal><br />
                                                (Dollar Figures Approximate! They do not include order level discounts & coupons!)  
                                            </font>
                                            <p>
                                                <asp:Literal ID="ltReport" runat="server"></asp:Literal>
                                            </p>
                                        </asp:PlaceHolder>
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
