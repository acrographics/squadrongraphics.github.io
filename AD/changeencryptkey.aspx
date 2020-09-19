<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.changeencryptkey" CodeFile="changeencryptkey.aspx.cs" EnableEventValidation="false"%>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>Encryption Test</title>
</head>
<body>
<form runat="server" id="form1">
    <b></b><strong>Change Your EncryptKey:<br /></strong>
    <br />
    We recommend that you periodically  change your encrypt key every 90 days. You only have to do this if you are storing credit card numbers in your database, or if you are using recurring billing products.<br />
    <br />
    You Are Storing Credit Cards:
    <asp:Label ID="StoringCC" runat="server" Font-Bold="True"></asp:Label><br />
    You are Using Recurring Billing Products That Require Credit Card Storage in the Database:
    <asp:Label ID="RecurringProducts" runat="server" Font-Bold="True"></asp:Label><br />
    <br />
    If either of the above values are <strong>Yes</strong>, you should change your encrypt key periodically. If both values above are <strong>No</strong> you do not need to do this.<br /><br />
    
    <asp:Panel runat="server" ID="DoItPanel" Width="100%" DefaultButton="Button1">
    <br />
    <hr size="1" />
    To change your encrypt key, just enter the new value below, and click on the 'Update EncryptKey' button below.<br />
    <br />
    <strong>Please be patient, as we will may have to update many thousands of records in your database!!! Do not cancel the operation!</strong><br />
    <br />
    Your encrypt key should be at least 8 characters and digits long.<br />
    
    <br />
    New Encrypt Key:
    <asp:TextBox ID="NewEncryptKey" runat="server" Width="317px" MaxLength="50"></asp:TextBox>
    <asp:Button ID="Button1" runat="server" CssClass="normalButton" OnClick="Button1_Click" Text="Update Encrypt Key!" /><br />
    <br />
    <asp:Label ID="OkLabel" runat="server" Font-Bold="True" ForeColor="Blue" Text="Done!" Visible="False"></asp:Label>
    <asp:Label ID="ErrorLabel" runat="server" Font-Bold="True" ForeColor="Crimson" Text="Error!!" Visible="False"></asp:Label>
    </asp:Panel>
    </form>
</body>
</html>


