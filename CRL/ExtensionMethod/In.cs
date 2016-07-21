/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;
using CRL.LambdaQuery;
using System;
namespace CRL
{
    public static partial class ExtensionMethod
    {
        /// <summary>
        /// 表示in
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool In(this string origin, params string[] values)
        {
            return true;
        }
        /// <summary>
        /// 表示in
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="origin"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool In<T>(this T origin, params T[] values) where T : struct
        {
            return true;
        }
        /// <summary>
        /// 表示in
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="origin"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool In<T>(this Nullable<T> origin, params T[] values) where T : struct
        {
            return true;
        }
        
    }
}
