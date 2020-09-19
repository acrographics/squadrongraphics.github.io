// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/orderoptions.aspx.cs 2     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for OrderOptions
    /// </summary>
    public partial class OrderOptions : AspDotNetStorefront.SkinBase
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache"); 
            

            SectionTitle = "Manage OrderOptions";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            if (CommonLogic.FormBool("IsSubmit"))
            {
                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    if (Request.Form.Keys[i].IndexOf("DisplayOrder_") != -1)
                    {
                        String[] keys = Request.Form.Keys[i].Split('_');
                        int OrderOptionID = Localization.ParseUSInt(keys[1]);
                        int DispOrd = 1;
                        try
                        {
                            DispOrd = Localization.ParseUSInt(Request.Form[Request.Form.Keys[i]]);
                        }
                        catch { }
                        DB.ExecuteSQL("update OrderOption set DisplayOrder=" + DispOrd.ToString() + " where OrderOptionID=" + OrderOptionID.ToString());
                    }
                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
            {
                // delete the mfg:
                DB.ExecuteSQL("delete from orderoption where OrderOptionID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID"));
            }

            DataSet ds = DB.GetDS("select * from OrderOption  " + DB.GetNoLock() + " order by DisplayOrder,Name", false);
            writer.Write("<form method=\"POST\" action=\"OrderOptions.aspx\">\n");
            writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
            writer.Write("<table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
            writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
            writer.Write("<td><b>ID</b></td>\n");
            writer.Write("<td><b>Order Option</b></td>\n");
            writer.Write("<td align=\"center\"><b>Display Order</b></td>\n");
            writer.Write("<td><b>Cost</b></td>\n");
            writer.Write("<td align=\"center\"><b>Edit</b></td>\n");
            writer.Write("<td align=\"center\"><b>Delete</b></td>\n");
            writer.Write("</tr>\n");
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                writer.Write("<td>" + DB.RowFieldInt(row, "OrderOptionID").ToString() + "</td>\n");
                writer.Write("<td><a href=\"editOrderOption.aspx?OrderOptionid=" + DB.RowFieldInt(row, "OrderOptionID").ToString() + "\">" + DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting) + "</a></td>\n");
                writer.Write("<td align=\"center\"><input size=4 type=\"text\" name=\"DisplayOrder_" + DB.RowFieldInt(row, "OrderOptionID").ToString() + "\" value=\"" + DB.RowFieldInt(row, "DisplayOrder").ToString() + "\"></td>\n");
                writer.Write("<td>" + ThisCustomer.CurrencyString(DB.RowFieldDecimal(row, "Cost")) + "</td>\n");
                writer.Write("<td align=\"center\"><input type=\"button\" value=\"Edit\" name=\"Edit_" + DB.RowFieldInt(row, "OrderOptionID").ToString() + "\" onClick=\"self.location='editOrderOption.aspx?OrderOptionid=" + DB.RowFieldInt(row, "OrderOptionID").ToString() + "'\"></td>\n");
                writer.Write("<td align=\"center\"><input type=\"button\" value=\"Delete\" name=\"Delete_" + DB.RowFieldInt(row, "OrderOptionID").ToString() + "\" onClick=\"DeleteOrderOption(" + DB.RowFieldInt(row, "OrderOptionID").ToString() + ")\"></td>\n");
                writer.Write("</tr>\n");
            }
            ds.Dispose();
            writer.Write("<tr>\n");
            writer.Write("<td colspan=\"2\" align=\"left\"></td>\n");
            writer.Write("<td align=\"center\" bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></td>\n");
            writer.Write("<td colspan=\"3\"></td>\n");
            writer.Write("</tr>\n");
            writer.Write("</table>\n");

            writer.Write("<p align=\"left\"><input type=\"button\" value=\"Add New Order Option\" name=\"AddNew\" onClick=\"self.location='editOrderOption.aspx';\"></p>\n");
            writer.Write("</form>\n");

            writer.Write("</center></b>\n");

            writer.Write("<script type=\"text/javascript\">\n");
            writer.Write("function DeleteOrderOption(id)\n");
            writer.Write("{\n");
            writer.Write("if(confirm('Are you sure you want to delete Order Option: ' + id))\n");
            writer.Write("{\n");
            writer.Write("self.location = 'OrderOptions.aspx?deleteid=' + id;\n");
            writer.Write("}\n");
            writer.Write("}\n");
            writer.Write("</SCRIPT>\n");
        }
    }
}
