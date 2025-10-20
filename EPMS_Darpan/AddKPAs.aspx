<%@ Page Title="Add KPAs - Step 2" Language="C#" MasterPageFile="~/KpaMaster.Master" AutoEventWireup="true" CodeFile="AddKPAs.aspx.cs" Inherits="AddKPAs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
  KPA Co-Creation Portal
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container my-4">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="~/AddRole.aspx" CssClass="btn btn-sm btn-outline-secondary me-2">Back</asp:HyperLink>
        </div>

        <div class="d-flex justify-content-between align-items-center mb-3">
            <h6 class="m-0">Add Role - Step 2 of 2</h6>
            <asp:Button ID="btnAddRow" runat="server" CssClass="btn btn-sm btn-primary" Text="+ Add KPAs" OnClick="btnAddRow_Click" />
        </div>

        <!-- Repeater Table -->
        <asp:Repeater ID="rptKPA" runat="server" OnItemCommand="rptKPA_ItemCommand" OnItemDataBound="rptKPA_ItemDataBound">
            <HeaderTemplate>
                <div class="table-responsive">
                    <table class="table table-bordered align-middle">
                        <thead class="table-light">
                            <tr>
                                <th>KPA Category</th>
                                <th>KPA Name</th>
                                <th>KPA Importance</th>
                                <th>Unit Of KPA</th>
                                <th>Data Source</th>
                                <th>Action</th>
                            </tr>
                        </thead>
                        <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select form-select-sm"></asp:DropDownList>
                    </td>
                    <td>
                        <asp:TextBox ID="txtKPAName" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("KPAName") %>'></asp:TextBox>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlImportance" runat="server" CssClass="form-select form-select-sm">
                            <asp:ListItem Text="Mid" Value="2"></asp:ListItem>
                            <asp:ListItem Text="High" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Low" Value="3"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:TextBox ID="txtUnitOfKPA" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("UnitOfKPA") %>'></asp:TextBox>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlDatasource" runat="server" CssClass="form-select form-select-sm"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlDatasource_SelectedIndexChanged">
                            <asp:ListItem Text="New 1" Value="New 1"></asp:ListItem>
                            <asp:ListItem Text="New 2" Value="New 2"></asp:ListItem>
                            <asp:ListItem Text="Other" Value="other"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:TextBox ID="txtOtherSource" runat="server" CssClass="form-control form-control-sm mt-2 d-none"
                            Text='<%# Eval("OtherSource") %>' placeholder="Enter other source"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Button ID="btnDelete" runat="server" CssClass="btn btn-outline-danger btn-sm"
                            Text="Delete" CommandName="DeleteRow" CommandArgument='<%# Container.ItemIndex %>'
                            OnClientClick="return confirm('Are you sure you want to delete this row?');" />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                    </table>
                </div>
            </FooterTemplate>
        </asp:Repeater>

        <!-- Comment Section -->
        <div class="textAreaInfo mt-3 mb-3">
            <asp:TextBox ID="txtComment" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"
                Placeholder="Please enter comment here!"></asp:TextBox>
        </div>

        <div class="mt-3">
            <asp:Panel ID="pnlMessage" runat="server" Visible="false">
                <div id="alertBox" runat="server" class="alert" role="alert"></div>
            </asp:Panel>
        </div>

        <!-- Submit Button -->
        <div class="actions d-flex justify-content-end">
            <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-primary" Text="Submit Suggestion ►" OnClick="btnSubmit_Click" />
        </div>

    </div>

</asp:Content>
