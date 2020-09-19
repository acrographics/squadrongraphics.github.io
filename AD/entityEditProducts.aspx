<%@ Page Language="C#" AutoEventWireup="true" CodeFile="entityEditProducts.aspx.cs" Inherits="entityEditProducts" Theme="default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ OutputCache  Duration="1"  Location="none" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Entity Edit Product</title>
    <asp:Literal runat="server" ID="ltStyles"></asp:Literal>
    <script type="text/javascript"  src="Editor/scripts/innovaeditor.js"></script>
    <script type="text/javascript"  src="jscripts/toolTip.js">
        /***********************************************
        * Cool DHTML tooltip script II- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
        * This notice MUST stay intact for legal use
        * Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
        ***********************************************/
    </script>
    <script type="text/javascript" >
        function calcHeight()
        {
          //find the height of the internal page
          var the_height= document.getElementById('variantFrame').contentWindow.document.body.scrollHeight + 5;

          //change the height of the iframe
          document.getElementById('variantFrame').height=the_height;
        }
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
                        
        <div id="help" style="float: left;">
            <table border="0" cellpadding="1" cellspacing="0" class="outerTable">
                <tr>
                    <td>
                        <div class="wrapper">
                            <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                                <tr>                            
                                    <td class="titleTable">
                                        <font class="subTitle"><asp:Literal ID="ltPreEntity" runat="server"></asp:Literal> Product View:</font>
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
        <div id="help" style="float: left; margin-left: 0px;">
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
                                        <asp:Literal ID="ltStatus" runat="server"></asp:Literal>&nbsp;&nbsp;<asp:Literal ID="ltStatus2" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                    <td>
                        <asp:Panel ID="pnlDelete" runat="server" CssClass="wrapper">
                            <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                                <tr>                            
                                    <td class="contentTable">
                                        <asp:LinkButton ID="btnDeleteProduct" runat="server" Text="Delete this Product" OnClick="btnDeleteProduct_Click" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
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
                SiteMapXmlFile="EntityHelper/ProductTabs.xml" 
                MultiPageId="MultiPage1"
                ImagesBaseUrl="images/"
                DefaultSelectedItemLookId="SelectedTabLook" 
                DefaultItemLookId="DefaultTabLook"
                CssClass="TopGroup" >                            
                <ItemLooks>
                    <ComponentArt:ItemLook LabelPaddingLeft="5px" LabelPaddingRight="5px" LookId="DefaultTabLook" CssClass="DefaultTab">                 </ComponentArt:ItemLook>
                    <ComponentArt:ItemLook LabelPaddingLeft="5px" LabelPaddingRight="5px" LookId="SelectedTabLook" CssClass="SelectedTab">                </ComponentArt:ItemLook>
                </ItemLooks>
            </ComponentArt:TabStrip>
            
            <ComponentArt:MultiPage id="MultiPage1" runat="server" cssclass="MultiPage" Height="100%" Width="750">
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
                                                            <asp:RequiredFieldValidator ControlToValidate="txtName" ErrorMessage="Please enter the Product Name" ID="rfvName" ValidationGroup="Main" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!!</asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Product Type:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:DropDownList ID="ddType" runat="server"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator ControlToValidate="ddType" InitialValue="0" ErrorMessage="Please select a product type." ID="RequiredFieldValidator3" ValidationGroup="Main" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!!</asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Manufacturer:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:DropDownList ID="ddManufacturer" runat="server"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator ControlToValidate="ddManufacturer" InitialValue="0" ErrorMessage="Please select a manufacturer." ID="RequiredFieldValidator1" ValidationGroup="Main" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!!</asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Distributor:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:DropDownList ID="ddDistributor" runat="server"></asp:DropDownList>
                                                        </td>
                                                    </tr>                                                    
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">SKU:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox MaxLength="50" ID="txtSKU" runat="server" CssClass="singleShorter"></asp:TextBox>
                                                            &nbsp;
                                                            <font class="subTitleSmall">Manufacturer Part #:</font>
                                                            <asp:TextBox MaxLength="50" ID="txtManufacturePartNumber" runat="server" CssClass="singleShorter"></asp:TextBox>
                                                        </td>
                                                    </tr>    
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <asp:Label ID="lblPublished" runat="server">*Published:</asp:Label>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:RadioButtonList ID="rblPublished" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" style="height: 10px;"></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Google Checkout Allowed:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:RadioButtonList ID="GoogleCheckoutAllowed" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" style="height: 10px;"></td>
                                                    </tr>                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Display Format XmlPackage:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:DropDownList ID="ddXmlPackage" runat="Server"></asp:DropDownList>
                                                            <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Select XmlPackage to use to display this Product.</font>', 300)" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Page Size:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox MaxLength="2" ID="txtPageSize" runat="server" CssClass="single3chars"></asp:TextBox>
                                                            <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>May be used by the XmlPackage displaying this page.</font>', 300)" />
                                                            &nbsp;
                                                            <font class="subTitleSmall">Column Width:</font>
                                                            <asp:TextBox MaxLength="2" ID="txtColumn" runat="server" CssClass="single3chars"></asp:TextBox>
                                                            <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>May be used by the XmlPackage displaying this page.</font>', 300)" />
                                                        </td>
                                                    </tr>   
                                                    <tr>
                                                        <td colspan="2" style="height: 10px;"></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Tax Class:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:DropDownList ID="ddTaxClass" runat="Server"></asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" style="height: 10px;"></td>
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
                                                        <td colspan="2" style="height: 10px;"></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Start Date:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox ID="txtStartDate" runat="server" CssClass="singleShorter"></asp:TextBox>
                                                            <asp:Literal ID="ltStartDate" runat="server"></asp:Literal>
                                                            &nbsp;
                                                            <font class="subTitleSmall">Stop Date:</font>
                                                            <asp:TextBox ID="txtStopDate" runat="server" CssClass="singleShorter"></asp:TextBox>
                                                            <asp:Literal ID="ltStopDate" runat="server"></asp:Literal>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" style="height: 10px;"></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Show Buy Button:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:RadioButtonList ID="rblShowBuyButton" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                            <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>If No, the add to cart button will not be shown for this product.</font>', 300)" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Requires Registration To View:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:RadioButtonList ID="rblRequiresRegistrationToView" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Is Call to Order:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:RadioButtonList ID="rblIsCallToOrder" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                            <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>If Yes, CALL TO ORDER will be shown for this product, instead of the add to cart form/button.</font>', 300)" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Hide Price Until Cart:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:RadioButtonList ID="rblHidePriceUntilCart" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                            <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>If yes, customer must add product to cart in order to see the price.</font>', 300)" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Allow to be Added to Packs:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:RadioButtonList ID="rblAddToPacks" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Exclude from Froogle/PriceGrabber:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:RadioButtonList ID="rblExcludeFroogle" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" style="height: 10px;"></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Is a Kit:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:RadioButtonList ID="rblIsKit" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                            <asp:Literal ID="ltKit" runat="server"></asp:Literal>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Is a Pack:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:RadioButtonList ID="rblIsPack" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                            &nbsp;
                                                            <font class="subTitleSmall">Pack Size:</font>
                                                            <asp:TextBox MaxLength="3" ID="txtPackSize" runat="server" CssClass="single3chars"></asp:TextBox>
                                                        </td>
                                                    </tr> 
                                                    <tr>
                                                        <td colspan="2" style="height: 10px;"></td>
                                                    </tr>                                                 
                                                    <asp:Panel id="PopularityRow" runat="server" visible="false">
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Popularity:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox MaxLength="4" ID="txtPopularity" runat="server" CssClass="single3chars"></asp:TextBox>
                                                        </td>
                                                    </tr> 
                                                    <tr>
                                                        <td colspan="2" style="height: 10px;"></td>
                                                    </tr>   
                                                    </asp:Panel>                                              
                                                    <tr runat="server" id="trInventory1">
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Track Inventory By Size and Color:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:RadioButtonList ID="rblTrackSizeColor" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                    </tr>                                             
                                                    <tr runat="server" id="trInventory2">
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Color Option Prompt:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox MaxLength="100" ID="txtColorOption" runat="server" CssClass="singleLonger"></asp:TextBox>
                                                        </td>
                                                    </tr>                                             
                                                    <tr runat="server" id="trInventory3">
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Size Option Prompt:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox MaxLength="100" ID="txtSizeOption" runat="server" CssClass="singleLonger"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" style="height: 10px;"></td>
                                                    </tr>                                                
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Requires Text Field:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:RadioButtonList ID="rblRequiresTextField" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                            &nbsp;
                                                            <font class="subTitleSmall">Field Prompt:</font>
                                                            <asp:TextBox MaxLength="100" ID="txtTextFieldPrompt" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                            &nbsp;
                                                            <font class="subTitleSmall">Max Length:</font>
                                                            <asp:TextBox MaxLength="3" ID="txtTextOptionMax" runat="server" CssClass="single3chars"></asp:TextBox>
                                                            <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Number of characters allowed for this text option.</font>', 300)" />
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
                                                            <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Filename, with extension, e.g. myproductpic14.jpg, still assumed to be in images/product/icon, images/product/medium, and images/product/large directories!</font>', 300)" />
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
                                                    <tr>
                                                        <td align="right" valign="top">
                                                            <font class="subTitleSmall">Color Swatch:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:FileUpload CssClass="fileUpload" ID="fuSwatch" runat="Server" />
                                                            <div>
                                                                <asp:Literal ID="ltSwatch" runat="server"></asp:Literal>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="top">
                                                            <font class="subTitleSmall">Swatch Image Map:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox id="txtSwatchImageMap" runat="server" CssClass="multiLong" TextMode="multiline"></asp:TextBox>
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
                                                            <ed:RadEditorWrapper ID="radSummary" Runat="server" Editable="true"></ed:RadEditorWrapper> 
                                                            <br />
                                                            <asp:Literal ID="ltSummaryAuto" runat="server"></asp:Literal>
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
                                                            <asp:Literal ID="ltDescriptionFile1" runat="server"></asp:Literal>
                                                            <asp:Button ID="btnDescription" runat="server" CssClass="normalButton" OnClick="btnDescription_Click" Text="Delete"/>
                                                            <asp:Literal ID="ltDescriptionFile2" runat="server"></asp:Literal>
                                                            <ed:RadEditorWrapper ID="radDescription" Runat="server" Editable="true"></ed:RadEditorWrapper> 
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
                <ComponentArt:PageView runat="server" ID="Pageview6">
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
                                                            <font class="subTitleSmall">Froogle Description (No HTML):</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox CssClass="multiExtension" ID="txtFroogle" runat="Server" TextMode="multiLine"></asp:TextBox>
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
                    <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                        <tr>
                            <td align="left" width="100%">
                                <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                                    <tr>
                                        <td width="100%">
                                            <div class="wrapperExtraTop">
                                                <table cellpadding="0" cellspacing="1" border="0" width="100%">
                                                    <tr>
                                                        <td colspan="2" align="left" valign="top">
                                                            <font class="subTitleSmall">Product Mappings:</font>                                                            
                                                            <div style="padding-top: 10px; margin-bottom: 10px;">
                                                                <div style=" border-top: dashed 1px #909090; border-bottom: dashed 1px #909090; float: left; height: 350px; width: 25%; overflow: auto; font-size: 10px;">
                                                                    <b>Categories:</b>
                                                                    <br />
                                                                    <asp:CheckBoxList ID="cblCategory" runat="server" CellPadding="0" CellSpacing="0" RepeatColumns="1" RepeatDirection="Vertical" RepeatLayout="Flow"></asp:CheckBoxList>
                                                                </div>
                                                                <div style=" border-top: dashed 1px #909090; border-bottom: dashed 1px #909090; float: left; height: 350px; width: 25%; overflow: auto; font-size: 10px;">
                                                                    <b>Departments:</b>
                                                                    <br />
                                                                    <asp:CheckBoxList ID="cblDepartment" runat="server" CellPadding="0" CellSpacing="0" RepeatColumns="1" RepeatDirection="Vertical" RepeatLayout="Flow"></asp:CheckBoxList>
                                                                </div>
                                                                <div style=" border-top: dashed 1px #909090; border-bottom: dashed 1px #909090; float: left; height: 350px; width: 24%; overflow: auto; font-size: 10px;">
                                                                    <b>Affiliates:</b>
                                                                    <br />
                                                                    <asp:CheckBoxList ID="cblAffiliates" runat="server" CellPadding="0" CellSpacing="0" RepeatColumns="1" RepeatDirection="Vertical" RepeatLayout="Flow"></asp:CheckBoxList>
                                                                </div>
                                                                <div style=" border-top: dashed 1px #909090; border-bottom: dashed 1px #909090; float: left; height: 350px; width: 24%; overflow: auto; font-size: 10px;">
                                                                    <b>Customer Levels:</b>
                                                                    <br />
                                                                    <asp:CheckBoxList ID="cblCustomerLevels" runat="server" CellPadding="0" CellSpacing="0" RepeatColumns="1" RepeatDirection="Vertical" RepeatLayout="Flow"></asp:CheckBoxList>
                                                                </div>
                                                                <div style=" border-top: dashed 1px #909090; border-bottom: dashed 1px #909090; float: left; height: 350px; width: 24%; overflow: auto; font-size: 10px; visibility: hidden;">
                                                                    <b>Genres:</b>
                                                                    <br />
                                                                    <asp:CheckBoxList ID="cblGenres" runat="server" CellPadding="0" CellSpacing="0" RepeatColumns="1" RepeatDirection="Vertical" RepeatLayout="Flow"></asp:CheckBoxList>
                                                                </div>
                                                                <div style=" border-top: dashed 1px #909090; border-bottom: dashed 1px #909090; float: left; height: 350px; width: 24%; overflow: auto; font-size: 10px; visibility: hidden;">
                                                                    <b>Vectors:</b>
                                                                    <br />
                                                                    <asp:CheckBoxList ID="cblVectors" runat="server" CellPadding="0" CellSpacing="0" RepeatColumns="1" RepeatDirection="Vertical" RepeatLayout="Flow"></asp:CheckBoxList>
                                                                </div>
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
                <ComponentArt:PageView runat="server" ID="Pageview10">
                    <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                        <tr>
                            <td align="left" width="100%">
                                <table border="0" cellpadding="0" cellspacing="0" class="innerTable" width="100%">
                                    <tr>
                                        <td width="100%">
                                            <div class="wrapperExtraTop">
                                                <table cellpadding="0" cellspacing="1" border="0" width="100%">
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Related Products:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox CssClass="singleLongest" ID="txtRelatedProducts" runat="Server"></asp:TextBox>
                                                            <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter related PRODUCT IDs, NOT names, e.g. 42,13,150</font>', 300)" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Upsell Products:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox CssClass="singleLongest" ID="txtUpsellProducts" runat="Server"></asp:TextBox>
                                                            <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter upsell PRODUCT IDs, NOT names, e.g. 42,13,150</font>', 300)" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Upsell Product Discount Percent:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox MaxLength="5" CssClass="singleShortest" ID="txtUpsellProductsDiscount" runat="Server"></asp:TextBox>
                                                            <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter 0, or a percentage like 5 or 7.5</font>', 300)" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Requires Products:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox CssClass="singleLongest" ID="txtRequiresProducts" runat="Server"></asp:TextBox>
                                                            <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Enter PRODUCT IDs, NOT names, that MUST be in the cart if this product is also in the cart. The store will add these to the customer cart automatically if they are not present when this product is added. e.g. 42,13,150</font>', 300)" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" style="height: 10px;"></td>
                                                    </tr>  
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*'On Sale' Prompt:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:DropDownList ID="ddOnSalePrompt" runat="server"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator ControlToValidate="ddOnSalePrompt" InitialValue="0" ErrorMessage="Please select an 'On Sale' prompt." ID="RequiredFieldValidator4" ValidationGroup="Main" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!!</asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" style="height: 10px;"></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Spec Title:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox CssClass="singleLonger" ID="txtSpecTitle" runat="Server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Spec Call:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox CssClass="singleLonger" ID="txtSpecCall" runat="Server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">*Specs Inline:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:RadioButtonList ID="rblSpecsInline" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                    </tr>
                                                    <tr id="trSpecs" runat="server">
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Specifications:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:Literal ID="ltSpecs1" runat="server"></asp:Literal>
                                                            <asp:Button ID="btnSpecs" runat="server" OnClick="btnSpecs_Click" CssClass="normalButton" Text="Delete" />
                                                            <asp:Literal ID="ltSpecs2" runat="server"></asp:Literal>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" style="height: 10px;"></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Page BG Color:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox CssClass="singleShorter" ID="txtPageBG" runat="Server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Contents BG Color:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox CssClass="singleShorter" ID="txtContentsBG" runat="Server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="middle">
                                                            <font class="subTitleSmall">Skin Graphics Color:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox CssClass="singleShorter" ID="txtSkinColor" runat="Server"></asp:TextBox>
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
                <ComponentArt:PageView runat="server" ID="Pageview8">
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
                                                    <tr>
                                                        <td align="right" valign="top">
                                                            <font class="subTitleSmall">Miscellaneous Text:</font>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:TextBox CssClass="multiExtension" ID="txtMiscText" runat="Server" TextMode="multiLine"></asp:TextBox>
                                                            <br />
                                                            <asp:HyperLink ID="lnkAutoFill" runat="Server" Visible="False" NavigateUrl=""  Target="_blank" Text="AutoFill Variants"/>
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
                
                <ComponentArt:PageView runat="server" ID="Pageview9">
                    <asp:Placeholder runat="Server" ID="phAddVariant">
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
                                                                <font class="subTitleSmall">Price:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox MaxLength="10" ID="txtPrice" runat="server" CssClass="singleShortest"></asp:TextBox>
                                                            </td>
                                                        </tr>     
                                                        <tr>
                                                            <td align="right" valign="top">
                                                                <font class="subTitleSmall">Sale Price:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox MaxLength="10" ID="txtSalePrice" runat="server" CssClass="singleShortest"></asp:TextBox>
                                                            </td>
                                                        </tr>     
                                                        <tr>
                                                            <td align="right" valign="middle">
                                                                <font class="subTitleSmall">Weight:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox MaxLength="10" ID="txtWeight" runat="server" CssClass="single3chars"></asp:TextBox>
                                                                <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>In LBS eg. 4.59</font>', 300)" />
                                                            </td>
                                                        </tr>   
                                                        <tr>
                                                            <td align="right" valign="top">
                                                                <font class="subTitleSmall">Dimensions:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox MaxLength="10" ID="txtDimensions" runat="server" CssClass="singleShorter"></asp:TextBox>
                                                            </td>
                                                        </tr>   
                                                        <tr id="trInventory" runat="server">
                                                            <td align="right" valign="top">
                                                                <font class="subTitleSmall">Current Inventory:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox MaxLength="10" ID="txtInventory" runat="server" CssClass="singleShorter"></asp:TextBox>
                                                            </td>
                                                        </tr> 
                                                        <tr>
                                                            <td align="right" valign="middle">
                                                                <font class="subTitleSmall">Colors:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox ID="txtColors" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                                <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Separate colors by commas</font>', 300)" />
                                                            </td>
                                                        </tr>   
                                                        <tr>
                                                            <td align="right" valign="middle">
                                                                <font class="subTitleSmall">Color SKU Modifiers:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox ID="txtColorSKUModifiers" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                                <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Separate skus by commas to match colors</font>', 300)" />
                                                            </td>
                                                        </tr>   
                                                        <tr>
                                                            <td align="right" valign="middle">
                                                                <font class="subTitleSmall">Sizes:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox ID="txtSizes" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                                <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Separate sizes by commas</font>', 300)" />
                                                            </td>
                                                        </tr>   
                                                        <tr>
                                                            <td align="right" valign="middle">
                                                                <font class="subTitleSmall">Size SKU Modifiers:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:TextBox ID="txtSizeSKUModifiers" runat="server" CssClass="singleNormal"></asp:TextBox>
                                                                <img src="images/info.gif" border="0" onMouseOut="hideddrivetip()" onmouseover="ddrivetip('<font class=\'exampleText\'>Separate skus by commas to match sizes</font>', 300)" />
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
                    <asp:Placeholder ID="phAllVariants" runat="server">
                        <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                            <tr>
                                <td align="left">
                                    <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                                        <tr>
                                            <td>
                                                <div class="wrapperExtraTop">
                                                    <table cellpadding="0" cellspacing="1" border="0" width="100%">
                                                        <tr>
                                                            <td align="right" valign="middle">
                                                                <font class="subTitleSmall">Action:</font>
                                                            </td>
                                                            <td align="left" valign="middle">
                                                                <asp:Literal ID="ltVariantsLinks" runat="server"></asp:Literal> |
                                                                <asp:LinkButton ID="btnDeleteAll" runat="server" Text="Delete All Variants" OnClick="btnDeleteAll_Click" />
                                                            </td>
                                                        </tr>  
                                                        <tr>
                                                            <td align="right" valign="middle" colspan="2">
                                                                <asp:Literal ID="ltIFrame" runat="server"></asp:Literal>
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
                </ComponentArt:PageView>
            </ComponentArt:MultiPage>                                                              
        </div>
        
        <div style="width: 100%; text-align: left; padding-top: 10px;">
            &nbsp;&nbsp;<asp:Button ID="btnSubmit" runat="server" CssClass="normalButton" OnClick="btnSubmit_Click" ValidationGroup="Main" />
            <asp:ValidationSummary ID="validationSummary" runat="Server" ValidationGroup="Main" DisplayMode="BulletList" ShowMessageBox="true" ShowSummary="false" />
        </div>
                  
    </div>                              
    </form>
    <asp:Literal ID="ltScript" runat="server"></asp:Literal>
    <asp:Literal ID="ltScript2" runat="server"></asp:Literal>
    <asp:Literal ID="ltScript3" runat="server"></asp:Literal>
</body>
</html>