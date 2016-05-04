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

namespace Shopping.Model.Order
{
    /// <summary>
    /// 订单明细
    /// </summary>
    public class OrderDetail : CRL.IModelBase
    {     
        /// <summary>
        /// 订单号
        /// </summary>
        [CRL.Attribute.Field(FieldIndexType = CRL.Attribute.FieldIndexType.非聚集)]
        public string OrderId
        {
            get;
            set;
        }
        
        /// <summary>
        /// 结算价
        /// </summary>
        public decimal SettlementPrice
        {
            get;
            set;
        }
        /// <summary>
        /// 产品编号
        /// </summary>
        public int ProductId
        {
            get;
            set;
        }
        /// <summary>
        /// 用户编号
        /// </summary>
        public int UserId
        {
            get;
            set;
        }
        

        /// <summary>
        /// 供货商ID
        /// </summary>
        public int SupplierId
        {
            get;
            set;
        }


        /// <summary>
        /// 产品名称
        /// </summary>
        [CRL.Attribute.Field(Length = 50)]
        public string ProductName
        {
            get;
            set;
        }
        /// <summary>
        /// 数量
        /// </summary>
        public int Num
        {
            get;
            set;
        }
        /// <summary>
        /// <summary>
        /// 购买价格
        /// </summary>
        /// </summary>
        public decimal Price
        {
            get;
            set;
        }
        [CRL.Attribute.Field(Length = 1000)]
        public string Remark
        {
            get;
            set;
        }
       
    }
}
