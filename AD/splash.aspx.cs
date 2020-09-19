// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/splash.aspx.cs 15    10/04/06 12:00p Redwoodtree $
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
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{

    public partial class splash : System.Web.UI.Page
    {
        Customer ThisCustomer;
        private int m_SkinID = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            resetError("", false);
            if (CommonLogic.QueryStringCanBeDangerousContent("CacheMenus").Length != 0)
            {
                AppLogic.AppConfigTable["CacheMenus"].ConfigValue = CommonLogic.QueryStringBool("CacheMenus").ToString();
                AppLogic.ClearCache();
            }
            CustomerSession.StaticClear();
            ltDateTime.Text = Localization.ToNativeDateTimeString(System.DateTime.Now);
            if (!IsPostBack)
            {
                DateTime NextEncryptKeyChangeDate = System.DateTime.MinValue;
                try
                {
                    NextEncryptKeyChangeDate = System.DateTime.Parse(Security.GetEncryptParam("NextKeyChange"));
                }
                catch { }
                bool StoringCCInDB = AppLogic.StoreCCInDB();
                if (StoringCCInDB && NextEncryptKeyChangeDate < System.DateTime.Now)
                {
                    ChangeEncryptKeyReminder.Visible = true;
                }
                //setSSL(true);
                if (ThisCustomer.IsAdminSuperUser)
                {
                    if (!AppLogic.UseSSL())
                    {
                        lnkSSL.Text = AppLogic.GetString("admin.splash.aspx.11", m_SkinID, ThisCustomer.LocaleSetting);
                    }
                    else
                    {
                        lnkSSL.Text = AppLogic.GetString("admin.splash.aspx.12", m_SkinID, ThisCustomer.LocaleSetting);
                    }
                }
                else
                {
                    lnkSSL.Text = String.Empty; // "Cannot set SSL - not SA";
                }

                loadInformation();
                loadGrids();
                loadSummaryReport();
                loadFeeds();
            }

        }

        protected void loadFeeds()
        {
            pnlSecurityFeed.Visible = AppLogic.AppConfigBool("Admin.ShowSecurityFeed");
            pnlNewsFeed.Visible = AppLogic.AppConfigBool("Admin.ShowNewsFeed");
            pnlSponsorFeed.Visible = AppLogic.AppConfigBool("Admin.ShowSponsorFeed");
        }

        protected void loadSummaryReport()
        {
            if (ThisCustomer.IsAdminSuperUser)
            {
                ltCustomerStats.Text = AppLogic.GetCustomerStatsBox();
                ltStatistics.Text = AppLogic.GetSiteStatsBox(AppLogic.ro_TXStateAuthorized, false, ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                Literal1.Text = AppLogic.GetSiteStatsBox(AppLogic.ro_TXStateCaptured, false, ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                Literal2.Text = AppLogic.GetSiteStatsBox(AppLogic.ro_TXStateVoided, true, ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                Literal3.Text = AppLogic.GetSiteStatsBox(AppLogic.ro_TXStateRefunded, true, ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                StatsTable.Visible = true;
            }
            else
            {
                StatsTable.Visible = false;
            }
        }

        protected void loadGrids()
        {
            //Orders
            string SummaryReportFields = "OrderNumber,OrderDate,OrderTotal,FirstName,LastName,ShippingMethod,isnull(MaxMindFraudScore, -1) MaxMindFraudScore ";
            String summarySQL = "SELECT " + SummaryReportFields + " from Orders " + DB.GetNoLock() + " where IsNew=1 ORDER BY OrderDate DESC";
            DataSet ds = DB.GetDS(summarySQL, false);
            gOrders.DataSource = ds;
            gOrders.DataBind();
            ds.Dispose();

            //Customers
            SummaryReportFields = "CustomerID,RegisterDate,FirstName,LastName ";
            summarySQL = "SELECT TOP 25 " + SummaryReportFields + " from Customer " + DB.GetNoLock() + " WHERE IsRegistered=1 ORDER BY RegisterDate DESC";
            ds = DB.GetDS(summarySQL, false);
            gCustomers.DataSource = ds;
            gCustomers.DataBind();
            ds.Dispose();
        }


        protected void loadInformation()
        {
            ltOnLiveServer.Text = AppLogic.OnLiveServer().ToString();
            ltServerPortSecure.Text = CommonLogic.ServerVariables("SERVER_PORT_SECURE").ToString();
            ltStoreVersion.Text = CommonLogic.GetVersion();
            ltCaching.Text = (AppLogic.AppConfigBool("CacheMenus") ? AppLogic.GetString("admin.common.OnUC", m_SkinID, ThisCustomer.LocaleSetting) + "<img src=\"images/spacer.gif\" width=\"10\" height=\"1\" /><a href=\"splash.aspx?cachemenus=false\">" + AppLogic.GetString("admin.splash.aspx.19", m_SkinID, ThisCustomer.LocaleSetting) + "</a>" : AppLogic.GetString("admin.common.OffUC", m_SkinID, ThisCustomer.LocaleSetting) + "<img src=\"images/spacer.gif\" width=\"10\" height=\"1\" /><a href=\"splash.aspx?cachemenus=true\">" + AppLogic.GetString("admin.splash.aspx.18", m_SkinID, ThisCustomer.LocaleSetting) + "</a>");
            ltWebConfigLocaleSetting.Text = Localization.GetWebConfigLocale();
            ltSQLLocaleSetting.Text = Localization.GetSqlServerLocale();
            ltCustomerLocaleSetting.Text = ThisCustomer.LocaleSetting;
            ltPaymentGateway.Text = AppLogic.ActivePaymentGatewayRAW();
            ltUseLiveTransactions.Text = (AppLogic.AppConfigBool("UseLiveTransactions") ? AppLogic.GetString("admin.splash.aspx.20", m_SkinID, ThisCustomer.LocaleSetting) : AppLogic.GetString("admin.splash.aspx.21", m_SkinID, ThisCustomer.LocaleSetting));
            ltTransactionMode.Text = AppLogic.AppConfig("TransactionMode").ToString();
            ltPaymentMethods.Text = AppLogic.AppConfig("PaymentMethods").ToString();
            ltMicroPayEnabled.Text = AppLogic.MicropayIsEnabled().ToString();
            CardinalEnabled.Text = AppLogic.AppConfigBool("CardinalCommerce.Centinel.Enabled").ToString();
            StoreCC.Text = AppLogic.StoreCCInDB().ToString();
            GatewayRecurringBilling.Text = AppLogic.AppConfigBool("Recurring.UseGatewayInternalBilling").ToString();
            ltUseSSL.Text = AppLogic.UseSSL().ToString();
            PrimaryCurrency.Text = Localization.GetPrimaryCurrency();

            if (!ThisCustomer.IsAdminSuperUser)
            {
                MonthlyMaintenancePrompt.Visible = false;
                lnkSSL.Visible = false;
            }
            lnkGateway.NavigateUrl = "appconfig.aspx?searchfor=" + AppLogic.ActivePaymentGatewayRAW();
        }

        protected void lnkSSL_Click(object sender, EventArgs e)
        {
            if (ThisCustomer.IsAdminSuperUser)
            {
                //if (CommonLogic.QueryStringCanBeDangerousContent("set").ToUpper(CultureInfo.InvariantCulture) == "TRUE")
                if (!AppLogic.UseSSL())
                {

                    bool OkToUseSSL = false;
                    String WorkerWindowInSSL = String.Empty;
                    try
                    {
                        WorkerWindowInSSL = CommonLogic.AspHTTP(AppLogic.GetStoreHTTPLocation(false).Replace("http://", "https://") + "empty.htm",10);
                    }
                    catch { }
                    if (WorkerWindowInSSL.IndexOf("Worker") != -1)
                    {
                        OkToUseSSL = true;
                    }
                    if (OkToUseSSL)
                    {
                        AppLogic.AppConfigTable["UseSSL"].ConfigValue = "true";
                        AppLogic.AppConfigTable["RedirectLiveToWWW"].ConfigValue = "true";
                        resetError(AppLogic.GetString("admin.common.SSLOnUC", m_SkinID, ThisCustomer.LocaleSetting), false);
                        lnkSSL.Text = AppLogic.GetString("admin.splash.aspx.12", m_SkinID, ThisCustomer.LocaleSetting);
                    }
                    else
                    {
                        AppLogic.AppConfigTable["UseSSL"].ConfigValue = "false";
                        AppLogic.AppConfigTable["RedirectLiveToWWW"].ConfigValue = "false";
                        resetError(AppLogic.GetString("admin.common.NoSSLCertFoundOnYourSite", m_SkinID, ThisCustomer.LocaleSetting), false);
                        lnkSSL.Text = AppLogic.GetString("admin.splash.aspx.22", m_SkinID, ThisCustomer.LocaleSetting);
                        lnkSSL.Attributes.Add("onClick", "alert('" + AppLogic.GetString("admin.common.NoSSLCertFoundOnYourSite", m_SkinID, ThisCustomer.LocaleSetting) + "')");
                    }
                }
                else
                {
                    AppLogic.AppConfigTable["UseSSL"].ConfigValue = "false";
                    AppLogic.AppConfigTable["RedirectLiveToWWW"].ConfigValue = "false";
                    resetError(AppLogic.GetString("admin.common.SSLOffUC", m_SkinID, ThisCustomer.LocaleSetting), false);
                    lnkSSL.Text = AppLogic.GetString("admin.splash.aspx.11", m_SkinID, ThisCustomer.LocaleSetting);
                }
                AppLogic.ClearCache();
            }
            else
            {
                lnkSSL.Text = String.Empty; // "Cannot set SSL - not SA";
            }
        }

        protected void resetError(string error, bool isError)
        {
            string str = "<font class=\"noticeMsg\">" + AppLogic.GetString("admin.common.Notice", m_SkinID, ThisCustomer.LocaleSetting) + "</font>&nbsp;&nbsp;&nbsp;";
            if (isError)
            {
                str = "<font class=\"noticeMsg\">" + AppLogic.GetString("admin.common.Error", m_SkinID, ThisCustomer.LocaleSetting) + "</font>&nbsp;&nbsp;&nbsp;";
            }

            if (error.Length > 0)
            {
                str += error + "";
            }
            else
            {
                str = "";
            }

            ltError.Text = str;
        }


        protected void gOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                //Order
                e.Row.Cells[0].Text = AppLogic.GetString("admin.common.Order", m_SkinID, ThisCustomer.LocaleSetting);
                //Date
                e.Row.Cells[1].Text = AppLogic.GetString("admin.common.Date", m_SkinID, ThisCustomer.LocaleSetting);
                //Customer
                e.Row.Cells[2].Text = AppLogic.GetString("admin.common.Customer", m_SkinID, ThisCustomer.LocaleSetting);
                //Shipping
                e.Row.Cells[3].Text = AppLogic.GetString("admin.common.Shipping", m_SkinID, ThisCustomer.LocaleSetting);
                //Total
                e.Row.Cells[4].Text = AppLogic.GetString("admin.common.Total", m_SkinID, ThisCustomer.LocaleSetting);
            }
        }

        protected void gCustomers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                //CustomerID
                e.Row.Cells[0].Text = AppLogic.GetString("admin.common.CustomerID", m_SkinID, ThisCustomer.LocaleSetting);
                //Date
                e.Row.Cells[1].Text = AppLogic.GetString("admin.common.Date", m_SkinID, ThisCustomer.LocaleSetting);
                //Customer
                e.Row.Cells[2].Text = AppLogic.GetString("admin.common.Customer", m_SkinID, ThisCustomer.LocaleSetting);
            }
        }
    }
}