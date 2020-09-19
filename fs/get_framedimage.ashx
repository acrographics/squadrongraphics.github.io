<%@ WebHandler Language="C#" Class="FrameStudio.GetFramedImage" %>

using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Configuration;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Collections.Generic;

using AspDotNetStorefrontCommon;
using FrameEngine;

namespace FrameStudio
{
	public class GetFramedImage : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
            int printID = Convert.ToInt32(context.Request.QueryString["pr1"]);
            SortedDictionary<string, string> dict = new SortedDictionary<string, string>();
            foreach (string key in context.Request.QueryString)
            {
                if (key!=null)
                 dict[key] = context.Request.QueryString[key];
            }
            string physhicsAppRoot=context.Server.MapPath("~");
            ViewPicture vp = new ViewPicture(dict);
            PictureGenerator pg = new PictureGenerator(vp, Path.Combine(physhicsAppRoot,"images"));
            context.Response.ContentType = "image/jpeg";
            pg.GetCachedImage(context.Response.OutputStream,DB.GetDBConn());
            context.Response.Flush();
		}

		// Override the IsReusable property
		public bool IsReusable
		{
			get { return true; }
		}

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            } return null;
        }
	}
}

