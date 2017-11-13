/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
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
        internal static T ReturnList<T>(Func<T> func, string sql) where T : ICollection
        {
            T list = default(T);
            var el = Run(() =>
            {
                list = func();
            });
            var n = 0;
            if (list != null)
            {
                n = list.Count;
            }
            Base.SaveSQLRunningtme(sql, el, n);
            return list;
        }
        internal static T ReturnData<T>(Func<CallBackDataReader> func1, Func<CallBackDataReader, T> func2) where T : ICollection
        {
            T list = default(T);
            string sql = "";
            var el = Run(() =>
             {
                 var reader = func1();
                 if (reader != null)
                 {
                     sql = reader.Sql;
                     list = func2(reader);
                 }
                 else
                 {
                     list = Activator.CreateInstance<T>();
                 }
             });
            var n = 0;
            if(list!=null)
            {
                n = list.Count;
            }
            Base.SaveSQLRunningtme(sql, el, n);
            return list;
        }
        public static long Run(Action act)
        {
            var time = DateTime.Now;
            act();
            var ts = DateTime.Now - time;
            return (long)ts.TotalMilliseconds;
        }
    }
}
