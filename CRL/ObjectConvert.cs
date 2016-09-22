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
        internal struct ActionItem<T>
        {
            public Action<T, object> Set;
            public string Name;
            public int ValueIndex;
        }
        #region 返回object
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
            var columns = new Dictionary<string, int>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i).ToLower(), i);
            }
            var leftColumns = new Dictionary<string, int>(columns);
            var reflection = ReflectionHelper.GetInfo<T>();
            var actions = new List<ActionItem<T>>();
            var first = true;

            var canTuple = mainType == typeof(T);
            object[] values = new object[columns.Count];
            while (reader.Read())
            {
                object objInstance;
                if (canTuple)
                {
                    objInstance = reflection.CreateObject();
                }
                else
                {
                    objInstance = System.Activator.CreateInstance(mainType);
                }
                reader.GetValues(values);
                var detailItem = DataReaderToObj<T>(columns, values, reflection, canTuple, objInstance, typeArry, actions, first, leftColumns) as T;
                list.Add(detailItem);
                first = false;
            }
            reader.Close();
            sw.Stop();
            runTime = sw.ElapsedMilliseconds;
            Console.WriteLine("CRL映射用时:" + runTime);
            return list;
        }

        internal static object DataReaderToObj<T>(Dictionary<string, int> columns, object[] values, ReflectionInfo<T> reflection, bool canTuple, object detailItem, IEnumerable<Attribute.FieldAttribute> typeArry, List<ActionItem<T>> actions, bool first, Dictionary<string, int> leftColumns) where T : class,new()
        {
            IModel obj2 = null;
            if (detailItem is IModel)
            {
                obj2 = detailItem as IModel;
                obj2.BoundChange = false;
            }
            //return detailItem;
            if (first)
            {
                #region foreach field
                foreach (Attribute.FieldAttribute info in typeArry)
                {
                    ActionItem<T> action;
                    string nameLower = info.MappingName.ToLower();
                    if (info.FieldType == Attribute.FieldType.关联字段)//按外部字段
                    {
                        #region 按外部字段
                        string tab = TypeCache.GetTableName(info.ConstraintType, null);
                        string fieldName = info.GetTableFieldFormat(tab, info.ConstraintResultField).ToLower();
                        if (columns.ContainsKey(fieldName))
                        {
                            if (canTuple)
                            {
                                var accessor = reflection.GetAccessor(info.MemberName);
                                action = new ActionItem<T>() { Set = accessor.Set, Name = fieldName, ValueIndex = columns[fieldName] };
                                //actions.Add(new ActionItem<T>() { Action = accessor.Set, Name = fieldName, ValueIndex = columns[fieldName] });
                            }
                            else
                            {
                                action = new ActionItem<T>() { Set = info.SetValue, Name = fieldName, ValueIndex = columns[fieldName] };
                                //actions.Add(new ActionItem<T>() { Action = info.SetValue, Name = fieldName, ValueIndex = columns[fieldName] });
                            }
                            leftColumns.Remove(fieldName);
                            actions.Add(action);
                            object value = values[action.ValueIndex];
                            action.Set((T)detailItem, value);
                        }
                        #endregion
                    }
                    else
                    {
                        #region 按属性
                        if (columns.ContainsKey(nameLower))
                        {
                            if (canTuple)
                            {
                                var accessor = reflection.GetAccessor(info.MemberName);
                                action = new ActionItem<T>() { Set = accessor.Set, Name = nameLower, ValueIndex = columns[nameLower] };
                                //actions.Add(new ActionItem<T>() { Action = accessor.Set, Name = nameLower, ValueIndex = columns[nameLower] });
                            }
                            else
                            {
                                action = new ActionItem<T>() { Set = info.SetValue, Name = nameLower, ValueIndex = columns[nameLower] };
                                //actions.Add(new ActionItem<T>() { Action = info.SetValue, Name = nameLower, ValueIndex = columns[nameLower] });
                            }
                            leftColumns.Remove(nameLower);
                            actions.Add(action);
                            object value = values[action.ValueIndex];
                            action.Set((T)detailItem, value);
                        }
                        #endregion
                    }

                }
                #endregion
            }
            else
            {
                for (int i = 1; i < actions.Count; i++)
                {
                    var item = actions[i];
                    object value = values[item.ValueIndex];
                    item.Set((T)detailItem, value);
                }
            }

            if (obj2 != null && leftColumns.Count > 0)
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
                    obj2[mapingName] = values[item.Value];

                }
                obj2.BoundChange = true;
            }
            return detailItem;
        }

        #endregion
        #region 返回T 按IModel
        internal static List<T> DataReaderToIModelList<T>(DbDataReader reader, out double runTime, bool setConstraintObj = false) where T : IModel, new()
        {
            var mainType=typeof(T);
            var sw = new Stopwatch();
            sw.Start();
            var list = new List<T>();
            var typeArry = TypeCache.GetProperties(mainType, !setConstraintObj).Values;
            var columns = new Dictionary<string, int>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i).ToLower(), i);
            }
            var leftColumns = new Dictionary<string, int>(columns);
            var reflection = ReflectionHelper.GetInfo<T>();
            var actions = new List<ActionItem<T>>();
            var first = true;
            object[] values = new object[columns.Count];
            while (reader.Read())
            {
                reader.GetValues(values);
                var detailItem = DataReaderToIModel<T>(columns, values, reflection, typeArry, actions, first, leftColumns);
                list.Add(detailItem);
                first = false;
            }
            reader.Close();
            sw.Stop();
            runTime = sw.ElapsedMilliseconds;
            Console.WriteLine("CRL映射用时:" + runTime);
            return list;
        }

        internal static T DataReaderToIModel<T>(Dictionary<string, int> columns, object[] values, ReflectionInfo<T> reflection, IEnumerable<Attribute.FieldAttribute> typeArry, List<ActionItem<T>> actions, bool first, Dictionary<string, int> leftColumns) where T : IModel, new()
        {
            T detailItem = reflection.CreateObject();
            detailItem.BoundChange = false;
            //return detailItem;
            if (first)
            {
                #region foreach field
                foreach (Attribute.FieldAttribute info in typeArry)
                {
                    ActionItem<T> action;
                    string nameLower = info.MappingName.ToLower();
                    if (info.FieldType == Attribute.FieldType.关联字段)//按外部字段
                    {
                        #region 按外部字段
                        string tab = TypeCache.GetTableName(info.ConstraintType, null);
                        string fieldName = info.GetTableFieldFormat(tab, info.ConstraintResultField).ToLower();
                        if (columns.ContainsKey(fieldName))
                        {
                            var accessor = reflection.GetAccessor(info.MemberName);
                            action = new ActionItem<T>() { Set = accessor.Set, Name = fieldName, ValueIndex = columns[fieldName] };

                            leftColumns.Remove(fieldName);
                            actions.Add(action);
                            object value = values[action.ValueIndex];
                            action.Set((T)detailItem, value);
                        }
                        #endregion
                    }
                    else
                    {
                        #region 按属性
                        if (columns.ContainsKey(nameLower))
                        {
                            var accessor = reflection.GetAccessor(info.MemberName);
                            action = new ActionItem<T>() { Set = accessor.Set, Name = nameLower, ValueIndex = columns[nameLower] };

                            leftColumns.Remove(nameLower);
                            actions.Add(action);
                            object value = values[action.ValueIndex];
                            action.Set(detailItem, value);
                        }
                        #endregion
                    }

                }
                #endregion
            }
            else
            {
                for (int i = 1; i < actions.Count; i++)
                {
                    var item = actions[i];
                    object value = values[item.ValueIndex];
                    item.Set(detailItem, value);
                }
            }

            if (leftColumns.Count > 0)
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
                    detailItem[mapingName] = values[item.Value];

                }
                detailItem.BoundChange = true;
            }
            return detailItem;
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
