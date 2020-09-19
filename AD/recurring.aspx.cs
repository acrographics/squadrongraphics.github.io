// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using System.Collections;
using System.IO;
using AspDotNetStorefrontCommon;
using AspDotNetStorefrontGateways;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for recurring.
    /// </summary>
    public partial class recurring : AspDotNetStorefront.SkinBase
    {

        // NOTE: THIS PAGE DOES NOT PROCESS GATEWAY AUTOBILL RECURRING ORDERS!
        // NOTE: USE THE RECURRINGIMPORT.ASPX PAGE FOR THOSE!!
        
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            SectionTitle = "Recurring Shipments " + CommonLogic.IIF(CommonLogic.QueryStringCanBeDangerousContent("Show").ToUpperInvariant() != "ALL", "Due Today", "All Pending");
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            if (CommonLogic.QueryStringBool("ProcessAll"))
            {
                IDataReader rsp = DB.GetRS("Select distinct(OriginalRecurringOrderNumber) from ShoppingCart where RecurringSubscriptionID='' and CartType=" + ((int)CartTypeEnum.RecurringCart).ToString() + " and NextRecurringShipDate<" + DB.SQuote(Localization.ToDBShortDateString(System.DateTime.Now.AddDays(1))));
                RecurringOrderMgr rmgr = new RecurringOrderMgr(base.EntityHelpers, base.GetParser);
                while (rsp.Read())
                {
                    writer.Write("Processing Next Occurrence of Original Recurring Order # " + DB.RSFieldInt(rsp, "OriginalRecurringOrderNumber").ToString() + "...");
                    writer.Write(rmgr.ProcessRecurringOrder(DB.RSFieldInt(rsp, "OriginalRecurringOrderNumber")));
                    writer.Write("...<br/>");
                }
                rsp.Close();
            }

            int OriginalRecurringOrderNumber = CommonLogic.QueryStringUSInt("OriginalRecurringOrderNumber");
            int ProcessCustomerID = CommonLogic.QueryStringUSInt("ProcessCustomerID");

            if (ProcessCustomerID != 0 && OriginalRecurringOrderNumber != 0)
            {
                writer.Write("Processing Next Occurrence of Original Recurring Order # " + OriginalRecurringOrderNumber.ToString() + "...");
                RecurringOrderMgr rmgr = new RecurringOrderMgr(base.EntityHelpers, base.GetParser);
                writer.Write(rmgr.ProcessRecurringOrder(OriginalRecurringOrderNumber));
                writer.Write("...<br/>");
            }

            writer.Write("<br/>");

            writer.Write("<ul>");

            bool PendingOnly = (CommonLogic.QueryStringCanBeDangerousContent("Show").ToUpperInvariant() != "ALL");

            if (PendingOnly)
            {
                if (DB.GetSqlN("Select count(*) as N from ShoppingCart  " + DB.GetNoLock() + " where RecurringSubscriptionID='' and CartType=" + ((int)CartTypeEnum.RecurringCart).ToString() + " and NextRecurringShipDate<" + DB.SQuote(Localization.ToDBDateTimeString(System.DateTime.Now.AddDays(1)))) > 0)
                {
                    writer.Write("<li><b><a href=\"recurring.aspx?processall=true\">PROCESS CHARGES (RUN CREDIT CARDS AND CREATE ORDERS) FOR ALL SHIPMENTS DUE TODAY</a></b> (or process them one by one below)</li>");
                }
                else
                {
                    writer.Write("<li><b>NO RECURRING SHIPMENTS ARE DUE TODAY!</b></li>");
                }
            }
            else
            {
                if (DB.GetSqlN("Select count(*) as N from ShoppingCart  " + DB.GetNoLock() + " where RecurringSubscriptionID='' and CartType=" + ((int)CartTypeEnum.RecurringCart).ToString()) == 0)
                {
                    writer.Write("<li><b>NO ACTIVE RECURRING ORDERS FOUND.</b></li>");
                }
            }
            if (AppLogic.AppConfigBool("Recurring.UseGatewayInternalBilling"))
            {
                writer.Write("<li><b>NOTE: AUTOBILL GATEWAY ORDERS MAY STILL BE ACTIVE! See the 'Recurring Shipments - Import Status From Gateway' Menu Option under 'Orders'!</b></li>");
            }
            writer.Write("</ul>");



            String CustomerList = ",";
            String sql = "Select CustomerID,nextrecurringshipdate from ShoppingCart " + DB.GetNoLock() + " where RecurringSubscriptionID='' and CartType=" + ((int)CartTypeEnum.RecurringCart).ToString() + CommonLogic.IIF(PendingOnly, " and NextRecurringShipDate<" + DB.SQuote(Localization.ToDBShortDateString(System.DateTime.Now.AddDays(1))), "") + " order by nextrecurringshipdate desc";
            IDataReader rs = DB.GetRS(sql);
            while (rs.Read())
            {
                if (CustomerList.IndexOf("," + DB.RSFieldInt(rs, "CustomerID").ToString() + ",") == -1)
                {
                    Customer TargetCustomer = new Customer(DB.RSFieldInt(rs, "CustomerID"), true);
                    if (ShoppingCart.NumItems(TargetCustomer.CustomerID, CartTypeEnum.RecurringCart) != 0)
                    {
                        IDataReader rsr = DB.GetRS("Select distinct OriginalRecurringOrderNumber from ShoppingCart " + DB.GetNoLock() + " where RecurringSubscriptionID='' and CartType=" + ((int)CartTypeEnum.RecurringCart).ToString() + CommonLogic.IIF(PendingOnly, " and NextRecurringShipDate<" + DB.SQuote(Localization.ToDBShortDateString(System.DateTime.Now.AddDays(1))), "") + " and CustomerID=" + TargetCustomer.CustomerID.ToString() + " order by OriginalRecurringOrderNumber desc");
                        while (rsr.Read())
                        {
                            writer.Write(AppLogic.GetRecurringCart(base.EntityHelpers, base.GetParser, TargetCustomer, DB.RSFieldInt(rsr, "OriginalRecurringOrderNumber"), SkinID, false));
                        }
                        rsr.Close();
                    }
                }
                CustomerList += DB.RSFieldInt(rs, "CustomerID").ToString() + ",";
            }
            rs.Close();
        }

    }
}
