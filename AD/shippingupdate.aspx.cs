// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/shippingupdate.aspx.cs 4     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System; 
using System.Data; 
using System.Data.SqlClient; 
using System.Data.SqlTypes; 
using System.Text; 
using System.Web; 
using AspDotNetStorefrontCommon; 

namespace AspDotNetStorefrontAdmin 
{
	/// <summary>
	/// Summary description for shippingupdate.
	/// </summary>
    public partial class shippingupdate : AspDotNetStorefront.SkinBase
	{

		private void Page_Load(object sender, System.EventArgs e) 
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            SectionTitle = "Process Notice Emails"; 
		} 

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer) 
		{
		

			if (CommonLogic.FormCanBeDangerousContent("IsSubmit").ToUpperInvariant() == "TRUE")
			// do update
			{

                string outstr = ShippingImportCls.ProcessOrderNoticeEmail(base.EntityHelpers, base.GetParser); 

				writer.Write(outstr); 
			}
			else
			// show items to update	
			{
                string outstr = ShippingImportCls.CheckOrderNoticeEmail();

				writer.Write(outstr); 

				writer.Write("<script type=\"text/javascript\">\n"); 
				writer.Write("function Form_Validator(theForm)\n"); 
				writer.Write("  {\n"); 
				writer.Write("  return (true);\n"); 
				writer.Write("  }\n"); 
				writer.Write("</script>\n"); 
		
				writer.Write("<form action=\"\" method=\"post\" onsubmit=\"return (validateForm(this) && Form_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n"); 
				writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n"); 
				writer.Write("<input type=\"submit\" value=\"Send All Email Notices\" name=\"submit\">\n"); 
				writer.Write("</form>\n"); 
				
			}
		
		}

		#region Skin Required code
		protected override void OnInit(EventArgs e) 
		{ 
			InitializeComponent(); 
			base.OnInit(e); 
		} 

		private void InitializeComponent() 
		{ 
		} 
		#endregion
	}
}
