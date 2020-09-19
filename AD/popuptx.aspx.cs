// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/popuptx.aspx.cs 9     10/03/06 7:56p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for popuptx.
    /// </summary>
    public partial class popuptx : System.Web.UI.Page
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
            Response.Write("<title>Transaction Details For Order #: " + CommonLogic.QueryStringUSInt("OrderNumber").ToString() + "</title>\n");
            Response.Write("<link rel=\"stylesheet\" href=\"skins/Skin_" + ThisCustomer.SkinID.ToString() + "/style.css\" type=\"text/css\">\n");
            Response.Write("<script type=\"text/javascript\" src=\"jscripts/formValidate.js\"></script>\n");
            Response.Write("</head>\n");
            Response.Write("<body style=\"margin: 0px;\" bottommargin=\"0\" leftmargin=\"0\" marginheight=\"0\" marginwidth=\"0\" rightmargin=\"0\" topmargin=\"0\" bgcolor=\"#FFFFFF\" onLoad=\"self.focus()\">\n");

            bool IsEcheck = false;
            bool IsMicroPay = false;
            bool IsCard = false;

            if (!ThisCustomer.IsAdminUser)
            {
                Response.Write("<b><font color=red>PERMISSION DENIED</b></font>");
            }
            else
            {
                IDataReader rs = DB.GetRS("Select * from orders  " + DB.GetNoLock() + " where ordernumber=" + CommonLogic.QueryStringUSInt("OrderNumber").ToString());
                if (rs.Read())
                {
                    IsEcheck = (DB.RSField(rs, "PaymentMethod").ToUpperInvariant().Trim() == AppLogic.ro_PMECheck);
                    //V3_9
                    IsMicroPay = (AppLogic.CleanPaymentMethod(DB.RSField(rs, "PaymentMethod")) == AppLogic.ro_PMMicropay);
                    IsCard = (AppLogic.CleanPaymentMethod(DB.RSField(rs, "PaymentMethod")) == AppLogic.ro_PMCreditCard);
                    //V3_9
                    if (IsEcheck || IsMicroPay || IsCard)
                    {
                        Response.Write("<b>Order Number: </b>" + CommonLogic.QueryStringUSInt("OrderNumber").ToString() + "<br/>");
                        Response.Write("<b>Customer ID: </b>" + DB.RSFieldInt(rs, "CustomerID").ToString() + "<br/>");
                        Response.Write("<b>Order Date: </b>" + DB.RSFieldDateTime(rs, "OrderDate").ToString() + "<br/>");
                        Response.Write("<b>Order Total: </b>" + ThisCustomer.CurrencyString(DB.RSFieldDecimal(rs, "OrderTotal")) + "<br/>");
                        Response.Write("<b>Card Type: </b>" + DB.RSField(rs, "CardType") + "<br/>");
                        Response.Write("<b>Payment Gateway: </b>" + DB.RSField(rs, "PaymentGateway") + "<br/>");
                        Response.Write("<b>Transaction State: </b>" + DB.RSField(rs, "TransactionState") + "<br/>");

                        String _cardNumber = AppLogic.SafeDisplayCardNumber(DB.RSField(rs, "CardNumber"), "Orders", CommonLogic.QueryStringUSInt("OrderNumber"));
                        String _cardType = DB.RSField(rs, "CardType");

                        if (IsEcheck)
                        {
                            Response.Write(String.Format("<b>ECheck Bank Name: </b> {0}<br/>", DB.RSField(rs, "ECheckBankName")));
                            Response.Write(String.Format("<b>ECheck ABA: </b> {0}<br/>", DB.RSField(rs, "ECheckBankABACode")));
                            Response.Write(String.Format("<b>ECheck Account: </b> {0}<br/>", DB.RSField(rs, "ECheckBankAccountNumber")));
                            Response.Write(String.Format("<b>ECheck Account Name: </b> {0}<br/>", DB.RSField(rs, "ECheckBankAccountName")));
                            Response.Write(String.Format("<b>ECheck Account Type: </b> {0}<br/>", DB.RSField(rs, "ECheckBankAccountType")));
                        }
                        //V3_9
                        if (IsMicroPay)
                        {
                            Response.Write("<b>Micropay Transaction:</b>");
                        }
                        //V3_9
                        else
                        {
                            if (_cardType.StartsWith(AppLogic.ro_PMPayPal, StringComparison.InvariantCultureIgnoreCase))
                            {
                                Response.Write("<b>Card Number: </b>");
                            }
                            else
                            {
                                Response.Write("<b>Card Number:</b> " + _cardNumber + "<br/>");
                            }
                            if (_cardNumber.Length == 0 || _cardNumber == AppLogic.ro_CCNotStoredString)
                            {
                                Response.Write("<b>Card Expiration:</b> Not Available<br/><,br>>");
                            }
                            else
                            {
                                if (_cardType.StartsWith(AppLogic.ro_PMPayPal, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    Response.Write("<b>Card Expiration:</b>  Not Available<br/>");
                                }
                                else
                                {
                                    Response.Write("<b>Card Expiration:</b> " + DB.RSField(rs, "CardExpirationMonth") + "/" + DB.RSField(rs, "cardExpirationYear") + "<br/>");
                                }
                            }
                        }

                        Response.Write("<b>Transaction Command: </b>" + Server.HtmlEncode(DB.RSField(rs, "TransactionCommand")).Replace(" ", "&nbsp;").Replace("\r\n", "<br/>") + "<br/>");
                        Response.Write("<b>Authorization Result: </b>" + Server.HtmlEncode(DB.RSField(rs, "AuthorizationResult")).Replace(" ", "&nbsp;").Replace("\r\n", "<br/>") + "<br/>");
                        Response.Write("<b>Authorization Code: </b>" + Server.HtmlEncode(DB.RSField(rs, "AuthorizationCode")) + "<br/>");
                        Response.Write("<b>Transaction ID: </b>" + DB.RSField(rs, "AuthorizationPNREF") + "<br/>");
                        Response.Write("<hr size=\"1\"/>");
                        Response.Write("<b>Capture TX Command: </b>" + Server.HtmlEncode(CommonLogic.IIF(DB.RSField(rs, "CaptureTXCommand").Length == 0, AppLogic.ro_NotApplicable, DB.RSField(rs, "CaptureTXCommand"))).Replace(" ", "&nbsp;").Replace("\r\n", "<br/>") + "<br/>");
                        Response.Write("<b>Capture TX Result: </b>" + Server.HtmlEncode(CommonLogic.IIF(DB.RSField(rs, "CaptureTXResult").Length == 0, AppLogic.ro_NotApplicable, DB.RSField(rs, "CaptureTXResult"))).Replace(" ", "&nbsp;").Replace("\r\n", "<br/>") + "<br/>");
                        Response.Write("<b>Void TX Command: </b>" + Server.HtmlEncode(CommonLogic.IIF(DB.RSField(rs, "VoidTXCommand").Length == 0, AppLogic.ro_NotApplicable, DB.RSField(rs, "VoidTXCommand"))).Replace(" ", "&nbsp;").Replace("\r\n", "<br/>") + "<br/>");
                        Response.Write("<b>Void TX Result: </b>" + Server.HtmlEncode(CommonLogic.IIF(DB.RSField(rs, "VoidTXResult").Length == 0, AppLogic.ro_NotApplicable, DB.RSField(rs, "VoidTXResult"))).Replace(" ", "&nbsp;").Replace("\r\n", "<br/>") + "<br/>");
                        Response.Write("<b>Refund TX Command: </b>" + Server.HtmlEncode(CommonLogic.IIF(DB.RSField(rs, "RefundTXCommand").Length == 0, AppLogic.ro_NotApplicable, DB.RSField(rs, "RefundTXCommand"))).Replace(" ", "&nbsp;").Replace("\r\n", "<br/>") + "<br/>");
                        Response.Write("<b>Refund TX Result: </b>" + Server.HtmlEncode(CommonLogic.IIF(DB.RSField(rs, "RefundTXResult").Length == 0, AppLogic.ro_NotApplicable, DB.RSField(rs, "RefundTXResult"))).Replace(" ", "&nbsp;").Replace("\r\n", "<br/>") + "<br/>");

                        if (AppLogic.AppConfigBool("CardinalCommerce.Centinel.Enabled"))
                        {
                            Response.Write("<b>Cardinal Lookup Result: </b>" + Server.HtmlEncode(CommonLogic.IIF(DB.RSField(rs, "CardinalLookupResult").Length == 0, AppLogic.ro_NotApplicable, DB.RSField(rs, "CardinalLookupResult"))) + "<br/>");
                            Response.Write("<b>Cardinal Authenticate Result: </b>" + Server.HtmlEncode(CommonLogic.IIF(DB.RSField(rs, "CardinalAuthenticateResult").Length == 0, AppLogic.ro_NotApplicable, DB.RSField(rs, "CardinalAuthenticateResult"))) + "<br/>");
                            Response.Write("<b>Cardinal Gateway Parms: </b>" + Server.HtmlEncode(CommonLogic.IIF(DB.RSField(rs, "CardinalGatewayParms").Length == 0, AppLogic.ro_NotApplicable, DB.RSField(rs, "CardinalGatewayParms"))) + "<br/>");
                        }

                    }
                    else
                    {
                        Response.Write(AppLogic.ro_NotApplicable);
                    }
                }
                else
                {
                    Response.Write("<b><font color=red>ORDER NOT FOUND</b></font>");
                }
                rs.Close();
            }

            Response.Write("<p align=\"center\"><a href=\"javascript:self.close();\">Close</a></p>");


            Response.Write("</body>\n");
            Response.Write("</html>\n");
        }

    }
}
