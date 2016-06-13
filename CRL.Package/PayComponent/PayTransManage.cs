/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL;
namespace CRL.Package.PayComponent
{
    /// <summary>
    /// 同一订单多账户付款和退款
    /// </summary>
    public class PayTransManage : BaseProvider<PayTrans>
    {
        public static PayTransManage Instance
        {
            get { return new PayTransManage(); }
        }
   
        /// <summary>
        /// 提交付款
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderType"></param>
        /// <param name="trans"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Submit(string orderId, OrderType orderType,List<PayTrans> trans, out string error)
        {
            var item = trans[0];
            error = "";
            Delete(b => b.OutOrderId == item.OutOrderId && b.OrderType == item.OrderType);
            foreach (var b in trans)
            {
                b.OutOrderId = orderId;
                b.OrderType = orderType;
                b.Status = Status.已提交;
                var accountInstance = new Account.AccountBusiness<PayTransManage>();
                if (b.OperateType == Account.OperateType.支出)
                {
                    var account = accountInstance.GetAccount(b.UserId, b.AccountType, b.TransactionType);
                    if (account.AvailableBalance < b.Amount)
                    {
                        error = string.Format("账户余额不足{0} {1}", b.TransactionType, account.AvailableBalance);
                        return false;
                    }
                }
            }
            BatchInsert(trans);
            return true;
        }
        public List<Account.Transaction> GetConfirmTrans(List<PayTrans> payTrans)
        {
            List<Account.Transaction> trans = new List<Account.Transaction>();
            var accountInstance = new Account.AccountBusiness<PayTransManage>();
            foreach (var item in payTrans)
            {
                if (!item.MakeTrans)
                {
                    continue;
                }
                var account = accountInstance.GetAccountId(item.UserId, item.AccountType, item.TransactionType);
                Account.Transaction ts = new Account.Transaction() { AccountId = account, Amount = item.Amount, OperateType = item.OperateType, TradeType = item.TradeType, OutOrderId = item.OutOrderId, Remark = item.Remark,TransactionType = (int)item.TransactionType};
                trans.Add(ts);
            }
            return trans;
        }
        /// <summary>
        /// 确认支付
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderType"></param>
        /// <param name="trans"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Confirm(string orderId, OrderType orderType,List<Account.Transaction> trans, out string error)
        {
            error = "";
            var instance = new Account.TransactionBusiness<PayTransManage>();
            var a = instance.SubmitTransaction(out error, false, trans.ToArray());
            if (!a)
            {
                return false;
            }
            ParameCollection c = new ParameCollection();
            c["Status"] = Status.已确认;
            Update(b => b.OutOrderId == orderId && b.OrderType == orderType && b.Status == Status.已提交, c);
            return true;
        }
        /// <summary>
        /// 取消/退款
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderType"></param>
        /// <param name="remark"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Cancel(string orderId, OrderType orderType, string remark, out string error)
        {
            var list = QueryList(b => b.OutOrderId == orderId && b.Status == Status.已确认 && b.OrderType == orderType);
            var instance = new Account.TransactionBusiness<PayTransManage>();
            var accountInstance = new Account.AccountBusiness<PayTransManage>();
            List<Account.Transaction> trans = new List<Account.Transaction>();
            foreach (var item in list)
            {
                if (!item.MakeTrans)
                {
                    continue;
                }
                var account = accountInstance.GetAccountId(item.UserId, item.AccountType, item.TransactionType);
                var operateType = item.OperateType == Account.OperateType.收入 ? Account.OperateType.支出 : Account.OperateType.收入;
                Account.Transaction ts = new Account.Transaction() { AccountId = account, Amount = item.Amount, OperateType = operateType, TradeType = item.TradeTypeCancel, OutOrderId = item.OutOrderId, Remark = remark };
                trans.Add(ts);
            }
            bool a = instance.SubmitTransaction(out error,false, trans.ToArray());//提交流水
            if (!a)
            {
                error = "退款时发生错误:" + error;
                return false;
            }
            ParameCollection c = new ParameCollection();
            c["Status"] = Status.已退款;
            Update(b => b.OutOrderId == orderId && b.OrderType == orderType && b.Status == Status.已提交, c);
            return true;
        }
    }
}
