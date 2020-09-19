<%@ Page Language="C#" AutoEventWireup="true" CodeFile="signin.aspx.cs" Inherits="signin" Theme="Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>AspDotNetStorefront Admin for Store: <asp:Literal id="ltStoreName" runat="server"></asp:Literal></title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
	<link rel="stylesheet" href="skins/Skin_1/style.css" type="text/css" />
</head>
<body>
    <script type="text/javascript" >
        if(top != self)
        {
            top.location = self.location;
        }
    </script>
    <form id="SigninForm" runat="server">
        <table border="0"  width="100%"><tr><td align="center">
		    <table width="564" cellpadding="0" cellspacing="0" border="0">
			    <tr>
				    <td><img alt="" src="images/spacer.gif" height="20" width="1"></td>
			    </tr>
			    <tr>
				    <td><table width="564" cellpadding="0" cellspacing="0">
					    <tr>
						    <td colspan="4">
						        <img alt="Site Login" src="skins/skin_1/images/title.gif" width="564" height="116"/>
						    </td>
					    </tr>
					    </table></td>
			    </tr>
			    <tr>
				    <td>
                        <asp:Panel ID="pnlSignIn" runat="server" HorizontalAlign="Center" DefaultButton="btnSubmit">
				            <table cellpadding="0" cellspacing="0" width="100%" border="0">
                                <tr>
		                            <td bgcolor="#CED6EA"><img alt="" src="images/spacer.gif" height="1" width="1"/></td>
		                            <td align="center">
        	        
		                                <asp:Literal ID="ltError" runat="Server"></asp:Literal>
        		
		                                <table cellspacing="0" cellpadding="0" width="80%" border="0">
                                            <tr valign="top">
                                              <td align="left" height="18" valign="middle" bgcolor="#6487DB"><font class="DarkCellText" size="2"><b>&nbsp;<asp:Label ID="Label4" runat="server" Text="(!signin.aspx.8!)"></asp:Label></b></font></td>
                                            </tr>

                                            <tr valign="top" bgcolor="#6487DB">
                                              <td align="left" width="100%" bgcolor="#e0e0e0">
                                                <table cellspacing="5" cellpadding="0" width="100%" border="0">
                                                    <tr valign="baseline">
                                                      <td colspan="2"><b><asp:Label ID="Label3" runat="server" Text="(!signin.aspx.11!)"></asp:Label></b></td>
                                                    </tr>

                                                    <tr valign="baseline">
                                                      <td align="right"><asp:Label ID="Label5" runat="server" Text="(!signin.aspx.10!)"></asp:Label></td>
                                                      <td>
                                                          <asp:TextBox ID="txtEMail" runat="server" Width="200" CausesValidation="True"></asp:TextBox>
                                                          <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtEMail" ErrorMessage="!!" Font-Bold="True" SetFocusOnError="True" ValidationGroup="LOGIN"></asp:RequiredFieldValidator></td>
                                                    </tr>

                                                    <tr valign="baseline">
                                                      <td align="right"><asp:Label ID="Label7" runat="server" Text="(!signin.aspx.12!)"></asp:Label></td>
                                                      <td>
                                                        <asp:TextBox ID="txtPassword" TextMode="password" Width="176px" runat="server" EnableViewState="False" CausesValidation="True"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtPassword" ErrorMessage="!!" Font-Bold="True" SetFocusOnError="True" ValidationGroup="LOGIN"></asp:RequiredFieldValidator></td>
                                                    </tr>
                                                    <asp:Literal ID="ltSecurityCode" runat="server"></asp:Literal><tr valign="baseline">
                                                      <td colspan="2" align="center">
                                                        <asp:CheckBox ID="cPersistLogin" runat="server" Visible="false"/>
                                                      </td>
                                                    </tr>
                                                    <tr valign="baseline">
                                                        <td valign="middle" align="right">
                                                            <asp:Label ID="Label9" runat="server" Text="" Visible="False"></asp:Label>
                                                        </td>
                                                        <td valign="middle" align="left">
                                                            <asp:TextBox ID="SecurityCode" runat="server" Visible="False" ValidationGroup="Group1" CausesValidation="True" Width="73px" EnableViewState="False"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="SecurityCode" ErrorMessage="(!signin.aspx.20!)" ValidationGroup="Group1" Enabled="False"></asp:RequiredFieldValidator></td>
                                                    </tr>
                                                    <tr valign="baseline">
                                                        <td valign="middle" align="center" colspan="2">
                                                            <asp:Image ID="SecurityImage" runat="server" Visible="False"></asp:Image>
                                                        </td>
                                                    </tr>
                                                </table>
                                              </td>
                                            </tr>

                                            <tr valign="top" bgcolor="#6487DB">
                                              <td align="left"><img alt="" height="2" src="images/spacer.gif" width="2" border="0"/></td>
                                            </tr>

                                            <tr valign="top">
                                              <td align="left">
                                                <p align="right">
                                                <asp:Button ID="btnSubmit" CssClass="normalButton" Text="Sign In" runat="Server" OnClick="btnSubmit_Click" ValidationGroup="LOGIN" />
                                                </p>
                                              </td>
                                            </tr>
                                        </table>
                

		                                <table cellspacing="0" cellpadding="0" width="80%" border="0">
		                                    <tr>
		                                        <td>
                                                    <p align="left"><asp:Label ID="Label6" runat="server" Text="(!signin.aspx.15!)" Font-Bold="true"></asp:Label>&nbsp;</p>
                                                    <p align="left"><asp:Label ID="Label8" runat="server" Text="(!signin.aspx.16!)"></asp:Label></p>
		                                        </td>
		                                    </tr>
                                            <tr valign="top" bgcolor="#6487DB">
                                              <td align="left" height="18" valign="middle"><font class="DarkCellText" size="2"><b>&nbsp;<asp:Label ID="Label2" runat="server" Text="(!signin.aspx.17!)"></asp:Label></b></font></td>
                                            </tr>
                                            <tr valign="top" bgcolor="#6487DB">
                                              <td align="left" bgcolor="#e0e0e0" colspan="3" style="height: 60px">
                                                <table cellspacing="5" cellpadding="0" width="100%" border="0">
                                                    <tr valign="baseline">
                                                      <td align="right"><asp:Label ID="Label1" runat="server" Text="(!signin.aspx.10!)"></asp:Label></td>
                                                      <td>
                      	                                <asp:TextBox ID="txtEMailRemind" Width="200" runat="Server" CausesValidation="True"></asp:TextBox>
                      	                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtEMailRemind" ErrorMessage="!!" Font-Bold="True" SetFocusOnError="True" ValidationGroup="FORGOT"></asp:RequiredFieldValidator></td>
                                                    </tr>
                                                </table>
                                              </td>
                                            </tr>
                                            <tr valign="top" bgcolor="#6487DB">
                                              <td align="left"><img alt="" height="2" src="images/spacer.gif" width="2" border="0"></td>
                                            </tr>
                                            <tr valign="top">
                                              <td align="left">
                                                <p align="right">
                                                    <asp:Button runat="server" CssClass="normalButton" Text="(!signin.aspx.18!)" ID="btnRequestNewPassword" OnClick="btnRequestNewPassword_Click"  ValidationGroup="FORGOT" />
                                                </p>
                                              </td>
                                            </tr>
		                                </table>
		                            </td>
		                            <td width="20" style="background-image:url(skins/skin_1/images/righttableline.gif)" height="100"></td>
		                        </tr>
                            </table>
	                    </asp:Panel>
	                    <asp:Panel ID="pnlChangePwd" runat="server" HorizontalAlign="center" Visible="false" DefaultButton="btnChangePwd">
	                        <table cellpadding="0" cellspacing="0" width="100%" border="0">
	                            <tr>
		                            <td style="border-left:solid #CED6EA 1px;" width="1">&nbsp;</td>
	                            
	                                <td align="center">
	                                    <table  cellpadding="1" cellspacing="0" width="80%" border="0">
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblPwdChgErrMsg" runat="server" Font-Bold="true" ForeColor="red" Visible="false"></asp:Label><br /><br />
                                                </td>
                                            </tr>
	                                        <tr>
	                                            <td colspan="2" bgcolor="#6487DB" ><asp:Label ID="lblChgPwdHeader" CssClass="DarkCellText" Text="Change Password" runat="server" Font-Bold="true"></asp:Label></td>
	                                        </tr>
                                            <tr>
                                                <td colspan="2" bgcolor="#e0e0e0">&nbsp;</td>
	                                        </tr>
                                            <tr>
	                                            <td align="right" bgcolor="#e0e0e0">
	                                                <asp:Label ID="lblChgPwdEmail" runat="server" Text="Email Address:"></asp:Label>
	                                            </td>
	                                            <td bgcolor="#e0e0e0">
	                                                <asp:TextBox ID="txtEmailNewPwd" runat="server" Columns="30"></asp:TextBox>
	                                            </td>
	                                        </tr>
	                                        <tr>
	                                            <td align="right" bgcolor="#e0e0e0">
	                                                <asp:Label ID="lblOldPwd" runat="server" Text="Old Password:"></asp:Label>
	                                            </td>
	                                            <td bgcolor="#e0e0e0">
	                                                <asp:TextBox ID="txtOldPwd" runat="server" Columns="30" TextMode="Password"></asp:TextBox>
	                                            </td>
	                                        </tr>
	                                        <tr>
	                                            <td align="right" bgcolor="#e0e0e0">
	                                                <asp:Label ID="lblNewPwd" runat="server" Text="New Password:"></asp:Label>
	                                            </td>
	                                            <td bgcolor="#e0e0e0">
	                                                <asp:TextBox ID="txtNewPwd" runat="server" Columns="30" TextMode="Password"></asp:TextBox>
	                                            </td>
	                                        </tr>
	                                        <tr>
	                                            <td align="right" bgcolor="#e0e0e0">
	                                                <asp:Label ID="lblConfirmPwd" runat="server" Text="Confirm Password:"></asp:Label>
	                                            </td>
	                                            <td bgcolor="#e0e0e0">
	                                                <asp:TextBox ID="txtConfirmPwd" runat="server" Columns="30" TextMode="Password"></asp:TextBox>
	                                            </td>
	                                        </tr>
                                            <tr>
                                                <td colspan="2" bgcolor="#e0e0e0">&nbsp;</td>
	                                        </tr>
                                            <tr valign="top" bgcolor="#6487DB">
                                                <td colspan="2" align="left"><img alt="" height="2" src="images/spacer.gif" width="2" border="0"/></td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">&nbsp;</td>
	                                        </tr>
	                                        <tr>
	                                            <td align="right" colspan="2"><asp:Button ID="btnChangePwd" CssClass="normalButton" runat="server" Text="Change Password" OnClick="btnChangePwd_OnClick"/></td>
	                                        </tr>
	                                    </table>
	                                </td>
                                    <td width="20" style="background-image:url(skins/skin_1/images/righttableline.gif)" height="100"></td>
	                            </tr>
	                        </table>
	                    </asp:Panel>
                    </td>
                </tr>

                <tr>
	                <td>
	                    <table cellpadding="0" cellspacing="0">
	                        <tr>
	                            <td valign="top"><img alt="" src="skins/skin_1/images/footer1.gif" height="34" width="48"/></td>
	                            <td style="background-image:url(skins/skin_1/images/footer2.gif)"  width=100%></td>
	                            <td valign="top"><img alt="" src="skins/skin_1/images/footer3.gif"  height="34" width="497"/></td>
	                            <td><img alt="" src="images/spacer.gif" height="1" width="19"/></td>
	                        </tr>
	                    </table>
	                </td>
                </tr>
            </table>
            <asp:ValidationSummary ID="validationSummary" runat="server" EnableClientScript="true" ShowMessageBox="true" ShowSummary="false" Enabled="true" ValidationGroup="vgLogin" />
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" EnableClientScript="true" ShowMessageBox="true" ShowSummary="false" Enabled="true" ValidationGroup="vgPassword" />

	 
	    </td></tr></table>
    </form>
</body>
</html>
