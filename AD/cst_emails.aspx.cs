// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/cst_emails.aspx.cs 5     9/13/06 7:38p Buddy $
// ------------------------------------------------------------------------------------------
using System;
using System.Text;
using System.Web;
using System.Xml;
using System.Data;
using System.Xml.Serialization;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for cst_EMails.
	/// </summary>
	public partial class cst_EMails : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            // dump the order & customer info:
			Response.Write("<html><body>");
			Response.Expires = -1;
            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
			if(ThisCustomer.IsAdminUser)
			{
				Response.Write("<a href=\"cst_EMails.aspx?issubmit=true&type=all\">All Customers</a>&nbsp;&nbsp;<a href=\"cst_EMails.aspx?issubmit=true&type=ordersonly\">Only Customers With Orders</a><br/><br/>");
				if(CommonLogic.QueryStringCanBeDangerousContent("issubmit").Length != 0)
				{
					IDataReader rs = DB.GetRS("select * from customer  " + DB.GetNoLock() + " where deleted=0 and EMail <> '' " + CommonLogic.IIF(CommonLogic.QueryStringCanBeDangerousContent("Type").ToUpperInvariant() == "ALL", "", " and CustomerID in (select distinct customerid from orders " + DB.GetNoLock() + ")") + " order by createdon desc");
					while(rs.Read())
					{
						Response.Write(DB.RSField(rs,"EMail") + "<br/>");
					}
					rs.Close();
				}
			}
			else
			{
				Response.Write("Insufficient Privilege");
			}
			Response.Write("</body></html>");
		}
	}
}
