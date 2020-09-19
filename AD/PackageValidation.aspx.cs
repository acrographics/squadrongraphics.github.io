// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/PackageValidation.aspx.cs 3     8/19/06 8:50p Buddy $
// ------------------------------------------------------------------------------------------
using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Schema;
using System.Xml;
using AspDotNetStorefrontCommon;

public partial class PackageValidation : System.Web.UI.Page
{
    int intValidErrors = 0;
    Customer ThisCustomer;

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");

        ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
        ltError.Text = "";
    }

    private void ValidatePackageX(string XmlPackagepath)
    {
        intValidErrors = 0;

        XmlReaderSettings settings = new XmlReaderSettings();
        settings.ValidationType = ValidationType.Schema;
        settings.Schemas.Add("", Server.MapPath("../Xmlpackages/xmlpackage.xsd"));
        settings.ValidationEventHandler += new ValidationEventHandler(ValidationHandler);

        XmlReader xmlReaderObj = XmlReader.Create(XmlPackagepath, settings);

        try
        {
            //xd.Validate(ValidationHandler);
            while (xmlReaderObj.Read()) { }
            ltError.Text = "<b>* XML was validated - " + intValidErrors + "</b> error(s) found<br/>" + ltError.Text;
        }
        catch (Exception ex)
        {
            ltError.Text += "<br /><b>* Read/Parser error:</b> " + ex.Message;
        }
        //xmlReader.Close();
        xmlReaderObj.Close();
    }


    private void ValidationHandler(object sender, ValidationEventArgs args)
    {
        //event handler called when a validation error is found
        intValidErrors += 1;   //increment count of errors

        //check the severity of the error
        string strSeverity = "";
        if (args.Severity == XmlSeverityType.Error) strSeverity = "Error";
        if (args.Severity == XmlSeverityType.Warning) strSeverity = "Warning";
        
        //display a message
        ltError.Text += "<b>* Validation error:</b> " + args.Message + ".  ";
        if (args.Exception.LineNumber > 0)
        {
            ltError.Text += "<b>Line:</b> " + args.Exception.LineNumber.ToString() + ", <b>character:</b> " + args.Exception.LinePosition.ToString() + ".  ";
        }
        ltError.Text += "<b>Severity level:</b> '" + strSeverity + "'. <br /><br />";
    }



    protected void validatepackage_Click(object sender, EventArgs e)
    {
        bool inputvalid = false;
        
        String TargetFile = CommonLogic.SafeMapPath("../images" + "/" + this.ThisCustomer.CustomerGUID + DateTime.Now.Millisecond.ToString() + ".xml.config");
        StreamWriter sw = null;

        try
        {
            HttpPostedFile XmlPackageFile = xmlpackage.PostedFile;
            if (XmlPackageFile != null && XmlPackageFile.ContentLength != 0)
            {
                XmlPackageFile.SaveAs(TargetFile);
                inputvalid = true;
            }
            if (xmltext.Text.Trim().Length != 0)
            {
                using (sw = new StreamWriter(TargetFile))
                {
                    // Add some text to the file.
                    sw.Write(xmltext.Text);
                    sw.Close();
                }
                inputvalid = true;
            }

            if (inputvalid)
            {
                ValidatePackageX(TargetFile);
            }
            else
            {
                ltError.Text = "<font color=\"red\"><b>No Valid input was submited</b></font>";
            }
        }
        catch (Exception ex)
        {
            if (sw != null)
            {
                sw.Close();
            }
            ltError.Text += "<br />" + CommonLogic.GetExceptionDetail(ex, "<br/>");
        }
        try { File.Delete(TargetFile); }
        catch { }
        ltError.Text = "<b>OUTPUT: </b>" + ltError.Text;
    }
}
