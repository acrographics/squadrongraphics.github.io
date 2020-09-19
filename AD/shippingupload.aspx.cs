// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/shippingupload.aspx.cs 5     10/04/06 12:00p Redwoodtree $
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
    public partial class shippingupload : AspDotNetStorefront.SkinBase 
	{ 

		private void Page_Load(object sender, System.EventArgs e) 
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            SectionTitle = "Import Postage Log (Step 1)"; 
		} 

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer) 
		{ 
			string FileName = "Import_" + Localization.ToNativeShortDateString(System.DateTime.Now).Replace(" ", "").Replace("/", "").Replace(":", "").Replace(".", ""); 
			FileName = "PostageImport_" + DateTime.Now.ToString("MMddyyyyHHmmss"); 
			bool DataUpdated = false; 
			string ErrorMsg = string.Empty; 
			string FmtPath = CommonLogic.SafeMapPath("ShippingImportFormats.xml");

            DataTable FmtDt = ShippingImportCls.GetImportFormatList(); 
			if (CommonLogic.FormCanBeDangerousContent("IsSubmit").ToUpperInvariant() == "TRUE") 
			{ 
				string PostedFormat = Request.Form["LogFormat"]; 
				try 
				{ 
					string LogFile = string.Empty; 
					HttpPostedFile PostedFile = Request.Files["LogFile"]; 
					writer.Write("<p><b><font color=red>" + "</font></b></p>\n"); 
					if (PostedFile.ContentLength != 0) 
					{ 
						string TargetFile = CommonLogic.SafeMapPath("../download/" + FileName + ".txt"); 
						PostedFile.SaveAs(TargetFile); 
					} 
					DataUpdated = true; 
				} 
				catch (Exception ex) 
				{ 
					ErrorMsg = CommonLogic.GetExceptionDetail(ex, "<br/>"); 
				} 
				if (DataUpdated) 
				{ 
					writer.Write("<p align=\"left\"><font size=3 color=blue><b><a href=\"shippingimport.aspx?LogFile=" + FileName + "&LogFormat=" + PostedFormat + "\">Shipping Log Upload successful. Click here to go to begin processing...</a></b></font></p><p></p>\n"); 
				} 
			} 
			if (ErrorMsg.Length != 0) 
			{ 
				writer.Write("<p><b><font color=red>" + ErrorMsg + "</font></b></p>\n"); 
			} 
			if (ErrorMsg.Length == 0) 
			{ 
				writer.Write("<p><big><b><font color=red>Import Postage Log (Step 1: Upload File)</font></b></big></p>"); 
				writer.Write("<p>This is still a experimental function...</p>"); 
				writer.Write("<hr size=1>"); 
				writer.Write("<p align=\"left\"><b>Import Shipping Data</b></p>\n"); 
				writer.Write("<script type=\"text/javascript\">\n"); 
				writer.Write("function Form_Validator(theForm)\n"); 
				writer.Write("  {\n"); 
				writer.Write("  return (true);\n"); 
				writer.Write("  }\n"); 
				writer.Write("</script>\n"); 
				writer.Write("<p align=\"left\">Select the local Postage Log file that you want to upload. This file must conform to our Shipping Import File Format Specifications defined in the manual!<br/><br/>This file should be on your own computer. Click 'browse' to select the file on your computer:</p>\n"); 
				writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n"); 
				writer.Write("<form enctype=\"multipart/form-data\" action=\"?LogFile=" + FileName + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(this) && Form_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n"); 
				writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n"); 
				writer.Write("<tr>\n"); 
				writer.Write("<td valign=\"top\" align=\"right\">*File:\n"); 
				writer.Write("</td>\n"); 
				writer.Write("<td valign=\"top\" align=\"left\">"); 
				writer.Write("<input type=\"file\" name=\"LogFile\" size=\"50\" value=\"\">\n"); 
				writer.Write("</td>\n"); 
				writer.Write(" </tr>\n"); 
				writer.Write("<tr>\n"); 
				writer.Write("<td valign=\"top\" align=\"right\">*Format:\n"); 
				writer.Write("</td>\n"); 
				writer.Write("<td valign=\"top\" align=\"left\">"); 
				writer.Write("<select size=\"1\" name=\"LogFormat\">\n"); 
				writer.Write("<option selected>select format:</option>\n"); 
				foreach (DataRow r in FmtDt.Rows) 
				{ 
					writer.Write("<option value=\"{0}\">{1}</option>\n", r["ID"], r["Name"]); 
				} 
				writer.Write("</select>\n"); 
				writer.Write("</td>\n"); 
				writer.Write(" </tr>\n"); 
				writer.Write("<tr>\n"); 
				writer.Write("<td valign=\"top\" align=\"right\">Send Notice Email:\n"); 
				writer.Write("</td>\n"); 
				writer.Write("<td valign=\"top\" align=\"left\">"); 
				writer.Write("<input type=\"checkbox\" name=\"SendEmail\" value=\"true\">\n"); 
				writer.Write("</td>\n"); 
				writer.Write(" </tr>\n"); 
				writer.Write("<tr>\n"); 
				writer.Write("<td></td><td align=\"left\"><br/>\n"); 
				writer.Write("<input type=\"submit\" value=\"Upload\" name=\"submit\">\n"); 
				writer.Write("</td>\n"); 
				writer.Write("</tr>\n"); 
				writer.Write("</form>\n"); 
				writer.Write("</table>\n"); 
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