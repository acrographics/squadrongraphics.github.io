// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/twocheckout_return.aspx.cs 3     7/19/06 11:13p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using AspDotNetStorefrontCommon;
using AspDotNetStorefrontGateways;

namespace AspDotNetStorefront
{
	/// <summary>
	/// Summary description for twocheckout_return.
	/// </summary>
	public partial class twocheckout_return : SkinBase
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(CommonLogic.FormCanBeDangerousContent("x_2checked") == "Y" || CommonLogic.FormCanBeDangerousContent("x_2checked") == "K")
			{
				ShoppingCart cart = new ShoppingCart(1,ThisCustomer,CartTypeEnum.ShoppingCart,0,false);
				int OrderNumber = AppLogic.GetNextOrderNumber();
				//String status = Gateway.MakeOrder(cart,ro_GW2CHECKOUT,String.Empty,String.Empty,ro_GW2CHECKOUT,String.Empty,String.Empty,String.Empty,String.Empty,OrderNumber,String.Empty,String.Empty, String.Empty,String.Empty,String.Empty,String.Empty,String.Empty,String.Empty);
				Address UseBillingAddress = new Address();
				UseBillingAddress.LoadByCustomer(ThisCustomer.CustomerID,ThisCustomer.PrimaryBillingAddressID,AddressTypes.Billing);
                String status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, OrderNumber, String.Empty, String.Empty, String.Empty, String.Empty);

				Response.Redirect("orderconfirmation.aspx?ordernumber=" + OrderNumber.ToString() + "&paymentmethod=Credit+Card");
			}
			
			// error or not approved:
			Response.Redirect("shoppingcart.aspx?errormsg=" + Server.UrlEncode(AppLogic.GetString("twocheckout_return.aspx.1",SkinID,ThisCustomer.LocaleSetting)));
		}
	}
}
