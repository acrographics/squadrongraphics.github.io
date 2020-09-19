// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web.UI.WebControls;
using AspDotNetStorefrontCommon;
using AspDotNetStorefrontGateways;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for recurringimport.
	/// </summary>
	public partial class recurringimport : System.Web.UI.Page
	{
        protected Customer ThisCustomer;
        String m_GW;
        DateTime dtLastRun = System.DateTime.MinValue;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            dtLastRun = Localization.ParseDBDateTime(AppLogic.AppConfig("Recurring.GatewayLastImportedDate"));
            if (dtLastRun > System.DateTime.MinValue)
            {
                lblLastRun.Text = "Last import was from " + Localization.ToNativeShortDateString(dtLastRun) + "&nbsp;&nbsp;";
            }

            if (dtLastRun.AddDays((double)1.0) >= DateTime.Today)
            {
                txtInputFile.Text = "Nothing to process... You are already up to date.";
                btnGetGatewayStatus.Enabled = false;
            }

            ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
            m_GW = AppLogic.ActivePaymentGatewayCleaned();
            btnGetGatewayStatus.Text = "Get " + CommonLogic.IIF(dtLastRun > System.DateTime.MinValue, "Next ", "Today's ")
                                        + m_GW + " AutoBill Status File...";
            if (!IsPostBack)
            {
                if (!AppLogic.ThereAreRecurringGatewayAutoBillOrders())
                {
                    pnlMain.Visible = false;
                    pnlNotSupported.Visible = true;
                }
                else
                {
                    if (m_GW == Gateway.ro_GWAUTHORIZENET)
                    {
                        btnGetGatewayStatus.Visible = false;
                        btnProcessFile.Visible = true;
//                        txtInputFile.Visible = true;
//                        PastePromptLabel.Visible = true;
                        pnlMain.Visible = true;
                        pnlNotSupported.Visible = false;
                    }
                    else if (m_GW == Gateway.ro_GWVERISIGN || m_GW == Gateway.ro_GWPAYFLOWPRO)
                    {
                        btnGetGatewayStatus.Visible = true;

                        //btnProcessFile.Visible = true; //MS temporary, remove line

//                        txtInputFile.Visible = false;
                        //                        PastePromptLabel.Visible = true;
                        pnlMain.Visible = true;
                        pnlNotSupported.Visible = false;
                    }
                    else if (m_GW == Gateway.ro_GWMANUAL)
                    {
                        btnGetGatewayStatus.Visible = true;
//                        txtInputFile.Visible = false;
                        //                        PastePromptLabel.Visible = true;
                        pnlMain.Visible = true;
                        pnlNotSupported.Visible = false;
                    }
                    else
                    {
                        pnlMain.Visible = false;
                        pnlNotSupported.Visible = true;
                    }
                }
            }
            else
            {
            }
        }

        protected void btnGetGatewayStatus_Click(object sender, EventArgs e)
        {
            txtResults.Text = "";
            btnGetGatewayStatus.Enabled = false;
            //Parser m_Parser = new Parser(ThisCustomer.SkinID, ThisCustomer);
            RecurringOrderMgr rmgr = new RecurringOrderMgr(AppLogic.MakeEntityHelpers(), null);
            btnProcessFile.Visible = true;
            btnProcessFile.Enabled = true;
//            txtInputFile.Visible = true;
            String sResults = String.Empty;
            String Status = rmgr.GetAutoBillStatusFile(m_GW, out sResults);
            if (Status == AppLogic.ro_OK)
            {
                txtInputFile.Text = sResults;
            }
            else
            {
                txtInputFile.Text = Status;
            }
        }

        protected void btnProcessFile_Click(object sender, EventArgs e)
        {
            //Parser m_Parser = new Parser(ThisCustomer.SkinID, ThisCustomer);
            txtResults.Visible = true;

            btnProcessFile.Enabled = false;

            dtLastRun = Localization.ParseDBDateTime(AppLogic.AppConfig("Recurring.GatewayLastImportedDate"));
            DateTime dtRun = dtLastRun;
            if (dtRun == System.DateTime.MinValue)
            {
                dtRun = DateTime.Today.AddDays((double)-1); // Defaults to yesterday
            }
            else
            {
                if (m_GW == Gateway.ro_GWVERISIGN || m_GW == Gateway.ro_GWPAYFLOWPRO)
                {
                    dtRun = DateTime.Today.AddDays((double)-1); // Always runs through yesterday
                }
                else
                {
                    dtRun = dtLastRun.AddDays((double)1.0);
                } 
            }

            if (dtRun >= DateTime.Today)
            {
                txtInputFile.Text = "Nothing to process... You are already up to date.";
                btnGetGatewayStatus.Enabled = false;
                return;
            }


            if (txtInputFile.Text.Length == 0)
            {
                txtResults.Text = "Nothing to process...did you forget to paste the AutoBill transaction report file into the text box to the left?";
            }
            else
            {
                RecurringOrderMgr rmgr = new RecurringOrderMgr(AppLogic.MakeEntityHelpers(), null);
                String sResults = String.Empty;
                String Status = rmgr.ProcessAutoBillStatusFile(m_GW, txtInputFile.Text, out sResults);
                if (Status == AppLogic.ro_OK)
                {
                    txtResults.Text = sResults;
                }
                else
                {
                    txtResults.Text = Status;
                }
            }

            btnGetGatewayStatus.Enabled = true;
            AppLogic.SetAppConfig("Recurring.GatewayLastImportedDate", Localization.ToDBDateTimeString(dtRun), false);
            lblLastRun.Text = "Last import was from " + Localization.ToNativeShortDateString(dtRun) + "&nbsp;&nbsp;";
            dtLastRun = dtRun;

        }
}
}
