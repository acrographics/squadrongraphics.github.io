// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/securitylog.aspx.cs 8     10/03/06 7:56p Redwoodtree $
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

    public partial class securitylog : AspDotNetStorefront.SkinBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Server.ScriptTimeout = 5000000;

            if (!ThisCustomer.IsAdminSuperUser)
            {
                Response.Redirect("default.aspx");
            }
            if (!IsPostBack)
            {
                LoadGridView();
                //Button1.Attributes.Add("onClick", "javascript: return confirm('Are you sure you want to clear your security log? This action is not undo-able!')");
                Security.AgeSecurityLog();
            }
        }

        private void LoadGridView()
        {
            DataSet ds = DB.GetDS("select SecurityAction Action,Description,ActionDate Date,UpdatedBy CustomerID,c.EMail from SecurityLog with (NOLOCK) left outer join Customer c with (NOLOCK) on SecurityLog.UpdatedBy=c.CustomerID order by ActionDate desc", false);
            GridView1.DataSource = ds;
            GridView1.DataBind();
            ds.Dispose();
        }

        //protected void Button1_Click(object sender, EventArgs e)
        //{
        //    DB.ExecuteSQL("truncate table SecurityLog");
        //    LoadGridView();
        //}

        protected void Refresh_Click(object sender, EventArgs e)
        {
            LoadGridView();
        }
        protected void GridView1_RowDataBound1(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (TableCell c in e.Row.Cells)
                {
                    String DD = c.Text;
                    // do in place decrypt:
                    String DDDecrypted = Security.UnmungeString(DD);
                    if (DDDecrypted.StartsWith(Security.ro_DecryptFailedPrefix, StringComparison.InvariantCultureIgnoreCase))
                    {
                        DDDecrypted = c.Text;
                    }
                    c.Text = DDDecrypted;
                    if (c.Text.Length > 70)
                    {
                        c.Text = CommonLogic.WrapString(c.Text, 70, "<br/>");
                        //c.Text = "<textarea READONLY rows=\"12\" cols=\"50\">" + c.Text + "</textarea>";
                    }
                }
            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            GridView1.EditIndex = -1;
            LoadGridView();
        }

    }
}