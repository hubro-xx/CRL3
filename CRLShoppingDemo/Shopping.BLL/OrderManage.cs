/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using Shopping.Model.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.BLL
{
    /// <summary>
    /// 订单管理
    /// </summary>
    public class OrderManage : CRL.BaseProvider<OrderMain>
    {
        public static OrderManage Instance
        {
            get { return new OrderManage(); }
        }
        /// <summary>
        /// 从购物车生成订单
        /// </summary>
        /// <param name="order"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool SubmitFromCart(OrderMain order, out string error)
        {
            error = "";
            var cart = CartManage.Instance.QueryList(b => b.UserId == order.UserId && b.Selected == true);
            if (cart.Count == 0)
            {
                error = "购物车为空";
                return false;
            }
            List<OrderDetail> detail = new List<OrderDetail>();
            foreach (var c in cart)
            {
                var d = new OrderDetail();
                d.OrderId = order.OrderId;
                d.ProductId = c.ProductId;
                d.ProductName = c.ProductName;
                d.Price = c.Price;
                d.SupplierId = c.SupplierId;
                d.Num = c.Num;
                d.UserId = Convert.ToInt32(c.UserId);
                detail.Add(d);
            }
            bool a = SubmitOrder(order, detail);
            if (a)
            {
                CartManage.Instance.RemoveAll(order.UserId);
            }
            return a;
        }
        /// <summary>
        /// 生成订单
        /// </summary>
        /// <param name="order"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public bool SubmitOrder(OrderMain order, List<OrderDetail> details)
        {
            order.Status = OrderStatus.待付款;
            int total = 0;
            decimal amountTotal = 0;
            var db = DBExtend;
            foreach (var item in details)
            {
                total += item.Num;
                item.OrderId = order.OrderId;
                amountTotal += item.Num * item.Price;
                db.InsertFromObj(item);
            }
            order.TotalAmount = amountTotal;
            order.TotalNum = total;
            db.InsertFromObj(order);
            return true;
        }
        /// <summary>
        /// 扣款
        /// </summary>
        /// <param name="order"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool PayOrder(OrderMain order, out string error)
        {
            error = "";
            //会员账户
            var accountUser = Transaction.AccountManage.Instance.GetAccount(order.UserId, Model.AccountType.会员, Model.TransactionType.现金);
            var accountUser2 = Transaction.AccountManage.Instance.GetAccount(order.UserId, Model.AccountType.会员, Model.TransactionType.积分);
            if (accountUser.AvailableBalance < order.TotalAmount)
            {
                error = "账户余额不足";
                return false;
            }
            
            int tradeType = 1001;
            var amount = order.TotalAmount;
            var orderId = order.OrderId;
            var remark = "订单支付";
            var trans = new List<CRL.Package.Account.Transaction>();
            //生成会员交易流水
            var ts = new CRL.Package.Account.Transaction() { AccountId = accountUser.Id, Amount = amount, OperateType = CRL.Package.Account.OperateType.支出, TradeType = tradeType, OutOrderId = orderId, Remark = remark };
            trans.Add(ts);
            //赠送积分
            var ts3 = new CRL.Package.Account.Transaction() { AccountId = accountUser2.Id, Amount = amount, OperateType = CRL.Package.Account.OperateType.收入, TradeType = tradeType, OutOrderId = orderId, Remark = "赠送积分" };
            trans.Add(ts3);
            bool b = Transaction.TransactionManage.Instance.SubmitTransaction(out error,true, trans.ToArray());//提交流水
            if(!b)
            {
                return false;
            }
            //order是查询创建的,直接更改值即可更新
            order.Status = OrderStatus.下单成功;
            Update(order);
            return b;
        }
    }
}
