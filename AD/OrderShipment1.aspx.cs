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
    public partial class OrderShipment1 : AspDotNetStorefront.SkinBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            SectionTitle = "Export Shipment";
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {

            writer.Write("<script type=\"text/javascript\">\n");
            writer.Write("function ViewShipmentInformation()\n");
            writer.Write("{\n");
            writer.Write("window.open('viewshipment.aspx', '', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,copyhistory=no,width=600,height=500,left=0,top=0');\n");
            writer.Write("}\n");
            writer.Write("</SCRIPT>\n");


            writer.Write("<form enctype=\"multipart/form-data\" method=\"post\" action=\"OrderShipment2.aspx\">");
            writer.Write("<p><b>Shipping Label Program:</b></p>");
            writer.Write("<p><input type=\"radio\" name=\"exporttype\" value=\"UPS WorldShip\" checked>UPS WorldShip</p>");
            //writer.Write("<p><input type=\"radio\" name=\"exporttype\" value=\"FedEx\">FedEx Shipping Manager</p>");
            //writer.Write("<p><input type=\"radio\" name=\"exporttype\" value=\"USPS\">USPS (Endicia)</p>");

            string sql = "select count(*) as N  ";
            sql += "FROM Orders o left join ( select ordernumber, count(distinct shippingaddressid) addresses from orders_shoppingcart group by ordernumber having count(distinct shippingaddressid) > 1) a on o.ordernumber = a.ordernumber ";
            sql += "WHERE ReadyToShip = 1 AND ShippedOn IS NULL and TransactionState in (" + DB.SQuote(AppLogic.ro_TXStateAuthorized) + "," + DB.SQuote(AppLogic.ro_TXStateCaptured) + ")"; //and a.ordernumber is null";
            int NumOrdersReadyToExport = DB.GetSqlN(sql);
            if (NumOrdersReadyToExport == 0)
            {
                writer.Write("<p><b>Exporting Your Orders</b></p>");
                writer.Write("<p><b><font color=\"red\">There is nothing to export! There are no orders that are " + AppLogic.ro_TXStateAuthorized + " or " + AppLogic.ro_TXStateCaptured + " and which are marked as Ready To Ship and which have not already been Shipped! You should mark the orders as ready to ship in the order management screen before running this export page.</font></b></p>");
            }
            else
            {
                writer.Write("<p><b>Exporting Your Orders</b></p>");
                writer.Write("<p>There are <b>" + NumOrdersReadyToExport.ToString() + "</b> order(s) ready to ship!</p>");
                writer.Write("<p>Click the Export Orders button below to export these orders to your selected shipping label program. You will be prompted to save the export file on your local PC and then use your shipping label program to process it. When completed, you will then import the shipping tracking number information back into the storefront using the Import Tracking Numbers section below.</p>");
                writer.Write("<p><input name=\"state\" type=\"submit\" value=\"Export Orders\">");
                //writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"button\" value=\"Preview Export In Browser\" name=\"ExportShipment\" onClick=\"ViewShipmentInformation()\">");
                writer.Write("</p>");
                writer.Write("<p></p>");
            }
            writer.Write("<p><b>Importing Tracking Numbers</b></p>");
            writer.Write("<p>Select the shipping tracking information file to Import. You use this to import the shipping tracking numbers after you have printed the labels for the orders.<br/><input type=\"file\" name=\"pricingimport\" accept=\"text/csv\" size=\"60\"></p>");
            writer.Write("<p><input name=\"state\" type=\"submit\" value=\"Import Tracking Numbers\"></p>");
            writer.Write("</form>");

            writer.Write("<p><b><font color=\"blue\">NOTE: Multiple-Ship-to orders are NOT supported for exporting automation at this time! You must process any multiple-ship-to orders manually, and update their shipping status in the order management page for the order.</font></b></p>");

        }
    }
}