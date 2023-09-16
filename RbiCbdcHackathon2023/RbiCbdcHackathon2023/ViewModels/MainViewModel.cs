using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RbiCbdcHackathon2023.Database;
using RbiCbdcHackathon2023.Database.Models;
using RbiCbdcHackathon2023.Helper;
using RbiCbdcHackathon2023.Pages.Popups;
using RbiCbdcHackathon2023.Services;
using RbiCbdcHackathon2023.Services.PartialMethods;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace RbiCbdcHackathon2023.ViewModels
{
    public partial class MainViewModel : ObservableObject, IQueryAttributable
    {
        private readonly DatabaseContext _databaseContext;

        [ObservableProperty]
        ObservableCollection<Transaction> transactions = new ObservableCollection<Transaction>();

        [ObservableProperty]
        bool isLoading = false;

        [ObservableProperty]
        double balance = 0;

        [ObservableProperty]
        double unclearedBal = 0;

        public MainViewModel(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
            LoadTransactionsAsync();
            CheckUserReceivedTrx();
            LoadBalance();
        }

        private async Task LoadBalance()
        {
            var balStr = await SecureStorage.GetAsync("balance");
            if (double.TryParse(balStr, out var bal))
                Balance = bal;

            var unclearBalStr = await SecureStorage.GetAsync("unclearedBal");
            if (double.TryParse(unclearBalStr, out var unclearBal))
                UnclearedBal = unclearBal;

        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            try
            {
                var trx = query["transaction"] as Transaction;
                if (trx != null)
                {
                    trx.AmtColor = "#FEBE00";
                    await _databaseContext.AddItemAsync(trx);
                    Transactions.Insert(0, trx);
                    CheckUserInitTrxResponses(trx);
                    UnclearedBal += trx.Amount;
                    await SecureStorage.Default.SetAsync("unclearedBal", UnclearedBal.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [RelayCommand]
        public async Task ClearData()
        {
            try
            {
                await _databaseContext.DeleteAllAsync<Transaction>();
                Transactions.Clear();
                UnclearedBal = Balance = 0;
                await SecureStorage.SetAsync("unclearedBal", UnclearedBal.ToString());
                await SecureStorage.SetAsync("balance", Balance.ToString());
                await SecureStorage.SetAsync("denominations", string.Empty);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task LoadTransactionsAsync()
        {
            var trxs = await _databaseContext.GetAllAsync<Transaction>();

            if (trxs is not null && trxs.Any())
            {
                foreach (var item in trxs.OrderByDescending(t => t.ReqId))
                {
                    Transactions.Add(item);
                }
            }
        }

        private async Task CheckUserInitTrxResponses(Transaction trx)
        {
            try
            {
                if (trx == null)
                {
                    return;
                }

                await Task.Delay(10000);
                do
                {
                    JsonElement res = default(JsonElement);
                    if (trx.Desc == "Load money")
                    {
                        var searchStr = "has been successfully credited in your ePaisa wallet.";
                        res = SmsResponseEvaluator.GetDygnifySms(searchStr);

                        if (res.ValueKind != System.Text.Json.JsonValueKind.Undefined &&
                            res.GetProperty("r").GetString() == trx.ReqId && res.GetProperty("s").GetBoolean())
                        {

                            trx.AmtColor = "#0B6623";
                            await UpdateTransaction(trx);

                            UnclearedBal -= trx.Amount;
                            await SecureStorage.SetAsync("unclearedBal", UnclearedBal.ToString());
                            Balance += trx.Amount;
                            await SecureStorage.SetAsync("balance", Balance.ToString());
                            break;
                        }
                    }
                    await Task.Delay(2000);
                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task UpdateTransaction(Transaction trx)
        {
            try
            {
                var trxCopy = trx.Clone();
                trxCopy.Status = "Complete";
                await _databaseContext.UpdateItemAsync(trxCopy);
                var index = Transactions.IndexOf(trx);
                Transactions.RemoveAt(index);
                Transactions.Insert(index, trxCopy);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task CheckUserReceivedTrx()
        {
            try
            {
                do
                {
                    // wait for sometime
                    await Task.Delay(2000);
                    // send sms to server for send transaction
                    // Check if permission is already granted
                    PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Sms>();
                    if (status != PermissionStatus.Granted)
                    {
                        // Request permission
                        await Permissions.RequestAsync<Permissions.Sms>();
                    }
                    var res = ReadSmsService.GetSmsStartWithDYGNI();

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        var data = CommonFunctions.GetDecryptedMessage(res);
                        if (!string.IsNullOrWhiteSpace(data))
                        {
                            var jsonData = JsonDocument.Parse(data).RootElement;
                            if (jsonData.ValueKind != System.Text.Json.JsonValueKind.Undefined)
                            {
                                var reqId = jsonData.GetProperty("requestId").GetString();
                                var Transaction = Transactions.FirstOrDefault(t => t.ReqId == reqId, null);
                                if (Transaction == null && !string.IsNullOrWhiteSpace(reqId))
                                {
                                    var amt = jsonData.GetProperty("amount").GetDouble();
                                    var from = jsonData.GetProperty("from").GetInt64().ToString();
                                    var to = jsonData.GetProperty("to").GetInt64().ToString();
                                    CommonFunctions.SendSmsToServer(data);
                                    Transaction trx = new Transaction { ReqId = reqId, Amount = amt, Desc = "Rcvd money", From = from, To = to, Status = "In Process" };
                                    await _databaseContext.AddItemAsync(trx);
                                    trx.AmtColor = "#FEBE00";
                                    Transactions.Insert(0, trx);

                                    UnclearedBal += trx.Amount;
                                    await SecureStorage.SetAsync("unclearedBal", UnclearedBal.ToString());
                                }
                            }
                        }
                    }
                    // read send sms response
                    var searchStr = "Your have received amount of";
                    JsonElement sendMoneyRes = SmsResponseEvaluator.GetDygnifySms(searchStr);
                    if (sendMoneyRes.ValueKind != System.Text.Json.JsonValueKind.Undefined)
                    {
                        var rId = sendMoneyRes.GetProperty("r").GetString();
                        var sendMonetTrx = Transactions.FirstOrDefault(t => t.ReqId == rId, null);
                        if (sendMonetTrx != null && sendMonetTrx.Status != "Complete" && sendMoneyRes.GetProperty("s").GetBoolean())
                        {
                            sendMonetTrx.AmtColor = "#0B6623";
                            await UpdateTransaction(sendMonetTrx);
                            Balance += sendMonetTrx.Amount;
                            UnclearedBal -= sendMonetTrx.Amount;
                            await SecureStorage.SetAsync("balance", Balance.ToString());
                            await SecureStorage.SetAsync("unclearedBal", UnclearedBal.ToString());
                        }
                    }

                    searchStr = "has been successfully credited to ePaisa wallet of";
                    sendMoneyRes = SmsResponseEvaluator.GetDygnifySms(searchStr);
                    if (sendMoneyRes.ValueKind != System.Text.Json.JsonValueKind.Undefined)
                    {
                        var rId = sendMoneyRes.GetProperty("r").GetString();
                        var sendMonetTrx = Transactions.FirstOrDefault(t => t.ReqId == rId, null);
                        if (sendMonetTrx != null && sendMonetTrx.Status != "Complete" && sendMoneyRes.GetProperty("s").GetBoolean())
                        {
                            sendMonetTrx.AmtColor = "#7f0000";
                            await UpdateTransaction(sendMonetTrx);
                            Balance -= sendMonetTrx.Amount;
                            UnclearedBal -= sendMonetTrx.Amount;
                            await SecureStorage.SetAsync("balance", Balance.ToString());
                            await SecureStorage.SetAsync("unclearedBal", UnclearedBal.ToString());
                        }
                    }

                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [RelayCommand]
        public async Task SendMoney()
        {
            await Shell.Current.GoToAsync(nameof(SendMoneyPopup));
        } 
    }
}