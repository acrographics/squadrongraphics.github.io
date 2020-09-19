// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editshippingzone.aspx.cs 6     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for editshippingzone
    /// </summary>
    public partial class editshippingzone : AspDotNetStorefront.SkinBase
    {

        int ShippingZoneID;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            ShippingZoneID = 0;

            if (CommonLogic.QueryStringCanBeDangerousContent("ShippingZoneID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("ShippingZoneID") != "0")
            {
                Editing = true;
                ShippingZoneID = Localization.ParseUSInt(CommonLogic.QueryStringCanBeDangerousContent("ShippingZoneID"));
            }
            else
            {
                Editing = false;
            }

            IDataReader rs;

            if (CommonLogic.FormBool("IsSubmit"))
            {
                StringBuilder sql = new StringBuilder(2500);
                if (!Editing)
                {
                    // ok to add:
                    String NewGUID = DB.GetNewGUID();
                    sql.Append("insert into shippingZone(ShippingZoneGUID,Name,ZipCodes) values(");
                    sql.Append(DB.SQuote(NewGUID) + ",");
                    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                    sql.Append(DB.SQuote(Regex.Replace(CommonLogic.FormCanBeDangerousContent("ZipCodes"),"\\s+","")));
                    sql.Append(")");
                    DB.ExecuteSQL(sql.ToString());

                    rs = DB.GetRS("select ShippingZoneID from shippingZone  " + DB.GetNoLock() + " where deleted=0 and ShippingZoneGUID=" + DB.SQuote(NewGUID));
                    rs.Read();
                    ShippingZoneID = DB.RSFieldInt(rs, "ShippingZoneID");
                    Editing = true;
                    rs.Close();
                    DataUpdated = true;
                }
                else
                {
                    // ok to update:
                    sql.Append("update shippingZone set ");
                    sql.Append("Name=" + DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                    sql.Append("ZipCodes=" + DB.SQuote(Regex.Replace(CommonLogic.FormCanBeDangerousContent("ZipCodes"), "\\s+", "")));
                    sql.Append(" where ShippingZoneID=" + ShippingZoneID.ToString());
                    DB.ExecuteSQL(sql.ToString());
                    DataUpdated = true;
                    Editing = true;
                }
            }
            SectionTitle = "<a href=\"shippingZones.aspx\">Shipping Zones</a> - Manage Shipping Zones";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            IDataReader rs = DB.GetRS("select * from ShippingZone  " + DB.GetNoLock() + " where deleted=0 and ShippingZoneID=" + ShippingZoneID.ToString());
            Editing = false;
            if (rs.Read())
            {
                Editing = true;
            }

            if (ErrorMsg.Length != 0)
            {
                writer.Write("<p align=\"left\"><b><font color=red>" + ErrorMsg + "</font></b></p>\n");
            }
            if (DataUpdated)
            {
                writer.Write("<p align=\"left\"><b><font color=blue>(UPDATED)</font></b></p>\n");
            }

            writer.Write(CommonLogic.ReadFile("jscripts/tabs.js", true));

            if (ErrorMsg.Length == 0)
            {

                if (Editing)
                {
                    writer.Write("<p align=\"left\"><b>Editing Shipping Zone: " + DB.RSFieldByLocale(rs, "Name", ThisCustomer.LocaleSetting) + " (ID=" + DB.RSFieldInt(rs, "ShippingZoneID").ToString() + ")</p></b>\n");
                }
                else
                {
                    writer.Write("<p align=\"left\"><b>Adding New Shipping Zone:</p></b>\n");
                }

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function ShippingZoneForm_Validator(theForm)\n");
                writer.Write("{\n");
                writer.Write("submitonce(theForm);\n");
                writer.Write("return (true);\n");
                writer.Write("}\n");
                writer.Write("</script>\n");

                writer.Write("<p align=\"left\">Please enter the following information about this zone. Fields marked with an asterisk (*) are required. All other fields are optional.</p>\n");
                writer.Write("<form action=\"editshippingZone.aspx?ShippingZoneID=" + ShippingZoneID.ToString() + "&edit=" + Editing.ToString() + "\" Method=\"post\" id=\"ShippingZoneForm\" name=\"ShippingZoneForm\" onsubmit=\"return (validateForm(this) && ShippingZoneForm_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"100%\" colspan=\"2\" align=\"left\">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Name:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Name"), "Name", false, true, true, "Please enter the shipping zone name", 100, 30, 0, 0, false));
                //writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"Name\" value=\"" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Name")) , "") + "\">\n");
                //writer.Write("                	<input type=\"hidden\" name=\"Name_vldt\" value=\"[req][blankalert=Please enter the shipping Zone name]\">\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");
                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td width=\"25%\" align=\"right\" valign=\"top\">*ZipCodes:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\" valign=\"top\">\n");
                writer.Write("Enter the target zip code prefixes to match against, e.g. 850, 851, 004-005, etc.<br/>Use a comma separate list of 3 digit prefixes.<br/>Ranges can be entered as NNN-MMM.<br/>");
                writer.Write("                	<textarea id=\"ZipCodes\" name=\"ZipCodes\" cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeightSmall") + "\">" + CommonLogic.IIF(Editing, Server.HtmlEncode(DB.RSField(rs, "ZipCodes")), "") + "</textarea>\n");
                writer.Write("                	</td>\n");
                writer.Write("              </tr>\n");

                writer.Write("<tr>\n");
                writer.Write("<td></td><td align=\"left\" valign=\"top\"><br/>\n");
                if (Editing)
                {
                    writer.Write("<input type=\"submit\" value=\"Update\" name=\"submit\">\n");
                }
                else
                {
                    writer.Write("<input type=\"submit\" value=\"Add New\" name=\"submit\">\n");
                }
                writer.Write("        </td>\n");
                writer.Write("      </tr>\n");
                writer.Write("  </table>\n");
                writer.Write("</form>\n");
            }
            rs.Close();
        }

    }
}
