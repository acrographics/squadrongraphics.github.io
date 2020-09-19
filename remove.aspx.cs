// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/remove.aspx.cs 3     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
	/// <summary>
	/// Summary description for remove.
	/// </summary>
	public partial class remove : SkinBase
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(CommonLogic.QueryStringCanBeDangerousContent("id").Length != 0)
			{
                AppLogic.CheckForScriptTag(CommonLogic.QueryStringCanBeDangerousContent("id"));
                DB.ExecuteSQL("update customer set OKToEMail=0 where customerguid=" + DB.SQuote(CommonLogic.QueryStringCanBeDangerousContent("id")));
			}	
		}
	}
}
