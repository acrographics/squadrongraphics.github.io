<%@ Page Language="C#" AutoEventWireup="true" CodeFile="entityFrame.aspx.cs" Inherits="entityFrame" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Entity Frame</title>
</head>
<frameset id="entityFrame" name="entityFrame" cols="<%=GetFrameSize()%>,*">
    <frame runat="server" id="entityMenu" name="entityMenu" scrolling="auto" frameborder="0" style="border-right: solid 1px #404040; margin: 0px 0px 0px 0px; padding: 0px 0px 0px 0px;" />
    <frame runat="server" id="entityBody" name="entityBody" scrolling="auto" frameborder="1" style="margin: 0px 0px 0px 0px; padding: 0px 0px 0px 0px;"/>
</frameset>
</html>
