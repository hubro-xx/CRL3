using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Runtime
{
    [Serializable]
    public class RunTimeCache : CoreHelper.ICoreConfig<RunTimeCache>
    {
        private System.Collections.Concurrent.ConcurrentDictionary<string, RunTime> runTimeCache = new System.Collections.Concurrent.ConcurrentDictionary<string, RunTime>();

        public ConcurrentDictionary<string, RunTime> RunTimeCacheList
        {
            get
            {
                return runTimeCache;
            }

            set
            {
                runTimeCache = value;
            }
        }
    }
    [Serializable]
    public class RunTime : CoreHelper.ICoreConfig<RunTime>
    {
        public string path;
        public List<long> record = new List<long>();
        public long avg
        {
            get
            {
                if (times == 0)
                    return 0;
                return totalTimes / times;
            }
        }
        public long totalTimes
        {
            get
            {
                return record.Sum();
            }
        }
        public int times
        {
            get
            {
                return record.Count;
            }
        }
        public long Max
        {
            get
            {
                return times == 0 ? 0 : record.Max();
            }
        }
        public long Min
        {
            get
            {
                return times == 0 ? 0 : record.Min();
            }
        }
        public List<CRL.Base.SqlInfo> AllCall = new List<CRL.Base.SqlInfo>();
        public List<string> DBCall = new List<string>();
    }
}
