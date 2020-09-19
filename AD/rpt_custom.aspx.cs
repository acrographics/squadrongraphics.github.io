// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/rpt_custom.aspx.cs 2     8/19/06 8:48p Buddy $
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

    public partial class rpt_custom : System.Web.UI.Page
    {
        private Customer cust;
        private String ActiveReport;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
            ActiveReport = String.Empty;

            if (!IsPostBack)
            {
                loadReports();
            }
        }

        protected void ddReports_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActiveReport = ddReports.SelectedValue;
            if (ActiveReport.Length != 0 && ActiveReport != "--SELECT REPORT--")
            {
                RunReport(ActiveReport);
            }
        }

        private void RunReport(String ReportName)
        {
            try
            {
                if (ReportName.Length != 0)
                {
                    IDataReader rs = DB.GetRS("Select * from CustomReport " + DB.GetNoLock() + " where lower(name)=" + DB.SQuote(ReportName.ToLowerInvariant()));
                    if (rs.Read())
                    {
                        ResultsPanel.Visible = true;
                        String sql = DB.RSField(rs, "SQLCommand");
                        DataSet ds = DB.GetDS(sql, false);
                        ResultsGrid.DataSource = ds;
                        ResultsGrid.DataBind();
                        ds.Dispose();
                    }
                    rs.Close();
                }
                else
                {
                    ResultsPanel.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ltError.Text = CommonLogic.GetExceptionDetail(ex, "<br/>");
            }
        }

        private void loadReports()
        {
            ddReports.Items.Clear();
            ddReports.Items.Add("--SELECT REPORT--");
            DataSet ds = DB.GetDS("select distinct Name from CustomReport " + DB.GetNoLock() + " order by Name", AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                ListItem myNode = new ListItem();
                myNode.Value = DB.RowField(row, "Name");
                ddReports.Items.Add(myNode);
            }
            ds.Dispose();
        }

    }
}