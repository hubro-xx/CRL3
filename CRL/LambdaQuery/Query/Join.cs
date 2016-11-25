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
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using CRL.LambdaQuery;
using System.Text.RegularExpressions;
namespace CRL.LambdaQuery
{
    public abstract partial class LambdaQuery<T> : LambdaQueryBase where T : IModel, new()
    {
        
        /// <summary>
        /// LeftJoin查询分支
        /// </summary>
        /// <typeparam name="TJoin"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQueryJoin<T, TJoin> LeftJoin<TJoin>(Expression<Func<T, TJoin, bool>> expression) where TJoin : IModel, new()
        {
            return Join(expression, JoinType.Left);
        }
        /// <summary>
        /// RightJoin查询分支
        /// </summary>
        /// <typeparam name="TJoin"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQueryJoin<T, TJoin> RightJoin<TJoin>(Expression<Func<T, TJoin, bool>> expression) where TJoin : IModel, new()
        {
            return Join(expression, JoinType.Right);
        }
        /// <summary>
        /// 创建一个JOIN查询分支
        /// </summary>
        /// <typeparam name="TJoin">关联类型</typeparam>
        /// <returns></returns>
        public LambdaQueryJoin<T, TJoin> Join<TJoin>(Expression<Func<T, TJoin, bool>> expression,JoinType joinType = JoinType.Inner) where TJoin : IModel, new()
        {
            var query2 = new LambdaQueryJoin<T, TJoin>(this);
            var innerType = typeof(TJoin);
            //__JoinTypes.Add(new TypeQuery(innerType), joinType);
            //firstJoinType = innerType;
            GetPrefix(innerType);
            string condition = FormatJoinExpression(expression.Body);
            AddInnerRelation(new TypeQuery(innerType), joinType, condition);
            return query2;
        }
        /// <summary>
        /// 创建关联一个强类型查询
        /// </summary>
        /// <typeparam name="TJoinResult"></typeparam>
        /// <param name="resultSelect"></param>
        /// <param name="expression"></param>
        /// <param name="joinType"></param>
        /// <returns></returns>
        public LambdaQueryViewJoin<T, TJoinResult> Join<TJoinResult>(LambdaQueryResultSelect<TJoinResult> resultSelect, Expression<Func<T, TJoinResult, bool>> expression, JoinType joinType = JoinType.Inner) 
        {
            if(!resultSelect.BaseQuery.__FromDbContext)
            {
                throw new CRLException("关联需要由LambdaQuery.CreateQuery创建");
            }
            var query2 = new LambdaQueryViewJoin<T, TJoinResult>(this, resultSelect);
            //var innerType = typeof(TSource);
            var innerType = resultSelect.InnerType;
            //__JoinTypes.Add(new TypeQuery(innerType, "T_" + prefixIndex), joinType);
            var prefix1 = GetPrefix(innerType);
            var prefix2 = GetPrefix(typeof(TJoinResult));
            var typeQuery = new TypeQuery(innerType, prefix2);
            var baseQuery = resultSelect.BaseQuery;
            foreach (var kv in baseQuery.QueryParames)
            {
                QueryParames[kv.Key] = kv.Value;
            }
            string innerQuery = baseQuery.GetQuery();
            typeQuery.InnerQuery = innerQuery;
            string condition = FormatJoinExpression(expression.Body);
            AddInnerRelation(typeQuery, joinType, condition);
            return query2;
        }
    }
}
