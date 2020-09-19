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
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

public partial class RTShippingProviderSpecUserData : AspDotNetStorefront.SkinBase
{
    #region Variable Declaration
    
    private string _specFile = string.Empty;

    #endregion

    #region Methods

    #region ParseAndModifySpecFile
    private void ParseAndModifySpecFile()
    {
        Dictionary<string, string> mods = new Dictionary<string, string>();
        foreach (string key in Request.Form.AllKeys)
        {
            if (key.StartsWith("rtshipping.") && !key.EndsWith(".xpath"))
            {
                // key : xpath
                // value : modified value
                string xpath = CommonLogic.FormCanBeDangerousContent(key+".xpath");
                string value = CommonLogic.FormCanBeDangerousContent(key);
                mods.Add(xpath, value);
            }
        }

        XmlDocument doc = GetUserXmlDataDocument();
        foreach (string xpath in mods.Keys)
        {
            XmlNode node = doc.SelectSingleNode(xpath);
            node.Attributes["Default"].Value = mods[xpath];
        }

        DB.ExecuteSQL(
            string.Format(
                "UPDATE RTShippingProvider SET XmlSpecificationUserData = {0} WHERE XmlSpecificationFile = {1}", 
                DB.SQuote(doc.OuterXml),
                DB.SQuote(_specFile)
            )
        );
    }
    #endregion

    #region GenerateXPath
    private string GenerateXPath(IXPathNavigable node)
    {
        string xpath = string.Empty;
        XPathNavigator nav = node.CreateNavigator();

        do
        {
            bool hasIdenticalSibling;
            int position = DeterminePosition(nav, out hasIdenticalSibling);
            xpath = string.Format("{0}{1}/{2}", nav.Name, hasIdenticalSibling?string.Format("[{0}]",position):string.Empty, xpath);
        }
        while (nav.MoveToParent());

        if (xpath.EndsWith("/")) xpath = xpath.Remove(xpath.LastIndexOf("/"));
        return xpath;
    }

    #region DeterminePosition
    private int DeterminePosition(XPathNavigator nav, out bool hasIdenticalSibling)
    {
        hasIdenticalSibling = false;

        int ctr = 0;
        int position = 1;
        XPathNavigator nav2 = nav.Clone();
        if (nav2.MoveToParent())
        {
            nav2.MoveToFirstChild();
            do
            {
                if (nav2.Name == nav.Name)
                {
                    ctr++;
                    if (nav2.IsSamePosition(nav)) position = ctr;
                    hasIdenticalSibling = ctr > 1;
                }
            }
            while (nav2.MoveToNext(XPathNodeType.Element));
        }

        return position;
    }
    #endregion

    #endregion

    #endregion

    #region Events

    #region Page_Load
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");

        // NOTE : verify spec file is not empty
        _specFile = CommonLogic.QueryStringCanBeDangerousContent("specfile");
//#if Gago
//        _specFile = "rtshipping.provider.ups.xml";
//#endif
        SectionTitle = "<a href=\"rtshippingmgr.aspx\">RTShipping Providers</a> - Edit User Provider Specification Data";

