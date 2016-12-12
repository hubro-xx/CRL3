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
using System.Threading.Tasks;

namespace CRL.LambdaQuery
{
    /// <summary>
    /// 关联查询分支
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TJoin"></typeparam>
    public sealed class LambdaQueryJoin<T, TJoin>
    {
        /// <summary>
        /// 关联查询分支
        /// </summary>
        /// <param name="query"></param>
        internal LambdaQueryJoin(LambdaQueryBase query)
        {
            BaseQuery = query;
        }
        LambdaQueryBase BaseQuery;

        /// <summary>
        /// 按TJoin追加条件
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQueryJoin<T, TJoin> Where(Expression<Func<TJoin, bool>> expression)
        {
            string condition = BaseQuery.FormatExpression(expression.Body).SqlOut;
            BaseQuery.AddInnerRelationCondition(new TypeQuery(typeof(TJoin)), condition);
            return this;
        }
        /// <summary>
        /// 按关联对象选择查询字段
        /// 可多次调用,不要重复
        /// </summary>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public LambdaQueryJoin<T, TJoin> Select<TResult>(Expression<Func<T, TJoin, TResult>> resultSelector) 
        {
            //在关联两次以上,可调用以下方法指定关联对象获取对应的字段
            var parameters = resultSelector.Parameters.Select(b => b.Type).ToArray();
            var selectFieldItem = BaseQuery.GetSelectField(true, resultSelector.Body, false, parameters);
            //BaseQuery.__QueryFields.AddRange(resultFields);
            selectFieldItem.queryFieldString = "";
            BaseQuery._CurrentSelectFieldCache = selectFieldItem;
            BaseQuery._CurrentAppendSelectField.AddRange(selectFieldItem.fields);
            //BaseQuery.currentSelectFieldCache.queryFieldString += "," + BaseQuery.GetQueryFieldsString(resultFields);
            return this;
        }
        /// <summary>
        /// 返回强类型结果选择
        /// 兼容老写法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public LambdaQueryResultSelect<TResult> SelectV<TResult>(Expression<Func<T, TJoin, TResult>> resultSelector)
        {
            Select(resultSelector);
            return new LambdaQueryResultSelect<TResult>(BaseQuery, resultSelector.Body);
        }

        /// <summary>
        /// 选择TJoin关联值到对象内部索引
        /// 可调用多次,不要重复
        /// </summary>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public LambdaQueryJoin<T, TJoin> SelectAppendValue<TResult>(Expression<Func<TJoin, TResult>> resultSelector)
        {
            //var innerType = typeof(TJoin);
            //if (BaseQuery.__QueryFields.Count == 0)
            //{
            //    BaseQuery.SelectAll();
            //}
            if (BaseQuery._CurrentSelectFieldCache==null)
            {
                BaseQuery.SelectAll();
            }
            var parameters = resultSelector.Parameters.Select(b => b.Type).ToArray();
            var resultFields = BaseQuery.GetSelectField(true, resultSelector.Body, true, parameters).fields;
            //BaseQuery.__QueryFields.AddRange(resultFields);
            BaseQuery._CurrentAppendSelectField.AddRange(resultFields);
            return this;
        }
        /// <summary>
        /// 按关联对象设置GROUP字段
        /// 可多次调用,不要重复
        /// </summary>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public LambdaQueryJoin<T, TJoin> GroupBy<TResult>(Expression<Func<T, TJoin, TResult>> resultSelector)
        {
            //在关联两次以上,可调用以下方法指定关联对象获取对应的字段
            //var innerType = typeof(TJoin);
            var parameters = resultSelector.Parameters.Select(b => b.Type).ToArray();
            var resultFields = BaseQuery.GetSelectField(false, resultSelector.Body, false, parameters).fields;
            BaseQuery.__GroupFields.AddRange(resultFields);
            return this;
        }
        /// <summary>
        /// 按TJoin排序
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public LambdaQueryJoin<T, TJoin> OrderBy<TResult>(Expression<Func<TJoin, TResult>> expression, bool desc = true)
        {
            var parameters = expression.Parameters.Select(b => b.Type).ToArray();
            //var innerType = typeof(TJoin);
            var fields = BaseQuery.GetSelectField(false, expression.Body, false, parameters).fields;
            if (!string.IsNullOrEmpty(BaseQuery.__QueryOrderBy))
            {
                BaseQuery.__QueryOrderBy += ",";
            }
            BaseQuery.__QueryOrderBy += string.Format(" {0} {1}", fields.First().QueryField, desc ? "desc" : "asc");
            return this;
        }
        /// <summary>
        /// 在当前关联基础上再创建关联
        /// </summary>
        /// <typeparam name="TJoin2">再关联的类型</typeparam>
        /// <param name="expression">关联语法</param>
        /// <param name="joinType">关联类型</param>
        /// <returns></returns>
        public LambdaQueryJoin<TJoin, TJoin2> Join<TJoin2>(Expression<Func<TJoin, TJoin2, bool>> expression, JoinType joinType = JoinType.Inner) where TJoin2 : IModel, new()
        {
            //like
            //query.Join<Code.Member>((a, b) => a.UserId == b.Id)
            //    .Select((a, b) => new { a.BarCode, b.Name })
            //    .Join<Code.Order>((a, b) => a.Id == b.Id);
            var query2 = new LambdaQueryJoin<TJoin, TJoin2>(BaseQuery);
            var innerType = typeof(TJoin2);
            //BaseQuery.__JoinTypes.Add(new TypeQuery(innerType), joinType);
            BaseQuery.GetPrefix(innerType);
            string condition = BaseQuery.FormatJoinExpression(expression.Body);
            BaseQuery.AddInnerRelation(new TypeQuery(innerType), joinType, condition);
            return query2;
        }
        /// <summary>
        /// LeftJoin
        /// 在当前关联基础上再创建关联
        /// </summary>
        /// <typeparam name="TJoin2"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQueryJoin<TJoin, TJoin2> LeftJoin<TJoin2>(Expression<Func<TJoin, TJoin2, bool>> expression) where TJoin2 : IModel, new()
        {
            return Join(expression, JoinType.Left);
        }
        /// <summary>
        /// RightJoin
        /// 在当前关联基础上再创建关联
        /// </summary>
        /// <typeparam name="TJoin2"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQueryJoin<TJoin, TJoin2> RightJoin<TJoin2>(Expression<Func<TJoin, TJoin2, bool>> expression) where TJoin2 : IModel, new()
        {
            return Join(expression, JoinType.Right);
        }
    }
}
