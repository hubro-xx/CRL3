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