        bool isUpdate = CommonLogic.QueryStringBool("update");
        if (isUpdate)
        {
            ParseAndModifySpecFile();
        }
    }
    #endregion

    #endregion

    #region GetUserXmlDataDocument
    private XmlDocument GetUserXmlDataDocument()
    {
        string userXmlData = string.Empty;
        using (IDataReader reader = DB.GetRS(string.Format("SELECT XmlSpecificationUserData FROM RTShippingProvider WHERE XmlSpecificationFile = {0}", DB.SQuote(_specFile))))
        {
            if (reader.Read())
            {
                userXmlData = DB.RSField(reader, "XmlSpecificationUserData");
            }
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(userXmlData);

        return doc;
    }
    #endregion

    #region RenderContents
    protected override void RenderContents(HtmlTextWriter writer)
    {
        if (!string.IsNullOrEmpty(_specFile) )     
        {
            XmlDocument doc = GetUserXmlDataDocument();
            XPathNavigator navigator = doc.CreateNavigator();
            XPathNodeIterator iterator = navigator.Select("descendant::node()[@AllowUserEdit=\"true\"]");

            writer.WriteLine();
            writer.Write(string.Format("<form method=\"POST\" action=\"rtshippingproviderspecuserdata.aspx?update=true&specfile={0}\" >", _specFile));
            writer.WriteLine();

            writer.Write("<table cellspacing=\"0\" width=\"100%\">");
            writer.Write("<tr bgcolor=\"" + AppLogic.AppConfig("LightCellColor") + "\">");
            writer.Write("<td>Name</td>");
            writer.WriteLine();
            writer.Write("<td>Value</td>");
            writer.WriteLine();
            writer.WriteLine();
            writer.Write("</tr>");

            //writer.Write("<th>Name</th>");
            //writer.WriteLine();
            //writer.Write("<th>Value</th>");
            //writer.WriteLine();

            int id = 0;
            while (iterator.MoveNext())
            {
                XPathNavigator currentNode = iterator.Current;                
                writer.Write("<tr>");
                writer.WriteLine();
                writer.Write("<td width=\"30%\" >");
                writer.WriteLine();
                string prompt = currentNode.GetAttribute("Prompt", string.Empty);
                writer.Write(string.IsNullOrEmpty(prompt) ? currentNode.Name : prompt);
                writer.Write("</td>");
                writer.WriteLine();

                writer.Write("<td width=\"70%\" align=\"left\" >");
                writer.WriteLine();
                id++;
                string controlName = string.Format("rtshipping.{0}", id.ToString());
                string xpathName = string.Format("rtshipping.{0}.xpath", id.ToString());
                string xpath = GenerateXPath(currentNode);

                writer.Write(
                    string.Format(
                        "<input type=\"hidden\" id=\"{0}\" name=\"{0}\" value=\"{1}\" />",
                        xpathName,
                        xpath
                    )
                );

                switch (currentNode.GetAttribute("Type", string.Empty).ToLowerInvariant())
                {
                    case "boolean":
                        bool isChecked = bool.TryParse(currentNode.GetAttribute("Default", string.Empty), out isChecked) && isChecked;
                        writer.Write(
                            string.Format(
                                "<input type=\"radio\" id=\"{0}\" name=\"{0}\" value=\"{1}\" {2} /> Yes",
                                controlName,
                                bool.TrueString,
                                isChecked?"checked=\"CHECKED\"":string.Empty
                            )
                        );

                        writer.Write(
                            string.Format(
                                    "<input type=\"radio\" id=\"{0}\" name=\"{0}\" value=\"{1}\"  {2} /> No",
                                    controlName,
                                    bool.FalseString,
                                    !isChecked ? "checked=\"CHECKED\"" : string.Empty
                                )
                        );
                        break;
                    default:
                        writer.Write(
                            string.Format(
                                "<input type=\"text\" id=\"{0}\" name=\"{0}\" value=\"{1}\" style=\"width: 100%\"/>",
                                controlName,
                                currentNode.GetAttribute("Default", string.Empty)
                            )
                        );
                        break;
                }

                writer.Write("</td>");
                writer.WriteLine();

                writer.Write("</tr>");
                writer.WriteLine();
            }

            writer.Write("<tr>");
            writer.WriteLine();
            writer.Write("<td></td>");
            writer.WriteLine();
            writer.Write("<td align=\"right\" >");
            writer.WriteLine();
            writer.Write("<input type=\"submit\" value=\"Save\" >");
            writer.WriteLine();
            writer.Write("</td>");
            writer.WriteLine();
            writer.Write("</tr>");

            writer.Write("</table>");
            writer.WriteLine();
            writer.Write("</form>");
            writer.WriteLine();
            base.RenderContents(writer);
        }
    }

    
    
    
    #endregion
}
