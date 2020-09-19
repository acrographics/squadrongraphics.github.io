// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the Poll homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editpoll.aspx.cs 7     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.IO;
using System.Collections;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for editpoll
    /// </summary>
    public partial class editpoll : AspDotNetStorefront.SkinBase
    {

        int PollID;
        String PollCategories;
        String PollSections;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            PollID = 0;

            if (CommonLogic.QueryStringCanBeDangerousContent("PollID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("PollID") != "0")
            {
                Editing = true;
                PollID = Localization.ParseUSInt(CommonLogic.QueryStringCanBeDangerousContent("PollID"));
            }
            else
            {
                Editing = false;
            }

            IDataReader rs;

            PollCategories = AppLogic.GetPollCategories(PollID);
            PollSections = AppLogic.GetPollSections(PollID);

            if (CommonLogic.FormBool("IsSubmit"))
            {
                StringBuilder sql = new StringBuilder(2500);
                if (!Editing)
                {
                    // ok to add:
                    String NewGUID = DB.GetNewGUID();
                    sql.Append("insert into Poll(PollGUID,Name,PollSortOrderID,Published,AnonsCanVote,ExpiresOn) values(");
                    sql.Append(DB.SQuote(NewGUID) + ",");
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                    sql.Append(CommonLogic.FormCanBeDangerousContent("PollSortOrderID") + ",");
                    sql.Append(CommonLogic.FormCanBeDangerousContent("Published") + ",");
                    sql.Append(CommonLogic.FormCanBeDangerousContent("AnonsCanVote") + ",");
                    sql.Append(DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("ExpiresOn"), 100)));
                    sql.Append(")");
                    DB.ExecuteSQL(sql.ToString());

                    rs = DB.GetRS("select PollID from Poll  " + DB.GetNoLock() + " where deleted=0 and PollGUID=" + DB.SQuote(NewGUID));
                    rs.Read();
                    PollID = DB.RSFieldInt(rs, "PollID");
                    Editing = true;
                    rs.Close();
                    DataUpdated = true;
                }
                else
                {
                    // ok to update:
                    sql.Append("update Poll set ");
                    sql.Append("Name=" + DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                    sql.Append("PollSortOrderID=" + CommonLogic.FormCanBeDangerousContent("PollSortOrderID") + ",");
                    sql.Append("Published=" + CommonLogic.FormCanBeDangerousContent("Published") + ",");
                    sql.Append("AnonsCanVote=" + CommonLogic.FormCanBeDangerousContent("AnonsCanVote") + ",");
                    sql.Append("ExpiresOn=" + DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("ExpiresOn"), 100)));
                    sql.Append(" where PollID=" + PollID.ToString());
                    DB.ExecuteSQL(sql.ToString());
                    DataUpdated = true;
                    Editing = true;
                }

                // UPDATE CATEGORY MAPPINGS:
                if (DataUpdated)
                {
                    DB.ExecuteSQL("delete from Pollcategory where Pollid=" + PollID.ToString());
                    String CMap = CommonLogic.FormCanBeDangerousContent("CategoryMap");
                    if (CMap.Length != 0)
                    {
                        String[] CMapArray = CMap.Split(',');
                        foreach (String s in CMapArray)
                        {
                            DB.ExecuteSQL("insert into Pollcategory(Pollid,categoryid) values(" + PollID.ToString() + "," + s + ")");
                        }
                    }
                }

                // UPDATE SECTION MAPPINGS:
                if (DataUpdated)
                {
                    DB.ExecuteSQL("delete from Pollsection where Pollid=" + PollID.ToString());
                    String SMap = CommonLogic.FormCanBeDangerousContent("SectionMap");
                    if (SMap.Length != 0)
                    {
                        String[] SMapArray = SMap.Split(',');
                        foreach (String s in SMapArray)
                        {
                            DB.ExecuteSQL("insert into Pollsection(Pollid,sectionid) values(" + PollID.ToString() + "," + s + ")");
                        }
                    }
                }
            }
            SectionTitle = "<a href=\"Polls.aspx\">Polls</a> - Manage Polls";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            if (DataUpdated)
            {
                writer.Write("<p><b><font color=blue>(UPDATED)</font></b></p>\n");
            }

            IDataReader rs = DB.GetRS("select * from Poll  " + DB.GetNoLock() + " where PollID=" + PollID.ToString());
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
                    writer.Write("<p align=\"left\"><b>Editing Poll: " + DB.RSFieldByLocale(rs, "Name", ThisCustomer.LocaleSetting) + " (Poll SKU=" + DB.RSField(rs, "SKU") + ", PollID=" + DB.RSFieldInt(rs, "PollID").ToString() + ")&nbsp;&nbsp;&nbsp;&nbsp;<a href=\"pollanswers.aspx?Pollid=" + PollID.ToString() + "\">Add/Edit Poll Answers</a>&nbsp;&nbsp;&nbsp;&nbsp;<a href=\"managepoll.aspx?Pollid=" + PollID.ToString() + "\">Review Votes</a>");
                    writer.Write("</b>");
                    writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;");
                    writer.Write("</p>\n");
                }
                else
                {
                    writer.Write("<p align=\"left\"><b>Adding New Poll:</p></b>\n");
                }

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function Form_Validator(theForm)\n");
                writer.Write("{\n");
                writer.Write("submitonce(theForm);\n");
                writer.Write("if (theForm.PollSortOrderID.selectedIndex < 1)\n");
                writer.Write("{\n");
                writer.Write("alert(\"Please select the poll sort order.\");\n");
                writer.Write("theForm.PollSortOrderID.focus();\n");
                writer.Write("submitenabled(theForm);\n");
                writer.Write("return (false);\n");
                writer.Write("    }\n");
                writer.Write("return (true);\n");
                writer.Write("}\n");
                writer.Write("</script>\n");

                if (AppLogic.NumLocaleSettingsInstalled() > 1)
                {
                    writer.Write(CommonLogic.ReadFile("jscripts/tabs.js", true));
                }

                writer.Write("<p>Please enter the following information about this Poll. Fields marked with an asterisk (*) are required. All other fields are optional.</p>\n");
                writer.Write("<form action=\"editPoll.aspx?PollID=" + PollID.ToString() + "&edit=" + Editing.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(this) && Form_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"100%\" colspan=\"2\" align=\"left\">\n");
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

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Poll Name:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Name"), "Name", false, true, true, "Please enter the Poll name", 100, 50, 0, 0, false));
                //        writer.Write("                	<input maxLength=\"100\" size=\"50\" name=\"Name\" value=\"" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Name")) , "") + "\">\n");
                //				writer.Write("                	<input type=\"hidden\" name=\"Name_vldt\" value=\"[req][blankalert=Please enter the Poll name]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("            <tr>");
                writer.Write("              <td width=\"25%\" align=\"right\" valign=\"middle\">Expires On:</td>");
                writer.Write("              <td align=\"left\" valign=\"top\"><input type=\"text\" name=\"ExpiresOn\" size=\"11\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(Localization.ToNativeShortDateString(DB.RSFieldDateTime(rs, "ExpiresOn"))), Localization.ToNativeShortDateString(System.DateTime.Now.AddMonths(1))) + "\">&nbsp;<img src=\"" + AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/calendar.gif") + "\" style=\"cursor:hand;cursor:pointer;\" align=\"absmiddle\" id=\"f_trigger_s\">");
                //writer.Write("                	<input type=\"hidden\" name=\"ExpiresOn_vldt\" value=\"[date][invalidalert=Please enter a valid starting date in the format " + Localization.ShortDateFormat() + "]\">");
                writer.Write("</td>");
                writer.Write("            </tr>");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Poll Sort Order:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("<select size=\"1\" name=\"PollSortOrderID\">\n");
                writer.Write(" <OPTION VALUE=\"0\">SELECT ONE</option>\n");
                IDataReader rsst = DB.GetRS("select * from PollSortOrder " + DB.GetNoLock() + " order by DisplayOrder,Name");
                while (rsst.Read())
                {
                    writer.Write("<option value=\"" + DB.RSFieldInt(rsst, "PollSortOrderID").ToString() + "\"");
                    if (Editing)
                    {
                        if (DB.RSFieldInt(rs, "PollSortOrderID") == DB.RSFieldInt(rsst, "PollSortOrderID"))
                        {
                            writer.Write(" selected");
                        }
                    }
                    writer.Write(">" + DB.RSFieldByLocale(rsst, "Name", ThisCustomer.LocaleSetting) + "</option>");
                }
                rsst.Close();
                writer.Write("</select>\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\" bgcolor=\"" + CommonLogic.IIF(Editing && !DB.RSFieldBool(rs, "Published"), "#FFFFCC", "FFFFFF") + "\">*Published:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" bgcolor=\"" + CommonLogic.IIF(Editing && !DB.RSFieldBool(rs, "Published"), "#FFFFCC", "FFFFFF") + "\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"Published\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "Published"), " checked ", ""), " checked ") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"Published\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "Published"), "", " checked "), "") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Anons Can Vote:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"AnonsCanVote\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "AnonsCanVote"), " checked ", ""), "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"AnonsCanVote\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "AnonsCanVote"), "", " checked "), " checked ") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"top\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Category(s):&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");

                writer.Write("<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">\n");
                writer.Write("<tr>");
                writer.Write("<td align=\"left\" valign=\"top\">" + GetCategoryList(PollID, PollCategories, 0, 1, ThisCustomer.LocaleSetting, base.EntityHelpers) + "</td>");
                writer.Write("<td align=\"right\" valign=\"top\">Section(s):&nbsp;&nbsp;</td>");
                writer.Write("<td align=\"left\" valign=\"top\">" + GetSectionList(PollID, PollSections, 0, 1, ThisCustomer.LocaleSetting, base.EntityHelpers) + "</td>");
                writer.Write("</tr>");
                writer.Write("</table>");

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
                writer.Write("\n<script type=\"text/javascript\">\n");
                writer.Write("    Calendar.setup({\n");
                writer.Write("        inputField     :    \"ExpiresOn\",      // id of the input field\n");
                writer.Write("        ifFormat       :    \"" + Localization.JSCalendarDateFormatSpec() + "\",       // format of the input field\n");
                writer.Write("        showsTime      :    false,            // will display a time selector\n");
                writer.Write("        button         :    \"f_trigger_s\",   // trigger for the calendar (button ID)\n");
                writer.Write("        singleClick    :    true            // Single-click mode\n");
                writer.Write("    });\n");
                writer.Write("</script>\n");

            }
            rs.Close();
        }

        static public String GetCategoryList(int PollID, String PollCategories, int ForParentCategoryID, int level, string LocaleSetting, System.Collections.Generic.Dictionary<string, EntityHelper> EntityHelpers)
        {
            StringBuilder tmpS = new StringBuilder(4096);
            String sql = String.Empty;
            EntityHelper CategoryHelper = AppLogic.LookupHelper(EntityHelpers, "Category");
            if (ForParentCategoryID == 0)
            {
                sql = "select * from category  " + DB.GetNoLock() + " where (parentcategoryid=0 or ParentCategoryID IS NULL) and published=1 and deleted=0 order by DisplayOrder,Name";
            }
            else
            {
                sql = "select * from category  " + DB.GetNoLock() + " where parentcategoryid=" + ForParentCategoryID.ToString() + " and published=1 and deleted=0 order by DisplayOrder,Name";
            }
            IDataReader rs = DB.GetRS(sql);

            String Indent = String.Empty;
            for (int i = 1; i < level; i++)
            {
                Indent += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            }
            while (rs.Read())
            {
                bool PollIsMappedToThisCategory = (("," + PollCategories + ",").IndexOf("," + DB.RSFieldInt(rs, "CategoryID").ToString() + ",") != -1);
                tmpS.Append("<input type=\"checkbox\" name=\"CategoryMap\" value=\"" + DB.RSFieldInt(rs, "CategoryID").ToString() + "\" " + CommonLogic.IIF(PollIsMappedToThisCategory, " checked ", "") + ">" + CommonLogic.IIF(level == 1, "<b>", "") + Indent + DB.RSFieldByLocale(rs, "name", LocaleSetting) + CommonLogic.IIF(level == 1, "</b>", "") + "<br/>\n");
                if (CategoryHelper.EntityHasSubs(DB.RSFieldInt(rs, "CategoryID")))
                {
                    tmpS.Append(GetCategoryList(PollID, PollCategories, DB.RSFieldInt(rs, "CategoryID"), level + 1, LocaleSetting, EntityHelpers));
                }
            }
            rs.Close();
            return tmpS.ToString();
        }

        static public String GetSectionList(int PollID, String PollSections, int ForParentSectionID, int level, string LocaleSetting, System.Collections.Generic.Dictionary<string, EntityHelper> EntityHelpers)
        {
            EntityHelper SectionHelper = AppLogic.LookupHelper(EntityHelpers, "Section");
            StringBuilder tmpS = new StringBuilder(4096);
            String sql = String.Empty;
            if (ForParentSectionID == 0)
            {
                sql = "select * from [Section] " + DB.GetNoLock() + " where (ParentSectionID=0 or ParentSectionID IS NULL) and Published=1 and Deleted=0 order by DisplayOrder,Name";
            }
            else
            {
                sql = "select * from [Section] " + DB.GetNoLock() + " where ParentSectionID=" + ForParentSectionID.ToString() + " and Published=1 and Deleted=0 order by DisplayOrder,Name";
            }
            IDataReader rs = DB.GetRS(sql);

            String Indent = String.Empty;
            for (int i = 1; i < level; i++)
            {
                Indent += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            }
            while (rs.Read())
            {
                bool PollIsMappedToThisSection = (("," + PollSections + ",").IndexOf("," + DB.RSFieldInt(rs, "SectionID").ToString() + ",") != -1);
                tmpS.Append("<input type=\"checkbox\" name=\"SectionMap\" value=\"" + DB.RSFieldInt(rs, "SectionID").ToString() + "\" " + CommonLogic.IIF(PollIsMappedToThisSection, " checked ", "") + ">" + CommonLogic.IIF(level == 1, "<b>", "") + Indent + DB.RSFieldByLocale(rs, "name", LocaleSetting) + CommonLogic.IIF(level == 1, "</b>", "") + "<br/>\n");
                if (SectionHelper.EntityHasSubs(DB.RSFieldInt(rs, "SectionID")))
                {
                    tmpS.Append(GetSectionList(PollID, PollSections, DB.RSFieldInt(rs, "SectionID"), level + 1, LocaleSetting, EntityHelpers));
                }
            }
            rs.Close();
            return tmpS.ToString();
        }

    }
}
