// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2006.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/shoppingcart.aspx.cs 31    10/03/06 7:56p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Xml;
using System.Xml.Xsl;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Globalization;
using AspDotNetStorefrontCommon;
using ASPDNSFControls;
using AspDotNetStorefrontGateways;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for ShoppingCartPage.
    /// </summary>
    public partial class ShoppingCartPage : SkinBase
    {
        string SkinImagePath = String.Empty;
        ShoppingCart cart = null;
        bool VATEnabled = false;
        bool VATOn = false;
        int CountryID = 0;
        int StateID = 0;
        string ZipCode = string.Empty;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            SkinImagePath = "skins/skin_" + SkinID.ToString() + "/images/";
            this.RequireCustomerRecord();
            RequireSecurePage();
            SectionTitle = AppLogic.GetString("AppConfig.CartPrompt", SkinID, ThisCustomer.LocaleSetting);
            ClearErrors();

            if (AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
            {
                // don't need here, it's redundant with the regular checkout button:
                btnQuickCheckoutTop.Visible = false;
                btnQuickCheckoutBottom.Visible = false;
            }
            else
            {
                btnQuickCheckoutTop.Visible = AppLogic.AppConfigBool("QuickCheckout.Enabled");
                btnQuickCheckoutBottom.Visible = AppLogic.AppConfigBool("QuickCheckout.Enabled");
            }

            VATEnabled = AppLogic.AppConfigBool("VAT.Enabled");
            VATOn = (VATEnabled && ThisCustomer.VATSettingReconciled == VATSettingEnum.ShowPricesInclusiveOfVAT);

            if (VATEnabled)
            {
                CountryID = AppLogic.AppConfigUSInt("VAT.CountryID");
                StateID = 0;
                ZipCode = string.Empty;

                if (ThisCustomer.IsRegistered)
                {
                    if (ThisCustomer.PrimaryShippingAddress.CountryID > 0)
                    {
                        CountryID = ThisCustomer.PrimaryShippingAddress.CountryID;
                    }
                    if (ThisCustomer.PrimaryShippingAddress.StateID > 0)
                    {
                        StateID = ThisCustomer.PrimaryShippingAddress.StateID;
                    }
                    if (ThisCustomer.PrimaryShippingAddress.Zip.Trim().Length != 0)
                    {
                        ZipCode = ThisCustomer.PrimaryShippingAddress.Zip.Trim();
                    }
                }
            }


            if (!this.IsPostBack)
            {
                string ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnUrl");
                AppLogic.CheckForScriptTag(ReturnURL);
                ViewState["ReturnURL"] = ReturnURL;
                InitializePageContent();
            }
            else
            {
                cart = new ShoppingCart(SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);
            }

            string[] formkeys = Request.Form.AllKeys;
            foreach (String s in formkeys)
            {
                if (s == "bt_Delete")
                {
                    ProcessCart(false, false,false);
                    InitializePageContent();
                }
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            OrderOptionsList.ItemDataBound += new System.Web.UI.WebControls.RepeaterItemEventHandler(OrderOptionsList_ItemDataBound);
            btnContinueShoppingTop.Click += new EventHandler(btnContinueShoppingTop_Click);
            btnContinueShoppingBottom.Click += new EventHandler(btnContinueShoppingBottom_Click);
            btnCheckOutNowTop.Click += new EventHandler(btnCheckOutNowTop_Click);
            btnCheckOutNowBottom.Click += new EventHandler(btnCheckOutNowBottom_Click);
            btnInternationalCheckOutNowTop.Click += new EventHandler(btnInternationalCheckOutNowTop_Click);
            btnInternationalCheckOutNowBottom.Click += new EventHandler(btnInternationalCheckOutNowBottom_Click);
            btnQuickCheckoutTop.Click += new EventHandler(btnQuickCheckoutTop_Click);
            btnQuickCheckoutBottom.Click += new EventHandler(btnQuickCheckoutBottom_Click);
            btnUpdateCart1.Click += new EventHandler(btnUpdateCart1_Click);
            btnUpdateCart2.Click += new EventHandler(btnUpdateCart2_Click);
            btnUpdateCart3.Click += new EventHandler(btnUpdateCart3_Click);
            btnUpdateCart4.Click += new EventHandler(btnUpdateCart4_Click);
            btnUpdateCart5.Click += new EventHandler(btnUpdateCart5_Click);
        }
        #endregion


        void OrderOptionsList_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            Image imgControl = (Image)e.Item.FindControl("OptionImage");
            String ImgUrl = AppLogic.LookupImage("OrderOption", Convert.ToInt32(((XmlNode)e.Item.DataItem)["OrderOptionID"].InnerText), "icon", SkinID, ThisCustomer.LocaleSetting);
            if (ImgUrl.Length != 0 && ImgUrl.IndexOf("nopicture") == -1)
            {
                imgControl.ImageUrl = ImgUrl;
                imgControl.Visible = true;
            }

            Image helpCircle = (Image)e.Item.FindControl("helpcircle_gif");
            helpCircle.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "helpcircle.gif");
            helpCircle.Attributes.Add("onclick", "popuporderoptionwh('Order Option " + ((XmlNode)e.Item.DataItem)["OrderOptionID"].InnerText + "', " + ((XmlNode)e.Item.DataItem)["OrderOptionID"].InnerText + ",650,550,'yes');");

            DataCheckBox cbk = (DataCheckBox)e.Item.FindControl("OrderOptions");
            cbk.Checked = cart.OptionIsSelected(Convert.ToInt32(((XmlNode)e.Item.DataItem)["OrderOptionID"].InnerText), cart.OrderOptions) || (!IsPostBack && Convert.ToInt32(((XmlNode)e.Item.DataItem)["DefaultIsChecked"].InnerText) == 1);
            //cbk.Checked = cart.OptionIsSelected(Convert.ToInt32(((XmlNode)e.Item.DataItem)["OrderOptionID"].InnerText), cart.OrderOptions);

            Label price = (Label)e.Item.FindControl("OrderOptionPrice");
            decimal cost = Convert.ToDecimal(((XmlNode)e.Item.DataItem)["Cost"].InnerText);
            int TaxClassID = Convert.ToInt32(((XmlNode)e.Item.DataItem)["TaxClassID"].InnerText);
            if (cost == System.Decimal.Zero)
            {
                price.Text = AppLogic.GetString("shoppingcart.aspx.16", this.ThisCustomer.SkinID, this.ThisCustomer.LocaleSetting);
            }
            else
            {
                decimal TaxRate = ThisCustomer.TaxRate(TaxClassID);
                decimal VAT = cost * (TaxRate / 100.0M);
                if (VATOn)
                {
                    cost += VAT;
                }
                price.Text = ThisCustomer.CurrencyString(cost);
                if (VATEnabled)
                {
                    price.Text += "&nbsp;";
                    price.Text += CommonLogic.IIF(VATOn, AppLogic.GetString("setvatsetting.aspx.6", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), AppLogic.GetString("setvatsetting.aspx.7", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
                    price.Text += "<br/>VAT: " + ThisCustomer.CurrencyString(VAT);
                }
            }

        }
        void btnContinueShoppingTop_Click(object sender, EventArgs e)
        {
            ContinueShopping();
        }
        void btnContinueShoppingBottom_Click(object sender, EventArgs e)
        {
            ContinueShopping();
        }
        void btnCheckOutNowTop_Click(object sender, EventArgs e)
        {
            ProcessCart(true, false, false);
        }
        void btnCheckOutNowBottom_Click(object sender, EventArgs e)
        {
            ProcessCart(true, false, false);
        }
        void btnInternationalCheckOutNowTop_Click(object sender, EventArgs e)
        {
            ProcessCart(true, false, true);
        }
        void btnInternationalCheckOutNowBottom_Click(object sender, EventArgs e)
        {
            ProcessCart(true, false, true);
        }
        void btnQuickCheckoutTop_Click(object sender, EventArgs e)
        {
            ProcessCart(true, true, false);
        }
        void btnQuickCheckoutBottom_Click(object sender, EventArgs e)
        {
            ProcessCart(true, true, false);
        }
        void btnUpdateCart1_Click(object sender, EventArgs e)
        {
            ProcessCart(false, false, false);
            InitializePageContent();
        }
        void btnUpdateCart2_Click(object sender, EventArgs e)
        {
            ProcessCart(false, false, false);
            InitializePageContent();
        }
        void btnUpdateCart3_Click(object sender, EventArgs e)
        {
            ProcessCart(false, false, false);
            InitializePageContent();
        }
        void btnUpdateCart4_Click(object sender, EventArgs e)
        {
            ProcessCart(false, false, false);
            InitializePageContent();
        }
        void btnUpdateCart5_Click(object sender, EventArgs e)
        {
            ProcessCart(false, false, false);
            InitializePageContent();
        }


        public void InitializePageContent()
        {
            int AgeCartDays = AppLogic.AppConfigUSInt("AgeCartDays");
            if (AgeCartDays == 0)
            {
                AgeCartDays = 7;
            }

            ShoppingCart.Age(ThisCustomer.CustomerID, AgeCartDays, CartTypeEnum.ShoppingCart);
            cart = new ShoppingCart(SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);
            shoppingcartaspx8.Text = AppLogic.GetString("shoppingcart.aspx.8", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartaspx10.Text = AppLogic.GetString("shoppingcart.aspx.10", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartaspx11.Text = AppLogic.GetString("shoppingcart.aspx.11", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartaspx12.Text = AppLogic.GetString("shoppingcart.aspx.12", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartaspx13.Text = AppLogic.GetString("shoppingcart.aspx.13", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartaspx14.Text = AppLogic.GetString("shoppingcart.aspx.14", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartaspx15.Text = AppLogic.GetString("shoppingcart.aspx.15", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartaspx9.Text = AppLogic.GetString("shoppingcart.aspx.9", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartcs27.Text = AppLogic.GetString("shoppingcart.cs.27", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartcs28.Text = AppLogic.GetString("shoppingcart.cs.28", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartcs29.Text = AppLogic.GetString("shoppingcart.cs.29", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartcs31.Text = AppLogic.GetString("shoppingcart.cs.31", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartcs96.Text = AppLogic.GetString("shoppingcart.cs.96", SkinID, ThisCustomer.LocaleSetting);
            btnUpdateCart1.Text = AppLogic.GetString("shoppingcart.cs.110", SkinID, ThisCustomer.LocaleSetting);
            btnUpdateCart2.Text = AppLogic.GetString("shoppingcart.cs.110", SkinID, ThisCustomer.LocaleSetting);
            btnUpdateCart3.Text = AppLogic.GetString("shoppingcart.cs.110", SkinID, ThisCustomer.LocaleSetting);
            btnUpdateCart4.Text = AppLogic.GetString("shoppingcart.cs.110", SkinID, ThisCustomer.LocaleSetting);
            btnUpdateCart5.Text = AppLogic.GetString("shoppingcart.cs.110", SkinID, ThisCustomer.LocaleSetting);
            lblOrderNotes.Text = AppLogic.GetString("shoppingcart.cs.66", SkinID, ThisCustomer.LocaleSetting);
            btnContinueShoppingTop.Text = AppLogic.GetString("shoppingcart.cs.62", SkinID, ThisCustomer.LocaleSetting);
            btnContinueShoppingBottom.Text = AppLogic.GetString("shoppingcart.cs.62", SkinID, ThisCustomer.LocaleSetting);
            btnCheckOutNowTop.Text = AppLogic.GetString("shoppingcart.cs.111", SkinID, ThisCustomer.LocaleSetting);
            btnCheckOutNowBottom.Text = AppLogic.GetString("shoppingcart.cs.111", SkinID, ThisCustomer.LocaleSetting);

            bool reqOver13 = AppLogic.AppConfigBool("RequireOver13Checked");
            btnCheckOutNowTop.Enabled = !cart.IsEmpty() && !cart.RecurringScheduleConflict && (!reqOver13 || (reqOver13 && ThisCustomer.IsOver13)) || !ThisCustomer.IsRegistered;
            btnCheckOutNowBottom.Enabled = btnCheckOutNowTop.Enabled;
            ErrorMsgLabel.Text = CommonLogic.IIF(!cart.IsEmpty() && (reqOver13 && !ThisCustomer.IsOver13 && ThisCustomer.IsRegistered), AppLogic.GetString("Over13OnCheckout", SkinID, ThisCustomer.LocaleSetting), String.Empty);

            PayPalExpressSpan.Visible = false;
            PayPalExpressSpan2.Visible = false;

            Decimal MinOrderAmount = AppLogic.AppConfigUSDecimal("CartMinOrderAmount");

            if (!cart.IsEmpty() && !cart.ContainsRecurringAutoShip)
            {
                // Enable PayPalExpress if using PayPalPro or PayPal Express is an active payment method.
                bool IncludePayPalExpress = false;

                if (AppLogic.AppConfigBool("PayPal.Express.ShowOnCartPage") && cart.MeetsMinimumOrderAmount(MinOrderAmount))
                {
                    if (AppLogic.ActivePaymentGatewayCleaned() == Gateway.ro_GWPAYPALPRO)
                    {
                        IncludePayPalExpress = true;
                    }
                    else
                    {
                        foreach (String PM in AppLogic.AppConfig("PaymentMethods").ToUpperInvariant().Split(','))
                        {
                            String PMCleaned = AppLogic.CleanPaymentMethod(PM);
                            if (PMCleaned == AppLogic.ro_PMPayPalExpress)
                            {
                                IncludePayPalExpress = true;
                                break;
                            }
                        }
                    }
                }

                if (IncludePayPalExpress)
                {
                    if (AppLogic.AppConfigBool("PayPal.Promo.Enabled")
                        && cart.Total(true) >= AppLogic.AppConfigNativeDecimal("PayPal.Promo.CartMinimum")
                        && cart.Total(true) <= AppLogic.AppConfigNativeDecimal("PayPal.Promo.CartMaximum"))
                    {
                        btnPayPalExpressCheckout.ImageUrl = AppLogic.AppConfig("PayPal.Promo.ButtonImageURL");
                    }
                    else
                    {
                        btnPayPalExpressCheckout.ImageUrl = AppLogic.AppConfig("PayPal.Express.ButtonImageURL");
                    }

                    btnPayPalExpressCheckout2.ImageUrl = btnPayPalExpressCheckout.ImageUrl;
                    PayPalExpressSpan.Visible = true;
                    PayPalExpressSpan2.Visible = true;
                }
            }

            string googleimageurl = String.Format(AppLogic.AppConfig("GoogleCheckout.LiveCheckoutButton"), AppLogic.AppConfig("GoogleCheckout.MerchantId"));
            if (AppLogic.AppConfigBool("GoogleCheckout.UseSandbox"))
            {
                googleimageurl = String.Format(AppLogic.AppConfig("GoogleCheckout.SandBoxCheckoutButton"), AppLogic.AppConfig("GoogleCheckout.SandboxMerchantId"));
            }
            googleimageurl = CommonLogic.IIF(Request.IsSecureConnection, googleimageurl.ToLower().Replace("http://", "https://"), googleimageurl);
            btnGoogleCheckout.ImageUrl = googleimageurl;
            btnGoogleCheckout2.ImageUrl = googleimageurl;

            bool ForceGoogleOff = false;
            if (cart.IsEmpty() || cart.ContainsRecurringAutoShip || !cart.MeetsMinimumOrderAmount(MinOrderAmount) || ThisCustomer.ThisCustomerSession["IGD"].Length != 0 || (AppLogic.AppConfig("GoogleCheckout.MerchantId").Length == 0 && AppLogic.AppConfig("GoogleCheckout.SandboxMerchantId").Length == 0))
            {
                GoogleCheckoutSpan.Visible = false;
                GoogleCheckoutSpan2.Visible = false;
                ForceGoogleOff = true; // these conditions force google off period (don't care about other settings)
            }

            if (!AppLogic.AppConfigBool("GoogleCheckout.ShowOnCartPage"))
            {
                // turn off the google checkout, but not in a forced condition, as the mall may turn it back on
                GoogleCheckoutSpan.Visible = false;
                GoogleCheckoutSpan2.Visible = false;
            }

            // allow the GooglerMall to turn google checkout back on, if not forced off prior and not already visible anyway:
            if (!ForceGoogleOff && !GoogleCheckoutSpan.Visible && (AppLogic.AppConfigBool("GoogleCheckout.GoogleMallEnabled") && CommonLogic.CookieCanBeDangerousContent("GoogleMall", false) != String.Empty))
            {
                GoogleCheckoutSpan.Visible = true;
                GoogleCheckoutSpan2.Visible = true;
            }

            if (GoogleCheckoutSpan.Visible || PayPalExpressSpan.Visible)
            {
                AlternativeCheckouts.Visible = true;
            }
            else
            {
                AlternativeCheckouts.Visible = false;
            }

            if (GoogleCheckoutSpan2.Visible || PayPalExpressSpan2.Visible)
            {
                AlternativeCheckouts2.Visible = true;
            }
            else
            {
                AlternativeCheckouts2.Visible = false;
            }

            if (!cart.IsEmpty())
            {
                // check cart for non-allowed google checkout shipping address
                bool gNonAllowedAddress = false;
                foreach (CartItem c in cart.CartItems)
                {
                    if (!c.m_IsDownload && !c.m_FreeShipping && !c.m_IsSystem)
                    {
                        if (c.m_ShippingAddressID != 0)
                        {
                            Address sa = new Address();
                            sa.LoadFromDB(c.m_ShippingAddressID);
                            if (sa.Country != "" && sa.Country.Trim().ToUpperInvariant() != "UNITED STATES" && sa.Country.Trim().ToUpperInvariant() != "UNITED KINGDOM")
                            {
                                gNonAllowedAddress = true;
                                break;
                            }
                        }
                    }
                }

                // hide GC button for carts with multiple shipping addresses, or non-US addresses
                imgGoogleCheckoutDisabled.Visible = cart.HasMultipleShippingAddresses() || gNonAllowedAddress;
                btnGoogleCheckout.Visible = !imgGoogleCheckoutDisabled.Visible;

                imgGoogleCheckout2Disabled.Visible = imgGoogleCheckoutDisabled.Visible;
                btnGoogleCheckout2.Visible = btnGoogleCheckout.Visible;

                if (GoogleCheckoutSpan.Visible)
                {
                    // only allow google checkout also if cart does not contain any "non allowed products for google checkout":
                    if (DB.GetSqlN("select count(*) as N from ShoppingCart where CartType=" + ((int)CartTypeEnum.ShoppingCart).ToString() + " and CustomerID=" + ThisCustomer.CustomerID.ToString() + " and ProductID in (select ProductID from Product where GoogleCheckoutAllowed=0)") > 0)
                    {
                        btnGoogleCheckout.Visible = false;
                        btnGoogleCheckout2.Visible = false;
                        imgGoogleCheckoutDisabled.Visible = true;
                        imgGoogleCheckout2Disabled.Visible = true;
                    }
                }
            }

            Shipping.ShippingCalculationEnum ShipCalcID = Shipping.GetActiveShippingCalculationID();
            String BACKURL = AppLogic.GetCartContinueShoppingURL(SkinID, ThisCustomer.LocaleSetting);


            StringBuilder html = new StringBuilder("");
            html.Append("<script type=\"text/javascript\">\n");
            html.Append("function Cart_Validator(theForm)\n");
            html.Append("{\n");
            String cartJS = CommonLogic.ReadFile("jscripts/shoppingcart.js", true);
            foreach (CartItem c in cart.CartItems)
            {
                html.Append(cartJS.Replace("%SKU%", c.m_ShoppingCartRecordID.ToString()));
            }
            html.Append("return(true);\n");
            html.Append("}\n");
            html.Append("</script>\n");

            ValidationScript.Text = html.ToString();

            JSPopupRoutines.Text = AppLogic.GetJSPopupRoutines();
            String XmlPackageName = AppLogic.AppConfig("XmlPackage.ShoppingCartPageHeader");
            if (XmlPackageName.Length != 0)
            {
                XmlPackage_ShoppingCartPageHeader.Text = AppLogic.RunXmlPackage(XmlPackageName, base.GetParser, ThisCustomer, SkinID, String.Empty, String.Empty, true, true);
            }

            String XRI = AppLogic.LocateImageURL(SkinImagePath + "redarrow.gif");
            redarrow1.ImageUrl = XRI;
            redarrow2.ImageUrl = XRI;
            redarrow3.ImageUrl = XRI;
            redarrow4.ImageUrl = XRI;

            ShippingInformation.Visible = (!AppLogic.AppConfigBool("SkipShippingOnCheckout") && !cart.IsAllFreeShippingComponents() && !cart.IsAllSystemComponents());
            AddresBookLlink.Visible = ThisCustomer.IsRegistered;

            btnCheckOutNowTop.Visible = (!cart.IsEmpty());



            if (cart.HasCoupon() && !cart.CouponIsValid)
            {
                pnlCouponError.Visible = true;
                CouponError.Text = cart.CouponStatusMessage + " (" + Server.HtmlEncode(CommonLogic.IIF(cart.Coupon.m_Code.Length != 0, cart.Coupon.m_Code, ThisCustomer.CouponCode)) + ")";
                cart.ClearCoupon();
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg").Length != 0 || ErrorMsgLabel.Text.Length > 0)
            {
                AppLogic.CheckForScriptTag(CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg"));
                pnlErrorMsg.Visible = true;
                ErrorMsgLabel.Text += Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg")).Replace("+", " ");
            }

            if (cart.InventoryTrimmed)
            {
                pnlInventoryTrimmedError.Visible = true;
                InventoryTrimmedError.Text = AppLogic.GetString("shoppingcart.aspx.3", SkinID, ThisCustomer.LocaleSetting);
            }

            if (cart.RecurringScheduleConflict)
            {
                pnlRecurringScheduleConflictError.Visible = true;
                RecurringScheduleConflictError.Text = AppLogic.GetString("shoppingcart.aspx.19", SkinID, ThisCustomer.LocaleSetting);
            }

            if (cart.MinimumQuantitiesUpdated)
            {
                pnlMinimumQuantitiesUpdatedError.Visible = true;
                MinimumQuantitiesUpdatedError.Text = AppLogic.GetString("shoppingcart.aspx.7", SkinID, ThisCustomer.LocaleSetting);
            }

            if (!cart.MeetsMinimumOrderAmount(MinOrderAmount))
            {
                pnlMeetsMinimumOrderAmountError.Visible = true;
                MeetsMinimumOrderAmountError.Text = String.Format(AppLogic.GetString("shoppingcart.aspx.4", SkinID, ThisCustomer.LocaleSetting), ThisCustomer.CurrencyString(MinOrderAmount));
            }

            int MinQuantity = AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout");
            if (!cart.MeetsMinimumOrderQuantity(MinQuantity))
            {
                pnlMeetsMinimumOrderQuantityError.Visible = true;
                MeetsMinimumOrderQuantityError.Text = String.Format(AppLogic.GetString("shoppingcart.cs.20", SkinID, ThisCustomer.LocaleSetting), MinQuantity.ToString(), MinQuantity.ToString());
            }


            if (AppLogic.MicropayIsEnabled() && AppLogic.AppConfigBool("Micropay.ShowTotalOnTopOfCartPage"))
            {
                pnlMicropay_EnabledError.Visible = true;
                Micropay_EnabledError.Text = "<div align=\"left\">" + String.Format(AppLogic.GetString("account.aspx.10", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), AppLogic.GetString("account.aspx.11", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), ThisCustomer.CurrencyString(ThisCustomer.MicroPayBalance)) + "</div>";
            }

            ShoppingCartGif.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "ShoppingCart.gif");
            CartItems.Text = cart.DisplayItems(false, ThisCustomer, true);
            pnlCartSummarySubTotals.Visible = !cart.IsEmpty();

            decimal RAWSubTotal = cart.SubTotal(false, false, true, true);
            decimal SubTotal = cart.SubTotal(true, false, true, true);
            if (RAWSubTotal == SubTotal)
            {
                shoppingcartcs96.Text = AppLogic.GetString("shoppingcart.cs.96", SkinID, ThisCustomer.LocaleSetting) + "&#0160;";
            }
            else
            {
                shoppingcartcs96.Text = AppLogic.GetString("shoppingcart.cs.97", SkinID, ThisCustomer.LocaleSetting) + "&#0160;";
            }
            CartSubTotal.Text = ThisCustomer.CurrencyString(cart.SubTotal(true, false, true, true));

            if (AppLogic.AppConfigBool("SkipShippingOnCheckout") || cart.IsAllFreeShippingComponents() || cart.IsAllSystemComponents())
            {
                ShippingLine.Visible = true;
            }

            if (!cart.HasTaxableComponents() || AppLogic.CustomerLevelHasNoTax(ThisCustomer.CustomerLevelID))
            {
                TaxLine.Visible = false;
            }

            if (!cart.IsEmpty())
            {

                ShoppingCartorderoptions_gif.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "ShoppingCartorderoptions.gif");
                string strXml = String.Empty;
                int optionscount = DB.GetXml("Select OrderOptionID, convert(decimal(10, 2), Cost) Cost, Name, DefaultIsChecked, TaxClassID from orderoption " + DB.GetNoLock() + " order by displayorder", "options", "orderoption", ref strXml);

                //if (ds.Tables[0].Rows.Count > 0)
                if (optionscount > 0)
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(strXml);
                    XmlDocument XslDoc = new XmlDocument();
                    XslDoc.LoadXml("<?xml version=\"1.0\"?><xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"><xsl:param name=\"locale\" /><xsl:template match=\"/\"><xsl:for-each select=\"*\"><xsl:copy><xsl:for-each select=\"*\"><xsl:copy><xsl:for-each select=\"*\"><xsl:copy><xsl:choose><xsl:when test=\"ml\"><xsl:value-of select=\"ml/locale[@name=$locale]\"/></xsl:when><xsl:otherwise><xsl:value-of select=\".\"/></xsl:otherwise></xsl:choose></xsl:copy></xsl:for-each></xsl:copy></xsl:for-each></xsl:copy></xsl:for-each></xsl:template></xsl:stylesheet>");
                    XslCompiledTransform xsl = new XslCompiledTransform();
                    xsl.Load(XslDoc);
                    TextWriter tw = new StringWriter();
                    XsltArgumentList XslArgs = new XsltArgumentList();
                    XslArgs.AddParam("locale", "", ThisCustomer.LocaleSetting);
                    xsl.Transform(xdoc, XslArgs, tw);
                    XmlDocument xresults = new XmlDocument();
                    xresults.LoadXml(tw.ToString());
                    XmlNodeList nodelist = xresults.SelectNodes("//orderoption");


                    OrderOptionsList.DataSource = nodelist;
                    OrderOptionsList.DataBind();
                    pnlOrderOptions.Visible = true;
                }
                else
                {
                    pnlOrderOptions.Visible = false;
                }


                string upsellproductlist = GetUpsellProducts(cart);
                if (upsellproductlist.Length > 0)
                {
                    UpsellProducts.Text = upsellproductlist;
                    btnUpdateCart5.Visible = true;
                    pnlUpsellProducts.Visible = true;
                }
                else
                {
                    pnlUpsellProducts.Visible = false;
                }

                if (cart.CouponsAllowed)
                {
                    ShoppingCartCoupon_gif.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "ShoppingCartCoupon.gif");
                    CouponCode.Text = Server.HtmlEncode(ThisCustomer.CouponCode);
                    pnlCoupon.Visible = true;
                }
                else
                {
                    pnlCoupon.Visible = false;
                }

                ShoppingCartNotes_gif.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "ShoppingCartNotes.gif");
                if (!AppLogic.AppConfigBool("DisallowOrderNotes"))
                {
                    OrderNotes.Text = HttpContext.Current.Server.HtmlEncode(cart.OrderNotes);
                    pnlOrderNotes.Visible = true;
                }
                else
                {
                    pnlOrderNotes.Visible = false;
                }

                btnCheckOutNowBottom.Visible = true;
            }
            else
            {
                pnlOrderOptions.Visible = false;
                pnlUpsellProducts.Visible = false;
                pnlCoupon.Visible = false;
                pnlOrderNotes.Visible = false;
                btnCheckOutNowBottom.Visible = false;
            }

            //if (BACKURL.ToLowerInvariant().IndexOf("javascript") == -1 && BACKURL.ToLowerInvariant().IndexOf("history") == -1)
            //{
            btnContinueShoppingBottom.OnClientClick = "self.location='" + BACKURL + "'";
            //}
            //else
            //{
            //    btnContinueShoppingBottom.OnClientClick = BACKURL;
            //}
            String XmlPackageName2 = AppLogic.AppConfig("XmlPackage.ShoppingCartPageFooter");
            if (XmlPackageName2.Length != 0)
            {
                XmlPackage_ShoppingCartPageFooter.Text = AppLogic.RunXmlPackage(XmlPackageName2, base.GetParser, ThisCustomer, SkinID, String.Empty, String.Empty, true, true);
            }

            // if only using google checkout, the customer may want to hide the coupon field, as they
            // must be entered on google's site:
            if (GoogleCheckoutSpan.Visible)
            {
                if (AppLogic.AppConfigBool("GoogleCheckout.HideCouponOnCartPage"))
                {
                    pnlCoupon.Visible = false;
                }
            }

            // handle international checkout buttons now (see internationalcheckout.com).
            if (btnCheckOutNowTop.Visible && AppLogic.AppConfigBool("InternationalCheckout.Enabled"))
            {
                // check to see if cart contains all known, and US addresses...if so, internationalcheckout should not be visible
                bool gAllUSAddresses = true;
                foreach (CartItem c in cart.CartItems)
                {
                    if (!c.m_IsDownload && !c.m_IsSystem && c.m_ShippingAddressID != 0)
                    {
                        Address sa = new Address();
                        sa.LoadFromDB(c.m_ShippingAddressID);
                        if (sa.Country.Trim().ToUpperInvariant() != "UNITED STATES")
                        {
                            gAllUSAddresses = false;
                            break;
                        }
                    }
                    else
                    {
                        gAllUSAddresses = false; // unknown address, or download or system product, etc, so it could be going anywhere
                        break;
                    }
                }

                if (!gAllUSAddresses && !cart.HasDownloadComponents() && !cart.HasGiftRegistryComponents() && !cart.HasCoupon() 
                    && !cart.HasMicropayProduct() && !cart.HasRecurringComponents() && !cart.HasMultipleShippingAddresses() 
                    && !cart.HasSystemComponents() && !cart.IsEmpty() && !cart.ContainsGiftCard() && !cart.HasPackComponents()
                    && !cart.HasKitComponents())
                {
                    btnInternationalCheckOutNowTop.Visible = true;
                    btnInternationalCheckOutNowBottom.Visible = btnInternationalCheckOutNowTop.Visible;

                    // I don't think this appconfig can be supported. the logic would be very difficult, commenting out for now.
                    //if (AppLogic.AppConfigBool("InternationalCheckout.ForceForInternationalCustomers"))
                    //{
                    //    btnCheckOutNowTop.Visible = false;
                    //    btnCheckOutNowBottom.Visible = false;
                    //    btnQuickCheckoutTop.Visible = false;
                    //    btnQuickCheckoutTop.Visible = false;
                    //}
                }
                else
                {
                    btnInternationalCheckOutNowTop.Visible = false;
                    btnInternationalCheckOutNowBottom.Visible = btnInternationalCheckOutNowTop.Visible;
                }
            }

        }

        public string GetUpsellProducts(ShoppingCart cart)
        {
            StringBuilder UpsellProductList = new StringBuilder(1024);
            StringBuilder results = new StringBuilder("");

            // ----------------------------------------------------------------------------------------
            // WRITE OUT UPSELL PRODUCTS:
            // ----------------------------------------------------------------------------------------
            if (AppLogic.AppConfigBool("ShowUpsellProductsOnCartPage"))
            {
                foreach (CartItem c in cart.CartItems)
                {
                    if (UpsellProductList.Length != 0)
                    {
                        UpsellProductList.Append(",");
                    }
                    UpsellProductList.Append(c.m_ProductID.ToString());
                }
                if (UpsellProductList.Length != 0)
                {
                    // get list of all upsell products for those products now in the cart:
                    String sql = "select UpsellProducts from Product " + DB.GetNoLock() + " where ProductID in (" + UpsellProductList.ToString() + ")";
                    IDataReader rs = DB.GetRS(sql);
                    UpsellProductList.Remove(0, UpsellProductList.Length);
                    while (rs.Read())
                    {
                        if (DB.RSField(rs, "UpsellProducts").Length != 0)
                        {
                            if (UpsellProductList.Length != 0)
                            {
                                UpsellProductList.Append(",");
                            }
                            UpsellProductList.Append(DB.RSField(rs, "UpsellProducts"));
                        }
                    }
                    rs.Close();
                    if (UpsellProductList.Length != 0)
                    {
                        int ShowN = AppLogic.AppConfigUSInt("UpsellProductsLimitNumberOnCart");
                        if (ShowN == 0)
                        {
                            ShowN = 10;
                        }
                        String S = String.Empty;
                        try
                        {
                            S = AppLogic.GetUpsellProductsBoxExpandedForCart(UpsellProductList.ToString(), ShowN, true, String.Empty, AppLogic.AppConfig("RelatedProductsFormat").ToUpperInvariant() == "GRID", SkinID, ThisCustomer);
                        }
                        catch { }
                        if (S.Length != 0)
                        {
                            results.Append(S);
                        }
                    }
                }
            }
            return results.ToString();
        }

        public void ProcessCart(bool DoingFullCheckout, bool ForceOnePageCheckout, bool InternationalCheckout)
        {
            Regex re = new Regex("^\\d{1,4}$");
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            ThisCustomer.RequireCustomerRecord();
            CartTypeEnum cte = CartTypeEnum.ShoppingCart;
            if (CommonLogic.QueryStringCanBeDangerousContent("CartType").Length != 0)
            {
                cte = (CartTypeEnum)CommonLogic.QueryStringUSInt("CartType");
            }
            cart = new ShoppingCart(1, ThisCustomer, cte, 0, false);

            if (cart.IsEmpty())
            {
                // can't have this at this point:
                switch (cte)
                {
                    case CartTypeEnum.ShoppingCart:
                        Response.Redirect("shoppingcart.aspx");
                        break;
                    case CartTypeEnum.WishCart:
                        Response.Redirect("wishlist.aspx");
                        break;
                    case CartTypeEnum.GiftRegistryCart:
                        Response.Redirect("giftregistry.aspx");
                        break;
                    default:
                        Response.Redirect("shoppingcart.aspx");
                        break;
                }
            }


            // update cart quantities:
            for (int i = 0; i <= Request.Form.Count - 1; i++)
            {
                String fld = Request.Form.Keys[i];
                String fldval = Request.Form[Request.Form.Keys[i]];
                int recID;
                String quantity;
                if (fld.StartsWith("quantity", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (re.IsMatch(fldval))
                    {
                        recID = Localization.ParseUSInt(fld.Substring("Quantity".Length + 1));
                        quantity = fldval;
                        int iquan = Localization.ParseUSInt(quantity);
                        if (iquan < 0)
                        {
                            iquan = 0;
                        }
                        cart.SetItemQuantity(recID, iquan);
                    }
                    else
                    {
                        ErrorMsgLabel.Text += "The item quantity must be a number between 0 and 9999";
                    }
                }
                if (fld.StartsWith("notes", StringComparison.InvariantCultureIgnoreCase))
                {
                    recID = Localization.ParseUSInt(fld.Substring("Notes".Length + 1));
                    cart.SetItemNotes(recID, CommonLogic.CleanLevelOne(fldval));
                }
            }

            // save coupon code, no need to reload cart object
            // will update customer record also:
            if (cte == CartTypeEnum.ShoppingCart)
            {
                cart.SetCoupon(CouponCode.Text, true);

                // check for upsell products
                if (CommonLogic.FormCanBeDangerousContent("Upsell").Length != 0)
                {
                    foreach (String s in CommonLogic.FormCanBeDangerousContent("Upsell").Split(','))
                    {
                        int ProductID = Localization.ParseUSInt(s);
                        if (ProductID != 0)
                        {
                            int VariantID = AppLogic.GetProductsDefaultVariantID(ProductID);
                            if (VariantID != 0)
                            {
                                cart.AddItem(ThisCustomer, ThisCustomer.PrimaryShippingAddressID, ProductID, VariantID, 1, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, CartTypeEnum.ShoppingCart, true, false, 0, System.Decimal.Zero);
                            }
                        }
                    }
                }

                if (ShoppingCart.CheckInventory(ThisCustomer.CustomerID))
                {
                    ErrorMsgLabel.Text += Server.HtmlEncode(AppLogic.GetString("shoppingcart_process.aspx.1", SkinID, ThisCustomer.LocaleSetting));
                    // inventory got adjusted, send them back to the cart page to confirm the new values!
                }

                String sOrderNotes = CommonLogic.CleanLevelOne(OrderNotes.Text);
                String OrderOptions = String.Empty;
                foreach (RepeaterItem ri in OrderOptionsList.Items)
                {
                    DataCheckBox cbk = (DataCheckBox)ri.FindControl("OrderOptions");
                    OrderOptions += CommonLogic.IIF(cbk.Checked, cbk.Data.ToString() + ",", "");
                }
                if (OrderOptions.Length > 0)
                {
                    OrderOptions = OrderOptions.Substring(0, OrderOptions.Length - 1);
                }
                if (OrderOptions.Length > 0 || (cart.OrderOptions.Length > 0 && OrderOptions.Length == 0) || sOrderNotes.Length > 0 || (cart.OrderNotes.Length > 0 && sOrderNotes.Length == 0))
                {
                    DB.ExecuteSQL("update customer set OrderOptions=" + DB.SQuote(OrderOptions) + ", OrderNotes=" + DB.SQuote(sOrderNotes) + ", FinalizationData=NULL where CustomerID=" + ThisCustomer.CustomerID.ToString());
                    cart = new ShoppingCart(SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);
                }
            }


            if (cte == CartTypeEnum.WishCart)
            {
                Response.Redirect("wishlist.aspx");
            }
            if (cte == CartTypeEnum.GiftRegistryCart)
            {
                Response.Redirect("giftregistry.aspx");
            }

            cart.ClearShippingOptions();
            if (DoingFullCheckout)
            {
                bool validated = true;
                if (!cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
                {
                    validated = false;
                }

                if (!cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
                {
                    validated = false;
                }

                if (cart.HasCoupon() && !cart.CouponIsValid)
                {
                    //Response.Redirect("shoppingcart.aspx?resetlinkback=1&discountvalid=false");
                    validated = false;

                }

                // try to use one page quick checkout, if enabled and allowed...
                if ((ForceOnePageCheckout || AppLogic.AppConfigBool("Checkout.UseOnePageCheckout")) && !cart.HasGiftRegistryComponents() && !cart.HasMultipleShippingAddresses() && !cart.ContainsGiftCard())
                {
                    Response.Redirect("checkout1.aspx?checkout=true");
                }

                if (validated)
                {
                    AppLogic.eventHandler("BeginCheckout").CallEvent("&BeginCheckout=true");

                    if (InternationalCheckout)
                    {
                        Response.Redirect("internationalcheckout.aspx");
                    }
                    if ((ThisCustomer.IsRegistered || ThisCustomer.EMail.Length != 0) && (ThisCustomer.Password.Length == 0 || ThisCustomer.PrimaryBillingAddressID == 0 || ThisCustomer.PrimaryShippingAddressID == 0 || !ThisCustomer.HasAtLeastOneAddress()))
                    {
                        Response.Redirect("createaccount.aspx?checkout=true");
                    }

                    if (!ThisCustomer.IsRegistered || ThisCustomer.PrimaryBillingAddressID == 0 || ThisCustomer.PrimaryShippingAddressID == 0 || !ThisCustomer.HasAtLeastOneAddress())
                    {
                        Response.Redirect("checkoutanon.aspx?checkout=true");
                    }
                    else
                    {
                        if (AppLogic.AppConfigBool("SkipShippingOnCheckout") || cart.IsAllSystemComponents() || cart.IsAllDownloadComponents())
                        {
                            if (cart.ContainsGiftCard())
                            {
                                Response.Redirect("checkoutgiftcard.aspx");
                            }
                            else
                            {
                                Response.Redirect("checkoutpayment.aspx");
                            }
                        }

                        //Response.Redirect("selectaddress.aspx?gotostep3=true&checkout=true&addresstype=billing");
                        if ((cart.HasMultipleShippingAddresses() || (cart.HasGiftRegistryComponents() && cart.TotalQuantity() > 1)) && cart.TotalQuantity() <= AppLogic.MultiShipMaxNumItemsAllowed() && cart.CartAllowsShippingMethodSelection)
                        {
                            Response.Redirect("checkoutshippingmult.aspx");
                        }
                        else
                        {
                            Response.Redirect("checkoutshipping.aspx");
                        }
                    }
                }
                InitializePageContent();
            }
        }

        private void ClearErrors()
        {
            CouponError.Text = "";
            ErrorMsgLabel.Text = "";
            InventoryTrimmedError.Text = "";
            RecurringScheduleConflictError.Text = "";
            MinimumQuantitiesUpdatedError.Text = "";
            MeetsMinimumOrderAmountError.Text = "";
            MeetsMinimumOrderQuantityError.Text = "";
            Micropay_EnabledError.Text = "";
        }

        private void ContinueShopping()
        {
            if (AppLogic.AppConfig("ContinueShoppingURL") == "")
            {
                if (ViewState["ReturnURL"].ToString() == "")
                {
                    Response.Redirect("default.aspx");
                }
                else
                {
                    Response.Redirect(ViewState["ReturnURL"].ToString());
                }
            }
            else
            {
                Response.Redirect(AppLogic.AppConfig("ContinueShoppingURL"));
            }
        }
        private bool DeleteButtonExists(string s)
        {
            return s == "bt_Delete";
        }

        protected void btnGoogleCheckout_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ProcessCart(false, false, false);
            if (!ThisCustomer.IsRegistered && !AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout") && !AppLogic.AppConfigBool("GoogleCheckout.AllowAnonCheckout"))
            {
                Response.Redirect("checkoutanon.aspx?checkout=true");
            }
            else
            {
                Response.Redirect(GoogleCheckout.CreateGoogleCheckout(cart));
            }
        }

        protected void btnPayPalExpressCheckout_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ProcessCart(false, false, false);

            if (CommonLogic.CookieCanBeDangerousContent("PayPalExpressToken", false) == "")
            {
                if (!ThisCustomer.IsRegistered && !AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout")
                        && !AppLogic.AppConfigBool("PayPal.Express.AllowAnonCheckout"))
                {
                    Response.Redirect("checkoutanon.aspx?checkout=true");
                }
                if (cart == null)
                {
                    cart = new ShoppingCart(SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);
                }

                string url = String.Empty;
                //AspDotNetStorefrontGateways.PayPal PPP = new AspDotNetStorefrontGateways.PayPal();
                if (ThisCustomer.IsRegistered && ThisCustomer.PrimaryShippingAddressID != 0)
                {
                    Address shippingAddress = new Address();
                    shippingAddress.LoadByCustomer(ThisCustomer.CustomerID, ThisCustomer.PrimaryShippingAddressID, AddressTypes.Shipping);
                    url = Gateway.StartExpressCheckout(cart, shippingAddress);
                }
                else
                {
                    url = Gateway.StartExpressCheckout(cart, null);
                }
                Response.Redirect(url);
            }
            else
            {
                Response.Redirect("checkoutshipping.aspx");
            }
        }
    }
}
