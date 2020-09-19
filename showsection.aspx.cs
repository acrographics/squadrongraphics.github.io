// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/showsection.aspx.cs 1     7/08/06 10:43p Admin $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
	/// <summary>
	/// Summary description for showsection.
	/// </summary>
	public partial class showsection : SkinBase
	{
		ShowEntityPage m_EP;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_EP = new ShowEntityPage(EntityDefinitions.readonly_SectionEntitySpecs,this);
			m_EP.Page_Load(sender,e);
		}

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
		{
			m_EP.RenderContents(writer);
		}

        override protected void OnInit(EventArgs e)
        {
            if (AppLogic.AppConfigBool("TemplateSwitching.Enabled"))
            {
                String HT = AppLogic.GetCurrentEntityTemplateName(EntityDefinitions.readonly_SectionEntitySpecs.m_EntityName);
                SetTemplate(HT);
            }
            base.OnInit(e);
        }
	}
}
