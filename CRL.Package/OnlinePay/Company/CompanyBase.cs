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
using System.Xml.Linq;
using CoreHelper;
using System.Collections.Generic;
using System.Web;
namespace CRL.Package.OnlinePay.Company
{
	/// <summary>
	/// 支付公司类型抽象类
	/// </summary>
	public  abstract class CompanyBase
	{
        /// <summary>
        /// 商户密钥
        /// </summary>
        public string MerchantKey
        {
            get
            {
                return ChargeConfig.GetConfigKey(ThisCompanyType, ChargeConfig.DataType.Key);
            }
        }
        /// <summary>
        /// 商户编号
        /// </summary>
        public string MerchantId
        {
            get
            {
                return ChargeConfig.GetConfigKey(ThisCompanyType, ChargeConfig.DataType.User);
            }
        }
        /// <summary>
        /// 跳转URL
        /// </summary>
        public string ReturnUrl
        {
            get
            {
                return GetAbsUrl(ChargeConfig.GetConfigKey(ThisCompanyType, ChargeConfig.DataType.ReturnUrl));
            }
        }
        /// <summary>
        /// 证书路径
        /// </summary>
        public string CertFile
        {
            get
            {
                return ChargeConfig.GetConfigKey(ThisCompanyType, ChargeConfig.DataType.CertFile);
            }
        }
        /// <summary>
        /// 证书密码
        /// </summary>
        public string CertFilePass
        {
            get
            {
                return ChargeConfig.GetConfigKey(ThisCompanyType, ChargeConfig.DataType.CertFilePass);
            }
        }
        /// <summary>
        /// 通知URL
        /// </summary>
        public string NotifyUrl
        {
            get
            {
                return GetAbsUrl(ChargeConfig.GetConfigKey(ThisCompanyType, ChargeConfig.DataType.NotifyUrl));
            }
        }

        public static string CurrentHost
        {
            get
            {
                string url = HttpContext.Current.Request.Url.ToString();
                string[] arry = url.Split('/');
                string host = arry[2];
                string url1 = arry[0] + "//" + host;
                //todo 更改主机URL
                
                return url1;
            }
        }
        public static string GetAbsUrl(string url)
        {
            if (url.StartsWith("http"))
            {
                return url;
            }
            return CurrentHost + url;
        }
        /// <summary>
        /// 当前类型
        /// </summary>
        public abstract CompanyType ThisCompanyType { get; }
        static object lockObj = new object();
        /// <summary>
        /// 生成订单号
        /// </summary>
        /// <returns></returns>
        public virtual string CreateOrderId()
        {
            return DateTime.Now.ToString("yyMMddHHmmssffff");
        }
		/// <summary>
        /// 生成订单实例
		/// </summary>
		/// <param name="amount"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
        public PayHistory CreateOrder(decimal amount, int userId)
        {
            if (amount <= 0)
            {
                throw new Exception("支付金额不能小于或等于0");
            }
            PayHistory order = new PayHistory();
            order.Amount = amount;
            order.CompanyType = ThisCompanyType;
            order.Status = OrderStatus.已提交;
            order.UserId = userId;
            lock (lockObj)
            {
                order.OrderId = CreateOrderId();
            }
            return order;    
        }

		/// <summary>
		/// 在这里写日志
		/// </summary>
		/// <param name="context"></param>
		private void NotifyLog(HttpContext context)
		{
            string address = CoreHelper.RequestHelper.GetCdnIP();

            string content = "NotifyLog:" + ThisCompanyType;
			content += " IP:" + address + "\r\n";
			content += " REQUEST: " + context.Request.QueryString.ToString() + "\r\n";
			content += " POST: " + context.Request.Form.ToString();
            Log(content);
		}
		/// <summary>
		/// 接口回调,在这里处理信息
		/// </summary>
		/// <param name="context"></param>
        protected abstract string OnNotify(HttpContext context);
        /// <summary>
        /// 获取通知
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetNotify(HttpContext context)
        {
            NotifyLog(context);
            return OnNotify(context);
        }
		/// <summary>
		/// 提交到数据库+
		/// 对于代理,需查询OrderType为Charge的记录
		/// </summary>
		/// <param name="order"></param>
		protected void BaseSubmit(PayHistory order)
		{
            if (string.IsNullOrEmpty(order.OrderId))
            {
                throw new Exception("没有指定OrderId");
            }
            OnlinePayBusiness.Instance.Add(order);
            string log = string.Format("提交订单 {0} {1} {2} {3}", order.UserId, order.OrderId, order.Amount, order.OrderType);
            Log(log, false);
		}

