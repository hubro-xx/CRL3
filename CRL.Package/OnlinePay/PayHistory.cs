/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace CRL.Package.OnlinePay
{
	/// <summary>
	/// 充值/支付订单
    /// 不要继承
	/// </summary>
    [Attribute.Table(TableName = "PayHistory")]
    public class PayHistory : IModelBase
    {
        public override string CheckData()
        {
            return "";
        }
        /// <summary>
        /// 渠道
        /// </summary>
        public int Channel
        {
            get;
            set;
        }
		/// <summary>
		/// 用户编号
		/// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public int UserId
        {
            get;
            set;
        }
        /// <summary>
        /// 帐号类型
        /// </summary>
        public int AccountType
        {
            get;
            set;
        } 
		/// <summary>
		/// 金额
		/// </summary>
        public decimal Amount
        {
            get;
            set;
        }
		/// <summary>
		/// 订单ID
		/// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集唯一)]
        public string OrderId
        {
            get;
            set;
        }

        /// <summary>
        /// 平台订单号,暂支付宝用
        /// </summary>
        public string SpBillno
        {
            get;
            set;
        }
		/// <summary>
		/// 状态,默认Default
		/// </summary>
        public OrderStatus Status
        {
            get;
            set;
        }
		/// <summary>
		/// 支付公司类型
		/// </summary>
        public CompanyType CompanyType
        {
            get;
            set;
        }
		/// <summary>
		/// 订单描述
		/// 存放自定义信息,不存入数据库
		/// </summary>
        public string Desc = "在线支付";

		/// <summary>
		/// 订单类型,默认Charge
		/// </summary>
        public OrderType OrderType
        {
            get;
            set;
        }
        /// <summary>
        /// 外部订单类型
        /// </summary>
        public int OutOrderType
        {
            get;
            set;
        }
		/// <summary>
		/// 产品订单编号,在直接付款时使用
		/// 和SiteType联合使用
		/// </summary>
        public string ProductOrderId
        {
            get;
            set;
        }
		/// <summary>
		/// 银行代码,用第三方支付时传入的银行代码
		/// </summary>
        public string BankType
        {
            get;
            set;
        }
		/// <summary>
		/// 手动转向的URL
		/// 如果要第二次跳转,请传入值
		/// 这里值供其它站点使用
		/// </summary>
        public string RedirectUrl
        {
            get;
            set;
        }
        /// <summary>
        /// 存入自定义数据
        /// </summary>
        [Attribute.Field(Length = 200)]
        public string TagData
        {
            get;
            set;
        }
	}
}
