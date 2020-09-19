<%@ Page Language="c#" Inherits="AspDotNetStorefront.galleries" CodeFile="galleries.aspx.cs" %>
<%@ Register TagPrefix="aspdnsf" TagName="XmlPackage" Src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <aspdnsf:XmlPackage ID="XmlPackage1" PackageName="page.galleries.xml.config" runat="server"
        EnforceDisclaimer="true" EnforcePassword="True" EnforceSubscription="true" AllowSEPropogation="True" />
</body>
</html>
