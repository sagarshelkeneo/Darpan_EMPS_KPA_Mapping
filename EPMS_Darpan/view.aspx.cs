using DBService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class view : Page
{
    private readonly ViewService _kpaService = new ViewService();
    private readonly ErrorLog _errorLog = new ErrorLog();

    public int roleId = 0;
    public int empId = 0;
    public int isSubmitted = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {

                string encryptedData = Request.QueryString["data"];
                if (!string.IsNullOrEmpty(encryptedData))
                {
                    string decrypted = EncryptionHelper.Decrypt(encryptedData);
                    var queryParams = System.Web.HttpUtility.ParseQueryString(decrypted);

                    roleId = Convert.ToInt32(queryParams["RoleID"]);
                    isSubmitted = Convert.ToInt32(queryParams["isSubmitted"]);

                    // Use roleId and isSubmitted as needed
                }


                if (roleId == 0)
                {
                    Response.Redirect("Index.aspx", false);
                    return;
                }



                BindRole(roleId);

                string status;
                empId = Session["EmpID"] != null ? Convert.ToInt32(Session["EmpID"]) : 0;

                DataTable dtKpa = _kpaService.GetKpaRoleMapping(roleId, empId, out status);

                if (isSubmitted == 1)
                    BindReadonlyKPA(dtKpa);
                else
                    BindKPA(roleId);

                DetermineApplicableGuardrails();

                ValidateGuardrails();
            }

            // Register dynamic controls for async updates
            RegisterAsyncControls();
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Page Load Error: {ex.Message}');", true);
        }
    }

    private void BindKPA(int roleId)
    {
        try
        {
            DataTable dt = _kpaService.GetKpaMaster(roleId, empId);
            rptKPA.DataSource = dt;
            rptKPA.DataBind();
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error fetching KPAs: {ex.Message}');", true);
        }
    }

    private void BindRole(int roleId)
    {
        try
        {
            DataTable dt = _kpaService.GetRoleById(roleId);
            if (dt.Rows.Count > 0)
            {
                lblRole.Text = dt.Rows[0]["ROLEHEADING"].ToString();
                lblStatus.Text = Session["status"].ToString();
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error fetching role info: {ex.Message}');", true);
        }
    }

    private void RegisterAsyncControls()
    {
        try
        {
            foreach (RepeaterItem item in rptKPA.Items)
            {
                if (item.ItemType != ListItemType.Item && item.ItemType != ListItemType.AlternatingItem)
                    continue;

                foreach (string ctrlId in new[] { "chkKPA", "rbHigh", "rbMed", "rbLow" })
                {
                    var ctrl = item.FindControl(ctrlId);
                    if (ctrl != null)
                        ScriptManager.GetCurrent(this).RegisterAsyncPostBackControl(ctrl);
                }
            }
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
                RadioButton rbHigh = e.Item.FindControl("rbHigh") as RadioButton;
                RadioButton rbMed = e.Item.FindControl("rbMed") as RadioButton;
                RadioButton rbLow = e.Item.FindControl("rbLow") as RadioButton;
                HiddenField hfImportance = e.Item.FindControl("hfImportance") as HiddenField;

                if (hfImportance != null)
                {
                    switch (hfImportance.Value)
                    {
                        case "1": if (rbHigh != null) rbHigh.Checked = true; break;
                        case "2": if (rbMed != null) rbMed.Checked = true; break;
                        case "3": if (rbLow != null) rbLow.Checked = true; break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    /// <summary>
    /// Determines which guardrail rules should apply based on total KPA data.
    /// </summary>
    /// 
    public string ShowKPAImp()
    {

        int totalKpaCount = rptKPA.Items.Count;

        //// --- Set importance rule message dynamically ---
        string importanceMsg = string.Empty;
        switch (totalKpaCount)
        {
            case 1:
                importanceMsg += "H=1";
                break;

            case 2:
                importanceMsg += "H=2";
                break;

            case 3:
                importanceMsg += "H=2 M=1";
                break;

            case 4:
                importanceMsg += "H=2 M=1 L=1";
                break;

            case 5:
            case 6:
                importanceMsg += "H=2 M≥1 L≥1";
                break;

            case 7:
            case 8:
            case 9:
            case 10:
                importanceMsg += "H≥2 M≥2 L≥2";
                break;

            default:
                importanceMsg += "H≥2 M≥1 L≥1";
                break;
        }
        return importanceMsg;


    }
    private void DetermineApplicableGuardrails()
    {
        try
        {
            // Get total KPAs bound to repeater
            int totalKpaCount = rptKPA.Items.Count;
            bool hasSafety = false;

            // Check if any KPA belongs to Safety category
            foreach (RepeaterItem item in rptKPA.Items)
            {
                HiddenField hfCategory = item.FindControl("hfCategory") as HiddenField;
                if (hfCategory != null && hfCategory.Value.Trim().Equals("Safety", StringComparison.OrdinalIgnoreCase))
                {
                    hasSafety = true;
                    break;
                }
            }

            // --- Guardrail visibility control ---

            // 1️⃣ Show Min/Max KPA guardrail only if there are >= 5 KPAs
            grMinMax.Visible = totalKpaCount >= 5;

            // 2️⃣ Show Safety guardrail only if Safety category exists
            grSafety.Visible = hasSafety;

            // 3️⃣ Category coverage always visible
            grCategory.Visible = true;

            // 4️⃣ Importance rule is always visible, but text depends on count
            grImportance.Visible = true;


            //Optional: reset the label counts if first load
            lblImportance.InnerText = "H:0 M:0 L:0";

            // Update panel for immediate UI reflection
            upGuardrails.Update();
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    protected void ValidateGuardrails(object sender = null, EventArgs e = null)
    {
        try
        {


            // --- Load Configuration ---
            int minKpaLimit = Convert.ToInt32(ConfigurationManager.AppSettings["New_MinKpaLimit"]);
            int maxKpaLimit = Convert.ToInt32(ConfigurationManager.AppSettings["New_MaxKpaLimit"]);
            int minSafetyKpa = Convert.ToInt32(ConfigurationManager.AppSettings["New_MinSafetyKpa"]);
            int maxSafetyKpa = Convert.ToInt32(ConfigurationManager.AppSettings["New_MaxSafetyKpa"]);

            int totalSelected = 0, hCount = 0, mCount = 0, lCount = 0;
            Dictionary<string, int> categoryCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            // --- Collect categories dynamically ---
            foreach (RepeaterItem item in rptKPA.Items)
            {
                HiddenField hfCategory = item.FindControl("hfCategory") as HiddenField;
                if (hfCategory != null && !categoryCounts.ContainsKey(hfCategory.Value.Trim()))
                    categoryCounts[hfCategory.Value.Trim()] = 0;
            }

            // --- Count selected KPAs ---
            foreach (RepeaterItem item in rptKPA.Items)
            {
                CheckBox chk = item.FindControl("chkKPA") as CheckBox;
                RadioButton rbHigh = item.FindControl("rbHigh") as RadioButton;
                RadioButton rbMed = item.FindControl("rbMed") as RadioButton;
                RadioButton rbLow = item.FindControl("rbLow") as RadioButton;
                HiddenField hfCategory = item.FindControl("hfCategory") as HiddenField;

                if (chk != null && chk.Checked && hfCategory != null)
                {
                    totalSelected++;
                    categoryCounts[hfCategory.Value.Trim()]++;

                    if (rbHigh?.Checked == true) hCount++;
                    if (rbMed?.Checked == true) mCount++;
                    if (rbLow?.Checked == true) lCount++;
                }
            }

            // --- Update UI counts ---
            lblSelected.InnerText = $"Selected: {totalSelected}";

            bool validMinMax = false, validSafety = false, validCategory = false, validImportance = false;

            // --- 1️⃣ Min/Max KPA Rule ---
            if (grMinMax.Visible)
            {
                lblMinMax.InnerText = $"{totalSelected}/{maxKpaLimit}";
                validMinMax = totalSelected >= Math.Min(minKpaLimit, rptKPA.Items.Count) && totalSelected <= maxKpaLimit;
                grMinMax.Attributes["class"] = validMinMax ? "guardrail-item greenData" : "guardrail-item redData";
            }

            // --- 2️⃣ Safety KPA Rule ---
            if (grSafety.Visible)
            {
                int safetyCount = categoryCounts.ContainsKey("Safety") ? categoryCounts["Safety"] : 0;
                lblSafety.InnerText = safetyCount.ToString();
                validSafety = safetyCount >= minSafetyKpa && safetyCount <= maxSafetyKpa;
                grSafety.Attributes["class"] = validSafety ? "guardrail-item greenData" : "guardrail-item redData";
            }

            // --- 3️⃣ Category Coverage ---
            if (grCategory.Visible)
            {
                List<string> categoryStatus = new List<string>();
                bool allCatsHaveOne = true;
                foreach (var cat in categoryCounts)
                {
                    categoryStatus.Add($"{cat.Key}:{cat.Value}");
                    if (cat.Value == 0) allCatsHaveOne = false;
                }

                lblCategory.InnerText = string.Join(" • ", categoryStatus);
                if (categoryCounts.Count > 0)
                {
                    validCategory = allCatsHaveOne;
                }
                grCategory.Attributes["class"] = validCategory ? "guardrail-item greenData" : "guardrail-item redData";
            }


            //// --- Set importance rule message dynamically ---

            // --- 4️⃣ Importance Guardrail ---

            if (grImportance.Visible)
            {
                string importanceMsg = "KPA Importance: ";

                // Only validate based on existing rule (don’t overwrite text)
                switch (totalSelected)
                {
                    case 1:
                        importanceMsg += "H=1";
                        validImportance = (hCount == 1);
                        break;
                    case 2:
                        importanceMsg += "H=2";

                        validImportance = (hCount >= 2);
                        break;
                    case 3:
                        importanceMsg += "H=2 M=1";

                        validImportance = (hCount >= 2 && mCount >= 1);
                        break;
                    case 4:
                        importanceMsg += "H=2 M=1 L=1";

                        validImportance = (hCount >= 2 && mCount >= 1 && lCount >= 1);
                        break;
                    case 5:
                    case 6:
                        importanceMsg += "H=2 M≥1 L≥1";

                        validImportance = (hCount >= 2 && mCount >= 1 && lCount >= 1);
                        break;
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        if (totalSelected >= 7 && totalSelected <= 10)
                        {
                            importanceMsg += "H≥2 M≥2 L≥2";

                            validImportance = (hCount >= 2 && mCount >= 2 && lCount >= 2);
                        }
                        break;
                    default:
                        importanceMsg += ShowKPAImp();
                        break;
                }

                lblKPAImportance.InnerText = importanceMsg;

                lblImportance.InnerText = $"H:{hCount} M:{mCount} L:{lCount}";
                grImportance.Attributes["class"] = validImportance ? "guardrail-item greenData" : "guardrail-item redData";
            }

            // --- Enable/Disable Submit ---
            bool allGreen =
                (!grMinMax.Visible || validMinMax) &&
                (!grSafety.Visible || validSafety) &&
                (!grCategory.Visible || validCategory) &&
                (!grImportance.Visible || validImportance);

            string script = allGreen
                ? $"document.getElementById('{btnSubmit.ClientID}').removeAttribute('disabled');"
                : $"document.getElementById('{btnSubmit.ClientID}').setAttribute('disabled','disabled');";

            ScriptManager.RegisterStartupScript(this, GetType(), "EnableSubmitBtn_New", script, true);
            upGuardrails.Update();
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                $"alert('Error validating guardrails: {ex.Message.Replace("'", "\\'")}');", true);
        }
    }





    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            ValidateGuardrails();

            //if (string.IsNullOrWhiteSpace(txtComment.Text))
            //{
            //    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Comment cannot be empty.');", true);
            //    return;
            //}



            try
            {
                bool isValid = true;
                string invalidKPAName = string.Empty;

                foreach (RepeaterItem item in rptKPA.Items)
                {
                    // Find controls
                    CheckBox chk = item.FindControl("chkKPA") as CheckBox;
                    RadioButton rbHigh = item.FindControl("rbHigh") as RadioButton;
                    RadioButton rbMed = item.FindControl("rbMed") as RadioButton;
                    RadioButton rbLow = item.FindControl("rbLow") as RadioButton;
                    Label lblKPAName = item.FindControl("lblKPAName") as Label; // Optional if you have one
                    HiddenField hfKPAName = item.FindControl("hfCategory") as HiddenField; // Or use category/KPA field

                    // Validate only checked rows
                    if (chk != null && chk.Checked)
                    {
                        bool anySelected = (rbHigh != null && rbHigh.Checked) ||
                                           (rbMed != null && rbMed.Checked) ||
                                           (rbLow != null && rbLow.Checked);

                        if (!anySelected)
                        {
                            isValid = false;

                            // Capture which KPA failed (optional but helpful)
                            if (lblKPAName != null)
                                invalidKPAName = lblKPAName.Text;
                            else if (hfKPAName != null)
                                invalidKPAName = hfKPAName.Value;

                            break;
                        }
                    }
                }

                if (!isValid)
                {
                    string message = string.IsNullOrEmpty(invalidKPAName)
                        ? "Please select importance (H/M/L) for all selected KPAs."
                        : $"Please select importance (H/M/L) for the selected KPA: \"{invalidKPAName}\".";

                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{message}');", true);
                    return;
                }
            }
            catch (Exception ex)
            {
                _errorLog.LogError(ex);
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    $"alert('Error while saving: {ex.Message.Replace("'", "\\'")}');", true);
            }



            List<object> kpaMappings = new List<object>();
            foreach (RepeaterItem item in rptKPA.Items)
            {
                if (item.ItemType != ListItemType.Item && item.ItemType != ListItemType.AlternatingItem)
                    continue;

                HiddenField hfKPAId = item.FindControl("hfKPAId") as HiddenField;
                RadioButton rbHigh = item.FindControl("rbHigh") as RadioButton;
                RadioButton rbMed = item.FindControl("rbMed") as RadioButton;
                RadioButton rbLow = item.FindControl("rbLow") as RadioButton;
                Label lblUnitOfKPA = item.FindControl("lblUnitOfKPA") as Label;
                TextBox txtDataSource = item.FindControl("txtDataSource") as TextBox;

                int importanceId = rbHigh?.Checked == true ? 1 :
                                   rbMed?.Checked == true ? 2 :
                                   rbLow?.Checked == true ? 3 : 0;

                if (hfKPAId == null || importanceId == 0) continue;

                int empId = Session["EmpID"] != null ? Convert.ToInt32(Session["EmpID"]) : 0;
                // int roleId = Request.QueryString["RoleID"] != null ? Convert.ToInt32(Request.QueryString["RoleID"]) : 0;

                int roleId = 0;
                string encryptedData = Request.QueryString["data"];
                if (!string.IsNullOrEmpty(encryptedData))
                {
                    string decrypted = EncryptionHelper.Decrypt(encryptedData);
                    var queryParams = System.Web.HttpUtility.ParseQueryString(decrypted);

                    roleId = Convert.ToInt32(queryParams["RoleID"]);
                    // Use roleId and isSubmitted as needed
                }

                kpaMappings.Add(new
                {
                    KpaID = Convert.ToInt32(hfKPAId.Value),
                    RoleID = roleId,
                    EmpID = empId,
                    ImportanceID = importanceId,
                    UnitOfKPA = lblUnitOfKPA?.Text,
                    DataSource = txtDataSource?.Text,
                    Comments = txtComment.Text
                });
            }

            if (kpaMappings.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Please select at least one KPA.');", true);
                return;
            }

            string jsonPayload = JsonConvert.SerializeObject(new { KPAMAPPING = kpaMappings });
            _kpaService.SaveKpaMap(jsonPayload);

            BindKPA(roleId);
            ValidateGuardrails();

            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('KPA Signoff saved successfully.');", true);
            // After successful login
            HttpCookie cookie = new HttpCookie("Message");
            cookie.Value = "KPA Mapping Successful!";
            cookie.Expires = DateTime.Now.AddSeconds(2); // optional, short-lived
            Response.Cookies.Add(cookie);

            Response.Redirect("index.aspx?msg=KPA mapped successfully!", false);

        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error while saving: {ex.Message}');", true);
        }
    }


    protected bool CheckedKPAImportanceValidation()
    {

        return false;

    }

    private void BindReadonlyKPA(DataTable dt)
    {
        try
        {
            if (dt.Rows.Count > 0)
                txtComment.Text = dt.Rows[0]["Comments"]?.ToString() ?? string.Empty;

            rptKPA.DataSource = dt;
            rptKPA.DataBind();

            foreach (RepeaterItem item in rptKPA.Items)
            {
                if (item.ItemType != ListItemType.Item && item.ItemType != ListItemType.AlternatingItem)
                    continue;

                DisableKpaControls(item, dt.Rows[item.ItemIndex]);
            }

            txtComment.ReadOnly = true;
            btnSubmit.Visible = false;
            MarkAllGuardrailsGreen();
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error loading readonly KPAs: {ex.Message}');", true);
        }
    }

    private void DisableKpaControls(RepeaterItem item, DataRow row)
    {
        CheckBox chk = item.FindControl("chkKPA") as CheckBox;
        RadioButton rbHigh = item.FindControl("rbHigh") as RadioButton;
        RadioButton rbMed = item.FindControl("rbMed") as RadioButton;
        RadioButton rbLow = item.FindControl("rbLow") as RadioButton;
        Label lblDataSource = item.FindControl("lblDataSource") as Label;
        TextBox txtDataSource = item.FindControl("txtDataSource") as TextBox;
        Label lblUnitOfKPA = item.FindControl("lblUnitOfKPA") as Label;
        PlaceHolder phEditBtn = item.FindControl("phEditBtn") as PlaceHolder;

        if (chk != null) { chk.Checked = true; chk.Enabled = false; }
        if (rbHigh != null) rbHigh.Enabled = false;
        if (rbMed != null) rbMed.Enabled = false;
        if (rbLow != null) rbLow.Enabled = false;
        if (phEditBtn != null) phEditBtn.Visible = false;

        txtDataSource.Visible = false;
        lblDataSource.Text = row["DataSource"]?.ToString() ?? "";
        lblUnitOfKPA.Text = row["UnitOfKPA"]?.ToString() ?? "";

        int importanceId = row["ImportanceID"] != DBNull.Value ? Convert.ToInt32(row["ImportanceID"]) : 0;
        switch (importanceId)
        {
            case 1: rbHigh.Checked = true; break;
            case 2: rbMed.Checked = true; break;
            case 3: rbLow.Checked = true; break;
        }



    }

    private void MarkAllGuardrailsGreen()
    {
        lblSelected.InnerText = $"Selected: {rptKPA.Items.Count}";
        lblMinMax.InnerText = $"{rptKPA.Items.Count}/10";
        grMinMax.Attributes["class"] = "guardrail-item greenData";
        lblSafety.InnerText = "1";
        grSafety.Attributes["class"] = "guardrail-item greenData";
        lblCategory.InnerText = "Quality:1 • Safety:1 • Cost:1 • Production:1";
        grCategory.Attributes["class"] = "guardrail-item greenData";
        lblImportance.InnerText = "H:2 M:1 L:1";
        grImportance.Attributes["class"] = "guardrail-item greenData";
        upGuardrails.Update();
    }
}


