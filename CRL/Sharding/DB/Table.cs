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
