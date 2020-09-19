<%@ Page Language="C#" AutoEventWireup="true" CodeFile="entityBody.aspx.cs" Inherits="entityBody" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Entity Body</title>
    <asp:Literal runat="server" ID="ltStyles"></asp:Literal>
</head>
<body>
    <form id="frmEntityBody" runat="server">
    <div style="padding-top: 100px; text-align: center; width: 100%;">
        <div id="help" style="text-align: center; width: 100%;">
            <table border="0" cellpadding="1" cellspacing="0" class="outerTable">
                <tr>
                    <td align="center">
                        <div class="wrapper">
                            <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                                <tr>                            
                                    <td class="titleTable">
                                        <font class="subTitle">Please start by selecting the Entity from the Menu on LEFT.</font>
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
    
    </form>
    <asp:Literal ID="ltScript" runat="server"></asp:Literal>
</body>
</html>