/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace CRL.Package.OnlinePay.Company.Bill99
{
    public class PCIBase
    {
        public string Host
        {
            get
            {
                return "https://sandbox.99bill.com:9445";//测试
            }
        }
        public virtual string InterfacePath
        {
            get
            {
                return "/";
            }
        }
        /// <summary>
        /// 商户编号
        /// </summary>
        public string merchantId;
        /// <summary>
        /// 客户编号
        /// </summary>
        public string customerId;
        /// <summary>
        /// 返回状态码
        /// </summary>
        public string responseCode;
        public string OtherMsg = null;//附加消息

        public string FormatMsg()
        {
            merchantId = ChargeConfig.GetConfigKey(CompanyType.快钱, ChargeConfig.DataType.User);
            var messageName = GetType().Namespace;
            messageName = messageName.Substring(messageName.LastIndexOf(".") + 1);
            String signMsgXml = "";
            signMsgXml += "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>";
            signMsgXml += "<MasMessage xmlns=\"http://www.99bill.com/mas_cnp_merchant_interface\">";
            signMsgXml += "<version>1.0</version>";
            signMsgXml += "<" + messageName + ">";
            signMsgXml += "<merchantId>" + merchantId + "</merchantId>";
            signMsgXml += "<customerId>" + customerId + "</customerId>";
            var fields = GetType().GetFields();
            foreach(var item in fields)
            {
                var value = item.GetValue(this) + "";
                if (item.Name == "OtherMsg")
                {
                    signMsgXml += value;
                }
                else
                {
                    signMsgXml += string.Format("<{0}>{1}</{0}>", item.Name, value);
                }
            }
            signMsgXml += "</" + messageName + ">";
            signMsgXml += "</MasMessage>";
            return signMsgXml;
        }
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        public XmlDocument SendRequest()
        {
            var data = FormatMsg();
            string url = Host + InterfacePath;
            var privateKey = CoreHelper.CustomSetting.GetConfigKey("99billQuickPayPFXFile");
            var user = ChargeConfig.GetConfigKey(CompanyType.快钱, ChargeConfig.DataType.User);
            var pass = ChargeConfig.GetConfigKey(CompanyType.快钱, ChargeConfig.DataType.PayPass);
            X509Certificate2 x509 = new X509Certificate2(privateKey, pass, X509KeyStorageFlags.MachineKeySet);

            byte[] buffer = Encoding.GetEncoding("utf-8").GetBytes(data);
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);

            webReq.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(user + ":" + pass)));

            webReq.ClientCertificates.Add(x509);
            webReq.Timeout = 40000;
            webReq.KeepAlive = true;
            webReq.Method = "POST";
            webReq.ContentType = "application/x-www-form-urlencoded";
            webReq.ContentLength = buffer.Length;
            Stream reqStream = webReq.GetRequestStream();
            reqStream.Write(buffer, 0, buffer.Length);
            reqStream.Close();

            HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();
            StreamReader sr = new StreamReader(webResp.GetResponseStream());
            var returnData = sr.ReadToEnd();
            returnData = returnData.Replace("xmlns=\"http://www.99bill.com/mas_cnp_merchant_interface\"", "");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(returnData);
            webResp.Close();
            sr.Close();
            return xmlDoc;
        }
    }
}
