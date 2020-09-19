// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editquantitydiscounttable.aspx.cs 7     9/30/06 3:37p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for editquantitydiscounttable.
    /// </summary>
    public partial class editquantitydiscounttable : AspDotNetStorefront.SkinBase
    {

        int QuantityDiscountID;
        String QuantityDiscountName;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache"); 
            

            QuantityDiscountID = CommonLogic.QueryStringUSInt("QuantityDiscountID");
            QuantityDiscountName = AppLogic.GetQuantityDiscountName(QuantityDiscountID, ThisCustomer.LocaleSetting);
            if (CommonLogic.FormBool("IsSubmitByCount"))
            {
                // check for new row addition:
                int Low0 = CommonLogic.FormUSInt("Low_0");
                int High0 = CommonLogic.FormUSInt("High_0");
                String NewGUID = DB.GetNewGUID();
                int NewRowID = 0;

                if (Low0 != 0 || High0 != 0)
                {
                    // add the new row if necessary:
                    Decimal Discount = CommonLogic.FormUSDecimal("Rate_0_" + QuantityDiscountID.ToString());
                    DB.ExecuteSQL("insert into QuantityDiscountTable(QuantityDiscountTableGUID,QuantityDiscountID,LowQuantity,HighQuantity,DiscountPercent) values(" + DB.SQuote(NewGUID) + "," + QuantityDiscountID.ToString() + "," + Localization.IntStringForDB(Low0) + "," + Localization.IntStringForDB(High0) + "," + Localization.DecimalStringForDB(Discount) + ")");
                }
                IDataReader rs = DB.GetRS("Select QuantityDiscountTableID from QuantityDiscountTable  " + DB.GetNoLock() + " where QuantityDiscountTableGUID=" + DB.SQuote(NewGUID));
                rs.Read();
                NewRowID = DB.RSFieldInt(rs, "QuantityDiscountTableID");
                rs.Close();

                // update existing rows:
                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    String FieldName = Request.Form.Keys[i];
                    if (FieldName.IndexOf("_0_") == -1 && FieldName != "Low_0" && FieldName != "High_0" && FieldName.IndexOf("_vldt") == -1 && (FieldName.IndexOf("Rate_") != -1 || FieldName.IndexOf("Low_") != -1 || FieldName.IndexOf("High_") != -1))
                    {
                        Decimal FieldVal = CommonLogic.FormUSDecimal(FieldName);
                        // this field should be processed
                        String[] Parsed = FieldName.Split('_');
                        if (FieldName.IndexOf("Rate_") != -1)
                        {
                            // update discount:
                            DB.ExecuteSQL("Update QuantityDiscountTable set DiscountPercent=" + Localization.DecimalStringForDB(FieldVal) + " where QuantityDiscountTableID=" + Parsed[1]);
                        }
                        if (FieldName.IndexOf("Low_") != -1)
                        {
                            // update low value:
                            DB.ExecuteSQL("Update QuantityDiscountTable set LowQuantity=" + FieldVal.ToString() + " where QuantityDiscountTableID=" + DB.SQuote(Parsed[1]));
                        }
                        if (FieldName.IndexOf("High_") != -1)
                        {
                            // update high value:
                            DB.ExecuteSQL("Update QuantityDiscountTable set HighQuantity=" + FieldVal.ToString() + " where QuantityDiscountTableID=" + DB.SQuote(Parsed[1]));
                        }
                    }
                }
                DB.ExecuteSQL("Update QuantityDiscountTable set HighQuantity=999999 where HighQuantity=0 and LowQuantity<>0");
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("deleteByCountid").Length != 0)
            {
                DB.ExecuteSQL("delete from QuantityDiscountTable where QuantityDiscountTableID=" + CommonLogic.QueryStringCanBeDangerousContent("deleteByCountid"));
            }
            SectionTitle = "<a href=\"quantitydiscounts.aspx\">Quantity Discounts</a> - Manage Quantity Discounts Table: " + QuantityDiscountName;
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write("<script type=\"text/javascript\">\n");
            writer.Write("function ByCountForm_Validator(theForm)\n");
            writer.Write("{\n");
            writer.Write("submitonce(theForm);\n");
            writer.Write("return (true);\n");
            writer.Write("}\n");
            writer.Write("</script>\n");

            writer.Write("<p align=\"left\"><b>DISCOUNT QUANTITY TABLE: " + QuantityDiscountName.ToUpperInvariant() + "</b></p>\n");

            writer.Write("<form action=\"editquantitydiscounttable.aspx?quantitydiscountid=" + QuantityDiscountID.ToString() + "\" method=\"post\" id=\"ByCountForm\" name=\"ByCountForm\" onsubmit=\"return (validateForm(this) && ByCountForm_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
            writer.Write("<input type=\"hidden\" name=\"IsSubmitByCount\" value=\"true\">\n");

            writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"1\">\n");
            writer.Write("<tr bgcolor=\"#FFFFDD\"><td colspan=3 align=\"center\"><b>Order Quantity</b></td><td align=\"center\"><b>Percent Discount</b></td></tr>\n");
            writer.Write("<tr>\n");
            writer.Write("<td align=\"center\" bgcolor=\"#CCFFFF\"><b>Delete</b></td>\n");
            writer.Write("<td align=\"center\" bgcolor=\"#CCFFFF\"><b>Low</b></td>\n");
            writer.Write("<td align=\"center\" bgcolor=\"#CCFFFF\"><b>High</b></td>\n");
            writer.Write("<td align=\"center\" bgcolor=\"#CCFFFF\"><b>Discount Percentage</b></td>\n");
            writer.Write("</tr>\n");

            DataSet QuantityDiscountRows = DB.GetDS("select * from QuantityDiscountTable  " + DB.GetNoLock() + " where QuantityDiscountID=" + QuantityDiscountID.ToString() + " order by LowQuantity", false);
            foreach (DataRow discountrow in QuantityDiscountRows.Tables[0].Rows)
            {
                writer.Write("<tr>\n");
                writer.Write("<td align=\"center\" bgcolor=\"#CCFFFF\"><input type=\"Button\" name=\"Delete\" value=\"X\" onClick=\"self.location='editquantitydiscounttable.aspx?quantitydiscountid=" + QuantityDiscountID.ToString() + "&deleteByCountid=" + DB.RowFieldInt(discountrow, "QuantityDiscountTableID").ToString() + "'\"></td>\n");
                writer.Write("<td align=\"center\" bgcolor=\"#CCFFFF\">\n");
                writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Low_" + DB.RowFieldInt(discountrow, "QuantityDiscountTableID").ToString() + "\" value=\"" + DB.RowFieldInt(discountrow, "LowQuantity").ToString() + "\">\n");
                writer.Write("<input type=\"hidden\" name=\"Low_" + DB.RowFieldInt(discountrow, "QuantityDiscountTableID").ToString() + "_vldt\" value=\"[number][blankalert=Please enter starting order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                writer.Write("</td>\n");
                writer.Write("<td align=\"center\" bgcolor=\"#CCFFFF\">\n");
                writer.Write("<input maxLength=\"10\" size=\"10\" name=\"High_" + DB.RowFieldInt(discountrow, "QuantityDiscountTableID").ToString() + "\" value=\"" + DB.RowFieldInt(discountrow, "HighQuantity").ToString() + "\">\n");
                writer.Write("<input type=\"hidden\" name=\"High_" + DB.RowFieldInt(discountrow, "QuantityDiscountTableID").ToString() + "_vldt\" value=\"[number][blankalert=Please enter ending order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                writer.Write("</td>\n");
                writer.Write("<td align=\"center\" bgcolor=\"#CCFFFF\">\n");
                writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Rate_" + DB.RowFieldInt(discountrow, "QuantityDiscountTableID").ToString() + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(discountrow, "DiscountPercent")) + "\">\n");
                writer.Write("<input type=\"hidden\" name=\"Rate_" + DB.RowFieldInt(discountrow, "QuantityDiscountTableID").ToString() + "_vldt\" value=\"[number][blankalert=Please enter the discount percent][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                writer.Write("</td>\n");
                writer.Write("</tr>\n");
            }
            // add new row:
            writer.Write("<tr>\n");
            writer.Write("<td align=\"center\" bgcolor=\"#CCFFFF\">Add New Row Here:</td>\n");
            writer.Write("<td align=\"center\" bgcolor=\"#CCFFFF\">\n");
            writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Low_0\" value=\"\">\n");
            writer.Write("<input type=\"hidden\" name=\"Low_0_vldt\" value=\"[int][blankalert=Please enter starting order quantity][invalidalert=Please enter an integer]\">\n");
            writer.Write("</td>\n");
            writer.Write("<td align=\"center\" bgcolor=\"#CCFFFF\">\n");
            writer.Write("<input maxLength=\"10\" size=\"10\" name=\"High_0\" value=\"\">\n");
            writer.Write("<input type=\"hidden\" name=\"High_0_vldt\" value=\"[int][blankalert=Please enter ending order quantity][invalidalert=Please enter an integer]\">\n");
            writer.Write("</td>\n");
            writer.Write("<td align=\"center\" bgcolor=\"#CCFFFF\">\n");
            writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Rate_0_" + QuantityDiscountID.ToString() + "\" value=\"\">\n");
            writer.Write("<input type=\"hidden\" name=\"Rate_0_" + QuantityDiscountID.ToString() + "_vldt\" value=\"[number][blankalert=Please enter the desired percent discount][invalidalert=Please enter a percentage value, WITHOUT the percent sign]\">\n");
            writer.Write("</td>\n");
            writer.Write("</tr>\n");
            writer.Write("</table>\n");
            writer.Write("<p align=\"left\"><input type=\"submit\" value=\"Update\" name=\"submit\"></p>\n");

            writer.Write("</form>\n");
            QuantityDiscountRows.Dispose();
        }
    }
}
