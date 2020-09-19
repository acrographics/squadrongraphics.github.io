// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/checkoutgiftcard.aspx.cs 5     9/13/06 11:13p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefront
{
    public partial class checkoutgiftcard : SkinBase
    {
        ShoppingCart cart = null;
        bool Checkout = true;
        bool ContainsEmailGiftCards = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = -1;
            Response.AddHeader("pragma", "no-cache");

            RequireSecurePage();

            SectionTitle = AppLogic.GetString("checkoutpayment.aspx.1", SkinID, ThisCustomer.LocaleSetting);

            cart = new ShoppingCart(SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, 0, false);

            // -----------------------------------------------------------------------------------------------
            // NOTE ON PAGE LOAD LOGIC:
            // We are checking here for required elements to allowing the customer to stay on this page.
            // Many of these checks may be redundant, and they DO add a bit of overhead in terms of db calls, but ANYTHING really
            // could have changed since the customer was on the last page. Remember, the web is completely stateless. Assume this
            // page was executed by ANYONE at ANYTIME (even someone trying to break the cart). 
            // It could have been yesterday, or 1 second ago, and other customers could have purchased limitied inventory products, 
            // coupons may no longer be valid, etc, etc, etc...
            // -----------------------------------------------------------------------------------------------
            if (!ThisCustomer.IsRegistered && !AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout"))
            {
                Response.Redirect("createaccount.aspx?checkout=true");
            }
            if (ThisCustomer.PrimaryBillingAddressID == 0 || ThisCustomer.PrimaryShippingAddressID == 0)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutpayment.aspx.2", SkinID, ThisCustomer.LocaleSetting)));
            }


            // re-validate all shipping info, as ANYTHING could have changed since last page:
            if (!cart.ShippingIsAllValid())
            {
                HttpContext.Current.Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + HttpContext.Current.Server.UrlEncode(AppLogic.GetString("shoppingcart.cs.95", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)));
            }

            if (!IsPostBack)
            {
                CreateGiftCards(cart);
                InitializePageContent();
                if (!ContainsEmailGiftCards)
                {
                    Response.Redirect("checkoutpayment.aspx");
                }
            }

        }


        private void InitializePageContent()
        {
            JSPopupRoutines.Text = AppLogic.GetJSPopupRoutines();
            pnlHeaderGraphic.Visible = Checkout;
            checkoutheadergraphic.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_3.gif");
            if (ContainsEmailGiftCards)
            {
                lnkGiftCard.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_3_2.gif");
            }

            if (lblErrMsg.Text.Length != 0)
            {
                lblErrMsg.Visible = true;
            }
            else
            {
                lblErrMsg.Visible = false;
            }

            string sql = "select p.name productname, case when isnull(pv.name, '')='' then '' else ' - ' + pv.name end variantname, g.* from GiftCard g join ShoppingCart s on g.ShoppingCartRecID = s.ShoppingCartRecID join product p on s.productid = p.productid join productvariant pv on s.variantid = pv.variantid where g.GiftCardTypeID=101 and g.PurchasedByCustomerID = " + ThisCustomer.CustomerID.ToString();
            IDataReader dr = DB.GetRS(sql);
            rptrEmailGiftCards.DataSource = dr;
            rptrEmailGiftCards.DataBind();
            dr.Close();

        }

      private void CreateGiftCards(ShoppingCart cart)
      {
        for (int i = 0; i < cart.CartItems.Count; i++)
        {
          CartItem c = (CartItem)cart.CartItems[i];
          if (c.m_ProductTypeId == 100 || c.m_ProductTypeId == 101 || c.m_ProductTypeId == 102)
          {
            //Check the number of certificate records in the GiftCard table for this item.
            int CardCnt = DB.GetSqlN("select count(*) as N from GiftCard where " +
                                      "ShoppingCartRecID=" + c.m_ShoppingCartRecordID.ToString() +
                                      " and PurchasedByCustomerID=" + cart.ThisCustomer.CustomerID.ToString());
            //Add records if not enough
            if (CardCnt < c.m_Quantity)
            {
              for (int j = 1; j <= c.m_Quantity - CardCnt; j++)
              {
                GiftCard.CreateGiftCard(ThisCustomer.CustomerID, null, null, c.m_ShoppingCartRecordID, null, null, c.m_Price, null, c.m_Price, c.m_ProductTypeId, null, null, null, null, null, null, null, null, null);
              }
            }
            //Delete records if there are too many. Delete from the end of the list just because we have to delete something.
            if (CardCnt > c.m_Quantity)
            {
              int DeleteCnt = CardCnt - c.m_Quantity;
              string sql = "delete from GiftCard where GiftCardID in (select TOP " + DeleteCnt.ToString() + " GiftCardID from GiftCard where ShoppingCartRecID=" + c.m_ShoppingCartRecordID.ToString() + " and PurchasedByCustomerID=" + cart.ThisCustomer.CustomerID.ToString() + " order by GiftCardID DESC)";
              DB.ExecuteSQL(sql);
            }

            //GiftCard.DeleteGiftCardsInCart(c.m_ShoppingCartRecordID);
          }
          if (c.m_ProductTypeId == 101) ContainsEmailGiftCards = true;
        }
      }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            int completed = 0;
            string retval = string.Empty;
            foreach (RepeaterItem ri in rptrEmailGiftCards.Items)
            {
                string recipientname = ((TextBox)ri.FindControl("EmailName")).Text;
                string emailto = ((TextBox)ri.FindControl("EmailTo")).Text;
                string emailmsg = ((TextBox)ri.FindControl("EmailMessage")).Text;
                string giftcardid = ((TextBox)ri.FindControl("giftcardid")).Text;

                retval = GiftCard.UpdateCard(Int32.Parse(giftcardid), null, null, null, null, null, null, recipientname, emailto, emailmsg, null, null, null, null, null, null);

                if (recipientname.Trim().Length > 0 && emailto.Trim().Length > 0 && retval == "")
                {
                    completed++;
                }
                retval = string.Empty;
            }

            if (completed == rptrEmailGiftCards.Items.Count)
            {
                Response.Redirect("checkoutpayment.aspx?checkout=true");
            }
            else
            {
                lblErrMsg.Text = "Blank address fields! All fields must be completed to insure proper delivery";
                InitializePageContent();
            }

        }
    }
}