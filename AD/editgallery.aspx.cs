// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editgallery.aspx.cs 7     9/30/06 3:39p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.Collections;
using System.IO;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for editgallery
    /// </summary>
    public partial class editgallery : AspDotNetStorefront.SkinBase
    {

        int GalleryID;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            GalleryID = 0;

            if (CommonLogic.QueryStringCanBeDangerousContent("GalleryID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("GalleryID") != "0")
            {
                Editing = true;
                GalleryID = Localization.ParseUSInt(CommonLogic.QueryStringCanBeDangerousContent("GalleryID"));
            }
            else
            {
                Editing = false;
            }

            IDataReader rs;

            //			int N = 0;
            if (CommonLogic.FormBool("IsSubmit"))
            {
                if (ErrorMsg.Length == 0)
                {
                    StringBuilder sql = new StringBuilder(2500);
                    if (!Editing)
                    {
                        // ok to add them:
                        String NewGUID = DB.GetNewGUID();
                        sql.Append("insert into gallery(GalleryGUID,Name,DirName,Description) values(");
                        sql.Append(DB.SQuote(NewGUID) + ",");
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                        sql.Append(DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("DirName"), 100)) + ",");
                        if (AppLogic.FormLocaleXml("Description").Length != 0)
                        {
                            sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Description")));
                        }
                        else
                        {
                            sql.Append("NULL");
                        }
                        sql.Append(")");
                        DB.ExecuteSQL(sql.ToString());

                        rs = DB.GetRS("select GalleryID from gallery  " + DB.GetNoLock() + " where deleted=0 and GalleryGUID=" + DB.SQuote(NewGUID));
                        rs.Read();
                        GalleryID = DB.RSFieldInt(rs, "GalleryID");
                        Editing = true;
                        rs.Close();
                        DataUpdated = true;
                        // BUILD THE GALLERY DIRECTORY:
                        String SFP = CommonLogic.SafeMapPath("../images/spacer.gif").Replace("images\\spacer.gif", "images\\gallery") + "\\" + AppLogic.GetGalleryDir(GalleryID);
                        try
                        {
                            if (!Directory.Exists(SFP))
                            {
                                DirectoryInfo di = Directory.CreateDirectory(SFP);
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        // ok to update:
                        sql.Append("update gallery set ");
                        sql.Append("Name=" + DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                        if (AppLogic.FormLocaleXml("Description").Length != 0)
                        {
                            sql.Append("Description=" + DB.SQuote(AppLogic.FormLocaleXml("Description")));
                        }
                        else
                        {
                            sql.Append("Description=NULL");
                        }
                        sql.Append(" where GalleryID=" + GalleryID.ToString());
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
                                System.IO.File.Delete(AppLogic.GetImagePath("Gallery", "icon", true) + GalleryID.ToString() + ".jpg");
                                System.IO.File.Delete(AppLogic.GetImagePath("Gallery", "icon", true) + GalleryID.ToString() + ".gif");
                                System.IO.File.Delete(AppLogic.GetImagePath("Gallery", "icon", true) + GalleryID.ToString() + ".png");
                            }
                            catch
                            { }

                            String s = Image1File.ContentType;
                            switch (Image1File.ContentType)
                            {
                                case "image/gif":
                                    Image1 = AppLogic.GetImagePath("Gallery", "icon", true) + GalleryID.ToString() + ".gif";
                                    Image1File.SaveAs(Image1);
                                    break;
                                case "image/x-png":
                                    Image1 = AppLogic.GetImagePath("Gallery", "icon", true) + GalleryID.ToString() + ".png";
                                    Image1File.SaveAs(Image1);
                                    break;
                                case "image/jpg":
                                case "image/jpeg":
                                case "image/pjpeg":
                                    Image1 = AppLogic.GetImagePath("Gallery", "icon", true) + GalleryID.ToString() + ".jpg";
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

            }
            SectionTitle = "<a href=\"galleries.aspx\">Galleries</a> - Manage Galleries" + CommonLogic.IIF(DataUpdated, " (Updated)", "");
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            IDataReader rs = DB.GetRS("select * from Gallery  " + DB.GetNoLock() + " where GalleryID=" + GalleryID.ToString());
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
                    writer.Write("<b>Editing Gallery: " + DB.RSFieldByLocale(rs, "Name", ThisCustomer.LocaleSetting) + " (ID=" + DB.RSFieldInt(rs, "GalleryID").ToString() + ")<br/><br/></b>\n");
                }
                else
                {
                    writer.Write("<b>Adding New Gallery:<br/><br/></b>\n");
                }

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function Form_Validator(theForm)\n");
                writer.Write("{\n");
                writer.Write("submitonce(theForm);\n");
                writer.Write("return (true);\n");
                writer.Write("}\n");

                if (!Editing)
                {
                    writer.Write("function MakeSafeName(theForm)\n");
                    writer.Write("{\n");
                    writer.Write("var cn = document.Form1.Name.value;\n");
                    writer.Write("var cn2 = stripCharsNotInBag(cn,\"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_1234567890\");\n");
                    writer.Write("document.Form1.DirName.value = cn2;\n");
                    writer.Write("document.Form1.DirName.focus();\n");
                    writer.Write("return (true);\n");
                    writer.Write("}\n");
                }
                writer.Write("</script>\n");

                if (AppLogic.NumLocaleSettingsInstalled() > 1)
                {
                    writer.Write(CommonLogic.ReadFile("jscripts/tabs.js", true));
                }

                writer.Write("<p>Please enter the following information about this gallery. Fields marked with an asterisk (*) are required. All other fields are optional.</p>\n");
                writer.Write("<form enctype=\"multipart/form-data\" action=\"editgallery.aspx?GalleryID=" + GalleryID.ToString() + "&edit=" + Editing.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(document.forms[0]) && Form_Validator(document.forms[0]))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"100%\" colspan=\"2\" align=\"left\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Gallery Name:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Name"), "Name", false, true, true, "Please enter the gallery name", 100, 30, 0, 0, false));
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Gallery Directory:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                if (!Editing)
                {
                    writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"DirName\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "DirName")), "") + "\">\n");
                    writer.Write("                	<input type=\"hidden\" name=\"DirName_vldt\" value=\"[req][blankalert=Please enter the gallery directory. This MUST be a valid Windows directory name, a-z, A-Z, 9-0, and _ characters ONLY!]\">\n");
                }
                else
                {
                    writer.Write(DB.RSField(rs, "DirName"));
                }
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("    <td valign=\"top\" align=\"right\">Icon:\n");
                writer.Write("</td>\n");
                writer.Write("    <td valign=\"top\" align=\"left\">");
                writer.Write("    <input type=\"file\" name=\"Image1\" size=\"30\" value=\"" + CommonLogic.IIF(Editing, "", "") + "\">\n");
                String Image1URL = AppLogic.LookupImage("Gallery", GalleryID, "icon", SkinID, ThisCustomer.LocaleSetting);
                if (Image1URL.Length != 0)
                {
                    if (Image1URL.IndexOf("nopicture") == -1)
                    {
                        writer.Write("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image1URL + "','GalleryPic');\">Click here</a> to delete the current image<br/>\n");
                    }
                    writer.Write("<br/><img id=\"GalleryPic\" name=\"GalleryPic\" border=\"0\" src=\"" + Image1URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\">\n");
                }
                writer.Write("</td>\n");
                writer.Write(" </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Description:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Description"), "Description", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true));
                //        writer.Write("                	<textarea style=\"height: 60em; width: 100%;\" cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeightl") + "\" id=\"Description\" name=\"Description\">" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Description")) , "") + "</textarea>\n");
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
