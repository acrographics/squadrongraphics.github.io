// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/paypalreauthorder.aspx.cs 1     4/12/2007 1:39p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;
using AspDotNetStorefrontCommon;
using AspDotNetStorefrontGateways;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
    /// Summary description for paypalreauthorder.
	/// </summary>
	public partial class paypalreauthorder : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

			Response.Write("<html>\n");
			Response.Write("<head>\n");
			Response.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\n");
			Response.Write("<title>Reauthorize Order</title>\n");
			Response.Write("<link rel=\"stylesheet\" href=\"skins/Skin_" + ThisCustomer.SkinID.ToString() + "/style.css\" type=\"text/css\">\n");
			Response.Write("<script type=\"text/javascript\" src=\"jscripts/formValidate.js\"></script>\n");
			Response.Write("</head>\n");
			Response.Write("<body style=\"margin: 0px;\" bottommargin=\"0\" leftmargin=\"0\" marginheight=\"0\" marginwidth=\"0\" rightmargin=\"0\" topmargin=\"0\" bgcolor=\"#FFFFFF\" onLoad=\"self.focus()\">\n");
			Response.Write("<div align=\"left\">");
			int ONX = CommonLogic.QueryStringUSInt("OrderNumber");

            if (!ThisCustomer.IsAdminUser) // safety check
			{
				Response.Write("<b><font color=red>PERMISSION DENIED</b></font>");
			}
			else
			{
				Response.Write("<b>REAUTHORIZE ORDER: " + ONX.ToString() + "</b><br/><br/>");
				IDataReader rs = DB.GetRS("Select * from orders  " + DB.GetNoLock() + " where ordernumber=" + ONX.ToString());
				if(rs.Read())
				{
					String PM = AppLogic.CleanPaymentMethod(DB.RSField(rs,"PaymentMethod"));
                    if (DB.RSFieldDateTime(rs, "CapturedOn") == System.DateTime.MinValue)
					{
						if(DB.RSField(rs,"TransactionState") == AppLogic.ro_TXStateAuthorized)
						{
                            String Status = String.Empty;
                            String GW = AppLogic.CleanPaymentGateway(DB.RSField(rs, "PaymentGateway"));
                            if (PM == AppLogic.ro_PMPayPal || PM == AppLogic.ro_PMPayPalExpress)
                            {
                                GW = Gateway.ro_GWPAYPAL;
                            }
                            AspDotNetStorefrontGateways.PayPal gw = new AspDotNetStorefrontGateways.PayPal();
                            Status = gw.ReAuthorizeOrder(ONX);
                            Response.Write("<b>Reauthorize Response:</b> " + Status + "<br/><br/>");
                            Response.Write("<script type=\"text/javascript\">\n");
                            Response.Write("opener.window.location.reload();");
                            Response.Write("</script>\n");
                        }
						else
						{
							Response.Write("The transaction state (" + DB.RSField(rs,"TransactionState") + ") is not AUTH.");
						}
					}
					else
					{
						Response.Write("The payment for this order was already captured on " + Localization.ToNativeDateTimeString(DB.RSFieldDateTime(rs,"CapturedOn")) + ".");
					}

				}
				else
				{
					Response.Write("<b><font color=red>ORDER NOT FOUND</b></font>");
				}
				rs.Close();
			}

			Response.Write("</div>");

			Response.Write("<p align=\"center\"><a href=\"javascript:self.close();\">Close</a></p>");
			
			Response.Write("</body>\n");
			Response.Write("</html>\n");
		}
	}
}
