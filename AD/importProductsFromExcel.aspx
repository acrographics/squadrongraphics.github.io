<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.importProductsFromExcel" CodeFile="importProductsFromExcel.aspx.cs" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Import Products</title>
    <script type="text/javascript"  src="jscripts/toolTip.js">
        /***********************************************
        * Cool DHTML tooltip script II- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
        * This notice MUST stay intact for legal use
        * Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
        ***********************************************/
    </script>
</head>

<body>
    <form id="frmImport" runat="server" enctype="multipart/form-data" method="post">   
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
                                        Import Products from Excel
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
        <p><big><b><font color="red">BACKUP YOUR DATABASE BEFORE DOING ANY IMPORTS!</font></b></big><br />
        There will be an undo option available after the import file is loaded, but <b>do not count on it</b>. Backup your site database beforehand. Contact your hosting company if they need to assist you. It is also help to have successfully tested your import file on a development/local web site before you apply it to the production site!</p>                    
        <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
            <tr>
                <td>
                    <div class="wrapper">                       
                        <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                            <tr>
                                <td class="titleTable">
                                    <font class="subTitle">Import Products from Excel:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="100%">
                                    <div class="wrapper">
                                        <div id="divMain" runat="server">
                                            Select the local Excel file that you want to upload. This file must conform to our Excel Product Import File Format Specifications defined in the manual!
                                            <br />
                                            This file should be on your own computer. 
                                            <br />
                                            Click 'browse' to select the file on your computer:
                                            <asp:FileUpload ID="fuFile" runat="server" CssClass="fileUpload"  />
                                            <br /><br />
                                            <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" CssClass="normalButton" />
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="wrapper" runat="server" id="divReview">
                        <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                            <tr>
                                <td class="titleTable">
                                    <font class="subTitle">Review Upload:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="100%">
                                    <div class="wrapper">
                                        <b>Please review the log status shown below, and then test your store web site, to double check that the import worked properly</b>
                                        <br /><br />
                                        <asp:Button ID="btnAccept" runat="server" Text="CLICK HERE TO ACCEPT IMPORT" CssClass="normalButton" OnClick="btnAccept_Click" />
                                        &nbsp;
                                        <asp:Button ID="btnUndo" runat="server" Text="CLICK HERE TO UNDO THE IMPORT" CssClass="normalButton" OnClick="btnUndo_Click" />
                                        <hr noshade="noshade" />
                                        <asp:Literal ID="ltResults" runat="server"></asp:Literal>
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
