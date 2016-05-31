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
            var condition = query.FormatJoinExpression(expression.Body);
            condition = query.ReplacePrefix(condition);
            query.FillParames(this);
            var t1 = TypeCache.GetTableName(typeof(TModel), dbContext);
            var t2 = TypeCache.GetTableName(typeof(TJoin), dbContext);
            string sql = _DBAdapter.GetRelationDeleteSql(t1, t2, condition);
            int n = dbHelper.Execute(sql);
            ClearParame();
            return n;
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
