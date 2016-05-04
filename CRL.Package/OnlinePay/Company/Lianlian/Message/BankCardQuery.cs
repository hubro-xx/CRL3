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

namespace CRL.Package.OnlinePay.Company.Lianlian.Message
{
    public class BankCardQuery : MessageBase
    {
        public override string InterFaceUrl
        {
            get
            {
                return "https://yintong.com.cn/traderapi%20/bankcardquery.htm";
            }
        }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string card_no;
        /// <summary>
        /// 支付方式
        /// 2：快捷支付 （默认）D：认证支付
        /// </summary>
        public string pay_type;
        /// <summary>
        /// 是否返回限额标 0：不返回（默认）
        /// </summary>
        public string flag_amt_limit;
    }
}
