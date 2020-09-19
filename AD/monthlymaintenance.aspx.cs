// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v7.0/Web/Admin/monthlymaintenance.aspx.cs 2     9/30/06 10:32p Buddy $
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for monthlymaintenance
    /// </summary>
    public partial class monthlymaintenance : System.Web.UI.Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
            
            Server.ScriptTimeout = 10000; // these could run quite a long time!
            if (!IsPostBack)
            {
                String SavedSettings = AppLogic.AppConfig("System.SavedMonthlyMaintenance");
                if (SavedSettings.Length != 0)
                {
                    foreach (String s in SavedSettings.Split(','))
                    {
                        if (s.Trim().Length != 0)
                        {
                            String[] token = s.Trim().Split('=');
                            String ParmName = token[0].ToUpper(CultureInfo.InvariantCulture).Trim();
                            String ParmValue = token[1].ToUpper(CultureInfo.InvariantCulture).Trim();
                            switch (ParmName)
                            {
                                case "INVALIDATEUSERLOGINS":
                                    InvalidateUserLogins.Checked = (ParmValue == "TRUE");
                                    break;
                                case "PURGEANONUSERS":
                                    PurgeAnonUsers.Checked = (ParmValue == "TRUE");
                                    break;
                                case "CLEARALLSHOPPINGCARTS":
                                    ClearAllShoppingCarts.SelectedValue = ParmValue;
                                    break;
                                case "CLEARALLWISHLISTS":
                                    ClearAllWishLists.SelectedValue = ParmValue;
                                    break;
                                case "CLEARALLGIFTREGISTRIES":
                                    ClearAllGiftRegistries.SelectedValue = ParmValue;
                                    break;
                                case "ERASEORDERPASSWORDS":
                                    EraseOrderPasswords.SelectedValue = ParmValue;
                                    break;
                                case "ERASEORDERCREDITCARDS":
                                    EraseOrderCreditCards.SelectedValue = ParmValue;
                                    break;
                                case "ERASEADDRESSCREDITCARDS":
                                    EraseAddressCreditCards.Checked = (ParmValue == "TRUE");
                                    break;
                                case "ERASESQLLOG":
                                    EraseSQLLog.SelectedValue = ParmValue;
                                    break;
                                case "TUNEINDEXES":
                                    TuneIndexes.Checked = (ParmValue == "TRUE");
                                    break;
                                case "SAVESETTINGS":
                                    SaveSettings.Checked = (ParmValue == "TRUE");
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private int GetIndex(DropDownList l, String TheVal)
        {
            int i = 0;
            foreach (ListItem ix in l.Items)
            {
                if (ix.Text.ToUpper(CultureInfo.InvariantCulture) == TheVal.ToUpper(CultureInfo.InvariantCulture))
                {
                    break;
                }
                i++;
            }
            return i;
        }


        protected void GOButton_Click(object sender, EventArgs e)
        {
            resetError("", false);

            String sql = String.Format("exec dbo.aspdnsf_MonthlyMaintenance @InvalidateCustomerCookies={0},@PurgeAnonCustomers={1},@CleanShoppingCartsOlderThan={2},@CleanWishListsOlderThan={3},@CleanGiftRegistriesOlderThan={4},@EraseCCFromAddresses={5},@EraseSQLLogOlderThan={6},@EraseCCFromOrdersOlderThan={7},@DefragIndexes={8}",
                CommonLogic.IIF(InvalidateUserLogins.Checked, "1", "0"),
                CommonLogic.IIF(PurgeAnonUsers.Checked, "1", "0"),
                ClearAllShoppingCarts.Items[ClearAllShoppingCarts.SelectedIndex].Value,
                ClearAllWishLists.Items[ClearAllWishLists.SelectedIndex].Value,
                ClearAllGiftRegistries.Items[ClearAllGiftRegistries.SelectedIndex].Value,
                CommonLogic.IIF(EraseAddressCreditCards.Checked, "1", "0"),
                EraseSQLLog.Items[EraseSQLLog.SelectedIndex].Value,
                EraseOrderCreditCards.Items[EraseOrderCreditCards.SelectedIndex].Value,
                CommonLogic.IIF(TuneIndexes.Checked, "1", "0")
                );
            DB.ExecuteLongTimeSQL(sql, 1000);

            if (SaveSettings.Checked)
            {
                StringBuilder tmpS = new StringBuilder(1024);
                tmpS.Append("InvalidateUserLogins=");
                tmpS.Append(InvalidateUserLogins.Checked);
                tmpS.Append(",");
                tmpS.Append("PurgeAnonUsers=");
                tmpS.Append(PurgeAnonUsers.Checked);
                tmpS.Append(",");
                tmpS.Append("ClearAllShoppingCarts=");
                tmpS.Append(ClearAllShoppingCarts.Items[ClearAllShoppingCarts.SelectedIndex].Value);
                tmpS.Append(",");
                tmpS.Append("ClearAllWishLists=");
                tmpS.Append(ClearAllWishLists.Items[ClearAllWishLists.SelectedIndex].Value);
                tmpS.Append(",");
                tmpS.Append("ClearAllGiftRegistries=");
                tmpS.Append(ClearAllGiftRegistries.Items[ClearAllGiftRegistries.SelectedIndex].Value);
                tmpS.Append(",");
                tmpS.Append("EraseOrderPasswords=");
                tmpS.Append(EraseOrderPasswords.Items[EraseOrderPasswords.SelectedIndex].Value);
                tmpS.Append(",");
                tmpS.Append("EraseOrderCreditCards=");
                tmpS.Append(EraseOrderCreditCards.Items[EraseOrderCreditCards.SelectedIndex].Value);
                tmpS.Append(",");
                tmpS.Append("EraseAddressCreditCards=");
                tmpS.Append(EraseAddressCreditCards.Checked);
                tmpS.Append(",");
                tmpS.Append("EraseSQLLog=");
                tmpS.Append(EraseSQLLog.Items[EraseSQLLog.SelectedIndex].Value);
                tmpS.Append(",");
                tmpS.Append("TuneIndexes=");
                tmpS.Append(TuneIndexes.Checked);
                tmpS.Append(",");
                tmpS.Append("SaveSettings=");
                tmpS.Append(SaveSettings.Checked);
                AppLogic.SetAppConfig("System.SavedMonthlyMaintenance", tmpS.ToString(), true);
            }

            resetError("Maintenance complete.", false);
        }

        protected void resetError(string error, bool isError)
        {
            string str = "<font class=\"noticeMsg\">NOTICE:</font>&nbsp;&nbsp;&nbsp;";
            if (isError)
            {
                str = "<font class=\"errorMsg\">ERROR:</font>&nbsp;&nbsp;&nbsp;";
            }

            if (error.Length > 0)
            {
                str += error + "";
            }
            else
            {
                str = "";
            }

            ltError.Text = str;
        }
    }
}
