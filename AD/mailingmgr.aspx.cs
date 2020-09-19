// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/mailingmgr.aspx.cs 10    9/14/06 12:05a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.IO;
using System.Web.UI;
using ASPDNSFEditor;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for mailingmgr
    /// </summary>
    public partial class mailingmgr : AspDotNetStorefront.SkinBase
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            Server.ScriptTimeout = 50000;
            SectionTitle = "Mailing Manager";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            if (CommonLogic.FormCanBeDangerousContent("RemoveEMail").Length != 0)
            {
                DB.ExecuteSQL("update customer set OKToEMail=0 where EMail=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("RemoveEMail").ToLowerInvariant()));
                writer.Write("<p align=\"left\"><b>Customer Removed...<br/></b></p>");
            }

            Topic t = new Topic("MailFooter", ThisCustomer.LocaleSetting, SkinID);
            String sFooter = t.Contents;
            if (CommonLogic.FormBool("IsSubmit"))
            {
                String Subject = CommonLogic.FormCanBeDangerousContent("Subject");
                String Body = CommonLogic.FormCanBeDangerousContent("MessageBodyText");
                int CustomerLevelID = Localization.ParseUSInt(CommonLogic.FormCanBeDangerousContent("CustomerLevelID"));
                String FromEMail = AppLogic.AppConfig("ReceiptEMailFrom");
                String FromName = AppLogic.AppConfig("ReceiptEMailFromName");
                String ToEMail = String.Empty;
                String FromServer = AppLogic.MailServer();

                bool testing = (CommonLogic.FormCanBeDangerousContent("TESTONLY") == "1");
                bool ordersonly = (CommonLogic.FormCanBeDangerousContent("ORDERSONLY") == "1");
                bool listonly = (CommonLogic.FormCanBeDangerousContent("LISTONLY") == "1");
                writer.Write("<div align=\"left\">");
                String tmpS = String.Empty;
                String ToList = String.Empty;
                if (testing)
                {
                    writer.Write("Sending TEST to: " + FromEMail + "...");
                    try
                    {
                        AppLogic.SendMail(Subject, Body + sFooter, true, FromEMail, FromName, FromEMail, FromName, "", FromServer);
                    }
                    catch (Exception ex)
                    {
                        writer.Write(CommonLogic.GetExceptionDetail(ex, "<br/>"));
                    }
                    writer.Write("done<br/>");
                    writer.Flush();
                }
                else
                {
                    String CustSQL = "select CustomerGUID,EMail from Customer " + DB.GetNoLock() + " where EMail like '%@%' and EMail not in (select ToEMail from mailingmgrlog  " + DB.GetNoLock() + " where month(senton)=" + System.DateTime.Now.Month.ToString() + " and day(senton)=" + System.DateTime.Now.Day.ToString() + " and year(SentOn)=" + System.DateTime.Now.Year.ToString() + " and Subject=" + DB.SQuote(Subject) + ") and Deleted=0 and OKToEMail=1 and EMail <> '' " + CommonLogic.IIF(CustomerLevelID != 0, " and CustomerLevelID=" + CustomerLevelID.ToString(), "") + CommonLogic.IIF(ordersonly, " and CustomerID in (select distinct CustomerID from Orders " + DB.GetNoLock() + " where TransactionState in (" + DB.SQuote(AppLogic.ro_TXStateAuthorized) + "," + DB.SQuote(AppLogic.ro_TXStateCaptured) + ")) ", "") + " order by CustomerID";
                    //writer.Write("Customer Mailing SQL=" + CustSQL + "<br/>");
                    DataSet ds = DB.GetDS(CustSQL, false);
                    int NumEMails = 0;
                    String[] s = new String[ds.Tables[0].Rows.Count];
                    String[] guid = new String[ds.Tables[0].Rows.Count];
                    // validate each e-mail:
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        String EM = DB.RowField(row, "EMail");
                        bool EMailIsValid = true;
                        if (EMailIsValid)
                        {
                            s[NumEMails] = EM;
                            guid[NumEMails] = DB.RowField(row, "CustomerGUID");
                            NumEMails++;
                        }
                    }
                    ds.Dispose();

                    int BCCSize = AppLogic.AppConfigUSInt("MailingMgr.BlockSize");
                    if (BCCSize == 0)
                    {
                        BCCSize = 5; // default
                    }
                    if (AppLogic.AppConfigBool("MailingMgr.SendEachEMailSeparately"))
                    {
                        // send each e-mail separately:
                        for (int i = 0; i < NumEMails; i++)
                        {
                            string Footer = sFooter.Replace("%REMOVEURL%", AppLogic.GetStoreHTTPLocation(false) + "remove.aspx?id=" + guid[i]);
                            String ToThisPerson = s[i];
                            writer.Write(CommonLogic.IIF(listonly, "JUST LISTING ADDRESS:<br/>", "SENDING LIVE TO:<br/>") + ToThisPerson + "...<br/>");
                            try
                            {
                                if (!listonly)
                                {
                                    AppLogic.SendMail(Subject, Body + Footer, true, FromEMail, FromName, ToThisPerson, ToThisPerson, String.Empty, FromServer);
                                    DB.ExecuteSQL("insert into MailingMgrLog(ToEMail,FromEMail,Subject,Body) values(" + DB.SQuote(ToThisPerson) + "," + DB.SQuote(FromEMail) + "," + DB.SQuote(Subject) + "," + DB.SQuote(Body) + ")");
                                }
                            }
                            catch (Exception ex)
                            {
                                writer.Write(CommonLogic.GetExceptionDetail(ex, "<br/>"));
                            }
                            writer.Write("done<br/>");
                            writer.Flush();
                        }
                    }
                    else
                    {
                        // send in groups of bcc's:
                        int i = 0;
                        string Footer = sFooter.Replace("%REMOVEURL%", String.Empty); // no way to do this really, so remove the remove link! 
                        while (i < NumEMails)
                        {
                            ToList = String.Empty;
                            for (int j = 1; j <= BCCSize && i < NumEMails; j++)
                            {
                                if (ToList.Length != 0)
                                {
                                    ToList += ";";
                                }
                                ToList += s[i];
                                i++;
                            }
                            writer.Write(CommonLogic.IIF(listonly, "JUST LISTING ADDRESS:<br/>", "SENDING LIVE TO:<br/>") + ToList.Replace(";", "...<br/>") + "<br/>");
                            try
                            {
                                if (!listonly)
                                {
                                    AppLogic.SendMail(Subject, Body + Footer, true, FromEMail, FromName, FromEMail, FromName, ToList, FromServer);
                                    String[] sentto = ToList.Split(';');
                                    foreach (String ss in sentto)
                                    {
                                        DB.ExecuteSQL("insert into MailingMgrLog(ToEMail,FromEMail,Subject,Body) values(" + DB.SQuote(ss) + "," + DB.SQuote(FromEMail) + "," + DB.SQuote(Subject) + "," + DB.SQuote(Body) + ")");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                writer.Write(CommonLogic.GetExceptionDetail(ex, "<br/>"));
                            }
                            writer.Write("done<br/>");
                            writer.Flush();
                        }
                    }
                }
                writer.Write("</div>");
            }

            if (ErrorMsg.Length != 0)
            {
                writer.Write("<p><b><font color=red>" + ErrorMsg + "</font></b></p>\n");
            }

            writer.Write("<p align=\"left\">You may use this page to send e-mails to all registered customers who have chosen to accept e-mails. You can remove customers from the mailing list at the bottom of the page.</p>\n");
            writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n");
            writer.Write("<form action=\"mailingmgr.aspx\" method=\"post\" id=\"Form1\" name=\"Form1\">\n");
            writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");

            writer.Write("<tr valign=\"middle\">\n");
            writer.Write("<td width=\"25%\" align=\"right\" valign=\"middle\">*Subject:&nbsp;&nbsp;</td>\n");
            writer.Write("<td align=\"left\">\n");
            writer.Write("<input maxLength=\"100\" size=\"30\" name=\"Subject\" value=\"" + CommonLogic.FormCanBeDangerousContent("Subject") + "\">\n");
            writer.Write("</td>\n");
            writer.Write("</tr>\n");

            writer.Write("<tr valign=\"middle\">\n");
            writer.Write("<td width=\"25%\" align=\"right\" valign=\"middle\">*TEST ONLY:&nbsp;&nbsp;</td>\n");
            writer.Write("<td align=\"left\">\n");
            writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"TESTONLY\" value=\"1\" checked>\n");
            writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"TESTONLY\" value=\"0\">\n");
            writer.Write("&nbsp;&nbsp;(if testing, only 1 e-mail will be sent to the store contact address)");
            writer.Write("</td>\n");
            writer.Write("</tr>\n");

            writer.Write("<tr valign=\"middle\">\n");
            writer.Write("<td width=\"25%\" align=\"right\" valign=\"middle\">*CUSTOMERS WITH ORDERS ONLY:&nbsp;&nbsp;</td>\n");
            writer.Write("<td align=\"left\">\n");
            writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"ORDERSONLY\" value=\"1\" checked>\n");
            writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"ORDERSONLY\" value=\"0\">\n");
            writer.Write("</td>\n");
            writer.Write("</tr>\n");

            writer.Write("<tr valign=\"middle\">\n");
            writer.Write("<td width=\"25%\" align=\"right\" valign=\"middle\">*LIST CUSTOMERS WHO WOULD BE E-MAILED ONLY:&nbsp;&nbsp;</td>\n");
            writer.Write("<td align=\"left\">\n");
            writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LISTONLY\" value=\"1\" checked>\n");
            writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LISTONLY\" value=\"0\">\n");
            writer.Write("&nbsp;&nbsp;(if list only, e-mails will not be sent)");
            writer.Write("</td>\n");
            writer.Write("</tr>\n");

            writer.Write("<tr valign=\"middle\">\n");
            writer.Write("<td width=\"25%\" align=\"right\" valign=\"middle\">Customer Level:&nbsp;&nbsp;</td>\n");
            writer.Write("<td align=\"left\">\n");
            writer.Write("<select size=\"1\" name=\"CustomerLevelID\">\n");
            writer.Write("<OPTION VALUE=\"0\" " + CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("CustomerLevelID") == "0", " selected ", "") + ">All Customers</option>\n");
            DataSet dsst = DB.GetDS("select * from CustomerLevel  " + DB.GetNoLock() + " where deleted=0 order by DisplayOrder,Name", false, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
            foreach (DataRow row in dsst.Tables[0].Rows)
            {
                writer.Write("<option value=\"" + DB.RowFieldInt(row, "CustomerLevelID").ToString() + "\"");
                if (CommonLogic.FormUSInt("CustomerLevelID") == DB.RowFieldInt(row, "CustomerLevelID"))
                {
                    writer.Write(" selected");
                }
                writer.Write(">" + DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting) + "</option>");
            }
            dsst.Dispose();
            writer.Write("</select>\n");
            writer.Write("</td>\n");
            writer.Write("</tr>\n");

            writer.Write("<tr valign=\"middle\">\n");
            writer.Write("<td width=\"25%\" align=\"right\" valign=\"top\">Message Body:&nbsp;&nbsp;</td>\n");
            writer.Write("<td align=\"left\">\n");
            //writer.Write("<textarea style=\"height: 60em; width: 100%;\" cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeight") + "\" id=\"MessageBodyText\" name=\"MessageBodyText\">" + Server.HtmlEncode() + "</textarea>\n");
            //writer.Write(AppLogic.GetLocaleEntryFields(String.Empty,"MessageBodyText",true,false,false,"",0,0,AppLogic.AppConfigUSInt("Admin_TextareaHeight"),AppLogic.AppConfigUSInt("Admin_TextareaWidth"),true));

            writer.Write("<div id=\"idMessageBodyText\">");
            writer.Write("<textarea style=\"width: 100%;\" rows=\"40\" id=\"MessageBodyText\" name=\"MessageBodyText\">" + CommonLogic.FormCanBeDangerousContent("MessageBodyText") + "</textarea>\n");
            writer.Write(AppLogic.GenerateInnovaEditor("MessageBodyText"));
            writer.Write("</div>");
            writer.Write("</td>\n");
            writer.Write("</tr>\n");

            writer.Write("<tr valign=\"middle\">\n");
            writer.Write("<td width=\"25%\" align=\"right\" valign=\"top\">Message Footer (From Topic: MailFooter):&nbsp;&nbsp;</td>\n");
            writer.Write("<td align=\"left\">\n");
            writer.Write(sFooter);
            writer.Write("</td>\n");
            writer.Write("</tr>\n");

            writer.Write("<tr>\n");
            writer.Write("<td></td><td align=\"left\"><br/>\n");
            writer.Write("<input type=\"submit\" value=\"Submit\" name=\"submit\">\n");
            writer.Write("</td>\n");
            writer.Write("</tr>\n");
            writer.Write("</form>\n");
            writer.Write("</table>\n");

            writer.Write("<hr size=\"1\"/>");
            writer.Write("<div align=\"left\">\n");
            writer.Write("<b>USE THE FORM BELOW TO REMOVE SOMEONE FROM THE MAILING LIST:</b><br/><br/>\n");
            writer.Write("<form action=\"mailingmgr.aspx\" method=\"post\" id=\"Form2\" name=\"Form2\">\n");
            writer.Write("<b>Remove Customer E-Mail From Mailing List:</b> <input maxLength=\"100\" size=\"30\" name=\"RemoveEMail\" value=\"\">\n");
            writer.Write("&nbsp;&nbsp;<input type=\"submit\" value=\"Submit\" name=\"submit\">\n");
            writer.Write("</form>");
            writer.Write("</div>");
        }

    }
}
