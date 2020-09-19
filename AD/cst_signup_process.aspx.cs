// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/cst_signup_process.aspx.cs 6     9/14/06 12:05a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for cst_signup_process.
    /// </summary>
    public partial class cst_signup_process : AspDotNetStorefront.SkinBase
    {

        int CustomerID;
        String EMailField;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            EMailField = CommonLogic.FormCanBeDangerousContent("EMail").ToLowerInvariant().Trim();
            if (!AppLogic.AppConfigBool("AllowCustomerDuplicateEMailAddresses") && Customer.EmailInUse(EMailField, 0))
            {
                ErrorMsg = "That E-Mail Address is already Taken! Please use another E-mail address!";
            }

            Regex re = new Regex(@"^[a-zA-Z0-9][-\+\w\.]*@([a-zA-Z0-9][\w\-]*\.)+[a-zA-Z]{2,4}$");
            if (!re.IsMatch(EMailField))
            {
                ErrorMsg = AppLogic.GetString("createaccount.aspx.17", SkinID, ThisCustomer.LocaleSetting);
            }

            CustomerID = 0;

            if (ErrorMsg.Length == 0)
            {
                IDataReader rs;

                try
                {
                    StringBuilder sql = new StringBuilder(2500);

                    if (!Editing)
                    {
                        // ok to add them:
                        String NewGUID = DB.GetNewGUID();
                        sql.Append("insert into Customer(CustomerGUID,IsRegistered, EMail,Password,SaltKey,Notes,DateOfBirth,SubscriptionExpiresOn,Gender,OKToEMail,FirstName,LastName,Phone,LocaleSetting,CurrencySetting,CouponCode) values(");
                        sql.Append(DB.SQuote(NewGUID) + ",");
                        sql.Append("1,"); //IsRegistered
                        sql.Append(DB.SQuote(CommonLogic.Left(EMailField, 100)) + ",");

                        AspDotNetStorefrontCommon.Password pwd = new Password(CommonLogic.FormCanBeDangerousContent("Password"));

                        sql.Append(DB.SQuote(pwd.SaltedPassword) + ",");
                        sql.Append(pwd.Salt.ToString() + ",");

                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("Notes")) + ",");

                        if (CommonLogic.FormCanBeDangerousContent("DateOfBirth").Length != 0)
                        {
                            try
                            {
                                DateTime dob = Localization.ParseNativeDateTime(CommonLogic.FormCanBeDangerousContent("DateOfBirth"));
                                sql.Append(DB.DateQuote(Localization.ToDBShortDateString(dob)) + ",");
                            }
                            catch
                            {
                                sql.Append("NULL,");
                            }
                        }
                        else
                        {
                            sql.Append("NULL,");
                        }
                        if (CommonLogic.FormCanBeDangerousContent("SubscriptionExpiresOn").Length != 0)
                        {
                            try
                            {
                                DateTime seo = Localization.ParseNativeDateTime(CommonLogic.FormCanBeDangerousContent("SubscriptionExpiresOn"));
                                sql.Append(DB.DateQuote(Localization.ToDBShortDateString(seo)) + ",");
                            }
                            catch
                            {
                                sql.Append("NULL,");
                            }
                        }
                        else
                        {
                            sql.Append("NULL,");
                        }

                        sql.Append(DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("Gender"), 1)) + ",");
                        sql.Append(CommonLogic.FormCanBeDangerousContent("OKToEMail") + ",");
                        sql.Append(DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("FirstName"), 50)) + ",");
                        sql.Append(DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("LastName"), 50)) + ",");
                        sql.Append(DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("Phone"), 25)) + ",");
                        sql.Append(DB.SQuote(Localization.GetWebConfigLocale()) + ",");
                        sql.Append(DB.SQuote(Currency.GetDefaultCurrency()) + ",");
                        sql.Append(DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("CouponCode"), 50)));

                        sql.Append(")");
                        DB.ExecuteSQL(sql.ToString());

                        rs = DB.GetRS("select CustomerID from Customer  " + DB.GetNoLock() + " where deleted=0 and CustomerGUID=" + DB.SQuote(NewGUID));
                        rs.Read();
                        CustomerID = DB.RSFieldInt(rs, "CustomerID");
                        Editing = true;
                        rs.Close();
                    }
                    else
                    {
                        // ok to update:
                        sql.Append("update Customer set ");
                        sql.Append("EMail=" + DB.SQuote(CommonLogic.Left(EMailField, 100)) + ",");

                        AspDotNetStorefrontCommon.Password pwd = new Password(CommonLogic.FormCanBeDangerousContent("Password"));
                        sql.Append("Password=" + DB.SQuote(pwd.SaltedPassword) + ",");
                        sql.Append("SaltKey=" + pwd.Salt.ToString() + ",");

                        sql.Append("Notes=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Notes")) + ",");

                        if (CommonLogic.FormCanBeDangerousContent("DateOfBirth").Length != 0)
                        {
                            try
                            {
                                DateTime dob = Localization.ParseNativeDateTime(CommonLogic.FormCanBeDangerousContent("DateOfBirth"));
                                sql.Append("DateOfBirth=" + DB.DateQuote(Localization.ToDBShortDateString(dob)) + ",");
                            }
                            catch
                            {
                                sql.Append("DateOfBirth=NULL,");
                            }
                        }
                        else
                        {
                            sql.Append("DateOfBirth=NULL,");
                        }
                        if (CommonLogic.FormCanBeDangerousContent("SubscriptionExpiresOn").Length != 0)
                        {
                            try
                            {
                                DateTime seo = Localization.ParseNativeDateTime(CommonLogic.FormCanBeDangerousContent("SubscriptionExpiresOn"));
                                sql.Append("SubscriptionExpiresOn=" + DB.DateQuote(Localization.ToDBShortDateString(seo)) + ",");
                            }
                            catch
                            {
                                sql.Append("SubscriptionExpiresOn=NULL,");
                            }
                        }
                        else
                        {
                            sql.Append("SubscriptionExpiresOn=NULL,");
                        }

                        //sql.Append("Gender=" + DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("Gender"),1)) + ",");
                        sql.Append("FirstName=" + DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("FirstName"), 50)) + ",");
                        sql.Append("LastName=" + DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("LastName"), 50)) + ",");
                        sql.Append("Phone=" + DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("Phone"), 25)) + ",");
                        sql.Append("CouponCode=" + DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("CouponCode"), 50)));
                        sql.Append(" where CustomerID=" + CustomerID.ToString());
                        DB.ExecuteSQL(sql.ToString());
                        Editing = true;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMsg = "<p><b>ERROR: " + CommonLogic.GetExceptionDetail(ex, "<br/>") + "<br/><br/></b></p>";
                }

            }
            SectionTitle = "<a href=\"Customers.aspx\">Customers</a> - Add new Customer";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            if (ErrorMsg.Length > 0)
            {
                writer.Write("<p align=\"left\"><b><font color=\"red\">" + ErrorMsg + "</font></b></p>");
                writer.Write("<p align=\"left\">Please <a href=\"javascript:history.back(-1);\">go back</a> and correct the error. Thanks.</p>");
            }
            else
            {
                writer.Write("<p align=\"left\"><b>Customer add was successful.<br/><br/><a href=\"cst_account.aspx?Customerid=" + CustomerID.ToString() + "\">Click here</a> to go to their account page.</p>");
            }
        }

    }
}
