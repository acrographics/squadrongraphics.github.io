<%@ Page language="c#" Inherits="AspDotNetStorefront.partners" CodeFile="partners.aspx.cs" %>
<%@ Register TagPrefix="aspdnsf" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <aspdnsf:XmlPackage ID="XmlPackage1" PackageName="page.partners.xml.config" runat="server"
        EnforceDisclaimer="true" EnforcePassword="True" EnforceSubscription="true" AllowSEPropogation="True" />
</body>
</html>
