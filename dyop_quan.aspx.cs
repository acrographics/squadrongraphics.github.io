// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/dyop_quan.aspx.cs 5     8/19/06 8:46p Buddy $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary description for dyop_quan.
    /// </summary>
    public partial class dyop_quan : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((AspDotNetStorefrontPrincipal)Context.User).ThisCustomer;

            int PackID = CommonLogic.QueryStringUSInt("PackID");
            int ProductID = CommonLogic.QueryStringUSInt("ProductID");
            int CategoryID = CommonLogic.QueryStringUSInt("CategoryID");
            int SectionID = CommonLogic.QueryStringUSInt("SectionID");
            int Quan = CommonLogic.QueryStringUSInt("Quan");
            int CustomCartRecID = CommonLogic.QueryStringUSInt("ccartID");
            int ShoppingCartRecID = CommonLogic.QueryStringUSInt("scartID");

            if (Quan != 0 && CustomCartRecID != 0)
            {
                if (Quan > 0)
                {
                    DB.ExecuteSQL("update CustomCart set quantity=quantity+1 where ShoppingCartRecID=" + ShoppingCartRecID.ToString() + " and CustomCartRecID=" + CustomCartRecID.ToString() + " and PackID=" + PackID.ToString() + " and CustomerID=" + ThisCustomer.CustomerID.ToString());
                }
                else
                {
                    DB.ExecuteSQL("update CustomCart set quantity=quantity-1 where ShoppingCartRecID=" + ShoppingCartRecID.ToString() + " and CustomCartRecID=" + CustomCartRecID.ToString() + " and PackID=" + PackID.ToString() + " and CustomerID=" + ThisCustomer.CustomerID.ToString());
                }
                DB.ExecuteSQL("delete from CustomCart where quantity<=0 and ShoppingCartRecID=" + ShoppingCartRecID.ToString() + " and CustomerID=" + ThisCustomer.CustomerID.ToString() + " and PackID=" + PackID.ToString());
            }

            String url = SE.MakeProductAndEntityLink(CommonLogic.QueryStringCanBeDangerousContent("entityname"), PackID, CommonLogic.QueryStringUSInt("entityid"), "");
            Response.Redirect(url + CommonLogic.IIF(CustomCartRecID == 0, "", "?cartrecid=" + ShoppingCartRecID.ToString()));
        }

    }
}
