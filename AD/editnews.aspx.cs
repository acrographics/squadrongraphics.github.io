// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editnews.aspx.cs 7     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary NewsCopy for editnews
    /// </summary>
    public partial class editnews : AspDotNetStorefront.SkinBase
    {

        int NewsID;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache"); 
            

            NewsID = 0;
            if (CommonLogic.QueryStringCanBeDangerousContent("NewsID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("NewsID") != "0")
            {
                Editing = true;
                NewsID = Localization.ParseUSInt(CommonLogic.QueryStringCanBeDangerousContent("NewsID"));
            }
            else
            {
                Editing = false;
            }

            IDataReader rs;

            if (CommonLogic.FormBool("IsSubmit"))
            {
                StringBuilder sql = new StringBuilder(2500);
                DateTime dt = System.DateTime.Now.AddMonths(6);
                if (CommonLogic.FormCanBeDangerousContent("ExpiresOn").Length > 0)
                {
                    dt = Localization.ParseNativeDateTime(CommonLogic.FormCanBeDangerousContent("ExpiresOn"));
                }
                if (!Editing)
                {
                    // ok to add them:
                    String NewGUID = DB.GetNewGUID();
                    sql.Append("insert into news(NewsGUID,ExpiresOn,Headline,NewsCopy,Published) values(");
                    sql.Append(DB.SQuote(NewGUID) + ",");
                    sql.Append(DB.DateQuote(Localization.ToDBDateTimeString(dt)) + ",");
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Headline")) + ",");
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("NewsCopy")) + ",");
                    sql.Append(CommonLogic.FormCanBeDangerousContent("Published"));
                    sql.Append(")");
                    DB.ExecuteSQL(sql.ToString());

                    rs = DB.GetRS("select NewsID from news " + DB.GetNoLock() + " where deleted=0 and NewsGUID=" + DB.SQuote(NewGUID));
                    rs.Read();
                    NewsID = DB.RSFieldInt(rs, "NewsID");
                    Editing = true;
                    rs.Close();
                    DataUpdated = true;
                }
                else
                {
                    // ok to update:
                    sql.Append("update news set ");
                    sql.Append("Headline=" + DB.SQuote(AppLogic.FormLocaleXml("Headline")) + ",");
                    sql.Append("NewsCopy=" + DB.SQuote(AppLogic.FormLocaleXml("NewsCopy")) + ",");
                    sql.Append("ExpiresOn=" + DB.DateQuote(Localization.ToDBDateTimeString(dt)) + ",");
                    sql.Append("Published=" + CommonLogic.FormCanBeDangerousContent("Published"));
                    sql.Append(" where NewsID=" + NewsID.ToString());
                    DB.ExecuteSQL(sql.ToString());
                    DataUpdated = true;
                    Editing = true;
                }
            }
            SectionTitle = "<a href=\"news.aspx\">News</a> - Manage News" + CommonLogic.IIF(DataUpdated, " (Updated)", "");
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            IDataReader rs = DB.GetRS("select * from News " + DB.GetNoLock() + " where NewsID=" + NewsID.ToString());
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
                    writer.Write("<b>Editing News: (ID=" + DB.RSFieldInt(rs, "NewsID").ToString() + ")<br/><br/></b>\n");
                }
                else
                {
                    writer.Write("<b>Adding New News Item:<br/><br/></b>\n");
                }

                writer.Write("  <!-- calendar stylesheet -->\n");
                writer.Write("  <link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"jscalendar/calendar-win2k-cold-1.css\" title=\"win2k-cold-1\" />\n");
                writer.Write("\n");
                writer.Write("  <!-- main calendar program -->\n");
                writer.Write("  <script type=\"text/javascript\" src=\"jscalendar/calendar.js\"></script>\n");
                writer.Write("\n");
                writer.Write("  <!-- language for the calendar -->\n");
                writer.Write("  <script type=\"text/javascript\" src=\"jscalendar/lang/" + Localization.JSCalendarLanguageFile() + "\"></script>\n");
                writer.Write("\n");
                writer.Write("  <!-- the following script defines the Calendar.setup helper function, which makes\n");
                writer.Write("       adding a calendar a matter of 1 or 2 lines of code. -->\n");
                writer.Write("  <script type=\"text/javascript\" src=\"jscalendar/calendar-setup.js\"></script>\n");

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function Form_Validator(theForm)\n");
                writer.Write("{\n");
                writer.Write("submitonce(theForm);\n");
                writer.Write("return (true);\n");
                writer.Write("}\n");
                writer.Write("</script>\n");

                if (AppLogic.NumLocaleSettingsInstalled() > 1)
                {
                    writer.Write(CommonLogic.ReadFile("jscripts/tabs.js", true));
                }

                writer.Write("<p>Please enter the following information about this news item. Fields marked with an asterisk (*) are required. All other fields are optional.</p>\n");
                writer.Write("<form enctype=\"multipart/form-data\" action=\"editnews.aspx?NewsID=" + NewsID.ToString() + "&edit=" + Editing.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(document.forms[0]) && Form_Validator(document.forms[0]))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"100%\" colspan=\"2\" align=\"left\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Headline:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Headline"), "Headline", false, true, true, "Please enter the news headline", 100, 50, 0, 0, false));
                //        writer.Write("                	<textarea cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeight") + "\" name=\"NewsCopy\">" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"NewsCopy")) , "") + "</textarea>\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">News Copy:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "NewsCopy"), "NewsCopy", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true));
                //        writer.Write("                	<textarea cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeight") + "\" name=\"NewsCopy\">" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"NewsCopy")) , "") + "</textarea>\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Expiration Date:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"ExpiresOn\" value=\"" + CommonLogic.IIF(Editing, Localization.ToNativeShortDateString(DB.RSFieldDateTime(rs, "ExpiresOn")), Localization.ToNativeShortDateString(System.DateTime.Now.AddMonths(1))) + "\">&nbsp;<img src=\"" + AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/calendar.gif") + "\" style=\"cursor:hand;cursor:pointer;\" align=\"absmiddle\" id=\"f_trigger_s\">&nbsp; <small>(" + Localization.ShortDateFormat() + ")</small>\n");
                writer.Write("                	<input type=\"hidden\" name=\"ExpiresOn_vldt\" value=\"[req][blankalert=Please enter the expiration date (e.g. " + Localization.ShortDateFormat() + ")]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Published:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"Published\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "Published"), " checked ", ""), " checked ") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"Published\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "Published"), "", " checked "), "") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");


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
                writer.Write("        </td>\n");
                writer.Write("      </tr>\n");
                writer.Write("  </table>\n");
                writer.Write("</form>\n");


                writer.Write("\n<script type=\"text/javascript\">\n");
                writer.Write("    Calendar.setup({\n");
                writer.Write("        inputField     :    \"ExpiresOn\",      // id of the input field\n");
                writer.Write("        ifFormat       :    \"" + Localization.JSCalendarDateFormatSpec() + "\",       // format of the input field\n");
                writer.Write("        showsTime      :    false,            // will display a time selector\n");
                writer.Write("        button         :    \"f_trigger_s\",   // trigger for the calendar (button ID)\n");
                writer.Write("        singleClick    :    true            // double-click mode\n");
                writer.Write("    });\n");
                writer.Write("</script>\n");
            }
            rs.Close();
        }

    }
}
