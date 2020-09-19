// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the Object homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/App_Code/ShowEntityPage.cs 2     7/19/06 11:13p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Xml;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for ShowEntityPage.
    /// 
    /// NOTE: the sql creation statements neeed to be cleaned up. they are pretty unreadible and inefficient at the moment due to substitutions
    /// 
    /// </summary>
    public class ShowEntityPage
    {
        private EntitySpecs m_EntitySpecs;

        private int m_EntityInstanceID;
        private String m_EntityInstanceName;
        private EntityHelper m_EntityHelper;
        private SkinBase m_SkinBase;
        private String m_ResourcePrefix;
        private String m_XmlPackage;
        private XmlNode n;
        private XmlNode m_ThisEntityNodeContext;

        private int m_SectionFilterID = 0;
        private int m_CategoryFilterID = 0;
        private int m_ManufacturerFilterID = 0;
        private int m_DistributorFilterID = 0;
        private int m_GenreFilterID = 0;
        private int m_VectorFilterID = 0;
        private int m_ProductTypeFilterID = 0;

        private bool m_URLValidated = true;

        private string m_PageOutput;

        public ShowEntityPage(EntitySpecs eSpecs, SkinBase sb)
        {
            m_EntitySpecs = eSpecs;
            m_SkinBase = sb;
            m_EntityHelper = AppLogic.LookupHelper(m_EntitySpecs.m_EntityName);
            m_ResourcePrefix = String.Format("show{0}.aspx.", m_EntitySpecs.m_EntityName.ToLowerInvariant());
            m_EntityInstanceID = CommonLogic.QueryStringUSInt(m_EntityHelper.GetEntitySpecs.m_EntityName + "ID");
        }

        public EntityHelper GetActiveEntityHelper
        {
            get { return m_EntityHelper; }
        }

        public XmlNode GetActiveEntityNodeContext
        {
            get { return m_ThisEntityNodeContext; }
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }

            n = m_EntityHelper.m_TblMgr.SetContext(m_EntityInstanceID);
            if (n == null)
            {
                HttpContext.Current.Response.Redirect("default.aspx");
            }
            m_ThisEntityNodeContext = n;

            String SENameINURL = CommonLogic.QueryStringCanBeDangerousContent("SEName");
            if (SENameINURL.ToLowerInvariant() !=
                XmlCommon.XmlField(GetActiveEntityNodeContext, "SEName").ToLowerInvariant())
            {
                string QS = BuildQueryString();
                String NewURL = string.Format("{0}{1}{2}", AppLogic.GetStoreHTTPLocation(false), SE.MakeEntityLink(m_EntityHelper.GetEntitySpecs.m_EntityName, m_EntityInstanceID, XmlCommon.XmlField(GetActiveEntityNodeContext, "SEName")), QS);
                HttpContext.Current.Response.Write("<html><head><title>Object Moved</title></head><body><b>Object moved to <a href=\"" + NewURL + "\">HERE</a></b></body></html>");
                HttpContext.Current.Response.Status = "301 Moved Permanently";
                HttpContext.Current.Response.AddHeader("Location", NewURL);
                //HttpContext.Current.Response.End();
                m_URLValidated = false;
            }

            if (m_URLValidated)
            {
                m_CategoryFilterID = CommonLogic.QueryStringUSInt("CategoryFilterID");
                m_SectionFilterID = CommonLogic.QueryStringUSInt("SectionFilterID");
                m_ProductTypeFilterID = CommonLogic.QueryStringUSInt("ProductTypeFilterID");
                m_ManufacturerFilterID = CommonLogic.QueryStringUSInt("ManufacturerFilterID");
                m_DistributorFilterID = CommonLogic.QueryStringUSInt("DistributorFilterID");
                m_GenreFilterID = CommonLogic.QueryStringUSInt("GenreFilterID");
                m_VectorFilterID = CommonLogic.QueryStringUSInt("VectorFilterID");

                if (CommonLogic.QueryStringCanBeDangerousContent("CategoryFilterID").Length == 0)
                {
                    if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilters").Length == 0 && AppLogic.AppConfigBool("PersistFilters") &&
                        CommonLogic.CookieUSInt("CategoryFilterID") != 0)
                    {
                        m_CategoryFilterID = CommonLogic.CookieUSInt("CategoryFilterID");
                    }
                }

                if (CommonLogic.QueryStringCanBeDangerousContent("SectionFilterID").Length == 0)
                {
                    if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilters").Length == 0 && AppLogic.AppConfigBool("PersistFilters") &&
                        CommonLogic.CookieUSInt("SectionFilterID") != 0)
                    {
                        m_SectionFilterID = CommonLogic.CookieUSInt("SectionFilterID");
                    }
                }

                if (CommonLogic.QueryStringCanBeDangerousContent("ProductTypeFilterID").Length == 0)
                {
                    if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilters").Length == 0 && AppLogic.AppConfigBool("PersistFilters") &&
                        CommonLogic.CookieUSInt("ProductTypeFilterID") != 0)
                    {
                        m_ProductTypeFilterID = CommonLogic.CookieUSInt("ProductTypeFilterID");
                    }
                    if (m_ProductTypeFilterID != 0 &&
                        !AppLogic.ProductTypeHasVisibleProducts(m_ProductTypeFilterID))
                    {
                        m_ProductTypeFilterID = 0;
                    }
                }

                if (CommonLogic.QueryStringCanBeDangerousContent("ManufacturerFilterID").Length == 0)
                {
                    if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilters").Length == 0 && AppLogic.AppConfigBool("PersistFilters") &&
                        CommonLogic.CookieUSInt("ManufacturerFilterID") != 0)
                    {
                        m_ManufacturerFilterID = CommonLogic.CookieUSInt("ManufacturerFilterID");
                    }
                }

                if (CommonLogic.QueryStringCanBeDangerousContent("DistributorFilterID").Length == 0)
                {
                    if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilters").Length == 0 && AppLogic.AppConfigBool("PersistFilters") &&
                        CommonLogic.CookieUSInt("DistributorFilterID") != 0)
                    {
                        m_DistributorFilterID = CommonLogic.CookieUSInt("DistributorFilterID");
                    }
                }

                if (CommonLogic.QueryStringCanBeDangerousContent("GenreFilterID").Length == 0)
                {
                    if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilters").Length == 0 && AppLogic.AppConfigBool("PersistFilters") &&
                        CommonLogic.CookieUSInt("GenreFilterID") != 0)
                    {
                        m_GenreFilterID = CommonLogic.CookieUSInt("GenreFilterID");
                    }
                }

                if (CommonLogic.QueryStringCanBeDangerousContent("VectorFilterID").Length == 0)
                {
                    if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilters").Length == 0 && AppLogic.AppConfigBool("PersistFilters") &&
                        CommonLogic.CookieUSInt("VectorFilterID") != 0)
                    {
                        m_VectorFilterID = CommonLogic.CookieUSInt("VectorFilterID");
                    }
                }

                if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilters").Length != 0)
                {
                    m_CategoryFilterID = 0;
                    m_SectionFilterID = 0;
                    m_ManufacturerFilterID = 0;
                    m_DistributorFilterID = 0;
                    m_GenreFilterID = 0;
                    m_VectorFilterID = 0;
                    m_ProductTypeFilterID = 0;
                }

                if (AppLogic.AppConfigBool("PersistFilters"))
                {
                    AppLogic.SetCookie("CategoryFilterID", m_CategoryFilterID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                    AppLogic.SetCookie("SectionFilterID", m_SectionFilterID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                    AppLogic.SetCookie("ManufacturerFilterID", m_ManufacturerFilterID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                    AppLogic.SetCookie("DistributorFilterID", m_DistributorFilterID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                    AppLogic.SetCookie("GenreFilterID", m_GenreFilterID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                    AppLogic.SetCookie("VectorFilterID", m_VectorFilterID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                    AppLogic.SetCookie("ProductTypeFilterID", m_ProductTypeFilterID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                }

                m_EntityInstanceName = m_EntityHelper.m_TblMgr.CurrentName(n, m_SkinBase.ThisCustomer.LocaleSetting);

                AppLogic.SetCookie("LastViewedEntityName", m_EntitySpecs.m_EntityName, new TimeSpan(1, 0, 0, 0, 0));
                AppLogic.SetCookie("LastViewedEntityInstanceID", m_EntityInstanceID.ToString(), new TimeSpan(1, 0, 0, 0, 0));
                AppLogic.SetCookie("LastViewedEntityInstanceName", m_EntityInstanceName, new TimeSpan(1, 0, 0, 0, 0));


                m_XmlPackage = m_EntityHelper.m_TblMgr.CurrentField(n, "XmlPackage").ToLowerInvariant();
                if (m_XmlPackage.Length == 0)
                {
                    m_XmlPackage = AppLogic.ro_DefaultEntityXmlPackage; // provide a default for backwards compatibility
                }


                String RunTimeParms = String.Format("EntityName={0}&EntityID={1}", m_EntitySpecs.m_EntityName, m_EntityInstanceID.ToString());

                RunTimeParms += String.Format("&CatID={0}", CommonLogic.IIF(m_EntitySpecs.m_EntityName.Trim().ToUpperInvariant() == "CATEGORY", m_EntityInstanceID.ToString(), m_CategoryFilterID.ToString()));
                RunTimeParms += String.Format("&SecID={0}", CommonLogic.IIF(m_EntitySpecs.m_EntityName.Trim().ToUpperInvariant() == "SECTION", m_EntityInstanceID.ToString(), m_SectionFilterID.ToString()));
                RunTimeParms += String.Format("&ManID={0}", CommonLogic.IIF(m_EntitySpecs.m_EntityName.Trim().ToUpperInvariant() == "MANUFACTURER", m_EntityInstanceID.ToString(), m_ManufacturerFilterID.ToString()));
                RunTimeParms += String.Format("&DistID={0}", CommonLogic.IIF(m_EntitySpecs.m_EntityName.Trim().ToUpperInvariant() == "DISTRIBUTOR", m_EntityInstanceID.ToString(), m_DistributorFilterID.ToString()));
                RunTimeParms += String.Format("&GenreID={0}", CommonLogic.IIF(m_EntitySpecs.m_EntityName.Trim().ToUpperInvariant() == "GENRE", m_EntityInstanceID.ToString(), m_GenreFilterID.ToString()));
                RunTimeParms += String.Format("&VectorID={0}", CommonLogic.IIF(m_EntitySpecs.m_EntityName.Trim().ToUpperInvariant() == "VECTOR", m_EntityInstanceID.ToString(), m_VectorFilterID.ToString()));
                RunTimeParms += String.Format("&ProductTypeFilterID={0}", m_ProductTypeFilterID.ToString());

                // CacheEntityPageHTML is an UNSUPPORTED and UNDOCUMENTED AppConfig
                // caching does NOT honor cross entity filtering, or other filters. Use it only on high traffic sites
                // with entity pages that do NOT vary by params other than those used in the CacheName string below.
                // if you are showing prices, they will remain the same during the cache duration (AppLogic.CacheDurationMinutes setting, usually 1 hr)
                String CacheName = String.Empty;


                m_SkinBase.SETitle = m_EntityHelper.m_TblMgr.CurrentFieldByLocale(n, "SETitle", m_SkinBase.ThisCustomer.LocaleSetting);
                if (m_SkinBase.SETitle.Length == 0)
                {
                    m_SkinBase.SETitle = Security.HtmlEncode(AppLogic.AppConfig("StoreName") + " - " + m_EntityInstanceName);
                }
                m_SkinBase.SEDescription = m_EntityHelper.m_TblMgr.CurrentFieldByLocale(n, "SEDescription", m_SkinBase.ThisCustomer.LocaleSetting);
                if (m_SkinBase.SEDescription.Length == 0)
                {
                    m_SkinBase.SEDescription = Security.HtmlEncode(m_EntityInstanceName);
                }
                m_SkinBase.SEKeywords = m_EntityHelper.m_TblMgr.CurrentFieldByLocale(n, "SEKeywords", m_SkinBase.ThisCustomer.LocaleSetting);
                if (m_SkinBase.SEKeywords.Length == 0)
                {
                    m_SkinBase.SEKeywords = Security.HtmlEncode(m_EntityInstanceName);
                }
                m_SkinBase.SENoScript = m_EntityHelper.m_TblMgr.CurrentFieldByLocale(n, "SENoScript", m_SkinBase.ThisCustomer.LocaleSetting);

                m_SkinBase.SectionTitle = Breadcrumb.GetEntityBreadcrumb(m_EntityInstanceID, m_EntityInstanceName, m_EntitySpecs.m_EntityName, m_SkinBase.ThisCustomer);

                 if (m_URLValidated)
                {
                    m_PageOutput = "<!-- XmlPackage: " + m_XmlPackage + " -->\n";
                    if (m_XmlPackage.Length == 0)
                    {
                        m_PageOutput += "<p><b><font color=red>XmlPackage format was chosen, but no XmlPackage was specified!</font></b></p>";
                    }
                    else
                    {
                        String s = null;
                        if (AppLogic.AppConfigBool("CacheEntityPageHTML"))
                        {
                            CacheName = String.Format("entityname={0}&entityid={1}&customerlevelid={2}&localesetting={3}&pagenum={4}&affiliateid={5}",
                                                      m_EntitySpecs.m_EntityName,
                                                      m_EntityInstanceID.ToString(),
                                                      m_SkinBase.ThisCustomer.CustomerLevelID.ToString(),
                                                      m_SkinBase.ThisCustomer.LocaleSetting,
                                                      CommonLogic.QueryStringUSInt("PageNum").ToString(),
                                                      m_SkinBase.ThisCustomer.AffiliateID.ToString()
                                );
                            s = (String)HttpContext.Current.Cache.Get(CacheName);
                        }
                        if (s == null ||s.Length == 0)
                        {
                            XmlPackage2 p = new XmlPackage2(m_XmlPackage, m_SkinBase.ThisCustomer, m_SkinBase.SkinID, "", RunTimeParms, String.Empty, true);
                            s = AppLogic.RunXmlPackage(p, m_SkinBase.GetParser, m_SkinBase.ThisCustomer, m_SkinBase.SkinID, true, true);
                            if (AppLogic.AppConfigBool("CacheEntityPageHTML"))
                            {
                                HttpContext.Current.Cache.Insert(CacheName, s, null, DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()), TimeSpan.Zero);
                            }

                            if (p.SectionTitle != "")
                            {
                                m_SkinBase.SectionTitle = p.SectionTitle;
                            }
                            if (p.SETitle != "")
                            {
                                m_SkinBase.SETitle = p.SETitle;
                            }
                            if (p.SEDescription != "")
                            {
                                m_SkinBase.SEDescription = p.SEDescription;
                            }
                            if (p.SEKeywords != "")
                            {
                                m_SkinBase.SEKeywords = p.SEKeywords;
                            }
                            if (p.SENoScript != "")
                            {
                                m_SkinBase.SENoScript = p.SENoScript;
                            }
                        }
                        m_PageOutput += s;
                    }
                }
            }
            AppLogic.eventHandler("ViewEntityPage").CallEvent("&ViewEntityPage=true");

        }

        private string BuildQueryString()
        {
            HttpRequest rqst = HttpContext.Current.Request;
            StringBuilder QStr = new StringBuilder("?", 1024);
            string EntityIDName = string.Format("{0}id", m_EntityHelper.GetEntitySpecs.m_EntityName.ToLower());
            for (int i = 0; i < rqst.QueryString.Count; i++)
            {
                string key = rqst.QueryString.GetKey(i);
                if (key.ToLowerInvariant() != EntityIDName)
                {
                    QStr.AppendFormat("{0}={1}&", key, rqst.QueryString[i]);
                }
            }
            return QStr.ToString();
        }

        public void RenderContents(HtmlTextWriter writer)
        {
            writer.Write(m_PageOutput);
        }
    }
}