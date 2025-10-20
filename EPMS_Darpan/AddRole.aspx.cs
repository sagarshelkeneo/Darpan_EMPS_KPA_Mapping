using AjaxControlToolkit;
using DBService;
using DBService.Model;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;

public partial class AddRole : System.Web.UI.Page
{
    private readonly AddRoleService _addroleService = new AddRoleService();
    public const string Other = "other";
    private readonly ErrorLog _errorLog = new ErrorLog();


    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                SetValidationMessages();
                BindWorksDropdown();
                functionDiv.Visible = false;
                sectionPanel.Visible = false;
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    private void SetValidationMessages()
    {
        // Works
        rfvWorks.ErrorMessage = ConfigurationManager.AppSettings["Error_SelectWorks"];

        // Role Type
        rfvRoleType.ErrorMessage = ConfigurationManager.AppSettings["Error_SelectRoleType"];

        // Shop
        cvShop.ErrorMessage = ConfigurationManager.AppSettings["Error_SelectShop"];

        // Function
        cvFunction.ErrorMessage = ConfigurationManager.AppSettings["Error_SelectFunction"];

        // Section
        cvSection.ErrorMessage = ConfigurationManager.AppSettings["Error_SelectSection"];

        // Role Name
        rfvRoleName.ErrorMessage = ConfigurationManager.AppSettings["Error_RoleName"];
    }

    #region Dropdown Binding

