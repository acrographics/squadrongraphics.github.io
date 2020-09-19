// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/importstringresourcefile1.aspx.cs 5     10/04/06 12:00p Redwoodtree $
// ------------------------------------------------------------------------------------------
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
using System.Text;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    public partial class importstringresourcefile1 : System.Web.UI.Page
    {
        protected Customer cust;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
            Server.ScriptTimeout = 1000000;

            cust = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            bool isMasterReload = CommonLogic.QueryStringBool("master");
            pnlReload.Visible = isMasterReload;
            pnlUpload.Visible = !isMasterReload;

            if (!IsPostBack)
            {
                Literal1.Text = CommonLogic.QueryStringCanBeDangerousContent("ShowLocaleSetting");
                if(isMasterReload )
                {
                    litStage.Text = string.Format("Reload {0} Master String Resource File", CommonLogic.QueryStringCanBeDangerousContent("ShowLocaleSetting"));
                }
                else
                {
                    litStage.Text = string.Format("Import {0} String Resource File (Step 1)", CommonLogic.QueryStringCanBeDangerousContent("ShowLocaleSetting"));
                }

                lnkBack1.NavigateUrl = "stringresource.aspx?showlocalesetting=" + Localization.CheckLocaleSettingForProperCase(CommonLogic.QueryStringCanBeDangerousContent("ShowLocaleSetting"));
                lnkBack2.NavigateUrl = "stringresource.aspx?showlocalesetting=" + Localization.CheckLocaleSettingForProperCase(CommonLogic.QueryStringCanBeDangerousContent("ShowLocaleSetting"));
            }
        }

        protected void resetError(string error, bool isError)
        {
            string str = "<font class=\"noticeMsg\">NOTICE:</font>&nbsp;&nbsp;&nbsp;";
            if (isError)
            {
                str = "<font class=\"errorMsg\">ERROR:</font>&nbsp;&nbsp;&nbsp;";
            }

            if (error.Length > 0)
            {
                str += error + "";
            }
            else
            {
                str = "";
            }

            ltError.Text = str;
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            String ShowLocaleSetting = Localization.CheckLocaleSettingForProperCase(CommonLogic.QueryStringCanBeDangerousContent("ShowLocaleSetting"));
            String SpreadsheetName = "Strings_" + Localization.ToNativeShortDateString(System.DateTime.Now).Replace(" ", "").Replace("/", "").Replace(":", "").Replace(".", "");
            bool DataUpdated = false;
            resetError("", false);

            ImportOption option = ImportOption.Default;
            if (chkLeaveModified.Checked)
            {
                option = option | ImportOption.LeaveModified;
            }
            if (chkReplaceExisting.Checked)
            {
                option = option | ImportOption.OverWrite;
            }

            // handle file upload:
            try
            {
                String TargetFile = CommonLogic.SafeMapPath("../images" + "/" + SpreadsheetName + ".xls");
                fuMain.SaveAs(TargetFile);
                DataUpdated = true;
            }
            catch (Exception ex)
            {
                resetError("Could not upload file: " + ex.ToString(), true);
            }

            if (DataUpdated)
            {
                resetError("<a href=\"importstringresourcefile2.aspx?spreadsheetname=" + SpreadsheetName + "&showlocalesetting=" + ShowLocaleSetting + "&option=" + ((int)option).ToString() + "\"><strong>Upload successful. Click here to go to preview the upload...</strong></a>\n", false);
            }
        }

        protected void btnReload_Click(object sender, EventArgs e)
        {
            String ShowLocaleSetting = Localization.CheckLocaleSettingForProperCase(CommonLogic.QueryStringCanBeDangerousContent("ShowLocaleSetting"));
            
            resetError("", false);

            ImportOption option = ImportOption.Default;
            if (chkReloadLeaveModified.Checked)
            {
                option = option | ImportOption.LeaveModified;
            }
            if (chkReloadReplaceExisting.Checked)
            {
                option = option | ImportOption.OverWrite;
            }

            Response.Redirect("importstringresourcefile2.aspx?master=true&showlocalesetting=" + ShowLocaleSetting + "&option=" + ((int)option).ToString());            
        }
    }
}