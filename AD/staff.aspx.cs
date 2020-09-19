// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/staff.aspx.cs 6     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for staff.
	/// </summary>
    public partial class staff : AspDotNetStorefront.SkinBase
	{
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache"); 
            

			SectionTitle = "Manage Staff";
		}

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
		{
			if(CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
			{
				// delete the mfg:
				DB.ExecuteSQL("update Staff set deleted=1 where StaffID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID"));
			}

			if(CommonLogic.FormBool("IsSubmit"))
			{
				for(int i = 0; i<=Request.Form.Count-1; i++)
				{
					if(Request.Form.Keys[i].IndexOf("DisplayOrder_") != -1)
					{
						String[] keys = Request.Form.Keys[i].Split('_');
						int StaffID = Localization.ParseUSInt(keys[1]);
						int DispOrd = 1;
						try
						{
							DispOrd = Localization.ParseUSInt(Request.Form[Request.Form.Keys[i]]);
						}
						catch {}
						DB.ExecuteSQL("update Staff set DisplayOrder=" + DispOrd.ToString() + " where StaffID=" + StaffID.ToString());
					}
				}
			}
			
			DataSet ds = DB.GetDS("select * from Staff  " + DB.GetNoLock() + " where deleted=0 order by DisplayOrder,Name",false);
			writer.Write("<form method=\"POST\" action=\"staff.aspx\">\n");
			writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
			writer.Write("  <table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
			writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
			writer.Write("      <td><b>ID</b></td>\n");
			writer.Write("      <td><b>Staff</b></td>\n");
			writer.Write("      <td align=\"center\"><b>Staff Display Order</b></td>\n");
			writer.Write("      <td align=\"center\"><b>Edit Staff</b></td>\n");
			writer.Write("      <td align=\"center\"><b>Delete Staff</b></td>\n");
			writer.Write("    </tr>\n");
			foreach(DataRow row in ds.Tables[0].Rows)
			{
				writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
				writer.Write("      <td >" + DB.RowFieldInt(row,"StaffID").ToString() + "</td>\n");
				writer.Write("<td>\n");
				String Image1URL = AppLogic.LookupImage("Staff",DB.RowFieldInt(row,"StaffID"),"icon",SkinID,ThisCustomer.LocaleSetting);
				writer.Write("<a href=\"editStaff.aspx?Staffid=" + DB.RowFieldInt(row,"StaffID").ToString() + "\">");
				writer.Write("<img src=\"" + Image1URL + "\" height=\"25\" border=\"0\" align=\"absmiddle\">");
				writer.Write("</a>&nbsp;\n");
				writer.Write("<a href=\"editStaff.aspx?Staffid=" + DB.RowFieldInt(row,"StaffID").ToString() + "\">");
				writer.Write(DB.RowField(row,"Name"));
				writer.Write("</a>");
				writer.Write("</td>\n");
				writer.Write("      <td align=\"center\"><input size=2 type=\"text\" name=\"DisplayOrder_" + DB.RowFieldInt(row,"StaffID").ToString() + "\" value=\"" + DB.RowFieldInt(row,"DisplayOrder").ToString() + "\"></td>\n");
				writer.Write("      <td align=\"center\"><input type=\"button\" value=\"Edit\" name=\"Edit_" + DB.RowFieldInt(row,"StaffID").ToString() + "\" onClick=\"self.location='editStaff.aspx?Staffid=" + DB.RowFieldInt(row,"StaffID").ToString() + "'\"></td>\n");
				writer.Write("      <td align=\"center\"><input type=\"button\" value=\"Delete\" name=\"Delete_" + DB.RowFieldInt(row,"StaffID").ToString() + "\" onClick=\"DeleteStaff(" + DB.RowFieldInt(row,"StaffID").ToString() + ")\"></td>\n");
				writer.Write("    </tr>\n");
			}
			ds.Dispose();
			writer.Write("    <tr>\n");
			writer.Write("      <td colspan=\"2\" align=\"left\"></td>\n");
			writer.Write("      <td align=\"center\" bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></td>\n");
			writer.Write("      <td colspan=\"2\"></td>\n");
			writer.Write("    </tr>\n");
			writer.Write("    <tr>\n");
			writer.Write("      <td colspan=\"5\" height=5></td>\n");
			writer.Write("    </tr>\n");
			writer.Write("  </table>\n");
			writer.Write("<input type=\"button\" value=\"Add New Staff\" name=\"AddNew\" onClick=\"self.location='editStaff.aspx';\">");
			writer.Write("</form>\n");

			writer.Write("</center></b>\n");

			writer.Write("<script type=\"text/javascript\">\n");
			writer.Write("function DeleteStaff(id)\n");
			writer.Write("{\n");
			writer.Write("if(confirm('Are you sure you want to delete Staff: ' + id))\n");
			writer.Write("{\n");
			writer.Write("self.location = 'staff.aspx?deleteid=' + id;\n");
			writer.Write("}\n");
			writer.Write("}\n");
			writer.Write("</SCRIPT>\n");
		}

	}
}
