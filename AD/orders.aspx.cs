// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/orders.aspx.cs 24    10/03/06 7:56p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Threading;
using System.Globalization;
using System.Web;
using System.Text;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for orders.
    /// </summary>
    public partial class orders : AspDotNetStorefront.SkinBase
    {
        private string OrderByFields = "IsNew desc, OrderDate desc";

        private int m_FirstOrderNumber = 0;

        protected DataSet dsAffiliate = null;
        protected DataSet dsCouponCode = null;
        protected DataSet dsState = null;
        protected DataSet dsSelected = null;

        public string HeaderImage
        {
            get
            {
                return String.Format(AppLogic.LocateImageURL("images/orders.gif"), SkinID);
            }
        }

        public string NewImage
        {
            get
            {
                return String.Format(AppLogic.LocateImageURL("images/new.gif"), SkinID);
            }
        }

        public int FirstOrderNumber
        {
            get
            {
                return m_FirstOrderNumber;
            }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            if (CommonLogic.QueryStringCanBeDangerousContent("OrderNumber").Length != 0)
            {
                m_FirstOrderNumber = CommonLogic.QueryStringUSInt("OrderNumber");
                txtOrderNumber.Text = m_FirstOrderNumber.ToString();
                rbNewOrdersOnly.SelectedValue = "0";
            }

            DataRow dr;
            //DoLocalization();

            if (!Page.IsPostBack) // Only initialize once
            {
                DoLocalization();
                DB.ExecuteSQL("update orders set IsNew=0 where ParentOrderNumber IS NOT NULL"); // any "ad hoc" orders should not be new. so this is a safety check to force that.

                ProductMatchRow.Visible = (AppLogic.NumProductsInDB < 250);
                if (ProductMatchRow.Visible)
                {
                    ProductMatch.Items.Add(new ListItem("ALL","-"));
                    IDataReader rs = DB.GetRS("select ProductID,Name from Product with (NOLOCK) order by convert(nvarchar(4000),Name),ProductID");
                    while (rs.Read())
                    {
                        ProductMatch.Items.Add(new ListItem(DB.RSField(rs, "Name"), DB.RSFieldInt(rs, "ProductID").ToString()));
                    }
                    rs.Close();
                }
                
                IDataReader rsd = DB.GetRS("Select min(OrderDate) as MinDate from orders " + DB.GetNoLock());
                DateTime MinOrderDate = Localization.ParseUSDateTime("1/1/1990");
                if (rsd.Read())
                {
                    MinOrderDate = DB.RSFieldDateTime(rsd, "MinDate");
                }
                rsd.Close();
                dateStart.SelectedDate = MinOrderDate;
                if (dateStart.SelectedDate == System.DateTime.MinValue)
                {
                    dateStart.SelectedDate = System.DateTime.Now;
                }
                dateEnd.SelectedDate = System.DateTime.Now;

                dsAffiliate = DB.GetDS("select AffiliateID,Name from affiliate  " + DB.GetNoLock() + " order by displayorder,name", false);
                dr = dsAffiliate.Tables[0].NewRow();
                dr["AffiliateID"] = 0;
                dr["Name"] = "-";
                dsAffiliate.Tables[0].Rows.InsertAt(dr, 0);
                ddAffiliate.DataSource = dsAffiliate;
                ddAffiliate.DataBind();
                dsAffiliate.Dispose();

                dsCouponCode = DB.GetDS("select CouponCode from Coupon  " + DB.GetNoLock() + " order by CouponCode", false);
                dr = dsCouponCode.Tables[0].NewRow();
                dr["CouponCode"] = "-";
                dsCouponCode.Tables[0].Rows.InsertAt(dr, 0);
                ddCouponCode.DataSource = dsCouponCode;
                ddCouponCode.DataBind();
                dsCouponCode.Dispose();

                dsState = DB.GetDS("select Abbreviation,Name from state  " + DB.GetNoLock() + " order by DisplayOrder,Name", AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
                dr = dsState.Tables[0].NewRow();
                dr["Abbreviation"] = "-";
                dr["Name"] = "SELECT ONE";
                dsState.Tables[0].Rows.InsertAt(dr, 0);
                ddShippingState.DataSource = dsState;
                ddShippingState.DataBind();
                dsState.Dispose();

                GenerateReport().Dispose();
            }
        }


        private DataSet GenerateReport()
        {
            string sql = "select * from orders  " + DB.GetNoLock() + " where " + WhereClause() + DateClause() + " order by " + OrderByFields;
            dsSelected = DB.GetDS(sql, false);

            dlSelected.DataSource = dsSelected;

            lblError.Text = String.Empty;
            if (AppLogic.AppConfigBool("Admin_ShowReportSQL"))
            {
                lblError.Text = "SQL=" + sql;
            }

            pnlBulkPrintingReport.Visible = BulkPrintingReport.Checked;
            pnlRegularReport.Visible = RegularReport.Checked;
            pnlSummaryReport.Visible = SummaryReport.Checked;

            if (dsSelected.Tables[0].Rows.Count > 0)
            {
                m_FirstOrderNumber = (Int32)dsSelected.Tables[0].Rows[0]["OrderNumber"];
                if (RegularReport.Checked)
                {
                    // don't have to do anything here
                }
                if (BulkPrintingReport.Checked)
                {
                    String summarySQL = "select OrderNumber,IsPrinted,ReadyToShip from orders " + DB.GetNoLock() + " where " + WhereClause() + DateClause() + " order by OrderNumber";
                    DataSet dsBulkPrinting = DB.GetDS(summarySQL, false);
                    StringBuilder tmpS = new StringBuilder(4096);

                    tmpS.Append("<script type=\"text/javascript\">\n");
                    tmpS.Append("   function checkUncheckAll(theElement, ElementName)\n");
                    tmpS.Append("{\n");

                    //tmpS.Append("alert('theElement=' + theElement);\n");
                    //tmpS.Append("alert('theElement.form=' + theElement.form);\n");
                    tmpS.Append("	var chkb = theElement.form.PrintNow;\n");
                    tmpS.Append("	var chkb0 = theElement.form.checkall;\n");
                    //				tmpS.Append("alert('chkb=' + chkb);\n");
                    //				tmpS.Append("alert('chkb0=' + chkb0);\n");
                    //				tmpS.Append("alert('chkb.length=' + chkb.length);\n");
                    //				tmpS.Append("alert('chkb0.length=' + chkb0.length);\n");
                    //				tmpS.Append("alert('chkb[0].checked=' + chkb[0].checked);\n");
                    //				tmpS.Append("alert('chkb0.checked=' + chkb0.checked);\n");
                    tmpS.Append("	for (var i=0; i < chkb.length; i++)\n");
                    tmpS.Append("	{\n");
                    tmpS.Append("		chkb[i].checked = chkb0.checked;\n");
                    tmpS.Append("	}\n");

                    //				//tmpS.Append("   var theForm = theElement.form, z = 0;\n");
                    //				tmpS.Append("   var theForm = document.forms[0], z = 0;\n");
                    //				tmpS.Append("alert(theForm);\n");
                    //				tmpS.Append("alert(theForm.length);\n");
                    //				tmpS.Append("	for(z=0; z<theForm.length;z++){\n");
                    //				tmpS.Append("		alert(theForm[z].type + ',' + theForm[z].name + ',' + ElementName);\n");
                    //				tmpS.Append("     if(theForm[z].type == 'checkbox' && theForm[z].name == ElementName){\n");
                    //				tmpS.Append("			alert('set it');\n");
                    //				tmpS.Append("	 theForm[z].checked = theElement.checked;\n");
                    //				tmpS.Append("	  }\n");
                    //				tmpS.Append("     }\n");
                    tmpS.Append("    }\n");
                    tmpS.Append("</script>\n");

                    tmpS.Append("<table cellpadding=\"4\" cellspacing=\"0\" border=\"0\" style=\"border-width: 1px; border-style: solid;\">");
                    tmpS.Append("<tr>");
                    tmpS.Append("<td align=\"center\"><b><nobr>Order Number</nobr></b></td>");
                    tmpS.Append("<td align=\"center\"><b><nobr>Order Date</nobr></b></td>");
                    tmpS.Append("<td align=\"center\"><b><nobr>Order Total</nobr></b></td>");
                    tmpS.Append("<td align=\"left\"><b><nobr>Ship To Address</nobr></b></td>");
                    tmpS.Append("<td align=\"left\"><b><nobr>Items</nobr></b></td>");
                    tmpS.Append("<td align=\"center\"><b><nobr>Is Shipped</nobr></b></td>");
                    tmpS.Append("<td align=\"center\"><b><nobr>Is Printed</nobr></b></td>");
                    tmpS.Append("<td align=\"center\"><b><nobr>Print It Now</nobr></b><br/>check all<br/><input type=\"checkbox\" id=\"checkall\" name=\"checkall\" onclick=\"checkUncheckAll(this,'PrintNow');\"/></td>");
                    tmpS.Append("</tr>");
                    foreach (DataRow row in dsBulkPrinting.Tables[0].Rows)
                    {

                        int ONX = DB.RowFieldInt(row, "OrderNumber");
                        Order ord = new Order(ONX, ThisCustomer.LocaleSetting);

                        String ShipAddr = (ord.ShippingAddress.m_FirstName + " " + ord.ShippingAddress.m_LastName).Trim() + "<br/>";
                        ShipAddr += ord.ShippingAddress.m_Address1;
                        if (ord.ShippingAddress.m_Address2.Length != 0)
                        {
                            ShipAddr += "<br/>" + ord.ShippingAddress.m_Address2;
                        }
                        if (ord.ShippingAddress.m_Suite.Length != 0)
                        {
                            ShipAddr += ", " + ord.ShippingAddress.m_Suite;
                        }
                        ShipAddr += "<br/>" + ord.ShippingAddress.m_City + ", " + ord.ShippingAddress.m_State + " " + ord.ShippingAddress.m_Zip;
                        ShipAddr += "<br/>" + ord.ShippingAddress.m_Country.ToUpper(CultureInfo.InvariantCulture);
                        ShipAddr += "<br/>" + ord.ShippingAddress.m_Phone;

                        tmpS.Append("<tr>");
                        tmpS.Append("<td align=\"center\" valign=\"top\" style=\"border-width: 1px; border-style: solid;\">");
                        tmpS.Append(ONX.ToString());
                        tmpS.Append("</td>");
                        tmpS.Append("<td align=\"center\" valign=\"top\" style=\"border-width: 1px; border-style: solid;\">");
                        tmpS.Append(Localization.ToNativeDateTimeString(ord.OrderDate));
                        tmpS.Append("</td>");
                        tmpS.Append("<td align=\"center\" valign=\"top\" style=\"border-width: 1px; border-style: solid;\">");
                        tmpS.Append(ThisCustomer.CurrencyString(ord.Total(true)));
                        tmpS.Append("</td>");
                        tmpS.Append("<td align=\"left\" valign=\"top\" style=\"border-width: 1px; border-style: solid;\">");
                        tmpS.Append(ShipAddr);
                        tmpS.Append("</td>");
                        tmpS.Append("<td align=\"left\" valign=\"top\" style=\"border-width: 1px; border-style: solid;\">");

                        bool first = true;
                        foreach (CartItem c in ord.CartItems)
                        {
                            if (!first)
                            {
                                tmpS.Append("<br/><br/>");
                            }
                            tmpS.Append("(" + c.m_Quantity.ToString() + ") ");
                            tmpS.Append(ord.GetLineItemDescription(c));
                            first = false;
                        }

                        tmpS.Append("</td>");
                        tmpS.Append("<td align=\"center\" valign=\"top\" style=\"border-width: 1px; border-style: solid;\">");
                        tmpS.Append(CommonLogic.IIF(DB.RowFieldDateTime(row, "ShippedOn") == System.DateTime.MinValue, "No", Localization.ToNativeDateTimeString(DB.RowFieldDateTime(row, "ShippedOn"))));
                        tmpS.Append("</td>");
                        tmpS.Append("<td align=\"center\" valign=\"top\" style=\"border-width: 1px; border-style: solid;\">");
                        //tmpS.Append("<input type=\"checkbox\" id=\"IsPrinted\" name=\"IsPrinted\" value=\"" + ONX.ToString() + "\" " + CommonLogic.IIF(DB.RowFieldBool(row,"IsPrinted")," checked ","") + " READONLY>");
                        tmpS.Append(CommonLogic.IIF(DB.RowFieldBool(row, "IsPrinted"), "Yes", "No"));
                        tmpS.Append("</td>");
                        tmpS.Append("<td align=\"center\" valign=\"top\" style=\"border-width: 1px; border-style: solid;\">");
                        tmpS.Append("<input type=\"checkbox\" id=\"PrintNow\" name=\"PrintNow\" value=\"" + ONX.ToString() + "\" " + CommonLogic.IIF(!DB.RowFieldBool(row, "IsPrinted"), " checked ", "") + ">");
                        tmpS.Append("</td>");
                        tmpS.Append("</tr>");
                    }
                    tmpS.Append("<tr>");
                    tmpS.Append("<td colspan=\"7\">&nbsp;</td>");
                    //tmpS.Append("<td><input type=\"button\" value=\"Update Status\" onClick=\"alert('hi');\"></td>");
                    tmpS.Append("<td>");
                    tmpS.Append("<div style=\"display:none;\"><input type=\"checkbox\" id=\"PrintNow\" name=\"PrintNow\" value=\"0\"></div>");
                    tmpS.Append("<input type=\"button\" value=\"Print Receipts\" onClick=\"OpenPrintWindow();\">");
                    tmpS.Append("</td>");
                    tmpS.Append("</tr>");
                    tmpS.Append("</table>");

                    tmpS.Append("<script type=\"text/javascript\">\n");
                    tmpS.Append("	function OpenPrintWindow()\n");
                    tmpS.Append("	{\n");
                    tmpS.Append("	alert('Now Opening Print Window. If it does not appear, please check your browser and google toobar pop-up blocking settings.');\n");
                    tmpS.Append("	var Orders = '';\n");
                    tmpS.Append("	var chkb = document.getElementsByName('PrintNow');\n");
                    tmpS.Append("	for (var i=0; i < chkb.length; i++)\n");
                    tmpS.Append("	{\n");
                    //tmpS.Append("		alert(chkb[i].value);\n");
                    tmpS.Append("		if (chkb[i].checked)\n");
                    tmpS.Append("		{\n");
                    tmpS.Append("			if(i > 0)\n");
                    tmpS.Append("			{\n");
                    tmpS.Append("				Orders = Orders + ',';\n");
                    tmpS.Append("			}\n");
                    tmpS.Append("			if(chkb[i].value != '0') Orders = Orders + chkb[i].value;\n");
                    tmpS.Append("		}\n");
                    tmpS.Append("	}\n");
                    //tmpS.Append("	alert(Orders);\n");
                    tmpS.Append("	if(Orders == '')\n");
                    tmpS.Append("	{\n");
                    tmpS.Append("		alert('Nothing To Print');\n");
                    tmpS.Append("	}\n");
                    tmpS.Append("	else\n");
                    tmpS.Append("	{\n");
                    tmpS.Append("		window.open('printreceipts.aspx?ordernumbers=' + Orders,'ASPDNSF_ML" + CommonLogic.GetRandomNumber(1, 100000).ToString() + "','height=600,width=800,top=0,left=0,status=yes,toolbar=yes,menubar=yes,scrollbars=yes,location=yes')\n");
                    tmpS.Append("	}\n");
                    tmpS.Append("	}\n");
                    tmpS.Append("</SCRIPT>\n");

                    Literal4.Text = tmpS.ToString();
                    dsBulkPrinting.Dispose();
                }
                if (SummaryReport.Checked)
                {
                    // doing summary report:
                    String SummaryReportFields = AppLogic.AppConfig("OrderSummaryReportFields");
                    if (SummaryReportFields.Length == 0)
                    {
                        SummaryReportFields = "*";
                    }
                    String summarySQL = "select " + SummaryReportFields + " from orders " + DB.GetNoLock() + " where " + WhereClause() + DateClause() + " order by " + OrderByFields;
                    DataSet dsSummary = DB.GetDS(summarySQL, false);

                    // unencrypt some data in the ds:
                    int SummaryCardNumberFieldIndex = -1;
                    int col = 0;
                    foreach (String s in SummaryReportFields.Split(','))
                    {
                        if (s.Trim().ToUpper(CultureInfo.InvariantCulture) == "CARDNUMBER")
                        {
                            SummaryCardNumberFieldIndex = col;
                        }
                        col++;
                    }

                    if (SummaryCardNumberFieldIndex != -1)
                    {
                        foreach (DataRow row in dsSummary.Tables[0].Rows)
                        {
                            String s = row[SummaryCardNumberFieldIndex].ToString();
                            if (s.Length != 0)
                            {
                                try
                                {
                                    s = Security.UnmungeString(s, Order.StaticGetSaltKey(DB.RowFieldInt(row,"OrderNumber")));
                                    if (!s.StartsWith(Security.ro_DecryptFailedPrefix, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        row[SummaryCardNumberFieldIndex] = s;
                                    }
                                }
                                catch { }
                            }
                        }
                    }

                    SummaryGrid.DataSource = dsSummary;
                    SummaryGrid.DataBind();
                    dsSummary.Dispose();
                }
            }
            else
            {
                lblError.Text += "<br/><br/>" + AppLogic.GetString("No Orders Found", SkinID, ThisCustomer.LocaleSetting);
                pnlBulkPrintingReport.Visible = false;
                pnlRegularReport.Visible = false;
                pnlSummaryReport.Visible = false;
            }

            Page.DataBind();
            return dsSelected;
        }

        private void btnReset_Click(object sender, System.EventArgs e)
        {
            txtOrderNumber.Text = String.Empty;
            txtCustomerID.Text = String.Empty;
            txtEMail.Text = String.Empty;
            txtCreditCardNumber.Text = String.Empty;
            txtCustomerName.Text = String.Empty;
            txtCompany.Text = String.Empty;
            ddPaymentMethod.SelectedIndex = 0;
            TransactionState.SelectedIndex = 0;
            TransactionType.SelectedIndex = 0;
            ProductMatch.SelectedIndex = 0;
            ddAffiliate.SelectedIndex = 0;
            ddCouponCode.SelectedIndex = 0;
            ddShippingState.SelectedIndex = 0;
            dateStart.Clear();
            dateEnd.Clear();
            rbRange.Checked = true;
            rbNewOrdersOnly.SelectedValue = "1";
            GenerateReport().Dispose();
        }

        /// <summary>
        /// Calculates the Where clause for the date portion of the search.
        /// </summary>
        public string DateClause()
        {
            string result = String.Empty;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;

            if (rbRange.Checked) //Use Dates above Range
            {
                //if (!(dateStart.ValidDateEntered && dateEnd.ValidDateEntered))  //blank dates
                //{
                //    return String.Empty;
                //}
                //if (!dateEnd.ValidDateEntered)
                //{
                //    dateEnd.SelectedDate = DateTime.Today.AddDays(1);
                //}
                if (dateStart.SelectedDate.CompareTo(dateEnd.SelectedDate) > 0) //Flip them
                {
                    endDate = dateStart.SelectedDate;
                    dateStart.SelectedDate = dateEnd.SelectedDate;
                    dateEnd.SelectedDate = endDate;
                }

                startDate = dateStart.SelectedDate;
                endDate = dateEnd.SelectedDate;

            }
            else
            {
                switch (rbEasyRange.SelectedValue)
                {
                    case "Today":
                        {
                            startDate = DateTime.Today;
                            endDate = startDate;
                            break;
                        }
                    case "Yesterday":
                        {
                            startDate = DateTime.Today.AddDays(-1);
                            endDate = startDate;
                            break;
                        }
                    case "ThisWeek":
                        {
                            startDate = DateTime.Today.AddDays(-((int)DateTime.Today.DayOfWeek));
                            endDate = startDate.AddDays(6);
                            break;
                        }
                    case "LastWeek":
                        {
                            startDate = DateTime.Today.AddDays(-((int)DateTime.Today.DayOfWeek) - 7);
                            endDate = startDate.AddDays(6);
                            break;
                        }
                    case "ThisMonth":
                        {
                            startDate = DateTime.Today.AddDays(1 - DateTime.Today.Day);
                            endDate = startDate.AddMonths(1);
                            break;
                        }
                    case "LastMonth":
                        {
                            startDate = DateTime.Today.AddMonths(-1);
                            startDate = startDate.AddDays(1 - startDate.Day);
                            endDate = startDate.AddMonths(1);
                            break;
                        }
                    case "ThisYear":
                        {
                            startDate = DateTime.Today.AddMonths(1 - DateTime.Today.Month);
                            startDate = startDate.AddDays(1 - startDate.Day);
                            endDate = startDate.AddYears(1);
                            break;
                        }
                    case "LastYear":
                        {
                            startDate = DateTime.Today.AddYears(-1);
                            startDate = startDate.AddMonths(1 - startDate.Month);
                            startDate = startDate.AddDays(1 - startDate.Day);
                            endDate = startDate.AddYears(1);
                            break;
                        }
                }
            }
            if (startDate == endDate)
            {
                result = String.Format(" and ((OrderDate>={0}) and (OrderDate < {1}))", DB.DateQuote(Localization.ToDBShortDateString(startDate)), DB.DateQuote(Localization.ToDBShortDateString(endDate.AddDays(1))));
            }
            else
            {
                result = String.Format(" and ((OrderDate>={0}) and (OrderDate < {1}))", DB.DateQuote(Localization.ToDBShortDateString(startDate)), DB.DateQuote(Localization.ToDBShortDateString(endDate.AddDays(1))));
            }
            return result;
        }

        /// <summary>
        /// Creates the Where Clause based on the Qualification fields.
        /// </summary>
        public string WhereClause()
        {
            string result = "1=1";
            string sQuery = " and ({0}={1})";

            // if they are searching, clear the IsNew flag, as a convenience, as they are almost always searching for "old" orders:
            if (txtCustomerID.Text.Trim().Length != 0 || txtOrderNumber.Text.Trim().Length != 0 || txtEMail.Text.Trim().Length != 0 || txtCreditCardNumber.Text.Trim().Length != 0 || txtCustomerName.Text.Length != 0)
            {
                rbNewOrdersOnly.SelectedValue = "0";
            }
            if (ddAffiliate.SelectedItem != null)
            {
                if (ddAffiliate.SelectedValue != "0" && ddAffiliate.SelectedItem.Text.Length != 0)
                {
                    result += String.Format(sQuery, "AffiliateID", ddAffiliate.SelectedValue);
                }
            }
            if (ddCouponCode.SelectedItem != null)
            {
                if (ddCouponCode.SelectedValue != "-" && ddCouponCode.SelectedItem.Text.Length != 0)
                {
                    result += String.Format(sQuery, "CouponCode", DB.SQuote(ddCouponCode.SelectedValue));
                }
            }
            if (ddShippingState.SelectedItem != null)
            {
                if (ddShippingState.SelectedValue != "-" && ddShippingState.SelectedItem.Text.Length != 0)
                {
                    result += String.Format(sQuery, "ShippingState", DB.SQuote(ddShippingState.SelectedValue));
                }
            }
            if (rbNewOrdersOnly.SelectedValue == "1")
            {
                result += String.Format(sQuery, "IsNew", 1);
            }
            if (txtEMail.Text.Trim().Length != 0)
            {
                result += String.Format(" and (EMail like {0})", DB.SQuote("%" + txtEMail.Text + "%"));
            }
            if (txtCustomerID.Text.Trim().Length != 0)
            {
                result += String.Format(sQuery, "CustomerID", txtCustomerID.Text);
            }
            if (txtOrderNumber.Text.Trim().Length != 0)
            {
                result += String.Format(" and (OrderNumber like {0} or AuthorizationPNREF like {1} or RecurringSubscriptionID like {2})", DB.SQuote("%" + txtOrderNumber.Text + "%"), DB.SQuote("%" + txtOrderNumber.Text + "%"), DB.SQuote("%" + txtOrderNumber.Text + "%"));
            }
            if (txtCreditCardNumber.Text.Trim().Length != 0)
            {
                result += String.Format(" and ((convert(nvarchar(4000),{0})={1})", "CardNumber", DB.SQuote(Security.MungeString(txtCreditCardNumber.Text.Trim(), Order.StaticGetSaltKey(0))));
                if (txtCreditCardNumber.Text.Trim().Length == 4)
                {
                    result += String.Format(" or (convert(nvarchar(4000),{0})={1})", "Last4", DB.SQuote(txtCreditCardNumber.Text.Trim()));
                }
                result += ")";
            }
            if (txtCustomerName.Text.Length != 0)
            {
                result += String.Format(" and ((FirstName + ' ' + LastName) like {0})", DB.SQuote(txtCustomerName.Text));
            }
            if (txtCompany.Text.Length != 0)
            {
                result += String.Format(" and (ShippingCompany like {0} or BillingCompany like {0})", DB.SQuote("%" + txtCompany.Text + "%"));
            }
            if (TransactionState.SelectedValue != "-")
            {
                result += String.Format(" and TransactionState={0}", DB.SQuote(TransactionState.SelectedValue));
            }
            if (TransactionType.SelectedValue != "-")
            {
                AppLogic.TransactionTypeEnum tt = (AppLogic.TransactionTypeEnum)Enum.Parse(typeof(AppLogic.TransactionTypeEnum), TransactionType.SelectedValue, true);
                result += String.Format(" and TransactionType={0}", (int)tt);
            }

            if (ProductMatchRow.Visible)
            {
                if ( ProductMatch.SelectedValue != "-" )
                {
                    result +=
                        String.Format(
                            " and OrderNumber in (select ordernumber from orders_shoppingcart where productid={0})",
                            DB.SQuote( ProductMatch.SelectedValue ) );
                }
            }

            if (ddPaymentMethod.SelectedValue != "-")
            {
                String PM = AppLogic.CleanPaymentMethod(ddPaymentMethod.SelectedValue);
                if (PM == AppLogic.ro_PMCreditCard)
                {
                    result += String.Format(" and (PaymentMethod={0} or (PaymentGateway is not null and upper(PaymentGateway)<>" + DB.SQuote(AppLogic.ro_PMPayPal) + "))", DB.SQuote(ddPaymentMethod.SelectedValue));
                }
                else if (PM == AppLogic.ro_PMPayPal)
                {
                    result += String.Format(" and (PaymentMethod={0} or upper(PaymentGateway)={1})", DB.SQuote(ddPaymentMethod.SelectedValue), DB.SQuote(AppLogic.ro_PMPayPal));
                }
                else if (PM == AppLogic.ro_PMPayPalExpress)
                {
                    result += String.Format(" and (PaymentMethod={0} or upper(PaymentGateway)={1})", DB.SQuote(ddPaymentMethod.SelectedValue), DB.SQuote(AppLogic.ro_PMPayPalExpress));
                }
                else if (PM == AppLogic.ro_PMPurchaseOrder)
                {
                    result += String.Format(sQuery, "PaymentMethod", DB.SQuote(ddPaymentMethod.SelectedValue));
                }
                else if (PM == AppLogic.ro_PMCODMoneyOrder)
                {
                    result += String.Format(sQuery, "PaymentMethod", DB.SQuote(ddPaymentMethod.SelectedValue));
                }
                else if (PM == AppLogic.ro_PMCODCompanyCheck)
                {
                    result += String.Format(sQuery, "PaymentMethod", DB.SQuote(ddPaymentMethod.SelectedValue));
                }
                else if (PM == AppLogic.ro_PMCODNet30)
                {
                    result += String.Format(sQuery, "PaymentMethod", DB.SQuote(ddPaymentMethod.SelectedValue));
                }
                else if (PM == AppLogic.ro_PMRequestQuote)
                {
                    result += String.Format(" and  (PaymentMethod={0} or QuotECheckout<>0)", DB.SQuote(ddPaymentMethod.SelectedValue));
                }
                else if (PM == AppLogic.ro_PMCheckByMail)
                {
                    result += String.Format(sQuery, "PaymentMethod", DB.SQuote(ddPaymentMethod.SelectedValue));
                }
                else if (PM == AppLogic.ro_PMCOD)
                {
                    result += String.Format(sQuery, "PaymentMethod", DB.SQuote(ddPaymentMethod.SelectedValue));
                }
                else if (PM == AppLogic.ro_PMECheck)
                {
                    result += String.Format(sQuery, "PaymentMethod", DB.SQuote(ddPaymentMethod.SelectedValue));
                }
                else if (PM == AppLogic.ro_PMMicropay)
                {
                    result += String.Format(sQuery, "PaymentMethod", DB.SQuote(ddPaymentMethod.SelectedValue));
                }
            }
            return result;
        }

        private void DoLocalization()
        {
            SectionTitle = "Manage/View Orders"; //AppLogic.GetString("orders.aspx.SectionTitle",SkinID,ThisCustomer.LocaleSetting);
            dateStart.Culture = Thread.CurrentThread.CurrentUICulture;
            dateEnd.Culture = Thread.CurrentThread.CurrentUICulture;
            dateStart.ClearDateText = AppLogic.GetString("txtClearDate", SkinID, ThisCustomer.LocaleSetting);
            dateEnd.ClearDateText = AppLogic.GetString("txtClearDate", SkinID, ThisCustomer.LocaleSetting);
            dateStart.GoToTodayText = AppLogic.GetString("txtTodaysDate", SkinID, ThisCustomer.LocaleSetting);
            dateEnd.GoToTodayText = AppLogic.GetString("txtTodaysDate", SkinID, ThisCustomer.LocaleSetting);
        }

        private void BulkPrintingGrid_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            GenerateReport().Dispose();
        }

        private void SummaryGrid_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // convert the long data fields to scrolling textarea fields, for compactness:
                foreach (TableCell c in e.Item.Cells)
                {
                    if (c.Text.Length > 50)
                    {
                        c.Text = "<textarea READONLY rows=\"12\" cols=\"50\">" + c.Text + "</textarea>";
                    }
                }
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
            }
        }


        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SummaryGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.SummaryGrid_ItemDataBound);

        }
        #endregion
    }
}
