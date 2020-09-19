// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/changeencryptkey.aspx.cs 4     9/05/06 8:32p Redwoodtree $
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
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{

    public partial class changeencryptkey : AspDotNetStorefront.SkinBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Server.ScriptTimeout = 5000000;

            if (!ThisCustomer.IsAdminSuperUser)
            {
                Response.Redirect("default.aspx");
            }
            if (!IsPostBack)
            {
                bool StoringCCInDB = AppLogic.StoreCCInDB();
                bool HaveRecurringThatNeedCC = AppLogic.ThereAreRecurringOrdersThatNeedCCStorage();
                bool ValidTrustLevel = (AppLogic.TrustLevel == AspNetHostingPermissionLevel.Unrestricted || AppLogic.TrustLevel == AspNetHostingPermissionLevel.High);
                StoringCC.Text = CommonLogic.IIF(StoringCCInDB, "Yes", "No");
                RecurringProducts.Text = CommonLogic.IIF(HaveRecurringThatNeedCC, "Yes", "No");

                if ((!StoringCCInDB && !HaveRecurringThatNeedCC) || !ValidTrustLevel)
                {
                    DoItPanel.Visible = false;
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (NewEncryptKey.Text.Trim().Length < 8 || NewEncryptKey.Text.Trim().Length > 50)
            {
                ErrorLabel.Text = "The EncryptKey must be at least 8, and at most 50, alphanumeric characters long";
                ErrorLabel.Visible = true;
                return;
            }
            try
            {
                Security.ChangeEncryptKey(NewEncryptKey.Text);
                ErrorLabel.Visible = false;
                OkLabel.Visible = true;
            }
            catch (Exception ex)
            {
                ErrorLabel.Text = CommonLogic.GetExceptionDetail(ex, "<br/>");
                OkLabel.Visible = false;
                ErrorLabel.Visible = true;
            }
        }
    }
}