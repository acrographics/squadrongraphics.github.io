// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/addtocart.aspx.cs 7     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for addtocart.
    /// </summary>
    public partial class addtocart : SkinBase
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            ThisCustomer.RequireCustomerRecord();

            String ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");
            AppLogic.CheckForScriptTag(ReturnURL);

            int ShippingAddressID = CommonLogic.QueryStringUSInt("ShippingAddressID"); // only used for multi-ship
            if (ShippingAddressID == 0)
            {
                ShippingAddressID = CommonLogic.FormNativeInt("ShippingAddressID");
            }
            if ((ShippingAddressID == 0 || !ThisCustomer.OwnsThisAddress(ShippingAddressID)) && ThisCustomer.PrimaryShippingAddressID != 0)
            {
                ShippingAddressID = ThisCustomer.PrimaryShippingAddressID;
            }

            int ProductID = CommonLogic.QueryStringUSInt("ProductID");
            if (ProductID == 0)
            {
                ProductID = CommonLogic.FormUSInt("ProductID");
            }

            int VariantID = CommonLogic.QueryStringUSInt("VariantID");
            if (VariantID == 0)
            {
                VariantID = CommonLogic.FormUSInt("VariantID");
            }
            if (ProductID == 0)
            {
                ProductID = AppLogic.GetVariantProductID(VariantID);
            } 
            
            int Quantity = CommonLogic.QueryStringUSInt("Quantity");
            if (Quantity == 0)
            {
                Quantity = CommonLogic.FormNativeInt("Quantity");
            }
            if (Quantity == 0)
            {
                Quantity = 1;
            }

            VariantStyleEnum VariantStyle = (VariantStyleEnum)CommonLogic.QueryStringUSInt("VariantStyle");
            if (CommonLogic.QueryStringCanBeDangerousContent("VariantStyle").Length == 0)
            {
                VariantStyle = (VariantStyleEnum)CommonLogic.FormNativeInt("VariantStyle");
            }
            decimal CustomerEnteredPrice;
            decimal CustomerEnteredWeight;
            bool isFramed="yes".Equals(CommonLogic.FormCanBeDangerousContent("framed"));
            if (isFramed)
            {
                CustomerEnteredPrice = CommonLogic.FormNativeDecimal("FinishedPrice");
                CustomerEnteredWeight = CommonLogic.FormNativeDecimal("FinishedWeight");
                if (CustomerEnteredPrice == System.Decimal.Zero)
                {
                    CustomerEnteredPrice = CommonLogic.QueryStringNativeDecimal("FinishedPrice");
                }
            }
            else
            {
                CustomerEnteredPrice = CommonLogic.FormNativeDecimal("Price");
                CustomerEnteredWeight = 0;
                if (CustomerEnteredPrice == System.Decimal.Zero)
                {
                    CustomerEnteredPrice = CommonLogic.QueryStringNativeDecimal("Price");
                }
                if (!AppLogic.VariantAllowsCustomerPricing(VariantID))
                {
                    CustomerEnteredPrice = System.Decimal.Zero;
                }
                if (CustomerEnteredPrice < System.Decimal.Zero)
                {
                    CustomerEnteredPrice = -CustomerEnteredPrice;
                }
            }
            int CustomerID = ThisCustomer.CustomerID;

            // QueryString params override Form Params!

            String ChosenColor = String.Empty;
            String ChosenColorSKUModifier = String.Empty;
            String ChosenSize = String.Empty;
            String ChosenSizeSKUModifier = String.Empty;
            //String ChosenAttr3 = String.Empty;
            //String ChoseAttr3SKUModifier = String.Empty;
            String TextOption = CommonLogic.FormCanBeDangerousContent("TextOption");
            if (CommonLogic.QueryStringCanBeDangerousContent("TextOption").Length != 0)
            {
                TextOption = Security.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("TextOption"));
            }

            CartTypeEnum CartType = CartTypeEnum.ShoppingCart;
            if (CommonLogic.FormNativeInt("IsWishList") == 1 || CommonLogic.QueryStringUSInt("IsWishList") == 1)
            {
                CartType = CartTypeEnum.WishCart;
            }
            if (CommonLogic.FormNativeInt("IsGiftRegistry") == 1 || CommonLogic.QueryStringUSInt("IsGiftRegistry") == 1)
            {
                CartType = CartTypeEnum.GiftRegistryCart;
            }

            // the color & sizes coming in here are MUST be in the Master WebConfig Locale ALWAYS!
            if (CommonLogic.QueryStringCanBeDangerousContent("Color").Length != 0)
            {
                String[] ColorSel = CommonLogic.QueryStringCanBeDangerousContent("Color").Split(',');
                try
                {
                    ChosenColor = Security.HtmlEncode(ColorSel[0]);
                }
                catch { }
                try
                {
                    ChosenColorSKUModifier = Security.HtmlEncode(ColorSel[1]);
                }
                catch { }
            }

            if (ChosenColor.Length == 0 && CommonLogic.FormCanBeDangerousContent("Color").Length != 0)
            {
                String[] ColorSel = CommonLogic.FormCanBeDangerousContent("Color").Split(',');
                try
                {
                    ChosenColor = Security.HtmlEncode(ColorSel[0]);
                }
                catch { }
                try
                {
                    ChosenColorSKUModifier = Security.HtmlEncode(ColorSel[1]);
                }
                catch { }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("Size").Length != 0)
            {
                String[] SizeSel = CommonLogic.QueryStringCanBeDangerousContent("Size").Split(',');
                try
                {
                    ChosenSize = Security.HtmlEncode(SizeSel[0]);
                }
                catch { }
                try
                {
                    ChosenSizeSKUModifier = Security.HtmlEncode(SizeSel[1]);
                }
                catch { }
            }

            if (ChosenSize.Length == 0 && CommonLogic.FormCanBeDangerousContent("Size").Length != 0)
            {
                String[] SizeSel = CommonLogic.FormCanBeDangerousContent("Size").Split(',');
                try
                {
                    ChosenSize = Security.HtmlEncode(SizeSel[0]);
                }
                catch { }
                try
                {
                    ChosenSizeSKUModifier = Security.HtmlEncode(SizeSel[1]);
                }
                catch { }
            }

            //if (CommonLogic.QueryStringCanBeDangerousContent("Attr3").Length != 0)
            //{
            //    String[] SizeSel2 = CommonLogic.QueryStringCanBeDangerousContent("Attr3").Split(',');
            //    try
            //    {
            //        ChosenAttr3 = Security.HtmlEncode(SizeSel2[0]);
            //    }
            //    catch { }
            //    try
            //    {
            //        ChoseAttr3SKUModifier = Security.HtmlEncode(SizeSel2[1]);
            //    }
            //    catch { }
            //}

            //if (ChosenAttr3.Length == 0 && CommonLogic.FormCanBeDangerousContent("Attr3").Length != 0)
            //{
            //    String[] SizeSel2 = CommonLogic.FormCanBeDangerousContent("Attr3").Split(',');
            //    try
            //    {
            //        ChosenAttr3 = Security.HtmlEncode(SizeSel2[0]);
            //    }
            //    catch { }
            //    try
            //    {
            //        ChoseAttr3SKUModifier = Security.HtmlEncode(SizeSel2[1]);
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

            if (VariantStyle == VariantStyleEnum.ERPWithRollupAttributes)
            {

                //MOD START DV - The item may only have one attribute, and they can be in any order!
                String match = "<GroupAttributes></GroupAttributes>";
                String match2 = "<GroupAttributes></GroupAttributes>";
                if (ChosenSize.Trim().Length != 0 && ChosenColor.Trim().Length != 0)
                {
                    match = "<GroupAttributes><GroupAttributeName=\"Attr1\"Value=\"" + ChosenSize + "\"/><GroupAttributeName=\"Attr2\"Value=\"" + ChosenColor + "\"/></GroupAttributes>";
                    match2 = "<GroupAttributes><GroupAttributeName=\"Attr1\"Value=\"" + ChosenColor + "\"/><GroupAttributeName=\"Attr2\"Value=\"" + ChosenSize + "\"/></GroupAttributes>";
                }
                else if (ChosenSize.Trim().Length != 0 && ChosenColor.Trim().Length == 0)
                {
                    match = "<GroupAttributes><GroupAttributeName=\"Attr1\"Value=\"" + ChosenSize + "\"/></GroupAttributes>";
                }
                else if (ChosenSize.Trim().Length == 0 && ChosenColor.Trim().Length != 0)
                {
                    match = "<GroupAttributes><GroupAttributeName=\"Attr1\"Value=\"" + ChosenColor + "\"/></GroupAttributes>";
                }
                //END MOD

                // reset variant id to the proper attribute match!
                IDataReader rsERP = DB.GetRS("select VariantID,ExtensionData2 from ProductVariant with (NOLOCK) where VariantID=" + VariantID.ToString());
                while (rsERP.Read())
                {
                    //String match = "<GroupAttributes><GroupAttributeName=\"Attr1\"Value=\"" + ChosenSize + "\"/><GroupAttributeName=\"Attr2\"Value=\"" + ChosenColor + "\"/></GroupAttributes>";

                    String thisVariantMatch = DB.RSField(rsERP, "ExtensionData2").Replace(" ", "").Trim();
                    match = Regex.Replace(match, "\\s+", "").ToLowerInvariant();

                    //MOD START DV
                    match2 = Regex.Replace(match2, "\\s+", "").ToLowerInvariant();

                    thisVariantMatch = Regex.Replace(thisVariantMatch, "\\s+", "").ToLowerInvariant();
                    //if (match == thisVariantMatch)
                    if (match == thisVariantMatch || match2 == thisVariantMatch)
                    {
                        VariantID = DB.RSFieldInt(rsERP, "VariantID");
                        break;
                    }
                    //END MOD
                }
                rsERP.Close();
            }

            ShoppingCart cart = new ShoppingCart(1, ThisCustomer, CartType, 0, false);
            if (Quantity > 0)
            {
                if (AppLogic.IsAKit(ProductID))
                {
                    String tmp = DB.GetNewGUID();
                    int NewRecID = cart.AddItem(ThisCustomer, ShippingAddressID, ProductID, VariantID, Quantity, "NEWKIT", ChosenColorSKUModifier, ChosenSize, ChosenSizeSKUModifier, TextOption, CartType, false, false, 0, System.Decimal.Zero);
                    String totalPrice = null;
                    Decimal totalWeight = 0;
                    using(IDataReader dr = DB.GetRS("select InventoryVariantColor,InventoryVariantSize from KitCart where ProductID=" + ProductID.ToString() + " and VariantID=" + VariantID.ToString() + " and ShoppingCartRecID=0 and CustomerID=" + CustomerID.ToString()+" and KitGroupTypeID="+(int)KitGroupType.TempTotalPrint))
                    {
                      if (dr.Read())
                      {
                          totalPrice = DB.RSField(dr, "InventoryVariantColor");
                          try
                          {
                              totalWeight = Convert.ToDecimal(DB.RSField(dr, "InventoryVariantSize"));
                          }
                          catch
                          {   totalWeight=0;
                          }
                      }
                    }
                    DB.ExecuteSQL("update KitCart set ShoppingCartRecID=" + NewRecID.ToString() + " where ProductID=" + ProductID.ToString() + " and VariantID=" + VariantID.ToString() + " and ShoppingCartRecID=0 and CustomerID=" + CustomerID.ToString());
                    DB.ExecuteSQL("update ShoppingCart set ChosenColor='' where ChosenColor=" + DB.SQuote("NEWKIT") + " and ShoppingCartRecID=" + NewRecID.ToString());
                    
                    //AB:Framed update Price and White
                    decimal kitWeight = AppLogic.KitWeightDelta(ThisCustomer.CustomerID, ProductID, NewRecID);
                    if (!String.IsNullOrEmpty(totalPrice)  && totalWeight!=null)
                    {
                        totalWeight += kitWeight;
                        DB.ExecuteSQL("update ShoppingCart set ProductPrice=" + totalPrice + ",ProductWeight=" + totalWeight + " where ShoppingCartRecID=" + NewRecID);
                    }
                    else
                        DB.ExecuteSQL("update ShoppingCart set ProductWeight=" + kitWeight + " where ShoppingCartRecID=" + NewRecID);
                }
                else if (AppLogic.IsAPack(ProductID) && CommonLogic.QueryStringCanBeDangerousContent("UpdateCartPack") == "")
                {
                    IDataReader dr = DB.GetRS("select RequiresTextOption from dbo.Product where ProductID=" + ProductID.ToString());
                    dr.Read();
                    bool RequiresTextOption = DB.RSFieldBool(dr, "RequiresTextOption");
                    dr.Close();
                    int NewRecID = 0;
                    if (AppLogic.IsAPresetPack(ProductID) && !RequiresTextOption)
                    {
                        //Make sure Customer Cart has items
                        decimal PackPrice = 0.0M;
                        string PackProducts = String.Empty;
                        AppLogic.PresetPack(ThisCustomer, ProductID, CartType, out PackPrice, out PackProducts);

                        //If it is a Preset Pack the mix doesn't change. Check to see if this pack has been added previously. if so just bump the number.
                        NewRecID = DB.GetSqlN("select ShoppingCartRecID as N from ShoppingCart where ProductID=" + ProductID.ToString() + " and CartType=" + ((int)CartType).ToString() + " and CustomerID=" + CustomerID.ToString());
                        if (NewRecID != 0)
                        {
                            //Already in the ShoppingCart so just up the Quantity
                            DB.ExecuteSQL("update ShoppingCart set Quantity=Quantity + " + Quantity.ToString() + " where ShoppingCartRecID=" + NewRecID + " and CartType=" + ((int)CartType).ToString() + " and CustomerID=" + CustomerID.ToString());
                        }
                    }
                    if (NewRecID == 0)
                    {
                        String tmp = CommonLogic.GetNewGUID();
                        cart.AddItem(ThisCustomer, ShippingAddressID, ProductID, VariantID, Quantity, tmp, ChosenColorSKUModifier, ChosenSize, ChosenSizeSKUModifier, TextOption, CartType, false, false, 0, System.Decimal.Zero);
                        IDataReader rs = DB.GetRS("select ShoppingCartRecID from ShoppingCart where ChosenColor=" + DB.SQuote(tmp) + " and CustomerID=" + CustomerID.ToString());
                        rs.Read();
                        NewRecID = DB.RSFieldInt(rs, "ShoppingCartRecID");
                        rs.Close();
                        DB.ExecuteSQL("update CustomCart set CartType=" + ((int)CartType).ToString() + ", ShoppingCartRecID=" + NewRecID.ToString() + " where PackID=" + ProductID.ToString() + " and CartType=" + ((int)CartType).ToString() + " and ShoppingCartRecID=0 and CustomerID=" + CustomerID.ToString());
                        DB.ExecuteSQL("update ShoppingCart set ChosenColor=" + DB.SQuote("") + " where ShoppingCartRecID=" + NewRecID.ToString());
                    }
                }
                else
                {
                    cart.AddItem(ThisCustomer, ShippingAddressID, ProductID, VariantID, Quantity, ChosenColor, ChosenColorSKUModifier, ChosenSize, ChosenSizeSKUModifier, TextOption, CartType, false, false, 0, CustomerEnteredPrice);
                }
            }

            // handle upsell products:
            String UpsellProducts = CommonLogic.FormCanBeDangerousContent("UpsellProducts").Trim();
            if (UpsellProducts.Length != 0 && CartType == CartTypeEnum.ShoppingCart)
            {
                foreach (String s in UpsellProducts.Split(','))
                {
                    String PID = s.Trim();
                    if (PID.Length != 0)
                    {
                        int UpsellProductID = 0;
                        try
                        {
                            UpsellProductID = Localization.ParseUSInt(PID);
                            if (UpsellProductID != 0)
                            {
                                int UpsellVariantID = AppLogic.GetProductsFirstVariantID(UpsellProductID);
                                if (UpsellVariantID != 0)
                                {
                                    // this variant COULD have one size or color, so set it up like that:
                                    String Sizes = String.Empty;
                                    String SizeSKUModifiers = String.Empty;
                                    String Colors = String.Empty;
                                    String ColorSKUModifiers = String.Empty;
                                    IDataReader rs = DB.GetRS("select Sizes,SizeSKUModifiers,Colors,ColorSKUModifiers from ProductVariant " + DB.GetNoLock() + " where VariantID=" + UpsellVariantID.ToString());
                                    if (rs.Read())
                                    {
                                        Sizes = DB.RSField(rs, "Sizes");
                                        SizeSKUModifiers = DB.RSField(rs, "SizeSKUModifiers");
                                        Colors = DB.RSField(rs, "Colors");
                                        ColorSKUModifiers = DB.RSField(rs, "ColorSKUModifiers");
                                    }
                                    rs.Close();
                                    // safety check:
                                    if (Sizes.IndexOf(',') != -1)
                                    {
                                        Sizes = String.Empty;
                                        SizeSKUModifiers = String.Empty;
                                    }
                                    // safety check:
                                    if (Colors.IndexOf(',') != -1)
                                    {
                                        Colors = String.Empty;
                                        ColorSKUModifiers = String.Empty;
                                    }
                                    cart.AddItem(ThisCustomer, ShippingAddressID, UpsellProductID, UpsellVariantID, 1, Colors, ColorSKUModifiers, Sizes, SizeSKUModifiers, String.Empty, CartType, false, false, 0, System.Decimal.Zero);
                                    Decimal PR = AppLogic.GetUpsellProductPrice(ProductID, UpsellProductID, ThisCustomer.CustomerLevelID);
                                    DB.ExecuteSQL("update shoppingcart set IsUpsell=1, ProductPrice=" + Localization.CurrencyStringForDBWithoutExchangeRate(PR) + " where CartType=" + ((int)CartType).ToString() + " and CustomerID=" + ThisCustomer.CustomerID.ToString() + " and ProductID=" + UpsellProductID.ToString() + " and VariantID=" + UpsellVariantID.ToString() + " and convert(nvarchar(1000),ChosenColor)='' and convert(nvarchar(1000),ChosenSize)='' and convert(nvarchar(1000),TextOption)=''");
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            cart = null;

            if (AppLogic.AppConfig("AddToCartAction").ToUpperInvariant() == "STAY" && ReturnURL.Length != 0)
            {
                Response.Redirect(ReturnURL);
            }
            else
            {
                if (ReturnURL.Length == 0)
                {
                    ReturnURL = String.Empty;
                    if (Request.UrlReferrer != null)
                    {
                        ReturnURL = Request.UrlReferrer.AbsoluteUri; // could be null
                    }
                    if (ReturnURL == null)
                    {
                        ReturnURL = String.Empty;
                    }
                }
                if (CartType == CartTypeEnum.WishCart)
                {
                    Response.Redirect("wishlist.aspx?ReturnUrl=" + Security.UrlEncode(ReturnURL));
                }
                if (CartType == CartTypeEnum.GiftRegistryCart)
                {
                    Response.Redirect("giftregistry.aspx?ReturnUrl=" + Security.UrlEncode(ReturnURL));
                }
                Response.Redirect("ShoppingCart.aspx?add=true&ReturnUrl=" + Security.UrlEncode(ReturnURL));
            }

            AppLogic.eventHandler("AddToCart").CallEvent("&AddToCart=true&VariantID=" + VariantID.ToString() + "&ProductID" + ProductID.ToString() + "&ChosenColor=" + ChosenColor + "&ChosenSize=" + ChosenSize);

        }

    }
}
