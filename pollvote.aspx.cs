// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/pollvote.aspx.cs 2     8/19/06 8:46p Buddy $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
	/// <summary>
	/// Summary description for pollvote.
	/// </summary>
	public partial class pollvote : System.Web.UI.Page
	{
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
			ThisCustomer.RequireCustomerRecord();

			int PollID = CommonLogic.FormNativeInt("PollID");
			int CustomerID = ThisCustomer.CustomerID;
			int PollAnswerID = CommonLogic.FormNativeInt("Poll_" + PollID.ToString());

			if(PollID != 0 && CustomerID != 0 && PollAnswerID != 0)
			{
				// record the vote:
				try
				{
					DB.ExecuteSQL("insert into PollVotingRecord(PollID,CustomerID,PollAnswerID) values(" + PollID.ToString() + "," + CustomerID.ToString() + "," + PollAnswerID.ToString() + ")");
				}
				catch {}
			}

			Response.Redirect("polls.aspx");
		}

	}
}
