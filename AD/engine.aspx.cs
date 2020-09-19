// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/engine.aspx.cs 3     9/03/06 8:41p Redwoodtree $
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

namespace AspDotNetStorefrontAdmin
{

    public partial class engine : AspDotNetStorefront.SkinBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            // set the Customer context, and set the SkinBase context, so meta tags will be set if they are not blank in the XmlPackage results
            Package1.SetContext = this;
            if (Package1.PackageName.Length == 0)
            {
                String PN = CommonLogic.QueryStringCanBeDangerousContent("PackageName");
                if (PN.Length == 0)
                {
                    PN = CommonLogic.QueryStringCanBeDangerousContent("XmlPackage");
                }
                if (PN.Length == 0)
                {
                    PN = CommonLogic.QueryStringCanBeDangerousContent("Package");
                }
                PN = PN.ToLowerInvariant();
                if (PN.Length == 0)
                {
                    Package1.PackageName = "helloworld.xml.config";
                }
                else
                {
                    Package1.PackageName = PN;
                }
            }
            if (Package1.ContentType.Length != 0)
            {
                Page.Response.ContentType = Package1.ContentType;
            }
        }
    }
}