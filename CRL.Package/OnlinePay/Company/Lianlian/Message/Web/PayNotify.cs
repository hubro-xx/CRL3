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

namespace CRL.Package.OnlinePay.Company.Lianlian.Message.Web
{
    /// <summary>
    /// 交易结果代码 ret_code 是 定(4) 0000
    ///交易结果描述 ret_msg 是 变(100) 交易成功
    /// </summary>
    public class PayNotify : PayReturn
    {
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
    }
}
