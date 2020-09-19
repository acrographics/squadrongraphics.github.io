<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MapShippingMethodToPaymentMethod.aspx.cs" Inherits="AspDotNetStorefrontAdmin.MapShippingMethodToPaymentMethod" Theme="default" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Map Shipping Methods to Payment Methods</title>
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
                                            Map Shipping Methods to Payment Methods
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
        <div style="margin-bottom: 5px; margin-top: 5px;">
            <asp:Literal ID="ltError" runat="server"></asp:Literal>
        </div>
    </div>

        <br />
        To use this function you have to activate (set TRUE)&nbsp;the AppSettings called
        UseMappingShipToPayment. If this AppConfig isn´t activated the standard selection
        of payment methods will appear (which means ALL payment method will show for all
        shipping options). Also if something in this mapping function will go wrong the
        original method will always be there as an backup function. No need to worry that
        NO payment options will appear in your customers browser.<br />
     <br />
     <div id="content">
        <table border="0" cellpadding="1" cellspacing="0" class="outerTable">
            <tr>
                <td style="width: 669px">
                    <div class="wrapper">                        
                        <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                            <tr>
                                <td class="titleTable">
                                    <font class="subTitle">Availible shipping methods</font>
                                </td>
              
                                <td class="titleTable">              
                                    <font class="subTitle">Availible payment methods</font>    
        
                                </td>
                                <td class="titleTable" style="width: 65px">
                                    <font class="subTitle">Move</font>
                                 </td>
                                <td class="titleTable">              
                                    <font class="subTitle">Selected payments methods</font>    
                                 </td>
                                <td class="titleTable" style="width: 196px" align="center">              
                                    <font class="subTitle">Prioritize</font></td>
                            <tr>
                                <td>
                                    <asp:ListBox ID="ListBoxAvailShippingMethods" runat="server" Height="150px" Width="225px" OnSelectedIndexChanged="ListBoxAvailShippingMethods_SelectedIndexChanged" AutoPostBack="True"></asp:ListBox>
                                </td>
                                <td>
                                    <asp:ListBox ID="ListBoxAvailPaymentMethods" runat="server" Height="150px" SelectionMode="Single" Width="225px"></asp:ListBox>
                                </td>
                                  <TD vAlign="middle" align="center" style="width: 53px"><asp:button id="BtnSelectOne" CssClass="normalButton" runat="server" Width="23" Height="18" Text=">" OnClick="BtnSelectOne_Click"></asp:button><br/>
									<asp:button id="BtnDeSelectOne" runat="server" Width="23" Height="18" CssClass="normalButton" Text="<" OnClick="BtnDeSelectOne_Click"></asp:button><br/>
									<asp:button id="BtnSelectAll" runat="server" Width="23" Height="18" CssClass="normalButton" Text=">>" OnClick="BtnSelectAll_Click"></asp:button><br/>
									<asp:button id="BtnDeSelectALL" runat="server" Width="23" Height="18" CssClass="normalButton" Text="<<" OnClick="BtnDeSelectALL_Click"></asp:button></TD>
                                   <td><asp:ListBox ID="ListBoxSelectedPaymentMethods" runat="server" Height="150px" SelectionMode="Multiple" Width="225px">
                                   </asp:ListBox>
                                   <br />
                                 </td>
                                <td style="width: 196px">
                                    &nbsp;<asp:Button ID="btnMovePaymentUp" runat="server" CssClass="normalButton" OnClick="btnMovePaymentUp_Click"
                                        Text="Move up" /></td>
                            </tr>
                           
                        </table>
                    </div>
                </td>
            </tr>
        </table>
       </div>
        <br />
        <font class="subTitle">Information to save into database if you update:</font><br />
                                   <asp:TextBox ID="txtSaveToDBInfo" runat="server" Width="453px" ReadOnly="true"></asp:TextBox>
        <br />
        <asp:Button ID="btnUpdateShippingToPaymentMethod" runat="server" CssClass="normalButton" OnClick="btnUpdateShippingToPaymentMethod_Click"
            Text="Update your mapping for selected shipping method to database" Width="455px" />
      
    </form>
</body>
</html>
