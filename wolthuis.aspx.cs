// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/wolthuis.aspx.cs 10    10/04/06 12:00p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Drawing.Imaging;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for wolthuis.
    /// </summary>
    public partial class wolthuis : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            int ProductID = CommonLogic.QueryStringUSInt("ProductID");
            int VariantID = CommonLogic.QueryStringUSInt("VariantID");
            String ChosenColor = CommonLogic.QueryStringCanBeDangerousContent("Color"); // TBD for future use, not yet enabled!
            int SkinID = CommonLogic.QueryStringUSInt("SkinID");
            if (SkinID == 0)
            {
                SkinID = CommonLogic.QueryStringUSInt("SkinID");
            }
            if (SkinID == 0)
            {
                SkinID = 1;
            }

            String LocaleSetting = CommonLogic.QueryStringCanBeDangerousContent("LocaleSetting");
            AppLogic.CheckForScriptTag(LocaleSetting);
            if (LocaleSetting.Length == 0)
            {
                LocaleSetting = Localization.GetWebConfigLocale();
            }
            LocaleSetting = Localization.CheckLocaleSettingForProperCase(LocaleSetting);
            String ImgSize = CommonLogic.QueryStringCanBeDangerousContent("size");
            if (ImgSize.Length == 0)
            {
                ImgSize = "icon";
            }

            String EOName = "Product";
            int ID = ProductID;
            if (VariantID != 0)
            {
                EOName = "Variant";
                ID = VariantID;
            }

            String ImgUrl = String.Empty;
            if (CommonLogic.QueryStringCanBeDangerousContent("src").Length == 0)
            {
                ImgUrl = AppLogic.LookupImage(EOName, ID, ImgSize, SkinID, LocaleSetting);
            }
            else
            {
                ImgUrl = Server.UrlDecode(CommonLogic.QueryStringCanBeDangerousContent("src"));
                if (CommonLogic.QueryStringCanBeDangerousContent("e") == "1")
                {
                    ImgUrl = Security.UnmungeString(ImgUrl);
                }
            }


            System.Drawing.Image imgPhoto = CommonLogic.LoadImage(ImgUrl);
            String CopyrightText = AppLogic.AppConfig("Watermark.CopyrightText");
            if (CopyrightText.Length == 0)
            {
                CopyrightText = AppLogic.AppConfig("StoreName");
            }
            String CopyrightImage = AppLogic.AppConfig("Watermark.CopyrightImage." + ImgSize);

#if PRO
			// watermarks ML feature only
#else
            if (AppLogic.AppConfigBool("Watermark.Enabled") && ImgUrl.IndexOf("nopicture") == -1 && (CopyrightText.Length != 0 || CopyrightImage.Length != 0))
            {
                try
                {
                    imgPhoto = CommonLogic.AddWatermark(imgPhoto, CopyrightText, CopyrightImage);
                }
                catch { }
            }
#endif

            if (ImgUrl.EndsWith(".jpeg", StringComparison.InvariantCultureIgnoreCase))
            {
                Response.ContentType = "image/jpeg";
                EncoderParameters encoderParameters = new EncoderParameters();
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                imgPhoto.Save(Response.OutputStream, ImageCodecInfo.GetImageEncoders()[1], encoderParameters);
            }
            if (ImgUrl.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase))
            {
                Response.ContentType = "image/jpg";
                EncoderParameters encoderParameters = new EncoderParameters();
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                imgPhoto.Save(Response.OutputStream, ImageCodecInfo.GetImageEncoders()[1], encoderParameters);
            }
            if (ImgUrl.EndsWith(".gif", StringComparison.InvariantCultureIgnoreCase))
            {
                Response.ContentType = "image/gif";
                imgPhoto.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Gif);
            }
            if (ImgUrl.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
            {
                Response.ContentType = "image/png";
                imgPhoto.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Gif);
            }
            imgPhoto.Dispose();

        }

    }
}
