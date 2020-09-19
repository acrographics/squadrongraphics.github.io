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
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    public partial class viewshipment : AspDotNetStorefront.SkinBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sql = "SELECT o.OrderNumber, o.ShippingTrackingNumber, o.ShippedOn, o.CustomerID, o.FirstName + ' ' + o.LastName AS Name, o.Email, o.ShippingFirstName + ' ' + o.ShippingLastName AS ShippingName, o.ShippingCompany, o.ShippingAddress1, o.ShippingCity, o.ShippingState, o.ShippingZip, o.ShippingCountry, o.ShippingPhone, o.OrderSubtotal, o.OrderTax, o.OrderShippingCosts, o.OrderTotal, o.OrderDate, o.OrderWeight  ";
            sql += "FROM Orders o left join ( select ordernumber, count(distinct shippingaddressid) addresses from orders_shoppingcart group by ordernumber having count(distinct shippingaddressid) > 1) a on o.ordernumber = a.ordernumber ";
            sql += "WHERE ReadyToShip = 1 AND ShippedOn IS NOT NULL and TransactionState in (" + DB.SQuote(AppLogic.ro_TXStateAuthorized) + "," + DB.SQuote(AppLogic.ro_TXStateCaptured) + ") and a.ordernumber is null";
            DataSet ds = DB.GetDS(sql, false);
            BindDataView(ds);
            ds.Dispose();
        }

        protected void BindDataView(DataSet ds)
        {
            dview.DataSource = ds;
            dview.DataBind();
        }

        private DataTable BuildDataTable(string fileFullPath, char seperator)
        {
            DataTable myTable = new DataTable("MyTable");
            int i;
            System.Data.DataRow myRow;
            String[] fieldValues;
            StreamReader myReader;

            try
            {
                myReader = File.OpenText(fileFullPath);
                fieldValues = myReader.ReadLine().Split(seperator);

                for (i = 0; i <= fieldValues.Length - 1; i++)
                {
                    myTable.Columns.Add(new DataColumn(fieldValues[i].ToString()));
                }
                myRow = myTable.NewRow();
                String temp = String.Empty;

                while (myReader.Read() != -1)
                {
                    fieldValues = myReader.ReadLine().Split(seperator);
                    myRow = myTable.NewRow();
                    for (i = 0; i <= fieldValues.Length - 1; i++)
                    {
                        temp = fieldValues[i].Replace("\"", "");
                        myRow[i] = temp;
                    }
                    myTable.Rows.Add(myRow);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            myReader.Close();
            myReader.Dispose();
            return myTable;
        }

    }
}