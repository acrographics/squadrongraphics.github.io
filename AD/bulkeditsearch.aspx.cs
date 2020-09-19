// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/bulkeditsearch.aspx.cs 6     10/03/06 7:56p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for bulkeditsearch.
    /// </summary>
    public partial class bulkeditsearch : AspDotNetStorefront.SkinBase
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

            if (CommonLogic.FormCanBeDangerousContent("IsSubmit").ToUpper(CultureInfo.InvariantCulture) == "TRUE")
            {
                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    String FieldName = Request.Form.Keys[i];
                    if (FieldName.StartsWith("setitle", StringComparison.InvariantCultureIgnoreCase))
                    {
                        String[] FieldNameSplit = FieldName.Split('_');
                        int TheProductID = Localization.ParseUSInt(FieldNameSplit[1]);
                        int TheVariantID = Localization.ParseUSInt(FieldNameSplit[2]);
                        String inputVal = AppLogic.FormLocaleXml("SETitle_" + TheProductID.ToString() + "_" + TheVariantID.ToString()).Trim();
                        if (inputVal.Length == 0)
                        {
                            DB.ExecuteSQL("update Product set SETitle=NULL where ProductID=" + TheProductID.ToString());
                        }
                        else
                        {
                            DB.ExecuteSQL("update Product set SETitle=" + DB.SQuote(inputVal) + " where ProductID=" + TheProductID.ToString());
                        }
                    }
                    if (FieldName.StartsWith("sekeywords", StringComparison.InvariantCultureIgnoreCase))
                    {
                        String[] FieldNameSplit = FieldName.Split('_');
                        int TheProductID = Localization.ParseUSInt(FieldNameSplit[1]);
                        int TheVariantID = Localization.ParseUSInt(FieldNameSplit[2]);
                        String inputVal = AppLogic.FormLocaleXml("SEKeywords_" + TheProductID.ToString() + "_" + TheVariantID.ToString()).Trim();
                        if (inputVal.Length == 0)
                        {
                            DB.ExecuteSQL("update Product set SEKeywords=NULL where ProductID=" + TheProductID.ToString());
                        }
                        else
                        {
                            DB.ExecuteSQL("update Product set SEKeywords=" + DB.SQuote(inputVal) + " where ProductID=" + TheProductID.ToString());
                        }
                    }
                    if (FieldName.StartsWith("sedescription", StringComparison.InvariantCultureIgnoreCase))
                    {
                        String[] FieldNameSplit = FieldName.Split('_');
                        int TheProductID = Localization.ParseUSInt(FieldNameSplit[1]);
                        int TheVariantID = Localization.ParseUSInt(FieldNameSplit[2]);
                        String inputVal = AppLogic.FormLocaleXml("SEDescription_" + TheProductID.ToString() + "_" + TheVariantID.ToString()).Trim();
                        if (inputVal.Length == 0)
                        {
                            DB.ExecuteSQL("update Product set SEDescription=NULL where ProductID=" + TheProductID.ToString());
                        }
                        else
                        {
                            DB.ExecuteSQL("update Product set SEDescription=" + DB.SQuote(inputVal) + " where ProductID=" + TheProductID.ToString());
                        }
                    }
                    if (FieldName.StartsWith("senoscript", StringComparison.InvariantCultureIgnoreCase))
                    {
                        String[] FieldNameSplit = FieldName.Split('_');
                        int TheProductID = Localization.ParseUSInt(FieldNameSplit[1]);
                        int TheVariantID = Localization.ParseUSInt(FieldNameSplit[2]);
                        String inputVal = AppLogic.FormLocaleXml("SENoScript_" + TheProductID.ToString() + "_" + TheVariantID.ToString()).Trim();
                        if (inputVal.Length == 0)
                        {
                            DB.ExecuteSQL("update Product set SENoScript=NULL where ProductID=" + TheProductID.ToString());
                        }
                        else
                        {
                            DB.ExecuteSQL("update Product set SENoScript=" + DB.SQuote(inputVal) + " where ProductID=" + TheProductID.ToString());
                        }
                    }
                    //if (FieldName.StartsWith("sealttext", StringComparison.InvariantCultureIgnoreCase))
                    //{
                    //    String[] FieldNameSplit = FieldName.Split('_');
                    //    int TheProductID = Localization.ParseUSInt(FieldNameSplit[1]);
                    //    int TheVariantID = Localization.ParseUSInt(FieldNameSplit[2]);
                    //    String inputVal = AppLogic.FormLocaleXml("SEAltText_" + TheProductID.ToString() + "_" + TheVariantID.ToString()).Trim();
                    //    if (inputVal.Length == 0)
                    //    {
                    //        DB.ExecuteSQL("update Product set SEAltText=NULL where ProductID=" + TheProductID.ToString());
                    //    }
                    //    else
                    //    {
                    //        DB.ExecuteSQL("update Product set SEAltText=" + DB.SQuote(inputVal) + " where ProductID=" + TheProductID.ToString());
                    //    }
                    //}
                }
            }
            SectionTitle = "<a href=\"entities.aspx?entityname=" + m_EntitySpecs.m_EntityName + "\">" + AppLogic.GetString("AppConfig." + m_EntitySpecs.m_EntityName + "PromptPlural", SkinID, ThisCustomer.LocaleSetting) + "</a> - Edit Inventory For: " + Helper.GetEntityName(EntityID, ThisCustomer.LocaleSetting);
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write(CommonLogic.ReadFile("jscripts/tabs.js", true));
            ProductCollection products = new ProductCollection(m_EntitySpecs.m_EntityName, EntityID);
            products.PageSize = 0;
            products.PageNum = 1;
            products.PublishedOnly = false;
            products.ReturnAllVariants = false;
            DataSet dsProducts = products.LoadFromDB();
            int NumProducts = products.NumProducts;
            if (NumProducts > 0)
            {
                String prompt = "Editing Product Search Engine Fields for " + m_EntitySpecs.m_EntityName + ": <a href=\"editentity.aspx?entityname=" + m_EntitySpecs.m_EntityName + "&entityid=" + EntityID.ToString() + "\">" + Helper.GetEntityName(EntityID, ThisCustomer.LocaleSetting) + "</a>";
                writer.Write("<p><b>" + prompt + "</b></p>");

                writer.Write("<form id=\"Form1\" name=\"Form1\" method=\"POST\" action=\"bulkeditsearch.aspx?entityid=" + EntityID.ToString() + "&entityname=" + m_EntitySpecs.m_EntityName + "\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<p align=\"right\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></p>\n");
                writer.Write("<table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
                writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                writer.Write("<td><b>ProductID</b></td>\n");
                writer.Write("<td><b>VariantID</b></td>\n");
                writer.Write("<td><b>Product Name</b></td>\n");
                writer.Write("<td><b>Variant Name</b></td>\n");
                writer.Write("<td align=\"left\"><b>Product Fields</b></td>\n");
                //writer.Write("<td align=\"left\"><b>Variant Fields</b></td>\n");
                writer.Write("</tr>\n");
                int LastProductID = 0;
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
                    writer.Write("<td align=\"left\" valign=\"top\">");
                    writer.Write("<div align=\"left\">");
                    writer.Write("<b>Search Engine Page Title:</b><br/>");
                    writer.Write(AppLogic.GetLocaleEntryFields(DB.RowField(row, "SETitle"), "SETitle_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString(), false, true, false, "", 100, 100, 0, 0, false));
                    writer.Write("<b>Search Engine Keywords:</b><br/>");
                    writer.Write(AppLogic.GetLocaleEntryFields(DB.RowField(row, "SEKeywords"), "SEKeywords_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString(), false, true, false, "", 255, 100, 0, 0, false));
                    writer.Write("<b>Search Engine Description:</b><br/>");
                    writer.Write(AppLogic.GetLocaleEntryFields(DB.RowField(row, "SEDescription"), "SEDescription_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString(), false, true, false, "", 255, 100, 0, 0, false));
                    writer.Write("<b>Search Engine NoScript:</b><br/>");
                    writer.Write(AppLogic.GetLocaleEntryFields(DB.RowField(row, "SENoScript"), "SENoScript_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString(), true, true, false, "", 50, 50, 0, 0, false));
                    //writer.Write("<b>Search Engine AltText:</b><br/>");
                    //writer.Write(AppLogic.GetLocaleEntryFields(DB.RowField(row, "SEAltText"), "SEAltText_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString(), false, true, false, "", 50, 50, 0, 0, false));
                    writer.Write("</div>");
                    writer.Write("</td>\n");
                    writer.Write("</tr>\n");
                    LastProductID = ThisProductID;
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
