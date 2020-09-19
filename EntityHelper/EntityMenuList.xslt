<?xml version="1.0" encoding="UTF-8" ?>
<!-- ###################################################################################################### -->
<!-- Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.					                -->
<!-- http://www.aspdotnetstorefront.com														                -->
<!-- For details on this license please visit  the product homepage at the URL above.		                -->
<!-- THE ABOVE NOTICE MUST REMAIN INTACT.                                                                   -->
<!-- $Header: /v6.2/Web/EntityHelper/EntityMenuList.xslt 2     8/12/06 10:16a Redwoodtree $	   -->
<!-- ###################################################################################################### -->
	<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" standalone="no" omit-xml-declaration="yes"/>

	<xsl:param name="entity"></xsl:param>
	<xsl:param name="ForParentEntityID"></xsl:param>
    <xsl:param name="custlocale"></xsl:param>
	<xsl:param name="deflocale"></xsl:param>
	<xsl:param name="adminsite"></xsl:param>
	<xsl:param name="nodeclass"></xsl:param>
	<xsl:param name="suppresstoparrow"></xsl:param>

	<xsl:template match="root">
		<xsl:param name="prefix">
			<xsl:choose>
				<xsl:when test="$entity='Category'"><xsl:value-of select="'c'" /></xsl:when>
				<xsl:when test="$entity='Section'"><xsl:value-of select="'s'" /></xsl:when>
				<xsl:when test="$entity='Manufacturer'"><xsl:value-of select="'m'" /></xsl:when>
				<xsl:when test="$entity='Library'"><xsl:value-of select="'l'" /></xsl:when>
			</xsl:choose>
		</xsl:param>

        <xsl:if test="not(boolean($ForParentEntityID)) and boolean($adminsite)">
            <item>
                <xsl:attribute name="Text">Add New <xsl:value-of select="$entity"/></xsl:attribute>
                <xsl:attribute name="NavigateUrl">editentity.aspx?entityname=<xsl:value-of select="$entity"/></xsl:attribute>
            </item>
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

	</xsl:template>

	<xsl:template match="Entity">
		<xsl:param name="ParentEntityID"><xsl:value-of select="ParentEntityID"/></xsl:param>
		<xsl:param name="prefix"/>
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

		<item>
			<xsl:attribute name="Text"><xsl:value-of select="$eName"/></xsl:attribute>
			<xsl:attribute name="NavigateUrl"><xsl:value-of select="concat($prefix, '-', EntityID, '-', SEName)"/>.aspx</xsl:attribute>
			<xsl:choose>
				<xsl:when test="$adminsite=0">
                    <xsl:attribute name="NavigateUrl"><xsl:value-of select="concat($prefix, '-', EntityID, '-', SEName)"/>.aspx</xsl:attribute>
                </xsl:when>
				<xsl:otherwise><xsl:attribute name="NavigateUrl">editentity.aspx?entityname=<xsl:value-of select="$entity"/>&amp;entityid=<xsl:value-of select="EntityID"/></xsl:attribute></xsl:otherwise>
			</xsl:choose>
			<xsl:if test="$nodeclass!='' and $ParentEntityID=0">
                <xsl:attribute name="LookId"><xsl:value-of select="$nodeclass"/></xsl:attribute>
			</xsl:if>
			<xsl:if test="$suppresstoparrow='0' or $ParentEntityID!=0">
				<xsl:if test="boolean(child::Entity)">
					<xsl:attribute name="Look-RightIconUrl">arrow.gif</xsl:attribute>
					<xsl:attribute name="Look-RightIconWidth">15</xsl:attribute>
				</xsl:if>
			</xsl:if>
			<xsl:apply-templates select="Entity">
				<xsl:with-param name="prefix" select="$prefix"/>
			</xsl:apply-templates>
		</item>

	</xsl:template>

</xsl:stylesheet>
