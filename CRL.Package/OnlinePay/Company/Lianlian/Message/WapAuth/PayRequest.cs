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
    public class PayRequest : MessageBase
    {
        public override string InterFaceUrl
        {
            get
            {
                return "https://yintong.com.cn/llpayh5/authpay.htm";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string version;
        /// <summary>
        /// 商户用户唯一编号
        /// </summary>
        public string user_id;
        /// <summary>
        /// 平台来源标示
        /// </summary>
        public string platform;
        /// <summary>
        /// 请求应用标识
        /// </summary>
        public string app_request;
        /// <summary>
        /// 商户业务类型
        /// </summary>
        public string busi_partner;
        /// <summary>
        /// 商户唯一订单号
        /// </summary>
        public string no_order;
        /// <summary>
        /// 商户订单时间
        /// </summary>
        public string dt_order;
        public string name_goods;
        public string money_order;
        /// <summary>
        /// 服务器异步通知地址
        /// </summary>
        public string notify_url;
        /// <summary>
        /// 支付结束回显ur
        /// </summary>
        public string url_return;
        /// <summary>
        /// 签约协议号
        /// </summary>
        public string no_agree;
        /// <summary>
        /// 订单有效时间
        /// </summary>
        public string valid_order;
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
        /// 分帐信息数据
        /// </summary>
        public string shareing_data;
        /// <summary>
        /// 风险控制参数
        /// </summary>
        public string risk_item;
        /// <summary>
        /// 银行卡号
        /// 银行卡号前置，卡号可以在商户的页面输入
        /// </summary>
        public string card_no;
    }
}
