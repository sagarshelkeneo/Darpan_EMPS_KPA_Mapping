<%@ Page Title="Add Role - Step 1" Language="C#" MasterPageFile="~/KpaMaster.Master" AutoEventWireup="true" CodeFile="AddRole.aspx.cs" Inherits="AddRole" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    KPA Co-Creation Portal
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container my-4">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Index.aspx" CssClass="btn btn-sm btn-outline-secondary me-2">Back</asp:HyperLink>
        </div>

        <div class="card shadow-sm p-4">
            <h5 class="mb-4">Add Role - Step 1 of 2</h5>

            <!-- Works Dropdown -->
            <div class="mb-2">
                <label class="form-label">Works / Non-Works<span class="required-star">*</span></label>
                <asp:DropDownList ID="ddlWorks" runat="server" CssClass="form-select" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlWorks_SelectedIndexChanged" />
                <asp:RequiredFieldValidator ID="rfvWorks" runat="server" ControlToValidate="ddlWorks"
                    InitialValue="0" CssClass="text-danger" Display="Dynamic" />
            </div>

            <!-- Shop -->
            <div class="mb-3" id="shopDiv" runat="server">
                <asp:Label ID="lblShop" runat="server" CssClass="form-label">Shop<span class="required-star">*</span></asp:Label>
                <asp:DropDownList ID="ddlShop" runat="server" CssClass="form-select" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlShop_SelectedIndexChanged" />
                <asp:TextBox ID="txtOtherShop" runat="server" CssClass="form-control mt-2"
                    Placeholder="Enter Other Shop" Visible="false" />
                <asp:CustomValidator ID="cvShop" runat="server" ControlToValidate="ddlShop"
                    OnServerValidate="cvShop_ServerValidate"
                    CssClass="text-danger" Display="Dynamic" />
            </div>


            <!-- Role Type -->
            <div class="mb-2">
                <label class="form-label">Role Type<span class="required-star">*</span></label>
                <asp:DropDownList ID="ddlRoleType" runat="server" CssClass="form-select" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlRoleType_SelectedIndexChanged" />
                <asp:RequiredFieldValidator ID="rfvRoleType" runat="server" ControlToValidate="ddlRoleType"
                    InitialValue="0" CssClass="text-danger" Display="Dynamic" />
            </div>


            <!-- Function Dropdown -->
            <div id="functionDiv" runat="server" visible="false" class="mb-3">
                <label class="form-label">Function<span class="required-star">*</span></label>
                <asp:DropDownList ID="ddlFunction" runat="server" CssClass="form-select" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlFunction_SelectedIndexChanged" />
                <asp:TextBox ID="txtOtherFunction" runat="server" CssClass="form-control mt-2"
                    Placeholder="Enter Other Function" Visible="false" />
                <asp:CustomValidator ID="cvFunction" runat="server" ControlToValidate="ddlFunction"
                    OnServerValidate="cvFunction_ServerValidate"
                    CssClass="text-danger" Display="Dynamic" />
            </div>

            <!-- Section -->
            <asp:Panel ID="sectionPanel" runat="server" Visible="false" CssClass="mb-3">
                <asp:Label ID="lblSection" runat="server" CssClass="form-label">Section<span class="required-star">*</span></asp:Label>
                <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-select" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlSection_SelectedIndexChanged" />
                <asp:TextBox ID="txtOtherSection" runat="server" CssClass="form-control mt-2"
                    Placeholder="Enter Other Section" Visible="false" />
                <asp:CustomValidator ID="cvSection" runat="server" ControlToValidate="ddlSection"
                    OnServerValidate="cvSection_ServerValidate"
                    CssClass="text-danger" Display="Dynamic" />
            </asp:Panel>

            <!-- Role Name -->
            <div class="mb-3">
                <label class="form-label">Role Name<span class="required-star">*</span></label>
                <asp:TextBox ID="txtRoleName" runat="server" CssClass="form-control" Placeholder="Enter Role Name" />
                <asp:RequiredFieldValidator ID="rfvRoleName" runat="server" ControlToValidate="txtRoleName"
                    CssClass="text-danger" Display="Dynamic" />
            </div>

            <!-- Next Button -->
            <div class="d-flex justify-content-end">
                <asp:Button ID="btnNext" runat="server" Text="Next ➤" CssClass="btn btn-primary" OnClick="btnNext_Click" />
            </div>
        </div>
    </div>

</asp:Content>
