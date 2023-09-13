using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RbiCbdcHackathon2023.Helper;
using RbiCbdcHackathon2023.Pages;
using RbiCbdcHackathon2023.Database.Models;

namespace RbiCbdcHackathon2023.ViewModels
{
    public partial class SendMoneyViewModel : ObservableObject
    {
        [ObservableProperty]
        string receiverMobileNo;

        [ObservableProperty]
        string amount;

        [ObservableProperty]
        string pin;

        [ObservableProperty]
        string error;

        public event EventHandler ClosePopup;

        [RelayCommand]
        async Task SendMoney()
        {
            Error = string.Empty;
            if (!CommonFunctions.ValidatePhoneNumber(ReceiverMobileNo))
            {
                Error = "Enter valid receiver number";
                return;
            }
            double amt;
            if(!double.TryParse(Amount, out amt) || amt < 0)
            {
                Error = "Enter valid amount";
                return;
            }

            var reqId = CommonFunctions.GetEpochTime();
            var message = "{" + $"\"requestId\": \"{reqId}\",\"action\":\"sendMoney\",\"amount\": {amt}, \"from\": {CommonFunctions.LoggedInMobileNo}, \"pin\": {Pin}, \"to\": {ReceiverMobileNo}, \"desc\":\"Send money\"" + "}";
            CommonFunctions.SendEncryptedSms(ReceiverMobileNo, message);
            Transaction newItem = new Transaction { ReqId = reqId.ToString(), Amount = amt, From = CommonFunctions.LoggedInMobileNo, To = ReceiverMobileNo, Status = "In Process", Desc = "Send money" };

            var navigationParameter = new Dictionary<string, object>
                                            {
                                                { "transaction", newItem }
                                            };
            await Shell.Current.GoToAsync("..", true, navigationParameter);
            ClosePopup?.Invoke(this, EventArgs.Empty);
        }
    }
}
