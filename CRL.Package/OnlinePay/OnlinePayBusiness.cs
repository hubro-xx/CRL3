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
using System.Collections;
namespace CRL.Package.OnlinePay
{
    //public delegate void OrderDealHandler(PayHistory order);

    /// <summary>
    /// 在线支付交易
    /// 始终使用默认数据连接
    /// </summary>
    public class OnlinePayBusiness : BaseProvider<PayHistory>
    {
        public static OnlinePayBusiness Instance
        {
            get { return new OnlinePayBusiness(); }
        }

        /// <summary>
        /// 根据订单ID,公司类型查询订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="companyType"></param>
        /// <returns></returns>
        public PayHistory GetOrder(string orderId, CompanyType companyType)
        {
            PayHistory order = QueryItem(b => b.OrderId == orderId && b.CompanyType == companyType);
            return order;
        }

        /// <summary>
        /// 获取公司类型中文名,增加类型时需更改
        /// </summary>
        /// <param name="companyType"></param>
        /// <returns></returns>
        private string GetCompanyName(CompanyType companyType)
        {
            return companyType.ToString();
        }
        #region 确认订单
        /// <summary>
        /// 确认订单,状态设为成功
        /// </summary>
        /// <param name="order"></param>
        /// <param name="fromType"></param>
        public void ConfirmByType(PayHistory order, Type fromType)
        {
            if (order.Status != OrderStatus.已确认 && order.Status != OrderStatus.已退款)
            {
                object[] parames1 = new object[] { order.Id,order.SpBillno };
                try
                {
                    SetConfirmStatus(parames1);
                }
                catch (Exception ero)
                {
                    CoreHelper.EventLog.Log("更新充值状态时发生错误,orderId:" + order.Id + "\t" + ero, true);
                    CoreHelper.Reflection.DynamicVisitor.AddMechodCacheByHandler(SetConfirmStatus, parames1);
                }
                //CoreHelper.EventLog.Log("确认订单 " + order.OrderId + " From:" + fromType);
                object[] parames = new object[] { order };
                try
                {
                    ConfirmOrderBase(parames);
                }
                catch (Exception ero)
                {
                    CoreHelper.Reflection.DynamicVisitor.AddMechodCacheByHandler(ConfirmOrderBase, parames);
                    CoreHelper.EventLog.Log("在线支付确认订单失败,加入缓存方法,orderId:" + order.Id + "\t" + ero, true);
                    //throw new Exception("确认订单产生错误");
                }
            }
            else
            {
                //CoreHelper.EventLog.WriteLog("在线支付重复确认订单," + order.Id);
            }
        } 
        /// <summary>
        /// 设置订单为以支付状态
        /// </summary>
        /// <param name="args"></param>
        void SetConfirmStatus(params object[] args)
        {
            int id = Convert.ToInt32(args[0]);
            string sp_billno = args[1].ToString();
            ParameCollection c = new ParameCollection();
            c["spBillno"] = sp_billno;
            c["status"] = OrderStatus.已确认;
            Update(b => b.Id == id, c);
        }
        void ConfirmOrderBase(params object[] args)
        {
            PayHistory order = (PayHistory)args[0];
            ConfirmOrderBase(order);
        }
        public void ConfirmOrderBase(PayHistory order)
        {
            SettingConfig.OnlinePayConfirmOrder(order);
        }
        #endregion
        /// <summary>
        /// 设置订单状态为已退款
        /// </summary>
        /// <param name="args"></param>
        void SetRefundStatus(params object[] args)
        {
            int id = Convert.ToInt32(args[0]);
            
            ParameCollection c = new ParameCollection();
            c["status"] = OrderStatus.已退款;
            Update(b => b.Id == id, c);

        }
        /// <summary>
        /// 退款处理
        /// </summary>
        /// <param name="order"></param>
        public void RefundOrder(PayHistory order)
        {
            if (order.Status != OrderStatus.已确认)
            {
                return;
            }
            object[] parames = new object[] { order.Id};
            try
            {
                SetRefundStatus(parames);
            }
            catch (Exception ero)
            {
                CoreHelper.EventLog.Log("更新订单为已退款时发生错误,orderId:" + order.Id + "\t" + ero, true);
                CoreHelper.Reflection.DynamicVisitor.AddMechodCacheByHandler(SetRefundStatus, parames);
            }
            SettingConfig.OnlinePayOrderRefund(order);
        }
    }
}
