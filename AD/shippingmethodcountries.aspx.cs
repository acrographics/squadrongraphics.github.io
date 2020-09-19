// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/shippingmethodcountries.aspx.cs 4     9/03/06 8:41p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for ShippingMethodCountries.
	/// </summary>
    public partial class ShippingMethodCountries : AspDotNetStorefront.SkinBase
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
			SectionTitle = "<a href=\"shippingmethods.aspx\">Shipping Methods</a> - Setting Allowed Countries for Shipping Method: " + ShippingMethodName;
		
			if(CommonLogic.FormBool("IsSubmit"))
			{

                IsUpdated = true;

				DB.ExecuteSQL("delete from ShippingMethodToCountryMap where ShippingMethodID=" + ShippingMethodID.ToString());
				foreach(String s in CommonLogic.FormCanBeDangerousContent("CountryList").Split(','))
				{
					if(s.Trim().Length != 0)
					{
						DB.ExecuteSQL("insert ShippingMethodToCountryMap(ShippingMethodID,CountryID) values(" + ShippingMethodID.ToString() + "," + s + ")");
					}
				}
			}

			if(CommonLogic.QueryStringCanBeDangerousContent("clearall").Length != 0)
			{
				DB.ExecuteSQL("delete from ShippingMethodToCountryMap where ShippingMethodID=" + ShippingMethodID.ToString());
			}
			if(CommonLogic.QueryStringCanBeDangerousContent("allowall").Length != 0)
			{
				DB.ExecuteSQL("delete from ShippingMethodToCountryMap where ShippingMethodID=" + ShippingMethodID.ToString());
				DB.ExecuteSQL("insert into ShippingMethodToCountryMap(ShippingMethodID,CountryID) select " + ShippingMethodID.ToString() + ",CountryID from Country");
			}
		}

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
		{
			if(DB.GetSqlN("select count(*) as N from Country " + DB.GetNoLock()) == 0)
			{
				writer.Write("<p><b><font color=red>No Countries are defined!</font></b></p>");
			}
			else
			{

                if (IsUpdated)
                {
                    writer.Write("<p><strong>NOTICE: </strong> Item Updated</p?");
                }

				writer.Write("<form method=\"POST\" action=\"shippingmethodCountries.aspx?shippingmethodid=" + ShippingMethodID.ToString() + "\">\n");
				writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
				writer.Write("<p align=\"left\">Check the Countries that you want to <b>ALLOW</b> for this shipping method.&nbsp;&nbsp;&nbsp;&nbsp;<a href=\"shippingmethodCountries.aspx?shippingmethodid=" + ShippingMethodID.ToString() + "&allowall=true\">ALLOW ALL</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href=\"shippingmethodCountries.aspx?shippingmethodid=" + ShippingMethodID.ToString() + "&clearall=true\">CLEAR ALL</a><p>");
			
				writer.Write("<p align=\"left\"><input type=\"submit\" value=\"Update\"><p>");
				writer.Write("<table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" >\n");
				DataSet ds = DB.GetDS("select Country.CountryID,Country.Name,ShippingMethodToCountryMap.ShippingMethodID from Country " + DB.GetNoLock() + " left outer join ShippingMethodToCountryMap " + DB.GetNoLock() + " on Country.CountryID=ShippingMethodToCountryMap.CountryID and ShippingMethodToCountryMap.ShippingMethodID=" + ShippingMethodID.ToString() + " order by displayorder,name",false);
				foreach(DataRow row in ds.Tables[0].Rows)
				{
					bool AllowedForThisCountry = DB.RowFieldInt(row,"ShippingMethodID") != 0;
					writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">");
					writer.Write("<td>");
					writer.Write(DB.RowField(row,"Name"));
					writer.Write("</td>");
					writer.Write("<td>");
					writer.Write("<input type=\"checkbox\" name=\"CountryList\" value=\"" + DB.RowFieldInt(row,"CountryID").ToString() + "\" " + CommonLogic.IIF(AllowedForThisCountry, " checked ","") + ">");
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
