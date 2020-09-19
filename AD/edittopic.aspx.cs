// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/edittopic.aspx.cs 6     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for edittopic
    /// </summary>
    public partial class edittopic : AspDotNetStorefront.SkinBase
    {

        int TopicID;

        private void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache"); 
            

            TopicID = 0;

            if (CommonLogic.QueryStringCanBeDangerousContent("TopicID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("TopicID") != "0")
            {
                Editing = true;
                TopicID = Localization.ParseUSInt(CommonLogic.QueryStringCanBeDangerousContent("TopicID"));
            }
            else
            {
                Editing = false;
            }

            IDataReader rs;

            if (CommonLogic.FormCanBeDangerousContent("IsSubmit").ToUpper(CultureInfo.InvariantCulture) == "TRUE")
            {
                StringBuilder sql = new StringBuilder(2500);
                if (!Editing)
                {
                    // ok to add them:
                    String NewGUID = DB.GetNewGUID();
                    sql.Append("insert into Topic(TopicGUID,Name,SkinID,ContentsBGColor,PageBGColor,GraphicsColor,Title,Description,Password,RequiresSubscription,HTMLOk,RequiresDisclaimer,ShowInSiteMap,SEKeywords,SEDescription,SETitle) values(");
                    sql.Append(DB.SQuote(NewGUID) + ",");
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                    sql.Append(CommonLogic.FormUSInt("SkinID") + ",");
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("ContentsBGColor")) + ",");
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("PageBGColor")) + ",");
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("GraphicsColor")) + ",");
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Title")) + ",");
                    if (AppLogic.FormLocaleXml("Description").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Description")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("Password").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("Password")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    sql.Append(CommonLogic.FormUSInt("RequiresSubscription").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("HTMLOk").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("RequiresDisclaimer").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("ShowInSiteMap").ToString() + ",");
                    if (AppLogic.FormLocaleXml("SEKeywords").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SEKeywords")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (AppLogic.FormLocaleXml("SEDescription").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SEDescription")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (AppLogic.FormLocaleXml("SETitle").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SETitle")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    sql.Append(ThisCustomer.CustomerID);
                    sql.Append(")");
                    DB.ExecuteSQL(sql.ToString());

                    rs = DB.GetRS(string.Format( "select TopicID from Topic  {0} where deleted=0 and TopicGUID={1}" , DB.GetNoLock() , DB.SQuote(NewGUID) ));
                    rs.Read();
                    TopicID = DB.RSFieldInt(rs, "TopicID");
                    Editing = true;
                    rs.Close();
                    DataUpdated = true;
                }
                else
                {
                    // ok to update:
                    sql.Append("update Topic set ");
                    sql.Append("Name=" + DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                    sql.Append("SkinID=" + CommonLogic.FormUSInt("SkinID") + ",");
                    sql.Append("ContentsBGColor=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ContentsBGColor")) + ",");
                    sql.Append("PageBGColor=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("PageBGColor")) + ",");
                    sql.Append("GraphicsColor=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("GraphicsColor")) + ",");
                    sql.Append("Title=" + DB.SQuote(AppLogic.FormLocaleXml("Title")) + ",");
                    if (AppLogic.FormLocaleXml("Description").Length != 0)
                    {
                        sql.Append("Description=" + DB.SQuote(AppLogic.FormLocaleXml("Description")) + ",");
                    }
                    else
                    {
                        sql.Append("Description=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("Password").Length != 0)
                    {
                        sql.Append("Password=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Password")) + ",");
                    }
                    else
                    {
                        sql.Append("Password=NULL,");
                    }
                    sql.Append("RequiresSubscription=" + CommonLogic.FormUSInt("RequiresSubscription").ToString() + ",");
                    sql.Append("HTMLOk=" + CommonLogic.FormUSInt("HTMLOk").ToString() + ",");
                    sql.Append("RequiresDisclaimer=" + CommonLogic.FormUSInt("RequiresDisclaimer").ToString() + ",");
                    sql.Append("ShowInSiteMap=" + CommonLogic.FormUSInt("ShowInSiteMap").ToString() + ",");
                    if (AppLogic.FormLocaleXml("SEKeywords").Length != 0)
                    {
                        sql.Append("SEKeywords=" + DB.SQuote(AppLogic.FormLocaleXml("SEKeywords")) + ",");
                    }
                    else
                    {
                        sql.Append("SEKeywords=NULL,");
                    }
                    if (AppLogic.FormLocaleXml("SEDescription").Length != 0)
                    {
                        sql.Append("SEDescription=" + DB.SQuote(AppLogic.FormLocaleXml("SEDescription")) + ",");
                    }
                    else
                    {
                        sql.Append("SEDescription=NULL,");
                    }
                    if (AppLogic.FormLocaleXml("SETitle").Length != 0)
                    {
                        sql.Append("SETitle=" + DB.SQuote(AppLogic.FormLocaleXml("SETitle")));
                    }
                    else
                    {
                        sql.Append("SETitle=NULL");
                    }
                    sql.Append(" where TopicID=" + TopicID.ToString());
                    DB.ExecuteSQL(sql.ToString());
                    DataUpdated = true;
                    Editing = true;
                }
            }
            SectionTitle = "<a href=\"Topics.aspx\">Topics</a> - Manage Topics";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            IDataReader rs = DB.GetRS(string.Format( "select * from Topic  {0} where TopicID={1}" , DB.GetNoLock() , TopicID.ToString() ));
            if (rs.Read())
            {
                Editing = true;
            }

            if (ErrorMsg.Length != 0)
            {
                writer.Write("<p><b><font color=red>" + ErrorMsg + "</font></b></p>\n");
            }
            if (DataUpdated)
            {
                writer.Write("<p align=\"left\"><b><font color=blue>(UPDATED)</font></b></p>\n");
            }


            if (ErrorMsg.Length == 0)
            {

                if (Editing)
                {
                    writer.Write("<b>Editing Topic: " + DB.RSFieldByLocale(rs, "Name", ThisCustomer.LocaleSetting) + " (ID=" + DB.RSFieldInt(rs, "TopicID").ToString() + ")<br/><br/></b>\n");
                }
                else
                {
                    writer.Write("<b>Adding New Topic:<br/><br/></b>\n");
                }

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

                if (TopicID != 0)
                {
                    writer.Write("<p align=\"left\"><a href=\"edittopic.aspx\"><b>Make Another New Topic</b></a></p>");
                }

                writer.Write("<p>Please enter the following information about this topic. Fields marked with an asterisk (*) are required. All other fields are optional.</p>\n");
                writer.Write("<form action=\"edittopic.aspx?TopicID=" + TopicID.ToString() + "&edit=" + Editing.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(document.forms[0]) && Form_Validator(document.forms[0]))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"100%\" colspan=\"2\" align=\"left\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Topic Name:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Name"), "Name", false, true, true, "Please enter the topic name", 100, 30, 0, 0, false));
                //				writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"Name\" value=\"" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Name")) , "") + "\">\n");
                //				writer.Write("                	<input type=\"hidden\" name=\"Name_vldt\" value=\"[req][blankalert=Please enter the topic name]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Topic Title:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Title"), "Title", false, true, true, "Please enter the topic page title", 100, 30, 0, 0, false));
                //        writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"Title\" value=\"" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Title")) , "") + "\">\n");
                //				writer.Write("                	<input type=\"hidden\" name=\"Title_vldt\" value=\"[req][blankalert=Please enter the topic page title]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Applies Only To Skin:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("        <input type=\"text\" name=\"SkinID\" size=\"3\" maxlength=\"3\" value=\"" + CommonLogic.IIF(DB.RSFieldInt(rs, "SkinID") == 0, "", DB.RSFieldInt(rs, "SkinID").ToString()) + "\">");
                writer.Write("        <input type=\"hidden\" name=\"SkinID_vldt\" value=\"[number][invalidalert=Please enter a skin number, e.g. 1]\">\n");
                writer.Write("&nbsp;<small>(Leave blank to allow topic to apply to all skins!)</small>");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Description:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Description"), "Description", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), !Editing || DB.RSFieldBool(rs, "HTMLOk")));
                //        writer.Write("                	<textarea style=\"height: 60em; width: 100%;\" cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeightl") + "\" id=\"Description\" name=\"Description\">" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Description")) , "") + "</textarea>\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Search Engine Page Title:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "SETitle"), "SETitle", false, true, false, "", 100, 100, 0, 0, false));
                //        writer.Write("                	<input maxLength=\"100\" size=\"100\" name=\"SETitle\" value=\"" + CommonLogic.IIF(Editing , DB.RSField(rs,"SETitle") , "") + "\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Search Engine Keywords:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "SEKeywords"), "SEKeywords", false, true, false, "", 255, 100, 0, 0, false));
                //				writer.Write("                	<input maxLength=\"255\" size=\"100\" name=\"SEKeywords\" value=\"" + CommonLogic.IIF(Editing , DB.RSField(rs,"SEKeywords") , "") + "\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Search Engine Description:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "SEDescription"), "SEDescription", false, true, false, "", 255, 100, 0, 0, false));
                //        writer.Write("                	<input maxLength=\"255\" size=\"100\" name=\"SEDescription\" value=\"" + CommonLogic.IIF(Editing , DB.RSField(rs,"SEDescription") , "") + "\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Password:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input Password=\"100\" size=\"20\" name=\"Password\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "Password"), "") + "\"> (Only required if you want to protect this topic content by requiring a password to be entered)\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("      <tr>");
                writer.Write("                <td align=\"right\" valign=\"top\">Requires Subscription:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"RequiresSubscription\" value=\"1\" " + CommonLogic.IIF(DB.RSFieldBool(rs, "RequiresSubscription"), " checked ", "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"RequiresSubscription\" value=\"0\" " + CommonLogic.IIF(DB.RSFieldBool(rs, "RequiresSubscription"), "", " checked ") + ">\n");
                writer.Write("        </td>");
                writer.Write("      </tr>");

                writer.Write("      <tr>");
                writer.Write("                <td align=\"right\" valign=\"top\">HTML Ok:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"HTMLOk\" value=\"1\" " + CommonLogic.IIF(!Editing || DB.RSFieldBool(rs, "HTMLOk"), " checked ", "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"HTMLOk\" value=\"0\" " + CommonLogic.IIF(!Editing || DB.RSFieldBool(rs, "HTMLOk"), "", " checked ") + ">\n");
                writer.Write("        </td>");
                writer.Write("      </tr>");

                writer.Write("      <tr>");
                writer.Write("                <td align=\"right\" valign=\"top\">Requires Disclaimer:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"RequiresDisclaimer\" value=\"1\" " + CommonLogic.IIF(DB.RSFieldBool(rs, "RequiresDisclaimer"), " checked ", "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"RequiresDisclaimer\" value=\"0\" " + CommonLogic.IIF(DB.RSFieldBool(rs, "RequiresDisclaimer"), "", " checked ") + ">\n");
                writer.Write("        </td>");
                writer.Write("      </tr>");

                writer.Write("      <tr>");
                writer.Write("                <td align=\"right\" valign=\"top\">Publish In Site Map:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"ShowInSiteMap\" value=\"1\" " + CommonLogic.IIF(DB.RSFieldBool(rs, "ShowInSiteMap"), " checked ", "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"ShowInSiteMap\" value=\"0\" " + CommonLogic.IIF(DB.RSFieldBool(rs, "ShowInSiteMap"), "", " checked ") + ">\n");
                writer.Write("        </td>");
                writer.Write("      </tr>");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Page BG Color:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"20\" size=\"10\" name=\"PageBGColor\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "PageBGColor"), "") + "\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Contents BG Color:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"20\" size=\"10\" name=\"ContentsBGColor\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "ContentsBGColor"), "") + "\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Skin Graphics Color:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"20\" size=\"10\" name=\"GraphicsColor\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "GraphicsColor"), "") + "\">\n");
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
            }
            rs.Close();

        }

    }
}
