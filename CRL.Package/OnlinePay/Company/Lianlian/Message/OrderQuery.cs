using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.OnlinePay.Company.Lianlian.Message
{
    public class OrderQuery : MessageBase
    {
        public override string InterFaceUrl
        {
            get
            {
                return "https://yintong.com.cn/traderapi/orderquery.htm";
            }
        }
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
        /// 查询版本号
        /// </summary>
        public string query_version;
    }
}
