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
using CoreHelper;
namespace CRL.Package.OnlinePay
{
	/// <summary>
	/// 订单状态
	/// </summary>
	public enum OrderStatus
	{
		/// <summary>
		/// 默认
		/// </summary>
		已提交=0,
		/// <summary>
		/// 超时
		/// </summary>
		已过期=1,
		/// <summary>
        ///已确认
		/// </summary>
		已确认=2,
        /// <summary>
        /// 已退款
        /// </summary>
        已退款 = 3
	}
}
