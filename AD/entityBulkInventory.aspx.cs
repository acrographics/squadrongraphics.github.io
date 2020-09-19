// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/entityBulkInventory.aspx.cs 7     10/04/06 6:23a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Text;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for bulkeditinventory.
    /// </summary>
    public partial class entityBulkInventory : System.Web.UI.Page
    {
        int EntityID;
        String EntityName;
        EntitySpecs m_EntitySpecs;
        EntityHelper Helper;
        new int SkinID = 1;
        private Customer cust;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            EntityID = CommonLogic.QueryStringUSInt("EntityID"); ;
            EntityName = CommonLogic.QueryStringCanBeDangerousContent("EntityName");
            m_EntitySpecs = EntityDefinitions.LookupSpecs(EntityName);
            Helper = new EntityHelper(m_EntitySpecs);
            //Helper = AppLogic.LookupHelper(base.EntityHelpers, m_EntitySpecs.m_EntityName);

            if (EntityID == 0 || EntityName.Length == 0)
            {
                ltBody.Text = "Invalid Parameters";
                return;
            }

            if (CommonLogic.FormBool("IsSubmit"))
            {
                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    String FieldName = Request.Form.Keys[i];
                    if (FieldName.IndexOf("|") != -1 && ((FieldName.StartsWith("simple", StringComparison.InvariantCultureIgnoreCase) || FieldName.StartsWith("sizecolor", StringComparison.InvariantCultureIgnoreCase))))
                    {
                        String KeyVal = CommonLogic.FormCanBeDangerousContent(FieldName);
                        // this field should be processed
                        String[] FieldNameSplit = FieldName.Split('|');
                        String InventoryType = FieldNameSplit[0].ToLower(CultureInfo.InvariantCulture);
                        int TheProductID = Localization.ParseUSInt(FieldNameSplit[1]);
                        int TheVariantID = Localization.ParseUSInt(FieldNameSplit[2]);
                        String Size = FieldNameSplit[3];
                        String Color = FieldNameSplit[4];
                        int inputVal = CommonLogic.FormUSInt(FieldName);
                        if (InventoryType == "simple")
                        {
                            DB.ExecuteSQL("update ProductVariant set Inventory=" + inputVal.ToString() + " where VariantID=" + TheVariantID.ToString());
                        }
                        else
                        {
                            String sql = "select count(*) as N from Inventory " + DB.GetNoLock() + " where VariantID=" + TheVariantID.ToString() + " and lower([size])=" + DB.SQuote(AppLogic.CleanSizeColorOption(Size).ToLowerInvariant()) + " and lower(color)=" + DB.SQuote(AppLogic.CleanSizeColorOption(Color).ToLowerInvariant());
                            if (DB.GetSqlN(sql) == 0)
                            {
                                sql = "insert into Inventory(InventoryGUID,VariantID,[Size],Color,Quan) values(" + DB.SQuote(DB.GetNewGUID()) + "," + TheVariantID.ToString() + "," + DB.SQuote(AppLogic.CleanSizeColorOption(Size)) + "," + DB.SQuote(AppLogic.CleanSizeColorOption(Color)) + "," + inputVal.ToString() + ")";
                                DB.ExecuteSQL(sql);
                            }
                            else
                            {
                                sql = "update Inventory set Quan=" + inputVal.ToString() + " where VariantID=" + TheVariantID.ToString() + " and lower([size])=" + DB.SQuote(AppLogic.CleanSizeColorOption(Size).ToLowerInvariant()) + " and lower(color)=" + DB.SQuote(AppLogic.CleanSizeColorOption(Color).ToLowerInvariant());
                                DB.ExecuteSQL(sql);
                            }
                        }
                    }
                }
                DB.ExecuteSQL("Update Inventory set Quan=0 where Quan<0"); // safety check
                DB.ExecuteSQL("Update ProductVariant set Inventory=0 where Inventory<0"); // safety check
            }

            LoadBody();
        }

        protected void LoadBody()
        {
            StringBuilder tmpS = new StringBuilder(4096);

            tmpS.Append("<div style=\"width: 100%; border-top: solid 1px #d2d2d2; padding-top: 3px; margin-top: 5px;\">");

            ProductCollection products = new ProductCollection(EntityName, EntityID);
            products.PageSize = 0;
            products.PageNum = 1;
            products.PublishedOnly = false;
            products.ReturnAllVariants = true;
            DataSet dsProducts = products.LoadFromDB();
            int NumProducts = products.NumProducts;
            if (NumProducts > 0)
            {
                tmpS.Append("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                tmpS.Append("<table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
                tmpS.Append("<tr><td colspan=\"5\" align=\"right\"><input type=\"submit\" value=\"Inventory Update\" class=\"normalButton\" name=\"Submit\"></td></tr>");
                tmpS.Append("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                tmpS.Append("<td><b>ProductID</b></td>\n");
                tmpS.Append("<td><b>VariantID</b></td>\n");
                tmpS.Append("<td><b>Product Name</b></td>\n");
                tmpS.Append("<td><b>Variant Name</b></td>\n");
                tmpS.Append("<td><b>Inventory</b></td>\n");
                tmpS.Append("</tr>\n");
                foreach (DataRow row in dsProducts.Tables[0].Rows)
                {
                    int ThisProductID = DB.RowFieldInt(row, "ProductID");
                    int ThisVariantID = DB.RowFieldInt(row, "VariantID");
                    tmpS.Append("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                    tmpS.Append("<td align=\"left\" valign=\"middle\">");
                    tmpS.Append(ThisProductID.ToString());
                    tmpS.Append("</td>");
                    tmpS.Append("<td align=\"left\" valign=\"middle\">");
                    tmpS.Append(ThisVariantID.ToString());
                    tmpS.Append("</td>");
                    tmpS.Append("<td align=\"left\" valign=\"middle\">");
                    tmpS.Append("<a target=\"entityBody\" href=\"editproduct.aspx?productid=" + ThisProductID.ToString() + "\">");
                    tmpS.Append(DB.RowFieldByLocale(row, "Name", cust.LocaleSetting));
                    tmpS.Append("</a>");
                    tmpS.Append("</td>\n");
                    tmpS.Append("<td align=\"left\" valign=\"middle\">");
                    tmpS.Append("<a target=\"entityBody\" href=\"editvariant.aspx?productid=" + ThisProductID.ToString() + "&variantid=" + ThisVariantID.ToString() + "\">");
                    tmpS.Append(DB.RowFieldByLocale(row, "VariantName", cust.LocaleSetting));
                    tmpS.Append("</a>");
                    tmpS.Append("</td>\n");
                    tmpS.Append("<td align=\"left\" valign=\"middle\">");
                    String s = AppLogic.GetInventoryTable(ThisProductID, ThisVariantID, true, SkinID, false, true);
                    tmpS.Append(s);
                    tmpS.Append("</td>\n");
                    tmpS.Append("</tr>\n");
                }
                tmpS.Append("<tr><td colspan=\"5\" align=\"right\"><input type=\"submit\" value=\"Inventory Update\" class=\"normalButton\" name=\"Submit\"></td></tr>");
                tmpS.Append("</table>\n");
                //tmpS.Append("<div style=\"width: 99%; text-align: right;\"><input type=\"submit\" value=\"Inventory Update\" class=\"normalButton\" name=\"Submit\"></div>\n");
            }
            else
            {
                tmpS.Append("<p><b>No Products Found</b></p>");
            }
            dsProducts.Dispose();
            products.Dispose();

            tmpS.Append("</div>");
            ltBody.Text = tmpS.ToString();
        }

    }
}
