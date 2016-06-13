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
    /// <summary>
    /// 帐户信息 不可继承
    /// </summary>
    [Attribute.Table(TableName = "AccountInfo")]
    public class AccountDetail : IModelBase
    {
        /// <summary>
        /// 帐号类型
        /// 用以区分不同业务的帐号
        /// 如,商家,会员
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public int AccountType
        {
            get;
            set;
        }
        /// <summary>
        /// 对应的帐号
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public int Account
        {
            get;
            set;
        }
        /// <summary>
        /// 流水种类
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public int TransactionType
        {
            get;
            set;
        }
        /// <summary>
        /// 锁定金额
        /// </summary>
        public decimal LockedAmount
        {
            get;
            set;
        }
        /// <summary>
        /// 当前金额
        /// </summary>
        public decimal CurrentBalance
        {
            get;
            set;
        }
        /// <summary>
        /// 可用余额
        /// </summary>
        public decimal AvailableBalance
        {
            get
            {
                return CurrentBalance - LockedAmount;
            }
        }
    }
}
