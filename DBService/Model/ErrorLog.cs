
using System;
using System.Configuration;
using System.IO;
using System.Web;

/// <summary>
/// Handles logging and redirecting on exceptions
/// </summary>
public class ErrorLog
{
    // Log folder path
    private readonly string _logFolderPath = ConfigurationManager.AppSettings["ErrorLogFolderPath"];



    public void LogError(Exception ex)
    {
        if (ex == null) return;

        try
        {
            // Ensure App_Data folder exists
            if (!Directory.Exists(_logFolderPath))
                Directory.CreateDirectory(_logFolderPath);

            // Generate log file name with today's date (e.g., ErrorLog_07Oct25.txt)
            string logFileName = $"ErrorLog_{DateTime.Now:ddMMMyy}.txt";
            string logFilePath = Path.Combine(_logFolderPath, logFileName);

            // Ensure file exists
            if (!File.Exists(logFilePath))
                using (File.Create(logFilePath)) { } // creates empty file safely

            // Log error details safely with timestamp
            string logEntry = $@"
==================== ERROR LOG ====================
Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
Message: {ex.Message}
Source: {ex.Source}
Stack Trace: {ex.StackTrace}
===================================================
";

            File.AppendAllText(logFilePath, logEntry);

            // Store error details in session for display
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session["LastError"] = ex.ToString();

                // Redirect to ErrorPage.aspx without ThreadAbortException
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Redirect("~/ErrorPage.aspx", false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }
        catch
        {
            // Fail silently if logging or redirecting fails
        }
    }
}



