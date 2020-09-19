using System;
using System.Xml;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using AspDotNetStorefrontCommon;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Summary description for ASPDNSFHandlers
/// </summary>
public class GoogleMall:IHttpHandler
{
    public bool IsReusable
    {
        get { return true; }
    }

    public void ProcessRequest(HttpContext context)
    {
        string username = CommonLogic.FormCanBeDangerousContent("Username");
        string auth = CommonLogic.FormCanBeDangerousContent("Auth");
        string ReferralKey = AppLogic.AppConfig("GoogleCheckout.GoogleMallReferalKey");

        string authstring = ReferralKey + ":" + username + ":" + ReferralKey;

        Byte[] authstringBytes = Encoding.UTF8.GetBytes(authstring);
        SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
        sha1.ComputeHash(authstringBytes);
        Byte[] hashedBytes = sha1.Hash;
        sha1.Clear();
        string authstringComputedHash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

        if (auth == authstringComputedHash)
        {
            AppLogic.SetSessionCookie("GoogleMall", "true");
        }
        context.Response.Redirect("default.aspx");
    }
}


/// <summary>
/// Outputs the raw package results along with setting any http headers specified in the package.
/// The package transform output method needs to match the Content-Type http header or you may not get the results you expect
/// </summary>
public class ExecXmlPackage : IHttpHandler
{
    public bool IsReusable
    {
        get { return true; }
    }

    public void ProcessRequest(HttpContext context)
    {
        string pn = CommonLogic.QueryStringCanBeDangerousContent("xmlpackage");
        Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)context.User).ThisCustomer;
        try
        {
            XmlPackage2 p = new XmlPackage2(pn, ThisCustomer, ThisCustomer.SkinID, "", "", "", true);
            if (!p.AllowEngine)
            {
                context.Response.Write("This XmlPackage is not allowed to be run from the engine.  Set the package element's allowengine attribute to true to enable this package to run.");
            }
            else
            {
                if (p.HttpHeaders != null)
                {
                    foreach (XmlNode xn in p.HttpHeaders)
                    {
                        string headername = xn.Attributes["headername"].InnerText;
                        string headervalue = xn.Attributes["headervalue"].InnerText;
                        context.Response.AddHeader(headername, headervalue);
                    }
                }
                string output = p.TransformString();
                context.Response.AddHeader("Content-Length", output.Length.ToString());
                context.Response.Write(output);
            }
        }
        catch (Exception ex)
        {
            context.Response.Write(ex.Message + "<br/><br/>");
            Exception iex = ex.InnerException;
            while (iex != null)
            {
                context.Response.Write(ex.Message + "<br/><br/>");
                iex = iex.InnerException;
            }
        }
    }
}