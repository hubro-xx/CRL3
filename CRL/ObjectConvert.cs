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
                //throw new Exception("至少一项不能为空");
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
                    return (int)a;
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
            #region old
            if (type.BaseType == typeof(Enum))
            {
                value = (int)value;
            }
            else if (type == typeof(DateTime))
            {
                DateTime time = (DateTime)value;
                if (time.Year == 1)
                {
                    value = DateTime.Now;
                }
            }
            else if (type == typeof(byte[]))
            {
                if (value == null)
                    return 0;
            }
            else if (type == typeof(Guid))
            {
                if (value == null)
                    return Guid.NewGuid().ToString();
            }
            else if (type == typeof(string))
            {
                value = value + "";
            }
            return value;
            #endregion
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
            //if (type.BaseType == typeof(Enum))
            //{
            //    return convertMethod[type.BaseType](value);
            //}
            if (type.FullName.StartsWith("System.Nullable"))
            {
                //Nullable<T> 可空属性
                type = type.GenericTypeArguments[0];
            }
            return Convert.ChangeType(value, type);
            #region 类型转换
            if (type == typeof(Int32))
            {
                value = Convert.ToInt32(value);
            }
            else if (type == typeof(Int16))
            {
                value = Convert.ToInt16(value);
            }
            else if (type == typeof(Int64))
            {
                value = Convert.ToInt64(value);
            }
            else if (type == typeof(DateTime))
            {
                value = Convert.ToDateTime(value);
            }
            else if (type == typeof(Decimal))
            {
                value = Convert.ToDecimal(value);
            }
            else if (type == typeof(Double))
            {
                value = Convert.ToDouble(value);
            }
            else if (type == typeof(System.Byte[]))
            {
                value = (byte[])value;
            }
            else if (type.BaseType == typeof(System.Enum))
            {
                value = Convert.ToInt32(value);
            }
            else if (type == typeof(System.Boolean))
            {
                value = Convert.ToBoolean(value);
            }
            else if (type == typeof(Guid))
            {
                value = new Guid(value.ToString());
            }
            #endregion
            return value;
        }
        #region GetDataReaderValue
        static Dictionary<Type, Func<DbDataReader, int, object>> convertDataReaderMethod = new Dictionary<Type, Func<DbDataReader, int, object>>();
        internal static object GetDataReaderValue(DbDataReader _reader, Type propType, int _index)
        {
            if (propType.IsEnum)
            {
                propType = propType.GetEnumUnderlyingType();
            }
            if (propType.FullName.StartsWith("System.Nullable"))
            {
                //Nullable<T> 可空属性
                propType = propType.GenericTypeArguments[0];
            }
            if (convertDataReaderMethod.Count == 0)
            {
                convertDataReaderMethod.Add(typeof(string), (reader, index) =>
                {
                    return reader.GetString(index);
                });
                convertDataReaderMethod.Add(typeof(int), (reader, index) =>
                {
                    return reader.GetInt32(index);
                });
                convertDataReaderMethod.Add(typeof(DateTime), (reader, index) =>
                {
                    return reader.GetDateTime(index);
                });
                convertDataReaderMethod.Add(typeof(long), (reader, index) =>
                {
                    return reader.GetInt64(index);
                });
                convertDataReaderMethod.Add(typeof(float), (reader, index) =>
                {
                    return reader.GetFloat(index);
                });
                convertDataReaderMethod.Add(typeof(double), (reader, index) =>
                {
                    return reader.GetDouble(index);
                });
                convertDataReaderMethod.Add(typeof(Guid), (reader, index) =>
                {
                    return reader.GetGuid(index);
                });
                convertDataReaderMethod.Add(typeof(short), (reader, index) =>
                {
                    return reader.GetInt16(index);
                });
                convertDataReaderMethod.Add(typeof(byte), (reader, index) =>
                {
                    return reader.GetByte(index);
                });
                convertDataReaderMethod.Add(typeof(char), (reader, index) =>
                {
                    return reader.GetChar(index);
                });
                convertDataReaderMethod.Add(typeof(decimal), (reader, index) =>
                {
                    return reader.GetDecimal(index);
                });
                convertDataReaderMethod.Add(typeof(byte[]), (reader, index) =>
                {
                    return reader.GetValue(index);
                });
                convertDataReaderMethod.Add(typeof(bool), (reader, index) =>
                {
                    return reader.GetBoolean(index);
                });
            }
            Func<DbDataReader, int, object> method;
            var a = convertDataReaderMethod.TryGetValue(propType, out method);
            if (a)
            {
                return method(_reader, _index);
            }
            return _reader.GetValue(_index);
        }
        #endregion
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
       
        internal static List<TItem> DataReaderToList<TItem>(DbDataReader reader, out double runTime, bool setConstraintObj = false) where TItem : class, new()
        {
            var mainType = typeof(TItem);
            return DataReaderToList<TItem>(reader, mainType, out runTime, setConstraintObj);
        }
        internal static List<T> DataReaderToList<T>(DbDataReader reader, Type mainType, out double runTime, bool setConstraintObj = false) where T : class, new()
        {
            //rem mainType 不一定为T
            var sw = new Stopwatch();
            sw.Start();
            var list = new List<T>();
            var typeArry = TypeCache.GetProperties(mainType, !setConstraintObj).Values;
            var columns = new Dictionary<int, string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(i, reader.GetName(i).ToLower());
            }
            var reflection = ReflectionHelper.GetInfo<T>();
            var actions = new List<ActionItem<T>>();
            var first = true;
            var objOrigin = System.Activator.CreateInstance(mainType);
            var canTuple = mainType == typeof(T);
            IModel obj2 = null;
            if (objOrigin is IModel)
            {
                obj2 = objOrigin as IModel;
            }
            while (reader.Read())
            {
                object objInstance = null;
                if (canTuple)
                {
                    objInstance = obj2.Clone();
                    //objInstance = new T();
                    //objInstance = obj2;
                }
                else
                {
                    objInstance = System.Activator.CreateInstance(mainType);
                }
                object[] values = new object[columns.Count];
                reader.GetValues(values);
                var dic = new Dictionary<string, object>();
                for (int i = 0; i < columns.Count; i++)
                {
                    var name = columns[i];
                    dic.Add(name.ToLower(), values[i]);
                }
                var detailItem = DataReaderToObj<T>(dic, reflection, canTuple, objInstance, typeArry, actions, first) as T;
                list.Add(detailItem);
                first = false;
            }
            reader.Close();
            sw.Stop();
            runTime = sw.ElapsedMilliseconds;
            Console.WriteLine("CRL映射用时:"+runTime);
            return list;
        }
        internal struct ActionItem<T>
        {
            public Action<T, object> Action;
            public string Name;
        }
        internal static object DataReaderToObj<T>(Dictionary<string, object> values, ReflectionInfo<T> reflection, bool canTuple, object detailItem, IEnumerable<Attribute.FieldAttribute> typeArry, List<ActionItem<T>> actions, bool first) where T : class,new()
        {
            IModel obj2 = null;
            if (detailItem is IModel)
            {
                obj2 = detailItem as IModel;
                obj2.BoundChange = false;
            }
            if (first)
            {
                #region foreach field
                foreach (Attribute.FieldAttribute info in typeArry)
                {
                    string nameLower = info.MappingName.ToLower();
                    if (info.FieldType == Attribute.FieldType.关联字段)//按外部字段
                    {
                        #region 按外部字段
                        string tab = TypeCache.GetTableName(info.ConstraintType, null);
                        string fieldName = info.GetTableFieldFormat(tab, info.ConstraintResultField).ToLower();
                        if (values.ContainsKey(fieldName))
                        {
                            if (canTuple)
                            {
                                var accessor = reflection.GetAccessor(info.MemberName);
                                actions.Add(new ActionItem<T>() { Action = accessor.Set, Name = fieldName });
                            }
                            else
                            {
                                actions.Add(new ActionItem<T>() { Action = info.SetValue, Name = fieldName });
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region 按属性
                        if (values.ContainsKey(nameLower))
                        {
                            if (canTuple)
                            {
                                var accessor = reflection.GetAccessor(info.MemberName);
                                actions.Add(new ActionItem<T>() { Action = accessor.Set, Name = nameLower });
                            }
                            else
                            {
                                actions.Add(new ActionItem<T>() { Action = info.SetValue, Name = nameLower });
                            }
                        }
                        #endregion
                    }
                }
                #endregion
            }
            foreach (var item in actions)
            {
                //var item = actions[i];
                object value = values[item.Name];
                item.Action((T)detailItem, value);
                values.Remove(item.Name);
            }
            if (obj2 != null && values.Count > 0)
            {
                var b = obj2.BoundChange;
                //没有找到属性的列放入索引,按别名
                foreach (var kv in values)
                {
                    var col = kv.Key;
                    var n = col.LastIndexOf("__");
                    if (n == -1)
                    {
                        continue;
                    }
                    var mapingName = col.Substring(n + 2);
                    obj2[mapingName] = kv.Value;
                }
                obj2.BoundChange = true;
            }
            return detailItem;
        }

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
            while (reader.Read())
            {
                object data1 = reader[0];
                object data2 = reader[1];
                Tkey key = ConvertObject<Tkey>(data1);
                TValue value = ConvertObject<TValue>(data2);
                dic.Add(key, value);
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
                var keyValue = (item as IModel).GetpPrimaryKeyValue().ToString();
                if (!dic.ContainsKey(keyValue))
                {
                    dic.Add(keyValue, item as TItem);
                }
            }
            return dic;
        }
    }
}
