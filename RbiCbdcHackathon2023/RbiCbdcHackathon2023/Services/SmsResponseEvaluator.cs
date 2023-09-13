using RbiCbdcHackathon2023.Helper;
using RbiCbdcHackathon2023.Services.PartialMethods;
using System.Text.Json;

namespace RbiCbdcHackathon2023.Services
{
    public static class SmsResponseEvaluator
    {
        private static string getInteractionId(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                return string.Empty;
            }

            var stringToFind = "Your interaction id is ";
            int strlen = stringToFind.Length;
            int index = msg.IndexOf(stringToFind);
            if (index != -1)
            {
                int andOneIndex = msg.IndexOf(" and", index + strlen);
                int andTwoIndex = msg.IndexOf(" and", andOneIndex + 4);
                int dotIndex = msg.IndexOf(".", andTwoIndex + 4);
                int idOneLength = andOneIndex - (index + strlen);
                int idTwoLength = andTwoIndex - (andOneIndex + 5);
                int idThreeLength = dotIndex - (andTwoIndex + 5);

                Console.WriteLine(msg.Length + "," + index + "," + strlen + "," + (index + strlen) + "," + andOneIndex + "," + andTwoIndex + "," + dotIndex);
                Console.WriteLine(msg.Substring(index + strlen, idOneLength));
                Console.WriteLine(msg.Substring(andOneIndex + 4, idTwoLength));
                Console.WriteLine(msg.Substring(andTwoIndex + 4, idThreeLength));
                return msg.Substring(index + strlen, idOneLength) +
                        msg.Substring(andOneIndex + 5, idTwoLength) +
                        msg.Substring(andTwoIndex + 5, idThreeLength);
            }
            return string.Empty;
        }

        public static JsonElement GetDygnifySms(string searchStr)
        {
            try
            {
                var msg = ReadSmsService.GetDygnifySms();
                if (string.IsNullOrWhiteSpace(msg))
                {
                    return default(JsonElement);
                }
                var interactionId = string.Empty;
                if (msg.Contains(searchStr))
                {
                    interactionId = getInteractionId(msg);
                    var res = CommonFunctions.GetDecryptedMessage(interactionId);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        return JsonDocument.Parse(res).RootElement;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return default(JsonElement);
        }

        public static JsonElement GetSmsStartWithDYGNI()
        {
            try
            {
                var msg = ReadSmsService.GetSmsStartWithDYGNI();
                if (string.IsNullOrWhiteSpace(msg))
                {
                    return default(JsonElement);
                }

                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return default(JsonElement);
        }
    }
}
