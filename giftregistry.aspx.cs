// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/giftregistry.aspx.cs 8     10/03/06 7:56p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for giftregistry.
    /// </summary>
    public partial class giftregistry : SkinBase
    {

        String GiftRegistryGUID = String.Empty;
        String DisplayName = String.Empty;
        int RegistryOwnerCustomerID = 0;
        Customer RegistryOwnerCustomer = null;
        bool NickNameClash = false;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (CommonLogic.QueryStringCanBeDangerousContent("addtolistguid").Length != 0)
            {
                AppLogic.CheckForScriptTag(CommonLogic.QueryStringCanBeDangerousContent("addtolistguid"));
                try // ignore dups
                {
                    DB.ExecuteSQL("insert CustomerGiftRegistrySearches(CustomerID,GiftRegistryGUID) values(" + ThisCustomer.CustomerID.ToString() + "," + DB.SQuote(CommonLogic.QueryStringCanBeDangerousContent("addtolistguid")) + ")");
                }
                catch { }
            }
            GiftRegistryGUID = CommonLogic.QueryStringCanBeDangerousContent("GUID");
            AppLogic.CheckForScriptTag(GiftRegistryGUID);
            SectionTitle = AppLogic.GetString("giftregistry.aspx.1", SkinID, ThisCustomer.LocaleSetting);
            if (GiftRegistryGUID.Length != 0)
            {
                IDataReader rs = DB.GetRS("select CustomerID from Customer where GiftRegistryGUID=" + DB.SQuote(GiftRegistryGUID) + " and Deleted=0");
                if (rs.Read())
                {
                    RegistryOwnerCustomerID = DB.RSFieldInt(rs, "CustomerID");
                }
                rs.Close();
                if (RegistryOwnerCustomerID == ThisCustomer.CustomerID)
                {
                    Response.Redirect("giftregistry.aspx"); // they are viewing their OWN registry!
                }
                RegistryOwnerCustomer = new Customer(RegistryOwnerCustomerID, true);
                DisplayName = AppLogic.GiftRegistryDisplayName(RegistryOwnerCustomer.CustomerID, false, SkinID, ThisCustomer.LocaleSetting);
                SectionTitle = String.Format(AppLogic.GetString("giftregistry.aspx.16", SkinID, ThisCustomer.LocaleSetting), DisplayName);
            }

            if (GiftRegistryGUID.Length == 0)
            {
                if (CommonLogic.QueryStringUSInt("DeleteID") != 0)
                {
                    DB.ExecuteSQL("delete from ShoppingCart where CustomerID=" + ThisCustomer.CustomerID.ToString() + " and ShoppingCartRecID=" + CommonLogic.QueryStringUSInt("DeleteID").ToString());
                    DB.ExecuteSQL("delete from customcart where CustomerID=" + ThisCustomer.CustomerID.ToString() + " and ShoppingCartRecID=" + CommonLogic.QueryStringUSInt("DeleteID").ToString());
                    DB.ExecuteSQL("delete from kitcart where CustomerID=" + ThisCustomer.CustomerID.ToString() + " and ShoppingCartRecID=" + CommonLogic.QueryStringUSInt("DeleteID").ToString());
                }
            }
            // move is a misnomer, we're really "copying" the gift registry item, we don't remove it from the gift registry until THIS 
            // customer actually purchases it and the payment has cleared and only then if AppConfig:DecrementGiftRegistryOnOrder=true
            int MoveID = CommonLogic.QueryStringUSInt("MoveToCartID");
            if (GiftRegistryGUID.Length != 0 && MoveID != 0)
            {
                ThisCustomer.RequireCustomerRecord();
                String NewGUID = DB.GetNewGUID();
                int RegistryOwnerCustomerID = AppLogic.GiftRegistryOwnerID(GiftRegistryGUID);
                int ExistingShoppingCartRecID = 0;
                if (RegistryOwnerCustomerID != 0)
                {
                    // increment their cart if they already have this item in there for this gift recipient:
                    bool TheyHaveInCartAlready = false;
                    IDataReader rs = DB.GetRS("select * from ShoppingCart where ShoppingCartRecID=" + MoveID.ToString());
                    if (rs.Read())
                    {
                        int ProductID = DB.RSFieldInt(rs, "ProductID");
                        int VariantID = DB.RSFieldInt(rs, "VariantID");
                        String ChosenColor = DB.RSField(rs, "ChosenColor");
                        String ChosenSize = DB.RSField(rs, "ChosenSize");
                        String TextOption = DB.RSField(rs, "TextOption");
                        String sqlx = String.Format("select ShoppingCartRecID from shoppingcart where ProductID={0} and VariantID={1} and ChosenColor like {2} and ChosenSize like {3} and TextOption like {4} and GiftRegistryForCustomerID={5} and CustomerID={6}", ProductID.ToString(), VariantID.ToString(), DB.SQuote("%" + ChosenColor + "%"), DB.SQuote("%" + ChosenSize + "%"), DB.SQuote("%" + TextOption + "%"), RegistryOwnerCustomerID.ToString(), ThisCustomer.CustomerID.ToString());
                        IDataReader rsx = DB.GetRS(sqlx);
                        if (rsx.Read())
                        {
                            ExistingShoppingCartRecID = DB.RSFieldInt(rsx, "ShoppingCartRecID");
                        }
                        rsx.Close();
                        TheyHaveInCartAlready = (ExistingShoppingCartRecID != 0);
                    }
                    rs.Close();
                    if (TheyHaveInCartAlready)
                    {
                        DB.ExecuteSQL("update ShoppingCart set Quantity=Quantity+1 where ShoppingCartRecID=" + ExistingShoppingCartRecID.ToString());
                    }
                    else
                    {
                        int GiftShippingAddressID = Customer.GetCustomerPrimaryShippingAddressID(RegistryOwnerCustomerID);
                        String sql = "insert into shoppingcart(ShoppingCartRecGUID,CustomerID,ProductSKU,ProductPrice,ProductWeight,ProductID,VariantID,Quantity,ChosenColor,ChosenColorSKUModifier,ChosenSize,ChosenSizeSKUModifier,IsTaxable,IsShipSeparately,IsDownload,DownloadLocation,ProductDimensions,CartType,TextOption,NextRecurringShipDate,RecurringIndex,OriginalRecurringOrderNumber,BillingAddressID,ShippingAddressID,DistributorID,SubscriptionInterval,SubscriptionIntervalType,Notes,IsUpsell,GiftRegistryForCustomerID,RecurringInterval,RecurringIntervalType,ExtensionData) ";
                        sql += " select " + DB.SQuote(NewGUID) + "," + ThisCustomer.CustomerID.ToString() + ",ProductSKU,ProductPrice,ProductWeight,ProductID,VariantID,Quantity,ChosenColor,ChosenColorSKUModifier,ChosenSize,ChosenSizeSKUModifier,IsTaxable,IsShipSeparately,IsDownload,DownloadLocation,ProductDimensions," + ((int)CartTypeEnum.ShoppingCart).ToString() + ",TextOption,NextRecurringShipDate,RecurringIndex,OriginalRecurringOrderNumber,BillingAddressID," + GiftShippingAddressID.ToString() + ",DistributorID,SubscriptionInterval,SubscriptionIntervalType,Notes,IsUpsell," + RegistryOwnerCustomerID.ToString() + ",RecurringInterval,RecurringIntervalType,ExtensionData";
                        sql += " from ShoppingCart where ShoppingCartRecID=" + MoveID.ToString();
                        DB.ExecuteSQL(sql);

                        // get new ShoppingCartRecID:
                        rs = DB.GetRS("Select ShoppingCartRecID from ShoppingCart " + DB.GetNoLock() + " where ShoppingCartRecGUID=" + DB.SQuote(NewGUID));
                        rs.Read();
                        int NewShoppingCartRecID = DB.RSFieldInt(rs, "ShoppingCartRecID");
                        rs.Close();

                        String sql2 = "insert into kitcart(CustomerID,ShoppingCartRecID,ProductID,VariantID,KitGroupID,KitItemID,Quantity,CartType,OriginalRecurringOrderNumber,ExtensionData) ";
                        sql2 += " select " + ThisCustomer.CustomerID.ToString() + "," + NewShoppingCartRecID.ToString() + ",ProductID,VariantID,KitGroupID,KitItemID,Quantity," + ((int)CartTypeEnum.ShoppingCart).ToString() + ",OriginalRecurringOrderNumber,ExtensionData";
                        sql2 += " from kitcart where ShoppingCartRecID=" + MoveID.ToString();
                        DB.ExecuteSQL(sql2);

                        String sql3 = "insert into customcart(CustomerID,PackID,ShoppingCartRecID,ProductSKU,ProductWeight,ProductID,VariantID,Quantity,ChosenColor,ChosenColorSKUModifier,ChosenSize,ChosenSizeSKUModifier,CartType,OriginalRecurringOrderNumber,ExtensionData) ";
                        sql3 += " select " + ThisCustomer.CustomerID.ToString() + ",PackID," + NewShoppingCartRecID.ToString() + ",ProductSKU,ProductWeight,ProductID,VariantID,Quantity,ChosenColor,ChosenColorSKUModifier,ChosenSize,ChosenSizeSKUModifier," + ((int)CartTypeEnum.ShoppingCart).ToString() + ",OriginalRecurringOrderNumber,ExtensionData";
                        sql3 += " from customcart where ShoppingCartRecID=" + MoveID.ToString();
                        DB.ExecuteSQL(sql3);
                    }
                    Response.Redirect("shoppingcart.aspx");
                }
            }


            if (!IsPostBack)
            {
                string ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnUrl");
                AppLogic.CheckForScriptTag(ReturnURL);
                ViewState["returnurl"] = ReturnURL;
                InitializePageContent();
            }

            string[] formkeys = Request.Form.AllKeys;
            foreach (String s in formkeys)
            {
                if (s == "bt_Delete")
                {
                    UpdateGiftRegistry();
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
            this.btnUpdateGiftSettings.Click += new EventHandler(btnUpdateGiftSettings_Click);
            this.rptrGiftRegistrySearches.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler(rptrGiftRegistrySearches_ItemCommand);
            this.btnUpdateRegistryItems.Click += new EventHandler(btnUpdateRegistryItems_Click);
        }

        void btnUpdateRegistryItems_Click(object sender, EventArgs e)
        {
            UpdateGiftRegistry();
        }

        #endregion

        void rptrGiftRegistrySearches_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                DB.ExecuteSQL("delete CustomerGiftRegistrySearches where CustomerGiftRegistrySearchesID = " + e.CommandArgument);
            }
            InitializePageContent();
        }
        void btnUpdateGiftSettings_Click(object sender, EventArgs e)
        {
            string sql = "update Customer ";
            sql += "set GiftRegistryIsAnonymous=" + CommonLogic.IIF(GiftRegistryIsAnonymousYes.Checked, "1", "0");
            sql += ", GiftRegistryAllowSearchByOthers=" + CommonLogic.IIF(GiftRegistryAllowSearchByOthersYes.Checked, "1", "0");
            sql += ", GiftRegistryNickName='" + txtGiftRegistryNickName.Text.Trim() + "'";
            sql += ", GiftRegistryHideShippingAddresses=" + CommonLogic.IIF(GiftRegistryHideShippingAddressesYes.Checked, "1", "0");
            sql += " where CustomerID=" + ThisCustomer.CustomerID.ToString();
            DB.ExecuteSQL(sql);
            InitializePageContent();
        }


        private void InitializePageContent()
        {
            if (ThisCustomer.GiftRegistryNickName.Trim().ToLowerInvariant().Length == 0)
            {
                NickNameClash = false;
            }
            else
            {
                NickNameClash = (DB.GetSqlN("select count(*) as N from Customer " + DB.GetNoLock() + " where lower(GiftRegistryNickName)=" + DB.SQuote(ThisCustomer.GiftRegistryNickName.Trim().ToLowerInvariant())) > 1);
            }
            pnlGiftRegistrySearches.Visible = true;
            pnlGiftRegistrySettings.Visible = false;
            pnlMyGiftRegistryItems.Visible = false;
            pnlNoCustomer.Visible = false;
            pnlNoCustomer.Visible = false;
            pnlRegistryLink.Visible = false;
            pnlTheirGiftRegistry.Visible = false;

            pnlGiftRegistrySearches.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
            tblGiftRegistrySearchesBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
            giftregistry_aspx_10.Text = String.Format(AppLogic.GetString("giftregistry.aspx.10", SkinID, ThisCustomer.LocaleSetting), "giftregistrysearch.aspx") + "<br /><br />";
            IDataReader dr = DB.GetRS("Select CustomerGiftRegistrySearches.*, Customer.FirstName + ' ' + Customer.LastName CustomerName, Customer.CustomerID as RegistryCustomerID, '" + AppLogic.GetString("giftregistry.aspx.23", SkinID, ThisCustomer.LocaleSetting) + "' Label from CustomerGiftRegistrySearches " + DB.GetNoLock() + " inner join Customer " + DB.GetNoLock() + " on CustomerGiftRegistrySearches.GiftRegistryGUID=Customer.GiftRegistryGUID where CustomerGiftRegistrySearches.CustomerID=" + ThisCustomer.CustomerID.ToString());
            rptrGiftRegistrySearches.DataSource = dr;
            rptrGiftRegistrySearches.DataBind();
            dr.Close();
            giftregistry6_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/giftregistry6.gif");
            if (rptrGiftRegistrySearches.Items.Count == 0)
            {
                pnlGiftRegistrySearches.Visible = false;
            }

            if (GiftRegistryGUID.Length == 0)
            {
                if (!ThisCustomer.HasCustomerRecord)
                {
                    tblNoCustomer.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
                    tblNoCustomerBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
                    pnlNoCustomer.Visible = true;
                    Topic t1 = new Topic("EmptyGiftRegistryText", ThisCustomer.LocaleSetting, SkinID, null);
                    giftregistry_aspx_2.Text = t1.Contents;
                    giftregistry_gif1.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/giftregistry.gif");
                    btnContinueShopping1.PostBackUrl = "default.aspx"; //.OnClientClick = "self.location='javascript:history.back(-1)'";

                }
                else
                {
                    ShoppingCart cart = new ShoppingCart(SkinID, ThisCustomer, CartTypeEnum.GiftRegistryCart, 0, false);
                    if (!ThisCustomer.IsRegistered)
                    {
                        giftregistry_aspx_18.Text = "<p>" + String.Format(AppLogic.GetString("giftregistry.aspx.18", SkinID, ThisCustomer.LocaleSetting), "createaccount.aspx?returnURL=giftregistry.aspx") + "</p>";
                        giftregistry_aspx_18.Visible = true;
                    }
                    else
                    {
                        pnlGiftRegistrySettings.Visible = true;
                        tblGiftRegSettings.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
                        tblGiftRegSettingsBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
                        if (ThisCustomer.PrimaryShippingAddressID == 0)
                        {
                            giftregistry_aspx_17.Text = String.Format(AppLogic.GetString("giftregistry.aspx.17", SkinID, ThisCustomer.LocaleSetting), "selectaddress.aspx?AddressType=shipping&returnURL=giftregistry.aspx") + "</p>";
                            giftregistry_aspx_17.Visible = true;
                        }
                        giftregistry2_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/giftregistry2.gif");
                        if (!IsPostBack)
                        {
                            GiftRegistryIsAnonymousYes.Checked = ThisCustomer.GiftRegistryIsAnonymous;
                            GiftRegistryIsAnonymousNo.Checked = !ThisCustomer.GiftRegistryIsAnonymous;
                        }
                        giftregistry_aspx_5.Text = AppLogic.GetString("giftregistry.aspx.5", SkinID, ThisCustomer.LocaleSetting);
                        litYes1.Text = AppLogic.GetString("account.aspx.25", SkinID, ThisCustomer.LocaleSetting);
                        LitNo1.Text = AppLogic.GetString("account.aspx.26", SkinID, ThisCustomer.LocaleSetting);
                        giftregistry_aspx_6.Text = AppLogic.GetString("giftregistry.aspx.6", SkinID, ThisCustomer.LocaleSetting);
                        giftregistry_aspx_9.Text = AppLogic.GetString("giftregistry.aspx.9", SkinID, ThisCustomer.LocaleSetting);

                        if (!IsPostBack)
                        {
                            txtGiftRegistryNickName.Text = Server.HtmlEncode(ThisCustomer.GiftRegistryNickName);
                        }
                        if (NickNameClash)
                        {
                            giftregistry_aspx_19.Text = AppLogic.GetString("giftregistry.aspx.19", SkinID, ThisCustomer.LocaleSetting);
                            giftregistry_aspx_19.Visible = true;
                        }
                        else
                        {
                            giftregistry_aspx_19.Visible = false;
                        }
                        giftregistry_aspx_7.Text = AppLogic.GetString("giftregistry.aspx.7", SkinID, ThisCustomer.LocaleSetting);
                        litYes2.Text = AppLogic.GetString("account.aspx.25", SkinID, ThisCustomer.LocaleSetting);
                        LitNo2.Text = AppLogic.GetString("account.aspx.26", SkinID, ThisCustomer.LocaleSetting);
                        if (!IsPostBack)
                        {
                            GiftRegistryHideShippingAddressesYes.Checked = ThisCustomer.GiftRegistryHideShippingAddresses;
                            GiftRegistryHideShippingAddressesNo.Checked = !ThisCustomer.GiftRegistryHideShippingAddresses;
                        }

                        giftregistry_aspx_12.Text = AppLogic.GetString("giftregistry.aspx.12", SkinID, ThisCustomer.LocaleSetting);
                        litYes3.Text = AppLogic.GetString("account.aspx.25", SkinID, ThisCustomer.LocaleSetting);
                        LitNo3.Text = AppLogic.GetString("account.aspx.26", SkinID, ThisCustomer.LocaleSetting);
                        if (!IsPostBack)
                        {
                            GiftRegistryAllowSearchByOthersYes.Checked = ThisCustomer.GiftRegistryAllowSearchByOthers;
                            GiftRegistryAllowSearchByOthersNo.Checked = !ThisCustomer.GiftRegistryAllowSearchByOthers;
                        }

                        btnUpdateGiftSettings.Text = AppLogic.GetString("giftregistry.aspx.8", SkinID, ThisCustomer.LocaleSetting);
                    }

                    if (ThisCustomer.IsRegistered && !cart.IsEmpty())
                    {
                        pnlRegistryLink.Visible = true;
                        tblRegistryLink.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
                        tblRegistryLinkBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
                        giftregistry7_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/giftregistry7.gif");
                        giftregistry_aspx_11.Text = String.Format(AppLogic.GetString("giftregistry.aspx.11", SkinID, ThisCustomer.LocaleSetting), "giftregistrysearch.aspx");
                        txtMyyRegistryLink.Text = String.Format("{0}giftregistry.aspx?guid={1}", AppLogic.GetStoreHTTPLocation(false), ThisCustomer.GiftRegistryGUID);
                    }
                    pnlMyGiftRegistryItems.Visible = true;
                    tblGiftRegistryItems.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
                    giftregistry_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/giftregistry.gif");
                    tblGiftRegistryItemsBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
                    litCartItems.Text = cart.DisplayItems(false, ThisCustomer, true);

                    if (!cart.IsEmpty())
                    {
                        pnlUpdateRegistryItems.Visible = true;
                        btnUpdateRegistryItems.Text = AppLogic.GetString("shoppingcart.cs.109", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                    }

                    String CartBottomControlLinesXmlPackage = AppLogic.AppConfig("XmlPackage.WishListPageBottomControlLines");
                    if (CartBottomControlLinesXmlPackage.Length != 0)
                    {
                        litBottomControl.Text = AppLogic.RunXmlPackage(CartBottomControlLinesXmlPackage, base.GetParser, ThisCustomer, SkinID, String.Empty, String.Empty, true, true);
                    }

                }


            }
            else
            {
                pnlTheirGiftRegistry.Visible = true;
                if (RegistryOwnerCustomerID != 0)
                {
                    tblTheirGiftRegistryItems.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
                    giftregistry5_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/giftregistry5.gif");
                    tblTheirGiftRegistryItemsBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
                    ShoppingCart ViewingRegistryCart = new ShoppingCart(SkinID, RegistryOwnerCustomer, CartTypeEnum.GiftRegistryCart, 0, false);
                    litTheirRegistryItems.Text = ViewingRegistryCart.DisplayItems(true, ThisCustomer, true);
                }
                else
                {
                    giftregistry_aspx_13.Text = AppLogic.GetString("giftregistry.aspx.13", SkinID, ThisCustomer.LocaleSetting);
                }

            }

            dr.Close();
            //AppLogic.GetButtonDisable(btnDelGiftRegSrch);
        }

        private void UpdateGiftRegistry()
        {
            for (int i = 0; i <= Request.Form.Count - 1; i++)
            {
                String fld = Request.Form.Keys[i];
                String fldval = Request.Form[Request.Form.Keys[i]];
                int recID;
                String quantity;
                if (fld.StartsWith("quantity", StringComparison.InvariantCultureIgnoreCase))
                {
                    recID = Localization.ParseUSInt(fld.Substring("Quantity".Length + 1));
                    quantity = fldval;
                    int iquan = 0;
                    try
                    {
                        iquan = Localization.ParseUSInt(quantity);
                    }
                    catch
                    {
                        iquan = 0;
                    }
                    if (iquan < 0)
                    {
                        iquan = 0;
                    }
                    if (iquan == 0)
                    {
                        DB.ExecuteSQL("delete from ShoppingCart where CartType=" + ((int)CartTypeEnum.GiftRegistryCart).ToString() + " and ShoppingCartRecID=" + recID.ToString() + " and CustomerID=" + ThisCustomer.CustomerID.ToString());
                    }
                    else
                    {
                        DB.ExecuteSQL("update ShoppingCart set Quantity=" + iquan.ToString() + " where CartType=" + ((int)CartTypeEnum.GiftRegistryCart).ToString() + " and ShoppingCartRecID=" + recID.ToString() + " and CustomerID=" + ThisCustomer.CustomerID.ToString());
                    }
                }
            }
            InitializePageContent();
        }

    }
}
