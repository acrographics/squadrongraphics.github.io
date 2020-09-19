// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/cst_editaddress.aspx.cs 13    9/30/06 3:39p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for cst_editaddress.
    /// </summary>
    public partial class cst_editaddress : AspDotNetStorefront.SkinBase
    {

        private Customer TargetCustomer;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            TargetCustomer = new Customer(CommonLogic.QueryStringUSInt("CustomerID"), true);
            if (TargetCustomer.CustomerID == 0)
            {
                Response.Redirect("Customers.aspx");
            }
            if (TargetCustomer.IsAdminSuperUser && !ThisCustomer.IsAdminSuperUser)
            {
                throw new ArgumentException("Security Exception. Not Allowed");
            } 
            SectionTitle = "<a href=\"Customers.aspx?searchfor=" + TargetCustomer.CustomerID.ToString() + "\">Customers</a> - Account Info: <a href=\"cst_account.aspx?customerid=" + TargetCustomer.CustomerID.ToString() + "\">" + TargetCustomer.FullName() + " (" + TargetCustomer.EMail + ")</a>";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {

            bool AllowShipToDifferentThanBillTo = AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo") && !AppLogic.AppConfigBool("SkipShippingOnCheckout");
            String TabImage = AppLogic.LocateImageURL("editaddress.gif");
            int AddressID = CommonLogic.QueryStringUSInt("AddressID");

            string ReturnUrl = CommonLogic.QueryStringCanBeDangerousContent("ReturnUrl");

            AddressTypes AddressType = AddressTypes.Unknown;
            String AddressTypeString = CommonLogic.QueryStringCanBeDangerousContent("AddressType");
            AddressType = (AddressTypes)Enum.Parse(typeof(AddressTypes), AddressTypeString, true);

            writer.Write("\n<script type=\"text/javascript\">\n");
            writer.Write("function ShowPaymentInput(theRadio)\n");
            writer.Write("{\n");
            writer.Write("  if (theRadio.value == 'NONE')\n");
            writer.Write("  {\n");
            writer.Write("    document.getElementById('divCheck').style.display = 'none';\n");
            writer.Write("    document.getElementById('divCard').style.display = 'none';\n");
            writer.Write("  }\n");
            writer.Write("  else if (theRadio.value == 'ECHECK')\n");
            writer.Write("  {\n");
            writer.Write("    document.getElementById('divCheck').style.display = '';\n");
            writer.Write("    document.getElementById('divCard').style.display = 'none';\n");
            writer.Write("  }\n");
            writer.Write("  else\n");
            writer.Write("  {\n");
            writer.Write("    document.getElementById('divCheck').style.display = 'none';\n");
            writer.Write("    document.getElementById('divCard').style.display = '';\n");
            writer.Write("  }\n");
            writer.Write("  return true;\n");
            writer.Write("}\n");
            writer.Write("</script>\n");

            // ACCOUNT BOX:
            writer.Write("<table width=\"100%\" cellpadding=\"2\" cellspacing=\"0\" border=\"0\" style=\"border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor") + "\">\n");
            writer.Write("<tr><td align=\"left\" valign=\"top\">\n");
            writer.Write("<img src=\"skins/Skin_" + SkinID.ToString() + "/images/" + TabImage + "\" border=\"0\"><br/>");
            writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"0\" style=\"" + AppLogic.AppConfig("BoxFrameStyle") + "\">\n");
            writer.Write("<tr><td align=\"left\" valign=\"top\">\n");

            // ADDRESS BOX:

            Address theAddress = new Address(TargetCustomer.CustomerID);
            theAddress.LoadFromDB(AddressID);

            String act = String.Format("cst_editaddress_process.aspx?CustomerID={0}&AddressType={1}&AddressID={2}&ReturnUrl={3}", TargetCustomer.CustomerID, AddressType, AddressID, ReturnUrl);
            writer.Write("<form method=\"POST\" action=\"" + act + "\" name=\"EditAddressForm\" id=\"EditAddressForm\" onSubmit=\"return (validateForm(this))\">\n");
            writer.Write("<input type=\"hidden\" id=\"DeleteAddressID\" name=\"DeleteAddressID\" value=\"0\" >\n");

            writer.Write(String.Format("<b>You are Editing the {0} Address below.</b><hr/>\n", AddressType));

            //Display the Address input form fields
            writer.Write(theAddress.InputHTML());

            if (AddressType == AddressTypes.Billing && ThisCustomer.AdminCanViewCC)
            {
                if (theAddress.CardNumber.Trim().Length > 0)
                {
                    Security.LogEvent("Viewed Credit Card", "Viewed on Customer billing address edit page in admin site", TargetCustomer.CustomerID, ThisCustomer.CustomerID, ThisCustomer.CurrentSessionID);
                }
                writer.Write("<hr/>");
                writer.Write("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">");
                writer.Write("<tr><td width=\"25%\" align=\"left\" valign=\"bottom\">\n");
                writer.Write("Preferred Payment Method:</td>\n");
                writer.Write("<td align=\"left\" valign=\"bottom\">\n");
                writer.Write(String.Format("Credit Card<input type=\"radio\" id=\"CCPaymentMethod\" name=\"PaymentMethod\" value=\"Credit Card\" onClick=\"ShowPaymentInput(this)\" {0}>", CommonLogic.IIF(AppLogic.CleanPaymentMethod(theAddress.PaymentMethodLastUsed) == AppLogic.ro_PMCreditCard, "checked", "")));
                writer.Write(String.Format(" ECheck<input type=\"radio\" id=\"ECheckPaymentMethod\" name=\"PaymentMethod\" value=\"ECHECK\" onClick=\"ShowPaymentInput(this)\" {0}>", CommonLogic.IIF(AppLogic.CleanPaymentMethod(theAddress.PaymentMethodLastUsed) == AppLogic.ro_PMECheck, "checked", "")));
                writer.Write("</td></tr>\n");
                writer.Write("</table>");

                writer.Write(String.Format("<div id=\"divCard\" name-\"divCard\" style=\"display:{0}\">", CommonLogic.IIF(AppLogic.CleanPaymentMethod(theAddress.PaymentMethodLastUsed) == AppLogic.ro_PMCreditCard, "", "none")));
                writer.Write("<p><hr/><b>Enter Credit Card Account information</b><hr/></p>");
                writer.Write(theAddress.InputCardHTML(TargetCustomer, false, false));
                writer.Write("</div>");

                writer.Write(String.Format("<div id=\"divCheck\" name=\"divCheck\" style=\"display:{0}\">", CommonLogic.IIF(AppLogic.CleanPaymentMethod(theAddress.PaymentMethodLastUsed) == AppLogic.ro_PMECheck, "", "none")));
                writer.Write("<p><hr/><b>Enter Checking Account information</b><hr/></p>");
                writer.Write(theAddress.InputECheckHTML(false));
                writer.Write("</div>");
            }

            bool CanDelete = (0 == DB.GetSqlN(String.Format("select count(0) as N from ShoppingCart  " + DB.GetNoLock() + " where (ShippingAddressID={0} or BillingAddressID={0}) and CartType={1}", AddressID, (int)CartTypeEnum.RecurringCart)));

            writer.Write("</td></tr>\n");
            writer.Write("</table>\n");
            //Button to submit the form
            writer.Write("<p align=\"center\"><input type=\"submit\" value=\"Save Edited Address\" name=\"Continue\"></p>\n");
            if (CanDelete)
            {
                writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"submit\" value=\"Delete This Address\" name=\"Delete\" onClick=\"if (confirm('Do you want to delete this address permanently?')) {EditAddressForm.DeleteAddressID.value=" + AddressID.ToString() + ";} \"></p>");
            }
            else
            {
                writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"button\" value=\"Delete This Address\" name=\"Delete\" onClick=\"alert('You may not delete this address because it is being used by an Auto-Ship order. You will need to remove it from all Auto-Ship orders before it may be deleted.')  \"></p>");
            }
            writer.Write("</form>\n");
            writer.Write("</td></tr>\n");
            writer.Write("</table>\n");

        }

    }
}
