/**
* CRL 快速开发框架 V4.5
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
        /// <summary>
        /// 表示SUM此字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static T SUM<T>(this T origin) where T : struct
        {
            return origin;
        }
        /// <summary>
        /// 表示SUM此字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static T SUM<T>(this T? origin) where T : struct
        {
            return origin.Value;
        }

        /// <summary>
        /// 表示Sum一个属性二元运算 如 Sum(b=>b.Num*b.Price)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="origin"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static TResult SUM<T, TResult>(this T origin, Expression<Func<T, TResult>> resultSelector) where T : IModel
        {
            return default(TResult);
        }
        /// <summary>
        /// 关联时Sum一个二元运算
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="origin"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static object SUM<T,T2>(this T origin, Expression<Func<T, T2, object>> resultSelector) where T : IModel
            where T2 : IModel
        {
            return default(object);
        }
        /// <summary>
        /// 关联时Sum一个二元运算
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TJoin"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="origin"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static TResult SUM<T, TJoin,TResult>(this LambdaQueryJoin<T, TJoin> origin, Expression<Func<T, TJoin, TResult>> resultSelector) where T : IModel
       where TJoin : IModel
        {
            return default(TResult);
        }
    }
}
