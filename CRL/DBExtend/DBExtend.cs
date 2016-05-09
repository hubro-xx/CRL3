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
using System.Reflection;
using System.Data;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Data.Common;
using CRL.LambdaQuery;
namespace CRL
{
    /// <summary>
    /// 对象数据访问
    /// </summary>
    public sealed partial class DBExtend
    {
        #region 属性
        /// <summary>
        /// 对象被更新时,是否通知缓存服务器
        /// </summary>
        internal bool OnUpdateNotifyCacheServer = false;
        internal Guid GUID;

        DBExtend backgroundDBExtend;
        /// <summary>
        /// 仅用来检查表结构
        /// </summary>
        /// <returns></returns>
        DBExtend GetBackgroundDBExtend()
        {
            if (backgroundDBExtend == null)
            {
                var helper = SettingConfig.GetDbAccess(dbContext.DBLocation);
                var dbContext2 = new DbContext(helper, dbContext.DBLocation);
                dbContext2.ShardingMainDataIndex = dbContext.ShardingMainDataIndex;
                dbContext2.UseSharding = dbContext.UseSharding;
                backgroundDBExtend = new DBExtend(dbContext2);
            }
            return backgroundDBExtend;
        }
       

        internal CoreHelper.DBHelper dbHelper;
        internal string DatabaseName
        {
            get
            {
                return dbHelper.DatabaseName;
            }
        }
        DBAdapter.DBAdapterBase __DBAdapter;
        /// <summary>
        /// 当前数据库适配器
        /// </summary>
        internal DBAdapter.DBAdapterBase _DBAdapter
        {
            get
            {
                //return Base.CurrentDBAdapter;
                if (__DBAdapter == null)
                {
                    __DBAdapter = DBAdapter.DBAdapterBase.GetDBAdapterBase(dbContext);
                }
                return __DBAdapter;
            }
        }

        static object lockObj = new object();
        
        #endregion

