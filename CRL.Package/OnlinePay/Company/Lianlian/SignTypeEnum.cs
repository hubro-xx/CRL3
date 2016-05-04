/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;


/**
* 签名方式枚举
* @author guoyx
* @date:May 13, 2013 8:22:15 PM
* @version :1.0
*
*/

namespace CRL.Package.OnlinePay.Company.Lianlian
{
	public enum SignTypeEnum
	{
		RSA,MD5
	}

	public class SignTypeEnumClass
	{
		public static string  getCode(SignTypeEnum x)
		{
			switch (x) {
			case SignTypeEnum.MD5:
				return "MD5";
			case SignTypeEnum.RSA:
				return "RSA";
			default:
				return "";
			}
		}
	}
}

