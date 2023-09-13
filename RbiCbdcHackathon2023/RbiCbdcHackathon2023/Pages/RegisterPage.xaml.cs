using RbiCbdcHackathon2023.ViewModels;

namespace RbiCbdcHackathon2023.Pages;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}