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
    /// 分表,主数据表不分表,只按库分
    /// 其它表按主数据段分
    /// </summary>
    public class TablePart : CRL.IModelBase
    { 
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
        /// 源表名
        /// </summary>
        [CRL.Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public string TableName
        {
            get;
            set;
        }
        /// <summary>
        /// 分表名
        /// </summary>
        public string PartName
        {
            get;
            set;
        }
        /// <summary>
        /// 分表索引,从0开始
        /// </summary>
        public int PartIndex
        {
            get;
            set;
        }
        /// <summary>
        /// 主数据开始索引值
        /// </summary>
        public int MainDataStartIndex
        {
            get;
            set;
        }
        /// <summary>
        /// 主数据结束索引值
        /// </summary>
        public int MainDataEndIndex
        {
            get;
            set;
        }
        ///// <summary>
        ///// 分表最大数据量
        ///// </summary>
        //public int MaxPartDataTotal
        //{
        //    get;
        //    set;
        //}
    }
}
