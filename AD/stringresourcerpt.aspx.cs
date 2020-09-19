// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/stringresourcerpt.aspx.cs 3     9/16/06 10:56a Redwoodtree $
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
using System.IO;
using System.Xml;

namespace AspDotNetStorefrontAdmin
{

    public partial class stringresourcerpt : System.Web.UI.Page
    {
        protected Customer cust;
        private string ShowLocaleSetting = Localization.CheckLocaleSettingForProperCase(CommonLogic.QueryStringCanBeDangerousContent("ShowLocaleSetting"));
        String ReportType = "missing";

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            ReportType = CommonLogic.QueryStringCanBeDangerousContent("ReportType").ToLowerInvariant().Trim();
            if (ReportType != "missing" && ReportType != "modified")
            {
                ReportType = "missing";
            }
            Page.Header.Title = "String Resource Report: " + ReportType;
            cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            if (!IsPostBack)
            {
                if (ReportType == "missing")
                {
                    ltLocale.Text = "The following strings are defined in en-US locale, but not in " + ShowLocaleSetting + " locale. <a href=\"stringresource.aspx?ShowLocaleSetting=" + ShowLocaleSetting + "\">Back To String Resource Manager</a>";
                    ReportLabel.Text = "Missing Strings:";
                }
                else
                {
                    ltLocale.Text = "The following strings in the " + ShowLocaleSetting + " locale have been modified from their default values. <a href=\"stringresource.aspx?ShowLocaleSetting=" + ShowLocaleSetting + "\">Back To String Resource Manager</a>";
                    ReportLabel.Text = "Modified Strings:";
                }
                loadData();
            }
        }

        protected void loadData()
        {
            IDataReader rs;
            if (ReportType == "missing")
            {
                btnSubmit.Visible = true;
                rs = DB.GetRS("select A.* from (select * from StringResource where LocaleSetting=" + DB.SQuote("en-US") + ") A left join (select * from StringResource where LocaleSetting=" + DB.SQuote(ShowLocaleSetting) + ") B on A.Name=B.Name WHERE B.LocaleSetting is null order by A.Name");
            }
            else
            {
                btnSubmit.Visible = false;
                rs = DB.GetRS("select A.* from StringResource A where LocaleSetting=" + DB.SQuote(ShowLocaleSetting) + " and Modified=1 order by A.Name");
            }
            ltData.Text = "<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"overallGrid\">";
            ltData.Text += "<tr><td width=\"200\" class=\"headerGrid\">Resource Name</td>";
            if (ReportType == "missing")
            {
                ltData.Text += "<td width=\"400\" class=\"headerGrid\">en-US Value</td>";
            }
            ltData.Text += "<td width=\"*\" class=\"headerGrid\">" + ShowLocaleSetting + " value</td></tr>";
            bool data = false;
            bool alt = false;

            bool hasRecordsFound = false;
            while (rs.Read())
            {
                hasRecordsFound = true;
                ltData.Text += "<tr><td width=\"200\" align=\"left\" valign=\"top\" class=\"" + (alt ? "DataCellGridAlt" : "DataCellGrid") + "\">";
                ltData.Text += DB.RSField(rs, "Name");
                ltData.Text += "</td>";
                if (ReportType == "missing")
                {
                    ltData.Text += "<td align=\"left\" valign=\"top\" width=\"400\" class=\"" + (alt ? "DataCellGridAlt" : "DataCellGrid") + "\">";
                    ltData.Text += DB.RSField(rs, "ConfigValue");
                    ltData.Text += "</td>";
                    ltData.Text += "<td width=\"*\" align=\"left\" valign=\"top\" class=\"" + (alt ? "DataCellGridAlt" : "DataCellGrid") + "\">";
                    ltData.Text += "<input type=\"text\" class=\"singleAuto\" maxlength=\"500\" id=\"" + DB.RSField(rs, "Name") + "\" name=\"" + DB.RSField(rs, "Name") + "\">";
                    ltData.Text += "</td>";
                }
                else
                {
                    ltData.Text += "<td align=\"left\" valign=\"top\" width=\"400\" class=\"" + (alt ? "DataCellGridAlt" : "DataCellGrid") + "\">";
                    ltData.Text += DB.RSField(rs, "ConfigValue");
                    ltData.Text += "</td>";
                }
                ltData.Text += "</tr>";
                alt = !alt;
                data = true;
            }
            rs.Close();

            if (!data)
            {
                if (ReportType == "missing")
                {
                    ltData.Text += ("<tr><td colspan=\"3\" class=\"" + (alt ? "DataCellGridAlt" : "DataCellGrid") + "\">No strings missing</td></tr>");
                }
                else
                {
                    ltData.Text += ("<tr><td colspan=\"2\" class=\"" + (alt ? "DataCellGridAlt" : "DataCellGrid") + "\">No strings have been modified</td></tr>");
                }
                btnSubmit.Visible = false;
            }
            ltData.Text += ("</table>");


            rs.Dispose();

            btnExportExcel.Visible = hasRecordsFound;
        }

