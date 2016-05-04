/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
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

