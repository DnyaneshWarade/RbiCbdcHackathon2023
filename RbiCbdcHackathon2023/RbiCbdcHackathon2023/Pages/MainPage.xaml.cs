using CommunityToolkit.Maui.Views;
using RbiCbdcHackathon2023.Pages.Popups;
using RbiCbdcHackathon2023.ViewModels;

namespace RbiCbdcHackathon2023.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        private void LoadMoney_Clicked(object sender, EventArgs e)
        {
            this.ShowPopup(new LoadMoneyPopup());
        }
    }
}