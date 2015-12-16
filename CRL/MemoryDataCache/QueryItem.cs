using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.MemoryDataCache
{
    /// <summary>
    /// 查询的项
    /// </summary>
    public class QueryItem
    {
        public string TableName
        {
            get;
            set;
        }
        public Type DataType
        {
            get;
            set;
        }
        public string Key
        {
            get;
            set;
        }
        public string Params
        {
            get;
            set;
        }
        public int TimeOut
        {
            get;
            set;
        }
        public DateTime UpdateTime
        {
            get;
            set;
        }
        public int RowCount
        {
            get;
            set;
        }
    }
}
