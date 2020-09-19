<%@ Page Language="C#" AutoEventWireup="true" CodeFile="editAffiliates.aspx.cs" Inherits="editAffiliates" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ OutputCache  Duration="1"  Location="none" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Affiliates</title>
    <asp:Literal runat="server" ID="ltStyles"></asp:Literal>
    <script type="text/javascript"  src="jscripts/toolTip.js">
        /***********************************************
        * Cool DHTML tooltip script II- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
        * This notice MUST stay intact for legal use
        * Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
        ***********************************************/
    </script>
</head>
<body>
    <form id="frmEditAffiliates" runat="server">
    <asp:Literal ID="ltScript1" runat="server"></asp:Literal>
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
                                    <a href="affiliates.aspx">
                                        <font class="title">
                                            Manage Affiliates
                                        </font>
                                    </a>
                                    ->
                                    <font class="title">
                                        Editing Affiliate:
                                        <span style="color: Red;">
                                            <asp:Literal id="ltAffiliate" runat="server"></asp:Literal>
                                        </span>
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
                                    <font class="subTitle">Affiliate Details:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="*">
                                    <div class="wrapperLeft">                                        
                                        <div style="margin-top: 5px; margin-bottom: 15px;">
                                            Fields marked with an asterisk (*) are required. All other fields are optional.
                                        </div>
                                        <table width="100%" cellpadding="1" cellspacing="0" border="0">
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Nick Name:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtNickName" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="260" align="right" valign="middle">
                                                    <font class="subTitleSmall">*First Name:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ValidationGroup="account" ErrorMessage="Fill in First Name" ControlToValidate="txtFirstName" ID="RequiredFieldValidator4" SetFocusOnError="true" runat="server" Display="None"></asp:RequiredFieldValidator> 
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="260" align="right" valign="middle">
                                                    <font class="subTitleSmall">Last Name:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtLastName" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Parent Affiliate:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:DropDownList ID="ddParent" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="top">
                                                    <font class="subTitleSmall">E-Mail:</font>
                                                </td>
                                                <td align="left" valign="top">
                                                    <asp:TextBox ID="txtEmail" runat="server" Width="269px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr runat="server" id="ResetPasswordRow">
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Reset Password:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:LinkButton ID="ResetPasswordLink" runat="server" OnClick="ResetPasswordLink_Click">Reset Password</asp:LinkButton>
                                                    <asp:Label ID="ResetPasswordError" runat="server" Font-Bold="True" ForeColor="Red" Text="Label" Visible="False"></asp:Label>
                                                    <asp:Label ID="ResetPasswordOk" runat="server" Font-Bold="True" ForeColor="Black" Text="Label" Visible="False"></asp:Label>
                                                </td>
                                            </tr>
                                            <asp:Panel ID="CreatePasswordRow" runat="server">
                                            <tr>
                                                <td align="right">Password:</td>

                                                <td >
                                                    <asp:TextBox ID="AffPassword" TextMode="Password" Columns="37" maxlength="100" runat="server" ValidationGroup="signup" CausesValidation="true"></asp:TextBox> (at least 4 chars long) 
                                                    <asp:CustomValidator ID="valPassword" runat="server" ControlToValidate="AffPassword" Display="none" EnableClientScript="false" ErrorMessage="" ValidationGroup="signup" SetFocusOnError="true" OnServerValidate="ValidatePassword"></asp:CustomValidator>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td align="right" valign="middle">*Repeat Password:</td>

                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="AffPassword2" TextMode="Password" Columns="37" maxlength="100" runat="server" ValidationGroup="signup" CausesValidation="true"></asp:TextBox>
                                                </td>
                                            </tr>
                                            </asp:Panel>
                                             <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Default Skin:</font>
                                                </td>                                                
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtSkin" runat="server" CssClass="singleShortest" Width="72px">1</asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="260" align="right" valign="middle">
                                                    <font class="subTitleSmall">Company:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtCompany" runat="server" Width="200px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="260" align="right" valign="middle">
                                                    <font class="subTitleSmall">Address1:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtAddress1" runat="server" CssClass="singleNormal" Width="200px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="260" align="right" valign="middle">
                                                    <font class="subTitleSmall">Address2:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtAddress2" runat="server" CssClass="singleNormal" Width="200px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="260" align="right" valign="middle">
                                                    <font class="subTitleSmall">Suite:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtSuite" runat="server" CssClass="singleShortest" Width="85px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="260" align="right" valign="middle">
                                                    <font class="subTitleSmall">City:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtCity" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">State/Province:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:DropDownList ID="ddState" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="260" align="right" valign="middle">
                                                    <font class="subTitleSmall">Zip:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtZip" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">Country:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:DropDownList ID="ddCountry" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="260" align="right" valign="middle">
                                                    <font class="subTitleSmall">Phone:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="260" align="right" valign="middle">
                                                    <font class="subTitleSmall">Date of Birth:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtBirthdate" runat="server" CssClass="singleShorter"></asp:TextBox>
                                                    <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>In format M/D/YYYY</font>', 200)" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="middle">
                                                    <font class="subTitleSmall">For Ad Tracking Only:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:RadioButtonList ID="rblAdTracking" runat="server">
                                                        <asp:ListItem Value="0">False</asp:ListItem>
                                                        <asp:ListItem Value="1" Selected="true">True</asp:ListItem>                                                            
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:ValidationSummary ValidationGroup="account" ID="validationSummary" runat="server" EnableClientScript="true" ShowMessageBox="true" ShowSummary="false" Enabled="true" />
                                                </td>
                                            </tr>
                                        </table>
                                        <div style="width: 100%; text-align: center;">
                                            &nbsp;&nbsp;<asp:Button ValidationGroup="account" ID="btnSubmit" runat="server" CssClass="normalButton" OnClick="btnSubmit_Click" />
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
                                    <font class="subTitle">Web Site Information:</font>
                                </td>
                            </tr>
                            <tr>
                                <td class="contentTable" valign="top" width="*">
                                    <div class="wrapperLeft">                                        
                                        <div style="margin-top: 5px; margin-bottom: 15px;">
                                            Enter these fields if the affiliate will be using a web site to link to us.
                                        </div>
                                        <table width="100%" cellpadding="1" cellspacing="0" border="0">
                                            <tr>
                                                <td width="260" align="right" valign="middle">
                                                    <font class="subTitleSmall">Web Site Name:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtWebName" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                </td>
                                            </tr>  
                                            <tr>
                                                <td width="260" align="right" valign="middle">
                                                    <font class="subTitleSmall">Web Site Description:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtWebDescription" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                </td>
                                            </tr>  
                                            <tr>
                                                <td width="260" align="right" valign="middle">
                                                    <font class="subTitleSmall">Web Site URL:</font>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:TextBox ID="txtWebURL" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                </td>
                                            </tr>                                         
                                            <tr>
                                                <td colspan="2">
                                                </td>
                                            </tr>
                                        </table>
                                        <div style="width: 100%; text-align: center;">
                                            &nbsp;&nbsp;<asp:Button ValidationGroup="account" ID="btnSubmit1" runat="server" CssClass="normalButton" OnClick="btnSubmit1_Click" />
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
    <asp:Literal ID="ltScript" runat="server"></asp:Literal>
</body>
</html>
