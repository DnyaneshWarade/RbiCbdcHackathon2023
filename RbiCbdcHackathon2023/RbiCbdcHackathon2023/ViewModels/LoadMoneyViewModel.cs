using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RbiCbdcHackathon2023.Database;
using RbiCbdcHackathon2023.Helper;
using RbiCbdcHackathon2023.Pages;
using RbiCbdcHackathon2023.Database.Models;

namespace RbiCbdcHackathon2023.ViewModels
{
    public partial class LoadMoneyViewModel : ObservableObject
    {
        [ObservableProperty]
        string amount;

        [ObservableProperty]
        string error;

        public event EventHandler ClosePopup;

        [RelayCommand]
        async Task LoadMoney()
        {
            try
            {
                Error = string.Empty;
                int amt;
                if (!int.TryParse(Amount, out amt) || amt < 0)
                {
                    Error = "Enter valid amount";
                    return;
                }
                var reqId = CommonFunctions.GetEpochTime();
                var message = "{" + $"\"requestId\": \"{reqId}\",\"action\":\"loadMoney\",\"amount\": {amt}, \"to\": {CommonFunctions.LoggedInMobileNo}, \"pin\": {CommonFunctions.LoggedInMobilePin}, \"status\": \"In Process\", \"desc\": \"Load money\"" + "}";
                CommonFunctions.SendSmsToServer(message);
                Transaction newItem = new Transaction { ReqId = reqId.ToString(), Amount = amt, To = CommonFunctions.LoggedInMobileNo, Status = "In Process", Desc = "Load money" };

                var navigationParameter = new Dictionary<string, object>
                                            {
                                                { "transaction", newItem }
                                            };
                await Shell.Current.GoToAsync("..", true, navigationParameter);
                ClosePopup?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Error = "Failed to Load Money";
                Console.WriteLine(ex.Message);
            }
        }
    }
}
