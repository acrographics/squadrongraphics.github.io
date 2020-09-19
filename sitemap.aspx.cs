// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/sitemap.aspx.cs 2     7/19/06 11:13p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for sitemap.
    /// </summary>
    public partial class sitemap : SkinBase
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }
            SectionTitle = AppLogic.GetString("sitemap.aspx.1", SkinID, ThisCustomer.LocaleSetting);
            String XmlPackageName = AppLogic.AppConfig("XmlPackage.SiteMapPage");
            if (XmlPackageName.Length != 0)
            {
                PackagePanel.Visible = true;
                EntityPanel.Visible = false;
                XmlPackage1.PackageName = XmlPackageName;
                XmlPackage1.SetContext = this;
            }
            else
            {
                PackagePanel.Visible = false;
                EntityPanel.Visible = true;
                Literal1.Text = new SiteMap1(base.EntityHelpers, SkinID, ThisCustomer).Contents;
            }

        }

    }
}
