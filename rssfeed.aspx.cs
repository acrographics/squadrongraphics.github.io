// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/rssfeed.aspx.cs 3     8/15/06 7:27p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for rssfeed.
    /// </summary>
    public partial class rssfeed : System.Web.UI.Page
    {

        private void Page_Load(object sender, System.EventArgs e)
        {
            String Channel = CommonLogic.QueryStringCanBeDangerousContent("Channel").ToLowerInvariant();

            if (Channel.Length == 0)
            {
                Channel = "unknown";
            }
            
            XmlPackage2 p = new XmlPackage2("rss." + Channel + ".xml.config");

            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/xml";
            String s = p.TransformString();
            Response.Write(s);
        }
    }
}
