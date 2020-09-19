using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using AspDotNetStorefrontCommon;

public partial class entityFrame : System.Web.UI.Page
{
    public string GetFrameSize()
    {
        int width = AppLogic.AppConfigNativeInt("Admin.EntityFrameMenuWidth");
        if (width == 0)
        {
            width = 345;
        }

        return width.ToString();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string loadEntity = CommonLogic.QueryStringCanBeDangerousContent("entityname");

        int sectionID = CommonLogic.QueryStringNativeInt("sectionID");
        int categoryID = CommonLogic.QueryStringNativeInt("categoryID");
        int manuID = CommonLogic.QueryStringNativeInt("manufacturerID");
        int distID = CommonLogic.QueryStringNativeInt("distributorID");
        int genreID = CommonLogic.QueryStringNativeInt("genreID");
        int vectorID = CommonLogic.QueryStringNativeInt("VectorID");
        int libraryID = CommonLogic.QueryStringNativeInt("libraryID");

        int productID = CommonLogic.QueryStringNativeInt("productID");
               
        if (loadEntity.Length == 0)
        {
            if (sectionID > 0)
            {
                //menu: entityID, entityName
                entityMenu.Attributes.Add("src", "entityMenu.aspx?entityID=" + sectionID + "&entityName=Section");

                if (productID == 0)
                {
                    //entity: entityName, entityID
                    entityBody.Attributes.Add("src", "entityEdit.aspx?entityID=" + sectionID + "&entityName=Section");
                }
                else
                {
                    entityBody.Attributes.Add("src", "entityEditProducts.aspx?EntityFilterID=" + sectionID + "&EntityName=Section&iden=" + productID);
                }
            }
            else if (categoryID > 0)
            {
                //menu: entityID, entityName
                entityMenu.Attributes.Add("src", "entityMenu.aspx?entityID=" + categoryID + "&entityName=Category");

                if (productID == 0)
                {
                    //entity: entityName, entityID
                    entityBody.Attributes.Add("src", "entityEdit.aspx?entityID=" + categoryID + "&entityName=Category");
                }
                else
                {
                    entityBody.Attributes.Add("src", "entityEditProducts.aspx?EntityFilterID=" + categoryID + "&EntityName=Category&iden=" + productID);
                }
            }
            else if (manuID > 0)
            {
                //menu: entityID, entityName
                entityMenu.Attributes.Add("src", "entityMenu.aspx?entityID=" + manuID + "&entityName=Manufacturer");

                if (productID == 0)
                {
                    //entity: entityName, entityID
                    entityBody.Attributes.Add("src", "entityEdit.aspx?entityID=" + manuID + "&entityName=Manufacturer");
                }
                else
                {
                    entityBody.Attributes.Add("src", "entityEditProducts.aspx?EntityFilterID=" + manuID + "&EntityName=Manufacturer&iden=" + productID);
                }
            }
            else if (distID > 0)
            {
                //menu: entityID, entityName
                entityMenu.Attributes.Add("src", "entityMenu.aspx?entityID=" + distID + "&entityName=Distributor");

                if (productID == 0)
                {
                    //entity: entityName, entityID
                    entityBody.Attributes.Add("src", "entityEdit.aspx?entityID=" + distID + "&entityName=Distributor");
                }
                else
                {
                    entityBody.Attributes.Add("src", "entityEditProducts.aspx?EntityFilterID=" + distID + "&EntityName=Distributor&iden=" + productID);
                }
            }
            else if (genreID > 0)
            {
                //menu: entityID, entityName
                entityMenu.Attributes.Add("src", "entityMenu.aspx?entityID=" + genreID + "&entityName=Genre");

                if (productID == 0)
                {
                    //entity: entityName, entityID
                    entityBody.Attributes.Add("src", "entityEdit.aspx?entityID=" + genreID + "&entityName=Genre");
                }
                else
                {
                    entityBody.Attributes.Add("src", "entityEditProducts.aspx?EntityFilterID=" + genreID + "&EntityName=Genre&iden=" + productID);
                }
            }
            else if (vectorID > 0)
            {
                //menu: entityID, entityName
                entityMenu.Attributes.Add("src", "entityMenu.aspx?entityID=" + vectorID + "&entityName=Vector");

                if (productID == 0)
                {
                    //entity: entityName, entityID
                    entityBody.Attributes.Add("src", "entityEdit.aspx?entityID=" + vectorID + "&entityName=Vector");
                }
                else
                {
                    entityBody.Attributes.Add("src", "entityEditProducts.aspx?EntityFilterID=" + vectorID + "&EntityName=Vector&iden=" + productID);
                }
            }
            else if (libraryID > 0)
            {
                //menu: entityID, entityName
                entityMenu.Attributes.Add("src", "entityMenu.aspx?entityID=" + libraryID + "&entityName=Library");

                if (productID == 0)
                {
                    //entity: entityName, entityID
                    entityBody.Attributes.Add("src", "entityEdit.aspx?entityID=" + libraryID + "&entityName=Library");
                }
                else
                {
                    entityBody.Attributes.Add("src", "entityEditProducts.aspx?EntityFilterID=" + libraryID + "&EntityName=Library&iden=" + productID);
                }
            }
            else if (productID > 0)
            {
                //get first map entity
                //order: category, section, manufacturer, distributor, genre, Vector
                int eID = 0;
                string eName = "";

                if (eID == 0)
                {
                    eID = DB.GetSqlN("SELECT TOP 1 CategoryID AS N FROM ProductCategory WHERE ProductID=" + productID + " ORDER BY DisplayOrder");
                    if (eID > 0)
                    {
                        eName = "Category";
                    }
                }

                if (eID == 0)
                {
                    eID = DB.GetSqlN("SELECT TOP 1 SectionID AS N FROM ProductSection WHERE ProductID=" + productID + " ORDER BY DisplayOrder");
                    if (eID > 0)
                    {
                        eName = "Section";
                    }
                }

                if (eID == 0)
                {
                    eID = DB.GetSqlN("SELECT TOP 1 ManufacturerID AS N FROM ProductManufacturer WHERE ProductID=" + productID + " ORDER BY DisplayOrder");
                    if (eID > 0)
                    {
                        eName = "Manufacturer";
                    }
                }

                if (eID == 0)
                {
                    eID = DB.GetSqlN("SELECT TOP 1 DistributorID AS N FROM ProductDistributor WHERE ProductID=" + productID + " ORDER BY DisplayOrder");
                    if (eID > 0)
                    {
                        eName = "Distributor";
                    }
                }

                if (eID == 0)
                {
                    eID = DB.GetSqlN("SELECT TOP 1 GenreID AS N FROM ProductGenre WHERE ProductID=" + productID + " ORDER BY DisplayOrder");
                    if (eID > 0)
                    {
                        eName = "Genre";
                    }
                }

                if (eID == 0)
                {
                    eID = DB.GetSqlN("SELECT TOP 1 VectorID AS N FROM ProductVector WHERE ProductID=" + productID + " ORDER BY DisplayOrder");
                    if (eID > 0)
                    {
                        eName = "Vector";
                    }
                }

                //menu: entityID, entityName
                entityMenu.Attributes.Add("src", "entityMenu.aspx?entityID=" + eID + "&entityName=" + eName);
                
                entityBody.Attributes.Add("src", "entityEditProducts.aspx?EntityFilterID=" + eID + "&EntityName=" + eName + "&iden=" + productID);
            }
            //otherwise
            else
            {
                //menu: entityID, entityName
                entityMenu.Attributes.Add("src", "entityMenu.aspx");

                //entity: entityName, entityID
                entityBody.Attributes.Add("src", "entityBody.aspx");
            }
        }
        else
        {
            //menu: entityID, entityName
            entityMenu.Attributes.Add("src", "entityMenu.aspx?entityName=" + loadEntity);

            //entity: entityName, entityID
            entityBody.Attributes.Add("src", "entityBody.aspx");
        }
    }
}
