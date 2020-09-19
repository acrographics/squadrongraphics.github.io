// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/bulkeditprices.aspx.cs 7     9/30/06 3:39p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Text;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for bulkeditprices.
    /// </summary>
    public partial class bulkeditprices : AspDotNetStorefront.SkinBase
    {
        int EntityID;
        String EntityName;
        EntitySpecs m_EntitySpecs;
        EntityHelper Helper;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            EntityID = CommonLogic.QueryStringUSInt("EntityID"); ;
            EntityName = CommonLogic.QueryStringCanBeDangerousContent("EntityName");
            m_EntitySpecs = EntityDefinitions.LookupSpecs(EntityName);
            Helper = AppLogic.LookupHelper(base.EntityHelpers, m_EntitySpecs.m_EntityName);

            if (EntityID == 0 || EntityName.Length == 0)
            {
                Response.Redirect("splash.aspx");
            }

            if (CommonLogic.FormCanBeDangerousContent("IsSubmit").ToUpper(CultureInfo.InvariantCulture) == "TRUE")
            {
                ProductCollection products = new ProductCollection(m_EntitySpecs.m_EntityName, EntityID);
                products.PageSize = 0;
                products.PageNum = 1;
                products.PublishedOnly = false;
                products.ReturnAllVariants = true;
                DataSet dsProducts = products.LoadFromDB();
                int NumProducts = products.NumProducts;
                foreach (DataRow row in dsProducts.Tables[0].Rows)
                {
                    int ThisProductID = DB.RowFieldInt(row, "ProductID");
                    int ThisVariantID = DB.RowFieldInt(row, "VariantID");
                    decimal Price = System.Decimal.Zero;
                    decimal SalePrice = System.Decimal.Zero;
                    decimal MSRP = System.Decimal.Zero;
                    decimal Cost = System.Decimal.Zero;
                    if (CommonLogic.FormCanBeDangerousContent("Price_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString()).Length != 0)
                    {
                        Price = CommonLogic.FormUSDecimal("Price_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString());
                    }
                    if (CommonLogic.FormCanBeDangerousContent("SalePrice_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString()).Length != 0)
                    {
                        SalePrice = CommonLogic.FormUSDecimal("SalePrice_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString());
                    }
                    if (CommonLogic.FormCanBeDangerousContent("MSRP_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString()).Length != 0)
                    {
                        MSRP = CommonLogic.FormUSDecimal("MSRP_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString());
                    }
                    if (CommonLogic.FormCanBeDangerousContent("Cost_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString()).Length != 0)
                    {
                        Cost = CommonLogic.FormUSDecimal("Cost_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString());
                    }
                    StringBuilder sql = new StringBuilder(1024);
                    sql.Append("update productvariant set ");
                    sql.Append("Price=" + Localization.DecimalStringForDB(Price) + ",");
                    sql.Append("SalePrice=" + CommonLogic.IIF(SalePrice != System.Decimal.Zero, Localization.DecimalStringForDB(SalePrice), "NULL") + ",");
                    sql.Append("MSRP=" + CommonLogic.IIF(MSRP != System.Decimal.Zero, Localization.DecimalStringForDB(MSRP), "NULL") + ",");
                    sql.Append("Cost=" + CommonLogic.IIF(Cost != System.Decimal.Zero, Localization.DecimalStringForDB(Cost), "NULL"));
                    sql.Append(" where VariantID=" + ThisVariantID.ToString());
                    DB.ExecuteSQL(sql.ToString());
                }
                dsProducts.Dispose();
            }
            SectionTitle = "<a href=\"entities.aspx?entityname=" + m_EntitySpecs.m_EntityName + "\">" + AppLogic.GetString("AppConfig." + m_EntitySpecs.m_EntityName + "PromptPlural", SkinID, ThisCustomer.LocaleSetting) + "</a> - Edit Inventory For: " + Helper.GetEntityName(EntityID, ThisCustomer.LocaleSetting);
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            ProductCollection products = new ProductCollection(m_EntitySpecs.m_EntityName, EntityID);
            products.PageSize = 0;
            products.PageNum = 1;
            products.PublishedOnly = false;
            products.ReturnAllVariants = true;
            DataSet dsProducts = products.LoadFromDB();
            int NumProducts = products.NumProducts;
            if (NumProducts > 0)
            {
                String prompt = "Editing Pricing for " + m_EntitySpecs.m_EntityName + ": <a href=\"editentity.aspx?entityname=" + m_EntitySpecs.m_EntityName + "&entityid=" + EntityID.ToString() + "\">" + Helper.GetEntityName(EntityID, ThisCustomer.LocaleSetting) + "</a>";
                writer.Write("<p><b>" + prompt + "</b></p>");

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function Form_Validator(theForm)\n");
                writer.Write("{\n");
                writer.Write("submitonce(theForm);\n");
                writer.Write("return (true);\n");
                writer.Write("}\n");
                writer.Write("</script>\n");

                writer.Write("<form id=\"Form1\" name=\"Form1\" method=\"POST\" action=\"bulkeditprices.aspx?entityid=" + EntityID.ToString() + "&entityname=" + m_EntitySpecs.m_EntityName + "\" onsubmit=\"alert('Please Be Patient, this will take a minute to validate all prices!');return (validateForm(document.forms[0]) && Form_Validator(document.forms[0]))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<p align=\"right\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></p>\n");
                writer.Write("<table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
                writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                writer.Write("<td><b>ProductID</b></td>\n");
                writer.Write("<td><b>VariantID</b></td>\n");
                writer.Write("<td><b>Product Name</b></td>\n");
                writer.Write("<td><b>Variant Name</b></td>\n");
                writer.Write("<td align=\"center\"><b>Price</b><br/><small>(in format x.xx)</small></td>\n");
                writer.Write("<td align=\"center\"><b>Sale Price</b><br/><small>(in format x.xx)</small></td>\n");
                writer.Write("<td align=\"center\"><b>MSRP</b><br/><small>(in format x.xx)</small></td>\n");
                writer.Write("<td align=\"center\"><b>Cost</b><br/><small>(in format x.xx)</small></td>\n");
                writer.Write("</tr>\n");
                int LastProductID = 0;
                foreach (DataRow row in dsProducts.Tables[0].Rows)
                {
                    int ThisProductID = DB.RowFieldInt(row, "ProductID");
                    int ThisVariantID = DB.RowFieldInt(row, "VariantID");
                    writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                    writer.Write("<td align=\"left\" valign=\"top\">");
                    writer.Write(ThisProductID.ToString());
                    writer.Write("</td>");
                    writer.Write("<td align=\"left\" valign=\"top\">");
                    writer.Write(ThisVariantID.ToString());
                    writer.Write("</td>");
                    writer.Write("<td align=\"left\" valign=\"top\">");
                    writer.Write("<a href=\"editproduct.aspx?productid=" + ThisProductID.ToString() + "\">");
                    writer.Write(DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting));
                    writer.Write("</a>");
                    writer.Write("</td>\n");
                    writer.Write("<td align=\"left\" valign=\"top\">");
                    writer.Write("<a href=\"editvariant.aspx?productid=" + ThisProductID.ToString() + "&variantid=" + ThisVariantID.ToString() + "\">");
                    writer.Write(DB.RowFieldByLocale(row, "VariantName", ThisCustomer.LocaleSetting));
                    writer.Write("</a>");
                    writer.Write("</td>\n");
                    writer.Write("<td align=\"center\" valign=\"top\">");
                    writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Price_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(row, "Price")) + "\">");
                    writer.Write("<input type=\"hidden\" name=\"Price_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "_vldt\" value=\"[req][number][blankalert=Please enter the variant price][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                    writer.Write("</td>");
                    writer.Write("<td align=\"center\" valign=\"top\">");
                    writer.Write("<input maxLength=\"10\" size=\"10\" name=\"SalePrice_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" value=\"" + CommonLogic.IIF(DB.RowFieldDecimal(row, "SalePrice") != System.Decimal.Zero, Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(row, "SalePrice")), "") + "\">\n");
                    writer.Write("<input type=\"hidden\" name=\"SalePrice_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "_vldt\" value=\"[number][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                    writer.Write("</td>");
                    writer.Write("<td align=\"center\" valign=\"top\">");
                    writer.Write("<input maxLength=\"10\" size=\"10\" name=\"MSRP_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" value=\"" + CommonLogic.IIF(DB.RowFieldDecimal(row, "MSRP") != System.Decimal.Zero, Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(row, "MSRP")), "") + "\">\n");
                    writer.Write("<input type=\"hidden\" name=\"MSRP_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "_vldt\" value=\"[number][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                    writer.Write("</td>");
                    writer.Write("<td align=\"center\" valign=\"top\">");
                    writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Cost_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" value=\"" + CommonLogic.IIF(DB.RowFieldDecimal(row, "Cost") != System.Decimal.Zero, Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(row, "Cost")), "") + "\">\n");
                    writer.Write("<input type=\"hidden\" name=\"Cost_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "_vldt\" value=\"[number][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                    writer.Write("</td>\n");

                    writer.Write("</tr>\n");
                    LastProductID = ThisProductID;
                }
                writer.Write("</table>\n");
                writer.Write("<p align=\"right\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></p>\n");
                writer.Write("</form>\n");
            }
            else
            {
                writer.Write("<p><b>No Products Found</b></p>");
            }
            dsProducts.Dispose();
            products.Dispose();
        }
    }
}
