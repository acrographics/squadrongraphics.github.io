// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/rpt_orders.aspx.cs 7     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for rpt_orders.
    /// </summary>
    public partial class rpt_orders : AspDotNetStorefront.SkinBase
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            SectionTitle = "Reports - Orders";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            String StartDate = CommonLogic.QueryStringCanBeDangerousContent("StartDate");
            String EndDate = CommonLogic.QueryStringCanBeDangerousContent("EndDate");
            String AffiliateID = CommonLogic.QueryStringCanBeDangerousContent("AffiliateID");
            String Gender = CommonLogic.QueryStringCanBeDangerousContent("Gender");
            String GroupBy = CommonLogic.QueryStringCanBeDangerousContent("GroupBy");
            String CouponCode = CommonLogic.QueryStringCanBeDangerousContent("CouponCode");
            String TransactionState = CommonLogic.QueryStringCanBeDangerousContent("TransactionState");
            String TransactionType = CommonLogic.QueryStringCanBeDangerousContent("TransactionType");
            String ProductMatch = CommonLogic.QueryStringCanBeDangerousContent("ProductMatch");
            String ShippingState = CommonLogic.QueryStringCanBeDangerousContent("ShippingState");
            String EasyRange = CommonLogic.QueryStringCanBeDangerousContent("EasyRange");
            String Day = CommonLogic.QueryStringCanBeDangerousContent("Day");
            String Month = CommonLogic.QueryStringCanBeDangerousContent("Month");
            String Year = CommonLogic.QueryStringCanBeDangerousContent("Year");
            //String ShowType = CommonLogic.QueryStringCanBeDangerousContent("ShowType");
            String ReportType = CommonLogic.QueryStringCanBeDangerousContent("ReportType");

            if (StartDate.Length == 0)
            {
                IDataReader rsd = DB.GetRS("Select min(OrderDate) as MinDate from orders " + DB.GetNoLock());
                DateTime MinOrderDate = Localization.ParseUSDateTime("1/1/1990");
                if (rsd.Read())
                {
                    StartDate = Localization.ToNativeShortDateString(DB.RSFieldDateTime(rsd, "MinDate"));
                }
                rsd.Close();
            }
            if (EndDate.Length == 0)
            {
                EndDate = Localization.ToNativeShortDateString(System.DateTime.Now);
            }

            if (EasyRange.Length == 0)
            {
                EasyRange = "Today";
            }
            if (GroupBy.Length == 0)
            {
                GroupBy = "Day";
            }
            //if(ShowType.Length == 0)
            //{
            //	ShowType = "NumberOf";
            //}
            if (ReportType.Length == 0)
            {
                ReportType = "Table";
            }

            // make sure group by matches easyrange:
            switch (EasyRange)
            {
                case "UseDatesAbove":
                    // all options ok
                    break;
                case "UseDatesBelow":
                    // all options ok
                    break;
                case "Today":
                    GroupBy = "Day";
                    break;
                case "Yesterday":
                    GroupBy = "Day";
                    break;
                case "ThisWeek":
                    GroupBy = "Day";
                    break;
                case "LastWeek":
                    GroupBy = "Day";
                    break;
                case "ThisMonth":
                    if (GroupBy == "Year")
                    {
                        GroupBy = "Day";
                    }
                    break;
                case "LastMonth":
                    if (GroupBy == "Year")
                    {
                        GroupBy = "Day";
                    }
                    break;
                case "ThisYear":
                    break;
                case "LastYear":
                    break;
            }

            writer.Write("  <!-- calendar stylesheet -->\n");
            writer.Write("  <link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"jscalendar/calendar-win2k-cold-1.css\" title=\"win2k-cold-1\" />\n");
            writer.Write("\n");
            writer.Write("  <!-- main calendar program -->\n");
            writer.Write("  <script type=\"text/javascript\" src=\"jscalendar/calendar.js\"></script>\n");
            writer.Write("\n");
            writer.Write("  <!-- language for the calendar -->\n");
            writer.Write("  <script type=\"text/javascript\" src=\"jscalendar/lang/" + Localization.JSCalendarLanguageFile() + "\"></script>\n");
            writer.Write("\n");
            writer.Write("  <!-- the following script defines the Calendar.setup helper function, which makes\n");
            writer.Write("       adding a calendar a matter of 1 or 2 lines of code. -->\n");
            writer.Write("  <script type=\"text/javascript\" src=\"jscalendar/calendar-setup.js\"></script>\n");

            writer.Write("<script type=\"text/javascript\">\n");
            writer.Write("function ReportForm_Validator(theForm)\n");
            writer.Write("{\n");
            writer.Write("submitonce(theForm);\n");
            writer.Write("return (true);\n");
            writer.Write("}\n");
            writer.Write("</script>\n");

            writer.Write("<form method=\"GET\" action=\"rpt_orders.aspx\" id=\"ReportForm\" name=\"ReportForm\" onsubmit=\"return (validateForm(this) && ReportForm_Validator(this))\">");
            writer.Write("  <table border=\"1\" cellpadding=\"1\" cellspacing=\"0\" width=\"100%\">");
            writer.Write("    <tr>");
            writer.Write("      <td width=\"25%\" align=\"center\" bgcolor=\"#EAEAEA\"><b><font color=\"#000000\">Date Range:</font></b></td>");
            writer.Write("      <td width=\"25%\" align=\"center\" bgcolor=\"#EAEAEA\"><b><font color=\"#000000\">Order Qualifiers:</font></b></td>");
            writer.Write("      <td width=\"25%\" align=\"center\" bgcolor=\"#EAEAEA\"><b><font color=\"#000000\">Report Type:</font></b></td>");
            writer.Write("    </tr>");
            writer.Write("    <tr>");
            writer.Write("      <td width=\"25%\" valign=\"top\" align=\"left\" bgcolor=\"#FFFFCC\">");
            writer.Write("          <table border=\"0\" cellpadding=\"4\" cellspacing=\"0\" width=\"100%\">");
            writer.Write("            <tr>");
            writer.Write("              <td width=\"50%\">Start Date:</td>");
            writer.Write("              <td width=\"50%\"><input type=\"text\" name=\"StartDate\" size=\"11\" value=\"" + StartDate + "\">&nbsp;<button id=\"f_trigger_s\">...</button>");
            //writer.Write("                	<input type=\"hidden\" name=\"StartDate_vldt\" value=\"[date][invalidalert=Please enter a valid starting date in the format " + Localization.ShortDateFormat() + "]\">");
            writer.Write("</td>");
            writer.Write("            </tr>");
            writer.Write("            <tr>");
            writer.Write("              <td width=\"50%\">End Date:</td>");
            writer.Write("              <td width=\"50%\"><input type=\"text\" name=\"EndDate\" size=\"11\" value=\"" + EndDate + "\">&nbsp;<button id=\"f_trigger_e\">...</button>");
            //writer.Write("                	<input type=\"hidden\" name=\"EndDate_vldt\" value=\"[date][invalidalert=Please enter a valid ending date in the format " + Localization.ShortDateFormat() + "]\">");
            writer.Write("              </td>");
            writer.Write("            </tr>");
            writer.Write("          </table>");
            writer.Write("          <hr size=\"1\">");
            writer.Write("          <table border=\"0\" cellpadding=\"4\" cellspacing=\"0\" width=\"100%\">");
            writer.Write("            <tr>");
            writer.Write("              <td colspan=\"2\" align=\"center\" width=\"100%\"><input type=\"radio\" value=\"UseDatesAbove\" name=\"EasyRange\" " + CommonLogic.IIF(EasyRange == "UseDatesAbove", "checked", "") + ">Use Dates Above</td>");
            writer.Write("            </tr>");
            writer.Write("            <tr>");
            writer.Write("              <td width=\"50%\"><input type=\"radio\" value=\"Today\" name=\"EasyRange\" " + CommonLogic.IIF(EasyRange == "Today" || EasyRange == "", "checked", "") + ">Today</td>");
            writer.Write("              <td width=\"50%\"><input type=\"radio\" value=\"Yesterday\" name=\"EasyRange\" " + CommonLogic.IIF(EasyRange == "Yesterday", "checked", "") + ">Yesterday</td>");
            writer.Write("            </tr>");
            writer.Write("            <tr>");
            writer.Write("              <td width=\"50%\"><input type=\"radio\" value=\"ThisWeek\" name=\"EasyRange\" " + CommonLogic.IIF(EasyRange == "ThisWeek", "checked", "") + ">This Week</td>");
            writer.Write("              <td width=\"50%\"><input type=\"radio\" value=\"LastWeek\" name=\"EasyRange\" " + CommonLogic.IIF(EasyRange == "LastWeek", "checked", "") + ">Last Week</td>");
            writer.Write("            </tr>");
            writer.Write("            <tr>");
            writer.Write("              <td width=\"50%\"><input type=\"radio\" value=\"ThisMonth\" name=\"EasyRange\" " + CommonLogic.IIF(EasyRange == "ThisMonth", "checked", "") + ">This Month</td>");
            writer.Write("              <td width=\"50%\"><input type=\"radio\" value=\"LastMonth\" name=\"EasyRange\" " + CommonLogic.IIF(EasyRange == "LastMonth", "checked", "") + ">Last Month</td>");
            writer.Write("            </tr>");
            writer.Write("            <tr>");
            writer.Write("              <td width=\"50%\"><input type=\"radio\" value=\"ThisYear\" name=\"EasyRange\" " + CommonLogic.IIF(EasyRange == "ThisYear", "checked", "") + ">This Year</td>");
            writer.Write("              <td width=\"50%\"><input type=\"radio\" value=\"LastYear\" name=\"EasyRange\" " + CommonLogic.IIF(EasyRange == "LastYear", "checked", "") + ">Last Year</td>");
            writer.Write("            </tr>");
            //writer.Write("            <tr>");
            //writer.Write("              <td colspan=\"2\" align=\"center\" width=\"100%\"><input type=\"radio\" value=\"UseDatesBelow\" name=\"EasyRange\" " + (EasyRange == "UseDatesBelow" , "checked" , "") + ">Use Dates Below</td>");
            //writer.Write("            </tr>");
            writer.Write("          </table>");
            writer.Write("      </td>");
            writer.Write("      <td width=\"25%\" valign=\"top\" align=\"left\" bgcolor=\"#CCFFFF\">");
            writer.Write("        <table border=\"0\" cellpadding=\"4\" cellspacing=\"0\" width=\"100%\">");
            writer.Write("          <tr>");
            writer.Write("            <td width=\"50%\">Affiliate:</td>");
            writer.Write("            <td width=\"50%\"><select size=\"1\" name=\"AffiliateID\">");
            writer.Write("                  <option value=\"-\" " + CommonLogic.IIF(AffiliateID == "" || AffiliateID == "-", "selected", "") + ">-</option>");
            IDataReader rs = DB.GetRS("select * from affiliate  " + DB.GetNoLock() + " where deleted in (0,1) order by displayorder,name");
            while (rs.Read())
            {
                writer.Write("<option value=\"" + DB.RSFieldInt(rs, "AffiliateID").ToString() + "\"" + CommonLogic.IIF(AffiliateID == DB.RSFieldInt(rs, "AffiliateID").ToString(), "selected", "") + ">" + DB.RSField(rs, "Name") + "</option>");
            }
            rs.Close();
            writer.Write("              </select></td>");
            writer.Write("          </tr>");
            writer.Write("          <tr>");
            writer.Write("            <td width=\"50%\">Gender:</td>");
            writer.Write("            <td width=\"50%\"><select size=\"1\" name=\"Gender\">");
            writer.Write("                  <option value=\"-\" " + CommonLogic.IIF(Gender == "" || Gender == "-", "selected", "") + ">-</option>");
            writer.Write("                <option value=\"M\"" + CommonLogic.IIF(Gender == "M", "selected", "") + ">Male</option>");
            writer.Write("                <option value=\"F\"" + CommonLogic.IIF(Gender == "F", "selected", "") + ">Female</option>");
            writer.Write("              </select></td>");
            writer.Write("          </tr>");
            writer.Write("          <tr>");
            writer.Write("            <td width=\"50%\">Coupon Code:</td>");
            writer.Write("            <td width=\"50%\"><select size=\"1\" name=\"CouponCode\">");
            writer.Write("                  <option value=\"-\" " + CommonLogic.IIF(CouponCode == "" || CouponCode == "-", "selected", "") + ">-</option>");
            rs = DB.GetRS("select * from Coupon  " + DB.GetNoLock() + " order by CouponCode");
            while (rs.Read())
            {
                writer.Write("<option value=\"" + DB.RSField(rs, "CouponCode").Replace("\"", "").Replace("'", "") + "\"" + CommonLogic.IIF(CouponCode == DB.RSField(rs, "CouponCode"), "selected", "") + ">" + Server.HtmlEncode(DB.RSField(rs, "CouponCode")) + "</option>");
            }
            rs.Close();
            writer.Write("              </select></td>");
            writer.Write("          </tr>");

            writer.Write("          <tr>");
            writer.Write("            <td width=\"50%\">Ship To State:</td>");
            writer.Write("            <td width=\"50%\">");

            writer.Write("<select size=\"1\" name=\"ShippingState\">");
            writer.Write("<option value=\"-\">-</option>");

            DataSet dsstate = DB.GetDS("select * from state " + DB.GetNoLock() + " order by DisplayOrder,Name", AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
            foreach (DataRow row in dsstate.Tables[0].Rows)
            {
                writer.Write("<option value=\"" + DB.RowField(row, "Abbreviation") + "\"" + CommonLogic.IIF(DB.RowField(row, "Abbreviation") == ShippingState, " selected", String.Empty) + ">" + DB.RowField(row, "Name") + "</option>");
            }
            dsstate.Dispose();
            writer.Write("</select>");

            writer.Write("              </td>");
            writer.Write("          </tr>");

            writer.Write("          <tr>");
            writer.Write("            <td width=\"50%\">Transaction State:</td>");
            writer.Write("            <td width=\"50%\"><select size=\"1\" name=\"TransactionState\">");
            writer.Write("                <option value=\"-\">ALL</option>");
            writer.Write("                <option value=\"" + AppLogic.ro_TXStateAuthorized + "\" " + CommonLogic.IIF(TransactionState == AppLogic.ro_TXStateAuthorized, "selected", "") + ">" + AppLogic.ro_TXStateAuthorized + "</option>");
            writer.Write("                <option value=\"" + AppLogic.ro_TXStateCaptured + "\"" + CommonLogic.IIF(TransactionState == "" || TransactionState == "-" || TransactionState == AppLogic.ro_TXStateCaptured, "selected", "") + ">" + AppLogic.ro_TXStateCaptured + "</option>");
            writer.Write("                <option value=\"" + AppLogic.ro_TXStateVoided + "\"" + CommonLogic.IIF(TransactionState == AppLogic.ro_TXStateVoided, "selected", "") + ">" + AppLogic.ro_TXStateVoided + "</option>");
            writer.Write("                <option value=\"" + AppLogic.ro_TXStateRefunded + "\"" + CommonLogic.IIF(TransactionState == AppLogic.ro_TXStateRefunded, "selected", "") + ">" + AppLogic.ro_TXStateRefunded + "</option>");
            writer.Write("                <option value=\"" + AppLogic.ro_TXStateFraud + "\"" + CommonLogic.IIF(TransactionState == AppLogic.ro_TXStateFraud, "selected", "") + ">" + AppLogic.ro_TXStateFraud + "</option>");
            writer.Write("              </select></td>");
            writer.Write("          </tr>");

            writer.Write("          <tr>");
            writer.Write("            <td width=\"50%\">Transaction Type:</td>");
            writer.Write("            <td width=\"50%\"><select size=\"1\" name=\"TransactionType\">");
            writer.Write("                <option value=\"-\">ALL</option>");
            writer.Write("                <option value=\"UNKNOWN\" " + CommonLogic.IIF(TransactionState == "UNKNOWN", "selected", "") + ">UNKNOWN</option>");
            writer.Write("                <option value=\"CHARGE\" " + CommonLogic.IIF(TransactionState == "CHARGE", "selected", "") + ">CHARGE</option>");
            writer.Write("                <option value=\"CREDIT\" " + CommonLogic.IIF(TransactionState == "CREDIT", "selected", "") + ">CREDIT</option>");
            writer.Write("                <option value=\"RECURRING_AUTO\" " + CommonLogic.IIF(TransactionState == "RECURRING_AUTO", "selected", "") + ">RECURRING_BILLING</option>");
            writer.Write("              </select></td>");
            writer.Write("          </tr>");

            if (AppLogic.NumProductsInDB < 250)
            {
                writer.Write("          <tr>");
                writer.Write("            <td width=\"50%\">Product Match:</td>");
                writer.Write("            <td width=\"50%\"><select size=\"1\" name=\"ProductMatch\">");
                writer.Write("                <option value=\"-\">ALL</option>");
                IDataReader rsx = DB.GetRS("select ProductID,Name from Product with (NOLOCK) order by Name,ProductID");
                while (rsx.Read())
                {
                    writer.Write("<option value=\"" + DB.RSFieldInt(rsx, "ProductID").ToString() + "\" " + CommonLogic.IIF(ProductMatch == DB.RSFieldInt(rsx, "ProductID").ToString(), "selected", "") + ">" + DB.RSField(rsx, "Name") + "</option>");
                }
                rsx.Close();
                writer.Write("              </select></td>");
                writer.Write("          </tr>");
            }
            
            writer.Write("        </table>");
            writer.Write("        </td>");
            writer.Write("      <td width=\"25%\" valign=\"top\" align=\"left\" bgcolor=\"#CCCCFF\">");
            writer.Write("        <table border=\"0\" cellpadding=\"4\" cellspacing=\"0\" width=\"100%\">");
            writer.Write("          <tr>");
            writer.Write("            <td width=\"100%\"><b>Group Data By:</b><br/> <input type=\"radio\" value=\"Day\" name=\"GroupBy\"  " + CommonLogic.IIF(GroupBy == "Day" || GroupBy == "", "checked", "") + ">Day</td>");
            writer.Write("          </tr>");
            writer.Write("          <tr>");
            writer.Write("            <td width=\"100%\"> <input type=\"radio\" value=\"Month\" name=\"GroupBy\" " + CommonLogic.IIF(GroupBy == "Month", "checked", "") + ">Month</td>");
            writer.Write("          </tr>");
            writer.Write("          <tr>");
            writer.Write("            <td width=\"100%\"> <input type=\"radio\" value=\"Year\" name=\"GroupBy\" " + CommonLogic.IIF(GroupBy == "Year", "checked", "") + ">Year</td>");
            writer.Write("          </tr>");
            writer.Write("        </table>");

            writer.Write("        </td>");
            writer.Write("    </tr>");
            writer.Write("    <tr>");
            writer.Write("      <td width=\"100%\" valign=\"top\" align=\"center\" bgcolor=\"#EAEAEA\" colspan=\"3\">");
            writer.Write("        <input type=\"submit\" value=\"Submit\" name=\"B1\"><input type=\"button\" onClick=\"javascript:self.location='rpt_orders.aspx';\" value=\"Reset\" name=\"B2\">");
            writer.Write("      </td>");
            writer.Write("    </tr>");
            writer.Write("  </table>");
            writer.Write("</form>");

            writer.Write("\n<script type=\"text/javascript\">\n");
            writer.Write("    Calendar.setup({\n");
            writer.Write("        inputField     :    \"StartDate\",      // id of the input field\n");
            writer.Write("        ifFormat       :    \"" + Localization.JSCalendarDateFormatSpec() + "\",       // format of the input field\n");
            writer.Write("        showsTime      :    false,            // will display a time selector\n");
            writer.Write("        button         :    \"f_trigger_s\",   // trigger for the calendar (button ID)\n");
            writer.Write("        singleClick    :    true            // Single-click mode\n");
            writer.Write("    });\n");
            writer.Write("    Calendar.setup({\n");
            writer.Write("        inputField     :    \"EndDate\",      // id of the input field\n");
            writer.Write("        ifFormat       :    \"" + Localization.JSCalendarDateFormatSpec() + "\",       // format of the input field\n");
            writer.Write("        showsTime      :    false,            // will display a time selector\n");
            writer.Write("        button         :    \"f_trigger_e\",   // trigger for the calendar (button ID)\n");
            writer.Write("        singleClick    :    true            // Single-click mode\n");
            writer.Write("    });\n");
            writer.Write("</script>\n");

            DateTime RangeStartDate = System.DateTime.MinValue;
            DateTime RangeEndDate = System.DateTime.MaxValue;

            String DateWhere = String.Empty;
            switch (EasyRange)
            {
                case "UseDatesAbove":
                    if (StartDate.Length != 0)
                    {
                        DateTime dt = Localization.ParseNativeDateTime(StartDate + " 12:00:00.000 AM");
                        DateWhere = " OrderDate>=" + DB.DateQuote(Localization.ToDBDateTimeString(dt));
                        RangeStartDate = Localization.ParseNativeDateTime(StartDate);
                    }
                    else
                    {
                        RangeStartDate = System.DateTime.MinValue; // will get min date returned from either query
                    }
                    if (EndDate.Length != 0)
                    {
                        DateTime dt = Localization.ParseNativeDateTime(EndDate + " 11:59:59.999 PM");
                        DateWhere += CommonLogic.IIF(DateWhere.Length != 0, " and ", "") + "OrderDate <=" + DB.DateQuote(Localization.ToDBDateTimeString(dt));
                        RangeEndDate = Localization.ParseNativeDateTime(EndDate);
                    }
                    else
                    {
                        RangeEndDate = System.DateTime.Now;
                    }
                    break;
                case "UseDatesBelow":
                    if (Day.Length != 0 && Day != "0")
                    {
                        DateWhere = " day(OrderDate)=" + Day + " ";
                    }
                    else
                    {
                        GroupBy = "Month";
                    }
                    if (Month.Length != 0 && Month != "0")
                    {
                        if (DateWhere.Length != 0)
                        {
                            DateWhere += " and ";
                        }
                        DateWhere += " month(OrderDate)=" + Month + " ";
                    }
                    else
                    {
                        GroupBy = "Year";
                    }
                    if (Year.Length != 0 && Year != "0")
                    {
                        if (DateWhere.Length != 0)
                        {
                            DateWhere += " and ";
                        }
                        DateWhere += " year(OrderDate)=" + Year + " ";
                    };
                    String DaySpec = CommonLogic.IIF(Day.Length == 0 || Day == "0", "1", Day);
                    String MonthSpec = CommonLogic.IIF(Month.Length == 0 || Month == "0", "1", Month);
                    String YearSpec = CommonLogic.IIF(Year.Length == 0 || Year == "0", System.DateTime.Now.Year.ToString(), Year);
                    RangeStartDate = Localization.ParseNativeDateTime(MonthSpec + "/" + DaySpec + "/" + YearSpec);
                    RangeEndDate = RangeStartDate;
                    break;
                case "Today":
                    DateWhere = "day(OrderDate)=" + System.DateTime.Now.Day.ToString() + " and month(OrderDate)=" + System.DateTime.Now.Month.ToString() + " and year(OrderDate)=" + System.DateTime.Now.Year.ToString();
                    RangeStartDate = System.DateTime.Now;
                    RangeEndDate = System.DateTime.Now;
                    break;
                case "Yesterday":
                    DateWhere = "day(OrderDate)=" + System.DateTime.Now.AddDays(-1).Day.ToString() + " and month(OrderDate)=" + System.DateTime.Now.AddDays(-1).Month.ToString() + " and year(OrderDate)=" + System.DateTime.Now.AddDays(-1).Year.ToString();
                    RangeStartDate = System.DateTime.Now.AddDays(-1);
                    RangeEndDate = System.DateTime.Now.AddDays(-1);
                    break;
                case "ThisWeek":
                    int DayOfWeek = (int)System.DateTime.Now.DayOfWeek;
                    System.DateTime weekstart = System.DateTime.Now.AddDays(-(DayOfWeek));
                    System.DateTime weekend = weekstart.AddDays(6);
                    int weekstartday = weekstart.DayOfYear;
                    int weekendday = weekend.DayOfYear;
                    DateWhere = "year(OrderDate)=" + System.DateTime.Now.Year.ToString() + " and (datepart(\"dy\",OrderDate)>=" + weekstartday.ToString() + " and datepart(\"dy\",OrderDate)<=" + weekendday.ToString() + ")";
                    RangeStartDate = weekstart;
                    RangeEndDate = weekend;
                    break;
                case "LastWeek":
                    int DayOfWeek2 = (int)System.DateTime.Now.DayOfWeek;
                    System.DateTime weekstart2 = System.DateTime.Now.AddDays(-(DayOfWeek2)).AddDays(-7);
                    System.DateTime weekend2 = weekstart2.AddDays(6);
                    int weekstartday2 = weekstart2.DayOfYear;
                    int weekendday2 = weekend2.DayOfYear;
                    DateWhere = "year(OrderDate)=" + System.DateTime.Now.Year.ToString() + " and (datepart(\"dy\",OrderDate)>=" + weekstartday2.ToString() + " and datepart(\"dy\",OrderDate)<=" + weekendday2.ToString() + ")";
                    RangeStartDate = weekstart2;
                    RangeEndDate = weekend2;
                    break;
                case "ThisMonth":
                    DateWhere = "month(OrderDate)=" + System.DateTime.Now.Month.ToString() + " and year(OrderDate)=" + System.DateTime.Now.Year.ToString();
                    RangeStartDate = Localization.ParseNativeDateTime(System.DateTime.Now.Month.ToString() + "/1/" + System.DateTime.Now.Year.ToString());
                    RangeEndDate = RangeStartDate.AddMonths(1).AddDays(-1);
                    break;
                case "LastMonth":
                    DateWhere = "month(OrderDate)=" + System.DateTime.Now.AddMonths(-1).Month.ToString() + " and year(OrderDate)=" + System.DateTime.Now.AddMonths(-1).Year.ToString();
                    RangeStartDate = Localization.ParseNativeDateTime(System.DateTime.Now.AddMonths(-1).Month.ToString() + "/1/" + System.DateTime.Now.AddMonths(-1).Year.ToString());
                    RangeEndDate = RangeStartDate.AddMonths(1).AddDays(-1);
                    break;
                case "ThisYear":
                    DateWhere = "year(OrderDate)=" + System.DateTime.Now.Year.ToString();
                    RangeStartDate = Localization.ParseUSDateTime("1/1/" + System.DateTime.Now.Year.ToString());
                    RangeEndDate = RangeStartDate.AddYears(1).AddDays(-1);
                    if (RangeEndDate > System.DateTime.Now)
                    {
                        RangeEndDate = System.DateTime.Now;
                    }
                    break;
                case "LastYear":
                    DateWhere = "year(OrderDate)=" + System.DateTime.Now.AddYears(-1).Year.ToString();
                    RangeStartDate = Localization.ParseUSDateTime("1/1/" + System.DateTime.Now.AddYears(-1).Year.ToString());
                    RangeEndDate = RangeStartDate.AddYears(1).AddDays(-1);
                    break;
            }
            if (DateWhere.Length != 0)
            {
                DateWhere = "(" + DateWhere + ")";
            }

            String WhereClause = DateWhere;

            String Series1Name = String.Empty;
            String Series2Name = String.Empty;

            String SelectFields = String.Empty;
            String GroupByFields = String.Empty;
            String OrderByFields = String.Empty;
            String DateFormat = String.Empty;
            String GroupByIncrement = String.Empty;
            switch (GroupBy)
            {
                case "Day":
                    SelectFields = "datepart(\"dy\",OrderDate) as [Day], Year(OrderDate) as [Year]";
                    GroupByFields = "Year(OrderDate), datepart(\"dy\",OrderDate)";
                    OrderByFields = "Year(OrderDate) asc, datepart(\"dy\",OrderDate) asc";
                    DateFormat = "mm-dd-yyyy";
                    GroupByIncrement = "0";
                    break;
                case "Month":
                    SelectFields = "month(OrderDate) as [Month], Year(OrderDate) as [Year]";
                    GroupByFields = "Year(OrderDate), month(OrderDate)";
                    OrderByFields = "Year(OrderDate) asc, month(OrderDate) asc";
                    DateFormat = "mm-yyyy";
                    GroupByIncrement = "2";
                    break;
                case "Year":
                    SelectFields = "Year(OrderDate) as [Year]";
                    GroupByFields = "Year(OrderDate)";
                    OrderByFields = "Year(OrderDate) asc";
                    DateFormat = "yyyy";
                    GroupByIncrement = "3";
                    break;
            }

            String GeneralWhere = String.Empty;
            if (AffiliateID != "-" && AffiliateID.Length != 0)
            {
                if (GeneralWhere.Length != 0)
                {
                    GeneralWhere += " and ";
                }
                GeneralWhere += "AffiliateID=" + AffiliateID;
            }
            if (Gender != "-" && Gender.Length != 0)
            {
                if (GeneralWhere.Length != 0)
                {
                    GeneralWhere += " and ";
                }
                GeneralWhere += "customerid in (select distinct customerid from customer  " + DB.GetNoLock() + " where upper(Gender)=" + DB.SQuote(Gender.ToUpperInvariant()) + ")";
            }
            if (CouponCode != "-" && CouponCode.Length != 0)
            {
                if (GeneralWhere.Length != 0)
                {
                    GeneralWhere += " and ";
                }
                GeneralWhere += "upper(CouponCode)=" + DB.SQuote(CouponCode.ToUpperInvariant());
            }
            if (ShippingState != "-" && ShippingState.Length != 0)
            {
                if (GeneralWhere.Length != 0)
                {
                    GeneralWhere += " and ";
                }
                GeneralWhere += "upper(ShippingState)=" + DB.SQuote(ShippingState.ToUpperInvariant());
            }
            if (TransactionState.Length != 0 && TransactionState != "-")
            {
                if (GeneralWhere.Length != 0)
                {
                    GeneralWhere += " and ";
                }
                GeneralWhere += "TransactionState=" + DB.SQuote(TransactionState);
            }
            if (TransactionType.Length != 0 && TransactionType != "-")
            {
                if (GeneralWhere.Length != 0)
                {
                    GeneralWhere += " and ";
                }
                GeneralWhere += "TransactionType=" + DB.SQuote(TransactionType);
            }
            if (ProductMatch.Length != 0 && ProductMatch != "-")
            {
                if (GeneralWhere.Length != 0)
                {
                    GeneralWhere += " and ";
                }
                GeneralWhere += "OrderNumber in (select ordernumber from orders_shoppingcart where ProductID=" + ProductMatch + ")";
            }
            if (GeneralWhere.Length != 0)
            {
                GeneralWhere = "(" + GeneralWhere + ")";
            }

            if (DateWhere.Length != 0)
            {
                String DS1SQL = "select count(OrderNumber) as N, Sum(OrderSubTotal) as SubTotal, Sum(OrderTotal) as Total, Sum(OrderTax) as Tax, Sum(OrderShippingCosts) as Shipping, " + SelectFields + " from orders  " + DB.GetNoLock() + " where 1=1 " + CommonLogic.IIF(GeneralWhere.Length != 0, " and " + GeneralWhere, "") + CommonLogic.IIF(WhereClause.Length != 0, " and " + WhereClause, "") + " group by " + GroupByFields + " order by " + OrderByFields;
                if (AppLogic.AppConfigBool("Admin_ShowReportSQL"))
                {
                    writer.Write("<p align=\"left\">DS1SQL=" + DS1SQL + "</p>\n");
                }

                DataSet ds1;
                try
                {
                    ds1 = DB.GetDS(DS1SQL, false, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
                    if (ds1.Tables[0].Rows.Count == 0)
                    {
                        writer.Write("<p align=\"left\"><b>NO DATA FOUND</b></p>\n");
                    }
                    else
                    {
                        int DS1SumN = 0;
                        decimal DS1SumSubTotal = System.Decimal.Zero;
                        decimal DS1SumTotal = System.Decimal.Zero;
                        decimal DS1SumTax = System.Decimal.Zero;
                        decimal DS1SumShipping = System.Decimal.Zero;
                        int DS1NumRecs = ds1.Tables[0].Rows.Count;
                        int MaxNumRecs = DS1NumRecs;
                        foreach (DataRow row in ds1.Tables[0].Rows)
                        {
                            DS1SumN += DB.RowFieldInt(row, "N");
                            DS1SumSubTotal += DB.RowFieldDecimal(row, "SubTotal");
                            DS1SumTotal += DB.RowFieldDecimal(row, "Total");
                            DS1SumTax += DB.RowFieldDecimal(row, "Tax");
                            DS1SumShipping += DB.RowFieldDecimal(row, "Shipping");
                        }
                        // set range start date, if necessary:
                        IDataReader rsd = DB.GetRS("select min(OrderDate) as RangeStartDate from orders " + DB.GetNoLock() + " ");
                        DateTime MinCustomerDate = System.DateTime.MinValue;
                        if (rsd.Read())
                        {
                            MinCustomerDate = DB.RSFieldDateTime(rsd, "RangeStartDate");
                        }
                        else
                        {
                            MinCustomerDate = Localization.ParseUSDateTime("1/1/2003"); // we need SOME value!
                        }
                        rsd.Close();
                        if (RangeStartDate == System.DateTime.MinValue)
                        {
                            RangeStartDate = MinCustomerDate;
                        }
                        if (RangeStartDate < MinCustomerDate)
                        {
                            RangeStartDate = MinCustomerDate;
                        }
                        String DateSeries = String.Empty;

                        String DS1Dates = String.Empty;
                        String DS1ValuesN = String.Empty;
                        String DS1ValuesSubTotal = String.Empty;
                        String DS1ValuesTotal = String.Empty;
                        String DS1ValuesTax = String.Empty;
                        String DS1ValuesShipping = String.Empty;

                        bool first = true;
                        foreach (DataRow row in ds1.Tables[0].Rows)
                        {
                            if (!first)
                            {
                                DS1Dates += "|";
                            }
                            switch (GroupBy)
                            {
                                case "Day":
                                    int dy = DB.RowFieldInt(row, "Day");
                                    DateTime dt = Localization.ParseUSDateTime("1/1/" + DB.RowField(row, "Year")).AddDays(dy - 1);
                                    DS1Dates += dt.Month.ToString().PadLeft(2, '0') + '-' + dt.Day.ToString().PadLeft(2, '0') + "-" + dt.Year.ToString().PadLeft(4, '0');
                                    break;
                                case "Month":
                                    DS1Dates += DB.RowField(row, "Month").PadLeft(2, '0') + "-" + DB.RowField(row, "Year").PadLeft(4, '0');
                                    break;
                                case "Year":
                                    DS1Dates += DB.RowField(row, "Year").PadLeft(4, '0');
                                    break;
                            }
                            first = false;
                        }


                        int NumBuckets = 0;
                        // determine how many "buckets" are in the date series:
                        switch (GroupBy)
                        {
                            case "Day":
                                for (DateTime yy = RangeStartDate; yy <= RangeEndDate; yy = yy.AddDays(1))
                                {
                                    NumBuckets++;
                                }
                                break;
                            case "Month":
                                for (DateTime yy = Localization.ParseNativeDateTime(RangeStartDate.Month.ToString() + "/1/" + RangeStartDate.Year.ToString()); yy <= Localization.ParseNativeDateTime(RangeEndDate.Month.ToString() + "/1/" + RangeEndDate.Year.ToString()); yy = yy.AddMonths(1))
                                {
                                    NumBuckets++;
                                }
                                break;
                            case "Year":
                                for (DateTime yy = Localization.ParseUSDateTime("1/1/" + RangeStartDate.Year.ToString()); yy <= Localization.ParseUSDateTime("1/1/" + RangeEndDate.Year.ToString()); yy = yy.AddYears(1))
                                {
                                    NumBuckets++;
                                }
                                break;
                        }

                        // COMPOSE FULL DATE and RANGE and SUM SERIES:
                        int ds1_idx = 0;
                        int[] SumsN = new int[NumBuckets];
                        decimal[] SumsSubTotal = new decimal[NumBuckets];
                        decimal[] SumsTotal = new decimal[NumBuckets];
                        decimal[] SumsTax = new decimal[NumBuckets];
                        decimal[] SumsShipping = new decimal[NumBuckets];
                        for (int i = SumsN.GetLowerBound(0); i <= SumsN.GetUpperBound(0); i++)
                        {
                            SumsN[i] = 0;
                            SumsSubTotal[i] = System.Decimal.Zero;
                            SumsTotal[i] = System.Decimal.Zero;
                            SumsTax[i] = System.Decimal.Zero;
                            SumsShipping[i] = System.Decimal.Zero;
                        }
                        int SumBucketIdx = 0;
                        switch (GroupBy)
                        {
                            case "Day":
                                for (DateTime yy = RangeStartDate; yy <= RangeEndDate; yy = yy.AddDays(1))
                                {
                                    if (DateSeries.Length != 0)
                                    {
                                        DateSeries += "|";
                                        DS1ValuesN += "|";
                                        DS1ValuesSubTotal += "|";
                                        DS1ValuesTotal += "|";
                                        DS1ValuesTax += "|";
                                        DS1ValuesShipping += "|";
                                    }
                                    DateSeries += yy.Month.ToString() + "-" + yy.Day.ToString() + "-" + yy.Year.ToString();
                                    if (ds1_idx < DS1NumRecs)
                                    {
                                        DataRow ds1Row = ds1.Tables[0].Rows[ds1_idx];
                                        int dy1 = DB.RowFieldInt(ds1Row, "Day");
                                        DateTime dt1 = Localization.ParseUSDateTime("1/1/" + DB.RowField(ds1Row, "Year")).AddDays(dy1 - 1);
                                        if (dt1.Month == yy.Month && dt1.Day == yy.Day && dt1.Year == yy.Year)
                                        {
                                            DS1ValuesN += DB.RowFieldInt(ds1Row, "N").ToString();
                                            DS1ValuesSubTotal += Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(ds1Row, "SubTotal"));
                                            DS1ValuesTotal += Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(ds1Row, "Total"));
                                            DS1ValuesTax += Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(ds1Row, "Tax"));
                                            DS1ValuesShipping += Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(ds1Row, "Shipping"));
                                            SumsN[SumBucketIdx] += DB.RowFieldInt(ds1Row, "N");
                                            SumsSubTotal[SumBucketIdx] += DB.RowFieldDecimal(ds1Row, "SubTotal");
                                            SumsTotal[SumBucketIdx] += DB.RowFieldDecimal(ds1Row, "Total");
                                            SumsTax[SumBucketIdx] += DB.RowFieldDecimal(ds1Row, "Tax");
                                            SumsShipping[SumBucketIdx] += DB.RowFieldDecimal(ds1Row, "Shipping");
                                            ds1_idx++;
                                        }
                                        else
                                        {
                                            DS1ValuesN += "0";
                                            DS1ValuesSubTotal += "0.0";
                                            DS1ValuesTotal += "0.0";
                                            DS1ValuesTax += "0.0";
                                            DS1ValuesShipping += "0.0";
                                        }
                                    }
                                    else
                                    {
                                        DS1ValuesN += "0";
                                        DS1ValuesSubTotal += "0.0";
                                        DS1ValuesTotal += "0.0";
                                        DS1ValuesTax += "0.0";
                                        DS1ValuesShipping += "0.0";
                                    }
                                    SumBucketIdx++;
                                }
                                break;
                            case "Month":
                                for (DateTime yy = Localization.ParseNativeDateTime(RangeStartDate.Month.ToString() + "/1/" + RangeStartDate.Year.ToString()); yy <= Localization.ParseNativeDateTime(RangeEndDate.Month.ToString() + "/1/" + RangeEndDate.Year.ToString()); yy = yy.AddMonths(1))
                                {
                                    if (DateSeries.Length != 0)
                                    {
                                        DateSeries += "|";
                                        DS1ValuesN += "|";
                                        DS1ValuesSubTotal += "|";
                                        DS1ValuesTotal += "|";
                                        DS1ValuesTax += "|";
                                        DS1ValuesShipping += "|";
                                    }
                                    DateSeries += yy.Month.ToString() + "-" + yy.Year.ToString();
                                    if (ds1_idx < DS1NumRecs)
                                    {
                                        DataRow ds1Row = ds1.Tables[0].Rows[ds1_idx];
                                        DateTime dt1 = Localization.ParseNativeDateTime(DB.RowField(ds1Row, "Month").ToString() + "/1/" + DB.RowField(ds1Row, "Year"));
                                        if (dt1.Month == yy.Month && dt1.Year == yy.Year)
                                        {
                                            DS1ValuesN += DB.RowFieldInt(ds1Row, "N").ToString();
                                            DS1ValuesSubTotal += Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(ds1Row, "SubTotal"));
                                            DS1ValuesTotal += Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(ds1Row, "Total"));
                                            DS1ValuesTax += Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(ds1Row, "Tax"));
                                            DS1ValuesShipping += Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(ds1Row, "Shipping"));
                                            SumsN[SumBucketIdx] += DB.RowFieldInt(ds1Row, "N");
                                            SumsSubTotal[SumBucketIdx] += DB.RowFieldDecimal(ds1Row, "SubTotal");
                                            SumsTotal[SumBucketIdx] += DB.RowFieldDecimal(ds1Row, "Total");
                                            SumsTax[SumBucketIdx] += DB.RowFieldDecimal(ds1Row, "Tax");
                                            SumsShipping[SumBucketIdx] += DB.RowFieldDecimal(ds1Row, "Shipping");
                                            ds1_idx++;
                                        }
                                        else
                                        {
                                            DS1ValuesN += "0";
                                            DS1ValuesSubTotal += "0.0";
                                            DS1ValuesTotal += "0.0";
                                            DS1ValuesTax += "0.0";
                                            DS1ValuesShipping += "0.0";
                                        }
                                    }
                                    else
                                    {
                                        DS1ValuesN += "0";
                                        DS1ValuesSubTotal += "0.0";
                                        DS1ValuesTotal += "0.0";
                                        DS1ValuesTax += "0.0";
                                        DS1ValuesShipping += "0.0";
                                    }
                                    SumBucketIdx++;
                                }
                                break;
                            case "Year":
                                for (DateTime yy = Localization.ParseUSDateTime("1/1/" + RangeStartDate.Year.ToString()); yy <= Localization.ParseUSDateTime("1/1/" + RangeEndDate.Year.ToString()); yy = yy.AddYears(1))
                                {
                                    if (DateSeries.Length != 0)
                                    {
                                        DateSeries += "|";
                                        DS1ValuesN += "|";
                                        DS1ValuesSubTotal += "|";
                                        DS1ValuesTotal += "|";
                                        DS1ValuesTax += "|";
                                        DS1ValuesShipping += "|";
                                    }
                                    DateSeries += yy.Year.ToString();
                                    if (ds1_idx < DS1NumRecs)
                                    {
                                        DataRow ds1Row = ds1.Tables[0].Rows[ds1_idx];
                                        DateTime dt1 = Localization.ParseUSDateTime("1/1/" + DB.RowField(ds1Row, "Year"));
                                        if (dt1.Year == yy.Year)
                                        {
                                            DS1ValuesN += DB.RowFieldInt(ds1Row, "N").ToString();
                                            DS1ValuesSubTotal += Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(ds1Row, "SubTotal"));
                                            DS1ValuesTotal += Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(ds1Row, "Total"));
                                            DS1ValuesTax += Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(ds1Row, "Tax"));
                                            DS1ValuesShipping += Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(ds1Row, "Shipping"));
                                            SumsN[SumBucketIdx] += DB.RowFieldInt(ds1Row, "N");
                                            SumsSubTotal[SumBucketIdx] += DB.RowFieldDecimal(ds1Row, "SubTotal");
                                            SumsTotal[SumBucketIdx] += DB.RowFieldDecimal(ds1Row, "Total");
                                            SumsTax[SumBucketIdx] += DB.RowFieldDecimal(ds1Row, "Tax");
                                            SumsShipping[SumBucketIdx] += DB.RowFieldDecimal(ds1Row, "Shipping");
                                            ds1_idx++;
                                        }
                                        else
                                        {
                                            DS1ValuesN += "0";
                                            DS1ValuesSubTotal += "0.0";
                                            DS1ValuesTotal += "0.0";
                                            DS1ValuesTax += "0.0";
                                            DS1ValuesShipping += "0.0";
                                        }
                                    }
                                    else
                                    {
                                        DS1ValuesN += "0";
                                        DS1ValuesSubTotal += "0.0";
                                        DS1ValuesTotal += "0.0";
                                        DS1ValuesTax += "0.0";
                                        DS1ValuesShipping += "0.0";
                                    }
                                    SumBucketIdx++;
                                }
                                break;
                        }

                        writer.Write("<p align=\"left\"><b>Number of Orders: " + DS1SumN.ToString() + "</b></p>\n");
                        if (DS1SumN > 0)
                        {
                            String ReportTitle = "Orders Report|" + Localization.ToNativeShortDateString(RangeStartDate) + " - " + Localization.ToNativeShortDateString(RangeEndDate) + "|Group By: " + GroupBy;
                            String ReportTitleN = "Number Of Orders Report|" + Localization.ToNativeShortDateString(RangeStartDate) + " - " + Localization.ToNativeShortDateString(RangeEndDate) + "|Group By: " + GroupBy;
                            String ReportTitleSubTotal = "Order SubTotal Report|Sum=" + Localization.CurrencyStringForDBWithoutExchangeRate(DS1SumSubTotal) + "|" + Localization.ToNativeShortDateString(RangeStartDate) + " - " + Localization.ToNativeShortDateString(RangeEndDate) + "|Group By: " + GroupBy;
                            String ReportTitleTotal = "Order Total Report|Sum=" + Localization.CurrencyStringForDBWithoutExchangeRate(DS1SumTotal) + "|" + Localization.ToNativeShortDateString(RangeStartDate) + " - " + Localization.ToNativeShortDateString(RangeEndDate) + "|Group By: " + GroupBy;
                            String ReportTitleTax = "Tax Report|Sum=" + Localization.CurrencyStringForDBWithoutExchangeRate(DS1SumTax) + "|" + Localization.ToNativeShortDateString(RangeStartDate) + " - " + Localization.ToNativeShortDateString(RangeEndDate) + "|Group By: " + GroupBy;
                            String ReportTitleShipping = "Shipping Report|Sum=" + Localization.CurrencyStringForDBWithoutExchangeRate(DS1SumShipping) + "|" + Localization.ToNativeShortDateString(RangeStartDate) + " - " + Localization.ToNativeShortDateString(RangeEndDate) + "|Group By: " + GroupBy;
                            // WRITE OUT THE TABLE:
                            String[] DD = DateSeries.Split('|');

                            String[] S1N = DS1ValuesN.Split('|');
                            String[] S1SubTotal = DS1ValuesSubTotal.Split('|');
                            String[] S1Total = DS1ValuesTotal.Split('|');
                            String[] S1Tax = DS1ValuesTax.Split('|');
                            String[] S1Shipping = DS1ValuesShipping.Split('|');

                            if (NumBuckets > 60)
                            {
                                // VERTICAL:
                                writer.Write("<p align=\"center\"><b>" + ReportTitle + "</b></p>\n");
                                writer.Write("<table border=\"1\" cellpadding=\"4\" cellspacing=\"0\">\n");
                                writer.Write("  <tr>\n");
                                writer.Write("    <td bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>Date</b></td>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b># Orders</b></td>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>SubTotal</b></td>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>Tax</b></td>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>Shipping</b></td>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>Total</b></td>\n");
                                writer.Write("  </tr>\n");

                                writer.Write("  <tr>\n");
                                writer.Write("    <td bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>Total</b></td>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>" + DS1SumN.ToString() + "</b></td>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>" + Localization.CurrencyStringForDisplayWithoutExchangeRate(DS1SumSubTotal) + "</b></td>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>" + Localization.CurrencyStringForDisplayWithoutExchangeRate(DS1SumTax) + "</b></td>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>" + Localization.CurrencyStringForDisplayWithoutExchangeRate(DS1SumShipping) + "</b></td>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>" + Localization.CurrencyStringForDisplayWithoutExchangeRate(DS1SumTotal) + "</b></td>\n");
                                writer.Write("  </tr>\n");

                                for (int row = 0; row < NumBuckets; row++)
                                {
                                    writer.Write("  <tr>\n");
                                    writer.Write("    <td>" + DD[row] + "</td>\n");
                                    writer.Write("    <td align=\"center\" >" + CommonLogic.IIF(S1N[row] == "0", "&nbsp;", S1N[row]) + "</td>\n");
                                    writer.Write("    <td align=\"center\" >" + CommonLogic.IIF(S1N[row] == "0", "&nbsp;", S1SubTotal[row]) + "</td>\n");
                                    writer.Write("    <td align=\"center\" >" + CommonLogic.IIF(S1N[row] == "0", "&nbsp;", S1Tax[row]) + "</td>\n");
                                    writer.Write("    <td align=\"center\" >" + CommonLogic.IIF(S1N[row] == "0", "&nbsp;", S1Shipping[row]) + "</td>\n");
                                    writer.Write("    <td align=\"center\" >" + CommonLogic.IIF(S1N[row] == "0", "&nbsp;", S1Total[row]) + "</td>\n");
                                    writer.Write("  </tr>\n");
                                }
                                writer.Write("  <tr>\n");
                                writer.Write("    <td bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>Total</b></td>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>" + DS1SumN.ToString() + "</b></td>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>" + Localization.CurrencyStringForDisplayWithoutExchangeRate(DS1SumSubTotal) + "</b></td>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>" + Localization.CurrencyStringForDisplayWithoutExchangeRate(DS1SumTax) + "</b></td>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>" + Localization.CurrencyStringForDisplayWithoutExchangeRate(DS1SumShipping) + "</b></td>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>" + Localization.CurrencyStringForDisplayWithoutExchangeRate(DS1SumTotal) + "</b></td>\n");
                                writer.Write("  </tr>\n");
                                writer.Write("</table>\n");
                            }
                            else
                            {
                                // HORIZONTAL:

                                // Number Of Orders Table:
                                writer.Write("<p align=\"center\"><b>" + ReportTitle + "</b></p>\n");
                                writer.Write("<table border=\"1\" cellpadding=\"4\" cellspacing=\"0\">\n");

                                writer.Write("  <tr>\n");
                                writer.Write("    <td bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\">&nbsp;</td>\n");
                                for (int row = 0; row < NumBuckets; row++)
                                {
                                    writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>" + DD[row] + "</b></td>\n");
                                }
                                writer.Write("    <td align=\"center\" bgcolor=\"#FFFFCC\"><b>Total</b></td>\n");
                                writer.Write("  </tr>\n");

                                // Number of Orders
                                writer.Write("  <tr>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b># Orders</b></td>\n");
                                for (int row = 0; row < NumBuckets; row++)
                                {
                                    writer.Write("    <td align=\"center\" >" + CommonLogic.IIF(S1N[row] == "0", "&nbsp;", S1N[row]) + "</td>\n");
                                }
                                writer.Write("    <td align=\"center\" bgcolor=\"#FFFFCC\"><b>" + DS1SumN.ToString() + "</b></td>\n");
                                writer.Write("  </tr>\n");

                                // SubTotals
                                writer.Write("  <tr>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>SubTotal</b></td>\n");
                                for (int row = 0; row < NumBuckets; row++)
                                {
                                    writer.Write("    <td align=\"center\" >" + CommonLogic.IIF(S1SubTotal[row] == "0", "&nbsp;", Localization.CurrencyStringForDisplayWithoutExchangeRate(Localization.ParseDBDecimal(S1SubTotal[row]))) + "</td>\n");
                                }
                                writer.Write("    <td align=\"center\" bgcolor=\"#FFFFCC\"><b>" + Localization.CurrencyStringForDisplayWithoutExchangeRate(DS1SumSubTotal) + "</b></td>\n");
                                writer.Write("  </tr>\n");

                                // Tax
                                writer.Write("  <tr>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>Tax</b></td>\n");
                                for (int row = 0; row < NumBuckets; row++)
                                {
                                    writer.Write("    <td align=\"center\" >" + CommonLogic.IIF(S1Tax[row] == "0", "&nbsp;", Localization.CurrencyStringForDisplayWithoutExchangeRate(Localization.ParseDBDecimal(S1Tax[row]))) + "</td>\n");
                                }
                                writer.Write("    <td align=\"center\" bgcolor=\"#FFFFCC\"><b>" + Localization.CurrencyStringForDisplayWithoutExchangeRate(DS1SumTax) + "</b></td>\n");
                                writer.Write("  </tr>\n");

                                // Shipping
                                writer.Write("  <tr>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>Shipping</b></td>\n");
                                for (int row = 0; row < NumBuckets; row++)
                                {
                                    writer.Write("    <td align=\"center\" >" + CommonLogic.IIF(S1Shipping[row] == "0", "&nbsp;", Localization.CurrencyStringForDisplayWithoutExchangeRate(Localization.ParseDBDecimal(S1Shipping[row]))) + "</td>\n");
                                }
                                writer.Write("    <td align=\"center\" bgcolor=\"#FFFFCC\"><b>" + Localization.CurrencyStringForDisplayWithoutExchangeRate(DS1SumShipping) + "</b></td>\n");
                                writer.Write("  </tr>\n");

                                // Totals
                                writer.Write("  <tr>\n");
                                writer.Write("    <td align=\"center\" bgcolor=\"#" + AppLogic.AppConfig("LightCellColor") + "\"><b>Totals</b></td>\n");
                                for (int row = 0; row < NumBuckets; row++)
                                {
                                    writer.Write("    <td align=\"center\" >" + CommonLogic.IIF(S1Total[row] == "0", "&nbsp;", Localization.CurrencyStringForDisplayWithoutExchangeRate(Localization.ParseDBDecimal(S1Total[row]))) + "</td>\n");
                                }
                                writer.Write("    <td align=\"center\" bgcolor=\"#FFFFCC\"><b>" + Localization.CurrencyStringForDisplayWithoutExchangeRate(DS1SumTotal) + "</b></td>\n");
                                writer.Write("  </tr>\n");

                                writer.Write("</table>\n");
                            }
                        }
                    }
                    ds1.Dispose();
                }
                catch (Exception ex)
                {
                    ErrorMsg = CommonLogic.GetExceptionDetail(ex, "<br/>");
                    writer.Write("<p align=\"left\"><b><font color=\"red\">" + ErrorMsg + "</font></b></p>\n");
                }

            }
        }

    }
}
