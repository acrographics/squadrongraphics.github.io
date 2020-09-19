// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/cloneproduct.aspx.cs 3     8/03/06 10:43p Redwoodtree $
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
	/// Summary description for cloneproduct.
	/// </summary>
	public partial class cloneproduct : System.Web.UI.Page
	{
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            int ProductID = CommonLogic.QueryStringUSInt("ProductID");

			if(ProductID != 0)
			{
				int NewProductID = 0;
				IDataReader rs = DB.GetRS("aspdnsf_CloneProduct " + ProductID.ToString());
				if(rs.Read())
				{
					NewProductID = DB.RSFieldInt(rs,"ProductID");
				}
				rs.Close();
				if(NewProductID != 0)
				{
					Response.Redirect("editproduct.aspx?productid=" + NewProductID.ToString());
				}
			}
			Response.Redirect("products.aspx");
		}

	}
}
