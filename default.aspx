<%@ Page Language="c#" Inherits="AspDotNetStorefront._default" CodeFile="default.aspx.cs" %>
<%@ Register TagPrefix="aspdnsf" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <aspdnsf:XmlPackage id="Package1" PackageName="page.default.xml.config" runat="server" EnforceDisclaimer="true" EnforcePassword="true" EnforceSubscription="True" AllowSEPropogation="True"/>
</body>
</html>
