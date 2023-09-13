using Android.Content;
using Android.Telephony;

namespace RbiCbdcHackathon2023.Services.PartialMethods
{
    public static partial class ReadSmsService
    {
        public static partial string GetDygnifySms()
        {
            string INBOX = "content://sms/inbox";
            string[] reqCols = new string[] { "_id", "thread_id", "address", "person", "date", "body", "type" };
            Android.Net.Uri uri = Android.Net.Uri.Parse(INBOX);
            var cursor = Android.App.Application.Context.ContentResolver.Query(uri, reqCols, null, null, null);
            if (cursor.MoveToFirst())
            {
                for (int i = 0; i < cursor.Count; i++) {
                    String messageId = cursor.GetString(cursor.GetColumnIndex(reqCols[0]));
                    String threadId = cursor.GetString(cursor.GetColumnIndex(reqCols[1]));
                    String address = cursor.GetString(cursor.GetColumnIndex(reqCols[2]));
                    String name = cursor.GetString(cursor.GetColumnIndex(reqCols[3]));
                    String date = cursor.GetString(cursor.GetColumnIndex(reqCols[4]));
                    String msg = cursor.GetString(cursor.GetColumnIndex(reqCols[5]));
                    String type = cursor.GetString(cursor.GetColumnIndex(reqCols[6]));

                    if (address.Contains("DYGNIF"))
                    {
                        return msg;
                    }
                    cursor.MoveToNext();
                }
            }
            return "";
        }

        public static partial string GetSmsStartWithDYGNI()
        {
            string INBOX = "content://sms/inbox";
            string[] reqCols = new string[] { "_id", "thread_id", "address", "person", "date", "body", "type" };
            Android.Net.Uri uri = Android.Net.Uri.Parse(INBOX);
            var cursor = Android.App.Application.Context.ContentResolver.Query(uri, reqCols, null, null, null);
            if (cursor.MoveToFirst())
            {
                for (int i = 0; i < /*cursor.Count*/ 20; i++)
                {
                    String messageId = cursor.GetString(cursor.GetColumnIndex(reqCols[0]));
                    String threadId = cursor.GetString(cursor.GetColumnIndex(reqCols[1]));
                    String address = cursor.GetString(cursor.GetColumnIndex(reqCols[2]));
                    String name = cursor.GetString(cursor.GetColumnIndex(reqCols[3]));
                    String date = cursor.GetString(cursor.GetColumnIndex(reqCols[4]));
                    String msg = cursor.GetString(cursor.GetColumnIndex(reqCols[5]));
                    String type = cursor.GetString(cursor.GetColumnIndex(reqCols[6]));

                    if (msg.Contains("DYGNI"))
                    {
                        return msg.Substring(5);
                    }
                    cursor.MoveToNext();
                }
            }
            return "";
        }
    }
}
