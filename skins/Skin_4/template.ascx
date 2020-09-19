<%@ Control Language="c#" AutoEventWireup="false" Inherits="AspDotNetStorefront.TemplateBase" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<html>
<head>
<title>(!METATITLE!)</title>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<meta NAME="description" CONTENT="(!METADESCRIPTION!)">
<meta NAME="keywords" CONTENT="(!METAKEYWORDS!)">
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/style.css" type="text/css">
<script type="text/javascript" src="jscripts/formValidate.js"></script>
<link rel="alternate" type="application/rss+xml" title="ROR" href="rorindex.aspx" />
</head>
<body rightmargin="0" leftmargin="0" topmargin="0" marginwidth="0" marginheight="0" bgcolor="#ffffff">
(!XmlPackage Name="skin.adminalert.xml.config"!)
    <!-- PAGE INVOCATION: '(!INVOCATION!)' -->
    <!-- PAGE REFERRER: '(!REFERRER!)' -->
    <!-- STORE LOCALE: '(!STORELOCALE!)' -->
    <!-- CUSTOMER LOCALE: '(!CUSTOMERLOCALE!)' -->
    <!-- STORE VERSION: '(!STORE_VERSION!)' -->
    <!-- CACHE MENUS: '(!AppConfig name="CacheMenus"!)' -->
    <!-- to center your pages, set text-align: center -->
    <!-- to left justify your pages, set text-align: center -->
    <!-- if using dynamic full width page sizes, the left-right align has no effect (obviously) -->
    <div style="MARGIN-TOP: 0px; MARGIN-LEFT: 0px; TEXT-ALIGN: left"> <!-- for dynamic full width page sizes, leave centerDiv width at 100% --> <!-- for fixed width page sizes, set centerDiv width:800px (or whatever you want) -->  <!-- to left align, set margin-right: auto; --> <!-- to center align, set margin-left: auto; margin-right: auto; -->
      <div id="centerDiv" style="MARGIN-LEFT:0px; WIDTH:100%; MARGIN-RIGHT:auto">
        (!COUNTRYBAR!)
        <table width="100%" align="center" cellpadding="0" cellspacing="0" border="0">
          <tr>
            <td colspan="3" height="3" width="100%"><img src="images/spacer.gif" width="100%" height="5"></td>
          </tr>
          <tr>
            <td colspan="3" width="100%" align="left" valign="top" height="60">
              <table width="100%" cellpadding="0" cellspacing="0" border="0" bgcolor="#ffffff">
                <tr>
                  <td width="316" height="60" align="left" valign="middle"><a href="default.aspx"><img alt="home page" src="skins/Skin_(!SKINID!)/images/logo.gif" border="0"></a></td>
                  <td align="right" valign="middle">(!VBV!)</td>
                  <td height="60" align="right" valign="middle">
                    <a href="default.aspx" class="head">Home</a> | <a href="shoppingcart.aspx" class="head">
                      Shopping Cart</a> | <a href="wishlist.aspx" class="head">My Wish List</a> | <a href="bestsellers.aspx" class="head">
                      Best Sellers</a> | <a href="t-affiliate.aspx" class="head">Affiliates</a> | <a href="t-service.aspx" class="head">
                      Customer Service</a> | <a href="search.aspx" class="head">Search</a>&nbsp;&nbsp;<br>
                    <br>
                    <span class="username">(!USERNAME!)&nbsp;&nbsp;<a class="username" href="(!SIGNINOUT_LINK!)">(!SIGNINOUT_TEXT!)</a>&nbsp;&nbsp;&nbsp;&nbsp;You 
                      have (!NUM_CART_ITEMS!) item(s) in your <a class="username" href="shoppingcart.aspx">
                        (!CARTPROMPT!)</a>&nbsp;&nbsp;</span>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td colspan="3" height="5" width="100%" background="skins/Skin_(!SKINID!)/images/topgradient.jpg"><img src="images/spacer.gif" width="100%" height="5"></td>
          </tr>
          <tr>
            <td colspan="3" width="100%" height="24" align="left" valign="middle" bgcolor="#cccccc">
              <table width="100%" cellpadding="0" cellspacing="0" border="0" bgcolor="#ffffff">
                <tr>
                  <td align="left" valign="middle" bgcolor="#cccccc" height="25">
