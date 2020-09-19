// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/shippingexport.aspx.cs 3     9/13/06 4:27p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using System.Web;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    public partial class shippingexport : System.Web.UI.Page
    {

        private void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
            string PostedFormat = Request.Form["Format"];
            if (!IsNum(PostedFormat))
            {
                PostedFormat = Request.QueryString["Format"];
            }
            if (!IsNum(PostedFormat))
            {
                DataTable efl = ShippingImportCls.GetExportFormatList();
                ShowList(efl);
                efl.Dispose();
                return;
            }
            else
            {

                Int16 fmtID = Int16.Parse(PostedFormat);
                string exp = ShippingImportCls.ExportShippingString(fmtID);
                string filename = ShippingImportCls.GetExportFilename(fmtID);
                Response.ContentType = "text/csv";
                Response.ContentEncoding = Encoding.UTF8;
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                Response.Write(exp);
            }

        }


        public bool IsNum(string NumStr)
        {
            double tmp;
            bool success = false;
            try
            {
                tmp = double.Parse(NumStr);
                success = true;
            }
            catch
            {
                success = false;
            }
            return success;
        }


        void ShowList(DataTable efl)
        {
            Response.Write("<html><body>");
            Response.Write("<p>No format specified... try one of these:</p>");
            foreach (DataRow ef in efl.Rows)
            {
                Response.Write("<a href=\"?format=" + ef["id"] + "\">" + ef["Name"] + "</a><br/>");
            }
            Response.Write("</body></html>");
            Response.End();
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