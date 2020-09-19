// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editskinpreview.aspx.cs 9     9/30/06 3:38p Redwoodtree $
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
    /// Summary description for editskinpreview
    /// </summary>
    public partial class editskinpreview : AspDotNetStorefront.SkinBase
    {

        String SearchFor;
        String GroupName;
        String BeginsWith;
        int SkinPreviewID;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            SearchFor = CommonLogic.QueryStringCanBeDangerousContent("SearchFor");
            GroupName = CommonLogic.QueryStringCanBeDangerousContent("GroupName");
            BeginsWith = CommonLogic.QueryStringCanBeDangerousContent("BeginsWith");
            SkinPreviewID = 0;

            if (CommonLogic.QueryStringCanBeDangerousContent("SkinPreviewID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("SkinPreviewID") != "0")
            {
                Editing = true;
                SkinPreviewID = Localization.ParseUSInt(CommonLogic.QueryStringCanBeDangerousContent("SkinPreviewID"));
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
                    sql.Append("insert into SkinPreview(SkinPreviewGUID,Name,GroupName,SkinID) values(");
                    sql.Append(DB.SQuote(NewGUID) + ",");
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("GroupName")) + ",");
                    sql.Append(CommonLogic.FormUSInt("SkinID").ToString());
                    sql.Append(")");
                    DB.ExecuteSQL(sql.ToString());

                    rs = DB.GetRS("select SkinPreviewID from SkinPreview  " + DB.GetNoLock() + " where SkinPreviewGUID=" + DB.SQuote(NewGUID));
                    rs.Read();
                    SkinPreviewID = DB.RSFieldInt(rs, "SkinPreviewID");
                    Editing = true;
                    rs.Close();
                    DataUpdated = true;
                }
                else
                {
                    // ok to update:
                    sql.Append("update SkinPreview set ");
                    sql.Append("Name=" + DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                    sql.Append("GroupName=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("GroupName")) + ",");
                    sql.Append("SkinID=" + CommonLogic.FormUSInt("SkinID").ToString());
                    sql.Append(" where SkinPreviewID=" + SkinPreviewID.ToString());
                    DB.ExecuteSQL(sql.ToString());
                    DataUpdated = true;
                    Editing = true;
                }

                // handle image uploaded:
                String FN = SkinPreviewID.ToString();
                try
                {
                    String Image1 = String.Empty;
                    HttpPostedFile Image1File = Request.Files["Image1"];
                    if (Image1File.ContentLength != 0)
                    {
                        // delete any current image file first
                        try
                        {
                            System.IO.File.Delete(AppLogic.GetImagePath("SkinPreviews", "icon", true) + FN + ".jpg");
                            System.IO.File.Delete(AppLogic.GetImagePath("SkinPreviews", "icon", true) + FN + ".gif");
                            System.IO.File.Delete(AppLogic.GetImagePath("SkinPreviews", "icon", true) + FN + ".png");
                        }
                        catch
                        { }

                        String s = Image1File.ContentType;
                        switch (Image1File.ContentType)
                        {
                            case "image/gif":
                                Image1 = AppLogic.GetImagePath("SkinPreviews", "icon", true) + FN + ".gif";
                                Image1File.SaveAs(Image1);
                                break;
                            case "image/x-png":
                                Image1 = AppLogic.GetImagePath("SkinPreviews", "icon", true) + FN + ".png";
                                Image1File.SaveAs(Image1);
                                break;
                            case "image/jpg":
                            case "image/jpeg":
                            case "image/pjpeg":
                                Image1 = AppLogic.GetImagePath("SkinPreviews", "icon", true) + FN + ".jpg";
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
                            System.IO.File.Delete(AppLogic.GetImagePath("SkinPreviews", "medium", true) + FN + ".jpg");
                            System.IO.File.Delete(AppLogic.GetImagePath("SkinPreviews", "medium", true) + FN + ".gif");
                            System.IO.File.Delete(AppLogic.GetImagePath("SkinPreviews", "medium", true) + FN + ".png");
                        }
                        catch
                        { }

                        String s = Image2File.ContentType;
                        switch (Image2File.ContentType)
                        {
                            case "image/gif":
                                Image2 = AppLogic.GetImagePath("SkinPreviews", "medium", true) + FN + ".gif";
                                Image2File.SaveAs(Image2);
                                break;
                            case "image/x-png":
                                Image2 = AppLogic.GetImagePath("SkinPreviews", "medium", true) + FN + ".png";
                                Image2File.SaveAs(Image2);
                                break;
                            case "image/jpg":
                            case "image/jpeg":
                            case "image/pjpeg":
                                Image2 = AppLogic.GetImagePath("SkinPreviews", "medium", true) + FN + ".jpg";
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
            SectionTitle = "<a href=\"skinpreview.aspx?GroupName=" + Server.UrlEncode(GroupName) + "&beginsWith=" + Server.UrlEncode(BeginsWith) + "&searchfor=" + Server.UrlEncode(SearchFor) + "\">Skin Previews</a> - Add/Edit Skin Preview";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            IDataReader rs = DB.GetRS("select * from SkinPreview  " + DB.GetNoLock() + " where SkinPreviewID=" + SkinPreviewID.ToString());
            if (rs.Read())
            {
                Editing = true;
            }

            if (AppLogic.NumLocaleSettingsInstalled() > 1)
            {
                writer.Write(CommonLogic.ReadFile("jscripts/tabs.js", true));
            }

            if (ErrorMsg.Length != 0)
            {
                writer.Write("<p align=\"left\"><b><font color=red>" + ErrorMsg + "</font></b></p>\n");
            }
            if (DataUpdated)
            {
                writer.Write("<p align=\"left\"><b><font color=blue>(UPDATED)</font></b></p>\n");
            }


            if (ErrorMsg.Length == 0)
            {

                if (Editing)
                {
                    writer.Write("<p align=\"left\"><b>Editing Skin Preview: " + DB.RSField(rs, "Name") + " (SkinPreviewID=" + DB.RSFieldInt(rs, "SkinPreviewID").ToString() + ", SkinID=" + DB.RSFieldInt(rs, "SkinID").ToString() + ")</b></p>\n");
                }
                else
                {
                    writer.Write("<p align=\"left\"><b>Adding New Skin Preview:</b></p>\n");
                }

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function Form_Validator(theForm)\n");
                writer.Write("{\n");
                writer.Write("submitonce(theForm);\n");
                writer.Write("return (true);\n");
                writer.Write("}\n");
                writer.Write("</script>\n");

                writer.Write("<p>Please enter the following information about this Skin Preview. Fields marked with an asterisk (*) are required. All other fields are optional.</p>\n");
                writer.Write("<form enctype=\"multipart/form-data\" action=\"editSkinPreview.aspx?GroupName=" + Server.UrlEncode(GroupName) + "&beginsWith=" + Server.UrlEncode(BeginsWith) + "&searchfor=" + Server.UrlEncode(SearchFor) + "&SkinPreviewID=" + SkinPreviewID.ToString() + "&edit=" + Editing.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(this) && Form_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"100%\" colspan=\"2\" align=\"left\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Skin ID:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"10\" size=\"10\" name=\"SkinID\" value=\"" + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldInt(rs, "SkinID") != 0, DB.RSFieldInt(rs, "SkinID").ToString(), ""), "") + "\"> (specify the skin id this preview is associated with, e.g. 1)\n");
                writer.Write("                	<input type=\"hidden\" name=\"SkinID_vldt\" value=\"[req][number][invalidalert=Please enter a valid integer number, e.g. 250!]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Skin Preview Name:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                //				String PName = String.Empty;
                //				if(Editing)
                //				{
                //					PName = Server.HtmlEncode(DB.RSField(rs,"Name"));
                //				}
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Name"), "Name", false, true, true, "Please enter the skin preview name", 100, 30, 0, 0, false));
                //        writer.Write("                	<input maxLength=\"100\" size=\"50\" name=\"Name\" value=\"" + CommonLogic.IIF(Editing , PName , "") + "\">\n");
                //				writer.Write("                	<input type=\"hidden\" name=\"Name_vldt\" value=\"[req][blankalert=Please enter the product name]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Group Name:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"100\" size=\"50\" name=\"GroupName\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "GroupName")), "") + "\">\n");
                writer.Write("                	<input type=\"hidden\" name=\"GroupName_vldt\" value=\"[req][blankalert=Please enter the skin preview group name, helpful to group your skins by category, or whatever you want to group them by]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("  <tr>\n");
                writer.Write("    <td valign=\"top\" align=\"right\">Icon:\n");
                writer.Write("</td>\n");
                writer.Write("    <td valign=\"top\" align=\"left\">");
                writer.Write("    <input type=\"file\" name=\"Image1\" size=\"50\" value=\"" + CommonLogic.IIF(Editing, "", "") + "\">\n");
                String Image1URL = AppLogic.LookupImage("SkinPreviews", SkinPreviewID, "icon", SkinID, ThisCustomer.LocaleSetting);
                if (Image1URL.Length != 0)
                {
                    if (Image1URL.IndexOf("nopicture") == -1)
                    {
                        writer.Write("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image1URL + "','Pic1');\">Click here</a> to delete the current image<br/>\n");
                    }
                    writer.Write("<br/><img id=\"Pic1\" name=\"Pic1\" border=\"0\" src=\"" + Image1URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\">\n");
                }
                writer.Write("</td>\n");
                writer.Write(" </tr>\n");

                writer.Write("  <tr>\n");
                writer.Write("    <td valign=\"top\" align=\"right\">Medium Pic:\n");
                writer.Write("</td>\n");
                writer.Write("    <td valign=\"top\" align=\"left\">");
                writer.Write("    <input type=\"file\" name=\"Image2\" size=\"50\" value=\"" + CommonLogic.IIF(Editing, "", "") + "\">\n");
                String Image2URL = AppLogic.LookupImage("SkinPreviews", SkinPreviewID, "medium", SkinID, ThisCustomer.LocaleSetting);
                if (Image2URL.Length != 0)
                {
                    if (Image2URL.IndexOf("nopicture") == -1)
                    {
                        writer.Write("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image2URL + "','Pic2');\">Click here</a> to delete the current image<br/>\n");
                    }
                    writer.Write("<br/><img id=\"Pic2\" name=\"Pic2\" border=\"0\" src=\"" + Image2URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\">\n");
                }
                writer.Write("</td>\n");
                writer.Write(" </tr>\n");

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

                writer.Write("<script type=\"text/javascript\">\n");

                writer.Write("function DeleteImage(imgurl,name)\n");
                writer.Write("{\n");
                writer.Write("window.open('deleteimage.aspx?imgurl=' + imgurl + '&FormImageName=' + name,\"Admin_ML\",\"height=250,width=440,top=10,left=20,status=no,toolbar=no,menubar=no,scrollbars=yes,location=no\")\n");
                writer.Write("}\n");

                writer.Write("</SCRIPT>\n");

            }
            rs.Close();
        }

    }
}
