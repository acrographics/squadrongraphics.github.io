// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/driver.aspx.cs 6     9/30/06 10:30p Redwoodtree $
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

    public partial class driver : SkinBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // set the Customer context, and set the SkinBase context, so meta tags will be set if they are not blank in the Topic results
            if (Topic1.TopicName.Length == 0)
            {
                String PN = CommonLogic.QueryStringCanBeDangerousContent("TopicName");
                if (PN.Length == 0)
                {
                    PN = CommonLogic.QueryStringCanBeDangerousContent("Topic");
                }
                AppLogic.CheckForScriptTag(PN);
                Topic1.TopicName = PN;
            }
            if (CommonLogic.IsInteger(Topic1.TopicName))
            {
                Topic1.TopicID = System.Int32.Parse(Topic1.TopicName);
            }
            if (CommonLogic.FormCanBeDangerousContent("Password").Length != 0)
            {
                ThisCustomer.ThisCustomerSession["Topic" + Topic1.TopicName] = Security.MungeString(CommonLogic.FormCanBeDangerousContent("Password"));
            }
        }
    }
}