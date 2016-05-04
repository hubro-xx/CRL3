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
    /// 主订单
    /// </summary>
    public class OrderMain : CRL.IModelBase
    {
        public OrderMain()
        {
            OrderId = DateTime.Now.ToString("yyMMddhhmmssff");
        }
        /// <summary>
        /// 标题,信息备用
        /// </summary>
        [CRL.Attribute.Field(Length = 100)]
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// 所有者ID
        /// </summary>
        public int UserId
        {
            get;
            set;
        }
        /// <summary>
        /// 订单ID 
        /// </summary>
        [CRL.Attribute.Field(FieldIndexType = CRL.Attribute.FieldIndexType.非聚集唯一)]
        public string OrderId
        {
            get;
            set;
        }
        
        /// <summary>
        /// 备注
        /// </summary>
        [CRL.Attribute.Field(Length = 3000, NotNull = false)]
        public string Remark
        {
            get;
            set;
        }
        /// <summary>
        /// 总价值
        /// </summary>
        public decimal TotalAmount
        {
            get;
            set;
        }
       
        /// <summary>
        /// 货品总数
        /// </summary>
        public int TotalNum { get; set; }
        /// <summary>
        /// 收货人
        /// </summary>
        public string ReceiveName
        {
            get;
            set;
        }

        /// <summary>
        /// 收货地址
        /// </summary>
        [CRL.Attribute.Field(Length = 100)]
        public string ReceiveAddress
        {
            get;
            set;
        }
        /// <summary>
        /// 收货人电话
        /// </summary>
        public string ReceivePhone
        {
            get;
            set;
        }
        /// <summary>
        /// 状态
        /// </summary>
        [CRL.Attribute.Field(FieldIndexType = CRL.Attribute.FieldIndexType.非聚集)]
        public OrderStatus Status
        {
            get;
            set;
        }
    }
}
