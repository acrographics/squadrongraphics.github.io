<%@ Control Language="c#" AutoEventWireup="false" Inherits="AspDotNetStorefront.TemplateBase" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<title>(!METATITLE!)</title>
<meta name="description" content="(!METADESCRIPTION!)">
<meta name="keywords" content="(!METAKEYWORDS!)">
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/style.css" type="text/css">
<script type="text/javascript" src="jscripts/formValidate.js"></script>
</head>
<body rightmargin="0" leftmargin="0" topmargin="0" marginwidth="0" marginheight="0" bgcolor="#ffffff">
(!XmlPackage Name="skin.adminalert.xml.config"!)
(!PAGEINFO!)
    <!-- to center your pages, set text-align: center -->
    <!-- to left justify your pages, set text-align: center -->
    <!-- if using dynamic full width page sizes, the left-right align has no effect (obviously) -->
    <div style="margin-top: 0px; margin-left: auto; text-align: center"> <!-- for dynamic full width page sizes, leave centerdiv width at 100% --> <!-- for fixed width page sizes, set centerdiv width:800px (or whatever you want) -->  <!-- to left align, set margin-right: auto; --> <!-- to center align, set margin-left: auto; margin-right: auto; -->
      <div id="centerdiv" style="margin-left:auto; width:800; margin-right:auto">
    <table width="800" border="0" cellspacing="0" cellpadding="0">
    <tr><td colspan="2" width="100%" valign="middle" align="right"><div style="visibility:(!COUNTRYDIVVISIBILITY!); display:(!COUNTRYDIVDISPLAY!);">Language: (!COUNTRYSELECTLIST!)&nbsp;</div><div style="visibility:(!CURRENCYDIVVISIBILITY!); display:(!CURRENCYDIVDISPLAY!);">Currency:(!CURRENCYSELECTLIST!)</div><div style="visibility:(!VATDIVVISIBILITY!); display:(!VATDIVDISPLAY!);">VAT Mode: (!VATSELECTLIST!)</div></td></tr>
    <tr><td colspan="2" width="100%" align="left"><font color="red"><b>
The sample skin by default has FIVE navigational elements in it: Horizontal Menu, Vertical Menu, Dynamic Expanding Tree, and 3 Browse By Boxes. You should delete ALL BUT ONE from your production skin, or you site will run really slow. Most sites only require ONE navigational element! To remove this notice, edit the skins/skin_(!SKINID!)/template.ascx file. That file is also where you will be removing the navigational elements that you do not need. You can edit that file in Notepad.
    </b></font>
</td></tr>
    <tr> 
      <td colspan="2"><table width="771" border="0" cellspacing="0" cellpadding="0">
          <tr> 
            <td><table width="771" height="56" border="0" cellpadding="0" cellspacing="0">
                <tr> 
                  <td width="308" height="56"><a href="default.aspx"><img src="skins/skin_(!SKINID!)/images/logo.gif" width="308" height="56" border="0" alt="Home"/></a></td>
                  <td height="56" width="100%" align="right">
                    <a href="shoppingcart.aspx" class="head">
                      Shopping Cart</a> | <a href="wishlist.aspx" class="head">Wish List</a> | <a href="giftregistry.aspx" class="head">Gift Registry</a> | <a href="t-affiliate.aspx" class="head">Affiliates</a> | <a href="t-service.aspx" class="head">
                      Customer Service</a>&nbsp;&nbsp;<br/>
                    <br/>
                    <span class="username">(!USERNAME!)&nbsp;&nbsp;<a class="username" href="(!SIGNINOUT_LINK!)">(!SIGNINOUT_TEXT!)</a>&nbsp;&nbsp;<br/>
                    You have (!NUM_CART_ITEMS!) item(s) in your <a class="username" href="shoppingcart.aspx">(!CARTPROMPT!)</a>&nbsp;&nbsp;</span></td>
                </tr>
              </table></td>
          </tr>
          <tr>
            <td><table width="800" height="83" border="0" cellpadding="0" cellspacing="0">
                <tr> 
                  <td width="179" height="83" valign="top"><table width="179" height="83" border="0" cellpadding="0" cellspacing="0">
                      <tr> 
                        <td width="179" height="17" valign="top"><img src="skins/skin_(!SKINID!)/images/rounded_top.gif" width="179" height="17"/></td>
                      </tr>
                      <tr>
                        <td valign="top" background="skins/skin_(!SKINID!)/images/Grey.gif">
							<form style="cursor: hand; margin-top: 0px; margin-bottom: 0px;" id="topsearchform" name="topsearchform" method="GET" action="search.aspx">
                            <div align="center" class="OrangeTitles">Search<br/>
                            <input name="SearchTerm" type="text" class="searchBox" size="15">
                            &nbsp;<img src="skins/skin_(!SKINID!)/images/GO.gif" style="cursor:hand;" onClick="document.topsearchform.submit()" width="28" height="28" align="absmiddle" border="0"/> 
                            </div>
                            </form>
                          </td>
                      </tr>
                    </table></td>
                  <td width="621" height="83" valign="top" background="skins/skin_(!SKINID!)/images/Navi.jpg" class="TopNavi"> 
                    <table width="621" height="22" border="0" cellpadding="0" cellspacing="0">
                      <tr>
                        <td valign="middle" align="left" style="padding-left: 12px;" height="23">
