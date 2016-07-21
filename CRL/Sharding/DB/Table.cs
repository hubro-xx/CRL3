/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Sharding.DB
{
    /// <summary>
    /// 表
    /// 主数据表不分表,只按库分,其它表再按主数据段分表
    /// </summary>
    public class Table:CRL.IModelBase
    {
        /// <summary>
        /// 源表名
        /// </summary>
        [CRL.Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public string TableName
        {
            get;
            set;
        }
        /// <summary>
        /// 库名
        /// </summary>
        [CRL.Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public string DataBaseName
        {
            get;
            set;
        }
        /// <summary>
        /// 分表数
        /// </summary>
        public int TablePartTotal
        {
            get;
            set;
        }
        /// <summary>
        /// 分表最大数据量
        /// </summary>
        public int MaxPartDataTotal
        {
            get;
            set;
        }
        /// <summary>
        /// 是否为主数据表
        /// 主数据表在当前库只存在一个
        /// </summary>
        public bool IsMainTable
        {
            get;
            set;
        }

    }
    
}
