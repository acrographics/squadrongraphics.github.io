// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/editkit.aspx.cs 9     10/04/06 6:22a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.IO;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for editkit
    /// </summary>
    public partial class editkit : AspDotNetStorefront.SkinBase
    {

        int ProductID;
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            ProductID = CommonLogic.QueryStringUSInt("ProductID");

            if (CommonLogic.QueryStringUSInt("DeleteGroupID") != 0)
            {
                // delete the group, and any items it contains:
                DB.ExecuteSQL("delete from kitcart where kitgroupid=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteGroupID"));
                DB.ExecuteSQL("delete from kititem where kitgroupid=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteGroupID"));
                DB.ExecuteSQL("delete from kitgroup where kitgroupid=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteGroupID") + " and ProductID=" + ProductID.ToString());
            }

            if (CommonLogic.QueryStringUSInt("DeleteItemID") != 0)
            {
                // delete the item:
                DB.ExecuteSQL("delete from kitcart where kititemid=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteItemID"));
                DB.ExecuteSQL("delete from kititem where kititemid=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteItemID"));
            }

            if (CommonLogic.QueryStringUSInt("DeleteItemID") != 0)
            {
                // delete the item:
                DB.ExecuteSQL("delete from kititem where kititemid=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteItemID"));
            }

            //int N = 0;
            if (CommonLogic.FormBool("IsSubmit"))
            {
                String NewGroupFormName = "NewGroupName_" + Localization.GetWebConfigLocale().Replace("-", "_");
                // are we adding a new group:
                if (AppLogic.NumLocaleSettingsInstalled() < 2)
                {
                    NewGroupFormName = "NewGroupName";
                }
                if (CommonLogic.FormCanBeDangerousContent(NewGroupFormName).Length != 0)
                {
                    String NewGUID = DB.GetNewGUID();
                    DB.ExecuteSQL("insert into KitGroup(KitGroupGUID,Name,Description,ProductID,DisplayOrder,KitGroupTypeID,IsRequired) values(" + DB.SQuote(NewGUID) + "," + DB.SQuote(AppLogic.FormLocaleXml("NewGroupName")) + "," + DB.SQuote(AppLogic.FormLocaleXml("NewGroupDescription")) + "," + ProductID.ToString() + "," + CommonLogic.FormUSInt("NewGroupDisplayOrder").ToString() + "," + CommonLogic.FormUSInt("NewGroupType").ToString() + "," + CommonLogic.FormUSInt("NewGroupIsRequired").ToString() + ")");
                }

                // add new group items:
                IDataReader rsg = DB.GetRS("select * from KitGroup  " + DB.GetNoLock() + " where ProductID=" + ProductID.ToString());
                while (rsg.Read())
                {
                    int ThisGroupID = DB.RSFieldInt(rsg, "KitGroupID");
                    String NewItemFormName = "NewItemName_" + DB.RSFieldInt(rsg, "KitGroupID").ToString() + "_" + Localization.GetWebConfigLocale().Replace("-", "_");
                    if (AppLogic.NumLocaleSettingsInstalled() < 2)
                    {
                        NewItemFormName = "NewItemName_" + DB.RSFieldInt(rsg, "KitGroupID").ToString();
                    }
                    if (CommonLogic.FormCanBeDangerousContent(NewItemFormName).Length != 0)
                    {
                        int DDDO = CommonLogic.FormUSInt("NewItemDisplayOrder_" + ThisGroupID.ToString());
                        decimal PriceDelta = System.Decimal.Zero;
                        if (CommonLogic.FormCanBeDangerousContent("NewItemPriceDelta_" + ThisGroupID.ToString()).Length != 0)
                        {
                            try
                            {
                                PriceDelta = CommonLogic.FormUSDecimal("NewItemPriceDelta_" + ThisGroupID.ToString());
                            }
                            catch { }
                        }
                        decimal WeightDelta = System.Decimal.Zero;
                        if (CommonLogic.FormCanBeDangerousContent("NewItemWeightDelta_" + ThisGroupID.ToString()).Length != 0)
                        {
                            try
                            {
                                WeightDelta = CommonLogic.FormUSDecimal("NewItemWeightDelta_" + ThisGroupID.ToString());
                            }
                            catch { }
                        }
                        String KIGUID = DB.GetNewGUID();
                        StringBuilder sql = new StringBuilder(1024);
                        sql.Append("insert into KitItem(KitItemGUID,KitGroupID,Name,Description,InventoryVariantID,InventoryVariantColor,InventoryVariantSize,PriceDelta,WeightDelta,DisplayOrder,IsDefault) values(");
                        sql.Append(DB.SQuote(KIGUID) + ",");
                        sql.Append(ThisGroupID.ToString() + ",");
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("NewItemName_" + ThisGroupID.ToString())) + ",");
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("NewItemDescription_" + ThisGroupID.ToString())) + ",");
                        sql.Append(CommonLogic.FormUSInt("NewItemInventoryVariantID_" + ThisGroupID.ToString()).ToString() + ",");
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("NewItemInventoryVariantColor_" + ThisGroupID.ToString())) + ",");
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("NewItemInventoryVariantSize_" + ThisGroupID.ToString())) + ",");
                        sql.Append(Localization.CurrencyStringForDBWithoutExchangeRate(PriceDelta) + ",");
                        sql.Append(Localization.CurrencyStringForDBWithoutExchangeRate(WeightDelta) + ",");
                        sql.Append(CommonLogic.FormUSInt("NewItemDisplayOrder_" + ThisGroupID.ToString()).ToString() + ",");
                        sql.Append(CommonLogic.FormUSInt("NewItemIsDefault_" + ThisGroupID.ToString()).ToString());
                        sql.Append(")");
                        DB.ExecuteSQL(sql.ToString());
                    }
                }
                rsg.Close();

                // update Groups:
                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    if (Request.Form.Keys[i].StartsWith("groupname", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int thisID = Localization.ParseUSInt(Request.Form.Keys[i].Split('_')[1]);
                        DB.ExecuteSQL("update KitGroup set Name=" + DB.SQuote(AppLogic.FormLocaleXml("GroupName_" + thisID.ToString())) + ",Description=" + DB.SQuote(AppLogic.FormLocaleXml("GroupDescription_" + thisID.ToString())) + ",DisplayOrder=" + CommonLogic.FormUSInt("GroupDisplayOrder_" + thisID.ToString()).ToString() + ",KitGroupTypeID=" + CommonLogic.FormCanBeDangerousContent("GroupType_" + thisID.ToString()) + ",IsRequired=" + CommonLogic.FormCanBeDangerousContent("GroupIsRequired_" + thisID.ToString()) + " where KitGroupID=" + thisID.ToString());
                    }
                }

                // update Items:
                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    if (Request.Form.Keys[i].StartsWith("itemname", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int thisID = Localization.ParseUSInt(Request.Form.Keys[i].Split('_')[1]);
                        decimal PriceDelta = System.Decimal.Zero;
                        try
                        {
                            PriceDelta = CommonLogic.FormUSDecimal("ItemPriceDelta_" + thisID.ToString());
                        }
                        catch { }
                        decimal WeightDelta = System.Decimal.Zero;
                        try
                        {
                            WeightDelta = CommonLogic.FormUSDecimal("ItemWeightDelta_" + thisID.ToString());
                        }
                        catch { }
                        DB.ExecuteSQL("update KitItem set Name=" + DB.SQuote(AppLogic.FormLocaleXml("ItemName_" + thisID.ToString())) + ",Description=" + DB.SQuote(AppLogic.FormLocaleXml("ItemDescription_" + thisID.ToString())) + ",InventoryVariantID=" + CommonLogic.FormUSInt("ItemInventoryVariantID_" + thisID.ToString()).ToString() + ",InventoryVariantColor=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ItemInventoryVariantColor_" + thisID.ToString())) + ",InventoryVariantSize=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ItemInventoryVariantSize_" + thisID.ToString())) + ",DisplayOrder=" + CommonLogic.FormUSInt("ItemDisplayOrder_" + thisID.ToString()).ToString() + ",IsDefault=" + CommonLogic.FormCanBeDangerousContent("ItemIsDefault_" + thisID.ToString()) + ",PriceDelta=" + Localization.CurrencyStringForDBWithoutExchangeRate(PriceDelta) + ", WeightDelta=" + Localization.CurrencyStringForDBWithoutExchangeRate(WeightDelta) + " where KitItemID=" + thisID.ToString());
                    }
                }
            }
            SectionTitle = "<a href=\"editproduct.aspx?productid=" + ProductID.ToString() + "\">Product</a> - Edit Kit" + CommonLogic.IIF(DataUpdated, " (Updated)", "");
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            if (ErrorMsg.Length != 0)
            {
                writer.Write("<p align=\"left\"><b><font color=red>" + ErrorMsg + "</font></b></p>\n");
            }

            if (AppLogic.NumLocaleSettingsInstalled() > 1)
            {
                writer.Write(CommonLogic.ReadFile("jscripts/tabs.js", true));
            }

            if (ErrorMsg.Length == 0)
            {

                writer.Write("<p align=\"left\"><b>Within Product: <a href=\"editproduct.aspx?productid=" + ProductID.ToString() + "\">" + AppLogic.GetProductName(ProductID, ThisCustomer.LocaleSetting) + "</a> (Product SKU=" + AppLogic.GetProductSKU(ProductID) + ", ProductID=" + ProductID.ToString() + ")</b</p>\n");
                writer.Write("<p align=\"left\"><b>Editing Kit: " + AppLogic.GetProductName(ProductID, ThisCustomer.LocaleSetting) + ", ProductID=" + ProductID.ToString() + ")</b></p>\n");

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function Form_Validator(theForm)\n");
                writer.Write("{\n");
                writer.Write("submitonce(theForm);\n");
                writer.Write("return (true);\n");
                writer.Write("}\n");
                writer.Write("</script>\n");

                writer.Write("<p align=\"left\">Please enter the following information about this kit. Kits are composed of groups, and groups are composed of items. Each item can have a price and weight delta applied to the base kit (product) price or weight.</p>\n");
                writer.Write("<form enctype=\"multipart/form-data\" action=\"editkit.aspx?productid=" + ProductID.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(this) && Form_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n");
                writer.Write("<tr valign=\"middle\">\n");
                writer.Write("<td width=\"100%\" colspan=\"2\" align=\"left\">\n");
                writer.Write("</td>\n");
                writer.Write("</tr>\n");

                writer.Write("<tr>\n");
                writer.Write("<td></td><td align=\"left\" valign=\"top\"><br/>\n");
                writer.Write("<input type=\"submit\" value=\"Update\" name=\"submit\">\n");
                writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"reset\" value=\"Reset\" name=\"reset\">\n");
                writer.Write("</td>\n");
                writer.Write("</tr>\n");

                IDataReader rs = DB.GetRS("select * from KitGroup  " + DB.GetNoLock() + " where ProductID=" + ProductID.ToString() + " order by DisplayOrder,Name");
                int i = 1;
                while (rs.Read())
                {
                    writer.Write("<tr><td colspan=\"2\" bgcolor=\"" + CommonLogic.IIF(i % 2 == 0, "#FFFFFF", "#EEEEEE") + "\"><font style=\"font-size: 14px; font-weight: bold\">Group: " + Security.HtmlEncode(DB.RSFieldByLocale(rs, "Name", ThisCustomer.LocaleSetting)) + "</font></td></tr>");
                    writer.Write("<tr><td colspan=\"2\" bgcolor=\"" + CommonLogic.IIF(i % 2 == 0, "#FFFFFF", "#EEEEEE") + "\">");
                    int ThisGroupID = DB.RSFieldInt(rs, "KitGroupID");
                    writer.Write("Name: ");
                    writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Name"), "GroupName_" + ThisGroupID.ToString(), false, true, true, "Please enter the group name", 200, 75, 0, 0, false));
                    //writer.Write("<input maxLength=\"100\" size=\"40\" name=\"GroupName_" + ThisGroupID.ToString() + "\" value=\"" + Security.HtmlEncode(DB.RSFieldByLocale(rs,"Name",ThisCustomer.LocaleSetting)) + "\">");
                    writer.Write("&nbsp;&nbsp;\n");
                    writer.Write("Description: ");
                    writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Description"), "GroupDescription_" + ThisGroupID.ToString(), true, true, false, "", 2000, 40, 5, 90, false));
                    //writer.Write("<input maxLength=\"1000\" size=\"40\" name=\"GroupDescription_" + ThisGroupID.ToString() + "\" value=\"" + Security.HtmlEncode(DB.RSFieldByLocale(rs,"Description",ThisCustomer.LocaleSetting)) + "\">");
                    writer.Write("&nbsp;&nbsp;\n");
                    writer.Write("Display Order: <input maxLength=\"3\" size=\"5\" name=\"GroupDisplayOrder_" + ThisGroupID.ToString() + "\" value=\"" + DB.RSFieldInt(rs, "DisplayOrder").ToString() + "\"><input type=\"hidden\" name=\"GroupDisplayOrder_" + ThisGroupID.ToString() + "_vldt\" value=\"[number][blankalert=Please enter an integer number\">");
                    writer.Write("&nbsp;&nbsp;");
                    writer.Write("<select size=\"1\" name=\"GroupType_" + ThisGroupID.ToString() + "\">\n");
                    IDataReader rsst = DB.GetRS("select * from KitGroupType  " + DB.GetNoLock() + " order by DisplayOrder,Name");
                    while (rsst.Read())
                    {
                        writer.Write("<option value=\"" + DB.RSFieldInt(rsst, "KitGroupTypeID").ToString() + "\"" + CommonLogic.IIF(DB.RSFieldInt(rs, "KitGroupTypeID") == DB.RSFieldInt(rsst, "KitGroupTypeID"), " selected ", "") + ">" + DB.RSFieldByLocale(rsst, "Name", ThisCustomer.LocaleSetting) + "</option>");
                    }
                    writer.Write("</select>");
                    writer.Write("&nbsp;&nbsp;");
                    rsst.Close();
                    writer.Write("Is Required: Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"GroupIsRequired_" + ThisGroupID.ToString() + "\" value=\"1\" " + CommonLogic.IIF(DB.RSFieldBool(rs, "IsRequired"), " checked ", "") + ">\n");
                    writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"GroupIsRequired_" + ThisGroupID.ToString() + "\" value=\"0\" " + CommonLogic.IIF(DB.RSFieldBool(rs, "IsRequired"), "", " checked ") + ">");
                    writer.Write("&nbsp;&nbsp;\n");
                    writer.Write("<input type=\"button\" value=\"Delete This Group\" name=\"DeleteGroup_" + ThisGroupID.ToString() + "\" onClick=\"DeleteGroup(" + ThisGroupID.ToString() + ")\">\n");
                    writer.Write("</td></tr>");

                    // ITEMS:
                    writer.Write("<tr><td colspan=2 bgcolor=\"" + CommonLogic.IIF(i % 2 == 0, "#FFFFFF", "#EEEEEE") + "\">Items In This Group:</td></tr>");
                    writer.Write("<tr><td colspan=2 bgcolor=\"" + CommonLogic.IIF(i % 2 == 0, "#FFFFFF", "#EEEEEE") + "\">");
                    writer.Write("<table width=\"100%\" cellpadding=\"8\" cellspacing=\"0\" border=\"0\">");
                    writer.Write("<tr><td><b>KitItemID</b></td><td><b>Name</b></td><td><b>Description</b></td><td><b>Inventory VariantID</b></td><td><b>Inventory Color</b></td><td><b>Inventory Size</b></td><td><b>Delta Price</b></td><td><b>Delta Weight</b></td><td><b>Display order</b></td><td><b>Is Default</b></td><td><b>Delete Item</b></td></tr>");
                    IDataReader rsi = DB.GetRS("select * from KitItem  " + DB.GetNoLock() + " where KitGroupID=" + ThisGroupID.ToString() + " order by DisplayOrder,Name");
                    while (rsi.Read())
                    {
                        writer.Write("<tr>");
                        writer.Write("<td>" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "</td>");
                        writer.Write("<td valign=\"top\">");
                        writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rsi, "Name"), "ItemName_" + DB.RSFieldInt(rsi, "KitItemID").ToString(), false, true, true, "Each kit item must have a name", 200, 50, 0, 0, false));
                        //writer.Write("<input maxLength=\"100\" size=\"40\" name=\"ItemName_" + DB.RSFieldInt(rsi,"KitItemID").ToString() + "\" value=\"" + Security.HtmlEncode(DB.RSFieldByLocale(rsi,"Name",ThisCustomer.LocaleSetting)) + "\">");
                        //writer.Write("<input type=\"hidden\" name=\"ItemName_" + DB.RSFieldInt(rsi,"KitItemID").ToString() + "_vldt\" value=\"[req][blankalert=Each kit item must have a name!]\">");
                        writer.Write("</td>");
                        writer.Write("<td>");
                        writer.Write(AppLogic.GetLocaleEntryFields(DB.RSField(rsi, "Description"), "ItemDescription_" + DB.RSFieldInt(rsi, "KitItemID").ToString(), true, true, false, "", 2000, 40, 3, 40, false));
                        //writer.Write("<input maxLength=\"2000\" size=\"40\" name=\"ItemDescription_" + DB.RSFieldInt(rsi,"KitItemID").ToString() + "\" value=\"" + Security.HtmlEncode(DB.RSFieldByLocale(rsi,"Description",ThisCustomer.LocaleSetting)) + "\">");
                        writer.Write("</td>");
                        writer.Write("<td>");
                        writer.Write("<input maxLength=\"10\" size=\"4\" name=\"ItemInventoryVariantID_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" value=\"" + DB.RSFieldInt(rsi, "InventoryVariantID").ToString() + "\">");
                        writer.Write("</td>");
                        writer.Write("<td>");
                        writer.Write("<input maxLength=\"100\" size=\"20\" name=\"ItemInventoryVariantColor_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" value=\"" + DB.RSField(rsi, "InventoryVariantColor") + "\">");
                        writer.Write("</td>");
                        writer.Write("<td>");
                        writer.Write("<input maxLength=\"100\" size=\"20\" name=\"ItemInventoryVariantSize_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" value=\"" + DB.RSField(rsi, "InventoryVariantSize") + "\">");
                        writer.Write("</td>");
                        writer.Write("<td>");
                        writer.Write("<input maxLength=\"10\" size=\"10\" name=\"ItemPriceDelta_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RSFieldDecimal(rsi, "PriceDelta")) + "\"> (in x.xx format)");
                        writer.Write("<input type=\"hidden\" name=\"ItemPriceDelta_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "_vldt\" value=\"[req][number][blankalert=Please enter the item delta price][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                        writer.Write("</td>");
                        writer.Write("<td>");
                        writer.Write("<input maxLength=\"10\" size=\"10\" name=\"ItemWeightDelta_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RSFieldDecimal(rsi, "WeightDelta")) + "\"> (in x.xx format)");
                        writer.Write("<input type=\"hidden\" name=\"ItemWeightDelta_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "_vldt\" value=\"[req][number][blankalert=Please enter the item delta weight][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                        writer.Write("</td>");
                        writer.Write("<td><input maxLength=\"5\" size=\"10\" name=\"ItemDisplayOrder_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" value=\"" + DB.RSFieldInt(rsi, "DisplayOrder").ToString() + "\"></td>");
                        writer.Write("<td>");
                        writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"ItemIsDefault_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" value=\"1\" " + CommonLogic.IIF(DB.RSFieldBool(rsi, "IsDefault"), " checked ", "") + ">\n");
                        writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"ItemIsDefault_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" value=\"0\" " + CommonLogic.IIF(DB.RSFieldBool(rsi, "IsDefault"), "", " checked ") + ">&nbsp;&nbsp;\n");
                        writer.Write("</td>");
                        writer.Write("<td>");
                        writer.Write("<input type=\"button\" value=\"Delete This Item\" name=\"DeleteItem_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" onClick=\"DeleteItem(" + DB.RSFieldInt(rsi, "KitItemID").ToString() + ")\">\n");
                        writer.Write("</td>");
                        writer.Write("<tr>");
                    }
                    rsi.Close();
                    // new item row:
                    writer.Write("<tr>");
                    writer.Write("<td>Add New Item:</td>");
                    writer.Write("<td valign=\"top\">");
                    writer.Write(AppLogic.GetLocaleEntryFields(String.Empty, "NewItemName_" + ThisGroupID.ToString(), false, false, false, "", 200, 50, 0, 0, false));
                    //writer.Write("<input maxLength=\"100\" size=\"40\" name=\"NewItemName_" + ThisGroupID.ToString() + "\">");
                    writer.Write("</td>");
                    writer.Write("<td>");
                    writer.Write(AppLogic.GetLocaleEntryFields(String.Empty, "NewItemDescription_" + ThisGroupID.ToString(), true, false, false, "", 2000, 40, 3, 40, false));
                    //writer.Write("<input maxLength=\"2000\" size=\"40\" name=\"NewItemDescription_" + ThisGroupID.ToString() + "\">");
                    writer.Write("</td>");
                    writer.Write("<td>");
                    writer.Write("<input maxLength=\"10\" size=\"5\" name=\"NewItemInventoryVariantID_" + ThisGroupID.ToString() + "\">");
                    writer.Write("</td>");
                    writer.Write("<td>");
                    writer.Write("<input maxLength=\"100\" size=\"20\" name=\"NewItemInventoryVariantColor_" + ThisGroupID.ToString() + "\">");
                    writer.Write("</td>");
                    writer.Write("<td>");
                    writer.Write("<input maxLength=\"100\" size=\"20\" name=\"NewItemInventoryVariantSize_" + ThisGroupID.ToString() + "\">");
                    writer.Write("</td>");
                    writer.Write("<td>");
                    writer.Write("<input maxLength=\"10\" size=\"10\" name=\"NewItemPriceDelta_" + ThisGroupID.ToString() + "\"> (in x.xx format)");
                    writer.Write("<input type=\"hidden\" name=\"NewItemPriceDelta_" + ThisGroupID.ToString() + "_vldt\" value=\"[number][blankalert=Please enter the item delta price][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                    writer.Write("</td>");
                    writer.Write("<td>");
                    writer.Write("<input maxLength=\"10\" size=\"10\" name=\"NewItemWeightDelta_" + ThisGroupID.ToString() + "\"> (in x.xx format)");
                    writer.Write("<input type=\"hidden\" name=\"NewItemWeightDelta_" + ThisGroupID.ToString() + "_vldt\" value=\"[number][blankalert=Please enter the item delta weight][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                    writer.Write("</td>");
                    writer.Write("<td><input maxLength=\"5\" size=\"10\" name=\"NewItemDisplayOrder_" + ThisGroupID.ToString() + "\" value=\"1\"></td>");
                    writer.Write("<td>");
                    writer.Write("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"NewItemIsDefault_" + ThisGroupID.ToString() + "\" value=\"1\">\n");
                    writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"NewItemIsDefault_" + ThisGroupID.ToString() + "\" value=\"0\" checked >\n");
                    writer.Write("</td>");
                    writer.Write("<td>N/A</td>");
                    writer.Write("<tr>");

                    writer.Write("</table>");
                    writer.Write("</td></tr>");
                    writer.Write("<tr><td colspan=2 height=10 bgcolor=\"" + CommonLogic.IIF(i % 2 == 0, "#FFFFFF", "#EEEEEE") + "\"><img src=\"images/spacer.gif\" width=\"1\" height=\"20\"></td></tr>");

                    i++;
                }
                rs.Close();


                writer.Write("<tr><td colspan=\"2\" bgcolor=\"" + CommonLogic.IIF(i % 2 == 0, "#FFFFFF", "#EEEEEE") + "\">");
                writer.Write("<hr size=\"1\"/>");
                writer.Write("<p align=\"left\"><b>ADD NEW GROUP:</b></p>");
                writer.Write("Group Name: ");
                writer.Write(AppLogic.GetLocaleEntryFields(String.Empty, "NewGroupName", false, false, false, "", 200, 75, 0, 0, false));
                //writer.Write("<input maxLength=\"100\" size=\"50\" name=\"NewGroupName\">");
                writer.Write("&nbsp;&nbsp;\n");
                writer.Write("Description: ");
                writer.Write(AppLogic.GetLocaleEntryFields(String.Empty, "NewGroupDescription", true, false, false, "", 2000, 0, 5, 90, false));
                //writer.Write("<input maxLength=\"1000\" size=\"50\" name=\"NewGroupDescription\">");
                writer.Write("&nbsp;&nbsp;\n");
                writer.Write("Display Order: <input maxLength=\"3\" size=\"5\" name=\"NewGroupDisplayOrder\" value=\"1\"><input type=\"hidden\" name=\"NewGroupDisplayOrder_vldt\" value=\"[number][blankalert=Please enter an integer number\">&nbsp;&nbsp;");
                writer.Write("<select size=\"1\" name=\"NewGroupType\">\n");
                IDataReader rsst3 = DB.GetRS("select * from KitGroupType  " + DB.GetNoLock() + " order by DisplayOrder,Name");
                while (rsst3.Read())
                {
                    writer.Write("<option value=\"" + DB.RSFieldInt(rsst3, "KitGroupTypeID").ToString() + "\">" + DB.RSFieldByLocale(rsst3, "Name", ThisCustomer.LocaleSetting) + "</option>");
                }
                writer.Write("</select>&nbsp;&nbsp;");
                rsst3.Close();
                writer.Write("Is Required: Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"NewGroupIsRequired\" value=\"1\" checked>\n");
                writer.Write("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"NewGroupIsRequired\" value=\"0\" >\n");
                writer.Write("</td></tr>");

                writer.Write("<tr>\n");
                writer.Write("<td></td><td align=\"left\" valign=\"top\"><br/>\n");
                writer.Write("<input type=\"submit\" value=\"Update\" name=\"submit\">\n");
                writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"reset\" value=\"Reset\" name=\"reset\">\n");
                writer.Write("</td>\n");
                writer.Write("</tr>\n");
                writer.Write("</table>\n");
                writer.Write("</form>\n");

            }

            writer.Write("<script type=\"text/javascript\">\n");
            writer.Write("function DeleteGroup(id)\n");
            writer.Write("{\n");
            writer.Write("if(confirm('Are you sure you want to delete Group: ' + id + ' from this kit? This will also delete any items that are in this group!'))\n");
            writer.Write("{\n");
            writer.Write("self.location = 'editkit.aspx?deletegroupid=' + id + '&productid=" + ProductID.ToString() + "';\n");
            writer.Write("}\n");
            writer.Write("}\n");
            writer.Write("function DeleteItem(id)\n");
            writer.Write("{\n");
            writer.Write("if(confirm('Are you sure you want to delete Item: ' + id + ' from this group?'))\n");
            writer.Write("{\n");
            writer.Write("self.location = 'editkit.aspx?deleteitemid=' + id + '&productid=" + ProductID.ToString() + "';\n");
            writer.Write("}\n");
            writer.Write("}\n");
            writer.Write("</SCRIPT>\n");
        }

    }
}
