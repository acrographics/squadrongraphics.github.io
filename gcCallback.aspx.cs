// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Text;
using System.Web;
using System.Xml;
using AspDotNetStorefrontCommon;
using AspDotNetStorefrontGateways;

namespace AspDotNetStorefront
{
    public partial class gcCallback : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.ContentType = "application/xml";
            //Response.ContentEncoding = Encoding.UTF8;

            if (CommonLogic.QueryStringCanBeDangerousContent("loadcheck") == "1")
            {
                Response.Write("<loadcheck>" + System.DateTime.Now.ToString() + "</loadcheck>");
            }
            else
            {
                // this callback requires basic authentication
                if (!GoogleCheckout.VerifyMessageAuthentication(Request.Headers["Authorization"]) && !AppLogic.AppConfigBool("GoogleCheckout.UseSandbox"))
                {
                    Response.StatusCode = 401;
                    Response.StatusDescription = "Access Denied";
                }

                else if (Request.ContentLength > 0)
                {
                    // place notification into string
                    string xmlData = Encoding.ASCII.GetString(Request.BinaryRead(Request.ContentLength));

                    //  Select the appropriate function to handle the notification
                    //  by evaluating the root tag of the document
                    XmlDocument googleResponse = new XmlDocument();
                    googleResponse.LoadXml(xmlData);
                    //DB.ExecuteSQL("insert sqlLog(sqltext) values(" + DB.SQuote(xmlData) + ")");
                    int MerchantOrderNumber = 0;

                    switch (googleResponse.DocumentElement.Name)
                    {
                        case "merchant-calculation-callback":
                            Response.Write(GoogleCheckout.createMerchantCalculationResults(xmlData));
                            break;
                        case "new-order-notification":
                            sendNotificationAcknowledgment();
                            int ordernumber = GoogleCheckout.processNewOrderNotification(xmlData);
                            GoogleCheckout.AddMerchantOrderNumber(ordernumber);
                            break;
                        case "order-state-change-notification":
                            sendNotificationAcknowledgment();
                            GoogleCheckout.ProcessOrderStateChangeNotification(xmlData);
                            break;
                        case "risk-information-notification":
                            sendNotificationAcknowledgment();
                            MerchantOrderNumber = GoogleCheckout.processRiskInformationNotification(xmlData);
                            break;
                        case "charge-amount-notification":
                            sendNotificationAcknowledgment();
                            GoogleCheckout.processChargeAmountNotification(xmlData);
                            break;
                        case "refund-amount-notification":
                            sendNotificationAcknowledgment();
                            GoogleCheckout.processRefundAmountNotification(xmlData);
                            break;
                        case "chargeback-amount-notification":
                            sendNotificationAcknowledgment();
                            GoogleCheckout.processChargebackAmountNotification(xmlData);
                            break;
                        case "request-received":
                            GoogleCheckout.processRequestReceived(xmlData);
                            break;
                        case "error":
                            GoogleCheckout.processErrorNotification(xmlData);
                            break;
                        case "diagnosis":
                            GoogleCheckout.processDiagnosisNotification(xmlData);
                            break;
                        default:
                            GoogleCheckout.processUnknownNotification(xmlData);
                            break;
                    }
                }
            }
        }

        private void sendNotificationAcknowledgment()
        {
            Response.Write(GoogleCheckout.sendNotificationAcknowledgment());
            Response.Flush();
        }
    }
}
