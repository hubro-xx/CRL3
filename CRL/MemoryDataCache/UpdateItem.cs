using CoreHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.MemoryDataCache
{
    /// <summary>
    /// 更新的项
    /// </summary>
    class UpdateItem
    {
        public string Key;
        public string TableName;
        public DBHelper DBHelper;
        public Dictionary<string, object> Params;
        public DateTime UpdateTime;
        public Type Type;
    }
}
