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

namespace CRL.Package.OnlinePay.Company.Bill99.PciQueryContent
{
    public class Response : PCIBase
    {
        /// <summary>
        /// 卡类型
        /// </summary>
        public string cardType;
        /// <summary>
        /// 银行代码
        /// </summary>
        public string bankId;
        /// <summary>
        /// 缩略卡号
        /// </summary>
        public string storablePan;
        /// <summary>
        /// 手机号
        /// </summary>
        public string phoneNO;
        public static Response FromXml(XmlDocument xmlDoc)
        {
            var obj = new Response();
            obj.customerId = xmlDoc.SelectSingleNode("MasMessage/PciQueryContent/customerId").InnerText;
            obj.cardType = xmlDoc.SelectSingleNode("MasMessage/PciQueryContent/cardType").InnerText;
            obj.responseCode = xmlDoc.SelectSingleNode("MasMessage/PciQueryContent/responseCode").InnerText;
            obj.bankId = xmlDoc.SelectSingleNode("MasMessage/PciQueryContent/pciInfos/pciInfo/bankId").InnerText;
            obj.storablePan = xmlDoc.SelectSingleNode("MasMessage/PciQueryContent/pciInfos/pciInfo/storablePan").InnerText;
            obj.phoneNO = xmlDoc.SelectSingleNode("MasMessage/PciQueryContent/pciInfos/pciInfo/phoneNO").InnerText;
            return obj;
        }
    }
}
