// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/setlocale.aspx.cs 4     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
	/// <summary>
	/// Summary description for setlocale.
	/// </summary>
	public partial class setlocale : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			 
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

			String LocaleSetting = CommonLogic.QueryStringCanBeDangerousContent("LocaleSetting");
            if (LocaleSetting.Length == 0)
            {
                LocaleSetting = CommonLogic.QueryStringCanBeDangerousContent("Locale");
            }
            if (LocaleSetting.Length == 0)
            {
                LocaleSetting = Localization.GetWebConfigLocale();
            }
            AppLogic.CheckForScriptTag(LocaleSetting);
            LocaleSetting = Localization.CheckLocaleSettingForProperCase(LocaleSetting);
            ThisCustomer.SetLocale(LocaleSetting);

            String CurrencySetting = CommonLogic.QueryStringCanBeDangerousContent("CurrencySetting");
            if (CurrencySetting.Length == 0)
            {
                CurrencySetting = CommonLogic.QueryStringCanBeDangerousContent("Currency");
            }
            if (CurrencySetting.Length == 0)
            {
                CurrencySetting = AppLogic.GetLocaleDefaultCurrency(LocaleSetting);
            }
            if (CurrencySetting.Length == 0)
            {
                CurrencySetting = Localization.GetPrimaryCurrency();
            }
            AppLogic.CheckForScriptTag(CurrencySetting);
            CurrencySetting = Localization.CheckCurrencySettingForProperCase(CurrencySetting);
            ThisCustomer.SetCurrency(CurrencySetting);

			Label1.Text = String.Format(AppLogic.GetString("setlocale.aspx.1",ThisCustomer.SkinID,ThisCustomer.LocaleSetting),AppLogic.GetLocaleSettingDescription(ThisCustomer.LocaleSetting));

			string ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");
            AppLogic.CheckForScriptTag(ReturnURL);

            if (ReturnURL.IndexOf("setlocale.aspx") != -1)
			{
				ReturnURL = String.Empty;
			}

			if (ReturnURL.Length == 0)
			{
				ReturnURL = "default.aspx";
			}
			Response.AddHeader("REFRESH","1; URL=" + Server.UrlDecode(ReturnURL));
		}

	}
}
