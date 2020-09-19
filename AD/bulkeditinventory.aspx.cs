// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/bulkeditinventory.aspx.cs 8     10/03/06 7:56p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for bulkeditinventory.
    /// </summary>
    public partial class bulkeditinventory : AspDotNetStorefront.SkinBase
    {
        int EntityID;
        String EntityName;
        EntitySpecs m_EntitySpecs;
        EntityHelper Helper;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            EntityID = CommonLogic.QueryStringUSInt("EntityID"); ;
            EntityName = CommonLogic.QueryStringCanBeDangerousContent("EntityName");
            m_EntitySpecs = EntityDefinitions.LookupSpecs(EntityName);
            Helper = AppLogic.LookupHelper(base.EntityHelpers, m_EntitySpecs.m_EntityName);

            if (EntityID == 0 || EntityName.Length == 0)
            {
                Response.Redirect("splash.aspx");
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
            SectionTitle = "<a href=\"entities.aspx?entityname=" + m_EntitySpecs.m_EntityName + "\">" + AppLogic.GetString("AppConfig." + m_EntitySpecs.m_EntityName + "PromptPlural", SkinID, ThisCustomer.LocaleSetting) + "</a> - Edit Inventory For: " + Helper.GetEntityName(EntityID, ThisCustomer.LocaleSetting);
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {

            ProductCollection products = new ProductCollection(EntityName, EntityID);
            products.PageSize = 0;
            products.PageNum = 1;
            products.PublishedOnly = false;
            products.ReturnAllVariants = true;
            DataSet dsProducts = products.LoadFromDB();
            int NumProducts = products.NumProducts;
            if (NumProducts > 0)
            {
                String prompt = "Editing Product Inventory for " + m_EntitySpecs.m_EntityName + ": <a href=\"editentity.aspx?entityname=" + m_EntitySpecs.m_EntityName + "&entityid=" + EntityID.ToString() + "\">" + Helper.GetEntityName(EntityID, ThisCustomer.LocaleSetting) + "</a>";
                writer.Write("<p><b>" + prompt + "</b></p>");

                writer.Write("<form id=\"Form1\" name=\"Form1\" method=\"POST\" action=\"bulkeditinventory.aspx?entityid=" + EntityID.ToString() + "&entityname=" + m_EntitySpecs.m_EntityName + "\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<p align=\"right\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></p>\n");
                writer.Write("<table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
                writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                writer.Write("<td><b>ProductID</b></td>\n");
                writer.Write("<td><b>VariantID</b></td>\n");
                writer.Write("<td><b>Product Name</b></td>\n");
                writer.Write("<td><b>Variant Name</b></td>\n");
                //writer.Write("<td><b>Inventory Tracking</b></td>\n");
                writer.Write("<td><b>Inventory</b></td>\n");
                writer.Write("</tr>\n");
                foreach (DataRow row in dsProducts.Tables[0].Rows)
                {
                    int ThisProductID = DB.RowFieldInt(row, "ProductID");
                    int ThisVariantID = DB.RowFieldInt(row, "VariantID");
                    writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                    writer.Write("<td align=\"left\" valign=\"top\">");
                    writer.Write(ThisProductID.ToString());
                    writer.Write("</td>");
                    writer.Write("<td align=\"left\" valign=\"top\">");
                    writer.Write(ThisVariantID.ToString());
                    writer.Write("</td>");
                    writer.Write("<td align=\"left\" valign=\"top\">");
                    writer.Write("<a href=\"editproduct.aspx?productid=" + ThisProductID.ToString() + "\">");
                    writer.Write(DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting));
                    writer.Write("</a>");
                    writer.Write("</td>\n");
                    writer.Write("<td align=\"left\" valign=\"top\">");
                    writer.Write("<a href=\"editvariant.aspx?productid=" + ThisProductID.ToString() + "&variantid=" + ThisVariantID.ToString() + "\">");
                    writer.Write(DB.RowFieldByLocale(row, "VariantName", ThisCustomer.LocaleSetting));
                    writer.Write("</a>");
                    writer.Write("</td>\n");
                    //writer.Write("<td align=\"left\" valign=\"top\">");
                    //if (AppLogic.ProductTracksInventoryBySizeAndColor(ThisProductID))
                    //{
                    //    writer.Write("size/color");
                    //}
                    //else
                    //{
                    //    writer.Write("simple");
                    //}
                    //writer.Write("</td>\n");
                    writer.Write("<td align=\"left\" valign=\"top\">");
                    String s = AppLogic.GetInventoryTable(ThisProductID, ThisVariantID, true, SkinID, false, true);
                    writer.Write(s);
                    writer.Write("</td>\n");
                    writer.Write("</tr>\n");
                }
                writer.Write("</table>\n");
                writer.Write("<p align=\"right\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></p>\n");
                writer.Write("</form>\n");
            }
            else
            {
                writer.Write("<p><b>No Products Found</b></p>");
            }
            dsProducts.Dispose();
            products.Dispose();
        }

    }
}
