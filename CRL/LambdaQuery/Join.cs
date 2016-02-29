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
