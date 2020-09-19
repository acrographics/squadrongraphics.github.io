// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editcustomerlevel.aspx.cs 6     9/30/06 3:39p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for editcustomerlevel
    /// </summary>
    public partial class editcustomerlevel : AspDotNetStorefront.SkinBase
    {

        int CustomerLevelID;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            CustomerLevelID = 0;

            if (CommonLogic.QueryStringCanBeDangerousContent("CustomerLevelID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("CustomerLevelID") != "0")
            {
                Editing = true;
                CustomerLevelID = Localization.ParseUSInt(CommonLogic.QueryStringCanBeDangerousContent("CustomerLevelID"));
            }
            else
            {
                Editing = false;
            }


            IDataReader rs;

            if (CommonLogic.FormBool("IsSubmit"))
            {

                if (ErrorMsg.Length == 0)
                {
                    StringBuilder sql = new StringBuilder(2500);
                    if (!Editing)
                    {
                        // ok to add them:
                        String NewGUID = DB.GetNewGUID();
                        sql.Append("insert into CustomerLevel(CustomerLevelGUID,Name,LevelDiscountPercent,LevelDiscountAmount,LevelHasFreeShipping,LevelAllowsQuantityDiscounts,LevelAllowsPO,LevelHasNoTax,LevelAllowsCoupons,LevelDiscountsApplyToExtendedPrices) values(");
                        sql.Append(DB.SQuote(NewGUID) + ",");
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                        sql.Append(Localization.DecimalStringForDB(CommonLogic.FormUSDecimal("LevelDiscountPercent")) + ",");
                        sql.Append(Localization.CurrencyStringForDBWithoutExchangeRate(CommonLogic.FormUSDecimal("LevelDiscountAmount")) + ",");
                        sql.Append(CommonLogic.FormCanBeDangerousContent("LevelHasFreeShipping") + ",");
                        sql.Append(CommonLogic.FormCanBeDangerousContent("LevelAllowsQuantityDiscounts") + ",");
                        sql.Append(CommonLogic.FormCanBeDangerousContent("LevelAllowsPO") + ",");
                        sql.Append(CommonLogic.FormCanBeDangerousContent("LevelHasNoTax") + ",");
                        sql.Append(CommonLogic.FormCanBeDangerousContent("LevelAllowsCoupons") + ",");
                        sql.Append(CommonLogic.FormCanBeDangerousContent("LevelDiscountsApplyToExtendedPrices"));
                        sql.Append(")");
                        DB.ExecuteSQL(sql.ToString());

                        rs = DB.GetRS("select CustomerLevelID from CustomerLevel  " + DB.GetNoLock() + " where deleted=0 and CustomerLevelGUID=" + DB.SQuote(NewGUID));
                        rs.Read();
                        CustomerLevelID = DB.RSFieldInt(rs, "CustomerLevelID");
                        Editing = true;
                        rs.Close();
                        DataUpdated = true;
                    }
                    else
                    {
                        // ok to update:
                        sql.Append("update CustomerLevel set ");
                        sql.Append("Name=" + DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                        sql.Append("LevelDiscountPercent=" + CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("LevelDiscountPercent").Trim() == "", "0", CommonLogic.FormCanBeDangerousContent("LevelDiscountPercent")) + ",");
                        sql.Append("LevelDiscountAmount=" + CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("LevelDiscountAmount").Trim() == "", "0", CommonLogic.FormCanBeDangerousContent("LevelDiscountAmount")) + ",");
                        sql.Append("LevelHasFreeShipping=" + CommonLogic.FormCanBeDangerousContent("LevelHasFreeShipping") + ",");
                        sql.Append("LevelAllowsQuantityDiscounts=" + CommonLogic.FormCanBeDangerousContent("LevelAllowsQuantityDiscounts") + ",");
                        sql.Append("LevelAllowsPO=" + CommonLogic.FormCanBeDangerousContent("LevelAllowsPO") + ",");
                        sql.Append("LevelHasNoTax=" + CommonLogic.FormCanBeDangerousContent("LevelHasNoTax") + ",");
                        sql.Append("LevelAllowsCoupons=" + CommonLogic.FormCanBeDangerousContent("LevelAllowsCoupons") + ",");
                        sql.Append("LevelDiscountsApplyToExtendedPrices=" + CommonLogic.FormCanBeDangerousContent("LevelDiscountsApplyToExtendedPrices"));
                        sql.Append(" where CustomerLevelID=" + CustomerLevelID.ToString());
                        DB.ExecuteSQL(sql.ToString());
                        DataUpdated = true;
                        Editing = true;
                    }
                }

            }
            SectionTitle = "<a href=\"CustomerLevels.aspx\">CustomerLevels</a> - Manage Customer Levels";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write(CommonLogic.ReadFile("jscripts/tabs.js", true));
            IDataReader rs = DB.GetRS("select * from CustomerLevel  " + DB.GetNoLock() + " where CustomerLevelID=" + CustomerLevelID.ToString());
            if (rs.Read())
            {
                Editing = true;
            }
            if (DataUpdated)
            {
                writer.Write("<p align=\"left\"><b><font color=blue>(UPDATED)</font></b></p>\n");
            }


            if (ErrorMsg.Length != 0)
            {
                writer.Write("<p align=\"left\"><b><font color=red>" + ErrorMsg + "</font></b></p>\n");
            }


            if (ErrorMsg.Length == 0)
            {

                if (Editing)
                {
                    writer.Write("<p align=\"left\"><b>Editing CustomerLevel: " + DB.RSFieldByLocale(rs, "Name", ThisCustomer.LocaleSetting) + " (ID=" + DB.RSFieldInt(rs, "CustomerLevelID").ToString() + ")</p></b>\n");
                }
                else
                {
                    writer.Write("<p align=\"left\"><b>Adding New Customer Level:</p></b>\n");
                }

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function Form_Validator(theForm)\n");
                writer.Write("{\n");
                writer.Write("submitonce(theForm);\n");
                writer.Write("return (true);\n");
                writer.Write("}\n");
                writer.Write("</script>\n");

                writer.Write("<p align=\"left\">Please enter the following information about this Customer Level. Fields marked with an asterisk (*) are required. All other fields are optional.</p>\n");
                writer.Write("<p align=\"left\"><b><font color=blue>WARNING: You can easily define a pricing/level/extended pricing/quantity pricing/coupon scheme that NO ONE could possibly figure out how the actual pricing was computed. Our suggestion is to KEEP IT SIMPLE. If you use Discount Percents for levels, DON'T USE extended pricing, and TURN OFF coupons for the level, etc... If you want to define extended pricing for each product variant, then set ALL level discounts to 0, disallow coupons, etc... NetStore will ALWAYS compute A price for each product and the entire order, but YOU may not be able to explain how the price was arrived at to your customers. KEEP IT SIMPLE!</font></b></p>\n");
                writer.Write("<form enctype=\"multipart/form-data\" action=\"editCustomerLevel.aspx?CustomerLevelID=" + CustomerLevelID.ToString() + "&edit=" + Editing.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(this) && Form_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"100%\" colspan=\"3\" align=\"left\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Name:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Name"), "Name", false, true, true, "Please enter the customer level name", 100, 30, 0, 0, false));
                //writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"Name\" value=\"" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Name")) , "") + "\">\n");
                //writer.Write("                	<input type=\"hidden\" name=\"Name_vldt\" value=\"[req][blankalert=Please enter a description of this Customer Level, e.g. Reseller]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("<small>Enter the name for this level, e.g. Reseller, Wholesale, Platinum, Gold, Preferred, etc...best to KEEP IT SHORT!.</small>");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"top\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"top\">*Level Discount Percent:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"LevelDiscountPercent\" value=\"" + CommonLogic.IIF(Editing, Localization.CurrencyStringForDBWithoutExchangeRate(DB.RSFieldDecimal(rs, "LevelDiscountPercent")), "") + "\">\n");
                //writer.Write("                	<input type=\"hidden\" name=\"LevelDiscountPercent_vldt\" value=\"[req][blankalert=Please enter the Customer Level discount percentage, or 0]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("<small>(Enter 0, or a percentage like 5 or 7.5)</small> <small>if non-zero, this each product price will be discounted by this amount for customers of this level. If Level Discounts also apply to extended prices is also checked, this discount percentage will ALSO be applied to any level specific extended prices. Generally, if you are going to the trouble of setting up extended pricing schemes, this percentage should be 0, and the extended prices should be the actual level price. If you don't want go to all the work of setting up extended prices for each product, this percentage discount is the best way to go to offer reseller/wholsale pricing to certain customers.</small>");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"top\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"top\">*Level Discount Amount:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"LevelDiscountAmount\" value=\"" + CommonLogic.IIF(Editing, Localization.CurrencyStringForDBWithoutExchangeRate(DB.RSFieldDecimal(rs, "LevelDiscountAmount")), "") + "\">\n");
                //writer.Write("                	<input type=\"hidden\" name=\"LevelDiscountAmount_vldt\" value=\"[req][blankalert=Please enter the Customer Level discount amount in your currency (e.g. dollars), or 0]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("<small>(Enter 0, or a dollar amount like 2.50 or 10.00)</small> <small>if non-zero, this amount will be subtracted from the entire order total, after all other criteria/discounts etc are applied. It will be unusual to apply this kind of level discount, but... you can.</small>");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"top\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">*Level Includes Free Shipping:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LevelHasFreeShipping\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "LevelHasFreeShipping"), " checked ", ""), "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LevelHasFreeShipping\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "LevelHasFreeShipping"), "", " checked "), " checked ") + ">\n");
                writer.Write("                	</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("<small>if yes, then all customer orders for this level will have $0 shipping charges on their order.</small>");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"top\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">*Level Allows Quantity Discounts:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LevelAllowsQuantityDiscounts\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "LevelAllowsQuantityDiscounts"), " checked ", ""), "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LevelAllowsQuantityDiscounts\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "LevelAllowsQuantityDiscounts"), "", " checked "), " checked ") + ">\n");
                writer.Write("                	</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("<small>if yes, then all quantity discount tables (specified elsewhere) are applied on a product by product basis, based on quantity. Discounts are applied after extended price, or level discounted price is applied.</small>");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"top\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">*Level Allows Purchase Orders:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LevelAllowsPO\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "LevelAllowsPO"), " checked ", ""), "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LevelAllowsPO\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "LevelAllowsPO"), "", " checked "), " checked ") + ">\n");
                writer.Write("                	</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"top\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">*Level Has No Tax On Orders:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LevelHasNoTax\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "LevelHasNoTax"), " checked ", ""), "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LevelHasNoTax\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "LevelHasNoTax"), "", " checked "), " checked ") + ">\n");
                writer.Write("                	</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("<small>if yes, then all customer orders for this level will have $0 tax charged, regardless of their address.</small>");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"top\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">*Level Allows Coupons On Orders:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LevelAllowsCoupons\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "LevelAllowsCoupons"), " checked ", ""), "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LevelAllowsCoupons\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "LevelAllowsCoupons"), "", " checked "), " checked ") + ">\n");
                writer.Write("                	</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("<small>if yes, then if the customer enters a valid coupon, that coupon discount specificiations will be applied to the ENTIRE ORDER, ON TOP of any level extended prices and level discounts. If no, then these customers are not allowed to enter coupon codes.</small>");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"top\">\n");
                writer.Write("                <td align=\"right\" valign=\"top\">*Level Discounts Also Apply To Extended Prices:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LevelDiscountsApplyToExtendedPrices\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "LevelDiscountsApplyToExtendedPrices"), " checked ", ""), "") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"LevelDiscountsApplyToExtendedPrices\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "LevelDiscountsApplyToExtendedPrices"), "", " checked "), " checked ") + ">\n");
                writer.Write("                	</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("<small>if yes, then level discount amount and percents will be applied ON TOP of level extended prices. If no, then if an extended price exists for a product, that price is used as is.</small>");
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
                writer.Write("  </table>\n");
                writer.Write("</form>\n");

            }
            rs.Close();


        }

    }
}
