// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/setssl.aspx.cs 2     9/03/06 8:41p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for setssl.
    /// </summary>
    public partial class setssl : AspDotNetStorefront.SkinBase
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            SectionTitle = "SetSSL On/Off";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            if (ThisCustomer.IsAdminSuperUser)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("set").ToUpper(CultureInfo.InvariantCulture) == "TRUE")
                {

                    bool OkToUseSSL = false;
                    String WorkerWindowInSSL = String.Empty;
                    String TestSSLUrl = AppLogic.GetStoreHTTPLocation(false).Replace("http://", "https://") + "empty.htm";
                    writer.Write("Testing URL: " + TestSSLUrl + "...<br/>");
                    try
                    {
                        WorkerWindowInSSL = CommonLogic.AspHTTP(TestSSLUrl,30);
                    }
                    catch (Exception ex)
                    {
                        writer.Write("Failed: " + CommonLogic.GetExceptionDetail(ex, "<br/>") + "<br/>");
                    }
                    writer.Write("Worker Window Contents: <textarea rows=\"10\" cols=\"60\">" + WorkerWindowInSSL + "</textarea><br/>");
                    if (WorkerWindowInSSL.IndexOf("Worker") != -1)
                    {
                        OkToUseSSL = true;
                    }
                    if (OkToUseSSL)
                    {
                        AppLogic.SetAppConfig("UseSSL", "true", false);
                        writer.Write("SSL ON");
                    }
                    else
                    {
                        AppLogic.SetAppConfig("UseSSL", "false", false);
                        writer.Write("<font color=\"red\"><b>No SSL certificate was found on your site. Please check with your hosting company! You must be able to invoke your store site using https:// before turning SSL on in this admin site!</b></font>");
                    }
                }
                else
                {
                    AppLogic.SetAppConfig("UseSSL", "false", false);
                    writer.Write("SSL OFF");
                }
                AppLogic.ClearCache();
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion
    }
}
