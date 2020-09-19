<%@ Page language="c#" Inherits="AspDotNetStorefront.recentadditions" CodeFile="recentadditions.aspx.cs" %>
<%@ Register TagPrefix="aspdnsf" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <aspdnsf:XmlPackage ID="XmlPackage1" PackageName="page.recentadditions.xml.config" runat="server"
        EnforceDisclaimer="true" EnforcePassword="True" EnforceSubscription="true" AllowSEPropogation="True" />
</body>
</html>