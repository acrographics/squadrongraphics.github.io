// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2006.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editextendedprices.aspx.cs 6     9/30/06 3:39p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for editextendedprices
    /// </summary>
    public partial class editextendedprices : AspDotNetStorefront.SkinBase
    {

        int VariantID;
        int ProductID;
        String VariantName;
        String VariantSKUSuffix;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            VariantID = CommonLogic.QueryStringUSInt("VariantID");
            ProductID = CommonLogic.QueryStringUSInt("ProductID");
            VariantName = AppLogic.GetVariantName(VariantID, ThisCustomer.LocaleSetting);
            VariantSKUSuffix = AppLogic.GetVariantSKUSuffix(VariantID);
            if (VariantName.Length == 0)
            {
                VariantName = "(Unnamed Variant)";
            }
            if (VariantSKUSuffix.Length == 0)
            {
                VariantSKUSuffix = String.Empty;
            }
            if (ProductID == 0)
            {
                ProductID = AppLogic.GetVariantProductID(VariantID);
            }

            //int N = 0;
            if (CommonLogic.FormBool("IsSubmit"))
            {
                // start with clean slate, to make all adds easy:
                DB.ExecuteSQL("delete from extendedprice where VariantID=" + VariantID.ToString());
                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    String FieldName = Request.Form.Keys[i];
                    if (FieldName.IndexOf("_vldt") == -1 && FieldName.IndexOf("Price_") != -1)
                    {
                        // this field should be processed
                        decimal FieldVal = CommonLogic.FormUSDecimal(FieldName);
                        String[] Parsed = FieldName.Split('_');
                        int CustomerLevelID = Localization.ParseUSInt(Parsed[1]);
                        if (FieldVal != System.Decimal.Zero)
                        {
                            DB.ExecuteSQL("insert into ExtendedPrice(ExtendedPriceGUID,VariantID,CustomerLevelID,Price) values(" + DB.SQuote(DB.GetNewGUID()) + "," + VariantID.ToString() + "," + CustomerLevelID.ToString() + "," + Localization.CurrencyStringForDBWithoutExchangeRate(FieldVal) + ")");
                        }
                    }
                }
            }
            SectionTitle = "<a href=\"variants.aspx?productid=" + ProductID.ToString() + "\">Variants</a> - <a href=\"editvariant.aspx?productid=" + ProductID.ToString() + "&VariantID=" + VariantID.ToString() + "\">Edit Variant</a> - Extended Prices";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            IDataReader rs = DB.GetRS("select cl.*, ep.Price from ProductCustomerLevel pcl join CustomerLevel cl " + DB.GetNoLock() + " on pcl.CustomerLevelID = cl.CustomerLevelID left join ExtendedPrice ep  " + DB.GetNoLock() + " on cl.CustomerLevelID = ep.CustomerLevelID and ep.VariantID = " + VariantID.ToString() + " where pcl.ProductID = " + ProductID.ToString() + " and cl.deleted=0 order by cl.DisplayOrder,cl.Name");

            if (ErrorMsg.Length != 0)
            {
                writer.Write("<p align=\"left\"><b><font color=red>" + ErrorMsg + "</font></b></p>\n");
            }
            if (DataUpdated)
            {
                writer.Write("<p align=\"left\"><b><font color=blue>(UPDATED)</font></b></p>\n");
            }

            writer.Write("<p align=\"left\"><b>Editing Extended Prices For <a href=\"editvariant.aspx?variantID=" + VariantID.ToString() + "\">Variant</a>: " + VariantName + " (Variant SKUSuffix=" + VariantSKUSuffix + ", VariantID=" + VariantID.ToString() + ")</b></p>\n");

            writer.Write("<p align=\"left\"><b>Within Product: <a href=\"editproduct.aspx?productid=" + ProductID.ToString() + "\">" + AppLogic.GetProductName(ProductID, ThisCustomer.LocaleSetting) + "</a> (Product SKU=" + AppLogic.GetProductSKU(ProductID) + ", ProductID=" + ProductID.ToString() + ")</b></p>\n");

            writer.Write("<script type=\"text/javascript\">\n");
            writer.Write("function Form_Validator(theForm)\n");
            writer.Write("{\n");
            writer.Write("submitonce(theForm);\n");
            writer.Write("return (true);\n");
            writer.Write("}\n");
            writer.Write("</script>\n");

            writer.Write("<p align=\"left\"><br/><br/>Specify any extended pricing that you want for this variant. Entering a value of 0 will delete the extended price for that customer level.</p>\n");
            writer.Write("<form action=\"editextendedprices.aspx?productid=" + ProductID.ToString() + "&VariantID=" + VariantID.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(this) && Form_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
            writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
            writer.Write("<table align=\"left\" cellpadding=\"4\" cellspacing=\"0\">\n");

            writer.Write("<tr><td><b>Customer Level</b></td><td><b>Extended Price</b></td></tr>\n");

            while (rs.Read())
            {
                writer.Write("<tr valign=\"middle\">\n");
                writer.Write("<td align=\"left\" valign=\"middle\">Price for Level: <b>" + DB.RSFieldByLocale(rs, "Name", ThisCustomer.LocaleSetting) + "</b>:&nbsp;&nbsp;</td>\n");
                writer.Write("<td align=\"left\" valign=\"top\">\n");
                writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Price_" + DB.RSFieldInt(rs, "CustomerLevelID").ToString() + "\" value=\"" + (Localization.DecimalStringForDB(DB.RSFieldDecimal(rs, "Price"))) + "\">");
                writer.Write("<input type=\"hidden\" name=\"Price_" + DB.RSFieldInt(rs, "CustomerLevelID").ToString() + "_vldt\" value=\"[req][number][blankalert=Please enter an extended price, enter 0 to delete this extended price][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                writer.Write("</td>\n");
                writer.Write("</tr>\n");
            }
            rs.Close();

            writer.Write("<tr>\n");
            writer.Write("<td align=\"left\" colspan=\"2\"><br/>\n");
            writer.Write("<input type=\"submit\" value=\"Update\" name=\"submit\">\n");
            writer.Write("</td>\n");
            writer.Write("</tr>\n");
            writer.Write("</table>\n");
            writer.Write("</form>\n");


        }

    }
}
