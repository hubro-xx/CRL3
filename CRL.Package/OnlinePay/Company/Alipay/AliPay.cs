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
using System.Text;
using System.Security.Cryptography;

namespace CRL.Package.OnlinePay.Company.Alipay
{
	public class AliPay
	{


		/// <summary>
		/// 与ASP兼容的MD5加密算法
		/// </summary>
		public static string GetMD5(string s, string _input_charset)
		{
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(s));
			StringBuilder sb = new StringBuilder(32);
			for (int i = 0; i < t.Length; i++)
			{
				sb.Append(t[i].ToString("x").PadLeft(2, '0'));
			}
			return sb.ToString();
		}

		/// <summary>
		/// 冒泡排序法
		/// </summary>
		public static string[] BubbleSort(string[] r)
		{
			int i, j; //交换标志 
			string temp;

			bool exchange;

			for (i = 0; i < r.Length; i++) //最多做R.Length-1趟排序 
			{
				exchange = false; //本趟排序开始前，交换标志应为假

				for (j = r.Length - 2; j >= i; j--)
				{
					if (System.String.CompareOrdinal(r[j + 1], r[j]) < 0)　//交换条件
					{
						temp = r[j + 1];
						r[j + 1] = r[j];
						r[j] = temp;

						exchange = true; //发生了交换，故将交换标志置为真 
					}
				}

				if (!exchange) //本趟排序未发生交换，提前终止算法 
				{
					break;
				}
			}
			return r;
		}
		/// <summary>
		/// 生成查询订单URL
		/// </summary>
		/// <param name="gateway"></param>
		/// <param name="service"></param>
		/// <param name="partner"></param>
		/// <param name="sign_type"></param>
		/// <param name="out_trade_no"></param>
		/// <param name="key"></param>
		/// <param name="input_charset"></param>
		/// <returns></returns>
		public static string CreateQueryOrderUrl(string gateway, string service, string partner, string sign_type,
			string out_trade_no, string key, string input_charset)
		{
			string[] Oristr ={ 
                "service="+service, 
                "partner=" + partner, 
				"out_trade_no=" + out_trade_no, 
                "_input_charset="+input_charset
                };
			//进行排序；
			string[] Sortedstr = BubbleSort(Oristr);


			//构造待md5摘要字符串 ；

			StringBuilder prestr = new StringBuilder();
			int i;
			for (i = 0; i < Sortedstr.Length; i++)
			{
				if (i == Sortedstr.Length - 1)
				{
					prestr.Append(Sortedstr[i]);

				}
				else
				{
					prestr.Append(Sortedstr[i] + "&");
				}

			}

			prestr.Append(key);
			//生成Md5摘要；
			string sign = GetMD5(prestr.ToString(), input_charset);

			//构造支付Url；
			StringBuilder parameter = new StringBuilder();
			parameter.Append(gateway);
			for (i = 0; i < Sortedstr.Length; i++)
			{
				parameter.Append(Sortedstr[i] + "&");
			}

			parameter.Append("sign=" + sign + "&sign_type=" + sign_type);

			//返回支付Url；
			return parameter.ToString();
		}
		public static string CreatUrl(
			string gateway,
			string service,
			string partner,
			string sign_type,
			string out_trade_no,
			string subject,
			string body,
			string total_fee,
			string show_url,
			string seller_email,
			string key,
			string return_url,
			string _input_charset,
			string notify_url,
            string bankType
			)
		{
			/// <summary>
			/// created by sunzhizhi 2006.5.21,sunzhizhi@msn.com。
			/// </summary>
			int i;
            string paymethod = string.Empty;
            //高级网银支付
            string[] OristrA ={ 
                "service="+service, 
                "partner=" + partner, 
                "subject=" + subject, 
                "body=" + body, 
                "out_trade_no=" + out_trade_no, 
                "total_fee=" + total_fee, 
                "show_url=" + show_url,  
                "payment_type=1", 
                "seller_email=" + seller_email, 
                "notify_url=" + notify_url,
                "_input_charset="+_input_charset,          
                "return_url=" + return_url,
                "paymethod=bankPay",
                "defaultbank="+bankType
                };
            //普通支付
            string[] OristrB ={ 
                "service="+service, 
                "partner=" + partner, 
                "subject=" + subject, 
                "body=" + body, 
                "out_trade_no=" + out_trade_no, 
                "total_fee=" + total_fee, 
                "show_url=" + show_url,  
                "payment_type=1", 
                "seller_email=" + seller_email, 
                "notify_url=" + notify_url,
                "_input_charset="+_input_charset,          
                "return_url=" + return_url
                };
           

			//进行排序；
            string[] Sortedstr = (bankType != "" && bankType != "alipay") ? BubbleSort(OristrA) : BubbleSort(OristrB);


			//构造待md5摘要字符串 ；

			StringBuilder prestr = new StringBuilder();

			for (i = 0; i < Sortedstr.Length; i++)
			{
				if (i == Sortedstr.Length - 1)
				{
					prestr.Append(Sortedstr[i]);

				}
				else
				{

					prestr.Append(Sortedstr[i] + "&");
				}

			}

			prestr.Append(key);

			//生成Md5摘要；
			string sign = GetMD5(prestr.ToString(), _input_charset);

			//构造支付Url；
			StringBuilder parameter = new StringBuilder();
			parameter.Append(gateway);
			for (i = 0; i < Sortedstr.Length; i++)
			{
				parameter.Append(Sortedstr[i] + "&");
			}

			parameter.Append("sign=" + sign + "&sign_type=" + sign_type);

			//返回支付Url；
			return parameter.ToString();

		}
        /// <summary>
        /// 添加为存在token 的方法
        /// </summary>
        /// <param name="gateway"></param>
        /// <param name="service"></param>
        /// <param name="partner"></param>
        /// <param name="sign_type"></param>
        /// <param name="out_trade_no"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="total_fee"></param>
        /// <param name="show_url"></param>
        /// <param name="seller_email"></param>
        /// <param name="key"></param>
        /// <param name="return_url"></param>
        /// <param name="_input_charset"></param>
        /// <param name="notify_url"></param>
        /// <param name="bankType"></param>
        /// <param name="token">支付宝登录过程中返回的记录值</param>
        /// <returns></returns>
        public static string CreatUrl(
    string gateway,
    string service,
    string partner,
    string sign_type,
    string out_trade_no,
    string subject,
    string body,
    string total_fee,
    string show_url,
    string seller_email,
    string key,
    string return_url,
    string _input_charset,
    string notify_url,
    string bankType,
    string token
    )
        {
            /// <summary>
            /// created by sunzhizhi 2006.5.21,sunzhizhi@msn.com。
            /// </summary>
            int i;
            string paymethod = string.Empty;
            //高级网银支付
            string[] OristrA ={ 
                "service="+service, 
                "partner=" + partner, 
                "subject=" + subject, 
                "body=" + body, 
                "out_trade_no=" + out_trade_no, 
                "total_fee=" + total_fee, 
                "show_url=" + show_url,  
                "payment_type=1", 
                "seller_email=" + seller_email, 
                "notify_url=" + notify_url,
                "_input_charset="+_input_charset,          
                "return_url=" + return_url,
                "paymethod=bankPay",
                "defaultbank="+bankType,
                "token="+token
                };
            //普通支付
            string[] OristrB ={ 
                "service="+service, 
                "partner=" + partner, 
                "subject=" + subject, 
                "body=" + body, 
                "out_trade_no=" + out_trade_no, 
                "total_fee=" + total_fee, 
                "show_url=" + show_url,  
                "payment_type=1", 
                "seller_email=" + seller_email, 
                "notify_url=" + notify_url,
                "_input_charset="+_input_charset,          
                "return_url=" + return_url,
                "token="+token
                };


            //进行排序；
            string[] Sortedstr = (bankType != "" && bankType != "alipay") ? BubbleSort(OristrA) : BubbleSort(OristrB);


            //构造待md5摘要字符串 ；

            StringBuilder prestr = new StringBuilder();

            for (i = 0; i < Sortedstr.Length; i++)
            {
                if (i == Sortedstr.Length - 1)
                {
                    prestr.Append(Sortedstr[i]);

                }
                else
                {

                    prestr.Append(Sortedstr[i] + "&");
                }

            }

            prestr.Append(key);

            //生成Md5摘要；
            string sign = GetMD5(prestr.ToString(), _input_charset);

            //构造支付Url；
            StringBuilder parameter = new StringBuilder();
            parameter.Append(gateway);
            for (i = 0; i < Sortedstr.Length; i++)
            {
                parameter.Append(Sortedstr[i] + "&");
            }

            parameter.Append("sign=" + sign + "&sign_type=" + sign_type);

            //返回支付Url；
            return parameter.ToString();

        }

	}
}
