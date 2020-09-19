// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/bulkeditshippingcosts.aspx.cs 7     10/03/06 7:56p Redwoodtree $
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
    public partial class bulkeditshippingcosts : AspDotNetStorefront.SkinBase
    {
        int EntityID;
        String EntityName;
        EntitySpecs m_EntitySpecs;
        EntityHelper Helper;
        DataSet ds;

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
                String prompt = "Editing Per Item Shipping Costs for " + m_EntitySpecs.m_EntityName + ": <a href=\"editentity.aspx?entityname=" + m_EntitySpecs.m_EntityName + "&entityid=" + EntityID.ToString() + "\">" + Helper.GetEntityName(EntityID, ThisCustomer.LocaleSetting) + "</a>";
                writer.Write("<p><b>" + prompt + "</b></p>");

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function Form_Validator(theForm)\n");
                writer.Write("{\n");
                writer.Write("submitonce(theForm);\n");
                writer.Write("return (true);\n");
                writer.Write("}\n");
                writer.Write("</script>\n");

                writer.Write("<form id=\"Form1\" name=\"Form1\" method=\"POST\" action=\"bulkeditshippingcosts.aspx?entityid=" + EntityID.ToString() + "&entityname=" + m_EntitySpecs.m_EntityName + "\" onsubmit=\"alert('Please Be Patient, this will take a minute to validate all fields!');return (validateForm(document.forms[0]) && Form_Validator(document.forms[0]))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<p align=\"right\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></p>\n");
                writer.Write("<table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
                writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                writer.Write("<td><b>ProductID</b></td>\n");
                writer.Write("<td><b>VariantID</b></td>\n");
                writer.Write("<td><b>Product Name</b></td>\n");
                writer.Write("<td><b>Variant Name</b></td>\n");
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    writer.Write("<td align=\"center\"><b>" + DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting) + " Cost</b><br/><small>(in format x.xx)</small></td>\n");
                }
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
                    foreach (DataRow row2 in ds.Tables[0].Rows)
                    {
                        writer.Write("<td align=\"center\" valign=\"top\">");
                        writer.Write("<input maxLength=\"10\" size=\"10\" name=\"ShippingCost_" + ThisVariantID.ToString() + "_" + DB.RowFieldInt(row2, "ShippingMethodID") + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(Shipping.GetVariantShippingCost(ThisVariantID, DB.RowFieldInt(row2, "ShippingMethodID"))) + "\">\n");
                        writer.Write("<input type=\"hidden\" name=\"ShippingCost_" + ThisVariantID.ToString() + "_" + DB.RowFieldInt(row2, "ShippingMethodID") + "_vldt\" value=\"[number][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                        writer.Write("</td>\n");
                    }
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

            ds.Dispose();
            dsProducts.Dispose();
            products.Dispose();
        }
    }
}
