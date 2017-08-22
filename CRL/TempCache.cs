using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL
{
    public class TempCache
    {
        static ConcurrentDictionary<string, Dictionary<int, SqlInfo>> allSqlCache = new ConcurrentDictionary<string, Dictionary<int, SqlInfo>>();

        [Serializable]
        public class SqlInfo
        {
            /// <summary>
            /// sql
            /// </summary>
            public string SQL
            {
                get; set;
            }
            /// <summary>
            /// ms
            /// </summary>
            public long Time
            {
                get; set;
            }
        }
    }
}
