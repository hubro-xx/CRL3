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

namespace CRL.ExistsTableCache
{
    [Serializable]
    internal class ExistsTableCache : CoreHelper.ICoreConfig<ExistsTableCache>
    {
        #region 属性
        Dictionary<string, DataBase> dataBase = new Dictionary<string, DataBase>();
        public Dictionary<string, DataBase> DataBase
        {
            get { return dataBase; }
            set { dataBase = value; }
        }
        public string Server
        {
            get;
            set;
        }
        #endregion
        static object lockObj = new object();
        /// <summary>
        /// 初始所有表
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tables"></param>
        public void InitTable(string dbName, List<string> tables)
        {
            DataBase db;
            if (!DataBase.ContainsKey(dbName))
            {
                db = new DataBase() { Name = dbName };
                DataBase.Add(dbName, db);
            }
            db = DataBase[dbName];
            var tableCache = new Dictionary<string, Table>();
            foreach (var item in tables)
            {
                tableCache.Add(item.ToLower(), new Table() { Name = item.ToLower() });
            }
            db.Tables = tableCache;
        }
        /// <summary>
        /// 获取一个表
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public Table GetTable(string dbName, string tableName)
        {
            tableName = tableName.ToLower();
            var db = DataBase[dbName];
            Table tb;
            db.Tables.TryGetValue(tableName, out tb);
            return tb;
        }
        /// <summary>
        /// 保存表字段
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="table"></param>
        /// <param name="tableName"></param>
        public void SaveTable(string dbName, Attribute.TableAttribute table,string tableName)
        {
            tableName = tableName.ToLower();
            var fields = table.Fields;
            lock (lockObj)
            {
                var fields2 = new List<string>();
                fields.ForEach(b =>
                {
                    fields2.Add(b.MemberName.ToLower());
                });
                var tb = new Table() { Name = tableName, Fields = fields2 };
                var db = DataBase[dbName];
                db.Tables.Remove(tableName);
                db.Tables.Add(tableName,tb);
            }
            Save();
        }
        /// <summary>
        /// 检查字段
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="table"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public List<Attribute.FieldAttribute> CheckFieldExists(string dbName, Attribute.TableAttribute table, string tableName)
        {
            tableName = tableName.ToLower();
            var tb = GetTable(dbName, tableName);
            var returns = new List<Attribute.FieldAttribute>();
           
            if (tb.ColumnChecked)
            {
                return returns;
            }
            var fields = table.Fields;
    
            if (tb.Fields.Count==0)//首次不检查
            {
                SaveTable(dbName, table, tableName);
                return returns;
            }
            //检查字段是否一致
            foreach (var item in fields)
            {
                if (item.FieldType != Attribute.FieldType.数据库字段)
                    continue;
                if (!tb.Fields.Contains(item.MemberName.ToLower()))
                {
                    returns.Add(item);
                }
            }
            tb.ColumnChecked = true;
            if (returns.Count > 0)
            {
                var fields2 = new List<string>();
                fields.ForEach(b =>
                {
                    fields2.Add(b.MemberName.ToLower());
                });
                tb.Fields = fields2;
                Save();
            }
            return returns;
        }

    }
}
