// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2005.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. ADD YOUR ADDITIONAL COPYRIGHT HERE ALSO IF DESIRED
// $Header: /v6.2/Web/dumpuser.aspx.cs 1     9/20/06 5:41p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Web;
using System.Collections;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
	/// <summary>
	/// Summary description for dumpuser.
	/// </summary>
	public partial class dumpuser : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");

			Response.Write("<html><head><title>Dump User</title>");
			Response.Write("<style>");
            Response.Write("TD, SPAN, LI, BODY, P { color: #444444; font-size: 11px; font-family: Verdana, Geneva, Arial, Helvetica, sans-serif; }");
            Response.Write("</style>");
			Response.Write("</head><body bgcolor=\"FFFFFF\">");
            Response.Write("<b>SESSION:</b><hr size=1>");
			for(int i = 0; i<=Session.Count-1; i++)
			{
				Response.Write("<b>" + Session.Keys[i] + "</b>=" + Session[Session.Keys[i]] + "<br/>");
			}
			Response.Write("<br/>");

			Response.Write("<b>APPLICATION:</b><hr size=1>");
			for(int i = 0; i<=Application.Count-1; i++)
			{
				Response.Write("<b>" + Application.Keys[i] + "</b>=" + Application[Application.Keys[i]] + "<br/>");
			}
			Response.Write("<br/>");

			Response.Write("<b>CACHE:</b><hr size=1>");
			foreach (DictionaryEntry entry in HttpContext.Current.Cache)
			{
				Response.Write("<b>" + entry.Key + "</b><br/>");
			}
			Response.Write("<br/>");

			Response.Write("<b>QUERY STRING:</b><hr size=1>");
			for(int i = 0; i<=Request.QueryString.Count-1; i++)
			{
				Response.Write("<b>" + Request.QueryString.Keys[i] + "</b>=" + Request.QueryString[Request.QueryString.Keys[i]] + "<br/>");
			}
			Response.Write("<br/>");

			Response.Write("<b>FORM:</b><hr size=1>");
			for(int i = 0; i<=Request.Form.Count-1; i++)
			{
				Response.Write("<b>" + Request.Form.Keys[i] + "</b>=" + Request.Form[Request.Form.Keys[i]] + "<br/>");
			}
			Response.Write("<br/>");

			Response.Write("<b>COOKIES:</b><hr size=1>");
			for(int i = 0; i<=Request.Cookies.Count-1; i++)
			{
				Response.Write("<b>" + Request.Cookies.Keys[i] + "</b>=" + Server.UrlDecode(Request.Cookies[Request.Cookies.Keys[i]].Value) + "<br/>");
			}
			Response.Write("<br/>");

			Response.Write("<b>SERVER VARIABLES:</b><hr size=1>");
			for(int i = 0; i<=Request.ServerVariables.Count-1; i++)
			{
				Response.Write("<b>" + Request.ServerVariables.Keys[i] + "</b>=" + Request.ServerVariables[Request.ServerVariables.Keys[i]] + "<br/>");
			}
			Response.Write("<br/>");
			Response.Write("</body></html>");
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}
