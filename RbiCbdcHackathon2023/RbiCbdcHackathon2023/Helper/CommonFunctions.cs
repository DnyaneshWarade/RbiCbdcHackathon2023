using RbiCbdcHackathon2023.Database.Models;
using RbiCbdcHackathon2023.Services.PartialMethods;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace RbiCbdcHackathon2023.Helper
{
    internal static class CommonFunctions
    {
        // Regular expression to match exactly 10 digits
        static readonly string mobileNoPattern = @"^\d{10}$";
        static readonly public string Key = "yourSecretKey123";
        static private ICryptoTransform encyptCryptoTransform;
        static private ICryptoTransform decyptCryptoTransform;
        static private SmsService smsService;
        static public string LoggedInMobileNo { get; set; }
        static public string LoggedInMobilePin { get; set; }

        public static bool ValidatePhoneNumber(string mobileNo)
        {
            if (string.IsNullOrEmpty(mobileNo)) { return false; }
            return Regex.IsMatch(mobileNo, mobileNoPattern);
        }

        private static ICryptoTransform GetEncryptor()
        {
            try
            {
                if (encyptCryptoTransform != null)
                {
                    return encyptCryptoTransform;
                }

                // Convert the secret key to a byte array
                byte[] keyBytes = Encoding.UTF8.GetBytes(Key);
                Aes aesAlg = Aes.Create();

                aesAlg.Key = keyBytes;
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.Padding = PaddingMode.PKCS7;
                encyptCryptoTransform = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                return encyptCryptoTransform;
            }
            catch
            {
                return null;
            }
        }

        private static ICryptoTransform GetDecryptor()
        {
            try
            {
                if (decyptCryptoTransform != null)
                {
                    return decyptCryptoTransform;
                }

                // Convert the secret key to a byte array
                byte[] keyBytes = Encoding.UTF8.GetBytes(Key);
                Aes aesAlg = Aes.Create();

                aesAlg.Key = keyBytes;
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.Padding = PaddingMode.PKCS7;
                decyptCryptoTransform = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                return decyptCryptoTransform;
            }
            catch
            {
                return null;
            }
        }

        public static string GetEncryptedMessage(string message)
        {
            if (message == null)
            {
                return string.Empty;
            }
            try
            {
                // get encryptor first
                ICryptoTransform encryptor = GetEncryptor();
                if (encryptor == null)
                {
                    return string.Empty;
                }

                // encrypt the message
                byte[] encryptedBytes = encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(message), 0, message.Length);
                return Convert.ToBase64String(encryptedBytes);
            }
            catch { return string.Empty; }
        }

        public static string GetDecryptedMessage(string message)
        {
            if (message == null)
            {
                return string.Empty;
            }
            try
            {
                // get encryptor first
                ICryptoTransform decryptor = GetDecryptor();
                if (decryptor == null)
                {
                    return string.Empty;
                }

                // encrypt the message
                byte[] encryptedBytes = Convert.FromBase64String(message);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }
        }

        private static SmsService GetSmsService()
        {
            if (smsService != null)
            {
                return smsService;
            }
            smsService = new SmsService();
            return smsService;
        }

        private static async Task SendSms(string mobileNo, string message)
        {
            if (string.IsNullOrEmpty(mobileNo) || string.IsNullOrEmpty(message) || !ValidatePhoneNumber(mobileNo))
            {
                return;
            }
            try
            {
                // Check if permission is already granted
                PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Sms>();
                if (status != PermissionStatus.Granted)
                {
                    // Request permission
                    await Permissions.RequestAsync<Permissions.Sms>();
                }
                GetSmsService().Send("+91" + mobileNo, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SendSmsToServer(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            string encryptedMsg = GetEncryptedMessage(message);
            SendSms("9220592205", "6MG9L" + encryptedMsg);
        }

        public static void SendEncryptedSms(string mobileNo, string message)
        {
            if (string.IsNullOrEmpty(mobileNo) || string.IsNullOrEmpty(message) || !ValidatePhoneNumber(mobileNo))
            {
                return;
            }
            string encryptedMsg = GetEncryptedMessage(message);
            SendSms(mobileNo, "DYGNI" + encryptedMsg);
        }

        public static long GetEpochTime()
        {
            return (long)(DateTimeOffset.UtcNow - DateTimeOffset.UnixEpoch).TotalSeconds;
        }

        public static ICollection<Denomination> GetDenominations()
        {
            return new List<Denomination> { 
                new Denomination("Ten", "ten.jpg", 10),
                new Denomination("Twenty", "twenty.jpg", 20),
                new Denomination("Fifty", "fifty.jpg", 50),
                new Denomination("Hundred", "hundred.jpg", 100),
                new Denomination("TwoHundred", "twohundred.jpg", 200),
                new Denomination("FiveHundred", "fivehundred.jpg", 500),
                new Denomination("TwoThousand", "twothousand.jpg", 2000),
            };
        }
    }
}
