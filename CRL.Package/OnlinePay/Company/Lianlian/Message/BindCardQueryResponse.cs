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
    public class BindCardQueryResponse : MessageBase
    {       
        /// <summary>
        /// 交易结果代码 是 定(4) 0000
        /// </summary>
        public string ret_code;
        /// <summary>
        /// 交易结果描述 是 变(100) 交易成功
        /// </summary>
        public string ret_msg;

        public string user_id;

        public string count;
        public List<agreement_list> agreement_list;
    }
    public class agreement_list
    {
        public string no_agree;
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
        public string card_no;
        public string bind_mobile;
    }
}
