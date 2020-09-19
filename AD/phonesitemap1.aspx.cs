// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/phonesitemap1.aspx.cs 4     9/08/06 11:49p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for phonesitemap1.
    /// </summary>
    public partial class phonesitemap1 : AspDotNetStorefront.SkinBase
    {

        String m_IGD = String.Empty;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            m_IGD = CommonLogic.QueryStringCanBeDangerousContent("IGD");
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            System.Collections.Generic.Dictionary<string, EntityHelper> eh = AppLogic.MakeEntityHelpers();
            Customer cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
            writer.Write(new SiteMap1PhoneOrder(eh, 1, cust, m_IGD).Contents);
        }

        override protected void OnInit(EventArgs e)
        {
            SetTemplate("empty.ascx");
            base.OnInit(e);
        }
    }
}
