// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.AspDotNetStorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/search.aspx.cs 3     7/31/06 11:17a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for search.
    /// </summary>
    public partial class search : SkinBase
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }

            // this may be overwridden by the XmlPackage below!
            SectionTitle = AppLogic.GetString("search.aspx.1", SkinID, ThisCustomer.LocaleSetting);

            String st = CommonLogic.QueryStringCanBeDangerousContent("SearchTerm").Trim();
            if (st.Length != 0)
            {
                DB.ExecuteSQL("insert into SearchLog(SearchTerm,CustomerID,LocaleSetting) values(" + DB.SQuote(CommonLogic.Ellipses(st, 97, true)) + "," + ThisCustomer.CustomerID.ToString() + "," + DB.SQuote(ThisCustomer.LocaleSetting) + ")");
            }

            // set the Customer context, and set the SkinBase context, so meta tags to be set if they are not blank in the XmlPackage results
            Package1.SetContext = this;
        }

    }
}
