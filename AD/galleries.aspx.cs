// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/galleries.aspx.cs 2     9/30/06 3:38p Redwoodtree $
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
    /// Summary description for galleries.
    /// </summary>
    public partial class galleries : AspDotNetStorefront.SkinBase
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            SectionTitle = "Manage Galleries";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            if (CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
            {
                // delete any images:
                try
                {
                    System.IO.File.Delete(AppLogic.GetImagePath("Gallery", "icon", true) + CommonLogic.QueryStringUSInt("DeleteID").ToString() + ".jpg");
                    System.IO.File.Delete(AppLogic.GetImagePath("Gallery", "icon", true) + CommonLogic.QueryStringUSInt("DeleteID").ToString() + ".png");
                    System.IO.File.Delete(AppLogic.GetImagePath("Gallery", "icon", true) + CommonLogic.QueryStringUSInt("DeleteID").ToString() + ".gif");
                }
                catch { }

                // delete the gallery directory also!
                String GalleryDirName = AppLogic.GetGalleryDir(CommonLogic.QueryStringUSInt("DeleteID"));
                String SFP = CommonLogic.SafeMapPath("../images/spacer.gif").Replace("images\\spacer.gif", "images\\gallery") + "\\" + GalleryDirName;
                try
                {
                    if (Directory.Exists(SFP))
                    {
                        String[] files = Directory.GetFiles(SFP, "*.*");
                        foreach (String file in files)
                        {
                            System.IO.File.Delete(file);
                        }
                        Directory.Delete(SFP);
                    }
                }
                catch { }

                // delete the gallery:
                DB.ExecuteSQL("delete from gallery where GalleryID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID"));
            }

            if (CommonLogic.FormBool("IsSubmit"))
            {
                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    if (Request.Form.Keys[i].IndexOf("DisplayOrder_") != -1)
                    {
                        String[] keys = Request.Form.Keys[i].Split('_');
                        int GalleryID = Localization.ParseUSInt(keys[1]);
                        int DispOrd = 1;
                        try
                        {
                            DispOrd = Localization.ParseUSInt(Request.Form[Request.Form.Keys[i]]);
                        }
                        catch { }
                        DB.ExecuteSQL("update gallery set DisplayOrder=" + DispOrd.ToString() + " where GalleryID=" + GalleryID.ToString());
                    }
                }
            }

            DataSet ds = DB.GetDS("select * from gallery  " + DB.GetNoLock() + " where deleted=0 order by DisplayOrder,Name", false);
            writer.Write("<form method=\"POST\" action=\"galleries.aspx\">\n");
            writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
            writer.Write("  <table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
            writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
            writer.Write("      <td ><b>ID</b></td>\n");
            writer.Write("      <td ><b>Gallery Name</b></td>\n");
            writer.Write("      <td ><b>Gallery URL</b></td>\n");
            writer.Write("      <td align=\"center\"><b>Display Order</b></td>\n");
            writer.Write("      <td align=\"center\"><b>Edit</b></td>\n");
            writer.Write("      <td align=\"center\"><b>Manage Images</b></td>\n");
            writer.Write("      <td align=\"center\"><b>Delete</b></td>\n");
            writer.Write("    </tr>\n");
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                writer.Write("      <td>" + DB.RowFieldInt(row, "GalleryID").ToString() + "</td>\n");
                writer.Write("      <td >");
                String Image1URL = AppLogic.LookupImage("Gallery", DB.RowFieldInt(row, "GalleryID"), "icon", SkinID, ThisCustomer.LocaleSetting);
                if (Image1URL.Length != 0)
                {
                    writer.Write("<a href=\"editGallery.aspx?Galleryid=" + DB.RowFieldInt(row, "GalleryID").ToString() + "\">");
                    writer.Write("<img src=\"" + Image1URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\" height=\"35\" border=\"0\" align=\"absmiddle\">");
                    writer.Write("</a>&nbsp;\n");
                }
                writer.Write("      <a href=\"editGallery.aspx?Galleryid=" + DB.RowFieldInt(row, "GalleryID").ToString() + "\">");
                writer.Write(DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting));
                writer.Write("</a>");
                writer.Write("</td>\n");
                writer.Write("      <td><a target=\"_blank\" href=\"../showgallery.aspx?galleryid=" + DB.RowFieldInt(row, "GalleryID").ToString() + "\">showgallery.aspx?galleryid=" + DB.RowFieldInt(row, "GalleryID").ToString() + "</a></td>\n");
                writer.Write("      <td align=\"center\"><input size=2 type=\"text\" name=\"DisplayOrder_" + DB.RowFieldInt(row, "GalleryID").ToString() + "\" value=\"" + DB.RowFieldInt(row, "DisplayOrder").ToString() + "\"></td>\n");
                writer.Write("      <td align=\"center\"><input type=\"button\" value=\"Edit\" name=\"Edit_" + DB.RowFieldInt(row, "GalleryID").ToString() + "\" onClick=\"self.location='editgallery.aspx?galleryid=" + DB.RowFieldInt(row, "GalleryID").ToString() + "'\"></td>\n");
                writer.Write("      <td align=\"center\"><input type=\"button\" value=\"Add/Delete Gallery Images\" name=\"ManageImages_" + DB.RowFieldInt(row, "GalleryID").ToString() + "\" onClick=\"self.location='galleryimages.aspx?galleryid=" + DB.RowFieldInt(row, "GalleryID").ToString() + "'\"></td>\n");
                writer.Write("      <td align=\"center\"><input type=\"button\" value=\"Delete\" name=\"Delete_" + DB.RowFieldInt(row, "GalleryID").ToString() + "\" onClick=\"DeleteGallery(" + DB.RowFieldInt(row, "GalleryID").ToString() + ")\"></td>\n");
                writer.Write("    </tr>\n");
            }
            ds.Dispose();
            writer.Write("    <tr>\n");
            writer.Write("      <td colspan=\"4\" align=\"left\"></td>\n");
            writer.Write("      <td align=\"center\" bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></td>\n");
            writer.Write("      <td colspan=\"2\"></td>\n");
            writer.Write("    </tr>\n");
            writer.Write("    <tr>\n");
            writer.Write("      <td colspan=\"7\" height=5></td>\n");
            writer.Write("    </tr>\n");
            writer.Write("  </table>\n");
            writer.Write("<p align=\"left\"><input type=\"button\" value=\"Add New Gallery\" name=\"AddNew\" onClick=\"self.location='editgallery.aspx';\"><p/>");
            writer.Write("</form>\n");

            writer.Write("<script type=\"text/javascript\">\n");
            writer.Write("function DeleteGallery(id)\n");
            writer.Write("{\n");
            writer.Write("if(confirm('Are you sure you want to delete gallery: ' + id))\n");
            writer.Write("{\n");
            writer.Write("self.location = 'galleries.aspx?deleteid=' + id;\n");
            writer.Write("}\n");
            writer.Write("}\n");
            writer.Write("</SCRIPT>\n");
        }

    }
}
