using CommunityToolkit.Maui.Views;
using RbiCbdcHackathon2023.Database.Models;
using RbiCbdcHackathon2023.ViewModels;

namespace RbiCbdcHackathon2023.Pages.Popups;

public partial class LoadMoneyPopup : Popup
{
    LoadMoneyViewModel vm = new LoadMoneyViewModel();
    public LoadMoneyPopup()
	{
		InitializeComponent();
		BindingContext = vm;
        vm.ClosePopup += ExecuteClosePopup;
	}

    private void ExecuteClosePopup(object sender, EventArgs e)
    {
		this.Close();
    }

    private void OnStepperValueChanged(object sender, EventArgs e)
    {
        vm.OnStepperValueCahnged();
    }
}