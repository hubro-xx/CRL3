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
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Collections.Specialized;
using System.Net;
using System.Collections.Generic;

namespace CRL.Package.OnlinePay.Company.Alipay
{
	public class AlipayCompany : CompanyBase
	{
        public override CompanyType ThisCompanyType
        {
            get { return CompanyType.支付宝; }
        }
		public AlipayCompany()
		{
			key = ChargeConfig.GetConfigKey(CompanyType.支付宝, ChargeConfig.DataType.Key);
		}
		#region 参数
		//业务参数赋值；
        string company_gateway = "https://mapi.alipay.com/gateway.do?";
		string service = "create_direct_pay_by_user";                       //服务名称

        string seller_email
        {
            get
            {
                return ChargeConfig.GetConfigKey(CompanyType.支付宝, ChargeConfig.DataType.Email);
            }
        }
        //获取或设置付款人真实姓名，参数email对应的真实姓名
        private string _account_name = "";
		string sign_type = Config.Sign_type;                                           //加密类型,签名方式“不用改”
		string key = "";//安全校验码
        string partner = Config.Partner;                                //商户ID，合作ID
		string _input_charset = Config.Input_charset;                                   //编码类型

        string show_url
        {
            get
            {
                return new Uri(CurrentHost).Host;
            }
        }
        //static string URL =CurrentHost + "";
        //static string URL = "http://localhost";

        string return_url
        {
            get
            {
                return CurrentHost + ChargeConfig.GetConfigKey(ThisCompanyType, ChargeConfig.DataType.ReturnUrl);
            }
        }
        string notify_url
        {
            get
            {
                return CurrentHost + ChargeConfig.GetConfigKey(ThisCompanyType, ChargeConfig.DataType.NotifyUrl);
            }
        }

        string alipayNotifyURL = "https://mapi.alipay.com/gateway.do?service=notify_verify";
        string refundNotifyUrl
        {
            get
            {
                return CurrentHost + ChargeConfig.GetConfigKey(ThisCompanyType, ChargeConfig.DataType.RefundUrl);
            }
        }


