using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace CRL.ExistsTableCache
{
    internal class ColumnBackgroundCheck
    {
        static Dictionary<string, DBExtend> dBExtends = new Dictionary<string, DBExtend>();
        static object lockObj = new object();
        static Dictionary<Type, string> needCheks = new Dictionary<Type, string>();
        static Thread thread;
        public static void Add(DBExtend dBExtend, Type type)
        {
            var dbName = dBExtend.dbHelper.DatabaseName;
            lock (lockObj)
            {
                if (!dBExtends.ContainsKey(dbName))
                {
                    dBExtends.Add(dbName, dBExtend);
                }
                if (!needCheks.ContainsKey(type))
                {
                    needCheks.Add(type, dbName);
                }
            }
            if (thread == null)
            {
                thread = new Thread(new ThreadStart(DoWatch));
                thread.Start();
            }
        }
        static void DoWatch()
        {
            #region watch
            while (true)
            {
                DoCheck();
                Thread.Sleep(10000);
            }
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
                var db = dBExtends[item.Value];
                var table=TypeCache.GetTable(item.Key);
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
                    if (!allFileds2.ContainsKey(field.Name.ToLower()))
                    {
                        needCreates.Add(field);
                    }
                }
                foreach (var field in needCreates)
                {
                    IModel.SetColumnDbType(_DBAdapter, field);
                    string str = IModel.CreateColumn(db, field);
                }
                needCheks.Remove(item.Key);
            }
        }
    }
}
