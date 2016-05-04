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
using System.Collections.Generic;

namespace CRL.Package.OnlinePay
{
	/// <summary>
	/// 提供充值方法
	/// </summary>
	public class ChargeService
	{
		private static Company.CompanyBase GetCompany(CompanyType companyType)
        {
            #region 实例化
            Company.CompanyBase company = null;
            var dic = new Dictionary<CompanyType, Type>();
            dic.Add(CompanyType.支付宝, typeof(Company.Alipay.AlipayCompany));
            dic.Add(CompanyType.财付通, typeof(Company.Tenpay.TenpayCompany));
            dic.Add(CompanyType.快钱, typeof(OnlinePay.Company.Bill99.Bill99Company));
            dic.Add(CompanyType.连连, typeof(OnlinePay.Company.Lianlian.LianlianCompany));
            dic.Add(CompanyType.汇付天下, typeof(OnlinePay.Company.Chinapnr.ChinapnrCompany));
            dic.Add(CompanyType.微信, typeof(Company.Weixin.WeixinCompany));
            dic.Add(CompanyType.支付宝WAP, typeof(Company.AlipayWap.AlipayWapCompany));
            if (!dic.ContainsKey(companyType))
            {
                throw new Exception("未实现的CompanyType" + companyType);
            }
            company = System.Activator.CreateInstance(dic[companyType]) as Company.CompanyBase;
            return company;
			#endregion
		}
        static object lockObj = new object();
        /// <summary>
        /// 生成订单
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="userId"></param>
        /// <param name="companyType"></param>
        /// <returns></returns>
		public static PayHistory CreateOrder(decimal amount, int userId,CompanyType companyType)
		{
			Company.CompanyBase company = GetCompany(companyType);
            PayHistory order = null;
            lock (lockObj)
            {
                order = company.CreateOrder(amount, userId);
            }
            return order;
		}
		/// <summary>
		/// 提交支付
		/// </summary>
		/// <param name="order"></param>
		public static void Submit(PayHistory order)
        {
            if (order.OrderType == OrderType.支付)
            {
                if (string.IsNullOrEmpty(order.ProductOrderId))
                {
                    throw new Exception("支付类型订单必须传ProductOrderId");
                }
            }

			Company.CompanyBase company = GetCompany(order.CompanyType);
            try
            {
                company.Submit(order);
            }
            catch(Exception ero)
            {
                if (ero is System.Threading.ThreadAbortException)
                {
                    return;
                }
                CoreHelper.EventLog.Log("提交支付订单时出错:" + ero, true);
                throw ero;
            }
		}
        /// <summary>
        /// 通过参数直接提交
        /// 会产生跳转
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="amount"></param>
        /// <param name="bankType"></param>
        /// <param name="orderType"></param>
        /// <param name="companyType"></param>
        /// <param name="byProductOrder"></param>
        /// <param name="redirectUrl"></param>
        public static void Submit(int userId, decimal amount, string bankType, OrderType orderType, CompanyType companyType, string byProductOrder, string redirectUrl)
        {
            //todo 订单需要判断没有有付过款,并存不存在
            if (amount <= 0)
            {
                throw new Exception("找不到订单,或订单金额为0");
            }

            PayHistory order = ChargeService.CreateOrder(amount, userId, companyType);
            order.RedirectUrl = redirectUrl;
            //在这里传入银行代码
            order.BankType = bankType;
            //传入商城订单编号
            order.ProductOrderId = byProductOrder;
            //在这里传入订单类型,默认为充值

            order.OrderType = orderType;
  
            ChargeService.Submit(order);
        }
		/// <summary>
		/// 接口回调
		/// </summary>
		/// <param name="companyType"></param>
		/// <param name="context"></param>
		public static string GetNotify(CompanyType companyType, HttpContext context)
		{
			Company.CompanyBase company = GetCompany(companyType);

            return company.GetNotify(context);
		}

		/// <summary>
		/// 查询订单
		/// 如果订单未确认,会自动确认
		/// </summary>
		/// <param name="order"></param>
		public static bool CheckOrder(PayHistory order,out string message)
		{
            if (order.Status == OrderStatus.已确认 || order.Status == OrderStatus.已退款)
            {
                message = "此订单状态为" + order.Status;
                return false;
            }
			Company.CompanyBase company = GetCompany(order.CompanyType);
			return company.CheckOrder(order,out message);
		}
		/// <summary>
		/// 接口转向页执行此方法
		/// </summary>
		/// <param name="order"></param>
		public static void Redirect(PayHistory order)
		{
			Company.CompanyBase company = GetCompany(order.CompanyType);
			company.Redirect(order);
		}

		
        /// <summary>
        /// 订单取消,退款
        /// 只要提交成功就一定能成功,特殊情况除外
        /// </summary>
        /// <param name="order"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool RefundOrder(PayHistory order, out string message)
        {
            if (order.Status != OrderStatus.已确认)
            {
                message = "此订单状态为" + order.Status;
                return false;
            }
            Company.CompanyBase company = GetCompany(order.CompanyType);
            return company.RefundOrder(order, out message);
        }
        /// <summary>
        /// 提交支付转帐
        /// </summary>
        /// <param name="payDetail"></param>
        /// <param name="batch_no"></param>
        public static void BatchTransfers(List<Company.Alipay.AlipayCompany.BatchPayItem> payDetail, string batch_no)
        {
            new Company.Alipay.AlipayCompany().BatchTransfers(payDetail, batch_no);
        }
	}
}
