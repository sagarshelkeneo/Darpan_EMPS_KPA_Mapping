using System;
using System.Web.UI;

public partial class ErrorPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Show error message if passed via querystring or session
            string errorMessage = Session["LastError"] as string;
            lblErrorDetails.Text = string.IsNullOrEmpty(errorMessage)
                ? "No detailed error information available."
                : Server.HtmlEncode(errorMessage);
        }
    }

    protected void btnGoToLogin_Click(object sender, EventArgs e)
    {
        // Clear session and redirect to login
        Session.Clear();
        Response.Redirect("Login.aspx");
    }
}



