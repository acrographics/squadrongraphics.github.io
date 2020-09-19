// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2004.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/yahooentity.aspx.cs 4     9/30/06 3:38p Redwoodtree $
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
    /// Summary description for yahooentity.
    /// </summary>
    public partial class yahooentity : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            String EntityName = CommonLogic.QueryStringCanBeDangerousContent("EntityName");
            AppLogic.CheckForScriptTag(EntityName);
            int EntityID = CommonLogic.QueryStringUSInt("EntityID");
            if (EntityName.Length != 0 && EntityID != 0)
            {
                EntityHelper eHlp = AppLogic.CategoryEntityHelper;
                Response.Write(XmlCommon.XmlEncode(AppLogic.GetStoreHTTPLocation(false) + SE.MakeEntityLink(EntityName, EntityID, String.Empty)) + Environment.NewLine);
                Response.Write(eHlp.GetEntityYahooObjectList(EntityID, Localization.GetWebConfigLocale(), 0, 0));
            }
        }


    }
}
