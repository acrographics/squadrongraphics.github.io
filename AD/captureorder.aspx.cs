// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/captureorder.aspx.cs 7     9/30/06 3:39p Redwoodtree $
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
	/// Summary description for captureorder.
	/// </summary>
	public partial class captureorder : System.Web.UI.Page
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
			Response.Write("<title>Capture Order</title>\n");
			Response.Write("<link rel=\"stylesheet\" href=\"skins/Skin_" + ThisCustomer.SkinID.ToString() + "/style.css\" type=\"text/css\">\n");
			Response.Write("<script type=\"text/javascript\" src=\"jscripts/formValidate.js\"></script>\n");
			Response.Write("</head>\n");
			Response.Write("<body style=\"margin: 0px;\" bottommargin=\"0\" leftmargin=\"0\" marginheight=\"0\" marginwidth=\"0\" rightmargin=\"0\" topmargin=\"0\" bgcolor=\"#FFFFFF\" onLoad=\"self.focus()\">\n");
			Response.Write("<div align=\"left\">");
			int ONX = CommonLogic.QueryStringUSInt("OrderNumber");
            Order ord = new Order(ONX, ThisCustomer.LocaleSetting);
            Customer c = new Customer(ord.CustomerID);

            if (!ThisCustomer.IsAdminUser) // safety check
			{
				Response.Write("<b><font color=red>PERMISSION DENIED</b></font>");
			}
			else
			{
				Response.Write("<b>CAPTURE ORDER: " + ONX.ToString() + "</b><br/><br/>");
                String Status = Gateway.OrderManagement_DoCapture(ord, ThisCustomer.LocaleSetting);
                Response.Write("Capture Status: " + Status);
                if (Status == AppLogic.ro_OK)
                {
                    Response.Write("<script type=\"text/javascript\">\n");
                    Response.Write("opener.window.location.reload();");
                    Response.Write("</script>\n");
                }
			}
			Response.Write("</div>");
			Response.Write("<p align=\"center\"><a href=\"javascript:self.close();\">Close</a></p>");
			Response.Write("</body>\n");
			Response.Write("</html>\n");
		}
	}
}
