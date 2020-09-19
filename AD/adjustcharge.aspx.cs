// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/adjustcharge.aspx.cs 5     9/30/06 3:38p Redwoodtree $
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
    /// Summary description for adjustcharge.
    /// </summary>
    public partial class adjustcharge : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            int ONX = CommonLogic.QueryStringUSInt("OrderNumber");


            Response.Write("<html>\n");
            Response.Write("<head>\n");
            Response.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\n");
            Response.Write("<title>Ad/Hoc Charge/Refund</title>\n");
            Response.Write("<link rel=\"stylesheet\" href=\"skins/Skin_" + ThisCustomer.SkinID.ToString() + "/style.css\" type=\"text/css\">\n");
            Response.Write("<script type=\"text/javascript\" src=\"jscripts/formValidate.js\"></script>\n");
            Response.Write("</head>\n");
            Response.Write("<body style=\"margin: 0px;\" bottommargin=\"0\" leftmargin=\"0\" marginheight=\"0\" marginwidth=\"0\" rightmargin=\"0\" topmargin=\"0\" bgcolor=\"#FFFFFF\" onLoad=\"self.focus()\">\n");
            Response.Write("<div style=\"margin-left: 10px;\" align=\"left\">");

            if (!ThisCustomer.IsAdminUser)
            {
                Response.Write("<b><font color=red>PERMISSION DENIED</b></font>");
            }
            else
            {
                if (CommonLogic.FormBool("IsSubmit"))
                {
                    if (CommonLogic.FormCanBeDangerousContent("OrderTotal").Trim().Length != 0)
                    {
                        try
                        {
                            Decimal NewOrderTotal = CommonLogic.FormNativeDecimal("OrderTotal");
                            if (NewOrderTotal != 0.0M)
                            {
                                DB.ExecuteSQL(String.Format("update orders set CustomerServiceNotes={0}, OrderTotal={1} where OrderNumber={2}", DB.SQuote(CommonLogic.FormCanBeDangerousContent("CustomerServiceNotes")), Localization.CurrencyStringForDBWithoutExchangeRate(NewOrderTotal), ONX.ToString()));
                            }
                        }
                        catch { }
                    }
                    Response.Write("\n<script type=\"text/javascript\">\n");
                    Response.Write("opener.window.location.reload();\n");
                    Response.Write("self.close();\n");
                    Response.Write("</script>\n");
                }
                else
                {
                    Decimal OrderTotal = 0.0M;
                    String CustomerServiceNotes = String.Empty;
                    IDataReader rs = DB.GetRS(String.Format("select * from Orders {0} where OrderNumber={1}", DB.GetNoLock(), ONX.ToString()));
                    if (rs.Read())
                    {
                        OrderTotal = DB.RSFieldDecimal(rs, "OrderTotal");
                        CustomerServiceNotes = DB.RSField(rs, "CustomerServiceNotes");
                    }
                    rs.Close();

                    Response.Write("<script type=\"text/javascript\">\n");
                    Response.Write("function AdjustChargeForm_Validator(theForm)\n");
                    Response.Write("	{\n");
                    Response.Write("	submitonce(theForm);\n");
                    Response.Write("	return (true);\n");
                    Response.Write("	}\n");
                    Response.Write("</script>\n");

                    Response.Write("<b>Adjust Charge CAPTURE Total For Order: " + ONX.ToString() + "</b><br/><br/>");
                    Response.Write("<form id=\"AdjustChargeForm\" name=\"AdjustChargeForm\" method=\"POST\" action=\"adjustcharge.aspx?OrderNumber=" + ONX.ToString() + "\" onsubmit=\"return (validateForm(this) && AdjustChargeForm_Validator(this))\" >");
                    Response.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                    Response.Write("<p>Manually adjust the order total before CAPTURING it.<br/><br/>This capability ONLY applies to AUTH transactions before CAPTURE!</p>");
                    Response.Write("<p>New Order Total: <input type=\"text\" name=\"OrderTotal\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(OrderTotal) + "\"><input type=\"hidden\" name=\"OrderTotal_vldt\" value=\"[req][number][blankalert=Please enter the new order TOTAL amount, e.g. 25.00][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\"></p>");
                    Response.Write("<p>Customer Service Notes For Order:<br/>");
                    Response.Write("<textarea id=\"CustomerServiceNotes\" name=\"CustomerServiceNotes\" rows=\"20\" cols=\"50\">" + Server.HtmlEncode(CustomerServiceNotes) + "</textarea>");
                    Response.Write("<p><input type=\"submit\" value=\"Submit\" name=\"B1\"><input type=\"button\" value=\"Cancel\" name=\"B2\" onClick=\"javascript:self.close()\"></p>");
                    Response.Write("</form>");
                }
            }

            Response.Write("</div>");



            Response.Write("</body>\n");
            Response.Write("</html>\n");
        }

    }
}
