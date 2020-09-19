// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/driver2.aspx.cs 7     9/30/06 10:42p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{

    // this provides a topic page that is NOT rendered inside the skin. The topic page itself should also contain the full <html>, not just the body.
    public partial class driver2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // set the Customer context, and set the SkinBase context, so meta tags will be set if they are not blank in the Topic results
            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            String PN = CommonLogic.QueryStringCanBeDangerousContent("TopicName");
            if (PN.Length == 0)
            {
                PN = CommonLogic.QueryStringCanBeDangerousContent("Topic");
            }
            AppLogic.CheckForScriptTag(PN);
            TopicContents.Text = new Topic(PN, ThisCustomer.LocaleSetting, ThisCustomer.SkinID).Contents;

            if (CommonLogic.FormCanBeDangerousContent("Password").Length != 0)
            {
                ThisCustomer.ThisCustomerSession["Topic" + PN] = Security.MungeString(CommonLogic.FormCanBeDangerousContent("Password"));
            }
        }
    }
}