// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// $Header: /v6.2/Web/Admin/setlast4.aspx.cs 7     9/05/06 2:08p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.IO;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for setlast4
    /// </summary>
    public partial class setlast4 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
            Server.ScriptTimeout = 30000;

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
            if (!ThisCustomer.IsAdminSuperUser)
            {
                throw new ArgumentException("Permission Denied, Must Be SuperAdmin");
            }

            Response.Write("<html><head><title>Setting Last4 Field on All Address & Order Records Where Possible</title></head><body>");

            Response.Write("Updating Last4 fields of all order records...<br/>");
            Response.Flush();

            IDataReader rs = DB.GetRS("select OrderNumber, CardNumber from orders " + DB.GetNoLock() + " where CardNumber IS NOT NULL and (Last4 IS NULL or Last4='') order by ordernumber");
            while (rs.Read())
            {
                int ONX = DB.RSFieldInt(rs, "OrderNumber");

                Response.Write("Checking order " + ONX.ToString() + "...");
                Response.Flush();

                String CardNumber = AppLogic.AdminViewCardNumber(DB.RSField(rs, "CardNumber"), "Orders", DB.RSFieldInt(rs,"OrderNumber"));
                if (CardNumber.Length != 0)
                {
                    String Last4 = AppLogic.SafeDisplayCardNumberLast4(CardNumber, "Orders", DB.RSFieldInt(rs,"OrderNumber"));
                    if (Last4.Length != 0)
                    {
                        DB.ExecuteSQL("update orders set Last4=" + DB.SQuote(Last4) + " where OrderNumber=" + ONX.ToString() + "");
                        Response.Write("update orders set Last4=" + DB.SQuote(Last4) + " where OrderNumber=" + ONX.ToString());
                    }
                    else
                    {
                        Response.Write("Last4 could not be set");
                    }
                }
                else
                {
                    Response.Write("CardNumber not found");
                }
                Response.Write("<br/>");
                Response.Flush();
            }
            rs.Close();

            Response.Write("done<br/>");

            Response.Write("</body></html>");
        }

    }
}
