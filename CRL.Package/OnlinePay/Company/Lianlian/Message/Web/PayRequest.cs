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
    public class PayRequest : MessageBase
    {
        public override string InterFaceUrl
        {
            get
            {
                return "https://yintong.com.cn/payment/bankgateway.htm";
            }
        }
        /// <summary>
        /// 版本号
        /// </summary>
        public string version;
        /// <summary>
        /// 参数字符编码集
        /// </summary>
        public string charset_name;
        /// <summary>
        /// 商户用户唯一编号
        /// </summary>
        public string user_id;
        /// <summary>
        /// 时间戳
        /// </summary>
        public string timestamp;
        /// <summary>
        /// 商户业务类型
        /// 虚拟商品销售：101001
        /// 实物商品销售：109001
        /// 外部账户充值：108001
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
        /// <summary>
        /// 商品名称
        /// </summary>
        public string name_goods;
        /// <summary>
        /// 订单描述
        /// </summary>
        public string info_order;
        /// <summary>
        /// 交易金额
        /// </summary>
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
        /// 用户端申请 IP
        /// </summary>
        public string userreq_ip;
        /// <summary>
        /// 订单地址
        /// </summary>
        public string url_order;
        /// <summary>
        /// 订单有效时间
        /// </summary>
        public string valid_order;
        /// <summary>
        /// 指定银行网银编号
        /// </summary>
        public string bank_code;
        /// <summary>
        /// 支付方式
        /// </summary>
        public string pay_type;
        /// <summary>
        /// 风险控制参数
        /// </summary>
        public string risk_item;
    }
}
