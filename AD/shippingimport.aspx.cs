// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/shippingimport.aspx.cs 2     9/03/06 8:40p Redwoodtree $
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
    public partial class shippingimport : AspDotNetStorefront.SkinBase 
	{ 

		private void Page_Load(object sender, System.EventArgs e) 
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            SectionTitle = "Import Postage Log (Step 2)"; 
		} 

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer) 
		{ 
			writer.Write("<p><b>Please review the log status shown below, and then test your store web site, to double check that the import worked properly</b></p>"); 
			writer.Write("<hr size=1>"); 
			writer.Write("<p><b>IMPORT LOG:</b></p>"); 
			string LogFileName = CommonLogic.QueryStringCanBeDangerousContent("LogFile"); 
			string LogFormat = CommonLogic.QueryStringCanBeDangerousContent("LogFormat"); 
			bool SendEmail = CommonLogic.QueryStringBool("SendEmail"); 
			bool tffDebug = CommonLogic.QueryStringBool("debug"); 
			
			string LogFile = CommonLogic.SafeMapPath("../download" + "/" + LogFileName + ".txt"); 
			string FmtPath = CommonLogic.SafeMapPath("ShippingImportFormats.xml");
            Int16 fmtNo = 0;
            if (LogFormat.Length > 0)
            {
                fmtNo = short.Parse(LogFormat);
                string outstr = ShippingImportCls.ProcessShippingLog(LogFile, fmtNo, SendEmail, tffDebug, base.EntityHelpers, base.GetParser);
                writer.Write(outstr);
            }
		} 

		protected override void OnInit(EventArgs e) 
		{ 
			InitializeComponent(); 
			base.OnInit(e); 
		} 

		private void InitializeComponent() 
		{ 
		} 
	} 
}