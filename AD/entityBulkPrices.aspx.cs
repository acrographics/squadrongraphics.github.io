// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/entityBulkPrices.aspx.cs 5     10/04/06 6:23a Redwoodtree $
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
    public partial class entityBulkPrices : System.Web.UI.Page
    {
        int EntityID;
        String EntityName;
        EntitySpecs m_EntitySpecs;
        EntityHelper Helper;
        private Customer cust;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            EntityID = CommonLogic.QueryStringUSInt("EntityID"); ;
            EntityName = CommonLogic.QueryStringCanBeDangerousContent("EntityName");
            m_EntitySpecs = EntityDefinitions.LookupSpecs(EntityName);
            Helper = new EntityHelper(m_EntitySpecs);
            //Helper = AppLogic.LookupHelper(base.EntityHelpers, m_EntitySpecs.m_EntityName);

            if (EntityID == 0 || EntityName.Length == 0)
            {
                ltBody.Text = "Invalid Parameters";
                return;
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

            LoadBody();
        }

        protected void LoadBody()
        {
            StringBuilder tmpS = new StringBuilder(4096);
            tmpS.Append("<div style=\"width: 100%; border-top: solid 1px #d2d2d2; padding-top: 3px; margin-top: 5px;\">");

            ProductCollection products = new ProductCollection(m_EntitySpecs.m_EntityName, EntityID);
            products.PageSize = 0;
            products.PageNum = 1;
            products.PublishedOnly = false;
            products.ReturnAllVariants = true;
            DataSet dsProducts = products.LoadFromDB();
            int NumProducts = products.NumProducts;
            if (NumProducts > 0)
            {
                tmpS.Append("<script type=\"text/javascript\">\n");
                tmpS.Append("function Form_Validator(theForm)\n");
                tmpS.Append("{\n");
                tmpS.Append("submitonce(theForm);\n");
                tmpS.Append("return (true);\n");
                tmpS.Append("}\n");
                tmpS.Append("</script>\n");

                tmpS.Append("<form id=\"Form1\" name=\"Form1\" method=\"POST\" action=\"entityBulkPrices.aspx?entityid=" + EntityID.ToString() + "&entityname=" + m_EntitySpecs.m_EntityName + "\" onsubmit=\"alert('Please Be Patient, this will take a minute to validate all prices!');return (validateForm(document.forms[0]) && Form_Validator(document.forms[0]))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                tmpS.Append("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                tmpS.Append("<table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
                tmpS.Append("<tr><td colspan=\"8\" align=\"right\"><input type=\"submit\" value=\"Prices Update\" name=\"Submit\" class=\"normalButton\"></td></tr>\n");
                tmpS.Append("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                tmpS.Append("<td><b>ProductID</b></td>\n");
                tmpS.Append("<td><b>VariantID</b></td>\n");
                tmpS.Append("<td><b>Product Name</b></td>\n");
                tmpS.Append("<td><b>Variant Name</b></td>\n");
                tmpS.Append("<td align=\"center\"><b>Price</b><br/><small>(in format x.xx)</small></td>\n");
                tmpS.Append("<td align=\"center\"><b>Sale Price</b><br/><small>(in format x.xx)</small></td>\n");
                tmpS.Append("<td align=\"center\"><b>MSRP</b><br/><small>(in format x.xx)</small></td>\n");
                tmpS.Append("<td align=\"center\"><b>Cost</b><br/><small>(in format x.xx)</small></td>\n");
                tmpS.Append("</tr>\n");
                int LastProductID = 0;
                foreach (DataRow row in dsProducts.Tables[0].Rows)
                {
                    int ThisProductID = DB.RowFieldInt(row, "ProductID");
                    int ThisVariantID = DB.RowFieldInt(row, "VariantID");
                    tmpS.Append("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                    tmpS.Append("<td align=\"left\" valign=\"middle\">");
                    tmpS.Append(ThisProductID.ToString());
                    tmpS.Append("</td>");
                    tmpS.Append("<td align=\"left\" valign=\"middle\">");
                    tmpS.Append(ThisVariantID.ToString());
                    tmpS.Append("</td>");
                    tmpS.Append("<td align=\"left\" valign=\"middle\">");
                    tmpS.Append("<a target=\"entityBody\" href=\"editproduct.aspx?productid=" + ThisProductID.ToString() + "\">");
                    tmpS.Append(DB.RowFieldByLocale(row, "Name", cust.LocaleSetting));
                    tmpS.Append("</a>");
                    tmpS.Append("</td>\n");
                    tmpS.Append("<td align=\"left\" valign=\"middle\">");
                    tmpS.Append("<a target=\"entityBody\" href=\"editvariant.aspx?productid=" + ThisProductID.ToString() + "&variantid=" + ThisVariantID.ToString() + "\">");
                    tmpS.Append(DB.RowFieldByLocale(row, "VariantName", cust.LocaleSetting));
                    tmpS.Append("</a>");
                    tmpS.Append("</td>\n");
                    tmpS.Append("<td align=\"center\" valign=\"middle\">");
                    tmpS.Append("<input maxLength=\"10\" size=\"10\" class=\"default\" name=\"Price_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(row, "Price")) + "\">");
                    tmpS.Append("<input type=\"hidden\" name=\"Price_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "_vldt\" value=\"[req][number][blankalert=Please enter the variant price][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                    tmpS.Append("</td>");
                    tmpS.Append("<td align=\"center\" valign=\"middle\">");
                    tmpS.Append("<input maxLength=\"10\" size=\"10\" class=\"default\" name=\"SalePrice_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" value=\"" + CommonLogic.IIF(DB.RowFieldDecimal(row, "SalePrice") != System.Decimal.Zero, Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(row, "SalePrice")), "") + "\">\n");
                    tmpS.Append("<input type=\"hidden\" name=\"SalePrice_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "_vldt\" value=\"[number][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                    tmpS.Append("</td>");
                    tmpS.Append("<td align=\"center\" valign=\"middle\">");
                    tmpS.Append("<input maxLength=\"10\" size=\"10\" class=\"default\" name=\"MSRP_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" value=\"" + CommonLogic.IIF(DB.RowFieldDecimal(row, "MSRP") != System.Decimal.Zero, Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(row, "MSRP")), "") + "\">\n");
                    tmpS.Append("<input type=\"hidden\" name=\"MSRP_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "_vldt\" value=\"[number][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                    tmpS.Append("</td>");
                    tmpS.Append("<td align=\"center\" valign=\"middle\">");
                    tmpS.Append("<input maxLength=\"10\" size=\"10\" class=\"default\" name=\"Cost_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" value=\"" + CommonLogic.IIF(DB.RowFieldDecimal(row, "Cost") != System.Decimal.Zero, Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(row, "Cost")), "") + "\">\n");
                    tmpS.Append("<input type=\"hidden\" name=\"Cost_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "_vldt\" value=\"[number][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                    tmpS.Append("</td>\n");

                    tmpS.Append("</tr>\n");
                    LastProductID = ThisProductID;
                }
                tmpS.Append("<tr><td colspan=\"8\" align=\"right\"><input type=\"submit\" value=\"Prices Update\" name=\"Submit\" class=\"normalButton\"></td></tr>\n");
                tmpS.Append("</table>\n");
                tmpS.Append("</form>\n");
            }
            else
            {
                tmpS.Append("<p><b>No Products Found</b></p>");
            }
            dsProducts.Dispose();
            products.Dispose();

            tmpS.Append("</div>");
            ltBody.Text = tmpS.ToString();
        }
    }
}
