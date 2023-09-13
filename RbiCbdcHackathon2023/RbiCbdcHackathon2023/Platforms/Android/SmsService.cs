using Android.Telephony;
using static Android.Renderscripts.ScriptGroup;

namespace RbiCbdcHackathon2023.Services.PartialMethods
{
    public partial class SmsService
    {
        private IList<string> SplitStringIntoChunks(string str, int chunkSize = 150)
        {
            IList<string> chunks = new List<string>();
            if (string.IsNullOrWhiteSpace(str))
            {
                return chunks;
            }

            for (int i = 0; i < str.Length; i += chunkSize)
            {
                int length = Math.Min(chunkSize, str.Length - i);
                chunks.Add(str.Substring(i, length));
            }

            return chunks;
        }

        public partial void Send(string phoneNo, string message)
        {
            var parts = SplitStringIntoChunks(message);
            SmsManager.Default.SendMultipartTextMessage(phoneNo, null, parts, null, null);
        }
    }
}
