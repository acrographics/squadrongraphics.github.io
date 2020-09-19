<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.entityeditkit" CodeFile="entityeditkit.aspx.cs" Theme="Default" %>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Edit Kit</title>
        <link href="../App_Themes/Default/StyleSheet.css" type="text/css" rel="stylesheet" />
    </head>

    <body>
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
                                        <font class="subTitle">Kit Product View:</font>
                                    </td>
                                    <td class="contentTable">
                                        <font class="title">
                                            Editing Kit : <asp:Literal ID="ltProduct" runat="server"></asp:Literal>
                                        </font>
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
                                        <font class="subTitle">Kit Product:</font>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="contentTable" valign="top" width="100%">
                                        <asp:Literal ID="ltContent" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </body>
</html>