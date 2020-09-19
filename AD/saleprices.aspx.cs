// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/saleprices.aspx.cs 7     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for saleprices.
    /// </summary>
    public partial class saleprices : AspDotNetStorefront.SkinBase
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            SectionTitle = "Products On Sale";
            if (CommonLogic.FormBool("IsSubmit"))
            {
                for (int i = 0; i <= Request.Form.Count - 1; i++)
                {
                    if (Request.Form.Keys[i].IndexOf("XPrice_") != -1 && Request.Form.Keys[i].IndexOf("_vldt") == -1)
                    {
                        String[] keys = Request.Form.Keys[i].Split('_');
                        int VariantID = Localization.ParseUSInt(keys[1]);
                        decimal Price = System.Decimal.Zero;
                        try
                        {
                            if (CommonLogic.FormCanBeDangerousContent("XPrice_" + VariantID.ToString()).Length != 0)
                            {
                                Price = CommonLogic.FormUSDecimal("XPrice_" + VariantID.ToString());
                            }
                            DB.ExecuteSQL("update ProductVariant set Price=" + CommonLogic.IIF(Price != System.Decimal.Zero, Localization.CurrencyStringForDBWithoutExchangeRate(Price), "NULL") + " where VariantID=" + VariantID.ToString());
                        }
                        catch { }
                    }
                    if (Request.Form.Keys[i].IndexOf("YPrice") != -1 && Request.Form.Keys[i].IndexOf("_vldt") == -1)
                    {
                        String[] keys = Request.Form.Keys[i].Split('_');
                        int VariantID = Localization.ParseUSInt(keys[1]);
                        decimal SalePrice = System.Decimal.Zero;
                        try
                        {
                            if (CommonLogic.FormCanBeDangerousContent("YPrice_" + VariantID.ToString()).Length != 0)
                            {
                                SalePrice = CommonLogic.FormUSDecimal("YPrice_" + VariantID.ToString());
                            }
                            DB.ExecuteSQL("update ProductVariant set SalePrice=" + CommonLogic.IIF(SalePrice != System.Decimal.Zero, Localization.CurrencyStringForDBWithoutExchangeRate(SalePrice), "NULL") + " where VariantID=" + VariantID.ToString());
                        }
                        catch { }
                    }
                }
            }
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
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("NukeID").Length != 0)
            {
                int DeleteID = CommonLogic.QueryStringUSInt("NukeID");
                DB.ExecuteLongTimeSQL("aspdnsf_NukeProduct " + DeleteID.ToString(), 120);
            }

            writer.Write("<form id=\"FilterForm\" name=\"FilterForm\" method=\"GET\" action=\"saleprices.aspx\">\n");
            writer.Write("<a href=\"saleprices.aspx?resetfilters=true&categoryfilterid=0&sectionfilterid=0&producttypefilterid=0&manufacturerfilterid=0&distributorfilterid=0&genreid=0&Vectorid=0&affiliatefilterid=0&customerlevelfilterid=0\">RESET FILTERS</a>&nbsp;&nbsp;&nbsp;&nbsp;");

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

            //String DistSel = DistributorHelper.GetEntitySelectList(0, String.Empty, 0, ThisCustomer.LocaleSetting, false);
            //if (DistributorHelper.m_TblMgr.NumRootLevelNodes > 0)
            //{
            //    DistSel = DistSel.Replace("<option value=\"" + DistributorFilterID.ToString() + "\">", "<option value=\"" + DistributorFilterID.ToString() + "\" selected>");
            //    writer.Write("Distributor: <select size=\"1\" name=\"DistributorFilterID\" onChange=\"document.FilterForm.submit();\">\n");
            //    writer.Write("<OPTION VALUE=\"0\" " + CommonLogic.IIF(DistributorFilterID == 0, " selected ", "") + ">All Distributors</option>\n");
            //    writer.Write(DistSel);
            //    writer.Write("</select>\n");
            //    writer.Write("&nbsp;&nbsp;");
            //}

            //String AffSel = AffiliateHelper.GetEntitySelectList(0, String.Empty, 0, ThisCustomer.LocaleSetting, false);
            //if (AffiliateHelper.m_TblMgr.NumRootLevelNodes > 0)
            //{
            //    AffSel = AffSel.Replace("<option value=\"" + AffiliateFilterID.ToString() + "\">", "<option value=\"" + AffiliateFilterID.ToString() + "\" selected>");
            //    writer.Write("Affiliate: <select size=\"1\" name=\"AffiliateFilterID\" onChange=\"document.FilterForm.submit();\">\n");
            //    writer.Write("<OPTION VALUE=\"0\" " + CommonLogic.IIF(AffiliateFilterID == 0, " selected ", "") + ">All Affiliates</option>\n");
            //    writer.Write(AffSel);
            //    writer.Write("</select>\n");
            //    writer.Write("&nbsp;&nbsp;");
            //}

            //String CLSel = CustomerLevelHelper.GetEntitySelectList(0, String.Empty, 0, ThisCustomer.LocaleSetting, false);
            //if (CustomerLevelHelper.m_TblMgr.NumRootLevelNodes > 0)
            //{
            //    CLSel = CLSel.Replace("<option value=\"" + CustomerLevelFilterID.ToString() + "\">", "<option value=\"" + CustomerLevelFilterID.ToString() + "\" selected>");
            //    writer.Write("CustomerLevel: <select size=\"1\" name=\"CustomerLevelFilterID\" onChange=\"document.FilterForm.submit();\">\n");
            //    writer.Write("<OPTION VALUE=\"0\" " + CommonLogic.IIF(CustomerLevelFilterID == 0, " selected ", "") + ">All CustomerLevels</option>\n");
            //    writer.Write(CLSel);
            //    writer.Write("</select>\n");
            //    writer.Write("&nbsp;&nbsp;");
            //}

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
            products.OnSaleOnly = true;
            products.ReturnAllVariants = true;
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
                    writer.Write("Click <a class=\"PageNumber\" href=\"saleprices.aspx?" + QueryParms + "&pagenum=1\">here</a> to turn paging back on.");
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
                            writer.Write("<a class=\"PageNumber\" href=\"saleprices.aspx?" + QueryParms + "&pagenum=" + u.ToString() + "\">" + u.ToString() + "</a> ");
                        }
                    }
                    writer.Write(" <a class=\"PageNumber\" href=\"saleprices.aspx?" + QueryParms + "&show=all\">all</a>");
                }
                writer.Write("</p>\n");
            }

            writer.Write("<script type=\"text/javascript\">\n");
            writer.Write("function PriceForm_Validator(theForm)\n");
            writer.Write("{\n");
            writer.Write("submitonce(theForm);\n");
            writer.Write("return (true);\n");
            writer.Write("}\n");
            writer.Write("</script>\n");

            writer.Write("<form id=\"PriceForm\" name=\"PriceForm\" method=\"POST\" action=\"saleprices.aspx?categoryfilterfilterid=" + CategoryFilterID.ToString() + "&manufacturerFilterID=" + ManufacturerFilterID.ToString() + "&distributorFilterID=" + DistributorFilterID.ToString() + "&genreFilterID=" + GenreFilterID.ToString() + "&distributorFilterID=" + VectorFilterID.ToString() + "&VectorFilterID=" + AffiliateFilterID.ToString() + "&customerlevelFilterID=" + CustomerLevelFilterID.ToString() + "&producttypefilterid=" + ProductTypeFilterID.ToString() + "&pagenum=" + PageNum.ToString() + "\" onsubmit=\"return (validateForm(this) && PriceForm_Validator(this))\" >\n");
            writer.Write("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
            writer.Write("  <table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
            writer.Write("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
            writer.Write("      <td><b>Product ID</b></td>\n");
            writer.Write("      <td><b>Variant ID</b></td>\n");
            writer.Write("      <td><b>Product</b></td>\n");
            writer.Write("      <td><b>SKU</b></td>\n");
            writer.Write("      <td><b>Price</b></td>\n");
            writer.Write("      <td><b>SalePrice</b></td>\n");
            writer.Write("    </tr>\n");
            foreach (DataRow row in dsProducts.Tables[0].Rows)
            {
                writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                writer.Write("<td >" + DB.RowFieldInt(row, "ProductID").ToString() + "</td>\n");
                writer.Write("<td >" + DB.RowFieldInt(row, "VariantID").ToString() + "</td>\n");
                writer.Write("<td >");

                String Image1URL = AppLogic.LookupImage("Product", DB.RowFieldInt(row, "ProductID"), "icon", SkinID, ThisCustomer.LocaleSetting);
                if (Image1URL.Length != 0)
                {
                    writer.Write("<a href=\"editProduct.aspx?Productid=" + DB.RowFieldInt(row, "ProductID").ToString() + "\">");
                    writer.Write("<img src=\"" + Image1URL + "\" height=\"25\" border=\"0\" align=\"absmiddle\">");
                    writer.Write("</a>&nbsp;\n");
                }
                writer.Write("<a href=\"editProduct.aspx?Productid=" + DB.RowFieldInt(row, "ProductID").ToString() + "\">");
                writer.Write(DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting));
                if (DB.RowField(row, "VName").Length != 0)
                {
                    writer.Write(" - ");
                }
                writer.Write(DB.RowField(row, "VName"));
                writer.Write("</a>");

                writer.Write("</a>");
                writer.Write("</td>\n");
                writer.Write("<td>" + DB.RowField(row, "SKU") + DB.RowField(row, "SKUSuffix") + "</td>\n");
                writer.Write("<td>");
                writer.Write("<input name=\"XPrice_" + DB.RowFieldInt(row, "VariantID").ToString() + "\" type=\"text\" size=\"10\" value=\"" + Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(row, "Price")) + "\">");
                writer.Write("<input name=\"XPrice_" + DB.RowFieldInt(row, "VariantID").ToString() + "_vldt\" type=\"hidden\" value=\"[number][invalidalert=please enter a valid dollar amount]\">");
                writer.Write("</td>\n");
                writer.Write("<td>");
                writer.Write("<input name=\"YPrice_" + DB.RowFieldInt(row, "VariantID").ToString() + "\" type=\"text\" size=\"10\" value=\"" + CommonLogic.IIF(DB.RowFieldDecimal(row, "SalePrice") != System.Decimal.Zero, Localization.CurrencyStringForDBWithoutExchangeRate(DB.RowFieldDecimal(row, "SalePrice")), "") + "\">");
                writer.Write("<input name=\"YPrice_" + DB.RowFieldInt(row, "VariantID").ToString() + "_vldt\" type=\"hidden\" value=\"[number][invalidalert=please enter a valid dollar amount]\">");
                writer.Write("</td>\n");
                writer.Write("</tr>\n");
            }
            products.Dispose();
            writer.Write("</table>\n");
            writer.Write("<p align=\"right\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></p>\n");
            writer.Write("</form>\n");

            dsProducts.Dispose();
        }

    }
}
