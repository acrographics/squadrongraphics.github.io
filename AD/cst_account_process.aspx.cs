// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/cst_account_process.aspx.cs 33    9/22/06 11:03p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for cst_account_process.
    /// </summary>
    public partial class cst_account_process : System.Web.UI.Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
            Customer TargetCustomer = new Customer(CommonLogic.QueryStringUSInt("CustomerID"), true);

            if (TargetCustomer.IsAdminSuperUser && !ThisCustomer.IsAdminSuperUser)
            {
                throw new ArgumentException("Security Exception. Not Allowed");
            }

            if (CommonLogic.QueryStringUSInt("resetpwd") > 0)
            {
                TargetCustomer = new Customer(CommonLogic.QueryStringUSInt("resetpwd"), true);
                if ((TargetCustomer.IsAdminUser || TargetCustomer.IsAdminSuperUser) && !ThisCustomer.IsAdminSuperUser)
                {
                    Response.Redirect("cst_account.aspx?customerid=" + TargetCustomer.CustomerID.ToString() + "&errormsg=" + Server.UrlEncode("Permission Denied"));
                }
                if (AppLogic.MailServer().Length == 0 || AppLogic.MailServer() == AppLogic.ro_TBD)
                {
                    Response.Redirect("cst_account.aspx?customerid=" + TargetCustomer.CustomerID.ToString() + "&errormsg=" + Server.UrlEncode(AppLogic.GetString("cst_account_process.aspx.2", TargetCustomer.SkinID, TargetCustomer.LocaleSetting)));
                }
                else
                {
                    Password p = new Password(AspDotNetStorefrontEncrypt.Encrypt.CreateRandomStrongPassword(8));

                    try
                    {
                        AppLogic.SendMail(AppLogic.AppConfig("StoreName") + " - " + AppLogic.GetString("cst_account_process.aspx.1", TargetCustomer.SkinID, TargetCustomer.LocaleSetting), AppLogic.RunXmlPackage("notification.lostpassword.xml.config", null, TargetCustomer, TargetCustomer.SkinID, "", "thiscustomerid=" + TargetCustomer.CustomerID.ToString() + "&newpwd=" + p.ClearPassword, false, false), true, AppLogic.AppConfig("MailMe_FromAddress"), AppLogic.AppConfig("MailMe_FromName"), TargetCustomer.EMail, TargetCustomer.FullName(), "", "", AppLogic.MailServer());
                        Security.LogEvent("Admin Reset Customer Password", "", TargetCustomer.CustomerID, ThisCustomer.CustomerID, Convert.ToInt32(ThisCustomer.CurrentSessionID));

                        String vtr = CommonLogic.FormCanBeDangerousContent("VATRegistrationID");
                        if (!AppLogic.AppConfigBool("VAT.Enabled") || !AppLogic.VATRegistrationIDIsValid(TargetCustomer, vtr))
                        {
                            vtr = null;
                        }

                        object lockeduntil = DateTime.Now.AddMinutes(-1);
                        TargetCustomer.UpdateCustomer(
                            /*CustomerLevelID*/ null,
                            /*EMail*/ null,
                            /*SaltedAndHashedPassword*/ p.SaltedPassword,
                            /*SaltKey*/ p.Salt,
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
                            /*VATRegistrationID*/ vtr,
                            /*StoreCCInDB*/ null,
                            /*IsRegistered*/ null,
                            /*LockedUntil*/ lockeduntil,
                            /*AdminCanViewCC*/ null,
                            /*BadLogin*/ -1,
                            /*Active*/ null,
                            /*PwdChangeRequired*/ 1,
                            /*RegisterDate*/ null
                         );
                    }
                    catch (Exception ex)
                    {
                        Response.Redirect("cst_account.aspx?customerid=" + TargetCustomer.CustomerID.ToString() + "&errormsg=" + Server.UrlEncode(AppLogic.GetString("cst_account_process.aspx.2", TargetCustomer.SkinID, TargetCustomer.LocaleSetting) + ex.Message));
                    }
                    Response.Redirect("cst_account.aspx?customerid=" + TargetCustomer.CustomerID.ToString() + "&msg=" + Server.UrlEncode(AppLogic.GetString("cst_account_process.aspx.3", TargetCustomer.SkinID, TargetCustomer.LocaleSetting)));
                }
            }

            if (CommonLogic.FormCanBeDangerousContent("ForcePassword").Trim().Length > 0)
            {
                if ((TargetCustomer.IsAdminUser || TargetCustomer.IsAdminSuperUser) && !ThisCustomer.IsAdminSuperUser)
                {
                    Response.Redirect("cst_account.aspx?customerid=" + TargetCustomer.CustomerID.ToString() + "&errormsg=" + Server.UrlEncode("Permission Denied"));
                }

                if (TargetCustomer.IsAdminUser || TargetCustomer.IsAdminSuperUser || AppLogic.AppConfigBool("UseStrongPwd"))
                {
                    Regex re = new Regex(AppLogic.AppConfig("CustomerPwdValidator"));
                    if (!re.IsMatch(CommonLogic.FormCanBeDangerousContent("ForcePassword").Trim()))
                    {
                        Response.Redirect("cst_account.aspx?customerid=" + TargetCustomer.CustomerID.ToString() + "&errormsg=" + AppLogic.GetString("account.aspx.69", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
                    }
                }

                Password p = new Password(CommonLogic.FormCanBeDangerousContent("ForcePassword").Trim());

                try
                {
                    // NOTE: DO NOT SEND THIS FORCE PASSWORD TO THE CUSTOMER BY EMAIL. IT MUST BE TOLD TO THE CUSTOMER OVER THE PHONE AFTER
                    // PROPER USER IDENTIFICATION VALIDATION PROCEDURES PER PABP
                    //AppLogic.SendMail(AppLogic.AppConfig("StoreName") + " - " + AppLogic.GetString("cst_account_process.aspx.1", TargetCustomer.SkinID, TargetCustomer.LocaleSetting), AppLogic.RunXmlPackage("notification.lostpassword.xml.config", null, TargetCustomer, TargetCustomer.SkinID, "", "thiscustomerid=" + TargetCustomer.CustomerID.ToString() + "&newpwd=" + p.ClearPassword.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;").Replace("&", "&amp;"), false, false), true, AppLogic.AppConfig("MailMe_FromAddress"), AppLogic.AppConfig("MailMe_FromName"), TargetCustomer.EMail, TargetCustomer.FullName(), "", "", AppLogic.MailServer());
                    Security.LogEvent(AppLogic.GetString("cst_account_process.aspx.5", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), "", TargetCustomer.CustomerID, ThisCustomer.CustomerID, Convert.ToInt32(ThisCustomer.CurrentSessionID));

                    String vtr = CommonLogic.FormCanBeDangerousContent("VATRegistrationID");
                    if (!AppLogic.AppConfigBool("VAT.Enabled") || !AppLogic.VATRegistrationIDIsValid(TargetCustomer, vtr))
                    {
                        vtr = null;
                    }

                    object lockeduntil = DateTime.Now.AddMinutes(-1);
                    TargetCustomer.UpdateCustomer(
                        /*CustomerLevelID*/ null,
                        /*EMail*/ null,
                        /*SaltedAndHashedPassword*/ p.SaltedPassword,
                        /*SaltKey*/ p.Salt,
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
                        /*VATRegistrationID*/ vtr,
                        /*StoreCCInDB*/ null,
                        /*IsRegistered*/ null,
                        /*LockedUntil*/ lockeduntil,
                        /*AdminCanViewCC*/ null,
                        /*BadLogin*/ -1,
                        /*Active*/ null,
                        /*PwdChangeRequired*/ 1,
                        /*RegisterDate*/ null
                     );
                }
                catch (Exception ex)
                {
                    Response.Redirect("cst_account.aspx?customerid=" + TargetCustomer.CustomerID.ToString() + "&errormsg=" + Security.UrlEncode(AppLogic.GetString("cst_account_process.aspx.2", TargetCustomer.SkinID, TargetCustomer.LocaleSetting) + ex.Message));
                }
                Response.Redirect("cst_account.aspx?customerid=" + TargetCustomer.CustomerID.ToString() + "&msg=" + Security.UrlEncode("New Password Has Been Set"));
            }

            if (CommonLogic.QueryStringUSInt("forcelogout") > 0)
            {
                Customer customer = new Customer(CommonLogic.QueryStringUSInt("forcelogout"), true);
                DB.ExecuteSQL("update Customer set CustomerGUID=newid() where CustomerID=" + customer.CustomerID.ToString());
                Response.Redirect("cst_account.aspx?customerid=" + customer.CustomerID.ToString() + "&msg=Force+Logout+Executed");
            }

            if (TargetCustomer.CustomerID == 0)
            {
                Response.Redirect("Customers.aspx");
            }

            String EMailField = CommonLogic.FormCanBeDangerousContent("EMail").ToLowerInvariant().Trim();
            bool EMailAlreadyTaken = false;
            bool VATIsValid = true;
            if (!AppLogic.AppConfigBool("AllowCustomerDuplicateEMailAddresses"))
            {
                EMailAlreadyTaken = Customer.EmailInUse(EMailField, TargetCustomer.CustomerID);
            }

            String PaymentMethod = CommonLogic.QueryStringCanBeDangerousContent("PaymentMethod");

            // if discount code was entered, see if it's valid, If not put up error message
            String DiscountValid = String.Empty;
            if (CommonLogic.FormCanBeDangerousContent("CouponCode").Length != 0)
            {
                DiscountValid = "&discounterror=1";
                IDataReader rsdis = DB.GetRS("select * from Coupon  " + DB.GetNoLock() + " where lower(CouponCode)=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("CouponCode").ToLowerInvariant().Trim()) + " and expirationdate>=getdate() and deleted=0");
                if (rsdis.Read())
                {
                    DiscountValid = String.Empty;
                }
                rsdis.Close();
            }

            StringBuilder sql = new StringBuilder(10000);
            sql.Append("update customer set ");
            if (CommonLogic.FormCanBeDangerousContent("BillingEqualsShipping").Length != 0)
            {
                sql.Append("BillingEqualsShipping=1,");
            }
            if (!TargetCustomer.IsRegistered)
            {
                sql.Append("RegisterDate=getdate(),");
            }
            if (DiscountValid.Length == 0) // meaning no error
            {
                sql.Append("CouponCode=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("CouponCode")) + ",");
            }
            else
            {
                sql.Append("CouponCode=NULL,");
            }


            sql.Append("FirstName=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("FirstName")) + ",");
            sql.Append("LastName=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("LastName")) + ",");

            try
            {
                if (CommonLogic.FormCanBeDangerousContent("DateOfBirth").Length != 0)
                {
                    DateTime dob = Localization.ParseNativeDateTime(CommonLogic.FormCanBeDangerousContent("DateOfBirth"));
                    sql.Append("DateOfBirth=" + DB.SQuote(Localization.ToDBShortDateString(dob)) + ",");
                }
                else
                {
                    sql.Append("DateOfBirth=NULL,");
                }
            }
            catch
            {
                sql.Append("DateOfBirth=NULL,");
            }
            if (!EMailAlreadyTaken)
            {
                sql.Append("EMail=" + DB.SQuote(EMailField) + ",");
            }
            else
            {
                sql.Append("EMail=" + DB.SQuote(TargetCustomer.EMail.ToLowerInvariant().Trim()) + ",");
            }

            sql.Append("Phone=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Phone")) + ",");

            VATIsValid = true;
            if (AppLogic.AppConfigBool("VAT.Enabled"))
            {
                String vtr = CommonLogic.FormCanBeDangerousContent("VATRegistrationID");
                if (!AppLogic.VATRegistrationIDIsValid(TargetCustomer, vtr))
                {
                    vtr = String.Empty;
                    VATIsValid = false;
                }
                sql.Append("VATRegistrationID=" + DB.SQuote(vtr) + ",");
            }

            sql.Append("LocaleSetting=" + DB.SQuote(Localization.CheckLocaleSettingForProperCase(CommonLogic.FormCanBeDangerousContent("LocaleSetting"))) + ",");

            if (AppLogic.MicropayIsEnabled())
            {
                sql.Append("MicroPayBalance=" + Localization.DecimalStringForDB(CommonLogic.FormUSDecimal("MicroPayBalance")) + ",");
            }
            sql.Append("OKToEMail=" + CommonLogic.FormUSInt("OKToEMail").ToString() + ",");
            sql.Append("CODCompanyCheckAllowed=" + CommonLogic.FormUSInt("CODCompanyCheckAllowed").ToString() + ",");
            sql.Append("AffiliateID=" + CommonLogic.FormUSInt("AffiliateID").ToString() + ",");
            sql.Append("CustomerLevelID=" + CommonLogic.FormUSInt("CustomerLevelID").ToString() + ",");
            sql.Append("CODNet30Allowed=" + CommonLogic.FormUSInt("CODNet30Allowed").ToString() + ",");
            sql.Append("Active=" + CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("locked").Length > 0, "0", "1") + ",");
            sql.Append("Over13Checked=" + CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("Over13").Length > 0, "1", "0") + ",");
            sql.Append("AdminCanViewCC=" + CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("canviewcc").Length > 0, "1", "0") + ",");
            sql.Append("Notes=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("CustomerNotes")));
            sql.Append(" where customerid=" + TargetCustomer.CustomerID.ToString());

            if ((TargetCustomer.AdminCanViewCC && CommonLogic.FormCanBeDangerousContent("canviewcc").Length == 0) || (!TargetCustomer.AdminCanViewCC && CommonLogic.FormCanBeDangerousContent("canviewcc").Length > 0))
            {
                Security.LogEvent("Admin Can View CC Changed", "The \"Admin Can View CC\" setting was changed from " + CommonLogic.IIF(TargetCustomer.AdminCanViewCC, "on", "off") + " to " + CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("canviewcc").Length > 0, "on", "off"), TargetCustomer.CustomerID, ThisCustomer.CustomerID, Convert.ToInt32(ThisCustomer.CurrentSessionID));
            }
            try
            {
                DB.ExecuteSQL(sql.ToString());
            }
            catch (Exception ex)
            {
                Response.Redirect("cst_account.aspx?customerid=" + TargetCustomer.CustomerID.ToString() + "&errormsg=" + Server.UrlEncode(ex.Message));
            }

            if (CommonLogic.FormCanBeDangerousContent("SubscriptionExpiresOn").Length == 0)
            {
                DB.ExecuteSQL("update customer set SubscriptionExpiresOn=NULL where CustomerID=" + TargetCustomer.CustomerID.ToString());
            }
            else
            {
                DateTime dt = Localization.ParseNativeDateTime(CommonLogic.FormCanBeDangerousContent("SubscriptionExpiresOn"));
                DB.ExecuteSQL("update customer set SubscriptionExpiresOn=" + DB.DateQuote(Localization.ToDBDateTimeString(dt)) + " where CustomerID=" + TargetCustomer.CustomerID.ToString());
            }


            if (AppLogic.AppConfigBool("VAT.Enabled") && !VATIsValid)
            {
                Response.Redirect("cst_account.aspx?customerid=" + TargetCustomer.CustomerID.ToString() + "&errormsg=" + AppLogic.GetString("cst_account.aspx.1x", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
            }

            if (!EMailAlreadyTaken)
            {
                Response.Redirect("cst_account.aspx?customerid=" + TargetCustomer.CustomerID.ToString() + "&msg=The Account Information Has Been Updated");
            }
            else
            {
                Response.Redirect("cst_account.aspx?customerid=" + TargetCustomer.CustomerID.ToString() + "&errormsg=" + AppLogic.GetString("cst_account.aspx.1", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
            }
        }

    }
}
