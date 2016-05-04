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

namespace CRL.Package.OnlinePay.Company.Lianlian.Message.WapAuth
{
    public class PayNotify : MessageBase
    {
        /// <summary>
        /// 商户订单时间
        /// </summary>
        public string dt_order;

        /// <summary>
        /// 商户唯一订单号
        /// </summary>
        public string no_order;
        /// <summary>
        /// 连连支付支付单号
        /// </summary>
        public string oid_paybill;
        /// <summary>
        /// 交易金额
        /// </summary>
        public string money_order;
        /// <summary>
        /// 支付结果
        /// SUCCESS 成功
        /// </summary>
        public string result_pay;
        /// <summary>
        /// 清算日期
        /// </summary>
        public string settle_date;
        /// <summary>
        /// 订单描述
        /// </summary>
        public string info_order;
        /// <summary>
        /// 支付方式
        /// </summary>
        public string pay_type;
        /// <summary>
        /// 银行编号
        /// </summary>
        public string bank_code;
        /// <summary>
        /// 签约协议号
        /// </summary>
        public string no_agree;
        /// <summary>
        /// 证件类型
        /// </summary>
        public string id_type;
        /// <summary>
        /// 证件号码
        /// </summary>
        public string id_no;
        /// <summary>
        /// 银行账号姓名
        /// </summary>
        public string acct_name;
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string card_no;
    }
}
