<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.monthlymaintenance" CodeFile="monthlymaintenance.aspx.cs" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Monthly Maintenance</title>
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
                                        Monthly Maintenance
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
                                        This page helps you perform recommended monthly maintenance on your storefront database. You should run this late at night, probably on weekends, when site activity is minimal, as the store database could be locked for several minutes during these operations. It is recommended that you do this maintenance monthly.
                                        <br /><br />
                                        <div id="divMain" runat="server">
                                            <table cellpadding="1" cellspacing="0" border="0">
                                                <tr>
                                                    <td align="right" style="width: 300px">
                                                        <font class="subTitleSmall">
                                                            Clear All Shopping Carts:
                                                        </font>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="ClearAllShoppingCarts" CssClass="default" runat="server">
                                                            <asp:ListItem Value="0">Leave Unchanged</asp:ListItem>
                                                            <asp:ListItem Value="-1">Clear All Regardless of Date</asp:ListItem>
                                                            <asp:ListItem Selected="True" Value="30">&gt;30 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="60">&gt;60 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="90">&gt;90 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="120">&gt;120 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="150">&gt;150 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="180">&gt;180 Days Old</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <img onmouseover="ddrivetip('<font class=\'exampleText\'>If activated, all shopping cart items will be cleared that are older than the # of days specified (wish lists, and gift registries, and recurring orders not affected by this checkbox). If you wish to set the aging # days for all shopping carts, See AppConfig:AgeCartDays storewide parameter.</font>', 300)" onmouseout="hideddrivetip()" src="images/info.gif" alt="" border="0" />
                                                    </td>
                                                </tr> 
                                                <tr>
                                                    <td align="right" style="width: 300px">
                                                        <font class="subTitleSmall">
                                                            Clear All Wish Lists: 
                                                        </font>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="ClearAllWishLists" CssClass="default" runat="server">
                                                            <asp:ListItem Value="0">Leave Unchanged</asp:ListItem>
                                                            <asp:ListItem Value="-1">Clear All Regardless of Date</asp:ListItem>
                                                            <asp:ListItem Value="30">&gt;30 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="60">&gt;60 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="90">&gt;90 Days Old</asp:ListItem>
                                                            <asp:ListItem Selected="True" Value="120">&gt;120 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="150">&gt;150 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="180">&gt;180 Days Old</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <img onmouseover="ddrivetip('<font class=\'exampleText\'>If activated, all wishlist items will be cleared that are older than the # of days specified.</font>', 300)" onmouseout="hideddrivetip()" src="images/info.gif" alt="" border="0" />
                                                    </td>
                                                </tr> 
                                                <tr>
                                                    <td align="right" style="width: 300px">
                                                        <font class="subTitleSmall">
                                                            Clear All Gift Registries: 
                                                        </font>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="ClearAllGiftRegistries" CssClass="default" runat="server">
                                                            <asp:ListItem Value="0">Leave Unchanged</asp:ListItem>
                                                            <asp:ListItem Value="-1">Clear All Regardless of Date</asp:ListItem>
                                                            <asp:ListItem Value="30">&gt;30 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="60">&gt;60 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="90">&gt;90 Days Old</asp:ListItem>
                                                            <asp:ListItem Selected="True" Value="120">&gt;120 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="150">&gt;150 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="180">&gt;180 Days Old</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <img onmouseover="ddrivetip('<font class=\'exampleText\'>If activated, all gift registry items will be cleared that are older than the # of days specified.</font>', 300)" onmouseout="hideddrivetip()" src="images/info.gif" alt="" border="0" />
                                                    </td>
                                                </tr> 
                                                <tr>
                                                    <td align="right" style="width: 300px">
                                                        <font class="subTitleSmall">
                                                            Erase Passwords from Order records:
                                                        </font>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="EraseOrderPasswords" CssClass="default" runat="server">
                                                            <asp:ListItem Value="0">Leave Unchanged</asp:ListItem>
                                                            <asp:ListItem Value="-1">Clear All Regardless of Date</asp:ListItem>
                                                            <asp:ListItem Selected="True" Value="30">&gt;30 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="60">&gt;60 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="90">&gt;90 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="120">&gt;120 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="150">&gt;150 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="180">&gt;180 Days Old</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <img onmouseover="ddrivetip('<font class=\'exampleText\'>Since order records contain a complete snapshot of all customer data, this option allows you to remove the customer passwords (which are encrypted) from the order records which are older than the # of days specified. The passwords are almost never needed after an order has been successfully processed and shipped. This does NOT affect the customer\'s main account record(s), and their password remains in those account record(s) in an encrypted state.</font>', 300)" onmouseout="hideddrivetip()" src="images/info.gif" alt="" border="0" />
                                                    </td>
                                                </tr> 
                                                <tr>
                                                    <td align="right" style="width: 300px">
                                                        <font class="subTitleSmall">
                                                            Erase Credit Cards from Order records:
                                                        </font>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="EraseOrderCreditCards" CssClass="default" runat="server">
                                                            <asp:ListItem Value="0">Leave Unchanged</asp:ListItem>
                                                            <asp:ListItem Value="-1">Clear All Regardless of Date</asp:ListItem>
                                                            <asp:ListItem Selected="True" Value="30">&gt;30 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="60">&gt;60 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="90">&gt;90 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="120">&gt;120 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="150">&gt;150 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="180">&gt;180 Days Old</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <img onmouseover="ddrivetip('<font class=\'exampleText\'>Since order records contain a complete snapshot of all customer data, this option allows you to remove the customer credit card information (which are encrypted) from the order records which are older than the # of days specified. The credit card information is almost never needed after an order has been successfully processed and shipped and then nothing has been heard back from the customer for 30 days. This does NOT affect the customer\'s main account record, and their credit card information remains in that account record, intact, in an encrypted state (if you have your storefront set to store credit card info by setting AppConfig:StoreCCInDB=true).</font>', 300)" onmouseout="hideddrivetip()" src="images/info.gif" alt="" border="0" />                                                    </td>
                                                </tr> 
                                                <tr>
                                                    <td align="right" style="width: 300px">
                                                        <font class="subTitleSmall">
                                                            Erase SQL Log:
                                                        </font>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="EraseSQLLog" CssClass="default" runat="server">
                                                            <asp:ListItem Value="0">Leave Unchanged</asp:ListItem>
                                                            <asp:ListItem Value="-1">Clear All Regardless of Date</asp:ListItem>
                                                            <asp:ListItem Selected="True" Value="30">&gt;30 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="60">&gt;60 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="90">&gt;90 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="120">&gt;120 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="150">&gt;150 Days Old</asp:ListItem>
                                                            <asp:ListItem Value="180">&gt;180 Days Old</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <img onmouseover="ddrivetip('<font class=\'exampleText\'>Since the storefront can log all SQL statements done by admin users, this can help keep the sql log table size to a minimum. The SQL Log records are not needed by the storefront site to operate. They are simply an audit capability if required.</font>', 300)" onmouseout="hideddrivetip()" src="images/info.gif" alt="" border="0" />
                                                    </td>
                                                </tr>    
                                                <tr>
                                                    <td align="right" style="width: 300px">
                                                        <font class="subTitleSmall">
                                                            Invalidate All User Logins:
                                                        </font>
                                                    </td>
                                                    <td align="left">
                                                        <asp:CheckBox ID="InvalidateUserLogins" runat="server" />
                                                        <img onmouseover="ddrivetip('<font class=\'exampleText\'>If checked, all user Cookies are invalidated. You can use this if you want to force all users to re-login to the store (or admin) site the next time. NO OTHER customer information is affected (billing, shipping, shopping carts, wishlists, etc...).</font>', 300)" onmouseout="hideddrivetip()" src="images/info.gif" alt="" border="0" />
                                                    </td>
                                                </tr>   
                                                <tr>
                                                    <td align="right" style="width: 300px">
                                                        <font class="subTitleSmall">
                                                            Purge Anon Customers:
                                                        </font>
                                                    </td>
                                                    <td align="left">
                                                        <asp:CheckBox ID="PurgeAnonUsers" runat="server" Checked="True" />
                                                        <img onmouseover="ddrivetip('<font class=\'exampleText\'>If checked, the store will delete all customer records that belong to \'Anon\' customers. Anon customers are those who have added something to their cart (or take an action on the store site which requires them to have a record in the db customer table) but they have never registered, checked out, etc. So these are dead records that can be safely deleted. If that customer returns to the storefront later, they will simply get a new customer record automatically created for them.</font>', 300)" onmouseout="hideddrivetip()" src="images/info.gif" alt="" border="0" />
                                                    </td>
                                                </tr> 
                                                <tr>
                                                    <td align="right" style="width: 300px">
                                                        <font class="subTitleSmall">
                                                            Erase Credit Card info from Address records:
                                                        </font>
                                                    </td>
                                                    <td align="left">
                                                        <asp:CheckBox ID="EraseAddressCreditCards" runat="server" Checked="True" />
                                                        <img onmouseover="ddrivetip('<font class=\'exampleText\'>Since order records contain a complete snapshot of all customer data, this option allows you to remove the customer credit card information (which are encrypted) from the order records which are older than the # of days specified. The credit card information is almost never needed after an order has been successfully processed and shipped and then nothing has been heard back from the customer for 30 days. This does NOT affect the customer\'s main account record, and their credit card information remains in that account record, intact, in an encrypted state (if you have your storefront set to store credit card info by setting AppConfig:StoreCCInDB=true).</font>', 300)" onmouseout="hideddrivetip()" src="images/info.gif" alt="" border="0" />
                                                    </td>
                                                </tr>                                                       
                                                <tr>
                                                    <td align="right" style="width: 300px">
                                                        <font class="subTitleSmall">
                                                            Tune Indexes:
                                                        </font>
                                                    </td>
                                                    <td align="left">
                                                        <asp:CheckBox ID="TuneIndexes" runat="server" Checked="True" />
                                                        <img onmouseover="ddrivetip('<font class=\'exampleText\'>Select this option to tune the table indexes.Periodically, they can require some reorganization for maximum performance. If you select this option, your database may be locked for several minutes while the indexes are tuned.</font>', 300)" onmouseout="hideddrivetip()" src="images/info.gif" alt="" border="0" />
                                                    </td>
                                                </tr>          
                                                <tr>
                                                    <td align="right" style="width: 300px">
                                                        <font class="subTitleSmall">
                                                            Save Settings:
                                                        </font>
                                                    </td>
                                                    <td align="left">
                                                        <asp:CheckBox ID="SaveSettings" runat="server" Checked="True" />
                                                        <img onmouseover="ddrivetip('<font class=\'exampleText\'>Select this option if you want to save your settings for next time.</font>', 300)" onmouseout="hideddrivetip()" src="images/info.gif" alt="" border="0" />
                                                    </td>
                                                </tr>                                           
                                                <tr>
                                                    <td>&nbsp;</td>
                                                    <td align="left" style="padding-top: 5px;">
                                                        (Please be patient, depending on the options selected, and the size of your database,
                                                        this operation could take minutes to complete!)<br /><br />
                                                        <asp:Label ID="lblNotice" runat="server" Font-Bold="true" ForeColor="Red" Text="Running this maintenance process will log out all users from the admin site, including the user running the maintenance"></asp:Label>
                                                        <br />
                                                        <asp:Button ID="GOButton" runat="server" CssClass="normalButton" Text="GO" OnClick="GOButton_Click" />
                                                        
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="wrapper">
                        <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                            <tr>
                                <td class="titleTable">
                                    <font class="subTitle">Other Required Maintenance:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="100%">
                                    <div class="wrapper">
                                        Along with using this page to perform monthly maintenance operations on your storefront, you, or your hosting provider (whoever manages your server) should also be performing weekly full backups, daily incremental backups, virus monitoring, applying the latest security and service patches, and generally watching over the health of your database server. Those functions are not provided by our technical support team, as they are in the operations area of managing your business. You can contact your hosting provider for more information also on their policies and procedures.
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
