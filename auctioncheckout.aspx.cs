// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Text;
using System.Web.Security;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for auctioncheckout.
    /// </summary>
    public partial class auctioncheckout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            if(AppLogic.OnLiveServer() && CommonLogic.ServerVariables("SERVER_PORT_SECURE") != "1")
            {
                // MUST be https on a production site:
                ErrorMsg.Text = "HTTPS Protocol required";
                return;
            }

            String AuctionID = CommonLogic.QueryStringCanBeDangerousContent("AuctionID");
            int Step = CommonLogic.QueryStringUSInt("Step");

            String url = "auctioncheckout.aspx?auctionid=" + AuctionID + "&step=2";

            if (Step != 2)
            {
                Response.Redirect("signout.aspx?RedirectURL=" + Server.UrlEncode(url));
            }
            else
            {
                if (AuctionID.Length == 0)
                {
                    ErrorMsg.Text = "NO Auction ID Found";
                    return;
                }
                Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
                
                String CustomerGUID = Security.UnmungeString(AuctionID);
                if (CustomerGUID.ToLowerInvariant().IndexOf("error") != -1)
                {
                    CustomerGUID = AuctionID;
                }

                // we cannot have any customer logged in at this point, they should be fully anon at step 2:
                if (ThisCustomer.CustomerID != 0)
                {
                    ErrorMsg.Text = "NO Auction Customer Found";
                    return;
                }

                System.Nullable<Guid> GD = null;
                try
                {
                    GD = new Guid(CustomerGUID);
                }
                catch { }
                if (GD == null)
                {
                    ErrorMsg.Text = "NO Auction Customer GUID Found";
                    return;
                }

                Customer AuctionCustomer = new Customer((Guid)GD, true);
                if (AuctionCustomer.CustomerID == 0)
                {
                    ErrorMsg.Text = "NO Auction Customer ID Found";
                    return;
                }

                if (AuctionCustomer.LockedUntil > System.DateTime.Now)
                {
                    ErrorMsg.Text = "NO Auction ID Found";
                    return;
                }

                if (AuctionCustomer.PwdChangeRequired)
                {
                    ErrorMsg.Text = "Customer Account Requires Password Change";
                    return;
                }

                if (AppLogic.IPIsRestricted(AuctionCustomer.LastIPAddress))
                {
                    ErrorMsg.Text = "Customer Account Locked";
                    return;
                }

                // ok, we have a clear customer...mark them as the customer that owns this page.
                int CurrentCustomerID = ThisCustomer.CustomerID;
                int NewCustomerID = AuctionCustomer.CustomerID;

                if (AuctionCustomer.IsAdminUser)
                {
                    Security.LogEvent("Store Login", "", AuctionCustomer.CustomerID, AuctionCustomer.CustomerID, AuctionCustomer.ThisCustomerSession.SessionID);
                }

                object lockeduntil = DateTime.Now.AddMinutes(-1);
                AuctionCustomer.UpdateCustomer(
                    /*CustomerLevelID*/ null,
                    /*EMail*/ null,
                    /*SaltedAndHashedPassword*/ null,
                    /*SaltKey*/ null,
                    /*DateOfBirth*/ null,
                    /*Gender*/ null,
                    /*FirstName*/ null,
                    /*LastName*/ null,
                    /*Notes*/ null,
                    /*SkinID*/ null,
                    /*Phone*/ null,
                    /*AffiliateID*/ null,
                    /*Referrer*/ null,
                    /*CouponCode*/ null,
                    /*OkToEmail*/ null,
                    /*IsAdmin*/ null,
                    /*BillingEqualsShipping*/ null,
                    /*LastIPAddress*/ null,
                    /*OrderNotes*/ null,
                    /*SubscriptionExpiresOn*/ null,
                    /*RTShipRequest*/ null,
                    /*RTShipResponse*/ null,
                    /*OrderOptions*/ null,
                    /*LocaleSetting*/ null,
                    /*MicroPayBalance*/ null,
                    /*RecurringShippingMethodID*/ null,
                    /*RecurringShippingMethod*/ null,
                    /*BillingAddressID*/ null,
                    /*ShippingAddressID*/ null,
                    /*GiftRegistryGUID*/ null,
                    /*GiftRegistryIsAnonymous*/ null,
                    /*GiftRegistryAllowSearchByOthers*/ null,
                    /*GiftRegistryNickName*/ null,
                    /*GiftRegistryHideShippingAddresses*/ null,
                    /*CODCompanyCheckAllowed*/ null,
                    /*CODNet30Allowed*/ null,
                    /*ExtensionData*/ null,
                    /*FinalizationData*/ null,
                    /*Deleted*/ null,
                    /*Over13Checked*/ null,
                    /*CurrencySetting*/ null,
                    /*VATSetting*/ null,
                    /*VATRegistrationID*/ null,
                    /*StoreCCInDB*/ null,
                    /*IsRegistered*/ null,
                    /*LockedUntil*/ lockeduntil,
                    /*AdminCanViewCC*/ null,
                    /*BadLogin*/ -1,
                    /*Active*/ null,
                    /*PwdChangeRequired*/ 0,
                    /*RegisterDate*/ null
                    );

                String AuctionCustomerGUID = AuctionCustomer.CustomerGUID.Replace("{", "").Replace("}", "");

                string sReturnURL = "shoppingcart.aspx";
                FormsAuthentication.SetAuthCookie(AuctionCustomerGUID, true); // force persist login to false for security

                Response.AddHeader("REFRESH", "1; URL=" + Server.UrlDecode(sReturnURL));
            }
        }
    }
}
