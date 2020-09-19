// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/productimagemgr.aspx.cs 7     9/30/06 3:38p Redwoodtree $
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
    /// Summary description for productimagemgr
    /// </summary>
    public partial class productimagemgr : AspDotNetStorefront.SkinBase
    {

        int ProductID;
        int VariantID;
        String TheSize;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            ProductID = CommonLogic.QueryStringUSInt("ProductID");
            VariantID = CommonLogic.QueryStringUSInt("VariantID");
            TheSize = CommonLogic.QueryStringCanBeDangerousContent("Size");
            if (TheSize.Length == 0)
            {
                TheSize = "medium";
            }
            if (VariantID == 0)
            {
                VariantID = AppLogic.GetDefaultProductVariant(ProductID);
            }

            if (CommonLogic.FormBool("IsSubmit"))
            {
                String FN = ProductID.ToString();
                if (AppLogic.AppConfigBool("UseSKUForProductImageName"))
                {
                    IDataReader rs = DB.GetRS("select SKU from product  " + DB.GetNoLock() + " where productid=" + ProductID.ToString());
                    if (rs.Read())
                    {
                        String SKU = DB.RSField(rs, "SKU").Trim();
                        if (SKU.Length != 0)
                        {
                            FN = SKU;
                        }
                    }
                    rs.Close();
                }
                try
                {
                    for (int i = 0; i <= Request.Form.Count - 1; i++)
                    {
                        String FieldName = Request.Form.Keys[i];
                        if (FieldName.IndexOf("Key_") != -1)
                        {
                            String KeyVal = CommonLogic.FormCanBeDangerousContent(FieldName);
                            // this field should be processed
                            String[] KeyValSplit = KeyVal.Split('|');
                            int TheFieldID = Localization.ParseUSInt(KeyValSplit[0]);
                            int TheProductID = Localization.ParseUSInt(KeyValSplit[1]);
                            int TheVariantID = Localization.ParseUSInt(KeyValSplit[2]);
                            String ImageNumber = AppLogic.CleanSizeColorOption(KeyValSplit[3]);
                            String Color = AppLogic.CleanSizeColorOption(HttpContext.Current.Server.UrlDecode(KeyValSplit[4]));
                            String SafeColor = CommonLogic.MakeSafeFilesystemName(Color);
                            bool DeleteIt = (CommonLogic.FormCanBeDangerousContent("Delete_" + TheFieldID.ToString()).Length != 0);
                            if (DeleteIt)
                            {
                                System.IO.File.Delete(AppLogic.GetImagePath("Product", TheSize, true) + FN + "_" + ImageNumber.ToLowerInvariant() + "_" + SafeColor + ".jpg");
                                System.IO.File.Delete(AppLogic.GetImagePath("Product", TheSize, true) + FN + "_" + ImageNumber.ToLowerInvariant() + "_" + SafeColor + ".gif");
                                System.IO.File.Delete(AppLogic.GetImagePath("Product", TheSize, true) + FN + "_" + ImageNumber.ToLowerInvariant() + "_" + SafeColor + ".png");
                            }

                            String Image2 = String.Empty;
                            HttpPostedFile Image2File = Request.Files["Image" + TheFieldID.ToString()];
                            if (Image2File.ContentLength != 0)
                            {
                                // delete any current image file first
                                try
                                {
                                    System.IO.File.Delete(AppLogic.GetImagePath("Product", TheSize, true) + FN + "_" + ImageNumber.ToLowerInvariant() + "_" + SafeColor + ".jpg");
                                    System.IO.File.Delete(AppLogic.GetImagePath("Product", TheSize, true) + FN + "_" + ImageNumber.ToLowerInvariant() + "_" + SafeColor + ".gif");
                                    System.IO.File.Delete(AppLogic.GetImagePath("Product", TheSize, true) + FN + "_" + ImageNumber.ToLowerInvariant() + "_" + SafeColor + ".png");
                                }
                                catch
                                { }

                                String s = Image2File.ContentType;
                                switch (Image2File.ContentType)
                                {
                                    case "image/gif":
                                        Image2 = AppLogic.GetImagePath("Product", TheSize, true) + FN + "_" + ImageNumber.ToLowerInvariant() + "_" + SafeColor + ".gif";
                                        Image2File.SaveAs(Image2);
                                        break;
                                    case "image/x-png":
                                        Image2 = AppLogic.GetImagePath("Product", TheSize, true) + FN + "_" + ImageNumber.ToLowerInvariant() + "_" + SafeColor + ".png";
                                        Image2File.SaveAs(Image2);
                                        break;
                                    case "image/jpg":
                                    case "image/jpeg":
                                    case "image/pjpeg":
                                        Image2 = AppLogic.GetImagePath("Product", TheSize, true) + FN + "_" + ImageNumber.ToLowerInvariant() + "_" + SafeColor + ".jpg";
                                        Image2File.SaveAs(Image2);
                                        break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMsg += CommonLogic.GetExceptionDetail(ex, "<br/>");
                }
            }
            SectionTitle = "<a href=\"editproduct.aspx?productid=" + ProductID.ToString() + "\">Products</a> - Multiple Image Manager, Size=" + TheSize;
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {

            if (ErrorMsg.Length != 0)
            {
                writer.Write("<p align=\"left\"><b><font color=red>" + ErrorMsg + "</font></b></p>\n");
            }
            if (DataUpdated)
            {
                writer.Write("<p align=\"left\"><b><font color=blue>(UPDATED)</font></b></p>\n");
            }

            IDataReader rs = DB.GetRS("select * from productvariant  " + DB.GetNoLock() + " where VariantID=" + VariantID.ToString());
            if (!rs.Read())
            {
                rs.Close();
                Response.Redirect("splash.aspx"); // should not happen, but...
            }

            String ProductName = AppLogic.GetProductName(ProductID, ThisCustomer.LocaleSetting);
            String ProductSKU = AppLogic.GetProductSKU(ProductID);
            String VariantName = AppLogic.GetVariantName(VariantID, ThisCustomer.LocaleSetting);
            String VariantSKU = AppLogic.GetVariantSKUSuffix(VariantID);

            String ImageNumbers = "1,2,3,4,5,6,7,8,9,10";
            String Colors = "," + DB.RSFieldByLocale(rs, "Colors", Localization.GetWebConfigLocale()); // add an "empty" color to the first entry, to allow an image to be specified for "no color selected"
            String[] ColorsSplit = Colors.Split(',');
            String[] ImageNumbersSplit = ImageNumbers.Split(',');

            writer.Write("<p align=\"left\"><b>PRODUCT: <a href=\"editproduct.aspx?productid=" + ProductID.ToString() + "\">" + ProductName + " (ProductID=" + ProductID.ToString() + ")</a></b></p>");
            writer.Write("<p align=\"left\">Manage (" + TheSize + ") images for this product by image # and color. You can have up to 10 images for a product, and an image for each color, so this forms a 2 dimensional grid if images: image number x color. Each slot can have a separate picture. You should also load the " + TheSize + " image pic on the editproduct page...that image is used by default for most page displays. These images are only used on the product page, when the user actively selects a different image number icon and/or color selection.</p>\n");

            writer.Write("<script type=\"text/javascript\">\n");
            writer.Write("function MultiImageForm_Validator(theForm)\n");
            writer.Write("{\n");
            writer.Write("submitonce(theForm);\n");
            writer.Write("return (true);\n");
            writer.Write("}\n");
            writer.Write("</script>\n");

            writer.Write("<div align=\"left\">");
            writer.Write("<form enctype=\"multipart/form-data\" action=\"productimagemgr.aspx?size=" + TheSize + "&productid=" + ProductID.ToString() + "&VariantID=" + VariantID.ToString() + "\" method=\"post\" id=\"MultiImageForm\" name=\"MultiImageForm\" onsubmit=\"return (validateForm(this) && MultiImageForm_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
            writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");

            writer.Write("<table border=\"0\" cellspacing=\"4\" cellpadding=\"4\" border=\"1\">\n");
            writer.Write("<tr>\n");
            writer.Write("<td valign=\"middle\" align=\"right\"><b>Color\\Image#</b></td>\n");
            for (int i = ImageNumbersSplit.GetLowerBound(0); i <= ImageNumbersSplit.GetUpperBound(0); i++)
            {
                writer.Write("<td valign=\"middle\" align=\"center\"><b>" + AppLogic.CleanSizeColorOption(ImageNumbersSplit[i]) + "</b></td>\n");
            }
            writer.Write("</tr>\n");
            int FormFieldID = 1000; // arbitrary number
            bool first = true;
            for (int i = ColorsSplit.GetLowerBound(0); i <= ColorsSplit.GetUpperBound(0); i++)
            {
                if (ColorsSplit[i].Length == 0 && !first)
                {
                    continue;
                }
                writer.Write("<tr>\n");
                writer.Write("<td valign=\"middle\" align=\"right\"><b>" + CommonLogic.IIF(ColorsSplit[i].Length == 0, "(No Color Selected)", AppLogic.CleanSizeColorOption(ColorsSplit[i])) + "</b></td>\n");
                for (int j = ImageNumbersSplit.GetLowerBound(0); j <= ImageNumbersSplit.GetUpperBound(0); j++)
                {
                    writer.Write("<td valign=\"bottom\" align=\"center\" bgcolor=\"#EEEEEE\">");
                    int ImgWidth = AppLogic.AppConfigNativeInt("Admin.MultiGalleryImageWidth");
                    writer.Write("<img " + CommonLogic.IIF(ImgWidth != 0, "width=\"" + ImgWidth.ToString() + "\"", "") + " src=\"" + AppLogic.LookupProductImageByNumberAndColor(ProductID, SkinID, ThisCustomer.LocaleSetting, Localization.ParseUSInt(ImageNumbersSplit[j]), ColorsSplit[i], TheSize) + "\"><br/>");
                    writer.Write("<input style=\"font-size: 9px;\" type=\"file\" name=\"Image" + FormFieldID.ToString() + "\" size=\"24\" value=\"\"><br/>\n");
                    writer.Write("<input type=\"checkbox\" name=\"Delete_" + FormFieldID.ToString() + "\"> delete");
                    String sColorValue = HttpContext.Current.Server.UrlEncode(AppLogic.CleanSizeColorOption(ColorsSplit[i]));
                    writer.Write("<input type=\"hidden\" name=\"Key_" + FormFieldID.ToString() + "\" value=\"" + FormFieldID.ToString() + "|" + ProductID.ToString() + "|" + VariantID.ToString() + "|" + AppLogic.CleanSizeColorOption(ImageNumbersSplit[j]) + "|" + sColorValue + "\">");
                    FormFieldID++;
                    writer.Write("</td>\n");
                }
                writer.Write("</tr>\n");
                first = false;
            }

            writer.Write("</table>\n");
            writer.Write("<p align=\"left\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"submit\" value=\"Update\" name=\"submit\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"reset\" value=\"Reset\" name=\"reset\"></p>\n");
            writer.Write("</form>\n");
            writer.Write("</div>");
            rs.Close();
        }

    }
}
