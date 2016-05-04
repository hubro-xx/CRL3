/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;

/**
* 请求服务地址 配置
*
*/

namespace CRL.Package.OnlinePay.Company.Lianlian
{
	public class ServerURLConfig
	{
		public static string PAY_URL = "https://yintong.com.cn/payment/bankgateway.htm"; // 连连支付WEB收银台支付服务地址
		public static string QUERY_USER_BANKCARD_URL = "https://yintong.com.cn/traderapi/userbankcard.htm"; // 用户已绑定银行卡列表查询
		public static string QUERY_BANKCARD_URL = "https://yintong.com.cn/traderapi/bankcardquery.htm"; //银行卡卡bin信息查询
	}
}

