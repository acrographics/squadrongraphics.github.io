// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/ShoppingCart_change.aspx.cs 4     7/19/06 7:25p Buddy $
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
using System.Globalization;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
	/// <summary>
	/// Summary description for ShoppingCart_change.
	/// </summary>
	public partial class ShoppingCart_change : SkinBase
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");

            int ShoppingCartRecID = CommonLogic.QueryStringUSInt("RecID");
            int ProductID = CommonLogic.QueryStringUSInt("ProductId");
		    string sename = CommonLogic.QueryStringCanBeDangerousContent( "SEName" );
            if (ShoppingCartRecID == 0)
			{
				Response.Redirect("ShoppingCart.aspx");
			}
			// move this item back to the customcart (only one of the following two updates will actually do anything) depending on if the product is a pack or a kit:
            //DB.ExecuteSQL("update kitcart set ShoppingCartRecID=0 where ShoppingCartRecID=" + ShoppingCartRecID.ToString() + " and CustomerID=" + ThisCustomer.CustomerID.ToString());
			//DB.ExecuteSQL("update customcart set ShoppingCartRecID=0 where ShoppingCartRecID=" + ShoppingCartRecID.ToString() + " and CustomerID=" + ThisCustomer.CustomerID.ToString());

            IDataReader rs = DB.GetRS("select * from ShoppingCart " + DB.GetNoLock() + " where ShoppingCartRecID=" + ShoppingCartRecID.ToString() + " and CustomerID=" + ThisCustomer.CustomerID.ToString());
            int VariantID = 0;
            if (rs.Read())
            {
                ProductID = DB.RSFieldInt(rs, "ProductID");
                VariantID = DB.RSFieldInt(rs, "VariantID");
            }
            rs.Close();
            if (AppLogic.IsAKit(ProductID))
            {
                // move this item back to the kitcart
                DB.ExecuteSQL("delete from kitcart where ShoppingCartRecID=0 and CustomerID=" + ThisCustomer.CustomerID.ToString());
//                DB.ExecuteSQL("update kitcart set ShoppingCartRecID=0 where ShoppingCartRecID=" + ShoppingCartRecID.ToString() + " and CustomerID=" + ThisCustomer.CustomerID.ToString());
//                DB.ExecuteSQL("delete from ShoppingCart where ShoppingCartRecID=" + ShoppingCartRecID.ToString() + " and CustomerID=" + ThisCustomer.CustomerID.ToString());
            }
            if (AppLogic.IsAPack(ProductID))
            {
                // move this item back to the customcart
                DB.ExecuteSQL("delete from customcart where ShoppingCartRecID=0 and CustomerID=" + ThisCustomer.CustomerID.ToString());
                DB.ExecuteSQL("update customcart set ShoppingCartRecID=0 where ShoppingCartRecID=" + ShoppingCartRecID.ToString() + " and CustomerID=" + ThisCustomer.CustomerID.ToString());
                DB.ExecuteSQL("delete from ShoppingCart where ShoppingCartRecID=" + ShoppingCartRecID.ToString() + " and CustomerID=" + ThisCustomer.CustomerID.ToString());
            }
            string frtypeParam;
            if (!String.IsNullOrEmpty(CommonLogic.QueryStringCanBeDangerousContent("frtype")))
            {
                frtypeParam = "&frtype=" + CommonLogic.QueryStringCanBeDangerousContent("frtype");
            }
            else frtypeParam = "";
            Response.Redirect("showproduct.aspx?productid=" + ProductID.ToString() + "&cartrecid=" + ShoppingCartRecID.ToString() + "&variantid=" + VariantID+ "&SEName=" + sename.ToString() + frtypeParam);
		}

	}
}
