<?xml version="1.0" encoding="UTF-8" ?>
<!-- ###################################################################################################### -->
<!-- Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.					                -->
<!-- http://www.aspdotnetstorefront.com														                -->
<!-- For details on this license please visit  the product homepage at the URL above.		                -->
<!-- THE ABOVE NOTICE MUST REMAIN INTACT.                                                                   -->
<!-- $Header: /v6.2/Web/EntityHelper/ComponentArtTree.xslt 1     7/08/06 10:42p Admin $	   -->
<!-- ###################################################################################################### -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" omit-xml-declaration="yes" />
	
	<xsl:param name="entity"></xsl:param>
	<xsl:param name="ForParentEntityID"></xsl:param>
	<xsl:param name="entityDispName"></xsl:param>
	<xsl:param name="adminsite"></xsl:param>
    <xsl:param name="custlocale"></xsl:param>
	<xsl:param name="deflocale"></xsl:param>
	<xsl:param name="expandID"></xsl:param>

	<xsl:template match="root">
		<xsl:param name="prefix">
			<xsl:choose>
				<xsl:when test="$entity='Category'"><xsl:value-of select="'c'" /></xsl:when>
				<xsl:when test="$entity='Section'"><xsl:value-of select="'s'" /></xsl:when>
				<xsl:when test="$entity='Manufacturer'"><xsl:value-of select="'m'" /></xsl:when>
				<xsl:when test="$entity='Library'"><xsl:value-of select="'l'" /></xsl:when>
			</xsl:choose>
		</xsl:param>

		<siteMapNode>

            <xsl:attribute name="Text"><xsl:value-of select="$entityDispName"/></xsl:attribute>
			<xsl:if test="boolean($expandID)">
				<xsl:attribute name="Expanded">true</xsl:attribute>
			</xsl:if>
            <xsl:choose>
                <xsl:when test="$ForParentEntityID=0">
                    <xsl:apply-templates select="Entity">
        				<xsl:with-param name="prefix" select="$prefix"/>
    	    		</xsl:apply-templates>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:apply-templates select="descendant-or-self::Entity[EntityID=$ForParentEntityID]" >
        				<xsl:with-param name="prefix" select="$prefix"/>
    	    		</xsl:apply-templates>
                </xsl:otherwise>
            </xsl:choose>

        </siteMapNode>
	</xsl:template>

	<xsl:template match="Entity">
		<xsl:param name="prefix"></xsl:param>
		<xsl:param name="eName">
			<xsl:choose>
				<xsl:when test="count(Name/ml/locale[@name=$custlocale])!=0">
					<xsl:value-of select="Name/ml/locale[@name=$custlocale]"/>
				</xsl:when>
				<xsl:when test="count(Name/ml/locale[@name=$deflocale])!=0">
					<xsl:value-of select="Name/ml/locale[@name=$deflocale]"/>
				</xsl:when>
				<xsl:when test="count(Name/ml)=0">
					<xsl:value-of select="Name"/>
				</xsl:when>
			</xsl:choose>
		</xsl:param>
		<siteMapNode>
			<xsl:if test="boolean(descendant-or-self::Entity[./EntityID=$expandID])">
				<xsl:attribute name="Expanded">true</xsl:attribute>
			</xsl:if>
			<xsl:choose>
				<xsl:when test="$adminsite=0">
                    <xsl:attribute name="NavigateUrl"><xsl:value-of select="concat($prefix, '-', EntityID, '-', SEName)"/>.aspx</xsl:attribute>
                </xsl:when>
				<xsl:otherwise><xsl:attribute name="NavigateUrl">editentity.aspx?entityname=<xsl:value-of select="$entity"/>&amp;entityid=<xsl:value-of select="EntityID"/></xsl:attribute></xsl:otherwise>
			</xsl:choose>
			<xsl:attribute name="Text"><xsl:value-of select="$eName"/></xsl:attribute>
			<xsl:apply-templates select="Entity">
				<xsl:with-param name="prefix" select="$prefix"/>
			</xsl:apply-templates>
		</siteMapNode>
	</xsl:template>

</xsl:stylesheet>
