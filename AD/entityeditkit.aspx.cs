// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/entityeditkit.aspx.cs 5     10/03/06 7:56p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.IO;
using AspDotNetStorefrontCommon;
using System.Web.UI.WebControls;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for entityeditkit
    /// </summary>
    public partial class entityeditkit : System.Web.UI.Page
    {
        private int ProductID;
        private Customer cust;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            ProductID = CommonLogic.QueryStringUSInt("ProductID");
            cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            if (!IsPostBack)
            {
                loadScript(true);
                ltProduct.Text = "<a href=\"entityEditProducts.aspx?iden=" + ProductID + "&entityName=" + CommonLogic.QueryStringCanBeDangerousContent("entityName") + "&entityFilterID=" + CommonLogic.QueryStringCanBeDangerousContent("entityFilterID") + "\">" + AppLogic.GetProductName(ProductID, cust.LocaleSetting) + " (" + ProductID + ")</a>";
            }

            if (CommonLogic.QueryStringUSInt("DeleteGroupID") != 0)
            {
                // delete the group, and any items it contains:
                DB.ExecuteSQL("delete from kitcart where kitgroupid=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteGroupID"));
                DB.ExecuteSQL("delete from kititem where kitgroupid=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteGroupID"));
                DB.ExecuteSQL("delete from kitgroup where kitgroupid=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteGroupID") + " and ProductID=" + ProductID.ToString());
                resetError("Kit Group Deleted.", false);
            }

            if (CommonLogic.QueryStringUSInt("DeleteItemID") != 0)
            {
                // delete the item:
                DB.ExecuteSQL("delete from kitcart where kititemid=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteItemID"));
                DB.ExecuteSQL("delete from kititem where kititemid=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteItemID"));
                resetError("Kit Item Deleted.", false);
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
                        String sql = "insert into KitItem(KitItemGUID,KitGroupID,Name,Description,InventoryVariantID,InventoryVariantColor,InventoryVariantSize,PriceDelta,WeightDelta,DisplayOrder,IsDefault) values(";
                        sql += DB.SQuote(KIGUID) + ",";
                        sql += ThisGroupID.ToString() + ",";
                        sql += DB.SQuote(AppLogic.FormLocaleXml("NewItemName_" + ThisGroupID.ToString())) + ",";
                        sql += DB.SQuote(AppLogic.FormLocaleXml("NewItemDescription_" + ThisGroupID.ToString())) + ",";
                        sql += CommonLogic.FormUSInt("NewItemInventoryVariantID_" + ThisGroupID.ToString()).ToString() + ",";
                        sql += DB.SQuote(CommonLogic.FormCanBeDangerousContent("NewItemInventoryVariantColor_" + ThisGroupID.ToString())) + ",";
                        sql += DB.SQuote(CommonLogic.FormCanBeDangerousContent("NewItemInventoryVariantSize_" + ThisGroupID.ToString())) + ",";
                        sql += Localization.CurrencyStringForDBWithoutExchangeRate(PriceDelta) + ",";
                        sql += Localization.CurrencyStringForDBWithoutExchangeRate(WeightDelta) + ",";
                        sql += CommonLogic.FormUSInt("NewItemDisplayOrder_" + ThisGroupID.ToString()).ToString() + ",";
                        sql += CommonLogic.FormUSInt("NewItemIsDefault_" + ThisGroupID.ToString()).ToString();
                        sql += ")";
                        DB.ExecuteSQL(sql);
                    }

                    resetError("Kit Items Added.", false);
                }
                rsg.Close();

                // update Groups:
                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    if (Request.Form.Keys[i].StartsWith("groupname", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int thisID = Localization.ParseUSInt(Request.Form.Keys[i].Split('_')[1]);
                        DB.ExecuteSQL("update KitGroup set Name=" + DB.SQuote(AppLogic.FormLocaleXml("GroupName_" + thisID.ToString())) + ",Description=" + DB.SQuote(AppLogic.FormLocaleXml("GroupDescription_" + thisID.ToString())) + ",DisplayOrder=" + CommonLogic.FormUSInt("GroupDisplayOrder_" + thisID.ToString()).ToString() + ",KitGroupTypeID=" + CommonLogic.FormCanBeDangerousContent("GroupType_" + thisID.ToString()) + ",IsRequired=" + CommonLogic.FormCanBeDangerousContent("GroupIsRequired_" + thisID.ToString()) + " where KitGroupID=" + thisID.ToString());
                        resetError("Kit Group Updated.", false);
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
                        DB.ExecuteSQL("update KitItem set Name=" + DB.SQuote(AppLogic.FormLocaleXml("ItemName_" + thisID.ToString())) + ",Description=" + DB.SQuote(AppLogic.FormLocaleXml("ItemDescription_" + thisID.ToString())) + ",InventoryVariantID=" + CommonLogic.FormUSInt("ItemInventoryVariantID_" + thisID.ToString()).ToString() + ",InventoryVariantColor=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ItemInventoryVariantColor_" + thisID.ToString())) + ",InventoryVariantSize=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ItemInventoryVariantSize_" + thisID.ToString())) + ",DisplayOrder=" + CommonLogic.FormUSInt("ItemDisplayOrder_" + thisID.ToString()).ToString() + ",IsDefault=" + CommonLogic.FormCanBeDangerousContent("ItemIsDefault_" + thisID.ToString()) + ",PriceDelta=" + Localization.CurrencyStringForDBWithoutExchangeRate(PriceDelta) + " ,WeightDelta=" + Localization.CurrencyStringForDBWithoutExchangeRate(WeightDelta) + " where KitItemID=" + thisID.ToString());
                        resetError("Kit Item Updated.", false);
                    }
                }
            }

            LoadContent();
        }

        public void LoadContent()
        {
            StringBuilder tmpS = new StringBuilder(4096);

            tmpS.Append("<script type=\"text/javascript\">\n");
            tmpS.Append("function Form_Validator(theForm)\n");
            tmpS.Append("{\n");
            tmpS.Append("submitonce(theForm);\n");
            tmpS.Append("return (true);\n");
            tmpS.Append("}\n");
            tmpS.Append("</script>\n");

            tmpS.Append("<div style=\"padding-top: 3px; padding-bottom: 5px;\">Please enter the following information about this kit. Kits are composed of groups, and groups are composed of items. Each item can have a price and weight delta applied to the base kit (product) price or weight.</div>\n");
            tmpS.Append("<form enctype=\"multipart/form-data\" action=\"entityeditkit.aspx?productid=" + ProductID.ToString() + "&entityName=" + CommonLogic.QueryStringCanBeDangerousContent("entityName") + "&entityFilterID=" + CommonLogic.QueryStringCanBeDangerousContent("entityFilterID") + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(this) && Form_Validator(this))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
            tmpS.Append("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
            tmpS.Append("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n");
            tmpS.Append("<tr valign=\"middle\">\n");
            tmpS.Append("<td width=\"100%\" colspan=\"2\" align=\"left\">\n");
            tmpS.Append("</td>\n");
            tmpS.Append("</tr>\n");

            //tmpS.Append("<tr>\n");
            //tmpS.Append("<td></td><td align=\"left\" valign=\"top\"><br/>\n");
            //tmpS.Append("<input type=\"submit\" value=\"Update\" name=\"submit\">\n");
            //tmpS.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"reset\" value=\"Reset\" name=\"reset\">\n");
            //tmpS.Append("</td>\n");
            //tmpS.Append("</tr>\n");

            IDataReader rs = DB.GetRS("select * from KitGroup  " + DB.GetNoLock() + " where ProductID=" + ProductID.ToString() + " order by DisplayOrder,Name");
            int i = 1;
            while (rs.Read())
            {
                tmpS.Append("<tr><td colspan=\"2\" bgcolor=\"" + CommonLogic.IIF(i % 2 == 0, "#FFFFFF", "#EEEEEE") + "\"><font style=\"font-size: 14px; font-weight: bold\">Group: " + Security.HtmlEncode(DB.RSFieldByLocale(rs, "Name", cust.LocaleSetting)) + "</font></td></tr>");
                tmpS.Append("<tr><td colspan=\"2\" bgcolor=\"" + CommonLogic.IIF(i % 2 == 0, "#FFFFFF", "#EEEEEE") + "\">");
                int ThisGroupID = DB.RSFieldInt(rs, "KitGroupID");
                tmpS.Append("Name: ");
                tmpS.Append(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Name"), "GroupName_" + ThisGroupID.ToString(), false, true, true, "Please enter the group name", 100, 40, 0, 0, false));
                //tmpS.Append("<input maxLength=\"100\" size=\"40\" name=\"GroupName_" + ThisGroupID.ToString() + "\" value=\"" + Security.HtmlEncode(DB.RSFieldByLocale(rs,"Name",cust.LocaleSetting)) + "\">");
                tmpS.Append("&nbsp;&nbsp;\n");
                tmpS.Append("Description: ");
                tmpS.Append(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Description"), "GroupDescription_" + ThisGroupID.ToString(), false, true, false, "", 2000, 40, 0, 0, false));
                //tmpS.Append("<input maxLength=\"1000\" size=\"40\" name=\"GroupDescription_" + ThisGroupID.ToString() + "\" value=\"" + Security.HtmlEncode(DB.RSFieldByLocale(rs,"Description",cust.LocaleSetting)) + "\">");
                tmpS.Append("&nbsp;&nbsp;\n");
                tmpS.Append("Display Order: <input class=\"default\" maxLength=\"3\" size=\"5\" name=\"GroupDisplayOrder_" + ThisGroupID.ToString() + "\" value=\"" + DB.RSFieldInt(rs, "DisplayOrder").ToString() + "\"><input type=\"hidden\" name=\"GroupDisplayOrder_" + ThisGroupID.ToString() + "_vldt\" value=\"[number][blankalert=Please enter an integer number\">");
                tmpS.Append("&nbsp;&nbsp;");
                tmpS.Append("<select class=\"default\" size=\"1\" name=\"GroupType_" + ThisGroupID.ToString() + "\">\n");
                IDataReader rsst = DB.GetRS("select * from KitGroupType  " + DB.GetNoLock() + " order by DisplayOrder,Name");
                while (rsst.Read())
                {
                    tmpS.Append("<option value=\"" + DB.RSFieldInt(rsst, "KitGroupTypeID").ToString() + "\"" + CommonLogic.IIF(DB.RSFieldInt(rs, "KitGroupTypeID") == DB.RSFieldInt(rsst, "KitGroupTypeID"), " selected ", "") + ">" + DB.RSFieldByLocale(rsst, "Name", cust.LocaleSetting) + "</option>");
                }
                tmpS.Append("</select>");
                tmpS.Append("&nbsp;&nbsp;");
                rsst.Close();
                tmpS.Append("Is Required: Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"GroupIsRequired_" + ThisGroupID.ToString() + "\" value=\"1\" " + CommonLogic.IIF(DB.RSFieldBool(rs, "IsRequired"), " checked ", "") + ">\n");
                tmpS.Append("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"GroupIsRequired_" + ThisGroupID.ToString() + "\" value=\"0\" " + CommonLogic.IIF(DB.RSFieldBool(rs, "IsRequired"), "", " checked ") + ">");
                tmpS.Append("&nbsp;&nbsp;\n");
                tmpS.Append("<input class=\"normalButton\" type=\"button\" value=\"Delete This Group\" name=\"DeleteGroup_" + ThisGroupID.ToString() + "\" onClick=\"DeleteGroup(" + ThisGroupID.ToString() + ")\">\n");
                tmpS.Append("</td></tr>");

                // ITEMS:
                tmpS.Append("<tr><td colspan=2 bgcolor=\"" + CommonLogic.IIF(i % 2 == 0, "#FFFFFF", "#EEEEEE") + "\">Items In This Group:</td></tr>");
                tmpS.Append("<tr><td colspan=2 bgcolor=\"" + CommonLogic.IIF(i % 2 == 0, "#FFFFFF", "#EEEEEE") + "\">");
                tmpS.Append("<table width=\"100%\" cellpadding=\"8\" cellspacing=\"0\" border=\"0\">");
                tmpS.Append("<tr><td><b>KitItemID</b></td><td><b>Name</b></td><td><b>Description</b></td><td><b>Inventory VariantID</b></td><td><b>Inventory Color</b></td><td><b>Inventory Size</b></td><td><b>Delta Price</b></td><td><b>Delta Weight</b></td><td><b>Display order</b></td><td><b>Is Default</b></td><td><b>Delete Item</b></td></tr>");
                IDataReader rsi = DB.GetRS("select * from KitItem  " + DB.GetNoLock() + " where KitGroupID=" + ThisGroupID.ToString() + " order by DisplayOrder,Name");
                while (rsi.Read())
                {
                    tmpS.Append("<tr>");
                    tmpS.Append("<td>" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "</td>");
                    tmpS.Append("<td>");
                    tmpS.Append(AppLogic.GetLocaleEntryFields(DB.RSField(rsi, "Name"), "ItemName_" + DB.RSFieldInt(rsi, "KitItemID").ToString(), false, true, true, "Each kit item must have a name", 100, 40, 0, 0, false));
                    //tmpS.Append("<input maxLength=\"100\" size=\"40\" name=\"ItemName_" + DB.RSFieldInt(rsi,"KitItemID").ToString() + "\" value=\"" + Security.HtmlEncode(DB.RSFieldByLocale(rsi,"Name",cust.LocaleSetting)) + "\">");
                    //tmpS.Append("<input type=\"hidden\" name=\"ItemName_" + DB.RSFieldInt(rsi,"KitItemID").ToString() + "_vldt\" value=\"[req][blankalert=Each kit item must have a name!]\">");
                    tmpS.Append("</td>");
                    tmpS.Append("<td>");
                    tmpS.Append(AppLogic.GetLocaleEntryFields(DB.RSField(rsi, "Description"), "ItemDescription_" + DB.RSFieldInt(rsi, "KitItemID").ToString(), false, true, false, "", 2000, 40, 0, 0, false));
                    //tmpS.Append("<input maxLength=\"2000\" size=\"40\" name=\"ItemDescription_" + DB.RSFieldInt(rsi,"KitItemID").ToString() + "\" value=\"" + Security.HtmlEncode(DB.RSFieldByLocale(rsi,"Description",cust.LocaleSetting)) + "\">");
                    tmpS.Append("</td>");
                    tmpS.Append("<td>");
                    tmpS.Append("<input maxLength=\"10\" size=\"10\" name=\"ItemInventoryVariantID_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" value=\"" + DB.RSFieldInt(rsi, "InventoryVariantID").ToString() + "\">");
                    tmpS.Append("</td>");
                    tmpS.Append("<td>");
                    tmpS.Append("<input maxLength=\"100\" size=\"20\" name=\"ItemInventoryVariantColor_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" value=\"" + DB.RSField(rsi, "InventoryVariantColor") + "\">");
                    tmpS.Append("</td>");
                    tmpS.Append("<td>");
                    tmpS.Append("<input maxLength=\"100\" size=\"20\" name=\"ItemInventoryVariantSize_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" value=\"" + DB.RSField(rsi, "InventoryVariantSize") + "\">");
                    tmpS.Append("</td>");
                    tmpS.Append("<td>");
                    tmpS.Append("<input class=\"default\" maxLength=\"10\" size=\"10\" name=\"ItemPriceDelta_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RSFieldDecimal(rsi, "PriceDelta")) + "\"> (in x.xx format)");
                    tmpS.Append("<input type=\"hidden\" name=\"ItemPriceDelta_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "_vldt\" value=\"[req][number][blankalert=Please enter the item delta price][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                    tmpS.Append("</td>");
                    tmpS.Append("<td>");
                    tmpS.Append("<input class=\"default\" maxLength=\"10\" size=\"10\" name=\"ItemWeightDelta_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RSFieldDecimal(rsi, "WeightDelta")) + "\"> (in x.xx format)");
                    tmpS.Append("<input type=\"hidden\" name=\"ItemWeightDelta_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "_vldt\" value=\"[req][number][blankalert=Please enter the item delta Weight][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                    tmpS.Append("</td>");
                    tmpS.Append("<td><input class=\"default\" maxLength=\"5\" size=\"10\" name=\"ItemDisplayOrder_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" value=\"" + DB.RSFieldInt(rsi, "DisplayOrder").ToString() + "\"></td>");
                    tmpS.Append("<td>");
                    tmpS.Append("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"ItemIsDefault_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" value=\"1\" " + CommonLogic.IIF(DB.RSFieldBool(rsi, "IsDefault"), " checked ", "") + ">\n");
                    tmpS.Append("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"ItemIsDefault_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" value=\"0\" " + CommonLogic.IIF(DB.RSFieldBool(rsi, "IsDefault"), "", " checked ") + ">&nbsp;&nbsp;\n");
                    tmpS.Append("</td>");
                    tmpS.Append("<td>");
                    tmpS.Append("<input class=\"normalButton\" type=\"button\" value=\"Delete This Item\" name=\"DeleteItem_" + DB.RSFieldInt(rsi, "KitItemID").ToString() + "\" onClick=\"DeleteItem(" + DB.RSFieldInt(rsi, "KitItemID").ToString() + ")\">\n");
                    tmpS.Append("</td>");
                    tmpS.Append("<tr>");
                }
                rsi.Close();
                // new item row:
                tmpS.Append("<tr>");
                tmpS.Append("<td>Add New Item:</td>");
                tmpS.Append("<td>");
                tmpS.Append(AppLogic.GetLocaleEntryFields(String.Empty, "NewItemName_" + ThisGroupID.ToString(), false, false, false, "", 100, 40, 0, 0, false));
                //tmpS.Append("<input maxLength=\"100\" size=\"40\" name=\"NewItemName_" + ThisGroupID.ToString() + "\">");
                tmpS.Append("</td>");
                tmpS.Append("<td>");
                tmpS.Append(AppLogic.GetLocaleEntryFields(String.Empty, "NewItemDescription_" + ThisGroupID.ToString(), false, false, false, "", 2000, 40, 0, 0, false));
                //tmpS.Append("<input maxLength=\"2000\" size=\"40\" name=\"NewItemDescription_" + ThisGroupID.ToString() + "\">");
                tmpS.Append("</td>");
                tmpS.Append("<td>");
                tmpS.Append("<input maxLength=\"10\" size=\"10\" name=\"NewItemInventoryVariantID_" + ThisGroupID.ToString() + "\">");
                tmpS.Append("</td>");
                tmpS.Append("<td>");
                tmpS.Append("<input maxLength=\"100\" size=\"20\" name=\"NewItemInventoryVariantColor_" + ThisGroupID.ToString() + "\">");
                tmpS.Append("</td>");
                tmpS.Append("<td>");
                tmpS.Append("<input maxLength=\"100\" size=\"20\" name=\"NewItemInventoryVariantSize_" + ThisGroupID.ToString() + "\">");
                tmpS.Append("</td>");
                tmpS.Append("<td>");
                tmpS.Append("<input class=\"default\" maxLength=\"10\" size=\"10\" name=\"NewItemPriceDelta_" + ThisGroupID.ToString() + "\"> (in x.xx format)");
                tmpS.Append("<input type=\"hidden\" name=\"NewItemPriceDelta_" + ThisGroupID.ToString() + "_vldt\" value=\"[number][blankalert=Please enter the item delta price][invalidalert=Please enter a valid dollar amount, e.g. 10.00 without the leading $ sign!]\">\n");
                tmpS.Append("</td>");
                tmpS.Append("<td>");
                tmpS.Append("<input class=\"default\" maxLength=\"10\" size=\"10\" name=\"NewItemWeightDelta_" + ThisGroupID.ToString() + "\"> (in x.xx format)");
                tmpS.Append("<input type=\"hidden\" name=\"NewItemWeightDelta_" + ThisGroupID.ToString() + "_vldt\" value=\"[number][blankalert=Please enter the item delta weight][invalidalert=Please enter a valid dollar amount, e.g. 10.00]\">\n");
                tmpS.Append("</td>");
                tmpS.Append("<td><input class=\"default\" maxLength=\"5\" size=\"10\" name=\"NewItemDisplayOrder_" + ThisGroupID.ToString() + "\" value=\"1\"></td>");
                tmpS.Append("<td>");
                tmpS.Append("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"NewItemIsDefault_" + ThisGroupID.ToString() + "\" value=\"1\">\n");
                tmpS.Append("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"NewItemIsDefault_" + ThisGroupID.ToString() + "\" value=\"0\" checked >\n");
                tmpS.Append("</td>");
                tmpS.Append("<td>N/A</td>");
                tmpS.Append("<tr>");

                tmpS.Append("</table>");
                tmpS.Append("</td></tr>");
                tmpS.Append("<tr><td colspan=2 height=10 bgcolor=\"" + CommonLogic.IIF(i % 2 == 0, "#FFFFFF", "#EEEEEE") + "\"><img src=\"images/spacer.gif\" width=\"1\" height=\"20\"></td></tr>");

                i++;
            }
            rs.Close();


            tmpS.Append("<tr><td colspan=\"2\" bgcolor=\"" + CommonLogic.IIF(i % 2 == 0, "#FFFFFF", "#EEEEEE") + "\">");
            tmpS.Append("<hr size=\"1\"/>");
            tmpS.Append("<p align=\"left\"><b>ADD NEW GROUP:</b></p>");
            tmpS.Append("Group Name: ");
            tmpS.Append(AppLogic.GetLocaleEntryFields(String.Empty, "NewGroupName", false, false, false, "", 100, 50, 0, 0, false));
            //tmpS.Append("<input maxLength=\"100\" size=\"50\" name=\"NewGroupName\">");
            tmpS.Append("&nbsp;&nbsp;\n");
            tmpS.Append("Description: ");
            tmpS.Append(AppLogic.GetLocaleEntryFields(String.Empty, "NewGroupDescription", false, false, false, "", 2000, 50, 0, 0, false));
            //tmpS.Append("<input maxLength=\"1000\" size=\"50\" name=\"NewGroupDescription\">");
            tmpS.Append("&nbsp;&nbsp;\n");
            tmpS.Append("Display Order: <input class=\"default\" maxLength=\"3\" size=\"5\" name=\"NewGroupDisplayOrder\" value=\"1\"><input type=\"hidden\" name=\"NewGroupDisplayOrder_vldt\" value=\"[number][blankalert=Please enter an integer number\">&nbsp;&nbsp;");
            tmpS.Append("<select class=\"default\" size=\"1\" name=\"NewGroupType\">\n");
            IDataReader rsst3 = DB.GetRS("select * from KitGroupType  " + DB.GetNoLock() + " order by DisplayOrder,Name");
            while (rsst3.Read())
            {
                tmpS.Append("<option value=\"" + DB.RSFieldInt(rsst3, "KitGroupTypeID").ToString() + "\">" + DB.RSFieldByLocale(rsst3, "Name", cust.LocaleSetting) + "</option>");
            }
            tmpS.Append("</select>&nbsp;&nbsp;");
            rsst3.Close();
            tmpS.Append("Is Required: Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"NewGroupIsRequired\" value=\"1\" checked>\n");
            tmpS.Append("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"NewGroupIsRequired\" value=\"0\" >\n");
            tmpS.Append("</td></tr>");

            tmpS.Append("<tr>\n");
            tmpS.Append("<td></td><td align=\"left\" valign=\"top\"><br/>\n");
            tmpS.Append("<input type=\"submit\" value=\"Update\" name=\"submit\" class=\"normalButton\">\n");
            tmpS.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"reset\" value=\"Reset\" class=\"normalButton\" name=\"reset\">\n");
            tmpS.Append("</td>\n");
            tmpS.Append("</tr>\n");
            tmpS.Append("</table>\n");
            tmpS.Append("</form>\n");

            tmpS.Append("<script type=\"text/javascript\">\n");
            tmpS.Append("function DeleteGroup(id)\n");
            tmpS.Append("{\n");
            tmpS.Append("if(confirm('Are you sure you want to delete Group: ' + id + ' from this kit? This will also delete any items that are in this group!'))\n");
            tmpS.Append("{\n");
            tmpS.Append("self.location = 'entityeditkit.aspx?deletegroupid=' + id + '&productid=" + ProductID.ToString() + "&entityName=" + CommonLogic.QueryStringCanBeDangerousContent("entityName") + "&entityFilterID=" + CommonLogic.QueryStringCanBeDangerousContent("entityFilterID") + "';\n");
            tmpS.Append("}\n");
            tmpS.Append("}\n");
            tmpS.Append("function DeleteItem(id)\n");
            tmpS.Append("{\n");
            tmpS.Append("if(confirm('Are you sure you want to delete Item: ' + id + ' from this group?'))\n");
            tmpS.Append("{\n");
            tmpS.Append("self.location = 'entityeditkit.aspx?deleteitemid=' + id + '&productid=" + ProductID.ToString() + "&entityName=" + CommonLogic.QueryStringCanBeDangerousContent("entityName") + "&entityFilterID=" + CommonLogic.QueryStringCanBeDangerousContent("entityFilterID") + "';\n");
            tmpS.Append("}\n");
            tmpS.Append("}\n");
            tmpS.Append("</SCRIPT>\n");

            ltContent.Text = tmpS.ToString();
        }

        protected void loadScript(bool load)
        {
            if (load)
            {
                if (AppLogic.NumLocaleSettingsInstalled() > 1)
                {
                    this.ltScript.Text += CommonLogic.ReadFile("jscripts/tabs.js", true);
                }
            }
            else
            {
                this.ltScript.Text = "";
            }
        }

        protected void resetError(string error, bool isError)
        {
            string str = "<font class=\"noticeMsg\">NOTICE:</font>&nbsp;&nbsp;&nbsp;";
            if (isError)
                str = "<font class=\"errorMsg\">ERROR:</font>&nbsp;&nbsp;&nbsp;";

            if (error.Length > 0)
                str += error + "";
            else
                str = "";

            ltError.Text = str;
        }
    }
}