<!-- TOP MENU -->
    <ComponentArt:Menu id="PageMenu" 
	  ClientScriptLocation="skins/componentart_webui_client/"
      ImagesBaseUrl="skins/skin_1/images/"
	  ScrollingEnabled="true"
      ScrollUpLookId="ScrollUpItemLook"
      ScrollDownLookId="ScrollDownItemLook"
	  Orientation="horizontal"
      CssClass="TopMenuGroup"
      DefaultGroupCssClass="MenuGroup"
      DefaultItemLookID="DefaultItemLook"
      DefaultGroupItemSpacing="1"
      ExpandDelay="0"
      ExpandDuration="0"
      ExpandSlide="None"
      ExpandTransition="None"
	  CascadeCollapse="false"
	  CollapseDelay="0"
	  CollapseSlide="None"
	  CollapseTransition="None"
      EnableViewState="false"
      runat="server">
    <ItemLooks>
		  <ComponentArt:ItemLook LookId="DefaultItemLook" HoverCssClass="MenuItemHover" LabelPaddingTop="2px" ActiveCssClass="MenuItemDown" LabelPaddingRight="15px" LabelPaddingBottom="2px" ExpandedCssClass="MenuItemDown" LabelPaddingLeft="5px" CssClass="MenuItem" />
      	  <ComponentArt:ItemLook LookId="TopItemLook" CssClass="TopMenuItem" HoverCssClass="TopMenuItemHover" LabelPaddingLeft="4" LabelPaddingRight="4" LabelPaddingTop="2" LabelPaddingBottom="2" />
    	  <ComponentArt:ItemLook LookID="ScrollUpItemLook" ImageUrl="scroll_up.gif" ImageWidth="15" ImageHeight="13" CssClass="ScrollItem" HoverCssClass="ScrollItemH" ActiveCssClass="ScrollItemA" />
	      <ComponentArt:ItemLook LookID="ScrollDownItemLook" ImageUrl="scroll_down.gif" ImageWidth="15" ImageHeight="13" CssClass="ScrollItem" HoverCssClass="ScrollItemH" ActiveCssClass="ScrollItemA" />
	      <ComponentArt:ItemLook LookID="BreakItem" ImageUrl="break.gif" ImageHeight="1" ImageWidth="100%" />
    </ItemLooks>
    </ComponentArt:Menu>
<!-- END TOP MENU -->
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table></td>
          </tr>
        </table></td>
    </tr>
    <tr> 
      <td width="179" valign="top" height="100%">
<!-- START LEFT COL -->
			<table width="179" height="100%" border="0" cellspacing="0" cellpadding="0">
          <tr><td height="1" colspan="3" valign="top"></td></tr>
		  <tr>
            <td width="5" valign="top" background="skins/skin_(!SKINID!)/images/Grey.gif" class="LeftNavi">&nbsp;</td>
	        <td colspan="2" valign="top" background="skins/skin_(!SKINID!)/images/grey.gif" align="left">
