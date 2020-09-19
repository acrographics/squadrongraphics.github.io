// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/products.aspx.cs 6     9/30/06 3:38p Redwoodtree $
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
    /// Summary description for products.
    /// </summary>
    public partial class products : AspDotNetStorefront.SkinBase
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            SectionTitle = "Manage Products"; // + m_EntitySpecs.m_ObjectNamePlural;
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            int CategoryFilterID = CommonLogic.QueryStringUSInt("CategoryFilterID");
            int SectionFilterID = CommonLogic.QueryStringUSInt("SectionFilterID");
            int ProductTypeFilterID = CommonLogic.QueryStringUSInt("ProductTypeFilterID");
            int ManufacturerFilterID = CommonLogic.QueryStringUSInt("ManufacturerFilterID");
            int DistributorFilterID = CommonLogic.QueryStringUSInt("DistributorFilterID");
            int GenreFilterID = CommonLogic.QueryStringUSInt("GenreFilterID");
            int VectorFilterID = CommonLogic.QueryStringUSInt("VectorFilterID");
            int AffiliateFilterID = CommonLogic.QueryStringUSInt("AffiliateFilterID");
            int CustomerLevelFilterID = CommonLogic.QueryStringUSInt("CustomerLevelFilterID");

            String ENCleaned = CommonLogic.QueryStringCanBeDangerousContent("EntityName").Trim().ToUpperInvariant();

            // kludge for now, during conversion to properly entity/object setup:
            if (ENCleaned == "CATEGORY")
            {
                CategoryFilterID = CommonLogic.QueryStringUSInt("EntityFilterID");
                SectionFilterID = 0;
                ProductTypeFilterID = 0;
                ManufacturerFilterID = 0;
                DistributorFilterID = 0;
                GenreFilterID = 0;
                VectorFilterID = 0;
                AffiliateFilterID = 0;
                CustomerLevelFilterID = 0;
            }
            if (ENCleaned == "SECTION")
            {
                CategoryFilterID = 0;
                SectionFilterID = CommonLogic.QueryStringUSInt("EntityFilterID");
                ProductTypeFilterID = 0;
                ManufacturerFilterID = 0;
                DistributorFilterID = 0;
                GenreFilterID = 0;
                VectorFilterID = 0;
                AffiliateFilterID = 0;
                CustomerLevelFilterID = 0;
            }
            if (ENCleaned == "MANUFACTURER")
            {
                CategoryFilterID = 0;
                SectionFilterID = 0;
                ProductTypeFilterID = 0;
                ManufacturerFilterID = CommonLogic.QueryStringUSInt("EntityFilterID");
                DistributorFilterID = 0;
                GenreFilterID = 0;
                VectorFilterID = 0;
                AffiliateFilterID = 0;
                CustomerLevelFilterID = 0;
            }
            if (ENCleaned == "DISTRIBUTOR")
            {
                CategoryFilterID = 0;
                SectionFilterID = 0;
                ProductTypeFilterID = 0;
                ManufacturerFilterID = 0;
                DistributorFilterID = CommonLogic.QueryStringUSInt("EntityFilterID");
                GenreFilterID = 0;
                VectorFilterID = 0;
                AffiliateFilterID = 0;
                CustomerLevelFilterID = 0;
            }
            if (ENCleaned == "GENRE")
            {
                CategoryFilterID = 0;
                SectionFilterID = 0;
                ProductTypeFilterID = 0;
                ManufacturerFilterID = 0;
                DistributorFilterID = 0;
                GenreFilterID = CommonLogic.QueryStringUSInt("EntityFilterID");
                VectorFilterID = 0;
                AffiliateFilterID = 0;
                CustomerLevelFilterID = 0;
            }
            if (ENCleaned == "VECTOR")
            {
                CategoryFilterID = 0;
                SectionFilterID = 0;
                ProductTypeFilterID = 0;
                ManufacturerFilterID = 0;
                DistributorFilterID = 0;
                GenreFilterID = 0;
                VectorFilterID = CommonLogic.QueryStringUSInt("EntityFilterID");
                AffiliateFilterID = 0;
                CustomerLevelFilterID = 0;
            }
            if (ENCleaned == "AFFILIATE")
            {
                CategoryFilterID = 0;
                SectionFilterID = 0;
                ProductTypeFilterID = 0;
                ManufacturerFilterID = 0;
                DistributorFilterID = 0;
                GenreFilterID = 0;
                VectorFilterID = 0;
                AffiliateFilterID = CommonLogic.QueryStringUSInt("EntityFilterID");
                CustomerLevelFilterID = 0;
            }
            if (ENCleaned == "CUSTOMERLEVEL")
            {
                CategoryFilterID = 0;
                SectionFilterID = 0;
                ProductTypeFilterID = 0;
                ManufacturerFilterID = 0;
                DistributorFilterID = 0;
                GenreFilterID = 0;
                VectorFilterID = 0;
                AffiliateFilterID = 0;
                CustomerLevelFilterID = CommonLogic.QueryStringUSInt("EntityFilterID");
            }
            // end kludge


            if (CommonLogic.QueryStringCanBeDangerousContent("CategoryFilterID").Length == 0 && CommonLogic.QueryStringCanBeDangerousContent("EntityFilterID").Length == 0)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilter").Length == 0 && CommonLogic.CookieCanBeDangerousContent("AdminCategoryFilterID", true).Length != 0)
                {
                    CategoryFilterID = CommonLogic.CookieUSInt("AdminCategoryFilterID");
                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("SectionFilterID").Length == 0 && CommonLogic.QueryStringCanBeDangerousContent("EntityFilterID").Length == 0)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilter").Length == 0 && CommonLogic.CookieCanBeDangerousContent("AdminSectionFilterID", true).Length != 0)
                {
                    SectionFilterID = CommonLogic.CookieUSInt("AdminSectionFilterID");
                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("ManufacturerFilterID").Length == 0 && CommonLogic.QueryStringCanBeDangerousContent("EntityFilterID").Length == 0)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilter").Length == 0 && CommonLogic.CookieCanBeDangerousContent("AdminManufacturerFilterID", true).Length != 0)
                {
                    ManufacturerFilterID = CommonLogic.CookieUSInt("AdminManufacturerFilterID");
                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("DistributorFilterID").Length == 0 && CommonLogic.QueryStringCanBeDangerousContent("EntityFilterID").Length == 0)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilter").Length == 0 && CommonLogic.CookieCanBeDangerousContent("AdminDistributorFilterID", true).Length != 0)
                {
                    DistributorFilterID = CommonLogic.CookieUSInt("AdminDistributorFilterID");
                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("GenreFilterID").Length == 0 && CommonLogic.QueryStringCanBeDangerousContent("EntityFilterID").Length == 0)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilter").Length == 0 && CommonLogic.CookieCanBeDangerousContent("AdminGenreFilterID", true).Length != 0)
                {
                    GenreFilterID = CommonLogic.CookieUSInt("AdminGenreFilterID");
                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("VectorFilterID").Length == 0 && CommonLogic.QueryStringCanBeDangerousContent("EntityFilterID").Length == 0)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilter").Length == 0 && CommonLogic.CookieCanBeDangerousContent("AdminVectorFilterID", true).Length != 0)
                {
                    VectorFilterID = CommonLogic.CookieUSInt("AdminVectorFilterID");
                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("AffiliateFilterID").Length == 0 && CommonLogic.QueryStringCanBeDangerousContent("EntityFilterID").Length == 0)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilter").Length == 0 && CommonLogic.CookieCanBeDangerousContent("AdminAffiliateFilterID", true).Length != 0)
                {
                    AffiliateFilterID = CommonLogic.CookieUSInt("AdminAffiliateFilterID");
                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("CustomerLevelFilterID").Length == 0 && CommonLogic.QueryStringCanBeDangerousContent("EntityFilterID").Length == 0)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilter").Length == 0 && CommonLogic.CookieCanBeDangerousContent("AdminCustomerLevelFilterID", true).Length != 0)
                {
                    CustomerLevelFilterID = CommonLogic.CookieUSInt("AdminCustomerLevelFilterID");
                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("ProductTypeFilterID").Length == 0 && CommonLogic.QueryStringCanBeDangerousContent("EntityFilterID").Length == 0)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilter").Length == 0 && CommonLogic.CookieCanBeDangerousContent("AdminProductTypeFilterID", true).Length != 0)
                {
                    ProductTypeFilterID = CommonLogic.CookieUSInt("AdminProductTypeFilterID");
                }
                if (ProductTypeFilterID != 0 && !AppLogic.ProductTypeHasVisibleProducts(ProductTypeFilterID))
                {
                    ProductTypeFilterID = 0;
                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilter").Length != 0)
            {
                CategoryFilterID = 0;
                SectionFilterID = 0;
                ManufacturerFilterID = 0;
                DistributorFilterID = 0;
                GenreFilterID = 0;
                VectorFilterID = 0;
                AffiliateFilterID = 0;
                CustomerLevelFilterID = 0;
                ProductTypeFilterID = 0;
            }

            AppLogic.SetCookie("AdminCategoryFilterID", CategoryFilterID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
            AppLogic.SetCookie("AdminSectionFilterID", SectionFilterID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
            AppLogic.SetCookie("AdminManufacturerFilterID", ManufacturerFilterID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
            AppLogic.SetCookie("AdminDistributorFilterID", DistributorFilterID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
            AppLogic.SetCookie("AdminGenreFilterID", GenreFilterID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
            AppLogic.SetCookie("AdminVectorFilterID", VectorFilterID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
            AppLogic.SetCookie("AdminAffiliateFilterID", AffiliateFilterID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
            AppLogic.SetCookie("AdminCustomerLevelFilterID", CustomerLevelFilterID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
            AppLogic.SetCookie("AdminProductTypeFilterID", ProductTypeFilterID.ToString(), new TimeSpan(365, 0, 0, 0, 0));

            EntityHelper CategoryHelper = AppLogic.LookupHelper(base.EntityHelpers, "Category");
            EntityHelper SectionHelper = AppLogic.LookupHelper(base.EntityHelpers, "Section");
            EntityHelper ManufacturerHelper = AppLogic.LookupHelper(base.EntityHelpers, "Manufacturer");
            EntityHelper DistributorHelper = AppLogic.LookupHelper(base.EntityHelpers, "Distributor");
            EntityHelper GenreHelper = AppLogic.LookupHelper(base.EntityHelpers, "Genre");
            EntityHelper VectorHelper = AppLogic.LookupHelper(base.EntityHelpers, "VECTOR");
            EntityHelper AffiliateHelper = AppLogic.LookupHelper(base.EntityHelpers, "Affiliate");
            EntityHelper CustomerLevelHelper = AppLogic.LookupHelper(base.EntityHelpers, "CustomerLevel");

            if (CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
            {
                int DeleteID = CommonLogic.QueryStringUSInt("DeleteID");
                DB.ExecuteSQL("delete from ShoppingCart where productid=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from kitcart where productid=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from customcart where productid=" + DeleteID.ToString());
                DB.ExecuteSQL("update Product set deleted=1 where ProductID=" + DeleteID.ToString());

                /* Modified by : mark
                 * Date : 11.16.2006
                 * No : 108
                 * Remove all Entity mappings for this product
                 */
                DB.ExecuteSQL("delete from ProductAffiliate where productid=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ProductCategory where productid=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ProductCustomerLevel where productid=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ProductDistributor where productid=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ProductGenre where productid=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ProductVector where productid=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ProductLocaleSetting where productid=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ProductManufacturer where productid=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ProductSection where productid=" + DeleteID.ToString());
                /******* end modification ****************/
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("NukeID").Length != 0)
            {
                int DeleteID = CommonLogic.QueryStringUSInt("NukeID");
                DB.ExecuteLongTimeSQL("aspdnsf_NukeProduct " + DeleteID.ToString(), 120);
            }

            writer.Write("<form id=\"FilterForm\" name=\"FilterForm\" method=\"GET\" action=\"products.aspx\">\n");
            writer.Write("<a href=\"products.aspx?resetfilters=true&categoryfilterid=0&sectionfilterid=0&producttypefilterid=0&manufacturerfilterid=0&distributorfilterid=0&genreid=0&Vectorid=0&affiliatefilterid=0&customerlevelfilterid=0\">RESET FILTERS</a>&nbsp;&nbsp;&nbsp;&nbsp;");

            String CatSel = CategoryHelper.GetEntitySelectList(0, String.Empty, 0, ThisCustomer.LocaleSetting, false);
            // mark current Category:
            CatSel = CatSel.Replace("<option value=\"" + CategoryFilterID.ToString() + "\">", "<option value=\"" + CategoryFilterID.ToString() + "\" selected>");
            if (CategoryHelper.m_TblMgr.NumRootLevelNodes > 0)
            {
                writer.Write(AppLogic.GetString("AppConfig.CategoryPromptSingular", SkinID, ThisCustomer.LocaleSetting) + ": ");
                writer.Write("<select onChange=\"document.FilterForm.submit()\" style=\"font-size: 9px;\" size=\"1\" name=\"CategoryFilterID\">\n");
                writer.Write("<OPTION VALUE=\"0\" " + CommonLogic.IIF(CategoryFilterID == 0, " selected ", "") + ">All " + AppLogic.GetString("AppConfig.CategoryPromptPlural", SkinID, ThisCustomer.LocaleSetting) + "</option>\n");
                writer.Write(CatSel);
                writer.Write("</select>\n");
                writer.Write("&nbsp;&nbsp;");
            }

            String SecSel = SectionHelper.GetEntitySelectList(0, String.Empty, 0, ThisCustomer.LocaleSetting, false);
            if (SectionHelper.m_TblMgr.NumRootLevelNodes > 0)
            {
                SecSel = SecSel.Replace("<option value=\"" + SectionFilterID.ToString() + "\">", "<option value=\"" + SectionFilterID.ToString() + "\" selected>");
                writer.Write(AppLogic.GetString("AppConfig.SectionPromptSingular", SkinID, ThisCustomer.LocaleSetting) + ": ");
                writer.Write("<select onChange=\"document.FilterForm.submit()\" style=\"font-size: 9px;\" size=\"1\" name=\"SectionFilterID\">\n");
                writer.Write("<OPTION VALUE=\"0\" " + CommonLogic.IIF(SectionFilterID == 0, " selected ", "") + ">All " + AppLogic.GetString("AppConfig.SectionPromptPlural", SkinID, ThisCustomer.LocaleSetting) + "</option>\n");
                writer.Write(SecSel);
                writer.Write("</select>\n");
                writer.Write("&nbsp;&nbsp;");
            }

            String MfgSel = ManufacturerHelper.GetEntitySelectList(0, String.Empty, 0, ThisCustomer.LocaleSetting, false);
            if (ManufacturerHelper.m_TblMgr.NumRootLevelNodes > 0)
            {
                MfgSel = MfgSel.Replace("<option value=\"" + ManufacturerFilterID.ToString() + "\">", "<option value=\"" + ManufacturerFilterID.ToString() + "\" selected>");
                writer.Write("Manufacturer: <select size=\"1\" name=\"ManufacturerFilterID\" onChange=\"document.FilterForm.submit();\">\n");
                writer.Write("<OPTION VALUE=\"0\" " + CommonLogic.IIF(ManufacturerFilterID == 0, " selected ", "") + ">All Manufacturers</option>\n");
                writer.Write(MfgSel);
                writer.Write("</select>\n");
                writer.Write("&nbsp;&nbsp;");
            }

            DataSet dsst = DB.GetDS("select * from ProductType  " + DB.GetNoLock() + " order by DisplayOrder,Name", AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
            if (dsst.Tables[0].Rows.Count > 0)
            {
                writer.Write("Product Type: <select size=\"1\" name=\"ProductTypeFilterID\" onChange=\"document.FilterForm.submit();\">\n");
                writer.Write("<OPTION VALUE=\"0\" " + CommonLogic.IIF(ProductTypeFilterID == 0, " selected ", "") + ">All Product Types</option>\n");
                foreach (DataRow row in dsst.Tables[0].Rows)
                {
                    writer.Write("<option value=\"" + DB.RowFieldInt(row, "ProductTypeID").ToString() + "\"");
                    if (DB.RowFieldInt(row, "ProductTypeID") == ProductTypeFilterID)
                    {
                        writer.Write(" selected");
                    }
                    writer.Write(">" + DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting) + "</option>");
                }
                writer.Write("</select>\n");
            }
            dsst.Dispose();

            writer.Write("</form>\n");

            int PageSize = AppLogic.AppConfigUSInt("Admin_ProductPageSize");
            if (PageSize == 0)
            {
                PageSize = 50;
            }
            int PageNum = CommonLogic.QueryStringUSInt("PageNum");
            if (PageNum == 0)
            {
                PageNum = 1;
            }
            bool ShowAll = (CommonLogic.QueryStringCanBeDangerousContent("show").ToUpperInvariant() == "ALL");
            if (ShowAll)
            {
                PageSize = 0;
                PageNum = 1;
            }

            ProductCollection products = new ProductCollection();
            products.PageSize = PageSize;
            products.PageNum = PageNum;
            products.CategoryID = CategoryFilterID;
            products.SectionID = SectionFilterID;
            products.ManufacturerID = ManufacturerFilterID;
            products.DistributorID = DistributorFilterID;
            products.GenreID = GenreFilterID;
            products.VectorID = VectorFilterID;
            products.AffiliateID = AffiliateFilterID;
            products.CustomerLevelID = CustomerLevelFilterID;
            products.ProductTypeID = ProductTypeFilterID;
            products.PublishedOnly = false;
            DataSet dsProducts = products.LoadFromDB();

            String QueryParms = "categoryfilterid=" + CategoryFilterID.ToString() + "&sectionfilterid=" + SectionFilterID.ToString() + "&manufacturerfilterid=" + ManufacturerFilterID.ToString() + "&distributorfilterid=" + DistributorFilterID.ToString() + "&genrefilterid=" + GenreFilterID.ToString() + "&Vectorfilterid=" + VectorFilterID.ToString() + "&affiliatefilterid=" + AffiliateFilterID.ToString() + "&customerlevelfilterid=" + CustomerLevelFilterID.ToString() + "&producttypefilterid=" + ProductTypeFilterID.ToString();

            int NumPages = products.NumPages;

            // ---------------------------------------------------
            // write paging info:
            // ---------------------------------------------------
            if (NumPages > 1 || ShowAll)
            {
                writer.Write("<p class=\"PageNumber\" align=\"left\">");
                writer.Write("<p class=\"PageNumber\" align=\"left\">");
                if (CommonLogic.QueryStringCanBeDangerousContent("show") == "all")
                {
                    writer.Write("Click <a class=\"PageNumber\" href=\"products.aspx?" + QueryParms + "&pagenum=1\">here</a> to turn paging back on.");
                }
                else
                {
                    writer.Write("Page: ");
                    for (int u = 1; u <= NumPages; u++)
                    {
                        if (u == PageNum)
                        {
                            writer.Write(u.ToString() + " ");
                        }
                        else
                        {
                            writer.Write("<a class=\"PageNumber\" href=\"products.aspx?" + QueryParms + "&pagenum=" + u.ToString() + "\">" + u.ToString() + "</a> ");
                        }
                    }
                    writer.Write(" <a class=\"PageNumber\" href=\"products.aspx?" + QueryParms + "&show=all\">all</a>");
                }
                writer.Write("</p>\n");
            }

            writer.Write("<form id=\"Form1\" name=\"Form1\" method=\"POST\" action=\"products.aspx?categoryfilterfilterid=" + CategoryFilterID.ToString() + "&manufacturerFilterID=" + ManufacturerFilterID.ToString() + "&distributorFilterID=" + DistributorFilterID.ToString() + "&genreFilterID=" + GenreFilterID.ToString() + "&VectorFilterID=" + VectorFilterID.ToString() + "&affiliateFilterID=" + AffiliateFilterID.ToString() + "&customerlevelFilterID=" + CustomerLevelFilterID.ToString() + "&producttypefilterid=" + ProductTypeFilterID.ToString() + "\">\n");
            writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
            writer.Write("<p align=\"left\"><input type=\"button\" value=\"Add New Product\" name=\"AddNew\" onClick=\"self.location='editProduct.aspx?categoryfilterid=" + CategoryFilterID.ToString() + "&sectionfilterid=" + SectionFilterID.ToString() + "&manufacturerfilterID=" + ManufacturerFilterID.ToString() + "&producttypefilterid=" + ProductTypeFilterID.ToString() + "';\"></p>");
            writer.Write("  <table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
            writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
            writer.Write("      <td><b>ID</b></td>\n");
            writer.Write("      <td><b>Product</b></td>\n");
            writer.Write("      <td><b>SKU</b></td>\n");
            writer.Write("      <td><b>Mfg Part #</b></td>\n");
            writer.Write("      <td align=\"center\"><b>Inventory</b></td>\n");
            writer.Write("      <td align=\"center\"><b>Edit</b></td>\n");
            writer.Write("      <td align=\"center\"><b>Clone</b></td>\n");
            writer.Write("      <td align=\"center\"><b>Variants</b></td>\n");
            writer.Write("      <td align=\"center\"><b>Ratings</b></td>\n");
            writer.Write("      <td align=\"center\"><b>Soft Delete</b></td>\n");
            writer.Write("      <td align=\"center\"><b>Nuke</b></td>\n");
            writer.Write("    </tr>\n");

            foreach (DataRow row in dsProducts.Tables[0].Rows)
            {
                writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                writer.Write("      <td >" + DB.RowFieldInt(row, "ProductID").ToString() + "</td>\n");
                writer.Write("      <td >");

                String Image1URL = AppLogic.LookupImage("Product", DB.RowFieldInt(row, "ProductID"), "icon", SkinID, ThisCustomer.LocaleSetting);
                writer.Write("<a href=\"editProduct.aspx?Productid=" + DB.RowFieldInt(row, "ProductID").ToString() + "&categoryfilterid=" + CategoryFilterID.ToString() + "&sectionFilterID=" + SectionFilterID.ToString() + "&manufacturerfilterid=" + ManufacturerFilterID.ToString() + "&producttypefilterid=" + ProductTypeFilterID.ToString() + "\">");
                writer.Write("<img src=\"" + Image1URL + "\" height=\"25\" border=\"0\" align=\"absmiddle\">");
                writer.Write("</a>&nbsp;\n");
                writer.Write("<a href=\"editProduct.aspx?Productid=" + DB.RowFieldInt(row, "ProductID").ToString() + "&categoryfilterid=" + CategoryFilterID.ToString() + "&sectionFilterID=" + SectionFilterID.ToString() + "&manufacturerfilterid=" + ManufacturerFilterID.ToString() + "&producttypefilterid=" + ProductTypeFilterID.ToString() + "\">");
                writer.Write(DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting));
                writer.Write("</a>");

                writer.Write("</a>");
                writer.Write("</td>\n");
                writer.Write("<td >" + DB.RowField(row, "SKU") + "</td>\n");
                writer.Write("<td >" + DB.RowField(row, "ManufacturerPartNumber") + "</td>\n");
                writer.Write("<td align=\"center\">");
                if (DB.RowFieldBool(row, "IsSystem"))
                {
                    writer.Write("System<br/>Product"); // this type of product can only be deleted in the db!
                }
                else
                {
                    if (AppLogic.ProductTracksInventoryBySizeAndColor(DB.RowFieldInt(row, "ProductID")))
                    {
                        writer.Write("<a href=\"editinventory.aspx?productid=" + DB.RowFieldInt(row, "ProductID").ToString() + "\">Inventory</a>\n");
                    }
                    else
                    {
                        writer.Write(DB.RowFieldInt(row, "Inventory").ToString());
                    }
                }
                writer.Write("</td>");

                writer.Write("<td align=\"center\">");
                if (DB.RowFieldBool(row, "IsSystem"))
                {
                    writer.Write("System<br/>Product"); // this type of product can only be deleted in the db!
                }
                else
                {
                    writer.Write("<input type=\"button\" value=\"Edit\" name=\"Edit_" + DB.RowFieldInt(row, "ProductID").ToString() + "\" onClick=\"self.location='editProduct.aspx?Productid=" + DB.RowFieldInt(row, "ProductID").ToString() + "&categoryfilterid=" + CategoryFilterID.ToString() + "&sectionfilterID=" + SectionFilterID.ToString() + "&manufacturerfilterid=" + ManufacturerFilterID.ToString() + "&producttypefilterid=" + ProductTypeFilterID.ToString() + "'\">\n");
                }
                writer.Write("</td>");

                writer.Write("<td align=\"center\">");
                if (DB.RowFieldBool(row, "IsSystem"))
                {
                    writer.Write("System<br/>Product"); // this type of product can only be deleted in the db!
                }
                else
                {
                    writer.Write("<input type=\"button\" value=\"Clone\" name=\"Clone_" + DB.RowFieldInt(row, "ProductID").ToString() + "\" onClick=\"if(confirm('Are you sure you want to create a clone of this product, and all variants?')) {self.location='cloneproduct.aspx?productid=" + DB.RowFieldInt(row, "ProductID").ToString() + "&categoryfilterid=" + CategoryFilterID.ToString() + "&sectionfilterID=" + SectionFilterID.ToString() + "&manufacturerfilterid=" + ManufacturerFilterID.ToString() + "&producttypefilterid=" + ProductTypeFilterID.ToString() + "';}\">\n");
                }
                writer.Write("</td>");

                writer.Write("<td align=\"center\">");
                if (DB.RowFieldBool(row, "IsSystem"))
                {
                    writer.Write("System<br/>Product"); // this type of product can only be deleted in the db!
                }
                else
                {
                    int NumVariants = DB.GetSqlN("select count(*) as N from productvariant  " + DB.GetNoLock() + " where deleted=0 and productid=" + DB.RowFieldInt(row, "ProductID").ToString());
                    writer.Write("<input type=\"button\" value=\"Variants (" + NumVariants.ToString() + ")\" name=\"Variants_" + DB.RowFieldInt(row, "ProductID").ToString() + "\" onClick=\"self.location='" + CommonLogic.IIF(NumVariants == 1, "variants.aspx?Productid=" + DB.RowFieldInt(row, "ProductID").ToString(), "variants.aspx?Productid=" + DB.RowFieldInt(row, "ProductID").ToString()) + "'\">\n");
                }
                writer.Write("</td>");

                writer.Write("<td align=\"center\">");
                if (DB.RowFieldBool(row, "IsSystem"))
                {
                    writer.Write("System<br/>Product"); // this type of product can only be deleted in the db!
                }
                else
                {
                    int NumRatings = DB.GetSqlN("select count(*) as N from rating  " + DB.GetNoLock() + " where productid=" + DB.RowFieldInt(row, "ProductID").ToString());
                    writer.Write("<input type=\"button\" value=\"Ratings (" + NumRatings.ToString() + ")\" name=\"Ratings_" + DB.RowFieldInt(row, "ProductID").ToString() + "\" onClick=\"self.location='productratings.aspx?Productid=" + DB.RowFieldInt(row, "ProductID").ToString() + "'\">\n");
                }
                writer.Write("</td>");

                writer.Write("<td align=\"center\">");
                if (DB.RowFieldBool(row, "IsSystem"))
                {
                    writer.Write("System<br/>Product"); // this type of product can only be deleted in the db!
                }
                else
                {
                    writer.Write("<input type=\"button\" value=\"Delete\" name=\"Delete_" + DB.RowFieldInt(row, "ProductID").ToString() + "\" onClick=\"DeleteProduct(" + DB.RowFieldInt(row, "ProductID").ToString() + ")\">");
                }
                writer.Write("</td>");

                writer.Write("<td align=\"center\">");
                if (DB.RowFieldBool(row, "IsSystem"))
                {
                    writer.Write("System<br/>Product"); // this type of product can only be deleted in the db!
                }
                else
                {
                    writer.Write("<input type=\"button\" value=\"Nuke\" name=\"Nuke_" + DB.RowFieldInt(row, "ProductID").ToString() + "\" onClick=\"NukeProduct(" + DB.RowFieldInt(row, "ProductID").ToString() + ")\">");
                }
                writer.Write("</td>");

                writer.Write("</tr>\n");
            }
            products.Dispose();
            writer.Write("  </table>\n");
            writer.Write("<p align=\"left\"><input type=\"button\" value=\"Add New Product\" name=\"AddNew\" onClick=\"self.location='editProduct.aspx?categoryfilterid=" + CategoryFilterID.ToString() + "&sectionfilterid=" + SectionFilterID.ToString() + "&manufacturerfilterid=" + ManufacturerFilterID.ToString() + "&producttypefilterid=" + ProductTypeFilterID.ToString() + "';\"></p>");
            writer.Write("</form>\n");

            if (NumPages > 1 || ShowAll)
            {
                writer.Write("<p class=\"PageNumber\" align=\"left\">");
                writer.Write("<p class=\"PageNumber\" align=\"left\">");
                if (CommonLogic.QueryStringCanBeDangerousContent("show") == "all")
                {
                    writer.Write("Click <a class=\"PageNumber\" href=\"products.aspx?" + QueryParms + "&pagenum=1\">here</a> to turn paging back on.");
                }
                else
                {
                    writer.Write("Page: ");
                    for (int u = 1; u <= NumPages; u++)
                    {
                        if (u == PageNum)
                        {
                            writer.Write(u.ToString() + " ");
                        }
                        else
                        {
                            writer.Write("<a class=\"PageNumber\" href=\"products.aspx?" + QueryParms + "&pagenum=" + u.ToString() + "\">" + u.ToString() + "</a> ");
                        }
                    }
                    writer.Write(" <a class=\"PageNumber\" href=\"products.aspx?" + QueryParms + "&show=all\">all</a>");
                }
                writer.Write("</p>\n");
            }

            writer.Write("<script type=\"text/javascript\">\n");
            writer.Write("function DeleteProduct(id)\n");
            writer.Write("{\n");
            writer.Write("  if(confirm('Are you sure you want to soft-delete product: ' + id + '? This will also remove all items from customer shopping carts, wish lists, and gift registries IF they reference this product! The product will remain in the db, with a delete flag set, so it is not visible on the admin or store sites, but you could recover the product record later if you have to'))\n");
            writer.Write("  {\n");
            writer.Write("      self.location = 'products.aspx?deleteid=' + id;\n");
            writer.Write("  }\n");
            writer.Write("}\n");

            writer.Write("function NukeProduct(id)\n");
            writer.Write("{\n");
            writer.Write("  if(confirm('Are you sure you want to nuke product: ' + id + '? This will also remove all items from customer shopping carts, wish lists, and gift registries IF they reference this product! This action cannot be undone! The product, and it\\'s variants, are completely removed from the database'))\n");
            writer.Write("  {\n");
            writer.Write("      self.location = 'products.aspx?nukeid=' + id;\n");
            writer.Write("  }\n");
            writer.Write("}\n");

            writer.Write("</SCRIPT>\n");
            dsProducts.Dispose();
        }

    }
}
