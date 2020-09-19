// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/MapShippingMethodToPaymentMethod.aspx.cs 4     7/16/06 2:15p Redwoodtree $
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using AspDotNetStorefrontCommon;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// To be able to mapp a payment method to a specific shipping method. This is to avoid situations where customers
    /// can choose a payment method that is NOT supported for choosen shipping method. For example you don´t want customers to be able 
    /// to choose to pay by credit card when the cusotmer had choosen the shipping option COD (Cash on delivery). 
    /// We want to make it easy for the customer and therfor only show the supported payment method for each shipping option.
    /// 
    /// </summary>
    public partial class MapShippingMethodToPaymentMethod : System.Web.UI.Page
    {
        string strAvailiblePaymentMethods = AppLogic.AppConfig("PaymentMethods");

        /// <summary>
        /// Runs when the user hit the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            //If the page is not in postback then we do this
            if (!IsPostBack)
            {
                //Select all Shipping methods from database and add them to the listbox
                IDataReader rsReferenceShippingMethods = DB.GetRS("SELECT ShippingMethodID , Name FROM ShippingMethod");
                while (rsReferenceShippingMethods.Read())
                {
                    this.ListBoxAvailShippingMethods.Items.Add(new ListItem(DB.RSFieldByLocale(rsReferenceShippingMethods, "Name", "en-US") + " ( ID=" + rsReferenceShippingMethods.GetValue(0).ToString() + " ) ", rsReferenceShippingMethods.GetValue(0).ToString()));
                }
                rsReferenceShippingMethods.Close();

                //Here we PreSelect the 1:st Item in the Listbox
                this.ListBoxAvailShippingMethods.SelectedIndex = 0;

                //Update the payment options for selected shipping method.
                UpdateSelectedPayments(int.Parse(this.ListBoxAvailShippingMethods.SelectedValue));
            }
        }


        /// <summary>
        /// Build the new supported payments STRING and save this into the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnUpdateShippingToPaymentMethod_Click(object sender, EventArgs e)
        {
            int intSelectedShippingMethodID = int.Parse(this.ListBoxAvailShippingMethods.SelectedValue.ToString());
            string strSelectedPM = "";
            int intSelectedPaymentMethods = this.ListBoxSelectedPaymentMethods.Items.Count;

            if (intSelectedPaymentMethods > 0)
            {
                int i = 0;
                for (i = 0; i < intSelectedPaymentMethods; i++)
                {
                    strSelectedPM += this.ListBoxSelectedPaymentMethods.Items[i].Value.ToString().Trim();
                    strSelectedPM += ",";
                }

                if (strSelectedPM.EndsWith(","))
                {
                    int intSelectedPMLength = strSelectedPM.Length;
                    strSelectedPM = strSelectedPM.Remove(intSelectedPMLength - 1);
                }

                DB.ExecuteSQL("UPDATE ShippingMethod SET MappedPM=" + DB.SQuote(strSelectedPM) + " WHERE ShippingMethodID=" + intSelectedShippingMethodID.ToString());
            }

        }

        /// <summary>
        /// When the user select another item in the Listbox this event fires and call the function 
        /// to update the listbox for payment options for selected shipping method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ListBoxAvailShippingMethods_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ListBoxSelectedPaymentMethods.Items.Clear();
            UpdateSelectedPayments(int.Parse(this.ListBoxAvailShippingMethods.SelectedValue));
        }

        /// <summary>
        /// Update the listbox for payment options for selected shipping method.
        /// </summary>
        /// <param name="intSelectedShippingMethodID"></param>
        private void UpdateSelectedPayments(int intSelectedShippingMethodID)
        {
            string strCurrentMappingsInDB = "";

            this.ListBoxAvailPaymentMethods.Items.Clear();
            this.ListBoxSelectedPaymentMethods.Items.Clear();

            //Get the information from database of current payment options for selected shipping option.
            IDataReader rsCurrentMappings = DB.GetRS("SELECT MappedPM FROM ShippingMethod WHERE ShippingMethodID=" + intSelectedShippingMethodID.ToString());
            while (rsCurrentMappings.Read())
            {
                strCurrentMappingsInDB = DB.RSField(rsCurrentMappings, "MappedPM");
            }
            rsCurrentMappings.Close();

            Hashtable hashCurrentMappingsInDB = new Hashtable();

            if (strCurrentMappingsInDB.Length > 0)
            {

                string[] strSplittedCurrentMappingsInDB = strCurrentMappingsInDB.Split(new char[] { ',' });

                foreach (string strPMinDB in strSplittedCurrentMappingsInDB)
                {
                    hashCurrentMappingsInDB.Add(strPMinDB, strPMinDB);
                    this.ListBoxSelectedPaymentMethods.Items.Add(new ListItem(strPMinDB, strPMinDB));

                }

            }

            //ALL Possible payment options
            //CREDIT CARD,PAYPAL,PAYPALEXPRESS,REQUEST QUOTE,PURCHASE ORDER,CHECK BY MAIL,C.O.D.,ECHECK, MICROPAY

            if (AppLogic.MicropayIsEnabled())
            {
                if (strAvailiblePaymentMethods.Length != 0)
                {
                    strAvailiblePaymentMethods += ",";
                }
                strAvailiblePaymentMethods += AppLogic.ro_PMMicropay;
            }

            string[] strSplittedAvailiblePaymentMethods = strAvailiblePaymentMethods.Split(new char[] { ',' });

            //Loop through all availible payment options and select those option in listbox that is
            //allready selected in the database.
            foreach (string strPM in strSplittedAvailiblePaymentMethods)
            {
                if (hashCurrentMappingsInDB.ContainsKey(strPM))
                {

                }
                else
                {
                    this.ListBoxAvailPaymentMethods.Items.Add(new ListItem(strPM, strPM));
                }

            }

            if (this.ListBoxSelectedPaymentMethods.Items.Count > 0)
            {
                this.ListBoxSelectedPaymentMethods.Items[0].Text = this.ListBoxSelectedPaymentMethods.Items[0].Value + "(Show by default)";

            }
            UpdateSaveToDBTextFiled();
        }


        /// <summary>
        /// Move a selected payment options one step up in the listbox. The top Item will alsyas be the
        /// payemnt option that the store show as default (expanded) to the user at checkout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnMovePaymentUp_Click(object sender, EventArgs e)
        {
            int intSelectedIndex;
            string strTempSwapText;
            string strTempSwapValue;


            intSelectedIndex = ListBoxSelectedPaymentMethods.SelectedIndex;

            if (intSelectedIndex > 0)
            {
                strTempSwapText = ListBoxSelectedPaymentMethods.Items[intSelectedIndex - 1].Text.ToString();
                strTempSwapValue = ListBoxSelectedPaymentMethods.Items[intSelectedIndex - 1].Value.ToString();


                ListBoxSelectedPaymentMethods.Items[intSelectedIndex - 1].Value = ListBoxSelectedPaymentMethods.Items[intSelectedIndex].Value;
                ListBoxSelectedPaymentMethods.Items[intSelectedIndex - 1].Text = ListBoxSelectedPaymentMethods.Items[intSelectedIndex].Text;

                ListBoxSelectedPaymentMethods.Items[intSelectedIndex].Value = strTempSwapValue;
                ListBoxSelectedPaymentMethods.Items[intSelectedIndex].Text = strTempSwapText;
                ListBoxSelectedPaymentMethods.SelectedIndex = intSelectedIndex - 1;
            }

            this.ListBoxSelectedPaymentMethods.Items[0].Text = this.ListBoxSelectedPaymentMethods.Items[0].Value + " (Show by default)";
            this.ListBoxSelectedPaymentMethods.Items[1].Text = this.ListBoxSelectedPaymentMethods.Items[1].Value;

            UpdateSaveToDBTextFiled();
        }

        /// <summary>
        /// Move the selected itme from the Avail list to the selected list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSelectOne_Click(object sender, EventArgs e)
        {
            int intSelectedIndex;
            //Get the selected Item in the UnSelected ListBox
            intSelectedIndex = this.ListBoxAvailPaymentMethods.SelectedIndex;

            //If there are a selected Item then move the Item
            if (this.ListBoxAvailPaymentMethods.Items.Count >= 1 && intSelectedIndex != -1)
            {
                //Make the Move
                this.ListBoxSelectedPaymentMethods.Items.Add(this.ListBoxAvailPaymentMethods.Items[intSelectedIndex]);
                this.ListBoxAvailPaymentMethods.Items.Remove(this.ListBoxAvailPaymentMethods.Items[intSelectedIndex]);

                //Take away the selection from the ListBox
                this.ListBoxSelectedPaymentMethods.SelectedIndex = -1;
            }

            if (this.ListBoxSelectedPaymentMethods.Items.Count > 0)
            {
                this.ListBoxSelectedPaymentMethods.Items[0].Text = this.ListBoxSelectedPaymentMethods.Items[0].Value + " (Show by default)";
                UpdateSaveToDBTextFiled();
            }
        }


        /// <summary>
        /// Move the selected itme from the selected list to the avail list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnDeSelectOne_Click(object sender, EventArgs e)
        {

            int intSelectedIndex;


            //Get the selected Index from the ListBox
            intSelectedIndex = this.ListBoxSelectedPaymentMethods.SelectedIndex;

            //If there are a Item in the 
            if (intSelectedIndex != -1)
            {

                //If there are a selected Item in the ListBox then make the move
                if (this.ListBoxSelectedPaymentMethods.Items.Count >= 1 && intSelectedIndex != -1)
                {
                    this.ListBoxAvailPaymentMethods.Items.Add(new ListItem(this.ListBoxSelectedPaymentMethods.Items[intSelectedIndex].Value, this.ListBoxSelectedPaymentMethods.Items[intSelectedIndex].Value));
                    this.ListBoxSelectedPaymentMethods.Items.Remove(this.ListBoxSelectedPaymentMethods.Items[intSelectedIndex]);
                    this.ListBoxAvailPaymentMethods.SelectedIndex = -1;
                }

            }
            else
            {
                //Response.Write("You must select ONE");
            }

            if (this.ListBoxSelectedPaymentMethods.Items.Count > 0)
            {
                this.ListBoxSelectedPaymentMethods.Items[0].Text = this.ListBoxSelectedPaymentMethods.Items[0].Value + " (Show by default)";
                UpdateSaveToDBTextFiled();
            }

        }


        /// <summary>
        /// Move ALL itmes from the Avail list to the selected list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSelectAll_Click(object sender, EventArgs e)
        {

            int intListBoxItems;
            int i = 0;

            //Get the selected Item in the UnSelected ListBox
            intListBoxItems = this.ListBoxAvailPaymentMethods.Items.Count;

            //If there are Item then move the Item
            if (intListBoxItems > 0)
            {
                for (i = 0; i < intListBoxItems; i++)
                {
                    //Make the Move
                    this.ListBoxSelectedPaymentMethods.Items.Add(this.ListBoxAvailPaymentMethods.Items[0]);
                    this.ListBoxAvailPaymentMethods.Items.Remove(this.ListBoxAvailPaymentMethods.Items[0]);
                }
            }

            if (this.ListBoxSelectedPaymentMethods.Items.Count > 0)
            {
                this.ListBoxSelectedPaymentMethods.Items[0].Text = this.ListBoxSelectedPaymentMethods.Items[0].Value + " (Show by default)";
                UpdateSaveToDBTextFiled();
            }

        }

        /// <summary>
        /// Move ALL itmes from the selected list to the avail list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnDeSelectALL_Click(object sender, EventArgs e)
        {

            int intListBoxItems;
            int i = 0;

            intListBoxItems = this.ListBoxSelectedPaymentMethods.Items.Count;

            //Make the move
            if (intListBoxItems > 0)
            {
                for (i = 0; i < intListBoxItems; i++)
                {
                    this.ListBoxAvailPaymentMethods.Items.Add(new ListItem(this.ListBoxSelectedPaymentMethods.Items[0].Value, this.ListBoxSelectedPaymentMethods.Items[0].Value));
                    this.ListBoxSelectedPaymentMethods.Items.Remove(this.ListBoxSelectedPaymentMethods.Items[0]);
                }
            }
            this.txtSaveToDBInfo.Text = "";

        }

        /// <summary>
        /// Update (for display only) the textbox so it get more "clear" which information that is
        /// saved to the databse for mapping payments
        /// </summary>
        private void UpdateSaveToDBTextFiled()
        {
            string strSelectedPM = "";

            int intSelectedPaymentMethods = this.ListBoxSelectedPaymentMethods.Items.Count;

            int i = 0;

            for (i = 0; i < intSelectedPaymentMethods; i++)
            {
                strSelectedPM += this.ListBoxSelectedPaymentMethods.Items[i].Value.ToString().Trim();
                strSelectedPM += ",";
            }

            if (strSelectedPM.EndsWith(","))
            {
                int intSelectedPMLength = strSelectedPM.Length;
                strSelectedPM = strSelectedPM.Remove(intSelectedPMLength - 1);
            }
            this.txtSaveToDBInfo.Text = strSelectedPM;
        }
    }
}