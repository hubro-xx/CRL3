using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.Product
{
    /// <summary>
    /// 产品基本类型
    /// </summary>
    public class ProductBase : IModelBase
    {
        /// <summary>
        /// 商家ID
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public int SupplierId
        {
            get;
            set;
        }
        /// <summary>
        /// 产品名称
        /// </summary>
        [CRL.Attribute.Field(Length = 100)]
        public string ProductName
        {
            get;
            set;
        }
        /// <summary>
        /// 卖价
        /// </summary>
        public decimal SoldPrice
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
        /// 分类编号
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public string CategoryCode
        {
            get;
            set;
        }
        /// <summary>
        /// 产品图片
        /// </summary>
        [Attribute.Field(Length = 800)]
        public string ProductImage
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
        /// 销量
        /// </summary>
        public int SoldCount
        {
            get;
            set;
        }
        /// <summary>
        /// 状态
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public ProductStatus ProductStatus
        {
            get;
            set;
        }
        public int Hit
        {
            get;
            set;
        }
    }
}
