// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2006.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/phonesearch.aspx.cs 6     9/30/06 3:38p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;

using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for phonesearch.
    /// </summary>
    public partial class phonesearch : AspDotNetStorefront.SkinBase
    {

        String m_IGD = String.Empty;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            m_IGD = CommonLogic.QueryStringCanBeDangerousContent("IGD");

            EntityHelper CategoryHelper = AppLogic.LookupHelper(base.EntityHelpers, "Category");
            EntityHelper SectionHelper = AppLogic.LookupHelper(base.EntityHelpers, "Section");
            EntityHelper ManufacturerHelper = AppLogic.LookupHelper(base.EntityHelpers, "Manufacturer");
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write("<script type=\"text/javascript\">\n");
            writer.Write("function SearchForm2_Validator(theForm)\n");
            writer.Write("{\n");
            writer.Write("  if (theForm.SearchTerm.value.length < 3)\n");
            writer.Write("  {\n");
            writer.Write("    alert('Please enter at least 3 characters in the Search For field.');\n");
            writer.Write("    theForm.SearchTerm.focus();\n");
            writer.Write("    return (false);\n");
            writer.Write("  }\n");
            writer.Write("  return (true);\n");
            writer.Write("}\n");
            writer.Write("</script>\n");

            writer.Write("<form method=\"GET\" action=\"phonesearch.aspx\" onsubmit=\"return (validateForm(this) && SearchForm2_Validator(this))\" name=\"SearchForm2\">\n");
            writer.Write("<input type=\"hidden\" name=\"IGD\" id=\"IGD\" value=\"" + m_IGD + "\">");
            writer.Write("<p>Search for product name, id, sku, or description:</p>\n");
            writer.Write("<input type=\"text\" name=\"SearchTerm\" size=\"25\" maxlength=\"70\" value=\"" + Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("SearchTerm")) + "\">\n");
            writer.Write("<input type=\"hidden\" name=\"SearchTerm_Vldt\" value=\"[req][blankalert=Please enter something to search for!]\">\n");
            writer.Write("&nbsp;<input type=\"submit\" value=\"Search\" name=\"B1\"></td>\n");
            writer.Write("</form>\n");

            String st = CommonLogic.QueryStringCanBeDangerousContent("SearchTerm").Trim();
            if (st.Length != 0)
            {
                String stlike = "%" + st + "%";
                String stquoted = DB.SQuote(stlike);

                // MATCHING PRODUCTS:
                ProductCollection products = new ProductCollection();
                products.PageSize = 0;
                products.PageNum = 1;
                products.SearchMatch = st;
                products.SearchDescriptionAndSummaryFields = false;
                products.PublishedOnly = false;
                products.ExcludePacks = true;
                DataSet dsProducts = products.LoadFromDB();
                int NumProducts = products.NumProducts;

                bool anyFound = false;
                if (NumProducts > 0)
                {
                    anyFound = true;
                    foreach (DataRow row in dsProducts.Tables[0].Rows)
                    {
                        String url = "javascript:window.parent.frames['RightPanel2Frame'].location.href='../showproduct.aspx?IGD=" + m_IGD + "&amp;productid=" + DB.RowFieldInt(row, "ProductID").ToString() + "';javascript:void(0);";

                        writer.Write("<a href=\"" + url + "\">" + AppLogic.MakeProperObjectName(DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting), DB.RowFieldByLocale(row, "VariantName", ThisCustomer.LocaleSetting), ThisCustomer.LocaleSetting) + "</a>");
                        writer.Write("<br/>");
                        anyFound = true;
                    }

                }
                products.Dispose();
                dsProducts.Dispose();

                if (!anyFound)
                {
                    writer.Write("No matches found\n");
                }
            }

        }

        override protected void OnInit(EventArgs e)
        {
            SetTemplate("empty.ascx");
            base.OnInit(e);
        }
    }
}
