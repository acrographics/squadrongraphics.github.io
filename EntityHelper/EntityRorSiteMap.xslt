<?xml version="1.0" encoding="UTF-8" ?>
<!-- ###################################################################################################### -->
<!-- Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.					                -->
<!-- http://www.aspdotnetstorefront.com														                -->
<!-- For details on this license please visit  the product homepage at the URL above.		                -->
<!-- THE ABOVE NOTICE MUST REMAIN INTACT.                                                                   -->
<!-- $Header: /v6.2/Web/EntityHelper/EntityGoogleSiteMap.xslt 1     7/08/06 10:42p Admin $	   -->
<!-- ###################################################################################################### -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ror="http://rorweb.com/0.1/">
    <xsl:output method="html" standalone="yes" omit-xml-declaration="yes" />

	<xsl:param name="entity"></xsl:param>
	<xsl:param name="ForParentEntityID"></xsl:param>
	<xsl:param name="StoreLoc"></xsl:param>

	<xsl:template match="root">
        <xsl:choose>
        	<xsl:when test="$ForParentEntityID=0"><xsl:apply-templates select="Entity" /></xsl:when>
        	<xsl:otherwise><xsl:apply-templates select="descendant-or-self::Entity[EntityID=$ForParentEntityID]" /></xsl:otherwise>
        </xsl:choose>
		
	</xsl:template>

	<xsl:template match="Entity">
	   <item>
	      <title>Products</title>
	      <ror:type>Product</ror:type>
			<ror:seeAlso>
				<xsl:value-of select="$StoreLoc"/>rorentity.aspx?entityname=<xsl:value-of select="$entity"/>&amp;entityid=<xsl:value-of select="EntityID"/>
			</ror:seeAlso>
	   </item>
	   <xsl:apply-templates select="Entity" />
	</xsl:template>

</xsl:stylesheet>