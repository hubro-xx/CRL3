using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Transactions;
using CRL.LambdaQuery;

namespace CRL
{
    /// <summary>
    /// 业务基类
    /// 请实现调用对象Instance
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class BaseProvider<TModel> : ProviderOrigin<TModel>
        where TModel : IModel, new()
    {

        internal override DbContext GetDbContext()
        {
            if (SettingConfig.GetDbAccess == null)
            {
                throw new Exception("请配置CRL数据访问对象,实现CRL.SettingConfig.GetDbAccess");
            }
            var helper = SettingConfig.GetDbAccess(dbLocation);
            var dbContext = new DbContext(helper, dbLocation);
            return dbContext;
        }
        #region 属性
        /// <summary>
        /// 是否从远程查询缓存
        /// </summary>
        protected virtual bool QueryCacheFromRemote
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// 是否启用缓存并行查询(耗CPU,但速度快),默认false
        /// 当数据量大于10W时才会生效
        /// </summary>
        protected virtual bool CacheQueryAsParallel
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region 创建缓存
        /// <summary>
        /// 按类型清除当前所有缓存
        /// </summary>
        public void ClearCache()
        {
            Type type = typeof(TModel);
            if (TypeCache.ModelKeyCache.ContainsKey(type))
            {
                CRL.MemoryDataCache.CacheService.RemoveCache(TypeCache.ModelKeyCache[type]);
                string val;
                TypeCache.ModelKeyCache.TryRemove(type,out val);
            }
        }
        /// <summary>
        /// 缓存默认查询
        /// </summary>
        /// <returns></returns>
        protected virtual LambdaQuery<TModel> CacheQuery()
        {
            return GetLambdaQuery();
        }
        /// <summary>
        /// 获取当前对象缓存,不指定条件
        /// </summary>
        public IEnumerable<TModel> AllCache
        {
            get
            {
                var query = CacheQuery();
                var all = GetCache(query);
                if (all == null)
                {
                    return new List<TModel>();
                }
                return all.Values;
            }
        }
        #region 查询分布式缓存
        #region 客户端
        /// <summary>
        /// 从服务端查询
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="total"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<TModel> QueryFromCacheServer(Expression<Func<TModel, bool>> expression, out int total, int pageIndex = 0, int pageSize = 0)
        {
            var proxy = CacheServerSetting.GetCurrentClient(typeof(TModel));
            if (proxy == null)
            {
                throw new Exception("未在服务器上找到对应的数据处理类型:" + typeof(TModel).FullName);
            }
            var data = proxy.Query(expression, out total, pageIndex, pageSize);
            return data;
        }
        #endregion

        #region 服务端
        /// <summary>
        /// 查询命令处理
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public CacheServer.ResultData DeaCacheCommand(CacheServer.Command command)
        {
            if (command.CommandType == CacheServer.CommandType.查询)
            {
                var expression = LambdaQuery.CRLExpression.CRLQueryExpression.FromJson(command.Data);
                return QueryFromCache(expression);
            }
            else
            {
                //更新缓存
                var item = (TModel)CoreHelper.SerializeHelper.SerializerFromJSON(command.Data, typeof(TModel), Encoding.UTF8);
                var updateModel = MemoryDataCache.CacheService.GetCacheTypeKey(typeof(TModel));
                foreach (var key in updateModel)
                {
                    MemoryDataCache.CacheService.UpdateCacheItem(key, item, null);
                }
                return new CacheServer.ResultData();
            }
        }

        /// <summary>
        /// 使用CRLExpression从缓存中查询
        /// 仅在缓存接口部署
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        CacheServer.ResultData QueryFromCache(LambdaQuery.CRLExpression.CRLQueryExpression expression)
        {
            var _CRLExpression = new CRL.LambdaQuery.CRLExpression.CRLExpressionVisitor<TModel>().CreateLambda(expression.Expression);
            int total;
            var data = QueryFromCacheBase(_CRLExpression, out total, expression.PageIndex, expression.PageSize);
            return new CacheServer.ResultData() { Total = total, JsonData = CoreHelper.StringHelper.SerializerToJson(data) };
        }
        #endregion
        #endregion
        /// <summary>
        /// 从对象缓存中进行查询
        /// 如果QueryCacheFromRemote为true,则从远端查询
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public List<TModel> QueryFromCache(Expression<Func<TModel, bool>> expression)
        {
            int total;
            return QueryFromCache(expression, out total, 0, 0);
        }
        /// <summary>
        /// 从对象缓存中进行查询一项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TModel QueryItemFromCache(object key)
        {
            string id = key.ToString();
            if (QueryCacheFromRemote)
            {
                var expression = Base.GetQueryIdExpression<TModel>(id);
                return QueryItemFromCache(expression);
            }
            else
            {
                var all = GetCache(CacheQuery());
                if (all.ContainsKey(id))
                {
                    return all[id];
                }
                return null;
            }
        }
        /// <summary>
        /// 从对象缓存中进行查询
        /// 如果QueryCacheFromRemote为true,则从远端查询
        /// 返回一项
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public TModel QueryItemFromCache(Expression<Func<TModel, bool>> expression)
        {
            int total;
            int pageIndex = 0;
            int pageSize = 0;
            if (QueryCacheFromRemote)
            {
                pageIndex = 1;
                pageSize = 1;
            }
            var list = QueryFromCache(expression, out total, pageIndex, pageSize);
            if (list.Count == 0)
                return null;
            return list[0];
        }
        /// <summary>
        /// 从对象缓存中进行查询
        /// 如果QueryCacheFromRemote为true,则从远端查询
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="total"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<TModel> QueryFromCache(Expression<Func<TModel, bool>> expression, out int total, int pageIndex = 0, int pageSize = 0)
        {
            total = 0;
            if (QueryCacheFromRemote)
            {
                return QueryFromCacheServer(expression, out total, pageIndex, pageSize);
            }
            return QueryFromCacheBase(expression, out total, pageIndex, pageSize);
        }
        List<TModel> QueryFromCacheBase(Expression<Func<TModel, bool>> expression, out int total, int pageIndex = 0, int pageSize = 0)
        {
            total = 0;
            #region 按KEY查找
            if (expression.Body is BinaryExpression)
            {
                var binary = expression.Body as BinaryExpression;
                if (binary.NodeType == ExpressionType.Equal)
                {
                    if (binary.Left is MemberExpression)
                    {
                        var member = binary.Left as MemberExpression;
                        var primaryKey = TypeCache.GetTable(typeof(TModel)).PrimaryKey.Name;
                        if (member.Member.Name.ToUpper() == primaryKey.ToUpper())
                        {
                            var value = LambdaCompileCache.GetParameExpressionValue(binary.Right).ToString();
                            //var value = (int)Expression.Lambda(binary.Right).Compile().DynamicInvoke();
                            var all = GetCache(CacheQuery());
                            if(all==null)
                            {
                                return new List<TModel>();
                            }
                            if (all.ContainsKey(value))
                            {
                                total = 1;
                                return new List<TModel>() { all[value] };
                            }
                            return new List<TModel>();
                        }
                    }
                }
            }
            #endregion
            var predicate = expression.Compile();
            IEnumerable<TModel> data;
            int cacheTotal = AllCache.Count();
            if (CacheQueryAsParallel && cacheTotal > 100000)
            {
                data = AllCache.AsParallel().Where(predicate);
            }
            else
            {
                data = AllCache.Where(predicate);
            }
            total = data.Count();
            if (pageIndex > 0)
            {
                //var data2 = Base.CutList(data, pageIndex, pageSize);
                var data2 = data.Page(pageIndex, pageSize).ToList();
                return data2;
            }
            return data.ToList();
        }
        /// <summary>
        /// 按类型获取缓存,只能在继承类实现,只能同时有一个类型
        /// 不建议直接调用,请调用AllCache或重写调用
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected Dictionary<string, TModel> GetCache(LambdaQuery<TModel> query)
        {
            Type type = typeof(TModel);
            int expMinute = query.__ExpireMinute;
            if (expMinute == 0)
                expMinute = 5;
            query.__ExpireMinute = expMinute;
            string dataCacheKey;
            var list = new Dictionary<string, TModel>();
            if (!TypeCache.ModelKeyCache.ContainsKey(type))
            {
                var db = GetDbHelper();//避开事务控制,使用新的连接
                var list2 = db.QueryList<TModel>(query, out dataCacheKey);
                list = ObjectConvert.ConvertToDictionary<TModel>(list2);
                lock (lockObj)
                {
                    if (!TypeCache.ModelKeyCache.ContainsKey(type))
                    {
                        TypeCache.ModelKeyCache.TryAdd(type, dataCacheKey);
                    }
                }
            }
            else
            {
                dataCacheKey = TypeCache.ModelKeyCache[type];
                list = MemoryDataCache.CacheService.GetCacheItem<TModel>(dataCacheKey);
            }
            return list;
        }
        #endregion

        #region 存储过程
        /// <summary>
        /// 执行存储过程返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sp"></param>
        /// <returns></returns>
        protected List<T> RunList<T>(string sp) where T : class, new()
        {
            DBExtend db = DBExtend;
            return db.RunList<T>(sp);
        }
        #endregion
    }
}
