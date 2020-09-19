<?xml version="1.0" encoding="UTF-8" ?>
<!-- ###################################################################################################### -->
<!-- Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.					                -->
<!-- http://www.aspdotnetstorefront.com														                -->
<!-- For details on this license please visit  the product homepage at the URL above.		                -->
<!-- THE ABOVE NOTICE MUST REMAIN INTACT.                                                                   -->
<!-- $Header: /v6.2/Web/EntityHelper/EntityArrayList.xslt 1     7/08/06 10:42p Admin $	   -->
<!-- ###################################################################################################### -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" standalone="yes"/>

	<xsl:param name="ForParentEntityID"></xsl:param>
    <xsl:param name="filterID"></xsl:param>
    <xsl:param name="custlocale"></xsl:param>
	<xsl:param name="deflocale"></xsl:param>

	<xsl:template match="root">

        <xsl:choose>
            <xsl:when test="$ForParentEntityID=0">
                <xsl:apply-templates select="Entity[./EntityID!=$filterID]">
                    <xsl:with-param name="prefix"></xsl:with-param>
                </xsl:apply-templates>
            </xsl:when>
            <xsl:otherwise>
                <xsl:apply-templates select="descendant-or-self::Entity[EntityID=$ForParentEntityID]" >
                </xsl:apply-templates>
            </xsl:otherwise>
        </xsl:choose>

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
        <xsl:value-of select="EntityID"/>|<xsl:value-of select="concat($prefix, $eName)"/>~<xsl:apply-templates select="Entity[./EntityID!=$filterID]"><xsl:with-param name="prefix" select="concat($prefix, $eName, ' -&gt; ')"></xsl:with-param></xsl:apply-templates>
	</xsl:template>
</xsl:stylesheet>