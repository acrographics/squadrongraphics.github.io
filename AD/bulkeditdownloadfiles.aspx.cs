// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/bulkeditdownloadfiles.aspx.cs 7     9/30/06 3:39p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Text;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for bulkeditdownloadfiles.
    /// </summary>
    public partial class bulkeditdownloadfiles : AspDotNetStorefront.SkinBase
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
                ProductCollection products = new ProductCollection(m_EntitySpecs.m_EntityName, EntityID);
                products.PageSize = 0;
                products.PageNum = 1;
                products.PublishedOnly = false;
                products.ReturnAllVariants = true;
                DataSet dsProducts = products.LoadFromDB();
                int NumProducts = products.NumProducts;
                foreach (DataRow row in dsProducts.Tables[0].Rows)
                {
                    if (DB.RowFieldBool(row, "IsDownload"))
                    {
                        int ThisProductID = DB.RowFieldInt(row, "ProductID");
                        int ThisVariantID = DB.RowFieldInt(row, "VariantID");
                        StringBuilder sql = new StringBuilder(1024);
                        sql.Append("update productvariant set ");
                        String DLoc = CommonLogic.FormCanBeDangerousContent("DownloadLocation_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString());
                        if (DLoc.StartsWith("/"))
                        {
                            DLoc = DLoc.Substring(1, DLoc.Length - 1); // remove leading / char!
                        }
                        sql.Append("DownloadLocation=" + DB.SQuote(DLoc));
                        sql.Append(" where VariantID=" + ThisVariantID.ToString());
                        DB.ExecuteSQL(sql.ToString());
                    }
                }
                dsProducts.Dispose();
            }
            SectionTitle = "<a href=\"entities.aspx?entityname=" + m_EntitySpecs.m_EntityName + "\">" + AppLogic.GetString("AppConfig." + m_EntitySpecs.m_EntityName + "PromptPlural", SkinID, ThisCustomer.LocaleSetting) + "</a> - Edit Inventory For: " + Helper.GetEntityName(EntityID, ThisCustomer.LocaleSetting);
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            ProductCollection products = new ProductCollection(m_EntitySpecs.m_EntityName, EntityID);
            products.PageSize = 0;
            products.PageNum = 1;
            products.PublishedOnly = false;
            products.ReturnAllVariants = true;
            DataSet dsProducts = products.LoadFromDB();
            int NumProducts = products.NumProducts;
            if (NumProducts > 0)
            {
                String prompt = "Editing Download Files for " + m_EntitySpecs.m_EntityName + ": <a href=\"editentity.aspx?entityname=" + m_EntitySpecs.m_EntityName + "&entityid=" + EntityID.ToString() + "\">" + Helper.GetEntityName(EntityID, ThisCustomer.LocaleSetting) + "</a>";
                writer.Write("<p><b>" + prompt + "</b></p>");

                writer.Write("<script type=\"text/javascript\">\n");
                writer.Write("function Form_Validator(theForm)\n");
                writer.Write("{\n");
                writer.Write("submitonce(theForm);\n");
                writer.Write("return (true);\n");
                writer.Write("}\n");
                writer.Write("</script>\n");

                writer.Write("<form id=\"Form1\" name=\"Form1\" method=\"POST\" action=\"bulkeditdownloadfiles.aspx?entityid=" + EntityID.ToString() + "&entityname=" + m_EntitySpecs.m_EntityName + "\" onsubmit=\"return (validateForm(document.forms[0]) && Form_Validator(document.forms[0]))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");
                writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                writer.Write("<table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
                writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                writer.Write("<td><b>ProductID</b></td>\n");
                writer.Write("<td><b>VariantID</b></td>\n");
                writer.Write("<td><b>Product Name</b></td>\n");
                writer.Write("<td><b>Variant Name</b></td>\n");
                writer.Write("<td align=\"left\"><b>Download File</b></td>\n");
                writer.Write("</tr>\n");
                int LastProductID = 0;
                foreach (DataRow row in dsProducts.Tables[0].Rows)
                {
                    if (DB.RowFieldBool(row, "IsDownload"))
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
                        writer.Write("<td align=\"center\" valign=\"top\">");
                        writer.Write("<input maxLength=\"1000\" size=\"150\" name=\"DownloadLocation_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" value=\"" + DB.RowField(row, "DownloadLocation") + "\">\n");
                        writer.Write("</td>\n");
                        writer.Write("</tr>\n");
                        LastProductID = ThisProductID;
                    }
                }
                writer.Write("</table>\n");
                if (LastProductID == 0)
                {
                    writer.Write("<p align=\"left\"><b>No Download Products Found</b></p>");
                }
                else
                {
                    writer.Write("<p align=\"left\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></p>\n");
                }
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
