// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/cst_signup.aspx.cs 7     9/30/06 3:39p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for cst_signup.
	/// </summary>
    public partial class cst_signup : AspDotNetStorefront.SkinBase
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache"); 
            

			SectionTitle = "<a href=\"Customers.aspx\">Customers</a> - Create New Customer";
		}

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
		{
			if(CommonLogic.QueryStringCanBeDangerousContent("unknownerror").Length > 0)
			{
				writer.Write("<p align=\"left\"><b><font color=\"#FF0000\">There was an error saving the account profile. Please try again.</font></b></p>");
			}
			if(CommonLogic.QueryStringCanBeDangerousContent("badlogin").Length > 0)
			{
				writer.Write("<p align=\"left\"><b><font color=\"#FF0000\">That e-mail/password combination is already taken. Please enter a different password.</font></b></p>");
			}
			if(CommonLogic.QueryStringCanBeDangerousContent("msg").Length > 0)
			{
				writer.Write("<p align=\"left\"><b><font color=\"blue\">The Account Information Has Been Updated</font></b></p>");
			}

			if(CommonLogic.QueryStringCanBeDangerousContent("errormsg").Length > 0)
			{
				writer.Write("<p align=\"left\"><b><font color=\"red\">" + Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg")) + "</font></b></p>");
			}

            bool AllowShipToDifferentThanBillTo = AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo") && !AppLogic.AppConfigBool("SkipShippingOnCheckout");

			writer.Write("<script type=\"text/javascript\">\n");
      
			writer.Write("function copyaccount(theForm)\n");
			writer.Write("{\n");
			writer.Write("if (theForm.ShippingEqualsAccount.checked)\n");
			writer.Write("{\n");
			writer.Write("theForm.AddressFirstName.value = theForm.FirstName.value;\n");
			writer.Write("theForm.AddressLastName.value = theForm.LastName.value;\n");
			writer.Write("theForm.AddressPhone.value = theForm.Phone.value;\n");
			writer.Write("}\n");
			writer.Write("return (true);\n");
			writer.Write("}\n");
			writer.Write("\n");

			writer.Write("function NewCustForm_Validator(theForm)\n");
			writer.Write("{\n");
			writer.Write("  submitonce(theForm);\n");
			writer.Write("  if (theForm.FirstName.value == \"\" && theForm.LastName.value == \"\")\n");
			writer.Write("  {\n");
			writer.Write("    alert(\"You must enter at least one of First Name or Last Name.\");\n");
			writer.Write("    theForm.FirstName.focus();\n");
			writer.Write("    submitenabled(theForm);\n");
			writer.Write("    return (false);\n");
			writer.Write("  }\n");
			writer.Write("  if (theForm.Password.value.length < 3)\n");
			writer.Write("  {\n");
			writer.Write("    alert(\"Please enter at least 3 characters for the password.\");\n");
			writer.Write("    theForm.Password.focus();\n");
			writer.Write("    submitenabled(theForm);\n");
			writer.Write("    return (false);\n");
			writer.Write("  }\n");
			writer.Write("  if (theForm.Password.value != theForm.Password2.value)\n");
			writer.Write("  {\n");
			writer.Write("    alert(\"The passwords do not match. Please re-enter the password to confirm.\");\n");
			writer.Write("    theForm.Password2.focus();\n");
			writer.Write("    submitenabled(theForm);\n");
			writer.Write("    return (false);\n");
			writer.Write("  }\n");
			//      writer.Write("  if (theForm.ShippingState.selectedIndex < 1)\n");
			//      writer.Write("  {\n");
			//      writer.Write("    alert(\"Please select one of the Shipping State options.\");\n");
			//      writer.Write("    theForm.ShippingState.focus();\n");
			//      writer.Write("    submitenabled(theForm);\n");
			//      writer.Write("    return (false);\n");
			//      writer.Write("  }\n");
			//      writer.Write("  if (theForm.BillingState.selectedIndex < 1)\n");
			//      writer.Write("  {\n");
			//      writer.Write("    alert(\"Please select one of the Billing State options.\");\n");
			//      writer.Write("    theForm.BillingState.focus();\n");
			//      writer.Write("    submitenabled(theForm);\n");
			//      writer.Write("    return (false);\n");
			//      writer.Write("  }\n");
			writer.Write("  return (true);\n");
			writer.Write("}\n");
			writer.Write("</script>\n");
			
			writer.Write("  <!-- calendar stylesheet -->\n");
			writer.Write("  <link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"jscalendar/calendar-win2k-cold-1.css\" title=\"win2k-cold-1\" />\n");
			writer.Write("\n");
			writer.Write("  <!-- main calendar program -->\n");
			writer.Write("  <script type=\"text/javascript\" src=\"jscalendar/calendar.js\"></script>\n");
			writer.Write("\n");
			writer.Write("  <!-- language for the calendar -->\n");
			writer.Write("  <script type=\"text/javascript\" src=\"jscalendar/lang/" + Localization.JSCalendarLanguageFile() + "\"></script>\n");
			writer.Write("\n");
			writer.Write("  <!-- the following script defines the Calendar.setup helper function, which makes\n");
			writer.Write("       adding a calendar a matter of 1 or 2 lines of code. -->\n");
			writer.Write("  <script type=\"text/javascript\" src=\"jscalendar/calendar-setup.js\"></script>\n");
			
			String act = "cst_signup_process.aspx";
			writer.Write("<form method=\"POST\" action=\"" + act + "\" name=\"NewCustForm\" id=\"NewCustForm\")\" onSubmit=\"return (validateForm(this) && NewCustForm_Validator(this))\">");

			// ACCOUNT BOX:
			writer.Write("<table width=\"100%\" cellpadding=\"2\" cellspacing=\"0\" border=\"0\" style=\"border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor") + "\">\n");
			writer.Write("<tr><td align=\"left\" valign=\"top\">\n");
			writer.Write("<img src=\"" + AppLogic.LocateImageURL("../skins/Skin_" + SkinID.ToString().ToString()+ "/images/accountinfo.gif") + "\" border=\"0\"><br/>");
			writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"0\" style=\"" + AppLogic.AppConfig("BoxFrameStyle") + "\">\n");
			writer.Write("<tr><td align=\"left\" valign=\"top\">\n");

			writer.Write("    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">");
			writer.Write("      <tr>");
			writer.Write("        <td width=\"100%\" colspan=\"2\"><b>The Account &amp; Contact Information is used to login to the site. Please save the password in a safe place.</b></td>");
			writer.Write("      </tr>");
			writer.Write("      <tr>");
			writer.Write("        <td width=\"100%\" colspan=\"2\">");
			writer.Write("          <hr/>");
			writer.Write("        </td>");
			writer.Write("      </tr>");
			writer.Write("      <tr>");
			writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">*First Name:</td>");
			writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
			writer.Write("        <input type=\"text\" name=\"FirstName\" size=\"20\" maxlength=\"50\" value=\"\">");
			writer.Write("        <input type=\"hidden\" name=\"FirstName_vldt\" value=\"[req][blankalert=Please enter the first name]\">");
			writer.Write("        </td>");
			writer.Write("      </tr>");
			writer.Write("      <tr>");
			writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">*Last Name:</td>");
			writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
			writer.Write("        <input type=\"text\" name=\"LastName\" size=\"20\" maxlength=\"50\" value=\"\">");
			writer.Write("        <input type=\"hidden\" name=\"LastName_vldt\" value=\"[req][blankalert=Please enter the last name]\">");
			writer.Write("        </td>");
			writer.Write("      </tr>");
			writer.Write("      <tr>");
			writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">*E-Mail:</td>");
			writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
			writer.Write("        <input type=\"text\" name=\"EMail\" size=\"37\" maxlength=\"100\" value=\"\">");
			writer.Write("        <input type=\"hidden\" name=\"EMail_vldt\" value=\"[req][blankalert=Please enter an e-mail address][invalidalert=Please enter a valid e-mail address]\">");
			writer.Write("        </td>");
			writer.Write("      </tr>");
			writer.Write("      <tr>");
			writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">*Create A Password:</td>");
			writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
			writer.Write("        <input type=\"password\" name=\"Password\" size=\"37\" maxlength=\"100\" value=\"\">");
			writer.Write("        <input type=\"hidden\" name=\"Password_vldt\" value=\"[req][blankalert=Please enter a password so you can login to this site at a later time]\">\n");
			writer.Write("        </td>");
			writer.Write("      </tr>");
						
			writer.Write("      <tr>");
			writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">*Repeat Password:</td>");
			writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
			writer.Write("        <input type=\"password\" name=\"Password2\" size=\"37\" maxlength=\"100\" value=\"\">");
			writer.Write("        <input type=\"hidden\" name=\"Password2_vldt\" value=\"[req][blankalert=Please re-enter a password again to verify]\">\n");
			writer.Write("        </td>");
			writer.Write("      </tr>");
			
			writer.Write("      <tr>");
			writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">Subscription Expires On:</td>");
			writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
            writer.Write("        <input type=\"text\" name=\"SubscriptionExpiresOn\" size=\"12\" value=\"\">&nbsp;<img src=\"" + AppLogic.LocateImageURL("../skins/skin_" + SkinID.ToString() + "/images/calendar.gif") + "\" style=\"cursor:hand;cursor:pointer;\" align=\"absmiddle\" id=\"f_trigger_s\">");
			//writer.Write("			<input type=\"hidden\" name=\"SubscriptionExpiresOn_vldt\" value=\"[date][invalidalert=Please enter a valid date in the format " + Localization.ShortDateFormat() + "]\">");
			writer.Write("        </td>");
			writer.Write("      </tr>");
			
			writer.Write("      <tr>");
			writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">*Phone:</td>");
			writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
			writer.Write("        <input type=\"text\" name=\"Phone\" size=\"14\" maxlength=\"20\" value=\"\">");
			writer.Write("        </td>");
			writer.Write("      </tr>");

			writer.Write("      <tr>");
			writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">Special Offer/Coupon Code:</td>");
			writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
			writer.Write("        <input type=\"text\" name=\"CouponCode\" size=\"14\" maxlength=\"50\" value=\"\"> (If you received a special offer code, enter it here)");
			writer.Write("        </td>");
			writer.Write("      </tr>");

			writer.Write("      <tr>");
			writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">Date Of Birth:</td>");
			writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
			writer.Write("        <input type=\"text\" name=\"DateOfBirth\" size=\"14\" maxlength=\"20\" value=\"\"> (in format " + Localization.ShortDateFormat() + ")");
			//writer.Write("        <input type=\"hidden\" name=\"DateOfBirth_vldt\" value=\"[date][blankalert=Please enter a date of birth][invalidalert=Please enter a valid date of birth in the format " + Localization.ShortDateFormat() + "]\">");
			writer.Write("        </td>");
			writer.Write("      </tr>");

			writer.Write("      <tr>");
			writer.Write("        <td width=\"25%\" align=\"right\" valign=\"middle\">OK To EMail:</td>");
			writer.Write("        <td width=\"75%\" align=\"left\" valign=\"middle\">");
			writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"OKToEMail\" value=\"1\"  checked >\n");
			writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"OKToEMail\" value=\"0\">\n");
			writer.Write("<small>(Can we contact you with product updates/information, etc...)</small>");
			writer.Write("        </td>");
			writer.Write("      </tr>");

            writer.Write("        <tr>");
            writer.Write("            <td width=\"25%\" align=\"right\" valign=\"top\">Notes:</td>");
            writer.Write("            <td width=\"75%\" align=\"left\" valign=\"top\"> <textarea rows=\"5\" style=\"width: 100%;\" name=\"CustomerNotes\"></textarea></td>");
            writer.Write("        </tr>");

			writer.Write("</table>\n");
			
			writer.Write("</td></tr>\n");
			writer.Write("</table>\n");
			writer.Write("</td></tr>\n");
			writer.Write("</table>\n");

			writer.Write("<p align=\"center\"><input type=\"submit\" value=\"Create Customer Record\" name=\"Continue\"></p>");
			writer.Write("</form>");

			writer.Write("\n<script type=\"text/javascript\">\n");
			writer.Write("    Calendar.setup({\n");
			writer.Write("        inputField     :    \"SubscriptionExpiresOn\",      // id of the input field\n");
			writer.Write("        ifFormat       :    \"" + Localization.JSCalendarDateFormatSpec() + "\",       // format of the input field\n");
			writer.Write("        showsTime      :    false,            // will display a time selector\n");
			writer.Write("        button         :    \"f_trigger_s\",   // trigger for the calendar (button ID)\n");
			writer.Write("        singleClick    :    true            // Single-click mode\n");
			writer.Write("    });\n");
			writer.Write("</script>\n");
		}

	}
}
