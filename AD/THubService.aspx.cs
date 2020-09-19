// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/THubService.aspx.cs 14    10/04/06 12:00p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Web.Util;
using System.Web.SessionState;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;
using System.Web.Mail;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Reflection;
using System.Security.Principal;
using AspDotNetStorefrontCommon;
using AspDotNetStorefrontGateways;

namespace AspDotNetStorefrontAdmin
{

    public class ThubService : Page
    {
        private string myConnectionString = "Provider=SQLOLEDB;" + DB.GetDBConn();
        private bool enableLogging = false; // You can change it to make logging on/off

        private void ShowError(string str, int code, Hashtable ht)
        {
            StringWriter streamw = new StringWriter();
            XmlTextWriter w = new XmlTextWriter(streamw);
            w.WriteStartDocument();
            w.WriteStartElement("RESPONSE");
            w.WriteAttributeString("version", "", "1.0");
            w.WriteStartElement("Envelope");

            AddXmlEl(w, "Command", ht["Command"].ToString());
            AddXmlEl(w, "StatusCode", code.ToString());
            AddXmlEl(w, "StatusMessage", str);
            AddXmlEl(w, "RequestID", ht["RequestID"].ToString());
            string provider = ht["Provider"] == null ? "" : ht["Provider"].ToString();
            AddXmlEl(w, "Provider", provider);

            w.WriteEndElement();
            w.WriteEndDocument();
            w.Close();
            Response.Clear();
            Response.ContentType = "text/xml";
            String tmpstr = streamw.ToString().Replace("encoding=\"utf-16\"?>", "encoding=\"utf-8\"?>");
            Response.Write(tmpstr);
            logging("Response " + '\r' + '\r' + tmpstr.Replace("><", ">" + '\r' + "<") + '\r' + '\r');
        }

        private void logging(string str)
        {
            if (enableLogging == true)
            {
                StreamWriter myWriter = null;
                string scriptname = Request.ServerVariables["PATH_TRANSLATED"].Replace("THubService.aspx", "");
                string filename = Localization.ToNativeDateTimeString(System.DateTime.Now);
                filename = filename.Replace(" ", "_");
                filename = filename.Replace(":", "_");
                filename = filename.Replace("/", "_");
                myWriter = File.CreateText(scriptname + "logs/" + filename + ".txt");
                myWriter.WriteLine("Request" + '\r' + '\r' + Request.Form["request"].Replace("><", ">" + '\r' + "<") + '\r' + '\r');
                myWriter.WriteLine(str);
                myWriter.Close();
            }
        }
        private void AddXmlEl(XmlTextWriter w, string name, string value)
        {
            w.WriteStartElement(name);
            w.WriteString(value);
            w.WriteEndElement();
        }
        private String AddZero(int a)
        {
            String tempAddZero = null;
            if (a < 10)
                tempAddZero = "0" + a;
            else
                tempAddZero = a.ToString();

            return tempAddZero;
        }


