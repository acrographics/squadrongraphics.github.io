// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/importstringresourcefile2.aspx.cs 5     9/30/06 1:08p Redwoodtree $
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
using AspDotNetStorefrontExcelWrapper;
using System.Xml;

namespace AspDotNetStorefrontAdmin
{

    public partial class importstringresourcefile2 : System.Web.UI.Page
    {
        protected Customer cust;
        private const int NAME_COLUMN = 1;
        private const int VALUE_COLUMN = 2;
        private String ShowLocaleSetting;
        private String SpreadsheetName;
        private String ExcelFile;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
            Server.ScriptTimeout = 1000000;

            cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            ShowLocaleSetting = Localization.CheckLocaleSettingForProperCase(CommonLogic.QueryStringCanBeDangerousContent("ShowLocaleSetting"));
            
            // this should tell that the excel file to be loaded
            // is a master string resource for the locale i.e. strings.fr-FR.xls
            bool isMasterStringResource = CommonLogic.QueryStringBool("master");
            if (isMasterStringResource)
            {
                bool exists = AppLogic.CheckStringResourceExcelFileExists(ShowLocaleSetting, out ExcelFile);
                if (!exists)
                {
                    resetError("String Resource file missing!", true);
                    return;
                }
            }
            else
            {
                SpreadsheetName = CommonLogic.QueryStringCanBeDangerousContent("SpreadsheetName");
                ExcelFile = CommonLogic.SafeMapPath("../images" + "/" + SpreadsheetName + ".xls");
            }

            if (!IsPostBack)
            {
                if (isMasterStringResource)
                {
                    litStage.Text = string.Format("Reload {0} Master String Resource File", CommonLogic.QueryStringCanBeDangerousContent("ShowLocaleSetting"));
                    btnProcessFile.Text = "Begin Reload Processing";
                    ltStrings.Text = "<a href=\"stringresource.aspx?showlocalesetting=" + ShowLocaleSetting + "\">Cancel Reload</a>";
                }
                else
                {
                    litStage.Text = string.Format("Import {0} String Resource File - Verification (Step 2)", CommonLogic.QueryStringCanBeDangerousContent("ShowLocaleSetting"));
                    btnProcessFile.Text = "Begin Import Processing";
                    ltStrings.Text = "<a href=\"stringresource.aspx?showlocalesetting=" + ShowLocaleSetting + "\">Cancel Import</a>";
                }
                ltProcessing.Text = "<b>Processing File: " + ExcelFile + "</b>";
                ltVerify.Text = "<b>If the preview data below looks good:</b>";
                loadData();
            }
        }

        protected void loadData()
        {
            StringBuilder tmpS = new StringBuilder(4096);
            tmpS.Append("<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"1\">\n");
            tmpS.Append("<tr>\n");
            tmpS.Append("<td class=\"headerGrid\">Row</td>\n");
            tmpS.Append("<td class=\"headerGrid\">Status</td>\n");
            tmpS.Append("<td class=\"headerGrid\">Name</td>\n");
            tmpS.Append("<td class=\"headerGrid\">LocaleSetting</td>\n");
            tmpS.Append("<td class=\"headerGrid\">String Value</td>\n");
            tmpS.Append("</tr>\n");

            String NameField = String.Empty;
            String ValueField = String.Empty;

			if (AppLogic.AppConfigBool("ExcelOldStyleLoad"))
			{
				DataSet ds = Excel.GetDS(ExcelFile, "Sheet1");
            
				for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
				{
					try
					{
						// this col may not exist:
						NameField = "" + ds.Tables[0].Rows[i][NAME_COLUMN - 1].ToString();
					}
					catch { }
					try
					{
						// this col may not exist:
						ValueField = "" + ds.Tables[0].Rows[i][VALUE_COLUMN - 1].ToString();
					}
					catch { }

					tmpS.Append("<tr>\n");
					tmpS.Append("<td class=\"DataCellGrid\">" + (i + 1) + "</td>\n");

                    string status = ProcessLine(NameField, ShowLocaleSetting, ValueField, true);
                    string statusHTML = CommonLogic.IIF(status.StartsWith(AppLogic.ro_OK), status, string.Format("<font color=\"#ff0000\"><b>{0}</b></font>", status));
                    tmpS.Append("<td class=\"DataCellGrid\">" + (statusHTML.Length == 0 ? "&nbsp;" : statusHTML) + "</td>\n");

					tmpS.Append("<td class=\"DataCellGrid\">" + (NameField.Length == 0 ? "&nbsp;" : NameField) + "</td>\n");
					tmpS.Append("<td class=\"DataCellGrid\">" + (ShowLocaleSetting.Length == 0 ? "&nbsp;" : ShowLocaleSetting) + "</td>\n");
					tmpS.Append("<td class=\"DataCellGrid\">" + (ValueField.Length == 0 ? "&nbsp;" : ValueField) + "</td>\n");
					tmpS.Append("</tr>\n");
				}
				tmpS.Append("</table>\n");
				ds.Dispose();
			}
			else
			{
				ExcelToXml exf = new ExcelToXml(ExcelFile);
				XmlDocument xmlDoc = exf.LoadSheet("Sheet1", "B", 5000, "A");
				foreach (XmlNode row in xmlDoc.SelectNodes("/excel/sheet/row"))
				{
					NameField = exf.GetCell(row, "A");
					ValueField = exf.GetCell(row, "B");
						
					tmpS.Append("<td class=\"DataCellGrid\">" + XmlCommon.XmlAttributeUSInt(row,"id").ToString() + "</td>\n");

                    string status = ProcessLine(NameField, ShowLocaleSetting, ValueField, true);
                    string statusHTML = CommonLogic.IIF(status.StartsWith(AppLogic.ro_OK), status, string.Format("<font color=\"#ff0000\"><b>{0}</b></font>", status));
                    tmpS.Append("<td class=\"DataCellGrid\">" + (statusHTML.Length == 0 ? "&nbsp;" : statusHTML) + "</td>\n");

					tmpS.Append("<td class=\"DataCellGrid\">" + (NameField.Length == 0 ? "&nbsp;" : NameField) + "</td>\n");
					tmpS.Append("<td class=\"DataCellGrid\">" + (ShowLocaleSetting.Length == 0 ? "&nbsp;" : ShowLocaleSetting) + "</td>\n");
					tmpS.Append("<td class=\"DataCellGrid\">" + (ValueField.Length == 0 ? "&nbsp;" : ValueField) + "</td>\n");
                    tmpS.Append("</tr>\n");
				}
				tmpS.Append("</table>\n");
				tmpS.Append("<a href=\"stringresource.aspx?showlocalesetting=" + ShowLocaleSetting + "\">Back to String Resources</a>");
			}
            ltData.Text = tmpS.ToString();
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

            ((Literal)Form.FindControl("ltError")).Text = str;
        }
        

