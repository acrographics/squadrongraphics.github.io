// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/tableorder_process.aspx.cs 4     10/03/06 7:56p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for tableorder_process.
    /// </summary>
    public partial class tableorder_process : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
            ThisCustomer.RequireCustomerRecord();

            CartTypeEnum CartType = CartTypeEnum.ShoppingCart;

            int CustomerID = ThisCustomer.CustomerID;
            ShoppingCart cart = new ShoppingCart(1, ThisCustomer, CartType, 0, false);
            for (int i = 0; i < Request.Form.Count; i++)
            {
                String FieldName = Request.Form.Keys[i];
                String FieldVal = Request.Form[Request.Form.Keys[i]];
                if (FieldName.ToUpperInvariant().IndexOf("_VLDT") == -1 && FieldName.ToLowerInvariant().StartsWith("qty_") && FieldVal.Trim().Length != 0)
                {
                    try // ignore errors, just add what items we can:
                    {
                        String[] flds = FieldName.Split('_');
                        int ProductID = Localization.ParseUSInt(flds[1]);
                        int VariantID = Localization.ParseUSInt(flds[2]);
                        int ColorIdx = Localization.ParseUSInt(flds[3]);
                        int SizeIdx = Localization.ParseUSInt(flds[4]);
                        int Qty = Localization.ParseUSInt(FieldVal);

                        IDataReader rs = DB.GetRS("select * from productvariant where VariantID=" + VariantID.ToString());
                        rs.Read();
                        String ChosenColor = DB.RSField(rs, "Colors").Split(',')[ColorIdx].Trim();
                        String ChosenColorSKUModifier = String.Empty;
                        if (DB.RSField(rs, "ColorSKUModifiers").Length != 0)
                        {
                            ChosenColorSKUModifier = DB.RSField(rs, "ColorSKUModifiers").Split(',')[ColorIdx].Trim();
                        }
                        String ChosenSize = DB.RSField(rs, "Sizes").Split(',')[SizeIdx].Trim();
                        String ChosenSizeSKUModifier = String.Empty;
                        if (DB.RSField(rs, "SizeSKUModifiers").Length != 0)
                        {
                            ChosenSizeSKUModifier = DB.RSField(rs, "SizeSKUModifiers").Split(',')[SizeIdx].Trim();
                        }
                        rs.Close();

                        String TextOption = String.Empty;
                        cart.AddItem(ThisCustomer, 0, ProductID, VariantID, Qty, ChosenColor, ChosenColorSKUModifier, ChosenSize, ChosenSizeSKUModifier, TextOption, CartType, false, false, 0, System.Decimal.Zero);

                    }
                    catch { }
                }
            }
            cart = null;

            String url = CommonLogic.IIF(CartType == CartTypeEnum.WishCart, "wishlist.aspx", "ShoppingCart.aspx?add=true");
            Response.Redirect(url);
        }

    }
}
