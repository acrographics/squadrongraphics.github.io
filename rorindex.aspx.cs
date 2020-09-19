// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/rorindex.aspx.cs 2     7/15/06 11:38a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
	/// <summary>
    /// Summary description for rorindex.
	/// </summary>
    public partial class rorindex : System.Web.UI.Page
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.ContentType = "text/xml";
			Response.ContentEncoding = new System.Text.UTF8Encoding();
			Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            Response.Write("<rss version=\"2.0\" xmlns:ror=\"http://rorweb.com/0.1/\">");
            Response.Write("<channel>");

            String StoreLoc = AppLogic.GetStoreHTTPLocation(false);


            Response.Write("<title>" + XmlCommon.XmlEncode(AppLogic.AppConfig("SE_MetaTitle")) + "</title>");
            Response.Write("<link>" + StoreLoc + "</link>");

            Response.Write("<item>");
            Response.Write("    <title>" + XmlCommon.XmlEncode(AppLogic.AppConfig("SE_MetaTitle")) + "</title>");
            Response.Write("    <link>" + StoreLoc + "</link>");
            Response.Write("    <description>" + XmlCommon.XmlEncode(AppLogic.AppConfig("SE_MetaDescription")) + "</description>");
            Response.Write("    <ror:type>Main</ror:type>");
            Response.Write("    <ror:keywords>" + XmlCommon.XmlEncode(AppLogic.AppConfig("SE_MetaKeywords")) + "</ror:keywords>");
            Response.Write("    <ror:image></ror:image>"); // not supported
            Response.Write("    <ror:updated>" + System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-" + System.DateTime.Now.Day.ToString() + "</ror:updated>");
            Response.Write("    <ror:updatePeriod>day</ror:updatePeriod>");
            Response.Write("</item>");

            Response.Write("<item>");
            Response.Write("    <title>Articles</title> ");
            Response.Write("    <ror:type>Articles</ror:type>");
            Response.Write("    <ror:seeAlso>" + StoreLoc + "rortopics.aspx</ror:seeAlso>");
            Response.Write("</item>");
			
			Response.Write(AppLogic.CategoryEntityHelper.GetEntityRorSiteMap(0,Localization.GetWebConfigLocale(),true,true));
            Response.Write(AppLogic.SectionEntityHelper.GetEntityRorSiteMap(0, Localization.GetWebConfigLocale(), true, true));
            Response.Write(AppLogic.ManufacturerEntityHelper.GetEntityRorSiteMap(0, Localization.GetWebConfigLocale(), true, true));
            Response.Write(AppLogic.DistributorEntityHelper.GetEntityRorSiteMap(0, Localization.GetWebConfigLocale(), true, true));
		
#if PRO
			// not supported
#else
            Response.Write(AppLogic.LibraryEntityHelper.GetEntityRorSiteMap(0, Localization.GetWebConfigLocale(), true, true));
            Response.Write(AppLogic.GenreEntityHelper.GetEntityRorSiteMap(0, Localization.GetWebConfigLocale(), true, true));
            Response.Write(AppLogic.VectorEntityHelper.GetEntityRorSiteMap(0, Localization.GetWebConfigLocale(), true, true));
#endif
			Response.Write("</channel>");
			Response.Write("</rss>");

		}

	}
}
