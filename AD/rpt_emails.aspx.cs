// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/rpt_emails.aspx.cs 7     9/30/06 3:38p Redwoodtree $
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
    /// Summary description for rpt_EMails.
    /// </summary>
    public partial class rpt_EMails : AspDotNetStorefront.SkinBase
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            SectionTitle = "Reports - Customer E-Mails";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {

            string SuperuserFilter = CommonLogic.IIF(ThisCustomer.IsAdminSuperUser, String.Empty, String.Format(" Customer.CustomerID not in ({0}) and ", AppLogic.AppConfig("Admin_Superuser")));

            IDataReader rsd = DB.GetRS("Select min(CreatedOn) as MinDate from customer " + DB.GetNoLock() + " ");
            DateTime MinCustomerDate = Localization.ParseUSDateTime("1/1/1990");
            if (rsd.Read())
            {
                MinCustomerDate = DB.RSFieldDateTime(rsd, "MinDate");
            }
            rsd.Close();

            String StartDate = CommonLogic.QueryStringCanBeDangerousContent("StartDate");
            String EndDate = CommonLogic.QueryStringCanBeDangerousContent("EndDate");
            String AffiliateID = CommonLogic.QueryStringCanBeDangerousContent("AffiliateID");
            String Gender = CommonLogic.QueryStringCanBeDangerousContent("Gender");
            String CouponCode = CommonLogic.QueryStringCanBeDangerousContent("CouponCode");
            String WithOrders = CommonLogic.QueryStringCanBeDangerousContent("WithOrders");
            String EasyRange = CommonLogic.QueryStringCanBeDangerousContent("EasyRange");
            String Day = CommonLogic.QueryStringCanBeDangerousContent("Day");
            String Month = CommonLogic.QueryStringCanBeDangerousContent("Month");
            String Year = CommonLogic.QueryStringCanBeDangerousContent("Year");
            String CustomerType = CommonLogic.QueryStringCanBeDangerousContent("CustomerType");

            if (StartDate.Length == 0)
            {
                StartDate = Localization.ToNativeShortDateString(MinCustomerDate);
            }
            if (EndDate.Length == 0)
            {
                EndDate = Localization.ToNativeShortDateString(System.DateTime.Now);
            }

            if (EasyRange.Length == 0)
            {
                EasyRange = "Today";
            }
            if (CustomerType.Length == 0)
            {
                CustomerType = "AllCustomers";
            }

            // reset date range here, to ensure new orders are visible:
            if (StartDate.Length == 0)
            {
                StartDate = Localization.ToNativeShortDateString(MinCustomerDate);
            }
            if (EndDate.Length == 0)
            {
                EndDate = Localization.ToNativeShortDateString(System.DateTime.Now.AddDays(1));
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

            writer.Write("<form method=\"GET\" action=\"rpt_EMails.aspx\" id=\"ReportForm\" name=\"ReportForm\" onsubmit=\"return (validateForm(this) && ReportForm_Validator(this))\">");
            writer.Write("  <table border=\"1\" cellpadding=\"1\" cellspacing=\"0\" width=\"100%\">");
            writer.Write("    <tr>");
            writer.Write("      <td width=\"25%\" align=\"center\" bgcolor=\"#EAEAEA\"><b><font color=\"#000000\">Date Range:</font></b></td>");
            writer.Write("      <td width=\"25%\" align=\"center\" bgcolor=\"#EAEAEA\"><b><font color=\"#000000\">Customer Qualifiers:</font></b></td>");
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
            writer.Write("            <td width=\"50%\">With Orders:</td>");
            writer.Write("            <td width=\"50%\">");
            writer.Write("                <input type=\"radio\" name=\"WithOrders\" value=\"No\"" + CommonLogic.IIF(WithOrders == "No" || WithOrders.Length == 0, " checked ", "") + ">No&nbsp;&nbsp;&nbsp;&nbsp;");
            writer.Write("                <input type=\"radio\" name=\"WithOrders\" value=\"Yes\"" + CommonLogic.IIF(WithOrders == "Yes", " checked ", "") + ">Yes");
            writer.Write("                <input type=\"radio\" name=\"WithOrders\" value=\"Invert\"" + CommonLogic.IIF(WithOrders == "Invert", " checked ", "") + ">Without Orders");
            writer.Write("              </td>");
            writer.Write("          </tr>");
            writer.Write("        </table>");
            writer.Write("        </td>");
            writer.Write("    </tr>");
            writer.Write("    <tr>");
            writer.Write("      <td width=\"100%\" valign=\"top\" align=\"center\" bgcolor=\"#EAEAEA\" colspan=\"2\">");
            writer.Write("        <input type=\"submit\" value=\"Submit\" name=\"B1\"><input type=\"button\" onClick=\"javascript:self.location='rpt_EMails.aspx';\" value=\"Reset\" name=\"B2\">");
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
                        DateWhere = " CreatedOn>=" + DB.DateQuote(Localization.ToDBDateTimeString(dt));
                        RangeStartDate = Localization.ParseNativeDateTime(StartDate);
                    }
                    else
                    {
                        RangeStartDate = System.DateTime.MinValue; // will get min date returned from either query
                    }
                    if (EndDate.Length != 0)
                    {
                        DateTime dt = Localization.ParseNativeDateTime(EndDate + " 11:59:59.999 PM");
                        DateWhere += CommonLogic.IIF(DateWhere.Length != 0, " and ", "") + "CreatedOn <=" + DB.DateQuote(Localization.ToDBDateTimeString(dt));
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
                        DateWhere = " day(CreatedOn)=" + Day + " ";
                    }
                    if (Month.Length != 0 && Month != "0")
                    {
                        if (DateWhere.Length != 0)
                        {
                            DateWhere += " and ";
                        }
                        DateWhere += " month(CreatedOn)=" + Month + " ";
                    }
                    if (Year.Length != 0 && Year != "0")
                    {
                        if (DateWhere.Length != 0)
                        {
                            DateWhere += " and ";
                        }
                        DateWhere += " year(CreatedOn)=" + Year + " ";
                    };
                    String DaySpec = CommonLogic.IIF(Day.Length == 0 || Day == "0", "1", Day);
                    String MonthSpec = CommonLogic.IIF(Month.Length == 0 || Month == "0", "1", Month);
                    String YearSpec = CommonLogic.IIF(Year.Length == 0 || Year == "0", System.DateTime.Now.Year.ToString(), Year);
                    RangeStartDate = Localization.ParseNativeDateTime(MonthSpec + "/" + DaySpec + "/" + YearSpec);
                    RangeEndDate = RangeStartDate;
                    break;
                case "Today":
                    DateWhere = "day(CreatedOn)=" + System.DateTime.Now.Day.ToString() + " and month(CreatedOn)=" + System.DateTime.Now.Month.ToString() + " and year(CreatedOn)=" + System.DateTime.Now.Year.ToString();
                    RangeStartDate = System.DateTime.Now;
                    RangeEndDate = System.DateTime.Now;
                    break;
                case "Yesterday":
                    DateWhere = "day(CreatedOn)=" + System.DateTime.Now.AddDays(-1).Day.ToString() + " and month(CreatedOn)=" + System.DateTime.Now.AddDays(-1).Month.ToString() + " and year(CreatedOn)=" + System.DateTime.Now.AddDays(-1).Year.ToString();
                    RangeStartDate = System.DateTime.Now.AddDays(-1);
                    RangeEndDate = System.DateTime.Now.AddDays(-1);
                    break;
                case "ThisWeek":
                    int DayOfWeek = (int)System.DateTime.Now.DayOfWeek;
                    System.DateTime weekstart = System.DateTime.Now.AddDays(-(DayOfWeek));
                    System.DateTime weekend = weekstart.AddDays(6);
                    int weekstartday = weekstart.DayOfYear;
                    int weekendday = weekend.DayOfYear;
                    DateWhere = "year(CreatedOn)=" + System.DateTime.Now.Year.ToString() + " and (datepart(\"dy\",CreatedOn)>=" + weekstartday.ToString() + " and datepart(\"dy\",CreatedOn)<=" + weekendday.ToString() + ")";
                    RangeStartDate = weekstart;
                    RangeEndDate = weekend;
                    break;
                case "LastWeek":
                    int DayOfWeek2 = (int)System.DateTime.Now.DayOfWeek;
                    System.DateTime weekstart2 = System.DateTime.Now.AddDays(-(DayOfWeek2)).AddDays(-7);
                    System.DateTime weekend2 = weekstart2.AddDays(6);
                    int weekstartday2 = weekstart2.DayOfYear;
                    int weekendday2 = weekend2.DayOfYear;
                    DateWhere = "year(CreatedOn)=" + System.DateTime.Now.Year.ToString() + " and (datepart(\"dy\",CreatedOn)>=" + weekstartday2.ToString() + " and datepart(\"dy\",CreatedOn)<=" + weekendday2.ToString() + ")";
                    RangeStartDate = weekstart2;
                    RangeEndDate = weekend2;
                    break;
                case "ThisMonth":
                    DateWhere = "month(CreatedOn)=" + System.DateTime.Now.Month.ToString() + " and year(CreatedOn)=" + System.DateTime.Now.Year.ToString();
                    RangeStartDate = Localization.ParseNativeDateTime(System.DateTime.Now.Month.ToString() + "/1/" + System.DateTime.Now.Year.ToString());
                    RangeEndDate = RangeStartDate.AddMonths(1).AddDays(-1);
                    break;
                case "LastMonth":
                    DateWhere = "month(CreatedOn)=" + System.DateTime.Now.AddMonths(-1).Month.ToString() + " and year(CreatedOn)=" + System.DateTime.Now.AddMonths(-1).Year.ToString();
                    RangeStartDate = Localization.ParseNativeDateTime(System.DateTime.Now.AddMonths(-1).Month.ToString() + "/1/" + System.DateTime.Now.AddMonths(-1).Year.ToString());
                    RangeEndDate = RangeStartDate.AddMonths(1).AddDays(-1);
                    break;
                case "ThisYear":
                    DateWhere = "year(CreatedOn)=" + System.DateTime.Now.Year.ToString();
                    RangeStartDate = Localization.ParseUSDateTime("1/1/" + System.DateTime.Now.Year.ToString());
                    RangeEndDate = RangeStartDate.AddYears(1).AddDays(-1);
                    if (RangeEndDate > System.DateTime.Now)
                    {
                        RangeEndDate = System.DateTime.Now;
                    }
                    break;
                case "LastYear":
                    DateWhere = "year(CreatedOn)=" + System.DateTime.Now.AddYears(-1).Year.ToString();
                    RangeStartDate = Localization.ParseUSDateTime("1/1/" + System.DateTime.Now.AddYears(-1).Year.ToString());
                    RangeEndDate = RangeStartDate.AddYears(1).AddDays(-1);
                    break;
            }
            if (DateWhere.Length != 0)
            {
                DateWhere = "(" + DateWhere + ")";
            }


            String WhereClause = DateWhere;
            String GeneralWhere = String.Empty;
            String RegOnlyWhere = String.Empty;
            if (AffiliateID != "-" && AffiliateID.Length != 0)
            {
                if (GeneralWhere.Length != 0)
                {
                    GeneralWhere += " and ";
                }
                GeneralWhere += "AffiliateID=" + AffiliateID.ToString();
            }
            if (Gender != "-" && Gender.Length != 0)
            {
                if (GeneralWhere.Length != 0)
                {
                    GeneralWhere += " and ";
                }
                GeneralWhere += "upper(Gender)=" + DB.SQuote(Gender.ToUpperInvariant());
            }
            if (CouponCode != "-" && CouponCode.Length != 0)
            {
                if (GeneralWhere.Length != 0)
                {
                    GeneralWhere += " and ";
                }
                GeneralWhere += "upper(CouponCode)=" + DB.SQuote(CouponCode.ToUpperInvariant());
            }
            if (WithOrders == "Yes")
            {
                if (RegOnlyWhere.Length != 0)
                {
                    RegOnlyWhere += " and ";
                }
                RegOnlyWhere += "customerid in (select distinct customerid from orders " + DB.GetNoLock() + " )";
            }
            if (WithOrders == "Invert")
            {
                if (RegOnlyWhere.Length != 0)
                {
                    RegOnlyWhere += " and ";
                }
                RegOnlyWhere += "customerid not in (select distinct customerid from orders " + DB.GetNoLock() + " )";
            }
            if (GeneralWhere.Length != 0)
            {
                GeneralWhere = "(" + GeneralWhere + ")";
            }
            if (RegOnlyWhere.Length != 0)
            {
                RegOnlyWhere = "(" + RegOnlyWhere + ")";
            }

            if (DateWhere.Length != 0)
            {
                String sql = "select EMail from Customer " + DB.GetNoLock() + " where " + SuperuserFilter.ToString() + " EMail <> '' " + CommonLogic.IIF(RegOnlyWhere.Length != 0, " and " + RegOnlyWhere, "") + CommonLogic.IIF(GeneralWhere.Length != 0, " and " + GeneralWhere, "") + CommonLogic.IIF(WhereClause.Length != 0, " and " + WhereClause, "") + " order by createdon desc";
                if (AppLogic.AppConfigBool("Admin_ShowReportSQL"))
                {
                    writer.Write("<p align=\"left\">SQL=" + sql + "</p>\n");
                }
                rs = DB.GetRS(sql);
                while (rs.Read())
                {
                    writer.Write(DB.RSField(rs, "EMail") + "<br/>");
                }
                rs.Close();
            }
        }

    }
}
