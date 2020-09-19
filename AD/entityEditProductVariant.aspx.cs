// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2006.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/entityEditProductVariant.aspx.cs 11    10/04/06 6:23a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AspDotNetStorefrontCommon;

public partial class entityEditProductVariant : Page
{
    private int eID;
    private string eName;
    private EntityHelper entity;
    private EntitySpecs eSpecs;
    private int pID;
    private bool ProductTracksInventoryBySizeAndColor;
    private Shipping.ShippingCalculationEnum ShipCalcID = Shipping.ShippingCalculationEnum.Unknown;
    private int skinID = 1;
    private Customer ThisCustomer;
    private bool UseSpecialRecurringIntervals = false;
    private int vID;

    protected void Page_Load( object sender , EventArgs e )
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader( "pragma" , "no-cache" );

        if ( !IsPostBack )
        {
            ViewState.Add( "VariantEditID" , 0 );
        }

        ThisCustomer = ( ( AspDotNetStorefrontPrincipal )Context.User ).ThisCustomer;

        UseSpecialRecurringIntervals = AppLogic.UseSpecialRecurringIntervals( );
        ShipCalcID = Shipping.GetActiveShippingCalculationID( );
        vID = CommonLogic.QueryStringNativeInt( "VariantID" );
        pID = CommonLogic.QueryStringNativeInt( "ProductID" );
        eID = CommonLogic.QueryStringNativeInt( "EntityID" );
        eName = CommonLogic.QueryStringCanBeDangerousContent( "EntityName" );
        eSpecs = EntityDefinitions.LookupSpecs( eName );

        if ( Localization.ParseNativeInt( ViewState[ "VariantEditID" ].ToString( ) ) > 0 )
        {
            vID = Localization.ParseNativeInt( ViewState[ "VariantEditID" ].ToString( ) );
        }

        switch ( eName.ToUpperInvariant( ) )
        {
            case "SECTION" :
                entity = new EntityHelper( EntityDefinitions.readonly_SectionEntitySpecs );
                break;
            case "MANUFACTURER" :
                entity = new EntityHelper( EntityDefinitions.readonly_ManufacturerEntitySpecs );
                break;
            case "DISTRIBUTOR" :
                entity = new EntityHelper( EntityDefinitions.readonly_DistributorEntitySpecs );
                break;
            case "LIBRARY" :
                entity = new EntityHelper( EntityDefinitions.readonly_LibraryEntitySpecs );
                break;
            default :
                entity = new EntityHelper( EntityDefinitions.readonly_CategoryEntitySpecs );
                break;
        }

