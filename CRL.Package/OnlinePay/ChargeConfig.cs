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
using System.Collections.Generic;
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
	public class ChargeConfig
	{
        public enum DataType
        {
            /// <summary>
            /// 密钥
            /// </summary>
            Key,
            /// <summary>
            /// 用户号
            /// </summary>
            User,
            /// <summary>
            /// 相关EMAIL
            /// </summary>
            Email,
            /// <summary>
            /// 通知URL
            /// </summary>
            NotifyUrl,
            /// <summary>
            /// 转向URL
            /// </summary>
            ReturnUrl,
            /// <summary>
            /// 退款URL
            /// </summary>
            RefundUrl,
            /// <summary>
            /// 证书路径
            /// </summary>
            CertFile,
            /// <summary>
            /// 证书密码
            /// </summary>
            CertFilePass,
            /// <summary>
            /// 支付密码
            /// </summary>
            PayPass,
            /// <summary>
            /// 自定义数据
            /// </summary>
            Data
        }
		#region 读取KEY
        public static string GetConfigKey(CompanyType companyType, string type)
        {
            string key = companyType + type.ToString();
            string result = CoreHelper.CustomSetting.GetConfigKey(key);
            return result;
        }
		/// <summary>
        /// 依赖CustomSetting,名称定义格式为
        /// AlipayKey=key
        /// AlipayUser=user
        /// AlipayEmail=abc@163.com
		/// </summary>
        /// <param name="companyType"></param>
        /// <param name="type"></param>
		/// <returns></returns>
		public static string GetConfigKey(CompanyType companyType,DataType type)
		{
            string key = companyType + type.ToString();
            //if (accountKeys.Count == 0)
            //{
            //    string file = System.Web.Hosting.HostingEnvironment.MapPath("/charge/charge.config");
            //    string content = System.IO.File.ReadAllText(file);
            //    string entryKey = "S8S7FLDL";
            //    content = CoreHelper.StringHelper.Decrypt(content, entryKey);
            //    string[] arry = content.Split('\n');
            //    foreach (string str in arry)
            //    {
            //        string[] arry1 = str.Trim().Split('=');
            //        accountKeys.Add(new KeySetting() { Key = arry1[0], Value = arry1[1] });
            //    }
            //}
            //KeySetting set = accountKeys.Find(b => b.Key.ToUpper() == key.ToUpper());
            //if (set != null)
            //    return set.Value;
            string result = CoreHelper.CustomSetting.GetConfigKey(key);
            return result;
		}
		#endregion

        public static string Charset = HttpContext.Current.Request.ContentEncoding.BodyName;
	}
}
