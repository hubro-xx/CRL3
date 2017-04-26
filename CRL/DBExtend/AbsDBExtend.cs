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
using System.Linq.Expressions;
using System.Text;
using CoreHelper;
namespace CRL
{
    public abstract class AbsDBExtend
    {
        /// <summary>
        /// 创建当前数据库类型查询
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        protected abstract LambdaQuery.LambdaQuery<TModel> CreateLambdaQuery<TModel>() where TModel : IModel, new();
        #region 属性
        /// <summary>
        /// 数据库架构类型
        /// </summary>
        internal DataBaseArchitecture DataBaseArchitecture
        {
            get
            {
                return dbContext.DataBaseArchitecture;
            }
        }
        /// <summary>
        /// 对象被更新时,是否通知缓存服务器
        /// </summary>
        public bool OnUpdateNotifyCacheServer
        {
            get;
            set;
        }
        internal enum TranStatus
        {
            未开始,
            已开始
        }
        /// <summary>
        /// 事务状态
        /// </summary>
        internal TranStatus currentTransStatus = TranStatus.未开始;
        internal Guid GUID;

        AbsDBExtend backgroundDBExtend;
        /// <summary>
        /// 仅用来检查表结构
        /// </summary>
        /// <returns></returns>
        internal AbsDBExtend GetBackgroundDBExtend()
        {
            if (backgroundDBExtend == null)
            {
                backgroundDBExtend = copyDBExtend();
            }
            return backgroundDBExtend;
        }
        internal AbsDBExtend copyDBExtend()
        {
            var helper = SettingConfig.GetDbAccess(dbContext.DBLocation);
            var dbContext2 = new DbContext(helper, dbContext.DBLocation);
            dbContext2.ShardingMainDataIndex = dbContext.ShardingMainDataIndex;
            dbContext2.UseSharding = dbContext.UseSharding;
            return DBExtendFactory.CreateDBExtend(dbContext2);
        }

