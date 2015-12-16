namespace tenpay
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class MD5Util
    {
        public static string GetMD5(string encypStr, string charset)
        {
            byte[] bytes;
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            try
            {
                bytes = Encoding.GetEncoding(charset).GetBytes(encypStr);
            }
            catch (Exception)
            {
                bytes = Encoding.GetEncoding("GB2312").GetBytes(encypStr);
            }
            return BitConverter.ToString(provider.ComputeHash(bytes)).Replace("-", "").ToUpper();
        }
    }
}

