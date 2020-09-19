// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/entityMoveVariant.aspx.cs 3     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for movevariant
	/// </summary>
	public partial class entityMoveVariant : System.Web.UI.Page
	{
        private Customer ThisCustomer;
		int ProductID;
		int VariantID;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

			ProductID = CommonLogic.QueryStringUSInt("ProductID");
			VariantID = CommonLogic.QueryStringUSInt("VariantID");

			if(CommonLogic.FormBool("IsSubmit"))
			{
				DB.ExecuteSQL("Update productvariant set ProductID=" + CommonLogic.FormCanBeDangerousContent("NewProductID") + " where VariantID=" + VariantID.ToString());
			}

            LoadContent();
		}

        protected void LoadContent()
		{
			IDataReader rs = DB.GetRS("select * from productvariant  " + DB.GetNoLock() + " where VariantID=" + VariantID.ToString());
			rs.Read();
            string str = "";
			str += ("<b>Within Product: " + AppLogic.GetProductName(ProductID,ThisCustomer.LocaleSetting) + " (SKU=" + AppLogic.GetProductSKU(ProductID) + ", ID=" + ProductID.ToString() + ")<br/></b>\n");
			str += ("<b>Moving Variant: " + DB.RSFieldByLocale(rs,"Name",ThisCustomer.LocaleSetting) + " (SKUSuffix=" + DB.RSField(rs,"SKUSuffix") + ", ID=" + DB.RSFieldInt(rs,"VariantID").ToString() + ")<br/><br/></b>\n");
			rs.Close();

			str += ("<script type=\"text/javascript\">\n");
			str += ("function MoveForm_Validator(theForm)\n");
			str += ("{\n");
			str += ("submitonce(theForm);\n");
			str += ("if (theForm.NewProductID.selectedIndex < 1)\n");
			str += ("{\n");
			str += ("alert(\"Please select the product to which you want to move this variant.\");\n");
			str += ("theForm.NewProductID.focus();\n");
			str += ("submitenabled(theForm);\n");
			str += ("return (false);\n");
			str += ("    }\n");
			str += ("return (true);\n");
			str += ("}\n");
			str += ("</script>\n");

			str += ("<p>Select the product to which you want to move this variant:</p>\n");
			str += ("<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\">\n");
			str += ("<form action=\"movevariant.aspx?productid=" + ProductID.ToString() + "&VariantID=" + VariantID.ToString() + "\" method=\"post\" id=\"MoveForm\" name=\"MoveForm\" onsubmit=\"return (validateForm(this) && MoveForm_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
			str += ("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
			str += ("              <tr valign=\"middle\">\n");
			str += ("                <td width=\"100%\" colspan=\"2\" align=\"left\">\n");
			str += ("                </td>\n");
			str += ("              </tr>\n");

			str += ("              <tr valign=\"middle\">\n");
			str += ("                <td align=\"right\" valign=\"middle\">*Assign To Product:&nbsp;&nbsp;</td>\n");
			str += ("                <td align=\"left\">\n");
			str += ("<select size=\"1\" name=\"NewProductID\">\n");
			str += (" <OPTION VALUE=\"0\" selected>SELECT ONE</option>\n");
			IDataReader rsst = DB.GetRS("select * from Product  " + DB.GetNoLock() + " where deleted=0 and published=1");
			while(rsst.Read())
			{
				str += ("<option value=\"" + DB.RSFieldInt(rsst,"ProductID").ToString() + "\"");
				str += (">" + DB.RSFieldByLocale(rsst,"Name",ThisCustomer.LocaleSetting) + "</option>");
			}
			rsst.Close();
			str += ("</select>\n");
			str += ("</td>\n");
			str += ("</tr>\n");

			str += ("<tr>\n");
			str += ("<td></td><td align=\"left\"><br/>\n");
			str += ("<input type=\"submit\" value=\"Move\" name=\"submit\">\n");
			str += ("</td>\n");
			str += ("</tr>\n");
			str += ("</form>\n");
			str += ("</table>\n");

            ltContent.Text = str;
		}

	}
}
