using RbiCbdcHackathon2023.Pages;
using RbiCbdcHackathon2023.Pages.Popups;

namespace RbiCbdcHackathon2023
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(SendMoneyPopup), typeof(SendMoneyPopup));
        }
    }
}