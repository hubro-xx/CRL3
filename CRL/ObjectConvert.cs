/**
* CRL 快速开发框架 V4.5
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using CRL.LambdaQuery.Mapping;
/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CRL
{
    internal class ObjectConvert
    {
        static Dictionary<Type, Func<object, object>> nullCheckMethod = new Dictionary<Type, Func<object, object>>();
        /// <summary>
        /// 转化值,并处理默认值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static object CheckNullValue(object value, Type type = null)
        {
            if (type == null && value == null)
            {
                return DBNull.Value;
                //throw new CRLException("至少一项不能为空");
            }
            if (value != null)
            {
                type = value.GetType();
            }
            if (nullCheckMethod.Count == 0)
            {
                nullCheckMethod.Add(typeof(string), (a) =>
                {
                    return a + "";
                });
                nullCheckMethod.Add(typeof(Enum), (a) =>
                {
                    return Convert.ToInt32(a);
                });
                nullCheckMethod.Add(typeof(DateTime), (a) =>
                {
                    DateTime time = (DateTime)a;
                    if (time.Year == 1)
                    {
                        a = DateTime.Now;
                    }
                    return a;
                });
                nullCheckMethod.Add(typeof(byte[]), (a) =>
                {
                    if (a == null)
                        return 0;
                    return a;
                });
                nullCheckMethod.Add(typeof(Guid), (a) =>
                {
                    if (a == null)
                        return Guid.NewGuid().ToString();
                    return a;
                });

            }
            if (nullCheckMethod.ContainsKey(type))
            {
                return nullCheckMethod[type](value);
            }
            if (type.BaseType == typeof(Enum))
            {
                return nullCheckMethod[type.BaseType](value);
            }
            return value;
        }
        static Dictionary<Type, Func<object, object>> convertMethod = new Dictionary<Type, Func<object, object>>();

        /// <summary>
        /// 转换为为强类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static object ConvertObject(Type type, object value)
        {
            if (value == null)
            {
                return value;
            }
            if (type == value.GetType())
            {
                return value;
            }
            if (convertMethod.Count == 0)
            {
                convertMethod.Add(typeof(byte[]), (a) =>
                {
                    return (byte[])a;
                });
                convertMethod.Add(typeof(Guid), (a) =>
                {
                    return new Guid(a.ToString());
                });
                //convertMethod.Add(typeof(Enum), (a) =>
                //{
                //    return Convert.ToInt32(a);
                //});
            }
            if (type.IsEnum)
            {
                type = type.GetEnumUnderlyingType();
            }
            if (convertMethod.ContainsKey(type))
            {
                return convertMethod[type](value);
            }
            if (type.FullName.StartsWith("System.Nullable"))
            {
                //Nullable<T> 可空属性
                type = type.GenericTypeArguments[0];
            }
            return Convert.ChangeType(value, type);
        }
        
        /// <summary>
        /// 转换为为强类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static T ConvertObject<T>(object obj)
        {
            if (obj == null)
                return default(T);
            if (obj is DBNull)
                return default(T);
            var type = typeof(T);
            return (T)ConvertObject(type, obj);
        }
        class ActionItem<T>
        {
            public Action<T, object> Set;
            public Action<object, object> Set2;
            public string Name;
            public int ValueIndex;
            public Attribute.FieldAttribute FieldAttribute;
            public void SetValue(T item, object[] values)
            {
                Set(item, values[ValueIndex]);
            }
            public void SetValue2(object item, object[] values)
            {
                Set2(item, values[ValueIndex]);
            }
        }

        #region 返回object
        /// <summary>
        /// 仅缓存查询用
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="mainType"></param>
        /// <param name="mapping"></param>
        /// <param name="runTime"></param>
        /// <returns></returns>
        internal static List<object> DataReaderToObjectList(DbDataReader reader, Type mainType, IEnumerable<Attribute.FieldMapping> mapping, out double runTime)
        {
            //rem mainType 不一定为T
            //var sw = new Stopwatch();
            //sw.Start();
            runTime = 0;
            var list = new List<object>();
            if (reader.FieldCount == 0)//分页时不会返回查询
            {
                return list;
            }
            var typeArry = TypeCache.GetTable(mainType).FieldsDic;
            var columns = new Dictionary<string, int>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i).ToLower(), i);
            }
            var actions = new List<ActionItem<object>>();

            object[] values = new object[reader.FieldCount];
            foreach (var mp in mapping)
            {
                var fieldName = mp.ResultName.ToLower();
                Attribute.FieldAttribute info;
                var a = typeArry.TryGetValue(mp.ResultName, out info);
                if (!a)
                {
                    continue;
                }
                var action = new ActionItem<object>() { Set2 = info.SetValue, Name = mp.ResultName, ValueIndex = columns[fieldName], FieldAttribute = info };
                columns.Remove(fieldName);
                actions.Add(action);
            }
            var _actions = actions.ToArray();
            int actionsCount = actions.Count;
            try
            {
                #region while
                while (reader.Read())
                {
                    reader.GetValues(values);
                    //var detailItem = System.Activator.CreateInstance(mainType);
                    var detailItem = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(mainType);
                    foreach (var ac in _actions)
                    {
                        try
                        {
                            ac.SetValue2(detailItem, values);
                        }
                        catch (Exception ero)
                        {
                            var value = reader.GetValue(ac.ValueIndex);
                            reader.Close();
                            reader.Dispose();
                            var columnType = ac.FieldAttribute;
                            throw new CRLException($"反射赋值时发生错误,在:{mainType }  字段:{columnType.MapingName} {value.GetType()} 类型:{columnType.PropertyType },请检查数据库字段类型与对象是否一致");
                        }
                    }
                    #region 剩下的放索引
                    //按IModel算
                    if (columns.Count > 0)
                    {
                        var model = detailItem as IModel;
                        if (model != null)
                        {
                            foreach (var item in columns)
                            {
                                var col = item.Key;
                                var n = col.LastIndexOf("__");
                                if (n == -1)
                                {
                                    continue;
                                }
                                var mapingName = col.Substring(n + 2);
                                model.SetIndexData(mapingName, values[item.Value]);
                            }
                        }
                    }
                    #endregion
                    list.Add(detailItem);

                }
                #endregion
            }
            catch( Exception ero)
            {
                reader.Close();
                reader.Dispose();
                throw new CRLException("转换数据时发生错误:" + ero.Message);
            }
            reader.Close();
            reader.Dispose();
            //sw.Stop();
            //runTime = sw.ElapsedMilliseconds;
            //Console.WriteLine("CRL映射用时:" + runTime);
            return list;
        }
        #endregion
        #region 返回指定类型

        internal static Dictionary<string, Dictionary<string, int>> columnCache = new Dictionary<string, Dictionary<string, int>>();
        internal static Dictionary<string, Dictionary<string, ColumnType>> queryColumnCache = new Dictionary<string, Dictionary<string, ColumnType>>();
        /// <summary>
        /// 返回指定类型,支持强类型和匿名类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        internal static List<T> DataReaderToSpecifiedList<T>(DbDataReader reader, QueryInfo<T> queryInfo)
        {
            var mapping = queryInfo.Mapping;
            var list = new List<T>();
            if (reader.FieldCount == 0)//分页时不会返回查询
            {
                return list;
            }
            string columnCacheKey = queryInfo.selectKey;
            var leftColumns = new Dictionary<string, int>();
            var dicColumns = new Dictionary<string, ColumnType>();
            var a = columnCache.TryGetValue(columnCacheKey, out leftColumns);
            if (!a)
            {
                leftColumns = new Dictionary<string, int>();
                dicColumns= new Dictionary<string, ColumnType>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i).ToLower();
                    var find = mapping.Find(b => b.ResultName.ToLower() == name);
                    Type proType = typeof(string);
                    if (find == null)
                    {
                        leftColumns.Add(name, i);
                    }
                    else
                    {
                        proType = find.PropertyType;
                    }
                    dicColumns.Add(name, new ColumnType() { name = name, index = i, typeName = proType.Name });
                }
                columnCache[columnCacheKey] = leftColumns;
                queryColumnCache[columnCacheKey] = dicColumns;
            }
            dicColumns = queryColumnCache[columnCacheKey];
            queryInfo.CreateObjCreater(dicColumns);
            var objCreater = queryInfo.GetObjCreater();
            int leftColumnCount = leftColumns.Count;
            var type = typeof(T);
            DataContainer dataContainer;
            while (reader.Read())
            {
                dataContainer = new DataContainer(reader, type, dicColumns);
                T detailItem;
                try
                {
                    detailItem = objCreater(dataContainer);
                }
                catch(Exception ero)
                {
                    reader.Close();
                    reader.Dispose();
                    queryInfo = null;
                    var columnType = dataContainer._GetCurrentColumnName();
                    throw new CRLException($"反射赋值时发生错误,在:{type }  字段:{columnType.name} 类型:{columnType.typeName },请检查数据库字段类型与对象是否一致");
                }
                #region 剩下的放索引
                //按IModel算
                if (leftColumnCount > 0)
                {
                    var model = detailItem as IModel;
                    if (model != null)//当不是IModel,不给索引赋值
                    {
                        foreach (var item in leftColumns)
                        {
                            var col = item.Key;
                            var n = col.LastIndexOf("__");
                            if (n == -1)
                            {
                                continue;
                            }
                            var mapingName = col.Substring(n + 2);
                            var val = reader.GetValue(item.Value);
                            model.SetIndexData(mapingName, val);
                        }
                    }
                }
                #endregion
                list.Add(detailItem);
            }
            reader.Close();
            reader.Dispose();
            queryInfo = null;
            return list;
        }
        #endregion
        /// <summary>
        /// DataRead转为字典
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        internal static Dictionary<Tkey, TValue> DataReadToDictionary<Tkey, TValue>(DbDataReader reader)
        {
            var dic = new Dictionary<Tkey, TValue>();
            try
            {
                while (reader.Read())
                {
                    object data1 = reader[0];
                    if (data1 is DBNull)
                    {
                        continue;
                    }
                    object data2 = reader[1];
                    Tkey key = (Tkey)data1;
                    TValue value = (TValue)data2;
                    dic.Add(key, value);
                }
            }
            catch (Exception ero)
            {
                reader.Close();
                throw new CRLException("转换为字典时发生错误" + ero.Message);
            }
            reader.Close();
            return dic;
        }
        /// <summary>
        /// 将集合转换为主键字典
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        internal static Dictionary<string, TItem> ConvertToDictionary<TItem>(IEnumerable list) where TItem : IModel
        {
            var dic = new Dictionary<string, TItem>();
            foreach (var item in list)
            {
                var model = item as TItem;
                var keyValue = model.GetpPrimaryKeyValue().ToString();
                if (!dic.ContainsKey(keyValue))
                {
                    model.FromCache = true;
                    if (SettingConfig.AutoTrackingModel)
                    {
                        model.SetOriginClone();
                    }
                    dic.Add(keyValue, model);
                }
            }
            return dic;
        }
    }
}
