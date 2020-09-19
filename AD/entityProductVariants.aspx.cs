// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/entityProductVariants.aspx.cs 4     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for variants.
    /// </summary>
    public partial class entityProductVariants : System.Web.UI.Page
    {
        private int ProductID = 0;
        private String ProductName;
        private String ProductSKU;
        private bool ProductTracksInventoryBySizeAndColor;
        private Customer ThisCustomer;
        private int skinID = 1;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            ProductID = CommonLogic.QueryStringUSInt("ProductID");
            if (ProductID == 0)
            {
                Response.Redirect("products.aspx");
            }

            ProductName = AppLogic.GetProductName(ProductID, ThisCustomer.LocaleSetting);
            ProductSKU = AppLogic.GetProductSKU(ProductID);

            ProductTracksInventoryBySizeAndColor = AppLogic.ProductTracksInventoryBySizeAndColor(ProductID);

            if (CommonLogic.QueryStringCanBeDangerousContent("CloneID").Length != 0)
            {
                int CloneID = CommonLogic.QueryStringUSInt("CloneID");
                DB.ExecuteSQL("aspdnsf_CloneVariant " + CloneID.ToString() + "," + ThisCustomer.CustomerID.ToString());
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
            {
                int DeleteID = CommonLogic.QueryStringUSInt("DeleteID");
                DB.ExecuteSQL("delete from CustomCart where VariantID=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from KitCart where VariantID=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ShoppingCart where VariantID=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ProductVariant where VariantID=" + DeleteID.ToString());
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
                DB.ExecuteSQL("update ProductVariant set IsDefault=0 where ProductID=" + ProductID.ToString());
                if (CommonLogic.FormCanBeDangerousContent("IsDefault").Length == 0 || CommonLogic.FormUSInt("IsDefault") == 0)
                {
                    // try to force a default variant, none was specified!
                    DB.ExecuteSQL("update ProductVariant set IsDefault=1 where ProductID=" + ProductID.ToString() + " and VariantID in (SELECT top 1 VariantID from ProductVariant where ProductID=" + ProductID.ToString() + " order by DisplayOrder,Name)");
                }
                else
                {
                    DB.ExecuteSQL("update ProductVariant set IsDefault=1 where ProductID=" + ProductID.ToString() + " and VariantID=" + CommonLogic.FormUSInt("IsDefault").ToString());
                }
                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    if (Request.Form.Keys[i].IndexOf("DisplayOrder_") != -1)
                    {
                        String[] keys = Request.Form.Keys[i].Split('_');
                        int VariantID = Localization.ParseUSInt(keys[1]);
                        int DispOrd = 1;
                        try
                        {
                            DispOrd = Localization.ParseUSInt(Request.Form[Request.Form.Keys[i]]);
                        }
                        catch { }
                        DB.ExecuteSQL("update productvariant set DisplayOrder=" + DispOrd.ToString() + " where VariantID=" + VariantID.ToString());
                    }
                }
            } 
            
            if (CommonLogic.QueryStringCanBeDangerousContent("DeleteAllVariants").ToUpperInvariant() != "TRUE")
            {
                AppLogic.MakeSureProductHasAtLeastOneVariant(ProductID);
            }
            AppLogic.EnsureProductHasADefaultVariantSet(ProductID);

            LoadData();
        }

        protected void LoadData()
        {
            string temp = "";

            DataSet ds = DB.GetDS("select * from productvariant  " + DB.GetNoLock() + " where deleted=0 and ProductID=" + ProductID.ToString() + " order by DisplayOrder,Name", false);

            temp += ("<form id=\"Form1\" name=\"Form1\" method=\"POST\" action=\"entityProductVariants.aspx?ProductID=" + CommonLogic.QueryStringNativeInt("ProductID") + "&entityname=" + CommonLogic.QueryStringCanBeDangerousContent("entityname") + "&EntityID=" + CommonLogic.QueryStringNativeInt("EntityID") + "\">\n");
            temp += ("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
            temp += ("<table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
            temp += ("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
            temp += ("<td><b>ID</b></td>\n");
            temp += ("<td><b>Variant</b></td>\n");
            temp += ("<td><b>Variant SKU Suffix</b></td>\n");
            temp += ("<td><b>Full SKU</b></td>\n");
            temp += ("<td><b>Price</b></td>\n");
            temp += ("<td><b>Sale Price</b></td>\n");
            temp += ("<td align=\"center\"><b>Inventory</b></td>\n");
            temp += ("<td align=\"center\"><b>Display Order</b></td>\n");
            temp += ("<td align=\"center\"><b>Is Default Variant</b></td>\n");
            temp += ("<td align=\"center\"><b>Clone</b></td>\n");
            temp += ("<td align=\"center\"><b>Move</b></td>\n");
            temp += ("<td align=\"center\"><b>Delete</b></td>\n");
            temp += ("</tr>\n");
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                temp += ("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                temp += ("<td >" + DB.RowFieldInt(row, "VariantID").ToString() + "</td>\n");
                temp += ("<td >");
                String Image1URL = AppLogic.LookupImage("Variant", DB.RowFieldInt(row, "VariantID"), "icon", skinID, ThisCustomer.LocaleSetting);
                if (Image1URL.Length != 0)
                {
                    temp += ("<a href=\"entityEditProductVariant.aspx?ProductID=" + CommonLogic.QueryStringNativeInt("ProductID") + "&entityname=" + CommonLogic.QueryStringCanBeDangerousContent("entityname") + "&EntityID=" + CommonLogic.QueryStringNativeInt("EntityID") + "&Variantid=" + DB.RowFieldInt(row, "VariantID").ToString() + "\">");
                    temp += ("<img src=\"" + Image1URL + "\" height=\"35\" border=\"0\" align=\"absmiddle\">");
                    temp += ("</a>&nbsp;\n");
                }
                temp += ("<a href=\"entityEditProductVariant.aspx?ProductID=" + CommonLogic.QueryStringNativeInt("ProductID") + "&entityname=" + CommonLogic.QueryStringCanBeDangerousContent("entityname") + "&EntityID=" + CommonLogic.QueryStringNativeInt("EntityID") + "&Variantid=" + DB.RowFieldInt(row, "VariantID").ToString() + "\">" + CommonLogic.IIF(DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting).Length == 0, "(Unnamed Variant)", DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting)) + "</a>");
                temp += ("</td>\n");
                temp += ("<td >" + DB.RowField(row, "SKUSuffix") + "</td>\n");
                temp += ("<td >" + AppLogic.GetProductSKU(ProductID) + DB.RowField(row, "SKUSuffix") + "</td>\n");
                temp += ("<td >" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(row, "Price")) + "</td>\n");
                temp += ("<td >" + CommonLogic.IIF(DB.RowFieldDecimal(row, "SalePrice") != System.Decimal.Zero, Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(row, "SalePrice")), "&nbsp;") + "</td>\n");
                if (ProductTracksInventoryBySizeAndColor)
                {
                    //temp += ("<td  align=\"center\"><a href=\"editinventory.aspx?productid=" + ProductID.ToString() + "&variantid=" + DB.RowFieldInt(row, "VariantID").ToString() + "\">Click Here</a></td>\n");
                    temp += ("<td  align=\"center\"><a href=\"entityEditInventory.aspx?productid=" + ProductID.ToString() + "&variantid=" + DB.RowFieldInt(row, "VariantID").ToString() + "\">Click Here</a></td>\n");
                }
                else
                {
                    temp += ("<td  align=\"center\">");
                    temp += (DB.RowFieldInt(row, "Inventory").ToString());
                    temp += ("</td>");
                }
                temp += ("<td align=\"center\"><input size=2 type=\"text\" name=\"DisplayOrder_" + DB.RowFieldInt(row, "VariantID").ToString() + "\" value=\"" + DB.RowFieldInt(row, "DisplayOrder").ToString() + "\"></td>\n");
                temp += ("<td align=\"center\"><input type=\"radio\" name=\"IsDefault\" value=\"" + DB.RowFieldInt(row, "VariantID").ToString() + "\" " + CommonLogic.IIF(DB.RowFieldBool(row, "IsDefault"), " checked ", "") + "></td>\n");
                temp += ("<td align=\"center\">");
                if (DB.RowFieldBool(row, "IsSystem"))
                {
                    temp += ("System<br/>Product"); // this type of product can only be deleted in the db!
                }
                else
                {
                    temp += ("<input type=\"button\" value=\"Clone\" name=\"Clone_" + DB.RowFieldInt(row, "ProductID").ToString() + "\" onClick=\"if(confirm('Are you sure you want to create a clone of this variant?')) {self.location='entityProductVariants.aspx?ProductID=" + CommonLogic.QueryStringNativeInt("ProductID") + "&entityname=" + CommonLogic.QueryStringCanBeDangerousContent("entityname") + "&EntityID=" + CommonLogic.QueryStringNativeInt("EntityID") + "&cloneid=" + DB.RowFieldInt(row, "VariantID").ToString() + "';}\">\n");
                }
                temp += ("</td>");
                temp += ("<td align=\"center\"><input type=\"button\" value=\"Move\" name=\"Move_" + DB.RowFieldInt(row, "VariantID").ToString() + "\" onClick=\"self.location='entityMoveVariant.aspx?productid=" + ProductID.ToString() + "&Variantid=" + DB.RowFieldInt(row, "VariantID").ToString() + "'\"></td>\n");
                temp += ("<td align=\"center\"><input type=\"button\" value=\"Delete\" name=\"Delete_" + DB.RowFieldInt(row, "VariantID").ToString() + "\" onClick=\"DeleteVariant(" + DB.RowFieldInt(row, "VariantID").ToString() + ")\"></td>\n");
                temp += ("</tr>\n");
            }
            ds.Dispose();
            temp += ("<tr>\n");
            temp += ("<td colspan=\"7\" align=\"left\"></td>\n");
            temp += ("<td colspan=\"2\" align=\"center\" bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></td>\n");
            temp += ("<td colspan=\"3\"></td>\n");
            temp += ("</tr>\n");
            temp += ("</table>\n");
            //temp += ("<input type=\"button\" value=\"Add New Variant\" name=\"AddNew\" onClick=\"self.location='editVariant.aspx?productid=" + ProductID.ToString() + "';\">\n");

            temp += ("</form>\n");

            temp += ("<script type=\"text/javascript\">\n");
            temp += ("function DeleteVariant(id)\n");
            temp += ("{\n");
            temp += ("if(confirm('Are you sure you want to delete Variant: ' + id + '? This will also remove all items from customer shopping carts, wish lists, and gift registries IF they reference this variant!'))\n");
            temp += ("{\n");
            temp += ("self.location = 'Variants.aspx?productid=" + ProductID.ToString() + "&deleteid=' + id;\n");
            temp += ("}\n");
            temp += ("}\n");
            temp += ("</SCRIPT>\n");

            ltContent.Text = temp;
        }

    }
}
