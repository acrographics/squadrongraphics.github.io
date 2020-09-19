// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/editproduct.aspx.cs 14    10/04/06 6:22a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Xsl;
using System.Collections;
using System.IO;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for editproduct
    /// </summary>
    public partial class editproduct : AspDotNetStorefront.SkinBase
    {
        int NumCats;
        int NumSecs;
        int NumAffs;
        int NumCL;
        int NumGenre;
        int NumVector;
        //int NumDist;
        int MaxCatMaps;
        int MM;
        int ProductID;
        ProductDescriptionFile pdesc;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            NumCats = DB.GetSqlN("Select count(*) as N from category  " + DB.GetNoLock() + " where deleted=0");
            NumSecs = DB.GetSqlN("Select count(*) as N from [section]  " + DB.GetNoLock() + " where deleted=0");
            NumAffs = DB.GetSqlN("Select count(*) as N from affiliate  " + DB.GetNoLock() + " where deleted=0");
            NumCL = DB.GetSqlN("Select count(*) as N from customerlevel  " + DB.GetNoLock() + " where deleted=0");
            NumGenre = DB.GetSqlN("Select count(*) as N from genre  " + DB.GetNoLock() + " where deleted=0");
            NumVector = DB.GetSqlN("Select count(*) as N from Vector  " + DB.GetNoLock() + " where deleted=0");
            //NumDist = DB.GetSqlN("Select count(*) as N from distributor  " + DB.GetNoLock());
            MaxCatMaps = AppLogic.AppConfigUSInt("MaxCatMaps");
            if (MaxCatMaps == 0)
            {
                MaxCatMaps = 5;
            }
            MM = CommonLogic.Min(CommonLogic.Max(CommonLogic.Max(NumCats, CommonLogic.Max(NumSecs, NumCL)), NumAffs), MaxCatMaps);
            ProductID = 0;

            if (CommonLogic.QueryStringCanBeDangerousContent("ProductID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("ProductID") != "0")
            {
                Editing = true;
                ProductID = Localization.ParseUSInt(CommonLogic.QueryStringCanBeDangerousContent("ProductID"));
            }
            else
            {
                Editing = false;
            }

            AppLogic.EnsureProductHasADefaultVariantSet(ProductID);

            IDataReader rs;

            pdesc = new ProductDescriptionFile(ProductID, ThisCustomer.LocaleSetting, SkinID);

            //int N = 0;
            if (CommonLogic.QueryStringBool("DeleteDescriptionFile"))
            {
                System.IO.File.Delete(pdesc.FN);
                Response.Redirect("editproduct.aspx?productid=" + ProductID.ToString());
            }

            if (CommonLogic.QueryStringBool("DeleteSpecFile"))
            {
                ProductSpecFile pspec = new ProductSpecFile(ProductID, SkinID);
                System.IO.File.Delete(pspec.FN);
                Response.Redirect("editproduct.aspx?productid=" + ProductID.ToString());
            }

            if (CommonLogic.QueryStringBool("DeleteAllVariants"))
            {
                DB.ExecuteSQL("delete from CustomCart where VariantID in (select VariantID from ProductVariant where ProductID=" + ProductID.ToString() + ")");
                DB.ExecuteSQL("delete from KitCart where VariantID in (select VariantID from ProductVariant where ProductID=" + ProductID.ToString() + ")");
                DB.ExecuteSQL("delete from ShoppingCart where VariantID in (select VariantID from ProductVariant where ProductID=" + ProductID.ToString() + ")");
                DB.ExecuteSQL("delete from ProductVariant where ProductID=" + ProductID.ToString()); ;
            }

            if (CommonLogic.FormBool("IsSubmit"))
            {
                StringBuilder sql = new StringBuilder(2500);
                String LargeProductImage = AppLogic.LookupImage("Product", ProductID, "large", SkinID, ThisCustomer.LocaleSetting);
                if (!Editing)
                {
                    // ok to add them:
                    String NewGUID = DB.GetNewGUID();
                    sql.Append("insert into product(ProductGUID,Name,SEName,ContentsBGColor,PageBGColor,GraphicsColor,AvailableStartDate,AvailableStopDate,ImageFilenameOverride,ProductTypeID,TaxClassID,Summary,Description,ExtensionData,ColorOptionPrompt,SizeOptionPrompt,RequiresTextOption,TextOptionPrompt,TextOptionMaxLength,FroogleDescription,RelatedProducts,UpsellProducts,UpsellProductDiscountPercentage,RequiresProducts,SEKeywords,SEDescription,SETitle,SENoScript,SKU,PageSize,ColWidth,XmlPackage,ManufacturerPartNumber,SalesPromptID,SpecTitle,SpecCall,Published,GoogleCheckoutAllowed,ShowBuyButton,IsCallToOrder,HidePriceUntilCart,ShowInProductBrowser,ExcludeFromPriceFeeds,IsAKit,IsAPack,PackSize,TrackInventoryBySizeAndColor,TrackInventoryBySize,TrackInventoryByColor,RequiresRegistration,SpecsInline,MiscText,SwatchImageMap,QuantityDiscountID) values(");
                    sql.Append(DB.SQuote(NewGUID) + ",");
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                    if (AppLogic.NumLocaleSettingsInstalled() > 1)
                    {
                        sql.Append(DB.SQuote(SE.MungeName(CommonLogic.FormCanBeDangerousContent("Name_" + Localization.GetWebConfigLocale().Replace("-", "_")))) + ",");
                    }
                    else
                    {
                        sql.Append(DB.SQuote(SE.MungeName(CommonLogic.FormCanBeDangerousContent("Name"))) + ",");
                    }
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("ContentsBGColor")) + ",");
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("PageBGColor")) + ",");
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("GraphicsColor")) + ",");
                    if (CommonLogic.FormCanBeDangerousContent("AvailableStartDate").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("AvailableStartDate")) + ",");
                    }
                    else
                    {
                        sql.Append("getdate(),");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("AvailableStopDate").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("AvailableStopDate")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("ImageFilenameOverride")) + ",");
                    sql.Append(CommonLogic.FormCanBeDangerousContent("ProductTypeID").ToString() + ",");
                    sql.Append(CommonLogic.FormCanBeDangerousContent("TaxClassID").ToString() + ",");
                    if (AppLogic.FormLocaleXml("Summary").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Summary")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (AppLogic.FormLocaleXml("Description").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Description")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("ExtensionData").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("ExtensionData")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (AppLogic.FormLocaleXml("ColorOptionPrompt").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("ColorOptionPrompt")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (AppLogic.FormLocaleXml("SizeOptionPrompt").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SizeOptionPrompt")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    sql.Append(CommonLogic.FormUSInt("RequiresTextOption").ToString() + ",");
                    if (CommonLogic.FormCanBeDangerousContent("TextOptionPrompt").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("TextOptionPrompt")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("TextOptionMaxLength").Length != 0)
                    {
                        sql.Append(CommonLogic.FormUSInt("TextOptionMaxLength").ToString() + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (AppLogic.FormLocaleXml("FroogleDescription").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("FroogleDescription")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("RelatedProducts").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("RelatedProducts").Trim().Replace(" ", "")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("UpsellProducts").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("UpsellProducts").Trim().Replace(" ", "")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    sql.Append(Localization.DecimalStringForDB(CommonLogic.FormUSDecimal("UpsellProductDiscountPercentage")) + ",");
                    if (CommonLogic.FormCanBeDangerousContent("RequiresProducts").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("RequiresProducts").Trim().Replace(" ", "")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (AppLogic.FormLocaleXml("SEKeywords").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SEKeywords")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (AppLogic.FormLocaleXml("SEDescription").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SEDescription")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (AppLogic.FormLocaleXml("SETitle").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SETitle")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (AppLogic.FormLocaleXml("SENoScript").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SENoScript")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    //if (AppLogic.FormLocaleXml("SEAltText").Length != 0)
                    //{
                    //    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SEAltText")) + ",");
                    //}
                    //else
                    //{
                    //    sql.Append("NULL,");
                    //}
                    if (CommonLogic.FormCanBeDangerousContent("SKU").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("SKU")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    sql.Append(CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("PageSize").Length == 0, AppLogic.AppConfig("Default_ProductPageSize"), CommonLogic.FormCanBeDangerousContent("PageSize")) + ",");
                    sql.Append(CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("ColWidth").Length == 0, AppLogic.AppConfig("Default_ProductColWidth"), CommonLogic.FormCanBeDangerousContent("ColWidth")) + ",");
                    if (CommonLogic.FormCanBeDangerousContent("XmlPackage").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("XmlPackage").ToLowerInvariant()) + ",");
                    }
                    else
                    {
                        if (CommonLogic.FormUSInt("IsAPack") == 1)
                        {
                            sql.Append(DB.SQuote(AppLogic.ro_DefaultProductPackXmlPackage) + ","); // force a default!
                        }
                        else if (CommonLogic.FormUSInt("IsAKit") == 1)
                        {
                            sql.Append(DB.SQuote(AppLogic.ro_DefaultProductKitXmlPackage) + ","); // force a default!
                        }
                        else
                        {
                            sql.Append(DB.SQuote(AppLogic.ro_DefaultProductXmlPackage) + ","); // force a default!
                        }
                    }
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("ManufacturerPartNumber")) + ",");
                    sql.Append(CommonLogic.FormUSInt("SalesPromptID") + ",");
                    if (AppLogic.FormLocaleXml("SpecTitle").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SpecTitle")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("SpecCall").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("SpecCall")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    sql.Append(CommonLogic.FormUSInt("Published").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("GoogleCheckoutAllowed").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("ShowBuyButton").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("IsCallToOrder").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("HidePriceUntilCart").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("ShowInProductBrowser").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("ExcludeFromPriceFeeds").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("IsAKit").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("IsAPack").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("PackSize").ToString() + ",");
                    if (CommonLogic.FormUSInt("IsAKit") == 1 || CommonLogic.FormUSInt("IsAPack") == 1)
                    {
                        sql.Append("0,"); // cannot track inventory by size and color
                        sql.Append("0,"); // cannot track inventory by size and color
                        sql.Append("0,"); // cannot track inventory by size and color
                    }
                    else
                    {
                        sql.Append(CommonLogic.FormUSInt("TrackInventoryBySizeAndColor").ToString() + ",");
                        sql.Append(CommonLogic.FormUSInt("TrackInventoryBySizeAndColor").ToString() + ","); // this is correct. change made in v6.1.1.1
                        sql.Append(CommonLogic.FormUSInt("TrackInventoryBySizeAndColor").ToString() + ","); // this is correct. change made in v6.1.1.1
                    }
                    sql.Append(CommonLogic.FormUSInt("RequiresRegistration").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("SpecsInline").ToString() + ",");
                    if (CommonLogic.FormCanBeDangerousContent("MiscText").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("MiscText")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("SwatchImageMap").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("SwatchImageMap")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    sql.Append(CommonLogic.FormCanBeDangerousContent("QuantityDiscountID"));
                    sql.Append(")");
                    DB.ExecuteSQL(sql.ToString());

                    rs = DB.GetRS("select ProductID from product  " + DB.GetNoLock() + " where deleted=0 and ProductGUID=" + DB.SQuote(NewGUID));
                    rs.Read();
                    ProductID = DB.RSFieldInt(rs, "ProductID");
                    Editing = true;
                    rs.Close();
                    DataUpdated = true;

                    // ARE WE ADDING A SIMPLE PRODUCT, IF SO, CREATE THE DEFAULT VARIANT
                    if (CommonLogic.FormCanBeDangerousContent("Price").Length != 0)
                    {
                        // ok to add:
                        NewGUID = DB.GetNewGUID();
                        sql.Remove(0, sql.Length);
                        sql.Append("insert into productvariant(VariantGUID,Name,IsDefault,ProductID,Price,SalePrice,Weight,Dimensions,Inventory,Published,Colors,ColorSKUModifiers,Sizes,SizeSKUModifiers) values(");
                        sql.Append(DB.SQuote(NewGUID) + ",");
                        sql.Append(DB.SQuote(String.Empty) + ","); // add empty variant name
                        sql.Append("1,"); // IsDefault=1
                        sql.Append(ProductID.ToString() + ",");
                        sql.Append(CommonLogic.FormCanBeDangerousContent("Price") + ",");
                        sql.Append(CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("SalePrice").Length != 0, CommonLogic.FormCanBeDangerousContent("SalePrice"), "NULL") + ",");
                        sql.Append(CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("Weight").Length != 0, CommonLogic.FormCanBeDangerousContent("Weight"), "NULL") + ",");
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("Dimensions")) + ",");
                        sql.Append(CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("Inventory").Length != 0, CommonLogic.FormCanBeDangerousContent("Inventory"), AppLogic.AppConfig("Admin_DefaultInventory")) + ",");
                        sql.Append("1,");
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("Colors")) + ",");
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("ColorSKUModifiers")) + ",");
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("Sizes")) + ",");
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("SizeSKUModifiers")));
                        sql.Append(")");
                        DB.ExecuteSQL(sql.ToString());
                    }
                }
                else
                {
                    // ok to update:
                    sql.Append("update product set ");
                    sql.Append("Name=" + DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                    sql.Append("SEName=" + DB.SQuote(SE.MungeName(CommonLogic.FormCanBeDangerousContent("Name_" + Localization.GetWebConfigLocale().Replace("-", "_")))) + ",");
                    sql.Append("ContentsBGColor=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ContentsBGColor")) + ",");
                    sql.Append("PageBGColor=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("PageBGColor")) + ",");
                    sql.Append("GraphicsColor=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("GraphicsColor")) + ",");
                    if (CommonLogic.FormCanBeDangerousContent("AvailableStartDate").Length != 0)
                    {
                        sql.Append("AvailableStartDate=" + DB.SQuote(Localization.ToDBShortDateString(Localization.ParseNativeDateTime(CommonLogic.FormCanBeDangerousContent("AvailableStartDate")))) + ",");
                    }
                    else
                    {
                        sql.Append("AvailableStartDate=getdate(),");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("AvailableStopDate").Length != 0)
                    {
                        sql.Append("AvailableStopDate=" + DB.SQuote(Localization.ToDBShortDateString(Localization.ParseNativeDateTime(CommonLogic.FormCanBeDangerousContent("AvailableStopDate")))) + ",");
                    }
                    else
                    {
                        sql.Append("AvailableStopDate=NULL,");
                    }
                    sql.Append("ImageFilenameOverride=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ImageFilenameOverride")) + ",");
                    sql.Append("ProductTypeID=" + CommonLogic.FormUSInt("ProductTypeID") + ",");
                    sql.Append("TaxClassID=" + CommonLogic.FormUSInt("TaxClassID") + ",");
                    if (AppLogic.FormLocaleXml("Summary").Length != 0)
                    {
                        sql.Append("Summary=" + DB.SQuote(AppLogic.FormLocaleXml("Summary")) + ",");
                    }
                    else
                    {
                        sql.Append("Summary=NULL,");
                    }
                    if (AppLogic.FormLocaleXml("Description").Length != 0)
                    {
                        sql.Append("Description=" + DB.SQuote(AppLogic.FormLocaleXml("Description")) + ",");
                    }
                    else
                    {
                        sql.Append("Description=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("ExtensionData").Length != 0)
                    {
                        sql.Append("ExtensionData=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ExtensionData")) + ",");
                    }
                    else
                    {
                        sql.Append("ExtensionData=NULL,");
                    }
                    if (AppLogic.FormLocaleXml("ColorOptionPrompt").Length != 0)
                    {
                        sql.Append("ColorOptionPrompt=" + DB.SQuote(AppLogic.FormLocaleXml("ColorOptionPrompt")) + ",");
                    }
                    else
                    {
                        sql.Append("ColorOptionPrompt=NULL,");
                    }
                    if (AppLogic.FormLocaleXml("SizeOptionPrompt").Length != 0)
                    {
                        sql.Append("SizeOptionPrompt=" + DB.SQuote(AppLogic.FormLocaleXml("SizeOptionPrompt")) + ",");
                    }
                    else
                    {
                        sql.Append("SizeOptionPrompt=NULL,");
                    }
                    sql.Append("RequiresTextOption=" + CommonLogic.FormUSInt("RequiresTextOption") + ",");
                    if (AppLogic.FormLocaleXml("TextOptionPrompt").Length != 0)
                    {
                        sql.Append("TextOptionPrompt=" + DB.SQuote(AppLogic.FormLocaleXml("TextOptionPrompt")) + ",");
                    }
                    else
                    {
                        sql.Append("TextOptionPrompt=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("TextOptionMaxLength").Length != 0)
                    {
                        sql.Append("TextOptionMaxLength=" + CommonLogic.FormUSInt("TextOptionMaxLength").ToString() + ",");
                    }
                    else
                    {
                        sql.Append("TextOptionMaxLength=NULL,");
                    }
                    if (AppLogic.FormLocaleXml("FroogleDescription").Length != 0)
                    {
                        sql.Append("FroogleDescription=" + DB.SQuote(AppLogic.FormLocaleXml("FroogleDescription")) + ",");
                    }
                    else
                    {
                        sql.Append("FroogleDescription=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("RelatedProducts").Length != 0)
                    {
                        sql.Append("RelatedProducts=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("RelatedProducts").Trim().Replace(" ", "")) + ",");
                    }
                    else
                    {
                        sql.Append("RelatedProducts=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("UpsellProducts").Length != 0)
                    {
                        sql.Append("UpsellProducts=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("UpsellProducts").Trim().Replace(" ", "")) + ",");
                    }
                    else
                    {
                        sql.Append("UpsellProducts=NULL,");
                    }
                    sql.Append("UpsellProductDiscountPercentage=" + Localization.DecimalStringForDB(CommonLogic.FormUSDecimal("UpsellProductDiscountPercentage")) + ",");
                    if (CommonLogic.FormCanBeDangerousContent("RequiresProducts").Length != 0)
                    {
                        sql.Append("RequiresProducts=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("RequiresProducts").Trim().Replace(" ", "")) + ",");
                    }
                    else
                    {
                        sql.Append("RequiresProducts=NULL,");
                    }
                    if (AppLogic.FormLocaleXml("SEKeywords").Length != 0)
                    {
                        sql.Append("SEKeywords=" + DB.SQuote(AppLogic.FormLocaleXml("SEKeywords")) + ",");
                    }
                    else
                    {
                        sql.Append("SEKeywords=NULL,");
                    }
                    if (AppLogic.FormLocaleXml("SEDescription").Length != 0)
                    {
                        sql.Append("SEDescription=" + DB.SQuote(AppLogic.FormLocaleXml("SEDescription")) + ",");
                    }
                    else
                    {
                        sql.Append("SEDescription=NULL,");
                    }
                    if (AppLogic.FormLocaleXml("SETitle").Length != 0)
                    {
                        sql.Append("SETitle=" + DB.SQuote(AppLogic.FormLocaleXml("SETitle")) + ",");
                    }
                    else
                    {
                        sql.Append("SETitle=NULL,");
                    }
                    if (AppLogic.FormLocaleXml("SENoScript").Length != 0)
                    {
                        sql.Append("SENoScript=" + DB.SQuote(AppLogic.FormLocaleXml("SENoScript")) + ",");
                    }
                    else
                    {
                        sql.Append("SENoScript=NULL,");
                    }
                    //if (AppLogic.FormLocaleXml("SEAltText").Length != 0)
                    //{
                    //    sql.Append("SEAltText=" + DB.SQuote(AppLogic.FormLocaleXml("SEAltText")) + ",");
                    //}
                    //else
                    //{
                    //    sql.Append("SEAltText=NULL,");
                    //}
                    if (CommonLogic.FormCanBeDangerousContent("SKU").Length != 0)
                    {
                        sql.Append("SKU=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("SKU")) + ",");
                    }
                    else
                    {
                        sql.Append("SKU=NULL,");
                    }
                    sql.Append("PageSize=" + CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("PageSize").Length == 0, AppLogic.AppConfig("Default_ProductPageSize"), CommonLogic.FormCanBeDangerousContent("PageSize")) + ",");
                    sql.Append("ColWidth=" + CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("ColWidth").Length == 0, AppLogic.AppConfig("Default_ProductColWidth"), CommonLogic.FormCanBeDangerousContent("ColWidth")) + ",");
                    if (CommonLogic.FormCanBeDangerousContent("XmlPackage").Length != 0)
                    {
                        sql.Append("XmlPackage=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("XmlPackage").ToLowerInvariant()) + ",");
                    }
                    else
                    {
                        if (CommonLogic.FormUSInt("IsAPack") == 1)
                        {
                            sql.Append("XmlPackage=" + DB.SQuote(AppLogic.ro_DefaultProductPackXmlPackage) + ","); // force a default!
                        }
                        else if (CommonLogic.FormUSInt("IsAKit") == 1)
                        {
                            sql.Append("XmlPackage=" + DB.SQuote(AppLogic.ro_DefaultProductKitXmlPackage) + ","); // force a default!
                        }
                        else
                        {
                            sql.Append("XmlPackage=" + DB.SQuote(AppLogic.ro_DefaultProductXmlPackage) + ","); // force a default!
                        }
                    }
                    sql.Append("ManufacturerPartNumber=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ManufacturerPartNumber")) + ",");
                    sql.Append("SalesPromptID=" + CommonLogic.FormUSInt("SalesPromptID") + ",");
                    if (AppLogic.FormLocaleXml("SpecTitle").Length != 0)
                    {
                        sql.Append("SpecTitle=" + DB.SQuote(AppLogic.FormLocaleXml("SpecTitle")) + ",");
                    }
                    else
                    {
                        sql.Append("SpecTitle=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("SpecCall").Length != 0)
                    {
                        sql.Append("SpecCall=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("SpecCall")) + ",");
                    }
                    else
                    {
                        sql.Append("SpecCall=NULL,");
                    }
                    sql.Append("Published=" + CommonLogic.FormUSInt("Published") + ",");
                    sql.Append("GoogleCheckoutAllowed=" + CommonLogic.FormUSInt("GoogleCheckoutAllowed") + ",");
                    sql.Append("ShowBuyButton=" + CommonLogic.FormUSInt("ShowBuyButton") + ",");
                    sql.Append("IsCallToOrder=" + CommonLogic.FormUSInt("IsCallToOrder") + ",");
                    sql.Append("HidePriceUntilCart=" + CommonLogic.FormUSInt("HidePriceUntilCart") + ",");
                    sql.Append("ShowInProductBrowser=" + CommonLogic.FormUSInt("ShowInProductBrowser") + ",");
                    sql.Append("ExcludeFromPriceFeeds=" + CommonLogic.FormUSInt("ExcludeFromPriceFeeds") + ",");
                    sql.Append("IsAKit=" + CommonLogic.FormUSInt("IsAKit") + ",");
                    sql.Append("IsAPack=" + CommonLogic.FormUSInt("IsAPack") + ",");
                    sql.Append("PackSize=" + CommonLogic.FormUSInt("PackSize").ToString() + ",");
                    if (CommonLogic.FormUSInt("IsAKit") == 1 || CommonLogic.FormUSInt("IsAPack") == 1)
                    {
                        sql.Append("TrackInventoryBySizeAndColor=0,"); // cannot track inventory by size and color
                        sql.Append("TrackInventoryBySize=0,");
                        sql.Append("TrackInventoryByColor=0,");
                    }
                    else
                    {
                        sql.Append("TrackInventoryBySizeAndColor=" + CommonLogic.FormCanBeDangerousContent("TrackInventoryBySizeAndColor") + ",");
                        sql.Append("TrackInventoryBySize=" + CommonLogic.FormCanBeDangerousContent("TrackInventoryBySizeAndColor") + ","); // this is correct. change made in v6.1.1.1
                        sql.Append("TrackInventoryByColor=" + CommonLogic.FormCanBeDangerousContent("TrackInventoryBySizeAndColor") + ","); // this is correct. change made in v6.1.1.1
                    }
                    sql.Append("RequiresRegistration=" + CommonLogic.FormCanBeDangerousContent("RequiresRegistration") + ",");
                    sql.Append("SpecsInline=" + CommonLogic.FormCanBeDangerousContent("SpecsInline") + ",");
                    if (CommonLogic.FormCanBeDangerousContent("MiscText").Length != 0)
                    {
                        sql.Append("MiscText=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("MiscText")) + ",");
                    }
                    else
                    {
                        sql.Append("MiscText=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("SwatchImageMap").Length != 0)
                    {
                        sql.Append("SwatchImageMap=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("SwatchImageMap")) + ",");
                    }
                    else
                    {
                        sql.Append("SwatchImageMap=NULL,");
                    }
                    sql.Append("QuantityDiscountID=" + CommonLogic.FormCanBeDangerousContent("QuantityDiscountID"));
                    sql.Append(" where ProductID=" + ProductID.ToString());
                    DB.ExecuteSQL(sql.ToString());
                    DataUpdated = true;
                    Editing = true;
                }

                // handle image uploaded:
                String FN = String.Empty;
                if (AppLogic.AppConfigBool("UseSKUForProductImageName"))
                {
                    FN = CommonLogic.FormCanBeDangerousContent("SKU").Trim();
                }
                if (CommonLogic.FormCanBeDangerousContent("ImageFilenameOverride").Trim().Length != 0)
                {
                    FN = CommonLogic.FormCanBeDangerousContent("ImageFilenameOverride").Trim();
                }
                if (FN.Length == 0)
                {
                    FN = ProductID.ToString();
                }
                //					try
                //					{
                String Image1 = String.Empty;
                HttpPostedFile Image1File = Request.Files["Image1"];
                if (Image1File != null && Image1File.ContentLength != 0)
                {
                    // delete any current image file first
                    try
                    {
                        if (FN.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) || FN.EndsWith(".gif", StringComparison.InvariantCultureIgnoreCase) || FN.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
                        {
                            System.IO.File.Delete(AppLogic.GetImagePath("Product", "icon", true) + FN);
                        }
                        else
                        {
                            System.IO.File.Delete(AppLogic.GetImagePath("Product", "icon", true) + FN + ".jpg");
                            System.IO.File.Delete(AppLogic.GetImagePath("Product", "icon", true) + FN + ".gif");
                            System.IO.File.Delete(AppLogic.GetImagePath("Product", "icon", true) + FN + ".png");
                        }
                    }
                    catch
                    { }

                    String s = Image1File.ContentType;
                    switch (Image1File.ContentType)
                    {
                        case "image/gif":
                            Image1 = AppLogic.GetImagePath("Product", "icon", true) + FN + ".gif";
                            Image1File.SaveAs(Image1);
                            break;
                        case "image/x-png":
                            Image1 = AppLogic.GetImagePath("Product", "icon", true) + FN + ".png";
                            Image1File.SaveAs(Image1);
                            break;
                        case "image/jpg":
                        case "image/jpeg":
                        case "image/pjpeg":
                            Image1 = AppLogic.GetImagePath("Product", "icon", true) + FN + ".jpg";
                            Image1File.SaveAs(Image1);
                            break;
                    }
                }

                String Image2 = String.Empty;
                HttpPostedFile Image2File = Request.Files["Image2"];
                if (Image2File != null && Image2File.ContentLength != 0)
                {
                    // delete any current image file first
                    try
                    {
                        System.IO.File.Delete(AppLogic.GetImagePath("Product", "medium", true) + FN + ".jpg");
                        System.IO.File.Delete(AppLogic.GetImagePath("Product", "medium", true) + FN + ".gif");
                        System.IO.File.Delete(AppLogic.GetImagePath("Product", "medium", true) + FN + ".png");
                    }
                    catch
                    { }

                    String s = Image2File.ContentType;
                    switch (Image2File.ContentType)
                    {
                        case "image/gif":
                            Image2 = AppLogic.GetImagePath("Product", "medium", true) + FN + ".gif";
                            Image2File.SaveAs(Image2);
                            break;
                        case "image/x-png":
                            Image2 = AppLogic.GetImagePath("Product", "medium", true) + FN + ".png";
                            Image2File.SaveAs(Image2);
                            break;
                        case "image/jpg":
                        case "image/jpeg":
                        case "image/pjpeg":
                            Image2 = AppLogic.GetImagePath("Product", "medium", true) + FN + ".jpg";
                            Image2File.SaveAs(Image2);
                            break;
                    }
                }

                String Image3 = String.Empty;
                HttpPostedFile Image3File = Request.Files["Image3"];
                if (Image3File != null && Image3File.ContentLength != 0)
                {
                    // delete any current image file first
                    try
                    {
                        System.IO.File.Delete(AppLogic.GetImagePath("Product", "large", true) + FN + ".jpg");
                        System.IO.File.Delete(AppLogic.GetImagePath("Product", "large", true) + FN + ".gif");
                        System.IO.File.Delete(AppLogic.GetImagePath("Product", "large", true) + FN + ".png");
                    }
                    catch
                    { }

                    String s = Image3File.ContentType;
                    switch (Image3File.ContentType)
                    {
                        case "image/gif":
                            Image3 = AppLogic.GetImagePath("Product", "large", true) + FN + ".gif";
                            Image3File.SaveAs(Image3);
                            break;
                        case "image/x-png":
                            Image3 = AppLogic.GetImagePath("Product", "large", true) + FN + ".png";
                            Image3File.SaveAs(Image3);
                            break;
                        case "image/jpg":
                        case "image/jpeg":
                        case "image/pjpeg":
                            Image3 = AppLogic.GetImagePath("Product", "large", true) + FN + ".jpg";
                            Image3File.SaveAs(Image3);
                            break;
                    }
                }

                // color swatch image
                String Image4 = String.Empty;
                HttpPostedFile Image4File = Request.Files["Image4"];
                if (Image4File.ContentLength != 0)
                {
                    // delete any current image file first
                    try
                    {
                        System.IO.File.Delete(AppLogic.GetImagePath("Product", "swatch", true) + FN + ".jpg");
                        System.IO.File.Delete(AppLogic.GetImagePath("Product", "swatch", true) + FN + ".gif");
                        System.IO.File.Delete(AppLogic.GetImagePath("Product", "swatch", true) + FN + ".png");
                    }
                    catch
                    { }

                    String s = Image4File.ContentType;
                    switch (Image4File.ContentType)
                    {
                        case "image/gif":
                            Image4 = AppLogic.GetImagePath("Product", "swatch", true) + FN + ".gif";
                            Image4File.SaveAs(Image4);
                            break;
                        case "image/x-png":
                            Image4 = AppLogic.GetImagePath("Product", "swatch", true) + FN + ".png";
                            Image4File.SaveAs(Image4);
                            break;
                        case "image/jpg":
                        case "image/jpeg":
                        case "image/pjpeg":
                            Image4 = AppLogic.GetImagePath("Product", "swatch", true) + FN + ".jpg";
                            Image4File.SaveAs(Image4);
                            break;
                    }
                }

                // UPDATE 1:1 ENTITY MAPPINGS:
                {
                    String[] Entities = { "Manufacturer", "Distributor" };
                    foreach (String en in Entities)
                    {
                        int NewID = CommonLogic.FormUSInt(en + "ID");
                        if (NewID == 0)
                        {
                            // no mapping (should not be allowed by form validator, but...)
                            DB.ExecuteSQL("delete from Product" + en + " where ProductID=" + ProductID.ToString());
                        }
                        else
                        {
                            int OldID = CommonLogic.IIF(en == "Manufacturer", AppLogic.GetProductManufacturerID(ProductID), AppLogic.GetProductDistributorID(ProductID));
                            if (OldID == 0)
                            {
                                // create default mapping:
                                DB.ExecuteSQL(String.Format("insert into Product{0}(ProductID,{1}ID,DisplayOrder) values({2},{3},1)", en, en, ProductID.ToString(), NewID.ToString()));
                            }
                            else if (OldID != NewID)
                            {
                                // update existing mapping:
                                DB.ExecuteSQL(String.Format("update Product{0} set {1}ID={2} where {3}ID={4} and ProductID={5}", en, en, NewID.ToString(), en, OldID.ToString(), ProductID.ToString()));
                            }
                        }
                    }
                }

                // UPDATE 1:N ENTITY MAPPINGS:
                {
                    String[] Entities2 = { "Category", "Section", "Affiliate", "CustomerLevel", "Genre", "Vector" };
                    foreach (String en in Entities2)
                    {
                        String EnMap = CommonLogic.FormCanBeDangerousContent(en + "Map");
                        if (EnMap.Length == 0)
                        {
                            // no mappings
                            DB.ExecuteSQL(String.Format("delete from Product{0} where ProductID={1}", en, ProductID.ToString()));
                        }
                        else
                        {
                            // remove any mappings not current anymore:
                            DB.ExecuteSQL(String.Format("delete from Product{0} where ProductID={1} and {2}ID not in ({3})", en, ProductID.ToString(), en, EnMap));
                            // add new default mappings:
                            String[] EnMapArray = EnMap.Split(',');
                            foreach (String EntityID in EnMapArray)
                            {
                                try
                                {
                                    DB.ExecuteSQL(String.Format("insert Product{0}(ProductID,{1}ID,DisplayOrder) values({2},{3},1)", en, en, ProductID.ToString(), EntityID));
                                }
                                catch { }
                            }
                        }
                    }
                }
            }

            SectionTitle = "<a href=\"products.aspx\">Products</a> - Manage Products";
            if (CommonLogic.QueryStringCanBeDangerousContent("DeleteAllVariants").ToUpperInvariant() != "TRUE")
            {
                AppLogic.MakeSureProductHasAtLeastOneVariant(ProductID);
            }
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            bool ProductTracksInventoryBySizeAndColor = AppLogic.ProductTracksInventoryBySizeAndColor(ProductID);
            bool IsAKit = AppLogic.IsAKit(ProductID);
            bool IsAPack = AppLogic.IsAPack(ProductID);
            if (IsAKit || IsAPack)
            {
                ProductTracksInventoryBySizeAndColor = false;
            }
            ProductSpecFile pspec = new ProductSpecFile(ProductID, SkinID);

            writer.Write(CommonLogic.ReadFile("jscripts/tabs.js", true));

            IDataReader rs = DB.GetRS("select * from Product  " + DB.GetNoLock() + " where ProductID=" + ProductID.ToString());
            if (rs.Read())
            {
                Editing = true;
            }

            if (ErrorMsg.Length != 0)
            {
                writer.Write("<p><b><font color=red>" + ErrorMsg + "</font></b></p>\n");
            }

            if (DataUpdated)
            {
                writer.Write("<p align=\"left\"><b><font color=blue>(UPDATED)</font></b></p>\n");
            }

            writer.Write("  <!-- calendar stylesheet -->\n");
            writer.Write("  <link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"jscalendar/calendar-win2k-cold-1.css\" title=\"win2k-cold-1\" />\n");
            writer.Write("\n");
            writer.Write("  <!-- main calendar program -->\n");
            writer.Write("  <script type=\"text/javascript\" src=\"jscalendar/calendar.js\"></script>\n");
            writer.Write("\n");
            writer.Write("  <!-- language for the calendar -->\n");
            writer.Write("  <script type=\"text/javascript\" src=\"jscalendar/lang/" + Localization.JSCalendarLanguageFile() + "\"></script>\n");
            writer.Write("\n");
            writer.Write("  <!-- the following script defines the Calendar.setup helper function, which makes\n");
            writer.Write("       adding a calendar a matter of 1 or 2 lines of code. -->\n");
            writer.Write("  <script type=\"text/javascript\" src=\"jscalendar/calendar-setup.js\"></script>\n");


            int ManufacturerID = CommonLogic.QueryStringUSInt("ManufacturerID");
            int DistributorID = CommonLogic.QueryStringUSInt("DistributorID");
            int CategoryID = CommonLogic.QueryStringUSInt("CategoryID");
            int SectionID = CommonLogic.QueryStringUSInt("SectionID");

            if (CategoryID == 0 && SectionID == 0)
            {
                CategoryID = CommonLogic.CookieUSInt("CategoryID");
                if (CategoryID == 0)
                {
                    SectionID = CommonLogic.CookieUSInt("SectionID");
                }
            }

            EntityHelper CategoryHelper = AppLogic.LookupHelper(base.EntityHelpers, "Category");
            EntityHelper SectionHelper = AppLogic.LookupHelper(base.EntityHelpers, "Section");
            EntityHelper AffiliateHelper = AppLogic.LookupHelper(base.EntityHelpers, "Affiliate");

            String ProductCategories = CategoryHelper.GetObjectEntities(ProductID, false);
            String ProductSections = SectionHelper.GetObjectEntities(ProductID, false);
            String ProductAffiliates = AffiliateHelper.GetObjectEntities(ProductID, false);
            String ProductCustomerLevels = AppLogic.GetProductCustomerLevels(ProductID);
            String ProductGenres = AppLogic.GetProductGenres(ProductID);
            String ProductVectors = AppLogic.GetProductVectors(ProductID);
            //String ProductDistributors = AppLogic.GetProductDistributors(ProductID);
            String[] Cats = ProductCategories.Split(',');
            String[] Secs = ProductSections.Split(',');
            String[] Affs = ProductAffiliates.Split(',');
            String[] CLs = ProductCustomerLevels.Split(',');
            String[] GLs = ProductGenres.Split(',');
            String[] SLs = ProductVectors.Split(',');
            //String[] Dists = ProductDistributors.Split(',');

            if (ErrorMsg.Length == 0)
            {

                if (Editing)
                {
                    writer.Write("<p align=\"left\"><b>Editing Product: " + DB.RSFieldByLocale(rs, "Name", ThisCustomer.LocaleSetting) + " (Product SKU=" + DB.RSField(rs, "SKU") + ", ProductID=" + DB.RSFieldInt(rs, "ProductID").ToString() + ")");
                    if (CategoryID == 0 && SectionID == 0)
                    {
                        if (ProductCategories.Length > 0)
                        {
                            CategoryID = Localization.ParseUSInt(Cats[0]);
                        }
                    }
                    int NumProducts = CommonLogic.IIF(CategoryID != 0, CategoryHelper.GetNumEntityObjects(CategoryID, true, true), SectionHelper.GetNumEntityObjects(SectionID, true, true));
                    writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;");
                    if (NumProducts > 1)
                    {
                        int PreviousProductID = AppLogic.GetPreviousProduct(ProductID, CategoryID, SectionID, 0, 0, false, true, true);
                        if (CategoryID != 0)
                        {
                            writer.Write("<a class=\"ProductNavLink\" href=\"editproduct.aspx?productid=" + PreviousProductID.ToString() + "&categoryID=" + CategoryID.ToString() + "\"><img src=\"skins/skin_" + SkinID.ToString() + "/images/previous.gif\" border=\"0\" align=\"absmiddle\"></a>&nbsp;&nbsp;");
                        }
                        else
                        {
                            writer.Write("<a class=\"ProductNavLink\" href=\"editproduct.aspx?productid=" + PreviousProductID.ToString() + "&SectionID=" + SectionID.ToString() + "\"><img src=\"skins/skin_" + SkinID.ToString() + "/images/previous.gif\" border=\"0\" align=\"absmiddle\"></a>&nbsp;&nbsp;");
                        }
                    }
                    writer.Write("<a class=\"ProductNavLink\" href=\"products.aspx\"><img src=\"skins/skin_" + SkinID.ToString() + "/images/up.gif\" border=\"0\" align=\"absmiddle\"></a>");
                    if (NumProducts > 1)
                    {
                        int NextProductID = AppLogic.GetNextProduct(ProductID, CategoryID, SectionID, 0, 0, false, true, true);
                        if (CategoryID != 0)
                        {
                            writer.Write("&nbsp;&nbsp;<a class=\"ProductNavLink\" href=\"editproduct.aspx?productid=" + NextProductID.ToString() + "&categoryID=" + CategoryID.ToString() + "\"><img src=\"skins/skin_" + SkinID.ToString() + "/images/next.gif\" border=\"0\" align=\"absmiddle\"></a>");
                        }
                        else
                        {
                            writer.Write("&nbsp;&nbsp;<a class=\"ProductNavLink\" href=\"editproduct.aspx?productid=" + NextProductID.ToString() + "&sectionID=" + SectionID.ToString() + "\"><img src=\"skins/skin_" + SkinID.ToString() + "/images/next.gif\" border=\"0\" align=\"absmiddle\"></a>");
                        }
                    }
                    writer.Write("</p>\n");
                    writer.Write("<input style=\"font-size: 9px;\" type=\"button\" value=\"Show/Edit Variants\" name=\"ShowVariants_" + ProductID.ToString() + "\" onClick=\"self.location='variants.aspx?productid=" + ProductID.ToString() + "'\">");
                    if (!DB.RSFieldBool(rs, "IsSystem"))
                    {
                        writer.Write("&nbsp;&nbsp;");
                        writer.Write("<input style=\"font-size: 9px;\" type=\"button\" value=\"Delete All Variants\" name=\"DeleteAllVariants_" + ProductID.ToString() + "\" onClick=\"if(confirm('Are you sure you want to delete ALL variants for this product? This will also remove all items from customer shopping carts, wish lists, and gift registries IF they reference any of these variants!')){self.location='editproduct.aspx?DeleteAllVariants=true&productid=" + ProductID.ToString() + "';}\">");
                        writer.Write("&nbsp;&nbsp;");
                        writer.Write("<input style=\"font-size: 9px;\" type=\"button\" value=\"Clone Product\" name=\"Clone_" + ProductID.ToString() + "\" onClick=\"if(confirm('Are you sure you want to create a clone of this product, and all variants?')) {self.location='cloneproduct.aspx?productid=" + ProductID.ToString() + "';}\">\n");
                    }
                    if (!DB.RSFieldBool(rs, "Deleted"))
                    {

                        writer.Write("&nbsp;&nbsp;<input style=\"font-size: 9px;\" type='button' value='Delete Product' onclick=\"if(confirm('Are you sure you want to delete this product?')) {self.location='products.aspx?DeleteID=" + ProductID.ToString() + "'}\" />");

                    }
                    if (ProductTracksInventoryBySizeAndColor)
                    {
                        writer.Write("&nbsp;&nbsp;");
                        writer.Write("<input style=\"font-size: 9px;\" type=\"button\" value=\"Edit Inventory\" name=\"EditInventory_" + ProductID.ToString() + "\"  onClick=\"self.location='editinventory.aspx?productid=" + ProductID.ToString() + "'\">");
                    }
                    writer.Write("</b>");
                }
                else
                {
                    writer.Write("<p align=\"left\"><b>Adding New Product:</p></b>\n");
                }

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function Form_Validator(theForm)\n");
                writer.Write("{\n");
                writer.Write("submitonce(theForm);\n");
                writer.Write("if (theForm.SalesPromptID.selectedIndex < 1)\n");
                writer.Write("{\n");
                writer.Write("alert(\"Please select a sales prompt to use.\");\n");
                writer.Write("theForm.SalesPromptID.focus();\n");
                writer.Write("submitenabled(theForm);\n");
                writer.Write("return (false);\n");
                writer.Write("    }\n");
                writer.Write("if (theForm.ProductTypeID.selectedIndex < 1)\n");
                writer.Write("{\n");
                writer.Write("alert(\"Please select a product type.\");\n");
                writer.Write("theForm.ProductTypeID.focus();\n");
                writer.Write("submitenabled(theForm);\n");
                writer.Write("return (false);\n");
                writer.Write("    }\n");
                writer.Write("if (theForm.ManufacturerID.selectedIndex < 1)\n");
                writer.Write("{\n");
                writer.Write("alert(\"Please select a manufacturer.\");\n");
                writer.Write("theForm.ManufacturerID.focus();\n");
                writer.Write("submitenabled(theForm);\n");
                writer.Write("return (false);\n");
                writer.Write("    }\n");
                writer.Write("return (true);\n");
                writer.Write("}\n");
                writer.Write("</script>\n");

                writer.Write("<p>Please enter the following information about this product. Fields marked with an asterisk (*) are required. All other fields are optional.</p>\n");
                writer.Write("<form enctype=\"multipart/form-data\" action=\"editproduct.aspx?ProductID=" + ProductID.ToString() + "&edit=" + Editing.ToString() + "&manufacturerid=" + CommonLogic.QueryStringCanBeDangerousContent("ManufacturerID") + "&distributorid=" + CommonLogic.QueryStringCanBeDangerousContent("DistributorID") + "&categoryid=" + CategoryID.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(document.forms[0]) && Form_Validator(document.forms[0]))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"100%\" colspan=\"2\" align=\"left\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("<tr>\n");
                writer.Write("<td></td><td align=\"left\" valign=\"top\"><br/>\n");
                if (Editing)
                {
                    writer.Write("<input type=\"submit\" value=\"Update\" name=\"submit\">\n");
                    writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"reset\" class=\"CPButton\" value=\"Reset\" name=\"reset\">\n");
                }
                else
                {
                    writer.Write("<input type=\"submit\" value=\"Add New\" name=\"submit\">\n");
                }
                writer.Write("        </td>\n");
                writer.Write("      </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Product Name:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                //				String PName = String.Empty;
                //				if(Editing)
                //				{
                //					PName = Server.HtmlEncode(DB.RSField(rs,"Name"));
                //				}
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Name"), "Name", false, true, true, "Please enter the product name", 100, 50, 0, 0, false));
                //        writer.Write("                	<input maxLength=\"100\" size=\"50\" name=\"Name\" value=\"" + CommonLogic.IIF(Editing , PName , "") + "\">\n");
                //				writer.Write("                	<input type=\"hidden\" name=\"Name_vldt\" value=\"[req][blankalert=Please enter the product name]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Product Type:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("<select size=\"1\" name=\"ProductTypeID\">\n");
                writer.Write(" <OPTION VALUE=\"0\">SELECT ONE</option>\n");
                IDataReader rsst = DB.GetRS("select * from ProductType  " + DB.GetNoLock() + " order by DisplayOrder,Name");
                while (rsst.Read())
                {
                    writer.Write("<option value=\"" + DB.RSFieldInt(rsst, "ProductTypeID").ToString() + "\"");
                    if (Editing)
                    {
                        if (DB.RSFieldInt(rs, "ProductTypeID") == DB.RSFieldInt(rsst, "ProductTypeID"))
                        {
                            writer.Write(" selected");
                        }
                    }
                    else
                    {
                        if (DB.RSFieldInt(rsst, "ProductTypeID") == AppLogic.AppConfigUSInt("Admin_DefaultProductTypeID"))
                        {
                            writer.Write(" selected");
                        }
                    }
                    writer.Write(">" + DB.RSFieldByLocale(rsst, "Name", ThisCustomer.LocaleSetting) + "</option>");
                }
                rsst.Close();
                writer.Write("</select>\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Tax Class:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("<select size=\"1\" name=\"TaxClassID\">\n");
                rsst = DB.GetRS("select * from TaxClass " + DB.GetNoLock() + " order by DisplayOrder,Name");
                while (rsst.Read())
                {
                    writer.Write("<option value=\"" + DB.RSFieldInt(rsst, "TaxClassID").ToString() + "\"");
                    if (Editing)
                    {
                        if (DB.RSFieldInt(rs, "TaxClassID") == DB.RSFieldInt(rsst, "TaxClassID"))
                        {
                            writer.Write(" selected");
                        }
                    }
                    else
                    {
                        if (DB.RSFieldInt(rsst, "TaxClassID") == AppLogic.AppConfigUSInt("Admin_DefaultTaxClassID"))
                        {
                            writer.Write(" selected");
                        }
                    }
                    writer.Write(">" + DB.RSFieldByLocale(rsst, "Name", ThisCustomer.LocaleSetting) + "</option>");
                }
                rsst.Close();
                writer.Write("</select>\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Manufacturer:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("<select size=\"1\" name=\"ManufacturerID\">\n");
                writer.Write(" <OPTION VALUE=\"0\" " + CommonLogic.IIF(!Editing && ManufacturerID == 0, " selected ", "") + ">SELECT ONE</option>\n");
                int ThisMfgID = AppLogic.GetProductManufacturerID(ProductID);
                rsst = DB.GetRS("select * from Manufacturer " + DB.GetNoLock() + " where deleted=0 order by DisplayOrder,Name");
                while (rsst.Read())
                {
                    writer.Write("<option value=\"" + DB.RSFieldInt(rsst, "ManufacturerID").ToString() + "\"");
                    if (Editing)
                    {
                        if (ThisMfgID == DB.RSFieldInt(rsst, "ManufacturerID"))
                        {
                            writer.Write(" selected");
                        }
                    }
                    else
                    {
                        if (ManufacturerID == DB.RSFieldInt(rsst, "ManufacturerID"))
                        {
                            writer.Write(" selected");
                        }
                    }
                    writer.Write(">" + DB.RSFieldByLocale(rsst, "Name", ThisCustomer.LocaleSetting) + "</option>");
                }
                rsst.Close();
                writer.Write("</select>\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">Distributor:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("<select size=\"1\" name=\"DistributorID\">\n");
                writer.Write(" <OPTION VALUE=\"0\" " + CommonLogic.IIF(!Editing && DistributorID == 0, " selected ", "") + ">SELECT ONE</option>\n");
                int ThisDistID = AppLogic.GetProductDistributorID(ProductID);
                rsst = DB.GetRS("select * from Distributor  " + DB.GetNoLock() + " where Deleted=0 order by DisplayOrder,Name");
                while (rsst.Read())
                {
                    writer.Write("<option value=\"" + DB.RSFieldInt(rsst, "DistributorID").ToString() + "\"");
                    if (Editing)
                    {
                        if (ThisDistID == DB.RSFieldInt(rsst, "DistributorID"))
                        {
                            writer.Write(" selected");
                        }
                    }
                    else
                    {
                        if (DistributorID == DB.RSFieldInt(rsst, "DistributorID"))
                        {
                            writer.Write(" selected");
                        }
                    }
                    writer.Write(">" + DB.RSFieldByLocale(rsst, "Name", ThisCustomer.LocaleSetting) + "</option>");
                }
                rsst.Close();
                writer.Write("</select>\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">SKU:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"50\" size=\"50\" name=\"SKU\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "SKU")), "") + "\">\n");
                //writer.Write("                	<input type=\"hidden\" name=\"SKU_vldt\" value=\"[req][blankalert=Please enter the product SKU]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Manufacturer Part #:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"50\" size=\"50\" name=\"ManufacturerPartNumber\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "ManufacturerPartNumber")), "") + "\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\" bgcolor=\"" + CommonLogic.IIF(Editing && !DB.RSFieldBool(rs, "Published"), "#FFFFCC", "FFFFFF") + "\">*Published:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" bgcolor=\"" + CommonLogic.IIF(Editing && !DB.RSFieldBool(rs, "Published"), "#FFFFCC", "FFFFFF") + "\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"Published\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "Published"), " checked ", ""), " checked ") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"Published\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "Published"), "", " checked "), "") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                //if(AppLogic.AppConfigBool("GoogleCheckout.ShowOnCartPage"))
                //{
                    writer.Write("              <tr valign=\"middle\">\n");
                    writer.Write("                <td align=\"right\" valign=\"middle\">*Google Checkout Allowed:&nbsp;&nbsp;</td>\n");
                    writer.Write("                <td align=\"left\">\n");
                    writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"GoogleCheckoutAllowed\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "GoogleCheckoutAllowed"), " checked ", ""), " checked ") + ">\n");
                    writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"GoogleCheckoutAllowed\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "GoogleCheckoutAllowed"), "", " checked "), "") + ">\n");
                    writer.Write("                </td>\n");
                    writer.Write("              </tr>\n");
                //}

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Available Start Date:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"100\" size=\"15\" name=\"AvailableStartDate\" value=\"" + CommonLogic.IIF(Editing, Localization.ToNativeShortDateString(DB.RSFieldDateTime(rs, "AvailableStartDate")), Localization.ToNativeShortDateString(System.DateTime.Now)) + "\">&nbsp;<img src=\"" + AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/calendar.gif") + "\" style=\"cursor:hand;cursor:pointer;\" align=\"absmiddle\" id=\"f_trigger_s\">&nbsp;<small>(" + Localization.ShortDateFormat() + ", leave blank to start showing product right now)</small>\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Available Stop Date:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"100\" size=\"15\" name=\"AvailableStopDate\" value=\"" + CommonLogic.IIF(Editing, Localization.ToNativeShortDateString(DB.RSFieldDateTime(rs, "AvailableStopDate")), "") + "\">&nbsp;<img src=\"" + AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/calendar.gif") + "\" style=\"cursor:hand;cursor:pointer;\" align=\"absmiddle\" id=\"f_trigger_e\">&nbsp;<small>(" + Localization.ShortDateFormat() + ", leave blank to never expire the product)</small>\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("<tr valign=\"top\">\n");
                writer.Write("<td align=\"right\" valign=\"top\">*Display Format Xml Package:&nbsp;&nbsp;</td>\n");
                writer.Write("<td align=\"left\" valign=\"top\">\n");
                writer.Write("<select size=\"1\" name=\"XmlPackage\">\n");
                ArrayList xmlPackages = AppLogic.ReadXmlPackages("product", 1);
                foreach (String s in xmlPackages)
                {
                    writer.Write("<option value=\"" + s + "\"");
                    if (Editing)
                    {
                        if (DB.RSField(rs, "XmlPackage").ToLowerInvariant() == s)
                        {
                            writer.Write(" selected");
                        }
                    }
                    writer.Write(">" + s + "</option>");
                }
                writer.Write("</select>\n");
                writer.Write("</td>\n");
                writer.Write("</tr>\n");

                writer.Write("<tr valign=\"middle\">\n");
                writer.Write("<td align=\"right\" valign=\"middle\">Quantity Discount Table:&nbsp;&nbsp;</td>\n");
                writer.Write("<td align=\"left\" valign=\"top\">\n");
                writer.Write("<select size=\"1\" name=\"QuantityDiscountID\">\n");
                writer.Write("<option value=\"0\">None</option>");
                rsst = DB.GetRS("select * from QuantityDiscount  " + DB.GetNoLock() + " order by DisplayOrder,Name");
                while (rsst.Read())
                {
                    writer.Write("<option value=\"" + DB.RSFieldInt(rsst, "QuantityDiscountID").ToString() + "\"");
                    if (Editing)
                    {
                        if (DB.RSFieldInt(rs, "QuantityDiscountID") == DB.RSFieldInt(rsst, "QuantityDiscountID"))
                        {
                            writer.Write(" selected");
                        }
                    }
                    writer.Write(">" + DB.RSFieldByLocale(rsst, "Name", ThisCustomer.LocaleSetting) + "</option>");
                }
                rsst.Close();
                writer.Write("</select>\n");
                writer.Write("</td>\n");
                writer.Write("</tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Show Buy Button:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" >\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"ShowBuyButton\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "ShowBuyButton"), " checked ", ""), " checked ") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"ShowBuyButton\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "ShowBuyButton"), "", " checked "), "") + ">\n");
                writer.Write(" <small>if no, the add to cart button will not be shown for this product</small>");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Is Call To Order:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" >\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"IsCallToOrder\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "IsCallToOrder"), " checked ", ""), "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"IsCallToOrder\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "IsCallToOrder"), "", " checked "), " checked ") + ">\n");
                writer.Write(" <small>if yes, CALL TO ORDER will be shown for this product, instead of the add to cart form/button</small>");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Hide Price Until Cart:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" >\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"HidePriceUntilCart\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "HidePriceUntilCart"), " checked ", ""), "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"HidePriceUntilCart\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "HidePriceUntilCart"), "", " checked "), " checked ") + ">\n");
                writer.Write(" <small>if yes, customer must add product to cart in order to see the price.</small>");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Allow This To Be Added To Packs:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" >\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"ShowInProductBrowser\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "ShowInProductBrowser"), " checked ", ""), " checked ") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"ShowInProductBrowser\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "ShowInProductBrowser"), "", " checked "), "") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Exclude From Froogle/PriceGrabber Feeds:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" >\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"ExcludeFromPriceFeeds\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "ExcludeFromPriceFeeds"), " checked ", ""), " checked ") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"ExcludeFromPriceFeeds\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "ExcludeFromPriceFeeds"), "", " checked "), "") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Is A Kit:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"IsAKit\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "IsAKit"), " checked ", ""), "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"IsAKit\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "IsAKit"), "", " checked "), " checked ") + ">\n");
                writer.Write(CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "IsAKit"), "<a href=\"editkit.aspx?productid=" + DB.RSFieldInt(rs, "ProductID").ToString() + "\">Edit Kit</a>", ""), ""));
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Is A Pack:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"IsAPack\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "IsAPack"), " checked ", ""), "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"IsAPack\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "IsAPack"), "", " checked "), " checked ") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Pack Size:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"10\" size=\"10\" name=\"PackSize\" value=\"" + CommonLogic.IIF(Editing, DB.RSFieldInt(rs, "PackSize").ToString(), "") + "\">\n");
                writer.Write("                	<input type=\"hidden\" name=\"PackSize_vldt\" value=\"[number][invalidalert=Please enter the pack size, e.g. 12]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                if (IsAKit || IsAPack)
                {
                    writer.Write("<input type=\"hidden\" name=\"TrackInventoryBySizeAndColor\" value=\"0\">");
                    writer.Write("<input type=\"hidden\" name=\"TrackInventoryBySize\" value=\"0\">");
                    writer.Write("<input type=\"hidden\" name=\"TrackInventoryByColor\" value=\"0\">");
                }
                else
                {
                    writer.Write("              <tr valign=\"middle\">\n");
                    writer.Write("                <td align=\"right\" valign=\"middle\">*Track Inventory By Size And Color:&nbsp;&nbsp;</td>\n");
                    writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"TrackInventoryBySizeAndColor\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "TrackInventoryBySizeAndColor"), " checked ", ""), "") + ">\n");
                    writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"TrackInventoryBySizeAndColor\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "TrackInventoryBySizeAndColor"), "", " checked "), " checked ") + ">\n");
                    writer.Write("                </td>\n");
                    writer.Write("              </tr>\n");

                    // obsolete fields in v6.1.1.1+
                    //writer.Write("              <tr valign=\"middle\">\n");
                    //writer.Write("                <td align=\"right\" valign=\"middle\">*Track Inventory By Size:&nbsp;&nbsp;</td>\n");
                    //writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    //writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"TrackInventoryBySize\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "TrackInventoryBySize"), " checked ", ""), "") + ">\n");
                    //writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"TrackInventoryBySize\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "TrackInventoryBySize"), "", " checked "), " checked ") + ">\n");
                    //writer.Write("                </td>\n");
                    //writer.Write("              </tr>\n");

                    //writer.Write("              <tr valign=\"middle\">\n");
                    //writer.Write("                <td align=\"right\" valign=\"middle\">*Track Inventory By Color:&nbsp;&nbsp;</td>\n");
                    //writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    //writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"TrackInventoryByColor\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "TrackInventoryByColor"), " checked ", ""), "") + ">\n");
                    //writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"TrackInventoryByColor\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "TrackInventoryByColor"), "", " checked "), " checked ") + ">\n");
                    //writer.Write("                </td>\n");
                    //writer.Write("              </tr>\n");

                    writer.Write("              <tr valign=\"middle\">\n");
                    writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Color Option Prompt:&nbsp;&nbsp;</td>\n");
                    writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "ColorOptionPrompt"), "ColorOptionPrompt", false, false, false, String.Empty, 100, 30, 0, 0, false));
                    //writer.Write("                	<input maxLength=\"50\" size=\"20\" name=\"ColorOptionPrompt\" value=\"" + CommonLogic.IIF(Editing , DB.RSField(rs,"ColorOptionPrompt") , "") + "\">\n");
                    writer.Write("                	</td>\n");
                    writer.Write("              </tr>\n");

                    writer.Write("              <tr valign=\"middle\">\n");
                    writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Size Option Prompt:&nbsp;&nbsp;</td>\n");
                    writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "SizeOptionPrompt"), "SizeOptionPrompt", false, false, false, String.Empty, 100, 30, 0, 0, false));
                    //writer.Write("                	<input maxLength=\"50\" size=\"20\" name=\"SizeOptionPrompt\" value=\"" + CommonLogic.IIF(Editing , DB.RSField(rs,"SizeOptionPrompt") , "") + "\">\n");
                    writer.Write("                	</td>\n");
                    writer.Write("              </tr>\n");
                }

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Requires Text Field:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"RequiresTextOption\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "RequiresTextOption"), " checked ", ""), "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"RequiresTextOption\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "RequiresTextOption"), "", " checked "), " checked ") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Text Field Prompt:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "TextOptionPrompt"), "TextOptionPrompt", false, false, false, String.Empty, 100, 30, 0, 0, false));
                //writer.Write("                	<input maxLength=\"50\" size=\"20\" name=\"TextOptionPrompt\" value=\"" + CommonLogic.IIF(Editing , DB.RSField(rs,"TextOptionPrompt") , "") + "\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Text Option Max Length:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"5\" size=\"5\" name=\"TextOptionMaxLength\" value=\"" + CommonLogic.IIF(Editing, DB.RSFieldInt(rs, "TextOptionMaxLength").ToString(), "") + "\"> (# of characters allowed for this text option)\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Requires Registration To View:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"RequiresRegistration\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "RequiresRegistration"), " checked ", ""), "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"RequiresRegistration\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "RequiresRegistration"), "", " checked "), " checked ") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"top\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Page Size:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"2\" size=\"2\" name=\"PageSize\" value=\"" + CommonLogic.IIF(Editing, DB.RSFieldInt(rs, "PageSize").ToString(), AppLogic.AppConfig("Default_ProductPageSize")) + "\"> (may be used by the XmlPackage displaying this page)\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"top\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Column Width:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"2\" size=\"2\" name=\"ColWidth\" value=\"" + CommonLogic.IIF(Editing, DB.RSFieldInt(rs, "ColWidth").ToString(), AppLogic.AppConfig("Default_ProductColWidth")) + "\"> (may be used by the XmlPackage displaying this page)\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Summary:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Summary"), "Summary", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true));
                //        writer.Write("                	<textarea style=\"height: 30em; width: 100%;\" cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeightSmall") + "\" id=\"Summary\" name=\"Summary\">" + CommonLogic.IIF(Editing ,Server.HtmlEncode(DB.RSField(rs,"Summary")) , "") + "</textarea>");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Description:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                if (Editing && pdesc.Contents.Length != 0)
                {
                    writer.Write("<b>From File: " + pdesc.URL + "</b> &nbsp;&nbsp;\n");
                    writer.Write("<input type=\"button\" value=\"Delete\" name=\"DeleteDescriptionFile_" + ProductID.ToString() + "\" onClick=\"DeleteDescriptionFile()\">");
                    writer.Write("<div style=\"border-style: dashed; border-width: 1px;\">\n");
                    writer.Write(pdesc.Contents);
                    writer.Write("</div>\n");
                }
                //else
                //{
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Description"), "Description", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true));
                //        writer.Write("                	<textarea style=\"height: 60em; width: 100%;\" cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeightl") + "\" id=\"Description\" name=\"Description\">" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Description")) , "") + "</textarea>\n");
                //}
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Froogle Description (NO HTML):&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                //writer.Write("                	<textarea style=\"height: 30em; width: 100%;\" cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeightSmall") + "\" id=\"FroogleDescription\" name=\"FroogleDescription\">" + CommonLogic.IIF(Editing ,Server.HtmlEncode(DB.RSField(rs,"FroogleDescription")) , "") + "</textarea>\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "FroogleDescription"), "FroogleDescription", true, false, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), false));
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"top\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Extension Data (User Defined Data):&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<textarea style=\"width: 100%;\" cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"10\" id=\"ExtensionData\" name=\"ExtensionData\">" + CommonLogic.IIF(Editing, DB.RSField(rs, "ExtensionData"), "") + "</textarea>\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Misc Text:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<textarea style=\"width: 100%;\" cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"10\" id=\"MiscText\" name=\"MiscText\">" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "MiscText")), "") + "</textarea>\n");
                if (AppLogic.AppConfigBool("ShowAutoFill"))
                {
                    writer.Write("<a href=\"autofill.aspx?productid=" + ProductID.ToString() + "\" target=\"_blank\">AutoFill Variants</a>\n");
                }
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Product Mappings(s):&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");

                String[] names = { AppLogic.GetString("AppConfig.CategoryPromptPlural", SkinID, ThisCustomer.LocaleSetting), AppLogic.GetString("AppConfig.SectionPromptPlural", SkinID, ThisCustomer.LocaleSetting), "Affiliates", "CustomerLevels", "Genres", "Vectors" };
                String[] values = {	GetEntityListNew(ProductID, ProductCategories,		0,1,ThisCustomer.LocaleSetting,base.EntityHelpers,"Category"),
									  GetEntityListNew(ProductID, ProductSections,		0,1,ThisCustomer.LocaleSetting,base.EntityHelpers,"Section"),
									  GetEntityListNew(ProductID, ProductAffiliates,		0,1,ThisCustomer.LocaleSetting,base.EntityHelpers,"Affiliate"),
									  GetEntityListNew(ProductID, ProductCustomerLevels, 0,1,ThisCustomer.LocaleSetting,base.EntityHelpers,"CustomerLevel"),
									  GetEntityListNew(ProductID, ProductGenres, 0,1,ThisCustomer.LocaleSetting,base.EntityHelpers,"Genre"),
									  GetEntityListNew(ProductID, ProductVectors, 0,1,ThisCustomer.LocaleSetting,base.EntityHelpers,"Vector")
                };
                writer.Write("<table width=\"100%\"><tr><td width=\"100%\">");
                writer.Write(AppLogic.WriteTabbedContents("Mappings", 1, false, names, values));
                writer.Write("</td></tr></table>");

                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Related Products:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"255\" size=\"100\" name=\"RelatedProducts\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "RelatedProducts"), "") + "\"> (enter related PRODUCT IDs, NOT names, e.g. 42,13,150)\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Upsell Products:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"255\" size=\"100\" name=\"UpsellProducts\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "UpsellProducts"), "") + "\"> (enter upsell PRODUCT IDs, NOT names, e.g. 42,13,150)\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Upsell Product Discount Percent:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"UpsellProductDiscountPercentage\" value=\"" + CommonLogic.IIF(Editing, Localization.DecimalStringForDB(DB.RSFieldDecimal(rs, "UpsellProductDiscountPercentage")), "") + "\"><small>(Enter 0, or a percentage like 5 or 7.5)</small>\n");
                writer.Write("                	<input type=\"hidden\" name=\"UpsellProductDiscountPercentage_vldt\" value=\"[number][invalidalert=Please enter a valid percentage amount, e.g. 10.0]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Requires Products:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"255\" size=\"100\" name=\"RequiresProducts\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "RequiresProducts"), "") + "\"> (enter PRODUCT IDs, NOT names, that MUST be in the cart if this product is also in the cart. The store will add these to the customer cart automatically if they are not present when this product is added. e.g. 42,13,150)\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Search Engine Page Title:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "SETitle"), "SETitle", false, true, false, "", 100, 100, 0, 0, false));
                //				writer.Write("                	<input maxLength=\"100\" size=\"100\" name=\"SETitle\" value=\"" + CommonLogic.IIF(Editing ,DB.RSField(rs,"SETitle") , "") + "\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Search Engine Keywords:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "SEKeywords"), "SEKeywords", false, true, false, "", 255, 100, 0, 0, false));
                //				writer.Write("                	<input maxLength=\"255\" size=\"100\" name=\"SEKeywords\" value=\"" + CommonLogic.IIF(Editing ,DB.RSField(rs,"SEKeywords") , "") + "\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Search Engine Description:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "SEDescription"), "SEDescription", false, true, false, "", 255, 100, 0, 0, false));
                //				writer.Write("                	<input maxLength=\"255\" size=\"100\" name=\"SEDescription\" value=\"" + CommonLogic.IIF(Editing ,DB.RSField(rs,"SEDescription") , "") + "\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");


                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Search Engine NoScript:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "SENoScript"), "SENoScript", true, true, false, "", 50, 50, 0, 0, false));
                //				writer.Write("                	<textarea name=\"SENoScript\" rows=\"10\" cols=\"50\">" + CommonLogic.IIF(Editing , DB.RSField(rs,"SENoScript") , "") + "</textarea>\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                //writer.Write("              <tr valign=\"middle\">\n");
                //writer.Write("                <td align=\"right\" valign=\"top\">Search Engine AltText:&nbsp;&nbsp;</td>\n");
                //writer.Write("                <td align=\"left\" valign=\"top\">\n");
                //writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "SEAltText"), "SEAltText", false, true, false, "", 50, 50, 0, 0, false));
                ////        writer.Write("                	<input maxLength=\"50\" size=\"50\" name=\"SEAltText\" value=\"" + CommonLogic.IIF(Editing , DB.RSField(rs,"SEAltText") , "") + "\">\n");
                //writer.Write("                </td>\n");
                //writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*'On Sale' Prompt:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("<select size=\"1\" name=\"SalesPromptID\">\n");
                writer.Write(" <OPTION VALUE=\"0\">SELECT ONE</option>\n");
                rsst = DB.GetRS("select * from salesprompt  " + DB.GetNoLock() + " where deleted=0");
                while (rsst.Read())
                {
                    writer.Write("<option value=\"" + DB.RSFieldInt(rsst, "SalesPromptID").ToString() + "\"");
                    if (Editing)
                    {
                        if (DB.RSFieldInt(rs, "SalesPromptID") == DB.RSFieldInt(rsst, "SalesPromptID"))
                        {
                            writer.Write(" selected");
                        }
                    }
                    else
                    {
                        if (DB.RSFieldInt(rsst, "SalesPromptID") == AppLogic.AppConfigUSInt("Admin_DefaultSalesPromptID"))
                        {
                            writer.Write(" selected");
                        }
                    }
                    writer.Write(">" + DB.RSFieldByLocale(rsst, "Name", ThisCustomer.LocaleSetting) + "</option>");
                }
                rsst.Close();
                writer.Write("</select>\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Spec Title:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                //writer.Write("                	<input maxLength=\"100\" size=\"50\" name=\"SpecTitle\" value=\"" + CommonLogic.IIF(Editing ,Server.HtmlEncode(DB.RSField(rs,"SpecTitle")) , "") + "\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "SpecTitle"), "SpecTitle", false, true, false, "", 255, 100, 0, 0, false));
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Spec Call:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"1000\" size=\"50\" name=\"SpecCall\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "SpecCall"), "") + "\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Specs Inline:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"SpecsInline\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "SpecsInline"), " checked ", ""), CommonLogic.IIF(AppLogic.AppConfigBool("Admin_SpecsInlineByDefault"), " checked ", "")) + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"SpecsInline\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "SpecsInline"), "", " checked "), CommonLogic.IIF(AppLogic.AppConfigBool("Admin_SpecsInlineByDefault"), "", " checked ")) + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Page BG Color:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"20\" size=\"10\" name=\"PageBGColor\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "PageBGColor"), "") + "\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Contents BG Color:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"20\" size=\"10\" name=\"ContentsBGColor\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "ContentsBGColor"), "") + "\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Skin Graphics Color:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"20\" size=\"10\" name=\"GraphicsColor\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "GraphicsColor"), "") + "\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Image Filename Override:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"100\" size=\"40\" name=\"ImageFilenameOverride\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "ImageFilenameOverride"), "") + "\"> (filename, with extension, e.g. myproductpic14.jpg, still assumed to be in images/product/icon, images/product/medium, and images/product/large directories!)\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");



                bool disableupload = (Editing && DB.RSField(rs, "ImageFilenameOverride") != "");

                writer.Write("  <tr>\n");
                writer.Write("    <td valign=\"top\" align=\"right\">Icon (ProductID=" + ProductID.ToString() + "):\n");
                writer.Write("</td>\n");
                writer.Write("    <td valign=\"top\" align=\"left\">");
                writer.Write("    <input type=\"file\" name=\"Image1\" size=\"50\" value=\"" + CommonLogic.IIF(Editing, "", "") + "\" " + CommonLogic.IIF(disableupload, " disabled ", "") + ">\n");
                String Image1URL = AppLogic.LookupImage("Product", ProductID, "icon", SkinID, ThisCustomer.LocaleSetting);
                if (Image1URL.Length != 0)
                {
                    if (Image1URL.IndexOf("nopicture") == -1)
                    {
                        writer.Write("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image1URL + "','Pic1');\">Click here</a> to delete the current image<br/>\n");
                    }
                    if (AppLogic.GetProductsFirstVariantID(ProductID) != 0)
                    {
                        writer.Write("&nbsp;&nbsp;<a href=\"productimagemgr.aspx?size=icon&productid=" + ProductID.ToString() + "\">Icon Multi-Image Manager</a>");
                    }
                    writer.Write("<br/><img id=\"Pic1\" name=\"Pic1\" border=\"0\" src=\"" + Image1URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\">\n");
                }
                writer.Write("</td>\n");
                writer.Write(" </tr>\n");

                writer.Write("  <tr>\n");
                writer.Write("    <td valign=\"top\" align=\"right\">Medium Pic (ProductID=" + ProductID.ToString() + "):\n");
                writer.Write("</td>\n");
                writer.Write("    <td valign=\"top\" align=\"left\">");
                writer.Write("    <input type=\"file\" name=\"Image2\" size=\"50\" value=\"" + CommonLogic.IIF(Editing, "", "") + "\" " + CommonLogic.IIF(disableupload, " disabled ", "") + ">\n");
                String Image2URL = AppLogic.LookupImage("Product", ProductID, "medium", SkinID, ThisCustomer.LocaleSetting);
                if (Image2URL.Length != 0)
                {
                    if (Image2URL.IndexOf("nopicture") == -1)
                    {
                        writer.Write("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image2URL + "','Pic2');\">Click here</a> to delete the current image<br/>\n");
                    }
                    if (AppLogic.GetProductsFirstVariantID(ProductID) != 0)
                    {
                        writer.Write("&nbsp;&nbsp;<a href=\"productimagemgr.aspx?size=medium&productid=" + ProductID.ToString() + "\">Medium Multi-Image Manager</a>");
                    }
                    writer.Write("<br/><img id=\"Pic2\" name=\"Pic2\" border=\"0\" src=\"" + Image2URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\">\n");
                }
                writer.Write("</td>\n");
                writer.Write(" </tr>\n");

                writer.Write("  <tr>\n");
                writer.Write("    <td valign=\"top\" align=\"right\">Large Pic (ProductID=" + ProductID.ToString() + "):\n");
                writer.Write("</td>\n");
                writer.Write("    <td valign=\"top\" align=\"left\">");
                writer.Write("    <input type=\"file\" name=\"Image3\" size=\"50\" value=\"" + CommonLogic.IIF(Editing, "", "") + "\" " + CommonLogic.IIF(disableupload, " disabled ", "") + ">\n");
                String Image3URL = AppLogic.LookupImage("Product", ProductID, "large", SkinID, ThisCustomer.LocaleSetting);
                if (Image3URL.Length == 0)
                {
                    Image3URL = AppLogic.NoPictureImageURL(false, SkinID, ThisCustomer.LocaleSetting);
                }
                if (Image3URL.Length != 0)
                {
                    if (Image3URL.IndexOf("nopicture") == -1)
                    {
                        writer.Write("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image3URL + "','Pic3');\">Click here</a> to delete the current image<br/>\n");
                    }
                    if (AppLogic.GetProductsFirstVariantID(ProductID) != 0)
                    {
                        writer.Write("&nbsp;&nbsp;<a href=\"productimagemgr.aspx?size=large&productid=" + ProductID.ToString() + "\">Large Multi-Image Manager</a>");
                    }
                    writer.Write("<br/><img id=\"Pic3\" name=\"Pic3\" border=\"0\" src=\"" + Image3URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\">\n");
                }
                writer.Write("</td>\n");
                writer.Write(" </tr>\n");

                writer.Write("  <tr>\n");
                writer.Write("    <td valign=\"top\" align=\"right\">Color Swatch Pic:\n");
                writer.Write("</td>\n");
                writer.Write("    <td valign=\"top\" align=\"left\">");
                writer.Write("    <input type=\"file\" name=\"Image4\" size=\"50\" value=\"" + CommonLogic.IIF(Editing, "", "") + "\">\n");
                String Image4URL = AppLogic.LookupImage("Product", ProductID, "swatch", SkinID, ThisCustomer.LocaleSetting);
                if (Image4URL.Length != 0)
                {
                    if (Image4URL.IndexOf("nopicture") == -1)
                    {
                        writer.Write("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image4URL + "','SwatchPic');\">Click here</a> to delete the current image<br/>\n");
                    }
                    writer.Write("<br/><img id=\"SwatchPic\" name=\"SwatchPic\" border=\"0\" src=\"" + Image4URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\">\n");
                }
                writer.Write("</td>\n");
                writer.Write(" </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Swatch Image Map:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<textarea cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeightSmall") + "\" name=\"SwatchImageMap\">" + CommonLogic.IIF(Editing, DB.RSField(rs, "SwatchImageMap"), "") + "</textarea>\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");



                writer.Write("<tr>\n");
                writer.Write("<td></td><td align=\"left\" valign=\"top\"><br/>\n");
                if (Editing)
                {
                    writer.Write("<input type=\"submit\" value=\"Update\" name=\"submit\">\n");
                    writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"reset\" class=\"CPButton\" value=\"Reset\" name=\"reset\">\n");
                }
                else
                {
                    writer.Write("<input type=\"submit\" value=\"Add New\" name=\"submit\">\n");
                }
                writer.Write("        </td>\n");
                writer.Write("      </tr>\n");

                if (Editing && pspec.Contents.Length != 0)
                {
                    writer.Write("<tr valign=\"middle\">\n");
                    writer.Write("<td align=\"right\" valign=\"top\">Specifications:&nbsp;&nbsp;</td>\n");
                    writer.Write("<td align=\"left\" valign=\"top\">\n");
                    writer.Write("<b>From File: " + pspec.FN + "</b> &nbsp;&nbsp;\n");
                    writer.Write("<input type=\"button\" value=\"Delete\" name=\"DeleteSpecFile_" + ProductID.ToString() + "\" onClick=\"DeleteSpecFile()\">");
                    writer.Write("<div style=\"border-style: dashed; border-width: 1px;\">\n");
                    writer.Write(pspec.Contents);
                    writer.Write("</div>\n");
                    writer.Write("</td>\n");
                    writer.Write("</tr>\n");
                }



                if (!Editing)
                {
                    writer.Write("<tr><td colspan=\"2\"><br/><hr size=\"1\"/><b>IF YOU ARE ADDING A SIMPLE PRODUCT, OR PACK, OR KIT YOU MUST ENTER AT LEAST THE PRICE BELOW AND USE THE BOTTOM SUBMIT BUTTON ON THIS PAGE!! IF YOU NEED A COMPLEX PRODUCT (ONE THAT HAS MULTIPLE VARIANTS), LEAVE ALL FIELDS BELOW BLANK AND USE THE SUBMIT BUTTON ABOVE, AND THEN ENTER PRODUCT VARIANTS <font color=blue>AFTER</font> YOU HAVE ADDED THE MAIN PRODUCT!</b><br/></td></tr>\n");

                    // SIMPLE VARIANT STUFF:
                    writer.Write("              <tr valign=\"middle\">\n");
                    writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Price:&nbsp;&nbsp;</td>\n");
                    writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    writer.Write("                	<input maxLength=\"10\" size=\"10\" name=\"Price\" value=\"" + CommonLogic.IIF(Editing, DB.RSFieldDecimal(rs, "Price").ToString(), "") + "\">\n");
                    writer.Write("                	<input type=\"hidden\" name=\"Price_vldt\" value=\"[number][blankalert=Please enter the variant price][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                    writer.Write("                	</td>\n");
                    writer.Write("              </tr>\n");

                    writer.Write("              <tr valign=\"middle\">\n");
                    writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Sale Price:&nbsp;&nbsp;</td>\n");
                    writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    writer.Write("                	<input maxLength=\"10\" size=\"10\" name=\"SalePrice\" value=\"" + CommonLogic.IIF(Editing, DB.RSFieldDecimal(rs, "SalePrice").ToString(), "") + "\">\n");
                    writer.Write("                	<input type=\"hidden\" name=\"SalePrice_vldt\" value=\"[number][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                    writer.Write("                	</td>\n");
                    writer.Write("              </tr>\n");

                    writer.Write("              <tr valign=\"middle\">\n");
                    writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Weight:&nbsp;&nbsp;</td>\n");
                    writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    writer.Write("                	<input maxLength=\"10\" size=\"10\" name=\"Weight\" value=\"" + CommonLogic.IIF(Editing, DB.RSFieldDecimal(rs, "Weight").ToString(), "") + "\"> <small>(in " + Localization.WeightUnits() + ")</small>\n");
                    writer.Write("                	<input type=\"hidden\" name=\"Weight_vldt\" value=\"[number][invalidalert=Please enter the weight of this item in " + Localization.WeightUnits() + ", e.g. 2.5]\">\n");
                    writer.Write("                	</td>\n");
                    writer.Write("              </tr>\n");

                    writer.Write("              <tr valign=\"middle\">\n");
                    writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Dimensions:&nbsp;&nbsp;</td>\n");
                    writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    writer.Write("                	<input maxLength=\"50\" size=\"50\" name=\"Dimensions\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "Dimensions"), "") + "\">\n");
                    writer.Write("                	</td>\n");
                    writer.Write("              </tr>\n");

                    if (!ProductTracksInventoryBySizeAndColor)
                    {
                        writer.Write("              <tr valign=\"middle\">\n");
                        writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Current Inventory:&nbsp;&nbsp;</td>\n");
                        writer.Write("                <td align=\"left\" valign=\"top\">\n");
                        writer.Write("                	<input maxLength=\"10\" size=\"10\" name=\"Inventory\" value=\"" + CommonLogic.IIF(Editing, DB.RSFieldInt(rs, "Inventory").ToString(), "") + "\">\n");
                        writer.Write("                	<input type=\"hidden\" name=\"Inventory_vldt\" value=\"[number][invalidalert=Please enter the current inventory in stock for this item, e.g. 100]\">\n");
                        writer.Write("                	</td>\n");
                        writer.Write("              </tr>\n");
                    }

                    writer.Write("              <tr valign=\"middle\">\n");
                    writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Colors:&nbsp;&nbsp;</td>\n");
                    writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    writer.Write("                	<input maxLength=\"5000\" size=\"50\" name=\"Colors\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "Colors"), "") + "\">&nbsp;<small>(Separate colors by commas)</small>\n");
                    writer.Write("                	</td>\n");
                    writer.Write("              </tr>\n");

                    writer.Write("              <tr valign=\"middle\">\n");
                    writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Color SKU Modifiers:&nbsp;&nbsp;</td>\n");
                    writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    writer.Write("                	<input maxLength=\"5000\" size=\"50\" name=\"ColorSKUModifiers\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "ColorSKUModifiers"), "") + "\">&nbsp;<small>(Separate skus by commas to match colors)</small>\n");
                    writer.Write("                	</td>\n");
                    writer.Write("              </tr>\n");

                    writer.Write("              <tr valign=\"middle\">\n");
                    writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Sizes:&nbsp;&nbsp;</td>\n");
                    writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    writer.Write("                	<input maxLength=\"5000\" size=\"50\" name=\"Sizes\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "Sizes"), "") + "\">&nbsp;<small>(Separate sizes by commas)</small>\n");
                    writer.Write("                	</td>\n");
                    writer.Write("              </tr>\n");

                    writer.Write("              <tr valign=\"middle\">\n");
                    writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Size SKU Modifiers:&nbsp;&nbsp;</td>\n");
                    writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    writer.Write("                	<input maxLength=\"5000\" size=\"50\" name=\"SizeSKUModifiers\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "SizeSKUModifiers"), "") + "\">&nbsp;<small>(Separate skus by commas to match sizes)</small>\n");
                    writer.Write("                	</td>\n");
                    writer.Write("              </tr>\n");

                    writer.Write("<tr>\n");
                    writer.Write("<td></td><td align=\"left\" valign=\"top\"><br/>\n");
                    writer.Write("<input type=\"submit\" value=\"Add New (Simple Product, or Kit, Or Pack)\" name=\"submit\">\n");
                    writer.Write("</td>\n");
                    writer.Write("</tr>\n");



                }

                writer.Write("  </table>\n");
                writer.Write("</form>\n");

                writer.Write("<script type=\"text/javascript\">\n");

                writer.Write("function DeleteImage(imgurl,name)\n");

                writer.Write("{\n");

                // Delete confirmation
                writer.Write("if(!(confirm('Are you sure you want to delete this image?'))) return false;\n");
                //******************************

                writer.Write("window.open('deleteimage.aspx?imgurl=' + imgurl + '&FormImageName=' + name,\"Admin_ML\",\"height=250,width=440,top=10,left=20,status=no,toolbar=no,menubar=no,scrollbars=yes,location=no\");\n");
                writer.Write("}\n");

                writer.Write("function DeleteDescriptionFile()\n");
                writer.Write("{\n");
                writer.Write("if(confirm('Are you sure you want to delete the description file for this product?'))\n");
                writer.Write("{\n");
                writer.Write("self.location = 'editproduct.aspx?productid=" + ProductID.ToString() + "&deletedescriptionfile=true';\n");
                writer.Write("}\n");
                writer.Write("}\n");

                writer.Write("function DeleteSpecFile()\n");
                writer.Write("{\n");
                writer.Write("if(confirm('Are you sure you want to delete the spec file for this product?'))\n");
                writer.Write("{\n");
                writer.Write("self.location = 'editproduct.aspx?productid=" + ProductID.ToString() + "&deletespecfile=true';\n");
                writer.Write("}\n");
                writer.Write("}\n");

                writer.Write("</SCRIPT>\n");

                writer.Write("\n<script type=\"text/javascript\">\n");
                writer.Write("    Calendar.setup({\n");
                writer.Write("        inputField     :    \"AvailableStartDate\",      // id of the input field\n");
                writer.Write("        ifFormat       :    \"" + Localization.JSCalendarDateFormatSpec() + "\",       // format of the input field\n");
                writer.Write("        showsTime      :    false,            // will display a time selector\n");
                writer.Write("        button         :    \"f_trigger_s\",   // trigger for the calendar (button ID)\n");
                writer.Write("        singleClick    :    true            // double-click mode\n");
                writer.Write("    });\n");
                writer.Write("    Calendar.setup({\n");
                writer.Write("        inputField     :    \"AvailableStopDate\",      // id of the input field\n");
                writer.Write("        ifFormat       :    \"" + Localization.JSCalendarDateFormatSpec() + "\",       // format of the input field\n");
                writer.Write("        showsTime      :    false,            // will display a time selector\n");
                writer.Write("        button         :    \"f_trigger_e\",   // trigger for the calendar (button ID)\n");
                writer.Write("        singleClick    :    true            // double-click mode\n");
                writer.Write("    });\n");
                writer.Write("</script>\n");

            }
            rs.Close();
        }


        private String GetEntityListNew(int ProductID, String ProductEntities, int ForParentEntityID, int level, string LocaleSetting, System.Collections.Generic.Dictionary<string, EntityHelper> EntityHelpers, String EntityName)
        {
            XmlPackage2 p = new XmlPackage2("Product" + EntityName + "Edit.xml.config", base.ThisCustomer, base.SkinID, String.Empty, "ProductID=" + ProductID.ToString(), String.Empty);
            string results = AppLogic.RunXmlPackage(p, base.GetParser, base.ThisCustomer, base.SkinID, false, false);
            return results;
        }


    }
}
