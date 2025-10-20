<%@ Page Title="KPA Sign-off" Language="C#" MasterPageFile="~/KpaMaster.Master" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    KPA Co-Creation Portal
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container my-4">

        <div class="d-flex justify-content-between align-items-center mb-3">
            <asp:HiddenField ID="hfInstructionState" runat="server" Value="show" />


            <asp:Button ID="btnToggleInstructions" runat="server"
                CssClass="btn btn-sm btn-outline-secondary me-2"
                Text="Hide / Show Instructions"
                OnClick="btnToggleInstructions_Click" />

            <div>
                <asp:HyperLink ID="lnkAddRole" runat="server" NavigateUrl="~/AddRole.aspx" CssClass="btn btn-sm btn-primary">Suggest to Add Role</asp:HyperLink>
            </div>
        </div>

        <!-- Instructions -->
        <div class='<%# "collapse " + hfInstructionState.Value %>' id="instructionInfo" runat="server">

            <div class="instructionInfo">
                <h6 class="section-title">Instructions</h6>
                <ul class="instructions-list">
                    <li>Review the roles and KPAs related to your subordinates.</li>
                    <li>Select a role to view its associated KPAs.</li>
                    <li>Choose the most relevant KPAs and mark importance level of each selected KPA.</li>
                </ul>

                <h6 class="section-title">Guardrails</h6>
                <ul class="guardrails-list">
                    <li>Select at least <strong>5 KPAs</strong>, or the entire list if fewer than 5 are available.</li>
                    <li>Select no more than <strong>10 KPAs</strong>.</li>
                    <li>Include <strong>1–2 safety KPAs</strong>.</li>
                    <li>Select at least <strong>one KPA from each visible category</strong>.</li>
                </ul>

                <h6 class="section-title">Add New Roles</h6>
                <p>
                    If you feel a role is missing, click the
                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/AddRole.aspx" CssClass="btn btn-sm btn-primary">Suggest to Add Role</asp:HyperLink>
                    button (top right) to add it.
                </p>

                <h6 class="section-title">Review Access</h6>
                <ul class="guardrails-list">
                    <li><strong>EDs</strong> can review Shop Head roles under their purview.</li>
                    <li><strong>Shop Heads</strong> can review Function Heads roles (e.g. Shop Ops Head, Shop Electrical Maintenance Head, etc.)</li>
                    <li><strong>Non-Works Department Heads</strong> can review all roles in their department.</li>
                    <li><strong>Function Heads (per shop)</strong> can review Shop Section Heads and Shift In-charges relevant to their function.</li>
                    <li><strong>DSO</strong> can review all safety roles across all shops.</li>
                </ul>

                <h6 class="section-title-new">All the suggestions will go through a review process before final incorporation.</h6>
            </div>
        </div>

        <!-- Search & Filters -->
        <div class="row mb-3">
            <div class="col-md-4">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search" OnTextChanged="txtSearch_TextChanged" AutoPostBack="true"></asp:TextBox>
            </div>
            <div class="col-md-3">
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged" AutoPostBack="true">
                    <asp:ListItem Value="">Status (all)</asp:ListItem>
                    <asp:ListItem Value="NOT STARTED">Not started</asp:ListItem>
                    <asp:ListItem Value="SUBMITTED">Submitted</asp:ListItem>
                    <asp:ListItem Value="MANUALLY ADDED">Manually added</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-5 d-flex align-items-center justify-content-end">
                <h6 class="mb-0 text-end">Summary : Submitted, Manually Added&nbsp;<asp:Label ID="lblMessage" runat="server" Text=""></asp:Label></h6>
            </div>
        </div>

        <!-- Table -->
        <div class="table-responsive">
            <asp:GridView ID="gvRoles" runat="server" AutoGenerateColumns="false"
                AllowPaging="true" PageSize="10"
                OnPageIndexChanging="gvRoles_PageIndexChanging"
                EmptyDataText="No records found."
                OnRowCommand="gvRoles_RowCommand"
                OnRowDataBound="gvRoles_RowDataBound"
                CssClass="table table-bordered table-striped">

                <Columns>
                    <asp:BoundField DataField="SHOPNAME" HeaderText="Shop" />
                    <asp:BoundField DataField="SECTIONNAME" HeaderText="Section" />
                    <asp:BoundField DataField="RoleName" HeaderText="Role Name" />
                    <asp:BoundField DataField="Status" HeaderText="Status" />

                    <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Button ID="btnAction" runat="server"
                                Text='<%# (Eval("Status") != null && Eval("Status").ToString().ToUpper() == "NOT STARTED") ? "Review" : "View" %>'
                                CommandName="ActionRole"
                                CommandArgument='<%# Eval("RoleID") + "|" + (Eval("Status") ?? "") %>'
                                CssClass='<%# (Eval("Status") != null && Eval("Status").ToString().ToUpper() == "NOT STARTED") ? "btn btn-primary btn-sm" : "btn btn-primary btn-sm" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

    </div>

    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.js"></script>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.css" rel="stylesheet" />

    <script>
        $(document).ready(function () {
            // Read the cookie value
            function getCookie(name) {
                let match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
                if (match) return decodeURIComponent(match[2]);
                return null;
            }

            var message = getCookie('Message');

            if (message) {
                toastr.success(message);

                // Delete the cookie so it shows only once
                document.cookie = "LoginMessage=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
            }
        });
    </script>

</asp:Content>