<!-- LEFT "VERTICAL" MENU -->
<span class="OrangeTitles">Browse By</span><br/><br/>
    <ComponentArt:Menu id="VertMenu" 
	  ClientScriptLocation="skins/componentart_webui_client/"
	  Orientation="vertical"
	  ScrollingEnabled="true"
      ScrollUpLookId="VertScrollUpItemLook"
      ScrollDownLookId="VertScrollDownItemLook"
      CssClass="VertTopMenuGroup"
      DefaultGroupCssClass="VertMenuGroup"
      DefaultItemLookID="VertDefaultItemLook"
      DefaultGroupItemSpacing="1"
      ExpandDelay="0"
      ExpandDuration="0"
      ExpandSlide="None"
      ExpandTransition="None"
	  CascadeCollapse="false"
	  CollapseDelay="0"
	  CollapseSlide="None"
	  CollapseTransition="None"
      ImagesBaseUrl="skins/skin_1/images/"
      EnableViewState="false"
	  Width="100%"
      runat="server">
    <ItemLooks>
		  <ComponentArt:ItemLook LookId="DefaultItemLook" HoverCssClass="MenuItemHover" LabelPaddingTop="2px" ActiveCssClass="MenuItemDown" LabelPaddingRight="15px" LabelPaddingBottom="2px" ExpandedCssClass="MenuItemDown" LabelPaddingLeft="5px" CssClass="MenuItem" />
    	  <ComponentArt:ItemLook LookId="VertTopItemLook" CssClass="VertTopMenuItem" HoverCssClass="VertTopMenuItemHover" LabelPaddingLeft="4" LabelPaddingRight="4" LabelPaddingTop="0" LabelPaddingBottom="0" />
    	  <ComponentArt:ItemLook LookId="VertDefaultItemLook" CssClass="VertMenuItem" HoverCssClass="VertMenuItemHover" LabelPaddingLeft="4" LabelPaddingRight="4" LabelPaddingTop="0" LabelPaddingBottom="0" />
    	  <ComponentArt:ItemLook LookID="VertScrollUpItemLook" ImageUrl="scroll_up.gif" ImageWidth="15" ImageHeight="13" CssClass="VertScrollItem" HoverCssClass="VertScrollItemH" ActiveCssClass="VertScrollItemA" />
	      <ComponentArt:ItemLook LookID="VertScrollDownItemLook" ImageUrl="scroll_down.gif" ImageWidth="15" ImageHeight="13" CssClass="VertScrollItem" HoverCssClass="VertScrollItemH" ActiveCssClass="VertScrollItemA" />
    </ItemLooks>
    </ComponentArt:Menu>
<!-- END LEFT "VERTICAL" MENU -->
<br/>
</td>
		</tr>

          <tr><td height="1" colspan="3" valign="top"></td></tr>
		  <tr>
            <td width="5" valign="top" background="skins/skin_(!SKINID!)/images/Grey.gif" class="LeftNavi">&nbsp;</td>
	        <td colspan="2" valign="top" background="skins/skin_(!SKINID!)/images/grey.gif" align="left">
<!-- LEFT TREE CONTROL -->
<span class="OrangeTitles">Browse By</span><br/><br/>
<ComponentArt:TreeView id="PageTree" Width="175"  
	  ClientScriptLocation="skins/componentart_webui_client/"
      DragAndDropEnabled="false" 
      NodeEditingEnabled="false" 
      KeyboardEnabled="true"
      CssClass="TreeView" 
      NodeCssClass="TreeNode" 
      HoverNodeCssClass="HoverTreeNode" 
      SelectedNodeCssClass="SelectedTreeNode" 
      NodeEditCssClass="NodeEdit"
      DefaultImageWidth="16" 
      DefaultImageHeight="16"
      ExpandCollapseImageWidth="17"
      ExpandCollapseImageHeight="15"
      NodeLabelPadding="1"
      ItemSpacing="1" 
      NodeIndent="10"
      ImagesBaseUrl="skins/skin_1/images/"
      ParentNodeImageUrl="" 
      LeafNodeImageUrl="" 
      ExpandImageUrl="col.gif" 
      CollapseImageUrl="exp.gif" 
      EnableViewState="false"
			runat="server" >
    </ComponentArt:TreeView>
