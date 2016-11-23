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
using CRL.LambdaQuery;
namespace CRL
{
    /// <summary>
    /// 可拼接的Lambda表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExpressionJoin<T> where T : class
    {
        IEnumerable<T> data;
        /// <summary>
        /// 列表筛选
        /// </summary>
        /// <param name="_data"></param>
        /// <param name="expr1"></param>
        public ExpressionJoin(IEnumerable<T> _data, Expression<Func<T, bool>> expr1 = null)
        {
            data = _data;
            currentExpression = expr1;
        }
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
        /// 当前条件And
        /// </summary>
        /// <param name="expr2"></param>
        public void And(Expression<Func<T, bool>> expr2)
        {
            if (currentExpression != null)
            {
                var exp = currentExpression.And(expr2);
                currentExpression = exp;
            }
            else
            {
                currentExpression = expr2;
            }
        }
        /// <summary>
        /// 当前条件Or
        /// </summary>
        /// <param name="expr2"></param>
        public void Or(Expression<Func<T, bool>> expr2)
        {
            if (currentExpression != null)
            {
                var exp = currentExpression.Or(expr2);
                currentExpression = exp;
            }
            else
            {
                currentExpression = expr2;
            }
        }
    
        /// <summary>
        /// 动态排序
        /// </summary>
        /// <param name="sortName"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public IEnumerable<T> OrderBy(string sortName, bool desc)
        {
            if (data == null || data.Count() == 0)
                return data;
            Type type =typeof(T);
            PropertyInfo propertyInfo = type.GetProperty(sortName);
            if (propertyInfo == null)
                throw new CRLException("找不到属性" + sortName);
            ParameterExpression parameter = Expression.Parameter(type, "");
            Expression body = Expression.Property(parameter, propertyInfo);
            Expression sourceExpression = data.AsQueryable().Expression;
            Type sourcePropertyType = propertyInfo.PropertyType;
            Expression lambda = Expression.Call(typeof(Queryable),
                desc ? "OrderByDescending" : "OrderBy",
                new Type[] { type, sourcePropertyType }
                , sourceExpression, Expression.Lambda(body, parameter)
                );
            return data.AsQueryable().Provider.CreateQuery<T>(lambda);
        }

        /// <summary>
        /// 动态排序
        /// </summary>
        /// <param name="resultSelector"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public IEnumerable<T> OrderBy<TKey>(Expression<Func<T, TKey>> resultSelector, bool desc)
        {
            if (data == null || data.Count() == 0)
                return data;
            MemberExpression mExp = (MemberExpression)resultSelector.Body;
            var type=typeof(T);

            Expression sourceExpression = data.AsQueryable().Expression;
            Type sourcePropertyType = mExp.Type;

            Expression lambda = Expression.Call(typeof(Queryable),
                desc ? "OrderByDescending" : "OrderBy",
                new Type[] { type, sourcePropertyType }
                , sourceExpression, resultSelector
                );
            return data.AsQueryable().Provider.CreateQuery<T>(lambda);
        }
        /// <summary>
        /// 查询出结果
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> ToList()
        {
            if (currentExpression == null)
            {
                return data;
            }
            var result = data.Where(currentExpression.Compile()).ToList();
            return result;
        }
    }
}
