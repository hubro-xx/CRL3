using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
                convertMethod.Add(typeof(Enum), (a) =>
                {
                    return Convert.ToInt32(a);
                });
            }
            if (convertMethod.ContainsKey(type))
            {
                return convertMethod[type](value);
            }
            if (type.BaseType == typeof(Enum))
            {
                return convertMethod[type.BaseType](value);
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
        internal static List<TItem> DataReaderToList<TItem>(DbDataReader reader, Type mainType,out double runTime, bool setConstraintObj = false) where TItem : class, new()
        {
            var time = DateTime.Now;
            var list = new List<TItem>();
            var typeArry = TypeCache.GetProperties(mainType, !setConstraintObj).Values;
            while (reader.Read())
            {
                var detailItem = DataReaderToObj(reader, mainType, typeArry) as TItem;
                list.Add(detailItem);
            }
            reader.Close();
            var ts = DateTime.Now - time;
            runTime = ts.TotalMilliseconds;
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="mainType"></param>
        /// <param name="typeArry"></param>
        /// <returns></returns>
        internal static object DataReaderToObj(DbDataReader reader, Type mainType, IEnumerable<Attribute.FieldAttribute> typeArry)
        {
            object detailItem = System.Activator.CreateInstance(mainType);
            var columns = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i).ToLower());
            }
            IModel obj2 = null;
            if (detailItem is IModel)
            {
                obj2 = detailItem as IModel;
                obj2.BoundChange = false;
            }
            foreach (Attribute.FieldAttribute info in typeArry)
            {
                if (info.FieldType == Attribute.FieldType.关联字段)//按外部字段
                {
                    #region 按外部字段
                    string tab = TypeCache.GetTableName(info.ConstraintType,null);
                    string fieldName = info.GetTableFieldFormat(tab, info.ConstraintResultField);
                    var value = reader[fieldName];
                    info.SetValue(detailItem, value);
                    if (obj2 != null)
                    {
                        obj2[info.Name] = value;
                        columns.Remove(info.Name.ToLower());
                    }
                    #endregion
                }
                else if (info.FieldType == Attribute.FieldType.关联对象)//按动态实例
                {
                    #region 按动态实例
                    Type type = info.PropertyType;
                    object oleObject = System.Activator.CreateInstance(type);
                    string tableName = TypeCache.GetTableName(type,null);
                    var typeArry2 = TypeCache.GetProperties(type, true).Values;
                    foreach (Attribute.FieldAttribute info2 in typeArry2)
                    {
                        string fieldName = info2.MapingName;
                        object value = reader[fieldName];
                        info2.SetValue(oleObject, value);
                        if (obj2 != null)
                        {
                            obj2[info2.Name] = value;
                            columns.Remove(info2.Name.ToLower());
                        }
                    }
                    info.SetValue(detailItem, oleObject);
                    #endregion
                }
                else
                {
                    #region 按属性
                    if (!columns.Contains(info.Name.ToLower()))
                    {
                        continue;
                    }
                    object value = reader[info.Name];
                    columns.Remove(info.Name.ToLower());
                    info.SetValue(detailItem, value);
                    #endregion
                }
            }
            if (obj2 != null)
            {
                var b = obj2.BoundChange;
                //没有找到属性的列放入索引,按别名
                foreach (var col in columns)
                {
                    var n = col.LastIndexOf("__");
                    if (n == -1)
                    {
                        continue;
                    }
                    var mapingName = col.Substring(n+2);
                    obj2[mapingName] = reader[col];
                }
                //if (fieldMapping != null)//由lambdaQuery创建
                //{
                //    foreach (var name in fieldMapping.Keys)
                //    {
                //        obj2[name] = reader[fieldMapping[name].ToString()];
                //    }
                //}
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
                var keyValue = (item as IModel).GetpPrimaryKeyValue();
                dic.Add(keyValue, item as TItem);
            }
            return dic;
        }
    }
}
