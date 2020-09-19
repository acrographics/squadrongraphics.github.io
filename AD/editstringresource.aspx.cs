// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editstringresource.aspx.cs 7     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.IO;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary LocaleSetting for editstringresource
    /// </summary>
    public partial class editstringresource : AspDotNetStorefront.SkinBase
    {

        String SearchFor;
        String ShowLocaleSetting;
        String BeginsWith;
        int StringResourceID;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            SearchFor = CommonLogic.QueryStringCanBeDangerousContent("SearchFor");
            ShowLocaleSetting = Localization.CheckLocaleSettingForProperCase(CommonLogic.QueryStringCanBeDangerousContent("ShowLocaleSetting"));
            BeginsWith = CommonLogic.QueryStringCanBeDangerousContent("BeginsWith");
            StringResourceID = 0;

            if (CommonLogic.QueryStringCanBeDangerousContent("StringResourceID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("StringResourceID") != "0")
            {
                Editing = true;
                StringResourceID = Localization.ParseUSInt(CommonLogic.QueryStringCanBeDangerousContent("StringResourceID"));
            }
            else
            {
                Editing = false;
            }

            IDataReader rs;

            if (CommonLogic.FormBool("IsSubmit"))
            {
                StringBuilder sql = new StringBuilder(2500);
                if (!Editing)
                {
                    // ok to add them:
                    String NewGUID = DB.GetNewGUID();
                    sql.Append("insert into StringResource(StringResourceGUID,Name,LocaleSetting,ConfigValue) values(");
                    sql.Append(DB.SQuote(NewGUID) + ",");
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("Name")) + ",");
                    sql.Append(DB.SQuote(Localization.CheckLocaleSettingForProperCase(CommonLogic.FormCanBeDangerousContent("LocaleSetting"))) + ",");
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("ConfigValue")));
                    sql.Append(")");
                    DB.ExecuteSQL(sql.ToString());

                    rs = DB.GetRS("select StringResourceID from StringResource  " + DB.GetNoLock() + " where StringResourceGUID=" + DB.SQuote(NewGUID));
                    rs.Read();
                    StringResourceID = DB.RSFieldInt(rs, "StringResourceID");
                    Editing = true;
                    rs.Close();
                    DataUpdated = true;
                }
                else
                {
                    // ok to update:
                    sql.Append("update StringResource set ");
                    sql.Append("Name=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Name")) + ",");
                    sql.Append("LocaleSetting=" + DB.SQuote(Localization.CheckLocaleSettingForProperCase(CommonLogic.FormCanBeDangerousContent("LocaleSetting"))) + ",");
                    sql.Append("ConfigValue=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ConfigValue")));
                    sql.Append(" where StringResourceID=" + StringResourceID.ToString());
                    DB.ExecuteSQL(sql.ToString());
                    DataUpdated = true;
                    Editing = true;
                }
                AppLogic.ClearCache();
            }
            SectionTitle = "<a href=\"StringResource.aspx?ShowLocaleSetting=" + Server.UrlEncode(ShowLocaleSetting) + "&beginsWith=" + Server.UrlEncode(BeginsWith) + "&searchfor=" + Server.UrlEncode(SearchFor) + "\">StringResource</a> - Manage StringResource Parameters";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            IDataReader rs = DB.GetRS("select * from StringResource  " + DB.GetNoLock() + " where StringResourceID=" + StringResourceID.ToString());
            if (rs.Read())
            {
                Editing = true;
            }

            if (ErrorMsg.Length != 0)
            {
                writer.Write("<p align=\"left\"><b><font color=red>" + ErrorMsg + "</font></b></p>\n");
            }
            if (DataUpdated)
            {
                writer.Write("<p align=\"left\"><b><font color=blue>(UPDATED)</font></b></p>\n");
            }


            if (ErrorMsg.Length == 0)
            {

                if (Editing)
                {
                    writer.Write("<p align=\"left\"><b>Editing String Resource: " + DB.RSField(rs, "Name") + " (ID=" + DB.RSFieldInt(rs, "StringResourceID").ToString() + ")</b></p>\n");
                }
                else
                {
                    writer.Write("<p align=\"left\"><b>Adding New String Resource:</b></p>\n");
                }

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function Form_Validator(theForm)\n");
                writer.Write("{\n");
                writer.Write("submitonce(theForm);\n");
                writer.Write("return (true);\n");
                writer.Write("}\n");
                writer.Write("</script>\n");

                writer.Write("<p>Please enter the following information about this String Resource. Fields marked with an asterisk (*) are required. All other fields are optional.</p>\n");
                writer.Write("<form enctype=\"multipart/form-data\" action=\"editStringResource.aspx?ShowLocaleSetting=" + Server.UrlEncode(ShowLocaleSetting) + "&beginsWith=" + Server.UrlEncode(BeginsWith) + "&searchfor=" + Server.UrlEncode(SearchFor) + "&StringResourceID=" + StringResourceID.ToString() + "&edit=" + Editing.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(this) && Form_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"100%\" colspan=\"2\" align=\"left\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Name:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"100\" size=\"50\" name=\"Name\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "Name")), "") + "\">\n");
                writer.Write("                	<input type=\"hidden\" name=\"Name_vldt\" value=\"[req][blankalert=Please enter the StringResource parameter name]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">LocaleSetting:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");

                StringBuilder tmpS = new StringBuilder(4096);
                DataSet ds = DB.GetDS("select * from LocaleSetting  " + DB.GetNoLock() + " order by displayorder,description", AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    tmpS.Append("<!-- COUNTRY SELECT LIST -->\n");
                    tmpS.Append("<select size=\"1\" id=\"LocaleSetting\" name=\"LocaleSetting\">");
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        tmpS.Append("<option value=\"" + DB.RowField(row, "Name") + "\" " + CommonLogic.IIF(DB.RSField(rs, "LocaleSetting") == DB.RowField(row, "Name"), " selected ", "") + ">" + DB.RowField(row, "Name") + "</option>");
                    }
                    tmpS.Append("</select>");
                    tmpS.Append("<!-- END COUNTRY SELECT LIST -->\n");
                }
                ds.Dispose();
                writer.Write(tmpS);

                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Value:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"2500\" size=\"150\" name=\"ConfigValue\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "ConfigValue")), "") + "\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("<tr>\n");
                writer.Write("<td></td><td align=\"left\" valign=\"top\"><br/>\n");
                if (Editing)
                {
                    writer.Write("<input type=\"submit\" value=\"Update\" name=\"submit\">\n");
                    writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"reset\" value=\"Reset\" name=\"reset\">\n");
                }
                else
                {
                    writer.Write("<input type=\"submit\" value=\"Add New\" name=\"submit\">\n");
                }
                writer.Write("        </td>\n");
                writer.Write("      </tr>\n");
                writer.Write("  </table>\n");
                writer.Write("</form>\n");
            }
            rs.Close();
        }

    }
}
