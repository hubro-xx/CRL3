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
        public static int SUM<T>(this T origin) where T:struct
        {
            return 0;
        }
        /// <summary>
        /// 表示SUM此字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static int SUM<T>(this Nullable<T> origin) where T : struct
        {
            return 0;
        }

        /// <summary>
        /// 表示Sum一个属性二元运算 如 Sum(b=>b.Num*b.Price)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="origin"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static int SUM<T, TResult>(this T origin, Expression<Func<T, TResult>> resultSelector) where T : IModel
        {
            return 0;
        }
    }
}
