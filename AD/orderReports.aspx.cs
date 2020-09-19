// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/orderReports.aspx.cs 8     9/19/06 12:18a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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
    public partial class orderReports : System.Web.UI.Page
    {
        protected Customer cust;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            dateStart.Culture = new CultureInfo(Localization.GetWebConfigLocale());
            dateEnd.Culture = new CultureInfo(Localization.GetWebConfigLocale());
            if (!IsPostBack)
            {
                IDataReader rsd = DB.GetRS("Select min(OrderDate) as MinDate from orders " + DB.GetNoLock());
                DateTime MinOrderDate = Localization.ParseUSDateTime("1/1/1990");
                if (rsd.Read())
                {
                    MinOrderDate = DB.RSFieldDateTime(rsd, "MinDate");
                }
                rsd.Close();
                dateStart.SelectedDate = MinOrderDate;

                dateEnd.SelectedDate = DateTime.Now;

                dateStart.Culture = Thread.CurrentThread.CurrentUICulture;
                dateEnd.Culture = Thread.CurrentThread.CurrentUICulture;
                dateStart.ClearDateText = AppLogic.GetString("txtClearDate", CommonLogic.QueryStringUSInt("SkinID"), cust.LocaleSetting);
                dateEnd.ClearDateText = AppLogic.GetString("txtClearDate", CommonLogic.QueryStringUSInt("SkinID"), cust.LocaleSetting);
                dateStart.GoToTodayText = AppLogic.GetString("txtTodaysDate", CommonLogic.QueryStringUSInt("SkinID"), cust.LocaleSetting);
                dateEnd.GoToTodayText = AppLogic.GetString("txtTodaysDate", CommonLogic.QueryStringUSInt("SkinID"), cust.LocaleSetting);
                loadDD();
                phMain.Visible = false;
            }
        }

        protected void loadDD()
        {
            ddCategory.Items.Clear();
            ddSection.Items.Clear();
            ddManufacturer.Items.Clear();
            //ddType.Items.Clear();

            ddCategory.Items.Add(new ListItem(" - All Categories - ", "0"));
            ddSection.Items.Add(new ListItem(" - All Sections - ", "0"));
            ddManufacturer.Items.Add(new ListItem(" - All Manufacturers -", "0"));
            //ddType.Items.Add(new ListItem(" - All Product Types -", "0"));

            //Categories
            EntityHelper Helper = new EntityHelper(EntityDefinitions.LookupSpecs("Category"));
            ArrayList al = Helper.GetEntityArrayList(0, String.Empty, 0, cust.LocaleSetting, false);
            foreach (ListItemClass li in al)
            {
                ddCategory.Items.Add(new ListItem(li.Item, li.Value.ToString()));
            }

            //Sections
            Helper = new EntityHelper(EntityDefinitions.LookupSpecs("Section"));
            al = Helper.GetEntityArrayList(0, String.Empty, 0, cust.LocaleSetting, false);
            foreach (ListItemClass li in al)
            {
                ddSection.Items.Add(new ListItem(li.Item, li.Value.ToString()));
            }

            //Products
            //loadProducts();

            //Manufacturers
            Helper = new EntityHelper(EntityDefinitions.LookupSpecs("Manufacturer"));
            al = Helper.GetEntityArrayList(0, String.Empty, 0, cust.LocaleSetting, false);
            foreach (ListItemClass li in al)
            {
                ddManufacturer.Items.Add(new ListItem(li.Item, li.Value.ToString()));
            }
        }

        protected void resetError(string error, bool isError)
        {
            string str = "<font class=\"noticeMsg\">NOTICE:</font>&nbsp;&nbsp;&nbsp;";
            if (isError)
            {
                str = "<font class=\"errorMsg\">ERROR:</font>&nbsp;&nbsp;&nbsp;";
            }

            if (error.Length > 0)
            {
                str += error + "";
            }
            else
            {
                str = "";
            }
            ltError.Text = str;
        }

        protected void btnReport_Click(object sender, EventArgs e)
        {
            ltMode.Text = "";
            resetError("", false);

            ltReport.Text = "";
            phMain.Visible = true;
            string select = "SELECT P.ProductID, P.Name, PV.VariantID, PV.Name AS Variant, PV.Price, P.Description, O.OrderDate, O.OrderNumber, OSC.Quantity";
            string selectC = "";
            string selectS = "";

            string from = "FROM Product P, Orders O, Orders_ShoppingCart OSC, ProductVariant PV";
            string fromC = "";
            string fromS = "";

            string where = "WHERE P.ProductID=OSC.ProductID AND OSC.VariantID=PV.VariantID AND OSC.OrderNumber=O.OrderNumber ";
            where += DateClause();
            string whereC = "";
            string whereS = "";
            string whereP = "";

            string order = "";
            string orderC = "";
            string orderS = "";

            string temp = "";
            string temp1 = "";
            string categorized = "";
            string sectionalized = "";
            string statistical = "";

            if (!ddCategory.SelectedValue.Equals("0"))
            {
                //Category selected
                selectC += ", C.CategoryID, C.Name AS CategoryName";
                fromC += ", Category C, ProductCategory PC";
                whereC += " AND C.CategoryID=" + ddCategory.SelectedValue + " AND C.CategoryID=PC.CategoryID AND PC.ProductID=P.ProductID";
                orderC += " C.Name ASC, ";
            }

            if (!ddSection.SelectedValue.Equals("0"))
            {
                //Section selected
                selectS += ", S.SectionID, S.Name AS SectionName";
                fromS += ", Section S, ProductSection PS";
                whereS += " AND S.SectionID=" + ddSection.SelectedValue + " AND S.SectionID=PS.SectionID AND PS.ProductID=P.ProductID";
                orderS += " S.Name ASC, ";
            }

            //if (!ddProduct.SelectedValue.Equals("0"))
            //{
            //    //Product selected
            //    whereP += " AND P.ProductID=" + ddProduct.SelectedValue;
            //}

            order = "ORDER BY " + orderC + orderS + " P.Name ASC";

            string sql = select + selectC + selectS + " " + from + fromC + fromS + " " + where + whereC + whereS + whereP;

            if (!ddCategory.SelectedValue.Equals("0"))
            {
                ltMode.Text += " AND '" + ddCategory.SelectedItem.Text + "'";
            }
            if (!ddSection.SelectedValue.Equals("0"))
            {
                ltMode.Text += " AND '" + ddSection.SelectedItem.Text + "'";
            }
            //if (!ddProduct.SelectedValue.Equals("0"))
            //{
            //    ltMode.Text += " AND '" + ddProduct.SelectedItem.Text + "'";
            //}

            if (ltMode.Text.Length > 4)
            {
                ltMode.Text = ltMode.Text.Substring(4);
            }
            else
            {
                ltMode.Text = "Date Range";
            }

            try
            {
                if (!ddCategory.SelectedValue.Equals("0"))
                {
                    temp = "SELECT DISTINCT T.CategoryID, T.CategoryName, SUM(T.Quantity) AS Total, SUM(T.Quantity*T.Price) AS Revenue FROM (" + sql + ") T GROUP BY T.CategoryID, T.CategoryName ";
                    DataSet dsC = DB.GetDS(temp, false);
                    if (dsC.Tables[0].Rows.Count > 0)
                    {
                        DataRow rowC = dsC.Tables[0].Rows[0];
                        ltReport.Text += "<table width=\"100%\" cellpadding='0' cellspacing='0' border='0' id='reportTable'><tr class='reportOuter'><td><table width=\"100%\" cellpadding='3' cellspacing='1' border='0'>";
                        ltReport.Text += "<tr class='reportTitle'><td colspan='3'><!--" + DB.RowFieldInt(rowC, "CategoryID") + "-->Category: " + DB.RowFieldByLocale(rowC, "CategoryName", cust.LocaleSetting) + "</td><td>Total: " + cust.CurrencyString(DB.RowFieldDecimal(rowC, "Revenue")) + "</td><td>Quantity: " + DB.RowFieldInt(rowC, "Total") + "</td></tr>";

                        temp1 = select + selectC + selectS + " " + from + fromC + fromS + " " + where + " AND C.CategoryID=" + DB.RowFieldInt(rowC, "CategoryID") + " AND C.CategoryID=PC.CategoryID AND PC.ProductID=P.ProductID" + whereS + whereP + " " + order; ;
                        DataSet ds = DB.GetDS(temp1, false);

                        ltReport.Text += "<tr class='reportHeader'><td>Name</td><td>Variant</td><td>Order</td><td>Date</td><td>Quantity</td></tr>";

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            int pID = 0;
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                ltReport.Text += "<tr class='reportRow'><td><!--" + DB.RowFieldInt(row, "ProductID") + "-->" + (pID == DB.RowFieldInt(row, "ProductID") ? "" : DB.RowFieldByLocale(row, "Name", cust.LocaleSetting)) + "</td><td>" + DB.RowFieldByLocale(row, "Variant", cust.LocaleSetting) + "</td><td><!--" + DB.RowField(row, "Description").ToString() + "-->" + DB.RowFieldInt(row, "OrderNumber") + "</td><td>" + Localization.ToNativeDateTimeString(DB.RowFieldDateTime(row, "OrderDate")) + "</td><td>" + DB.RowFieldInt(row, "Quantity") + "</td></tr>";
                                pID = DB.RowFieldInt(row, "ProductID");
                            }
                        }
                        else
                        {
                            ltReport.Text = "<font class=\"errorMsg\">No Orders found for Category. Please try again.</font>";
                        }
                        ltReport.Text += "</table></td></tr></table><br/>";
                        ds.Dispose();
                    }
                    dsC.Dispose();
                }
                if (!ddSection.SelectedValue.Equals("0"))
                {
                    temp = "SELECT DISTINCT T.SectionID, T.SectionName, SUM(T.Quantity) AS Total, SUM(T.Quantity*T.Price) AS Revenue  FROM (" + sql + ") T GROUP BY T.SectionID, T.SectionName ";
                    DataSet dsS = DB.GetDS(temp, false);
                    if (dsS.Tables[0].Rows.Count > 0)
                    {
                        DataRow rowS = dsS.Tables[0].Rows[0];
                        ltReport.Text += "<table width=\"100%\" cellpadding='0' cellspacing='0' border='0' id='reportTable'><tr class='reportOuter'><td><table width=\"100%\" cellpadding='3' cellspacing='1' border='0'>";
                        ltReport.Text += "<tr class='reportTitle'><td colspan='3'><!--" + DB.RowFieldInt(rowS, "SectionID") + "-->Section: " + DB.RowFieldByLocale(rowS, "SectionName", cust.LocaleSetting) + "</td><td>Total: " + cust.CurrencyString(DB.RowFieldDecimal(rowS, "Revenue")) + "</td><td>Quantity: " + DB.RowFieldInt(rowS, "Total") + "</td></tr>";

                        temp1 = select + selectC + selectS + " " + from + fromC + fromS + " " + where + " AND S.SectionID=" + DB.RowFieldInt(rowS, "SectionID") + " AND S.SectionID=PS.SectionID AND PS.ProductID=P.ProductID" + whereC + whereP + " " + order; ;
                        DataSet ds = DB.GetDS(temp1, false);

                        ltReport.Text += "<tr class='reportHeader'><td>Name</td><td>Variant</td><td>Order</td><td>Date</td><td>Quantity</td></tr>";

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            int pID = 0;
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                ltReport.Text += "<tr class='reportRow'><td><!--" + DB.RowFieldInt(row, "ProductID") + "-->" + (pID == DB.RowFieldInt(row, "ProductID") ? "" : DB.RowFieldByLocale(row, "Name", cust.LocaleSetting)) + "</td><td>" + DB.RowFieldByLocale(row, "Variant", cust.LocaleSetting) + "</td><td><!--" + DB.RowField(row, "Description").ToString() + "-->" + DB.RowFieldInt(row, "OrderNumber") + "</td><td>" + Localization.ToNativeDateTimeString(DB.RowFieldDateTime(row, "OrderDate")) + "</td><td>" + DB.RowFieldInt(row, "Quantity") + "</td></tr>";
                                pID = DB.RowFieldInt(row, "ProductID");
                            }
                        }
                        else
                        {
                            ltReport.Text = "<font class=\"errorMsg\">No Orders found for Section. Please try again.</font>";
                        }
                        ltReport.Text += "</table></td></tr></table><br/>";
                        ds.Dispose();
                    }
                    dsS.Dispose();
                }
            }
            catch (Exception ex)
            {
                resetError("TEMP: " + temp + "<br/>TEMP1: " + temp1 + "<br/>ERROR: " + ex.ToString(), true);
            }

            // FOR All Categories
            if (ddCategory.SelectedValue.Equals("0") && ddSection.SelectedValue.Equals("0"))
            {
                try
                {
                    categorized = "SELECT S.*, C.CategoryID, C.Name AS CategoryName FROM (" + sql + ") S, Category C, ProductCategory PC WHERE C.CategoryID=PC.CategoryID AND PC.ProductID=S.ProductID ";
                    statistical = "SELECT DISTINCT T.CategoryID, T.CategoryName, SUM(T.Quantity) AS Total, SUM(T.Quantity*T.Price) AS Revenue  FROM (" + categorized + ") T GROUP BY T.CategoryID, T.CategoryName ";
                    DataSet dsC = DB.GetDS(statistical, false);
                    if (dsC.Tables[0].Rows.Count > 0)
                    {
                        ltReport.Text += "<table width=\"100%\" cellpadding='0' cellspacing='0' border='0' id='reportTable'><tr><td><b>Categories:</b></td></tr><tr class='reportOuter'><td><table width=\"100%\" cellpadding='2' cellspacing='1' border='0'>";
                        int count = 0;

                        foreach (DataRow rowC in dsC.Tables[0].Rows)
                        {
                            ltReport.Text += "<tr class='reportTitle' onClick=\"expandcontent('scC" + count + "')\" style=\"cursor:hand; cursor:pointer\"><td><!--Category: -->" + DB.RowFieldByLocale(rowC, "CategoryName", cust.LocaleSetting) + "</td><td>Total: " + cust.CurrencyString(DB.RowFieldDecimal(rowC, "Revenue")) + "</td><td>Quantity: " + DB.RowFieldInt(rowC, "Total") + "</td></tr>";

                            ltReport.Text += "<tr><td colspan='3' width=\"100%\"><div style=\"width: 100%;\" id=\"scC" + count + "\" class=\"switchcontent\">";

                            temp = select + " " + from + fromS + ", Category C, ProductCategory PC " + where + whereS + whereP + " AND C.CategoryID=" + DB.RowFieldInt(rowC, "CategoryID") + " AND C.CategoryID=PC.CategoryID AND PC.ProductID=P.ProductID " + order;
                            DataSet ds = DB.GetDS(temp, false);

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                int pID = 0;
                                ltReport.Text += "<table cellpadding=\"2\" cellspacing=\"1\" border=\"0\"><tr class='reportHeader'><td>Name</td><td>Variant</td><td>Order</td><td>Date</td><td>Quantity</td></tr>";

                                foreach (DataRow row in ds.Tables[0].Rows)
                                {
                                    ltReport.Text += "<tr class='reportRow'><td>" + (pID == DB.RowFieldInt(row, "ProductID") ? "" : DB.RowFieldByLocale(row, "Name", cust.LocaleSetting)) + "</td><td>" + DB.RowFieldByLocale(row, "Variant", cust.LocaleSetting) + "</td><td>" + DB.RowFieldInt(row, "OrderNumber") + "</td><td>" + Localization.ToNativeDateTimeString(DB.RowFieldDateTime(row, "OrderDate")) + "</td><td>" + DB.RowFieldInt(row, "Quantity") + "</td></tr>";
                                    pID = DB.RowFieldInt(row, "ProductID");
                                }
                            }
                            else
                            {
                                ltReport.Text = "<font class=\"errorMsg\">No Orders found for Category. Please try again.</font>";
                            }

                            count++;
                            ltReport.Text += "</table></div></td></tr>";
                            ds.Dispose();

                        }
                        ltReport.Text += "</table></td></tr></table><br/>";
                    }
                    dsC.Dispose();
                }
                catch (Exception ex)
                {
                    resetError("STATISTICAL: " + statistical + "<br/><br/>TEMP: " + temp + "<br/><br/>ERROR: " + ex.ToString(), true);
                }
            }

            // FOR ALL Sections
            if (ddSection.SelectedValue.Equals("0") && ddCategory.SelectedValue.Equals("0"))
            {
                try
                {
                    sectionalized = "SELECT X.*, S.SectionID, S.Name AS SectionName FROM (" + sql + ") X, Section S, ProductSection PS WHERE S.SectionID=PS.SectionID AND PS.ProductID=X.ProductID ";
                    statistical = "SELECT DISTINCT T.SectionID, T.SectionName, SUM(T.Quantity) AS Total, SUM(T.Quantity*T.Price) AS Revenue  FROM (" + sectionalized + ") T GROUP BY T.SectionID, T.SectionName ";
                    DataSet dsS = DB.GetDS(statistical, false);
                    if (dsS.Tables[0].Rows.Count > 0)
                    {
                        ltReport.Text += "<table width=\"100%\" cellpadding='0' cellspacing='0' border='0' id='reportTable'><tr><td><b>Sections:</b></td></tr><tr class='reportOuter'><td><table width=\"100%\" cellpadding='2' cellspacing='1' border='0'>";
                        int count = 0;

                        foreach (DataRow rowS in dsS.Tables[0].Rows)
                        {
                            ltReport.Text += "<tr class='reportTitle' onClick=\"expandcontent('scS" + count + "')\" style=\"cursor:hand; cursor:pointer\"><td><!--Section: -->" + DB.RowFieldByLocale(rowS, "SectionName", cust.LocaleSetting) + "</td><td>Total: " + cust.CurrencyString(DB.RowFieldDecimal(rowS, "Revenue")) + "</td><td>Quantity: " + DB.RowFieldInt(rowS, "Total") + "</td></tr>";

                            ltReport.Text += "<tr><td colspan='3' width=\"100%\"><div style=\"width: 100%;\" id=\"scS" + count + "\" class=\"switchcontent\">";

                            temp = select + " " + from + fromC + ", Section S, ProductSection PS " + where + whereC + whereP + " AND S.SectionID=" + DB.RowFieldInt(rowS, "SectionID") + " AND S.SectionID=PS.SectionID AND PS.ProductID=P.ProductID " + order;
                            DataSet ds = DB.GetDS(temp, false);

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                int pID = 0;
                                ltReport.Text += "<table cellpadding=\"2\" cellspacing=\"1\" border=\"0\"><tr class='reportHeader'><td>Name</td><td>Variant</td><td>Order</td><td>Date</td><td>Quantity</td></tr>";

                                foreach (DataRow row in ds.Tables[0].Rows)
                                {
                                    ltReport.Text += "<tr class='reportRow'><td>" + (pID == DB.RowFieldInt(row, "ProductID") ? "" : DB.RowFieldByLocale(row, "Name", cust.LocaleSetting)) + "</td><td>" + DB.RowFieldByLocale(row, "Variant", cust.LocaleSetting) + "</td><td>" + DB.RowFieldInt(row, "OrderNumber") + "</td><td>" + Localization.ToNativeDateTimeString(DB.RowFieldDateTime(row, "OrderDate")) + "</td><td>" + DB.RowFieldInt(row, "Quantity") + "</td></tr>";
                                    pID = DB.RowFieldInt(row, "ProductID");
                                }
                            }
                            else
                            {
                                ltReport.Text = "<font class=\"errorMsg\">No Orders found for Section. Please try again.</font>";
                            }

                            count++;
                            ltReport.Text += "</table></div></td></tr>";
                            ds.Dispose();
                        }
                        ltReport.Text += "</table></td></tr></table><br/>";
                    }
                    dsS.Dispose();
                }
                catch (Exception ex)
                {
                    resetError("STATISTICAL: " + statistical + "<br/><br/>TEMP: " + temp + "<br/><br/>ERROR: " + ex.ToString(), true);
                }
            }

            if (true)//ddProduct.SelectedValue.Equals("0"))
            {
                try
                {
                    statistical = "SELECT T.ProductID, T.Name, T.VariantID, T.Variant, SUM(T.Quantity) AS Total, MIN(T.Quantity) AS Minimum, MAX(T.Quantity) AS Maximum, AVG(T.Quantity) AS Average, SUM(T.Quantity*T.Price) AS Revenue  FROM (" + sql + ") T GROUP BY T.ProductID, T.Name, T.VariantID, T.Variant ";
                    DataSet dsP = DB.GetDS(statistical, false);
                    if (dsP.Tables[0].Rows.Count > 0)
                    {
                        ltReport.Text += "<table width=\"100%\" cellpadding='0' cellspacing='0' border='0' id='reportTable'><tr class='reportOuter'><td><table width=\"100%\" cellpadding='3' cellspacing='1' border='0'>";
                        ltReport.Text += "<tr class='reportTitle'><td colspan='7'>Product Statistics</td></tr>";
                        int pID = 0;
                        ltReport.Text += "<tr class='reportHeader'><td>Name</td><td>Variant</td><td>Quantity</td><td>Total</td><td>Average</td><td>Minimum</td><td>Maximum</td></tr>";

                        foreach (DataRow rowP in dsP.Tables[0].Rows)
                        {
                            ltReport.Text += "<tr class='reportRow'><td>" + DB.RowFieldByLocale(rowP, "Name", cust.LocaleSetting) + "</td><td>" + DB.RowFieldByLocale(rowP, "Variant", cust.LocaleSetting) + "</td><td>" + DB.RowFieldInt(rowP, "Total") + "</td><td>" + cust.CurrencyString(DB.RowFieldDecimal(rowP, "Revenue")) + "</td><td>" + DB.RowFieldDouble(rowP, "Average") + "</td><td>" + DB.RowFieldInt(rowP, "Minimum") + "</td><td>" + DB.RowFieldInt(rowP, "Maximum") + "</td></tr>";
                            pID = DB.RowFieldInt(rowP, "ProductID");
                        }
                        ltReport.Text += "</table></td></tr></table><br/>";
                    }
                    dsP.Dispose();
                }
                catch (Exception ex)
                {
                    resetError("STATISTICAL: " + statistical + "<br/><br/>ERROR: " + ex.ToString(), true);
                }
            }

            if (ltReport.Text.Trim().Length == 0)
                ltReport.Text = "<font class=\"errorMsg\">No Order information found for specifications. Please try again.</font>";
        }

        protected string DateClause()
        {
            string result = "";
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            
            switch (rblRange.SelectedValue)
            {
                case "0":
                    {
                        if (dateStart.SelectedDate.CompareTo(dateEnd.SelectedDate) > 0) //Flip them
                        {
                            
                            endDate = dateStart.SelectedDate;
                            dateStart.SelectedDate = dateEnd.SelectedDate;
                            dateEnd.SelectedDate = endDate;
                        }

                        startDate = dateStart.SelectedDate;
                        endDate = dateEnd.SelectedDate;

                        break;
                    }
                case "1":
                    {
                        startDate = DateTime.Today;
                        endDate = startDate;
                        break;
                    }
                case "2":
                    {
                        startDate = DateTime.Today.AddDays(-1);
                        endDate = startDate;
                        break;
                    }
                case "3":
                    {
                        startDate = DateTime.Today.AddDays(-((int)DateTime.Today.DayOfWeek));
                        endDate = startDate.AddDays(6);
                        break;
                    }
                case "4":
                    {
                        startDate = DateTime.Today.AddDays(-((int)DateTime.Today.DayOfWeek) - 7);
                        endDate = startDate.AddDays(6);
                        break;
                    }
                case "5":
                    {
                        startDate = DateTime.Today.AddDays(1 - DateTime.Today.Day);
                        endDate = startDate.AddMonths(1);
                        break;
                    }
                case "6":
                    {
                        startDate = DateTime.Today.AddMonths(-1);
                        startDate = startDate.AddDays(1 - startDate.Day);
                        endDate = startDate.AddMonths(1);
                        break;
                    }
                case "7":
                    {
                        startDate = DateTime.Today.AddMonths(1 - DateTime.Today.Month);
                        startDate = startDate.AddDays(1 - startDate.Day);
                        endDate = startDate.AddYears(1);
                        break;
                    }
                case "8":
                    {
                        startDate = DateTime.Today.AddYears(-1);
                        startDate = startDate.AddMonths(1 - startDate.Month);
                        startDate = startDate.AddDays(1 - startDate.Day);
                        endDate = startDate.AddYears(1);
                        break;
                    }
            }

            result = CommonLogic.IIF(startDate <= SqlDateTime.MinValue.Value || startDate >=  SqlDateTime.MaxValue.Value, "", " O.OrderDate>=" + DB.DateQuote(Localization.ToDBShortDateString(startDate)));
            result += CommonLogic.IIF(endDate <= SqlDateTime.MinValue.Value || endDate >= SqlDateTime.MaxValue.Value, "", " and O.OrderDate < " + DB.DateQuote(Localization.ToDBShortDateString(endDate.AddDays(1))));

            if (result.Length > 0)
            {
                result = " and " + result;
            }
            return result;
        }
        protected void ddManufacturer_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetError("", false);
        }
        protected void ddType_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetError("", false);
        }

    }
}