// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/importProductsFromXML.aspx.cs 2     10/04/06 12:00p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using System.Web;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for importProductsFromXML
    /// </summary>
    public partial class importProductsFromXML : System.Web.UI.Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
            Server.ScriptTimeout = 1000000;

            if (!IsPostBack)
            {
                divReview.Visible = false;
            }
        }

        protected void resetError(string error, bool isError)
        {
            string str = "<font class=\"noticeMsg\">NOTICE:</font>&nbsp;&nbsp;&nbsp;";
            if (isError)
                str = "<font class=\"errorMsg\">ERROR:</font>&nbsp;&nbsp;&nbsp;";

            if (error.Length > 0)
                str += error + "";
            else
                str = "";

            this.ltError.Text = str;
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            String XmlName = "Import_" + Localization.ToNativeShortDateString(System.DateTime.Now).Replace(" ", "").Replace("/", "").Replace(":", "").Replace(".", "");
            // handle file upload:
            try
            {
                String Image1 = String.Empty;
                HttpPostedFile Image1File = fuFile.PostedFile;
                String XmlFile = CommonLogic.SafeMapPath("../images" + "/" + XmlName + ".xml");
                if (Image1File.ContentLength != 0)
                {
                    Image1File.SaveAs(XmlFile);
                }

                if (fuFile.FileName.Length != 0)
                {
                    resetError("File Uploaded, please review below.", false);
                    divReview.Visible = true;
                    Import.ProcessXmlImportFile(XmlFile);
                    ltResults.Text = "<a href=\"../images/import.htm\" target=\"_blank\">View Import Log</a>";
                }
                else
                {
                    divReview.Visible = false;
                    resetError("File Uploaded Error:<br/> Please a select a File to Upload" , true);
                }

            }
            catch (Exception ex)
            {
                divReview.Visible = false;
                resetError("File Uploaded Error:<br/>" + CommonLogic.GetExceptionDetail(ex, "<br/>"), true);
            }
        }

        protected void btnAccept_Click(object sender, EventArgs e)
        {
            DB.ExecuteLongTimeSQL("aspdnsf_ClearAllImportFlags", 1000);
            resetError("<span style=\"color: red; font-weight: bold;\">IMPORT ACCEPTED</span>", false);
            divReview.Visible = false;
        }

        protected void btnUndo_Click(object sender, EventArgs e)
        {
            DB.ExecuteLongTimeSQL("aspdnsf_UndoImport", 1000);
            resetError("<span style=\"color: red; font-weight: bold;\">IMPORT HAS BEEN UNDONE</span>", false);
            divReview.Visible = false;
        }
    }
}
