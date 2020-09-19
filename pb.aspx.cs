// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/pb.aspx.cs 4     9/13/06 11:13p Redwoodtree $
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
	/// Summary description for pb.
	/// </summary>
	public partial class pb : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
			
			Response.Write("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0//EN\" \"http://www.w3.org/TR/REC-html40/strict.dtd\">\n");
			Response.Write("<html>\n");
			Response.Write("<head>\n");
			Response.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\n");
			Response.Write("<title>Product Browser</title>\n");
			Response.Write("<link rel=\"stylesheet\" href=\"skins/Skin_" + ThisCustomer.SkinID.ToString() + "/style.css\" type=\"text/css\">\n");
			Response.Write("<script type=\"text/javascript\" src=\"jscripts/formValidate.js\"></script>\n");
			Response.Write("</head>\n");
			Response.Write("<body style=\"margin: 0px;\" bottommargin=\"0\" leftmargin=\"0\" marginheight=\"0\" marginwidth=\"0\" rightmargin=\"0\" topmargin=\"0\" bgcolor=\"#FFFFFF\">\n");
			Response.Write("<!-- PAGE INVOCATION: '%INVOCATION%' -->\n");

			
			int PackID = CommonLogic.QueryStringUSInt("PackID");
			int ProductID = CommonLogic.QueryStringUSInt("ProductID");
			int CategoryID = CommonLogic.QueryStringUSInt("CategoryID");
			int SectionID = CommonLogic.QueryStringUSInt("SectionID");

			if(AppLogic.ProductHasBeenDeleted(ProductID))
			{
				Response.Redirect(SE.MakeDriverLink("ProductNotFound"));
			}

			IDataReader rs = DB.GetRS("select * from product  " + DB.GetNoLock() + " where product.ProductID=" + ProductID.ToString());
			if(!rs.Read())
			{
				rs.Close();
				Response.Redirect(SE.MakeDriverLink("ProductNotFound"));
			}

			bool RequiresReg = DB.RSFieldBool(rs,"RequiresRegistration");
			rs.Close();

			if(RequiresReg && !ThisCustomer.IsRegistered)
			{
				Response.Write("<b>" + AppLogic.GetString("pb.aspx.1",1,Localization.GetWebConfigLocale()) + "</b>");
			}
			else
			{
                XmlPackage2 p = new XmlPackage2("productbrowser.xml.config", ThisCustomer, 1, String.Empty, String.Empty, String.Empty);
                Response.Write(AppLogic.RunXmlPackage(p, null, ThisCustomer, 1, false, false));
  			}
			Response.Write("</body>\n");
			Response.Write("</html>\n");
		}

	}
}
