// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/emailproduct.aspx.cs 6     9/13/06 11:13p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Text;
using System.Globalization;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for EMailproduct.
    /// </summary>
    public partial class EMailproduct : SkinBase
    {
        int ProductID;
        int CategoryID;
        String ProductName = string.Empty;
        String VariantName = string.Empty;
        String SEName = string.Empty;
        String ProductDescription = string.Empty;
        bool RequiresReg;

        protected void Page_Load(object sender, EventArgs e)
        {
            ProductID = CommonLogic.QueryStringUSInt("ProductID");
            EntityHelper CategoryHelper = AppLogic.LookupHelper("Category");
            CategoryID = CommonLogic.QueryStringUSInt("CategoryID");

            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }
            // DOS attack prevention:
            if (AppLogic.OnLiveServer() && (Request.UrlReferrer == null || Request.UrlReferrer.Authority != Request.Url.Authority))
            {
                //Response.Redirect("default.aspx", true);
                Response.Redirect(SE.MakeDriverLink("EmailError")); 
            }
            if (ProductID == 0)
            {
                Response.Redirect("default.aspx");
            }
            if (AppLogic.ProductHasBeenDeleted(ProductID))
            {
                Response.Redirect(SE.MakeDriverLink("ProductNotFound"));
            }

            IDataReader rs = DB.GetRS("select p.*, pv.name variantname from product p " + DB.GetNoLock() + " join productvariant pv " + DB.GetNoLock() + " on p.ProductID = pv.ProductID and pv.isdefault = 1 where p.ProductID=" + ProductID.ToString());
            if (!rs.Read())
            {
                rs.Close();
                Response.Redirect("default.aspx");
            }
            SEName = DB.RSField(rs, "SEName");
            ProductName = DB.RSFieldByLocale(rs, "Name", ThisCustomer.LocaleSetting);
            VariantName = DB.RSFieldByLocale(rs, "VariantName", ThisCustomer.LocaleSetting);

            RequiresReg = DB.RSFieldBool(rs, "RequiresRegistration");
            //int ProductDisplayFormatID = DB.RSFieldInt(rs, "ProductDisplayFormatID");
            ProductDescription = DB.RSFieldByLocale(rs, "Description", ThisCustomer.LocaleSetting); //.Replace("\n","<br/>");
            if (AppLogic.ReplaceImageURLFromAssetMgr)
            {
                ProductDescription = ProductDescription.Replace("../images", "images");
            }
            String FileDescription = new ProductDescriptionFile(ProductID, ThisCustomer.LocaleSetting, SkinID).Contents;
            if (FileDescription.Length != 0)
            {
                ProductDescription += "<div align=\"left\">" + FileDescription + "</div>";
            }
            rs.Close();

            if (CategoryID == 0)
            {
                // no category passed in, pick first one that this product is mapped to:
                String tmpS = CategoryHelper.GetObjectEntities(ProductID, false);
                if (tmpS.Length != 0)
                {
                    String[] catIDs = tmpS.Split(',');
                    CategoryID = Localization.ParseUSInt(catIDs[0]);
                }
            }
            String CategoryName = CategoryHelper.GetEntityName(CategoryID, ThisCustomer.LocaleSetting);

            SectionTitle = String.Empty;
            int pid = CategoryHelper.GetParentEntity(CategoryID);
            while (pid != 0)
            {
                SectionTitle = "<a class=\"SectionTitleText\" href=\"" + SE.MakeCategoryLink(pid, "") + "\">" + CategoryHelper.GetEntityName(pid, ThisCustomer.LocaleSetting) + "</a> - " + SectionTitle;
                pid = CategoryHelper.GetParentEntity(pid);
            }
            SectionTitle += "<a class=\"SectionTitleText\" href=\"" + SE.MakeCategoryLink(CategoryID, "") + "\">" + CategoryName + "</a> - ";
            SectionTitle += ProductName;
            SectionTitle += "</span>";

            reqToAddress.ErrorMessage = AppLogic.GetString("emailproduct.aspx.13", SkinID, ThisCustomer.LocaleSetting);
            regexToAddress.ErrorMessage = AppLogic.GetString("emailproduct.aspx.14", SkinID, ThisCustomer.LocaleSetting);
            reqFromAddress.ErrorMessage = AppLogic.GetString("emailproduct.aspx.16", SkinID, ThisCustomer.LocaleSetting);
            regexFromAddress.ErrorMessage = AppLogic.GetString("emailproduct.aspx.17", SkinID, ThisCustomer.LocaleSetting);

            if (!this.IsPostBack)
            {
                InitializePageContent();
            }
        }

        private void InitializePageContent()
        {
            pnlRequireReg.Visible = (RequiresReg && !ThisCustomer.IsRegistered);
            this.pnlEmailToFriend.Visible = !(RequiresReg && !ThisCustomer.IsRegistered);

            emailproduct_aspx_1.Text = "<br/><br/><br/><br/><b>" + AppLogic.GetString("emailproduct.aspx.1", SkinID, ThisCustomer.LocaleSetting) + "</b><br/><br/><br/><a href=\"signin.aspx?returnurl=showproduct.aspx?" + Server.HtmlEncode(Server.UrlEncode(CommonLogic.ServerVariables("QUERY_STRING"))) + "\">" + AppLogic.GetString("emailproduct.aspx.2", SkinID, ThisCustomer.LocaleSetting) + "</a> " + AppLogic.GetString("emailproduct.aspx.3", SkinID, ThisCustomer.LocaleSetting);

            String ProdPic = String.Empty;
            bool m_WatermarksEnabled = AppLogic.AppConfigBool("Watermark.Enabled");
            if (m_WatermarksEnabled)
            {
                ProdPic = String.Format("wolthuis.aspx?productid={0}&size=medium", ProductID.ToString());
            }
            else
            {
                ProdPic = AppLogic.LookupImage("Product", ProductID, "medium", SkinID, ThisCustomer.LocaleSetting);
            }
            imgProduct.ImageUrl = ProdPic;
            ProductNavLink.NavigateUrl = SE.MakeProductAndCategoryLink(ProductID, CategoryID, SEName);
            ProductNavLink.Text = AppLogic.GetString("emailproduct.aspx.24", SkinID, ThisCustomer.LocaleSetting);
            emailproduct_aspx_4.Text = AppLogic.GetString("emailproduct.aspx.4", SkinID, ThisCustomer.LocaleSetting) + " " + ProductName + CommonLogic.IIF(VariantName.Length > 0, " - " + VariantName, "");
            emailproduct_aspx_11.Text = AppLogic.GetString("emailproduct.aspx.11", SkinID, ThisCustomer.LocaleSetting);
            emailproduct_aspx_12.Text = AppLogic.GetString("emailproduct.aspx.12", SkinID, ThisCustomer.LocaleSetting);
            emailproduct_aspx_22.Text = AppLogic.GetString("emailproduct.aspx.22", SkinID, ThisCustomer.LocaleSetting);
            emailproduct_aspx_15.Text = AppLogic.GetString("emailproduct.aspx.15", SkinID, ThisCustomer.LocaleSetting);
            emailproduct_aspx_18.Text = AppLogic.GetString("emailproduct.aspx.18", SkinID, ThisCustomer.LocaleSetting);
            emailproduct_aspx_19.Text = AppLogic.GetString("emailproduct.aspx.19", SkinID, ThisCustomer.LocaleSetting);
            txtMessage.Text = AppLogic.GetString("emailproduct.aspx.23", SkinID, ThisCustomer.LocaleSetting);
            btnSubmit.Text = AppLogic.GetString("emailproduct.aspx.20", SkinID, ThisCustomer.LocaleSetting);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {
                String FromAddress = txtFromAddress.Text;
                String ToAddress = txtToAddress.Text;
                String BotAddress = AppLogic.AppConfig("ReceiptEMailFrom");
                String Subject = AppLogic.AppConfig("StoreName") + " - " + ProductName;
                StringBuilder Body = new StringBuilder(4096);
                Body.Append(AppLogic.RunXmlPackage("notification.emailproduct.xml.config", null, ThisCustomer, SkinID, "", "Subject=" + Subject, false, false, this.EntityHelpers));

                //try
                //{
                AppLogic.SendMail(Subject, Body.ToString(), true, BotAddress, BotAddress, ToAddress, ToAddress, "", FromAddress, AppLogic.MailServer());
                emailproduct_aspx_8.Text = AppLogic.GetString("emailproduct.aspx.8", SkinID, ThisCustomer.LocaleSetting);
                pnlSuccess.Visible = true;
                pnlRequireReg.Visible = false;
                pnlEmailToFriend.Visible = false;
                //}
                //catch (Exception ex)
                //{
                //    emailproduct_aspx_8.Text = String.Format(AppLogic.GetString("emailproduct.aspx.9", SkinID, ThisCustomer.LocaleSetting), CommonLogic.GetExceptionDetail(ex, "<br/>"));
                //}
                ReturnToProduct.Text = AppLogic.GetString("emailproduct.aspx.10", SkinID, ThisCustomer.LocaleSetting);
                ReturnToProduct.NavigateUrl = SE.MakeProductAndCategoryLink(ProductID, CategoryID, SEName);
            }
            else
            {
                InitializePageContent();
            }
        }

        override protected void OnInit(EventArgs e)
        {
            String HT = String.Empty;
            if (CommonLogic.QueryStringUSInt("CategoryID") != 0)
            {
                HT = AppLogic.GetCurrentEntityTemplateName(EntityDefinitions.readonly_CategoryEntitySpecs.m_EntityName);
            }
            else if (CommonLogic.QueryStringUSInt("SectionID") != 0)
            {
                HT = AppLogic.GetCurrentEntityTemplateName(EntityDefinitions.readonly_SectionEntitySpecs.m_EntityName);
            }
            else if (CommonLogic.QueryStringUSInt("ManufacturerID") != 0)
            {
                HT = AppLogic.GetCurrentEntityTemplateName(EntityDefinitions.readonly_ManufacturerEntitySpecs.m_EntityName);
            }
            else if (CommonLogic.QueryStringUSInt("DistributorID") != 0)
            {
                HT = AppLogic.GetCurrentEntityTemplateName(EntityDefinitions.readonly_DistributorEntitySpecs.m_EntityName);
            }
            else if (CommonLogic.QueryStringUSInt("GenreID") != 0)
            {
                HT = AppLogic.GetCurrentEntityTemplateName(EntityDefinitions.readonly_GenreEntitySpecs.m_EntityName);
            }
            else if (CommonLogic.QueryStringUSInt("VectorID") != 0)
            {
                HT = AppLogic.GetCurrentEntityTemplateName(EntityDefinitions.readonly_VectorEntitySpecs.m_EntityName);
            }
            SetTemplate(HT);
            base.OnInit(e);
        }


    }
}
