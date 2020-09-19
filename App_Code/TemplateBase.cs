// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/App_Code/TemplateBase.cs 2     9/07/06 8:05p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.IO;
using System.Xml.Xsl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using AspDotNetStorefrontCommon;
using System.Globalization;
using ComponentArt.Web.UI;

namespace AspDotNetStorefront
{
    /// <summary>
    ///		This is the "code behind" for the skin user control (usually template.ascx) to provide menu building logic, or 
    ///		any other common logic required. The code is placed in this class, and the user control is derived from this class
    ///		so users can create new skins WITHOUT having to recompile, or even have VS.NET, etc...
    /// </summary>
    public class TemplateBase : System.Web.UI.UserControl
    {

        protected PlaceHolder PageContent;
        protected ComponentArt.Web.UI.Menu PageMenu;
        protected ComponentArt.Web.UI.Menu VertMenu;
        protected ComponentArt.Web.UI.TreeView PageTree;

        public new SkinBase Page
        {
            get
            {
                return (SkinBase)base.Page;
            }
        }

        public PlaceHolder Content
        {
            get
            {
                return PageContent;
            }
        }

        public string CurrentEntity
        {
            get
            {
                return CommonLogic.Capitalize(CommonLogic.GetThisPageName(false).Replace("show", "").Replace(".aspx", "").ToLowerInvariant());
            }
        }

        public String ResourceMatchEvaluator(Match match)
        {
            String l = match.Groups[1].Value;
            string s = AppLogic.GetString(l, Page.SkinID, Page.ThisCustomer.LocaleSetting);
            if (s == null || s.Length == 0 || s == l)
            {
                s = match.Value;
            }
            return s;
        }

        public String ResourceMatchEvaluatorXmlEncoded(Match match)
        {
            String l = match.Groups[1].Value;
            string s = AppLogic.GetString(l, Page.SkinID, Page.ThisCustomer.LocaleSetting);
            if (s == null || s.Length == 0 || s == l)
            {
                s = match.Value;
            }
            return XmlCommon.XmlEncode(s);
        }

        //Canged to protected by AB
        protected virtual void Page_Load(object sender, EventArgs e)
        {
            if (PageMenu != null)
            {
                // get menu config file:
                String MN = "menuData.xml";
                if (AppLogic.IsAdminSite)
                {
                    if (Page.ThisCustomer.Notes.Trim().Length != 0 && Page.ThisCustomer.Notes.ToLowerInvariant().IndexOf(".xml") != -1)
                    {
                        MN = Page.ThisCustomer.Notes.Trim();
                    }
                }

                String CacheName = String.Format("menudoc_{0}_{1}_{2}_{3}", AppLogic.IsAdminSite.ToString(), Page.SkinID.ToString(), Page.ThisCustomer.LocaleSetting,MN);
                XmlDocument doc = null;
                if (AppLogic.CachingOn)
                {
                    doc = (XmlDocument)HttpContext.Current.Cache.Get(CacheName);
                }
                if (doc == null)
                {

                    doc = new XmlDocument();

                    String MenuConfigFileString = CommonLogic.ReadFile(CommonLogic.SafeMapPath("skins/skin_" + Page.SkinID.ToString() + "/" + MN), false);

                    doc.LoadXml(MenuConfigFileString);

                    HierarchicalTableMgr tblMgr;

                    XmlNode rootNode = doc.SelectSingleNode("/SiteMap");

                    // Find Manufacturers menu top
                    XmlNode mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite,"admin.","") + "menu.Manufacturers!)']");
                    tblMgr = AppLogic.ManufacturerEntityHelper.m_TblMgr;
                    if (tblMgr.NumRootLevelNodes <= AppLogic.MaxMenuSize())
                    {
                        AddEntityMenuXsl(doc, "Manufacturer", tblMgr, mNode, 0, String.Empty);
                    }
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Manufacturers0!)']");
                    //tblMgr = AppLogic.LookupHelper(Page.EntityHelpers, "Manufacturer").m_TblMgr;
                    if (tblMgr.NumRootLevelNodes <= AppLogic.MaxMenuSize())
                    {
                        AddEntityMenuXsl(doc, "Manufacturer", tblMgr, mNode, 0, "TopItemLook");
                    }

