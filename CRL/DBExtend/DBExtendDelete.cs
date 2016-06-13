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
using System.Linq.Expressions;
using System.Text;
using CRL.LambdaQuery;
namespace CRL
{
    public sealed partial class DBExtend
    {
        #region delete
        /// <summary>
        /// 按条件删除
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Delete<TModel>(string where) where TModel : IModel,new()
        {
            CheckTableCreated<TModel>();
            string table = TypeCache.GetTableName(typeof(TModel),dbContext);
            string sql = _DBAdapter.GetDeleteSql(table, where);
            sql = _DBAdapter.SqlFormat(sql);
            int n = dbHelper.Execute(sql);
            ClearParame();
            return n;
        }
        /// <summary>
        /// 按主键删除
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete<TModel>(object id) where TModel : IModel, new()
        {
            var expression = Base.GetQueryIdExpression<TModel>(id);
            return Delete<TModel>(expression);
        }
        /// <summary>
        /// 指定条件删除
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public int Delete<TModel>(Expression<Func<TModel, bool>> expression) where TModel : IModel, new()
        {
            LambdaQuery<TModel> query = new LambdaQuery<TModel>(dbContext, false);
            string condition = query.FormatExpression(expression.Body);
            query.FillParames(this);
            return Delete<TModel>(condition);
        }

        /// <summary>
        /// 关联删除
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TJoin"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public int Delete<TModel, TJoin>(Expression<Func<TModel, TJoin, bool>> expression)
            where TModel : IModel, new()
            where TJoin : IModel, new()
        {
            var query = new LambdaQuery<TModel>(dbContext);
            query.Join<TJoin>(expression);
            return Delete(query);
        }

        /// <summary>
        /// 按完整查询条件进行删除
        /// goup语法不支持,其它支持
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public int Delete<T>(LambdaQuery<T> query) where T : IModel, new()
        {
            if (query.__GroupFields.Count > 0)
            {
                throw new Exception("delete不支持group查询");
            }
            query._IsRelationUpdate = true;
            var conditions = query.GetQueryConditions(false).Trim();
            conditions = conditions.Substring(5);
            string table = query.QueryTableName;
            table = query.__DBAdapter.KeyWordFormat(table);
            query.FillParames(this);
            if (query.__Relations.Count > 0)
            {
                var kv = query.__Relations.First();
                var t1 = query.QueryTableName;
                var t2 = TypeCache.GetTableName(kv.Key, query.__DbContext);
                var join = kv.Value;
                join = join.Substring(join.IndexOf(" on ") + 3);
                string sql = query.__DBAdapter.GetRelationDeleteSql(t1, t2, join + " and " + conditions);
                return Execute(sql);
            }
            return Delete<T>(conditions);
        }

        #endregion

        void DeleteCacheItem<TModel>(string[] ids) where TModel : IModel, new()
        {
            var updateModel = MemoryDataCache.CacheService.GetCacheTypeKey(typeof(TModel));
            foreach (var key in updateModel)
            {
                MemoryDataCache.CacheService.DeleteCacheItem<TModel>(key, ids);
            }
        }
    }
}
