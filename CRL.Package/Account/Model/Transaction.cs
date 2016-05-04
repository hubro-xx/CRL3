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
namespace CRL.Package.Account
{
    /// <summary>
    /// 操作流水
    /// 不要继承
    /// </summary>
    [Attribute.Table(TableName = "AccountTransaction")]
    public class Transaction : IModelBase
    {
        public override string CheckData()
        {
            if (Amount == 0)
            {
                return "金额不能为0";
            }
            if (string.IsNullOrEmpty(Remark))
            {
                return "备注必须填写";
            }
            if (TransactionType<=0)
            {
                //return "TransactionType为0";
            }
            if (string.IsNullOrEmpty(OutOrderId))
            {
                //return "外部订单号必须填写";
            }
            return base.CheckData();
        }
        /// <summary>
        /// 流水类型
        /// 传入作查询用
        /// 提交时会按帐户自动赋值
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public int TransactionType
        {
            get;
            set;
        }
        /// <summary>
        /// 账户ID(内部帐号账户)
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public int AccountId
        {
            get;
            set;
        }
        /// <summary>
        /// 流水号
        /// 如果为空则自动生成
        /// </summary>
        //[Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集唯一)]
        public string TransactionNo
        {
            get;
            set;
        }
        /// <summary>
        /// 金额正负
        /// 根据OperateType自动处理
        /// </summary>
        public decimal Amount
        {
            get;
            set;
        }
        /// <summary>
        /// 上次余额
        /// </summary>
        public decimal LastBalance
        {
            get;
            set;
        }
        /// <summary>
        /// 当前余额
        /// </summary>
        public decimal CurrentBalance
        {
            get;
            set;
        }
        /// <summary>
        /// 操作类型,收入还是支出
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public OperateType OperateType
        {
            get;
            set;
        }
        /// <summary>
        /// 交易类型
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public int TradeType
        {
            get;
            set;
        }
        /// <summary>
        /// 此条交易名称
        /// </summary>
        [Attribute.Field(Length = 100)]
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 备注
        /// </summary>
        [Attribute.Field(Length = 500)]
        public string Remark
        {
            get;
            set;
        }
        /// <summary>
        /// 外部订单号
        /// </summary>
        [Attribute.Field(Length = 50)]
        public string OutOrderId
        {
            get;
            set;
        }
        private string hash;
        /// <summary>
        /// 根据订单数据计算的唯一值
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public string Hash
        {
            get
            {
                string str = string.Format("{0}_{1}_{2}_{3}_{4}", AccountId, Amount, OperateType, TradeType, OutOrderId);
                hash = CoreHelper.StringHelper.EncryptMD5(str).Substring(10, 16);
                return hash;
            }
            set
            {
                hash = value;
            }
        }
        /// <summary>
        /// 生成反转流水
        /// 生成指定帐户和交易类型的反转流水
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="tradeType"></param>
        /// <returns></returns>
        public Transaction GetReversal(int accountId, int tradeType)
        {
            Transaction item = (Transaction)this.MemberwiseClone();
            item.AccountId = accountId;
            item.TradeType = tradeType;
            item.OperateType = item.OperateType == Account.OperateType.收入 ? Account.OperateType.支出 : Account.OperateType.收入;
            item.Amount = Math.Abs(item.Amount);
            if (item.OperateType == OperateType.支出)
            {
                item.Amount = 0 - item.Amount;
            }
            return item;
        }
        public override string ToString()
        {
            return string.Format("{0} {1}",Amount,OperateType);
        }
        /// <summary>
        /// 是否检测余额
        /// </summary>
        public bool CheckBalance = true;
    }
}
