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
using System.Reflection;
using System.Text;

namespace CRL
{
    /// <summary>
    /// 可拼接的Lambda表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExpressionJoin<T> where T : class
    {
        Expression<Func<T, bool>> currentExpression;
        /// <summary>
        /// 获取最终表达式
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, bool>> GetExpression()
        {
            return currentExpression;
        }
        /// <summary>
        /// 可拼接的Lambda表达式
        /// </summary>
        /// <param name="expr1"></param>
        public ExpressionJoin(Expression<Func<T, bool>> expr1=null)
        {
            currentExpression = expr1;
        }
        /// <summary>
        /// 当前条件And
        /// </summary>
        /// <param name="expr2"></param>
        public void And(Expression<Func<T, bool>> expr2)
        {
            if (currentExpression != null)
            {
                var exp = And(currentExpression, expr2);
                currentExpression = exp;
            }
            else
            {
                currentExpression = expr2;
            }
        }
        /// <summary>
        /// 组合And
        /// </summary>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <returns></returns>
        public Expression<Func<T, bool>> And(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            var newExpr = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
            return newExpr as Expression<Func<T, bool>>;
        }
        /// <summary>
        /// 当前条件Or
        /// </summary>
        /// <param name="expr2"></param>
        public void Or(Expression<Func<T, bool>> expr2)
        {
            if (currentExpression != null)
            {
                var exp = Or(currentExpression, expr2);
                currentExpression = exp;
            }
            else
            {
                currentExpression = expr2;
            }
        }
        /// <summary>
        /// 组合Or
        /// </summary>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <returns></returns>
        public Expression<Func<T, bool>> Or(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            var newExpr = Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
            return newExpr as Expression<Func<T, bool>>;
        }
        /// <summary>
        /// 查询出结果
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public IEnumerable<T> Where(IEnumerable<T> list)
        {
            if (currentExpression == null)
            {
                return list;
            }
            var result = list.Where(currentExpression.Compile());
            try
            {
                result.Count();
            }
            catch
            {
                return new List<T>();
            }
            return result;
        }
        /// <summary>
        /// 动态排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="sortName"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public IEnumerable<T> OrderBy(IEnumerable<T> list, string sortName, bool desc)
        {
            if (list == null || list.Count() == 0)
                return list;
            Type type = list.FirstOrDefault().GetType();
            PropertyInfo propertyInfo = type.GetProperty(sortName);
            if (propertyInfo == null)
                throw new CRLException("找不到属性" + sortName);
            ParameterExpression parameter = Expression.Parameter(type, "");
            Expression body = Expression.Property(parameter, propertyInfo);
            Expression sourceExpression = list.AsQueryable().Expression;
            Type sourcePropertyType = propertyInfo.PropertyType;
            Expression lambda = Expression.Call(typeof(Queryable),
                desc ? "OrderByDescending" : "OrderBy",
                new Type[] { type, sourcePropertyType }
                , sourceExpression, Expression.Lambda(body, parameter)
                );
            return list.AsQueryable().Provider.CreateQuery<T>(lambda);
        }

        /// <summary>
        /// 动态排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="resultSelector"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public IEnumerable<T> OrderBy<TKey>(IEnumerable<T> list, Expression<Func<T, TKey>> resultSelector, bool desc)
        {
            if (list == null || list.Count() == 0)
                return list;
            MemberExpression mExp = (MemberExpression)resultSelector.Body;
            var type=typeof(T);

            Expression sourceExpression = list.AsQueryable().Expression;
            Type sourcePropertyType = mExp.Type;

            Expression lambda = Expression.Call(typeof(Queryable),
                desc ? "OrderByDescending" : "OrderBy",
                new Type[] { type, sourcePropertyType }
                , sourceExpression, resultSelector
                );
            return list.AsQueryable().Provider.CreateQuery<T>(lambda);
        }

    }
}
