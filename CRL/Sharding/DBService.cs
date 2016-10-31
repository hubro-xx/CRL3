/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using CRL.Sharding.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Sharding
{
    /// <summary>
    /// 数据表定位
    /// </summary>
    public class DBService
    {
        static List<DataBase> _DataBase = new List<DataBase>();
        static List<DB.Table> _Table = new List<DB.Table>();
        static List<TablePart> _TablePart = new List<TablePart>();
        static object lockObj = new object();
        /// <summary>
        /// 初始表
        /// </summary>
        public static void Init()
        {
            _DataBase = DataBaseManage.Instance.QueryList();
            _Table = TableManage.Instance.QueryList();
            _TablePart = TablePartManage.Instance.QueryList();
        }
        /// <summary>
        /// 按主数据索引,确定库
        /// </summary>
        /// <param name="mainDataIndex"></param>
        /// <returns></returns>
        public static DataBase GetDataBase(int mainDataIndex)
        {
            if (_DataBase.Count() == 0)
            {
                Init();
            }
            var db = _DataBase.Find(b => mainDataIndex >= b.MainDataStartIndex && mainDataIndex <= b.MainDataEndIndex);
            if (db == null)//找属于哪个库
            {
                throw new CRLException("找不到指定的库,在主数据索引:" + mainDataIndex);
            }
            return db;
        }
        /// <summary>
        /// 按主数据索引,获取该查询位置
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="mainDataIndex"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static Location GetLocation(string tableName, int mainDataIndex, DataBase db)
        {
            var table = _Table.Find(b => b.TableName == tableName && b.DataBaseName == db.Name);
            if (table == null)//找哪个表
            {
                throw new CRLException(string.Format("找不到指定的表{1}在库{0}", db.Name, tableName));
            }
            TablePart part;
            //找分表
            if (table.IsMainTable)//如果只是主数据表,只按一个找就行了
            {
                part = _TablePart.Find(b => b.TableName == tableName && b.DataBaseName == db.Name);
            }
            else//其它表,按分表找
            {
                part = _TablePart.Find(b => mainDataIndex >= b.MainDataStartIndex && mainDataIndex <= b.MainDataEndIndex && b.TableName == tableName && b.DataBaseName == db.Name);
            }
            if (part == null)
            {
                throw new CRLException(string.Format("找不到指定的表{1}在库{0}", db.Name, tableName));
            }
            return new Location() { DataBase = db, TablePart = part };
        }
        /// <summary>
        /// 按主数据索引,获取该查询位置
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="mainDataIndex"></param>
        /// <returns></returns>
        public static Location GetLocation(string tableName, int mainDataIndex)
        {
            var db = GetDataBase(mainDataIndex);
            return GetLocation(tableName, mainDataIndex,db);
        }
        /// <summary>
        /// 获取主数据表新索引和位置,并返回新索引
        /// </summary>
        public static int GetInsertMainDataLocation(string tableName, out Location location)
        {
            lock (lockObj)
            {
                var mainDataIndex = DataSequenceManage.Instance.GetSequence();
                location = GetLocation(tableName, mainDataIndex);
                return mainDataIndex;
            }
        }

        public static List<TablePart> GetAllTable(DataBase db,string tableName)
        {
            return _TablePart.FindAll(b => b.DataBaseName == db.Name && b.TableName == tableName);
        }
        
    }
}
