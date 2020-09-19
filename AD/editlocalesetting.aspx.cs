// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editlocalesetting.aspx.cs 6     9/30/06 3:39p Redwoodtree $
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
    /// Summary description for editLocaleSetting
    /// </summary>
    public partial class editLocaleSetting : AspDotNetStorefront.SkinBase
    {

        int LocaleSettingID;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            LocaleSettingID = 0;

            if (CommonLogic.QueryStringCanBeDangerousContent("LocaleSettingID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("LocaleSettingID") != "0")
            {
                Editing = true;
                LocaleSettingID = Localization.ParseUSInt(CommonLogic.QueryStringCanBeDangerousContent("LocaleSettingID"));
            }
            else
            {
                Editing = false;
            }

            IDataReader rs;

            if (CommonLogic.FormBool("IsSubmit"))
            {

                if (Editing)
                {
                    // see if this LocaleSetting already exists:
                    int N = DB.GetSqlN("select count(name) as N from LocaleSetting  " + DB.GetNoLock() + " where LocaleSettingID<>" + LocaleSettingID.ToString() + " and Name=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Name")));
                    if (N != 0)
                    {
                        ErrorMsg = "<p><b><font color=red>ERROR:<br/><br/></font><blockquote>There is already another Locale with that name. Please <a href=\"javascript:history.back(-1);\">go back</a>.</b></blockquote></p>";
                    }
                }
                else
                {
                    // see if this name is already there:
                    int N = DB.GetSqlN("select count(name) as N from LocaleSetting  " + DB.GetNoLock() + " where Name=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Name")));
                    if (N != 0)
                    {
                        ErrorMsg = "<p><b><font color=red>ERROR:<br/><br/></font><blockquote>There is already another Locale with that name. Please <a href=\"javascript:history.back(-1);\">go back</a>.</b></blockquote></p>";
                    }
                }

                if (ErrorMsg.Length == 0)
                {
                    StringBuilder sql = new StringBuilder(2500);
                    if (!Editing)
                    {
                        // ok to add them:
                        String NewGUID = DB.GetNewGUID();
                        sql.Append("insert into LocaleSetting(LocaleSettingGUID,Name,Description,DefaultCurrencyID) values(");
                        sql.Append(DB.SQuote(NewGUID) + ",");
                        sql.Append(DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("Name"), 10)) + ",");
                        sql.Append(DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("Description"), 100)) + ",");
                        sql.Append(Currency.GetCurrencyID(CommonLogic.FormCanBeDangerousContent("DefaultCurrency")).ToString());
                        sql.Append(")");
                        DB.ExecuteSQL(sql.ToString());

                        rs = DB.GetRS("select LocaleSettingID from LocaleSetting  " + DB.GetNoLock() + " where LocaleSettingGUID=" + DB.SQuote(NewGUID));
                        rs.Read();
                        LocaleSettingID = DB.RSFieldInt(rs, "LocaleSettingID");
                        Editing = true;
                        rs.Close();
                        DataUpdated = true;
                        AppLogic.UpdateNumLocaleSettingsInstalled();
                    }
                    else
                    {
                        // ok to update:
                        sql.Append("update LocaleSetting set ");
                        sql.Append("Name=" + DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("Name"), 10)) + ",");
                        sql.Append("Description=" + DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("Description"), 100)) + ",");
                        sql.Append("DefaultCurrencyID=" + Currency.GetCurrencyID(CommonLogic.FormCanBeDangerousContent("DefaultCurrency")).ToString());
                        sql.Append(" where LocaleSettingID=" + LocaleSettingID.ToString());
                        DB.ExecuteSQL(sql.ToString());
                        DataUpdated = true;
                        Editing = true;
                    }
                }
            }
            SectionTitle = "<a href=\"LocaleSettings.aspx\">Locale Settings</a> - Manage Locales" + CommonLogic.IIF(DataUpdated, " (Updated)", "");
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            IDataReader rs = DB.GetRS("select * from LocaleSetting  " + DB.GetNoLock() + " where LocaleSettingID=" + LocaleSettingID.ToString());
            if (rs.Read())
            {
                Editing = true;
            }

            if (ErrorMsg.Length != 0)
            {
                writer.Write("<p><b><font color=red>" + ErrorMsg + "</font></b></p>\n");
            }

            if (ErrorMsg.Length == 0)
            {

                if (Editing)
                {
                    writer.Write("<b>Editing Locale: " + DB.RSField(rs, "Name") + " (ID=" + DB.RSFieldInt(rs, "LocaleSettingID").ToString() + ")<br/><br/></b>\n");
                }
                else
                {
                    writer.Write("<b>Adding New Locale:<br/><br/></b>\n");
                }

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function Form_Validator(theForm)\n");
                writer.Write("{\n");
                writer.Write("submitonce(theForm);\n");
                writer.Write("return (true);\n");
                writer.Write("}\n");
                writer.Write("</script>\n");

                writer.Write("<p>Please enter the following information about this Locale. Fields marked with an asterisk (*) are required. All other fields are optional.</p>\n");
                writer.Write("<form action=\"editLocaleSetting.aspx?LocaleSettingID=" + LocaleSettingID.ToString() + "&edit=" + Editing.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(this) && Form_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n");
                writer.Write("<tr valign=\"middle\">\n");
                writer.Write("<td width=\"100%\" colspan=\"2\" align=\"left\">\n");
                writer.Write("</td>\n");
                writer.Write("</tr>\n");

                writer.Write("<tr>\n");
                writer.Write("<td></td><td align=\"left\" valign=\"top\"><br/>\n");
                if (Editing)
                {
                    writer.Write("<input type=\"submit\" value=\"Update\" name=\"submit\">\n");
                    writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"reset\" class=\"CPButton\" value=\"Reset\" name=\"reset\">\n");
                }
                else
                {
                    writer.Write("<input type=\"submit\" value=\"Add New\" name=\"submit\">\n");
                }
                writer.Write("</td>\n");
                writer.Write("</tr>\n");

                writer.Write("<tr valign=\"middle\">\n");
                writer.Write("<td width=\"25%\" align=\"right\" valign=\"middle\">*Locale Setting:&nbsp;&nbsp;</td>\n");
                writer.Write("<td align=\"left\" valign=\"top\">\n");
                writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Name\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "Name")), "") + "\"> (e.g. en-US, en-GB, etc...)\n");
                writer.Write("<input type=\"hidden\" name=\"Name_vldt\" value=\"[req][blankalert=Please enter the Locale Setting]\">\n");
                writer.Write("</td>\n");
                writer.Write("</tr>\n");

                writer.Write("<tr valign=\"middle\">\n");
                writer.Write("<td width=\"25%\" align=\"right\" valign=\"middle\">*Description:&nbsp;&nbsp;</td>\n");
                writer.Write("<td align=\"left\" valign=\"top\">\n");
                writer.Write("<input maxLength=\"100\" size=\"50\" name=\"Description\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "Description")), "") + "\"> (e.g. United States, United Kingdom, Sweden, etc...)\n");
                writer.Write("<input type=\"hidden\" name=\"Description_vldt\" value=\"[req][blankalert=Please enter the description for this locale. The description should usually be the country name, e.g. United States, United Kingdom, etc...]\">\n");
                writer.Write("</td>\n");
                writer.Write("</tr>\n");

                writer.Write("<tr valign=\"middle\">\n");
                writer.Write("<td width=\"25%\" align=\"right\" valign=\"middle\">*Default Currency:&nbsp;&nbsp;</td>\n");
                writer.Write("<td align=\"left\" valign=\"top\">\n");
                String DefCur = Localization.GetPrimaryCurrency();
                if (Editing)
                {
                    DefCur = Currency.GetCurrencyCode(DB.RSFieldInt(rs, "DefaultCurrencyID"));
                }
                String CurrencySelectList = Currency.GetSelectList("DefaultCurrency", String.Empty, String.Empty, DefCur);
                writer.Write(CurrencySelectList);
                writer.Write("</td>\n");
                writer.Write("</tr>\n");

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
