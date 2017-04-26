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
    
    /// <summary>
    /// 查询扩展方法,请引用CRL命名空间
    /// </summary>
    public static partial class ExtensionMethod
    {
        /// <summary>
        /// 字符串格式化
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(this string source, params object[] args)
        {
            return string.Format(source, args);
        }
        /// <summary>
        /// 对集合进行分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IEnumerable<T> Page<T>(this IEnumerable<T> source, int index, int pageSize) where T : class, new()
        {
            return source.Skip((index - 1) * pageSize).Take(pageSize);
        }
        #region 查询方法
        /// <summary>
        /// Like("%key%")
        /// 如果包函%通配符,则按通配符算
        /// </summary>
        /// <param name="s"></param>
        /// <param name="likeString"></param>
        /// <returns></returns>
        public static bool Like(this string s, string likeString)
        {
            if (string.IsNullOrEmpty(likeString))
                throw new CRLException("参数值不能为空:likeString");
            return s.IndexOf(likeString) > -1;
        }
        /// <summary>
        /// Like("%key")
        /// </summary>
        /// <param name="s"></param>
        /// <param name="likeString"></param>
        /// <returns></returns>
        public static bool LikeLeft(this string s, string likeString)
        {
            if (string.IsNullOrEmpty(likeString))
                throw new CRLException("参数值不能为空:likeString");
            return s.IndexOf(likeString) > -1;
        }
        /// <summary>
        /// Like("key%")
        /// </summary>
        /// <param name="s"></param>
        /// <param name="likeString"></param>
        /// <returns></returns>
        public static bool LikeRight(this string s, string likeString)
        {
            if (string.IsNullOrEmpty(likeString))
                throw new CRLException("参数值不能为空:likeString");
            return s.IndexOf(likeString) > -1;
        }

        /// <summary>
        /// 按索引和长度检索字符串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Substring(this String s, int index, int length)
        {
            return "";
        }

        /// <summary>
        /// 表示COUNT
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static int COUNT(this object origin)
        {
            return 0;
        }
        #endregion

        #region IEnumerable Find
        /// <summary>
        /// Find
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T Find<T>(this IEnumerable<T> source,Func<T, bool> predicate)
        {
            return source.Where(predicate).FirstOrDefault();
        }
        /// <summary>
        /// FindAll
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<T> FindAll<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.Where(predicate).ToList();
        }
        #endregion

        /// <summary>
        /// 判断类型是否为集合类型
        /// </summary>
        /// <param name="type">要处理的类型</param>
        /// <returns>是返回True，不是返回False</returns>
        public static bool IsEnumerable(this Type type)
        {
            if (type == typeof(string))
            {
                return false;
            }
            return typeof(IEnumerable).IsAssignableFrom(type);
        }
        /// <summary>
        /// 判断值为空
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        public static void CheckNull(this object obj, object name)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(name.ToString());
            }
        }
    }

    
}
