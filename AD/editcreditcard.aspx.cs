// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editcreditcard.aspx.cs 6     9/30/06 3:39p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for editcreditcard
    /// </summary>
    public partial class editcreditcard : AspDotNetStorefront.SkinBase
    {

        int CardTypeID;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            CardTypeID = 0;

            if (CommonLogic.QueryStringCanBeDangerousContent("CardTypeID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("CardTypeID") != "0")
            {
                Editing = true;
                CardTypeID = Localization.ParseUSInt(CommonLogic.QueryStringCanBeDangerousContent("CardTypeID"));
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
                    // see if this card type already exists:
                    int N = DB.GetSqlN("select count(CardType) as N from CreditCardType  " + DB.GetNoLock() + " where CardTypeID<>" + CardTypeID.ToString() + " and upper(CardType)=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("CardType").ToUpperInvariant()));
                    if (N != 0)
                    {
                        ErrorMsg = "<p><b><font color=red>ERROR:<br/><br/></font><blockquote>There is already another credit card type with that name. Please <a href=\"javascript:history.back(-1);\">go back</a>.</b></blockquote></p>";
                    }
                }
                else
                {
                    // see if this cardtype is already there:
                    int N = DB.GetSqlN("select count(CardType) as N from CreditCardType  " + DB.GetNoLock() + " where upper(CardType)=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("CardType").ToUpperInvariant()));
                    if (N != 0)
                    {
                        ErrorMsg = "<p><b><font color=red>ERROR:<br/><br/></font><blockquote>There is already another credit card type with that name. Please <a href=\"javascript:history.back(-1);\">go back</a>.</b></blockquote></p>";
                    }
                }

                if (ErrorMsg.Length == 0)
                {
                    StringBuilder sql = new StringBuilder(2500);
                    if (!Editing)
                    {
                        // ok to add them:
                        String NewGUID = DB.GetNewGUID();
                        sql.Append("insert into CreditCardType(CardTypeGUID,CardType) values(");
                        sql.Append(DB.SQuote(NewGUID) + ",");
                        sql.Append(DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("CardType"), 100)));
                        sql.Append(")");
                        DB.ExecuteSQL(sql.ToString());

                        rs = DB.GetRS("select CardTypeID from CreditCardType  " + DB.GetNoLock() + " where CardTypeGUID=" + DB.SQuote(NewGUID));
                        rs.Read();
                        CardTypeID = DB.RSFieldInt(rs, "CardTypeID");
                        Editing = true;
                        rs.Close();
                        DataUpdated = true;
                    }
                    else
                    {
                        // ok to update:
                        sql.Append("update CreditCardType set ");
                        sql.Append("CardType=" + DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("CardType"), 100)));
                        sql.Append(" where CardTypeID=" + CardTypeID.ToString());
                        DB.ExecuteSQL(sql.ToString());
                        DataUpdated = true;
                        Editing = true;
                    }
                }

            }
            SectionTitle = "<a href=\"creditcards.aspx\">Credit Cards</a> - Manage Credit Card Types";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            IDataReader rs = DB.GetRS("select * from CreditCardType  " + DB.GetNoLock() + " where CardTypeID=" + CardTypeID.ToString());
            if (rs.Read())
            {
                rs.Close();
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
                    writer.Write("<b>Editing Credit Card Type: " + DB.RSField(rs, "CardType") + " (ID=" + DB.RSFieldInt(rs, "CardTypeID").ToString() + ")<br/><br/></b>\n");
                }
                else
                {
                    writer.Write("<b>Adding New Credit Card Type:<br/><br/></b>\n");
                }

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function Form_Validator(theForm)\n");
                writer.Write("{\n");
                writer.Write("submitonce(theForm);\n");
                writer.Write("return (true);\n");
                writer.Write("}\n");
                writer.Write("</script>\n");

                writer.Write("<p>Please enter the following information about this credit card type. Fields marked with an asterisk (*) are required. All other fields are optional.</p>\n");
                writer.Write("<form enctype=\"multipart/form-data\" action=\"editcreditcard.aspx?CardTypeID=" + CardTypeID.ToString() + "&edit=" + Editing.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(this) && Form_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"100%\" colspan=\"2\" align=\"left\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Credit Card Type:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"CardType\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "CardType")), "") + "\">\n");
                writer.Write("                	<input type=\"hidden\" name=\"CardType_vldt\" value=\"[req][blankalert=Please enter the credit card type]\">\n");
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
