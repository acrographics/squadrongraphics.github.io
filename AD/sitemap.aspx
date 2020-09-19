<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.sitemap" CodeFile="sitemap.aspx.cs" %>
<html>
<head>
<title>Site Map</title>
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
                                            Site Map
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
        </div>
    </div>
    <div id="content">
                <asp:Literal ID="SiteContents" runat="server"></asp:Literal>
    </div>
    </form>
</body>
</html>