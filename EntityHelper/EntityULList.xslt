<?xml version="1.0" encoding="UTF-8" ?>
<!-- ###################################################################################################### -->
<!-- Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.					                -->
<!-- http://www.aspdotnetstorefront.com														                -->
<!-- For details on this license please visit  the product homepage at the URL above.		                -->
<!-- THE ABOVE NOTICE MUST REMAIN INTACT.                                                                   -->
<!-- $Header: /v6.2/Web/EntityHelper/EntityULList.xslt 1     7/08/06 10:42p Admin $	   -->
<!-- ###################################################################################################### -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" standalone="yes"/>

	<xsl:param name="entity"></xsl:param>
	<xsl:param name="ForParentEntityID"></xsl:param>
	<xsl:param name="IncludeLinks"></xsl:param>
	<xsl:param name="AffiliateID"></xsl:param>
	<xsl:param name="IncludeObjects"></xsl:param>
	<xsl:param name="CssClassName"></xsl:param>
	<xsl:param name="RecurseChildren"></xsl:param>
	<xsl:param name="OnlyExpandForThisChildID"></xsl:param>
	<xsl:param name="Prefix"></xsl:param>
	<xsl:param name="adminsite"></xsl:param>
    <xsl:param name="custlocale"></xsl:param>
	<xsl:param name="deflocale"></xsl:param>
	<xsl:param name="objName"></xsl:param>

	<xsl:key name="objectEntity" match="object" use="EntityID"/>

	<xsl:template match="root">
		<xsl:param name="prefix">
			<xsl:choose>
				<xsl:when test="$entity='Category'"><xsl:value-of select="'c'" /></xsl:when>
				<xsl:when test="$entity='Section'"><xsl:value-of select="'s'" /></xsl:when>
				<xsl:when test="$entity='Manufacturer'"><xsl:value-of select="'m'" /></xsl:when>
				<xsl:when test="$entity='Library'"><xsl:value-of select="'l'" /></xsl:when>
			</xsl:choose>
		</xsl:param>
		<ul>
            <xsl:attribute name="class"><xsl:value-of select="$CssClassName" /></xsl:attribute>
            <xsl:choose>
                <xsl:when test="$ForParentEntityID=0">
                    <xsl:apply-templates select="Entity">
                        <xsl:with-param name="prefix" select="$prefix"/>
                    </xsl:apply-templates>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:apply-templates select="descendant-or-self::Entity[EntityID=$ForParentEntityID]" >
                        <xsl:with-param name="prefix" select="$prefix"/>
                        <xsl:with-param name="indent" select="$Prefix"/>
                    </xsl:apply-templates>
                </xsl:otherwise>
            </xsl:choose>
		</ul>
	</xsl:template>

	<xsl:template match="Entity">
		<xsl:param name="prefix"></xsl:param>
        <xsl:param name="indent"></xsl:param>
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

		<xsl:variable name="containsChild" select="boolean(descendant-or-self::Entity[./EntityID=$OnlyExpandForThisChildID])"></xsl:variable>

        <li>
            <xsl:choose>
                <xsl:when test="boolean($IncludeLinks)">
                    <xsl:choose>
                        <xsl:when test="boolean($adminsite)">
                            <a>
                                <xsl:attribute name="href">editentity.aspx?entityname=<xsl:value-of select="$entity"/>&amp;entityid=<xsl:value-of select="EntityID"/></xsl:attribute>
                                <xsl:if test="./ParentEntityID!=0"><xsl:value-of select="$indent" disable-output-escaping="yes"/></xsl:if>
                                <xsl:choose>
                                    <xsl:when test="boolean($OnlyExpandForThisChildID) and boolean($containsChild)"><b>&#187;<xsl:value-of select="$eName" /></b></xsl:when>
                                    <xsl:otherwise>
                                        <xsl:value-of select="$eName"/>
                                    </xsl:otherwise>
                                </xsl:choose>
                            </a>
                        </xsl:when>
                        <xsl:otherwise>
                            <a>
                                <xsl:attribute name="href"><xsl:value-of select="concat($prefix, '-', EntityID, '-', SEName)"/>.aspx</xsl:attribute>
                                <xsl:if test="./ParentEntityID!=0"><xsl:value-of select="$indent"  disable-output-escaping="yes"/></xsl:if>
                                <xsl:choose>
                                    <xsl:when test="boolean($OnlyExpandForThisChildID) and boolean($containsChild)"><b>&#187;<xsl:value-of select="$eName" /></b></xsl:when>
                                    <xsl:otherwise><xsl:value-of select="$eName"/></xsl:otherwise>
                                </xsl:choose>
                            </a>
                        </xsl:otherwise>
                    </xsl:choose>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:choose>
                        <xsl:when test="boolean($adminsite)">
                            <xsl:if test="./ParentEntityID!=0"><xsl:value-of select="$indent" disable-output-escaping="yes"/></xsl:if>
                            <xsl:choose>
                                <xsl:when test="boolean($OnlyExpandForThisChildID) and boolean($containsChild)"><b>&#187;<xsl:value-of select="$eName" /></b></xsl:when>
                                <xsl:otherwise><xsl:value-of select="$eName"/></xsl:otherwise>
                            </xsl:choose>
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:if test="./ParentEntityID!=0"><xsl:value-of select="$indent"  disable-output-escaping="yes"/></xsl:if>
                            <xsl:choose>
                                <xsl:when test="boolean($OnlyExpandForThisChildID) and boolean($containsChild)"><b>&#187;<xsl:value-of select="$eName" /></b></xsl:when>
                                <xsl:otherwise><xsl:value-of select="$eName"/></xsl:otherwise>
                            </xsl:choose>
                        </xsl:otherwise>
                    </xsl:choose>
                </xsl:otherwise>
            </xsl:choose>

            <xsl:if test="boolean($IncludeObjects) and count(key('objectEntity', EntityID))!=0">
				<ul>
		            <xsl:attribute name="class"><xsl:value-of select="$CssClassName" /></xsl:attribute>
					<xsl:call-template name="obj">
						<xsl:with-param name="prefix" select="$prefix"/>
						<xsl:with-param name="objEID" select="EntityID"/>
					</xsl:call-template>
				</ul>
            </xsl:if>

            <xsl:if test="boolean($RecurseChildren) or boolean($containsChild)">
				<ul>
		            <xsl:attribute name="class"><xsl:value-of select="$CssClassName" /></xsl:attribute>
					<xsl:apply-templates select="Entity">
						<xsl:with-param name="prefix" select="$prefix"/>
                        <xsl:with-param name="indent" select="concat($indent, $Prefix)"/>
					</xsl:apply-templates>
				</ul>
            </xsl:if>
		</li>
	</xsl:template>

	<xsl:template name="obj">
		<xsl:param name="prefix"></xsl:param>
		<xsl:param name="objEID"></xsl:param>

        
		<xsl:for-each select="key('objectEntity', $objEID)">
            <xsl:variable name="eName">
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
            </xsl:variable>
            <li>
				<xsl:choose>
	   				<xsl:when test="boolean($adminsite)">
						<a>
							<xsl:attribute name="href">edit<xsl:value-of select="$objName"/>.aspx?<xsl:value-of select="$objName"/>id=<xsl:value-of select="ProductID"/></xsl:attribute>
					   		<xsl:value-of select="$eName"/>
				         </a>
					</xsl:when>
	  				<xsl:otherwise>
	      				<a>
	         				<xsl:attribute name="href"><xsl:value-of select="concat('p', '-', ProductID, '-', SEName)"/>.aspx</xsl:attribute>
	      					<xsl:value-of select="$eName"/>
	           			</a>
					</xsl:otherwise>
	    		</xsl:choose>

			</li>
		</xsl:for-each>

	</xsl:template>

</xsl:stylesheet>
