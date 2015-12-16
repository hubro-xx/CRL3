namespace tenpay
{
    using System;
    using System.Text;
    using System.Web;

    public class TenpayUtil
    {
        public static string BuildRandomStr(int length)
        {
            Random random = new Random();
            string str = random.Next().ToString();
            if (str.Length > length)
            {
                return str.Substring(0, length);
            }
            if (str.Length < length)
            {
                for (int i = length - str.Length; i > 0; i--)
                {
                    str.Insert(0, "0");
                }
            }
            return str;
        }

        public static uint UnixStamp()
        {
            TimeSpan span = (TimeSpan) (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(0x7b2, 1, 1)));
            return Convert.ToUInt32(span.TotalSeconds);
        }

        public static string UrlDecode(string instr, string charset)
        {
            if ((instr == null) || (instr.Trim() == ""))
            {
                return "";
            }
            try
            {
                return HttpUtility.UrlDecode(instr, Encoding.GetEncoding(charset));
            }
            catch (Exception)
            {
                return HttpUtility.UrlDecode(instr, Encoding.GetEncoding("GB2312"));
            }
        }

        public static string UrlEncode(string instr, string charset)
        {
            if ((instr == null) || (instr.Trim() == ""))
            {
                return "";
            }
            try
            {
                return HttpUtility.UrlEncode(instr, Encoding.GetEncoding(charset));
            }
            catch (Exception)
            {
                return HttpUtility.UrlEncode(instr, Encoding.GetEncoding("GB2312"));
            }
        }
    }
}

