// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/dyop.aspx.cs 2     9/13/06 11:13p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
  /// <summary>
  /// Summary description for dyop.
  /// </summary>
  public partial class dyop : SkinBase
  {
    int CategoryID;
    int SectionID;
    String CategoryName;
    String SectionName;
    bool RequiresReg;
    int PackID;
    decimal PresetPackPrice;
    String PresetPackProducts;
    String ProductName;
    String ProductDescription;
    String FileDescription;
    bool SpecsInline;
    String LargePic;
    bool HasLargePic;
    String SpecTitle;
    decimal BasePrice;
    int Points;
    EntityHelper m_CategoryHelper;
    EntityHelper m_SectionHelper;
    //private bool m_IsXmlPackage;
    private String m_XmlPackage = String.Empty;

    protected void Page_Load(object sender, System.EventArgs e)
    {
      if (AppLogic.AppConfigBool("GoNonSecureAgain"))
      {
        SkinBase.GoNonSecureAgain();
      }

      PackID = CommonLogic.QueryStringUSInt("PackID");
      if (PackID == 0)
      {
        Response.Redirect("default.aspx");
      }
      if (AppLogic.ProductHasBeenDeleted(PackID))
      {
        Response.Redirect(SE.MakeDriverLink("ProductNotFound"));
      }
      m_CategoryHelper = AppLogic.LookupHelper(base.EntityHelpers, "Category");
      m_SectionHelper = AppLogic.LookupHelper(base.EntityHelpers, "Section");

      CategoryID = CommonLogic.QueryStringUSInt("CategoryID");
      SectionID = CommonLogic.QueryStringUSInt("SectionID");
      if (CategoryID == 0 && SectionID == 0)
      {
        // no category or section passed in, pick first one that this product is mapped to:
        String tmpS = m_CategoryHelper.GetObjectEntities(PackID, true);
        if (tmpS.Length != 0)
        {
          String[] catIDs = tmpS.Split(',');
          CategoryID = Localization.ParseUSInt(catIDs[0]);
        }
        else
        {
          String tmpS2 = m_SectionHelper.GetObjectEntities(PackID, true);
          if (tmpS2.Length != 0)
          {
            String[] secIDs = tmpS2.Split(',');
            SectionID = Localization.ParseUSInt(secIDs[0]);
          }
        }
      }
      CategoryName = m_CategoryHelper.GetEntityName(CategoryID, ThisCustomer.LocaleSetting);
      SectionName = m_SectionHelper.GetEntityName(SectionID, ThisCustomer.LocaleSetting);

      IDataReader rs = DB.GetRS("select product.*,productvariant.price,productvariant.points,productvariant.saleprice from product  " + DB.GetNoLock() + " left outer join productvariant " + DB.GetNoLock() + " on product.productid=productvariant.productid where productvariant.deleted=0 and productvariant.published=1 and product.ProductID=" + PackID.ToString());
      if (!rs.Read())
      {
        rs.Close();
        Response.Redirect("default.aspx");
      }

      base.ContentsBGColor = DB.RSField(rs, "ContentsBGColor");
      base.PageBGColor = DB.RSField(rs, "PageBGColor");
      base.GraphicsColor = DB.RSField(rs, "GraphicsColor");
      SpecsInline = DB.RSFieldBool(rs, "SpecsInline");
      SpecTitle = DB.RSFieldByLocale(rs, "SpecTitle", ThisCustomer.LocaleSetting);
      Points = DB.RSFieldInt(rs, "Points");

      ProductName = DB.RSFieldByLocale(rs, "Name", ThisCustomer.LocaleSetting);
      ProductDescription = DB.RSFieldByLocale(rs, "Description", ThisCustomer.LocaleSetting);
      if (AppLogic.ReplaceImageURLFromAssetMgr)
      {
        ProductDescription = ProductDescription.Replace("../images", "images");
      }
      FileDescription = new ProductDescriptionFile(PackID, ThisCustomer.LocaleSetting, SkinID).Contents;
      if (FileDescription.Length != 0)
      {
        ProductDescription += "<br/>" + FileDescription;
      }
      String ProductPicture = String.Empty;
      bool m_WatermarksEnabled = AppLogic.AppConfigBool("Watermark.Enabled");
      if (m_WatermarksEnabled)
      {
        ProductPicture = String.Format("wolthuis.aspx?productid={0}&size=medium", PackID.ToString());
      }
      else
      {
        ProductPicture = AppLogic.LookupImage("Product", PackID, "medium", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
      }
      LargePic = AppLogic.LookupImage("Product", PackID, "large", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
      HasLargePic = (LargePic.Length != 0);
      String LargePicWatermarked = String.Format("wolthuis.aspx?productid={0}&size=large", PackID.ToString());
      String LargePicForPopup = LargePic;
      if (m_WatermarksEnabled)
      {
        LargePicForPopup = Server.UrlEncode(LargePicWatermarked);
      }

      RequiresReg = DB.RSFieldBool(rs, "RequiresRegistration");

      BasePrice = System.Decimal.Zero;
      if (DB.RSFieldDecimal(rs, "SalePrice") != System.Decimal.Zero)
      {
        BasePrice = DB.RSFieldDecimal(rs, "SalePrice");
      }
      else
      {
        BasePrice = DB.RSFieldDecimal(rs, "Price");
      }
      rs.Close();
    }

    protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
    {
        if (RequiresReg && !ThisCustomer.IsRegistered)
      {
        writer.Write("<br/><br/><br/><br/><b>" + AppLogic.GetString("dyop.aspx.1", SkinID, ThisCustomer.LocaleSetting) + "</b><br/><br/><br/><a href=\"signin.aspx?returnurl=showproduct.aspx?" + Server.HtmlEncode(Server.UrlEncode(CommonLogic.ServerVariables("QUERY_STRING"))) + "\">" + AppLogic.GetString("dyop.aspx.2", SkinID, ThisCustomer.LocaleSetting) + "</a> " + AppLogic.GetString("dyop.aspx.3", SkinID, ThisCustomer.LocaleSetting));
      }
      else
      {
        PresetPackPrice = System.Decimal.Zero;
        PresetPackProducts = String.Empty;
        AppLogic.PresetPack(ThisCustomer, PackID, CartTypeEnum.ShoppingCart, out PresetPackPrice, out PresetPackProducts);

        m_XmlPackage = AppLogic.GetProductXmlPackage(PackID);
        XmlPackage2 p = new XmlPackage2(m_XmlPackage, ThisCustomer, SkinID, String.Empty, String.Empty, String.Empty, true);
        writer.Write(AppLogic.RunXmlPackage(p, null, ThisCustomer, SkinID, false, false));
        this.SETitle = p.SETitle;
        this.SEDescription = p.SEDescription;
        this.SEKeywords = p.SEKeywords;
        this.SENoScript = p.SENoScript;
        this.SectionTitle = p.SectionTitle;

      }
    }

  }
}
