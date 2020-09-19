// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editinventory.aspx.cs 7     9/30/06 3:39p Redwoodtree $
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
    /// Summary description for editinventory
    /// </summary>
    public partial class editinventory : AspDotNetStorefront.SkinBase
    {

        int ProductID;
        int VariantID;
        bool ProductTracksInventoryBySize;
        bool ProductTracksInventoryByColor;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            ProductID = CommonLogic.QueryStringUSInt("ProductID");
            VariantID = CommonLogic.QueryStringUSInt("VariantID");
            if (VariantID == 0)
            {
                VariantID = AppLogic.GetDefaultProductVariant(ProductID, false);
            }
            if (VariantID == 0)
            {
                VariantID = AppLogic.GetProductsFirstVariantID(ProductID);
            }
            if (VariantID == 0)
            {
                Response.Redirect("splash.aspx"); // should never get here, but...
            }
            if (ProductID == 0)
            {
                ProductID = AppLogic.GetVariantProductID(ProductID);
            }

            ProductTracksInventoryBySize = AppLogic.ProductTracksInventoryBySize(ProductID);
            ProductTracksInventoryByColor = AppLogic.ProductTracksInventoryByColor(ProductID);

            if (CommonLogic.FormBool("IsSubmit"))
            {
                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    String FieldName = Request.Form.Keys[i];
                    if (FieldName.IndexOf("|") != -1 && ((FieldName.StartsWith("simple", StringComparison.InvariantCultureIgnoreCase) || FieldName.StartsWith("sizecolor", StringComparison.InvariantCultureIgnoreCase))))
                    {
                        String KeyVal = CommonLogic.FormCanBeDangerousContent(FieldName);
                        // this field should be processed
                        String[] FieldNameSplit = FieldName.Split('|');
                        String InventoryType = FieldNameSplit[0].ToLower(CultureInfo.InvariantCulture);
                        int TheProductID = Localization.ParseUSInt(FieldNameSplit[1]);
                        int TheVariantID = Localization.ParseUSInt(FieldNameSplit[2]);
                        String Size = FieldNameSplit[3];
                        String Color = FieldNameSplit[4];
                        int inputVal = CommonLogic.FormUSInt(FieldName);
                        if (InventoryType == "simple")
                        {
                            DB.ExecuteSQL("update ProductVariant set Inventory=" + inputVal.ToString() + " where VariantID=" + TheVariantID.ToString());
                        }
                        else
                        {
                            String sql = "select count(*) as N from Inventory " + DB.GetNoLock() + " where VariantID=" + TheVariantID.ToString() + " and lower([size])=" + DB.SQuote(AppLogic.CleanSizeColorOption(Size).ToLowerInvariant()) + " and lower(color)=" + DB.SQuote(AppLogic.CleanSizeColorOption(Color).ToLowerInvariant());
                            if (DB.GetSqlN(sql) == 0)
                            {
                                sql = "insert into Inventory(InventoryGUID,VariantID,[Size],Color,Quan) values(" + DB.SQuote(DB.GetNewGUID()) + "," + TheVariantID.ToString() + "," + DB.SQuote(AppLogic.CleanSizeColorOption(Size)) + "," + DB.SQuote(AppLogic.CleanSizeColorOption(Color)) + "," + inputVal.ToString() + ")";
                                DB.ExecuteSQL(sql);
                            }
                            else
                            {
                                sql = "update Inventory set Quan=" + inputVal.ToString() + " where VariantID=" + TheVariantID.ToString() + " and lower([size])=" + DB.SQuote(AppLogic.CleanSizeColorOption(Size).ToLowerInvariant()) + " and lower(color)=" + DB.SQuote(AppLogic.CleanSizeColorOption(Color).ToLowerInvariant());
                                DB.ExecuteSQL(sql);
                            }
                        }
                    }
                    DataUpdated = true;
                }
            }
            SectionTitle = "<a href=\"editvariant.aspx?productid=" + ProductID.ToString() + "&variantid=" + VariantID.ToString() + "\">Back To Variant</a> - Manage Inventory" + CommonLogic.IIF(DataUpdated, " (Updated)", "");
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            IDataReader rs = DB.GetRS("select * from productvariant  " + DB.GetNoLock() + " where VariantID=" + VariantID.ToString());
            if (!rs.Read())
            {
                rs.Close();
                Response.Redirect("splash.aspx"); // should not happen, but...
            }

            if (ErrorMsg.Length != 0)
            {
                writer.Write("<p align=\"left\"><b><font color=red>" + ErrorMsg + "</font></b></p>\n");
            }

            if (ErrorMsg.Length == 0)
            {

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function InventoryForm_Validator(theForm)\n");
                writer.Write("{\n");
                writer.Write("submitonce(theForm);\n");
                writer.Write("return (true);\n");
                writer.Write("}\n");
                writer.Write("</script>\n");

                String ProductName = AppLogic.GetProductName(ProductID, ThisCustomer.LocaleSetting);
                String ProductSKU = AppLogic.GetProductSKU(ProductID);
                String VariantName = AppLogic.GetVariantName(VariantID, ThisCustomer.LocaleSetting);
                String VariantSKU = AppLogic.GetVariantSKUSuffix(VariantID);

                String Sizes = DB.RSFieldByLocale(rs, "Sizes", Localization.GetWebConfigLocale()).Trim();
                String Colors = DB.RSFieldByLocale(rs, "Colors", Localization.GetWebConfigLocale()).Trim();

                if (!ProductTracksInventoryBySize)
                {
                    Sizes = String.Empty;
                }
                if (!ProductTracksInventoryByColor)
                {
                    Colors = String.Empty;
                }

                String[] ColorsSplit = Colors.Split(',');
                String[] SizesSplit = Sizes.Split(',');

                writer.Write("<p align=\"left\"><b>PRODUCT: <a href=\"editproduct.aspx?productid=" + ProductID.ToString() + "\">" + ProductName + " (ProductID=" + ProductID.ToString() + ")</a><br/>VARIANT: <a href=\"editvariant.aspx?productid=" + ProductID.ToString() + "&variantID=" + VariantID.ToString() + "\">" + VariantID.ToString() + " (VariantID=" + VariantID.ToString() + ")</a></b></p>");
                writer.Write("<p align=\"left\">Please enter the following inventory data for this product variant.</p>\n");

                writer.Write("<div align=\"left\">");
                writer.Write("<form action=\"editinventory.aspx?productid=" + ProductID.ToString() + "&VariantID=" + VariantID.ToString() + "\" method=\"post\" id=\"InventoryForm\" name=\"InventoryForm\" onsubmit=\"return (validateForm(this) && InventoryForm_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");

                writer.Write("<table border=\"0\" cellspacing=\"0\">\n");
                writer.Write("<tr>\n");
                writer.Write("<td valign=\"middle\" align=\"right\"><b>Inventory</b></td>\n");
                for (int i = SizesSplit.GetLowerBound(0); i <= SizesSplit.GetUpperBound(0); i++)
                {
                    writer.Write("<td valign=\"middle\" align=\"center\"><b>" + AppLogic.CleanSizeColorOption(SizesSplit[i]) + "</b></td>\n");
                }
                writer.Write("</tr>\n");
                int FormFieldID = 1000; // arbitrary number
                for (int i = ColorsSplit.GetLowerBound(0); i <= ColorsSplit.GetUpperBound(0); i++)
                {
                    writer.Write("<tr>\n");
                    writer.Write("<td valign=\"middle\" align=\"right\"><b>" + AppLogic.CleanSizeColorOption(ColorsSplit[i]) + "</b></td>\n");
                    for (int j = SizesSplit.GetLowerBound(0); j <= SizesSplit.GetUpperBound(0); j++)
                    {
                        writer.Write("<td valign=\"middle\" align=\"center\">");
                        writer.Write("<input type=\"text\" name=\"Field_" + FormFieldID.ToString() + "\" size=\"8\" value=\"" + AppLogic.GetInventory(ProductID, VariantID, AppLogic.CleanSizeColorOption(SizesSplit[j]), AppLogic.CleanSizeColorOption(ColorsSplit[i])).ToString() + "\">");
                        writer.Write("<input type=\"hidden\" name=\"Field_" + FormFieldID.ToString() + "_vldt\" value=\"[number][invalidalert=Please enter a number, without any commas, e.g. 100]\">");
                        writer.Write("<input type=\"hidden\" name=\"Key_" + FormFieldID.ToString() + "\" value=\"" + FormFieldID.ToString() + "|" + ProductID.ToString() + "|" + VariantID.ToString() + "|" + Server.HtmlEncode(AppLogic.CleanSizeColorOption(SizesSplit[j])) + "|" + Server.HtmlEncode(AppLogic.CleanSizeColorOption(ColorsSplit[i])) + "\">");
                        FormFieldID++;
                        writer.Write("</td>\n");
                    }
                    writer.Write("</tr>\n");
                }

                writer.Write("</table>\n");
                writer.Write("<p align=\"left\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"submit\" value=\"Update\" name=\"submit\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"reset\" value=\"Reset\" name=\"reset\"></p>\n");
                writer.Write("</form>\n");
                writer.Write("</div>");
            }
            rs.Close();

        }

    }
}