<!-- END LEFT TREE CONTROL -->
<br/>
			  </td>
		  </tr>
		  <tr><td height="1" colspan="3" valign="top"></td></tr>
          <tr> 
            <td width="5" valign="top" background="skins/skin_(!SKINID!)/images/Grey.gif" class="LeftNavi">&nbsp;</td>
            <td colspan="2" valign="top" background="skins/skin_(!SKINID!)/images/Grey.gif">
            <span class="OrangeTitles">Browse By (!CATEGORY_PROMPT_SINGULAR!)</span><br/><br/>
			(!ADVANCED_CATEGORY_BROWSE_BOX!)
			<br/>
            </td>
          </tr>
          <tr> 
            <td height="1" colspan="3" valign="top"></td>
          </tr>
          <tr> 
            <td valign="top" background="skins/skin_(!SKINID!)/images/Grey.gif">&nbsp;</td>
            <td colspan="2" valign="top" background="skins/skin_(!SKINID!)/images/Grey.gif">
            <span class="OrangeTitles">Browse By (!SECTION_PROMPT_SINGULAR!)</span><br/><br/>
			(!ADVANCED_SECTION_BROWSE_BOX!)
			<br/>
            </td>
          </tr>
          <tr> 
            <td height="1" colspan="3" valign="top"></td>
          </tr>
          <tr> 
            <td valign="top" background="skins/skin_(!SKINID!)/images/Grey.gif">&nbsp;</td>
            <td colspan="2" valign="top" background="skins/skin_(!SKINID!)/images/Grey.gif">
            <span class="OrangeTitles">Browse By Manufacturer</span><br/><br/>
			(!ADVANCED_MANUFACTURER_BROWSE_BOX!)
			<br/>
            </td>
          </tr>
          <tr> 
            <td height="1" colspan="3" valign="top"></td>
          </tr>
          <tr> 
            <td valign="top" background="skins/skin_(!SKINID!)/images/Grey.gif"></td>
            <td valign="top" background="skins/skin_(!SKINID!)/images/Grey.gif">
                <span class="OrangeTitles">Help &amp; Info</span><br/><br/>
				<table width="100%" cellpadding="4" cellspacing="0" border="0">
				<tr><td align="left" valign="top">
				<span class="InfoText">
				(!HELPBOX_CONTENTS!)
				<br/><br/>
				</span>
				</td></tr></table>
                </td>
            <td width="5" valign="top" background="skins/skin_(!SKINID!)/images/Grey.gif">&nbsp;</td>
          </tr>
          <tr> 
            <td height="1" colspan="3" valign="top"></td>
          </tr>
          <tr> 
            <td valign="top" background="skins/skin_(!SKINID!)/images/Grey.gif"></td>
            <td valign="top" background="skins/skin_(!SKINID!)/images/Grey.gif">
                <span class="OrangeTitles">Hello World XmlPackage</span><br/><br/>
				<table width="100%" cellpadding="4" cellspacing="0" border="0">
				<tr><td align="left" valign="top">
				<span class="InfoText">
				(!XmlPackage name="skin.helloworld.xml.config" version="2"!)
				<br/><br/>
				</span>
				</td></tr></table>
                </td>
            <td width="5" valign="top" background="skins/skin_(!SKINID!)/images/Grey.gif">&nbsp;</td>
          </tr>
          <tr> 
            <td height="100%" colspan="3" valign="top" background="skins/skin_(!SKINID!)/images/Grey.gif"></td>
          </tr>
          <tr> 
            <td height="17" colspan="3"><img src="skins/skin_(!SKINID!)/images/rounded_buttom.gif" width="179" height="17"/></td>
          </tr>
        </table>
<!-- END LEFT COL -->
      </td>
      <td width="621" align="left" valign="top">
<table width="100%" cellpadding="4" cellspacing="0" border="0">
<tr><td align="left" valign="top" width="100%" height="350">
<div align="left">
<span class="SectionTitleText">Now In: (!SECTION_TITLE!)</span>
</div>
<br/>
<!-- CONTENTS START -->
<asp:placeholder id="PageContent" runat="server"></asp:placeholder>
<!-- CONTENTS END -->
</td></tr></table>
      </td>
    </tr>
    <tr> 
      <td colspan="2"><div align="center" class="CartInfo">
              <br/><a href="default.aspx" class="foot">Home</a> | <a href="t-contact.aspx" class="foot">
                Contact Us</a> | <a href="t-affiliate.aspx" class="foot">Affiliates</a> | <a href="t-returns.aspx" class="foot">
                Return Policy</a> | <a href="t-privacy.aspx" class="foot">Privacy Policy</a> |
              <a href="t-security.aspx" class="foot">Security Policy</a> | <a href="sitemap2.aspx" class="foot">
                Site Map</a> | <a class="foot" href="t-copyright.aspx">Copyright &copy; 1995-2006. All Rights Reserved.</a>

<br/><br/><small>Powered by <a href="http://www.aspdotnetstorefront.com"  target="_blank" title="AspDotNetStoreFront.com">AspDotNetStorefront</a> <a  href="http://www.aspdotnetstorefront.com" target="_blank"  title="e-Commerce Shopping Cart">  E-Commerce Shopping  Cart</a></small><br/><br/>
      </div>
      </td>
    </tr>
  </table>
      </div>
    </div>
                                                                                                                                                                                 	<noscript>Powered by <a href="http://www.aspdotnetstorefront.com" target="_blank">AspDotNetStorefront E-Commerce Shopping Cart</a></noscript>
</body>
</html>
