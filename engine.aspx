<%@ Page language="c#" Inherits="AspDotNetStorefront.engine" CodeFile="engine.aspx.cs" EnableEventValidation="false"%>
<%@ Register TagPrefix="aspdnsf" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head></head>
<body>
<aspdnsf:XmlPackage id="Package1" runat="server" EnforceDisclaimer="true" EnforcePassword="true" EnforceSubscription="true" AllowSEPropogation="true"/>
</body>
</html>


