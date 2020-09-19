<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:aspdnsf="urn:aspdnsf" exclude-result-prefixes="aspdnsf">
  <xsl:output method="html" omit-xml-declaration="yes" />
  <xsl:param name="LocaleSetting" select="/root/Runtime/LocaleSetting" />
  <xsl:param name="WebConfigLocaleSetting" select="/root/Runtime/WebConfigLocaleSetting" />
  <xsl:param name="ShowSubcatsInGrid">
    <xsl:value-of select="aspdnsf:AppConfig('ShowSubcatsInGrid')" />
  </xsl:param>
  <xsl:param name="SubcatGridCols">
    <xsl:value-of select="/root/EntityHelpers/*[name()=/root/Runtime/EntityName]/descendant::Entity[EntityID=/root/Runtime/EntityID]/ColWidth" />
  </xsl:param>
  <xsl:param name="EntityName">
    <xsl:value-of select="/root/Runtime/EntityName" />
  </xsl:param>
  <xsl:param name="EntityID">
    <xsl:value-of select="/root/Runtime/EntityID" />
  </xsl:param>
  <xsl:param name="WholesaleOnlySite">
    <xsl:value-of select="aspdnsf:AppConfigBool('WholesaleOnlySite')" />
  </xsl:param>
  <xsl:param name="BaseURL">
    <xsl:choose>
      <xsl:when test="aspdnsf:StrToLower(/root/Runtime/EntityName) = 'category'">c-<xsl:value-of select="/root/Runtime/EntityID" />-<xsl:value-of select="/root/QueryString/sename" />.aspx</xsl:when>
      <xsl:when test="aspdnsf:StrToLower(/root/Runtime/EntityName) = 'section'">s-<xsl:value-of select="/root/Runtime/EntityID" />-<xsl:value-of select="/root/QueryString/sename" />.aspx</xsl:when>
      <xsl:when test="aspdnsf:StrToLower(/root/Runtime/EntityName) = 'manufacturer'">m-<xsl:value-of select="/root/Runtime/EntityID" />-<xsl:value-of select="/root/QueryString/sename" />.aspx</xsl:when>
      <xsl:when test="aspdnsf:StrToLower(/root/Runtime/EntityName) = 'library'">l-<xsl:value-of select="/root/Runtime/EntityID" />-<xsl:value-of select="/root/QueryString/sename" />.aspx</xsl:when>
    </xsl:choose>
  </xsl:param>
  <xsl:param name="CurrentPage">
    <xsl:choose>
      <xsl:when test="/root/QueryString/pagenum">
        <xsl:value-of select="/root/QueryString/pagenum" />
      </xsl:when>
      <xsl:otherwise>1</xsl:otherwise>
    </xsl:choose>
  </xsl:param>
  <xsl:template match="/">
    <div>
      <xsl:value-of select="aspdnsf:EntityPageHeaderDescription($EntityName, $EntityID)" disable-output-escaping="yes" />
    </div>
    <xsl:value-of select="aspdnsf:EntityPageFilterOptions($EntityName, $EntityID, /root/Runtime/SecID, /root/Runtime/CatID, /root/Runtime/ManID, /root/Runtime/ProductTypeFilterID)" disable-output-escaping="yes" />
    <xsl:call-template name="SubEntity" />
    <xsl:choose>
      <xsl:when test="count(/root/Products/Product) = 0 and count(/root/EntityHelpers/*[name()=/root/Runtime/EntityName]/descendant::Entity[EntityID=/root/Runtime/EntityID]/Entity) = 0">
        <xsl:value-of select="aspdnsf:Topic(concat('empty', /root/Runtime/EntityName, 'text'))" disable-output-escaping="yes" />
      </xsl:when>
      <xsl:otherwise>
        <div style="text-align:right;">
          <xsl:value-of select="aspdnsf:PagingControl($BaseURL, $CurrentPage, /root/Products2/Product/pages)" disable-output-escaping="yes" />
        </div>
        <table border="0" cellpadding="0" cellspacing="4" width="100%">
          <xsl:apply-templates select="/root/Products/Product" />
        </table>
        <div style="text-align:right;">
          <xsl:value-of select="aspdnsf:PagingControl($BaseURL, $CurrentPage, /root/Products2/Product/pages)" disable-output-escaping="yes" />
        </div>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="SubEntity">
    <xsl:for-each select="/root/EntityHelpers/*[name()=/root/Runtime/EntityName]/descendant::Entity[ParentEntityID=/root/Runtime/EntityID]">
      <xsl:choose>
        <xsl:when test="$ShowSubcatsInGrid = 'true'">
          <table border="0" cellpadding="0" cellspacing="4" width="100%">
            <xsl:if test="position() mod $SubcatGridCols = 1 or ($SubcatGridCols = 1)">
              <tr>
                <xsl:for-each select=". | following-sibling::*[position() &lt; $SubcatGridCols]">
                  <xsl:variable name="scName" select="aspdnsf:GetMLValue(Name)">
                  </xsl:variable>
                  <xsl:call-template name="SubCatCell">
                    <xsl:with-param name="scName" select="$scName" />
                  </xsl:call-template>
                </xsl:for-each>
              </tr>
              <tr>
                <td height="10" colspan="{$SubcatGridCols}"> </td>
              </tr>
            </xsl:if>
          </table>
        </xsl:when>
        <xsl:otherwise>
          <xsl:variable name="scName" select="aspdnsf:GetMLValue(Name)">
          </xsl:variable>
          <p align="left">
                                   <img border="0"><xsl:attribute name="src">skins/skin_<xsl:value-of select="aspdnsf:SkinID()" />/images/redarrow.gif</xsl:attribute></img> 
                                <a href="{aspdnsf:EntityLink(EntityID, SEName, $EntityName, 0, '')}"><xsl:value-of select="$scName" disable-output-escaping="yes" /></a></p>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:for-each>
  </xsl:template>
  <xsl:template name="SubCatCell">
    <xsl:param name="scName">
    </xsl:param>
    <xsl:param name="URL">
      <xsl:value-of select="aspdnsf:EntityLink(EntityID, SEName, $EntityName, 0, '')" />
    </xsl:param>
    <td align="center">
      <a href="{$URL}">
        <xsl:value-of select="aspdnsf:LookupEntityImage(EntityID, $EntityName, 'icon', 0)" disable-output-escaping="yes" />
      </a>
      <br />
      <a href="{$URL}">
        <xsl:value-of select="$scName" disable-output-escaping="yes" />
      </a>
    </td>
  </xsl:template>
  <xsl:template match="Product">
    <xsl:if test="position() mod $SubcatGridCols = 1">
      <tr>
        <xsl:for-each select=". | following-sibling::*[position() &lt; $SubcatGridCols]">
          <xsl:call-template name="ProductCell">
          </xsl:call-template>
        </xsl:for-each>
      </tr>
    </xsl:if>
  </xsl:template>
  <xsl:template name="ProductCell">
    <xsl:param name="pName2" select="aspdnsf:GetMLValue(Name)">
    </xsl:param>
    <td align="center">
      <a href="{aspdnsf:ProductandEntityLink(ProductID, SEName, $EntityID, $EntityName, 0)}">
        <xsl:value-of select="aspdnsf:LookupImage(ProductID, 'product', 'icon', 1)" disable-output-escaping="yes" />
      </a>
      <br />
      <a href="{aspdnsf:ProductandEntityLink(ProductID, SEName, $EntityID, $EntityName, 0)}">
        <xsl:value-of select="$pName2" disable-output-escaping="yes" />
      </a>
    </td>
  </xsl:template>
</xsl:stylesheet>
