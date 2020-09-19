// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/reorder.aspx.cs 2     7/19/06 11:13p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
	/// <summary>
	/// Summary description for reorder.
	/// </summary>
	public partial class reorder : SkinBase
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
            // currently viewing user must be logged in to view receipts:
            if (!ThisCustomer.IsRegistered)
            {
                Response.Redirect("signin.aspx?returnurl=reorder.aspx?" + Server.UrlEncode(CommonLogic.ServerVariables("QUERY_STRING")));
            }

			SectionTitle = AppLogic.GetString("reorder.aspx.1",SkinID,ThisCustomer.LocaleSetting);
		}

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
		{
			int OrderNumber = CommonLogic.QueryStringUSInt("OrderNumber");

            // are we allowed to view?
            // if currently logged in user is not the one who owns the order, and this is not an admin user who is logged in, reject the reorder:
            if (ThisCustomer.CustomerID != Order.GetOrderCustomerID(OrderNumber) && !ThisCustomer.IsAdminUser)
            {
                Response.Redirect(SE.MakeDriverLink("ordernotfound"));
            }

			if(OrderNumber == 0)
			{
				writer.Write("<p>" + String.Format(AppLogic.GetString("reorder.aspx.2",SkinID,ThisCustomer.LocaleSetting),"account.aspx") + "</p>");
			}
			String StatusMsg = String.Empty;
			if(Order.BuildReOrder(base.EntityHelpers,ThisCustomer,OrderNumber,out StatusMsg))
			{
				Response.Redirect("shoppingcart.aspx");
			}
			else
			{ 
				Response.Write("<p>There were some errors in trying to create the order.</p>");
				Response.Write("<p>Error: " + StatusMsg + "</p>");
				Response.Write("<p>" + String.Format(AppLogic.GetString("reorder.aspx.2",SkinID,ThisCustomer.LocaleSetting),"shoppingcart.aspx",AppLogic.GetString("AppConfig.CartPrompt",SkinID,ThisCustomer.LocaleSetting)) + "</p>");
			}
		}
	}
}