        private String ProcessLine(String Name, String LocaleSetting, String ConfigValue, bool preview)
        {
            String result = AppLogic.ro_OK;
            if (Name.Length != 0)
            {
                try
                {
                    ImportOption option = (ImportOption)CommonLogic.QueryStringUSInt("option");
                    bool existing = false;
                    bool modified = false;
                    string resourceId = string.Empty;
                    using (IDataReader reader = DB.GetRS(string.Format("SELECT Name, Modified, StringResourceGuid FROM StringResource WHERE Name = {0} AND LocaleSetting = {1}", DB.SQuote(Name), DB.SQuote(LocaleSetting))))
                    {
                        existing = reader.Read();
                        if (existing)
                        {
                            modified = (DB.RSFieldTinyInt(reader, "Modified") > 0);
                            resourceId = DB.RSFieldGUID(reader, "StringResourceGuid");
                        }
                    }

                    if (existing)
                    {
                        if (modified && ((option & ImportOption.LeaveModified) == ImportOption.LeaveModified))
                        {
                            // do nothing
                            //result = AppLogic.ro_OK + "- Left Modified";
                            result = "Not Imported(was already modified)";
                        }
                        else if ((option & ImportOption.OverWrite) == ImportOption.OverWrite)
                        {
                            if (!preview)
                            {
                                DB.ExecuteSQL(string.Format("DELETE StringResource WHERE StringResourceGuid = {0}", DB.SQuote(resourceId)));
                                InsertStringResource(Name, LocaleSetting, ConfigValue);
                            }

                            result = AppLogic.ro_OK; // +"- Overwritten";
                        }
                        else
                        {
                            result = "Not Imported(duplicate existing)";
                        }
                    }
                    else
                    {
                        if (!preview)
                        {
                            InsertStringResource(Name, LocaleSetting, ConfigValue);
                        }
                        result = AppLogic.ro_OK; // +"- Added";
                    }
                }
                catch (Exception ex)
                {
                    result = CommonLogic.GetExceptionDetail(ex, "<br/>");
                }
            }

            return result;
        }

        private void InsertStringResource(string name, string locale, string configValue)
        {
            String NewGUID = DB.GetNewGUID();
            StringBuilder sql = new StringBuilder(1000);
            sql.Append("insert into StringResource(StringResourceGUID,Name,LocaleSetting,ConfigValue) values(");
            sql.Append(DB.SQuote(NewGUID) + ",");
            sql.Append(DB.SQuote(name) + ",");
            sql.Append(DB.SQuote(locale) + ",");
            sql.Append(DB.SQuote(configValue));
            sql.Append(")");
            DB.ExecuteSQL(sql.ToString());
        }


