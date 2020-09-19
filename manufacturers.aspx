<%@ Page language="c#" Inherits="AspDotNetStorefront.manufacturers" CodeFile="manufacturers.aspx.cs" EnableEventValidation="false"%>
<%@ Register TagPrefix="aspdnsf" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head></head>
<body>
<aspdnsf:XmlPackage id="Package1" PackageName="entity.manufacturers.xml.config" runat="server" EnforceDisclaimer="true" EnforcePassword="true" EnforceSubscription="true" AllowSEPropogation="true" RuntimeParams="entity=Manufacturer"/>
</body>
</html>


