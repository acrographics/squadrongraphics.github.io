// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/shippingzones.aspx.cs 4     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for ShippingZones.
	/// </summary>
    public partial class ShippingZones : AspDotNetStorefront.SkinBase
	{
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache"); 
            

			SectionTitle = "Manage Shipping Zones";
		}

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
		{
			if(CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
			{
				// delete the Zone:
				DB.ExecuteSQL("delete from ShippingWeightByZone where ShippingZoneID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID"));
				DB.ExecuteSQL("delete from ShippingTotalByZone where ShippingZoneID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID"));
				DB.ExecuteSQL("delete from ShippingZone where ShippingZoneID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID"));
			}

			DataSet ds = DB.GetDS("select * from ShippingZone  " + DB.GetNoLock() + " where deleted=0 order by DisplayOrder,Name",false);
			writer.Write("<form Method=\"POST\" action=\"ShippingZones.aspx\">\n");
			writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
			writer.Write("  <table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
			writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
			writer.Write("      <td width=\"5%\" align=\"center\"><b>ID</b></td>\n");
			writer.Write("      <td align=\"left\"><b>Zone</b></td>\n");
			writer.Write("      <td align=\"left\" width=\"50%\" align=\"left\"><b>ZipCodes</b></td>\n");
			writer.Write("      <td align=\"center\"><b>Edit</b></td>\n");
			writer.Write("      <td align=\"center\"><b>Delete</b></td>\n");
			writer.Write("    </tr>\n");
			foreach(DataRow row in ds.Tables[0].Rows)
			{
				writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
				writer.Write("      <td width=\"5%\"  align=\"center\">" + DB.RowFieldInt(row,"ShippingZoneID").ToString() + "</td>\n");
				writer.Write("      <td align=\"left\"><a href=\"editShippingZone.aspx?ShippingZoneID=" + DB.RowFieldInt(row,"ShippingZoneID").ToString() + "\">" + DB.RowFieldByLocale(row,"Name",ThisCustomer.LocaleSetting) + "</a></td>\n");
				writer.Write("      <td align=\"left\" width=\"50%\"  align=\"center\">" + DB.RowField(row,"ZipCodes") + "</td>\n");
				writer.Write("      <td align=\"center\"><input type=\"button\" value=\"Edit\" name=\"Edit_" + DB.RowFieldInt(row,"ShippingZoneID").ToString() + "\" onClick=\"self.location='editShippingZone.aspx?ShippingZoneID=" + DB.RowFieldInt(row,"ShippingZoneID").ToString() + "'\"></td>\n");
				writer.Write("      <td align=\"center\"><input type=\"button\" value=\"Delete\" name=\"Delete_" + DB.RowFieldInt(row,"ShippingZoneID").ToString() + "\" onClick=\"DeleteShippingZone(" + DB.RowFieldInt(row,"ShippingZoneID").ToString() + ")\"></td>\n");
				writer.Write("    </tr>\n");
			}
			ds.Dispose();
			writer.Write("</table>\n");
			writer.Write("<input type=\"button\" value=\"Add New Shipping Zone\" name=\"AddNew\" onClick=\"self.location='editShippingZone.aspx';\">\n");
			writer.Write("</form>\n");

			writer.Write("</center></b>\n");

			writer.Write("<script type=\"text/javascript\">\n");
			writer.Write("function DeleteShippingZone(id)\n");
			writer.Write("{\n");
			writer.Write("if(confirm('Are you sure you want to delete Shipping Zone: ' + id))\n");
			writer.Write("{\n");
			writer.Write("self.location = 'ShippingZones.aspx?deleteid=' + id;\n");
			writer.Write("}\n");
			writer.Write("}\n");
			writer.Write("</SCRIPT>\n");
		}

	}
}
