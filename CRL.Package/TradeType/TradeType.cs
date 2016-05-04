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

namespace CRL.Package.Business.TradeType
{
    /// <summary>
    /// 交易类型
    /// </summary>
    [CRL.Attribute.Table(TableName = "TradeType")]
    public class TradeType : Category.Category
    {
        /// <summary>
        /// 交易方向
        /// </summary>
        public TradeDirection TradeDirection
        {
            get;
            set;
        }
        /// <summary>
        /// 重新计算后的交易类型代码
        /// </summary>
        [CRL.Attribute.Field(FieldIndexType = CRL.Attribute.FieldIndexType.非聚集唯一)]
        public string TradeCode
        {
            get;
            set;
        }
    }
    
}
