// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/variants.aspx.cs 6     9/30/06 3:38p Redwoodtree $
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
    public partial class variants : AspDotNetStorefront.SkinBase
    {
        int ProductID = 0;
        String ProductName;
        String ProductSKU;
        bool ProductTracksInventoryBySizeAndColor;

        protected void Page_Load(object sender, System.EventArgs e)
        {
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
            
            SectionTitle = "<a href=\"products.aspx\">Manage Products</a> - Add/Edit Variants";
            if (CommonLogic.QueryStringCanBeDangerousContent("DeleteAllVariants").ToUpperInvariant() != "TRUE")
            {
                AppLogic.MakeSureProductHasAtLeastOneVariant(ProductID);
            }
            AppLogic.EnsureProductHasADefaultVariantSet(ProductID);
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write("<p align=\"left\"<b>Editing Variants for Product: <a href=\"editproduct.aspx?productid=" + ProductID.ToString() + "\">" + ProductName + "</a> (Product SKU=" + ProductSKU + ", ProductID=" + ProductID.ToString() + ")</b>");
            writer.Write("&nbsp;&nbsp;");
            writer.Write("<a class=\"ProductNavLink\" href=\"editproduct.aspx?productid=" + ProductID.ToString() + "\"><img src=\"skins/skin_" + SkinID.ToString() + "/images/up.gif\" border=\"0\" align=\"absmiddle\"></a>");

            writer.Write("&nbsp;&nbsp;");
            writer.Write("<input style=\"font-size: 9px;\" type=\"button\" value=\"Delete All Variants\" name=\"DeleteAllVariants_" + ProductID.ToString() + "\" onClick=\"if(confirm('Are you sure you want to delete ALL variants for this product? This will also remove all items from customer shopping carts, wish lists, and gift registries IF they reference any of these variants!')){self.location='variants.aspx?DeleteAllVariants=true&productid=" + ProductID.ToString() + "';}\">");

            writer.Write("</p>\n");

            DataSet ds = DB.GetDS("select * from productvariant  " + DB.GetNoLock() + " where deleted=0 and ProductID=" + ProductID.ToString() + " order by DisplayOrder,Name", false);

            writer.Write("<form id=\"Form1\" name=\"Form1\" method=\"POST\" action=\"variants.aspx?productid=" + ProductID.ToString() + "\">\n");
            writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
            writer.Write("<table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
            writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
            writer.Write("<td><b>ID</b></td>\n");
            writer.Write("<td><b>Variant</b></td>\n");
            writer.Write("<td><b>Variant SKU Suffix</b></td>\n");
            writer.Write("<td><b>Full SKU</b></td>\n");
            writer.Write("<td><b>Price</b></td>\n");
            writer.Write("<td><b>Sale Price</b></td>\n");
            writer.Write("<td align=\"center\"><b>Inventory</b></td>\n");
            writer.Write("<td align=\"center\"><b>Display Order</b></td>\n");
            writer.Write("<td align=\"center\"><b>Is Default Variant</b></td>\n");
            writer.Write("<td align=\"center\"><b>Edit</b></td>\n");
            writer.Write("<td align=\"center\"><b>Clone</b></td>\n");
            writer.Write("<td align=\"center\"><b>Move</b></td>\n");
            writer.Write("<td align=\"center\"><b>Delete</b></td>\n");
            writer.Write("</tr>\n");
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                writer.Write("<td >" + DB.RowFieldInt(row, "VariantID").ToString() + "</td>\n");
                writer.Write("<td >");
                String Image1URL = AppLogic.LookupImage("Variant", DB.RowFieldInt(row, "VariantID"), "icon", SkinID, ThisCustomer.LocaleSetting);
                if (Image1URL.Length != 0)
                {
                    writer.Write("<a href=\"editVariant.aspx?Variantid=" + DB.RowFieldInt(row, "VariantID").ToString() + "\">");
                    writer.Write("<img src=\"" + Image1URL + "\" height=\"35\" border=\"0\" align=\"absmiddle\">");
                    writer.Write("</a>&nbsp;\n");
                }
                writer.Write("<a href=\"editVariant.aspx?productid=" + ProductID.ToString() + "&Variantid=" + DB.RowFieldInt(row, "VariantID").ToString() + "\">" + CommonLogic.IIF(DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting).Length == 0, "(Unnamed Variant)", DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting)) + "</a>");
                writer.Write("</td>\n");
                writer.Write("<td >" + DB.RowField(row, "SKUSuffix") + "</td>\n");
                writer.Write("<td >" + AppLogic.GetProductSKU(ProductID) + DB.RowField(row, "SKUSuffix") + "</td>\n");
                writer.Write("<td >" + ThisCustomer.CurrencyString(DB.RowFieldDecimal(row, "Price")) + "</td>\n");
                writer.Write("<td >" + CommonLogic.IIF(DB.RowFieldDecimal(row, "SalePrice") != System.Decimal.Zero, ThisCustomer.CurrencyString(DB.RowFieldDecimal(row, "SalePrice")), "&nbsp;") + "</td>\n");
                if (ProductTracksInventoryBySizeAndColor)
                {
                    writer.Write("<td  align=\"center\"><a href=\"editinventory.aspx?productid=" + ProductID.ToString() + "&variantid=" + DB.RowFieldInt(row, "VariantID").ToString() + "\">Click Here</a></td>\n");
                }
                else
                {
                    writer.Write("<td  align=\"center\">");
                    writer.Write(DB.RowFieldInt(row, "Inventory").ToString());
                    writer.Write("</td>");
                }
                writer.Write("<td align=\"center\"><input size=2 type=\"text\" name=\"DisplayOrder_" + DB.RowFieldInt(row, "VariantID").ToString() + "\" value=\"" + DB.RowFieldInt(row, "DisplayOrder").ToString() + "\"></td>\n");
                writer.Write("<td align=\"center\"><input type=\"radio\" name=\"IsDefault\" value=\"" + DB.RowFieldInt(row, "VariantID").ToString() + "\" " + CommonLogic.IIF(DB.RowFieldBool(row, "IsDefault"), " checked ", "") + "></td>\n");
                writer.Write("<td align=\"center\"><input type=\"button\" value=\"Edit\" name=\"Edit_" + DB.RowFieldInt(row, "VariantID").ToString() + "\" onClick=\"self.location='editVariant.aspx?productid=" + ProductID.ToString() + "&Variantid=" + DB.RowFieldInt(row, "VariantID").ToString() + "'\"></td>\n");
                writer.Write("<td align=\"center\">");
                if (DB.RowFieldBool(row, "IsSystem"))
                {
                    writer.Write("System<br/>Product"); // this type of product can only be deleted in the db!
                }
                else
                {
                    writer.Write("<input type=\"button\" value=\"Clone\" name=\"Clone_" + DB.RowFieldInt(row, "ProductID").ToString() + "\" onClick=\"if(confirm('Are you sure you want to create a clone of this variant?')) {self.location='variants.aspx?productid=" + ProductID.ToString() + "&cloneid=" + DB.RowFieldInt(row, "VariantID").ToString() + "';}\">\n");
                }
                writer.Write("</td>");
                writer.Write("<td align=\"center\"><input type=\"button\" value=\"Move\" name=\"Move_" + DB.RowFieldInt(row, "VariantID").ToString() + "\" onClick=\"self.location='moveVariant.aspx?productid=" + ProductID.ToString() + "&Variantid=" + DB.RowFieldInt(row, "VariantID").ToString() + "'\"></td>\n");
                writer.Write("<td align=\"center\"><input type=\"button\" value=\"Delete\" name=\"Delete_" + DB.RowFieldInt(row, "VariantID").ToString() + "\" onClick=\"DeleteVariant(" + DB.RowFieldInt(row, "VariantID").ToString() + ")\"></td>\n");
                writer.Write("</tr>\n");
            }
            ds.Dispose();
            writer.Write("<tr>\n");
            writer.Write("<td colspan=\"7\" align=\"left\"></td>\n");
            writer.Write("<td colspan=\"2\" align=\"center\" bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></td>\n");
            writer.Write("<td colspan=\"3\"></td>\n");
            writer.Write("</tr>\n");
            writer.Write("</table>\n");
            writer.Write("<input type=\"button\" value=\"Add New Variant\" name=\"AddNew\" onClick=\"self.location='editVariant.aspx?productid=" + ProductID.ToString() + "';\">\n");



            writer.Write("</form>\n");

            writer.Write("<script type=\"text/javascript\">\n");
            writer.Write("function DeleteVariant(id)\n");
            writer.Write("{\n");
            writer.Write("if(confirm('Are you sure you want to delete Variant: ' + id + '? This will also remove all items from customer shopping carts, wish lists, and gift registries IF they reference this variant!'))\n");
            writer.Write("{\n");
            writer.Write("self.location = 'Variants.aspx?productid=" + ProductID.ToString() + "&deleteid=' + id;\n");
            writer.Write("}\n");
            writer.Write("}\n");
            writer.Write("</SCRIPT>\n");
        }

    }
}
