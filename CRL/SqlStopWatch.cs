using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL
{
    public class SqlStopWatch
    {
        internal static int Execute(CoreHelper.DBHelper __DbHelper, string sql)
        {
            int n = 0;
            var el = Run(() =>
            {
                n = __DbHelper.Execute(sql);
            });
            Base.SaveSQLRunningtme(sql, el);
            return n;
        }
        internal static object ExecScalar(CoreHelper.DBHelper __DbHelper, string sql)
        {
            object obj = null;
            var el = Run(() =>
            {
                obj = __DbHelper.ExecScalar(sql);
            });
            Base.SaveSQLRunningtme(sql, el);
            return obj;
        }
        internal static DbDataReader RunDataReader(CoreHelper.DBHelper __DbHelper, string sp)
        {
            DbDataReader reader = null;
            var el = Run(() =>
            {
                reader = __DbHelper.RunDataReader(sp);
            });
            Base.SaveSQLRunningtme(sp, el);
            return reader;
        }
        internal static DbDataReader ExecuteDataReader(CoreHelper.DBHelper __DbHelper, string sql)
        {
            DbDataReader reader = null;
            var el = Run(() =>
            {
                reader = __DbHelper.ExecDataReader(sql);
            });
            Base.SaveSQLRunningtme(sql, el);
            return reader;
        }
        static long Run(Action act)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            act();
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
    }
}
