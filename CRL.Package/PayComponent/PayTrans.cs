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
    /// 对一个订单,分多个账户付款
    /// </summary>
    public class PayTrans : CRL.IModelBase
    {
        public override string CheckData()
        {
            if (string.IsNullOrEmpty(OutOrderId))
            {
                return "OutOrderId不能为空";
            }
            if (OrderType.ToInt() == 0)
            {
                return "OrderType不能为空";
            }
            return base.CheckData();
        }
        Account.OperateType operateType = Account.OperateType.支出;
        public Account.OperateType OperateType
        {
            get
            {
                return operateType;
            }
            set
            {
                operateType = value;
            }
        }
        public OrderType OrderType
        {
            get;
            set;
        }
        [CRL.Attribute.Field(FieldIndexType = CRL.Attribute.FieldIndexType.非聚集)]
        public string OutOrderId
        {
            get;
            set;
        }
        /// <summary>
        /// 账号类型
        /// </summary>
        public int AccountType
        {
            get;
            set;
        }
        /// <summary>
        /// 用户编号
        /// </summary>
        [CRL.Attribute.Field(FieldIndexType = CRL.Attribute.FieldIndexType.非聚集)]
        public int UserId
        {
            get;
            set;
        }
        /// <summary>
        /// 账户类型
        /// </summary>
        public int TransactionType
        {
            get;
            set;
        }
        /// <summary>
        /// 交易类型
        /// </summary>
        public int TradeType
        {
            get;
            set;
        }
        /// <summary>
        /// 取消交易类型
        /// </summary>
        public int TradeTypeCancel
        {
            get;
            set;
        }
        /// <summary>
        /// 需付款金额
        /// </summary>
        public decimal Amount
        {
            get;
            set;
        }
        /// <summary>
        /// 状态
        /// </summary>
        public Status Status
        {
            get;
            set;
        }
        bool makeTrans = true;
        /// <summary>
        /// 是否生成交易流水
        /// </summary>
        public bool MakeTrans
        {
            get
            {
                return makeTrans;
            }
            set
            {
                makeTrans = value;
            }
        }
        [CRL.Attribute.Field(Length = 100)]
        public string Remark
        {
            get;
            set;
        }
    }
    public enum OrderType
    {
        商品订单=1,
        充值订单=2,
        其它订单=3
    }
    public enum Status
    {
        已提交,
        已确认,
        已取消,
        已退款
    }
}
