using DBService;
using DBService.Model;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class index : System.Web.UI.Page
{
    private readonly IndexService _indexService = new IndexService();
    private readonly ErrorLog _errorLog = new ErrorLog();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                if (Request.Cookies["LoginSuccess"] != null)
                {
                    // Call JavaScript to show toaster
                    ClientScript.RegisterStartupScript(this.GetType(), "ShowToaster", "showToaster('Login Successful!');", true);
                    
                }


                BindGrid();
                instructionInfo.Attributes["class"] = "collapse show"; // default
                hfInstructionState.Value = "show";
            }
            else
            {
                // Restore previous state after postback
                instructionInfo.Attributes["class"] = "collapse " + hfInstructionState.Value;
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            lblMessage.Text = "Error during page load.";
            lblMessage.Visible = true;
        }
    }

    protected void btnAddRole_Click(object sender, EventArgs e)
    {
        try
        {
            Response.Redirect("AddRole.aspx", false);
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            lblMessage.Text = "Error redirecting to Add Role page.";
            lblMessage.Visible = true;
        }
    }

    protected void gvRoles_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "ActionRole")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                string roleId = args[0];
                string status = args.Length > 1 ? args[1] : "";

                Session["status"] = status;
                if (status.Equals("NOT STARTED", StringComparison.OrdinalIgnoreCase))
                {
                    string encryptedParams = EncryptionHelper.Encrypt($"RoleID={roleId}&isSubmitted=0");
                    Response.Redirect($"view.aspx?data={encryptedParams}", false);

                    //Response.Redirect($"view.aspx?RoleID={roleId}&isSubmitted=0", false);
                }
                else
                {
                    string encryptedParams = EncryptionHelper.Encrypt($"RoleID={roleId}&isSubmitted=1");
                    Response.Redirect($"view.aspx?data={encryptedParams}", false);

                    //Response.Redirect($"view.aspx?RoleID={roleId}&isSubmitted=1",false);
                }
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            lblMessage.Text = "Error while processing the role action.";
            lblMessage.Visible = true;
        }
    }

    private void BindGrid()
    {
        try
        {
            int empId = 0;
            if (Session["EmpID"] != null)
            {
                int.TryParse(Session["EmpID"].ToString(), out empId);
            }

            DataTable dt = _indexService.GetRoles(empId);

            if (dt == null)
            {
                lblMessage.Text = "No data found.";
                lblMessage.Visible = true;
                return;
            }

            DataView dv = dt.DefaultView;

            string selectedStatus = ddlStatus.SelectedValue;
            if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "Status (all)")
            {
                dv.RowFilter = $"Status = '{selectedStatus}'";
            }

            string searchText = txtSearch.Text.Trim().Replace("'", "''");
            if (!string.IsNullOrEmpty(searchText))
            {
                var conditions = dt.Columns.Cast<DataColumn>()
                                    .Where(c => c.DataType == typeof(string))
                                    .Select(c => $"{c.ColumnName} LIKE '%{searchText}%'");

                string searchFilter = string.Join(" OR ", conditions);

                dv.RowFilter = !string.IsNullOrEmpty(dv.RowFilter)
                    ? $"({dv.RowFilter}) AND ({searchFilter})"
                    : searchFilter;
            }

            gvRoles.DataSource = dv;
            gvRoles.DataBind();

           
            // Get total count
            int totalCount = dt.Rows.Count;

            // Get SUBMITTED count
            int submittedCount = dv.ToTable()
                                   .AsEnumerable()
                                   .Count(row => row["Status"].ToString()
                                   .Equals("SUBMITTED", StringComparison.OrdinalIgnoreCase));

            // Get MANUALLY ADDED count
            int manuallyAddedCount = dv.ToTable()
                                       .AsEnumerable()
                                       .Count(row => row["Status"].ToString()
                                       .Equals("MANUALLY ADDED", StringComparison.OrdinalIgnoreCase));


            lblMessage.Text = $"{submittedCount + manuallyAddedCount} | {totalCount}";
            lblMessage.Visible = true;
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            lblMessage.Text = "Error loading role data.";
            lblMessage.Visible = true;
        }
    }

    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            BindGrid();
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            lblMessage.Text = "Error filtering by status.";
            lblMessage.Visible = true;
        }
    }

    protected void gvRoles_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            gvRoles.PageIndex = e.NewPageIndex;
            BindGrid();
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            lblMessage.Text = "Error changing page index.";
            lblMessage.Visible = true;
        }
    }

    protected void gvRoles_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string status = DataBinder.Eval(e.Row.DataItem, "Status")?.ToString() ?? "";
                Button btn = (Button)e.Row.FindControl("btnView");

                if (btn != null)
                {
                    if (status.Equals("NOT STARTED", StringComparison.OrdinalIgnoreCase))
                    {
                        btn.Text = "Review";
                        btn.CssClass = "btn btn-primary btn-sm";
                    }
                    else
                    {
                        btn.Text = "View";
                        btn.CssClass = "btn btn-primary btn-sm";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        try
        {
            BindGrid();
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            lblMessage.Text = "Error while searching roles.";
            lblMessage.Visible = true;
        }
    }

    protected void btnToggleInstructions_Click(object sender, EventArgs e)
    {
        try
        {
            if (hfInstructionState.Value == "show")
                hfInstructionState.Value = "";
            else
                hfInstructionState.Value = "show";

            instructionInfo.Attributes["class"] = "collapse " + hfInstructionState.Value;
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }
}
