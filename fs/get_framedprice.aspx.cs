// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// ------------------------------------------------------------------------------------------
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
using System.Xml;

using AspDotNetStorefrontCommon;
using FrameEngine;

namespace FrameStudio
{
    /// <summary>
    /// Summary description for search.
    /// </summary>
    public partial class get_framedprice : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            SortedDictionary<string, string> dict = new SortedDictionary<string, string>();
            foreach (string key in Request.QueryString)
            {
                if (key != null)
                  dict[key] = Request.QueryString[key];
            }
            ViewPicture vp = new ViewPicture(dict);

            Response.Expires = -1;
            Response.ContentType = "application/xml";
            XmlTextWriter writer = new XmlTextWriter(Response.OutputStream, System.Text.Encoding.UTF8);
            writer.WriteStartDocument();
            writer.WriteStartElement("root");

            vp.GetFinishedInfo(DB.GetDBConn(),writer);
          
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
        }

    }
}