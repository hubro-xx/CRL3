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
using System.Xml;

namespace CRL.Package.OnlinePay.Company.Bill99.PciDataContent
{
    public class Response : PCIBase
    {
        /// <summary>
        /// 缩略卡号
        /// </summary>
        public string storablePan;
        public static Response FromXml(XmlDocument xmlDoc)
        {
            var obj = new Response();
            obj.customerId = xmlDoc.SelectSingleNode("MasMessage/PciDataContent/customerId").InnerText;
            obj.storablePan = xmlDoc.SelectSingleNode("MasMessage/PciDataContent/storablePan").InnerText;
            obj.responseCode = xmlDoc.SelectSingleNode("MasMessage/PciDataContent/responseCode").InnerText;
            return obj;
        }
    }
}
