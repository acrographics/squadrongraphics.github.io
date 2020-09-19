// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/skinpreview.aspx.cs 6     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for skinpreview
    /// </summary>
    public partial class skinpreview : AspDotNetStorefront.SkinBase
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");


            if (CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
            {
                DB.ExecuteSQL("delete from SkinPreview where SkinPreviewID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID"));
            }
            if (CommonLogic.QueryStringCanBeDangerousContent("DefaultSkinID").Length != 0)
            {
                AppLogic.SetAppConfig("DefaultSkinID", CommonLogic.QueryStringCanBeDangerousContent("DefaultSkinID"), true);
                AppLogic.ClearCache();
            }
            SectionTitle = "Manage SkinPreview Parameters";
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            String SearchFor = CommonLogic.QueryStringCanBeDangerousContent("SearchFor");
            String GroupName = CommonLogic.QueryStringCanBeDangerousContent("GroupName");
            writer.Write("<form id=\"SkinPreviewForm\" name=\"SkinPreviewForm\" method=\"GET\" action=\"skinpreview.aspx\">");
            DataSet ds = DB.GetDS("select distinct groupname from SkinPreview  " + DB.GetNoLock() + " where groupname is not null order by groupname", AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
            writer.Write("Config Group: <select onChange=\"document.SkinPreviewForm.submit()\" size=\"1\" name=\"GroupName\">\n");
            writer.Write("<OPTION VALUE=\"0\">ALL GROUPS</option>\n");
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                writer.Write("<option value=\"" + DB.RowField(row, "GroupName") + "\"");
                if (DB.RowField(row, "GroupName") == GroupName)
                {
                    writer.Write(" selected");
                }
                writer.Write(">" + DB.RowField(row, "GroupName") + "</option>");
            }
            writer.Write("</select>&nbsp;&nbsp;&nbsp;");
            ds.Dispose();
            String BeginsWith = CommonLogic.IIF(CommonLogic.QueryStringCanBeDangerousContent("BeginsWith").Length == 0, "%", CommonLogic.QueryStringCanBeDangerousContent("BeginsWith"));
            String alpha = "%#ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 1; i <= alpha.Length; i++)
            {
                if (BeginsWith.Substring(0, 1) == alpha.Substring(i - 1, 1))
                {
                    writer.Write(alpha.Substring(i - 1, 1) + "&nbsp;");
                }
                else
                {
                    writer.Write("<a href=\"skinpreview.aspx?GroupName=" + Server.UrlEncode(GroupName) + "&BeginsWith=" + Server.UrlEncode("" + alpha.Substring(i - 1, 1)) + "\">" + alpha.Substring(i - 1, 1) + "</a>&nbsp;");
                }
            }
            writer.Write("&nbsp;&nbsp;Search For: <input type=\"text\" name=\"SearchFor\" value=\"" + SearchFor + "\"><input type=\"submit\" name=\"search\" value=\"submit\">");
            writer.Write("</form>");

            String sql = String.Empty;
            if (SearchFor.Length != 0)
            {
                sql = "select * from SkinPreview  " + DB.GetNoLock() + " where " + " name like " + DB.SQuote("%" + SearchFor + "%") + " order by skinid,name";
            }
            else
            {
                sql = "select * from SkinPreview  " + DB.GetNoLock() + " where " + " name like " + DB.SQuote(BeginsWith + "%") + " order by skinid,name";
            }
            if (GroupName.Length != 0 && GroupName != "0")
            {
                sql = "select * from SkinPreview  " + DB.GetNoLock() + " where " + " groupname=" + DB.SQuote(GroupName) + " order by skinid,name";
            }

            //writer.Write("<p align=left><big><font color=red><b>WARNING: Consult the documentation before modifying these values, as incorrect values can cause your store site and/or administration site to stop working!</b></font></big></p>\n");
            ds = DB.GetDS(sql, false);
            writer.Write("<form method=\"POST\" action=\"skinpreview.aspx\">\n");
            writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
            writer.Write("  <table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
            writer.Write("    <tr>\n");
            writer.Write("      <td align=\"left\"><input type=\"button\" value=\"Add New SkinPreview\" name=\"AddNew\" onClick=\"self.location='editSkinPreview.aspx?GroupName=" + Server.UrlEncode(GroupName) + "&beginsWith=" + Server.UrlEncode(BeginsWith) + "&searchfor=" + Server.UrlEncode(SearchFor) + "';\"></td>\n");
            writer.Write("      <td colspan=\"6\"></td>\n");
            writer.Write("    </tr>\n");
            writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
            writer.Write("      <td width=\"10%\" align=\"left\"><b>Skin ID</b></td>\n");
            writer.Write("      <td width=\"20%\" align=\"left\"><b>Name</b></td>\n");
            writer.Write("      <td width=\"30%\" align=\"left\"><b>Thumbnail</b></td>\n");
            writer.Write("      <td width=\"10%\" align=\"center\"><b>Edit</b></td>\n");
            writer.Write("      <td width=\"10%\" align=\"center\"><b>Set As Default</b></td>\n");
            writer.Write("      <td width=\"10%\" align=\"center\"><b>Delete</b></td>\n");
            writer.Write("    </tr>\n");
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                bool okToShow = true;
                if (DB.RowField(row, "Name").ToUpperInvariant() == "ADMIN_SUPERUSER" && !ThisCustomer.IsAdminSuperUser)
                {
                    okToShow = false;
                }
                if (okToShow)
                {
                    writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                    writer.Write("      <td valign=\"top\" align=\"left\">");
                    if (AppLogic.AppConfigUSInt("DefaultSkinID") == DB.RowFieldInt(row, "SkinPreviewID"))
                    {
                        writer.Write("<b>*" + DB.RowFieldInt(row, "SkinPreviewID").ToString() + "</b>");
                    }
                    else
                    {
                        writer.Write(DB.RowFieldInt(row, "SkinPreviewID").ToString());
                    }
                    writer.Write("</td>\n");
                    writer.Write("      <td valign=\"top\" align=\"left\"><a href=\"editSkinPreview.aspx?GroupName=" + Server.UrlEncode(GroupName) + "&beginsWith=" + Server.UrlEncode(BeginsWith) + "&searchfor=" + Server.UrlEncode(SearchFor) + "&SkinPreviewID=" + DB.RowFieldInt(row, "SkinPreviewID").ToString() + "\">" + DB.RowField(row, "Name") + "</a></td>\n");
                    writer.Write("      <td valign=\"top\" align=\"left\">");
                    String Image1URL = AppLogic.LookupImage("SkinPreviews", DB.RowFieldInt(row, "SkinPreviewID"), "icon", SkinID, ThisCustomer.LocaleSetting);

                    String MediumPic = AppLogic.LookupImage("SkinPreviews", DB.RowFieldInt(row, "SkinPreviewID"), "medium", SkinID, ThisCustomer.LocaleSetting);
                    bool HasMediumPic = (MediumPic.Length != 0);
                    writer.Write("<a href=\"editSkinPreview.aspx?GroupName=" + Server.UrlEncode(GroupName) + "&beginsWith=" + Server.UrlEncode(BeginsWith) + "&searchfor=" + Server.UrlEncode(SearchFor) + "&SkinPreviewID=" + DB.RowFieldInt(row, "SkinPreviewID").ToString() + "\">");
                    writer.Write("<img src=\"" + Image1URL + "\" border=\"0\" align=\"absmiddle\">");
                    writer.Write("</a>&nbsp;\n");
                    if (HasMediumPic)
                    {
                        writer.Write("<br/><img src=\"images/spacer.gif\" width=\"1\" height=\"4\"><br/>");
                        writer.Write("<div><a href=\"" + MediumPic + "\" target=\"_blank\">(view larger thumbnail)</a></div><br/>");
                    }
                    writer.Write("</td>\n");
                    writer.Write("      <td valign=\"top\" align=\"center\"><input type=\"button\" value=\"Edit\" name=\"Edit_" + DB.RowFieldInt(row, "SkinPreviewID").ToString() + "\" onClick=\"self.location='editSkinPreview.aspx?GroupName=" + Server.UrlEncode(GroupName) + "&beginsWith=" + Server.UrlEncode(BeginsWith) + "&searchfor=" + Server.UrlEncode(SearchFor) + "&SkinPreviewID=" + DB.RowFieldInt(row, "SkinPreviewID").ToString() + "'\"></td>\n");
                    writer.Write("      <td valign=\"top\" align=\"center\">");
                    writer.Write("<input type=\"button\" value=\"Set As Default Store Skin\" name=\"SetAsDefault_" + DB.RowFieldInt(row, "SkinPreviewID").ToString() + "\" onClick=\"SetAsDefaultSkin(" + DB.RowFieldInt(row, "SkinID").ToString() + ")\">");

                    writer.Write("<br/><img src=\"images/spacer.gif\" width=\"1\" height=\"4\"><br/>");
                    writer.Write("<div><a href=\"" + AppLogic.GetStoreHTTPLocation(true) + "default.aspx?skinid=" + DB.RowFieldInt(row, "SkinID").ToString() + "\" target=\"_blank\">(open store in this skin)</a></div><br/>");

                    writer.Write("</td>\n");
                    writer.Write("      <td valign=\"top\" align=\"center\"><input type=\"button\" value=\"Delete\" name=\"Delete_" + DB.RowFieldInt(row, "SkinPreviewID").ToString() + "\" onClick=\"DeleteSkinPreview(" + DB.RowFieldInt(row, "SkinPreviewID").ToString() + ")\"></td>\n");
                    writer.Write("    </tr>\n");
                }
            }
            ds.Dispose();
            writer.Write("    <tr>\n");
            writer.Write("      <td align=\"left\"><input type=\"button\" value=\"Add New Skin Preview\" name=\"AddNew\" onClick=\"self.location='editSkinPreview.aspx?GroupName=" + Server.UrlEncode(GroupName) + "&beginsWith=" + Server.UrlEncode(BeginsWith) + "&searchfor=" + Server.UrlEncode(SearchFor) + "';\"></td>\n");
            writer.Write("      <td colspan=\"6\"></td>\n");
            writer.Write("    </tr>\n");
            writer.Write("  </table>\n");
            writer.Write("</form>\n");

            writer.Write("</center></b>\n");

            writer.Write("<script type=\"text/javascript\">\n");

            writer.Write("function DeleteSkinPreview(id)\n");
            writer.Write("{\n");
            writer.Write("if(confirm('Are you sure you want to delete Skin Preview: ' + id + '.\\n\\nNOTE: THIS DOES NOT DELETE ANY SKINS OR SKIN FILES!!! ONLY THE PREVIEW IS DELETED!'))\n");
            writer.Write("{\n");
            writer.Write("self.location = 'skinpreview.aspx?GroupName=" + Server.UrlEncode(GroupName) + "&beginsWith=" + Server.UrlEncode(BeginsWith) + "&searchfor=" + Server.UrlEncode(SearchFor) + "&deleteid=' + id;\n");
            writer.Write("}\n");
            writer.Write("}\n");

            writer.Write("function SetAsDefaultSkin(skinid)\n");
            writer.Write("{\n");
            writer.Write("if(confirm('Are you sure you want to make skin: ' + skinid + ' the active default skin for your storefront?'))\n");
            writer.Write("{\n");
            writer.Write("self.location = 'skinpreview.aspx?GroupName=" + Server.UrlEncode(GroupName) + "&beginsWith=" + Server.UrlEncode(BeginsWith) + "&searchfor=" + Server.UrlEncode(SearchFor) + "&defaultskinid=' + skinid;\n");
            writer.Write("}\n");
            writer.Write("}\n");

            writer.Write("</SCRIPT>\n");

        }

    }
}
