// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/images.aspx.cs 3     10/04/06 12:00p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.IO;
using System.Collections;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for images.
    /// </summary>
    public partial class images : AspDotNetStorefront.SkinBase
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            SectionTitle = "Manage Images";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            String SFP = CommonLogic.SafeMapPath("../images/spacer.gif").Replace("images\\spacer.gif", "images\\upload");

            if (CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
            {
                // delete the image:
                System.IO.File.Delete(SFP + "/" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID"));
            }

            if (CommonLogic.FormCanBeDangerousContent("IsSubmit") == "true")
            {
                // handle upload if any also:
                HttpPostedFile Image1File = Request.Files["Image1"];
                if (Image1File.ContentLength != 0)
                {
                    String tmp = Image1File.FileName.ToLowerInvariant();
                    if (tmp.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) || tmp.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase) || tmp.EndsWith(".gif", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (tmp.LastIndexOf('\\') != -1)
                        {
                            tmp = tmp.Substring(tmp.LastIndexOf('\\') + 1);
                        }
                        String fn = SFP + "/" + tmp;
                        Image1File.SaveAs(fn);
                    }
                }
            }


            writer.Write("<form enctype=\"multipart/form-data\" id=\"Form1\" name=\"Form1\" method=\"POST\" action=\"images.aspx\">\n");
            writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
            writer.Write("  <table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
            writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
            writer.Write("      <td><b>File Name</b></td>\n");
            writer.Write("      <td><b>Img Tag Src</b></td>\n");
            writer.Write("      <td><b>Dimensions</b></td>\n");
            writer.Write("      <td><b>Size (KB)</b></td>\n");
            writer.Write("      <td><b>Image</b></td>\n");
            writer.Write("      <td align=\"center\"><b>Delete</b></td>\n");
            writer.Write("    </tr>\n");

            // create an array to hold the list of files
            ArrayList fArray = new ArrayList();

            // get information about our initial directory
            DirectoryInfo dirInfo = new DirectoryInfo(SFP);

            // retrieve array of files & subdirectories
            FileSystemInfo[] myDir = dirInfo.GetFileSystemInfos();

            for (int i = 0; i < myDir.Length; i++)
            {
                // check the file attributes

                // if a subdirectory, add it to the sArray    
                // otherwise, add it to the fArray
                if (((Convert.ToUInt32(myDir[i].Attributes) & Convert.ToUInt32(FileAttributes.Directory)) > 0))
                {
                    //sArray.Add(Path.GetFileName(myDir[i].FullName));  
                }
                else
                {
                    bool skipit = false;
                    if (myDir[i].FullName.StartsWith("_") || (!myDir[i].FullName.EndsWith("jpg", StringComparison.InvariantCultureIgnoreCase) && !myDir[i].FullName.EndsWith("gif", StringComparison.InvariantCultureIgnoreCase) && !myDir[i].FullName.EndsWith("png", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        skipit = true;
                    }
                    if (!skipit)
                    {
                        fArray.Add(Path.GetFileName(myDir[i].FullName));
                    }
                }
            }

            if (fArray.Count != 0)
            {
                // sort the files alphabetically
                fArray.Sort(0, fArray.Count, null);
                for (int i = 0; i < fArray.Count; i++)
                {
                    String src = "../images/upload/" + fArray[i].ToString();
                    System.Drawing.Size size = CommonLogic.GetImagePixelSize(src);
                    long s = CommonLogic.GetImageSize(src);
                    writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                    writer.Write("      <td >" + fArray[i].ToString() + "</td>\n");
                    writer.Write("      <td >../images/upload/" + fArray[i].ToString() + "</td>\n");
                    writer.Write("      <td >" + size.Width.ToString() + "x" + size.Height.ToString() + "</td>\n");
                    writer.Write("      <td >" + (s / 1000).ToString() + " KB</td>\n");
                    writer.Write("<td><a target=\"_blank\" href=\"" + src + "\">\n");
                    writer.Write("<img border=\"0\" src=\"" + src + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\"" + CommonLogic.IIF(size.Height > 50, " height=\"50\"", "") + ">\n");
                    writer.Write("</a></td>\n");
                    writer.Write("      <td align=\"center\"><input type=\"button\" value=\"Delete\" name=\"Delete_" + i.ToString() + "\" onClick=\"DeleteImage(" + CommonLogic.SQuote(fArray[i].ToString()) + ")\"></td>\n");
                    writer.Write("    </tr>\n");
                }
            }

            writer.Write("    <tr>\n");
            writer.Write("      <td colspan=\"6\" height=5></td>\n");
            writer.Write("    </tr>\n");
            writer.Write("  </table>\n");
            writer.Write("<p align=\"left\">Upload A New Image: <input type=\"file\" name=\"Image1\" size=\"50\"><br/><input type=\"submit\" value=\"Submit\" name=\"submit\"></p>\n");
            writer.Write("</form>\n");

            writer.Write("</center></b>\n");

            writer.Write("<script type=\"text/javascript\">\n");
            writer.Write("function DeleteImage(name)\n");
            writer.Write("{\n");
            writer.Write("if(confirm('Are you sure you want to delete image: ' + name))\n");
            writer.Write("{\n");
            writer.Write("self.location = 'images.aspx?deleteid=' + name;\n");
            writer.Write("}\n");
            writer.Write("}\n");
            writer.Write("</SCRIPT>\n");
        }

    }
}
