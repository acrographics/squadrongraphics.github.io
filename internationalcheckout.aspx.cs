// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
	/// <summary>
    /// Summary description for internationalcheckout.
	/// </summary>
    public partial class internationalcheckout : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            ShoppingCart cart = new ShoppingCart(ThisCustomer.SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);

            String YourStore = AppLogic.AppConfig("InternationalCheckout.StoreID");

            Response.Write("<html>\n");
            Response.Write("<head>\n");
            Response.Write("<title>" + AppLogic.AppConfig("StoreName") + " - International Checkout Redirect</title>\n");
            Response.Write("</head>\n");
            Response.Write("<body \n");
            if (!AppLogic.AppConfigBool("InternationalCheckout.TestMode"))
            {
                Response.Write("onLoad=\"document.getElementById('icForm').submit();\"\n");
            }
            Response.Write(">");
            Response.Write("<form name=\"icForm\" method=\"post\" action=\"http://www.internationalcheckout.com/cart.php?p=" + YourStore + "\">\n");
            int i = 1;
            foreach (CartItem c in cart.CartItems)
            {
                Response.Write("<input type=\"hidden\" name=\"ItemStore1\" value=\"" + YourStore + "\">\n");
                Response.Write("<input type=\"hidden\" name=\"ItemDescription1\" value=\"" + (c.m_ProductName + " " + c.m_VariantName).Trim() + "\">\n");
                Response.Write("<input type=\"hidden\" name=\"ItemSKU1\" value=\"" + c.m_SKU + "\">\n");
                if (c.m_ChosenSize.Trim().Length != 0)
                {
                    Response.Write("<input type=\"hidden\" name=\"ItemSize" + i.ToString() + "\" value=\"" + c.m_ChosenSize.Trim() + "\">\n");
                }
                if (c.m_ChosenColor.Trim().Length != 0)
                {
                    Response.Write("<input type=\"hidden\" name=\"ItemColor" + i.ToString() + "\" value=\"" + c.m_ChosenColor.Trim() + "\">\n");
                }
                Response.Write("<input type=\"hidden\" name=\"ItemQuantity" + i.ToString() + "\" value=\"" + c.m_Quantity.ToString() + "\">\n");
                Response.Write("<input type=\"hidden\" name=\"ItemPrice" + i.ToString() + "\" value=\"" + Localization.CurrencyStringForGatewayWithoutExchangeRate(c.m_Price) + "\">\n");
                i++;
            }
            Response.Write("<input type=\"hidden\" name=\"p\" value=\"" + YourStore + "\">\n");
            //Response.Write("<a href=\"javascript:document.icForm.submit();\"><img src=\" international_checkout.gif\" width=\"170\" height=\"47\" border=\"0\" /></a>\n");
            Response.Write("</form>\n");
            Response.Write("If you are not redirected automatically to InternationalCheckout.com, <a href=\"javascript:void(0);\" onClick=\"document.getElementById('icForm').submit();\">click here</a>");
            Response.Write("</body></html>");
		}

	}
}
