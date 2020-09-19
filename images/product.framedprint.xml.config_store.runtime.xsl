<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:aspdnsf="urn:aspdnsf" exclude-result-prefixes="aspdnsf">
  <xsl:output method="html" omit-xml-declaration="yes" />
  <xsl:param name="LocaleSetting" select="/root/Runtime/LocaleSetting" />
  <xsl:param name="WebConfigLocaleSetting" select="/root/Runtime/WebConfigLocaleSetting" />
  <xsl:param name="SecID">
    <xsl:choose>
      <xsl:when test="count(/root/QueryString/sectionid) &gt; 0">
        <xsl:value-of select="/root/QueryString/sectionid" />
      </xsl:when>
      <xsl:otherwise>0</xsl:otherwise>
    </xsl:choose>
  </xsl:param>
  <xsl:param name="CatID">
    <xsl:choose>
      <xsl:when test="count(/root/QueryString/categoryid) &gt; 0">
        <xsl:value-of select="/root/QueryString/categoryid" />
      </xsl:when>
      <xsl:otherwise>0</xsl:otherwise>
    </xsl:choose>
  </xsl:param>
  <xsl:param name="ManID">
    <xsl:choose>
      <xsl:when test="count(/root/QueryString/manufacturerid) &gt; 0">
        <xsl:value-of select="/root/QueryString/manufacturerid" />
      </xsl:when>
      <xsl:otherwise>0</xsl:otherwise>
    </xsl:choose>
  </xsl:param>
  <xsl:param name="CartRecID">
    <xsl:choose>
      <xsl:when test="/root/QueryString/cartrecid">
        <xsl:value-of select="/root/QueryString/cartrecid" />
      </xsl:when>
      <xsl:otherwise>0</xsl:otherwise>
    </xsl:choose>
  </xsl:param>
  <xsl:param name="frtype">
    <xsl:choose>
      <xsl:when test="/root/Form/frtype">
        <xsl:value-of select="/root/Form/frtype" />
      </xsl:when>
      <xsl:when test="/root/QueryString/frtype">
        <xsl:value-of select="/root/QueryString/frtype" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="'s'" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:param>
  <xsl:param name="mt1">
    <xsl:value-of select="/root/Form/mt1" />
  </xsl:param>
  <xsl:param name="ProductID" select="/root/Products/Product/ProductID" />
  <xsl:param name="VariantID" select="/root/Products/Product/VariantID" />
  <xsl:param name="SEName" select="/root/Products/Product/SEName" />
  <xsl:param name="pName">
    <xsl:value-of select="aspdnsf:GetMLValue(/root/Products/Product/Name)" disable-output-escaping="yes" /> 
      </xsl:param>
  <xsl:param name="pBase">
    <!--xsl:value-of select="/root/ProductsSections/ProductSections[SectionID=/root/ProductsSections/ProductSections[ProductID=$ProductID]/ParentSectionID]/Name" />&#160;-->
  </xsl:param>
  <xsl:param name="pUnit">
    <xsl:value-of select="/root/ProductsSections/ProductSections[ProductID=$ProductID]/Name" />
  </xsl:param>
  <xsl:param name="pDescription" select="aspdnsf:GetMLValue(/root/Products/Product/Description)">
  </xsl:param>
  <xsl:param name="HidePriceUntilCart" select="/root/Products/Product/HidePriceUntilCart" />
  <xsl:template match="/">
    <xsl:choose>
      <xsl:when test="/root/Products/Product/IsAKit='0'">
        <b>This XMLPackage is designed to work with Kit products only</b>
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates select="/root/Products/Product">
        </xsl:apply-templates>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template match="Product">
    <xsl:param name="EntityID">
      <xsl:value-of select="/root/Runtime/EntityID" />
    </xsl:param>
    <xsl:param name="EntityName">
      <xsl:value-of select="/root/Runtime/EntityName" />
    </xsl:param>
    <xsl:variable name="vKitMessage" select="aspdnsf:CustomQuickKitMessage('True', ProductID, price, saleprice, HidePriceUntilCart, Colors, $CartRecID, TaxClassID)">
    </xsl:variable>
    <xsl:variable name="vCustomPrice" select="aspdnsf:GetKitCustomPrice(ProductID, price, saleprice, ExtendedPrice,HidePriceUntilCart, Colors, $CartRecID, TaxClassID)">
    </xsl:variable>
    <script type="text/javascript">
          PrintPrice=<xsl:value-of select="$vCustomPrice" /></script>
    <!--link rel="stylesheet" type="text/css" href="jscripts/yui/fonts/fonts-min.css" /-->
    <link rel="stylesheet" type="text/css" href="jscripts/yui/tabview/assets/skins/sam/tabview.css" />
    <script type="text/javascript" src="jscripts/utils.js">
    </script>
    <script type="text/javascript" src="jscripts/yui/yahoo-dom-event/yahoo-dom-event.js">
    </script>
    <script type="text/javascript" src="jscripts/yui/element/element-beta.js">
    </script>
    <script type="text/javascript" src="jscripts/yui/tabview/tabview.js">
    </script>
    <script type="text/javascript" src="jscripts/yui/animation.js">
    </script>
    <script type="text/javascript" src="jscripts/fe.js">
    </script>
    <link rel="stylesheet" href="skins/Skin_(!SKINID!)/miniframes.css" type="text/css" />
    <link rel="stylesheet" href="skins/Skin_(!SKINID!)/minimats.css" type="text/css" />
    <xsl:value-of select="aspdnsf:GetJSPopupRoutines()" disable-output-escaping="yes" />
    <xsl:value-of select="aspdnsf:GenerateFramedAttrScript($CartRecID)" disable-output-escaping="yes" />
    <div class="prodHeader">
      <div style="float:left;">
        <xsl:value-of select="$pName" />
        <xsl:value-of select="$pBase" />
        <xsl:value-of select="$pUnit" />
      </div>
      <div style="text-align:right;float:right;">
        <xsl:value-of select="aspdnsf:ProductNavLinks($ProductID, /root/Runtime/EntityID, /root/Runtime/EntityName, /root/EntityHelpers/*[name()=/root/Runtime/EntityName]/descendant::Entity[EntityID=/root/Runtime/EntityID]/SEName, 0, 1, 1)" disable-output-escaping="yes" />
      </div>
    </div>
    <BR />
    <div class=" yui-skin-sam" id="top_product" style="width:100%;float:none;">
      <div style="width:530px;float:left;margin-right:12px;">
        <div align="center" style="background-color:#f4f5e9;width:530px;vertical-align: middle;border:solid 1px #BBBBBB;">
          <BR />
          <img id="framedImage" src="" border="0" />
          <br />
          <br />
        </div>
        <xsl:if test="$frtype='c' or $frtype='s'">
          <div style="width:530px;border:solid 1px #BBBBBB;">
            <table border="0" cellpadding="6px" cellspacing="0" width="100%">
              <tr>
                <td align="left" valign="top" width="500px">
                  <xsl:call-template name="PrintInfoTable" />
                  <br />
                  <div align="left">
                    <xsl:value-of select="aspdnsf:Decode($pDescription)" disable-output-escaping="yes" />
                  </div>
                </td>
              </tr>
              <tr valign="top">
                <td height="10">
                </td>
              </tr>
            </table>
          </div>
          <div style="width:530px;border:solid 1px #BBBBBB;background-color:#f4f5e9;">
            <div style="padding:5px;">
              <span class="fs_TotalPrice">
                    Total: <span id="TotalPrice" /></span> 
                  <xsl:if test="$frtype='c'"><a href="javascript:togglePriceDetails();">Click For Price and Size Details</a><div id="PriceDetails" style="height:0px; overflow:hidden; clear:both;"><div id="pricing_table_wrapper" style=""><div><table id="DetailsTable" cellpadding="0" cellspacing="0" style="border-collapse:collapse;"><tbody><tr><td></td></tr></tbody></table></div></div><script type="text/javascript">
                        initPriceDetails();
                      </script></div></xsl:if><xsl:for-each select="/root/KitItems/Item"><xsl:variable name="vName" select="aspdnsf:GetMLValue(Name)"></xsl:variable><xsl:variable name="vDescription" select="aspdnsf:GetMLValue(Description)"></xsl:variable></xsl:for-each><div style="height:30px;margin-top:5px;"><div align="center" style="width:300px;float:left"><xsl:call-template name="AddToCartForm"><xsl:with-param name="pKitMessage" select="$vKitMessage" /><xsl:with-param name="pCustomPrice" select="$vCustomPrice" /></xsl:call-template></div><div align="right" style="width:200px;float:right"><div class="buttonwrapper"><a class="ovalbutton" href="javascript:setFrameMode('p');"><span>Purchase print only</span></a></div></div></div></div>
          </div>
        </xsl:if>
      </div>
      <!--<div id="frame_mode" class="yui-navset" style="width:350px;float:left;margin-bottom:0px;">
           
            <a name="SelectedFrameButton" id="SelectedFrameButton" href="javascript:setFrameMode('')">Print Only</a>
           &#160;
           <a name="SelectedFrameButton" id="SelectedFrameButton" href="javascript:setFrameMode('s')">Select Framing</a>
           
            &#160;<a name="CustomFrameButton" id="CustomFrameButton" href="javascript:setFrameMode('c')">Custom Framing</a>
          </div>-->
      <xsl:choose>
        <xsl:when test="$frtype='c' or $frtype='s'">
          <xsl:call-template name="framedprinttab" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:call-template name="printonlytab">
            <xsl:with-param name="pKitMessage" select="$vKitMessage" />
            <xsl:with-param name="pCustomPrice" select="$vCustomPrice" />
          </xsl:call-template>
        </xsl:otherwise>
      </xsl:choose>
      <!-- End Framing modes-->
    </div>
    <br clear="all" />
    <div id="bottom" style="width:100%;">
      <table border="0" cellpadding="0" cellspacing="4" width="100%">
        <tr>
          <td align="center" valign="top" width="40%">
            <script type="text/javascript">
                  frtype='<xsl:value-of select="$frtype" />';
                  fixpr1='<xsl:value-of select="$ProductID" />';
                  $("frtype").value=frtype;
                  if (frtype=='c'||frtype=='s')
                  { //Framed
                  var frameOptionsView;
                  var matsView;
                  (function()
                  {
                  frameOptionsView = new YAHOO.widget.TabView('frame_options');
                  matsView = new YAHOO.widget.TabView('mat_tabs');
                  })();
                  addLoadListener(initFrameEngine);
                  }
                  else
                  { forceRefresh();
                  }
                </script>
          </td>
        </tr>
      </table>
      <hr size="1" color="#666666" />
      <div>
        <br />
        <xsl:value-of select="aspdnsf:RelatedProducts(ProductID)" disable-output-escaping="yes" />
        <xsl:value-of select="aspdnsf:ProductSpecs(ProductID, 1, SpecsInline, SpecCall, 400)" disable-output-escaping="yes" />
        <xsl:value-of select="aspdnsf:ProductRatings(ProductID, 0, 0, 0, 1)" disable-output-escaping="yes" />
      </div>
    </div>
  </xsl:template>
  <xsl:template name="AddToCartForm">
    <xsl:param name="pKitMessage" />
    <xsl:param name="pCustomPrice" />
    <xsl:param name="q">
      <xsl:if test="$CartRecID&gt;0">
        <xsl:value-of select="'?CartRecID='" />
        <xsl:value-of select="$CartRecID" />
      </xsl:if>
    </xsl:param>
    <form method="post" enctype="multipart/form-data" name="updatekit">
      <xsl:attribute name="action">
        <xsl:value-of select="aspdnsf:ProductLink($ProductID, $SEName, 0, '')" />
        <xsl:value-of select="$q" />
      </xsl:attribute>
      <input type="hidden" name="IsKitSubmit" value="false" />
      <input type="hidden" name="CartRecID" value="{$CartRecID}" />
      <!--input type="hidden" name="ProductID" id="ProductID" value="{$ProductID}" /-->
      <input type="hidden" name="framed" id="framed" value="yes" />
      <input type="hidden" name="frtype" id="frtype" value="s" />
      <input type="hidden" name="fr" id="fr" value="0" />
      <input type="hidden" name="frp" id="frp" value="0" />
      <input type="hidden" name="pr1" id="pr1" value="{$ProductID}" />
      <input type="hidden" name="FrameName" id="FrameName" value="" />
      <!--input type="hidden" name="MatName" id="MatName" value="" /-->
      <input type="hidden" name="smt" id="smt" value="0" />
      <input type="hidden" name="mt1" id="mt1" value="0" />
      <input type="hidden" name="mt2" id="mt2" value="0" />
      <input type="hidden" name="mt3" id="mt3" value="0" />
      <input type="hidden" name="MatN" id="MatN" value="1" />
      <input type="hidden" name="mtt1" id="mtt1" value="2.5" />
      <input type="hidden" name="mtb1" id="mtb1" value="2.5" />
      <input type="hidden" name="mtl1" id="mtl1" value="2.5" />
      <input type="hidden" name="mtr1" id="mtr1" value="2.5" />
      <input type="hidden" name="mtt2" id="mtt2" value="0.25" />
      <input type="hidden" name="mtb2" id="mtb2" value="0.25" />
      <input type="hidden" name="mtl2" id="mtl2" value="0.25" />
      <input type="hidden" name="mtr2" id="mtr2" value="0.25" />
      <input type="hidden" name="mtt3" id="mtt3" value="0.25" />
      <input type="hidden" name="mtb3" id="mtb3" value="0.25" />
      <input type="hidden" name="mtl3" id="mtl3" value="0.25" />
      <input type="hidden" name="mtr3" id="mtr3" value="0.25" />
      <input type="hidden" name="gl" id="gl" value="0" />
      <input type="hidden" name="glo" id="glo" value="0" />
      <input type="hidden" name="GlazeName" id="GlazeName" value="" />
      <input type="hidden" name="FinishedPrice" id="FinishedPrice" value="{$pCustomPrice}" />
      <input type="hidden" name="FinishedWeight" id="FinishedWeight" value="" />
      <xsl:choose>
        <xsl:when test="$HidePriceUntilCart=0">
          <xsl:choose>
            <xsl:when test="$CartRecID=0">
              <xsl:call-template name="AddToCartButton" />
            </xsl:when>
            <xsl:otherwise>
              <div class="buttonwrapper">
                <a class="ovalbutton" href="javascript:document.updatekit.IsKitSubmit.value='true';document.updatekit.submit();">
                  <span>Update and Return to Cart</span>
                </a>
              </div>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:when>
        <xsl:otherwise>
          <xsl:call-template name="AddToCartButton" />
        </xsl:otherwise>
      </xsl:choose>
    </form>
  </xsl:template>
  <xsl:template name="AddToCartButton">
    <div align="left">
      <div>
        <script language="JavaScript" type="text/javascript">
