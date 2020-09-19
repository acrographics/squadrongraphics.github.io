// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2004.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/checkoutshippingmult.aspx.cs 11    9/30/06 10:44p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for checkoutshippingmult.
    /// </summary>
    public partial class checkoutshippingmult : SkinBase
    {

        ShoppingCart cart = null;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            //Response.CacheControl = "no-cache";
            Response.Expires = -1;
            Response.AddHeader("pragma", "no-cache");
            Response.AddHeader("Last-Modified", DateTime.Now.AddMinutes(-10).ToUniversalTime() + " GMT" );
            Response.AddHeader("Cache-Control", "no-store, no-cache, must-revalidate"); // HTTP/1.1
            Response.AddHeader("Cache-Control", "post-check=0, pre-check=0");
            Response.AddHeader("Pragma", "no-cache"); // HTTP/1.0

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

            SectionTitle = AppLogic.GetString("checkoutshippingmult.aspx.1", SkinID, ThisCustomer.LocaleSetting);
            cart = new ShoppingCart(SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);

            cart.ValidProceedCheckout(); // will not come back from this if any issue. they are sent back to the cart page!

            if (cart.IsAllDownloadComponents() || cart.IsAllFreeShippingComponents() || cart.IsAllSystemComponents() || !Shipping.MultiShipEnabled() || cart.TotalQuantity() == 1 || (cart.TotalQuantity() > AppLogic.MultiShipMaxNumItemsAllowed()) || !cart.CartAllowsShippingMethodSelection)
            {
                // not allowed then:
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutshippingmult.aspx.12", SkinID, ThisCustomer.LocaleSetting)));
            }

            // don't allow multi ship checkout if cart sum(Q) = 1
            if (cart.TotalQuantity() < 2)
            {
                Response.Redirect("checkoutshipping.aspx");
            }

            CartItem FirstCartItem = (CartItem)cart.CartItems[0];
            Address FirstItemShippingAddress = new Address();
            FirstItemShippingAddress.LoadByCustomer(ThisCustomer.CustomerID, FirstCartItem.m_ShippingAddressID, AddressTypes.Shipping);
            if (FirstItemShippingAddress.AddressID == 0)
            {
                // not allowed here anymore!
                Response.Redirect("shoppingcart.aspx?errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutshippingmult.aspx.10", SkinID, ThisCustomer.LocaleSetting)));
            }

            if (!IsPostBack && CommonLogic.FormCanBeDangerousContent("update") == "" && CommonLogic.FormCanBeDangerousContent("continue") == "" && CommonLogic.QueryStringCanBeDangerousContent("setallprimary") == "")
            {
                UpdatepageContent();
            }

            if (CommonLogic.FormCanBeDangerousContent("update") != "" || CommonLogic.FormCanBeDangerousContent("continue") != "" || CommonLogic.QueryStringCanBeDangerousContent("setallprimary") != "")
            {
                ProcessCart();
            }
            JSPopupRoutines.Text = AppLogic.GetJSPopupRoutines();

            AppLogic.eventHandler("CheckoutShipping").CallEvent("&CheckoutShipping=true");
        }

        private void UpdatepageContent()
        {
            //set header graphic image and set the hotspot alternate text
            checkoutheadergraphic.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_3.gif");
            ((RectangleHotSpot)checkoutheadergraphic.HotSpots[0]).AlternateText = AppLogic.GetString("checkoutshippingmult.aspx.3", SkinID, ThisCustomer.LocaleSetting);
            ((RectangleHotSpot)checkoutheadergraphic.HotSpots[1]).AlternateText = AppLogic.GetString("checkoutshippingmult.aspx.4", SkinID, ThisCustomer.LocaleSetting);


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
            String XmlPackageName = AppLogic.AppConfig("XmlPackage.CheckoutShippingMultPageHeader");
            if (XmlPackageName.Length != 0)
            {
                XmlPackage_CheckoutShippingPageHeader.Text = AppLogic.RunXmlPackage(XmlPackageName, base.GetParser, ThisCustomer, SkinID, String.Empty, String.Empty, true, true);
            }


            //
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
                    Reason = AppLogic.GetString("checkoutshippingmult.aspx.5", SkinID, ThisCustomer.LocaleSetting);
                }
                if (cart.FreeShippingReason == Shipping.FreeShippingReasonEnum.AllFreeShippingItems)
                {
                    Reason = AppLogic.GetString("checkoutshippingmult.aspx.25", SkinID, ThisCustomer.LocaleSetting);
                }
                if (cart.FreeShippingReason == Shipping.FreeShippingReasonEnum.CouponHasFreeShipping)
                {
                    Reason = AppLogic.GetString("checkoutshippingmult.aspx.6", SkinID, ThisCustomer.LocaleSetting);
                }
                if (cart.FreeShippingReason == Shipping.FreeShippingReasonEnum.AllOrdersHaveFreeShipping)
                {
                    Reason = String.Format(AppLogic.GetString("checkoutshippingmult.aspx.7", SkinID, ThisCustomer.LocaleSetting), cart.FreeShippingMethod);
                }
                if (cart.FreeShippingReason == Shipping.FreeShippingReasonEnum.CustomerLevelHasFreeShipping)
                {
                    Reason = String.Format(AppLogic.GetString("checkoutshippingmult.aspx.8", SkinID, ThisCustomer.LocaleSetting), ThisCustomer.CustomerLevelName);
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

            checkoutshippingmultaspx16.Text = "<p><b>" + AppLogic.GetString("checkoutshippingmult.aspx.16", SkinID, ThisCustomer.LocaleSetting) + "</b></p>";
            checkoutshippingmultaspx18.Text = "<p>" + String.Format(AppLogic.GetString("checkoutshippingmult.aspx.18", SkinID, ThisCustomer.LocaleSetting), "account.aspx?checkout=true", "checkoutshippingmult.aspx?setallprimary=true") + "</p>";

            CartItems.Text = WriteItemAddresses();

            String XmlPackageName2 = AppLogic.AppConfig("XmlPackage.CheckoutShippingMultPageFooter");
            if (XmlPackageName2.Length != 0)
            {
                XmlPackage_CheckoutShippingMultPageFooter.Text = AppLogic.RunXmlPackage(XmlPackageName2, base.GetParser, ThisCustomer, SkinID, String.Empty, String.Empty, true, true);
            }

        }
        private string WriteItemAddresses()
        {
            StringBuilder s = new StringBuilder("");

            s.Append("<script type=\"text/javascript\">\n");

            s.Append("function AddressIsComplete(SrcID)\n");
            s.Append("{\n");
            s.Append("if(document.getElementById('AddressFirstName_' + SrcID).value == '') return false;\n");
            s.Append("if(document.getElementById('AddressLastName_' + SrcID).value == '') return false;\n");
            s.Append("if(document.getElementById('AddressAddress1_' + SrcID).value == '') return false;\n");
            s.Append("if(document.getElementById('AddressCity_' + SrcID).value == '') return false;\n");
            s.Append("if(document.getElementById('AddressState_' + SrcID).selectedIndex == 0) return false;;\n");
            s.Append("if(document.getElementById('AddressZip_' + SrcID).value == '') return false;\n");
            s.Append("return (true);\n");
            s.Append("}\n");

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

            s.Append("function CheckoutShippingMultForm_Validator(theForm)\n");
            s.Append("{\n");
            s.Append("submitonce(theForm);\n");

            foreach (CartItem c in cart.CartItems)
            {
                if (!c.m_IsDownload && c.m_SKU != AppLogic.ro_PMMicropay)
                {
                    for (int i = 1; i <= c.m_Quantity; i++)
                    {
                        String ThisID = c.m_ShoppingCartRecordID.ToString() + "_" + i.ToString();
                        s.Append("myOption" + ThisID + " = -1;\n");
                        s.Append("for(i = 0; i < theForm.ShipToType_" + ThisID + ".length; i++)\n");
                        s.Append("{\n");
                        s.Append("	if (theForm.ShipToType_" + ThisID + "[i].checked)\n");
                        s.Append("	{\n");
                        s.Append("		myOption" + ThisID + " = i;\n");
                        s.Append("	}\n");
                        s.Append("}\n");
                        s.Append("if(myOption" + ThisID + " == -1)\n");
                        s.Append("{\n");
                        s.Append("    alert(\"" + AppLogic.GetString("checkoutshippingmult.aspx.21", SkinID, ThisCustomer.LocaleSetting) + "\");\n");
                        s.Append("	  theForm.ContinueCheckout.value='0';\n");
                        s.Append("    submitenabled(theForm);\n");
                        s.Append("    return (false);\n");
                        s.Append("}\n");

                        s.Append("if(get_radio_value(theForm.ShipToType_" + ThisID + ") == 'NewAddress')\n");
                        s.Append("{\n");
                        s.Append("	if(!AddressIsComplete('" + ThisID + "'))\n");
                        s.Append("	{\n");
                        s.Append("	    alert(\"" + AppLogic.GetString("checkoutshippingmult.aspx.22", SkinID, ThisCustomer.LocaleSetting) + "\");\n");
                        s.Append("		theForm.ContinueCheckout.value='0';\n");
                        s.Append("	    submitenabled(theForm);\n");
                        s.Append("	    return (false);\n");
                        s.Append("	}\n");
                        s.Append("}\n");

                    }
                }
            }

            s.Append("submitenabled(theForm);\n");
            s.Append("return (true);\n");
            s.Append("}\n");
            s.Append("</script>\n");

            s.Append("<form action=\"checkoutshippingmult.aspx\" method=\"post\" id=\"CheckoutShippingMultForm\" name=\"CheckoutShippingMultForm\" onsubmit=\"return (validateForm(this) && CheckoutShippingMultForm_Validator(this))\">\n");
            s.Append("<input type=\"hidden\" id=\"ContinueCheckout\" name=\"ContinueCheckout\" value=\"0\">");

            s.Append(cart.DisplayMultiShipAddressSelector(false, ThisCustomer));

            s.Append("<div align=\"center\">");
            s.Append("<br/>");
            s.Append("<input type=\"submit\" class=\"ShippingMultPageUpdateButton\" name=\"update\" value=\"" + AppLogic.GetString("checkoutshippingmult.aspx.19", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "\">");
            s.Append("&nbsp;&nbsp;");
            s.Append("<input type=\"submit\" class=\"ShippingMultPageContinueCheckoutButton\" name=\"continue\" value=\"" + AppLogic.GetString("checkoutshippingmult.aspx.20", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "\">");
            s.Append("</div>");

            s.Append("</form>");

            return s.ToString();

        }
        private void ProcessCart()
        {
            bool ContinueCheckout = (CommonLogic.FormCanBeDangerousContent("continue") != "");
            if (cart.IsEmpty())
            {
                Response.Redirect("shoppingcart.aspx");
            }

            if (CommonLogic.QueryStringBool("setallprimary"))
            {
                cart.ResetAllAddressToPrimaryShippingAddress();
                if (ContinueCheckout)
                {
                    Response.Redirect("checkoutshippingmult2.aspx");
                }
                else
                {
                    Response.Redirect("checkoutshippingmult.aspx");
                }
            }

            Hashtable NewAddresses = new Hashtable();
            Hashtable AddressIDs = new Hashtable();

            StringBuilder xmlDoc = new StringBuilder(4096);
            xmlDoc.Append("<root>");

            // add NEW address blocks, if necessary:
            foreach (CartItem c in cart.CartItems)
            {
                if (!c.m_IsDownload && c.m_SKU != AppLogic.ro_PMMicropay)
                {
                    for (int i = 1; i <= c.m_Quantity; i++)
                    {
                        int ThisAddressID = 0;
                        String ThisID = c.m_ShoppingCartRecordID.ToString() + "_" + i.ToString();
                        String ShipToType = CommonLogic.FormCanBeDangerousContent("ShipToType_" + ThisID);
                        switch (ShipToType.ToUpperInvariant())
                        {
                            case "NEWADDRESS":
                                {
                                    Address addr = new Address();
                                    addr.CustomerID = ThisCustomer.CustomerID;
                                    addr.NickName = CommonLogic.FormCanBeDangerousContent("AddressNickName_" + ThisID);
                                    addr.FirstName = CommonLogic.FormCanBeDangerousContent("AddressFirstName_" + ThisID);
                                    addr.LastName = CommonLogic.FormCanBeDangerousContent("AddressLastName_" + ThisID);
                                    addr.Address1 = CommonLogic.FormCanBeDangerousContent("AddressAddress1_" + ThisID);
                                    addr.Address2 = CommonLogic.FormCanBeDangerousContent("AddressAddress2_" + ThisID);
                                    addr.Company = CommonLogic.FormCanBeDangerousContent("AddressCompany_" + ThisID);
                                    addr.Suite = CommonLogic.FormCanBeDangerousContent("AddressSuite_" + ThisID);
                                    addr.City = CommonLogic.FormCanBeDangerousContent("AddressCity_" + ThisID);
                                    addr.State = CommonLogic.FormCanBeDangerousContent("AddressState_" + ThisID);
                                    addr.Zip = CommonLogic.FormCanBeDangerousContent("AddressZip_" + ThisID);
                                    addr.Country = CommonLogic.FormCanBeDangerousContent("AddressCountry_" + ThisID);
                                    addr.Phone = CommonLogic.FormCanBeDangerousContent("AddressPhone_" + ThisID);

                                    // did we add this address already?
                                    if (NewAddresses.ContainsKey(addr.Address1))
                                    {
                                        ThisAddressID = System.Int32.Parse(NewAddresses[addr.Address1].ToString());
                                    }
                                    else
                                    {
                                        addr.AddressType = AddressTypes.Shipping;
                                        addr.InsertDB();
                                        NewAddresses.Add(addr.Address1, addr.AddressID.ToString());
                                        ThisAddressID = addr.AddressID;
                                    }
                                    break;
                                }
                            case "GIFTREGISTRYADDRESS":
                                {
                                    int GiftCustomerID = c.m_GiftRegistryForCustomerID;
                                    ThisAddressID = AppLogic.GiftRegistryShippingAddressID(GiftCustomerID);
                                    break;
                                }
                            case "EXISTINGADDRESS":
                            case "":
                                {
                                    ThisAddressID = CommonLogic.FormUSInt(ThisID);
                                    break;
                                }
                        }
                        if (ThisAddressID > 0)
                        {
                            xmlDoc.Append(String.Format("<row cartid=\"{0}\" addressid=\"{1}\" />", c.m_ShoppingCartRecordID.ToString(), ThisAddressID.ToString()));
                        }
                        else
                        {
                            UpdatepageContent();
                            ErrorMsgLabel.Text = AppLogic.GetString("checkoutshippingmult.aspx.27", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                            pnlErrorMsg.Visible = true;
                            return;
                        }
                    }
                }
            }
            xmlDoc.Append("</root>");

            cart.SetAddressesToXmlSpec(xmlDoc.ToString());

            if (!ContinueCheckout)
            {
                //Response.Redirect("checkoutshippingmult.aspx");
                UpdatepageContent();
            }
            else
            {
                Response.Redirect("checkoutshippingmult2.aspx");
            }
        }
    }
}
