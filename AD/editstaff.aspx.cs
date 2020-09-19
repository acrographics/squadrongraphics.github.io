// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editstaff.aspx.cs 8     9/30/06 3:38p Redwoodtree $
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
    /// Summary description for editstaff
    /// </summary>
    public partial class editstaff : AspDotNetStorefront.SkinBase
    {

        int StaffID;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            StaffID = 0;

            if (CommonLogic.QueryStringCanBeDangerousContent("StaffID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("StaffID") != "0")
            {
                Editing = true;
                StaffID = Localization.ParseUSInt(CommonLogic.QueryStringCanBeDangerousContent("StaffID"));
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
                    // see if this staff already exists:
                    int N = DB.GetSqlN("select count(name) as N from staff  " + DB.GetNoLock() + " where StaffID<>" + StaffID.ToString() + " and deleted=0 and upper(Name)=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Name").ToUpperInvariant()));
                    if (N != 0)
                    {
                        ErrorMsg = "<p><b><font color=red>ERROR:<br/><br/></font><blockquote>There is already another staff member with that name. Please <a href=\"javascript:history.back(-1);\">go back</a>.</b></blockquote></p>";
                    }
                }
                else
                {
                    // see if this name is already there:
                    int N = DB.GetSqlN("select count(name) as N from Staff  " + DB.GetNoLock() + " where deleted=0 and upper(Name)=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Name").ToUpperInvariant()));
                    if (N != 0)
                    {
                        ErrorMsg = "<p><b><font color=red>ERROR:<br/><br/></font><blockquote>There is already another staff member with that name. Please <a href=\"javascript:history.back(-1);\">go back</a>.</b></blockquote></p>";
                    }
                }

                StringBuilder sql = new StringBuilder(2500);
                if (!Editing)
                {
                    // ok to add them:
                    String NewGUID = DB.GetNewGUID();
                    sql.Append("insert into staff(StaffGUID,Name,Published,Title,Phone,FAX,EMail) values(");
                    sql.Append(DB.SQuote(NewGUID) + ",");
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("Name")) + ",");
                    sql.Append(CommonLogic.FormCanBeDangerousContent("Published") + ",");
                    if (CommonLogic.FormCanBeDangerousContent("Title").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("Title")) + ",");
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
                    if (CommonLogic.FormCanBeDangerousContent("EMail").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("EMail")));
                    }
                    else
                    {
                        sql.Append("NULL");
                    }
                    sql.Append(")");
                    DB.ExecuteSQL(sql.ToString());

                    rs = DB.GetRS("select StaffID from staff  " + DB.GetNoLock() + " where deleted=0 and StaffGUID=" + DB.SQuote(NewGUID));
                    rs.Read();
                    StaffID = DB.RSFieldInt(rs, "StaffID");
                    Editing = true;
                    rs.Close();
                    DataUpdated = true;

                    sql.Remove(0, sql.Length);
                    sql.Append("update staff set ");
                    if (AppLogic.FormLocaleXml("Bio").Length != 0)
                    {
                        sql.Append("Bio=" + DB.SQuote(AppLogic.FormLocaleXml("Bio")));
                    }
                    else
                    {
                        sql.Append("Bio=NULL");
                    }
                    sql.Append(" where StaffID=" + StaffID.ToString());
                    DB.ExecuteSQL(sql.ToString());
                }
                else
                {
                    // ok to update:
                    sql.Append("update staff set ");
                    sql.Append("Name=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Name")) + ",");
                    sql.Append("Published=" + CommonLogic.FormCanBeDangerousContent("Published") + ",");
                    if (CommonLogic.FormCanBeDangerousContent("Title").Length != 0)
                    {
                        sql.Append("Title=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Title")) + ",");
                    }
                    else
                    {
                        sql.Append("Title=NULL,");
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
                    if (CommonLogic.FormCanBeDangerousContent("EMail").Length != 0)
                    {
                        sql.Append("EMail=" + DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("EMail"), 100)));
                    }
                    else
                    {
                        sql.Append("EMail=NULL");
                    }
                    sql.Append(" where StaffID=" + StaffID.ToString());
                    DB.ExecuteSQL(sql.ToString());


                    sql.Remove(0, sql.Length);
                    sql.Append("update staff set ");
                    if (AppLogic.FormLocaleXml("Bio").Length != 0)
                    {
                        sql.Append("Bio=" + DB.SQuote(AppLogic.FormLocaleXml("Bio")));
                    }
                    else
                    {
                        sql.Append("Bio=NULL");
                    }
                    sql.Append(" where StaffID=" + StaffID.ToString());
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
                            System.IO.File.Delete(AppLogic.GetImagePath("Staff", "icon", true) + StaffID.ToString() + ".jpg");
                            System.IO.File.Delete(AppLogic.GetImagePath("Staff", "icon", true) + StaffID.ToString() + ".gif");
                            System.IO.File.Delete(AppLogic.GetImagePath("Staff", "icon", true) + StaffID.ToString() + ".png");
                        }
                        catch
                        { }

                        String s = Image1File.ContentType;
                        switch (Image1File.ContentType)
                        {
                            case "image/gif":
                                Image1 = AppLogic.GetImagePath("Staff", "icon", true) + StaffID.ToString() + ".gif";
                                Image1File.SaveAs(Image1);
                                break;
                            case "image/x-png":
                                Image1 = AppLogic.GetImagePath("Staff", "icon", true) + StaffID.ToString() + ".png";
                                Image1File.SaveAs(Image1);
                                break;
                            case "image/jpg":
                            case "image/jpeg":
                            case "image/pjpeg":
                                Image1 = AppLogic.GetImagePath("Staff", "icon", true) + StaffID.ToString() + ".jpg";
                                Image1File.SaveAs(Image1);
                                break;
                        }
                    }

                    String Image2 = String.Empty;
                    HttpPostedFile Image2File = Request.Files["Image2"];
                    if (Image2File.ContentLength != 0)
                    {
                        // delete any current image file first
                        try
                        {
                            System.IO.File.Delete(AppLogic.GetImagePath("Staff", "medium", true) + StaffID.ToString() + ".jpg");
                            System.IO.File.Delete(AppLogic.GetImagePath("Staff", "medium", true) + StaffID.ToString() + ".gif");
                            System.IO.File.Delete(AppLogic.GetImagePath("Staff", "medium", true) + StaffID.ToString() + ".png");
                        }
                        catch
                        { }

                        String s = Image2File.ContentType;
                        switch (Image2File.ContentType)
                        {
                            case "image/gif":
                                Image2 = AppLogic.GetImagePath("Staff", "medium", true) + StaffID.ToString() + ".gif";
                                Image2File.SaveAs(Image2);
                                break;
                            case "image/x-png":
                                Image2 = AppLogic.GetImagePath("Staff", "medium", true) + StaffID.ToString() + ".png";
                                Image2File.SaveAs(Image2);
                                break;
                            case "image/jpg":
                            case "image/jpeg":
                            case "image/pjpeg":
                                Image2 = AppLogic.GetImagePath("Staff", "medium", true) + StaffID.ToString() + ".jpg";
                                Image2File.SaveAs(Image2);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMsg = CommonLogic.GetExceptionDetail(ex, "<br/>");
                }
            }
            SectionTitle = "<a href=\"staff.aspx\">Staff</a> - Manage Staff";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            IDataReader rs = DB.GetRS("select * from Staff  " + DB.GetNoLock() + " where StaffID=" + StaffID.ToString());
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
                    writer.Write("<b>Editing Staff: " + DB.RSField(rs, "Name") + " (ID=" + DB.RSFieldInt(rs, "StaffID").ToString() + ")<br/><br/></b>\n");
                }
                else
                {
                    writer.Write("<b>Adding New Staff:<br/><br/></b>\n");
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

                writer.Write("<p>Please enter the following information about this staff member. Fields marked with an asterisk (*) are required. All other fields are optional.</p>\n");
                writer.Write("<form enctype=\"multipart/form-data\" action=\"editstaff.aspx?StaffID=" + StaffID.ToString() + "&edit=" + Editing.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(document.forms[0]) && Form_Validator(document.forms[0]))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
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
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Name:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"Name\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "Name")), "") + "\">\n");
                writer.Write("                	<input type=\"hidden\" name=\"Name_vldt\" value=\"[req][blankalert=Please enter the staff name]\">\n");
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
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Title:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"Title\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "Title")), "") + "\">\n");
                writer.Write("                	</td>\n");
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
                writer.Write("    <input type=\"file\" name=\"Image1\" size=\"50\" value=\"" + CommonLogic.IIF(Editing, "", "") + "\">\n");
                String Image1URL = AppLogic.LookupImage("Staff", StaffID, "icon", SkinID, ThisCustomer.LocaleSetting);
                if (Image1URL.Length != 0)
                {
                    if (Image1URL.IndexOf("spacer") == -1)
                    {
                        writer.Write("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image1URL + "','Pic1');\">Click here</a> to delete the current image<br/>\n");
                    }
                    writer.Write("<br/><img id=\"Pic1\" name=\"Pic1\" border=\"0\" src=\"" + Image1URL + "\">\n");
                }
                writer.Write("</td>\n");
                writer.Write(" </tr>\n");

                writer.Write("  <tr>\n");
                writer.Write("    <td valign=\"top\" align=\"right\">Medium Pic:\n");
                writer.Write("</td>\n");
                writer.Write("    <td valign=\"top\" align=\"left\">");
                writer.Write("    <input type=\"file\" name=\"Image2\" size=\"50\" value=\"" + CommonLogic.IIF(Editing, "", "") + "\">\n");
                String Image2URL = AppLogic.LookupImage("Staff", StaffID, "medium", SkinID, ThisCustomer.LocaleSetting);
                if (Image2URL.Length != 0)
                {
                    if (Image2URL.IndexOf("spacer") == -1)
                    {
                        writer.Write("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image2URL + "','Pic2');\">Click here</a> to delete the current image<br/>\n");
                        writer.Write("<br/><img id=\"Pic2\" name=\"Pic2\" border=\"0\" src=\"" + Image2URL + "\">\n");
                    }
                }
                writer.Write("</td>\n");
                writer.Write(" </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Bio:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Bio"), "Bio", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true));
                //        writer.Write("                	<textarea cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeight") + "\" name=\"Bio\">" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Bio")) , "") + "</textarea>\n");
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
                writer.Write("window.open('deleteimage.aspx?imgurl=' + imgurl + '&FormImageName=' + name,\"Admin_ML\",\"height=250,width=440,top=10,left=20,status=no,toolbar=no,menubar=no,scrollbars=yes,location=no\")\n");
                writer.Write("}\n");

                writer.Write("</script>\n");

            }
            rs.Close();
        }

    }
}
