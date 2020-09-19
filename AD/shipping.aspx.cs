// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/shipping.aspx.cs 10    9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for shippingfrm.f
    /// </summary>
    public partial class shippingfrm : AspDotNetStorefront.SkinBase
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            SectionTitle = "Shipping Tables";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            String EditGUID = CommonLogic.FormCanBeDangerousContent("EditGUID");
            if (EditGUID.Length == 0)
            {
                EditGUID = CommonLogic.QueryStringCanBeDangerousContent("EditGUID");
            }

            if (CommonLogic.FormBool("IsSubmitCalculationID"))
            {
                DB.ExecuteSQL("Update ShippingCalculation set Selected=0");
                DB.ExecuteSQL("Update ShippingCalculation set Selected=1 where ShippingCalculationID=" + CommonLogic.FormUSInt("ShippingCalculationID").ToString());
            }

            if (CommonLogic.FormBool("IsSubmitFixedRate"))
            {
                DataSet ds = DB.GetDS("select * from ShippingMethod  " + DB.GetNoLock() + " where name not like " + DB.SQuote("%Real Time%") + " and IsRTShipping=0 order by DisplayOrder", false);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    String FieldName = "FixedRate_" + DB.RowFieldInt(row, "ShippingMethodID").ToString();
                    if (CommonLogic.FormCanBeDangerousContent(FieldName).Length != 0)
                    {
                        DB.ExecuteSQL("Update ShippingMethod set FixedRate=" + Localization.CurrencyStringForDBWithoutExchangeRate(CommonLogic.FormUSDecimal(FieldName)) + " where ShippingMethodID=" + DB.RowFieldInt(row, "ShippingMethodID").ToString());
                    }
                    else
                    {
                        DB.ExecuteSQL("Update ShippingMethod set FixedRate=NULL where ShippingMethodID=" + DB.RowFieldInt(row, "ShippingMethodID").ToString());
                    }
                }
                ds.Dispose();
            }

            if (CommonLogic.FormBool("IsSubmitFixedPercentOfTotal"))
            {
                DataSet ds = DB.GetDS("select * from ShippingMethod  " + DB.GetNoLock() + " where name not like " + DB.SQuote("%Real Time%") + " and IsRTShipping=0 order by DisplayOrder", false);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    String FieldName = "FixedPercentOfTotal_" + DB.RowFieldInt(row, "ShippingMethodID").ToString();
                    if (CommonLogic.FormCanBeDangerousContent(FieldName).Length != 0)
                    {
                        DB.ExecuteSQL("Update ShippingMethod set FixedPercentOfTotal=" + Localization.DecimalStringForDB(CommonLogic.FormUSDecimal(FieldName)) + " where ShippingMethodID=" + DB.RowFieldInt(row, "ShippingMethodID").ToString());
                    }
                    else
                    {
                        DB.ExecuteSQL("Update ShippingMethod set FixedPercentOfTotal=NULL where ShippingMethodID=" + DB.RowFieldInt(row, "ShippingMethodID").ToString());
                    }
                }
                ds.Dispose();
            }

            if (CommonLogic.FormBool("IsSubmitByTotal"))
            {
                if (EditGUID.Length != 0)
                {
                    DB.ExecuteSQL("delete from ShippingByTotal where RowGUID=" + DB.SQuote(EditGUID));
                }

                // check for new row addition:
                Decimal Low0 = CommonLogic.FormUSDecimal("Low_0");
                Decimal High0 = CommonLogic.FormUSDecimal("High_0");
                String NewRowGUID = DB.GetNewGUID();

                if (Low0 != System.Decimal.Zero || High0 != System.Decimal.Zero)
                {
                    // add the new row if necessary:
                    DataSet dsx = DB.GetDS("select * from ShippingMethod  " + DB.GetNoLock() + " where IsRTShipping=0 order by DisplayOrder", false);
                    foreach (DataRow row in dsx.Tables[0].Rows)
                    {
                        decimal Charge = CommonLogic.FormUSDecimal("Rate_0_" + DB.RowFieldInt(row, "ShippingMethodID").ToString());
                        DB.ExecuteSQL("insert into ShippingByTotal(RowGUID,LowValue,HighValue,ShippingMethodID,ShippingCharge) values(" + DB.SQuote(NewRowGUID) + "," + Localization.CurrencyStringForDBWithoutExchangeRate(Low0) + "," + Localization.CurrencyStringForDBWithoutExchangeRate(High0) + "," + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "," + Localization.CurrencyStringForDBWithoutExchangeRate(Charge) + ")");
                    }
                    dsx.Dispose();
                }

                // update existing rows:
                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    String FieldName = Request.Form.Keys[i];
                    if (FieldName.IndexOf("_0_") == -1 && FieldName != "Low_0" && FieldName != "High_0" && FieldName.IndexOf("_vldt") == -1 && (FieldName.IndexOf("Rate_") != -1 || FieldName.IndexOf("Low_") != -1 || FieldName.IndexOf("High_") != -1))
                    {
                        decimal FieldVal = CommonLogic.FormUSDecimal(FieldName);
                        // this field should be processed
                        String[] Parsed = FieldName.Split('_');
                        if (FieldName.IndexOf("Rate_") != -1)
                        {
                            // update shipping costs:
                            DB.ExecuteSQL("insert into ShippingByTotal(RowGUID,LowValue,HighValue,ShippingMethodID,ShippingCharge) values(" + DB.SQuote(Parsed[1]) + "," + Localization.CurrencyStringForDBWithoutExchangeRate(CommonLogic.FormUSDecimal("Low_" + Parsed[1])) + "," + Localization.CurrencyStringForDBWithoutExchangeRate(CommonLogic.FormUSDecimal("High_" + Parsed[1])) + "," + Parsed[2] + "," + Localization.CurrencyStringForDBWithoutExchangeRate(FieldVal) + ")");
                        }
                    }
                }
                DB.ExecuteSQL("Update ShippingByTotal set HighValue=99999.99 where HighValue=0.0 and LowValue<>0.0");
            }

            if (CommonLogic.FormBool("IsSubmitByTotalByPercent"))
            {
                if (EditGUID.Length != 0)
                {
                    DB.ExecuteSQL("delete from ShippingByTotalByPercent where RowGUID=" + DB.SQuote(EditGUID));
                }

                // check for new row addition:
                Decimal Low0 = CommonLogic.FormUSDecimal("Low_0");
                Decimal High0 = CommonLogic.FormUSDecimal("High_0");
                Decimal Minimum0 = CommonLogic.FormUSDecimal("Minimum_0");
                Decimal Base0 = CommonLogic.FormUSDecimal("Base_0");
                String NewRowGUID = DB.GetNewGUID();

                if (Low0 != System.Decimal.Zero || High0 != System.Decimal.Zero)
                {
                    // add the new row if necessary:
                    DataSet dsx = DB.GetDS("select * from ShippingMethod  " + DB.GetNoLock() + " where IsRTShipping=0 order by DisplayOrder", false);
                    foreach (DataRow row in dsx.Tables[0].Rows)
                    {
                        decimal PercentOfTotal = CommonLogic.FormUSDecimal("Rate_0_" + DB.RowFieldInt(row, "ShippingMethodID").ToString());
                        String sql = "insert into ShippingByTotalByPercent(RowGUID,LowValue,HighValue,ShippingMethodID,MinimumCharge,SurCharge,PercentOfTotal) values(" + DB.SQuote(NewRowGUID) + "," + Localization.CurrencyStringForDBWithoutExchangeRate(Low0) + "," + Localization.CurrencyStringForDBWithoutExchangeRate(High0) + "," + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "," + Localization.CurrencyStringForDBWithoutExchangeRate(Minimum0) + "," + Localization.CurrencyStringForDBWithoutExchangeRate(Base0) + "," + Localization.CurrencyStringForDBWithoutExchangeRate(PercentOfTotal) + ")";
                        DB.ExecuteSQL(sql);
                    }
                    dsx.Dispose();
                }

                // update existing rows:
                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    String FieldName = Request.Form.Keys[i];
                    if (FieldName.IndexOf("_0_") == -1 && FieldName != "Low_0" && FieldName != "High_0" && FieldName.IndexOf("_vldt") == -1 && (FieldName.IndexOf("Rate_") != -1 || FieldName.IndexOf("Low_") != -1 || FieldName.IndexOf("High_") != -1))
                    {
                        decimal FieldVal = CommonLogic.FormUSDecimal(FieldName);
                        // this field should be processed
                        String[] Parsed = FieldName.Split('_');
                        if (FieldName.IndexOf("Rate_") != -1)
                        {
                            // update shipping costs:
                            String sql = "insert into ShippingByTotalByPercent(RowGUID,LowValue,HighValue,ShippingMethodID,MinimumCharge,SurCharge,PercentOfTotal) values(" + DB.SQuote(Parsed[1]) + "," + Localization.CurrencyStringForDBWithoutExchangeRate(CommonLogic.FormUSDecimal("Low_" + Parsed[1])) + "," + Localization.CurrencyStringForDBWithoutExchangeRate(CommonLogic.FormUSDecimal("High_" + Parsed[1])) + "," + Parsed[2] + "," + Localization.CurrencyStringForDBWithoutExchangeRate(CommonLogic.FormUSDecimal("Minimum_" + Parsed[1])) + "," + Localization.CurrencyStringForDBWithoutExchangeRate(CommonLogic.FormUSDecimal("Base_" + Parsed[1])) + "," + Localization.CurrencyStringForDBWithoutExchangeRate(FieldVal) + ")";
                            DB.ExecuteSQL(sql);
                        }
                    }
                }
                DB.ExecuteSQL("Update ShippingByTotalByPercent set HighValue=99999.99 where HighValue=0.0 and LowValue<>0.0");
            }

            if (CommonLogic.FormBool("IsSubmitByWeight"))
            {
                if (EditGUID.Length != 0)
                {
                    DB.ExecuteSQL("delete from ShippingByWeight where RowGUID=" + DB.SQuote(EditGUID));
                }

                // check for new row addition:
                Decimal Low0 = CommonLogic.FormUSDecimal("Low_0");
                Decimal High0 = CommonLogic.FormUSDecimal("High_0");
                String NewRowGUID = DB.GetNewGUID();

                if (Low0 != 0.0M || High0 != 0.0M)
                {
                    // add the new row if necessary:
                    DataSet dsx = DB.GetDS("select * from ShippingMethod  " + DB.GetNoLock() + " where IsRTShipping=0 order by DisplayOrder", false);
                    foreach (DataRow row in dsx.Tables[0].Rows)
                    {
                        decimal Charge = CommonLogic.FormUSDecimal("Rate_0_" + DB.RowFieldInt(row, "ShippingMethodID").ToString());
                        DB.ExecuteSQL("insert into ShippingByWeight(RowGUID,LowValue,HighValue,ShippingMethodID,ShippingCharge) values(" + DB.SQuote(NewRowGUID) + "," + Localization.DecimalStringForDB(Low0) + "," + Localization.DecimalStringForDB(High0) + "," + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "," + Localization.CurrencyStringForDBWithoutExchangeRate(Charge) + ")");
                    }
                    dsx.Dispose();
                }

                // update existing rows:
                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    String FieldName = Request.Form.Keys[i];
                    if (FieldName.IndexOf("_0_") == -1 && FieldName != "Low_0" && FieldName != "High_0" && FieldName.IndexOf("_vldt") == -1 && (FieldName.IndexOf("Rate_") != -1 || FieldName.IndexOf("Low_") != -1 || FieldName.IndexOf("High_") != -1))
                    {
                        decimal FieldVal = CommonLogic.FormUSDecimal(FieldName);
                        // this field should be processed
                        String[] Parsed = FieldName.Split('_');
                        if (FieldName.IndexOf("Rate_") != -1)
                        {
                            // update shipping costs:
                            DB.ExecuteSQL("insert into ShippingByWeight(RowGUID,LowValue,HighValue,ShippingMethodID,ShippingCharge) values(" + DB.SQuote(Parsed[1]) + "," + Localization.DecimalStringForDB(CommonLogic.FormUSDecimal("Low_" + Parsed[1])) + "," + Localization.DecimalStringForDB(CommonLogic.FormUSDecimal("High_" + Parsed[1])) + "," + Parsed[2] + "," + Localization.CurrencyStringForDBWithoutExchangeRate(FieldVal) + ")");
                        }
                    }
                }
                DB.ExecuteSQL("Update ShippingByWeight set HighValue=99999.99 where HighValue=0.0 and LowValue<>0.0");
            }

            if (CommonLogic.FormBool("IsSubmitWeightByZone"))
            {
                int ShippingMethodID = CommonLogic.FormUSInt("ShippingMethodID");
                if (ShippingMethodID == 0)
                {
                    ShippingMethodID = CommonLogic.QueryStringUSInt("ShippingMethodID");
                }
                if (EditGUID.Length != 0)
                {
                    DB.ExecuteSQL("delete from ShippingWeightByZone where ShippingMethodID=" + ShippingMethodID.ToString() + " and RowGUID=" + DB.SQuote(EditGUID));
                }

                // check for new row addition:
                Decimal Low0 = CommonLogic.FormUSDecimal("Low_0");
                Decimal High0 = CommonLogic.FormUSDecimal("High_0");
                String NewRowGUID = DB.GetNewGUID();

                if (Low0 != 0.0M || High0 != 0.0M)
                {
                    // add the new row if necessary:
                    DataSet dsx = DB.GetDS("select * from ShippingZone " + DB.GetNoLock() + " where Deleted=0 order by DisplayOrder,Name", false);
                    foreach (DataRow row in dsx.Tables[0].Rows)
                    {
                        Decimal Charge = CommonLogic.FormUSDecimal("Rate_0_" + DB.RowFieldInt(row, "ShippingZoneID").ToString());
                        DB.ExecuteSQL("insert into ShippingWeightByZone(RowGUID,ShippingMethodID,LowValue,HighValue,ShippingZoneID,ShippingCharge) values(" + DB.SQuote(NewRowGUID) + "," + ShippingMethodID.ToString() + "," + Localization.DecimalStringForDB(Low0) + "," + Localization.DecimalStringForDB(High0) + "," + DB.RowFieldInt(row, "ShippingZoneID").ToString() + "," + Localization.CurrencyStringForDBWithoutExchangeRate(Charge) + ")");
                    }
                    dsx.Dispose();
                }

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
                            // update shipping costs:
                            DB.ExecuteSQL("insert into ShippingWeightByZone(RowGUID,ShippingMethodID,LowValue,HighValue,ShippingZoneID,ShippingCharge) values(" + DB.SQuote(Parsed[1]) + "," + ShippingMethodID.ToString() + "," + Localization.DecimalStringForDB(CommonLogic.FormUSDecimal("Low_" + Parsed[1])) + "," + Localization.DecimalStringForDB(CommonLogic.FormUSDecimal("High_" + Parsed[1])) + "," + Parsed[2] + "," + Localization.CurrencyStringForDBWithoutExchangeRate(FieldVal) + ")");
                        }
                    }
                }
                DB.ExecuteSQL("Update ShippingWeightByZone set HighValue=99999.99 where HighValue=0.0 and LowValue<>0.0");
            }

            if (CommonLogic.FormBool("IsSubmitTotalByZone"))
            {
                int ShippingMethodID = CommonLogic.FormUSInt("ShippingMethodID");
                if (ShippingMethodID == 0)
                {
                    ShippingMethodID = CommonLogic.QueryStringUSInt("ShippingMethodID");
                }
                if (EditGUID.Length != 0)
                {
                    DB.ExecuteSQL("delete from ShippingTotalByZone where ShippingMethodID=" + ShippingMethodID.ToString() + " and RowGUID=" + DB.SQuote(EditGUID));
                }

                // check for new row addition:
                Decimal Low0 = CommonLogic.FormUSDecimal("Low_0");
                Decimal High0 = CommonLogic.FormUSDecimal("High_0");
                String NewRowGUID = DB.GetNewGUID();

                if (Low0 != 0.0M || High0 != 0.0M)
                {
                    // add the new row if necessary:
                    DataSet dsx = DB.GetDS("select * from ShippingZone " + DB.GetNoLock() + " where Deleted=0 order by DisplayOrder,Name", false);
                    foreach (DataRow row in dsx.Tables[0].Rows)
                    {
                        Decimal Charge = CommonLogic.FormUSDecimal("Rate_0_" + DB.RowFieldInt(row, "ShippingZoneID").ToString());
                        DB.ExecuteSQL("insert into ShippingTotalByZone(RowGUID,ShippingMethodID,LowValue,HighValue,ShippingZoneID,ShippingCharge) values(" + DB.SQuote(NewRowGUID) + "," + ShippingMethodID.ToString() + "," + Localization.CurrencyStringForDBWithoutExchangeRate(Low0) + "," + Localization.CurrencyStringForDBWithoutExchangeRate(High0) + "," + DB.RowFieldInt(row, "ShippingZoneID").ToString() + "," + Localization.CurrencyStringForDBWithoutExchangeRate(Charge) + ")");
                    }
                    dsx.Dispose();
                }

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
                            // update shipping costs:
                            DB.ExecuteSQL("insert into ShippingTotalByZone(RowGUID,ShippingMethodID,LowValue,HighValue,ShippingZoneID,ShippingCharge) values(" + DB.SQuote(Parsed[1]) + "," + ShippingMethodID.ToString() + "," + Localization.CurrencyStringForDBWithoutExchangeRate(CommonLogic.FormUSDecimal("Low_" + Parsed[1])) + "," + Localization.CurrencyStringForDBWithoutExchangeRate(CommonLogic.FormUSDecimal("High_" + Parsed[1])) + "," + Parsed[2] + "," + Localization.CurrencyStringForDBWithoutExchangeRate(FieldVal) + ")");
                        }
                    }
                }
                DB.ExecuteSQL("Update ShippingTotalByZone set HighValue=99999.99 where HighValue=0.0 and LowValue<>0.0");
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("deletebytotalid").Length != 0)
            {
                DB.ExecuteSQL("delete from ShippingByTotal where RowGUID=" + DB.SQuote(CommonLogic.QueryStringCanBeDangerousContent("DeleteByTotalID")));
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("deletebytotalbypercentid").Length != 0)
            {
                DB.ExecuteSQL("delete from ShippingByTotalByPercent where RowGUID=" + DB.SQuote(CommonLogic.QueryStringCanBeDangerousContent("DeleteByTotalByPercentID")));
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("deletebyWeightid").Length != 0)
            {
                DB.ExecuteSQL("delete from ShippingByWeight where RowGUID=" + DB.SQuote(CommonLogic.QueryStringCanBeDangerousContent("DeleteByWeightID")));
            }
            if (CommonLogic.QueryStringCanBeDangerousContent("deleteWeightByZoneid").Length != 0)
            {
                DB.ExecuteSQL("delete from ShippingWeightByZone where ShippingMethodID=" + CommonLogic.QueryStringUSInt("ShippingMethodID").ToString() + " and RowGUID=" + DB.SQuote(CommonLogic.QueryStringCanBeDangerousContent("DeleteWeightByZoneID")));
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("deleteTotalByZoneid").Length != 0)
            {
                DB.ExecuteSQL("delete from ShippingTotalByZone where ShippingMethodID=" + CommonLogic.QueryStringUSInt("ShippingMethodID").ToString() + " and RowGUID=" + DB.SQuote(CommonLogic.QueryStringCanBeDangerousContent("DeleteTotalByZoneID")));
            }

            writer.Write("<script type=\"text/javascript\">\n");
            writer.Write("function ShippingForm_Validator(theForm)\n");
            writer.Write("{\n");
            writer.Write("submitonce(theForm);\n");
            writer.Write("return (true);\n");
            writer.Write("}\n");
            writer.Write("function FixedRateForm_Validator(theForm)\n");
            writer.Write("{\n");
            writer.Write("submitonce(theForm);\n");
            writer.Write("return (true);\n");
            writer.Write("}\n");
            writer.Write("function FixedPercentOfTotalForm_Validator(theForm)\n");
            writer.Write("{\n");
            writer.Write("submitonce(theForm);\n");
            writer.Write("return (true);\n");
            writer.Write("}\n");
            writer.Write("function ByTotalForm_Validator(theForm)\n");
            writer.Write("{\n");
            writer.Write("submitonce(theForm);\n");
            writer.Write("return (true);\n");
            writer.Write("}\n");
            writer.Write("function ByTotalByPercentForm_Validator(theForm)\n");
            writer.Write("{\n");
            writer.Write("submitonce(theForm);\n");
            writer.Write("return (true);\n");
            writer.Write("}\n");
            writer.Write("function ByWeightForm_Validator(theForm)\n");
            writer.Write("{\n");
            writer.Write("submitonce(theForm);\n");
            writer.Write("return (true);\n");
            writer.Write("}\n");
            writer.Write("function WeightByZoneForm_Validator(theForm)\n");
            writer.Write("{\n");
            writer.Write("submitonce(theForm);\n");
            writer.Write("return (true);\n");
            writer.Write("}\n");
            writer.Write("function TotalByZoneForm_Validator(theForm)\n");
            writer.Write("{\n");
            writer.Write("submitonce(theForm);\n");
            writer.Write("return (true);\n");
            writer.Write("}\n");
            writer.Write("</script>\n");

            IDataReader rs = DB.GetRS("select * from ShippingCalculation  " + DB.GetNoLock() + " order by shippingcalculationid");
            writer.Write("<form action=\"shipping.aspx\" method=\"post\" id=\"ShippingForm\" name=\"ShippingForm\" onsubmit=\"return (validateForm(this) && ShippingForm_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
            writer.Write("<input type=\"hidden\" name=\"IsSubmitCalculationID\" value=\"true\">\n");
            Shipping.ShippingCalculationEnum ShipCalcID = 0;
            while (rs.Read())
            {
                writer.Write("<p><input type=\"radio\" name=\"ShippingCalculationID\" value=\"" + DB.RSFieldInt(rs, "ShippingCalculationID").ToString() + "\" " + CommonLogic.IIF(DB.RSFieldBool(rs, "Selected"), " checked ", "") + "><b>" + DB.RSFieldByLocale(rs, "Name", ThisCustomer.LocaleSetting) + "</b></p>\n");
                if (DB.RSFieldBool(rs, "Selected"))
                {
                    ShipCalcID = (Shipping.ShippingCalculationEnum)DB.RSFieldInt(rs, "ShippingCalculationID");
                }
            }
            rs.Close();
            writer.Write("<input type=\"submit\" value=\"Set As Active Shipping Calculation Method\" name=\"submit\">\n");
            writer.Write("</form>\n");

            switch (ShipCalcID)
            {
                case Shipping.ShippingCalculationEnum.CalculateShippingByWeight:
                    {
                        writer.Write("<hr size=1>");

                        writer.Write("<p><b>ACTIVE RATE TABLE FOR: CALCULATE SHIPPING BY ORDER WEIGHT:</b></p>\n");

                        writer.Write("<form action=\"shipping.aspx?EditGUID=" + EditGUID + "\" method=\"post\" id=\"ByWeightForm\" name=\"ByWeightForm\" onsubmit=\"return (validateForm(this) && ByWeightForm_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                        writer.Write("<input type=\"hidden\" name=\"IsSubmitByWeight\" value=\"true\">\n");

                        DataSet dsWeight = DB.GetDS("select * from ShippingMethod  " + DB.GetNoLock() + " where IsRTShipping=0 order by DisplayOrder", false);
                        if (dsWeight.Tables[0].Rows.Count == 0)
                        {
                            writer.Write("No shipping methods are defined. <a href=\"shippingmethods.aspx\"><b>Click here</b></a> to define your shipping methods");
                        }
                        else
                        {
                            writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"1\">\n");
                            writer.Write("<tr bgcolor=\"#FFFFDD\"><td colspan=2 align=\"center\"><b>Order Weight (in " + Localization.WeightUnits() + ")</b></td><td colspan=" + dsWeight.Tables[0].Rows.Count.ToString() + " align=\"center\"><b>Shipping Charge By Weight</b></td><td>&nbsp;</td><td>&nbsp;</td></tr>\n");
                            writer.Write("<tr>\n");
                            writer.Write("<td align=\"center\"><b>Low</b></td>\n");
                            writer.Write("<td align=\"center\"><b>High</b></td>\n");
                            foreach (DataRow row in dsWeight.Tables[0].Rows)
                            {
                                writer.Write("<td align=\"center\"><b>" + DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting) + "</b></td>\n");
                            }
                            writer.Write("<td align=\"center\"><b>Edit</b></td>\n");
                            writer.Write("<td align=\"center\"><b>Delete</b></td>\n");
                            writer.Write("</tr>\n");

                            DataSet ShippingRows = DB.GetDS("select distinct rowguid,lowvalue,highvalue from ShippingByWeight  " + DB.GetNoLock() + " order by LowValue", false);
                            foreach (DataRow shiprow in ShippingRows.Tables[0].Rows)
                            {
                                bool EditRow = (EditGUID == DB.RowFieldGUID(shiprow, "RowGUID"));
                                writer.Write("<tr>\n");
                                writer.Write("<td align=\"center\">\n");
                                if (EditRow)
                                {
                                    writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Low_" + DB.RowFieldGUID(shiprow, "RowGUID") + "\" value=\"" + Localization.DecimalStringForDB(DB.RowFieldDecimal(shiprow, "LowValue")) + "\">\n");
                                    writer.Write("<input type=\"hidden\" name=\"Low_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_vldt\" value=\"[number][blankalert=Please enter starting order amount][invalidalert=Please enter a weight value]\">\n");
                                }
                                else
                                {
                                    writer.Write(Localization.DecimalStringForDB(DB.RowFieldDecimal(shiprow, "LowValue")));
                                }
                                writer.Write("</td>\n");
                                writer.Write("<td align=\"center\">\n");
                                if (EditRow)
                                {
                                    writer.Write("<input maxLength=\"10\" size=\"10\" name=\"High_" + DB.RowFieldGUID(shiprow, "RowGUID") + "\" value=\"" + Localization.DecimalStringForDB(DB.RowFieldDecimal(shiprow, "HighValue")) + "\">\n");
                                    writer.Write("<input type=\"hidden\" name=\"High_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_vldt\" value=\"[number][blankalert=Please enter ending order amount][invalidalert=Please enter a weight value]\">\n");
                                }
                                else
                                {
                                    writer.Write(Localization.DecimalStringForDB(DB.RowFieldDecimal(shiprow, "HighValue")));
                                }
                                writer.Write("</td>\n");
                                foreach (DataRow row in dsWeight.Tables[0].Rows)
                                {
                                    writer.Write("<td align=\"center\">\n");
                                    if (EditRow)
                                    {
                                        writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Rate_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_" + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(Shipping.GetShipByWeightCharge(DB.RowFieldInt(row, "ShippingMethodID"), DB.RowFieldGUID(shiprow, "RowGUID"))) + "\">\n");
                                        writer.Write("<input type=\"hidden\" name=\"Rate_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_" + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "_vldt\" value=\"[number][blankalert=Please enter the shipping cost][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                    }
                                    else
                                    {
                                        writer.Write(Localization.CurrencyStringForDBWithoutExchangeRate(Shipping.GetShipByWeightCharge(DB.RowFieldInt(row, "ShippingMethodID"), DB.RowFieldGUID(shiprow, "RowGUID"))));
                                    }
                                    writer.Write("</td>\n");
                                }
                                if (EditRow)
                                {
                                    writer.Write("<td align=\"center\">");
                                    writer.Write("<input type=\"submit\" value=\"Update\" name=\"submit\">\n");
                                    writer.Write("</td>");
                                }
                                else
                                {
                                    writer.Write("<td align=\"center\"><input type=\"Button\" name=\"Edit\" value=\"Edit\" onClick=\"self.location='shipping.aspx?EditGUID=" + DB.RowFieldGUID(shiprow, "RowGUID") + "'\"></td>\n");
                                }
                                writer.Write("<td align=\"center\"><input type=\"Button\" name=\"Delete\" value=\"Delete\" onClick=\"self.location='shipping.aspx?DeleteByWeightID=" + DB.RowFieldGUID(shiprow, "RowGUID") + "'\"></td>\n");
                                writer.Write("</tr>\n");
                            }
                            // add new row:
                            writer.Write("<tr>\n");
                            writer.Write("<td align=\"center\">\n");
                            writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Low_0\" \">\n");
                            writer.Write("<input type=\"hidden\" name=\"Low_0_vldt\" value=\"[number][blankalert=Please enter starting order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                            writer.Write("</td>\n");
                            writer.Write("<td align=\"center\">\n");
                            writer.Write("<input maxLength=\"10\" size=\"10\" name=\"High_0\" >\n");
                            writer.Write("<input type=\"hidden\" name=\"High_0_vldt\" value=\"[number][blankalert=Please enter ending order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                            writer.Write("</td>\n");
                            foreach (DataRow row in dsWeight.Tables[0].Rows)
                            {
                                writer.Write("<td align=\"center\">\n");
                                writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Rate_0_" + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "\">\n");
                                writer.Write("<input type=\"hidden\" name=\"Rate_0_" + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "_vldt\" value=\"[number][blankalert=Please enter the shipping cost][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                writer.Write("</td>\n");
                            }
                            writer.Write("<td align=\"center\">");
                            writer.Write("<input type=\"submit\" value=\"Add New Row\" name=\"submit\">\n");
                            writer.Write("</td>\n");
                            writer.Write("<td>&nbsp;</td>");
                            writer.Write("</tr>\n");
                            writer.Write("</table>\n");

                            ShippingRows.Dispose();
                        }
                        dsWeight.Dispose();

                        writer.Write("</form>\n");
                        break;
                    }
                case Shipping.ShippingCalculationEnum.CalculateShippingByTotal:
                    {
                        writer.Write("<hr size=1>");

                        writer.Write("<p><b>ACTIVE RATE TABLE FOR: CALCULATE SHIPPING BY ORDER TOTAL:</b></p>\n");

                        writer.Write("<form action=\"shipping.aspx?EditGUID=" + EditGUID + "\" method=\"post\" id=\"ByTotalForm\" name=\"ByTotalForm\" onsubmit=\"return (validateForm(this) && ByTotalForm_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                        writer.Write("<input type=\"hidden\" name=\"IsSubmitByTotal\" value=\"true\">\n");

                        DataSet dsTotal = DB.GetDS("select * from ShippingMethod  " + DB.GetNoLock() + " where IsRTShipping=0 order by DisplayOrder", false);
                        if (dsTotal.Tables[0].Rows.Count == 0)
                        {
                            writer.Write("No shipping methods are defined. <a href=\"shippingmethods.aspx\"><b>Click here</b></a> to define your shipping methods");
                        }
                        else
                        {
                            writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"1\">\n");
                            writer.Write("<tr bgcolor=\"#FFFFDD\"><td colspan=2 align=\"center\"><b>Order Total (in your currency)</b></td><td colspan=" + dsTotal.Tables[0].Rows.Count.ToString() + " align=\"center\"><b>Shipping Charge By Total</b></td><td>&nbsp;</td><td>&nbsp;</td></tr>\n");
                            writer.Write("<tr>\n");
                            writer.Write("<td align=\"center\"><b>Low</b></td>\n");
                            writer.Write("<td align=\"center\"><b>High</b></td>\n");
                            foreach (DataRow row in dsTotal.Tables[0].Rows)
                            {
                                writer.Write("<td align=\"center\"><b>" + DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting) + "</b></td>\n");
                            }
                            writer.Write("<td align=\"center\"><b>Edit</b></td>\n");
                            writer.Write("<td align=\"center\"><b>Delete</b></td>\n");
                            writer.Write("</tr>\n");

                            DataSet ShippingRows = DB.GetDS("select distinct rowguid,lowvalue,highvalue from ShippingByTotal  " + DB.GetNoLock() + " order by LowValue", false);
                            foreach (DataRow shiprow in ShippingRows.Tables[0].Rows)
                            {
                                bool EditRow = (EditGUID == DB.RowFieldGUID(shiprow, "RowGUID"));
                                writer.Write("<tr>\n");
                                writer.Write("<td align=\"center\">\n");
                                if (EditRow)
                                {
                                    writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Low_" + DB.RowFieldGUID(shiprow, "RowGUID") + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "LowValue")) + "\">\n");
                                    writer.Write("<input type=\"hidden\" name=\"Low_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_vldt\" value=\"[number][blankalert=Please enter starting order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                }
                                else
                                {
                                    writer.Write(Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "LowValue")));
                                }
                                writer.Write("</td>\n");
                                writer.Write("<td align=\"center\">\n");
                                if (EditRow)
                                {
                                    writer.Write("<input maxLength=\"10\" size=\"10\" name=\"High_" + DB.RowFieldGUID(shiprow, "RowGUID") + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "HighValue")) + "\">\n");
                                    writer.Write("<input type=\"hidden\" name=\"High_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_vldt\" value=\"[number][blankalert=Please enter ending order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                }
                                else
                                {
                                    writer.Write(Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "HighValue")));
                                }
                                writer.Write("</td>\n");
                                foreach (DataRow row in dsTotal.Tables[0].Rows)
                                {
                                    writer.Write("<td align=\"center\">\n");
                                    if (EditRow)
                                    {
                                        writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Rate_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_" + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(Shipping.GetShipByTotalCharge(DB.RowFieldInt(row, "ShippingMethodID"), DB.RowFieldGUID(shiprow, "RowGUID"))) + "\">\n");
                                        writer.Write("<input type=\"hidden\" name=\"Rate_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_" + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "_vldt\" value=\"[number][blankalert=Please enter the shipping cost][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                    }
                                    else
                                    {
                                        writer.Write(Localization.CurrencyStringForDBWithoutExchangeRate(Shipping.GetShipByTotalCharge(DB.RowFieldInt(row, "ShippingMethodID"), DB.RowFieldGUID(shiprow, "RowGUID"))));
                                    }
                                    writer.Write("</td>\n");
                                }
                                if (EditRow)
                                {
                                    writer.Write("<td align=\"center\">");
                                    writer.Write("<input type=\"submit\" value=\"Update\" name=\"submit\">\n");
                                    writer.Write("</td>");
                                }
                                else
                                {
                                    writer.Write("<td align=\"center\"><input type=\"Button\" name=\"Edit\" value=\"Edit\" onClick=\"self.location='shipping.aspx?EditGUID=" + DB.RowFieldGUID(shiprow, "RowGUID") + "'\"></td>\n");
                                }
                                writer.Write("<td align=\"center\"><input type=\"Button\" name=\"Delete\" value=\"Delete\" onClick=\"self.location='shipping.aspx?deleteByTotalid=" + DB.RowFieldGUID(shiprow, "RowGUID") + "'\"></td>\n");
                                writer.Write("</tr>\n");
                            }
                            // add new row:
                            writer.Write("<tr>\n");
                            writer.Write("<td align=\"center\">\n");
                            writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Low_0\" \">\n");
                            writer.Write("<input type=\"hidden\" name=\"Low_0_vldt\" value=\"[number][blankalert=Please enter starting order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                            writer.Write("</td>\n");
                            writer.Write("<td align=\"center\">\n");
                            writer.Write("<input maxLength=\"10\" size=\"10\" name=\"High_0\" >\n");
                            writer.Write("<input type=\"hidden\" name=\"High_0_vldt\" value=\"[number][blankalert=Please enter ending order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                            writer.Write("</td>\n");
                            foreach (DataRow row in dsTotal.Tables[0].Rows)
                            {
                                writer.Write("<td align=\"center\">\n");
                                writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Rate_0_" + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "\">\n");
                                writer.Write("<input type=\"hidden\" name=\"Rate_0_" + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "_vldt\" value=\"[number][blankalert=Please enter the shipping cost][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                writer.Write("</td>\n");
                            }
                            writer.Write("<td align=\"center\">");
                            writer.Write("<input type=\"submit\" value=\"Add New Row\" name=\"submit\">\n");
                            writer.Write("</td>\n");
                            writer.Write("<td>&nbsp;</td>");
                            writer.Write("</tr>\n");
                            writer.Write("</table>\n");

                            ShippingRows.Dispose();
                        }
                        dsTotal.Dispose();

                        writer.Write("</form>\n");
                        break;
                    }
                case Shipping.ShippingCalculationEnum.CalculateShippingByTotalByPercent:
                    {
                        writer.Write("<hr size=1>");

                        writer.Write("<p><b>ACTIVE RATE TABLE FOR: CALCULATE SHIPPING BY ORDER TOTAL BY PERCENT:</b></p>\n");

                        writer.Write("<form action=\"shipping.aspx?EditGUID=" + EditGUID + "\" method=\"post\" id=\"ByTotalByPercentForm\" name=\"ByTotalByPercentForm\" onsubmit=\"return (validateForm(this) && ByTotalByPercentForm_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                        writer.Write("<input type=\"hidden\" name=\"IsSubmitByTotalByPercent\" value=\"true\">\n");

                        DataSet dsTotal = DB.GetDS("select * from ShippingMethod  " + DB.GetNoLock() + " where IsRTShipping=0 order by DisplayOrder", false);
                        if (dsTotal.Tables[0].Rows.Count == 0)
                        {
                            writer.Write("No shipping methods are defined. <a href=\"shippingmethods.aspx\"><b>Click here</b></a> to define your shipping methods");
                        }
                        else
                        {
                            writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"1\">\n");
                            writer.Write("<tr bgcolor=\"#FFFFDD\"><td colspan=2 align=\"center\"><b>Order Total (xx.xx)</b></td><td align=\"center\"><b>Minimum Charge (xx.xx)</b></td></td><td align=\"center\"><b>Base Charge (xx.xx)</b></td><td colspan=" + dsTotal.Tables[0].Rows.Count.ToString() + " align=\"center\"><b>Shipping Charge As % Of Total</b></td><td>&nbsp;</td><td>&nbsp;</td></tr>\n");
                            writer.Write("<tr>\n");
                            writer.Write("<td align=\"center\"><b>Low</b></td>\n");
                            writer.Write("<td align=\"center\"><b>High</b></td>\n");
                            writer.Write("<td align=\"center\"><b>Minimum</b></td>\n");
                            writer.Write("<td align=\"center\"><b>Base</b></td>\n");
                            foreach (DataRow row in dsTotal.Tables[0].Rows)
                            {
                                writer.Write("<td align=\"center\"><b>" + DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting) + "</b></td>\n");
                            }
                            writer.Write("<td align=\"center\"><b>Edit</b></td>\n");
                            writer.Write("<td align=\"center\"><b>Delete</b></td>\n");
                            writer.Write("</tr>\n");

                            DataSet ShippingRows = DB.GetDS("select distinct rowguid,lowvalue,highvalue,minimumcharge,SurCharge from ShippingByTotalByPercent " + DB.GetNoLock() + " order by LowValue", false);
                            foreach (DataRow shiprow in ShippingRows.Tables[0].Rows)
                            {
                                bool EditRow = (EditGUID == DB.RowFieldGUID(shiprow, "RowGUID"));
                                writer.Write("<tr>\n");
                                writer.Write("<td align=\"center\">\n");
                                if (EditRow)
                                {
                                    writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Low_" + DB.RowFieldGUID(shiprow, "RowGUID") + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "LowValue")) + "\">\n");
                                    writer.Write("<input type=\"hidden\" name=\"Low_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_vldt\" value=\"[number][blankalert=Please enter starting order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                }
                                else
                                {
                                    writer.Write(Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "LowValue")));
                                }
                                writer.Write("</td>\n");
                                writer.Write("<td align=\"center\">\n");
                                if (EditRow)
                                {
                                    writer.Write("<input maxLength=\"10\" size=\"10\" name=\"High_" + DB.RowFieldGUID(shiprow, "RowGUID") + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "HighValue")) + "\">\n");
                                    writer.Write("<input type=\"hidden\" name=\"High_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_vldt\" value=\"[number][blankalert=Please enter ending order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                }
                                else
                                {
                                    writer.Write(Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "HighValue")));
                                }
                                writer.Write("</td>\n");

                                writer.Write("<td align=\"center\">\n");
                                if (EditRow)
                                {
                                    writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Minimum_" + DB.RowFieldGUID(shiprow, "RowGUID") + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "MinimumCharge")) + "\">\n");
                                    writer.Write("<input type=\"hidden\" name=\"Minimum_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_vldt\" value=\"[number][blankalert=Please enter Minimum shipping cost for this order range, in xx.xx format][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                }
                                else
                                {
                                    writer.Write(Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "MinimumCharge")));
                                }
                                writer.Write("</td>\n");

                                writer.Write("<td align=\"center\">\n");
                                if (EditRow)
                                {
                                    writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Base_" + DB.RowFieldGUID(shiprow, "RowGUID") + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "SurCharge")) + "\">\n");
                                    writer.Write("<input type=\"hidden\" name=\"Base_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_vldt\" value=\"[number][blankalert=Please enter base shipping cost for this order range, in xx.xx format][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                }
                                else
                                {
                                    writer.Write(Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "SurCharge")));
                                }
                                writer.Write("</td>\n");

                                foreach (DataRow row in dsTotal.Tables[0].Rows)
                                {
                                    Decimal SurCharge = System.Decimal.Zero; // not used here
                                    Decimal MinimumCharge = System.Decimal.Zero; // not used here
                                    writer.Write("<td align=\"center\">\n");
                                    if (EditRow)
                                    {
                                        writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Rate_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_" + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(Shipping.GetShipByTotalByPercentCharge(DB.RowFieldInt(row, "ShippingMethodID"), DB.RowFieldGUID(shiprow, "RowGUID"), out MinimumCharge, out SurCharge)) + "\">\n");
                                        writer.Write("<input type=\"hidden\" name=\"Rate_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_" + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "_vldt\" value=\"[number][blankalert=Please enter the shipping cost][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                    }
                                    else
                                    {
                                        writer.Write(Localization.CurrencyStringForDBWithoutExchangeRate(Shipping.GetShipByTotalByPercentCharge(DB.RowFieldInt(row, "ShippingMethodID"), DB.RowFieldGUID(shiprow, "RowGUID"), out MinimumCharge, out SurCharge)));
                                    }
                                    writer.Write("</td>\n");
                                }
                                if (EditRow)
                                {
                                    writer.Write("<td align=\"center\">");
                                    writer.Write("<input type=\"submit\" value=\"Update\" name=\"submit\">\n");
                                    writer.Write("</td>");
                                }
                                else
                                {
                                    writer.Write("<td align=\"center\"><input type=\"Button\" name=\"Edit\" value=\"Edit\" onClick=\"self.location='shipping.aspx?EditGUID=" + DB.RowFieldGUID(shiprow, "RowGUID") + "'\"></td>\n");
                                }
                                writer.Write("<td align=\"center\"><input type=\"Button\" name=\"Delete\" value=\"Delete\" onClick=\"self.location='shipping.aspx?deleteByTotalByPercentid=" + DB.RowFieldGUID(shiprow, "RowGUID") + "'\"></td>\n");
                                writer.Write("</tr>\n");
                            }
                            // add new row:
                            writer.Write("<tr>\n");
                            writer.Write("<td align=\"center\">\n");
                            writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Low_0\" \">\n");
                            writer.Write("<input type=\"hidden\" name=\"Low_0_vldt\" value=\"[number][blankalert=Please enter starting order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                            writer.Write("</td>\n");
                            writer.Write("<td align=\"center\">\n");
                            writer.Write("<input maxLength=\"10\" size=\"10\" name=\"High_0\" >\n");
                            writer.Write("<input type=\"hidden\" name=\"High_0_vldt\" value=\"[number][blankalert=Please enter ending order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                            writer.Write("</td>\n");
                            writer.Write("<td align=\"center\">\n");
                            writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Minimum_0\" >\n");
                            writer.Write("<input type=\"hidden\" name=\"Minimum_0_vldt\" value=\"[number][blankalert=Please enter minimum shipping charge for this range, in xx.xx format][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                            writer.Write("</td>\n");
                            writer.Write("<td align=\"center\">\n");
                            writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Base_0\" >\n");
                            writer.Write("<input type=\"hidden\" name=\"Base_0_vldt\" value=\"[number][blankalert=Please enter base shipping charge for this range, in xx.xx format][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                            writer.Write("</td>\n");
                            foreach (DataRow row in dsTotal.Tables[0].Rows)
                            {
                                writer.Write("<td align=\"center\">\n");
                                writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Rate_0_" + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "\">\n");
                                writer.Write("<input type=\"hidden\" name=\"Rate_0_" + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "_vldt\" value=\"[number][blankalert=Please enter the shipping cost][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                writer.Write("</td>\n");
                            }
                            writer.Write("<td align=\"center\">");
                            writer.Write("<input type=\"submit\" value=\"Add New Row\" name=\"submit\">\n");
                            writer.Write("</td>\n");
                            writer.Write("<td>&nbsp;</td>");
                            writer.Write("</tr>\n");
                            writer.Write("</table>\n");

                            ShippingRows.Dispose();
                        }
                        dsTotal.Dispose();

                        writer.Write("</form>\n");
                        break;
                    }
                case Shipping.ShippingCalculationEnum.UseFixedPrice:
                    {
                        writer.Write("<hr size=1>");
                        writer.Write("<p><b>FIXED RATE SHIPPING TABLE:</b></p>\n");

                        writer.Write("<form action=\"shipping.aspx?EditGUID=" + EditGUID + "\" method=\"post\" id=\"FixedRateForm\" name=\"FixedRateForm\" onsubmit=\"return (validateForm(this) && FixedRateForm_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                        writer.Write("<input type=\"hidden\" name=\"IsSubmitFixedRate\" value=\"true\">\n");

                        DataSet dsfixed = DB.GetDS("select * from ShippingMethod  " + DB.GetNoLock() + " where name not like " + DB.SQuote("%Real Time%") + " and IsRTShipping=0 order by DisplayOrder", false);
                        if (dsfixed.Tables[0].Rows.Count == 0)
                        {
                            writer.Write("No shipping methods are defined. <a href=\"shippingmethods.aspx\"><b>Click here</b></a> to define your shipping methods first");
                        }
                        else
                        {
                            writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"1\">\n");
                            writer.Write("<tr><td bgcolor=\"#CCFFFF\"><b>Shipping Method</b></td><td bgcolor=\"#CCFFFF\"><b>Flat Rate</b></td></tr>\n");
                            foreach (DataRow row in dsfixed.Tables[0].Rows)
                            {
                                writer.Write("<tr>\n");
                                writer.Write("<td>" + DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting) + "</td>\n");
                                writer.Write("<td>\n");
                                writer.Write("<input maxLength=\"10\" size=\"10\" name=\"FixedRate_" + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(row, "FixedRate")) + "\"> (in x.xx format)\n");
                                writer.Write("<input type=\"hidden\" name=\"FixedRate_" + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "_vldt\" value=\"[number][req][blankalert=Please enter the shipping cost][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                writer.Write("</td>\n");
                                writer.Write("</tr>\n");
                            }
                            writer.Write("<tr><td></td><td align=\"left\"><input type=\"submit\" value=\"Update\" name=\"submit\"></td></tr>\n");
                            writer.Write("</table>\n");
                        }
                        dsfixed.Dispose();

                        writer.Write("</form>\n");

                        break;
                    }
                case Shipping.ShippingCalculationEnum.AllOrdersHaveFreeShipping:
                    {
                        break;
                    }
                case Shipping.ShippingCalculationEnum.UseFixedPercentageOfTotal:
                    {
                        writer.Write("<hr size=1>");
                        writer.Write("<p><b>FIXED PERCENT OF TOTAL ORDER SHIPPING TABLE:</b></p>\n");

                        writer.Write("<form action=\"shipping.aspx?EditGUID=" + EditGUID + "\" method=\"post\" id=\"FixedPercentOfTotalForm\" name=\"FixedPercentOfTotalForm\" onsubmit=\"return (validateForm(this) && FixedPercentOfTotalForm_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                        writer.Write("<input type=\"hidden\" name=\"IsSubmitFixedPercentOfTotal\" value=\"true\">\n");

                        DataSet dsfixedp = DB.GetDS("select * from ShippingMethod  " + DB.GetNoLock() + " where name not like " + DB.SQuote("%Real Time%") + "  and IsRTShipping=0 order by DisplayOrder", false);
                        if (dsfixedp.Tables[0].Rows.Count == 0)
                        {
                            writer.Write("No shipping methods are defined. <a href=\"shippingmethods.aspx\"><b>Click here</b></a> to define your shipping methods first");
                        }
                        else
                        {
                            writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"1\">\n");
                            writer.Write("<tr><td bgcolor=\"#CCFFFF\"><b>Shipping Method</b></td><td bgcolor=\"#CCFFFF\"><b>Flat Percent Of Total Order Cost</b></td></tr>\n");
                            foreach (DataRow row in dsfixedp.Tables[0].Rows)
                            {
                                writer.Write("<tr>\n");
                                writer.Write("<td>" + DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting) + "</td>\n");
                                writer.Write("<td>\n");
                                writer.Write("<input maxLength=\"10\" size=\"10\" name=\"FixedPercentOfTotal_" + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "\" value=\"" + Localization.DecimalStringForDB(DB.RowFieldDecimal(row, "FixedPercentOfTotal")) + "\"> (in x.xx format)\n");
                                writer.Write("<input type=\"hidden\" name=\"FixedPercentOfTotal_" + DB.RowFieldInt(row, "ShippingMethodID").ToString() + "_vldt\" value=\"[number][req][blankalert=Please enter the shipping percentage][invalidalert=Please enter a shipping percentage (percent of total order) without the leading % sign]\">\n");
                                writer.Write("</td>\n");
                                writer.Write("</tr>\n");
                            }
                            writer.Write("<tr><td></td><td align=\"left\"><input type=\"submit\" value=\"Update\" name=\"submit\"></td></tr>\n");
                            writer.Write("</table>\n");
                        }
                        dsfixedp.Dispose();

                        writer.Write("</form>\n");

                        break;
                    }
                case Shipping.ShippingCalculationEnum.UseIndividualItemShippingCosts:
                    {
                        writer.Write("<p>Set Your shipping costs in each product variant.</p>");
                        break;
                    }
                case Shipping.ShippingCalculationEnum.UseRealTimeRates:
                    {
                        writer.Write("<p align=\"left\">Real Time I/F will be used for rates, based on order weights. Remember to set your AppConfig:RTShipping parameters accordingly! Current settings are shown below.<br/><br/>");
                        //writer.Write("<a href=\"rtshippingmgr.aspx\"><b>Click here</b></a> to manage the RT Shipping Providers.<br/><br/>");
                        writer.Write("<a href=\"appconfig.aspx?searchfor=RTShipping\"><b>Click here</b></a> to edit these settings.<br/><br/>");

                        IDataReader rss = DB.GetRS("Select * from appconfig  " + DB.GetNoLock() + " where name like " + DB.SQuote("RTShipping%") + " order by name");
                        while (rss.Read())
                        {
                            writer.Write(DB.RSField(rss, "Name") + "=" + DB.RSField(rss, "ConfigValue") + "<br/>");
                        }
                        writer.Write("</p>");
                        rss.Close();
                        break;
                    }
                case Shipping.ShippingCalculationEnum.CalculateShippingByWeightAndZone:
                    {
                        writer.Write("<hr size=1>");

                        writer.Write("<p><b>ACTIVE RATE TABLE FOR: CALCULATE SHIPPING BY ORDER WEIGHT BY ZONE:</b></p>\n");

                        int ShippingMethodID = CommonLogic.FormUSInt("ShippingMethodID");
                        if (ShippingMethodID == 0)
                        {
                            ShippingMethodID = CommonLogic.QueryStringUSInt("ShippingMethodID");
                        }

                        writer.Write("<form action=\"shipping.aspx\" method=\"post\" id=\"WeightByZoneSelectForm\" name=\"WeightByZoneSelectForm\" onSubmit=\"return confirm('If you have unsaved changes in your rate table below, click CANCEL and save them first!')\">\n");
                        writer.Write("<input type=\"hidden\" name=\"IsSubmitWeightByZone\" value=\"true\">\n");
                        IDataReader rssm = DB.GetRS("Select * from ShippingMethod " + DB.GetNoLock() + " where IsRTShipping=0 order by DisplayOrder");
                        writer.Write("Edit weight/zone table for shipping method: ");
                        writer.Write("<select name=\"ShippingMethodID\" id=\"ShippingMethodID\" size=\"1\">");
                        writer.Write("<option value=\"0\" " + CommonLogic.IIF(ShippingMethodID == 0, " selected ", "") + ">Select Shipping Method To Edit</option>");
                        while (rssm.Read())
                        {
                            writer.Write("<option value=\"" + DB.RSFieldInt(rssm, "ShippingMethodID").ToString() + "\" " + CommonLogic.IIF(ShippingMethodID == DB.RSFieldInt(rssm, "ShippingMethodID"), " selected ", "") + ">" + DB.RSFieldByLocale(rssm, "Name", ThisCustomer.LocaleSetting) + "</option>");
                        }
                        rssm.Close();
                        writer.Write("</select>");
                        writer.Write("&nbsp;");
                        writer.Write("<input type=\"submit\" value=\"Submit\" name=\"submit\">");
                        writer.Write("</form>");
                        if (ShippingMethodID != 0)
                        {
                            DataSet dsWeight = DB.GetDS("select * from ShippingZone " + DB.GetNoLock() + " where Deleted=0 order by DisplayOrder,Name", false);
                            if (dsWeight.Tables[0].Rows.Count == 0)
                            {
                                writer.Write("No shipping zones are defined. <a href=\"shippingzones.aspx\"><b>Click here</b></a> to define your zones");
                            }
                            else
                            {
                                writer.Write("<form action=\"shipping.aspx\" method=\"post\" id=\"WeightByZoneForm\" name=\"WeightByZoneForm\" onsubmit=\"return (validateForm(this) && WeightByZoneForm_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                                writer.Write("<input type=\"hidden\" name=\"EditGUID\" value=\"" + EditGUID + "\">");
                                writer.Write("<input type=\"hidden\" name=\"IsSubmitWeightByZone\" value=\"true\">\n");
                                writer.Write("<input type=\"hidden\" name=\"ShippingMethodID\" value=\"" + ShippingMethodID.ToString() + "\">\n");
                                writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"1\">\n");
                                writer.Write("<tr bgcolor=\"#FFFFDD\"><td colspan=2 align=\"center\"><b>Order Weight (in " + Localization.WeightUnits() + ")</b></td><td colspan=" + dsWeight.Tables[0].Rows.Count.ToString() + " align=\"center\"><b>Shipping Charge By Zone</b></td><td>&nbsp;</td><td>&nbsp;</td></tr>\n");
                                writer.Write("<tr>\n");
                                writer.Write("<td align=\"center\"><b>Low</b></td>\n");
                                writer.Write("<td align=\"center\"><b>High</b></td>\n");
                                foreach (DataRow row in dsWeight.Tables[0].Rows)
                                {
                                    writer.Write("<td align=\"center\"><b>" + DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting) + "</b></td>\n");
                                }
                                writer.Write("<td align=\"center\"><b>Edit</b></td>\n");
                                writer.Write("<td align=\"center\"><b>Delete</b></td>\n");
                                writer.Write("</tr>\n");

                                DataSet ShippingRows = DB.GetDS("select distinct RowGUID,LowValue,HighValue from ShippingWeightByZone " + DB.GetNoLock() + " where ShippingMethodID=" + ShippingMethodID.ToString() + " order by LowValue", false);
                                foreach (DataRow shiprow in ShippingRows.Tables[0].Rows)
                                {
                                    bool EditRow = (EditGUID == DB.RowFieldGUID(shiprow, "RowGUID"));
                                    writer.Write("<tr>\n");
                                    writer.Write("<td align=\"center\">\n");
                                    if (EditRow)
                                    {
                                        writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Low_" + DB.RowFieldGUID(shiprow, "RowGUID") + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "LowValue")) + "\">\n");
                                        writer.Write("<input type=\"hidden\" name=\"Low_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_vldt\" value=\"[number][blankalert=Please enter starting order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                    }
                                    else
                                    {
                                        writer.Write(Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "LowValue")));
                                    }
                                    writer.Write("</td>\n");
                                    writer.Write("<td align=\"center\">\n");
                                    if (EditRow)
                                    {
                                        writer.Write("<input maxLength=\"10\" size=\"10\" name=\"High_" + DB.RowFieldGUID(shiprow, "RowGUID") + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "HighValue")) + "\">\n");
                                        writer.Write("<input type=\"hidden\" name=\"High_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_vldt\" value=\"[number][blankalert=Please enter ending order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                    }
                                    else
                                    {
                                        writer.Write(Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "HighValue")));
                                    }
                                    writer.Write("</td>\n");
                                    foreach (DataRow row in dsWeight.Tables[0].Rows)
                                    {
                                        writer.Write("<td align=\"center\">\n");
                                        if (EditRow)
                                        {
                                            writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Rate_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_" + DB.RowFieldInt(row, "ShippingZoneID").ToString() + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(Shipping.GetShipByWeightAndZoneCharge(DB.RowFieldInt(row, "ShippingZoneID"), ShippingMethodID, DB.RowFieldGUID(shiprow, "RowGUID"))) + "\">\n");
                                            writer.Write("<input type=\"hidden\" name=\"Rate_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_" + DB.RowFieldInt(row, "ShippingZoneID").ToString() + "_vldt\" value=\"[number][blankalert=Please enter the shipping cost][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                        }
                                        else
                                        {
                                            writer.Write(Localization.CurrencyStringForDBWithoutExchangeRate(Shipping.GetShipByWeightAndZoneCharge(DB.RowFieldInt(row, "ShippingZoneID"), ShippingMethodID, DB.RowFieldGUID(shiprow, "RowGUID"))));
                                        }
                                        writer.Write("</td>\n");
                                    }
                                    if (EditRow)
                                    {
                                        writer.Write("<td align=\"center\">");
                                        writer.Write("<input type=\"submit\" value=\"Update\" name=\"submit\">\n");
                                        writer.Write("</td>");
                                    }
                                    else
                                    {
                                        writer.Write("<td align=\"center\"><input type=\"Button\" name=\"Edit\" value=\"Edit\" onClick=\"self.location='shipping.aspx?ShippingMethodID=" + ShippingMethodID.ToString() + "&EditGUID=" + DB.RowFieldGUID(shiprow, "RowGUID") + "'\"></td>\n");
                                    }
                                    writer.Write("<td align=\"center\"><input type=\"Button\" name=\"Delete\" value=\"Delete\" onClick=\"self.location='shipping.aspx?ShippingMethodID=" + ShippingMethodID.ToString() + "&DeleteWeightByZoneID=" + DB.RowFieldGUID(shiprow, "RowGUID") + "'\"></td>\n");
                                    writer.Write("</tr>\n");
                                }
                                // add new row:
                                writer.Write("<tr>\n");
                                writer.Write("<td align=\"center\">\n");
                                writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Low_0\" \">\n");
                                writer.Write("<input type=\"hidden\" name=\"Low_0_vldt\" value=\"[number][blankalert=Please enter starting order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                writer.Write("</td>\n");
                                writer.Write("<td align=\"center\">\n");
                                writer.Write("<input maxLength=\"10\" size=\"10\" name=\"High_0\" >\n");
                                writer.Write("<input type=\"hidden\" name=\"High_0_vldt\" value=\"[number][blankalert=Please enter ending order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                writer.Write("</td>\n");
                                foreach (DataRow row in dsWeight.Tables[0].Rows)
                                {
                                    writer.Write("<td align=\"center\">\n");
                                    writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Rate_0_" + DB.RowFieldInt(row, "ShippingZoneID").ToString() + "\">\n");
                                    writer.Write("<input type=\"hidden\" name=\"Rate_0_" + DB.RowFieldInt(row, "ShippingZoneID").ToString() + "_vldt\" value=\"[number][blankalert=Please enter the shipping cost][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                    writer.Write("</td>\n");
                                }
                                writer.Write("<td align=\"center\">");
                                writer.Write("<input type=\"submit\" value=\"Add New Row\" name=\"submit\">\n");
                                writer.Write("</td>\n");
                                writer.Write("<td>&nbsp;</td>");
                                writer.Write("</tr>\n");
                                writer.Write("</table>\n");

                                ShippingRows.Dispose();
                            }
                            dsWeight.Dispose();
                        }

                        writer.Write("</form>\n");
                        break;
                    }
                case Shipping.ShippingCalculationEnum.CalculateShippingByTotalAndZone:
                    {
                        writer.Write("<hr size=1>");

                        writer.Write("<p><b>ACTIVE RATE TABLE FOR: CALCULATE SHIPPING BY ORDER TOTAL BY ZONE:</b></p>\n");

                        int ShippingMethodID = CommonLogic.FormUSInt("ShippingMethodID");
                        if (ShippingMethodID == 0)
                        {
                            ShippingMethodID = CommonLogic.QueryStringUSInt("ShippingMethodID");
                        }

                        writer.Write("<form action=\"shipping.aspx\" method=\"post\" id=\"TotalByZoneSelectForm\" name=\"TotalByZoneSelectForm\" onSubmit=\"return confirm('If you have unsaved changes in your rate table below, click CANCEL and save them first!')\">\n");
                        writer.Write("<input type=\"hidden\" name=\"IsSubmitTotalByZone\" value=\"true\">\n");
                        IDataReader rssm = DB.GetRS("Select * from ShippingMethod " + DB.GetNoLock() + " where IsRTShipping=0 order by DisplayOrder");
                        writer.Write("Edit total/zone table for shipping method: ");
                        writer.Write("<select name=\"ShippingMethodID\" id=\"ShippingMethodID\" size=\"1\">");
                        writer.Write("<option value=\"0\" " + CommonLogic.IIF(ShippingMethodID == 0, " selected ", "") + ">Select Shipping Method To Edit</option>");
                        while (rssm.Read())
                        {
                            writer.Write("<option value=\"" + DB.RSFieldInt(rssm, "ShippingMethodID").ToString() + "\" " + CommonLogic.IIF(ShippingMethodID == DB.RSFieldInt(rssm, "ShippingMethodID"), " selected ", "") + ">" + DB.RSFieldByLocale(rssm, "Name", ThisCustomer.LocaleSetting) + "</option>");
                        }
                        rssm.Close();
                        writer.Write("</select>");
                        writer.Write("&nbsp;");
                        writer.Write("<input type=\"submit\" value=\"Submit\" name=\"submit\">");
                        writer.Write("</form>");
                        if (ShippingMethodID != 0)
                        {
                            DataSet dsTotal = DB.GetDS("select * from ShippingZone " + DB.GetNoLock() + " where deleted=0 order by DisplayOrder,Name", false);
                            if (dsTotal.Tables[0].Rows.Count == 0)
                            {
                                writer.Write("No shipping zones are defined. <a href=\"shippingzones.aspx\"><b>Click here</b></a> to define your zones");
                            }
                            else
                            {
                                writer.Write("<form action=\"shipping.aspx\" method=\"post\" id=\"TotalByZoneForm\" name=\"TotalByZoneForm\" onsubmit=\"return (validateForm(this) && TotalByZoneForm_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                                writer.Write("<input type=\"hidden\" name=\"EditGUID\" value=\"" + EditGUID + "\">");
                                writer.Write("<input type=\"hidden\" name=\"IsSubmitTotalByZone\" value=\"true\">\n");
                                writer.Write("<input type=\"hidden\" name=\"ShippingMethodID\" value=\"" + ShippingMethodID.ToString() + "\">\n");
                                writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"1\">\n");
                                writer.Write("<tr bgcolor=\"#FFFFDD\"><td colspan=2 align=\"center\"><b>Order Total</b></td><td colspan=" + dsTotal.Tables[0].Rows.Count.ToString() + " align=\"center\"><b>Shipping Charge By Zone</b></td><td>&nbsp;</td><td>&nbsp;</td></tr>\n");
                                writer.Write("<tr>\n");
                                writer.Write("<td align=\"center\"><b>Low</b></td>\n");
                                writer.Write("<td align=\"center\"><b>High</b></td>\n");
                                foreach (DataRow row in dsTotal.Tables[0].Rows)
                                {
                                    writer.Write("<td align=\"center\"><b>" + DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting) + "</b></td>\n");
                                }
                                writer.Write("<td align=\"center\"><b>Edit</b></td>\n");
                                writer.Write("<td align=\"center\"><b>Delete</b></td>\n");
                                writer.Write("</tr>\n");

                                DataSet ShippingRows = DB.GetDS("select distinct RowGUID,LowValue,HighValue from ShippingTotalByZone " + DB.GetNoLock() + " where ShippingMethodID=" + ShippingMethodID.ToString() + " order by LowValue", false);
                                foreach (DataRow shiprow in ShippingRows.Tables[0].Rows)
                                {
                                    bool EditRow = (EditGUID == DB.RowFieldGUID(shiprow, "RowGUID"));
                                    writer.Write("<tr>\n");
                                    writer.Write("<td align=\"center\">\n");
                                    if (EditRow)
                                    {
                                        writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Low_" + DB.RowFieldGUID(shiprow, "RowGUID") + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "LowValue")) + "\">\n");
                                        writer.Write("<input type=\"hidden\" name=\"Low_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_vldt\" value=\"[number][blankalert=Please enter starting order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                    }
                                    else
                                    {
                                        writer.Write(Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "LowValue")));
                                    }
                                    writer.Write("</td>\n");
                                    writer.Write("<td align=\"center\">\n");
                                    if (EditRow)
                                    {
                                        writer.Write("<input maxLength=\"10\" size=\"10\" name=\"High_" + DB.RowFieldGUID(shiprow, "RowGUID") + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "HighValue")) + "\">\n");
                                        writer.Write("<input type=\"hidden\" name=\"High_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_vldt\" value=\"[number][blankalert=Please enter ending order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                    }
                                    else
                                    {
                                        writer.Write(Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(shiprow, "HighValue")));
                                    }
                                    writer.Write("</td>\n");
                                    foreach (DataRow row in dsTotal.Tables[0].Rows)
                                    {
                                        writer.Write("<td align=\"center\">\n");
                                        if (EditRow)
                                        {
                                            writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Rate_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_" + DB.RowFieldInt(row, "ShippingZoneID").ToString() + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(Shipping.GetShipByTotalAndZoneCharge(DB.RowFieldInt(row, "ShippingZoneID"), ShippingMethodID, DB.RowFieldGUID(shiprow, "RowGUID"))) + "\">\n");
                                            writer.Write("<input type=\"hidden\" name=\"Rate_" + DB.RowFieldGUID(shiprow, "RowGUID") + "_" + DB.RowFieldInt(row, "ShippingZoneID").ToString() + "_vldt\" value=\"[number][blankalert=Please enter the shipping cost][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                        }
                                        else
                                        {
                                            writer.Write(Localization.CurrencyStringForDBWithoutExchangeRate(Shipping.GetShipByTotalAndZoneCharge(DB.RowFieldInt(row, "ShippingZoneID"), ShippingMethodID, DB.RowFieldGUID(shiprow, "RowGUID"))));
                                        }
                                        writer.Write("</td>\n");
                                    }
                                    if (EditRow)
                                    {
                                        writer.Write("<td align=\"center\">");
                                        writer.Write("<input type=\"submit\" value=\"Update\" name=\"submit\">\n");
                                        writer.Write("</td>");
                                    }
                                    else
                                    {
                                        writer.Write("<td align=\"center\"><input type=\"Button\" name=\"Edit\" value=\"Edit\" onClick=\"self.location='shipping.aspx?ShippingMethodID=" + ShippingMethodID.ToString() + "&EditGUID=" + DB.RowFieldGUID(shiprow, "RowGUID") + "'\"></td>\n");
                                    }
                                    writer.Write("<td align=\"center\"><input type=\"Button\" name=\"Delete\" value=\"Delete\" onClick=\"self.location='shipping.aspx?ShippingMethodID=" + ShippingMethodID.ToString() + "&DeleteTotalByZoneID=" + DB.RowFieldGUID(shiprow, "RowGUID") + "'\"></td>\n");
                                    writer.Write("</tr>\n");
                                }
                                // add new row:
                                writer.Write("<tr>\n");
                                writer.Write("<td align=\"center\">\n");
                                writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Low_0\" \">\n");
                                writer.Write("<input type=\"hidden\" name=\"Low_0_vldt\" value=\"[number][blankalert=Please enter starting order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                writer.Write("</td>\n");
                                writer.Write("<td align=\"center\">\n");
                                writer.Write("<input maxLength=\"10\" size=\"10\" name=\"High_0\" >\n");
                                writer.Write("<input type=\"hidden\" name=\"High_0_vldt\" value=\"[number][blankalert=Please enter ending order amount][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                writer.Write("</td>\n");
                                foreach (DataRow row in dsTotal.Tables[0].Rows)
                                {
                                    writer.Write("<td align=\"center\">\n");
                                    writer.Write("<input maxLength=\"10\" size=\"10\" name=\"Rate_0_" + DB.RowFieldInt(row, "ShippingZoneID").ToString() + "\">\n");
                                    writer.Write("<input type=\"hidden\" name=\"Rate_0_" + DB.RowFieldInt(row, "ShippingZoneID").ToString() + "_vldt\" value=\"[number][blankalert=Please enter the shipping cost][invalidalert=Please enter a money value, WITHOUT the dollar sign]\">\n");
                                    writer.Write("</td>\n");
                                }
                                writer.Write("<td align=\"center\">");
                                writer.Write("<input type=\"submit\" value=\"Add New Row\" name=\"submit\">\n");
                                writer.Write("</td>\n");
                                writer.Write("<td>&nbsp;</td>");
                                writer.Write("</tr>\n");
                                writer.Write("</table>\n");

                                ShippingRows.Dispose();
                            }
                            dsTotal.Dispose();
                        }

                        writer.Write("</form>\n");
                        break;
                    }
            }
        }

    }
}
