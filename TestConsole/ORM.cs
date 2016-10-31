using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace Loogn.OrmLite
{

    /// <summary>
    /// ORM映射类，从reader到模型
    /// </summary>
    public static class ORM
    {
        /// <summary>
        /// 用reader填充T类型的对象，并返回
        /// </summary>
        /// <typeparam name="T">类型参数</typeparam>
        /// <param name="reader">dataReader</param>
        /// <returns></returns>
        public static T ReaderToObject<T>(DbDataReader reader) 
        {
            if (reader.Read())
            {
                T obj = Activator.CreateInstance<T>();
                var type = typeof(T);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var tuple = ReflectionCache.GetCachedPP<T>(type, reader.GetName(i));
                    SetPropertyValue(tuple, type, obj, reader.GetName(i), reader.GetValue(i));
                }
                return obj;
            }
            else
            {
                return default(T);
            }
        }

        private static void SetPropertyValue<TObject>(Tuple<PropertyInfo, Delegate> tuple, Type objType, TObject obj, string fieldName, object value)
        {
            var fieldType = tuple.Item1.PropertyType;

            if (fieldType == typeof(string))
            {
                ((Action<TObject, string>)tuple.Item2)(obj, (string)value);
            }
            else if (fieldType == typeof(int))
            {
                ((Action<TObject, int>)tuple.Item2)(obj, (int)value);
            }
            else if (fieldType == typeof(int?))
            {
                ((Action<TObject, int?>)tuple.Item2)(obj, (int)value);
            }
            else if (fieldType == typeof(DateTime))
            {
                ((Action<TObject, DateTime>)tuple.Item2)(obj, (DateTime)value);
            }
            else if (fieldType == typeof(DateTime?))
            {
                ((Action<TObject, DateTime?>)tuple.Item2)(obj, (DateTime)value);
            }
            else if (fieldType == typeof(long))
            {
                ((Action<TObject, long>)tuple.Item2)(obj, (long)value);
            }
            else if (fieldType == typeof(long?))
            {
                ((Action<TObject, long?>)tuple.Item2)(obj, (long)value);
            }
            else if (fieldType == typeof(double))
            {
                ((Action<TObject, double>)tuple.Item2)(obj, (double)value);
            }
            else if (fieldType == typeof(double?))
            {
                ((Action<TObject, double?>)tuple.Item2)(obj, (double)value);
            }
            else if (fieldType == typeof(Guid))
            {
                ((Action<TObject, Guid>)tuple.Item2)(obj, (Guid)value);
            }
            else if (fieldType == typeof(Guid?))
            {
                ((Action<TObject, Guid?>)tuple.Item2)(obj, (Guid)value);
            }
            else if (fieldType == typeof(float))
            {
                ((Action<TObject, float>)tuple.Item2)(obj, (float)value);
            }
            else if (fieldType == typeof(float?))
            {
                ((Action<TObject, float?>)tuple.Item2)(obj, (float)value);
            }
            else if (fieldType == typeof(byte))
            {
                ((Action<TObject, byte>)tuple.Item2)(obj, Convert.ToByte(value));
            }
            else if (fieldType == typeof(byte?))
            {
                ((Action<TObject, byte?>)tuple.Item2)(obj, (byte)value);
            }
            else if (fieldType == typeof(char))
            {
                ((Action<TObject, char>)tuple.Item2)(obj, (char)value);
            }
            else if (fieldType == typeof(char?))
            {
                ((Action<TObject, char?>)tuple.Item2)(obj, (char)value);
            }
            else if (fieldType == typeof(bool))
            {
                bool theValue = false;
                if (value is bool)
                {
                    theValue = (bool)value;
                }
                else
                {
                    var intValue = Convert.ToInt32(value);
                    theValue = intValue > 0;
                }
                ((Action<TObject, bool>)tuple.Item2)(obj, theValue);
            }
            else if (fieldType == typeof(bool?))
            {
                ((Action<TObject, bool?>)tuple.Item2)(obj, (bool)value);
            }
            else if (fieldType == typeof(decimal))
            {
                ((Action<TObject, decimal>)tuple.Item2)(obj, (decimal)value);
            }
            else if (fieldType == typeof(decimal?))
            {
                ((Action<TObject, decimal?>)tuple.Item2)(obj, (decimal)value);
            }
            else if (value is byte[])
            {
                ((Action<TObject, byte[]>)tuple.Item2)(obj, (byte[])value);
            }
        }

        internal static T ConvertToType<T>(object obj)
        {
            if (obj == null || obj is DBNull)
            {
                return default(T);
            }
            else
            {
                var type = typeof(T);
                object newobj = obj;
                if (type == typeof(int))
                {
                    newobj = Convert.ToInt32(obj);
                }
                return (T)newobj;

            }
        }

        public static List<T> ReaderToObjectList<T>(DbDataReader reader) 
        {
            if (!reader.HasRows)
            {
                return new List<T>();
            }
            var type = typeof(T);
            List<T> list = new List<T>();
            var first = true;
            Tuple<PropertyInfo, Delegate>[] tupleArr = new Tuple<PropertyInfo, Delegate>[reader.FieldCount];
            string[] nameArr = new string[reader.FieldCount];

            while (reader.Read())
            {
                T obj = Activator.CreateInstance<T>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Tuple<PropertyInfo, Delegate> tuple = null;
                    string fieldName = null;
                    if (first)
                    {
                        tuple = ReflectionCache.GetCachedPP<T>(type, fieldName);
                        tupleArr[i] = tuple;
                        fieldName = reader.GetName(i);
                        nameArr[i] = fieldName;
                    }
                    else
                    {
                        tuple = tupleArr[i];
                        fieldName = nameArr[i];
                    }
                    if (tuple != null)
                    {
                        var value = reader.GetValue(i);
                        if (value == null || value is DBNull)
                        {
                            continue;
                        }
                        SetPropertyValue(tuple, type, obj, fieldName, value);
                    }
                }
                list.Add(obj);
                first = false;
            }

            return list;
        }

        internal static List<MyTuple<T1, T2>> ReaderToTupleList<T1, T2>(DbDataReader reader)
        {
            if (!reader.HasRows) return new List<MyTuple<T1, T2>>();
            var list = new List<MyTuple<T1, T2>>();
            while (reader.Read())
            {
                var tuple = new MyTuple<T1, T2>();
                tuple.Item1 = (T1)reader[0];
                tuple.Item2 = (T2)reader[1];
                list.Add(tuple);
            }
            return list;
        }

        public static List<T> ReaderToColumnList<T>(DbDataReader reader)
        {
            if (!reader.HasRows) return new List<T>();
            List<T> list = new List<T>();
            while (reader.Read())
            {
                list.Add(ConvertToType<T>(reader[0]));
            }
            return list;
        }

        public static HashSet<T> ReaderToColumnSet<T>(DbDataReader reader)
        {
            if (!reader.HasRows) return new HashSet<T>();
            HashSet<T> set = new HashSet<T>();
            while (reader.Read())
            {
                set.Add(ConvertToType<T>(reader[0]));
            }
            return set;
        }

        public static dynamic ReaderToDynamic(DbDataReader reader)
        {
            if (reader.Read())
            {
                dynamic obj = new ExpandoObject();
                var dict = obj as IDictionary<string, object>;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dict.Add(reader.GetName(i), reader.GetValue(i));
                }
                return obj;
            }
            else
            {
                return null;
            }
        }

        public static List<dynamic> ReaderToDynamicList(DbDataReader reader)
        {
            if (!reader.HasRows)
            {
                return new List<dynamic>();
            }
            List<dynamic> list = new List<dynamic>();
            while (reader.Read())
            {
                dynamic obj = new ExpandoObject();
                var dict = obj as IDictionary<string, object>;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dict.Add(reader.GetName(i), reader.GetValue(i));
                }
                list.Add(obj);
            }
            return list;
        }

        private static void ReaderToJson(DbDataReader reader, StringBuilder result)
        {
            result.Append("{");
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var val = reader.GetValue(i);
                if (val is DBNull)
                {
                    result.AppendFormat("\"{0}\":null", reader.GetName(i));
                }
                else
                {
                    var type = val.GetType();
                    if (type == typeof(DateTime) || type == typeof(string))
                    {
                        result.AppendFormat("\"{0}\":\"{1}\"", reader.GetName(i), val.ToString());
                    }
                    else if (type == typeof(bool))
                    {
                        result.AppendFormat("\"{0}\":{1}", reader.GetName(i), true.Equals(val) ? "true" : "false");
                    }
                    else
                    {
                        result.AppendFormat("\"{0}\":{1}", reader.GetName(i), val.ToString());
                    }
                }

                if (i < reader.FieldCount - 1)
                {
                    result.Append(",");
                }
            }
            result.Append("}");
        }

        public static string ReaderToJsonArray(DbDataReader reader)
        {
            StringBuilder json = new StringBuilder(500);
            while (reader.Read())
            {
                ReaderToJson(reader, json);
                json.Append(",");
            }
            if (json.Length == 0)
            {
                json.Append("[]");
            }
            else
            {
                json.Remove(json.Length - 1, 1);
                json.Insert(0, "[");
                json.Append("]");
            }
            return json.ToString();
        }

        public static string ReaderToJsonObject(DbDataReader reader)
        {
            if (reader.Read())
            {
                StringBuilder sb = new StringBuilder(100);
                ReaderToJson(reader, sb);
                return sb.ToString();
            }
            else
            {
                return "{}";
            }
        }



        public static Dictionary<K, List<V>> ReaderToLookup<K, V>(DbDataReader reader)
        {
            if (!reader.HasRows) return new Dictionary<K, List<V>>();
            var list = ReaderToTupleList<K, V>(reader);
            var dict = new Dictionary<K, List<V>>(list.Count / 2);
            foreach (var tuple in list)
            {
                List<V> value = null;
                if (!dict.TryGetValue(tuple.Item1, out value))
                {
                    value = new List<V>();
                    dict.Add(tuple.Item1, value);
                }
                value.Add(tuple.Item2);
            }
            return dict;
        }

        public static Dictionary<K, V> ReaderToDictionary<K, V>(DbDataReader reader)
        {
            if (!reader.HasRows) return new Dictionary<K, V>();
            var list = ReaderToTupleList<K, V>(reader);
            var dict = new Dictionary<K, V>(list.Count);
            foreach (var tuple in list)
            {
                dict[tuple.Item1] = tuple.Item2;
            }
            return dict;
        }


    }
    internal class MyTuple<T1, T2>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
    }

    internal enum PartSqlType
    {
        Select,
        Count
    }

}
