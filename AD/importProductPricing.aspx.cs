// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/importProductPricing.aspx.cs 4     10/04/06 12:00p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Xml.Xsl;
using System.Xml;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using AspDotNetStorefrontCommon;
using AspDotNetStorefrontExcelWrapper;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for importProductPricing
    /// </summary>
    public partial class importProductPricing : System.Web.UI.Page
    {
        private Customer cust;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
            Server.ScriptTimeout = 1000000;

            cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            if (!IsPostBack)
            {
                trResults.Visible = false;
            }
        }

        protected void LoadData()
        {
            
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
            trResults.Visible = true;
            string errors = "";

            if (fuFile.HasFile)
            {
                HttpPostedFile PostedFile = fuFile.PostedFile;
                if (!PostedFile.FileName.EndsWith("xls", StringComparison.InvariantCultureIgnoreCase) && !PostedFile.FileName.EndsWith("xml", StringComparison.InvariantCultureIgnoreCase) && !PostedFile.FileName.EndsWith("csv", StringComparison.InvariantCultureIgnoreCase) && PostedFile.FileName.Trim() != "")
                {
                    errors = "<b>Invalid file type submitted. " + CommonLogic.IIF(PostedFile.ContentLength == 0, "The file contents were empty.", "Please submit only XML, XLS (Excel), or CSV (comma delimited) files.") + "</b>";
                }
                else
                {
                    string filename = System.Guid.NewGuid().ToString();
                    string FullFilePath = CommonLogic.SafeMapPath("../images") + "\\" + filename + PostedFile.FileName.ToLowerInvariant().Substring(PostedFile.FileName.ToLowerInvariant().LastIndexOf('.'));
                    string xml = String.Empty;

                    PostedFile.SaveAs(FullFilePath);
                    StreamReader sr = new StreamReader(FullFilePath);
                    string filecontent = sr.ReadToEnd();
                    sr.Close();

                    if (PostedFile.FileName.EndsWith("csv", StringComparison.InvariantCultureIgnoreCase))
                    {
                        xml = "<productlist>";
                        string[] rows = filecontent.Split(Environment.NewLine.ToCharArray());
                        for (int i = 1; i < rows.Length; i++)
                        {
                            if (rows[i].Length > 0)
                            {
                                xml += "<productvariant>";
                                string delim = ",";
                                string[] cols = rows[i].Split(delim.ToCharArray());
                                xml += "<ProductID>" + cols[0] + "</ProductID>";
                                xml += "<VariantID>" + cols[1] + "</VariantID>";
                                xml += "<KitItemID>" + cols[2] + "</KitItemID>";
                                xml += "<Name>" + cols[3] + "</Name>";
                                xml += "<KitGroup>" + cols[4] + "</KitGroup>";
                                xml += "<SKU>" + cols[5] + "</SKU>";
                                xml += "<SKUSuffix>" + cols[7] + "</SKUSuffix>";
                                xml += "<ManufacturerPartNumber>" + cols[6] + "</ManufacturerPartNumber>";
                                xml += "<Cost>" + cols[8] + "</Cost>";
                                xml += "<MSRP>" + cols[9] + "</MSRP>";
                                xml += "<Price>" + cols[10] + "</Price>";
                                xml += "<SalePrice>" + cols[11] + "</SalePrice>";
                                xml += "<Inventory>" + cols[12] + "</Inventory>";
                                xml += "</productvariant>";
                            }
                        }
                        xml += "</productlist>";
                    }
                    else if (PostedFile.FileName.EndsWith("xls", StringComparison.InvariantCultureIgnoreCase))
                    {
                        xml = Import.ConvertPricingFileToXml(FullFilePath);
                        XslCompiledTransform xForm = new XslCompiledTransform();
                        xForm.Load(CommonLogic.SafeMapPath("XmlPackages/ExcelPricingImport.xslt"));
                        Localization ExtObj = new Localization();
                        XsltArgumentList m_TransformArgumentList = new XsltArgumentList();
                        m_TransformArgumentList.AddExtensionObject("urn:aspdnsf", ExtObj);
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.LoadXml(xml);
                        StringWriter xsw = new StringWriter();
                        xForm.Transform(xdoc, m_TransformArgumentList, xsw);
                        xml = xsw.ToString();
                    }
                    else
                    {
                        xml = filecontent;
                    }
                    File.Delete(FullFilePath);
                    errors = AppLogic.ImportProductList(xml);
                }
            }

            if (errors.Length == 0)
            {
                ltResult.Text = ("<b>Prices Imported OK</b>");
            }
            else
            {
                ltResult.Text = ("<b>The following errors where encountered:</b><br/><div style=\"color:red;\">" + errors + "</div>");
            }
        }
    }
}
