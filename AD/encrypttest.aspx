<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.encrypttest" CodeFile="encrypttest.aspx.cs" EnableEventValidation="false"%>
<html>
<head>
<title>Encryption Test</title>
</head>
<body>
<form runat="server" id="form1">
    <b>V2 Encryption:</b><br />
    <br />
    String To Encrypt:
    <asp:TextBox ID="TextBox1" runat="server" Width="390px"></asp:TextBox>
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" CssClass="normalButton" Text="Encrypt It!" /><br />
    <br />
    Encrypted Value:&nbsp; &nbsp;<asp:Label ID="Label1" runat="server" Width="395px"></asp:Label><br />
    <br />
    Decrypted Value:&nbsp;
    <asp:Label ID="Label2" runat="server" Width="389px"></asp:Label>
    <br />
    <br />
    <hr size="1"/>
    <br />
    <b>V1 Encryption:</b><br />
    <br />
    String To Encrypt:
    <asp:TextBox ID="TextBox2" runat="server" Width="390px"></asp:TextBox>
    <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" CssClass="normalButton" Text="Encrypt It!" /><br />
    <br />
    Encrypted Value:&nbsp; &nbsp;<asp:Label ID="Label3" runat="server" Width="395px"></asp:Label><br />
    <br />
    Decrypted Value:&nbsp;
    <asp:Label ID="Label4" runat="server" Width="389px"></asp:Label>
    <br />
    <br />
    <br />
    <hr size="1"/>
    <br />
    <b>V1 Encrypted -&gt; Unencrypt -&gt; V2 -&gt; Encrypt -&gt; Plain Text Again:</b><br />
    <br />
    V1 Encrypted Value:
    <asp:TextBox ID="TextBox3" runat="server" Width="390px"></asp:TextBox>
    <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" CssClass="normalButton" Text="Do It!" /><br />
    <br />
    V1 Plain Text:&nbsp; &nbsp;<asp:Label ID="Label5" runat="server" Width="395px"></asp:Label><br />
    <br />
    V2 Encrypted Value:&nbsp; &nbsp;<asp:Label ID="Label6" runat="server" Width="395px"></asp:Label><br />
    <br />
    V2
    Decrypted Value:&nbsp;
    <asp:Label ID="Label7" runat="server" Width="389px"></asp:Label>
    </form>
</body>
</html>


