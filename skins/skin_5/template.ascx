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
    <!-- PAGE INVOCATION: '(!INVOCATION!)' -->
    <!-- PAGE REFERRER: '(!REFERRER!)' -->
    <!-- STORE LOCALE: '(!STORELOCALE!)' -->
    <!-- CUSTOMER LOCALE: '(!CUSTOMERLOCALE!)' -->
    <!-- STORE VERSION: '(!STORE_VERSION!)' -->
    <!-- CACHE MENUS: '(!AppConfig name="CacheMenus"!)' -->
    <!-- to center your pages, set text-align: center -->
    <!-- to left justify your pages, set text-align: center -->
    <!-- if using dynamic full width page sizes, the left-right align has no effect (obviously) -->
    <div style="margin-top: 0px; margin-left: auto; text-align: center"> <!-- for dynamic full width page sizes, leave centerdiv width at 100% --> <!-- for fixed width page sizes, set centerdiv width:800px (or whatever you want) -->  <!-- to left align, set margin-right: auto; --> <!-- to center align, set margin-left: auto; margin-right: auto; -->
      <div id="centerdiv" style="margin-left:auto; width:800; margin-right:auto">
    <table width="800" border="0" cellspacing="0" cellpadding="0">
    <tr><td colspan="2" width="100%" valign="middle" align="right"><div style="visibility:(!COUNTRYDIVVISIBILITY!); display:(!COUNTRYDIVDISPLAY!);">Language: (!COUNTRYSELECTLIST!)&nbsp;</div><div style="visibility:(!CURRENCYDIVVISIBILITY!); display:(!CURRENCYDIVDISPLAY!);">Currency:(!CURRENCYSELECTLIST!)</div></td></tr>
    <tr> 
      <td colspan="2"><table width="771" border="0" cellspacing="0" cellpadding="0">
          <tr> 
            <td><table width="771" height="56" border="0" cellpadding="0" cellspacing="0">
                <tr> 
                  <td width="308" height="75" valign="middle"><a href="default.aspx"><img src="skins/skin_(!SKINID!)/images/logo.gif" border="0" alt="Home"/></a></td>
                  <td height="56" width="100%" align="right">
                      <img src="skins/skin_(!SKINID!)/images\topicon.gif" width="12" align="absMiddle" border="0">
                      <a href="shoppingcart.aspx" class="headblue">Checkout</a>&nbsp;&nbsp;&nbsp;&nbsp;
                      <img src="skins/skin_(!SKINID!)/images\topicon.gif" width="12" align="absMiddle" border="0">
                      <a href="wishlist.aspx" class="headblue">Wish List</a>&nbsp;&nbsp;&nbsp;&nbsp;
                      <img src="skins/skin_(!SKINID!)/images\topicon.gif" width="12" align="absMiddle" border="0">
                      <a href="giftregistry.aspx" class="headblue">Gift Registry</a>&nbsp;&nbsp;&nbsp;&nbsp;
                      <img src="skins/skin_(!SKINID!)/images\topicon.gif" width="12" align="absMiddle" border="0">
                      <a href="t-service.aspx" class="headblue">Customer Service</a>&nbsp;&nbsp;<br/>
                    <br/>
                    <span class="username">(!USERNAME!)&nbsp;&nbsp;<a class="headblue" href="(!SIGNINOUT_LINK!)">(!SIGNINOUT_TEXT!)</a>&nbsp;&nbsp;<br/>
                    You have (!NUM_CART_ITEMS!) item(s) in your <a class="headblue" href="shoppingcart.aspx">(!CARTPROMPT!)</a>&nbsp;&nbsp;</span></td>
                </tr>
              </table></td>
          </tr>
          		<tr>
			<td style="padding: 0px" width="100%" bgColor="#EEEEEE" height="32">
			<table style="border-collapse: collapse" borderColor="#111111" height="32" cellSpacing="0" cellPadding="0" width="100%" border="1" id="table5">
					<tr>
						<td width="60%">
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
						<td align="right" width="40%">
						<form style="cursor: hand; margin-top: 0px; margin-bottom: 0px;" id="topsearchform" name="topsearchform" method="GET" action="search.aspx">
						<nobr><font color=444444><b>Find It Now: </b></font><input name="SearchTerm" type="text" class="searchBox" size="25"><input type="submit" value="GO"></nobr>
						</form>
						</td>
					</tr>
			</table>
			</td>
		  </tr>
		  </table>
		  </td>
		  </tr>
    <tr> 
      <td width="179" valign="top" height="100%">
<!-- START LEFT COL -->
			<table width="179" height="100%" bgcolor="#FFFFFF" bordercolor="#FFFFFF" border="1" cellspacing="0" cellpadding="0">
          <tr><td height="1" colspan="3" valign="top"></td></tr>
          <tr><td height="1" colspan="3" valign="top"></td></tr>
		  <tr>
	        <td colspan="2" valign="top" bgcolor="#EEEEEE" bordercolor="#111111" align="left">
<!-- LEFT TREE CONTROL -->
<span class="BlueTitles">&nbsp;Browse By</span><br/><br/>
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
		  <tr><td height="1" colspan="3" valign="top"></td></tr>
          <tr> 
            <td colspan="2" valign="top" bordercolor="#111111" border="1" bgcolor="#EEEEEE">
            <span class="BlueTitles">&nbsp;Browse By (!CATEGORY_PROMPT_SINGULAR!)</span><br/><br/>
			(!ADVANCED_CATEGORY_BROWSE_BOX!)
			<br/>
            </td>
          </tr>
          <tr><td height="1" colspan="3" valign="top"></td></tr>
          <tr><td height="1" colspan="3" valign="top"></td></tr>
          <tr><td height="1" colspan="3" valign="top"></td></tr>
          <tr><td height="1" colspan="3" valign="top"></td></tr>
          <tr>
            <td valign="top" colspan="2" bgcolor="#EEEEEE" bordercolor="#111111">
                <span class="BlueTitles">&nbsp;Help &amp; Info</span><br/><br/>
				<table width="100%" cellpadding="4" cellspacing="0" border="0">
				<tr><td align="left" valign="top">
				<span class="InfoText">
				(!HELPBOX_CONTENTS!)
				<br/><br/>
				</span>
				</td></tr></table>
                </td>
            <td width="5" valign="top" bgcolor="#FFFFFF">&nbsp;</td>
          </tr>
          <tr><td height="1" colspan="3" valign="top"></td></tr>
          <tr><td height="1" colspan="3" valign="top"></td></tr>
          <tr>
            <td valign="top" colspan="2" bgcolor="#EEEEEE" bordercolor="#111111">
                <span class="BlueTitles">&nbsp;Hello World XmlPackage</span><br/><br/>
				<table width="100%" cellpadding="4" cellspacing="0" border="0">
				<tr><td align="left" valign="top">
				<span class="InfoText">
				(!XmlPackage name="skin.helloworld.xml.config" version="2"!)
				<br/><br/>
				</span>
				</td></tr></table>
                </td>
            <td width="5" valign="top" background="#EEEEEE">&nbsp;</td>
          </tr>
          <tr> 
            <td height="100%" colspan="3" valign="top" background="#EEEEEE"></td>
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
      <td colspan="2"><br/><hr size="1"/><div align="center" class="CartInfo">
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
