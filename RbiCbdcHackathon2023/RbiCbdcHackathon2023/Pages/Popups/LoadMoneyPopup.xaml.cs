using CommunityToolkit.Maui.Views;
using RbiCbdcHackathon2023.ViewModels;

namespace RbiCbdcHackathon2023.Pages.Popups;

public partial class LoadMoneyPopup : Popup
{
	public LoadMoneyPopup()
	{
		InitializeComponent();
		var vm = new LoadMoneyViewModel();
		BindingContext = vm;
        vm.ClosePopup += ExecuteClosePopup;

	}

    private void ExecuteClosePopup(object sender, EventArgs e)
    {
		this.Close();
    }
}