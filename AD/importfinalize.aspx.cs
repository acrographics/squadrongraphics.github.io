// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/importfinalize.aspx.cs 3     9/03/06 8:42p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for importfinalize
    /// </summary>
    public partial class importfinalize : AspDotNetStorefront.SkinBase
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
            Server.ScriptTimeout = 1000000;

            SectionTitle = "Import Finalization";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            String action = CommonLogic.QueryStringCanBeDangerousContent("action").ToUpperInvariant();
            if (action == "ACCEPT")
            {
                DB.ExecuteLongTimeSQL("aspdnsf_ClearAllImportFlags", 1000);
                writer.Write("<p><b>IMPORT ACCEPTED</b></p>");
            }
            if (action == "UNDO")
            {
                DB.ExecuteLongTimeSQL("aspdnsf_UndoImport", 1000);
                writer.Write("<p><b><font color=red>IMPORT HAS BEEN UNDONE</font></b></p>");
            }
        }
    }
}
