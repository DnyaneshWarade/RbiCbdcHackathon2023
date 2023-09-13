using CommunityToolkit.Maui.Views;
using RbiCbdcHackathon2023.ViewModels;

namespace RbiCbdcHackathon2023.Pages.Popups;

public partial class SendMoneyPopup : Popup
{
	public SendMoneyPopup()
	{
		InitializeComponent();
		var vm = new SendMoneyViewModel();
		BindingContext = vm;
        vm.ClosePopup += ExecuteClosePopup;
    }

    private void ExecuteClosePopup(object sender, EventArgs e)
    {
        this.Close();
    }
}