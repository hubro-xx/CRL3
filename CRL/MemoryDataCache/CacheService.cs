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
using System.Data;
using System.Threading;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using CoreHelper;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace CRL.MemoryDataCache
{
    /// <summary>
    /// 数据缓存组件
    /// 此组件会按参数缓存查询结果,在后台自动进行更新
    /// </summary>
    public class CacheService
    {

        static Thread thread;

        static object lockObj = new object();
        static ConcurrentDictionary<string, MemoryDataCacheItem> cacheDatas = new ConcurrentDictionary<string, MemoryDataCacheItem>();
        /// <summary>
        /// 缓存类型的KEY
        /// </summary>
        static Dictionary<Type, List<string>> typeCache = new Dictionary<Type, List<string>>();
        /// <summary>
        /// 获取同一类型缓存所有KEY
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static List<string> GetCacheTypeKey(Type type)
        {
            if (typeCache.ContainsKey(type))
            {
                return typeCache[type];
            }
            return new List<string>();
        }

        internal static void DeleteCacheItem<TItem>(string typeKey, string[] ids) where TItem : IModel
        {
            if (!cacheDatas.ContainsKey(typeKey))
                return;
            var data = cacheDatas[typeKey].Data as Dictionary<string, TItem>;
            foreach (var id in ids)
            {
                data.Remove(id);
            }
        }
        /// <summary>
        /// 更新缓存中的一项
        /// </summary>
        /// <param name="typeKey"></param>
        /// <param name="obj"></param>
        /// <param name="c"></param>
        /// <param name="checkInsert"></param>
        internal static void UpdateCacheItem<TItem>(string typeKey, TItem obj, ParameCollection c = null, bool checkInsert = false) where TItem : IModel
        {
            if (obj == null)
            {
                throw new Exception("obj is null");
            }
            if (!cacheDatas.ContainsKey(typeKey))
                return;
            var data = cacheDatas[typeKey].Data as Dictionary<string, TItem>;
            var keyValue = obj.GetpPrimaryKeyValue().ToString();
            if (!data.ContainsKey(keyValue))
            {
                if (checkInsert)
                {
                    data.Add(keyValue, obj);
                }
                return;
            }
            TItem originObj = data[keyValue];
            if (c != null)//按更改的值
            {
                var fields = TypeCache.GetProperties(obj.GetType(), false);
                var Reflection = ReflectionHelper.GetInfo<TItem>();
                foreach (var f in c)
                {
                    //var field = fields.Find(b => b.Name.ToUpper() == f.Key.ToUpper());
                    var field = fields[f.Key];
                    if (field == null)//名称带$时不更新
                        continue;
                    //field.TupleSetValue<TItem>(originObj, f.Value);
                    Reflection.GetAccessor(field.Name).Set((TItem)originObj, f.Value);
                }
            }
            else//整体更新
            {
                lock (lockObj)
                {
                    if (originObj != null)//删除原来的
                    {
                        data.Remove(keyValue);
                    }
                    data.Add(keyValue, obj);
                }
            }
            //CacheUpdated(data.Type.Name);
            string log = string.Format("更新缓存中的一项[{0}]", obj.GetModelKey());
            CoreHelper.EventLog.Log(log, "DataCache", false);
        }
        /// <summary>
        /// 获取缓存
        /// 缓存在进程重启后才失效
        /// </summary>
        /// <param name="qeury">表名或查询语句,存储过程前加exec </param>
        /// <param name="timeOut">失效分钟</param>
        /// <param name="helper">DBHelper对象,如果Params有值,则按参数缓存,慎用,会造成大量缓存</param>
        /// <returns></returns>
        internal static Dictionary<string, TItem> GetCacheList<TItem>(string qeury, int timeOut, DBHelper helper) where TItem : IModel, new()
        {
            string key = "";
            return GetCacheList<TItem>(qeury, timeOut, helper, out key);
        }

        internal static Dictionary<string, TItem> GetCacheList<TItem>(string qeury, int timeOut, DBHelper helper, out string key) where TItem : IModel, new()
        {
            Type type = typeof(TItem);
            qeury = qeury.ToLower();
            string Params = "";
            foreach (KeyValuePair<string, object> item in helper.Params)
            {
                Params += item.Key + ":" + item.Value;
            }
            //按参数进行缓存
            key = StringHelper.EncryptMD5(qeury + Params);
            //初始缓存
            //lock (lockObj)
            //{
                if (!cacheDatas.ContainsKey(key))
                {
                    cacheDatas.TryAdd(key, new MemoryDataCacheItem() { Data = null, TimeOut = timeOut, DBHelper = helper, Query = qeury, Params = new Dictionary<string, object>(helper.Params), Type = type });
                    if (typeCache.ContainsKey(type))
                    {
                        typeCache[type].Add(key);
                    }
                    else
                    {
                        typeCache[type] = new List<string>() { key };
                    }
                }
                else
                {
                    var dataItem2 = cacheDatas[key];
                    if (dataItem2.QueryCount == 0)//缓存没有创建好时返回空
                    {
                        return new Dictionary<string, TItem>();
                    }
                }
            //}
            var dataItem = cacheDatas[key];
            //首次查询
            if (dataItem.QueryCount == 0)
            {
                var data = QueryData(key, type, qeury, helper);
                dataItem.Data = ObjectConvert.ConvertToDictionary<TItem>(data);
                dataItem.Count = data.Count;
                dataItem.QueryCount = 1;
            }

            if (thread == null)
            {
                StarWatch();
            }
            //更新缓存数据
            if (dataItem.UpdatedData != null)
            {
                dataItem.Data = ObjectConvert.ConvertToDictionary<TItem>(dataItem.UpdatedData);
                dataItem.UpdatedData = null;
            }
            cacheDatas[key].UseTime = DateTime.Now;
            return dataItem.Data as Dictionary<string, TItem>;
        }


        static List<object> QueryData(string key,Type type, string query, DBHelper helper)
        {
            if (cacheDatas.Count > 1000)
            {
                EventLog.Log("数据缓存超过了1000个,请检查程序调用是否正确", true);
            }
            DateTime time = DateTime.Now;
            System.Data.Common.DbDataReader reader;
            string sql;
            //语句
            if (query.IndexOf("select ") > -1)
            {
                sql = query;
                reader = helper.ExecDataReader(sql);
            }//存储过程
            else if (query.IndexOf("exec ") > -1)
            {
                string sp = query.Replace("exec ", "");
                reader = helper.RunDataReader(sp);
            }//表名
            else
            {
                sql = "select * from " + query;
                reader = helper.ExecDataReader(sql);
            }
            double runTime;
            var list = ObjectConvert.DataReaderToList<object>(reader, type,out runTime, true);
            string par = "";
            foreach (KeyValuePair<string, object> item in helper.Params)
            {
                par += item.Key + ":" + item.Value;
            }
            //list.ForEach(b =>
            //    {
            //        var item = b as IModel;
            //        item.AddCacheListen();
            //    });
            var ts = DateTime.Now - time;
            //WriteLog("更新查询 " + tableName + " 参数 " + par);
            EventLog.Log("更新查询 " + key + " 用时:" + ts.TotalSeconds + "秒", "DataCache");
            return list;
        }
        
        /// <summary>
        /// 清空缓存
        /// </summary>
        /// <param name="type">对象名</param>
        public static void Clear(Type type = null)
        {
            if (type == null)
            {
                lock (lockObj)
                {
                    cacheDatas.Clear();
                }
            }
            else
            {
                var keys = GetCacheTypeKey(type);
                foreach (var key in keys)
                {
                    MemoryDataCache.MemoryDataCacheItem val;
                    cacheDatas.TryRemove(key,out val);
                }
            }
        }
        internal static Dictionary<string, TItem> GetCacheItem<TItem>(string key) where TItem : IModel
        {
            if (!cacheDatas.ContainsKey(key))
            {
                return new Dictionary<string, TItem>();
            }
            var dataItem = cacheDatas[key];
            dataItem.UseTime = DateTime.Now;
            if (dataItem.UpdatedData != null)
            {
                dataItem.Data = ObjectConvert.ConvertToDictionary<TItem>(dataItem.UpdatedData);
                dataItem.UpdatedData = null;
            }

            return dataItem.Data as Dictionary<string, TItem>;
        }
        /// <summary>
        /// 根据键移除一个缓存
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveCache(string key)
        {
            MemoryDataCacheItem val;
            cacheDatas.TryRemove(key,out val);
        }
        /// <summary>
        /// 根据键更新一个缓存
        /// </summary>
        /// <param name="key"></param>
        public static bool UpdateCache(string key)
        {
            key = key.Trim();
            if (cacheDatas.ContainsKey(key))
            {
                MemoryDataCacheItem cacheItem = cacheDatas[key];
                DBHelper helper = cacheItem.DBHelper;
                helper.Params = cacheItem.Params;
                try
                {
                    var data = QueryData(key, cacheItem.Type, cacheItem.Query, helper);
                    //将新数据放放UpdateData中, 下次调用时填入Data
                    cacheItem.UpdatedData = data;
                    cacheItem.UpdateTime = DateTime.Now;
                    cacheItem.QueryCount += 1;
                    cacheItem.Data = null;
                    return true;
                }
                catch { }
            }
            return false;
        }

        /// <summary>
        /// 获取缓存列表
        /// </summary>
        /// <returns></returns>
        public static List<QueryItem> GetCacheList()
        {
            List<QueryItem> result = new List<QueryItem>();
            foreach (KeyValuePair<string, MemoryDataCacheItem> item in cacheDatas)
            {
                string par = "";
                foreach (KeyValuePair<string, object> item1 in item.Value.Params)
                {
                    par += item1.Key + ":" + item1.Value;
                }
                result.Add(new QueryItem() { TableName = item.Value.Query, Key = item.Key, Params = par, TimeOut = item.Value.TimeOut, UpdateTime = item.Value.UpdateTime, RowCount = item.Value.Count, DataType = item.Value.Type });
            }
            return result;
        }
        /// <summary>
        /// 获取缓存数量
        /// </summary>
        public static int CacheCount
        {
            get
            {
                return cacheDatas.Count;
            }
        }
        #region 线程
        /// <summary>
        /// 启动线程监视
        /// </summary>
        public static void StarWatch()
        {
            if (thread == null)
            {
                thread = new Thread(new ThreadStart(DoWatch));
                thread.Start();
            }
        }
        /// <summary>
        /// 停止线程监视
        /// </summary>
        public static void StopWatch()
        {
            if (thread != null)
            {
                thread.Abort();
            }
            thread = null;
            //WriteLog("监听已停止");
        }
        static bool working = false;
        static void DoWatch()
        {
            #region watch
            while (true)
            {
                DoUpdate();
                Thread.Sleep(30000);
            }
            #endregion
        }
        static void DoUpdate()
        {
            if (working)
            {
                return;
            }
            var needUpdates = new List<UpdateItem>();
            List<string> removes = new List<string>();
            #region 找出需要更新的
            lock (lockObj)
            {
                //先找出超时的数据,避免长时间锁定,没法插入
                foreach (KeyValuePair<string, MemoryDataCacheItem> item in cacheDatas)
                {
                    TimeSpan ts = DateTime.Now - item.Value.UpdateTime;
                    TimeSpan useTimeOut = DateTime.Now - item.Value.UseTime;
                    int timeOutSecend = item.Value.TimeOut * 60;
                    //在周期内没有使用的不进行更新
                    bool needUpdate = useTimeOut.TotalSeconds <= timeOutSecend - timeOutSecend * 0.4;
                    //两个周期内没有使用就移除
                    bool needRemove = useTimeOut.TotalSeconds > timeOutSecend * 2;
                    if (needRemove)
                    {
                        removes.Add(item.Key);
                    }
                    if (ts.TotalSeconds > timeOutSecend && needUpdate)
                    {
                        needUpdates.Add(new UpdateItem() { Key = item.Key, TableName = item.Value.Query, DBHelper = item.Value.DBHelper, Params = item.Value.Params, UpdateTime = item.Value.UpdateTime, Type = item.Value.Type });
                    }
                }
            }
            #endregion
            lock (lockObj)
            {
                foreach (string s in removes)
                {
                    //暂不移除
                    //cacheTables.Remove(s);
                    //EventLog.Log(string.Format("KEY:{0} 两个周期未被使用,被移除", s), "DataCache");
                }
            }
            //批量更新
            if (needUpdates.Count == 0)
            {
                return;
            }
            working = true;
            int threadTask = needUpdates.Count / 10;//每个线程10个任务
            if (threadTask > 5)
                threadTask = 5;
            //多线程处理
            var threadSplit = new CoreHelper.ThreadSplit<UpdateItem>(needUpdates, threadTask);
            threadSplit.UseLog = false;
            //任务执行时
            #region 多线程
            threadSplit.OnWork = (sender) =>
            {
                foreach (var item in sender)
                {
                    try
                    {
                        //EventLog.WriteLog("更新TABLE " + item.Value.TableName);
                        //重新给参数赋值
                        DBHelper helper = item.DBHelper;
                        helper.Params = item.Params;
                        //将新数据放放UpdateData中, 下次调用时填入Data
                        var data = QueryData(item.Key, item.Type, item.TableName, helper);
                        cacheDatas[item.Key].UpdateTime = DateTime.Now;
                        cacheDatas[item.Key].UpdatedData = data;
                        cacheDatas[item.Key].Data = null;
                    }
                    catch (Exception ero)
                    {
                        EventLog.Log("更新数据缓存查询时出现错误 " + item.TableName + "错误," + ero.Message, true);
                    }
                }
            };
            //任务执行完成
            threadSplit.OnFinish += (sender, e) =>
            {
                working = false;
            };
            threadSplit.Start();
            #endregion
            needUpdates.Clear();
            needUpdates = null;
        }
        #endregion
        
    }
}
