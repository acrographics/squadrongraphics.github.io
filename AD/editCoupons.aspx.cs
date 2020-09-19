// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editCoupons.aspx.cs 5     9/14/06 12:05a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using AspDotNetStorefrontCommon;

public partial class editCoupons : System.Web.UI.Page
{
    protected string couponCode;
    protected int couponID;

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");

        couponID = CommonLogic.QueryStringNativeInt("iden");
                
        if (!IsPostBack)
        {
            ViewState["EditingCoupon"] = false;
            ViewState["EditingCouponID"] = "0";

            resetForm();
            loadScript(true);

            if (couponID > 0)
            {
                ViewState["EditingCoupon"] = true;
                ViewState["EditingCouponID"] = couponID; 

                getCouponDetails();
            }
            else
            {
                ltCoupon.Text = "NEW Coupon";
                btnSubmit.Text = "Add Coupon";
            }
        }
    }

    protected void getCouponDetails()
    {
        IDataReader rs = DB.GetRS("select * from Coupon  " + DB.GetNoLock() + " where CouponID=" + ViewState["EditingCouponID"].ToString());
        if (!rs.Read())
        {
            rs.Close();
            resetError("Unable to retrieve data.", true);
            return;
        }

        //editing coupon
        btnSubmit.Text = "Update Coupon";
        
        txtCode.Text = ltCoupon.Text = Server.HtmlEncode(DB.RSField(rs, "CouponCode"));
        txtCategory.Text = Server.HtmlEncode(DB.RSField(rs, "ValidForCategories"));
        txtCustomers.Text = Server.HtmlEncode(DB.RSField(rs, "ValidForCustomers"));
        txtDate.Text = Localization.ToNativeShortDateString(DB.RSFieldDateTime(rs, "ExpirationDate"));
        
        txtDescription.Text = DB.RSField(rs, "Description");
        //ltDescription.Text = AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Description"), "Description", true, true, false, "", 0, 0, 5, 80, false);

        txtDiscountAmount.Text = Localization.CurrencyStringForDBWithoutExchangeRate(DB.RSFieldDecimal(rs, "DiscountAmount"));
        txtDiscountPercent.Text = Localization.CurrencyStringForDBWithoutExchangeRate(DB.RSFieldDecimal(rs, "DiscountPercent"));
        txtManufacturer.Text = Server.HtmlEncode(DB.RSField(rs, "ValidForManufacturers"));
        txtMinOrderAmount.Text = Localization.CurrencyStringForDBWithoutExchangeRate(DB.RSFieldDecimal(rs, "RequiresMinimumOrderAmount"));
        txtNUses.Text = DB.RSFieldInt(rs, "ExpiresAfterNUses").ToString();
        txtProducts.Text = Server.HtmlEncode(DB.RSField(rs, "ValidForProducts"));
        txtSection.Text = Server.HtmlEncode(DB.RSField(rs, "ValidForSections"));
        ltNumUses.Text = DB.RSFieldInt(rs, "NumUses").ToString();

        ddType.SelectedIndex = DB.RSFieldInt(rs, "CouponType");
        try
        {
            rbFirstUsage.Items.FindByValue(rs["ExpiresOnFirstUseByAnyCustomer"].ToString()).Selected = true;
            rbOneUsage.Items.FindByValue(rs["ExpiresAfterOneUsageByEachCustomer"].ToString()).Selected = true;
            rbShipping.Items.FindByValue(rs["DiscountIncludesFreeShipping"].ToString()).Selected = true;
        }
        catch { }
        rs.Close();
    }

    protected void resetError(string error, bool isError)
    {
        string str = "<font class=\"noticeMsg\">NOTICE:</font>&nbsp;&nbsp;&nbsp;";
        if (isError)
            str = "<font class=\"errorMsg\">ERROR:</font>&nbsp;&nbsp;&nbsp;";

        if (error.Length > 0)
            str += error + "";
        else
            str = "";

        ((Literal)Form.FindControl("ltError")).Text = str;
    }
   
    protected void loadScript(bool load)
    {
        if (load)
        {
            ltScript.Text = ("\n<script type=\"text/javascript\">\n");
            ltScript.Text += ("    Calendar.setup({\n");
            ltScript.Text += ("        inputField     :    \"txtDate\",      // id of the input field\n");
            ltScript.Text += ("        ifFormat       :    \"" + Localization.JSCalendarDateFormatSpec() + "\",       // format of the input field\n");
            ltScript.Text += ("        showsTime      :    false,            // will display a time selector\n");
            ltScript.Text += ("        button         :    \"f_trigger_s\",   // trigger for the calendar (button ID)\n");
            ltScript.Text += ("        singleClick    :    true            // double-click mode\n");
            ltScript.Text += ("    });\n");
            ltScript.Text += ("</script>\n");

            if (AppLogic.NumLocaleSettingsInstalled() > 1)
            {
                ltScript1.Text = CommonLogic.ReadFile("jscripts/tabs.js", true);
            }

            ltDate.Text = "<img src=\"" + AppLogic.LocateImageURL("skins/skin_1/images/calendar.gif") + "\" style=\"cursor:hand;cursor:pointer;\" align=\"absmiddle\" id=\"f_trigger_s\">&nbsp;<small>(" + Localization.ShortDateFormat() + ")</small>";

            ltStyles.Text = ("  <!-- calendar stylesheet -->\n");
            ltStyles.Text += ("  <link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"jscalendar/calendar-win2k-cold-1.css\" title=\"win2k-cold-1\" />\n");
            ltStyles.Text += ("\n");
            ltStyles.Text += ("  <!-- main calendar program -->\n");
            ltStyles.Text += ("  <script type=\"text/javascript\" src=\"jscalendar/calendar.js\"></script>\n");
            ltStyles.Text += ("\n");
            ltStyles.Text += ("  <!-- language for the calendar -->\n");
            ltStyles.Text += ("  <script type=\"text/javascript\" src=\"jscalendar/lang/" + Localization.JSCalendarLanguageFile() + "\"></script>\n");
            ltStyles.Text += ("\n");
            ltStyles.Text += ("  <!-- the following script defines the Calendar.setup helper function, which makes\n");
            ltStyles.Text += ("       adding a calendar a matter of 1 or 2 lines of code. -->\n");
            ltStyles.Text += ("  <script type=\"text/javascript\" src=\"jscalendar/calendar-setup.js\"></script>\n");
        }
        else
        {
            ltScript.Text = "";
            ltStyles.Text = "";
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        bool Editing = Localization.ParseBoolean(ViewState["EditingCoupon"].ToString());
        string CouponID = ViewState["EditingCouponID"].ToString();
        IDataReader rs;

        if (Editing)
        {
            // see if this coupon already exists://deleted=0 
            int N = DB.GetSqlN("select count(CouponCode) as N from coupon  " + DB.GetNoLock() + " where CouponID<>" + CouponID.ToString() + " and lower(CouponCode)=" + DB.SQuote(CommonLogic.Left(txtCode.Text, 100).ToLowerInvariant().Trim()));
            if (N != 0)
            {
                resetError("There is already another coupon with that code (could be a deleted coupon).", true);
                return;
            }
        }
        else
        {
            // see if this name is already there://deleted=0 
            int N = DB.GetSqlN("select count(CouponID) as N from Coupon  " + DB.GetNoLock() + " where lower(CouponCode)=" + DB.SQuote(CommonLogic.Left(txtCode.Text, 100).ToLowerInvariant().Trim()));
            if (N != 0)
            {
                resetError("There is already another coupon with that name (could be a deleted coupon).", true);
                return;
            }
        }

        StringBuilder sql = new StringBuilder(2500);
        if (!Editing)
        {
            // ok to add them:
            String NewGUID = DB.GetNewGUID();
            sql.Append("insert into coupon(CouponGUID,CouponCode,CouponType,ExpirationDate,Description,DiscountPercent,DiscountAmount,DiscountIncludesFreeShipping,ExpiresOnFirstUseByAnyCustomer,ExpiresAfterNUses,NumUses,ExpiresAfterOneUsageByEachCustomer,ValidForCustomers,ValidForProducts,ValidForManufacturers,RequiresMinimumOrderAmount,ValidForCategories,ValidForSections) values(");
            sql.Append(DB.SQuote(NewGUID) + ",");
            sql.Append(DB.SQuote(CommonLogic.Left(txtCode.Text, 100)) + ",");
            sql.Append(ddType.SelectedValue + ",");
            sql.Append(DB.SQuote(Localization.ConvertLocaleDateTime(CommonLogic.Left(txtDate.Text, 100), Localization.GetWebConfigLocale(), Localization.GetSqlServerLocale())) + ",");
            
            sql.Append(DB.SQuote(txtDescription.Text) + ",");
            //sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Description")) + ",");

            sql.Append(Localization.CurrencyStringForDBWithoutExchangeRate(Localization.ParseUSDecimal(txtDiscountPercent.Text)) + ",");
            sql.Append(Localization.CurrencyStringForDBWithoutExchangeRate(Localization.ParseUSDecimal(txtDiscountAmount.Text)) + ",");
            sql.Append(rbShipping.SelectedValue + ",");
            sql.Append(rbFirstUsage.SelectedValue + ",");
            sql.Append(Localization.ParseUSInt(txtNUses.Text) + ",");
            sql.Append("0,");
            sql.Append(rbOneUsage.SelectedValue + ",");
            sql.Append(DB.SQuote(txtCustomers.Text) + ",");
            sql.Append(DB.SQuote(txtProducts.Text) + ",");
            sql.Append(DB.SQuote(txtManufacturer.Text) + ",");
            sql.Append(Localization.CurrencyStringForDBWithoutExchangeRate(Localization.ParseUSDecimal(txtMinOrderAmount.Text)) + ",");
            sql.Append(DB.SQuote(txtCategory.Text) + ",");
            sql.Append(DB.SQuote(txtSection.Text) + "");
            sql.Append(")");
            DB.ExecuteSQL(sql.ToString());

            resetError("Coupon added.", false);

            rs = DB.GetRS("select CouponID from Coupon " + DB.GetNoLock() + " where CouponGUID=" + DB.SQuote(NewGUID));
            rs.Read();
            CouponID = DB.RSFieldInt(rs, "CouponID").ToString();
            ViewState["EditingCoupon"] = true;
            ViewState["EditingCouponID"] = CouponID.ToString();
            rs.Close();
            
            getCouponDetails();
        }
        else
        {
            // ok to update:
            sql.Append("update coupon set ");
            sql.Append("CouponCode=" + DB.SQuote(CommonLogic.Left(txtCode.Text, 100)) + ",");
            sql.Append("CouponType=" + ddType.SelectedValue + ",");
            sql.Append("ExpirationDate=" + DB.SQuote(Localization.ConvertLocaleDateTime(CommonLogic.Left(txtDate.Text, 100), Localization.GetWebConfigLocale(), Localization.GetSqlServerLocale())) + ",");
            

            sql.Append("Description=" + DB.SQuote(txtDescription.Text) + ",");
            //sql.Append("Description=" + DB.SQuote(AppLogic.FormLocaleXml("Description")) + ",");

            sql.Append("DiscountPercent=" + Localization.CurrencyStringForDBWithoutExchangeRate(Localization.ParseUSDecimal(txtDiscountPercent.Text)) + ",");
            sql.Append("DiscountAmount=" + Localization.CurrencyStringForDBWithoutExchangeRate(Localization.ParseUSDecimal(txtDiscountAmount.Text)) + ",");
            sql.Append("DiscountIncludesFreeShipping=" + rbShipping.SelectedValue + ",");
            sql.Append("ExpiresOnFirstUseByAnyCustomer=" + rbFirstUsage.SelectedValue + ",");
            sql.Append("ExpiresAfterNUses=" + Localization.ParseUSInt(txtNUses.Text) + ",");
            sql.Append("ExpiresAfterOneUsageByEachCustomer=" + rbOneUsage.SelectedValue+ ",");
            sql.Append("ValidForCustomers=" + DB.SQuote(txtCustomers.Text) + ",");
            sql.Append("ValidForProducts=" + DB.SQuote(txtProducts.Text) + ",");
            sql.Append("ValidForManufacturers=" + DB.SQuote(txtManufacturer.Text) + ",");
            sql.Append("RequiresMinimumOrderAmount=" + Localization.CurrencyStringForDBWithoutExchangeRate(Localization.ParseUSDecimal(txtMinOrderAmount.Text)) + ",");
            sql.Append("ValidForCategories=" + DB.SQuote(txtCategory.Text) + ",");
            sql.Append("ValidForSections=" + DB.SQuote(txtSection.Text) + " ");
            sql.Append("where CouponID=" + CouponID);
            DB.ExecuteSQL(sql.ToString());
            resetError("Coupon updated.", false);

            getCouponDetails();
        }

    }

    protected void resetForm()
    {
        txtCode.Text = "";
        txtCategory.Text = "";
        txtCustomers.Text = "";
        txtDate.Text = Localization.ToNativeShortDateString(System.DateTime.Now.AddMonths(1));
       
        txtDescription.Text = "";
        //ltDescription.Text = AppLogic.GetLocaleEntryFields("", "Description", true, true, false, "", 0, 0, 5, 80, false);
        
        txtDiscountAmount.Text = "0.0";
        txtDiscountPercent.Text = "0.0";
        txtManufacturer.Text = "";
        txtMinOrderAmount.Text = "0";
        txtNUses.Text = "0";
        txtProducts.Text = "";
        txtSection.Text = "";
        ltNumUses.Text = "0";

        ddType.SelectedIndex = 0;
        rbShipping.SelectedIndex = 0;
        rbOneUsage.SelectedIndex = 0;
        rbFirstUsage.SelectedIndex = 0;
    }
}
