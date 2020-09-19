// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/showcustomerlevel.aspx.cs 7     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for showcustomerlevel.
	/// </summary>
    public partial class showcustomerlevel : AspDotNetStorefront.SkinBase
	{
		
		private int CustomerLevelID;
		private String CustomerLevelName;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            CustomerLevelID = CommonLogic.QueryStringUSInt("CustomerLevelID");
			CustomerLevelName = Customer.GetCustomerLevelName(CustomerLevelID,ThisCustomer.LocaleSetting);
			if(CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
			{
				// remove this level from this customer:
				DB.ExecuteSQL("update Customer set CustomerLevelID=0 where CustomerID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID"));
			}

			if(CommonLogic.FormBool("IsSubmit"))
			{
				String EMail = CommonLogic.FormCanBeDangerousContent("EMail");
				if(EMail.Length != 0)
				{
					int CustomerID = Customer.GetIDFromEMail(EMail);
					if(CustomerID == 0)
					{
						if(CommonLogic.IsInteger(CommonLogic.FormCanBeDangerousContent("EMail")))
						{
							CustomerID = Localization.ParseUSInt(CommonLogic.FormCanBeDangerousContent("EMail")); // in case they just entered a customer id into the field.
						}
					}
					if(CustomerID != 0)
					{
						// clear the carts for this customer. This is to ensure their produce pricing is correct
						// their current cart can have customer level pricing, not retail pricing, and this prevents that:
						DB.ExecuteSQL("delete from shoppingcart where customerid=" + CustomerID.ToString());
						DB.ExecuteSQL("delete from customcart where customerid=" + CustomerID.ToString());
						DB.ExecuteSQL("delete from kitcart where customerid=" + CustomerID.ToString());

						DB.ExecuteSQL("Update customer set CustomerLevelID=" + CustomerLevelID.ToString() + " where CustomerID=" + CustomerID.ToString());
					}
					else
					{
						ErrorMsg = "That customer e-mail was not found in the database";
					}
				}
			}
			SectionTitle = "<a href=\"CustomerLevels.aspx\">CustomerLevels</a> - Show Customer Level: " + CustomerLevelName;
		}

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
		{
            writer.Write("<p><b>There are " + DB.GetSqlN("select count(CustomerID) as N from Customer " + DB.GetNoLock() + " where EMail <> '' and Deleted=0 and CustomerLevelID=" + CustomerLevelID.ToString()).ToString() + " registered customers in this customer level</b></p>");

            String SearchFor = CommonLogic.QueryStringCanBeDangerousContent("SearchFor");
            writer.Write("<form method=\"GET\" action=\"showcustomerlevel.aspx\">");
            writer.Write("<input type=\"hidden\" name=\"CustomerLevelID\" value=\"" + CustomerLevelID.ToString() + "\">");
            String BeginsWith = CommonLogic.IIF(CommonLogic.QueryStringCanBeDangerousContent("BeginsWith").Length == 0, "A", CommonLogic.QueryStringCanBeDangerousContent("BeginsWith"));
            String alpha = "%#ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 1; i <= alpha.Length; i++)
            {
                if (BeginsWith[0] == alpha[i - 1])
                {
                    writer.Write(alpha[i - 1] + "&nbsp;");
                }
                else
                {
                    writer.Write("<a href=\"showcustomerlevel.aspx?CustomerLevelID=" + CustomerLevelID.ToString() + "&BeginsWith=" + Server.UrlEncode("" + alpha[i - 1]) + "\">" + alpha[i - 1] + "</a>&nbsp;");
                }
            }
            writer.Write("&nbsp;&nbsp;Search For: <input type=\"text\" name=\"SearchFor\" value=\"" + SearchFor + "\"><input type=\"submit\" name=\"search\" value=\"submit\">");
            writer.Write("</form>");


            String SQL = String.Empty;
            string SuperuserFilter = (ThisCustomer.IsAdminSuperUser) ? String.Empty : String.Format(" Customer.CustomerID not in ({0}) and ", AppLogic.AppConfig("Admin_Superuser"));

            if (SearchFor.Length != 0)
            {
                int CID = 0;
                if (CommonLogic.IsInteger(SearchFor))
                {
                    CID = Localization.ParseUSInt(SearchFor);
                }
                SQL = "select * from Customer " + DB.GetNoLock() + " where " + SuperuserFilter + " Customer.EMail <> '' and Customer.deleted=0 and (Customer.LastName like " + DB.SQuote("%" + SearchFor + "%") + CommonLogic.IIF(CID != 0, " or Customer.CustomerID=" + CID.ToString(), "") + " or Customer.Firstname like " + DB.SQuote("%" + SearchFor + "%") + " or Customer.EMail like " + DB.SQuote("%" + SearchFor + "%") + ")";
            }
            else
            {
                SQL = "select * from Customer " + DB.GetNoLock() + " where " + SuperuserFilter + " Customer.EMail <> '' and Customer.deleted=0 and Customer.LastName like " + DB.SQuote(BeginsWith + "%");
            }
            SQL += " and CustomerLevelID=" + CustomerLevelID.ToString();
            String OrderBySQL = " order by LastName, FirstName";
            SQL = SQL + OrderBySQL;

            // ------------------------------------------------------------
            // Setup Paging Vars:
            // ------------------------------------------------------------
            int PageSize = 50;
            int PageNum = CommonLogic.QueryStringUSInt("PageNum");
            if (PageNum == 0)
            {
                PageNum = 1;
            }

            String FinalSQL = SQL;
            int NumPages = 1;
            bool ShowAll = (CommonLogic.QueryStringCanBeDangerousContent("show").ToUpperInvariant() == "ALL");
            if (!ShowAll)
            {
                FinalSQL = String.Format("aspdnsf_PageQuery {0},'',{1},{2},{3}", DB.SQuote(SQL), PageNum.ToString(), PageSize.ToString(), "0"); //paging stats last!
            }
            DataSet ds = DB.GetDS(FinalSQL, false);
            if (!ShowAll)
            {
                DataRow PagingStatsRow = ds.Tables[1].Rows[0];
                NumPages = DB.RowFieldInt(PagingStatsRow, "Pages");
            }

            // ---------------------------------------------------
            // write paging info:
            // ---------------------------------------------------
            if (NumPages > 1 || ShowAll)
            {
                writer.Write("<p class=\"PageNumber\" align=\"left\">");
                if (CommonLogic.QueryStringCanBeDangerousContent("show").ToUpperInvariant() == "ALL")
                {
                    writer.Write("Click <a class=\"PageNumber\" href=\"showcustomerlevel.aspx?CustomerLevelID=" + CustomerLevelID.ToString() + "&BeginsWith=" + BeginsWith + "&SearchFor=" + Server.UrlEncode(SearchFor) + "&pagenum=1\">here</a> to turn paging back on.");
                }
                else
                {
                    writer.Write("Page: ");
                    for (int u = 1; u <= NumPages; u++)
                    {
                        if (u == PageNum)
                        {
                            writer.Write(u.ToString() + " ");
                        }
                        else
                        {
                            writer.Write("<a class=\"PageNumber\" href=\"showcustomerlevel.aspx?CustomerLevelID=" + CustomerLevelID.ToString() + "&BeginsWith=" + BeginsWith + "&SearchFor=" + Server.UrlEncode(SearchFor) + "&pagenum=" + u.ToString() + "\">" + u.ToString() + "</a> ");
                        }
                    }
                    writer.Write(" <a class=\"PageNumber\" href=\"showcustomerlevel.aspx?CustomerLevelID=" + CustomerLevelID.ToString() + "&BeginsWith=" + BeginsWith + "&SearchFor=" + Server.UrlEncode(SearchFor) + "&show=all\">all</a>");
                }
                writer.Write("</p>\n");
            }	
            
			writer.Write("<script type=\"text/javascript\">\n");
			writer.Write("function Form_Validator(theForm)\n");
			writer.Write("{\n");
			writer.Write("submitonce(theForm);\n");
			writer.Write("return (true);\n");
			writer.Write("}\n");
			writer.Write("</script>\n");
			
			writer.Write("<form method=\"POST\" action=\"showCustomerLevel.aspx?customerlevelid=" + CustomerLevelID.ToString() + "\"  id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(this) && Form_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
			writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
			writer.Write("<table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
			writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
			writer.Write("<td width=\"10%\"><b>Customer ID</b></td>\n");
			writer.Write("<td ><b>Name</b></td>\n");
			writer.Write("<td ><b>EMail</b></td>\n");
			writer.Write("<td align=\"center\"><b>Clear Level for this Customer</b></td>\n");
			writer.Write("</tr>\n");
			foreach(DataRow row in ds.Tables[0].Rows)
			{
				writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
				writer.Write("<td width=\"10%\">" + DB.RowFieldInt(row,"CustomerID").ToString() + "</td>\n");
				writer.Write("<td >" + (DB.RowField(row,"FirstName") + " " + DB.RowField(row,"LastName")).Trim() + "</td>\n");
				writer.Write("<td >" + DB.RowField(row,"EMail") + "</td>\n");
				writer.Write("<td width=\"10%\" align=\"center\"><input type=\"button\" value=\"Clear Level\" name=\"Delete_" + DB.RowFieldInt(row,"CustomerID").ToString() + "\" onClick=\"DeleteCustomerLevel(" + DB.RowFieldInt(row,"CustomerID").ToString() + ")\"></td>\n");
				writer.Write("</tr>\n");
			}
			ds.Dispose();
			writer.Write(" </table>\n");
			writer.Write("<p align=\"left\">Enter CustomerID or EMail to add to this level: ");
			writer.Write("<input type=\"text\" name=\"EMail\" size=\"37\" maxlength=\"100\">");
			writer.Write("<input type=\"hidden\" name=\"EMail_vldt\" value=\"[req][blankalert=Please enter a valid customer e-mail address.  You must know this in advance, and type it in here][invalidalert=Please enter a valid customer e-mail address]\">");
			writer.Write("<input type=\"submit\" value=\"Add Customer To This Level\" name=\"Submit\">");
			writer.Write("</p>\n");
			writer.Write("</form>\n");

			writer.Write("</center></b>\n");

			writer.Write("<script type=\"text/javascript\">\n");
			writer.Write("function DeleteCustomerLevel(id)\n");
			writer.Write("{\n");
			writer.Write("if(confirm('Are you sure you clear the level for customer id: ' + id + '. NOTE: this will NOT delete their customer record'))\n");
			writer.Write("{\n");
            writer.Write("self.location = 'showcustomerlevel.aspx?CustomerLevelID=" + CustomerLevelID.ToString() + "&beginswith=" + BeginsWith + "&searchfor=" + Server.UrlEncode(SearchFor) + "&deleteid=' + id;\n");
            writer.Write("}\n");
			writer.Write("}\n");
			writer.Write("</SCRIPT>\n");
		}


	}
}
