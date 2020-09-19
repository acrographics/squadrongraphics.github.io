<%@ Page language="c#" Inherits="AspDotNetStorefront.driver" CodeFile="driver.aspx.cs" EnableEventValidation="false"%>
<%@ Register TagPrefix="aspdnsf" TagName="Topic" src="TopicControl.ascx" %>
<html>
<head></head>
<body>
<aspdnsf:Topic id="Topic1" runat="server" EnforceDisclaimer="true" EnforcePassword="true" EnforceSubscription="true" AllowSEPropogation="true"/>
</body>
</html>