var VariantMinimumQty = 0;
function updatekit_Validator(theForm)
	{
	submitonce(theForm);
	if ((theForm.Quantity.value*1)&lt;1)
	{
		alert("Please specify the quantity you want to add to your cart");
		theForm.Quantity.focus();
		submitenabled(theForm);
		return (false);
    }
	submitenabled(theForm);
	return (true);
	}
   
   function addToCartClick()
   { document.updatekit.IsWishList.value='0';document.updatekit.IsGiftRegistry.value='0';if (validateForm(document.updatekit) &amp;&amp; updatekit_Validator(document.updatekit)) { if (document.updatekit.IsKitSubmit) document.updatekit.IsKitSubmit.value='true';document.updatekit.submit();};
   }
 </script>
        <input type="hidden" value="0" id="VariantStyle" name="VariantStyle" />
        <input type="hidden" value="0" id="IsWishList" name="IsWishList" />
        <input type="hidden" value="0" id="IsGiftRegistry" name="IsGiftRegistry" />
        <input type="hidden" value="" id="UpsellProducts" name="UpsellProducts" />
        <input type="hidden" value="{$VariantID}" id="VariantID" name="VariantID" />
        <input type="hidden" value="[req][number][blankalert=Please enter a quantity][invalidalert=Please enter a number for the quantity]" name="Quantity_vldt" />
        <div class="buttonwrapper">
          <div style="float:left;">
            <span>Quantity: </span>
            <input type="text" maxlength="4" size="3" id="Quantity" name="Quantity" value="1" /> 
      </div>
          <a class="ovalbutton" href="javascript:addToCartClick();">
            <span>Add to Cart</span>
          </a>
        </div>
      </div>
      <p />
    </div>
  </xsl:template>
  <xsl:template name="CustomMats">
    <ul class="fs_number_mats">
      <li>
        <input onclick="setMatN(1);lazyRefresh();" id="mat1rb" name="mat1rb" type="radio" selected="selected" />
            1 Mat
          </li>
      <li>
        <input onclick="setMatN(2);lazyRefresh();" id="mat2rb" name="mat1rb" type="radio" />
            2 Mat
          </li>
      <li>
        <input onclick="setMatN(3);lazyRefresh();" id="mat3rb" name="mat1rb" type="radio" />
            3 Mat
          </li>
      <li>
        <input onclick="setMatN(0);lazyRefresh();" id="mat0rb" name="mat1rb" type="radio" />
            No Mat
          </li>
    </ul>
    <div id="mat_tabs" class="yui-navset" style="width:100%;float:right;">
      <ul class="yui-nav">
        <li class="selected">
          <a href="#mat1">
            <em>Top Mat</em>
          </a>
        </li>
        <li>
          <a href="#mat1">
            <em>Middle Mat</em>
          </a>
        </li>
        <li>
          <a href="#mat1">
            <em>Bottom Mat</em>
          </a>
        </li>
      </ul>
      <div class="yui-content" style="border:none;">
      </div>
    </div>
    <div id="mat1">
      <span id="asyncBeginFrames" class="display: none;">
      </span>
      <div class="clear" style="padding-top: 1px;">
      </div>
      <div style="float:left;margin-left:10px;">Mat Name: </div>
      <div id="MatName"> </div>
      <br />
      <div style="float:left;margin-left:10px;">Mat Width: </div>
      <div id="MatWidth" style="text-align:center;float:left;width:45px;"> </div>
      <a href="javascript:changeMatWidth(-1);lazyRefresh();" style="margin:1px;">
        <img src="fs/images/minus_sm.gif" />
      </a>
      <a href="javascript:changeMatWidth(+1);lazyRefresh();" style="margin:1px;">
        <img src="fs/images/plus_sm.gif" />
      </a> (Decrease/Increase mat size)
          <div style="overflow-x: hidden; overflow-y:auto; height: 448px;" id="matContainer"><div><xsl:for-each select="/root/Mats/Mat"><xsl:variable name="vMatID" select="MatID"></xsl:variable><xsl:variable name="vSKU" select="SKU"></xsl:variable><div class="fs_matbox" style="border:solid 2px #EDF5FF" id="mat_box_{$vMatID}" onmouseout="javascript:mat_box_out({$vMatID});" onmouseover="javascript:mat_box_over({$vMatID});"><div class="fs_icon"><a href="javascript:smt({$vMatID},'{Name}');"><xsl:choose><xsl:when test="Color"><div class="fs_colormat" style="background-color:{Color}" /></xsl:when><xsl:otherwise><img width="23" height="23" alt="Click to try this mat" title="Click to try this mat" src="images/Mat/{$vSKU}/choose_icon.jpg" border="0" /></xsl:otherwise></xsl:choose></a></div></div></xsl:for-each></div></div></div>
  </xsl:template>
  <xsl:template name="SelectMats">
    <xsl:for-each select="/root/MatSizes/Size">
      <script type="text/javascript">
            def_selmat_rl1=<xsl:value-of select="RightLeft1" />;
            def_selmat_tb1=<xsl:value-of select="TopBottom1" />;
            def_selmat_rl2=<xsl:value-of select="RightLeft2" />;
            def_selmat_tb2=<xsl:value-of select="TopBottom2" />;
          </script>
    </xsl:for-each>
    <div id="mat1">
      <span id="asyncBeginFrames" class="display: none;">
      </span>
      <div class="clear" style="padding-top: 1px;">
      </div>
      <div style="float:left;margin-left:10px;">Mat Name: </div>
      <div id="MatName"> </div>
      <br />
      <div style="overflow-x: hidden; overflow-y:auto; height: 448px;" id="matContainer">
        <div>
          <xsl:for-each select="/root/Mats/Mat">
            <xsl:variable name="vMatID" select="MatID">
            </xsl:variable>
            <xsl:variable name="vSKU" select="SKU">
            </xsl:variable>
            <div class="fs_matbox" style="border:solid 2px #EDF5FF" id="mat_box_{$vMatID}" onmouseout="javascript:mat_box_out({$vMatID});" onmouseover="javascript:mat_box_over({$vMatID});">
              <a href="javascript:ssmt({$vMatID},'{Name}',{Mat1ID},{Mat2ID});">
                <div class="fs_pairmat" style="background-color:{Color1}">
                  <div style="background-color:{Color2}">
                    <div>
                    </div>
                  </div>
                </div>
              </a>
            </div>
            <xsl:if test="position()=1">
              <script type="text/javascript">
                    setSelectMat(<xsl:value-of select="$vMatID" />,'<xsl:value-of select="Name" />',<xsl:value-of select="Mat1ID" />,<xsl:value-of select="Mat2ID" />);
                  </script>
            </xsl:if>
          </xsl:for-each>
        </div>
        <div class="clear" />
        <div style="margin:4px;">Mats are included in frame price</div>
      </div>
    </div>
  </xsl:template>
  <xsl:template name="printonlytab">
    <xsl:param name="pKitMessage" />
    <xsl:param name="pCustomPrice" />
    <div id="frame_options" class="yui-navset" style="width:350px;float:left;margin-right:3px;">
      <span class="fs_PrintName">
        <xsl:value-of select="aspdnsf:Decode($pName)" disable-output-escaping="yes" /> Print
          </span>
      <br />
      <xsl:call-template name="PrintInfoTable" />
      <br />
      <br />
      <span class="fs_TotalPrice">
              Total: <b>
            $<xsl:value-of select="$pCustomPrice" /></b></span> 
          <br /><div style="text-align:left;margin-top:5px;"><xsl:call-template name="AddToCartForm"><xsl:with-param name="pKitMessage" select="$pKitMessage" /><xsl:with-param name="pCustomPrice" select="$pCustomPrice" /></xsl:call-template><br /><div class="buttonwrapper"><a class="ovalbutton" href="javascript:javascript:setFrameMode('s');"><span>Frame this print</span></a></div><br /><br /><br /><br /><div align="left"><xsl:value-of select="aspdnsf:Decode($pDescription)" disable-output-escaping="yes" /></div></div></div>
  </xsl:template>
  <xsl:template name="framedprinttab">
    <div id="frame_options" class="yui-navset" style="width:350px;float:left;">
      <a style="float:right;" name="SelectedFrameButton" id="SelectedFrameButton" href="javascript:setFrameMode('p')">
        <b>Print Only</b>
      </a>
      <ul class="yui-nav">
        <li class="selected">
          <a href="#tab_frame">
            <em>Frame</em>
          </a>
        </li>
        <li>
          <a href="#tab_mat">
            <em>Mats</em>
          </a>
        </li>
        <xsl:if test="$frtype='c'">
          <li>
            <a href="#tab_glazzing">
              <em>Other Options</em>
            </a>
          </li>
        </xsl:if>
      </ul>
      <div class="yui-content">
        <div id="tab_frame" style="height:500px;">
          <div class="clear" style="padding-top: 1px;">
          </div>
          <span id="asyncBeginFrames" class="display: none;">
          </span>
          <div style="overflow-x: hidden; overflow-y: scroll; height: 499px;" id="frameContainer">
            <div class="fs_choosenarea">
              <script type="text/javascript">
                  frameprices=[];
                  frameweights=[];
                </script>
              <xsl:for-each select="/root/Frames/Frame">
                <xsl:variable name="vFrameID" select="FrameID">
                </xsl:variable>
                <xsl:variable name="vSKU" select="SKU">
                </xsl:variable>
                <xsl:variable name="vName" select="aspdnsf:GetMLValue(Name)">
                </xsl:variable>
                <div class="fs_framebox">
                  <div style="border:solid 2px #FFFFFF" id="frame_box_{$vFrameID}" onmouseout="javascript:frame_box_out({$vFrameID});" onmouseover="javascript:frame_box_over({$vFrameID});">
                    <div class="fs_icon">
                      <a href="javascript:sfr({$vFrameID},'{$vName}');">
                        <img width="115" height="115" alt="Click to try this frame" title="Click to try this frame" src="images/Frame/{$vSKU}/choose_icon.jpg" border="0" />
                      </a>
                    </div>
                    <div class="fs_data">
                      <div class="fs_label">
                        <xsl:value-of select="Name" /> - <xsl:value-of select="format-number(LengthWide, '#0.00')" />"
                        </div>
                      <div class="fs_action">
                        <a href="javascript:sfr({$vFrameID},'{$vName}');">
                          <b> Select this frame</b>
                        </a>
                      </div>
                      <xsl:if test="Price">
                        <script type="text/javascript">
                            frameprices['<xsl:value-of select="$vFrameID" />']=<xsl:value-of select="Price" />;
                            frameweights['<xsl:value-of select="$vFrameID" />']=<xsl:value-of select="DeltaWeight" />;
                          </script>
                        <div class="fs_frameprice">
                            Frame Price: $<xsl:value-of select="format-number(Price, '##.00')" /></div>
                      </xsl:if>
                    </div>
                  </div>
                </div>
                <xsl:if test="position()=1">
                  <script type="text/javascript">
                      setFrame(<xsl:value-of select="$vFrameID" />,'<xsl:value-of select="$vName" />');
                    </script>
                </xsl:if>
              </xsl:for-each>
            </div>
          </div>
        </div>
        <div id="tab_mat">
          <div class="clear" style="padding-top: 1px;">
          </div>
          <xsl:choose>
            <xsl:when test="$frtype='c'">
              <xsl:call-template name="CustomMats">
              </xsl:call-template>
            </xsl:when>
            <xsl:when test="$frtype='s'">
              <xsl:call-template name="SelectMats">
              </xsl:call-template>
            </xsl:when>
          </xsl:choose>
        </div>
        <xsl:if test="$frtype='c'">
          <div id="tab_glazzing">
            <div class="clear" style="padding-top: 1px;">
            </div>
            <div style="overflow-x: hidden; overflow-y:auto; height: 448px;" id="matContainer">
              <span>Glazing Type</span>
              <ul class="fs_glazing">
                <li>
                  <input onclick="javascript:setGlaze(0);priceRefresh();" id="glaze_rb0" name="glaze_rb" type="radio" selected="selected" />No Glass
                  </li>
                <xsl:for-each select="/root/Glazes/Glaze">
                  <xsl:variable name="vGlazeID" select="GlazeID">
                  </xsl:variable>
                  <xsl:variable name="vSKU" select="SKU">
                  </xsl:variable>
                  <li>
                    <input onclick="javascript:setGlaze({$vGlazeID},'{Name}');priceRefresh();" id="glaze_rb{$vGlazeID}" name="glaze_rb" type="radio" />
                    <xsl:value-of select="Name" />
                  </li>
                </xsl:for-each>
              </ul>
            </div>
          </div>
        </xsl:if>
      </div>
    </div>
  </xsl:template>
  <xsl:template name="PrintInfoTable">
    <div class="ProductNameText">
      <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr>
          <td align="left" nowrap="nowrap" width="90px">
              Aircraft Name:
            </td>
          <td align="left">
            <b>
              <xsl:value-of select="aspdnsf:Decode($pName)" disable-output-escaping="yes" />
            </b>
          </td>
        </tr>
        <xsl:if test="string-length($pBase)&gt;0">
          <tr>
            <td align="left">
                Based at:
              </td>
            <td align="left">
              <b>
                <xsl:value-of select="aspdnsf:Decode($pBase)" disable-output-escaping="yes" />
              </b>
            </td>
          </tr>
        </xsl:if>
        <tr>
          <td align="left">
              Unit:
            </td>
          <td align="left">
            <b>
              <xsl:value-of select="aspdnsf:Decode($pUnit)" disable-output-escaping="yes" />
            </b>
          </td>
        </tr>
        <tr>
          <td align="left">
              Print #:
            </td>
          <td align="left">
            <b>
              <xsl:value-of select="SKU" />
            </b>
          </td>
        </tr>
      </table>
    </div>
  </xsl:template>
</xsl:stylesheet>
