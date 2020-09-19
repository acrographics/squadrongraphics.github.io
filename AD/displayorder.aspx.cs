// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/displayorder.aspx.cs 4     9/03/06 8:41p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for displayorder.
    /// </summary>
    public partial class displayorder : AspDotNetStorefront.SkinBase
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
                if (EntityID != 0)
                {
                    DB.ExecuteSQL(String.Format("delete from {0}{1} where {2}ID={3}", m_EntitySpecs.m_ObjectName, m_EntitySpecs.m_EntityName, m_EntitySpecs.m_EntityName, EntityID.ToString()));
                }

                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    if (Request.Form.Keys[i].IndexOf("DisplayOrder_") != -1)
                    {
                        String[] keys = Request.Form.Keys[i].Split('_');
                        int ObjectID = Localization.ParseUSInt(keys[1]);
                        int DispOrd = 1;
                        try
                        {
                            DispOrd = Localization.ParseUSInt(Request.Form[Request.Form.Keys[i]]);
                        }
                        catch { }
                        DB.ExecuteSQL(String.Format("insert into {0}{1}({2}ID,{3}ID,DisplayOrder) values({4},{5},{6})", m_EntitySpecs.m_ObjectName, m_EntitySpecs.m_EntityName, m_EntitySpecs.m_EntityName, m_EntitySpecs.m_ObjectName, EntityID.ToString(), ObjectID.ToString(), DispOrd.ToString()));
                    }
                }
            }
            SectionTitle = "Set Display Order";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {

            String sql = "select ~.*,DisplayOrder from ~  " + DB.GetNoLock() + " left outer join ~^ " + DB.GetNoLock() + " on ~.~id=~^.~id where ~^.^id=" + EntityID.ToString() + " and deleted=0 ";
            sql += " and ~.~ID in (select distinct ~id from ~^  " + DB.GetNoLock() + " where ^id=" + EntityID.ToString() + ")";
            sql += " order by DisplayOrder,Name";

            sql = sql.Replace("^", m_EntitySpecs.m_EntityName).Replace("~", m_EntitySpecs.m_ObjectName);

            String prompt = "Setting " + m_EntitySpecs.m_ObjectName + " Display Order for " + m_EntitySpecs.m_EntityName + ": " + Helper.GetEntityName(EntityID, ThisCustomer.LocaleSetting);
            DataSet ds = DB.GetDS(sql, false);

            writer.Write("<p><b>" + prompt + "</b></p>");

            writer.Write("<form id=\"Form1\" name=\"Form1\" method=\"POST\" action=\"displayorder.aspx?entityid=" + EntityID.ToString() + "&entityname=" + m_EntitySpecs.m_EntityName + "\">\n");
            writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
            writer.Write("  <table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
            writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
            writer.Write("      <td><b>ID</b></td>\n");
            writer.Write("      <td><b>" + m_EntitySpecs.m_ObjectName + "</b></td>\n");
            writer.Write("      <td align=\"center\"><b>Display Order</b></td>\n");
            writer.Write("    </tr>\n");
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                int ThisID = DB.RowFieldInt(row, m_EntitySpecs.m_ObjectName + "ID");
                writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                writer.Write("      <td >" + ThisID.ToString() + "</td>\n");
                writer.Write("      <td >");
                String Image1URL = AppLogic.LookupImage(m_EntitySpecs.m_ObjectName, ThisID, "icon", SkinID, ThisCustomer.LocaleSetting);
                writer.Write("<a href=\"edit" + m_EntitySpecs.m_ObjectName + ".aspx?" + m_EntitySpecs.m_ObjectName + "id=" + ThisID.ToString() + "\">");
                writer.Write("<img src=\"" + Image1URL + "\" height=\"25\" border=\"0\" align=\"absmiddle\">");
                writer.Write("</a>&nbsp;\n");
                writer.Write("<a href=\"edit" + m_EntitySpecs.m_ObjectName + ".aspx?" + m_EntitySpecs.m_ObjectName + "id=" + ThisID.ToString() + "\">");
                writer.Write(DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting));
                writer.Write("</a>");

                writer.Write("</a>");
                writer.Write("</td>\n");
                writer.Write("      <td align=\"center\"><input size=2 type=\"text\" name=\"DisplayOrder_" + ThisID.ToString() + "\" value=\"" + CommonLogic.IIF(DB.RowFieldInt(row, "DisplayOrder") == 0, "1", DB.RowField(row, "DisplayOrder")) + "\"></td>\n");
                writer.Write("    </tr>\n");
            }
            ds.Dispose();
            writer.Write("    <tr>\n");
            writer.Write("      <td colspan=\"2\" align=\"left\"></td>\n");
            writer.Write("      <td align=\"center\" bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></td>\n");
            writer.Write("    </tr>\n");
            writer.Write("  </table>\n");
            writer.Write("</form>\n");

            writer.Write("</center></b>\n");
        }

    }
}
