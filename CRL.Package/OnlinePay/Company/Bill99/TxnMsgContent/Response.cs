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

namespace CRL.Package.OnlinePay.Company.Bill99.TxnMsgContent
{
    public class Response : PCIBase
    {
        /// <summary>
        /// 交易类型
        /// </summary>
        public string txnType;
        /// <summary>
        /// 消息状态
        /// </summary>
        public string interactiveStatus;
        /// <summary>
        /// 金额
        /// </summary>
        public string amount;
        /// <summary>
        /// 终端号
        /// </summary>
        public string terminalId;
        /// <summary>
        /// 交易时间
        /// </summary>
        public string entryTime;
        /// <summary>
        /// 外部跟踪编号
        /// </summary>
        public string externalRefNumber;
        /// <summary>
        /// 交易传输时间
        /// </summary>
        public string transTime;
        /// <summary>
        /// 系统参考号
        /// </summary>
        public string refNumber;
        /// <summary>
        /// 应答码文本消息
        /// </summary>
        public string responseTextMessage;
        /// <summary>
        /// 卡组织编号
        /// </summary>
        public string cardOrg;
        /// <summary>
        /// 发卡银行名称
        /// </summary>
        public string issuer;
        /// <summary>
        /// 缩略卡号
        /// </summary>
        public string storableCardNo;
        /// <summary>
        /// 授权码
        /// </summary>
        public string authorizationCode;
        public static Response FromXml(XmlDocument xmlDoc)
        {
            var obj = new Response();
            obj.txnType = xmlDoc.SelectSingleNode("MasMessage/TxnMsgContent/txnType").InnerText;
            obj.interactiveStatus = xmlDoc.SelectSingleNode("MasMessage/TxnMsgContent/interactiveStatus").InnerText;
            obj.amount = xmlDoc.SelectSingleNode("MasMessage/TxnMsgContent/amount").InnerText;
            obj.terminalId = xmlDoc.SelectSingleNode("MasMessage/TxnMsgContent/terminalId").InnerText;
            obj.entryTime = xmlDoc.SelectSingleNode("MasMessage/TxnMsgContent/entryTime").InnerText;
            obj.externalRefNumber = xmlDoc.SelectSingleNode("MasMessage/TxnMsgContent/externalRefNumber").InnerText;
            obj.customerId = xmlDoc.SelectSingleNode("MasMessage/TxnMsgContent/customerId").InnerText;
            obj.transTime = xmlDoc.SelectSingleNode("MasMessage/TxnMsgContent/transTime").InnerText;
            obj.refNumber = xmlDoc.SelectSingleNode("MasMessage/TxnMsgContent/refNumber").InnerText;
            obj.responseCode = xmlDoc.SelectSingleNode("MasMessage/TxnMsgContent/responseCode").InnerText;
            obj.responseTextMessage = xmlDoc.SelectSingleNode("MasMessage/TxnMsgContent/responseTextMessage").InnerText;
            obj.cardOrg = xmlDoc.SelectSingleNode("MasMessage/TxnMsgContent/cardOrg").InnerText;
            obj.issuer = xmlDoc.SelectSingleNode("MasMessage/TxnMsgContent/issuer").InnerText;
            obj.storableCardNo = xmlDoc.SelectSingleNode("MasMessage/TxnMsgContent/storableCardNo").InnerText;
            obj.authorizationCode = xmlDoc.SelectSingleNode("MasMessage/TxnMsgContent/authorizationCode").InnerText;
            return obj;
        }
    }
}
