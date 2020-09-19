// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/shippingmethodzones.aspx.cs 4     9/03/06 8:41p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for ShippingMethodZones.
	/// </summary>
    public partial class ShippingMethodZones : AspDotNetStorefront.SkinBase
	{

		int ShippingMethodID = 0;
		String ShippingMethodName = String.Empty;
        bool IsUpdated = false;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache"); 
            

			ShippingMethodID = CommonLogic.QueryStringUSInt("ShippingMethodID");
			if(ShippingMethodID == 0)
			{
				Response.Redirect("shippingmethods.aspx");
			}
			ShippingMethodName = Shipping.GetShippingMethodName(ShippingMethodID,ThisCustomer.LocaleSetting);
			SectionTitle = "<a href=\"shippingmethods.aspx\">Shipping Methods</a> - Setting Allowed Zones for Shipping Method: " + ShippingMethodName;
		
			if(CommonLogic.FormBool("IsSubmit"))
			{
                IsUpdated = true;
				DB.ExecuteSQL("delete from ShippingMethodToZoneMap where ShippingMethodID=" + ShippingMethodID.ToString());
				foreach(String s in CommonLogic.FormCanBeDangerousContent("ZoneList").Split(','))
				{
					if(s.Trim().Length != 0)
					{
						DB.ExecuteSQL("insert ShippingMethodToZoneMap(ShippingMethodID,ShippingZoneID) values(" + ShippingMethodID.ToString() + "," + s + ")");
					}
				}
			}

			if(CommonLogic.QueryStringCanBeDangerousContent("clearall").Length != 0)
			{
				DB.ExecuteSQL("delete from ShippingMethodToZoneMap where ShippingMethodID=" + ShippingMethodID.ToString());
			}
			if(CommonLogic.QueryStringCanBeDangerousContent("allowall").Length != 0)
			{
				DB.ExecuteSQL("delete from ShippingMethodToZoneMap where ShippingMethodID=" + ShippingMethodID.ToString());
				DB.ExecuteSQL("insert into ShippingMethodToZoneMap(ShippingMethodID,ShippingZoneID) select " + ShippingMethodID.ToString() + ",ShippingZoneID from ShippingZone");
			}
		}

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
		{
			if(CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
			{
				// delete the record:
				DB.ExecuteSQL("delete from ShippingZone where ShippingZoneID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID"));
			}

			if(DB.GetSqlN("select count(*) as N from ShippingZone " + DB.GetNoLock()) == 0)
			{
				writer.Write("<p><b><font color=red>No Shipping Zones are defined!</font></b></p>");
			}
			else
			{

                if (IsUpdated)
                {
                    writer.Write("<p><strong>NOTICE: </strong> Item Updated</p?");
                }

				writer.Write("<form method=\"POST\" action=\"shippingmethodZones.aspx?shippingmethodid=" + ShippingMethodID.ToString() + "\">\n");
				writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
				writer.Write("<p align=\"left\">Check the Shipping Zones that you want to <b>ALLOW</b> for this shipping method.&nbsp;&nbsp;&nbsp;&nbsp;<a href=\"shippingmethodZones.aspx?shippingmethodid=" + ShippingMethodID.ToString() + "&allowall=true\">ALLOW ALL</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href=\"shippingmethodZones.aspx?shippingmethodid=" + ShippingMethodID.ToString() + "&clearall=true\">CLEAR ALL</a><p>");
			
				writer.Write("<p align=\"left\"><input type=\"submit\" value=\"Update\"><p>");
				writer.Write("<table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" >\n");
				DataSet ds = DB.GetDS("select ShippingZone.ShippingZoneID,ShippingZone.Name,ShippingMethodToZoneMap.ShippingMethodID from ShippingZone " + DB.GetNoLock() + " left outer join ShippingMethodToZoneMap " + DB.GetNoLock() + " on ShippingZone.ShippingZoneID=ShippingMethodToZoneMap.ShippingZoneID and ShippingMethodToZoneMap.ShippingMethodID=" + ShippingMethodID.ToString() + " order by displayorder,name",false);
				foreach(DataRow row in ds.Tables[0].Rows)
				{
					bool AllowedForThisZone = DB.RowFieldInt(row,"ShippingMethodID") != 0;
					writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">");
					writer.Write("<td>");
					writer.Write(DB.RowFieldByLocale(row,"Name",ThisCustomer.LocaleSetting));
					writer.Write("</td>");
					writer.Write("<td>");
					writer.Write("<input type=\"checkbox\" name=\"ZoneList\" value=\"" + DB.RowFieldInt(row,"ShippingZoneID").ToString() + "\" " + CommonLogic.IIF(AllowedForThisZone, " checked ","") + ">");
					writer.Write("</td>");
					writer.Write("</tr>\n");
				}
				writer.Write("</table>");
				ds.Dispose();

				writer.Write("<p align=\"left\"><input type=\"submit\" value=\"Update\"><p>");
				writer.Write("</form>\n");
			}
		}

	}
}
