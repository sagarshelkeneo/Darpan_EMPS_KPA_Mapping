using System;
using System.Web;
using System.Web.UI;

public partial class _Default_logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Clear session
        Session.Clear();
        Session.Abandon();

        // Clear authentication cookie if using FormsAuthentication
        System.Web.Security.FormsAuthentication.SignOut();


        // Redirect to login page
        Response.Redirect("Login.aspx", false);
    }
}
