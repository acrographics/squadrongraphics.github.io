// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/giftcards.aspx.cs 7     9/18/06 2:39p Jan Simacek $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{

    public partial class giftcards : System.Web.UI.Page
    {
        protected string selectSQL = "SELECT G.*, C.FirstName, C.LastName from GiftCard G with (NOLOCK) LEFT OUTER JOIN Customer C with (NOLOCK) ON G.PurchasedByCustomerID = C.CustomerID ";
        private Customer ThisCustomer;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            //temporarily hide, these are not a supported feature yet
            divForFilters.Visible = false;


            if (!IsPostBack)
            {
                string query = CommonLogic.QueryStringCanBeDangerousContent("searchfor");

                loadGroups();
                ViewState["Sort"] = "G.CreatedOn";
                ViewState["SortOrder"] = "DESC";
                ViewState["SQLString"] = selectSQL;

                if (query.Length > 0)
                {
                    resultFilter(query);
                }
                else
                {
                    retrieveFilters();
                    resultFilter("");// buildGridData(buildGridData());
                }

                txtSearch.Attributes.Add("onKeyPress", "javascript:if (event.keyCode == 13) __doPostBack('btnSearch','')");
            }
        }

        private void loadGroups()
        {
            try
            {
                ddStatus.Items.Clear();//hard coded
                ddTypes.Items.Clear();//from giftcard class

                // types
                ddTypes.Items.Add(new ListItem("SHOW ALL", "0"));
                ddTypes.Items.Add(new ListItem("Certificate", ((int)GiftCardTypes.CertificateGiftCard).ToString()));
                ddTypes.Items.Add(new ListItem("E-Mail", ((int)GiftCardTypes.EMailGiftCard).ToString()));
                ddTypes.Items.Add(new ListItem("Physical", ((int)GiftCardTypes.PhysicalGiftCard).ToString()));

                //status
                ddStatus.Items.Add(new ListItem("SHOW ALL", "0"));
                ddStatus.Items.Add(new ListItem("Expired", "1"));
                ddStatus.Items.Add(new ListItem("Active", "2"));
                ddStatus.Items.Add(new ListItem("Used At Least Once", "3"));
                ddStatus.Items.Add(new ListItem("Disabled", "4"));

            }
            catch (Exception ex)
            {
                resetError(ex.ToString(), true);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ViewState["IsInsert"] = false;
            gMain.EditIndex = -1;
            resetError("", false);
            gMain.PageIndex = 0;

            resultFilter("");
        }

        protected void resultFilter(string SearchFor)
        {
            String sql = selectSQL + " ";
            String temp = "";
            String WhereClause = String.Empty;

            //search
            if (SearchFor.Length == 0)
            {
                SearchFor = txtSearch.Text;
                if (SearchFor.Length != 0)
                {
                    if (ddSearch.SelectedValue == "1")
                    {
                        temp = " EMailTo LIKE " + DB.SQuote("%" + SearchFor + "%") + " ";
                    }
                    else if (ddSearch.SelectedValue == "2")
                    {
                        temp = " (LastName LIKE " + DB.SQuote("%" + SearchFor + "%") + " OR FirstName LIKE " + DB.SQuote("%" + SearchFor + "%") + ")";
                    }
                    else if (ddSearch.SelectedValue == "3")
                    {
                        temp = " SerialNumber LIKE " + DB.SQuote("%" + SearchFor + "%") + " ";
                    }
                    if (WhereClause.Length == 0)
                    {
                        WhereClause += " WHERE " + temp;
                    }
                    else
                    {
                        WhereClause += " AND " + temp;
                    }
                }
            }
            else
            {
                temp = " SerialNumber LIKE " + DB.SQuote("%" + SearchFor + "%") + " ";
                if (WhereClause.Length == 0)
                {
                    WhereClause += " WHERE " + temp;
                }
                else
                {
                    WhereClause += " AND " + temp;
                }
            }

            //Types
            if (ddTypes.SelectedValue != "0")
            {
                temp = " GiftCardTypeID=" + ddTypes.SelectedValue;
                if (WhereClause.Length == 0)
                {
                    WhereClause += " WHERE " + temp;
                }
                else
                {
                    WhereClause += " AND " + temp;
                }
            }

            //Status
            int status = Localization.ParseNativeInt(ddStatus.SelectedValue);
            if (status != 0)
            {
                if (status == 1)
                {
                    temp = " ExpirationDate < getdate()";
                }
                else if (status == 2)
                {
                    temp = " ExpirationDate >= getdate()";
                }
                else if (status == 3)
                {
                    temp = " GiftCardID in (select distinct GiftCardID from GiftCardUsage with (NOLOCK)) ";
                }
                else if (status == 4)
                {
                    temp = " DisabledByAdministrator = 1";
                }

                if (WhereClause.Length == 0)
                {
                    WhereClause += " WHERE " + temp;
                }
                else
                {
                    WhereClause += " AND " + temp;
                }
            }

            sql += WhereClause;

            //set states
            ViewState["SQLString"] = sql.ToString();
            sql += " ORDER BY " + ViewState["Sort"].ToString() + " " + ViewState["SortOrder"].ToString();

            //build grid
            buildGridData(DB.GetDS(sql, false));

            //if need to return to page
            setFilters(ddStatus.SelectedValue, ddTypes.SelectedValue, ViewState["Sort"].ToString(), ViewState["SortOrder"].ToString(), SearchFor);

            txtSearch.Text = SearchFor;
        }

        protected void treeMain_SelectedNodeChanged(object sender, EventArgs e)
        {
            ViewState["IsInsert"] = false;
            gMain.EditIndex = -1;
            resetError("", false);
            gMain.PageIndex = 0;

            resultFilter("");
        }

        protected void ddTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            preFilter();
        }

        protected void ddStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            preFilter();
        }
        protected void ddForProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            preFilter();
        }
        protected void ddForCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            preFilter();
        }
        protected void ddForSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            preFilter();
        }
        protected void ddForManufacturer_SelectedIndexChanged(object sender, EventArgs e)
        {
            preFilter();
        }

        protected void preFilter()
        {
            ViewState["IsInsert"] = false;
            gMain.EditIndex = -1;
            resetError("", false);
            gMain.PageIndex = 0;

            resultFilter("");
        }

        protected DataSet buildGridData()
        {
            string sql = ViewState["SQLString"].ToString();
            sql += " order by " + ViewState["Sort"].ToString() + " " + ViewState["SortOrder"].ToString();

            return DB.GetDS(sql, false);
        }

        protected void buildGridData(DataSet ds)
        {
            gMain.DataSource = ds;
            gMain.DataBind();
            ds.Dispose();
        }

        protected void resetError(string error, bool isError)
        {
            string str = "<font class=\"noticeMsg\">NOTICE:</font>&nbsp;&nbsp;&nbsp;";
            if (isError)
                str = "<font class=\"errorMsg\">ERROR:</font>&nbsp;&nbsp;&nbsp;";

            if (error.Length > 0)
                str += error + "";
            else
                str = "";

            ((Literal)Form.FindControl("ltError")).Text = str;
        }

        protected void gMain_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView myrow = (DataRowView)e.Row.DataItem;

                LinkButton action = (LinkButton)e.Row.FindControl("lnkAction");
                if (myrow["DisabledByAdministrator"].ToString() == "1")
                {
                    action.CommandArgument = "0|" + myrow["GiftCardID"].ToString();
                    action.Attributes.Add("onClick", "javascript: return confirm('Are you sure you want to enable this Gift Card?')");
                    action.Text = "ENABLE";
                }
                else
                {
                    action.CommandArgument = "1|" + myrow["GiftCardID"].ToString();
                    action.Attributes.Add("onClick", "javascript: return confirm('Are you sure you want to disable this Gift Card?')");
                    action.Text = "DISABLE";
                }

                int type = Localization.ParseNativeInt(myrow["GiftCardTypeID"].ToString());
                Literal lt = (Literal)e.Row.FindControl("ltCardType");
                lt.Text = GiftCard.s_GetCardType(type);
                if (((int)GiftCardTypes.EMailGiftCard) == type)
                {
                    lt.Text += "&nbsp;<a href=\"javascript:;\" onclick=\"window.open('giftcardview.aspx?iden=" + myrow["GiftCardID"].ToString() + "','View','width=500,height=300,resizable=yes, toolbar=no, scrollbars=yes, status=yes, location=no, directories=no, menubar=no, alwaysRaised=yes');\">View</a>";
                }

                string amount = Localization.CurrencyStringForDisplayWithoutExchangeRate(Localization.ParseNativeCurrency(myrow["InitialAmount"].ToString()));
                Literal ltAmount = (Literal)e.Row.FindControl("ltInitialAmount");
                ltAmount.Text = amount;

                string balance = Localization.CurrencyStringForDisplayWithoutExchangeRate(Localization.ParseNativeCurrency(myrow["Balance"].ToString()));
                Literal ltBalance = (Literal)e.Row.FindControl("ltBalance");
                ltBalance.Text = balance;
            }
        }
        protected void gMain_Sorting(object sender, GridViewSortEventArgs e)
        {
            ViewState["IsInsert"] = false;
            gMain.EditIndex = -1;
            ViewState["Sort"] = e.SortExpression.ToString();
            ViewState["SortOrder"] = (ViewState["SortOrder"].ToString() == "ASC" ? "DESC" : "ASC");
            buildGridData(buildGridData());
        }
        protected void gMain_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            resetError("", false);

            if (e.CommandName == "ItemAction")
            {
                ViewState["IsInsert"] = false;
                gMain.EditIndex = -1;
                string temp = e.CommandArgument.ToString();
                int action = Localization.ParseNativeInt(temp.Substring(0, 1));
                int iden = Localization.ParseNativeInt(temp.Substring(temp.IndexOf("|") + 1));
                updateRow(iden, action);
            }
        }
        protected void updateRow(int iden, int action)
        {
            try
            {
                DB.ExecuteSQL("UPDATE GiftCard SET DisabledByAdministrator=" + action + " WHERE GiftCardID=" + iden.ToString());

                loadGroups();
                buildGridData(buildGridData());
                resetError("Gift Card Was " + (action == 1 ? "Dis" : "En") + "abled", false);
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't update database: " + ex.ToString());
            }
        }

        protected void gMain_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["IsInsert"] = false;
            resetError("", false);
            gMain.PageIndex = e.NewPageIndex;
            gMain.EditIndex = -1;
            buildGridData(buildGridData());
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            ViewState["IsInsert"] = false;
            gMain.EditIndex = -1;

            ViewState["Sort"] = "GiftCardID";
            ViewState["SortOrder"] = "DESC";

            //create new gift card
            Response.Redirect("editgiftcard.aspx?iden=0");
        }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            //ddForCategory.ClearSelection();
            //ddForManufacturer.ClearSelection();
            //ddForProduct.ClearSelection();
            //ddForSection.ClearSelection();
            //ddSearch.ClearSelection();
            ddStatus.ClearSelection();
            ddTypes.ClearSelection();

            preFilter();
        }

        protected void setFilters(string status, string types, string sort, string order, string search)
        {
            ThisCustomer.ThisCustomerSession.SetVal("GCStatus", status);
            ThisCustomer.ThisCustomerSession.SetVal("GCTypes", types);
            ThisCustomer.ThisCustomerSession.SetVal("GCSort", sort);
            ThisCustomer.ThisCustomerSession.SetVal("GCOrder", order);
            ThisCustomer.ThisCustomerSession.SetVal("GCSearch", search);
        }

        protected void retrieveFilters()
        {
            //set states
            if (ThisCustomer.ThisCustomerSession.Session("GCSearch").Length > 0)
            {
                txtSearch.Text = ThisCustomer.ThisCustomerSession.Session("GCSearch");
            }
            if (ThisCustomer.ThisCustomerSession.Session("GCStatus").Length > 0)
            {
                ddStatus.Items.FindByValue(ThisCustomer.ThisCustomerSession.Session("GCStatus")).Selected = true;
            }
            if (ThisCustomer.ThisCustomerSession.Session("GCTypes").Length > 0)
            {
                ddTypes.Items.FindByValue(ThisCustomer.ThisCustomerSession.Session("GCTypes")).Selected = true;
            }
            if (ThisCustomer.ThisCustomerSession.Session("GCSort").Length > 0)
            {
                string temp = ThisCustomer.ThisCustomerSession.Session("GCSort");
                ViewState["Sort"] = temp;
            }
            if (ThisCustomer.ThisCustomerSession.Session("GCOrder").Length > 0)
            {
                string temp = ThisCustomer.ThisCustomerSession.Session("GCOrder");
                ViewState["SortOrder"] = temp;
            }
        }
    }
}