		#endregion

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestPost(HttpContext context)
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = context.Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], context.Request.Form[requestItem[i]]);
            }

            return sArray;
        }
        /// <summary>
        /// 获取通知,按具体需求重写
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override string OnNotify(HttpContext context)
        {
            SortedDictionary<string, string> sPara = GetRequestPost(context);

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, context.Request.Form["notify_id"], context.Request.Form["sign"]);

                if (verifyResult)//验证成功
                {
                    string strOrderNO = context.Request.Form["out_trade_no"];//订单号

                    string strPrice = context.Request.Form["total_fee"];//金额
                    string trade_no = context.Request.Form["trade_no"];//支付宝订单号

                    string notify_type = context.Request.Form["notify_type"];
                    string trade_status = context.Request.Form["trade_status"];//交易状态
                    string refund_status = context.Request.Form["refund_status"];//退款状态
                    string gmt_refund = context.Request.Form["refund_status"];//退款时间

                    if (notify_type == "trade_status_sync")
                    {
                        #region 支付&退款
                        if (trade_status == "WAIT_BUYER_PAY")//   判断支付状态_等待买家付款（文档中有枚举表可以参考）            
                        {
                            Log("WAIT_BUYER_PAY" + strOrderNO);
                            context.Response.Write("success");     //返回给支付宝消息，成功，请不要改写这个success
                        }//支付状态成功并且退款通知为NULL
                        else if ((trade_status == "TRADE_FINISHED" || trade_status == "TRADE_SUCCESS") && string.IsNullOrEmpty(refund_status))
                        {
                            #region 支付处理
                            PayHistory order = OnlinePayBusiness.Instance.GetOrder(strOrderNO, ThisCompanyType);
                            if (order == null)
                            {
                                CoreHelper.EventLog.Log(string.Format("在线支付支付成功，确认时找不到订单{0} 订单号{1}", ThisCompanyType, strOrderNO), true);
                                //context.Response.Write("fail");
                                //return "fail";
                            }
                            else
                            {
                                order.SpBillno = trade_no;
                                Confirm(order, GetType(), Convert.ToDecimal(strPrice));
                            }
                            #endregion
                            //context.Response.Write("success");     //返回给支付宝消息，成功，请不要改写这个success
                            return "success";
                        }
                        else if (refund_status == "REFUND_SUCCESS")//退款成功
                        {
                            #region 退款处理
                            Log("收到退款通知:" + strOrderNO);
                            PayHistory order = OnlinePayBusiness.Instance.GetOrder(strOrderNO, ThisCompanyType);
                            if (order == null)
                            {
                                CoreHelper.EventLog.Log(string.Format("在线支付支付成功，确认时找不到订单{0} 订单号{1}", ThisCompanyType, strOrderNO), true);
                                //context.Response.Write("fail");
                                //return "fail";
                            }
                            else
                            {
                               BaseRefundOrder(order); 
                            }                            
                            #endregion
                            //context.Response.Write("success");
                            return "success";
                        }
                        else
                        {
                            //context.Response.Write("fail");
                            return "fail";
                        }
                        #endregion
                    }
                    else if (notify_type == "batch_trans_notify")
                    {
                        #region 批量付款通知
                        /***
                    string batch_no = context.Request.Form["batch_no"];
                    string success_details = context.Request.Form["success_details"];
                    string fail_details = context.Request.Form["success_details"];
                    List<BatchPayItem> arry1 = BatchPayItem.FromDetail(success_details);
                    List<BatchPayItem> arry2 = BatchPayItem.FromDetail(fail_details);
                    foreach(BatchPayItem a in arry1)
                    {
                        HotelBookModel.AccountHistory item = new HotelBookModel.AccountHistory();
                        item.OrderId = a.OrderNo;
                        item.Status = HotelBookModel.AccountHistory.AccountStatus.Success;
                        item.Remark = "转帐成功:" + a.Remark;
                        item.BatchNo = batch_no;
                        AccountHistoryAction.UpdateStatus(item);
                    } 
                    foreach (BatchPayItem a in arry2)
                    {
                        HotelBookModel.AccountHistory item = new HotelBookModel.AccountHistory();
                        item.OrderId = a.OrderNo;
                        item.Status = HotelBookModel.AccountHistory.AccountStatus.Fail;
                        item.Remark = "转帐失败:" + a.Remark;
                        item.BatchNo = batch_no;
                        AccountHistoryAction.UpdateStatus(item);
                    }
                    context.Response.Write("success");
                     * **/
                        #endregion
                        return "success";
                    }

                }
                else//验证失败
                {
                    return "fail";
                }
            }
            else
            {
                return "无通知参数";
            }
            return "fail";
        }
		/// <summary>
		/// 获取通知,按具体需求重写
		/// </summary>
		/// <param name="context"></param>
        protected string OnNotify2(HttpContext context)
		{
			alipayNotifyURL = alipayNotifyURL + "&partner=" + partner + "&notify_id=" + context.Request.Form["notify_id"];

			//获取支付宝ATN返回结果，true是正确的订单信息，false 是无效的
			string responseTxt = CoreHelper.HttpRequest.HttpGet(alipayNotifyURL, Encoding.Default);

			int i;
			NameValueCollection coll = context.Request.Form;

			// Get names of all forms into a string array.
			String[] requestarr = coll.AllKeys;

			//进行排序；
			string[] Sortedstr = AliPay.BubbleSort(requestarr);

			//构造待md5摘要字符串 ；
			string prestr = "";
			for (i = 0; i < Sortedstr.Length; i++)
			{
				if (context.Request.Form[Sortedstr[i]] != "" && Sortedstr[i] != "sign" && Sortedstr[i] != "sign_type")
				{
					if (i == Sortedstr.Length - 1)
					{
						prestr = prestr + Sortedstr[i] + "=" + context.Request.Form[Sortedstr[i]];
					}
					else
					{
						prestr = prestr + Sortedstr[i] + "=" + context.Request.Form[Sortedstr[i]] + "&";
					}
				}
			}
			prestr = prestr + key;

			string mysign = AliPay.GetMD5(prestr, _input_charset);

			string sign = context.Request.Form["sign"];

			string strOrderNO = context.Request.Form["out_trade_no"];//订单号
					
			string strPrice = context.Request.Form["total_fee"];//金额
            string trade_no = context.Request.Form["trade_no"];//支付宝订单号

            string notify_type = context.Request.Form["notify_type"];
            string trade_status = context.Request.Form["trade_status"];//交易状态
            string refund_status = context.Request.Form["refund_status"];//退款状态
            string gmt_refund = context.Request.Form["refund_status"];//退款时间

			if (mysign == sign && responseTxt == "true")   //验证支付发过来的消息，签名是否正确
			{
                if (notify_type == "trade_status_sync")
                {
                    #region 支付&退款
                    if (trade_status == "WAIT_BUYER_PAY")//   判断支付状态_等待买家付款（文档中有枚举表可以参考）            
                    {
                        Log("WAIT_BUYER_PAY" + strOrderNO);
                        context.Response.Write("success");     //返回给支付宝消息，成功，请不要改写这个success
                    }//支付状态成功并且退款通知为NULL
                    else if ((trade_status == "TRADE_FINISHED" || trade_status == "TRADE_SUCCESS") && string.IsNullOrEmpty(refund_status))
                    {
                        #region 支付处理
                        PayHistory order = OnlinePayBusiness.Instance.GetOrder(strOrderNO, ThisCompanyType);
                        order.SpBillno = trade_no;
                        Confirm(order, GetType(), Convert.ToDecimal(strPrice));
                        #endregion
                        //context.Response.Write("success");     //返回给支付宝消息，成功，请不要改写这个success
                        return "success";
                    }
                    else if (refund_status == "REFUND_SUCCESS")//退款成功
                    {
                        #region 退款处理
                        Log("收到退款通知:" + strOrderNO);
                        PayHistory order = OnlinePayBusiness.Instance.GetOrder(strOrderNO, ThisCompanyType);
                        BaseRefundOrder(order);
                        #endregion
                        //context.Response.Write("success");
                        return "success";
                    }
                    else
                    {
                        //context.Response.Write("fail");
                        return "fail";
                    }
                    #endregion
                }
                else if (notify_type == "batch_trans_notify")
                {
                    #region 批量付款通知
                    /***
                    string batch_no = context.Request.Form["batch_no"];
                    string success_details = context.Request.Form["success_details"];
                    string fail_details = context.Request.Form["success_details"];
                    List<BatchPayItem> arry1 = BatchPayItem.FromDetail(success_details);
                    List<BatchPayItem> arry2 = BatchPayItem.FromDetail(fail_details);
                    foreach(BatchPayItem a in arry1)
                    {
                        HotelBookModel.AccountHistory item = new HotelBookModel.AccountHistory();
                        item.OrderId = a.OrderNo;
                        item.Status = HotelBookModel.AccountHistory.AccountStatus.Success;
                        item.Remark = "转帐成功:" + a.Remark;
                        item.BatchNo = batch_no;
                        AccountHistoryAction.UpdateStatus(item);
                    } 
                    foreach (BatchPayItem a in arry2)
                    {
                        HotelBookModel.AccountHistory item = new HotelBookModel.AccountHistory();
                        item.OrderId = a.OrderNo;
                        item.Status = HotelBookModel.AccountHistory.AccountStatus.Fail;
                        item.Remark = "转帐失败:" + a.Remark;
                        item.BatchNo = batch_no;
                        AccountHistoryAction.UpdateStatus(item);
                    }
                    context.Response.Write("success");
                     * **/
                    #endregion
                    return "success";
                }
			}
			else
			{
				//context.Response.Write("fail");
                return "fail";
			}
            return "fail";
		}
		/// <summary>
		/// 提交支付,转向需要按具体需求重写
		/// </summary>
		/// <param name="order"></param>
		public override void Submit(PayHistory order)
		{
			BaseSubmit(order);
			string out_trade_no =order.OrderId;
			string subject = order.Desc;                           //商品名称
            string body = order.UserId.ToString();                                  //商品描述
			string price = order.Amount.ToString();                                //商品价格
            string bankType = order.BankType; //银行代码
            string token = (HttpContext.Current.Session["ali_token"] != null) ? HttpContext.Current.Session["ali_token"].ToString() : string.Empty;  // 支付宝来源用户的验证码

			//相关参数名称具体含义，可以在支付宝接口服务文档中查询到，
			//以上两个文件，通知正常都可以在notify data目录找到通知过来的日志
            string aliay_url = string.Empty;
            if (token == string.Empty)
            {
                //HttpContext.Current.Request.QueryString["is_company"] 判断企业网银支付渠道
                if (HttpContext.Current.Request.QueryString["is_company"] != null && HttpContext.Current.Request.QueryString["is_company"].ToString()!="")
                {
                    aliay_url = AliPay.CreatUrl(company_gateway, service, partner, sign_type, out_trade_no, subject, body, price, show_url, seller_email, key, return_url, _input_charset, notify_url, bankType);
                }
                else
                {
                    aliay_url = AliPay.CreatUrl(company_gateway, service, partner, sign_type, out_trade_no, subject, body, price, show_url, seller_email, key, return_url, _input_charset, notify_url, bankType);
                }
            }
            else
            {
                if (HttpContext.Current.Request.QueryString["is_company"] != null && HttpContext.Current.Request.QueryString["is_company"].ToString() != "")
                {
                    aliay_url = AliPay.CreatUrl(company_gateway, service, partner, sign_type, out_trade_no, subject, body, price, show_url, seller_email, key, return_url, _input_charset, notify_url, bankType, token);
                }
                else
                {
                    aliay_url = AliPay.CreatUrl(company_gateway, service, partner, sign_type, out_trade_no, subject, body, price, show_url, seller_email, key, return_url, _input_charset, notify_url, bankType, token);
                }
            }
            //HttpContext.Current.Response.Write(aliay_url);
            //HttpContext.Current.Response.End();
			HttpContext.Current.Response.Redirect(aliay_url);

		}
		/// <summary>
		/// 查询订单状态,返回是否成功
		/// </summary>
		/// <param name="order"></param>
		/// <returns></returns>
		public override bool CheckOrder(PayHistory order,out string message)
		{
            string url = AliPay.CreateQueryOrderUrl(company_gateway, "single_trade_query", partner, sign_type, order.OrderId, key, _input_charset);
			string responseTxt = CoreHelper.HttpRequest.HttpGet(url, Encoding.Default);
			try
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(responseTxt);
				XmlNode node = doc.SelectSingleNode("alipay/response/trade/trade_status");
				if (node == null)
				{
					message = "服务器返回:" + responseTxt;
					return false;
				}
				if (node != null)
				{
					string trade_status = node.InnerText;
					if (trade_status == "TRADE_FINISHED" || trade_status == "TRADE_SUCCESS")
					{
						if (order.Status != OrderStatus.已确认)
						{
                            Confirm(order, GetType(), order.Amount);
						}
						message = "成功";
						return true;
					}
				}
				
			}
			catch(Exception ero)
			{
				message = "出现错误:" + ero.Message;
				return false;
			}
			message = "失败";
			return false;
		}
        public override bool RefundOrder(PayHistory order, out string message)
        {
            message = "";
            if (string.IsNullOrEmpty(order.SpBillno))
            {
                throw new Exception("支付宝订单号为空,请检查sp_billno");
            }
            ////////////////////////////////////////////请求参数////////////////////////////////////////////

            //服务器异步通知页面路径
            string notify_url = refundNotifyUrl;
            //需http://格式的完整路径，不允许加?id=123这类自定义参数

            //退款批次号
            string batch_no = DateTime.Now.ToString("yyyyMMdd") + order.OrderId.Substring(6, order.OrderId.Length - 6) + "C";
            //必填，每进行一次即时到账批量退款，都需要提供一个批次号，必须保证唯一性

            //退款请求时间
            string refund_date = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            //必填，格式为：yyyy-MM-dd hh:mm:ss

            //退款总笔数
            string batch_num = "1";
            //必填，即参数detail_data的值中，“#”字符出现的数量加1，最大支持1000笔（即“#”字符出现的最大数量999个）

            //单笔数据集
            string detail_data = order.SpBillno + "^" + order.Amount + "^协商退款";
            //必填，格式详见“4.3 单笔数据集参数说明”
            //Log(detail_data);

            ////////////////////////////////////////////////////////////////////////////////////////////////

            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", partner);
            sParaTemp.Add("_input_charset", _input_charset);
            sParaTemp.Add("service", "refund_fastpay_by_platform_nopwd");
            sParaTemp.Add("notify_url", notify_url);
            sParaTemp.Add("batch_no", batch_no);
            sParaTemp.Add("refund_date", refund_date);
            sParaTemp.Add("batch_num", batch_num);
            sParaTemp.Add("detail_data", detail_data);

            //建立请求
            string sHtmlText = AlipaySubmit.BuildRequest(sParaTemp);

            //请在这里加上商户的业务逻辑程序代码

            //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(sHtmlText);
            Log(sHtmlText);
            string status = xmlDoc.SelectSingleNode("/alipay/is_success").InnerText;
            if (status == "F")
            {
                message = "退款失败:" + xmlDoc.SelectSingleNode("/alipay/error").InnerText;
                return false;
            }
            else
            {
                if (status == "T")
                {
                    message = "退款成功";
                    BaseRefundOrder(order);
                    return true;
                }
                else if (status == "P")
                {
                    message = "退款处理中";
                    //退款通知会用及时到帐接口发过来
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 格式：流水号1^收款方帐号1^收款帐号1真实姓名^付款金额1^备注说明1
        /// </summary>
        public class BatchPayItem
        {
            public string OrderNo;
            public string ReceiveAccount;
            public string ReceiveName;
            public double Amount;
            public string Remark;
            public override string ToString()
            {
                return string.Format("{0}^{1}^{2}^{3}^{4}", OrderNo, ReceiveAccount, ReceiveName, Amount, Remark);
            }
            public static List<BatchPayItem> FromDetail(string detail)
            {
                List<BatchPayItem> items = new List<BatchPayItem>();
                string[] arry = detail.Split('|');
                foreach(string s in arry)
                {
                    BatchPayItem item = new BatchPayItem();
                    string[] arry1 = s.Split('^');
                    item.OrderNo = arry1[0];
                    item.ReceiveAccount = arry1[1];
                    item.ReceiveName = arry1[2];
                    item.Amount = Convert.ToDouble(arry1[3]);
                    item.Remark = arry1[5];
                    items.Add(item);
                }
                return items;
            }
        }
        /// <summary>
        /// 构造支付宝批量付款到支付宝账户有密接口
        /// </summary>
        /// <param name="payDetail"></param>
        /// <param name="batch_no">保证其唯一性，格式：当天日期[8位]+序列号[3至16位]，如：201101010000001</param>
        public void BatchTransfers(List<BatchPayItem> payDetail, string batch_no)
        {
            string pay_date = DateTime.Now.ToString("yyyyMMdd");
            //获取当天日期，格式：年[4位]月[2位]日[2位]，如：20110101

            //保证其唯一性，格式：当天日期[8位]+序列号[3至16位]，如：201101010000001

             //格式：流水号1^收款方帐号1^收款帐号1真实姓名^付款金额1^备注说明1|流水号2^收款方帐号2^收款帐号2真实姓名^付款金额2^备注说明2....
            double total = 0;
            //付款详细数据
            string detail_data = "";
            foreach (BatchPayItem s in payDetail)
            {
                total += s.Amount;
                detail_data += s.ToString() + "|";
            }
            detail_data = detail_data.Substring(0, detail_data.Length - 1);
            //付款总笔数
            string batch_num = payDetail.Count.ToString();
            //批量付款笔数（最少1笔，最多1000笔）。

            //付款总金额
            string batch_fee = total.ToString();

            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();

            sParaTemp.Add("pay_date", pay_date);
            sParaTemp.Add("batch_no", batch_no);
            sParaTemp.Add("batch_num", batch_num);
            sParaTemp.Add("batch_fee", batch_fee);
            sParaTemp.Add("detail_data", detail_data);

            //增加基本配置
            sParaTemp.Add("service", "batch_trans_notify");
            sParaTemp.Add("partner",partner);
            sParaTemp.Add("_input_charset", _input_charset);
            sParaTemp.Add("email", seller_email);
            sParaTemp.Add("notify_url",notify_url);
            sParaTemp.Add("account_name", _account_name);

            //确认按钮显示文字
            string strButtonValue = "确认";

            //表单提交HTML数据
            string strHtml = "";

            //构造表单提交XML数据
            strHtml = AlipaySubmit.BuildRequest(sParaTemp, "post", strButtonValue);
            HttpContext.Current.Response.Write(strHtml);
            HttpContext.Current.Response.End();
        }
	}
}
