// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.AspDotNetStorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/editorderoption.aspx.cs 2     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.IO;
using System.Web;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for editorderoption
	/// </summary>
    public partial class editorderoption : AspDotNetStorefront.SkinBase
	{
		
		int OrderOptionID;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache"); 
            

			OrderOptionID = 0;
			
			if(CommonLogic.QueryStringCanBeDangerousContent("OrderOptionID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("OrderOptionID") != "0") 
			{
				Editing = true;
				OrderOptionID = Localization.ParseUSInt(CommonLogic.QueryStringCanBeDangerousContent("OrderOptionID"));
			} 
			else 
			{
				Editing = false;
			}

			
			IDataReader rs;
			
			if(CommonLogic.FormBool("IsSubmit"))
			{
				StringBuilder sql = new StringBuilder(2500);
				decimal Cost = System.Decimal.Zero;
				if(CommonLogic.FormCanBeDangerousContent("Cost").Length != 0)
				{
					Cost = CommonLogic.FormUSDecimal("Cost");
				}
				if(!Editing)
				{
					// ok to add them:
					String NewGUID = DB.GetNewGUID();
					sql.Append("insert into OrderOption(OrderOptionGUID,Name,Description,DefaultIsChecked,Cost) values(");
					sql.Append(DB.SQuote(NewGUID) + ",");
					sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
					if(AppLogic.FormLocaleXml("Description").Length != 0)
					{
						sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Description")) + ",");
					}
					else
					{
						sql.Append("NULL,");
					}
                    sql.Append(CommonLogic.FormUSInt("DefaultIsChecked").ToString() + ",");
                    sql.Append(CommonLogic.IIF(Cost != System.Decimal.Zero, Localization.DecimalStringForDB(Cost), "0.0"));
					sql.Append(")");
					DB.ExecuteSQL(sql.ToString());

					rs = DB.GetRS("select OrderOptionID from OrderOption  " + DB.GetNoLock() + " where OrderOptionGUID=" + DB.SQuote(NewGUID));
					rs.Read();
					OrderOptionID = DB.RSFieldInt(rs,"OrderOptionID");
					Editing = true;
					rs.Close();
					DataUpdated = true;
				}
				else
				{
					// ok to update:
					sql.Append("update OrderOption set ");
					sql.Append("Name=" + DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
					if(AppLogic.FormLocaleXml("Description").Length != 0)
					{
						sql.Append("Description=" + DB.SQuote(AppLogic.FormLocaleXml("Description")) + ",");
					}
					else
					{
						sql.Append("Description=NULL,");
					}
                    sql.Append("DefaultIsChecked=" + CommonLogic.FormUSInt("DefaultIsChecked").ToString() + ",");
                    sql.Append("Cost=" + CommonLogic.IIF(Cost != System.Decimal.Zero, Localization.DecimalStringForDB(Cost), "0.0"));
					sql.Append(" where OrderOptionID=" + OrderOptionID.ToString());
					DB.ExecuteSQL(sql.ToString());
					DataUpdated = true;
					Editing = true;
				}
				// handle image uploaded:
				try
				{
					String Image1 = String.Empty;
					HttpPostedFile Image1File = Request.Files["Image1"];
					if(Image1File.ContentLength != 0)
					{
						// delete any current image file first
						try
						{
							System.IO.File.Delete(AppLogic.GetImagePath("OrderOption","icon",true) + OrderOptionID.ToString() + ".jpg");
							System.IO.File.Delete(AppLogic.GetImagePath("OrderOption","icon",true) + OrderOptionID.ToString() + ".gif");
							System.IO.File.Delete(AppLogic.GetImagePath("OrderOption","icon",true) + OrderOptionID.ToString() + ".png");
						}
						catch
						{}

						String s = Image1File.ContentType;
						switch(Image1File.ContentType)
						{
							case "image/gif":
								Image1 = AppLogic.GetImagePath("OrderOption","icon",true) + OrderOptionID.ToString() + ".gif";
								Image1File.SaveAs(Image1);
								break;
							case "image/x-png":
								Image1 = AppLogic.GetImagePath("OrderOption","icon",true) + OrderOptionID.ToString() + ".png";
								Image1File.SaveAs(Image1);
								break;
                            case "image/jpg":
                            case "image/jpeg":
							case "image/pjpeg":
								Image1 = AppLogic.GetImagePath("OrderOption","icon",true) + OrderOptionID.ToString() + ".jpg";
								Image1File.SaveAs(Image1);
								break;
						}
					}
				}
				catch(Exception ex)
				{
					ErrorMsg = CommonLogic.GetExceptionDetail(ex,"<br/>");
				}
			}
			SectionTitle = "<a href=\"OrderOptions.aspx\">OrderOptions</a> - Manage OrderOptions" + CommonLogic.IIF(DataUpdated , " (Updated)" , "");
		}

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
		{
			IDataReader rs = DB.GetRS("select * from OrderOption  " + DB.GetNoLock() + " where OrderOptionID=" + OrderOptionID.ToString());
			if(rs.Read())
			{
				Editing = true;
			}
			
			if(ErrorMsg.Length != 0)
			{
				writer.Write("<p><b><font color=red>" + ErrorMsg + "</font></b></p>\n");
			}


			if(ErrorMsg.Length == 0)
			{

				if(Editing)
				{
					writer.Write("<p><b>Editing OrderOption: " + DB.RSFieldByLocale(rs,"Name",ThisCustomer.LocaleSetting) + " (ID=" + DB.RSFieldInt(rs,"OrderOptionID").ToString() + ")<br/><br/></b>\n");
				}
				else
				{
					writer.Write("<b>Adding New OrderOption:<br/><br/></b>\n");
				}

				writer.Write("<script type=\"text/javascript\">\n");
				writer.Write("function Form_Validator(theForm)\n");
				writer.Write("{\n");
				writer.Write("submitonce(theForm);\n");
				writer.Write("return (true);\n");
				writer.Write("}\n");
				writer.Write("</script>\n");

				if(AppLogic.NumLocaleSettingsInstalled() > 1)
				{
					writer.Write(CommonLogic.ReadFile("jscripts/tabs.js",true));
				}
        
				writer.Write("<p>Please enter the following information about this orderoption. Fields marked with an asterisk (*) are required. All other fields are optional.</p>\n");
                writer.Write("<form enctype=\"multipart/form-data\" action=\"editorderoption.aspx?OrderOptionID=" + OrderOptionID.ToString() + "&edit=" + Editing.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(this) && Form_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n");
				writer.Write("              <tr valign=\"middle\">\n");
				writer.Write("                <td width=\"100%\" colspan=\"2\" align=\"left\">\n");
				writer.Write("                </td>\n");
				writer.Write("              </tr>\n");
				writer.Write("              <tr valign=\"middle\">\n");
				writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">*Order Option Name:&nbsp;&nbsp;</td>\n");
				writer.Write("                <td align=\"left\" valign=\"top\">\n");
				writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs,"Name"),"Name",false,true,true,"Please enter the order option name",100,30,0,0,false));
				//        writer.Write("                	<input maxLength=\"100\" size=\"30\" name=\"Name\" value=\"" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Name")) , "") + "\">\n");
				//				writer.Write("                	<input type=\"hidden\" name=\"Name_vldt\" value=\"[req][blankalert=Please enter the order option name]\">\n");
				writer.Write("                	</td>\n");
				writer.Write("              </tr>\n");

				writer.Write("              <tr valign=\"middle\">\n");
				writer.Write("                <td align=\"right\" valign=\"top\">Description:&nbsp;&nbsp;</td>\n");
				writer.Write("                <td align=\"left\" valign=\"top\">\n");
				writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs,"Description"),"Description",true,true,false,"",0,0,AppLogic.AppConfigUSInt("Admin_TextareaHeight"),AppLogic.AppConfigUSInt("Admin_TextareaWidth"),true));
				//        writer.Write("                	<textarea cols=\"60\" rows=\"20\" id=\"Description\" name=\"Description\">" + CommonLogic.IIF(Editing , Server.HtmlEncode(DB.RSField(rs,"Description")) , "") + "</textarea>\n");
				writer.Write("                </td>\n");
				writer.Write("              </tr>\n");

                writer.Write("              <tr valign=\"middle\">\n");
                writer.Write("                <td align=\"right\" valign=\"middle\">*Default Is Checked:&nbsp;&nbsp;</td>\n");
                writer.Write("                <td align=\"left\">\n");
                writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"DefaultIsChecked\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "DefaultIsChecked"), " checked ", ""), " checked ") + ">\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"DefaultIsChecked\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "DefaultIsChecked"), "", " checked "), "") + ">\n");
                writer.Write("                </td>\n");
                writer.Write("              </tr>\n");
                
                writer.Write("              <tr valign=\"middle\">\n");
				writer.Write("                <td width=\"25%\" align=\"right\" valign=\"middle\">Cost:&nbsp;&nbsp;</td>\n");
				writer.Write("                <td align=\"left\" valign=\"top\">\n");
				writer.Write("                	<input maxLength=\"10\" size=\"10\" name=\"Cost\" value=\"" + CommonLogic.IIF(Editing , CommonLogic.IIF(DB.RSFieldDecimal(rs,"Cost") != System.Decimal.Zero , Localization.CurrencyStringForDBWithoutExchangeRate( DB.RSFieldDecimal(rs,"Cost")) , "") , "") + "\"> (in x.xx format)\n");
				writer.Write("                	<input type=\"hidden\" name=\"Cost_vldt\" value=\"[number][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
				writer.Write("                	</td>\n");
				writer.Write("              </tr>\n");
				
				writer.Write("  <tr>\n");
				writer.Write("    <td valign=\"top\" align=\"right\">Icon:\n");
				writer.Write("</td>\n");
				writer.Write("    <td valign=\"top\" align=\"left\">");
				writer.Write("    <input type=\"file\" name=\"Image1\" size=\"30\" value=\"" + CommonLogic.IIF(Editing , "" , "") + "\">\n");
				String Image1URL = AppLogic.LookupImage("OrderOption",OrderOptionID,"icon",SkinID,ThisCustomer.LocaleSetting);
				if(Image1URL.Length != 0)
				{
					if(Image1URL.IndexOf("nopicture") == -1)
					{
						writer.Write("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image1URL + "','CatPic');\">Click here</a> to delete the current image<br/>\n");
					}
					writer.Write("<br/><img id=\"CatPic\" name=\"CatPic\" border=\"0\" src=\"" + Image1URL + "?" + CommonLogic.GetRandomNumber(1,1000000).ToString() + "\">\n");
				}
				writer.Write("</td>\n");
				writer.Write(" </tr>\n");
				
				writer.Write("<tr>\n");
				writer.Write("<td></td><td align=\"left\" valign=\"top\"><br/>\n");
				if(Editing) 
				{
					writer.Write("<input type=\"submit\" value=\"Update\" name=\"submit\">\n");
					writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"reset\" class=\"CPButton\" value=\"Reset\" name=\"reset\">\n");
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

			writer.Write("<script type=\"text/javascript\">\n");
			writer.Write("function DeleteImage(imgurl,name)\n");
			writer.Write("{\n");
			writer.Write("window.open('deleteimage.aspx?imgurl=' + imgurl + '&FormImageName=' + name,\"AspDotNetStorefrontAdmin_ML\",\"height=250,width=440,top=10,left=20,status=no,toolbar=no,menubar=no,scrollbars=yes,location=no\")\n");
			writer.Write("}\n");
			writer.Write("</SCRIPT>\n");
		}

	}
}
