// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/cst_editaddress_process.aspx.cs 9     9/27/06 4:19p Redwoodtree $
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

namespace AspDotNetStorefront
{
	/// <summary>
	/// Summary description for cst_editaddress_process.
	/// </summary>
	public partial class cst_editaddress_process : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
			Customer TargetCustomer = new Customer(CommonLogic.QueryStringUSInt("CustomerID"),true);

            if (TargetCustomer.IsAdminSuperUser && !ThisCustomer.IsAdminSuperUser)
            {
                throw new ArgumentException("Security Exception. Not Allowed");
            } 
            
            if (TargetCustomer.CustomerID == 0)
			{
				Response.Redirect("Customers.aspx");
			}

			int AddressID = CommonLogic.QueryStringUSInt("AddressID");
			int CustomerID = CommonLogic.QueryStringUSInt("CustomerID");
			string ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");

			String AddressTypeString = CommonLogic.QueryStringCanBeDangerousContent("AddressType");
			AddressTypes AddressType = (AddressTypes)Enum.Parse(typeof(AddressTypes),AddressTypeString,true);
      
			int DeleteAddressID = CommonLogic.FormUSInt("DeleteAddressID");
			if(DeleteAddressID == 0) 
			{
				DeleteAddressID = CommonLogic.QueryStringUSInt("DeleteAddressID");
			}
			if(DeleteAddressID != 0) 
			{
				Address.DeleteFromDB(DeleteAddressID,TargetCustomer.CustomerID);
				Response.Redirect("cst_selectaddress.aspx?" +CommonLogic.ServerVariables("QUERY_STRING"));
			}

            bool AllowShipToDifferentThanBillTo = AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo") && !AppLogic.AppConfigBool("SkipShippingOnCheckout");

			Address thisAddress = new Address();
			thisAddress.LoadFromDB(AddressID);

			thisAddress.CustomerID = CustomerID;
			thisAddress.AddressType = AddressType;

            thisAddress.NickName = CommonLogic.FormCanBeDangerousContent("AddressNickName");
			thisAddress.FirstName = CommonLogic.FormCanBeDangerousContent("AddressFirstName");
			thisAddress.LastName = CommonLogic.FormCanBeDangerousContent("AddressLastName");
			thisAddress.Company = CommonLogic.FormCanBeDangerousContent("AddressCompany");
			thisAddress.Address1 = CommonLogic.FormCanBeDangerousContent("AddressAddress1");
			thisAddress.Address2 = CommonLogic.FormCanBeDangerousContent("AddressAddress2");
			thisAddress.Suite = CommonLogic.FormCanBeDangerousContent("AddressSuite");
			thisAddress.City = CommonLogic.FormCanBeDangerousContent("AddressCity");
			thisAddress.State = CommonLogic.FormCanBeDangerousContent("AddressState");
			thisAddress.Zip = CommonLogic.FormCanBeDangerousContent("AddressZip");
			thisAddress.Country = CommonLogic.FormCanBeDangerousContent("AddressCountry");
			thisAddress.Phone = CommonLogic.FormCanBeDangerousContent("AddressPhone");

            if ((thisAddress.AddressType & AddressTypes.Billing) != 0 && ThisCustomer.AdminCanViewCC)
            {
                thisAddress.PaymentMethodLastUsed = CommonLogic.FormCanBeDangerousContent("PaymentMethod");
                if (AppLogic.CleanPaymentMethod(thisAddress.PaymentMethodLastUsed) == AppLogic.ro_PMECheck)
                {
                    thisAddress.ECheckBankABACode = CommonLogic.FormCanBeDangerousContent("ECheckBankABACode");
                    thisAddress.ECheckBankAccountNumber = CommonLogic.FormCanBeDangerousContent("ECheckBankAccountNumber");
                    thisAddress.ECheckBankName = CommonLogic.FormCanBeDangerousContent("ECheckBankName");
                    thisAddress.ECheckBankAccountName = CommonLogic.FormCanBeDangerousContent("ECheckBankAccountName");
                    thisAddress.ECheckBankAccountType = CommonLogic.FormCanBeDangerousContent("ECheckBankAccountType");
                }
                if (AppLogic.CleanPaymentMethod(thisAddress.PaymentMethodLastUsed) == AppLogic.ro_PMCreditCard)
                {
                    Security.LogEvent("Updated Credit Card", "", thisAddress.CustomerID, ThisCustomer.CustomerID, ThisCustomer.CurrentSessionID);
                    thisAddress.CardName = CommonLogic.FormCanBeDangerousContent("CardName");
                    thisAddress.CardType = CommonLogic.FormCanBeDangerousContent("CardType");

                    string tmpS = CommonLogic.FormCanBeDangerousContent("CardNumber");
                    if (!tmpS.StartsWith("*"))
                    {
                        thisAddress.CardNumber = tmpS;
                    }
                    thisAddress.CardExpirationMonth = CommonLogic.FormCanBeDangerousContent("CardExpirationMonth");
                    thisAddress.CardExpirationYear = CommonLogic.FormCanBeDangerousContent("CardExpirationYear");
                }
            }
			thisAddress.UpdateDB();

			Response.Redirect("cst_selectaddress.aspx?" +CommonLogic.ServerVariables("QUERY_STRING"));
		}

	}
}
