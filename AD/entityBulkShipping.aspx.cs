// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/entityBulkShipping.aspx.cs 6     10/04/06 6:23a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Text;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for bulkeditshippingcosts.
    /// </summary>
    public partial class entityBulkShipping : System.Web.UI.Page
    {
        int EntityID;
        String EntityName;
        EntitySpecs m_EntitySpecs;
        EntityHelper Helper;
        DataSet ds;
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
            ds = DB.GetDS("select * from ShippingMethod " + DB.GetNoLock() + " where IsRTShipping=0 order by DisplayOrder", false);

            if (CommonLogic.FormCanBeDangerousContent("IsSubmit").ToUpper(CultureInfo.InvariantCulture) == "TRUE")
            {
                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    String FieldName = Request.Form.Keys[i];
                    if (FieldName.StartsWith("shippingcost", StringComparison.InvariantCultureIgnoreCase) && !FieldName.EndsWith("_vldt", StringComparison.InvariantCultureIgnoreCase))
                    {
                        String[] FieldNameSplit = FieldName.Split('_');
                        int ThisVariantID = Localization.ParseUSInt(FieldNameSplit[1]);
                        int ThisShippingMethodID = Localization.ParseUSInt(FieldNameSplit[2]);
                        decimal ShippingCost = CommonLogic.FormUSDecimal(FieldName);
                        DB.ExecuteSQL("delete from ShippingByProduct where VariantID=" + ThisVariantID.ToString() + " and ShippingMethodID=" + ThisShippingMethodID.ToString());
                        DB.ExecuteSQL("insert ShippingByProduct(VariantID,ShippingMethodID,ShippingCost) values(" + ThisVariantID.ToString() + "," + ThisShippingMethodID.ToString() + "," + Localization.CurrencyStringForDBWithoutExchangeRate(ShippingCost) + ")");
                    }
                }
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

                tmpS.Append("<form id=\"Form1\" name=\"Form1\" method=\"POST\" action=\"entityBulkShipping.aspx?entityid=" + EntityID.ToString() + "&entityname=" + m_EntitySpecs.m_EntityName + "\" onsubmit=\"alert('Please Be Patient, this will take a minute to validate all fields!');return (validateForm(document.forms[0]) && Form_Validator(document.forms[0]))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                tmpS.Append("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                //tmpS.Append("<p align=\"right\"><input type=\"submit\" value=\"Update\" name=\"Submit\" class=\"normalButton\"></p>\n");
                tmpS.Append("<table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
                tmpS.Append("    <tr>\n");
                tmpS.Append("      <td colspan=\"8\" align=\"right\"><input type=\"submit\" value=\"Shipping Update\" name=\"Submit\" class=\"normalButton\"></td>\n");
                tmpS.Append("    </tr>\n");
                tmpS.Append("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                tmpS.Append("<td><b>ProductID</b></td>\n");
                tmpS.Append("<td><b>VariantID</b></td>\n");
                tmpS.Append("<td><b>Product Name</b></td>\n");
                tmpS.Append("<td><b>Variant Name</b></td>\n");
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    tmpS.Append("<td align=\"center\"><b>" + DB.RowFieldByLocale(row, "Name", cust.LocaleSetting) + " Cost</b><br/><small>(in format x.xx)</small></td>\n");
                }
                tmpS.Append("</tr>\n");
                int LastProductID = 0;
                foreach (DataRow row in dsProducts.Tables[0].Rows)
                {
                    int ThisProductID = DB.RowFieldInt(row, "ProductID");
                    int ThisVariantID = DB.RowFieldInt(row, "VariantID");
                    tmpS.Append("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                    tmpS.Append("<td align=\"left\" valign=\"top\">");
                    tmpS.Append(ThisProductID.ToString());
                    tmpS.Append("</td>");
                    tmpS.Append("<td align=\"left\" valign=\"top\">");
                    tmpS.Append(ThisVariantID.ToString());
                    tmpS.Append("</td>");
                    tmpS.Append("<td align=\"left\" valign=\"top\">");
                    tmpS.Append("<a target=\"entityBody\" href=\"editproduct.aspx?productid=" + ThisProductID.ToString() + "\">");
                    tmpS.Append(DB.RowFieldByLocale(row, "Name", cust.LocaleSetting));
                    tmpS.Append("</a>");
                    tmpS.Append("</td>\n");
                    tmpS.Append("<td align=\"left\" valign=\"top\">");
                    tmpS.Append("<a target=\"entityBody\" href=\"editvariant.aspx?productid=" + ThisProductID.ToString() + "&variantid=" + ThisVariantID.ToString() + "\">");
                    tmpS.Append(DB.RowFieldByLocale(row, "VariantName", cust.LocaleSetting));
                    tmpS.Append("</a>");
                    tmpS.Append("</td>\n");
                    foreach (DataRow row2 in ds.Tables[0].Rows)
                    {
                        tmpS.Append("<td align=\"center\" valign=\"top\">");
                        tmpS.Append("<input class=\"default\" maxLength=\"10\" size=\"10\" name=\"ShippingCost_" + ThisVariantID.ToString() + "_" + DB.RowFieldInt(row2, "ShippingMethodID") + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(Shipping.GetVariantShippingCost(ThisVariantID, DB.RowFieldInt(row2, "ShippingMethodID"))) + "\">\n");
                        tmpS.Append("<input type=\"hidden\" name=\"ShippingCost_" + ThisVariantID.ToString() + "_" + DB.RowFieldInt(row2, "ShippingMethodID") + "_vldt\" value=\"[number][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                        tmpS.Append("</td>\n");
                    }
                    tmpS.Append("</tr>\n");
                    LastProductID = ThisProductID;
                }
                tmpS.Append("    <tr>\n");
                tmpS.Append("      <td colspan=\"8\" align=\"right\"><input type=\"submit\" value=\"Shipping Update\" name=\"Submit\" class=\"normalButton\"></td>\n");
                tmpS.Append("    </tr>\n");
                tmpS.Append("</table>\n");
                //tmpS.Append("<p align=\"right\"><input type=\"submit\" value=\"Update\" name=\"Submit\" class=\"normalButton\"></p>\n");
                tmpS.Append("</form>\n");
            }
            else
            {
                tmpS.Append("<p><b>No Products Found</b></p>");
            }

            ds.Dispose();
            dsProducts.Dispose();
            products.Dispose();

            tmpS.Append("</div>");
            ltBody.Text = tmpS.ToString();
        }
    }
}
