<%@ Control Language="c#" AutoEventWireup="false" Inherits="AspDotNetStorefront.TemplateBase" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<title>Empty Page Skin</title>
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/style.css" type="text/css">
<script type="text/javascript" src="jscripts/formValidate.js"></script>
</head>
<body>
(!XmlPackage Name="skin.adminalert.xml.config"!)
(!PAGEINFO!)
<div style="font-size: 11px;">
<asp:placeholder id="PageContent" runat="server"></asp:placeholder>
</div>
</body>
</html>
