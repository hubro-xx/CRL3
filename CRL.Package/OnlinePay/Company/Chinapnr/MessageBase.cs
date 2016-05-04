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
using CHINAPNRLib;
namespace CRL.Package.OnlinePay.Company.Chinapnr
{
    public class MessageBase
    {
        public string GetSignData()
        {
            var fields = GetType().GetFields();
            string returnStr = "";
            foreach (var item in fields)
            {
                if (item.Name == "ChkValue")
                {
                    continue;
                }
                string paramValue = item.GetValue(this) + "";
                returnStr += paramValue;
            }
            return returnStr;
        }
        static string privateKey = CoreHelper.CustomSetting.GetConfigKey("汇付天下私钥文件");
        static string publicKey = CoreHelper.CustomSetting.GetConfigKey("汇付天下公钥文件");
        public string MakeSign(string merId)
        {
            CHINAPNRLib.NetpayClient SignObject = new NetpayClientClass();
            string data = GetSignData();
            data = SignObject.SignMsg0(merId, privateKey, data, data.Length); 
            return data;
        }

        public bool CheckSign(string ChkValue)
        {
            var signMsgVal = GetSignData();
            CHINAPNRLib.NetpayClient SignObject = new NetpayClientClass();
            var a = SignObject.VeriSignMsg0(publicKey, signMsgVal, signMsgVal.Length, ChkValue);
            return a == "0";
        }
    }
}