        //----------------------------------------------------------------------------------------------------------------------------------------
        //----------   GetOrders
        //----------------------------------------------------------------------------------------------------------------------------------------
        private void GetOrders(Hashtable ht, OleDbDataReader reader1)
        {
            StringWriter streamw = new StringWriter();
            XmlTextWriter w = new XmlTextWriter(streamw);

            w.Formatting = Formatting.Indented;
            w.Indentation = 5;
            w.WriteStartDocument();
            w.WriteStartElement("RESPONSE");
            w.WriteAttributeString("version", "", "1.0");
            w.WriteStartElement("Envelope");
            AddXmlEl(w, "Command", ht["Command"].ToString());
            AddXmlEl(w, "StatusCode", "0");
            AddXmlEl(w, "StatusMessage", "");
            AddXmlEl(w, "RequestID", ht["RequestID"].ToString());
            AddXmlEl(w, "Provider", "Value from Request");
            w.WriteEndElement(); //Envelop
            w.WriteStartElement("Orders");

            while (reader1.Read())
            {
                w.WriteStartElement("Order");
                AddXmlEl(w, "OrderID", reader1["OrderNumber"].ToString());
                AddXmlEl(w, "ProviderOrderRef", reader1["OrderNumber"].ToString());
                string dt = reader1["OrderDate"].ToString();
                AddXmlEl(w, "Date", System.Convert.ToDateTime(dt).Year + "-" + AddZero(System.Convert.ToDateTime(dt).Month) + "-" + AddZero(System.Convert.ToDateTime(dt).Day));
                AddXmlEl(w, "Time", AddZero(System.Convert.ToDateTime(dt).Hour) + ":" + AddZero(System.Convert.ToDateTime(dt).Minute) + ":" + AddZero(System.Convert.ToDateTime(dt).Second));
                AddXmlEl(w, "TimeZone", "");
                AddXmlEl(w, "StoreID", GetConfigValue("StoreName"));
                AddXmlEl(w, "StoreName", GetConfigValue("StoreName"));
                AddXmlEl(w, "Comment", HttpContext.Current.Server.HtmlEncode(reader1["OrderNotes"].ToString().Trim()));
                AddXmlEl(w, "Currency", GetConfigValue("Localization.StoreCurrency"));
                w.WriteStartElement("Bill");
                AddXmlEl(w, "PayMethod", reader1["PaymentMethod"].ToString().Trim());
                string PayStatus = "Cleared";
                if (reader1["TransactionState"].ToString() != AppLogic.ro_TXStateCaptured)
                {
                    PayStatus = "Pending";
                }
                AddXmlEl(w, "PayStatus", PayStatus);
                AddXmlEl(w, "FirstName", reader1["BillingFirstName"].ToString().Trim());
                AddXmlEl(w, "LastName", reader1["BillingLastName"].ToString().Trim());
                AddXmlEl(w, "MiddleName", "");
                AddXmlEl(w, "CompanyName", reader1["BillingCompany"].ToString().Trim());

                AddXmlEl(w, "Address1", reader1["BillingAddress1"].ToString().Trim());
                AddXmlEl(w, "Address2", (reader1["BillingAddress2"].ToString().Trim() + " " + reader1["BillingSuite"].ToString().Trim()).Trim());
                //AddXmlEl(w, "Suite", reader1["BillingSuite"].ToString().Trim());
                AddXmlEl(w, "City", reader1["BillingCity"].ToString().Trim());
                AddXmlEl(w, "State", reader1["BillingState"].ToString().Trim());
                AddXmlEl(w, "Zip", reader1["BillingZip"].ToString().Trim());
                AddXmlEl(w, "Country", reader1["BillingCountry"].ToString().Trim());
                AddXmlEl(w, "Email", reader1["Email"].ToString().Trim());
                AddXmlEl(w, "Phone", reader1["BillingPhone"].ToString().Trim());
                if (reader1["CardType"].ToString().Trim() != "" & reader1["CardExpirationYear"].ToString().Trim().Length != 0 & reader1["CardExpirationMonth"].ToString().Trim().Length != 0)
                {
                    //w.WriteStartElement(AppLogic.ro_PMCreditCard);
                    w.WriteStartElement("CreditCard");
                    AddXmlEl(w, "CreditCardType", reader1["CardType"].ToString().Trim());
                    AddXmlEl(w, "CreditCardCharge", "");
                    //AddXmlEl(w, "ExpirationDate", AddZero(reader1("CardExpirationYear")) & "-" & AddZero(reader1("CardExpirationMonth")) & "-" & "01")
                    //AddXmlEl(w, "ExpirationDate", reader1["CardExpirationYear"].ToString().Trim());
                    AddXmlEl(w, "ExpirationDate", reader1["CardExpirationMonth"].ToString().Trim() + "/" + reader1["CardExpirationYear"].ToString().Trim());

                    AddXmlEl(w, "CreditCardName", reader1["CardName"].ToString().Trim());

                    //
                    string ccNum = "";
                    ccNum = Security.UnmungeString(reader1["CardNumber"].ToString().Trim(), Order.StaticGetSaltKey(DB.RSFieldInt(reader1, "OrderNumber")));
                    if (ccNum.StartsWith(Security.ro_DecryptFailedPrefix, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ccNum = reader1["CardNumber"].ToString().Trim();
                    }

                    AddXmlEl(w, "CreditCardNumber", ccNum);
                    AddXmlEl(w, "AuthDetails", "AuthCode=" + reader1["AuthorizationCode"].ToString().Trim() + ";TransId=" + reader1["AuthorizationResult"].ToString().Trim() + ";AVSCode=" + reader1["AVSResult"].ToString().Trim()); //AuthCode=Q31234;TransId=4423412312;AVSCode=P
                    w.WriteEndElement(); //CreditCard
                }
                w.WriteEndElement(); //Bill
                w.WriteStartElement("Ship");

                //AddXmlEl(w, "ShipMethod", reader1["ShippingMethod"].ToString().Trim());
                string shipMethodDb = reader1["ShippingMethod"].ToString().Trim();
                string shipMethodParsed = "";

                string[] arInfo = new string[1];

                // define which character is seperating fields
                char[] splitter = { '|' };

                arInfo = shipMethodDb.Split(splitter);
                //if (arInfo.Length > 0) {
                shipMethodParsed = arInfo[0];
                //} else {
                //	shipMethodParsed = shipMethodDb;
                //}
                AddXmlEl(w, "ShipMethod", shipMethodParsed);


                AddXmlEl(w, "FirstName", reader1["ShippingFirstName"].ToString().Trim());
                AddXmlEl(w, "LastName", reader1["ShippingLastName"].ToString().Trim());
                AddXmlEl(w, "CompanyName", reader1["ShippingCompany"].ToString().Trim());
                AddXmlEl(w, "Address1", reader1["ShippingAddress1"].ToString().Trim());
                AddXmlEl(w, "Address2", reader1["ShippingAddress2"].ToString().Trim());
                AddXmlEl(w, "Address3", reader1["ShippingSuite"].ToString().Trim());
                AddXmlEl(w, "City", reader1["ShippingCity"].ToString().Trim());
                AddXmlEl(w, "State", reader1["ShippingState"].ToString().Trim());
                AddXmlEl(w, "Zip", reader1["ShippingZip"].ToString().Trim());
                AddXmlEl(w, "Country", reader1["ShippingCountry"].ToString().Trim());
                AddXmlEl(w, "Email", reader1["Email"].ToString().Trim());
                AddXmlEl(w, "Phone", reader1["ShippingPhone"].ToString().Trim());
                w.WriteEndElement();
                w.WriteStartElement("Charges");
                AddXmlEl(w, "Shipping", reader1["OrderShippingCosts"].ToString().Trim());
                AddXmlEl(w, "Handling", "");
                AddXmlEl(w, "Tax", reader1["OrderTax"].ToString().Trim());
                AddXmlEl(w, "Fee", "");
                AddXmlEl(w, "TaxOther", "0");
                w.WriteStartElement("FeeDetails");
                AddXmlEl(w, "FeeDetail", "");
                AddXmlEl(w, "FeeName", "");
                AddXmlEl(w, "FeeValue", "");
                w.WriteEndElement();
                AddXmlEl(w, "Discount", reader1["CouponDiscountAmount"].ToString().Trim());
                AddXmlEl(w, "DiscountPercent", reader1["CouponDiscountPercent"].ToString().Trim());
                AddXmlEl(w, "Total", reader1["OrderTotal"].ToString().Trim());
                w.WriteEndElement();

                if (reader1["CouponCode"].ToString().Trim() != "")
                {
                    w.WriteStartElement("Coupon");
                    AddXmlEl(w, "CouponCode", reader1["CouponCode"].ToString().Trim());
                    AddXmlEl(w, "CouponID", reader1["CouponCode"].ToString().Trim());
                    AddXmlEl(w, "CouponDescription", reader1["CouponDescription"].ToString().Trim());
                    AddXmlEl(w, "CouponValue", reader1["CouponDiscountAmount"].ToString().Trim());
                    AddXmlEl(w, "CouponPercent", reader1["CouponDiscountPercent"].ToString().Trim());
                    w.WriteEndElement();
                }
                w.WriteStartElement("Items");

                string myQuery2 = "" + "SELECT shc.ProductID, shc.OrderedProductSKU, shc.ShoppingCartRecID, shc.OrderedProductName AS OrderedProductName, shc.OrderedProductVariantName, shc.Quantity, shc.OrderedProductRegularPrice, shc.OrderedProductPrice, " + "shc.ChosenColor, shc.ChosenSize, shc.TextOption " + "FROM Orders_ShoppingCart shc " + "WHERE shc.OrderNumber = " + reader1["OrderNumber"];
                OleDbConnection myConnection2 = new OleDbConnection(myConnectionString);
                OleDbCommand myCommand2 = new OleDbCommand(myQuery2, myConnection2);
                OleDbDataReader reader2 = null;

                try
                {
                    myConnection2.Open();
                    reader2 = myCommand2.ExecuteReader();
                    while (reader2.Read())
                    {
                        w.WriteStartElement("Item");
                        AddXmlEl(w, "ItemCode", reader2["OrderedProductSKU"].ToString().Trim());
                        //AddXmlEl(w, "ItemDescription", reader2["OrderedProductName"].ToString().Trim());
                        string itemD = "";
                        itemD = reader2["OrderedProductName"].ToString().Trim();
                        if (reader2["OrderedProductVariantName"].ToString().Trim() != "")
                        {
                            itemD = itemD + "-" + reader2["OrderedProductVariantName"].ToString().Trim();
                        }
                        AddXmlEl(w, "ItemDescription", itemD);

                        AddXmlEl(w, "Quantity", reader2["Quantity"].ToString().Trim());
                        AddXmlEl(w, "UnitPrice", "");
                        AddXmlEl(w, "ItemTotal", reader2["OrderedProductPrice"].ToString().Trim());
                        AddXmlEl(w, "CustomField1", "");
                        AddXmlEl(w, "CustomField2", "");
                        AddXmlEl(w, "CustomField3", "");
                        AddXmlEl(w, "CustomField4", "");
                        AddXmlEl(w, "CustomField5", "");
                        w.WriteStartElement("ItemOptions");
                        w.WriteStartElement("ItemOption");
                        w.WriteAttributeString("Name", "", "ChosenColor");
                        w.WriteAttributeString("Value", "", reader2["ChosenColor"].ToString().Trim());
                        w.WriteEndElement();
                        w.WriteStartElement("ItemOption");
                        w.WriteAttributeString("Name", "", "ChosenSize");
                        w.WriteAttributeString("Value", "", reader2["ChosenSize"].ToString().Trim());
                        w.WriteEndElement();
                        string myQuery3 = "" + "select KitGroupName, KitItemName " + "from Orders_KitCart  " + "where OrderNumber = " + reader1["OrderNumber"] + " and ShoppingCartRecID=" + reader2["ShoppingCartRecID"];
                        OleDbConnection myConnection3 = new OleDbConnection(myConnectionString);
                        OleDbCommand myCommand3 = new OleDbCommand(myQuery3, myConnection3);
                        OleDbDataReader reader3 = null;

                        try
                        {
                            myConnection3.Open();
                            reader3 = myCommand3.ExecuteReader();

                            while (reader3.Read())
                            {
                                w.WriteStartElement("ItemOption");
                                w.WriteAttributeString("Name", "", "KitItem");
                                w.WriteAttributeString("Value", "", reader3["KitGroupName"].ToString().Trim() + "/" + reader3["KitItemName"].ToString().Trim());
                                w.WriteEndElement();
                                w.WriteStartElement("ItemOption");
                                w.WriteAttributeString("Name", "", "Size");
                                w.WriteAttributeString("Value", "", "");
                                w.WriteEndElement();
                            }
                            reader3.Close();
                        }
                        catch (Exception err3)
                        {
                            ShowError("Error reading Item Options." + err3.Message, 9999, ht);
                        }
                        finally
                        {
                            if (myConnection3 != null)
                                myConnection3.Close();
                        }

                        w.WriteEndElement(); //ItemOptions
                        w.WriteEndElement(); //Item

                        String myQuery4 = "" + "SELECT ProductSKU, ProductName, Quantity, ChosenColor, ChosenSize " + "FROM Orders_CustomCart  " + "WHERE OrderNumber = " + reader1["OrderNumber"] + " AND ShoppingCartRecID = " + reader2["ShoppingCartRecID"];
                        OleDbConnection myConnection4 = new OleDbConnection(myConnectionString);
                        OleDbDataReader reader4 = null;
                        OleDbCommand myCommand4 = new OleDbCommand(myQuery4, myConnection4);

                        try
                        {
                            myConnection4.Open();
                            reader4 = myCommand4.ExecuteReader();
                            //~ AddXmlEl(w, "DEBUG", "---------------------------------------------------------")

                            while (reader4.Read())
                            {
                                w.WriteStartElement("Item");
                                AddXmlEl(w, "ItemCode", reader4["ProductSKU"].ToString());
                                AddXmlEl(w, "ItemDescription", reader4["ProductName"].ToString().Trim());
                                AddXmlEl(w, "Quantity", reader4["Quantity"].ToString().Trim());
                                AddXmlEl(w, "UnitPrice", "0");
                                AddXmlEl(w, "ItemTotal", "0");
                                w.WriteStartElement("ItemOptions");
                                w.WriteStartElement("ItemOption");
                                w.WriteAttributeString("Name", "", "ChosenColor");
                                w.WriteAttributeString("Value", "", reader4["ChosenColor"].ToString().Trim());
                                w.WriteEndElement();
                                w.WriteStartElement("ItemOption");
                                w.WriteAttributeString("Name", "", "ChosenSize");
                                w.WriteAttributeString("Value", "", reader4["ChosenSize"].ToString().Trim());
                                w.WriteEndElement();
                                w.WriteEndElement(); //itemOption
                                w.WriteEndElement(); //item
                            }
                            reader4.Close();
                        }
                        catch (Exception err4)
                        {
                            ShowError("Error reading Items from Custom Cart." + err4.Message, 9999, ht);
                        }
                        finally
                        {
                            if (myConnection4 != null)
                                myConnection4.Close();
                        }


                    }
                    reader2.Close();

                }
                catch (Exception err2)
                {
                    ShowError("Error reading Items of order." + err2.Message + " -- " + myQuery2, 9999, ht);
                }
                finally
                {
                    if (myConnection2 != null)
                        myConnection2.Close();
                }
                w.WriteEndElement(); //Items
                w.WriteEndElement(); //Order

            }

            reader1.Close();
            w.WriteEndElement(); //Orders
            w.WriteEndElement(); //Respons
            w.WriteEndDocument();
            w.Close();
            Response.Clear();
            Response.ContentType = "text/xml";
            //      response.write (streamw.ToString.Replace ("encoding=""utf-16""?>","encoding=""utf-8""?>"))
            String tmpstr = streamw.ToString().Replace("encoding=\"utf-16\"?>", "encoding=\"utf-8\"?>");
            Response.Write(tmpstr);
            logging("Response " + '\r' + '\r' + tmpstr.Replace("><", ">" + '\r' + "<") + '\r' + '\r');
        }

        //{{{{{{{{{{{{{{{{{{{{{{{{{   UpdateOrders    {{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{
        private void UpdateOrders(Hashtable ht, string req)
        {
            StringReader stream1 = new StringReader(req);
            XmlTextReader r = new XmlTextReader(stream1);
            Hashtable myHT1 = new Hashtable();
            Hashtable hostOrderHT = new Hashtable();
            Hashtable LocalOrderIDHT = new Hashtable();
            Hashtable LocalOrderRefHT = new Hashtable();
            Hashtable resOfUpdate = new Hashtable();
            string curName = null;
            int index1 = 0;
            if (req == "")
                ShowError("Request is empty.", 9999, ht);

            // if ErrCode=0 then 
            // get variables from xml request
            index1 = -1;
            while (r.Read())
            {
                if (r.Name != "" & r.NodeType == XmlNodeType.Element)
                {
                    curName = r.Name.Trim();
                    r.Read();
                    if (r.NodeType == XmlNodeType.Text)
                        myHT1[curName] = r.Value.Trim();
                    if (r.NodeType == XmlNodeType.Text & curName.ToUpperInvariant() == "HOSTORDERID")
                    {
                        index1 += 1;
                        hostOrderHT[index1] = r.Value.Trim();
                    }
                    if (r.NodeType == XmlNodeType.Text & curName.ToUpperInvariant() == "LOCALORDERREF")
                        LocalOrderRefHT[index1] = r.Value.Trim();
                    if (r.NodeType == XmlNodeType.Text & curName.ToUpperInvariant() == "LOCALORDERID")
                        LocalOrderIDHT[index1] = r.Value.Trim();
                }
            }
            r.Close();

            int i = 0;
            int failCount = 0;
            failCount = 0;
            int tempFor1 = index1;
            for (i = 0; i <= tempFor1; i++)
            {
                string myQuery = "UPDATE Orders SET " + "THUB_POSTED_TO_ACCOUNTING = 'Y', thub_posted_date = getdate()" + ", thub_accounting_ref = '" + LocalOrderRefHT[i] + "' where OrderNumber=" + hostOrderHT[i];
                OleDbConnection myConnection = new OleDbConnection(myConnectionString);
                OleDbCommand myCommand = new OleDbCommand(myQuery, myConnection);
                try
                {
                    myConnection.Open();
                    resOfUpdate[i] = myCommand.ExecuteNonQuery();
                    if (System.Int32.Parse(resOfUpdate[i].ToString()) == 0)
                        failCount += 1;
                }
                catch
                {
                    resOfUpdate[i] = -1;
                    failCount += 1;
                }
                finally
                {
                    if (myConnection != null)
                        myConnection.Close();
                }
            }

            StringWriter streamw = new StringWriter();
            XmlTextWriter w = new XmlTextWriter(streamw);
            w.Formatting = Formatting.Indented;
            w.Indentation = 5;
            w.WriteStartDocument();
            w.WriteStartElement("RESPONSE");
            w.WriteAttributeString("version", "", "1.0");
            w.WriteStartElement("Envelope");
            AddXmlEl(w, "Command", ht["Command"].ToString());
            AddXmlEl(w, "StatusCode", "0");
            AddXmlEl(w, "StatusMessage", "Successful Count=" + (index1 - failCount + 1) + ";Failed Count=" + failCount);
            AddXmlEl(w, "RequestID", ht["RequestID"].ToString());
            string provider = ht["Provider"] == null ? "" : ht["Provider"].ToString();
            AddXmlEl(w, "Provider", provider);
            w.WriteEndElement();
            w.WriteStartElement("Orders");
            int tempFor2 = index1;
            for (i = 0; i <= tempFor2; i++)
            {
                w.WriteStartElement("Order");
                AddXmlEl(w, "HostOrderID", hostOrderHT[i].ToString());
                AddXmlEl(w, "LocalOrderID", LocalOrderIDHT[i].ToString());
                AddXmlEl(w, "LocalOrderRef", LocalOrderRefHT[i].ToString());
                if (System.Int32.Parse(resOfUpdate[i].ToString()) > 0)
                    AddXmlEl(w, "HostStatus", "Success");
                else
                    AddXmlEl(w, "HostStatus", "Fail");
                w.WriteEndElement();
            }
            w.WriteEndElement();
            w.WriteEndElement();
            w.WriteEndDocument();
            w.Close();
            Response.Clear();
            Response.ContentType = "text/xml";
            String tmpstr = streamw.ToString().Replace("encoding=\"utf-16\"?>", "encoding=\"utf-8\"?>");
            Response.Write(tmpstr);
            logging("Response " + '\r' + '\r' + tmpstr.Replace("><", ">" + '\r' + "<") + '\r' + '\r');

        } //Update Orders
        //-------------------------------------------------------------------------------------------------------------------------------

        //{{{{{{{{{{{{{{{{{{{{{{{{{   updateOrdersShippingStatus    {{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{
        private void updateOrdersShippingStatus(Hashtable ht, string req)
        {
            StringReader stream1 = new StringReader(req);
            XmlTextReader r = new XmlTextReader(stream1);
            Hashtable myHT1 = new Hashtable();
            Hashtable hostOrderHT = new Hashtable();
            Hashtable LocalOrderIDHT = new Hashtable();
            //~ Dim LocalOrderRefHT As New Hashtable()
            Hashtable ShippedOnHT = new Hashtable();
            Hashtable ShippedViaHT = new Hashtable();
            Hashtable TrackingNumberHT = new Hashtable();
            Hashtable resOfUpdate = new Hashtable();
            string curName = null;
            int index1 = 0;
            if (req == "")
                ShowError("Request is empty.", 9999, ht);

            // if ErrCode=0 then 
            // get variables from xml request
            index1 = -1;
            while (r.Read())
            {
                if (r.Name != "" & r.NodeType == XmlNodeType.Element)
                {
                    curName = r.Name.Trim();
                    r.Read();
                    if (r.NodeType == XmlNodeType.Text)
                        myHT1[curName] = r.Value.Trim();
                    if (r.NodeType == XmlNodeType.Text & curName.ToUpperInvariant() == "HOSTORDERID")
                    {
                        index1 += 1;
                        hostOrderHT[index1] = r.Value.Trim();
                    }
                    //~ If r.NodeType=XmlNodeType.Text and curName.ToUpper="LOCALORDERREF" then 
                    //~ LocalOrderRefHT(index1)=r.Value.Trim
                    //~ end If
                    if (r.NodeType == XmlNodeType.Text & curName.ToUpperInvariant() == "LOCALORDERID")
                        LocalOrderIDHT[index1] = r.Value.Trim();
                    if (r.NodeType == XmlNodeType.Text & curName.ToUpperInvariant() == "SHIPPEDON")
                        ShippedOnHT[index1] = r.Value.Trim();
                    if (r.NodeType == XmlNodeType.Text & curName.ToUpperInvariant() == "SHIPPEDVIA")
                        ShippedViaHT[index1] = r.Value.Trim();
                    if (r.NodeType == XmlNodeType.Text & curName.ToUpperInvariant() == "TRACKINGNUMBER")
                        TrackingNumberHT[index1] = r.Value.Trim();
                }
            }
            r.Close();

            int i = 0;
            int failCount = 0;
            failCount = 0;
            int tempFor1 = index1;
            for (i = 0; i <= tempFor1; i++)
            {
                string myQuery = "UPDATE Orders SET " + "ShippedOn = '" + ShippedOnHT[i] + "', ShippedVIA = '" + ShippedViaHT[i] + "', " + "ShippingTrackingNumber = '" + TrackingNumberHT[i] + "' where OrderNumber=" + hostOrderHT[i];
                OleDbConnection myConnection = new OleDbConnection(myConnectionString);
                OleDbCommand myCommand = new OleDbCommand(myQuery, myConnection);
                try
                {
                    myConnection.Open();
                    resOfUpdate[i] = myCommand.ExecuteNonQuery();
                    if (System.Int32.Parse(resOfUpdate[i].ToString()) == 0)
                        failCount += 1;
                }
                catch
                {
                    resOfUpdate[i] = -1;
                    failCount += 1;
                }
                finally
                {
                    if (myConnection != null)
                        myConnection.Close();
                }
            }

            StringWriter streamw = new StringWriter();
            XmlTextWriter w = new XmlTextWriter(streamw);
            w.Formatting = Formatting.Indented;
            w.Indentation = 5;
            w.WriteStartDocument();
            w.WriteStartElement("RESPONSE");
            w.WriteAttributeString("version", "", "1.0");
            w.WriteStartElement("Envelope");
            AddXmlEl(w, "Command", ht["Command"].ToString());
            AddXmlEl(w, "StatusCode", "0");
            AddXmlEl(w, "StatusMessage", "Successful Count=" + (index1 - failCount + 1) + ";Failed Count=" + failCount);
            AddXmlEl(w, "RequestID", ht["RequestID"].ToString());
            //~ AddXmlEl(w,"Provider",ht("Provider"))
            w.WriteEndElement();
            w.WriteStartElement("Orders");
            int tempFor2 = index1;
            for (i = 0; i <= tempFor2; i++)
            {
                w.WriteStartElement("Order");
                AddXmlEl(w, "HostOrderID", hostOrderHT[i].ToString());
                AddXmlEl(w, "LocalOrderID", (LocalOrderIDHT[i] == null) ? "" : LocalOrderIDHT[i].ToString());
                //AddXmlEl(w,"resOfUpdate",resOfUpdate(i))
                if (System.Int32.Parse(resOfUpdate[i].ToString()) > 0)
                    AddXmlEl(w, "HostStatus", "Success");
                else
                    AddXmlEl(w, "HostStatus", "Fail");
                w.WriteEndElement();
            }
            w.WriteEndElement();
            w.WriteEndElement();
            w.WriteEndDocument();
            w.Close();
            Response.Clear();
            Response.ContentType = "text/xml";
            String tmpstr = streamw.ToString().Replace("encoding=\"utf-16\"?>", "encoding=\"utf-8\"?>");
            Response.Write(tmpstr);
            logging("Response " + '\r' + '\r' + tmpstr.Replace("><", ">" + '\r' + "<") + '\r' + '\r');

        } //updateOrdersShippingStatus
        //-------------------------------------------------------------------------------------------------------------------------------

        //{{{{{{{{{{{{{{{{{{{{{{{{{   updateOrdersPaymentStatus    {{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{
        private void updateOrdersPaymentStatus(Hashtable ht, string req)
        {
            StringReader stream1 = new StringReader(req);
            XmlTextReader r = new XmlTextReader(stream1);
            Hashtable myHT1 = new Hashtable();
            Hashtable hostOrderHT = new Hashtable();
            Hashtable LocalOrderIDHT = new Hashtable();
            Hashtable PaymentStatusHT = new Hashtable();
            Hashtable ClearedOnHT = new Hashtable();
            Hashtable resOfUpdate = new Hashtable();
            string curName = null;
            int index1 = 0;
            if (req == "")
                ShowError("Request is empty.", 9999, ht);

            // if ErrCode=0 then 
            // get variables from xml request
            index1 = -1;
            while (r.Read())
            {
                if (r.Name != "" & r.NodeType == XmlNodeType.Element)
                {
                    curName = r.Name.Trim();
                    r.Read();
                    if (r.NodeType == XmlNodeType.Text)
                        myHT1[curName] = r.Value.Trim();
                    if (r.NodeType == XmlNodeType.Text & curName.ToUpperInvariant() == "HOSTORDERID")
                    {
                        index1 += 1;
                        hostOrderHT[index1] = r.Value.Trim();
                    }
                    if (r.NodeType == XmlNodeType.Text & curName.ToUpperInvariant() == "LOCALORDERID")
                        LocalOrderIDHT[index1] = r.Value.Trim();
                    if (r.NodeType == XmlNodeType.Text & curName.ToUpperInvariant() == "CLEAREDON")
                        ClearedOnHT[index1] = r.Value.Trim();
                    if (r.NodeType == XmlNodeType.Text & curName.ToUpperInvariant() == "PAYMENTSTATUS")
                        PaymentStatusHT[index1] = r.Value.Trim().ToUpperInvariant();
                }
            }
            r.Close();

            StringWriter streamw = new StringWriter();
            XmlTextWriter w = new XmlTextWriter(streamw);
            w.Formatting = Formatting.Indented;
            w.Indentation = 5;
            w.WriteStartDocument();
            w.WriteStartElement("RESPONSE");
            w.WriteAttributeString("version", "", "1.0");
            w.WriteStartElement("Envelope");

            int i = 0;
            int failCount = 0;
            failCount = 0;
            int tempFor1 = index1;
            for (i = 0; i <= tempFor1; i++)
            {
                string myQuery = String.Empty;
                if (PaymentStatusHT[i].ToString() == "CLEARED")
                {
                    Gateway.ProcessOrderAsCaptured(System.Int32.Parse(hostOrderHT[i].ToString()));
                }
                if (PaymentStatusHT[i].ToString() == "DECLINED")
                {
                    myQuery = "UPDATE Orders SET TransactionState=" + DB.SQuote(AppLogic.ro_TXStateVoided) + " where OrderNumber=" + hostOrderHT[i];
                }
                if (PaymentStatusHT[i].ToString() == "FAILED")
                {
                    myQuery = "UPDATE Orders SET TransactionState=" + DB.SQuote(AppLogic.ro_TXStateVoided) + " where OrderNumber=" + hostOrderHT[i];
                }
                OleDbConnection myConnection = new OleDbConnection(myConnectionString);
                OleDbCommand myCommand = new OleDbCommand(myQuery, myConnection);
                //~ AddXmlEl(w,"Query",myQuery)
                try
                {
                    myConnection.Open();
                    resOfUpdate[i] = myCommand.ExecuteNonQuery();
                    if (System.Int32.Parse(resOfUpdate[i].ToString()) == 0)
                        failCount += 1;
                }
                catch
                {
                    resOfUpdate[i] = -1;
                    failCount += 1;
                    //~ AddXmlEl(w,"Error",err.Message)
                }
                finally
                {
                    if (myConnection != null)
                        myConnection.Close();
                }
            }

            AddXmlEl(w, "Command", ht["Command"].ToString());
            AddXmlEl(w, "StatusCode", "0");
            AddXmlEl(w, "StatusMessage", "Successful Count=" + (index1 - failCount + 1) + ";Failed Count=" + failCount);
            AddXmlEl(w, "RequestID", ht["RequestID"].ToString());
            w.WriteEndElement();
            w.WriteStartElement("Orders");
            int tempFor2 = index1;
            for (i = 0; i <= tempFor2; i++)
            {
                w.WriteStartElement("Order");
                AddXmlEl(w, "HostOrderID", hostOrderHT[i].ToString());
                AddXmlEl(w, "LocalOrderID", LocalOrderIDHT[i].ToString());
                if (System.Int32.Parse(resOfUpdate[i].ToString()) > 0)
                    AddXmlEl(w, "HostStatus", "Success");
                else
                    AddXmlEl(w, "HostStatus", "Fail");
                w.WriteEndElement();
            }
            w.WriteEndElement();
            w.WriteEndElement();
            w.WriteEndDocument();
            w.Close();
            Response.Clear();
            Response.ContentType = "text/xml";
            String tmpstr = streamw.ToString().Replace("encoding=\"utf-16\"?>", "encoding=\"utf-8\"?>");
            Response.Write(tmpstr);
            logging("Response " + '\r' + '\r' + tmpstr.Replace("><", ">" + '\r' + "<") + '\r' + '\r');

        } //updateOrdersPaymentStatus
        //-------------------------------------------------------------------------------------------------------------------------------



        private void parsexml()
        {
            string curName = null;
            string tmp = null;
            int ErrCode = 0;
            tmp = Request.Form["request"];
            tmp = tmp.Replace("><", "> <");
            ErrCode = 0;
            if (tmp == "")
                ErrCode = 1;
            StringReader stream1 = new StringReader(tmp);
            XmlTextReader r = new XmlTextReader(stream1);
            Hashtable myHT = new Hashtable();
            if (tmp == "")
                ShowError("Request is empty.", 9999, myHT);
            if (ErrCode == 0)
            {
                while (r.Read())
                {
                    if (r.Name != "" & r.NodeType == XmlNodeType.Element)
                        curName = r.Name.Trim();
                    r.Read();
                    if (r.NodeType == XmlNodeType.Text)
                        myHT[curName] = r.Value.Trim();
                }
                r.Close();
            }

            Customer c = new Customer(myHT["UserID"].ToString());
            Password pwd = new Password(myHT["Password"].ToString(), c.SaltKey);

            string SaltedAndHashedPassword = pwd.SaltedPassword;
            string myQuery3 = "Select Count(*) as count1 FROM Customer where Deleted=0 and (IsAdmin&1 > 0 or IsAdmin&2>0) and EMail=" + DB.SQuote(myHT["UserID"].ToString()) + " and Password = " + DB.SQuote(SaltedAndHashedPassword);
            OleDbConnection myConnection5 = new OleDbConnection(myConnectionString);
            OleDbCommand myCommand5 = new OleDbCommand(myQuery3, myConnection5);
            OleDbDataReader reader5 = null;
            try
            {
                myConnection5.Open();
                reader5 = myCommand5.ExecuteReader();
                reader5.Read();
                if (System.Int32.Parse(reader5["count1"].ToString()) == 0)
                {
                    ErrCode = 9000;
                    ShowError("Login failed.", ErrCode, myHT);
                }
                reader5.Close();
            }
            catch (Exception err)
            {
                ShowError("Error. Auth Code. " + err.Message + " -- " + myQuery3 + " -- ", 9999, myHT);
            }
            finally
            {
                if (myConnection5 != null)
                    myConnection5.Close();
            }

            // making sql request for Get Orders

            if (ErrCode == 0)
            {
                string LOC = (myHT["LimitOrderCount"] == null) ? "" : myHT["LimitOrderCount"].ToString();
                if (LOC == "")
                    myHT["LimitOrderCount"] = "25";
                switch (myHT["Command"].ToString().ToUpperInvariant())
                {
                    case "GETORDERS":
                        OleDbConnection myConnection = new OleDbConnection(myConnectionString);
                        string exclList = null;
                        string exclord = (myHT["exclude-orders"] == null) ? "" : myHT["exclude-orders"].ToString();
                        if (exclord != "")
                        {
                            //exclList = " and "
                            string[] excl = exclord.Split(',');
                            int k = 0;
                            int tempFor1 = excl.Length;
                            for (k = 0; k < tempFor1; k++)
                            {
                                exclList = exclList + " o.OrderNumber<>" + excl[k] + " and ";
                            }
                        }

                        string myQuery = "" + "select TOP " + myHT["LimitOrderCount"] + "  o.OrderNumber, o.OrderGUID, o.OrderDate, " + "o.SkinID as StoreID,  o.OrderNotes, " + "o.PaymentMethod, o.Email," + "o.BillingFirstName, o.BillingLastName, " + "o.BillingAddress1, o.BillingAddress2, " + "o.TransactionState, " + "o.AVSResult, o.AuthorizationCode, o.AuthorizationResult," + "o.BillingCity, o.BillingState, o.BillingZip, o.BillingCountry, o.BillingPhone, o.BillingSuite, o.BillingCompany, " + "o.CardType, o.CardExpirationMonth, o.CardExpirationYear, o.CardName, o.CardNumber, " + "o.ShippingMethod, o.ShippingFirstName, o.ShippingLastName, o.ShippingAddress1, " + "o.ShippingAddress2, o.ShippingCity, o.ShippingState, o.ShippingZip, o.ShippingCountry, o.ShippedVIA, " + "o.ShippingPhone, o.ShippingCompany, o.ShippingSuite, o.ShippingPhone, o.CouponCode, o.CouponDescription, o.CouponDiscountAmount, o.CouponDiscountPercent,  " + "o.OrderShippingCosts, o.OrderTax, " + "o.OrderTotal " + "from Orders o (NOLOCK)" + "WHERE " + exclList + " o.THUB_POSTED_TO_ACCOUNTING = 'N'  ORDER BY o.OrderNumber";

                        //"where o.SkinID = " & myHT("UserID") & _
                        OleDbCommand myCommand = new OleDbCommand(myQuery, myConnection);
                        OleDbDataReader reader = null;
                        try
                        {
                            myConnection.Open();
                            reader = myCommand.ExecuteReader();
                            GetOrders(myHT, reader);
                            reader.Close();
                        }
                        catch (Exception err)
                        {
                            ShowError("Error Creating Response. " + err.Message + " -- " + myQuery, 9999, myHT);
                        }
                        finally
                        {
                            if (myConnection != null)
                                myConnection.Close();
                        }

                        break;
                    case "UPDATEORDERS":
                        UpdateOrders(myHT, tmp);

                        break;
                    case "UPDATEORDERSSHIPPINGSTATUS":
                        updateOrdersShippingStatus(myHT, tmp);
                        break;
                    case "UPDATEORDERSPAYMENTSTATUS":
                        updateOrdersPaymentStatus(myHT, tmp);
                        break;
                    default:
                        ShowError("Wrong Command value: '" + myHT["Command"] + "'", -1, myHT);
                        ErrCode = -1;
                        break;
                }

            }
        }

        //--------------------------------------------
        private String GetConfigValue(string valueName)
        {
            String tempGetConfigValue = null;
            Hashtable myHT9 = new Hashtable();
            OleDbConnection myConnection9 = new OleDbConnection(myConnectionString);
            string myQuery9 = "select ConfigValue FROM AppConfig WHERE Name = '" + valueName + "'";
            OleDbCommand myCommand9 = new OleDbCommand(myQuery9, myConnection9);
            OleDbDataReader reader9 = null;
            try
            {
                myConnection9.Open();
                reader9 = myCommand9.ExecuteReader();
                reader9.Read();
                tempGetConfigValue = reader9["ConfigValue"].ToString();
                reader9.Close();
            }
            catch (Exception err9)
            {
                ShowError("Error GetConfigValue. " + err9.Message + " -- " + myQuery9, 9999, myHT9);
            }
            finally
            {
                if (myConnection9 != null)
                    myConnection9.Close();
            }
            return tempGetConfigValue;
        }
        //____________________________________________

        override protected void OnInit(EventArgs e)
        {
            this.Load += new System.EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        private void Page_Load(object Sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            parsexml();
        }

    }
}
