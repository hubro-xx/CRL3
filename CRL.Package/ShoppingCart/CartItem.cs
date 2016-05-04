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

namespace CRL.Package.ShoppingCart
{
    /// <summary>
    /// 购物车项,不要指定别名
    /// </summary>
    [Attribute.Table(TableName = "Cart")]
    public class CartItem : IModelBase
    {
        public override string CheckData()
        {
            return "";
        }
        /// <summary>
        /// 用户ID
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public int UserId
        {
            get;
            set;
        }
        /// <summary>
        /// 自定义产品来源
        /// </summary>
        public int Source
        {
            get;
            set;
        }
        /// <summary>
        /// 商家ID
        /// </summary>
        public int SupplierId
        {
            get;
            set;
        }
        /// <summary>
        /// 产品ID
        /// </summary>
        public int ProductId
        {
            get;
            set;
        }
        /// <summary>
        /// 产品名
        /// </summary>
        public string ProductName
        {
            get;
            set;
        }
        /// <summary>
        /// 自定义产品类型
        /// </summary>
        public int ProductType
        {
            get;
            set;
        }
        /// <summary>
        /// 样式ID
        /// </summary>
        public int StyleId
        {
            get;
            set;
        }
        public string StyleName
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
        /// 购物车类型
        /// </summary>
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public int CartType
        {
            get;
            set;
        }
        /// <summary>
        /// 购买价格
        /// </summary>
        public decimal Price
        {
            get;
            set;
        }
        /// <summary>
        /// 赠送积分
        /// </summary>
        public decimal Integral
        {
            get;
            set;
        }

        /// <summary>
        /// 是否计入商家满就免运费金额
        /// </summary>
        public bool IncludedFreePost
        {
            get;
            set;
        }
        /// <summary>
        /// 总重量,如果不算运费,则为0
        /// </summary>
        public double TotalWeight
        {
            get;
            set;
        }
        /// <summary>
        /// 存入自定义数据
        /// </summary>
        [CRL.Attribute.Field(Length=100)]
        public string TagData
        {
            get;
            set;
        }
        /// <summary>
        /// 分割自定义数据,按|分割
        /// </summary>
        public string[] TagDataArray
        {
            get
            {
                return TagData.Split('|');
            }
        }
        /// <summary>
        /// 推广数据
        /// </summary>
        [CRL.Attribute.Field(Length = 100)]
        public string SpreadInfo
        {
            get;
            set;
        }
        /// <summary>
        /// 是否缺货,二次查询得出
        /// </summary>
        public bool OutStock
        {
            get;
            set;
        }
        bool selected = true;

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
            }
        }
        /// <summary>
        /// 付款类型
        /// </summary>
        public int PayType
        {
            get;
            set;
        }
    }
}
