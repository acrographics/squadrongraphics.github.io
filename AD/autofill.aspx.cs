// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/autofill.aspx.cs 4     9/30/06 3:39p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.IO;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for autofill.
    /// </summary>
    public partial class autofill : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;
            String FormImageName = CommonLogic.QueryStringCanBeDangerousContent("FormImageName");

            if (ThisCustomer.IsAdminUser)
            {
                int ProductID = CommonLogic.QueryStringUSInt("ProductID");
                Response.Write("<html><head><title>AutoFill Variants</title></head><body>\n");

                if (ProductID != 0)
                {
                    String MiscText = String.Empty;
                    IDataReader rs = DB.GetRS("Select MiscText from product  " + DB.GetNoLock() + " where ProductID=" + ProductID.ToString());
                    if (rs.Read())
                    {
                        MiscText = DB.RSField(rs, "MiscText");
                    }
                    rs.Close();
                    if (MiscText.Length != 0)
                    {
                        // MiscText must be in format: name skusuffix price cols
                        String MiscText2 = MiscText.Replace("\n", "|").Replace("\r", "|").Replace("||", "|");
                        foreach (String s in MiscText2.Split('|'))
                        {
                            try
                            {
                                String stmp = s.Trim();
                                while (stmp.IndexOf("  ") != -1)
                                {
                                    stmp = stmp.Replace("  ", " ");
                                }
                                stmp = stmp.Trim();
                                if (stmp.Length != 0)
                                {
                                    String[] sarray = stmp.Split(' ');
                                    String Price = sarray[sarray.GetUpperBound(0)];
                                    String SKUSuffix = sarray[sarray.GetUpperBound(0) - 1];
                                    String Name = String.Empty;
                                    for (int i = 0; i <= sarray.GetUpperBound(0) - 2; i++)
                                    {
                                        Name += sarray[i] + " ";
                                    }
                                    Name = Name.Trim();
                                    if (Price.Length != 0 && Name.Length != 0)
                                    {
                                        String sql = "insert into productvariant(ProductID,Name,SKUSuffix,Price,Inventory,Published) values(" + ProductID.ToString() + "," + DB.SQuote(Name) + "," + DB.SQuote(SKUSuffix) + "," + Price + ",1000000,1)";
                                        Response.Write("Executing: " + sql + "<br/>");
                                        DB.ExecuteSQL(sql);
                                    }
                                    else
                                    {
                                        Response.Write("<p><b>Bad Line Format: " + s + "</b></p>");
                                    }
                                }
                            }
                            catch
                            {
                                Response.Write("<p><b>Error On Line: " + s + ", SKIPPING LINE</b></p>");
                            }
                        }
                    }
                    else
                    {
                        Response.Write("<p><b>Product MiscText Is Empty</b></p>");
                    }
                }
                else
                {
                    Response.Write("<p><b>No Product ID Specified</b></p>");
                }
            }

            Response.Write("<a href=\"javascript:self.close();\">Close</a>");
            Response.Write("</body></html>\n");
        }
    }
}
