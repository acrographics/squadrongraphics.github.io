// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editpollanswer.aspx.cs 6     9/30/06 3:38p Redwoodtree $
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
    /// Summary description for editpollanswer
    /// </summary>
    public partial class editpollanswer : AspDotNetStorefront.SkinBase
    {

        int PollID;
        int PollAnswerID;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            PollID = CommonLogic.QueryStringUSInt("PollID");
            PollAnswerID = 0;


            if (CommonLogic.QueryStringCanBeDangerousContent("PollAnswerID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("PollAnswerID") != "0")
            {
                Editing = true;
                PollAnswerID = Localization.ParseUSInt(CommonLogic.QueryStringCanBeDangerousContent("PollAnswerID"));
            }
            else
            {
                Editing = false;
            }
            if (PollID == 0)
            {
                Response.Redirect("polls.aspx");
            }


            IDataReader rs;

            if (CommonLogic.FormBool("IsSubmit"))
            {

                StringBuilder sql = new StringBuilder(2500);
                if (!Editing)
                {
                    // ok to add:
                    String NewGUID = DB.GetNewGUID();
                    sql.Append("insert into PollAnswer(PollAnswerGUID,PollID,Name) values(");
                    sql.Append(DB.SQuote(NewGUID) + ",");
                    sql.Append(PollID.ToString() + ",");
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Name")));
                    sql.Append(")");
                    DB.ExecuteSQL(sql.ToString());

                    rs = DB.GetRS("select PollAnswerID from PollAnswer  " + DB.GetNoLock() + " where deleted=0 and PollAnswerGUID=" + DB.SQuote(NewGUID));
                    rs.Read();
                    PollAnswerID = DB.RSFieldInt(rs, "PollAnswerID");
                    Editing = true;
                    rs.Close();
                    DataUpdated = true;
                }
                else
                {
                    // ok to update:
                    sql.Append("update PollAnswer set ");
                    sql.Append("PollID=" + PollID.ToString() + ",");
                    sql.Append("Name=" + DB.SQuote(AppLogic.FormLocaleXml("Name")));
                    sql.Append(" where PollAnswerID=" + PollAnswerID.ToString());
                    DB.ExecuteSQL(sql.ToString());
                    DataUpdated = true;
                    Editing = true;
                }
            }
            SectionTitle = "<a href=\"PollAnswers.aspx?Pollid=" + PollID.ToString() + "\">PollAnswers</a> - Manage PollAnswers";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            if (DataUpdated)
            {
                writer.Write("<p><b><font color=blue>(UPDATED)</font></b></p>\n");
            }

            IDataReader rs = DB.GetRS("select * from PollAnswer  " + DB.GetNoLock() + " where PollAnswerID=" + PollAnswerID.ToString());
            if (rs.Read())
            {
                Editing = true;
            }

            if (ErrorMsg.Length != 0)
            {
                writer.Write("<p align=\"left\"><b><font color=red>" + ErrorMsg + "</font></b></p>\n");
            }


            if (ErrorMsg.Length == 0)
            {

                writer.Write("<p align=\"left\"><b>Within Poll: <a href=\"editPoll.aspx?Pollid=" + PollID.ToString() + "\">" + AppLogic.GetPollName(PollID, ThisCustomer.LocaleSetting) + "</a> (PollID=" + PollID.ToString() + ")</b</p>\n");
                if (Editing)
                {
                    writer.Write("<p align=\"left\"><b>Editing Poll Answer: " + DB.RSFieldByLocale(rs, "Name", ThisCustomer.LocaleSetting) + " (PollAnswerID=" + DB.RSFieldInt(rs, "PollAnswerID").ToString() + ")</b></p>\n");
                }
                else
                {
                    writer.Write("<p align=\"left\"><b>Adding New Poll Answer:</p></b>\n");
                }

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function Form_Validator(theForm)\n");
                writer.Write("{\n");
                writer.Write("submitonce(theForm);\n");
                writer.Write("return (true);\n");
                writer.Write("}\n");
                writer.Write("</script>\n");

                writer.Write("<p align=\"left\">Please enter the following information about this Poll Answer. Fields marked with an asterisk (*) are required. All other fields are optional.</p>\n");
                writer.Write("<form action=\"editPollAnswer.aspx?Pollid=" + PollID.ToString() + "&PollAnswerID=" + PollAnswerID.ToString() + "&edit=" + Editing.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(this) && Form_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
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
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Answer Text:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Name"), "Name", false, true, true, "Please enter the answer text", 100, 30, 0, 0, false));
                //writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"Name\" value=\"" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Name")) , "") + "\">\n");
                //writer.Write("                	<input type=\"hidden\" name=\"Name_vldt\" value=\"[req][blankalert=Please enter the answer text]\">\n");
                writer.Write("                	</td>\n");
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
