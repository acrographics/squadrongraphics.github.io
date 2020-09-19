// ------------------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com, 1995-2007.  All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// $Header: /v6.2/Web/Admin/encrypttest.aspx.cs 4     9/05/06 4:19p Redwoodtree $
// ------------------------------------------------------------------------------------------
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

    public partial class encrypttest : AspDotNetStorefront.SkinBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ThisCustomer.IsAdminSuperUser)
            {
                Response.Redirect("default.aspx");
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Label1.Text = Security.MungeString(TextBox1.Text);
            Label2.Text = Security.UnmungeString(Label1.Text);
        }
        protected void Button2_Click(object sender, EventArgs e)
        {
            Label3.Text = Security.MungeStringOld(TextBox2.Text);
            Label4.Text = Security.UnmungeStringOld(Label3.Text);
        }
        protected void Button3_Click(object sender, EventArgs e)
        {
            Label5.Text = Security.UnmungeStringOld(TextBox3.Text);
            Label6.Text = Security.MungeString(Label5.Text);
            Label7.Text = Security.UnmungeString(Label6.Text);
        }
}
}