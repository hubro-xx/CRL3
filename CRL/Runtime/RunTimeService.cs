using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Runtime
{
    public class RunTimeService
    {
        static System.Timers.Timer timer;
        static object lockObj = new object();

        public static void BeginLog(string url)
        {
            CallContext.SetData(Base.ContextUrlName, url);
        }
        public static void Log(string url, long el)
        {
            var instance = RunTimeCache.Instance;
            if (timer == null)
            {
                timer = new System.Timers.Timer(10000);
                timer.Elapsed += (s, e) =>
                {
                
                    if (instance.SaveTime.Day != DateTime.Now.Day)//每日清理一次
                    {
                        instance.RunTimeCacheList.Clear();
                        instance.SaveTime = DateTime.Now;
                    }
                    instance.Save();
                };
                timer.Start();
            }
            RunTime runTime;
            var a = instance.RunTimeCacheList.TryGetValue(url, out runTime);
            if (runTime == null)
            {
                runTime = new RunTime() { path = url };
                instance.RunTimeCacheList.TryAdd(url, runTime);
            }
            else
            {
                if (runTime.Max < el)
                {
                    var k2 = CRL.Base.GetCallDBContext();
                    runTime.DBCall = k2;
                    bool useContext;
                    runTime.AllCall = CRL.Base.GetSQLRunningtime(out useContext).Select(b => b.Value).ToList();
                }
                runTime.record.Add(el);
            }
            lock (lockObj)
            {
                runTime.TotalVisitor += 1;
            }
            if (runTime.record.Count > 50)
            {
                runTime.record.RemoveAt(0);
            }
        }
        public static string Display()
        {
            string str = "<table border=1><tr><td>路径</td><td>总访问</td><td>平均</td><td>最大</td><td>最小</td><td>次数</td><td>最近50次</td><td>DBCall</td><td>AllCall</td></tr>";
            var cache = RunTimeCache.Instance.RunTimeCacheList;
            foreach (var kv in cache)
            {
                var obj = kv.Value;
                var max = obj.Max;
                var min = obj.Min;
                var all = string.Join(",", obj.record);
                var call = string.Join(",", obj.DBCall);
                var str2 = "<table border=1>";
                var sqlCount = kv.Value.AllCall.Count;
                for (int i = 0; i < sqlCount; i++)
                {
                    var t = kv.Value.AllCall[i];
                    str2 += string.Format("<tr><td>{2}[{0},{3}R]</td><td>{1}</td></tr>", t.Time, t.SQL, (i + 1) + "/" + sqlCount, t.RowCount);
                }
                str2 += "</table>";
                str += string.Format("<tr><td>{0}</td><td>{8}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td></tr>", kv.Key, obj.avg, max, min, obj.times, all, call, str2, obj.TotalVisitor);
            }
            str += "</table>";
            return str;
        }
    }
}