                    // Find Categories menu top
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Categories!)']");
                    AddEntityMenuXsl(doc, "Category", AppLogic.CategoryEntityHelper.m_TblMgr, mNode, 0, String.Empty);
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Categories0!)']");
                    AddEntityMenuXsl(doc, "Category", AppLogic.CategoryEntityHelper.m_TblMgr, mNode, 0, "TopItemLook");


                    // Find Sections menu top
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Sections!)']");
                    AddEntityMenuXsl(doc, "Section", AppLogic.SectionEntityHelper.m_TblMgr, mNode, 0, String.Empty);
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Sections0!)']");
                    AddEntityMenuXsl(doc, "Section", AppLogic.SectionEntityHelper.m_TblMgr, mNode, 0, "TopItemLook");

#if PRO
					// not supported in PRO version
#else
                    // Find Libraries menu top
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Libraries!)']");
                    AddEntityMenuXsl(doc, "Library", AppLogic.LibraryEntityHelper.m_TblMgr, mNode, 0, String.Empty);
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Libraries0!)']");
                    AddEntityMenuXsl(doc, "Library", AppLogic.LibraryEntityHelper.m_TblMgr, mNode, 0, "TopItemLook");

                    // Find Genres menu top
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Genres!)']");
                    AddEntityMenuXsl(doc, "Genre", AppLogic.GenreEntityHelper.m_TblMgr, mNode, 0, String.Empty);
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Genres0!)']");
                    AddEntityMenuXsl(doc, "Genre", AppLogic.GenreEntityHelper.m_TblMgr, mNode, 0, "TopItemLook");

                    // Find Vectors menu top
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Vectors!)']");
                    AddEntityMenuXsl(doc, "Vector", AppLogic.VectorEntityHelper.m_TblMgr, mNode, 0, String.Empty);
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Vectors0!)']");
                    AddEntityMenuXsl(doc, "Vector", AppLogic.VectorEntityHelper.m_TblMgr, mNode, 0, "TopItemLook");


#endif

                    //CommonLogic.WriteFile("images/menudata.xml",XmlCommon.PrettyPrintXml(doc.InnerXml),true);

                    Regex m_ReMatch = new Regex(@"\(!(.*?)!\)");
                    MatchEvaluator m_ResourceMatch = new MatchEvaluator(ResourceMatchEvaluatorXmlEncoded);
                    doc.InnerXml = m_ReMatch.Replace(doc.InnerXml, m_ResourceMatch);

                    //CommonLogic.WriteFile("images/menudata.xml",XmlCommon.PrettyPrintXml(doc.InnerXml),true);

                    if (AppLogic.CachingOn)
                    {
                        HttpContext.Current.Cache.Insert(CacheName, doc, null, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()), TimeSpan.Zero);
                    }
                }
                if (PageMenu != null)
                {
                    PageMenu.LoadXml(doc);
                }
            }
            if (VertMenu != null)
            {
                String CacheName = String.Format("vertmenudoc_{0}_{1}_{2}", AppLogic.IsAdminSite.ToString(), Page.SkinID.ToString(), Page.ThisCustomer.LocaleSetting);
                XmlDocument doc = null;
                if (AppLogic.CachingOn)
                {
                    doc = (XmlDocument)HttpContext.Current.Cache.Get(CacheName);
                }
                if (doc == null)
                {

                    doc = new XmlDocument();

                    // get menu config file:
                    String MenuConfigFileString = CommonLogic.ReadFile(CommonLogic.SafeMapPath("skins/skin_" + Page.SkinID.ToString() + "/vertMenuData.xml"), false);

                    doc.LoadXml(MenuConfigFileString);

                    XmlNode rootNode = doc.SelectSingleNode("/SiteMap");

                    // Find Manufacturers menu top
                    XmlNode mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Manufacturers!)']");
                    AddEntityMenuXsl(doc, "Manufacturer", AppLogic.ManufacturerEntityHelper.m_TblMgr, mNode, 0, String.Empty);
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Manufacturers0!)']");
                    AddEntityMenuXsl(doc, "Manufacturer", AppLogic.ManufacturerEntityHelper.m_TblMgr, mNode, 0, "VertTopItemLook");

                    // Find Categories menu top
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Categories!)']");
                    AddEntityMenuXsl(doc, "Category", AppLogic.CategoryEntityHelper.m_TblMgr, mNode, 0, String.Empty);
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Categories0!)']");
                    AddEntityMenuXsl(doc, "Category", AppLogic.CategoryEntityHelper.m_TblMgr, mNode, 0, "VertTopItemLook");

                    // Find Sections menu top
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Sections!)']");
                    AddEntityMenuXsl(doc, "Section", AppLogic.SectionEntityHelper.m_TblMgr, mNode, 0, String.Empty);
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Sections0!)']");
                    AddEntityMenuXsl(doc, "Section", AppLogic.SectionEntityHelper.m_TblMgr, mNode, 0, "VertTopItemLook");

