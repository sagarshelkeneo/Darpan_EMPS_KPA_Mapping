<%@ Application Language="C#" %>
<%@ Import Namespace="EPMS_Darpan" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Routing" %>

<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);
    }

    //protected void Application_Error(object sender, EventArgs e)
    //{
    //    Exception ex = Server.GetLastError();
    //    if (ex != null)
    //    {
    //        // Log the error (you can reuse your ErrorLog class)
    //        try
    //        {
    //            string errorDetails = $"{DateTime.Now}: {ex.Message}\n{ex.StackTrace}";
    //            string errorLogPath = HttpContext.Current.Server.MapPath("~/App_Data/ErrorLog.txt");
    //            System.IO.File.AppendAllText(errorLogPath, errorDetails + Environment.NewLine);

    //            // Store error details in session for display
    //            HttpContext.Current.Session["LastError"] = ex.ToString();
    //        }
    //        catch
    //        {
    //            // Ignore logging errors
    //        }

    //        // Redirect to ErrorPage
    //        Server.ClearError();
    //        Response.Redirect("~/ErrorPage.aspx");
    //    }
    //}



</script>
