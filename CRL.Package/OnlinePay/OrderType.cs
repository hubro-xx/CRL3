/**
* CRL 快速开发框架 V4.5
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
	/// 订单类型
	/// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 充值
        /// </summary>
        充值 = 0,
        /// <summary>
        /// 支付
        /// </summary>
        支付 = 1
    }
}
