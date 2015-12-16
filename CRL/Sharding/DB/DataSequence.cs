using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Sharding.DB
{
    /// <summary>
    /// 自增编号
    /// 主数据自增编号(存在于索引库)
    /// </summary>
    public class DataSequence : CRL.IModelBase
    {
        /// <summary>
        /// 源表名
        /// </summary>
        public string TableName
        {
            get;
            set;
        }
        /// <summary>
        /// 自增编号
        /// </summary>
        public int Sequence
        {
            get;
            set;
        }
    }
}
