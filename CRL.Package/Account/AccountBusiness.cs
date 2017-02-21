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
using CoreHelper;
using System.Data;
using System.Collections;
namespace CRL.Package.Account
{

    public class AccountBusiness<TType> : AccountBusiness where TType : class
    {

        public static AccountBusiness<TType> Instance
        {
            get { return new AccountBusiness<TType>(); }
        }
    }
    /// <summary>
    /// 帐号维护,区分不同的帐号类型和流水类型
    /// </summary>
    public class AccountBusiness : BaseProvider<AccountDetail>
    {
        /// <summary>
        /// 创建帐户
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool CreateAccount(AccountDetail info)
        {
            Add(info);
            return true;
        }
        /// <summary>
        /// 获取交易账户传入枚举
        /// </summary>
        /// <param name="account"></param>
        /// <param name="accountType"></param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        public AccountDetail GetAccount(int account, Enum accountType, Enum transactionType)
        {
            return GetAccount(account, accountType.ToInt(), transactionType.ToInt());
        }
        public AccountDetail GetAccount(string account, int accountType, int transactionType)
        {
            return GetAccount(account.ToInt(), accountType, transactionType);
        }
        /// <summary>
        /// 取得帐户信息,没有则创建(实时)
        /// </summary>
        /// <param name="account"></param>
        /// <param name="accountType"></param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        public AccountDetail GetAccount(int account, int accountType, int transactionType)
        {
            var info = QueryItem(b => b.Account == account && b.TransactionType == transactionType && b.AccountType == accountType);
            if (info == null)
            {
                info = new AccountDetail();
                info.Account = account;
                info.AccountType = accountType;
                info.TransactionType = transactionType;
                CreateAccount(info);
            }
            return info;
        }
        static Dictionary<int, AccountDetail> detailInfoCache = new Dictionary<int, AccountDetail>();
        /// <summary>
        /// 获取帐户详细信息,按帐户ID
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public AccountDetail GetAccountFromCache(int accountId)
        {
            if (detailInfoCache.ContainsKey(accountId))
            {
                return detailInfoCache[accountId];
            }
            AccountDetail info = QueryItem(b => b.Id == accountId);
            if (info == null)
                return null;

            lock (lockObj)
            {
                if (!detailInfoCache.ContainsKey(accountId))
                {
                    detailInfoCache.Add(accountId, info);
                }
            }
            return info;
        }

        /// <summary>
        /// 根据帐户ID取得对应的帐号
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public int GetAccountNoFromCache(int accountId)
        {
            var info = GetAccountFromCache(accountId);
            if (info == null)
                return 0;
            return info.Account;
        }
        /// <summary>
        /// 获账户ID
        /// </summary>
        /// <param name="account"></param>
        /// <param name="accountType"></param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        public int GetAccountId(int account, Enum accountType, Enum transactionType)
        {
            return GetAccountId(account, accountType.ToInt(), transactionType.ToInt());
        }
        public int GetAccountId(string account, int accountType, int transactionType)
        {
            return GetAccountId(account, accountType, transactionType);
        }
        /// <summary>
        /// 取得帐户ID(从缓存)
        /// </summary>
        /// <param name="account"></param>
        /// <param name="accountType">帐号类型,用以区分不同渠道用户</param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        public int GetAccountId(int account,int accountType, int transactionType)
        {
            int id = 0;
            foreach (var item in detailInfoCache.Values)
            {
                if (item.Account == account && item.AccountType == accountType&& item.TransactionType== transactionType)
                {
                    return item.Id;
                }
            }
            AccountDetail detail = GetAccount(account, accountType, transactionType);
            lock (lockObj)
            {
                if (!detailInfoCache.ContainsKey(detail.Id))
                {

                    detailInfoCache.Add(detail.Id, detail);
                }
            }
            return detail.Id;
        }
    }
}