        internal DbContext dbContext;
        public override string ToString()
        {
            return string.Format("{0} {1}", GUID, currentTransStatus);
        }
        /// <summary>
        /// 构造DBExtend
        /// </summary>
        /// <param name="_dbContext"></param>
        public DBExtend(DbContext _dbContext)
        {
            dbContext = _dbContext;
            var _helper = _dbContext.DBHelper;
            if (_helper == null)
            {
                throw new Exception("数据访问对象未实例化,请实现CRL.SettingConfig.GetDbAccess");
            }
            GUID = Guid.NewGuid();
            dbHelper = _helper;
        }
        #region 参数处理
        /// <summary>
        /// 清除参数
        /// </summary>
        public void ClearParams()
        {
            dbHelper.ClearParams();
        }
        /// <summary>
        /// 增加参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddParam(string name,object value)
        {
            value = ObjectConvert.CheckNullValue(value);
            dbHelper.AddParam(name,value);
        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetParam(string name, object value)
        {
            value = ObjectConvert.CheckNullValue(value);
            dbHelper.SetParam(name, value);
        }
        /// <summary>
        /// 增加输出参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value">对应类型任意值</param>
        public void AddOutParam(string name, object value = null)
        {
            dbHelper.AddOutParam(name, value);
        }
        /// <summary>
        /// 获取存储过程return的值
        /// </summary>
        /// <returns></returns>
        public int GetReturnValue()
        {
            return dbHelper.GetReturnValue();
        }
        /// <summary>
        /// 获取OUTPUT的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetOutParam(string name)
        {
            return dbHelper.GetOutParam(name);
        }
        /// <summary>
        /// 获取OUT值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetOutParam<T>(string name)
        {
            var obj = dbHelper.GetOutParam(name);
            return ObjectConvert.ConvertObject<T>(obj);
        }
        void ClearParame()
        {
            dbHelper.ClearParams();
        }
        #endregion
        void CheckData(IModel obj)
        {
            var types = CRL.TypeCache.GetProperties(obj.GetType(), true).Values;
            string msg;
            var sb = new StringBuilder();
            //检测数据约束
            foreach (Attribute.FieldAttribute p in types)
            {
                string value = p.GetValue(obj) + "";
                if (!string.IsNullOrEmpty(value) && p.Name != "AddTime" && obj.CheckRepeatedInsert)
                {
                    sb.Append(value.GetHashCode().ToString());
                }
                if (p.PropertyType == typeof(System.String))
                {
                    if (p.NotNull && string.IsNullOrEmpty(value))
                    {
                        msg = string.Format("对象{0}属性{1}值不能为空", obj.GetType(), p.Name);
                        throw new Exception(msg);
                    }
                    if (value.Length > p.Length && p.Length < 3000)
                    {
                        msg = string.Format("对象{0}属性{1}长度超过了设定值{2}", obj.GetType(), p.Name, p.Length);
                        throw new Exception(msg);
                    }
                }
            }
            if (obj.CheckRepeatedInsert)
            {
                string concurrentKey = "insertRepeatedCheck_" + CoreHelper.StringHelper.EncryptMD5(sb.ToString());
                if (!CoreHelper.ConcurrentControl.Check(concurrentKey, 3))
                {
                    throw new Exception("检测到有重复提交的数据");
                }
            }
            //校验数据
            msg = obj.CheckData();
            if (!string.IsNullOrEmpty(msg))
            {
                msg = string.Format("数据校验证失败,在类型{0} {1} 请核对校验规则", obj.GetType(), msg);
                throw new Exception(msg);
            }
        }

        #region 更新缓存中的一项

        /// <summary>
        /// 按表达式更新缓存中项
        /// 当前类型有缓存时才会进行查询
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="expression"></param>
        void UpdateCacheItem<TItem>(Expression<Func<TItem, bool>> expression, ParameCollection c) where TItem : IModel, new()
        {
            //事务开启不执行查询
            if (currentTransStatus == TranStatus.已开始)
            {
                return;
            }
            Type type = typeof(TItem);
            var updateModel = MemoryDataCache.CacheService.GetCacheTypeKey(typeof(TItem));
            foreach (var key in updateModel)
            {
                var list = QueryList<TItem>(expression);
                foreach (var item in list)
                {
                    MemoryDataCache.CacheService.UpdateCacheItem(key, item, c);
                    //NotifyCacheServer(item);//远端缓存暂无法更新
                }
            }
        }
        /// <summary>
        /// 更新缓存中的一项
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="newObj"></param>
        /// <param name="c"></param>
        internal void UpdateCacheItem<TItem>(TItem newObj, ParameCollection c, bool checkInsert = false) where TItem : IModel
        {
            var updateModel = MemoryDataCache.CacheService.GetCacheTypeKey(typeof(TItem));
            foreach (var key in updateModel)
            {
                MemoryDataCache.CacheService.UpdateCacheItem(key, newObj, c, checkInsert);
            }
            NotifyCacheServer(newObj);
            //Type type = typeof(TItem);
            //if (TypeCache.ModelKeyCache.ContainsKey(type))
            //{
            //    string key = TypeCache.ModelKeyCache[type];
            //    MemoryDataCache.UpdateCacheItem(key,newObj);
            //}
        }
        /// <summary>
        /// 通知缓存服务器
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="newObj"></param>
        void NotifyCacheServer<TItem>(TItem newObj) where TItem : IModel
        {
            if (!OnUpdateNotifyCacheServer)
                return;
            System.Threading.Tasks.Task.Run(() =>
            {
                var client = CacheServerSetting.GetCurrentClient(typeof(TItem));
                if (client != null)
                {
                    client.Update(newObj);
                }
            });
        }
        #endregion
        /// <summary>
        /// 格式化为更新值查询
        /// </summary>
        /// <param name="setValue"></param>
        /// <returns></returns>
        string ForamtSetValue<T>(ParameCollection setValue) where T : IModel
        {
            string tableName = TypeCache.GetTableName(typeof(T),dbContext);
            string setString = "";
            foreach (var pair in setValue)
            {
                string name = pair.Key;
                object value = pair.Value;
                value = ObjectConvert.CheckNullValue(value);
                if (name.StartsWith("$"))//直接按值拼接 c2["$SoldCount"] = "SoldCount+" + num;
                {
                    name = name.Substring(1, name.Length - 1);
                    setString += string.Format(" {0}={1},", _DBAdapter.KeyWordFormat(name), value);
                }
                else
                {
                    setString += string.Format(" {0}=@{1},", _DBAdapter.KeyWordFormat(name), name);
                    dbHelper.AddParam(name, value);
                }
            }
            setString = setString.Substring(0, setString.Length - 1);
            return setString;
        }
        /// <summary>
        /// 通过关键类型,格式化SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args">格式化SQL语句的关键类型</param>
        /// <returns></returns>
        string AutoFormat(string sql, params Type[] args)
        {
            if (args == null)
            {
                return sql;
            }
            if (args.Length == 0)
            {
                return sql;
            }
            //System.Object
            Regex r = new Regex(@"\$(\w+)", RegexOptions.IgnoreCase);//like $table
            Match m;
            List<string> pars = new List<string>();
            for (m = r.Match(sql); m.Success; m = m.NextMatch())
            {
                string par = m.Groups[1].ToString();
                if (!pars.Contains(par))
                {
                    pars.Add(par);
                }
            }
            foreach (string par in pars)
            {
                foreach (Type type in args)
                {
                    string tableName = TypeCache.GetTableName(type,null);
                    //string fullTypeName = type.FullName.Replace("+", ".") + ".";//like classA+classB
                    string fullTypeName = GetTypeFullName(type);
                    if (fullTypeName.IndexOf("." + par + ".") > -1)
                    {
                        sql = sql.Replace("$" + par, tableName);
                    }
                }
            }
            if (sql.IndexOf("$") > -1)
            {
                throw new Exception("格式化SQL语句时发生错误,表名未被替换:" + sql);
            }
            return sql;
        }
        static string GetTypeFullName(Type type)
        {
            string str = "";
            while (type != typeof(IModel))
            {
                str += "." + type.FullName + ".;";
                type = type.BaseType;
            }
            return str;
        }

        #region 语句查询
        /// <summary>
        /// 指定替换对象查询,并返回对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public List<T> ExecList<T>(string sql, params Type[] types) where T : class, new()
        {
            sql = _DBAdapter.SqlFormat(sql);
            var reader = GetDataReader(sql, types);
            double runTime;
            return ObjectConvert.DataReaderToList<T>(reader, out runTime);
        }
        
        DbDataReader GetDataReader(string sql, params Type[] types)
        {
            sql = AutoFormat(sql, types);
            sql = _DBAdapter.SqlFormat(sql);
            var  reader = dbHelper.ExecDataReader(sql);
            ClearParame();
            return reader;
        }
        public Dictionary<TKey, TValue> ExecDictionary<TKey, TValue>(string sql, params Type[] types)
        {
            var reader = GetDataReader(sql, types);
            return ObjectConvert.DataReadToDictionary<TKey, TValue>(reader);
        }

        /// <summary>
        /// 指定替换对象更新
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public int Execute(string sql, params Type[] types)
        {
            sql = AutoFormat(sql, types);
            sql = _DBAdapter.SqlFormat(sql);
            int count = dbHelper.Execute(sql);
            ClearParame();
            return count;
        }
        /// <summary>
        /// 指定替换对象返回单个结果
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="types">格式化SQL语句的关键类型</param>
        /// <returns></returns>
        public object ExecScalar(string sql, params Type[] types)
        {
            sql = AutoFormat(sql, types);
            sql = _DBAdapter.SqlFormat(sql);
            object obj = dbHelper.ExecScalar(sql);
            ClearParame();
            return obj;
        }
        /// <summary>
        /// 指定替换对象返回单个结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public T ExecScalar<T>(string sql, params Type[] types)
        {
            sql = _DBAdapter.SqlFormat(sql);
            var obj = ExecScalar(sql, types);
            return ObjectConvert.ConvertObject<T>(obj);
        }
        /// <summary>
        /// 返回首个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public T ExecObject<T>(string sql, params Type[] types) where T : class, new()
        {
            var list = ExecList<T>(sql, types);
            if (list.Count == 0)
                return null;
            return list[0];
        }
        #endregion

        #region 存储过程
        /// <summary>
        /// 执行存储过程返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sp"></param>
        /// <returns></returns>
        public List<T> RunList<T>(string sp) where T : class, new()
        {
            var reader = dbHelper.RunDataReader(sp);
            ClearParame();
            double runTime;
            return ObjectConvert.DataReaderToList<T>(reader, out runTime);
        }
        /// <summary>
        /// 执行一个存储过程
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public int Run(string sp)
        {
            int count = dbHelper.Run(sp);
            ClearParame();
            return count;
        }
        /// <summary>
        /// 返回首个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sp"></param>
        /// <returns></returns>
        public T RunObject<T>(string sp) where T : class, new()
        {
            var list = RunList<T>(sp);
            if (list.Count == 0)
                return null;
            return list[0];
        }
        /// <summary>
        /// 执行存储过程并返回结果
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public object RunScalar(string sp)
        {
            object obj = dbHelper.RunScalar(sp);
            ClearParame();
            return obj;
        }

        #endregion

        #region 事务控制
        enum TranStatus
        {
            未开始,
            已开始
        }
        TranStatus currentTransStatus = TranStatus.未开始;
        /// <summary>
        /// 开始物务
        /// </summary>
        internal void BeginTran()
        {
            if (currentTransStatus != TranStatus.未开始)
            {
                throw new Exception("事务开始失败,已有未完成的事务");
            }
            dbHelper.BeginTran();
            currentTransStatus = TranStatus.已开始;
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        internal void RollbackTran()
        {
            if (currentTransStatus != TranStatus.已开始)
            {
                throw new Exception("事务回滚失败,没有需要回滚的事务");
            }
            dbHelper.RollbackTran();
            currentTransStatus = TranStatus.未开始;
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        internal void CommitTran()
        {
            if (currentTransStatus != TranStatus.已开始)
            {
                throw new Exception("事务提交失败,没有需要提交的事务");
            }
            dbHelper.CommitTran();
            currentTransStatus = TranStatus.未开始;
        }
        #endregion

        #region 检查表是否被创建
        internal void CheckTableCreated<T>() where T : IModel, new()
        {
            CheckTableCreated(typeof(T));
        }
        //static Dictionary<string, List<string>> tableCache = new Dictionary<string, List<string>>();
        /// <summary>
        /// 检查表是否被创建
        /// </summary>
       internal void CheckTableCreated(Type type)
        {
            if (!SettingConfig.CheckModelTableMaping)
            {
                return;
            }
            //TypeCache.SetDBAdapterCache(type, _DBAdapter);
            var dbName = dbHelper.DatabaseName;
            var cacheInstance =CRL.ExistsTableCache.ExistsTableCache.Instance;
            var table = TypeCache.GetTable(type);
            DBExtend db;
            if (!cacheInstance.DataBase.ContainsKey(dbName))
            {
                db = GetBackgroundDBExtend();
                #region 初始表
                lock (lockObj)
                {
                    //BackupParams();
                    string sql = _DBAdapter.GetAllTablesSql();
                    var dic = db.ExecDictionary<string, int>(sql);
                    //RecoveryParams();
                    cacheInstance.InitTable(dbName, dic.Keys.ToList());
                }
                #endregion
            }
            var tableName = TypeCache.GetTableName(table.TableName, dbContext);
            var tb = cacheInstance.GetTable(dbName, tableName);
            if (tb == null)//没有创建表
            {
                db = GetBackgroundDBExtend();
                #region 创建表
                //BackupParams();
                var obj = System.Activator.CreateInstance(type) as IModel;
                var initDatas = obj.GetInitData();
                if (initDatas != null)
                {
                    foreach (IModel item in initDatas)
                    {
                        CheckData(item);
                    }
                }
                string msg;
                obj.CreateTable(db, out msg);
                //RecoveryParams();
                //if (!a)
                //{
                //    return;
                //    throw new Exception(msg);
                //}
                cacheInstance.SaveTable(dbName, table, tableName);
                if (initDatas != null)
                {
                    _DBAdapter.BatchInsert(initDatas, false);
                }
                return;
                #endregion
            }
            if (tb.ColumnChecked)
            {
                return;
            }
            //判断字段是否一致
            var needCreates = CRL.ExistsTableCache.ExistsTableCache.Instance.CheckFieldExists(dbName, table, tableName);
            if (needCreates.Count > 0)
            {
                db = GetBackgroundDBExtend();
                #region 创建列
                //BackupParams();
                foreach (var item in needCreates)
                {
                    IModel.SetColumnDbType(_DBAdapter, item);
                    string str = IModel.CreateColumn(db, item);
                }
                //RecoveryParams();
                #endregion
            }
           //二次检查,对照表结构
            if (!tb.ColumnChecked2)
            {
                db = GetBackgroundDBExtend();
                ExistsTableCache.ColumnBackgroundCheck.Add(db, type);
                tb.ColumnChecked2 = true;
            }
        }
        #endregion


    }
}
