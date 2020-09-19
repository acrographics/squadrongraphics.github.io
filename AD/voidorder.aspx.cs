// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/voidorder.aspx.cs 7     9/30/06 3:38p Redwoodtree $
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
    /// Summary description for voidorder.
    /// </summary>
    public partial class voidorder : System.Web.UI.Page
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
            Response.Write("<title>Void Order</title>\n");
            Response.Write("<link rel=\"stylesheet\" href=\"skins/Skin_" + ThisCustomer.SkinID.ToString() + "/style.css\" type=\"text/css\">\n");
            Response.Write("<script type=\"text/javascript\" src=\"formValidate.js\"></script>\n");
            Response.Write("</head>\n");
            Response.Write("<body style=\"margin: 0px;\" bottommargin=\"0\" leftmargin=\"0\" marginheight=\"0\" marginwidth=\"0\" rightmargin=\"0\" topmargin=\"0\" bgcolor=\"#FFFFFF\" onLoad=\"self.focus()\">\n");
            Response.Write("<div align=\"left\">");

            if (!ThisCustomer.IsAdminUser) // safety check
            {
                Response.Write("<b><font color=red>PERMISSION DENIED</b></font>");
            }
            else
            {
                int ONX = CommonLogic.QueryStringUSInt("OrderNumber");
                Order ord = new Order(ONX, ThisCustomer.LocaleSetting);
                Response.Write("<b>VOID ORDER: " + ONX.ToString() + "</b><br/><br/>");
                if (CommonLogic.FormCanBeDangerousContent("IsSubmit") == "true")
                {
                    String Status = Gateway.OrderManagement_DoVoid(ord, ThisCustomer.LocaleSetting);
                    Response.Write("Void Status: " + Status);
                    if (Status == AppLogic.ro_OK)
                    {
                        Response.Write("<script type=\"text/javascript\">\n");
                        Response.Write("opener.window.location.reload();");
                        Response.Write("</script>\n");
                    }
                    Response.Write("<p align=\"center\"><a href=\"javascript:self.close();\">Close</a></p>");
                }
                else
                {
                    Response.Write("<form method=\"POST\" action=\"voidorder.aspx?ordernumber=" + ONX.ToString() + "&confirm=yes\" id=\"RefundOrderForm\" name=\"RefundOrderForm\">");
                    Response.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">");
                    Response.Write("<p align=\"center\">Are you sure you want to void this order?<br/><br/></p>");
                    Response.Write("<p align=\"center\"><input type=\"submit\" name=\"submit\" value=\"&nbsp;&nbsp;Yes&nbsp;&nbsp;\">");
                    Response.Write("<img src=\"images/spacer.gif\" width=\"100\" height=\"1\">");
                    Response.Write("<input type=\"button\" name=\"cancel\" value=\"&nbsp;&nbsp;No&nbsp;&nbsp;\" onClick=\"javascript:self.close();\">");
                    Response.Write("</p>");
                    Response.Write("</form>");
                }
            }
            Response.Write("</div>");
            Response.Write("</body>\n");
            Response.Write("</html>\n");
        }

    }
}
