// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/dyop_addtocart.aspx.cs 5     9/13/06 9:27p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using AspDotNetStorefrontCommon;


namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for dyop_addtocart.
    /// </summary>
    public partial class dyop_addtocart : SkinBase
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            ThisCustomer.RequireCustomerRecord();

            int PackID = CommonLogic.QueryStringUSInt("PackID");
            int ProductID = CommonLogic.QueryStringUSInt("ProductID");
            int VariantID = CommonLogic.QueryStringUSInt("VariantID");
            int CategoryID = CommonLogic.QueryStringUSInt("CategoryID");
            int SectionID = CommonLogic.QueryStringUSInt("SectionID");
            int Quantity = CommonLogic.QueryStringUSInt("Quantity");
            if (Quantity == 0)
            {
                Quantity = 1;
            }

            //If editing a pack is from the shopping cart
            bool FromCart = (CommonLogic.QueryStringUSInt("CartRecID") > 0);
            int CartRecID = CommonLogic.QueryStringUSInt("CartRecID");


            String ChosenColor = String.Empty;
            String ChosenColorSKUModifier = String.Empty;
            String ChosenSize = String.Empty;
            String ChosenSizeSKUModifier = String.Empty;
            //String ChosenAttr3 = String.Empty;
            //String ChoseAttr3SKUModifier = String.Empty;

            if (CommonLogic.QueryStringCanBeDangerousContent("Color").Length != 0)
            {
                String[] ColorSel = CommonLogic.QueryStringCanBeDangerousContent("Color").Split(',');
                try
                {
                    ChosenColor = ColorSel[0];
                }
                catch { }
                try
                {
                    ChosenColorSKUModifier = ColorSel[1];
                }
                catch { }
            }
            if (ChosenColor.Length != 0)
            {
                ThisCustomer.ThisCustomerSession["ChosenColor"] = ChosenColor;
            }


            if (CommonLogic.QueryStringCanBeDangerousContent("Size").Length != 0)
            {
                String[] SizeSel = CommonLogic.QueryStringCanBeDangerousContent("Size").Split(',');
                try
                {
                    ChosenSize = SizeSel[0];
                }
                catch { }
                try
                {
                    ChosenSizeSKUModifier = SizeSel[1];
                }
                catch { }
            }
            if (ChosenSize.Length != 0)
            {
                ThisCustomer.ThisCustomerSession["ChosenSize"] = ChosenSize;
            }


            //if (CommonLogic.QueryStringCanBeDangerousContent("Attr3").Length != 0)
            //{
            //    String[] SizeSel2 = CommonLogic.QueryStringCanBeDangerousContent("Attr3").Split(',');
            //    try
            //    {
            //        ChosenAttr3 = SizeSel2[0];
            //    }
            //    catch { }
            //    try
            //    {
            //        ChoseAttr3SKUModifier = SizeSel2[1];
            //    }
            //    catch { }
            //}

            //if (ChosenAttr3.Length != 0)
            //{
            //    ChosenSize += "," + ChosenAttr3;
            //}
            //if (ChoseAttr3SKUModifier.Length != 0)
            //{
            //    ChosenSizeSKUModifier += "," + ChoseAttr3SKUModifier;
            //}
            //if (ChosenAttr3.Length != 0)
            //{
            //    ThisCustomer.ThisCustomerSession["ChosenAttr3"] = ChosenAttr3;
            //}

            if (Quantity > 0)
            {
                // add to custom cart:
                if (FromCart)
                {
                    CustomCart.AddItem(PackID, ProductID, VariantID, Quantity, ChosenColor, ChosenColorSKUModifier, ChosenSize, ChosenSizeSKUModifier, CartRecID, ThisCustomer, CartTypeEnum.ShoppingCart);
                }
                else
                {
                    CustomCart cart = new CustomCart(ThisCustomer, PackID, 1, CartTypeEnum.ShoppingCart);
                    cart.AddItem(ProductID, VariantID, Quantity, ChosenColor, ChosenColorSKUModifier, ChosenSize, ChosenSizeSKUModifier);
                }
            }


            if (CommonLogic.QueryStringCanBeDangerousContent("UpdateCartPack") == "")
            {
                String url = SE.MakeProductAndEntityLink(CommonLogic.QueryStringCanBeDangerousContent("entityname"), PackID, CommonLogic.QueryStringUSInt("entityid"), "");
                Response.Redirect(url + CommonLogic.IIF(FromCart, "?cartrecid=" + CartRecID.ToString(), ""));
            }
            else
            {
                Response.Redirect("shoppingcart.aspx");
            }
        }

    }
}
