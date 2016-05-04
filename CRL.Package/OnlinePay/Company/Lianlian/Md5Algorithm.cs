/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace CRL.Package.OnlinePay.Company.Lianlian
{

	/**
	 * 32位MD5摘要算法
 	 * @author shmily
	 * @date 2012-6-28 下午02:34:45
	 */
	public sealed class Md5Algorithm {

		public Md5Algorithm(){

		}

		private static Md5Algorithm instance;

		public static Md5Algorithm getInstance(){
			if(null == instance)
				return new Md5Algorithm();
			return instance;
		}



		private static String[] hexDigits = { "0", "1", "2", "3", "4", "5",
			"6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };

		/**
	 * 转换字节数组�?6进制字串
	 * @param b 字节数组
	 * @return 16进制字串
	 */
		private String byteArrayToHexString(byte[] b) {
			StringBuilder resultSb = new StringBuilder();
			for (int i = 0; i < b.Length; i++) {
				resultSb.Append(byteToHexString(b[i]));
			}
			return resultSb.ToString();
		}

		/**
	 * 转换字节数组为高位字符串
	 * @param b 字节数组
	 * @return
	 */
		private String byteToHexString(byte b) {
			int n = b;
			if (n < 0)
				n = 256 + n;
			int d1 = n / 16;
			int d2 = n % 16;
			return hexDigits[d1] + hexDigits[d2];
		}

		/**
	 * MD5 摘要计算(byte[]).
	 * @param src byte[]
	 * @throws Exception
	 * @return String
	 */
		public String md5Digest(byte[] src) {
			try {
				// MD5 is 32 bit message digest
				MD5 md5 = new MD5CryptoServiceProvider();
				return byteArrayToHexString(md5.ComputeHash(src));
			} catch (Exception e) {
				Console.WriteLine ("异常:" + e.Message);
				return null;
			} 

		}
	}
}
