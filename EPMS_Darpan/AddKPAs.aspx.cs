using DBService;
using DBService.Model;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AddKPAs : System.Web.UI.Page
{
    private readonly AddKPAService _addkpaservice = new AddKPAService();
    private readonly ErrorLog _errorLog = new ErrorLog();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            int roleId = 0;

            string encryptedData = Request.QueryString["data"];
            if (!string.IsNullOrEmpty(encryptedData))
            {
                string decrypted = EncryptionHelper.Decrypt(encryptedData);
                var queryParams = System.Web.HttpUtility.ParseQueryString(decrypted);

                roleId = Convert.ToInt32(queryParams["roleId"]);

                // Use roleId as needed
            }

            if (roleId == 0)
            {
                Response.Redirect("AddRole.aspx", false);

                return; // Stop further execution
            }

            if (!IsPostBack)
            {
                // Add one default row on first load
                var initialRows = new List<KPARow>
            {
                new KPARow
                {
                    KPAName = "",
                    UnitOfKPA = "",
                    CategoryValue = "",
                    CategoryText = "--Select--",
                    DatasourceValue = "",
                    DatasourceText = "--Select--",
                    Importance = "", // let dropdown decide
                    OtherSource = ""
                }
            };

                ViewState["KpaRows"] = initialRows;
                BindRepeater();
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }


    private void BindRepeater()
    {
        try
        {
            var rows = ViewState["KpaRows"] as List<KPARow>;
            rptKPA.DataSource = rows;
            rptKPA.DataBind();
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    protected void btnAddRow_Click(object sender, EventArgs e)
    {
        try
        {
            var rows = ViewState["KpaRows"] as List<KPARow>;

            // Save current rows
            foreach (RepeaterItem item in rptKPA.Items)
            {
                DropDownList ddlCategory = item.FindControl("ddlCategory") as DropDownList;
                DropDownList ddlDatasource = item.FindControl("ddlDatasource") as DropDownList;
                DropDownList ddlImportance = item.FindControl("ddlImportance") as DropDownList;
                TextBox txtKPAName = item.FindControl("txtKPAName") as TextBox;
                TextBox txtOtherSource = item.FindControl("txtOtherSource") as TextBox;
                TextBox txtUnitOfKPA = item.FindControl("txtUnitOfKPA") as TextBox;

                rows[item.ItemIndex].KPAName = txtKPAName?.Text;
                rows[item.ItemIndex].UnitOfKPA = txtUnitOfKPA?.Text;
                rows[item.ItemIndex].CategoryValue = ddlCategory.SelectedValue;
                rows[item.ItemIndex].CategoryText = ddlCategory.SelectedItem.Text;
                rows[item.ItemIndex].DatasourceValue = ddlDatasource.SelectedValue;
                rows[item.ItemIndex].DatasourceText = ddlDatasource.SelectedItem.Text;
                rows[item.ItemIndex].Importance = ddlImportance.SelectedValue;
                rows[item.ItemIndex].OtherSource = txtOtherSource.Text;
            }

            // Add new empty row
            rows.Add(new KPARow
            {
                KPAName = "",
                UnitOfKPA = "",
                CategoryValue = "",
                CategoryText = "--Select--",
                DatasourceValue = "",
                DatasourceText = "--Select--",
                Importance = "Mid",
                OtherSource = ""
            });

            ViewState["KpaRows"] = rows;
            BindRepeater();
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    protected void rptKPA_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DropDownList ddlCategory = e.Item.FindControl("ddlCategory") as DropDownList;
                DropDownList ddlDatasource = e.Item.FindControl("ddlDatasource") as DropDownList;
                DropDownList ddlImportance = e.Item.FindControl("ddlImportance") as DropDownList;
                TextBox txtOtherSource = e.Item.FindControl("txtOtherSource") as TextBox;

                BindDropdown(ddlCategory, _addkpaservice.GetCategories(), "KPACategoryName", "KPACategoryID");
                BindDropdown(ddlDatasource, _addkpaservice.GetDataSources(), "DataSourceName", "DataSourceID");
                BindDropdown(ddlImportance, _addkpaservice.GetImportance(), "IMPORTANCECODE", "IMPORTANCEID", addSelect: false);

                var rows = ViewState["KpaRows"] as List<KPARow>;
                if (rows != null && e.Item.ItemIndex < rows.Count)
                {
                    var row = rows[e.Item.ItemIndex];

                    if (!string.IsNullOrEmpty(row.CategoryValue))
                        ddlCategory.SelectedValue = row.CategoryValue;

                    if (!string.IsNullOrEmpty(row.DatasourceValue))
                        ddlDatasource.SelectedValue = row.DatasourceValue;

                    ddlImportance.SelectedValue = string.IsNullOrEmpty(row.Importance) ? ddlImportance.Items[0].Value : row.Importance;

                    txtOtherSource.CssClass = row.DatasourceValue.Equals("other", StringComparison.OrdinalIgnoreCase)
                        ? "form-control form-control-sm mt-2"
                        : "form-control form-control-sm mt-2 d-none";
                    txtOtherSource.Text = row.OtherSource;
                }
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    protected void rptKPA_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "DeleteRow")
            {
                var rows = ViewState["KpaRows"] as List<KPARow>;

                // Save current values
                foreach (RepeaterItem item in rptKPA.Items)
                {
                    DropDownList ddlCategory = item.FindControl("ddlCategory") as DropDownList;
                    DropDownList ddlDatasource = item.FindControl("ddlDatasource") as DropDownList;
                    DropDownList ddlImportance = item.FindControl("ddlImportance") as DropDownList;
                    TextBox txtKPAName = item.FindControl("txtKPAName") as TextBox;
                    TextBox txtOtherSource = item.FindControl("txtOtherSource") as TextBox;
                    TextBox txtUnitOfKPA = item.FindControl("txtUnitOfKPA") as TextBox;

                    rows[item.ItemIndex].KPAName = txtKPAName.Text;
                    rows[item.ItemIndex].UnitOfKPA = txtUnitOfKPA.Text;
                    rows[item.ItemIndex].CategoryValue = ddlCategory.SelectedValue;
                    rows[item.ItemIndex].CategoryText = ddlCategory.SelectedItem.Text;
                    rows[item.ItemIndex].DatasourceValue = ddlDatasource.SelectedValue;
                    rows[item.ItemIndex].DatasourceText = ddlDatasource.SelectedItem.Text;
                    rows[item.ItemIndex].Importance = ddlImportance.SelectedValue;
                    rows[item.ItemIndex].OtherSource = txtOtherSource.Text;
                }

                int index = Convert.ToInt32(e.CommandArgument);
                if (rows != null && index >= 0 && index < rows.Count)
                {
                    rows.RemoveAt(index);
                }

                ViewState["KpaRows"] = rows;
                BindRepeater();
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    protected void ddlDatasource_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            DropDownList ddl = sender as DropDownList;
            if (ddl != null)
            {
                RepeaterItem item = ddl.NamingContainer as RepeaterItem;
                TextBox txtOther = item.FindControl("txtOtherSource") as TextBox;

                txtOther.CssClass = ddl.SelectedItem.Text.Equals("other", StringComparison.OrdinalIgnoreCase)
                    ? "form-control form-control-sm mt-2"
                    : "form-control form-control-sm mt-2 d-none";

                var rows = ViewState["KpaRows"] as List<KPARow>;
                rows[item.ItemIndex].DatasourceValue = ddl.SelectedValue;
                rows[item.ItemIndex].DatasourceText = ddl.SelectedItem.Text;
                rows[item.ItemIndex].OtherSource = txtOther.Text;
                ViewState["KpaRows"] = rows;
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }


    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            int roleId = 0;

            string encryptedData = Request.QueryString["data"];
            if (!string.IsNullOrEmpty(encryptedData))
            {
                string decrypted = EncryptionHelper.Decrypt(encryptedData);
                var queryParams = System.Web.HttpUtility.ParseQueryString(decrypted);

                roleId = Convert.ToInt32(queryParams["roleId"]);

                // Use roleId as needed
            }


            var rows = ViewState["KpaRows"] as List<KPARow>;
            if (rows == null || rows.Count == 0)
            {
                ShowMessage(ConfigurationManager.AppSettings["Error_AddAtLeastOneKPA"]);
                return;
            }

            if (!ValidateKpaRows())
                return;

            //if (string.IsNullOrWhiteSpace(txtComment.Text))
            //{
            //    ShowMessage(ConfigurationManager.AppSettings["Error_EnterComment"]);
            //    return;
            //}

            string businessrulemessage;
            if (!ValidateGuardrails(out businessrulemessage))
            {
                ShowMessage(businessrulemessage);
                return;
            }

            List<KPAMaster> kpaList = new List<KPAMaster>();
            foreach (RepeaterItem item in rptKPA.Items)
            {
                DropDownList ddlCategory = item.FindControl("ddlCategory") as DropDownList;
                DropDownList ddlDatasource = item.FindControl("ddlDatasource") as DropDownList;
                DropDownList ddlImportance = item.FindControl("ddlImportance") as DropDownList;
                TextBox txtKPAName = item.FindControl("txtKPAName") as TextBox;
                TextBox txtOtherSource = item.FindControl("txtOtherSource") as TextBox;
                TextBox txtUnitOfKPA = item.FindControl("txtUnitOfKPA") as TextBox;

                kpaList.Add(new KPAMaster
                {
                    CategoryId = Convert.ToInt32(ddlCategory.SelectedValue),
                    DataSourceId = Convert.ToInt32(ddlDatasource.SelectedValue),
                    KpaName = txtKPAName.Text.Trim(),
                    UnitOfKpa = txtUnitOfKPA.Text.Trim(),
                    DataSourceText = ddlDatasource.SelectedItem.Text,
                    OtherSource = txtOtherSource.Text.Trim(),
                    ImportanceId = Convert.ToInt32(ddlImportance.SelectedValue),
                    Comment = txtComment.Text.Trim()
                });
            }

            int empId = 0;
            if (Session["EmpID"] != null)
            {
                int.TryParse(Session["EmpID"].ToString(), out empId);
            }

            _addkpaservice.InsertKpaData(kpaList, roleId, empId);

            // Reset after insert
            ViewState["KpaRows"] = new List<KPARow>();
            BindRepeater();

            // After successful login
            HttpCookie cookie = new HttpCookie("Message");
            cookie.Value = "KPA Mapping Successful!";
            cookie.Expires = DateTime.Now.AddSeconds(2); // optional, short-lived
            Response.Cookies.Add(cookie);


            Response.Redirect("Index.aspx", false);
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }


    private bool ValidateKpaRows()
    {
        try
        {
            foreach (RepeaterItem item in rptKPA.Items)
            {
                DropDownList ddlCategory = item.FindControl("ddlCategory") as DropDownList;
                DropDownList ddlDatasource = item.FindControl("ddlDatasource") as DropDownList;
                DropDownList ddlImportance = item.FindControl("ddlImportance") as DropDownList;
                TextBox txtKPAName = item.FindControl("txtKPAName") as TextBox;
                TextBox txtOtherSource = item.FindControl("txtOtherSource") as TextBox;

                if (string.IsNullOrWhiteSpace(txtKPAName.Text))
                {
                    ShowMessage(ConfigurationManager.AppSettings["Error_KPANameRequired"]);
                    return false;
                }
                if (ddlCategory == null || string.IsNullOrEmpty(ddlCategory.SelectedValue))
                {
                    ShowMessage(ConfigurationManager.AppSettings["Error_SelectCategory"]);
                    return false;
                }
                if (ddlImportance == null || string.IsNullOrEmpty(ddlImportance.SelectedValue))
                {
                    ShowMessage(ConfigurationManager.AppSettings["Error_SelectImportance"]);
                    return false;
                }
                if (ddlDatasource == null || string.IsNullOrEmpty(ddlDatasource.SelectedValue))
                {
                    ShowMessage(ConfigurationManager.AppSettings["Error_SelectDataSource"]);
                    return false;
                }
                if (ddlDatasource.SelectedItem.Text.Equals("other", StringComparison.OrdinalIgnoreCase) &&
                    string.IsNullOrWhiteSpace(txtOtherSource.Text))
                {
                    ShowMessage(ConfigurationManager.AppSettings["Error_EnterOtherSource"]);
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            return false;
        }
    }


    private bool ValidateGuardrails(out string message)
    {
        message = string.Empty;
        try
        {
            int totalSelected = rptKPA.Items.Count;

            // Validate only maximum KPA limit
            int maxKpaLimit = Convert.ToInt32(ConfigurationManager.AppSettings["MaxKPALimit"]);
            if (totalSelected > maxKpaLimit)
            {
                message = ConfigurationManager.AppSettings["Error_MaxKPA"];
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    $"alert('{message}');", true);
                return false;
            }

            // Count importance levels
            int hCount = 0, mCount = 0, lCount = 0;

            foreach (RepeaterItem item in rptKPA.Items)
            {
                DropDownList ddlImportance = item.FindControl("ddlImportance") as DropDownList;

                if (ddlImportance != null && !string.IsNullOrEmpty(ddlImportance.SelectedItem.Text))
                {
                    string imp = ddlImportance.SelectedItem.Text.Trim().ToLower();
                    if (imp == "high") hCount++;
                    else if (imp == "medium" || imp == "mid") mCount++;
                    else if (imp == "low") lCount++;
                }
            }

            bool validImportance = true;

            //  Apply importance rule only if section is visible
            switch (totalSelected)
            {
                case 1:
                    validImportance = (hCount == 1);
                    if (!validImportance)
                        message = ConfigurationManager.AppSettings["Error_Importance_1"];
                    break;

                case 2:
                    validImportance = (hCount >= 2);
                    if (!validImportance)
                        message = ConfigurationManager.AppSettings["Error_Importance_2"];
                    break;

                case 3:
                    validImportance = (hCount >= 2 && mCount >= 1);
                    if (!validImportance)
                        message = ConfigurationManager.AppSettings["Error_Importance_3"];
                    break;

                case 4:
                    validImportance = (hCount >= 2 && mCount >= 1 && lCount >= 1);
                    if (!validImportance)
                        message = ConfigurationManager.AppSettings["Error_Importance_4"];
                    break;

                case 5:
                case 6:
                    validImportance = (hCount >= 2 && mCount >= 1 && lCount >= 1);
                    if (!validImportance)
                        message = ConfigurationManager.AppSettings["Error_Importance_5to6"];
                    break;

                default:
                    if (totalSelected >= 7 && totalSelected <= 10)
                    {
                        validImportance = (hCount >= 2 && mCount >= 2 && lCount >= 2);
                        if (!validImportance)
                            message = ConfigurationManager.AppSettings["Error_Importance_7to10"];
                    }
                    break;
            }


            if (!validImportance)
            {
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            message = ConfigurationManager.AppSettings["Error_ValidationUnexpected"];

            return false;
        }
    }


    private void ShowMessage(string message, string type = "danger")
    {
        try
        {
            pnlMessage.Visible = true;
            alertBox.Attributes["class"] = $"alert alert-{type}";
            alertBox.InnerText = message;
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }


    public void BindDropdown(DropDownList ddl, DataTable dt, string textField, string valueField, bool addSelect = true)
    {
        try
        {
            ddl.Items.Clear();
            if (addSelect)
                ddl.Items.Add(new ListItem("--Select--", ""));

            foreach (DataRow row in dt.Rows)
            {
                if (row[valueField] != DBNull.Value && row[textField] != DBNull.Value)
                    ddl.Items.Add(new ListItem(row[textField].ToString(), row[valueField].ToString()));
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }


}
