// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/managepoll.aspx.cs 4     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for managepoll.
	/// </summary>
	public partial class managepoll : AspDotNetStorefront.SkinBase
	{
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache"); 
            

			SectionTitle = "<a href=\"Polls.aspx\">Manage Polls</a> - Review Customer Votes";
		}

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
		{
			int PollID = CommonLogic.QueryStringUSInt("PollID");
			if(PollID == 0)
			{
				Response.Redirect("Polls.aspx");
			}

			String PollName = AppLogic.GetPollName(PollID,ThisCustomer.LocaleSetting);

			if(CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
			{
				DB.ExecuteSQL("delete from PollVotingRecord where PollVotingRecordID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID"));
			}

			if(CommonLogic.QueryStringCanBeDangerousContent("BanID").Length != 0)
			{
				DB.ExecuteSQL("delete from PollVotingRecord where CustomerID=" + CommonLogic.QueryStringCanBeDangerousContent("BanID"));
			}

			writer.Write("<p align=\"left\"<b>Reviewing Votes for Poll: <a href=\"editPoll.aspx?Pollid=" + PollID.ToString() + "\">" + PollName + "</a> (PollID=" + PollID.ToString() + ")</b></p>\n");

			DataSet ds = DB.GetDS("SELECT  Customer.EMail, Customer.FirstName AS CFN, Customer.LastName AS CLN, Customer.FirstName + ' ' + Customer.LastName AS Name, PollVotingRecord.PollVotingRecordID,PollVotingRecord.CreatedOn, PollVotingRecord.PollID, PollAnswer.Name AS AnswerName, PollVotingRecord.PollAnswerID, PollVotingRecord.CustomerID FROM (PollVotingRecord LEFT OUTER JOIN Customer ON PollVotingRecord.CustomerID = Customer.CustomerID) LEFT OUTER JOIN PollAnswer ON PollVotingRecord.PollAnswerID = PollAnswer.PollAnswerID where PollVotingRecord.PollID=" + PollID.ToString() + " order by PollVotingRecord.CreatedOn desc",false);


			writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
			writer.Write("  <table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
			writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
			writer.Write("      <td><b>Record ID</b></td>\n");
			writer.Write("      <td><b>Answered On</b></td>\n");
			writer.Write("      <td><b>Customer ID</b></td>\n");
			writer.Write("      <td><b>Customer EMail</b></td>\n");
			writer.Write("      <td><b>Customer Name</b></td>\n");
			writer.Write("      <td><b>Answer Picked</b></td>\n");
			writer.Write("      <td align=\"center\"><b>Delete</b></td>\n");
			writer.Write("      <td align=\"center\"><b>BAN Customer</b></td>\n");
			writer.Write("    </tr>\n");
			foreach(DataRow row in ds.Tables[0].Rows)
			{
				writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
				writer.Write("      <td>" + DB.RowFieldInt(row,"PollVotingRecordID").ToString() + "</td>\n");
				writer.Write("      <td>" + DB.RowFieldDateTime(row,"CreatedOn").ToString() + "</td>\n");
				writer.Write("      <td>" + DB.RowFieldInt(row,"CustomerID").ToString() + "</td>\n");
				writer.Write("      <td>" + DB.RowField(row,"EMail") + "</td>\n");
				writer.Write("      <td>" + (DB.RowField(row,"CFN") + " " + DB.RowField(row,"CLN")).Trim() + "</td>\n");
				writer.Write("      <td>" + DB.RowField(row,"AnswerName") + "</td>\n");
				writer.Write("      <td align=\"center\"><input type=\"button\" value=\"Delete\" name=\"Delete_" + DB.RowFieldInt(row,"PollVotingRecordID").ToString() + "\" onClick=\"DeleteVote(" + DB.RowFieldInt(row,"PollVotingRecordID").ToString() + ")\"></td>\n");
				writer.Write("      <td align=\"center\"><input type=\"button\" value=\"BAN\" name=\"BanCustomer_" + DB.RowFieldInt(row,"CustomerID").ToString() + "\" onClick=\"BanCustomer(" + DB.RowFieldInt(row,"CustomerID").ToString() + ")\"></td>\n");
				writer.Write("    </tr>\n");
			}
			ds.Dispose();
			writer.Write("  </table>\n");

			writer.Write("<script type=\"text/javascript\">\n");
			writer.Write("function DeleteVote(id)\n");
			writer.Write("{\n");
			writer.Write("if(confirm('Are you sure you want to delete this vote?'))\n");
			writer.Write("{\n");
			writer.Write("self.location = 'managepoll.aspx?Pollid=" + PollID.ToString() + "&deleteid=' + id;\n");
			writer.Write("}\n");
			writer.Write("}\n");
			writer.Write("function BanCustomer(id)\n");
			writer.Write("{\n");
			writer.Write("if(confirm('Are you sure you want to remove this customers votes from ALL polls?'))\n");
			writer.Write("{\n");
			writer.Write("self.location = 'managepoll.aspx?Pollid=" + PollID.ToString() + "&banid=' + id;\n");
			writer.Write("}\n");
			writer.Write("}\n");
			writer.Write("</SCRIPT>\n");
		}

	}
}
