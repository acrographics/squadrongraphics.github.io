// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/giftcardview.aspx.cs 2     8/19/06 8:50p Buddy $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using AspDotNetStorefrontCommon;

public partial class giftcardview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");

        Customer cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
        string temp = AppLogic.RunXmlPackage(AppLogic.AppConfig("XmlPackage.EmailGiftCardNotification"), new Parser(1, cust), cust, 1, String.Empty, "GiftCardID=" + CommonLogic.QueryStringNativeInt("iden"), true, true);
        ltView.Text = temp;
    }
}
