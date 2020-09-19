<%@ Page language="c#" Inherits="AspDotNetStorefrontAdmin.entityEditExtendedPrices" CodeFile="entityEditExtendedPrices.aspx.cs" %>
<html>
    <head>
        <title>Extended Price</title>
        <link href="../App_Themes/Default/StyleSheet.css" type="text/css" rel="stylesheet" />
        <script type="text/javascript" src="jscripts/formValidate.js"></script>
    </head>
    
    <script type="text/javascript" type="text/javascript" >
    function Form_Validator(theForm)
    {
    submitonce(theForm);
    return (true);
    }
    </script>

    <body>
        <asp:Literal ID="ltContent" runat="server"></asp:Literal>
    </body>
</html>