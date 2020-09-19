<%@ Control Language="c#" AutoEventWireup="false" Inherits="AspDotNetStorefront.TemplateBase" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<title>Empty Template</title>
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/style.css" type="text/css">
<script type="text/javascript" src="jscripts/formValidate.js"></script>
</head>
<body rightmargin="0" leftmargin="0" topmargin="0" marginwidth="0" marginheight="0" bgcolor="#ffffff">
<!-- PAGE INVOCATION: '(!INVOCATION!)' -->
<!-- PAGE REFERRER: '(!REFERRER!)' -->
<!-- STORE LOCALE: '(!STORELOCALE!)' -->
<!-- CUSTOMER ID: '(!CUSTOMERID!)' -->
<!-- CUSTOMER LOCALE: '(!CUSTOMERLOCALE!)' -->
<!-- STORE VERSION: '(!STORE_VERSION!)' -->
<!-- CACHE MENUS: '(!AppConfig name="CacheMenus"!)' -->
<!-- CONTENTS START -->
<div align="center" style="width:100%;"><p style="background-color:#FFFF00; padding: 4px;"><span style="font-size: 12px;"><font color="red"><b>**** You Are Acting For Customer: (!USER_MENU_NAME!), ID=(!CUSTOMERID!) ****</b>&nbsp;&nbsp;<b><a href="default.aspx?IGD=">End Session</a></b></font></span></p></div>
<asp:placeholder id="PageContent" runat="server"></asp:placeholder>
<!-- CONTENTS END -->
</body>
</html>
