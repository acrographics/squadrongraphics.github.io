// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/galleryimages.aspx.cs 7     10/04/06 6:23a Redwoodtree $
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
    /// Summary description for galleryimages.
    /// </summary>
    public partial class galleryimages : System.Web.UI.Page
    {
        private int GalleryID;
        private String GalleryName;
        private String Dir;
        private String SFP;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
            
            GalleryID = CommonLogic.QueryStringUSInt("GalleryID");
            GalleryName = AppLogic.GetGalleryName(GalleryID, Localization.GetWebConfigLocale());
            Dir = AppLogic.GetGalleryDir(GalleryID); //CommonLogic.QueryStringCanBeDangerousContent("Gallery");
            if (Dir.StartsWith("/") && Dir.Length > 0)
            {
                Dir = Dir.Substring(1);
            }

            SFP = CommonLogic.SafeMapPath("../images/spacer.gif").Replace("images\\spacer.gif", "");
            if (!SFP.EndsWith("\\"))
            {
                SFP = SFP + "\\";
            }

            SFP = SFP + "images\\gallery\\" + Dir + "\\";

            if (CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
            {
                // delete the image:
                int DeletingSlideID = CommonLogic.QueryStringUSInt("DeleteID");
                int LastSlide = AppLogic.GetNumSlides(SFP);
                String fndel = SFP + "slide" + DeletingSlideID.ToString().PadLeft(2, '0') + ".jpg";
                String fndel_lg = SFP + "slide" + DeletingSlideID.ToString().PadLeft(2, '0') + "_lg.jpg";

                System.IO.File.Delete(fndel);
                System.IO.File.Delete(fndel_lg);

                // now must renumber all "higher slides"
                for (int i = DeletingSlideID + 1; i <= AppLogic.AppConfigUSInt("MaxSlides"); i++)
                {
                    String src = SFP + "slide" + i.ToString().PadLeft(2, '0') + ".jpg";
                    String src_lg = SFP + "slide" + i.ToString().PadLeft(2, '0') + "_lg.jpg";
                    String dest = SFP + "slide" + (i - 1).ToString().PadLeft(2, '0') + ".jpg";
                    String dest_lg = SFP + "slide" + (i - 1).ToString().PadLeft(2, '0') + "_lg.jpg";
                    if (System.IO.File.Exists(src))
                    {
                        try
                        {
                            System.IO.File.Move(src, dest);
                            System.IO.File.Move(src_lg, dest_lg);
                        }
                        catch { }
                    }
                }
                try
                {
                    String lastslide2 = SFP + "slide" + AppLogic.AppConfigUSInt("MaxSlides").ToString().PadLeft(2, '0') + ".jpg";
                    String lastslide_lg = SFP + "slide" + AppLogic.AppConfigUSInt("MaxSlides").ToString().PadLeft(2, '0') + "_lg.jpg";
                    if (System.IO.File.Exists(lastslide2))
                    {
                        System.IO.File.Delete(lastslide2);
                        System.IO.File.Delete(lastslide_lg);
                    }
                }
                catch { }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("MoveUpID").Length != 0)
            {
                // move the specified image up:
                int MoveUpID = CommonLogic.QueryStringUSInt("MoveUpID");
                int LastSlide = AppLogic.GetNumSlides(SFP);
                String srcup = SFP + "slide" + MoveUpID.ToString().PadLeft(2, '0') + ".jpg";
                String srcup_lg = SFP + "slide" + MoveUpID.ToString().PadLeft(2, '0') + "_lg.jpg";
                String destup = SFP + "slide" + (MoveUpID - 1).ToString().PadLeft(2, '0') + ".jpg";
                String destup_lg = SFP + "slide" + (MoveUpID - 1).ToString().PadLeft(2, '0') + "_lg.jpg";
                String tmpup = SFP + "slide00.jpg";
                String tmpup_lg = SFP + "slide00_lg.jpg";
                if (MoveUpID > 1)
                {
                    System.IO.File.Move(destup, tmpup);
                    System.IO.File.Move(destup_lg, tmpup_lg);
                    System.IO.File.Move(srcup, destup);
                    System.IO.File.Move(srcup_lg, destup_lg);
                    System.IO.File.Move(tmpup, srcup);
                    System.IO.File.Move(tmpup_lg, srcup_lg);

                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("MoveFirstID").Length != 0)
            {
                // move the specified image to first:
                int MoveFirstID = CommonLogic.QueryStringUSInt("MoveFirstID");
                int LastSlide = AppLogic.GetNumSlides(SFP);
                String srcup = SFP + "slide" + MoveFirstID.ToString().PadLeft(2, '0') + ".jpg";
                String srcup_lg = SFP + "slide" + MoveFirstID.ToString().PadLeft(2, '0') + "_lg.jpg";
                String destup = SFP + "slide01.jpg";
                String destup_lg = SFP + "slide01_lg.jpg";
                String tmpup = SFP + "slide00.jpg";
                String tmpup_lg = SFP + "slide00_lg.jpg";
                if (MoveFirstID > 1)
                {
                    System.IO.File.Move(srcup, tmpup);
                    System.IO.File.Move(srcup_lg, tmpup_lg);

                    // now must move "up" all "lower slides"
                    for (int i = MoveFirstID; i >= 2; i--)
                    {
                        String xsrc = SFP + "slide" + (i - 1).ToString().PadLeft(2, '0') + ".jpg";
                        String xsrc_lg = SFP + "slide" + (i - 1).ToString().PadLeft(2, '0') + "_lg.jpg";
                        String xdest = SFP + "slide" + i.ToString().PadLeft(2, '0') + ".jpg";
                        String xdest_lg = SFP + "slide" + i.ToString().PadLeft(2, '0') + "_lg.jpg";
                        System.IO.File.Delete(xdest);
                        System.IO.File.Delete(xdest_lg);
                        System.IO.File.Move(xsrc, xdest);
                        System.IO.File.Move(xsrc_lg, xdest_lg);
                    }
                    System.IO.File.Move(tmpup, destup);
                    System.IO.File.Move(tmpup_lg, destup_lg);

                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("MoveDownID").Length != 0)
            {
                // move the specified image down:
                int MoveDownID = CommonLogic.QueryStringUSInt("MoveDownID");
                int LastSlide = AppLogic.GetNumSlides(SFP);
                String srcup = SFP + "slide" + MoveDownID.ToString().PadLeft(2, '0') + ".jpg";
                String srcup_lg = SFP + "slide" + MoveDownID.ToString().PadLeft(2, '0') + "_lg.jpg";
                String destup = SFP + "slide" + (MoveDownID + 1).ToString().PadLeft(2, '0') + ".jpg";
                String destup_lg = SFP + "slide" + (MoveDownID + 1).ToString().PadLeft(2, '0') + "_lg.jpg";
                String tmpup = SFP + "slide00.jpg";
                String tmpup_lg = SFP + "slide00_lg.jpg";
                if (MoveDownID < LastSlide)
                {
                    System.IO.File.Move(destup, tmpup);
                    System.IO.File.Move(destup_lg, tmpup_lg);
                    System.IO.File.Move(srcup, destup);
                    System.IO.File.Move(srcup_lg, destup_lg);
                    System.IO.File.Move(tmpup, srcup);
                    System.IO.File.Move(tmpup_lg, srcup_lg);

                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("MoveLastID").Length != 0)
            {
                // move the specified image to Last:
                int MoveLastID = CommonLogic.QueryStringUSInt("MoveLastID");
                int LastSlide = AppLogic.GetNumSlides(SFP);
                String srcup = SFP + "slide" + MoveLastID.ToString().PadLeft(2, '0') + ".jpg";
                String srcup_lg = SFP + "slide" + MoveLastID.ToString().PadLeft(2, '0') + "_lg.jpg";
                String destup = SFP + "slide" + LastSlide.ToString().PadLeft(2, '0') + ".jpg";
                String destup_lg = SFP + "slide" + LastSlide.ToString().PadLeft(2, '0') + "_lg.jpg";
                String tmpup = SFP + "slide00.jpg";
                String tmpup_lg = SFP + "slide00_lg.jpg";
                if (MoveLastID < LastSlide)
                {
                    System.IO.File.Move(srcup, tmpup);
                    System.IO.File.Move(srcup_lg, tmpup_lg);

                    // now must move "down" all "higher slides"
                    for (int i = MoveLastID; i <= LastSlide - 1; i++)
                    {
                        String xsrc = SFP + "slide" + (i + 1).ToString().PadLeft(2, '0') + ".jpg";
                        String xsrc_lg = SFP + "slide" + (i + 1).ToString().PadLeft(2, '0') + "_lg.jpg";
                        String xdest = SFP + "slide" + i.ToString().PadLeft(2, '0') + ".jpg";
                        String xdest_lg = SFP + "slide" + i.ToString().PadLeft(2, '0') + "_lg.jpg";
                        System.IO.File.Delete(xdest);
                        System.IO.File.Delete(xdest_lg);
                        System.IO.File.Move(xsrc, xdest);
                        System.IO.File.Move(xsrc_lg, xdest_lg);
                    }
                    System.IO.File.Move(tmpup, destup);
                    System.IO.File.Move(tmpup_lg, destup_lg);

                }
            }

            LoadContent();
        }

        protected void LoadContent()
        {
            StringBuilder tmpS = new StringBuilder(4096);

            // create an array to hold the list of files
            ArrayList fArray = new ArrayList();

            // get information about our initial directory
            DirectoryInfo dirInfo = new DirectoryInfo(SFP);

            // retrieve array of files & subdirectories
            FileSystemInfo[] myDir = dirInfo.GetFileSystemInfos();

            for (int i = 0; i < myDir.Length; i++)
            {
                // check the file attributes
                if (myDir[i].FullName.ToUpperInvariant().EndsWith("jpg", StringComparison.InvariantCultureIgnoreCase) && myDir[i].FullName.ToUpperInvariant().IndexOf("_LG.JPG") == -1)
                {
                    fArray.Add(Path.GetFileName(myDir[i].FullName));
                }
            }
            // sort the files alphabetically
            fArray.Sort(0, fArray.Count, null);

            tmpS.Append("<input type=\"hidden\" name=\"NewSlideNumber\" value=\"" + (fArray.Count + 1).ToString() + "\">\n");
            tmpS.Append("  <table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
            tmpS.Append("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
            tmpS.Append("      <td><b>Slide ID</b></td>\n");
            tmpS.Append("      <td><b>File Name</b></td>\n");
            tmpS.Append("      <td><b>Img Tag Src</b></td>\n");
            tmpS.Append("      <td><b>Dimensions</b></td>\n");
            tmpS.Append("      <td><b>Size (KB)</b></td>\n");
            tmpS.Append("      <td align=\"center\"><b>Image</b></td>\n");
            tmpS.Append("      <td align=\"center\"><b>Move Up/Down</b></td>\n");
            tmpS.Append("      <td align=\"center\"><b>Delete</b></td>\n");
            tmpS.Append("    </tr>\n");

            if (fArray.Count != 0)
            {
                for (int i = 0; i < fArray.Count; i++)
                {
                    //String src = AppLogic.GetImagePath("GALLERY", "icon", false) + Dir + "/" + fArray[i].ToString().ToLowerInvariant();
                    String src = "../images/gallery/" + Dir + "/" + fArray[i].ToString().ToLowerInvariant();
                    String src_lg = src.Replace(".jpg", "_lg.jpg");
                    System.Drawing.Size size = CommonLogic.GetImagePixelSize(src_lg);
                    long s = CommonLogic.GetImageSize(src_lg);
                    tmpS.Append("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                    tmpS.Append("<td >" + (i + 1).ToString() + "</td>\n");
                    tmpS.Append("<td >" + fArray[i].ToString() + "</td>\n");
                    tmpS.Append("<td >images/gallery/" + Dir + "/" + fArray[i].ToString() + "</td>\n");
                    tmpS.Append("<td >" + size.Width.ToString() + "x" + size.Height.ToString() + "</td>\n");
                    tmpS.Append("<td >" + (s / 1000).ToString() + " KB</td>\n");
                    tmpS.Append("<td align=\"center\"><a target=\"_blank\" href=\"" + src_lg + "\">\n");
                    tmpS.Append("<img border=\"0\" src=\"" + src + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\"" + CommonLogic.IIF(size.Height > 50, " height=\"50\"", "") + ">\n");
                    tmpS.Append("</a></td>\n");
                    tmpS.Append("<td align=\"center\">");
                    if (i != 0)
                    {
                        tmpS.Append("<input type=\"button\" value=\"To First\" name=\"MoveFirst_" + i.ToString() + "\" onClick=\"self.location = 'galleryimages.aspx?galleryid=" + GalleryID.ToString() + "&movefirstid=' + " + (i + 1).ToString() + ";\">");
                        tmpS.Append("<input type=\"button\" value=\"Up\" name=\"MoveUp_" + i.ToString() + "\" onClick=\"self.location = 'galleryimages.aspx?galleryid=" + GalleryID.ToString() + "&moveupid=' + " + (i + 1).ToString() + ";\">");
                    }
                    if (i != fArray.Count - 1)
                    {
                        tmpS.Append("<input type=\"button\" value=\"Down\" name=\"MoveDown_" + i.ToString() + "\" onClick=\"self.location = 'galleryimages.aspx?galleryid=" + GalleryID.ToString() + "&movedownid=' + " + (i + 1).ToString() + ";\">");
                        tmpS.Append("<input type=\"button\" value=\"To Last\" name=\"MoveLast_" + i.ToString() + "\" onClick=\"self.location = 'galleryimages.aspx?galleryid=" + GalleryID.ToString() + "&movelastid=' + " + (i + 1).ToString() + ";\">");
                    }
                    tmpS.Append("</td>\n");
                    tmpS.Append("<td align=\"center\">");
                    tmpS.Append("<input type=\"button\" value=\"Delete\" name=\"Delete_" + i.ToString() + "\" onClick=\"DeleteImage(" + (i + 1).ToString() + ")\">");
                    tmpS.Append("</td>\n");
                    tmpS.Append("</tr>\n");
                }
            }

            tmpS.Append("</table>\n");
            tmpS.Append("<script type=\"text/javascript\">\n");
            tmpS.Append("function DeleteImage(name)\n");
            tmpS.Append("{\n");
            tmpS.Append("if(confirm('Are you sure you want to delete gallery slide: ' + name))\n");
            tmpS.Append("{\n");
            tmpS.Append("self.location = 'galleryimages.aspx?galleryid=" + GalleryID.ToString() + "&deleteid=' + name;\n");
            tmpS.Append("}\n");
            tmpS.Append("}\n");
            tmpS.Append("</SCRIPT>\n");

            ltContent.Text = tmpS.ToString();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            // handle upload if any also:
            int NextSlideNumber = CommonLogic.FormUSInt("NewSlideNumber");
            HttpPostedFile Image1File = fuMain.PostedFile;
            if (Image1File.ContentLength != 0)
            {
                String tmp = Image1File.FileName;
                if (tmp.LastIndexOf('\\') != -1)
                {
                    tmp = tmp.Substring(tmp.LastIndexOf('\\') + 1);
                }
                String fn = SFP + "slide" + NextSlideNumber.ToString().PadLeft(2, '0') + ".jpg";
                String fn_lg = SFP + "slide" + NextSlideNumber.ToString().PadLeft(2, '0') + "_lg.jpg";
                Image1File.SaveAs(fn_lg);

                // create thumbnail:
                System.Drawing.Image g;
                System.Drawing.Bitmap g2;
                int newWidth = 0;
                int newHeight = 0;
                Single sizer = 0;
                int boxWidth = 125;
                int boxHeight = 125;

                // create a new image from file
                g = System.Drawing.Image.FromFile(fn_lg);

                if (g.Height > g.Width)		// portrait
                {
                    sizer = (Single)boxWidth / (Single)g.Height;
                }
                else
                {
                    sizer = (Single)boxHeight / (Single)g.Width;
                }

                newWidth = Convert.ToInt32(g.Width * sizer);
                newHeight = Convert.ToInt32(g.Height * sizer);

                g2 = new System.Drawing.Bitmap(g, newWidth, newHeight);

                // set the content type
                Response.ContentType = "image/jpeg";

                // send the image to the viewer
                g2.Save(fn, g.RawFormat);

                // tidy up
                g.Dispose();

                LoadContent();
            }
        }
    }
}
