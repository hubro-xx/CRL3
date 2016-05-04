/**
* CRL 快速开发框架 V3.1
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
    internal class ExistsTableCache
    {
        #region 属性
        List<DataBase> dataBase = new List<DataBase>();
        public List<DataBase> DataBase
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
            var db = DataBase.Find(b => b.Name == dbName);
            if (db == null)
            {
                db = new DataBase() { Name = dbName };
                DataBase.Add(db);
            }
            var tableCache = new List<Table>();
            foreach (var item in tables)
            {
                tableCache.Add(new Table() { Name = item.ToLower() });
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
            var db = DataBase.Find(b => b.Name == dbName);
            return db.Tables.Find(b => b.Name == tableName);
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
                    fields2.Add(b.Name.ToLower());
                });
                var tb = new Table() { Name = tableName, Fields = fields2 };
                var db = DataBase.Find(b => b.Name == dbName);
                db.Tables.RemoveAll(b => b.Name == tableName);
                db.Tables.Add(tb);
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
                if (!tb.Fields.Contains(item.Name.ToLower()))
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
                    fields2.Add(b.Name.ToLower());
                });
                tb.Fields = fields2;
                Save();
            }
            return returns;
        }
        
        private static ExistsTableCache instance;
        /// <summary>
        /// 实例
        /// </summary>
        internal static ExistsTableCache Instance
        {
            get
            {
                if (instance == null)
                    instance = FromFile();
                return instance;
            }
            set
            {
                instance = value;
            }
        }
        const string confgiFile = @"\config\TableCache.config";
        #region 初始
        public static ExistsTableCache FromFile()
        {
            var file = CoreHelper.RequestHelper.GetFilePath(confgiFile);
            var folder = CoreHelper.RequestHelper.GetFilePath(@"\config");
            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }
            ExistsTableCache cache = null;
            var server = CoreHelper.RequestHelper.GetServerIp();
            if (System.IO.File.Exists(file))
            {
                try
                {
                    //cache = CoreHelper.SerializeHelper.BinaryDeserialize<ExistsTableCache>(file);
                    //cache = CoreHelper.SerializeHelper.XmlDeserialize<ExistsTableCache>(file);
                    var json = System.IO.File.ReadAllText(file);
                    cache = (ExistsTableCache)CoreHelper.SerializeHelper.SerializerFromJSON(json, typeof(ExistsTableCache), Encoding.UTF8);
                    //不是同一服务器创建的缓存则重建
                    if (cache.Server != server)
                    {
                        //cache = new ExistsTableCache() { Server = server };
                    }
                }
                catch { }
            }
            if (cache == null)
                cache = new ExistsTableCache() { Server = server };
            return cache;
        }
        public void Save()
        {
            var file = CoreHelper.RequestHelper.GetFilePath(confgiFile);
            lock (lockObj)
            {
                //CoreHelper.SerializeHelper.BinarySerialize(this, file);
                var json = CoreHelper.SerializeHelper.SerializerToJson(this, Encoding.UTF8);
                System.IO.File.WriteAllText(file,json);
                //CoreHelper.EventLog.Log("保存CoreConfig");
            }
        }
        #endregion
    }
}
