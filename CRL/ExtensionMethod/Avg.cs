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
        /// <summary>
        /// 表示Avg此字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static T AVG<T>(this T origin) where T : struct
        {
            return default(T);
        }
        /// <summary>
        /// 表示Avg此字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static T AVG<T>(this Nullable<T> origin) where T : struct
        {
            return default(T);
        }
        /// <summary>
        /// 表示Avg一个属性二元运算 如 AVG(b=>b.Num*b.Price)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="origin"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static TResult AVG<T, TResult>(this T origin, Expression<Func<T, TResult>> resultSelector) where T : IModel
        {
            return default(TResult);
        }
    }
}
