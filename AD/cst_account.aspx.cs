// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/cst_account.aspx.cs 25    9/30/06 3:39p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Text;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for cst_account.
    /// </summary>
    public partial class cst_account : AspDotNetStorefront.SkinBase
    {

        private Customer TargetCustomer;
        private bool AllowShipToDifferentThanBillTo;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            TargetCustomer = new Customer(CommonLogic.QueryStringUSInt("CustomerID"), true, true);
            if (TargetCustomer.CustomerID == 0)
            {
                Response.Redirect("Customers.aspx");
            }
            if (TargetCustomer.IsAdminSuperUser && !ThisCustomer.IsAdminSuperUser)
            {
                throw new ArgumentException("Security Exception. Not Allowed");
            }
            AllowShipToDifferentThanBillTo = AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo") && !AppLogic.AppConfigBool("SkipShippingOnCheckout");
            SectionTitle = "<a href=\"Customers.aspx?searchfor=" + TargetCustomer.CustomerID.ToString() + "\">Customers</a> - Account Info: <a href=\"customers.aspx?searchfor=" + TargetCustomer.CustomerID.ToString() + "\">" + TargetCustomer.FullName() + " (" + TargetCustomer.EMail + ")</a>";
            if (CommonLogic.QueryStringCanBeDangerousContent("blockip") == "true")
            {
                try
                {
                    if (TargetCustomer.LastIPAddress.Length != 0)
                    {
                        // ignore duplicates:
                        DB.ExecuteSQL("insert RestrictedIP(IPAddress) values(" + DB.SQuote(TargetCustomer.LastIPAddress) + ")");
                    }
                }
                catch { }
            }
            if (CommonLogic.QueryStringCanBeDangerousContent("blockip") == "false")
            {
                DB.ExecuteSQL("delete from RestrictedIP where IPAddress=" + DB.SQuote(TargetCustomer.LastIPAddress));
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("clearsession") == "true")
            {
                CustomerSession.StaticClear(TargetCustomer.CustomerID);
            }

        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {

            if (CommonLogic.QueryStringCanBeDangerousContent("unknownerror").Length > 0)
            {
                writer.Write("<p align=\"left\"><b><font color=\"#FF0000\">There was an error saving the account profile. Please try again.</font></b></p>");
            }
            if (CommonLogic.QueryStringCanBeDangerousContent("badlogin").Length > 0)
            {
                writer.Write("<p align=\"left\"><b><font color=\"#FF0000\">That e-mail/password combination is already taken. Please enter a different password.</font></b></p>");
            }
            if (CommonLogic.QueryStringCanBeDangerousContent("msg").Length > 0)
            {
                writer.Write("<p align=\"left\"><b><font color=\"blue\">" + Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("msg")) + "</font></b></p>");
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("errormsg").Length > 0)
            {
                writer.Write("<p align=\"left\"><b><font color=\"red\">" + Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg")) + "</font></b></p>");
            }

            writer.Write("<script type=\"text/javascript\">\n");
            writer.Write("function CustAccountForm_Validator(theForm)\n");
            writer.Write("{\n");
            writer.Write("  submitonce(theForm);\n");
            writer.Write("  if (theForm.FirstName.value == \"\" && theForm.LastName.value == \"\")\n");
            writer.Write("  {\n");
            writer.Write("    alert(\"You must enter at least one of First Name or Last Name.\");\n");
            writer.Write("    theForm.FirstName.focus();\n");
            writer.Write("    submitenabled(theForm);\n");
            writer.Write("    return (false);\n");
            writer.Write("  }\n");
            writer.Write("  return (true);\n");
            writer.Write("}\n");
            writer.Write("</script>\n");

            writer.Write("  <!-- calendar stylesheet -->\n");
            writer.Write("  <link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"jscalendar/calendar-win2k-cold-1.css\" title=\"win2k-cold-1\" />\n");
            writer.Write("\n");
            writer.Write("  <!-- main calendar program -->\n");
            writer.Write("  <script type=\"text/javascript\" src=\"jscalendar/calendar.js\"></script>\n");
            writer.Write("\n");
            writer.Write("  <!-- language for the calendar -->\n");
            writer.Write("  <script type=\"text/javascript\" src=\"jscalendar/lang/" + Localization.JSCalendarLanguageFile() + "\"></script>\n");
            writer.Write("\n");
            writer.Write("  <!-- the following script defines the Calendar.setup helper function, which makes\n");
            writer.Write("       adding a calendar a matter of 1 or 2 lines of code. -->\n");
            writer.Write("  <script type=\"text/javascript\" src=\"jscalendar/calendar-setup.js\"></script>\n");

            String act = String.Format("cst_account_process.aspx?CustomerID={0}", TargetCustomer.CustomerID);
            writer.Write("<form method=\"POST\" action=\"" + act + "\" name=\"CustAccountForm\" id=\"CustAccountForm\")\" onSubmit=\"return (validateForm(this) && CustAccountForm_Validator(this))\">\n");

            // ACCOUNT BOX:
            writer.Write("<table width=\"100%\" cellpadding=\"2\" cellspacing=\"0\" border=\"0\" style=\"border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor") + "\">\n");
            writer.Write("<tr><td align=\"left\" valign=\"top\">\n");
            writer.Write("<img src=\"" + AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/accountinfo.gif") + "\" border=\"0\"><br/>");
            writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"0\" style=\"" + AppLogic.AppConfig("BoxFrameStyle") + "\">\n");
            writer.Write("<tr><td align=\"left\" valign=\"top\">\n");

            writer.Write("    <table border=\"0\" cellpadding=\"0\" cellspacing=\"2\" width=\"100%\">");
            writer.Write("      <tr>");
            writer.Write("        <td width=\"100%\" colspan=\"2\"><b>The Account &amp; Contact Information is used to login to the site. Please save the password in a safe place.</b></td>");
            writer.Write("      </tr>");
            writer.Write("      <tr>");
            writer.Write("        <td width=\"100%\" colspan=\"2\">");
            writer.Write("          <hr/>");
            writer.Write("        </td>");
            writer.Write("      </tr>");
            writer.Write("      <tr>");
            writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">Customer ID:</td>");
            writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
            writer.Write(TargetCustomer.CustomerID.ToString());
            writer.Write("        </td>");
            writer.Write("      </tr>");
            writer.Write("      <tr>");
            writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">Customer GUID:</td>");
            writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
            writer.Write(TargetCustomer.CustomerGUID);
            writer.Write("        </td>");
            writer.Write("      </tr>");
            writer.Write("      <tr>");
            writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">IP Address:</td>");
            writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
            if (AppLogic.IPIsRestricted(TargetCustomer.LastIPAddress))
            {
                writer.Write("<font color=\"red\"><b>");
                writer.Write(TargetCustomer.LastIPAddress);
                writer.Write("</b></font>");
                writer.Write("&nbsp;&nbsp;");
                writer.Write("<input type=\"button\" name=\"BlockIP\" style=\"font-size: 9px;\" value=\"Allow This IP Address\" onClick=\"self.location='cst_account.aspx?blockip=false&customerid=" + TargetCustomer.CustomerID.ToString() + "'\">");
            }
            else
            {
                writer.Write(TargetCustomer.LastIPAddress);
                writer.Write("&nbsp;&nbsp;");
                writer.Write("<input type=\"button\" name=\"BlockIP\" style=\"font-size: 9px;\" value=\"Block This IP Address\" onClick=\"self.location='cst_account.aspx?blockip=true&customerid=" + TargetCustomer.CustomerID.ToString() + "'\">");
            }
            writer.Write("        </td>");
            writer.Write("      </tr>");
            writer.Write("      <tr>");
            if (ThisCustomer.IsAdminSuperUser)
            {
                writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\"></td>");
                writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
                writer.Write("        <input type=\"button\" name=\"ClearSession\" style=\"font-size: 9px;\" value=\"Clear this User's Session\" onClick=\"self.location='cst_account.aspx?clearsession=true&customerid=" + TargetCustomer.CustomerID.ToString() + "'\">");
                writer.Write("        </td>");
                writer.Write("      </tr>");
            }
            writer.Write("      <tr>");
            writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">*First Name:</td>");
            writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
            writer.Write("        <input type=\"text\" name=\"FirstName\" size=\"20\" maxlength=\"50\" value=\"" + Server.HtmlEncode(TargetCustomer.FirstName) + "\">");
            writer.Write("        </td>");
            writer.Write("      </tr>");
            writer.Write("      <tr>");
            writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">*Last Name:</td>");
            writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
            writer.Write("        <input type=\"text\" name=\"LastName\" size=\"20\" maxlength=\"50\" value=\"" + Server.HtmlEncode(TargetCustomer.LastName) + "\">");
            writer.Write("        </td>");
            writer.Write("      </tr>");
            writer.Write("      <tr>");
            writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">*Locale Setting:</td>");
            writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");

            StringBuilder tmpS = new StringBuilder(4096);
            DataSet ds = DB.GetDS("select * from LocaleSetting  " + DB.GetNoLock() + " order by displayorder,description", AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
            if (ds.Tables[0].Rows.Count > 0)
            {
                tmpS.Append("<!-- COUNTRY SELECT LIST -->\n");
                tmpS.Append("<select size=\"1\" id=\"LocaleSetting\" name=\"LocaleSetting\">");
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    tmpS.Append("<option value=\"" + DB.RowField(row, "Name") + "\" " + CommonLogic.IIF(TargetCustomer.LocaleSetting == DB.RowField(row, "Name"), " selected ", "") + ">" + DB.RowField(row, "Name") + "</option>");
                }
                tmpS.Append("</select>");
                tmpS.Append("<!-- END COUNTRY SELECT LIST -->\n");
            }
            ds.Dispose();
            writer.Write(tmpS);

            writer.Write("        </td>");
            writer.Write("      </tr>");
            writer.Write("      <tr>");
            writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">*E-Mail:</td>");
            writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
            String EMailField = TargetCustomer.EMail;
            writer.Write("        <input type=\"text\" name=\"EMail\" size=\"37\" maxlength=\"100\" value=\"" + Server.HtmlEncode(EMailField) + "\">");
            //writer.Write("        <input type=\"hidden\" name=\"EMail_vldt\" value=\"[req][EMail][blankalert=Please enter an e-mail address][invalidalert=Please enter a valid e-mail address]\">");
            writer.Write("        </td>");
            writer.Write("      </tr>");

            if ((ThisCustomer.IsAdminSuperUser && (TargetCustomer.IsAdminUser || TargetCustomer.IsAdminSuperUser)) || (!TargetCustomer.IsAdminUser))
            {
                writer.Write("      <tr>");
                writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">Reset Password:</td>");
                writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
                writer.Write("             <input type=\"Button\" onClick=\"ResetPasswordConfirm(" + TargetCustomer.CustomerID.ToString() + ")\" style=\"font-size: 9px;\" Value=\"Reset To Random Password\">");
                bool AllowedToForce = false;
                if (ThisCustomer.IsAdminSuperUser)
                {
                    AllowedToForce = !TargetCustomer.IsAdminSuperUser;
                }
                else
                {
                    AllowedToForce = !TargetCustomer.IsAdminUser && !TargetCustomer.IsAdminSuperUser;
                }
                if (AllowedToForce)
                {
                    writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"text\" size=\"15\" name=\"ForcePassword\">&nbsp;<input type=\"submit\" value=\"Forcefully Set New Password\" style=\"font-size: 9px;\" name=\"ForcePasswordButton\">");
                }
                writer.Write("        </td>");
                writer.Write("      </tr>");
            }
            writer.Write("      <tr>");
            writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">Account Locked:</td>");
            writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
            writer.Write("        <input type=\"checkbox\" name=\"locked\" " + CommonLogic.IIF(TargetCustomer.Active, "", "checked") + ">");
            writer.Write("        &nbsp;&nbsp;&nbsp;&nbsp;<input type=\"Button\" onClick=\"ForceLogout(" + TargetCustomer.CustomerID.ToString() + ")\" style=\"font-size: 9px;\" Value=\"Force Logout\">");
            writer.Write("        </td>");
            writer.Write("      </tr>");

            writer.Write("      <tr>");
            writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">Over 13:</td>");
            writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
            writer.Write("        <input type=\"checkbox\" name=\"Over13\" " + CommonLogic.IIF(TargetCustomer.IsOver13, "checked", "") + ">");
            writer.Write("        </td>");
            writer.Write("      </tr>");

            if (TargetCustomer.IsAdminUser && ThisCustomer.IsAdminSuperUser)
            {
                writer.Write("      <tr>");
                writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">Can View Credit Card #s:</td>");
                writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
                writer.Write("        <input type=\"checkbox\" name=\"canviewcc\" " + CommonLogic.IIF(TargetCustomer.AdminCanViewCC, "checked", "") + ">");
                writer.Write("        </td>");
                writer.Write("      </tr>");
            }

            writer.Write("      <tr>");
            writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">Subscription Expires On:</td>");
            writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
            writer.Write("        <input type=\"text\" name=\"SubscriptionExpiresOn\" size=\"12\" value=\"" + CommonLogic.IIF(TargetCustomer.SubscriptionExpiresOn != System.DateTime.MinValue, Localization.ToNativeShortDateString(TargetCustomer.SubscriptionExpiresOn), "") + "\">&nbsp;<img src=\"" + AppLogic.LocateImageURL("../skins/skin_" + SkinID.ToString() + "/images/calendar.gif") + "\" style=\"cursor:hand;cursor:pointer;\" align=\"absmiddle\" id=\"f_trigger_s\">");
            writer.Write("			<input type=\"hidden\" name=\"SubscriptionExpiresOn_vldt\" value=\"[date][invalidalert=Please enter a valid date in the format " + Localization.ShortDateFormat() + "]\">");
            writer.Write(" (in format " + Localization.ShortDateFormat() + ")");
            writer.Write("        </td>");
            writer.Write("      </tr>");

            writer.Write("      <tr>");
            writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">Phone:</td>");
            writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
            writer.Write("        <input type=\"text\" name=\"Phone\" size=\"14\" maxlength=\"20\" value=\"" + Server.HtmlEncode(TargetCustomer.Phone) + "\">");
            writer.Write("        </td>");
            writer.Write("      </tr>");

            if (AppLogic.AppConfigBool("VAT.Enabled"))
            {
                writer.Write("      <tr>");
                writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">VAT Registration ID:</td>");
                writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
                writer.Write("        <input type=\"text\" name=\"VATRegistrationID\" size=\"14\" maxlength=\"20\" value=\"" + Server.HtmlEncode(TargetCustomer.VATRegistrationID) + "\">");
                writer.Write("        </td>");
                writer.Write("      </tr>");
            }

            writer.Write("      <tr>");
            writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">Special Offer/Coupon Code:</td>");
            writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
            writer.Write("        <input type=\"text\" name=\"CouponCode\" size=\"14\" maxlength=\"20\" value=\"" + Server.HtmlEncode(TargetCustomer.CouponCode) + "\"> (If you received a special offer code, enter it here)");
            writer.Write("        </td>");
            writer.Write("      </tr>");

            writer.Write("      <tr>");
            writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">Date Of Birth:</td>");
            writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
            writer.Write("        <input type=\"text\" name=\"DateOfBirth\" size=\"14\" maxlength=\"20\" value=\"" + CommonLogic.IIF(TargetCustomer.DateOfBirth != System.DateTime.MinValue, Localization.ToNativeShortDateString(TargetCustomer.DateOfBirth), "") + "\"> (in format " + Localization.ShortDateFormat() + ")");
            //writer.Write("        <input type=\"hidden\" name=\"DateOfBirth_vldt\" value=\"[date][blankalert=Please enter a date of birth][invalidalert=Please enter a valid date of birth in the format " + Localization.ShortDateFormat() + "]\">");
            writer.Write("        </td>");
            writer.Write("      </tr>");
            //V3_9
            if (AppLogic.MicropayIsEnabled())
            {
                writer.Write("      <tr>");
                writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">" + AppLogic.GetString("account.aspx.11", SkinID, ThisCustomer.LocaleSetting) + " Balance:</td>");
                writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
                writer.Write("        <input type=\"text\" name=\"MicroPayBalance\" size=\"14\" maxlength=\"20\" value=\"" + CommonLogic.IIF(TargetCustomer.MicroPayBalance != System.Decimal.Zero, Localization.CurrencyStringForGatewayWithoutExchangeRate(TargetCustomer.MicroPayBalance), "") + "\">");
                writer.Write("        <input type=\"hidden\" name=\"MicroPayBalance_vldt\" value=\"[blankalert=Please enter a valid number][invalidalert=Please enter a valid number]\">");
                writer.Write("        </td>");
                writer.Write("      </tr>");
            }
            writer.Write("      <tr>");
            writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">OK To EMail:</td>");
            writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
            writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"OKToEMail\" value=\"1\" " + CommonLogic.IIF(TargetCustomer.OKToEMail, " checked ", "") + ">\n");
            writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"OKToEMail\" value=\"0\" " + CommonLogic.IIF(TargetCustomer.OKToEMail, "", " checked ") + ">\n");
            writer.Write("<small>(Can we contact you with product updates/information, etc...)</small>");
            writer.Write("        </td>");
            writer.Write("      </tr>");

            writer.Write("      <tr>");
            writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">C.O.D. Company Check Allowed:</td>");
            writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
            writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"CODCompanyCheckAllowed\" value=\"1\" " + CommonLogic.IIF(TargetCustomer.CODCompanyCheckAllowed, " checked ", "") + ">\n");
            writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"CODCompanyCheckAllowed\" value=\"0\" " + CommonLogic.IIF(TargetCustomer.CODCompanyCheckAllowed, "", " checked ") + ">\n");
            writer.Write("        </td>");
            writer.Write("      </tr>");

            writer.Write("      <tr>");
            writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">C.O.D. Net 30 Allowed:</td>");
            writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
            writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"CODNet30Allowed\" value=\"1\" " + CommonLogic.IIF(TargetCustomer.CODNet30Allowed, " checked ", "") + ">\n");
            writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"CODNet30Allowed\" value=\"0\" " + CommonLogic.IIF(TargetCustomer.CODNet30Allowed, "", " checked ") + ">\n");
            writer.Write("        </td>");
            writer.Write("      </tr>");

            ds = DB.GetDS("select * from Affiliate " + DB.GetNoLock() + " where deleted=0 order by Name", AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
            if (ds.Tables[0].Rows.Count > 0)
            {
                writer.Write("      <tr>");
                writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">Customer's Affiliate:</td>");
                writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
                tmpS.Length = 0;
                tmpS.Append("<!-- AFFILIATE SELECT LIST -->\n");
                tmpS.Append("<select size=\"1\" id=\"AffiliateID\" name=\"AffiliateID\">");
                tmpS.Append("<option value=\"0\">None (ID=0)</option>");
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    String aName = DB.RowField(row, "Name");
                    tmpS.Append("<option value=\"" + DB.RowField(row, "AffiliateID") + "\" " + CommonLogic.IIF(TargetCustomer.AffiliateID == DB.RowFieldInt(row, "AffiliateID"), " selected ", "") + ">" + aName + " (ID=" + DB.RowFieldInt(row, "AffiliateID").ToString() + ")</option>");
                }
                tmpS.Append("</select>");
                tmpS.Append("<!-- END AFFILIATE SELECT LIST -->\n");
                writer.Write(tmpS);
                writer.Write("        </td>");
                writer.Write("      </tr>");
            }
            ds.Dispose();

            ds = DB.GetDS("select * from CustomerLevel " + DB.GetNoLock() + " order by DisplayOrder,Name", AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
            if (ds.Tables[0].Rows.Count > 0)
            {
                writer.Write("      <tr>");
                writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">Customer's Level:</td>");
                writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
                tmpS.Length = 0;
                tmpS.Append("<!-- CUSTOMER LEVEL SELECT LIST -->\n");
                tmpS.Append("<select size=\"1\" id=\"CustomerLevelID\" name=\"CustomerLevelID\">");
                tmpS.Append("<option value=\"0\">None (ID=0)</option>");
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    tmpS.Append("<option value=\"" + DB.RowField(row, "CustomerLevelID") + "\" " + CommonLogic.IIF(TargetCustomer.CustomerLevelID == DB.RowFieldInt(row, "CustomerLevelID"), " selected ", "") + ">" + DB.RowField(row, "Name") + " (ID=" + DB.RowFieldInt(row, "CustomerLevelID").ToString() + ")</option>");
                }
                tmpS.Append("</select>");
                tmpS.Append("<!-- END CUSTOMER LEVEL SELECT LIST -->\n");
                ds.Dispose();
                writer.Write(tmpS);
                writer.Write("        </td>");
                writer.Write("      </tr>");
            }
            ds.Dispose();

            writer.Write("        <tr>");
            writer.Write("            <td width=\"25%\" align=\"right\" valign=\"top\">Notes:</td>");
            writer.Write("            <td width=\"75%\" align=\"left\" valign=\"top\"> <textarea rows=\"5\" style=\"width: 100%;\" name=\"CustomerNotes\">" + Server.HtmlEncode(TargetCustomer.Notes) + "</textarea></td>");
            writer.Write("        </tr>");
            writer.Write("</table>\n");

            writer.Write("</td></tr>\n");
            writer.Write("</table>\n");
            writer.Write("</td></tr>\n");
            writer.Write("</table>\n");

            writer.Write("</table>\n");

            writer.Write("</td></tr>\n");
            writer.Write("</table>\n");
            writer.Write("</td></tr>\n");
            writer.Write("</table>\n");

            // BILLING BOX:

            writer.Write("<table width=\"100%\" cellpadding=\"2\" cellspacing=\"0\" border=\"0\" style=\"border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor") + "\">\n");
            writer.Write("<tr><td align=\"left\" valign=\"top\">\n");
            if (AllowShipToDifferentThanBillTo)
            {
                writer.Write("<img src=\"" + AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/billinginfo.gif") + "\" border=\"0\"><br/>");
            }
            else
            {
                writer.Write("<img src=\"" + AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/shippingandbillinginfo.gif") + "\" border=\"0\"><br/>");
            }
            writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"0\" style=\"" + AppLogic.AppConfig("BoxFrameStyle") + "\">\n");
            writer.Write("<tr><td align=\"left\" valign=\"top\">\n");

            Address BillingAddress = new Address();
            Address ShippingAddress = new Address();

            BillingAddress.LoadByCustomer(TargetCustomer.CustomerID, TargetCustomer.PrimaryBillingAddressID, AddressTypes.Billing);
            ShippingAddress.LoadByCustomer(TargetCustomer.CustomerID, TargetCustomer.PrimaryShippingAddressID, AddressTypes.Shipping);

            writer.Write("<table width=\"100%\" border=\"0\">\n");
            writer.Write("<tr>\n");
            writer.Write("<td colspan=\"3\"><b>Verify that your default Billing and Shipping addresses match your Payment information.</b><br/><hr/></td>");
            writer.Write("</tr>\n");
            writer.Write("<tr>\n");
            writer.Write("<td width=\"33%\" valign=\"top\"><b>");

            writer.Write(String.Format("Bill To Address:&nbsp;&nbsp;&nbsp;&nbsp;</b><img src=\"" + AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/change.gif") + "\" border=\"0\" align=\"absmiddle\" style=\"cursor:hand;cursor:pointer;\" onClick=\"self.location='cst_selectaddress.aspx?CustomerID={0}&AddressType=Billing&ReturnUrl={1}'\">\n<br/>\n", TargetCustomer.CustomerID, CommonLogic.GetThisPageName(true) + "?" + CommonLogic.ServerVariables("QUERY_STRING")));
            writer.Write(BillingAddress.DisplayHTML(false));
            if (ThisCustomer.AdminCanViewCC)
            {
                if (BillingAddress.PaymentMethodLastUsed.Length != 0)
                {
                    writer.Write("<b>Payment Method:</b><br/>");
                    writer.Write(BillingAddress.DisplayPaymentMethodInfo(ThisCustomer, BillingAddress.PaymentMethodLastUsed));
                }
            }
            writer.Write("</td>");

            writer.Write("<td valign=\"top\">");
            writer.Write(String.Format("<b>Ship To Address:&nbsp;&nbsp;&nbsp;&nbsp;</b><img style=\"cursor:hand;cursor:pointer;\" src=\"" + AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/change.gif") + "\" border=\"0\" align=\"absmiddle\" onClick=\"self.location='cst_selectaddress.aspx?CustomerID={0}&AddressType=Shipping&ReturnUrl={1}'\"><br/>\n", TargetCustomer.CustomerID, CommonLogic.GetThisPageName(true) + "?" + CommonLogic.ServerVariables("QUERY_STRING")));
            writer.Write(ShippingAddress.DisplayHTML(false));
            writer.Write("</td>");
            writer.Write("<td width=\"33%\"></td>");
            writer.Write("</tr></table>");

            writer.Write("</td></tr>\n");
            writer.Write("</table>\n");
            writer.Write("</td></tr>\n");
            writer.Write("</table>\n");
            writer.Write("<input type=\"hidden\" name=\"OriginalEMail\" value=\"" + TargetCustomer.EMail + "\">");
            writer.Write("<p align=\"center\"><input type=\"submit\" value=\"Update Account Information\" name=\"Continue\"></p>");
            writer.Write("</form>");

            writer.Write("\n<script type=\"text/javascript\">\n");

            writer.Write("function ResetPasswordConfirm(id)\n");
            writer.Write("{\n");
            writer.Write("  if(confirm('Are you sure you want to reset the password for this customer?'))\n");
            writer.Write("  {\n");
            writer.Write("      self.location = 'cst_account_process.aspx?resetpwd=' + id;\n");
            writer.Write("  }\n");
            writer.Write("}\n");

            writer.Write("function ForceLogout(id)\n");
            writer.Write("{\n");
            writer.Write("  if(confirm('Are you sure you want to force a logout for this customer?'))\n");
            writer.Write("  {\n");
            writer.Write("      self.location = 'cst_account_process.aspx?forcelogout=' + id;\n");
            writer.Write("  }\n");
            writer.Write("}\n");

            writer.Write("    Calendar.setup({\n");
            writer.Write("        inputField     :    \"SubscriptionExpiresOn\",      // id of the input field\n");
            writer.Write("        ifFormat       :    \"" + Localization.JSCalendarDateFormatSpec() + "\",       // format of the input field\n");
            writer.Write("        showsTime      :    false,            // will display a time selector\n");
            writer.Write("        button         :    \"f_trigger_s\",   // trigger for the calendar (button ID)\n");
            writer.Write("        singleClick    :    true            // Single-click mode\n");
            writer.Write("    });\n");
            writer.Write("</script>\n");

            //rs.Close();
        }

    }
}
