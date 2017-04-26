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

namespace CRL.LambdaQuery
{
    internal class LambdaCreater<T> where T : class, new()
    {
        public Expression CreatePropertyExpression(string propertyName)
        {
            var parameter = CreateParameter();
            return parameter.Property(propertyName);
        }
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
        public Expression<Func<T, bool>> Equal(Expression left, object value)
        {
            var parameter = CreateParameter();
            return left
                    .Equal(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }
        ParameterExpression currentParameter = null;
        /// <summary>
        /// 创建参数
        /// </summary>
        private ParameterExpression CreateParameter()
        {
            if (currentParameter == null)
            {
                //expression.Compile()时
                //只能创建一次,否则提示跨作用域
                currentParameter = Expression.Parameter(typeof(T), "b");
            }
            return currentParameter;
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
        public Expression<Func<T, bool>> NotEqual(Expression left, object value)
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
        public Expression<Func<T, bool>> Greater(Expression left, object value)
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
        public Expression<Func<T, bool>> Less(Expression left, object value)
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
        public Expression<Func<T, bool>> GreaterThan(Expression left, object value)
        {
            var parameter = CreateParameter();
            return left
                    .GreaterThan(value)
                    .ToLambda<Func<T, bool>>(parameter);
        }
        #endregion
        public Expression And(Expression left, object value)
        {
            var parameter = CreateParameter();
            var exp = left
                    .And(left.GetConstant(value));
            return exp;
        }
        public Expression Or(Expression left, object value)
        {
            var parameter = CreateParameter();
            var exp = left
                    .Or(left.GetConstant(value));
            return exp;
        }
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
        public Expression<Func<T, bool>> LessThan(Expression left, object value)
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
        public Expression<Func<T, bool>> IsNullOrEmpty(string propertyName, params object[] value)
        {
            var parameter = CreateParameter();
            var property = parameter.Property(propertyName);
            var call = Expression.Call(property.Type.GetMethod("IsNullOrEmpty", new Type[] { typeof(string) }), property);
            return call.ToLambda<Func<T, bool>>(parameter);
        }

        public Expression<Func<T, bool>> Equals(string propertyName, params object[] value)
        {
            var parameter = CreateParameter();
            var property = parameter.Property(propertyName);
            var type2 = value[0].GetType();
            var call = Expression.Call(property, property.Type.GetMethod("Equals", new Type[] { type2 }),
                Expression.Constant(value[0], type2));
            return call.ToLambda<Func<T, bool>>(parameter);
        }
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
        #region 转换方法
        public MethodCallExpression ToString(string propertyName, params object[] value)
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
            var method = property.Type.GetMethod("ToString", argsTypes.ToArray());
            var call = Expression.Call(property, method,
                 args.ToArray());
            return call;
        }
        public MethodCallExpression ToUpper(string propertyName, params object[] value)
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
            var method = property.Type.GetMethod("ToUpper", argsTypes.ToArray());
            var call = Expression.Call(property, method,
                 args.ToArray());
            return call;
        }
        public MethodCallExpression ToLower(string propertyName, params object[] value)
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
            var method = property.Type.GetMethod("ToLower", argsTypes.ToArray());
            var call = Expression.Call(property, method,
                 args.ToArray());
            return call;
        }
        public MethodCallExpression ToInt32(string propertyName, params object[] value)
        {
            var call = CreateConvert("ToInt32", propertyName, value);
            return call;
        }
        public MethodCallExpression ToDecimal(string propertyName, params object[] value)
        {
            var call = CreateConvert("ToDecimal", propertyName, value);
            return call;
        }
        public MethodCallExpression ToDouble(string propertyName, params object[] value)
        {
            var call = CreateConvert("ToDouble", propertyName, value);
            return call;
        }
        public MethodCallExpression ToBoolean(string propertyName, params object[] value)
        {
            var call = CreateConvert("ToBoolean", propertyName, value);
            return call;
        }
        public MethodCallExpression ToDateTime(string propertyName, params object[] value)
        {
            var call = CreateConvert("ToDateTime", propertyName, value);
            return call;
        }
        public MethodCallExpression ToInt16(string propertyName, params object[] value)
        {
            var call = CreateConvert("ToInt16", propertyName, value);
            return call;
        }
        public MethodCallExpression ToSingle(string propertyName, params object[] value)
        {
            var call = CreateConvert("ToSingle", propertyName, value);
            return call;
        }
        MethodCallExpression CreateConvert(string methodName,string propertyName, params object[] value)
        {
            var parameter = CreateParameter();
            var property = parameter.Property(propertyName);
            var method = typeof(Convert).GetMethod(methodName, new Type[] { property.Type });
            var call = Expression.Call(method,
                 property);
            return call;
        }
        #endregion
    }
}