    private void BindWorksDropdown()
    {
        try
        {
            var dt = _addroleService.GetWorksData();
            ddlWorks.DataSource = dt;
            ddlWorks.DataTextField = "ROLEWORKSNAME";
            ddlWorks.DataValueField = "ROLEWORKSID";
            ddlWorks.DataBind();
            ddlWorks.Items.Insert(0, new ListItem("--Select--", "0"));
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    private void BindRoleTypeDropdown(int roleWorksId)
    {
        try
        {
            var dt = _addroleService.GetRoleTypeData(roleWorksId);
            ddlRoleType.DataSource = dt;
            ddlRoleType.DataTextField = "ROLETYPENAME";
            ddlRoleType.DataValueField = "ROLETYPEID";
            ddlRoleType.DataBind();
            ddlRoleType.Items.Insert(0, new ListItem("--Select Role Type--", "0"));
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    private void BindShopDropdown(int? roleWorksId = null)
    {
        try
        {
            ddlShop.Items.Clear();
            txtOtherShop.Visible = false;

            var dt = _addroleService.GetShopData(roleWorksId);
            if (dt.Rows.Count > 0)
            {
                ddlShop.DataSource = dt;
                ddlShop.DataTextField = "ShopName";
                ddlShop.DataValueField = "ShopID";
                ddlShop.DataBind();
                ddlShop.Items.Insert(0, new ListItem("--Select Shop--", "0"));
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    private void BindFunctionDropdown(int? functionId = null)
    {
        try
        {
            ddlFunction.Items.Clear();

            var dt = _addroleService.GetFunctionData(functionId);
            if (dt.Rows.Count > 0)
            {
                ddlFunction.DataSource = dt;
                ddlFunction.DataTextField = "FUNCTIONNAME";
                ddlFunction.DataValueField = "FUNCTIONID";
                ddlFunction.DataBind();
                ddlFunction.Items.Insert(0, new ListItem("--Select Function--", "0"));
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    private void BindSectionDropdown(int shopId)
    {
        try
        {
            var dt = _addroleService.GetSectionData(shopId);
            ddlSection.DataSource = dt;
            ddlSection.DataTextField = "SECTIONNAME";
            ddlSection.DataValueField = "SECTIONID";
            ddlSection.DataBind();
            ddlSection.Items.Insert(0, new ListItem("--Select Section--", "0"));
            sectionPanel.Visible = true;
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    #endregion

    #region Server Side Validation
    protected void cvShop_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            if (ddlShop.Visible)
            {
                if (ddlShop.SelectedItem.Text == Other)
                {
                    args.IsValid = !string.IsNullOrWhiteSpace(txtOtherShop.Text);
                }
                else
                {
                    args.IsValid = ddlShop.SelectedValue != "0";
                }
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            args.IsValid = false;
        }
    }

    protected void cvFunction_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            if (ddlFunction.Visible)
            {
                if (ddlFunction.SelectedItem.Text == Other)
                {
                    args.IsValid = !string.IsNullOrWhiteSpace(txtOtherFunction.Text);
                }
                else
                {
                    args.IsValid = ddlFunction.SelectedValue != "0";
                }
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            args.IsValid = false;
        }
    }

    protected void cvSection_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            if (ddlSection.Visible)
            {
                if (ddlSection.SelectedItem.Text == Other)
                {
                    args.IsValid = !string.IsNullOrWhiteSpace(txtOtherSection.Text);
                }
                else
                {
                    args.IsValid = ddlSection.SelectedValue != "0";
                }
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
            args.IsValid = false;
        }
    }
    #endregion

    #region Events

    protected void ddlWorks_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            HideDiv();
            int worksId = int.TryParse(ddlWorks.SelectedValue, out int wId) ? wId : 0;
            BindRoleTypeDropdown(worksId);
            BindShopDropdown(worksId);
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    private void HideDiv()
    {
        try
        {
            sectionPanel.Visible = false;
            functionDiv.Visible = false;
            ddlFunction.Items.Clear();
            ddlSection.Items.Clear();
            txtOtherFunction.Visible = false;
            txtOtherSection.Visible = false;
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    private void SetRoleFlagsInViewState(int roleTypeId)
    {
        try
        {
            var flags = _addroleService.FetchRoleFlags(roleTypeId);
            ViewState["IsFunctionApplicable"] = flags?.IsFunctionApplicable;
            ViewState["IsSectionApplicable"] = flags?.IsSectionApplicable;
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    protected void ddlRoleType_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            HideDiv();
            int roleTypeId = int.TryParse(ddlRoleType.SelectedValue, out int rId) ? rId : 0;
            if (roleTypeId > 0)
            {
                SetRoleFlagsInViewState(roleTypeId);

                if (string.Equals(ViewState["IsFunctionApplicable"] as string, "Y", StringComparison.OrdinalIgnoreCase))
                {
                    functionDiv.Visible = true;
                    BindFunctionDropdown(null);
                }

                if (string.Equals(ViewState["IsSectionApplicable"] as string, "Y", StringComparison.OrdinalIgnoreCase))
                {
                    int shopId = int.TryParse(ddlShop.SelectedValue, out int sId) ? sId : 0;
                    BindSectionDropdown(shopId);
                }
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    protected void ddlShop_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            txtOtherShop.Visible = ddlShop.SelectedItem?.Text.ToLower() == Other;

            if (string.Equals(ViewState["IsSectionApplicable"] as string, "Y", StringComparison.OrdinalIgnoreCase) && !txtOtherShop.Visible)
            {
                int shopId = int.TryParse(ddlShop.SelectedValue, out int sId) ? sId : 0;
                BindSectionDropdown(shopId);
            }
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    protected void ddlFunction_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            txtOtherFunction.Visible = ddlFunction.SelectedItem?.Text.ToLower() == Other;
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    protected void ddlSection_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            txtOtherSection.Visible = ddlSection.SelectedItem?.Text.ToLower() == Other;
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        try
        {
            Response.Redirect("Index.aspx", false);
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        try
        {
            Page.Validate();
            if (!Page.IsValid) return;

            RoleMaster role = new RoleMaster
            {
                WorkId = ddlWorks.SelectedValue != "" ? Convert.ToInt32(ddlWorks.SelectedValue) : (int?)null,
                RoleTypeId = ddlRoleType.SelectedValue != "" ? Convert.ToInt32(ddlRoleType.SelectedValue) : (int?)null,
                RoleName = txtRoleName.Text.Trim(),
                RoleCode = txtRoleName.Text.Trim(),
                ShopId = ddlShop.SelectedValue != "" ? Convert.ToInt32(ddlShop.SelectedValue) : (int?)null,
                ShopName = txtOtherShop.Visible ? txtOtherShop.Text.Trim() : ddlShop.SelectedItem?.Text,
                FunctionId = ddlFunction.SelectedValue != "" ? Convert.ToInt32(ddlFunction.SelectedValue) : (int?)null,
                FunctionName = txtOtherFunction.Visible ? txtOtherFunction.Text.Trim() : ddlFunction.SelectedItem?.Text,
                SectionId = ddlSection.SelectedValue != "" ? Convert.ToInt32(ddlSection.SelectedValue) : (int?)null,
                SectionName = txtOtherSection.Visible ? txtOtherSection.Text.Trim() : ddlSection.SelectedItem?.Text
            };

            int newRoleId = _addroleService.InsertRole(role);

            string encryptedParams = EncryptionHelper.Encrypt($"roleId={newRoleId}");
            Response.Redirect($"AddKPAs.aspx?data={encryptedParams}", false);

            //Response.Redirect("AddKPAs.aspx?roleId=" + newRoleId,false);
        }
        catch (Exception ex)
        {
            _errorLog.LogError(ex);
        }
    }

    #endregion


}
