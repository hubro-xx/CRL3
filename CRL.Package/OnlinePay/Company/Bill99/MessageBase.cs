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
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CRL.Package.OnlinePay.Company.Bill99
{
    public class MessageBase
    {
        public string signMsg
        {
            get;
            set;
        }
        protected string GetSign()
        {
            var fields = GetType().GetProperties();
            string returnStr = "";
            foreach (var item in fields)
            {
                if (item.Name == "signMsg")
                {
                    continue;
                }
                string paramValue = item.GetValue(this, null) + "";
                var paramId = item.Name;
                if (returnStr != "")
                {

                    if (paramValue != "")
                    {

                        returnStr += "&" + paramId + "=" + paramValue;
                    }

                }
                else
                {

                    if (paramValue != "")
                    {
                        returnStr = paramId + "=" + paramValue;
                    }
                }
            }
            return returnStr;
        }

        public void MakeSign()
        {
            var returnStr = GetSign();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(returnStr);
            var privateKey = CoreHelper.CustomSetting.GetConfigKey("快钱WEB私钥文件");
            var pass = CoreHelper.CustomSetting.GetConfigKey("快钱WEB私钥文件密码");
            X509Certificate2 cert = new X509Certificate2(privateKey, pass, X509KeyStorageFlags.MachineKeySet);
            RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)cert.PrivateKey;
            RSAPKCS1SignatureFormatter f = new RSAPKCS1SignatureFormatter(rsapri);
            byte[] result;
            f.SetHashAlgorithm("SHA1");
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            result = sha.ComputeHash(bytes);
            var signMsg2 = System.Convert.ToBase64String(f.CreateSignature(result)).ToString();
            signMsg = signMsg2;
        }
        public bool CheckSign()
        {
            var signMsgVal = GetSign();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(signMsgVal);
            byte[] SignatureByte = Convert.FromBase64String(signMsg);
            var publicKey = CoreHelper.CustomSetting.GetConfigKey("快钱WEB公钥文件");
            X509Certificate2 cert = new X509Certificate2(publicKey, "");
            RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)cert.PublicKey.Key;
            rsapri.ImportCspBlob(rsapri.ExportCspBlob(false));
            RSAPKCS1SignatureDeformatter f = new RSAPKCS1SignatureDeformatter(rsapri);
            byte[] result;
            f.SetHashAlgorithm("SHA1");
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            result = sha.ComputeHash(bytes);
            return f.VerifySignature(result, SignatureByte);
        }
    }
}