        lblerr.Text = string.Empty;
        if ( !IsPostBack )
        {
            if ( ThisCustomer.ThisCustomerSession.Session( "entityUserLocale" ).Length == 0 )
            {
                ThisCustomer.ThisCustomerSession.SetVal( "entityUserLocale" , Localization.GetWebConfigLocale( ) );
            }

            if ( !UseSpecialRecurringIntervals )
            {
                rblRecurringIntervalType.Items.Clear( );
                rblRecurringIntervalType.Items.Add(
                        new ListItem( DateIntervalTypeEnum.Day.ToString( ) ,
                                      ( ( int )DateIntervalTypeEnum.Day ).ToString( ) ) );
                rblRecurringIntervalType.Items.Add(
                        new ListItem( DateIntervalTypeEnum.Week.ToString( ) ,
                                      ( ( int )DateIntervalTypeEnum.Week ).ToString( ) ) );
                rblRecurringIntervalType.Items.Add(
                        new ListItem( DateIntervalTypeEnum.Month.ToString( ) ,
                                      ( ( int )DateIntervalTypeEnum.Month ).ToString( ) ) );
                rblRecurringIntervalType.Items.Add(
                        new ListItem( DateIntervalTypeEnum.Year.ToString( ) ,
                                      ( ( int )DateIntervalTypeEnum.Year ).ToString( ) ) );
            }
            else
            {
                txtRecurringInterval.Text = "1";
                RecurringIntervalDiv.Visible = false;
                rblRecurringIntervalType.Items.Clear( );
                rblRecurringIntervalType.Items.Add(
                        new ListItem( DateIntervalTypeEnum.Weekly.ToString( ) ,
                                      ( ( int )DateIntervalTypeEnum.Weekly ).ToString( ) ) );
                rblRecurringIntervalType.Items.Add(
                        new ListItem( DateIntervalTypeEnum.BiWeekly.ToString( ) ,
                                      ( ( int )DateIntervalTypeEnum.BiWeekly ).ToString( ) ) );
                //rblRecurringIntervalType.Items.Add(new ListItem(DateIntervalTypeEnum.SemiMonthly.ToString(), ((int)DateIntervalTypeEnum.SemiMonthly).ToString()));
                rblRecurringIntervalType.Items.Add(
                        new ListItem( DateIntervalTypeEnum.Monthly.ToString( ) ,
                                      ( ( int )DateIntervalTypeEnum.Monthly ).ToString( ) ) );
                rblRecurringIntervalType.Items.Add(
                        new ListItem( DateIntervalTypeEnum.EveryFourWeeks.ToString( ) ,
                                      ( ( int )DateIntervalTypeEnum.EveryFourWeeks ).ToString( ) ) );
                rblRecurringIntervalType.Items.Add(
                        new ListItem( DateIntervalTypeEnum.Quarterly.ToString( ) ,
                                      ( ( int )DateIntervalTypeEnum.Quarterly ).ToString( ) ) );
                rblRecurringIntervalType.Items.Add(
                        new ListItem( DateIntervalTypeEnum.SemiYearly.ToString( ) ,
                                      ( ( int )DateIntervalTypeEnum.SemiYearly ).ToString( ) ) );
                rblRecurringIntervalType.Items.Add(
                        new ListItem( DateIntervalTypeEnum.Yearly.ToString( ) ,
                                      ( ( int )DateIntervalTypeEnum.Yearly ).ToString( ) ) );
            }

            rblSubscriptionIntervalType.Items.Clear( );
            rblSubscriptionIntervalType.Items.Add(
                    new ListItem( DateIntervalTypeEnum.Day.ToString( ) , ( ( int )DateIntervalTypeEnum.Day ).ToString( ) ) );
            rblSubscriptionIntervalType.Items.Add(
                    new ListItem( DateIntervalTypeEnum.Week.ToString( ) ,
                                  ( ( int )DateIntervalTypeEnum.Week ).ToString( ) ) );
            rblSubscriptionIntervalType.Items.Add(
                    new ListItem( DateIntervalTypeEnum.Month.ToString( ) ,
                                  ( ( int )DateIntervalTypeEnum.Month ).ToString( ) ) );
            rblSubscriptionIntervalType.Items.Add(
                    new ListItem( DateIntervalTypeEnum.Year.ToString( ) ,
                                  ( ( int )DateIntervalTypeEnum.Year ).ToString( ) ) );

            ddLocale.Items.Clear( );
#if PRO
            //not supported in PRO version
#else
            DataSet ds = Localization.GetLocales( );
            foreach ( DataRow dr in ds.Tables[ 0 ].Rows )
            {
                ddLocale.Items.Add( new ListItem( DB.RowField( dr , "Name" ) , DB.RowField( dr , "Name" ) ) );
            }
            ds.Dispose( );
#endif
            if ( ddLocale.Items.Count < 2 )
            {
                tblLocale.Visible = false;
            }
            LoadScript( );
            LoadContent( );
        }
    }

    protected void resetError( string error , bool isError )
    {
        string str = "<font class=\"noticeMsg\">NOTICE:</font>&nbsp;&nbsp;&nbsp;";
        if ( isError )
        {
            str = "<font class=\"errorMsg\">ERROR:</font>&nbsp;&nbsp;&nbsp;";
        }

        if ( error.Length > 0 )
        {
            str += error + "";
        }
        else
        {
            str = "";
        }

        ( ( Literal )Form.FindControl( "ltError" ) ).Text = str;
    }

    protected void LoadScript( )
    {
        ltScript.Text = "";

        ltStyles.Text = "";
    }

    protected void LoadContent( )
    {
        //pull locale from user session
        string entityLocale = ThisCustomer.ThisCustomerSession.Session( "entityUserLocale" );
        if ( entityLocale.Length > 0 )
        {
            try
            {
                ddLocale.SelectedValue = entityLocale;
                        // user's locale may not exist any more, so don't let the assignment crash
            }
            catch {}
        }

        string locale = Localization.CheckLocaleSettingForProperCase( ddLocale.SelectedValue );

        bool Editing = false;

        IDataReader rs =
                DB.GetRS( "select PV.*,P.ColorOptionPrompt,P.SizeOptionPrompt from Product P " + DB.GetNoLock( ) +
                          " , productvariant PV  " + DB.GetNoLock( ) + " where PV.ProductID=P.ProductID and VariantID=" +
                          vID.ToString( ) );
        if ( rs.Read( ) )
        {
            Editing = true;
            if ( DB.RSFieldBool( rs , "Deleted" ) )
            {
                btnDeleteVariant.Text = "UnDelete this Variant";
            }
            else
            {
                btnDeleteVariant.Text = "Delete this Variant";
            }
        }

        ViewState.Add( "VariantEdit" , Editing );

        bool IsAKit = AppLogic.IsAKit( pID );
        bool IsAPack = AppLogic.IsAPack( pID );

        String ColorOptionPrompt = DB.RSField( rs , "ColorOptionPrompt" );
        String SizeOptionPrompt = DB.RSField( rs , "SizeOptionPrompt" );
        if ( ColorOptionPrompt.Length == 0 )
        {
            ColorOptionPrompt =
                    AppLogic.GetString( "AppConfig.ColorOptionPrompt" , skinID , ThisCustomer.LocaleSetting );
        }
        if ( SizeOptionPrompt.Length == 0 )
        {
            SizeOptionPrompt = AppLogic.GetString( "AppConfig.SizeOptionPrompt" , skinID , ThisCustomer.LocaleSetting );
        }

        txtFroogle.Columns = Localization.ParseNativeInt( AppLogic.AppConfig( "Admin_TextareaWidth" ) );
        txtFroogle.Rows = 10;

        //SHIPPING
        trShippingCost.Visible = false;
        trShipSeparately.Visible = false;
        if ( ShipCalcID == Shipping.ShippingCalculationEnum.UseIndividualItemShippingCosts )
        {
            trShippingCost.Visible = true;
            int NF = 0;
            string temp = "";
            IDataReader rs3 =
                    DB.GetRS( "select * from ShippingMethod " + DB.GetNoLock( ) +
                              " where IsRTShipping=0 order by DisplayOrder" );
            while ( rs3.Read( ) )
            {
                temp += ( DB.RSFieldByLocale( rs3 , "Name" , ThisCustomer.LocaleSetting ) + ": " );
                temp += ( "<input class=\"default\" maxLength=\"10\" size=\"10\" name=\"ShippingCost_" +
                          DB.RSFieldInt( rs3 , "ShippingMethodID" ) + "\" value=\"" +
                          CommonLogic.IIF( Editing ,
                                           Localization.CurrencyStringForDBWithoutExchangeRate(
                                                   Shipping.GetVariantShippingCost( vID ,
                                                                                    DB.RSFieldInt( rs3 ,
                                                                                                   "ShippingMethodID" ) ) ) ,
                                           "" ) + "\">  (in format x.xx)\n" );
                temp += ( "<input type=\"hidden\" name=\"ShippingCost_" + DB.RSFieldInt( rs3 , "ShippingMethodID" ) +
                          "_vldt\" value=\"[number][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n" );
                NF++;
            }
            rs3.Close( );
            rs3.Dispose( );
            if ( NF == 0 )
            {
                temp += ( "No Shipping Methods Are Found" );
            }
            ltShippingCost.Text = temp;
            rblShipSeparately.SelectedIndex = 0;
        }
        else
        {
            trShipSeparately.Visible = true;
            rblShipSeparately.SelectedIndex = ( DB.RSFieldBool( rs , "IsShipSeparately" ) ? 1 : 0 );
        }

        //INVENTORY
        ProductTracksInventoryBySizeAndColor = AppLogic.ProductTracksInventoryBySizeAndColor( pID );

        //////////////////////////////////////////////////////////////////////////////////////
        if ( Editing )
        {
            pnlDelete.Visible = true;
            ltStatus.Text = "Editing Variant";
            btnSubmit.Text = "Update";
            ltEntity.Text = entity.GetEntityBreadcrumb6( eID , ThisCustomer.LocaleSetting ) +
                            " : <a href=\"entityEditProducts.aspx?iden=" + pID + "&entityName=" + eName +
                            "&EntityFilterID=" + eID + "\">" +
                            AppLogic.GetProductName( pID , ThisCustomer.LocaleSetting ) + " (" + pID +
                            ")</a> : <a href=\"entityProductVariantsOverview.aspx?ProductID=" + pID + "&entityname=" +
                            eName + "&EntityID=" + eID + "\">Variant Management</a> : " +
                            XmlCommon.GetLocaleEntry( DB.RSField( rs , "Name" ) , ddLocale.SelectedValue , false ) +
                            " (" + vID + ")";

            txtName.Text = XmlCommon.GetLocaleEntry( DB.RSField( rs , "Name" ) , ddLocale.SelectedValue , false );

            txtSKU.Text = Server.HtmlEncode( DB.RSField( rs , "SKUSuffix" ) );
            txtManufacturePartNumber.Text = Server.HtmlEncode( DB.RSField( rs , "ManufacturerPartNumber" ) );

            if ( !DB.RSFieldBool( rs , "Published" ) )
            {
                rblPublished.BackColor = Color.LightYellow;
            }
            rblPublished.SelectedIndex = ( DB.RSFieldBool( rs , "Published" ) ? 1 : 0 );

            rblRecurring.SelectedIndex = ( DB.RSFieldBool( rs , "IsRecurring" ) ? 1 : 0 );

            if ( !UseSpecialRecurringIntervals )
            {
                txtRecurringInterval.Text = DB.RSFieldInt( rs , "RecurringInterval" ).ToString( );
            }
            else
            {
                txtRecurringInterval.Text = "1";
                RecurringIntervalDiv.Visible = false;
            }
            try
            {
                rblRecurringIntervalType.SelectedValue = DB.RSFieldInt( rs , "RecurringIntervalType" ).ToString( );
            }
            catch {}

            //DESCRIPTION
            /*ltDescription.Text = ("<div id=\"idDescription\" style=\"height: 1%;\">");
            ltDescription.Text += ("<textarea rows=\"" + AppLogic.AppConfigUSInt("Admin_TextareaHeight") + "\" cols=\"" + AppLogic.AppConfigUSInt("Admin_TextareaWidth") + "\" id=\"Description\" name=\"Description\">" + XmlCommon.GetLocaleEntry(HttpContext.Current.Server.HtmlEncode(DB.RSField(rs, "Description")), ddLocale.SelectedValue, false) + "</textarea>\n");
            ltDescription.Text += (AppLogic.GenerateInnovaEditor("Description"));
            ltDescription.Text += ("</div>");*/
            radDescription.Html = DB.RSFieldByLocale(rs, "Description", ddLocale.SelectedValue);
            //FROOGLE
            txtFroogle.Text =
                    XmlCommon.GetLocaleEntry( DB.RSField( rs , "FroogleDescription" ) , ddLocale.SelectedValue , false );

            txtRestrictedQuantities.Text = DB.RSField( rs , "RestrictedQuantities" );
            txtMinimumQuantity.Text =
                    CommonLogic.IIF( DB.RSFieldInt( rs , "MinimumQuantity" ) != 0 ,
                                     DB.RSFieldInt( rs , "MinimumQuantity" ).ToString( ) , "" );

            int NumCustomerLevels =
                    DB.GetSqlN( "select count(*) as N from dbo.CustomerLevel cl " + DB.GetNoLock( ) +
                                " join dbo.ProductCustomerLevel pcl " + DB.GetNoLock( ) +
                                " on cl.CustomerLevelID = pcl.CustomerLevelID where pcl.ProductID = " + pID.ToString( ) +
                                " and cl.deleted = 0" );
            //PRICE
            txtPrice.Text = Localization.DecimalStringForDB( DB.RSFieldDecimal( rs , "Price" ) );
            if ( NumCustomerLevels > 0 )
            {
                ltExtendedPricing.Text =
                        "<a href=\"javascript:;\" onclick=\"window.open('entityEditExtendedPrices.aspx?ProductID=" +
                        pID.ToString( ) + "&VariantID=" + vID.ToString( ) +
                        "','Pricing','height=400, width=500, scrollbars=yes, resizable=yes, toolbar=no, status=yes, location=no, directories=no, menubar=no, alwaysRaised=yes');\">Define Extended Prices</a> (Defined By Customer Level)";
            }
            txtSalePrice.Text =
                    CommonLogic.IIF( DB.RSFieldDecimal( rs , "SalePrice" ) != Decimal.Zero ,
                                     Localization.DecimalStringForDB( DB.RSFieldDecimal( rs , "SalePrice" ) ) , "" );

            trCustomerEntersPrice1.Visible = false;
            //trCustomerEntersPrice2.Visible = false;
            if ( IsAKit || IsAPack )
            {
                rblCustomerEntersPrice.SelectedIndex = 0;
                txtCustomerEntersPricePrompt.Text = "";
            }
            else
            {
                trCustomerEntersPrice1.Visible = true;
                //trCustomerEntersPrice2.Visible = true;
                rblCustomerEntersPrice.SelectedIndex = ( DB.RSFieldBool( rs , "CustomerEntersPrice" ) ? 1 : 0 );
                txtCustomerEntersPricePrompt.Text =
                        XmlCommon.GetLocaleEntry( DB.RSField( rs , "CustomerEntersPricePrompt" ) ,
                                                  ddLocale.SelectedValue , false );
            }
            txtMSRP.Text =
                    CommonLogic.IIF( DB.RSFieldDecimal( rs , "MSRP" ) != Decimal.Zero ,
                                     Localization.CurrencyStringForDBWithoutExchangeRate(
                                             DB.RSFieldDecimal( rs , "MSRP" ) ) , "" );
            txtActualCost.Text =
                    CommonLogic.IIF( DB.RSFieldDecimal( rs , "Cost" ) != Decimal.Zero ,
                                     Localization.CurrencyStringForDBWithoutExchangeRate(
                                             DB.RSFieldDecimal( rs , "Cost" ) ) , "" );

            rblTaxable.SelectedIndex = ( DB.RSFieldBool( rs , "IsTaxable" ) ? 1 : 0 );

            rblFreeShipping.SelectedIndex = ( DB.RSFieldBool( rs , "FreeShipping" ) ? 1 : 0 );
            rblDownload.SelectedIndex = ( DB.RSFieldBool( rs , "IsDownload" ) ? 1 : 0 );
            txtDownloadLocation.Text = Server.HtmlEncode( DB.RSField( rs , "DownloadLocation" ) );
            txtWeight.Text = Localization.DecimalStringForDB( DB.RSFieldDecimal( rs , "Weight" ) );
            txtDimensions.Text = Server.HtmlEncode( DB.RSField( rs , "Dimensions" ) );

            //INVENTORY
            trCurrentInventory.Visible = false;
            trManageInventory.Visible = false;
            if ( ProductTracksInventoryBySizeAndColor )
            {
                trManageInventory.Visible = true;
                ltManageInventory.Text = ( "<a href=\"entityEditInventory.aspx?productid=" + pID.ToString( ) +
                                           "&variantid=" + vID.ToString( ) + "\">Click Here</a>\n" );
            }
            else
            {
                trCurrentInventory.Visible = true;
                txtCurrentInventory.Text = DB.RSFieldInt( rs , "Inventory" ).ToString( );
            }

            txtSubscriptionInterval.Text = DB.RSFieldInt( rs , "SubscriptionInterval" ).ToString( );
            if ( DB.RSFieldInt( rs , "SubscriptionIntervalType" ) ==
                 ( ( int )DateIntervalTypeEnum.Day ) )
            {
                rblSubscriptionIntervalType.SelectedIndex = 0;
            }
            else if ( DB.RSFieldInt( rs , "SubscriptionIntervalType" ) ==
                      ( ( int )DateIntervalTypeEnum.Week ) )
            {
                rblSubscriptionIntervalType.SelectedIndex = 1;
            }
            else if ( DB.RSFieldInt( rs , "SubscriptionIntervalType" ) ==
                      ( ( int )DateIntervalTypeEnum.Month ) )
            {
                rblSubscriptionIntervalType.SelectedIndex = 2;
            }
            else
            {
                rblSubscriptionIntervalType.SelectedIndex = 3;
            }

            //BG COLOR
            txtPageBG.Text = CommonLogic.IIF( Editing , DB.RSField( rs , "PageBGColor" ) , "" );
            txtContentsBG.Text = CommonLogic.IIF( Editing , DB.RSField( rs , "ContentsBGColor" ) , "" );
            txtSkinColor.Text = CommonLogic.IIF( Editing , DB.RSField( rs , "GraphicsColor" ) , "" );

            // BEGIN IMAGES 
            txtImageOverride.Text = CommonLogic.IIF( Editing , DB.RSField( rs , "ImageFilenameOverride" ) , "" );

            String Image1URL = AppLogic.LookupImage( "Variant" , vID , "icon" , skinID , ThisCustomer.LocaleSetting );
            if ( Image1URL.Length == 0 )
            {
                Image1URL = AppLogic.NoPictureImageURL( false , skinID , ThisCustomer.LocaleSetting );
            }
            if ( Image1URL.Length != 0 )
            {
                ltIcon.Text = "";
                if ( Image1URL.IndexOf( "nopicture" ) ==
                     -1 )
                {
                    ltIcon.Text = ( "<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image1URL +
                                    "','Pic1');\">Click here</a> to delete the current image <br/>\n" );
                }
                ltIcon.Text +=
                        "<img align=\"absmiddle\" style=\"margin-top: 3px; margin-bottom: 5px;\" id=\"Pic1\" name=\"Pic1\" border=\"0\" src=\"" +
                        Image1URL + "?" + CommonLogic.GetRandomNumber( 1 , 1000000 ).ToString( ) + "\" />\n";
            }
            String Image2URL = AppLogic.LookupImage( "Variant" , vID , "medium" , skinID , ThisCustomer.LocaleSetting );
            if ( Image2URL.Length == 0 )
            {
                Image2URL = AppLogic.NoPictureImageURL( false , skinID , ThisCustomer.LocaleSetting );
            }
            if ( Image2URL.Length != 0 )
            {
                ltMedium.Text = "";
                if ( Image2URL.IndexOf( "nopicture" ) ==
                     -1 )
                {
                    ltMedium.Text = ( "<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image2URL +
                                      "','Pic2');\">Click here</a> to delete the current image <br/>\n" );
                }
                ltMedium.Text +=
                        "<img align=\"absmiddle\" style=\"margin-top: 3px; margin-bottom: 5px;\" id=\"Pic2\" name=\"Pic2\" border=\"0\" src=\"" +
                        Image2URL + "?" + CommonLogic.GetRandomNumber( 1 , 1000000 ).ToString( ) + "\" />\n";
            }
            String Image3URL = AppLogic.LookupImage( "Variant" , vID , "large" , skinID , ThisCustomer.LocaleSetting );
            if ( Image3URL.Length == 0 )
            {
                Image3URL = AppLogic.NoPictureImageURL( false , skinID , ThisCustomer.LocaleSetting );
            }
            if ( Image3URL.Length != 0 )
            {
                ltLarge.Text = "";
                if ( Image3URL.IndexOf( "nopicture" ) ==
                     -1 )
                {
                    ltLarge.Text = ( "<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image3URL +
                                     "','Pic3');\">Click here</a> to delete the current image <br/>\n" );
                }
                ltLarge.Text +=
                        "<img align=\"absmiddle\" style=\"margin-top: 3px; margin-bottom: 5px;\" id=\"Pic3\" name=\"Pic3\" border=\"0\" src=\"" +
                        Image3URL + "?" + CommonLogic.GetRandomNumber( 1 , 1000000 ).ToString( ) + "\" />\n";
            }
            // END IMAGES   

            // COLORS & SIZES:
            txtColors.Text =
                    Server.HtmlEncode(
                            XmlCommon.GetLocaleEntry( DB.RSField( rs , "Colors" ) , ddLocale.SelectedValue , false ) );
            txtColorSKUModifiers.Text = Server.HtmlEncode( DB.RSField( rs , "ColorSKUModifiers" ) );
            txtSizes.Text =
                    Server.HtmlEncode(
                            XmlCommon.GetLocaleEntry( DB.RSField( rs , "Sizes" ) , ddLocale.SelectedValue , false ) );
            ;
            txtSizeSKUModifiers.Text = Server.HtmlEncode( DB.RSField( rs , "SizeSKUModifiers" ) );

            string temp = "";
            trColorSizeSummary.Visible = false;
            // size/color tables for display purposes only:
            if ( Editing &&
                 ( DB.RSField( rs , "Colors" ).Length != 0 || DB.RSField( rs , "ColorSKUModifiers" ).Length != 0 ||
                   DB.RSField( rs , "Sizes" ).Length != 0 || DB.RSField( rs , "SizeSKUModifiers" ).Length != 0 ) )
            {
                trColorSizeSummary.Visible = true;
                temp += "<br/><b>" + ColorOptionPrompt + "/" + SizeOptionPrompt +
                        " Tables:&nbsp;&nbsp;<small>(summary)</small></b>";
                temp +=
                        ( "<table border=\"1\" cellpadding=\"0\" cellspacing=\"0\"><tr valign=\"left\"><td colspan=\"2\" height=\"10\"></td></tr>\n" );
                temp += ( "<tr valign=\"left\">\n" );
                //temp += ("<td align=\"right\" valign=\"top\">" + ColorOptionPrompt + "/" + SizeOptionPrompt + " Tables:&nbsp;&nbsp;<small>(summary)</small></td>\n");
                temp += ( "<td align=\"left\" valign=\"top\">\n" );

                String[ ] Colors = DB.RSField( rs , "Colors" ).Split( ',' );
                String[ ] Sizes = DB.RSField( rs , "Sizes" ).Split( ',' );
                String[ ] ColorSKUModifiers = DB.RSField( rs , "ColorSKUModifiers" ).Split( ',' );
                String[ ] SizeSKUModifiers = DB.RSField( rs , "SizeSKUModifiers" ).Split( ',' );

                for ( int i = Colors.GetLowerBound( 0 ) ; i <= Colors.GetUpperBound( 0 ) ; i++ )
                {
                    Colors[ i ] = Colors[ i ].Trim( );
                }

                for ( int i = Sizes.GetLowerBound( 0 ) ; i <= Sizes.GetUpperBound( 0 ) ; i++ )
                {
                    Sizes[ i ] = Sizes[ i ].Trim( );
                }

                for ( int i = ColorSKUModifiers.GetLowerBound( 0 ) ; i <= ColorSKUModifiers.GetUpperBound( 0 ) ; i++ )
                {
                    ColorSKUModifiers[ i ] = ColorSKUModifiers[ i ].Trim( );
                }

                for ( int i = SizeSKUModifiers.GetLowerBound( 0 ) ; i <= SizeSKUModifiers.GetUpperBound( 0 ) ; i++ )
                {
                    SizeSKUModifiers[ i ] = SizeSKUModifiers[ i ].Trim( );
                }

                int ColorCols = Colors.GetUpperBound( 0 );
                int SizeCols = Sizes.GetUpperBound( 0 );
                ColorCols = Math.Max( ColorCols , ColorSKUModifiers.GetUpperBound( 0 ) );
                SizeCols = Math.Max( SizeCols , SizeSKUModifiers.GetUpperBound( 0 ) );

                if ( DB.RSField( rs , "Colors" ).Length != 0 ||
                     DB.RSField( rs , "ColorSKUModifiers" ).Length != 0 )
                {
                    temp += ( "<tr>\n" );
                    temp += ( "<td><b>" + ColorOptionPrompt + "</b></td>\n" );
                    for ( int i = 0 ; i <= ColorCols ; i++ )
                    {
                        String ColorVal = String.Empty;
                        try
                        {
                            ColorVal = Colors[ i ];
                        }
                        catch {}
                        if ( ColorVal.Length == 0 )
                        {
                            ColorVal = "&nbsp;";
                        }
                        temp += ( "<td align=\"center\">" + ColorVal + "</td>\n" );
                    }
                    temp += ( "<tr>\n" );
                    temp += ( "<tr>\n" );
                    temp += ( "<td><b>SKU Modifier</b></td>\n" );
                    for ( int i = 0 ; i <= ColorCols ; i++ )
                    {
                        String SKUVal = String.Empty;
                        try
                        {
                            SKUVal = ColorSKUModifiers[ i ];
                        }
                        catch {}
                        if ( SKUVal.Length == 0 )
                        {
                            SKUVal = "&nbsp;";
                        }
                        temp += ( "<td align=\"center\">" + SKUVal + "</td>\n" );
                    }
                    temp += ( "<tr>\n" );
                    temp += ( "</table>\n" );
                    temp += ( "<br/><br/>" );
                }

                if ( DB.RSField( rs , "Sizes" ).Length != 0 ||
                     DB.RSField( rs , "SizeSKUModifiers" ).Length != 0 )
                {
                    temp += ( "<table cellpadding=\"2\" cellspacing=\"0\" border=\"1\">\n" );
                    temp += ( "<tr>\n" );
                    temp += ( "<td><b>" + SizeOptionPrompt + "</b></td>\n" );
                    for ( int i = 0 ; i <= SizeCols ; i++ )
                    {
                        String SizeVal = String.Empty;
                        try
                        {
                            SizeVal = Sizes[ i ];
                        }
                        catch {}
                        if ( SizeVal.Length == 0 )
                        {
                            SizeVal = "&nbsp;";
                        }
                        temp += ( "<td align=\"center\">" + SizeVal + "</td>\n" );
                    }
                    temp += ( "<tr>\n" );
                    temp += ( "<tr>\n" );
                    temp += ( "<td><b>SKU Modifier</b></td>\n" );
                    for ( int i = 0 ; i <= SizeCols ; i++ )
                    {
                        String SKUVal = String.Empty;
                        try
                        {
                            SKUVal = SizeSKUModifiers[ i ];
                        }
                        catch {}
                        if ( SKUVal.Length == 0 )
                        {
                            SKUVal = "&nbsp;";
                        }
                        temp += ( "<td align=\"center\">" + SKUVal + "</td>\n" );
                    }
                    temp += ( "<tr>\n" );
                    temp += ( "</table>\n" );
                }

                temp += ( "</td>\n" );
                temp += ( "</tr></table>\n" );
            }
            ltColorSizeSummary.Text = temp;
        }
        else
        {
            pnlDelete.Visible = false;
            ltStatus.Text = "Adding Variant";
            btnSubmit.Text = "Add New";

            ltEntity.Text = entity.GetEntityBreadcrumb6( eID , ThisCustomer.LocaleSetting ) +
                            " : <a href=\"entityEditProducts.aspx?iden=" + pID + "&entityName=" + eName +
                            "&EntityFilterID=" + eID + "\">" +
                            AppLogic.GetProductName( pID , ThisCustomer.LocaleSetting ) + " (" + pID +
                            ")</a> : <a href=\"entityProductVariantsOverview.aspx?ProductID=" + pID + "&entityname=" +
                            eName + "&EntityID=" + eID + "\">Variant Management</a> : Adding New Variant";

            //DESCRIPTION
            /*ltDescription.Text = ("<div id=\"idDescription\" style=\"height: 1%;\">");
            ltDescription.Text += ("<textarea rows=\"" + AppLogic.AppConfigUSInt("Admin_TextareaHeight") + "\" cols=\"" + AppLogic.AppConfigUSInt("Admin_TextareaWidth") + "\" id=\"Description\" name=\"Description\"></textarea>\n");
            ltDescription.Text += (AppLogic.GenerateInnovaEditor("Description"));
            ltDescription.Text += ("</div>");*/

            //INVENTORY
            trCurrentInventory.Visible = false;
            trManageInventory.Visible = false;
            if ( ProductTracksInventoryBySizeAndColor )
            {
                trManageInventory.Visible = true;
                ltManageInventory.Text = ( "<a href=\"entityEditInventory.aspx?productid=" + pID.ToString( ) +
                                           "&variantid=" + vID.ToString( ) + "\">Click Here</a>\n" );
            }
            else
            {
                trCurrentInventory.Visible = true;
            }

            //set defaults
            rblCustomerEntersPrice.SelectedIndex = 0;
            rblDownload.SelectedIndex = 0;
            rblFreeShipping.SelectedIndex = 0;
            rblPublished.SelectedIndex = 1;
            rblRecurring.SelectedIndex = 0;
            rblRecurringIntervalType.SelectedIndex = 2;
            rblShipSeparately.SelectedIndex = 0;
            rblSubscriptionIntervalType.SelectedIndex = 2;
            rblTaxable.SelectedIndex = 1;
        }

        ltScript2.Text = ( "<script type=\"text/javascript\">\n" );
        ltScript2.Text += ( "function DeleteImage(imgurl,name)\n" );
        ltScript2.Text += ( "{\n" );
        ltScript2.Text += ( "if(confirm('Are you sure you want to delete this image?')){\n" );
        ltScript2.Text +=
                ( "window.open('deleteimage.aspx?imgurl=' + imgurl + '&FormImageName=' + name,\"Admin_ML\",\"height=250,width=440,top=10,left=20,status=no,toolbar=no,menubar=no,scrollbars=yes,location=no\")\n" );
        ltScript2.Text += ( "}}\n" );
        ltScript2.Text += ( "</SCRIPT>\n" );
        rs.Close( );
    }

    protected void ddLocale_SelectedIndexChanged( object sender , EventArgs e )
    {
        ThisCustomer.ThisCustomerSession.SetVal( "entityUserLocale" , ddLocale.SelectedValue );
        LoadContent( );
    }

    protected void btnSubmit_Click( object sender , EventArgs e )
    {
        UpdateVariant( );
    }

    protected void UpdateVariant( )
    {
        bool Editing = Localization.ParseBoolean( ViewState[ "VariantEdit" ].ToString( ) );
        StringBuilder sql = new StringBuilder( );

        decimal Price = Decimal.Zero;
        decimal SalePrice = Decimal.Zero;
        decimal MSRP = Decimal.Zero;
        decimal Cost = Decimal.Zero;
        int Points = 0;
        int MinimumQuantity = 0;
        if ( txtPrice.Text.Length != 0 )
        {
            Price = Localization.ParseUSDecimal( txtPrice.Text );
        }
        if ( txtSalePrice.Text.Length != 0 )
        {
            SalePrice = Localization.ParseUSDecimal( txtSalePrice.Text );
        }

        if ( txtMSRP.Text.Length != 0 )
        {
            MSRP = Localization.ParseUSDecimal( txtMSRP.Text );
        }
        if ( txtActualCost.Text.Length != 0 )
        {
            Cost = Localization.ParseUSDecimal( txtActualCost.Text );
        }

        if ( txtMinimumQuantity.Text.Length != 0 )
        {
            MinimumQuantity = Localization.ParseNativeInt( txtMinimumQuantity.Text );
        }

        bool IsFirstVariantAdded =
                ( DB.GetSqlN( "select count(VariantID) as N from ProductVariant " + DB.GetNoLock( ) +
                              " where ProductID=" + pID.ToString( ) + " and Deleted=0" ) == 0 );

        if ( !Editing )
        {
            // ok to ADD them:
            String NewGUID = DB.GetNewGUID( );
            sql.Append(
                    "insert into productvariant(VariantGUID,ProductID,Name,ContentsBGColor,PageBGColor,GraphicsColor,ImageFilenameOverride,IsDefault,Description,RestrictedQuantities,FroogleDescription,Price,SalePrice,MSRP,Cost,Points,MinimumQuantity,SKUSuffix,ManufacturerPartNumber,Weight,Dimensions,Inventory,SubscriptionInterval,SubscriptionIntervalType,Published,CustomerEntersPrice,CustomerEntersPricePrompt,IsRecurring,RecurringInterval,RecurringIntervalType,Colors,ColorSKUModifiers,Sizes,SizeSKUModifiers,IsTaxable,IsShipSeparately,IsDownload,FreeShipping,DownloadLocation) values(" );
            sql.Append( DB.SQuote( NewGUID ) + "," );
            sql.Append( pID.ToString( ) + "," );
            sql.Append(
                    DB.SQuote( AppLogic.FormLocaleXmlVariant( "Name" , txtName.Text , ddLocale.SelectedValue , vID ) ) +
                    "," );
            sql.Append( DB.SQuote( txtContentsBG.Text ) + "," );
            sql.Append( DB.SQuote( txtPageBG.Text ) + "," );
            sql.Append( DB.SQuote( txtSkinColor.Text ) + "," );
            sql.Append( DB.SQuote( txtImageOverride.Text ) + "," );
            if ( IsFirstVariantAdded )
            {
                sql.Append( "1," ); // IsDefault=1
            }
            else
            {
                sql.Append( "0," ); // IsDefault=0
            }
            string temp = AppLogic.FormLocaleXmlVariant("FroogleDescription", radDescription.Html, ddLocale.SelectedValue, vID);
            if ( temp.Length != 0 )
            {
                sql.Append( DB.SQuote( temp ) + "," );
            }
            else
            {
                sql.Append( "NULL," );
            }
            if ( txtRestrictedQuantities.Text.Length != 0 )
            {
                sql.Append( DB.SQuote( txtRestrictedQuantities.Text ) + "," );
            }
            else
            {
                sql.Append( "NULL," );
            }
            temp =
                    AppLogic.FormLocaleXmlVariant( "FroogleDescription" , txtFroogle.Text , ddLocale.SelectedValue , vID );
            if ( temp.Length != 0 )
            {
                sql.Append( DB.SQuote( temp ) + "," );
            }
            else
            {
                sql.Append( "NULL," );
            }
            sql.Append( Localization.DecimalStringForDB( Price ) + "," );
            sql.Append(
                    CommonLogic.IIF( SalePrice != Decimal.Zero , Localization.DecimalStringForDB( SalePrice ) , "NULL" ) +
                    "," );
            sql.Append( CommonLogic.IIF( MSRP != Decimal.Zero , Localization.DecimalStringForDB( MSRP ) , "NULL" ) + "," );
            sql.Append( CommonLogic.IIF( Cost != Decimal.Zero , Localization.DecimalStringForDB( Cost ) , "NULL" ) + "," );
            sql.Append( Localization.IntStringForDB( Points ) + "," );
            sql.Append(
                    CommonLogic.IIF( MinimumQuantity != 0 , Localization.IntStringForDB( MinimumQuantity ) , "NULL" ) +
                    "," );
            if ( txtSKU.Text.Length != 0 )
            {
                sql.Append( DB.SQuote( txtSKU.Text ) + "," );
            }
            else
            {
                sql.Append( "NULL," );
            }
            if ( txtManufacturePartNumber.Text.Length != 0 )
            {
                sql.Append( DB.SQuote( txtManufacturePartNumber.Text ) + "," );
            }
            else
            {
                sql.Append( "NULL," );
            }
            decimal Weight = Localization.ParseNativeDecimal( txtWeight.Text );
            sql.Append( CommonLogic.IIF( Weight != 0.0M , Localization.DecimalStringForDB( Weight ) , "NULL" ) + "," );
            if ( txtDimensions.Text.Length != 0 )
            {
                sql.Append( DB.SQuote( txtDimensions.Text ) + "," );
            }
            else
            {
                sql.Append( "NULL," );
            }
            sql.Append( CommonLogic.IIF( txtCurrentInventory.Text.Length != 0 , txtCurrentInventory.Text , "1000000" ) +
                        "," );
            sql.Append( Localization.ParseNativeInt( txtSubscriptionInterval.Text ).ToString( ) + "," );
            sql.Append( rblSubscriptionIntervalType.SelectedValue.ToString( ) + "," );
            sql.Append( rblPublished.SelectedValue + "," );
            sql.Append( rblCustomerEntersPrice.SelectedValue + "," );
            sql.Append(
                    DB.SQuote(
                            AppLogic.FormLocaleXmlVariant( "CustomerEntersPricePrompt" ,
                                                           txtCustomerEntersPricePrompt.Text , ddLocale.SelectedValue ,
                                                           vID ) ) + "," );
            sql.Append( rblRecurring.SelectedValue + "," );
            sql.Append( Localization.ParseNativeInt( txtRecurringInterval.Text ).ToString( ) + "," );
            sql.Append( rblRecurringIntervalType.SelectedValue + "," );
            sql.Append(
                    DB.SQuote(
                            AppLogic.FormLocaleXmlVariant( "Colors" , txtColors.Text , ddLocale.SelectedValue , vID ).
                                    Replace( ", " , "," ).Replace( " ," , "," ).Replace( "'" , "" ).Trim( ) ) + "," );
            sql.Append( DB.SQuote( txtColorSKUModifiers.Text.Replace( ", " , "," ).Replace( " ," , "," ).Trim( ) ) + "," );
            sql.Append(
                    DB.SQuote(
                            AppLogic.FormLocaleXmlVariant( "Sizes" , txtSizes.Text , ddLocale.SelectedValue , vID ).
                                    Replace( ", " , "," ).Replace( " ," , "," ).Replace( "'" , "" ).Trim( ) ) + "," );
            sql.Append( DB.SQuote( txtSizeSKUModifiers.Text.Replace( ", " , "," ).Replace( " ," , "," ).Trim( ) ) + "," );
            sql.Append( rblTaxable.SelectedValue + "," );
            sql.Append( rblShipSeparately.SelectedValue + "," );
            sql.Append( rblDownload.SelectedValue + "," );
            sql.Append( rblFreeShipping.SelectedValue + "," );
            String DLoc = txtDownloadLocation.Text;
            if ( DLoc.StartsWith( "/" ) )
            {
                DLoc = DLoc.Substring( 1 , DLoc.Length - 1 ); // remove leading / char!
            }
            sql.Append( DB.SQuote( DLoc ) );
            sql.Append( ")" );
            try
            {
                DB.ExecuteSQL( sql.ToString( ) );
            }
            catch ( Exception ex )
            {
                throw new ArgumentException( "Error in EditVariant(.RunSql), Msg=[" +
                                             CommonLogic.GetExceptionDetail( ex , String.Empty ) + "], Sql=[" +
                                             sql.ToString( ) + "]" );
            }

            //Get variantID for editing
            IDataReader rs =
                    DB.GetRS( "select VariantID from productvariant  " + DB.GetNoLock( ) +
                              " where deleted=0 and VariantGUID=" + DB.SQuote( NewGUID ) );
            rs.Read( );
            vID = DB.RSFieldInt( rs , "VariantID" );
            ViewState.Add( "VariantEdit" , true );
            ViewState.Add( "VariantEditID" , vID );
            rs.Close( );

            resetError( "Variant Added." , false );
        }
        else
        {
            // ok to update:
            sql.Append( "update productvariant set " );
            sql.Append( "ProductID=" + pID.ToString( ) + "," );
            sql.Append( "Name=" +
                        DB.SQuote( AppLogic.FormLocaleXmlVariant( "Name" , txtName.Text , ddLocale.SelectedValue , vID ) ) +
                        "," );
            sql.Append( "ContentsBGColor=" + DB.SQuote( txtContentsBG.Text ) + "," );
            sql.Append( "PageBGColor=" + DB.SQuote( txtPageBG.Text ) + "," );
            sql.Append( "GraphicsColor=" + DB.SQuote( txtSkinColor.Text ) + "," );
            sql.Append( "ImageFilenameOverride=" + DB.SQuote( txtImageOverride.Text ) + "," );
            string temp = AppLogic.FormLocaleXmlVariant("FroogleDescription", radDescription.Html, ddLocale.SelectedValue, vID);
            if ( temp.Length != 0 )
            {
                sql.Append( "Description=" + DB.SQuote( temp ) + "," );
            }
            else
            {
                sql.Append( "Description=NULL," );
            }
            if ( txtRestrictedQuantities.Text.Length != 0 )
            {
                sql.Append( "RestrictedQuantities=" + DB.SQuote( txtRestrictedQuantities.Text ) + "," );
            }
            else
            {
                sql.Append( "RestrictedQuantities=NULL," );
            }
            temp =
                    AppLogic.FormLocaleXmlVariant( "FroogleDescription" , txtFroogle.Text , ddLocale.SelectedValue , vID );
            if ( temp.Length != 0 )
            {
                sql.Append( "FroogleDescription=" + DB.SQuote( temp ) + "," );
            }
            else
            {
                sql.Append( "FroogleDescription=NULL," );
            }
            sql.Append( "Price=" + Localization.DecimalStringForDB( Price ) + "," );
            sql.Append( "SalePrice=" +
                        CommonLogic.IIF( SalePrice != Decimal.Zero , Localization.DecimalStringForDB( SalePrice ) ,
                                         "NULL" ) + "," );
            sql.Append( "MSRP=" +
                        CommonLogic.IIF( MSRP != Decimal.Zero , Localization.DecimalStringForDB( MSRP ) , "NULL" ) + "," );
            sql.Append( "Cost=" +
                        CommonLogic.IIF( Cost != Decimal.Zero , Localization.DecimalStringForDB( Cost ) , "NULL" ) + "," );
            sql.Append( "Points=" + Localization.IntStringForDB( Points ) + "," );
            sql.Append( "MinimumQuantity=" +
                        CommonLogic.IIF( MinimumQuantity != 0 , Localization.IntStringForDB( MinimumQuantity ) , "NULL" ) +
                        "," );
            if ( txtSKU.Text.Length != 0 )
            {
                sql.Append( "SKUSuffix=" + DB.SQuote( txtSKU.Text ) + "," );
            }
            else
            {
                sql.Append( "SKUSuffix=NULL," );
            }
            if ( txtManufacturePartNumber.Text.Length != 0 )
            {
                sql.Append( "ManufacturerPartNumber=" + DB.SQuote( txtManufacturePartNumber.Text ) + "," );
            }
            else
            {
                sql.Append( "ManufacturerPartNumber=NULL," );
            }
            decimal Weight = Localization.ParseNativeDecimal( txtWeight.Text );
            sql.Append( "Weight=" +
                        CommonLogic.IIF( Weight != 0.0M , Localization.DecimalStringForDB( Weight ) , "NULL" ) + "," );
            if ( txtDimensions.Text.Length != 0 )
            {
                sql.Append( "Dimensions=" + DB.SQuote( txtDimensions.Text ) + "," );
            }
            else
            {
                sql.Append( "Dimensions=NULL," );
            }
            sql.Append( "Inventory=" +
                        CommonLogic.IIF( txtCurrentInventory.Text.Length != 0 , txtCurrentInventory.Text , "1000000" ) +
                        "," );
            sql.Append( "SubscriptionInterval=" +
                        Localization.ParseNativeInt( txtSubscriptionInterval.Text ).ToString( ) + "," );
            sql.Append( "SubscriptionIntervalType=" + rblSubscriptionIntervalType.SelectedValue.ToString( ) + "," );
            sql.Append( "Published=" + rblPublished.SelectedValue + "," );
            sql.Append( "CustomerEntersPrice=" + rblCustomerEntersPrice.SelectedValue + "," );
            sql.Append( "CustomerEntersPricePrompt=" +
                        DB.SQuote(
                                AppLogic.FormLocaleXmlVariant( "CustomerEntersPricePrompt" ,
                                                               txtCustomerEntersPricePrompt.Text ,
                                                               ddLocale.SelectedValue , vID ) ) + "," );
            sql.Append( "IsRecurring=" + rblRecurring.SelectedValue + "," );
            sql.Append( "RecurringInterval=" + Localization.ParseNativeInt( txtRecurringInterval.Text ).ToString( ) +
                        "," );
            sql.Append( "RecurringIntervalType=" + rblRecurringIntervalType.SelectedValue + "," );
            sql.Append( "Colors=" +
                        DB.SQuote(
                                AppLogic.FormLocaleXmlVariant( "Colors" , txtColors.Text , ddLocale.SelectedValue , vID )
                                        .Replace( ", " , "," ).Replace( " ," , "," ).Replace( "'" , "" ).Trim( ) ) + "," );
            sql.Append( "ColorSKUModifiers=" +
                        DB.SQuote( txtColorSKUModifiers.Text.Replace( ", " , "," ).Replace( " ," , "," ).Trim( ) ) + "," );
            sql.Append( "Sizes=" +
                        DB.SQuote(
                                AppLogic.FormLocaleXmlVariant( "Sizes" , txtSizes.Text , ddLocale.SelectedValue , vID ).
                                        Replace( ", " , "," ).Replace( " ," , "," ).Replace( "'" , "" ).Trim( ) ) + "," );
            sql.Append( "SizeSKUModifiers=" +
                        DB.SQuote( txtSizeSKUModifiers.Text.Replace( ", " , "," ).Replace( " ," , "," ).Trim( ) ) + "," );
            sql.Append( "IsTaxable=" + rblTaxable.SelectedValue + "," );
            sql.Append( "IsShipSeparately=" + rblShipSeparately.SelectedValue + "," );
            sql.Append( "IsDownload=" + rblDownload.SelectedValue + "," );
            sql.Append( "FreeShipping=" + rblFreeShipping.SelectedValue + "," );
            String DLoc = txtDownloadLocation.Text;
            if ( DLoc.StartsWith( "/" ) )
            {
                DLoc = DLoc.Substring( 1 , DLoc.Length - 1 ); // remove leading / char!
            }
            sql.Append( "DownloadLocation=" + DB.SQuote( DLoc ) );
            sql.Append( " where VariantID=" + vID.ToString( ) );
            try
            {
                DB.ExecuteSQL( sql.ToString( ) );
            }
            catch ( Exception ex )
            {
                throw new ArgumentException( "Error in EditVariant(.RunSql), Msg=[" +
                                             CommonLogic.GetExceptionDetail( ex , String.Empty ) + "], Sql=[" +
                                             sql.ToString( ) + "]" );
            }

            ViewState.Add( "VariantEdit" , true );
            ViewState.Add( "VariantEditID" , vID );

            resetError( "Variant Updated." , false );
        }

        // handle shipping costs uploaded (if any):
        if ( ShipCalcID == Shipping.ShippingCalculationEnum.UseIndividualItemShippingCosts )
        {
            DB.ExecuteSQL( "delete from ShippingByProduct where VariantID=" + vID.ToString( ) );
            IDataReader rs3 =
                    DB.GetRS( "select * from ShippingMethod " + DB.GetNoLock( ) +
                              " where IsRTShipping=0 order by DisplayOrder" );
            while ( rs3.Read( ) )
            {
                String FldName = "ShippingCost_" + DB.RSFieldInt( rs3 , "ShippingMethodID" );
                decimal ShippingCost = CommonLogic.FormUSDecimal( FldName );
                DB.ExecuteSQL( "insert ShippingByProduct(VariantID,ShippingMethodID,ShippingCost) values(" +
                               vID.ToString( ) + "," + DB.RSFieldInt( rs3 , "ShippingMethodID" ).ToString( ) + "," +
                               Localization.CurrencyStringForDBWithoutExchangeRate( ShippingCost ) + ")" );
            }
            rs3.Close( );
        }

        //Upload Images
        HandleImageSubmits( );

        LoadContent( );
    }

    public void HandleImageSubmits( )
    {
        // handle image uploaded:
        String FN = CommonLogic.FormCanBeDangerousContent( "ImageFilenameOverride" ).Trim( );
        if ( FN.Length == 0 )
        {
            FN = vID.ToString( );
        }

        String ErrorMsg = String.Empty;

        String Image1 = String.Empty;
        String TempImage1 = String.Empty;
        HttpPostedFile Image1File = fuIcon.PostedFile;
        if ( Image1File != null &&
             Image1File.ContentLength != 0 )
        {
            // delete any current image file first
            try
            {
                if ( FN.EndsWith( ".jpg" , StringComparison.InvariantCultureIgnoreCase ) ||
                     FN.EndsWith( ".gif" , StringComparison.InvariantCultureIgnoreCase ) ||
                     FN.EndsWith( ".png" , StringComparison.InvariantCultureIgnoreCase ) )
                {
                    File.Delete( AppLogic.GetImagePath( "Product" , "icon" , true ) + FN );
                }
                else
                {
                    File.Delete( AppLogic.GetImagePath( "Variant" , "icon" , true ) + FN + ".jpg" );
                    File.Delete( AppLogic.GetImagePath( "Variant" , "icon" , true ) + FN + ".gif" );
                    File.Delete( AppLogic.GetImagePath( "Variant" , "icon" , true ) + FN + ".png" );
                }
            }
            catch {}

            String s = Image1File.ContentType;
            switch ( Image1File.ContentType )
            {
                case "image/gif" :

                    TempImage1 = AppLogic.GetImagePath( "Variant" , "icon" , true ) + "tmp_" + FN + ".gif";
                    Image1 = AppLogic.GetImagePath( "Variant" , "icon" , true ) + FN + ".gif";
                    Image1File.SaveAs( TempImage1 );
                    AppLogic.ResizeEntityOrObject( "Variant" , TempImage1 , Image1 , "icon" , "image/gif" );

                    break;
                case "image/x-png" :
                case "image/png" :

                    TempImage1 = AppLogic.GetImagePath( "Variant" , "icon" , true ) + "tmp_" + FN + ".png";
                    Image1 = AppLogic.GetImagePath( "Variant" , "icon" , true ) + FN + ".png";
                    Image1File.SaveAs( TempImage1 );
                    AppLogic.ResizeEntityOrObject( "Variant" , TempImage1 , Image1 , "icon" , "image/png" );

                    break;
                case "image/jpg" :
                case "image/jpeg" :
                case "image/pjpeg" :

                    TempImage1 = AppLogic.GetImagePath( "Variant" , "icon" , true ) + "tmp_" + FN + ".jpg";
                    Image1 = AppLogic.GetImagePath( "Variant" , "icon" , true ) + FN + ".jpg";
                    Image1File.SaveAs( TempImage1 );
                    AppLogic.ResizeEntityOrObject( "Variant" , TempImage1 , Image1 , "icon" , "image/jpeg" );

                    break;
            }
            AppLogic.DisposeOfTempImage( TempImage1 );
        }

        String Image2 = String.Empty;
        String TempImage2 = String.Empty;
        HttpPostedFile Image2File = fuMedium.PostedFile;
        if ( Image2File != null &&
             Image2File.ContentLength != 0 )
        {
            // delete any current image file first
            try
            {
                File.Delete( AppLogic.GetImagePath( "Variant" , "medium" , true ) + FN + ".jpg" );
                File.Delete( AppLogic.GetImagePath( "Variant" , "medium" , true ) + FN + ".gif" );
                File.Delete( AppLogic.GetImagePath( "Variant" , "medium" , true ) + FN + ".png" );
            }
            catch {}

            String s = Image2File.ContentType;
            switch ( Image2File.ContentType )
            {
                case "image/gif" :

                    TempImage2 = AppLogic.GetImagePath( "Variant" , "medium" , true ) + "tmp_" + FN + ".gif";
                    Image2 = AppLogic.GetImagePath( "Variant" , "medium" , true ) + FN + ".gif";
                    Image2File.SaveAs( TempImage2 );
                    AppLogic.ResizeEntityOrObject( "Variant" , TempImage2 , Image2 , "medium" , "image/gif" );

                    break;
                case "image/x-png" :
                case "image/png" :

                    TempImage2 = AppLogic.GetImagePath( "Variant" , "medium" , true ) + "tmp_" + FN + ".png";
                    Image2 = AppLogic.GetImagePath( "Variant" , "medium" , true ) + FN + ".png";
                    Image2File.SaveAs( TempImage2 );
                    AppLogic.ResizeEntityOrObject( "Variant" , TempImage2 , Image2 , "medium" , "image/png" );

                    break;
                case "image/jpg" :
                case "image/jpeg" :
                case "image/pjpeg" :

                    TempImage2 = AppLogic.GetImagePath( "Variant" , "medium" , true ) + "tmp_" + FN + ".jpg";
                    Image2 = AppLogic.GetImagePath( "Variant" , "medium" , true ) + FN + ".jpg";
                    Image2File.SaveAs( TempImage2 );
                    AppLogic.ResizeEntityOrObject( "Variant" , TempImage2 , Image2 , "medium" , "image/jpeg" );

                    break;
            }
            AppLogic.DisposeOfTempImage( TempImage2 );
        }

        String Image3 = String.Empty;
        String TempImage3 = String.Empty;
        HttpPostedFile Image3File = fuLarge.PostedFile;
        if ( Image3File != null &&
             Image3File.ContentLength != 0 )
        {
            // delete any current image file first
            try
            {
                File.Delete( AppLogic.GetImagePath( "Variant" , "large" , true ) + vID.ToString( ) + ".jpg" );
                File.Delete( AppLogic.GetImagePath( "Variant" , "large" , true ) + vID.ToString( ) + ".gif" );
                File.Delete( AppLogic.GetImagePath( "Variant" , "large" , true ) + vID.ToString( ) + ".png" );
            }
            catch {}

            String s = Image3File.ContentType;
            switch ( Image3File.ContentType )
            {
                case "image/gif" :

                    TempImage3 = AppLogic.GetImagePath( "Variant" , "large" , true ) + "tmp_" + FN + ".gif";
                    Image3 = AppLogic.GetImagePath( "Variant" , "large" , true ) + FN + ".gif";
                    Image3File.SaveAs( TempImage3 );
                    AppLogic.ResizeEntityOrObject( "Variant" , TempImage3 , Image3 , "large" , "image/gif" );
                    AppLogic.CreateOthersFromLarge( "Variant" , TempImage3 , FN , "image/gif" );

                    break;
                case "image/x-png" :
                case "image/png" :

                    TempImage3 = AppLogic.GetImagePath( "Variant" , "large" , true ) + "tmp_" + FN + ".png";
                    Image3 = AppLogic.GetImagePath( "Variant" , "large" , true ) + FN + ".png";
                    Image3File.SaveAs( TempImage3 );
                    AppLogic.ResizeEntityOrObject( "Variant" , TempImage3 , Image3 , "large" , "image/png" );
                    AppLogic.CreateOthersFromLarge( "Variant" , TempImage3 , FN , "image/png" );

                    break;
                case "image/jpg" :
                case "image/jpeg" :
                case "image/pjpeg" :

                    TempImage3 = AppLogic.GetImagePath( "Variant" , "large" , true ) + "tmp_" + FN + ".jpg";
                    Image3 = AppLogic.GetImagePath( "Variant" , "large" , true ) + FN + ".jpg";
                    Image3File.SaveAs( TempImage3 );
                    AppLogic.ResizeEntityOrObject( "Variant" , TempImage3 , Image3 , "large" , "image/jpeg" );
                    AppLogic.CreateOthersFromLarge( "Variant" , TempImage3 , FN , "image/jpeg" );

                    break;
            }
        }
    }

    protected void btnDeleteVariant_Click( object sender , EventArgs e )
    {
        string sql = "if (select count(*) from dbo.ProductVariant where ProductID = " + pID.ToString( ) +
                     " and VariantID <> " + vID.ToString( ) + " and deleted = 0 ) = 0 \n";
        sql += "select 'This is the only Variant for this product and cannot be deleted' \n";
        sql += "else begin \n";
        sql +=
                "update dbo.ProductVariant set Deleted = case deleted when 1 then 0 else 1 end, isdefault = 0 where VariantID = " +
                vID.ToString( ) + "\n";
        sql += " if exists (select * from dbo.ProductVariant where ProductID = " + pID.ToString( ) +
               " and published = 1 and deleted = 0 and isdefault = 1 ) declare @t tinyint \n";
        sql +=
                " else update dbo.ProductVariant set isdefault = 1 where VariantID = (select top 1 VariantID from dbo.ProductVariant where ProductID = " +
                pID.ToString( ) + " and published = 1 and deleted = 0)\n";
        sql += "select '' \n";
        sql += "end";

        IDataReader dr = DB.GetRS( sql );
        dr.Read( );
        string err = dr.GetString( 0 );
        dr.Close( );
        lblerr.Text = err;
        LoadContent( );
    }
}