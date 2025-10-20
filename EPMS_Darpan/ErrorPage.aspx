<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ErrorPage.aspx.cs" Inherits="ErrorPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>KPA Co-Creation Portal
    </title>
    <link href="Content/errorpage.css" rel="stylesheet" />

</head>
<body>
    <form id="form1" runat="server">
        <div class="error-container">
            <h1>Oops!</h1>
            <p>Something went wrong. Please try again later.</p>

            <asp:Button ID="btnToggleDetails" runat="server" Text="Show Details" CssClass="toggle" OnClientClick="toggleDetails(); return false;" />

            <asp:Label ID="lblErrorDetails" runat="server" CssClass="details"></asp:Label>
            <br />
            <asp:Button ID="btnGoToLogin" runat="server" Text="Go to Login Page" CssClass="btn" OnClick="btnGoToLogin_Click" />
        </div>
    </form>

    <script>



        function toggleDetails() {
            var details = document.querySelector('.details');
            var toggleBtn = document.getElementById('<%= btnToggleDetails.ClientID %>'); // Get ASP.NET button

            if (details.style.display === 'block') {
                details.style.display = 'none';
                toggleBtn.value = 'Show Details'; // For ASP.NET Button, use .value
                toggleBtn.classList.remove('active');
            } else {
                details.style.display = 'block';
                toggleBtn.value = 'Hide Details';
                toggleBtn.classList.add('active');
            }
        }

    </script>
</body>
</html>
