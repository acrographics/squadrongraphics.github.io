// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/ratecomment.aspx.cs 4     8/19/06 8:46p Buddy $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Text;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
	/// <summary>
	/// Summary description for ratecomment.
	/// </summary>
	public partial class ratecomment : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
			ThisCustomer.RequireCustomerRecord();

			int ProductID = CommonLogic.QueryStringUSInt("ProductID");
			int VotingCustomerID = CommonLogic.QueryStringUSInt("VotingCustomerID");
			int CustomerID = CommonLogic.QueryStringUSInt("CustomerID");
			String MyVote = CommonLogic.QueryStringCanBeDangerousContent("MyVote").ToUpperInvariant();
			int HelpfulVal = CommonLogic.IIF(MyVote == "YES" , 1 , 0);
			bool IsProduct = CommonLogic.QueryStringBool("IsProduct");

			bool AlreadyVoted = false;
			IDataReader rs = DB.GetRS("select * from RatingCommentHelpfulness  " + DB.GetNoLock() + " where ProductID=" + ProductID.ToString() + " and RatingCustomerID=" + CustomerID.ToString() + " and VotingCustomerID=" + VotingCustomerID.ToString());
			if(rs.Read())
			{
				AlreadyVoted = true;
				// they have already voted on this comment, and are changing their minds perhaps, so adjust totals, and reapply vote:
				if(DB.RSFieldBool(rs,"Helpful"))
				{
					DB.ExecuteSQL("update Rating set FoundHelpful = FoundHelpful-1 where ProductID=" + ProductID.ToString() + " and CustomerID=" + CustomerID.ToString());
				}
				else
				{
					DB.ExecuteSQL("update Rating set FoundNotHelpful = FoundNotHelpful-1 where ProductID=" + ProductID.ToString() + " and CustomerID=" + CustomerID.ToString());
				}
			}
			rs.Close();
			if(AlreadyVoted)
			{
				DB.ExecuteSQL("delete from RatingCommentHelpfulness where ProductID=" + ProductID.ToString() + " and RatingCustomerID=" + CustomerID.ToString() + " and VotingCustomerID=" + VotingCustomerID.ToString());
			}

			DB.ExecuteSQL("insert into RatingCommentHelpfulness(ProductID,RatingCustomerID,VotingCustomerID,Helpful) values(" + ProductID.ToString() + "," + CustomerID.ToString() + "," + VotingCustomerID.ToString() + "," + HelpfulVal.ToString() + ")");
			if(MyVote == "YES")
			{
				DB.ExecuteSQL("update Rating set FoundHelpful = FoundHelpful+1 where ProductID=" + ProductID.ToString() + " and CustomerID=" + CustomerID.ToString());
			}
			else
			{
				DB.ExecuteSQL("update Rating set FoundNotHelpful = FoundNotHelpful+1 where ProductID=" + ProductID.ToString() + " and CustomerID=" + CustomerID.ToString());
			}

			Response.Write("<html>\n");
			Response.Write("<head>\n");
			Response.Write("<title>Rate Comment</title>\n");
			Response.Write("</head>\n");
			Response.Write("<body>\n");
			Response.Write("<!-- INVOCATION: " + CommonLogic.PageInvocation() + " -->\n");
			Response.Write("</body>\n");
			Response.Write("</html>\n");

		}

	}
}
