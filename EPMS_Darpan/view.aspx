<%@ Page Title="View KPAs" Language="C#" MasterPageFile="~/KpaMaster.Master" AutoEventWireup="true" CodeFile="view.aspx.cs" Inherits="view" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    KPA Co-Creation Portal
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <asp:HiddenField ID="hfRoleId" runat="server" />
    <asp:HiddenField ID="hfEmpId" runat="server" />

    <!-- Main Container -->
    <div class="container my-4">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <asp:HyperLink ID="hlBack" runat="server" NavigateUrl="index.aspx"
                CssClass="btn btn-sm btn-outline-secondary me-2" Text="Back"></asp:HyperLink>
        </div>

        <div class="row">
            <!-- Left Column (KPA Table) -->
            <div class="col-md-8">
                <div class="d-flex justify-content-between mb-3">
                    <h6 class="mb-0 fw-bold">
                        <asp:Label ID="lblRole" runat="server" /></h6>
                    <span class="badge bg-secondary p-2">
                        <asp:Label ID="lblStatus" runat="server" /></span>
                </div>

                <div class="tableKPAInfo">
                    <table class="table table-striped">
                        <thead>
                            <tr>
                                <th></th>
                                <th>Category</th>
                                <th>KPA Name</th>
                                <th>KPA Importance</th>
                                <th>Unit of KPA</th>
                                <th>Data Source</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptKPA" runat="server" OnItemDataBound="rptKPA_ItemDataBound">
                                <ItemTemplate>
                                    <tr runat="server" id="trKPA">
                                        <td>
                                            <asp:CheckBox ID="chkKPA" runat="server" AutoPostBack="true"
                                                OnCheckedChanged="ValidateGuardrails" />
                                        </td>
                                        <td>
                                            <asp:HiddenField ID="hfCategory" runat="server" Value='<%# Eval("Category") %>' />
                                            <%# Eval("Category") %>
                                        </td>
                                        <td>
                                            <h6 class="m-0"><%# Eval("KPAName") %></h6>
                                        </td>
                                        <td>
                                            <div class="importance-btn btn-group" role="group">
                                                <label class="btn btn-outline-primary btn-sm">
                                                    <asp:RadioButton ID="rbHigh" runat="server"
                                                        GroupName='<%# "grp" + Container.ItemIndex %>'
                                                        AutoPostBack="true"
                                                        OnCheckedChanged="ValidateGuardrails"
                                                        CssClass="btn-check" />H
                                                </label>
                                                <label class="btn btn-outline-primary btn-sm">
                                                    <asp:RadioButton ID="rbMed" runat="server"
                                                        GroupName='<%# "grp" + Container.ItemIndex %>'
                                                        AutoPostBack="true"
                                                        OnCheckedChanged="ValidateGuardrails"
                                                        CssClass="btn-check" />M
                                                </label>
                                                <label class="btn btn-outline-primary btn-sm">
                                                    <asp:RadioButton ID="rbLow" runat="server"
                                                        GroupName='<%# "grp" + Container.ItemIndex %>'
                                                        AutoPostBack="true"
                                                        OnCheckedChanged="ValidateGuardrails"
                                                        CssClass="btn-check" />L
                                                </label>
                                            </div>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblUnitOfKPA" runat="server" CssClass="form-label mb-0 flex-grow-1"
                                                Text='<%# Eval("UnitOfKPA") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <div class="d-flex align-items-center justify-content-between gap-2">
                                                <asp:Label ID="lblDataSource" CssClass="form-label mb-0 flex-grow-1"
                                                    runat="server" Text='<%# Eval("DataSource") %>'></asp:Label>
                                                <asp:TextBox ID="txtDataSource" runat="server"
                                                    CssClass="form-control form-control-sm flex-grow-1"
                                                    Text='<%# Eval("DataSource") %>' Style="display: none;"></asp:TextBox>
                                                <asp:HiddenField ID="hfKPAId" runat="server" Value='<%# Eval("KpaID") %>' />
                                                <asp:PlaceHolder ID="phEditBtn" runat="server">
                                                    <button type="button" class="btn btn-sm btn-primary float-end edit-save-btn">
                                                        Edit
                                                    </button>
                                                </asp:PlaceHolder>
                                            </div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>

                <div class="textAreaInfo mt-3">
                    <asp:TextBox ID="txtComment" runat="server" CssClass="form-control" TextMode="MultiLine"
                        Placeholder="Please enter comment here!"></asp:TextBox>
                    <div class="d-flex justify-content-end mt-2">
                        <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-primary" Text="Submit Suggestion"
                            OnClientClick="return validateForm();" OnClick="btnSubmit_Click" />
                    </div>
                </div>
            </div>

          
            <div class="col-md-4 guardrails-container">
                <asp:UpdatePanel ID="upGuardrails" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="d-flex justify-content-between mb-3">
                            <h6 class="mb-0 fw-bold">Guardrails</h6>
                            <span class="badge bg-secondary p-2" id="lblSelected" runat="server">Selected: 0</span>
                        </div>

                        <div>
                            <div id="grMinMax" runat="server" class="guardrail-item redData" visible="false">
                                <span>Select a minimum of five KPA's (max 10)<br />
                                    <small id="lblMinMax" runat="server" class="text-muted">0/10</small></span>
                            </div>

                            <div id="grSafety" runat="server" class="guardrail-item redData" visible="false">
                                <span>Safety KPAs: 1 to 2<br />
                                    <small id="lblSafety" runat="server" class="text-muted">0</small></span>
                            </div>

                            <div id="grCategory" runat="server" class="guardrail-item redData" visible="true">
                                <span>≥1 per KPA category<br />
                                    <small id="lblCategory" runat="server" class="text-muted"></small></span>
                            </div>

                            <div id="grImportance" runat="server" class="guardrail-item redData" visible="true">
                                <span>
                                    <span id="lblKPAImportance" runat="server"></span><br />
                                    <small id="lblImportance" runat="server" class="text-muted">H:0 M:0 L:0</small>
                                </span>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

        </div>
    </div>

    <!-- JS for edit buttons and radio highlights -->
    <script>
        document.addEventListener("DOMContentLoaded", function () {

            // Edit buttons functionality
            document.querySelectorAll(".edit-save-btn").forEach(btn => {
                btn.addEventListener("click", function () {
                    const container = this.closest("div");
                    const label = container.querySelector("span[id*='lblDataSource']");
                    const textbox = container.querySelector("input[id*='txtDataSource']");
                    label.style.display = "none";
                    textbox.style.display = "inline-block";
                    textbox.focus();
                    this.style.display = "none";
                });
            });

            // Handle checkboxes for KPAs
            document.querySelectorAll("input[id*='chkKPA']").forEach(chk => {
                // Initialize radios state
                toggleRadios(chk);

                // On checkbox change
                chk.addEventListener("change", function () {
                    toggleRadios(this);
                    if (!this.checked) {
                        resetRadios(this);
                    }
                });
            });

            // Enable/disable radios based on checkbox
            function toggleRadios(chk) {
                const row = chk.closest("tr");
                if (!row) return;
                const radios = row.querySelectorAll("input[type='radio']");
                radios.forEach(r => r.disabled = !chk.checked);
            }

            // Reset all radio buttons in the row
            function resetRadios(chk) {
                const row = chk.closest("tr");
                if (!row) return;
                const radios = row.querySelectorAll("input[type='radio']");
                radios.forEach(r => {
                    r.checked = false;
                    const lbl = r.closest("label");
                    if (lbl) lbl.classList.remove("active", "btn-primary");
                });
            }

            // Update radio button styling
            function updateRadioButtonColor(container) {
                const labels = container.querySelectorAll("label");
                labels.forEach(lbl => lbl.classList.remove("active", "btn-primary"));
                const checkedRadio = container.querySelector("input[type='radio']:checked");
                if (checkedRadio && !checkedRadio.disabled) {
                    const parentLabel = checkedRadio.closest("label");
                    parentLabel.classList.add("active", "btn-primary");
                }
            }

            // Initialize importance button groups
            document.querySelectorAll(".importance-btn").forEach(group => {
                updateRadioButtonColor(group);
                group.querySelectorAll("input[type='radio']").forEach(radio => {
                    radio.addEventListener("change", () => updateRadioButtonColor(group));
                });
            });

            
            //-----------------disable radio on view page 
            const submitBtn = document.getElementById('<%= btnSubmit.ClientID %>');

            if (!submitBtn) {
                const allRadios = document.querySelectorAll("input[type='radio']");

                allRadios.forEach(radio => {
                    // Only apply to radios that are NOT already checked
                    if (!radio.checked) {
                        // Prevent click on the radio itself
                        radio.addEventListener("click", function (e) {
                            e.preventDefault();
                        });

                        // Prevent click and hover effects on the label
                        const label = radio.closest("label");
                        if (label) {
                            label.addEventListener("click", function (e) {
                                e.preventDefault();
                            });
                            // Disable hover styles via CSS
                            label.style.pointerEvents = "none";  // Prevent hover and click
                            label.style.cursor = "not-allowed";  // Show not-allowed cursor
                            label.classList.remove("active", "btn-primary"); // Remove any active class
                        }
                    }
                });
            }

        });
    </script>


</asp:Content>
