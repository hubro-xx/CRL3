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
        /// DateDiff
        /// </summary>
        /// <param name="time"></param>
        /// <param name="format">DatePart</param>
        /// <param name="compareTime">比较的时间</param>
        /// <returns></returns>
        public static int DateDiff(this DateTime time, DatePart format, DateTime compareTime)
        {
            return 1;
        }
        /// <summary>
        /// DateDiff
        /// </summary>
        /// <param name="time"></param>
        /// <param name="format"></param>
        /// <param name="compareTime"></param>
        /// <returns></returns>
        public static int DateDiff(this Nullable<DateTime> time, DatePart format, DateTime compareTime)
        {
            return 1;
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

        #region 对象转换
        /// <summary>
        /// 转换共同属性的对象
        /// </summary>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDest ToType<TDest>(this object source)
            where TDest : class,new()
        {
            var destTypes = typeof(TDest).GetProperties().ToList();
            destTypes.RemoveAll(b => b.SetMethod == null || (b.SetMethod != null && b.SetMethod.Name == "set_Item"));
            List<PropertyInfo> sourceTypes = source.GetType().GetProperties().ToList();
            sourceTypes.RemoveAll(b => b.SetMethod == null || (b.SetMethod != null && b.SetMethod.Name == "set_Item"));
            var obj = ToType(sourceTypes, destTypes,source, typeof(TDest));
            return obj as TDest;
        }
        static object ToType(IEnumerable<PropertyInfo> sourceTypes, IEnumerable<PropertyInfo> destTypes, object source, Type toType)
        {
            if (source == null)
                return null;
            object obj ;
            //obj = System.Activator.CreateInstance(toType);
            try
            {
                obj = System.Activator.CreateInstance(toType);
            }
            catch
            {
                throw new CRLException(string.Format("{1}不能转换为{0},请检查属性定义", toType, source.GetType()));
            }
            foreach (var info in destTypes)
            {
                var sourceInfo = sourceTypes.Find(b => b.Name.ToLower() == info.Name.ToLower());
                if (sourceInfo != null)
                {
                    object value;
                    var nameSpace = sourceInfo.PropertyType.Namespace;
                    if (nameSpace == "System" || sourceInfo.PropertyType.BaseType == typeof(Enum))
                    {
                        value = sourceInfo.GetValue(source, null);
                        value = ObjectConvert.ConvertObject(info.PropertyType, value);
                    }
                    else//如果是class,则再转换一次
                    {
                        object value2 = sourceInfo.GetValue(source,null);
                        if(value2==null)
                        {
                            continue;
                        }
                        var sourceTypes2 = sourceInfo.PropertyType.GetProperties().ToList();
                        sourceTypes2.RemoveAll(b => b.SetMethod == null || (b.SetMethod != null && b.SetMethod.Name == "set_Item"));
                        var destTypes2 = info.PropertyType.GetProperties().ToList();
                        destTypes2.RemoveAll(b => b.SetMethod == null || (b.SetMethod != null && b.SetMethod.Name == "set_Item"));
                        value = ToType(sourceTypes2, destTypes2, value2, info.PropertyType);
                    }
                    info.SetValue(obj, value, null);
                }
            }
            return obj;
        }
        /// <summary>
        /// 转换为共同属性的集合
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <param name="action">指定转换委托再次处理,可为空</param>
        /// <returns></returns>
        public static List<TDest> ToType<TSource, TDest>(this IEnumerable<TSource> source, Action<TSource, TDest> action)
    where TDest : class,new()
        {
            List<TDest> list = new List<TDest>();
            if (source.Count() == 0)
            {
                return list;
            }
            var destTypes = typeof(TDest).GetProperties().ToList();
            destTypes.RemoveAll(b => b.SetMethod == null || (b.SetMethod != null && b.SetMethod.Name == "set_Item"));
            List<PropertyInfo> sourceTypes = source.First().GetType().GetProperties().ToList();
            sourceTypes.RemoveAll(b => b.SetMethod == null || (b.SetMethod != null && b.SetMethod.Name == "set_Item"));

            foreach (var item in source)
            {
                TDest obj = ToType(sourceTypes, destTypes, item, typeof(TDest)) as TDest;
                if (action != null)
                {
                    action(item, obj);
                }
                list.Add(obj);
            }
            return list;
        }
        /// <summary>
        /// 转换为共同属性的集合
        /// </summary>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<TDest> ToType<TDest>(this IEnumerable<object> source)
            where TDest : class,new()
        {
            return ToType<object, TDest>(source, null);
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
        /// 枚举转换为INT
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static int ToInt(this Enum e)
        {
            return Convert.ToInt32(e);
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
        public static int ToInt(this String s)
        {
            return Convert.ToInt32(s);
        }
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

    #region 比较时间格式
    /// <summary>
    /// 比较时间格式
    /// </summary>
    public enum DatePart
    {
        /// <summary>
        /// 年
        /// </summary>
        yy,
        /// <summary>
        /// 季度
        /// </summary>
        qq,
        /// <summary>
        /// 月
        /// </summary>
        mm,
        /// <summary>
        /// 年中的日
        /// </summary>
        dy,
        /// <summary>
        /// 日
        /// </summary>
        dd,
        /// <summary>
        /// 周
        /// </summary>
        ww,
        /// <summary>
        /// 星期
        /// </summary>
        dw,
        /// <summary>
        /// 小时
        /// </summary>
        hh,
        /// <summary>
        /// 分
        /// </summary>
        mi,
        /// <summary>
        /// 秒
        /// </summary>
        ss,
        /// <summary>
        /// 毫秒
        /// </summary>
        ms,
        /// <summary>
        /// 微妙
        /// </summary>
        mcs,
        /// <summary>
        /// 纳秒
        /// </summary>
        ns
    }
    #endregion
}
