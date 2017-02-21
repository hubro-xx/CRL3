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

namespace CRL.Package.Account
{
    public class TransactionBusiness<TType> : TransactionBusiness where TType : class
    {
        public static TransactionBusiness<TType> Instance
        {
            get { return new TransactionBusiness<TType>(); }
        }
    }
    /// <summary>
    /// 帐户交易
    /// 锁定=>提交流水=>解锁=>确认流水
    /// 此逻辑会生成详细的帐户变动情况
    /// </summary>
    public class TransactionBusiness: BaseProvider<Transaction>
    {

        #region 内部属性
        long serialNumber = 0;
        #endregion
        /// <summary>
        /// 根据交易类型生成当前时间唯一流水号
        /// </summary>
        /// <returns></returns>
        public string GetSerialNumber(int transactionType, object tradeType, int operateType)
        {
            //I/X(收入/支出)01(流水类型)0021(交易类型)
            string pat = (operateType == 1 ? "I" : "X") + ((transactionType + "").PadLeft(2, '0'));
            pat += (tradeType + "").PadLeft(4, '0');
            string no;
            lock (lockObj)
            {
                serialNumber += 1;
                if (serialNumber > 10000)
                    serialNumber = 1;
                no = DateTime.Now.ToString("yyMMddhhmmssff") + serialNumber.ToString().PadLeft(5, '0');
            }
            return pat + no;
        }


        #region 提交
        /// <summary>
        /// 判断流水是否提交过
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckTransactionSubmited(Transaction item)
        {
            return Count(b => b.Hash == item.Hash, true) > 0;
        }

