using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using CRL.LambdaQuery;
using System.Text.RegularExpressions;
namespace CRL.LambdaQuery
{
    public sealed partial class LambdaQuery<T> : LambdaQueryBase where T : IModel, new()
    {
        #region join
        /**
        /// <summary>
        /// Join,并返回筛选值
        /// </summary>
        /// <typeparam name="TJoin">关联类型</typeparam>
        /// <param name="expression">关关表达式</param>
        /// <param name="resultSelector">返回值,如果为空,则返回主表</param>
        /// <param name="joinType">join类型,默认Inner</param>
        /// <returns></returns>
        public LambdaQuery<T> Join<TJoin>(
            Expression<Func<T, TJoin, bool>> expression,
            Expression<Func<T, TJoin, object>> resultSelector = null, JoinType joinType = JoinType.Inner) where TJoin : IModel, new()
        {
            QueryFields.Clear();
            var innerType = typeof(TJoin);
            //TypeCache.SetDBAdapterCache(innerType, dBAdapter);
            string condition = FormatJoinExpression(expression);
            var resultFields = new List<Attribute.FieldAttribute>();
            if (resultSelector != null)
            {
                resultFields = GetSelectField(resultSelector.Body, innerType, false);
                QueryFields = resultFields;
            }
            AddInnerRelation(innerType, condition);
            return this;
        }

        /// <summary>
        /// 存入关联值到对象内部索引
        /// </summary>
        /// <typeparam name="TJoin">关联类型</typeparam>
        /// <param name="expression">关联表达式</param>
        /// <param name="resultSelector">选择的字段 如为null,则不返回</param>
        /// <param name="joinType">join类型,默认Inner</param>
        /// <returns></returns>
        public LambdaQuery<T> AppendJoinValue<TJoin>(
           Expression<Func<T, TJoin, bool>> expression,
           Expression<Func<TJoin, object>> resultSelector = null, JoinType joinType = JoinType.Inner) where TJoin : IModel, new()
        {
            var innerType = typeof(TJoin);
            if (QueryFields.Count == 0)
            {
                SelectAll();
            }
            string condition = FormatJoinExpression(expression);
            var resultFields = new List<Attribute.FieldAttribute>();
            if (resultSelector != null)
            {
                resultFields = GetSelectField(resultSelector.Body, innerType, true);
            }
            QueryFields.AddRange(resultFields);
            AddInnerRelation(innerType, condition);
            return this;
        }
        **/
        #endregion 

        /// <summary>
        /// 创建一个JOIN查询分支
        /// </summary>
        /// <typeparam name="TJoin">关联类型</typeparam>
        /// <returns></returns>
        public LambdaQueryJoin<T, TJoin> Join<TJoin>(Expression<Func<T, TJoin, bool>> expression,JoinType joinType = JoinType.Inner) where TJoin : IModel, new()
        {
            var query2 = new LambdaQueryJoin<T, TJoin>(this);
            var innerType = typeof(TJoin);
            __JoinTypes.Add(innerType, joinType);
            GetPrefix(innerType);
            string condition = FormatJoinExpression(expression.Body);
            AddInnerRelation(innerType, condition);
            return query2;
        }
        ///// <summary>
        ///// 创建一个JOIN查询分支
        ///// </summary>
        ///// <typeparam name="TJoin">关联类型</typeparam>
        ///// <returns></returns>
        //public LambdaQueryJoin2<T, TJoin, TJoin2> Join<TJoin, TJoin2>(Expression<Func<T, TJoin, TJoin2, bool>> expression, JoinType joinType = JoinType.Inner) where TJoin : IModel, new()
        //    where TJoin2 : IModel, new()
        //{
        //    var query2 = new LambdaQueryJoin2<T, TJoin, TJoin2>(this);
        //    var innerType = typeof(TJoin);
        //    JoinTypes.Add(typeof(TJoin), joinType);
        //    JoinTypes.Add(typeof(TJoin2), joinType);
        //    string condition = FormatJoinExpression(expression);
        //    AddInnerRelation(innerType, condition);
        //    return query2;
        //}
    }
}
