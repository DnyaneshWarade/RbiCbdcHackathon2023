using RbiCbdcHackathon2023.ViewModels;

namespace RbiCbdcHackathon2023.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}