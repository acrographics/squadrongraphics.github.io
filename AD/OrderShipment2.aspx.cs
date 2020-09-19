using System;
using System.IO;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Xsl;
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
    public partial class OrderShipment2 : System.Web.UI.Page
    {

        private string methodtype = string.Empty;
        private string state = string.Empty;
        private string errors = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {

            methodtype = CommonLogic.FormCanBeDangerousContent("exporttype");
            state = CommonLogic.FormCanBeDangerousContent("state");

            if (state.IndexOf("Export") != -1)
            {
                if (methodtype == "UPS WorldShip")
                {
                    ExportShipment();
                }
                else
                {
                    MsgBox("This feature is not implemented");
                    Response.Redirect("exportshipment1.aspx");
                }
            }
            else
            {
                ImportShipment();
                Response.Redirect("viewshipment.aspx?statename=import");
                
            }

        }

        private void ImportShipment()
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFile PostedFile = Request.Files[0];
                if (!PostedFile.FileName.EndsWith("csv", StringComparison.InvariantCultureIgnoreCase) && !PostedFile.FileName.EndsWith("xml", StringComparison.InvariantCultureIgnoreCase) && PostedFile.FileName.Trim() != "")
                {
                    errors = "<b>Invalid file type submitted. " + CommonLogic.IIF(PostedFile.ContentLength == 0, "The file contents were empty.", "Please submit only XML, XLS (Excel), or CSV (comma delimited) files.") + "</b>";
                }
                else
                {
                    string filename = "Import_ASPDNSF";
                    string FullFilePath = CommonLogic.SafeMapPath("../images") + "\\" + filename + PostedFile.FileName.ToLowerInvariant().Substring(PostedFile.FileName.ToLowerInvariant().LastIndexOf('.'));
                    string xml = String.Empty;

                    PostedFile.SaveAs(FullFilePath);
                    StreamReader sr = new StreamReader(FullFilePath);
                    string filecontent = sr.ReadToEnd();
                    sr.Close();

                    if (PostedFile.FileName.EndsWith("csv", StringComparison.InvariantCultureIgnoreCase))
                    {
                        xml = ImportXmlData(filecontent);
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
                    errors = ImportProductList(xml);

                }
            }
        }

        private void ExportShipment()
        {
            string importMethod = "export";
            string filepath = CommonLogic.SafeMapPath("../images") + "\\";
            string filename = importMethod + Localization.ToNativeShortDateString(System.DateTime.Now).Replace(" ", "").Replace("/", "").Replace(":", "").Replace(".", "");
            string fileextension = String.Empty;
            string xml = ExportXML();
            string extname = "csv";

            string[] oldfiles = Directory.GetFiles(filepath, "export*." + extname);
            
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

            //filepath += ".csv";
            fileextension = ".csv";
            xsl.Load(CommonLogic.SafeMapPath("XmlPackages/ShipmentOrderExport.xslt"));

            StreamWriter sw = new StreamWriter(filepath + filename + ".csv");
            xsl.Transform(xdoc, args, sw);
            sw.Close();

            Response.AddHeader("content-disposition", "attachment; filename=Export_ADNSF" + fileextension);
            Response.BufferOutput = false;

            Response.ContentType = "application/x-unknown";
            Response.TransmitFile(FullFilePath + ".csv");
        }

        private String ExportXML()
        {

            SqlConnection dbconn = new SqlConnection();
            dbconn.ConnectionString = DB.GetDBConn();
            dbconn.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbconn;
            cmd.CommandType = CommandType.Text;
            string sql = "exec dbo.aspdnsf_WorldShipExport";
            cmd.CommandText = sql;

            SqlDataReader dr = cmd.ExecuteReader();
            StringBuilder sb = new StringBuilder(1024);
            int n = DB.GetENLocaleXml(dr, "root", "order", ref sb);
            dr.Close();
            return sb.ToString();

        }

        private void MsgBox(String Message)
        {
            System.Web.HttpContext.Current.Response.Write("<SCRIPT LANGUAGE = \"JavaScript\">" + Environment.NewLine);
            System.Web.HttpContext.Current.Response.Write("alert(\"" + Message + "\")" + Environment.NewLine);
            System.Web.HttpContext.Current.Response.Write("</SCRIPT>");
        }

        private string ImportProductList(string importtext)
        {
            if (importtext.Trim() == "")
            {
                return "No data to import";
            }

            SqlConnection dbconn = new SqlConnection();
            dbconn.ConnectionString = DB.GetDBConn();
            dbconn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbconn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "aspdnsf_ImportOrderShipment_XML";
     
            cmd.Parameters.Add(new SqlParameter("@xmlorder", SqlDbType.NText));
            cmd.Parameters["@xmlorder"].Value = importtext;
            cmd.Parameters.Add(new SqlParameter("@carrierName", SqlDbType.NVarChar));
            cmd.Parameters["@carrierName"].Value = "UPS";

            try
            {
                cmd.ExecuteNonQuery();
                dbconn.Close();
                if (AppLogic.AppConfigBool("BulkImportSendsShipmentNotifications"))
                    SendShippedEMails(importtext);
                return "";
            }
            catch (Exception ex)
            {
                dbconn.Close();
                throw ex;
            }

        }

        private string ImportXmlData(string fileContent)
        {
            string[] rows = fileContent.Split(Environment.NewLine.ToCharArray());
            StringBuilder sb = new StringBuilder();

            string xml = string.Empty;
            string delim = ",";
            string[] cols;

            ArrayList list = new ArrayList();
            list.Clear();

            DataSet ds = new DataSet("WorldShip");
            DataRow dr;
            DataTable dt = new DataTable("ordershipment");
            ds.Tables.Add(dt);
            //dt.Columns.Add("Voided", typeof(string));
            dt.Columns.Add("ReadyToShip", typeof(Int16));
            dt.Columns.Add("OrderNumber", typeof(Int32));
            dt.Columns.Add("TrackingNumber", typeof(string));
            dt.Columns.Add("Notes", typeof(string));

            for (int i = 0; i < rows.Length; i++)
            {
                cols = rows[i].Split(delim.ToCharArray());
                if (cols[0] != string.Empty)
                {
                    string[] orderNo = cols[1].Split('-');
                    dr = dt.NewRow();
                    if (cols[0].Replace("'", "") != "Y")
                    {
                        /*//dr["Voided"] = cols[0];
                        dr["Voided"] = String.Format("{0:yyyy}0{1:MM}{2:dd}", DateTime.Today.Year.ToString(), DateTime.Today.Month.ToString(), DateTime.Today.Day.ToString());
                        //dr["Voided"] = DateTime.Today.ToShortDateString();
                        dr["ReadyToShip"] = 0;
                        dr["OrderNumber"] = Int32.Parse(orderNo[0]);
                        dr["TrackingNumber"] = DBNull.Value;
                        list.Add(orderNo[0]);*/
                        //dr["Voided"] = cols[0];
                        dr["ReadyToShip"] = 1;
                        dr["OrderNumber"] = Int32.Parse(orderNo[0]);
                        dr["TrackingNumber"] = cols[2];
                        //dr["TrackingNumber"] = String.Format("{0:yyyy}0{1:MM}{2:dd}", DateTime.Today.Year.ToString(), DateTime.Today.Month.ToString(), DateTime.Today.Day.ToString());
                        list.Add(orderNo[0]);

                        
                    }
                    /*else
                    {
                        dr["Voided"] = cols[0];
                        dr["ReadyToShip"] = 1;
                        dr["OrderNumber"] = Int32.Parse(orderNo[0]);
                        dr["TrackingNumber"] = cols[2];
                        //dr["TrackingNumber"] = String.Format("{0:yyyy}0{1:MM}{2:dd}", DateTime.Today.Year.ToString(), DateTime.Today.Month.ToString(), DateTime.Today.Day.ToString());
                        list.Add(orderNo[0]);
                    }*/
                    dt.Rows.Add(dr);
                }
            }

            DataRow[] filterRow;
            string val = string.Empty;

            DataSet shipmentDS = new DataSet("shipment");
            DataTable shipmentDT = new DataTable("ordershipment");
            DataRow shipmentDR;
            shipmentDS.Tables.Add(shipmentDT);
            //shipmentDT.Columns.Add("Voided", typeof(string));
            shipmentDT.Columns.Add("ReadyToShip", typeof(Int16));
            shipmentDT.Columns.Add("OrderNumber", typeof(Int32));
            shipmentDT.Columns.Add("TrackingNumber", typeof(string));
            shipmentDT.Columns.Add("Notes", typeof(string));

            foreach (string item in list)
            {
                filterRow = ds.Tables[0].Select("OrderNumber = " + item);
                if (filterRow.Length > 1)
                {
                    if (shipmentDS.Tables[0].Select("OrderNumber = " + item).Length == 0)
                    {
                        shipmentDR = shipmentDT.NewRow();
                        //shipmentDR["Voided"] = filterRow[0]["Voided"];
                        shipmentDR["ReadyToShip"] = filterRow[0]["ReadyToShip"];
                        shipmentDR["OrderNumber"] = filterRow[0]["OrderNumber"];
                        shipmentDR["TrackingNumber"] = filterRow[0]["TrackingNumber"];
                        shipmentDR["Notes"] = Notes(filterRow);
                        shipmentDT.Rows.Add(shipmentDR);
                    }
                }
                else
                {
                    shipmentDR = shipmentDT.NewRow();
                    //shipmentDR["Voided"] = filterRow[0]["Voided"];
                    shipmentDR["ReadyToShip"] = filterRow[0]["ReadyToShip"];
                    shipmentDR["OrderNumber"] = filterRow[0]["OrderNumber"];
                    shipmentDR["TrackingNumber"] = filterRow[0]["TrackingNumber"];
                    shipmentDR["Notes"] = "Tracking Number: " + filterRow[0]["TrackingNumber"];
                    shipmentDT.Rows.Add(shipmentDR);
                }
            }

            return shipmentDS.GetXml();            
        }

        private string Notes(DataRow[] notesrow)
        {
            string orderNotes = string.Empty;
            for (int i = 0; i < notesrow.Length; i++)
            {
                orderNotes += "Tracking Number: " + notesrow[i]["TrackingNumber"].ToString() + ",";
            }
            return orderNotes.TrimEnd(',');
        }

        private void SendShippedEMails(String importtext)
        {
            //sending email
            StringReader sr = new StringReader(importtext);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(sr);
            XmlNodeList xmlnodelist = xmldoc.GetElementsByTagName("ordershipment");
            
            bool OKToSend = false;

            String MailServer = AppLogic.AppConfig("MailMe_Server");

            if (AppLogic.AppConfigBool("SendShippedEMailToCustomer") && MailServer.Length != 0 && MailServer != AppLogic.ro_TBD)
            {
                OKToSend = true;
            }

            if (OKToSend)
            {
                for (int i = 0; i < xmlnodelist.Count; i++)
                {
                    Order order = new Order(Int32.Parse(xmlnodelist[i].ChildNodes[1].InnerText), null);
                    try
                    {
                        // try to send "shipped on" EMail
                        String SubjectShipped = String.Format(AppLogic.GetString("common.cs.9", order.SkinID, order.LocaleSetting), AppLogic.AppConfig("StoreName"));

                        String BodyShipped = order.ShippedNotification();
                        if (MailServer.Length != 0 && MailServer.ToUpperInvariant() != AppLogic.ro_TBD)
                        {
                            AppLogic.SendMail(SubjectShipped, BodyShipped + AppLogic.AppConfig("MailFooter"), true, AppLogic.AppConfig("ReceiptEMailFrom"), AppLogic.AppConfig("ReceiptEMailFromName"), order.EMail, order.EMail, String.Empty, MailServer);
                        }
                    }
                    catch { }
                    //Debug.WriteLine("xmlnodelist[i].ChildNodes[1].InnerText = " + xmlnodelist[i].ChildNodes[1].InnerText);
                }
            
                
            }
            //end sending email
        }
    }
}