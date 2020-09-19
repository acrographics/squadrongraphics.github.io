// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2006.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/editvariant.aspx.cs 10    10/04/06 6:22a Redwoodtree $
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
    /// Summary description for editvariant
    /// </summary>
    public partial class editvariant : AspDotNetStorefront.SkinBase
    {

        int ProductID;
        int VariantID;
        bool ProductTracksInventoryBySizeAndColor;
        Shipping.ShippingCalculationEnum ShipCalcID = Shipping.ShippingCalculationEnum.Unknown;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            ShipCalcID = Shipping.GetActiveShippingCalculationID();
            ProductID = CommonLogic.QueryStringUSInt("ProductID");
            VariantID = 0;

            if (CommonLogic.QueryStringCanBeDangerousContent("VariantID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("VariantID") != "0")
            {
                Editing = true;
                VariantID = Localization.ParseUSInt(CommonLogic.QueryStringCanBeDangerousContent("VariantID"));
            }
            else
            {
                Editing = false;
            }
            if (ProductID == 0)
            {
                ProductID = AppLogic.GetVariantProductID(VariantID);
            }

            ProductTracksInventoryBySizeAndColor = AppLogic.ProductTracksInventoryBySizeAndColor(ProductID);

            IDataReader rs;

            //int N = 0;
            if (CommonLogic.FormBool("IsSubmit"))
            {
                decimal Price = System.Decimal.Zero;
                decimal SalePrice = System.Decimal.Zero;
                //Decimal KitPrice = System.Decimal.Zero;
                decimal MSRP = System.Decimal.Zero;
                decimal Cost = System.Decimal.Zero;
                int Points = 0;
                int MinimumQuantity = 0;
                if (CommonLogic.FormCanBeDangerousContent("Price").Length != 0)
                {
                    Price = CommonLogic.FormUSDecimal("Price");
                }
                if (CommonLogic.FormCanBeDangerousContent("SalePrice").Length != 0)
                {
                    SalePrice = CommonLogic.FormUSDecimal("SalePrice");
                }

                if (CommonLogic.FormCanBeDangerousContent("MSRP").Length != 0)
                {
                    MSRP = CommonLogic.FormUSDecimal("MSRP");
                }
                if (CommonLogic.FormCanBeDangerousContent("Cost").Length != 0)
                {
                    Cost = CommonLogic.FormUSDecimal("Cost");
                }

                if (CommonLogic.FormCanBeDangerousContent("MinimumQuantity").Length != 0)
                {
                    MinimumQuantity = CommonLogic.FormUSInt("MinimumQuantity");
                }

                bool IsFirstVariantAdded = (DB.GetSqlN("select count(VariantID) as N from ProductVariant " + DB.GetNoLock() + " where ProductID=" + ProductID.ToString() + " and Deleted=0") == 0);
                StringBuilder sql = new StringBuilder(2500);
                if (!Editing)
                {
                    // ok to add:
                    String NewGUID = DB.GetNewGUID();
                    sql.Append("insert into productvariant(VariantGUID,ProductID,Name,ContentsBGColor,PageBGColor,GraphicsColor,ImageFilenameOverride,IsDefault,Description,RestrictedQuantities,FroogleDescription,Price,SalePrice,MSRP,Cost,Points,MinimumQuantity,SKUSuffix,ManufacturerPartNumber,Weight,Dimensions,Inventory,SubscriptionInterval,SubscriptionIntervalType,Published,CustomerEntersPrice,CustomerEntersPricePrompt,IsRecurring,RecurringInterval,RecurringIntervalType,Colors,ColorSKUModifiers,Sizes,SizeSKUModifiers,IsTaxable,IsShipSeparately,IsDownload,FreeShipping,DownloadLocation) values(");
                    sql.Append(DB.SQuote(NewGUID) + ",");
                    sql.Append(ProductID.ToString() + ",");
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("ContentsBGColor")) + ",");
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("PageBGColor")) + ",");
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("GraphicsColor")) + ",");
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("ImageFilenameOverride")) + ",");
                    if (IsFirstVariantAdded)
                    {
                        sql.Append("1,"); // IsDefault=1
                    }
                    else
                    {
                        sql.Append("0,"); // IsDefault=0
                    }
                    if (AppLogic.FormLocaleXml("Description").Length != 0)
                    {
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Description")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("RestrictedQuantities").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("RestrictedQuantities")) + ",");
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
                    sql.Append(Localization.DecimalStringForDB(Price) + ",");
                    sql.Append(CommonLogic.IIF(SalePrice != System.Decimal.Zero, Localization.DecimalStringForDB(SalePrice), "NULL") + ",");
                    sql.Append(CommonLogic.IIF(MSRP != System.Decimal.Zero, Localization.DecimalStringForDB(MSRP), "NULL") + ",");
                    sql.Append(CommonLogic.IIF(Cost != System.Decimal.Zero, Localization.DecimalStringForDB(Cost), "NULL") + ",");
                    sql.Append(Localization.IntStringForDB(Points) + ",");
                    sql.Append(CommonLogic.IIF(MinimumQuantity != 0, Localization.IntStringForDB(MinimumQuantity), "NULL") + ",");
                    if (CommonLogic.FormCanBeDangerousContent("SKUSuffix").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("SKUSuffix")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("ManufacturerPartNumber").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("ManufacturerPartNumber")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    sql.Append(CommonLogic.IIF(CommonLogic.FormUSDecimal("Weight") != 0.0M, Localization.DecimalStringForDB(CommonLogic.FormUSDecimal("Weight")), "NULL") + ",");
                    if (CommonLogic.FormCanBeDangerousContent("Dimensions").Length != 0)
                    {
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("Dimensions")) + ",");
                    }
                    else
                    {
                        sql.Append("NULL,");
                    }
                    sql.Append(CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("Inventory").Length != 0, CommonLogic.FormCanBeDangerousContent("Inventory"), "1000000") + ",");
                    sql.Append(CommonLogic.FormUSInt("SubscriptionInterval").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("SubscriptionIntervalType").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("Published").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("CustomerEntersPrice").ToString() + ",");
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("CustomerEntersPricePrompt")) + ",");
                    sql.Append(CommonLogic.FormUSInt("IsRecurring").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("RecurringInterval").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("RecurringIntervalType").ToString() + ",");
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Colors").Replace(", ", ",").Replace(" ,", ",").Replace("'", "").Trim()) + ",");
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("ColorSKUModifiers").Replace(", ", ",").Replace(" ,", ",").Trim()) + ",");
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Sizes").Replace(", ", ",").Replace(" ,", ",").Replace("'", "").Trim()) + ",");
                    sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("SizeSKUModifiers").Replace(", ", ",").Replace(" ,", ",").Trim()) + ",");
                    //sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Sizes2").Replace(", ", ",").Replace(" ,", ",").Replace("'", "").Trim()) + ",");
                    //sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("SizeSKUModifiers2").Replace(", ", ",").Replace(" ,", ",").Trim()) + ",");
                    sql.Append(CommonLogic.FormUSInt("IsTaxable").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("IsShipSeparately").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("IsDownload").ToString() + ",");
                    sql.Append(CommonLogic.FormUSInt("FreeShipping").ToString() + ",");
                    String DLoc = CommonLogic.FormCanBeDangerousContent("DownloadLocation");
                    if (DLoc.StartsWith("/"))
                    {
                        DLoc = DLoc.Substring(1, DLoc.Length - 1); // remove leading / char!
                    }
                    sql.Append(DB.SQuote(DLoc));
                    sql.Append(")");
                    try
                    {
                        DB.ExecuteSQL(sql.ToString());
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException("Error in EditVariant(.RunSql), Msg=[" + CommonLogic.GetExceptionDetail(ex, String.Empty) + "], Sql=[" + sql.ToString() + "]");
                    }

                    rs = DB.GetRS("select VariantID from productvariant  " + DB.GetNoLock() + " where deleted=0 and VariantGUID=" + DB.SQuote(NewGUID));
                    rs.Read();
                    VariantID = DB.RSFieldInt(rs, "VariantID");
                    Editing = true;
                    rs.Close();
                    DataUpdated = true;
                }
                else
                {
                    // ok to update:
                    sql.Append("update productvariant set ");
                    sql.Append("ProductID=" + ProductID.ToString() + ",");
                    sql.Append("Name=" + DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                    sql.Append("ContentsBGColor=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ContentsBGColor")) + ",");
                    sql.Append("PageBGColor=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("PageBGColor")) + ",");
                    sql.Append("GraphicsColor=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("GraphicsColor")) + ",");
                    sql.Append("ImageFilenameOverride=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ImageFilenameOverride")) + ",");
                    if (AppLogic.FormLocaleXml("Description").Length != 0)
                    {
                        sql.Append("Description=" + DB.SQuote(AppLogic.FormLocaleXml("Description")) + ",");
                    }
                    else
                    {
                        sql.Append("Description=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("RestrictedQuantities").Length != 0)
                    {
                        sql.Append("RestrictedQuantities=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("RestrictedQuantities")) + ",");
                    }
                    else
                    {
                        sql.Append("RestrictedQuantities=NULL,");
                    }
                    if (AppLogic.FormLocaleXml("FroogleDescription").Length != 0)
                    {
                        sql.Append("FroogleDescription=" + DB.SQuote(AppLogic.FormLocaleXml("FroogleDescription")) + ",");
                    }
                    else
                    {
                        sql.Append("FroogleDescription=NULL,");
                    }
                    sql.Append("Price=" + Localization.DecimalStringForDB(Price) + ",");
                    sql.Append("SalePrice=" + CommonLogic.IIF(SalePrice != System.Decimal.Zero, Localization.DecimalStringForDB(SalePrice), "NULL") + ",");
                    sql.Append("MSRP=" + CommonLogic.IIF(MSRP != System.Decimal.Zero, Localization.DecimalStringForDB(MSRP), "NULL") + ",");
                    sql.Append("Cost=" + CommonLogic.IIF(Cost != System.Decimal.Zero, Localization.DecimalStringForDB(Cost), "NULL") + ",");
                    sql.Append("Points=" + Localization.IntStringForDB(Points) + ",");
                    sql.Append("MinimumQuantity=" + CommonLogic.IIF(MinimumQuantity != 0, Localization.IntStringForDB(MinimumQuantity), "NULL") + ",");
                    if (CommonLogic.FormCanBeDangerousContent("SKUSuffix").Length != 0)
                    {
                        sql.Append("SKUSuffix=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("SKUSuffix")) + ",");
                    }
                    else
                    {
                        sql.Append("SKUSuffix=NULL,");
                    }
                    if (CommonLogic.FormCanBeDangerousContent("ManufacturerPartNumber").Length != 0)
                    {
                        sql.Append("ManufacturerPartNumber=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ManufacturerPartNumber")) + ",");
                    }
                    else
                    {
                        sql.Append("ManufacturerPartNumber=NULL,");
                    }
                    sql.Append("Weight=" + CommonLogic.IIF(CommonLogic.FormUSDecimal("Weight") != 0.0M, Localization.DecimalStringForDB(CommonLogic.FormUSDecimal("Weight")), "NULL") + ",");
                    if (CommonLogic.FormCanBeDangerousContent("Dimensions").Length != 0)
                    {
                        sql.Append("Dimensions=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Dimensions")) + ",");
                    }
                    else
                    {
                        sql.Append("Dimensions=NULL,");
                    }
                    sql.Append("Inventory=" + CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("Inventory").Length != 0, CommonLogic.FormCanBeDangerousContent("Inventory"), "1000000") + ",");
                    sql.Append("SubscriptionInterval=" + CommonLogic.FormUSInt("SubscriptionInterval").ToString() + ",");
                    sql.Append("SubscriptionIntervalType=" + CommonLogic.FormUSInt("SubscriptionIntervalType").ToString() + ",");
                    sql.Append("Published=" + CommonLogic.FormUSInt("Published").ToString() + ",");
                    sql.Append("CustomerEntersPrice=" + CommonLogic.FormUSInt("CustomerEntersPrice").ToString() + ",");
                    sql.Append("CustomerEntersPricePrompt=" + DB.SQuote(AppLogic.FormLocaleXml("CustomerEntersPricePrompt")) + ",");
                    sql.Append("IsRecurring=" + CommonLogic.FormUSInt("IsRecurring").ToString() + ",");
                    sql.Append("RecurringInterval=" + CommonLogic.FormUSInt("RecurringInterval").ToString() + ",");
                    sql.Append("RecurringIntervalType=" + CommonLogic.FormUSInt("RecurringIntervalType").ToString() + ",");
                    sql.Append("Colors=" + DB.SQuote(AppLogic.FormLocaleXml("Colors").Replace(", ", ",").Replace(" ,", ",").Replace("'", "").Trim()) + ",");
                    sql.Append("ColorSKUModifiers=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ColorSKUModifiers").Replace(", ", ",").Replace(" ,", ",").Trim()) + ",");
                    sql.Append("Sizes=" + DB.SQuote(AppLogic.FormLocaleXml("Sizes").Replace(", ", ",").Replace(" ,", ",").Replace("'", "").Trim()) + ",");
                    sql.Append("SizeSKUModifiers=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("SizeSKUModifiers").Replace(", ", ",").Replace(" ,", ",").Trim()) + ",");
                    //sql.Append("Sizes2=" + DB.SQuote(AppLogic.FormLocaleXml("Sizes2").Replace(", ", ",").Replace(" ,", ",").Replace("'","").Trim()) + ",");
                    //sql.Append("SizeSKUModifiers2=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("SizeSKUModifiers2").Replace(", ", ",").Replace(" ,", ",").Trim()) + ",");
                    sql.Append("IsTaxable=" + CommonLogic.FormUSInt("IsTaxable").ToString() + ",");
                    sql.Append("IsShipSeparately=" + CommonLogic.FormUSInt("IsShipSeparately").ToString() + ",");
                    sql.Append("IsDownload=" + CommonLogic.FormUSInt("IsDownload").ToString() + ",");
                    sql.Append("FreeShipping=" + CommonLogic.FormUSInt("FreeShipping").ToString() + ",");
                    String DLoc = CommonLogic.FormCanBeDangerousContent("DownloadLocation");
                    if (DLoc.StartsWith("/"))
                    {
                        DLoc = DLoc.Substring(1, DLoc.Length - 1); // remove leading / char!
                    }
                    sql.Append("DownloadLocation=" + DB.SQuote(DLoc));
                    sql.Append(" where VariantID=" + VariantID.ToString());
                    try
                    {
                        DB.ExecuteSQL(sql.ToString());
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException("Error in EditVariant(.RunSql), Msg=[" + CommonLogic.GetExceptionDetail(ex, String.Empty) + "], Sql=[" + sql.ToString() + "]");
                    }

                    DataUpdated = true;
                    Editing = true;
                }

                // handle shipping costs uploaded (if any):
                if (ShipCalcID == Shipping.ShippingCalculationEnum.UseIndividualItemShippingCosts)
                {
                    DB.ExecuteSQL("delete from ShippingByProduct where VariantID=" + VariantID.ToString());
                    IDataReader rs3 = DB.GetRS("select * from ShippingMethod " + DB.GetNoLock() + " where IsRTShipping=0 order by DisplayOrder");
                    while (rs3.Read())
                    {
                        String FldName = "ShippingCost_" + DB.RSFieldInt(rs3, "ShippingMethodID");
                        decimal ShippingCost = CommonLogic.FormUSDecimal(FldName);
                        DB.ExecuteSQL("insert ShippingByProduct(VariantID,ShippingMethodID,ShippingCost) values(" + VariantID.ToString() + "," + DB.RSFieldInt(rs3, "ShippingMethodID").ToString() + "," + Localization.CurrencyStringForDBWithoutExchangeRate(ShippingCost) + ")");
                    }
                    rs3.Close();
                }

                // handle image uploaded:
                String FN = String.Empty;
                if (CommonLogic.FormCanBeDangerousContent("ImageFilenameOverride").Trim().Length != 0)
                {
                    FN = CommonLogic.FormCanBeDangerousContent("ImageFilenameOverride").Trim();
                }
                if (FN.Length == 0)
                {
                    FN = VariantID.ToString();
                }
                try
                {
                    String Image1 = String.Empty;
                    HttpPostedFile Image1File = Request.Files["Image1"];
                    if (Image1File.ContentLength != 0)
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
                                System.IO.File.Delete(AppLogic.GetImagePath("Variant", "icon", true) + FN + ".jpg");
                                System.IO.File.Delete(AppLogic.GetImagePath("Variant", "icon", true) + FN + ".gif");
                                System.IO.File.Delete(AppLogic.GetImagePath("Variant", "icon", true) + FN + ".png");
                            }
                        }
                        catch
                        { }

                        String s = Image1File.ContentType;
                        switch (Image1File.ContentType)
                        {
                            case "image/gif":
                                Image1 = AppLogic.GetImagePath("Variant", "icon", true) + FN + ".gif";
                                Image1File.SaveAs(Image1);
                                break;
                            case "image/x-png":
                                Image1 = AppLogic.GetImagePath("Variant", "icon", true) + FN + ".png";
                                Image1File.SaveAs(Image1);
                                break;
                            case "image/jpg":
                            case "image/jpeg":
                            case "image/pjpeg":
                                Image1 = AppLogic.GetImagePath("Variant", "icon", true) + FN + ".jpg";
                                Image1File.SaveAs(Image1);
                                break;
                        }
                    }

                    String Image2 = String.Empty;
                    HttpPostedFile Image2File = Request.Files["Image2"];
                    if (Image2File.ContentLength != 0)
                    {
                        // delete any current image file first
                        try
                        {
                            System.IO.File.Delete(AppLogic.GetImagePath("Variant", "medium", true) + FN + ".jpg");
                            System.IO.File.Delete(AppLogic.GetImagePath("Variant", "medium", true) + FN + ".gif");
                            System.IO.File.Delete(AppLogic.GetImagePath("Variant", "medium", true) + FN + ".png");
                        }
                        catch
                        { }

                        String s = Image2File.ContentType;
                        switch (Image2File.ContentType)
                        {
                            case "image/gif":
                                Image2 = AppLogic.GetImagePath("Variant", "medium", true) + FN + ".gif";
                                Image2File.SaveAs(Image2);
                                break;
                            case "image/x-png":
                                Image2 = AppLogic.GetImagePath("Variant", "medium", true) + FN + ".png";
                                Image2File.SaveAs(Image2);
                                break;
                            case "image/jpg":
                            case "image/jpeg":
                            case "image/pjpeg":
                                Image2 = AppLogic.GetImagePath("Variant", "medium", true) + FN + ".jpg";
                                Image2File.SaveAs(Image2);
                                break;
                        }
                    }

                    String Image3 = String.Empty;
                    HttpPostedFile Image3File = Request.Files["Image3"];
                    if (Image3File.ContentLength != 0)
                    {
                        // delete any current image file first
                        try
                        {
                            System.IO.File.Delete(AppLogic.GetImagePath("Variant", "large", true) + VariantID.ToString() + ".jpg");
                            System.IO.File.Delete(AppLogic.GetImagePath("Variant", "large", true) + VariantID.ToString() + ".gif");
                            System.IO.File.Delete(AppLogic.GetImagePath("Variant", "large", true) + VariantID.ToString() + ".png");
                        }
                        catch
                        { }

                        String s = Image3File.ContentType;
                        switch (Image3File.ContentType)
                        {
                            case "image/gif":
                                Image3 = AppLogic.GetImagePath("Variant", "large", true) + VariantID.ToString() + ".gif";
                                Image3File.SaveAs(Image3);
                                break;
                            case "image/x-png":
                                Image3 = AppLogic.GetImagePath("Product", "large", true) + VariantID.ToString() + ".png";
                                Image3File.SaveAs(Image3);
                                break;
                            case "image/jpg":
                            case "image/jpeg":
                            case "image/pjpeg":
                                Image3 = AppLogic.GetImagePath("Variant", "large", true) + VariantID.ToString() + ".jpg";
                                Image3File.SaveAs(Image3);
                                break;
                        }
                    }


                }
                catch (Exception ex)
                {
                    ErrorMsg = CommonLogic.GetExceptionDetail(ex, "<br/>");
                }
            }
            SectionTitle = "<a href=\"variants.aspx?productid=" + ProductID.ToString() + "\">Variants</a> - Manage Variants";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            IDataReader rs = DB.GetRS("select PV.*,P.ColorOptionPrompt,P.SizeOptionPrompt from Product P " + DB.GetNoLock() + " , productvariant PV  " + DB.GetNoLock() + " where PV.ProductID=P.ProductID and VariantID=" + VariantID.ToString());
            if (rs.Read())
            {
                Editing = true;
            }

            bool IsAKit = AppLogic.IsAKit(ProductID);
            bool IsAPack = AppLogic.IsAPack(ProductID);

            String ColorOptionPrompt = DB.RSField(rs, "ColorOptionPrompt");
            String SizeOptionPrompt = DB.RSField(rs, "SizeOptionPrompt");
            if (ColorOptionPrompt.Length == 0)
            {
                ColorOptionPrompt = AppLogic.GetString("AppConfig.ColorOptionPrompt", SkinID, ThisCustomer.LocaleSetting);
            }
            if (SizeOptionPrompt.Length == 0)
            {
                SizeOptionPrompt = AppLogic.GetString("AppConfig.SizeOptionPrompt", SkinID, ThisCustomer.LocaleSetting);
            }

            if (ErrorMsg.Length != 0)
            {
                writer.Write("<p align=\"left\"><b><font color=red>" + ErrorMsg + "</font></b></p>\n");
            }
            if (DataUpdated)
            {
                writer.Write("<p align=\"left\"><b><font color=blue>(UPDATED)</font></b></p>\n");
            }


            if (ErrorMsg.Length == 0)
            {

                writer.Write("<p align=\"left\"><b>Within Product: <a href=\"editproduct.aspx?productid=" + ProductID.ToString() + "\">" + AppLogic.GetProductName(ProductID, ThisCustomer.LocaleSetting) + "</a> (Product SKU=" + AppLogic.GetProductSKU(ProductID) + ", ProductID=" + ProductID.ToString() + ")</b</p>\n");
                if (Editing)
                {
                    writer.Write("<p align=\"left\"><b>Editing Variant: " + DB.RSFieldByLocale(rs, "Name", ThisCustomer.LocaleSetting) + " (Variant SKUSuffix=" + DB.RSField(rs, "SKUSuffix") + ", VariantID=" + DB.RSFieldInt(rs, "VariantID").ToString() + ")</b>");

                    int NumVariants = DB.GetSqlN(String.Format("select count(*) as N from ProductVariant {0} where Deleted=0 and ProductID={1}", DB.GetNoLock(), ProductID.ToString()));
                    writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;");
                    if (NumVariants > 1)
                    {
                        int PreviousVariantID = AppLogic.GetPreviousVariant(ProductID, VariantID);
                        writer.Write("<a class=\"ProductNavLink\" href=\"editvariant.aspx?variantid=" + PreviousVariantID.ToString() + "\"><img src=\"skins/skin_" + SkinID.ToString() + "/images/previous.gif\" border=\"0\" align=\"absmiddle\"></a>&nbsp;&nbsp;");
                    }
                    writer.Write("<a class=\"ProductNavLink\" href=\"variants.aspx?productid=" + ProductID.ToString() + "\"><img src=\"skins/skin_" + SkinID.ToString() + "/images/up.gif\" border=\"0\" align=\"absmiddle\"></a>");
                    if (NumVariants > 1)
                    {
                        int NextVariantID = AppLogic.GetNextVariant(ProductID, VariantID);
                        writer.Write("&nbsp;&nbsp;<a class=\"ProductNavLink\" href=\"editvariant.aspx?variantid=" + NextVariantID.ToString() + "\"><img src=\"skins/skin_" + SkinID.ToString() + "/images/next.gif\" border=\"0\" align=\"absmiddle\"></a>");
                    }
                    writer.Write("</p>\n");
                }
                else
                {
                    writer.Write("<p align=\"left\"><b>Adding New Variant:</p></b>\n");
                }

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function Form_Validator(theForm)\n");
                writer.Write("{\n");
                writer.Write("submitonce(theForm);\n");
                writer.Write("return (true);\n");
                writer.Write("}\n");
                writer.Write("</script>\n");

                if (AppLogic.NumLocaleSettingsInstalled() > 1)
                {
                    writer.Write(CommonLogic.ReadFile("jscripts/tabs.js", true));
                }

                writer.Write("<p align=\"left\">Please enter the following information about this variant. Fields marked with an asterisk (*) are required. All other fields are optional.</p>\n");
                writer.Write("<form enctype=\"multipart/form-data\" action=\"editvariant.aspx?productid=" + ProductID.ToString() + "&VariantID=" + VariantID.ToString() + "&edit=" + Editing.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(document.forms[0]) && Form_Validator(document.forms[0]))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
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
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Variant Name:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Name"), "Name", false, true, false, "Please enter the variant name", 100, 50, 0, 0, false));
                //        writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"Name\" value=\"" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Name")) , "") + "\">\n");
                //				writer.Write("                	<input type=\"hidden\" name=\"Name_vldt\" value=\"[req][blankalert=Please enter the variant name]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">SKU Suffix:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"50\" size=\"30\" name=\"SKUSuffix\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "SKUSuffix")), "") + "\">\n");
                //writer.Write("                	<input type=\"hidden\" name=\"SKUSuffix_vldt\" value=\"[req][blankalert=Please enter the variant SKU Suffix]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Manufacturer Part #:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"50\" size=\"30\" name=\"ManufacturerPartNumber\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "ManufacturerPartNumber")), "") + "\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\" bgcolor=\"" + CommonLogic.IIF(Editing && !DB.RSFieldBool(rs, "Published"), "#FFFFCC", "FFFFFF") + "\">*Published:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" bgcolor=\"" + CommonLogic.IIF(Editing && !DB.RSFieldBool(rs, "Published"), "#FFFFCC", "FFFFFF") + "\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"Published\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "Published"), " checked ", ""), " checked ") + ">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"Published\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "Published"), "", " checked "), "") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Is Recurring:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" >\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"IsRecurring\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "IsRecurring"), " checked ", ""), "") + ">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"IsRecurring\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "IsRecurring"), "", " checked "), " checked ") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                bool UseSpecialRecurringIntervals = AppLogic.UseSpecialRecurringIntervals();
                if (!UseSpecialRecurringIntervals)
                {
                    writer.Write("              <tr valign=\"middle\">\n");
                    writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Recurring Interval:&nbsp;&nbsp;</td>\n");
                    writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    writer.Write("                	<input maxLength=\"10\" size=\"10\" name=\"RecurringInterval\" value=\"" + CommonLogic.IIF(Editing, DB.RSFieldInt(rs, "RecurringInterval").ToString(), "") + "\">\n");
                    writer.Write("                	<input type=\"hidden\" name=\"RecurringInterval_vldt\" value=\"[number][invalidalert=Please enter the recurring interval length for of this variant, as an integer, e.g. 1]\">\n");
                    writer.Write("                	</td>\n");
                    writer.Write("              </tr>\n");
                }
                else
                {
                    writer.Write("                	<input type=\"hidden\" name=\"RecurringInterval\" value=\"1\">\n");
                }

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Recurring Interval Type:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" >\n");
                if (!UseSpecialRecurringIntervals)
                {
                    writer.Write("Day(s)&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"RecurringIntervalType\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldInt(rs, "RecurringIntervalType") == ((int)DateIntervalTypeEnum.Day), " checked ", ""), "") + ">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\n");
                    writer.Write("Week(s)&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"RecurringIntervalType\" value=\"2\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldInt(rs, "RecurringIntervalType") == ((int)DateIntervalTypeEnum.Week), " checked ", ""), "") + ">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\n");
                    writer.Write("Month(s)&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"RecurringIntervalType\" value=\"3\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldInt(rs, "RecurringIntervalType") == ((int)DateIntervalTypeEnum.Month), " checked ", ""), " checked ") + ">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\n");
                    writer.Write("Year(s)&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"RecurringIntervalType\" value=\"4\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldInt(rs, "RecurringIntervalType") == ((int)DateIntervalTypeEnum.Year), " checked ", ""), "") + ">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\n");
                }
                else
                {
                    writer.Write("<select size=\"1\" name=\"RecurringIntervalType\">\n");

                    DateIntervalTypeEnum e;
                    e = DateIntervalTypeEnum.Weekly;
                    writer.Write("<option value=\"" + ((int)e).ToString() + "\"");
                    if (Editing && DB.RSFieldInt(rs, "RecurringIntervalType") == (int)e)
                    {
                        writer.Write(" selected");
                    }
                    writer.Write(">" + e.ToString() + "</option>");

                    e = DateIntervalTypeEnum.BiWeekly;
                    writer.Write("<option value=\"" + ((int)e).ToString() + "\"");
                    if (Editing && DB.RSFieldInt(rs, "RecurringIntervalType") == (int)e)
                    {
                        writer.Write(" selected");
                    }
                    else
                    {
                        writer.Write(" selected"); //default it
                    }
                    writer.Write(">" + e.ToString() + "</option>");

                    //e = DateIntervalTypeEnum.SemiMonthly;
                    //writer.Write("<option value=\"" + ((int)e).ToString() + "\"");
                    //if (Editing && DB.RSFieldInt(rs, "RecurringIntervalType") == (int)e)
                    //{
                    //    writer.Write(" selected");
                    //}
                    //writer.Write(">" + e.ToString() + "</option>");

                    e = DateIntervalTypeEnum.Monthly;
                    writer.Write("<option value=\"" + ((int)e).ToString() + "\"");
                    if (Editing && DB.RSFieldInt(rs, "RecurringIntervalType") == (int)e)
                    {
                        writer.Write(" selected");
                    }
                    writer.Write(">" + e.ToString() + "</option>");

                    e = DateIntervalTypeEnum.EveryFourWeeks;
                    writer.Write("<option value=\"" + ((int)e).ToString() + "\"");
                    if (Editing && DB.RSFieldInt(rs, "RecurringIntervalType") == (int)e)
                    {
                        writer.Write(" selected");
                    }
                    writer.Write(">" + e.ToString() + "</option>");

                    e = DateIntervalTypeEnum.Quarterly;
                    writer.Write("<option value=\"" + ((int)e).ToString() + "\"");
                    if (Editing && DB.RSFieldInt(rs, "RecurringIntervalType") == (int)e)
                    {
                        writer.Write(" selected");
                    }
                    writer.Write(">" + e.ToString() + "</option>");

                    e = DateIntervalTypeEnum.SemiYearly;
                    writer.Write("<option value=\"" + ((int)e).ToString() + "\"");
                    if (Editing && DB.RSFieldInt(rs, "RecurringIntervalType") == (int)e)
                    {
                        writer.Write(" selected");
                    }
                    writer.Write(">" + e.ToString() + "</option>");

                    e = DateIntervalTypeEnum.Yearly;
                    writer.Write("<option value=\"" + ((int)e).ToString() + "\"");
                    if (Editing && DB.RSFieldInt(rs, "RecurringIntervalType") == (int)e)
                    {
                        writer.Write(" selected");
                    }
                    writer.Write(">" + e.ToString() + "</option>");
                }
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Description:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Description"), "Description", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true));
                //        writer.Write("                	<textarea style=\"height: 60em; width: 100%;\" cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeightl") + "\" id=\"Description\" name=\"Description\">" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Description")) , "") + "</textarea>\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Froogle Description (NO HTML):&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                //writer.Write("                	<textarea style=\"height: 60em; width: 100%;\" cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeightl") + "\" id=\"FroogleDescription\" name=\"FroogleDescription\">" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"FroogleDescription")) , "") + "</textarea>\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "FroogleDescription"), "FroogleDescription", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), false));
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                int NumCustomerLevels = DB.GetSqlN("select count(*) as N from dbo.CustomerLevel cl " + DB.GetNoLock() + " join dbo.ProductCustomerLevel pcl " + DB.GetNoLock() + " on cl.CustomerLevelID = pcl.CustomerLevelID where pcl.ProductID = " + ProductID.ToString() + " and cl.deleted = 0");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">Restricted Quantities:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"255\" size=\"100\" name=\"RestrictedQuantities\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "RestrictedQuantities"), "") + "\"> (quantities allowed, e.g. 5, 10, 15, 20, 25)\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Minimum Quantity:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"10\" size=\"10\" name=\"MinimumQuantity\" value=\"" + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldInt(rs, "MinimumQuantity") != 0, DB.RSFieldInt(rs, "MinimumQuantity").ToString(), ""), "") + "\"> (leave blank for no min quantity)\n");
                writer.Write("                	<input type=\"hidden\" name=\"MinimumQuantity_vldt\" value=\"[number][invalidalert=Please enter a valid integer number, e.g. 250!]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Price:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"10\" size=\"10\" name=\"Price\" value=\"" + CommonLogic.IIF(Editing, (Localization.DecimalStringForDB(DB.RSFieldDecimal(rs, "Price"))), "") + "\"> (in format x.xx)");
                if (NumCustomerLevels > 0)
                {
                    writer.Write("&nbsp;&nbsp;<a href=\"editextendedprices.aspx?ProductID=" + ProductID.ToString() + "&VariantID=" + VariantID.ToString() + "\">Define Extended Prices</a> <small>(Defined By Customer Level)</small>\n");
                }
                writer.Write("                	<input type=\"hidden\" name=\"Price_vldt\" value=\"[req][number][blankalert=Please enter the variant price][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Sale Price:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"10\" size=\"10\" name=\"SalePrice\" value=\"" + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldDecimal(rs, "SalePrice") != System.Decimal.Zero, Localization.DecimalStringForDB(DB.RSFieldDecimal(rs, "SalePrice")), ""), "") + "\"> (in format x.xx)\n");
                writer.Write("                	<input type=\"hidden\" name=\"SalePrice_vldt\" value=\"[number][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                if (IsAKit || IsAPack)
                {
                    writer.Write("<input type=\"hidden\" name=\"CustomerEntersPrice\" value=\"0\">");
                    writer.Write("<input type=\"hidden\" name=\"CustomerEntersPricePrompt\" value=\"\">");
                }
                else
                {
                    writer.Write("              <tr valign=\"middle\">\n");
                    writer.Write("                <td align=\"right\" valign=\"middle\">*CustomerEntersPrice:&nbsp;&nbsp;</td>\n");
                    writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"CustomerEntersPrice\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "CustomerEntersPrice"), " checked ", ""), "") + ">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\n");
                    writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"CustomerEntersPrice\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "CustomerEntersPrice"), "", " checked "), " checked ") + ">\n");
                    writer.Write("                </td>\n");
                    writer.Write("              </tr>\n");

                    writer.Write("              <tr valign=\"middle\">\n");
                    writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Customer Enters Price Prompt:&nbsp;&nbsp;</td>\n");
                    writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "CustomerEntersPricePrompt"), "CustomerEntersPricePrompt", false, true, false, String.Empty, 100, 50, 0, 0, false));
                    writer.Write("                	</td>\n");
                    writer.Write("              </tr>\n");
                }

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">MSRP:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"10\" size=\"10\" name=\"MSRP\" value=\"" + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldDecimal(rs, "MSRP") != System.Decimal.Zero, Localization.CurrencyStringForDBWithoutExchangeRate(DB.RSFieldDecimal(rs, "MSRP")), ""), "") + "\"> (in format x.xx)\n");
                writer.Write("                	<input type=\"hidden\" name=\"MSRP_vldt\" value=\"[number][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Actual Cost:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"10\" size=\"10\" name=\"Cost\" value=\"" + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldDecimal(rs, "Cost") != System.Decimal.Zero, Localization.CurrencyStringForDBWithoutExchangeRate(DB.RSFieldDecimal(rs, "Cost")), ""), "") + "\"> (in format x.xx)\n");
                writer.Write("                	<input type=\"hidden\" name=\"Cost_vldt\" value=\"[number][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Is Taxable:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"IsTaxable\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "IsTaxable"), " checked ", ""), " checked ") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"IsTaxable\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "IsTaxable"), "", " checked "), "") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                if (ShipCalcID == Shipping.ShippingCalculationEnum.UseIndividualItemShippingCosts)
                {
                    writer.Write("<tr>\n");
                    writer.Write("<td width=\"25%\" align=\"right\" valign=\"top\">Shipping Cost:&nbsp;&nbsp;</td>\n");
                    writer.Write("<td align=\"left\" valign=\"top\">\n");
                    int NF = 0;
                    IDataReader rs3 = DB.GetRS("select * from ShippingMethod " + DB.GetNoLock() + " where IsRTShipping=0 order by DisplayOrder");
                    while (rs3.Read())
                    {
                        writer.Write(DB.RSFieldByLocale(rs3, "Name", ThisCustomer.LocaleSetting) + ": ");
                        writer.Write("<input maxLength=\"10\" size=\"10\" name=\"ShippingCost_" + DB.RSFieldInt(rs3, "ShippingMethodID") + "\" value=\"" + CommonLogic.IIF(Editing, Localization.CurrencyStringForDBWithoutExchangeRate(Shipping.GetVariantShippingCost(VariantID, DB.RSFieldInt(rs3, "ShippingMethodID"))), "") + "\">  (in format x.xx)\n");
                        writer.Write("<input type=\"hidden\" name=\"ShippingCost_" + DB.RSFieldInt(rs3, "ShippingMethodID") + "_vldt\" value=\"[number][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                        writer.Write("<br/><br/>");
                        NF++;
                    }
                    rs3.Close();
                    if (NF == 0)
                    {
                        writer.Write("No Shipping Methods Are Found");
                    }
                    writer.Write("<INPUT TYPE=\"hidden\" NAME=\"IsShipSeparately\" value=\"0\">");
                    writer.Write("</td>\n");
                    writer.Write("</tr>\n");
                }
                else
                {
                    writer.Write("              <tr valign=\"middle\">\n");
                    writer.Write("                <td align=\"right\" valign=\"middle\">*Is Ship Separately:&nbsp;&nbsp;</td>\n");
                    writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"IsShipSeparately\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "IsShipSeparately"), " checked ", ""), "") + ">\n");
                    writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"IsShipSeparately\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "IsShipSeparately"), "", " checked "), " checked ") + ">\n");
                    writer.Write("                </td>\n");
                    writer.Write("              </tr>\n");
                }

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Is Free Shipping:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"FreeShipping\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "FreeShipping"), " checked ", ""), "  ") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"FreeShipping\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "FreeShipping"), "", " checked "), " checked ") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Is Download:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"IsDownload\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "IsDownload"), " checked ", ""), "  ") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"IsDownload\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "IsDownload"), "", " checked "), " checked ") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Download Location:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"1000\" size=\"100\" name=\"DownloadLocation\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "DownloadLocation")), "") + "\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Weight:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"10\" size=\"10\" name=\"Weight\" value=\"" + CommonLogic.IIF(Editing, Localization.DecimalStringForDB(DB.RSFieldDecimal(rs, "Weight")), "") + "\"> <small>(in format x.xx, in " + Localization.WeightUnits() + ")</small>\n");
                writer.Write("                	<input type=\"hidden\" name=\"Weight_vldt\" value=\"[number][invalidalert=Please enter the weight of this item in " + Localization.WeightUnits() + ", e.g. 2.5]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Dimensions:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"50\" size=\"30\" name=\"Dimensions\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "Dimensions")), "") + "\"> MUST be in format: N.NN x N.NN x N.NN, Height x Width x Depth, in inches, e.g. 4.5 x 7.8 x 2\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                if (ProductTracksInventoryBySizeAndColor)
                {
                    writer.Write("              <tr valign=\"middle\">\n");
                    writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Manage Inventory:&nbsp;&nbsp;</td>\n");
                    writer.Write("                <td align=\"left\" valign=\"top\">\n");
                    writer.Write("                	<a href=\"editinventory.aspx?productid=" + ProductID.ToString() + "&variantid=" + VariantID.ToString() + "\">Click Here</a>\n");
                    writer.Write("                	</td>\n");
                    writer.Write("              </tr>\n");
                }
                else
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
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Subscription Interval:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"10\" size=\"10\" name=\"SubscriptionInterval\" value=\"" + CommonLogic.IIF(Editing, DB.RSFieldInt(rs, "SubscriptionInterval").ToString(), "") + "\">\n");
                writer.Write("                	<input type=\"hidden\" name=\"SubscriptionInterval_vldt\" value=\"[number][invalidalert=Please enter the Subscription interval length for of this variant, as an integer, e.g. 1]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Subscription Interval Type:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" >\n");
                writer.Write("Day(s)&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"SubscriptionIntervalType\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldInt(rs, "SubscriptionIntervalType") == ((int)DateIntervalTypeEnum.Day), " checked ", ""), "") + ">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\n");
                writer.Write("Week(s)&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"SubscriptionIntervalType\" value=\"2\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldInt(rs, "SubscriptionIntervalType") == ((int)DateIntervalTypeEnum.Week), " checked ", ""), "") + ">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\n");
                writer.Write("Month(s)&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"SubscriptionIntervalType\" value=\"3\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldInt(rs, "SubscriptionIntervalType") == ((int)DateIntervalTypeEnum.Month), " checked ", ""), " checked ") + ">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\n");
                writer.Write("Year(s)&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"SubscriptionIntervalType\" value=\"4\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldInt(rs, "SubscriptionIntervalType") == ((int)DateIntervalTypeEnum.Year), " checked ", ""), "") + ">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\n");
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
                writer.Write("                	<input maxLength=\"100\" size=\"40\" name=\"ImageFilenameOverride\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "ImageFilenameOverride"), "") + "\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("  <tr>\n");
                writer.Write("    <td valign=\"top\" align=\"right\">Icon (VariantID=" + VariantID.ToString() + "):\n");
                writer.Write("</td>\n");
                writer.Write("    <td valign=\"top\" align=\"left\">");
                writer.Write("    <input type=\"file\" name=\"Image1\" size=\"30\" value=\"" + CommonLogic.IIF(Editing, "", "") + "\">\n");
                String Image1URL = AppLogic.LookupImage("Variant", VariantID, "icon", SkinID, ThisCustomer.LocaleSetting);
                if (Image1URL.Length != 0)
                {
                    if (Image1URL.IndexOf("nopicture") == -1)
                    {
                        writer.Write("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image1URL + "','Pic1');\">Click here</a> to delete the current image<br/>\n");
                    }
                    writer.Write("<br/><img id=\"Pic1\" name=\"Pic1\" border=\"0\" src=\"" + Image1URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\">\n");
                }
                writer.Write("</td>\n");
                writer.Write(" </tr>\n");

                writer.Write("  <tr>\n");
                writer.Write("    <td valign=\"top\" align=\"right\">Medium Pic (VariantID=" + VariantID.ToString() + "):\n");
                writer.Write("</td>\n");
                writer.Write("    <td valign=\"top\" align=\"left\">");
                writer.Write("    <input type=\"file\" name=\"Image2\" size=\"30\" value=\"" + CommonLogic.IIF(Editing, "", "") + "\">\n");
                String Image2URL = AppLogic.LookupImage("Variant", VariantID, "medium", SkinID, ThisCustomer.LocaleSetting);
                if (Image2URL.Length != 0)
                {
                    if (Image2URL.IndexOf("nopicture") == -1)
                    {
                        writer.Write("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image2URL + "','Pic2');\">Click here</a> to delete the current image<br/>\n");
                    }
                    writer.Write("<br/><img id=\"Pic2\" name=\"Pic2\" border=\"0\" src=\"" + Image2URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\">\n");
                }
                writer.Write("</td>\n");
                writer.Write(" </tr>\n");

                writer.Write("  <tr>\n");
                writer.Write("    <td valign=\"top\" align=\"right\">Large Pic (VariantID=" + VariantID.ToString() + "):\n");
                writer.Write("</td>\n");
                writer.Write("    <td valign=\"top\" align=\"left\">");
                writer.Write("    <input type=\"file\" name=\"Image3\" size=\"30\" value=\"" + CommonLogic.IIF(Editing, "", "") + "\">\n");
                String Image3URL = AppLogic.LookupImage("Variant", VariantID, "large", SkinID, ThisCustomer.LocaleSetting);
                if (Image3URL.Length != 0)
                {
                    if (Image3URL.IndexOf("nopicture") == -1)
                    {
                        writer.Write("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image3URL + "','Pic3');\">Click here</a> to delete the current image<br/>\n");
                    }
                    writer.Write("<br/><img id=\"Pic3\" name=\"Pic3\" border=\"0\" src=\"" + Image3URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\">\n");
                }
                writer.Write("</td>\n");
                writer.Write(" </tr>\n");

                // COLORS & SIZES:
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">" + ColorOptionPrompt + "(s):&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Colors"), "Colors", false, true, false, "", 1000, 60, 0, 0, false));
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">" + ColorOptionPrompt + " SKU Modifiers:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"750\" size=\"50\" name=\"ColorSKUModifiers\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "ColorSKUModifiers")), "") + "\">&nbsp;<small>(Separate skus by commas)</small>\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">" + SizeOptionPrompt + "(s):&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Sizes"), "Sizes", false, true, false, "", 1000, 60, 0, 0, false));
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">" + SizeOptionPrompt + " SKU Modifiers:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"750\" size=\"50\" name=\"SizeSKUModifiers\" value=\"" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "SizeSKUModifiers")), "") + "\">&nbsp;<small>(Separate skus by commas)</small>\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                //				writer.Write("              <tr valign=\"middle\">\n");
                //				writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">" + SizeOptionPrompt + " 2(s):&nbsp;&nbsp;</td>\n");
                //				writer.Write("                <td align=\"left\" valign=\"top\">\n");
                //				writer.Write("                	<input maxLength=\"750\" size=\"50\" name=\"Sizes2\" value=\"" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Sizes2")) , "") + "\">&nbsp;<small>(Separate " + SizeOptionPrompt.ToLowerInvariant() + "(s) by commas)</small>\n");
                //				writer.Write("                	</td>\n");
                //				writer.Write("              </tr>\n");
                //
                //				writer.Write("              <tr valign=\"middle\">\n");
                //				writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">" + SizeOptionPrompt + " SKU Modifiers 2:&nbsp;&nbsp;</td>\n");
                //				writer.Write("                <td align=\"left\" valign=\"top\">\n");
                //				writer.Write("                	<input maxLength=\"750\" size=\"50\" name=\"SizeSKUModifiers2\" value=\"" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"SizeSKUModifiers2")) , "") + "\">&nbsp;<small>(Separate skus by commas)</small>\n");
                //				writer.Write("                	</td>\n");
                //				writer.Write("              </tr>\n");

                // size/color tables for display purposes only:
                if (Editing && (DB.RSField(rs, "Colors").Length != 0 || DB.RSField(rs, "ColorSKUModifiers").Length != 0 || DB.RSField(rs, "Sizes").Length != 0 || DB.RSField(rs, "SizeSKUModifiers").Length != 0))
                {
                    writer.Write("<tr valign=\"left\"><td colspan=\"2\" height=\"10\"></td></tr>\n");
                    writer.Write("<tr valign=\"left\">\n");
                    writer.Write("<td width=\"25%\" align=\"right\" valign=\"top\">" + ColorOptionPrompt + "/" + SizeOptionPrompt + " Tables:&nbsp;&nbsp;<br/><small>(summary)</small></td>\n");
                    writer.Write("<td align=\"left\" valign=\"top\">\n");

                    String[] Colors = DB.RSField(rs, "Colors").Split(',');
                    String[] Sizes = DB.RSField(rs, "Sizes").Split(',');
                    String[] ColorSKUModifiers = DB.RSField(rs, "ColorSKUModifiers").Split(',');
                    String[] SizeSKUModifiers = DB.RSField(rs, "SizeSKUModifiers").Split(',');

                    for (int i = Colors.GetLowerBound(0); i <= Colors.GetUpperBound(0); i++)
                    {
                        Colors[i] = Colors[i].Trim();
                    }

                    for (int i = Sizes.GetLowerBound(0); i <= Sizes.GetUpperBound(0); i++)
                    {
                        Sizes[i] = Sizes[i].Trim();
                    }

                    for (int i = ColorSKUModifiers.GetLowerBound(0); i <= ColorSKUModifiers.GetUpperBound(0); i++)
                    {
                        ColorSKUModifiers[i] = ColorSKUModifiers[i].Trim();
                    }

                    for (int i = SizeSKUModifiers.GetLowerBound(0); i <= SizeSKUModifiers.GetUpperBound(0); i++)
                    {
                        SizeSKUModifiers[i] = SizeSKUModifiers[i].Trim();
                    }

                    int ColorCols = Colors.GetUpperBound(0);
                    int SizeCols = Sizes.GetUpperBound(0);
                    ColorCols = Math.Max(ColorCols, ColorSKUModifiers.GetUpperBound(0));
                    SizeCols = Math.Max(SizeCols, SizeSKUModifiers.GetUpperBound(0));

                    if (DB.RSField(rs, "Colors").Length != 0 || DB.RSField(rs, "ColorSKUModifiers").Length != 0)
                    {
                        writer.Write("<table cellpadding=\"2\" cellspacing=\"0\" border=\"1\">\n");
                        writer.Write("<tr>\n");
                        writer.Write("<td><b>" + ColorOptionPrompt + "</b></td>\n");
                        for (int i = 0; i <= ColorCols; i++)
                        {
                            String ColorVal = String.Empty;
                            try
                            {
                                ColorVal = Colors[i];
                            }
                            catch { }
                            if (ColorVal.Length == 0)
                            {
                                ColorVal = "&nbsp;";
                            }
                            writer.Write("<td align=\"center\">" + ColorVal + "</td>\n");
                        }
                        writer.Write("<tr>\n");
                        writer.Write("<tr>\n");
                        writer.Write("<td><b>SKU Modifier</b></td>\n");
                        for (int i = 0; i <= ColorCols; i++)
                        {
                            String SKUVal = String.Empty;
                            try
                            {
                                SKUVal = ColorSKUModifiers[i];
                            }
                            catch { }
                            if (SKUVal.Length == 0)
                            {
                                SKUVal = "&nbsp;";
                            }
                            writer.Write("<td align=\"center\">" + SKUVal + "</td>\n");
                        }
                        writer.Write("<tr>\n");
                        writer.Write("</table>\n");
                        writer.Write("<br/><br/>");
                    }


                    if (DB.RSField(rs, "Sizes").Length != 0 || DB.RSField(rs, "SizeSKUModifiers").Length != 0)
                    {
                        writer.Write("<table cellpadding=\"2\" cellspacing=\"0\" border=\"1\">\n");
                        writer.Write("<tr>\n");
                        writer.Write("<td><b>" + SizeOptionPrompt + "</b></td>\n");
                        for (int i = 0; i <= SizeCols; i++)
                        {
                            String SizeVal = String.Empty;
                            try
                            {
                                SizeVal = Sizes[i];
                            }
                            catch { }
                            if (SizeVal.Length == 0)
                            {
                                SizeVal = "&nbsp;";
                            }
                            writer.Write("<td align=\"center\">" + SizeVal + "</td>\n");
                        }
                        writer.Write("<tr>\n");
                        writer.Write("<tr>\n");
                        writer.Write("<td><b>SKU Modifier</b></td>\n");
                        for (int i = 0; i <= SizeCols; i++)
                        {
                            String SKUVal = String.Empty;
                            try
                            {
                                SKUVal = SizeSKUModifiers[i];
                            }
                            catch { }
                            if (SKUVal.Length == 0)
                            {
                                SKUVal = "&nbsp;";
                            }
                            writer.Write("<td align=\"center\">" + SKUVal + "</td>\n");
                        }
                        writer.Write("<tr>\n");
                        writer.Write("</table>\n");
                    }

                    writer.Write("</td>\n");
                    writer.Write("</tr>\n");
                }

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
                writer.Write("  </table>\n");
                writer.Write("</form>\n");

                writer.Write("<script type=\"text/javascript\">\n");

                writer.Write("function DeleteImage(imgurl,name)\n");
                writer.Write("{\n");
                writer.Write("window.open('deleteimage.aspx?imgurl=' + imgurl + '&FormImageName=' + name,\"alikaussadmin_ML\",\"height=250,width=440,top=10,left=20,status=no,toolbar=no,menubar=no,scrollbars=yes,location=no\")\n");
                writer.Write("}\n");

                writer.Write("</SCRIPT>\n");

            }
            rs.Close();
        }

    }
}
