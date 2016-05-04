/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using Shopping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.BLL
{
    /// <summary>
    /// 会员管理
    /// </summary>
    public class MemberManage : CRL.Package.Person.PersonBusiness<MemberManage, Member>
    {
        public static MemberManage Instance
        {
            get { return new MemberManage(); }
        }
        /// <summary>
        /// 充值入口
        /// </summary>
        /// <param name="member"></param>
        /// <param name="amount"></param>
        /// <param name="remark"></param>
        /// <param name="transactionType"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Charge(Member member, decimal amount, string remark, TransactionType transactionType, out string error)
        {
            var account = Transaction.AccountManage.Instance.GetAccountId(member.Id, Model.AccountType.会员, transactionType);
            string orderId = DateTime.Now.ToString("yyMMddhhmmssff");
            int tradeType = 10001;
            var trans = new List<CRL.Package.Account.Transaction>();
            var ts = new CRL.Package.Account.Transaction() { AccountId = account, Amount = amount, OperateType = CRL.Package.Account.OperateType.收入, TradeType = tradeType, OutOrderId = orderId, Remark = remark };
            trans.Add(ts);

            bool b = Transaction.TransactionManage.Instance.SubmitTransaction(out error,true, trans.ToArray());//提交流水
            return b;
        }
        
    }
}
