using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.OnlinePay.Company.Lianlian.Message
{
    public class BankCardQueryResponse : MessageBase
    {
        /// <summary>
        /// 交易结果代码 是 定(4) 0000
        /// </summary>
        public string ret_code;
        /// <summary>
        /// 交易结果描述 是 变(100) 交易成功
        /// </summary>
        public string ret_msg;
        
        /// <summary>
        /// 所属银行编号 是 定(8)
        /// </summary>
        public string bank_code;
        /// <summary>
        /// 所属银行名称 是 变(32)
        /// </summary>
        public string bank_name;
        /// <summary>
        /// 银行卡类型 是 定(1) 2-储蓄卡 3-信用卡
        /// </summary>
        public string card_type;
        /// <summary>
        /// 单笔限额 否 String 单位 元
        /// </summary>
        public string single_amt;
        /// <summary>
        /// 单日限额 否 String 单位 元
        /// </summary>
        public string day_amt;
        /// <summary>
        /// 单月限额 否 String 单位 元
        /// </summary>
        public string month_amt;
    }
}
