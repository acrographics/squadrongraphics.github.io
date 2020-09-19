// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/rortopics.aspx.cs 6     10/04/06 6:21a Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.IO;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for rortopics.
    /// </summary>
    public partial class rortopics : System.Web.UI.Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.ContentType = "text/xml";
            Response.ContentEncoding = new System.Text.UTF8Encoding();
            Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            Response.Write("<rss version=\"2.0\" xmlns:ror=\"http://rorweb.com/0.1/\">");
            Response.Write("<channel>");
            Response.Write("<title>Articles</title>");

            int SkinID = 1; // not sure what to do about this...google can't invoke different skins easily
            String StoreLoc = AppLogic.GetStoreHTTPLocation(false);

            if (AppLogic.AppConfigBool("SiteMap.ShowTopics"))
            {
                // DB Topics:
                DataSet ds = DB.GetDS(string.Format( "select * from Topic {0} where {1} Deleted=0 and (SkinID IS NULL or SkinID=0 or SkinID={2}) Order By DisplayOrder, Name ASC" , DB.GetNoLock() , CommonLogic.IIF(AppLogic.IsAdminSite, "", "ShowInSiteMap=1 and ") , SkinID.ToString() ), AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Response.Write("<item>");
                    Response.Write("<link>" + XmlCommon.XmlEncode(StoreLoc + SE.MakeDriverLink(DB.RowFieldByLocale(row, "Name", Localization.GetWebConfigLocale()))) + "</link>");
                    Response.Write("<ror:type>Article</ror:type>");
                    Response.Write("<ror:descLong>" + XmlCommon.XmlEncode(DB.RowFieldByLocale(row, "Name", Localization.GetWebConfigLocale())) + "</ror:descLong>");
                    Response.Write("<ror:author></ror:author>"); // not available topics
                    Response.Write("<ror:created>" + DB.RowFieldDateTime(row, "CreatedOn").Year.ToString() + "-" + DB.RowFieldDateTime(row, "CreatedOn").Month.ToString() + "-" + DB.RowFieldDateTime(row, "CreatedOn").Day.ToString() + "</ror:created>");
                    Response.Write("<ror:published>" + DB.RowFieldDateTime(row, "CreatedOn").Year.ToString() + "-" + DB.RowFieldDateTime(row, "CreatedOn").Month.ToString() + "-" + DB.RowFieldDateTime(row, "CreatedOn").Day.ToString() + "</ror:published>");
                    Response.Write("<ror:publisher>" + StoreLoc + "</ror:publisher>");
                    Response.Write("</item>");
                }
                ds.Dispose();

                // File Topics:
                // create an array to hold the list of files
                ArrayList fArray = new ArrayList();

                // get information about our initial directory
                String SFP = CommonLogic.SafeMapPath(CommonLogic.IIF(AppLogic.IsAdminSite, "../", "") + "skins/skin_" + SkinID.ToString() + "/template.htm").Replace("template.htm", "");

                DirectoryInfo dirInfo = new DirectoryInfo(SFP);

                // retrieve array of files & subdirectories
                FileSystemInfo[] myDir = dirInfo.GetFileSystemInfos();

                for (int i = 0; i < myDir.Length; i++)
                {
                    // check the file attributes

                    // if a subdirectory, add it to the sArray    
                    // otherwise, add it to the fArray
                    if (((Convert.ToUInt32(myDir[i].Attributes) & Convert.ToUInt32(FileAttributes.Directory)) > 0))
                    {
                        //sArray.Add(Path.GetFileName(myDir[i].FullName));  
                    }
                    else
                    {
                        bool skipit = false;
                        if (!myDir[i].FullName.EndsWith("htm", StringComparison.InvariantCultureIgnoreCase) || (myDir[i].FullName.ToUpperInvariant().IndexOf("TEMPLATE") != -1) || (myDir[i].FullName.ToUpperInvariant().IndexOf("AFFILIATE_") != -1) || (myDir[i].FullName.ToUpperInvariant().IndexOf(AppLogic.ro_PMMicropay) != -1))
                        {
                            skipit = true;
                        }
                        if (!skipit)
                        {
                            fArray.Add(Path.GetFileName(myDir[i].FullName));
                        }
                    }
                }

                if (fArray.Count != 0)
                {
                    // sort the files alphabetically
                    fArray.Sort(0, fArray.Count, null);
                    for (int i = 0; i < fArray.Count; i++)
                    {
                        Response.Write("<item>");
                        Response.Write("<link>" + StoreLoc + SE.MakeDriverLink(fArray[i].ToString().Replace(".htm", "")) + "</link>");
                        Response.Write("<ror:type>Article</ror:type>");
                        Response.Write("<ror:descLong></ror:descLong>"); // not available for file based topics
                        Response.Write("<ror:author></ror:author>"); // not available for file based topics
                        Response.Write("<ror:created></ror:created>"); // not available for file based topics
                        Response.Write("<ror:published></ror:published>"); // not available for file based topics
                        Response.Write("<ror:publisher>" + StoreLoc + "</ror:publisher>");
                        Response.Write("</item>");
                    }
                }
            }

            Response.Write("</channel>");
            Response.Write("</rss>");
        }

    }
}
