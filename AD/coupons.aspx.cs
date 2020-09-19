// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/coupons.aspx.cs 5     9/18/06 2:39p Jan Simacek $
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

public partial class coupons : System.Web.UI.Page
{
    protected string selectSQL = "select * from Coupon";
    private Customer ThisCustomer;
        
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");

        ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

        if (!IsPostBack)
        {
            string query = CommonLogic.QueryStringCanBeDangerousContent("searchfor");

            this.loadTree();

            ViewState["SQLString"] = this.selectSQL;

            //set page settings
            if (ThisCustomer.ThisCustomerSession.Session("CouponsSort").Length == 0)
            {
                ViewState["Sort"] = "CouponCode";
            }
            else
            {
                ViewState["Sort"] = ThisCustomer.ThisCustomerSession.Session("CouponsSort");
            }
            if (ThisCustomer.ThisCustomerSession.Session("CouponsOrder").Length == 0)
            {
                ViewState["SortOrder"] = "ASC";
            }
            else
            {
                ViewState["SortOrder"] = ThisCustomer.ThisCustomerSession.Session("CouponsOrder"); 
            }
            if (ThisCustomer.ThisCustomerSession.Session("CouponsSearch").Length != 0)
            {
                query = ThisCustomer.ThisCustomerSession.Session("CouponsSearch");
            }
            if (ThisCustomer.ThisCustomerSession.Session("CouponsTree").Length != 0)
            {
                treeMain.FindNode(ThisCustomer.ThisCustomerSession.Session("CouponsTree")).Selected = true;
            }

            this.resultFilter(query);
            
            this.txtSearch.Attributes.Add("onKeyPress", "javascript:if (event.keyCode == 13) __doPostBack('btnSearch','')");
        }
    }

    private void loadTree()
    {
        try
        {
            string index = "#ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            treeMain.Nodes.Clear();
            treeMain.Nodes.Add(new TreeNode("All", "All", "icons/dot.gif"));

            foreach (char c in index)
            {
                TreeNode myNode = new TreeNode();
                myNode.Text = c.ToString();
                myNode.ImageUrl = "icons/dot.gif";
                treeMain.Nodes.Add(myNode);
            }
        }
        catch (Exception ex)
        {
            this.resetError(ex.ToString(), true);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        ViewState["IsInsert"] = false;
        gMain.EditIndex = -1;
        this.resetError("", false);
        this.gMain.PageIndex = 0;

        this.resultFilter("");
    }

    protected void resultFilter(string SearchFor)
    {
        String sql = selectSQL + DB.GetNoLock() + " ";
        String WhereClause = " Deleted=0 ";

        //search
        if (SearchFor.Length == 0)
        {
            SearchFor = this.txtSearch.Text;
            ThisCustomer.ThisCustomerSession.SetVal("CouponsSearch", this.txtSearch.Text);
        }

        if (SearchFor.Length != 0)
        {
            if (WhereClause.Length != 0)
            {
                WhereClause += " and ";
            }
            WhereClause += " (CouponCode like " + DB.SQuote("%" + SearchFor + "%") + " or Description like " + DB.SQuote("%" + SearchFor + "%") + ")";
        }

        //Node filter
        string Index = "";
        for (int i = 0; i < this.treeMain.Nodes.Count; i++)
        {
            if (this.treeMain.Nodes[i].Selected)
            {
                Index = treeMain.Nodes[i].Value;

                ThisCustomer.ThisCustomerSession.SetVal("CouponsTree", treeMain.Nodes[i].Value);

                break;
            }
        }

        if (!Index.Equals("All"))
        {
            if (WhereClause.Length != 0)
            {
                WhereClause += " and ";
            }
            WhereClause += " CouponCode like " + DB.SQuote(Index + "%");
        }
        if (Index.Equals("#"))
        {
            if (WhereClause.Length != 0)
            {
                WhereClause += " and ";
            }
            WhereClause += " CouponCode like (" + DB.SQuote("0%") + ")";
            WhereClause += " OR CouponCode like (" + DB.SQuote("1%") + ")";
            WhereClause += " OR CouponCode like (" + DB.SQuote("2%") + ")";
            WhereClause += " OR CouponCode like (" + DB.SQuote("3%") + ")";
            WhereClause += " OR CouponCode like (" + DB.SQuote("4%") + ")";
            WhereClause += " OR CouponCode like (" + DB.SQuote("5%") + ")";
            WhereClause += " OR CouponCode like (" + DB.SQuote("6%") + ")";
            WhereClause += " OR CouponCode like (" + DB.SQuote("7%") + ")";
            WhereClause += " OR CouponCode like (" + DB.SQuote("8%") + ")";
            WhereClause += " OR CouponCode like (" + DB.SQuote("9%") + ")";
            WhereClause += " OR CouponCode like (" + DB.SQuote("10%") + ")";
        }

        if (WhereClause.Length != 0)
        {
            sql += " where " + WhereClause;
        }

        //set states
        ViewState["SQLString"] = sql.ToString();
        sql += " order by " + ViewState["Sort"].ToString() + " " + ViewState["SortOrder"].ToString();

        ThisCustomer.ThisCustomerSession.SetVal("CouponsSort", ViewState["Sort"].ToString());
        ThisCustomer.ThisCustomerSession.SetVal("CouponsOrder", ViewState["SortOrder"].ToString());
        
        //remember page
        if (ThisCustomer.ThisCustomerSession.SessionNativeInt("CouponsPage") > 0)
        {
            gMain.PageIndex = ThisCustomer.ThisCustomerSession.SessionNativeInt("CouponsPage");
        }

        //build grid
        this.buildGridData(DB.GetDS(sql, false));

        ((TextBox)this.Form.FindControl("txtSearch")).Text = SearchFor;
    }

    protected void treeMain_SelectedNodeChanged(object sender, EventArgs e)
    {
        ViewState["IsInsert"] = false;
        gMain.EditIndex = -1;
        this.resetError("", false);
        this.gMain.PageIndex = 0;

        this.resultFilter("");
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

        try
        {
            for (int i = 0; i < gMain.HeaderRow.Cells.Count; i++)
            {
                if (gMain.Columns[i].SortExpression.Equals(ViewState["Sort"].ToString()))
                {
                    Image arrow = new Image();
                    if (ViewState["SortOrder"].ToString().ToLowerInvariant().Equals("asc"))
                    {
                        arrow.ImageUrl = "icons/asc.gif";
                    }
                    else
                    {
                        arrow.ImageUrl = "icons/desc.gif";
                    }
                    gMain.HeaderRow.Cells[i].Controls.Add(arrow);
                }
            }
        }
        catch { }

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

        ((Literal)this.Form.FindControl("ltError")).Text = str;
    }
    
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect("editCoupons.aspx?iden=0");
    }

    protected void gMain_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView myrow = (DataRowView)e.Row.DataItem;

            ImageButton ib = (ImageButton)e.Row.FindControl("imgDelete");
            ib.Attributes.Add("onClick", "javascript: return confirm('Confirm Delete?')");

            string amount = Localization.CurrencyStringForDisplayWithoutExchangeRate(Localization.ParseNativeCurrency(myrow["DiscountAmount"].ToString()));
            Literal ltAmount = (Literal)e.Row.FindControl("ltAmount");
            ltAmount.Text = amount;

            string percent = Localization.FormatDecimal2Places(myrow["DiscountPercent"].ToString());
            Literal ltPercent = (Literal)e.Row.FindControl("ltPercent");
            ltPercent.Text = percent;
        }
        //if (e.Row.RowType == DataControlRowType.Header)
        //{
        //    for (int i = 0; i < gMain.Columns.Count; i++)
        //    {
        //        if (gMain.Columns[i].SortExpression.Equals(ViewState["Sort"].ToString()))
        //        {
        //            e.Row.Cells[i].Text += ViewState["SortOrder"].ToString();
        //        }
        //    }
        //}
    }
    protected void gMain_Sorting(object sender, GridViewSortEventArgs e)
    {
        gMain.EditIndex = -1;
        ViewState["Sort"] = e.SortExpression.ToString();
        ViewState["SortOrder"] = (ViewState["SortOrder"].ToString() == "ASC" ? "DESC" : "ASC");
        this.buildGridData(this.buildGridData());
    }
    protected void gMain_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        this.resetError("", false);

        if (e.CommandName == "DeleteItem")
        {
            gMain.EditIndex = -1;
            int iden = Localization.ParseNativeInt(e.CommandArgument.ToString());
            this.deleteRow(iden);
        }
    }

    protected void deleteRow(int CouponID)
    {
        DB.ExecuteSQL("Update customer set CouponCode=NULL where lower(CouponCode)=(select lower(couponcode) from coupon  " + DB.GetNoLock() + " where couponid=" + CouponID + ")");
        DB.ExecuteSQL("update Coupon set deleted=1 where CouponID=" + CouponID);

        string treeT = treeMain.SelectedValue;
        this.loadTree();
        foreach (TreeNode node in treeMain.Nodes)
        {
            if (node.Value.Equals(treeT))
            {
                node.Selected = true;
            }
        }

        this.buildGridData(this.buildGridData());
        resetError("Coupon deleted.", false);
    }

    protected void gMain_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.resetError("", false);
        gMain.PageIndex = e.NewPageIndex;
        gMain.EditIndex = -1;
        this.buildGridData(this.buildGridData());
        ThisCustomer.ThisCustomerSession.SetVal("CouponsPage", e.NewPageIndex.ToString());
    }
}
