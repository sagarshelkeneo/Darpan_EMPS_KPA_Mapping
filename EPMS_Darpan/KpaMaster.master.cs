using System;
using System.Web;
using System.Web.UI;

public partial class KpaMaster : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Display the username from session
            if (Session["Username"] != null)
            {
                // You can access it directly in markup as well
                // lblUsername.Text = Session["Username"].ToString();
            }
            else
            {
                // If session expired, redirect to login page
                Response.Redirect("~/Login.aspx", false);
            }
        }
    }

    protected void lnkLogout_Click(object sender, EventArgs e)
    {
        try
        {
            // Clear session
            Session.Clear();
            Session.Abandon();

            // Optionally clear authentication cookie
            if (Request.Cookies[".ASPXAUTH"] != null)
            {
                HttpCookie cookie = new HttpCookie(".ASPXAUTH", "")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(cookie);
            }

            // Redirect to login page
            Response.Redirect("~/Login.aspx", false);
        }
        catch (Exception ex)
        {
            // Log error if needed
            // For simplicity, just rethrow
            throw new Exception("Error during logout: " + ex.Message);
        }
    }
}
