// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/popuprt.aspx.cs 3     8/19/06 8:49p Buddy $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for popuprt.
	/// </summary>
	public partial class popuprt : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
			
			Response.Write("<html>\n");
			Response.Write("<head>\n");
			Response.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\n");
			Response.Write("<title>Admin Popup</title>\n");
			Response.Write("<link rel=\"stylesheet\" href=\"skins/Skin_" + ThisCustomer.SkinID.ToString() + "/style.css\" type=\"text/css\">\n");
			Response.Write("<script type=\"text/javascript\" src=\"jscripts/formValidate.js\"></script>\n");
			Response.Write("</head>\n");
			Response.Write("<body style=\"margin: 0px;\" bottommargin=\"0\" leftmargin=\"0\" marginheight=\"0\" marginwidth=\"0\" rightmargin=\"0\" topmargin=\"0\" bgcolor=\"#FFFFFF\" onLoad=\"self.focus()\">\n");

			Response.Write("<div style=\"margin-left: 10px;\">");
			if(!ThisCustomer.IsAdminUser)
			{
				Response.Write("<b><font color=red>PERMISSION DENIED</b></font>");
			}
			else
			{
				IDataReader rs = DB.GetRS("Select * from orders  " + DB.GetNoLock() + " where ordernumber=" + CommonLogic.QueryStringUSInt("OrderNumber").ToString());
				if(rs.Read())
				{
					String r1 = DB.RSField(rs,"RTShipRequest");
					String r2 = DB.RSField(rs,"RTShipResponse");
					String rqst = String.Empty;
					try
					{
						rqst = XmlCommon.PrettyPrintXml(r1);
					}
					catch
					{
						rqst = r1;
					}
					String resp = String.Empty;
					try
					{
						resp = XmlCommon.PrettyPrintXml(r2);
					}
					catch
					{
						resp = r2;
					}
					Response.Write("<b>RT Shipping Request: </b><br/><br/><textarea rows=\"20\" style=\"width: 90%\">" + Server.HtmlEncode(r1) + "</textarea><br/><br/>");
					Response.Write("<b>RT Shipping Response: </b><br/><br/><textarea rows=\"35\" style=\"width: 90%\">" + Server.HtmlEncode(r2) + "</textarea><br/><br/>");
				}
				else
				{
					Response.Write("<b><font color=red>ORDER NOT FOUND</b></font>");
				}
				rs.Close();
			}

			Response.Write("</div>");
			
			Response.Write("</body>\n");
			Response.Write("</html>\n");
		}

	}
}
