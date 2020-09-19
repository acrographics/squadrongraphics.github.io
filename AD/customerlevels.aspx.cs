// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/customerlevels.aspx.cs 4     9/30/06 3:39p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for customerlevels.
    /// </summary>
    public partial class customerlevels : AspDotNetStorefront.SkinBase
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            if (CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
            {
                // delete the level:

                // clear the carts for all customers whose customer levels are being reassigned. This is to ensure their produce pricing is correct
                // their current cart can have customer level pricing, not retail pricing, and this prevents that:
                DB.ExecuteSQL("delete from shoppingcart where CartType in (" + ((int)CartTypeEnum.ShoppingCart).ToString() + "," + ((int)CartTypeEnum.GiftRegistryCart).ToString() + "," + ((int)CartTypeEnum.WishCart).ToString() + ") and customerid in (select customerid from customer where CustomerLevelID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID") + ")");
                DB.ExecuteSQL("delete from customcart where CartType in (" + ((int)CartTypeEnum.ShoppingCart).ToString() + "," + ((int)CartTypeEnum.GiftRegistryCart).ToString() + "," + ((int)CartTypeEnum.WishCart).ToString() + ") and customerid in (select customerid from customer where CustomerLevelID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID") + ")");
                DB.ExecuteSQL("delete from kitcart where CartType in (" + ((int)CartTypeEnum.ShoppingCart).ToString() + "," + ((int)CartTypeEnum.GiftRegistryCart).ToString() + "," + ((int)CartTypeEnum.WishCart).ToString() + ") and customerid in (select customerid from customer where CustomerLevelID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID") + ")");

                DB.ExecuteSQL("delete from ExtendedPrice where CustomerLevelID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID"));
                DB.ExecuteSQL("update Customer set CustomerLevelID=0 where CustomerLevelID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID"));
                DB.ExecuteSQL("delete from CustomerLevel where CustomerLevelID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID"));
                AppLogic.ClearCache();
            }
            SectionTitle = "Manage Customer Levels";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            DataSet ds = DB.GetDS("select * from CustomerLevel  " + DB.GetNoLock() + " where deleted=0 order by DisplayOrder,Name", false);
            writer.Write("<form method=\"POST\" action=\"CustomerLevels.aspx\">\n");
            writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
            writer.Write("<table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
            writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
            writer.Write("<td width=\"10%\"><b>ID</b></td>\n");
            writer.Write("<td ><b>Description</b></td>\n");
            writer.Write("<td width=\"20%\" align=\"center\"><b>View Customers Of This Level</b></td>\n");
            writer.Write("<td width=\"10%\" align=\"center\"><b>Edit</b></td>\n");
            writer.Write("<td width=\"10%\" align=\"center\"><b>Delete</b></td>\n");
            writer.Write("</tr>\n");
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                writer.Write("<td width=\"10%\">" + DB.RowFieldInt(row, "CustomerLevelID").ToString() + "</td>\n");
                writer.Write("<td><a href=\"editCustomerLevel.aspx?CustomerLevelID=" + DB.RowFieldInt(row, "CustomerLevelID").ToString() + "\">" + DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting) + "</a></td>\n");
                writer.Write("<td width=\"20%\" align=\"center\"><input type=\"button\" value=\"Show Customers\" name=\"Edit_" + DB.RowFieldInt(row, "CustomerLevelID").ToString() + "\" onClick=\"self.location='showCustomerLevel.aspx?CustomerLevelID=" + DB.RowFieldInt(row, "CustomerLevelID").ToString() + "'\"></td>\n");
                writer.Write("<td width=\"10%\" align=\"center\"><input type=\"button\" value=\"Edit\" name=\"Edit_" + DB.RowFieldInt(row, "CustomerLevelID").ToString() + "\" onClick=\"self.location='editCustomerLevel.aspx?CustomerLevelID=" + DB.RowFieldInt(row, "CustomerLevelID").ToString() + "'\"></td>\n");
                writer.Write("<td width=\"10%\" align=\"center\"><input type=\"button\" value=\"Delete\" name=\"Delete_" + DB.RowFieldInt(row, "CustomerLevelID").ToString() + "\" onClick=\"DeleteCustomerLevel(" + DB.RowFieldInt(row, "CustomerLevelID").ToString() + ")\"></td>\n");
                writer.Write("</tr>\n");
            }
            ds.Dispose();
            writer.Write(" </table>\n");
            writer.Write("<p align=\"left\"><input type=\"button\" value=\"Add New Customer Level\" name=\"AddNew\" onClick=\"self.location='editCustomerLevel.aspx';\"></p>\n");
            writer.Write("</form>\n");

            writer.Write("</center></b>\n");

            writer.Write("<script type=\"text/javascript\">\n");
            writer.Write("function DeleteCustomerLevel(id)\n");
            writer.Write("{\n");
            writer.Write("if(confirm('Are you sure you want to delete Customer Level: ' + id))\n");
            writer.Write("{\n");
            writer.Write("self.location = 'CustomerLevels.aspx?deleteid=' + id;\n");
            writer.Write("}\n");
            writer.Write("}\n");
            writer.Write("</SCRIPT>\n");
        }

    }
}
