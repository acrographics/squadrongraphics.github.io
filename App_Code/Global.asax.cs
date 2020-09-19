// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/App_Code/Global.asax.cs 28    9/25/06 2:47p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.IO;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Resources;
using ASPDNSF.URLRewriter;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for Global.
    /// </summary>
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Global()
        {
            InitializeComponent();
        }

        protected void Application_Start(Object sender, EventArgs e)
        {
            AppStartLogger.ResetLog();
            AppStartLogger.WriteLine("Starting AspDotNetStorefront...");

            // test DBConn before we do anything:
            AppStartLogger.WriteLine("Testing DBConn...");
            SqlConnection dbconn = new SqlConnection();
            dbconn.ConnectionString = DB.GetDBConn();
            dbconn.Open();
            dbconn.Close();
            dbconn.Dispose();
            AppStartLogger.WriteLine("DBConn OK.");

            AppStartLogger.WriteLine("AppLogic.ApplicationStart...");
            AppLogic.ApplicationStart(true);
            AppStartLogger.WriteLine("AppLogic.ApplicationStart OK.");
            AppStartLogger.WriteLine("AspDotNetStorefront Started OK.");
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            // this is an undocumented/unsupported feature:
            if (AppLogic.AppConfigBool("SendEMailOnApplicationError"))
            {
                String tmp = "Error: " + Server.GetLastError().InnerException.ToString() + "\n\n"
                    + "URL: " + Request.Url.ToString() + "\n\n"
                    + "Referer: " + Request.UrlReferrer.ToString() + "\n\n"
                    + "LastError: " + Server.GetLastError().ToString() + "\n\n"
                    + "Message: " + Server.GetLastError().Message.ToString() + "\n\n"
                    + "Source: " + Server.GetLastError().Source.ToString() + "\n\n"
                    + "TargetSite: " + Server.GetLastError().TargetSite.Name;
                AppLogic.SendMail("*** " + AppLogic.AppConfig("StoreName").ToUpperInvariant() + " SITE ERROR ***", tmp, false);
            }
        }

        public void Application_PreRequestHandlerExecute(Object sender, EventArgs e)
        {
            if (!AppLogic.AppIsStarted)
            {
                Response.Redirect("restarting.htm");
            } 
            if (CommonLogic.ApplicationBool("SiteDownForMaintenance"))
            {
                String URL = CommonLogic.Application("SiteDownForMaintenancePage");
                if (URL.Length == 0)
                {
                    URL = CommonLogic.Application("SiteDownForMaintenanceURL"); // in case someone is using the instructions in the forums!
                }
                if (URL.Length == 0)
                {
                    URL = "default.htm";
                }
                Response.Redirect(URL);
            }
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            if (Request.Path.IndexOf('\\') >= 0 || System.IO.Path.GetFullPath(Request.PhysicalPath) != Request.PhysicalPath)
            {
                throw new HttpException(404, "not found");
            }
            bool IsAdmin = CommonLogic.ApplicationBool("IsAdminSite");
            HttpContext.Current.Items.Add("IsAdminSite", IsAdmin.ToString().ToLowerInvariant());
            HttpContext.Current.Items.Add("RequestedPage", HttpContext.Current.Request.Url.AbsolutePath.Split('/')[HttpContext.Current.Request.Url.AbsolutePath.Split('/').Length - 1]);
            if (Request.Path.ToLowerInvariant().IndexOf(".asmx") == -1)
            {
                ASPDNSF.URLRewriter.Rewriter.Process();
            }
        }

        protected void Application_EndRequest(Object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {            
            if (AppLogic.AppConfigTable == null)
            {
                // test DBConn before we do anything:

                SqlConnection dbconn = new SqlConnection();
                dbconn.ConnectionString = DB.GetDBConn();
                dbconn.Open();
                dbconn.Close();
                dbconn.Dispose();

                throw new ArgumentException("AppConfig Table Is NULL!! First, Please re-start your web site, and re-try. If that does not resolve it, please contact support");
            }
            if (AppLogic.AppConfigBool("IPAddress.RefuseRestrictedIPsFromSite"))
            {
                String IPAddress = CommonLogic.ServerVariables("REMOTE_ADDR");
                if (AppLogic.IPIsRestricted(IPAddress))
                {
                    Response.Redirect("refused.htm");
                }
            }
            if (Context.Request.Url.AbsoluteUri.ToLowerInvariant().IndexOf("webresource.axd") == -1)
            {
                String Referrer = CommonLogic.PageReferrer();
                // store their referrer IF it is the first one, and it's not coming from internal web site
                // we only want the very first referral
                if (Referrer.Length != 0 && CommonLogic.CookieCanBeDangerousContent(Customer.ro_ReferrerCookieName, true).Length == 0 && Referrer.IndexOf("localhost") == -1 && Referrer.IndexOf("192.") == -1 && Referrer.IndexOf("10.") == -1 && Referrer.IndexOf(AppLogic.LiveServer()) == -1)
                {
                    AppLogic.SetCookie(Customer.ro_ReferrerCookieName, Referrer, new TimeSpan(365, 0, 0, 0, 0));
                }

                string cookiedata;
                HttpCookie authcookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authcookie == null)
                {
                    if (CommonLogic.QueryStringCanBeDangerousContent("AffiliateID") != "")
                    {
                        AppLogic.SetCookie(Customer.ro_AffiliateCookieName, CommonLogic.QueryStringCanBeDangerousContent("AffiliateID"), new TimeSpan(365, 0, 0, 0, 0));
                    }
                    if (CommonLogic.QueryStringCanBeDangerousContent("AffID") != "")
                    {
                        AppLogic.SetCookie(Customer.ro_AffiliateCookieName, CommonLogic.QueryStringCanBeDangerousContent("AffID"), new TimeSpan(365, 0, 0, 0, 0));
                    }
                    if (CommonLogic.QueryStringCanBeDangerousContent("LocaleSetting") != "")
                    {
                        AppLogic.SetCookie(Customer.ro_LocaleSettingCookieName, CommonLogic.QueryStringCanBeDangerousContent("LocaleSetting"), new TimeSpan(365, 0, 0, 0, 0));
                    }
                    if (CommonLogic.QueryStringCanBeDangerousContent("CurrencySetting") != "")
                    {
                        AppLogic.SetCookie(Customer.ro_CurrencySettingCookieName, CommonLogic.QueryStringCanBeDangerousContent("CurrencySetting"), new TimeSpan(365, 0, 0, 0, 0));
                    }
                    if (CommonLogic.QueryStringCanBeDangerousContent("VATSetting") != "")
                    {
                        AppLogic.SetCookie(Customer.ro_VATSettingCookieName, CommonLogic.QueryStringCanBeDangerousContent("VATSetting"), new TimeSpan(365, 0, 0, 0, 0));
                    }
                    Context.User = new AspDotNetStorefrontPrincipal(new Customer(AppLogic.IsAdminSite));
                }
                else
                {
                    cookiedata = authcookie.Value;
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookiedata);
                    if (ticket.Name.Trim() != string.Empty)
                    {
                        Guid g = new Guid(ticket.Name);
                        DB.ExecuteSQL("update Customer set LastIPAddress=" + DB.SQuote(CommonLogic.ServerVariables("REMOTE_ADDR")) + " where CustomerGUID=" + DB.SQuote(g.ToString()));
                        Customer ThisCustomer = new Customer(g, AppLogic.IsAdminSite);
                        Context.User = new AspDotNetStorefrontPrincipal(ThisCustomer);
                    }
                    else
                    {
                        FormsAuthentication.SignOut();
                        Context.User = new AspDotNetStorefrontPrincipal(new Customer(AppLogic.IsAdminSite));
                    }
                }
                if (AppLogic.IsAdminSite && ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer.IsAdminUser && ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer.LastActivity < DateTime.Now.AddMinutes(-AppLogic.SessionTimeout()))
                {
                    FormsAuthentication.SignOut();
                    FormsAuthentication.RedirectToLoginPage();
                }
                else
                {
                    ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer.ThisCustomerSession.UpdateCustomerSession(null, null);
                }
            }
        }

        protected void Session_End(Object sender, EventArgs e)
        {
        }

        protected void Application_End(Object sender, EventArgs e)
        {
            CustomerSession.StaticClear();
        }

        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
        }
        #endregion
    }
}

