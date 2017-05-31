using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL
{
    /// <summary>
    /// 对运行时间计时
    /// </summary>
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
        internal static T ReturnList<T>(Func<T> func,string sql)
        {
            T list = default(T);
            var el = Run(() =>
            {
                list = func();
            });
            Base.SaveSQLRunningtme(sql, el);
            return list;
        }
        internal static T ReturnData<T>(Func<CallBackDataReader> func1, Func<CallBackDataReader, T> func2)
        {
            T list = default(T);
            string sql = "";
            var el = Run(() =>
             {
                 var reader = func1();
                 sql = reader.Sql;
                 list = func2(reader);
             });
            Base.SaveSQLRunningtme(sql, el);
            return list;
        }
        //internal static DbDataReader RunDataReader(CoreHelper.DBHelper __DbHelper, string sp)
        //{
        //    DbDataReader reader = null;
        //    var el = Run(() =>
        //    {
        //        reader = __DbHelper.RunDataReader(sp);
        //    });
        //    Base.SaveSQLRunningtme(sp, el);
        //    return reader;
        //}
        //internal static DbDataReader ExecuteDataReader(CoreHelper.DBHelper __DbHelper, string sql)
        //{
        //    DbDataReader reader = null;
        //    var el = Run(() =>
        //    {
        //        reader = __DbHelper.ExecDataReader(sql);
        //    });
        //    Base.SaveSQLRunningtme(sql, el);
        //    return reader;
        //}
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
