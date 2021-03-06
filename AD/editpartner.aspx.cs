// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editpartner.aspx.cs 8     9/30/06 3:38p Redwoodtree $
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
    /// Summary description for editpartners
    /// </summary>
    public partial class editpartners : AspDotNetStorefront.SkinBase
    {
        private int PartnerID;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            PartnerID = 0;
            if (CommonLogic.QueryStringCanBeDangerousContent("PartnerID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("PartnerID") != "0")
            {
                Editing = true;
                PartnerID = Localization.ParseUSInt(CommonLogic.QueryStringCanBeDangerousContent("PartnerID"));
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
                    sql.Append("insert into partner(PartnerGUID,Name,Published,Address1,Address2,Suite,City,State,ZipCode,Phone,FAX,URL,EMail,LinkToSite,LinkInNewWindow) values(");
                    sql.Append(DB.SQuote(NewGUID) + ",");
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                    sql.Append(CommonLogic.FormCanBeDangerousContent("Published") + ",");
                    if (CommonLogic.FormCanBeDangerousContent("Address1").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("Address1")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("Address2").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("Address2")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("Suite").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("Suite")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("City").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("City")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("State").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("State")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("ZipCode").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("ZipCode")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("Phone").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.MakeProperPhoneFormat(CommonLogic.FormCanBeDangerousContent("Phone"))) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("FAX").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.MakeProperPhoneFormat(CommonLogic.FormCanBeDangerousContent("FAX"))) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("URL").Length != 0)
                    {
                        String theUrl = CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("URL"), 80);
                        if (theUrl.IndexOf("http://") == -1 && theUrl.Length != 0)
                        {
                            theUrl = "http://" + theUrl;
                        }
                        if (theUrl.Length == 0)
                        {
                            sql.Append("NULL,");
                        }
                        else
                        {
                            sql.Append(DB.SQuote(theUrl) + ",");
                        }
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("EMail").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("EMail"), 100)) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    sql.Append(CommonLogic.FormCanBeDangerousContent("LinkToSite") + ",");
                    sql.Append(CommonLogic.FormCanBeDangerousContent("LinkInNewWindow"));
                    sql.Append(")");
                    DB.ExecuteSQL(sql.ToString());

                    rs = DB.GetRS("select PartnerID from partner  " + DB.GetNoLock() + " where deleted=0 and PartnerGUID=" + DB.SQuote(NewGUID));
                    rs.Read();
                    PartnerID = DB.RSFieldInt(rs, "PartnerID");
                    Editing = true;
                    rs.Close();
                    DataUpdated = true;

                    sql.Remove(0, sql.Length);
                    sql.Append("update partner set ");
                    if (AppLogic.FormLocaleXml("Summary").Length != 0)
                    {
                        sql.Append("Summary=" + DB.SQuote(AppLogic.FormLocaleXml("Summary")));
                    }
                    else
                    {
                        sql.Append("Summary=NULL");
                    }
                    sql.Append(" where PartnerID=" + PartnerID.ToString());
                    DB.ExecuteSQL(sql.ToString());

                    sql.Remove(0, sql.Length);
                    sql.Append("update partner set ");
                    if (AppLogic.FormLocaleXml("Specialty").Length != 0)
                    {
                        sql.Append("Specialty=" + DB.SQuote(AppLogic.FormLocaleXml("Specialty")));
                    }
                    else
                    {
                        sql.Append("Specialty=NULL");
                    }
                    sql.Append(" where PartnerID=" + PartnerID.ToString());
                    DB.ExecuteSQL(sql.ToString());

                    sql.Remove(0, sql.Length);
                    sql.Append("update partner set ");
                    if (AppLogic.FormLocaleXml("Instructors").Length != 0)
                    {
                        sql.Append("Instructors=" + DB.SQuote(AppLogic.FormLocaleXml("Instructors")));
                    }
                    else
                    {
                        sql.Append("Instructors=NULL");
                    }
                    sql.Append(" where PartnerID=" + PartnerID.ToString());
                    DB.ExecuteSQL(sql.ToString());

                    sql.Remove(0, sql.Length);
                    sql.Append("update partner set ");
                    if (AppLogic.FormLocaleXml("Schedule").Length != 0)
                    {
                        sql.Append("Schedule=" + DB.SQuote(AppLogic.FormLocaleXml("Schedule")));
                    }
                    else
                    {
                        sql.Append("Schedule=NULL");
                    }
                    sql.Append(" where PartnerID=" + PartnerID.ToString());
                    DB.ExecuteSQL(sql.ToString());

                    sql.Remove(0, sql.Length);
                    sql.Append("update partner set ");
                    if (AppLogic.FormLocaleXml("Testimonials").Length != 0)
                    {
                        sql.Append("Testimonials=" + DB.SQuote(AppLogic.FormLocaleXml("Testimonials")));
                    }
                    else
                    {
                        sql.Append("Testimonials=NULL");
                    }
                    sql.Append(" where PartnerID=" + PartnerID.ToString());
                    DB.ExecuteSQL(sql.ToString());

                }
                else
                {
                    // ok to update:
                    sql.Append("update partner set ");
                    sql.Append("Name=" + DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                    sql.Append("Published=" + CommonLogic.FormCanBeDangerousContent("Published") + ",");
                    if (CommonLogic.FormCanBeDangerousContent("Address1").Length != 0)
                    {
                        sql.Append("Address1=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Address1")) + ",");
                    }
                    else
                    {
                        sql.Append("Address1=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("Address2").Length != 0)
                    {
                        sql.Append("Address2=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Address2")) + ",");
                    }
                    else
                    {
                        sql.Append("Address2=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("Suite").Length != 0)
                    {
                        sql.Append("Suite=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Suite")) + ",");
                    }
                    else
                    {
                        sql.Append("Suite=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("City").Length != 0)
                    {
                        sql.Append("City=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("City")) + ",");
                    }
                    else
                    {
                        sql.Append("City=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("State").Length != 0)
                    {
                        sql.Append("State=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("State")) + ",");
                    }
                    else
                    {
                        sql.Append("State=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("ZipCode").Length != 0)
                    {
                        sql.Append("ZipCode=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ZipCode")) + ",");
                    }
                    else
                    {
                        sql.Append("ZipCode=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("Phone").Length != 0)
                    {
                        sql.Append("Phone=" + DB.SQuote(AppLogic.MakeProperPhoneFormat(CommonLogic.FormCanBeDangerousContent("Phone"))) + ",");
                    }
                    else
                    {
                        sql.Append("Phone=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("FAX").Length != 0)
                    {
                        sql.Append("FAX=" + DB.SQuote(AppLogic.MakeProperPhoneFormat(CommonLogic.FormCanBeDangerousContent("FAX"))) + ",");
                    }
                    else
                    {
                        sql.Append("FAX=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("URL").Length != 0)
                    {
                        String theUrl2 = CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("URL"), 80);
                        if (theUrl2.IndexOf("http://") == -1 && theUrl2.Length != 0)
                        {
                            theUrl2 = "http://" + theUrl2;
                        }
                        if (theUrl2.Length != 0)
                        {
                            sql.Append("URL=" + DB.SQuote(theUrl2) + ",");
                        }
                        else
                        {
                            sql.Append("URL=NULL,");
                        }
                    }
                    else
                    {
                        sql.Append("URL=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("EMail").Length != 0)
                    {
                        sql.Append("EMail=" + DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("EMail"), 100)) + ",");
                    }
                    else
                    {
                        sql.Append("EMail=NULL,");
                    }
                    sql.Append("LinkToSite=" + CommonLogic.FormCanBeDangerousContent("LinkToSite") + ",");
                    sql.Append("LinkInNewWindow=" + CommonLogic.FormCanBeDangerousContent("LinkInNewWindow"));
                    sql.Append(" where PartnerID=" + PartnerID.ToString());
                    DB.ExecuteSQL(sql.ToString());


                    sql.Remove(0, sql.Length);
                    sql.Append("update partner set ");
                    if (AppLogic.FormLocaleXml("Summary").Length != 0)
                    {
                        sql.Append("Summary=" + DB.SQuote(AppLogic.FormLocaleXml("Summary")));
                    }
                    else
                    {
                        sql.Append("Summary=NULL");
                    }
                    sql.Append(" where PartnerID=" + PartnerID.ToString());
                    DB.ExecuteSQL(sql.ToString());

                    sql.Remove(0, sql.Length);
                    sql.Append("update partner set ");
                    if (AppLogic.FormLocaleXml("Specialty").Length != 0)
                    {
                        sql.Append("Specialty=" + DB.SQuote(AppLogic.FormLocaleXml("Specialty")));
                    }
                    else
                    {
                        sql.Append("Specialty=NULL");
                    }
                    sql.Append(" where PartnerID=" + PartnerID.ToString());
                    DB.ExecuteSQL(sql.ToString());

                    sql.Remove(0, sql.Length);
                    sql.Append("update partner set ");
                    if (AppLogic.FormLocaleXml("Instructors").Length != 0)
                    {
                        sql.Append("Instructors=" + DB.SQuote(AppLogic.FormLocaleXml("Instructors")));
                    }
                    else
                    {
                        sql.Append("Instructors=NULL");
                    }
                    sql.Append(" where PartnerID=" + PartnerID.ToString());
                    DB.ExecuteSQL(sql.ToString());

                    sql.Remove(0, sql.Length);
                    sql.Append("update partner set ");
                    if (AppLogic.FormLocaleXml("Schedule").Length != 0)
                    {
                        sql.Append("Schedule=" + DB.SQuote(AppLogic.FormLocaleXml("Schedule")));
                    }
                    else
                    {
                        sql.Append("Schedule=NULL");
                    }
                    sql.Append(" where PartnerID=" + PartnerID.ToString());
                    DB.ExecuteSQL(sql.ToString());

                    sql.Remove(0, sql.Length);
                    sql.Append("update partner set ");
                    if (AppLogic.FormLocaleXml("Testimonials").Length != 0)
                    {
                        sql.Append("Testimonials=" + DB.SQuote(AppLogic.FormLocaleXml("Testimonials")));
                    }
                    else
                    {
                        sql.Append("Testimonials=NULL");
                    }
                    sql.Append(" where PartnerID=" + PartnerID.ToString());
                    DB.ExecuteSQL(sql.ToString());

                    DataUpdated = true;
                    Editing = true;
                }


                // handle image uploaded:
                try
                {
                    String Image1 = String.Empty;
                    HttpPostedFile Image1File = Request.Files["Image1"];
                    if (Image1File.ContentLength != 0)
                    {
                        // delete any current image file first
                        try
                        {
                            System.IO.File.Delete(AppLogic.GetImagePath("Partner", "icon", true) + PartnerID.ToString() + ".jpg");
                            System.IO.File.Delete(AppLogic.GetImagePath("Partner", "icon", true) + PartnerID.ToString() + ".gif");
                            System.IO.File.Delete(AppLogic.GetImagePath("Partner", "icon", true) + PartnerID.ToString() + ".png");
                        }
                        catch
                        { }

                        String s = Image1File.ContentType;
                        switch (Image1File.ContentType)
                        {
                            case "image/gif":
                                Image1 = AppLogic.GetImagePath("Partner", "icon", true) + PartnerID.ToString() + ".gif";
                                Image1File.SaveAs(Image1);
                                break;
                            case "image/x-png":
                                Image1 = AppLogic.GetImagePath("Partner", "icon", true) + PartnerID.ToString() + ".png";
                                Image1File.SaveAs(Image1);
                                break;
                            case "image/jpg":
                            case "image/jpeg":
                            case "image/pjpeg":
                                Image1 = AppLogic.GetImagePath("Partner", "icon", true) + PartnerID.ToString() + ".jpg";
                                Image1File.SaveAs(Image1);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMsg = CommonLogic.GetExceptionDetail(ex, "<br/>");
                }
            }
            SectionTitle = "<a href=\"partners.aspx\">Partners</a> - Manage Partners " + CommonLogic.IIF(DataUpdated, " (Updated)", "");
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            IDataReader rs = DB.GetRS("select * from Partner  " + DB.GetNoLock() + " where PartnerID=" + PartnerID.ToString());
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
                    writer.Write("<b>Editing Partner: " + DB.RSFieldByLocale(rs, "Name", ThisCustomer.LocaleSetting) + " (ID=" + DB.RSFieldInt(rs, "PartnerID").ToString() + ")<br/><br/></b>\n");
                }
                else
                {
                    writer.Write("<b>Adding New Partner:<br/><br/></b>\n");
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

                writer.Write("<p>Please enter the following information about this partner. Fields marked with an asterisk (*) are required. All other fields are optional.</p>\n");
                writer.Write("<form enctype=\"multipart/form-data\" action=\"editpartner.aspx?PartnerID=" + PartnerID.ToString() + "&edit=" + Editing.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(document.forms[0]) && Form_Validator(document.forms[0]))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n");

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
                writer.Write("                <td width=\"100%\" colspan=\"2\" align=\"left\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Partner Name:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Name"), "Name", false, true, true, "Please enter the partner name", 100, 30, 0, 0, false));
                //writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"Name\" value=\"" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Name")) , "") + "\">\n");
                //writer.Write("                	<input type=\"hidden\" name=\"Name_vldt\" value=\"[req][blankalert=Please enter the partner name]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Published:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"Published\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "Published"), " checked ", " checked "), " checked ") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"Published\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "Published"), "", " checked "), "") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">Street Address:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"Address1\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "Address1")), "") + "\">\n");
                //writer.Write("                    <input type=\"hidden\" name=\"Address1_vldt\" value=\"[req][blankalert=Please enter a street address]\">\n");
                writer.Write("                	&nbsp;&nbsp;\n");
                writer.Write("                	Apt/Suite#:\n");
                writer.Write("                	<input maxLength=\"100\" size=\"5\" name=\"Suite\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "Suite")), "") + "\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\"></td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"Address2\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "Address2")), "") + "\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">City:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"City\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "City")), "") + "\">\n");
                //writer.Write("                    <input type=\"hidden\" name=\"City_vldt\" value=\"[req][blankalert=Please enter a city]\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">State:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">");
                writer.Write("<select size=\"1\" name=\"State\">");
                writer.Write("<OPTION value=\"\"" + CommonLogic.IIF(DB.RSField(rs, "shippingstate").Length == 0, " selected", String.Empty) + ">SELECT ONE</option>");
                DataSet dsstate = DB.GetDS("select * from state  " + DB.GetNoLock() + " order by DisplayOrder,Name", AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
                foreach (DataRow row in dsstate.Tables[0].Rows)
                {
                    writer.Write("<OPTION value=\"" + DB.RowField(row, "Abbreviation") + "\"" + CommonLogic.SelectOption(rs, DB.RowField(row, "Abbreviation"), "shippingstate") + ">" + DB.RowField(row, "Name") + "</option>");
                }
                dsstate.Dispose();
                writer.Write("</select>");
                writer.Write("			</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">Zip Code:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"16\" size=\"15\" name=\"ZipCode\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "ZipCode"), "") + "\">\n");
                writer.Write("                    <input type=\"hidden\" name=\"ZipCode_vldt\" value=\"[invalidalert=Please enter a valid zipcode]\">\n");
                writer.Write("                </td>\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">Web Site:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"100\" size=\"35\" name=\"URL\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "URL"), "") + "\">&nbsp;&nbsp;<small>(e.g. http://abcd.com)</small>\n");
                //writer.Write("                    <input type=\"hidden\" name=\"URL_vldt\" value=\"[req][EMail][blankalert=Please enter the partner's Web site address]\">\n");
                writer.Write("               	</td>\n");
                writer.Write("              </tr>\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Link To Web Site:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LinkToSite\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "LinkToSite"), " checked ", " checked "), "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LinkToSite\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "LinkToSite"), "", " checked "), " checked ") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Link Opens New Window:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LinkInNewWindow\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "LinkInNewWindow"), " checked ", " checked "), "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LinkInNewWindow\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "LinkInNewWindow"), "", " checked "), " checked ") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">E-Mail Address:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"100\" size=\"35\" name=\"EMail\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "EMail"), CommonLogic.QueryStringCanBeDangerousContent("EMail")) + "\">\n");
                writer.Write("                    <input type=\"hidden\" name=\"EMail_vldt\" value=\"[invalidalert=Please enter a valid e-mail address]\">\n");
                writer.Write("               	</td>\n");
                writer.Write("              </tr>\n");
                writer.Write("              </tr>\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">Phone:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"35\" size=\"35\" name=\"Phone\" value=\"" + CommonLogic.IIF(Editing, CommonLogic.GetPhoneDisplayFormat(DB.RSField(rs, "Phone")), "") + "\">&nbsp;&nbsp;<small>(optional, including area code)</small>\n");
                writer.Write("                    <input type=\"hidden\" name=\"Phone_vldt\" value=\"[invalidalert=Please enter a valid phone number with areacode, e.g. (480) 555-1212]\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">Fax:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"35\" size=\"35\" name=\"FAX\" value=\"" + CommonLogic.IIF(Editing, CommonLogic.GetPhoneDisplayFormat(DB.RSField(rs, "Fax")), "") + "\">&nbsp;&nbsp;<small>(optional, including area code)</small>\n");
                writer.Write("                    <input type=\"hidden\" name=\"FAX_vldt\" value=\"[invalidalert=Please enter a valid FAX number with areacode, e.g. (480) 555-1212]\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");
                writer.Write("  <tr>\n");
                writer.Write("    <td valign=\"top\" align=\"right\">Icon:\n");
                writer.Write("</td>\n");
                writer.Write("    <td valign=\"top\" align=\"left\">");
                writer.Write("    <input type=\"file\" name=\"Image1\" size=\"30\" value=\"\">\n");
                String Image1URL = AppLogic.LookupImage("Partner", PartnerID, "", SkinID, ThisCustomer.LocaleSetting);
                if (Image1URL.Length != 0)
                {
                    if (Image1URL.IndexOf("nopicture") == -1)
                    {
                        writer.Write("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image1URL + "','PartnerPic');\">Click here</a> to delete the current image<br/>\n");
                    }
                    writer.Write("<br/><img id=\"PartnerPic\" name=\"PartnerPic\" border=\"0\" src=\"" + Image1URL + "\">\n");
                }
                writer.Write("</td>\n");
                writer.Write(" </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Summary:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Summary"), "Summary", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true));
                //        writer.Write("                	<textarea style=\"height: 30em; width: 100%;\" cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeightSmall") + "\" id=\"Summary\" name=\"Summary\">" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Summary")) , "") + "</textarea>\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Area of Specialty:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Specialty"), "Specialty", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true));
                //        writer.Write("                	<textarea cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeight") + "\" name=\"Specialty\">" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Specialty")) , "") + "</textarea>\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Instructors:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Instructors"), "Instructors", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true));
                //        writer.Write("                	<textarea cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeight") + "\" name=\"Instructors\">" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Instructors")) , "") + "</textarea>\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Upcoming Schedule:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Schedule"), "Schedule", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true));
                //        writer.Write("                	<textarea cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeight") + "\" name=\"Schedule\">" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Schedule")) , "") + "</textarea>\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Testimonials:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Testimonials"), "Testimonials", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true));
                //        writer.Write("                	<textarea cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeight") + "\" name=\"Testimonials\">" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Testimonials")) , "") + "</textarea>\n");
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

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function DeleteImage(imgurl,name)\n");
                writer.Write("{\n");
                writer.Write("window.open('deleteimage.aspx?imgurl=' + imgurl + '&FormImageName=' + name,\"AspDotNetStorefrontAdmin_ML\",\"height=250,width=440,top=10,left=20,status=no,toolbar=no,menubar=no,scrollbars=yes,location=no\")\n");
                writer.Write("}\n");
                writer.Write("</SCRIPT>\n");

            }
            rs.Close();
        }

    }
}
