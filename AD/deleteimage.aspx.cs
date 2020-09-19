// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/deleteimage.aspx.cs 4     9/30/06 3:39p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.IO;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for deleteimage.
	/// </summary>
	public partial class deleteimage : System.Web.UI.Page
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
			String FormImageName = CommonLogic.QueryStringCanBeDangerousContent("FormImageName");
			String ImgUrl = CommonLogic.QueryStringCanBeDangerousContent("ImgUrl");

			if(ThisCustomer.IsAdminUser)
			{
				Response.Write("<html><head><title>Delete Image</title></head><body>\n");
				System.IO.File.Delete(CommonLogic.SafeMapPath(ImgUrl));
				Response.Write("<script type=\"text/javascript\">\n");
				Response.Write("opener.document.getElementById('" + FormImageName + "').src = '../images/spacer.gif';\n");
				Response.Write("self.close();\n");
				Response.Write("</script>\n");
			}
			else
			{
				Response.Write("<script type=\"text/javascript\">\n");
				Response.Write("self.close();\n");
				Response.Write("</script>\n");
			}

			Response.Write("</body></html>\n");
		}

	}
}
