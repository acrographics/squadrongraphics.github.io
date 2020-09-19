// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/cst_export.aspx.cs 6     9/13/06 10:47p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Text;
using System.Web;
using System.Xml;
using System.Data;
using System.Xml.Serialization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
	/// <summary>
	/// Summary description for cst_export.
	/// </summary>
	public partial class cst_export : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
			if(ThisCustomer.IsAdminUser)
			{
				Response.Expires = -1;
				Response.ContentType = "text/xml";
				// Create a new XmlTextWriter instance
				XmlTextWriter writer = new XmlTextWriter(Response.OutputStream, Encoding.UTF8);
    
				// start writing!
				writer.WriteStartDocument();
				writer.WriteStartElement("CustomerList");
				string SuperuserFilter = CommonLogic.IIF(ThisCustomer.IsAdminSuperUser , String.Empty , String.Format(" Customer.CustomerID not in ({0}) and ",AppLogic.AppConfig("Admin_Superuser")));

				IDataReader rs = DB.GetRS("select * from customer  " + DB.GetNoLock() + " where " + SuperuserFilter.ToString() + " deleted=0 and EMail<> '' order by createdon desc");
				while(rs.Read())
				{
					writer.WriteStartElement("Customer");
					writer.WriteElementString("FirstName", DB.RSField(rs,"FirstName"));
					writer.WriteElementString("LastName", DB.RSField(rs,"LastName"));
					writer.WriteElementString("EMail", DB.RSField(rs,"EMail"));
					writer.WriteElementString("OKToEMail", DB.RSFieldBool(rs,"OKToEMail").ToString());
					writer.WriteElementString("CreatedOn", Localization.ToNativeDateTimeString(DB.RSFieldDateTime(rs,"CreatedOn")));
					writer.WriteElementString("RegisteredOn", Localization.ToNativeDateTimeString(DB.RSFieldDateTime(rs,"RegisteredOn")));
					writer.WriteEndElement();
				}
				rs.Close();
	    
				writer.WriteEndElement();
				writer.WriteEndDocument();
				writer.Close();    
			}
			else
			{
				Response.Expires = -1;
				Response.Write("<html><body>");
				Response.Write("Insufficient Privilege");
				Response.Write("</body></html>");
			}
		}

	}
}