        /// <summary>
        /// 提交流水,并指定是否带上事务
        /// </summary>
        /// <param name="error"></param>
        /// <param name="useTrans">是否使用事务,默认为true</param>
        /// <param name="items">多个流水,请根据实际情况处理</param>
        /// <returns></returns>
        public bool SubmitTransaction(out string error, bool useTrans = true, params Transaction[] items)
        {
            error = "";
            if (items.Length == 0)
            {
                error = "流水为空 items";
                return false;
            }
            #region 数据检测
            foreach (var item in items)
            {
                var str = item.CheckData();
                if(!string.IsNullOrEmpty(str))
                {
                    error = str;
                    return false;
                }
                //if (CheckTransactionSubmited(helper, item))
                //{
                //    error = "该流水已经提交过" + item.ToString();
                //    return false;
                //}
            }
            #endregion
            if (useTrans)
            {
                return PackageTrans((out string ex) =>
                {
                    ex = "";
                    //事务内部方法
                    foreach (var item in items)
                    {
                        bool a = SubmitTransaction(item, out ex);
                        if (!a)
                        {
                            return false;
                        }
                    }
                    return true;
                }, out error);
            }
            else
            {
                foreach (var item in items)
                {
                    bool a = SubmitTransaction(item, out error);
                    if (!a)
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// 提交流水不带事务
        /// </summary>
        /// <param name="item"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        bool SubmitTransaction(Transaction item, out string error)
        {
            error = "";
            var helper = DBExtend;
            var account = AccountBusiness<TransactionBusiness>.Instance.QueryItem(item.AccountId);
            item.TransactionType = account.TransactionType;
            item.Amount = Math.Abs(item.Amount);
            if (item.OperateType == OperateType.支出)
            {
                item.Amount = 0 - item.Amount;
            }
            if (string.IsNullOrEmpty(item.TransactionNo))
            {
                item.TransactionNo = GetSerialNumber(1, item.TradeType, (int)item.OperateType);
            }
            //检测余额
            if (item.OperateType == OperateType.支出 && item.CheckBalance)
            {
                var balance = account.AvailableBalance;
                if (balance + item.Amount < 0)
                {
                    error = "对应帐户余额不足";
                    return false;
                }
            }
            item.CurrentBalance = account.CurrentBalance + item.Amount;
            item.LastBalance = account.CurrentBalance;
            helper.InsertFromObj(item);
            //当前余额
            account.Cumulation(b => b.CurrentBalance, item.Amount);
            helper.Update(account);
            return true;
        }
        /// <summary>
        /// 解锁锁定金额并确认流水
        /// </summary>
        /// <param name="item"></param>
        /// <param name="lockId"></param>
        /// <param name="error"></param>
        /// <param name="useTrans">是否使用事务,默认为true</param>
        /// <returns></returns>
        public bool SubmitTransactionAndUnlock(Transaction item, int lockId, out string error, bool useTrans = true)
        {
            error = "";
            if (lockId <= 0)
            {
                throw new Exception("lockId值不符合");
            }
            if (useTrans)
            {
                return PackageTrans((out string ex) =>
                {
                    ex = "";
                    bool a = UnlockAmount(lockId, out ex);
                    if (!a)
                    {
                        return false;
                    }
                    bool b = SubmitTransaction(item, out ex);
                    return b;
                }, out error);
            }
            else
            {
                bool a = UnlockAmount(lockId, out error);
                if (!a)
                {
                    return false;
                }
                bool b = SubmitTransaction(item, out error);
                return b;
            }
        }

        #endregion

        #region 锁定

        /// <summary>
        /// 锁定金额
        /// </summary>
        /// <param name="record"></param>
        /// <param name="error"></param>
        /// <param name="useTrans">是否使用事务,默认为true</param>
        /// <returns></returns>
        public bool LockAmount(LockRecord record, out string error, bool useTrans = true)
        {
            error = "";
            if (useTrans)
            {
                return PackageTrans((out string ex) =>
                {
                    return LockAmountNoTrans(record, out ex);
                }, out error);
            }
            else
            {
                return LockAmountNoTrans(record, out error); 
            }
        }
        /// <summary>
        /// 锁定,不带事务
        /// </summary>
        /// <param name="record"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        bool LockAmountNoTrans(LockRecord record, out string error)
        {
            error = "";
            var helper = DBExtend;
            if (record.Amount <= 0)
            {
                error = "amount格式不正确";
                return false;
            }
            string key = string.Format("LockAmount_{0}_{1}_{2}_{3}", record.AccountId, 0, record.Remark, 0);
            if (!CoreHelper.ConcurrentControl.Check(key, 3))
            {
                error = "同时提交了多次相同的参数" + key;
                return false;
                //throw new Exception("同时提交了多次相同的参数" + key);
            }
            var account = AccountBusiness<TransactionBusiness>.Instance.QueryItem(record.AccountId);
            if (account.AvailableBalance < record.Amount)
            {
                CoreHelper.ConcurrentControl.Remove(key);
                error = "余额不足";
                return false;
            }
            helper.InsertFromObj(record);
            var accountDetail = new AccountDetail() { Id = record.AccountId };
            accountDetail.Cumulation(b => b.LockedAmount, Math.Abs(record.Amount));
            helper.Update(accountDetail);
            return true;
        }
        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="lockId"></param>
        /// <param name="useTrans">是否使用事务,默认为true</param>
        /// <returns></returns>
        public bool UnlockAmount(int lockId, out string error, bool useTrans = true)
        {
            if (useTrans)
            {
                return PackageTrans((out string ex) =>
                {
                    return UnlockAmountNoTrans(lockId, out ex);
                }, out error);
            }
            else
            {
                return UnlockAmountNoTrans(lockId, out error);
            }
        }
        /// <summary>
        /// 解锁金额,没有事务
        /// </summary>
        bool UnlockAmountNoTrans(int lockedId, out string error)
        {
            var helper = DBExtend;
            var lockRecord = helper.QueryItem<LockRecord>(b => b.Id == lockedId);
            error = "";
            if (lockRecord == null)
            {
                error = "找不到锁ID:" + lockedId;
                return false;
            }
            if (lockRecord.Checked)
            {
                error = "该锁已经解过ID:" + lockedId;
                return false;
            }
            string key = string.Format("UnlockAmount_{0}", lockedId);
            if (!CoreHelper.ConcurrentControl.Check(key))
            {
                error = "同时提交了多次相同的参数" + key;
                return false;
            }
            var accountDetail = new AccountDetail() { Id = lockRecord.AccountId };
            accountDetail.Cumulation(b => b.LockedAmount, -Math.Abs(lockRecord.Amount));
            helper.Update(accountDetail);
            int count = helper.Delete<LockRecord>(b => b.Id == lockedId);
            if (count == 0)
            {
                CoreHelper.ConcurrentControl.Remove(key);
            }
            return true;
        }
        #endregion

    }
}