        protected void resetError(string error, bool isError)
        {
            string str = "<font class=\"noticeMsg\">NOTICE:</font>&nbsp;&nbsp;&nbsp;";
            if (isError)
                str = "<font class=\"errorMsg\">ERROR:</font>&nbsp;&nbsp;&nbsp;";

            if (error.Length > 0)
            {
                str += error;
            }
            else
            {
                str = String.Empty;
            }

            ltError.Text = str;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < HttpContext.Current.Request.Form.Count; i++)
            {
                bool okField = false;
                if (HttpContext.Current.Request.Form.Keys[i].ToUpperInvariant().IndexOf(".") != -1)
                {
                    okField = true;
                }
                if (okField)
                {
                    String FldVal = HttpContext.Current.Request.Form[HttpContext.Current.Request.Form.Keys[i]];
                    FldVal = FldVal.Trim();
                    if (FldVal.Length != 0)
                    {
                        String sql = String.Format("insert StringResource(Name,LocaleSetting,ConfigValue) values({0},{1},{2})", DB.SQuote(HttpContext.Current.Request.Form.Keys[i]), DB.SQuote(ShowLocaleSetting), DB.SQuote(FldVal));
                        DB.ExecuteSQL(sql);
                    }
                }
            }

            resetError("Strings updated.", false);
            loadData();
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            bool isReportTypeMissingStringResource = ReportType.ToLowerInvariant().Equals("missing");
            string query = string.Empty;

            if (isReportTypeMissingStringResource)
            {
                query = "SELECT A.[Name], A.ConfigValue FROM (SELECT * FROM StringResource WHERE LocaleSetting=" + DB.SQuote("en-US") + ") A LEFT JOIN (SELECT * from StringResource where LocaleSetting=" + DB.SQuote(ShowLocaleSetting) + ") B on A.Name=B.Name WHERE B.LocaleSetting IS NULL ORDER BY A.NAME";
            }
            else
            {
                query = "SELECT A.[Name], A.ConfigValue FROM StringResource A WHERE LocaleSetting=" + DB.SQuote(ShowLocaleSetting) + " AND Modified=1 ORDER BY A.Name";
            }
            // reload the data.
            XmlDocument doc = new XmlDocument();
            XmlNode root = doc.CreateNode(XmlNodeType.Element, "root", string.Empty);
            doc.AppendChild(root);
            using (IDataReader reader = DB.GetRS(query))
            {
                while (reader.Read())
                {
                    XmlNode stringResourceNode = doc.CreateNode(XmlNodeType.Element, "StringResource", string.Empty);
                    XmlNode nameNode = doc.CreateNode(XmlNodeType.Element, "Name", string.Empty);
                    XmlNode valueNode = doc.CreateNode(XmlNodeType.Element, "Value", string.Empty);

                    nameNode.InnerText = DB.RSField(reader, "Name");
                    //valueNode.InnerText = CommonLogic.IIF(isReportTypeMissingStringResource, string.Empty, DB.RSField(reader, "ConfigValue"));
                    valueNode.InnerText = DB.RSField(reader, "ConfigValue");

                    stringResourceNode.AppendChild(nameNode);
                    stringResourceNode.AppendChild(valueNode);

                    root.AppendChild(stringResourceNode);
                }
            }

            string filePath = CommonLogic.SafeMapPath("../images") + "\\";
            string fileName = "StringResource_" + Localization.ToNativeDateTimeString(System.DateTime.Now).Replace(" ", "").Replace("/", "").Replace(":", "").Replace(".", "") + ".xls";

            //remove old export files
            string[] oldfiles = Directory.GetFiles(filePath, "StringResource_*.xls");
            foreach (string oldfile in oldfiles)
            {
                try
                {
                    File.Delete(oldfile);
                }
                catch { }
            }
            
            string fileNameWithFullPath = filePath+fileName;
            AspDotNetStorefrontExcelWrapper.XmlToExcel.ConvertToExcel(doc, fileNameWithFullPath);

            string outputFileName = CommonLogic.IIF(isReportTypeMissingStringResource, "Missing", "Modified");
            
            Response.Clear();
            Response.AddHeader("content-disposition", string.Format("attachment; filename={0}StringResource.{1}.xls", outputFileName, ShowLocaleSetting));
            Response.BufferOutput = false;            
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.TransmitFile(fileNameWithFullPath);
            Response.Flush();
            Response.End();
        }
}
}