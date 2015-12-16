using CoreHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.MemoryDataCache
{
    /// <summary>
    /// 缓存的查询的项
    /// </summary>
    internal class MemoryDataCacheItem
    {
        /// <summary>
        /// 查询
        /// </summary>
        public string Query;
        /// <summary>
        /// 缓存数据
        /// Dictionary<int, TItem>
        /// </summary>
        public object Data;
        /// <summary>
        /// 要更新的数据
        /// </summary>
        public List<object> UpdatedData;
        public int Count;
        /// <summary>
        /// 对象类型
        /// </summary>
        public Type Type;
        /// <summary>
        /// 超时时间分
        /// </summary>
        public int TimeOut;
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime = DateTime.Now;
        /// <summary>
        /// 使用时间,如果一定时间内没有使用,将不会进行更新或被移除
        /// </summary>
        public DateTime UseTime = DateTime.Now;
        public DBHelper DBHelper;
        public Dictionary<string, object> Params;
        /// <summary>
        /// 查询次数
        /// </summary>
        public int QueryCount = 0;
    }
}
