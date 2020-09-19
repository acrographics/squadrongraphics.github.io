<%@ Page language="c#" Inherits="AspDotNetStorefront.lat_account" CodeFile="lat_account.aspx.cs" %>
<%@ Register TagPrefix="aspdnsf" TagName="Topic" src="TopicControl.ascx" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title></title>
</head>
<body>
    <asp:Panel ID="pnlMain" runat="server">

    <table id="Table1" runat="server" visible="true" cellspacing="5" cellpadding="5" border="0" width="100%">
            <tr>
                <td valign="top" align="left">
                    <asp:Panel ID="pnlBeforeSignup" runat="server" Visible="true">
                        <table cellspacing="0" cellpadding="1" width="171" bgcolor="#AAAAAA" border="0">
                            <tr>
                                <td bgcolor="#AAAAAA">&nbsp;<asp:Label ID="AppConfigAffiliateProgramName" runat="server" Font-Size="Smaller" Font-Bold="true" ForeColor="white" BackColor="#AAAAAA"></asp:Label></td>
                            </tr>

                            <tr valign="middle" align="center">
                                <td>
                                    <table cellspacing="0" cellpadding="8" width="100%" bgcolor="#AAAAAA" border="0">
                                        <tr>
                                            <td valign="top" bgcolor="#CCCCCC">
                                                <center>
                                                    <a href="lat_signout.aspx"><asp:Image ID="imgLogOut" AlternateText="Affiliate Logout" runat="server" /></a>
                                                </center>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>

                        <p>&nbsp;</p>

                        <table cellspacing="0" cellpadding="1" width="171" bgcolor="#AAAAAA" border="0" style="">
                            <tr>
                                <td bgcolor="#AAAAAA" height="18"><b style="color: #ffffff" class="small">&nbsp;<asp:Literal ID="Literal5" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.28!)" /></b></td>
                            </tr>

                            <tr valign="middle" align="center">
                                <td>
                                    <table cellspacing="0" cellpadding="4" width="100%" bgcolor="#AAAAAA" border="0">
                                        <tr>
                                            <td valign="top" align="left" width="100%" bgcolor="#ffffff">
                                                 &bull; <a href="lat_account.aspx"><asp:Literal ID="Literal1" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.27!)" /></a><br />
                                                 &bull; <a href="lat_driver.aspx?topic=affiliate_linking"><asp:Literal ID="Literal2" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.26!)" /></a><br/>
                                                 &bull; <a href="lat_driver.aspx?topic=affiliate_faq"><asp:Literal ID="Literal3" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.25!)" /></a><br />
                                                 &bull; <asp:HyperLink ID="AskAQuestion" runat="server" Text="(!lataccount.aspx.24!)"></asp:HyperLink><br />
                                                 &bull; <a href="lat_driver.aspx?topic=affiliate_terms"><asp:Literal ID="Literal4" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.23!)" /></a>
                                             </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>


                    <asp:Panel ID="pnlAfterSignup" runat="server" Visible="false">
                        <table cellSpacing="0" cellPadding="1" width="171" bgColor="#AAAAAA" border="0">
                            <tr>
                              <td bgcolor="#AAAAAA" height="18"><b style="color: #ffffff" class="small">&nbsp;<asp:Literal ID="Literal6" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.22!)" /></b></td>
                            </tr>
                            <tr valign="middle" align="center">
                                <td>
                                    <table cellSpacing="0" cellPadding="4" width="100%" bgColor="#AAAAAA" border="0">
                                        <tr>
                                            <td vAlign="top" align="left" width="100%" bgColor="#ffffff">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>

                    <br/><br/>
                    
                </td>

                <td width="100%" align="left" valign="top">
                    <table cellpadding="0" cellspacing="0" align="center" width="400">
                        <tr><td><asp:Image id="affiliateheader_small_gif" runat="server" /><br /><br /></td></tr>
                        <tr>
                            <td>
                                <center><asp:Label ID="lblErrMsg" runat="server" Font-Bold="true" ForeColor="red"></asp:Label></center>
                                <center><asp:Label ID="lblNote" runat="server" Font-Bold="true" ForeColor="blue"></asp:Label></center>
                            </td>
                        </tr>
                        <tr><td><b><asp:Literal ID="AppConfig_AffiliateProgramName2" runat="server" Mode="PassThrough"></asp:Literal></b><br /><br /></td></tr>
                        <tr><td><b><asp:Literal ID="AppConfig_AffiliateProgramName3" runat="server" Mode="PassThrough"></asp:Literal></b><br /><br /></td></tr>
                        <tr><td><b><asp:Literal ID="Literal7" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.29!)" /></b><br /><br /></td></tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblErrorMsg" runat="server" Font-Bold="true" ForeColor="red" ></asp:Label><br/><br/>

                                <form runat="server" action="lat_account.aspx">
                                    <asp:Table ID="tblAccount" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                                        <asp:TableRow>
                                            <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                                <asp:Image ID="accountinfo_gif" runat="server" AlternateText="" /><br />

                                                <asp:Table ID="tblAcctInfoBox" CellSpacing="0" CellPadding="4" Width="100%" runat="server">
                                                    <asp:TableRow>
                                                        <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                                            <table border="0" cellpadding="0" cellspacing="2" width="100%">
                                                                <tr>
                                                                    <td width="100%" colspan="2"><asp:Label runat="server" ID="AppConfig_AffiliateProgramName4" Font-Bold="true"></asp:Label></td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="100%" colspan="2">
                                                                        <hr />
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="30%"><asp:Literal ID="Literal8" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.7!)" /></td>

                                                                    <td width="70%">
                                                                        <asp:TextBox ID="FirstName" runat="server" MaxLength="50" Columns="20" TextMode="SingleLine" ValidationGroup="signup" CausesValidation="true" />
                                                                        <asp:RequiredFieldValidator ID="valReqFName" ControlToValidate="FirstName" EnableClientScript="true" ErrorMessage="Please enter your first name" runat="server" SetFocusOnError="true" ValidationGroup="signup" Display="None"></asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="30%"><asp:Literal ID="Literal9" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.8!)" /></td>

                                                                    <td width="70%">
                                                                        <asp:TextBox ID="LastName" runat="server" MaxLength="50" Columns="20" TextMode="SingleLine" ValidationGroup="signup" CausesValidation="true" />
                                                                        <asp:RequiredFieldValidator ID="valReqLName" ControlToValidate="LastName" EnableClientScript="true" ErrorMessage="Please enter your last name" runat="server" SetFocusOnError="true" ValidationGroup="signup" Display="None"></asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="30%"><asp:Literal ID="Literal10" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.9!)" /></td>

                                                                    <td width="70%">
                                                                        <asp:TextBox ID="EMail" runat="server" MaxLength="100" Columns="37" TextMode="SingleLine" CausesValidation="true" ValidationGroup="signup" /> 
                                                                        <asp:RequiredFieldValidator ID="Reqpwd" ControlToValidate="EMail" EnableClientScript="true" ErrorMessage="Please enter your e-mail address" runat="server" SetFocusOnError="true" ValidationGroup="signup" Display="None"></asp:RequiredFieldValidator>
                                                                        <asp:RegularExpressionValidator id="RegExValEmail" runat="SERVER" Display="None" ControlToValidate="EMail" EnableClientScript="true" ErrorMessage="Please enter a valid e-mail address" ValidationGroup="signup"  ValidationExpression="^[a-zA-Z0-9][-\w\.\+]*@([a-zA-Z0-9][\w\-]*\.)+[a-zA-Z]{2,4}$"></asp:RegularExpressionValidator>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="30%"><asp:Literal ID="Literal11" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.10!)" /></td>

                                                                    <td width="70%">
                                                                        <asp:TextBox ID="AffPassword" TextMode="Password" Columns="37" maxlength="100" runat="server" ValidationGroup="signup" CausesValidation="true"></asp:TextBox><br /><asp:Literal ID="Literal184" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.34!)" /> 
                                                                        <asp:CustomValidator ID="valPwd" runat="server" ControlToValidate="AffPassword" Display="none" EnableClientScript="false" ErrorMessage="" ValidationGroup="signup" SetFocusOnError="true" OnServerValidate="ValidatePassword"></asp:CustomValidator>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="30%"><asp:Literal ID="Literal12" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.11!)" /></td>

                                                                    <td width="70%">
                                                                        <asp:TextBox ID="AffPassword2" TextMode="Password" Columns="37" maxlength="100" runat="server" ValidationGroup="signup" CausesValidation="true"></asp:TextBox>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="30%"><asp:Literal ID="Literal13" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.12!)" /></td>

                                                                    <td width="70%"><asp:TextBox ID="Company" Columns="34" MaxLength="100" TextMode="singleLine"  runat="server" CausesValidation="false" /></td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="30%"><asp:Literal ID="Literal14" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.13!)" /></td>

                                                                    <td width="70%">
                                                                        <asp:TextBox ID="Address1" Columns="34" MaxLength="100" TextMode="singleLine"  runat="server" CausesValidation="true" ValidationGroup="signup" /> 
                                                                        <asp:RequiredFieldValidator ID="ReqAddr1" runat="server" ControlToValidate="Address1" ValidationGroup="signup" Display="None" EnableClientScript="true" ErrorMessage="Please enter an address" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="30%"><asp:Literal ID="Literal141" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.14!)" /></td>

                                                                    <td width="70%">
                                                                        <asp:TextBox ID="Address2" Columns="34" MaxLength="100" TextMode="singleLine"  runat="server" /> 
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="30%"><asp:Literal ID="Literal15" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.15!)" /></td>

                                                                    <td width="70%"><asp:TextBox ID="Suite" Columns="34" MaxLength="100" TextMode="singleLine"  runat="server" /></td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="30%"><asp:Literal ID="Literal16" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.16!)" /></td>

                                                                    <td width="70%">
                                                                        <asp:TextBox ID="City" Columns="34" MaxLength="50" TextMode="singleLine"  runat="server" CausesValidation="true" ValidationGroup="signup" /> 
                                                                        <asp:RequiredFieldValidator ID="valReqCity" ControlToValidate="City" ErrorMessage="(!address.cs.17!)" runat="server" ValidationGroup="signup" Display="None" EnableClientScript="true" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="30%"><asp:Literal ID="Literal17" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.17!)" /></td>

                                                                    <td width="70%">
                                                                        <asp:DropDownList ID="State" runat="server" Width="250" OnDataBound="State_DataBound"></asp:DropDownList>
                                                                        <asp:RequiredFieldValidator ID="valReqState" ControlToValidate="State" ErrorMessage="(!address.cs.17!)" runat="server" ValidationGroup="signup" Display="None" EnableClientScript="true" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="30%"><asp:Literal ID="Literal39" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.18!)" /></td>

                                                                    <td width="70%">
                                                                        <asp:TextBox ID="Zip" Columns="14" MaxLength="10" TextMode="singleLine"  runat="server" CausesValidation="true" ValidationGroup="signup" /> 
                                                                        <asp:RequiredFieldValidator ControlToValidate="Zip" ErrorMessage="(!address.cs.18!)" ID="RequiredFieldValidatorZip" runat="server" ValidationGroup="signup" Display="None" EnableClientScript="true" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="30%"><asp:Literal ID="Literal40" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.19!)" /></td>

                                                                    <td width="70%">
                                                                        <asp:DropDownList ID="Country" runat="server" Width="250" OnDataBound="Country_DataBound"></asp:DropDownList>
                                                                        <asp:RequiredFieldValidator ID="valReqCountry" ErrorMessage="(!address.cs.17a!)" runat="server" ControlToValidate="City" ValidationGroup="signup" Display="None" EnableClientScript="true" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="30%"><asp:Literal ID="Literal41" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.20!)" /></td>

                                                                    <td width="70%">
                                                                        <asp:TextBox ID="Phone" Columns="14" MaxLength="20" TextMode="singleLine"  runat="server" CausesValidation="true" ValidationGroup="signup" /> 
                                                                        <asp:RequiredFieldValidator ControlToValidate="Phone" ErrorMessage="(!address.cs.15!)" ID="RequiredFieldValidatorPhone" runat="server" ValidationGroup="signup" Display="None" EnableClientScript="true" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td width="30%"><asp:Literal ID="Literal42" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.21!)" /></td>

                                                                    <td width="70%"><asp:TextBox ID="DOBTxt" Columns="15" MaxLength="10" TextMode="singleLine"  runat="server" /></td>
                                                                </tr>
                                                            </table>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                </asp:Table>
                                                <br />

                                                <div align="center">
                                                    <asp:Button ID="btnUpdate1" runat="server" Text="(!lataccount.aspx.1!)" CausesValidation="true" ValidationGroup="signup" OnClick="btnUpdate1_Click" />
                                                </div>
                                                <br /><br />
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>

                                    <asp:Table ID="tblOnlineInfo" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                                        <asp:TableRow>
                                            <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                                <asp:Image ID="onlineinfo_gif" runat="server" AlternateText="" /><br />
                                                <asp:Table ID="tblOnlineInfoBox" CellSpacing="0" CellPadding="4" Width="100%" runat="server">
                                                    <asp:TableRow>
                                                        <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                                            <table border="0" cellpadding="0" cellspacing="2" width="100%">
                                                                <tr>
                                                                    <td width="100%" colspan="2"><b><asp:Literal ID="Literal22" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.5!)" /><br />
                                                                    <br />
                                                                    <asp:Literal ID="Literal23" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.6!)" /></b></td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="100%" colspan="2">
                                                                        <hr />
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="30%"><asp:Literal ID="Literal24" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.4!)" /></td>

                                                                    <td width="60%"><asp:TextBox ID="WebSiteName" runat="server" Columns="35" MaxLength="100"></asp:TextBox></td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="40%"><asp:Literal ID="Literal25" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.3!)" /></td>

                                                                    <td width="60%"><asp:TextBox ID="WebSiteDescription" runat="server" Columns="35" MaxLength="500"></asp:TextBox></td>
                                                                </tr>

                                                                <tr>
                                                                    <td width="40%"><asp:Literal ID="Literal26" runat="server" Mode="PassThrough" Text="(!lataccount.aspx.2!)" /></td>

                                                                    <td width="60%"><asp:TextBox ID="URL" runat="server" Columns="35" MaxLength="100"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </asp:TableCell>
                                                     </asp:TableRow>
                                                </asp:Table>
                                                <br />
                                                
                                                <div align="center">
                                                    <asp:Button ID="btnUpdate2" runat="server" Text="(!lataccount.aspx.1!)" CausesValidation="true" ValidationGroup="signup" OnClick="btnUpdate2_Click"  />
                                                </div>
                                                <br /><br />
                                            </asp:TableCell> 
                                        </asp:TableRow> 
                                    </asp:Table>
                                    
                                </form>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>
</body>
</html>
