<%@ Page Language="C#" AutoEventWireup="true" CodeFile="entityEdit.aspx.cs" Inherits="entityEdit" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ OutputCache  Duration="1"  Location="none" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Entity Edit</title>
    <asp:Literal runat="server" ID="ltStyles"></asp:Literal>
    <script type="text/javascript"  src="Editor/scripts/innovaeditor.js"></script>
    <script type="text/javascript"  src="jscripts/toolTip.js">
        /***********************************************
        * Cool DHTML tooltip script II- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
        * This notice MUST stay intact for legal use
        * Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
        ***********************************************/
    </script>
</head>
<body>
    <form id="frmEntityEdit" runat="server" enctype="multipart/form-data" method="post">
    <div style="width: 100%;">
        <div id="help" style="float: right; margin-right: 5px;">
            <table border="0" cellpadding="1" cellspacing="0" class="outerTable" runat="server" id="tblLocale">
                <tr>
                    <td>        
                        <div class="wrapper">
                            <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                                <tr>                            
                                    <td class="titleTable">
                                        <font class="subTitle">Locale:</font>
                                    </td>
                                    <td class="contentTable">
                                        <asp:DropDownList ID="ddLocale" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddLocale_SelectedIndexChanged"></asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </div>                   
                    </td>
                </tr>
            </table>        
        </div>
                        
        <div id="Div1" style="float: left;">
            <table border="0" cellpadding="1" cellspacing="0" class="outerTable">
                <tr>
                    <td>
                        <div class="wrapper">
                            <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                                <tr>                            
                                    <td class="titleTable">
                                        <font class="subTitle"><asp:Literal ID="ltPreEntity" runat="server"></asp:Literal> View:</font>
                                    </td>
                                    <td class="contentTable">
                                        <font class="title">
                                            <asp:Literal ID="ltEntity" runat="server"></asp:Literal>
                                        </font>
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
        <div style="padding-top: 2px; clear: left;"></div>
        <div id="Div2" style="float: left; margin-left: 0px;">
            <table border="0" cellpadding="1" cellspacing="0" class="outerTable">
                <tr>
                    <td>        
                        <div class="wrapper">
                            <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                                <tr>                            
                                    <td class="titleTable">
                                        <font class="subTitle">Status:</font>
                                    </td>
                                    <td class="contentTable">
                                        <asp:Literal ID="ltStatus" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                            </table>
                        </div>                   
                    </td>
                </tr>
            </table>        
        </div>

    <%--<div style="padding-top: 15px; padding-bottom: 15px; clear: both;">
        <asp:Literal ID="ltPreText" runat="server"></asp:Literal>
    </div>--%>
        <div style="clear: both; padding-bottom: 15px;"></div>
       
        <div id="content" style="margin-right: 10px;"><!-- style="width: 98%;"-->
                                            
            <ComponentArt:TabStrip id="TabStrip1" runat="server" AutoPostBackOnSelect="false" 
                SiteMapXmlFile="EntityHelper/EntityTabs.xml" 
                MultiPageId="MultiPage1"
                ImagesBaseUrl="images/"
                DefaultSelectedItemLookId="SelectedTabLook" 
                DefaultItemLookId="DefaultTabLook"
                CssClass="TopGroup" >                            
                <ItemLooks>
                    <ComponentArt:ItemLook LabelPaddingLeft="5px" LabelPaddingRight="5px" LookId="DefaultTabLook" CssClass="DefaultTab"></ComponentArt:ItemLook>
                    <ComponentArt:ItemLook LabelPaddingLeft="5px" LabelPaddingRight="5px" LookId="SelectedTabLook" CssClass="SelectedTab"></ComponentArt:ItemLook>
                </ItemLooks>
            </ComponentArt:TabStrip>
            
            <ComponentArt:MultiPage id="MultiPage1" runat="server" cssclass="MultiPage" Height="100%" Width="100%">
                <ComponentArt:PageView runat="server" ID="Pageview1">
                    <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                        <tr>
                            <td align="left">
                                <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                                    <tr>
                                        <td>
                                            <div class="wrapperExtraTop">
                                                <table cellpadding="0" cellspacing="1" border="0">
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Name:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox CssClass="singleNormal" ID="txtName" runat="Server"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ControlToValidate="txtName" ID="rfvName" ValidationGroup="Main" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!!</asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Published:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:RadioButtonList ID="rblPublished" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                    </tr>
                                                    <tr id="trBrowser" runat="server">
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Show in Product Browser:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:RadioButtonList ID="rblBrowser" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                    </tr>
                                                    <tr id="trParent" runat="server">
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Parent:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:DropDownList ID="ddParent" runat="Server"></asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Display Format XmlPackage:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:DropDownList ID="ddXmlPackage" runat="Server"></asp:DropDownList>
                                                            <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Select XmlPackage to use to display this entity.</font>', 300)" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Quantity Discount Table:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:DropDownList ID="ddDiscountTable" runat="Server"></asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Page Size:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox MaxLength="2" ID="txtPageSize" runat="server" CssClass="single3chars"></asp:TextBox>
                                                            <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>May be used by the XmlPackage displaying this page.</font>', 300)" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Column Width:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox MaxLength="2" ID="txtColumn" runat="server" CssClass="single3chars"></asp:TextBox>
                                                            <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>May be used by the XmlPackage displaying this page.</font>', 300)" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Order Products By Looks:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:RadioButtonList ID="rblLooks" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                    </tr>
                                                    <!-- Address -->
                                                    <asp:PlaceHolder ID="phAddress" runat="Server">
                                                        <tr>
                                                            <td align="right" valign="middle">
                                                                <font class="subTitleSmall">Address:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox CssClass="singleNormal" ID="txtAddress1" runat="Server"></asp:TextBox>
                                                                <%--<asp:RequiredFieldValidator ControlToValidate="txtAddress1" ErrorMessage="Please enter the address." ID="RequiredFieldValidator1" ValidationGroup="Main" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!</asp:RequiredFieldValidator>--%>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" valign="middle">
                                                                <font class="subTitleSmall">Apt/Suite #:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox CssClass="singleShortest" ID="txtApt" runat="Server"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" valign="middle">
                                                                <font class="subTitleSmall">Address 2:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox CssClass="singleNormal" ID="txtAddress2" runat="Server"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" valign="middle">
                                                                <font class="subTitleSmall">City:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox CssClass="singleShorter" ID="txtCity" runat="Server"></asp:TextBox>
                                                                <%--<asp:RequiredFieldValidator ControlToValidate="txtCity" ErrorMessage="Please enter the city." ID="RequiredFieldValidator2" ValidationGroup="Main" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!</asp:RequiredFieldValidator>--%>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" valign="middle">
                                                                <font class="subTitleSmall">State:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:DropDownList ID="ddState" runat="server"></asp:DropDownList>
                                                                <%--<asp:RequiredFieldValidator ControlToValidate="ddState" InitialValue="0" ErrorMessage="Please select a state." ID="RequiredFieldValidator3" ValidationGroup="Main" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!</asp:RequiredFieldValidator>--%>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" valign="middle">
                                                                <font class="subTitleSmall">Zip Code:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox CssClass="singleShortest" ID="txtZip" runat="Server"></asp:TextBox>
                                                                <%--<asp:RequiredFieldValidator ControlToValidate="txtZip" ErrorMessage="Please enter the zip." ID="RequiredFieldValidator4" ValidationGroup="Main" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!</asp:RequiredFieldValidator>--%>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" valign="middle">
                                                                <font class="subTitleSmall">Country:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:DropDownList ID="ddCountry" runat="server"></asp:DropDownList>
                                                                <%--<asp:RequiredFieldValidator ControlToValidate="ddCountry" InitialValue="0" ErrorMessage="Please select a country." ID="RequiredFieldValidator5" ValidationGroup="Main" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!</asp:RequiredFieldValidator>--%>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" valign="middle">
                                                                <font class="subTitleSmall">E-Mail:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ErrorMessage="Fill in E-Mail" ControlToValidate="txtEmail" ID="rfvEmail" ValidationGroup="Main" SetFocusOnError="true" runat="server">!!</asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" valign="middle">
                                                                <font class="subTitleSmall">Website:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox ID="txtURL" runat="server"></asp:TextBox>
                                                                <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>e.g. http://www.abcd.com.</font>', 300)" />
                                                                <%--<asp:RequiredFieldValidator ErrorMessage="Fill in Website" ControlToValidate="txtURL" ID="RequiredFieldValidator7" ValidationGroup="Main" SetFocusOnError="true" runat="server">!</asp:RequiredFieldValidator> --%>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" valign="middle">
                                                                <font class="subTitleSmall">Phone:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox>
                                                                <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Please enter a valid phone number with areacode, e.g. (480) 555-1212</font>', 300)" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" valign="middle">
                                                                <font class="subTitleSmall">Fax:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox ID="txtFax" runat="server"></asp:TextBox>
                                                                <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Please enter a valid phone number with areacode, e.g. (480) 555-1212</font>', 300)" />
                                                            </td>
                                                        </tr>
                                                    </asp:PlaceHolder>
                                                </table>
                                            </div>                    
                                        </td>
                                    </tr>
                                </table> 
                            </td>
                        </tr>
                    </table>
                </ComponentArt:PageView>
                <ComponentArt:PageView runat="server" ID="Pageview2">
                    <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                        <tr>
                            <td align="left">
                                <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                                    <tr>
                                        <td>
                                            <div class="wrapperExtraTop">
                                                <table cellpadding="0" cellspacing="1" border="0">
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Image Filename Override:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox CssClass="singleNormal" ID="txtImageOverride" runat="Server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="top">
                                                            <font class="subTitleSmall">Icon:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:FileUpload CssClass="fileUpload" ID="fuIcon" runat="Server" />
                                                            <div>
                                                                <asp:Literal ID="ltIcon" runat="server"></asp:Literal>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="top">
                                                            <font class="subTitleSmall">Medium:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:FileUpload CssClass="fileUpload" ID="fuMedium" runat="Server" />
                                                            <div>
                                                                <asp:Literal ID="ltMedium" runat="server"></asp:Literal>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="top">
                                                            <font class="subTitleSmall">Large:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:FileUpload CssClass="fileUpload" ID="fuLarge" runat="Server" />
                                                            <div>
                                                                <asp:Literal ID="ltLarge" runat="server"></asp:Literal>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>                    
                                        </td>
                                    </tr>
                                </table> 
                            </td>
                        </tr>
                    </table>
                </ComponentArt:PageView>
                <ComponentArt:PageView runat="server" ID="Pageview4">
                    <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                        <tr>
                            <td align="left">
                                <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                                    <tr>
                                        <td>
                                            <div class="wrapperExtraTop">
                                                <table cellpadding="0" cellspacing="1" border="0" width="100%">
                                                    <tr>
                                                        <td align="left" valign="middle" width="100%">
                                                            <!--<asp:Literal ID="ltSummary" runat="Server"></asp:Literal>-->
                                                            <ed:RadEditorWrapper runat="server" id="radSummary"></ed:RadEditorWrapper>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>                    
                                        </td>
                                    </tr>
                                </table> 
                            </td>
                        </tr>
                    </table>
                </ComponentArt:PageView>
                <ComponentArt:PageView runat="server" ID="Pageview3">
                    <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                        <tr>
                            <td align="left">
                                <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                                    <tr>
                                        <td>
                                            <div class="wrapperExtraTop">
                                                <table cellpadding="0" cellspacing="1" border="0" width="100%">
                                                    <tr>
                                                        <td align="left" valign="middle" width="100%">
                                                            <!--<asp:Literal ID="ltDescription" runat="Server"></asp:Literal>-->
                                                            <ed:RadEditorWrapper runat="server" id="radDescription"></ed:RadEditorWrapper>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>                    
                                        </td>
                                    </tr>
                                </table> 
                            </td>
                        </tr>
                    </table>
                </ComponentArt:PageView>
                <ComponentArt:PageView runat="server" ID="Pageview5">
                    <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                        <tr>
                            <td align="left">
                                <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                                    <tr>
                                        <td>
                                            <div class="wrapperExtraTop">
                                                <table cellpadding="0" cellspacing="1" border="0">
                                                    <tr>
                                                        <td align="right" valign="top">
                                                            <font class="subTitleSmall">Extension Data (User Defined Data):</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox CssClass="multiExtension" ID="txtExtensionData" runat="Server" TextMode="multiLine"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr id="Tr2" runat="server" visible="false">
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Use Skin Template:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox CssClass="singleShorter" ID="txtUseSkinTemplateFile" runat="Server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr id="Tr1" runat="server" visible="false">
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Use Skin ID:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox CssClass="singleShorter" ID="txtUseSkinID" runat="Server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>                    
                                        </td>
                                    </tr>
                                </table> 
                            </td>
                        </tr>
                    </table>
                </ComponentArt:PageView>
                <ComponentArt:PageView runat="server" ID="Pageview6">
                    <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                        <tr>
                            <td align="left">
                                <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                                    <tr>
                                        <td>
                                            <div class="wrapperExtraTop">
                                                <table cellpadding="0" cellspacing="1" border="0" width="100%">
                                                    <tr>
                                                        <td align="right" valign="top" width="150">
                                                            <font class="subTitleSmall">Page Title:</font>
                                                        </td>
                                                        <td align="left" valign="middle" width="*">
                                                            <asp:TextBox CssClass="singleAuto" ID="txtSETitle" runat="Server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle" width="150">
                                                            <font class="subTitleSmall">Keywords:</font>
                                                        </td>
                                                        <td align="left" valign="middle" width="*">
                                                            <asp:TextBox CssClass="singleAuto" ID="txtSEKeywords" runat="Server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle" width="150">
                                                            <font class="subTitleSmall">Description:</font>
                                                        </td>
                                                        <td align="left" valign="middle" width="*">
                                                            <asp:TextBox CssClass="singleAuto" ID="txtSEDescription" runat="Server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="top" width="150">
                                                            <font class="subTitleSmall">No Script:</font>
                                                        </td>
                                                        <td align="left" valign="middle" width="*">
                                                            <asp:TextBox CssClass="multiAutoNormal" ID="txtSENoScript" runat="Server" TextMode="multiLine"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr runat="server" visible="false">
                                                        <td align="right" valign="middle" width="150">
                                                            <font class="subTitleSmall">Alt Text:</font>
                                                        </td>
                                                        <td align="left" valign="middle" width="*">
                                                            <asp:TextBox CssClass="singleAuto" ID="txtSEAlt" runat="Server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>                    
                                        </td>
                                    </tr>
                                </table> 
                            </td>
                        </tr>
                    </table>
                </ComponentArt:PageView>
                <ComponentArt:PageView runat="server" ID="Pageview7">
                    <asp:Placeholder runat="Server" ID="phProducts">
                        <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                            <tr>
                                <td align="left" width="100%">
                                    <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                                        <tr>
                                            <td width="100%">
                                                <div class="wrapperExtraTop">
                                                    <table cellpadding="0" cellspacing="1" border="0" width="100%">
                                                        <tr>
                                                            <td align="right" valign="top">
                                                                <font class="subTitleSmall">Products:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:HyperLink ID="lnkProducts" runat="server"></asp:HyperLink>
                                                            </td>
                                                        </tr>
                                                        <tr style="padding-top: 10px;">
                                                            <td align="right" valign="top">
                                                                <font class="subTitleSmall">Bulk Products:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:Literal ID="ltProducts" runat="server"></asp:Literal>
                                                            </td>
                                                        </tr>  
                                                        <tr id="trBulkFrame" runat="server">
                                                            <td colspan="2" width="100%">
                                                                <iframe name="bulkFrame" id="bulkFrame" frameborder="0" onLoad="calcHeight('bulkFrame');" scrolling="no" width="100%" marginheight="0" marginwidth="0"></iframe>
                                                            </td>
                                                        </tr>                                                  
                                                    </table>
                                                </div>                    
                                            </td>
                                        </tr>
                                    </table> 
                                </td>
                            </tr>
                        </table>
                    </asp:Placeholder>
                    <asp:Placeholder ID="phProductsNone" runat="server">
                        <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                            <tr>
                                <td align="left">
                                    <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                                        <tr>
                                            <td>
                                                <div class="wrapperExtraTop">
                                                    No Products exist.
                                                </div>                    
                                            </td>
                                        </tr>
                                    </table> 
                                </td>
                            </tr>
                        </table>
                    </asp:Placeholder>
                </ComponentArt:PageView>
                <ComponentArt:PageView runat="server" ID="pageView8">
                    <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="95%">
                        <tr>
                            <td align="left" width="100%">
                                <asp:Literal ID="ltIFrame" Mode="PassThrough" runat="server"></asp:Literal>
                            </td>
                        </tr>
                    </table>
                </ComponentArt:PageView>
            </ComponentArt:MultiPage>
                                                              
        </div>
        
        <div style="width: 100%; text-align: left; padding-top: 10px;">
            &nbsp;&nbsp;<asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" CssClass="normalButton" ValidationGroup="Main" />
            &nbsp;&nbsp;<asp:Button ID="btnDelete" runat="server" OnClick="btnDelete_Click" CssClass="normalButton" Text="Delete" />
            <asp:ValidationSummary ID="validationSummary" runat="Server" ValidationGroup="Main" DisplayMode="BulletList" ShowMessageBox="true" ShowSummary="false" />
        </div>
                  
    </div>                              
    </form>
    <asp:Literal ID="ltScript" runat="server"></asp:Literal>
    <asp:Literal ID="ltScript2" runat="server"></asp:Literal>
</body>
</html>
<script type="text/javascript" >
    function calcHeight(elName)
    {
      //find the height of the internal page
      var the_height=document.getElementById(elName).contentWindow.document.body.scrollHeight + 5;

      //change the height of the iframe
      document.getElementById(elName).height=the_height;
    }
</script>
