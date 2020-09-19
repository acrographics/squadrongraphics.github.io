// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2004.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/checkoutshippingmult2.aspx.cs 10    9/30/06 10:30p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for checkoutshippingmult2.
    /// </summary>
    public partial class checkoutshippingmult2 : SkinBase
    {

        ShoppingCart cart = null;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = -1;
            Response.AddHeader("pragma", "no-cache");

            if (AppLogic.AppConfigBool("RequireOver13Checked") && !ThisCustomer.IsOver13)
            {
                Response.Redirect("shoppingcart.aspx?errormsg=" + Server.UrlEncode(AppLogic.GetString("checkout.over13required", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)));
            }

            RequireSecurePage();

            // -----------------------------------------------------------------------------------------------
            // NOTE ON PAGE LOAD LOGIC:
            // We are checking here for required elements to allowing the customer to stay on this page.
            // Many of these checks may be redundant, and they DO add a bit of overhead in terms of db calls, but ANYTHING really
            // could have changed since the customer was on the last page. Remember, the web is completely stateless. Assume this
            // page was executed by ANYONE at ANYTIME (even someone trying to break the cart). 
            // It could have been yesterday, or 1 second ago, and other customers could have purchased limitied inventory products, 
            // coupons may no longer be valid, etc, etc, etc...
            // -----------------------------------------------------------------------------------------------
            ThisCustomer.RequireCustomerRecord();

            if (!ThisCustomer.IsRegistered)
            {
                bool boolAllowAnon = AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout");
                if (!boolAllowAnon && ThisCustomer.PrimaryBillingAddressID > 0)
                {
                    Address BillingAddress = new Address();
                    BillingAddress.LoadByCustomer(ThisCustomer.CustomerID, ThisCustomer.PrimaryBillingAddressID, AddressTypes.Billing);
                    if (BillingAddress.PaymentMethodLastUsed == AppLogic.ro_PMPayPalExpress || BillingAddress.PaymentMethodLastUsed == AppLogic.ro_PMPayPalExpressMark)
                    {
                        boolAllowAnon = AppLogic.AppConfigBool("PayPal.Express.AllowAnonCheckout");
                    }
                }

                if (!boolAllowAnon)
                {
                    Response.Redirect("createaccount.aspx?checkout=true");
                }
            }
            if (ThisCustomer.PrimaryBillingAddressID == 0 || ThisCustomer.PrimaryShippingAddressID == 0)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutpayment.aspx.2", SkinID, ThisCustomer.LocaleSetting)));
            }

            SectionTitle = AppLogic.GetString("checkoutshippingmult2.aspx.1", SkinID, ThisCustomer.LocaleSetting);
            cart = new ShoppingCart(SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);

            cart.ValidProceedCheckout(); // will not come back from this if any issue. they are sent back to the cart page!

            if (cart.IsAllDownloadComponents() || cart.IsAllFreeShippingComponents() || cart.IsAllSystemComponents() || !Shipping.MultiShipEnabled() || cart.TotalQuantity() == 1 || (cart.TotalQuantity() > AppLogic.MultiShipMaxNumItemsAllowed()) || !cart.CartAllowsShippingMethodSelection)
            {
                // not allowed here:
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutshippingmult2.aspx.12", SkinID, ThisCustomer.LocaleSetting)));
            }

            CartItem FirstCartItem = (CartItem)cart.CartItems[0];
            Address FirstItemShippingAddress = new Address();
            FirstItemShippingAddress.LoadByCustomer(ThisCustomer.CustomerID, FirstCartItem.m_ShippingAddressID, AddressTypes.Shipping);
            if (FirstItemShippingAddress.AddressID == 0)
            {
                // not allowed here anymore!
                Response.Redirect("shoppingcart.aspx?errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutshippingmult2.aspx.10", SkinID, ThisCustomer.LocaleSetting)));
            }


            if (!IsPostBack && CommonLogic.FormCanBeDangerousContent("update") == "" && CommonLogic.FormCanBeDangerousContent("continue") == "")
            {
                UpdatepageContent();
            }

            if (CommonLogic.FormCanBeDangerousContent("update") != "" || CommonLogic.FormCanBeDangerousContent("continue") != "")
            {
                ProcessCart();
            }
            JSPopupRoutines.Text = AppLogic.GetJSPopupRoutines();
        }

        private void UpdatepageContent()
        {
            //set header graphic image and set the hotspot alternate text
            checkoutheadergraphic.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_3.gif");
            ((RectangleHotSpot)checkoutheadergraphic.HotSpots[0]).AlternateText = AppLogic.GetString("checkoutshippingmult2.aspx.3", SkinID, ThisCustomer.LocaleSetting);
            ((RectangleHotSpot)checkoutheadergraphic.HotSpots[1]).AlternateText = AppLogic.GetString("checkoutshippingmult2.aspx.4", SkinID, ThisCustomer.LocaleSetting);
            ((RectangleHotSpot)checkoutheadergraphic.HotSpots[2]).AlternateText = AppLogic.GetString("checkoutshippingmult2.aspx.5", SkinID, ThisCustomer.LocaleSetting);


            if (CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg").Length != 0)
            {
                AppLogic.CheckForScriptTag(CommonLogic.QueryStringCanBeDangerousContent("errormsg"));
                ErrorMsgLabel.Text = "<p align=\"left\"><span class=\"errorLg\">" + Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg")) + "</span></p>";
                pnlErrorMsg.Visible = true;
            }
            else
            {
                pnlErrorMsg.Visible = false;
            }

            //write out header package is it exists
            String XmlPackageName = AppLogic.AppConfig("XmlPackage.CheckoutShippingMult2PageHeader");
            if (XmlPackageName.Length != 0)
            {
                XmlPackage_CheckoutShippingMult2PageHeader.Text = AppLogic.RunXmlPackage(XmlPackageName, base.GetParser, ThisCustomer, SkinID, String.Empty, String.Empty, true, true);
            }


            if (!cart.ShippingIsFree && cart.MoreNeededToReachFreeShipping != 0.0M)
            {
                GetFreeShipping.Text = "<div class=\"FreeShippingThresholdPrompt\">";
                GetFreeShipping.Text += String.Format(AppLogic.GetString("checkoutshippingmult.aspx.2", SkinID, ThisCustomer.LocaleSetting), ThisCustomer.CurrencyString(cart.FreeShippingThreshold), CommonLogic.Capitalize(cart.FreeShippingMethod));
                GetFreeShipping.Text += "<br/>&nbsp;";
                GetFreeShipping.Text += "</div>";
                pnlGetFreeShipping.Visible = true;
            }
            else
            {
                pnlGetFreeShipping.Visible = false;
            }

            if (cart.ShippingIsFree)
            {
                String Reason = String.Empty;
                if (cart.FreeShippingReason == Shipping.FreeShippingReasonEnum.AllDownloadItems)
                {
                    Reason = AppLogic.GetString("checkoutshippingmult2.aspx.5", SkinID, ThisCustomer.LocaleSetting);
                }
                if (cart.FreeShippingReason == Shipping.FreeShippingReasonEnum.AllFreeShippingItems)
                {
                    Reason = AppLogic.GetString("checkoutshippingmult2.aspx.24", SkinID, ThisCustomer.LocaleSetting);
                }
                if (cart.FreeShippingReason == Shipping.FreeShippingReasonEnum.CouponHasFreeShipping)
                {
                    Reason = AppLogic.GetString("checkoutshippingmult2.aspx.6", SkinID, ThisCustomer.LocaleSetting);
                }
                if (cart.FreeShippingReason == Shipping.FreeShippingReasonEnum.AllOrdersHaveFreeShipping)
                {
                    Reason = String.Format(AppLogic.GetString("checkoutshippingmult2.aspx.7", SkinID, ThisCustomer.LocaleSetting), cart.FreeShippingMethod);
                }
                if (cart.FreeShippingReason == Shipping.FreeShippingReasonEnum.CustomerLevelHasFreeShipping)
                {
                    Reason = String.Format(AppLogic.GetString("checkoutshippingmult2.aspx.8", SkinID, ThisCustomer.LocaleSetting), ThisCustomer.CustomerLevelName);
                }
                if (cart.FreeShippingReason == Shipping.FreeShippingReasonEnum.ExceedsFreeShippingThreshold)
                {
                    Reason = cart.FreeShippingMethod;
                }
                IsFreeShipping.Text = String.Format(AppLogic.GetString("checkoutshippingmult.aspx.9", SkinID, ThisCustomer.LocaleSetting), Reason);
                pnlIsFreeShipping.Visible = true;
            }
            else
            {
                pnlIsFreeShipping.Visible = false;
            }

            CartItems.Text = WriteItemAddresses();

            String XmlPackageName2 = AppLogic.AppConfig("XmlPackage.CheckoutShippingMult2PageFooter");
            if (XmlPackageName2.Length != 0)
            {
                XmlPackage_CheckoutShippingMult2PageFooter.Text = AppLogic.RunXmlPackage(XmlPackageName2, base.GetParser, ThisCustomer, SkinID, String.Empty, String.Empty, true, true);
            }

        }
        private string WriteItemAddresses()
        {
            StringBuilder s = new StringBuilder("");

            s.Append("<p><b>");
            s.Append(AppLogic.GetString("checkoutshippingmult2.aspx.16", SkinID, ThisCustomer.LocaleSetting));
            s.Append("</b></p>");

            s.Append("<script type=\"text/javascript\">\n");

            s.Append("function get_radio_value(theRadio)\n");
            s.Append("{\n");
            s.Append("for (var i=0; i < theRadio.length; i++)\n");
            s.Append("   {\n");
            s.Append("   if (theRadio[i].checked)\n");
            s.Append("      {\n");
            s.Append("      var rad_val = theRadio[i].value;\n");
            s.Append("		return rad_val;\n");
            s.Append("      }\n");
            s.Append("   }\n");
            s.Append("return '';\n");
            s.Append("}\n");

            // make sure one shipping method is selected for each address id (ignore download & system products):
            s.Append("function CheckoutShippingMult2Form_Validator(theForm)\n");
            s.Append("{\n");
            s.Append("submitonce(theForm);\n");

            String DistinctAddrIds = cart.GetDistinctShippingAddressIDs(false, false);
            if (DistinctAddrIds.Length != 0)
            {
                foreach (String AddressID in DistinctAddrIds.Split(','))
                {
                    s.Append("myOption" + AddressID + " = -1;\n");
                    s.Append("for(i = 0; i < theForm.ShippingMethodID_" + AddressID + ".length; i++)\n");
                    s.Append("{\n");
                    s.Append("	if (theForm.ShippingMethodID_" + AddressID + "[i].checked)\n");
                    s.Append("	{\n");
                    s.Append("		myOption" + AddressID + " = i;\n");
                    s.Append("	}\n");
                    s.Append("}\n");
                    s.Append("if(myOption" + AddressID + " == -1)\n");
                    s.Append("{\n");
                    s.Append("    alert(\"" + AppLogic.GetString("checkoutshippingmult2.aspx.21", SkinID, ThisCustomer.LocaleSetting) + "\");\n");
                    s.Append("	  theForm.ContinueCheckout.value='0';\n");
                    s.Append("    submitenabled(theForm);\n");
                    s.Append("    return (false);\n");
                    s.Append("}\n");
                }
            }

            s.Append("submitenabled(theForm);\n");
            s.Append("return (true);\n");
            s.Append("}\n");
            s.Append("</script>\n");

            s.Append("<form action=\"checkoutshippingmult2.aspx\" method=\"post\" id=\"CheckoutShippingMult2Form\" name=\"CheckoutShippingMult2Form\" onsubmit=\"return (validateForm(this) && CheckoutShippingMult2Form_Validator(this))\">\n");
            s.Append("<input type=\"hidden\" id=\"ContinueCheckout\" name=\"ContinueCheckout\" value=\"0\">");
            if (DistinctAddrIds.Length != 0)
            {
                foreach (String AddressID in DistinctAddrIds.Split(','))
                {
                    s.Append("<input type=\"hidden\" id=\"ShippingMethodID_" + AddressID + "\" name=\"ShippingMethodID_" + AddressID + "\" value=\"\">"); // must have this so all ShippingMethodID radio lists are arrays, even if there is only ONE radio button on the form (stupid javascript)
                }
            }

            s.Append(cart.DisplayMultiShipMethodSelector(false, ThisCustomer));

            s.Append("<div align=\"center\">");
            s.Append("<br/>");
            s.Append("<input type=\"submit\" class=\"ShippingMultPageUpdateButton\" name=\"update\" value=\"" + AppLogic.GetString("checkoutshippingmult2.aspx.19", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "\">");
            s.Append("&nbsp;&nbsp;");
            s.Append("<input type=\"submit\" class=\"ShippingMultPageContinueCheckoutButton\" name=\"continue\" value=\"" + AppLogic.GetString("checkoutshippingmult2.aspx.20", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "\">");
            s.Append("</div>");

            s.Append("</form>");

            return s.ToString();

        }
        private void ProcessCart()
        {
            if (cart.IsEmpty())
            {
                Response.Redirect("shoppingcart.aspx");
            }

            bool ContinueCheckout = (CommonLogic.FormCanBeDangerousContent("continue") != "");

            String DistinctAddrIds = cart.GetDistinctShippingAddressIDs(false, false);
            if (DistinctAddrIds.Length != 0)
            {
                foreach (String AddressID in DistinctAddrIds.Split(','))
                {
                    String ShippingMethodIDFormField = CommonLogic.FormCanBeDangerousContent("ShippingMethodID_" + AddressID).Replace(",", ""); // remember to remove the stupid hidden field which adds a comma to the form post (stupid stupid javascript again)
                    int ShippingMethodID = 0;
                    String ShippingMethod = String.Empty;
                    if (cart.ShipCalcID != Shipping.ShippingCalculationEnum.UseRealTimeRates)
                    {
                        ShippingMethodID = Localization.ParseUSInt(ShippingMethodIDFormField);
                        ShippingMethod = Shipping.GetShippingMethodName(ShippingMethodID, null);
                    }
                    else
                    {
                        String[] frmsplit = ShippingMethodIDFormField.Split('|');
                        ShippingMethodID = Localization.ParseUSInt(frmsplit[0]);
                        ShippingMethod = String.Format("{0}|{1}", frmsplit[1], frmsplit[2]);
                    }

                    String sql = String.Format("update ShoppingCart set ShippingMethodID={0}, ShippingMethod={1} where CustomerID={2} and CartType={3} and ShippingAddressID={4}", ShippingMethodID.ToString(), DB.SQuote(ShippingMethod), ThisCustomer.CustomerID.ToString(), ((int)CartTypeEnum.ShoppingCart).ToString(), AddressID);
                    DB.ExecuteSQL(sql);

                    //find and update the existing cart item so it's available later when displaying the page.
                    for (int i = 0; i < cart.CartItems.Capacity; i++)
                    {
                        if (((CartItem)cart.CartItems[i]).m_ShippingAddressID.ToString() == AddressID)
                        {
                            CartItem ci = (CartItem)cart.CartItems[i];
                            ci.m_ShippingMethod = ShippingMethod;
                            ci.m_ShippingMethodID = ShippingMethodID;
                            string itemnotes = Request.Form["notes_" + ci.m_ShoppingCartRecordID.ToString()];
                            if (itemnotes != null)
                            {
                                cart.SetItemNotes(ci.m_ShoppingCartRecordID, itemnotes);
                                ci.m_Notes = itemnotes;
                            }
                            cart.CartItems[i] = ci;
                            //break;
                        }
                    }
                }

            }

            if (!ContinueCheckout)
            {
                //Response.Redirect("checkoutshippingmult2.aspx");
                UpdatepageContent();
            }
            else
            {
                if (ThisCustomer.ThisCustomerSession["PayPalExpressToken"] == "")
                {
                    Response.Redirect("checkoutpayment.aspx");
                }
                else
                {
                    Response.Redirect("checkoutreview.aspx?PaymentMethod=PAYPALEXPRESS");
                }
            }
        }
    }
}
