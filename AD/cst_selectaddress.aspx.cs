// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/cst_selectaddress.aspx.cs 9     9/27/06 4:19p Redwoodtree $
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
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for cst_selectaddress.
    /// </summary>
    public partial class cst_selectaddress : AspDotNetStorefront.SkinBase
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

            String TabImage = String.Empty;

            AddressTypes AddressType = AddressTypes.Unknown;
            String AddressTypeString = CommonLogic.QueryStringCanBeDangerousContent("AddressType");

            AddressType = (AddressTypes)Enum.Parse(typeof(AddressTypes), AddressTypeString, true);

            if (AddressType == AddressTypes.Billing)
            {
                TabImage = AppLogic.LocateImageURL("selectbillingaddress.gif");
            }
            if (AddressType == AddressTypes.Shipping)
            {
                TabImage = AppLogic.LocateImageURL("selectshippingaddress.gif");
            }

            int OriginalRecurringOrderNumber = CommonLogic.QueryStringUSInt("OriginalRecurringOrderNumber");
            string ReturnUrl = CommonLogic.QueryStringCanBeDangerousContent("ReturnUrl");

            // ACCOUNT BOX:
            writer.Write("<table width=\"100%\" cellpadding=\"2\" cellspacing=\"0\" border=\"0\" style=\"border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor") + "\">\n");
            writer.Write("<tr><td align=\"left\" valign=\"top\">\n");
            writer.Write("<img src=\"skins/Skin_" + SkinID.ToString() + "/images/" + TabImage + "\" border=\"0\"><br/>");
            writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"0\" style=\"" + AppLogic.AppConfig("BoxFrameStyle") + "\">\n");
            writer.Write("<tr><td align=\"left\" valign=\"top\">\n");

            writer.Write("</td><tr><td>\n");
            writer.Write("<table width=\"100%\" border=\"0\">");
            writer.Write("<tr>");

            Addresses custAddresses = new Addresses();
            custAddresses.LoadCustomer(TargetCustomer.CustomerID);
            int pos = 0;
            foreach (Address adr in custAddresses)
            {
                writer.Write("<td align=\"left\" valign=\"top\">\n");
                writer.Write(String.Format("<img style=\"cursor:hand;cursor:pointer;\" src=\"" + AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/usethisaddress.gif") + "\" onClick=\"self.location='cst_selectaddress_process.aspx?CustomerID={0}&AddressType={1}&AddressID={2}&OriginalRecurringOrderNumber={3}&ReturnUrl={4}'\"><br/>", TargetCustomer.CustomerID, AddressType, adr.AddressID, OriginalRecurringOrderNumber, ReturnUrl));
                writer.Write(adr.DisplayHTML(false));
                if (adr.CardNumber.Length != 0)
                {
                    writer.Write(adr.DisplayCardHTML());
                }
                writer.Write(String.Format("<img style=\"cursor:hand;cursor:pointer;\" src=\"" + AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/edit2.gif") + "\" onClick=\"self.location='cst_editaddress.aspx?CustomerID={0}&AddressType={1}&AddressID={2}&ReturnUrl={3}'\"><br/><br/>", TargetCustomer.CustomerID, AddressType, adr.AddressID, ReturnUrl));
                writer.Write("</td>");
                pos++;
                if ((pos % 2) == 0)
                {
                    writer.Write("</tr><tr>");
                }
            }

            writer.Write("</tr></table>");

            writer.Write("</td></tr>");
            writer.Write("<tr><td align=\"left\" valign=\"top\">\n");

            // ADDRESS BOX:

            Address newAddress = new Address();
            newAddress.AddressType = AddressType;

            String act = String.Format("cst_selectaddress_process.aspx?CustomerID={0}&AddressType={1}&ReturnUrl={2}", TargetCustomer.CustomerID, AddressType, ReturnUrl);
            if (OriginalRecurringOrderNumber != 0)
            {
                act += String.Format("OriginalRecurringOrderNumber={0}&", OriginalRecurringOrderNumber);
            }

            writer.Write("<form method=\"POST\" action=\"" + act + "\" name=\"SelectAddressForm\" id=\"SelectAddressForm\" onSubmit=\"return (validateForm(this))\">");

            writer.Write(String.Format("<hr/><b>Or Enter a New {0} Address</b><hr/>", AddressType));

            //Display the Address input form fields
            writer.Write(newAddress.InputHTML());

            //Button to submit the form
            writer.Write("<p align=\"center\"><input type=\"submit\" value=\"Add New Address\" name=\"Continue\"></p>");

            writer.Write("</td></tr>\n");
            writer.Write("</table>\n");
            writer.Write("</form>");
            writer.Write("</td></tr>\n");
            writer.Write("</table>\n");
            writer.Write("</td></tr>\n");
            writer.Write("</table>\n");
        }

    }
}
