// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/polls.aspx.cs 5     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for polls.
	/// </summary>
    public partial class polls : AspDotNetStorefront.SkinBase
	{
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache"); 
            

			SectionTitle = "Manage Polls";
		}

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
		{
			if(CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
			{
				// delete the record:
				DB.ExecuteSQL("update Poll set deleted=1 where PollID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID"));
			}

			if(CommonLogic.FormBool("IsSubmit"))
			{
				for(int i = 0; i<=Request.Form.Count-1; i++)
				{
					if(Request.Form.Keys[i].IndexOf("DisplayOrder_") != -1)
					{
						String[] keys = Request.Form.Keys[i].Split('_');
						int PollID = Localization.ParseUSInt(keys[1]);
						int DispOrd = 1;
						try
						{
							DispOrd = Localization.ParseUSInt(Request.Form[Request.Form.Keys[i]]);
						}
						catch {}
						DB.ExecuteSQL("update Poll set DisplayOrder=" + DispOrd.ToString() + " where PollID=" + PollID.ToString());
					}
				}
			}
			
			DataSet ds = DB.GetDS("select * from Poll  " + DB.GetNoLock() + " where deleted=0 order by DisplayOrder,Name",false);
			writer.Write("<form method=\"POST\" action=\"polls.aspx\">\n");
			writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
			writer.Write("<p align=\"left\"><input type=\"button\" value=\"Add New Poll\" name=\"AddNew\" onClick=\"self.location='editPoll.aspx';\"><p>");
			writer.Write("  <table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
			writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
			writer.Write("      <td><b>ID</b></td>\n");
			writer.Write("      <td><b>Poll</b></td>\n");
			writer.Write("      <td align=\"center\"><b>Expires On</b></td>\n");
			writer.Write("      <td align=\"center\"><b>NumVotes</b></td>\n");
			writer.Write("      <td align=\"center\"><b>Display Order</b></td>\n");
			writer.Write("      <td align=\"center\"><b>Manage Answers</b></td>\n");
			writer.Write("      <td align=\"center\"><b>Review Votes</b></td>\n");
			writer.Write("      <td align=\"center\"><b>Delete Poll</b></td>\n");
			writer.Write("    </tr>\n");
			foreach(DataRow row in ds.Tables[0].Rows)
			{
				writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
				writer.Write("      <td >" + DB.RowFieldInt(row,"PollID").ToString() + "</td>\n");
				writer.Write("<td>\n");
				writer.Write("<a href=\"editPoll.aspx?Pollid=" + DB.RowFieldInt(row,"PollID").ToString() + "\">");
				writer.Write(DB.RowFieldByLocale(row,"Name",ThisCustomer.LocaleSetting));
				writer.Write("</a>");
				writer.Write("</td>\n");
				writer.Write("<td align=\"center\">" + Localization.ToNativeShortDateString(DB.RowFieldDateTime(row,"ExpiresOn")) + "</td>");
				writer.Write("<td align=\"center\">" + DB.GetSqlN("select count(*) as N from PollVotingRecord  " + DB.GetNoLock() + " where pollanswerid in (select distinct pollanswerid from pollanswer where deleted=0) and PollID=" + DB.RowFieldInt(row,"PollID").ToString()).ToString() + "</td>");
				writer.Write("      <td align=\"center\"><input size=2 type=\"text\" name=\"DisplayOrder_" + DB.RowFieldInt(row,"PollID").ToString() + "\" value=\"" + DB.RowFieldInt(row,"DisplayOrder").ToString() + "\"></td>\n");
				writer.Write("      <td align=\"center\"><input type=\"button\" value=\"Manage Answers\" name=\"ManageAnswers_" + DB.RowFieldInt(row,"PollID").ToString() + "\" onClick=\"self.location='pollanswers.aspx?Pollid=" + DB.RowFieldInt(row,"PollID").ToString() + "'\"></td>\n");
				writer.Write("      <td align=\"center\"><input type=\"button\" value=\"Review Votes\" name=\"ReviewVotes_" + DB.RowFieldInt(row,"PollID").ToString() + "\" onClick=\"self.location='managepoll.aspx?Pollid=" + DB.RowFieldInt(row,"PollID").ToString() + "'\"></td>\n");
				writer.Write("      <td align=\"center\"><input type=\"button\" value=\"Delete\" name=\"Delete_" + DB.RowFieldInt(row,"PollID").ToString() + "\" onClick=\"DeletePoll(" + DB.RowFieldInt(row,"PollID").ToString() + ")\"></td>\n");
				writer.Write("    </tr>\n");
			}
			ds.Dispose();
			writer.Write("    <tr>\n");
			writer.Write("      <td colspan=\"4\" align=\"left\"></td>\n");
			writer.Write("      <td align=\"center\" bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></td>\n");
			writer.Write("      <td colspan=\"3\"></td>\n");
			writer.Write("    </tr>\n");
			writer.Write("  </table>\n");
			writer.Write("<p align=\"left\"><input type=\"button\" value=\"Add New Poll\" name=\"AddNew\" onClick=\"self.location='editPoll.aspx';\"><p>");
			writer.Write("</form>\n");

			writer.Write("</center></b>\n");

			writer.Write("<script type=\"text/javascript\">\n");
			writer.Write("function DeletePoll(id)\n");
			writer.Write("{\n");
			writer.Write("if(confirm('Are you sure you want to delete Poll: ' + id))\n");
			writer.Write("{\n");
			writer.Write("self.location = 'polls.aspx?deleteid=' + id;\n");
			writer.Write("}\n");
			writer.Write("}\n");
			writer.Write("</SCRIPT>\n");
		}



	}
}
