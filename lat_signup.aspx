<%@ Page Language="c#" Inherits="AspDotNetStorefront.lat_signup" CodeFile="lat_signup.aspx.cs" %>

<%@ Register TagPrefix="aspdnsfc" Namespace="ASPDNSFControls" Assembly="ASPDNSFControls" %>
<%@ Register TagPrefix="aspdnsf" TagName="Topic" Src="TopicControl.ascx" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
</head>
<body>
    <asp:Literal ID="JSPopupRoutines" runat="server" Mode="PassThrough"></asp:Literal>
    <table runat="server" visible="true" cellspacing="5" cellpadding="5" border="0" width="100%">
        <tr>
            <td valign="top" align="left">
                <asp:Panel ID="pnlBeforeSignup" runat="server" Visible="true">
                    <table cellspacing="0" cellpadding="0" width="171" bgcolor="#AAAAAA" border="0">
                        <tr>
                            <td bgcolor="#AAAAAA">
                                &nbsp;<asp:Label ID="AppConfigAffiliateProgramName" runat="server" Font-Size="Smaller" Font-Bold="true" ForeColor="white" BackColor="#AAAAAA"></asp:Label></td>
                        </tr>
                        <tr valign="middle" align="center">
                            <td>
                                <table cellspacing="0" cellpadding="4" width="100%" bgcolor="#AAAAAA" border="0">
                                    <tr>
                                        <td valign="top" bgcolor="#CCCCCC">
                                            <center>
                                                <a href="lat_signin.aspx">
                                                    <asp:Image ID="imgLogin" AlternateText="Affiliate Login" runat="server" /></a>
                                            </center>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <p>
                        &nbsp;</p>
                    <table cellspacing="0" cellpadding="0" width="171" bgcolor="#AAAAAA" border="0">
                        <tr>
                            <td bgcolor="#AAAAAA" height="18">
                                <b class="small" style="color: #ffffff">&nbsp;Learn More</b></td>
                        </tr>
                        <tr valign="middle" align="center">
                            <td>
                                <table cellspacing="0" cellpadding="4" width="100%" bgcolor="#AAAAAA" border="0">
                                    <tr>
                                        <td valign="top" align="left" width="100%" bgcolor="#ffffff">
                                            Join our rapidly growing network of
                                            <asp:Literal ID="AppConfigAffiliateProgramName2" runat="server" Mode="PassThrough"></asp:Literal>
                                            <a href="t-affiliate.aspx">Read more</a>.</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <p>
                        &nbsp;</p>
                    <table cellspacing="0" cellpadding="0" width="171" bgcolor="#AAAAAA" border="0">
                        <tr>
                            <td bgcolor="#AAAAAA" height="18">
                                <b style="color: #ffffff" class="small">&nbsp;Need Help?</b></td>
                        </tr>
                        <tr valign="middle" align="center">
                            <td>
                                <table cellspacing="0" cellpadding="4" width="100%" bgcolor="#AAAAAA" border="0">
                                    <tr>
                                        <td valign="top" align="left" width="100%" bgcolor="#ffffff">
                                            &bull; <a href="lat_signin.aspx">Forgot your password?</a><br />
                                            <asp:Panel Style="display: inline;" runat="server" ID="YourAccountLinkPanel">
                                                &bull; <a href="lat_account.aspx">Your Account Page</a><br />
                                            </asp:Panel>
                                            &bull; <a href="t-affiliate_faq.aspx">FAQs</a><br />
                                            &bull;
                                            <asp:HyperLink ID="CustSvcEmailLink" runat="server" Text="Customer Service"></asp:HyperLink></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="pnlAfterSignup" runat="server" Visible="false">
                    <table cellspacing="0" cellpadding="0" width="171" bgcolor="#AAAAAA" border="0">
                        <tr>
                            <td bgcolor="#AAAAAA">
                                &nbsp;<asp:Label ID="AppConfigAffiliateProgramName4" runat="server" Font-Size="Smaller" Font-Bold="true" ForeColor="white" BackColor="#AAAAAA"></asp:Label></td>
                        </tr>
                        <tr valign="middle" align="center">
                            <td>
                                <table cellspacing="0" cellpadding="4" width="100%" bgcolor="#AAAAAA" border="0">
                                    <tr>
                                        <td valign="top" bgcolor="#CCCCCC">
                                            <center>
                                                <a href="lat_signout.aspx">
                                                    <asp:Image ID="imgLogout" AlternateText="Affiliate Logout" runat="server" /></a>
                                            </center>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <p>
                        &nbsp;</p>
                    <table cellspacing="0" cellpadding="1" width="171" bgcolor="#AAAAAA" border="0">
                        <tr>
                            <td bgcolor="#AAAAAA" height="18">
                                <b style="color: #ffffff" class="small">&nbsp;Program Links</b></td>
                        </tr>
                        <tr valign="middle" align="center">
                            <td>
                                <table cellspacing="0" cellpadding="4" width="100%" bgcolor="#AAAAAA" border="0">
                                    <tr>
                                        <td valign="top" align="left" width="100%" bgcolor="#ffffff">
                                            &bull; <a href="lat_account.aspx">Account Home</a><br/>
                                            &bull; <a href="lat_getlinking.aspx">Web Linking Instructions</a><br/>
                                            &bull; <a href="lat_driver.aspx?topic=affiliate_faq">FAQs</a><br/>
                                            &bull;
                                            <asp:HyperLink ID="lnkAskAQuestion" Text="Ask A Question" runat="server"></asp:HyperLink><br/>
                                            &bull; <a href="lat_driver.aspx?topic=affiliate_terms">Terms &amp; Conditions</a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <br />
                <br />
            </td>
            <td width="100%" align="left" valign="top">
                <asp:Image ID="AffiliateHeader" runat="server" AlternateText="" /><br />
                <br />
                <asp:Panel ID="pnlSignupSuccess" runat="server" Visible="false">
                    <asp:Label ID="lblSignupSuccess" runat="server" Font-Bold="true"></asp:Label>
                </asp:Panel>
                <asp:Panel ID="pnlSignedInMsg" runat="server" Visible="false">
                    You're already signed in...please <a href="lat_signout.aspx">sign out</a> here
                </asp:Panel>
                <asp:Panel ID="pnlSignUpForm" runat="server" Visible="false">
                    <aspdnsf:Topic runat="server" ID="AffiliateTeaser" TopicName="AffiliateTeaser" />
                    <p>
                        <b>Sign Up Below</b><br /><br />
                        <span class="serif">Please complete the sign-up form below. If you have a web site that you will be using to link to us, please complete the additional fields. If you do not have a web site, just ignore the additional fields.</span>
                        <br />
                    </p>
                    <asp:Label ID="lblErrorMsg" runat="server" Font-Bold="true" ForeColor="red"></asp:Label>
                    <form runat="server">
                        <asp:ValidationSummary DisplayMode="BulletList" ID="ValSummary" ShowMessageBox="false" runat="server" ShowSummary="true" ValidationGroup="signup" EnableClientScript="false" />
                        <table id="tblAccount" cellspacing="0" cellpadding="2" width="100%" runat="server">
                            <tr>
                                <td align="left" valign="top">
                                    <asp:Image ID="accountinfo_gif" runat="server" AlternateText="" /><br />
                                    <table id="tblAcctInfoBox" cellspacing="0" cellpadding="4" width="100%" runat="server">
                                        <tr>
                                            <td align="left" valign="top">
                                                <table border="0" cellpadding="0" cellspacing="2" width="100%">
                                                    <tr>
                                                        <td width="100%" colspan="2">
                                                            <asp:Label runat="server" ID="AppConfigAffiliateProgramName3" Font-Bold="true"></asp:Label></td>
                                                    </tr>
                                                    <tr>
                                                        <td width="100%" colspan="2">
                                                            <hr />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%">
                                                            *Your First Name:</td>
                                                        <td width="70%">
                                                            <asp:TextBox ID="FirstName" runat="server" MaxLength="50" Columns="20" TextMode="SingleLine" CausesValidation="false" />
                                                            <asp:RequiredFieldValidator ID="valReqFName" ControlToValidate="FirstName" EnableClientScript="false" ErrorMessage="Please enter your first name" runat="server" SetFocusOnError="true" ValidationGroup="signup" Display="None"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%">
                                                            *Your Last Name:</td>
                                                        <td width="70%">
                                                            <asp:TextBox ID="LastName" runat="server" MaxLength="50" Columns="20" TextMode="SingleLine" CausesValidation="false" />
                                                            <asp:RequiredFieldValidator ID="valReqLName" ControlToValidate="LastName" EnableClientScript="false" ErrorMessage="Please enter your last name" runat="server" SetFocusOnError="true" ValidationGroup="signup" Display="None"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%">
                                                            *Your E-Mail:</td>
                                                        <td width="70%">
                                                            <asp:TextBox ID="EMail" runat="server" MaxLength="100" Columns="37" TextMode="SingleLine" CausesValidation="true" ValidationGroup="signup" />
                                                            <asp:RequiredFieldValidator ID="Reqpwd" ControlToValidate="EMail" EnableClientScript="false" ErrorMessage="Please enter your e-mail address" runat="server" SetFocusOnError="true" ValidationGroup="signup" Display="None"></asp:RequiredFieldValidator>
                                                            <asp:RegularExpressionValidator ID="RegExValEmail" runat="SERVER" Display="None" ControlToValidate="EMail" EnableClientScript="false" ErrorMessage="Please enter a valid e-mail address" ValidationGroup="signup" ValidationExpression="^[a-zA-Z0-9][-\w\.\+]*@([a-zA-Z0-9][\w\-]*\.)+[a-zA-Z]{2,4}$"></asp:RegularExpressionValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%">
                                                            *Password</td>
                                                        <td width="70%">
                                                            <asp:TextBox ID="AffPassword" TextMode="Password" Columns="37" MaxLength="100" runat="server" ValidationGroup="signup" CausesValidation="true"></asp:TextBox><br />
                                                            (at least 5 chars long)
                                                            <asp:RequiredFieldValidator ID="reqAffPwd" runat="server" Display="None" ControlToValidate="AffPassword" EnableClientScript="false" ValidationGroup="signup" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                            <asp:CustomValidator ID="valPwd" runat="server" ControlToValidate="AffPassword" Display="none" EnableClientScript="false" ErrorMessage="" ValidationGroup="signup" SetFocusOnError="true" OnServerValidate="ValidatePassword"></asp:CustomValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%">
                                                            *Repeat Password:</td>
                                                        <td width="70%">
                                                            <asp:TextBox ID="AffPassword2" TextMode="Password" Columns="37" MaxLength="100" runat="server" ValidationGroup="signup" CausesValidation="true"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%">
                                                            Company:</td>
                                                        <td width="70%">
                                                            <asp:TextBox ID="Company" Columns="34" MaxLength="100" TextMode="singleLine" runat="server" CausesValidation="false" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%">
                                                            *Address1:</td>
                                                        <td width="70%">
                                                            <asp:TextBox ID="Address1" Columns="34" MaxLength="100" TextMode="singleLine" runat="server" CausesValidation="true" ValidationGroup="signup" />
                                                            <asp:RequiredFieldValidator ID="ReqAddr1" runat="server" ControlToValidate="Address1" ValidationGroup="signup" Display="None" EnableClientScript="false" ErrorMessage="Please enter an address" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%">
                                                            Address2:</td>
                                                        <td width="70%">
                                                            <asp:TextBox ID="Address2" Columns="34" MaxLength="100" TextMode="singleLine" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%">
                                                            Suite:</td>
                                                        <td width="70%">
                                                            <asp:TextBox ID="Suite" Columns="34" MaxLength="100" TextMode="singleLine" runat="server" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%">
                                                            *City:</td>
                                                        <td width="70%">
                                                            <asp:TextBox ID="City" Columns="34" MaxLength="50" TextMode="singleLine" runat="server" CausesValidation="true" ValidationGroup="signup" />
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorCity" ErrorMessage="Please enter a city" runat="server" ControlToValidate="City" ValidationGroup="signup" Display="None" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%">
                                                            *State/Province:</td>
                                                        <td width="70%">
                                                            <asp:DropDownList ID="State" runat="server" Width="250"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator ID="valReqState" ControlToValidate="State" ErrorMessage="Please select a state" runat="server" ValidationGroup="signup" Display="None" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%">
                                                            *Zip:</td>
                                                        <td width="70%">
                                                            <asp:TextBox ID="Zip" Columns="14" MaxLength="10" TextMode="singleLine" runat="server" CausesValidation="true" ValidationGroup="signup" />
                                                            <asp:RequiredFieldValidator ControlToValidate="Zip" ErrorMessage="Please enter the zipcode" ID="RequiredFieldValidatorZip" runat="server" ValidationGroup="signup" Display="None" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%">
                                                            *Country:</td>
                                                        <td width="70%">
                                                            <asp:DropDownList ID="Country" runat="server" Width="250">
                                                            </asp:DropDownList>
                                                            <asp:RequiredFieldValidator ID="valReqCountry" ControlToValidate="Country" ErrorMessage="Please select a country" runat="server" ValidationGroup="signup" Display="None" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%">
                                                            *Phone:</td>
                                                        <td width="70%">
                                                            <asp:TextBox ID="Phone" Columns="14" MaxLength="20" TextMode="singleLine" runat="server" CausesValidation="true" ValidationGroup="signup" />
                                                            <asp:RequiredFieldValidator ControlToValidate="Phone" ErrorMessage="Please enter the phone number" ID="RequiredFieldValidatorPhone" runat="server" ValidationGroup="signup" Display="None" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%">
                                                            Birthday:</td>
                                                        <td width="70%">
                                                            <asp:TextBox ID="DateOfBirth" runat="server" MaxLength="30" TextMode="singleLine" Columns="14"></asp:TextBox></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                    <table id="tblAffWebInfo" cellspacing="0" cellpadding="4" width="100%" runat="server">
                                        <tr>
                                            <td align="left" valign="top">
                                                <asp:Image ID="WebSiteInfoImage" runat="server" /><br />
                                                <table id="tblWebSiteInfoBox" cellspacing="0" cellpadding="4" width="100%" runat="server">
                                                    <tr>
                                                        <td align="left" valign="top">
                                                            <table border="0" cellpadding="0" cellspacing="2" width="100%">
                                                                <tr>
                                                                    <td width="100%" colspan="2">
                                                                        <b>Online affiliates must also complete the following fields.<br />
                                                                            <br />
                                                                            You only need to enter these fields if you will be using a web site to link to us.</b></td>
                                                                </tr>
                                                                <tr>
                                                                    <td width="100%" colspan="2">
                                                                        <hr />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td width="45%">
                                                                        Your Web Site Name:</td>
                                                                    <td width="55%">
                                                                        <asp:TextBox ID="WebSiteName" Columns="30" MaxLength="100" TextMode="singleLine" runat="server" /></td>
                                                                </tr>
                                                                <tr>
                                                                    <td width="45%">
                                                                        Your Web Site Description:</td>
                                                                    <td width="55%">
                                                                        <asp:TextBox ID="WebSiteDescription" Columns="30" MaxLength="500" TextMode="singleLine" runat="server" /></td>
                                                                </tr>
                                                                <tr>
                                                                    <td width="45%">
                                                                        Your Web Site URL:</td>
                                                                    <td width="55%">
                                                                        <asp:TextBox ID="URL" Columns="30" MaxLength="100" TextMode="singleLine" runat="server" /></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                    <div align="center">
                                        <br />
                                        <asp:CheckBox ID="cbkAgreeToTermsAndConditions" runat="server" CausesValidation="true" ValidationGroup="signup" />By selecting this box and pressing the "Join" button, I agree to these
                                        <asp:Literal ID="TermsLink" runat="server" Mode="PassThrough"></asp:Literal>
                                        <br />
                                        <br />
                                        <asp:Button ID="btnJoin" Text="Join" runat="server" CausesValidation="true" ValidationGroup="signup" Enabled="false" OnClick="btnJoin_Click" />
                                        <asp:CustomValidator ID="ValTerms" runat="server" ErrorMessage="Please select the Terms and Conditions checkbox to indicate that you agree with the Terms and Conditions." Display="None" ClientValidationFunction="AgreeToTerms" ValidationGroup="signup" OnServerValidate="ValTerms_ServerValidate"></asp:CustomValidator>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </form>
                </asp:Panel>
            </td>
        </tr>
    </table>
</body>
</html>
