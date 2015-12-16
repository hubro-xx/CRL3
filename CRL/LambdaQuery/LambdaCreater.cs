using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CRL.LambdaQuery
{
    internal class LambdaCreater<T> where T : class, new()
    {
        #region Equal(等于表达式)

        /// <summary>
        /// 创建等于运算lambda表达式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public Expression<Func<T, bool>> Equal(string propertyName, object value)
        {
            var parameter = CreateParameter();
            return parameter.Property(propertyName)
                    .Equal(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }
        public Expression<Func<T, bool>> Equal(MethodCallExpression left, object value)
        {
            var parameter = CreateParameter();
            return left
                    .Equal(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }
        /// <summary>
        /// 创建参数
        /// </summary>
        private ParameterExpression CreateParameter()
        {
            return Expression.Parameter(typeof(T), "b");
        }

        #endregion

        #region NotEqual(不等于表达式)

        /// <summary>
        /// 创建不等于运算lambda表达式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public Expression<Func<T, bool>> NotEqual(string propertyName, object value)
        {
            var parameter = CreateParameter();
            return parameter.Property(propertyName)
                    .NotEqual(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }
        public Expression<Func<T, bool>> NotEqual(MethodCallExpression left, object value)
        {
            var parameter = CreateParameter();
            return left
                    .NotEqual(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }
        #endregion

        #region Greater(大于表达式)

        /// <summary>
        /// 创建大于运算lambda表达式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public Expression<Func<T, bool>> Greater(string propertyName, object value)
        {
            var parameter = CreateParameter();
            return parameter.Property(propertyName)
                    .Greater(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }
        public Expression<Func<T, bool>> Greater(MethodCallExpression left, object value)
        {
            var parameter = CreateParameter();
            return left
                    .Greater(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }
        #endregion

        #region Less(小于表达式)

        /// <summary>
        /// 创建小于运算lambda表达式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public Expression<Func<T, bool>> Less(string propertyName, object value)
        {
            var parameter = CreateParameter();
            return parameter.Property(propertyName)
                    .Less(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }
        public Expression<Func<T, bool>> Less(MethodCallExpression left, object value)
        {
            var parameter = CreateParameter();
            return left
                    .Less(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }
        #endregion

        #region GreaterEqual(大于等于表达式)

        /// <summary>
        /// 创建大于等于运算lambda表达式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public Expression<Func<T, bool>> GreaterThan(string propertyName, object value)
        {
            var parameter = CreateParameter();
            return parameter.Property(propertyName)
                    .GreaterThan(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }
        public Expression<Func<T, bool>> GreaterThan(MethodCallExpression left, object value)
        {
            var parameter = CreateParameter();
            return left
                    .GreaterThan(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }
        #endregion

        #region LessEqual(小于等于表达式)

        /// <summary>
        /// 创建小于等于运算lambda表达式
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public Expression<Func<T, bool>> LessThan(string propertyName, object value)
        {
            var parameter = CreateParameter();
            return parameter.Property(propertyName)
                    .LessThan(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }
        public Expression<Func<T, bool>> LessThan(MethodCallExpression left, object value)
        {
            var parameter = CreateParameter();
            return left
                    .LessThan(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }
        #endregion

        #region Contains(调用Contains方法)

        /// <summary>
        /// 调用Contains方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public Expression<Func<T, bool>> Contains(string propertyName, params object[] value)
        {
            return Call(propertyName, "Contains", value[0]);
        }

        /// <summary>
        /// 调用方法
        /// </summary>
        private Expression<Func<T, bool>> Call(string propertyName, string methodName, object value)
        {
            var parameter = CreateParameter();
            return parameter.Property(propertyName)
                .Call(methodName, value)
                .ToLambda<Func<T, bool>>(parameter);
        }

        #endregion

        #region Starts(调用StartsWith方法)

        /// <summary>
        /// 调用StartsWith方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public Expression<Func<T, bool>> StartsWith(string propertyName, params object[] value)
        {
            var parameter = CreateParameter();
            var property = parameter.Property(propertyName);
            var call = Expression.Call(property, property.Type.GetMethod("StartsWith", new Type[] { typeof(string) }),
                Expression.Constant(value[0]));
            return call.ToLambda<Func<T, bool>>(parameter);
        }

        #endregion

        #region Ends(调用EndsWith方法)

        /// <summary>
        /// 调用EndsWith方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        public Expression<Func<T, bool>> EndsWith(string propertyName, params object[] value)
        {
            var parameter = CreateParameter();
            var property = parameter.Property(propertyName);
            var call = Expression.Call(property, property.Type.GetMethod("EndsWith", new Type[] { typeof(string) }),
                Expression.Constant(value[0]));
            return call.ToLambda<Func<T, bool>>(parameter);
        }

        #endregion
        public MethodCallExpression Substring(string propertyName, params object[] value)
        {
            var parameter = CreateParameter();
            var property = parameter.Property(propertyName);
            var args = new List<Expression>();
            var argsTypes = new List<Type>();
            foreach(var v in value)
            {
                args.Add(Expression.Constant(Convert.ToInt32(v)));
                argsTypes.Add(typeof(int));
            }

            var method = property.Type.GetMethod("Substring", argsTypes.ToArray());
            var call = Expression.Call(property, method,
                 args.ToArray());
            return call;
        }
        public MethodCallExpression IndexOf(string propertyName, params object[] value)
        {
            var parameter = CreateParameter();
            var property = parameter.Property(propertyName);
            var args = new List<Expression>();
            var argsTypes = new List<Type>();
            foreach (var v in value)
            {
                args.Add(Expression.Constant(v));
                argsTypes.Add(v.GetType());
            }
            var method = property.Type.GetMethod("IndexOf", argsTypes.ToArray());
            var call = Expression.Call(property, method,
                 args.ToArray());
            return call;
        }
    }
}
