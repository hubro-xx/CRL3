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
        #region 对象转换
        static Dictionary<Type, Dictionary<string, PropertyInfo>> objProperty = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        static Dictionary<string, PropertyInfo> GetObjProperty(Type type)
        {
            if (objProperty.ContainsKey(type))
            {
                return objProperty[type];
            }
            var destTypes = type.GetProperties().ToList();
            destTypes.RemoveAll(b => b.SetMethod == null || (b.SetMethod != null && b.SetMethod.Name == "set_Item"));
            var dic = destTypes.ToDictionary(b => b.Name.ToUpper());
            objProperty[type] = dic;
            return dic;
        }
        /// <summary>
        /// 转换共同属性的对象
        /// </summary>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDest ToType<TDest>(this object source)
            where TDest : class, new()
        {
            var destTypes = GetObjProperty(typeof(TDest));
            var sourceTypes = GetObjProperty(source.GetType());
            var obj = ToType(sourceTypes, destTypes, source, typeof(TDest));
            return obj as TDest;
        }
        static object ToType(Dictionary<string, PropertyInfo> sourceTypes, Dictionary<string, PropertyInfo> destTypes, object source, Type toType)
        {
            if (source == null)
                return null;
            object obj;
            //obj = System.Activator.CreateInstance(toType);
            try
            {
                obj = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(toType);
            }
            catch
            {
                throw new CRLException(string.Format("{1}不能转换为{0},请检查属性定义", toType, source.GetType()));
            }
            foreach (var kv in destTypes)
            {
                var key = kv.Key;
                var info = kv.Value;
                PropertyInfo sourceInfo;
                var a = sourceTypes.TryGetValue(key, out sourceInfo);
                if (!a)
                {
                    continue;
                }

                object value;
                var nameSpace = sourceInfo.PropertyType.Namespace;
                if (nameSpace == "System" || sourceInfo.PropertyType.BaseType == typeof(Enum))
                {
                    value = sourceInfo.GetValue(source, null);
                    value = ObjectConvert.ConvertObject(info.PropertyType, value);
                }
                else//如果是class,则再转换一次
                {
                    object value2 = sourceInfo.GetValue(source, null);
                    if (value2 == null)
                    {
                        continue;
                    }
                    var sourceTypes2 = GetObjProperty(sourceInfo.PropertyType);
                    var destTypes2 = GetObjProperty(info.PropertyType);
                    value = ToType(sourceTypes2, destTypes2, value2, info.PropertyType);
                }
                info.SetValue(obj, value, null);
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
    where TDest : class, new()
        {
            List<TDest> list = new List<TDest>();
            if (source.Count() == 0)
            {
                return list;
            }
            var destTypes = GetObjProperty(typeof(TDest));
            var sourceTypes = GetObjProperty(source.First().GetType());
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
            where TDest : class, new()
        {
            return ToType<object, TDest>(source, null);
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
        /// 字符串转为int
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int ToInt(this String s)
        {
            return Convert.ToInt32(s);
        }
    }
}
