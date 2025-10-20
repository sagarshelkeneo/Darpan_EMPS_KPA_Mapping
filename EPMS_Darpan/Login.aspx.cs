using AjaxControlToolkit.HtmlEditor.ToolbarButtons;
using Antlr.Runtime;
using DBService;
using DBService.Model;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    private readonly LoginService _loginService = new LoginService();

    private string connString = System.Configuration.ConfigurationManager.ConnectionStrings["EPMS"].ConnectionString;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //throw new Exception("Test unhandled exception!");

            txtUsername.Text = "";
            txtPassword.Text = "";

        }

    }

    protected void btnSignIn_Click(object sender, EventArgs e)
    {
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text.Trim();
        int empId = _loginService.ValidateUser(username, password);
        if (empId != 0)
        {

            bool result = _loginService.CheckLogin(username, password);

            if (result == true)
            {
                Session["Username"] = username;
                Session["EmpID"] = empId;


                // After successful login
                HttpCookie cookie = new HttpCookie("Message");
                cookie.Value = "Login Successful!";
                cookie.Expires = DateTime.Now.AddSeconds(2); // optional, short-lived
                Response.Cookies.Add(cookie);


                Response.Redirect("Index.aspx", false);

            }
            else
            {
                // Failure
                lblMessage.Text = "Invalid username or password.";
            }
        }
        else
        {
            lblMessage.Text = "Invalid username or you dont have acess.";

        }



    }

}