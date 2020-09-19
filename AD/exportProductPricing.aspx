<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.exportProductPricing" CodeFile="exportProductPricing.aspx.cs" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Export Product Pricing</title>
    <script type="text/javascript"  type="text/javascript" src="jscripts/toolTip.js">
        /***********************************************
        * Cool DHTML tooltip script II- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
        * This notice MUST stay intact for legal use
        * Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
        ***********************************************/
    </script>
</head>

<body>
    <form id="frmExportProductPricing" runat="server" enctype="multipart/form-data" method="post">   
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
                                        Export Product Pricing to Excel/XML
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
                                    <font class="subTitle">Export Prices:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="100%">
                                    <div class="wrapper">
                                        <div id="divMain" runat="server">
                                            <span class="subTitleSmall">Filter by Category:</span>
                                            <br /><asp:DropDownList ID="ddCategory" runat="server"></asp:DropDownList>
                                            <br />
                                            <span class="subTitleSmall">Filter by Section:</span>
                                            <br /><asp:DropDownList ID="ddSection" runat="server"></asp:DropDownList>
                                            <br />
                                            <span class="subTitleSmall">Filter by Manufacturer:</span>
                                            <br /><asp:DropDownList ID="ddManufacturer" runat="server"></asp:DropDownList>
                                            <div id="divDistributor" runat="server">
                                                <span class="subTitleSmall">Filter by Distributor:</span>
                                                <br /><asp:DropDownList ID="ddDistributor" runat="server"></asp:DropDownList>
                                            </div>
                                            <div id="divGenre" runat="server">
                                                <span class="subTitleSmall">Filter by Genre:</span>
                                                <br /><asp:DropDownList ID="ddGenre" runat="server"></asp:DropDownList>
                                            </div>
                                            <div id="divVector" runat="server">
                                                <span class="subTitleSmall">Filter by Vector:</span>
                                                <br /><asp:DropDownList ID="ddVector" runat="server"></asp:DropDownList>
                                            </div>
                                            <br />
                                            <span class="subTitleSmall">Select Export Format:</span>
                                            <br />
                                            <asp:RadioButtonList ID="rblExport" runat="server">
                                                <asp:ListItem Value="xls" Text="Excel (xls)" Selected="true"></asp:ListItem>
                                                <asp:ListItem Value="xml" Text="XML"></asp:ListItem>
                                                <asp:ListItem Value="csv" Text="Comma Delimited (csv)"></asp:ListItem>
                                            </asp:RadioButtonList>
                                            <br />
                                            <br />
                                            <asp:Button ID="btnUpload" runat="server" CssClass="normalButton" Text="Submit" OnClick="btnUpload_Click" />
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
