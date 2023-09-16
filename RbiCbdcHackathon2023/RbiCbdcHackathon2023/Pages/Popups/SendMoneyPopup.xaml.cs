using RbiCbdcHackathon2023.Database.Models;
using RbiCbdcHackathon2023.ViewModels;

namespace RbiCbdcHackathon2023.Pages.Popups;

public partial class SendMoneyPopup : ContentPage
{
    SendMoneyViewModel vm = new SendMoneyViewModel();
    public SendMoneyPopup()
	{
		InitializeComponent();
		BindingContext = vm;
    }

    private void OnStepperValueChanged(object sender, EventArgs e)
    {
        Stepper stepper = (Stepper)sender;
        Denomination item = (Denomination)stepper.BindingContext;
        if (item != null && item.MaxLimit < item.Quantity)
        {
            --item.Quantity;
            return;
        }
        vm.OnStepperValueCahnged();
    }
}