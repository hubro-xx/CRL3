/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
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
