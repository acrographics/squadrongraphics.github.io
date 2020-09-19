// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/exportProductPricing.aspx.cs 3     10/04/06 12:00p Redwoodtree $
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
    /// Summary description for exportProductPricing
    /// </summary>
    public partial class exportProductPricing : System.Web.UI.Page
    {
        private Customer cust;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            if (!IsPostBack)
            {
                LoadData();
                divDistributor.Visible = false;
                divGenre.Visible = false;
                divVector.Visible = false;
#if PRO
			// not supported in PRO
#else
                divDistributor.Visible = true;
                divGenre.Visible = true;
                divVector.Visible = true;
#endif
            }
        }

        protected void LoadData()
        {
            //clear
            ddCategory.Items.Clear();
            ddCategory.ClearSelection();
            ddSection.Items.Clear();
            ddSection.ClearSelection();
            ddManufacturer.Items.Clear();
            ddManufacturer.ClearSelection();
            ddDistributor.Items.Clear();
            ddDistributor.ClearSelection();
            ddGenre.Items.Clear();
            ddGenre.ClearSelection();
            ddVector.Items.Clear();
            ddVector.ClearSelection();

            //load Categories
            ddCategory.Items.Add(new ListItem(" - Select -", "-1"));
            EntityHelper eTemp = new EntityHelper(EntityDefinitions.readonly_CategoryEntitySpecs);
            ArrayList al = eTemp.GetEntityArrayList(0, "", 0, cust.LocaleSetting, false);
            for (int i = 0; i < al.Count; i++)
            {
                ListItemClass lic = (ListItemClass)al[i];
                string value = lic.Value.ToString();
                string name = lic.Item;

                ddCategory.Items.Add(new ListItem(name, value));
            }

            //load Sections
            ddSection.Items.Add(new ListItem(" - Select -", "-1"));
            eTemp = new EntityHelper(EntityDefinitions.readonly_SectionEntitySpecs);
            al = eTemp.GetEntityArrayList(0, "", 0, cust.LocaleSetting, false);
            for (int i = 0; i < al.Count; i++)
            {
                ListItemClass lic = (ListItemClass)al[i];
                string value = lic.Value.ToString();
                string name = lic.Item;

                ddSection.Items.Add(new ListItem(name, value));
            }
            
            //load Manufacturers
            ddManufacturer.Items.Add(new ListItem(" - Select -", "-1"));
            IDataReader rsst = DB.GetRS("select * from Manufacturer " + DB.GetNoLock() + " where deleted=0 order by DisplayOrder,Name");
            while (rsst.Read())
            {
                ddManufacturer.Items.Add(new ListItem(DB.RSFieldByLocale(rsst, "Name", cust.LocaleSetting), DB.RSFieldInt(rsst, "ManufacturerID").ToString()));
            }
            rsst.Close();

            //load Distributors
            ddDistributor.Items.Add(new ListItem(" - Select -", "-1"));
            rsst = DB.GetRS("select * from Distributor  " + DB.GetNoLock() + " where Deleted=0 order by DisplayOrder,Name");
            while (rsst.Read())
            {
                ddDistributor.Items.Add(new ListItem(DB.RSFieldByLocale(rsst, "Name", cust.LocaleSetting), DB.RSFieldInt(rsst, "DistributorID").ToString()));
            }
            rsst.Close();

            //load Genres
            ddGenre.Items.Add(new ListItem(" - Select -", "-1"));
            rsst = DB.GetRS("select * from Genre  " + DB.GetNoLock() + " where Deleted=0 order by DisplayOrder,Name");
            while (rsst.Read())
            {
                ddGenre.Items.Add(new ListItem(DB.RSFieldByLocale(rsst, "Name", cust.LocaleSetting), DB.RSFieldInt(rsst, "GenreID").ToString()));
            }
            rsst.Close();

            //load Vectors
            ddVector.Items.Add(new ListItem(" - Select -", "-1"));
            rsst = DB.GetRS("select * from Vector " + DB.GetNoLock() + " where Deleted=0 order by DisplayOrder,Name");
            while (rsst.Read())
            {
                ddVector.Items.Add(new ListItem(DB.RSFieldByLocale(rsst, "Name", cust.LocaleSetting), DB.RSFieldInt(rsst, "VectorID").ToString()));
            }
            rsst.Close();

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
            string filepath = CommonLogic.SafeMapPath("../images") + "\\";
            string filename = "priceexport_" + Localization.ToNativeDateTimeString(System.DateTime.Now).Replace(" ", "").Replace("/", "").Replace(":", "").Replace(".", "");
            string fileextension = String.Empty;

            string xml = AppLogic.ExportProductList(Localization.ParseNativeInt(ddCategory.SelectedValue), Localization.ParseNativeInt(ddSection.SelectedValue), Localization.ParseNativeInt(ddManufacturer.SelectedValue), Localization.ParseNativeInt(ddDistributor.SelectedValue), Localization.ParseNativeInt(ddGenre.SelectedValue), Localization.ParseNativeInt(ddVector.SelectedValue));
            string exporttype = rblExport.SelectedValue;

            //remove old export files
            string[] oldfiles = Directory.GetFiles(filepath, "priceexport_*." + exporttype);
            foreach (string oldfile in oldfiles)
            {
                try
                {
                    File.Delete(oldfile);
                }
                catch { }
            }

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xml);

            string FullFilePath = filepath + filename;
            XslCompiledTransform xsl = new XslCompiledTransform();

            Customer c = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
            XsltArgumentList args = new XsltArgumentList();
            args.AddParam("locale", "", c.LocaleSetting);

            if (exporttype == "xls")
            {
                FullFilePath += ".xls";
                fileextension = ".xls";
                xsl.Load(CommonLogic.SafeMapPath("XmlPackages/ProductPricingExportExcel.xslt"));
                StringWriter xsw = new StringWriter();
                xsl.Transform(xdoc, args, xsw);
                xdoc.LoadXml(xsw.ToString());
                XmlToExcel.ConvertToExcel(xdoc, FullFilePath);
            }
            else
            {
                if (exporttype == "xml")
                {
                    FullFilePath += ".xml";
                    fileextension = ".xml";
                    xsl.Load(CommonLogic.SafeMapPath("XmlPackages/ProductPricingExport.xslt"));
                }
                else
                {
                    FullFilePath += ".csv";
                    fileextension = ".csv";
                    xsl.Load(CommonLogic.SafeMapPath("XmlPackages/ProductPricingExportCSV.xslt"));
                }

                StreamWriter sw = new StreamWriter(FullFilePath);
                xsl.Transform(xdoc, args, sw);
                sw.Close();
            }

            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=ProductPricing" + fileextension);
            Response.BufferOutput = false;
            if (exporttype == "xml")
            {
                //Send the XML
                Response.ContentType = "text/xml";
                Response.Write(XmlCommon.PrettyPrintXml(CommonLogic.ReadFile(FullFilePath, false)));
                Response.Flush();
                Response.End();
            }
            else if (exporttype == "xls")
            {
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "";
                Response.TransmitFile(FullFilePath);
                Response.Flush();
                Response.End();
            }
            else
            {
                // Send the CSV
                Response.BufferOutput = false;
                Response.ContentType = "application/x-unknown";
                Response.TransmitFile(FullFilePath);
                Response.Flush();
                Response.End();
            }
        }
    }
}
