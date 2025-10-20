<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>KPA Co-Creation Portal</title>
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
    <link href="~/Content/custom.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="bgdataInfo">
            <div class="container">
                <div class="login-container">
                    <!-- Header -->
                    <div class="login-header">
                        <h4>KPA Co-Creation Portal</h4>
                        <p class="text-muted">Sign in with your credentials</p>
                    </div>
                      <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mt-2" Visible="true"></asp:Label>
                    <!-- Login Form -->
                    <div class="mb-3">
                        <asp:Label ID="lblUsername" runat="server" Text="Username" CssClass="form-label" AssociatedControlID="txtUsername"></asp:Label>
                        <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" Placeholder="Enter Username"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <asp:Label ID="lblPassword" runat="server" Text="Password" CssClass="form-label" AssociatedControlID="txtPassword"></asp:Label>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" Placeholder="Enter Password"></asp:TextBox>
                    </div>
                    <div class="d-grid">
                        <asp:Button ID="btnSignIn" runat="server" Text="Sign In" CssClass="btn btn-primary" OnClick="btnSignIn_Click" OnClientClick="return validateLogin();" />
                    </div>

                  
                </div>
            </div>
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js" 
integrity="sha384-YvpcrYf0tY3lHB60NNkmXc5s9fDVZLESaAA55NDzOxhy9GkcIdslK1eN7N6jIeHz" 
crossorigin="anonymous"></script>
</body>
</html>
<script type="text/javascript">
        // JavaScript validation function
        function validateLogin() {
            var username = document.getElementById("<%= txtUsername.ClientID %>").value.trim();
            var password = document.getElementById("<%= txtPassword.ClientID %>").value.trim();
            var messageLabel = document.getElementById("<%= lblMessage.ClientID %>");

            if (username === "") {
                messageLabel.innerHTML = "Please enter your username.";
                return false; 
            }
            if (password === "") {
                messageLabel.innerHTML = "Please enter your password.";
                return false; 
            }

           
            messageLabel.innerHTML = "";
            return true; 
        }
</script>

  

</script>
