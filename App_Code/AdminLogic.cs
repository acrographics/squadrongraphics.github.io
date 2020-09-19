// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/App_Code/AdminLogic.cs 11    10/04/06 6:22a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Web.Security;
using System.Configuration;
using System.Web.SessionState;
using System.Web.Caching;
using System.Net.Mail;
using System.Web.Util;
using System.Data;
using System.Security.Principal;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;
using System.Xml;
using System.Drawing;
using System.Xml.Serialization;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Threading;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    public enum ImportOption
    {
        Default = 0,
        LeaveModified = 1,
        OverWrite = 2
    }
    /// <summary>
    /// Summary description for AdminLogic.
    /// </summary>
    public class AdminLogic
    {

        public AdminLogic()
        { }

        static public String EntityEditPageFormHandler(AspDotNetStorefront.SkinBase sb, EntitySpecs specs, Customer ThisCustomer, int SkinID)
        {
            int EntityID = 0;
            bool Editing = false;
            String ErrorMsg = String.Empty;
            if (CommonLogic.QueryStringCanBeDangerousContent("EntityID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("EntityID") != "0")
            {
                Editing = true;
                EntityID = CommonLogic.QueryStringUSInt("EntityID");
            }

            IDataReader rs;

            if (CommonLogic.FormBool("IsSubmit"))
            {
                if (ErrorMsg.Length == 0)
                {
                    //try
                    //{
                    StringBuilder sql = new StringBuilder(2500);
                    int ParID = CommonLogic.FormUSInt("Parent" + specs.m_EntityName + "ID");
                    if (ParID == EntityID)  // prevent (stupid case which causes endless recursion)
                    {
                        ParID = 0;
                    }
                    if (!Editing)
                    {
                        // ok to add them:
                        String NewGUID = DB.GetNewGUID();
                        sql.Append("insert into " + specs.m_EntityName + "(" + specs.m_EntityName + "GUID,Name,SEName," + CommonLogic.IIF(specs.m_HasAddress, "Address1,Address2,Suite,City,State,ZipCode,Country,Phone,FAX,URL,EMail,", "") + "ContentsBGColor,PageBGColor,GraphicsColor,ImageFilenameOverride," + CommonLogic.IIF(specs.m_HasParentChildRelationship, "Parent" + specs.m_EntityName + "ID,", "") + "Summary,Description,ExtensionData,SEKeywords,SEDescription,SETitle,SENoScript,Published," + CommonLogic.IIF(specs.m_EntityName == "Category" || specs.m_EntityName == "Section", "ShowIn" + specs.m_ObjectName + "Browser,", "") + "PageSize,ColWidth,XmlPackage,SortByLooks,QuantityDiscountID) values(");
                        sql.Append(DB.SQuote(NewGUID) + ",");
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                        if (AppLogic.NumLocaleSettingsInstalled() > 1)
                        {
                            sql.Append(DB.SQuote(SE.MungeName(CommonLogic.FormCanBeDangerousContent("Name_" + Localization.GetWebConfigLocale().Replace("-", "_")))) + ",");
                        }
                        else
                        {
                            sql.Append(DB.SQuote(SE.MungeName(CommonLogic.FormCanBeDangerousContent("Name"))) + ",");
                        }
                        if (specs.m_HasAddress)
                        {
                            if (CommonLogic.FormCanBeDangerousContent("Address1").Length != 0)
                            {
                                sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("Address1")) + ",");
                            }
                            else
                            {
                                sql.Append("NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("Address2").Length != 0)
                            {
                                sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("Address2")) + ",");
                            }
                            else
                            {
                                sql.Append("NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("Suite").Length != 0)
                            {
                                sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("Suite")) + ",");
                            }
                            else
                            {
                                sql.Append("NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("City").Length != 0)
                            {
                                sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("City")) + ",");
                            }
                            else
                            {
                                sql.Append("NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("State").Length != 0)
                            {
                                sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("State")) + ",");
                            }
                            else
                            {
                                sql.Append("NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("ZipCode").Length != 0)
                            {
                                sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("ZipCode")) + ",");
                            }
                            else
                            {
                                sql.Append("NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("Country").Length != 0)
                            {
                                sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("Country")) + ",");
                            }
                            else
                            {
                                sql.Append("NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("Phone").Length != 0)
                            {
                                sql.Append(DB.SQuote(AppLogic.MakeProperPhoneFormat(CommonLogic.FormCanBeDangerousContent("Phone"))) + ",");
                            }
                            else
                            {
                                sql.Append("NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("FAX").Length != 0)
                            {
                                sql.Append(DB.SQuote(AppLogic.MakeProperPhoneFormat(CommonLogic.FormCanBeDangerousContent("FAX"))) + ",");
                            }
                            else
                            {
                                sql.Append("NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("URL").Length != 0)
                            {
                                String theUrl = CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("URL"), 80);
                                if (theUrl.IndexOf("http://") == -1 && theUrl.Length != 0)
                                {
                                    theUrl = "http://" + theUrl;
                                }
                                if (theUrl.Length == 0)
                                {
                                    sql.Append("NULL,");
                                }
                                else
                                {
                                    sql.Append(DB.SQuote(theUrl) + ",");
                                }
                            }
                            else
                            {
                                sql.Append("NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("EMail").Length != 0)
                            {
                                sql.Append(DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("EMail"), 100)) + ",");
                            }
                            else
                            {
                                sql.Append("NULL,");
                            }

                        }
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("ContentsBGColor")) + ",");
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("PageBGColor")) + ",");
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("GraphicsColor")) + ",");
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("ImageFilenameOverride")) + ",");
                        if (specs.m_HasParentChildRelationship)
                        {
                            sql.Append(ParID.ToString() + ",");
                        }
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Summary")) + ",");
                        sql.Append(DB.SQuote(AppLogic.FormLocaleXml("Description")) + ",");
                        sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("ExtensionData")) + ",");
                        if (AppLogic.FormLocaleXml("SEKeywords").Length != 0)
                        {
                            sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SEKeywords")) + ",");
                        }
                        else
                        {
                            sql.Append("NULL,");
                        }
                        if (AppLogic.FormLocaleXml("SEDescription").Length != 0)
                        {
                            sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SEDescription")) + ",");
                        }
                        else
                        {
                            sql.Append("NULL,");
                        }
                        if (AppLogic.FormLocaleXml("SETitle").Length != 0)
                        {
                            sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SETitle")) + ",");
                        }
                        else
                        {
                            sql.Append("NULL,");
                        }
                        if (AppLogic.FormLocaleXml("SENoScript").Length != 0)
                        {
                            sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SENoScript")) + ",");
                        }
                        else
                        {
                            sql.Append("NULL,");
                        }
                        //if (AppLogic.FormLocaleXml("SEAltText").Length != 0)
                        //{
                        //    sql.Append(DB.SQuote(AppLogic.FormLocaleXml("SEAltText")) + ",");
                        //}
                        //else
                        //{
                        //    sql.Append("NULL,");
                        //}
                        sql.Append(CommonLogic.FormUSInt("Published") + ",");
                        if (specs.m_EntityName == "Category" || specs.m_EntityName == "Section")
                        {
                            sql.Append(CommonLogic.FormCanBeDangerousContent("ShowIn" + specs.m_ObjectName + "Browser") + ",");
                        }
                        sql.Append(CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("PageSize").Length == 0, AppLogic.AppConfig("Default_" + specs.m_EntityName + "PageSize"), CommonLogic.FormCanBeDangerousContent("PageSize")) + ",");
                        sql.Append(CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("ColWidth").Length == 0, AppLogic.AppConfig("Default_" + specs.m_EntityName + "ColWidth"), CommonLogic.FormCanBeDangerousContent("ColWidth")) + ",");
                        if (CommonLogic.FormCanBeDangerousContent("XmlPackage").Length != 0)
                        {
                            sql.Append(DB.SQuote(CommonLogic.FormCanBeDangerousContent("XmlPackage").ToLowerInvariant()) + ",");
                        }
                        else
                        {
                            sql.Append(DB.SQuote(AppLogic.ro_DefaultEntityXmlPackage) + ","); // force a default!
                        }
                        sql.Append(CommonLogic.FormCanBeDangerousContent("SortByLooks") + ",");
                        sql.Append(CommonLogic.FormCanBeDangerousContent("QuantityDiscountID"));
                        sql.Append(")");

                        DB.ExecuteSQL(sql.ToString());

                        rs = DB.GetRS("select " + specs.m_EntityName + "ID from " + specs.m_EntityName + "  " + DB.GetNoLock() + " where Deleted=0 and " + specs.m_EntityName + "GUID=" + DB.SQuote(NewGUID));
                        rs.Read();
                        EntityID = DB.RSFieldInt(rs, specs.m_EntityName + "ID");
                        Editing = true;
                        // next line forces entity context to be reset to the newly added entity, this is not the most elegant solution, but it works fine.
                        HttpContext.Current.Response.Redirect("editentity.aspx?entityname=" + specs.m_EntityName + "&entityid=" + EntityID.ToString());
                        rs.Close();
                    }
                    else
                    {
                        // ok to update:
                        sql.Append("update " + specs.m_EntityName + " set ");
                        sql.Append("Name=" + DB.SQuote(AppLogic.FormLocaleXml("Name")) + ",");
                        sql.Append("SEName=" + DB.SQuote(SE.MungeName(CommonLogic.FormCanBeDangerousContent("Name_" + Localization.GetWebConfigLocale().Replace("-", "_")))) + ",");

                        if (specs.m_HasAddress)
                        {
                            if (CommonLogic.FormCanBeDangerousContent("Address1").Length != 0)
                            {
                                sql.Append("Address1=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Address1")) + ",");
                            }
                            else
                            {
                                sql.Append("Address1=NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("Address2").Length != 0)
                            {
                                sql.Append("Address2=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Address2")) + ",");
                            }
                            else
                            {
                                sql.Append("Address2=NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("Suite").Length != 0)
                            {
                                sql.Append("Suite=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Suite")) + ",");
                            }
                            else
                            {
                                sql.Append("Suite=NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("City").Length != 0)
                            {
                                sql.Append("City=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("City")) + ",");
                            }
                            else
                            {
                                sql.Append("City=NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("State").Length != 0)
                            {
                                sql.Append("State=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("State")) + ",");
                            }
                            else
                            {
                                sql.Append("State=NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("ZipCode").Length != 0)
                            {
                                sql.Append("ZipCode=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ZipCode")) + ",");
                            }
                            else
                            {
                                sql.Append("ZipCode=NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("Country").Length != 0)
                            {
                                sql.Append("Country=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("Country")) + ",");
                            }
                            else
                            {
                                sql.Append("Country=NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("Phone").Length != 0)
                            {
                                sql.Append("Phone=" + DB.SQuote(AppLogic.MakeProperPhoneFormat(CommonLogic.FormCanBeDangerousContent("Phone"))) + ",");
                            }
                            else
                            {
                                sql.Append("Phone=NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("FAX").Length != 0)
                            {
                                sql.Append("FAX=" + DB.SQuote(AppLogic.MakeProperPhoneFormat(CommonLogic.FormCanBeDangerousContent("FAX"))) + ",");
                            }
                            else
                            {
                                sql.Append("FAX=NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("URL").Length != 0)
                            {
                                String theUrl2 = CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("URL"), 80);
                                if (theUrl2.IndexOf("http://") == -1 && theUrl2.Length != 0)
                                {
                                    theUrl2 = "http://" + theUrl2;
                                }
                                if (theUrl2.Length != 0)
                                {
                                    sql.Append("URL=" + DB.SQuote(theUrl2) + ",");
                                }
                                else
                                {
                                    sql.Append("URL=NULL,");
                                }
                            }
                            else
                            {
                                sql.Append("URL=NULL,");
                            }
                            if (CommonLogic.FormCanBeDangerousContent("EMail").Length != 0)
                            {
                                sql.Append("EMail=" + DB.SQuote(CommonLogic.Left(CommonLogic.FormCanBeDangerousContent("EMail"), 100)) + ",");
                            }
                            else
                            {
                                sql.Append("EMail=NULL,");
                            }
                        }

                        sql.Append("ContentsBGColor=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ContentsBGColor")) + ",");
                        sql.Append("PageBGColor=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("PageBGColor")) + ",");
                        sql.Append("GraphicsColor=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("GraphicsColor")) + ",");
                        sql.Append("ImageFilenameOverride=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ImageFilenameOverride")) + ",");
                        if (specs.m_HasParentChildRelationship)
                        {
                            sql.Append("Parent" + specs.m_EntityName + "ID=" + ParID.ToString() + ",");
                        }
                        sql.Append("Summary=" + DB.SQuote(AppLogic.FormLocaleXml("Summary")) + ",");
                        sql.Append("Description=" + DB.SQuote(AppLogic.FormLocaleXml("Description")) + ",");
                        sql.Append("ExtensionData=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("ExtensionData")) + ",");
                        if (AppLogic.FormLocaleXml("SEKeywords").Length != 0)
                        {
                            sql.Append("SEKeywords=" + DB.SQuote(AppLogic.FormLocaleXml("SEKeywords")) + ",");
                        }
                        else
                        {
                            sql.Append("SEKeywords=NULL,");
                        }
                        if (AppLogic.FormLocaleXml("SEDescription").Length != 0)
                        {
                            sql.Append("SEDescription=" + DB.SQuote(AppLogic.FormLocaleXml("SEDescription")) + ",");
                        }
                        else
                        {
                            sql.Append("SEDescription=NULL,");
                        }
                        if (AppLogic.FormLocaleXml("SETitle").Length != 0)
                        {
                            sql.Append("SETitle=" + DB.SQuote(AppLogic.FormLocaleXml("SETitle")) + ",");
                        }
                        else
                        {
                            sql.Append("SETitle=NULL,");
                        }
                        if (AppLogic.FormLocaleXml("SENoScript").Length != 0)
                        {
                            sql.Append("SENoScript=" + DB.SQuote(AppLogic.FormLocaleXml("SENoScript")) + ",");
                        }
                        else
                        {
                            sql.Append("SENoScript=NULL,");
                        }
                        //if (AppLogic.FormLocaleXml("SEAltText").Length != 0)
                        //{
                        //    sql.Append("SEAltText=" + DB.SQuote(AppLogic.FormLocaleXml("SEAltText")) + ",");
                        //}
                        //else
                        //{
                        //    sql.Append("SEAltText=NULL,");
                        //}
                        sql.Append("Published=" + CommonLogic.FormUSInt("Published") + ",");
                        if (specs.m_EntityName == "Category" || specs.m_EntityName == "Section")
                        {
                            sql.Append("ShowIn" + specs.m_ObjectName + "Browser=" + CommonLogic.FormCanBeDangerousContent("ShowIn" + specs.m_ObjectName + "Browser") + ",");
                        }
                        sql.Append("PageSize=" + CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("PageSize").Length == 0, AppLogic.AppConfig("Default_" + specs.m_EntityName + "PageSize"), CommonLogic.FormCanBeDangerousContent("PageSize")) + ",");
                        sql.Append("ColWidth=" + CommonLogic.IIF(CommonLogic.FormCanBeDangerousContent("ColWidth").Length == 0, AppLogic.AppConfig("Default_" + specs.m_EntityName + "ColWidth"), CommonLogic.FormCanBeDangerousContent("ColWidth")) + ",");
                        if (CommonLogic.FormCanBeDangerousContent("XmlPackage").Length != 0)
                        {
                            sql.Append("XmlPackage=" + DB.SQuote(CommonLogic.FormCanBeDangerousContent("XmlPackage").ToLowerInvariant()) + ",");
                        }
                        else
                        {
                            sql.Append("XmlPackage=NULL,");
                        }
                        sql.Append("SortByLooks=" + CommonLogic.FormCanBeDangerousContent("SortByLooks") + ",");
                        sql.Append("QuantityDiscountID=" + CommonLogic.FormCanBeDangerousContent("QuantityDiscountID"));
                        sql.Append(" where " + specs.m_EntityName + "ID=" + EntityID.ToString());
                        //try
                        //{
                        DB.ExecuteSQL(sql.ToString());
                        //}
                        //catch(Exception ex)
                        //{
                        //	throw new ArgumentException("Error in AdminLogic(.RunSql), Msg=[" + CommonLogic.GetExceptionDetail(ex,String.Empty) + "], Sql=[" + sql.ToString() + "]");
                        //}
                        Editing = true;
                    }
                    //}
                    //catch(Exception ex)
                    //{
                    //	ErrorMsg = "<p><b>ERROR: " + CommonLogic.GetExceptionDetail(ex,"<br/>") + "<br/><br/></b></p>";
                    //}

                    if (ErrorMsg.Length != 0)
                    {
                        ErrorMsg += "<br/>";
                    }
                    ErrorMsg += HandleImageSubmits(specs, EntityID);
                }

            }
            return ErrorMsg;
        }

        static public String HandleImageSubmits(EntitySpecs specs, int EntityID)
        {
            // handle image uploaded:
            String FN = String.Empty;
            if (AppLogic.AppConfigBool("UseSKUFor" + specs.m_ObjectName + "ImageName"))
            {
                FN = CommonLogic.FormCanBeDangerousContent("SKU").Trim();
            }
            if (CommonLogic.FormCanBeDangerousContent("ImageFilenameOverride").Trim().Length != 0)
            {
                FN = CommonLogic.FormCanBeDangerousContent("ImageFilenameOverride").Trim();
            }
            if (FN.Length == 0)
            {
                FN = EntityID.ToString();
            }
            String ErrorMsg = String.Empty;
            //try
            //{
            if (specs.m_HasIconPic)
            {
                String Image1 = String.Empty;
                HttpPostedFile Image1File = HttpContext.Current.Request.Files["Image1"];
                if (Image1File != null && Image1File.ContentLength != 0)
                {
                    // delete any current image file first
                    try
                    {
                        foreach (String ss in CommonLogic.SupportedImageTypes)
                        {
                            if (FN.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) || FN.EndsWith(".gif", StringComparison.InvariantCultureIgnoreCase) || FN.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
                            {
                                System.IO.File.Delete(AppLogic.GetImagePath(specs.m_ObjectName, "icon", true) + FN);
                            }
                            else
                            {
                                System.IO.File.Delete(AppLogic.GetImagePath(specs.m_EntityName, "icon", true) + FN + ss);
                            }
                        }
                    }
                    catch
                    { }

                    String s = Image1File.ContentType;
                    switch (Image1File.ContentType)
                    {
                        case "image/gif":
                            Image1 = AppLogic.GetImagePath(specs.m_EntityName, "icon", true) + FN + ".gif";
                            Image1File.SaveAs(Image1);
                            break;
                        case "image/x-png":
                            Image1 = AppLogic.GetImagePath(specs.m_EntityName, "icon", true) + FN + ".png";
                            Image1File.SaveAs(Image1);
                            break;
                        case "image/jpg":
                        case "image/jpeg":
                        case "image/pjpeg":
                            Image1 = AppLogic.GetImagePath(specs.m_EntityName, "icon", true) + FN + ".jpg";
                            Image1File.SaveAs(Image1);
                            break;
                    }
                }
            }

            if (specs.m_HasMediumPic)
            {
                String Image2 = String.Empty;
                HttpPostedFile Image2File = HttpContext.Current.Request.Files["Image2"];
                if (Image2File != null && Image2File.ContentLength != 0)
                {
                    // delete any current image file first
                    try
                    {
                        foreach (String ss in CommonLogic.SupportedImageTypes)
                        {
                            System.IO.File.Delete(AppLogic.GetImagePath(specs.m_EntityName, "medium", true) + FN + ss);
                        }
                    }
                    catch
                    { }

                    String s = Image2File.ContentType;
                    switch (Image2File.ContentType)
                    {
                        case "image/gif":
                            Image2 = AppLogic.GetImagePath(specs.m_EntityName, "medium", true) + FN + ".gif";
                            Image2File.SaveAs(Image2);
                            break;
                        case "image/x-png":
                            Image2 = AppLogic.GetImagePath(specs.m_EntityName, "medium", true) + FN + ".png";
                            Image2File.SaveAs(Image2);
                            break;
                        case "image/jpg":
                        case "image/jpeg":
                        case "image/pjpeg":
                            Image2 = AppLogic.GetImagePath(specs.m_EntityName, "medium", true) + FN + ".jpg";
                            Image2File.SaveAs(Image2);
                            break;
                    }
                }
            }

            if (specs.m_HasLargePic)
            {
                String Image3 = String.Empty;
                HttpPostedFile Image3File = HttpContext.Current.Request.Files["Image3"];
                if (Image3File != null && Image3File.ContentLength != 0)
                {
                    // delete any current image file first
                    try
                    {
                        foreach (String ss in CommonLogic.SupportedImageTypes)
                        {
                            System.IO.File.Delete(AppLogic.GetImagePath(specs.m_EntityName, "large", true) + FN + ss);
                        }
                    }
                    catch
                    { }

                    String s = Image3File.ContentType;
                    switch (Image3File.ContentType)
                    {
                        case "image/gif":
                            Image3 = AppLogic.GetImagePath(specs.m_EntityName, "large", true) + FN + ".gif";
                            Image3File.SaveAs(Image3);
                            break;
                        case "image/x-png":
                            Image3 = AppLogic.GetImagePath(specs.m_EntityName, "large", true) + FN + ".png";
                            Image3File.SaveAs(Image3);
                            break;
                        case "image/jpg":
                        case "image/jpeg":
                        case "image/pjpeg":
                            Image3 = AppLogic.GetImagePath(specs.m_EntityName, "large", true) + FN + ".jpg";
                            Image3File.SaveAs(Image3);
                            break;
                    }
                }
            }

            //}
            //catch(Exception ex)
            //{
            //	ErrorMsg = CommonLogic.GetExceptionDetail(ex,"<br/>");
            //}		
            return ErrorMsg;
        }

        static public String EntityEditPageRender(AspDotNetStorefront.SkinBase sb, EntitySpecs specs, Customer ThisCustomer, int SkinID, String ErrorMsg)
        {
            int EntityID = 0;
            bool Editing = false;
            if (CommonLogic.QueryStringCanBeDangerousContent("EntityID").Length != 0 && CommonLogic.QueryStringCanBeDangerousContent("EntityID") != "0")
            {
                Editing = true;
                EntityID = CommonLogic.QueryStringUSInt("EntityID");
            }

            IDataReader rs = DB.GetRS("select * from " + specs.m_EntityName + " " + DB.GetNoLock() + " where " + specs.m_EntityName + "ID=" + EntityID.ToString());
            if (rs.Read())
            {
                Editing = true;
            }


            StringBuilder tmpS = new StringBuilder(50000);
            tmpS.Append("<form enctype=\"multipart/form-data\" action=\"editentity.aspx?entityname=" + specs.m_EntityName + "&entityID=" + EntityID.ToString() + "&edit=" + Editing.ToString() + "\" method=\"post\" id=\"Form1\" name=\"Form1\" onsubmit=\"return (validateForm(document.forms[0]))\" onReset=\"return confirm('Do you want to reset all fields to their starting values?');\">\n");

            if (ErrorMsg.Length != 0)
            {
                tmpS.Append("<p><b><font color=red>" + ErrorMsg + "</font></b></p>\n");
            }
            if (CommonLogic.FormCanBeDangerousContent("IsSubmit").Length != 0)
            {
                tmpS.Append("<p align=\"left\"><b><font color=blue>(UPDATED)</font></b></p>\n");
            }


            EntityHelper EntityHelper = AppLogic.LookupHelper(sb.EntityHelpers, specs.m_EntityName);

            if (ErrorMsg.Length == 0)
            {

                if (Editing && EntityID != 0)
                {
                    tmpS.Append("<p><b>Editing " + AppLogic.GetString("AppConfig." + specs.m_EntityName + "PromptSingular", SkinID, ThisCustomer.LocaleSetting) + ": " + DB.RSFieldByLocale(rs, "Name", ThisCustomer.LocaleSetting) + " (ID=" + EntityID.ToString() + ")</b>\n");
                    XmlNode n = EntityHelper.m_TblMgr.SetContext(EntityID);
                    int NumSiblings = 0;
                    if (n != null)
                    {
                        NumSiblings = EntityHelper.m_TblMgr.NumSiblings(n);
                    }
                    tmpS.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
                    if (NumSiblings > 1)
                    {
                        int PreviousID = EntityHelper.GetPreviousEntity(EntityID, true);
                        tmpS.Append("<a class=\"" + specs.m_ObjectName + "NavLink\" href=\"editentity.aspx?entityname=" + specs.m_EntityName + "&entityid=" + PreviousID.ToString() + "\">&lt;&lt;</a>&nbsp;|&nbsp;");
                    }
                    tmpS.Append("<a class=\"" + specs.m_ObjectName + "NavLink\" href=\"entities.aspx?entityname=" + specs.m_EntityName + "\">up</a>");
                    if (NumSiblings > 1)
                    {
                        int NextID = EntityHelper.GetNextEntity(EntityID, true);
                        tmpS.Append("&nbsp;|&nbsp;<a class=\"" + specs.m_ObjectName + "NavLink\" href=\"editentity.aspx?entityname=" + specs.m_EntityName + "&entityid=" + NextID.ToString() + "\">&gt;&gt;</a>&nbsp");
                    }
                    tmpS.Append("&nbsp;&nbsp;&nbsp;|&nbsp;<a class=\"" + specs.m_ObjectName + "NavLink\" href=\"" + specs.m_ObjectNamePlural + ".aspx?entityname=" + specs.m_EntityName + "&entityfilterid=" + EntityID.ToString() + "\">Show " + specs.m_ObjectNamePlural + "</a>");
                    tmpS.Append("&nbsp;&nbsp;&nbsp;|&nbsp;<a class=\"" + specs.m_ObjectName + "NavLink\" href=\"displayorder.aspx?entityname=" + specs.m_EntityName + "&EntityID=" + EntityID.ToString() + "\">Set " + specs.m_ObjectName + " Display Orders</a>");
                    int N = EntityHelper.GetNumEntityObjects(EntityID, true, true);
                    int MaxBulkN = AppLogic.AppConfigUSInt("MaxBulkN");
                    if (MaxBulkN == 0)
                    {
                        MaxBulkN = 100; // default it
                    }
                    if (N > 0 && N <= MaxBulkN)
                    {
                        tmpS.Append("<br/><br/>");
                        int ThisID = EntityID;
                        if (specs.m_ObjectName == "Product")
                        {
                            tmpS.Append("Bulk Edit (All " + specs.m_ObjectNamePlural + " In " + specs.m_EntityName + "): ");
                            tmpS.Append("<input type=\"button\" style=\"font-size: 9px;\" value=\"Inventory\" name=\"InventoryEdit_" + ThisID.ToString() + "\" onClick=\"self.location='bulkeditinventory.aspx?entityname=" + specs.m_EntityName + "&EntityID=" + ThisID.ToString() + "'\">&nbsp;");
                            tmpS.Append("<input type=\"button\" style=\"font-size: 9px;\" value=\"SEFields\" name=\"SearchEdit_" + ThisID.ToString() + "\" onClick=\"self.location='bulkeditsearch.aspx?entityname=" + specs.m_EntityName + "&EntityID=" + ThisID.ToString() + "'\">&nbsp;");
                            tmpS.Append("<input type=\"button\" style=\"font-size: 9px;\" value=\"Prices\" name=\"PricesEdit_" + ThisID.ToString() + "\" onClick=\"self.location='bulkeditprices.aspx?entityname=" + specs.m_EntityName + "&EntityID=" + ThisID.ToString() + "'\">&nbsp;");
                            if (Shipping.GetActiveShippingCalculationID() == Shipping.ShippingCalculationEnum.UseIndividualItemShippingCosts)
                            {
                                tmpS.Append("<input type=\"button\" style=\"font-size: 9px;\" value=\"ShipCosts\" name=\"ShippingCostsEdit_" + ThisID.ToString() + "\" onClick=\"self.location='bulkeditshippingcosts.aspx?entityname=" + specs.m_EntityName + "&EntityID=" + ThisID.ToString() + "'\">&nbsp;");
                            }
                            tmpS.Append("<input type=\"button\" style=\"font-size: 9px;\" value=\"DownloadFiles\" name=\"DownloadFilesEdit_" + ThisID.ToString() + "\" onClick=\"self.location='bulkeditdownloadfiles.aspx?entityname=" + specs.m_EntityName + "&EntityID=" + ThisID.ToString() + "'\">");
                        }
                    }
                    tmpS.Append("<br/>");
                    tmpS.Append("</p>\n");
                }
                else
                {
                    tmpS.Append("<b>Adding New " + AppLogic.GetString("AppConfig." + specs.m_EntityName + "PromptSingular", SkinID, ThisCustomer.LocaleSetting) + ":<br/><br/></b>\n");
                }

                tmpS.Append("<script type=\"text/javascript\">\n");
                tmpS.Append("function Form_Validator(theForm)\n");
                tmpS.Append("{\n");
                tmpS.Append("submitonce(theForm);\n");
                tmpS.Append("return (true);\n");
                tmpS.Append("}\n");
                tmpS.Append("</script>\n");

                if (AppLogic.NumLocaleSettingsInstalled() > 1)
                {
                    tmpS.Append(CommonLogic.ReadFile("jscripts/tabs.js", true));
                }

                tmpS.Append("<p>Please enter the following information about this " + AppLogic.GetString("AppConfig." + specs.m_EntityName + "PromptSingular", SkinID, ThisCustomer.LocaleSetting).ToLowerInvariant() + ". Fields marked with an asterisk (*) are required. All other fields are optional.</p>\n");
                tmpS.Append("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\">\n");
                tmpS.Append("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
                tmpS.Append("              <tr valign=\"top\">\n");
                tmpS.Append("                <td width=\"100%\" colspan=\"2\" align=\"left\">\n");
                tmpS.Append("                </td>\n");
                tmpS.Append("              </tr>\n");

                tmpS.Append("<tr>\n");
                tmpS.Append("<td></td><td align=\"left\" valign=\"top\"><br/>\n");
                if (Editing)
                {
                    tmpS.Append("<input type=\"submit\" value=\"Update\" name=\"submit\">\n");
                    tmpS.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"reset\" class=\"CPButton\" value=\"Reset\" name=\"reset\">\n");
                }
                else
                {
                    tmpS.Append("<input type=\"submit\" value=\"Add New\" name=\"submit\">\n");
                }
                tmpS.Append("        </td>\n");
                tmpS.Append("      </tr>\n");

                tmpS.Append("              <tr valign=\"top\">\n");
                tmpS.Append("                <td width=\"25%\" align=\"right\" valign=\"top\">*" + AppLogic.GetString("AppConfig." + specs.m_EntityName + "PromptSingular", SkinID, ThisCustomer.LocaleSetting) + " Name:&nbsp;&nbsp;</td>\n");
                tmpS.Append("                <td align=\"left\" valign=\"top\" valign=\"top\">\n");
                tmpS.Append(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Name"), "Name", false, true, true, "Please enter the " + AppLogic.GetString("AppConfig." + specs.m_EntityName + "PromptSingular", SkinID, ThisCustomer.LocaleSetting).ToLowerInvariant() + " name", 100, 50, 0, 0, false));
                //tmpS.Append("                	<input maxLength=\"100\" size=\"30\" name=\"Name\" value=\"" + CommonLogic.IIF(Editing , HttpContext.Current.Server.HtmlEncode(DB.RSField(rs,"Name")) , "") + "\">\n");
                //tmpS.Append("                	<input type=\"hidden\" name=\"Name_vldt\" value=\"[req][blankalert=Please enter the " + AppLogic.GetString("AppConfig." + specs.m_EntityName + "PromptSingular").ToLowerInvariant() + " name]\">\n");
                tmpS.Append("                	</td>\n");
                tmpS.Append("              </tr>\n");

                tmpS.Append("              <tr valign=\"top\">\n");
                tmpS.Append("                <td align=\"right\" valign=\"top\" bgcolor=\"" + CommonLogic.IIF(Editing && !DB.RSFieldBool(rs, "Published"), "#FFFFCC", "FFFFFF") + "\">*Published:&nbsp;&nbsp;</td>\n");
                tmpS.Append("                <td align=\"left\" bgcolor=\"" + CommonLogic.IIF(Editing && !DB.RSFieldBool(rs, "Published"), "#FFFFCC", "FFFFFF") + "\">\n");
                tmpS.Append("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"Published\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "Published"), " checked ", ""), " checked ") + ">\n");
                tmpS.Append("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"Published\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "Published"), "", " checked "), "") + ">\n");
                tmpS.Append("                </td>\n");
                tmpS.Append("              </tr>\n");

                if (specs.m_EntityName == "Category" || specs.m_EntityName == "Section")
                {
                    tmpS.Append("              <tr valign=\"top\">\n");
                    tmpS.Append("                <td align=\"right\" valign=\"top\">*Show In " + specs.m_ObjectName + " Browser:&nbsp;&nbsp;</td>\n");
                    tmpS.Append("                <td align=\"left\" >\n");
                    tmpS.Append("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"ShowIn" + specs.m_ObjectName + "Browser\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "ShowIn" + specs.m_ObjectName + "Browser"), " checked ", ""), " checked ") + ">\n");
                    tmpS.Append("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"ShowIn" + specs.m_ObjectName + "Browser\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "ShowIn" + specs.m_ObjectName + "Browser"), "", " checked "), "") + ">\n");
                    tmpS.Append("                </td>\n");
                    tmpS.Append("              </tr>\n");
                }
                if (specs.m_HasParentChildRelationship)
                {
                    tmpS.Append("              <tr valign=\"top\">\n");
                    tmpS.Append("                <td align=\"right\" valign=\"top\">Parent " + AppLogic.GetString("AppConfig." + specs.m_EntityName + "PromptSingular", SkinID, ThisCustomer.LocaleSetting) + ":&nbsp;&nbsp;</td>\n");
                    tmpS.Append("                <td align=\"left\" valign=\"top\">\n");

                    tmpS.Append("<select size=\"1\" name=\"Parent" + specs.m_EntityName + "ID\">\n");
                    tmpS.Append(" <OPTION VALUE=\"0\" " + CommonLogic.IIF(!Editing, " selected ", "") + ">--ROOT LEVEL--</option>\n");
                    String EntitySel = EntityHelper.GetEntitySelectList(0, String.Empty, 0, ThisCustomer.LocaleSetting, false);
                    // mark current parent:
                    EntitySel = EntitySel.Replace("<option value=\"" + DB.RSFieldInt(rs, "Parent" + specs.m_EntityName + "ID").ToString() + "\">", "<option value=\"" + DB.RSFieldInt(rs, "Parent" + specs.m_EntityName + "ID").ToString() + "\" selected>");
                    tmpS.Append(EntitySel);
                    tmpS.Append("</select>\n");

                    tmpS.Append("                </td>\n");
                    tmpS.Append("              </tr>\n");
                }

                tmpS.Append("<tr valign=\"top\">\n");
                tmpS.Append("<td align=\"right\" valign=\"top\">*Display Format Xml Package:&nbsp;&nbsp;</td>\n");
                tmpS.Append("<td align=\"left\" valign=\"top\">\n");
                tmpS.Append("<select size=\"1\" name=\"XmlPackage\">\n");
                ArrayList xmlPackages = AppLogic.ReadXmlPackages("entity", 1);
                foreach (String s in xmlPackages)
                {
                    tmpS.Append("<option value=\"" + s + "\"");
                    if (Editing)
                    {
                        if (DB.RSField(rs, "XmlPackage").ToLowerInvariant() == s)
                        {
                            tmpS.Append(" selected");
                        }
                    }
                    tmpS.Append(">" + s + "</option>");
                }
                tmpS.Append("</select>\n");
                tmpS.Append("</td>\n");
                tmpS.Append("</tr>\n");

                tmpS.Append("<tr valign=\"top\">\n");
                tmpS.Append("<td align=\"right\" valign=\"top\">Quantity Discount Table:&nbsp;&nbsp;</td>\n");
                tmpS.Append("<td align=\"left\" valign=\"top\">\n");
                tmpS.Append("<select size=\"1\" name=\"QuantityDiscountID\">\n");
                tmpS.Append("<option value=\"0\">None</option>");
                IDataReader rsst = DB.GetRS("select * from QuantityDiscount  " + DB.GetNoLock() + " order by DisplayOrder,Name");
                while (rsst.Read())
                {
                    tmpS.Append("<option value=\"" + DB.RSFieldInt(rsst, "QuantityDiscountID").ToString() + "\"");
                    if (Editing)
                    {
                        if (DB.RSFieldInt(rs, "QuantityDiscountID") == DB.RSFieldInt(rsst, "QuantityDiscountID"))
                        {
                            tmpS.Append(" selected");
                        }
                    }
                    tmpS.Append(">" + DB.RSFieldByLocale(rsst, "Name", ThisCustomer.LocaleSetting) + "</option>");
                }
                rsst.Close();
                tmpS.Append("</select>\n");
                tmpS.Append("</td>\n");
                tmpS.Append("</tr>\n");

                tmpS.Append("              <tr valign=\"top\">\n");
                tmpS.Append("                <td align=\"right\" valign=\"top\">Page Size:&nbsp;&nbsp;</td>\n");
                tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                tmpS.Append("                	<input maxLength=\"2\" size=\"2\" name=\"PageSize\" value=\"" + CommonLogic.IIF(Editing, DB.RSFieldInt(rs, "PageSize").ToString(), AppLogic.AppConfig("Default_" + specs.m_EntityName + "PageSize")) + "\"> (may be used by the XmlPackage displaying this page)\n");
                tmpS.Append("                </td>\n");
                tmpS.Append("              </tr>\n");

                tmpS.Append("              <tr valign=\"top\">\n");
                tmpS.Append("                <td align=\"right\" valign=\"top\">Column Width:&nbsp;&nbsp;</td>\n");
                tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                tmpS.Append("                	<input maxLength=\"2\" size=\"2\" name=\"ColWidth\" value=\"" + CommonLogic.IIF(Editing, DB.RSFieldInt(rs, "ColWidth").ToString(), AppLogic.AppConfig("Default_" + specs.m_EntityName + "ColWidth")) + "\"> (may be used by the XmlPackage displaying this page)\n");
                tmpS.Append("                </td>\n");
                tmpS.Append("              </tr>\n");

                tmpS.Append("              <tr valign=\"top\">\n");
                tmpS.Append("                <td align=\"right\" valign=\"top\">Order " + specs.m_ObjectNamePlural + " By Looks:&nbsp;&nbsp;</td>\n");
                tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                tmpS.Append("Yes&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"SortByLooks\" value=\"1\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "SortByLooks"), " checked ", ""), "") + ">\n");
                tmpS.Append("No&nbsp;<INPUT TYPE=\"RADIO\" NAME=\"SortByLooks\" value=\"0\" " + CommonLogic.IIF(Editing, CommonLogic.IIF(DB.RSFieldBool(rs, "SortByLooks"), "", " checked "), " checked ") + ">\n");
                tmpS.Append("                </td>\n");
                tmpS.Append("              </tr>\n");

                if (specs.m_HasAddress)
                {
                    tmpS.Append("              <tr valign=\"middle\">\n");
                    tmpS.Append("                <td align=\"right\" valign=\"middle\">Street Address:&nbsp;&nbsp;</td>\n");
                    tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                    tmpS.Append("                	<input maxLength=\"100\" size=\"30\" name=\"Address1\" value=\"" + CommonLogic.IIF(Editing, HttpContext.Current.Server.HtmlEncode(DB.RSField(rs, "Address1")), "") + "\">\n");
                    //tmpS.Append("                    <input type=\"hidden\" name=\"Address1_vldt\" value=\"[req][blankalert=Please enter a street address]\">\n");
                    tmpS.Append("                	&nbsp;&nbsp;\n");
                    tmpS.Append("                	Apt/Suite#:\n");
                    tmpS.Append("                	<input maxLength=\"100\" size=\"5\" name=\"Suite\" value=\"" + CommonLogic.IIF(Editing, HttpContext.Current.Server.HtmlEncode(DB.RSField(rs, "Suite")), "") + "\">\n");
                    tmpS.Append("                </td>\n");
                    tmpS.Append("              </tr>\n");
                    tmpS.Append("              <tr valign=\"middle\">\n");
                    tmpS.Append("                <td align=\"right\" valign=\"middle\"></td>\n");
                    tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                    tmpS.Append("                	<input maxLength=\"100\" size=\"30\" name=\"Address2\" value=\"" + CommonLogic.IIF(Editing, HttpContext.Current.Server.HtmlEncode(DB.RSField(rs, "Address2")), "") + "\">\n");
                    tmpS.Append("                </td>\n");
                    tmpS.Append("              </tr>\n");
                    tmpS.Append("              <tr valign=\"middle\">\n");
                    tmpS.Append("                <td align=\"right\" valign=\"middle\">City:&nbsp;&nbsp;</td>\n");
                    tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                    tmpS.Append("                	<input maxLength=\"100\" size=\"30\" name=\"City\" value=\"" + CommonLogic.IIF(Editing, HttpContext.Current.Server.HtmlEncode(DB.RSField(rs, "City")), "") + "\">\n");
                    //tmpS.Append("                    <input type=\"hidden\" name=\"City_vldt\" value=\"[req][blankalert=Please enter a city]\">\n");
                    tmpS.Append("                </td>\n");
                    tmpS.Append("              </tr>\n");

                    tmpS.Append("              <tr valign=\"middle\">\n");
                    tmpS.Append("                <td align=\"right\" valign=\"middle\">State:&nbsp;&nbsp;</td>\n");
                    tmpS.Append("                <td align=\"left\" valign=\"top\">");
                    tmpS.Append("<select size=\"1\" name=\"State\">");
                    tmpS.Append("<OPTION value=\"\"" + CommonLogic.IIF(DB.RSField(rs, "State").Length == 0, " selected", String.Empty) + ">SELECT ONE</option>");
                    DataSet dsstate = DB.GetDS("select * from state  " + DB.GetNoLock() + " order by DisplayOrder,Name", AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
                    foreach (DataRow row in dsstate.Tables[0].Rows)
                    {
                        tmpS.Append("<OPTION value=\"" + HttpContext.Current.Server.HtmlEncode(DB.RowField(row, "Abbreviation")) + "\"" + CommonLogic.SelectOption(rs, DB.RowField(row, "Abbreviation"), "State") + ">" + HttpContext.Current.Server.HtmlEncode(DB.RowField(row, "Name")) + "</option>");
                    }
                    dsstate.Dispose();
                    tmpS.Append("</select>");
                    tmpS.Append("			</td>\n");
                    tmpS.Append("              </tr>\n");

                    tmpS.Append("              <tr valign=\"middle\">\n");
                    tmpS.Append("                <td align=\"right\" valign=\"middle\">Zip Code:&nbsp;&nbsp;</td>\n");
                    tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                    tmpS.Append("                	<input maxLength=\"16\" size=\"15\" name=\"ZipCode\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "ZipCode"), "") + "\">\n");
                    tmpS.Append("                    <input type=\"hidden\" name=\"ZipCode_vldt\" value=\"[invalidalert=Please enter a valid zipcode]\">\n");
                    tmpS.Append("                </td>\n");
                    tmpS.Append("              </tr>\n");

                    tmpS.Append("              <tr valign=\"middle\">\n");
                    tmpS.Append("                <td align=\"right\" valign=\"middle\">Country:&nbsp;&nbsp;</td>\n");
                    tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                    tmpS.Append("<SELECT NAME=\"Country\" size=\"1\">");
                    tmpS.Append("<option value=\"0\">SELECT ONE</option>");
                    DataSet dscountry2 = DB.GetDS("select * from country  " + DB.GetNoLock() + " order by DisplayOrder,Name", AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
                    foreach (DataRow row in dscountry2.Tables[0].Rows)
                    {
                        tmpS.Append("<OPTION value=\"" + DB.RowField(row, "Name") + "\"" + CommonLogic.IIF(DB.RSField(rs, "Country") == DB.RowField(row, "Name"), " selected ", "") + ">" + DB.RowField(row, "Name") + "</option>");
                    }
                    dscountry2.Dispose();
                    tmpS.Append("</SELECT>");
                    tmpS.Append("                </td>\n");
                    tmpS.Append("              </tr>\n");

                    tmpS.Append("              <tr valign=\"middle\">\n");
                    tmpS.Append("                <td align=\"right\" valign=\"middle\">Web Site:&nbsp;&nbsp;</td>\n");
                    tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                    tmpS.Append("                	<input maxLength=\"100\" size=\"35\" name=\"URL\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "URL"), "") + "\">&nbsp;&nbsp;<small>(e.g. http://abcd.com)</small>\n");
                    tmpS.Append("               	</td>\n");
                    tmpS.Append("              </tr>\n");
                    tmpS.Append("              <tr valign=\"middle\">\n");
                    tmpS.Append("                <td align=\"right\" valign=\"middle\">E-Mail Address:&nbsp;&nbsp;</td>\n");
                    tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                    tmpS.Append("                	<input maxLength=\"100\" size=\"35\" name=\"EMail\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "EMail"), CommonLogic.QueryStringCanBeDangerousContent("EMail")) + "\">\n");
                    tmpS.Append("                    <input type=\"hidden\" name=\"EMail_vldt\" value=\"" + CommonLogic.IIF(specs.m_EntityName.ToUpperInvariant() == "DISTRIBUTOR", "[req][blankalert=EMail is a required field for a Distributor]", "") + "[invalidalert=Please enter a valid e-mail address]\">\n");
                    tmpS.Append("               	</td>\n");
                    tmpS.Append("              </tr>\n");
                    tmpS.Append("              <tr valign=\"middle\">\n");
                    tmpS.Append("                <td align=\"right\" valign=\"middle\">Phone:&nbsp;&nbsp;</td>\n");
                    tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                    tmpS.Append("                	<input maxLength=\"35\" size=\"35\" name=\"Phone\" value=\"" + CommonLogic.IIF(Editing, CommonLogic.GetPhoneDisplayFormat(DB.RSField(rs, "Phone")), "") + "\">&nbsp;&nbsp;<small>(optional, including area code)</small>\n");
                    tmpS.Append("                    <input type=\"hidden\" name=\"Phone_vldt\" value=\"[invalidalert=Please enter a valid phone number with areacode, e.g. (480) 555-1212]\">\n");
                    tmpS.Append("                </td>\n");
                    tmpS.Append("              </tr>\n");
                    tmpS.Append("              <tr valign=\"middle\">\n");
                    tmpS.Append("                <td align=\"right\" valign=\"middle\">Fax:&nbsp;&nbsp;</td>\n");
                    tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                    tmpS.Append("                	<input maxLength=\"35\" size=\"35\" name=\"FAX\" value=\"" + CommonLogic.IIF(Editing, CommonLogic.GetPhoneDisplayFormat(DB.RSField(rs, "Fax")), "") + "\">&nbsp;&nbsp;<small>(optional, including area code)</small>\n");
                    tmpS.Append("                    <input type=\"hidden\" name=\"FAX_vldt\" value=\"[invalidalert=Please enter a valid FAX number with areacode, e.g. (480) 555-1212]\">\n");
                    tmpS.Append("                </td>\n");
                    tmpS.Append("              </tr>\n");
                }

                // BEGIN IMAGES 

                tmpS.Append("              <tr valign=\"middle\">\n");
                tmpS.Append("                <td align=\"right\" valign=\"top\">Image Filename Override:&nbsp;&nbsp;</td>\n");
                tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                tmpS.Append("                	<input maxLength=\"100\" size=\"40\" name=\"ImageFilenameOverride\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "ImageFilenameOverride"), "") + "\">\n");
                tmpS.Append("                </td>\n");
                tmpS.Append("              </tr>\n");


                bool disableupload = (Editing && DB.RSField(rs, "ImageFilenameOverride") != "");
                if (specs.m_HasIconPic)
                {
                    tmpS.Append("  <tr>\n");
                    tmpS.Append("    <td valign=\"top\" align=\"right\">Icon:\n");
                    tmpS.Append("</td>\n");
                    tmpS.Append("    <td valign=\"top\" align=\"left\">");
                    tmpS.Append("    <input type=\"file\" name=\"Image1\" size=\"50\" value=\"" + CommonLogic.IIF(Editing, "", "") + "\" " + CommonLogic.IIF(disableupload, " disabled ", "") + ">\n");
                    String Image1URL = AppLogic.LookupImage(specs.m_EntityName, EntityID, "icon", SkinID, ThisCustomer.LocaleSetting);
                    if (Image1URL.Length != 0)
                    {
                        if (Image1URL.IndexOf("nopicture") == -1)
                        {
                            tmpS.Append("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image1URL + "','Pic1');\">Click here</a> to delete the current image<br/>\n");
                        }
                        tmpS.Append("<br/><img id=\"Pic1\" name=\"Pic1\" border=\"0\" src=\"" + Image1URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\">\n");
                    }
                    tmpS.Append("</td>\n");
                    tmpS.Append(" </tr>\n");
                }

                if (specs.m_HasMediumPic)
                {
                    tmpS.Append("  <tr>\n");
                    tmpS.Append("    <td valign=\"top\" align=\"right\">Medium Pic:\n");
                    tmpS.Append("</td>\n");
                    tmpS.Append("    <td valign=\"top\" align=\"left\">");
                    tmpS.Append("    <input type=\"file\" name=\"Image2\" size=\"50\" value=\"" + CommonLogic.IIF(Editing, "", "") + "\" " + CommonLogic.IIF(disableupload, " disabled ", "") + ">\n");
                    String Image2URL = AppLogic.LookupImage(specs.m_EntityName, EntityID, "medium", SkinID, ThisCustomer.LocaleSetting);
                    if (Image2URL.Length != 0)
                    {
                        if (Image2URL.IndexOf("nopicture") == -1)
                        {
                            tmpS.Append("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image2URL + "','Pic2');\">Click here</a> to delete the current image<br/>\n");
                        }
                        tmpS.Append("<br/><img id=\"Pic2\" name=\"Pic2\" border=\"0\" src=\"" + Image2URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\">\n");
                    }
                    tmpS.Append("</td>\n");
                    tmpS.Append(" </tr>\n");
                }

                if (specs.m_HasLargePic)
                {
                    tmpS.Append("  <tr>\n");
                    tmpS.Append("    <td valign=\"top\" align=\"right\">Large Pic:\n");
                    tmpS.Append("</td>\n");
                    tmpS.Append("    <td valign=\"top\" align=\"left\">");
                    tmpS.Append("    <input type=\"file\" name=\"Image3\" size=\"50\" value=\"" + CommonLogic.IIF(Editing, "", "") + "\" " + CommonLogic.IIF(disableupload, " disabled ", "") + ">\n");
                    String Image3URL = AppLogic.LookupImage(specs.m_EntityName, EntityID, "large", SkinID, ThisCustomer.LocaleSetting);
                    if (Image3URL.Length == 0)
                    {
                        Image3URL = AppLogic.NoPictureImageURL(false, SkinID, ThisCustomer.LocaleSetting);
                    }
                    if (Image3URL.Length != 0)
                    {
                        if (Image3URL.IndexOf("nopicture") == -1)
                        {
                            tmpS.Append("<a href=\"javascript:void(0);\" onClick=\"DeleteImage('" + Image3URL + "','Pic3');\">Click here</a> to delete the current image<br/>\n");
                        }
                        tmpS.Append("<br/><img id=\"Pic3\" name=\"Pic3\" border=\"0\" src=\"" + Image3URL + "?" + CommonLogic.GetRandomNumber(1, 1000000).ToString() + "\">\n");
                    }
                    tmpS.Append("</td>\n");
                    tmpS.Append(" </tr>\n");
                }

                // END IMAGES

                tmpS.Append("              <tr valign=\"top\">\n");
                tmpS.Append("                <td align=\"right\" valign=\"top\">Summary:&nbsp;&nbsp;</td>\n");
                tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                tmpS.Append(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Summary"), "Summary", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true));
                //tmpS.Append("                	<textarea style=\"height: 30em; width: 100%;\" cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeightSmall") + "\" id=\"Summary\" name=\"Summary\">" + CommonLogic.IIF(Editing , HttpContext.Current.Server.HtmlEncode(DB.RSField(rs,"Summary")) , "") + "</textarea>\n");
                tmpS.Append("                </td>\n");
                tmpS.Append("              </tr>\n");

                tmpS.Append("              <tr valign=\"top\">\n");
                tmpS.Append("                <td align=\"right\" valign=\"top\">Description:&nbsp;&nbsp;</td>\n");
                tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                tmpS.Append(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "Description"), "Description", true, true, false, "", 0, 0, AppLogic.AppConfigUSInt("Admin_TextareaHeight"), AppLogic.AppConfigUSInt("Admin_TextareaWidth"), true));
                //        tmpS.Append("                	<textarea style=\"height: 60em; width: 100%;\" cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"" + AppLogic.AppConfig("Admin_TextareaHeightl") + "\" id=\"Description\" name=\"Description\">" + CommonLogic.IIF(Editing , HttpContext.Current.Server.HtmlEncode(DB.RSField(rs,"Description")) , "") + "</textarea>\n");
                tmpS.Append("                </td>\n");
                tmpS.Append("              </tr>\n");

                tmpS.Append("              <tr valign=\"top\">\n");
                tmpS.Append("                <td align=\"right\" valign=\"top\">Extension Data (User Defined Data):&nbsp;&nbsp;</td>\n");
                tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                tmpS.Append("                	<textarea style=\"width: 100%;\" cols=\"" + AppLogic.AppConfig("Admin_TextareaWidth") + "\" rows=\"10\" id=\"ExtensionData\" name=\"ExtensionData\">" + CommonLogic.IIF(Editing, DB.RSField(rs, "ExtensionData"), "") + "</textarea>\n");
                tmpS.Append("                </td>\n");
                tmpS.Append("              </tr>\n");

                tmpS.Append("              <tr valign=\"top\">\n");
                tmpS.Append("                <td align=\"right\" valign=\"top\">Search Engine Page Title:&nbsp;&nbsp;</td>\n");
                tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                tmpS.Append(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "SETitle"), "SETitle", false, true, false, "", 100, 100, 0, 0, false));
                //        tmpS.Append("                	<input maxLength=\"100\" size=\"100\" name=\"SETitle\" value=\"" + CommonLogic.IIF(Editing , DB.RSField(rs,"SETitle") , "") + "\">\n");
                tmpS.Append("                </td>\n");
                tmpS.Append("              </tr>\n");

                tmpS.Append("              <tr valign=\"top\">\n");
                tmpS.Append("                <td align=\"right\" valign=\"top\">Search Engine Keywords:&nbsp;&nbsp;</td>\n");
                tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                tmpS.Append(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "SEKeywords"), "SEKeywords", false, true, false, "", 255, 100, 0, 0, false));
                //        tmpS.Append("                	<input maxLength=\"255\" size=\"100\" name=\"SEKeywords\" value=\"" + CommonLogic.IIF(Editing , DB.RSField(rs,"SEKeywords") , "") + "\">\n");
                tmpS.Append("                </td>\n");
                tmpS.Append("              </tr>\n");

                tmpS.Append("              <tr valign=\"top\">\n");
                tmpS.Append("                <td align=\"right\" valign=\"top\">Search Engine Description:&nbsp;&nbsp;</td>\n");
                tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                tmpS.Append(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "SEDescription"), "SEDescription", false, true, false, "", 255, 100, 0, 0, false));
                //        tmpS.Append("                	<input maxLength=\"255\" size=\"100\" name=\"SEDescription\" value=\"" + CommonLogic.IIF(Editing , DB.RSField(rs,"SEDescription") , "") + "\">\n");
                tmpS.Append("                </td>\n");
                tmpS.Append("              </tr>\n");

                tmpS.Append("              <tr valign=\"top\">\n");
                tmpS.Append("                <td align=\"right\" valign=\"top\">Search Engine NoScript:&nbsp;&nbsp;</td>\n");
                tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                tmpS.Append(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "SENoScript"), "SENoScript", true, true, false, "", 50, 50, 0, 0, false));
                //        tmpS.Append("                	<textarea name=\"SENoScript\" rows=\"10\" cols=\"50\">" + CommonLogic.IIF(Editing , DB.RSField(rs,"SENoScript") , "") + "</textarea>\n");
                tmpS.Append("                </td>\n");
                tmpS.Append("              </tr>\n");

                //tmpS.Append("              <tr valign=\"top\">\n");
                //tmpS.Append("                <td align=\"right\" valign=\"top\">Search Engine AltText:&nbsp;&nbsp;</td>\n");
                //tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                //tmpS.Append(AppLogic.GetLocaleEntryFields(DB.RSField(rs, "SEAltText"), "SEAltText", false, true, false, "", 50, 50, 0, 0, false));
                ////        tmpS.Append("                	<input maxLength=\"50\" size=\"50\" name=\"SEAltText\" value=\"" + CommonLogic.IIF(Editing , DB.RSField(rs,"SEAltText") , "") + "\">\n");
                //tmpS.Append("                </td>\n");
                //tmpS.Append("              </tr>\n");

                tmpS.Append("              <tr valign=\"middle\">\n");
                tmpS.Append("                <td align=\"right\" valign=\"top\">Page BG Color:&nbsp;&nbsp;</td>\n");
                tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                tmpS.Append("                	<input maxLength=\"20\" size=\"10\" name=\"PageBGColor\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "PageBGColor"), "") + "\">\n");
                tmpS.Append("                </td>\n");
                tmpS.Append("              </tr>\n");

                tmpS.Append("              <tr valign=\"middle\">\n");
                tmpS.Append("                <td align=\"right\" valign=\"top\">Contents BG Color:&nbsp;&nbsp;</td>\n");
                tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                tmpS.Append("                	<input maxLength=\"20\" size=\"10\" name=\"ContentsBGColor\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "ContentsBGColor"), "") + "\">\n");
                tmpS.Append("                </td>\n");
                tmpS.Append("              </tr>\n");

                tmpS.Append("              <tr valign=\"middle\">\n");
                tmpS.Append("                <td align=\"right\" valign=\"top\">Skin Graphics Color:&nbsp;&nbsp;</td>\n");
                tmpS.Append("                <td align=\"left\" valign=\"top\">\n");
                tmpS.Append("                	<input maxLength=\"20\" size=\"10\" name=\"GraphicsColor\" value=\"" + CommonLogic.IIF(Editing, DB.RSField(rs, "GraphicsColor"), "") + "\">\n");
                tmpS.Append("                </td>\n");
                tmpS.Append("              </tr>\n");

                tmpS.Append("<tr>\n");
                tmpS.Append("<td></td><td align=\"left\" valign=\"top\"><br/>\n");
                if (Editing)
                {
                    tmpS.Append("<input type=\"submit\" value=\"Update\" name=\"submit\">\n");
                    tmpS.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"reset\" class=\"CPButton\" value=\"Reset\" name=\"reset\">\n");
                }
                else
                {
                    tmpS.Append("<input type=\"submit\" value=\"Add New\" name=\"submit\">\n");
                }
                tmpS.Append("        </td>\n");
                tmpS.Append("      </tr>\n");
                tmpS.Append("  </table>\n");

                tmpS.Append("<script type=\"text/javascript\">\n");

                tmpS.Append("function DeleteImage(imgurl,name)\n");
                tmpS.Append("{\n");
                tmpS.Append("window.open('deleteimage.aspx?imgurl=' + imgurl + '&FormImageName=' + name,\"Admin_ML\",\"height=250,width=440,top=10,left=20,status=no,toolbar=no,menubar=no,scrollbars=yes,location=no\")\n");
                tmpS.Append("}\n");

                tmpS.Append("</SCRIPT>\n");
            }
            rs.Close();
            tmpS.Append("</form>\n");
            return tmpS.ToString();
        }

        static public void EntityListPageFormHandler(EntitySpecs specs, Customer ThisCustomer, int SkinID)
        {
            if (CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
            {
                // delete the record:
                DB.ExecuteSQL("update " + specs.m_EntityName + " set Deleted=1 where " + specs.m_EntityName + "ID=" + CommonLogic.QueryStringCanBeDangerousContent("DeleteID"));
                HttpContext.Current.Response.Redirect("entities.aspx?entityname=" + specs.m_EntityName);
            }
            if (CommonLogic.FormBool("IsSubmit"))
            {
                for (int i = 0; i <= HttpContext.Current.Request.Form.Count - 1; i++)
                {
                    if (HttpContext.Current.Request.Form.Keys[i].IndexOf("DisplayOrder_") != -1)
                    {
                        String[] keys = HttpContext.Current.Request.Form.Keys[i].Split('_');
                        int EntityID = Localization.ParseUSInt(keys[1]);
                        int DispOrd = 1;
                        try
                        {
                            DispOrd = Localization.ParseUSInt(HttpContext.Current.Request.Form[HttpContext.Current.Request.Form.Keys[i]]);
                        }
                        catch { }
                        DB.ExecuteSQL("update " + specs.m_EntityName + " set DisplayOrder=" + DispOrd.ToString() + " where " + specs.m_EntityName + "ID=" + EntityID.ToString());
                    }
                }
                HttpContext.Current.Response.Redirect("entities.aspx?entityname=" + specs.m_EntityName);
            }
        }

        static public String EntityListPageRender(AspDotNetStorefront.SkinBase sb, EntitySpecs specs, Customer ThisCustomer, int SkinID)
        {
            StringBuilder tmpS = new StringBuilder(50000);
            EntityHelper EntityMgr = AppLogic.LookupHelper(sb.EntityHelpers, specs.m_EntityName);

            tmpS.Append("<form method=\"POST\" action=\"entities.aspx?entityname=" + specs.m_EntityName + "\">\n");
            tmpS.Append("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
            tmpS.Append("<p align=\"left\"><input type=\"button\" value=\"Add New " + AppLogic.GetString("AppConfig." + specs.m_EntityName + "PromptSingular", SkinID, ThisCustomer.LocaleSetting) + "\" name=\"AddNew\" onClick=\"self.location='editentity.aspx?entityname=" + specs.m_EntityName + "';\"><p>");
            tmpS.Append("  <table border=\"0\" cellpadding=\"2\" border=\"0\" cellspacing=\"1\" width=\"100%\">\n");
            tmpS.Append("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
            tmpS.Append("      <td><b>ID</b></td>\n");
            tmpS.Append("      <td><b>" + AppLogic.GetString("AppConfig." + specs.m_EntityName + "PromptSingular", SkinID, ThisCustomer.LocaleSetting) + "</b></td>\n");
            if (specs.m_HasParentChildRelationship)
            {
                tmpS.Append("      <td align=\"center\"><b>Parent " + AppLogic.GetString("AppConfig." + specs.m_EntityName + "PromptSingular", SkinID, ThisCustomer.LocaleSetting) + "</b></td>\n");
            }
            tmpS.Append("      <td align=\"center\"><b>" + AppLogic.GetString("AppConfig." + specs.m_EntityName + "PromptSingular", SkinID, ThisCustomer.LocaleSetting) + " Display Order</b></td>\n");
            tmpS.Append("      <td align=\"center\"><b>Edit " + AppLogic.GetString("AppConfig." + specs.m_EntityName + "PromptSingular", SkinID, ThisCustomer.LocaleSetting) + "</b></td>\n");
            tmpS.Append("      <td align=\"center\"><b>View " + specs.m_ObjectNamePlural + "</b></td>\n");
            tmpS.Append("      <td align=\"center\"><b>Set " + specs.m_ObjectNamePlural + " Display Order</b></td>\n");
            if (specs.m_ObjectName == "Product")
            {
                tmpS.Append("      <td align=\"center\"><b>Bulk " + specs.m_ObjectNamePlural + " Edit</b></td>\n");
            }
            tmpS.Append("      <td align=\"center\"><b>Delete " + AppLogic.GetString("AppConfig." + specs.m_EntityName + "PromptSingular", SkinID, ThisCustomer.LocaleSetting) + "</b></td>\n");
            tmpS.Append("    </tr>\n");

            GetEntities(specs, EntityMgr, tmpS, 0, 1, SkinID, ThisCustomer);

            tmpS.Append("    <tr>\n");
            tmpS.Append("      <td colspan=\"" + CommonLogic.IIF(specs.m_HasParentChildRelationship, "3", "2") + "\" align=\"left\"></td>\n");
            tmpS.Append("      <td align=\"center\" bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\"><input type=\"submit\" value=\"Update\" name=\"Submit\"></td>\n");
            tmpS.Append("      <td colspan=\"" + CommonLogic.IIF(specs.m_ObjectName == "Product", "5", "4") + "\"></td>\n");
            tmpS.Append("    </tr>\n");
            tmpS.Append("  </table>\n");
            tmpS.Append("<p align=\"left\"><input type=\"button\" value=\"Add New " + AppLogic.GetString("AppConfig." + specs.m_EntityName + "PromptSingular", SkinID, ThisCustomer.LocaleSetting) + "\" name=\"AddNew\" onClick=\"self.location='editentity.aspx?entityname=" + specs.m_EntityName + "';\"><p>");
            tmpS.Append("</form>\n");

            tmpS.Append("</center></b>\n");

            tmpS.Append("<script type=\"text/javascript\">\n");
            tmpS.Append("function Delete" + specs.m_EntityName + "(id)\n");
            tmpS.Append("{\n");
            tmpS.Append("if(confirm('Are you sure you want to delete " + AppLogic.GetString("AppConfig." + specs.m_EntityName + "PromptSingular", SkinID, ThisCustomer.LocaleSetting) + ": ' + id))\n");
            tmpS.Append("{\n");
            tmpS.Append("self.location = '" + CommonLogic.GetThisPageName(false) + "?entityname=" + specs.m_EntityName + "&deleteid=' + id;\n");
            tmpS.Append("}\n");
            tmpS.Append("}\n");
            tmpS.Append("</SCRIPT>\n");
            return tmpS.ToString();
        }

        private static void GetEntities(EntitySpecs specs, EntityHelper EntityMgr, StringBuilder tmpS, int ForParentEntityID, int level, int SkinID, Customer ThisCustomer)
        {
            String Indent = String.Empty;
            for (int i = 1; i < level; i++)
            {
                Indent += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            }

            XmlNode n;
            if (ForParentEntityID == 0)
            {
                n = EntityMgr.m_TblMgr.ResetToRootNode();
            }
            else
            {
                n = EntityMgr.m_TblMgr.SetContext(ForParentEntityID);
            }

            if (n != null && EntityMgr.m_TblMgr.HasChildren(n))
            {
                n = EntityMgr.m_TblMgr.MoveFirstChild(n);
                while (n != null)
                {
                    int ThisID = EntityMgr.m_TblMgr.CurrentID(n);
                    tmpS.Append("    <tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">\n");
                    tmpS.Append("      <td >" + ThisID.ToString() + "</td>\n");
                    tmpS.Append("<td>\n");
                    String Image1URL = AppLogic.LookupImage(specs.m_EntityName, ThisID, "icon", SkinID, ThisCustomer.LocaleSetting);
                    tmpS.Append("<a href=\"editentity.aspx?entityname=" + specs.m_EntityName + "&entityid=" + ThisID.ToString() + "\">");
                    tmpS.Append("<img src=\"" + Image1URL + "\" height=\"25\" border=\"0\" align=\"absmiddle\">");
                    tmpS.Append("</a>&nbsp;\n");
                    tmpS.Append("<a href=\"editentity.aspx?entityname=" + specs.m_EntityName + "&entityid=" + ThisID.ToString() + "\">");
                    if (level == 1)
                    {
                        tmpS.Append("<b>");
                    }
                    tmpS.Append(Indent + EntityMgr.m_TblMgr.CurrentName(n, ThisCustomer.LocaleSetting));
                    if (level == 1)
                    {
                        tmpS.Append("</b>");
                    }
                    tmpS.Append("</a>");
                    tmpS.Append("</td>\n");
                    if (specs.m_HasParentChildRelationship)
                    {
                        String ParDesc = EntityMgr.m_TblMgr.CurrentFieldInt(n, "ParentEntityID").ToString();
                        if (ParDesc == "0")
                        {
                            ParDesc = "&nbsp;";
                        }
                        tmpS.Append("<td align=\"center\">" + ParDesc + "</td>\n");
                    }
                    tmpS.Append("<td align=\"center\"><input size=4 type=\"text\" name=\"DisplayOrder_" + ThisID.ToString() + "\" value=\"" + EntityMgr.m_TblMgr.CurrentFieldInt(n, "DisplayOrder").ToString() + "\"></td>\n");
                    tmpS.Append("<td align=\"center\"><input type=\"button\" style=\"font-size: 9px;\" value=\"Edit\" name=\"Edit_" + ThisID.ToString() + "\" onClick=\"self.location='editentity.aspx?entityname=" + specs.m_EntityName + "&entityid=" + ThisID.ToString() + "'\"></td>\n");
                    tmpS.Append("<td align=\"center\"><input type=\"button\" style=\"font-size: 9px;\" value=\"" + specs.m_ObjectNamePlural + "\" name=\"" + specs.m_ObjectNamePlural + "_" + ThisID.ToString() + "\" onClick=\"self.location='" + specs.m_ObjectNamePlural + ".aspx?entityname=" + specs.m_EntityName + "&EntityFilterID=" + ThisID.ToString() + "'\"></td>\n");
                    tmpS.Append("<td align=\"center\"><input type=\"button\" style=\"font-size: 9px;\" value=\"DisplayOrder\" name=\"DisplayOrder_" + ThisID.ToString() + "\" onClick=\"self.location='displayorder.aspx?entityname=" + specs.m_EntityName + "&EntityID=" + ThisID.ToString() + "'\"></td>\n");
                    if (specs.m_ObjectName == "Product")
                    {
                        int N = EntityMgr.GetNumEntityObjects(ThisID, true, true);
                        if (N == 0)
                        {
                            tmpS.Append("<td align=\"center\">(No " + specs.m_ObjectNamePlural + ")</td>\n");
                        }
                        else if (N > 100)
                        {
                            tmpS.Append("<td align=\"center\">(Too Many " + specs.m_ObjectNamePlural + ")</td>\n");
                        }
                        else
                        {
                            tmpS.Append("<td align=\"center\">");
                            tmpS.Append("<input type=\"button\" style=\"font-size: 9px;\" value=\"Inventory\" name=\"InventoryEdit_" + ThisID.ToString() + "\" onClick=\"self.location='bulkeditinventory.aspx?entityname=" + specs.m_EntityName + "&EntityID=" + ThisID.ToString() + "'\"><br/>");
                            tmpS.Append("<input type=\"button\" style=\"font-size: 9px;\" value=\"SEFields\" name=\"SearchEdit_" + ThisID.ToString() + "\" onClick=\"self.location='bulkeditsearch.aspx?entityname=" + specs.m_EntityName + "&EntityID=" + ThisID.ToString() + "'\"><br/>");
                            tmpS.Append("<input type=\"button\" style=\"font-size: 9px;\" value=\"Prices\" name=\"PricesEdit_" + ThisID.ToString() + "\" onClick=\"self.location='bulkeditprices.aspx?entityname=" + specs.m_EntityName + "&EntityID=" + ThisID.ToString() + "'\"><br/>");
                            if (Shipping.GetActiveShippingCalculationID() == Shipping.ShippingCalculationEnum.UseIndividualItemShippingCosts)
                            {
                                tmpS.Append("<input type=\"button\" style=\"font-size: 9px;\" value=\"ShipCosts\" name=\"ShippingCostsEdit_" + ThisID.ToString() + "\" onClick=\"self.location='bulkeditshippingcosts.aspx?entityname=" + specs.m_EntityName + "&EntityID=" + ThisID.ToString() + "'\"><br/>");
                            }
                            tmpS.Append("<input type=\"button\" style=\"font-size: 9px;\" value=\"DownloadFiles\" name=\"DownloadFilesEdit_" + ThisID.ToString() + "\" onClick=\"self.location='bulkeditdownloadfiles.aspx?entityname=" + specs.m_EntityName + "&EntityID=" + ThisID.ToString() + "'\"><br/>");
                            tmpS.Append("</td>");
                        }
                    }
                    tmpS.Append("<td align=\"center\"><input type=\"button\" style=\"font-size: 9px;\" value=\"Delete\" name=\"Delete_" + ThisID.ToString() + "\" onClick=\"Delete" + specs.m_EntityName + "(" + ThisID.ToString() + ")\"></td>\n");
                    tmpS.Append("</tr>\n");
                    if (EntityMgr.m_TblMgr.HasChildren(n))
                    {
                        GetEntities(specs, EntityMgr, tmpS, ThisID, level + 1, SkinID, ThisCustomer);
                    }
                    n = EntityMgr.m_TblMgr.MoveNextSibling(n, false);
                }
            }
        }

    }
}
