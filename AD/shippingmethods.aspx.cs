// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/shippingmethods.aspx.cs 6     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for ShippingMethods.
    /// </summary>
    public partial class ShippingMethods : AspDotNetStorefront.SkinBase
    {

        bool IsShippingMethod = false;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            SectionTitle = "Manage Shipping Methods";
            if (CommonLogic.FormBool("IsSubmit"))
            {
                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    if (Request.Form.Keys[i].IndexOf("DisplayOrder_") != -1)
                    {
                        String[] keys = Request.Form.Keys[i].Split('_');
                        int ShippingMethodID = Localization.ParseUSInt(keys[1]);
                        int DispOrd = 1;
                        try
                        {
                            DispOrd = Localization.ParseUSInt(Request.Form[Request.Form.Keys[i]]);
                        }
                        catch { }
                        DB.ExecuteSQL("update ShippingMethod set DisplayOrder=" + DispOrd.ToString() + " where ShippingMethodID=" + ShippingMethodID.ToString());
                        IsShippingMethod = true;
                    }
                }
            }

        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            if (CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
            {
                // delete the method:
                int DeleteID = CommonLogic.QueryStringUSInt("DeleteID");
                DB.ExecuteSQL("delete from ShippingByTotal where ShippingMethodID=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ShippingByWeight where ShippingMethodID=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ShippingWeightByZone where ShippingMethodID=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ShippingTotalByZone where ShippingMethodID=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ShippingMethod where ShippingMethodID=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ShippingMethodToStateMap where ShippingMethodID=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ShippingMethodToCountryMap where ShippingMethodID=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ShippingMethodToZoneMap where ShippingMethodID=" + DeleteID.ToString());
                DB.ExecuteSQL("update shoppingcart set ShippingMethodID=0, ShippingMethod=NULL where ShippingMethodID=" + DeleteID.ToString());
            }

            if (IsShippingMethod)
            {
                writer.Write("<b>NOTICE:</b>&nbsp;&nbsp;&nbsp;ItemUpdate");
                writer.Write("<br />");
            }

            DataSet ds = DB.GetDS("select * from ShippingMethod " + DB.GetNoLock() + " where IsRTShipping=0 order by DisplayOrder", false);
            writer.Write("<form method=\"POST\" action=\"ShippingMethods.aspx\">\n");
            writer.Write("<input type=\"button\" value=\"Add New Shipping Method\" name=\"AddNew\" onClick=\"self.location='editShippingMethod.aspx';\"><br/>\n");
            writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
            writer.Write("<table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
            writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
            writer.Write("<td width=\"5%\" align=\"center\"><b>ID</b></td>\n");
            writer.Write("<td align=\"left\"><b>Method</b></td>\n");
            int ColSpan = 4;
            if (AppLogic.AppConfigBool("ShipRush.Enabled"))
            {
                ColSpan++;
                writer.Write("<td align=\"left\"><b>ShipRush Template</b></td>\n");
            }
            writer.Write("<td align=\"center\"><b>Display Order</b></td>\n");
            writer.Write("<td align=\"center\"><b>Edit</b></td>\n");
            writer.Write("<td align=\"center\"><b>Allowed States</b></td>\n");
            writer.Write("<td align=\"center\"><b>Allowed Countries</b></td>\n");
            writer.Write("<td align=\"center\"><b>Allowed Zones</b></td>\n");
            if (AppLogic.AppConfigBool("UseMappingShipToPayment"))
            {
                ColSpan++;
                writer.Write("<td align=\"center\"><b>Allowed Payment Methods</b></td>\n");
            }
            writer.Write("<td align=\"center\"><b>Delete</b></td>\n");
            writer.Write("</tr>\n");
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                int ThisID = DB.RowFieldInt(row, "ShippingMethodID");
                writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                writer.Write("<td width=\"5%\"  align=\"center\">" + ThisID.ToString() + "</td>\n");
                writer.Write("<td align=\"left\"><a href=\"editShippingMethod.aspx?ShippingMethodID=" + ThisID.ToString() + "\">" + DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting) + "</a></td>\n");
                writer.Write("<td align=\"center\"><input size=\"2\" type=\"text\" name=\"DisplayOrder_" + ThisID.ToString() + "\" value=\"" + DB.RowFieldInt(row, "DisplayOrder").ToString() + "\"></td>\n");
                if (AppLogic.AppConfigBool("ShipRush.Enabled"))
                {
                    writer.Write("<td align=\"left\">" + DB.RowField(row, "ShipRushTemplate") + "</td>\n");
                }
                writer.Write("<td align=\"center\"><input type=\"button\" value=\"Edit\" name=\"Edit_" + ThisID.ToString() + "\" onClick=\"self.location='editShippingMethod.aspx?ShippingMethodID=" + ThisID.ToString() + "'\"></td>\n");
                writer.Write("<td align=\"center\"><input type=\"button\" value=\"Set Allowed States\" name=\"SetStates_" + ThisID.ToString() + "\" onClick=\"self.location='ShippingMethodStates.aspx?ShippingMethodID=" + ThisID.ToString() + "'\"></td>\n");
                writer.Write("<td align=\"center\"><input type=\"button\" value=\"Set Allowed Countries\" name=\"SetCountries_" + ThisID.ToString() + "\" onClick=\"self.location='ShippingMethodCountries.aspx?ShippingMethodID=" + ThisID.ToString() + "'\"></td>\n");
                writer.Write("<td align=\"center\"><input type=\"button\" value=\"Set Allowed Zones\" name=\"SetZones_" + ThisID.ToString() + "\" onClick=\"self.location='ShippingMethodZones.aspx?ShippingMethodID=" + ThisID.ToString() + "'\"></td>\n");
                if (AppLogic.AppConfigBool("UseMappingShipToPayment"))
                {
                    writer.Write("<td align=\"center\"><input type=\"button\" value=\"Set Allowed Payment Methods\" name=\"SetPaymentMethods_" + ThisID.ToString() + "\" onClick=\"self.location='MapShippingMethodToPaymentMethod.aspx?ShippingMethodID=" + ThisID.ToString() + "'\"></td>\n");
                }
                writer.Write("<td align=\"center\"><input type=\"button\" value=\"Delete\" name=\"Delete_" + ThisID.ToString() + "\" onClick=\"DeleteShippingMethod(" + ThisID.ToString() + ")\"></td>\n");
                writer.Write("</tr>\n");
            }
            ds.Dispose();
            writer.Write("<tr>\n");
            writer.Write("<td colspan=\"" + CommonLogic.IIF(AppLogic.AppConfigBool("ShipRush.Enabled"), "3", "2") + "\" align=\"left\"></td>\n");
            writer.Write("<td align=\"center\" bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></td>\n");
            writer.Write("<td colspan=\"" + ColSpan.ToString() + "\"></td>\n");
            writer.Write("</tr>\n");
            writer.Write("</table>\n");
            writer.Write("<input type=\"button\" value=\"Add New Shipping Method\" name=\"AddNew\" onClick=\"self.location='editShippingMethod.aspx';\">\n");
            writer.Write("</form>\n");

            // ---------------------------------------------------------
            // REAL TIME RATES ADDED AUTOMATICALLY BY STOREFRONT:
            // ---------------------------------------------------------
            writer.Write("<hr size=1>");
            writer.Write("<p><b>The following Real Time Shipping Methods have been added automatically by the storefront, based on the rates returned for various customers. They should also be automatically mapped to allowed states & countries. You should only ever need to delete these shipping methods (and that should not be very often).<br/><br/>How were these mapped to states & countries? We assume that the carriers only return rates valid for the customer who requested them, so we analyzed that and just add the rate to the state and country that the customer was in when they requested the rates.<br/><br/>NOTE: It should be unusually rare to have to delete one of these methods! If you want to exclude rates from being used by customers, set the AppConfig:RTShipping.ShippingMethodsToPrevent parameter!</b></p>");
            ds = DB.GetDS("select * from ShippingMethod " + DB.GetNoLock() + " where IsRTShipping=1 order by DisplayOrder", false);
            writer.Write("  <table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
            writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
            writer.Write("      <td width=\"5%\" align=\"center\"><b>ID</b></td>\n");
            writer.Write("      <td align=\"left\"><b>Method</b></td>\n");
            if (AppLogic.AppConfigBool("ShipRush.Enabled"))
            {
                writer.Write("      <td align=\"center\"><b>ShipRush Template</b></td>\n");
                writer.Write("      <td align=\"center\"><b>Edit</b></td>\n");
            }
            writer.Write("      <td align=\"center\"><b>Delete</b></td>\n");
            writer.Write("    </tr>\n");
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                int ThisID = DB.RowFieldInt(row, "ShippingMethodID");
                writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                writer.Write("      <td width=\"5%\"  align=\"center\">" + ThisID.ToString() + "</td>\n");
                writer.Write("      <td align=\"left\">" + DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting) + "</td>\n");
                if (AppLogic.AppConfigBool("ShipRush.Enabled"))
                {
                    writer.Write("      <td align=\"left\">" + DB.RowField(row, "ShipRushTemplate") + "</td>\n");
                    writer.Write("      <td align=\"center\"><input type=\"button\" value=\"Edit\" name=\"Edit_" + ThisID.ToString() + "\" onClick=\"self.location='editShippingMethod.aspx?ShippingMethodID=" + ThisID.ToString() + "'\"></td>\n");
                }
                writer.Write("      <td align=\"center\"><input type=\"button\" value=\"Delete\" name=\"Delete_" + ThisID.ToString() + "\" onClick=\"DeleteShippingMethod(" + ThisID.ToString() + ")\"></td>\n");
                writer.Write("    </tr>\n");
            }
            ds.Dispose();
            writer.Write("</table>\n");

            writer.Write("<script type=\"text/javascript\">\n");
            writer.Write("function DeleteShippingMethod(id)\n");
            writer.Write("{\n");
            writer.Write("if(confirm('Are you sure you want to delete Shipping Method: ' + id + '. This action cannot be undone!'))\n");
            writer.Write("{\n");
            writer.Write("self.location = 'ShippingMethods.aspx?deleteid=' + id;\n");
            writer.Write("}\n");
            writer.Write("}\n");
            writer.Write("</SCRIPT>\n");
        }

    }
}
