<%@ Page Language="c#" Inherits="AspDotNetStorefront.bestsellers" CodeFile="bestsellers.aspx.cs" %>
<%@ Register TagPrefix="aspdnsf" TagName="XmlPackage" Src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <aspdnsf:XmlPackage ID="Package1" PackageName="page.bestsellers.xml.config" runat="server" EnforceDisclaimer="True" EnforcePassword="True" EnforceSubscription="True" AllowSEPropogation="true"/>
</body>
</html>
