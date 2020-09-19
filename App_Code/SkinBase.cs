// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/App_Code/SkinBase.cs 22    10/04/06 6:22a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.IO;
using System.Resources;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Data;
using System.Xml;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Threading;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    public class SkinBase : System.Web.UI.Page
    {

        private static readonly String ro_SkinCookieName = "SkinID";

        private String m_ErrorMsg = String.Empty;
        private bool m_Editing = false;
        private bool m_DataUpdated = false;
        //private Regex m_ReMatch = new Regex(@"\(!(.*?)!\)");
        private bool m_DesignMode = false;
        private Customer m_ThisCustomer;
        private String m_SectionTitle = String.Empty;
        private TemplateBase m_Template = null;
        private string m_TemplateName = "template.ascx";
        private int m_SkinID = 1;
        private String m_SETitle = String.Empty;
        private String m_SEDescription = String.Empty;
        private String m_SEKeywords = String.Empty;
        private String m_SENoScript = String.Empty;
        private String m_TemplateFN = String.Empty;
        private bool m_DisableContents = false;
        private int m_DefaultSkinID = 1;
        private String m_GraphicsColor = String.Empty;
        private String m_ContentsBGColor = String.Empty;
        private String m_PageBGColor = String.Empty;
        private String m_IGD = String.Empty; // impersonation customer guid for admin phone order entry display of product pages, etc...
        //CUSTOM
        protected String m_ContentStyle = String.Empty;

        private System.Collections.Generic.Dictionary<string, EntityHelper> m_EntityHelpers = new System.Collections.Generic.Dictionary<string, EntityHelper>();
        private Parser m_Parser;

        public SkinBase(String TemplateName)
        {
            if (AppLogic.IsAdminSite && AppLogic.OnLiveServer() && AppLogic.UseSSL() && CommonLogic.ServerVariables("SERVER_PORT_SECURE") != "1")
            {
                if (AppLogic.RedirectLiveToWWW())
                {
                    HttpContext.Current.Response.Redirect("https://www." + AppLogic.LiveServer() + CommonLogic.ServerVariables("PATH_INFO") + "?" + CommonLogic.ServerVariables("QUERY_STRING"));
                }
                else
                {
                    HttpContext.Current.Response.Redirect("https://" + AppLogic.LiveServer() + CommonLogic.ServerVariables("PATH_INFO") + "?" + CommonLogic.ServerVariables("QUERY_STRING"));
                }
            }
            m_TemplateName = TemplateName;
            if (TemplateName.Length == 0)
            {
                m_TemplateName = "template.ascx";
            }
            m_DesignMode = (HttpContext.Current == null);
            m_DefaultSkinID = CommonLogic.IIF(AppLogic.IsAdminSite, 1, AppLogic.DefaultSkinID());
            if (!m_DesignMode)
            {
                m_EntityHelpers.Add("Category", AppLogic.CategoryEntityHelper);
                m_EntityHelpers.Add("Section", AppLogic.SectionEntityHelper);
                m_EntityHelpers.Add("Manufacturer", AppLogic.ManufacturerEntityHelper);
                m_EntityHelpers.Add("Distributor", AppLogic.DistributorEntityHelper);
#if PRO
			// not supported in PRO
#else
                m_EntityHelpers.Add("Library", AppLogic.LibraryEntityHelper);
                m_EntityHelpers.Add("Genre", AppLogic.GenreEntityHelper);
                m_EntityHelpers.Add("Vector", AppLogic.VectorEntityHelper);
#endif
            }
        }

        public SkinBase() : this(GetTemplateName()) {}

        private static string GetTemplateName()
        {
            string templateName = CommonLogic.QueryStringCanBeDangerousContent("template");
            AppLogic.CheckForScriptTag(templateName);
            if (templateName == null || templateName.Length == 0)
            {
                // undocumented feature:
                templateName = AppLogic.AppConfig("Template" + CommonLogic.GetThisPageName(false));
            }
            if (templateName == null || templateName.Length == 0)
            {
                templateName = "template";
            }
            if (templateName.Length > 0 && !templateName.EndsWith(".ascx", StringComparison.InvariantCultureIgnoreCase))
            {
                // undocumented feature:
                templateName += ".ascx";
            }
            return templateName;
        }

        private void FindLocaleStrings(Control c)
        {
            try
            {
                System.Web.UI.WebControls.Image i = c as System.Web.UI.WebControls.Image;
                if (i != null)
                {
                    if (i.ImageUrl.IndexOf("(!") >= 0)
                    {
                        i.ImageUrl = AppLogic.LocateImageURL(i.ImageUrl.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", string.Empty).Replace("!)", string.Empty), ThisCustomer.LocaleSetting);
                    }
                    if (i.AlternateText.IndexOf("(!") >= 0)
                    {
                        i.AlternateText = AppLogic.GetString(i.AlternateText, SkinID, ThisCustomer.LocaleSetting);
                    }
                }
                else
                {
                    System.Web.UI.WebControls.ImageButton b = c as System.Web.UI.WebControls.ImageButton;
                    if (b != null)
                    {
                        if (b.ImageUrl.IndexOf("(!") >= 0)
                       {
                            b.ImageUrl = AppLogic.LocateImageURL(b.ImageUrl.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", string.Empty).Replace("!)", string.Empty), ThisCustomer.LocaleSetting);
                        }
                    }
                    else
                    {
                        IEditableTextControl e = c as IEditableTextControl;
                        if (e != null)
                        {
                            if (!(e is ListControl))
                            {
                                e.Text = AppLogic.GetString(e.Text.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", "").Replace("!)", ""), SkinID, ThisCustomer.LocaleSetting);
                            }
                        }
                        else
                        {
                            IValidator v = c as IValidator;
                            if (v != null)
                            {
                                v.ErrorMessage = AppLogic.GetString(v.ErrorMessage.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", "").Replace("!)", ""), SkinID, ThisCustomer.LocaleSetting);
                            }
                            ITextControl t = c as ITextControl;
                            if (t != null)
                            {
                                t.Text = AppLogic.GetString(t.Text.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", "").Replace("!)", ""), SkinID, ThisCustomer.LocaleSetting);
                            }
                            Button b2 = c as Button;
                            if (b2 != null)
                            {
                                b2.Text = AppLogic.GetString(b2.Text.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", "").Replace("!)", ""), SkinID, ThisCustomer.LocaleSetting);
                            }
                            LinkButton l = c as LinkButton;
                            if (l != null)
                            {
                                l.Text = AppLogic.GetString(l.Text.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", "").Replace("!)", ""), SkinID, ThisCustomer.LocaleSetting);
                            }
                            HyperLink h = c as HyperLink;
                            if (h != null)
                            {
                                h.Text = AppLogic.GetString(h.Text.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", "").Replace("!)", ""), SkinID, ThisCustomer.LocaleSetting);
                            }
                            RadioButton r = c as RadioButton;
                            if (r != null)
                            {
                                r.Text = AppLogic.GetString(r.Text.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", "").Replace("!)", ""), SkinID, ThisCustomer.LocaleSetting);
                            }
                        }
                    }
                }
                if (c.HasControls())
                {
                    foreach (Control cx in c.Controls)
                    {
                        FindLocaleStrings(cx);
                    }
                }
            }
            catch { } // for admin site, a hack really, will fix with master pages
        }

        protected override void OnInit(EventArgs e)
        {
            if (HttpContext.Current != null)
            {
                m_ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

                if (AppLogic.AppConfigBool("GoogleCheckout.ShowOnCartPage"))
                {
                    String s = (String)HttpContext.Current.Cache.Get("GCCallbackLoadCheck");
                    if (s == null)
                    {
                        String notused = CommonLogic.AspHTTP(AppLogic.GetStoreHTTPLocation(false) + "gccallback.aspx?loadcheck=1", 10);
                        HttpContext.Current.Cache.Insert("GCCallbackLoadCheck", "true", null, System.DateTime.Now.AddMinutes(5), TimeSpan.Zero);
                    }
                }

                // don't fire disclaimer logic on admin pages
                if (!AppLogic.IsAdminSite && CommonLogic.QueryStringCanBeDangerousContent("ReturnURL").IndexOf(AppLogic.AppConfig("AdminDir")) == -1 && (AppLogic.AppConfigBool("SiteDisclaimerRequired") && CommonLogic.CookieCanBeDangerousContent("SiteDisclaimerAccepted", true).Length == 0))
                {
                    String ThisPageURL = CommonLogic.GetThisPageName(true) + "?" + CommonLogic.ServerVariables("QUERY_STRING");
                    Response.Redirect("disclaimer.aspx?returnURL=" + Server.UrlEncode(ThisPageURL));
                }

                bool IGDQueryClear = false;
                m_IGD = CommonLogic.QueryStringCanBeDangerousContent("IGD").Trim();
                if (m_IGD.Length == 0 && CommonLogic.ServerVariables("QUERY_STRING").IndexOf("IGD=") != -1)
                {
                    m_IGD = String.Empty; // there was IGD={blank} in the query string, so forcefully clear IGD!
                    IGDQueryClear = true;
                }
                bool IsStartOfImpersonation = m_IGD.Length != 0; // the url invocation starts the impersonation only!

                if (!IGDQueryClear && m_IGD.Length == 0)
                {
                    if (m_ThisCustomer.IsAdminUser)
                    {
                        // pull out the impersonation IGD from the customer session, if any
                        m_IGD = m_ThisCustomer.ThisCustomerSession["IGD"];
                    }
                }

                if (IGDQueryClear)
                {
                    // forcefully clear any IGD for this customer, just to be safe!
                    m_ThisCustomer.ThisCustomerSession["IGD"] = "";
                }

                Customer PhoneCustomer = null;
                if (m_IGD.Length != 0)
                {
                    if (m_ThisCustomer.IsAdminUser)
                    {
                        try
                        {
                            Guid IGD = new Guid(m_IGD);
                            PhoneCustomer = new Customer(IGD);
                        }
                        catch
                        {
                            m_ThisCustomer.ThisCustomerSession["IGD"] = "";
                            m_IGD = string.Empty;
                        }
                    }
                    if (PhoneCustomer != null && PhoneCustomer.HasCustomerRecord)
                    {
                        int ImpersonationTimeoutInMinutes = AppLogic.AppConfigUSInt("ImpersonationTimeoutInMinutes");
                        if (ImpersonationTimeoutInMinutes == 0)
                        {
                            ImpersonationTimeoutInMinutes = 20;
                        }
                        if (PhoneCustomer.ThisCustomerSession.LastActivity >= DateTime.Now.AddMinutes(-ImpersonationTimeoutInMinutes))
                        {
                            m_ThisCustomer.ThisCustomerSession["IGD"] = IGD;
                            m_ThisCustomer = PhoneCustomer; // build the impersonation customer the phone order customer
                        }
                        else
                        {
                            m_ThisCustomer.ThisCustomerSession["IGD"] = "";
                            m_IGD = string.Empty;
                            Response.Redirect("t-phoneordertimeout.aspx");
                        }
                    }
                }

                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Localization.GetWebConfigLocale());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(ThisCustomer.LocaleSetting);
                LoadSkinTemplate();
                m_Parser = new Parser(m_EntityHelpers, m_SkinID, m_ThisCustomer);

                if (this.HasControls()) // probably a web form
                {
                    foreach (Control c in this.Controls)
                    {
                        FindLocaleStrings(c);
                    }

                    Control ctl;
                    int i = 1;
                    int limitLoop = 1000;
                    if (m_Template != null && m_Template.Content != null)
                    {
                        while (this.Controls.Count > 0 && i <= limitLoop)
                        {
                            bool FilterItOut = false;
                            ctl = this.Controls[0];
                            LiteralControl l = ctl as LiteralControl;
                            if (l != null)
                            {
                                String txtVal = l.Text;
                                String txtVallwr = txtVal.ToLowerInvariant();
                                if (txtVallwr.IndexOf("<html") != -1 || txtVallwr.IndexOf("</html") != -1)
                                {
                                    FilterItOut = true; // remove outer html/body crap, as we're going to be moving the page controls INSIDE The skin
                                }
                            }
                            if (!FilterItOut)
                            {
                                // reparent the page control to be moved inside the skin template user control
                                m_Template.Content.Controls.Add(ctl);
                            }
                            else
                            {
                                this.Controls.RemoveAt(0);
                            }
                            i++;
                        }
                    }
                    // clear the controls (they were now all moved inside the template user control:
                    this.Controls.Clear();
                    // set the template user control to be owned by this page:
                    this.Controls.Add(m_Template);
                    // Now move the template child controls up to the page level so the ViewState will load 
                    while (m_Template.Controls.Count > 0)
                    {
                        this.Controls.Add(m_Template.Controls[0]);
                    }
                }
            }
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (HttpContext.Current != null)
            {
                if (!this.HasControls()) //Probably a web form so use the control management technique
                {
                    if (m_Template.Content != null)
                    {
                        //No controls so html must come from RenderContents. Create a literal to contain RenderContents
                        m_Template.Content.Controls.Add(ParseControl(CreateContent()));
                    }
                    this.Controls.Clear();
                    this.Controls.Add(m_Template);
                    // Now move the template child controls up to the page level so the ViewState will load 
                    while (m_Template.Controls.Count > 0)
                    {
                        this.Controls.Add(m_Template.Controls[0]);
                    }
                }
            }
            base.OnPreRender(e);
        }

        public string CreateContent()
        {
            StringBuilder tmpS = new StringBuilder(25000);

            //Create a literal to contain RenderContents
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = this.CreateHtmlTextWriter(sw);
            RenderContents(htw);
            htw.Flush();
            tmpS.Append(sw.ToString());

            htw.Close();
            sw.Close();

            return tmpS.ToString();
        }

        // replace any localization strings in the controls:
        private void IterateControls(ControlCollection controls)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                Control c = controls[i];
                if (c.ID != "PageContent")
                {
                    ProcessControl(c, true);
                }
            }
        }

        private void ProcessControl(Control ctl, bool includeChildren)
        {
            IEditableTextControl e = ctl as IEditableTextControl;
            if (e != null) e.Text = ReplaceTokens(e.Text);
            else
            {
                ITextControl t = ctl as ITextControl;
                if (t != null) t.Text = ReplaceTokens(t.Text);
            }
            IValidator v = ctl as IValidator;
            if (v != null) v.ErrorMessage = ReplaceTokens(v.ErrorMessage);
            //
            // TODO: Add other controls which might have replaceable properties here
            //
            if (includeChildren && ctl.HasControls())
            {
                IterateControls(ctl.Controls);
            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            if (m_SETitle.Length == 0)
            {
                m_SETitle = AppLogic.AppConfig("SE_MetaTitle");
            }
            if (m_SEDescription.Length == 0)
            {
                m_SEDescription = AppLogic.AppConfig("SE_MetaDescription");
            }
            if (m_SEKeywords.Length == 0)
            {
                m_SEKeywords = AppLogic.AppConfig("SE_MetaKeywords");
            }
            if (m_SENoScript.Length == 0)
            {
                m_SENoScript = AppLogic.AppConfig("SE_MetaNoScript");
            }
            IterateControls(Controls);
            base.Render(writer);
        }

        public void SetTemplate(String TemplateName)
        {
            if (TemplateName.Trim().Length != 0)
            {
                if (TemplateName.EndsWith(".ascx", StringComparison.InvariantCultureIgnoreCase))
                {
                    m_TemplateName = TemplateName;
                }
                else
                {
                    throw new ArgumentException("Skin template files (" + TemplateName + ") must end with .ascx for AspDotNetStorefront versions v5.x+!");
                }
            }
        }

        public void LoadSkinTemplate()
        {
            SkinID = 1;
            if (m_IGD.Length != 0 && !AppLogic.IsAdminSite)
            {
                m_TemplateName = "empty.ascx"; // force override for admin phone order pages
            }
            if (m_TemplateName.Length == 0)
            {
                m_TemplateName = "template.ascx";
            }
            m_TemplateFN = String.Empty;
            if (m_TemplateName.Length != 0)
            {

                SkinID = CommonLogic.QueryStringUSInt("SkinID");
                if (SkinID == 0)
                {
                    SkinID = CommonLogic.QueryStringUSInt("SiteID"); // for backwards compatibility with pre 6.2 versions
                }
                if (SkinID == 0 && CommonLogic.QueryStringCanBeDangerousContent("AffiliateID").Length != 0)
                {
                    DataSet ds = DB.GetDS("Select DefaultSkinID from Affiliate  " + DB.GetNoLock() + " where AffiliateID=" + CommonLogic.QueryStringUSInt("AffiliateID").ToString(), AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        SkinID = DB.RowFieldInt(ds.Tables[0].Rows[0], "DefaultSkinID");
                    }
                    ds.Dispose();
                }
                if (SkinID == 0)
                {
                    SkinID = CommonLogic.CookieUSInt(ro_SkinCookieName);
                }
                if (SkinID == 0)
                {
                    SkinID = m_DefaultSkinID;
                }
                if (SkinID == 0)
                {
                    SkinID = 1;
                }
                //Force admin site to use skin 1
                if (AppLogic.IsAdminSite)
                {
                    SkinID = 1;
                }
                if (!AppLogic.IsAdminSite)
                {
                    AppLogic.SetCookie(ro_SkinCookieName, SkinID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                }
                m_ThisCustomer.SkinID = SkinID;

                String _url = null;
                String LocaleTemplateURLCacheName = String.Format("template_{0}_{1}_{1}", m_TemplateName, SkinID.ToString(), ThisCustomer.LocaleSetting);
                String WebLocaleTemplateURLCacheName = String.Format("template_{0}_{1}_{1}", m_TemplateName, SkinID.ToString(), Localization.GetWebConfigLocale());
                String TemplateURLCacheName = String.Format("template_{0}_{1}_{1}", m_TemplateName, SkinID.ToString(), "");

                if (_url == null)
                {
                    // try customer locale:
                    _url = Path.Combine(SkinRoot, m_TemplateName.Replace(".ascx", "." + ThisCustomer.LocaleSetting + ".ascx"));

                    m_TemplateFN = CommonLogic.SafeMapPath(_url);
                    if (!CommonLogic.FileExists(m_TemplateFN))
                    {
                        // try default store locale path:
                        _url = Path.Combine(SkinRoot, m_TemplateName.Replace(".ascx", "." + Localization.GetWebConfigLocale() + ".ascx"));
                        m_TemplateFN = CommonLogic.SafeMapPath(_url);
                    }
                    if (!CommonLogic.FileExists(m_TemplateFN))
                    {
                        // try base (NULL) locale path:
                        _url = Path.Combine(SkinRoot, m_TemplateName);
                        m_TemplateFN = CommonLogic.SafeMapPath(_url);
                    }
                    if (AppLogic.CachingOn)
                    {
                        HttpContext.Current.Cache.Insert(TemplateURLCacheName, _url, null, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()), TimeSpan.Zero);
                    }
                }
                m_Template = (TemplateBase)LoadControl(_url);

            }
            if (m_Template != null)
            {
                m_Template.AppRelativeTemplateSourceDirectory = "~/"; // move it from skins/skin_N to root relative, so all links/image refs are from root of site
            }
        }

        private string ReplaceTokens(String s)
        {
            if (s.IndexOf("(!") == -1)
            {
                return s;
            }
            String tmpS = String.Empty;
            // process SKIN specific tokens here only:
            s = s.Replace("(!SECTION_TITLE!)", SectionTitle);
            if (SectionTitle.Length != 0)
            {
                s = s.Replace("(!SUPERSECTIONTITLE!)", "<div align=\"left\"><span class=\"SectionTitleText\">" + SectionTitle + "</span><br/><small>&nbsp;</small></div>");
            }
            else
            {
                s = s.Replace("(!SUPERSECTIONTITLE!)", "");
            }

            s = s.Replace("(!METATITLE!)", CommonLogic.IIF(CommonLogic.StringIsAlreadyHTMLEncoded(m_SETitle), m_SETitle, HttpContext.Current.Server.HtmlEncode(m_SETitle)));
            s = s.Replace("(!METADESCRIPTION!)", CommonLogic.IIF(CommonLogic.StringIsAlreadyHTMLEncoded(m_SEDescription), m_SEDescription, HttpContext.Current.Server.HtmlEncode(m_SEDescription)));
            s = s.Replace("(!METAKEYWORDS!)", CommonLogic.IIF(CommonLogic.StringIsAlreadyHTMLEncoded(m_SEKeywords), m_SEKeywords, HttpContext.Current.Server.HtmlEncode(m_SEKeywords)));
            s = s.Replace("(!SENOSCRIPT!)", m_SENoScript);
            if (m_GraphicsColor.Length == 0)
            {
                m_GraphicsColor = AppLogic.AppConfig("GraphicsColorDefault");
                if (m_GraphicsColor.Length == 0)
                {
                    m_GraphicsColor = String.Empty;
                }
            }
            if (m_ContentsBGColor.Length == 0)
            {
                m_ContentsBGColor = AppLogic.AppConfig("ContentsBGColorDefault");
                if (m_ContentsBGColor.Length == 0)
                {
                    m_ContentsBGColor = "#FFFFFF";
                }
            }
            if (m_PageBGColor.Length == 0)
            {
                m_PageBGColor = AppLogic.AppConfig("PageBGColorDefault");
                if (m_PageBGColor.Length == 0)
                {
                    m_PageBGColor = "#FFFFFF";
                }
            }
            s = s.Replace("(!GRAPHICSCOLOR!)", m_GraphicsColor);
            s = s.Replace("(!CONTENTSBGCOLOR!)", m_ContentsBGColor);
            s = s.Replace("(!PAGEBGCOLOR!)", m_PageBGColor);
            s = s.Replace("(!CONTENTSTYLE!)", m_ContentStyle);
            s = GetParser.ReplaceTokens(s);
            return s;
        }

        protected virtual void RenderContents(System.Web.UI.HtmlTextWriter writer) { }

        public Customer ThisCustomer
        {
            get
            {
                return m_ThisCustomer;
            }
        }

        public Parser GetParser
        {
            get
            {
                return m_Parser;
            }
        }

        public String SectionTitle
        {
            get
            {
                return m_SectionTitle;
            }
            set
            {
                m_SectionTitle = value;
            }
        }

        public String ErrorMsg
        {
            get
            {
                return m_ErrorMsg;
            }
            set
            {
                m_ErrorMsg = value;
            }
        }

        public String SETitle
        {
            get
            {
                return m_SETitle;
            }
            set
            {
                m_SETitle = value;
            }
        }

        public String IGD
        {
            get
            {
                return m_IGD;
            }
        }

        public String SEKeywords
        {
            get
            {
                return m_SEKeywords;
            }
            set
            {
                m_SEKeywords = value;
            }
        }

        public String SEDescription
        {
            get
            {
                return m_SEDescription;
            }
            set
            {
                m_SEDescription = value;
            }
        }

        public String SENoScript
        {
            get
            {
                return m_SENoScript;
            }
            set
            {
                m_SENoScript = value;
            }
        }

        public String ContentsBGColor
        {
            get
            {
                return m_ContentsBGColor;
            }
            set
            {
                m_ContentsBGColor = value;
            }
        }

        public String PageBGColor
        {
            get
            {
                return m_PageBGColor;
            }
            set
            {
                m_PageBGColor = value;
            }
        }

        public String GraphicsColor
        {
            get
            {
                return m_GraphicsColor;
            }
            set
            {
                m_GraphicsColor = value;
            }
        }

        public bool Editing
        {
            get
            {
                return m_Editing;
            }
            set
            {
                m_Editing = value;
            }
        }

        public bool DisableContents
        {
            get
            {
                return m_DisableContents;
            }
            set
            {
                m_DisableContents = value;
            }
        }

        public bool DataUpdated
        {
            get
            {
                return m_DataUpdated;
            }
            set
            {
                m_DataUpdated = value;
            }
        }

        new public int SkinID
        {
            get
            {
                return m_SkinID;
            }
            set
            {
                m_SkinID = value;
            }
        }

        public string SkinRoot
        {
            get
            {
                return String.Format("skins/skin_{0}/", this.SkinID);
            }
        }

        public System.Collections.Generic.Dictionary<string, EntityHelper> EntityHelpers
        {
            get
            {
                return m_EntityHelpers;
            }
        }

        public string SkinImages
        {
            get
            {
                return String.Format("skins/skin_{0}/images/", this.SkinID);
            }
        }

        public static void RequireSecurePage()
        {
            AppLogic.RequireSecurePage();
        }

        static public void GoNonSecureAgain()
        {
            if (AppLogic.UseSSL())
            {
                if (AppLogic.OnLiveServer() && CommonLogic.ServerVariables("SERVER_PORT_SECURE") == "1")
                {
                    HttpContext.Current.Response.Redirect(AppLogic.GetStoreHTTPLocation(false) + CommonLogic.GetThisPageName(false) + "?" + CommonLogic.ServerVariables("QUERY_STRING"));
                }
            }
        }

        public void RequireCustomerRecord()
        {
            if (!m_ThisCustomer.HasCustomerRecord)
            {
                m_ThisCustomer.RequireCustomerRecord();
            }
        }

        public void RequiresLogin(String ReturnURL)
        {
            if (!m_ThisCustomer.IsRegistered)
            {
                Response.Redirect("signin.aspx?returnurl=" + Server.UrlEncode(ReturnURL));
            }
        }

        public void SetMetaTags(String SETitle, String SEKeywords, String SEDescription, String SENoScript)
        {
            m_SETitle = SETitle;
            m_SEDescription = SEKeywords;
            m_SEKeywords = SEDescription;
            m_SENoScript = SENoScript;
        }
    }
}