        protected void btnProcessFile_Click(object sender, EventArgs e)
        {
            resetError("", false);
            ltVerify.Visible = false;
            btnProcessFile.Visible = false;
            ltStrings.Text = String.Empty;
            StringBuilder tmpS = new StringBuilder(4096);
            try
            {
                //DB.ExecuteSQL("delete from StringResource where LocaleSetting=" + DB.SQuote(ShowLocaleSetting));

                tmpS.Append("<p align=\"left\">Importing the new strings (see status below)...</p>");
                tmpS.Append("<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"1\">\n");
                tmpS.Append("<tr>\n");
                tmpS.Append("<td class=\"headerGrid\">Status</td>\n");
                tmpS.Append("<td class=\"headerGrid\">Row</td>\n");
                tmpS.Append("<td class=\"headerGrid\">Name</td>\n");
                tmpS.Append("<td class=\"headerGrid\">LocaleSetting</td>\n");
                tmpS.Append("<td class=\"headerGrid\">String Value</td>\n");
                tmpS.Append("</tr>\n");

                String NameField = String.Empty;
                String ValueField = String.Empty;

                if (AppLogic.AppConfigBool("ExcelOldStyleLoad"))
                {
                    DataSet ds = Excel.GetDS(ExcelFile, "Sheet1");
                    for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            // this col may not exist:
                            NameField = "" + ds.Tables[0].Rows[i][NAME_COLUMN - 1].ToString();
                        }
                        catch { }
                        try
                        {
                            // this col may not exist:
                            ValueField = "" + ds.Tables[0].Rows[i][VALUE_COLUMN - 1].ToString();
                        }
                        catch { }

                        String ProcessIt = ProcessLine(NameField, ShowLocaleSetting, ValueField, false);
                        tmpS.Append("<tr bgcolor=\"" + (ProcessIt.StartsWith(AppLogic.ro_OK) ? "#DEFEDD" : "#FFCCCC") + "\">\n");
                        tmpS.Append("<td>" + ProcessIt + "</td>\n");
                        tmpS.Append("<td class=\"DataCellGrid\">" + (i + 1) + "</td>\n");
                        tmpS.Append("<td class=\"DataCellGrid\">" + (NameField.Length == 0 ? "&nbsp;" : NameField) + "</td>\n");
                        tmpS.Append("<td class=\"DataCellGrid\">" + (ShowLocaleSetting.Length == 0 ? "&nbsp;" : ShowLocaleSetting) + "</td>\n");
                        tmpS.Append("<td class=\"DataCellGrid\">" + (ValueField.Length == 0 ? "&nbsp;" : ValueField) + "</td>\n");
                        tmpS.Append("</tr>\n");
                    }
                    tmpS.Append("</table>\n");
                    tmpS.Append("<a href=\"stringresource.aspx?showlocalesetting=" + ShowLocaleSetting + "\">Back to String Resources</a>");
                    ds.Dispose();
                }
                else
                {
                    ExcelToXml exf = new ExcelToXml(ExcelFile);
                    XmlDocument xmlDoc = exf.LoadSheet("Sheet1", "B", 5000, "A");
                    foreach (XmlNode row in xmlDoc.SelectNodes("/excel/sheet/row"))
                    {
                        NameField = exf.GetCell(row, "A");
                        ValueField = exf.GetCell(row, "B");

                        String ProcessIt = ProcessLine(NameField, ShowLocaleSetting, ValueField, false);
                        tmpS.Append("<tr bgcolor=\"" + (ProcessIt.StartsWith(AppLogic.ro_OK) ? "#DEFEDD" : "#FFCCCC") + "\">\n");
                        tmpS.Append("<td>" + ProcessIt + "</td>\n");
                        tmpS.Append("<td class=\"DataCellGrid\">" + XmlCommon.XmlAttributeUSInt(row, "id").ToString() + "</td>\n");
                        tmpS.Append("<td class=\"DataCellGrid\">" + (NameField.Length == 0 ? "&nbsp;" : NameField) + "</td>\n");
                        tmpS.Append("<td class=\"DataCellGrid\">" + (ShowLocaleSetting.Length == 0 ? "&nbsp;" : ShowLocaleSetting) + "</td>\n");
                        tmpS.Append("<td class=\"DataCellGrid\">" + (ValueField.Length == 0 ? "&nbsp;" : ValueField) + "</td>\n");
                        tmpS.Append("</tr>\n");
                    }
                    tmpS.Append("</table>\n");
                    tmpS.Append("<b>Done</b><br/><br/>");
                    tmpS.Append("<a href=\"stringresource.aspx?showlocalesetting=" + ShowLocaleSetting + "\">Back to String Resources</a>");
                }
                AppLogic.StringResourceTable = new StringResources();
            }
            catch (Exception ex)
            {
                resetError("Error Processing Strings: " + ex.ToString(), true);
            }
            ltData.Text = tmpS.ToString();
        }
    }
}