<!-- START TOP MENU -->
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
          <tr>
            <td width="1" bgcolor="#cccccc"><img src="images/spacer.gif" width="1" height="1"></td>
            <td width="100%" height="25" align="left"
              valign="middle">
              <div id="SectionTitle" style="DISPLAY: block; VISIBILITY: visible">
                <table width="100%" cellpadding="0" cellspacing="0" border="0">
                  <tr>
                    <td align="left"><span class="SectionTitleText"><img align="absMiddle" src="images/spacer.gif" width="7" height="1">(!SECTION_TITLE!)</span></td>
                    <td align="right">&nbsp;</td>
                  </tr>
                </table>
              </div>
            </td>
            <td width="1" bgcolor="#cccccc"><img src="images/spacer.gif" width="1" height="1"></td>
          </tr>
          <tr>
            <td width="1" bgcolor="#cccccc"><img src="images/spacer.gif" width="1" height="1"></td>
            <td valign="top" align="center" bgcolor="#ffffff">
              <table width="100%" align="center" cellpadding="4" cellspacing="0">
                <tr>
                  <td align="left" valign="top" width="100%" height="400">
                    <table border="0" cellpadding="4" cellspacing="0" width="100%">
                      <tr>
                        <td width="100%" valign="top" align="left">
                          <table border="0" cellpadding="4" cellspacing="0" width="100%">
                            <tr>
								<!-- LEFT COL -->
                              <td valign="top" align="left">
								(!SEARCH_BOX!)
							    (!SECTION_BROWSE_BOX!)
							    (!CATEGORY_BROWSE_BOX!)
							    (!HELPBOX!)<br>
                              </td>
                                <!-- CENTER COL: -->
                              <td width="100%" valign="top" align="left">
                                <!-- CONTENTS START -->
                                <asp:placeholder id="PageContent" runat="server"></asp:placeholder>
                                <!-- CONTENTS END -->
                              </td>
                              <!-- RIGHT COL: -->
                              <td valign="top" align="left">
                              </td>
                            </tr>
                          </table>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </td>
            <td width="1" bgcolor="#cccccc"><img src="images/spacer.gif" width="1" height="1"></td>
          </tr>
          <tr>
            <td colspan="3" valign="middle" height="24" bgcolor="#cccccc" align="center">
              <a href="default.aspx" class="foot">Home</a> | <a href="t-contact.aspx" class="foot">
                Contact Us</a> | <a href="t-affiliate.aspx" class="foot">Affiliates</a> | <a href="t-returns.aspx" class="foot">
                Return Policy</a> | <a href="t-privacy.aspx" class="foot">Privacy Policy</a> |
              <a href="t-security.aspx" class="foot">Security Policy</a> | <a href="sitemap.aspx" class="foot">
                Site Map</a> | <a class="foot" href="t-copyright.aspx">Copyright &copy; 1995-2006. All Rights Reserved.</a>
            </td>
          </tr>
          <tr>
            <td colspan="3" height="5" width="100%" background="skins/Skin_(!SKINID!)/images/bottomgradient.jpg"></td>
          </tr>
          <tr>
            <td colspan="3" height="5" width="100%"><img src="images/spacer.gif" width="100%" height="5"></td>
          </tr>
          <tr>
            <td colspan="3" width="100%" align="center"><small>Powered by <a href="http://www.aspdotnetstorefront.com"  target="_blank" title="AspDotNetStoreFront.com">AspDotNetStorefront</a> <a  href="http://www.aspdotnetstorefront.com" target="_blank"  title="e-Commerce Shopping Cart">  E-Commerce Shopping  Cart</a></small></td>
          </tr>
          <tr>
            <td colspan="3" height="5" width="100%"><img src="images/spacer.gif" width="100%" height="5"></td>
          </tr>
        </table>
      </div>
    </div>
                                                                                                                                                                                 	<noscript>Powered by <a href="http://www.aspdotnetstorefront.com" target="_blank">AspDotNetStorefront E-Commerce Shopping Cart</a></noscript>
 </body>
</html>
