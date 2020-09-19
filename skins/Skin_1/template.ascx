<%@ Control Language="c#" AutoEventWireup="false" Inherits="AspDotNetStorefront.TemplateBase" TargetSchema="https://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "https://www.w3c.org/TR/1999/REC-html401-19991224/loose.dtd">
<!-- AB addeed to turn off scripts on-->
<script language="c#" runat="server">
protected override void Page_Load(Object Src, EventArgs E)
{
      base.Page_Load(Src, E);
      if (Request.IsLocal)
      {
          LiveScripts.Visible = false;
      }
}
</script>
<html>
<head>
<link rel="Shortcut Icon" href="favicon.ico" type="image/x-icon" />
<meta name="verify-v1" content="AhXI6Ow3jT/jcQKVq8CL6INp1XUtE6NDE4aGTdkL9CU=" />
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<script type="text/javascript" src="jscripts/revAjax.js"></script>
<link type="text/css" href="css/redmond/jquery-ui-1.8.4.custom.css" rel="Stylesheet" />	
<script type="text/javascript" src="js/jquery-1.4.2.min.js"></script>
<script type="text/javascript" src="js/jquery-ui-1.8.4.custom.min.js"></script>

<script type="text/javascript" src="jscripts/btn.js"></script>
<META HTTP-EQUIV="imagetoolbar" CONTENT="no">
<title>Squadron Graphics, Inc. - (!METATITLE!)</title>
(!CURRENCY_LOCALE_ROBOTS_TAG!)
<meta name="description" content="(!METADESCRIPTION!)">
<meta name="keywords" content="(!METAKEYWORDS!)">
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/style.css?t=1" type="text/css">
<script type="text/javascript" src="jscripts/formValidate.js"></script>
<script language="javascript" type="text/javascript">
//<![CDATA[
var cot_loc0=(window.location.protocol == "https:")? "https://secure.comodo.net/trustlogo/javascript/cot.js" :
"http://www.trustlogo.com/trustlogo/javascript/cot.js";
document.writeln('<scr' + 'ipt language="JavaScript" src="'+cot_loc0+'" type="text\/javascript">' + '<\/scr' + 'ipt>');
//]]>
</script>
<script type="text/javascript">
	$(function() {
		$("#datepicker").datepicker();
	});
	</script>
