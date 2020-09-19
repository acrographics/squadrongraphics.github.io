// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/entityBulkDisplayOrder.aspx.cs 5     10/04/06 12:00p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Data;
using System.Text;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for displayorder.
    /// </summary>
    public partial class entityBulkDisplayOrder : System.Web.UI.Page
    {
        private Customer ThisCustomer;
        private string eName;
        private int eID;
        private EntitySpecs eSpecs;
        private XmlDocument EntityXml;
        private string EntityPlural;

        protected void Page_Load(object sender, EventArgs e)
        {
            ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            eID = CommonLogic.QueryStringNativeInt("EntityID");
            eName = CommonLogic.QueryStringCanBeDangerousContent("EntityName");
            eSpecs = EntityDefinitions.LookupSpecs(eName);

            switch (eName.ToUpperInvariant())
            {
                case "SECTION":
                    ViewState["entityname"] = "Section";
                    EntityPlural = "Sections";
                    break;
                case "MANUFACTURER":
                    ViewState["entityname"] = "Manufacturer";
                    EntityPlural = "Manufacturers";
                    break;
                case "DISTRIBUTOR":
                    ViewState["entityname"] = "Distributor";
                    EntityPlural = "Distributors";
                    break;
                case "GENRE":
                    ViewState["entityname"] = "Genre";
                    EntityPlural = "Genres";
                    break;
                case "VECTOR":
                    ViewState["entityname"] = "Vector";
                    EntityPlural = "Vectors";
                    break;
                case "LIBRARY":
                    ViewState["entityname"] = "Library";
                    EntityPlural = "Libraries";
                    break;
                default:
                    ViewState["entityname"] = "Category";
                    EntityPlural = "Categories";
                    break;
            }

            if (eID == 0)
            {
                lblpagehdr.Text = "Set " + ViewState["entityname"].ToString() + " Display Order";
                lblpagehdr.Visible = true;
            }
            else
            {
                lblpagehdr.Visible = false;
            }

            if (!IsPostBack)
            {
                EntityXml = new EntityHelper(0, eSpecs).m_TblMgr.XmlDoc;
                LoadBody();
            }
        }

        private void LoadBody()
        {

            XmlNodeList nodelist = EntityXml.SelectNodes("//Entity[ParentEntityID=" + eID.ToString() + "]");
            subcategories.DataSource = nodelist;
            subcategories.DataBind();

            if (nodelist.Count > 0)
            {
                pnlNoSubEntities.Visible = false;
                pnlSubEntityList.Visible = true;
            }
            else
            {
                lblError.Text = "This " + ViewState["entityname"].ToString() + " has no sub-" + EntityPlural;
            }

        }


        protected void UpdateDisplayOrder(object sender, EventArgs e)
        {
            foreach (RepeaterItem ri in subcategories.Items)
            {
                TextBox d = (TextBox)ri.FindControl("DisplayOrder");
                TextBox eid = (TextBox)ri.FindControl("entityid");

                string displayorder = CommonLogic.IIF(CommonLogic.IsInteger(d.Text), d.Text, "1");
                DB.ExecuteSQL("update " + ViewState["entityname"].ToString() + " set displayorder = " + displayorder + " where " + ViewState["entityname"].ToString() + "ID = " + eid.Text);
            }

            //refresh the static entityhelper
            switch (ViewState["entityname"].ToString().ToUpperInvariant())
            {
                case "CATEGORY":
                    AppLogic.CategoryEntityHelper = new EntityHelper(0, EntityDefinitions.LookupSpecs("Category"), true);
                    EntityXml = new EntityHelper(0, EntityDefinitions.LookupSpecs("Category"), false).m_TblMgr.XmlDoc;

                    break;
                case "SECTION":
                    AppLogic.SectionEntityHelper = new EntityHelper(0, EntityDefinitions.LookupSpecs("Section"), true);
                    EntityXml = new EntityHelper(0, EntityDefinitions.LookupSpecs("Section"), false).m_TblMgr.XmlDoc;
                    break;
                case "MANUFACTURER":
                    AppLogic.ManufacturerEntityHelper = new EntityHelper(0, EntityDefinitions.LookupSpecs("Manufacturer"), true);
                    EntityXml = new EntityHelper(0, EntityDefinitions.LookupSpecs("Manufacturer"), false).m_TblMgr.XmlDoc;
                    break;
                case "DISTRIBUTOR":
                    AppLogic.DistributorEntityHelper = new EntityHelper(0, EntityDefinitions.LookupSpecs("Distributor"), true);
                    EntityXml = new EntityHelper(0, EntityDefinitions.LookupSpecs("Distributor"), false).m_TblMgr.XmlDoc;
                    break;
                case "GENRE":
                    AppLogic.GenreEntityHelper = new EntityHelper(0, EntityDefinitions.LookupSpecs("Genre"), true);
                    EntityXml = new EntityHelper(0, EntityDefinitions.LookupSpecs("Genre"), false).m_TblMgr.XmlDoc;
                    break;
                case "VECTOR":
                    AppLogic.VectorEntityHelper = new EntityHelper(0, EntityDefinitions.LookupSpecs("Vector"), true);
                    EntityXml = new EntityHelper(0, EntityDefinitions.LookupSpecs("Genre"), false).m_TblMgr.XmlDoc;
                    break;
                case "LIBRARY":
                    AppLogic.LibraryEntityHelper = new EntityHelper(0, EntityDefinitions.LookupSpecs("Library"), true);
                    EntityXml = new EntityHelper(0, EntityDefinitions.LookupSpecs("Library"), false).m_TblMgr.XmlDoc;
                    break;
            }

            LoadBody();
        }

        public string getLocaleValue(XmlNode n, string locale)
        {
            XmlNode xn = n.SelectSingleNode(".//locale[@name='" + locale + "']");
            if (xn != null)
            {
                return xn.InnerText;
            }
            else
            {
                return n.InnerText;
            }
        }
    }
}