        internal CoreHelper.DBHelper __DbHelper;
        /// <summary>
        /// 库名
        /// </summary>
        internal string DatabaseName
        {
            get
            {
                return CoreHelper.StringHelper.EncryptMD5(__DbHelper.ConnectionString);
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

        /// <summary>
        /// lockObj
        /// </summary>
        static protected object lockObj = new object();
        internal DbContext dbContext;
        #endregion
        /// <summary>
        ///检测数据
        /// </summary>
        /// <param name="obj"></param>
        internal void CheckData(IModel obj)
        {
            //var types = CRL.TypeCache.GetProperties(obj.GetType(), true).Values;
            var types = TypeCache.GetTable(obj.GetType()).Fields;
            string msg;
            var sb = new StringBuilder();
            //检测数据约束
            foreach (Attribute.FieldAttribute p in types)
            {
                string value = p.GetValue(obj) + "";
                if (!string.IsNullOrEmpty(value) && p.MemberName != "AddTime" && obj.CheckRepeatedInsert)
                {
                    sb.Append(value.GetHashCode().ToString());
                }
                if (p.PropertyType == typeof(System.String))
                {
                    if (p.NotNull && string.IsNullOrEmpty(value))
                    {
                        msg = string.Format("对象{0}属性{1}值不能为空", obj.GetType(), p.MemberName);
                        throw new CRLException(msg);
                    }
                    if (value.Length > p.Length && p.Length < 3000)
                    {
                        msg = string.Format("对象{0}属性{1}长度超过了设定值{2}[{3}]", obj.GetType(), p.MemberName, p.Length, value);
                        throw new CRLException(msg);
                    }
                }
            }
            if (obj.CheckRepeatedInsert)
            {
                string concurrentKey = "insertRepeatedCheck_" + CoreHelper.StringHelper.EncryptMD5(sb.ToString());
                if (!CoreHelper.ConcurrentControl.Check(concurrentKey, 1))
                {
                    throw new CRLException("检测到有重复提交的数据,在" + obj.GetType());
                }
            }
            //校验数据
            msg = obj.CheckData();
            if (!string.IsNullOrEmpty(msg))
            {
                msg = string.Format("数据校验证失败,在类型{0} {1} 请核对校验规则", obj.GetType(), msg);
                throw new CRLException(msg);
            }
        }

        #region 更新缓存

        /// <summary>
        /// 按表达式更新缓存中项
        /// 当前类型有缓存时才会进行查询
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <param name="c"></param>
        internal void UpdateCacheItem<TModel>(Expression<Func<TModel, bool>> expression, ParameCollection c) where TModel : IModel, new()
        {
            if (DataBaseArchitecture == CRL.DataBaseArchitecture.NotRelation)
            {
                //非关系型没有缓存
                return;
            }
            //事务开启不执行查询
            if (currentTransStatus == TranStatus.已开始)
            {
                return;
            }
            Type type = typeof(TModel);
            var updateModel = MemoryDataCache.CacheService.GetCacheTypeKey(typeof(TModel));
            var db = copyDBExtend();//使用新的访问对象
            foreach (var key in updateModel)
            {
                var list = db.QueryList<TModel>(expression);
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
        /// <typeparam name="TModel"></typeparam>
        /// <param name="newObj"></param>
        /// <param name="c"></param>
        /// <param name="checkInsert"></param>
        internal void UpdateCacheItem<TModel>(TModel newObj, ParameCollection c, bool checkInsert = false) where TModel : IModel, new()
        {
            if (DataBaseArchitecture == CRL.DataBaseArchitecture.NotRelation)
            {
                //非关系型没有缓存
                return;
            }
            var updateModel = MemoryDataCache.CacheService.GetCacheTypeKey(typeof(TModel));
            foreach (var key in updateModel)
            {
                MemoryDataCache.CacheService.UpdateCacheItem(key, newObj, c, checkInsert);
            }
            NotifyCacheServer(newObj);
            //Type type = typeof(TModel);
            //if (TypeCache.ModelKeyCache.ContainsKey(type))
            //{
            //    string key = TypeCache.ModelKeyCache[type];
            //    MemoryDataCache.UpdateCacheItem(key,newObj);
            //}
        }
        /// <summary>
        /// 通知缓存服务器
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="newObj"></param>
        void NotifyCacheServer<TModel>(TModel newObj) where TModel : IModel
        {
            if (DataBaseArchitecture == CRL.DataBaseArchitecture.NotRelation)
            {
                //非关系型没有缓存
                return;
            }
            if (!OnUpdateNotifyCacheServer)
                return;
            System.Threading.Tasks.Task.Run(() =>
            {
                var client = CacheServerSetting.GetCurrentClient(typeof(TModel));
                if (client != null)
                {
                    client.Update(newObj);
                }
            });
        }
        #endregion
        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", GUID, DatabaseName);
        }

        /// <summary>
        /// 构造DBExtend
        /// </summary>
        /// <param name="_dbContext"></param>
        public AbsDBExtend(DbContext _dbContext)
        {
            dbContext = _dbContext;
            var _helper = _dbContext.DBHelper;
            if (_helper == null)
            {
                throw new CRLException("数据访问对象未实例化,请实现CRL.SettingConfig.GetDbAccess");
            }
            GUID = Guid.NewGuid();
            __DbHelper = _helper;
        }
        #region 参数
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public abstract void SetParam(string name, object value);
        /// <summary>
        /// 增加存储过程out参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public abstract void AddOutParam(string name, object value = null);
        /// <summary>
        /// 增加参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public abstract void AddParam(string name, object value);
        /// <summary>
        /// 清除参数
        /// </summary>
        public abstract void ClearParame();
        /// <summary>
        /// 获取out参数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract object GetOutParam(string name);
        /// <summary>
        /// 获存储过程取out参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract T GetOutParam<T>(string name);
        /// <summary>
        /// 获取存储过程返回值
        /// </summary>
        /// <returns></returns>
        public abstract int GetReturnValue();
        #endregion

        #region 自动编译
        ///// <summary>
        ///// 返回首行结果
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="sql"></param>
        ///// <param name="types"></param>
        ///// <returns></returns>
        //public abstract T AutoExecuteScalar<T>(string sql, params Type[] types);

        ///// <summary>
        ///// 自动编译为存储过程查询
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="sql"></param>
        ///// <param name="types"></param>
        ///// <returns></returns>
        //public abstract List<T> AutoSpQuery<T>(string sql, params Type[] types) where T : class, new();
        ///// <summary>
        ///// 自动编译为存储过程查询
        ///// </summary>
        ///// <typeparam name="TKey"></typeparam>
        ///// <typeparam name="TValue"></typeparam>
        ///// <param name="sql"></param>
        ///// <param name="types"></param>
        ///// <returns></returns>
        //public abstract Dictionary<TKey, TValue> AutoSpQuery<TKey, TValue>(string sql, params Type[] types);
        ///// <summary>
        ///// 编译为存储过程更新
        ///// </summary>
        ///// <param name="sql"></param>
        ///// <param name="types"></param>
        ///// <returns></returns>
        //public abstract int AutoSpUpdate(string sql, params Type[] types);

        #endregion

        #region 事务
        /// <summary>
        /// 开始事务
        /// </summary>
        public abstract void BeginTran();
        /// <summary>
        /// 回滚事务
        /// </summary>
        public abstract void RollbackTran();
        /// <summary>
        /// 提交事务
        /// </summary>
        public abstract void CommitTran();
        #endregion
        /// <summary>
        /// 检查表是否创建了
        /// </summary>
        /// <param name="type"></param>
        internal abstract void CheckTableCreated(Type type);
        #region Insert
        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="obj"></param>
        public abstract void InsertFromObj<TModel>(TModel obj) where TModel : CRL.IModel, new();
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="details"></param>
        /// <param name="keepIdentity"></param>
        public abstract void BatchInsert<TModel>(List<TModel> details, bool keepIdentity = false) where TModel : CRL.IModel, new();
        #endregion

        #region 删除
        /// <summary>
        /// 按完整查询删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public abstract int Delete<T>(CRL.LambdaQuery.LambdaQuery<T> query) where T : CRL.IModel, new();
        /// <summary>
        /// 关联删除
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TJoin"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public abstract int Delete<TModel, TJoin>(System.Linq.Expressions.Expression<Func<TModel, TJoin, bool>> expression)
            where TModel : CRL.IModel, new()
            where TJoin : CRL.IModel, new();
        /// <summary>
        /// 按条件删除
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public abstract int Delete<TModel>(System.Linq.Expressions.Expression<Func<TModel, bool>> expression) where TModel : CRL.IModel, new();
        /// <summary>
        /// 按主键删除
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract int Delete<TModel>(object id) where TModel : CRL.IModel, new();
        #endregion

        #region 杂项查询
        /// <summary>
        /// 返回字典
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="sql"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public abstract Dictionary<TKey, TValue> ExecDictionary<TKey, TValue>(string sql, params Type[] types);
        /// <summary>
        /// 返回字典
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public abstract Dictionary<TKey, TValue> ToDictionary<TModel,TKey, TValue>(LambdaQuery.LambdaQuery<TModel> query)where TModel : CRL.IModel, new();

        /// <summary>
        /// 返回首行首列
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public abstract dynamic QueryScalar<TModel>(LambdaQuery.LambdaQuery<TModel> query) where TModel : CRL.IModel, new();
        /// <summary>
        /// 返回动态类型
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public abstract List<dynamic> ExecDynamicList(string sql, params Type[] types);
        /// <summary>
        /// 返回自定义类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public abstract List<T> ExecList<T>(string sql, params Type[] types) where T : class, new();
        /// <summary>
        /// 返回自定义类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public abstract T ExecObject<T>(string sql, params Type[] types) where T : class, new();
        /// <summary>
        /// 返回首行首个结果
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public abstract object ExecScalar(string sql, params Type[] types);
        /// <summary>
        /// 返回首行首个结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public abstract T ExecScalar<T>(string sql, params Type[] types);
        /// <summary>
        /// 执行一条语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public abstract int Execute(string sql, params Type[] types);
        #endregion

        #region 函数
        #region count
        /// <summary>
        /// count
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="expression"></param>
        /// <param name="compileSp"></param>
        /// <returns></returns>
        public int Count<TType>(Expression<Func<TType, bool>> expression, bool compileSp = false) where TType : IModel, new()
        {
            return GetFunction<int, TType>(expression, b => 0, FunctionType.COUNT, compileSp);
        }

        #endregion

        #region Min
        /// <summary>
        /// 最小值
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <param name="field"></param>
        /// <param name="compileSp"></param>
        /// <returns></returns>
        public TType Min<TType, TModel>(Expression<Func<TModel, bool>> expression, Expression<Func<TModel, TType>> field, bool compileSp = false) where TModel : IModel, new()
        {
            return GetFunction<TType, TModel>(expression, field, FunctionType.MIN, compileSp);
        }
        #endregion

        #region Max
        /// <summary>
        /// 最大值
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <param name="field"></param>
        /// <param name="compileSp"></param>
        /// <returns></returns>
        public TType Max<TType, TModel>(Expression<Func<TModel, bool>> expression, Expression<Func<TModel, TType>> field, bool compileSp = false) where TModel : IModel, new()
        {
            return GetFunction<TType, TModel>(expression, field, FunctionType.MAX, compileSp);
        }

        #endregion

        #region SUM
        /// <summary>
        /// 合计
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <param name="field"></param>
        /// <param name="compileSp"></param>
        /// <returns></returns>
        public TType Sum<TType, TModel>(Expression<Func<TModel, bool>> expression, Expression<Func<TModel, TType>> field, bool compileSp = false) where TModel : IModel, new()
        {
            return GetFunction<TType, TModel>(expression, field, FunctionType.SUM, compileSp);
        }
        //public TType Sum<TType, TModel>(Expression<Func<TModel, bool>> expression, string field, bool compileSp = false) where TModel : IModel, new()
        //{
        //    return GetFunction<TType, TModel>(expression, field, FunctionType.SUM, compileSp);
        //}
        #endregion
 
        internal abstract TType GetFunction<TType, TModel>(Expression<Func<TModel, bool>> expression, Expression<Func<TModel, TType>> selectField, FunctionType functionType, bool compileSp = false) where TModel : IModel, new();  
        #endregion

        #region Page
        ///// <summary>
        ///// 返回指定类型分页
        ///// </summary>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //public abstract List<TResult> Page<TResult>(CRL.LambdaQuery.LambdaQueryBase query);

        ///// <summary>
        ///// 返回当前类型分页
        ///// </summary>
        ///// <typeparam name="TModel"></typeparam>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //public abstract List<TModel> Page<TModel>(CRL.LambdaQuery.LambdaQuery<TModel> query)
        //    where TModel : CRL.IModel, new();

        ///// <summary>
        ///// 返回动态对象分页
        ///// </summary>
        ///// <typeparam name="TModel"></typeparam>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //public abstract List<dynamic> PageDynamic<TModel>(CRL.LambdaQuery.LambdaQuery<TModel> query) where TModel : CRL.IModel, new();
        #endregion

        #region QueryDynamic
        /// <summary>
        /// 返回动态对象
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public abstract List<dynamic> QueryDynamic(CRL.LambdaQuery.LambdaQueryBase query);
        ///// <summary>
        ///// 返回自定义对象
        ///// </summary>
        ///// <typeparam name="TModel"></typeparam>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //public abstract List<TResult> QueryDynamic<TModel, TResult>(CRL.LambdaQuery.LambdaQuery<TModel> query)
        //    where TModel : CRL.IModel, new();
        /// <summary>
        /// 返回指定对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public abstract List<TResult> QueryResult<TResult>(CRL.LambdaQuery.LambdaQueryBase query);
        
        /// <summary>
        /// 按筛选返回匿名对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="newExpression"></param>
        /// <returns></returns>
        public abstract List<TResult> QueryResult<TResult>(LambdaQuery.LambdaQueryBase query, NewExpression newExpression);
        #endregion

        #region query item
        /// <summary>
        /// 按ID查询
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public TModel QueryItem<TModel>(object id) where TModel : CRL.IModel, new()
        {
            var type = typeof(TModel);
            var table = TypeCache.GetTable(type);
            string where = _DBAdapter.KeyWordFormat(table.PrimaryKey.MapingName) + "=@par1";
            AddParam("par1", id);
            var query = CreateLambdaQuery<TModel>();
            query.Where(where);
            return QueryList(query).FirstOrDefault();
            //var expression = Base.GetQueryIdExpression<TModel>(id);
            //return QueryItem<TModel>(expression);
        }
        /// <summary>
        /// 查询返回单个结果
        /// 如果只查询ID,调用QueryItem(id)
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <param name="idDest">是否按主键倒序</param>
        /// <param name="compileSp">是否编译成存储过程</param>
        /// <returns></returns>
        public TModel QueryItem<TModel>(Expression<Func<TModel, bool>> expression, bool idDest = true, bool compileSp = false)where TModel : CRL.IModel, new()
        {
            var query = CreateLambdaQuery<TModel>();
            query.Top(1);
            query.Where(expression);
            query.CompileToSp(compileSp);
            query.OrderByPrimaryKey(idDest);
            var result = QueryList<TModel>(query);
            return result.FirstOrDefault();
        }
        #endregion

        #region QueryList
        /// <summary>
        /// 使用lamada设置条件查询
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <param name="compileSp">是否编译成储过程</param>
        /// <returns></returns>
        public List<TModel> QueryList<TModel>(Expression<Func<TModel, bool>> expression = null, bool compileSp = false) where TModel : CRL.IModel, new()
        {
            var query = CreateLambdaQuery<TModel>();
            query.Where(expression);
            query.CompileToSp(compileSp);
            return QueryList<TModel>(query);
        }


        /// <summary>
        /// 使用完整的LamadaQuery查询
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<TModel> QueryList<TModel>(LambdaQuery.LambdaQuery<TModel> query) where TModel : CRL.IModel, new()
        {
            string key;
            return QueryOrFromCache<TModel>(query, out key);
        }
        /// <summary>
        /// 返回多项结果
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public abstract List<TModel> QueryOrFromCache<TModel>(CRL.LambdaQuery.LambdaQuery<TModel> query, out string cacheKey) where TModel : CRL.IModel, new();
        #endregion

        #region 存储过程
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public abstract int Run(string sp);
        /// <summary>
        /// 执行存储过程并返回动态对象
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public abstract List<dynamic> RunDynamicList(string sp);
        /// <summary>
        /// 执行存储过程反回自定义对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sp"></param>
        /// <returns></returns>
        public abstract List<T> RunList<T>(string sp) where T : class, new();
        /// <summary>
        /// 执行存储过程并返回首个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sp"></param>
        /// <returns></returns>
        public abstract T RunObject<T>(string sp) where T : class, new();
        /// <summary>
        /// 执行存储过程返回首行首列
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public abstract object RunScalar(string sp);

        #endregion

        #region Update
        /// <summary>
        /// 关联更新
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TJoin"></typeparam>
        /// <param name="expression"></param>
        /// <param name="updateValue"></param>
        /// <returns></returns>
        public abstract int Update<TModel, TJoin>(System.Linq.Expressions.Expression<Func<TModel, TJoin, bool>> expression, CRL.ParameCollection updateValue)
            where TModel : CRL.IModel, new()
            where TJoin : CRL.IModel, new();
        /// <summary>
        /// 使用完整查询更新
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="updateValue"></param>
        /// <returns></returns>
        public abstract int Update<TModel>(CRL.LambdaQuery.LambdaQuery<TModel> query, CRL.ParameCollection updateValue) where TModel : CRL.IModel, new();
        /// <summary>
        /// 指定条件和参数进行更新
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <param name="setValue"></param>
        /// <returns></returns>
        public int Update<TModel>(System.Linq.Expressions.Expression<Func<TModel, bool>> expression, CRL.ParameCollection setValue) where TModel : CRL.IModel, new()
        {
            var query = CreateLambdaQuery<TModel>();
            query.Where(expression);
            var n= Update(query, setValue);
            System.Threading.Tasks.Task.Run(() =>
            {
                UpdateCacheItem<TModel>(expression, setValue);
            });
            return n;
        }

        /// <summary>
        /// 指定条件并按对象差异更新
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update<TModel>(Expression<Func<TModel, bool>> expression, TModel model) where TModel : CRL.IModel, new()
        {
            var c = GetUpdateField(model);
            if (c.Count == 0)
            {
                return 0;
                //throw new CRLException("更新集合为空");
            }
            var n = Update(expression, c);
            model.CleanChanges();
            return n;
        }

        /// <summary>
        /// 按匿名对象更新
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <param name="updateValue"></param>
        /// <returns></returns>
        public int Update<TModel>(Expression<Func<TModel, bool>> expression, dynamic updateValue) where TModel : CRL.IModel, new()
        {
            var properties = updateValue.GetType().GetProperties();
            var c = new ParameCollection();
            foreach (var p in properties)
            {
                c.Add(p.Name, p.GetValue(updateValue));
            }
            return Update(expression, c);
        }

        /// <summary>
        /// 按对象差异更新
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract int Update<TModel>(TModel obj) where TModel : CRL.IModel, new();
        #endregion

        /// <summary>
        /// 创建副本
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="list"></param>
        internal void SetOriginClone<TModel>(List<TModel> list) where TModel : IModel, new()
        {
            if (SettingConfig.UsePropertyChange)
            {
                return;
            }
            foreach (var item in list)
            {
                TModel clone = item.Clone() as TModel;
                item.OriginClone = clone;
            }
        }
        internal ParameCollection GetUpdateField<TModel>(TModel obj) where TModel : IModel, new()
        {
            var c = obj.GetUpdateField();
            
            if (c.Count() > 0 && obj.OriginClone != null)//只有克隆过的才进行检查
            {
                CheckData(obj);
            }
            return c;

        }
    }
}
