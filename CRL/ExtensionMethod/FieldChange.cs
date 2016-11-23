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
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;
using CRL.LambdaQuery;
namespace CRL
{
    public static partial class ExtensionMethod
    {
        #region 手动更改值,以代替ParameCollection
        /// <summary>
        /// 用==表示值被更改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="expression"></param>
        public static void Change<T>(this T obj, Expression<Func<T, bool>> expression) where T : CRL.IModel, new()
        {
            if (expression.Body is BinaryExpression)
            {
                var Reflection = ReflectionHelper.GetInfo<T>();
                BinaryExpression be = ((BinaryExpression)expression.Body);
                MemberExpression mExp = (MemberExpression)be.Left;
                string name = mExp.Member.Name;
                var right = be.Right;
                object value;
                if (right is ConstantExpression)
                {
                    ConstantExpression cExp = (ConstantExpression)right;
                    value = cExp.Value;
                }
                else
                {
                    value = ConstantValueVisitor.GetParameExpressionValue(right);
                    //value = Expression.Lambda(right).Compile().DynamicInvoke();
                }
                //更改对象值
                var pro = TypeCache.GetProperties(typeof(T), true);
                var field = pro[name];
                //field.TupleSetValue<T>(obj, value);
                Reflection.GetAccessor(field.MemberName).Set((T)obj, value);
                obj.SetChanges(name, value);
            }
            else
            {
                obj.Change<T, bool>(expression);
            }
        }
        /// <summary>
        /// 表示值被更改了
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="obj"></param>
        /// <param name="expression"></param>
        public static void Change<T, TKey>(this T obj, Expression<Func<T, TKey>> expression) where T : CRL.IModel, new()
        {
            MemberExpression mExp = (MemberExpression)expression.Body;
            string name = mExp.Member.Name;
            var field = TypeCache.GetProperties(typeof(T), true)[name];
            object value = field.GetValue(obj);
            obj.SetChanges(name, value);
        }
        /// <summary>
        /// 传参表示值被更改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="obj"></param>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        public static void Change<T, TKey>(this T obj, Expression<Func<T, TKey>> expression, TKey value) where T : CRL.IModel, new()
        {
            obj.CheckNull(typeof(T));
            MemberExpression mExp = (MemberExpression)expression.Body;
            string name = mExp.Member.Name;
            //更改对象值
            var pro = TypeCache.GetProperties(typeof(T), true);
            var field = pro[name];
            var Reflection = ReflectionHelper.GetInfo<T>();
            //field.TupleSetValue<T>(obj, value);
            Reflection.GetAccessor(field.MemberName).Set((T)obj, value);
            obj.SetChanges(name, value);
        }
        #region 表示按值累加
        /// <summary>
        /// 表示按值累加,等效为 name=name+'111'
        /// 或int类型 num=num+1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        public static void Cumulation<T>(this T obj, Expression<Func<T, float>> expression, float value) where T : CRL.IModel, new()
        {
            CumulationFun(obj, expression, value);
        }
        /// <summary>
        /// 表示按值累加,等效为 name=name+'111'
        /// 或int类型 num=num+1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        public static void Cumulation<T>(this T obj, Expression<Func<T, double>> expression, double value) where T : CRL.IModel, new()
        {
            CumulationFun(obj, expression, value);
        }
        /// <summary>
        /// 表示按值累加,等效为 name=name+'111'
        /// 或int类型 num=num+1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        public static void Cumulation<T>(this T obj, Expression<Func<T, decimal>> expression, decimal value) where T : CRL.IModel, new()
        {
            CumulationFun(obj, expression, value);
        }
        /// <summary>
        /// 表示按值累加,等效为 name=name+'111'
        /// 或int类型 num=num+1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        public static void Cumulation<T>(this T obj, Expression<Func<T, int>> expression, int value) where T : CRL.IModel, new()
        {
            CumulationFun(obj, expression, value);
        }
        /// <summary>
        /// 表示按值累加,等效为 name=name+'111'
        /// 或int类型 num=num+1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        public static void Cumulation<T>(this T obj, Expression<Func<T, string>> expression, string value) where T : CRL.IModel, new()
        {
            CumulationFun(obj, expression, value);
        }
        static void CumulationFun<T, TKey>(T obj, Expression<Func<T, TKey>> expression, TKey value) where T : CRL.IModel, new()
        {
            MemberExpression mExp = (MemberExpression)expression.Body;
            string name = mExp.Member.Name;
            //更改对象值
            var pro = TypeCache.GetProperties(typeof(T), true);
            var field = pro[name];
            dynamic origin = field.GetValue(obj);
            origin += value;
            var Reflection = ReflectionHelper.GetInfo<T>();
            //field.TupleSetValue<T>(obj, origin);
            Reflection.GetAccessor(field.MemberName).Set((T)obj, origin);
            obj.SetChanges("$" + field.MemberName, value);
        }
        #endregion
        #endregion
    }
}
