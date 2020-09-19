// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/giftcardusage.aspx.cs 2     8/19/06 8:50p Buddy $
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

    public partial class giftcardusage : System.Web.UI.Page
    {
        protected string selectSQL = "select G.*, C.LastName, C.FirstName FROM GiftCardUsage G with (NOLOCK) left outer join Customer C with (NOLOCK) " +
            " on G.UsedByCustomerID=C.CustomerID WHERE G.GiftCardID = " + CommonLogic.QueryStringCanBeDangerousContent("iden");
        private Customer cust;
        private int GiftCardID;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
            GiftCardID = CommonLogic.QueryStringNativeInt("iden");

            if (!IsPostBack)
            {
                ltCard.Text = DB.GetSqlS("SELECT SerialNumber AS S FROM GiftCard WHERE GiftCardID = " + CommonLogic.QueryStringCanBeDangerousContent("iden"));
                ViewState["Sort"] = "G.CreatedOn";
                ViewState["SortOrder"] = "DESC";
                ViewState["SQLString"] = selectSQL;

                buildGridData(buildGridData());
            }
        }

        protected DataSet buildGridData()
        {
            resetError("", false);

            string sql = ViewState["SQLString"].ToString();
            sql += " order by " + ViewState["Sort"].ToString() + " " + ViewState["SortOrder"].ToString();

            return DB.GetDS(sql, false);
        }

        protected void buildGridData(DataSet ds)
        {
            gMain.DataSource = ds;
            gMain.DataBind();

            if (ds.Tables[0].Rows.Count == 0)
            {
                resetError("No activity found.", false);
            }
            ds.Dispose();

            ltBalance.Text = Localization.CurrencyStringForDisplayWithoutExchangeRate(DB.GetSqlNDecimal("SELECT Balance AS N FROM GiftCard WHERE GiftCardID=" + GiftCardID));
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

                string amount = Localization.CurrencyStringForDisplayWithoutExchangeRate(Localization.ParseNativeCurrency(myrow["Amount"].ToString()));
                Literal ltAmount = (Literal)e.Row.FindControl("ltAmount");
                ltAmount.Text = amount;
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

        protected void gMain_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ViewState["IsInsert"] = false;
            resetError("", false);
            gMain.PageIndex = e.NewPageIndex;
            gMain.EditIndex = -1;
            buildGridData(buildGridData());
        }
        protected void btnUsage_Click(object sender, EventArgs e)
        {
            resetError("", false);

            StringBuilder sql = new StringBuilder(1024);
            decimal amnt = Localization.ParseUSDecimal(txtUsage.Text);
            int action = Localization.ParseNativeInt(ddUsage.SelectedValue);

            string NewGUID = DB.GetNewGUID();
            sql.Append("INSERT INTO GiftCardUsage (GiftCardUsageGUID,GiftCardID,UsageTypeID,UsedByCustomerID,Amount) VALUES(");
            sql.Append(DB.SQuote(NewGUID) + ",");
            sql.Append(GiftCardID + ",");
            sql.Append(action + ",");
            sql.Append(cust.CustomerID + ",");
            sql.Append(Localization.CurrencyStringForDBWithoutExchangeRate(amnt) + "");
            
            sql.Append(")");
            DB.ExecuteSQL(sql.ToString());

            //update the gift card
            DB.ExecuteSQL("UPDATE GiftCard SET Balance = Balance" + (action == 3 ? "+" + amnt : "-" + amnt) + " WHERE GiftCardID=" + GiftCardID);

            ViewState["Sort"] = "G.CreatedOn";
            ViewState["SortOrder"] = "DESC";

            buildGridData(buildGridData());
            resetError("Gift Card Usage added.", false);
            
            // reset form fields:
            txtUsage.Text = String.Empty;
            ddUsage.SelectedIndex = 0;
        }
}
}