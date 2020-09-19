 // ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/default.aspx.cs 3     10/04/06 6:20a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
	/// <summary>
	/// Summary description for _default.
	/// </summary>
	public partial class _default : SkinBase
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{

			if(CommonLogic.ServerVariables("HTTP_HOST").ToUpperInvariant().IndexOf(AppLogic.LiveServer().ToUpperInvariant()) != -1 && CommonLogic.ServerVariables("HTTP_HOST").ToUpperInvariant().IndexOf("WWW") == -1)
			{
				if(AppLogic.RedirectLiveToWWW())
				{
                    Response.Redirect("http://www." + AppLogic.LiveServer().ToLowerInvariant());
                }
			}
		
			if(AppLogic.AppConfigBool("GoNonSecureAgain"))
			{
				SkinBase.GoNonSecureAgain();
			}

            // this may be overwridden by the XmlPackage below!
            SectionTitle = String.Format(AppLogic.GetString("default.aspx.1", SkinID, ThisCustomer.LocaleSetting), AppLogic.AppConfig("StoreName"));
            
            // set the Customer context, and set the SkinBase context, so meta tags to be set if they are not blank in the XmlPackage results
            Package1.SetContext = this;

            // unsupported feature:
            //System.Diagnostics.Trace.WriteLineIf(Config.TraceLevel.TraceVerbose, "Welcome to AspDotNetStorefront");
		}

		override protected void OnInit(EventArgs e)
		{
			String HT = AppLogic.HomeTemplate();
			if(HT.Length != 0 )
			{
                if (!HT.EndsWith(".ascx", StringComparison.InvariantCultureIgnoreCase))
				{
					HT = HT + ".ascx";
				}
				SetTemplate(HT);
			}
			base.OnInit(e);
		}
	}
}