</head>
<body>
(!XmlPackage Name="skin.adminalert.xml.config"!)
(!PAGEINFO!)
    <div id="wrapper">
       <div id="login">
            <span id="userName">(!USERNAME!)</span><span id="loginText"><a href="(!SIGNINOUT_LINK!)">(!SIGNINOUT_TEXT!)</a></span></div>
        <div id="header">
			<a id="logo" href="default.aspx" title="squadrongraphics.com"><b>SquadronGraphics.com</b></a>
            <a class="wishlist" href="wishlist.aspx">Your Wishlist</a> <a class="cart" href="shoppingcart.aspx">
                Shopping Cart ((!NUM_CART_ITEMS!))</a> <a class="contact" href="t-contact.aspx">Contact
                    Us</a> <a class="account" href="account.aspx">Your Account</a></div>
        <div id="horizNav">
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
        </div>
        <div id="horizNav2">
            <form name="topsearchform" method="get" action="search.aspx">
                <fieldset>
					<label>Search:</label>
                    <input type="text" size="15" name="SearchTerm" class="searchBox" id="searchBox" autocomplete="off"
                        onfocus="javascript:this.style.background='#ffffff';" onBlur="javascript:this.style.background='#dddddd';" />
                    <input type="button" onClick="document.topsearchform.submit()" title="Click Go to Submit"
                        id="Go" class="submit" value="Go" /><br />
                </fieldset>
            </form>
            <ul class="tameHoriz">
                <li><a href="images/SQGI-Catalog.pdf" target="_blank"><B>Full Print Catalog</B> (PDF)</a><span class="pipe">|</span></li>
                <li><a href="images/GetPrintDone.pdf" target="_blank"><B>Get your unit print done!</B> (PDF)</a><span class="pipe">|</span></li>
                <li><a href="t-faq.aspx">FAQ</a><span class="pipe">|</span></li>
				<li><span>800.275.0986</span></li>
            </ul>
        </div>

        <div id="bodyWrapper">
        <div style="padding:2px;">
        	<div id="miniCart">You have (!NUM_CART_ITEMS!) item(s) in your <a class="username" href="shoppingcart.aspx">
					(!CARTPROMPT!)</a></div>
            <div id="ML">
                <div style="visibility: (!COUNTRYDIVVISIBILITY!); display: (!COUNTRYDIVDISPLAY!);">
                    Language: (!COUNTRYSELECTLIST!)</div>
                <div style="visibility: (!CURRENCYDIVVISIBILITY!); display: (!CURRENCYDIVDISPLAY!);">
                    Currency: (!CURRENCYSELECTLIST!)</div>
                <div style="visibility: (!VATDIVVISIBILITY!); display: (!VATDIVDISPLAY!);">
                    VAT Mode: (!VATSELECTLIST!)</div>
            </div>
			<div id="breadcrumb">
				Now In: (!SECTION_TITLE!)</div>
			<asp:PlaceHolder runat="server" ID="LeftColumnHolder" Visible="True">
			<div id="leftWrap">
				<div class="navHeader">
					Browse Categories</div>
				<div class="leftNav" id="categories">
					(!XmlPackage Name="rev.categories"!)
				</div>
                <div class="navHeader">
					Locate your print!
				</div>
				<div class="leftNav" id="Print Finder" align="center">
					<A HREF="printfinder.aspx"><IMG SRC="skins/Skin_(!SKINID!)/images/printfinder.jpg" width="135" height="50"/></A>
				</div>
                <div class="navHeader">
					Pilot Training Specials <img alt="" src="images/pilotwings.png" />
				</div>
				<div class="leftNav" id="UPT" align="center">
                    <p><a href="c-36-upt-class-prints.aspx"><img alt="Pilot Training Specials" border="0" width="90" height="90" src="images/specials.gif" /></a>            </p>
			  </div>
                
				<div id="new arrivals">
					(!XmlPackage Name="rev.newarrivals"!)
				</div>
			</div> 
            </asp:PlaceHolder>			
			<div id="content" (!CONTENTSTYLE!)>
				<!-- CONTENTS START -->
				<asp:PlaceHolder ID="PageContent" runat="server"></asp:PlaceHolder>
				<!-- CONTENTS END -->
			</div>
		
		</div>
		<div id="footer">
			<div id="footerWrap">
				<ul class="tameHoriz">
					<li><a href="t-about.aspx">About SquadronGraphics.com</a> |</li>
					<li><a href="t-faq.aspx">FAQ</a> |</li>
					<li><a href="t-contact.aspx">Contact Us</a></li>
				</ul>
				<ul class="tameHoriz">
					<li><a href="giftregistry.aspx">Gift Registry</a> |</li>
					<li><a href="wishlist.aspx">Wishlist</a> |</li>
					<li><a href="sitemap2.aspx">Site Map</a> |</li>
					<li><a href="t-privacy.aspx">Privacy Policy</a> |</li>
				</ul>
				<br />
				<ul class="tame">
					<li>&copy; Squadron Graphics, Inc. 2011. All Rights Reserved.</li>
				</ul>
			</div>
		</div>
			</div>
	</div>
	<div id="closer"></div>
<noscript></noscript>
<asp:PlaceHolder runat="server" ID="LiveScripts">
<a href="http://www.instantssl.com" id="comodoTL">Server SSL Certificate</a>
<script language="JavaScript" type="text/javascript">
COT("https://www.squadrongraphics.com/images/cornertrust.gif", "SC2", "none");
</script>
(!GOOGLE_ECOM_TRACKING!)
<script type="text/javascript">

  var _gaq = _gaq || [];
  _gaq.push(['_setAccount', 'UA-2594562-1']);
  _gaq.push(['_trackPageview']);

  (function() {
    var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
    ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'https://www') + '.google-analytics.com/ga.js';
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
  })();

</script>
</asp:PlaceHolder>
</body>
</html>
