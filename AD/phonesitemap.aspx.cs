// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/phonesitemap.aspx.cs 4     9/08/06 11:49p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for phonesitemap.
    /// </summary>
    public partial class phonesitemap : System.Web.UI.Page
    {

        String IGD = String.Empty;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            IGD = CommonLogic.QueryStringCanBeDangerousContent("IGD");

            System.Collections.Generic.Dictionary<string, EntityHelper> eh = AppLogic.MakeEntityHelpers();
            Customer cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
            SiteMap1.LoadXml(new SiteMapPhoneOrder(eh, 1, cust, IGD).Contents);
        }
    }
}
