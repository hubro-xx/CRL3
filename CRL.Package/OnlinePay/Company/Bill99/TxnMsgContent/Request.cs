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

namespace CRL.Package.OnlinePay.Company.Bill99.TxnMsgContent
{
    // 付款
    public class Request : PCIBase
    {
        public override string InterfacePath
        {
            get
            {
                return "/cnp/purchase";
            }
        }
        /// <summary>
        /// 交易类型编码
        /// </summary>
        public string txnType;
        /// <summary>
        /// interactiveStatus
        /// </summary>
        public string interactiveStatus;
        /// <summary>
        /// 交易金额
        /// </summary>
        public string amount;
        /// <summary>
        /// 终端编号
        /// </summary>
        public string terminalId;
        /// <summary>
        /// 商户端交易时间
        /// </summary>
        public string entryTime;
        /// <summary>
        /// 外部检索参考号
        /// </summary>
        public string externalRefNumber;
        /// <summary>
        /// 特殊交易标识
        /// </summary>
        public string spFlag;
        /// <summary>
        /// tr3回调地址
        /// </summary>
        public string tr3Url;

        #region 第二次时不需要传
        /// <summary>
        /// 信用卡卡号
        /// </summary>
        public string cardNo;
        public string expiredDate;
        /// <summary>
        /// 卡验证码
        /// </summary>
        public string cvv2;
        /// <summary>
        /// 持卡人姓名
        /// </summary>
        public string cardHolderName;
        /// <summary>
        /// 持卡人身份证号
        /// </summary>
        public string cardHolderId;
        /// <summary>
        /// 证件类型
        /// </summary>
        public string idType;
        #endregion
        public _extData extData
        {
            get;
            set;
        }
        public class _extData
        {
            /// <summary>
            /// 手机
            /// </summary>
            public string phone
            {
                get;
                set;
            }
            /// <summary>
            /// 手机验证码
            /// </summary>
            public string validCode
            {
                get;
                set;
            }
            /// <summary>
            /// 是否保存鉴权信息
            /// </summary>
            public string savePciFlag
            {
                get;
                set;
            }
            /// <summary>
            /// 手机验证令牌
            /// </summary>
            public string token
            {
                get;
                set;
            }
            /// <summary>
            /// 快捷支付批次 1首次支付 2再次支付
            /// </summary>
            public string payBatch
            {
                get;
                set;
            }
        }
    }
}
