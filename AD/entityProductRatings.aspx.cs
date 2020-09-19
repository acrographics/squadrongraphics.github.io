// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/entityProductRatings.aspx.cs 2     8/19/06 8:51p Buddy $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.IO;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for productratings
	/// </summary>
	public partial class entityProductRatings : System.Web.UI.Page
	{		
		private int ProductID;
        private Customer ThisCustomer;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache"); 
            
			ProductID = CommonLogic.QueryStringUSInt("ProductID");
			if(ProductID == 0)
			{
				Response.Redirect("products.aspx");
			}

			if(CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
			{
				// delete the rating:
				Ratings.DeleteRating(CommonLogic.QueryStringUSInt("DeleteID"));
			}
            LoadData();
		}

		protected void LoadData()
		{
			ltContent.Text = (Ratings.Display(ThisCustomer, ProductID,0,0,0,1));
		}

	}
}