		/// <summary>
		/// 产生记录并URL转向
		/// </summary>
		/// <param name="order"></param>
		public abstract void Submit(PayHistory order);

		/// <summary>
		/// 确认订单,失败会插入方法缓存,下次会自动重新执行方法
		/// </summary>
		/// <param name="orderNumber"></param>
		/// <param name="companyType"></param>
        //protected internal Order Confirm(string orderNumber, CompanyType companyType, Type fromType)
        //{
        //    Order order = OrderAction.GetOrder(orderNumber, companyType);
        //    if (order != null)
        //    {
        //        OrderAction.Confirm(order, fromType);
        //    }
        //    return order;
        //}

        /// <summary>
        /// 确认订单,并核对通知过来的金额
        /// </summary>
        /// <param name="order"></param>
        /// <param name="fromType"></param>
        /// <param name="notifyAmount">通知的订单金额,如果没有填原始订单的金额</param>
        protected internal void Confirm(PayHistory order, Type fromType,decimal notifyAmount)
        {
            if (order.Amount != notifyAmount)
            {
                string message = order.CompanyType + "订单金额和支付金额对不上:" + order.OrderId + " 订单金额:" + order.Amount + " 通知金额:" + notifyAmount;
                Log(message, true);
                throw new Exception(message);
            }
            string key = "IPayHistory_" + order.OrderId;
            if (!CoreHelper.ConcurrentControl.Check(key, 10))
            {
                Log("过滤重复通知确认:" + order.OrderId + " amount:" + order.Amount);
                return;
            }
            if (order.Status== OrderStatus.已确认 || order.Status == OrderStatus.已退款)
            {
                return;
            }
            OnlinePayBusiness.Instance.ConfirmByType(order, fromType);
            Log("确认订单:" + order.OrderId + " amount:" + order.Amount);
        }
		/// <summary>
		/// 调用接口检查支付是否成功
		/// 根据情况手动Confirm
		/// </summary>
		/// <param name="order"></param>
		public abstract bool CheckOrder(PayHistory order,out string message);
		/// <summary>
		/// 转向页执行此方法
		/// 这里为了实现自定义转向
		/// </summary>
		/// <param name="order"></param>
		public void Redirect(PayHistory order)
		{
			if (!string.IsNullOrEmpty(order.RedirectUrl))
			{
                HttpContext.Current.Response.Redirect(order.RedirectUrl + "?amount=" + order.Amount + "&orderId=" + order.OrderId + "&companyType=" + (int)order.CompanyType + "&ProductOrderId=" + order.ProductOrderId);
			}
		}

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="log"></param>
        public void Log(string log)
        {
            CoreHelper.EventLog.Log(log, ThisCompanyType.ToString(), false);
        }
        /// <summary>
        /// 记录日志并发送到服务器
        /// </summary>
        /// <param name="log"></param>
        /// <param name="sendToServer"></param>
        public void Log(string log, bool sendToServer)
        {
            Log(log);
            if (sendToServer)
            {
                EventLog.SendToServer(log, ThisCompanyType.ToString());
            }
        }
        /// <summary>
        /// 订单退款方法
        /// </summary>
        /// <param name="order"></param>
        protected internal void BaseRefundOrder(PayHistory order)
        {
            ///未确认的订单不能退款
            if (order.Status != OrderStatus.已确认)
            {
                return;
            }
            OnlinePayBusiness.Instance.RefundOrder(order);
            Log("支付订单被退款:" + order.OrderId + " amount:" + order.Amount, true);
        }

        /// <summary>
        /// 退款,取消订单
        /// </summary>
        /// <param name="order"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract bool RefundOrder(PayHistory order, out string message);

        //public abstract bool CheckCancelOrder(Order order, out string message);
	}
}
