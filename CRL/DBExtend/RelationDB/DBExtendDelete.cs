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
using CRL.LambdaQuery;
namespace CRL.DBExtend.RelationDB
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
        internal int Delete<TModel>(string where)where TModel:IModel,new()
        {
            CheckTableCreated<TModel>();
            string table = TypeCache.GetTableName(typeof(TModel),dbContext);
            string sql = _DBAdapter.GetDeleteSql(table, where);
            sql = _DBAdapter.SqlFormat(sql);
            int n = __DbHelper.Execute(sql);
            ClearParame();
            return n;
        }
        /// <summary>
        /// 按主键删除
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public override int Delete<TModel>(object id)
        {
            var type = typeof(TModel);
            var table = TypeCache.GetTable(type);
            string where = _DBAdapter.KeyWordFormat(table.PrimaryKey.MapingName) + "=@par1";
            AddParam("par1", id);
            return Delete<TModel>(where);

            //var expression = Base.GetQueryIdExpression<TModel>(id);
            //return Delete<TModel>(expression);
        }
        /// <summary>
        /// 指定条件删除
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override int Delete<TModel>(Expression<Func<TModel, bool>> expression)
        {
            LambdaQuery<TModel> query = new RelationLambdaQuery<TModel>(dbContext, false);
            query.Where(expression);
            string condition = query.FormatExpression(expression.Body).SqlOut;
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
        public override int Delete<TModel, TJoin>(Expression<Func<TModel, TJoin, bool>> expression)
        {
            var query = new RelationLambdaQuery<TModel>(dbContext);
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
        public override int Delete<T>(LambdaQuery<T> query)
        {
            var query1 = query as RelationLambdaQuery<T>;
            if (query1.__GroupFields.Count > 0)
            {
                throw new CRLException("delete不支持group查询");
            }
            if (query1.__Relations.Count > 1)
            {
                throw new CRLException("delete关联不支持多次");
            }
            query1._IsRelationUpdate = true;
            var conditions = query1.GetQueryConditions(false).Trim();
            conditions = conditions.Substring(5);
            string table = query1.QueryTableName;
            table = query1.__DBAdapter.KeyWordFormat(table);
            query1.FillParames(this);
            if (query1.__Relations.Count > 0)
            {
                var kv = query1.__Relations.First();
                var t1 = query1.QueryTableName;
                var t2 = TypeCache.GetTableName(kv.Key.OriginType, query1.__DbContext);
                var join = kv.Value;
                join = join.Substring(join.IndexOf(" on ") + 3);
                if (!string.IsNullOrEmpty(conditions))
                {
                    join += " and ";
                }
                string sql = query1.__DBAdapter.GetRelationDeleteSql(t1, t2, join + conditions);
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
