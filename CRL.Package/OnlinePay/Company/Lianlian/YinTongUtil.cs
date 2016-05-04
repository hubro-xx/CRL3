/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace CRL.Package.OnlinePay.Company.Lianlian
{
	public class YinTongUtil
	{
		public YinTongUtil ()
		{
		}

		//得到时间戳
		public static String getCurrentDateTimeStr()
		{
			return DateTime.Now.ToString ("yyyyMMddHHmmss");
		}

		//签名入口
		public static String addSign(SortedDictionary<string, string> sParaTemp, String rsa_private,
			String md5_key)
		{
			if (sParaTemp == null)
			{
				return "";
			}
			String sign_type;
			if (!sParaTemp.TryGetValue ("sign_type", out sign_type))   //没指定签名方式
			{
				return "";
			}
			if (SignTypeEnumClass.getCode(SignTypeEnum.MD5).Equals(sign_type))
			{
				return addSignMD5(sParaTemp, md5_key);
			} else
			{
				return addSignRSA(sParaTemp, rsa_private);
			}
		}

		//MD5签名
		private static String addSignMD5(SortedDictionary<string, string> sParaTemp, String md5_key)
		{
			string oid_partner;
			sParaTemp.TryGetValue ("sign_type", out oid_partner);

			Console.WriteLine("进入商户[" + oid_partner + "]MD5加签名");

			if (sParaTemp == null)
			{
				return "";
			}
			// 生成签名原串
			String sign_src = genSignData(sParaTemp);

			Console.WriteLine("商户[" + oid_partner + "]加签原串"
				+ sign_src);
			Console.WriteLine("MD5签名key:" + md5_key);

			sign_src += "&key=" + md5_key;

			try
			{
				string sign = Md5Algorithm.getInstance().md5Digest(
					Encoding.UTF8.GetBytes (sign_src));
				Console.WriteLine("商户[" + oid_partner + "]签名结果"
					+ sign);
				return sign;

			} catch (Exception e)
			{
				Console.WriteLine("商户[" + oid_partner
					+ "] MD5加签名异常" + e.Message);
				return "";
			}
		}

		//RSA签名
		private static String addSignRSA(SortedDictionary<string, string> sParaTemp, String rsa_private)
		{
			string oid_partner;
			sParaTemp.TryGetValue ("sign_type", out oid_partner);
			Console.WriteLine("进入商户[" + oid_partner + "]MD5加签名");

			if (sParaTemp == null)
			{
				return "";
			}
			// 生成签名原串
			String sign_src = genSignData(sParaTemp);
			Console.WriteLine("商户[" + oid_partner + "]加签原串"
				+ sign_src);
			Console.WriteLine("RSA签名key:" + rsa_private);
			try
			{
				string sign = RSAFromPkcs8.sign(sign_src,rsa_private,"utf-8");
				Console.WriteLine("商户[" + oid_partner + "]签名结果"
					+ sign);
				return sign;
			} catch (Exception e)
			{
				Console.WriteLine("商户[" + oid_partner + "]RSA加签名异常" + e.Message);
				return "";
			}
		}

		// 生成签名原串
		public static String genSignData(SortedDictionary<string, string> sParaTemp)
		{
			StringBuilder content = new StringBuilder();
			foreach (KeyValuePair<string, string> temp in sParaTemp)
			{
				//"sign"不参与签名
				if ("sign".Equals(temp.Key))
				{
					continue;
				}
				// 空串不参与签名
				if (isnull(temp.Value))
				{
					continue;
				}
				content.Append("&" + temp.Key + "=" + temp.Value);
			}
			String signSrc = content.ToString();
			if (signSrc.StartsWith("&"))
			{
				signSrc = signSrc.Substring (1);
			}
			return signSrc;
		}

		//验签入口
		public static bool checkSign(SortedDictionary<string, string> sParaTemp, String rsa_public, String md5_key)
		{
			if (sParaTemp == null)
			{
				return false;
			}
			String sign_type;
			if (!sParaTemp.TryGetValue ("sign_type", out sign_type))   //没指定签名方式
			{
				return false;
			}
			if (SignTypeEnumClass.getCode(SignTypeEnum.MD5).Equals(sign_type))
			{
				return checkSignMD5(sParaTemp, md5_key);
			} else
			{
				return checkSignRSA(sParaTemp, rsa_public);
			}
		}

		//MD5验签
		private static bool checkSignMD5(SortedDictionary<string, string> sParaTemp, String md5_key)
		{
			string oid_partner;
			sParaTemp.TryGetValue ("sign_type", out oid_partner);
			Console.WriteLine("进入商户[" + oid_partner + "]MD5签名验证");

			if (sParaTemp == null)
			{
				return false;
			}
			String sign;
			if (!sParaTemp.TryGetValue ("sign", out sign))   
			{
				return false;
			}

			// 生成签名原串
			String sign_src = genSignData(sParaTemp);
			Console.WriteLine("商户[" + oid_partner + "]待签名原串"
				+ sign_src);
			Console.WriteLine("商户[" + oid_partner + "]签名串"
				+ sign);

			sign_src += "&key=" + md5_key;
			try
			{
				if (sign.Equals(Md5Algorithm.getInstance().md5Digest(
					Encoding.UTF8.GetBytes (sign_src))))
				{
					Console.WriteLine("商户[" + oid_partner
						+ "]MD5签名验证通过");
					return true;
				} else
				{
					Console.WriteLine("商户[" + oid_partner
						+ "]MD5签名验证未通过");
					return false;
				}
			} catch (Exception e)
			{
				Console.WriteLine("商户[" + oid_partner
					+ "]MD5签名验证异常" + e.Message);
				return false;
			}
		}

		//RSA验签
		private static bool checkSignRSA(SortedDictionary<string, string> sParaTemp, String rsa_public)
		{

			string oid_partner;
			sParaTemp.TryGetValue ("sign_type", out oid_partner);
			Console.WriteLine("进入商户[" + oid_partner + "]MD5签名验证");

			if (sParaTemp == null)
			{
				return false;
			}
			String sign;
			if (!sParaTemp.TryGetValue ("sign", out sign))   
			{
				return false;
			}

			// 生成签名原串
			String sign_src = genSignData(sParaTemp);

			Console.WriteLine("商户[" + oid_partner + "]待签名原串"
				+ sign_src);
			Console.WriteLine("商户[" + oid_partner + "]签名串"
				+ sign);
			try
			{
				if (RSAFromPkcs8.verify(sign_src,sign,rsa_public,"UTF-8" ))
				{
					Console.WriteLine("商户[" + oid_partner
						+ "]RSA签名验证通过");
					return true;
				} else
				{
					Console.WriteLine("商户[" + oid_partner
						+ "]RSA签名验证未通过");
					return false;
				}
			} catch (Exception e)
			{
				Console.WriteLine("商户[" + oid_partner
					+ "]RSA签名验证异常" + e.Message);
				return false;
			}
		}

		//判断字符串是否为空
		public static bool isnull(String str)
		{

			if (null == str || str.ToLower().Equals("null") || str.Equals(""))
			{
				return true;
			} else
				return false;
		}

		//有序字典（最简单的有序字典）转为json
		public static string dictToJson(SortedDictionary<string, string> dict)
		{
			StringBuilder json = new StringBuilder();
			json.Append ("{");
			foreach (KeyValuePair<string, string> temp in dict)
			{
				json.Append("\"" + temp.Key + "\"" + ":"  + "\"" + temp.Value + "\"");
				json.Append (",");
			}
			json.Remove (json.Length-1, 1);
			json.Append ("}");
			string content = json.ToString ();
			return content;
		}

		//本地IP地址，不是远程client IP地址 
		//本程序无用
		public static string LocalIPAddress()
		{
			IPHostEntry host;
			string localIP = "";
			host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					localIP = ip.ToString();
					break;
				}
			}
			return localIP;
		}

	}
}

