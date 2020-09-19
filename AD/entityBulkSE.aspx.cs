// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/entityBulkSE.aspx.cs 7     10/03/06 7:56p Redwoodtree $
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
    public partial class entityBulkSE : System.Web.UI.Page
    {
        int EntityID;
        String EntityName;
        EntitySpecs m_EntitySpecs;
        EntityHelper Helper;
        private Customer ThisCustomer;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            EntityID = CommonLogic.QueryStringUSInt("EntityID"); ;
            EntityName = CommonLogic.QueryStringCanBeDangerousContent("EntityName");
            m_EntitySpecs = EntityDefinitions.LookupSpecs(EntityName);
            Helper = new EntityHelper(m_EntitySpecs);
            //Helper = AppLogic.LookupHelper(base.EntityHelpers, m_EntitySpecs.m_EntityName);

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
                        string inputVal = AppLogic.FormLocaleXml("SETitle", CommonLogic.FormCanBeDangerousContent(FieldName), ThisCustomer.ThisCustomerSession.Session("entityUserLocale"), "Product", TheProductID);
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
                        string inputVal = AppLogic.FormLocaleXml("SEKeywords", CommonLogic.FormCanBeDangerousContent(FieldName), ThisCustomer.ThisCustomerSession.Session("entityUserLocale"), "Product", TheProductID);
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
                        string inputVal = AppLogic.FormLocaleXml("SEDescription", CommonLogic.FormCanBeDangerousContent(FieldName), ThisCustomer.ThisCustomerSession.Session("entityUserLocale"), "Product", TheProductID);
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
                        string inputVal = AppLogic.FormLocaleXml("SENoScript", CommonLogic.FormCanBeDangerousContent(FieldName), ThisCustomer.ThisCustomerSession.Session("entityUserLocale"), "Product", TheProductID);
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
                    //    string inputVal = AppLogic.FormLocaleXml("SEAltText", CommonLogic.FormCanBeDangerousContent(FieldName), ThisCustomer.ThisCustomerSession.Session("entityUserLocale"), "Product", TheProductID);
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

            LoadBody();
        }

        protected void LoadBody()
        {
            ProductCollection products = new ProductCollection(m_EntitySpecs.m_EntityName, EntityID);
            products.PageSize = 0;
            products.PageNum = 1;
            products.PublishedOnly = false;
            products.ReturnAllVariants = false;
            DataSet dsProducts = products.LoadFromDB();
            int NumProducts = products.NumProducts;
            if (NumProducts > 0)
            {
                ltBody.Text += ("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                ltBody.Text += ("<table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
                ltBody.Text += ("<tr><td colspan=\"5\" align=\"right\"><input type=\"submit\" value=\"Search Engine Update\" name=\"Submit\" class=\"normalButton\"></td></tr>\n");                
                ltBody.Text += ("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                ltBody.Text += ("<td><b>ProductID</b></td>\n");
                ltBody.Text += ("<td><b>VariantID</b></td>\n");
                ltBody.Text += ("<td><b>Product Name</b></td>\n");
                ltBody.Text += ("<td><b>Variant Name</b></td>\n");
                ltBody.Text += ("<td align=\"left\"><b>Product Fields</b></td>\n");
                //ltBody.Text += ("<td align=\"left\"><b>Variant Fields</b></td>\n");
                ltBody.Text += ("</tr>\n");
                int LastProductID = 0;
                foreach (DataRow row in dsProducts.Tables[0].Rows)
                {
                    int ThisProductID = DB.RowFieldInt(row, "ProductID");
                    int ThisVariantID = DB.RowFieldInt(row, "VariantID");
                    ltBody.Text += ("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                    ltBody.Text += ("<td align=\"left\" valign=\"top\">");
                    ltBody.Text += (ThisProductID.ToString());
                    ltBody.Text += ("</td>");
                    ltBody.Text += ("<td align=\"left\" valign=\"top\">");
                    ltBody.Text += (ThisVariantID.ToString());
                    ltBody.Text += ("</td>");
                    ltBody.Text += ("<td align=\"left\" valign=\"top\">");
                    ltBody.Text += ("<a href=\"editproduct.aspx?productid=" + ThisProductID.ToString() + "\">");
                    ltBody.Text += (DB.RowFieldByLocale(row, "Name", ThisCustomer.LocaleSetting));
                    ltBody.Text += ("</a>");
                    ltBody.Text += ("</td>\n");
                    ltBody.Text += ("<td align=\"left\" valign=\"top\">");
                    ltBody.Text += ("<a href=\"editvariant.aspx?productid=" + ThisProductID.ToString() + "&variantid=" + ThisVariantID.ToString() + "\">");
                    ltBody.Text += (DB.RowFieldByLocale(row, "VariantName", ThisCustomer.LocaleSetting));
                    ltBody.Text += ("</a>");
                    ltBody.Text += ("</td>\n");
                    ltBody.Text += ("<td align=\"left\" valign=\"top\">");
                    ltBody.Text += ("<div align=\"left\">");
                    ltBody.Text += ("<b>Search Engine Page Title:</b><br/>");
                    ltBody.Text += ("<input maxLength=\"100\" name=\"SETitle_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" id=\"SETitle_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" value=\"" + XmlCommon.GetLocaleEntry(DB.RowField(row, "SETitle"), ThisCustomer.ThisCustomerSession.Session("entityUserLocale"), false) + "\" class=\"singleLongest\" /><br/>");
                    ltBody.Text += ("<b>Search Engine Keywords:</b><br/>");
                    ltBody.Text += ("<input maxLength=\"255\" name=\"SEKeywords_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" id=\"SEKeywords" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" value=\"" + XmlCommon.GetLocaleEntry(DB.RowField(row, "SEKeywords"), ThisCustomer.ThisCustomerSession.Session("entityUserLocale"), false) + "\" class=\"singleLongest\" /><br/>");
                    ltBody.Text += ("<b>Search Engine Description:</b><br/>");
                    ltBody.Text += ("<input maxLength=\"255\" name=\"SEDescription_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" id=\"SEDescription" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" value=\"" + XmlCommon.GetLocaleEntry(DB.RowField(row, "SEDescription"), ThisCustomer.ThisCustomerSession.Session("entityUserLocale"), false) + "\" class=\"singleLongest\" /><br/>");
                    ltBody.Text += ("<b>Search Engine NoScript:</b><br/>");
                    ltBody.Text += ("<textarea name=\"SENoScript_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" id=\"SENoScript" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" class=\"multiLong\">" + XmlCommon.GetLocaleEntry(DB.RowField(row, "SENoScript"), ThisCustomer.ThisCustomerSession.Session("entityUserLocale"), false) + "</textarea><br/>");
                    //ltBody.Text += ("<b>Search Engine AltText:</b><br/>");
                    //ltBody.Text += ("<input maxLength=\"50\" name=\"SEAltText_" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" id=\"SEAltText" + ThisProductID.ToString() + "_" + ThisVariantID.ToString() + "\" value=\"" + XmlCommon.GetLocaleEntry(DB.RowField(row, "SEAltText"), ThisCustomer.ThisCustomerSession.Session("entityUserLocale"), false) + "\" class=\"singleLongest\" /><br/>");
                    ltBody.Text += ("</div>");
                    ltBody.Text += ("</td>\n");
                    ltBody.Text += ("</tr>\n");
                    LastProductID = ThisProductID;
                }
                ltBody.Text += ("<tr><td colspan=\"5\" align=\"right\"><input type=\"submit\" value=\"Search Engine Update\" name=\"Submit\" class=\"normalButton\"></td></tr>\n");                                
                ltBody.Text += ("</table>\n");
                //ltBody.Text += ("<p align=\"right\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></p>\n");
                ltBody.Text += ("</form>\n");
            }
            else
            {
                ltBody.Text += ("<p><b>No Products Found</b></p>");
            }
            dsProducts.Dispose();
            products.Dispose();
        }
    }
}
