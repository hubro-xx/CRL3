/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
namespace CRL.ExistsTableCache
{
    internal class ColumnBackgroundCheck
    {
        static ConcurrentDictionary<string, AbsDBExtend> dBExtends = new ConcurrentDictionary<string, AbsDBExtend>();
        //static object lockObj = new object();
        static ConcurrentDictionary<Type, string> needCheks = new ConcurrentDictionary<Type, string>();
        static System.Timers.Timer timer;
        public static void Add(AbsDBExtend dBExtend, Type type)
        {
            var dbName = dBExtend.DatabaseName;
            //lock (lockObj)
            //{
                if (!dBExtends.ContainsKey(dbName))
                {
                    dBExtends.TryAdd(dbName, dBExtend);
                }
                if (!needCheks.ContainsKey(type))
                {
                    needCheks.TryAdd(type, dbName);
                }
            //}
            if (timer == null)
            {
                timer = new System.Timers.Timer(15000);
                timer.Elapsed += (a, b) =>
                {
                    DoWatch();
                };
                timer.Start();
            }
        }
        static void DoWatch()
        {
            #region watch
            try
            {
                DoCheck();
            }
            catch { }
            #endregion
        }
        static void DoCheck()
        {
            if (needCheks.Count == 0)
            {
                return;
            }
            var list = new Dictionary<Type, string>(needCheks);
            foreach(var item in list)
            {
                var db = dBExtends[item.Value];//todo 线程安全,对象在别的地方被使用过了,导至异常
                var table = TypeCache.GetTable(item.Key);
                var _DBAdapter = DBAdapter.DBAdapterBase.GetDBAdapterBase(db.dbContext);
                var sql = _DBAdapter.GetTableFields(table.TableName);
                var allFileds = db.ExecDictionary<string, int>(sql);
                var allFileds2 = new Dictionary<string, int>();
                foreach(var f in allFileds)
                {
                    allFileds2.Add(f.Key.ToLower(), 0);
                }
                var fields = table.Fields;
                var needCreates = new List<Attribute.FieldAttribute>();
                foreach (var field in fields)
                {
                    if (field.FieldType != Attribute.FieldType.数据库字段)
                    {
                        continue;
                    }
                    if (!allFileds2.ContainsKey(field.MapingName.ToLower()))
                    {
                        needCreates.Add(field);
                    }
                }
                //var model = System.Activator.CreateInstance(item.Key) as IModel;
                foreach (var field in needCreates)
                {
                    ModelCheck.SetColumnDbType(_DBAdapter, field);
                    string str = ModelCheck.CreateColumn(db, field);
                }
                string val;
                needCheks.TryRemove(item.Key, out val);
            }
        }
        public static void Stop()
        {
            if (timer != null)
            {
                timer.Stop();
            }
            timer = null;
        }
    }
}