#if PRO
					// not supported in PRO version
#else
                    // Find Libraries menu top
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Libraries!)']");
                    AddEntityMenuXsl(doc, "Library", AppLogic.LibraryEntityHelper.m_TblMgr, mNode, 0, String.Empty);
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Libraries0!)']");
                    AddEntityMenuXsl(doc, "Library", AppLogic.LibraryEntityHelper.m_TblMgr, mNode, 0, "VertTopItemLook");

                    // Find Genres menu top
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Genres!)']");
                    AddEntityMenuXsl(doc, "Genre", AppLogic.GenreEntityHelper.m_TblMgr, mNode, 0, String.Empty);
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Genres0!)']");
                    AddEntityMenuXsl(doc, "Genre", AppLogic.GenreEntityHelper.m_TblMgr, mNode, 0, "VertTopItemLook");

                    // Find Vectors menu top
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Vectors!)']");
                    AddEntityMenuXsl(doc, "Vector", AppLogic.VectorEntityHelper.m_TblMgr, mNode, 0, String.Empty);
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.Vectors0!)']");
                    AddEntityMenuXsl(doc, "Vector", AppLogic.VectorEntityHelper.m_TblMgr, mNode, 0, "VertTopItemLook");

#endif

                    Regex m_ReMatch = new Regex(@"\(!(.*?)!\)");
                    MatchEvaluator m_ResourceMatch = new MatchEvaluator(ResourceMatchEvaluatorXmlEncoded);
                    doc.InnerXml = m_ReMatch.Replace(doc.InnerXml, m_ResourceMatch);

                    //CommonLogic.WriteFile("images/vertmenudata.xml",XmlCommon.PrettyPrintXml(doc.InnerXml),true);

                    if (AppLogic.CachingOn)
                    {
                        HttpContext.Current.Cache.Insert(CacheName, doc, null, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()), TimeSpan.Zero);
                    }
                }
                if (VertMenu != null)
                {
                    VertMenu.LoadXml(doc);
                }
            }
            if (PageTree != null)
            {
                // Note: Tree doc cannot be cached, as it changes every page, (as we have to open the selected tree node based on query string params)
                StringBuilder tmpS = new StringBuilder(4096);
                int curEntityID = CommonLogic.QueryStringUSInt("EntityID");
                String curEntity = CurrentEntity;
                tmpS.Append("<siteMap>");
                if (AppLogic.AppConfigBool("Tree.ShowCategories"))
                {
                    tmpS.Append(AppLogic.LookupHelper("Category").ComponentArtTree(0, Page.SkinID, Page.ThisCustomer.LocaleSetting, CommonLogic.QueryStringUSInt("CategoryID")));
                }
                if (AppLogic.AppConfigBool("Tree.ShowSections"))
                {
                    tmpS.Append(AppLogic.LookupHelper("Section").ComponentArtTree(0, Page.SkinID, Page.ThisCustomer.LocaleSetting, CommonLogic.QueryStringUSInt("SectionID")));
                }
                //#if PRO
                // not supported in PRO version
                //#else
                if (AppLogic.AppConfigBool("Tree.ShowLibraries"))
                {
                    tmpS.Append(AppLogic.LookupHelper("Library").ComponentArtTree(0, Page.SkinID, Page.ThisCustomer.LocaleSetting, CommonLogic.QueryStringUSInt("LibraryID")));
                }
                if (AppLogic.AppConfigBool("Tree.ShowGenres"))
                {
                    tmpS.Append(AppLogic.LookupHelper("Genre").ComponentArtTree(0, Page.SkinID, Page.ThisCustomer.LocaleSetting, CommonLogic.QueryStringUSInt("GenreID")));
                }
                if (AppLogic.AppConfigBool("Tree.ShowVectors"))
                {
                    tmpS.Append(AppLogic.LookupHelper("Vector").ComponentArtTree(0, Page.SkinID, Page.ThisCustomer.LocaleSetting, CommonLogic.QueryStringUSInt("VectorID")));
                }
                //#endif
                if (AppLogic.AppConfigBool("Tree.ShowManufacturers"))
                {
                    tmpS.Append(AppLogic.LookupHelper("Manufacturer").ComponentArtTree(0, Page.SkinID, Page.ThisCustomer.LocaleSetting, CommonLogic.QueryStringUSInt("ManufacturerID")));
                }
                if (AppLogic.AppConfigBool("Tree.ShowCustomerService"))
                {
                    string custSvcXml = "<siteMapNode Text=\"" + XmlCommon.XmlEncodeAttribute(AppLogic.GetString("" + CommonLogic.IIF(AppLogic.IsAdminSite, "admin.", "") + "menu.CustomerService", Page.SkinID, Page.ThisCustomer.LocaleSetting)) + "\" NavigateUrl=\"t-service.aspx\">";
                    custSvcXml += AppLogic.AppConfig("Tree.CustomerServiceXml");
                    if (custSvcXml.Length != 0)
                    {
                        Regex m_ReMatch = new Regex(@"\(!(.*?)!\)");
                        MatchEvaluator m_ResourceMatch = new MatchEvaluator(ResourceMatchEvaluatorXmlEncoded);
                        custSvcXml = m_ReMatch.Replace(custSvcXml, m_ResourceMatch);
                    }
                    custSvcXml += "</siteMapNode>";
                    XmlDocument x = new XmlDocument();
                    try
                    {
                        x.LoadXml(custSvcXml);
                    }
                    catch
                    {
                        custSvcXml = "<siteMapNode Text=\"Invalid XML fragment in Tree.ShowCustomerService AppConfig parameter\" NavigateUrl=\"\" />";
                    }
                    tmpS.Append(custSvcXml);
                }
                tmpS.Append("</siteMap>");

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(tmpS.ToString());
                PageTree.LoadXml(doc);
            }
        }

        private void AddEntityMenuXsl(XmlDocument doc, String EntityName, HierarchicalTableMgr m_TblMgr, XmlNode mnuItem, int ForParentEntityID, String NodeClass)
        {
            if (mnuItem == null)
            {
                return;
            }
            StringWriter tmpS = new StringWriter();
            XslCompiledTransform xForm = new XslCompiledTransform();
            String XslFile = "EntityMenuList";
            xForm.Load(CommonLogic.SafeMapPath("EntityHelper/" + XslFile + ".xslt"));
            XsltArgumentList xslArgs = new XsltArgumentList();
            xslArgs.AddParam("entity", "", EntityName);
            xslArgs.AddParam("custlocale", "", Page.ThisCustomer.LocaleSetting);
            xslArgs.AddParam("deflocale", "", Localization.GetWebConfigLocale());
            xslArgs.AddParam("ForParentEntityID", "", ForParentEntityID);
            xslArgs.AddParam("adminsite", "", AppLogic.IsAdminSite);
            xslArgs.AddParam("nodeclass", "", NodeClass);
            xslArgs.AddParam("suppresstoparrow", "", CommonLogic.IIF(NodeClass.Length != 0, "1", "0"));
            xForm.Transform(m_TblMgr.XmlDoc, xslArgs, tmpS);
            if (AppLogic.AppConfigBool("XmlPackage.DumpTransform"))
            {
                try // don't let logging crash the site
                {
                    StreamWriter sw = File.CreateText(CommonLogic.SafeMapPath(String.Format("{0}images/{1}_{2}_{3}.xfrm.xml", CommonLogic.IIF(AppLogic.IsAdminSite, "../", ""), XslFile, EntityName, CommonLogic.IIF(AppLogic.IsAdminSite, "admin", "store"))));
                    sw.WriteLine(XmlCommon.PrettyPrintXml(tmpS.ToString()));
                    sw.Close();
                }
                catch { }
            }
            if (tmpS.ToString().Length != 0)
            {
                if (NodeClass.Length != 0) // this means we are adding to the ROOT level, not as children!
                {
                    // Create a document fragment to contain the XML to be inserted. 
                    XmlDocumentFragment docFrag = doc.CreateDocumentFragment();

                    // Set the contents of the document fragment. 
                    docFrag.InnerXml = tmpS.ToString();

                    // Add the children of the document fragment to the original document. 
                    mnuItem.ParentNode.InsertAfter(docFrag, mnuItem);

                    // now get rid of the parent placeholder node!
                    doc.SelectSingleNode("/SiteMap").RemoveChild(mnuItem);
                }
                else
                {
                    mnuItem.InnerXml = tmpS.ToString();
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }

    }
}
