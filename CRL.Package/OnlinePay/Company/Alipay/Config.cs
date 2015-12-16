using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.OnlinePay.Company.Alipay
{
    public class Config
    {
        public static string Partner
        {
            get
            {
                return ChargeConfig.GetConfigKey(CompanyType.支付宝, ChargeConfig.DataType.User);
            }
        }

        public static string Key
        {
            get
            {
                return ChargeConfig.GetConfigKey(CompanyType.支付宝, ChargeConfig.DataType.Key);
            }
        }
        public static string Input_charset = ChargeConfig.Charset;
        public static string Sign_type = "MD5";
    }
}
